# -*- coding: utf-8 -*-
"""Lee las pantallas de Tesis/Pages y escribe docs/inventario-pantallas.md.

El manual de usuario se escribe sobre esto y no sobre la memoria: de cada pantalla
salen su titulo, sus campos, sus botones y los mensajes con que rechaza o advierte.
Si cambia una pantalla, se vuelve a correr y se ve que hay que reescribir.
"""
import os
import re

AQUI = os.path.dirname(os.path.abspath(__file__))
RAIZ = os.path.dirname(AQUI)
PAGINAS = os.path.join(RAIZ, 'Tesis', 'Pages')

MODULOS = [
    ('PagesSeguridad', 'Modulo 0 - Seguridad y acceso'),
    ('PagesConfiguracion', 'Modulo 0 - Configuracion'),
    ('PagesAnimal', 'Modulo 1 - Animales y genetica'),
    ('PagesProduccion', 'Modulo 2 - Produccion'),
    ('PagesReproduccion', 'Modulo 3 - Reproduccion'),
    ('PagesSanidad', 'Modulo 4 - Sanidad'),
    ('PagesInsumo', 'Modulo 5 - Insumos y stock'),
    ('PagesIndicadores', 'Modulo 6 - Indicadores'),
    ('PagesReportes', 'Modulo 7 - Reportes'),
]


def leer(ruta):
    with open(ruta, encoding='utf-8-sig') as f:
        return f.read()


def titulo(cshtml):
    m = re.search(r'ViewData\["Title"\]\s*=\s*"([^"]+)"', cshtml)
    if m:
        return m.group(1)
    m = re.search(r'<h1[^>]*>([^<]+)</h1>', cshtml)
    return m.group(1).strip() if m else ''


def campos(cshtml):
    """Etiqueta visible de cada campo, en el orden en que aparece."""
    salida = []
    for m in re.finditer(r'<label[^>]*>(.*?)</label>', cshtml, re.S):
        texto = re.sub(r'<[^>]+>', '', m.group(1)).strip().rstrip(':')
        if texto and texto not in salida:
            salida.append(texto)
    return salida


def botones(cshtml):
    salida = []
    for m in re.finditer(r'<button[^>]*>(.*?)</button>', cshtml, re.S):
        texto = re.sub(r'<[^>]+>', '', m.group(1)).strip()
        if texto and texto not in salida:
            salida.append(texto)
    for m in re.finditer(r'<input[^>]*type="submit"[^>]*value="([^"]+)"', cshtml):
        if m.group(1) not in salida:
            salida.append(m.group(1))
    return salida


def mensajes(cs):
    """Lo que la pantalla le dice al usuario cuando algo no va."""
    salida = []
    for m in re.finditer(r'"([A-ZÁÉÍÓÚÑ][^"]{15,180}[!?.])"', cs):
        texto = m.group(1)
        if texto not in salida:
            salida.append(texto)
    return salida


def main():
    lineas = ['# Inventario de pantallas',
              '',
              'Generado por `docs/inventario_pantallas.py` leyendo `Tesis/Pages`. No editar a mano.',
              '',
              'Es la fuente de la sección 2.4, el Manual de Usuario: de cada pantalla salen su',
              'título, sus campos, sus botones y los mensajes con que rechaza o advierte.',
              '']
    total = 0
    for carpeta, nombre in MODULOS:
        ruta = os.path.join(PAGINAS, carpeta)
        if not os.path.isdir(ruta):
            continue
        lineas.append('## ' + nombre)
        lineas.append('')
        for archivo in sorted(os.listdir(ruta)):
            if not archivo.endswith('.cshtml') or archivo.startswith('_'):
                continue
            cshtml = leer(os.path.join(ruta, archivo))
            ruta_cs = os.path.join(ruta, archivo + '.cs')
            cs = leer(ruta_cs) if os.path.exists(ruta_cs) else ''
            total += 1
            lineas.append('### ' + archivo[:-7])
            lineas.append('')
            lineas.append('- **Título:** ' + (titulo(cshtml) or '—'))
            cs_campos = campos(cshtml)
            lineas.append('- **Campos:** ' + (', '.join(cs_campos) if cs_campos else '—'))
            bs = botones(cshtml)
            lineas.append('- **Acciones:** ' + (', '.join(bs) if bs else '—'))
            ms = mensajes(cs)
            if ms:
                lineas.append('- **Mensajes al usuario:**')
                for texto in ms:
                    lineas.append('  - ' + texto)
            lineas.append('')
    destino = os.path.join(AQUI, 'inventario-pantallas.md')
    with open(destino, 'w', encoding='utf-8') as f:
        f.write('\n'.join(lineas))
    print('escrito %s | %d pantallas' % (destino, total))


if __name__ == '__main__':
    main()
