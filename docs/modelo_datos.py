# -*- coding: utf-8 -*-
"""Secciones 2.2.5.2, 2.2.5.3 y 2.2.5.4 del Proyecto, derivadas de bd/CreacionDb.sql.

La normalizacion, la tabla de claves y las restricciones de integridad se arman
leyendo el DDL. Lo unico escrito a mano son las observaciones: el resto -tipos,
nulos, claves- sale del esquema, que es lo que evita que el documento describa
columnas que la base ya no tiene.
"""
import os
import sys

AQUI = os.path.dirname(os.path.abspath(__file__))
sys.path.insert(0, os.path.join(AQUI, 'diagramas'))

from esquema import leer  # noqa: E402

# Observaciones por columna. Solo estan las que dicen algo que el DDL no dice.
NOTAS = {
    ('animales', 'activo'): 'Marca la baja lógica. La baja nunca elimina la fila: '
        'el historial y el linaje de los descendientes dependen de ella.',
    ('animales', 'foto'): 'Nombre del archivo dentro de wwwroot. La imagen no se '
        'guarda en la base.',
    ('animales', 'id_madre'): 'Clave foránea recursiva hacia hembras. Admite nulo '
        'cuando el progenitor no está registrado, que es el caso de la carga inicial '
        'del rodeo.',
    ('animales', 'id_padre'): 'Clave foránea recursiva hacia machos. Admite nulo por '
        'el mismo motivo que id_madre.',
    ('animales', 'id_categoria'): 'Valor propuesto por el sistema a partir del sexo, '
        'la edad y la cantidad de partos, ajustable por el usuario.',
    ('hembras', 'id_animal'): 'Comparte la clave primaria con animales: así se '
        'resuelve la especialización.',
    ('machos', 'id_animal'): 'Comparte la clave primaria con animales: así se '
        'resuelve la especialización.',
    ('machos', 'en_pie'): 'Vale falso en el toro de catálogo, que aporta material '
        'genético sin integrar el rodeo.',
    ('insumos', 'stock_actual'): 'Resulta de la suma de los movimientos registrados.',
    ('insumos', 'periodo_descarte_dias'): 'Días de carencia del producto. Nulo en los '
        'insumos que no obligan a descartar leche.',
    ('insumos', 'id_macho'): 'Sólo en las pajuelas: vincula la dosis con el toro que '
        'la aporta, y es lo que permite reconstruir la genealogía de la cría.',
    ('movimientos_stock', 'fecha_vencimiento'): 'El vencimiento se registra por '
        'partida y no por insumo: un mismo insumo ingresa en partidas con '
        'vencimientos distintos.',
    ('lactancias', 'fecha_secado'): 'Nulo mientras la lactancia está en curso: ese '
        'nulo es lo que la identifica como abierta.',
    ('ordenies_lote', 'litros_totales'): 'La leche completa del turno, tal como se lee '
        'del tanque, incluida la de los animales que además se midieron uno por uno.',
    ('ordenies_individual', 'id_ordenie_lote'): 'Admite nulo: el control individual no '
        'exige que el total del turno esté cargado. Cuando falta, la producción de ese '
        'turno es la suma de los controles.',
    ('ordenie_lote_animales', 'id_animal'): 'Deja asentado qué animales integraron el '
        'lote del turno, que es lo que el usuario ajusta en el paso 4 de CU12.',
    ('servicios', 'id_macho'): 'Toro de la monta natural. Mutuamente excluyente con '
        'id_insumo.',
    ('servicios', 'id_insumo'): 'Pajuela de la inseminación artificial. Mutuamente '
        'excluyente con id_macho. La exclusión se controla en la Controladora, porque '
        'MySQL no admite un CHECK sobre columnas de otras tablas.',
    ('servicios', 'fecha_probable_parto'): 'Calculada a partir de la fecha del '
        'servicio y ajustable por el usuario.',
    ('tactos', 'id_servicio'): 'El tacto cuelga del servicio y no de la hembra: es el '
        'servicio lo que viene a confirmar.',
    ('planes_sanitarios', 'periodicidad_dias'): 'Nulo indica que el procedimiento se '
        'aplica una única vez en la vida del animal, como el descorne.',
    ('planes_sanitarios', 'id_insumo'): 'Nulo en los planes de descorne, que no '
        'consumen insumo.',
    ('plan_categorias', 'id_categoria'): 'Un plan sin filas en esta tabla alcanza a '
        'todo el rodeo: la ausencia de fila es información, no un dato faltante.',
    ('diagnosticos', 'estado'): 'Distingue el cuadro activo del cerrado (CU33).',
    ('tratamientos', 'id_diagnostico'): 'Nulo identifica al tratamiento preventivo, '
        'como la desparasitación, que no se origina en un diagnóstico.',
    ('tratamientos', 'id_animal'): 'Permite atribuir el tratamiento preventivo a un '
        'animal, sin lo cual no generaría descarte de leche para nadie.',
    ('tratamientos', 'cantidad_insumo'): 'Cuánto producto consumió la aplicación. Es '
        'lo que permite devolver la cantidad exacta al stock si el tratamiento se '
        'corrige o se elimina.',
    ('tratamientos', 'fecha_fin_descarte'): 'Calculada sumando los días de duración y '
        'el período de carencia del insumo, y ajustable por el usuario.',
    ('vacunaciones', 'id_plan'): 'Declara explícitamente qué plan da por cumplido la '
        'aplicación: el calendario no lo infiere del insumo, porque dos planes pueden '
        'usar la misma vacuna.',
    ('descornes', 'id_plan'): 'Ídem vacunaciones. El descorne es de aplicación única.',
    ('configuracion', 'id_configuracion'): 'Tabla de una sola fila: el sistema lee '
        'siempre la primera y escribe sobre ella. No hay alta de configuraciones.',
    ('configuracion', 'hora_resumen'): 'Hora a la que sale el resumen diario de '
        'Telegram. Es un parámetro de manejo como los demás: hay una sola para todo el '
        'establecimiento.',
    ('configuracion', 'chat_telegram'): 'Destinatario único de los avisos. Nulo '
        'mientras nadie haya vinculado una cuenta, y ese nulo es lo que el sistema lee '
        'como integración sin configurar. El token del bot no está en la base: es una '
        'credencial y vive en la configuración de la aplicación.',
    ('configuracion', 'fecha_ultimo_resumen'): 'Día en que salió el último resumen. '
        'Evita el mensaje repetido cuando el sitio se reinicia: la tabla de alertas no '
        'puede responder por un día sin pendientes, porque ese día no genera filas.',
    ('preferencias_notificacion', 'tipo_alerta'): 'Los ocho tipos se cargan con el '
        'esquema y el sistema no da de alta ninguno: son los ocho contadores del '
        'tablero de inicio, y que la lista sea cerrada es lo que garantiza que el '
        'resumen y las pantallas no puedan discrepar.',
    ('preferencias_notificacion', 'activa'): 'Un aviso apagado deja de enviarse por '
        'Telegram y se sigue viendo en el sistema.',
    ('alertas', 'fecha_generacion'): 'Un pendiente que no se resuelve genera su fila '
        'otra vez al día siguiente: el resumen es la lista de tareas del día, no un '
        'aviso de novedades.',
    ('alertas', 'mensaje'): 'El renglón tal como se envió. Se guarda armado para que '
        'el historial no dependa de que el cálculo siga dando lo mismo meses después.',
    ('alertas', 'id_animal'): 'Nulo según el tipo de alerta: unas se originan en un '
        'animal y otras en un insumo.',
    ('alertas', 'id_insumo'): 'Nulo por el mismo motivo que id_animal.',
}

ORDEN = [
    'razas', 'categorias', 'animales', 'hembras', 'machos',
    'insumos', 'movimientos_stock',
    'lactancias', 'ordenies_lote', 'ordenie_lote_animales', 'ordenies_individual',
    'celos', 'servicios', 'tactos', 'partos',
    'planes_sanitarios', 'plan_categorias', 'diagnosticos', 'tratamientos',
    'vacunaciones', 'descornes',
    'configuracion',
    'preferencias_notificacion', 'alertas',
]


def _restricciones(columna, tabla):
    partes = []
    if columna.pk:
        partes.append('PK')
        if columna.fk:
            partes.append('FK → %s' % columna.fk[0])
        partes.append('Auto increment' if len(tabla.pk) == 1 else 'Clave compuesta')
    elif columna.fk:
        partes.append('FK → %s' % columna.fk[0])
    if columna.unica and not columna.pk:
        partes.append('Único')
    partes.append('Nulo' if columna.nulo else 'No nulo')
    return ', '.join(partes)


def normalizacion():
    """Las relaciones del esquema, con la clave primaria primero."""
    tablas, _ = leer()
    salida = []
    for nombre in ORDEN:
        salida.append((nombre, [c.nombre for c in tablas[nombre].columnas]))
    return salida


def claves():
    """Filas de la tabla 2.2.5.3: primaria, alternas y foraneas de cada tabla."""
    tablas, _ = leer()
    filas = []
    for nombre in ORDEN:
        tabla = tablas[nombre]
        alternas = ', '.join(', '.join(g) for g in tabla.unicas)
        foraneas = ', '.join(', '.join(cols) for cols, _, _, _ in tabla.fks)
        filas.append((nombre, ', '.join(tabla.pk), alternas or '—', foraneas or '—'))
    return filas


def integridad():
    """Para cada tabla, la lista de (campo, tipo, restricciones, observaciones)."""
    tablas, _ = leer()
    salida = []
    for nombre in ORDEN:
        filas = []
        tabla = tablas[nombre]
        for columna in tabla.columnas:
            filas.append((columna.nombre, columna.tipo,
                          _restricciones(columna, tabla),
                          NOTAS.get((nombre, columna.nombre), '')))
        salida.append((nombre, filas))
    return salida


def main():
    destino = os.path.join(AQUI, 'modelo-datos-v6.md')
    p = ['# 2.2.5 Modelo de Datos — v6\n',
         'Generado desde `bd/CreacionDb.sql` por `modelo_datos.py`. No editar a mano.\n',
         '## 2.2.5.2 Normalización\n']
    for nombre, columnas in normalizacion():
        p.append('- **%s** = {%s}' % (nombre, ', '.join(columnas)))

    p.append('\n## 2.2.5.3 Tabla de Claves\n')
    p.append('| Tabla | Claves primarias | Claves alternas | Claves foráneas |')
    p.append('|---|---|---|---|')
    for fila in claves():
        p.append('| %s | %s | %s | %s |' % fila)

    p.append('\n## 2.2.5.4 Restricciones de Integridad\n')
    for nombre, filas in integridad():
        p.append('\n### Tabla: %s\n' % nombre)
        p.append('| Campo | Tipo | Restricciones | Observaciones |')
        p.append('|---|---|---|---|')
        for fila in filas:
            p.append('| %s | %s | %s | %s |' % fila)

    with open(destino, 'w', encoding='utf-8') as archivo:
        archivo.write('\n'.join(p) + '\n')
    total = sum(len(f) for _, f in integridad())
    print('escrito', os.path.basename(destino), '|', len(ORDEN), 'tablas |', total, 'campos')


if __name__ == '__main__':
    main()
