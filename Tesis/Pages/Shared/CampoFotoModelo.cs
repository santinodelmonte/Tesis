namespace Tesis.Pages.Shared
{
    // Datos que necesita el control de carga de foto, _CampoFoto.cshtml.
    //
    // Es un modelo de pantalla y no de negocio: por eso vive en Pages y no en
    // Dominio. Existe porque el mismo control lo usan el alta, la modificacion y el
    // registro de parto -este ultimo dos veces cuando el parto es doble-, y cada
    // instancia necesita su propio nombre de campo para no pisarse con las otras.
    public class CampoFotoModelo
    {
        // Nombre del campo oculto que lleva la imagen. De el salen tambien los id de
        // la vista previa y del campo que pide borrar la foto guardada.
        public string Campo { get; set; } = "foto";

        public string Etiqueta { get; set; } = "Foto:";

        // Direccion de la foto que el animal ya tiene guardada, o vacio si no tiene.
        // Solo la manda la modificacion: en un alta todavia no hay nada que mostrar.
        public string FotoActual { get; set; } = "";

        // Imagen que la usuaria ya habia elegido y que vuelve del servidor porque el
        // formulario se reenvio -calcular la categoria, confirmar una advertencia-.
        // Sin esto la foto se perdia en cada reenvio y habia que elegirla de nuevo.
        public string Valor { get; set; } = "";

        // Igual que Valor, pero para el pedido de borrar la foto guardada: si el
        // formulario se reenvia, el "quitar" que la usuaria ya habia apretado tiene
        // que seguir en pie.
        public bool Quitar { get; set; } = false;

        // Texto de ayuda debajo del control
        public string Ayuda { get; set; } = "Opcional. La imagen se achica sola antes de subirse.";

        // Contracara del control: convierte lo que llego en el campo oculto a los
        // bytes de la imagen. El navegador manda algo con esta forma
        //
        //     data:image/jpeg;base64,/9j/4AAQSkZJRgABAQ...
        //
        // o sea, un encabezado que dice de que tipo es y despues el contenido
        // codificado. Devuelve un arreglo vacio si no se puede interpretar: es dato
        // que viene del navegador, asi que puede llegar cualquier cosa.
        //
        // Que los bytes sean realmente una imagen lo verifica la persistencia, que
        // los mira antes de escribir el archivo.
        public static byte[] Decodificar(string pContenido)
        {
            if (pContenido == null || pContenido == "")
            {
                return new byte[0];
            }

            int vSeparador = pContenido.IndexOf("base64,");
            if (vSeparador < 0)
            {
                return new byte[0];
            }

            try
            {
                return Convert.FromBase64String(pContenido.Substring(vSeparador + "base64,".Length));
            }
            catch (FormatException)
            {
                return new byte[0];
            }
        }
    }
}
