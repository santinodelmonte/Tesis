# Reglas de negocio y usabilidad agregadas

Incremento posterior a los Módulos 4 y 5. No agrega casos de uso: cierra huecos del
modelo de negocio que el software dejaba pasar, y resuelve dos problemas de uso que
aparecen apenas el rodeo tiene volumen real.

Nada de esto está en el documento de Proyecto todavía. Al final de cada punto queda
anotado qué hay que corregir ahí.

---

## 1. Validaciones bloqueantes que faltaban

### 1.1 Una ternera no entra en servicio ni manifiesta celo

El sistema aceptaba registrarle un servicio a una hembra de cualquier edad. Recién
cuando nacía la cría, `ValidarGenealogia` rechazaba a una madre demasiado joven —o
sea, el error se descubría nueve meses después del dato que lo originó, con el parto
ya cargándose.

Ahora el control está en el momento en que el dato se carga:

| Evento | Edad mínima | Constante |
|---|---|---|
| Detección de celo | 9 meses | `EDAD_MINIMA_CELO_MESES` |
| Servicio | 13 meses | `EDAD_MINIMA_SERVICIO_HEMBRA_MESES` |

Las dos edades son distintas a propósito: la vaquillona empieza a ciclar bastante
antes de estar en condiciones de ser servida, y anotarle el celo a los 10 meses es
información útil, no un error. Servirla a esa edad sí lo es.

La edad se mide contra la fecha del evento y no contra la fecha de hoy, para que la
carga retroactiva se valide con la edad que el animal tenía ese día.

### 1.2 Un animal dado de baja no protagoniza eventos posteriores a su baja

`ValidarCelo` y `ValidarServicio` rechazan el registro cuando la fecha del evento es
posterior a la baja del animal. Un evento **anterior** a la baja sigue permitido: en
el tambo se carga el servicio del mes pasado de una vaca que después se vendió, y eso
es lo normal, no un error.

La regla vive en `Controladora.EstabaActivo(pAnimal, pFecha)` y es la misma que usan
las advertencias del punto 2.

**Corregir en el Proyecto.** Sumar las dos edades mínimas y esta restricción a las
reglas de negocio de CU14 y CU15, y las constantes a la tabla de `bd/LEEME.md`.

---

## 2. Advertencias: lo que es raro pero puede ser cierto

El sistema tenía un solo tipo de respuesta ante un dato sospechoso: rechazarlo. En un
tambo eso no alcanza, porque hay datos que parecen errores y no lo son, y otros que
son correctos aunque el sistema no pueda demostrarlo.

Se agrega un segundo mecanismo. Los métodos `Advertencias*` de la Controladora
devuelven una lista de motivos; la pantalla los muestra en un cuadro amarillo, **no
guarda nada**, y ofrece un botón "Registrar de todos modos" que repite el envío
confirmado. Si el usuario corrige un dato en lugar de confirmar, las advertencias se
vuelven a evaluar desde cero: la confirmación viaja en el botón y no en un campo del
formulario, justamente para que no quede pegada.

La diferencia con las validaciones es deliberada: `Validar*` tiene las
imposibilidades —un animal padre de sí mismo, una ternera servida—; `Advertencias*`,
lo que hay que mirar antes de confirmar.

### 2.1 Servicio con un toro dado de baja

Si la monta natural es posterior a la baja del toro, se advierte y no se bloquea: la
baja suele registrarse días después del hecho, así que la fecha almacenada puede ser
posterior a la real, y trabar el registro obligaría a falsear una de las dos fechas.

**La inseminación artificial no entra en esta advertencia.** Una pajuela sigue
sirviendo años después de que el toro murió, y usar semen de un reproductor muerto es
lo habitual, no una inconsistencia. Distinguir los dos casos es lo que hace que la
advertencia signifique algo.

### 2.2 Parentesco entre la hembra y el reproductor

Al registrar el servicio se corre la verificación de consanguinidad de CU6 entre la
hembra y el reproductor —el toro del rodeo o el que aportó la pajuela— y se informa el
ancestro común. Antes esa verificación existía sólo como consulta separada, así que
dependía de que la usuaria se acordara de hacerla antes de servir.

### 2.3 Mellizos de distinto sexo: freemartin

Los partos dobles ya se podían registrar. Lo que faltaba era la consecuencia: cuando
los mellizos son de distinto sexo, la cría hembra nace **freemartin** —comparte
circulación con el hermano macho durante la gestación y en la enorme mayoría de los
casos queda estéril—. El sistema lo advierte en el momento del parto, que es cuando
sirve: si no, la vaquillona se cría como futura reposición y el problema se descubre
dos años después, cuando no prende con ningún servicio.

### 2.4 Duración de la gestación fuera de rango

Se compara la fecha del parto contra la del servicio del que viene la preñez:

- menos de `GESTACION_DIAS_MINIMA` (240) días: aborto, prematuro o fecha mal cargada;
- más de `GESTACION_DIAS_MAXIMA` (320) días: falta registrar un servicio posterior, o
  hay un error de fecha.

### 2.5 Parto de una vaca que no figuraba preñada

El ternero está, así que el parto se registra igual, pero el historial reproductivo
quedó incompleto: falta el servicio o el tacto.

### 2.6 Genealogía con un progenitor dado de baja

En el alta y en la modificación de animal:

- **madre** dada de baja antes de la fecha de nacimiento de la cría;
- **padre** dado de baja antes de la concepción —nueve meses antes del nacimiento—,
  con la aclaración de que es correcto si la cría vino de una pajuela suya;
- **padre y madre emparentados entre sí**: la cría nace consanguínea.

**Corregir en el Proyecto.** Incorporar el concepto de advertencia a 2.2.3 y a los
cursos alternativos de CU2, CU3, CU15 y CU18, y documentar los métodos
`AdvertenciasServicio`, `AdvertenciasParto` y `AdvertenciasGenealogia` en el
Diccionario de Clases.

---

## 3. La baja deja de ser irreversible

La baja lógica de CU4 no tenía vuelta atrás. Una caravana equivocada sacaba del rodeo
al animal equivocado y no había forma de deshacerlo desde el sistema: el animal
desaparecía del lote de ordeñe, de las alertas de secado y de parto, y del calendario
sanitario, con todo su historial.

`Controladora.ReactivarAnimal(pNumCaravana)` lo devuelve al rodeo y limpia la fecha y
el motivo de baja. Se llega desde el detalle del animal, donde ahora un cuadro explica
que está dado de baja, desde cuándo y por qué motivo, y ofrece el botón para
reactivarlo.

**Corregir en el Proyecto.** Escribir el curso alternativo de CU4 —o un caso de uso
propio— y sumar `ReactivarAnimal` al Diccionario de Clases y a `pControladora`.

---

## 4. Paginado de los listados

Con un rodeo de cientos de animales, las tablas se volvían inmanejables: el listado de
animales, el historial de movimientos de stock y el selector de animal de los
formularios mostraban todo de una vez.

Se agregó un paginador reutilizable (`wwwroot/js/paginador.js`). Cualquier tabla que
declare `data-paginar="N"` se pagina sola: el script arma los controles debajo, muestra
de a N filas e informa cuántos registros hay en total. No hay que tocar la pantalla que
la contiene. Está aplicado a los listados de los cinco módulos y al modal selector de
animal, que además combina el paginado con su propio filtro por caravana, categoría y
estado.

**El paginado es del lado del navegador y no de la base.** Con el volumen de un tambo
la consulta ya trae todo a memoria de una sola vez —así está diseñada la caché de la
Controladora—, así que partir la consulta no ahorraría trabajo: el problema es la
tabla de trescientas filas en pantalla, y eso es lo que resuelve. Si en algún momento
el volumen creciera lo suficiente como para que traer todo sea caro, el cambio a
paginar en la consulta es en la capa de persistencia y no en las pantallas.

**Corregir en el Proyecto.** Mencionarlo en los requerimientos no funcionales de
usabilidad.

---

## 5. Lo que sigue sin cubrirse

Anotado para que no se confunda con lo que sí quedó resuelto:

- Los eventos **sanitarios** —diagnóstico, tratamiento, vacunación, descorne— no
  controlan la fecha de baja del animal como lo hacen el celo y el servicio. Tratar a
  un animal que ya salió del rodeo no rompe nada, pero es igual de inconsistente.
- La **fecha de baja** se sigue forzando a la fecha del día: el formulario de CU4 no la
  pide (deuda B13 del Módulo 1). Con la baja ya reversible, el impacto es menor.
- El **peso de la cría** que menciona RF3.8 sigue sin registrarse (D7 de los Módulos 2
  y 3).
- Un parto puede registrar como máximo dos crías. Los trillizos existen, pero son
  rarísimos en Holando y el formulario no los contempla.
