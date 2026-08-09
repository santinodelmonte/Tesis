using Microsoft.AspNetCore.Mvc.RazorPages;
using Tesis.Dominio;

namespace Tesis.Pages.PagesIndicadores
{
    // Los numeros con los que se sabe si el tambo va bien. Ninguno se almacena: todos
    // se calculan sobre lo que ya esta registrado.
    public class IndicadoresModel : PageModel
    {
        // Reproductivos
        public double promedioDiasAbiertos = 0;
        public double promedioIntervaloPartos = 0;
        public double serviciosPorPrenez = 0;

        // Productivos
        public double promedioDiarioRodeo = 0;
        public double promedioDiasEnLeche = 0;

        // Composicion del rodeo
        public int totalActivos = 0;
        public int enLactancia = 0;
        public int secas = 0;
        public int sinLactancia = 0;
        public int prenadas = 0;
        public int servidas = 0;
        public int vacias = 0;

        // Ranking de produccion de las lactancias en curso
        public List<Lactancia> lactanciasActivas = new List<Lactancia>();
        public List<int> diasEnLeche = new List<int>();
        public List<double> produccionEstimada = new List<double>();
        public List<double> promedioDiario = new List<double>();
        public List<double> proyeccion305 = new List<double>();

        public int diasLactanciaEstandar = Controladora.DIAS_LACTANCIA_ESTANDAR;

        public void OnGet()
        {
            Controladora unaControladora = new Controladora();
            unaControladora.ListarAnimales();

            promedioDiasAbiertos = unaControladora.PromedioDiasAbiertos();
            promedioIntervaloPartos = unaControladora.PromedioIntervaloEntrePartos();
            serviciosPorPrenez = unaControladora.ServiciosPorPrenez();

            promedioDiarioRodeo = unaControladora.PromedioDiarioRodeo();
            promedioDiasEnLeche = unaControladora.PromedioDiasEnLeche();

            totalActivos = unaControladora.FiltrarAnimalesXEstado(true).Count;
            enLactancia = unaControladora.ContarHembrasXEstadoProductivo(Hembra.EN_LACTANCIA);
            secas = unaControladora.ContarHembrasXEstadoProductivo(Hembra.SECA);
            sinLactancia = unaControladora.ContarHembrasXEstadoProductivo(Hembra.SIN_LACTANCIA);
            prenadas = unaControladora.ContarHembrasXEstadoReproductivo(Hembra.PRENADA);
            servidas = unaControladora.ContarHembrasXEstadoReproductivo(Hembra.SERVIDA);
            vacias = unaControladora.ContarHembrasXEstadoReproductivo(Hembra.VACIA);

            // Las lactancias en curso se ordenan de mayor a menor produccion diaria: la
            // comparacion entre vacas es el uso concreto de estos numeros.
            foreach (Lactancia unaLactancia in unaControladora.ListarLactanciasActivas())
            {
                this.InsertarOrdenada(unaControladora, unaLactancia);
            }
        }

        private void InsertarOrdenada(Controladora pControladoraDominio, Lactancia pLactancia)
        {
            double vPromedio = pControladoraDominio.PromedioDiarioLactancia(pLactancia);
            int vPosicion = 0;

            while (vPosicion < promedioDiario.Count && promedioDiario[vPosicion] >= vPromedio)
            {
                vPosicion = vPosicion + 1;
            }

            lactanciasActivas.Insert(vPosicion, pLactancia);
            promedioDiario.Insert(vPosicion, vPromedio);
            diasEnLeche.Insert(vPosicion, pControladoraDominio.DiasEnLeche(pLactancia));
            produccionEstimada.Insert(vPosicion, pControladoraDominio.EstimarProduccionLactancia(pLactancia));
            proyeccion305.Insert(vPosicion, pControladoraDominio.ProyectarProduccion305(pLactancia));
        }
    }
}
