using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tesis.Dominio;

namespace Tesis.Pages.PagesProduccion
{
    // CU9 - Registrar Ordenie Individual, de a un animal.
    //
    // Es la variante puntual del control lechero, no una pantalla hermana: las dos
    // guardan el mismo registro con la misma regla -AltaOrdenieIndividual- y la unica
    // diferencia es cuantas vacas se cargan de una vez. La entrada normal es la carga
    // masiva, porque el control se hace una vez por mes y se mide todo el rodeo el mismo
    // dia; esta pantalla es para lo que esa no cubre: la vaca que faltó, una medicion
    // suelta o la correccion de un control ya cargado.
    //
    // La misma pantalla da de alta y corrige. Con id en cero es un alta; con un
    // identificador se abre con el control cargado y guarda encima, como los siete
    // Registrar* de Reproduccion y Sanidad.
    //
    // En la correccion solo se tocan los litros. La fecha, el turno y el animal son la
    // clave alterna del control: cambiarlos no es corregir este registro sino cargar
    // otro, y el caso real que hay detras -"lo anote en la vaca de al lado"- se resuelve
    // con la baja.
    public class OrdenieIndividualModel : PageModel
    {
        [BindProperty]
        public int id { get; set; } = 0;
        [BindProperty]
        public string turno { get; set; } = OrdenieLote.NombreTurno(1);
        [BindProperty]
        public DateTime fecha { get; set; } = DateTime.Now;
        [BindProperty]
        public string? numCaravana { get; set; } = "";
        [BindProperty]
        public double litros { get; set; } = 0;

        public List<Animal> animales = new List<Animal>();
        public List<OrdenieIndividual> ultimosControles = new List<OrdenieIndividual>();

        // Turnos de ordenie del establecimiento, segun cuantas veces por dia se ordenia
        public List<string> turnos = new List<string>();

        public bool esCorreccion { get { return id > 0; } }

        public IActionResult OnGet(int id, string caravana)
        {
            Controladora unaControladora = new Controladora();
            this.CargarListados(unaControladora);

            if (id > 0)
            {
                OrdenieIndividual unOrdenie = unaControladora.BuscarOrdenieIndividual(id);
                if (unOrdenie == null)
                {
                    return Redirect("./HistorialProduccion");
                }

                this.id = id;
                this.CopiarDelRegistro(unOrdenie);
                litros = unOrdenie.Litros;
                return Page();
            }

            // La ficha del animal entra por acá con la caravana ya elegida
            if (caravana != null && caravana != "")
            {
                numCaravana = caravana;
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
                    unOrdenieCargado.Litros.ToString("N2") + " litros. Corrijalo desde el historial de producción.");
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

        // La correccion reescribe los litros del control ya cargado. Fecha, turno y
        // animal se releen del registro y no del formulario: no son editables, asi que
        // el formulario ni siquiera los manda.
        private IActionResult Corregir(Controladora pControladoraDominio)
        {
            OrdenieIndividual unOrdenie = pControladoraDominio.BuscarOrdenieIndividual(id);
            if (unOrdenie == null)
            {
                ModelState.AddModelError(string.Empty, "El control que se quiere corregir ya no existe!");
                return Page();
            }

            this.CopiarDelRegistro(unOrdenie);

            string vMotivo = pControladoraDominio.ValidarModificarOrdenieIndividual(id, litros);
            if (vMotivo != "")
            {
                ModelState.AddModelError(string.Empty, vMotivo);
                return Page();
            }

            if (pControladoraDominio.ModificarOrdenieIndividual(id, litros))
            {
                return Redirect("./HistorialProduccion");
            }

            ModelState.AddModelError(string.Empty, "No se pudo corregir el control individual!");
            return Page();
        }

        // Los datos que la correccion no toca. Salen siempre del registro guardado, para
        // que la pantalla muestre a que control se le estan cambiando los litros aunque
        // el intento vuelva con errores.
        private void CopiarDelRegistro(OrdenieIndividual pOrdenie)
        {
            fecha = pOrdenie.Fecha;
            turno = pOrdenie.Turno;
            numCaravana = pOrdenie.Animal != null ? pOrdenie.Animal.NumCaravana : "";
        }

        private void CargarListados(Controladora pControladoraDominio)
        {
            animales = pControladoraDominio.ListarAnimales();
            turnos = pControladoraDominio.ListarTurnos();

            ultimosControles = pControladoraDominio.FiltrarOrdeniesIndividualXFecha(
                DateTime.Now.AddDays(-7), DateTime.Now);
        }

        private void LeerFormulario()
        {
            int vId = 0;
            int.TryParse(Request.Form["id"], out vId);
            id = vId;

            double vLitros = 0;
            double.TryParse(Request.Form["litros"], out vLitros);
            litros = vLitros;

            // En la correccion la pantalla no manda ni fecha, ni turno, ni caravana:
            // son la clave alterna del control y no se editan.
            if (id > 0)
            {
                return;
            }

            turno = Request.Form["turno"] != "" ? Request.Form["turno"] : OrdenieLote.NombreTurno(1);
            fecha = Request.Form["fecha"] != "" ? Convert.ToDateTime(Request.Form["fecha"]) : DateTime.Now;
            numCaravana = Request.Form["numCaravana"];
        }
    }
}
