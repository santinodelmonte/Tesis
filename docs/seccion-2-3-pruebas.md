# 2.3 Pruebas

> **Estado de esta sección.** Los casos están escritos con sus datos y su resultado
> esperado, listos para ejecutar. **La columna «Resultado» y las capturas de evidencia
> se completan al correr las pruebas sobre el sistema andando** — no se pueden dar por
> ejecutadas desde acá. Hasta entonces esto es el protocolo; después es el registro.
>
> Los datos son los del rodeo de `bd/DatosPrueba.sql` y los mensajes son los que el
> sistema devuelve de verdad, leídos de `Tesis/Pages` (ver `docs/inventario-pantallas.md`).

---

A continuación se documentan las pruebas realizadas sobre el sistema, siguiendo lo
acordado en el plan de testing del Plan de SQA. Se documenta aquello en lo que se basó
principalmente el testing; la ejecución fue más exhaustiva que el registro.

Las pruebas se agrupan por función y siguen el recorrido completo del sistema. Se
utilizan dos formas de registro: una **tabla**, cuando lo que se prueba son variantes de
un mismo dato de entrada, y el par **prueba / resultado** con la captura de lo que el
sistema devolvió, cuando lo que se verifica es un recorrido completo.

## Entorno de pruebas

Las pruebas se ejecutaron sobre una instalación local con el rodeo de prueba cargado
(`bd/DatosPrueba.sql`), que reproduce un tambo en funcionamiento: animales en ordeñe y
secos, preñeces confirmadas y pendientes de tacto, un tratamiento sanitario en curso con
su descarte de leche vigente, planes sanitarios con procedimientos pendientes e insumos
por debajo de su stock mínimo.

El juego de datos **calcula sus fechas contra el día en que se carga**, de modo que las
alertas y los vencimientos quedan siempre vigentes y las pruebas se pueden repetir
cualquier día sin ajustar nada.

Las pruebas de las notificaciones necesitan, además, el **token del bot cargado** en la
configuración de la aplicación (`bd/LEEME.md`) y **una cuenta de Telegram** desde la cual
escribirle. Sin token, la pantalla lo avisa y no deja vincular: ése es el primer caso de
esa sección.

---

## Acceso al sistema

| Usuario | Contraseña | Resultado esperado | Resultado |
|---|---|---|---|
| `sofia` | `tambo2026` | Ingresa al sistema | |
| `sofia` | *(contraseña incorrecta)* | «Usuario o contraseña incorrectos!» | |
| *(vacío)* | `tambo2026` | Exige completar el usuario | |
| `sofia` | *(vacío)* | Exige completar la contraseña | |

**Prueba:** sin haber iniciado sesión, escribir en el navegador la dirección de una
pantalla interna del sistema.

**Resultado esperado:** el sistema redirige al inicio de sesión y, una vez autenticado,
continúa hacia la pantalla solicitada.

`[captura: t-acceso-directo]`

**Prueba:** cerrar la sesión y volver atrás con el navegador.

**Resultado esperado:** el sistema vuelve a exigir el inicio de sesión; la información no
queda accesible.

`[captura: t-acceso-atras]`

---

## Alta de animales

| Caravana | Fecha de nacimiento | Raza | Resultado esperado | Resultado |
|---|---|---|---|---|
| `200` | válida | Holando | Registra el animal | |
| `200` *(repetida)* | válida | Holando | «El número de caravana ya existe en el sistema!» | |
| *(vacía)* | válida | Holando | «El número de caravana y la raza son obligatorios!» | |
| `201` | válida | *(sin elegir)* | «El número de caravana y la raza son obligatorios!» | |
| `201` | futura | Holando | «La fecha de nacimiento no puede ser futura!» | |

**Prueba:** dar de alta una vaca comprada, con dos partos registrados, y presionar
*Calcular Categoría* antes de guardar.

**Resultado esperado:** el sistema propone la categoría **Vaca**, por tener partos
registrados, y permite aceptarla o sustituirla.

`[captura: t-alta-categoria]`

**Prueba:** dar de alta la caravana `201`, nacida hace dos meses, indicando como madre a
la ternera `177`, que no tenía edad para parir.

**Resultado esperado:** el sistema rechaza el alta con «La madre tiene que haber nacido
al menos 22 meses antes que la cria!» —la edad mínima al servicio configurada, 13 meses,
más los 9 de gestación— y **no ofrece forzarla**: una madre más joven que su cría es
imposible, no sospechosa.

**Prueba:** la misma alta, pero con madre `152` y padre `7HO12165`, que es el padre de
`152`.

**Resultado esperado:** el sistema no la registra automáticamente: advierte que los
progenitores tienen parentesco entre sí y que la cría nace consanguínea, y ofrece
**Guardar de todos modos**.

`[captura: t-alta-genealogia]`

---

## Búsqueda, filtros y ficha

| Filtros aplicados | Resultado esperado | Resultado |
|---|---|---|
| Categoría *Vaca* + estado *En lactancia* | Sólo las vacas en ordeñe; el animal dado de baja no figura | |
| Edad desde `5`, edad hasta `2` | «El rango etario es incorrecto: la edad desde no puede superar a la edad hasta!» | |
| Búsqueda rápida *Crías (0 a 12 meses)* | Sólo los animales de hasta doce meses | |
| Caravana inexistente | «No se encontraron animales con los criterios ingresados!» | |

**Prueba:** abrir la ficha de una vaca con un tratamiento sanitario en curso.

**Resultado esperado:** la ficha muestra en una sola pantalla los datos del animal, su
linaje, el diagnóstico en tratamiento, el descarte de leche vigente, sus partos, sus
servicios y su producción.

`[captura: t-ficha-integral]`

---

## Linaje y consanguinidad

**Prueba:** consultar el linaje de un animal cuya madre y cuyo padre están registrados.

**Resultado esperado:** el sistema arma el árbol con los dos progenitores, permite
desplegar cada rama y saltar a la ficha de cualquier ancestro.

`[captura: t-linaje-arbol]`

| Hembra | Reproductor | Resultado esperado | Resultado |
|---|---|---|---|
| Hija | Su propio padre | Advierte el parentesco e indica el antepasado común | |
| La misma hembra | Un toro sin relación | No detecta parentesco | |
| Un animal | El mismo animal | «No puede verificar un animal contra sí mismo!» | |
| *(vacío)* | *(vacío)* | «Seleccione la hembra y el reproductor!» | |

`[captura: t-consanguinidad]`

---

## Ordeñe por lote

| Dato | Resultado esperado | Resultado |
|---|---|---|
| Fecha y turno nuevos, litros válidos | Registra el ordeñe | |
| Misma fecha y mismo turno, repetidos | «Ya hay un ordeñe registrado para esa fecha y ese turno. Para corregirlo, edítelo desde el historial.» | |
| Fecha futura | «La fecha del ordeñe no puede ser futura!» | |
| Litros `0` o negativos | «Los litros tienen que ser un valor positivo y coherente!» | |
| Todos los animales destildados | «El lote tiene que tener al menos un animal!» | |
| Turno sin elegir | «Seleccione el turno!» | |

**Prueba:** registrar el ordeñe del turno con una vaca que tiene descarte de leche
vigente por un tratamiento sanitario.

**Resultado esperado:** la vaca **no viene tildada y no se puede sumar al lote**. Es el
control que impide que su leche entre al tanque por olvido.

`[captura: t-ordenie-descarte]`

---

## Control lechero

| Dato | Resultado esperado | Resultado |
|---|---|---|
| Litros cargados a varias vacas en ordeñe | Registra todos los controles de una sola vez | |
| Sin cargar ningún litro | «Cargue los litros de al menos un animal!» | |
| Fecha futura | «La fecha del control no puede ser futura!» | |
| Caravana de una vaca **seca** | «El animal no se encuentra en lactancia…» | |
| Litros por encima del máximo configurado | «Los litros tienen que ser un valor positivo y coherente!», con el tope | |
| Mismo animal, fecha y turno, repetidos | Avisa que ya hay un control cargado, con sus litros | |
| Animal sin lactancia abierta en esa fecha | «El animal no tenía una lactancia abierta en esa fecha…» | |

`[captura: t-control-seca]`

---

## Historial, corrección y eliminación

| Dato | Resultado esperado | Resultado |
|---|---|---|
| Rango de fechas válido | Lista los registros del período | |
| Rango invertido | «El rango de fechas es invalido…» | |
| Modalidad sin elegir | «Seleccione la modalidad de visualizacion!» | |

**Prueba:** corregir los litros de un control individual ya registrado.

**Resultado esperado:** la fecha, el turno y la caravana se muestran pero no se pueden
editar —identifican al control—; al guardar, la estimación de la lactancia y la
proyección a 305 días del animal se recalculan.

`[captura: t-correccion-control]`

**Prueba:** eliminar un ordeñe por lote de un turno que además tiene controles
individuales.

**Resultado esperado:** los controles individuales **siguen registrados** —son mediciones
válidas por sí solas— y el turno pasa a figurar como anotado únicamente animal por
animal.

`[captura: t-eliminar-lote]`

---

## Secado y lactancias

| Dato | Resultado esperado | Resultado |
|---|---|---|
| Vaca en lactancia | Cierra la lactancia; el animal pasa a **Seca** | |
| La misma vaca, de nuevo | «El animal no se encuentra en lactancia, así que no hay nada que secar!» | |
| Fecha futura | «La fecha de secado no puede ser futura!» | |
| Abrir lactancia a un animal que ya tiene una abierta | «El animal ya tiene una lactancia abierta!» | |
| Fecha de inicio anterior al nacimiento | «La fecha de inicio no puede ser anterior al nacimiento del animal!» | |

**Prueba:** abrir manualmente la lactancia de una vaca comprada, usando *Proponer* para
el número.

**Resultado esperado:** el sistema propone el número que corresponde según los partos
registrados, y al confirmar el animal pasa a **En lactancia** y aparece en el lote de
ordeñe.

`[captura: t-lactancia-manual]`

---

## Celo, servicio y tacto

| Caso | Resultado esperado | Resultado |
|---|---|---|
| Celo de una vaca en edad | Registra el celo; la vaca aparece en *Vacas para servir* | |
| Celo de una ternera por debajo de la edad mínima | Lo rechaza indicando la edad mínima de detección | |
| Celo de un macho | «La caravana corresponde a un macho: no se puede registrar un celo!» | |
| Celo con fecha posterior a la baja del animal | Lo rechaza por ser posterior a la baja | |
| Servicio sin elegir toro ni pajuela | Lo rechaza | |
| Servicio a una ternera por debajo de la edad mínima | Lo rechaza indicando la edad mínima al servicio | |
| Tacto sin resultado | «Hay que indicar el resultado del tacto!» | |
| Tacto de un animal sin servicio pendiente | «El animal no tiene un servicio pendiente…» | |

**Prueba:** registrar una inseminación artificial eligiendo una pajuela del stock.

**Resultado esperado:** el sistema calcula la fecha probable de parto sumando los días de
gestación; al guardar, **el stock de la pajuela baja una unidad** y queda el egreso
anotado en el historial de movimientos con la caravana del animal inseminado.

`[captura: t-servicio-ia]`

**Prueba:** registrar un servicio entre una hembra y una pajuela de su propio padre.

**Resultado esperado:** el sistema **advierte el parentesco y no guarda automáticamente**,
ofreciendo *Registrar de todos modos*.

`[captura: t-servicio-consanguineo]`

**Prueba:** registrar el tacto de una vaca servida, con resultado **Preñada**, usando
*Ver servicio* antes de confirmar.

**Resultado esperado:** el sistema muestra el servicio que originó la preñez y el parto
proyectado; al guardar, la vaca pasa a **Preñada**, **sigue en lactancia** y sale de
*Tactos pendientes*.

`[captura: t-tacto]`

---

## Parto

| Caso | Resultado esperado | Resultado |
|---|---|---|
| Cría sin caravana | «El numero de caravana de la cria es obligatorio!» | |
| Cría con caravana ya existente | «El numero de caravana de la cria ya existe en el sistema!» | |
| Cría sin raza | «La raza de la cria es obligatoria!» | |
| Parto doble con la misma caravana en las dos crías | «Las dos crias no pueden llevar la misma caravana!» | |
| Fecha del parto futura | «La fecha del parto no puede ser futura!» | |
| Fecha anterior al nacimiento de la madre | «La fecha del parto no puede ser anterior al nacimiento de la madre!» | |
| Parto de un animal dado de baja | «El animal figura dado de baja: no se le puede registrar un parto.» | |
| Parto de una vaca que figura **vacía** | Advierte que no figuraba preñada, **pero deja confirmar** | |

**Prueba:** registrar el parto de una vaca preñada, dando de alta la cría.

**Resultado esperado**, y es la prueba que más cosas verifica de una sola vez:

- La madre queda **En lactancia** y **Vacía**, con un parto más.
- Se abre su lactancia siguiente, numerada como corresponde.
- La cría se da de alta como animal del rodeo, con su categoría calculada.
- El linaje de la cría se arma solo: la madre del parto y el padre del servicio.
- La madre sale de las alertas de parto y entra al lote de ordeñe.

`[captura: t-parto]`

`[captura: t-parto-efecto]`

`[captura: t-parto-linaje]`

**Prueba:** registrar un parto doble con crías de distinto sexo.

**Resultado esperado:** suma **un solo parto y una sola lactancia** a la madre, da de alta
los dos animales, y advierte que la cría hembra nace *freemartin*.

`[captura: t-parto-doble]`

---

## Sanidad

| Caso | Resultado esperado | Resultado |
|---|---|---|
| Diagnóstico sin enfermedad indicada | «La enfermedad o el resultado de la revisacion es obligatorio!» | |
| Tratamiento sin producto | «Seleccione el producto aplicado!» | |
| Tratamiento con duración `0` | «La duracion del tratamiento tiene que ser de al menos un dia!» | |
| Tratamiento sin diagnóstico ni caravana | «Seleccione el diagnostico a tratar, o la caravana del animal…» | |
| Tratamiento preventivo, sin diagnóstico, con caravana y plan | Lo acepta | |
| Segundo descorne al mismo animal | «El animal ya tiene un descorne registrado: es un procedimiento de aplicacion unica.» | |
| Vacunación sin vacuna elegida | «Seleccione la vacuna aplicada!» | |

**Prueba:** registrar un tratamiento con un producto que tiene período de carencia, y
presionar *Calcular* para el descarte de leche.

**Resultado esperado:** el sistema propone como fin del descarte la fecha de inicio más
los días de tratamiento más la carencia del producto. Al guardar, **el stock del producto
baja** las unidades indicadas y el animal queda con descarte vigente.

`[captura: t-tratamiento-descarte]`

**Prueba:** después del tratamiento anterior, cargar el ordeñe por lote de ese día.

**Resultado esperado:** el animal tratado **no está disponible para sumar al lote**. Es la
verificación que cierra el circuito entre sanidad y producción.

`[captura: t-tratamiento-ordenie]`

**Prueba:** crear un plan sanitario nuevo para una categoría y una edad de inicio.

**Resultado esperado:** el calendario sanitario incorpora como pendientes a **todos** los
animales que cumplen la edad y la categoría, sin cargarlos uno por uno.

`[captura: t-plan-calendario]`

**Prueba:** registrar la vacunación pendiente de un animal indicando el plan que cumple.

**Resultado esperado:** el stock de la vacuna baja una dosis con el egreso anotado, y el
animal **sale del calendario**.

`[captura: t-vacunacion]`

---

## Insumos y stock

| Caso | Resultado esperado | Resultado |
|---|---|---|
| Alta de insumo sin nombre | «El nombre del insumo es obligatorio!» | |
| Alta de una pajuela sin toro asociado | «La pajuela tiene que estar vinculada al toro que la aporta…» | |
| Alta de un insumo ya registrado | «Ese insumo ya esta registrado. Si es una reposicion, cargue la partida desde Ingreso de Stock.» | |
| Valores numéricos negativos | «Los valores numericos no pueden ser negativos!» | |
| Ingreso con cantidad `0` | «La cantidad tiene que ser mayor a cero!» | |
| Ingreso con fecha futura | «La fecha del ingreso no puede ser futura!» | |
| Stock mínimo negativo | «El stock minimo tiene que ser mayor o igual a cero!» | |

**Prueba:** consultar las alertas de stock crítico, reponer uno de los insumos listados y
volver a consultarlas.

**Resultado esperado:** el insumo repuesto **desaparece de la alerta**.

`[captura: t-stock-antes]`

`[captura: t-stock-despues]`

**Prueba:** consultar el historial de movimientos filtrando por un insumo consumido en un
tratamiento y en una inseminación.

**Resultado esperado:** se ven el ingreso de la partida y los egresos automáticos, **cada
uno con su motivo**: ningún descuento aparece sin explicación.

`[captura: t-movimientos]`

---

## Tablero y registro rápido

**Prueba:** con el juego de datos recién cargado, entrar al sistema y comparar cada
contador del tablero de inicio con la pantalla de alerta que le corresponde.

**Resultado esperado:** los números coinciden uno por uno —cuatro vacas para servir, un
tacto pendiente, un parto próximo, dos insumos bajo el mínimo y dos partidas por vencer—,
porque el tablero no calcula nada propio: junta las mismas listas que muestran las
pantallas de alerta.

`[captura: t-tablero]`

**Prueba:** en *Registro rápido*, con la solapa **Celo**, tocar la caravana sugerida `130`
en *Para servir* y presionar **Registrar celo**. Sin tocar nada más, escribir `133` y
registrar de nuevo.

**Resultado esperado:** el tablero confirma «Quedó registrado el celo de la caravana 130»,
deja el campo de caravana **vacío y con el foco**, y conserva la solapa y la fecha. El
segundo celo se registra igual. Los dos quedan en *Reproducción → Celos*, cargados sin
haber entrado al menú.

`[captura: t-registro-rapido]`

| Caso | Resultado esperado | Resultado |
|---|---|---|
| Solapa **Tacto** con la `102` y resultado *Preñada* *(con el juego de datos recién cargado)* | La `102` pasa a *Preñada* y sale de *Tactos pendientes*; el contador del tablero baja en uno | |
| Solapa **Servicio** con la `133` | Abre *Registrar servicio* con la caravana ya cargada, sin haber guardado nada | |
| Celo a la caravana `T-01`, que es un macho | «La caravana T-01 corresponde a un macho: los eventos reproductivos se registran sobre la hembra!» | |
| Celo a la ternera `177`, de cuatro meses | La rechaza por la edad mínima de detección de celo, con el mismo mensaje que *Registrar celo* | |
| Celo a la caravana `999`, que no existe | «La caravana 999 no existe en el sistema!» | |
| Celo con el campo de caravana vacío | «Indique la caravana del animal!» | |
| Tacto a la `130`, que no tiene un servicio en espera | «La caravana 130 no tiene un servicio pendiente: hay que registrar el servicio antes del tacto!» | |

Las cinco variantes que fallan informan el motivo **sin salir del tablero** y conservan lo
que se había escrito.

---

## Indicadores

**Prueba:** consultar los indicadores del rodeo antes y después de registrar un secado y
un parto.

**Resultado esperado:** la composición del rodeo por estado productivo se mueve en
consecuencia —una vaca menos en ordeñe por el secado, una más por el parto— y los
promedios se recalculan.

`[captura: t-indicadores]`

**Prueba:** consultar las candidatas a descarte.

**Resultado esperado:** el sistema lista las hembras que cumplen al menos uno de los cinco
criterios, **indicando en cada caso el motivo**.

`[captura: t-descarte]`

---

## Reportes

**Prueba:** *Reportes y notificaciones → Productivo*, con el período que viene propuesto
—del primero del mes a hoy—, presionar **Ver en pantalla**; después **Descargar PDF** y
**Descargar Excel**.

**Resultado esperado:** los dos archivos traen exactamente las mismas secciones, columnas
y filas que la vista en pantalla —*Resumen del período*, *Producción del establecimiento*,
*Producción por animal* y *Controles lecheros del período*—, y se descargan como
`reporte-productivo-` seguido de la fecha de emisión.

`[captura: t-reporte]`

| Caso | Resultado esperado | Resultado |
|---|---|---|
| Reporte **sanitario** del mes | Diagnósticos, Tratamientos, Vacunaciones, Descornes y Situación al día de la emisión | |
| Reporte **reproductivo** del mes | Servicios, Tactos y confirmación de preñez, Partos, Secados e Indicadores reproductivos del rodeo | |
| Reporte **genético** | No pide fechas; trae la Genealogía del rodeo y el Rendimiento por línea paterna, «Rodeo al día de la emisión» | |
| Un período anterior al primer registro del rodeo | Genera igual la vista y los archivos: las secciones del período sin datos dicen «Sin registros en el período» | |
| Fecha *Desde* posterior a *Hasta* | «El rango de fechas es invalido: la fecha desde es posterior a la fecha hasta!», y no genera archivo | |

---

## Notificaciones por Telegram

**Prueba:** escribirle `/start` al bot desde Telegram, copiar el número que contesta en
*Reportes y notificaciones → Notificaciones* y presionar **Vincular y probar**.

**Resultado esperado:** llega a Telegram el mensaje de prueba —«La vinculación quedó
lista…»— y la pantalla confirma «La cuenta quedó vinculada y el mensaje de prueba salió.
Revisá Telegram.», muestra el chat vinculado y habilita la hora del resumen y los ocho
avisos, agrupados por módulo.

`[captura: t-telegram-vinculado]`

| Caso | Resultado esperado | Resultado |
|---|---|---|
| Sin el token del bot cargado | La pantalla avisa «El sistema no tiene cargado el token del bot.» y no deja vincular | |
| Identificador de chat vacío | «Hay que indicar el identificador de chat de Telegram.» | |
| Identificador con letras, como un nombre de usuario | «El identificador de chat es un numero: revise que no haya quedado pegado el nombre de usuario.» | |
| Un número válido de un chat que nunca le escribió al bot | «No se pudo enviar el mensaje de prueba a ese chat…», con el motivo que dio Telegram; **conserva la vinculación anterior** | |

**Prueba:** con el juego de datos recién cargado, poner la hora del resumen un minuto
adelante del reloj, **Guardar** y esperar.

**Resultado esperado:** llega «Tareas pendientes del» y la fecha, agrupado por módulo, con
los mismos números que muestra el tablero de inicio: cuatro vacas para servir, un tacto
pendiente, un parto próximo, los procedimientos del calendario sanitario, dos insumos bajo
el mínimo y dos partidas por vencer. **Ese contraste es la prueba**: el mensaje y el
tablero no pueden discrepar. Si antes se corrieron otras pruebas los números cambian, pero
tienen que seguir coincidiendo.

`[captura: t-telegram-resumen]`

| Caso | Resultado esperado | Resultado |
|---|---|---|
| `/resumen` desde el chat vinculado | Contesta con el resumen en el momento, sin cancelar el envío automático del día | |
| Apagar *Stock crítico*, **Guardar** y pedir `/resumen` | El bloque de stock desaparece del mensaje, y *Pendientes y alertas → Stock crítico* sigue mostrando los dos insumos | |
| Apagar los ocho avisos y pedir `/resumen` | «No hay tareas pendientes.»: el día sin pendientes también recibe mensaje | |
| Reiniciar el sitio después de que salió el resumen del día | No se manda de nuevo | |
| Sitio apagado a la hora del resumen y levantado después | El resumen sale al levantar, en lugar de saltearse el día | |
| `/resumen` desde otra cuenta de Telegram | «Este chat no está vinculado al sistema.», sin un solo dato del rodeo | |
| Un mensaje cualquiera al bot, en una conversación de a dos | Contesta que no entiende y sugiere escribir `/resumen` | |

Al terminar, volver a tildar los ocho avisos y dejar la hora del resumen como estaba.

---

## Configuración

**Prueba:** ampliar el parámetro *Parto próximo* y volver a consultar las alertas de parto.

**Resultado esperado:** aparecen las hembras que antes quedaban fuera de la ventana. Los
partos registrados no cambian: cambia con cuánta anticipación el sistema avisa.

`[captura: t-configuracion-efecto]`

| Caso | Resultado esperado | Resultado |
|---|---|---|
| Reducir los litros máximos por control y cargar uno por encima | Lo rechaza con el tope nuevo en el mensaje | |
| Ampliar los días para el tacto | La lista de *Tactos pendientes* se achica | |
| Valor fuera del rango admitido | Lo rechaza indicando el rango | |

---

## Casos de borde

Tres verificaciones que no salen de una pantalla sino de elegir el dato justo. Se
prueban desde la interfaz como cualquier otra, pero los datos están elegidos a
propósito.

**La categoría, en el borde de la edad de cambio.**

| Sexo | Edad | Partos / destino | Categoría esperada | Resultado |
|---|---|---|---|---|
| Hembra | cualquiera | 1 parto o más | **Vaca** | |
| Hembra | un día **antes** de la edad de cambio | sin partos | **Ternera** | |
| Hembra | **el día exacto** de la edad de cambio | sin partos | **Ternera** | |
| Hembra | un día **después** | sin partos | **Vaquillona** | |
| Macho | por encima de la edad mínima al servicio, reproductor | — | **Toro** | |
| Macho | por encima de la edad mínima al servicio, no reproductor | — | **Novillo** | |
| Macho | por debajo de la edad de cambio | — | **Ternero** | |

El caso del medio es el que justifica la prueba: el animal que cumple **exactamente**
la edad de cambio todavía es ternera, y recién al día siguiente pasa a vaquillona.

**El alcance de la verificación de consanguinidad.**

| Relación entre los animales | Resultado esperado | Resultado |
|---|---|---|
| Padre e hija | Detecta el parentesco | |
| Medios hermanos por padre | Detecta el parentesco | |
| Nieta y abuelo | Detecta el parentesco | |
| Primos por bisabuelo | **No detecta** | |
| Sin relación | No detecta | |

La anteúltima fila es un límite del sistema, no un error de carga: la verificación
compara la ascendencia hasta el nivel de los abuelos, y un parentesco que dependa de un
bisabuelo compartido queda fuera. Está declarado en las limitaciones del proyecto.

**La estimación de la lactancia, abierta y cerrada.**

| Situación de la lactancia | Resultado esperado | Resultado |
|---|---|---|
| Sin ningún control cargado | Estimación **0** | |
| Con controles, lactancia **abierta** | La estimación crece día a día aunque no se cargue nada | |
| Con controles, lactancia **cerrada** por secado | La estimación queda fija | |

Una lactancia abierta se estima contra el día de hoy; una cerrada, contra su fecha de
secado. Verificarlo evita interpretar mal el ranking de producción.

---

## Registro y corrección de errores

Los errores detectados durante las pruebas se anotan con la prueba que los encontró, el
comportamiento observado y el esperado. Cada corrección se verifica **repitiendo la prueba
que lo detectó** y, cuando el error tocaba una regla compartida, las pruebas de las
funciones que dependen de ella.

| # | Prueba que lo detectó | Comportamiento observado | Corrección | Verificado |
|---|---|---|---|---|
| | | | | |

> **Esta tabla se completa al ejecutar.** Los errores que ya se corrigieron durante el
> desarrollo, con su fecha y su solución, se cuentan en la sección 2.9.
