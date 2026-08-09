using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tesis.Dominio;

namespace Tesis.Pages.PagesProduccion
{
    // CU8 - Registrar Ordenie General (Por Lote)
    public class OrdenieLoteModel : PageModel
    {
        [BindProperty]
        public string turno { get; set; } = OrdenieLote.NombreTurno(1);
        [BindProperty]
        public DateTime fecha { get; set; } = DateTime.Now;
        [BindProperty]
        public double litrosTotales { get; set; } = 0;

        // Litros ya cargados como control individual en la fecha y el turno elegidos.
        // Es informacion de contexto: se muestran para poder comparar lo medido vaca por
        // vaca contra lo que dio el tanque, pero no se descuentan. Los litros del lote
        // son la leche del ordenie completo, y el control individual mide esa misma
        // leche repartida por animal.
        public double litrosIndividualesDelTurno = 0;

        // Hembras en lactancia que el sistema propone para el lote
        // Turnos de ordenie del establecimiento, segun cuantas veces por dia se ordenia
        public List<string> turnos = new List<string>();

        public List<Hembra> animalesDelLote = new List<Hembra>();
        // Hembras en lactancia excluidas por tener un descarte de leche vigente
        public List<Hembra> animalesEnDescarte = new List<Hembra>();
        // Fecha hasta la que dura el descarte de cada una de las excluidas
        public Dictionary<int, DateTime> finDescarte = new Dictionary<int, DateTime>();

        // Ids que el usuario dejo tildados al volver de un error de validacion
        public List<int> seleccionados = new List<int>();

        public List<OrdenieLote> ultimosOrdenies = new List<OrdenieLote>();

        public void OnGet()
        {
            Controladora unaControladora = new Controladora();
            this.CargarListados(unaControladora);

            // Al entrar vienen tildados todos los que el sistema propone
            foreach (Hembra unaHembra in animalesDelLote)
            {
                seleccionados.Add(unaHembra.IdAnimal);
            }
        }

        public IActionResult OnPostGuardar()
        {
            Controladora unaControladora = new Controladora();
            this.CargarListados(unaControladora);
            this.LeerFormulario();

            if (!unaControladora.EsTurnoValido(turno))
            {
                ModelState.AddModelError(string.Empty, "Seleccione el turno!");
                return Page();
            }

            if (fecha > DateTime.Now)
            {
                ModelState.AddModelError(string.Empty, "La fecha del ordeñe no puede ser futura!");
                return Page();
            }

            // Curso de excepcion 5a
            if (!unaControladora.ValidarLitros(litrosTotales))
            {
                ModelState.AddModelError(string.Empty, "Los litros tienen que ser un valor positivo y coherente!");
                return Page();
            }

            if (unaControladora.BuscarOrdenieLoteXFechaTurno(fecha, turno) != null)
            {
                ModelState.AddModelError(string.Empty,
                    "Ya hay un ordeñe registrado para esa fecha y ese turno. Para corregirlo, edítelo desde el historial.");
                return Page();
            }

            // Lo que se guarda son siempre los litros del ordeñe masivo, sin la leche
            // de las vacas controladas aparte: asi el historial puede sumar las dos
            // fuentes sin contar nada dos veces.
            litrosIndividualesDelTurno = unaControladora.SumarLitrosIndividualesDelTurno(fecha, turno);
            double vLitrosDelLote = litrosTotales;

            if (litrosDelTanque)
            {
                vLitrosDelLote = litrosTotales - litrosIndividualesDelTurno;

                if (vLitrosDelLote <= 0)
                {
                    ModelState.AddModelError(string.Empty,
                        "Los controles individuales de ese turno ya suman " +
                        litrosIndividualesDelTurno.ToString("N2") + " litros, igual o más que el total del tanque. " +
                        "Revise el total, o marque que los litros son sólo del ordeñe masivo.");
                    return Page();
                }
            }

            // Los animales del lote salen de lo que quedo tildado, que puede no ser lo
            // que propuso el sistema: el paso 4 del caso de uso permite sacar los que no
            // se ordeniaron y agregar los que faltan.
            List<Hembra> _animalesElegidos = new List<Hembra>();
            foreach (int vIdAnimal in seleccionados)
            {
                Hembra unaHembra = unaControladora.BuscarHembra(vIdAnimal);
                if (unaHembra != null)
                {
                    _animalesElegidos.Add(unaHembra);
                }
            }

            if (_animalesElegidos.Count == 0)
            {
                ModelState.AddModelError(string.Empty, "El lote tiene que tener al menos un animal!");
                return Page();
            }

            // El tope de coherencia sale de la cantidad de animales ordeniados: un tope
            // fijo alto no atrapa el cero de mas en un rodeo chico.
            if (!unaControladora.ValidarLitrosLote(vLitrosDelLote, _animalesElegidos.Count))
            {
                ModelState.AddModelError(string.Empty,
                    "Los litros no son coherentes con la cantidad de animales del lote: " +
                    _animalesElegidos.Count + " animales no pueden dar " + vLitrosDelLote.ToString("N2") + " litros en un turno.");
                return Page();
            }

            OrdenieLote unOrdenie = new OrdenieLote(0, fecha, turno, vLitrosDelLote, _animalesElegidos);

            if (unaControladora.AltaOrdenieLote(unOrdenie))
            {
                return Redirect("./HistorialProduccion");
            }

            ModelState.AddModelError(string.Empty, "No se pudo registrar el ordeñe del lote!");
            return Page();
        }

        private void CargarListados(Controladora pControladoraDominio)
        {
            pControladoraDominio.ListarAnimales();

            // Los turnos salen de la cantidad de ordenies por dia del establecimiento
            turnos = pControladoraDominio.ListarTurnos();

            animalesDelLote = pControladoraDominio.ListarAnimalesParaOrdenie();
            animalesEnDescarte = pControladoraDominio.ListarHembrasEnDescarte();

            foreach (Hembra unaHembra in animalesEnDescarte)
            {
                finDescarte.Add(unaHembra.IdAnimal, pControladoraDominio.FechaFinDescarte(unaHembra));
            }

            ultimosOrdenies = pControladoraDominio.FiltrarOrdeniesLoteXFecha(
                DateTime.Now.AddDays(-7), DateTime.Now);
        }

        private void LeerFormulario()
        {
            turno = Request.Form["turno"] != "" ? Request.Form["turno"] : OrdenieLote.NombreTurno(1);
            fecha = Request.Form["fecha"] != "" ? Convert.ToDateTime(Request.Form["fecha"]) : DateTime.Now;

            // El campo puede venir con coma o con punto segun el teclado, y vacio si el
            // usuario no lo completo
            double vLitros = 0;
            double.TryParse(Request.Form["litrosTotales"], out vLitros);
            litrosTotales = vLitros;

            // El checkbox manda "true,false" cuando esta tildado

            seleccionados = new List<int>();
            foreach (string? vValor in Request.Form["animales"])
            {
                int vIdAnimal = 0;
                if (int.TryParse(vValor, out vIdAnimal))
                {
                    seleccionados.Add(vIdAnimal);
                }
            }
        }
    }
}
