# -*- coding: utf-8 -*-
"""Comprueba que un repositorio este listo para entregar.

Se corre apuntando a la carpeta del repositorio nuevo:

    python3 verificar_entrega.py ../Tesis-entrega

Verifica el inventario de docs/entrega-repositorio.md: que no queden rastros de la
herramienta en la historia ni en los archivos, que no viajen las credenciales y que
no este el andamiaje de docs/.
"""
import os
import re
import subprocess
import sys

MARCAS = ['claude', 'anthropic']


def git(ruta, *args):
    try:
        return subprocess.run(['git', '-C', ruta] + list(args),
                              capture_output=True, text=True, timeout=60).stdout
    except Exception:
        return ''


def main():
    ruta = sys.argv[1] if len(sys.argv) > 1 else '.'
    ruta = os.path.abspath(ruta)
    print('verificando', ruta)
    print()
    fallas = []

    def revisar(titulo, valor, esperado=0):
        estado = 'ok' if valor == esperado else 'REVISAR'
        print('  %-52s %4s  %s' % (titulo, valor, estado))
        if valor != esperado:
            fallas.append(titulo)

    print('historia:')
    cuerpos = git(ruta, 'log', '--all', '--format=%s%n%b')
    revisar('commits que mencionan la herramienta',
            sum(1 for m in MARCAS for _ in re.finditer(m, cuerpos, re.I)))
    ramas = git(ruta, 'branch', '-a', '--format=%(refname:short)')
    revisar('ramas que la mencionan',
            sum(1 for l in ramas.splitlines() if any(m in l.lower() for m in MARCAS)))

    print('archivos:')
    sospechosos = []
    for base, dirs, archivos in os.walk(ruta):
        if '.git' in base.split(os.sep):
            continue
        for nombre in dirs + archivos:
            if any(m in nombre.lower() for m in MARCAS):
                sospechosos.append(os.path.join(base, nombre))
    revisar('archivos o carpetas con el nombre', len(sospechosos))
    for s in sospechosos:
        print('      ', os.path.relpath(s, ruta))

    contenido = 0
    for base, dirs, archivos in os.walk(ruta):
        if '.git' in base.split(os.sep):
            continue
        for nombre in archivos:
            if not nombre.endswith(('.cs', '.cshtml', '.md', '.json', '.sql', '.js', '.css', '.py')):
                continue
            try:
                with open(os.path.join(base, nombre), encoding='utf-8-sig', errors='ignore') as f:
                    texto = f.read()
            except OSError:
                continue
            if any(m in texto.lower() for m in MARCAS):
                contenido += 1
                print('      contenido:', os.path.relpath(os.path.join(base, nombre), ruta))
    revisar('archivos que la mencionan adentro', contenido)

    print('entrega:')
    revisar('carpeta docs/ presente', 1 if os.path.isdir(os.path.join(ruta, 'docs')) else 0)
    ajustes = os.path.join(ruta, 'Tesis', 'appsettings.json')
    expuesta = 0
    if os.path.exists(ajustes):
        with open(ajustes, encoding='utf-8-sig') as f:
            texto = f.read()
        if re.search(r'"Contrasena"\s*:\s*"(?!<)[^"]+"', texto):
            expuesta = 1
    revisar('credenciales reales en appsettings.json', expuesta)

    print()
    if fallas:
        print('QUEDAN %d PUNTOS POR REVISAR' % len(fallas))
        return 1
    print('el repositorio esta listo para entregar')
    return 0


if __name__ == '__main__':
    sys.exit(main())
