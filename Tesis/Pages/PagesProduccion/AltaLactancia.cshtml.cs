using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tesis.Dominio;

namespace Tesis.Pages.PagesProduccion
{
    // Apertura manual de lactancia.
    //
    // No sale de ningun caso de uso del documento: el unico camino previsto para abrir
    // una lactancia es el parto (CU24). Hace falta para las vacas que ya estaban en
    // ordenie cuando arranco el sistema, que no tienen un parto registrado y que sin
    // esta pantalla no podrian recibir controles individuales (CU13) ni secarse (CU16).
    public class AltaLactanciaModel : PageModel
    {
        [BindProperty]
        public string? numCaravana { get; set; } = "";
        [BindProperty]
        public int numeroLactancia { get; set; } = 0;
        [BindProperty]
        public DateTime fechaInicio { get; set; } = DateTime.Now;

        public List<Animal> animales = new List<Animal>();

        // Hembras que todavia no tienen ninguna lactancia abierta
        public List<Hembra> sinLactancia = new List<Hembra>();

        public void OnGet()
        {
            Controladora unaControladora = new Controladora();
            this.CargarListados(unaControladora);
        }

        // El numero de lactancia lo propone el sistema a partir de los partos
        // registrados de la hembra y de sus lactancias anteriores
        public void OnPostProponerNumero()
        {
            Controladora unaControladora = new Controladora();
            this.CargarListados(unaControladora);
            this.LeerFormulario();

            ModelState.Clear();

            if (numCaravana != null && numCaravana != "")
            {
                Animal unAnimal = unaControladora.BuscarAnimalXCaravana(numCaravana);
                if (unAnimal is Hembra)
                {
                    numeroLactancia = unaControladora.ProximoNumeroLactancia((Hembra)unAnimal);
                }
            }
        }

        public IActionResult OnPostAbrirLactancia()
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

            Hembra unaHembra = (Hembra)unAnimal;

            if (unaControladora.LactanciaActual(unaHembra) != null)
            {
                ModelState.AddModelError(string.Empty, "El animal ya tiene una lactancia abierta!");
                return Page();
            }

            if (fechaInicio > DateTime.Now)
            {
                ModelState.AddModelError(string.Empty, "La fecha de inicio no puede ser futura!");
                return Page();
            }

            if (fechaInicio < unaHembra.FechaNacimiento)
            {
                ModelState.AddModelError(string.Empty, "La fecha de inicio no puede ser anterior al nacimiento del animal!");
                return Page();
            }

            if (numeroLactancia <= 0)
            {
                numeroLactancia = unaControladora.ProximoNumeroLactancia(unaHembra);
            }

            // Sin fecha de secado y sin fecha probable de parto: la primera la pone el
            // secado y la segunda el tacto que confirma la preniez.
            Lactancia unaLactancia = new Lactancia(0, numeroLactancia, fechaInicio,
                DateTime.MinValue, DateTime.MinValue, unaHembra);

            if (unaControladora.AltaLactancia(unaLactancia))
            {
                return Redirect("./ListaLactancias");
            }

            ModelState.AddModelError(string.Empty, "No se pudo abrir la lactancia!");
            return Page();
        }

        private void CargarListados(Controladora pControladoraDominio)
        {
            animales = pControladoraDominio.ListarAnimales();

            sinLactancia = new List<Hembra>();
            foreach (Hembra unaHembra in pControladoraDominio.ListarHembras())
            {
                if (unaHembra.Activo && pControladoraDominio.LactanciaActual(unaHembra) == null)
                {
                    sinLactancia.Add(unaHembra);
                }
            }
        }

        private void LeerFormulario()
        {
            numCaravana = Request.Form["numCaravana"];
            fechaInicio = Request.Form["fechaInicio"] != "" ? Convert.ToDateTime(Request.Form["fechaInicio"]) : DateTime.Now;

            int vNumero = 0;
            int.TryParse(Request.Form["numeroLactancia"], out vNumero);
            numeroLactancia = vNumero;
        }
    }
}
