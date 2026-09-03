# -*- coding: utf-8 -*-
"""Compara las capturas que las secciones piden contra las que el guion define.

Las listas se escriben por separado -el manual, las pruebas y el guion- y se
desincronizan solas: una captura que una seccion pide y el guion no define no se saca
nunca, y el hueco aparece recien en el .docx. Esto lo dice antes.
"""
import os
import re
import sys

AQUI = os.path.dirname(os.path.abspath(__file__))
SECCIONES = [os.path.join(AQUI, 'seccion-2-4-manual.md'),
             os.path.join(AQUI, 'seccion-2-3-pruebas.md')]
GUION = os.path.join(AQUI, 'guion-capturas.md')
CAPTURAS = os.path.join(AQUI, 'capturas')


def leer(ruta):
    with open(ruta, encoding='utf-8') as f:
        return f.read()


def main():
    pedidas = set()
    for ruta in SECCIONES:
        if os.path.exists(ruta):
            pedidas |= set(re.findall(r'\[captura: ([a-z0-9\-]+)\]', leer(ruta)))
    definidas = set(re.findall(r'^\| `([a-z0-9\-]+)`', leer(GUION), re.M))

    sin_guion = sorted(pedidas - definidas)
    sin_usar = sorted(definidas - pedidas)

    print('las secciones piden %d capturas | el guion define %d' % (len(pedidas), len(definidas)))

    if sin_guion:
        print('\nLas secciones las piden y el guion no las define:')
        for nombre in sin_guion:
            print('  -', nombre)
    if sin_usar:
        print('\nEl guion las define y ninguna seccion las usa:')
        for nombre in sin_usar:
            print('  -', nombre)

    if os.path.isdir(CAPTURAS):
        hay = {a.rsplit('.', 1)[0] for a in os.listdir(CAPTURAS) if a.endswith('.png')}
        faltan = sorted(pedidas - hay)
        print('\nsacadas: %d de %d' % (len(pedidas & hay), len(pedidas)))
        if faltan:
            print('faltan sacar: ' + ', '.join(faltan[:8])
                  + (' y %d mas' % (len(faltan) - 8) if len(faltan) > 8 else ''))
    else:
        print('\nTodavia no hay docs/capturas: el sistema se levanta en la maquina de desarrollo.')

    return 1 if sin_guion else 0


if __name__ == '__main__':
    sys.exit(main())
