# -*- coding: utf-8 -*-
"""Arma Tesis.docx: un solo documento, desde el anteproyecto y el proyecto.

Los dos documentos se escriben y se corrigen por separado porque se entregan por
separado durante la carrera. La tesis es uno solo, y armarlo no es pegar dos archivos:
hay que renumerar el anteproyecto como `1.x`, mover el glosario y la bibliografia al
final de todo, poner un indice que calcule sus propias paginas, y traerse las sesenta y
dos imagenes del proyecto sin que se rompa ninguna.

    python docs/armar_tesis.py

Se corre **despues** de `editar_anteproyecto.py` y `editar_proyecto.py`, porque parte de
lo que ellos producen. No se edita `Tesis.docx` a mano: si algo esta mal, esta mal aca.

El indice llega hasta el tercer nivel: con el cuarto entran los catorce riesgos, los
ocho modulos de requerimientos y las factibilidades de cada alternativa, y el indice pasa
de dos paginas a seis. Va como **campo TOC de verdad**, no como una lista escrita. Word lo completa
solo: al abrir el documento pregunta si actualizar los campos y hay que decirle que si
-el archivo ya viene pidiendolo-, o se hace a mano con Ctrl+E, F9. Hasta que alguien lo
actualice, en su lugar se lee una linea que lo explica. Los numeros de pagina no los
puede calcular este script: dependen de la fuente, del interlineado y de la impresora,
y solo Word sabe donde va a caer cada corte.
"""
import copy
import os
import re
import sys
import zipfile

from docx import Document
from docx.opc.constants import RELATIONSHIP_TYPE as RT
from docx.opc.packuri import PackURI
from docx.opc.part import Part
from docx.oxml.ns import qn

import paginado

AQUI = os.path.dirname(os.path.abspath(__file__))
RAIZ = os.path.dirname(AQUI)

ANTEPROYECTO = os.path.join(RAIZ, 'Anteproyecto_v8.docx')
PROYECTO = os.path.join(RAIZ, 'Proyecto_v7.docx')
SALIDA = os.path.join(RAIZ, 'Tesis.docx')

# El anteproyecto abre con su portada y su material preliminar, que en la tesis pasan a
# ser los del documento entero; el cuerpo numerado arranca en la introduccion.
PRIMERA_SECCION = 'INTRODUCCIÓN'
# De aca hasta el compromiso, todo cuelga de «Plan de Proyecto», como en el modelo.
PLAN_DESDE = 'METODOLOGÍA'
PLAN_HASTA = 'Compromiso de Trabajo'
# Lo que en la tesis va despues del proyecto y no antes.
AL_FINAL = 'GLOSARIO'

# Palabras que en un titulo no llevan mayuscula, y siglas que no la pierden.
MENORES = {'de', 'del', 'y', 'e', 'o', 'u', 'la', 'el', 'los', 'las', 'en', 'a', 'al',
           'con', 'por', 'para', 'un', 'una', 'segun', 'sobre'}
SIGLAS = {'SQA', 'SCM', 'UML', 'MER', 'CTC', 'AP'}


# --------------------------------------------------------------------------- utilidades

def texto(elemento):
    if not elemento.tag.endswith('}p'):
        return ''
    return ''.join(n.text or '' for n in elemento.iter(qn('w:t'))).strip()


def estilo(elemento):
    if not elemento.tag.endswith('}p'):
        return None
    pPr = elemento.find(qn('w:pPr'))
    if pPr is None:
        return None
    pStyle = pPr.find(qn('w:pStyle'))
    return pStyle.get(qn('w:val')) if pStyle is not None else None


def nivel(elemento):
    """Devuelve 1..6 si el elemento es un titulo, y None si no lo es."""
    nombre = estilo(elemento) or ''
    m = re.match(r'^Heading(\d)$', nombre)
    return int(m.group(1)) if m else None


def poner_estilo(elemento, nombre):
    pPr = elemento.find(qn('w:pPr'))
    if pPr is None:
        pPr = elemento.makeelement(qn('w:pPr'), {})
        elemento.insert(0, pPr)
    for viejo in pPr.findall(qn('w:pStyle')):
        pPr.remove(viejo)
    pPr.insert(0, pPr.makeelement(qn('w:pStyle'), {qn('w:val'): nombre}))


def fuera_del_indice(elemento):
    """Deja un titulo con su aspecto pero afuera del indice.

    El material preliminar -la declaracion, el abstract, las palabras clave y el propio
    indice- usa el mismo estilo de titulo que las secciones, y sin esto el indice se
    lista a si mismo. Un campo TOC construido con el interruptor \\o se guia por el
    nivel de esquema del parrafo, no por el nombre del estilo, asi que basta con
    declararlo texto normal: el titulo se sigue viendo igual y el indice lo ignora.
    """
    pPr = elemento.find(qn('w:pPr'))
    if pPr is None:
        return
    for viejo_nivel in pPr.findall(qn('w:outlineLvl')):
        pPr.remove(viejo_nivel)
    pPr.append(pPr.makeelement(qn('w:outlineLvl'), {qn('w:val'): '9'}))


def titulo_bonito(bruto):
    """«DESCRIPCIÓN Y SELECCIÓN DE HERRAMIENTAS» -> «Descripción y Selección de...».

    El anteproyecto escribe sus titulos en mayuscula sostenida y el proyecto en capital
    inicial. En un documento con un solo indice esa mezcla se ve, asi que se unifica.
    Un titulo que ya viene en minusculas se deja como esta: no se toca lo que ya
    estaba bien.
    """
    if bruto != bruto.upper():
        return bruto
    palabras = []
    for i, palabra in enumerate(bruto.split()):
        limpia = palabra.strip('.,:()')
        if limpia in SIGLAS:
            palabras.append(palabra)
        elif i > 0 and palabra.lower() in MENORES:
            palabras.append(palabra.lower())
        else:
            palabras.append(palabra.capitalize())
    return ' '.join(palabras)


def escribir_texto(elemento, nuevo):
    """Reemplaza el texto de un parrafo conservando el formato del primer run."""
    runs = elemento.findall(qn('w:r'))
    if not runs:
        return
    for sobrante in runs[1:]:
        elemento.remove(sobrante)
    for enlace in elemento.findall(qn('w:hyperlink')):
        elemento.remove(enlace)
    run = runs[0]
    for hijo in list(run):
        if hijo.tag != qn('w:rPr'):
            run.remove(hijo)
    t = run.makeelement(qn('w:t'), {})
    t.set(qn('xml:space'), 'preserve')
    t.text = nuevo
    run.append(t)


def clonar_parrafo(modelo, texto_nuevo, estilo_nuevo=None):
    nuevo = copy.deepcopy(modelo)
    if estilo_nuevo:
        poner_estilo(nuevo, estilo_nuevo)
    escribir_texto(nuevo, texto_nuevo)
    return nuevo


# --------------------------------------------------------------------------- el campo TOC

def campo_toc(modelo, instruccion, aviso):
    """Devuelve los parrafos de un campo TOC, con su texto de espera adentro.

    Un campo de Word son tres marcas -empieza, la instruccion, termina- y en el medio el
    resultado que Word calcula. Mientras nadie lo actualice, el resultado es el aviso;
    despues, el indice con sus numeros de pagina.
    """
    p = copy.deepcopy(modelo)
    poner_estilo(p, 'Normal')
    for hijo in list(p):
        if hijo.tag != qn('w:pPr'):
            p.remove(hijo)

    def run(*hijos):
        r = p.makeelement(qn('w:r'), {})
        for hijo in hijos:
            r.append(hijo)
        p.append(r)
        return r

    inicio = p.makeelement(qn('w:fldChar'), {qn('w:fldCharType'): 'begin'})
    inicio.set(qn('w:dirty'), 'true')
    run(inicio)

    instr = p.makeelement(qn('w:instrText'), {})
    instr.set(qn('xml:space'), 'preserve')
    instr.text = instruccion
    run(instr)

    run(p.makeelement(qn('w:fldChar'), {qn('w:fldCharType'): 'separate'}))

    aviso_t = p.makeelement(qn('w:t'), {})
    aviso_t.set(qn('xml:space'), 'preserve')
    aviso_t.text = aviso
    run(aviso_t)

    run(p.makeelement(qn('w:fldChar'), {qn('w:fldCharType'): 'end'}))
    return p


def marcar(cuerpo, desde, hasta, nombre, identificador):
    """Envuelve un tramo del cuerpo en un marcador, para que un TOC lo pueda acotar."""
    inicio = desde.makeelement(qn('w:bookmarkStart'), {
        qn('w:id'): str(identificador), qn('w:name'): nombre})
    fin = desde.makeelement(qn('w:bookmarkEnd'), {qn('w:id'): str(identificador)})
    desde.addprevious(inicio)
    hasta.addnext(fin)


def pedir_actualizacion(ruta):
    """Deja el documento pidiendo que Word actualice sus campos al abrirlo.

    Sin esto el indice se ve vacio hasta que alguien se acuerde de apretar F9, y el que
    lo abre concluye que el indice no funciona.
    """
    with zipfile.ZipFile(ruta) as z:
        partes = {n: z.read(n) for n in z.namelist()}
    nombre = 'word/settings.xml'
    if nombre not in partes:
        return False
    ajustes = partes[nombre].decode('utf-8')
    if 'w:updateFields' in ajustes:
        return True
    ajustes = ajustes.replace(
        '<w:settings ', '<w:settings ', 1)
    corte = ajustes.index('>', ajustes.index('<w:settings')) + 1
    ajustes = ajustes[:corte] + '<w:updateFields w:val="true"/>' + ajustes[corte:]
    partes[nombre] = ajustes.encode('utf-8')

    temporal = ruta + '.tmp'
    with zipfile.ZipFile(temporal, 'w', zipfile.ZIP_DEFLATED) as z:
        for clave, contenido in partes.items():
            z.writestr(clave, contenido)
    os.replace(temporal, ruta)
    return True


# --------------------------------------------------------------------------- las imagenes

def sin_colision(parte, destino, copiadas):
    """Trae una imagen al paquete destino con un nombre que no pise a ninguna.

    Las dos fuentes numeran sus imagenes desde image1.png, asi que la primera del
    proyecto y la primera del anteproyecto se llaman igual. Guardadas con ese nombre en
    el mismo paquete, una se come a la otra y el documento pierde una figura sin avisar
    -asi se perdio el organigrama del entorno en la primera version de este script-.
    """
    if parte in copiadas:
        return copiadas[parte]
    paquete = destino.part.package
    ocupados = {str(otra.partname) for otra in paquete.iter_parts()}
    nombre = str(parte.partname)
    if nombre in ocupados:
        raiz, extension = os.path.splitext(nombre)
        i = 2
        while '%s_%d%s' % (raiz, i, extension) in ocupados:
            i += 1
        nombre = '%s_%d%s' % (raiz, i, extension)
    copia = Part(PackURI(nombre), parte.content_type, parte.blob, paquete)
    copiadas[parte] = copia
    return copia


def trasplantar(elemento, origen, destino, copiadas):
    """Reapunta las imagenes de un elemento copiado a las del documento nuevo.

    Un `r:embed` no nombra un archivo: nombra una relacion del documento del que salio.
    Copiado tal cual a otro documento, ese identificador apunta a cualquier cosa o a
    nada. Por cada uno hay que traer la imagen -con un nombre libre- y anotar el
    identificador nuevo.
    """
    for atributo in (qn('r:embed'), qn('r:id'), qn('r:link')):
        for nodo in elemento.iter():
            referencia = nodo.get(atributo)
            if not referencia:
                continue
            try:
                parte = origen.part.related_parts[referencia]
            except KeyError:
                continue
            copia = sin_colision(parte, destino, copiadas)
            nodo.set(atributo, destino.part.relate_to(copia, RT.IMAGE))


# --------------------------------------------------------------------------- el armado

def numerar(elementos):
    """Pone «1.x» delante de cada titulo del anteproyecto y unifica las mayusculas."""
    contadores = [0, 0, 0]
    puestos = []
    for elemento in elementos:
        n = nivel(elemento)
        bruto = texto(elemento)
        if n is None or n < 2 or not bruto:
            continue
        profundidad = min(n - 2, 2)
        contadores[profundidad] += 1
        for mas_hondo in range(profundidad + 1, 3):
            contadores[mas_hondo] = 0
        numero = '1.' + '.'.join(str(c) for c in contadores[:profundidad + 1] if True)
        numero = '1.' + '.'.join(
            str(contadores[i]) for i in range(profundidad + 1))
        escribir_texto(elemento, '%s  %s' % (numero, titulo_bonito(bruto)))
        puestos.append(numero)
    return puestos


def portada(elementos, modelo_titulo):
    """Pone la portada y la declaracion a nombre del trabajo entero.

    Las dos vienen del anteproyecto y hablan de el: la tapa dice «Anteproyecto» y la
    declaracion, «Examen Integrador 1». Este documento es la entrega final.
    """
    for elemento in elementos:
        crudo = texto(elemento)
        if crudo == 'Anteproyecto':
            escribir_texto(elemento, 'Trabajo Final de Carrera')
        elif crudo == 'Gestión de Tambo':
            elemento.addnext(clonar_parrafo(
                elemento,
                'Entregado para la obtención del título de Analista Programador'))
        elif crudo == 'Abril 2026':
            escribir_texto(elemento, '2026')
        elif crudo.startswith('- La obra fue producida en su totalidad'):
            escribir_texto(
                elemento,
                '- La obra fue producida en su totalidad mientras realizábamos el '
                'Examen Integrador 2 (Proyecto Integrador AP-CTC);')


ABSTRACT = [
    'El presente trabajo abarca el proceso completo de análisis, diseño, desarrollo e '
    'implementación de un sistema de gestión para un establecimiento de producción '
    'lechera de carácter familiar, ubicado en la zona de Colonia Cosmopolita, '
    'departamento de Colonia, Uruguay.',

    'El establecimiento lleva hoy la información del rodeo en cuadernos con formato de '
    'planilla y en anotaciones de pizarrón. Los datos se capturan durante la jornada y '
    'se transcriben después, lo que dispersa la información, da lugar a errores de '
    'copia y dificulta consultar la historia de un animal o construir indicadores para '
    'decidir.',

    'El sistema centraliza esa información en un único repositorio y cubre la operativa '
    'diaria en siete módulos: gestión de animales y genética, con control de linaje, '
    'verificación de consanguinidad y clasificación automática por categoría; control '
    'de producción, con el ordeñe por lote, el control lechero y el seguimiento de '
    'lactancias; gestión reproductiva, con celos, servicios, tactos y partos; gestión '
    'sanitaria, con diagnósticos, tratamientos, planes de vacunación y descornes; '
    'control de insumos, con alertas de stock mínimo y de vencimiento por partida; un '
    'tablero con las tareas del día, indicadores del rodeo y apoyo a la decisión de '
    'descarte; y reportes y notificaciones.',

    'La propiedad que ordena el diseño es que cada dato se carga una sola vez y el '
    'sistema propaga sus consecuencias a los demás módulos: un tratamiento sanitario '
    'deja a la vaca fuera del tanque de leche mientras dure el descarte, un parto abre '
    'la lactancia de la madre y da de alta la cría, y una inseminación descuenta la '
    'pajuela del stock.',

    'El sistema es una aplicación web, accesible desde cualquier dispositivo con '
    'conexión a internet y con la interfaz verificada para su uso desde el celular en '
    'el propio tambo, que es donde ocurren los eventos que se registran. Se implementó '
    'con ASP.NET Core y Razor Pages en lenguaje C#, sobre una arquitectura en tres '
    'capas, con MySQL como gestor de base de datos, y envía avisos y un resumen diario '
    'de tareas pendientes a través de un bot de Telegram.',
]


def abstract(elementos):
    """Reemplaza el abstract del anteproyecto por el del trabajo entero.

    El que traia el anteproyecto dice que el trabajo «aborda el análisis, diseño y
    planificación»: era cierto cuando se escribio, y este documento ademas construye el
    sistema y lo pone a andar.
    """
    i = next(i for i, e in enumerate(elementos) if texto(e) == 'ABSTRACT')
    j = next(j for j in range(i + 1, len(elementos))
             if nivel(elementos[j]) is not None and texto(elementos[j]))
    viejos = [e for e in elementos[i + 1:j] if e.tag.endswith('}p') and texto(e)]
    modelo = viejos[0]
    for sobrante in viejos[1:]:
        sobrante.getparent().remove(sobrante)
    escribir_texto(modelo, ABSTRACT[0])
    ultimo = modelo
    for parrafo in ABSTRACT[1:]:
        nuevo = clonar_parrafo(modelo, parrafo)
        ultimo.addnext(nuevo)
        ultimo = nuevo
    return len(ABSTRACT)


def main():
    ante = Document(ANTEPROYECTO)
    proy = Document(PROYECTO)
    cuerpo = ante.element.body
    elementos = list(cuerpo.iterchildren())

    def ubicar(prefijo, desde=0):
        for i, elemento in enumerate(elementos):
            if i >= desde and texto(elemento).startswith(prefijo):
                return i
        raise LookupError('no se encontro en el anteproyecto: ' + prefijo)

    i_primera = ubicar(PRIMERA_SECCION)
    i_final = ubicar(AL_FINAL)
    modelo_titulo = elementos[i_primera]
    modelo_parrafo = next(e for e in elementos[i_primera:]
                          if e.tag.endswith('}p') and texto(e) and estilo(e) is None)

    # 0. La portada, la declaracion y el abstract vienen del anteproyecto y hablan de
    #    el, no del trabajo terminado.
    portada(elementos, modelo_titulo)
    parrafos_abstract = abstract(elementos)

    # 1. El glosario y la bibliografia se guardan: en la tesis van despues del proyecto.
    cola = [e for e in elementos[i_final:] if not e.tag.endswith('}sectPr')]
    for elemento in cola:
        cuerpo.remove(elemento)

    # 2. «Plan de Proyecto», que agrupa los once apartados de planificacion como en el
    #    modelo. Los que agrupa bajan un nivel, y con ellos sus propios subtitulos.
    i_plan = ubicar(PLAN_DESDE)
    i_fin_plan = ubicar(PLAN_HASTA, i_plan)
    for elemento in elementos[i_plan:i_fin_plan]:
        n = nivel(elemento)
        if n is not None:
            poner_estilo(elemento, 'Heading%d' % min(6, n + 1))
    elementos[i_plan].addprevious(
        clonar_parrafo(modelo_titulo, 'PLAN DE PROYECTO', 'Heading2'))

    # 3. La numeracion, sobre el cuerpo ya reacomodado.
    elementos = list(cuerpo.iterchildren())
    i_primera = ubicar(PRIMERA_SECCION)
    i_fin = ubicar(PLAN_HASTA) + 1
    while i_fin < len(elementos) and not elementos[i_fin].tag.endswith('}sectPr'):
        i_fin += 1
    numeros = numerar(elementos[i_primera:i_fin])

    # 4. El titulo de la primera parte.
    elementos[i_primera].addprevious(
        clonar_parrafo(modelo_titulo, '1.  Anteproyecto', 'Heading1'))

    # 5. El proyecto entero, detras. Se saltean su indice de estado y su titulo de
    #    seccion, que se reescribe, y se deja afuera el sectPr, que es del otro archivo.
    elementos_proy = list(proy.element.body.iterchildren())
    i_cuerpo_proy = next(i for i, e in enumerate(elementos_proy)
                         if nivel(e) == 1 and texto(e).startswith('2.'))
    ultimo = list(cuerpo.iterchildren())[-1]
    if ultimo.tag.endswith('}sectPr'):
        ancla = ultimo
        def agregar(nuevo):
            ancla.addprevious(nuevo)
    else:
        estado = {'ancla': ultimo}
        def agregar(nuevo):
            estado['ancla'].addnext(nuevo)
            estado['ancla'] = nuevo

    # El v5 dejo cuatro apartados de diseño un nivel mas abajo que sus hermanas: 2.2.1,
    # 2.2.2 y 2.2.5 son Heading3 y 2.2.3, 2.2.4, 2.2.6 y 2.2.7 son Heading4. Nadie lo
    # notaba porque no habia indice; con indice, los diagramas de dominio, de
    # persistencia, los cuarenta y nueve de secuencia y el diccionario de clases se
    # caian de la lista. Se emparejan.
    desnivelados = 0
    for elemento in elementos_proy:
        if nivel(elemento) == 4 and re.match(r'^2\.2\.\d\s', texto(elemento)):
            poner_estilo(elemento, 'Heading3')
            desnivelados += 1

    copiados = 0
    copiadas = {}
    for elemento in elementos_proy[i_cuerpo_proy:]:
        if elemento.tag.endswith('}sectPr'):
            continue
        nuevo = copy.deepcopy(elemento)
        trasplantar(nuevo, proy, ante, copiadas)
        agregar(nuevo)
        copiados += 1

    # 6. El glosario y la bibliografia, ahora si, y el anexo que no existia.
    for elemento in cola:
        agregar(elemento)

    agregar(clonar_parrafo(modelo_titulo, 'ANEXO', 'Heading2'))
    agregar(clonar_parrafo(
        modelo_parrafo,
        'Se adjuntan los registros con los que el establecimiento lleva hoy la '
        'informacion del rodeo: el cuaderno de ordenie, el cuaderno de eventos '
        'reproductivos y sanitarios, y el pizarron de la sala. Son el punto de partida '
        'del problema que este proyecto resuelve.'))
    agregar(clonar_parrafo(
        modelo_parrafo,
        '[COMPLETAR: las fotografias de los cuadernos y del pizarron, que tienen que '
        'sacar los autores.]'))

    # 7. Los dos indices.
    #
    # El anteproyecto ya traia un indice, pero adentro de un control de contenido y con
    # el resultado de una version vieja congelado: listaba secciones que ya no existen y
    # una de sus paginas decia 0. Como no es un parrafo del cuerpo, no aparece al
    # recorrer el documento y por eso habia pasado por invisible. Se saca entero -si
    # quedara, la tesis tendria dos indices y el primero seria el equivocado- y en su
    # lugar va un campo con los interruptores que corresponden.
    viejos = [e for e in cuerpo.iterchildren()
              if any('TOC' in (t.text or '') for t in e.iter(qn('w:instrText')))]
    for elemento in viejos:
        cuerpo.remove(elemento)

    elementos = list(cuerpo.iterchildren())
    i_indice = ubicar('ÍNDICE')
    escribir_texto(elementos[i_indice], 'ÍNDICE')
    elementos[i_indice].addnext(campo_toc(
        modelo_parrafo, ' TOC \\o "1-3" \\h \\z \\u ',
        'El índice se genera solo: hacer clic acá, seleccionar todo con Ctrl+E y '
        'actualizar con F9. Word lo completa con sus números de página.'))

    for preliminar in ('DECLARACIÓN DE AUTORÍA', 'ABSTRACT', 'PALABRAS CLAVE', 'ÍNDICE'):
        fuera_del_indice(elementos[ubicar(preliminar)])

    marcado = indice_del_manual(cuerpo, modelo_parrafo)

    secciones_numeradas = paginado.numerar(ante)
    ante.save(SALIDA)
    actualiza = pedir_actualizacion(SALIDA)

    print('secciones numeradas del anteproyecto: %d (hasta %s)'
          % (len(numeros), numeros[-1] if numeros else '-'))
    print('apartados de 2.2 emparejados de nivel: %d' % desnivelados)
    print('abstract reescrito: %d parrafos' % parrafos_abstract)
    print('elementos traidos del proyecto: %d, con %d imagenes'
          % (copiados, len(copiadas)))
    print('indice del manual: %s' % ('acotado con un marcador' if marcado else
                                     'NO se pudo acotar'))
    print('secciones con numero de pagina: %d' % secciones_numeradas)
    print('Word va a pedir actualizar los campos al abrir: %s' % ('si' if actualiza
                                                                  else 'NO'))
    print('guardado', SALIDA)
    return 0


def indice_del_manual(cuerpo, modelo_parrafo):
    """Cambia el indice escrito a mano del manual por un campo acotado a su parte.

    El manual lleva indice propio, como en el modelo, y escrito a mano no tiene numeros
    de pagina. Acotado con un marcador al tramo del manual, el mismo mecanismo del
    indice general se los pone.
    """
    elementos = list(cuerpo.iterchildren())
    try:
        i_manual = next(i for i, e in enumerate(elementos)
                        if texto(e).startswith('2.4') and nivel(e) == 2)
        i_fin = next(i for i, e in enumerate(elementos)
                     if i > i_manual and texto(e).startswith('2.5') and nivel(e) == 2)
        i_titulo = next(i for i in range(i_manual, i_fin)
                        if texto(elementos[i]) == 'Índice')
    except StopIteration:
        return False

    # Las entradas escritas a mano: desde el titulo hasta el primer titulo siguiente.
    i_ultima = i_titulo + 1
    while i_ultima < i_fin and nivel(elementos[i_ultima]) is None:
        i_ultima += 1
    for elemento in elementos[i_titulo + 1:i_ultima]:
        cuerpo.remove(elemento)

    campo = campo_toc(
        modelo_parrafo, ' TOC \\b manual \\o "5-6" \\h \\z \\u ',
        'El índice del manual se genera solo: seleccionar todo con Ctrl+E y '
        'actualizar con F9.')
    elementos[i_titulo].addnext(campo)

    # El marcador arranca despues del campo y no en el titulo del manual: si empezara
    # antes, el indice se listaria a si mismo —«2.4 Manual de Usuario» y «Índice» son
    # titulos del rango que \o "5-6" alcanza— y un indice que figura dentro de si mismo
    # se lee como un error.
    marcar(cuerpo, campo.getnext(), elementos[i_fin - 1], 'manual', 9001)
    return True


if __name__ == '__main__':
    sys.exit(main())
