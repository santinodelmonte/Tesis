# 2.8 Grado de satisfacción del cliente

Fuente de la sección: el plan de iteraciones del anteproyecto, el relevamiento que dio
origen al registro rápido del tablero, el renombre de «novilla» a «vaquillona», el
criterio de verificación del RNF1 de Usabilidad y las instancias de validación de cada
incremento.

Sigue la forma del ejemplo del tutor: **el relato de la relación con la clienta a lo
largo del proyecto, no una encuesta.** Es una página, sin imágenes.

> **Quedan dos `[COMPLETAR: …]`, los dos de la misma sesión**: la capacitación con la
> encargada, que es donde se verifica el RNF1 de Usabilidad y donde ella ve el sistema
> terminado. El resto de la sección está escrito sobre hechos que dejaron rastro en el
> proyecto.

---

El sistema tiene una sola usuaria, y eso le dio a la relación con la clienta una forma
distinta de la habitual: **no hubo un cliente que encarga y un usuario que padece**. La
encargada del establecimiento es la persona que describió el problema, la que validó cada
incremento y la que va a operar el sistema todos los días. No hubo intermediarios ni
traducción de requerimientos.

El contacto arrancó antes del desarrollo, con el relevamiento de la operativa del tambo:
cómo se anota hoy el ordeñe, dónde queda escrito un celo, qué se hace cuando una vaca
entra en tratamiento. De ahí salieron los requerimientos, y también el vocabulario. El
caso más claro es el de una categoría del rodeo: el documento la llamaba «novilla», que
es el término de manual, y en el tambo nadie la llama así. Se cambió el sistema, la base
de datos y el documento para que dijeran **«vaquillona»**, que es la palabra que ella usa.
Parece un detalle y no lo es: un sistema que nombra las cosas distinto de como las nombra
quien lo usa obliga a traducir mentalmente en cada pantalla.

Durante el desarrollo el contacto fue **telefónico, con una frecuencia aproximada de
quince días**, y se acortó cuando hizo falta —al cerrar un incremento, o cuando una regla
del tambo admitía más de una interpretación y no se podía avanzar sin preguntar—. La
elección del medio no fue caprichosa: los horarios del tambo se organizan alrededor de
los dos ordeñes, y una llamada corta entraba donde una reunión no entraba.

Conviene decir con precisión de dónde vinieron los cambios, porque es el punto donde esta
sección puede sonar más complaciente de lo que fue. **La encargada prácticamente no
propuso modificaciones.** Las propuestas de cambio las llevó el equipo, casi siempre a
partir de una observación del tutor, y se decidían con ella: se le explicaba qué se
proponía cambiar y por qué, y la decisión se tomaba en conjunto. El caso más importante es
el de la pantalla de inicio. El tutor señaló que el tablero —hasta agosto, un resumen de
lo que estaba pasando en el rodeo— no era lo que más iba a usarse. Al revisar con ella qué
se carga efectivamente todos los días apareció el dato que ningún requerimiento había
registrado: los cuatro eventos reproductivos —celo, servicio, tacto y parto— se anotan a
diario, uno por animal, y llegar a cada uno costaba cuatro clics de menú. El tablero se
rehízo para que **dos de esos eventos se resuelvan en la propia pantalla de inicio** y los
otros dos abran su formulario con la caravana ya cargada. Es la funcionalidad que más uso
diario va a tener y no estaba en el anteproyecto.

Las instancias de validación entre un incremento y el siguiente fueron, en general,
**instancias sin sorpresas**: la encargada llegaba a cada una sabiendo qué se había
construido, porque lo que se mostraba era lo que se había conversado. Casi no hubo
correcciones ni pedidos nuevos. Eso admite dos lecturas y la honesta incluye las dos. La
favorable es que el relevamiento inicial fue lo bastante detallado como para que el
sistema no se desviara, y que mostrar cada incremento a medida que se construía evitó que
las diferencias se acumularan hasta volverse caras. La menos favorable es que **un cliente
que no propone cambios tampoco está poniendo el sistema a prueba**: la validación
confirmaba que lo construido era lo pedido, pero no siempre exploraba lo que todavía
faltaba pedir. Es una de las razones por las que la verificación que sigue importa más que
cualquier opinión recogida en el camino.

Porque sobre el sistema terminado, lo que vale no es una impresión sino una prueba: el
requerimiento no funcional de Usabilidad se da por cumplido si la encargada **completa sin
asistencia las cinco tareas de uso diario** —registrar el ordeñe del turno, un celo, un
servicio, un tratamiento y consultar la ficha de un animal— después de una única sesión de
capacitación. Es un criterio que se verifica mirándola trabajar, no preguntándole si le
gustó, y es la primera instancia del proyecto en la que ella usa el sistema sin que nadie
se lo esté mostrando.

[COMPLETAR: resultado de la sesión de capacitación —si completó las cinco tareas sin
ayuda, cuáles le costaron, qué hizo falta explicar dos veces—. Con eso se cierra la
verificación del RNF1 de Usabilidad.]

[COMPLETAR: qué dijo al ver el sistema terminado, con sus palabras. Si señaló algo que
falta o que haría distinto, va acá y también en los puntos a mejorar de la sección 2.9.]
