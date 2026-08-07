using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tesis.Dominio;

namespace Tesis.Pages.PagesProduccion
{
    // CU9 - Registrar Ordenie Individual
    public class OrdenieIndividualModel : PageModel
    {
        [BindProperty]
        public string turno { get; set; } = OrdenieLote.TURNO_1;
        [BindProperty]
        public DateTime fecha { get; set; } = DateTime.Now;
        [BindProperty]
        public string? numCaravana { get; set; } = "";
        [BindProperty]
        public double litros { get; set; } = 0;

        public List<Animal> animales = new List<Animal>();
        public List<OrdenieIndividual> ultimosControles = new List<OrdenieIndividual>();

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
            if (unAnimal == null || !(unAnimal is Hembra))
            {
                ModelState.AddModelError(string.Empty, "La caravana no corresponde a una hembra del rodeo!");
                return Page();
            }

            Hembra unaHembra = (Hembra)unAnimal;

            // Curso de excepcion 2a
            if (!unaControladora.EstaEnLactancia(numCaravana))
            {
                ModelState.AddModelError(string.Empty,
                    "El animal no se encuentra en lactancia: su estado productivo es " + unaHembra.EstadoProductivo + ".");
                return Page();
            }

            // Curso de excepcion 4a
            if (!unaControladora.ValidarLitrosIndividual(litros))
            {
                ModelState.AddModelError(string.Empty, "Los litros tienen que ser un valor positivo y coherente!");
                return Page();
            }

            if (fecha > DateTime.Now)
            {
                ModelState.AddModelError(string.Empty, "La fecha del control no puede ser futura!");
                return Page();
            }

            OrdenieIndividual unOrdenieCargado = unaControladora.BuscarOrdenieIndividualXFechaTurno(
                fecha, turno, unaHembra.IdAnimal);
            if (unOrdenieCargado != null)
            {
                ModelState.AddModelError(string.Empty,
                    "Ya hay un control de este animal para esa fecha y ese turno, de " +
                    unOrdenieCargado.Litros.ToString("N2") + " litros.");
                return Page();
            }

            // Se imputa a la lactancia que estaba en curso en la fecha del control, no a
            // la actual: la carga puede ser retroactiva
            if (unaControladora.LactanciaDeLaFecha(unaHembra, fecha) == null)
            {
                ModelState.AddModelError(string.Empty,
                    "El animal no tenia una lactancia abierta en esa fecha. Registrela desde Lactancias antes de cargar el control.");
                return Page();
            }

            // La lactancia y el ordenie de lote los resuelve la Controladora al guardar
            OrdenieIndividual unOrdenie = new OrdenieIndividual(0, fecha, turno, litros,
                unaHembra, null, null);

            if (unaControladora.AltaOrdenieIndividual(unOrdenie))
            {
                return Redirect("./HistorialProduccion");
            }

            ModelState.AddModelError(string.Empty, "No se pudo registrar el control individual!");
            return Page();
        }

        private void CargarListados(Controladora pControladoraDominio)
        {
            animales = pControladoraDominio.ListarAnimales();

            ultimosControles = pControladoraDominio.FiltrarOrdeniesIndividualXFecha(
                DateTime.Now.AddDays(-7), DateTime.Now);
        }

        private void LeerFormulario()
        {
            turno = Request.Form["turno"] != "" ? Request.Form["turno"] : OrdenieLote.TURNO_1;
            fecha = Request.Form["fecha"] != "" ? Convert.ToDateTime(Request.Form["fecha"]) : DateTime.Now;
            numCaravana = Request.Form["numCaravana"];

            double vLitros = 0;
            double.TryParse(Request.Form["litros"], out vLitros);
            litros = vLitros;
        }
    }
}
