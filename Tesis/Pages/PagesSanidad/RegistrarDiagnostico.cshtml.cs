using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tesis.Dominio;

namespace Tesis.Pages.PagesSanidad
{
    // Adelantado del Modulo 4 (CU27). Es el origen del tratamiento, y el tratamiento es
    // lo que define el periodo de descarte de leche.
    //
    // La misma pantalla da de alta y corrige: con id en cero es un alta, con un
    // identificador se abre con el diagnostico cargado.
    public class RegistrarDiagnosticoModel : PageModel
    {
        [BindProperty]
        public int id { get; set; } = 0;
        [BindProperty]
        public string? numCaravana { get; set; } = "";
        [BindProperty]
        public DateTime fechaDiagnostico { get; set; } = DateTime.Now;
        [BindProperty]
        public string? enfermedad { get; set; } = "";
        [BindProperty]
        public string estado { get; set; } = Diagnostico.ACTIVO;

        public List<Animal> animales = new List<Animal>();

        public bool esCorreccion { get { return id > 0; } }

        // Con tratamientos aplicados el animal queda fijo: el tratamiento se aplico
        // sobre esa vaca y su descarte de leche tambien.
        public bool animalBloqueado = false;

        public IActionResult OnGet(int id)
        {
            Controladora unaControladora = new Controladora();
            animales = unaControladora.ListarAnimales();

            if (id > 0)
            {
                Diagnostico unDiagnostico = unaControladora.BuscarDiagnostico(id);
                if (unDiagnostico == null)
                {
                    return Redirect("./ListaDiagnosticos");
                }

                this.id = id;
                numCaravana = unDiagnostico.Animal != null ? unDiagnostico.Animal.NumCaravana : "";
                fechaDiagnostico = unDiagnostico.FechaDiagnostico;
                enfermedad = unDiagnostico.Enfermedad;
                estado = unDiagnostico.Estado;
                animalBloqueado = unaControladora.FiltrarTratamientosXDiagnostico(id).Count > 0;
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

            if (id > 0)
            {
                return this.Corregir(unaControladora, unAnimal);
            }

            if (enfermedad == null || enfermedad == "")
            {
                ModelState.AddModelError(string.Empty, "La enfermedad o el resultado de la revisacion es obligatorio!");
                return Page();
            }

            if (fechaDiagnostico > DateTime.Now)
            {
                ModelState.AddModelError(string.Empty, "La fecha del diagnostico no puede ser futura!");
                return Page();
            }

            Diagnostico unDiagnostico = new Diagnostico(0, fechaDiagnostico, enfermedad, estado, unAnimal);

            if (unaControladora.AltaDiagnostico(unDiagnostico))
            {
                return Redirect("./ListaDiagnosticos");
            }

            ModelState.AddModelError(string.Empty, "No se pudo registrar el diagnostico!");
            return Page();
        }

        // El estado no viaja: lo mueven el alta del tratamiento y el boton de resolver
        // del listado, no esta pantalla.
        private IActionResult Corregir(Controladora pControladoraDominio, Animal pAnimal)
        {
            string vMotivo = pControladoraDominio.ValidarModificarDiagnostico(id, fechaDiagnostico,
                enfermedad ?? "", pAnimal.IdAnimal);

            if (vMotivo != "")
            {
                ModelState.AddModelError(string.Empty, vMotivo);
                return Page();
            }

            if (pControladoraDominio.ModificarDiagnostico(id, fechaDiagnostico, enfermedad ?? "",
                pAnimal.IdAnimal))
            {
                return Redirect("./ListaDiagnosticos");
            }

            ModelState.AddModelError(string.Empty, "No se pudo corregir el diagnostico!");
            return Page();
        }

        private void LeerFormulario()
        {
            int vId = 0;
            int.TryParse(Request.Form["id"], out vId);
            id = vId;

            numCaravana = Request.Form["numCaravana"];
            fechaDiagnostico = Request.Form["fechaDiagnostico"] != "" ? Convert.ToDateTime(Request.Form["fechaDiagnostico"]) : DateTime.Now;
            enfermedad = Request.Form["enfermedad"];
            estado = Request.Form["estado"] != "" ? Request.Form["estado"] : Diagnostico.ACTIVO;
        }
    }
}
