using ClosedXML.Excel;
using System.Globalization;
using Tesis.Dominio;

namespace Tesis.Reportes
{
    // Vuelca un Reporte a una planilla de Excel (RF7.1 a RF7.4).
    //
    // Una hoja por seccion, y no todo apilado en una sola. Es la diferencia entre una
    // planilla que se puede ordenar y filtrar y una que solo se puede mirar: con las
    // secciones apiladas, el autofiltro de Excel toma la primera tabla y las demas
    // quedan como texto suelto en el medio.
    //
    // Los numeros se escriben como numeros. Si fueran texto la planilla no sumaria una
    // columna de litros, que es lo primero que alguien va a intentar hacer con ella.
    public static class GeneradorExcel
    {
        public static byte[] Generar(Reporte pReporte)
        {
            using (XLWorkbook unLibro = new XLWorkbook())
            {
                int vNumero = 1;

                foreach (SeccionReporte unaSeccion in pReporte.Secciones)
                {
                    IXLWorksheet unaHoja = unLibro.Worksheets.Add(
                        NombreDeHoja(unaSeccion.Titulo, vNumero, unLibro));

                    EscribirSeccion(unaHoja, pReporte, unaSeccion);
                    vNumero = vNumero + 1;
                }

                // Un reporte sin secciones no deberia llegar aca, pero un libro sin
                // hojas no es un archivo valido y Excel lo rechaza al abrirlo.
                if (unLibro.Worksheets.Count == 0)
                {
                    IXLWorksheet unaHoja = unLibro.Worksheets.Add("Reporte");
                    unaHoja.Cell(1, 1).Value = pReporte.Titulo;
                    unaHoja.Cell(2, 1).Value = "Sin datos para el período consultado.";
                }

                using (MemoryStream unaMemoria = new MemoryStream())
                {
                    unLibro.SaveAs(unaMemoria);
                    return unaMemoria.ToArray();
                }
            }
        }

        private static void EscribirSeccion(IXLWorksheet pHoja, Reporte pReporte, SeccionReporte pSeccion)
        {
            // Encabezado: de que reporte salio esta hoja. Una planilla se manda por
            // correo suelta, y sin esto no se sabe de que periodo es.
            pHoja.Cell(1, 1).Value = pReporte.Titulo;
            pHoja.Cell(1, 1).Style.Font.Bold = true;
            pHoja.Cell(1, 1).Style.Font.FontSize = 14;

            pHoja.Cell(2, 1).Value = pReporte.Periodo;
            pHoja.Cell(3, 1).Value = "Emitido el " + pReporte.FechaEmision.ToString("dd/MM/yyyy HH:mm");
            pHoja.Cell(4, 1).Value = pSeccion.Titulo;
            pHoja.Cell(4, 1).Style.Font.Bold = true;

            int vFilaCabecera = 6;

            for (int vColumna = 0; vColumna < pSeccion.Columnas.Length; vColumna = vColumna + 1)
            {
                IXLCell unaCelda = pHoja.Cell(vFilaCabecera, vColumna + 1);
                unaCelda.Value = pSeccion.Columnas[vColumna];
                unaCelda.Style.Font.Bold = true;
                unaCelda.Style.Fill.BackgroundColor = XLColor.LightGray;
                unaCelda.Style.Border.BottomBorder = XLBorderStyleValues.Thin;
            }

            int vFila = vFilaCabecera + 1;

            foreach (string[] unaFila in pSeccion.Filas)
            {
                for (int vColumna = 0; vColumna < unaFila.Length; vColumna = vColumna + 1)
                {
                    Escribir(pHoja.Cell(vFila, vColumna + 1), unaFila[vColumna],
                        pSeccion.EsNumerica(vColumna));
                }
                vFila = vFila + 1;
            }

            if (pSeccion.Filas.Count == 0)
            {
                pHoja.Cell(vFila, 1).Value = "Sin registros en el período.";
                pHoja.Cell(vFila, 1).Style.Font.Italic = true;
            }
            else
            {
                pHoja.Range(vFilaCabecera, 1, vFila - 1, pSeccion.Columnas.Length).SetAutoFilter();
                pHoja.SheetView.FreezeRows(vFilaCabecera);
            }

            if (pSeccion.Nota != "")
            {
                pHoja.Cell(vFila + 1, 1).Value = pSeccion.Nota;
                pHoja.Cell(vFila + 1, 1).Style.Font.Italic = true;
            }

            pHoja.Columns().AdjustToContents();
        }

        // El valor viaja como texto ya formateado, asi que para escribirlo como numero
        // hay que deshacer ese formato. Se lee con la cultura del sistema, que es la
        // misma con la que la Controladora lo escribio.
        private static void Escribir(IXLCell pCelda, string pValor, bool pEsNumerica)
        {
            if (!pEsNumerica || pValor == null || pValor == "")
            {
                pCelda.Value = pValor ?? "";
                return;
            }

            double vNumero = 0;
            if (double.TryParse(pValor, NumberStyles.Any, CultureInfo.CurrentCulture, out vNumero))
            {
                pCelda.Value = vNumero;
                return;
            }

            // No era un numero después de todo: entra como texto y no se pierde.
            pCelda.Value = pValor;
        }

        // Excel no admite hojas con mas de treinta y un caracteres, ni con los signos
        // que usa para referenciar rangos, ni dos con el mismo nombre.
        private static string NombreDeHoja(string pTitulo, int pNumero, XLWorkbook pLibro)
        {
            string vNombre = pTitulo ?? "";

            foreach (char unSigno in new char[] { ':', '\\', '/', '?', '*', '[', ']' })
            {
                vNombre = vNombre.Replace(unSigno, ' ');
            }

            vNombre = vNombre.Trim();

            if (vNombre == "")
            {
                vNombre = "Sección " + pNumero;
            }

            if (vNombre.Length > 31)
            {
                vNombre = vNombre.Substring(0, 31).Trim();
            }

            // Si dos secciones se llaman parecido y quedaron iguales al recortarlas, se
            // desempatan con el numero de seccion.
            if (pLibro.Worksheets.Contains(vNombre))
            {
                string vSufijo = " (" + pNumero + ")";
                int vLargo = 31 - vSufijo.Length;

                if (vNombre.Length > vLargo)
                {
                    vNombre = vNombre.Substring(0, vLargo).Trim();
                }
                vNombre = vNombre + vSufijo;
            }
            return vNombre;
        }
    }
}
