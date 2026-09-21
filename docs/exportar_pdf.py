# -*- coding: utf-8 -*-
"""Exporta los .docx a PDF con los indices calculados.

La conversion directa de LibreOffice (`soffice --convert-to pdf`) deja el indice
sin numeros de pagina: el documento trae los campos TOC sin resultado y la marca
`w:updateFields`, que pide actualizarlos al abrir, la respeta Word pero no la linea
de comandos. Un indice sin numeros de pagina no sirve para nada, asi que el export
se hace por la otra puerta: se levanta LibreOffice como servicio, se abre el
documento por el puente UNO, se actualizan los indices y recien ahi se exporta.

Los PDF son generados, igual que los .docx: no se corrigen a mano.

Una advertencia sobre los numeros del indice. Armar el indice cambia el largo del
documento, y el largo del documento cambia los numeros del indice: la cuenta no tiene
un punto fijo, y LibreOffice oscila entre dos paginaciones que difieren en una pagina.
Los titulos que caen justo en un corte de pagina quedan, por eso, con un numero de
diferencia; en el ultimo control fueron ocho de cincuenta y nueve. El .docx es el
documento de referencia y Word rehace la cuenta al abrirlo, asi que el PDF sirve para
leer y para entregar, pero el indice exacto sale de Word.
"""
import os
import subprocess
import sys
import time

AQUI = os.path.dirname(os.path.abspath(__file__))
RAIZ = os.path.dirname(AQUI)
PERFIL = '/tmp/lo-export-tesis'
PUERTO = 2002

DOCUMENTOS = ['Anteproyecto_v8.docx', 'Proyecto_v7.docx', 'Tesis.docx']


def url(ruta):
    import uno
    return uno.systemPathToFileUrl(os.path.abspath(ruta))


def propiedad(nombre, valor):
    from com.sun.star.beans import PropertyValue
    p = PropertyValue()
    p.Name = nombre
    p.Value = valor
    return p


def levantar():
    """Arranca LibreOffice como servicio y devuelve el contexto, o muere en el intento."""
    import uno
    from com.sun.star.connection import NoConnectException

    proceso = subprocess.Popen([
        'soffice', '--headless', '--norestore', '--invisible',
        '-env:UserInstallation=file://' + PERFIL,
        '--accept=socket,host=localhost,port=%d;urp;' % PUERTO,
    ])
    local = uno.getComponentContext()
    resolver = local.ServiceManager.createInstanceWithContext(
        'com.sun.star.bridge.UnoUrlResolver', local)
    destino = ('uno:socket,host=localhost,port=%d;urp;'
               'StarOffice.ComponentContext' % PUERTO)
    # El servicio tarda unos segundos en abrir el socket; se reintenta en vez de dormir
    # un numero magico de segundos.
    for _ in range(60):
        try:
            return proceso, resolver.resolve(destino)
        except NoConnectException:
            time.sleep(1)
    proceso.terminate()
    raise SystemExit('LibreOffice no abrio el puerto %d' % PUERTO)


def niveles_de_esquema(doc):
    """Le devuelve a los titulos su nivel de esquema, que la importacion pierde.

    Los documentos vienen de una herramienta que escribe los estilos de titulo sin el
    `w:outlineLvl`: Word deduce el nivel del estilo integrado, LibreOffice no. Sin nivel
    de esquema no hay titulos que indexar, y el indice sale vacio. Se lo asigna al
    estilo, no al parrafo, para no pisar los parrafos que el armado saco del indice a
    proposito.
    """
    familia = doc.getStyleFamilies().getByName('ParagraphStyles')
    puestos = 0
    for nivel in range(1, 7):
        nombre = 'Heading %d' % nivel
        if not familia.hasByName(nombre):
            continue
        estilo = familia.getByName(nombre)
        if estilo.OutlineLevel == 0:
            estilo.OutlineLevel = nivel
            puestos += 1
    return puestos


def estilos_asignados(indice):
    """Dice si el indice nombra estilos por nivel, como hace `TOC \\t "Heading 1,1,..."`."""
    niveles = indice.LevelParagraphStyles
    return any(len(niveles.getByIndex(i)) for i in range(niveles.getCount()))


def exportar(escritorio, nombre):
    doc = escritorio.loadComponentFromURL(
        url(os.path.join(RAIZ, nombre)), '_blank', 0, (propiedad('Hidden', True),))
    try:
        niveles_de_esquema(doc)

        indices = doc.getDocumentIndexes()
        for i in range(indices.getCount()):
            indice = indices.getByIndex(i)
            if estilos_asignados(indice):
                # El indice del anteproyecto se arma con `TOC \\t`, que nombra los
                # estilos uno por uno. LibreOffice ademas lo arma por nivel de esquema,
                # y cada titulo termina listado dos veces. Se queda con los estilos,
                # que es lo que el documento pidio.
                indice.CreateFromOutline = False
            elif i > 0:
                # El indice del manual va acotado con un marcador —`TOC \\b manual`—,
                # que LibreOffice no entiende. Acotarlo al capitulo donde esta da
                # casi el mismo recorte, porque el manual es un capitulo entero.
                indice.CreateFromChapter = True
            indice.update()

        # Segunda vuelta: los numeros de pagina dependen de cuanto ocupa el indice, que
        # recien se sabe despues de armarlo.
        doc.getTextFields().refresh()
        doc.refresh()
        for i in range(indices.getCount()):
            indices.getByIndex(i).update()
        doc.refresh()
        # Recorrer hasta la ultima pagina obliga a paginar todo el documento, que es lo
        # que hace repetible el resultado: sin esto, el export sale distinto segun
        # cuanto haya alcanzado a calcular el maquetador.
        doc.getCurrentController().getViewCursor().jumpToLastPage()

        salida = os.path.join(RAIZ, os.path.splitext(nombre)[0] + '.pdf')
        doc.storeToURL(url(salida), (
            propiedad('FilterName', 'writer_pdf_Export'),
            propiedad('FilterData', uno_secuencia([
                # Marcadores y enlaces del indice, que es la ventaja de entregar un PDF.
                propiedad('ExportBookmarks', True),
                propiedad('UseTaggedPDF', True),
                # Las capturas son PNG: la compresion con perdida las arruina.
                propiedad('UseLosslessCompression', True),
            ])),
        ))
        entradas = sum(len(indices.getByIndex(i).getAnchor().getString().splitlines())
                       for i in range(indices.getCount()))
        return salida, indices.getCount(), entradas
    finally:
        doc.close(False)


def uno_secuencia(valores):
    import uno
    return uno.Any('[]com.sun.star.beans.PropertyValue', tuple(valores))


def main():
    pedidos = sys.argv[1:] or DOCUMENTOS
    proceso, contexto = levantar()
    try:
        escritorio = contexto.ServiceManager.createInstanceWithContext(
            'com.sun.star.frame.Desktop', contexto)
        for nombre in pedidos:
            salida, cuantos, entradas = exportar(escritorio, nombre)
            print('%s -> %s (%d indices, %d entradas, %.1f MB)' % (
                nombre, os.path.basename(salida), cuantos, entradas,
                os.path.getsize(salida) / 1024.0 / 1024.0))
    finally:
        try:
            contexto.ServiceManager.createInstanceWithContext(
                'com.sun.star.frame.Desktop', contexto).terminate()
        except Exception:
            proceso.terminate()
        proceso.wait()


if __name__ == '__main__':
    main()
