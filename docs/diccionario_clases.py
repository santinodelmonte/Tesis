# -*- coding: utf-8 -*-
"""Seccion 2.2.7 Diccionario de Clases, leida de Tesis/Dominio y Tesis/Persistencia.

Las clases de negocio son anemicas: conservan sus atributos y su constructor, y toda
la logica vive en la Controladora. Por eso el diccionario de cada entidad lista sus
atributos, y el de la Controladora sus metodos agrupados por region.
"""
import os
import sys

AQUI = os.path.dirname(os.path.abspath(__file__))
sys.path.insert(0, os.path.join(AQUI, 'diagramas'))

from codigo import dominio, persistencia  # noqa: E402

# Descripcion de cada atributo. Lo que el nombre no dice por si solo.
NOTAS = {
    ('PreferenciaNotificacion', 'TipoAlerta'): 'Cuál de los ocho avisos es. La lista '
        'es cerrada: un tipo nuevo es una funcionalidad nueva del sistema, no un dato '
        'que el usuario agregue.',
    ('PreferenciaNotificacion', 'Activa'): 'Un aviso apagado deja de enviarse por '
        'Telegram y se sigue viendo en el sistema.',
    ('PreferenciaNotificacion', 'Etiqueta'): 'El título con el que el aviso aparece en '
        'la pantalla y encabeza su bloque del mensaje. Derivado del tipo.',
    ('PreferenciaNotificacion', 'Modulo'): 'El módulo del que sale el aviso. Agrupa la '
        'pantalla y ordena el mensaje, que se arma por módulo.',
    ('Alerta', 'Mensaje'): 'El renglón tal como se envió. Se guarda armado para que el '
        'historial no dependa de que el cálculo siga dando lo mismo meses después.',
    ('Alerta', 'FechaGeneracion'): 'El día del resumen que la incluyó. Un pendiente sin '
        'resolver vuelve a generar su alerta al día siguiente.',
    ('Alerta', 'Animal'): 'El animal que la originó. Sin valor en los avisos que nacen '
        'de un insumo.',
    ('Alerta', 'Insumo'): 'El insumo que la originó. Sin valor por el mismo motivo que '
        'Animal.',
    ('Configuracion', 'HoraResumen'): 'Hora a la que sale el resumen diario de '
        'Telegram.',
    ('Configuracion', 'ChatTelegram'): 'Destinatario único de los avisos. Vacío '
        'mientras nadie haya vinculado una cuenta.',
    ('Configuracion', 'FechaUltimoResumen'): 'Día en que salió el último resumen. Es lo '
        'único de la configuración que escribe el sistema y no la encargada.',
    ('Configuracion', 'TelegramVinculado'): 'Derivado: la integración está lista cuando '
        'hay a quién escribirle.',
    ('Animal', 'Activo'): 'Falso cuando el animal está dado de baja.',
    ('Animal', 'FechaBaja'): 'Fecha de salida del rodeo. Sin valor mientras el animal '
        'está activo.',
    ('Animal', 'Foto'): 'Nombre del archivo de la fotografía. La imagen se guarda en '
        'disco, no en la base.',
    ('Animal', 'Madre'): 'Progenitora registrada. Puede no haberla.',
    ('Animal', 'Padre'): 'Progenitor registrado. Puede no haberlo.',
    ('Hembra', 'NumeroPartos'): 'Cantidad de partos registrados. Junto con la edad '
        'determina la categoría.',
    ('Hembra', 'EstadoProductivo'): 'Sin lactancia, En lactancia o Seca. Eje '
        'productivo, independiente del reproductivo.',
    ('Hembra', 'EstadoReproductivo'): 'Vacía, Servida o Preñada. Se deduce del '
        'servicio vigente y de su último tacto.',
    ('Macho', 'EnPie'): 'Falso en el toro de catálogo, que aporta material genético '
        'sin integrar el rodeo.',
    ('Lactancia', 'FechaSecado'): 'Sin valor mientras la lactancia está en curso.',
    ('OrdenieLote', 'LitrosTotales'): 'La leche completa del turno, tal como se lee '
        'del tanque.',
    ('OrdenieIndividual', 'Litros'): 'Producción medida de un animal en un turno. No '
        'se suma al total del lote del mismo turno.',
    ('Servicio', 'Insumo'): 'Pajuela utilizada en la inseminación artificial. '
        'Excluyente con Macho.',
    ('Servicio', 'Macho'): 'Toro de la monta natural. Excluyente con Insumo.',
    ('Tratamiento', 'CantidadInsumo'): 'Cantidad de producto consumida, que es lo que '
        'se devuelve al stock si el tratamiento se corrige o se elimina.',
    ('Tratamiento', 'FechaFinDescarte'): 'Fin del período en que la leche del animal '
        'no se destina a consumo.',
    ('Insumo', 'PeriodoDescarteDias'): 'Días de carencia del producto.',
    ('Insumo', 'Macho'): 'Sólo en las pajuelas: el toro que aporta la dosis.',
    ('MovimientoStock', 'FechaVencimiento'): 'Vencimiento de la partida ingresada.',
    ('PlanSanitario', 'PeriodicidadDias'): 'Sin valor cuando el procedimiento se '
        'aplica una única vez en la vida del animal.',
    ('Diagnostico', 'Estado'): 'Distingue el cuadro activo del cerrado.',
    ('ControlDiario', 'Litros'): 'Un punto de la curva de lactancia, usado para '
        'estimar la producción por intervalos.',
    ('CandidataDescarte', 'Motivos'): 'Los criterios de descarte que el animal cumple, '
        'en texto: el sistema informa y no decide.',
    ('ProcedimientoPendiente', 'FechaProxima'): 'Fecha proyectada del procedimiento, '
        'derivada del plan y de la última aplicación.',
}

# Clases que no tienen tabla: existen para transportar un resultado calculado.
SIN_TABLA = {
    'ControlDiario': 'No tiene tabla: es un punto de la curva de lactancia que el '
        'sistema arma en memoria a partir de los controles registrados.',
    'CandidataDescarte': 'No tiene tabla: reúne un animal con los motivos por los que '
        'aparece como candidata al descarte.',
    'PartidaVencimiento': 'No tiene tabla: es el remanente de una partida, derivado de '
        'los movimientos de stock.',
    'ProcedimientoPendiente': 'No tiene tabla: es un pendiente del calendario '
        'sanitario, derivado de comparar el plan con lo aplicado.',
    'PreferenciaNotificacion': 'Un tipo de aviso y el interruptor que decide si entra '
        'en el resumen diario. Los ocho son los ocho contadores del tablero de inicio.',
    'Alerta': 'Un pendiente concreto de un día concreto: el renglón del mensaje y, una '
        'vez guardada, el registro de que ese aviso salió.',
}


def entidades():
    """Las clases de negocio con sus atributos."""
    salida = []
    for clase in dominio():
        if clase.nombre == 'Controladora':
            continue
        filas = [(nombre, tipo, NOTAS.get((clase.nombre, nombre), ''))
                 for tipo, nombre in clase.propiedades]
        salida.append((clase.nombre, clase.base, SIN_TABLA.get(clase.nombre, ''), filas))
    return salida


def _agrupar_por_region(clase, texto):
    """Reparte los metodos en las regiones del archivo, en orden de aparicion."""
    posiciones = []
    inicio = 0
    for region in clase.regiones:
        i = texto.find('#region ' + region, inicio)
        posiciones.append((i, region))
        inicio = i + 1

    grupos = [(region, []) for _, region in posiciones]
    for tipo, nombre, parametros in clase.metodos:
        i = texto.find(' ' + nombre + '(')
        region = 0
        for j, (pos, _) in enumerate(posiciones):
            if pos != -1 and pos <= i:
                region = j
        grupos[region][1].append((nombre, tipo, parametros))
    return [(r, m) for r, m in grupos if m]


def controladora():
    clase = next(c for c in dominio() if c.nombre == 'Controladora')
    texto = open(os.path.join(AQUI, '..', 'Tesis', 'Dominio', 'Controladora.cs'),
                 encoding='utf-8').read()
    return _agrupar_por_region(clase, texto)


def fachada():
    clase = next(c for c in persistencia() if c.nombre == 'pControladora')
    texto = open(os.path.join(AQUI, '..', 'Tesis', 'Persistencia', 'pControladora.cs'),
                 encoding='utf-8').read()
    return _agrupar_por_region(clase, texto)


def acceso_a_datos():
    return [(c.nombre, [(n, t, p) for t, n, p in c.metodos])
            for c in persistencia()
            if c.nombre not in ('pControladora',)]


def main():
    p = ['# 2.2.7 Diccionario de Clases — v6\n',
         'Generado desde `Tesis/Dominio` y `Tesis/Persistencia` por '
         '`diccionario_clases.py`. No editar a mano.\n',
         '## Clases de negocio\n']

    for nombre, base, nota, filas in entidades():
        p.append('\n### %s%s\n' % (nombre, (' (hereda de %s)' % base) if base else ''))
        if nota:
            p.append(nota + '\n')
        p.append('| Atributo | Tipo | Descripción |')
        p.append('|---|---|---|')
        for fila in filas:
            p.append('| %s | %s | %s |' % fila)

    p.append('\n## Controladora\n')
    for region, metodos in controladora():
        p.append('\n### %s (%d métodos)\n' % (region.title(), len(metodos)))
        p.append('| Método | Devuelve | Parámetros |')
        p.append('|---|---|---|')
        for nombre, tipo, parametros in metodos:
            p.append('| %s | %s | %s |' % (nombre, tipo, parametros or '—'))

    p.append('\n## pControladora\n')
    for region, metodos in fachada():
        p.append('\n### %s (%d métodos)\n' % (region.title(), len(metodos)))
        p.append('| Método | Devuelve | Parámetros |')
        p.append('|---|---|---|')
        for nombre, tipo, parametros in metodos:
            p.append('| %s | %s | %s |' % (nombre, tipo, parametros or '—'))

    p.append('\n## Clases de acceso a datos\n')
    for nombre, metodos in acceso_a_datos():
        p.append('\n### %s\n' % nombre)
        p.append('| Método | Devuelve | Parámetros |')
        p.append('|---|---|---|')
        for m, tipo, parametros in metodos:
            p.append('| %s | %s | %s |' % (m, tipo, parametros or '—'))

    destino = os.path.join(AQUI, 'diccionario-clases-v6.md')
    with open(destino, 'w', encoding='utf-8') as archivo:
        archivo.write('\n'.join(p) + '\n')

    print('escrito %s | %d clases de negocio | %d métodos de Controladora | '
          '%d de pControladora | %d clases de datos'
          % (os.path.basename(destino), len(entidades()),
             sum(len(m) for _, m in controladora()),
             sum(len(m) for _, m in fachada()), len(acceso_a_datos())))


if __name__ == '__main__':
    main()
