using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tesis.Dominio;
using System.ComponentModel.DataAnnotations;

namespace Tesis.Pages.PagesAnimal
{
    public class ModificarAnimalModel : PageModel
    {
        public Animal animal { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "El ID es requerido")]
        public int id { get; set; } = 1;
        [BindProperty]
        [Required(ErrorMessage = "El número de caravana es requerido")]
        public string numCaravana { get; set; } = "";
        [BindProperty]
        [Required(ErrorMessage = "La fecha de nacimiento es requerida")]
        public DateTime fechaNacimiento { get; set; } = DateTime.Now;
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

        // Lo tilda el boton "Guardar de todos modos": llega en verdadero solo cuando el
        // usuario ya vio las advertencias de genealogia y decidio guardar igual.
        [BindProperty]
        public bool confirmado { get; set; } = false;

        public List<Raza> razas = new List<Raza>();
        public List<Categoria> categorias = new List<Categoria>();
        public List<Hembra> hembras = new List<Hembra>();
        public List<Macho> machos = new List<Macho>();
        public List<Animal> animales = new List<Animal>();

        // Motivos que no impiden la modificacion pero que el usuario tiene que ver antes
        public List<string> advertencias = new List<string>();

        // Caravanas de la madre y del padre elegidos, para mostrarlas en el formulario
        public string caravanaMadre = "";
        public string caravanaPadre = "";

        public void OnGet(int id)
        {
            Controladora unaControladora = new Controladora();
            this.CargarListas(unaControladora);
            animal = unaControladora.BuscarAnimal(id);

            if (animal != null)
            {
                // El formulario se dibuja a partir de estas propiedades y no del animal
                // de la cache, para que un error de validacion no borre lo tipeado.
                this.id = animal.IdAnimal;
                numCaravana = animal.NumCaravana;
                fechaNacimiento = animal.FechaNacimiento;
                idRaza = animal.Raza != null ? animal.Raza.IdRaza : 0;
                idCategoria = animal.Categoria != null ? animal.Categoria.IdCategoria : 0;
                idMadre = animal.Madre != null ? animal.Madre.IdAnimal : 0;
                idPadre = animal.Padre != null ? animal.Padre.IdAnimal : 0;
                caravanaMadre = animal.Madre != null ? animal.Madre.NumCaravana : "";
                caravanaPadre = animal.Padre != null ? animal.Padre.NumCaravana : "";

                if (animal is Hembra)
                {
                    numeroPartos = ((Hembra)animal).NumeroPartos;
                }
                else
                {
                    enPie = ((Macho)animal).EnPie;
                }
            }
        }

        // RF1.9: propone la categoria que corresponde a los datos que hay en el
        // formulario, sin guardarlos todavia.
        public void OnPostCalcularCategoria()
        {
            Controladora unaControladora = new Controladora();
            this.CargarListas(unaControladora);

            this.LeerFormulario();
            animal = unaControladora.BuscarAnimal(id);
            this.LeerCaravanasElegidas(unaControladora);

            // Sin esto el formulario vuelve a mostrar la categoria que se envio, no la calculada
            ModelState.Clear();

            if (animal != null)
            {
                Animal unAnimalDelFormulario = this.ArmarAnimalDelFormulario(unaControladora);
                Categoria unaCategoria = unaControladora.CalcularCategoria(unAnimalDelFormulario);
                if (unaCategoria != null)
                {
                    idCategoria = unaCategoria.IdCategoria;
                }
            }
        }

        public IActionResult OnPostModificarAnimal()
        {
            Controladora unaControladora = new Controladora();
            this.CargarListas(unaControladora);

            this.LeerFormulario();
            animal = unaControladora.BuscarAnimal(id);
            this.LeerCaravanasElegidas(unaControladora);

            Raza unaRaza = unaControladora.BuscarRaza(idRaza);
            Categoria unaCategoria = unaControladora.BuscarCategoria(idCategoria);
            Hembra unaMadre = unaControladora.BuscarHembra(idMadre);
            Macho unPadre = unaControladora.BuscarMacho(idPadre);

            if (numCaravana == "" || unaRaza == null || unaCategoria == null)
            {
                ModelState.AddModelError(string.Empty, "El número de caravana, la raza y la categoría son obligatorios!");
                return Page();
            }

            if (fechaNacimiento > DateTime.Now)
            {
                ModelState.AddModelError(string.Empty, "La fecha de nacimiento no puede ser futura!");
                return Page();
            }

            // La genealogia se valida antes de guardar para poder informar el motivo
            string vMotivo = unaControladora.ValidarGenealogia(id, fechaNacimiento, unaMadre, unPadre);
            if (vMotivo != "")
            {
                ModelState.AddModelError(string.Empty, vMotivo);
                return Page();
            }

            // Advertencias que no impiden la modificacion: un progenitor que ya figuraba
            // dado de baja, o padre y madre emparentados entre si.
            advertencias = unaControladora.AdvertenciasGenealogia(fechaNacimiento, unaMadre, unPadre);
            if (advertencias.Count > 0 && !confirmado)
            {
                return Page();
            }

            if (unaControladora.ModificarAnimal(id, numCaravana, fechaNacimiento, unaRaza, unaCategoria,
                unaMadre, unPadre, numeroPartos, enPie))
            {
                return Redirect("./ListaAnimales");
            }

            ModelState.AddModelError(string.Empty, "No se pudo modificar el animal. Verifique que el número de caravana no esté repetido!");
            return Page();
        }

        private void CargarListas(Controladora pControladoraDominio)
        {
            // Se recargan porque la modificacion se resuelve contra la cache
            razas = pControladoraDominio.ListarRazas();
            categorias = pControladoraDominio.ListarCategorias();
            hembras = pControladoraDominio.ListarHembras();
            machos = pControladoraDominio.ListarMachos();
            animales = pControladoraDominio.ListarAnimales();
        }

        private void LeerFormulario()
        {
            // Los select y las fechas pueden llegar vacios, por eso se comparan antes de convertir
            id = Request.Form["id"] != "" ? Convert.ToInt32(Request.Form["id"]) : 0;
            numCaravana = Request.Form["numCaravana"];
            fechaNacimiento = Request.Form["fechaNacimiento"] != "" ? Convert.ToDateTime(Request.Form["fechaNacimiento"]) : DateTime.Now;
            idRaza = Request.Form["idRaza"] != "" ? Convert.ToInt32(Request.Form["idRaza"]) : 0;
            idCategoria = Request.Form["idCategoria"] != "" ? Convert.ToInt32(Request.Form["idCategoria"]) : 0;
            idMadre = Request.Form["idMadre"] != "" ? Convert.ToInt32(Request.Form["idMadre"]) : 0;
            idPadre = Request.Form["idPadre"] != "" ? Convert.ToInt32(Request.Form["idPadre"]) : 0;
            numeroPartos = Request.Form["numeroPartos"] != "" ? Convert.ToInt32(Request.Form["numeroPartos"]) : 0;
            // El checkbox manda "true,false" cuando esta tildado
            enPie = Request.Form["enPie"].ToString().Contains("true");

            // Solo viaja cuando se apreto el boton de confirmacion: si el usuario
            // corrige un progenitor, las advertencias se evaluan de nuevo.
            confirmado = Request.Form["confirmado"].ToString().Contains("true");
        }

        private void LeerCaravanasElegidas(Controladora pControladoraDominio)
        {
            // El selector guarda el id, pero el formulario tiene que volver a mostrar la caravana
            Hembra unaMadre = pControladoraDominio.BuscarHembra(idMadre);
            Macho unPadre = pControladoraDominio.BuscarMacho(idPadre);

            caravanaMadre = unaMadre != null ? unaMadre.NumCaravana : "";
            caravanaPadre = unPadre != null ? unPadre.NumCaravana : "";
        }

        // Copia del animal con los datos que hay en el formulario, para calcular la
        // categoria sobre lo que la usuaria esta editando y no sobre lo guardado.
        private Animal ArmarAnimalDelFormulario(Controladora pControladoraDominio)
        {
            Raza unaRaza = pControladoraDominio.BuscarRaza(idRaza);
            Categoria unaCategoria = pControladoraDominio.BuscarCategoria(idCategoria);
            Hembra unaMadre = pControladoraDominio.BuscarHembra(idMadre);
            Macho unPadre = pControladoraDominio.BuscarMacho(idPadre);

            if (animal is Hembra)
            {
                Hembra unaHembra = (Hembra)animal;
                return new Hembra(id, numCaravana, fechaNacimiento, animal.Activo,
                    animal.FechaBaja, animal.MotivoBaja, unaRaza, unaCategoria, unaMadre, unPadre,
                    numeroPartos, unaHembra.EstadoProductivo, unaHembra.EstadoReproductivo);
            }

            return new Macho(id, numCaravana, fechaNacimiento, animal.Activo,
                animal.FechaBaja, animal.MotivoBaja, unaRaza, unaCategoria, unaMadre, unPadre, enPie);
        }
    }
}
