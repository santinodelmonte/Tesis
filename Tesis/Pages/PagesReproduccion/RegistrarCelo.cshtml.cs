using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tesis.Dominio;

namespace Tesis.Pages.PagesReproduccion
{
    // CU14 - Registrar Deteccion de Celo
    public class RegistrarCeloModel : PageModel
    {
        [BindProperty]
        public string? numCaravana { get; set; } = "";
        [BindProperty]
        public DateTime fecha { get; set; } = DateTime.Now;
        [BindProperty]
        public string? observaciones { get; set; } = "";

        public List<Animal> animales = new List<Animal>();

        public void OnGet(string caravana)
        {
            Controladora unaControladora = new Controladora();
            animales = unaControladora.ListarAnimales();

            if (caravana != null && caravana != "")
            {
                numCaravana = caravana;
            }
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

            // Curso de excepcion 1a
            if (!unaControladora.EsHembra(numCaravana))
            {
                ModelState.AddModelError(string.Empty, "La caravana corresponde a un macho: no se puede registrar un celo!");
                return Page();
            }

            if (fecha > DateTime.Now)
            {
                ModelState.AddModelError(string.Empty, "La fecha de deteccion no puede ser futura!");
                return Page();
            }

            Celo unCelo = new Celo(0, fecha, observaciones ?? "", (Hembra)unAnimal);

            if (unaControladora.AltaCelo(unCelo))
            {
                return Redirect("./ListaCelos");
            }

            ModelState.AddModelError(string.Empty, "No se pudo registrar el celo!");
            return Page();
        }

        private void LeerFormulario()
        {
            numCaravana = Request.Form["numCaravana"];
            fecha = Request.Form["fecha"] != "" ? Convert.ToDateTime(Request.Form["fecha"]) : DateTime.Now;
            observaciones = Request.Form["observaciones"];
        }
    }
}
