using Microsoft.AspNetCore.Mvc.RazorPages;
using Tesis.Dominio;

namespace Tesis.Pages.PagesInsumo
{
    // Pantalla de entrada del Modulo 5: el catalogo de insumos con su stock, y desde
    // aca se llega al ingreso de partidas, a los umbrales y a las alertas.
    public class ListaInsumosModel : PageModel
    {
        public List<Insumo> insumos = new List<Insumo>();
        public int criticos = 0;

        public void OnGet()
        {
            Controladora unaControladora = new Controladora();
            insumos = unaControladora.ListarInsumos();
            criticos = unaControladora.ListarAlertasStock().Count;
        }
    }
}
