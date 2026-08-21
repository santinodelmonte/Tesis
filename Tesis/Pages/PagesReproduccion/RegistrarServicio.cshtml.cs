using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tesis.Dominio;

namespace Tesis.Pages.PagesReproduccion
{
    // CU21 - Registrar Servicio.
    //
    // La misma pantalla da de alta y corrige, como la de celo. Con id en cero es un
    // alta; con un identificador se abre con el servicio cargado y guarda encima.
    public class RegistrarServicioModel : PageModel
    {
        [BindProperty]
        public int id { get; set; } = 0;
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

        // Lo tilda el boton "Registrar de todos modos": llega en verdadero solo cuando
        // el usuario ya vio las advertencias y decidio guardar igual.
        [BindProperty]
        public bool confirmado { get; set; } = false;

        public List<Animal> animales = new List<Animal>();
        public List<Macho> toros = new List<Macho>();
        public List<Insumo> pajuelas = new List<Insumo>();

        // Motivos que no impiden el registro pero que el usuario tiene que ver antes
        public List<string> advertencias = new List<string>();

        public string caravanaToro = "";

        public bool esCorreccion { get { return id > 0; } }

        // Cuando se corrige un servicio que ya tiene tactos, el animal queda fijo: el
        // control de gestacion se hizo sobre esa vaca.
        public bool animalBloqueado = false;

        public IActionResult OnGet(int id, string caravana)
        {
            Controladora unaControladora = new Controladora();
            this.CargarListados(unaControladora);

            if (id > 0)
            {
                return this.CargarServicio(unaControladora, id);
            }

            // La fecha probable de parto se propone desde el vamos, con la fecha de hoy
            fechaProbableParto = unaControladora.CalcularFechaParto(fechaServicio);

            if (caravana != null && caravana != "")
            {
                numCaravana = caravana;
            }
            return Page();
        }

        private IActionResult CargarServicio(Controladora pControladoraDominio, int pIdServicio)
        {
            Servicio unServicio = pControladoraDominio.BuscarServicio(pIdServicio);
            if (unServicio == null)
            {
                return Redirect("./ListaServicios");
            }

            id = pIdServicio;
            numCaravana = unServicio.Animal != null ? unServicio.Animal.NumCaravana : "";
            fechaServicio = unServicio.FechaServicio;
            fechaProbableParto = unServicio.FechaProbableParto;
            tipoServicio = unServicio.TipoServicio;
            observaciones = unServicio.Observaciones;
            idToro = unServicio.Toro != null ? unServicio.Toro.IdAnimal : 0;
            idPajuela = unServicio.Pajuela != null ? unServicio.Pajuela.IdInsumo : 0;
            caravanaToro = unServicio.Toro != null ? unServicio.Toro.NumCaravana : "";

            animalBloqueado = pControladoraDominio.FiltrarTactosXServicio(pIdServicio).Count > 0;

            // El toro que hizo el servicio puede haber salido del rodeo desde entonces.
            // Si no esta en la lista de candidatos, se agrega: corregirle la fecha al
            // servicio no tiene por que cambiarle el reproductor.
            if (unServicio.Toro != null && !this.EstaEnLaLista(unServicio.Toro))
            {
                toros.Add(unServicio.Toro);
            }
            return Page();
        }

        private bool EstaEnLaLista(Macho pToro)
        {
            foreach (Macho unMacho in toros)
            {
                if (unMacho.IdAnimal == pToro.IdAnimal)
                {
                    return true;
                }
            }
            return false;
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

            if (id > 0)
            {
                return this.Corregir(unaControladora, (Hembra)unAnimal, unToro, unaPajuela);
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

            // Las advertencias no bloquean: el toro dado de baja o el parentesco con el
            // reproductor se muestran y el servicio se guarda si el usuario insiste.
            advertencias = unaControladora.AdvertenciasServicio(unServicio);
            if (advertencias.Count > 0 && !confirmado)
            {
                return Page();
            }

            if (unaControladora.AltaServicio(unServicio))
            {
                return Redirect("./ListaServicios");
            }

            ModelState.AddModelError(string.Empty, "No se pudo registrar el servicio!");
            return Page();
        }

        // La correccion pasa por la Controladora, que ademas de revalidar arrastra los
        // tres efectos que el alta habia dejado: el estado de la hembra, la fecha
        // probable de parto de la lactancia en curso y, si cambio la pajuela, el stock.
        private IActionResult Corregir(Controladora pControladoraDominio, Hembra pHembra,
            Macho pToro, Insumo pPajuela)
        {
            int vIdToro = pToro != null ? pToro.IdAnimal : 0;
            int vIdPajuela = pPajuela != null ? pPajuela.IdInsumo : 0;

            string vMotivo = pControladoraDominio.ValidarModificarServicio(id, tipoServicio,
                fechaServicio, fechaProbableParto, pHembra.IdAnimal, vIdToro, vIdPajuela);

            if (vMotivo != "")
            {
                ModelState.AddModelError(string.Empty, vMotivo);
                return Page();
            }

            // Las advertencias tambien se revisan al corregir, y por el mismo motivo que
            // en el alta: cambiar la pajuela por una de un toro emparentado con la vaca
            // es un dato posible pero que hay que ver antes de confirmar. Sin esto, la
            // correccion era la puerta por la que un servicio consanguineo entraba sin
            // que nadie lo mirara.
            Servicio unServicioCorregido = new Servicio(id, tipoServicio, fechaServicio,
                fechaProbableParto, observaciones ?? "", pHembra, pToro, pPajuela);

            advertencias = pControladoraDominio.AdvertenciasServicio(unServicioCorregido);
            if (advertencias.Count > 0 && !confirmado)
            {
                return Page();
            }

            if (pControladoraDominio.ModificarServicio(id, tipoServicio, fechaServicio,
                fechaProbableParto, observaciones ?? "", pHembra.IdAnimal, vIdToro, vIdPajuela))
            {
                return Redirect("./ListaServicios");
            }

            ModelState.AddModelError(string.Empty, "No se pudo corregir el servicio!");
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
            int vId = 0;
            int.TryParse(Request.Form["id"], out vId);
            id = vId;

            if (id > 0)
            {
                animalBloqueado = pControladoraDominio.FiltrarTactosXServicio(id).Count > 0;
            }

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

            // Solo viaja cuando se apreto el boton de confirmacion, no en un guardado
            // normal: si el usuario cambia el reproductor, las advertencias se vuelven
            // a evaluar desde cero.
            confirmado = Request.Form["confirmado"].ToString().Contains("true");

            Macho unToro = pControladoraDominio.BuscarMacho(idToro);
            caravanaToro = unToro != null ? unToro.NumCaravana : "";
        }
    }
}
