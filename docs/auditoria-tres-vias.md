# Auditoría de tres vías — anteproyecto, proyecto y código

Fase 2 del `prompt-documento-final.md`. La regla es que los tres digan exactamente
lo mismo. Ante la duda manda el código, **pero nada se aplica sin consultar**: esta
lista es para que la respondan, no para que el agente decida.

Primera pasada el **26/08/2026**, contra `HEAD` de
`claude/thesis-project-document-prompt-nj7ffh`. **Segunda pasada el 08/09/2026**,
contra `master`: el Módulo 7, que en agosto todavía no existía, y los cuatro
requerimientos que habían quedado sin verificar a nivel de línea. Los hallazgos
nuevos son H9 a H13.

---

## 1. Qué se revisó y cómo

| Vía | Fuente |
|---|---|
| Anteproyecto | `Anteproyecto_v6.docx` en la primera pasada, `Anteproyecto_v7.docx` en la segunda — los **74 requerimientos funcionales**, extraídos con su texto completo |
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

**En la segunda pasada la medición se repitió, y sigue sin eliminarse un solo método.**
Contra el mismo commit `822a591` hay hoy **diecinueve métodos nuevos**: los ocho de
arriba, once del Módulo 7 y `ValidarTacto`.

| Método nuevo desde el 26/08 | Lo cubre |
|---|---|
| `VincularTelegram`, `ValidarChatTelegram` | RF7.5 |
| `ListarPreferencias`, `ModificarNotificaciones`, `ValidarHoraResumen` | RF7.6 → H10 y H11 |
| `GenerarAlertasDelDia`, `ArmarMensajeResumen`, `RegistrarEnvioResumen`, `ResumenEnviado`, `ContarAlertas` | RF7.7 |
| `ValidarTacto` | RF3.4 y RF3.5 — la validación del tacto, extraída del alta porque el registro rápido del tablero la necesita por separado |

**Los requerimientos que fijan números están todos bien.** Se verificaron uno por
uno contra las constantes del código:

| RF | Lo que afirma | Código |
|---|---|---|
| RF0.3 | Once parámetros configurables | Los once, en `Configuracion.cs` — y desde el Módulo 7 hay un doceavo que el RF no nombra → **H11** |
| RF2.2 | Que el lote no supere el máximo × cantidad de animales | `Controladora.cs:1779` |
| RF2.6 | Proyección lineal a 305 días | `DIAS_LACTANCIA_ESTANDAR = 305` |
| RF3.11 | Cinco advertencias que no bloquean | Las cinco, incluida la gestación fuera de 240–320 días |
| RF6.2 | Nueve indicadores | Los nueve, en `Indicadores.cshtml.cs` |
| RF6.3 | 70 % del promedio, 3 servicios, 150 días abiertos, 3 diagnósticos, 7 partos | Los cinco, en `MotivosDeDescarte` (`:6622`) |
| RF6.4 | Buscador desde cualquier pantalla | En `_Layout.cshtml:184` |
| RF5.10 | Contra-movimiento que devuelve el stock | `Controladora.cs:2593` |

Es un resultado bueno y conviene decirlo: **la parte difícil ya estaba hecha.** Lo
que sigue son trece hallazgos —ocho de la primera pasada, cinco de la segunda— y
ninguno es grave. **Doce de los trece se corrigen escribiendo, no programando**; el
único que pide tocar código es H5, y porque así se resolvió.

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

**APLICADO al sistema el 08/09/2026.** Era el único de los trece hallazgos que pedía
tocar código, porque la palabra sale en pantalla: si el documento dice *vaquillona* y
la captura dice *Novilla*, volvemos a tener las tres patas diciendo cosas distintas,
que es justo lo que esta auditoría existe para evitar.

El cambio fue de dos líneas de comportamiento más cuatro comentarios:

| Archivo | Qué cambió |
|---|---|
| `Tesis/Dominio/Controladora.cs:1009` | `vNombre = "Vaquillona"` en `CalcularCategoria` |
| `bd/CreacionDb.sql:558` | la fila semilla de `categorias` |
| `Controladora.cs:4073` y `:4467`, `pAnimal.cs:167`, `pParto.cs:250` | los comentarios de RF1.9, que decían «vuelve a ser novilla» |

**No se tocó un solo dato.** Las demás tablas apuntan a la categoría por
`id_categoria` y `bd/DatosPrueba.sql` también —usa los ids 2 y 3 y ningún nombre—, así
que la vaquillona sigue siendo la categoría 2. La búsqueda de `CalcularCategoria` es
por nombre contra la tabla (`:1034`), y por eso los dos cambios van juntos o ninguno:
con uno solo, el cálculo de categoría devolvería `null` para toda hembra de más de doce
meses sin partos.

`dotnet build` sale con 0 errores después del cambio. El glosario del documento declara
**novilla** como sinónimo que no se usa.

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

### H7 — La verificación de consanguinidad llega hasta los abuelos, y RF1.7 sugiere más

Apareció al escribir los casos de borde de la sección 2.3.

**RF1.7 dice:** «verificando si existe un ancestro común dentro de **la ascendencia
registrada** de ambos». Leído así, la verificación abarca toda la genealogía cargada.

**Lo que hace el código.** `ListarAscendencia` (`:1037`) recorre exactamente **dos
generaciones**: los padres del animal y los padres de cada uno de ellos. Con el propio
animal, la lista llega a siete integrantes y ahí termina. El comentario de
`BuscarAncestroComun` lo dice sin vueltas: *«Hay parentesco directo si coincide algun
ancestro hasta el nivel de los abuelos»*.

**La consecuencia concreta:** dos animales emparentados por un **bisabuelo compartido**
no se detectan. Para las decisiones de cruza del establecimiento el alcance alcanza —el
parentesco cercano es el que importa— pero el requerimiento promete más de lo que hay.

> **Propuesta:** precisar RF1.7 —«…verificando si existe un ancestro común entre los
> progenitores y los abuelos de ambos»— y sumarlo a las Limitaciones, donde ya están la
> proyección lineal a 305 días y los umbrales fijos del descarte. **No tocar el código**:
> ampliar la profundidad cambiaría el comportamiento del sistema por una razón
> documental, que es exactamente al revés de como debe ser.
>
> **Pregunta:** ¿se aprueba? Es de la misma familia que H1 y H4: el documento dice en
> general lo que el sistema hace acotado.

**Ya se corrigió el manual**, que afirmaba que el sistema detecta parentesco «en
cualquier nivel». Era falso y lo escribí yo en la sección 2.4; ahora dice hasta los
abuelos.

### H8 — El anteproyecto promete convenciones de versionado que el repositorio no usa

La sección **Control de Versionado** describe cómo se trabaja con Git. Es de las pocas
partes del documento que **un tribunal puede verificar en treinta segundos**, abriendo
el repositorio en GitHub. Hoy no coincide en tres puntos:

| El anteproyecto dice | El repositorio tiene |
|---|---|
| «La rama principal (**main**) contendrá únicamente versiones estables» | La rama principal se llama **`master`** |
| Ramas nombradas `feature/gestion-animales`, `feature/control-sanitario`, `feature/reportes`, `feature/reproduccion` | **Ninguna rama `feature/`.** Las once ramas siguen el patrón `claude/<descripción>-<sufijo>` |
| «Las versiones estables serán identificadas mediante **numeración incremental**» | **No hay un solo tag** en el repositorio |

Los ejemplos de ramas son especialmente delicados porque están **escritos uno por uno
en el documento**: no es una convención general que se pueda leer con flexibilidad, son
cuatro nombres concretos que no existen.

> **Propuesta:** corregir el documento, no el repositorio. Que diga que la rama estable
> es `master`, que las ramas de trabajo se nombran por la funcionalidad que desarrollan
> —con los nombres reales como ejemplo— y que las versiones se identifican por los
> documentos entregados (v5, v6, v7) en lugar de por tags.
>
> **La alternativa es peor:** renombrar la rama principal y crear tags a esta altura
> sería maquillar el repositorio para que se parezca al documento, que es al revés de
> como venimos trabajando.
>
> **Pregunta:** ¿se aprueba? Si además quieren empezar a etiquetar las entregas con
> tags de ahora en más, eso sí tiene valor propio y se puede documentar como práctica
> adoptada.

### H9 — RF7.6 enumera siete avisos y el sistema manda ocho

**El anteproyecto dice** que el sistema envía alertas sobre «procedimientos sanitarios
pendientes, partos próximos, tactos pendientes, secados próximos, stock crítico,
vencimiento de insumos y fin del período de descarte de leche». Son **siete**.

**El código tiene ocho**, y la lista es cerrada a propósito —`PreferenciaNotificacion.Tipos()`
y el `INSERT` de `preferencias_notificacion` en `bd/CreacionDb.sql:637`—:

| Tipo en el código | ¿Está en RF7.6? |
|---|---|
| `Sanitario pendiente` | sí |
| `Parto proximo` | sí |
| `Tacto pendiente` | sí |
| **`Vaca para servir`** | **no** |
| `Secado proximo` | sí |
| `Fin de descarte` | sí |
| `Stock critico` | sí |
| `Vencimiento de insumo` | sí |

El que falta no es un aviso menor: es el que abre el ciclo reproductivo —la vaca que
pasó la espera voluntaria y todavía no fue servida—, sale de `ListarVacasParaServir` y
es uno de los ocho contadores del tablero de inicio.

**El código tiene razón.** El RF se escribió cuando el Módulo 7 no existía y la lista
salió de memoria, no del tablero.

> **Propuesta:** agregar «vacas en condiciones de ser servidas» a la enumeración de
> RF7.6. Es un aviso más en una lista, no cambia nada más.

### H10 — RF7.6 no dice por dónde salen las alertas, y que se puedan apagar no lo pide ningún requerimiento

Dos cosas de la misma redacción.

**Por dónde salen.** RF7.6 dice «enviar alertas automáticas» y RF7.7, «enviar un
resumen diario». Leídos juntos parecen **dos canales**: un aviso cuando el pendiente
aparece, y además un resumen. **El sistema tiene uno solo**: los ocho tipos se arman en
`GenerarAlertasDelDia` (`:7502`) y salen agrupados por módulo dentro del mensaje
diario, o a pedido con el comando `/resumen`. No hay envío en el momento del evento.

Y está bien que no lo haya: un pendiente no «ocurre» en un instante —la vaca entra en
la lista de tactos porque pasaron los días, no porque alguien apretó algo—, así que no
hay un momento en el que disparar el aviso. El día es la unidad natural.

**Que se puedan apagar.** La pantalla de Notificaciones deja activar y desactivar cada
uno de los ocho tipos, y apagarlo lo saca del mensaje sin sacarlo del sistema. Es la
mitad de CU48 y **ningún RF lo describe**: RF7.5 habla del destinatario y del mensaje
de prueba, RF7.6 de qué se avisa, ninguno de que se pueda elegir. Es de la familia de
H2 —el sistema hace algo bueno que el documento no pide—.

> **Propuesta:** RF7.6 pasa a decir que el sistema debe enviar automáticamente, **en el
> resumen diario**, alertas sobre los ocho tipos, y **permitir activar o desactivar
> cada tipo por separado**. Con eso RF7.6 y RF7.7 dejan de leerse como dos canales y la
> selección deja de ser una función huérfana.

### H11 — La hora del resumen es configurable y ningún requerimiento lo dice

`configuracion` tiene `hora_resumen` (`TIME`, `07:00` por defecto), se edita desde la
pantalla de Notificaciones, se valida en `ValidarHoraResumen` (`:7444`) y el proceso la
relee cada cinco minutos.

**RF0.3 enumera once parámetros configurables** y los once están —lo verificó la pasada
del 26/08—. Éste es el **doceavo**, y no está en RF0.3 ni en RF7.5 ni en RF7.7. Se le
escapó a las dos vías: RF0.3 se escribió antes del Módulo 7, y la revisión de RF7.5 se
concentró en el mensaje de prueba.

Que viva en otra pantalla es correcto —se configura una vez, junto con el destinatario,
y no es un parámetro de manejo del rodeo—, pero el documento tiene que decirlo en algún
lado.

**En la misma bolsa:** el comando **`/resumen`**, que devuelve las tareas del día a
pedido y sólo al chat vinculado, tampoco lo pide ningún RF.

> **Propuesta:** que RF7.7 diga «…un resumen diario de las tareas pendientes **a la hora
> configurada**, y permitir consultarlo a demanda desde el mismo canal». Los dos
> agregados entran en una sola frase y no tocan RF0.3, que sigue siendo de parámetros de
> manejo.

### H12 — RF1.14 dice «la edad mínima al servicio»; el código exige esa edad más la gestación, y con dos umbrales distintos

**El anteproyecto dice** que se impide «un progenitor cuya fecha de nacimiento no
admita la edad mínima al servicio».

**El código exige más que eso**, y con razón: para ser progenitor no alcanza con haber
tenido edad de servicio, hay que haber tenido edad de servicio **nueve meses antes de
que naciera la cría** (`ValidarGenealogia`, `:846`):

| | Fórmula | Valor por defecto | ¿Configurable? |
|---|---|---|---|
| Madre | `Parametros().EdadMinimaServicioMeses + GESTACION_MESES` | 13 + 9 = **22 meses** | **sí**, el sumando de la edad |
| Padre | `EDAD_MINIMA_SERVICIO_MESES + GESTACION_MESES` (`:24`, `:26`) | 15 + 9 = **24 meses** | no |

Es exactamente el mismo par de problemas que H1 y H4 juntos: el documento dice «la edad
mínima al servicio» en singular cuando hay dos umbrales, y omite la gestación. Y la
asimetría no es un descuido: la constante de 15 meses del macho es la misma que decide
la categoría Toro, y ya quedó documentada en RF1.8 por H4.

**El código tiene razón.** El RF describe menos de lo que el sistema controla.

> **Propuesta:** RF1.14 pasa a decir «…un progenitor cuya fecha de nacimiento no admita
> **la edad mínima al servicio más los nueve meses de gestación —22 meses para la madre,
> con la edad configurable, y 24 para el padre—**». Queda medible: se toman dos fechas
> de nacimiento y se sabe si el sistema cumple.

### H13 — RF1.14 promete advertir por un progenitor «dado de baja»; el sistema advierte por uno que ya lo estaba en la fecha que corresponde

**El anteproyecto dice** que el sistema «debe advertir cuando el progenitor elegido se
encuentre dado de baja». En presente: si hoy está de baja, avisa.

**El código no hace eso**, y no debería (`AdvertenciasGenealogia`, `:900`):

| Progenitor | Cuándo advierte |
|---|---|
| Madre | si figuraba de baja **antes de la fecha de nacimiento de la cría** |
| Padre | si figuraba de baja **antes de la concepción**, o sea nueve meses antes |

O sea que una vaca vendida el año pasado **no genera advertencia** al cargarle un
ternero que parió hace tres años, y así tiene que ser: la mitad del rodeo histórico
está de baja y una advertencia en cada carga es una advertencia que nadie lee.

La distinción del padre es más fina todavía, y el código la explica: *«el semen
congelado sigue sirviendo años después de que el toro murió»*. Por eso el texto de esa
advertencia termina con «Es correcto si la cría vino de una pajuela suya».

**El código tiene razón.**

> **Propuesta:** RF1.14 pasa a decir «…y debe advertir cuando el progenitor elegido
> **figurara dado de baja antes de la fecha en que debió engendrar a la cría —el parto
> para la madre, la concepción para el padre—**». Es la misma familia que H1, H4, H7 y
> H12: el documento dice en general lo que el sistema hace acotado.
>
> **Y de paso, una imprecisión menor del mismo RF:** «un progenitor que figure en su
> propia descendencia» se lee mal —«su» cae sobre «un progenitor», y un animal en su
> propia descendencia es otra cosa—. Lo que el código impide es que el progenitor
> elegido descienda **del animal** que se está cargando (`ListarDescendencia`, `:805`,
> que recorre la descendencia completa y no sólo los hijos directos). Conviene escribirlo
> así.

---

## 4. El Módulo 7, auditado

La pasada del 26/08 dejó esto escrito: *«Cuando el bot esté, hay que volver a correr
esta auditoría sobre RF7.5 a RF7.7 y nada más»*. El bot está desde el 23/08 —y su
manejo de errores se corrigió el 08/09—, así que se corrió.

| RF | Estado | Verificado contra |
|---|---|---|
| RF7.1 a RF7.4 Reportes | **cumple**, ya estaba | `PagesReportes/`, `GeneradorPdf`, `GeneradorExcel` |
| RF7.5 Canal de notificaciones | **cumple** | `PagesNotificaciones/Notificaciones.cshtml.cs`, `VincularTelegram` (`:7483`) |
| RF7.6 Notificaciones automáticas | **cumple, con dos correcciones de redacción** | `GenerarAlertasXTipo` (`:7522`) → H9 y H10 |
| RF7.7 Resumen diario | **cumple, con una corrección de redacción** | `ServicioNotificaciones.RevisarResumen` → H11 |

**Las dos tablas que faltaban están.** `bd/CreacionDb.sql` termina hoy en
`preferencias_notificacion` y `alertas`, y `configuracion` sumó `hora_resumen`,
`chat_telegram` y `fecha_ultimo_resumen`. La observación de la pasada anterior —«la
última de `CreacionDb.sql` sigue siendo `configuracion`»— quedó saldada.

**RF7.5 se cumple en el orden correcto, que era lo difícil.** El requerimiento pide
verificar la conexión «mediante un mensaje de prueba **antes de darla por activa**», y
`OnPostVincular` manda el mensaje **primero** y guarda **después**: si Telegram
rechaza, la configuración anterior queda intacta, que es el curso de excepción 3a de
CU48. Lo fácil habría sido guardar y probar, y entonces un identificador mal copiado
dejaba el sistema apuntando a un chat que no existe.

**Lo que el resumen no hace, y es deliberado:** no calcula nada propio.
`GenerarAlertasDelDia` recorre las preferencias activas y le pide a la Controladora
**las mismas listas que alimentan el tablero de inicio**. Por eso el mensaje y la
pantalla no pueden discrepar en un número, que es la regla de negocio de CU49.

**Los tres hallazgos son de redacción del documento y ninguno pide tocar código:** H9
(falta un aviso en la enumeración), H10 (no dice que salen en el resumen, ni que se
pueden apagar) y H11 (la hora configurable y el `/resumen` a pedido no están en ningún
RF).

---

## 5. Lo que quedaba por verificar a nivel de línea, verificado

La pasada del 26/08 dejó cuatro requerimientos dados por cubiertos «con la existencia
de la pantalla y del método», sin seguir cada campo. Se siguieron.

### RF1.14 Validación del árbol genealógico — **cumple, con dos correcciones de redacción**

Las tres imposibilidades están en `ValidarGenealogia` (`:846`), que corre en el alta
(`:530`), en la modificación (`:582`) y en la cría que nace de un parto (`:4012`,
`:4296`):

| Lo que el RF promete | Dónde está | |
|---|---|---|
| Un animal como progenitor de sí mismo | `:850` a `:857`, madre y padre por separado | ✔ |
| Un progenitor que figure en la descendencia | `:859` a `:872`, con `ListarDescendencia` recursiva | ✔ |
| Un progenitor cuya edad no admita la concepción | `:875` a `:887` | ✔ **pero el RF dice menos que el código → H12** |
| Advertir por el progenitor dado de baja | `AdvertenciasGenealogia` (`:900`), sin bloquear | ✔ **con distinto criterio del que el RF anuncia → H13** |

La separación entre lo que bloquea y lo que sólo advierte está explicada en el código y
es la decisión correcta: las imposibilidades trancan, y lo que en un tambo real puede
ser cierto aunque parezca un error —una madre dada de baja, un padre muerto que dejó
pajuelas— avisa y deja guardar, porque trabarlo haría imposible la carga inicial del
rodeo.

### RF4.5 Calendario sanitario — **cumple entero**

| Lo que el RF promete | Dónde está |
|---|---|
| Vacunaciones, desparasitaciones **y** descornes | `UltimaAplicacion` (`:6792`) ramifica por los tres tipos de `PlanSanitario` y busca en `vacunaciones`, `tratamientos` y `descornes` respectivamente |
| Pendientes **y vencidos** | `CalcularPendientes` (`:6867`) sólo pone cota superior —`vProxima > vLimite` descarta lo lejano—, así que **todo lo atrasado entra**; `EstaVencido` (`:6940`) los distingue y quedan primero por orden de fecha |
| Calculados a partir de los planes configurados | `ListarPlanesActivos` y `PlanAlcanzaAnimal` (`:6752`): edad de inicio y categorías alcanzadas |
| …y de las aplicaciones ya registradas | `ProximaAplicacion` (`:6843`): nunca aplicado → el día en que el animal alcanza la edad de inicio; ya aplicado → última aplicación más la periodicidad |

Dos exclusiones que el RF no menciona y que **no lo contradicen**: el animal dado de
baja no figura (`!pAnimal.Activo`) y el toro de catálogo tampoco, porque no está en el
campo. La segunda tiene su comentario en el código y nació de un error real: sin ella
los toros de catálogo encabezaban el calendario con años de atraso.

También quedó verificado el curso alternativo 7a: `FiltrarCalendario` (`:6917`) filtra
por tipo de procedimiento y por categoría.

### RF4.9 Corrección y eliminación de eventos sanitarios — **cumple entero**

| Entidad | Modificar | Eliminar | Devolución al stock |
|---|---|---|---|
| Diagnóstico | `:5125` | `:5172` | **no corresponde**: no descuenta insumo |
| Tratamiento | `:5364` | `:5438` | ✔ contra-movimiento por la cantidad aplicada |
| Vacunación | `:5678` | `:5731` | ✔ contra-movimiento por `UNIDADES_POR_VACUNACION` |
| Descorne | `:5873` | `:5915` | **no corresponde**: `Descorne` no tiene insumo ni cantidad |

Los cuatro tienen su `Validar…` separado del método que ejecuta, y la eliminación se
bloquea con explicación cuando hay algo colgando —el diagnóstico con tratamientos
aplicados manda a borrar primero los tratamientos, y nombra cuántos son—.

**La modificación también mueve el stock**, no sólo la eliminación:
`MovimientosPorCambioDeInsumo` arma el par de movimientos cuando la corrección cambia
el insumo aplicado, sin borrar el egreso original. Es la misma decisión de RF5.10
—contra-movimiento de ajuste, no borrado— aplicada acá.

> **Precisión menor, sin propuesta formal.** El RF dice «devolviendo al stock los
> insumos que se habían descontado», que es exacto para las dos entidades que
> descuentan y vacío para las otras dos. Se puede dejar como está: la frase no afirma
> que las cuatro descuenten.

### RF5.8 Alertas de vencimiento — **cumple entero, y la parte difícil es la segunda mitad**

| Lo que el RF promete | Dónde está |
|---|---|
| Notificar los próximos vencimientos | `ListarAlertasVencimiento` (`:4881`), en pantalla, en el tablero y en el resumen diario |
| Con la anticipación **configurada** | `Parametros().DiasAnticipacionVencimiento`, 30 días por defecto, editable — es uno de los once de RF0.3 |
| Imputando el consumo a la partida que vence primero | `ListarPartidas` (`:4834`) |

La imputación es lo que había que mirar de cerca, porque **el modelo no vincula el
egreso con la partida de la que salió**: los movimientos de ingreso son las partidas y
los egresos son un total. `ListarPartidas` resuelve eso ordenando las partidas por
fecha de vencimiento e imputándoles el consumo acumulado en ese orden, hasta agotarlo.
Es FEFO, y es el orden en que se usan los productos en el tambo.

**El caso de borde está resuelto, y es el que se suele errar:** la partida **sin** fecha
de vencimiento no queda primera por tener la fecha vacía. `ClaveVencimiento` (`:4954`)
convierte el `MinValue` en `MaxValue`, así que va **al final** y consume última. Si
fuera al revés, una partida sin vencimiento se comería el consumo del rodeo entero y
todas las partidas con fecha quedarían enteras, alertando por unidades que ya no
existen.

Tampoco alertan las partidas agotadas ni las que no declaran vencimiento (`:4890`), que
es lo correcto: no se puede tirar lo que no queda.

---

## 6. Lo aprobado, aplicado — el Anteproyecto v8

Al ir a aplicar las resoluciones apareció un hallazgo del circuito y no del documento:
`editar_anteproyecto.py` tenía bloque `v6` y bloque `v7` y **ninguno para la auditoría**,
así que las resoluciones estaban escritas acá, aprobadas, y sin quién las aplicara.

Y una sorpresa a favor: **el script estaba más adelantado que su salida.** Desde el
02/09 ya traía RF7.6 con los ocho avisos y RF7.7 con la hora configurable —el Módulo 7
los corrigió al escribirse—, pero el `.docx` no se había regenerado. H9, buena parte de
H10 y la mitad de H11 se arreglaron **regenerando**. Es el modo de falla propio de un
documento que se genera: el script y su salida discrepan en silencio, y lo que el
tribunal lee es la salida.

**El 08/09/2026 se agregó el bloque `v8` y se produjo `Anteproyecto_v8.docx`.** Siguen
siendo **74 requerimientos, en el mismo orden** —verificado sobre el documento generado—.

| Hallazgo | Dónde impactó | Estado |
|---|---|---|
| H1 | RF3.10, con los umbrales de 9 y 13 meses | **aplicado** |
| H2 | RF2.2, con la validación cruzada lote / control individual | **aplicado** |
| H3 | `Privacy.cshtml` | **ya estaba hecho**: la pantalla se borró |
| H4 | RF0.3 «de la hembra» y RF1.8 con los seis cortes | **aplicado** |
| H5 | «vaquillona» en RF1.8 | **aplicado**, y el código y la base desde el 08/09 |
| H6 | credenciales versionadas | **abierto**, se resuelve al armar el repositorio de entrega |
| H7 | RF1.7 «entre los progenitores y los abuelos», y una limitación nueva | **aplicado** |
| H8 | Control de Versionado: `master`, sin las cuatro ramas inventadas, versiones por entrega documental | **aplicado** |
| H9, H10, H11 | RF7.6 y RF7.7 | **aplicado** (parte, regenerando) |
| H12, H13 | RF1.14 | **aplicado** |

**El Proyecto se corrigió en la misma tanda y pasa a ser `Proyecto_v7.docx`:** CU2, CU4,
CU9, CU20, CU21 y el análisis de 2.1 repetían con sus palabras lo que los requerimientos
ahora dicen distinto. CU48 y CU49 no hubo que tocarlos —se habían escrito leyendo el
código del Módulo 7—. Regenerarlo destapó, además, tres cosas escritas y nunca
publicadas: el registro rápido en CU40, el actor de CU49 y un método de más en el
diccionario.

**De los trece hallazgos quedan doce cerrados.** El único abierto es H6, que no es del
documento sino del despliegue, y se resuelve al armar el repositorio de entrega.

El detalle de cada cambio, con su porqué, está en `docs/cambios-anteproyecto-v8.md`.

> **Una decisión que se tomó al aplicar H8 y conviene tener a mano**, porque se aparta de
> lo que esta auditoría había propuesto. La propuesta era reemplazar las cuatro ramas
> `feature/…` inventadas por los nombres reales del repositorio; se optó por **quitarlas
> sin reemplazo**. El repositorio de entrega se arma de cero (`docs/entrega-repositorio.md`),
> así que los nombres reales tampoco van a existir en él: **un ejemplo que se puede
> desmentir es peor que ninguno**. La convención —nombre descriptivo por funcionalidad,
> integración por Pull Requests— es verdadera en los dos repositorios.
