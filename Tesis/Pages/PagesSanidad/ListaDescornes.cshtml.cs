using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tesis.Dominio;

namespace Tesis.Pages.PagesSanidad
{
    // Descornes registrados. Antes se veian al pie de la pantalla que los registraba,
    // que no era el lugar para gestionarlos.
    public class ListaDescornesModel : PageModel
    {
        public List<Descorne> descornes = new List<Descorne>();

        public void OnGet()
        {
            Controladora unaControladora = new Controladora();
            descornes = unaControladora.ListarDescornes();
        }

        public IActionResult OnPostEliminar(int id)
        {
            Controladora unaControladora = new Controladora();

            string vMotivo = unaControladora.ValidarEliminarDescorne(id);
            if (vMotivo != "")
            {
                descornes = unaControladora.ListarDescornes();
                ModelState.AddModelError(string.Empty, vMotivo);
                return Page();
            }

            if (!unaControladora.EliminarDescorne(id))
            {
                descornes = unaControladora.ListarDescornes();
                ModelState.AddModelError(string.Empty, "No se pudo eliminar el descorne!");
                return Page();
            }

            return RedirectToPage();
        }
    }
}
