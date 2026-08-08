using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tesis.Dominio;

namespace Tesis.Pages.PagesSanidad
{
    // CU20. El tratamiento nace de un diagnostico pendiente o se aplica de forma
    // preventiva sobre el animal (curso alternativo 2a). En los dos casos descuenta el
    // producto del stock y fija hasta cuando hay que descartar la leche, que es lo que
    // el paso 3 de CU8 usa para dejar a la hembra fuera del lote de ordenie.
    public class RegistrarTratamientoModel : PageModel
    {
        [BindProperty]
        public int idDiagnostico { get; set; } = 0;
        [BindProperty]
        public string? numCaravana { get; set; } = "";
        [BindProperty]
        public int idInsumo { get; set; } = 0;
        [BindProperty]
        public int idPlan { get; set; } = 0;
        [BindProperty]
        public DateTime fechaInicio { get; set; } = DateTime.Now;
        [BindProperty]
        public int diasDuracion { get; set; } = 1;
        [BindProperty]
        public string? dosisDiaria { get; set; } = "";
        [BindProperty]
        public double cantidadInsumo { get; set; } = 0;
        [BindProperty]
        public DateTime fechaFinDescarte { get; set; } = DateTime.MinValue;

        public List<Diagnostico> diagnosticos = new List<Diagnostico>();
        public List<Insumo> insumos = new List<Insumo>();
        public List<PlanSanitario> planes = new List<PlanSanitario>();
        public List<Animal> animales = new List<Animal>();

        public void OnGet()
        {
            Controladora unaControladora = new Controladora();
            this.CargarListados(unaControladora);
        }

        // La fecha de fin de descarte se propone sumando los dias del tratamiento y el
        // periodo de carencia del insumo. El usuario puede ajustarla.
        public void OnPostCalcularDescarte()
        {
            Controladora unaControladora = new Controladora();
            this.CargarListados(unaControladora);
            this.LeerFormulario();

            ModelState.Clear();

            Tratamiento unTratamiento = this.ArmarTratamiento(unaControladora);
            if (unTratamiento != null)
            {
                fechaFinDescarte = unaControladora.CalcularDescarte(unTratamiento);
            }
        }

        public IActionResult OnPostGuardar()
        {
            Controladora unaControladora = new Controladora();
            this.CargarListados(unaControladora);
            this.LeerFormulario();

            if (idInsumo == 0)
            {
                ModelState.AddModelError(string.Empty, "Seleccione el producto aplicado!");
                return Page();
            }

            // El tratamiento se aplica sobre un animal: si no viene de un diagnostico
            // -o sea, es preventivo- hay que decir sobre cual.
            if (this.ResolverAnimal(unaControladora) == null)
            {
                ModelState.AddModelError(string.Empty,
                    "Seleccione el diagnostico a tratar, o la caravana del animal si el tratamiento es preventivo!");
                return Page();
            }

            if (diasDuracion <= 0)
            {
                ModelState.AddModelError(string.Empty, "La duracion del tratamiento tiene que ser de al menos un dia!");
                return Page();
            }

            if (dosisDiaria == null || dosisDiaria == "")
            {
                ModelState.AddModelError(string.Empty, "La dosis diaria es obligatoria!");
                return Page();
            }

            if (fechaInicio > DateTime.Now)
            {
                ModelState.AddModelError(string.Empty, "La fecha de inicio no puede ser futura!");
                return Page();
            }

            // Curso de excepcion 3a: sin stock suficiente el tratamiento no se guarda
            Insumo unInsumo = unaControladora.BuscarInsumo(idInsumo);
            if (unInsumo != null && !unaControladora.VerificarStock(idInsumo, cantidadInsumo))
            {
                ModelState.AddModelError(string.Empty,
                    "No hay stock suficiente del producto: quedan " + unInsumo.StockActual.ToString("N2") + " unidades.");
                return Page();
            }

            Tratamiento unTratamiento = this.ArmarTratamiento(unaControladora);
            if (unTratamiento == null)
            {
                ModelState.AddModelError(string.Empty, "No se pudo armar el tratamiento!");
                return Page();
            }

            if (unaControladora.AltaTratamiento(unTratamiento, cantidadInsumo))
            {
                return Redirect("./ListaDiagnosticos");
            }

            ModelState.AddModelError(string.Empty, "No se pudo registrar el tratamiento!");
            return Page();
        }

        private Tratamiento ArmarTratamiento(Controladora pControladoraDominio)
        {
            Insumo unInsumo = pControladoraDominio.BuscarInsumo(idInsumo);
            if (unInsumo == null)
            {
                return null;
            }

            // El animal puede faltar todavia cuando se pide calcular el descarte, que
            // solo depende del producto y de la duracion. Para guardar es obligatorio, y
            // eso lo controlan el handler y la Controladora.
            Animal unAnimal = this.ResolverAnimal(pControladoraDominio);

            // El diagnostico en nulo identifica al tratamiento preventivo, como la
            // desparasitacion, que no se origina en un diagnostico. El plan en nulo, al
            // tratamiento que no cumple ningun plan sanitario.
            Diagnostico unDiagnostico = pControladoraDominio.BuscarDiagnostico(idDiagnostico);
            PlanSanitario unPlan = pControladoraDominio.BuscarPlanSanitario(idPlan);

            return new Tratamiento(0, fechaInicio, diasDuracion, dosisDiaria ?? "",
                fechaFinDescarte, unDiagnostico, unAnimal, unInsumo, unPlan);
        }

        // El animal sale del diagnostico elegido. Si no hay diagnostico, el tratamiento
        // es preventivo y el animal es el que se selecciono por caravana.
        private Animal ResolverAnimal(Controladora pControladoraDominio)
        {
            Diagnostico unDiagnostico = pControladoraDominio.BuscarDiagnostico(idDiagnostico);
            if (unDiagnostico != null)
            {
                return unDiagnostico.Animal;
            }

            if (numCaravana == null || numCaravana == "")
            {
                return null;
            }
            return pControladoraDominio.BuscarAnimalXCaravana(numCaravana);
        }

        private void CargarListados(Controladora pControladoraDominio)
        {
            insumos = pControladoraDominio.ListarInsumosSanitarios();
            animales = pControladoraDominio.ListarAnimales();

            // Un tratamiento cumple un plan de desparasitacion: la vacunacion tiene su
            // propia pantalla y el descorne no consume producto.
            planes = pControladoraDominio.ListarPlanesXTipo(PlanSanitario.DESPARASITACION);

            // Solo los diagnosticos abiertos: no tiene sentido tratar uno resuelto
            diagnosticos = new List<Diagnostico>();
            foreach (Diagnostico unDiagnostico in pControladoraDominio.ListarDiagnosticos())
            {
                if (pControladoraDominio.DiagnosticoEstaActivo(unDiagnostico))
                {
                    diagnosticos.Add(unDiagnostico);
                }
            }
        }

        private void LeerFormulario()
        {
            int vIdDiagnostico = 0;
            int.TryParse(Request.Form["idDiagnostico"], out vIdDiagnostico);
            idDiagnostico = vIdDiagnostico;

            numCaravana = Request.Form["numCaravana"];

            int vIdInsumo = 0;
            int.TryParse(Request.Form["idInsumo"], out vIdInsumo);
            idInsumo = vIdInsumo;

            int vIdPlan = 0;
            int.TryParse(Request.Form["idPlan"], out vIdPlan);
            idPlan = vIdPlan;

            fechaInicio = Request.Form["fechaInicio"] != "" ? Convert.ToDateTime(Request.Form["fechaInicio"]) : DateTime.Now;

            int vDias = 1;
            int.TryParse(Request.Form["diasDuracion"], out vDias);
            diasDuracion = vDias;

            dosisDiaria = Request.Form["dosisDiaria"];

            double vCantidad = 0;
            double.TryParse(Request.Form["cantidadInsumo"], out vCantidad);
            cantidadInsumo = vCantidad;

            fechaFinDescarte = Request.Form["fechaFinDescarte"] != ""
                ? Convert.ToDateTime(Request.Form["fechaFinDescarte"])
                : DateTime.MinValue;
        }
    }
}
