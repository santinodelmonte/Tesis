using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tesis.Dominio;

namespace Tesis.Pages.PagesReproduccion
{
    // Historial de partos. Es tambien desde donde se corrige uno mal cargado y desde
    // donde se lo deshace, que es la operacion mas pesada del sistema: se lleva las
    // crias que el parto dio de alta y la lactancia que abrio.
    public class ListaPartosModel : PageModel
    {
        public List<Parto> partos = new List<Parto>();

        // Crias de cada parto y motivo por el que no se puede deshacer, vacio si se
        // puede. Las dos cosas se calculan al armar el listado: la persona tiene que
        // ver que se lleva el borrado antes de apretar el boton, no despues.
        public Dictionary<int, List<Animal>> crias = new Dictionary<int, List<Animal>>();
        public Dictionary<int, string> bloqueos = new Dictionary<int, string>();

        public void OnGet()
        {
            Controladora unaControladora = new Controladora();
            this.CargarListado(unaControladora);
        }

        public IActionResult OnPostEliminar(int id)
        {
            Controladora unaControladora = new Controladora();

            string vMotivo = unaControladora.ValidarEliminarParto(id);
            if (vMotivo != "")
            {
                this.CargarListado(unaControladora);
                ModelState.AddModelError(string.Empty, vMotivo);
                return Page();
            }

            if (!unaControladora.EliminarParto(id))
            {
                this.CargarListado(unaControladora);
                ModelState.AddModelError(string.Empty, "No se pudo eliminar el parto!");
                return Page();
            }

            return RedirectToPage();
        }

        private void CargarListado(Controladora pControladoraDominio)
        {
            partos = pControladoraDominio.ListarPartos();

            foreach (Parto unParto in partos)
            {
                crias.Add(unParto.IdParto, pControladoraDominio.CriasDelParto(unParto));
                bloqueos.Add(unParto.IdParto, pControladoraDominio.ValidarEliminarParto(unParto.IdParto));
            }
        }
    }
}
