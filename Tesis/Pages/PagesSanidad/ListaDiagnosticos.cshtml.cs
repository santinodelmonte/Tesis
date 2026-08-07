using Microsoft.AspNetCore.Mvc.RazorPages;
using Tesis.Dominio;

namespace Tesis.Pages.PagesSanidad
{
    // Adelantado del Modulo 4. Solo el diagnostico y el tratamiento, que son los que
    // determinan el descarte de leche que el paso 3 de CU8 usa para excluir animales
    // del lote de ordenie. Los planes sanitarios, las vacunaciones, los descornes y el
    // calendario llegan con ese modulo.
    public class ListaDiagnosticosModel : PageModel
    {
        public List<Diagnostico> diagnosticos = new List<Diagnostico>();
        public List<Tratamiento> tratamientos = new List<Tratamiento>();

        // Animales que hoy tienen la leche en descarte, con la fecha hasta la que dura
        public Dictionary<int, DateTime> descartesVigentes = new Dictionary<int, DateTime>();
        public List<Animal> animalesEnDescarte = new List<Animal>();

        public void OnGet()
        {
            Controladora unaControladora = new Controladora();
            diagnosticos = unaControladora.ListarDiagnosticos();
            tratamientos = unaControladora.ListarTratamientos();

            foreach (Hembra unaHembra in unaControladora.ListarHembrasEnDescarte())
            {
                animalesEnDescarte.Add(unaHembra);
                descartesVigentes.Add(unaHembra.IdAnimal, unaControladora.FechaFinDescarte(unaHembra));
            }
        }
    }
}
