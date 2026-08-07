using Microsoft.AspNetCore.Mvc.RazorPages;
using Tesis.Dominio;

namespace Tesis.Pages.PagesReproduccion
{
    // Ficha reproductiva historica de los celos detectados en el rodeo
    public class ListaCelosModel : PageModel
    {
        public List<Celo> celos = new List<Celo>();

        public void OnGet()
        {
            Controladora unaControladora = new Controladora();
            celos = unaControladora.ListarCelos();
        }
    }
}
