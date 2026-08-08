using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tesis.Dominio;

namespace Tesis.Pages.PagesSanidad
{
    // CU23. El cronograma no se guarda en ningun lado: es lo que los planes activos
    // exigen menos lo que se aplico, calculado cada vez que se consulta.
    public class CalendarioSanitarioModel : PageModel
    {
        [BindProperty]
        public int anticipacionDias { get; set; } = 0;
        [BindProperty]
        public string tipoProcedimiento { get; set; } = "";
        [BindProperty]
        public int idCategoria { get; set; } = 0;

        public List<ProcedimientoPendiente> pendientes = new List<ProcedimientoPendiente>();

        // Dias que faltan para cada pendiente y si ya esta vencido, en el mismo orden
        // que la lista de pendientes.
        public List<int> diasRestantes = new List<int>();
        public List<bool> vencidos = new List<bool>();

        public List<Categoria> categorias = new List<Categoria>();
        public int planesActivos = 0;

        public void OnGet()
        {
            Controladora unaControladora = new Controladora();

            // El horizonte que propone la pantalla es el configurado por el establecimiento
            anticipacionDias = unaControladora.ObtenerConfiguracion().DiasAnticipacionSanitaria;

            this.Calcular(unaControladora);
        }

        public void OnPostBuscar()
        {
            Controladora unaControladora = new Controladora();
            this.LeerFormulario();

            if (anticipacionDias <= 0)
            {
                ModelState.AddModelError(string.Empty,
                    "El horizonte de anticipacion tiene que ser un numero positivo de dias!");
                anticipacionDias = unaControladora.ObtenerConfiguracion().DiasAnticipacionSanitaria;
            }

            this.Calcular(unaControladora);
        }

        private void Calcular(Controladora pControladoraDominio)
        {
            categorias = pControladoraDominio.ListarCategorias();
            planesActivos = pControladoraDominio.ListarPlanesActivos().Count;

            pendientes = pControladoraDominio.FiltrarCalendario(
                pControladoraDominio.ObtenerCalendarioSanitario(anticipacionDias),
                tipoProcedimiento, idCategoria);

            diasRestantes = new List<int>();
            vencidos = new List<bool>();

            foreach (ProcedimientoPendiente unPendiente in pendientes)
            {
                diasRestantes.Add(pControladoraDominio.DiasParaAplicar(unPendiente));
                vencidos.Add(pControladoraDominio.EstaVencido(unPendiente));
            }
        }

        private void LeerFormulario()
        {
            int vAnticipacion = 0;
            int.TryParse(Request.Form["anticipacionDias"], out vAnticipacion);
            anticipacionDias = vAnticipacion;

            tipoProcedimiento = Request.Form["tipoProcedimiento"].ToString();

            int vIdCategoria = 0;
            int.TryParse(Request.Form["idCategoria"], out vIdCategoria);
            idCategoria = vIdCategoria;
        }
    }
}
