using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tesis.Dominio;

namespace Tesis.Pages.PagesProduccion
{
    // CU10 - Consultar Historial de Produccion y Lactancias
    public class HistorialProduccionModel : PageModel
    {
        [BindProperty]
        public string modalidad { get; set; } = Controladora.MODALIDAD_LOTE;
        [BindProperty]
        public DateTime fechaDesde { get; set; } = DateTime.Now.AddDays(-30);
        [BindProperty]
        public DateTime fechaHasta { get; set; } = DateTime.Now;

        public bool consultado = false;
        public double acumulado = 0;

        public List<OrdenieLote> ordeniesLote = new List<OrdenieLote>();
        public List<OrdenieIndividual> ordeniesIndividual = new List<OrdenieIndividual>();

        // Turnos con control individual cargado y sin ordenie de lote. Como la
        // produccion del establecimiento sale de los lotes, esos turnos no suman nada:
        // casi siempre significa que falto cargar el ordenie.
        public List<OrdenieLote> turnosSinOrdenie = new List<OrdenieLote>();

        public void OnGet()
        {
            Controladora unaControladora = new Controladora();
            unaControladora.ListarAnimales();

            turnosSinOrdenie = unaControladora.ListarTurnosSinOrdenieLote();
        }

        public void OnPostBuscar()
        {
            Controladora unaControladora = new Controladora();
            unaControladora.ListarAnimales();

            turnosSinOrdenie = unaControladora.ListarTurnosSinOrdenieLote();

            this.LeerFormulario();

            // Curso de excepcion 3a
            if (fechaDesde > fechaHasta)
            {
                ModelState.AddModelError(string.Empty, "El rango de fechas es invalido: la fecha desde es posterior a la fecha hasta!");
                return;
            }

            if (modalidad != Controladora.MODALIDAD_INDIVIDUAL && modalidad != Controladora.MODALIDAD_LOTE)
            {
                ModelState.AddModelError(string.Empty, "Seleccione la modalidad de visualizacion!");
                return;
            }

            if (modalidad == Controladora.MODALIDAD_LOTE)
            {
                ordeniesLote = unaControladora.FiltrarOrdeniesLoteXFecha(fechaDesde, fechaHasta);
            }

            if (modalidad == Controladora.MODALIDAD_INDIVIDUAL)
            {
                ordeniesIndividual = unaControladora.FiltrarOrdeniesIndividualXFecha(fechaDesde, fechaHasta);
            }

            // En "Totales" se suman las dos fuentes: los litros del control individual
            // no estan incluidos en los del lote, asi que no hay doble conteo.
            acumulado = unaControladora.CalcularProduccionEnRango(fechaDesde, fechaHasta, modalidad);
            consultado = true;
        }

        private void LeerFormulario()
        {
            modalidad = Request.Form["modalidad"] != "" ? Request.Form["modalidad"] : Controladora.MODALIDAD_LOTE;
            fechaDesde = Request.Form["fechaDesde"] != "" ? Convert.ToDateTime(Request.Form["fechaDesde"]) : DateTime.Now.AddDays(-30);
            fechaHasta = Request.Form["fechaHasta"] != "" ? Convert.ToDateTime(Request.Form["fechaHasta"]) : DateTime.Now;
        }
    }
}
