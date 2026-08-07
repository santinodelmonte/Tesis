using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tesis.Dominio;

namespace Tesis.Pages.PagesReproduccion
{
    // CU16 - Registrar Tacto y Confirmacion de Preniez
    public class RegistrarTactoModel : PageModel
    {
        [BindProperty]
        public string? numCaravana { get; set; } = "";
        [BindProperty]
        public DateTime fechaTacto { get; set; } = DateTime.Now;
        [BindProperty]
        public string resultado { get; set; } = Tacto.PRENADA;
        [BindProperty]
        public string? observaciones { get; set; } = "";

        public List<Animal> animales = new List<Animal>();

        // Servicio sobre el que se hace el control y su historial de tactos
        public Servicio servicioVigente = null;
        public List<Tacto> tactosDelServicio = new List<Tacto>();

        public void OnGet(string caravana)
        {
            Controladora unaControladora = new Controladora();
            animales = unaControladora.ListarAnimales();

            if (caravana != null && caravana != "")
            {
                numCaravana = caravana;
                this.CargarServicio(unaControladora);
            }
        }

        // Trae el servicio vigente de la hembra para que el usuario vea sobre que
        // servicio va a quedar asentado el tacto antes de guardar
        public void OnPostBuscarServicio()
        {
            Controladora unaControladora = new Controladora();
            animales = unaControladora.ListarAnimales();

            this.LeerFormulario();
            ModelState.Clear();

            this.CargarServicio(unaControladora);

            if (servicioVigente == null && numCaravana != null && numCaravana != "")
            {
                ModelState.AddModelError(string.Empty,
                    "El animal no tiene un servicio pendiente: hay que registrar el servicio antes del tacto.");
            }
        }

        public IActionResult OnPostGuardar()
        {
            Controladora unaControladora = new Controladora();
            animales = unaControladora.ListarAnimales();

            this.LeerFormulario();
            this.CargarServicio(unaControladora);

            if (numCaravana == null || numCaravana == "")
            {
                ModelState.AddModelError(string.Empty, "Seleccione un animal!");
                return Page();
            }

            if (servicioVigente == null)
            {
                ModelState.AddModelError(string.Empty,
                    "El animal no tiene un servicio pendiente sobre el cual registrar el tacto!");
                return Page();
            }

            if (resultado != Tacto.PRENADA && resultado != Tacto.VACIA && resultado != Tacto.DUDOSA)
            {
                ModelState.AddModelError(string.Empty, "Es obligatorio definir un resultado para el tacto!");
                return Page();
            }

            if (fechaTacto > DateTime.Now)
            {
                ModelState.AddModelError(string.Empty, "La fecha del tacto no puede ser futura!");
                return Page();
            }

            if (fechaTacto < servicioVigente.FechaServicio)
            {
                ModelState.AddModelError(string.Empty,
                    "La fecha del tacto no puede ser anterior a la del servicio (" +
                    servicioVigente.FechaServicio.ToShortDateString() + ")!");
                return Page();
            }

            Tacto unTacto = new Tacto(0, fechaTacto, resultado, observaciones ?? "", servicioVigente);

            // El tacto mueve el estado reproductivo y nunca el productivo, y con
            // resultado positivo baja la fecha probable de parto a la lactancia en curso
            if (unaControladora.AltaTacto(unTacto))
            {
                return Redirect("./ListaServicios");
            }

            ModelState.AddModelError(string.Empty, "No se pudo registrar el tacto!");
            return Page();
        }

        private void CargarServicio(Controladora pControladoraDominio)
        {
            if (numCaravana == null || numCaravana == "")
            {
                return;
            }

            Animal unAnimal = pControladoraDominio.BuscarAnimalXCaravana(numCaravana);
            if (unAnimal == null || !(unAnimal is Hembra))
            {
                return;
            }

            servicioVigente = pControladoraDominio.ServicioVigente((Hembra)unAnimal);

            if (servicioVigente != null)
            {
                tactosDelServicio = pControladoraDominio.FiltrarTactosXServicio(servicioVigente.IdServicio);
            }
        }

        private void LeerFormulario()
        {
            numCaravana = Request.Form["numCaravana"];
            fechaTacto = Request.Form["fechaTacto"] != "" ? Convert.ToDateTime(Request.Form["fechaTacto"]) : DateTime.Now;
            resultado = Request.Form["resultado"] != "" ? Request.Form["resultado"] : Tacto.PRENADA;
            observaciones = Request.Form["observaciones"];
        }
    }
}
