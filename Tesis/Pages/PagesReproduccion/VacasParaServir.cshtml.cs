using Microsoft.AspNetCore.Mvc.RazorPages;
using Tesis.Dominio;

namespace Tesis.Pages.PagesReproduccion
{
    // Lista de trabajo: las hembras en condiciones de recibir servicio. Son las que ya
    // pasaron el periodo de espera voluntario despues de parir, y las vaquillonas que
    // alcanzaron la edad minima y nunca fueron servidas.
    public class VacasParaServirModel : PageModel
    {
        public List<Hembra> paraServir = new List<Hembra>();

        // Por que cada una aparece en la lista y hace cuanto pario, en el mismo orden
        public List<string> motivos = new List<string>();
        public List<int> diasAbiertos = new List<int>();
        public List<string> ultimoCelo = new List<string>();

        public int diasEsperaVoluntaria = 0;

        public void OnGet()
        {
            Controladora unaControladora = new Controladora();
            unaControladora.ListarAnimales();
            unaControladora.ListarCelos();

            diasEsperaVoluntaria = unaControladora.ObtenerConfiguracion().DiasEsperaVoluntaria;
            paraServir = unaControladora.ListarVacasParaServir();

            foreach (Hembra unaHembra in paraServir)
            {
                motivos.Add(unaControladora.MotivoParaServir(unaHembra));
                diasAbiertos.Add(unaControladora.DiasAbiertos(unaHembra));

                // El ultimo celo detectado es el dato con el que se decide a cual servir
                // primero: el servicio se hace sobre un celo.
                ultimoCelo.Add(this.UltimoCelo(unaControladora, unaHembra));
            }
        }

        private string UltimoCelo(Controladora pControladoraDominio, Hembra pHembra)
        {
            Celo unUltimo = null;

            foreach (Celo unCelo in pControladoraDominio.FiltrarCelosXHembra(pHembra.IdAnimal))
            {
                if (unUltimo == null || unCelo.Fecha > unUltimo.Fecha)
                {
                    unUltimo = unCelo;
                }
            }

            if (unUltimo == null)
            {
                return "";
            }
            return unUltimo.Fecha.ToShortDateString();
        }
    }
}
