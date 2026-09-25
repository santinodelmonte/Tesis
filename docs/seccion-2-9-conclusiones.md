# 2.9 Conclusiones

Fuente de la sección: los diez subtítulos del ejemplo de la cátedra, los catorce riesgos
del anteproyecto, `docs/auditoria-tres-vias.md`, los tres `cambios-anteproyecto-v*.md` y
la historia del repositorio.

> **Sin marcas pendientes.** La confirmación de «Producto final» sale de la sesión de
> capacitación del 23/09/2026 que relata la sección 2.8. Todo lo demás está escrito sobre
> hechos del proyecto.

---

## Dinámica del equipo de trabajo

Los dos integrantes ya habían trabajado juntos en varios trabajos académicos previos, y
eso se notó menos en la velocidad que en la falta de fricción: no hubo un período de
acomodamiento, porque cada uno sabía de antemano cómo trabajaba el otro.

**No hubo reparto fijo de tareas.** Ni por módulo ni por capa: el trabajo se asignaba
dentro de cada incremento según la disponibilidad de cada uno, y los dos terminaron
escribiendo en las tres capas del sistema. Lo que sí hubo fue una regla constante: **lo
que escribía uno lo revisaba el otro antes de integrarlo**. Cuando la revisión encontraba
algo, se corregía y se seguía; los desacuerdos se resolvieron discutiendo el caso concreto
y ninguno escaló más allá de eso.

Ese esquema tiene un costo y una ventaja, y conviene nombrar los dos. El costo es que
nadie es el experto de ningún módulo, y por lo tanto cada tarea empieza con un rato de
releer lo que hay. La ventaja quedó a la vista en el resultado y se explica en el párrafo
siguiente.

La forma de trabajo quedó asentada así: el desarrollo se dividió en **seis
incrementos**, cada uno con su conjunto de funcionalidades y sus pruebas, y el trabajo se
hizo sobre los mismos módulos con revisión cruzada, tal como se planificó en el
anteproyecto. Los módulos no se repartieron por integrante, y esa decisión tuvo una
consecuencia visible: **las reglas de negocio que cruzan módulos** —el tratamiento que
saca a la vaca del tanque de leche, el parto que abre una lactancia y da de alta la
cría— quedaron coherentes, porque nadie tuvo que adivinar qué hacía el módulo del otro.

## Relación con el cliente

El contacto con la encargada fue **telefónico y quincenal**, y se acortó cuando hizo falta:
al cerrar un incremento, o cuando una regla del tambo admitía más de una lectura y no se
podía seguir sin preguntar. El relato extendido va en la sección 2.8; acá interesa un
hecho que define la relación y que conviene no maquillar: **las propuestas de cambio no
vinieron de la clienta, vinieron del equipo**, casi siempre a partir de una observación
del tutor, y se decidían con ella. Las instancias de validación fueron, por eso,
instancias sin sorpresas.

Hay un cambio concreto que conviene dejar escrito acá porque rehízo la pantalla principal.
El disparador fue una observación del tutor sobre el tablero; el contenido salió de
revisar con la encargada **qué se carga efectivamente todos los días**, y ahí apareció lo
que ningún requerimiento había registrado: los cuatro eventos reproductivos —celo,
servicio, tacto y parto— se anotan a diario, uno por animal, y llegar a cada uno costaba
cuatro clics de menú. De ahí salió el **registro rápido del tablero**, que resuelve dos de
esos eventos en la propia pantalla de inicio y abre el formulario completo de los otros dos
con la caravana ya cargada. No es una funcionalidad que estuviera en el anteproyecto.

## Relación con el tutor

El acompañamiento del tutor fue **sostenido y exigente, sobre todo al principio**. Las
primeras reuniones no se ocuparon de corregir lo que había, sino de señalar que el equipo
podía ofrecer bastante más de lo que estaba ofreciendo: el alcance que se proponía era
razonable y también era cómodo. Esa exigencia inicial es la que explica buena parte de la
distancia entre lo que el anteproyecto imaginaba y lo que el sistema terminó siendo, y es
también el origen de casi todas las propuestas de cambio que el equipo después llevó a la
clienta. Visto desde el final, fue la intervención más productiva del proyecto: **subió el
piso antes de que estuviera caro subirlo**.

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

La subestimación también se puede medir en horas, y conviene hacerlo porque es la cuenta
que la sección 1.11 dejó planteada. El plan preveía **dieciséis horas semanales por
integrante a lo largo de veinticinco semanas: 800 horas**, o 1,35 horas por punto de
función. El esfuerzo real fue **de alrededor de mil horas**, un **veinticinco por ciento
por encima** de lo planificado, que llevan la productividad a unas 1,69 horas por punto de
función. La cifra es una reconstrucción y no una medición —el equipo no llevó registro de
horas, y decirlo es parte de la honestidad de la estimación—: sale de las veinticinco
semanas de calendario, de descontar las cinco en las que no hubo avance por obligaciones
de cursada, y de reconocer que en las restantes la dedicación estuvo bastante por encima
de las dieciséis horas pactadas. **Lo que sí es un dato y no una reconstrucción es que el
proyecto se entregó en la fecha prevista**: el desvío no se pagó con calendario, se pagó
con horas.

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
documento, no del código —salvo en H5, donde se cambiaron los dos lados—. **H6, el único
que quedaba abierto, se cerró el 15/09**: las credenciales y la cadena de conexión salieron
del archivo versionado y se cargan desde afuera del repositorio, que es lo que el
requerimiento no funcional de Seguridad pedía desde el principio y lo que la sección 2.6
afirma.

El mismo trabajo dio lugar a **tres versiones sucesivas del anteproyecto**: la v6, que
incorporó los veinte requerimientos que el sistema había ganado; la v7, que quitó lo que
no se podía verificar; y la v8, que aplicó los hallazgos de la auditoría.

**Y el riesgo volvió a aparecer al ejecutar las pruebas**, que es el dato que más vale la
pena contar, porque muestra que la auditoría no había agotado el problema. Correr los
ciento nueve casos de la sección 2.3 sobre el sistema andando encontró **tres divergencias
más del mismo tipo**, y en dos de ellas el equivocado volvió a ser el documento:

| Qué esperaba la prueba | Qué hace el sistema | Cómo se resolvió |
|---|---|---|
| Combinar el filtro de categoría *Vaca* con un estado *En lactancia* | El filtro de estado es activo contra inactivo: no existe un filtro por estado productivo | Se corrigió la prueba y el pie de figura del manual. El sistema cumple RF1.10 |
| Que una hembra sin partos pasara a *Vaquillona* un día después de la edad de cambio | Compara **meses cumplidos**: a los 12 meses y 20 días sigue siendo *Ternera* y recién cambia al cumplir 13 | Se corrigió la prueba. Queda decidir si RF1.8, que dice «vaquillona desde esa edad», se redacta en los mismos términos |
| Que el rechazo por litros fuera de rango repitiera el tope configurado | Rechaza bien, pero el mensaje no trae el tope | Sin corregir: es el texto del mensaje, no el control |

La segunda es la que más enseña. **Nadie había mirado nunca el límite exacto**: el
requerimiento decía «desde esa edad», el caso de uso lo repetía y el código comparaba
meses cumplidos con un «mayor que». Las tres redacciones convivieron seis meses sin que la
diferencia se notara, porque **ninguna prueba había pisado el borde**. Recién al escribir
el caso de prueba —y ejecutarlo a los 12, a los 12 más un día, a los 12 más veinte días, a
los 13 y a los 13 más un día— la diferencia apareció.

La ejecución dejó además un hallazgo de otra clase: **una validación del código que no se
puede alcanzar desde la pantalla**. La verificación de consanguinidad se defiende de que
alguien compare un animal contra sí mismo, pero busca la hembra entre las hembras y el
reproductor entre los machos, que son conjuntos disjuntos: la comparación nunca se cumple.
No es un error —el sistema no hace nada malo— pero es código que nadie va a ejecutar
nunca, y saberlo vale más que suponer que está probado.

**R3 — Falta de experiencia en desarrollos de gran escala. Se dio, y era esperable.**
Lo que más costó aprender no fue lo que más líneas tiene, sino lo que salía del terreno
conocido: **la integración con el bot de Telegram y la generación de reportes en PDF y en
planilla**. Las dos cosas son, cada una a su modo, salir del navegador: una habla con un
servicio externo que responde cuando quiere, y la otra construye un documento sin una
pantalla donde ver el resultado hasta que está hecho. Se resolvieron del mismo modo, que
es el único que había disponible: leyendo la documentación de la biblioteca, probando
contra casos chicos y recién después llevándolo al sistema. Quedó a la vista también en
otra parte del proceso:
varias decisiones de arquitectura se tomaron sobre la marcha y se corrigieron después —la
caché de la capa de dominio, que era compartida por todo el proceso y pasó a vivir en cada
instancia, es el ejemplo más claro—.

**R4 — Retrasos en el desarrollo. Se dio, al principio.** Los atrasos se concentraron en
los primeros incrementos, que es donde era esperable: es el tramo en el que todavía se
estaban tomando las decisiones de arquitectura y en el que cada pantalla se escribía por
primera vez, sin un patrón ya resuelto del cual copiar. El control previsto era reacomodar
el alcance de las iteraciones siguientes; **no hizo falta usarlo**. El tiempo se recuperó
aumentando la dedicación semanal por encima de la planificada, que es de dónde sale el
desvío de horas de R1, y a partir del tercer incremento el ritmo se estabilizó porque ya
había un patrón para cada tipo de pantalla. La fecha de entrega no se movió.

**R5 — Funcionalidades de alta complejidad. Se dio, y se resolvió acotando en vez de
prometer.** Tres reglas del tambo resultaron más caras de lo previsto, y en los tres casos
se optó por una versión acotada, **verificable y declarada como limitación** antes que por
una completa que no se pudiera garantizar: la verificación de consanguinidad llega hasta
los abuelos y no a la genealogía entera; la proyección de producción a 305 días es lineal
y no modela la curva de lactancia; y los umbrales de descarte son criterios fijos y no
parámetros configurables. **Las tres están escritas en las limitaciones del alcance**, que
es lo que las convierte en una decisión y no en una deuda.

La de la consanguinidad dejó además una enseñanza sobre cómo se prueba un límite. Decir
que la verificación llega hasta los abuelos no se demuestra con un caso que dé positivo:
se demuestra con **el que tiene que dar negativo**, dos primos cuyo ancestro común es el
bisabuelo. Para poder correrlo hubo que agregar al rodeo de prueba una línea de cuatro
generaciones, porque con tres no alcanza. El caso da negativo, como corresponde, y recién
ahí la limitación deja de ser una afirmación del documento y pasa a ser un resultado.

**R6 — Dependencia de servicios externos de notificación. No se dio, y está cubierto.** El
bot de Telegram funcionó, y el diseño no depende de él: las alertas que el bot envía son
las mismas que se ven en el tablero y en las pantallas de cada módulo, de donde el bot las
toma. Si el servicio dejara de responder, no se pierde información ni se interrumpe la
operación. El plan de contingencia de 2.7 lo retoma.

**R7 — Resistencia al cambio por parte de la usuaria. No se dio.** En ningún momento
apareció la objeción que el riesgo anticipaba —que en el cuaderno tal cosa se hacía más
rápido—. Hay que decir, para que la afirmación valga lo que vale, que **el riesgo se
evaluó sobre una sola persona**: la encargada es la única que va a operar el sistema, y a
los tamberos la incorporación no les modifica la tarea, así que no hubo con quiénes
medirlo. El
control preventivo previsto —mantener una operativa parecida a la actual— se aplicó desde
el diseño: el sistema se organiza en el orden en que ocurre el trabajo del tambo y no en
el orden en que se construyó el software.

**R8 — Errores durante la migración de datos ganaderos. No se dio, porque no hubo
migración automática.** Es una limitación declarada en el alcance desde el anteproyecto: la
carga inicial del rodeo se hace a mano. Se evitó el riesgo, no se lo resolvió, y conviene
decirlo así.

**R9 — Fallas del hosting o pérdida de información. No se dio.** El control preventivo
—los respaldos automáticos del servicio— está descrito en 2.6, junto con la recomendación
de una segunda copia fuera del servidor. No hubo ningún episodio de caída ni de pérdida de
información durante el despliegue ni en el uso posterior.

**R10 — Falta de comunicación entre los integrantes. No se dio.** El riesgo suponía un
equipo que tiene que construir su forma de trabajar durante el proyecto, y acá esa forma
ya existía por los trabajos anteriores. El mecanismo que lo sostuvo fue el mismo todo el
tiempo: mensajería para el día a día y videollamada cuando el tema pedía más ida y vuelta
que un mensaje. Con la revisión cruzada pasó algo parecido: las correcciones del otro se
aplicaban sin discusión de fondo, porque estaban hechas sobre un código que los dos
conocían.

**R11 — Abandono del proyecto por parte de un integrante. No se dio, y no llegó a
insinuarse.**

**R12 — Pérdida de interés o disponibilidad del cliente. No se dio en la forma prevista.**
La disposición de la encargada se mantuvo durante todo el proyecto. Lo que sí varió fue su
disponibilidad: hubo momentos en que el tiempo libre no daba y alguna instancia de
validación se pospuso. Nunca se cayó, y el desarrollo nunca quedó frenado esperándola,
porque el trabajo de la iteración siguiente no dependía de esa confirmación.

**R13 — El sistema no cumple las expectativas del cliente. No se dio.** Las validaciones
incrementales funcionaron como control preventivo con un efecto bastante literal: **al
final no hubo nada que la encargada viera por primera vez**, porque cada módulo se le
había mostrado a medida que se construía. Eso es exactamente lo que el riesgo buscaba
evitar, y tiene también la contracara que la sección 2.8 desarrolla: un cliente que
valida sin sorpresas es un cliente que tampoco está forzando los límites del sistema. La
confirmación que faltaba no era de expectativas sino de uso, y la dio la capacitación:
la encargada completó las cinco tareas de uso diario sin asistencia.

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

Con las pruebas pasó lo mismo, y es la segunda lección. Cada iteración terminaba con
pruebas funcionales, pero **el protocolo completo se escribió y se ejecutó al final**, y
ahí aparecieron tres divergencias que seis meses de uso no habían mostrado. La razón es
sencilla: probar mientras se construye verifica que lo que se acaba de hacer funciona;
escribir el caso de prueba obliga a decir **qué tendría que pasar**, y es ahí donde se ve
que el documento y el sistema no dicen lo mismo. Las dos cosas hacen falta, y la segunda
llegó tarde.

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

**Git y GitHub** se usaron desde el primer día con una base previa, pero el manejo se
pulió mucho durante el proyecto: pasar de versionar el trabajo propio a integrar el de dos
personas sobre los mismos archivos obliga a entender las ramas de otra manera. **No se
perdió trabajo en ninguna integración.** Lo que la herramienta aportó y no era obvio de
antemano es el registro: cuando la auditoría final preguntó por qué una regla del sistema
era como era, la respuesta estaba en el historial.

## Trabajo colaborativo

La coordinación del día a día fue **por mensajería**, por comodidad y porque permitía
trabajar en horarios distintos sin tener que sincronizarse; cuando un tema pedía más
interacción que un intercambio de mensajes —una decisión de diseño, repasar algo que no
cerraba— se pasaba a **videollamada**. No hubo reuniones fijas de calendario, y con este
equipo no hicieron falta.

Sobre trabajar de a dos en el mismo código, la enseñanza no es sobre la herramienta sino
sobre la decisión de no repartirse los módulos. Cuesta más al empezar cada tarea, porque
hay que volver a leer lo que escribió el otro. Lo devuelve en las reglas que cruzan
módulos, que en este sistema son las que le dan valor: **el tratamiento que saca a la vaca
del tanque, el parto que abre una lactancia y da de alta la cría, la inseminación que
descuenta una pajuela**. Esas reglas viven en el borde entre dos módulos, y son
exactamente las que se rompen cuando cada uno programa contra su idea de lo que hace el
módulo del otro.

Del repositorio se desprende una práctica que conviene contar: **el trabajo se hizo en
ramas por funcionalidad que se integraban a** `master` **mediante solicitudes de
incorporación**, lo que dejó cada incremento asociado a un conjunto de cambios
identificable. Es también lo que permitió hacer la auditoría final: se pudo reconstruir
cuándo y por qué cada regla del sistema quedó como quedó.

## Producto final

El sistema quedó terminado: **los siete módulos, los cuarenta y nueve casos de uso, y la
compilación sin errores**. De los ciento nueve casos de prueba de la sección 2.3, ochenta y
siete se ejecutaron con el resultado esperado y **ninguno dejó al sistema haciendo algo
incorrecto**: las tres diferencias que aparecieron eran del documento en dos casos y del
texto de un mensaje en el tercero. Cubre lo que el anteproyecto se propuso —centralizar en un
único lugar la información del rodeo que hoy se lleva en cuadernos y pizarrones— y lo hace
con una propiedad que no estaba pedida y que resultó ser lo más valioso: **cada dato se
carga una sola vez y el sistema propaga sus consecuencias**. Un tratamiento saca a la vaca
del tanque de leche; un parto abre una lactancia, da de alta la cría y actualiza la
categoría de la madre; una inseminación descuenta una pajuela del stock. Eso es lo que
distingue al sistema de una planilla mejor organizada.

Sobre el cumplimiento de los objetivos específicos, las pruebas de la sección 2.3
permiten decir algo concreto y no una impresión general, porque los objetivos están
redactados para verificarse contra una pantalla o un comportamiento.

**Se pueden dar por cumplidos contra la evidencia ejecutada**: la centralización del
rodeo en un único repositorio y la consulta de la información histórica en una sola
pantalla, verificadas en la ficha integral, que reúne datos, linaje, sanidad,
reproducción y producción a partir del número de caravana; la reducción de los errores de
transcripción, verificada en los valores que el sistema calcula y nadie escribe —la
categoría propuesta, la fecha probable de parto, el fin del descarte de leche, el número
de lactancia—; el seguimiento del ciclo reproductivo completo, con celo, servicio, tacto y
parto encadenados y el estado de la hembra actualizándose solo; el control de insumos, con
el descuento al aplicarlos, la alerta de stock mínimo y el aviso de vencimiento por
partida; y la reducción de la carga operativa del registro diario, que es lo que verifican
las cuatro pruebas de integración: el dato se carga una vez y el sistema propaga sus
consecuencias.

**Queda por verificar con la encargada** el objetivo que depende de su uso y no del
sistema: que la información disponible mejore efectivamente sus decisiones. Eso sale de la
sesión de trabajo que se relata en la sección 2.8, y no de una prueba funcional.

La sesión lo confirmó en la medida en que una sola jornada puede confirmarlo. Al abrir el
tablero, la encargada lo leyó como la lista de trabajo del día —qué vaca revisar, qué parto
se acerca, qué insumo reponer— y no como un resumen, y señaló el descarte de leche como el
dato que más le preocupa olvidar y que hoy depende de su memoria. Eso muestra que la
información llega a la decisión **en el momento en que la decisión se toma**, que es lo que
el objetivo pedía. Lo que una sesión no puede mostrar es el efecto sostenido —menos tactos
atrasados, ninguna leche en descarte en el tanque—, y eso sólo se va a ver con una
temporada de uso sobre el rodeo real.

## Puntos a mejorar

**Las pruebas son funcionales y manuales, y no hay pruebas automatizadas.** Fue una
decisión, no un olvido, y el plan de testing del anteproyecto promete exactamente eso
—pruebas de caja negra sobre cada funcionalidad, documentadas con sus datos, su resultado
esperado y su resultado obtenido—; de hecho, la mención a pruebas de caja blanca se quitó
del plan justamente para no comprometer algo que el proyecto no iba a hacer. Pero el costo
está y hay que nombrarlo: **cada cambio obliga a repetir el recorrido a mano**, y las
reglas que cruzan módulos son las que más se benefician de una prueba que se corra sola.
Es lo primero que agregaría el equipo si el sistema siguiera creciendo.

**La validación con la clienta confirmó más de lo que exploró.** Mostrarle cada incremento
a medida que se construía evitó las sorpresas del final, y eso es un acierto. Pero tuvo un
efecto que recién se ve con el proyecto terminado: **casi ninguna propuesta de cambio nació
de ella**. Las llevó el equipo, en general a partir de una observación del tutor. Una
validación que solamente confirma no encuentra lo que todavía falta pedir, y la forma de
haberlo evitado no era pedirle opiniones sino ponerla a operar el sistema antes: la
sesión en la que ella lo usa sin que nadie se lo muestre debería haber ocurrido en la
mitad del proyecto y no al final.

**Las dos formas de cargar la leche se confunden desde el menú.** Fue lo único que le
costó a la encargada en la capacitación: buscó el ordeñe del turno en *Control lechero*
antes de encontrarlo en *Ordeñe por lote*. El manual ya lo advierte como la duda más
frecuente, pero una advertencia en el manual es la solución cara. La barata es que el menú
lo diga solo —una línea debajo de cada entrada, «litros del tanque, cada turno» y «litros
por vaca, una vez por mes»—, o que las dos entradas se llamen por lo que la encargada
anota y no por el nombre técnico de la práctica.

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

**La validación está escrita dos veces y no dice lo mismo en los dos lados.** Al ejecutar
las pruebas, treinta y un casos terminaron con el mensaje del navegador y no con el del
sistema: el campo declara que es obligatorio, o que tiene un mínimo, y el formulario nunca
llega al servidor. La validación del servidor existe igual, que es lo correcto, pero **la
usuaria ve un mensaje distinto del que el documento describe**, y en inglés cuando el
navegador está en inglés. Unificar los dos textos es barato y se nota.

**No hay registro de quién hizo cada cosa.** El sistema tiene un único par de credenciales
—es una decisión de alcance, porque hay una sola persona a cargo de los registros— pero la
consecuencia es que no queda traza de las modificaciones. Si el tambo incorpora personal
que también cargue datos, el esquema de acceso es lo primero que habría que revisar.

## Conclusión final

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

Hay una imagen que resume el proyecto mejor que cualquier cifra, y es la de la sesión de
capacitación: la encargada abriendo el tablero y leyéndolo como la lista de lo que tiene
que hacer ese día. Para eso se construyó el sistema. No para guardar datos —eso ya lo hacía
el cuaderno— sino para que **el dato que se anota hoy aparezca mañana en el lugar donde hace
falta**, sin que nadie tenga que acordarse de ir a buscarlo.

El sistema queda ahora en manos de quien lo va a usar, y lo que sigue ya no depende del
equipo sino del uso: una temporada de partos sobre el rodeo real va a encontrar lo que
ninguna prueba encontró y lo que ninguna validación llegó a pedir. Lo que el equipo se
propone conservar de este trabajo es una costumbre más que un resultado: preguntar antes
de suponer, verificar lo que el documento afirma contra lo que el sistema hace, y contar
los errores con la misma precisión que los aciertos.
