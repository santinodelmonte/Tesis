using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tesis.Dominio;

namespace Tesis.Pages.PagesAnimal
{
    public class EliminarAnimalModel : PageModel
    {
        public Animal animal { get; set; }

        public void OnGet(int id)
        {
            Controladora unaControladora = new Controladora();
            unaControladora.ListarAnimales();
            animal = unaControladora.BuscarAnimal(id);
        }

        public IActionResult OnPostEliminarAnimal()
        {
            int id = Convert.ToInt32(Request.Form["id"]);

            Controladora unaControladora = new Controladora();
            unaControladora.ListarAnimales();
            animal = unaControladora.BuscarAnimal(id);

            // El borrado fisico solo se permite si el animal no es progenitor de otro
            if (unaControladora.EliminarAnimal(id))
            {
                return Redirect("./ListaAnimales");
            }

            ModelState.AddModelError(string.Empty, "No se puede eliminar el animal porque figura como padre o madre de otro animal. Utilice la baja lógica!");
            return Page();
        }
    }
}
