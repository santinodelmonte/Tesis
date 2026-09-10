# 2.4 Manual de Usuario

> **Cómo está escrito esto.** Los campos, las acciones y los mensajes de cada pantalla
> salen de `docs/inventario-pantallas.md`, que se genera leyendo `Tesis/Pages`. No hay
> ninguna pantalla descrita de memoria.
>
> **Las capturas todavía no están.** Cada una aparece como una marca `[captura: …]`
> seguida del pie de figura ya escrito. Cuando se
> corra el script de capturas sobre el sistema andando, el paso de editar las coloca
> en su lugar con ese pie. Los nombres coinciden con `docs/guion-capturas.md`.

---

## Índice

1. Introducción
2. Entrar al sistema
   - 2.1 Iniciar sesión
   - 2.2 Moverse por el sistema
   - 2.3 Cerrar sesión
3. Configurar el establecimiento
4. Los animales del rodeo
   - 4.1 Ver el rodeo
   - 4.2 Buscar y filtrar
   - 4.3 Dar de alta un animal
   - 4.4 La ficha del animal
   - 4.5 Modificar un animal
   - 4.6 El linaje
   - 4.7 Verificar consanguinidad
   - 4.8 Dar de baja y reactivar
5. La producción de leche
   - 5.1 El ordeñe del turno
   - 5.2 El control lechero
   - 5.3 El historial y la métrica mensual
   - 5.4 Lactancias: abrir y secar
   - 5.5 Corregir y eliminar
6. La reproducción
   - 6.1 Las listas de trabajo
   - 6.2 Celo
   - 6.3 Servicio
   - 6.4 Tacto
   - 6.5 Parto
   - 6.6 Corregir y eliminar
7. La sanidad
   - 7.1 El calendario sanitario
   - 7.2 Diagnóstico y tratamiento
   - 7.3 Vacunación y descorne
   - 7.4 Los planes sanitarios
8. Los insumos
   - 8.1 Dar de alta un insumo
   - 8.2 Reponer stock
   - 8.3 Las alertas
   - 8.4 El historial de movimientos
9. Indicadores y decisiones
10. Reportes y notificaciones
   - 10.1 Los reportes
   - 10.2 Las notificaciones por Telegram
11. Usar el sistema desde el celular

---

## 1. Introducción

Este manual explica cómo usar el sistema de gestión del tambo. Está organizado en el
orden en que el trabajo ocurre en el establecimiento y no en el orden en que el
sistema se construyó: primero entrar, después el rodeo, después lo que se le hace al
rodeo todos los días.

**Hay una idea que conviene tener presente desde el principio y que explica casi todo
lo demás: cada dato que se carga tiene consecuencias en los otros módulos.** El
sistema no son cinco listas separadas. Un tratamiento saca a la vaca del tanque de
leche; un parto abre una lactancia y da de alta un animal nuevo; una inseminación
descuenta una pajuela del stock. Por eso vale la pena cargar los eventos cuando
ocurren: lo que se anota una vez, el sistema lo propaga solo.

**El sistema calcula solo varias cosas, y es importante saber cuáles**, porque son las
que no hay que escribir a mano y las que van a aparecer sin que nadie las cargue:

| El sistema calcula | A partir de |
|---|---|
| La **categoría** del animal | El sexo, la fecha de nacimiento y la cantidad de partos |
| La **fecha probable de parto** | La fecha del servicio más los 283 días de gestación |
| La **fecha recomendada de secado** | La fecha probable de parto menos los días de secado configurados |
| El **fin del período de descarte de leche** | El fin del tratamiento más la carencia del producto aplicado |
| Las **tareas pendientes** del calendario sanitario | Los planes configurados y lo que ya se aplicó |
| La **proyección de producción a 305 días** | Los controles lecheros de la lactancia en curso |

Todas se pueden revisar antes de guardar, y las que son una propuesta —como la
categoría— se pueden cambiar.

**Sobre las advertencias.** El sistema distingue entre lo que no deja hacer y lo que
avisa. Cuando algo está mal —una fecha futura, una caravana repetida— lo rechaza y
explica por qué. Cuando algo es inusual pero posible en el campo —una vaca que pare
sin figurar preñada, un servicio entre parientes— **avisa y deja seguir**, con un
botón que dice *Guardar de todos modos* o *Registrar de todos modos*. Esa distinción
es deliberada: el sistema conoce las reglas del tambo, pero la que decide es la
encargada.

---

## 2. Entrar al sistema

### 2.1 Iniciar sesión

El sistema se abre en el navegador. Antes de mostrar nada pide usuario y contraseña:
no hay ninguna pantalla accesible sin iniciar sesión, y escribir directamente la
dirección de una pantalla interna lleva al inicio de sesión.

`[captura: m0-cu01-login]`
> Pantalla de inicio de sesión. Es lo primero que aparece al entrar al sistema y lo
> único accesible sin haberse autenticado.

Se completan **Usuario** y **Contraseña** y se presiona **Ingresar**. Si alguno de los
dos está mal, el sistema responde *«Usuario o contraseña incorrectos!»* sin decir cuál
de los dos falló, que es lo correcto: decirlo ayudaría a quien esté probando entrar.

El sistema tiene **un solo usuario**, el de la encargada del establecimiento. No hay
que administrar cuentas ni permisos.

### 2.2 Moverse por el sistema

Una vez adentro, el menú es una **columna a la izquierda**, con una sección por módulo
—Animales, Producción, Reproducción, Sanidad, Insumos, Indicadores, y Reportes y
notificaciones— y, arriba de todas, **Pendientes y alertas**, que junta las siete listas
de trabajo del día. Al pie de la columna, aparte, está **Configuración**. Cada entrada
abre primero el listado de lo que contiene; desde el
listado se llega a agregar, ver el detalle, corregir o eliminar. En una pantalla angosta
la columna se pliega detrás del botón de menú.

`[captura: m0-cu02-sesion]`
> La barra superior, con el buscador de caravana y el nombre de la usuaria conectada;
> a la izquierda, el menú con una sección por módulo.

**El buscador de caravana está siempre a la vista.** Escribir un número y confirmar
lleva directo a la ficha de ese animal, desde cualquier pantalla y sin pasar por
ningún listado. Es el camino más corto cuando se tiene el animal delante y hay que
consultar algo.

### 2.3 Cerrar sesión

El botón **Cerrar sesión** está en la barra superior y se puede usar desde cualquier
pantalla. Después de cerrarla, volver atrás con el navegador no muestra la información:
lleva de nuevo al inicio de sesión.

---

## 3. Configurar el establecimiento

**Configuración**, al pie del menú, abre una única pantalla con **once parámetros** que ajustan
cómo el sistema calcula fechas, qué rechaza y con cuánta anticipación avisa. Vienen con
valores por defecto que sirven para un tambo Holando, y conviene revisarlos una vez al
empezar a usar el sistema.

`[captura: m0-cu03-configuracion]`
> Configuración del establecimiento. Los once parámetros con que el sistema calcula
> fechas recomendadas, valida cargas y arma los avisos.

| Parámetro | Para qué sirve | Por defecto |
|---|---|---|
| Días de secado antes del parto | Cuánto antes del parto probable se recomienda secar la vaca | 60 |
| Edad mínima al servicio (meses) | A qué edad entra la vaquillona en servicio: por debajo, el sistema no deja registrarle uno. También es el piso que usa la validación de genealogía. Alcanza sólo a las hembras: el macho pasa a toro a los 15 meses, fijo | 13 |
| Edad de cambio de categoría (meses) | Cuándo la cría deja de ser ternera o ternero | 12 |
| Ordeñes por día | Cuántos turnos tiene la jornada | 2 |
| Litros máximos por control individual | Tope de coherencia: por encima, el sistema supone un error de tipeo | 100 |
| Espera voluntaria posparto (días) | Cuánto se espera después del parto antes de volver a servir | 45 |
| Días para el tacto | A los cuántos días del servicio la vaca aparece en Tactos Pendientes | 35 |
| Secado próximo (días) | Con cuánta anticipación avisa la alerta de secado | 15 |
| Parto próximo (días) | Con cuánta anticipación avisa la alerta de parto | 15 |
| Calendario sanitario (días) | Con cuánta anticipación se muestran los procedimientos pendientes | 30 |
| Vencimiento de insumos (días) | Con cuánta anticipación avisa el vencimiento de una partida | 30 |

Cada valor tiene un rango admitido y el sistema rechaza los que quedan fuera,
explicando el motivo. No hace falta completar todos: los que no se toquen conservan su
valor por defecto.

**Cambiar un parámetro cambia lo que se ve en las alertas, no lo que ya está
registrado.** Es la forma más rápida de entenderlo:

`[captura: m0-cu03-efecto]`
> Alertas de parto después de ampliar *Parto próximo* de 15 a 30 días: aparece una vaca
> más, que antes quedaba fuera de la ventana. Los partos registrados no cambiaron; lo
> que cambió es con cuánta anticipación el sistema avisa.

---

## 4. Los animales del rodeo

### 4.1 Ver el rodeo

**Animales → Rodeo** muestra el rodeo activo. Los animales dados de baja no
aparecen acá: siguen en el sistema y se los encuentra con el filtro correspondiente,
pero no ensucian la lista de todos los días.

`[captura: m1-cu10-lista]`
> Lista de animales. Muestra el rodeo activo con su caravana, categoría, raza y estado;
> los animales dados de baja no figuran.

Cuando un animal tiene la categoría desactualizada —una vaquillona que ya debería ser
vaca por su edad—, la fila lo señala y ofrece **Actualizar Categoría**. El sistema no
la cambia solo: propone y espera.

### 4.2 Buscar y filtrar

**Animales → Buscar y filtrar** combina filtros por estado, categoría, raza, rango de
edad en meses y número de caravana.

`[captura: m1-cu10-filtros]`
> Búsqueda con dos filtros combinados —categoría *Vaca* y estado *En lactancia*—: el
> resultado son las vacas que hoy están dando leche.

Arriba hay **búsquedas rápidas** que resuelven de un clic lo que se consulta seguido:
*Rodeo activo*, *Crías (0 a 12 meses)*, *Recría (13 a 24 meses)*, *Vacas*, *Toros*,
*Todos los inactivos* y *Todo el histórico*.

Si el rango de edad está invertido, el sistema avisa: *«El rango etario es incorrecto:
la edad desde no puede superar a la edad hasta!»*.

### 4.3 Dar de alta un animal

**Animales → Rodeo → Agregar animal**.

`[captura: m1-cu04-alta]`
> Alta de animal. La madre y el padre se eligen con el botón *Buscar*, que abre el
> selector del rodeo en vez de pedir que se escriba una caravana de memoria.

| Campo | Qué va |
|---|---|
| Número de Caravana | La identificación del animal. **Tiene que ser única** |
| Fecha de Nacimiento | No puede ser futura |
| Sexo | Hembra o macho. Define qué otros campos aparecen |
| Raza | Obligatoria |
| Madre / Padre | Opcionales: un animal comprado puede no tener padres en el sistema |
| Partos Registrados | Sólo en hembras. Sirve para las compradas que llegan con partos previos |
| En pie | Sólo en machos. Marca si integra el rodeo como reproductor |
| Categoría | **La propone el sistema** |

**La categoría es el campo que conviene entender.** Viene en *«La calcula el sistema»*.
Al presionar **Calcular Categoría**, el sistema la deduce del sexo, la fecha de
nacimiento y la cantidad de partos, y la propone. Se puede aceptar o elegir otra.

`[captura: m1-cu04-categoria]`
> Al presionar *Calcular Categoría* el sistema propone **Vaca**, porque el animal tiene
> partos registrados. La propuesta se puede aceptar o sustituir: es una ayuda, no una
> imposición.

El sistema rechaza el alta si falta la caravana o la raza, si la fecha de nacimiento es
futura, o si **la caravana ya existe**.

**También rechaza una genealogía imposible**, sin opción de forzarla: un animal como su
propio padre o madre, un progenitor que desciende del animal que se está cargando, o un
progenitor que no tenía edad para engendrarlo. La madre tiene que haber nacido al menos
**22 meses** antes que la cría —la edad mínima al servicio configurada, 13 meses, más los
9 de gestación— y el padre, **24**: los 15 meses del toro más la gestación. Una madre más
joven que eso es imposible, no sospechosa.

Y hay casos que **no rechaza, avisa**, porque en un tambo pueden ser ciertos aunque
parezcan errores: que la madre figure dada de baja antes del nacimiento, que el padre
figure dado de baja antes de la concepción —es correcto si la cría vino de una pajuela
suya— o que los dos progenitores sean parientes.

`[captura: m1-cu04-genealogia]`
> Advertencia de genealogía: la madre y el padre elegidos son parientes —él es su padre—
> y la cría nace consanguínea. El sistema no lo registra automáticamente, pero ofrece
> *Guardar de todos modos*: el dato puede ser correcto, y la decisión es de la encargada.

### 4.4 La ficha del animal

Desde cualquier listado, o escribiendo la caravana en el buscador de la barra superior,
se llega a la ficha: **todo lo que el sistema sabe de ese animal en una sola pantalla**.

`[captura: m1-cu11-ficha]`
> Ficha integral de un animal: sus datos, su foto, su linaje, y su historial productivo,
> reproductivo y sanitario. En este caso, una vaca con un diagnóstico en tratamiento y
> descarte de leche vigente, que por eso no se puede sumar al ordeñe.

Es la pantalla que más se usa cuando se tiene el animal delante y hay que decidir algo.

### 4.5 Modificar un animal

Desde la lista, la acción de editar abre la misma pantalla del alta con los datos
cargados. **Se puede modificar todo**, incluida la caravana —para corregir una mal
tipeada— y la cantidad de partos.

`[captura: m1-cu05-foto]`
> Modificación de un animal con la fotografía cargada. La foto aparece después en su
> ficha y en el árbol genealógico de sus crías.

Las validaciones son las mismas que en el alta: la caravana nueva no puede pisar la de
otro animal, y la genealogía se vuelve a verificar.

### 4.6 El linaje

**Animales → Linaje** arma el árbol genealógico a partir de los partos
registrados y de los padres cargados en cada alta. **Nadie lo dibuja: se arma solo.**

`[captura: m1-cu08-linaje]`
> Árbol genealógico. Cada rama se despliega y desde cualquier ancestro se salta a su
> ficha. El árbol se arma con los progenitores registrados: no se carga a mano.

El árbol se puede desplegar por generaciones, contraer, acercar y alejar, ver como
tabla, y recentrar en cualquier animal para seguir desde ahí.

### 4.7 Verificar consanguinidad

**Animales → Consanguinidad** responde una pregunta concreta antes de decidir
un servicio: *¿esta hembra y este reproductor son parientes?*

`[captura: m1-cu09-consanguinidad]`
> Verificación de consanguinidad. El sistema compara la ascendencia de los dos animales
> hasta los abuelos y avisa que encontró un antepasado en común, diciendo cuál es.

**La advertencia es informativa: no impide nada.** El sistema no se limita al
parentesco directo: compara **la ascendencia de los dos animales hasta el nivel de los
abuelos** —padres y abuelos de cada uno— y avisa si encuentra un antepasado en común.
Un parentesco más lejano que un bisabuelo compartido no lo detecta. Si se elige el
mismo animal en los dos campos, responde *«No puede verificar un animal contra sí
mismo!»*.

### 4.8 Dar de baja y reactivar

Cuando un animal sale del rodeo —venta, fallecimiento, descarte sanitario u otro
motivo— se registra la baja desde su ficha.

`[captura: m1-cu06-baja]`
> Baja de un animal. Se elige el motivo de salida y se confirma. La baja es lógica: el
> animal sale de las listas y de los desplegables, pero su historia y su lugar en el
> árbol genealógico se conservan.

**La baja no borra nada.** El animal deja de aparecer en la lista del rodeo y en los
desplegables donde se elige un animal, pero su ficha, su historia sanitaria y su lugar
en el linaje de sus crías siguen ahí. Se lo encuentra con el filtro de dados de baja.

Y se puede deshacer:

`[captura: m1-cu07-reactivar]`
> Reactivación. Un animal dado de baja por error vuelve al rodeo, y el sistema limpia su
> fecha y su motivo de salida.

---

## 5. La producción de leche

El sistema mide la leche de dos formas distintas y complementarias. **Conviene tener
clara la diferencia antes de usarlas**, porque es la duda más frecuente:

| | Qué mide | Cada cuánto |
|---|---|---|
| **Ordeñe por lote** | Los litros totales que dio el tanque en un turno | Todos los días, en cada turno |
| **Control lechero** | Cuánto dio **cada vaca** | Una vez por mes |

El primero dice cuánto produce el establecimiento. El segundo dice qué vaca lo produce,
y es el que alimenta las proyecciones y el ranking. **No se suman entre sí dentro del
mismo turno**: son dos miradas sobre la misma leche.

### 5.1 El ordeñe del turno

**Producción → Ordeñe por lote**. Se elige el turno, la fecha y se cargan los litros
totales. Abajo viene la lista de los animales en ordeñe, tildados por defecto.

`[captura: m2-cu12-lote]`
> Registro del ordeñe del turno. Se anotan los litros del tanque y se confirma qué
> animales integraron el lote; vienen tildados los que están en lactancia.

**Hay vacas que están en ordeñe pero cuya leche no va al tanque**, y el sistema no deja
sumarlas:

`[captura: m2-cu12-descarte]`
> Una vaca en tratamiento no se puede tildar: tiene descarte de leche vigente. El
> sistema lo sabe porque el tratamiento se registró en Sanidad, y así evita que su leche
> entre al tanque por olvido. **Es el control que cierra el circuito entre sanidad y
> producción.**

El sistema rechaza el ordeñe si la fecha es futura, si los litros no son positivos o
son incoherentes, si el lote queda sin ningún animal, o si **ya hay un ordeñe cargado
para esa fecha y ese turno** — en ese caso indica que se corrija desde el historial en
lugar de cargar uno nuevo.

### 5.2 El control lechero

**Producción → Control lechero** carga la medición de todo el rodeo en ordeñe de una
sola vez, que es como se hace: se mide a todas el mismo día.

`[captura: m2-cu13-masiva]`
> Control lechero. Se carga la medición de cada vaca en una sola pasada; el buscador de
> arriba ayuda a ubicar una caravana cuando la lista es larga.

Si hace falta cargar una sola vaca —porque se midió aparte, o desde el botón de su
ficha— está la variante puntual:

`[captura: m2-cu13-puntual]`
> Carga de un control individual. Se llega desde el control lechero o desde la ficha del
> animal, que además llega con la caravana ya elegida.

Dos cosas que el sistema no deja hacer, y que son reglas del tambo y no del programa:

`[captura: m2-cu13-seca]`
> No se puede cargar un control a una vaca seca: el sistema responde que su estado
> productivo es *Seca* y que por lo tanto no está dando leche. La medición pertenece a
> una lactancia, y una vaca seca no tiene ninguna abierta.

`[captura: m2-cu13-maximo]`
> Litros por encima del máximo configurado. El sistema lo rechaza e informa el tope, que
> se ajusta desde Configuración. Es un control de coherencia: una vaca no da esa
> cantidad en un turno, así que casi siempre es un error de tipeo.

El control se imputa a **la lactancia que estaba en curso en la fecha del control**, no
a la actual: la carga puede ser retroactiva sin que los números se desordenen.

### 5.3 El historial y la métrica mensual

**Producción → Historial de producción** consulta lo registrado en un rango de fechas,
eligiendo si se quiere ver la producción del establecimiento o los controles
individuales.

`[captura: m2-cu14-historial]`
> Historial de producción en un rango de fechas. Es también el listado desde el que se
> corrigen y se eliminan los registros.

Si el rango está invertido, el sistema avisa que la fecha desde es posterior a la fecha
hasta.

`[captura: m2-cu15-metrica]`
> Métrica mensual: el total del mes, el promedio por ordeñe y el promedio por vaca. La
> leche de un turno se cuenta una sola vez aunque ese día haya lote y controles
> individuales.

### 5.4 Lactancias: abrir y secar

Una **lactancia** es el período en que la vaca da leche, entre un parto y el secado
siguiente. Normalmente **se abre sola al registrar el parto** y no hay que hacer nada.

Hay un caso en que sí hay que abrirla a mano: **las vacas que ya estaban en ordeñe
cuando se empezó a usar el sistema**, y las compradas que llegan en producción. Su parto
no está registrado porque ocurrió antes.

`[captura: m2-cu18-lactancia]`
> Apertura manual de una lactancia. El botón *Proponer* sugiere el número que
> corresponde según los partos registrados del animal.

El sistema rechaza la apertura si el animal ya tiene una lactancia abierta, si no es una
hembra del rodeo, o si la fecha de inicio es futura o anterior a su nacimiento.

`[captura: m2-lactancias]`
> Listado de lactancias, con su número, su fecha de inicio y su estado. Es donde se ve de
> un vistazo qué vacas están en leche y desde cuándo.

El **secado** cierra la lactancia: la vaca deja de ordeñarse para descansar antes del
parto siguiente.

`[captura: m2-cu16-secado]`
> Registro del secado. Al confirmarlo, la lactancia se cierra con esa fecha, el animal
> pasa a estado **Seca** y deja de aparecer en el lote de ordeñe.

Si el animal no tiene lactancia abierta, el sistema responde que *«no hay nada que
secar!»*.

`[captura: m2-cu17-alertas]`
> Alertas de secado. Lista las vacas cuya fecha probable de parto está dentro de la
> ventana configurada, para secarlas a tiempo. El sistema la calcula: nadie lleva esa
> cuenta a mano.

### 5.5 Corregir y eliminar

Los registros de producción se corrigen y se eliminan **desde el historial**, que es su
listado. Primero se busca el rango, y las acciones aparecen sobre las filas encontradas.

`[captura: m2-cu19-corregir]`
> Corrección de un control individual. La fecha, el turno y la caravana se ven pero no
> se pueden editar: son los que identifican al control. Lo que se corrige son los litros.

`[captura: m2-cu19-eliminar]`
> Confirmación antes de eliminar. Nombra la caravana, la fecha, el turno y los litros
> del registro que se va a borrar, para que no haya duda de cuál es.

**Eliminar un ordeñe por lote no elimina los controles individuales de ese turno**: son
mediciones válidas por sí solas. El turno pasa a figurar como anotado únicamente vaca
por vaca, y el acumulado del período baja hasta la suma de esos controles.

---

## 6. La reproducción

El ciclo reproductivo es una cadena: **celo → servicio → tacto → parto**, y cada eslabón
habilita el siguiente. El sistema la sigue y mantiene el **estado reproductivo** de cada
hembra —vacía, servida o preñada— actualizado a partir de esos eventos.

Ese estado es **independiente del estado productivo**: una vaca puede estar preñada y en
lactancia al mismo tiempo, que es lo normal durante buena parte del año.

### 6.1 Las listas de trabajo

Antes de registrar nada conviene mirar las dos listas que el sistema arma solo, porque
dicen qué animales necesitan atención hoy.

`[captura: m3-cu25-servir]`
> Vacas para servir: las hembras en condiciones de recibir servicio, con el motivo por el
> que figuran. El sistema las selecciona por edad, estado reproductivo y período de
> espera posparto.

`[captura: m3-cu22-pendientes]`
> Tactos pendientes: los servicios que ya cumplieron los días configurados y todavía no
> tienen tacto registrado.

### 6.2 Celo

**Reproducción → Celos → Registrar celo**. Se elige la caravana, la fecha de detección y se
anota lo observado.

`[captura: m3-cu20-celo]`
> Registro de una detección de celo. Las observaciones importan: es lo que después
> permite entender por qué se decidió servir a ese animal.

El sistema rechaza el celo si la caravana corresponde a un macho, si el animal está por
debajo de la edad mínima de detección de celo —**9 meses**—, o si la fecha es posterior
a la baja del animal.

`[captura: m3-cu20-edad]`
> Celo rechazado por edad: el animal todavía no alcanza los meses mínimos. La ternera
> aún no manifiesta celo, y registrarlo sería un error de caravana o de fecha.

### 6.3 Servicio

**Reproducción → Servicios → Registrar servicio** registra el intento de preñar, que puede ser de
dos tipos.

`[captura: m3-cu21-ia]`
> Servicio por inseminación artificial. Al elegir la pajuela, el sistema calcula la fecha
> probable de parto sumando los 283 días de gestación; el botón *Recalcular* la vuelve a
> proponer si se cambia la fecha del servicio.

`[captura: m3-cu21-monta]`
> Servicio por monta natural. Al cambiar el tipo, el selector de pajuela se esconde y
> aparece el de toro del rodeo: son excluyentes, porque un servicio tiene un solo
> reproductor.

**La inseminación descuenta la pajuela del stock automáticamente**, y deja el egreso
anotado en el historial de movimientos con la caravana del animal inseminado. No hay que
descontarla aparte.

`[captura: m3-cu21-consanguineo]`
> Advertencia de consanguinidad al registrar el servicio: la pajuela elegida es del padre
> de esa hembra. El sistema no lo bloquea —ofrece *Registrar de todos modos*— porque la
> decisión es de la encargada, pero se asegura de que no pase inadvertido.

### 6.4 Tacto

El **tacto** es la palpación que confirma o descarta la preñez, y se registra a los días
configurados del servicio.

`[captura: m3-cu22-tacto]`
> Registro de un tacto. El botón *Ver servicio* muestra sobre qué servicio se está
> registrando —su fecha, su reproductor y el parto proyectado— antes de guardar.

El resultado es obligatorio. Si el animal no tiene un servicio pendiente, el sistema lo
dice: hay que registrar el servicio antes que el tacto.

Un tacto **positivo** deja al animal preñado; uno **negativo** lo devuelve a vacía, y
vuelve a aparecer en *Vacas para servir* cuando entre en celo. **El tacto no toca el
estado productivo**: una vaca en lactancia que queda preñada sigue en lactancia.

### 6.5 Parto

Es el evento que más cosas dispara, y por eso conviene mirarlo con calma.

`[captura: m3-cu23-alertas]`
> Alertas de parto próximo: las hembras cuya fecha probable de parto entra en la ventana
> configurada. Es la lista que se mira todos los días en la época de partos.

`[captura: m3-cu24-parto]`
> Registro de un parto. Arriba los datos de la madre; al presionar *Cargar datos* el
> sistema muestra el servicio que originó la preñez y el parto que había proyectado.
> Abajo, la cría, con el padre ya propuesto a partir de ese servicio.

Se completan los datos de la madre y los de la cría —caravana, sexo, raza y, si se
quiere, la foto—. Si el parto fue doble se tilda **Parto doble** y se carga la segunda
cría.

**Un solo parto registrado hace todo esto:**

- La madre queda **en lactancia** y **vacía**, con un parto más en su cuenta.
- Se le abre la **lactancia siguiente**, numerada como corresponde.
- La cría se **da de alta como animal del rodeo**, con su categoría calculada.
- El **linaje de la cría se arma solo**, con la madre del parto y el padre del servicio.
- La madre **sale de las alertas de parto** y entra al lote de ordeñe.

`[captura: m3-cu24-efecto]`
> La ficha de la madre después del parto: **En lactancia** y **Vacía**, con su lactancia
> nueva abierta y un parto más. Nada de esto se cargó a mano.

`[captura: m3-cu24-cria]`
> El linaje de la cría recién nacida, armado por el sistema: la madre viene del parto y
> el padre del servicio que lo originó.

Un **parto doble suma un solo parto y una sola lactancia** a la madre, pero da de alta
los dos animales. Y si las crías son de distinto sexo, el sistema avisa: la hembra nace
*freemartin* y en la enorme mayoría de los casos queda estéril, así que no conviene
criarla como futura lechera. Es el aviso que evita descubrirlo dos años después.

El sistema **advierte pero deja seguir** cuando la madre no figuraba preñada, cuando la
gestación quedó fuera del rango normal, o cuando hay parentesco con el reproductor.
Rechaza, en cambio, el parto sin caravana de cría, con caravana repetida, con fecha
futura o anterior al nacimiento de la madre.

### 6.6 Corregir y eliminar

Cada listado del módulo —Celos, Servicios, Tactos, Partos— permite corregir y eliminar
sus registros.

`[captura: m3-cu26-corregir]`
> Corrección de un parto. Se pueden ajustar la fecha, el tipo, las observaciones y los
> datos de las crías.

`[captura: m3-listas]`
> Listado de servicios. Además de corregir y eliminar, permite *Ajustar* la fecha
> probable de parto cuando el veterinario la corrige por tacto.

**Al corregir o eliminar un evento, el sistema vuelve a deducir el estado reproductivo
del animal** a partir de los eventos que quedan. No hay que arreglarlo a mano.

---

## 7. La sanidad

El módulo sanitario tiene dos mitades. Una es **reactiva**: el animal se enferma, se lo
diagnostica y se lo trata. La otra es **preventiva**: los planes sanitarios dicen qué le
corresponde a cada categoría y cada cuánto, y el sistema arma solo el calendario de lo
que está pendiente.

### 7.1 El calendario sanitario

**Pendientes y alertas → Calendario sanitario** es la pantalla que responde *¿qué hay que hacerle hoy
al rodeo?*

`[captura: m4-cu31-calendario]`
> Calendario sanitario. Lista los procedimientos pendientes y vencidos, calculados a
> partir de los planes configurados y de lo que ya se aplicó. Nadie carga esta lista: el
> sistema la deduce.

Se puede acotar por horizonte de días, por tipo de procedimiento y por categoría. El
horizonte viene del parámetro configurado y se puede cambiar en la propia pantalla.

### 7.2 Diagnóstico y tratamiento

Un **diagnóstico** registra qué tiene el animal. Un **tratamiento** registra qué se le
aplicó, y puede colgar de un diagnóstico o ser preventivo.

`[captura: m4-cu27-diagnostico]`
> Registro de un diagnóstico. El estado distingue los cuadros en curso de los ya
> resueltos, y es lo que después permite ver qué animales están enfermos hoy.

`[captura: m4-cu28-tratamiento]`
> Registro de un tratamiento. El botón *Calcular* propone hasta cuándo la leche de ese
> animal no se puede vender: la fecha de inicio, más los días de tratamiento, más la
> carencia del producto aplicado. Se puede ajustar a mano si el veterinario indica otra.

**Este es el punto donde sanidad y producción se tocan**, y conviene entenderlo bien:

- El tratamiento **descuenta del stock** las unidades indicadas y deja el egreso anotado.
- El animal queda con **descarte de leche vigente** hasta la fecha calculada.
- Mientras dure, **no se lo puede sumar al ordeñe por lote** (ver 5.1).

`[captura: m4-cu28-ficha]`
> La ficha sanitaria del animal, con el tratamiento colgado de su diagnóstico y el
> descarte de leche vigente a la vista.

Un tratamiento **preventivo** no necesita diagnóstico: se deja el campo en *«Sin
diagnóstico»* y se elige directamente la caravana y el plan que cumple.

`[captura: m4-cu28-preventivo]`
> Tratamiento preventivo: sin diagnóstico previo, aplicado según un plan sanitario. Es el
> caso de una desparasitación de rutina.

Cuando el cuadro se resuelve, se lo cierra desde el listado:

`[captura: m4-cu33-cerrar]`
> Cierre de un diagnóstico desde el listado. El animal deja de figurar entre los cuadros
> sanitarios activos.

### 7.3 Vacunación y descorne

`[captura: m4-cu29-vacunacion]`
> Registro de una vacunación. Al indicar el plan que cumple, el animal sale del
> calendario sanitario y el stock de la vacuna baja una dosis.

`[captura: m4-cu31-despues]`
> El calendario después de aplicar la vacuna: el animal ya no figura. La brucelosis es de
> aplicación única en la vida, así que no vuelve a aparecer.

`[captura: m4-cu32-descorne]`
> Registro de un descorne. Es un procedimiento de aplicación única: el sistema no deja
> registrar un segundo descorne al mismo animal.

El descorne **no descuenta insumo** si el plan está configurado sin producto, que es el
caso previsto para los métodos que no consumen nada del stock.

### 7.4 Los planes sanitarios

Un plan sanitario es la regla que genera el calendario. Se configura una vez y el sistema
lo aplica solo a todos los animales que correspondan.

`[captura: m4-cu30-plan]`
> Configuración de un plan sanitario: qué procedimiento es, qué insumo aplica, cada
> cuántos días se repite, desde qué edad y a qué categorías alcanza.

| Campo | Qué define |
|---|---|
| Tipo de procedimiento | Vacunación, desparasitación o descorne |
| Insumo a aplicar | Qué se descuenta del stock. Puede quedar vacío |
| Periodicidad (días) | Cada cuánto se repite. Para los de aplicación única, no aplica |
| Edad de inicio (meses) | Desde qué edad el animal entra al plan |
| Categorías alcanzadas | A quiénes se aplica |
| Plan activo | Permite suspenderlo sin borrarlo |

**Si no se tilda ninguna categoría, el plan alcanza a todo el rodeo.** La ausencia de
categorías es lo que lo hace general — es el caso de la aftosa, que se le da a todos.

`[captura: m4-cu30-efecto]`
> El calendario después de crear el plan: aparecen como pendientes todos los animales que
> cumplen la edad y la categoría. No hubo que cargarlos uno por uno.

`[captura: m4-listas]`
> Listado de tratamientos, con las acciones de corregir y eliminar. Los cuatro listados
> del módulo —diagnósticos, tratamientos, vacunaciones y descornes— siguen este patrón.

**Al eliminar o corregir un evento sanitario que había consumido un insumo, el sistema
devuelve la cantidad al stock** mediante un movimiento inverso, y conserva el movimiento
original en el historial. Nada se borra: se compensa.

---

## 8. Los insumos

El módulo de insumos existe para que no falte lo que hace falta el día que hace falta, y
para que el consumo quede anotado sin tener que anotarlo.

### 8.1 Dar de alta un insumo

**Insumos → Insumos y stock → Agregar insumo**.

`[captura: m5-cu35-alta]`
> Alta de un insumo. El *período de carencia* es el que después determina hasta cuándo no
> se puede vender la leche de un animal tratado con este producto.

| Campo | Qué va |
|---|---|
| Nombre | Obligatorio |
| Tipo | Medicamento, vacuna, antiparasitario o pajuela |
| Toro que aporta la pajuela | Sólo en las pajuelas. **Obligatorio**: la pajuela vale por su genética |
| Cantidad de la partida | Con cuánto entra |
| Vencimiento de la partida | El sistema lleva el stock desagregado por partida |
| Stock mínimo | Por debajo de este número, el insumo aparece en las alertas |
| Período de carencia (días) | Los días que el producto sigue presente después del tratamiento |

Si el insumo ya está registrado, el sistema no lo duplica: indica que la reposición se
carga desde **Ingreso de Stock**.

### 8.2 Reponer stock

`[captura: m5-cu35-ingreso]`
> Ingreso de stock. Cada ingreso es una partida con su propio vencimiento, y el motivo
> queda anotado en el historial.

`[captura: m5-cu36-minimo]`
> Configuración del stock mínimo de un insumo. Es el umbral que dispara la alerta de
> stock crítico.

### 8.3 Las alertas

`[captura: m5-cu37-critico]`
> Alertas de stock crítico **antes de reponer**: los insumos que alcanzaron o bajaron de
> su mínimo configurado.

`[captura: m5-cu37-resuelto]`
> La misma pantalla **después del ingreso**: el insumo repuesto ya no figura. El par
> antes/después muestra para qué sirve la alerta.

`[captura: m5-cu38-vencimiento]`
> Alertas de vencimiento: las partidas que vencen dentro de la ventana configurada, para
> usarlas primero o darlas de baja antes de que venzan en el estante.

### 8.4 El historial de movimientos

`[captura: m5-cu39-movimientos]`
> Historial de movimientos de un insumo, filtrado por producto. Se ven el ingreso de la
> partida y los egresos automáticos por tratamiento e inseminación, cada uno con su
> motivo. **Es la trazabilidad del stock: ningún descuento aparece sin explicación.**

`[captura: m5-insumos]`
> Listado de insumos con su stock actual, su mínimo y su vencimiento más próximo.

---

## 9. Indicadores y decisiones

Los módulos anteriores registran. Éste **lee lo registrado y devuelve una lectura del
rodeo**. La única carga que se hace acá es el registro rápido del tablero, que se explica
enseguida.

`[captura: m6-cu40-tablero]`
> Tablero de inicio. Es la primera pantalla al entrar y reúne las tareas pendientes y los
> avisos vigentes de todos los módulos: qué animales necesitan atención hoy.

**El tablero también carga.** Arriba de los avisos está el **registro rápido** de los
cuatro eventos reproductivos, que son los que se anotan todos los días. Se elige el
evento —celo, servicio, tacto o parto—, la caravana y la fecha:

- **El celo y el tacto se guardan ahí mismo**, sin salir del tablero. El sistema aplica
  las mismas reglas que en sus pantallas propias y, al guardar, confirma el registro y
  deja el campo de caravana vacío y listo para el animal siguiente.
- **El servicio y el parto abren su formulario** con la caravana ya cargada, porque
  arrastran más: el servicio puede descontar una pajuela del stock y el parto da de alta
  la cría.

Debajo del campo aparecen **las caravanas que ya corresponden hoy** —*Para servir*,
*Para tactar* y *Por parir*—, y tocar una la completa. En el caso más frecuente, que es
cargarle el evento a un animal que ya está en una lista, no hay nada que escribir.

`[captura: m6-cu40-registro]`
> Registro rápido en el tablero, recién usado: el celo quedó registrado sin salir de la
> pantalla, y el campo de caravana espera el animal siguiente.

`[captura: m6-cu41-indicadores]`
> Indicadores del rodeo. Composición por estado productivo y reproductivo, litros
> promedio, días abiertos, intervalo entre partos, servicios por preñez, días en leche, y
> el ranking de las lactancias en curso con su proyección a 305 días.

Dos aclaraciones sobre estos números, porque son los que se miran para decidir:

- **La proyección a 305 días es lineal.** Sirve para comparar vacas que van por distinto
  momento de su lactancia, no para pronosticar cuánto va a dar una vaca. La curva real de
  lactancia no es una recta.
- **Los indicadores se mueven con lo que se carga.** Un secado saca una vaca del ordeñe;
  un parto suma otra. Si los números no cuadran, casi siempre falta cargar un evento.

`[captura: m6-cu42-descarte]`
> Candidatas a descarte, con el motivo por el que figura cada una. El sistema informa: la
> decisión de descartar es de la encargada.

Una hembra aparece acá si cumple **al menos uno** de estos cinco criterios:

| Criterio | Umbral |
|---|---|
| Produce por debajo del promedio del rodeo | menos del **70 %** |
| Servicios desde el último parto sin preñez confirmada | **3 o más** |
| Días abiertos | más de **150** |
| Diagnósticos sanitarios en el último año | **3 o más** |
| Partos acumulados | **7 o más** |

Son criterios fijos del sistema, no parámetros configurables.

`[captura: m6-cu43-buscar]`
> Buscador de caravana en la barra superior. Lleva directo a la ficha del animal desde
> cualquier pantalla, sin pasar por ningún listado.

---

## 10. Reportes y notificaciones

### 10.1 Los reportes

**Reportes y notificaciones → Productivo, Sanitario, Reproductivo o Genético.** Los cuatro
funcionan igual: se elige el período con **Desde** y **Hasta** —el genético no lleva
período, porque la genealogía no depende de fechas— y se presiona uno de tres botones.

| Botón | Qué hace |
|---|---|
| Ver en pantalla | Muestra el reporte en el propio sistema |
| Descargar PDF | Lo baja como documento, para imprimir o mandar |
| Descargar Excel | Lo baja como planilla, para trabajar los números |

`[captura: m7-reportes]`
> Reporte productivo en pantalla, con sus botones de descarga. **Lo que se ve es exactamente
> lo que sale en el PDF y en la planilla**: los tres reciben el mismo reporte ya armado.

| Reporte | Qué trae |
|---|---|
| Productivo | La producción general del establecimiento y la de cada vaca en ordeñe, con el detalle de los ordeñes y los controles del período |
| Sanitario | Diagnósticos, tratamientos, vacunaciones y descornes del período, y la situación sanitaria del rodeo al día de la emisión |
| Reproductivo | Servicios, tactos, partos y secados del período, con los indicadores reproductivos del rodeo |
| Genético | La genealogía del rodeo y el rendimiento comparado de las líneas paternas |

**Un período sin novedades también se reporta.** Si en las fechas elegidas no hubo, por
ejemplo, ningún diagnóstico, el reporte sale igual y esa sección dice *«Sin registros en
el período»*: deja constancia de que no los hubo. Lo único que el sistema rechaza es un
rango al revés, con la fecha *Desde* posterior a *Hasta*.

Los reportes son la forma de llevar la información fuera del sistema: el dueño los recibe
para las decisiones económicas y el veterinario para ajustar los planes.

### 10.2 Las notificaciones por Telegram

El sistema manda **todos los días, a la hora que se elija, un resumen de las tareas
pendientes** a un chat de Telegram. La encargada empieza la jornada sabiendo qué animales
necesitan atención sin haber abierto el sistema.

**Los avisos son los mismos que se ven en el tablero de inicio** y en las pantallas de
alerta: la notificación es el canal, no una fuente de información distinta. No llega un
mensaje por cada novedad: **todo viaja junto, en el resumen del día**.

**Reportes y notificaciones → Notificaciones** se usa en dos pasos.

**1. Vincular la cuenta.** Es lo único que no se hace desde el sistema, porque Telegram no
deja que un bot inicie una conversación: la persona tiene que escribirle primero.

1. En Telegram, buscar el bot del establecimiento y escribirle **/start**.
2. El bot contesta con un número: el **identificador de chat**.
3. Copiar ese número en **Identificador de chat** y confirmar la vinculación.

El sistema **manda un mensaje de prueba a ese chat antes de guardar nada**. Si llega, la
cuenta queda vinculada. Si no llega, avisa con el motivo que dio Telegram y **conserva la
vinculación anterior**, así que un número mal copiado no deja al establecimiento sin
avisos.

Si la pantalla avisa que *el sistema no tiene cargado el token del bot*, es un dato de la
instalación y no algo que se configure desde acá.

**2. Elegir qué se recibe y cuándo.** Recién con la cuenta vinculada aparecen:

- **La hora del resumen diario.** Lo habitual es antes del ordeñe de la mañana.
- **Los ocho tipos de aviso**, agrupados por módulo, cada uno con su casilla, y **Guardar**:

| Módulo | Avisos |
|---|---|
| Sanidad | Procedimientos sanitarios pendientes |
| Reproducción | Partos próximos, tactos pendientes, vacas para servir |
| Producción | Secados próximos, fin del descarte de leche |
| Insumos | Stock crítico, partidas por vencer |

**Apagar un aviso lo saca del mensaje y nada más**: se sigue viendo en el sistema igual que
siempre.

`[captura: m7-configuracion-bot]`
> Notificaciones con la cuenta vinculada: el chat, la hora del resumen y los ocho avisos
> agrupados por módulo, cada uno con su casilla.

**Cómo llega el resumen.** Un encabezado con la fecha y, debajo, los pendientes agrupados
por módulo: cada tipo de aviso con su cantidad y una línea por animal o por insumo. De cada
tipo se listan hasta **diez**; si hay más, el mensaje lo dice y el resto se ve en el
sistema.

`[captura: m7-resumen-telegram]`
> El resumen diario recibido en el celular. Es la única captura del manual que no sale del
> navegador.

Tres cosas que conviene saber:

- **Un día sin pendientes también recibe mensaje**, que dice *«No hay tareas pendientes»*.
  Así el silencio no se confunde con una falla del envío.
- **El resumen sale mientras el sistema esté funcionando.** Si estuvo apagado a la hora
  configurada, sale cuando vuelve a funcionar, en lugar de saltearse el día.
- **Escribiéndole /resumen al bot** se pide la lista en cualquier momento. Sólo le contesta
  al chat vinculado: el resto de la información del tambo está detrás del inicio de sesión.

---

## 11. Usar el sistema desde el celular

El sistema se construyó para usarse **en el tambo**, que es donde ocurren los eventos que
se registran: el celo que se detecta, el parto que se asiste, el ordeñe que se acaba de
medir. Funciona en el navegador del teléfono sin instalar nada, y su uso está verificado
a partir de los **375 píxeles** de ancho.

`[captura: mov-menu]`
> El menú plegado en pantalla angosta: las ocho secciones siguen a un toque de distancia,
> detrás del botón de menú.

`[captura: mov-lista-animales]`
> La lista de animales en el celular: las tablas reacomodan sus columnas para leerse sin
> desplazamiento horizontal.

`[captura: mov-ficha]`
> La ficha de un animal en pantalla angosta, con sus secciones apiladas.

`[captura: mov-ordenie]`
> Carga del ordeñe por lote desde el celular. **Es el caso de uso móvil real**: se anota
> en el momento, al lado del tanque.

`[captura: mov-celo]`
> Registro de un celo desde el celular. Igual que el ordeñe, se carga cuando se observa y
> no un rato después.

`[captura: mov-alertas]`
> Una pantalla de alertas en el celular, para revisarla mientras se recorre el rodeo.

`[captura: mov-linaje]`
> El árbol genealógico en pantalla angosta, con los controles de acercar, alejar y
> recentrar.

**Un consejo de uso.** Lo que se carga en el momento es lo que queda bien cargado. El
sistema está pensado para que anotar un evento lleve menos tiempo que apuntarlo en el
cuaderno para pasarlo después — y ése es, al final, el punto de todo esto.
