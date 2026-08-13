using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tesis.Dominio;

namespace Tesis.Pages.PagesReproduccion
{
    // Celos detectados. Es la pantalla desde la que se gestionan: se registra uno
    // nuevo, se corrige el que quedo mal cargado y se elimina el que no correspondia.
    public class ListaCelosModel : PageModel
    {
        public List<Celo> celos = new List<Celo>();

        public void OnGet()
        {
            Controladora unaControladora = new Controladora();
            celos = unaControladora.ListarCelos();
        }

        public IActionResult OnPostEliminar(int id)
        {
            Controladora unaControladora = new Controladora();

            string vMotivo = unaControladora.ValidarEliminarCelo(id);
            if (vMotivo != "")
            {
                celos = unaControladora.ListarCelos();
                ModelState.AddModelError(string.Empty, vMotivo);
                return Page();
            }

            if (!unaControladora.EliminarCelo(id))
            {
                celos = unaControladora.ListarCelos();
                ModelState.AddModelError(string.Empty, "No se pudo eliminar el celo!");
                return Page();
            }

            return RedirectToPage();
        }
    }
}
