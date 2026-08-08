# Desvíos y decisiones de los Módulos 4 y 5

Puntos en los que la implementación de los Módulos 4 (Gestión Sanitaria) y 5
(Control de Insumos y Stock) se aparta del documento de Proyecto, o resuelve algo
que el documento no define. Cada uno tiene que reflejarse en el Proyecto antes de
la entrega.

Los dos módulos ya tenían tablas adelantadas por los Módulos 2 y 3 —`insumos`,
`movimientos_stock`, `diagnosticos` y `tratamientos`—, así que este incremento
completa lo que faltaba en lugar de arrancar de cero: `bd/tambo_m4_m5.sql` crea
`planes_sanitarios`, `plan_categorias`, `vacunaciones` y `descornes`, y cierra las
restricciones que habían quedado pendientes sobre `tratamientos`.

---

## D1. `tratamientos.id_animal`: el tratamiento apunta al animal

**Documento.** 2.2.5.2 y el MER vinculan el tratamiento con el animal únicamente a
través de `diagnosticos`, y declaran que `tratamientos.id_diagnostico` admite nulo
porque *"ese nulo es lo que identifica al tratamiento preventivo, como la
desparasitación, que no se origina en un diagnóstico"*.

**Problema.** Las dos cosas juntas dejan al tratamiento preventivo sin animal. Es
la limitación D5 del documento de los Módulos 2 y 3, anotada entonces para
resolverla acá, y con el Módulo 4 completo se vuelve bloqueante en tres puntos:

1. Una desparasitación no generaba descarte de leche para nadie, así que el paso 3
   de CU8 no excluía del lote de ordeñe a un animal tratado.
2. El calendario sanitario (CU23) busca la última aplicación de un plan de
   desparasitación sobre un animal: sin el vínculo, nunca la encontraba y el
   procedimiento quedaba pendiente para siempre.
3. El propio CU20, curso alternativo 2a, dice que el preventivo *"se registra
   directamente sobre el animal"*.

**Resuelto.** `tratamientos` lleva `id_animal` con clave foránea a `animales`. La
Controladora lo completa siempre: cuando el tratamiento viene de un diagnóstico, con
el animal de ese diagnóstico; cuando es preventivo, con el que eligió el usuario.
`FechaFinDescarte` pasa a recorrer los tratamientos del animal y no los de sus
diagnósticos.

La columna queda `Null` en el DDL sólo por las filas anteriores a este script: las
que venían de un diagnóstico se completan con el `UPDATE` del propio script, pero un
preventivo cargado antes no tiene de dónde sacar el animal.

**Corregir en el Proyecto.** Agregar `id_animal` a `tratamientos` en el MER
(2.2.5.1), en la normalización (2.2.5.2), en la tabla de claves (2.2.5.3) y en las
restricciones de integridad (2.2.5.4). Reescribir el párrafo de 2.2.5.2 sobre el
tratamiento preventivo: el nulo que lo identifica es el de `id_diagnostico`, no la
ausencia de animal.

---

## D2. Dos clases de dominio que no tienen tabla

`ProcedimientoPendiente` (una fila del calendario sanitario) y `PartidaVencimiento`
(una partida con su remanente) no figuran en el Diccionario de Clases ni en el MER,
y no deben figurar en el modelo relacional: son datos derivados que 2.2.5.2 declara
explícitamente como no almacenables —*"los procedimientos sanitarios pendientes del
calendario o la comparación entre el stock actual y el stock mínimo no se almacenan
y se derivan en el momento de la consulta"*—.

Existen para que la Controladora devuelva el cálculo armado y la pantalla no lo
rehaga. Es el mismo criterio con el que la lista de animales del ordeñe por lote
vive dentro de `OrdenieLote`.

**Corregir en el Proyecto.** Sumarlas al Diccionario de Clases marcadas como clases
de cálculo, sin tabla asociada.

---

## D3. El remanente de cada partida se imputa a la que vence primero

**Documento.** CU28, paso 4, pide mostrar *"el insumo, la partida, la cantidad
remanente y los días restantes"*.

**Problema.** El movimiento de egreso no dice de qué partida salió: `movimientos_stock`
no tiene una clave hacia el movimiento de ingreso que consumió. Con ese modelo, la
cantidad remanente de una partida no es un dato que esté guardado en ningún lado.

**Resuelto.** El total consumido de un insumo se imputa contra sus partidas ordenadas
por fecha de vencimiento ascendente, que es el orden en el que se usan los productos
en el tambo: primero se gasta lo que primero se vence. Lo que sobra de cada una es su
remanente. Las partidas agotadas y las que no declaran vencimiento no generan alerta.

**Corregir en el Proyecto.** Enunciar el criterio en las reglas de negocio de CU28.
La alternativa —agregar `id_movimiento_origen` a los egresos— cambia el MER y no la
pide ningún caso de uso.

---

## D4. El alta de insumo registra la partida inicial como movimiento

**Documento.** CU25 junta en un solo trámite el alta del insumo y el ingreso de
stock, y su paso 6 dice que el sistema *"registra el movimiento de ingreso, actualiza
el inventario disponible y confirma la operación"*.

**Problema.** Hasta el Módulo 3 la pantalla de alta escribía `insumos.stock_actual`
directamente, sin movimiento. Esa existencia inicial quedaba sin partida, o sea sin
fecha de vencimiento para CU28 y sin fila en el historial de CU29.

**Resuelto.** `RegistrarIngreso` da de alta el insumo con stock cero y carga la
cantidad inicial como un movimiento de ingreso con motivo "Stock inicial" y su fecha
de vencimiento. La pantalla de alta pide la cantidad de la partida y su vencimiento.

---

## D5. El stock resultante de CU29 se deriva hacia atrás

**Documento.** CU29, paso 5: el listado detalla *"fecha, tipo de movimiento,
cantidad, motivo y stock resultante"*.

**Resuelto.** El saldo tampoco se almacena. Se calcula partiendo del stock actual del
insumo —que es el valor bueno, el que mantienen las transacciones— y restándole los
movimientos posteriores al que se está mostrando. Reconstruirlo sumando desde cero
daría un número equivocado para los insumos cargados antes de D4, que no tienen
movimiento de stock inicial.

Dos movimientos de la misma fecha se ordenan por identificador, que es el orden en
que se registraron.

---

## D6. Constantes de negocio que el documento no fija

| Constante | Valor | Para qué |
|---|---|---|
| `DIAS_ANTICIPACION_SANITARIA` | 30 | Horizonte del calendario sanitario (CU23) |
| `DIAS_ANTICIPACION_VENCIMIENTO` | 30 | Ventana de la alerta de vencimiento (CU28) |
| `UNIDADES_POR_VACUNACION` | 1 | Dosis que consume una aplicación (CU21, paso 6) |

CU23 habla de "horizonte de anticipación" y CU28 de "ventana de anticipación" sin
decir de cuántos días. El mes es el margen con el que se planifica en el
establecimiento: alcanza para comprar el insumo y juntar los animales antes de que el
procedimiento se atrase, y para dar de baja una partida antes de que venza en el
estante. Las dos pantallas permiten cambiarlo.

CU21 no dice cuánto descuenta una vacunación: se descuenta una dosis.

**Corregir en el Proyecto.** Enunciarlas en las reglas de negocio de CU21, CU23 y
CU28, y sumarlas a la tabla de constantes de `bd/LEEME.md`.

---

## D7. Una sola pantalla para crear y modificar el plan

El paso 3 de CU22 dice que el usuario *"selecciona 'Nuevo Plan' o elige un plan
existente para modificarlo"* y el paso 4 describe un único formulario para los dos
caminos. Se implementó así: `Planes Sanitarios` lista lo configurado y
`Configurar Plan` recibe el identificador cuando se está editando y no lo recibe
cuando el plan es nuevo.

Es una diferencia con el Módulo 1, donde el alta y la modificación de animal son dos
pantallas distintas. Acá el formulario es el mismo y duplicarlo sólo agregaba código
repetido.

---

## D8. La aplicación sólo puede cumplir un plan de su propio tipo

**Documento.** `tipo_procedimiento` *"determina en qué tabla busca el sistema la
última aplicación"*: vacunación en `vacunaciones`, desparasitación en `tratamientos`
y descorne en `descornes`.

**Problema.** Nada impedía que una vacunación declarara cumplir un plan de
desparasitación. Ese plan seguiría buscando su última aplicación en `tratamientos`,
no la encontraría, y el pendiente quedaría vivo aunque el usuario creyera haberlo
cumplido.

**Resuelto.** La Controladora rechaza la combinación y cada pantalla ofrece sólo los
planes activos de su tipo. El descorne, además, no consume insumo, así que su
formulario de plan oculta el selector de producto (CU22, curso alternativo 4c).

**Corregir en el Proyecto.** Enunciarlo en las reglas de negocio de CU20, CU21 y CU24.

---

## D9. Cerrar el diagnóstico

**Documento.** 2.2.5.4 declara `diagnosticos.estado` con tres valores —activo, en
tratamiento y resuelto—. CU19 lo deja "Pendiente de Tratamiento" y CU20 lo pasa a "En
Tratamiento". Ningún caso de uso lo lleva a "Resuelto".

**Resuelto.** La ficha sanitaria tiene la acción "Marcar resuelto" sobre los
diagnósticos abiertos. Sin ella el tercer estado no se alcanza nunca y una afección ya
curada se sigue ofreciendo para tratar en CU20.

**Corregir en el Proyecto.** Escribir el paso —como curso alternativo de CU19 o como
caso de uso propio— y sumar `ResolverDiagnostico` al Diccionario de Clases.

---

## D10. El ingreso de stock pasa a ser una transacción

`IngresarStock` escribía el movimiento y después actualizaba `insumos.stock_actual`
con dos comandos sueltos: si el segundo fallaba, el movimiento quedaba asentado y el
inventario no lo reflejaba. Ahora las dos escrituras van en una misma transacción
dentro de `pMovimientoStock.RegistrarIngreso`, y la suma se hace con la cuenta hecha
en la base (`stock_actual = stock_actual + @cantidad`) para que dos ingresos
simultáneos no se pisen el saldo. Es el mismo criterio que ya usaban los egresos
automáticos de la inseminación y del tratamiento.

Con eso `pInsumo.ActualizarStock` y `pMovimientoStock.AltaMovimiento` quedaron sin
uso y se eliminaron, en lugar de sumarse a la lista de operaciones de persistencia
inalcanzables de A4.

---

## D11. El descorne es de aplicación única

CU24 dice que *"el descorne es un procedimiento de umbral de edad y de aplicación
única"*. La Controladora rechaza el segundo descorne de un mismo animal y la pantalla
explica por qué. El documento no define qué hacer ante ese intento.

---

## D11.b La cantidad a descontar del tratamiento se ingresa, no se calcula

**Documento.** CU20, paso 5: el sistema *"descuenta automáticamente del stock la
cantidad total de medicamento calculada"*, y el curso de excepción 3a habla del
*"total requerido para los días digitados"*.

**Problema.** El total sería dosis diaria × días de duración, pero `dosis_diaria` es
un `Varchar(60)` —el usuario escribe "10 ml" o "2 comprimidos"— y de ahí no sale un
número para multiplicar. El documento pide un cálculo que su propio modelo de datos no
habilita.

**Resuelto.** La pantalla pide las unidades a descontar como un campo aparte y el
sistema valida que haya stock suficiente antes de guardar (curso de excepción 3a). El
descuento sigue siendo automático en el sentido de que lo hace el sistema, dentro de la
transacción del tratamiento; lo que no se deriva es la cantidad.

**Corregir en el Proyecto.** O se separa la dosis en cantidad numérica y unidad de
medida en `tratamientos`, y entonces el total se calcula, o se reescribe el paso 5 de
CU20 para que la cantidad sea un dato de entrada. Lo segundo es lo que está
implementado.

---

## D12. Métodos que se agregaron al Diccionario de Clases

Además de los que el Diccionario ya lista para estos módulos:

- `ValidarPlanSanitario(pPlan)` — devuelve el motivo por el que el plan no se puede
  guardar, para que la pantalla informe cuál de las validaciones de CU22 falló. Es el
  mismo patrón de `ValidarServicio`.
- `ExistePlanSanitario(pNombre, pIdPlan)` — el Diccionario lo declara con el nombre
  solo. Al modificar hay que excluir el propio plan del control de duplicados, si no
  ningún plan se puede guardar dos veces.
- `ListarPlanesXTipo(pTipoProcedimiento)` — los planes que puede cumplir una
  aplicación de ese tipo (ver D8).
- `FiltrarCalendario(pCalendario, pTipoProcedimiento, pIdCategoria)` — el curso
  alternativo 7a de CU23.
- `EstaVencido(pPendiente)` y `DiasParaAplicar(pPendiente)` — el estado de cada fila
  del cronograma.
- `ListarPartidas(pInsumo)`, `ListarAlertasVencimiento(pAnticipacionDias)`,
  `EstaVencida(pPartida)` y `DiasParaVencer(pPartida)` — CU28 (ver D3).
- `FiltrarMovimientos(pIdInsumo, pTipoMovimiento, pDesde, pHasta)` y
  `StockResultante(pMovimiento)` — CU29 (ver D5).
- `RegistrarIngreso(pInsumo, pCantidad, pFecha, pFechaVencimiento)` — el Diccionario
  lo declara recibiendo un `MovimientoStock` ya armado; la pantalla no tiene por qué
  construirlo, así que recibe los datos de la partida (ver D4).
- `ListarInsumosXTipo(pTipoInsumo)` — la vacunación aplica una vacuna, no cualquier
  producto sanitario.
- `FiltrarVacunacionesXAnimal`, `FiltrarDescornesXAnimal`,
  `FiltrarTratamientosXAnimal` y `TieneDescorne(pAnimal)` — los filtros que reemplazan
  a las colecciones no materializadas (D8 del documento anterior) y que usa el
  calendario.
- `ResolverDiagnostico(pIdDiagnostico)` — ver D9.
- `pMovimientoStock.RegistrarIngreso`, `pPlanSanitario.AltaPlan` /
  `ModificarPlan` / `ListarPlanes`, `pVacunacion.AltaVacunacion` / `ListarVacunaciones`,
  `pDescorne.AltaDescorne` / `ListarDescornes` e `pInsumo.ModificarStockMinimo` en la
  capa de persistencia.

---

## D13. Métodos del Diccionario que no se implementaron

- `DescontarStock(pIdInsumo, pCantidad)` y `RegistrarEgresoStock(pMovimiento)` — el
  egreso automático no puede ser una escritura suelta: tiene que ocurrir dentro de la
  misma transacción que el tratamiento, la vacunación o el servicio que lo origina, y
  ahí es donde está resuelto. La Controladora conserva `VerificarStock`, que es lo que
  se consulta antes de guardar.
- `ModificarInsumo(...)` — ningún caso de uso modifica los datos del insumo. Lo único
  que se edita es el umbral, y eso es `ModificarStockMinimo` (CU26).
- `BuscarPajuelasXToro(pToro)` — lo consumiría el curso alternativo 3a de CU6, que
  quedó como deuda del Módulo 1.
- `ListarAlertasFinDescarte()` — alimenta el resumen diario de CU35, que es del
  Módulo 6.
- `ProximoInsumoId`, `ProximoMovimientoStockId`, `ProximoPlanId`,
  `ProximoVacunacionId`, `ProximoDescorneId` y `BuscarMovimientoStock` — los
  identificadores los asigna MySQL desde la corrección B6, así que los `Proximo*Id` ya
  no tienen sentido en ningún módulo.
- `EnDescarteDeLeche(pAnimal)` y `ListarHembrasOrdeniables()` — ya existían con los
  nombres `TieneDescarteVigente` y `ListarAnimalesParaOrdenie` desde el Módulo 2.

**Corregir en el Proyecto.** Quitar del Diccionario los que no van a existir y
renombrar los dos últimos.

---

## D14. Lo que queda para el Módulo 6

Las tablas `alertas` y `preferencias_notificacion` del modelo no se crean acá: son el
registro de los envíos del bot de Telegram (CU34 y CU35). Las pantallas de CU27 y CU28
muestran las alertas en la interfaz sin escribir nada, que es lo que sus
post-condiciones piden.

El cálculo del calendario sanitario está en `ObtenerCalendarioSanitario`, que es el
mismo que va a alimentar el resumen diario, para que las dos vistas no puedan
discrepar (CU23, reglas de negocio).
