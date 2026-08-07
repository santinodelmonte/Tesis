using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tesis.Dominio;

namespace Tesis.Pages.PagesReproduccion
{
    // Historial de servicios con el resultado del ultimo tacto de cada uno
    public class ListaServiciosModel : PageModel
    {
        public List<Servicio> servicios = new List<Servicio>();

        // Ultimo tacto de cada servicio y toro que aporto el material genetico
        public Dictionary<int, Tacto> ultimosTactos = new Dictionary<int, Tacto>();
        public Dictionary<int, Macho> toros = new Dictionary<int, Macho>();

        // Servicios sobre los que todavia tiene sentido ajustar la fecha de parto
        public List<int> conPrenezVigente = new List<int>();

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
            unaControladora.ListarAnimales();

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

        private void CargarListado(Controladora pControladoraDominio)
        {
            servicios = pControladoraDominio.ListarServicios();

            foreach (Servicio unServicio in servicios)
            {
                ultimosTactos.Add(unServicio.IdServicio, pControladoraDominio.UltimoTacto(unServicio));
                toros.Add(unServicio.IdServicio, pControladoraDominio.ToroDelServicio(unServicio));
            }

            foreach (Servicio unServicio in pControladoraDominio.ListarServiciosConPrenez())
            {
                conPrenezVigente.Add(unServicio.IdServicio);
            }
        }
    }
}
