# -*- coding: utf-8 -*-
"""Recorre el sistema y guarda las 112 capturas en docs/capturas/.

Lo corre el autor en su maquina, con el sistema levantado desde Visual Studio y el
rodeo de `bd/DatosPrueba.sql` recien cargado. El contenedor remoto no tiene ni dotnet
ni MySQL, asi que este script se escribe aca y se ejecuta alla.

    pip install playwright
    python -m playwright install chromium
    python docs/sacar_capturas.py --base http://localhost:5000

Que hace y que no
-----------------
Muchas pantallas del sistema se llegan por URL y ya vienen con datos: esas las saca
solo. Otras son el resultado de una accion -un parto registrado, un filtro aplicado,
una alerta que se vacio al reponer- y **no hay URL que las alcance**, porque los
filtros y las altas van por POST. Para esas el script abre la pantalla, escribe en la
terminal que hay que hacer y espera a que se haga; recien ahi dispara.

Es a proposito. Un script que fingiera cargar los cuarenta formularios seria mas
fragil que util: cualquier cambio de un nombre de campo lo rompe, y el que lo corre no
se entera hasta que abre el .docx y ve una pantalla en blanco. Asi, el script se ocupa
de lo que hace mal una persona -el tamano de ventana, el nombre del archivo, el orden,
no saltearse ninguna- y la persona se ocupa de lo que hace mal un script.

Dos capturas no las saca nadie: son fotos del telefono. Las nombra al final.

Como se usa
-----------
De una sola pasada y en orden: el sistema se va modificando a medida que se cargan
cosas y el orden de esta lista lo tiene en cuenta -la alerta de stock antes de
reponer, el efecto del parto despues del parto-.

Se puede cortar y seguir despues: **una captura que ya existe no se vuelve a sacar**,
salvo que se la pida con `--rehacer`. Para repetir una sola:

    python docs/sacar_capturas.py --rehacer m3-cu24-parto,t-parto-efecto

Opciones: `--base` la direccion del sistema, `--solo` un prefijo (`m5`, `t-`, `mov`)
para trabajar por partes, `--listar` para ver que falta sin abrir el navegador.
"""
from __future__ import print_function

import argparse
import os
import sys

AQUI = os.path.dirname(os.path.abspath(__file__))
CAPTURAS = os.path.join(AQUI, 'capturas')

USUARIO = 'sofia'
CONTRASENA = 'tambo2026'

ESCRITORIO = (1280, 800)
MOVIL = (375, 812)

# --------------------------------------------------------------------------- el guion
#
# Cada captura es (nombre, ruta, modo, detalle). El modo dice quien la saca:
#
#   'auto'     la pantalla se alcanza por URL y ya trae datos: la saca el script.
#   'pausa'    hace falta cargar, filtrar o confirmar algo primero: el script abre la
#              pantalla, muestra el detalle y espera Enter.
#   'telefono' foto del celular: el script no la puede sacar.
#
# En 'auto', `detalle` puede ser un selector CSS: entonces fotografia solo ese pedazo,
# que es lo que el guion llama *detalle*. En 'pausa' es la instruccion que se muestra.

GUION = [
    # -- Modulo 0 ---------------------------------------------------------------
    ('m0-cu01-login', '/PagesSeguridad/Login', 'auto', None),
    ('m0-cu02-sesion', '/', 'auto', '.barra-superior'),
    ('m0-cu03-configuracion', '/PagesConfiguracion/Configuracion', 'auto', None),
    ('m0-cu03-efecto', '/PagesConfiguracion/Configuracion', 'pausa',
     'Subi «Parto proximo» de 15 a 30 dias y guarda. Despues abri Reproduccion → '
     'Alertas de Parto: tiene que aparecer tambien la 140.'),
    ('t-configuracion-efecto', '/PagesReproduccion/AlertasParto', 'pausa',
     'La misma pantalla de alertas con la ventana ampliada, como evidencia de 2.3.'),

    # -- Modulo 1 ---------------------------------------------------------------
    ('m1-cu10-lista', '/PagesAnimal/ListaAnimales', 'auto', None),
    ('m1-cu10-filtros', '/PagesAnimal/BuscarAnimales', 'pausa',
     'Categoria «Vaca» + Estado «En lactancia» y aplica los filtros.'),
    ('m1-cu04-alta', '/PagesAnimal/AltaAnimal', 'pausa',
     'Carga la caravana 200: vaca comprada, con dos partos registrados. Todavia no '
     'guardes.'),
    ('m1-cu04-categoria', '/PagesAnimal/AltaAnimal', 'pausa',
     'Con los datos de la 200 cargados, apreta «Calcular Categoria»: propone Vaca. '
     'Guarda el animal despues de la foto, que la 200 hace falta mas adelante.'),
    ('m1-cu04-genealogia', '/PagesAnimal/AltaAnimal', 'pausa',
     'Alta de la caravana 201 con madre 152 y padre 7HO12165: tiene que advertir el '
     'parentesco y ofrecer «Guardar de todos modos». No la guardes.'),
    ('t-alta-categoria', '/PagesAnimal/AltaAnimal', 'pausa',
     'La misma categoria propuesta, como evidencia de 2.3.'),
    ('t-alta-genealogia', '/PagesAnimal/AltaAnimal', 'pausa',
     'La misma advertencia de parentesco, como evidencia de 2.3.'),
    ('m1-cu05-foto', '/PagesAnimal/ListaAnimales', 'pausa',
     'Entra a modificar un animal y carga una fotografia.'),
    ('m1-cu11-ficha', '/PagesAnimal/DetalleAnimal?caravana=115', 'auto', None),
    ('t-ficha-integral', '/PagesAnimal/DetalleAnimal?caravana=115', 'auto', None),
    ('m1-cu08-linaje', '/PagesAnimal/ConsultaLinaje?caravana=152', 'auto', None),
    ('t-linaje-arbol', '/PagesAnimal/ConsultaLinaje?caravana=152', 'auto', None),
    ('m1-cu09-consanguinidad', '/PagesAnimal/VerificarConsanguinidad', 'pausa',
     'Verifica 152 contra 7HO12165: es su padre.'),
    ('t-consanguinidad', '/PagesAnimal/VerificarConsanguinidad', 'pausa',
     'El mismo parentesco detectado, como evidencia de 2.3.'),
    ('m1-cu06-baja', '/PagesAnimal/ListaAnimales', 'pausa',
     'Da de baja la 160 con motivo «Venta», hasta la pantalla de confirmacion.'),
    ('m1-cu07-reactivar', '/PagesAnimal/ListaAnimales', 'pausa',
     'Filtra los dados de baja, donde se ve la accion de reactivar.'),

    # -- Modulo 2 ---------------------------------------------------------------
    ('m2-cu12-lote', '/PagesProduccion/OrdenieLote', 'pausa',
     'El lote con los animales tildados y 88.10 litros. Todavia no guardes.'),
    ('m2-cu12-descarte', '/PagesProduccion/OrdenieLote', 'pausa',
     'DETALLE: la 115 sin tildar y sin poder tildarse, por el descarte de leche '
     'vigente. Sacala ahora: el rodeo se ancla a la fecha de carga y el descarte se '
     'vence con los dias.'),
    ('t-ordenie-descarte', '/PagesProduccion/OrdenieLote', 'pausa',
     'La misma vaca fuera del lote, como evidencia de 2.3. Guarda el ordenie.'),
    ('m2-cu13-masiva', '/PagesProduccion/ControlLechero', 'pausa',
     'Los siete litros cargados vaca por vaca.'),
    ('m2-cu13-puntual', '/PagesProduccion/OrdenieIndividual', 'pausa',
     'Carga de una sola vaca, la 130.'),
    ('m2-cu13-seca', '/PagesProduccion/OrdenieIndividual', 'pausa',
     'Intenta cargar un control a la 136, que esta seca: tiene que rechazarlo.'),
    ('t-control-seca', '/PagesProduccion/OrdenieIndividual', 'pausa',
     'El mismo rechazo, como evidencia de 2.3.'),
    ('m2-cu13-maximo', '/PagesProduccion/OrdenieIndividual', 'pausa',
     'Litros por encima del maximo configurado: el mensaje trae el tope.'),
    ('m2-cu14-historial', '/PagesProduccion/HistorialProduccion', 'pausa',
     'Busca el rango del 01/07 a hoy, por lote.'),
    ('m2-cu15-metrica', '/PagesProduccion/MetricaMensual', 'pausa',
     'Consulta el mes en curso: total, promedio por ordenie y por vaca.'),
    ('m2-cu16-secado', '/PagesProduccion/RegistrarSecado', 'pausa',
     'Registra el secado de la 102.'),
    ('m2-cu18-lactancia', '/PagesProduccion/AltaLactancia', 'pausa',
     'Abri una lactancia para la 200 y apreta «Proponer numero»: sugiere 3.'),
    ('t-lactancia-manual', '/PagesProduccion/AltaLactancia', 'pausa',
     'El mismo numero propuesto, como evidencia de 2.3.'),
    ('m2-cu19-corregir', '/PagesProduccion/HistorialProduccion', 'pausa',
     'Corregi un control: fecha, turno y caravana se ven pero estan bloqueados.'),
    ('t-correccion-control', '/PagesProduccion/HistorialProduccion', 'pausa',
     'Los mismos campos bloqueados, como evidencia de 2.3.'),
    ('m2-cu19-eliminar', '/PagesProduccion/HistorialProduccion', 'pausa',
     'La confirmacion de borrado, que nombra caravana, fecha, turno y litros.'),
    ('t-eliminar-lote', '/PagesProduccion/HistorialProduccion', 'pausa',
     'Borra un ordenie por lote y mostra que los controles individuales sobreviven.'),
    ('m2-lactancias', '/PagesProduccion/ListaLactancias', 'auto', None),

    # -- Modulo 3 ---------------------------------------------------------------
    ('m3-cu20-celo', '/PagesReproduccion/RegistrarCelo', 'pausa',
     'Registra el celo de la 115.'),
    ('m3-cu20-edad', '/PagesReproduccion/RegistrarCelo', 'pausa',
     'Intenta un celo de la ternera 177, de cuatro meses: esta por debajo de la edad '
     'minima de deteccion, que son 9 meses fijos.'),
    ('m3-cu25-servir', '/PagesReproduccion/VacasParaServir', 'auto', None),
    ('m3-cu21-ia', '/PagesReproduccion/RegistrarServicio', 'pausa',
     'Insemina la 115 y mira la fecha probable de parto recalculada.'),
    ('t-servicio-ia', '/PagesReproduccion/RegistrarServicio', 'pausa',
     'La FPP calculada y el stock de la pajuela ya descontado, como evidencia de 2.3.'),
    ('m3-cu21-monta', '/PagesReproduccion/RegistrarServicio', 'pausa',
     'Monta natural de la vaquillona 158: al cambiar el tipo, el selector de pajuela '
     'se esconde y aparece el de toro.'),
    ('m3-cu21-consanguineo', '/PagesReproduccion/RegistrarServicio', 'pausa',
     'Un servicio consanguineo: advierte y ofrece «Registrar de todos modos».'),
    ('t-servicio-consanguineo', '/PagesReproduccion/RegistrarServicio', 'pausa',
     'La misma advertencia, como evidencia de 2.3.'),
    ('m3-cu22-pendientes', '/PagesReproduccion/TactosPendientes', 'auto', None),
    ('m3-cu22-tacto', '/PagesReproduccion/RegistrarTacto', 'pausa',
     'Abri «Ver servicio» de la 102: el servicio con su toro y el parto proyectado.'),
    ('t-tacto', '/PagesReproduccion/RegistrarTacto', 'pausa',
     'El mismo «Ver servicio» antes de confirmar, como evidencia de 2.3. Despues '
     'confirma las prenieces: la pantalla de alertas de secado las necesita.'),
    ('m2-cu17-alertas', '/PagesProduccion/AlertasSecado', 'auto', None),
    ('m3-cu23-alertas', '/PagesReproduccion/AlertasParto', 'auto', None),
    ('m3-cu24-parto', '/PagesReproduccion/RegistrarParto', 'pausa',
     'Parto de la 136: el cartel del servicio y la cria 180 con el padre propuesto. '
     'Todavia no guardes.'),
    ('t-parto', '/PagesReproduccion/RegistrarParto', 'pausa',
     'El mismo formulario, como evidencia de 2.3. Guarda el parto.'),
    ('m3-cu24-efecto', '/PagesAnimal/DetalleAnimal?caravana=136', 'auto', None),
    ('t-parto-efecto', '/PagesAnimal/DetalleAnimal?caravana=136', 'auto', None),
    ('m3-cu24-cria', '/PagesAnimal/ConsultaLinaje?caravana=180', 'auto', None),
    ('t-parto-linaje', '/PagesAnimal/ConsultaLinaje?caravana=180', 'auto', None),
    ('t-parto-doble', '/PagesReproduccion/RegistrarParto', 'pausa',
     'Un parto de mellizos: una sola lactancia para dos crias, con el aviso de '
     'freemartin.'),
    ('m3-cu26-corregir', '/PagesReproduccion/ListaPartos', 'pausa',
     'Entra a modificar un parto.'),
    ('m3-listas', '/PagesReproduccion/ListaServicios', 'auto', None),

    # -- Modulo 4 ---------------------------------------------------------------
    ('m4-cu27-diagnostico', '/PagesSanidad/RegistrarDiagnostico', 'pausa',
     'Dermatitis digital de la 108.'),
    ('m4-cu28-tratamiento', '/PagesSanidad/RegistrarTratamiento', 'pausa',
     'Trata a la 108 y apreta «Calcular»: el descarte sale de inicio + duracion + '
     'carencia del producto.'),
    ('t-tratamiento-descarte', '/PagesSanidad/RegistrarTratamiento', 'pausa',
     'El mismo descarte calculado, como evidencia de 2.3. Guarda el tratamiento.'),
    ('m4-cu28-ficha', '/PagesAnimal/DetalleAnimal?caravana=108', 'auto',
     '.ficha-sanitaria'),
    ('t-tratamiento-ordenie', '/PagesProduccion/OrdenieLote', 'pausa',
     'El animal recien tratado, ausente del lote de ordenie: evidencia de 2.3.'),
    ('m4-cu28-preventivo', '/PagesSanidad/RegistrarTratamiento', 'pausa',
     'Tratamiento preventivo sobre la 158, sin diagnostico, con el plan '
     '«Desparasitacion».'),
    ('m4-cu31-calendario', '/PagesSanidad/CalendarioSanitario', 'auto', None),
    ('m4-cu29-vacunacion', '/PagesSanidad/RegistrarVacunacion', 'pausa',
     'Aplica brucelosis a la 177.'),
    ('t-vacunacion', '/PagesInsumo/ListaInsumos', 'pausa',
     'El stock descontado por la vacunacion, como evidencia de 2.3.'),
    ('m4-cu31-despues', '/PagesSanidad/CalendarioSanitario', 'auto', None),
    ('m4-cu32-descorne', '/PagesSanidad/RegistrarDescorne', 'pausa',
     'Descorne de la 178 con pasta caustica.'),
    ('m4-cu30-plan', '/PagesSanidad/ListaPlanes', 'pausa',
     'Abri el plan «Vacuna clostridial»: periodicidad, edad de inicio y categorias '
     'alcanzadas. Guardalo para que pueble el calendario.'),
    ('m4-cu30-efecto', '/PagesSanidad/CalendarioSanitario', 'auto', None),
    ('t-plan-calendario', '/PagesSanidad/CalendarioSanitario', 'auto', None),
    ('m4-cu33-cerrar', '/PagesSanidad/ListaDiagnosticos', 'pausa',
     'Cerra el diagnostico de la 115.'),
    ('m4-listas', '/PagesSanidad/ListaTratamientos', 'auto', None),

    # -- Modulo 5 ---------------------------------------------------------------
    ('m5-cu37-critico', '/PagesInsumo/AlertasStock', 'auto', None),
    ('t-stock-antes', '/PagesInsumo/AlertasStock', 'auto', None),
    ('m5-cu35-alta', '/PagesInsumo/AltaInsumo', 'pausa',
     'Alta de cefquinoma intramamaria, con carencia de 5 dias.'),
    ('m5-cu35-ingreso', '/PagesInsumo/IngresoStock', 'pausa',
     'Ivermectina +10, motivo «Compra a veterinaria La Rural». Guarda: esto vacia la '
     'alerta de stock, que ya quedo fotografiada.'),
    ('m5-cu36-minimo', '/PagesInsumo/ConfigurarStockMinimo', 'pausa',
     'Pone la pajuela 29HO18296 con minimo 2.'),
    ('m5-cu37-resuelto', '/PagesInsumo/AlertasStock', 'auto', None),
    ('t-stock-despues', '/PagesInsumo/AlertasStock', 'auto', None),
    ('m5-cu38-vencimiento', '/PagesInsumo/AlertasVencimiento', 'auto', None),
    ('m5-cu39-movimientos', '/PagesInsumo/HistorialMovimientos', 'pausa',
     'Filtra por oxitetraciclina: el ingreso y los egresos por tratamiento.'),
    ('t-movimientos', '/PagesInsumo/HistorialMovimientos', 'pausa',
     'El mismo ingreso y sus egresos con el motivo, como evidencia de 2.3.'),
    ('m5-insumos', '/PagesInsumo/ListaInsumos', 'auto', None),

    # -- Modulo 6 ---------------------------------------------------------------
    ('m6-cu40-tablero', '/', 'auto', None),
    ('t-tablero', '/', 'auto', None),
    ('m6-cu40-registro', '/', 'pausa',
     'DETALLE: carga un celo desde el registro rapido a una caravana sugerida en '
     '«Para servir». La foto es despues de guardar: la confirmacion arriba y el campo '
     'de caravana vacio para el siguiente.'),
    ('t-registro-rapido', '/', 'pausa',
     'Carga un segundo celo desde el tablero: la confirmacion con el campo ya vacio, '
     'como evidencia de 2.3.'),
    ('m6-cu41-indicadores', '/PagesIndicadores/Indicadores', 'auto', None),
    ('t-indicadores', '/PagesIndicadores/Indicadores', 'auto', None),
    ('m6-cu42-descarte', '/PagesIndicadores/CandidatasDescarte', 'auto', None),
    ('t-descarte', '/PagesIndicadores/CandidatasDescarte', 'auto', None),
    ('m6-cu43-buscar', '/', 'pausa',
     'DETALLE: escribi una caravana en el buscador de la barra superior, sin enviar.'),

    # -- Modulo 7 ---------------------------------------------------------------
    ('m7-reportes', '/PagesReportes/ReporteProductivo', 'pausa',
     'Genera el reporte productivo: la tabla en pantalla con los botones de PDF y '
     'Excel a la vista.'),
    ('t-reporte', '/PagesReportes/ReporteProductivo', 'pausa',
     'El reporte en pantalla junto al PDF y la planilla abiertos, con las mismas '
     'filas: evidencia de 2.3.'),
    ('m7-configuracion-bot', '/PagesNotificaciones/Notificaciones', 'pausa',
     'Vinculada a un chat, con la hora del resumen y los ocho avisos por modulo, uno '
     'de ellos apagado.'),
    ('t-telegram-vinculado', '/PagesNotificaciones/Notificaciones', 'pausa',
     'La pantalla despues de vincular, con el mensaje de prueba ya recibido.'),
    ('m7-resumen-telegram', None, 'telefono',
     'Foto del celular: el resumen diario recibido por Telegram.'),
    ('t-telegram-resumen', None, 'telefono',
     'Foto del celular: el resumen al lado del tablero, con los mismos numeros.'),

    # -- Acceso, al final: cierra la sesion ---------------------------------------
    ('t-acceso-directo', '/PagesAnimal/ListaAnimales', 'pausa',
     'Cerra la sesion y despues escribi esta direccion en el navegador: tiene que '
     'llevarte al login. La foto es la del login con la direccion pedida en la barra.'),
    ('t-acceso-atras', '/PagesSeguridad/Login', 'pausa',
     'Con la sesion cerrada, apreta el boton atras del navegador: tiene que seguir '
     'exigiendo el inicio de sesion.'),

    # -- El celular, 375 x 812 ------------------------------------------------------
    ('mov-menu', '/', 'pausa', 'Abri el menu colapsado.'),
    ('mov-lista-animales', '/PagesAnimal/ListaAnimales', 'auto', None),
    ('mov-ficha', '/PagesAnimal/DetalleAnimal?caravana=115', 'auto', None),
    ('mov-ordenie', '/PagesProduccion/OrdenieLote', 'auto', None),
    ('mov-celo', '/PagesReproduccion/RegistrarCelo', 'auto', None),
    ('mov-alertas', '/PagesInsumo/AlertasStock', 'auto', None),
    ('mov-linaje', '/PagesAnimal/ConsultaLinaje?caravana=152', 'auto', None),
]


# --------------------------------------------------------------------------- el recorrido

def entrar(pagina, base):
    """Inicia sesion. La contrasena nunca queda escrita en una captura."""
    pagina.goto(base + '/PagesSeguridad/Login', wait_until='networkidle')
    if pagina.locator('#usuario').count() == 0:
        return  # la sesion ya estaba abierta
    pagina.fill('#usuario', USUARIO)
    pagina.fill('#contrasena', CONTRASENA)
    pagina.get_by_role('button', name='Ingresar').click()
    pagina.wait_for_load_state('networkidle')


def disparar(pagina, destino, selector=None):
    if selector and pagina.locator(selector).count():
        pagina.locator(selector).first.screenshot(path=destino)
    else:
        pagina.screenshot(path=destino, full_page=True)


def sacar(pagina, base, nombre, ruta, modo, detalle, destino):
    if ruta:
        pagina.goto(base + ruta, wait_until='networkidle')
    if modo == 'pausa':
        print('    %s' % detalle)
        respuesta = input('    [Enter cuando la pantalla este lista, "s" para saltarla] ')
        if respuesta.strip().lower() == 's':
            return False
        disparar(pagina, destino)
    else:
        disparar(pagina, destino, detalle)
    return True


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument('--base', default='http://localhost:5000',
                        help='direccion donde responde el sistema')
    parser.add_argument('--solo', default='',
                        help='prefijo de los nombres a sacar, por ejemplo m5 o t-')
    parser.add_argument('--rehacer', default='',
                        help='nombres separados por coma que hay que volver a sacar')
    parser.add_argument('--listar', action='store_true',
                        help='dice que falta y no abre el navegador')
    args = parser.parse_args()

    if not os.path.isdir(CAPTURAS):
        os.makedirs(CAPTURAS)

    rehacer = {n.strip() for n in args.rehacer.split(',') if n.strip()}
    pendientes, telefono, hechas = [], [], []
    for nombre, ruta, modo, detalle in GUION:
        if args.solo and not nombre.startswith(args.solo):
            continue
        destino = os.path.join(CAPTURAS, nombre + '.png')
        if modo == 'telefono':
            telefono.append((nombre, detalle))
        elif os.path.exists(destino) and nombre not in rehacer:
            hechas.append(nombre)
        else:
            pendientes.append((nombre, ruta, modo, detalle, destino))

    print('%d ya estaban | %d por sacar | %d son fotos del telefono'
          % (len(hechas), len(pendientes), len(telefono)))
    if args.listar:
        for nombre, _, modo, _, _ in pendientes:
            print('  %-26s %s' % (nombre, modo))
        return 0
    if not pendientes:
        print('no queda ninguna por sacar')
        return 0

    from playwright.sync_api import sync_playwright

    con_pausa = sum(1 for p in pendientes if p[2] == 'pausa')
    print('\n%d salen solas; en %d el script te espera. No cierres el navegador.\n'
          % (len(pendientes) - con_pausa, con_pausa))

    salteadas = []
    with sync_playwright() as play:
        navegador = play.chromium.launch(headless=False)
        contexto = navegador.new_context(viewport={'width': ESCRITORIO[0],
                                                   'height': ESCRITORIO[1]})
        pagina = contexto.new_page()

        # El login se fotografia sin sesion; todo lo demas, con ella.
        primera = pendientes[0][0] if pendientes else ''
        if primera != 'm0-cu01-login':
            entrar(pagina, args.base)

        movil = False
        for i, (nombre, ruta, modo, detalle, destino) in enumerate(pendientes, 1):
            if nombre.startswith('mov-') and not movil:
                # El celular es otro contexto: cambiar el viewport a mitad de camino
                # deja la barra de navegacion en el estado del ancho anterior.
                contexto.close()
                contexto = navegador.new_context(viewport={'width': MOVIL[0],
                                                           'height': MOVIL[1]})
                pagina = contexto.new_page()
                entrar(pagina, args.base)
                movil = True
                print('\n-- de aca en adelante, %d x %d --\n' % MOVIL)

            print('[%d/%d] %s' % (i, len(pendientes), nombre))
            if not sacar(pagina, args.base, nombre, ruta, modo, detalle, destino):
                salteadas.append(nombre)
                continue
            if nombre == 'm0-cu01-login':
                entrar(pagina, args.base)

        navegador.close()

    print('\nlisto: %d capturas en docs/capturas/'
          % len([n for n, _, _, _, _ in pendientes if n not in salteadas]))
    if salteadas:
        print('salteadas: ' + ', '.join(salteadas))
    if telefono:
        print('\nFaltan las dos fotos del telefono, que hay que sacar a mano y guardar '
              'en docs/capturas/ con estos nombres:')
        for nombre, detalle in telefono:
            print('  %s.png — %s' % (nombre, detalle))
    print('\nDespues: python docs/verificar_capturas.py '
          'y python docs/editar_proyecto.py')
    return 0


if __name__ == '__main__':
    sys.exit(main())
