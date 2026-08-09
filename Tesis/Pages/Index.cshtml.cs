using Microsoft.AspNetCore.Mvc.RazorPages;
using Tesis.Dominio;

namespace Tesis.Pages
{
    // Tablero de inicio: lo que hay que hacer hoy en el tambo.
    //
    // No calcula nada propio. Junta las listas de trabajo y las alertas que ya viven en
    // la Controladora y que hasta ahora estaban repartidas en seis menus distintos, de
    // manera que la encargada abra el sistema a la mañana y vea de una todo lo que
    // requiere atencion. Es, ademas, el mismo contenido del resumen diario del bot.
    public class IndexModel : PageModel
    {
        // Trabajo reproductivo
        public int vacasParaServir = 0;
        public int tactosPendientes = 0;
        public int partosProximos = 0;

        // Trabajo productivo
        public int secadosProximos = 0;
        public int animalesEnDescarte = 0;

        // Trabajo sanitario y de stock
        public int procedimientosPendientes = 0;
        public int insumosCriticos = 0;
        public int partidasPorVencer = 0;

        // Composicion del rodeo
        public int enLactancia = 0;
        public int secas = 0;
        public int prenadas = 0;
        public int vacias = 0;
        public int totalActivos = 0;

        public double promedioDiarioRodeo = 0;
        public double promedioDiasEnLeche = 0;

        // Cuando no hay ningun animal cargado, el tablero explica por donde empezar en
        // lugar de mostrar ocho ceros.
        public bool sinDatos = false;

        public void OnGet()
        {
            Controladora unaControladora = new Controladora();
            Configuracion unaConfiguracion = unaControladora.ObtenerConfiguracion();

            List<Animal> _listaAnimales = unaControladora.ListarAnimales();
            totalActivos = unaControladora.FiltrarAnimalesXEstado(true).Count;
            sinDatos = _listaAnimales.Count == 0;

            vacasParaServir = unaControladora.ListarVacasParaServir().Count;
            tactosPendientes = unaControladora.ListarTactosPendientes().Count;
            partosProximos = unaControladora.ListarAlertasParto().Count;

            secadosProximos = unaControladora.ListarAlertasSecado().Count;
            animalesEnDescarte = unaControladora.ListarHembrasEnDescarte().Count;

            procedimientosPendientes = unaControladora.ObtenerCalendarioSanitario(
                unaConfiguracion.DiasAnticipacionSanitaria).Count;
            insumosCriticos = unaControladora.ListarAlertasStock().Count;
            partidasPorVencer = unaControladora.ListarAlertasVencimiento(
                unaConfiguracion.DiasAnticipacionVencimiento).Count;

            enLactancia = unaControladora.ContarHembrasXEstadoProductivo(Hembra.EN_LACTANCIA);
            secas = unaControladora.ContarHembrasXEstadoProductivo(Hembra.SECA);
            prenadas = unaControladora.ContarHembrasXEstadoReproductivo(Hembra.PRENADA);
            vacias = unaControladora.ContarHembrasXEstadoReproductivo(Hembra.VACIA);

            promedioDiarioRodeo = unaControladora.PromedioDiarioRodeo();
            promedioDiasEnLeche = unaControladora.PromedioDiasEnLeche();
        }
    }
}
