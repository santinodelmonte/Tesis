# -*- coding: utf-8 -*-
"""Arma la seccion 2.2.2 a partir de casos_de_uso_parte1.py y parte2.py.

Sin argumentos escribe casos-de-uso-v6.md, la version legible para revisar.
El generador del .docx importa CARGAR() desde aca para no duplicar el orden ni el
formato de los campos.
"""
import importlib.util
import os

AQUI = os.path.dirname(os.path.abspath(__file__))

CAMPOS = [
    ('Nombre del CU', 'nombre'),
    ('Actores', 'actores'),
    ('Tipo', 'tipo'),
    ('Descripción', 'descripcion'),
    ('Referencia a Requerimientos Funcionales', 'rf'),
    ('Pre-condición', 'precondicion'),
    ('Desencadenante', 'desencadenante'),
]

CAMPOS_CIERRE = [
    ('Cursos Alternativos', 'alternativos'),
    ('Cursos de Excepción', 'excepcion'),
    ('Post-condición', 'postcondicion'),
    ('Reglas de Negocio', 'reglas'),
    ('Validaciones', 'validaciones'),
    ('Frecuencia de Uso', 'frecuencia'),
]


def _cargar_modulo(nombre):
    ruta = os.path.join(AQUI, nombre + '.py')
    spec = importlib.util.spec_from_file_location(nombre, ruta)
    modulo = importlib.util.module_from_spec(spec)
    spec.loader.exec_module(modulo)
    return modulo


def cargar():
    """Devuelve (casos, modulos) con los casos ordenados por numero."""
    parte1 = _cargar_modulo('casos_de_uso_parte1')
    parte2 = _cargar_modulo('casos_de_uso_parte2')
    casos = parte1.CASOS + parte2.CASOS
    casos.sort(key=lambda c: c['num'])
    return casos, parte1.MODULOS


def lineas(caso):
    """Devuelve el caso como la lista de parrafos que van al documento."""
    salida = ['CU %d' % caso['num']]
    for etiqueta, clave in CAMPOS:
        salida.append('%s: %s' % (etiqueta, caso[clave]))
    salida.append('Curso Básico:')
    for i, paso in enumerate(caso['curso'], start=1):
        salida.append('%d. %s' % (i, paso))
    for etiqueta, clave in CAMPOS_CIERRE:
        salida.append('%s: %s' % (etiqueta, caso[clave]))
    return salida


def main():
    casos, modulos = cargar()

    partes = ['# 2.2.2 Casos de uso — v6\n',
              'Generado desde `casos_de_uso_parte1.py` y `casos_de_uso_parte2.py`. '
              'No editar a mano: editar los datos y volver a generar.\n']

    partes.append('## Listado\n')
    modulo_actual = None
    for caso in casos:
        if caso['modulo'] != modulo_actual:
            modulo_actual = caso['modulo']
            partes.append('\n**%s**\n' % modulos[modulo_actual])
        partes.append('- CU %d — %s' % (caso['num'], caso['nombre']))

    partes.append('\n---\n')
    modulo_actual = None
    for caso in casos:
        if caso['modulo'] != modulo_actual:
            modulo_actual = caso['modulo']
            partes.append('\n## %s\n' % modulos[modulo_actual])
        partes.append('### CU %d — %s\n' % (caso['num'], caso['nombre']))
        for linea in lineas(caso)[1:]:
            partes.append(linea + '\n')

    destino = os.path.join(AQUI, 'casos-de-uso-v6.md')
    with open(destino, 'w', encoding='utf-8') as archivo:
        archivo.write('\n'.join(partes))
    print('escrito', destino, '|', len(casos), 'casos de uso')


if __name__ == '__main__':
    main()
