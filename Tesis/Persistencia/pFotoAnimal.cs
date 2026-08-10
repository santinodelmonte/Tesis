namespace Tesis.Persistencia
{
    // Guarda las fotos de los animales como archivos dentro de wwwroot/fotos.
    //
    // La imagen no va a la base: en la columna foto de animales queda unicamente el
    // nombre del archivo. Es la misma decision que toma cualquier sistema con
    // imagenes -la base no crece, el respaldo del dump sigue siendo chico y el
    // navegador cachea la foto como cualquier otro recurso estatico-, con la
    // contrapartida de que la carpeta hay que respaldarla aparte del dump de MySQL.
    //
    // Esta clase esta en Persistencia por el mismo motivo que pConexion: es la que
    // sabe donde y como se almacena el dato. El dominio le pide guardar bytes y
    // recibe un nombre de archivo, sin enterarse de que hay un disco atras.
    public class pFotoAnimal
    {
        // Subcarpeta de wwwroot donde viven las fotos, y prefijo con el que la
        // pantalla arma la direccion. Son el mismo nombre porque la carpeta cuelga
        // directo de la raiz del sitio.
        public const string CARPETA = "fotos";

        // Tope de la imagen ya decodificada. El navegador la achica antes de subirla,
        // asi que una foto normal entra con muchisimo margen: este limite esta para
        // frenar un envio armado a mano, no para el uso comun.
        public const int TAMANIO_MAXIMO_BYTES = 3 * 1024 * 1024;

        // Ruta fisica de wwwroot. La completa Program.cs al arrancar, igual que la
        // cadena de conexion: la clase no tiene por que saber donde quedo instalado
        // el sitio.
        private static string mRutaWwwRoot = "";

        public static void Configurar(string pRutaWwwRoot)
        {
            mRutaWwwRoot = pRutaWwwRoot;
        }

        // Escribe la imagen y devuelve el nombre con el que quedo guardada. Devuelve
        // vacio si el contenido no es una imagen de un formato admitido o si excede
        // el tope: el alta no se cae por una foto, sigue sin ella.
        public string Guardar(byte[] pContenido)
        {
            if (mRutaWwwRoot == "" || pContenido == null || pContenido.Length == 0)
            {
                return "";
            }

            if (pContenido.Length > TAMANIO_MAXIMO_BYTES)
            {
                return "";
            }

            // La extension sale de los primeros bytes del archivo y no de lo que dijo
            // el navegador. Un nombre terminado en .jpg no prueba que el contenido sea
            // una imagen, y aca se esta escribiendo dentro de la carpeta que el
            // servidor publica.
            string vExtension = this.DetectarExtension(pContenido);
            if (vExtension == "")
            {
                return "";
            }

            string vCarpeta = Path.Combine(mRutaWwwRoot, CARPETA);
            Directory.CreateDirectory(vCarpeta);

            // El nombre lo genera el sistema. Si se usara la caravana, dos altas
            // seguidas del mismo animal se pisarian entre si y ademas entraria texto
            // del usuario en una ruta del disco.
            string vNombre = "animal_" + Guid.NewGuid().ToString("N") + "." + vExtension;

            File.WriteAllBytes(Path.Combine(vCarpeta, vNombre), pContenido);

            return vNombre;
        }

        // Borra el archivo de una foto que se reemplazo o se quito. Que falle no es
        // motivo para cortar la operacion: el dato de la base ya no la referencia y
        // lo unico que queda es un archivo suelto.
        public void Borrar(string pNombreArchivo)
        {
            if (mRutaWwwRoot == "" || pNombreArchivo == null || pNombreArchivo == "")
            {
                return;
            }

            // Solo se borra dentro de la carpeta de fotos y solo un nombre plano: si
            // el valor guardado tuviera separadores, apuntaria a otro lugar del disco.
            if (pNombreArchivo.Contains("/") || pNombreArchivo.Contains("\\")
                || pNombreArchivo.Contains(".."))
            {
                return;
            }

            try
            {
                string vRuta = Path.Combine(mRutaWwwRoot, CARPETA, pNombreArchivo);
                if (File.Exists(vRuta))
                {
                    File.Delete(vRuta);
                }
            }
            catch (Exception)
            {
                // Un archivo que no se pudo borrar no invalida la operacion
            }
        }

        // Reconoce el formato por la firma que todos los archivos de imagen llevan al
        // principio. Se admiten los tres que produce el navegador al recomprimir.
        private string DetectarExtension(byte[] pContenido)
        {
            if (pContenido.Length > 3
                && pContenido[0] == 0xFF && pContenido[1] == 0xD8 && pContenido[2] == 0xFF)
            {
                return "jpg";
            }

            if (pContenido.Length > 8
                && pContenido[0] == 0x89 && pContenido[1] == 0x50 && pContenido[2] == 0x4E
                && pContenido[3] == 0x47 && pContenido[4] == 0x0D && pContenido[5] == 0x0A
                && pContenido[6] == 0x1A && pContenido[7] == 0x0A)
            {
                return "png";
            }

            // WEBP: los bytes 0 a 3 dicen RIFF y los 8 a 11 dicen WEBP
            if (pContenido.Length > 12
                && pContenido[0] == 0x52 && pContenido[1] == 0x49 && pContenido[2] == 0x46
                && pContenido[3] == 0x46 && pContenido[8] == 0x57 && pContenido[9] == 0x45
                && pContenido[10] == 0x42 && pContenido[11] == 0x50)
            {
                return "webp";
            }

            return "";
        }
    }
}
