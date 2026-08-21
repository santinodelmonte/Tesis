using Tesis.Dominio;

namespace Tesis.Pages.PagesReportes
{
    // CU44, RF7.1. Reporte productivo.
    //
    // Todo el contenido lo arma la Controladora: la pantalla elige el periodo y
    // decide si se ve o se descarga.
    public class ReporteProductivoModel : ModeloReporte
    {
        protected override Reporte Armar(Controladora pControladoraDominio)
        {
            return pControladoraDominio.ArmarReporteProductivo(fechaDesde, fechaHasta);
        }

        protected override string NombreDelArchivo { get { return "reporte-productivo"; } }
    }
}
