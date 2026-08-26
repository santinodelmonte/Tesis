# Preparación del repositorio de entrega

**Decisión tomada el 26/08/2026: opción A — repositorio nuevo.** No se ejecuta ahora;
se ejecuta cuando el trabajo esté terminado y antes de entregar. Este documento existe
para que llegado el momento sea un procedimiento y no una improvisación.

---

## 1. Qué se entrega

| Se entrega | No se entrega |
|---|---|
| El **código** del sistema (`Tesis/`, `bd/`, `Tesis.slnx`) | Todo `docs/` |
| El **documento único** —anteproyecto + proyecto—, con la forma de `EjemploTesis.pdf` | El `EjemploTesis.pdf` del tutor |
| | Los `.docx` intermedios (v5, v6, v7) |

`docs/` es **andamiaje**: el prompt de trabajo, el guion de capturas, la auditoría, la
revisión, los generadores de diagramas y el inventario de pantallas. Sirvieron para
construir el documento y no forman parte de lo que se entrega. **El documento final ya
contiene todo lo que producían.**

## 2. Por qué un repositorio nuevo y no limpiar el actual

Porque es **la única opción que resuelve los Pull Requests**. Reescribir la historia
con `git filter-repo` limpia los commits y las ramas, pero los PR ya abiertos en GitHub
conservan su título —`Merge pull request #9 from santinodelmonte/claude/…`—, sus
descripciones y su lista de commits. Eso no se borra reescribiendo: se borra borrando
el repositorio, y entonces ya estamos hablando de uno nuevo.

## 3. Dónde aparece hoy, medido

Este es el inventario del 26/08/2026. Sirve como lista de verificación: **al terminar,
ninguna de estas líneas debe dar un número distinto de cero en el repositorio nuevo.**

| Rastro | Cantidad hoy |
|---|---|
| Commits con `Co-Authored-By:` | **47** de 69 |
| Commits con enlace de sesión | **44** |
| Ramas con prefijo de herramienta | **11** de 13 |
| Merges en `master` que nombran esas ramas | **9** |
| Carpeta `.claude/` versionada | 1 (`launch.json`) |
| Archivos de `docs/` con menciones | 2 |
| Pull Requests en GitHub | 9 |

## 4. Antes de crear el repositorio nuevo

Tres cosas que hay que resolver **primero**, porque si no viajan al repo limpio:

1. **Sacar las credenciales de `Tesis/appsettings.json`** (hallazgo H6 de la auditoría).
   Hoy el archivo tiene usuario, contraseña y cadena de conexión en texto plano. En el
   repositorio nuevo tiene que ir **sólo con marcadores**. Y la contraseña que estuvo
   publicada no se reutiliza.
2. **Borrar `.claude/launch.json`**. Su contenido no dice nada, pero el nombre de la
   carpeta sí. Es una configuración de arranque que se puede rehacer en cualquier
   máquina.
3. **Verificar que el código no tenga comentarios que lo mencionen.** Hoy no los tiene;
   conviene volver a mirar al final, porque el Módulo 7 todavía se está escribiendo.

## 5. El procedimiento

```bash
# 1. Partir de una copia limpia del estado final, sin la carpeta .git
cd ..
cp -r Tesis Tesis-entrega
cd Tesis-entrega
rm -rf .git docs .claude EjemploTesis.pdf
rm -f Anteproyecto_v5.docx Anteproyecto_v5.docx.pdf Anteproyecto_v6.docx Anteproyecto_v7.docx
rm -f Proyecto_v5.docx Proyecto_v5.pdf Proyecto_v6.docx

# 2. Dejar el documento final con un nombre que se entienda solo
#    (copiarlo desde donde esté: Tesis.docx y Tesis.pdf)

# 3. Repositorio nuevo
git init -b main
git add .
git commit -m "Sistema de gestion de tambo"

# 4. Crear el repositorio vacío en GitHub, con nombre nuevo, y subir
git remote add origin <url-del-repo-nuevo>
git push -u origin main
```

**Sobre la rama principal:** el repositorio nuevo se crea con `main`, que es lo que el
anteproyecto declara en Control de Versionado. Hoy el repositorio se llama `master` y
por eso el hallazgo H8 propone corregir el documento; **si al final se entrega un repo
nuevo con `main`, H8 se resuelve solo en esa parte** y el documento puede quedar como
está. Los nombres de las ramas de trabajo, en cambio, hay que corregirlos en el
documento igual: el repositorio de entrega no va a tener ramas de trabajo.

## 6. Sobre la historia de commits

**Dos formas, y las dos son legítimas:**

- **Un solo commit inicial.** El repositorio entrega un estado terminado. Es lo más
  simple y no afirma nada que no sea cierto.
- **Unos pocos commits temáticos**, uno por módulo o por iteración, armados desde el
  estado final. Refleja la estructura real del trabajo y acompaña lo que el anteproyecto
  dice sobre desarrollo iterativo.

**Lo que no hay que hacer es inventar fechas.** Se pueden agrupar los cambios por tema,
pero fabricar una cronología que simule meses de trabajo es otra cosa, y además no
resiste una pregunta directa. Si se eligen commits temáticos, que lleven la fecha en que
efectivamente se crean.

## 7. El repositorio actual no se borra

**Se conserva, en privado.** Es el respaldo del trabajo: la historia real, la auditoría,
la revisión de ingeniería de software y los registros de cambios. Si en la defensa
preguntan cómo se llegó a algo —por qué RF1.1 dice lo que dice, de dónde salieron los
cinco criterios de descarte— la respuesta está ahí.

Lo que se hace con él es ponerlo en **privado** o **archivarlo**, no eliminarlo.

## 8. Verificación final

Con el repositorio nuevo creado, correr `docs/verificar_entrega.py` desde una copia de
este `docs/` (no desde el repo nuevo, que ya no lo tiene). Comprueba las siete líneas
del inventario del punto 3 y las tres del punto 4.

---

## Lo que este documento no decide

Si en el instituto una herramienta permitida cuenta o no como «ayuda recibida» a los
efectos de la Declaración de Autoría que firman, es una cuestión de las normas del
instituto y la deciden ustedes. Lo que está acá es el procedimiento técnico, medido y
verificable, para el caso en que decidan que no corresponde declararla.
