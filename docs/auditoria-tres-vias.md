# Auditoría de tres vías — anteproyecto, proyecto y código

Fase 2 del `prompt-documento-final.md`. La regla es que los tres digan exactamente
lo mismo. Ante la duda manda el código, **pero nada se aplica sin consultar**: esta
lista es para que la respondan, no para que el agente decida.

Hecha el **26/08/2026**, contra `HEAD` de `claude/thesis-project-document-prompt-nj7ffh`.

---

## 1. Qué se revisó y cómo

| Vía | Fuente |
|---|---|
| Anteproyecto | `Anteproyecto_v6.docx` — los **74 requerimientos funcionales**, extraídos con su texto completo |
| Proyecto | `docs/catalogo-casos-de-uso.md` — los **49 casos de uso** y sus referencias a RF |
| Código | `Tesis/Dominio/Controladora.cs` (252 métodos públicos), las 58 páginas, y `bd/CreacionDb.sql` |

Tres pasadas:

1. **Anteproyecto contra proyecto.** Automática: se extrajeron los RF que el
   anteproyecto define y los que los casos de uso referencian, y se compararon los
   dos conjuntos.
2. **Documento contra código.** Se leyeron los 74 RF y se verificó contra el código
   cada uno que **afirma un número o un comportamiento concreto** —los que pueden
   estar mal sin que se note—. Los que sólo dicen «el sistema debe permitir
   registrar X» se dan por cubiertos si existe la pantalla y el método.
3. **Código contra documento**, que es la dirección que se olvida: qué hace el
   sistema que ningún requerimiento describe.

Para la tercera pasada se usó un atajo que vale la pena anotar: **el anteproyecto
v6 se escribió leyendo el código** (commit `822a591`, «los requerimientos dicen lo
que el sistema hace»). Así que el riesgo no está en lo que ya estaba, sino en **lo
que el código cambió después**. Se comparó la superficie pública de la Controladora
entre aquel commit y hoy.

---

## 2. Lo que está alineado

**Anteproyecto y proyecto coinciden exactamente.** 74 requerimientos definidos, 74
referenciados por los casos de uso. Ni un RF huérfano, ni un caso de uso que invente
un requerimiento.

**La deriva del código desde la v6 es mínima y está contenida.** Ocho métodos nuevos,
**ninguno eliminado**:

| Método nuevo | Lo cubre |
|---|---|
| `ArmarReporteProductivo`, `ArmarReporteSanitario`, `ArmarReporteReproductivo`, `ArmarReporteGenetico` | RF7.1 a RF7.4 |
| `ListarReproductoresDeCatalogo` | RF3.3 |
| `ListarRodeo` | RF6.2 |
| `ValidarLoteContraMedido`, `ValidarMedidoContraLote` | **ningún RF** → hallazgo 2 |

**Los requerimientos que fijan números están todos bien.** Se verificaron uno por
uno contra las constantes del código:

| RF | Lo que afirma | Código |
|---|---|---|
| RF0.3 | Once parámetros configurables | Los once, en `Configuracion.cs` |
| RF2.2 | Que el lote no supere el máximo × cantidad de animales | `Controladora.cs:1779` |
| RF2.6 | Proyección lineal a 305 días | `DIAS_LACTANCIA_ESTANDAR = 305` |
| RF3.11 | Cinco advertencias que no bloquean | Las cinco, incluida la gestación fuera de 240–320 días |
| RF6.2 | Nueve indicadores | Los nueve, en `Indicadores.cshtml.cs` |
| RF6.3 | 70 % del promedio, 3 servicios, 150 días abiertos, 3 diagnósticos, 7 partos | Los cinco, en `MotivosDeDescarte` (`:6622`) |
| RF6.4 | Buscador desde cualquier pantalla | En `_Layout.cshtml:184` |
| RF5.10 | Contra-movimiento que devuelve el stock | `Controladora.cs:2593` |

Es un resultado bueno y conviene decirlo: **la parte difícil ya estaba hecha.** Lo
que sigue son cinco hallazgos, y ninguno es grave.

---

## 3. Hallazgos

### H1 — RF3.10 dice que el celo y el servicio comparten la misma edad mínima. No la comparten.

**El anteproyecto dice:** «El sistema debe impedir el registro de un celo o de un
servicio en un animal que no alcanzó **la edad mínima al servicio**».

**El código usa dos umbrales distintos:**

| | Constante | Valor | ¿Configurable? |
|---|---|---|---|
| Celo | `EDAD_MINIMA_CELO_MESES` (`:67`, `:2782`) | **9 meses** | no |
| Servicio | `Parametros().EdadMinimaServicioMeses` (`:3000`) | **13 meses** por defecto | **sí** |

Y el código explica por qué, en un comentario que es una razón de negocio, no de
implementación: *«la vaquillona empieza a manifestar celo bastante antes de que el
servicio sea conveniente»*. El celo se detecta y se anota; servirla a los 9 meses
sería otra cosa.

**El código tiene razón.** El RF está mal redactado, no el sistema.

> **Propuesta:** reescribir RF3.10 como «…impedir el registro de un celo en un animal
> que no alcanzó la edad mínima de detección de celo, y de un servicio en uno que no
> alcanzó la edad mínima al servicio configurada, y el de cualquier evento con fecha
> posterior a la baja del animal».
>
> **Pregunta:** ¿se aprueba la redacción? Toca también CU20 y CU21, que referencian
> RF3.10.

### H2 — El sistema valida algo que ningún requerimiento pide

`ValidarLoteContraMedido` y `ValidarMedidoContraLote` (`:2328`, `:2348`) verifican la
coherencia entre el ordeñe del lote y la suma de los controles individuales del mismo
turno: que los controles no sumen más de lo que dio el tanque, y que un control nuevo
entre en el total del turno.

Es una validación **buena** —cierra el circuito entre las dos formas de medir— pero
**no está en ningún RF**. RF2.2 enumera las validaciones de producción y no la
incluye; RF2.3 dice que el control lechero «convive con el registro por lote, sin
sumarse a él», que es la convivencia, no la verificación cruzada.

> **Propuesta:** agregarla a RF2.2, que es la lista de validaciones de producción:
> «…y que los controles individuales de un turno no superen el total registrado para
> ese turno por lote».
>
> **Pregunta:** ¿va en RF2.2 o prefieren un RF2.14 nuevo? Va en RF2.2, salvo que
> quieran que se vea como funcionalidad aparte en la lista de requerimientos.

### H3 — Hay una pantalla que nadie pidió

`Tesis/Pages/Privacy.cshtml` es **la página de la plantilla de ASP.NET, sin tocar**:

```
<h1>Privacy Policy</h1>
<p>Use this page to detail your site's privacy policy.</p>
```

En inglés, con el texto de ejemplo de Microsoft. No está en el menú, pero **la ruta
responde**: cualquiera que escriba `/Privacy` la ve. Ningún caso de uso la describe y
ningún requerimiento la pide.

> **Propuesta:** borrarla, junto con `Privacy.cshtml.cs`. Es un cambio de código, no
> de documento, y por eso se pregunta antes.
>
> **Pregunta:** ¿la borramos? Si por alguna razón la cátedra pide una política de
> privacidad, hay que escribirla en serio y darle su RF; dejarla como está es la
> única opción que no sirve.

### H4 — El parámetro «edad mínima al servicio» no alcanza a los machos

RF0.3 presenta «edad mínima al servicio» como un parámetro configurable, sin
distinguir sexos. Pero en `CalcularCategoria` (`:957`) la categoría **Toro** se
decide con `EDAD_MINIMA_SERVICIO_MESES` (`:24`, usada en `:981`), que es una
**constante de 15 meses**, no el parámetro.

O sea: si la encargada cambia el parámetro a 20 meses, cambia cuándo puede servir a
una vaquillona, pero un macho sigue pasando a Toro a los 15.

El código lo hace a propósito y lo dice: *«la hembra y el macho entran en servicio a
edades distintas… exigirle los 15 del toro rechazaba partos legítimos»*.

> **Propuesta:** aclarar en RF0.3 que el parámetro es la edad mínima al servicio **de
> la hembra**, y dejar los 15 meses del macho como constante del dominio, mencionada
> en RF1.8.
>
> **Pregunta:** ¿se aprueba? Es la misma familia que H1: el sistema distingue dos
> cosas que el documento nombra con una sola palabra.

### H5 — El documento usa dos palabras para la misma categoría

`bd/DatosPrueba.sql` y `docs/flujos-de-prueba.md` hablan de **vaquillonas**. El código
y la tabla `categorias` sólo conocen **Novilla**. Las dos son correctas en el tambo y
significan lo mismo: hembra de más de 12 meses sin partos.

> **Propuesta:** el documento usa **Novilla** —que es lo que la usuaria ve en
> pantalla— y el glosario declara *vaquillona* como sinónimo de uso corriente. Los
> comentarios del código y los datos de prueba pueden quedar como están.
>
> **Pregunta:** ¿de acuerdo, o prefieren al revés?

---

## 4. El Módulo 7, que está a mitad de camino

No es un hallazgo: es trabajo en curso. Pero define qué se puede escribir hoy.

| RF | Estado |
|---|---|
| RF7.1 Reportes productivos | **hecho** — `PagesReportes/ReporteProductivo`, con `GeneradorPdf` y `GeneradorExcel` |
| RF7.2 Reportes sanitarios | **hecho** |
| RF7.3 Reportes reproductivos | **hecho** |
| RF7.4 Reportes genéticos | **hecho** |
| RF7.5 Integración con Telegram | **falta** — no hay una línea de Telegram en el código |
| RF7.6 Notificaciones automáticas | **falta** |
| RF7.7 Resumen diario | **falta** — las 37 líneas nuevas de `Program.cs` son cultura y codificación, no un proceso programado |

Las dos tablas del Módulo 7 tampoco están: la última de `bd/CreacionDb.sql` sigue
siendo `configuracion`. **Cuando el bot esté, hay que volver a correr esta auditoría
sobre RF7.5 a RF7.7 y nada más.**

---

## 5. Lo que queda por verificar a nivel de línea

Para ser exactos sobre el alcance de esta pasada: se verificaron contra el código
**todos los RF que afirman un número o un comportamiento concreto**. Los que dicen
«el sistema debe permitir registrar X» se dieron por cubiertos con la existencia de
la pantalla y del método, sin seguir cada campo.

Quedan en ese grupo, y conviene mirarlos al escribir el manual —que obliga a recorrer
cada pantalla campo por campo—: **RF1.14** (las tres validaciones de genealogía
imposible), **RF4.5** (que el calendario calcule pendientes *y* vencidos), **RF4.9**
(que las cuatro entidades sanitarias se puedan corregir y eliminar) y **RF5.8** (que
el consumo se impute a la partida que vence primero).

Escribir el manual **es** la forma de terminar esta auditoría: no hay mejor manera de
verificar que el documento dice lo que el sistema hace que documentar pantalla por
pantalla lo que la pantalla muestra.
