using Microsoft.AspNetCore.Mvc.RazorPages;
using Tesis.Dominio;

namespace Tesis.Pages.PagesProduccion
{
    // RF2.7 - Historial de lactancias. Da acceso ademas al secado y a las alertas.
    public class ListaLactanciasModel : PageModel
    {
        public List<Lactancia> lactancias = new List<Lactancia>();

        // Litros acumulados de cada lactancia, sumando sus controles individuales
        public Dictionary<int, double> produccion = new Dictionary<int, double>();
        public Dictionary<int, int> controles = new Dictionary<int, int>();

        public int cantidadActivas = 0;

        public void OnGet()
        {
            Controladora unaControladora = new Controladora();
            lactancias = unaControladora.ListarLactancias();

            foreach (Lactancia unaLactancia in lactancias)
            {
                produccion.Add(unaLactancia.IdLactancia, unaControladora.CalcularProduccionTotal(unaLactancia));
                controles.Add(unaLactancia.IdLactancia,
                    unaControladora.FiltrarOrdeniesXLactancia(unaLactancia.IdLactancia).Count);

                if (unaControladora.LactanciaEstaActiva(unaLactancia))
                {
                    cantidadActivas = cantidadActivas + 1;
                }
            }
        }
    }
}
