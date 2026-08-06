# Discrepancias entre código y documentación — Módulos 0 y 1

Análisis del commit `1cf8d42` ("Modulo 0 y 1") contra el Anteproyecto v5 y el
documento de Proyecto (secciones 2.1 Análisis y 2.2 Diseño).

**Alcance.** La documentación cubre 35 casos de uso en 7 módulos (0 a 6). El
código implementa el Módulo 0 (Seguridad) y el Módulo 1 (Gestión de Animales y
Genética), que es el incremento planificado. Que los módulos 2 a 6 no estén
escritos **no** se cuenta como discrepancia: es el plan de iteraciones. Lo que
se lista abajo son diferencias dentro de lo que sí está construido, más los
puntos donde lo construido va a chocar con los módulos siguientes.

---

## Parte A — Listado de discrepancias

### A1. El login no autentica nada

| | |
|---|---|
| **Documentación** | RF0.1: "El sistema debe restringir el acceso mediante un único par de credenciales fijas". CU1 post-condición: "El usuario queda logueado en el sistema con acceso a todos los módulos". RNF Seguridad: "restringir el acceso a la información mediante mecanismos de autenticación". Los 34 CU restantes tienen como pre-condición "El usuario debe estar logueado en el sistema". |
| **Código** | `Pages/PagesSeguridad/Login.cshtml.cs:29` valida las credenciales y hace `Redirect("/Index")`. No crea sesión, ni cookie, ni claim. `Program.cs:20` invoca `UseAuthorization()` sin ningún esquema de autenticación registrado y sin `UseAuthentication()`. No hay una sola referencia a `Session`, `Cookie`, `Claims` ni `[Authorize]` en todo el proyecto. |

Consecuencia: escribir `/PagesAnimal/AltaAnimal` en la barra de direcciones
entra sin pasar por el login. La pre-condición que repiten los 34 casos de uso
no se cumple en ninguno. El Módulo 0 está documentado pero no implementado.

### A2. Eliminación física de animales: sin caso de uso y contra la regla de negocio

| | |
|---|---|
| **Documentación** | CU4, Reglas de Negocio: *"Las bajas deben ser lógicas para no romper el historial productivo ni el linaje genealógico de otros animales emparentados"*. No existe ningún CU de borrado definitivo. `EliminarAnimal` no figura en el Diccionario de Clases (2.2.7) ni en el diagrama de dominio. |
| **Código** | `Dominio/Controladora.cs:201` `EliminarAnimal()`, `Persistencia/pAnimal.cs:157` `DELETE FROM animales`, páginas `EliminarAnimal.cshtml(.cs)`, enlazadas desde `DetalleAnimal.cshtml:138`. |

El guard `EsProgenitor` (`Controladora.cs:225`) protege hoy el linaje, pero no
cubre las tablas de los módulos 2 a 6 (`partos`, `lactancias`,
`ordenies_individual`, `diagnosticos`, `vacunaciones`, `descornes`), todas con
FK a `animales` según 2.2.5.2.

### A3. Métodos en el código que no están en el Diccionario de Clases

`EsProgenitor` (`Controladora.cs:225`), `ListarAscendencia` (`:338`),
`BuscarAncestroComun` (`:375`) y `FiltrarAnimales` con seis parámetros (`:447`).

A la inversa: el diccionario documenta `FiltrarAnimalesXRaza` y
`FiltrarAnimalesXCategoria`, que existen (`:405`, `:419`) pero nunca se llaman
—las pantallas usan el `FiltrarAnimales` compuesto, que no está documentado.

### A4. Operaciones de persistencia sin contraparte en el dominio ni caso de uso

`pHembra.ModificarHembra` (`pHembra.cs:42`), `pRaza.AltaRaza` (`pRaza.cs:29`),
`pCategoria.AltaCategoria` (`pCategoria.cs:29`), `ProximoRazaId`,
`ProximoCategoriaId` — todas expuestas en `pControladora` y ninguna alcanzable
desde la Controladora. Además `bd/tambo_m0_m1.sql:100` dice explícitamente
"Razas y categorias no tienen alta desde el sistema: se cargan aca".

### A5. La capa de persistencia depende de la capa de dominio

| | |
|---|---|
| **Documentación** | 2.2.3: *"La Controladora es además el único punto de contacto entre la capa de dominio y la capa de acceso a datos, a través de su campo Persistencia"*. 2.2.4 muestra el flujo `pControladora → pX → pConexion`, sin dependencias hacia arriba. 2.1: "arquitectura en tres capas —presentación, dominio y datos—". |
| **Código** | `pAnimal.cs:23`, `pHembra.cs:16` y `pMacho.cs:16` hacen `new Controladora()` desde la capa de datos y leen la caché estática del dominio (`BuscarRaza`, `BuscarCategoria`, `BuscarAnimal`). |

Es una dependencia circular Dominio ↔ Persistencia que el diagrama de
persistencia no contempla.

### A6. Los identificadores se asignan a mano aunque el modelo los declara auto-incrementales

| | |
|---|---|
| **Documentación** | 2.2.5.4, tabla `animales`: `id_animal Int(11) — PK, Auto increment`. Lo mismo para el resto del esquema. |
| **Código** | `pAnimal.cs:164` `SELECT (IFNULL(MAX(id_animal),0)+1)` y `pAnimal.cs:101` incluye `id_animal` explícito en el INSERT. Idem `pHembra.cs:55` y `pMacho.cs:42`. |

El DDL (`tambo_m0_m1.sql:48`) sí declara `AUTO_INCREMENT`, así que la base y el
código están peleados: la secuencia interna de MySQL nunca avanza.

### A7. `ProximoHembraId` / `ProximoMachoId` no tienen sentido en el modelo

Están documentados en el Diccionario de Clases y codificados
(`Controladora.cs:519`, `:576`), pero `hembras.id_animal` es FK a
`animales.id_animal`: no existe una secuencia propia de hembras. Nunca se
invocan desde ninguna pantalla.

### A8. Genealogía obligatoria: el documento se contradice a sí mismo y con el código

| | |
|---|---|
| **Documentación (a favor de obligatorio)** | RF1.5: "El sistema debe permitir registrar **obligatoriamente** padre y madre de cada animal al momento del alta". CU2, Reglas de Negocio: "Todo animal debe registrar obligatoriamente a su padre y a su madre". CU2, Validaciones: "Todos los campos del formulario son obligatorios". |
| **Documentación (a favor de opcional)** | CU5, Curso de Excepción 3a: "El animal no tiene progenitores registrados". CU6, Curso de Excepción 5a: verificación parcial. 2.2.5.2: "`id_madre` e `id_padre` [...] admiten valor nulo cuando el progenitor no se encuentra registrado". |
| **Código** | `AltaAnimal.cshtml.cs:96` sólo exige caravana y raza. Madre y padre viajan como `0` y se guardan `NULL`. El DDL los declara `NULL`. |

Acá el código está bien y el documento mal: con RF1.5 como está escrito sería
imposible cargar el primer animal del rodeo.

### A9. RF1.9 (actualización automática de categoría) no está implementado

| | |
|---|---|
| **Documentación** | RF1.9: "El sistema debe actualizar automáticamente la categoría cuando cambie la condición biológica del animal". CU3 lo referencia. CU18: "El número de partos se incrementa y puede modificar la categoría del animal". |
| **Código** | `CalcularCategoria` (`Controladora.cs:259`) se llama sólo desde el botón "Calcular Categoria" del alta (`AltaAnimal.cshtml.cs:57`) y como valor por defecto si el usuario no eligió (`:119`). `ModificarAnimal.cshtml` no tiene ese botón y exige categoría manual (`ModificarAnimal.cshtml.cs:91`). Nada recalcula cuando el animal cruza los 12 o 15 meses. |

`AplicaCategoria` (`Controladora.cs:307`) —el método que detectaría el
desfasaje, documentado en el diccionario— está escrito pero nunca se invoca.
Una ternera cargada hace 13 meses sigue figurando "Ternera" indefinidamente, y
el filtro por categoría de CU7 devuelve datos viejos.

### A10. CU3 "Modificar Datos de Animal" está implementado a medias

CU3 dice "Permite actualizar la información de un animal previamente
registrado" y "Se mantienen las mismas validaciones de obligatoriedad de campos
que en el alta". El formulario de modificación no permite editar los atributos
de la especialización: `numeroPartos`, `estadoProductivo`, `estadoReproductivo`
(Hembra) ni `enPie` (Macho). `Controladora.ModificarAnimal` (`:156`) ni
siquiera los recibe como parámetro, y `pHembra.ModificarHembra` está sin
cablear (ver A4).

Como `numero_partos` es lo que distingue Novilla de Vaca según el seed de
`categorias` (`tambo_m0_m1.sql:111-113`), un error de carga en el alta queda
sin forma de corregirse.

### A11. CU7 filtra por raza, criterio que RF1.10 no pide

RF1.10 y CU7 paso 3 enumeran "número de caravana, categoría, estado o rango
etario". El código agrega el filtro por raza (`Controladora.cs:461`,
`BuscarAnimales.cshtml`). Es una mejora razonable, pero no está documentada.

### A12. Literales de estado sin normalizar, ya inconsistentes entre sí

| | |
|---|---|
| **Documentación** | 2.2.5.4 tabla `hembras`: `estado_productivo` ∈ {Sin lactancia, en lactancia, seca}; `estado_reproductivo` ∈ {vacia, servida, prenada}. Pero CU12 escribe "seca", CU18 escribe "Lactancia", CU16 escribe "servida"/"preñada"/"vacía". |
| **Código** | El alta escribe `"Sin lactancia"` y `"Vacía"` (`AltaAnimal.cshtml.cs:170`). `EstaEnLactancia` compara contra `"En lactancia"` (`Controladora.cs:557`). |

Hoy ningún camino del código escribe `"En lactancia"`, así que
`EstaEnLactancia` devuelve `false` siempre. Cuando se implemente el Módulo 2
—CU8 paso 2: "El sistema carga automáticamente la lista de los animales con
estado productivo 'en lactancia'"— el lote de ordeñe va a salir vacío.

### A13. `Macho.EnPie`: la semántica documentada y la de la pantalla no coinciden

Diccionario de Clases: `mEnPie` — "Indica si el toro integra físicamente el
rodeo. Vale falso en el toro de catálogo, que sólo aporta material genético".
La etiqueta de la pantalla (`AltaAnimal.cshtml:76`) dice "En pie (integra el
rodeo como reproductor)". Con esa lectura, un ternero macho recién nacido queda
`en_pie = 0`, indistinguible de un toro de catálogo. No rompe nada hoy porque
las pajuelas son del Módulo 5, pero sí cuando ese módulo consulte
`BuscarPajuelasXToro`.

### A14. `bd/LEEME.md` no coincide con el código que documenta

El LEEME transcribe `private static string contrasena = "CAMBIAR_POR_LA_CONTRASENA";`
y `pConexion.cs:15` tiene `private static string contrasena = "";`.

### A15. CU6: dos detalles del caso de uso que no están

- Curso de Excepción 5a pide informar "hasta qué nivel de la ascendencia pudo
  comparar". El código sólo dice "no tiene toda su ascendencia registrada"
  (`VerificarConsanguinidad.cshtml.cs:77-82`).
- Curso Alternativo 3a (elegir una pajuela como reproductor) no existe: el
  selector sólo ofrece machos del rodeo. Esperable, porque las pajuelas son del
  Módulo 5, pero conviene dejarlo anotado como deuda.

### A16. El orden "persistir y después actualizar la caché" no se respeta

2.2.3 y 2.2.7: la Controladora *"valida contra sus listas static en memoria,
delega la escritura en pControladora y recién después actualiza la caché"*.
`ModificarAnimal` (`Controladora.cs:166-173`) muta el objeto en memoria **antes**
de llamar a `Persistencia.ModificarAnimal`. Si la escritura falla, la caché
queda con datos que no están en la base.

### A17. Sección 2.3 "Pruebas" pendiente, sin infraestructura de test

No hay proyecto de test ni un solo caso automatizado en el repositorio. Es
coherente con que 2.3 figure como "Pendiente", pero el Plan de Testing del
anteproyecto ya está comprometido y no hay dónde empezar a escribirlo.

---

## Parte B — Correcciones propuestas

Ordenadas por prioridad. Las tres primeras bloquean cualquier uso real en el
tambo.

### Bloqueantes

**B1. Implementar la sesión de verdad (resuelve A1).**
Autenticación por cookie: `AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)`,
`app.UseAuthentication()` **antes** de `UseAuthorization()`, `HttpContext.SignInAsync`
en el handler de Login y `SignOutAsync` en un botón "Cerrar Sesión" en
`_Layout.cshtml`. Proteger todo con
`options.Conventions.AuthorizeFolder("/PagesAnimal")` y
`AllowAnonymousToPage("/PagesSeguridad/Login")`, y que la raíz redirija al
login cuando no hay sesión. Sin esto, RF0.1 y el RNF de Seguridad no se cumplen
y la pre-condición de los 34 CU restantes es falsa.

**B2. Consultas parametrizadas en toda la capa de persistencia (resuelve el riesgo abierto por A6/A14).**
Hoy todo el SQL se arma concatenando strings: `pAnimal.cs:101`, `:132`, `:146`,
`pHembra.cs:32`, `:44`, `pMacho.cs:32`. Dos problemas concretos, no teóricos:
una caravana o un motivo de baja con apóstrofo rompe el comando, y el campo es
inyectable. Pasar a `MySqlCommand` con `Parameters.AddWithValue`. Esto además
es lo que sostiene el RNF de Fiabilidad ("garantizar la integridad de los datos
registrados, evitando pérdidas de información ante fallos comunes").

**B3. Sacar las credenciales del código fuente.**
`pConexion.cs:11-15` tiene usuario y contraseña de MySQL hardcodeados;
`Controladora.cs:22-23` tiene el usuario (`sofia`) y la contraseña (`tambo2026`)
de la encargada en texto plano. Los tres están versionados en un repositorio.
Mover la cadena de conexión a `appsettings.json` / user-secrets, y guardar la
contraseña de la encargada como hash (`PasswordHasher<T>` de ASP.NET Core), no
en claro. La contraseña actual ya quedó en el historial de git: conviene
cambiarla.

### Correctitud

**B4. Transacción en el alta de animal.**
`Controladora.AltaAnimal:132-146` inserta en `animales` y después en
`hembras`/`machos` **ignorando el resultado del segundo INSERT**. Si el segundo
falla queda una fila huérfana en `animales` que `pAnimal.ListarAnimales:30` va a
clasificar como Macho, porque decide por `estado_productivo IS NULL`. Envolver
ambas escrituras en una transacción y verificar el retorno.

**B5. Discriminar Hembra/Macho por la tabla, no por si un campo vino NULL.**
`pAnimal.cs:30` usa `if (fila["estado_productivo"] != DBNull.Value)`. Traer
`h.id_animal AS id_hembra` y `m.id_animal AS id_macho` en el SELECT y decidir
por esos campos. Es lo que evita que un dato incompleto cambie el sexo de un
animal.

**B6. Dejar que MySQL asigne los identificadores (resuelve A6 y A7).**
Sacar `id_animal` del INSERT y recuperar `LAST_INSERT_ID()`. Como está,
`MAX(id)+1` reutiliza el id después de un DELETE —y el DELETE existe, ver A2— y
en dos altas concurrentes produce clave duplicada. Con este cambio desaparecen
`ProximoHembraId`/`ProximoMachoId` y el campo "ID" readonly del formulario de
alta (`AltaAnimal.cshtml:16-19`) deja de tener sentido.

**B7. Cerrar la conexión siempre.**
`pConexion.EjecutarComando` y `EjecutarConsulta` llaman a `Cerrar()` después del
comando, pero si el comando lanza, el `catch` re-lanza sin cerrar: la conexión
se filtra. Usar `using` sobre la conexión y el comando, o `try/finally`. Con el
pool de MySQL agotándose, el sistema deja de responder —directamente en contra
del RNF de Disponibilidad (95%).

**B8. Normalizar los literales de estado (resuelve A12).**
Constantes en el dominio para `"En lactancia"` / `"Sin lactancia"` / `"Seca"` y
`"Vacía"` / `"Servida"` / `"Preñada"`, usadas tanto al escribir como al
comparar, y corregir `EstaEnLactancia`. Hacerlo **antes** de empezar el Módulo 2:
si no, CU8 arranca con el lote de ordeñe vacío y el error va a parecer un
problema de la consulta.

### Funcionalidad de negocio

**B9. Implementar RF1.9 de verdad (resuelve A9).**
Dos partes: (a) en el listado y en el detalle, comparar la categoría almacenada
contra `CalcularCategoria` usando `AplicaCategoria` —que ya está escrito— y
marcar visualmente las que quedaron desactualizadas, con una acción para
aceptar la recalculada; (b) agregar el botón "Calcular Categoría" también en
Modificar. Es la diferencia entre un padrón que se mantiene solo y uno que
envejece: en un tambo, cada mes hay terneras que pasan a novillas.

**B10. Completar CU3 (resuelve A10).**
Permitir editar `numeroPartos` y `enPie` desde Modificar, exponiendo
`ModificarHembra` en la Controladora y agregando el equivalente para Macho. Es
el único camino para corregir un dato de carga que además determina la
categoría. Los estados productivo y reproductivo conviene dejarlos fuera del
formulario: los van a manejar los CU12, CU16 y CU18.

**B11. Validar el árbol genealógico al guardar.**
Hoy `ModificarAnimal` (`Controladora.cs:156`) acepta poner como madre a la
propia hembra, o a una hija, creando un ciclo. Agregar tres validaciones:
1. padre y madre distintos del propio animal;
2. el progenitor elegido no puede estar en la descendencia del animal;
3. la fecha de nacimiento del progenitor debe ser anterior a la de la cría, con
   un margen de edad mínima al servicio (del orden de 15 meses para la madre).

Esto es lo que le da valor real a CU5 y CU6 para el tambo: con ciclos o con una
madre más joven que su cría, la verificación de consanguinidad informa
cualquier cosa, y la usuaria toma la decisión de servicio sobre un dato falso.

**B12. Decidir qué pasa con "Eliminar" (resuelve A2).**
Recomendación: quitar la página, el método `EliminarAnimal` y el `DELETE`, y
dejar sólo la baja lógica de CU4. Es lo que dice la regla de negocio y lo que
necesita el establecimiento —el historial de una vaca vendida sigue haciendo
falta para el linaje de sus hijas y para los reportes del Módulo 6—. Si se
prefiere conservarlo, hay que escribir el caso de uso, sumarlo al Diccionario
de Clases y extender `EsProgenitor` a todas las tablas que referencien
`id_animal`.

**B13. Que la baja pida la fecha en lugar de asumir hoy.**
`Controladora.cs:192` y `pAnimal.cs:148` fuerzan `DateTime.Now`. En el tambo la
baja se carga cuando se puede, no cuando ocurre: una vaca que murió el jueves
se registra el lunes. CU4 no exige que sea la fecha del día. Agregar el campo
al formulario, validando que no sea futura ni anterior al nacimiento.

**B14. Validaciones de coherencia en el alta que el uso real va a pedir.**
- Fecha de nacimiento no anterior a un límite razonable (~30 años).
- `numeroPartos > 0` incompatible con una edad menor a ~20 meses.
- Aviso no bloqueante al dar de alta una hembra con partos y sin madre
  registrada (típico de la carga inicial del rodeo, conviene que quede visible).

### Calidad de código

**B15. Devolver la validación al model binding.**
`AltaAnimal.LeerFormulario` (`:134`) y el resto de los handlers leen
`Request.Form[...]` a mano, así que los `[Required]` y `[Range]` de los
PageModel no se aplican nunca —son decorativos—. Además un valor no numérico en
un campo oculto hace explotar `Convert.ToInt32` con un 500
(`AltaAnimal.cshtml.cs:137`, `EliminarAnimal.cshtml.cs:20`,
`BuscarAnimales.cshtml.cs:50`). Usar `[BindProperty]` + `ModelState.IsValid`, o
como mínimo `int.TryParse`.

**B16. Preservar lo tipeado cuando falla la validación en Modificar.**
`ModificarAnimal.cshtml:24` y `:29` pintan `value="@Model.animal.NumCaravana"`,
o sea el valor de la caché. Al volver con un error de validación el usuario
pierde lo que había editado.

**B17. Romper la dependencia Persistencia → Dominio (resuelve A5).**
Que `pAnimal` resuelva raza y categoría con sus propias consultas —o que las
reciba por parámetro— en vez de instanciar `Controladora`. Es lo que muestra el
diagrama de persistencia, y es lo que hace sostenible el crecimiento a seis
módulos: hoy son tres clases enredadas, con el esquema completo serían veinte.

**B18. Dejar constancia del alcance de la caché `static`.**
`Controladora.cs:9-13`: las listas son `static`, o sea compartidas por todas las
peticiones del proceso. Con una sola usuaria funciona y está documentado como
decisión de diseño, pero si en algún momento hay dos navegadores abiertos, una
petición puede reemplazar `mListaAnimales` mientras otra la está recorriendo.
Mínimo: anotarlo como limitación asumida en 2.2.3. Mejor: pasar las listas a
estado por petición (`AddScoped`).

**B19. Levantar el andamiaje de pruebas (A17).**
Un proyecto xUnit con los casos que ya se pueden testear sin base: `CalcularEdadMeses`,
`CalcularCategoria` en los bordes (12 y 15 meses), `ListarAscendencia`,
`BuscarAncestroComun` y `VerificarConsanguinidad`. Son lógica pura de la
Controladora y son exactamente las reglas que el análisis (2.1) identifica como
el desafío del dominio.

### Correcciones al documento

**B20.** Reescribir **RF1.5** y la Regla de Negocio de **CU2**: el registro de
padre y madre es **opcional**. Es lo consistente con CU5-3a, con CU6, con el
modelo relacional (2.2.5.2), con el DDL y con la realidad de la carga inicial
del rodeo. (Resuelve A8.)

**B21.** Agregar `raza` a **RF1.10** y al paso 3 de **CU7**. (Resuelve A11.)

**B22.** Incorporar al **Diccionario de Clases**: `EsProgenitor`,
`ListarAscendencia`, `BuscarAncestroComun` y `FiltrarAnimales`. Quitar
`ProximoHembraId` y `ProximoMachoId`, o justificar para qué existen.
(Resuelve A3 y A7.)

**B23.** Unificar en todo el documento la escritura de los estados: CU12 dice
"seca", CU16 "servida/preñada/vacía", CU18 "Lactancia", y 2.2.5.4 usa otra
grafía. Fijar un único juego de valores y usarlo en los CU, en el diccionario y
en el código. (Resuelve A12 junto con B8.)

**B24.** Definir sin ambigüedad qué significa `en_pie` —"reproductor activo del
rodeo" o "no es toro de catálogo"— y alinear la etiqueta de
`AltaAnimal.cshtml:76` con esa definición. (Resuelve A13.)

**B25.** Actualizar `bd/LEEME.md` para que coincida con `pConexion.cs`.
(Resuelve A14.)

**B26.** Documentar en 2.2.4 que la capa de persistencia no debe instanciar la
Controladora, o corregir el diagrama para reflejar lo que hace el código.
Recomendado lo primero, junto con B17. (Resuelve A5.)

**B27.** Anotar como deuda explícita del Módulo 1 los dos puntos de CU6 que
quedan atados al Módulo 5: la selección de pajuela como reproductor y el
detalle del nivel de ascendencia comparado. (Resuelve A15.)

---

## Resumen de prioridades

| Prioridad | Ítems | Por qué |
|---|---|---|
| Bloqueante | B1, B2, B3 | El sistema no autentica, el SQL es inyectable y las credenciales están versionadas. |
| Antes del Módulo 2 | B8, B5, B6, B4, B7 | Los estados sin normalizar y los ids manuales rompen el ordeñe apenas se implemente CU8. |
| Valor para el tambo | B9, B11, B10, B13 | Categoría que se mantiene sola, linaje confiable, corrección de datos y fecha de baja real. |
| Consolidación | B12, B14, B15, B16, B17, B18, B19 | Deuda técnica y alineación con la arquitectura documentada. |
| Documento | B20 – B27 | Coherencia interna del anteproyecto y del proyecto. |
