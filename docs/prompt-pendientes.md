# Prompt — lo que queda de la tesis

> Pegar en un chat nuevo de Claude Code abierto en `C:\Users\Usuario\Documents\Tesis`.

---

Estás retomando la tesis «Sistema de Gestión de Tambo» (Analista Programador, CTC
Rosario; autores Santino Delmonte y Alejo de León; tutor Andrés Klett). **El sistema está
terminado**: los siete módulos, CU1 a CU49, `dotnet build` con 0 errores. Lo que queda es
**documento y evidencia**. Antes de hacer nada, leé esto entero y después verificá contra
el repositorio lo que te importe: este prompt se escribió el 10/09/2026 y el código manda.

## 1. Dónde está cada cosa

| Qué | Dónde |
|---|---|
| El sistema | `Tesis/` (.NET 10, Razor Pages, MySQL/MariaDB en XAMPP) y `bd/CreacionDb.sql` + `bd/DatosPrueba.sql` |
| El plan maestro | `docs/prompt-documento-final.md` — cuatro fases; leé sobre todo sus puntos 2, 3, 7 y 8 |
| Anteproyecto | `Anteproyecto_v8.docx`, **generado** por `docs/editar_anteproyecto.py` desde el v5 |
| Proyecto | `Proyecto_v7.docx`, **generado** por `docs/editar_proyecto.py` desde el v5 más `casos_de_uso_parte*.py`, `modelo_datos.py`, `diccionario_clases.py` y `docs/diagramas/*.png` |
| Secciones 2.3 a 2.7 | `docs/seccion-2-3-pruebas.md` … `docs/seccion-2-7-contingencia.md` — escritas en markdown, **todavía no están en el Word** |
| Capturas | `docs/guion-capturas.md` (qué fotografiar) y `docs/verificar_capturas.py` (hoy: 112 pedidas, 112 definidas). `docs/capturas/` no existe |
| Registro de lo decidido | `docs/cambios-anteproyecto-v6.md`, `-v7.md`, `-v8.md` y `docs/auditoria-tres-vias.md` (14 hallazgos, 13 cerrados) |
| Lo que falta del sistema | `docs/pendientes-tecnicos.md` — dice «No queda trabajo de código» |
| Recorrido manual del sistema | `docs/flujos-de-prueba.md` — ya con el menú de hoy |

`docs/` es andamiaje: no se entrega. Lo que se entrega es el código y un documento único.

## 2. Reglas de trabajo — aprendidas, no supuestas

- **Idioma**: español rioplatense, con voseo. Los documentos se escriben en prosa que
  argumenta, con tablas donde hay datos; nada de relleno ni de afirmaciones sin verificar.
- **Manda el código.** Si un documento y el código discrepan, el código tiene razón salvo
  que sea un error. Pero **un cambio de requerimiento no se aplica sin consultar** a los
  autores: se propone, se aprueba y recién ahí se escribe.
- **Los `.docx` se generan, nunca se editan a mano.** Toda corrección va al script. Después
  de tocar un generador, **regenerá y compará la salida contra la anterior**: esta semana se
  encontraron dos veces documentos que no reflejaban su propio script, porque alguien
  corrigió la fuente y no volvió a generar.
- **Lo mismo con los diagramas**: `docs/diagramas/generar_*.py` y los tres `.md` del paso 1
  (`modelo_datos.py`, `diccionario_clases.py`, `render_casos_de_uso.py`). Regenerados contra
  el código de hoy, `git status` tiene que quedar limpio; si no, algo estaba viejo.
- **No hay pruebas automatizadas y no las va a haber** (decisión de los autores). Las
  pruebas son funcionales y manuales, sobre el sistema andando.
- **No levantes el sistema** (`dotnet run`, navegador): lo levanta el autor desde Visual
  Studio. Para verificar código alcanza con `dotnet build Tesis/Tesis.csproj`.
- **Cuidado con los archivos**: cada `.cs` y `.py` tiene su codificación —unos con BOM,
  otros sin— y CRLF. Editá a nivel de bytes y preservá las dos cosas.
- **Dos trampas de las herramientas que ya costaron caro**: un heredoc de bash con
  apóstrofes adentro se rompe (escribí el script con la herramienta de escritura y corrélo
  aparte); y un reemplazo por expresión regular con `.*` y DOTALL borró media tabla de un
  archivo. Para reemplazar un renglón usá `[^\n]*`, controlá la cantidad de coincidencias y
  el conteo de renglones antes de escribir, y no escribas nada si algo no coincidió.
- **Git**: se trabaja en `master`. **Commit sólo cuando te lo pidan; push nunca sin
  pedido.** Mensajes en español sin tildes: un título que cuenta qué cambió y por qué, un
  cuerpo en prosa (sin viñetas) y al final la línea `Co-Authored-By` que corresponda. Si
  hace falta una rama, nombre descriptivo de la funcionalidad, sin prefijo de herramienta.

## 3. Lo que queda, en orden

### A. Insertar 2.3 a 2.7 en el Proyecto — lo hacés vos

`editar_proyecto.py` arma hasta la 2.2.7 y deja el resto como estaba en el v5. Falta el
paso que toma `docs/seccion-2-*.md` y los escribe en su lugar, con el formato del
documento. El punto 3 del plan maestro («Dos pasos, no uno: generar y después editar»)
dice cómo. Tres condiciones:

- Cada marca `[captura: nombre]` se reemplaza por la imagen de `docs/capturas/nombre.png`
  con el pie que la sigue, usando el mismo `d.imagen(ruta, pie)` que coloca los diagramas.
  **Mientras no haya capturas**, dejá un marcador visible, no un hueco.
- **El manual (2.4) lleva su propio índice**, numerado aparte, como en `EjemploTesis.pdf`:
  es un documento adentro del documento.
- Cuando existan 2.8 y 2.9, entran por el mismo camino.

Regenerá y compará el Proyecto antes y después: todo lo que ya estaba tiene que seguir
igual.

### B. El script de capturas — lo hacés vos, lo corre el autor

No existe. `guion-capturas.md` describe un recorrido de **Playwright** que inicia sesión
como `sofia`, fija el tamaño de ventana (1280 × 800; las del celular a 375 × 812) y
visita cada pantalla con el rodeo de `DatosPrueba.sql` recién cargado, escribiendo
`docs/capturas/<nombre>.png`. Leé el guion completo, sobre todo **«El orden importa»**:
algunas capturas dependen de acciones previas (la alerta de stock antes de reponer, las
pruebas `t-` después de la acción que evidencian). Dos no las puede sacar un script: son
fotos del teléfono (`m7-resumen-telegram` y `t-telegram-resumen`).

El script corre en la máquina del autor, con el sistema levantado desde Visual Studio.
Dejalo listo para que él lo ejecute y explicale cómo en dos líneas.

### C. 2.9 Conclusiones — lo hacés vos

Siete páginas, sin imágenes, con los **diez subtítulos del ejemplo** que lista el plan
maestro (punto 7). El bloque largo es el **4, Riesgos**: los catorce del anteproyecto, uno
por uno, si se dio o no y cómo se resolvió. Ahí va lo que encontró la auditoría —qué
requerimiento cambió, cuándo y por qué—, con las fechas de `auditoria-tres-vias.md` y de
los tres `cambios-anteproyecto-v*.md`. El punto 9, *Puntos a mejorar*, dice de frente lo
que quedó corto.

Lo que depende de datos que no tenés —si los objetivos se cumplieron *según las pruebas
ejecutadas*, lo que dijo la clienta— queda **marcado para completar**, no inventado.

### D. `docs/armar_tesis.py`, la Fase 4 — lo hacés vos

Produce `Tesis.docx` desde el anteproyecto y el proyecto: el anteproyecto renumerado como
`1.x`, el proyecto como `2.x`, portada, declaración de autoría, abstract, palabras clave,
índice único, glosario, bibliografía y anexo. Está detallado en el punto 8 del plan
maestro. El índice va como **campo TOC de verdad**; los números de página los calcula Word
cuando alguien actualiza el campo, y eso hay que decírselo a los autores.

El plan señala **dos huecos** que conviene verificar contra el `Anteproyecto_v8.docx`
antes de darlos por ciertos: falta `1.11 Estimación del esfuerzo`, y el **anexo** necesita
fotos de los cuadernos y planillas con que hoy se lleva el tambo, que tienen que traer
los autores.

### E. Lo que sólo pueden hacer los autores

- **2.8 Grado de satisfacción del cliente** (una página, sin imágenes). No es una encuesta:
  es el relato de la relación con la encargada a lo largo de las iteraciones. Hace falta que
  le muestren el sistema y anoten lo que dice; con esas notas, la redactás vos. De la misma
  sesión sale el criterio del RNF de Usabilidad: que complete sola las cinco tareas diarias
  después de una única capacitación.
- **Levantar el sistema** con `DatosPrueba.sql` recién cargado, correr el script de
  capturas y ejecutar las pruebas de 2.3, completando la columna «Resultado». Conviene en
  una sola sesión.
- **Push**: `master` está seis commits adelante de GitHub.

### F. Decisiones que siguen abiertas — preguntá, no decidas

- **Fusionar RF3.4 con RF3.5 y quitar RF5.2.** Viene de la v7 y quedó para discusión
  aparte: cambia la cantidad de requerimientos (hoy 74) y obliga a renumerar y a tocar los
  casos de uso que los referencian. Si no se hace, que quede decidido antes de unificar.
- **El reporte genético** no tiene período, pero sus secciones vacías dicen «Sin registros
  en el período»: la frase está fija en `Tesis/Reportes/GeneradorPdf.cs` y
  `GeneradorExcel.cs`. Es una línea de código si la quieren cambiar.

### G. Lo último, y sólo al final

- **Las credenciales versionadas** (hallazgo H6, el único abierto de la auditoría):
  `Tesis/appsettings.json` tiene usuario, contraseña y cadena de conexión en texto plano.
- **El repositorio de entrega**, nuevo y sin historia: el procedimiento completo está en
  `docs/entrega-repositorio.md`. No se ejecuta hasta que todo lo demás esté terminado.

### Menor, cuando haya un rato

`flujos-de-prueba.md` ya usa los caminos del menú de hoy, pero todavía nombra algunas
pantallas por su título viejo —«Celos Detectados», «Tactos Pendientes»—. Si alguno no
coincide con el título real, se va a ver al ejecutar los flujos.

## 4. Por dónde empezar

Por **A**: sin él, nada de lo escrito en 2.3 a 2.7 está en el documento que se entrega, y
todo lo demás termina pasando por ese mismo paso. Después **B**, que es lo que destraba al
autor el día que levante el sistema; después **C** y **D**.

Antes de cambiar nada, contame en pocas líneas qué encontraste al verificar este prompt
contra el repositorio y cómo pensás encarar **A**.
