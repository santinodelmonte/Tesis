# -*- coding: utf-8 -*-
"""Lee las clases de Tesis/Dominio y Tesis/Persistencia.

Alimenta los diagramas de dominio y de persistencia, y el diccionario de clases.
Mismo criterio que con el esquema de la base: el documento se deriva del codigo, no
se transcribe.
"""
import os
import re

RAIZ = os.path.dirname(os.path.dirname(os.path.dirname(os.path.abspath(__file__))))
DOMINIO = os.path.join(RAIZ, 'Tesis', 'Dominio')
PERSISTENCIA = os.path.join(RAIZ, 'Tesis', 'Persistencia')

# Una propiedad con cuerpo en una linea: public Tipo Nombre { get {...} set {...} }
PROPIEDAD = re.compile(
    r'^\s*public\s+([\w<>,\[\]\?\. ]+?)\s+(\w+)\s*\{\s*get', re.M)

# Firma de metodo publico, con o sin static. La lista de parametros puede ocupar
# varias lineas, que es como estan escritas casi todas las de la Controladora.
METODO = re.compile(
    r'public\s+(?:static\s+)?([\w\.\?]+(?:\s*<[^>{}]*>)?(?:\[\])?)\s+(\w+)\s*'
    r'\(([^)]*)\)\s*\{', re.S)

CLASE = re.compile(r'public\s+class\s+(\w+)(?:\s*:\s*(\w+))?')
REGION = re.compile(r'^\s*#region\s+(.+?)\s*$', re.M)


def _sin_comentarios(texto):
    texto = re.sub(r'/\*.*?\*/', '', texto, flags=re.S)
    return re.sub(r'//[^\n]*', '', texto)


class Clase:
    def __init__(self, nombre, base=None, archivo=''):
        self.nombre = nombre
        self.base = base
        self.archivo = archivo
        self.propiedades = []  # (tipo, nombre)
        self.metodos = []      # (tipo, nombre, parametros)
        self.regiones = []

    @property
    def es_controladora(self):
        return self.nombre in ('Controladora', 'pControladora')


def _leer_carpeta(carpeta):
    clases = []
    for archivo in sorted(os.listdir(carpeta)):
        if not archivo.endswith('.cs'):
            continue
        texto = _sin_comentarios(open(os.path.join(carpeta, archivo), encoding='utf-8').read())
        m = CLASE.search(texto)
        if not m:
            continue
        clase = Clase(m.group(1), m.group(2), archivo)
        clase.propiedades = [(t.strip(), n) for t, n in PROPIEDAD.findall(texto)]
        nombres_propiedad = {n for _, n in clase.propiedades}
        for tipo, nombre, parametros in METODO.findall(texto):
            if nombre in nombres_propiedad or tipo.strip() in ('class', 'const'):
                continue
            clase.metodos.append((tipo.strip(), nombre,
                                  ' '.join(parametros.split())))
        clase.regiones = REGION.findall(texto)
        clases.append(clase)
    return clases


def dominio():
    return _leer_carpeta(DOMINIO)


def persistencia():
    return _leer_carpeta(PERSISTENCIA)


if __name__ == '__main__':
    for etiqueta, clases in (('DOMINIO', dominio()), ('PERSISTENCIA', persistencia())):
        print('==', etiqueta, '--', len(clases), 'clases')
        for c in clases:
            print('  %-22s %-10s %2d propiedades %3d metodos %s'
                  % (c.nombre, (': ' + c.base) if c.base else '',
                     len(c.propiedades), len(c.metodos),
                     ('| %d regiones' % len(c.regiones)) if c.regiones else ''))
