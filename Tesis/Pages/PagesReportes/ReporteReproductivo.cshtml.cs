using Tesis.Dominio;

namespace Tesis.Pages.PagesReportes
{
    // CU46, RF7.3. Reporte reproductivo.
    //
    // Los indicadores del final describen al rodeo entero y no al periodo: son el
    // contexto contra el que se leen los eventos de arriba.
    public class ReporteReproductivoModel : ModeloReporte
    {
        protected override Reporte Armar(Controladora pControladoraDominio)
        {
            return pControladoraDominio.ArmarReporteReproductivo(fechaDesde, fechaHasta);
        }

        protected override string NombreDelArchivo { get { return "reporte-reproductivo"; } }
    }
}
