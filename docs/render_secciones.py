# -*- coding: utf-8 -*-
"""Convierte las secciones 2.3 a 2.7 de markdown al cuerpo del Proyecto.

Las secciones se escriben en `docs/seccion-2-*.md` porque en markdown se corrigen y se
revisan sin abrir Word. Este modulo las traduce al documento: titulos, parrafos con su
negrita y su cursiva, tablas, listas, y las marcas `[captura: nombre]` con el pie que
las sigue.

Dos reglas de lectura, que son la convencion con que estan escritos los cinco archivos:

1. **Todo lo que hay entre el titulo `#` y la primera linea `---` es andamiaje** —de
   donde salio la seccion, que falta, que decision la condiciona— y no se entrega. El
   documento empieza despues de esa raya.
2. **Una cita `>` es el pie de una figura y solo eso**: se escribe pegada debajo de su
   marca `[captura: ...]`. Una cita que no venga detras de una marca es una nota de
   trabajo y tampoco se entrega.

Los titulos de adentro de estas secciones se escriben con estilo Heading5 y Heading6 a
proposito: el indice del documento final llega hasta el nivel 4 —que es donde viven
2.2.5.1 y las demas—, de modo que el manual de usuario no le vuelca sus cuarenta y
cuatro subtitulos encima. El manual lleva su propio indice, como en el ejemplo de la
catedra.
"""
import os
import re

AQUI = os.path.dirname(os.path.abspath(__file__))
CAPTURAS = os.path.join(AQUI, 'capturas')

SECCIONES = [
    ('2.3', '2.4', 'seccion-2-3-pruebas.md'),
    ('2.4', '2.5', 'seccion-2-4-manual.md'),
    ('2.5', '2.6', 'seccion-2-5-deployment.md'),
    ('2.6', '2.7', 'seccion-2-6-seguridad.md'),
    ('2.7', '2.8', 'seccion-2-7-contingencia.md'),
    # La 2.9 cierra el documento: no hay titulo siguiente que marque donde termina.
    ('2.9', None, 'seccion-2-9-conclusiones.md'),
]

MARCA_CAPTURA = re.compile(r'^`?\[captura:\s*([a-z0-9\-]+)\s*\]`?$')
# Los tres tramos son, en orden: **negrita**, `codigo` y *cursiva*. La cursiva va
# ultima y exige que el asterisco no este pegado a otro, para que no se coma la
# negrita que ya se reconocio.
INLINE = re.compile(r'\*\*(.+?)\*\*|`([^`]+)`|(?<!\*)\*([^*]+?)\*(?!\*)')


# --------------------------------------------------------------------------- lectura

def _preambulo_cortado(lineas):
    """Devuelve las lineas del cuerpo: lo que sigue a la primera raya horizontal."""
    for i, linea in enumerate(lineas):
        if linea.strip() == '---':
            return lineas[i + 1:]
    raise ValueError('el archivo no tiene la raya que separa el andamiaje del cuerpo')


def _fila(linea):
    """Parte una fila de tabla en celdas, sin los pipes de los extremos."""
    return [c.strip() for c in linea.strip().strip('|').split('|')]


def _es_separador(linea):
    return bool(re.match(r'^\|[\s:\-|]+\|$', linea.strip()))


def bloques(ruta):
    """Lee un .md y devuelve la lista de bloques que hay que escribir.

    Cada bloque es una tupla `(clase, valor)`. Las clases son: titulo, parrafo, vineta,
    numerada, tabla y captura.
    """
    with open(ruta, 'rb') as f:
        lineas = f.read().decode('utf-8').replace('\r\n', '\n').split('\n')
    lineas = _preambulo_cortado(lineas)

    salida = []
    parrafo = []

    def cerrar():
        if parrafo:
            salida.append(('parrafo', ' '.join(parrafo).strip()))
            del parrafo[:]

    i = 0
    while i < len(lineas):
        linea = lineas[i]
        pelada = linea.strip()

        if not pelada or pelada == '---':
            cerrar()
            i += 1
            continue

        if pelada.startswith('#'):
            cerrar()
            nivel = len(pelada) - len(pelada.lstrip('#'))
            salida.append(('titulo', (nivel, pelada.lstrip('#').strip())))
            i += 1
            continue

        marca = MARCA_CAPTURA.match(pelada)
        if marca:
            cerrar()
            nombre = marca.group(1)
            pie = []
            j = i + 1
            while j < len(lineas) and lineas[j].startswith('>'):
                pie.append(lineas[j].lstrip('>').strip())
                j += 1
            salida.append(('captura', (nombre, ' '.join(p for p in pie if p).strip())))
            i = j
            continue

        if pelada.startswith('>'):
            # Cita que no es pie de figura: nota de trabajo, no se entrega.
            cerrar()
            while i < len(lineas) and lineas[i].startswith('>'):
                i += 1
            continue

        if pelada.startswith('|'):
            cerrar()
            encabezados = _fila(pelada)
            filas = []
            j = i + 1
            if j < len(lineas) and _es_separador(lineas[j]):
                j += 1
            while j < len(lineas) and lineas[j].strip().startswith('|'):
                filas.append(_fila(lineas[j]))
                j += 1
            # Una fila entera vacia es el renglon en blanco que espera a que alguien
            # complete la tabla al ejecutar; se conserva, porque eso es lo que se llena.
            salida.append(('tabla', (encabezados, filas)))
            i = j
            continue

        if pelada.startswith('- '):
            cerrar()
            sangria = (len(linea) - len(linea.lstrip(' '))) // 3
            texto = [pelada[2:].strip()]
            j = i + 1
            while j < len(lineas) and lineas[j].strip() and \
                    not re.match(r'^\s*(-\s|\d+\.\s|\||>|#)', lineas[j]):
                texto.append(lineas[j].strip())
                j += 1
            salida.append(('vineta', (sangria, ' '.join(texto))))
            i = j
            continue

        numerada = re.match(r'^(\d+)\.\s+(.*)$', pelada)
        if numerada:
            cerrar()
            texto = [numerada.group(2).strip()]
            j = i + 1
            while j < len(lineas) and lineas[j].strip() and \
                    not re.match(r'^\s*(-\s|\d+\.\s|\||>|#)', lineas[j]):
                texto.append(lineas[j].strip())
                j += 1
            salida.append(('numerada', (numerada.group(1), ' '.join(texto))))
            i = j
            continue

        parrafo.append(pelada)
        i += 1

    cerrar()
    return salida


# --------------------------------------------------------------------------- escritura

def segmentos(texto):
    """Parte un texto en tramos `(contenido, formato)` segun su marcado markdown."""
    partes = []
    posicion = 0
    for m in INLINE.finditer(texto):
        if m.start() > posicion:
            partes.append((texto[posicion:m.start()], {}))
        if m.group(1) is not None:
            partes.append((m.group(1), {'negrita': True}))
        elif m.group(2) is not None:
            partes.append((m.group(2), {'mono': True}))
        else:
            partes.append((m.group(3), {'cursiva': True}))
        posicion = m.end()
    if posicion < len(texto):
        partes.append((texto[posicion:], {}))
    return partes or [(texto, {})]


def escribir(d, ruta_md, faltantes, base_titulo=3):
    """Escribe en el documento los bloques de un archivo. Devuelve cuantos escribio."""
    escritos = 0
    for clase, valor in bloques(ruta_md):
        if clase == 'titulo':
            nivel, texto = valor
            # En el Proyecto, `##` es el primer subtitulo de una seccion 2.x y va en
            # Heading5, para que no entre al indice. En el Anteproyecto, cuyas secciones
            # son Heading2, el mismo `##` es un Heading3.
            d.parrafo_rico(segmentos(texto),
                           estilo='Heading%d' % min(6, nivel + base_titulo))
        elif clase == 'parrafo':
            d.parrafo_rico(segmentos(valor))
        elif clase == 'vineta':
            sangria, texto = valor
            d.parrafo_rico([('•\t', {})] + segmentos(texto), estilo='ListParagraph',
                           sangria=sangria)
        elif clase == 'numerada':
            numero, texto = valor
            d.parrafo_rico([('%s.\t' % numero, {})] + segmentos(texto),
                           estilo='ListParagraph')
        elif clase == 'tabla':
            encabezados, filas = valor
            d.tabla_rica([segmentos(c) for c in encabezados],
                         [[segmentos(c) for c in fila] for fila in filas])
        elif clase == 'captura':
            nombre, pie = valor
            ruta = os.path.join(CAPTURAS, nombre + '.png')
            pie = ('Figura. ' + pie) if pie else ''
            if os.path.exists(ruta):
                d.imagen(ruta, pie)
            else:
                # Marcador visible: un hueco silencioso se entrega sin que nadie lo note.
                faltantes.append(nombre)
                d.parrafo_rico([('[FALTA LA CAPTURA: %s]' % nombre, {'negrita': True})])
                if pie:
                    d.parrafo_rico(segmentos(pie))
        escritos += 1
    return escritos
