using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tesis.Dominio;
using System.ComponentModel.DataAnnotations;

namespace Tesis.Pages.PagesAnimal
{
    public class AltaAnimalModel : PageModel
    {
        [BindProperty]
        [Required(ErrorMessage = "El número de caravana es requerido")]
        public string numCaravana { get; set; } = "";
        [BindProperty]
        [Required(ErrorMessage = "La fecha de nacimiento es requerida")]
        public DateTime fechaNacimiento { get; set; } = DateTime.Now;
        [BindProperty]
        [Required(ErrorMessage = "El sexo es requerido")]
        public string sexo { get; set; } = "H";
        [BindProperty]
        [Range(1, int.MaxValue, ErrorMessage = "La raza es requerida")]
        public int idRaza { get; set; } = 0;
        [BindProperty]
        public int idCategoria { get; set; } = 0;
        [BindProperty]
        public int idMadre { get; set; } = 0;
        [BindProperty]
        public int idPadre { get; set; } = 0;
        [BindProperty]
        public int numeroPartos { get; set; } = 0;
        [BindProperty]
        public bool enPie { get; set; } = false;

        public List<Raza> razas = new List<Raza>();
        public List<Categoria> categorias = new List<Categoria>();
        public List<Hembra> hembras = new List<Hembra>();
        public List<Macho> machos = new List<Macho>();
        public List<Animal> animales = new List<Animal>();

        // Caravanas de la madre y del padre elegidos, para mostrarlas en el formulario
        public string caravanaMadre = "";
        public string caravanaPadre = "";

        public void OnGet()
        {
            Controladora unaControladora = new Controladora();
            razas = unaControladora.ListarRazas();
            categorias = unaControladora.ListarCategorias();
            hembras = unaControladora.ListarHembras();
            machos = unaControladora.ListarMachos();
            animales = unaControladora.ListarAnimales();
        }

        public void OnPostCalcularCategoria()
        {
            Controladora unaControladora = new Controladora();

            // Recargar las listas para los select
            razas = unaControladora.ListarRazas();
            categorias = unaControladora.ListarCategorias();
            hembras = unaControladora.ListarHembras();
            machos = unaControladora.ListarMachos();
            animales = unaControladora.ListarAnimales();

            this.LeerFormulario();
            this.LeerCaravanasElegidas(unaControladora);

            // Sin esto el formulario vuelve a mostrar la categoria que se envio, no la calculada
            ModelState.Clear();

            Animal unAnimal = this.ArmarAnimal(unaControladora);
            Categoria unaCategoria = unaControladora.CalcularCategoria(unAnimal);
            if (unaCategoria != null)
            {
                idCategoria = unaCategoria.IdCategoria;
            }
        }

        public IActionResult OnPostAgregarAnimal()
        {
            Controladora unaControladora = new Controladora();

            // Se recargan las listas porque la categoria se resuelve contra la cache
            razas = unaControladora.ListarRazas();
            categorias = unaControladora.ListarCategorias();
            hembras = unaControladora.ListarHembras();
            machos = unaControladora.ListarMachos();
            animales = unaControladora.ListarAnimales();

            this.LeerFormulario();
            this.LeerCaravanasElegidas(unaControladora);

            if (numCaravana == "" || idRaza == 0)
            {
                ModelState.AddModelError(string.Empty, "El número de caravana y la raza son obligatorios!");
                return Page();
            }

            if (fechaNacimiento > DateTime.Now)
            {
                ModelState.AddModelError(string.Empty, "La fecha de nacimiento no puede ser futura!");
                return Page();
            }

            if (unaControladora.ExisteCaravana(numCaravana))
            {
                ModelState.AddModelError(string.Empty, "El número de caravana ya existe en el sistema!");
                return Page();
            }

            Animal unAnimal = this.ArmarAnimal(unaControladora);

            // La genealogia se valida antes de guardar para poder informar el motivo
            string vMotivo = unaControladora.ValidarGenealogia(unAnimal.IdAnimal, unAnimal.FechaNacimiento,
                unAnimal.Madre, unAnimal.Padre);
            if (vMotivo != "")
            {
                ModelState.AddModelError(string.Empty, vMotivo);
                return Page();
            }

            // Si el usuario no ajusto la categoria, se usa la que propone el sistema
            if (unAnimal.Categoria == null)
            {
                unAnimal.Categoria = unaControladora.CalcularCategoria(unAnimal);
            }

            if (unAnimal.Categoria != null && unAnimal.Raza != null)
            {
                if (unaControladora.AltaAnimal(unAnimal))
                {
                    return Redirect("/PagesAnimal/ListaAnimales");
                }
            }

            ModelState.AddModelError(string.Empty, "No se pudo registrar el animal!");
            return Page();
        }

        private void LeerFormulario()
        {
            // Los select y las fechas pueden llegar vacios, por eso se comparan antes de convertir
            numCaravana = Request.Form["numCaravana"];
            fechaNacimiento = Request.Form["fechaNacimiento"] != "" ? Convert.ToDateTime(Request.Form["fechaNacimiento"]) : DateTime.Now;
            sexo = Request.Form["sexo"] != "" ? Request.Form["sexo"] : "H";
            idRaza = Request.Form["idRaza"] != "" ? Convert.ToInt32(Request.Form["idRaza"]) : 0;
            idMadre = Request.Form["idMadre"] != "" ? Convert.ToInt32(Request.Form["idMadre"]) : 0;
            idPadre = Request.Form["idPadre"] != "" ? Convert.ToInt32(Request.Form["idPadre"]) : 0;
            idCategoria = Request.Form["idCategoria"] != "" ? Convert.ToInt32(Request.Form["idCategoria"]) : 0;
            numeroPartos = Request.Form["numeroPartos"] != "" ? Convert.ToInt32(Request.Form["numeroPartos"]) : 0;
            // El checkbox manda "true,false" cuando esta tildado
            enPie = Request.Form["enPie"].ToString().Contains("true");
        }

        private void LeerCaravanasElegidas(Controladora pControladoraDominio)
        {
            // El selector guarda el id, pero el formulario tiene que volver a mostrar la caravana
            Hembra unaMadre = pControladoraDominio.BuscarHembra(idMadre);
            Macho unPadre = pControladoraDominio.BuscarMacho(idPadre);

            caravanaMadre = unaMadre != null ? unaMadre.NumCaravana : "";
            caravanaPadre = unPadre != null ? unPadre.NumCaravana : "";
        }

        private Animal ArmarAnimal(Controladora pControladoraDominio)
        {
            Raza unaRaza = pControladoraDominio.BuscarRaza(idRaza);
            Categoria unaCategoria = pControladoraDominio.BuscarCategoria(idCategoria);
            Hembra unaMadre = pControladoraDominio.BuscarHembra(idMadre);
            Macho unPadre = pControladoraDominio.BuscarMacho(idPadre);

            // El id va en cero: lo asigna la base al insertar y la persistencia lo
            // devuelve cargado en el objeto.
            if (sexo == "H")
            {
                return new Hembra(0, numCaravana, fechaNacimiento, true, DateTime.MinValue, "",
                    unaRaza, unaCategoria, unaMadre, unPadre, numeroPartos,
                    Hembra.SIN_LACTANCIA, Hembra.VACIA);
            }

            return new Macho(0, numCaravana, fechaNacimiento, true, DateTime.MinValue, "",
                unaRaza, unaCategoria, unaMadre, unPadre, enPie);
        }
    }
}
