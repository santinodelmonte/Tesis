# -*- coding: utf-8 -*-
"""Diagramas de Dominio (2.2.3) y de Persistencia (2.2.4), leidos del codigo."""
import os
import sys

AQUI = os.path.dirname(os.path.abspath(__file__))
sys.path.insert(0, AQUI)

from codigo import dominio, persistencia  # noqa: E402
from lib_diagramas import Diagrama, escribir  # noqa: E402

ANCHO = 210
SEP_X = 262
SEP_Y = 26
X0, Y0 = 30, 56

# Las entidades del dominio, agrupadas por modulo. El orden es el de los modulos del
# sistema, para que el diagrama se lea igual que el resto del documento.
COLUMNAS_ENTIDADES = [
    ['Raza', 'Categoria', 'Animal', 'Hembra', 'Macho'],
    ['Insumo', 'MovimientoStock', 'PartidaVencimiento', 'Lactancia', 'OrdenieLote',
     'OrdenieIndividual'],
    ['Celo', 'Servicio', 'Tacto', 'Parto', 'ControlDiario', 'CandidataDescarte'],
    ['PlanSanitario', 'ProcedimientoPendiente', 'Diagnostico', 'Tratamiento',
     'Vacunacion', 'Descorne'],
    ['Configuracion'],
]


def _tipo_base(tipo):
    """Devuelve el nombre de clase que hay adentro de List<X> o de X."""
    tipo = tipo.strip()
    if tipo.startswith('List<') and tipo.endswith('>'):
        return tipo[5:-1].strip()
    return tipo


def diagrama_entidades(clases):
    por_nombre = {c.nombre: c for c in clases}
    nombres = {n for columna in COLUMNAS_ENTIDADES for n in columna}

    alto = Y0
    for columna in COLUMNAS_ENTIDADES:
        y = Y0
        for nombre in columna:
            y += 24 + max(1, len(por_nombre[nombre].propiedades)) * 15 + 6 + SEP_Y
        alto = max(alto, y)

    ancho = X0 * 2 + (len(COLUMNAS_ENTIDADES) - 1) * SEP_X + ANCHO
    d = Diagrama('dominio-clases-de-negocio', ancho, alto + 10,
                 'Diagrama de Dominio — clases de negocio')

    puestas = {}
    for i, columna in enumerate(COLUMNAS_ENTIDADES):
        x = X0 + i * SEP_X
        y = Y0
        for nombre in columna:
            clase = por_nombre[nombre]
            filas = [('%s: %s' % (n, t), '') for t, n in clase.propiedades] or [('—', '')]
            figura = d.tabla(x, y, nombre, filas, ANCHO)
            puestas[nombre] = figura
            y += figura.alto + SEP_Y

    def unir(origen, destino, texto=''):
        a, b = puestas.get(origen), puestas.get(destino)
        if a is None or b is None or a is b:
            return
        if b.x > a.x:
            d.unir(a, b, texto, flecha=True, desde='derecha', hasta='izquierda')
        elif b.x < a.x:
            d.unir(a, b, texto, flecha=True, desde='izquierda', hasta='derecha')
        elif b.y > a.y:
            d.unir(a, b, texto, flecha=True, desde='abajo', hasta='arriba')
        else:
            d.unir(a, b, texto, flecha=True, desde='arriba', hasta='abajo')

    # Herencia: Hembra y Macho especializan a Animal.
    for clase in clases:
        if clase.base in nombres:
            unir(clase.nombre, clase.base, 'hereda')

    # Asociaciones: toda propiedad cuyo tipo es otra clase del dominio.
    vistas = set()
    for clase in clases:
        if clase.nombre not in nombres:
            continue
        for tipo, _ in clase.propiedades:
            destino = _tipo_base(tipo)
            if destino in nombres and destino != clase.base:
                par = (clase.nombre, destino)
                if par not in vistas:
                    vistas.add(par)
                    unir(*par)

    return d


def diagrama_controladora(clases):
    controladora = next(c for c in clases if c.nombre == 'Controladora')
    filas = [(r.title(), '') for r in controladora.regiones]

    d = Diagrama('dominio-controladora', 760, 120 + len(filas) * 15 + 90,
                 'Diagrama de Dominio — clase Controladora')

    caja = d.tabla(30, 56, 'Controladora', filas, 330)
    d.caja(430, 70, 300, 74,
           'Listas static en memoria\n(caché del rodeo y de sus eventos)')
    d.caja(430, 175, 300, 60, 'pControladora\n(capa de persistencia)', negrita=True)
    d.caja(430, 268, 300, 92,
           'Clases de negocio\nAnimal, Hembra, Macho, Lactancia,\nServicio, Tratamiento, …')

    cache, persistencia_caja, entidades = d.figuras[1], d.figuras[2], d.figuras[3]
    d.unir(caja, cache, 'valida contra', flecha=True, desde='derecha', hasta='izquierda')
    d.unir(caja, persistencia_caja, 'delega la escritura', flecha=True,
           desde='derecha', hasta='izquierda')
    d.unir(caja, entidades, 'construye', flecha=True, desde='derecha', hasta='izquierda')
    return d


def main():
    clases = dominio()
    for d in (diagrama_entidades(clases), diagrama_controladora(clases)):
        base = escribir(d, AQUI)
        print(os.path.basename(base))

    # El diagrama de persistencia vive en el mismo generador porque comparte el
    # lector de codigo y el criterio de armado.
    clases_p = persistencia()
    facha = next(c for c in clases_p if c.nombre == 'pControladora')
    filas = [(r.title(), '') for r in facha.regiones]
    d = Diagrama('persistencia-fachada', 720, 130 + len(filas) * 15 + 60,
                 'Diagrama de Persistencia — fachada pControladora')
    caja = d.tabla(30, 56, 'pControladora', filas, 320)
    dominio_caja = d.caja(410, 70, 280, 62, 'Controladora\n(única que la invoca)',
                          negrita=True)
    clases_caja = d.caja(410, 165, 280, 78,
                         'Clases de acceso a datos\npAnimal, pHembra, pServicio, …')
    conexion = d.caja(410, 276, 280, 58, 'pConexion\n(MySQL)', negrita=True)
    d.unir(dominio_caja, caja, '', flecha=True, desde='izquierda', hasta='derecha')
    d.unir(caja, clases_caja, 'delega en', flecha=True, desde='derecha',
           hasta='izquierda')
    d.unir(clases_caja, conexion, 'ejecuta', flecha=True, desde='abajo', hasta='arriba')
    print(os.path.basename(escribir(d, AQUI)))

    # Clases de acceso a datos, una por entidad.
    entidades = [c for c in clases_p if c.nombre not in ('pControladora', 'pConexion')]
    porcolumna = 6
    columnas = [entidades[i:i + porcolumna] for i in range(0, len(entidades), porcolumna)]
    alto = Y0
    for columna in columnas:
        y = Y0
        for clase in columna:
            y += 24 + max(1, len(clase.metodos)) * 15 + 6 + SEP_Y
        alto = max(alto, y)
    # pConexion va en una columna propia a la derecha: todas las clases terminan
    # en ella, asi que ponerla dentro de la grilla la tapa con las lineas.
    ancho = X0 * 2 + len(columnas) * SEP_X + 250
    d = Diagrama('persistencia-clases', ancho, alto + 10,
                 'Diagrama de Persistencia — clases de acceso a datos')
    conexion = d.caja(ancho - 240, alto / 2.0 - 40, 210, 80,
                      'pConexion — Abrir, Cerrar, EjecutarComando, EjecutarConsulta',
                      negrita=True)
    for i, columna in enumerate(columnas):
        x = X0 + i * SEP_X
        y = Y0
        for clase in columna:
            filas = [('%s()' % n, '') for _, n, _ in clase.metodos] or [('—', '')]
            figura = d.tabla(x, y, clase.nombre, filas, ANCHO)
            d.unir(figura, conexion, punteado=True, flecha=True, desde='derecha',
                   hasta='izquierda')
            y += figura.alto + SEP_Y
    print(os.path.basename(escribir(d, AQUI)))


if __name__ == '__main__':
    main()
