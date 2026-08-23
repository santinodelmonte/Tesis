using Tesis.Dominio;

namespace Tesis.Pages.PagesReportes
{
    // CU47, RF7.4. Reporte genetico.
    //
    // Es el unico de los cuatro que no lleva rango de fechas.
    public class ReporteGeneticoModel : ModeloReporte
    {
        // La genealogia no ocurre en un periodo: es el estado del rodeo.
        public override bool UsaPeriodo { get { return false; } }

        protected override Reporte Armar(Controladora pControladoraDominio)
        {
            return pControladoraDominio.ArmarReporteGenetico();
        }

        protected override string NombreDelArchivo { get { return "reporte-genetico"; } }
    }
}
