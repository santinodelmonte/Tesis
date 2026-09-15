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
| Secciones 2.3 a 2.7 | `docs/seccion-2-3-pruebas.md` … `docs/seccion-2-7-contingencia.md`, que `docs/render_secciones.py` vuelca al Word desde el 14/09/2026 |
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

Actualizado el **15/09/2026**. Lo tachado de la lista anterior se hizo; lo que sigue es
lo que falta.

### Ya no queda pendiente

- **Las secciones 2.3 a 2.7 están en el Word** (14/09). `docs/render_secciones.py` las
  vuelca desde los markdown; `editar_proyecto.py` lo llama al final.
- **El script de capturas existe**: `docs/sacar_capturas.py`, 112 de 112 cubiertas.
- **1.11 Estimación del esfuerzo** está en el anteproyecto, con el conteo en
  `docs/estimacion_esfuerzo.py`, que se verifica solo contra el código.
- **2.9 Conclusiones** está escrita, con 16 marcas `[COMPLETAR]` para lo que sólo saben
  los autores.
- **Los reparos de forma** (15/09): palabras clave, RNF1 a RNF9 numerados, el subtítulo
  «Alcances», la línea de firmas que se colaba al índice, el «Indice» suelto del
  comienzo, el párrafo de criticidad del cronograma y los espacios invisibles de las
  viñetas.
- **La bibliografía cita libros**: Pressman y Elmasri-Navathe, que son de donde salen los
  riesgos, los ciclos de vida, el SQA, los tipos de prueba, los puntos de función, la
  normalización y el MER. Antes eran seis sitios sin autor ni URL.
- **H6, las credenciales versionadas** (15/09): `appsettings.json` va con las tres claves
  vacías y los valores se cargan con user-secrets o variables de entorno. `bd/LEEME.md`
  lo explica en los puntos 5, 6 y 7.
- **La etapa de pruebas de integración** quedó nombrada en 2.3, con las cuatro pruebas
  que cruzan módulos.

### A. `docs/armar_tesis.py`, la Fase 4 — lo hacés vos

Produce `Tesis.docx` desde el anteproyecto y el proyecto: el anteproyecto renumerado como
`1.x`, el proyecto como `2.x`, portada, declaración de autoría, abstract, palabras clave,
índice único, glosario, bibliografía y anexo. Está detallado en el punto 8 del plan
maestro.

Tres cosas que resuelve este paso y hoy no están:

- **El índice va como campo TOC de verdad**, con `\o "1-4"`: los subtítulos de 2.3 a 2.9
  van en Heading5 y Heading6 justamente para quedar afuera. Los números de página los
  calcula Word cuando alguien actualiza el campo, y eso hay que decírselo a los autores.
- **El índice del manual necesita el suyo**, acotado a su parte del documento
  (`TOC \b` sobre un marcador), porque hoy no tiene números de página.
- **La tabla de estado que hoy hace de índice en el Proyecto** —con su columna
  «Realizado / En proceso / Pendiente»— desaparece. No puede entregarse: le dice al
  tribunal en la página dos qué no se terminó.

### B. Lo que sólo pueden hacer los autores

- **Levantar el sistema** con `DatosPrueba.sql` recién cargado, correr
  `docs/sacar_capturas.py` y ejecutar las pruebas de 2.3 completando la columna
  «Resultado». Conviene en una sola sesión.
- **2.8 Grado de satisfacción del cliente** (una página, sin imágenes). No es una
  encuesta: es el relato de la relación con la encargada a lo largo de las iteraciones.
  Hace falta que le muestren el sistema y anoten lo que dice; con esas notas, la redactás
  vos. De la misma sesión sale el criterio del RNF1 de Usabilidad.
- **Las 16 marcas `[COMPLETAR]` de 2.9 y las 3 de 1.11**: cómo trabajó el equipo, cómo
  fue con el tutor y con la clienta, si hubo retrasos, y las horas dedicadas.
- **El anexo**: fotos de los cuadernos y del pizarrón con que hoy se lleva el tambo. Es
  la evidencia de la «Presentación del problema» y cuesta cinco minutos con un teléfono.
- **El organigrama** de la presentación del cliente: dueño, encargada, tamberos y
  veterinario externo. Cierra la sección de actores de un golpe de vista.

### C. Decisiones ya tomadas — no volver a abrirlas

- **RF3.4 y RF3.5 no se fusionan, y RF5.2 se queda** (15/09). Quedan **74
  requerimientos**. A esta altura renumerar toca los casos de uso, los diagramas de
  secuencia y el documento entero, y el beneficio es cosmético.
- **No hay pruebas automatizadas y no las va a haber.** Las pruebas son funcionales y
  manuales, sobre el sistema andando.
- **El reporte genético** no tiene período, pero sus secciones vacías dicen «Sin
  registros en el período»: la frase está fija en `Tesis/Reportes/GeneradorPdf.cs` y
  `GeneradorExcel.cs`. Es una línea de código si la quieren cambiar. **Sigue abierta.**

### D. Lo último, y sólo al final

**El repositorio de entrega**, nuevo y sin historia: el procedimiento completo está en
`docs/entrega-repositorio.md`. No se ejecuta hasta que todo lo demás esté terminado.

### Menor, cuando haya un rato

`flujos-de-prueba.md` ya usa los caminos del menú de hoy, pero todavía nombra algunas
pantallas por su título viejo —«Celos Detectados», «Tactos Pendientes»—. Si alguno no
coincide con el título real, se va a ver al ejecutar los flujos.

## 4. Por dónde empezar

Por **A**, que es lo único grande que queda del lado del documento, y no depende de
nadie. **B** se destraba el día que el autor levante el sistema.
