using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tesis.Dominio;

namespace Tesis.Pages.PagesProduccion
{
    // Correccion de un ordenie por lote ya cargado.
    //
    // No sale de ningun caso de uso: CU12 sólo registra. Hace falta porque la fecha y el
    // turno son clave alterna -no se puede volver a cargar el mismo turno- y un error
    // de tipeo en los litros quedaba fijo para siempre.
    public class ModificarOrdenieLoteModel : PageModel
    {
        [BindProperty]
        public int idOrdenieLote { get; set; } = 0;
        [BindProperty]
        public double litrosTotales { get; set; } = 0;

        public OrdenieLote ordenie = null;
        public List<Hembra> animalesDisponibles = new List<Hembra>();
        public List<int> seleccionados = new List<int>();

        // Controles individuales de ese turno, que no estan incluidos en los litros
        public double litrosIndividualesDelTurno = 0;

        public void OnGet(int id)
        {
            Controladora unaControladora = new Controladora();
            idOrdenieLote = id;
            this.CargarOrdenie(unaControladora);

            if (ordenie != null)
            {
                litrosTotales = ordenie.LitrosTotales;

                foreach (Hembra unaHembra in ordenie.Animales)
                {
                    seleccionados.Add(unaHembra.IdAnimal);
                }
            }
        }

        public IActionResult OnPostGuardarCambios()
        {
            Controladora unaControladora = new Controladora();
            this.LeerFormulario();
            this.CargarOrdenie(unaControladora);

            if (ordenie == null)
            {
                ModelState.AddModelError(string.Empty, "No se encontro el ordeñe solicitado.");
                return Page();
            }

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

            if (!unaControladora.ValidarLitrosLote(litrosTotales, _animalesElegidos.Count))
            {
                ModelState.AddModelError(string.Empty,
                    "Los litros no son coherentes con la cantidad de animales del lote: " +
                    _animalesElegidos.Count + " animales no pueden dar " + litrosTotales.ToString("N2") + " litros en un turno.");
                return Page();
            }

            if (unaControladora.ModificarOrdenieLote(idOrdenieLote, litrosTotales, _animalesElegidos))
            {
                return Redirect("./HistorialProduccion");
            }

            ModelState.AddModelError(string.Empty, "No se pudo modificar el ordeñe del lote!");
            return Page();
        }

        private void CargarOrdenie(Controladora pControladoraDominio)
        {
            pControladoraDominio.ListarAnimales();
            ordenie = pControladoraDominio.BuscarOrdenieLote(idOrdenieLote);

            if (ordenie == null)
            {
                return;
            }

            litrosIndividualesDelTurno = pControladoraDominio.SumarLitrosIndividualesDelTurno(
                ordenie.Fecha, ordenie.Turno);

            // Se ofrecen las hembras que estan hoy en lactancia mas las que integraban
            // el lote, que pueden haberse secado desde entonces
            animalesDisponibles = pControladoraDominio.ListarHembrasEnLactancia();

            foreach (Hembra unaHembra in ordenie.Animales)
            {
                if (!animalesDisponibles.Contains(unaHembra))
                {
                    animalesDisponibles.Add(unaHembra);
                }
            }
        }

        private void LeerFormulario()
        {
            int vId = 0;
            int.TryParse(Request.Form["idOrdenieLote"], out vId);
            idOrdenieLote = vId;

            double vLitros = 0;
            double.TryParse(Request.Form["litrosTotales"], out vLitros);
            litrosTotales = vLitros;

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
