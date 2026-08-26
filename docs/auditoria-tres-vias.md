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

**RESUELTO — aprobado el 26/08, con la condición de que el RF sea medible.** Queda:

> **RF3.10 Validaciones reproductivas:** El sistema debe impedir el registro de un
> celo en un animal que no alcanzó la **edad mínima de detección de celo, de 9
> meses**; el de un servicio en un animal que no alcanzó la **edad mínima al
> servicio configurada, de 13 meses por defecto**; y el de cualquier evento cuya
> fecha sea posterior a la fecha de baja del animal.

Los dos umbrales quedan escritos con su número, que es lo que lo vuelve verificable:
se puede tomar un animal, mirar su edad y decir si el sistema cumple o no. Toca
también CU20 y CU21, que referencian RF3.10.

### H2 — El sistema valida algo que ningún requerimiento pide

`ValidarLoteContraMedido` y `ValidarMedidoContraLote` (`:2328`, `:2348`) verifican la
coherencia entre el ordeñe del lote y la suma de los controles individuales del mismo
turno: que los controles no sumen más de lo que dio el tanque, y que un control nuevo
entre en el total del turno.

Es una validación **buena** —cierra el circuito entre las dos formas de medir— pero
**no está en ningún RF**. RF2.2 enumera las validaciones de producción y no la
incluye; RF2.3 dice que el control lechero «convive con el registro por lote, sin
sumarse a él», que es la convivencia, no la verificación cruzada.

**RESUELTO — aprobado el 26/08, va en RF2.2.** Queda:

> **RF2.2 Validación de producción:** El sistema debe validar que los litros
> ingresados sean valores positivos; que no superen el **máximo configurado por
> control individual, de 100 litros por defecto**; que en el registro por lote no
> superen ese máximo multiplicado por la cantidad de animales del lote; y que **la
> suma de los controles individuales de un turno no supere el total registrado para
> ese turno por lote**, ni un control nuevo haga que lo supere.

### H3 — Hay una pantalla que nadie pidió

`Tesis/Pages/Privacy.cshtml` es **la página de la plantilla de ASP.NET, sin tocar**:

```
<h1>Privacy Policy</h1>
<p>Use this page to detail your site's privacy policy.</p>
```

En inglés, con el texto de ejemplo de Microsoft. No está en el menú, pero **la ruta
responde**: cualquiera que escriba `/Privacy` la ve. Ningún caso de uso la describe y
ningún requerimiento la pide.

**RESUELTO — aprobado el 26/08. Borrada.** `Privacy.cshtml` y `Privacy.cshtml.cs`
salieron del proyecto. No las referenciaba nadie: ni el menú, ni una redirección, ni
un test.

### H4 — El parámetro «edad mínima al servicio» no alcanza a los machos

RF0.3 presenta «edad mínima al servicio» como un parámetro configurable, sin
distinguir sexos. Pero en `CalcularCategoria` (`:957`) la categoría **Toro** se
decide con `EDAD_MINIMA_SERVICIO_MESES` (`:24`, usada en `:981`), que es una
**constante de 15 meses**, no el parámetro.

O sea: si la encargada cambia el parámetro a 20 meses, cambia cuándo puede servir a
una vaquillona, pero un macho sigue pasando a Toro a los 15.

El código lo hace a propósito y lo dice: *«la hembra y el macho entran en servicio a
edades distintas… exigirle los 15 del toro rechazaba partos legítimos»*.

**RESUELTO — aprobado el 26/08.** Queda, con los números a la vista:

> **RF0.3** — el parámetro pasa a llamarse «edad mínima al servicio **de la
> hembra**».
>
> **RF1.8 Clasificación automática:** El sistema debe clasificar automáticamente a
> los animales según su sexo, su edad y su condición reproductiva. La hembra es
> **ternera** hasta la edad de cambio de categoría configurada —**12 meses** por
> defecto—, **vaquillona** desde esa edad y mientras no tenga partos registrados, y
> **vaca** desde su primer parto. El macho es **ternero** hasta esa misma edad,
> **toro** si supera los **15 meses** e integra el rodeo como reproductor, y
> **novillo** en cualquier otro caso.

### H5 — El documento usa dos palabras para la misma categoría

`bd/DatosPrueba.sql` y `docs/flujos-de-prueba.md` hablan de **vaquillonas**. El código
y la tabla `categorias` sólo conocen **Novilla**. Las dos son correctas en el tambo y
significan lo mismo: hembra de más de 12 meses sin partos.

**RESUELTO al revés de lo propuesto — aprobado el 26/08: se usa «vaquillona».** El
uso corriente en el tambo es *vaquillona*, y el documento habla el idioma de la
usuaria.

> **Y eso obliga a cambiar el sistema, no sólo el documento.** Hoy la pantalla
> muestra **Novilla**: si el documento dice *vaquillona* y la captura dice *Novilla*,
> volvemos a tener las tres patas diciendo cosas distintas, que es justo lo que esta
> auditoría existe para evitar.
>
> Por suerte el cambio es de dos líneas: `Controladora.cs:971` (`vNombre =
> "Novilla"`) y la fila de la tabla `categorias` en `bd/CreacionDb.sql:526`. Las
> demás tablas apuntan a la categoría por `id_categoria`, así que renombrar la
> etiqueta no toca un solo dato.
>
> **Queda pendiente de que lo confirmen**, porque es un cambio de código y de datos
> que no estaba en la pregunta original. El glosario declara **novilla** como
> sinónimo.

### H6 — Las credenciales viajan con el código, y el propio anteproyecto dice que no deberían

Apareció al escribir la sección 2.6, y es el hallazgo más serio de los seis.

**El RNF de Seguridad dice**, con todas las letras: *«Las credenciales de acceso y la
cadena de conexión a la base de datos **no deben residir en el código fuente**, y las
consultas deben construirse de forma parametrizada»*.

La segunda mitad **se cumple sin fisuras**: hay 78 parámetros `@nombre` distintos en
la capa de persistencia y **un único punto** donde se cargan valores
(`pConexion.cs:139`). No hay una sola consulta armada concatenando entrada del
usuario. Ese RNF se puede defender.

La primera mitad, no. `Tesis/appsettings.json` está **versionado en el repositorio**:

```json
"ConnectionStrings": { "Tambo": "server=localhost; ...; uid=root; pwd=; ..." },
"Seguridad": { "Usuario": "sofia", "Contrasena": "tambo2026" }
```

En texto plano, sin hash, y **en el historial de Git desde el commit `41f50db`** —o
sea que borrarlo del archivo hoy no lo saca del historial.

**La arquitectura ya es la correcta, y eso achica el problema.** `Program.cs:62-66`
lee la cadena de conexión y las credenciales de la configuración y se las pasa a
`pConexion.Configurar` y a `Controladora.ConfigurarCredenciales`; el comentario que
tiene encima dice, textual, *«no estan escritos en el codigo fuente»*. El código hace
lo que el RNF pide.

**El defecto es de una sola línea: el repositorio publica los valores.** No hay que
refactorizar nada, sólo dejar de versionar los datos reales.

> **Propuesta:** dejar en `appsettings.json` sólo los marcadores, y que los valores
> reales vengan de fuera —el panel del hosting en producción, `dotnet user-secrets`
> en desarrollo—. No hay que tocar una línea de lógica: el mecanismo de lectura ya
> está y sigue funcionando igual.
>
> **Pregunta:** ¿lo hacemos? Es la única de las seis que cambia el comportamiento del
> despliegue, y **la sección 2.6 no se puede terminar hasta decidirlo**: o el código
> se acomoda al RNF, o hay que reescribir el RNF para que diga lo que el sistema
> hace. Lo primero es mejor y es barato.
>
> Aparte, y aunque se arregle: la contraseña `tambo2026` estuvo en un repositorio.
> **Conviene cambiarla antes de la entrega**, y no reutilizarla en el hosting.

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
