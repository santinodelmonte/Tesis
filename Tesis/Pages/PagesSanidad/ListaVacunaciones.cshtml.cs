using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tesis.Dominio;

namespace Tesis.Pages.PagesSanidad
{
    // Vacunaciones aplicadas. Antes se veian al pie de la pantalla que las registraba,
    // que no era el lugar para gestionarlas.
    public class ListaVacunacionesModel : PageModel
    {
        public List<Vacunacion> vacunaciones = new List<Vacunacion>();

        public void OnGet()
        {
            Controladora unaControladora = new Controladora();
            vacunaciones = unaControladora.ListarVacunaciones();
        }

        public IActionResult OnPostEliminar(int id)
        {
            Controladora unaControladora = new Controladora();

            string vMotivo = unaControladora.ValidarEliminarVacunacion(id);
            if (vMotivo != "")
            {
                vacunaciones = unaControladora.ListarVacunaciones();
                ModelState.AddModelError(string.Empty, vMotivo);
                return Page();
            }

            if (!unaControladora.EliminarVacunacion(id))
            {
                vacunaciones = unaControladora.ListarVacunaciones();
                ModelState.AddModelError(string.Empty, "No se pudo eliminar la vacunacion!");
                return Page();
            }

            return RedirectToPage();
        }
    }
}
