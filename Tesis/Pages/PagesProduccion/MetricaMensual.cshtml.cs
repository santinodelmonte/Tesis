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

        // Litros de los turnos que se anotaron solo vaca por vaca. Suman a la produccion
        // del mes: son ordenies completos, anotados de otra manera.
        public double totalSoloIndividual = 0;
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

            // El volumen del mes es la leche que salio del tambo. Se arma turno por
            // turno: el total del ordenie cuando esta cargado, y la suma de los controles
            // cuando el turno se anoto unicamente vaca por vaca. El total medido en
            // control individual se muestra aparte como referencia, porque en los turnos
            // que si tienen ordenie por lote es una porcion de esos mismos litros.
            totalLote = unaControladora.CalcularProduccionEnRango(vDesde, vHasta, Controladora.MODALIDAD_LOTE);
            totalIndividual = unaControladora.CalcularProduccionEnRango(vDesde, vHasta, Controladora.MODALIDAD_INDIVIDUAL);
            totalMensual = unaControladora.CalcularProduccionMensual(mes, anio);

            totalSoloIndividual = unaControladora.SumarLitrosSinOrdenieLote(vDesde, vHasta);

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
