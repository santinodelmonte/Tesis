using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tesis.Dominio;
using System.ComponentModel.DataAnnotations;

namespace Tesis.Pages.PagesAnimal
{
    public class ConsultaLinajeModel : PageModel
    {
        // La caravana llega del selector modal, por eso se valida en el handler y no con atributos
        [BindProperty]
        public string? numCaravana { get; set; } = "";

        public Animal animal { get; set; }
        public List<Animal> animales = new List<Animal>();

        public void OnGet()
        {
            Controladora unaControladora = new Controladora();
            animales = unaControladora.ListarAnimales();
        }

        public void OnPostConsultarLinaje()
        {
            Controladora unaControladora = new Controladora();

            // Recargar la lista de animales para el select
            animales = unaControladora.ListarAnimales();

            numCaravana = Request.Form["numCaravana"];

            if (numCaravana != null && numCaravana != "")
            {
                // El recorrido de la ascendencia se resuelve en memoria sobre la cache
                animal = unaControladora.ObtenerLinaje(numCaravana);
            }

            if (animal == null)
            {
                ModelState.AddModelError(string.Empty, "Seleccione un animal!");
            }
        }
    }
}
