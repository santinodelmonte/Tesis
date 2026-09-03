# Revisión de ingeniería de software sobre el documento

Lectura del anteproyecto y del proyecto con el criterio con que los va a leer un
tribunal: **¿cada requerimiento se puede verificar? ¿cada caso de uso está bien
formado? ¿el documento se contradice consigo mismo?**

Criterios aplicados: **IEEE 830 / ISO-IEC-IEEE 29148** para las características de un
requerimiento —necesario, verificable, inequívoco, atómico, consistente, trazable,
independiente de la implementación—, **Cockburn y Larman** para los casos de uso, y
**SMART** para los objetivos.

---

## 0. Una calibración, antes de empezar

`EjemploTesis.pdf` es el modelo que dio el tutor, y **hace algunas cosas que el
estándar desaconseja**: sus 26 casos de uso son todos «Tipo: Primario», sus
precondiciones son «El actor debe estar logueado», y sus cursos básicos dicen «pulsa
el botón confirmar».

Así que esta revisión **no marca nada de eso**. Alejarse del modelo de la cátedra
para acercarse a un libro es un mal negocio: el tribunal evalúa contra su propia
pauta. Lo que sí se marca es lo que hace que **el documento no se pueda verificar o
se contradiga consigo mismo**, que es un problema en cualquier pauta.

Los hallazgos van ordenados por lo que costaría defenderlos en la mesa.

---

## 1. Grave — Los objetivos específicos tienen porcentajes que nadie puede medir

**La pauta.** Un objetivo es SMART si es medible: hay una forma de saber si se
cumplió. Un número sin línea base ni método de medición **no es medible**, y es peor
que no poner número, porque aparenta rigor.

**Lo que dice el documento.** Cinco de los diez objetivos específicos traen
porcentaje:

| Objetivo | Pregunta que va a hacer el tribunal |
|---|---|
| «reducir en al menos un **80 %** la dispersión de datos» | ¿Cómo se mide la dispersión de datos? ¿Cuánta había? |
| «disminuir los errores de transcripción en al menos un **60 %**» | ¿Cuántos errores había por mes antes? ¿Quién los contó? |
| «reducir en un **50 %** el tiempo de consulta de información histórica» | ¿Cuánto tardaba antes? ¿Medido cómo? |
| «reducir situaciones de faltante en al menos un **50 %**» | ¿Cuántos faltantes hubo el año pasado? |
| «reducir en un **40 %** el tiempo operativo de registro» | ídem |

Ninguno tiene línea base. **Nadie midió el estado anterior**, así que ninguno de los
cinco se puede declarar cumplido ni incumplido.

**Por qué importa.** Es el mismo problema que casi se les cuela en RF3.10, pero en la
sección que el tribunal lee primero. Y tiene una trampa peor: en 2.9 Conclusiones hay
que decir si los objetivos se cumplieron. Con estos, **no se puede decir nada
honesto**.

**Cómo se corrige.** Dos caminos, y los dos son válidos:

- **Poner la línea base.** Preguntarle a la encargada cuánto tarda hoy en encontrar
  la historia de un animal, cuántas veces se quedó sin una pajuela el año pasado. Con
  ese dato el porcentaje se sostiene.
- **Cambiar la métrica por una que sí se pueda verificar.** «Reducir a un único
  repositorio los registros del rodeo, hoy dispersos en cuadernos y pizarrones» es
  verificable —se cumple o no— y no necesita un 80 % inventado.

**El segundo camino es el que recomiendo**, salvo para el tiempo de consulta, que sí
se puede medir con un cronómetro en una tarde.

**Y hay un problema de consistencia dentro de la misma lista:** cinco objetivos
tienen número y cinco no —«mejorar el control sanitario», «optimizar el seguimiento
reproductivo»—. Uno de ellos dice **«logrando un mayor control total»**, que además
de no ser medible es una contradicción: o es mayor, o es total.

---

## 2. Grave — RF1.1 contradice a RF1.8, y ninguno de los dos describe lo que hace el sistema

**La pauta.** Los requerimientos tienen que ser **consistentes entre sí**: dos
requerimientos no pueden afirmar cosas incompatibles sobre la misma funcionalidad.

**Lo que dice el documento.**

> **RF1.1** — «…registrar nuevos animales **ingresando** número de caravana único,
> fecha de nacimiento, sexo, raza **y categoría inicial**».
>
> **RF1.8** — «El sistema debe **clasificar automáticamente** a los animales».

Uno dice que la categoría la ingresa el usuario; el otro, que la calcula el sistema.

**Lo que hace el código.** Ninguna de las dos, exactamente: la pantalla de alta trae
el campo en «La calcula el sistema», el usuario aprieta **Calcular Categoría** y el
sistema **propone** una que el usuario puede aceptar o cambiar. Es una propuesta, no
una imposición ni una carga manual.

**Por qué importa.** Es una divergencia de tres vías que la auditoría no había
detectado, porque miraba números y no redacción. Y la clasificación automática es
**la regla de negocio más vistosa del sistema**: es lo primero que se muestra en una
demo.

**Cómo se corrige.** RF1.1 pasa a decir «…y la categoría que el sistema propone a
partir del sexo, la fecha de nacimiento y la cantidad de partos, que el usuario puede
aceptar o sustituir». Con eso los dos requerimientos dicen lo mismo que el sistema.

---

## 3. Moderado — Seis requerimientos no son verificables

**La pauta.** Un requerimiento es verificable si existe un procedimiento finito para
decidir si el sistema lo cumple. «El sistema debe permitir actualizar información» no
lo tiene: ¿qué información?

| RF | Qué dice | Por qué no se puede verificar | Propuesta |
|---|---|---|---|
| **RF1.3** | «actualizar información de animales previamente registrados» | No dice qué campos. ¿La caravana? ¿El sexo? Un sistema que no deja cambiar nada lo cumple igual | Enumerar qué es modificable y qué no, que es donde está la regla de verdad |
| **RF2.4** | «calcular la producción total diaria del establecimiento» | Con RF2.3 en la mano —lote y control individual no se suman en el mismo turno— no se sabe qué entra en ese total | Decir cómo se compone: la suma de los turnos del día, tomando el lote cuando existe y los controles individuales cuando el turno se anotó sólo vaca por vaca |
| **RF2.5** | «**almacenar** el histórico de los controles individuales» | «Almacenar» no es observable desde afuera. Es una afirmación sobre la implementación, no sobre el comportamiento | «Registrar y permitir consultar el histórico…» |
| **RF3.4 + RF3.5** | «registrar controles reproductivos (palpaciones)» / «registrar resultados positivos o negativos de preñez» | Son una sola capacidad partida en dos, y **ninguna está completa sola**: RF3.4 no dice que el tacto tiene resultado, RF3.5 no dice de qué evento es | Fundirlos en un RF3.4 que registre el tacto con su fecha, su animal y su resultado |
| **RF5.2** | «registrar cantidades iniciales disponibles» | Está contenido en RF5.1 y en RF5.9, y «inicial» choca con RF5.7, que lleva el stock por partida | Quitarlo, o convertirlo en la carga inicial del inventario al empezar a usar el sistema, que es otra cosa y sí merece un RF |
| **RF7.5** | «El sistema debe **integrarse con un bot de Telegram**» | No dice qué resultado produce. Un sistema que se conecta al bot y no manda nada lo cumple. Además nombra la herramienta en vez del comportamiento | «Enviar las notificaciones por un canal de mensajería configurable, y permitir registrar y verificar el destinatario». Que sea Telegram es decisión de diseño, y ya está justificada en el estudio de alternativas |

---

## 4. Moderado — Dos requerimientos no funcionales no son medibles

**La pauta.** Un RNF sin métrica es una declaración de intención. Los de este
documento están casi todos bien —y conviene decirlo—:

| RNF | Métrica | ¿Verificable? |
|---|---|---|
| Disponibilidad | 95 % del tiempo | sí |
| Rendimiento | 5 segundos en consulta y registro | sí |
| Accesibilidad | WCAG 2.1 nivel AA | sí |
| Compatibilidad | desde 375 px de ancho | sí |
| Fiabilidad | transacciones en escrituras multi-tabla | sí |
| Seguridad | credenciales fuera del código, consultas parametrizadas | sí |
| Portabilidad | sin instalación, sólo navegador | sí |
| **Usabilidad** | «interfaz simple, intuitiva y de fácil aprendizaje» | **no** |
| **Mantenibilidad** | «permita su mantenimiento… sin afectar el funcionamiento general» | **no** |

**Cómo se corrigen.**

- **Usabilidad** tiene una métrica natural y al alcance: *«la encargada completa sin
  asistencia las cinco tareas de uso diario —registrar el ordeñe, un celo, un
  servicio, un tratamiento y consultar la ficha de un animal— después de una única
  sesión de capacitación»*. Se verifica el día de la capacitación y **de paso da
  material para la sección 2.8**.
- **Mantenibilidad** se apoya en lo que ya hicieron: *«el sistema se organiza en tres
  capas —presentación, dominio y persistencia— de modo que un cambio en el acceso a
  datos no obligue a modificar las pantallas»*. Eso se verifica mirando el código.

---

## 5. Moderado — Los actores del sistema y los interesados del proyecto están mezclados

**La pauta.** Un **actor** es quien interactúa con el sistema. Un **interesado** es
quien tiene algo en juego pero no lo usa. Van en listas distintas, porque los actores
salen en el diagrama de casos de uso y los interesados no.

**Lo que dice el documento.** «Actores Involucrados» enumera cinco:

| Declarado | ¿Interactúa con el sistema? |
|---|---|
| Sofía, encargada | **Sí** — es la única |
| Tamberos | No: registran en cuadernos y pizarrones para que Sofía cargue |
| Juan Vila, dueño | No: recibe reportes en papel |
| Médico veterinario | No: usa la información, no el sistema |
| **Santino y Alejo, «Administradores del Sistema»** | **No, y no deberían** |

El último es el que hay que arreglar sí o sí: **son los desarrolladores**, no usuarios
del sistema. Y ponerlos como actores choca de frente con **RF0.1**, que dice que hay
un único par de credenciales y ninguna administración de usuarios. Un tribunal que
cruce las dos cosas pregunta con qué usuario entran los administradores.

**Y hay una consecuencia visible:** los diagramas de casos de uso de la sección 2.2.1
tienen **un solo actor**, porque es la verdad. Contra una lista de cinco, parece un
error de los diagramas cuando en realidad el error está en la lista.

**Cómo se corrige.** Partir la sección en dos: **Actores del sistema**, con Sofía
sola, y **Otros interesados**, con los tamberos, el dueño y el veterinario, diciendo
en cada caso qué reciben. Los desarrolladores salen de ahí: su lugar es «Integrantes
y Roles», donde ya están.

---

## 6. Menor — CU49 tiene al sistema como actor principal

**La pauta.** El actor principal de un caso de uso es **quien persigue la meta**. Un
proceso que corre solo no persigue nada: se dispara por tiempo.

**Lo que dice el documento.** CU49 declara «Actores: Sistema (actor principal);
Encargada del sector (destinataria)». Es el único de los 49 así, y se nota que lo
escribieron con incomodidad.

**Cómo se corrige.** El actor principal es la **encargada**, que es quien quiere
enterarse de sus tareas del día; el desencadenante es **temporal** —«son las 6 de la
mañana»—. El curso básico arranca en el sistema, y eso está perfectamente permitido.
Alcanza con cambiar dos campos.

Es menor porque no afecta al sistema ni a nadie más, pero es de las cosas que un
tribunal pregunta justamente porque salta a la vista.

---

## 7. Menor — Las precondiciones no siguen un criterio único

**La pauta (matizada por el modelo de la cátedra).** La precondición describe el
estado que tiene que valer antes de arrancar. Que el usuario esté logueado vale para
los 49 casos: no distingue nada. **El ejemplo del tutor hace lo mismo**, así que no
hay que sacarlo — pero sí hay que ser consistente.

**Lo que dice el documento.** De 49 casos de uso, **20 dicen sólo** «El usuario debe
estar logueado en el sistema». Y hay tres que **no lo dicen** y en cambio ponen la
condición real:

> **CU21** — «La hembra debe estar registrada y en condiciones de recibir servicio. En
> la inseminación artificial debe existir stock de la pajuela seleccionada.»

Ésa es una buena precondición. El problema es que conviven dos criterios en el mismo
documento.

**Cómo se corrige.** Dejar «el usuario logueado» como premisa general enunciada una
vez —al presentar los casos de uso— y que **cada precondición diga además la
condición propia**, como ya hace CU21. Para los 20 flojos hay que pensar cuál es: en
CU24 Registrar Parto es que exista una hembra preñada o servida; en CU16 Registrar
Secado, que el animal tenga una lactancia abierta. **Ese ejercicio vale la pena por sí
mismo**: obliga a decir qué tiene que ser verdad antes, y ahí suelen aparecer reglas
de negocio que no estaban escritas.

---

## 8. Lo que está bien y conviene no tocar

Para que la lista de arriba no dé una impresión equivocada:

- **La trazabilidad es impecable.** 74 requerimientos, 74 referenciados por casos de
  uso, ninguno huérfano y ninguno inventado. Es lo primero que un tribunal verifica y
  lo que más seguido falla.
- **La plantilla de caso de uso está completa** y sigue la del ejemplo: nombre,
  actores, tipo, descripción, referencia a RF, precondición, desencadenante, curso
  básico, alternativos, excepciones, postcondición, reglas de negocio, validaciones y
  frecuencia de uso.
- **Los nombres de los casos de uso son verbo + objeto** en los 49: *Registrar Alta de
  Animal*, *Consultar Linaje*, *Configurar Plan Sanitario*. Suena obvio y casi nunca
  se cumple.
- **Los requerimientos que fijan números están bien** y coinciden con el código: los
  cinco criterios de descarte, los nueve indicadores, el tope del lote, las cinco
  advertencias del parto.
- **Los cursos alternativos y de excepción están numerados contra el paso** al que
  corresponden (`1a.`, `4a.`), que es exactamente como se hace.
- **Siete de los nueve RNF son medibles**, que es una proporción muy por encima de lo
  habitual en un trabajo de este tipo.

---

## 9. Por dónde empezar

| # | Hallazgo | Esfuerzo | Riesgo si queda |
|---|---|---|---|
| 1 | Objetivos con porcentajes sin línea base | Medio — hay que decidir | **Alto**: bloquea escribir 2.9 con honestidad |
| 2 | RF1.1 contra RF1.8 | Bajo — una frase | **Alto**: es la regla estrella del sistema |
| 5 | Actores mezclados con interesados | Bajo | **Alto**: choca con RF0.1 y con los diagramas |
| 3 | Seis RF no verificables | Medio | Medio |
| 4 | Usabilidad y Mantenibilidad sin métrica | Bajo | Medio |
| 6 | CU49 con el sistema como actor | Bajo — dos campos | Bajo, pero salta a la vista |
| 7 | Precondiciones sin criterio único | **Alto** — son 20 | Bajo |

**Los tres primeros son los que yo arreglaría antes de mostrarle el documento a
nadie.** El 7 es el más caro y el menos urgente: conviene hacerlo mientras se escribe
el manual de usuario, que obliga a recorrer cada pantalla y es cuando las
precondiciones reales aparecen solas.
