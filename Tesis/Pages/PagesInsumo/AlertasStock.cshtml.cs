using Microsoft.AspNetCore.Mvc.RazorPages;
using Tesis.Dominio;

namespace Tesis.Pages.PagesInsumo
{
    // CU37. La condicion se evalua contra el stock del momento: los egresos
    // automaticos de la inseminacion, el tratamiento y la vacunacion ya lo dejaron
    // descontado, asi que la alerta aparece apenas ocurren.
    public class AlertasStockModel : PageModel
    {
        public List<Insumo> criticos = new List<Insumo>();
        public int totalInsumos = 0;

        public void OnGet()
        {
            Controladora unaControladora = new Controladora();
            totalInsumos = unaControladora.ListarInsumos().Count;
            criticos = unaControladora.ListarAlertasStock();
        }
    }
}
