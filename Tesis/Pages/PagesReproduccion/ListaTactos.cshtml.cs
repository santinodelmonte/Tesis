using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tesis.Dominio;

namespace Tesis.Pages.PagesReproduccion
{
    // Historial de tactos. Antes los controles solo se veian de a uno, como el
    // "ultimo tacto" de cada servicio: no habia donde mirarlos todos ni, por lo tanto,
    // donde corregir el que se hubiera cargado mal.
    public class ListaTactosModel : PageModel
    {
        public List<Tacto> tactos = new List<Tacto>();

        public void OnGet()
        {
            Controladora unaControladora = new Controladora();
            tactos = unaControladora.ListarTactos();
        }

        public IActionResult OnPostEliminar(int id)
        {
            Controladora unaControladora = new Controladora();

            string vMotivo = unaControladora.ValidarEliminarTacto(id);
            if (vMotivo != "")
            {
                tactos = unaControladora.ListarTactos();
                ModelState.AddModelError(string.Empty, vMotivo);
                return Page();
            }

            if (!unaControladora.EliminarTacto(id))
            {
                tactos = unaControladora.ListarTactos();
                ModelState.AddModelError(string.Empty, "No se pudo eliminar el tacto!");
                return Page();
            }

            return RedirectToPage();
        }
    }
}
