# 2.8 Grado de satisfacción del cliente

Fuente de la sección: el plan de iteraciones del anteproyecto, el relevamiento que dio
origen al registro rápido del tablero, el renombre de «novilla» a «vaquillona», y el
criterio de verificación del RNF1 de Usabilidad.

Sigue la forma del ejemplo del tutor: **el relato de la relación con la clienta a lo
largo del proyecto, no una encuesta.** Es una página, sin imágenes.

> **Lo que está marcado `[COMPLETAR: …]` sale de la sesión de trabajo con la encargada y
> no del repositorio.** Es lo que ella diga al ver el sistema terminado. El resto de la
> sección está escrito sobre hechos que dejaron rastro en el proyecto.

---

El sistema tiene una sola usuaria, y eso le dio a la relación con la clienta una forma
distinta de la habitual: **no hubo un cliente que encarga y un usuario que padece**. La
encargada del establecimiento es la persona que describió el problema, la que validó cada
incremento y la que va a operar el sistema todos los días. Cada cosa que pidió la pidió
para sí misma.

El contacto arrancó antes del desarrollo, con el relevamiento de la operativa del tambo:
cómo se anota hoy el ordeñe, dónde queda escrito un celo, qué se hace cuando una vaca
entra en tratamiento. De ahí salieron los requerimientos, y también el vocabulario. El
caso más claro es el de una categoría del rodeo: el documento la llamaba «novilla», que es
el término de manual, y en el tambo nadie la llama así. Se cambió el sistema, la base de
datos y el documento para que dijeran **«vaquillona»**, que es la palabra que ella usa.
Parece un detalle y no lo es: un sistema que nombra las cosas distinto de como las nombra
quien lo usa obliga a traducir mentalmente en cada pantalla.

La metodología incremental puso una instancia de validación entre un incremento y el
siguiente, y ahí es donde la relación dejó marcas verificables. **La más importante cambió
la pantalla principal del sistema.** El tablero de inicio era, hasta agosto, un resumen de
lo que estaba pasando en el rodeo. Al mirar con ella lo que se carga efectivamente todos
los días apareció un dato que ningún requerimiento había registrado: los cuatro eventos
reproductivos —celo, servicio, tacto y parto— se anotan a diario, uno por animal, y llegar
a cada uno costaba cuatro clics de menú. El tablero se rehízo para que **dos de esos
eventos se resuelvan en la propia pantalla de inicio** y los otros dos abran su formulario
con la caravana ya cargada. Es la funcionalidad que más uso diario va a tener y no estaba
en el anteproyecto: salió de mirar cómo se trabaja.

[COMPLETAR: cómo fue el contacto en el día a día —con qué frecuencia, en persona o por
teléfono, si costó coordinar— y qué pidió la encargada en cada instancia de validación.]

Sobre el sistema terminado, la verificación que importa no es una opinión sino una
prueba: el requerimiento no funcional de Usabilidad se da por cumplido si la encargada
**completa sin asistencia las cinco tareas de uso diario** —registrar el ordeñe del turno,
un celo, un servicio, un tratamiento y consultar la ficha de un animal— después de una
única sesión de capacitación. Es un criterio que se verifica mirándola trabajar, no
preguntándole si le gustó.

[COMPLETAR: si completó las cinco tareas sin ayuda, cuáles le costaron y qué hizo falta
explicarle dos veces.]

[COMPLETAR: qué dijo al ver el sistema terminado, con sus palabras. Si señaló algo que
falta o que haría distinto, va acá y también en los puntos a mejorar de la sección 2.9.]
