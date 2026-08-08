using Microsoft.AspNetCore.Mvc.RazorPages;
using Tesis.Dominio;

namespace Tesis.Pages.PagesSanidad
{
    // CU22, pasos 1 y 2: los planes ya configurados con su estado. Desde aca se crea
    // uno nuevo o se elige uno existente para modificarlo.
    public class ListaPlanesModel : PageModel
    {
        public List<PlanSanitario> planes = new List<PlanSanitario>();

        public void OnGet()
        {
            Controladora unaControladora = new Controladora();
            planes = unaControladora.ListarPlanesSanitarios();
        }
    }
}
