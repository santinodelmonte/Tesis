# -*- coding: utf-8 -*-
"""Organigrama del establecimiento, para la Presentacion del Cliente (1.2).

El ejemplo de la catedra dedica una subseccion entera a la estructura organizacional
del cliente, y el nuestro la contaba en prosa. En un tambo familiar con una sola
encargada, cuatro subsecciones serian relleno; un organigrama chico, en cambio, cierra
la seccion de actores de un golpe de vista y explica sin decirlo por que el sistema
tiene un unico usuario.

La linea llena es dependencia; la punteada, un vinculo externo que no integra el
establecimiento. El recuadro marca quien opera el sistema, que es la unica persona que
lo hace: todos los demas aportan o reciben informacion sin entrar nunca a la
aplicacion.
"""
import os
import sys

AQUI = os.path.dirname(os.path.abspath(__file__))
sys.path.insert(0, AQUI)

from lib_diagramas import Diagrama, escribir  # noqa: E402

ANCHO_CAJA = 250
ALTO_CAJA = 58
# La encargada va con otro relleno: es la unica que entra al sistema.
RELLENO_USUARIA = '#e6efe8'
SALIDA = AQUI


def construir():
    d = Diagrama('organigrama-establecimiento', 580, 320,
                 'Estructura del establecimiento')

    centro = 60
    duenio = d.caja(centro, 56, ANCHO_CAJA, ALTO_CAJA,
                    'Juan Vila — dueño del establecimiento')
    encargada = d.caja(centro, 146, ANCHO_CAJA, ALTO_CAJA,
                       'Sofía Vila — encargada del sector, única usuaria del sistema',
                       relleno=RELLENO_USUARIA, negrita=True)
    tamberos = d.caja(centro, 236, ANCHO_CAJA, ALTO_CAJA,
                      'Tamberos — ordeñe y tareas diarias')
    veterinario = d.caja(centro + ANCHO_CAJA + 60, 146, 200, ALTO_CAJA,
                         'Médico veterinario — servicio externo')

    d.unir(duenio, encargada, desde='abajo', hasta='arriba')
    d.unir(encargada, tamberos, desde='abajo', hasta='arriba')
    d.unir(encargada, veterinario, punteado=True, desde='derecha', hasta='izquierda')
    return d


def main():
    base = escribir(construir(), SALIDA)
    print('generado', base + '.png')


if __name__ == '__main__':
    main()
