using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tesis.Dominio;

namespace Tesis.Pages.PagesInsumo
{
    // CU29. Todo movimiento queda asentado, incluidos los egresos automaticos que
    // genera el sistema al inseminar, tratar o vacunar: el motivo dice cual fue la
    // operacion que lo origino.
    public class HistorialMovimientosModel : PageModel
    {
        [BindProperty]
        public int idInsumo { get; set; } = 0;
        [BindProperty]
        public string tipoMovimiento { get; set; } = "";
        [BindProperty]
        public DateTime desde { get; set; } = DateTime.MinValue;
        [BindProperty]
        public DateTime hasta { get; set; } = DateTime.MinValue;

        public List<Insumo> insumos = new List<Insumo>();
        public List<MovimientoStock> movimientos = new List<MovimientoStock>();

        // Stock que quedo despues de cada movimiento, en el mismo orden que la lista
        public List<double> stockResultante = new List<double>();

        public bool buscoSinResultados = false;

        public void OnGet()
        {
            Controladora unaControladora = new Controladora();
            this.Buscar(unaControladora);
        }

        public void OnPostBuscar()
        {
            Controladora unaControladora = new Controladora();
            this.LeerFormulario();

            // El rango tiene que ser valido: la fecha de inicio, anterior o igual a la de fin
            if (desde != DateTime.MinValue && hasta != DateTime.MinValue && desde.Date > hasta.Date)
            {
                ModelState.AddModelError(string.Empty,
                    "La fecha de inicio tiene que ser anterior o igual a la fecha de fin!");
                insumos = unaControladora.ListarInsumos();
                return;
            }

            this.Buscar(unaControladora);

            // Curso de excepcion 4a: no hay movimientos que coincidan con los criterios
            buscoSinResultados = movimientos.Count == 0;
        }

        private void Buscar(Controladora pControladoraDominio)
        {
            insumos = pControladoraDominio.ListarInsumos();
            movimientos = pControladoraDominio.FiltrarMovimientos(idInsumo, tipoMovimiento, desde, hasta);

            stockResultante = new List<double>();
            foreach (MovimientoStock unMovimiento in movimientos)
            {
                stockResultante.Add(pControladoraDominio.StockResultante(unMovimiento));
            }
        }

        private void LeerFormulario()
        {
            int vIdInsumo = 0;
            int.TryParse(Request.Form["idInsumo"], out vIdInsumo);
            idInsumo = vIdInsumo;

            tipoMovimiento = Request.Form["tipoMovimiento"].ToString();

            desde = Request.Form["desde"] != "" ? Convert.ToDateTime(Request.Form["desde"]) : DateTime.MinValue;
            hasta = Request.Form["hasta"] != "" ? Convert.ToDateTime(Request.Form["hasta"]) : DateTime.MinValue;
        }
    }
}
