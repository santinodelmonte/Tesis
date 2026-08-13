# Corrección de registros y navegación

Dos desvíos respecto del Proyecto, tomados juntos porque el segundo es consecuencia
del primero. Este documento es la fuente para el capítulo correspondiente del
informe: acá está el porqué de cada decisión y la lista de reglas que hay que
transcribir, y no hace falta volver a deducirlas del código.

---

## 1. Por qué el sistema no preveía corregir

Los casos de uso de los Módulos 3 y 4 registran **hechos**: la vaca entró en celo,
se la sirvió, el veterinario la tactó, parió, se le diagnosticó una mastitis, se la
trató. Un hecho pasó una sola vez y de una sola manera, así que el Proyecto no
define ningún caso de uso para modificarlo ni para darlo de baja. Modelado como
modelo de dominio, es correcto.

El problema es que el sistema no guarda hechos: guarda **lo que una persona tecleó
sobre un hecho**, en un tambo, con las manos ocupadas y a veces varias horas
después. Una caravana mal tipeada, un resultado de tacto invertido, un servicio
anotado sobre la vaca de al lado. Sin manera de corregir, ese error quedaba adentro
del sistema para siempre y —peor— seguía produciendo consecuencias: la vaca
figuraba preñada, entraba en las alertas de parto, salía de la lista para servir y
se la secaba con una fecha calculada sobre una preñez que nunca existió.

De ahí el desvío: se agrega modificación y eliminación a los ocho registros de
Reproducción y Sanidad —celo, servicio, tacto, parto, diagnóstico, tratamiento,
vacunación y descorne—, con dos reglas que evitan que corregir un dato rompa otros
tres.

## 2. Las dos reglas

### 2.1 El estado derivado no se deshace: se vuelve a deducir

El sistema **no guarda** "en qué estado estaba la vaca antes de este tacto" para
poder revertirlo. Después de cada corrección mira los registros que quedaron y
deduce de cero el estado que corresponde:

```
sin servicio posterior al último parto            → Vacía
con servicio y sin tacto                          → Servida
con servicio y último tacto dudoso                → Servida
con servicio y último tacto preñada               → Preñada
con servicio y último tacto vacía                 → Vacía
```

Eso es `EstadoReproductivoDeducido` en la Controladora, y lo mismo hace
`LactanciaRecalculada` con la fecha probable de parto proyectada sobre la lactancia
en curso, que es de donde sale la fecha recomendada de secado.

La alternativa —un deshacer paso a paso, guardando el valor anterior de cada
campo— obligaba a llevar un historial de cambios y, sobre todo, no tenía respuesta
para el caso normal: corregir un registro **viejo** teniendo otros más nuevos
encima. Si se corrige el tacto de marzo y en abril hubo otro, el deshacer del
primero pisaría lo que dice el segundo. Deducir de cero no tiene ese problema: el
estado siempre termina siendo el que dicen los registros que hay.

Los métodos de deducción reciben el identificador del registro que se está por
eliminar, para saltearlo aunque todavía figure en la caché. En las modificaciones
ese parámetro va en cero: los datos nuevos se vuelcan primero a la caché y la
deducción los ve ya corregidos. Si la escritura falla, la caché se vuelve atrás.

### 2.2 Lo que tiene algo colgando no se borra: se avisa qué sacar primero

No hay borrado en cascada silencioso. Cuando un registro tiene otros que dependen
de él, el listado muestra el botón de eliminar y, al apretarlo, explica qué hay que
sacar antes. El usuario deshace en el orden inverso al que cargó, que es el orden
en el que se acuerda de lo que hizo.

| Registro | Qué lo bloquea |
|---|---|
| Celo | nada: nunca se bloquea |
| Servicio | tiene tactos registrados |
| Tacto | nada: nunca se bloquea |
| Parto | no es el último de la vaca; la lactancia que abrió tiene ordeñes cargados; alguna cría ya tiene historia propia |
| Diagnóstico | tiene tratamientos aplicados |
| Tratamiento | nada: nunca se bloquea |
| Vacunación | nada: nunca se bloquea |
| Descorne | nada: nunca se bloquea |

La única excepción a "nada en cascada" es el parto, y es deliberada: deshacerlo
paso a paso significaría que el usuario diera de baja las crías a mano y cerrara la
lactancia a mano antes de poder tocar el parto, que es justamente la parte que no
sabe hacer. Entonces el borrado del parto se lleva todo junto —crías, lactancia
nueva—, reabre la lactancia anterior y le devuelve a la madre el parto, el estado y
la categoría; pero **solo mientras nada de eso se haya usado todavía**. Si la
lactancia ya tiene ordeñes o una cría ya tiene registros propios, se bloquea: eso
ya no es un error de carga reciente.

## 3. Qué arrastra cada corrección

| Registro | Lo que se deshace o se recalcula |
|---|---|
| Celo | nada |
| Servicio | estado reproductivo de la hembra; fecha probable de parto de la lactancia; pajuela devuelta al stock |
| Tacto | estado reproductivo de la hembra; fecha probable de parto de la lactancia |
| Parto | crías; lactancia abierta; lactancia anterior reabierta; número de partos, estados y categoría de la madre |
| Diagnóstico | nada |
| Tratamiento | producto devuelto al stock; descarte de leche recalculado; estado del diagnóstico de origen |
| Vacunación | dosis devuelta al stock |
| Descorne | nada; el animal vuelve a figurar pendiente en el plan de descorne |

Todo eso va dentro de una misma transacción que la del borrado o la modificación.
Un servicio eliminado a medias dejaría una vaca servida que no lo está, o una
pajuela descontada dos veces.

### 3.1 El borrado es físico

Se descartó marcar el registro como anulado y dejarlo. La anulación lógica deja
auditoría, pero obliga a filtrar `anulado = 0` en las cuarenta y pico de consultas
y listados que recorren estas listas, y basta olvidarse el filtro en una sola para
que un indicador empiece a mentir sin que nadie lo note. Con borrado físico el dato
no está, y ninguna consulta necesita enterarse de nada.

### 3.2 El stock se devuelve con un contra-movimiento

Cuando se elimina o se corrige un registro que había consumido producto, el egreso
original **no se borra**: se asienta un ingreso de ajuste que devuelve la cantidad.
El historial de movimientos tiene que poder explicar por qué el saldo del 12 de
marzo era el que era, y para eso el error y su corrección tienen que estar los dos.

El contra-movimiento va sin fecha de vencimiento a propósito: no es una partida
nueva que entra al depósito, es producto que en realidad nunca salió, y sin
vencimiento no aparece en las alertas de CU28.

## 4. Cambio en la base: `tratamientos.cantidad_insumo`

Es el único cambio de esquema que trajo todo esto.

La cantidad de producto que consumía un tratamiento vivía únicamente en
`movimientos_stock`, y esa tabla no dice de qué tratamiento salió cada egreso. Al
eliminar o corregir un tratamiento no había forma de saber cuánto devolverle al
inventario. Se agregó la columna a `tratamientos`, con lo que además el listado
puede mostrar cuánto se aplicó, que antes no se veía en ningún lado.

Los tratamientos cargados antes de la columna quedan en cero y por lo tanto no
devuelven stock cuando se los elimina. Es preferible a inventar una cantidad.

El cambio está en `bd/tambo.sql` y en `bd/tambo_actualizacion.sql`; los scripts por
módulo quedan como estaban, que es el registro de cómo se entregó cada módulo.

## 5. Reconstrucciones por fecha

`partos`, `lactancias` y `animales` no están vinculadas entre sí por clave foránea:
el MER no las relaciona. Para deshacer un parto hay que encontrar la lactancia que
abrió, la que cerró y las crías que dio, y eso se reconstruye por fecha:

- **la lactancia que abrió**: la de esa vaca que empieza el mismo día del parto;
- **la lactancia que cerró**: la de esa vaca que quedó secada el mismo día del
  parto y había empezado antes;
- **las crías**: los animales de esa madre nacidos el día del parto.

Las tres son inequívocas salvo un caso: si el secado se hubiera registrado a mano
justo el día del parto, el borrado reabriría esa lactancia igual. Es improbable —el
parto cierra la lactancia solo cuando el secado *no* se registró— y la pantalla
muestra qué se va a llevar antes de confirmar. **Queda anotado como limitación
conocida**; resolverlo bien pide agregarle a `lactancias` una foránea al parto que
la origina, que es un cambio del MER.

---

## 6. Navegación: el menú lateral y "listado primero"

El menú superior de desplegables tenía dos problemas, y los dos venían de lo mismo:
no había un único lugar natural para cada pantalla.

- **No se sabía dónde estaba uno parado.** Había que abrir un desplegable para ver
  el mapa, y ninguno indicaba la pantalla abierta.
- **La misma pantalla aparecía en varios lados.** "Registrar tratamiento" estaba en
  el menú, en la pantalla de diagnósticos, en el calendario sanitario y al pie de
  la pantalla de vacunación. Cuatro caminos a lo mismo no es comodidad: es que
  ninguno de los cuatro es el camino.

### 6.1 Lo que se hizo

**El menú es una columna lateral.** En pantalla ancha queda fija a la izquierda y
se desplaza sola; abajo de 992px se pliega y la abre el botón de la barra superior.
Cada módulo es una sección desplegable, la sección de la pantalla abierta viene
desplegada y su ítem marcado con una barra de color a la izquierda.

**Una entrada por entidad, y esa entrada es su listado.** Registrar, corregir y
eliminar se hacen desde adentro del listado. El menú de Reproducción pasó de once
entradas a cuatro —Celos, Servicios, Tactos, Partos— y el de Sanidad de siete a
cinco.

**Los pendientes y las alertas se agruparon.** Vacas para servir, tactos
pendientes, partos próximos, secados próximos, calendario sanitario, stock crítico
y partidas por vencer están juntos arriba de todo, y no reparti­dos por módulo. Es
la primera pregunta de la mañana y no tiene sentido tener que recordar en qué
módulo vive cada alerta.

**Se sacaron los accesos repetidos de las pantallas.** Lo que está en el menú no se
repite como botón; lo que queda como botón es lo que se hace sobre lo que se está
mirando: "Agregar animal" en el rodeo, "Registrar ingreso" en insumos, "Abrir
lactancia" en lactancias. Los enlaces de las listas de trabajo —el "Servir" de la
fila de una vaca para servir— se mantienen: no son accesos repetidos, son la acción
que esa fila propone.

### 6.2 Pantallas nuevas y pantallas que cambiaron

| Pantalla | Qué pasó |
|---|---|
| `ListaTactos` | nueva: los tactos solo se veían de a uno, como "último tacto" de cada servicio |
| `ListaPartos` | nueva: los partos no se listaban en ningún lado |
| `ModificarParto` | nueva: corrige fecha, tipo y observaciones |
| `ListaTratamientos` | nueva; se lleva el aviso de descarte de leche vigente |
| `ListaVacunaciones` | nueva; antes se veían al pie de la pantalla que las registraba |
| `ListaDescornes` | nueva; ídem |
| `ListaDiagnosticos` | queda solo con diagnósticos: mostraba las cuatro tablas de Sanidad |
| `Registrar*` (7 pantallas) | las mismas pantallas dan de alta y corrigen, según reciban o no un identificador |

La ficha sanitaria completa de un animal —diagnósticos, tratamientos, vacunaciones
y descornes juntos— sigue estando donde corresponde, que es el detalle del animal.

### 6.3 Por qué el parto tiene pantalla propia de corrección

Las otras siete correcciones reusan la pantalla de alta: los campos son los mismos
y las reglas también, y separarlas habría significado mantener dos veces el mismo
selector de animal y la misma validación. El alta del parto, en cambio, carga
además las crías con su caravana, su raza y su foto, y esas ya son animales del
rodeo que se corrigen desde Animales. Por eso `ModificarParto` es una pantalla
aparte, como `ModificarAnimal` y `ModificarOrdenieLote`.

---

## 7. Qué falta verificar

El código compila, pero **el comportamiento real no se ejecutó todavía ni una vez**:
las transacciones, los contra-movimientos de stock y el borrado en cascada del parto
están escritos y no probados.

Antes de probar hay que correr la actualización de la base, porque la columna
`tratamientos.cantidad_insumo` es nueva y sin ella la aplicación no arranca:

```bash
mysql -u root -p < bd/tambo_actualizacion.sql
```

El orden de prueba es el de `docs/flujos-de-prueba.md`. Lo primero que conviene
mirar es el borrado del parto, que es lo que más tablas toca, y en segundo lugar el
tratamiento, que es el único que mueve stock y estado de otro registro a la vez.

Casos que conviene probar a mano porque no salen de un flujo normal:

1. Corregir el resultado de un tacto de "Preñada" a "Vacía" y verificar que la vaca
   vuelve a la lista para servir, desaparece de las alertas de parto y su lactancia
   pierde la fecha probable de parto.
2. Eliminar un servicio con pajuela y verificar que el stock vuelve **y** que el
   historial de movimientos muestra el egreso original más el ajuste.
3. Eliminar un parto reciente y verificar que la lactancia anterior queda otra vez
   abierta y que la vaca vuelve a la categoría que tenía.
4. Intentar eliminar un servicio con tactos y un diagnóstico con tratamientos, para
   comprobar que el mensaje dice qué hay que sacar primero.
