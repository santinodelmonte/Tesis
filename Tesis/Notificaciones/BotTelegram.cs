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

        // Manda un mensaje y devuelve si Telegram lo acepto.
        //
        // Nunca propaga la excepcion. Un aviso que no sale no puede voltear el sitio ni
        // cortar el proceso que lo intento: la regla de negocio de CU48 dice que la
        // falla del envio no interrumpe la operacion del sistema.
        public static async Task<bool> EnviarMensaje(string pChat, string pTexto)
        {
            if (!Configurado || pChat == "")
            {
                return false;
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

                return vRespuesta.IsSuccessStatusCode;
            }
            catch (Exception)
            {
                return false;
            }
        }

        // Los mensajes que le llegaron al bot desde el ultimo que se leyo.
        //
        // Telegram los guarda en una cola y los entrega numerados. Al pedirlos con
        // offset se le esta diciendo "de este numero en adelante", y eso confirma la
        // entrega de los anteriores: sin ese acuse los devolveria una y otra vez.
        //
        // Devuelve la lista vacia ante cualquier problema. Que se caiga la conexion a
        // internet no puede tumbar el proceso: en la vuelta siguiente vuelve a probar.
        public static async Task<List<MensajeTelegram>> ObtenerMensajes(long pDesde)
        {
            List<MensajeTelegram> _listaMensajes = new List<MensajeTelegram>();

            if (!Configurado)
            {
                return _listaMensajes;
            }

            try
            {
                string vUrl = URL_BASE + mToken + "/getUpdates?timeout=" + ESPERA_MENSAJES_SEGUNDOS
                    + "&allowed_updates=[\"message\"]"
                    + (pDesde > 0 ? "&offset=" + pDesde : "");

                string vRespuesta = await Cliente.GetStringAsync(vUrl);

                using (JsonDocument vDocumento = JsonDocument.Parse(vRespuesta))
                {
                    JsonElement vRaiz = vDocumento.RootElement;

                    if (!vRaiz.TryGetProperty("result", out JsonElement vResultado))
                    {
                        return _listaMensajes;
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
            catch (Exception)
            {
                return _listaMensajes;
            }
            return _listaMensajes;
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
