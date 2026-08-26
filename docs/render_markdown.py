# -*- coding: utf-8 -*-
"""Vuelca un .md de seccion al .docx, usando los helpers del Documento.

Las secciones 2.3 a 2.9 se escriben como markdown en docs/ y este modulo las coloca
en el documento. Entiende lo que esas secciones usan y nada mas: titulos, parrafos,
tablas, citas, vinietas, negrita, codigo y las marcas de captura.

Las marcas [captura: nombre] se resuelven contra docs/capturas/nombre.png. Si la
imagen todavia no esta -las capturas se sacan con el sistema andando- se escribe un
marcador visible en su lugar, para que el hueco se vea en el documento y no pase
inadvertido.
"""
import os
import re

AQUI = os.path.dirname(os.path.abspath(__file__))
CAPTURAS = os.path.join(AQUI, 'capturas')

# El encabezado de trabajo de cada .md -entre el titulo y la primera regla- explica
# como esta escrita la seccion y no forma parte del documento.
CORTE = re.compile(r'^---\s*$', re.M)


def _limpiar(texto):
    """Saca el marcado que Word resuelve con formato y no con caracteres."""
    texto = re.sub(r'\*\*(.+?)\*\*', r'\1', texto)
    texto = re.sub(r'(?<!\w)\*(.+?)\*(?!\w)', r'\1', texto)
    texto = re.sub(r'`(.+?)`', r'\1', texto)
    texto = re.sub(r'\[(.+?)\]\(.+?\)', r'\1', texto)
    return texto.strip()


def _cuerpo(texto):
    """Descarta el encabezado de trabajo: empieza despues de la primera regla."""
    partes = CORTE.split(texto, maxsplit=1)
    return partes[1] if len(partes) > 1 else texto


def _fila(linea):
    return [_limpiar(c) for c in linea.strip().strip('|').split('|')]


def volcar(d, ruta_md, nivel_base=4, titulos_como_parrafo=False):
    """Escribe el contenido del markdown detras del cursor del documento.

    Con titulos_como_parrafo los encabezados salen en negrita y no como estilo de
    titulo. Es lo que necesita el Manual de Usuario: tiene numeracion propia -1, 2,
    2.1, 3- que chocaria con la del documento y le ensuciaria el indice. El manual es
    un documento adentro del documento y lleva su propio indice.
    """
    with open(ruta_md, encoding='utf-8') as archivo:
        lineas = _cuerpo(archivo.read()).split('\n')

    faltantes = []
    i = 0
    while i < len(lineas):
        linea = lineas[i]
        desnuda = linea.strip()

        # tabla: encabezado, separador y filas
        if desnuda.startswith('|') and i + 1 < len(lineas) and \
                set(lineas[i + 1].strip()) <= set('|-: '):
            encabezados = _fila(desnuda)
            filas = []
            i = i + 2
            while i < len(lineas) and lineas[i].strip().startswith('|'):
                filas.append(_fila(lineas[i]))
                i = i + 1
            if any(any(c for c in f) for f in filas):
                d.tabla(encabezados, filas)
            else:
                # tabla vacia a proposito -la de registro de errores-: va con una fila
                d.tabla(encabezados, [[''] * len(encabezados)])
            continue

        # captura
        marca = re.match(r'`\[captura: ([a-z0-9\-]+)\]`', desnuda)
        if marca:
            nombre = marca.group(1)
            ruta = os.path.join(CAPTURAS, nombre + '.png')
            if os.path.exists(ruta):
                pie = ''
                if i + 1 < len(lineas) and lineas[i + 1].strip().startswith('>'):
                    pie = _limpiar(lineas[i + 1].strip().lstrip('> '))
                    i = i + 1
                d.imagen(ruta, pie)
            else:
                faltantes.append(nombre)
                d.parrafo('[falta la captura: %s]' % nombre, negrita=True)
            i = i + 1
            continue

        if desnuda.startswith('#'):
            nivel = len(desnuda) - len(desnuda.lstrip('#'))
            texto = _limpiar(desnuda.lstrip('#'))
            if titulos_como_parrafo:
                d.parrafo('')
                d.parrafo(texto, negrita=True)
            else:
                d.titulo(texto, nivel=min(nivel_base + nivel - 2, 6))
        elif desnuda.startswith('>'):
            d.parrafo(_limpiar(desnuda.lstrip('> ')))
        elif desnuda.startswith(('- ', '* ')):
            d.parrafo('• ' + _limpiar(desnuda[2:]))
        elif re.match(r'^\d+\. ', desnuda):
            d.parrafo(_limpiar(desnuda))
        elif desnuda and not CORTE.match(desnuda):
            # parrafo: junta las lineas hasta el proximo corte
            partes = [desnuda]
            while i + 1 < len(lineas) and lineas[i + 1].strip() and \
                    not lineas[i + 1].strip().startswith(('#', '>', '- ', '* ', '|', '`[captura')) and \
                    not CORTE.match(lineas[i + 1].strip()):
                i = i + 1
                partes.append(lineas[i].strip())
            d.parrafo(_limpiar(' '.join(partes)))
        i = i + 1

    return faltantes
