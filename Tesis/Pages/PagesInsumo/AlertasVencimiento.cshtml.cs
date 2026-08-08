using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tesis.Dominio;

namespace Tesis.Pages.PagesInsumo
{
    // CU28. El vencimiento vive en cada movimiento de ingreso y no en el insumo,
    // porque un mismo producto entra en partidas distintas: cada una se evalua por
    // separado.
    public class AlertasVencimientoModel : PageModel
    {
        [BindProperty]
        public int anticipacionDias { get; set; } = Controladora.DIAS_ANTICIPACION_VENCIMIENTO;

        public List<PartidaVencimiento> partidas = new List<PartidaVencimiento>();

        // Dias que faltan para cada partida y si ya esta vencida, en el mismo orden que
        // la lista de partidas.
        public List<int> diasRestantes = new List<int>();
        public List<bool> vencidas = new List<bool>();

        public void OnGet()
        {
            Controladora unaControladora = new Controladora();
            this.Calcular(unaControladora);
        }

        public void OnPostBuscar()
        {
            Controladora unaControladora = new Controladora();
            this.LeerFormulario();

            if (anticipacionDias <= 0)
            {
                ModelState.AddModelError(string.Empty,
                    "La ventana de anticipacion tiene que ser un numero positivo de dias!");
                anticipacionDias = Controladora.DIAS_ANTICIPACION_VENCIMIENTO;
            }

            this.Calcular(unaControladora);
        }

        private void Calcular(Controladora pControladoraDominio)
        {
            partidas = pControladoraDominio.ListarAlertasVencimiento(anticipacionDias);

            diasRestantes = new List<int>();
            vencidas = new List<bool>();

            foreach (PartidaVencimiento unaPartida in partidas)
            {
                diasRestantes.Add(pControladoraDominio.DiasParaVencer(unaPartida));
                vencidas.Add(pControladoraDominio.EstaVencida(unaPartida));
            }
        }

        private void LeerFormulario()
        {
            int vAnticipacion = 0;
            int.TryParse(Request.Form["anticipacionDias"], out vAnticipacion);
            anticipacionDias = vAnticipacion;
        }
    }
}
