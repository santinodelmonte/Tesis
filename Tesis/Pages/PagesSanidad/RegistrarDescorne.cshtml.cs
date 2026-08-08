using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tesis.Dominio;

namespace Tesis.Pages.PagesSanidad
{
    // CU24. El descorne no consume insumo y es de aplicacion unica: una vez
    // registrado, el plan de descorne deja de exigirlo para ese animal.
    public class RegistrarDescorneModel : PageModel
    {
        [BindProperty]
        public string? numCaravana { get; set; } = "";
        [BindProperty]
        public DateTime fecha { get; set; } = DateTime.Now;
        [BindProperty]
        public string metodo { get; set; } = Descorne.PASTA_CAUSTICA;
        [BindProperty]
        public string? observaciones { get; set; } = "";
        [BindProperty]
        public int idPlan { get; set; } = 0;

        public List<Animal> animales = new List<Animal>();
        public List<PlanSanitario> planes = new List<PlanSanitario>();
        public List<Descorne> descornes = new List<Descorne>();

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

            if (fecha > DateTime.Now)
            {
                ModelState.AddModelError(string.Empty, "La fecha del procedimiento no puede ser futura!");
                return Page();
            }

            if (unaControladora.TieneDescorne(unAnimal))
            {
                ModelState.AddModelError(string.Empty,
                    "El animal ya tiene un descorne registrado: es un procedimiento de aplicacion unica.");
                return Page();
            }

            // El plan en nulo es el descorne registrado fuera de todo plan
            PlanSanitario unPlan = unaControladora.BuscarPlanSanitario(idPlan);

            Descorne unDescorne = new Descorne(0, fecha, metodo, observaciones ?? "", unAnimal, unPlan);

            if (unaControladora.AltaDescorne(unDescorne))
            {
                return Redirect("./ListaDiagnosticos");
            }

            ModelState.AddModelError(string.Empty, "No se pudo registrar el descorne!");
            return Page();
        }

        private void CargarListados(Controladora pControladoraDominio)
        {
            animales = pControladoraDominio.ListarAnimales();
            planes = pControladoraDominio.ListarPlanesXTipo(PlanSanitario.DESCORNE);
            descornes = pControladoraDominio.ListarDescornes();
        }

        private void LeerFormulario()
        {
            numCaravana = Request.Form["numCaravana"];
            fecha = Request.Form["fecha"] != "" ? Convert.ToDateTime(Request.Form["fecha"]) : DateTime.Now;
            metodo = Request.Form["metodo"] != "" ? Request.Form["metodo"] : Descorne.PASTA_CAUSTICA;
            observaciones = Request.Form["observaciones"];

            int vIdPlan = 0;
            int.TryParse(Request.Form["idPlan"], out vIdPlan);
            idPlan = vIdPlan;
        }
    }
}
