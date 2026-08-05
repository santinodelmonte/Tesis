using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tesis.Dominio;

namespace Tesis.Pages.PagesAnimal
{
    public class ListaAnimalesModel : PageModel
    {
        public List<Animal> animales = new List<Animal>();

        public void OnGet()
        {
            Controladora unaControladora = new Controladora();
            unaControladora.ListarAnimales();

            // El listado muestra el rodeo actual: los animales dados de baja se consultan desde Buscar y Filtrar
            animales = unaControladora.FiltrarAnimalesXEstado(true);
        }
    }
}
