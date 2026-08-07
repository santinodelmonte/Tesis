using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tesis.Dominio;

namespace Tesis.Pages.PagesReproduccion
{
    // CU15 - Registrar Servicio
    public class RegistrarServicioModel : PageModel
    {
        [BindProperty]
        public string? numCaravana { get; set; } = "";
        [BindProperty]
        public DateTime fechaServicio { get; set; } = DateTime.Now;
        [BindProperty]
        public string tipoServicio { get; set; } = Servicio.MONTA_NATURAL;
        [BindProperty]
        public int idToro { get; set; } = 0;
        [BindProperty]
        public int idPajuela { get; set; } = 0;
        [BindProperty]
        public DateTime fechaProbableParto { get; set; } = DateTime.MinValue;
        [BindProperty]
        public string? observaciones { get; set; } = "";

        public List<Animal> animales = new List<Animal>();
        public List<Macho> toros = new List<Macho>();
        public List<Insumo> pajuelas = new List<Insumo>();

        public string caravanaToro = "";

        public void OnGet(string caravana)
        {
            Controladora unaControladora = new Controladora();
            this.CargarListados(unaControladora);

            // La fecha probable de parto se propone desde el vamos, con la fecha de hoy
            fechaProbableParto = unaControladora.CalcularFechaParto(fechaServicio);

            if (caravana != null && caravana != "")
            {
                numCaravana = caravana;
            }
        }

        // Paso 7: el sistema propone la fecha probable de parto sumando la gestacion a
        // la fecha del servicio. El curso alternativo 7a permite ajustarla.
        public void OnPostCalcularFechaParto()
        {
            Controladora unaControladora = new Controladora();
            this.CargarListados(unaControladora);
            this.LeerFormulario(unaControladora);

            ModelState.Clear();

            fechaProbableParto = unaControladora.CalcularFechaParto(fechaServicio);
        }

        public IActionResult OnPostGuardar()
        {
            Controladora unaControladora = new Controladora();
            this.CargarListados(unaControladora);
            this.LeerFormulario(unaControladora);

            if (numCaravana == null || numCaravana == "")
            {
                ModelState.AddModelError(string.Empty, "Seleccione la hembra que recibe el servicio!");
                return Page();
            }

            Animal unAnimal = unaControladora.BuscarAnimalXCaravana(numCaravana);
            if (unAnimal == null || !(unAnimal is Hembra))
            {
                ModelState.AddModelError(string.Empty, "La caravana no corresponde a una hembra del rodeo!");
                return Page();
            }

            // El reproductor es uno solo: el tipo de servicio decide cual se manda y el
            // otro viaja en nulo.
            Macho unToro = null;
            Insumo unaPajuela = null;

            if (tipoServicio == Servicio.MONTA_NATURAL)
            {
                unToro = unaControladora.BuscarMacho(idToro);
            }
            else
            {
                unaPajuela = unaControladora.BuscarInsumo(idPajuela);
            }

            if (fechaProbableParto == DateTime.MinValue)
            {
                fechaProbableParto = unaControladora.CalcularFechaParto(fechaServicio);
            }

            Servicio unServicio = new Servicio(0, tipoServicio, fechaServicio, fechaProbableParto,
                observaciones ?? "", (Hembra)unAnimal, unToro, unaPajuela);

            // La Controladora concentra las reglas del caso de uso: exclusividad del
            // reproductor, stock de la pajuela y fecha no futura.
            string vMotivo = unaControladora.ValidarServicio(unServicio);
            if (vMotivo != "")
            {
                ModelState.AddModelError(string.Empty, vMotivo);
                return Page();
            }

            if (unaControladora.AltaServicio(unServicio))
            {
                return Redirect("./ListaServicios");
            }

            ModelState.AddModelError(string.Empty, "No se pudo registrar el servicio!");
            return Page();
        }

        private void CargarListados(Controladora pControladoraDominio)
        {
            animales = pControladoraDominio.ListarAnimales();
            pajuelas = pControladoraDominio.ListarPajuelas();

            // Solo los machos del rodeo en condiciones de servicio: la monta natural la
            // hace un toro que esta fisicamente en el campo.
            toros = new List<Macho>();
            foreach (Macho unMacho in pControladoraDominio.ListarMachos())
            {
                if (unMacho.Activo && unMacho.EnPie && pControladoraDominio.EsToro(unMacho))
                {
                    toros.Add(unMacho);
                }
            }
        }

        private void LeerFormulario(Controladora pControladoraDominio)
        {
            numCaravana = Request.Form["numCaravana"];
            fechaServicio = Request.Form["fechaServicio"] != "" ? Convert.ToDateTime(Request.Form["fechaServicio"]) : DateTime.Now;
            tipoServicio = Request.Form["tipoServicio"] != "" ? Request.Form["tipoServicio"] : Servicio.MONTA_NATURAL;
            observaciones = Request.Form["observaciones"];

            int vIdToro = 0;
            int.TryParse(Request.Form["idToro"], out vIdToro);
            idToro = vIdToro;

            int vIdPajuela = 0;
            int.TryParse(Request.Form["idPajuela"], out vIdPajuela);
            idPajuela = vIdPajuela;

            fechaProbableParto = Request.Form["fechaProbableParto"] != ""
                ? Convert.ToDateTime(Request.Form["fechaProbableParto"])
                : DateTime.MinValue;

            Macho unToro = pControladoraDominio.BuscarMacho(idToro);
            caravanaToro = unToro != null ? unToro.NumCaravana : "";
        }
    }
}
