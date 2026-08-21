using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tesis.Dominio;

namespace Tesis.Pages.PagesAnimal
{
    public class ListaAnimalesModel : PageModel
    {
        public List<Animal> animales = new List<Animal>();

        // Los toros de catalogo van en una tabla aparte: aportan pajuelas y no integran
        // el rodeo, asi que mezclarlos con los animales del campo desvirtua el conteo y
        // les ofrece acciones que no tienen sentido, como darlos de baja.
        public List<Animal> reproductoresCatalogo = new List<Animal>();

        // RF1.9: animales cuya categoria guardada ya no es la que les corresponde por
        // edad o por partos, con el nombre de la que propone el sistema.
        public Dictionary<int, string> categoriasDesactualizadas = new Dictionary<int, string>();

        public void OnGet()
        {
            this.CargarListado();
        }

        public IActionResult OnPostActualizarCategoria(int id)
        {
            Controladora unaControladora = new Controladora();
            unaControladora.ListarAnimales();
            unaControladora.ActualizarCategoria(id);

            return RedirectToPage();
        }

        private void CargarListado()
        {
            Controladora unaControladora = new Controladora();
            unaControladora.ListarAnimales();

            // El listado muestra el rodeo actual: los animales dados de baja se consultan desde Buscar y Filtrar
            animales = unaControladora.ListarRodeo();
            reproductoresCatalogo = unaControladora.ListarReproductoresDeCatalogo();

            foreach (Animal unAnimal in animales)
            {
                if (!unaControladora.AplicaCategoria(unAnimal.Categoria, unAnimal))
                {
                    Categoria unaCategoria = unaControladora.CalcularCategoria(unAnimal);
                    if (unaCategoria != null)
                    {
                        categoriasDesactualizadas.Add(unAnimal.IdAnimal, unaCategoria.Nombre);
                    }
                }
            }
        }
    }
}
