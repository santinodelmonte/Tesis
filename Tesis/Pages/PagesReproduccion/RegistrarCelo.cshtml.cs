using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tesis.Dominio;

namespace Tesis.Pages.PagesReproduccion
{
    // CU20 - Registrar Deteccion de Celo.
    //
    // La misma pantalla da de alta y corrige. Con id en cero es un alta; con un
    // identificador se abre con el celo cargado y guarda encima. Es un solo formulario
    // porque los campos son los mismos y las reglas tambien: separar la correccion en
    // una pantalla propia habria significado mantener dos veces el mismo selector de
    // animal y la misma validacion.
    public class RegistrarCeloModel : PageModel
    {
        [BindProperty]
        public int id { get; set; } = 0;
        [BindProperty]
        public string? numCaravana { get; set; } = "";
        [BindProperty]
        public DateTime fecha { get; set; } = DateTime.Now;
        [BindProperty]
        public string? observaciones { get; set; } = "";

        public List<Animal> animales = new List<Animal>();

        public bool esCorreccion { get { return id > 0; } }

        public IActionResult OnGet(int id, string caravana)
        {
            Controladora unaControladora = new Controladora();
            animales = unaControladora.ListarAnimales();

            if (id > 0)
            {
                Celo unCelo = unaControladora.BuscarCelo(id);
                if (unCelo == null)
                {
                    return Redirect("./ListaCelos");
                }

                this.id = id;
                numCaravana = unCelo.Animal != null ? unCelo.Animal.NumCaravana : "";
                fecha = unCelo.Fecha;
                observaciones = unCelo.Observaciones;
                return Page();
            }

            if (caravana != null && caravana != "")
            {
                numCaravana = caravana;
            }
            return Page();
        }

        public IActionResult OnPostGuardar()
        {
            Controladora unaControladora = new Controladora();
            animales = unaControladora.ListarAnimales();

            this.LeerFormulario();

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

            // Curso de excepcion 1a
            if (!unaControladora.EsHembra(numCaravana))
            {
                ModelState.AddModelError(string.Empty, "La caravana corresponde a un macho: no se puede registrar un celo!");
                return Page();
            }

            if (id > 0)
            {
                return this.Corregir(unaControladora, unAnimal);
            }

            Celo unCelo = new Celo(0, fecha, observaciones ?? "", (Hembra)unAnimal);

            // La Controladora concentra las reglas: fecha no futura, edad minima a la
            // que la hembra empieza a ciclar y animal en el rodeo a esa fecha.
            string vMotivo = unaControladora.ValidarCelo(unCelo);
            if (vMotivo != "")
            {
                ModelState.AddModelError(string.Empty, vMotivo);
                return Page();
            }

            if (unaControladora.AltaCelo(unCelo))
            {
                return Redirect("./ListaCelos");
            }

            ModelState.AddModelError(string.Empty, "No se pudo registrar el celo!");
            return Page();
        }

        // La correccion valida con las mismas reglas del alta: un celo corregido tiene
        // que seguir siendo un celo posible.
        private IActionResult Corregir(Controladora pControladoraDominio, Animal pAnimal)
        {
            string vMotivo = pControladoraDominio.ValidarModificarCelo(id, fecha, pAnimal.IdAnimal);
            if (vMotivo != "")
            {
                ModelState.AddModelError(string.Empty, vMotivo);
                return Page();
            }

            if (pControladoraDominio.ModificarCelo(id, fecha, observaciones ?? "", pAnimal.IdAnimal))
            {
                return Redirect("./ListaCelos");
            }

            ModelState.AddModelError(string.Empty, "No se pudo corregir el celo!");
            return Page();
        }

        private void LeerFormulario()
        {
            int vId = 0;
            int.TryParse(Request.Form["id"], out vId);
            id = vId;

            numCaravana = Request.Form["numCaravana"];
            fecha = Request.Form["fecha"] != "" ? Convert.ToDateTime(Request.Form["fecha"]) : DateTime.Now;
            observaciones = Request.Form["observaciones"];
        }
    }
}
