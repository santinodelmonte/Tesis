using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tesis.Dominio;
using Tesis.Notificaciones;

namespace Tesis.Pages.PagesNotificaciones
{
    // CU48. Vincular el sistema con el bot de Telegram y elegir que avisos se reciben.
    //
    // La pantalla tiene dos partes porque son dos decisiones distintas: a quien se le
    // avisa, que se hace una vez al poner el sistema en marcha, y que se le avisa, que
    // se cambia cuando la encargada quiere. Guardar preferencias sin haber vinculado no
    // tiene sentido, y por eso la segunda parte aparece recien despues de la primera.
    public class NotificacionesModel : PageModel
    {
        [BindProperty]
        public string chatTelegram { get; set; } = "";

        [BindProperty]
        public string horaResumen { get; set; } = "";

        // Los tipos de aviso que quedaron tildados. Vienen del formulario como una
        // lista de identificadores: los que no estan, estan apagados.
        [BindProperty]
        public List<int> preferenciasActivas { get; set; } = new List<int>();

        public List<PreferenciaNotificacion> listaPreferencias = new List<PreferenciaNotificacion>();

        public bool botConfigurado = false;
        public bool vinculado = false;
        public string chatVinculado = "";
        public DateTime fechaUltimoResumen = DateTime.MinValue;
        public int alertasUltimoResumen = 0;

        public bool guardado = false;
        public bool recienVinculado = false;

        public void OnGet()
        {
            this.Cargar();
        }

        // Vincular es guardar el chat y escribirle. El mensaje de prueba no es un
        // adorno: es lo unico que distingue un identificador bien copiado de uno que
        // parece valido y no lo es -el paso 6 de CU48-.
        public async Task<IActionResult> OnPostVincular()
        {
            Controladora unaControladora = new Controladora();

            string vMotivo = unaControladora.ValidarChatTelegram(chatTelegram);
            if (vMotivo != "")
            {
                ModelState.AddModelError(string.Empty, vMotivo);
                this.Cargar();
                return Page();
            }

            if (!BotTelegram.Configurado)
            {
                ModelState.AddModelError(string.Empty,
                    "El sistema no tiene cargado el token del bot, asi que no puede escribirle a nadie.");
                this.Cargar();
                return Page();
            }

            // Primero se prueba y despues se guarda. Al reves quedaria guardado un
            // destinatario que no recibe nada, que es justamente lo que el curso de
            // excepcion 3a pide evitar: si la vinculacion no se completa, se conserva
            // la configuracion anterior.
            bool vLlego = await BotTelegram.EnviarMensaje(chatTelegram.Trim(),
                "<b>Sistema de Gestión de Tambo</b>\n\n"
                + "La vinculación quedó lista. Vas a recibir acá el resumen diario de "
                + "tareas pendientes.");

            if (!vLlego)
            {
                ModelState.AddModelError(string.Empty,
                    "No se pudo enviar el mensaje de prueba a ese chat. Revise que el identificador "
                    + "sea el que devolvió el bot y que le haya escrito al menos una vez: Telegram no "
                    + "deja que un bot inicie la conversación.");
                this.Cargar();
                return Page();
            }

            if (!unaControladora.VincularTelegram(chatTelegram))
            {
                ModelState.AddModelError(string.Empty, "No se pudo guardar la vinculación.");
                this.Cargar();
                return Page();
            }

            recienVinculado = true;
            this.Cargar();
            return Page();
        }

        public IActionResult OnPostGuardar()
        {
            Controladora unaControladora = new Controladora();

            if (!TimeSpan.TryParse(horaResumen, out TimeSpan vHora))
            {
                ModelState.AddModelError(string.Empty, "La hora del resumen no es una hora válida.");
                this.Cargar();
                return Page();
            }

            string vMotivo = unaControladora.ValidarHoraResumen(vHora);
            if (vMotivo != "")
            {
                ModelState.AddModelError(string.Empty, vMotivo);
                this.Cargar();
                return Page();
            }

            List<PreferenciaNotificacion> _listaPreferencias = unaControladora.ListarPreferencias();

            foreach (PreferenciaNotificacion unaPreferencia in _listaPreferencias)
            {
                unaPreferencia.Activa = preferenciasActivas.Contains(unaPreferencia.IdPreferencia);
            }

            if (!unaControladora.ModificarNotificaciones(vHora, _listaPreferencias))
            {
                ModelState.AddModelError(string.Empty, "No se pudieron guardar las preferencias.");
                this.Cargar();
                return Page();
            }

            guardado = true;
            this.Cargar();
            return Page();
        }

        // Los modulos de los que sale algun aviso, en el orden en que aparecen. La
        // pantalla agrupa los ocho tipos por modulo, y agruparlos aca en lugar de en la
        // vista deja la vista sin logica: recorre lo que se le da.
        public List<string> ListarModulos()
        {
            List<string> _listaModulos = new List<string>();

            foreach (PreferenciaNotificacion unaPreferencia in listaPreferencias)
            {
                if (!_listaModulos.Contains(unaPreferencia.Modulo))
                {
                    _listaModulos.Add(unaPreferencia.Modulo);
                }
            }
            return _listaModulos;
        }

        public List<PreferenciaNotificacion> PreferenciasXModulo(string pModulo)
        {
            List<PreferenciaNotificacion> _listaXModulo = new List<PreferenciaNotificacion>();

            foreach (PreferenciaNotificacion unaPreferencia in listaPreferencias)
            {
                if (unaPreferencia.Modulo == pModulo)
                {
                    _listaXModulo.Add(unaPreferencia);
                }
            }
            return _listaXModulo;
        }

        private void Cargar()
        {
            Controladora unaControladora = new Controladora();
            Configuracion unaConfiguracion = unaControladora.ObtenerConfiguracion();

            botConfigurado = BotTelegram.Configurado;
            vinculado = unaConfiguracion.TelegramVinculado;
            chatVinculado = unaConfiguracion.ChatTelegram;
            fechaUltimoResumen = unaConfiguracion.FechaUltimoResumen;

            if (fechaUltimoResumen != DateTime.MinValue)
            {
                alertasUltimoResumen = unaControladora.ContarAlertas(fechaUltimoResumen);
            }

            listaPreferencias = unaControladora.ListarPreferencias();

            // Lo que se muestra sale de la base, salvo cuando el formulario acaba de
            // rebotar: ahi se conserva lo que la usuaria habia escrito, para que no
            // tenga que escribirlo de nuevo.
            if (!ModelState.IsValid)
            {
                return;
            }

            chatTelegram = unaConfiguracion.ChatTelegram;
            horaResumen = unaConfiguracion.HoraResumen.ToString(@"hh\:mm");
            preferenciasActivas = new List<int>();

            foreach (PreferenciaNotificacion unaPreferencia in listaPreferencias)
            {
                if (unaPreferencia.Activa)
                {
                    preferenciasActivas.Add(unaPreferencia.IdPreferencia);
                }
            }
        }
    }
}
