using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tesis.Dominio;

namespace Tesis.Pages.PagesProduccion
{
    // CU12 - Registrar Periodo de Secado Manual
    public class RegistrarSecadoModel : PageModel
    {
        [BindProperty]
        public string? numCaravana { get; set; } = "";
        [BindProperty]
        public DateTime fechaSecado { get; set; } = DateTime.Now;

        public List<Animal> animales = new List<Animal>();
        public List<Hembra> enLactancia = new List<Hembra>();

        // Fecha recomendada de secado de cada hembra en lactancia, para orientar la carga
        public Dictionary<int, DateTime> fechasRecomendadas = new Dictionary<int, DateTime>();

        public void OnGet(string caravana)
        {
            Controladora unaControladora = new Controladora();
            this.CargarListados(unaControladora);

            // La pantalla se abre desde las alertas de secado con la caravana ya elegida
            if (caravana != null && caravana != "")
            {
                numCaravana = caravana;
            }
        }

        public IActionResult OnPostRegistrarSecado()
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
            if (unAnimal == null || !(unAnimal is Hembra))
            {
                ModelState.AddModelError(string.Empty, "La caravana no corresponde a una hembra del rodeo!");
                return Page();
            }

            if (!unaControladora.EstaEnLactancia(numCaravana))
            {
                ModelState.AddModelError(string.Empty,
                    "El animal no se encuentra en lactancia, asi que no hay nada que secar!");
                return Page();
            }

            if (fechaSecado > DateTime.Now)
            {
                ModelState.AddModelError(string.Empty, "La fecha de secado no puede ser futura!");
                return Page();
            }

            // El secado cierra la lactancia vigente y pasa la hembra a seca. El estado
            // reproductivo no se toca: una hembra seca puede estar prenada.
            if (unaControladora.RegistrarSecado(numCaravana, fechaSecado))
            {
                return Redirect("./ListaLactancias");
            }

            ModelState.AddModelError(string.Empty,
                "No se pudo registrar el secado. Verifique que el animal tenga una lactancia abierta y que la fecha no sea anterior a su inicio.");
            return Page();
        }

        private void CargarListados(Controladora pControladoraDominio)
        {
            animales = pControladoraDominio.ListarAnimales();
            enLactancia = pControladoraDominio.ListarHembrasEnLactancia();

            foreach (Hembra unaHembra in enLactancia)
            {
                fechasRecomendadas.Add(unaHembra.IdAnimal, pControladoraDominio.CalcularFechaSecado(unaHembra));
            }
        }

        private void LeerFormulario()
        {
            numCaravana = Request.Form["numCaravana"];
            fechaSecado = Request.Form["fechaSecado"] != "" ? Convert.ToDateTime(Request.Form["fechaSecado"]) : DateTime.Now;
        }
    }
}
