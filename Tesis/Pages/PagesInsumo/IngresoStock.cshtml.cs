using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tesis.Dominio;

namespace Tesis.Pages.PagesInsumo
{
    // Adelantado del Modulo 5. Cada ingreso deja su movimiento, para que el descuento
    // que hace CU21 al usar una pajuela quede trazado y no sea solo un numero que baja.
    public class IngresoStockModel : PageModel
    {
        [BindProperty]
        public int idInsumo { get; set; } = 0;
        [BindProperty]
        public double cantidad { get; set; } = 0;
        [BindProperty]
        public DateTime fecha { get; set; } = DateTime.Now;
        [BindProperty]
        public DateTime fechaVencimiento { get; set; } = DateTime.MinValue;
        [BindProperty]
        public string? motivo { get; set; } = "";

        public List<Insumo> insumos = new List<Insumo>();
        public List<MovimientoStock> movimientos = new List<MovimientoStock>();

        public void OnGet()
        {
            Controladora unaControladora = new Controladora();
            this.CargarListados(unaControladora);
        }

        public IActionResult OnPostRegistrarIngreso()
        {
            Controladora unaControladora = new Controladora();
            this.CargarListados(unaControladora);
            this.LeerFormulario();

            if (idInsumo == 0)
            {
                ModelState.AddModelError(string.Empty, "Seleccione un insumo!");
                return Page();
            }

            if (cantidad <= 0)
            {
                ModelState.AddModelError(string.Empty, "La cantidad tiene que ser mayor a cero!");
                return Page();
            }

            if (fecha > DateTime.Now)
            {
                ModelState.AddModelError(string.Empty, "La fecha del ingreso no puede ser futura!");
                return Page();
            }

            if (unaControladora.IngresarStock(idInsumo, cantidad, fecha, fechaVencimiento, motivo ?? ""))
            {
                return Redirect("./ListaInsumos");
            }

            ModelState.AddModelError(string.Empty, "No se pudo registrar el ingreso de stock!");
            return Page();
        }

        private void CargarListados(Controladora pControladoraDominio)
        {
            insumos = pControladoraDominio.ListarInsumos();
            movimientos = pControladoraDominio.ListarMovimientosStock();
        }

        private void LeerFormulario()
        {
            int vIdInsumo = 0;
            int.TryParse(Request.Form["idInsumo"], out vIdInsumo);
            idInsumo = vIdInsumo;

            cantidad = Shared.CampoNumerico.LeerDecimal(Request.Form["cantidad"]);

            fecha = Request.Form["fecha"] != "" ? Convert.ToDateTime(Request.Form["fecha"]) : DateTime.Now;
            fechaVencimiento = Request.Form["fechaVencimiento"] != ""
                ? Convert.ToDateTime(Request.Form["fechaVencimiento"])
                : DateTime.MinValue;
            motivo = Request.Form["motivo"];
        }
    }
}
