using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tesis.Dominio;

namespace Tesis.Pages.PagesInsumo
{
    // Adelantado del Modulo 5. Alcanza para cargar las pajuelas que CU15 necesita y
    // los productos sanitarios que CU20 consume.
    public class AltaInsumoModel : PageModel
    {
        [BindProperty]
        public string? nombre { get; set; } = "";
        [BindProperty]
        public string tipoInsumo { get; set; } = Insumo.PAJUELA;
        [BindProperty]
        public double stockActual { get; set; } = 0;
        [BindProperty]
        public double stockMinimo { get; set; } = 0;
        [BindProperty]
        public int periodoDescarteDias { get; set; } = 0;
        [BindProperty]
        public int idToro { get; set; } = 0;

        public List<Animal> animales = new List<Animal>();
        public string caravanaToro = "";

        public void OnGet()
        {
            Controladora unaControladora = new Controladora();
            animales = unaControladora.ListarAnimales();
        }

        public IActionResult OnPostAgregarInsumo()
        {
            Controladora unaControladora = new Controladora();
            animales = unaControladora.ListarAnimales();

            this.LeerFormulario(unaControladora);

            if (nombre == null || nombre == "")
            {
                ModelState.AddModelError(string.Empty, "El nombre del insumo es obligatorio!");
                return Page();
            }

            Macho unToro = unaControladora.BuscarMacho(idToro);

            // La pajuela existe para aportar material genetico: sin toro no sirve para
            // reconstruir la genealogia de la cria.
            if (tipoInsumo == Insumo.PAJUELA && unToro == null)
            {
                ModelState.AddModelError(string.Empty,
                    "La pajuela tiene que estar vinculada al toro que la aporta. Si el toro no integra el rodeo, cárguelo como animal con 'En pie' desmarcado.");
                return Page();
            }

            if (stockActual < 0 || stockMinimo < 0 || periodoDescarteDias < 0)
            {
                ModelState.AddModelError(string.Empty, "Los valores numericos no pueden ser negativos!");
                return Page();
            }

            Insumo unInsumo = new Insumo(0, nombre, tipoInsumo, stockActual, stockMinimo,
                periodoDescarteDias, tipoInsumo == Insumo.PAJUELA ? unToro : null);

            if (unaControladora.AltaInsumo(unInsumo))
            {
                return Redirect("./ListaInsumos");
            }

            ModelState.AddModelError(string.Empty, "No se pudo registrar el insumo!");
            return Page();
        }

        private void LeerFormulario(Controladora pControladoraDominio)
        {
            nombre = Request.Form["nombre"];
            tipoInsumo = Request.Form["tipoInsumo"] != "" ? Request.Form["tipoInsumo"] : Insumo.PAJUELA;

            double vStockActual = 0;
            double.TryParse(Request.Form["stockActual"], out vStockActual);
            stockActual = vStockActual;

            double vStockMinimo = 0;
            double.TryParse(Request.Form["stockMinimo"], out vStockMinimo);
            stockMinimo = vStockMinimo;

            int vDescarte = 0;
            int.TryParse(Request.Form["periodoDescarteDias"], out vDescarte);
            periodoDescarteDias = vDescarte;

            int vIdToro = 0;
            int.TryParse(Request.Form["idToro"], out vIdToro);
            idToro = vIdToro;

            Macho unToro = pControladoraDominio.BuscarMacho(idToro);
            caravanaToro = unToro != null ? unToro.NumCaravana : "";
        }
    }
}
