using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tesis.Dominio;

namespace Tesis.Pages.PagesAnimal
{
    public class DetalleAnimalModel : PageModel
    {
        public Animal animal { get; set; }
        public int edadMeses = 0;

        public void OnGet(int id)
        {
            Controladora unaControladora = new Controladora();
            unaControladora.ListarAnimales();
            animal = unaControladora.BuscarAnimal(id);

            if (animal != null)
            {
                edadMeses = unaControladora.CalcularEdadMeses(animal);
            }
        }
    }
}
