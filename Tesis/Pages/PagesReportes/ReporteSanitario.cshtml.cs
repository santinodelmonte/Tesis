using Tesis.Dominio;

namespace Tesis.Pages.PagesReportes
{
    // CU45, RF7.2. Reporte sanitario.
    //
    // Las cuatro secciones son los cuatro registros sanitarios que lleva el sistema.
    public class ReporteSanitarioModel : ModeloReporte
    {
        protected override Reporte Armar(Controladora pControladoraDominio)
        {
            return pControladoraDominio.ArmarReporteSanitario(fechaDesde, fechaHasta);
        }

        protected override string NombreDelArchivo { get { return "reporte-sanitario"; } }
    }
}
