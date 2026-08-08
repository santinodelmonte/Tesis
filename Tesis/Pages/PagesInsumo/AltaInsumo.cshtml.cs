using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tesis.Dominio;

namespace Tesis.Pages.PagesInsumo
{
    // CU25. El alta del insumo y el ingreso de su primera partida son un solo tramite:
    // la existencia inicial entra como movimiento de stock, con su vencimiento, para
    // que la alerta de CU28 y el historial de CU29 arranquen completos.
    public class AltaInsumoModel : PageModel
    {
        [BindProperty]
        public string? nombre { get; set; } = "";
        [BindProperty]
        public string tipoInsumo { get; set; } = Insumo.PAJUELA;
        [BindProperty]
        public double cantidadInicial { get; set; } = 0;
        [BindProperty]
        public DateTime fechaVencimiento { get; set; } = DateTime.MinValue;
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

            // Curso de excepcion 4a: la cantidad no puede ser negativa. El cero se
            // admite para dar de alta el producto antes de que llegue la mercaderia.
            if (cantidadInicial < 0 || stockMinimo < 0 || periodoDescarteDias < 0)
            {
                ModelState.AddModelError(string.Empty, "Los valores numericos no pueden ser negativos!");
                return Page();
            }

            // Paso 3: el insumo es nuevo o es una reposicion de uno que ya existe. La
            // reposicion se carga como ingreso de stock, no como alta.
            if (unaControladora.ExisteInsumo(nombre, tipoInsumo))
            {
                ModelState.AddModelError(string.Empty,
                    "Ese insumo ya esta registrado. Si es una reposicion, cargue la partida desde Ingreso de Stock.");
                return Page();
            }

            Insumo unInsumo = new Insumo(0, nombre, tipoInsumo, 0, stockMinimo,
                periodoDescarteDias, tipoInsumo == Insumo.PAJUELA ? unToro : null);

            if (unaControladora.RegistrarIngreso(unInsumo, cantidadInicial, DateTime.Now, fechaVencimiento))
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

            double vCantidadInicial = 0;
            double.TryParse(Request.Form["cantidadInicial"], out vCantidadInicial);
            cantidadInicial = vCantidadInicial;

            fechaVencimiento = Request.Form["fechaVencimiento"] != ""
                ? Convert.ToDateTime(Request.Form["fechaVencimiento"])
                : DateTime.MinValue;

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
