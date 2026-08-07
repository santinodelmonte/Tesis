using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tesis.Dominio;

namespace Tesis.Pages.PagesReproduccion
{
    // CU18 - Registrar Parto
    public class RegistrarPartoModel : PageModel
    {
        [BindProperty]
        public string? numCaravana { get; set; } = "";
        [BindProperty]
        public DateTime fechaParto { get; set; } = DateTime.Now;
        [BindProperty]
        public string tipoParto { get; set; } = Parto.NORMAL;
        [BindProperty]
        public string? observaciones { get; set; } = "";

        // Datos de la cria
        [BindProperty]
        public string? caravanaCria { get; set; } = "";
        [BindProperty]
        public string sexoCria { get; set; } = "H";
        [BindProperty]
        public int idRazaCria { get; set; } = 0;
        [BindProperty]
        public int idPadre { get; set; } = 0;

        // Segunda cria. Alrededor del cuatro por ciento de los partos Holando son
        // dobles, y con una sola cria por parto no habia forma de registrarlos.
        [BindProperty]
        public bool partoDoble { get; set; } = false;
        [BindProperty]
        public string? caravanaCria2 { get; set; } = "";
        [BindProperty]
        public string sexoCria2 { get; set; } = "H";
        [BindProperty]
        public int idRazaCria2 { get; set; } = 0;

        public List<Animal> animales = new List<Animal>();
        public List<Raza> razas = new List<Raza>();
        public List<Macho> machos = new List<Macho>();

        public Hembra madre = null;
        public Servicio servicioVigente = null;
        public Macho padreSugerido = null;
        public string caravanaPadre = "";

        public void OnGet(string caravana)
        {
            Controladora unaControladora = new Controladora();
            this.CargarListados(unaControladora);

            if (caravana != null && caravana != "")
            {
                numCaravana = caravana;
                this.CargarMadre(unaControladora);
                this.ProponerDatosDeLaCria(unaControladora);
            }
        }

        // Trae el servicio del que viene la preniez y propone el toro como padre de la
        // cria, que es lo que pide RF3.3 para poder reconstruir la genealogia
        public void OnPostBuscarMadre()
        {
            Controladora unaControladora = new Controladora();
            this.CargarListados(unaControladora);
            this.LeerFormulario(unaControladora);

            ModelState.Clear();

            this.CargarMadre(unaControladora);
            this.ProponerDatosDeLaCria(unaControladora);

            if (madre == null && numCaravana != null && numCaravana != "")
            {
                ModelState.AddModelError(string.Empty, "La caravana no corresponde a una hembra del rodeo!");
            }
        }

        public IActionResult OnPostConfirmarParto()
        {
            Controladora unaControladora = new Controladora();
            this.CargarListados(unaControladora);
            this.LeerFormulario(unaControladora);
            this.CargarMadre(unaControladora);

            if (madre == null)
            {
                ModelState.AddModelError(string.Empty, "Seleccione la madre!");
                return Page();
            }

            if (!madre.Activo)
            {
                ModelState.AddModelError(string.Empty, "El animal figura dado de baja: no se le puede registrar un parto.");
                return Page();
            }

            if (caravanaCria == null || caravanaCria == "")
            {
                ModelState.AddModelError(string.Empty, "El numero de caravana de la cria es obligatorio!");
                return Page();
            }

            // Curso de excepcion 5a
            if (unaControladora.ExisteCaravana(caravanaCria))
            {
                ModelState.AddModelError(string.Empty, "El numero de caravana de la cria ya existe en el sistema!");
                return Page();
            }

            if (idRazaCria == 0)
            {
                ModelState.AddModelError(string.Empty, "La raza de la cria es obligatoria!");
                return Page();
            }

            if (partoDoble)
            {
                if (caravanaCria2 == null || caravanaCria2 == "")
                {
                    ModelState.AddModelError(string.Empty, "El numero de caravana de la segunda cria es obligatorio!");
                    return Page();
                }

                if (unaControladora.ExisteCaravana(caravanaCria2))
                {
                    ModelState.AddModelError(string.Empty, "El numero de caravana de la segunda cria ya existe en el sistema!");
                    return Page();
                }

                if (caravanaCria2 == caravanaCria)
                {
                    ModelState.AddModelError(string.Empty, "Las dos crias no pueden llevar la misma caravana!");
                    return Page();
                }

                if (idRazaCria2 == 0)
                {
                    ModelState.AddModelError(string.Empty, "La raza de la segunda cria es obligatoria!");
                    return Page();
                }
            }

            if (fechaParto > DateTime.Now)
            {
                ModelState.AddModelError(string.Empty, "La fecha del parto no puede ser futura!");
                return Page();
            }

            if (fechaParto < madre.FechaNacimiento)
            {
                ModelState.AddModelError(string.Empty, "La fecha del parto no puede ser anterior al nacimiento de la madre!");
                return Page();
            }

            Macho unPadre = unaControladora.BuscarMacho(idPadre);

            // La genealogia se valida aca ademas de en la Controladora para poder
            // informar el motivo concreto, igual que en el alta de animal
            string vMotivo = unaControladora.ValidarGenealogia(0, fechaParto, madre, unPadre);
            if (vMotivo != "")
            {
                ModelState.AddModelError(string.Empty, vMotivo);
                return Page();
            }

            // La fecha de nacimiento, la madre y la categoria de las crias las completa
            // la Controladora al registrar el parto.
            List<Animal> _listaCrias = new List<Animal>();
            _listaCrias.Add(this.ArmarCria(unaControladora, caravanaCria, sexoCria, idRazaCria, unPadre));

            if (partoDoble)
            {
                _listaCrias.Add(this.ArmarCria(unaControladora, caravanaCria2, sexoCria2, idRazaCria2, unPadre));
            }

            Parto unParto = new Parto(0, fechaParto, tipoParto, observaciones ?? "", madre);

            // El parto actua sobre los dos ejes: cierra el reproductivo devolviendo la
            // hembra a vacia e inicia el productivo llevandola a lactancia. Ademas da de
            // alta las crias, cierra la lactancia anterior si habia quedado abierta y
            // abre la nueva.
            if (unaControladora.AltaParto(unParto, _listaCrias))
            {
                return Redirect("/PagesProduccion/ListaLactancias");
            }

            ModelState.AddModelError(string.Empty, "No se pudo registrar el parto!");
            return Page();
        }

        private Animal ArmarCria(Controladora pControladoraDominio, string? pCaravana, string pSexo,
            int pIdRaza, Macho pPadre)
        {
            Raza unaRaza = pControladoraDominio.BuscarRaza(pIdRaza);

            if (pSexo == "H")
            {
                return new Hembra(0, pCaravana ?? "", fechaParto, true, DateTime.MinValue, "",
                    unaRaza, null, madre, pPadre, 0, Hembra.SIN_LACTANCIA, Hembra.VACIA);
            }

            return new Macho(0, pCaravana ?? "", fechaParto, true, DateTime.MinValue, "",
                unaRaza, null, madre, pPadre, false);
        }

        private void CargarListados(Controladora pControladoraDominio)
        {
            animales = pControladoraDominio.ListarAnimales();
            razas = pControladoraDominio.ListarRazas();
            machos = pControladoraDominio.ListarMachos();
        }

        private void CargarMadre(Controladora pControladoraDominio)
        {
            if (numCaravana == null || numCaravana == "")
            {
                return;
            }

            Animal unAnimal = pControladoraDominio.BuscarAnimalXCaravana(numCaravana);
            if (unAnimal is Hembra)
            {
                madre = (Hembra)unAnimal;
                servicioVigente = pControladoraDominio.ServicioVigente(madre);
            }
        }

        private void ProponerDatosDeLaCria(Controladora pControladoraDominio)
        {
            if (madre == null)
            {
                return;
            }

            padreSugerido = pControladoraDominio.PadreSugerido(madre);

            // Solo se propone: si el usuario ya eligio otro padre, se respeta
            if (padreSugerido != null && idPadre == 0)
            {
                idPadre = padreSugerido.IdAnimal;
                caravanaPadre = padreSugerido.NumCaravana;
            }

            // La raza de la cria se propone a partir de la de la madre
            if (idRazaCria == 0 && madre.Raza != null)
            {
                idRazaCria = madre.Raza.IdRaza;
            }
            if (idRazaCria2 == 0 && madre.Raza != null)
            {
                idRazaCria2 = madre.Raza.IdRaza;
            }
        }

        private void LeerFormulario(Controladora pControladoraDominio)
        {
            numCaravana = Request.Form["numCaravana"];
            fechaParto = Request.Form["fechaParto"] != "" ? Convert.ToDateTime(Request.Form["fechaParto"]) : DateTime.Now;
            tipoParto = Request.Form["tipoParto"] != "" ? Request.Form["tipoParto"] : Parto.NORMAL;
            observaciones = Request.Form["observaciones"];

            caravanaCria = Request.Form["caravanaCria"];
            sexoCria = Request.Form["sexoCria"] != "" ? Request.Form["sexoCria"] : "H";

            int vIdRaza = 0;
            int.TryParse(Request.Form["idRazaCria"], out vIdRaza);
            idRazaCria = vIdRaza;

            // El checkbox manda "true,false" cuando esta tildado
            partoDoble = Request.Form["partoDoble"].ToString().Contains("true");
            caravanaCria2 = Request.Form["caravanaCria2"];
            sexoCria2 = Request.Form["sexoCria2"] != "" ? Request.Form["sexoCria2"] : "H";

            int vIdRaza2 = 0;
            int.TryParse(Request.Form["idRazaCria2"], out vIdRaza2);
            idRazaCria2 = vIdRaza2;

            int vIdPadre = 0;
            int.TryParse(Request.Form["idPadre"], out vIdPadre);
            idPadre = vIdPadre;

            Macho unPadre = pControladoraDominio.BuscarMacho(idPadre);
            caravanaPadre = unPadre != null ? unPadre.NumCaravana : "";
        }
    }
}
