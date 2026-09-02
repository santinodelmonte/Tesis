using Tesis.Dominio;

namespace Tesis.Notificaciones
{
    // El proceso programado de CU49, y el unico actor del sistema que no es una
    // persona.
    //
    // Arranca con el sitio y se queda dando vueltas mientras el sitio este arriba. En
    // cada vuelta hace dos cosas:
    //
    //   1. Escucha los mensajes que le llegaron al bot y contesta los dos comandos que
    //      entiende -/start y /resumen-. La espera de esos mensajes es la que marca el
    //      ritmo del ciclo: veinte segundos en los que no hay trafico ni consultas a la
    //      base.
    //   2. Mira si llego la hora del resumen y, si llego y todavia no salio, lo arma y
    //      lo manda.
    //
    // Que sea un BackgroundService y no una tarea del sistema operativo tiene una
    // consecuencia que conviene tener presente: el resumen sale porque el sitio esta
    // corriendo. Con el sitio apagado no hay proceso, y por eso al arrancar revisa si
    // el resumen del dia quedo sin enviar y lo manda en ese momento, en lugar de
    // saltear el dia.
    public class ServicioNotificaciones : BackgroundService
    {
        // Cada cuanto se relee la configuracion para saber a que hora hay que mandar.
        // No se lee en cada vuelta: son tres consultas por minuto durante todo el dia
        // para un dato que cambia una vez al anio. Cambiar la hora desde la pantalla
        // tarda, en el peor caso, estos cinco minutos en tomar efecto.
        private static readonly TimeSpan RELECTURA_CONFIGURACION = TimeSpan.FromMinutes(5);

        // Si el envio falla, cuanto espera antes de volver a intentarlo. Sin esta
        // pausa, un bot mal configurado reintentaria cada veinte segundos durante horas.
        private static readonly TimeSpan ESPERA_REINTENTO = TimeSpan.FromMinutes(10);

        private readonly ILogger<ServicioNotificaciones> Registro;

        // El numero de la ultima actualizacion procesada de Telegram. Se guarda en
        // memoria y no en la base a proposito: si el sitio se reinicia, los comandos
        // que quedaron sin leer se vuelven a leer, y volver a contestar un /resumen es
        // inofensivo.
        private long mUltimoMensajeLeido = 0;

        // El dia cuyo resumen ya salio. Es una copia en memoria de lo que dice la
        // base, y esta para no preguntarselo cada veinte segundos durante el resto del
        // dia: preguntar cuesta construir una Controladora, y la respuesta no cambia.
        private DateTime mFechaResumenEnviado = DateTime.MinValue;

        private TimeSpan mHoraResumen = Controladora.HORA_RESUMEN;
        private DateTime mUltimaLecturaConfiguracion = DateTime.MinValue;
        private DateTime mProximoIntento = DateTime.MinValue;

        public ServicioNotificaciones(ILogger<ServicioNotificaciones> pRegistro)
        {
            Registro = pRegistro;
        }

        protected override async Task ExecuteAsync(CancellationToken pCancelacion)
        {
            if (!BotTelegram.Configurado)
            {
                // Sin token no hay nada que hacer, y decirlo una vez al arrancar evita
                // que alguien busque por que no llegan los mensajes.
                Registro.LogInformation(
                    "Notificaciones: no hay token de Telegram configurado, el resumen diario queda apagado.");
                return;
            }

            Registro.LogInformation("Notificaciones: el proceso del resumen diario esta en marcha.");

            while (!pCancelacion.IsCancellationRequested)
            {
                try
                {
                    await this.AtenderComandos(pCancelacion);
                    await this.RevisarResumen();
                }
                catch (Exception e)
                {
                    // El ciclo no se corta nunca por una excepcion. Un error de red, la
                    // base caida o un dato inesperado dejan una linea en el registro y
                    // se reintenta en la vuelta siguiente: el sistema tiene que seguir
                    // funcionando aunque las notificaciones no salgan.
                    Registro.LogError(e, "Notificaciones: falla en el ciclo del proceso.");
                    await Task.Delay(TimeSpan.FromMinutes(1), pCancelacion);
                }
            }
        }

        // Los dos comandos que entiende el bot. Cualquier otra cosa recibe la ayuda.
        private async Task AtenderComandos(CancellationToken pCancelacion)
        {
            List<MensajeTelegram> _listaMensajes = await BotTelegram.ObtenerMensajes(mUltimoMensajeLeido + 1);

            foreach (MensajeTelegram unMensaje in _listaMensajes)
            {
                if (pCancelacion.IsCancellationRequested)
                {
                    return;
                }

                mUltimoMensajeLeido = unMensaje.IdActualizacion;

                string vComando = unMensaje.Texto.Trim().ToLower();

                // El comando puede venir con el nombre del bot pegado -/resumen@tambobot-
                // cuando el destino es un grupo.
                if (vComando.Contains("@"))
                {
                    vComando = vComando.Substring(0, vComando.IndexOf("@"));
                }

                if (vComando.StartsWith("/start"))
                {
                    await this.ResponderStart(unMensaje);
                }
                else if (vComando.StartsWith("/resumen"))
                {
                    await this.ResponderResumen(unMensaje);
                }
                else if (!unMensaje.Chat.StartsWith("-"))
                {
                    // Solo en las conversaciones de a dos. Telegram numera los grupos
                    // con identificadores negativos, y contestarle a todo lo que se
                    // dice en un grupo convertiria al bot en una molestia.
                    await BotTelegram.EnviarMensaje(unMensaje.Chat,
                        "No entiendo ese mensaje. Escribí <b>/resumen</b> para ver las tareas "
                        + "pendientes de hoy.");
                }
            }
        }

        // /start le contesta a cualquiera, y tiene que ser asi: es el paso con el que
        // se consigue el identificador de chat para completar la vinculacion, y en ese
        // momento el sistema todavia no sabe quien es el destinatario legitimo.
        //
        // Lo unico que revela es el numero de chat de quien pregunta, que es un dato de
        // esa persona y no del establecimiento.
        private async Task ResponderStart(MensajeTelegram pMensaje)
        {
            await BotTelegram.EnviarMensaje(pMensaje.Chat,
                "<b>Sistema de Gestión de Tambo</b>\n\n"
                + "El identificador de este chat es:\n\n<b>" + pMensaje.Chat + "</b>\n\n"
                + "Copiálo en la pantalla Reportes y notificaciones &gt; Notificaciones "
                + "del sistema para empezar a recibir el resumen diario.");
        }

        // /resumen le contesta solo al chat vinculado. El resto de la informacion del
        // tambo esta detras del login, y no tendria sentido que se pudiera pedir por
        // Telegram con solo dar con el bot.
        private async Task ResponderResumen(MensajeTelegram pMensaje)
        {
            Controladora unaControladora = new Controladora();
            Configuracion unaConfiguracion = unaControladora.ObtenerConfiguracion();

            if (unaConfiguracion.ChatTelegram != pMensaje.Chat)
            {
                await BotTelegram.EnviarMensaje(pMensaje.Chat,
                    "Este chat no está vinculado al sistema.");
                return;
            }

            List<Alerta> _listaAlertas = unaControladora.GenerarAlertasDelDia();

            // El pedido a mano no registra alertas ni marca el dia como enviado: es una
            // consulta, no el resumen diario. Si lo registrara, el resumen de la maniana
            // ya no saldria.
            await BotTelegram.EnviarMensaje(pMensaje.Chat,
                unaControladora.ArmarMensajeResumen(_listaAlertas));
        }

        // El envio automatico. Sale cuando el reloj paso la hora configurada y el
        // resumen del dia todavia no salio.
        //
        // La comparacion es "paso la hora" y no "es la hora": si el sitio estuvo caido
        // a las siete y volvio a las nueve, el resumen sale a las nueve. Un aviso tarde
        // sigue sirviendo; uno que no llega, no.
        //
        // El reloj es el del servidor, igual que en todo el resto del sistema: la fecha
        // probable de parto, el vencimiento de una partida y el fin del descarte se
        // calculan con DateTime.Now. Convertir la hora solo aca sonaba prolijo y era lo
        // contrario: el resumen saldria a las siete de la maniana argentinas con las
        // tareas del dia que el servidor cree que es. Lo que hay que hacer es correr el
        // sitio con la zona horaria del establecimiento -en el hosting, la variable de
        // entorno TZ- y entonces las dos cosas coinciden. Esta escrito en bd/LEEME.md.
        private async Task RevisarResumen()
        {
            DateTime vAhora = DateTime.Now;

            if (mFechaResumenEnviado.Date == vAhora.Date || vAhora < mProximoIntento)
            {
                return;
            }

            if (!this.LeerConfiguracion(vAhora))
            {
                return;
            }

            if (vAhora.TimeOfDay < mHoraResumen)
            {
                return;
            }

            Controladora unaControladora = new Controladora();
            Configuracion unaConfiguracion = unaControladora.ObtenerConfiguracion();

            if (!unaConfiguracion.TelegramVinculado)
            {
                return;
            }

            // Ya salio: es lo que pasa cuando el sitio se reinicia despues del envio.
            // La respuesta viene de la base, que es lo unico que sobrevive al reinicio.
            if (unaControladora.ResumenEnviado(vAhora))
            {
                mFechaResumenEnviado = vAhora.Date;
                return;
            }

            List<Alerta> _listaAlertas = unaControladora.GenerarAlertasDelDia();
            string vMensaje = unaControladora.ArmarMensajeResumen(_listaAlertas);

            if (!await BotTelegram.EnviarMensaje(unaConfiguracion.ChatTelegram, vMensaje))
            {
                // El envio fallo. No se registra nada -asi el dia sigue pendiente- y se
                // espera antes de reintentar. Es el curso de excepcion 4a de CU49: el
                // error se registra y se reintenta, sin interrumpir el funcionamiento.
                Registro.LogWarning("Notificaciones: no se pudo enviar el resumen del {Fecha:dd/MM/yyyy}. "
                    + "Se reintenta mas tarde.", vAhora);
                mProximoIntento = vAhora.Add(ESPERA_REINTENTO);
                return;
            }

            unaControladora.RegistrarEnvioResumen(_listaAlertas, vAhora);
            mFechaResumenEnviado = vAhora.Date;
            mProximoIntento = DateTime.MinValue;

            Registro.LogInformation("Notificaciones: resumen del {Fecha:dd/MM/yyyy} enviado con "
                + "{Cantidad} pendientes.", vAhora, _listaAlertas.Count);
        }

        // Relee la hora configurada cada tanto. Devuelve si pudo, para que un problema
        // con la base no derive en un envio a una hora inventada.
        private bool LeerConfiguracion(DateTime pAhora)
        {
            if (pAhora - mUltimaLecturaConfiguracion < RELECTURA_CONFIGURACION)
            {
                return true;
            }

            try
            {
                mHoraResumen = new Controladora().ObtenerConfiguracion().HoraResumen;
                mUltimaLecturaConfiguracion = pAhora;
                return true;
            }
            catch (Exception e)
            {
                Registro.LogError(e, "Notificaciones: no se pudo leer la configuracion.");
                return false;
            }
        }
    }
}
