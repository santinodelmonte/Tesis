# -*- coding: utf-8 -*-
"""Diagramas de secuencia (seccion 2.2.6), uno por caso de uso.

Los mensajes no se inventan: se leen de la pantalla que resuelve cada caso de uso.
Se extraen las llamadas a la Controladora en el orden en que aparecen en el archivo,
y para cada una se busca en pControladora a que clase de persistencia deriva. Asi el
diagrama muestra el recorrido real entre capas y no una version idealizada.

Los casos del Modulo 7 no tienen pantalla porque el modulo esta pendiente: sus
diagramas se arman con los mensajes previstos, declarados en PREVISTOS.
"""
import os
import re
import sys

AQUI = os.path.dirname(os.path.abspath(__file__))
sys.path.insert(0, AQUI)
sys.path.insert(0, os.path.dirname(AQUI))

from lib_diagramas import Diagrama, escribir  # noqa: E402
from render_casos_de_uso import cargar  # noqa: E402

RAIZ = os.path.dirname(os.path.dirname(AQUI))
PAGINAS = os.path.join(RAIZ, 'Tesis', 'Pages')
FACHADA = os.path.join(RAIZ, 'Tesis', 'Persistencia', 'pControladora.cs')

# Pantalla que resuelve cada caso de uso. Cuando son varias, la primera es la del
# curso basico y las demas aportan sus mensajes a continuacion.
PANTALLAS = {
    1: ['PagesSeguridad/Login'],
    2: ['PagesSeguridad/Login'],
    3: ['PagesConfiguracion/Configuracion'],
    4: ['PagesAnimal/AltaAnimal'],
    5: ['PagesAnimal/ModificarAnimal'],
    6: ['PagesAnimal/BajaAnimal'],
    7: ['PagesAnimal/DetalleAnimal'],
    8: ['PagesAnimal/ConsultaLinaje'],
    9: ['PagesAnimal/VerificarConsanguinidad'],
    10: ['PagesAnimal/BuscarAnimales', 'PagesAnimal/ListaAnimales'],
    11: ['PagesAnimal/DetalleAnimal'],
    12: ['PagesProduccion/OrdenieLote'],
    13: ['PagesProduccion/ControlLechero', 'PagesProduccion/OrdenieIndividual'],
    14: ['PagesProduccion/HistorialProduccion', 'PagesProduccion/ListaLactancias'],
    15: ['PagesProduccion/MetricaMensual'],
    16: ['PagesProduccion/RegistrarSecado'],
    17: ['PagesProduccion/AlertasSecado'],
    18: ['PagesProduccion/AltaLactancia'],
    19: ['PagesProduccion/HistorialProduccion', 'PagesProduccion/ModificarOrdenieLote',
         'PagesProduccion/OrdenieIndividual'],
    20: ['PagesReproduccion/RegistrarCelo'],
    21: ['PagesReproduccion/RegistrarServicio'],
    22: ['PagesReproduccion/RegistrarTacto'],
    23: ['PagesReproduccion/AlertasParto'],
    24: ['PagesReproduccion/RegistrarParto'],
    25: ['PagesReproduccion/TactosPendientes', 'PagesReproduccion/VacasParaServir'],
    26: ['PagesReproduccion/ListaServicios', 'PagesReproduccion/ModificarParto',
         'PagesReproduccion/ListaCelos', 'PagesReproduccion/ListaTactos',
         'PagesReproduccion/ListaPartos'],
    27: ['PagesSanidad/RegistrarDiagnostico'],
    28: ['PagesSanidad/RegistrarTratamiento'],
    29: ['PagesSanidad/RegistrarVacunacion'],
    30: ['PagesSanidad/ListaPlanes', 'PagesSanidad/ConfigurarPlan'],
    31: ['PagesSanidad/CalendarioSanitario'],
    32: ['PagesSanidad/RegistrarDescorne'],
    33: ['PagesSanidad/ListaDiagnosticos'],
    34: ['PagesSanidad/ListaTratamientos', 'PagesSanidad/ListaDiagnosticos',
         'PagesSanidad/ListaVacunaciones', 'PagesSanidad/ListaDescornes'],
    35: ['PagesInsumo/AltaInsumo', 'PagesInsumo/IngresoStock', 'PagesInsumo/ListaInsumos'],
    36: ['PagesInsumo/ConfigurarStockMinimo'],
    37: ['PagesInsumo/AlertasStock'],
    38: ['PagesInsumo/AlertasVencimiento'],
    39: ['PagesInsumo/HistorialMovimientos'],
    40: ['Index'],
    41: ['PagesIndicadores/Indicadores'],
    42: ['PagesIndicadores/CandidatasDescarte'],
    43: ['PagesAnimal/DetalleAnimal'],
}

# Modulo 7: pendiente de implementacion, sin pantalla de la cual leer.
PREVISTOS = {
    44: ['CalcularProduccionEnRango', 'FiltrarOrdeniesLoteXFecha', 'ArmarReporte',
         'DescargarArchivo'],
    45: ['FiltrarTratamientosXAnimal', 'ListarVacunaciones', 'ArmarReporte',
         'DescargarArchivo'],
    46: ['ListarServicios', 'ListarPartos', 'ListarTactos', 'ArmarReporte',
         'DescargarArchivo'],
    47: ['ListarAscendencia', 'ListarDescendencia', 'EstimarProduccionLactancia',
         'ArmarReporte'],
    48: ['ObtenerPreferencias', 'VincularBot', 'ModificarPreferencias',
         'EnviarMensajePrueba'],
    49: ['ObtenerCalendarioSanitario', 'ListarAlertasParto', 'ListarTactosPendientes',
         'ListarAlertasStock', 'ArmarResumen', 'EnviarMensaje'],
}

LLAMADA = re.compile(r'unaControladora\.([A-Za-z]\w*)\s*\(')
DERIVA = re.compile(
    r'public\s+[\w<>,\[\]\?\.]+\s+(\w+)\s*\([^)]*\)\s*\{\s*(?:return\s+)?new\s+(p\w+)\(\)',
    re.S)

MAX_MENSAJES = 7

# Las pantallas empiezan cargando los desplegables del formulario. Esas llamadas son
# reales pero no son el caso de uso: si se quedaran con todo el cupo, el mensaje que
# de verdad lo resuelve -el alta, la baja, el calculo- no entraria en el diagrama.
CARGA = re.compile(r'^(Listar|Buscar|Obtener|Existe)')
MAX_CARGA = 2


def mapa_persistencia():
    texto = open(FACHADA, encoding='utf-8').read()
    return {metodo: clase for metodo, clase in DERIVA.findall(texto)}


def llamadas_de(caso):
    """Metodos de la Controladora que invoca la pantalla del caso de uso."""
    if caso['num'] in PREVISTOS:
        return PREVISTOS[caso['num']]

    vistos = []
    for pagina in PANTALLAS.get(caso['num'], []):
        ruta = os.path.join(PAGINAS, pagina + '.cshtml.cs')
        if not os.path.exists(ruta):
            continue
        texto = re.sub(r'//[^\n]*', '', open(ruta, encoding='utf-8').read())
        for metodo in LLAMADA.findall(texto):
            if metodo not in vistos:
                vistos.append(metodo)
    return vistos


def seleccionar(metodos):
    """Deja las llamadas que cuentan el caso de uso, conservando el orden real."""
    carga = 0
    elegidos = []
    for metodo in metodos:
        if CARGA.match(metodo):
            if carga >= MAX_CARGA:
                continue
            carga += 1
        elegidos.append(metodo)

    if len(elegidos) > MAX_MENSAJES:
        # Con el cupo lleno se recortan las de carga, nunca las que escriben.
        nucleo = [m for m in elegidos if not CARGA.match(m)]
        cabecera = [m for m in elegidos if CARGA.match(m)][:1]
        elegidos = [m for m in elegidos if m in cabecera or m in nucleo][:MAX_MENSAJES]
    return elegidos


def diagrama(caso, mapa):
    metodos = seleccionar(llamadas_de(caso))
    pendiente = caso['num'] in PREVISTOS

    clases = []
    for metodo in metodos:
        clase = mapa.get(metodo)
        if clase and clase not in clases:
            clases.append(clase)

    # Las lineas de vida se arman con un indice por rol, porque no todos los casos
    # tienen las mismas: CU49 no tiene vista y CU48 suma el bot.
    lineas, papel = [], {}

    def linea(nombre, rol):
        papel[rol] = len(lineas)
        lineas.append(nombre)

    if caso['num'] == 49:
        linea('Proceso programado', 'actor')
    else:
        linea('Encargada', 'actor')
        linea(caso['nombre'][:26], 'vista')
    linea('Controladora', 'controladora')
    if clases:
        linea('pControladora', 'fachada')
        linea(clases[0] if len(clases) == 1 else 'pX (entidad)', 'datos')
        linea('pConexion', 'conexion')
    if caso['num'] == 48:
        linea('BotTelegram', 'bot')

    # Los mensajes se calculan antes de dibujar: la altura del lienzo y la de las
    # lineas de vida dependen de cuantos terminen siendo.
    guion = []
    indice = 1
    for metodo in metodos:
        guion.append(('vista', 'controladora', '%d. %s()' % (indice, metodo)))
        indice += 1
        if mapa.get(metodo) and clases:
            for a, b in (('controladora', 'fachada'), ('fachada', 'datos')):
                guion.append((a, b, '%d. %s()' % (indice, metodo)))
                indice += 1
            guion.append(('datos', 'conexion', '%d. EjecutarConsulta()' % indice))
            indice += 1
        else:
            guion.append(('controladora', 'controladora',
                          '%d. valida en memoria' % indice))
            indice += 1
    if 'bot' in papel:
        guion.append(('controladora', 'bot', '%d. Enviar()' % indice))

    ancho_columna = 172
    ancho = 60 + len(lineas) * ancho_columna
    paso = 46
    y0 = 140
    alto = y0 + len(guion) * paso + 60

    d = Diagrama('secuencia-cu%02d' % caso['num'], ancho, alto,
                 'CU%d — %s' % (caso['num'], caso['nombre']))

    cabezas = []
    for i, nombre in enumerate(lineas):
        x = 40 + i * ancho_columna
        caja = d.caja(x, 56, ancho_columna - 28, 46, nombre, negrita=True)
        cabezas.append(caja)
        # La linea de vida se dibuja como una caja angosta y alta, que es la forma
        # de conseguirla con las figuras que ya existen.
        cx = caja.x + caja.ancho / 2.0
        d.caja(cx - 1, 102, 2, alto - 130, '', relleno='#40484f')

    def cabeza(rol):
        if rol == 'vista':
            return cabezas[papel.get('vista', papel['actor'])]
        return cabezas[papel[rol]]

    y = y0
    for desde, hasta, texto in guion:
        d.flecha_mensaje(cabeza(desde), cabeza(hasta), y, texto)
        y += paso

    return d, pendiente


def main():
    casos, _ = cargar()
    mapa = mapa_persistencia()
    salida = os.path.join(AQUI, 'secuencia')
    sin_llamadas = []
    for caso in casos:
        if not llamadas_de(caso):
            sin_llamadas.append(caso['num'])
        d, _ = diagrama(caso, mapa)
        escribir(d, salida)
    print('%d diagramas de secuencia' % len(casos))
    if sin_llamadas:
        print('sin llamadas detectadas:', sin_llamadas)


if __name__ == '__main__':
    main()
