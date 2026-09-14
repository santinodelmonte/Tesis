# Estimación del Esfuerzo

Fuente de la sección: el método de puntos de función tal como lo presenta el modelo de
la cátedra, aplicado sobre el inventario de funciones de `docs/estimacion_esfuerzo.py`,
que a su vez se verifica contra `Tesis/Dominio/Controladora.cs` y `bd/CreacionDb.sql`.

**Dos cosas quedan para completar** y están marcadas en el texto: la dedicación semanal
que el equipo se fijó, y las horas efectivamente trabajadas, que van en 2.9.

---

La estimación de esfuerzo se realiza en base a los **puntos de función** del proyecto. Se
estima la cantidad de entradas externas, salidas externas, consultas externas, archivos
lógicos internos y archivos de interfaz externos.

**Número de entradas externas (EE):** cada entrada se origina en un usuario o se
transmite desde otra aplicación, y proporciona datos distintos. Normalmente son las
encargadas de actualizar archivos lógicos internos, y se cuentan por separado de las
consultas.

**Número de salidas externas (SE):** cada salida se origina dentro de la aplicación y
proporciona información al usuario. Por ejemplo, informes, mensajes y pantallas de
resultado.

**Número de consultas externas (CE):** una consulta externa es una entrada realizada por
un usuario que genera una respuesta inmediata del sistema, recuperada normalmente de un
archivo lógico interno, sin producir datos derivados.

**Número de archivos lógicos internos (ALI):** cada uno es un agrupamiento lógico de
datos que vive dentro de los límites de la aplicación. Se mantiene a través de entradas
externas y se consulta mediante consultas externas.

**Número de archivos de interfaz externos (AIE):** cada uno es un agrupamiento lógico de
datos externo a la aplicación, que sin embargo le proporciona datos que ésta usa.

El número total de puntos de función se determina con la siguiente fórmula:

**PF = conteo total × [0,65 + 0,01 × ΣFi]**

El conteo total es la suma de los puntos que surgen de la siguiente tabla:

| Parámetro de medición | Bajo | Medio | Complejo |
|---|---|---|---|
| Número de entradas externas | ×3 | ×4 | ×6 |
| Número de salidas externas | ×4 | ×5 | ×7 |
| Número de consultas externas | ×3 | ×4 | ×6 |
| Número de archivos lógicos internos | ×7 | ×10 | ×15 |
| Número de interfaces externas | ×5 | ×7 | ×10 |

**Fi** es un factor de ajuste que surge de las respuestas a un cuestionario de catorce
preguntas. Cada respuesta vale entre 0 y 5, según la escala: 0 sin influencia, 1
incidental, 2 moderado, 3 medio, 4 significativo, 5 esencial.

## El conteo de este sistema

**Entradas externas (46).** El alta, la modificación y la baja de un mismo dato se
cuentan por separado, porque son operaciones distintas sobre el archivo lógico.

| Grupo | Funciones | Cantidad |
|---|---|---|
| Altas | Animal, celo, descorne, diagnóstico, insumo, lactancia, ordeñe individual, ordeñe por lote, parto, plan sanitario, servicio, tacto, tratamiento, vacunación | 14 |
| Modificaciones | Las catorce anteriores salvo el ingreso de stock, más la configuración del establecimiento, las preferencias de notificación y el stock mínimo | 16 |
| Eliminaciones | Celo, descorne, diagnóstico, ordeñe individual, ordeñe por lote, parto, servicio, tacto, tratamiento, vacunación | 10 |
| Otras | Baja de animal, reactivación de animal, carga de fotografía, ingreso de stock, registro de secado | 5 |
| Acceso | Inicio de sesión | 1 |

**Consultas externas (20).** Recuperan información registrada y no calculan nada:
lista de animales, búsqueda y filtrado, ficha integral, consulta de linaje, búsqueda por
caravana en la barra superior, lista de lactancias, historial de producción, listas de
celos, servicios, tactos, partos, diagnósticos, tratamientos, vacunaciones, descornes,
planes sanitarios e insumos, historial de movimientos de stock, vacas para servir y
tactos pendientes.

**Salidas externas (26).** Son las que devuelven algo que el sistema calculó.

| Grupo | Funciones | Cantidad |
|---|---|---|
| Alertas | Secado próximo, parto próximo, stock crítico, vencimiento de insumos, calendario sanitario | 5 |
| Apoyo a la decisión | Tablero de inicio, indicadores del rodeo, candidatas a descarte, métrica de producción mensual | 4 |
| Reportes | Productivo, sanitario, reproductivo y genético, cada uno en pantalla, en PDF y en planilla | 12 |
| Notificaciones | Resumen diario por Telegram | 1 |
| Valores calculados | Categoría propuesta, verificación de consanguinidad, fin del descarte de leche, número de lactancia propuesto | 4 |

**Archivos lógicos internos (20).** Las veinticuatro tablas de la base se agrupan en
veinte archivos lógicos: hembras y machos son la especialización de animales, y las dos
tablas de unión —los animales de un ordeñe por lote y las categorías de un plan
sanitario— viven con la entidad que las gobierna. Los veinte son: animales, razas,
categorías, lactancias, ordeñes por lote, ordeñes individuales, celos, servicios,
tactos, partos, diagnósticos, tratamientos, vacunaciones, descornes, planes sanitarios,
insumos, movimientos de stock, configuración, preferencias de notificación y alertas.

**Interfaces externas (1).** La API del bot de Telegram, de la que el sistema lee el
identificador del chat al vincularlo.

## Puntos de función obtenidos

La complejidad de cada función la asignó el equipo con un criterio único: **es compleja
la que escribe en más de una tabla o propaga consecuencias a otro módulo** —el parto,
que abre una lactancia y da de alta la cría; el tratamiento, que saca a la vaca del
tanque; el servicio, que descuenta una pajuela—.

| Parámetro de medición | Bajo | Medio | Complejo | Subtotal |
|---|---|---|---|---|
| Entradas externas | 22 × 3 | 18 × 4 | 6 × 6 | 174 |
| Salidas externas | 5 × 4 | 13 × 5 | 8 × 7 | 141 |
| Consultas externas | 8 × 3 | 10 × 4 | 2 × 6 | 76 |
| Archivos lógicos internos | 12 × 7 | 6 × 10 | 2 × 15 | 174 |
| Interfaces externas | 1 × 5 | 0 × 7 | 0 × 10 | 5 |
| **Conteo total** | | | | **570** |

## Factor de ajuste

| # | Pregunta | Valor | Por qué |
|---|---|---|---|
| 1 | ¿El sistema requiere respaldo y recuperación confiables? | 4 | La información del rodeo es irrecuperable si se pierde: son años de registros que no están en ningún otro lado. Es el riesgo R9 |
| 2 | ¿Se requieren comunicaciones de datos especializadas? | 3 | El sistema sale a internet contra la API del bot de Telegram |
| 3 | ¿Hay funciones distribuidas de procesamiento? | 0 | Todo el procesamiento ocurre en el servidor |
| 4 | ¿El desempeño es crítico? | 2 | El RNF pide respuesta en menos de cinco segundos, pero con una sola usuaria no hay presión real |
| 5 | ¿El sistema se ejecutará en un entorno con uso pesado de operaciones? | 1 | Hosting compartido, con los límites que impone, pero sin carga propia |
| 6 | ¿El sistema requiere entrada de datos en línea? | 5 | Toda la carga es en línea: no hay ningún proceso por lotes |
| 7 | ¿La entrada en línea se construye en varias pantallas u operaciones? | 3 | El parto, el servicio y el ordeñe por lote se completan en más de un paso |
| 8 | ¿Los ALI se actualizan en línea? | 5 | Cada registro impacta en el momento y se ve en la pantalla siguiente |
| 9 | ¿Las entradas, salidas, archivos o consultas son complejos? | 4 | El linaje recorre la genealogía de forma recursiva y la ficha integral reúne seis orígenes en una pantalla |
| 10 | ¿Es complejo el procesamiento interno? | 4 | La categoría, la consanguinidad, el descarte de leche, el estado de las lactancias y el calendario que arma el plan sanitario |
| 11 | ¿El código diseñado será reutilizable? | 2 | Las tres capas lo permiten, pero el dominio es el de este establecimiento |
| 12 | ¿Se incluyen la conversión y la instalación en el diseño? | 2 | La carga inicial del rodeo está prevista; la migración automática desde los cuadernos es una limitación declarada |
| 13 | ¿Está diseñado para instalaciones múltiples en organizaciones distintas? | 0 | Un solo establecimiento, con configuración única |
| 14 | ¿Está diseñado para facilitar el cambio y el uso? | 4 | Son los RNF de Usabilidad y Mantenibilidad, con sus criterios de verificación |
| | **ΣFi** | **39** | |

Reemplazando en la fórmula:

**PF = 570 × [0,65 + 0,01 × 39] = 570 × 1,04 = 592,8 ≈ 593 puntos de función**

## Una verificación por otro camino

Un conteo de puntos de función es una estimación hecha de juicios, y conviene contrastarla
contra algo que no dependa de los mismos juicios. El sistema terminado tiene **28.684
líneas escritas a mano** —20.716 de C# y 7.968 de vistas—, sin contar las bibliotecas
externas ni los scripts de base de datos. Con la equivalencia habitual para C#, de unas
54 líneas por punto de función, esas líneas dan **alrededor de 531 puntos**.

Las dos cuentas quedan a **un diez por ciento** una de otra, y ninguna de las dos se
apoya en la otra. Es la mejor evidencia disponible de que el conteo no está inflado.

## Del tamaño al esfuerzo

Acá conviene ser explícito, porque es donde una estimación se vuelve honesta o se vuelve
un número decorativo.

La bibliografía convierte puntos de función en horas con un factor de productividad que,
para aplicaciones de gestión en un entorno industrial, se ubica entre **8 y 20 horas por
punto de función**. Aplicado sin más, este proyecto daría entre 4.700 y 11.800 horas: un
número que no tiene ninguna relación con un trabajo final de dos estudiantes.

La diferencia no es un error de conteo —la verificación del apartado anterior lo
descarta— sino de qué incluye cada cosa. **Esas horas cubren una organización**: gestión
de proyecto, un área de calidad separada del desarrollo, ciclos de release, soporte,
rotación de personal y el costo de coordinar equipos grandes. Nada de eso existe acá.
Y hay un segundo efecto: un sistema con mucha alta, modificación y baja sobre un
framework que resuelve el acceso a datos y la capa web **suma puntos de función mucho más
rápido de lo que suma trabajo**, porque cada operación cuenta como función completa y se
escribe siguiendo un patrón ya conocido.

Por eso la estimación de este proyecto no se tomó de la tabla, sino que se ancló en el
calendario disponible y se usó el tamaño para verificar que ese calendario alcanzara. Con
un equipo de dos integrantes, seis iteraciones y una dedicación de
**[COMPLETAR: horas semanales por integrante que el equipo se fijó]**, el esfuerzo
planificado es de **[COMPLETAR: total de horas]**, lo que supone una productividad de
**[COMPLETAR: horas por punto de función]**.

Ese número —y no el de la bibliografía— es el que hay que mirar al cerrar el proyecto, y
es lo que la sección 2.9 retoma al evaluar si la planificación fue realista. El riesgo
**R1, subestimación del tamaño del proyecto**, es exactamente el riesgo de que esta cuenta
esté mal.
