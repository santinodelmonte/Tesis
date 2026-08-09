using Microsoft.AspNetCore.Mvc.RazorPages;
using Tesis.Dominio;

namespace Tesis.Pages.PagesReproduccion
{
    // Lista de trabajo: los servicios que ya estan en condiciones de tactarse y todavia
    // no tienen tacto. Es con lo que se arma la visita del veterinario.
    public class TactosPendientesModel : PageModel
    {
        public List<Servicio> pendientes = new List<Servicio>();

        // Dias transcurridos desde cada servicio, en el mismo orden que la lista
        public List<int> diasDesdeServicio = new List<int>();

        public int diasParaTacto = 0;

        public void OnGet()
        {
            Controladora unaControladora = new Controladora();
            unaControladora.ListarAnimales();

            diasParaTacto = unaControladora.ObtenerConfiguracion().DiasParaTacto;
            pendientes = unaControladora.ListarTactosPendientes();

            foreach (Servicio unServicio in pendientes)
            {
                diasDesdeServicio.Add((int)(DateTime.Now.Date - unServicio.FechaServicio.Date).TotalDays);
            }
        }
    }
}
