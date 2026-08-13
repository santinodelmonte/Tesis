using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tesis.Dominio;

namespace Tesis.Pages.PagesReproduccion
{
    // Historial de servicios con el resultado del ultimo tacto de cada uno, y la
    // pantalla desde la que se los gestiona: registrar, corregir y eliminar.
    public class ListaServiciosModel : PageModel
    {
        public List<Servicio> servicios = new List<Servicio>();

        // Ultimo tacto de cada servicio y toro que aporto el material genetico
        public Dictionary<int, Tacto> ultimosTactos = new Dictionary<int, Tacto>();
        public Dictionary<int, Macho> toros = new Dictionary<int, Macho>();

        // Servicios sobre los que todavia tiene sentido ajustar la fecha de parto
        public List<int> conPrenezVigente = new List<int>();

        // Motivo por el que cada servicio no se puede eliminar, vacio si se puede. Se
        // calcula al armar el listado y no al apretar el boton: la persona tiene que
        // ver antes de intentar que ese servicio tiene tactos colgando.
        public Dictionary<int, string> bloqueos = new Dictionary<int, string>();

        public void OnGet()
        {
            Controladora unaControladora = new Controladora();
            this.CargarListado(unaControladora);
        }

        // Curso alternativo 7a de CU15: el usuario ajusta la fecha probable de parto que
        // propuso el sistema. El ajuste baja tambien a la lactancia en curso, que es de
        // donde sale la fecha recomendada de secado.
        public IActionResult OnPostAjustarFechaParto(int id)
        {
            Controladora unaControladora = new Controladora();

            DateTime vFechaProbableParto = DateTime.MinValue;
            if (Request.Form["fechaProbableParto"] != "")
            {
                vFechaProbableParto = Convert.ToDateTime(Request.Form["fechaProbableParto"]);
            }

            if (vFechaProbableParto == DateTime.MinValue)
            {
                this.CargarListado(unaControladora);
                ModelState.AddModelError(string.Empty, "Indique la fecha probable de parto!");
                return Page();
            }

            if (!unaControladora.ModificarServicio(id, vFechaProbableParto))
            {
                this.CargarListado(unaControladora);
                ModelState.AddModelError(string.Empty,
                    "No se pudo ajustar la fecha: tiene que ser posterior a la fecha del servicio.");
                return Page();
            }

            return RedirectToPage();
        }

        public IActionResult OnPostEliminar(int id)
        {
            Controladora unaControladora = new Controladora();

            string vMotivo = unaControladora.ValidarEliminarServicio(id);
            if (vMotivo != "")
            {
                this.CargarListado(unaControladora);
                ModelState.AddModelError(string.Empty, vMotivo);
                return Page();
            }

            if (!unaControladora.EliminarServicio(id))
            {
                this.CargarListado(unaControladora);
                ModelState.AddModelError(string.Empty, "No se pudo eliminar el servicio!");
                return Page();
            }

            return RedirectToPage();
        }

        private void CargarListado(Controladora pControladoraDominio)
        {
            servicios = pControladoraDominio.ListarServicios();

            foreach (Servicio unServicio in servicios)
            {
                ultimosTactos.Add(unServicio.IdServicio, pControladoraDominio.UltimoTacto(unServicio));
                toros.Add(unServicio.IdServicio, pControladoraDominio.ToroDelServicio(unServicio));
                bloqueos.Add(unServicio.IdServicio,
                    pControladoraDominio.ValidarEliminarServicio(unServicio.IdServicio));
            }

            foreach (Servicio unServicio in pControladoraDominio.ListarServiciosConPrenez())
            {
                conPrenezVigente.Add(unServicio.IdServicio);
            }
        }
    }
}
