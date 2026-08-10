# Puesta en marcha — Módulos 0 a 5

## 1. Crear la base

Con MySQL instalado y corriendo, alcanza con un solo script:

```bash
mysql -u root -p < bd/tambo.sql
```

O abrirlo en MySQL Workbench y ejecutarlo entero. Crea la base `tambo` con las
veintidós tablas de los Módulos 0 a 5 más la de configuración, y carga las razas,
las categorías y la fila de parámetros del establecimiento.

**El script empieza borrando la base `tambo` si ya existe, con todos sus datos.**
Eso es lo que le permite dejar siempre el mismo esquema por más veces que se
corra. Para conservar lo que haya cargado, respaldar antes:

```bash
mysqldump -u root -p tambo > respaldo_tambo.sql
```

### Los scripts por módulo

`tambo.sql` reemplaza a la secuencia de scripts por módulo, que quedan como el
registro de cómo se fue entregando cada uno y son los que citan los documentos de
`docs/`. Sirven igual —corridos en orden dejan la misma base—, pero para poner en
marcha una instalación no hacen falta:

```bash
mysql -u root -p < bd/tambo_m0_m1.sql
mysql -u root -p < bd/tambo_m2_m3.sql
mysql -u root -p < bd/tambo_m4_m5.sql
mysql -u root -p < bd/tambo_configuracion.sql
```

Cada uno asume que los anteriores ya corrieron: las tablas nuevas tienen claves
foráneas hacia `animales`, `hembras`, `machos` e `insumos`. Si la base ya tenía la
tabla `configuracion` creada de antes, en lugar del último hay que correr solo la
actualización, que agrega los dos parámetros de trabajo reproductivo:

```bash
mysql -u root -p < bd/tambo_configuracion_actualizacion.sql
```

`tambo_m0_m1.sql` crea la base `tambo`, las cinco tablas del Módulo 1 (`razas`,
`categorias`, `animales`, `hembras`, `machos`) y carga las razas y categorías
semilla. El Módulo 0 no necesita tablas: las credenciales son fijas y se validan
en memoria.

`tambo_m2_m3.sql` agrega:

- **Módulo 2:** `lactancias`, `ordenies_lote`, `ordenie_lote_animales`,
  `ordenies_individual`.
- **Módulo 3:** `celos`, `servicios`, `tactos`, `partos`.
- **Adelantado del Módulo 5:** `insumos`, `movimientos_stock` — CU15 registra la
  inseminación artificial con una pajuela del stock y le descuenta una unidad.
- **Adelantado del Módulo 4:** `diagnosticos`, `tratamientos` — el paso 3 de CU8
  excluye del lote de ordeñe a los animales con descarte de leche vigente.

`tambo_m4_m5.sql` completa esos dos módulos:

- **Módulo 4:** `planes_sanitarios`, `plan_categorias`, `vacunaciones`, `descornes`.
- **Módulo 5:** no crea tablas nuevas —`insumos` y `movimientos_stock` ya estaban—:
  los umbrales, los vencimientos por partida y el historial salen de esas dos.
- **Sobre `tratamientos`:** agrega la clave foránea de `id_plan`, que había quedado
  pendiente hasta que existiera `planes_sanitarios`, y la columna `id_animal`, sin la
  cual el tratamiento preventivo no se puede atribuir a ningún animal. El script
  completa esa columna en los tratamientos ya cargados que venían de un diagnóstico.
  Está explicado en `docs/desvios-modulos-4-y-5.md`.

## 1.a. Datos de prueba

`tambo_datos_prueba.sql` carga un rodeo chico pero completo para poder recorrer el
sistema sin tener que dar de alta todo a mano. Se corre después de `tambo.sql`:

```bash
mysql -u root -p < bd/tambo_datos_prueba.sql
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
lo último y que los controles individuales del 05/08 sumen el total del lote de ese
día: las dos tienen que devolver cero filas.

La fecha de referencia es **2026-08-09**, y el juego está armado para que parado en
ese día haya trabajo pendiente en todos los tableros: una vaca con descarte de leche
vigente y fuera del lote de ordeñe, dos partos próximos, un tacto atrasado, tres
vacas en condiciones de servicio, un ternero sin descornar, dos insumos bajo el
mínimo y dos partidas por vencer. Vale la pena tenerlo en cuenta al mirar las
alertas: corriendo el script mucho después de esa fecha, los vencimientos y los
partos próximos ya habrán pasado.

El script empieza vaciando las tablas de datos, así que se puede volver a correr.
No toca razas, categorías ni la fila de configuración, que las deja `tambo.sql`.

## 1.b. Carga inicial del rodeo

Las vacas que ya estaban en ordeñe cuando arranca el sistema no tienen un parto
registrado, así que tampoco tienen lactancia abierta. Sin lactancia no se les puede
cargar un ordeñe individual (CU9) ni registrarles el secado (CU12). Para eso está
**Producción → Lactancias → Abrir Lactancia**, que abre la lactancia a mano y deja
al animal en estado productivo "En lactancia".

De ahí en adelante la lactancia la abre sola el parto (CU18).

## 1.c. Puesta en marcha sanitaria

El calendario sanitario no trae nada precargado: proyecta lo que exigen los planes
configurados. Sin planes activos no muestra procedimientos, y así lo informa. El
esquema del establecimiento —aftosa, brucelosis, desparasitación de recría,
descorne— se carga desde **Sanidad → Planes Sanitarios**, y cada plan necesita el
insumo que aplica ya dado de alta en **Insumos** (salvo el descorne, que no consume).

Para que el calendario dé por cumplido un procedimiento, la aplicación tiene que
declarar a qué plan corresponde: es el campo "Plan sanitario que cumple" de las
pantallas de vacunación, tratamiento y descorne.

## 2. Completar la cadena de conexión

Los datos de conexión ya no están en el código: se leen de `Tesis/appsettings.json`.

```json
"ConnectionStrings": {
  "Tambo": "server=localhost; port=3306; database=tambo; uid=root; pwd=; CharSet=utf8mb4;"
}
```

Ajustar servidor, puerto, usuario y contraseña según la instalación de MySQL.

Para no versionar la contraseña real conviene usar user-secrets, que la guarda
fuera del repositorio y pisa el valor de `appsettings.json`:

```bash
dotnet user-secrets init --project Tesis/Tesis.csproj
dotnet user-secrets set "ConnectionStrings:Tambo" "server=localhost; port=3306; database=tambo; uid=root; pwd=LA_CONTRASENA; CharSet=utf8mb4;" --project Tesis/Tesis.csproj
```

## 3. Credenciales del sistema

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

Todo el sitio queda detrás del login: sin sesión iniciada cualquier página
redirige a `/PagesSeguridad/Login`.

## 4. Correr

```bash
dotnet run --project Tesis/Tesis.csproj
```

Queda en `http://localhost:5174`. El login está en `/PagesSeguridad/Login` y el
listado de animales en `/PagesAnimal/ListaAnimales`.

## 5. Constantes de negocio de los Módulos 2 a 5

El documento no las fija con números. Están en `Dominio/Controladora.cs` con los
valores habituales de un tambo Holando.

**Nueve de ellas ya no son constantes sino parámetros configurables**: viven en la
tabla `configuracion` y se editan desde **Configuración** en el menú. Los valores de
abajo pasaron a ser el valor por defecto de cada una, el que trae una base recién
creada. Qué se configura y qué no está explicado en
`docs/configuracion-del-establecimiento.md`.

| Constante | Valor | Para qué |
|---|---|---|
| `GESTACION_DIAS` | 283 | Fecha probable de parto (RF3.6, CU15, CU16) |
| `DIAS_SECADO_ANTES_PARTO` | 60 | Fecha recomendada de secado (RF2.10) |
| `DIAS_ANTICIPACION_SECADO` | 15 | Ventana de la alerta de secado (CU13) |
| `DIAS_ANTICIPACION_PARTO` | 15 | Ventana de la alerta de parto (CU17) |
| `LITROS_MAXIMOS_INDIVIDUAL` | 100 | Coherencia del control individual (RF2.3) |
| `LITROS_MAXIMOS_LOTE` | 100000 | Coherencia del ordeñe por lote (RF2.3) |
| `DIAS_ANTICIPACION_SANITARIA` | 30 | Horizonte del calendario sanitario (CU23) |
| `DIAS_ANTICIPACION_VENCIMIENTO` | 30 | Ventana de la alerta de vencimiento (CU28) |
| `UNIDADES_POR_VACUNACION` | 1 | Dosis que consume una aplicación (CU21) |
| `EDAD_MINIMA_CELO_MESES` | 9 | Edad a la que la hembra empieza a ciclar (CU14) |
| `DIAS_LACTANCIA_ESTANDAR` | 305 | Referencia para proyectar la produccion de una lactancia |
| `GESTACION_DIAS_MINIMA` | 240 | Piso de una gestación viable, para advertir en el parto |
| `GESTACION_DIAS_MAXIMA` | 320 | Techo de una gestación normal, para advertir en el parto |

Las dos ventanas de anticipación se pueden cambiar desde la pantalla: el valor de la
tabla es el que trae por defecto.

`GESTACION_MESES` (9) sigue existiendo aparte: la usa la validación de genealogía
del Módulo 1, que razona en meses.
