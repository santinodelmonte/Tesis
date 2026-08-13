# -*- coding: utf-8 -*-
"""Dibujo de los diagramas del proyecto.

Cada diagrama se declara una sola vez y de ahi salen las dos formas en que se
necesita: el .drawio que se abre y se edita en draw.io, y el PNG que se inserta en
el documento. Tener una sola definicion es lo que evita que el archivo editable y
la imagen del documento se separen con el tiempo.
"""
import html
import os
import xml.etree.ElementTree as ET

import cairosvg

FUENTE = 'Helvetica, Arial, sans-serif'
ANCHO_CARACTER = 0.55  # proporcion del tamano de fuente; alcanza para cortar lineas


# --------------------------------------------------------------------------- texto

def partir(texto, ancho_caja, tamano=12, margen=16):
    """Corta el texto en las lineas que entran en una caja del ancho dado."""
    limite = max(1, int((ancho_caja - margen) / (tamano * ANCHO_CARACTER)))
    palabras = texto.split()
    lineas, actual = [], ''
    for palabra in palabras:
        tentativa = (actual + ' ' + palabra).strip()
        if len(tentativa) <= limite or not actual:
            actual = tentativa
        else:
            lineas.append(actual)
            actual = palabra
    if actual:
        lineas.append(actual)
    return lineas


# --------------------------------------------------------------------------- figuras

class Figura:
    def __init__(self, ident, clase, x, y, ancho, alto, texto='', estilo=None):
        self.id = ident
        self.clase = clase
        self.x, self.y = x, y
        self.ancho, self.alto = ancho, alto
        self.texto = texto
        self.estilo = estilo or {}

    @property
    def centro(self):
        return (self.x + self.ancho / 2.0, self.y + self.alto / 2.0)


class Vinculo:
    def __init__(self, origen, destino, texto='', punteado=False, flecha=False,
                 desde='centro', hasta='centro'):
        self.origen, self.destino = origen, destino
        self.texto = texto
        self.punteado = punteado
        self.flecha = flecha
        # Anclar la linea a un borde en lugar del centro es lo que evita que las
        # asociaciones crucen por encima de las elipses vecinas.
        self.desde, self.hasta = desde, hasta

    @staticmethod
    def punto(figura, ancla):
        x, y = figura.centro
        if ancla == 'izquierda':
            return (figura.x, y)
        if ancla == 'derecha':
            return (figura.x + figura.ancho, y)
        if ancla == 'arriba':
            return (x, figura.y)
        if ancla == 'abajo':
            return (x, figura.y + figura.alto)
        return (x, y)

    @property
    def extremos(self):
        return (self.punto(self.origen, self.desde),
                self.punto(self.destino, self.hasta))


class Diagrama:
    """Un diagrama: figuras, vinculos y el tamano del lienzo."""

    def __init__(self, nombre, ancho, alto, titulo=''):
        self.nombre = nombre
        self.ancho, self.alto = ancho, alto
        self.titulo = titulo
        self.figuras = []
        self.vinculos = []
        self._n = 0

    def _ident(self):
        self._n += 1
        return 'f%d' % self._n

    def agregar(self, clase, x, y, ancho, alto, texto='', **estilo):
        figura = Figura(self._ident(), clase, x, y, ancho, alto, texto, estilo)
        self.figuras.append(figura)
        return figura

    def actor(self, x, y, texto):
        return self.agregar('actor', x, y, 40, 80, texto)

    def caso(self, x, y, texto, ancho=210, alto=54):
        return self.agregar('elipse', x, y, ancho, alto, texto)

    def marco(self, x, y, ancho, alto, texto):
        return self.agregar('marco', x, y, ancho, alto, texto)

    def caja(self, x, y, ancho, alto, texto, **estilo):
        return self.agregar('caja', x, y, ancho, alto, texto, **estilo)

    def tabla(self, x, y, titulo, filas, ancho=210, alto_fila=15, alto_titulo=24,
              **estilo):
        """Caja con encabezado y una linea por fila. Sirve para el MER y para las
        clases de los diagramas de dominio y persistencia."""
        alto = alto_titulo + max(1, len(filas)) * alto_fila + 6
        figura = self.agregar('tabla', x, y, ancho, alto, titulo, **estilo)
        figura.filas = filas
        figura.alto_fila = alto_fila
        figura.alto_titulo = alto_titulo
        return figura

    def unir(self, origen, destino, texto='', punteado=False, flecha=False,
             desde='centro', hasta='centro'):
        self.vinculos.append(
            Vinculo(origen, destino, texto, punteado, flecha, desde, hasta))


# --------------------------------------------------------------------------- SVG

# El tema del documento es sobrio y se imprime: gris de linea, relleno muy claro y
# texto negro. Nada de color de marca, que en blanco y negro se pierde.
BORDE = '#40484f'
RELLENO = '#f4f6f8'
RELLENO_MARCO = '#ffffff'
TEXTO = '#12181d'


def _texto_svg(partes, x, y, lineas, tamano=12, negrita=False, ancla='middle'):
    alto_linea = tamano * 1.25
    inicio = y - (len(lineas) - 1) * alto_linea / 2.0
    peso = ' font-weight="600"' if negrita else ''
    for i, linea in enumerate(lineas):
        partes.append(
            '<text x="%.1f" y="%.1f" font-family="%s" font-size="%d"%s fill="%s" '
            'text-anchor="%s" dominant-baseline="middle">%s</text>'
            % (x, inicio + i * alto_linea, FUENTE, tamano, peso, TEXTO, ancla,
               html.escape(linea)))


def a_svg(diagrama):
    p = ['<svg xmlns="http://www.w3.org/2000/svg" width="%d" height="%d" '
         'viewBox="0 0 %d %d">' % (diagrama.ancho, diagrama.alto,
                                   diagrama.ancho, diagrama.alto),
         '<rect width="100%" height="100%" fill="#ffffff"/>']

    if diagrama.titulo:
        _texto_svg(p, diagrama.ancho / 2.0, 26, [diagrama.titulo], 15, True)

    # Los marcos van primero para que queden por debajo del resto.
    for f in [x for x in diagrama.figuras if x.clase == 'marco']:
        p.append('<rect x="%.1f" y="%.1f" width="%.1f" height="%.1f" fill="%s" '
                 'stroke="%s" stroke-width="1.5"/>'
                 % (f.x, f.y, f.ancho, f.alto, RELLENO_MARCO, BORDE))
        _texto_svg(p, f.x + f.ancho / 2.0, f.y + 20, partir(f.texto, f.ancho, 13), 13, True)

    for v in diagrama.vinculos:
        (x1, y1), (x2, y2) = v.extremos
        guion = ' stroke-dasharray="6 4"' if v.punteado else ''
        punta = ' marker-end="url(#punta)"' if v.flecha else ''
        p.append('<line x1="%.1f" y1="%.1f" x2="%.1f" y2="%.1f" stroke="%s" '
                 'stroke-width="1.2"%s%s/>' % (x1, y1, x2, y2, BORDE, guion, punta))
        if v.texto:
            _texto_svg(p, (x1 + x2) / 2.0, (y1 + y2) / 2.0 - 8, [v.texto], 10)

    for f in diagrama.figuras:
        if f.clase == 'marco':
            continue
        cx, cy = f.centro
        if f.clase == 'elipse':
            p.append('<ellipse cx="%.1f" cy="%.1f" rx="%.1f" ry="%.1f" fill="%s" '
                     'stroke="%s" stroke-width="1.2"/>'
                     % (cx, cy, f.ancho / 2.0, f.alto / 2.0, RELLENO, BORDE))
            _texto_svg(p, cx, cy, partir(f.texto, f.ancho - 26, 11), 11)
        elif f.clase == 'caja':
            p.append('<rect x="%.1f" y="%.1f" width="%.1f" height="%.1f" rx="3" '
                     'fill="%s" stroke="%s" stroke-width="1.2"/>'
                     % (f.x, f.y, f.ancho, f.alto, f.estilo.get('relleno', RELLENO),
                        BORDE))
            _texto_svg(p, cx, cy, partir(f.texto, f.ancho, 11), 11,
                       f.estilo.get('negrita', False))
        elif f.clase == 'tabla':
            relleno = f.estilo.get('relleno', RELLENO)
            p.append('<rect x="%.1f" y="%.1f" width="%.1f" height="%.1f" rx="3" '
                     'fill="#ffffff" stroke="%s" stroke-width="1.3"/>'
                     % (f.x, f.y, f.ancho, f.alto, BORDE))
            p.append('<path d="M %.1f %.1f h %.1f v %.1f h %.1f Z" fill="%s"/>'
                     % (f.x + 1, f.y + f.alto_titulo, f.ancho - 2, -f.alto_titulo + 2,
                        -f.ancho + 2, relleno))
            p.append('<line x1="%.1f" y1="%.1f" x2="%.1f" y2="%.1f" stroke="%s" '
                     'stroke-width="1.3"/>'
                     % (f.x, f.y + f.alto_titulo, f.x + f.ancho,
                        f.y + f.alto_titulo, BORDE))
            _texto_svg(p, cx, f.y + f.alto_titulo / 2.0, [f.texto], 11.5, True)
            for i, fila in enumerate(f.filas):
                etiqueta, marca = fila if isinstance(fila, tuple) else (fila, '')
                y_fila = f.y + f.alto_titulo + 4 + i * f.alto_fila + f.alto_fila / 2.0
                _texto_svg(p, f.x + 7, y_fila, [etiqueta], 9.5, ancla='start')
                if marca:
                    _texto_svg(p, f.x + f.ancho - 7, y_fila, [marca], 8.5, True, 'end')
        elif f.clase == 'actor':
            cabeza = f.y + 12
            p.append('<circle cx="%.1f" cy="%.1f" r="10" fill="%s" stroke="%s" '
                     'stroke-width="1.4"/>' % (cx, cabeza, RELLENO, BORDE))
            p.append('<path d="M %.1f %.1f L %.1f %.1f M %.1f %.1f L %.1f %.1f '
                     'M %.1f %.1f L %.1f %.1f M %.1f %.1f L %.1f %.1f" stroke="%s" '
                     'stroke-width="1.4" fill="none"/>'
                     % (cx, cabeza + 10, cx, cabeza + 34,
                        cx - 16, cabeza + 18, cx + 16, cabeza + 18,
                        cx, cabeza + 34, cx - 13, cabeza + 54,
                        cx, cabeza + 34, cx + 13, cabeza + 54, BORDE))
            _texto_svg(p, cx, f.y + f.alto + 14, partir(f.texto, 150, 11), 11)

    p.insert(1, '<defs><marker id="punta" markerWidth="9" markerHeight="9" '
                'refX="8" refY="3" orient="auto"><path d="M0,0 L8,3 L0,6" '
                'fill="none" stroke="%s" stroke-width="1.2"/></marker></defs>' % BORDE)
    p.append('</svg>')
    return '\n'.join(p)


# --------------------------------------------------------------------------- drawio

ESTILOS = {
    'elipse': 'ellipse;whiteSpace=wrap;html=1;fillColor=#f4f6f8;strokeColor=#40484f;',
    'caja': 'rounded=1;whiteSpace=wrap;html=1;fillColor=#f4f6f8;strokeColor=#40484f;',
    'marco': 'rounded=0;whiteSpace=wrap;html=1;fillColor=none;strokeColor=#40484f;'
             'verticalAlign=top;fontStyle=1;',
    'actor': 'shape=umlActor;verticalLabelPosition=bottom;verticalAlign=top;html=1;'
             'outlineConnect=0;fillColor=#f4f6f8;strokeColor=#40484f;',
    'tabla': 'rounded=0;whiteSpace=wrap;html=1;fillColor=#ffffff;strokeColor=#40484f;'
             'align=left;verticalAlign=top;spacingLeft=6;spacingTop=2;fontSize=10;',
}


def a_drawio(diagrama):
    mxfile = ET.Element('mxfile', host='app.diagrams.net')
    pagina = ET.SubElement(mxfile, 'diagram', name=diagrama.nombre)
    modelo = ET.SubElement(pagina, 'mxGraphModel', dx='1100', dy='800', grid='1',
                           gridSize='10', page='1',
                           pageWidth=str(diagrama.ancho), pageHeight=str(diagrama.alto))
    raiz = ET.SubElement(modelo, 'root')
    ET.SubElement(raiz, 'mxCell', id='0')
    ET.SubElement(raiz, 'mxCell', id='1', parent='0')

    # Los marcos primero, por el mismo motivo que en el SVG.
    ordenadas = ([f for f in diagrama.figuras if f.clase == 'marco'] +
                 [f for f in diagrama.figuras if f.clase != 'marco'])
    for f in ordenadas:
        valor = f.texto
        if f.clase == 'tabla':
            # draw.io interpreta el valor como HTML: el encabezado en negrita y una
            # linea por columna, que es la forma en que se edita comodo despues.
            filas = []
            for fila in f.filas:
                etiqueta, marca = fila if isinstance(fila, tuple) else (fila, '')
                filas.append(html.escape(etiqueta) +
                             ('  <i>%s</i>' % html.escape(marca) if marca else ''))
            valor = '<b>%s</b><hr>%s' % (html.escape(f.texto), '<br>'.join(filas))
        celda = ET.SubElement(raiz, 'mxCell', id=f.id, value=valor,
                              style=ESTILOS[f.clase], vertex='1', parent='1')
        ET.SubElement(celda, 'mxGeometry', x=str(f.x), y=str(f.y),
                      width=str(f.ancho), height=str(f.alto), **{'as': 'geometry'})

    anclas = {'izquierda': (0, 0.5), 'derecha': (1, 0.5),
              'arriba': (0.5, 0), 'abajo': (0.5, 1)}
    for i, v in enumerate(diagrama.vinculos):
        estilo = 'edgeStyle=none;html=1;strokeColor=#40484f;'
        estilo += 'dashed=1;' if v.punteado else 'dashed=0;'
        estilo += 'endArrow=open;' if v.flecha else 'endArrow=none;'
        if v.desde in anclas:
            estilo += 'exitX=%s;exitY=%s;exitDx=0;exitDy=0;' % anclas[v.desde]
        if v.hasta in anclas:
            estilo += 'entryX=%s;entryY=%s;entryDx=0;entryDy=0;' % anclas[v.hasta]
        celda = ET.SubElement(raiz, 'mxCell', id='v%d' % i, value=v.texto, style=estilo,
                              edge='1', parent='1', source=v.origen.id,
                              target=v.destino.id)
        ET.SubElement(celda, 'mxGeometry', relative='1', **{'as': 'geometry'})

    return ET.tostring(mxfile, encoding='unicode')


# --------------------------------------------------------------------------- salida

def escribir(diagrama, carpeta):
    os.makedirs(carpeta, exist_ok=True)
    base = os.path.join(carpeta, diagrama.nombre)

    with open(base + '.drawio', 'w', encoding='utf-8') as archivo:
        archivo.write(a_drawio(diagrama))

    svg = a_svg(diagrama)
    with open(base + '.svg', 'w', encoding='utf-8') as archivo:
        archivo.write(svg)

    cairosvg.svg2png(bytestring=svg.encode('utf-8'), write_to=base + '.png',
                     output_width=diagrama.ancho * 2, output_height=diagrama.alto * 2)
    return base
