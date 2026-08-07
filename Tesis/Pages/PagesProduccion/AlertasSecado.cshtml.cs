using Microsoft.AspNetCore.Mvc.RazorPages;
using Tesis.Dominio;

namespace Tesis.Pages.PagesProduccion
{
    // CU13 - Consultar Alertas de Secado Proximo
    public class AlertasSecadoModel : PageModel
    {
        public List<Hembra> alertas = new List<Hembra>();

        // Fecha recomendada de secado y dias que faltan, calculados a partir de la
        // fecha probable de parto de la lactancia en curso
        public Dictionary<int, DateTime> fechasRecomendadas = new Dictionary<int, DateTime>();
        public Dictionary<int, int> diasRestantes = new Dictionary<int, int>();
        public Dictionary<int, DateTime> fechasProbablesParto = new Dictionary<int, DateTime>();

        public int diasAnticipacion = Controladora.DIAS_ANTICIPACION_SECADO;
        public int diasAntesDelParto = Controladora.DIAS_SECADO_ANTES_PARTO;

        public void OnGet()
        {
            Controladora unaControladora = new Controladora();
            unaControladora.ListarAnimales();

            alertas = unaControladora.ListarAlertasSecado();

            foreach (Hembra unaHembra in alertas)
            {
                DateTime vFechaSecado = unaControladora.CalcularFechaSecado(unaHembra);
                Lactancia unaLactancia = unaControladora.LactanciaActual(unaHembra);

                fechasRecomendadas.Add(unaHembra.IdAnimal, vFechaSecado);
                diasRestantes.Add(unaHembra.IdAnimal, (int)(vFechaSecado.Date - DateTime.Now.Date).TotalDays);
                fechasProbablesParto.Add(unaHembra.IdAnimal,
                    unaLactancia != null ? unaLactancia.FechaProbableParto : DateTime.MinValue);
            }
        }
    }
}
