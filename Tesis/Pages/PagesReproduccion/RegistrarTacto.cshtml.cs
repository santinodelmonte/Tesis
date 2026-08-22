using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tesis.Dominio;

namespace Tesis.Pages.PagesReproduccion
{
    // CU22 - Registrar Tacto y Confirmacion de Preniez.
    //
    // La misma pantalla da de alta y corrige. En el alta el tacto se asienta sobre el
    // servicio vigente de la hembra, que es lo que el caso de uso pide; en la
    // correccion el servicio ya esta decidido -es el del tacto que se abrio- y no se
    // vuelve a buscar, porque un control viejo puede estar colgado de un servicio que
    // hoy ya no es el vigente.
    public class RegistrarTactoModel : PageModel
    {
        [BindProperty]
        public int id { get; set; } = 0;
        [BindProperty]
        public int idServicio { get; set; } = 0;
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

        public bool esCorreccion { get { return id > 0; } }

        public IActionResult OnGet(int id, string caravana)
        {
            Controladora unaControladora = new Controladora();
            animales = unaControladora.ListarAnimales();

            if (id > 0)
            {
                Tacto unTacto = unaControladora.BuscarTacto(id);
                if (unTacto == null || unTacto.Servicio == null)
                {
                    return Redirect("./ListaTactos");
                }

                this.id = id;
                idServicio = unTacto.Servicio.IdServicio;
                fechaTacto = unTacto.FechaTacto;
                resultado = unTacto.Resultado;
                observaciones = unTacto.Observaciones;
                numCaravana = unTacto.Servicio.Animal != null ? unTacto.Servicio.Animal.NumCaravana : "";

                servicioVigente = unTacto.Servicio;
                tactosDelServicio = unaControladora.FiltrarTactosXServicio(idServicio);
                return Page();
            }

            if (caravana != null && caravana != "")
            {
                numCaravana = caravana;
                this.CargarServicio(unaControladora);
            }
            return Page();
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

            if (id > 0)
            {
                return this.Corregir(unaControladora);
            }

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

            Tacto unTacto = new Tacto(0, fechaTacto, resultado, observaciones ?? "", servicioVigente);

            // El resultado obligatorio y las dos reglas de fecha -no futura, no anterior
            // al servicio- las resuelve la Controladora, que es donde estan escritas una
            // sola vez para el alta, la correccion y el registro rapido del inicio.
            string vMotivo = unaControladora.ValidarTacto(unTacto);
            if (vMotivo != "")
            {
                ModelState.AddModelError(string.Empty, vMotivo);
                return Page();
            }

            // El tacto mueve el estado reproductivo y nunca el productivo, y con
            // resultado positivo baja la fecha probable de parto a la lactancia en curso
            if (unaControladora.AltaTacto(unTacto))
            {
                return Redirect("./ListaTactos");
            }

            ModelState.AddModelError(string.Empty, "No se pudo registrar el tacto!");
            return Page();
        }

        // Corregir un tacto vuelve a deducir el estado reproductivo de la vaca a partir
        // de los tactos que quedan, y con el la fecha probable de parto proyectada sobre
        // su lactancia. De eso se ocupa la Controladora.
        private IActionResult Corregir(Controladora pControladoraDominio)
        {
            servicioVigente = pControladoraDominio.BuscarServicio(idServicio);
            if (servicioVigente != null)
            {
                tactosDelServicio = pControladoraDominio.FiltrarTactosXServicio(idServicio);
            }

            string vMotivo = pControladoraDominio.ValidarModificarTacto(id, idServicio, fechaTacto, resultado);
            if (vMotivo != "")
            {
                ModelState.AddModelError(string.Empty, vMotivo);
                return Page();
            }

            if (pControladoraDominio.ModificarTacto(id, idServicio, fechaTacto, resultado,
                observaciones ?? ""))
            {
                return Redirect("./ListaTactos");
            }

            ModelState.AddModelError(string.Empty, "No se pudo corregir el tacto!");
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
                idServicio = servicioVigente.IdServicio;
                tactosDelServicio = pControladoraDominio.FiltrarTactosXServicio(servicioVigente.IdServicio);
            }
        }

        private void LeerFormulario()
        {
            int vId = 0;
            int.TryParse(Request.Form["id"], out vId);
            id = vId;

            int vIdServicio = 0;
            int.TryParse(Request.Form["idServicio"], out vIdServicio);
            idServicio = vIdServicio;

            numCaravana = Request.Form["numCaravana"];
            fechaTacto = Request.Form["fechaTacto"] != "" ? Convert.ToDateTime(Request.Form["fechaTacto"]) : DateTime.Now;
            resultado = Request.Form["resultado"] != "" ? Request.Form["resultado"] : Tacto.PRENADA;
            observaciones = Request.Form["observaciones"];
        }
    }
}
