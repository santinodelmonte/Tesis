using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tesis.Dominio;

namespace Tesis.Pages.PagesSanidad
{
    // CU21. La aplicacion descuenta la dosis del stock y declara, si corresponde, que
    // plan sanitario da por cumplido: sin esa declaracion el calendario tendria que
    // adivinarlo a partir del insumo, y dos planes distintos pueden usar la misma
    // vacuna.
    public class RegistrarVacunacionModel : PageModel
    {
        [BindProperty]
        public string? numCaravana { get; set; } = "";
        [BindProperty]
        public int idInsumo { get; set; } = 0;
        [BindProperty]
        public int idPlan { get; set; } = 0;
        [BindProperty]
        public DateTime fechaAplicacion { get; set; } = DateTime.Now;

        public List<Animal> animales = new List<Animal>();
        public List<Insumo> vacunas = new List<Insumo>();
        public List<PlanSanitario> planes = new List<PlanSanitario>();
        public List<Vacunacion> vacunaciones = new List<Vacunacion>();

        public void OnGet()
        {
            Controladora unaControladora = new Controladora();
            this.CargarListados(unaControladora);
        }

        public IActionResult OnPostGuardar()
        {
            Controladora unaControladora = new Controladora();
            this.CargarListados(unaControladora);
            this.LeerFormulario();

            if (numCaravana == null || numCaravana == "")
            {
                ModelState.AddModelError(string.Empty, "Seleccione un animal!");
                return Page();
            }

            Animal unAnimal = unaControladora.BuscarAnimalXCaravana(numCaravana);
            if (unAnimal == null)
            {
                ModelState.AddModelError(string.Empty, "La caravana no existe en el sistema!");
                return Page();
            }

            Insumo unaVacuna = unaControladora.BuscarInsumo(idInsumo);
            if (unaVacuna == null)
            {
                ModelState.AddModelError(string.Empty, "Seleccione la vacuna aplicada!");
                return Page();
            }

            if (fechaAplicacion > DateTime.Now)
            {
                ModelState.AddModelError(string.Empty, "La fecha de aplicacion no puede ser futura!");
                return Page();
            }

            if (!unaControladora.VerificarStock(idInsumo, Controladora.UNIDADES_POR_VACUNACION))
            {
                ModelState.AddModelError(string.Empty,
                    "No hay stock de la vacuna: quedan " + unaVacuna.StockActual.ToString("N2") + " unidades.");
                return Page();
            }

            // El plan en nulo es la vacunacion aplicada fuera de todo plan
            PlanSanitario unPlan = unaControladora.BuscarPlanSanitario(idPlan);

            Vacunacion unaVacunacion = new Vacunacion(0, fechaAplicacion, unAnimal, unaVacuna, unPlan);

            if (unaControladora.AltaVacunacion(unaVacunacion))
            {
                return Redirect("./ListaDiagnosticos");
            }

            ModelState.AddModelError(string.Empty, "No se pudo registrar la vacunacion!");
            return Page();
        }

        private void CargarListados(Controladora pControladoraDominio)
        {
            animales = pControladoraDominio.ListarAnimales();
            vacunas = pControladoraDominio.ListarInsumosXTipo(Insumo.VACUNA);
            planes = pControladoraDominio.ListarPlanesXTipo(PlanSanitario.VACUNACION);
            vacunaciones = pControladoraDominio.ListarVacunaciones();
        }

        private void LeerFormulario()
        {
            numCaravana = Request.Form["numCaravana"];

            int vIdInsumo = 0;
            int.TryParse(Request.Form["idInsumo"], out vIdInsumo);
            idInsumo = vIdInsumo;

            int vIdPlan = 0;
            int.TryParse(Request.Form["idPlan"], out vIdPlan);
            idPlan = vIdPlan;

            fechaAplicacion = Request.Form["fechaAplicacion"] != ""
                ? Convert.ToDateTime(Request.Form["fechaAplicacion"])
                : DateTime.Now;
        }
    }
}
