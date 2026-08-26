# -*- coding: utf-8 -*-
"""Compara las capturas que el manual pide contra las que el guion define.

Las dos listas se escriben por separado y se desincronizan solas: una captura que el
manual pide y el guion no define no se saca nunca, y el hueco aparece recien en el
.docx. Esto lo dice antes.
"""
import os
import re
import sys

AQUI = os.path.dirname(os.path.abspath(__file__))
MANUAL = os.path.join(AQUI, 'seccion-2-4-manual.md')
GUION = os.path.join(AQUI, 'guion-capturas.md')
CAPTURAS = os.path.join(AQUI, 'capturas')


def leer(ruta):
    with open(ruta, encoding='utf-8') as f:
        return f.read()


def main():
    pedidas = set(re.findall(r'\[captura: ([a-z0-9\-]+)\]', leer(MANUAL)))
    definidas = set(re.findall(r'^\| `([a-z0-9\-]+)`', leer(GUION), re.M))

    sin_guion = sorted(pedidas - definidas)
    sin_usar = sorted(definidas - pedidas)

    print('el manual pide %d capturas | el guion define %d' % (len(pedidas), len(definidas)))

    if sin_guion:
        print('\nEl manual las pide y el guion no las define:')
        for nombre in sin_guion:
            print('  -', nombre)
    if sin_usar:
        print('\nEl guion las define y el manual no las usa:')
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
