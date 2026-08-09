using Microsoft.AspNetCore.Mvc.RazorPages;
using Tesis.Dominio;

namespace Tesis.Pages.PagesIndicadores
{
    // Apoyo a la decision de descarte. El sistema no decide: junta lo que ya sabe de
    // cada vaca y muestra los motivos por los que conviene revisarla.
    public class CandidatasDescarteModel : PageModel
    {
        public List<CandidataDescarte> candidatas = new List<CandidataDescarte>();
        public double promedioRodeo = 0;
        public int totalEnRodeo = 0;

        // Los criterios que se aplicaron, para que la pantalla los explique
        public int porcentajeProduccionBaja = (int)(Controladora.PORCENTAJE_PRODUCCION_BAJA * 100);
        public int serviciosSinPrenez = Controladora.SERVICIOS_SIN_PRENIEZ_DESCARTE;
        public int diasAbiertosExcesivos = Controladora.DIAS_ABIERTOS_EXCESIVOS;
        public int diagnosticosRepetidos = Controladora.DIAGNOSTICOS_REPETIDOS_DESCARTE;
        public int partosParaDescarte = Controladora.PARTOS_PARA_DESCARTE;

        public void OnGet()
        {
            Controladora unaControladora = new Controladora();
            unaControladora.ListarAnimales();

            promedioRodeo = unaControladora.PromedioDiarioRodeo();
            totalEnRodeo = unaControladora.ContarHembrasXEstadoProductivo(Hembra.EN_LACTANCIA);
            candidatas = unaControladora.ListarCandidatasDescarte();
        }
    }
}
