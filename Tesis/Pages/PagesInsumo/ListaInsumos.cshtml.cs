using Microsoft.AspNetCore.Mvc.RazorPages;
using Tesis.Dominio;

namespace Tesis.Pages.PagesInsumo
{
    // Adelantado del Modulo 5. Solo el listado y el stock: los umbrales, los
    // vencimientos y las alertas llegan con ese modulo.
    public class ListaInsumosModel : PageModel
    {
        public List<Insumo> insumos = new List<Insumo>();

        public void OnGet()
        {
            Controladora unaControladora = new Controladora();
            insumos = unaControladora.ListarInsumos();
        }
    }
}
