using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tesis.Dominio;

namespace Tesis.Pages.PagesProduccion
{
    // CU14 - Consultar Historial de Produccion y Lactancias.
    //
    // Es ademas el listado desde el que se corrigen y se dan de baja los dos registros
    // de produccion, el ordenie por lote y el control individual. Va acá y no en las
    // pantallas de carga por el mismo criterio que en Reproduccion y Sanidad: una
    // entrada por entidad, y esa entrada es su listado.
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

        // Turnos anotados unicamente vaca por vaca, sin el total del ordenie. Su
        // produccion cuenta -es la suma de lo medido-, pero el dato es mas fragil que la
        // lectura del tanque, asi que la pantalla los senala.
        public List<OrdenieLote> turnosSoloIndividual = new List<OrdenieLote>();

        // El rango consultado, que los botones de baja mandan de vuelta. Sin esto la
        // pantalla vuelve sin consulta despues de borrar y hay que buscar otra vez para
        // ver como quedo. El formato es el mismo que manda el campo de fecha.
        public Dictionary<string, string> FiltroActual
        {
            get
            {
                return new Dictionary<string, string>
                {
                    { "modalidad", modalidad },
                    { "fechaDesde", fechaDesde.ToString("yyyy-MM-dd") },
                    { "fechaHasta", fechaHasta.ToString("yyyy-MM-dd") }
                };
            }
        }

        public void OnGet()
        {
            Controladora unaControladora = new Controladora();
            unaControladora.ListarAnimales();

            turnosSoloIndividual = unaControladora.ListarTurnosSoloConControlIndividual();
        }

        public void OnPostBuscar()
        {
            Controladora unaControladora = new Controladora();
            this.Buscar(unaControladora);
        }

        // Baja de un control individual. La consulta se rehace despues de borrar -con el
        // mismo rango, que viaja en el formulario de la baja- para que el listado quede
        // donde estaba y se vea el acumulado ya recalculado.
        public void OnPostEliminar(int id)
        {
            Controladora unaControladora = new Controladora();

            string vMotivo = unaControladora.ValidarEliminarOrdenieIndividual(id);

            if (vMotivo == "" && !unaControladora.EliminarOrdenieIndividual(id))
            {
                vMotivo = "No se pudo eliminar el control individual!";
            }

            if (vMotivo != "")
            {
                ModelState.AddModelError(string.Empty, vMotivo);
            }

            this.Buscar(unaControladora);
        }

        public void OnPostEliminarLote(int id)
        {
            Controladora unaControladora = new Controladora();

            string vMotivo = unaControladora.ValidarEliminarOrdenieLote(id);

            if (vMotivo == "" && !unaControladora.EliminarOrdenieLote(id))
            {
                vMotivo = "No se pudo eliminar el ordeñe del lote!";
            }

            if (vMotivo != "")
            {
                ModelState.AddModelError(string.Empty, vMotivo);
            }

            this.Buscar(unaControladora);
        }

        private void Buscar(Controladora pControladoraDominio)
        {
            pControladoraDominio.ListarAnimales();

            turnosSoloIndividual = pControladoraDominio.ListarTurnosSoloConControlIndividual();

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
                ordeniesLote = pControladoraDominio.FiltrarOrdeniesLoteXFecha(fechaDesde, fechaHasta);
            }

            if (modalidad == Controladora.MODALIDAD_INDIVIDUAL)
            {
                ordeniesIndividual = pControladoraDominio.FiltrarOrdeniesIndividualXFecha(fechaDesde, fechaHasta);
            }

            // La produccion se resuelve turno por turno: el total del lote ya incluye a
            // las vacas medidas, asi que no hay doble conteo que descontar.
            acumulado = pControladoraDominio.CalcularProduccionEnRango(fechaDesde, fechaHasta, modalidad);
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
