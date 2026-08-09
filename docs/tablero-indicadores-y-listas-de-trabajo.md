# Tablero, indicadores y listas de trabajo

Incremento posterior a la configuración del establecimiento. No agrega casos de uso
del documento: agrega lo que el sistema necesita para que la encargada lo use todos
los días en lugar de consultarlo cuando se acuerda.

Seis cosas, en el orden en que se usan.

---

## 1. Tablero de inicio: "Hoy en el tambo"

La pantalla de inicio era un cartel de bienvenida. Para saber qué había que hacer
había que entrar a seis menús distintos.

Ahora muestra, en tres tarjetas, todo lo que requiere atención: vacas para servir,
tactos pendientes, partos próximos, secados próximos, animales con descarte de leche
vigente, procedimientos sanitarios pendientes, insumos en stock crítico y partidas por
vencer. Abajo, la composición del rodeo y los dos números de referencia: litros por
vaca y día, y días en leche promedio.

No calcula nada propio: junta lo que ya vivía en la Controladora. Es además el mismo
contenido del resumen diario de CU35, así que el Módulo 6 arranca con ese cálculo
resuelto.

Cuando no hay ningún animal cargado, en lugar de ocho ceros el tablero explica por
dónde empezar.

---

## 2. Ficha integral del animal

`DetalleAnimal` mostraba identificación y linaje. Ahora es la pantalla que se mira
parado frente a la vaca:

- **Productivo**: lactancia en curso, días en leche, litros por día, producción
  estimada, proyección a 305 días, los controles de la lactancia y todas las
  lactancias registradas.
- **Reproductivo**: partos, días abiertos, intervalo entre partos, servicios desde el
  último parto, último servicio con su tacto y la fecha probable de parto.
- **Sanitario**: diagnósticos, tratamientos, vacunaciones, descornes, si tiene la
  leche en descarte y hasta cuándo, y qué le exigen los planes sanitarios activos.
- **Descendencia**: las crías registradas, con acceso a su ficha.

La pantalla acepta el identificador o el número de caravana, que es lo que el
buscador de la barra superior manda.

---

## 3. Las dos listas de trabajo

**Tactos pendientes.** Servicios con más de `dias_para_tacto` días sin tacto
registrado, sobre hembras que figuran servidas. Es la lista con la que se arma la
visita del veterinario, y antes había que acordarse vaca por vaca.

**Vacas para servir.** Hembras vacías que ya pasaron el período de espera voluntario
después de parir, más las vaquillonas que alcanzaron la edad mínima y nunca fueron
servidas. Muestra los días abiertos y el último celo detectado, que es con lo que se
decide a cuál servir primero.

Las dos necesitaron dos parámetros nuevos en la configuración:

| Parámetro | Por defecto | Para qué |
|---|---|---|
| `dias_espera_voluntaria` | 45 | Días posparto antes de volver a servir |
| `dias_para_tacto` | 35 | Desde cuándo el tacto ya se puede hacer |

Si la base ya tenía la tabla `configuracion` creada, hay que correr
`bd/tambo_configuracion_actualizacion.sql`. En una base nueva no hace falta.

---

## 4. Indicadores del rodeo

### El cambio importante: cómo se calcula la producción de una lactancia

`CalcularProduccionTotal` sumaba los controles individuales de la lactancia. **Con un
control lechero por mes, eso da la leche de unas diez jornadas sueltas, no la de la
lactancia**: una vaca que produjo 7.000 litros figuraba con 250. El número existía,
se mostraba en el listado de lactancias y no significaba nada.

`EstimarProduccionLactancia` lo reemplaza usando el **método de intervalos de
control**, que es con el que trabaja cualquier control lechero oficial: cada medición
representa a los días que la rodean, así que se multiplica el promedio de dos
controles consecutivos por los días que hay entre ellos, se agrega el tramo del parto
al primer control y el del último control al secado —o a hoy, si la vaca sigue en
ordeñe—.

Un detalle que importa: los controles se guardan por turno, y el cálculo trabaja sobre
la **jornada completa**. Una vaca controlada en los dos ordeñes del mismo día tiene un
solo dato de producción diaria, no dos.

La suma cruda no se borró: en el listado de lactancias figura como "litros medidos",
que es lo que efectivamente se midió, al lado de la producción estimada, que es lo que
la vaca dio.

### Los indicadores

- **Días abiertos**: del parto a la concepción. Mientras la vaca no queda preñada el
  número sigue corriendo, y eso también es información.
- **Intervalo entre partos**.
- **Servicios por preñez**: mide el gasto en pajuelas y el acierto en la detección de
  celo.
- **Litros por vaca y por día** y **días en leche promedio**.
- **Composición del rodeo** por estado productivo y reproductivo.
- **Ranking de las lactancias en curso** por producción diaria, con la proyección a
  305 días.

La proyección a 305 días supone que la vaca sostiene su último nivel controlado. Es
lineal y por lo tanto optimista —la curva de lactancia baja sobre el final—, y así
está aclarado en la pantalla: sirve para comparar vacas entre sí, no como pronóstico.

---

## 5. Carga masiva del control lechero y buscador de caravana

**Control lechero.** El control se hace una vez por mes y se miden todas las vacas en
ordeñe el mismo día. Cargarlas de a una era media hora de clics. La pantalla nueva
muestra la lista completa con una columna de litros, un filtro por caravana y guarda
de una sola vez las que tengan valor.

Cada control se guarda por separado a través de la misma regla de CU9, así que si una
fila falla —porque esa vaca ya tenía control ese día y turno, por ejemplo— las demás se
guardan igual y la pantalla informa cuáles quedaron afuera. Es lo contrario de una
transacción única, y es lo correcto acá: perder toda la carga por una fila repetida
sería peor.

**Buscador de caravana.** Un campo en la barra superior que lleva directo a la ficha
del animal desde cualquier pantalla.

---

## 6. Candidatas a descarte

Una vaca aparece en la lista si cumple al menos uno de estos criterios:

| Criterio | Umbral |
|---|---|
| Produce menos del 70 % del promedio del rodeo | `PORCENTAJE_PRODUCCION_BAJA` |
| Servicios desde el último parto sin preñez | 3 |
| Días abiertos sin quedar preñada | más de 150 |
| Diagnósticos sanitarios en el último año | 3 |
| Cantidad de partos | 7 o más |

La lista sale ordenada por cantidad de motivos y muestra **por qué** aparece cada una.
El sistema no decide: la decisión es de quien conoce al animal. Por eso son motivos en
texto y no un puntaje.

Los umbrales quedaron como constantes y no como parámetros configurables: son un
criterio de análisis, no una regla de manejo diaria. Si el establecimiento quisiera
ajustarlos, el lugar natural es la tabla `configuracion`.

---

## Qué hay que agregar al Proyecto

1. **Requerimientos funcionales nuevos**: tablero de inicio, listas de trabajo
   (tactos pendientes y vacas para servir), indicadores del rodeo, carga masiva del
   control lechero y apoyo al descarte.
2. **Casos de uso**: cinco nuevos. Los tres primeros son consultas sin escritura; la
   carga masiva es una variante de CU9 y puede escribirse como curso alternativo.
3. **Reescribir el cálculo de producción de la lactancia** donde el documento lo
   defina como la suma de los controles: pasa a ser una estimación por intervalos.
   Afecta a CU10 y CU11.
4. **Modelo de datos**: no cambia, salvo las dos columnas nuevas de `configuracion`.
   Todo lo demás es derivado.
5. **Diccionario de Clases**: `ControlDiario`, `CandidataDescarte` y los métodos de
   las dos regiones nuevas de la Controladora (`INDICADORES` y `LISTAS DE TRABAJO`).

---

## Limitaciones asumidas

- La proyección a 305 días es lineal (ver punto 4). Un ajuste por curva de lactancia
  daría un número mejor, pero necesita más controles por vaca de los que un tambo
  chico junta.
- **Días abiertos** toma como concepción la fecha del servicio vigente cuando la vaca
  figura preñada. Si el tacto positivo correspondía a un servicio anterior que no se
  registró, el número queda corto.
- Los indicadores se calculan sobre **todo el histórico**, sin filtro de período. Para
  comparar campañas hace falta un rango de fechas, que es material del Módulo 6.
- El tablero recalcula todo en cada carga. Con un rodeo de cientos de animales es
  imperceptible; con miles habría que cachear.
