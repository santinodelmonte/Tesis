# -*- coding: utf-8 -*-
"""Lee bd/tambo.sql y devuelve el esquema como estructura de datos.

El modelo entidad-relacion, la tabla de claves y las restricciones de integridad
del documento salen todas de aca. Derivarlas del DDL en lugar de transcribirlas a
mano es lo que evita que el documento y la base se separen: si cambia una columna,
cambia el documento al volver a generar.
"""
import os
import re

RAIZ = os.path.dirname(os.path.dirname(os.path.dirname(os.path.abspath(__file__))))
SQL = os.path.join(RAIZ, 'bd', 'tambo.sql')


class Columna:
    def __init__(self, nombre, tipo, nulo):
        self.nombre = nombre
        self.tipo = tipo
        self.nulo = nulo
        self.pk = False
        self.unica = False
        self.fk = None  # (tabla, columna)

    @property
    def marca(self):
        if self.pk and self.fk:
            return 'PK, FK'
        if self.pk:
            return 'PK'
        if self.fk:
            return 'FK'
        if self.unica:
            return 'CA'
        return ''

    def __repr__(self):
        return '<%s %s>' % (self.nombre, self.marca)


class Tabla:
    def __init__(self, nombre):
        self.nombre = nombre
        self.columnas = []
        self.pk = []
        self.unicas = []  # lista de listas
        self.fks = []     # (columnas, tabla_destino, columnas_destino, en_borrado)

    def columna(self, nombre):
        for c in self.columnas:
            if c.nombre == nombre:
                return c
        return None


def _limpiar(texto):
    """Saca los comentarios de linea, que confunden a las expresiones regulares."""
    return re.sub(r'--[^\n]*', '', texto)


def leer(ruta=SQL):
    texto = _limpiar(open(ruta, encoding='utf-8').read())
    tablas = {}
    orden = []

    for m in re.finditer(r'CREATE TABLE\s+(?:IF NOT EXISTS\s+)?(\w+)\s*\((.*?)\n\)\s*ENGINE',
                         texto, re.S):
        tabla = Tabla(m.group(1))
        tablas[tabla.nombre] = tabla
        orden.append(tabla.nombre)

        for linea in m.group(2).split('\n'):
            linea = linea.strip().rstrip(',').strip()
            if not linea:
                continue

            pk = re.match(r'PRIMARY KEY\s*\((.*?)\)', linea, re.I)
            if pk:
                tabla.pk = [c.strip() for c in pk.group(1).split(',')]
                continue

            uni = re.match(r'UNIQUE\s*\((.*?)\)', linea, re.I)
            if uni:
                tabla.unicas.append([c.strip() for c in uni.group(1).split(',')])
                continue

            fk = re.match(r'FOREIGN KEY\s*\((.*?)\)\s*REFERENCES\s+(\w+)\s*\((.*?)\)(.*)',
                          linea, re.I)
            if fk:
                tabla.fks.append(([c.strip() for c in fk.group(1).split(',')],
                                  fk.group(2),
                                  [c.strip() for c in fk.group(3).split(',')],
                                  'CASCADE' if 'CASCADE' in fk.group(4).upper() else ''))
                continue

            col = re.match(r'(\w+)\s+([A-Z]+(?:\(\s*[\d,\s]+\))?)\s*(.*)', linea, re.I)
            if col and col.group(1).upper() not in ('PRIMARY', 'UNIQUE', 'FOREIGN', 'KEY'):
                resto = col.group(3).upper()
                tabla.columnas.append(
                    Columna(col.group(1), col.group(2).upper(),
                            'NOT NULL' not in resto))

    # Las claves foraneas recursivas de animales se agregan con ALTER TABLE.
    for m in re.finditer(r'ALTER TABLE\s+(\w+)\s+ADD FOREIGN KEY\s*\((.*?)\)\s*'
                         r'REFERENCES\s+(\w+)\s*\((.*?)\)', texto, re.I | re.S):
        tabla = tablas.get(m.group(1))
        if tabla:
            tabla.fks.append(([m.group(2).strip()], m.group(3),
                              [m.group(4).strip()], ''))

    for tabla in tablas.values():
        for nombre in tabla.pk:
            columna = tabla.columna(nombre)
            if columna:
                columna.pk = True
        for grupo in tabla.unicas:
            for nombre in grupo:
                columna = tabla.columna(nombre)
                if columna:
                    columna.unica = True
        for columnas, destino, columnas_destino, _ in tabla.fks:
            for nombre in columnas:
                columna = tabla.columna(nombre)
                if columna:
                    columna.fk = (destino, columnas_destino[0])

    return tablas, orden


if __name__ == '__main__':
    tablas, orden = leer()
    print('%d tablas' % len(orden))
    for nombre in orden:
        t = tablas[nombre]
        print('%-24s %2d columnas | PK %-28s | %d FK' %
              (nombre, len(t.columnas), ','.join(t.pk), len(t.fks)))
