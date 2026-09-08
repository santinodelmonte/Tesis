using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Globalization;
using Tesis.Dominio;

namespace Tesis.Pages
{
    // Tablero de inicio: lo que hay que hacer hoy en el tambo.
    //
    // Junta las listas de trabajo y las alertas que ya viven en la Controladora y que
    // hasta ahora estaban repartidas en seis menus distintos, de manera que la
    // encargada abra el sistema a la maniana y vea de una todo lo que requiere
    // atencion. Es, ademas, el mismo contenido del resumen diario del bot.
    //
    // Arriba de las alertas esta el registro rapido. El relevamiento con la encargada
    // del sector dio que lo que se carga todos los dias son los eventos reproductivos
    // -celo, servicio, tacto y parto-, y que llegar a cada uno costaba cuatro clics de
    // menu que se repetian una vez por animal. El registro rapido resuelve dos de esos
    // eventos en el propio tablero y para los otros dos abre el formulario completo con
    // la caravana ya puesta: el corte no es por dificultad de programarlo sino por lo
    // que arrastra cada evento, y esta explicado en Guardar y en AbrirFormulario.
    public class IndexModel : PageModel
    {
        // Los cuatro eventos que ofrece el registro rapido. Son los valores del grupo
        // de opciones del formulario y los que devuelve la direccion despues de guardar.
        public const string EVENTO_CELO = "celo";
        public const string EVENTO_SERVICIO = "servicio";
        public const string EVENTO_TACTO = "tacto";
        public const string EVENTO_PARTO = "parto";

        // Cuantas caravanas sugeridas se ofrecen por evento. Pasadas ocho la fila deja
        // de leerse de un vistazo y conviene mandar a la lista de trabajo completa.
        public const int MAXIMO_SUGERIDAS = 8;

        // Trabajo reproductivo
        public int vacasParaServir = 0;
        public int tactosPendientes = 0;
        public int partosProximos = 0;

        // Trabajo productivo
        public int secadosProximos = 0;
        public int animalesEnDescarte = 0;

        // Trabajo sanitario y de stock
        public int procedimientosPendientes = 0;
        public int insumosCriticos = 0;
        public int partidasPorVencer = 0;

        // Composicion del rodeo
        public int enLactancia = 0;
        public int secas = 0;
        public int prenadas = 0;
        public int vacias = 0;
        public int totalActivos = 0;

        public double promedioDiarioRodeo = 0;
        public double promedioDiasEnLeche = 0;

        // Cuando no hay ningun animal cargado, el tablero explica por donde empezar en
        // lugar de mostrar ocho ceros.
        public bool sinDatos = false;

        // --- Registro rapido ---

        // Evento elegido. Viaja como un campo mas del formulario y no como estado del
        // servidor, asi que un guardado con error vuelve a la misma solapa.
        [BindProperty]
        public string evento { get; set; } = EVENTO_CELO;
        [BindProperty]
        public string? numCaravana { get; set; } = "";
        [BindProperty]
        public DateTime fecha { get; set; } = DateTime.Now;
        [BindProperty]
        public string resultado { get; set; } = Tacto.PRENADA;
        [BindProperty]
        public string? observaciones { get; set; } = "";

        // El padron completo, que es lo que necesita el selector de animal por caravana
        public List<Animal> animales = new List<Animal>();

        // Las hembras del rodeo, para la lista de sugerencias del campo caravana
        public List<Hembra> hembras = new List<Hembra>();

        // Las caravanas que el sistema ya sabe que corresponden hoy, separadas por
        // evento. Tocar una llena el campo, que es lo que evita tipear en el caso mas
        // frecuente: cargarle el evento a un animal que ya figura en una lista de
        // trabajo. Salen de las mismas listas que cuentan las alertas de mas abajo.
        public List<string> sugeridasParaServir = new List<string>();
        public List<string> sugeridasParaTactar = new List<string>();
        public List<string> sugeridasParaParir = new List<string>();

        // Lo ultimo que se guardo, para confirmarlo sin obligar a ir a buscarlo a la
        // lista. Llega por la direccion, no por memoria del servidor.
        public string eventoGuardado = "";
        public string caravanaGuardada = "";
        public bool hayConfirmacion { get { return eventoGuardado != ""; } }

        // El formato con el que la fecha del registro rapido va y vuelve en la direccion.
        // Es el mismo que usa el campo de fecha del navegador, y se lee con la cultura
        // invariante: es un dato que el sistema se manda a si mismo, no algo tipeado.
        public const string FORMATO_DIA = "yyyy-MM-dd";

        public void OnGet(string guardado, string caravana, string dia)
        {
            this.CargarTablero();

            // Despues de guardar se vuelve al inicio con ?guardado=celo&caravana=245.
            // Un dato que dura una sola pantalla no justifica sesion, y ademas asi la
            // confirmacion sobrevive a que la encargada recargue la pagina.
            if (guardado == EVENTO_CELO || guardado == EVENTO_TACTO)
            {
                eventoGuardado = guardado;
                caravanaGuardada = caravana ?? "";
                evento = guardado;
            }

            // La jornada que se estaba cargando vuelve con la direccion. Los registros
            // se cargan de a tandas de una misma fecha -los celos de ayer, los tactos de
            // la visita del veterinario-, y volver a poner la fecha en cada uno seria
            // devolver con una mano lo que la tarjeta ahorra con la otra.
            DateTime vDia;
            if (DateTime.TryParseExact(dia, FORMATO_DIA, CultureInfo.InvariantCulture,
                DateTimeStyles.None, out vDia))
            {
                fecha = vDia;
            }
        }

        // Un solo controlador para los cuatro eventos, porque el evento es un campo del
        // formulario y no un boton distinto. Es lo que deja que la pantalla funcione
        // igual sin JavaScript: sin el, en lugar de una solapa por vez se ven los
        // cuatro grupos de campos y el mismo boton, y el registro entra lo mismo.
        public IActionResult OnPost()
        {
            this.LeerFormulario();

            if (evento == EVENTO_SERVICIO)
            {
                return this.AbrirFormulario("/PagesReproduccion/RegistrarServicio");
            }
            if (evento == EVENTO_PARTO)
            {
                return this.AbrirFormulario("/PagesReproduccion/RegistrarParto");
            }
            return this.Guardar();
        }

        // Celo y tacto se guardan en el tablero.
        //
        // El celo no arrastra nada: no mueve el estado de la vaca, no toca la lactancia
        // y no consume insumos, asi que lo peor que puede pasar con uno mal cargado es
        // corregirlo. El tacto si mueve el estado reproductivo, pero es una eleccion
        // entre tres resultados sobre un servicio que el sistema ya conoce: no hay nada
        // que decidir que no este en la pantalla. Los dos entran en tres campos, y esos
        // tres campos son los que se repiten quince veces por maniana.
        private IActionResult Guardar()
        {
            Controladora unaControladora = new Controladora();

            Hembra unaHembra = this.BuscarHembra(unaControladora);
            if (unaHembra == null)
            {
                return this.Recargar();
            }

            if (evento == EVENTO_TACTO)
            {
                return this.GuardarTacto(unaControladora, unaHembra);
            }
            return this.GuardarCelo(unaControladora, unaHembra);
        }

        private IActionResult GuardarCelo(Controladora pControladoraDominio, Hembra pHembra)
        {
            Celo unCelo = new Celo(0, fecha, observaciones ?? "", pHembra);

            // Las reglas -fecha no futura, edad minima a la que la hembra empieza a
            // ciclar, animal en el rodeo a esa fecha- son las de la Controladora, las
            // mismas que aplica Registrar celo. El tablero no valida por su cuenta.
            string vMotivo = pControladoraDominio.ValidarCelo(unCelo);
            if (vMotivo != "")
            {
                ModelState.AddModelError(string.Empty, vMotivo);
                return this.Recargar();
            }

            if (!pControladoraDominio.AltaCelo(unCelo))
            {
                ModelState.AddModelError(string.Empty, "No se pudo registrar el celo!");
                return this.Recargar();
            }

            return RedirectToPage("/Index", new
            {
                guardado = EVENTO_CELO,
                caravana = pHembra.NumCaravana,
                dia = fecha.ToString(FORMATO_DIA, CultureInfo.InvariantCulture)
            });
        }

        private IActionResult GuardarTacto(Controladora pControladoraDominio, Hembra pHembra)
        {
            // El tacto se asienta sobre el servicio vigente de la hembra, igual que en
            // CU22: sin servicio pendiente no hay preniez para confirmar.
            Servicio unServicio = pControladoraDominio.ServicioVigente(pHembra);
            if (unServicio == null)
            {
                ModelState.AddModelError(string.Empty, "La caravana " + pHembra.NumCaravana
                    + " no tiene un servicio pendiente: hay que registrar el servicio antes del tacto!");
                return this.Recargar();
            }

            Tacto unTacto = new Tacto(0, fecha, resultado, observaciones ?? "", unServicio);

            string vMotivo = pControladoraDominio.ValidarTacto(unTacto);
            if (vMotivo != "")
            {
                ModelState.AddModelError(string.Empty, vMotivo);
                return this.Recargar();
            }

            if (!pControladoraDominio.AltaTacto(unTacto))
            {
                ModelState.AddModelError(string.Empty, "No se pudo registrar el tacto!");
                return this.Recargar();
            }

            return RedirectToPage("/Index", new
            {
                guardado = EVENTO_TACTO,
                caravana = pHembra.NumCaravana,
                dia = fecha.ToString(FORMATO_DIA, CultureInfo.InvariantCulture)
            });
        }

        // Servicio y parto no se guardan en el tablero, y es una decision tomada y no
        // algo que falto hacer: el servicio descuenta una pajuela del stock y elige toro
        // o pajuela, y el parto da de alta una cria -o dos- con su sexo, su raza y su
        // padre, y le abre la lactancia a la madre. Son altas que crean o consumen otros
        // registros. Meterlas en tres campos del inicio no las haria mas rapidas: las
        // haria mas faciles de equivocar, y el error de un parto mal cargado se arrastra
        // a la produccion y a la genealogia. Lo que el tablero ahorra es el camino, que
        // era el problema real: abre el formulario completo con la caravana ya puesta.
        private IActionResult AbrirFormulario(string pPagina)
        {
            return RedirectToPage(pPagina, new { caravana = numCaravana ?? "" });
        }

        // La caravana escrita a mano o elegida en el selector, resuelta al animal que
        // nombra. Devuelve null y deja escrito el motivo cuando no llega a una hembra.
        private Hembra BuscarHembra(Controladora pControladoraDominio)
        {
            if (numCaravana == null || numCaravana == "")
            {
                ModelState.AddModelError(string.Empty, "Indique la caravana del animal!");
                return null;
            }

            Animal unAnimal = pControladoraDominio.BuscarAnimalXCaravana(numCaravana);
            if (unAnimal == null)
            {
                ModelState.AddModelError(string.Empty,
                    "La caravana " + numCaravana + " no existe en el sistema!");
                return null;
            }

            if (!(unAnimal is Hembra))
            {
                ModelState.AddModelError(string.Empty, "La caravana " + numCaravana
                    + " corresponde a un macho: los eventos reproductivos se registran sobre la hembra!");
                return null;
            }

            return (Hembra)unAnimal;
        }

        // Un guardado que no entro vuelve a mostrar el tablero con el motivo arriba del
        // formulario. Los contadores hay que volver a pedirlos porque la peticion es
        // otra, y ademas asi el tablero refleja lo que si se guardo hasta recien.
        private IActionResult Recargar()
        {
            this.CargarTablero();
            return Page();
        }

        private void CargarTablero()
        {
            Controladora unaControladora = new Controladora();
            Configuracion unaConfiguracion = unaControladora.ObtenerConfiguracion();

            animales = unaControladora.ListarAnimales();
            hembras = unaControladora.ListarHembras();
            // El rodeo son los animales del establecimiento: los reproductores de
            // catalogo no se cuentan, porque no estan en el campo.
            totalActivos = unaControladora.ListarRodeo().Count;
            sinDatos = animales.Count == 0;

            // Las tres listas de trabajo reproductivo se piden enteras y no contadas:
            // de cada una sale el numero que va a la tarjeta de alertas y las primeras
            // caravanas que ofrece el registro rapido. Se piden una sola vez para las
            // dos cosas, que es lo que evita recorrer el rodeo dos veces por pantalla.
            List<Hembra> _listaParaServir = unaControladora.ListarVacasParaServir();
            List<Servicio> _listaParaTactar = unaControladora.ListarTactosPendientes();
            List<Servicio> _listaParaParir = unaControladora.ListarAlertasParto();

            vacasParaServir = _listaParaServir.Count;
            tactosPendientes = _listaParaTactar.Count;
            partosProximos = _listaParaParir.Count;

            sugeridasParaServir = this.CaravanasDeHembras(_listaParaServir);
            sugeridasParaTactar = this.CaravanasDeServicios(_listaParaTactar);
            sugeridasParaParir = this.CaravanasDeServicios(_listaParaParir);

            secadosProximos = unaControladora.ListarAlertasSecado().Count;
            animalesEnDescarte = unaControladora.ListarHembrasEnDescarte().Count;

            procedimientosPendientes = unaControladora.ObtenerCalendarioSanitario(
                unaConfiguracion.DiasAnticipacionSanitaria).Count;
            insumosCriticos = unaControladora.ListarAlertasStock().Count;
            partidasPorVencer = unaControladora.ListarAlertasVencimiento(
                unaConfiguracion.DiasAnticipacionVencimiento).Count;

            enLactancia = unaControladora.ContarHembrasXEstadoProductivo(Hembra.EN_LACTANCIA);
            secas = unaControladora.ContarHembrasXEstadoProductivo(Hembra.SECA);
            prenadas = unaControladora.ContarHembrasXEstadoReproductivo(Hembra.PRENADA);
            vacias = unaControladora.ContarHembrasXEstadoReproductivo(Hembra.VACIA);

            promedioDiarioRodeo = unaControladora.PromedioDiarioRodeo();
            promedioDiasEnLeche = unaControladora.PromedioDiasEnLeche();
        }

        private List<string> CaravanasDeHembras(List<Hembra> pLista)
        {
            List<string> _listaCaravanas = new List<string>();

            foreach (Hembra unaHembra in pLista)
            {
                if (_listaCaravanas.Count == MAXIMO_SUGERIDAS)
                {
                    break;
                }
                _listaCaravanas.Add(unaHembra.NumCaravana);
            }
            return _listaCaravanas;
        }

        private List<string> CaravanasDeServicios(List<Servicio> pLista)
        {
            List<string> _listaCaravanas = new List<string>();

            foreach (Servicio unServicio in pLista)
            {
                if (_listaCaravanas.Count == MAXIMO_SUGERIDAS)
                {
                    break;
                }
                if (unServicio.Animal != null)
                {
                    _listaCaravanas.Add(unServicio.Animal.NumCaravana);
                }
            }
            return _listaCaravanas;
        }

        // Los campos se leen del formulario y no del enlace automatico por el mismo
        // motivo que en el resto de las pantallas: la fecha llega del navegador como
        // aaaa-mm-dd y el enlace la interpreta con la cultura del sitio.
        private void LeerFormulario()
        {
            string? vEvento = Request.Form["evento"];
            evento = string.IsNullOrEmpty(vEvento) ? EVENTO_CELO : vEvento;

            numCaravana = Request.Form["numCaravana"];

            string? vFecha = Request.Form["fecha"];
            fecha = string.IsNullOrEmpty(vFecha) ? DateTime.Now : Convert.ToDateTime(vFecha);

            string? vResultado = Request.Form["resultado"];
            resultado = string.IsNullOrEmpty(vResultado) ? Tacto.PRENADA : vResultado;

            observaciones = Request.Form["observaciones"];
        }
    }
}
