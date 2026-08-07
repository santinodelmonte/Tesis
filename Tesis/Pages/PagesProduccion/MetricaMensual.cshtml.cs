using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tesis.Dominio;

namespace Tesis.Pages.PagesProduccion
{
    // CU11 - Consultar Metrica de Produccion Mensual
    public class MetricaMensualModel : PageModel
    {
        [BindProperty]
        public int mes { get; set; } = DateTime.Now.Month;
        [BindProperty]
        public int anio { get; set; } = DateTime.Now.Year;

        public bool consultado = false;
        public double totalMensual = 0;
        public double totalLote = 0;
        public double totalIndividual = 0;
        public int cantidadOrdeniesLote = 0;
        public int cantidadControles = 0;

        public void OnGet()
        {
            Controladora unaControladora = new Controladora();
            unaControladora.ListarAnimales();
        }

        public void OnPostConsultar()
        {
            Controladora unaControladora = new Controladora();
            unaControladora.ListarAnimales();

            this.LeerFormulario();

            if (mes < 1 || mes > 12 || anio < 2000 || anio > DateTime.Now.Year + 1)
            {
                ModelState.AddModelError(string.Empty, "Seleccione un mes y un año validos!");
                return;
            }

            DateTime vDesde = new DateTime(anio, mes, 1);
            DateTime vHasta = vDesde.AddMonths(1).AddDays(-1);

            // La regla de negocio es explicita: el mensual suma obligatoriamente las dos
            // fuentes para no perder informacion. Se muestran ademas por separado para
            // que se vea de donde sale el numero.
            totalLote = unaControladora.CalcularProduccionEnRango(vDesde, vHasta, Controladora.MODALIDAD_LOTE);
            totalIndividual = unaControladora.CalcularProduccionEnRango(vDesde, vHasta, Controladora.MODALIDAD_INDIVIDUAL);
            totalMensual = unaControladora.CalcularProduccionMensual(mes, anio);

            cantidadOrdeniesLote = unaControladora.FiltrarOrdeniesLoteXFecha(vDesde, vHasta).Count;
            cantidadControles = unaControladora.FiltrarOrdeniesIndividualXFecha(vDesde, vHasta).Count;

            consultado = true;
        }

        private void LeerFormulario()
        {
            int vMes = DateTime.Now.Month;
            int vAnio = DateTime.Now.Year;

            int.TryParse(Request.Form["mes"], out vMes);
            int.TryParse(Request.Form["anio"], out vAnio);

            mes = vMes;
            anio = vAnio;
        }
    }
}
