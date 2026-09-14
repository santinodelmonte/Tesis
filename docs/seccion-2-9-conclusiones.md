# 2.9 Conclusiones

Fuente de la sección: los diez subtítulos del ejemplo de la cátedra, los catorce riesgos
del anteproyecto, `docs/auditoria-tres-vias.md`, los tres `cambios-anteproyecto-v*.md` y
la historia del repositorio.

> **Lo que está marcado `[COMPLETAR: …]` no se puede escribir desde el repositorio.** Son
> los hechos que sólo conocen los autores: cómo trabajaron entre ellos, qué dijo el tutor
> y qué dijo la clienta, cuántas horas le dedicaron, y qué dieron las pruebas al
> ejecutarse. No están inventados a propósito.

---

## Dinámica del equipo de trabajo

[COMPLETAR: si habían trabajado juntos antes, cómo se repartieron las tareas, si el
reparto funcionó, y cómo resolvieron los desacuerdos cuando aparecieron.]

Lo que sí quedó asentado es la forma de trabajo: el desarrollo se dividió en **seis
incrementos**, cada uno con su conjunto de funcionalidades y sus pruebas, y el trabajo se
hizo sobre los mismos módulos con revisión cruzada, tal como se planificó en el
anteproyecto. Los módulos no se repartieron por integrante, y esa decisión tuvo una
consecuencia visible: **las reglas de negocio que cruzan módulos** —el tratamiento que
saca a la vaca del tanque de leche, el parto que abre una lactancia y da de alta la
cría— quedaron coherentes, porque nadie tuvo que adivinar qué hacía el módulo del otro.

## Relación con el cliente

[COMPLETAR: cómo fue el contacto con la encargada a lo largo de las iteraciones, con qué
frecuencia, por qué medios, y cómo se resolvieron las diferencias de criterio. El relato
extendido va en la sección 2.8.]

Hay un hecho concreto que conviene dejar escrito acá porque cambió el sistema: **el
relevamiento sobre lo que se carga todos los días** mostró que los cuatro eventos
reproductivos —celo, servicio, tacto y parto— se anotan a diario, uno por animal, y que
llegar a cada uno costaba cuatro clics de menú. De ahí salió el **registro rápido del
tablero**, que resuelve dos de esos eventos en la propia pantalla de inicio y abre el
formulario completo de los otros dos con la caravana ya cargada. No es una funcionalidad
que estuviera en el anteproyecto: salió de mirar cómo se trabaja.

## Relación con el tutor

[COMPLETAR: cómo fue el acompañamiento a lo largo del proyecto.]

Queda registrada una intervención que cambió el sistema: el **20/08/2026 el tutor pidió
rehacer la pantalla de inicio**, y el rediseño quedó hecho el 22/08. El tablero pasó de
ser un resumen a ser el lugar donde se carga lo del día. Y conservó la decisión que lo
hace confiable: **el tablero no calcula nada propio**, junta las listas de trabajo y las
alertas que ya viven en la capa de dominio. Si calculara por su cuenta, discreparía con
los módulos —y, desde que existe el resumen diario, discreparía además con el mensaje que
la encargada lee a la mañana en el teléfono.

## Riesgos

En la primera etapa del proyecto se analizaron catorce situaciones adversas que podían
darse durante el desarrollo y se planificó qué medidas tomar ante cada una. A
continuación se evalúa una por una: si se dio, y en caso afirmativo cómo se resolvió.

**R1 — Subestimación del tamaño del proyecto. Se dio.** Es el riesgo que más se notó. El
catálogo de casos de uso pasó de **treinta y cinco a cuarenta y nueve** al detallar los
módulos: quince casos de uso nuevos que describían cosas que el usuario podía hacer y que
ninguno de los anteriores cubría —la corrección y eliminación de registros ya cargados,
el tablero, los indicadores, las candidatas a descarte, la ficha integral—. El control
preventivo previsto, dividir el desarrollo en incrementos controlados, es el que absorbió
el crecimiento: los casos de uso nuevos se concentraron en la cuarta y la quinta
iteración, sin obligar a rehacer lo construido antes.

**R2 — Errores en la especificación o interpretación de requerimientos. Se dio, y fue el
hallazgo más importante del proyecto.** Al terminar el sistema se hizo una **auditoría de
tres vías** —requerimiento contra caso de uso contra código—, que encontró **catorce
divergencias**. No eran errores de programación: eran lugares donde el documento decía
una cosa y el sistema hacía otra, casi siempre porque el sistema había aprendido algo que
el documento no registró. Los casos más claros:

| # | Qué decía el documento | Qué hacía el sistema | Cómo se resolvió |
|---|---|---|---|
| H1 | RF3.10 daba la misma edad mínima al celo y al servicio | Usa nueve meses fijos para el celo y el parámetro configurable para el servicio | Se corrigió el requerimiento: el sistema tenía razón |
| H5 | El documento decía «novilla» | La pantalla decía «vaquillona» | Se renombró en el código y en la base el 08/09, porque «vaquillona» es la palabra del tambo |
| H7 | RF1.7 sugería recorrer toda la genealogía | Llega hasta los abuelos | Se acotó el requerimiento y se agregó la limitación al alcance |
| H12 | RF1.14 hablaba de «la edad mínima al servicio» | Exige esa edad más los nueve meses de gestación | Se corrigió el requerimiento |
| H14 | CU44 a CU47 prometían no generar el reporte de un período vacío | Lo genera igual, con las secciones vacías rotuladas | Se corrigió el caso de uso |

**Trece de los catorce hallazgos quedaron cerrados.** El criterio fue siempre el mismo, y
vale la pena decirlo porque es lo que un tribunal pregunta: **cuando el documento y el
sistema discrepaban, se revisó cuál de los dos tenía razón, y en la mayoría de los casos
la tenía el sistema**, porque describía la operativa real del tambo. La corrección fue del
documento, no del código —salvo en H5, donde se cambiaron los dos lados—. El hallazgo que
sigue abierto es **H6**: [COMPLETAR: si las credenciales se sacaron del repositorio antes
de la entrega, o si se decidió otra cosa].

El mismo trabajo dio lugar a **tres versiones sucesivas del anteproyecto**: la v6, que
incorporó los veinte requerimientos que el sistema había ganado; la v7, que quitó lo que
no se podía verificar; y la v8, que aplicó los hallazgos de la auditoría.

**R3 — Falta de experiencia en desarrollos de gran escala. Se dio, y era esperable.**
[COMPLETAR: qué costó aprender y cómo se resolvió.] Quedó a la vista en el propio proceso:
varias decisiones de arquitectura se tomaron sobre la marcha y se corrigieron después —la
caché de la capa de dominio, que era compartida por todo el proceso y pasó a vivir en cada
instancia, es el ejemplo más claro—.

**R4 — Retrasos en el desarrollo. [COMPLETAR: si se dio, en qué iteración, y cómo se
recuperó.]**

**R5 — Funcionalidades de alta complejidad. Se dio, y se resolvió acotando en vez de
prometer.** Tres reglas del tambo resultaron más caras de lo previsto, y en los tres casos
se optó por una versión acotada, **verificable y declarada como limitación** antes que por
una completa que no se pudiera garantizar: la verificación de consanguinidad llega hasta
los abuelos y no a la genealogía entera; la proyección de producción a 305 días es lineal
y no modela la curva de lactancia; y los umbrales de descarte son criterios fijos y no
parámetros configurables. **Las tres están escritas en las limitaciones del alcance**, que
es lo que las convierte en una decisión y no en una deuda.

**R6 — Dependencia de servicios externos de notificación. No se dio, y está cubierto.** El
bot de Telegram funcionó, y el diseño no depende de él: las alertas que el bot envía son
las mismas que se ven en el tablero y en las pantallas de cada módulo, de donde el bot las
toma. Si el servicio dejara de responder, no se pierde información ni se interrumpe la
operación. El plan de contingencia de 2.7 lo retoma.

**R7 — Resistencia al cambio por parte de la usuaria. [COMPLETAR: si apareció.]** El
control preventivo previsto —mantener una operativa parecida a la actual— se aplicó desde
el diseño: el sistema se organiza en el orden en que ocurre el trabajo del tambo y no en
el orden en que se construyó el software.

**R8 — Errores durante la migración de datos ganaderos. No se dio, porque no hubo
migración automática.** Es una limitación declarada en el alcance desde el anteproyecto: la
carga inicial del rodeo se hace a mano. Se evitó el riesgo, no se lo resolvió, y conviene
decirlo así.

**R9 — Fallas del hosting o pérdida de información. No se dio.** El control preventivo
—los respaldos automáticos del servicio— está descrito en 2.6, junto con la recomendación
de una segunda copia fuera del servidor. [COMPLETAR: si hubo algún episodio durante el
despliegue.]

**R10 — Falta de comunicación entre los integrantes. [COMPLETAR.]**

**R11 — Abandono del proyecto por parte de un integrante. [COMPLETAR: lo esperable es que
no se haya dado.]**

**R12 — Pérdida de interés o disponibilidad del cliente. [COMPLETAR.]**

**R13 — El sistema no cumple las expectativas del cliente. [COMPLETAR: qué dijo la
encargada al ver el sistema terminado; el relato va en 2.8.]** El control preventivo
funcionó al menos una vez de forma comprobable: el rediseño del tablero salió de un
relevamiento sobre lo que se carga todos los días, no de una suposición del equipo.

**R14 — Interfaz de usuario poco intuitiva. Se dio, y se corrigió.** La primera pantalla
de inicio era un resumen que no servía para trabajar, y hubo que rehacerla. Además se
tomaron medidas que el anteproyecto no preveía y que se convirtieron en criterios de
verificación de los requerimientos no funcionales: el cumplimiento de las pautas **WCAG
2.1 en nivel AA**, verificando contrastes y evitando que el color sea el único medio para
transmitir información, y el **uso desde el celular verificado a partir de los 375 píxeles
de ancho**, que es la condición real de uso: los eventos se registran en el tambo.

## Metodología utilizada

El desarrollo se organizó en seis incrementos funcionales, cada uno con sus pruebas, con
validaciones con la encargada entre uno y otro. Fue la decisión correcta, y por una razón
concreta que se puede señalar: **los quince casos de uso nuevos aparecieron a mitad de
camino**, y un modelo que no admitiera revisar el alcance entre iteraciones habría
obligado a elegir entre entregar lo planificado —sabiendo que le faltaba— o rehacer el
diseño entero.

El orden de los incrementos también resultó acertado y no fue casual: producción y
reproducción se abordaron **juntas**, porque el estado productivo de una hembra depende de
sus partos y de sus secados, y separarlas obligaba a construir dos veces la misma lógica
de estados. Lo mismo con sanidad e insumos, que dependen entre sí porque todo tratamiento
consume stock.

Lo que el modelo no resolvió solo es lo que corrigió la auditoría: **iterar sobre el
sistema no mantiene el documento al día**. El anteproyecto quedó describiendo un sistema
anterior al que se estaba construyendo, y hubo que hacer una pasada completa al final para
alinear los dos. Si hubiera que hacerlo de nuevo, la revisión del documento sería parte de
cada iteración y no una etapa final.

## Herramientas utilizadas

**ASP.NET Core con Razor Pages** cumplió lo que se esperaba de él al elegirlo: el modelo de
una página por pantalla se corresponde uno a uno con los casos de uso, lo que hizo que
encontrar el código de una funcionalidad fuera inmediato. La separación en tres capas
—presentación, dominio y persistencia— se sostuvo durante todo el proyecto.

**MySQL** se comportó como se esperaba. La decisión de resolver dentro de una transacción
toda operación que escribe en más de una tabla resultó imprescindible, y no por prolijidad:
un parto escribe el parto, abre la lactancia, da de alta la cría y actualiza el estado de
la madre. Sin transacción, un fallo a mitad de camino deja un rodeo con datos imposibles.

**Consultas parametrizadas en un único punto.** Las setenta y ocho variables que la
aplicación envía a la base viajan como parámetros con nombre, y hay un solo lugar en todo
el sistema donde un valor se carga en un comando SQL. Costó disciplina al principio y
eliminó por construcción una clase entera de problemas.

**El bot de Telegram** resultó la mejor decisión de la última iteración, y no por lo
técnico: es lo que hace que el sistema alcance a la usuaria sin que ella lo abra. El
resumen diario llega al teléfono a la hora configurada con las tareas del día.

**QuestPDF y ClosedXML** resolvieron los reportes en PDF y planilla sin fricción.

**Git y GitHub** [COMPLETAR: si el equipo tenía experiencia previa; en qué ayudó
concretamente].

## Trabajo colaborativo

[COMPLETAR: cómo se coordinaron en el día a día, qué medios usaron, y qué aprendieron
sobre trabajar de a dos sobre el mismo código.]

Del repositorio se desprende una práctica que conviene contar: **el trabajo se hizo en
ramas por funcionalidad que se integraban a `master` mediante solicitudes de
incorporación**, lo que dejó cada incremento asociado a un conjunto de cambios
identificable. Es también lo que permitió hacer la auditoría final: se pudo reconstruir
cuándo y por qué cada regla del sistema quedó como quedó.

## Producto final

El sistema quedó terminado: **los siete módulos, los cuarenta y nueve casos de uso, y la
compilación sin errores**. Cubre lo que el anteproyecto se propuso —centralizar en un
único lugar la información del rodeo que hoy se lleva en cuadernos y pizarrones— y lo hace
con una propiedad que no estaba pedida y que resultó ser lo más valioso: **cada dato se
carga una sola vez y el sistema propaga sus consecuencias**. Un tratamiento saca a la vaca
del tanque de leche; un parto abre una lactancia, da de alta la cría y actualiza la
categoría de la madre; una inseminación descuenta una pajuela del stock. Eso es lo que
distingue al sistema de una planilla mejor organizada.

Sobre el cumplimiento de los objetivos específicos: [COMPLETAR: cuáles se pueden dar por
cumplidos según las pruebas ejecutadas en 2.3 y la sesión de trabajo con la encargada.
Los objetivos están redactados para que cada uno se pueda verificar contra una pantalla o
un comportamiento concreto, de modo que esta evaluación es una por una y no una impresión
general.]

## Puntos a mejorar

**Las pruebas son funcionales y manuales, y no hay pruebas automatizadas.** Fue una
decisión, no un olvido, y el plan de testing del anteproyecto promete exactamente eso
—pruebas de caja negra sobre cada funcionalidad, documentadas con sus datos, su resultado
esperado y su resultado obtenido—; de hecho, la mención a pruebas de caja blanca se quitó
del plan justamente para no comprometer algo que el proyecto no iba a hacer. Pero el costo
está y hay que nombrarlo: **cada cambio obliga a repetir el recorrido a mano**, y las
reglas que cruzan módulos son las que más se benefician de una prueba que se corra sola.
Es lo primero que agregaría el equipo si el sistema siguiera creciendo.

**El documento se atrasó respecto del sistema.** Ya está dicho en Metodología, y es el
punto a mejorar más honesto del proyecto: hubo que hacer una auditoría de tres vías al
final para alinear requerimientos, casos de uso y código, y encontró catorce divergencias.
Ninguna era grave por separado; juntas describían un sistema distinto del que había.

**El paginado de los listados se resuelve en el navegador.** Alcanza para el volumen de un
establecimiento de esta escala, y está declarado como limitación, pero no escala: con años
de registros acumulados habría que paginar contra la base.

**La proyección de producción a 305 días es lineal.** Es la limitación que más se nota en
el uso real, porque la curva de lactancia no es una recta y la estimación se aleja hacia el
final del período.

**No hay registro de quién hizo cada cosa.** El sistema tiene un único par de credenciales
—es una decisión de alcance, porque hay una sola persona a cargo de los registros— pero la
consecuencia es que no queda traza de las modificaciones. Si el tambo incorpora personal
que también cargue datos, el esquema de acceso es lo primero que habría que revisar.

## Conclusión final

[COMPLETAR: el cierre es de los autores. Lo que sigue es el material con el que se puede
escribir.]

El proyecto entregó un sistema completo y en funcionamiento para un establecimiento que
hasta hoy trabaja con cuadernos y pizarrones, y lo entregó con la documentación que permite
mantenerlo: los requerimientos, los casos de uso, el modelo de datos, los diagramas y el
manual.

Lo que el equipo se lleva no es el sistema. Es haber recorrido el ciclo completo —relevar,
especificar, diseñar, construir, probar y documentar— y haber descubierto en el camino que
**la parte difícil no es escribir el código**. La parte difícil es que lo que el documento
dice y lo que el sistema hace sigan siendo lo mismo al final de seis iteraciones; y que las
reglas del negocio que uno cree entender después de una entrevista casi nunca son las
reglas que el trabajo real tiene.
