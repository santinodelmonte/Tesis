using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tesis.Dominio;

namespace Tesis.Pages.PagesSanidad
{
    // Ficha sanitaria del rodeo: diagnosticos, tratamientos, vacunaciones y descornes
    // en un solo lugar. Es la pantalla de entrada del Modulo 4.
    public class ListaDiagnosticosModel : PageModel
    {
        public List<Diagnostico> diagnosticos = new List<Diagnostico>();
        public List<Tratamiento> tratamientos = new List<Tratamiento>();
        public List<Vacunacion> vacunaciones = new List<Vacunacion>();
        public List<Descorne> descornes = new List<Descorne>();

        // Animales que hoy tienen la leche en descarte, con la fecha hasta la que dura
        public Dictionary<int, DateTime> descartesVigentes = new Dictionary<int, DateTime>();
        public List<Animal> animalesEnDescarte = new List<Animal>();

        // Cierra el diagnostico cuando el animal se recupero. Sin este paso una
        // afeccion resuelta seguiria ofreciendose para tratar.
        public IActionResult OnPostResolver(int id)
        {
            Controladora unaControladora = new Controladora();
            unaControladora.ListarDiagnosticos();
            unaControladora.ResolverDiagnostico(id);

            return RedirectToPage();
        }

        public void OnGet()
        {
            Controladora unaControladora = new Controladora();
            diagnosticos = unaControladora.ListarDiagnosticos();
            tratamientos = unaControladora.ListarTratamientos();
            vacunaciones = unaControladora.ListarVacunaciones();
            descornes = unaControladora.ListarDescornes();

            foreach (Hembra unaHembra in unaControladora.ListarHembrasEnDescarte())
            {
                animalesEnDescarte.Add(unaHembra);
                descartesVigentes.Add(unaHembra.IdAnimal, unaControladora.FechaFinDescarte(unaHembra));
            }
        }
    }
}
