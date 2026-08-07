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

        public int diasAnticipacion = Controladora.DIAS_ANTICIPACION_PARTO;

        public void OnGet()
        {
            Controladora unaControladora = new Controladora();
            unaControladora.ListarAnimales();

            alertas = unaControladora.ListarAlertasParto();

            foreach (Servicio unServicio in alertas)
            {
                diasRestantes.Add(unServicio.IdServicio,
                    (int)(unServicio.FechaProbableParto.Date - DateTime.Now.Date).TotalDays);
            }
        }
    }
}
