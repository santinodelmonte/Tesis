using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tesis.Dominio;

namespace Tesis.Pages.PagesSanidad
{
    // Adelantado del Modulo 4 (CU19). Es el origen del tratamiento, y el tratamiento es
    // lo que define el periodo de descarte de leche.
    public class RegistrarDiagnosticoModel : PageModel
    {
        [BindProperty]
        public string? numCaravana { get; set; } = "";
        [BindProperty]
        public DateTime fechaDiagnostico { get; set; } = DateTime.Now;
        [BindProperty]
        public string? enfermedad { get; set; } = "";
        [BindProperty]
        public string estado { get; set; } = Diagnostico.ACTIVO;

        public List<Animal> animales = new List<Animal>();

        public void OnGet()
        {
            Controladora unaControladora = new Controladora();
            animales = unaControladora.ListarAnimales();
        }

        public IActionResult OnPostGuardar()
        {
            Controladora unaControladora = new Controladora();
            animales = unaControladora.ListarAnimales();

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

            if (enfermedad == null || enfermedad == "")
            {
                ModelState.AddModelError(string.Empty, "La enfermedad o el resultado de la revisacion es obligatorio!");
                return Page();
            }

            if (fechaDiagnostico > DateTime.Now)
            {
                ModelState.AddModelError(string.Empty, "La fecha del diagnostico no puede ser futura!");
                return Page();
            }

            Diagnostico unDiagnostico = new Diagnostico(0, fechaDiagnostico, enfermedad, estado, unAnimal);

            if (unaControladora.AltaDiagnostico(unDiagnostico))
            {
                return Redirect("./ListaDiagnosticos");
            }

            ModelState.AddModelError(string.Empty, "No se pudo registrar el diagnostico!");
            return Page();
        }

        private void LeerFormulario()
        {
            numCaravana = Request.Form["numCaravana"];
            fechaDiagnostico = Request.Form["fechaDiagnostico"] != "" ? Convert.ToDateTime(Request.Form["fechaDiagnostico"]) : DateTime.Now;
            enfermedad = Request.Form["enfermedad"];
            estado = Request.Form["estado"] != "" ? Request.Form["estado"] : Diagnostico.ACTIVO;
        }
    }
}
