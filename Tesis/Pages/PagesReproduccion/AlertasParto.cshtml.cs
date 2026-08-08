using Microsoft.AspNetCore.Mvc.RazorPages;
using Tesis.Dominio;

namespace Tesis.Pages.PagesReproduccion
{
    // CU17 - Consultar Alertas de Parto Proximo
    public class AlertasPartoModel : PageModel
    {
        // Se listan los servicios y no las hembras porque la fecha proyectada vive en
        // el servicio
        public List<Servicio> alertas = new List<Servicio>();
        public Dictionary<int, int> diasRestantes = new Dictionary<int, int>();

        // Sale de la configuracion del establecimiento
        public int diasAnticipacion = 0;

        public void OnGet()
        {
            Controladora unaControladora = new Controladora();
            unaControladora.ListarAnimales();

            // La ventana de la alerta la fija la configuracion del establecimiento
            diasAnticipacion = unaControladora.ObtenerConfiguracion().DiasAnticipacionParto;

            alertas = unaControladora.ListarAlertasParto();

            foreach (Servicio unServicio in alertas)
            {
                diasRestantes.Add(unServicio.IdServicio,
                    (int)(unServicio.FechaProbableParto.Date - DateTime.Now.Date).TotalDays);
            }
        }
    }
}
