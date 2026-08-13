using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tesis.Dominio;

namespace Tesis.Pages.PagesSanidad
{
    // CU21. La aplicacion descuenta la dosis del stock y declara, si corresponde, que
    // plan sanitario da por cumplido: sin esa declaracion el calendario tendria que
    // adivinarlo a partir del insumo, y dos planes distintos pueden usar la misma
    // vacuna.
    //
    // La misma pantalla da de alta y corrige: con id en cero es un alta, con un
    // identificador se abre con la vacunacion cargada.
    public class RegistrarVacunacionModel : PageModel
    {
        [BindProperty]
        public int id { get; set; } = 0;
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

        public bool esCorreccion { get { return id > 0; } }

        public IActionResult OnGet(int id)
        {
            Controladora unaControladora = new Controladora();
            this.CargarListados(unaControladora);

            if (id > 0)
            {
                Vacunacion unaVacunacion = unaControladora.BuscarVacunacion(id);
                if (unaVacunacion == null)
                {
                    return Redirect("./ListaVacunaciones");
                }

                this.id = id;
                numCaravana = unaVacunacion.Animal != null ? unaVacunacion.Animal.NumCaravana : "";
                idInsumo = unaVacunacion.Insumo != null ? unaVacunacion.Insumo.IdInsumo : 0;
                idPlan = unaVacunacion.Plan != null ? unaVacunacion.Plan.IdPlan : 0;
                fechaAplicacion = unaVacunacion.FechaAplicacion;
            }
            return Page();
        }

        public IActionResult OnPostGuardar()
        {
            Controladora unaControladora = new Controladora();
            this.CargarListados(unaControladora);
            this.LeerFormulario();

            if (id > 0)
            {
                return this.Corregir(unaControladora);
            }

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
                return Redirect("./ListaVacunaciones");
            }

            ModelState.AddModelError(string.Empty, "No se pudo registrar la vacunacion!");
            return Page();
        }

        // Si la correccion cambia la vacuna, la Controladora devuelve la dosis anterior
        // al stock y descuenta la nueva.
        private IActionResult Corregir(Controladora pControladoraDominio)
        {
            Animal unAnimal = numCaravana != null && numCaravana != ""
                ? pControladoraDominio.BuscarAnimalXCaravana(numCaravana)
                : null;

            int vIdAnimal = unAnimal != null ? unAnimal.IdAnimal : 0;

            string vMotivo = pControladoraDominio.ValidarModificarVacunacion(id, fechaAplicacion,
                vIdAnimal, idInsumo, idPlan);

            if (vMotivo != "")
            {
                ModelState.AddModelError(string.Empty, vMotivo);
                return Page();
            }

            if (pControladoraDominio.ModificarVacunacion(id, fechaAplicacion, vIdAnimal, idInsumo, idPlan))
            {
                return Redirect("./ListaVacunaciones");
            }

            ModelState.AddModelError(string.Empty, "No se pudo corregir la vacunacion!");
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
            int vId = 0;
            int.TryParse(Request.Form["id"], out vId);
            id = vId;

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
