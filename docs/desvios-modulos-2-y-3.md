# Desvíos y decisiones de los Módulos 2 y 3

Puntos en los que la implementación de los Módulos 2 (Control de Producción) y 3
(Gestión Reproductiva) se aparta del documento de Proyecto, o resuelve algo que el
documento no define. Cada uno tiene que reflejarse en el Proyecto antes de la
entrega.

---

## D1. `ordenies_individual.id_ordenie_lote` pasa a admitir nulo

**Documento.** 2.2.5.4 lo declara `Not null, FK a ordenies_lote`, y el Diccionario
de Clases describe `OrdenieLote.mDetalles` como "ordeñes individuales que integran
el lote".

**Problema.** La regla de negocio de CU10 dice que en la modalidad "Totales
(Individual + Lote)" el sistema *"consolida y suma de forma matemática los litros
de ambas fuentes de registro para retornar el volumen neto exacto"*, y CU11 la
repite. Si el control individual estuviera incluido dentro del total del lote,
sumar las dos fuentes contaría esos litros dos veces. Las dos afirmaciones no
pueden ser ciertas a la vez.

**Resuelto.** `id_ordenie_lote` admite nulo. El control individual se registra por
sí solo, sin exigir que el ordeñe del lote de esa fecha y turno ya esté cargado; si
lo está, queda enganchado a él para poder reconstruir el turno.

**Sobre `litros_totales`, el criterio cambió.** La primera versión guardaba en el
lote los litros del ordeñe masivo *sin* la leche de las vacas controladas aparte: la
pantalla descontaba los controles individuales ya cargados, de manera que sumar las
dos fuentes diera el neto exacto y CU10 y CU11 cerraran tal como están escritos.

Eso resultó peor que el problema que resolvía, por tres motivos:

1. **Dependía del orden de carga.** La resta ocurría al guardar el lote. Un control
   individual cargado después ya no se descontaba de nada y se sumaba al total. En un
   día de control lechero —donde se mide casi todo el rodeo— el volumen del turno
   podía casi duplicarse, y nada recalculaba el lote.
2. **Lo almacenado no era lo que nadie midió.** La usuaria leía 1.500 del tanque y en
   la base quedaban 1.180: un número que no corresponde a ninguna lectura real.
3. **La leche descartada contaba como producción.** Medir lo que dio una vaca en
   tratamiento sumaba esos litros al turno, cuando esa leche se tira.

**Criterio actual.** El ordeñe individual no es otra fuente de leche: es el mismo
ordeñe del turno, anotado vaca por vaca en lugar de con un solo número. De ahí que
`litros_totales` sea la leche completa que salió en ese ordeñe, tal como se lee del
tanque, incluidas las vacas que además se midieron una por una.

La producción se resuelve **turno por turno**:

- si el turno tiene su ordeñe por lote, la producción es ese total, que ya incluye a
  las vacas controladas;
- si el turno se anotó únicamente con controles individuales, la producción es la
  suma de esos controles, porque el ordeñe ocurrió igual y esa leche salió.

Lo que nunca se hace es sumar las dos cosas dentro de un mismo turno: eso contaría
dos veces la leche de las vacas controladas. Ese —y sólo ése— es el sentido en que
las dos fuentes "no se suman".

Con eso desaparecen los tres problemas: no hay doble conteo posible, el orden de
carga es indistinto y se guarda lo que se midió. La pantalla del ordeñe muestra los
litros ya controlados del turno como referencia y rechaza que lo medido supere lo
que entregó el tanque, que es la única relación que tiene que cumplirse.

El historial señala los turnos anotados sólo vaca por vaca. No son un faltante y su
producción está contada: se marcan porque el dato es más frágil que la lectura del
tanque —si una vaca no se midió, esos litros no quedan registrados en ningún lado—.

**Corregir en el Proyecto.** 2.2.5.4, fila `ordenies_individual.id_ordenie_lote`:
pasa a `Null`. Aclarar en 2.2.5.2 que `litros_totales` es el ordeñe completo del
turno. Y reescribir la regla de negocio de **CU10 y CU11**: la consolidación no es
una suma de dos fuentes, sino la resolución turno por turno descrita arriba. La
modalidad "Totales (Individual + Lote)" desaparece; quedan la producción del
establecimiento y la vista de control individual, que es la porción de esa leche
medida animal por animal.

---

## D2. Tabla nueva `ordenie_lote_animales`

**Documento.** El Modelo Entidad-Relación no vincula `ordenies_lote` con
`animales`: el lote sólo guarda fecha, turno y litros totales.

**Problema.** El paso 4 de CU8 dice que el usuario *"modifica la lista si es
necesario, removiendo los animales que no se ordeñaron o agregando los que
faltan"*, y el curso alternativo 3a permite incluir a mano un animal excluido por
descarte. Sin una tabla que registre esa lista, el paso 4 no deja rastro y después
no hay forma de saber qué vacas se ordeñaron en cada turno.

**Resuelto.** Tabla intermedia `ordenie_lote_animales (id_ordenie_lote,
id_animal)`, con clave primaria compuesta. La cabecera y el detalle se guardan en
una misma transacción.

**Corregir en el Proyecto.** Agregarla al MER (2.2.5.1), a la normalización
(2.2.5.2), a la tabla de claves (2.2.5.3) y a las restricciones de integridad
(2.2.5.4). En el Diccionario de Clases, `OrdenieLote.mAnimales : List<Hembra>`
reemplaza a `mDetalles`.

---

## D3. Apertura manual de lactancia (caso de uso que falta)

**Documento.** El único camino previsto para abrir una lactancia es el parto
(CU18, paso 6).

**Problema.** Las vacas que ya estaban en ordeñe cuando arranca el sistema no
tienen un parto registrado. Sin lactancia abierta, CU9 no puede imputar el control
individual ("lo imputa a la lactancia vigente del animal") y CU12 no tiene qué
cerrar. La carga inicial del rodeo quedaría trabada.

**Resuelto.** Pantalla `Producción → Lactancias → Abrir Lactancia`. Pide caravana,
número de lactancia (que el sistema propone a partir de los partos registrados y de
las lactancias anteriores de la hembra) y fecha de inicio. Al abrirla, el animal
pasa a estado productivo "En lactancia".

**Corregir en el Proyecto.** Escribir el caso de uso, sumarlo al diagrama de casos
de uso del Módulo 2 y al Diccionario de Clases junto con `AltaLactancia` y
`ProximoNumeroLactancia`.

---

## D4. Se adelantan cuatro tablas de los Módulos 4 y 5

Los casos de uso de estos dos módulos no se pueden implementar completos sin ellas:

| Tabla | Módulo | Por qué hace falta ahora |
|---|---|---|
| `insumos` | 5 | CU15 registra la inseminación artificial con una pajuela del stock (`servicios.id_insumo`) y toma el toro de catálogo vinculado (`insumos.id_macho`) como reproductor genético (RF3.3). |
| `movimientos_stock` | 5 | CU15 incluye "Descontar Automáticamente Semen de Stock". El descuento deja su movimiento para quedar trazado. |
| `diagnosticos` | 4 | Es el único vínculo entre un tratamiento y un animal. |
| `tratamientos` | 4 | El paso 3 de CU8 excluye del lote a los animales con descarte de leche vigente, dato que sale de `tratamientos.fecha_fin_descarte`. |

Se implementó **sólo lo que estos dos módulos consumen**: alta de insumo, ingreso
de stock, alta de diagnóstico y alta de tratamiento con cálculo del descarte. Los
umbrales de stock mínimo, los vencimientos, las alertas, los planes sanitarios, las
vacunaciones, los descornes y el calendario sanitario quedan para sus módulos.

`tratamientos.id_plan` está declarado como columna pero todavía sin clave foránea:
la tabla `planes_sanitarios` se crea con el Módulo 4 y la restricción se agrega ahí
con un `ALTER TABLE`.

---

## D4.b Reglas de negocio que el documento no cubre

Salieron de revisar la lógica contra la operación real de un tambo, no del Proyecto.
Todas están implementadas y hay que incorporarlas a los casos de uso.

**El parto cierra la lactancia anterior.** CU18 abre la lactancia nueva pero no dice
nada de la vieja. Olvidarse de registrar el secado es de los errores más comunes; sin
este cierre la vaca queda con dos lactancias en curso y los ordeñes siguientes se
imputan a la equivocada, en silencio. Ahora el parto la cierra con su propia fecha, y
`LactanciaActual` devuelve la de inicio más reciente por si igual quedaran dos.

**Un parto puede tener más de una cría.** Alrededor del 4 % de los partos Holando son
dobles. `AltaParto` recibe una lista de crías; la madre suma **un** parto y **una**
lactancia aunque nazcan dos terneros. La pantalla tiene una casilla "Parto doble".

**No se puede servir una hembra preñada.** Provoca el aborto. `ValidarServicio` lo
rechaza e indica el camino correcto: registrar primero un tacto con resultado vacía si
la preñez estaba mal confirmada.

**Un tacto vacío borra la fecha probable de parto de la lactancia.** Si no, tras un
aborto la vaca sigue apareciendo en las alertas de secado y se termina secando por una
preñez que ya no existe.

**"Preñez confirmada" es preñez en curso.** `ListarServiciosConPrenez` sólo devuelve el
servicio vigente de cada hembra. Antes devolvía todos los servicios históricos con
tacto positivo, y como el filtro de CU17 es "FPP ≤ hoy + 15 días" —que cualquier fecha
pasada cumple—, cada preñez anterior reaparecía como alerta de parto atrasada apenas la
vaca volvía a preñarse.

**Ajustar la fecha probable de parto la baja a la lactancia.** La alerta de secado se
calcula sobre `lactancias.fecha_probable_parto`; corregir sólo el servicio no cambiaba
nada. Las dos escrituras van en una transacción.

**Un animal tiene un solo control individual por fecha y turno.** Restricción `UNIQUE`
en la tabla y control en la Controladora: la carga a mano en un día de control lechero
es propensa a la doble carga, que inflaba la producción de la lactancia.

**El ordeñe individual se imputa a la lactancia de su fecha, no a la actual.**
`LactanciaDeLaFecha` reemplaza a `LactanciaActual` en el alta: cargar el control de ayer
para una vaca que parió hoy tiene que ir a la lactancia vieja.

**El ordeñe por lote se puede corregir.** La clave alterna de fecha y turno impedía
recargarlo, así que un error de tipeo quedaba fijo. `ModificarOrdenieLote` reescribe los
litros y el detalle de animales.

**Los litros del lote se validan contra la cantidad de animales.** El tope fijo de
100.000 no atrapaba nada: un rodeo de 200 vacas da unos 2.500 litros por turno.
`ValidarLitrosLote` usa `animales × 100 litros`.

**El total del tanque se descuenta de los controles individuales.** La pantalla de CU8
pregunta si los litros vienen del tanque —que incluye toda la leche— y en ese caso resta
los controles individuales ya cargados de esa fecha y turno antes de guardar. Sin eso, la
suma de las dos fuentes de CU10 y CU11 contaba esa leche dos veces.

**La edad mínima al servicio es distinta para la hembra y para el macho.** Ver D4.c.

---

## D4.c La vaquillona entra en servicio a los 13 meses, no a los 15

`EDAD_MINIMA_SERVICIO_MESES` (15) se usaba para los dos progenitores, así que
`ValidarGenealogia` exigía 24 meses entre madre y cría. La vaquillona Holando bien criada
se sirve entre los 13 y los 15 meses y pare a los 22: el sistema rechazaba partos
perfectamente normales.

Se agregó `EDAD_MINIMA_SERVICIO_HEMBRA_MESES = 13`, con lo que la diferencia mínima pasa
a 22 meses para la madre y se mantiene en 24 para el padre.

**Corregir en el Proyecto.** Enunciar las dos edades por separado en las reglas de
negocio de CU2 y CU18.

---

## D5. El tratamiento preventivo no se puede atribuir a un animal

**Documento.** 2.2.5.2: en `tratamientos`, *"`id_diagnostico` admite valor nulo, y
ese nulo es lo que identifica al tratamiento preventivo, como la desparasitación,
que no se origina en un diagnóstico"*.

**Problema.** `tratamientos` no tiene `id_animal`: el único vínculo con el animal
pasa por `diagnosticos`. Un tratamiento preventivo, entonces, no está asociado a
ningún animal, y por lo tanto no puede generar descarte de leche para nadie.

**Estado.** Sin resolver, anotado como limitación. Hoy sólo los tratamientos que
nacen de un diagnóstico excluyen animales del lote de ordeñe. Al implementar el
Módulo 4 hay que decidir si `tratamientos` lleva `id_animal` propio, o si el
tratamiento preventivo se modela de otra forma.

---

## D6. El servicio "en estado pendiente" es un estado derivado

**Documento.** Pre-condición de CU16: *"el animal debe contar con un servicio previo
registrado en estado pendiente"*.

**Problema.** La tabla `servicios` no tiene ninguna columna de estado.

**Resuelto.** `Controladora.ServicioVigente(pHembra)` lo deduce de las fechas: el
servicio más reciente de la hembra posterior a su último parto. Es el servicio sobre
el que trabajan el tacto (CU16) y el parto (CU18).

**Corregir en el Proyecto.** Reescribir la pre-condición de CU16 en esos términos, o
agregar la columna de estado al modelo.

---

## D7. La cría no se referencia desde el parto

`partos` no tiene `id_cria`, siguiendo el MER. La cría queda vinculada por
`animales.id_madre` y se corresponde con su parto por la madre y la fecha de
nacimiento. Con mellizos —que ahora se pueden registrar, ver D4.b— las dos crías
comparten madre y fecha, así que a partir de esos dos datos no se puede distinguir cuál
es cuál. Para el linaje da igual (las dos tienen la misma madre y el mismo padre), pero
si algún reporte del Módulo 6 necesita "la cría de este parto", va a hacer falta una
tabla intermedia `parto_crias` o un `id_parto` en `animales`.

Tampoco se registra el **peso de la cría** que menciona RF3.8: el MER no tiene ese
campo y CU18 no lo pide en el formulario. O se agrega la columna, o se saca de
RF3.8.

---

## D8. Las colecciones del Diccionario de Clases no se materializan

El Diccionario declara `Hembra.mLactancias`, `mServicios`, `mCelos`, `mPartos`,
`mOrdenies`; `Lactancia.mOrdenies`; `Servicio.mTactos`; `Diagnostico.mTratamientos`.
Ninguna existe como campo, igual que ya pasaba con `Macho.mPajuelas` en el Módulo 1.

**Motivo.** Materializarlas obligaría a la capa de persistencia a resolver el grafo
completo en cada consulta, y las relaciones son circulares (`Hembra → Servicio →
Hembra`). En su lugar, la Controladora las resuelve con métodos de filtro sobre sus
listas caché: `FiltrarLactanciasXHembra`, `FiltrarServiciosXHembra`,
`FiltrarCelosXHembra`, `FiltrarPartosXHembra`, `FiltrarOrdeniesXLactancia`,
`FiltrarTactosXServicio`, `FiltrarDiagnosticosXAnimal`.

Es coherente con que las clases de dominio sean anémicas y con que toda la lógica
viva en la Controladora. **Corregir en el Proyecto:** quitar esas colecciones del
Diccionario de Clases y documentar los métodos de filtro que las reemplazan.

---

## D9. `Controladora.Refrescar()`: la caché se recarga completa

Con el Módulo 1 cada `Listar` refrescaba solo su lista. Ahora las entidades se
referencian entre sí —el servicio apunta a una hembra, el ordeñe individual a una
lactancia— y si cada `Listar` recargara por su cuenta, dos listas cargadas una
detrás de la otra terminarían apuntando a objetos distintos que representan la misma
fila. El caso concreto: el tacto actualiza la hembra que cuelga de su servicio, y
esa hembra tiene que ser la misma que está en `mListaHembras`.

Todos los `Listar*` invocan ahora un `Refrescar()` privado que recarga todas las
listas en orden de dependencia. Un campo de instancia `mRefrescado` hace que la
recarga ocurra una sola vez por Controladora: una pantalla que llama a cinco
`Listar*` distintos consulta la base una vez, no cinco. Como las altas y las
modificaciones actualizan la caché en memoria, un `Listar` posterior dentro de la
misma petición sigue viendo el dato recién guardado.

**Corregir en el Proyecto.** Documentarlo en 2.2.3, donde hoy dice que cada consulta
refresca su propia caché.

---

## D10. Métodos que se agregaron al Diccionario de Clases

Además de los que el Diccionario ya lista para estos módulos, la implementación
necesitó:

- `ValidarLitrosIndividual(pLitros)` — el tope de coherencia de un control
  individual (100 litros) no es el mismo que el del lote.
- `ValidarLitrosLote(pLitros, pCantidadAnimales)` — el tope del lote sale de la cantidad
  de animales ordeñados.
- `LactanciaDeLaFecha(pHembra, pFecha)` — la lactancia que estaba en curso en una fecha
  dada, para el ordeñe retroactivo.
- `BuscarOrdenieIndividualXFechaTurno(pFecha, pTurno, pIdAnimal)` — control de duplicados.
- `SumarLitrosIndividualesDelTurno(pFecha, pTurno)` — para descontar del total del tanque.
- `ModificarOrdenieLote(pIdOrdenieLote, pLitrosTotales, pAnimales)` — corrección del
  ordeñe ya cargado.
- `ValidarEliminarOrdenieLote(pIdOrdenieLote)` y `EliminarOrdenieLote(pIdOrdenieLote)` —
  la baja del ordeñe del turno, que la corrección no cubre: fecha y turno son clave
  alterna y no se reescriben.
- `ValidarModificarOrdenieIndividual(pIdOrdenieInd, pLitros)` y
  `ModificarOrdenieIndividual(pIdOrdenieInd, pLitros)` — corrección de los litros de un
  control ya cargado.
- `ValidarEliminarOrdenieIndividual(pIdOrdenieInd)` y
  `EliminarOrdenieIndividual(pIdOrdenieInd)` — la baja del control, que es como se
  arregla el control anotado en la vaca equivocada.
- `ValidarServicio(pServicio)` — devuelve el motivo por el que el servicio no se
  puede registrar, para que la pantalla informe cuál de las reglas de CU15 falló.
- `ServicioVigente(pHembra)` — ver D6.
- `PadreSugerido(pMadre)` — el toro del servicio, que CU18 propone como padre de la
  cría (RF3.3).
- `ProximoNumeroLactancia(pHembra)` — ver D3.
- `BuscarOrdenieLoteXFechaTurno(pFecha, pTurno)` — la clave alterna de
  `ordenies_lote`.
- `ListarAnimalesParaOrdenie()` y `ListarHembrasEnDescarte()` — los pasos 2 y 3 de
  CU8.
- `TieneDescarteVigente(pAnimal)` y `FechaFinDescarte(pAnimal)` — ver D5.
- `CalcularProduccionEnRango(pDesde, pHasta, pModalidad)` y
  `CalcularProduccionMensual(pMes, pAnio)` — CU10 y CU11.
- Los `Filtrar*X*` de D8.

Y `pConexion.EjecutarInsercion(pSql, pParametros)`, que devuelve el id que asignó
MySQL en una inserción que no está dentro de una transacción.

---

## D11. Constantes de negocio que el documento no fija

Ver la tabla en `bd/LEEME.md`. Son los valores habituales de un tambo Holando:
gestación 283 días, secado 60 días antes de la fecha probable de parto, ventana de
alerta de 15 días para el secado y para el parto. **Corregir en el Proyecto:**
enunciarlas en las reglas de negocio de CU13, CU15, CU16 y CU17, que hoy hablan de
"ventana crítica" y "rango crítico" sin números.
