using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tesis.Dominio;

namespace Tesis.Pages.PagesReproduccion
{
    // Correccion de un parto ya registrado.
    //
    // A diferencia de las demas correcciones, esta no reusa la pantalla de alta. El
    // alta del parto carga ademas las crias, con su caravana, su raza y su foto, y esas
    // ya son animales del rodeo: se corrigen desde Animales, como cualquier otro.
    // Aca quedan los tres campos que son del parto y de nadie mas.
    //
    // La fecha no es un campo inocuo: arrastra el inicio de la lactancia que el parto
    // abrio, el secado de la que cerro y el nacimiento de las crias. La Controladora
    // valida que ninguna de las tres cosas termine contradiciendo un registro
    // posterior y mueve las tres a la vez.
    public class ModificarPartoModel : PageModel
    {
        [BindProperty]
        public int id { get; set; } = 0;
        [BindProperty]
        public DateTime fechaParto { get; set; } = DateTime.Now;
        [BindProperty]
        public string tipoParto { get; set; } = Parto.NORMAL;
        [BindProperty]
        public string? observaciones { get; set; } = "";

        public string caravanaMadre = "";
        public List<Animal> crias = new List<Animal>();

        public IActionResult OnGet(int id)
        {
            Controladora unaControladora = new Controladora();

            Parto unParto = unaControladora.BuscarParto(id);
            if (unParto == null)
            {
                return Redirect("./ListaPartos");
            }

            this.id = id;
            fechaParto = unParto.FechaParto;
            tipoParto = unParto.TipoParto;
            observaciones = unParto.Observaciones;

            this.CargarContexto(unaControladora, unParto);
            return Page();
        }

        public IActionResult OnPostGuardar()
        {
            Controladora unaControladora = new Controladora();
            this.LeerFormulario();

            Parto unParto = unaControladora.BuscarParto(id);
            if (unParto == null)
            {
                return Redirect("./ListaPartos");
            }

            this.CargarContexto(unaControladora, unParto);

            string vMotivo = unaControladora.ValidarModificarParto(id, fechaParto, tipoParto);
            if (vMotivo != "")
            {
                ModelState.AddModelError(string.Empty, vMotivo);
                return Page();
            }

            if (unaControladora.ModificarParto(id, fechaParto, tipoParto, observaciones ?? ""))
            {
                return Redirect("./ListaPartos");
            }

            ModelState.AddModelError(string.Empty, "No se pudo corregir el parto!");
            return Page();
        }

        private void CargarContexto(Controladora pControladoraDominio, Parto pParto)
        {
            caravanaMadre = pParto.Madre != null ? pParto.Madre.NumCaravana : "";
            crias = pControladoraDominio.CriasDelParto(pParto);
        }

        private void LeerFormulario()
        {
            int vId = 0;
            int.TryParse(Request.Form["id"], out vId);
            id = vId;

            fechaParto = Request.Form["fechaParto"] != ""
                ? Convert.ToDateTime(Request.Form["fechaParto"])
                : DateTime.Now;

            tipoParto = Request.Form["tipoParto"] != "" ? Request.Form["tipoParto"] : Parto.NORMAL;
            observaciones = Request.Form["observaciones"];
        }
    }
}
