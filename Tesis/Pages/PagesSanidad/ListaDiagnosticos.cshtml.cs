using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tesis.Dominio;

namespace Tesis.Pages.PagesSanidad
{
    // Diagnosticos del rodeo.
    //
    // Antes esta pantalla mostraba las cuatro tablas de Sanidad juntas -diagnosticos,
    // tratamientos, vacunaciones y descornes-, y era el unico lugar donde se veian.
    // Ahora cada una tiene la suya: la que gestiona sus propios registros es la que los
    // lista, y la ficha completa de un animal sigue estando en su detalle.
    public class ListaDiagnosticosModel : PageModel
    {
        public List<Diagnostico> diagnosticos = new List<Diagnostico>();

        // Tratamientos aplicados a cada diagnostico y motivo por el que no se puede
        // eliminar, vacio si se puede
        public Dictionary<int, int> tratamientosPorDiagnostico = new Dictionary<int, int>();
        public Dictionary<int, string> bloqueos = new Dictionary<int, string>();

        public void OnGet()
        {
            Controladora unaControladora = new Controladora();
            this.CargarListado(unaControladora);
        }

        // CU27: el diagnostico se cierra cuando el animal se recupera. Sin este paso una
        // afeccion ya resuelta seguiria ofreciendose para tratar.
        public IActionResult OnPostResolver(int id)
        {
            Controladora unaControladora = new Controladora();

            if (!unaControladora.ResolverDiagnostico(id))
            {
                this.CargarListado(unaControladora);
                ModelState.AddModelError(string.Empty, "No se pudo marcar el diagnostico como resuelto!");
                return Page();
            }

            return RedirectToPage();
        }

        public IActionResult OnPostEliminar(int id)
        {
            Controladora unaControladora = new Controladora();

            string vMotivo = unaControladora.ValidarEliminarDiagnostico(id);
            if (vMotivo != "")
            {
                this.CargarListado(unaControladora);
                ModelState.AddModelError(string.Empty, vMotivo);
                return Page();
            }

            if (!unaControladora.EliminarDiagnostico(id))
            {
                this.CargarListado(unaControladora);
                ModelState.AddModelError(string.Empty, "No se pudo eliminar el diagnostico!");
                return Page();
            }

            return RedirectToPage();
        }

        private void CargarListado(Controladora pControladoraDominio)
        {
            diagnosticos = pControladoraDominio.ListarDiagnosticos();

            foreach (Diagnostico unDiagnostico in diagnosticos)
            {
                tratamientosPorDiagnostico.Add(unDiagnostico.IdDiagnostico,
                    pControladoraDominio.FiltrarTratamientosXDiagnostico(unDiagnostico.IdDiagnostico).Count);

                bloqueos.Add(unDiagnostico.IdDiagnostico,
                    pControladoraDominio.ValidarEliminarDiagnostico(unDiagnostico.IdDiagnostico));
            }
        }
    }
}
