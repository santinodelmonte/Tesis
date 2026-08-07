using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tesis.Dominio;

namespace Tesis.Pages.PagesSanidad
{
    // Adelantado del Modulo 4 (CU20). Lo que interesa para el Modulo 2 es la fecha de
    // fin de descarte: mientras no venza, la hembra tratada queda fuera del lote de
    // ordenie de CU8.
    public class RegistrarTratamientoModel : PageModel
    {
        [BindProperty]
        public int idDiagnostico { get; set; } = 0;
        [BindProperty]
        public int idInsumo { get; set; } = 0;
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

            Insumo unInsumo = unaControladora.BuscarInsumo(idInsumo);
            if (unInsumo != null && cantidadInsumo > unInsumo.StockActual)
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

            // El diagnostico en nulo identifica al tratamiento preventivo, como la
            // desparasitacion, que no se origina en un diagnostico.
            Diagnostico unDiagnostico = pControladoraDominio.BuscarDiagnostico(idDiagnostico);

            return new Tratamiento(0, fechaInicio, diasDuracion, dosisDiaria ?? "",
                fechaFinDescarte, unDiagnostico, unInsumo);
        }

        private void CargarListados(Controladora pControladoraDominio)
        {
            insumos = pControladoraDominio.ListarInsumosSanitarios();

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

            int vIdInsumo = 0;
            int.TryParse(Request.Form["idInsumo"], out vIdInsumo);
            idInsumo = vIdInsumo;

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
