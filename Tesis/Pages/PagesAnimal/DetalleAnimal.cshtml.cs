using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tesis.Dominio;

namespace Tesis.Pages.PagesAnimal
{
    public class DetalleAnimalModel : PageModel
    {
        public Animal animal { get; set; }
        public int edadMeses = 0;

        // RF1.9: nombre de la categoria que corresponde, si la guardada quedo vieja
        public string categoriaSugerida = "";

        public void OnGet(int id)
        {
            this.CargarDetalle(id);
        }

        public IActionResult OnPostActualizarCategoria(int id)
        {
            Controladora unaControladora = new Controladora();
            unaControladora.ListarAnimales();
            unaControladora.ActualizarCategoria(id);

            return RedirectToPage(new { id = id });
        }

        private void CargarDetalle(int pId)
        {
            Controladora unaControladora = new Controladora();
            unaControladora.ListarAnimales();
            animal = unaControladora.BuscarAnimal(pId);

            if (animal != null)
            {
                edadMeses = unaControladora.CalcularEdadMeses(animal);

                if (!unaControladora.AplicaCategoria(animal.Categoria, animal))
                {
                    Categoria unaCategoria = unaControladora.CalcularCategoria(animal);
                    if (unaCategoria != null)
                    {
                        categoriaSugerida = unaCategoria.Nombre;
                    }
                }
            }
        }
    }
}
