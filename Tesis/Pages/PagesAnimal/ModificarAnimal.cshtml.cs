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

        public List<Raza> razas = new List<Raza>();
        public List<Categoria> categorias = new List<Categoria>();
        public List<Hembra> hembras = new List<Hembra>();
        public List<Macho> machos = new List<Macho>();
        public List<Animal> animales = new List<Animal>();

        // Caravanas de la madre y del padre elegidos, para mostrarlas en el formulario
        public string caravanaMadre = "";
        public string caravanaPadre = "";

        public void OnGet(int id)
        {
            Controladora unaControladora = new Controladora();
            razas = unaControladora.ListarRazas();
            categorias = unaControladora.ListarCategorias();
            hembras = unaControladora.ListarHembras();
            machos = unaControladora.ListarMachos();
            animales = unaControladora.ListarAnimales();
            animal = unaControladora.BuscarAnimal(id);

            if (animal != null)
            {
                idMadre = animal.Madre != null ? animal.Madre.IdAnimal : 0;
                idPadre = animal.Padre != null ? animal.Padre.IdAnimal : 0;
                caravanaMadre = animal.Madre != null ? animal.Madre.NumCaravana : "";
                caravanaPadre = animal.Padre != null ? animal.Padre.NumCaravana : "";
            }
        }

        public IActionResult OnPostModificarAnimal()
        {
            Controladora unaControladora = new Controladora();

            // Se recargan las listas porque la modificacion se resuelve contra la cache
            razas = unaControladora.ListarRazas();
            categorias = unaControladora.ListarCategorias();
            hembras = unaControladora.ListarHembras();
            machos = unaControladora.ListarMachos();
            animales = unaControladora.ListarAnimales();

            // Los select y las fechas pueden llegar vacios, por eso se comparan antes de convertir
            id = Request.Form["id"] != "" ? Convert.ToInt32(Request.Form["id"]) : 0;
            numCaravana = Request.Form["numCaravana"];
            fechaNacimiento = Request.Form["fechaNacimiento"] != "" ? Convert.ToDateTime(Request.Form["fechaNacimiento"]) : DateTime.Now;
            idRaza = Request.Form["idRaza"] != "" ? Convert.ToInt32(Request.Form["idRaza"]) : 0;
            idCategoria = Request.Form["idCategoria"] != "" ? Convert.ToInt32(Request.Form["idCategoria"]) : 0;
            idMadre = Request.Form["idMadre"] != "" ? Convert.ToInt32(Request.Form["idMadre"]) : 0;
            idPadre = Request.Form["idPadre"] != "" ? Convert.ToInt32(Request.Form["idPadre"]) : 0;

            animal = unaControladora.BuscarAnimal(id);

            Raza unaRaza = unaControladora.BuscarRaza(idRaza);
            Categoria unaCategoria = unaControladora.BuscarCategoria(idCategoria);
            Hembra unaMadre = unaControladora.BuscarHembra(idMadre);
            Macho unPadre = unaControladora.BuscarMacho(idPadre);

            // El selector guarda el id, pero el formulario tiene que volver a mostrar la caravana
            caravanaMadre = unaMadre != null ? unaMadre.NumCaravana : "";
            caravanaPadre = unPadre != null ? unPadre.NumCaravana : "";

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

            if (unaControladora.ModificarAnimal(id, numCaravana, fechaNacimiento, unaRaza, unaCategoria, unaMadre, unPadre))
            {
                return Redirect("./ListaAnimales");
            }

            ModelState.AddModelError(string.Empty, "No se pudo modificar el animal. Verifique que el número de caravana no esté repetido!");
            return Page();
        }
    }
}
