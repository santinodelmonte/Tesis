# Cambios del anteproyecto — v6 a v7

Segunda tanda de correcciones sobre `Anteproyecto_v5.docx`. La primera —la v6— hizo
que los requerimientos dijeran lo que el sistema hace; ésta corrige **cómo están
escritos**, contra los hallazgos de `docs/revision-tutor.md`, aprobados el 26/08/2026.

Se aplican con el bloque `v7` de `docs/editar_anteproyecto.py`, que sigue partiendo
del v5. La salida pasa a ser `Anteproyecto_v7.docx`.

**Ningún requerimiento se agregó, se quitó ni se renumeró: siguen siendo 74.** Era la
condición para no tocar los 49 casos de uso que los referencian.

---

## 1. Actores del sistema, separados de los interesados

La lista mezclaba a quien opera el sistema con quien recibe su información, y metía
adentro a los propios desarrolladores.

| Antes | Ahora |
|---|---|
| Sofía (Encargada del Sector): Usuaria Directa/Principal | **único actor del sistema**, con la referencia a RF0.1 |
| Tamberos (Actores Operativos) | **interesados operativos** — registran en papel, no operan el sistema |
| Juan Vila (Dueño): Actor Indirecto/Interesado | **interesado** — recibe reportes, no opera el sistema |
| Médico Veterinario: Actor Externo/Consultor | **interesado externo** — no opera el sistema |
| Administradores del Sistema (Santino y Alejo) | **eliminado** |

**Por qué salieron los desarrolladores.** No son usuarios del sistema, y ponerlos como
actores contradecía RF0.1 —un único par de credenciales, sin administración de
usuarios—. Su lugar es «Integrantes y Roles», donde ya estaban.

**Y de paso se explica algo que parecía un error.** Los diagramas de casos de uso de
la sección 2.2.1 tienen **un solo actor**, porque Sofía es la única que ingresa al
sistema. Contra una lista de cinco parecía que faltaban actores; el problema estaba en
la lista.

## 2. Los objetivos específicos ya no prometen porcentajes que nadie midió

Cinco de los diez prometían reducir algo un 80 %, un 60 %, un 50 % o un 40 %. **No
existe línea base de ninguno**: nadie midió cuánto tardaba la encargada en encontrar
la historia de un animal ni cuántos faltantes de stock hubo el año pasado. Sin ese
dato, ninguno se puede declarar cumplido ni incumplido — y en 2.9 Conclusiones hay que
decir si se cumplieron.

Se reemplazan por metas que se verifican mirando el sistema terminado:

| Antes | Ahora, en una línea |
|---|---|
| «reducir en al menos un **80 %** la dispersión de datos» | Centralizar en un único repositorio lo que hoy está en cuadernos y pizarrones |
| «disminuir los errores de transcripción en al menos un **60 %**» | Sustituir la copia manual por un único registro validado, con los datos derivados calculados y no escritos a mano |
| «reducir en un **50 %** el tiempo de consulta» | La ficha integral se obtiene en una sola pantalla a partir de la caravana |
| «reducir faltantes en al menos un **50 %**» | Descontar el stock al aplicar, avisar al llegar al mínimo y anticipar el vencimiento por partida |
| «reducir en un **40 %** el tiempo operativo» | Cada evento se anota una sola vez y el sistema propaga sus consecuencias |

Se corrigió además **«logrando un mayor control total»**, que además de no ser medible
se contradecía: o es mayor, o es total.

## 3. Cinco requerimientos que no se podían verificar

| RF | El problema | Qué dice ahora |
|---|---|---|
| **RF1.1** | Decía que la categoría **se ingresa**, mientras RF1.8 decía que el sistema **la calcula**. El sistema hace una tercera cosa | Que el sistema **propone** la categoría a partir del sexo, la fecha de nacimiento y los partos, y el usuario **acepta o sustituye** |
| **RF1.3** | «Actualizar información» sin decir cuál: un sistema que no deja cambiar nada lo cumplía igual | Enumera los campos modificables, **leídos de `ModificarAnimal.cshtml`** |
| **RF2.4** | «Producción total diaria» era ambiguo con RF2.3 en la mano, que dice que el lote y el control individual no se suman en el mismo turno | Dice cómo se compone: el lote cuando existe, la suma de los controles cuando el turno se anotó sólo animal por animal, de modo que nada se cuente dos veces |
| **RF2.5** | «**Almacenar** el histórico» es una afirmación sobre la implementación, no observable desde afuera | «Registrar… y permitir consultarlos», imputados a la lactancia en curso |
| **RF7.5** | «Integrarse con un bot de Telegram» no decía qué resultado produce —un sistema que se conecta y no manda nada lo cumplía— y nombraba la herramienta en vez del comportamiento | Pasa a ser **Configuración del canal de notificaciones**: registrar el destinatario y verificar la conexión con un mensaje de prueba |

> **RF1.3 casi sale mal, y vale anotarlo.** La primera redacción decía «el número de
> caravana identifica al animal y no se modifica». Suena razonable y es **falso**:
> `ModificarAnimal.cshtml` deja editarlo, para corregir una caravana mal tipeada. Se
> verificó contra el código antes de escribirlo. Es exactamente el error que esta tanda
> venía a evitar, cometido mientras se la evitaba.

## 4. Dos requerimientos no funcionales que no eran medibles

Los otros siete ya lo eran. A estos dos se les agregó su criterio de verificación, sin
tocar lo que ya decían:

- **Usabilidad** — *«que la encargada complete sin asistencia las cinco tareas de uso
  diario —el ordeñe del turno, un celo, un servicio, un tratamiento y consultar la
  ficha de un animal— después de una única sesión de capacitación»*. Se comprueba el
  día de la capacitación, y **da material para la sección 2.8**.
- **Mantenibilidad** — *«la separación en tres capas —presentación, dominio y
  persistencia—, de modo que un cambio en el acceso a datos no obligue a modificar las
  pantallas ni la lógica de negocio»*. Se comprueba leyendo el código.

## 5. CU49 deja de tener al sistema como actor principal

No es del anteproyecto, pero forma parte de la misma tanda. Se corrigió en
`docs/casos_de_uso_parte2.py`:

| Campo | Antes | Ahora |
|---|---|---|
| Actores | Sistema (actor principal); Encargada del sector (destinataria) | **Encargada del sector** |
| Descripción | «El sistema envía…» | «La encargada recibe, sin tener que pedirlo…» |
| Desencadenante | «Se alcanza la hora programada» | «Se alcanza la hora configurada… **lo dispara el tiempo, no una acción de la encargada**» |

El actor principal es quien persigue la meta, y quien quiere enterarse de sus tareas
del día es la encargada. Que el curso básico arranque en el sistema está permitido: es
un caso de uso disparado por tiempo.

## 6. El plan de testing ya no promete caja blanca

**Criterio: si el ejemplo de la cátedra no documenta pruebas de caja blanca, nosotros
tampoco — y lo que el anteproyecto prometía se corrige.** Un plan que compromete algo
que el proyecto después no hace es peor que un plan más corto.

| Antes | Ahora |
|---|---|
| «se realizarán pruebas de caja negra **y caja blanca**» | «se realizarán pruebas de caja negra sobre cada funcionalidad… documentadas con los datos utilizados, el resultado esperado y el obtenido» |
| Subtítulo **Pruebas de Caja Blanca** con su párrafo | **Eliminado** |

**Lo que la caja blanca aportaba no se pierde.** Los casos de borde de los cálculos que
el sistema resuelve solo —la categoría en la edad de cambio, el alcance de la
verificación de consanguinidad, la estimación de una lactancia abierta contra una
cerrada— se siguen verificando, pero **desde la pantalla**, como pruebas funcionales.
Quedan mencionados en el objetivo de las pruebas de caja negra y documentados en la
sección 2.3.

---

## Lo que queda pendiente

- **Fusionar RF3.4 con RF3.5 y quitar RF5.2** quedó para discusión aparte: las dos
  cosas cambian la cantidad de requerimientos, obligan a renumerar y tocan los casos de
  uso que los referencian.
- **Las precondiciones de los casos de uso** —20 de 49 dicen sólo «el usuario debe
  estar logueado»— se corrigen mientras se escribe el manual de usuario, que obliga a
  recorrer cada pantalla y es cuando la precondición real aparece sola.
- **RF7.5 ahora especifica un mensaje de prueba** que el Módulo 7 todavía no
  implementa. Es una especificación de lo que falta construir, no una divergencia — pero
  **hay que construirla**, o el requerimiento queda incumplido.
