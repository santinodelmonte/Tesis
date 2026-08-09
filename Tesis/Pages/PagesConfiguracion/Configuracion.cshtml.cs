using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tesis.Dominio;

namespace Tesis.Pages.PagesConfiguracion
{
    // Parametros de manejo del establecimiento. Son las reglas que cada tambo resuelve
    // a su manera y que hasta ahora estaban fijas en el codigo.
    public class ConfiguracionModel : PageModel
    {
        [BindProperty]
        public int diasSecadoAntesParto { get; set; } = 0;
        [BindProperty]
        public int edadMinimaServicioMeses { get; set; } = 0;
        [BindProperty]
        public int edadCambioCategoriaMeses { get; set; } = 0;
        [BindProperty]
        public double litrosMaximosIndividual { get; set; } = 0;
        [BindProperty]
        public int ordeniesPorDia { get; set; } = 0;
        [BindProperty]
        public int diasAnticipacionSecado { get; set; } = 0;
        [BindProperty]
        public int diasAnticipacionParto { get; set; } = 0;
        [BindProperty]
        public int diasAnticipacionSanitaria { get; set; } = 0;
        [BindProperty]
        public int diasAnticipacionVencimiento { get; set; } = 0;
        [BindProperty]
        public int diasEsperaVoluntaria { get; set; } = 0;
        [BindProperty]
        public int diasParaTacto { get; set; } = 0;

        // Constantes que no se configuran: son biologia y no decisiones. Se muestran
        // para que quede claro que el sistema las usa y por que no estan editables.
        public int gestacionDias = Controladora.GESTACION_DIAS;
        public int edadMinimaCeloMeses = Controladora.EDAD_MINIMA_CELO_MESES;
        public int gestacionDiasMinima = Controladora.GESTACION_DIAS_MINIMA;
        public int gestacionDiasMaxima = Controladora.GESTACION_DIAS_MAXIMA;

        public bool guardado = false;

        public void OnGet()
        {
            Controladora unaControladora = new Controladora();
            this.LeerConfiguracion(unaControladora.ObtenerConfiguracion());
        }

        public IActionResult OnPostGuardar()
        {
            Controladora unaControladora = new Controladora();
            this.LeerFormulario();

            Configuracion unaConfiguracion = this.ArmarConfiguracion();

            // La Controladora concentra los rangos admitidos de cada parametro
            string vMotivo = unaControladora.ValidarConfiguracion(unaConfiguracion);
            if (vMotivo != "")
            {
                ModelState.AddModelError(string.Empty, vMotivo);
                return Page();
            }

            if (unaControladora.ModificarConfiguracion(unaConfiguracion))
            {
                guardado = true;
                this.LeerConfiguracion(unaControladora.ObtenerConfiguracion());
                return Page();
            }

            ModelState.AddModelError(string.Empty,
                "No se pudo guardar la configuracion. Verifique que la tabla de configuracion tenga su fila inicial!");
            return Page();
        }

        private Configuracion ArmarConfiguracion()
        {
            return new Configuracion(0, diasSecadoAntesParto, edadMinimaServicioMeses,
                edadCambioCategoriaMeses, litrosMaximosIndividual, ordeniesPorDia,
                diasAnticipacionSecado, diasAnticipacionParto, diasAnticipacionSanitaria,
                diasAnticipacionVencimiento, diasEsperaVoluntaria, diasParaTacto);
        }

        private void LeerConfiguracion(Configuracion pConfiguracion)
        {
            diasSecadoAntesParto = pConfiguracion.DiasSecadoAntesParto;
            edadMinimaServicioMeses = pConfiguracion.EdadMinimaServicioMeses;
            edadCambioCategoriaMeses = pConfiguracion.EdadCambioCategoriaMeses;
            litrosMaximosIndividual = pConfiguracion.LitrosMaximosIndividual;
            ordeniesPorDia = pConfiguracion.OrdeniesPorDia;
            diasAnticipacionSecado = pConfiguracion.DiasAnticipacionSecado;
            diasAnticipacionParto = pConfiguracion.DiasAnticipacionParto;
            diasAnticipacionSanitaria = pConfiguracion.DiasAnticipacionSanitaria;
            diasAnticipacionVencimiento = pConfiguracion.DiasAnticipacionVencimiento;
            diasEsperaVoluntaria = pConfiguracion.DiasEsperaVoluntaria;
            diasParaTacto = pConfiguracion.DiasParaTacto;
        }

        private void LeerFormulario()
        {
            int vDiasSecado = 0;
            int.TryParse(Request.Form["diasSecadoAntesParto"], out vDiasSecado);
            diasSecadoAntesParto = vDiasSecado;

            int vEdadServicio = 0;
            int.TryParse(Request.Form["edadMinimaServicioMeses"], out vEdadServicio);
            edadMinimaServicioMeses = vEdadServicio;

            int vEdadCategoria = 0;
            int.TryParse(Request.Form["edadCambioCategoriaMeses"], out vEdadCategoria);
            edadCambioCategoriaMeses = vEdadCategoria;

            double vLitros = 0;
            double.TryParse(Request.Form["litrosMaximosIndividual"], out vLitros);
            litrosMaximosIndividual = vLitros;

            int vOrdenies = 0;
            int.TryParse(Request.Form["ordeniesPorDia"], out vOrdenies);
            ordeniesPorDia = vOrdenies;

            int vSecado = 0;
            int.TryParse(Request.Form["diasAnticipacionSecado"], out vSecado);
            diasAnticipacionSecado = vSecado;

            int vParto = 0;
            int.TryParse(Request.Form["diasAnticipacionParto"], out vParto);
            diasAnticipacionParto = vParto;

            int vSanitaria = 0;
            int.TryParse(Request.Form["diasAnticipacionSanitaria"], out vSanitaria);
            diasAnticipacionSanitaria = vSanitaria;

            int vVencimiento = 0;
            int.TryParse(Request.Form["diasAnticipacionVencimiento"], out vVencimiento);
            diasAnticipacionVencimiento = vVencimiento;

            int vEspera = 0;
            int.TryParse(Request.Form["diasEsperaVoluntaria"], out vEspera);
            diasEsperaVoluntaria = vEspera;

            int vTacto = 0;
            int.TryParse(Request.Form["diasParaTacto"], out vTacto);
            diasParaTacto = vTacto;
        }
    }
}
