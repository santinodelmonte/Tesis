# Guion de la demo para el tutor

Reunion del **20/08/2026**. El tutor vio un avance del Modulo 1 en construccion: esta
demo tiene que mostrarle el sistema entero y, sobre todo, que las reglas del negocio
estan adentro del sistema y no en la cabeza del que carga.

Duracion objetivo: **50 a 60 minutos** de recorrido, mas preguntas. Si el tiempo se
achica, los tramos marcados **[nucleo]** son los que no se sacan; los marcados
**[opcional]** se cuentan en vez de mostrarse.

El hilo conductor es uno solo, y conviene decirlo al empezar y repetirlo al cerrar:

> Cada dato que se carga tiene consecuencias en los otros modulos. El sistema no es
> cinco ABM juntos: un tratamiento saca a la vaca del tanque de leche, un parto abre
> una lactancia y da de alta un animal, una inseminacion descuenta una pajuela del
> stock.

## Sobre las fechas de este guion

`bd/DatosPrueba.sql` ancla todo el rodeo al **dia en que se corre**, asi que no hay
nada que corregir a mano ni fechas que se venzan. Donde el guion dice **hoy**, hay
que escribir el dia de la reunion; entre parentesis va la fecha absoluta suponiendo
que la demo sea el **20/08/2026**.

| El rodeo trae | Cuando cae | Si la demo es el 20/08 |
|---|---|---|
| Ultimo ordeñe cargado | anteayer | 18/08/2026 |
| Descarte de leche de la `115` | vence en 6 dias | 26/08/2026 |
| Parto probable de la `136` | dentro de 7 dias | 27/08/2026 |
| Parto probable de la `140` | dentro de 30 dias | 19/09/2026 |
| Servicio sin tactar de la `102` | hace 52 dias | 29/06/2026 |
| Proxima campaña de aftosa | dentro de 25 dias | 14/09/2026 |

---

## 0. Puesta a punto (20 minutos antes, no delante del tutor)

### 0.1 El motor de base de datos

El sistema se conecta a MySQL/MariaDB en `localhost:3306` con el conector
`MySql.Data`. El motor de XAMPP ya esta instalado y escuchando en el 3306, con los
valores por defecto que espera `appsettings.json` (`localhost`, `root`, sin
contraseña).

> **Arrancarlo desde el panel de XAMPP antes que nada.** Con el motor apagado la
> pantalla de login **igual funciona** —valida contra `appsettings.json` y no contra la
> base— asi que entrar bien no prueba que la base este arriba. Lo que falla es la
> primera pantalla que pida datos, y para entonces el tutor ya esta sentado.

Comprobacion rapida de que el motor esta arriba:

```bash
mysqladmin -u root ping
```

### 0.2 Crear la base y cargar el rodeo

La base esta vacia, asi que van los dos scripts, en este orden:

```bash
mysql -u root < bd/CreacionDb.sql
```

```bash
mysql -u root < bd/DatosPrueba.sql
```

El primero crea la base `tambo` con las 22 tablas y los datos semilla (razas,
categorias y la fila de configuracion). Empieza con un `DROP DATABASE`, asi que deja
siempre el mismo esquema por mas veces que se corra.

El segundo carga el rodeo: 21 animales propios, 3 toros de catalogo, una semana de
ordeñes, dos controles lecheros, la reproduccion en marcha y el stock con sus
alertas. **Todas sus fechas se calculan contra `@hoy = CURDATE()`**, de manera que
corriendolo hoy el rodeo queda parado en hoy. Se puede volver a correr cuantas veces
haga falta: empieza vaciando las tablas de datos y no toca razas, categorias ni
configuracion.

**Conviene correrlo el mismo dia de la reunion.** Corrido la vispera, el rodeo queda
anclado a ayer: todo sigue funcionando -no hay nada que se venza en un dia- pero las
fechas absolutas de este guion bajan un dia y el ultimo ordeñe pasa a ser el de hace
tres dias.

Las dos ultimas consultas del script son la verificacion: **tienen que devolver cero
filas**. La primera comprueba que el stock declarado de cada insumo coincida con el
saldo de sus movimientos; la segunda, que los controles individuales del dia del
control lechero sumen exactamente el total del lote de ese dia. Si alguna devuelve
algo, el juego de datos quedo incoherente y conviene volver a correrlo.

### 0.3 Levantar el sistema

Abrir la solucion en Visual Studio y correr con F5. La ultima verificacion de
`dotnet build` dio **0 errores** (hay advertencias de nulabilidad, anotadas en
`docs/pendientes-tecnicos.md`).

### 0.4 Ultimo chequeo antes de que entre

- Sesion **cerrada**: la demo arranca por el login.
- Una sola pestaña abierta, sin la barra de favoritos llena.
- Zoom del navegador en 100% o 110%, para que la tabla del rodeo entre completa.
- Tener una **foto de una vaca** a mano, para el campo de foto del tramo 3.
- Este archivo en el celular o impreso, no en la misma pantalla.

### 0.5 El tablero tiene que verse asi

Despues del login, `Hoy en el tambo`:

| Indicador | Valor esperado |
|---|---|
| Vacas para servir | 4 |
| Tactos pendientes | 1 |
| Partos proximos | 1 |
| Secados proximos | 0 |
| Animales con descarte de leche vigente | 1 |
| Insumos en stock critico | 2 |
| Partidas por vencer | 2 |
| Procedimientos sanitarios pendientes | Muchos: ver la nota del tramo 6.3 |
| Activos / en ordeñe / secas / preñadas / vacias | 23 / 8 / 2 / 6 / 7 |

Si algun numero no da, lo mas probable es que el script de datos no haya corrido
completo. Volver a correrlo tarda segundos.

---

## 1. Apertura: que es esto [nucleo] — 3 min

Sin tocar el sistema todavia. Tres cosas, cortas:

**Que resuelve.** Un tambo lleva la informacion en cuadernos y planillas sueltas: el
ordeñe en una, la sanidad en otra, la reproduccion en la libreta del inseminador.
Nadie cruza las tres, y las decisiones que importan —a que vaca servir, cual secar,
cual descartar— salen justamente del cruce.

**Como esta hecho.** Tres capas, sin ORM y con SQL a la vista:

- `Tesis/Dominio` — las clases del negocio y la `Controladora`, que es donde viven
  **todas** las reglas. Ninguna pantalla decide nada.
- `Tesis/Persistencia` — una clase `p<Entidad>` por entidad, `pConexion` para el
  acceso a MySQL y `pControladora` como fachada.
- `Tesis/Pages` — Razor Pages. Cada pantalla lee el formulario, le pregunta a la
  Controladora y muestra lo que responde.

**El tamaño.** 22 tablas, 49 casos de uso, 7 secciones de menu, 6 modulos
funcionales. La documentacion (`Proyecto_v6.docx`) describe el sistema que existe:
los diagramas se regeneran desde scripts, no se dibujan a mano.

> Si pregunta por que MySQL y no SQL Server, o por que `pConexion` y no `Conexion`:
> la regla fue **manda el Proyecto**. Cuando el informe de estilo y el Proyecto se
> contradicen, se respeta el Proyecto y se anota.

---

## 2. Login y tablero [nucleo] — 3 min

**Pantalla.** La que aparece sola al entrar.

| Campo | Dato |
|---|---|
| Usuario | `sofia` |
| Contraseña | `tambo2026` |

**Mostrar antes de entrar bien:** contraseña `tambo2025` → *"Usuario o contraseña
incorrectos!"*.

**Mostrar despues:** cerrar sesion y pegar en la barra del navegador
`https://localhost:7283/PagesAnimal/ListaAnimales` → vuelve al login. Todo el sitio
esta detras de `AuthorizeFolder("/")`; solo el login y la pantalla de error son
anonimos.

**Que decir del tablero.** No calcula nada propio: junta las mismas listas de trabajo
que despues se ven una por una en cada modulo. Es la pantalla que contesta "que hay
que hacer hoy" sin abrir seis menus.

---

## 3. Animales: el rodeo [nucleo] — 8 min

### 3.1 Alta con categoria calculada

**Menu: Animales → Rodeo → Agregar animal.**

| Campo | Dato |
|---|---|
| Numero de caravana | `200` |
| Fecha de nacimiento | `2021-06-10` |
| Sexo | Hembra |
| Raza | Holando |
| Madre / Padre | vacios (viene de otro establecimiento) |
| Partos registrados | `2` |
| Categoria | dejar en *"La calcula el sistema"* |

Apretar **Calcular categoria** antes de guardar: propone **Vaca**, porque tiene
partos. Decir el corte completo, que es una de las decisiones de negocio del
proyecto: ternera/ternero hasta 12 meses; novilla si pasa los 12 y no pario; vaca al
primer parto; toro el macho de mas de 15 meses destinado a la reproduccion —el que
esta en pie o el de catalogo que aporta pajuelas—; novillo el resto de los machos de
mas de 12 meses. Los machos se clasifican por edad **y destino reproductivo**: es el
desvio de RF1.8 que quedo anotado en `docs/cambios-anteproyecto-v6.md`.

Guardar con **Agregar**.

### 3.2 Las tres validaciones que bloquean

Volver a **Agregar animal** y mostrarlas una atras de la otra, sin guardar:

| Que se carga | Mensaje |
|---|---|
| Caravana `200` de nuevo | *"El número de caravana ya existe en el sistema!"* |
| Fecha de nacimiento `2027-01-01` | *"La fecha de nacimiento no puede ser futura!"* |
| Caravana vacia o sin raza | *"El número de caravana y la raza son obligatorios!"* |

### 3.3 Genealogia: bloqueo y advertencia [nucleo]

Aca conviene detenerse, porque muestra que el sistema distingue **lo imposible** de
**lo sospechoso**. Son dos metodos distintos en la Controladora: `ValidarGenealogia`
devuelve el motivo por el que no se puede, y `AdvertenciasGenealogia` devuelve lo que
hay que mirar antes de confirmar.

**Caso que bloquea.** Alta de la caravana `201`, nacida hace un mes y medio (por
ejemplo `2026-07-01`), hembra, Holando, con **madre** `177` —la ternera que nacio
hace cuatro meses—:

> *"La madre tiene que haber nacido al menos 22 meses antes que la cria!"*

Los 22 meses no son un numero magico: son la edad minima al servicio configurada (13
meses) mas los 9 de gestacion. **No hay boton para forzarlo.** Una madre mas joven que
su propia cria es un dato imposible, no un dato raro.

**Caso que advierte y deja seguir.** En la misma alta, cambiar la madre por `152` y
poner **padre** `7HO12165`:

> *"Los progenitores tienen parentesco entre si: ... La cria nace consanguinea."*

Aparece el boton **Guardar de todos modos**. Explicar por que: en un tambo real la
consanguinidad pasa, y trabarla de entrada haria imposible cargar el rodeo inicial,
donde muchos animales tienen genealogia incompleta o incomoda. La decision es del
usuario; la responsabilidad del sistema es que no la tome sin verla.

*(No hace falta guardar `201`. Alcanza con mostrar el cartel.)*

### 3.4 Linaje y consanguinidad

**Menu: Animales → Linaje.** Elegir `152`: arma el arbol con madre `101` y padre
`7HO12165`.

**Menu: Animales → Consanguinidad.** Hembra `152`, reproductor `7HO12165` → avisa el
parentesco y **nombra el ancestro comun**. Repetir con `29HO18296` → sin parentesco.
Elegir el mismo animal en los dos campos → *"No puede verificar un animal contra sí
mismo!"*.

Decir que esta misma funcion corre sola en el alta de animales y en el registro del
servicio: la pantalla suelta existe para consultarla antes de decidir una pajuela.

### 3.5 Ficha del animal [nucleo]

**Menu: Animales → Rodeo**, entrar al detalle de `115`.

Es la pantalla que resume todo el sistema en un animal: datos, foto, diagnostico de
mastitis en tratamiento, **descarte de leche vigente por seis dias mas** (26/08),
partos, servicios, produccion de la lactancia en curso y proyeccion a 305 dias.

Tambien se llega escribiendo la caravana en el buscador de la barra superior. Vale la
pena mostrarlo: es el atajo del que esta en el campo.

### 3.6 Buscar y filtrar [opcional]

**Menu: Animales → Buscar y filtrar.** Categoria *Vaca*, estado productivo *En
lactancia* → salen las ocho vacas en ordeñe. La `112`, dada de baja hace cinco meses,
no aparece. Filtrar edad desde `5` y hasta `2` → *"El rango etario es incorrecto..."*.

---

## 4. Produccion [nucleo] — 8 min

### 4.1 Ordeñe por lote, y el cruce con Sanidad [nucleo]

**Menu: Produccion → Ordeñe por lote.**

| Campo | Dato |
|---|---|
| Fecha | **hoy** (20/08/2026) |
| Turno | Turno 1 |
| Litros del ordeñe | `88.10` |
| Animales del lote | las que vienen tildadas |

**Este es el primer momento fuerte de la demo.** Antes de guardar, hacer notar que la
vaca `115` **no viene tildada y no se puede sumar**: tiene descarte de leche vigente
por el tratamiento contra la mastitis. Nadie tuvo que acordarse: el sistema la saco
del lote porque en Sanidad se cargo un producto con carencia.

Guardar. (El ultimo ordeñe que trae el juego de datos es el de anteayer, asi que no
hay choque de fecha y turno.)

**Validaciones, repitiendo la carga:**

| Que se carga | Que pasa |
|---|---|
| Misma fecha y Turno 1 otra vez | Avisa que ya hay un ordeñe cargado para esa fecha y turno |
| Fecha del año que viene | *"La fecha del ordeñe no puede ser futura!"* |
| Destildar todos los animales | *"El lote tiene que tener al menos un animal!"* |
| Litros `0` o negativos | *"Los litros tienen que ser un valor positivo y coherente!"* |

### 4.2 Control lechero: la carga masiva

**Menu: Produccion → Control lechero.** Fecha **hoy**, Turno 1.

| Caravana | Litros |
|---|---|
| `101` | `11.00` |
| `102` | `8.20` |
| `108` | `16.00` |
| `121` | `13.40` |
| `124` | `15.10` |
| `130` | `11.50` |
| `133` | `12.90` |

**Un detalle que conviene adelantar.** En esta lista aparecen **ocho** vacas: la
`115` tambien esta, aunque hace un minuto el ordeñe por lote no la dejaba sumar al
lote. No es una incoherencia, es la distincion entre las dos pantallas: la vaca con
descarte **se ordeña igual** -hay que vaciarle la ubre o se enferma-, lo que no se
hace es mandar esa leche al tanque. Su produccion es real y pertenece a su lactancia,
asi que medirla es correcto; tirarla al tanque no. Dejarla en blanco y seguir.

Los siete suman **88.10**, exactamente los litros que se cargaron en el lote: se
midio todo el lote y no quedo ninguna vaca afuera. Vale la pena hacerlo notar, porque
es la relacion que el sistema espera y la que comprueba la consulta de verificacion
del script de datos. Si la suma diera **menos**, significaria que alguna vaca del lote
no se midio -normal en un tambo grande, y el sistema lo acepta-; si diera **mas**, seria
medir por animal mas leche de la que salio del tanque, y ahi el ordeñe por lote lo
rechaza con el total ya medido en el mensaje.

Guardar. Siete controles en una sola pasada, cada uno imputado a la lactancia en
curso de su vaca. Decir que asi es como se hace en el tambo: el control lechero es
una jornada, no siete formularios.

Los controles del Turno 1 quedan colgados del ordeñe de lote que se cargo recien: son
el mismo ordeñe anotado vaca por vaca, no leche aparte. Eso es lo que despues evita
contar la produccion dos veces.

### 4.3 Carga de una sola vaca, y el tope configurado

Desde adentro del control lechero, **Cargar una sola vaca**.

| Campo | Dato |
|---|---|
| Fecha | **hoy** |
| Turno | Turno 2 |
| Caravana | `130` |
| Litros | `8.90` |

Guardar. Despues, los tres rechazos:

| Que se carga | Mensaje |
|---|---|
| Otra vez `130`, misma fecha y turno | Avisa que ya hay un control cargado, **y dice con cuantos litros** |
| Caravana `136` (vaca seca) | *"El animal no se encuentra en lactancia: su estado productivo es Seca."* |
| Litros `250` | Supera el maximo por control configurado (100 litros) y lo rechaza **con el tope en el mensaje** |

El detalle a subrayar: los mensajes traen el dato que hace falta para corregir. No
dicen "error de validacion".

### 4.4 Historial y metrica mensual [opcional]

**Produccion → Historial de produccion**, rango del `2026-08-01` a **hoy**, por lote:
tienen que salir los catorce ordeñes del juego de datos mas el que se cargo recien.
Rango invertido → *"El rango de fechas es invalido..."*.

**Produccion → Metrica mensual**, Agosto 2026: total del mes, promedio por ordeñe y
promedio por vaca. Decir que la leche del dia del control lechero **no se cuenta dos
veces**, aunque ese dia haya ordeñe de lote y controles individuales: el control
apunta al lote y sus litros ya estan adentro del total.

---

## 5. Reproduccion [nucleo] — 12 min

Es el modulo que mas encadena. Recorrerlo en el orden del ciclo: celo → servicio →
tacto → parto.

### 5.1 Celo, y la validacion por edad

**Menu: Reproduccion → Celos → Registrar celo.**

| Campo | Dato |
|---|---|
| Caravana | `115` |
| Fecha de deteccion | **hoy** |
| Observaciones | `Celo firme, monta a otras vacas del lote.` |

Guardar. Despues, los tres rechazos:

| Que se carga | Mensaje |
|---|---|
| Caravana `177` (ternera de 4 meses) | *"El animal tenia 4 meses en esa fecha: la hembra empieza a manifestar celo a partir de los 9 meses. Revise la caravana o la fecha!"* |
| Caravana `T-01` (el toro) | *"La caravana corresponde a un macho: no se puede registrar un celo!"* |
| Caravana `112` (dada de baja hace cinco meses) | *"El animal figura dado de baja el ...: no se le puede registrar un celo posterior a esa fecha!"* |

El primero es el que vale la pena comentar: el mensaje **calcula la edad que el
animal tenia ese dia** y la pone en el texto. La regla de los 9 meses es biologia y
por eso es constante; la edad minima al servicio, en cambio, la decide cada tambo y
esta en Configuracion.

### 5.2 Servicio: descuento de stock y consanguinidad [nucleo]

**Menu: Reproduccion → Vacas para servir.** Estan `115`, `130`, `133` y la vaquillona
`158`, cada una **con el motivo por el que aparece** ("pario hace 127 dias",
"vaquillona de 18 meses, sin servicios registrados"). Entrar a **Registrar servicio**.

**Inseminacion artificial:**

| Campo | Dato |
|---|---|
| Caravana | `115` |
| Fecha del servicio | **hoy** |
| Tipo de servicio | Inseminación artificial |
| Pajuela | `Pajuela Holando 7HO12165` |
| Fecha probable de parto | apretar **Recalcular** → hoy + 283 dias (30/05/2027) |
| Observaciones | `Repite servicio, el tacto anterior dio vacia.` |

Guardar y mostrar **las dos consecuencias**, que es lo que hace que esto no sea un ABM:

1. `115` pasa a **Servida** y sale de *Vacas para servir*.
2. **Insumos → Movimientos**: la pajuela bajo de 17 a 16, con un egreso cuyo motivo
   dice *"Inseminación de la caravana 115"*. El stock del tambo se movio solo.

**Advertencia de consanguinidad.** Volver a **Registrar servicio** con la vaquillona
`158` y la pajuela **`Pajuela Jersey 7JE01722`**: `7JE01722` es el padre de `158`.

> *"La hembra y el reproductor tienen parentesco: ... La cria nace consanguinea."*

No guarda; ofrece **Registrar de todos modos**. Misma logica que en el alta de
animales, aplicada en el momento en que la decision todavia se puede cambiar.
**Mostrar el cartel y no confirmar**: si se guarda, `158` queda servida y despues no
sirve para la monta natural del paso siguiente.

**Bloqueos para mostrar:**

| Que se carga | Mensaje |
|---|---|
| Servicio a `152` (que figura preñada) | *"El animal figura prenado: servirlo le provoca el aborto. Si la preniez estaba mal confirmada, registre primero un tacto con resultado vacia!"* |
| Servicio a la ternera `177` | *"El animal tenia 4 meses en esa fecha: la edad minima para entrar en servicio es de 13 meses..."* |
| Monta natural sin elegir toro | *"La monta natural necesita un toro del rodeo!"* |
| Toro **y** pajuela a la vez | *"El servicio tiene un unico reproductor: el toro del rodeo y la pajuela son excluyentes!"* |

Al cambiar el tipo de servicio, el selector de pajuela se esconde y aparece el de
toro: son excluyentes tambien en la pantalla, no solo en la regla.

**Monta natural que si se guarda** [opcional]: `158` con el toro `T-01`, fecha
**hoy**.

### 5.3 Tacto

**Menu: Reproduccion → Tactos pendientes.** Esta `102`: fue servida por monta natural
hace 52 dias, mas de los 35 configurados. Es la lista con la que se arma la visita del
veterinario.

Entrar a **Registrar tacto**:

| Campo | Dato |
|---|---|
| Caravana | `102` |
| Fecha del tacto | **hoy** |
| Resultado | Preñada |
| Observaciones | `Preñez confirmada. Servicio del toro T-01.` |

Antes de guardar, apretar **Ver servicio**: muestra el servicio de hace 52 dias con el
toro `T-01` y el parto proyectado para dentro de 231 dias (08/04/2027). El tacto **no**
cuelga de la vaca: cuelga del servicio, que es lo que viene a confirmar.

**Esperado.** `102` pasa a **Preñada** y **sigue En lactancia**: el tacto toca el eje
reproductivo y no el productivo. Sale de tactos pendientes.

### 5.4 Parto de mellizos [nucleo] — el tramo que mas conviene mostrar

**Menu: Reproduccion → Partos proximos.** Aparece `136`, que pare dentro de 7 dias
(27/08). Pare una semana antes, que es lo que pasa la mitad de las veces. Entrar a
**Registrar parto**.

**Madre:**

| Campo | Dato |
|---|---|
| Caravana | `136` — apretar **Cargar datos** |
| Fecha del parto | **hoy** |
| Tipo de parto | Normal |
| Observaciones | `Parto sin asistencia. Dos crias.` |

Al cargar los datos aparece el cartel con el servicio de hace 276 dias (17/11/2025) y
el parto proyectado para dentro de 7 dias: 276 dias de gestacion, dentro del rango
normal de 240 a 320, asi que no advierte nada.

**Primera cria:**

| Campo | Dato |
|---|---|
| Numero de caravana | `180` |
| Sexo | Hembra |
| Raza | Holando |
| Padre | ya viene propuesto: `7HO12165`, el toro de la pajuela del servicio |

**Tildar "Parto doble (mellizos)"** — se despliega el bloque de la segunda cria:

| Campo | Dato |
|---|---|
| Numero de caravana | `181` |
| Sexo | Macho |
| Raza | Holando |

Antes de confirmar, mostrar **dos bloqueos** poniendo mal la segunda cria:

- Caravana `180` en las dos → *"Las dos crias no pueden llevar la misma caravana!"*
- Caravana `177` (que ya existe) → *"El numero de caravana de la segunda cria ya
  existe en el sistema!"*

Corregir a `181` y apretar **Confirmar parto**. Sale la advertencia que hay que leer
en voz alta:

> *"Mellizos de distinto sexo: la cria hembra nace freemartin y lo mas probable es
> que sea esteril. Conviene no destinarla a reposicion."*

**Por que esta esa advertencia.** Cuando nacen mellizos de distinto sexo, los dos
comparten circulacion sanguinea durante la gestacion y la hembra queda esteril en la
enorme mayoria de los casos. Sin el aviso, el tambo la cria dos años como futura vaca
lechera y recien se entera cuando no prende con ningun servicio. Es exactamente el
tipo de conocimiento que el sistema tiene que aportar y que la planilla no aporta.

Apretar **Confirmar de todos modos**.

**Que verificar despues, en la ficha de `136` y en el rodeo:**

- `136` queda **En lactancia** y **Vacia**, con **un** parto mas (no dos).
- Se abrio **una** lactancia, la numero 2, con fecha de hoy.
- Se dieron de alta **dos** animales: `180` Ternera y `181` Ternero, nacidos hoy, con
  madre `136` y padre `7HO12165`.
- El linaje de `180` y `181` se armo solo.
- `136` salio de *Partos proximos* y entra al lote de ordeñe.

Ese "suma un parto y una lactancia, pero da de alta dos animales" es el detalle que
conviene decir explicito: es la razon por la que `AltaParto` recibe una **lista** de
crias y no una cria. Alrededor del cuatro por ciento de los partos Holando son dobles.

**Otras dos advertencias del parto**, para nombrarlas aunque no se muestren:

- Parto de una vaca que figura **Vacia** → avisa que falta registrar el servicio o el
  tacto, pero deja guardar: el ternero esta ahi, lo que falta es el historial.
- Gestacion de menos de 240 o mas de 320 dias contra el servicio → avisa que revise
  las fechas. Se muestra rapido poniendo la fecha del parto tres meses atras, **sin
  guardar**.

---

## 6. Sanidad, y el circuito que se cierra [nucleo] — 8 min

### 6.1 Diagnostico

**Menu: Sanidad → Diagnosticos → Registrar diagnostico.**

| Campo | Dato |
|---|---|
| Caravana | `108` |
| Fecha | **hoy** |
| Estado | En tratamiento |
| Enfermedad | `Dermatitis digital (cojera), miembro posterior derecho` |

### 6.2 Tratamiento: descarte de leche y descuento de stock [nucleo]

**Menu: Sanidad → Tratamientos → Registrar tratamiento.**

| Campo | Dato |
|---|---|
| Diagnostico que lo origina | el de `108` de hoy |
| Producto aplicado | `Oxitetraciclina 20% LA (frasco 100 ml)` |
| Fecha de inicio | **hoy** |
| Duracion (dias) | `3` |
| Dosis diaria | `20 ml intramuscular cada 24 h` |
| Unidades a descontar | `2` |
| Plan sanitario que cumple | Fuera de plan |
| Descarte de leche hasta | apretar **Calcular** → hoy + 10 dias (30/08/2026) |

Explicar la cuenta mientras se aprieta el boton: inicio + 3 dias de tratamiento + 7
dias de carencia del producto. La carencia no la escribe el usuario: sale del insumo.

Guardar, y **cerrar el circuito delante del tutor**:

1. **Insumos → Insumos y stock**: la oxitetraciclina bajo de 8 a 6.
2. **Insumos → Movimientos**: el egreso quedo anotado con su motivo.
3. **Produccion → Ordeñe por lote**, fecha **hoy**, Turno 2: ahora **`108` tampoco
   aparece disponible**, igual que `115`.

Esa tercera pantalla es el cierre del argumento de toda la demo: se cargo algo en
Sanidad y la leche del tanque cambio sola.

**Validaciones:**

| Que se carga | Mensaje |
|---|---|
| Unidades `50` de un producto con 6 | *"No hay stock suficiente del producto: quedan 6,00 unidades."* |
| Duracion `0` | *"La duracion del tratamiento tiene que ser de al menos un dia!"* |
| Sin diagnostico y sin caravana | *"Seleccione el diagnostico a tratar, o la caravana del animal si el tratamiento es preventivo!"* |

**Variante preventiva** [opcional]: dejar *"Sin diagnostico (tratamiento
preventivo)"*, caravana `158`, producto `Ivermectina 1% (frasco 500 ml)`, plan
`Desparasitacion`. Lo acepta —el preventivo no necesita diagnostico— y **igual bloquea
la leche**, porque el tratamiento apunta al animal directamente.

### 6.3 Vacunacion y calendario

**Menu: Pendientes y alertas → Calendario sanitario.**

Dos avisos para no quedar descolocado, los dos mejor dichos antes de que los pregunte:

- La lista es **larga**. El plan de aftosa es semestral y la proxima campaña cae
  dentro de 25 dias, adentro de los 30 de anticipacion configurados: por eso figura
  **todo el rodeo**. Filtrando por plan se ve limpio.
- Los tres **toros de catalogo** (`7HO12165`, `7JE01722`, `29HO18296`) **no** estan
  en la lista, y es a proposito: no estan en el campo, no hay a quien vacunar. El
  sistema los distingue por no estar en pie y aportar pajuelas al stock. Si pregunta
  como los separa de los terneros machos y del novillo, que tampoco estan en pie: esos
  no aportan pajuelas, asi que siguen entrando en los planes —de hecho `178` figura
  pendiente de descorne unas lineas mas abajo—.

Los dos pendientes individuales que interesan: `177` pendiente de brucelosis y `178`
pendiente de descorne.

**Sanidad → Vacunaciones → Registrar vacunacion:**

| Campo | Dato |
|---|---|
| Caravana | `177` |
| Vacuna aplicada | `Vacuna Brucelosis cepa 19 (dosis)` |
| Fecha de aplicacion | **hoy** |
| Plan sanitario que cumple | `Brucelosis cepa 19` |

**Esperado.** El stock de la vacuna baja de 8 a 7 y `177` **sale del calendario para
siempre**: la brucelosis tiene periodicidad nula, o sea aplicacion unica en la vida.

### 6.4 Descorne y planes [opcional]

**Sanidad → Descornes → Registrar descorne**: caravana `178`, fecha **hoy**, metodo
*Pasta caustica*, plan `Descorne`. Sale del calendario y **no descuenta insumo**: el
plan de descorne esta configurado sin producto, que es un curso alternativo previsto
del caso de uso.

**Sanidad → Planes sanitarios → Agregar plan**: nombre `Vacuna clostridial`, tipo
*Vacunacion*, cualquier insumo de tipo Vacuna, periodicidad `365`, edad de inicio
`4`, categorias *Ternera* y *Ternero*, activo. En el calendario aparecen enseguida
las crias de mas de 4 meses.

El detalle que vale la pena: si el plan se deja **sin ninguna categoria tildada**,
alcanza a todo el rodeo. La ausencia de categorias es informacion, no un dato
faltante — es asi como esta cargado el plan de aftosa.

---

## 7. Insumos y stock [opcional] — 4 min

Este modulo ya se mostro de costado tres veces (pajuela, oxitetraciclina, vacuna).
Alcanza con recorrerlo rapido para mostrar que tambien se maneja solo.

- **Insumos → Insumos y stock → Ingreso de stock**: `Ivermectina 1% (frasco 500 ml)`,
  cantidad `10`, fecha **hoy**, vencimiento `2027-10-31`, motivo `Compra a
  veterinaria La Rural`. Pasa de 3 a 13 y **desaparece de Stock critico**.
- **Configurar stock minimo**: `Pajuela Holando 29HO18296`, minimo `2`. Queda en 3
  sobre 2 y sale de la alerta. Con eso, *Stock critico* queda vacio.
- **Pendientes y alertas → Partidas por vencer**: la partida de antiaftosa (vence
  dentro de 20 dias) y la de ivermectina (dentro de 22). El vencimiento vive en el
  **movimiento**, no en el insumo, porque el mismo producto entra en partidas
  distintas.
- **Insumos → Movimientos**, filtrando por oxitetraciclina: el ingreso de la compra y
  todos los egresos por tratamiento, incluido el que se cargo recien.

Validaciones: cantidad `0` → *"La cantidad tiene que ser mayor a cero!"*; fecha futura
→ la rechaza; alta de insumo sin nombre → la rechaza.

---

## 8. Indicadores y configuracion [nucleo] — 5 min

### 8.1 Indicadores

**Menu: Indicadores → Indicadores del rodeo.** Composicion por categoria, promedio de
litros, intervalo entre partos, porcentaje de preñez. Hacer notar que los numeros se
movieron respecto del arranque de la demo: hay una vaca mas en ordeñe por el parto de
`136` y dos animales nuevos en el rodeo.

**Menu: Indicadores → Candidatas a descarte.** El sistema **no decide**: junta lo que
ya sabe de cada vaca y muestra los motivos, ordenados por cantidad de motivos. Los
criterios son cinco: produce menos del 70% del promedio del rodeo, tres o mas
servicios sin preñez, mas de 150 dias abiertos, tres o mas diagnosticos en el año, o
siete partos o mas. `133` es la candidata tipica del juego de datos.

### 8.2 Configuracion: la regla que el tambo decide

**Menu: Configuracion** (abajo de todo, en el menu lateral). Aca esta el argumento de
por que hay una tabla de configuracion y no constantes: lo que cada tambo resuelve a
su manera es parametro; lo que es biologia es constante.

- **Parametro:** dias de secado antes del parto, edad minima al servicio, edad de
  cambio de categoria, litros maximos por control, ordeñes por dia, dias de espera
  voluntaria, dias para el tacto, y las cuatro anticipaciones de alerta.
- **Constante en el codigo:** los 283 dias de gestacion, los 9 meses de pubertad, el
  rango de 240 a 320 dias de una preñez viable, los 305 dias de lactancia estandar.

**Demostracion en vivo, la mas visible:** cambiar **Parto proximo (dias)** de `15` a
`45` y guardar. Volver a **Pendientes y alertas → Partos proximos**: ahora tambien
aparece `140`, que pare dentro de 30 dias. La ventana se amplio y la lista de trabajo
cambio.

**Si quiere ver la alerta de secado**, que hoy esta vacia: subir **Dias de
anticipacion al secado** de `15` a `120` y entrar a *Secados proximos* → aparece
`124`, que se seca dentro de 93 dias. Explicar por que estaba vacia: las vacas en
ordeñe con preñez confirmada paren recien dentro de cinco a siete meses, asi que
ninguna entra en la ventana de 60 + 15 dias. **La lista vacia es correcta, no es una
pantalla sin terminar.**

Tambien valida sus propios valores: cargar edad minima al servicio `3` → lo rechaza
con el motivo.

**Al terminar, dejar los valores como estaban** (`15` de parto, `15` de secado, `100`
litros, `35` dias para el tacto).

---

## 9. Correccion y baja: lo que muestra la integridad [nucleo] — 5 min

Suele ser el tramo que mas impresiona, porque es lo que un ABM generado no tiene. La
decision de diseño fue: **borrado fisico con reversion de efectos**, y **bloqueo con
explicacion** cuando hay registros colgando.

### 9.1 Un borrado que se bloquea y dice como avanzar

**Menu: Reproduccion → Servicios**, eliminar el servicio de la `102` que se acaba de
tactar:

> *"El servicio tiene 1 tacto(s) registrado(s): elimine primero esos tactos desde
> Reproduccion, Tactos!"*

El mensaje no dice "no se puede": dice **donde ir**. Esa fue la regla para todos los
borrados.

**Menu: Reproduccion → Partos**, intentar eliminar el parto de `136` que se cargo
recien. Si la lactancia nueva ya tiene controles de ordeñe:

> *"La lactancia que abrio este parto tiene N control(es) de ordeñe cargado(s):
> deshacer el parto borraria esa produccion. Eliminelos primero desde el historial de
> produccion!"*

Y si se intenta borrar un parto que no es el ultimo de la vaca, avisa cual es el
ultimo: deshacer uno del medio dejaria las lactancias numeradas con un salto.

### 9.2 Un borrado que si se hace, y devuelve lo que consumio

**Menu: Reproduccion → Servicios**, eliminar el servicio de `115` de hoy (todavia sin
tacto). Se borra, y ademas:

- `115` vuelve al estado que le dan los servicios que le quedan: **Vacia**.
- La pajuela **vuelve al stock** con un contra-movimiento cuyo motivo dice
  *"Devolucion por eliminacion del servicio del ..."*. El egreso original no se borra:
  el historial de movimientos es un libro, no una foto.

*(Si se prefiere no deshacer el servicio, el mismo efecto se ve eliminando un control
individual desde el historial de produccion: la confirmacion nombra caravana, fecha,
turno y litros antes de borrar.)*

### 9.3 Baja logica del animal

**Menu: Animales → Rodeo**, entrar a `160` (el novillo) y **Registrar baja**.

| Campo | Dato |
|---|---|
| Motivo de salida | Venta |
| Observaciones | `Venta a frigorífico. Remito 4471.` |

**Esperado.** `160` sale de las listas y de los desplegables, pero su ficha, su
historia sanitaria y su lugar en el arbol genealogico **se conservan**. Se lo sigue
encontrando con el filtro de dados de baja.

Distinguir explicitamente los dos mecanismos, porque son decisiones distintas:

- **Baja logica** (`activo = 0` mas motivo): el animal existio y se fue. Venta,
  muerte, descarte. Su historia es parte de la historia del tambo.
- **Borrado fisico con reversion**: el registro nunca debio existir. Es correccion de
  un error de carga, y por eso deshace todo lo que el alta habia hecho.

---

## 10. Lo que llega al telefono [nucleo] — 4 min

Es el unico tramo donde el sistema hace algo sin que nadie lo abra, asi que conviene
que sea el ultimo: cierra la idea de que los datos cargados sirven aunque la encargada
no este mirando la pantalla.

**Antes de la reunion** hay que tener el bot vinculado y el token cargado
(`bd/LEEME.md`, punto 7). Se prueba mandandole `/resumen` al bot y viendo que conteste.

### 10.1 De donde sale el mensaje

**Reportes y notificaciones → Notificaciones.** Mostrar la pantalla: el chat vinculado,
la hora del resumen y los ocho interruptores, agrupados por modulo.

**La frase:** son los ocho contadores del tablero de inicio, ni uno mas. El aviso no
calcula nada por su cuenta; es un canal de entrega.

### 10.2 El mensaje, al lado del tablero

Poner el telefono a la vista y escribirle **`/resumen`** al bot. Llega el mensaje
agrupado por modulo.

Abrir el tablero de inicio en la pantalla, al lado. **Recorrer los numeros de a uno**:
las vacas para servir, el tacto pendiente, el parto proximo, los insumos bajo el
minimo. Tienen que coincidir, y coinciden porque salen del mismo calculo.

### 10.3 Apagar un aviso

Destildar **Stock critico**, guardar, y volver a pedir `/resumen`: el bloque
desaparecio. Ir despues a **Insumos → Alertas de Stock**: los dos insumos siguen ahi.

**La frase:** apagar un aviso apaga el mensaje, no la informacion.

Volver a tildarlo antes de seguir.

**Si preguntan por el envio automatico:** el resumen sale solo a la hora configurada,
una vez por dia, y lo manda un proceso que vive adentro del sitio. Con el sitio
apagado no sale; si el sitio estuvo caido a esa hora, sale cuando vuelve a levantar en
lugar de saltear el dia.

---

## 11. Cierre — 2 min

Tres frases, y despues preguntas:

1. **Las reglas estan en un solo lugar.** Toda la logica vive en la Controladora; las
   pantallas no deciden nada. Por eso la misma regla de consanguinidad se aplica en el
   alta de animales, en el servicio y en la consulta suelta, y es una sola.
2. **El sistema distingue lo imposible de lo sospechoso.** Lo primero se bloquea con
   el motivo; lo segundo se advierte y el usuario confirma. Trabar todo haria
   imposible cargar un rodeo real.
3. **Lo que falta, esta anotado.** `docs/pendientes-tecnicos.md` tiene lo que queda
   abierto: el rediseno del home que el tutor pidio, y la ausencia de pruebas
   automatizadas. Decirlo antes de que lo pregunte suma mas que esconderlo.

---

## Anexo A. Catalogo de validaciones, por tipo

Sirve si pregunta "¿y que pasa si...?". Estan agrupadas por **como** reacciona el
sistema, que es el criterio de diseño.

### A.1 Bloqueos duros — no hay boton para forzarlos

| Regla | Donde |
|---|---|
| Caravana repetida | Alta de animal, crias del parto |
| Fecha futura | Todas las altas con fecha |
| La madre tiene que haber nacido 22 meses antes que la cria | Alta y modificacion de animal, parto |
| Un animal no puede ser su propio padre o madre; el progenitor no puede descender de el | Alta y modificacion de animal |
| Celo antes de los 9 meses (biologia, constante) | Registrar celo |
| Celo a un macho | Registrar celo |
| Servicio antes de la edad minima configurada (13 meses) | Registrar servicio |
| Servicio a una hembra preñada (le provoca el aborto) | Registrar servicio |
| Toro y pajuela a la vez, o ninguno de los dos | Registrar servicio |
| Pajuela sin stock | Registrar servicio |
| Movimiento sobre un animal posterior a su baja | Celo, servicio, parto |
| Control individual a una vaca que no esta en lactancia | Ordeñe individual |
| Control individual por encima del maximo configurado | Ordeñe individual |
| Ordeñe de lote repetido para la misma fecha y turno | Ordeñe por lote |
| Lote sin animales, o litros no positivos | Ordeñe por lote |
| Lactancia abierta sobre una que ya lo esta | Alta de lactancia |
| Secado de una vaca que no esta en lactancia | Registrar secado |
| Tratamiento sin stock suficiente | Registrar tratamiento |
| Las dos crias de un parto doble con la misma caravana | Registrar parto |
| Valores de configuracion fuera de rango | Configuracion |

### A.2 Advertencias — muestran el problema y ofrecen confirmar

| Regla | Boton |
|---|---|
| Padre y madre emparentados: la cria nace consanguinea | **Guardar de todos modos** |
| Hembra y reproductor emparentados | **Registrar de todos modos** |
| Progenitor que ya figuraba dado de baja (correcto si vino de una pajuela) | **Guardar de todos modos** |
| Mellizos de distinto sexo: la hembra nace freemartin | **Confirmar de todos modos** |
| Parto de una vaca que no figuraba preñada | **Confirmar de todos modos** |
| Gestacion de menos de 240 o mas de 320 dias | **Confirmar de todos modos** |
| Toro dado de baja antes de la fecha de la monta | **Registrar de todos modos** |
| Parto de una vaca en lactancia: avisa que va a cerrar la abierta | **Confirmar de todos modos** |

### A.3 Bloqueos por dependencia — dicen donde ir a resolverlo

| Que se intenta borrar | Que lo frena |
|---|---|
| Servicio | Tiene tactos registrados |
| Parto | No es el ultimo de la vaca / la lactancia tiene controles de ordeñe / la cria ya tiene historia propia |
| Diagnostico | Tiene tratamientos colgando |
| Insumo, plan, animal | Tienen movimientos o aplicaciones asociadas |

La excepcion deliberada es el **parto**, que se lleva las crias y la lactancia juntas
— pero solo si nada de eso se uso todavia.

---

## Anexo B. Preguntas probables, y como contestarlas

| Pregunta | Respuesta corta |
|---|---|
| ¿Por que no usaron Entity Framework? | El Proyecto define capa de persistencia con SQL explicito y una clase por entidad. Se respeto el diseño documentado; `pConexion` centraliza conexion y comandos. |
| ¿Por que MySQL? | Es lo que fija el Proyecto y corre sobre XAMPP, sin licencia ni instalacion de servicio. |
| ¿Como se prueba? | Hoy a mano, con `bd/DatosPrueba.sql` y el guion de `docs/flujos-de-prueba.md`. La falta de pruebas automatizadas esta declarada en `docs/pendientes-tecnicos.md`. |
| ¿Los datos de prueba no se vencen? | No: el script ancla todo el rodeo a `@hoy = CURDATE()`, asi que corriendolo cualquier dia las alertas quedan pobladas. El ancla se puede fijar a mano para reproducir una situacion puntual. |
| ¿Y si dos personas cargan a la vez? | La Controladora usa listas `static` como cache y cada peticion las recarga. Bajo concurrencia real hay que revisarlo; esta anotado como pendiente. |
| ¿Los diagramas se corresponden con el codigo? | Si: el Proyecto v6 documenta el sistema construido, y los diagramas y el diccionario de clases se **generan** con scripts desde `docs/`, no se dibujan a mano. |
| ¿De donde salen las reglas (283 dias, freemartin, carencia)? | Son criterios de manejo lechero. Los que cada establecimiento decide quedaron en Configuracion; los biologicos, como constantes documentadas en la Controladora. |
| ¿El control lechero tiene que dar igual que el ordeñe por lote? | No. El lote es lo que entro al tanque; el control es lo que dieron las vacas que se midieron. No son la misma poblacion ni la misma leche: la vaca con descarte se ordeña y no va al tanque, hay vacas sin medir, hay calostro y leche para los terneros, y sumar muchas mediciones acumula el error de cada una. El sistema toma el tanque como el volumen producido y el control como el reparto, y nunca los suma. |
| ¿Que pasa con la foto de los animales? | Se guarda recien cuando todas las validaciones pasaron, para no dejar archivos huerfanos en cada intento fallido. |
| ¿Hay control de acceso por rol? | No. Hay autenticacion por cookie y todo el sitio detras del login; los roles no estaban en el alcance. |
| ¿Por que el borrado es fisico y no una anulacion logica? | Para no tener que filtrar registros anulados en cuarenta consultas. El borrado revierte los efectos y se bloquea cuando hay dependencias. |

---

## Anexo C. Datos del rodeo, para tener a mano

| Caravana | Que es | Estado el dia de la demo |
|---|---|---|
| `T-01` | Toro del rodeo, en pie | Sirve para monta natural |
| `7HO12165`, `7JE01722`, `29HO18296` | Toros de catalogo | Solo aportan pajuelas |
| `101`, `108`, `124` | Vacas en ordeñe | Preñadas |
| `102` | Vaca en ordeñe, 11 meses de lactancia | Servida hace 52 dias, **tacto pendiente** |
| `115` | Vaca en ordeñe | Vacia, mastitis en tratamiento, **descarte hasta dentro de 6 dias** |
| `121` | Vaca en ordeñe | Servida hace 24 dias |
| `130`, `133` | Vacas en ordeñe | Vacias |
| `136` | Vaca seca | Preñada, **pare dentro de 7 dias** |
| `140` | Vaca seca | Preñada, pare dentro de 30 dias |
| `152` | Vaquillona | **Preñada**; hija del toro `7HO12165` |
| `155` | Vaquillona | Servida hace 14 dias |
| `158` | Vaquillona | Vacia; **hija del toro `7JE01722`** |
| `171`, `174`, `177` | Terneras | A `177` le falta la brucelosis |
| `175`, `178` | Terneros | `178` sin descornar |
| `160` | Novillo | Candidato a venta |
| `112` | Vaca dada de baja hace cinco meses | No aparece en las listas |

**Insumos que se tocan en la demo:** oxitetraciclina (8), ivermectina (3, bajo
minimo), vacuna brucelosis (8), pajuela `7HO12165` (17), pajuela `7JE01722` (6),
pajuela `29HO18296` (3, bajo minimo).

**Caravanas que se dan de alta durante la demo:** `200` (vaca comprada), `180` y `181`
(los mellizos). `201` se intenta y no se guarda.

---

## Anexo D. Plan B

| Si pasa esto | Hacer esto |
|---|---|
| El sitio no levanta | Revisar que el motor este arriba (`mysqladmin -u root ping`). El login anda igual con el motor apagado, asi que un login exitoso **no** prueba que la base este. |
| Se rompio el juego de datos en medio de la demo | Volver a correr `bd/DatosPrueba.sql`. Tarda segundos, vacia las tablas de datos y deja el rodeo como al empezar, anclado al dia de hoy. |
| Las alertas aparecen vacias | El script de datos no corrio completo. Correrlo de nuevo y mirar que las dos consultas del final devuelvan cero filas. |
| Una pantalla tira error | Seguir con el tramo siguiente y anotarlo. No debuggear en vivo. |
| El bot no contesta | Seguir con la pantalla de notificaciones, que se explica sola, y mostrar el ultimo resumen que si salio -la fecha figura ahi-. No pelear con la conexion en vivo. |
| Sobra tiempo | Los tramos `[opcional]`: buscar y filtrar, historial y metrica, insumos completo, planes sanitarios. |
| Falta tiempo | Saltar el 3.6 y el tramo 7, y comprimir el 4 a la carga del lote, que es donde esta el cruce con Sanidad. |

---

## Anexo E. Recordatorio para el que presenta

- **Ir de a un dato por vez.** En cada pantalla, decir primero que se va a cargar y
  por que, despues cargarlo.
- **Mostrar el efecto, no solo el guardado.** Ninguna carga termina en "guardo": todas
  terminan en otra pantalla donde se ve la consecuencia.
- **Leer los mensajes de error en voz alta.** Estan escritos para entenderse sin
  explicacion; que se note.
- **No pedir disculpas por lo que falta.** Esta en `pendientes-tecnicos.md` y se dice
  al final, una vez.
