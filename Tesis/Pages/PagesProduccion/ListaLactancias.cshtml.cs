using Microsoft.AspNetCore.Mvc.RazorPages;
using Tesis.Dominio;

namespace Tesis.Pages.PagesProduccion
{
    // RF2.7 - Historial de lactancias. Da acceso ademas al secado y a las alertas.
    public class ListaLactanciasModel : PageModel
    {
        public List<Lactancia> lactancias = new List<Lactancia>();

        // Litros medidos: la suma cruda de los controles individuales. Con un control
        // por mes son diez jornadas sueltas, no la leche de la lactancia.
        public Dictionary<int, double> produccion = new Dictionary<int, double>();
        public Dictionary<int, int> controles = new Dictionary<int, int>();

        // Lo que la lactancia realmente produjo, estimado por intervalos de control, y
        // los dias que lleva en leche.
        public Dictionary<int, double> estimada = new Dictionary<int, double>();
        public Dictionary<int, int> diasEnLeche = new Dictionary<int, int>();

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
                estimada.Add(unaLactancia.IdLactancia,
                    unaControladora.EstimarProduccionLactancia(unaLactancia));
                diasEnLeche.Add(unaLactancia.IdLactancia, unaControladora.DiasEnLeche(unaLactancia));

                if (unaControladora.LactanciaEstaActiva(unaLactancia))
                {
                    cantidadActivas = cantidadActivas + 1;
                }
            }
        }
    }
}
