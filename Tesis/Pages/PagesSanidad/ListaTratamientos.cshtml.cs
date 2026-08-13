using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tesis.Dominio;

namespace Tesis.Pages.PagesSanidad
{
    // Tratamientos aplicados, y el aviso de que animales tienen la leche descartada.
    // El descarte se muestra aca y no en la pantalla de diagnosticos porque sale del
    // tratamiento: es su duracion mas la carencia del producto.
    public class ListaTratamientosModel : PageModel
    {
        public List<Tratamiento> tratamientos = new List<Tratamiento>();

        public List<Animal> animalesEnDescarte = new List<Animal>();
        public Dictionary<int, DateTime> descartesVigentes = new Dictionary<int, DateTime>();

        public void OnGet()
        {
            Controladora unaControladora = new Controladora();
            this.CargarListado(unaControladora);
        }

        public IActionResult OnPostEliminar(int id)
        {
            Controladora unaControladora = new Controladora();

            string vMotivo = unaControladora.ValidarEliminarTratamiento(id);
            if (vMotivo != "")
            {
                this.CargarListado(unaControladora);
                ModelState.AddModelError(string.Empty, vMotivo);
                return Page();
            }

            if (!unaControladora.EliminarTratamiento(id))
            {
                this.CargarListado(unaControladora);
                ModelState.AddModelError(string.Empty, "No se pudo eliminar el tratamiento!");
                return Page();
            }

            return RedirectToPage();
        }

        private void CargarListado(Controladora pControladoraDominio)
        {
            tratamientos = pControladoraDominio.ListarTratamientos();

            animalesEnDescarte = new List<Animal>();
            descartesVigentes = new Dictionary<int, DateTime>();

            foreach (Animal unAnimal in pControladoraDominio.ListarAnimales())
            {
                if (pControladoraDominio.TieneDescarteVigente(unAnimal))
                {
                    animalesEnDescarte.Add(unAnimal);
                    descartesVigentes.Add(unAnimal.IdAnimal,
                        pControladoraDominio.FechaFinDescarte(unAnimal));
                }
            }
        }
    }
}
