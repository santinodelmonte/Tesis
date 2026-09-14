# -*- coding: utf-8 -*-
"""El conteo de puntos de funcion del sistema, y la cuenta que sale de el.

Los numeros de la seccion 1.11 del anteproyecto no se escriben a mano: se listan aca,
funcion por funcion, y el modulo los suma. Cada lista sale de un artefacto que se puede
abrir y contar —los metodos de escritura de `Tesis/Dominio/Controladora.cs`, las
pantallas de `Tesis/Pages`, las tablas de `bd/CreacionDb.sql`— asi que la cuenta se
puede rehacer y discutir, que es lo unico que le da valor a una estimacion.

`verificar()` vuelve a contar contra el codigo y avisa si algo se movio.
"""
import os
import re

AQUI = os.path.dirname(os.path.abspath(__file__))
RAIZ = os.path.dirname(AQUI)

# --------------------------------------------------------------- el dominio de informacion

# Entradas externas: cada operacion con la que un dato entra al sistema y actualiza un
# archivo logico. Alta, modificacion y baja se cuentan por separado, como pide el metodo.
ENTRADAS = {
    'Altas': ['Animal', 'Celo', 'Descorne', 'Diagnostico', 'Insumo', 'Lactancia',
              'Ordenie individual', 'Ordenie por lote', 'Parto', 'Plan sanitario',
              'Servicio', 'Tacto', 'Tratamiento', 'Vacunacion'],
    'Modificaciones': ['Animal', 'Celo', 'Configuracion del establecimiento', 'Descorne',
                       'Diagnostico', 'Lactancia', 'Preferencias de notificacion',
                       'Ordenie individual', 'Ordenie por lote', 'Parto',
                       'Plan sanitario', 'Servicio', 'Stock minimo', 'Tacto',
                       'Tratamiento', 'Vacunacion'],
    'Eliminaciones': ['Celo', 'Descorne', 'Diagnostico', 'Ordenie individual',
                      'Ordenie por lote', 'Parto', 'Servicio', 'Tacto', 'Tratamiento',
                      'Vacunacion'],
    'Otras': ['Baja de animal', 'Reactivacion de animal', 'Carga de fotografia',
              'Ingreso de stock', 'Registro de secado'],
    'Acceso': ['Inicio de sesion'],
}

# Consultas externas: recuperan informacion y no calculan nada derivado.
CONSULTAS = [
    'Lista de animales', 'Buscar y filtrar animales', 'Ficha integral del animal',
    'Consulta de linaje', 'Busqueda por caravana en la barra superior',
    'Lista de lactancias', 'Historial de produccion', 'Lista de celos',
    'Lista de servicios', 'Lista de tactos', 'Lista de partos',
    'Lista de diagnosticos', 'Lista de tratamientos', 'Lista de vacunaciones',
    'Lista de descornes', 'Lista de planes sanitarios', 'Lista de insumos',
    'Historial de movimientos de stock', 'Vacas para servir', 'Tactos pendientes',
]

# Salidas externas: lo que el sistema devuelve despues de calcular algo.
SALIDAS = {
    'Alertas': ['Secado proximo', 'Parto proximo', 'Stock critico',
                'Vencimiento de insumos', 'Calendario sanitario'],
    'Apoyo a la decision': ['Tablero de inicio', 'Indicadores del rodeo',
                            'Candidatas a descarte', 'Metrica de produccion mensual'],
    'Reportes en pantalla': ['Productivo', 'Sanitario', 'Reproductivo', 'Genetico'],
    'Reportes en PDF': ['Productivo', 'Sanitario', 'Reproductivo', 'Genetico'],
    'Reportes en Excel': ['Productivo', 'Sanitario', 'Reproductivo', 'Genetico'],
    'Notificaciones': ['Resumen diario por Telegram'],
    'Valores calculados': ['Categoria propuesta', 'Verificacion de consanguinidad',
                           'Fin del descarte de leche', 'Numero de lactancia propuesto'],
}

# Archivos logicos internos: agrupamientos de datos, no tablas. Las 24 tablas de la base
# se agrupan en 20 archivos: hembras y machos son la especializacion de animales, y las
# dos tablas de union viven con la entidad que las gobierna.
ARCHIVOS = [
    ('Animales', 'animales, hembras, machos'),
    ('Razas', 'razas'),
    ('Categorias', 'categorias'),
    ('Lactancias', 'lactancias'),
    ('Ordenies por lote', 'ordenies_lote, ordenie_lote_animales'),
    ('Ordenies individuales', 'ordenies_individual'),
    ('Celos', 'celos'),
    ('Servicios', 'servicios'),
    ('Tactos', 'tactos'),
    ('Partos', 'partos'),
    ('Diagnosticos', 'diagnosticos'),
    ('Tratamientos', 'tratamientos'),
    ('Vacunaciones', 'vacunaciones'),
    ('Descornes', 'descornes'),
    ('Planes sanitarios', 'planes_sanitarios, plan_categorias'),
    ('Insumos', 'insumos'),
    ('Movimientos de stock', 'movimientos_stock'),
    ('Configuracion', 'configuracion'),
    ('Preferencias de notificacion', 'preferencias_notificacion'),
    ('Alertas', 'alertas'),
]

INTERFACES = [('API del bot de Telegram',
               'el sistema lee de ella el identificador del chat al vincularlo')]

# --------------------------------------------------------------- ponderacion y ajuste

# (bajo, medio, complejo) y sus factores. La complejidad la asigna el equipo: complejo es
# lo que escribe en varias tablas o propaga consecuencias a otro modulo.
PONDERACION = [
    ('Entradas externas',           (22, 18, 6),  (3, 4, 6)),
    ('Salidas externas',            (5, 13, 8),   (4, 5, 7)),
    ('Consultas externas',          (8, 10, 2),   (3, 4, 6)),
    ('Archivos logicos internos',   (12, 6, 2),   (7, 10, 15)),
    ('Interfaces externas',         (1, 0, 0),    (5, 7, 10)),
]

FACTORES = [
    ('Respaldo y recuperacion confiables', 4,
     'La informacion del rodeo es irrecuperable si se pierde: son anios de registros '
     'que no estan en ningun otro lado. Es el riesgo R9.'),
    ('Comunicaciones de datos especializadas', 3,
     'El sistema sale a internet contra la API del bot de Telegram para enviar las '
     'alertas y el resumen diario.'),
    ('Funciones distribuidas de procesamiento', 0,
     'Todo el procesamiento ocurre en el servidor. No hay nada distribuido.'),
    ('Desempenio critico', 2,
     'El requerimiento no funcional pide respuesta en menos de cinco segundos, pero '
     'con una sola usuaria y un rodeo de escala familiar no hay presion real.'),
    ('Entorno con uso pesado de operaciones', 1,
     'Hosting compartido, con los limites que eso impone, pero sin carga propia.'),
    ('Entrada de datos en linea', 5,
     'Toda la carga es en linea: no existe ningun proceso por lotes.'),
    ('Entrada en linea repartida en varias pantallas', 3,
     'El parto, el servicio y el ordenie por lote se completan en mas de un paso y '
     'con seleccion de animales.'),
    ('Los archivos logicos se actualizan en linea', 5,
     'Cada registro impacta en el momento y su efecto se ve en la pantalla siguiente.'),
    ('Entradas, salidas, archivos o consultas complejos', 4,
     'El linaje recorre la genealogia de forma recursiva y la ficha integral reune '
     'seis origenes distintos en una sola pantalla.'),
    ('Procesamiento interno complejo', 4,
     'La categoria, la consanguinidad, el descarte de leche, el estado de las '
     'lactancias y el calendario que arma el plan sanitario son reglas del tambo que '
     'el sistema resuelve solo.'),
    ('Codigo disenado para ser reutilizable', 2,
     'La separacion en tres capas lo permite, pero el dominio es el de este '
     'establecimiento y no se penso para reutilizarlo afuera.'),
    ('Conversion e instalacion contempladas en el disenio', 2,
     'La carga inicial del rodeo esta prevista, pero no hay migracion automatica '
     'desde los cuadernos: es una limitacion declarada.'),
    ('Instalaciones multiples en organizaciones distintas', 0,
     'Un solo establecimiento. Los parametros de manejo son configurables, pero la '
     'configuracion es unica.'),
    ('Disenio pensado para el cambio y la facilidad de uso', 4,
     'Son los requerimientos no funcionales de Usabilidad y Mantenibilidad, con sus '
     'criterios de verificacion.'),
]


# --------------------------------------------------------------- las cuentas

def conteos():
    """Devuelve cuantas funciones hay de cada tipo."""
    return {
        'Entradas externas': sum(len(v) for v in ENTRADAS.values()),
        'Salidas externas': sum(len(v) for v in SALIDAS.values()),
        'Consultas externas': len(CONSULTAS),
        'Archivos logicos internos': len(ARCHIVOS),
        'Interfaces externas': len(INTERFACES),
    }


def conteo_total():
    """La suma ponderada: el conteo total de la formula."""
    filas, total = [], 0
    for nombre, cantidades, factores in PONDERACION:
        subtotal = sum(c * f for c, f in zip(cantidades, factores))
        filas.append((nombre, cantidades, factores, subtotal))
        total += subtotal
    return filas, total


def ajuste():
    return sum(valor for _, valor, _ in FACTORES)


def puntos_de_funcion():
    _, total = conteo_total()
    return total * (0.65 + 0.01 * ajuste())


# --------------------------------------------------------------- verificacion

def verificar():
    """Vuelve a contar contra el codigo y devuelve lo que no coincide."""
    problemas = []

    controladora = os.path.join(RAIZ, 'Tesis', 'Dominio', 'Controladora.cs')
    with open(controladora, 'rb') as f:
        fuente = f.read().decode('utf-8-sig', 'replace')
    # Los metodos de escritura publicos y de instancia. Los static y los que la propia
    # Controladora usa para propagar no son entradas del usuario.
    escritura = set(re.findall(
        r'public\s+(?!static)[\w<>,?\[\] ]+?\s+'
        r'((?:Alta|Modificar|Eliminar|Baja|Reactivar|Registrar|Guardar)\w*)\s*\(',
        fuente))
    internos = {'AltaHembra', 'AltaMacho', 'ModificarEstadoProductivo',
                'ModificarEstadoReproductivo', 'RegistrarEnvioResumen'}
    escritura -= internos
    declaradas = sum(len(v) for v in ENTRADAS.values()) - len(ENTRADAS['Acceso'])
    if len(escritura) != declaradas:
        problemas.append(
            'entradas externas: la lista declara %d y la Controladora expone %d '
            'metodos de escritura (%s)'
            % (declaradas, len(escritura), ', '.join(sorted(escritura))))

    sql = os.path.join(RAIZ, 'bd', 'CreacionDb.sql')
    with open(sql, 'rb') as f:
        tablas = set(re.findall(r'CREATE TABLE(?: IF NOT EXISTS)?\s+`?(\w+)`?',
                                f.read().decode('utf-8-sig', 'replace'), re.I))
    declaradas_tablas = set()
    for _, detalle in ARCHIVOS:
        declaradas_tablas |= {t.strip() for t in detalle.split(',')}
    if tablas and tablas != declaradas_tablas:
        faltan = sorted(tablas - declaradas_tablas)
        sobran = sorted(declaradas_tablas - tablas)
        problemas.append('archivos logicos: en la base y no en la lista %s; '
                         'en la lista y no en la base %s' % (faltan, sobran))
    return problemas


def main():
    filas, total = conteo_total()
    for nombre, cantidad in conteos().items():
        print('%-28s %3d' % (nombre, cantidad))
    print()
    for nombre, cantidades, factores, subtotal in filas:
        print('%-28s bajo %2d x%-2d  medio %2d x%-2d  complejo %2d x%-2d  = %3d'
              % (nombre, cantidades[0], factores[0], cantidades[1], factores[1],
                 cantidades[2], factores[2], subtotal))
    print('\nconteo total = %d' % total)
    print('suma de factores de ajuste = %d' % ajuste())
    print('PF = %d x [0,65 + 0,01 x %d] = %.1f' % (total, ajuste(), puntos_de_funcion()))
    for problema in verificar():
        print('\nOJO — ' + problema)


if __name__ == '__main__':
    main()
