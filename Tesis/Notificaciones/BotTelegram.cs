using System.Text;
using System.Text.Json;

namespace Tesis.Notificaciones
{
    // El canal hacia Telegram. Es lo unico del sistema que sale a internet.
    //
    // Habla directo contra la API de bots por HTTP y no usa ninguna biblioteca: de las
    // veinte largas operaciones que ofrece esa API el sistema necesita dos -mandar un
    // mensaje y leer los que llegaron-, y las dos son una peticion HTTP con parametros
    // en la URL.
    //
    // El token no esta escrito aca: lo carga Program.cs desde la configuracion de la
    // aplicacion, por el mismo motivo que la cadena de conexion y las credenciales de
    // acceso. Sin token la clase queda apagada y responde que no, en lugar de fallar:
    // un sistema sin Telegram configurado tiene que andar igual.
    public class BotTelegram
    {
        private const string URL_BASE = "https://api.telegram.org/bot";

        // Cuantos segundos deja abierta la peticion de mensajes nuevos esperando que
        // llegue alguno. Es "long polling": en vez de preguntar cada tanto y cortar,
        // Telegram sostiene la respuesta hasta que hay algo o hasta que se cumple el
        // plazo. Un mensaje llega al instante y el resto del tiempo no hay trafico.
        private const int ESPERA_MENSAJES_SEGUNDOS = 20;

        private static string mToken = "";

        // Un unico HttpClient para todo el proceso. Crear uno por peticion agota los
        // sockets del sistema operativo: es el error clasico de esta clase.
        private static readonly HttpClient Cliente = new HttpClient
        {
            Timeout = TimeSpan.FromSeconds(ESPERA_MENSAJES_SEGUNDOS + 15)
        };

        public static void Configurar(string pToken)
        {
            mToken = pToken ?? "";
        }

        // Si el sistema tiene bot. Sin token no hay integracion posible, y la pantalla
        // de notificaciones lo dice en lugar de dejar configurar algo que no va a
        // funcionar.
        public static bool Configurado
        {
            get { return mToken != ""; }
        }

        // Manda un mensaje y devuelve si Telegram lo acepto y, cuando no, por que.
        //
        // Nunca propaga la excepcion. Un aviso que no sale no puede voltear el sitio ni
        // cortar el proceso que lo intento: la regla de negocio de CU48 dice que la
        // falla del envio no interrumpe la operacion del sistema.
        public static async Task<RespuestaTelegram> EnviarMensaje(string pChat, string pTexto)
        {
            if (!Configurado)
            {
                return new RespuestaTelegram(false,
                    "el sistema no tiene cargado el token del bot");
            }

            if (pChat == "")
            {
                return new RespuestaTelegram(false, "no hay ningun chat vinculado");
            }

            try
            {
                // El cuerpo va como JSON y no en la URL: un resumen con veinte
                // pendientes pasa largamente el limite de una direccion web.
                string vCuerpo = JsonSerializer.Serialize(new
                {
                    chat_id = pChat,
                    text = pTexto,
                    parse_mode = "HTML"
                });

                StringContent vContenido = new StringContent(vCuerpo, Encoding.UTF8, "application/json");
                HttpResponseMessage vRespuesta = await Cliente.PostAsync(
                    URL_BASE + mToken + "/sendMessage", vContenido);

                if (vRespuesta.IsSuccessStatusCode)
                {
                    return new RespuestaTelegram(true, "");
                }

                return new RespuestaTelegram(false,
                    LeerMotivo(await vRespuesta.Content.ReadAsStringAsync()));
            }
            catch (Exception e)
            {
                return new RespuestaTelegram(false, e.Message);
            }
        }

        // Los mensajes que le llegaron al bot desde el ultimo que se leyo.
        //
        // Telegram los guarda en una cola y los entrega numerados. Al pedirlos con
        // offset se le esta diciendo "de este numero en adelante", y eso confirma la
        // entrega de los anteriores: sin ese acuse los devolveria una y otra vez.
        //
        // Ante cualquier problema devuelve una respuesta que dice que no se pudo, con
        // el motivo. No devuelve la lista vacia: "no se pudo preguntar" y "no llego
        // nada" tienen que poder distinguirse, porque la espera de veinte segundos que
        // le marca el ritmo al proceso solo existe cuando la pregunta llega a destino.
        //
        // La cancelacion se pasa a la peticion y no se consulta solo al volver: sin eso
        // apagar el sitio se queda esperando hasta veinte segundos una respuesta que ya
        // no le importa a nadie.
        public static async Task<RespuestaTelegram> ObtenerMensajes(long pDesde,
            CancellationToken pCancelacion)
        {
            List<MensajeTelegram> _listaMensajes = new List<MensajeTelegram>();

            if (!Configurado)
            {
                return new RespuestaTelegram(false,
                    "el sistema no tiene cargado el token del bot");
            }

            try
            {
                string vUrl = URL_BASE + mToken + "/getUpdates?timeout=" + ESPERA_MENSAJES_SEGUNDOS
                    + "&allowed_updates=[\"message\"]"
                    + (pDesde > 0 ? "&offset=" + pDesde : "");

                HttpResponseMessage vRespuesta = await Cliente.GetAsync(vUrl, pCancelacion);
                string vTexto = await vRespuesta.Content.ReadAsStringAsync(pCancelacion);

                if (!vRespuesta.IsSuccessStatusCode)
                {
                    return new RespuestaTelegram(false, LeerMotivo(vTexto));
                }

                using (JsonDocument vDocumento = JsonDocument.Parse(vTexto))
                {
                    JsonElement vRaiz = vDocumento.RootElement;

                    if (!vRaiz.TryGetProperty("result", out JsonElement vResultado))
                    {
                        return new RespuestaTelegram(false,
                            "Telegram contesto sin la lista de mensajes");
                    }

                    foreach (JsonElement vActualizacion in vResultado.EnumerateArray())
                    {
                        MensajeTelegram unMensaje = LeerMensaje(vActualizacion);
                        if (unMensaje != null)
                        {
                            _listaMensajes.Add(unMensaje);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                return new RespuestaTelegram(false, e.Message);
            }
            return new RespuestaTelegram(true, "", _listaMensajes);
        }

        // Telegram explica cada rechazo en el campo description de la respuesta:
        // "Unauthorized" cuando el token no sirve, "chat not found" cuando el numero de
        // chat no existe, "Conflict: terminated by other getUpdates request" cuando hay
        // dos copias del sitio preguntando por los mismos mensajes.
        //
        // Sin ese texto los tres problemas se ven exactamente igual desde el sistema, y
        // la pantalla termina culpando al identificador de chat de un token vencido.
        private static string LeerMotivo(string pRespuesta)
        {
            try
            {
                using (JsonDocument vDocumento = JsonDocument.Parse(pRespuesta))
                {
                    if (vDocumento.RootElement.TryGetProperty("description",
                        out JsonElement vMotivo))
                    {
                        return vMotivo.GetString() ?? "";
                    }
                }
            }
            catch (Exception)
            {
                // La respuesta no era JSON. Quedarse sin el texto del error no es razon
                // para voltear nada: el que llama ya sabe que la operacion no salio.
            }
            return "";
        }

        // Saca de la respuesta de Telegram lo unico que el sistema mira: el numero de
        // la actualizacion, el chat que escribio y el texto. Todo lo demas -el nombre,
        // la foto, si el mensaje reenvia a otro- no se usa.
        private static MensajeTelegram LeerMensaje(JsonElement pActualizacion)
        {
            if (!pActualizacion.TryGetProperty("update_id", out JsonElement vIdActualizacion))
            {
                return null;
            }

            if (!pActualizacion.TryGetProperty("message", out JsonElement vMensaje))
            {
                return null;
            }

            if (!vMensaje.TryGetProperty("chat", out JsonElement vChat)
                || !vChat.TryGetProperty("id", out JsonElement vIdChat))
            {
                return null;
            }

            string vTexto = "";
            if (vMensaje.TryGetProperty("text", out JsonElement vTextoJson))
            {
                vTexto = vTextoJson.GetString() ?? "";
            }

            return new MensajeTelegram(vIdActualizacion.GetInt64(),
                vIdChat.GetInt64().ToString(), vTexto);
        }
    }

    // Lo que contesto Telegram: si acepto la operacion, por que la rechazo cuando no y
    // los mensajes que trajo, si la operacion era leerlos.
    //
    // El motivo viaja con la respuesta y no en un campo compartido de BotTelegram
    // porque hay dos que le escriben al bot al mismo tiempo -el proceso del resumen y
    // la pantalla de notificaciones-, y un "ultimo error" unico le mostraria a una el
    // problema de la otra.
    public class RespuestaTelegram
    {
        private bool mSalioBien;
        private string mMotivo;
        private List<MensajeTelegram> mListaMensajes;

        // Si Telegram acepto la operacion. Al leer mensajes, false es "no se pudo
        // preguntar", que no es lo mismo que "no llego ninguno".
        public bool SalioBien { get { return mSalioBien; } }

        // Lo que contesto Telegram al rechazar, o el error de red si no llego a
        // contestar. Vacio cuando salio bien.
        public string Motivo { get { return mMotivo; } }

        // Los mensajes que llegaron. Vacia en los envios, que no leen nada.
        public List<MensajeTelegram> ListaMensajes { get { return mListaMensajes; } }

        public RespuestaTelegram(bool pSalioBien, string pMotivo,
            List<MensajeTelegram> pListaMensajes)
        {
            mSalioBien = pSalioBien;
            mMotivo = pMotivo ?? "";
            mListaMensajes = pListaMensajes ?? new List<MensajeTelegram>();
        }

        // La respuesta de una operacion que no trae mensajes: cualquier envio, y las
        // lecturas que no se pudieron hacer.
        public RespuestaTelegram(bool pSalioBien, string pMotivo)
            : this(pSalioBien, pMotivo, new List<MensajeTelegram>())
        {
        }
    }

    // Un mensaje que le llego al bot, con lo poco que el sistema necesita de el.
    public class MensajeTelegram
    {
        private long mIdActualizacion;
        private string mChat;
        private string mTexto;

        public long IdActualizacion { get { return mIdActualizacion; } }
        public string Chat { get { return mChat; } }
        public string Texto { get { return mTexto; } }

        public MensajeTelegram(long pIdActualizacion, string pChat, string pTexto)
        {
            mIdActualizacion = pIdActualizacion;
            mChat = pChat;
            mTexto = pTexto;
        }
    }
}
