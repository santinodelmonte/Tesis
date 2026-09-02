# Puesta en marcha

La carpeta tiene dos scripts y nada más:

| Script | Qué hace |
|---|---|
| `CreacionDb.sql` | Crea la base `tambo` completa: las veinticuatro tablas y los datos semilla |
| `DatosPrueba.sql` | Carga un rodeo de prueba sobre una base ya creada |

> **El entorno de desarrollo usa XAMPP.** El motor que trae XAMPP es MariaDB, que
> para todo lo que hace el sistema se comporta igual que MySQL y habla con el mismo
> conector. Se levanta desde el panel de control de XAMPP, no como servicio de
> Windows, así que buscarlo en la lista de servicios no lo encuentra. El cliente de
> línea de comandos está en `C:\xampp\mysql\bin\mysql.exe`. Los valores por defecto
> de XAMPP —`localhost`, puerto 3306, usuario `root`, sin contraseña— son los que ya
> trae la cadena de conexión de `Tesis/appsettings.json`.

## 1. Crear la base

Con el motor corriendo, alcanza con un solo script:

```bash
mysql -u root -p < bd/CreacionDb.sql
```

O abrirlo en MySQL Workbench y ejecutarlo entero. Crea la base `tambo` con las
veinticuatro tablas y carga las razas, las categorías, la fila de parámetros del
establecimiento y los ocho tipos de aviso, que son los cuatro datos que el sistema no da
de alta por pantalla.

**El script empieza borrando la base `tambo` si ya existe, con todos sus datos.**
Eso es lo que le permite dejar siempre el mismo esquema por más veces que se
corra. Para conservar lo que haya cargado, respaldar antes:

```bash
mysqldump -u root -p tambo > respaldo_tambo.sql
```

No hay script de actualización incremental: una base vieja se rehace corriendo
`CreacionDb.sql` de nuevo, y lo que se quiera conservar se restaura del respaldo.

**Sobre la foto del animal:** la columna `foto` de `animales` guarda el nombre del
archivo, no la imagen. Las imágenes viven en `Tesis/wwwroot/fotos`, así que **esa
carpeta hay que respaldarla aparte del dump de la base**: un dump solo no alcanza
para restaurar el sistema completo. La carpeta no se versiona —son datos del
establecimiento, no fuente— y la crea el sistema al arrancar si no existe.

## 2. Datos de prueba

`DatosPrueba.sql` carga un rodeo chico pero completo para poder recorrer el sistema
sin tener que dar de alta todo a mano. Se corre después de `CreacionDb.sql`:

```bash
mysql -u root -p < bd/DatosPrueba.sql
```

Son 21 animales del establecimiento más 3 toros de catálogo que sólo aportan
pajuelas, una semana de ordeñes con dos controles lecheros, la reproducción en
marcha —servicios, tactos, partos y celos—, los cinco planes sanitarios con sus
aplicaciones y el stock con sus movimientos.

Los datos no son al azar: la fecha probable de parto es siempre la del servicio más
283 días, el estado de cada hembra se corresponde con su último tacto, el número de
partos coincide con la lactancia abierta, la raza de cada cría sale de la de sus
padres y el `stock_actual` de cada insumo es exactamente el ingreso menos los
egresos que dejó cada aplicación. Las dos consultas del final del script comprueban
lo último y que los controles individuales del día del control lechero sumen el
total del lote de ese día: las dos tienen que devolver cero filas.

**El juego de datos no tiene fechas fijas.** Todas se escriben contra la variable
`@hoy`, que el script define como `CURDATE()`, así que el rodeo se corre solo al día
en que se carga y las alertas nunca aparecen vacías por haber quedado viejas. Parado
en el día de la carga hay trabajo pendiente en todos los tableros: una vaca con
descarte de leche vigente y fuera del lote de ordeñe, un parto próximo, un tacto
atrasado, cuatro vacas en condiciones de servicio, una ternera sin vacunar, un
ternero sin descornar, dos insumos bajo el mínimo y dos partidas por vencer. El
último ordeñe cargado es el de anteayer: el de ayer y el de hoy son los que se cargan
a mano al recorrer el sistema.

Para reproducir una situación puntual, el ancla se fija a mano editando esa línea:

```sql
SET @hoy = DATE('2026-08-19');
```

Con `SET @hoy = DATE('2026-08-11')` el juego queda idéntico al que tenía fechas
fijas, que es el que describe `docs/flujos-de-prueba.md`.

El script empieza vaciando las tablas de datos, así que se puede volver a correr.
No toca razas, categorías, los ocho tipos de aviso ni los parámetros de la fila de
configuración, que los deja `CreacionDb.sql`. Sí borra las alertas ya enviadas —apuntan
a animales e insumos que el script está por reemplazar— y deja el resumen del día como
pendiente, para que el bot vuelva a mandarlo con el rodeo recién cargado.

## 3. Carga inicial del rodeo

Las vacas que ya estaban en ordeñe cuando arranca el sistema no tienen un parto
registrado, así que tampoco tienen lactancia abierta. Sin lactancia no se les puede
cargar un control lechero (CU13) ni registrarles el secado (CU16). Para eso está
**Producción → Lactancias → Abrir Lactancia**, que abre la lactancia a mano y deja
al animal en estado productivo "En lactancia".

De ahí en adelante la lactancia la abre sola el parto.

## 4. Puesta en marcha sanitaria

El calendario sanitario no trae nada precargado: proyecta lo que exigen los planes
configurados. Sin planes activos no muestra procedimientos, y así lo informa. El
esquema del establecimiento —aftosa, brucelosis, desparasitación de recría,
descorne— se carga desde **Sanidad → Planes Sanitarios**, y cada plan necesita el
insumo que aplica ya dado de alta en **Insumos** (salvo el descorne, que no consume).

Para que el calendario dé por cumplido un procedimiento, la aplicación tiene que
declarar a qué plan corresponde: es el campo "Plan sanitario que cumple" de las
pantallas de vacunación, tratamiento y descorne.

## 5. Completar la cadena de conexión

Los datos de conexión no están en el código: se leen de `Tesis/appsettings.json`.

```json
"ConnectionStrings": {
  "Tambo": "server=localhost; port=3306; database=tambo; uid=root; pwd=; CharSet=utf8mb4;"
}
```

Ajustar servidor, puerto, usuario y contraseña según la instalación.

Para no versionar la contraseña real conviene usar user-secrets, que la guarda
fuera del repositorio y pisa el valor de `appsettings.json`:

```bash
dotnet user-secrets init --project Tesis/Tesis.csproj
```

```bash
dotnet user-secrets set "ConnectionStrings:Tambo" "server=localhost; port=3306; database=tambo; uid=root; pwd=LA_CONTRASENA; CharSet=utf8mb4;" --project Tesis/Tesis.csproj
```

## 6. Credenciales del sistema

También salen de `Tesis/appsettings.json`:

```json
"Seguridad": {
  "Usuario": "sofia",
  "Contrasena": "tambo2026"
}
```

Si no se cargan estas dos claves el sistema no habilita el acceso a nadie.
Igual que la cadena de conexión, se pueden mover a user-secrets:

```bash
dotnet user-secrets set "Seguridad:Contrasena" "LA_CONTRASENA" --project Tesis/Tesis.csproj
```

Todo el sitio queda detrás del login: sin sesión iniciada cualquier página redirige
a `/PagesSeguridad/Login`.

## 7. El token del bot de Telegram

Sin token, el sistema funciona igual: la pantalla **Reportes y notificaciones →
Notificaciones** avisa que falta y el proceso del resumen diario no arranca. Con token,
hay que completar además la vinculación desde esa pantalla.

El token **no va en `appsettings.json`**, que está versionado. Es una credencial —quien
lo tiene maneja el bot— y Telegram revoca solo los que aparecen en un repositorio
público. Va en `appsettings.Development.json`, que no se versiona:

```json
"Telegram": {
  "Token": "1234567890:AAxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx"
}
```

El repositorio trae `Tesis/appsettings.Development.json.ejemplo` como plantilla: se copia
sin la extensión `.ejemplo` y se completa. Igual que la cadena de conexión, también se
puede usar user-secrets:

```bash
dotnet user-secrets set "Telegram:Token" "EL_TOKEN" --project Tesis/Tesis.csproj
```

**En el hosting el archivo de desarrollo no se lee**, porque sólo carga en el entorno
Development. Ahí el token va como variable de entorno, con dos guiones bajos en lugar de
los dos puntos:

```
Telegram__Token=1234567890:AAxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx
```

**Y la zona horaria.** El resumen sale a la hora que dice el reloj del servidor, igual
que se calculan la fecha probable de parto, los vencimientos y el fin del descarte: todo
el sistema razona en hora local. Un servidor en UTC corre las ocho horas al resumen y,
peor, al día que el sistema considera "hoy". Se resuelve en el hosting con la variable
de entorno del sistema operativo:

```
TZ=America/Argentina/Buenos_Aires
```

## 8. Correr

```bash
dotnet run --project Tesis/Tesis.csproj
```

Queda en `http://localhost:5174`. El login está en `/PagesSeguridad/Login` y el
listado de animales en `/PagesAnimal/ListaAnimales`.

## 9. Parámetros de manejo y constantes de negocio

Los **doce parámetros de manejo** del establecimiento viven en la tabla
`configuracion` y se editan desde **Configuración** en el menú. `CreacionDb.sql`
crea la tabla con un valor por defecto en cada columna y deja cargada la única fila.

| Parámetro | Por defecto |
|---|---|
| `dias_secado_antes_parto` | 60 |
| `edad_minima_servicio_meses` | 13 |
| `edad_cambio_categoria_meses` | 12 |
| `litros_maximos_individual` | 100 |
| `ordenies_por_dia` | 2 |
| `dias_anticipacion_secado` | 15 |
| `dias_anticipacion_parto` | 15 |
| `dias_anticipacion_sanitaria` | 30 |
| `dias_anticipacion_vencimiento` | 30 |
| `dias_espera_voluntaria` | 45 |
| `dias_para_tacto` | 35 |
| `hora_resumen` | 07:00 |

Las mismas doce figuran como constantes en `Dominio/Controladora.cs`, pero ahí son
sólo el valor de respaldo: si la fila de configuración no existiera, el sistema se
comporta como antes de que la configuración existiera.

Lo que **no** es configurable y sigue siendo constante en la Controladora es lo que
no es una decisión del establecimiento:

| Constante | Valor | Por qué no se configura |
|---|---|---|
| `GESTACION_DIAS` | 283 | Biología: es la duración de la gestación Holando |
| `GESTACION_MESES` | 9 | La misma, en meses, para la validación de genealogía |
| `GESTACION_DIAS_MINIMA` / `MAXIMA` | 240 / 320 | Rango de una gestación viable, para advertir en el parto |
| `EDAD_MINIMA_SERVICIO_MESES` | 15 | Edad de servicio del macho |
| `EDAD_MINIMA_CELO_MESES` | 9 | Edad a la que la hembra empieza a ciclar |
| `DIAS_LACTANCIA_ESTANDAR` | 305 | Referencia con la que trabaja cualquier control lechero |
| `LITROS_MAXIMOS_LOTE` | 100000 | Tope de coherencia del ordeñe por lote |
| `UNIDADES_POR_VACUNACION` | 1 | Una aplicación consume una dosis |
| `PORCENTAJE_PRODUCCION_BAJA` | 0,7 | Criterio de descarte, no regla de manejo diaria |
| `SERVICIOS_SIN_PRENIEZ_DESCARTE` | 3 | Ídem |
| `DIAS_ABIERTOS_EXCESIVOS` | 150 | Ídem |
| `DIAGNOSTICOS_REPETIDOS_DESCARTE` | 3 | Ídem |
| `PARTOS_PARA_DESCARTE` | 7 | Ídem |

El período de carencia y el stock mínimo tampoco están acá: son datos del producto
y viven en `insumos`.

Las otras dos columnas de `configuracion` no son parámetros de manejo y no se editan
desde esa pantalla: `chat_telegram` lo escribe la pantalla de notificaciones al vincular
la cuenta, y `fecha_ultimo_resumen` lo escribe el proceso del resumen —es lo único de la
tabla que decide el sistema y no la encargada—.
