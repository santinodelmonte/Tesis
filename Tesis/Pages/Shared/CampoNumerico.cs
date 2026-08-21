using System.Globalization;

namespace Tesis.Pages.Shared
{
    // Lectura de los campos decimales del formulario: litros, cantidades y stock.
    //
    // Es un helper de pantalla y no de negocio -por eso vive en Pages y no en Dominio-:
    // resuelve como llega el dato desde el navegador, no que significa.
    //
    // El problema que corrige. HTML define que un <input type="number"> viaja siempre
    // con punto decimal y sin separador de miles, muestre lo que muestre en pantalla
    // segun el idioma del usuario. double.TryParse, en cambio, usa la cultura del
    // proceso, y esta corre en español: ahi la coma es el separador decimal y el punto
    // el de miles. El resultado era que "82.00" se leia como ocho mil doscientos y
    // "8.20" como ochocientos veinte, y el ordeñe se rechazaba por incoherente sin que
    // se entendiera por que.
    //
    // Por eso la conversion va contra InvariantCulture, que es el formato en el que el
    // dato realmente viene. El formato de salida no se toca: las pantallas siguen
    // mostrando fechas y numeros en español.
    public static class CampoNumerico
    {
        // El valor del campo, o cero si viene vacio o no es un numero. Cero es lo que
        // devolvia el TryParse fallido, asi que las validaciones que ya existen -litros
        // positivos, cantidad mayor a cero- siguen atajando el caso igual que antes.
        public static double LeerDecimal(string pValor)
        {
            double vValor = 0;

            double.TryParse(pValor, NumberStyles.Float, CultureInfo.InvariantCulture, out vValor);

            return vValor;
        }

        // El camino de vuelta: escribir el valor en el atributo value con el mismo
        // formato en el que el navegador lo espera.
        //
        // Un <input type="number"> con value="88,1" no es un numero valido para HTML y
        // el navegador muestra el campo vacio. Se notaba al corregir un ordeñe ya
        // cargado -los litros desaparecian del formulario- y al volver de una
        // validacion fallida, que devuelve la pantalla con lo que se habia escrito.
        //
        // Esto es solo para los campos de formulario. Lo que se muestra en pantalla
        // para leer sigue saliendo en español, con ToString("N2").
        public static string Escribir(double pValor)
        {
            return pValor.ToString(CultureInfo.InvariantCulture);
        }
    }
}
