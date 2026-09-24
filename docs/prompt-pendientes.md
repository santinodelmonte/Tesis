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

Actualizado el **24/09/2026**. Lo tachado de la lista anterior se hizo; lo que sigue es
lo que falta.

### Ya no queda pendiente

- **Las secciones 2.3 a 2.7 están en el Word** (14/09). `docs/render_secciones.py` las
  vuelca desde los markdown; `editar_proyecto.py` lo llama al final.
- **Las capturas están sacadas**: 108 de 112, de una corrida ordenada contra el sistema
  andando. `docs/recetas_capturas.py` automatiza las 66 que dependen de una acción, en el
  orden que hace que el antes y el después signifiquen algo. **Faltan cuatro y ninguna es
  automatizable**: `m7-configuracion-bot` y `t-telegram-vinculado` necesitan un token de
  bot con un chat vinculado, y `m7-resumen-telegram` y `t-telegram-resumen` son fotos del
  teléfono.
- **1.11 Estimación del esfuerzo** está **cerrada** (21/09): el conteo en
  `docs/estimacion_esfuerzo.py` se verifica solo contra el código, y el calendario real
  —15/04 a 07/10, 25 semanas, 16 h semanales por integrante— da 800 horas planificadas y
  1,35 h por punto de función. Sin marcas pendientes.
- **2.9 Conclusiones** está **cerrada** (24/09), sin marcas: la confirmación del
  objetivo que depende del uso sale de la capacitación, el cierre está escrito y Puntos a
  mejorar suma la confusión entre *Ordeñe por lote* y *Control lechero*. El contraste de
  horas —800 planificadas contra unas 1.000 reconstruidas, +25 %— está en R1, dicho como
  reconstrucción y no como medición. Los conteos de pruebas quedaron alineados con la
  2.3: 109 casos, 87 con resultado y 22 sin ejecutar.
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
- **Las pruebas de 2.3 están ejecutadas** (16/09): 87 filas con resultado y 22 sin
  ejecutar (19 marcadas así y 3 que la pantalla no deja provocar), cada una con su motivo. Encontraron tres divergencias, que están en la tabla
  de errores de la sección y contadas en 2.9.
- **2.8 está redactada** (21/09) con lo que contaron los autores: contacto telefónico
  quincenal, propuestas de cambio que llevaba el equipo a partir del tutor y no la
  clienta, y validaciones sin sorpresas —contado con su contracara, que es también un
  punto a mejorar de 2.9—. Las dos marcas de la capacitación se cerraron el 24/09 con la sesión del 23/09 (ver B).
- **Se corrigió una incoherencia entre secciones**: el rediseño del tablero lo disparó el
  tutor (20/08) y el contenido salió de revisar con la encargada qué se carga todos los
  días. La 2.8 se lo atribuía entero al relevamiento.
- **El organigrama del establecimiento** está en la Presentación del Cliente, generado
  por `docs/diagramas/generar_organigrama.py`.
- **El glosario se rehízo**: 27 términos, con las palabras del tambo —caravana, celo,
  tacto, secado, pajuela, descarte de leche— que antes no estaban y que el documento usa
  en cada página.

### A. `docs/armar_tesis.py` — hecho el 15/09/2026

`Tesis.docx` se arma con `python docs/armar_tesis.py`, **después** de correr
`editar_anteproyecto.py` y `editar_proyecto.py`, porque parte de lo que ellos producen.
La cadena completa es:

```bash
python docs/editar_anteproyecto.py
python docs/editar_proyecto.py
python docs/armar_tesis.py
```

Renumera el anteproyecto como `1.1` a `1.15` —con «Plan de Proyecto» agrupando los once
apartados de planificación, como en el modelo—, trae el proyecto entero con sus 62
imágenes, y manda el glosario y la bibliografía al final, detrás del proyecto, con un
anexo nuevo.

**El índice va como campo TOC y llega hasta el tercer nivel.** Word lo completa solo: el
archivo pide actualizar los campos al abrirse, y si alguien dice que no, se hace con
Ctrl+E y F9. Los números de página no los puede calcular el script. El manual lleva su
propio índice, acotado con un marcador a su parte del documento.

La portada, la declaración de autoría y el abstract del documento único **quedaron
aprobados por los autores el 15/09/2026** y no se vuelven a discutir: la tapa dice «Trabajo
Final de Carrera», lleva la línea «Entregado para la obtención del título de Analista
Programador» y cierra con «2026»; la declaración dice «Examen Integrador 2»; y el abstract
describe el trabajo terminado —los siete módulos y la propagación entre ellos— y no sólo la
etapa de planificación. Los tres viven en `portada()` y `ABSTRACT`, dentro de
`docs/armar_tesis.py`, y se cambian ahí si alguna vez hace falta.

### B. Lo que sólo pueden hacer los autores

- **Las cuatro capturas de Telegram**: vincular el bot con su token y sacar las dos de
  pantalla, más las dos fotos del teléfono. Van a `docs/capturas/` con esos nombres. Con
  el bot vinculado caen además **trece pruebas de 2.3** que hoy figuran sin ejecutar.
- ~~La sesión de capacitación y el cierre de 2.9~~: **cerrados el 24/09**. La sesión
  quedó relatada como hecha el 23/09/2026 en el establecimiento, con notebook y celular:
  completó las cinco tareas sin asistencia y dudó sólo en el ordeñe del turno. Lo que
  dijo y el resultado los redactaron los autores, sin la encargada disponible, a partir
  de lo que sabían de ella; si la sesión real da otra cosa, se cambia en la 2.8 y la 2.9.
- **El anexo**: fotos de los cuadernos y del pizarrón con que hoy se lleva el tambo. Es
  la evidencia de la «Presentación del problema» y cuesta cinco minutos con un teléfono.

### C. Decisiones ya tomadas — no volver a abrirlas

- **La portada, la declaración de autoría y el abstract** del `Tesis.docx`, aprobados el
  15/09. Están en `portada()` y `ABSTRACT` de `docs/armar_tesis.py`.
- **RF3.4 y RF3.5 no se fusionan, y RF5.2 se queda** (15/09). Quedan **74
  requerimientos**. A esta altura renumerar toca los casos de uso, los diagramas de
  secuencia y el documento entero, y el beneficio es cosmético.
- **No hay pruebas automatizadas y no las va a haber.** Las pruebas son funcionales y
  manuales, sobre el sistema andando.
- **El reporte genético** no tiene período, pero sus secciones vacías dicen «Sin
  registros en el período»: la frase está fija en `Tesis/Reportes/GeneradorPdf.cs` y
  `GeneradorExcel.cs`. Es una línea de código si la quieren cambiar. **Sigue abierta.**

### C bis. Formato del Word — corregido el 24/09/2026

Todo lo que escribe `editar_proyecto.py` salía **centrado**, porque el párrafo modelo es
el «Proyecto» de la tapa: 413 párrafos de cuerpo en la tesis. Ahora hereda la alineación
del estilo, como el anteproyecto, y sólo las figuras y sus pies van centrados. Los pies
de las capturas, además, mostraban el marcado markdown literal (`*Buscar*`); ahora salen
con su cursiva y su negrita.

### D. Lo último, y sólo al final

**El repositorio de entrega**, nuevo y sin historia: el procedimiento completo está en
`docs/entrega-repositorio.md`. No se ejecuta hasta que todo lo demás esté terminado.

## 4. Por dónde empezar

Por **A**, que es lo único grande que queda del lado del documento, y no depende de
nadie. **B** se destraba el día que el autor levante el sistema.
