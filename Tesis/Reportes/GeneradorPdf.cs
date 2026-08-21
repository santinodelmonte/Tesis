using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Tesis.Dominio;

namespace Tesis.Reportes
{
    // Vuelca un Reporte a PDF (RF7.1 a RF7.4).
    //
    // No decide contenido: recibe el reporte ya armado por la Controladora y sólo se
    // ocupa de como se ve. Es el mismo criterio que separa Dominio de Pages, aplicado a
    // un tercer destino: la pantalla, el PDF y la planilla muestran lo mismo porque los
    // tres reciben el mismo objeto ya resuelto.
    //
    // Las paginas van apaisadas porque las tablas del tambo son anchas -una fila de
    // produccion por animal lleva siete columnas- y en vertical entrarian partidas.
    public static class GeneradorPdf
    {
        // QuestPDF pide declarar la licencia antes de generar. La Community es la que
        // corresponde a un proyecto sin fines comerciales.
        static GeneradorPdf()
        {
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public static byte[] Generar(Reporte pReporte)
        {
            return Document.Create(documento =>
            {
                documento.Page(pagina =>
                {
                    pagina.Size(PageSizes.A4.Landscape());
                    pagina.Margin(1.5f, Unit.Centimetre);
                    pagina.DefaultTextStyle(estilo => estilo.FontSize(9).FontColor(Colors.Grey.Darken4));

                    pagina.Header().Element(cabecera => Cabecera(cabecera, pReporte));
                    pagina.Content().Element(cuerpo => Cuerpo(cuerpo, pReporte));

                    // El pie con la numeracion no es decorativo: un reporte impreso que
                    // se desarma sobre el escritorio hay que poder volver a ordenarlo.
                    pagina.Footer().AlignCenter().Text(texto =>
                    {
                        texto.DefaultTextStyle(estilo => estilo.FontSize(8).FontColor(Colors.Grey.Darken1));
                        texto.Span("Página ");
                        texto.CurrentPageNumber();
                        texto.Span(" de ");
                        texto.TotalPages();
                    });
                });
            }).GeneratePdf();
        }

        private static void Cabecera(IContainer pContenedor, Reporte pReporte)
        {
            pContenedor.PaddingBottom(10).Column(columna =>
            {
                columna.Item().Text("Gestión de Tambo")
                    .FontSize(8).FontColor(Colors.Grey.Darken1);

                columna.Item().Text(pReporte.Titulo)
                    .FontSize(16).SemiBold();

                columna.Item().Text(pReporte.Periodo)
                    .FontSize(10).FontColor(Colors.Grey.Darken2);

                columna.Item().Text("Emitido el " + pReporte.FechaEmision.ToString("dd/MM/yyyy") +
                        " a las " + pReporte.FechaEmision.ToString("HH:mm"))
                    .FontSize(8).FontColor(Colors.Grey.Darken1);

                columna.Item().PaddingTop(6).LineHorizontal(1).LineColor(Colors.Grey.Medium);
            });
        }

        private static void Cuerpo(IContainer pContenedor, Reporte pReporte)
        {
            pContenedor.Column(columna =>
            {
                foreach (SeccionReporte unaSeccion in pReporte.Secciones)
                {
                    columna.Item().PaddingTop(12).Text(unaSeccion.Titulo).FontSize(11).SemiBold();

                    if (unaSeccion.Filas.Count == 0)
                    {
                        // Una seccion vacia se informa y no se omite: que no haya
                        // tratamientos en el periodo es un dato, y borrarla del reporte
                        // dejaria la duda de si no hubo o no se consulto.
                        columna.Item().PaddingTop(4).Text("Sin registros en el período.")
                            .FontSize(9).Italic().FontColor(Colors.Grey.Darken1);
                        continue;
                    }

                    columna.Item().PaddingTop(4).Table(tabla =>
                    {
                        tabla.ColumnsDefinition(definicion =>
                        {
                            foreach (string unaColumna in unaSeccion.Columnas)
                            {
                                definicion.RelativeColumn();
                            }
                        });

                        tabla.Header(cabecera =>
                        {
                            for (int vIndice = 0; vIndice < unaSeccion.Columnas.Length; vIndice = vIndice + 1)
                            {
                                cabecera.Cell().Element(CeldaCabecera)
                                    .AlignedText(unaSeccion.Columnas[vIndice], unaSeccion.EsNumerica(vIndice));
                            }
                        });

                        foreach (string[] unaFila in unaSeccion.Filas)
                        {
                            for (int vIndice = 0; vIndice < unaFila.Length; vIndice = vIndice + 1)
                            {
                                tabla.Cell().Element(Celda)
                                    .AlignedText(unaFila[vIndice], unaSeccion.EsNumerica(vIndice));
                            }
                        }
                    });

                    if (unaSeccion.Nota != "")
                    {
                        columna.Item().PaddingTop(4).Text(unaSeccion.Nota)
                            .FontSize(8).Italic().FontColor(Colors.Grey.Darken1);
                    }
                }
            });
        }

        private static IContainer CeldaCabecera(IContainer pContenedor)
        {
            return pContenedor
                .Background(Colors.Grey.Lighten3)
                .BorderBottom(1).BorderColor(Colors.Grey.Medium)
                .PaddingVertical(4).PaddingHorizontal(3);
        }

        private static IContainer Celda(IContainer pContenedor)
        {
            return pContenedor
                .BorderBottom(1).BorderColor(Colors.Grey.Lighten2)
                .PaddingVertical(3).PaddingHorizontal(3);
        }
    }

    // Alinea a la derecha lo que es numero y a la izquierda lo que es texto. Los
    // numeros de una columna se comparan de un vistazo solo si estan alineados por la
    // unidad.
    internal static class ExtensionesDeCelda
    {
        public static void AlignedText(this IContainer pContenedor, string pTexto, bool pEsNumerica)
        {
            if (pEsNumerica)
            {
                pContenedor.AlignRight().Text(pTexto);
                return;
            }
            pContenedor.Text(pTexto);
        }
    }
}
