# -*- coding: utf-8 -*-
"""Las acciones que hay que hacer antes de cada captura que no se alcanza por URL.

`sacar_capturas.py` deja en manos de una persona las sesenta y ocho capturas que son el
resultado de una accion: no hay direccion que lleve a «la alerta de stock despues de
reponer». Este modulo automatiza esas acciones contra los nombres reales de los campos,
leidos de `Tesis/Pages` y verificados contra el sistema andando.

**El orden manda.** Las recetas se ejecutan en el orden de `SESION`, porque unas dejan el
sistema en el estado que la siguiente necesita: la alerta de stock se fotografia antes de
reponer, el efecto del parto despues del parto, el calendario sanitario despues de
vacunar. Correrlas salteadas da capturas que se ven bien y dicen lo que no es.

**No reemplaza a la persona, le saca el trabajo repetido.** Si una receta falla, la
captura queda sin sacar y se informa al final: se saca a mano, que es lo que el runbook
de `docs/prompt-capturas.md` explica.

Los identificadores de animales e insumos salen de `bd/DatosPrueba.sql` y se resuelven
por caravana o por nombre, no por numero: el juego de datos se puede recargar y los ids
cambian.
"""
import os


# --------------------------------------------------------------------------- utilidades

def caravana(pag, numero):
    """Carga un animal en el selector, que viaja en un campo oculto.

    El campo `id` de estas pantallas es el del registro que se corrige, no el del
    animal: llenarlo pone el formulario en modo correccion.
    """
    pag.eval_on_selector('#numCaravana', '(e, v) => e.value = v', numero)
    if pag.locator('#caravanaTexto').count():
        pag.eval_on_selector('#caravanaTexto', '(e, v) => e.value = v', numero)


def opcion_que_contenga(pag, selector, texto):
    """Elige la opcion de un desplegable por parte de su texto, sin depender del id."""
    # El texto exacto gana sobre el que solo lo contiene: si no, pedir «Holando»
    # elige «Cruza Holando-Jersey», que aparece antes en la lista.
    valor = pag.eval_on_selector(
        selector,
        '''(sel, t) => {
            // Sin tildes de los dos lados: el sistema escribe «Inseminación
            // artificial» y la receta no tiene por que acordarse del acento.
            const pelar = x => x.normalize('NFD').replace(/[\u0300-\u036f]/g, '')
                                .trim().toLowerCase();
            const b = pelar(t);
            const o = [...sel.options];
            const exacta = o.find(x => pelar(x.text) === b);
            const parcial = o.find(x => pelar(x.text).includes(b));
            const elegida = exacta || parcial;
            return elegida ? elegida.value : null;
        }''', texto)
    if valor is None:
        raise LookupError('no hay opcion con «%s» en %s' % (texto, selector))
    pag.select_option(selector, valor)
    return valor


def boton(pag, texto):
    pag.get_by_role('button', name=texto).first.click()


def hoy(pag):
    return pag.evaluate('() => new Date().toISOString().slice(0, 10)')


def foto_de_prueba(ruta):
    """Un PNG chico y valido, para la captura del campo de fotografia."""
    import base64
    datos = base64.b64decode(
        b'iVBORw0KGgoAAAANSUhEUgAAAEAAAABACAYAAACqaXHeAAAAVklEQVR4nO3QMQ0AAAjAMPybhn'
        b'sIyFqhTdaZWQoAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA'
        b'AAAAAAAAAADAxwGm2gABqDBdkgAAAABJRU5ErkJggg==')
    with open(ruta, 'wb') as f:
        f.write(datos)
    return ruta


IDS = {}
INSUMOS = {}


def insumo(nombre):
    """El id del insumo cuyo texto contiene ese nombre."""
    for texto, valor in INSUMOS.items():
        if nombre.lower() in texto.lower():
            return valor
    raise LookupError('no hay insumo que contenga «%s»' % nombre)


def resolver_ids(pag, base):
    """Mapea caravana -> id y nombre de insumo -> id leyendo las propias pantallas.

    Los ids se resuelven contra el sistema y no se escriben a mano, porque recargar
    `DatosPrueba.sql` los cambia y una receta con numeros fijos empieza a fotografiar
    el animal equivocado sin fallar.
    """
    pag.goto(base + '/PagesAnimal/BuscarAnimales', wait_until='networkidle')
    opcion_que_contenga(pag, '#estado', 'Todos')
    pag.click('#btnFiltrar')
    pag.wait_for_load_state('networkidle')
    IDS.clear()
    IDS.update(pag.evaluate("""() => {
        const m = {};
        for (const fila of document.querySelectorAll('tr')) {
            const enlace = fila.querySelector('a[href*="id="]');
            const celda = fila.querySelector('td');
            if (enlace && celda) {
                const id = new URL(enlace.href, location.origin).searchParams.get('id');
                // La tabla responsive antepone el nombre de la columna a cada celda.
                const texto = celda.textContent.trim().replace(/^Caravana\s*/, '');
                if (id) m[texto] = id;
            }
        }
        return m;
    }"""))

    pag.goto(base + '/PagesInsumo/IngresoStock', wait_until='networkidle')
    INSUMOS.clear()
    INSUMOS.update(pag.evaluate("""() => {
        const s = document.querySelector('[name=idInsumo]');
        const m = {};
        // El texto trae el tipo y el stock detras del nombre; se guarda entero y se
        // busca por contenido, para no depender de como quede armado.
        if (s) for (const o of s.options) if (o.value) m[o.text.trim()] = o.value;
        return m;
    }"""))
    return IDS, INSUMOS


# --------------------------------------------------------------------------- Modulo 0

def m0_configuracion_efecto(pag, base, sacar):
    """Ampliar la ventana de «parto proximo» hace aparecer una vaca mas en la alerta."""
    pag.goto(base + '/PagesConfiguracion/Configuracion', wait_until='networkidle')
    pag.fill('[name=diasAnticipacionParto]', '30')
    boton(pag, 'Guardar')
    pag.goto(base + '/PagesReproduccion/AlertasParto', wait_until='networkidle')
    sacar('m0-cu03-efecto')
    sacar('t-configuracion-efecto')


# --------------------------------------------------------------------------- Modulo 1

def m1_alta(pag, base, sacar):
    """El alta de la 200, con la categoria que el sistema propone."""
    pag.goto(base + '/PagesAnimal/AltaAnimal', wait_until='networkidle')
    pag.fill('[name=numCaravana]', '200')
    pag.fill('[name=fechaNacimiento]', '2021-04-12')
    opcion_que_contenga(pag, '[name=sexo]', 'Hembra')
    opcion_que_contenga(pag, '[name=idRaza]', 'Holando')
    pag.fill('[name=numeroPartos]', '2')
    sacar('m1-cu04-alta')
    boton(pag, 'Calcular Categoria')
    sacar('m1-cu04-categoria')
    sacar('t-alta-categoria')
    boton(pag, 'Agregar')


def m1_genealogia(pag, base, sacar):
    """La 201 con madre y padre emparentados: advierte y ofrece guardar igual."""
    pag.goto(base + '/PagesAnimal/AltaAnimal', wait_until='networkidle')
    pag.fill('[name=numCaravana]', '201')
    pag.fill('[name=fechaNacimiento]', hoy(pag))
    opcion_que_contenga(pag, '[name=sexo]', 'Hembra')
    opcion_que_contenga(pag, '[name=idRaza]', 'Holando')
    pag.eval_on_selector('[name=idMadre]', '(e, v) => e.value = v', IDS['152'])
    pag.eval_on_selector('[name=idPadre]', '(e, v) => e.value = v', IDS['7HO12165'])
    boton(pag, 'Agregar')
    sacar('m1-cu04-genealogia')
    sacar('t-alta-genealogia')


def m1_foto(pag, base, sacar):
    ruta = foto_de_prueba('/tmp/foto-animal.png')
    pag.goto(base + '/PagesAnimal/ModificarAnimal?id=%s' % IDS['115'],
             wait_until='networkidle')
    pag.set_input_files('input[type=file]', ruta)
    sacar('m1-cu05-foto')


def m1_baja(pag, base, sacar):
    pag.goto(base + '/PagesAnimal/BajaAnimal?id=%s' % IDS['160'], wait_until='networkidle')
    opcion_que_contenga(pag, 'select', 'Venta')
    sacar('m1-cu06-baja')


def m1_filtros(pag, base, sacar):
    pag.goto(base + '/PagesAnimal/BuscarAnimales', wait_until='networkidle')
    opcion_que_contenga(pag, '#idCategoria', 'Vaca')
    opcion_que_contenga(pag, '#estado', 'Activos')
    pag.click('#btnFiltrar')
    sacar('m1-cu10-filtros')


def m1_reactivar(pag, base, sacar):
    pag.goto(base + '/PagesAnimal/BuscarAnimales', wait_until='networkidle')
    opcion_que_contenga(pag, '#estado', 'Inactivos')
    pag.click('#btnFiltrar')
    sacar('m1-cu07-reactivar')


def m1_consanguinidad(pag, base, sacar):
    for nombre in ('m1-cu09-consanguinidad', 't-consanguinidad'):
        pag.goto(base + '/PagesAnimal/VerificarConsanguinidad', wait_until='networkidle')
        pag.eval_on_selector('#idHembra', '(e, v) => e.value = v', IDS['152'])
        pag.eval_on_selector('#idMacho', '(e, v) => e.value = v', IDS['7HO12165'])
        boton(pag, 'Verificar')
        sacar(nombre)


# --------------------------------------------------------------------------- Modulo 2

def m2_ordenie_lote(pag, base, sacar):
    """El ordenie del turno. La 115 no se puede tildar: tiene descarte vigente."""
    pag.goto(base + '/PagesProduccion/OrdenieLote', wait_until='networkidle')
    opcion_que_contenga(pag, '[name=turno]', 'Turno 1')
    pag.fill('[name=litrosTotales]', '88.10')
    sacar('m2-cu12-lote')
    sacar('m2-cu12-descarte')
    sacar('t-ordenie-descarte')
    boton(pag, 'Guardar')


def m2_control_masivo(pag, base, sacar):
    pag.goto(base + '/PagesProduccion/ControlLechero', wait_until='networkidle')
    pag.eval_on_selector_all(
        'input[type=number]',
        "(campos) => campos.forEach((c, i) => { c.value = (18 + i % 6) + '.5'; })")
    sacar('m2-cu13-masiva')


def m2_control_puntual(pag, base, sacar):
    pag.goto(base + '/PagesProduccion/OrdenieIndividual', wait_until='networkidle')
    caravana(pag, '130')
    opcion_que_contenga(pag, '[name=turno]', 'Turno 1')
    pag.fill('[name=litros]', '19.4')
    sacar('m2-cu13-puntual')


def m2_control_seca(pag, base, sacar):
    """La 136 esta seca: el sistema rechaza el control."""
    for nombre in ('m2-cu13-seca', 't-control-seca'):
        pag.goto(base + '/PagesProduccion/OrdenieIndividual', wait_until='networkidle')
        caravana(pag, '136')
        opcion_que_contenga(pag, '[name=turno]', 'Turno 1')
        pag.fill('[name=litros]', '15')
        boton(pag, 'Guardar')
        sacar(nombre)


def m2_control_maximo(pag, base, sacar):
    pag.goto(base + '/PagesProduccion/OrdenieIndividual', wait_until='networkidle')
    caravana(pag, '130')
    opcion_que_contenga(pag, '[name=turno]', 'Turno 2')
    pag.fill('[name=litros]', '95')
    boton(pag, 'Guardar')
    sacar('m2-cu13-maximo')


def m2_historial(pag, base, sacar):
    pag.goto(base + '/PagesProduccion/HistorialProduccion', wait_until='networkidle')
    pag.fill('[name=fechaDesde]', '2026-07-01')
    pag.fill('[name=fechaHasta]', hoy(pag))
    boton(pag, 'Buscar')
    sacar('m2-cu14-historial')
    sacar('m2-cu19-corregir')
    sacar('t-correccion-control')
    sacar('m2-cu19-eliminar')
    sacar('t-eliminar-lote')


def m2_metrica(pag, base, sacar):
    pag.goto(base + '/PagesProduccion/MetricaMensual', wait_until='networkidle')
    boton(pag, 'Consultar')
    sacar('m2-cu15-metrica')


def m2_secado(pag, base, sacar):
    pag.goto(base + '/PagesProduccion/RegistrarSecado', wait_until='networkidle')
    caravana(pag, '102')
    pag.fill('[name=fechaSecado]', hoy(pag))
    sacar('m2-cu16-secado')


def m2_lactancia(pag, base, sacar):
    pag.goto(base + '/PagesProduccion/AltaLactancia', wait_until='networkidle')
    caravana(pag, '200')
    boton(pag, 'Proponer')
    sacar('m2-cu18-lactancia')
    sacar('t-lactancia-manual')


# --------------------------------------------------------------------------- Modulo 3

def m3_celo(pag, base, sacar):
    """El celo de una vaca en servicio, y el de una ternera que no tiene edad."""
    pag.goto(base + '/PagesReproduccion/RegistrarCelo', wait_until='networkidle')
    caravana(pag, '115')
    sacar('m3-cu20-celo')
    pag.goto(base + '/PagesReproduccion/RegistrarCelo', wait_until='networkidle')
    caravana(pag, '177')
    boton(pag, 'Guardar')
    sacar('m3-cu20-edad')


def m3_servicio_ia(pag, base, sacar):
    pag.goto(base + '/PagesReproduccion/RegistrarServicio', wait_until='networkidle')
    caravana(pag, '115')
    opcion_que_contenga(pag, '[name=tipoServicio]', 'Inseminacion artificial')
    opcion_que_contenga(pag, '[name=idPajuela]', '7HO12165')
    pag.fill('[name=fechaServicio]', hoy(pag))
    boton(pag, 'Recalcular')
    sacar('m3-cu21-ia')
    boton(pag, 'Guardar')
    sacar('t-servicio-ia')


def m3_servicio_monta(pag, base, sacar):
    pag.goto(base + '/PagesReproduccion/RegistrarServicio', wait_until='networkidle')
    caravana(pag, '158')
    opcion_que_contenga(pag, '[name=tipoServicio]', 'Monta')
    sacar('m3-cu21-monta')


def m3_servicio_consanguineo(pag, base, sacar):
    for nombre in ('m3-cu21-consanguineo', 't-servicio-consanguineo'):
        pag.goto(base + '/PagesReproduccion/RegistrarServicio', wait_until='networkidle')
        caravana(pag, '152')
        # 7HO12165 es reproductor de catalogo: aporta pajuelas y no integra el rodeo,
        # asi que el servicio consanguineo se registra por inseminacion, no por monta.
        opcion_que_contenga(pag, '[name=tipoServicio]', 'Inseminacion artificial')
        opcion_que_contenga(pag, '[name=idPajuela]', '7HO12165')
        pag.fill('[name=fechaServicio]', hoy(pag))
        boton(pag, 'Guardar')
        sacar(nombre)


def m3_tacto(pag, base, sacar):
    """Ver servicio muestra el parto proyectado; despues se confirma la preniez."""
    pag.goto(base + '/PagesReproduccion/RegistrarTacto', wait_until='networkidle')
    caravana(pag, '102')
    boton(pag, 'Ver servicio')
    sacar('m3-cu22-tacto')
    sacar('t-tacto')
    opcion_que_contenga(pag, '[name=resultado]', 'Preñada')
    pag.fill('[name=fechaTacto]', hoy(pag))
    boton(pag, 'Guardar')


def m3_alertas_despues_del_tacto(pag, base, sacar):
    """Las alertas de secado se pueblan recien cuando hay prenieces confirmadas."""
    pag.goto(base + '/PagesProduccion/AlertasSecado', wait_until='networkidle')
    sacar('m2-cu17-alertas')
    pag.goto(base + '/PagesReproduccion/AlertasParto', wait_until='networkidle')
    sacar('m3-cu23-alertas')


def m3_parto(pag, base, sacar):
    pag.goto(base + '/PagesReproduccion/RegistrarParto', wait_until='networkidle')
    caravana(pag, '136')
    boton(pag, 'Cargar datos')
    pag.fill('[name=fechaParto]', hoy(pag))
    pag.fill('[name=caravanaCria]', '180')
    opcion_que_contenga(pag, '[name=sexoCria]', 'Hembra')
    opcion_que_contenga(pag, '[name=idRazaCria]', 'Holando')
    sacar('m3-cu24-parto')
    sacar('t-parto')
    boton(pag, 'Confirmar Parto')


def m3_efecto_del_parto(pag, base, sacar):
    pag.goto(base + '/PagesAnimal/DetalleAnimal?caravana=136', wait_until='networkidle')
    sacar('m3-cu24-efecto')
    sacar('t-parto-efecto')
    pag.goto(base + '/PagesAnimal/ConsultaLinaje?caravana=180', wait_until='networkidle')
    sacar('m3-cu24-cria')
    sacar('t-parto-linaje')


def m3_corregir_parto(pag, base, sacar):
    pag.goto(base + '/PagesReproduccion/ListaPartos', wait_until='networkidle')
    sacar('m3-cu26-corregir')


# --------------------------------------------------------------------------- Modulo 4

def m4_diagnostico(pag, base, sacar):
    pag.goto(base + '/PagesSanidad/RegistrarDiagnostico', wait_until='networkidle')
    caravana(pag, '108')
    pag.fill('[name=enfermedad]', 'Dermatitis digital')
    pag.fill('[name=fechaDiagnostico]', hoy(pag))
    opcion_que_contenga(pag, '[name=estado]', 'En tratamiento')
    sacar('m4-cu27-diagnostico')
    boton(pag, 'Guardar')


def m4_tratamiento(pag, base, sacar):
    """El descarte de leche sale de inicio + duracion + carencia del producto."""
    pag.goto(base + '/PagesSanidad/RegistrarTratamiento', wait_until='networkidle')
    caravana(pag, '108')
    opcion_que_contenga(pag, '[name=idDiagnostico]', 'Dermatitis digital')
    opcion_que_contenga(pag, '[name=idInsumo]', 'Oxitetraciclina')
    pag.fill('[name=fechaInicio]', hoy(pag))
    pag.fill('[name=diasDuracion]', '3')
    pag.fill('[name=dosisDiaria]', '1')
    boton(pag, 'Calcular')
    sacar('m4-cu28-tratamiento')
    sacar('t-tratamiento-descarte')
    boton(pag, 'Guardar')
    pag.goto(base + '/PagesAnimal/DetalleAnimal?caravana=108', wait_until='networkidle')
    sacar('m4-cu28-ficha')
    pag.goto(base + '/PagesProduccion/OrdenieLote', wait_until='networkidle')
    sacar('t-tratamiento-ordenie')


def m4_preventivo(pag, base, sacar):
    pag.goto(base + '/PagesSanidad/RegistrarTratamiento', wait_until='networkidle')
    caravana(pag, '158')
    opcion_que_contenga(pag, '[name=idDiagnostico]', 'Sin diagnostico')
    opcion_que_contenga(pag, '[name=idInsumo]', 'Ivermectina')
    opcion_que_contenga(pag, '[name=idPlan]', 'Desparasitacion')
    pag.fill('[name=fechaInicio]', hoy(pag))
    pag.fill('[name=diasDuracion]', '1')
    pag.fill('[name=dosisDiaria]', '1')
    sacar('m4-cu28-preventivo')


def m4_vacunacion(pag, base, sacar):
    """Vacunar saca al animal del calendario y descuenta la dosis del stock."""
    pag.goto(base + '/PagesSanidad/RegistrarVacunacion', wait_until='networkidle')
    caravana(pag, '177')
    opcion_que_contenga(pag, '[name=idInsumo]', 'Brucelosis')
    opcion_que_contenga(pag, '[name=idPlan]', 'Brucelosis')
    pag.fill('[name=fechaAplicacion]', hoy(pag))
    sacar('m4-cu29-vacunacion')
    boton(pag, 'Guardar')
    pag.goto(base + '/PagesInsumo/ListaInsumos', wait_until='networkidle')
    sacar('t-vacunacion')
    pag.goto(base + '/PagesSanidad/CalendarioSanitario', wait_until='networkidle')
    sacar('m4-cu31-despues')


def m4_descorne(pag, base, sacar):
    pag.goto(base + '/PagesSanidad/RegistrarDescorne', wait_until='networkidle')
    caravana(pag, '178')
    opcion_que_contenga(pag, '[name=metodo]', 'Pasta caustica')
    opcion_que_contenga(pag, '[name=idPlan]', 'Descorne')
    pag.fill('[name=fecha]', hoy(pag))
    sacar('m4-cu32-descorne')


def m4_plan(pag, base, sacar):
    """El plan reprograma el calendario sin cargar animal por animal."""
    pag.goto(base + '/PagesSanidad/ListaPlanes', wait_until='networkidle')
    enlace = pag.locator('a[href*="ConfigurarPlan"]').first
    enlace.click()
    sacar('m4-cu30-plan')
    boton(pag, 'Guardar')
    pag.goto(base + '/PagesSanidad/CalendarioSanitario', wait_until='networkidle')
    sacar('m4-cu30-efecto')
    sacar('t-plan-calendario')


def m4_cerrar(pag, base, sacar):
    pag.goto(base + '/PagesSanidad/ListaDiagnosticos', wait_until='networkidle')
    sacar('m4-cu33-cerrar')


# --------------------------------------------------------------------------- Modulo 5

def m5_alta_insumo(pag, base, sacar):
    pag.goto(base + '/PagesInsumo/AltaInsumo', wait_until='networkidle')
    pag.fill('[name=nombre]', 'Cefquinoma intramamaria (jeringa)')
    opcion_que_contenga(pag, '[name=tipoInsumo]', 'Medicamento')
    pag.fill('[name=cantidadInicial]', '12')
    pag.fill('[name=stockMinimo]', '4')
    pag.fill('[name=periodoDescarteDias]', '5')
    sacar('m5-cu35-alta')


def m5_ingreso(pag, base, sacar):
    """Reponer vacia la alerta: por eso el «antes» ya se fotografio."""
    pag.goto(base + '/PagesInsumo/IngresoStock', wait_until='networkidle')
    opcion_que_contenga(pag, '[name=idInsumo]', 'Ivermectina')
    pag.fill('[name=cantidad]', '10')
    pag.fill('[name=fecha]', hoy(pag))
    pag.fill('[name=motivo]', 'Compra a veterinaria La Rural')
    sacar('m5-cu35-ingreso')
    boton(pag, 'Registrar Ingreso')


def m5_minimo(pag, base, sacar):
    pag.goto(base + '/PagesInsumo/ConfigurarStockMinimo', wait_until='networkidle')
    opcion_que_contenga(pag, '[name=idInsumo]', '29HO18296')
    pag.fill('[name=stockMinimo]', '2')
    sacar('m5-cu36-minimo')
    boton(pag, 'Guardar')
    pag.goto(base + '/PagesInsumo/AlertasStock', wait_until='networkidle')
    sacar('m5-cu37-resuelto')
    sacar('t-stock-despues')


def m5_movimientos(pag, base, sacar):
    pag.goto(base + '/PagesInsumo/HistorialMovimientos', wait_until='networkidle')
    opcion_que_contenga(pag, '[name=idInsumo]', 'Oxitetraciclina')
    pag.get_by_role('button', name='Buscar').last.click()
    sacar('m5-cu39-movimientos')
    sacar('t-movimientos')


# --------------------------------------------------------------------------- Modulo 6

def m6_registro_rapido(pag, base, sacar):
    """El tablero registra el celo sin pasar por el menu, y deja el campo listo."""
    for nombre, vaca in (('m6-cu40-registro', '133'), ('t-registro-rapido', '130')):
        pag.goto(base + '/', wait_until='networkidle')
        pag.fill('#caravanaRapida, [name=caravanaRapida], #numCaravana', vaca)
        boton(pag, 'Registrar celo')
        sacar(nombre)


def m6_buscar(pag, base, sacar):
    pag.goto(base + '/', wait_until='networkidle')
    pag.fill('.buscador-caravana', '115')
    sacar('m6-cu43-buscar')


# --------------------------------------------------------------------------- Modulo 7

def m7_reportes(pag, base, sacar):
    pag.goto(base + '/PagesReportes/ReporteProductivo', wait_until='networkidle')
    boton(pag, 'Ver en pantalla')
    sacar('m7-reportes')
    sacar('t-reporte')


# --------------------------------------------------------------------- acceso, al final

def acceso(pag, base, sacar):
    """Cierra la sesion: va ultimo porque deja el sistema sin sesion abierta."""
    pag.goto(base + '/', wait_until='networkidle')
    boton(pag, 'Cerrar sesión')
    pag.wait_for_load_state('networkidle')
    pag.goto(base + '/PagesAnimal/ListaAnimales', wait_until='networkidle')
    sacar('t-acceso-directo')
    pag.go_back()
    pag.wait_for_load_state('networkidle')
    sacar('t-acceso-atras')


def t_parto_doble(pag, base, sacar):
    """Un parto de mellizos: una sola lactancia para dos crias."""
    pag.goto(base + '/PagesReproduccion/AlertasParto', wait_until='networkidle')
    fila = pag.locator('table tbody tr td').first
    proxima = fila.inner_text().strip().replace('Caravana', '').strip()
    pag.goto(base + '/PagesReproduccion/RegistrarParto', wait_until='networkidle')
    caravana(pag, proxima)
    boton(pag, 'Cargar datos')
    pag.fill('[name=fechaParto]', hoy(pag))
    pag.check('[name=partoDoble]')
    pag.fill('[name=caravanaCria]', '181')
    pag.fill('[name=caravanaCria2]', '182')
    opcion_que_contenga(pag, '[name=sexoCria]', 'Hembra')
    opcion_que_contenga(pag, '[name=sexoCria2]', 'Macho')
    opcion_que_contenga(pag, '[name=idRazaCria]', 'Holando')
    opcion_que_contenga(pag, '[name=idRazaCria2]', 'Holando')
    sacar('t-parto-doble')


def mov_menu(pag, base, sacar):
    """A 375 pixeles el menu esta colapsado detras del boton de hamburguesa."""
    pag.goto(base + '/', wait_until='networkidle')
    pag.locator('button[data-bs-toggle="offcanvas"], .navbar-toggler, '
                'button[aria-label*="menú"]').first.click()
    pag.wait_for_timeout(700)
    sacar('mov-menu')


# --------------------------------------------------------------------------- la sesion
#
# El orden es el del guion, con las dependencias que el guion identifica en su punto 4.
# Una entrada de texto es una captura que se alcanza por URL y la saca `sacar_capturas`;
# una funcion es una receta. Van entreveradas a proposito: la alerta de stock se
# fotografia antes de que la reposicion la vacie, y el calendario sanitario antes de que
# la vacunacion saque a la 177.

SESION = [
    # Modulo 0
    'm0-cu01-login', 'm0-cu02-sesion', 'm0-cu03-configuracion', m0_configuracion_efecto,
    # Modulo 1
    'm1-cu10-lista', m1_filtros, m1_alta, m1_genealogia, m1_foto,
    'm1-cu11-ficha', 't-ficha-integral', 'm1-cu08-linaje', 't-linaje-arbol',
    m1_consanguinidad, m1_baja, m1_reactivar,
    # Modulo 2
    m2_ordenie_lote, m2_control_masivo, m2_control_puntual, m2_control_seca,
    m2_control_maximo, m2_historial, m2_metrica, m2_secado, m2_lactancia, 'm2-lactancias',
    # Modulo 3
    'm3-cu25-servir', m3_celo, m3_servicio_ia, m3_servicio_monta, m3_servicio_consanguineo,
    'm3-cu22-pendientes', m3_tacto, m3_alertas_despues_del_tacto, m3_parto,
    m3_efecto_del_parto, t_parto_doble, m3_corregir_parto, 'm3-listas',
    # Modulo 4 — el calendario, antes de vacunar
    m4_diagnostico, m4_tratamiento, m4_preventivo, 'm4-cu31-calendario',
    m4_vacunacion, m4_descorne, m4_plan, m4_cerrar, 'm4-listas',
    # Modulo 5 — la alerta de stock, antes de reponer
    'm5-cu37-critico', 't-stock-antes', m5_alta_insumo, m5_ingreso, m5_minimo,
    'm5-cu38-vencimiento', m5_movimientos, 'm5-insumos',
    # Modulo 6
    'm6-cu40-tablero', 't-tablero', m6_registro_rapido, 'm6-cu41-indicadores',
    't-indicadores', 'm6-cu42-descarte', 't-descarte', m6_buscar,
    # Modulo 7
    m7_reportes,
    # El acceso va ultimo: cierra la sesion
    acceso,
]

# A 375 pixeles, en su propio contexto.
SESION_MOVIL = ['mov-lista-animales', 'mov-ficha', 'mov-ordenie', 'mov-celo',
                'mov-alertas', 'mov-linaje', mov_menu]

# Lo que no puede salir de una corrida automatica.
A_MANO = {
    'm7-configuracion-bot': 'necesita un token de bot de Telegram y un chat vinculado',
    't-telegram-vinculado': 'necesita un token de bot de Telegram y un chat vinculado',
    'm7-resumen-telegram': 'es una foto del telefono',
    't-telegram-resumen': 'es una foto del telefono',
}
