using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tesis.Dominio;

namespace Tesis.Pages.PagesProduccion
{
    // Carga masiva del control lechero (CU13).
    //
    // El control se hace una vez por mes y se miden todas las vacas en ordeñe el mismo
    // dia. Cargarlas de a una, eligiendo la caravana cada vez, es media hora de clics:
    // esta pantalla muestra la lista completa con una columna de litros y guarda de una
    // sola vez las que tengan valor.
    //
    // Cada control se guarda por separado a traves de la misma regla de CU13, asi que si
    // una fila falla -por ejemplo, porque esa vaca ya tenia control ese dia y turno- las
    // demas se guardan igual y la pantalla informa cuales quedaron afuera.
    public class ControlLecheroModel : PageModel
    {
        [BindProperty]
        public DateTime fecha { get; set; } = DateTime.Now;
        [BindProperty]
        public string turno { get; set; } = OrdenieLote.NombreTurno(1);

        public List<Hembra> vacasEnOrdenie = new List<Hembra>();
        public List<string> turnos = new List<string>();

        // Lo tipeado, para no perderlo si la carga vuelve con errores
        public Dictionary<int, double> litrosCargados = new Dictionary<int, double>();

        public int guardados = 0;
        public List<string> rechazados = new List<string>();

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

            if (fecha > DateTime.Now)
            {
                ModelState.AddModelError(string.Empty, "La fecha del control no puede ser futura!");
                return Page();
            }

            if (!unaControladora.EsTurnoValido(turno))
            {
                ModelState.AddModelError(string.Empty, "Seleccione el turno!");
                return Page();
            }

            if (litrosCargados.Count == 0)
            {
                ModelState.AddModelError(string.Empty, "Cargue los litros de al menos un animal!");
                return Page();
            }

            foreach (KeyValuePair<int, double> unaCarga in litrosCargados)
            {
                Hembra unaHembra = unaControladora.BuscarHembra(unaCarga.Key);
                if (unaHembra == null)
                {
                    continue;
                }

                OrdenieIndividual unOrdenie = new OrdenieIndividual(0, fecha, turno,
                    unaCarga.Value, unaHembra, null, null);

                if (unaControladora.AltaOrdenieIndividual(unOrdenie))
                {
                    guardados = guardados + 1;
                }
                else
                {
                    rechazados.Add(unaHembra.NumCaravana);
                }
            }

            // La lista se recarga para que los stocks y los controles del dia queden al dia
            this.CargarListados(unaControladora);

            if (guardados == 0)
            {
                ModelState.AddModelError(string.Empty,
                    "No se guardo ningun control. Revise que los animales sigan en ordeñe y que no tengan ya un control en esa fecha y turno!");
            }

            return Page();
        }

        private void CargarListados(Controladora pControladoraDominio)
        {
            pControladoraDominio.ListarAnimales();

            turnos = pControladoraDominio.ListarTurnos();
            vacasEnOrdenie = pControladoraDominio.ListarHembrasEnLactancia();
        }

        private void LeerFormulario()
        {
            fecha = Request.Form["fecha"] != "" ? Convert.ToDateTime(Request.Form["fecha"]) : DateTime.Now;
            turno = Request.Form["turno"] != "" ? Request.Form["turno"] : OrdenieLote.NombreTurno(1);

            // Cada fila manda su valor con el nombre litros_<idAnimal>. Las vacías se
            // ignoran: en un control siempre hay vacas que no se midieron.
            litrosCargados = new Dictionary<int, double>();

            foreach (Hembra unaHembra in vacasEnOrdenie)
            {
                string vValor = Request.Form["litros_" + unaHembra.IdAnimal].ToString();

                double vLitros = Shared.CampoNumerico.LeerDecimal(vValor);
                if (vLitros > 0)
                {
                    litrosCargados.Add(unaHembra.IdAnimal, vLitros);
                }
            }
        }
    }
}
