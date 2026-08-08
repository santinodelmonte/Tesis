using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tesis.Dominio;

namespace Tesis.Pages.PagesInsumo
{
    // CU26. El umbral es la barrera de seguridad del inventario: cuando el stock lo
    // alcanza, el insumo aparece en las alertas de stock critico de CU27.
    public class ConfigurarStockMinimoModel : PageModel
    {
        [BindProperty]
        public int idInsumo { get; set; } = 0;
        [BindProperty]
        public double stockMinimo { get; set; } = 0;

        public List<Insumo> insumos = new List<Insumo>();

        public void OnGet(int id)
        {
            Controladora unaControladora = new Controladora();
            insumos = unaControladora.ListarInsumos();

            Insumo unInsumo = unaControladora.BuscarInsumo(id);
            if (unInsumo == null)
            {
                return;
            }

            idInsumo = unInsumo.IdInsumo;
            stockMinimo = unInsumo.StockMinimo;
        }

        public IActionResult OnPostGuardar()
        {
            Controladora unaControladora = new Controladora();
            insumos = unaControladora.ListarInsumos();

            this.LeerFormulario();

            if (idInsumo == 0)
            {
                ModelState.AddModelError(string.Empty, "Seleccione un insumo!");
                return Page();
            }

            // Curso de excepcion 3a: el umbral no puede ser negativo
            if (stockMinimo < 0)
            {
                ModelState.AddModelError(string.Empty, "El stock minimo tiene que ser mayor o igual a cero!");
                return Page();
            }

            if (unaControladora.ModificarStockMinimo(idInsumo, stockMinimo))
            {
                return Redirect("./ListaInsumos");
            }

            ModelState.AddModelError(string.Empty, "No se pudo guardar el stock minimo!");
            return Page();
        }

        private void LeerFormulario()
        {
            int vIdInsumo = 0;
            int.TryParse(Request.Form["idInsumo"], out vIdInsumo);
            idInsumo = vIdInsumo;

            double vStockMinimo = 0;
            double.TryParse(Request.Form["stockMinimo"], out vStockMinimo);
            stockMinimo = vStockMinimo;
        }
    }
}
