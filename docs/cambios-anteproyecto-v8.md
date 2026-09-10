# Cambios del anteproyecto — v7 a v8

Tercera tanda de correcciones sobre `Anteproyecto_v5.docx`. La v6 hizo que los
requerimientos dijeran lo que el sistema hace; la v7 corrigió **cómo estaban escritos**;
ésta corrige **lo que dicen** donde el documento y el código no coincidían, contra los
trece hallazgos de `docs/auditoria-tres-vias.md`.

Se aplican con el bloque `v8` de `docs/editar_anteproyecto.py`, que sigue partiendo del
v5. La salida pasa a ser `Anteproyecto_v8.docx`.

**Ningún requerimiento se agregó, se quitó ni se renumeró: siguen siendo 74, en el mismo
orden.** Verificado sobre el documento generado.

---

## 0. Antes de escribir una línea: la mitad ya estaba hecha y no se había generado

El primer hallazgo de esta tanda fue del circuito y no del documento. `editar_anteproyecto.py`
ya traía, desde el 02/09, RF7.6 con los ocho avisos y RF7.7 con la hora configurable —el
Módulo 7 los había corregido al escribirse—, pero **el `.docx` no se había vuelto a
generar**: el commit de aquel día regeneró el v6 y dejó el v7 viejo.

O sea que H9, buena parte de H10 y la mitad de H11 se arreglaban **regenerando**, sin
tocar el script. Vale anotarlo porque es el modo de falla propio de un documento que se
genera: el script y su salida pueden discrepar en silencio, y lo que el tribunal lee es
la salida.

Lo mismo pasó con el estado de las iteraciones, que ahora dice que los seis incrementos
están completos y que la sexta iteración **se desarrolló** —en el v7 seguía en futuro—.

---

## 1. Cuatro requerimientos que decían menos de lo que el sistema controla

| RF | Lo que decía | Lo que dice ahora | Hallazgo |
|---|---|---|---|
| **RF3.10** | «un celo **o** un servicio en un animal que no alcanzó la edad mínima al servicio», como si compartieran umbral | Los dos umbrales con su número: **9 meses** para la detección de celo y la **edad mínima al servicio configurada, 13 por defecto**, para el servicio | H1 |
| **RF1.14** | «una edad que no admita la edad mínima al servicio» | Esa edad **más los nueve meses de gestación**, y son dos: **22 meses la madre** (con la edad configurable) y **24 el padre** | H12 |
| **RF1.7** | «un ancestro común dentro de **la ascendencia registrada** de ambos», que se lee como toda la genealogía | «un ancestro común **entre los progenitores y los abuelos** de ambos», que es lo que `ListarAscendencia` recorre | H7 |
| **RF1.8** | «las hembras según su edad y su número de partos, y los machos según su edad y su destino reproductivo» — sin un solo corte | Los seis cortes con sus números, leídos de `CalcularCategoria` | H4 |

**Por qué los números importan acá y no son un capricho.** Un requerimiento sin número no
se puede declarar cumplido ni incumplido: se puede tomar un animal, mirar su edad y decir
si el sistema cumple RF3.10 sólo si el RF dice *nueve* y *trece*. Es el mismo criterio
con el que la v7 sacó los porcentajes de los objetivos.

**Y en los tres primeros el código tenía razón.** El celo se detecta antes de que el
servicio sea conveniente; para ser progenitor no alcanza con haber tenido edad de
servicio alguna vez, sino nueve meses antes del nacimiento; y la consanguinidad hasta los
abuelos es una decisión, no un olvido. Lo que estaba mal redactado era el documento.

## 2. RF1.14 también prometía una advertencia distinta de la que el sistema da

Decía que advierte «cuando el progenitor elegido **se encuentre** dado de baja», en
presente. El sistema advierte cuando **ya lo estaba en la fecha en que debió engendrar a
la cría**: la del parto para la madre, la de la **concepción** para el padre.

La distinción del padre no es un detalle: el semen congelado sigue sirviendo años después
de que el toro murió, y por eso esa advertencia termina diciendo «Es correcto si la cría
vino de una pajuela suya».

**Advertir en presente sería peor**, y ésta es la razón de fondo: la mitad del rodeo
histórico está dado de baja, así que un aviso en cada carga es un aviso que nadie lee.

De paso se corrigió **«un progenitor que figure en su propia descendencia»**, que se lee
sobre el progenitor y describe algo imposible. Lo que el sistema impide es que el
progenitor **descienda del animal que se está cargando**.

## 3. Dos requerimientos que no describían algo que el sistema hace

| RF | Qué se agregó | Hallazgo |
|---|---|---|
| **RF2.2** | Que **la suma de los controles individuales de un turno no supere el total registrado por lote** para ese turno, ni un control nuevo haga que lo supere. Y el máximo por control, con su valor por defecto de 100 litros | H2 |
| **RF7.6** | Que las alertas salen **dentro del resumen diario** —no hay un aviso por evento en el momento— y que cada tipo se puede **activar y desactivar** | H10 |
| **RF7.7** | La **hora configurada** y la consulta **a demanda** por el mismo canal | H11 |

**RF7.6 y RF7.7 se leían como dos canales y hay uno solo.** Y está bien que haya uno
solo: un pendiente no ocurre en un instante —la vaca entra en la lista de tactos porque
pasaron los días, no porque alguien apretó algo—, así que no hay un momento en el que
disparar el aviso. El día es la unidad natural.

**La hora del resumen era el doceavo parámetro configurable** y RF0.3 promete once. Se
resolvió nombrándola en RF7.7 y no en RF0.3: se configura una vez, junto con el
destinatario, y no es un parámetro de manejo del rodeo.

## 4. «Vaquillona», que ya se había aplicado al sistema

RF1.8 nombra ahora las seis categorías, y la hembra de más de doce meses sin partos es
**vaquillona**. Es el hallazgo H5, resuelto el 26/08 al revés de lo propuesto —se usa la
palabra del tambo— y aplicado **al código y a la base el 08/09**, porque la categoría sale
en pantalla y el documento no podía decir una palabra distinta de la captura.

El glosario del documento final declara *novilla* como sinónimo que no se usa.

## 5. El control de versionado, que un tribunal verifica en treinta segundos

Es la parte del documento más fácil de desmentir: se abre el repositorio y se mira.

| Decía | Dice ahora |
|---|---|
| «La rama principal (**main**)» | «La rama principal (**master**)» |
| Cuatro ramas de ejemplo escritas una por una —`feature/gestion-animales`, `feature/control-sanitario`, `feature/reportes`, `feature/reproduccion`— que no existen | **Se quitaron.** Queda la convención —nombre descriptivo según la funcionalidad— y la integración por **Pull Requests** |
| «Las versiones estables serán identificadas mediante **numeración incremental**» | Se identifican por **las entregas documentales** que las acompañan |

> **Los cuatro nombres se quitaron en lugar de reemplazarse por los reales, y es una
> decisión.** La auditoría había propuesto poner los del repositorio como ejemplo, pero el
> repositorio de entrega se arma de cero (ver `docs/entrega-repositorio.md`) y esos nombres
> tampoco van a existir en él. **Un ejemplo que se puede desmentir es peor que ninguno**, y
> la convención sin ejemplos sigue siendo verdadera en cualquiera de los dos repositorios.

Se corrige el documento y no el repositorio. Renombrar la rama principal y crear tags a
esta altura sería maquillar el repositorio para que se parezca al documento, que es al
revés de como se viene trabajando.

---

## 6. El Proyecto, en la misma tanda — `Proyecto_v7.docx`

No es del anteproyecto, pero un requerimiento corregido y un caso de uso que lo cuenta al
revés valen lo mismo que no haber corregido nada. Se pasó por `docs/casos_de_uso_parte1.py`
y por `docs/editar_proyecto.py`, y la salida pasa a ser `Proyecto_v7.docx`.

| Dónde | Qué cambió | Hallazgo |
|---|---|---|
| **CU4** Configurar parámetros | «edad mínima al servicio **de la hembra**» entre los once | H4 |
| **CU2** Alta de animal | La fecha del progenitor debe admitir la edad mínima al servicio **más los nueve meses de gestación**: 22 meses la madre, 24 el padre | H12 |
| **CU9** Verificar consanguinidad | El curso básico y las reglas dicen que recorre **los progenitores y los abuelos**, no «la ascendencia registrada» | H7 |
| **CU20** Registrar celo | El umbral es la **edad mínima de detección de celo, 9 meses**, y las reglas explican por qué no es la del servicio | H1 |
| **CU21** Registrar servicio | «la edad mínima al servicio **configurada, de 13 meses por defecto**» | H1 |
| **2.1 Análisis** | «ternero, **vaquillona**, vaca» y el control de consanguinidad hasta los abuelos | H5, H7 |

**CU48 y CU49 no hubo que tocarlos:** ya describían los ocho avisos, la hora configurable
y el `/resumen` a demanda. Se habían escrito leyendo el código del Módulo 7.

### Y al regenerar aparecieron tres cosas que estaban escritas y no publicadas

El `Proyecto_v6.docx` en disco venía atrasado respecto de sus propios generadores, igual
que el anteproyecto:

- **CU40 (tablero de inicio)** no tenía el **registro rápido de eventos reproductivos**,
  que está en el sistema desde el 22/08: dos pasos del curso básico, tres cursos
  alternativos y de excepción, las reglas y las validaciones.
- **CU49** seguía teniendo al **Sistema como actor principal**, corregido en la tanda v7 y
  nunca generado.
- El **diccionario de clases** decía «Tactos (10 métodos)» y son **once** desde que
  `ValidarTacto` se separó del alta.


### Los diagramas, que tampoco estaban al día (10/09)

Con el mismo criterio se regeneraron todos los diagramas y los artefactos del paso 1
contra el código de hoy, y se compararon con los versionados. **La mayoría estaba bien:**
el MER con sus 24 tablas, los diagramas de dominio y persistencia con las clases del
Módulo 7, los 49 de secuencia y el de casos de uso del módulo 7 con CU44 a CU49. Ninguno
decía «Novilla». Lo que no:

| Diagrama | Qué tenía | Qué tiene |
|---|---|---|
| Secuencia **CU40** | Sólo la consulta del tablero | El **registro rápido**: `ValidarCelo`, `AltaCelo`, `ServicioVigente`, `ValidarTacto`, `AltaTacto` |
| Secuencia **CU22** | La validación del tacto adentro del alta | `ValidarTacto()`, que se separó para que el tablero la use |
| Casos de uso **módulo 7** | Un segundo actor, **«Sistema»**, conectado a CU49 | **Un solo actor**, como los otros siete módulos |

Los dos de secuencia son consecuencia del registro rápido del 22/08: los diagramas se
regeneraron al día siguiente, pero sin ese cambio adentro.

**El actor «Sistema» merece su párrafo, porque no era un olvido sino una decisión vieja.**
El generador lo dibujaba a propósito —«el resumen diario lo dispara un proceso
programado»—, pero la ficha de CU49 tiene a la encargada como único actor desde la
corrección aprobada el 26/08: el actor es quien persigue la meta, y quien quiere
enterarse de sus tareas es ella. Que el tiempo dispara el caso de uso ya está dicho donde
corresponde, en el desencadenante. Dibujarlo además como actor dejaba a la ficha y a su
diagrama diciendo cosas distintas, y contradecía al anteproyecto, que presenta a Sofía
como **único actor del sistema**. Se aplicó lo ya decidido, no se decidió nada nuevo.

En el diagrama de **secuencia** de CU49 el proceso programado **se queda** como primera
línea de vida: ahí no se afirma quién es el actor sino quién llama a la Controladora, y
ése es el proceso. Se corrigieron también los dos comentarios que lo llamaban actor, en
`ServicioNotificaciones.cs` y en `generar_secuencia.py`, para que el código diga lo mismo.

**Las copias de revisión en markdown** también estaban viejas: `diccionario-clases-v6.md`
decía «Tactos (10 métodos)» y llamaba `RegistrarAlertas` a lo que hoy es
`RegistrarEnvioResumen`; `modelo-datos-v6.md` tenía `tipo_servicio` en `VARCHAR(20)`, que
pasó a 30 el 02/09 porque no entraba la inseminación. Esas copias no llegan al documento
—`editar_proyecto.py` importa el modelo de datos y el diccionario en vivo—; los `.png` sí,
tal cual están en disco, y por eso `Proyecto_v7.docx` se regeneró con los tres diagramas
corregidos.

## 7. Las precondiciones, y lo que destaparon (10/09)

Era el punto 7 de `revision-tutor.md`: de 49 casos de uso, **21 decían sólo «el usuario
debe estar logueado»** —la revisión contó 20; CU23 lo decía sin «en el sistema» y se
escapaba de la cuenta—. Que el usuario esté logueado vale para todos, así que no distingue
nada.

Se siguió el criterio que la revisión propuso y que respeta al modelo de la cátedra: el
«logueado» **se queda** —el ejemplo del tutor lo usa— y **cada precondición dice además su
condición propia**, verificada contra la pantalla que resuelve el caso. Se aplicó también
a los cuatro que tenían la condición propia pero no el «logueado» (CU9, CU21, CU30 y CU31),
para que haya un solo criterio. Quedan sin él sólo **CU1**, que es el propio inicio de
sesión, y **CU49**, que lo dispara el reloj sin sesión de por medio.

Las precondiciones salieron de dos tipos:

| Tipo | Casos | Ejemplo |
|---|---|---|
| **Exige algo antes** | CU2, CU4, CU8, CU9, CU12, CU21, CU29, CU30, CU31, CU35 | CU12: tiene que haber hembras en lactancia, porque el lote se arma con ellas y la pantalla no deja guardarlo vacío |
| **Funciona sobre una base vacía** | CU3, CU10, CU14, CU15, CU17, CU23, CU25, CU37 a CU40, CU43 a CU47 | CU40: sin animales cargados, el tablero indica por dónde empezar |

**Decir que un caso funciona sin datos también es una precondición**, y no una manera de
esquivarla: dice qué no hace falta que sea verdad, y en casi todos remite al curso que
resuelve el vacío. Así la precondición y los cursos se leen juntos.

Algunas dejaron escrita una regla que no estaba en ningún lado. La raza es obligatoria en
el alta y **no tiene pantalla propia**: se carga con la base de datos. Una pajuela exige que
el toro que la aporta esté registrado como animal —el de catálogo, con «En pie»
desmarcado—. Y en CU29 se separó lo que es precondición de lo que es excepción: que haya
una vacuna dada de alta es precondición; que tenga stock se verifica al guardar.

### Y el ejercicio encontró una regla escrita al revés

La revisión lo había anticipado: pensar qué tiene que ser verdad antes es donde «suelen
aparecer reglas de negocio que no estaban escritas». Apareció una, pero en sentido
contrario: **una regla escrita que el sistema no cumple** (hallazgo H14).

CU44 a CU47 decían que, si el período no tiene registros, el sistema «informa la situación
y **no genera el archivo**». El sistema **lo genera igual**: el único motivo por el que no
arma un reporte es un rango de fechas inválido, y tanto el PDF como la planilla escriben
«Sin registros en el período» en cada sección vacía.

**El código tiene razón.** Un reporte sanitario de un mes sin diagnósticos es un documento
válido —dice que no hubo—, y negarse a emitirlo le quitaría a la encargada la constancia de
un período sin novedades.

| | Antes | Ahora |
|---|---|---|
| Período sin registros | Excepción 4a: no genera el archivo | **Curso alternativo** 4a: lo genera, con cada sección vacía indicada |
| Rango de fechas inválido | No figuraba | **Excepción** 3a de CU44 a CU46: informa y no genera |
| CU47, reporte genético | Excepción 4a: sin genealogía no genera; y la precondición exigía genealogía registrada | **Sin excepciones** —no usa período, así que siempre genera— y la precondición ya no la exige |

Nadie más repetía la promesa: ni el manual, ni el protocolo de pruebas, ni los flujos.

> **Un detalle del sistema, anotado y sin tocar.** El reporte genético no tiene período,
> pero sus secciones vacías dicen igual «Sin registros en el período», porque la frase es
> fija en los dos generadores. Es texto de pantalla y no un error de cálculo; se anota para
> que no sorprenda en la captura.


## 8. El manual de usuario, puesto al día (10/09)

El manual —sección 2.4— se escribió el 26/08 leyendo las pantallas, pero en una rama que
todavía no tenía ni el registro rápido del tablero ni las notificaciones por Telegram. Se
lo comparó contra su propio commit (`ba6959b`): desde entonces cambiaron siete archivos de
pantallas, y el manual no reflejaba cuatro de esos cambios. Revisándolo aparecieron,
además, errores que estaban desde el principio.

| Dónde | Qué decía | Qué dice ahora |
|---|---|---|
| §2.2 Moverse por el sistema | Un menú por módulo en la barra superior, seis módulos | El menú es una **columna lateral** con ocho secciones —*Pendientes y alertas* primero, *Reportes y notificaciones* al final— y Configuración al pie |
| §3 Configuración | «Edad mínima al servicio»: por debajo no deja registrar un servicio | Que alcanza sólo a las hembras, que es también el piso de la genealogía, y que el macho pasa a toro a los 15 meses, fijo (H4) |
| §4.3 Alta de animal | Que una madre sin edad para parir **avisa y deja guardar** | Que **se rechaza sin opción** —22 meses la madre, 24 el padre— y que lo que sólo avisa es el progenitor dado de baja o los padres emparentados (H12, H13) |
| §9 Indicadores | «No se carga nada acá» | El **registro rápido** del tablero: el celo y el tacto se guardan ahí; el servicio y el parto abren su formulario con la caravana cargada |
| §10 Reportes y notificaciones | Que el Módulo 7 estaba «construido a medias» | **Reescrita**: los tres botones de los reportes, qué trae cada uno y que un período vacío se genera igual; la vinculación con `/start` y el mensaje de prueba, la hora, los ocho avisos por módulo, cómo llega el resumen y `/resumen` |
| §11 Desde el celular | «Los seis módulos» | Las ocho secciones |
| Doce caminos de menú | Los del menú anterior: *Lista de Animales*, *Registrar Celo*, *Sanidad → Calendario Sanitario* | Los de hoy: *Rodeo*, *Celos → Registrar celo*, *Pendientes y alertas → Calendario sanitario* |

**El error de §4.3 no era sólo del manual.** El guion de capturas (E1) y el protocolo de
pruebas decían lo mismo, y `flujos-de-prueba.md` era el único que lo tenía bien. La prueba,
corrida como estaba, habría marcado **falla con el sistema funcionando correctamente**. Se
separó en dos: la madre sin edad, que se rechaza, y los padres emparentados, que avisan con
*Guardar de todos modos*. La captura de la advertencia usa ahora ese segundo caso, el mismo
que describe `flujos-de-prueba.md`.

En la misma pasada: E6 del guion decía que el umbral del celo es «configurado» —son 9
meses fijos, H1—, y la prueba del tacto sin resultado citaba un mensaje que cambió de texto
cuando la validación pasó a `ValidarTacto`.

Se sumó una captura, `m6-cu40-registro`, para el registro rápido. Las secciones piden
**107** capturas y el guion define **107**.

---

## Lo que queda pendiente
- **H3 y H6 no son del anteproyecto.** H3 —la pantalla `Privacy.cshtml` de la plantilla—
  ya se borró; H6, las credenciales versionadas, se resuelve al preparar el repositorio de
  entrega, que es lo último de la lista.
- Sigue en pie lo que ya arrastraba la v7: **fusionar RF3.4 con RF3.5 y quitar RF5.2**
  cambia la cantidad de requerimientos y obliga a renumerar, así que quedó para discusión
  aparte. Las precondiciones, que también venían de la v7, quedaron resueltas: ver el
  punto 7.
