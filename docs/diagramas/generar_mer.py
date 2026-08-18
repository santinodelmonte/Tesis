# -*- coding: utf-8 -*-
"""Modelo Entidad-Relacion (seccion 2.2.5.1), leido de bd/CreacionDb.sql."""
import os
import sys

AQUI = os.path.dirname(os.path.abspath(__file__))
sys.path.insert(0, AQUI)

from esquema import leer  # noqa: E402
from lib_diagramas import Diagrama, escribir  # noqa: E402

# Las dos tablas del Modulo 7 no estan en el DDL porque el modulo esta pendiente.
# Se declaran aca para que el modelo muestre el esquema completo del sistema, y se
# dibujan con otro relleno para que se distinga lo construido de lo proyectado.
PENDIENTES = {
    'preferencias_notificacion': [
        ('id_preferencia', 'PK'), ('tipo_alerta', 'CA'), ('activa', ''),
        ('destinatario', ''),
    ],
    'alertas': [
        ('id_alerta', 'PK'), ('tipo_alerta', ''), ('fecha_generacion', ''),
        ('mensaje', ''), ('enviada', ''), ('id_preferencia', 'FK'),
        ('id_animal', 'FK'), ('id_insumo', 'FK'),
    ],
}

FK_PENDIENTES = [
    ('alertas', 'preferencias_notificacion'),
    ('alertas', 'animales'),
    ('alertas', 'insumos'),
]

COLUMNAS = [
    ['razas', 'categorias', 'animales', 'hembras', 'machos'],
    ['insumos', 'movimientos_stock', 'lactancias', 'ordenies_lote',
     'ordenie_lote_animales', 'ordenies_individual'],
    ['celos', 'servicios', 'tactos', 'partos', 'diagnosticos', 'tratamientos'],
    ['planes_sanitarios', 'plan_categorias', 'vacunaciones', 'descornes',
     'configuracion'],
    ['preferencias_notificacion', 'alertas'],
]

ANCHO_TABLA = 215
SEPARACION_X = 265
SEPARACION_Y = 26
X0, Y0 = 30, 56
RELLENO_PENDIENTE = '#e6e8ea'


def main():
    tablas, _ = leer()

    filas_por_tabla = {}
    for nombre, tabla in tablas.items():
        filas_por_tabla[nombre] = [(c.nombre, c.marca) for c in tabla.columnas]
    filas_por_tabla.update(PENDIENTES)

    alto = Y0
    for columna in COLUMNAS:
        y = Y0
        for nombre in columna:
            y += 24 + len(filas_por_tabla[nombre]) * 15 + 6 + SEPARACION_Y
        alto = max(alto, y)

    ancho = X0 * 2 + (len(COLUMNAS) - 1) * SEPARACION_X + ANCHO_TABLA
    d = Diagrama('modelo-entidad-relacion', ancho, alto + 10,
                 'Modelo Entidad-Relación — Sistema de Gestión de Tambo')

    puestas = {}
    for i, columna in enumerate(COLUMNAS):
        x = X0 + i * SEPARACION_X
        y = Y0
        for nombre in columna:
            filas = filas_por_tabla[nombre]
            estilo = {'relleno': RELLENO_PENDIENTE} if nombre in PENDIENTES else {}
            figura = d.tabla(x, y, nombre, filas, ANCHO_TABLA, **estilo)
            puestas[nombre] = figura
            y += figura.alto + SEPARACION_Y

    def unir(hijo, padre):
        a, b = puestas.get(hijo), puestas.get(padre)
        if a is None or b is None or a is b:
            return
        # La linea sale por el costado que mira al destino: con veintitantas tablas
        # es lo unico que mantiene el dibujo leible.
        if b.x > a.x:
            d.unir(a, b, flecha=True, desde='derecha', hasta='izquierda')
        elif b.x < a.x:
            d.unir(a, b, flecha=True, desde='izquierda', hasta='derecha')
        elif b.y > a.y:
            d.unir(a, b, flecha=True, desde='abajo', hasta='arriba')
        else:
            d.unir(a, b, flecha=True, desde='arriba', hasta='abajo')

    for nombre, tabla in tablas.items():
        for _, destino, _, _ in tabla.fks:
            unir(nombre, destino)
    for hijo, padre in FK_PENDIENTES:
        unir(hijo, padre)

    base = escribir(d, AQUI)
    print(os.path.basename(base), '|', len(puestas), 'tablas |', ancho, 'x', alto)


if __name__ == '__main__':
    main()
