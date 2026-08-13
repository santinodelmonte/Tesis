# -*- coding: utf-8 -*-
"""Diagramas de casos de uso, uno por modulo (seccion 2.2.1).

Los casos se leen de docs/casos_de_uso_parte*.py, de modo que el diagrama no pueda
quedar desfasado del texto: si se agrega un caso de uso, aparece en el diagrama al
volver a generar.
"""
import os
import sys

AQUI = os.path.dirname(os.path.abspath(__file__))
sys.path.insert(0, AQUI)
sys.path.insert(0, os.path.dirname(AQUI))

from lib_diagramas import Diagrama, escribir  # noqa: E402
from render_casos_de_uso import cargar  # noqa: E402

# Casos de uso incluidos por otro, que se dibujan como «include». No son casos de uso
# del catalogo: son pasos que dos casos comparten y conviene mostrar una sola vez.
INCLUIDOS = {
    4: 'Validar Unicidad de Caravana',
    21: 'Descontar Semen de Stock',
    28: 'Descontar Insumo de Stock',
}

MARGEN = 30
ALTO_CASO = 56
SEPARACION = 24
ANCHO_CASO = 250
X_ACTOR = 65
X_MARCO = 205
ANCHO_MARCO = 290


def diagrama_modulo(numero, titulo, casos):
    incluidos = [(c['num'], INCLUIDOS[c['num']]) for c in casos if c['num'] in INCLUIDOS]

    filas = len(casos)
    alto_marco = MARGEN + 22 + filas * (ALTO_CASO + SEPARACION)
    alto = alto_marco + 90
    # El resumen diario suma un actor Sistema a la derecha del marco, que necesita
    # su propio lugar.
    con_sistema = any(c['num'] == 49 for c in casos)
    ancho = X_MARCO + ANCHO_MARCO + (270 if incluidos else 170 if con_sistema else 60)

    d = Diagrama('cu-modulo-%d' % numero, ancho, alto, titulo)

    actor = d.actor(X_ACTOR, alto_marco / 2.0 + 20, 'Encargada del sector')
    marco = d.marco(X_MARCO, 46, ANCHO_MARCO, alto_marco - 20, 'Sistema de Gestión de Tambo')

    x_caso = marco.x + (ANCHO_MARCO - ANCHO_CASO) / 2.0
    y = marco.y + 40
    puestos = {}
    for caso in casos:
        figura = d.caso(x_caso, y, 'CU%d — %s' % (caso['num'], caso['nombre']),
                        ANCHO_CASO, ALTO_CASO)
        puestos[caso['num']] = figura
        # El resumen diario lo dispara un proceso programado: la encargada es la
        # destinataria, no quien lo ejecuta.
        if caso['modulo'] == 7 and caso['num'] == 49:
            sistema = d.actor(ancho - 70, y - 4, 'Sistema')
            d.unir(sistema, figura, desde='izquierda', hasta='derecha')
            d.unir(figura, actor, desde='izquierda', hasta='derecha')
        else:
            d.unir(actor, figura, desde='derecha', hasta='izquierda')
        y += ALTO_CASO + SEPARACION

    y_incluido = marco.y + 40
    for num, nombre in incluidos:
        figura = d.caso(marco.x + ANCHO_MARCO + 40, y_incluido, nombre, 205, 50)
        d.unir(puestos[num], figura, '«include»', punteado=True, flecha=True,
               desde='derecha', hasta='izquierda')
        y_incluido += 50 + SEPARACION

    return d


def main():
    casos, modulos = cargar()
    salida = os.path.join(AQUI, 'casos-de-uso')

    for numero in sorted(modulos):
        del_modulo = [c for c in casos if c['modulo'] == numero]
        if not del_modulo:
            continue
        d = diagrama_modulo(numero, modulos[numero], del_modulo)
        base = escribir(d, salida)
        print('%-46s %2d casos' % (os.path.basename(base), len(del_modulo)))


if __name__ == '__main__':
    main()
