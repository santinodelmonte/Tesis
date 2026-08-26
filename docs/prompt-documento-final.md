# Prompt — del anteproyecto y el proyecto al documento final

Este archivo **es** el prompt. Para arrancar una sesión de trabajo alcanza con:

> Leé `docs/prompt-documento-final.md` y seguí con la Fase 3, sección 2.3.

No hace falta pegar nada más: acá están el objetivo, la referencia, las reglas y
el estado de cada sección.

---

## 1. Objetivo

Llegar a **un solo documento** con la forma de `EjemploTesis.pdf` —la tesis que
dio el tutor como modelo—, partiendo de los dos que ya existen:

```
Anteproyecto_v6.docx  ──┐
                        ├──►  Tesis.docx  (1. Anteproyecto / 2. Proyecto / Glosario / Bibliografía / Anexo)
Proyecto_v6.docx      ──┘
```

Cuatro fases, **en este orden y sin solaparlas**:

- **Fase 1 — Terminar el código.** El Módulo 7 completo y lo que arrastra. Hasta
  que el sistema no esté terminado no se toca la documentación.
- **Fase 2 — Reconciliar las tres patas.** Anteproyecto, proyecto y código tienen
  que decir exactamente lo mismo. **Es innegociable y bloquea todo lo que sigue.**
- **Fase 3 — Cerrar el Proyecto.** Escribir las secciones 2.3 a 2.9, hoy con la
  palabra «Pendiente.» y nada más.
- **Fase 4 — Unificar.** Renumerar el anteproyecto como `1.x`, concatenarlo con
  el proyecto, rehacer índice, portada y numeración de páginas.

**La regla de las fases es la que ordena todo lo demás.** El documento describe
el sistema que existe; documentar un sistema que todavía se está moviendo es
trabajo que hay que rehacer. Con el Módulo 7 en curso cambian el MER, los
diagramas de secuencia, el diccionario de clases, las pantallas del manual y lo
que se puede probar. Se termina de programar, y recién entonces se escribe.

Lo único que **sí** se puede adelantar en paralelo está en el punto 10: son las
tres cosas que no dependen del código.

### El flujo, de una mirada

```
FASE 1   programar                     Modulo 7 + cache static          ustedes / Claude Code local
           │
           ▼
FASE 2   reconciliar                   auditoria-tres-vias.md           Claude Code
           │                           anteproyecto = proyecto = codigo  (consultas ─► ustedes)
           ▼
FASE 3   refactor generar|editar       docs/generar.py                  Claude Code
           │
           ├─ por seccion ─┐
           │   paso 1  generar         .md y .png en docs/              Claude Code
           │   paso 1b capturas        docs/capturas/*.png              Claude Code local (app corriendo)
           │   revisar                 se mira el .md y el .png         ustedes / Cowork
           │   paso 2  editar          Proyecto_v6.docx                 Claude Code
           │   verificar               se abre el Word                  ustedes
           └───────────────┘
           ▼
FASE 4   unificar                      Tesis.docx                       Claude Code
           │
           ▼
         actualizar indice             abrir en Word, F9                ustedes  ← unico paso manual ineludible
```

---

## 2. La referencia manda

El ejemplo del tutor tiene 231 páginas y esta estructura, según su índice:

| | Sección | Páginas |
|---|---|---|
| **1** | **Anteproyecto** | 7–60 |
| 1.1 | Introducción | 7 |
| 1.2 | Presentación del Cliente | 8 |
| 1.3 | Presentación del Problema | 12 |
| 1.4 | Lista de Necesidades | 13 |
| 1.5 | Actores Involucrados | 14 |
| 1.6 | Objetivos | 15 |
| 1.7 | Lista de Requerimientos | 16 |
| 1.8 | Descripción del Entorno | 18 |
| 1.9 | Alcances y Limitaciones | 21 |
| 1.10 | Estudio de Alternativas | 23 |
| 1.11 | **Estimación del esfuerzo** | 29 |
| 1.12 | Análisis de riesgo | 35 |
| 1.13 | Plan de Proyecto | 41 |
| 1.14 | Compromiso de trabajo | 59 |
| **2** | **Proyecto** | 61–226 |
| 2.1 | Análisis | 61 |
| 2.2 | Diseño | 62–151 |
| 2.3 | **Pruebas** | 152–168 (17 pág.) |
| 2.4 | **Manual de Usuario** | 169–215 (47 pág.) |
| 2.5 | **Deployment** | 216 (1 pág.) |
| 2.6 | **Política de Seguridad y Respaldos** | 217 (1 pág.) |
| 2.7 | **Plan de contingencia** | 218 (1 pág.) |
| 2.8 | **Grado de satisfacción del cliente** | 219 (1 pág.) |
| 2.9 | **Conclusiones** | 220–226 (7 pág.) |
| | Glosario | 227 |
| | Bibliografía utilizada | 228 |
| | Anexo | 229 |

**El criterio de profundidad lo fija el ejemplo, no nuestro apetito.** Si el
ejemplo resuelve el Deployment en una página, nosotros también. Las páginas de
arriba son el presupuesto de cada sección, y el detalle de cómo está armada cada
una está leído y anotado abajo.

Esto vale sobre todo para **2.3 Pruebas**: el ejemplo documenta pruebas
**manuales**, sin una sola automatizada. Nosotros igual, y si algún documento
nuestro promete pruebas automatizadas, **se corrige el documento**.

### Qué hay realmente en cada sección del ejemplo

`EjemploTesis.pdf` **ya está en el repo** (raíz). Son 231 páginas y esto es lo que
trae cada una de las secciones que hay que escribir. Está leído, no supuesto.

| Sección | Imágenes | Cómo está armada |
|---|---|---|
| 2.3 Pruebas | **69** | Agrupada por función, no por caso de uso. Dos formatos mezclados: **tablas** de variantes de entrada (`Usuario / Contraseña / Resultado esperado / Resultado` con «Ok»), y **«Prueba: … / Resultado:» + captura**. Sin numerar los casos, sin precondiciones ni postcondiciones. |
| 2.4 Manual | **123** | Tiene **su propio índice interno**, con numeración propia (`1. Introducción`, `2. Manejo de Usuarios`, `2.1 Inicio de Sesión`…) y sus propios números de página. Es un documento adentro del documento. |
| 2.5 Deployment | 0 | **No es un instructivo de instalación.** Es qué necesita el servidor, qué necesita el usuario para acceder (navegadores), y quién opera el sistema y con qué capacitación. |
| 2.6 Seguridad | 0 | Roles y autenticación, cómo se guardan las contraseñas, cómo se evita la inyección SQL, el guardado transaccional, y los respaldos con **periodicidad y dónde se guarda cada copia**. |
| 2.7 Contingencia | 0 | **Dos párrafos.** Remite al análisis de riesgos ya hecho y dice que el equipo queda disponible después de la entrega. |
| 2.8 Satisfacción | 0 | **No es una encuesta.** Es el relato de la relación con el cliente a lo largo de las iteraciones y cómo eso acercó el producto a lo que esperaba. |
| 2.9 Conclusiones | 0 | Diez subtítulos, y el bloque más largo son **los riesgos uno por uno**. Ver el detalle en la Fase 3. |
| Glosario | — | **Cinco entradas numeradas.** Cuatro técnicas y una del dominio (*tanda*). |
| Bibliografía | — | Ocho entradas con formato de cita completo, con URL y edición. Incluye **Pressman 7ma ed. 2010** y **Elmasri y Navathe 5ta ed. 2007**. |
| Anexo | 3 | **Las planillas de Excel que el sistema vino a reemplazar**, cada una con un párrafo que explica para qué se usaba. |

**Dos cosas que nadie habría adivinado y cambian el trabajo:**

1. **La sección de Pruebas lleva 69 capturas.** El guion de `docs/guion-capturas.md`
   cubre sólo el manual: hay que ampliarlo. En el ejemplo, la evidencia de cada
   prueba **es** una captura del resultado.
2. **El Manual de Usuario tiene índice propio.** No es una sección más: se arma como
   un documento aparte, con su numeración y sus páginas, y después se inserta.

---

## 3. Dos pasos, no uno: generar y después editar

El trabajo sobre el documento tiene **dos etapas separadas, con dos comandos y dos
commits distintos**:

```
paso 1   generar      bd/CreacionDb.sql + Tesis/   ──►   docs/*.md  y  docs/diagramas/*.png
paso 2   editar       docs/*.md + docs/diagramas/  ──►   Proyecto_v6.docx
```

**El paso 2 no genera nada.** Toma los archivos que ya están en `docs/` y los
vuelca al documento. Si un diagrama está mal, se arregla en el paso 1 o a mano
sobre el `.drawio`, se vuelve a exportar, y el paso 2 se limita a colocar el
archivo que encuentre.

Por qué separados, y no como está hoy:

- **Los diagramas se pueden mirar antes de publicarlos.** Un error del generador
  hoy entra derecho al `.docx`; con la separación se ve primero.
- **Los diagramas se pueden retocar a mano y el retoque sobrevive.** Un MER de
  veinticuatro tablas casi nunca queda legible con el acomodo automático: se abre
  el `.drawio`, se mueven las cajas y se vuelve a exportar. El paso 2 usa lo que
  haya, no lo vuelve a calcular.
- **Corregir un párrafo no obliga a regenerar cuarenta y nueve diagramas de
  secuencia.**

**El precio de separar, y cómo se paga.** Al desacoplar aparece la posibilidad de
armar el documento con diagramas viejos. El paso 2 tiene que **avisar**: antes de
escribir, comparar la fecha de cada artefacto de `docs/` contra la de su fuente
—`bd/CreacionDb.sql`, `Tesis/`— y cortar con un mensaje claro si el artefacto es
más viejo. No es opcional: es lo único que reemplaza a la garantía que daba el
acoplamiento.

**Lo que no cambia.** Nada se escribe dos veces: si un dato está en el código, en
el SQL o en un `.md` de `docs/`, la sección lo lee de ahí, no se transcribe. Y
nunca se edita a mano en Word una sección que el paso 2 escribe: se pierde en la
próxima corrida. Si hay que cambiarla, se cambia el `.md`.

**Refactor pendiente.** Hoy `docs/editar_proyecto.py` hace las dos cosas: importa
`modelo_datos`, `diccionario_clases` y `render_casos_de_uso` y vuelve a derivar el
contenido mientras escribe el documento. Los tres módulos ya saben escribir su
`.md` —`modelo-datos-v6.md`, `diccionario-clases-v6.md`, `casos-de-uso-v6.md`—,
así que la separación es corta: un `docs/generar.py` que los corra a todos junto
con los de `docs/diagramas/`, y un `editar_proyecto.py` que lea de disco en lugar
de importar. Hacerlo **al empezar la Fase 3**, antes de escribir las secciones
nuevas, para que nazcan con la forma correcta.

Las secciones nuevas siguen el mismo camino: 2.3 a 2.9 se escriben como markdown
en `docs/`, y el paso 2 las vuelca. No se escriben directamente en Word.

---

## 4. Estado de partida

`Proyecto_v6.docx`, contenido por sección:

| Sección | Estado |
|---|---|
| 2.1 Análisis | escrita |
| 2.2.1 Diagramas de casos de uso | generada |
| 2.2.2 Casos de uso (49) | generada |
| 2.2.3 Diagrama de Dominio | generada |
| 2.2.4 Diagrama de Persistencia | generada |
| 2.2.5 Modelo de Datos (MER, normalización, claves, integridad) | generada |
| 2.2.6 Diagramas de Secuencia (49) | generada |
| 2.2.7 Diccionario de Clases | generada |
| **2.3 – 2.9** | **«Pendiente.»** |

Insumos que ya existen y hay que aprovechar, no rehacer:

- `docs/flujos-de-prueba.md` — guion manual completo del sistema, con datos
  concretos apoyados en `bd/DatosPrueba.sql`. Es el esqueleto de **2.3**.
- `docs/estilos-y-accesibilidad.md` — decisiones de interfaz con sus contrastes
  medidos. Alimenta **2.4** y la parte de accesibilidad.
- `docs/catalogo-casos-de-uso.md` — los 49 CU con su módulo y su pantalla. Es el
  índice natural del **manual de usuario**.
- `bd/LEEME.md` — puesta en marcha con XAMPP. Es la base de **2.5**.
- `docs/pendientes-tecnicos.md` — los dos puntos donde el código todavía no
  acompaña al documento. Alimenta **2.9**.

---

## 5. Fase 1 — Terminar el código

**Nada de documentación hasta que esta fase esté cerrada** (salvo lo del punto 10).

El documento final describe un sistema de ocho módulos terminados, no siete y una
promesa. **Master ya avanzó y parte del Módulo 7 está construido**, así que lo que
falta es menos de lo que decía este prompt:

| | Estado |
|---|---|
| CU44–CU47, los cuatro reportes | **hechos** — `Tesis/Pages/PagesReportes/`, con `GeneradorPdf.cs` y `GeneradorExcel.cs` |
| CU48, integración con el bot de Telegram | **falta**: no hay nada de Telegram en el código |
| CU49, resumen diario | **falta**: no hay proceso programado en `Program.cs` |
| Las dos tablas del Módulo 7 | **faltan**: la última tabla de `bd/CreacionDb.sql` sigue siendo `configuracion` |
| Las pantallas de configuración del bot | **faltan** |

Lo que queda, en este orden: crear las dos tablas, el bot y el envío de las alertas
de RF7.6 (sanidad pendiente, partos próximos, tactos pendientes, secados próximos,
stock crítico, vencimiento de insumos, fin del período de descarte de leche), el
resumen diario CU49 como proceso programado, y la pantalla de configuración.

> **CU49 obliga a tocar la caché `static` de la Controladora.** Está explicado en
> `docs/pendientes-tecnicos.md`: hay diecinueve listas `static` compartidas por
> todas las peticiones del proceso, y un proceso programado que corre solo a hora
> fija va a reemplazarlas mientras la encargada carga un parto. La limitación
> asumida se sostiene mientras el sistema sea de un usuario y sin procesos de
> fondo; el Módulo 7 termina con las dos condiciones. El arreglo no es borrar
> `static` veintiuna veces: hay que llamar a `Refrescar()` en el constructor,
> porque la mayoría de los métodos leen las listas sin refrescar y hoy funcionan
> sólo porque son `static`.

**Cierre de la fase.** Correr el paso 1 —los generadores— y revisar los artefactos:
los diagramas de secuencia del Módulo 7 ahora se leen del código, el MER pierde el
relleno de «proyectada» y el diccionario incorpora las clases nuevas. Ahí termina
la Fase 1. El documento todavía no se toca: primero la reconciliación.

---

## 6. Fase 2 — Reconciliar las tres patas

**Regla, sin excepciones: el anteproyecto, el proyecto y el código dicen
exactamente lo mismo.** Un tribunal que encuentra una diferencia entre lo que el
documento promete y lo que el sistema hace deja de creerle al resto del documento.

**Ante la duda manda el código, pero no se resuelve solo: se consulta.** El código
es la única de las tres patas que no puede mentir —hace lo que hace—, así que es
el árbitro natural. Pero que el código gane no significa que el agente decida por
su cuenta: una diferencia puede ser código que se apartó de lo acordado con la
clienta, y eso se arregla en el código, no tapándolo en el documento. Cada
divergencia se anota, se propone una resolución y **se pregunta antes de aplicarla**.

### Lo que ya está verificado

Anteproyecto y proyecto **ya coinciden**: el anteproyecto define **74
requerimientos funcionales** y los 49 casos de uso los referencian a todos. No hay
ningún RF huérfano ni ningún caso de uso que invente un requerimiento que no
existe. Eso lo dejó resuelto la v6 de los dos documentos.

| Módulo | RF |
|---|---|
| 0 Seguridad, Acceso y Configuración | 3 |
| 1 Gestión de Animales y Genética | 15 |
| 2 Control de Producción | 13 |
| 3 Gestión Reproductiva | 13 |
| 4 Gestión Sanitaria | 9 |
| 5 Control de Insumos y Stock | 10 |
| 6 Tablero, Indicadores y Apoyo a la Decisión | 4 |
| 7 Reportes y Notificaciones | 7 |
| **Total** | **74** |

**La divergencia vive en la tercera pata.** Las secciones de diseño del proyecto se
generan leyendo el código, así que ésas están sincronizadas por construcción. El
anteproyecto no: está escrito en futuro, antes de programar, y nadie lo volvió a
mirar contra lo que quedó hecho.

### El método

Producir `docs/auditoria-tres-vias.md`, una fila por divergencia:

| Qué dice el anteproyecto | Qué dice el proyecto | Qué hace el código | Quién tiene razón | Resuelto |
|---|---|---|---|---|

Se recorre en este orden, que va de lo más verificable a lo más interpretable:

1. **Los 74 RF, uno por uno.** Para cada uno, encontrar en el código dónde está
   resuelto. El que no aparezca es una de dos cosas: un requerimiento que no se
   implementó, o uno que se implementó distinto. Las dos hay que decidirlas.
2. **Al revés: lo que el código hace y ningún RF describe.** Es el más fácil de
   pasar por alto y el que peor queda en la defensa —una pantalla que nadie pidió—.
   Se recorre el listado de pantallas contra el catálogo de casos de uso.
3. **Los parámetros de configuración.** El anteproyecto los menciona sueltos, la
   pantalla tiene una lista concreta. Tienen que ser la misma lista.
4. **Alcance y limitaciones.** Lo que el anteproyecto declara afuera tiene que
   seguir afuera, y lo que declara adentro tiene que estar.
5. **Herramientas y arquitectura.** Framework, base de datos, acceso a datos,
   hosting, notificaciones: lo elegido en el anteproyecto contra lo que hay en
   `Tesis.csproj`, `appsettings.json` y las capas.
6. **Las iteraciones.** Las seis iteraciones definidas describen un orden de
   construcción. Si se construyó en otro orden, se dice el que fue.
7. **Los riesgos.** Los que se materializaron se cuentan en 2.9; los que no,
   quedan como estaban.

### Cómo se aplica

El anteproyecto se corrige con `docs/editar_anteproyecto.py`, que ya existe y ya
hizo este trabajo una vez —`docs/cambios-anteproyecto-v6.md` es el registro de esa
pasada—. Las correcciones de esta fase se anotan igual, en un
`cambios-anteproyecto-v7.md`, para que quede asentado qué se cambió y por qué.

**Un anteproyecto corregido después de programar no es hacer trampa.** Es un
documento vivo, y la alternativa —dejarlo diciendo algo que el sistema no hace— es
peor. Lo que no se puede hacer es cambiarlo en silencio: por eso el registro.

**Y hay dónde contarlo: el ejemplo ya resolvió cómo.** En sus Conclusiones, dentro
del riesgo de errores de especificación, dice qué requerimientos se quitaron, **con
fecha y con la reunión donde se acordó con el cliente**. Lo que encuentre esta
auditoría se cuenta ahí, en el punto 4 de 2.9, con ese mismo formato. Una
divergencia explicada así no resta: muestra que el proyecto se gestionó.

---

## 7. Fase 3 — Cerrar 2.3 a 2.9

Primero el refactor del punto 3. Después, para **cada** sección, siempre el mismo
procedimiento:

1. Leer la sección equivalente en `EjemploTesis.pdf` y anotar su forma.
2. Escribir `docs/<seccion>.md` desde las fuentes que se listan abajo.
3. Revisarla en el `.md`, que es donde se lee cómodo. Commit.
4. **Aparte**, correr el paso 2 y verificar en el `.docx` que quedó donde va y con
   el formato de las secciones vecinas. Commit.

### 2.3 Pruebas — 17 páginas, 69 capturas

Fuente: `docs/flujos-de-prueba.md` y el sistema corriendo con `bd/DatosPrueba.sql`.

**La forma la fija el ejemplo, y es más simple de lo que uno tiende a hacer.**
Arranca con un párrafo que remite al plan de testing del Plan de SQA, y después
agrupa **por función**, no por caso de uso ni por módulo. Cada grupo usa uno de dos
formatos:

- **Tabla**, cuando lo que se prueba son variantes de entrada. Columnas: los campos
  que se cargan, *Resultado esperado*, *Resultado*. La última dice «Ok».
- **`Prueba:` una línea de qué se hizo, `Resultado:` una captura.** La captura *es*
  la evidencia. Por eso la sección tiene 69 imágenes.

**Nada de numerar los casos, ni precondiciones, ni postcondiciones, ni veredictos.**
El ejemplo no los usa y el criterio es seguirlo.

Cubrir con tabla las variantes que ya están en los flujos de prueba —el login, el
alta con caravana repetida, los litros fuera de rango, los rangos de fechas
invertidos— y con `Prueba:/Resultado:` los recorridos completos: el parto que abre
la lactancia y da de alta la cría, el tratamiento que saca a la vaca del tanque, la
inseminación que descuenta la pajuela. Ahí están las cinco piezas delicadas
—`CalcularCategoria` en los bordes, `ListarAscendencia`, `BuscarAncestroComun`,
`VerificarConsanguinidad`, `EstimarProduccionLactancia`—, probadas por lo que
muestran en pantalla.

Los errores encontrados van con su corrección, como pide el «Registro y Corrección
de Errores» del anteproyecto.

Antes de escribir: **corregir `docs/pendientes-tecnicos.md`**, que hoy afirma que el
anteproyecto compromete pruebas automatizadas. No las compromete, y el ejemplo
tampoco las tiene.

### 2.4 Manual de Usuario — 47 páginas, 123 capturas

**Es un documento adentro del documento.** En el ejemplo abre con **su propio
índice**, numerado aparte (`1. Introducción`, `2. Manejo de Usuarios`, `2.1 Inicio
de Sesión`, `3.1.1 Alta Empresa`…) y con sus propios números de página. Se arma como
pieza separada y se inserta.

Fuentes: `docs/catalogo-casos-de-uso.md` para el recorrido,
`docs/estilos-y-accesibilidad.md` para lo que significa cada color y cada
señalización de la interfaz.

**Las capturas se sacan con un script, no a mano.** Un recorrido de Playwright que
haga login, fije el tamaño de ventana, visite cada pantalla con el rodeo de
`bd/DatosPrueba.sql` cargado y escriba `docs/capturas/<pantalla>.png`. Sale parejo y
es repetible. Corre en la máquina de ustedes: el contenedor remoto no tiene `dotnet`
ni MySQL.

**El guion está en `docs/guion-capturas.md`**, con la pantalla, el animal del rodeo y
para qué sirve cada una. Va sin flechas ni números encima: la imagen limpia y **el
pie de figura cargando la explicación**.

Organizado por módulo, en el orden en que la encargada usa el sistema, no en el
orden en que se programó. Lo que el sistema calcula solo —la categoría, la fecha
probable de parto, la fecha de secado, el fin del período de descarte— tiene que
quedar explicado ahí, porque es lo que la usuaria no espera.

**Insertarlas en el Word es automático**: el mismo `d.imagen(ruta, pie)` que ya
coloca los 49 diagramas de secuencia.

### 2.5 Deployment — 1 página, sin imágenes

**No es un instructivo de instalación.** El ejemplo no pone un solo comando. Dice
tres cosas: qué necesita el servidor (versiones), qué necesita el usuario para
acceder (navegadores y conexión), y **quién opera el sistema y con qué preparación**
—que no hace falta formación técnica, que hay capacitación, manual y conocimiento
previo del trabajo—.

Para nosotros: .NET y MySQL con sus versiones, el hosting elegido en el
anteproyecto, los navegadores, **el uso desde el celular en el tambo**, y que la
encargada opera el sistema sin conocimientos técnicos apoyada en el manual y la
capacitación. Los scripts de `bd/` y el `dotnet run` son detalle de desarrollo: van
en `bd/LEEME.md`, no acá.

### 2.6 Política de Seguridad y Respaldos — 1 página, sin imágenes

El ejemplo cubre: autenticación y roles, cómo se guardan las contraseñas, cómo se
evita la inyección SQL, el guardado transaccional, y los respaldos con **su
periodicidad y dónde vive cada copia**.

Para nosotros, y **sin disimular nada**: el sistema es de un solo usuario con
credenciales fijas —se dice—; cómo se guarda la contraseña; que las consultas no se
arman concatenando; los respaldos automáticos del hosting, que ya están
comprometidos en el control preventivo del riesgo R9. Y el **token del bot de
Telegram**, que es un secreto: dónde vive y quién lo rota.

Como hace el ejemplo, cerrar con recomendaciones concretas a la clienta:
periodicidad del respaldo y dónde guardar la segunda copia.

### 2.7 Plan de contingencia — 1 página, sin imágenes

**Dos párrafos.** El ejemplo remite al análisis de riesgos ya hecho para lo que pueda
pasar durante el desarrollo, y para después de la entrega dice que el equipo queda
disponible hasta que el sistema esté completamente operativo.

Lo mismo, nombrando los riesgos que siguen vivos después de entregar: que se caiga
el hosting (R9) y que Telegram deje de responder (R6). **No inventar una lista
nueva** — el plan de contingencia es la respuesta a los riesgos que el anteproyecto
ya identificó.

### 2.8 Grado de satisfacción del cliente — 1 página, sin imágenes

**No es una encuesta, y esto corrige lo que decía antes este prompt.** El ejemplo
narra la relación con el cliente: las reuniones iniciales donde planteó sus
problemas, cómo la metodología elegida llevó a mostrarle cada iteración, cómo
priorizaban juntos los ajustes, y cierra con la instalación de prueba y su reacción.

Para nosotros, con lo que efectivamente pasó: las reuniones de relevamiento donde
salieron las reglas del tambo, las devoluciones que cambiaron el sistema, y lo que
la encargada dijo al usarlo. **Una frase suya, dicha de verdad, vale más que
cualquier métrica inventada** — pero no hace falta un instrumento formal.

### 2.9 Conclusiones — 7 páginas, sin imágenes

El ejemplo la organiza en **diez subtítulos**, y conviene copiar la estructura
entera:

| | Subtítulo | Qué va |
|---|---|---|
| 1 | Dinámica del equipo de trabajo | Cómo se repartió el trabajo, si hubo desacuerdos y cómo se resolvieron |
| 2 | Relación con el cliente | Disponibilidad, frecuencia del contacto |
| 3 | Relación con el tutor | Qué aportó |
| 4 | **Riesgos** | **Los catorce del anteproyecto, uno por uno**: si se dio o no, y si se dio, cómo se resolvió. Es el bloque más largo |
| 5 | Metodología utilizada | Si el ciclo de vida elegido fue la decisión correcta |
| 6 | Herramientas utilizadas | Cada una, y si cumplió |
| 7 | Trabajo colaborativo | Git y cómo se usó |
| 8 | Producto final | Qué cubre de lo que la clienta necesitaba |
| 9 | Puntos a mejorar | Lo que quedó corto, dicho de frente |
| 10 | Conclusión final | Qué dejó el proyecto |

**El punto 4 es el que hace que la sección valga.** El ejemplo recorre sus riesgos y
en el de errores de especificación hace algo que a nosotros nos sirve como modelo:
**dice qué requerimientos se quitaron, con fecha y con la reunión donde se acordó
con el cliente**. Ése es el lugar donde se cuenta lo que la Fase 2 encontró: si un
requerimiento cambió, acá se dice cuál, cuándo y por qué.

Lo que quede abierto de `docs/pendientes-tecnicos.md` va en el punto 9. Un documento
que reconoce sus límites se defiende mejor que uno que los esconde.

---

## 8. Fase 4 — Unificar y renumerar

**Decisión tomada: se renumera todo como el ejemplo.**

1. El anteproyecto pasa de títulos sueltos en mayúsculas —`INTRODUCCIÓN`,
   `PRESENTACIÓN DEL CLIENTE`, `LISTA DE NECESIDADES`— a `1.1`, `1.2`, `1.3`,
   siguiendo el mapa del punto 2. Sus subtítulos actuales bajan a `1.x.y`.
2. El proyecto queda como está: ya es `2.x`.
3. Portada, declaración de autoría, abstract, **palabras clave** (el ejemplo las
   tiene, el anteproyecto no), índice único, glosario, bibliografía y anexo.
4. El índice se genera, no se escribe. Con números de página reales.

> **El índice es el único paso que no se puede automatizar del todo.** El de
> `Proyecto_v6.docx` hoy es texto plano: no tiene campo TOC ni un solo `PAGEREF`,
> por eso tampoco tiene números de página. Los números dependen de la paginación,
> y la paginación la calcula un procesador de texto, no `python-docx`. La forma
> correcta es que `armar_tesis.py` inserte un campo `TOC \o "1-4" \h \z \u`
> de verdad y que **alguien abra el documento en Word una vez y actualice el
> campo** (clic derecho sobre el índice, «Actualizar campos», o F9). Es un clic al
> final de todo, pero hay que acordarse: si no, el índice sale en blanco.

Hacerlo con un script —`docs/armar_tesis.py`— que produzca `Tesis.docx` a partir
de los dos `.docx`. Es un paso 2, no un paso 1: no deriva nada del código, sólo
concatena y renumera lo que ya está escrito.

**Dos huecos del anteproyecto que aparecen al comparar con el ejemplo:**

- **`1.11 Estimación del esfuerzo`** (7 páginas en el ejemplo) no existe en el
  anteproyecto. Hay que escribirla o justificar su ausencia con el tutor.
- **Anexo**: el ejemplo pone **las planillas de Excel que su sistema vino a
  reemplazar**, una por página, cada una con un párrafo que explica para qué se
  usaba. El equivalente nuestro es directo: **los cuadernos y las planillas con que
  la encargada lleva hoy el tambo**, fotografiados. Es la prueba visual del problema
  que abre el documento, y cierra el círculo con la Presentación del Problema.

---

## 9. Cómo se escribe

**El texto del documento** sigue la voz del que ya está: afirmativo, concreto,
sin adjetivos de relleno. Nada de «cabe destacar», «es importante mencionar» ni
«la presente sección tiene como objetivo». Los números van con su fuente: no
«numerosas tablas» sino «las veintidós tablas». Si algo no se hizo, se dice que
no se hizo.

**Los commits** siguen la costumbre del repo: el título dice el hallazgo, no la
tarea —«El modelo entidad-relacion se lee del esquema, no se transcribe», no
«Actualizar MER»—, sin tildes, y el cuerpo explica por qué se hizo así y qué
apareció en el camino.

**El trabajo va en la rama `claude/thesis-project-document-prompt-nj7ffh`**, con
commits separados para el contenido (`.md`) y para el documento (`.docx`). No
mezclar los dos pasos en un commit: el diff del `.docx` es ilegible y arrastraría
al `.md` con él.

---

## 10. Lo que se puede adelantar mientras se programa

Estas no dependen del código y tienen dependencias externas lentas. Conviene
destrabarlas durante la Fase 1, no esperar a la Fase 3:

1. **~~Subir `EjemploTesis.pdf` al repo.~~ Hecho** — está en la raíz, y las
   secciones 2.3 a 2.9 ya se leyeron. Lo que trae cada una está en el punto 2.
2. **La opinión de la usuaria, para 2.8.** Mostrarle el sistema y anotar lo que
   diga. El ejemplo no usa encuesta: narra la relación a lo largo de las
   iteraciones. Alcanza con eso más **una frase suya dicha de verdad**. Se puede
   hacer con lo que ya anda.
3. **Fotografiar los cuadernos y planillas** con que lleva hoy el tambo. Van al
   Anexo y no dependen de nada.
4. **`1.11 Estimación del esfuerzo`.** Es del anteproyecto y habla de lo que se
   planificó al principio: no cambia con el Módulo 7. Preguntarle al tutor temprano
   si la pide.

Y una que **sí** espera al código, pero que hay que preparar antes porque es la
sección más larga:

5. **Las capturas de pantalla.** El contenedor remoto no
   tiene `dotnet` ni MySQL, así que el sistema no se puede levantar ahí. Hay dos
   caminos: sacarlas a mano con XAMPP y Visual Studio andando, o correr Claude
   Code **en la máquina de ustedes**, que sí puede levantar la app y sacarlas con
   Playwright de forma consistente —mismo tamaño de ventana, mismos datos de
   `DatosPrueba.sql`, mismo recorte—. El segundo camino es bastante mejor: son
   **más de ciento cincuenta** contando las del manual y las 69 que lleva la sección
   de Pruebas, y a mano salen desparejas.

---

## 11. Criterios de aceptación

El documento está terminado cuando:

- [ ] Cada sección de 2.3 a 2.9 copia la forma de su equivalente en el ejemplo:
      Pruebas sin numerar casos, el Manual con índice propio, Deployment sin
      instructivo de instalación, Contingencia en dos párrafos, Conclusiones con los
      diez subtítulos y los catorce riesgos uno por uno.
- [ ] El Módulo 7 está construido y los diagramas de secuencia del módulo se leen
      del código, no de los mensajes previstos.
- [ ] El MER no tiene tablas dibujadas como proyectadas.
- [ ] Ninguna sección dice «Pendiente.».
- [ ] Ningún documento promete pruebas automatizadas.
- [ ] `docs/auditoria-tres-vias.md` está cerrado: los 74 RF tienen su lugar en el
      código, no hay pantalla que ningún caso de uso describa, y toda divergencia
      quedó resuelta y consultada, no decidida por cuenta propia.
- [ ] Las correcciones al anteproyecto están registradas en
      `cambios-anteproyecto-v7.md`.
- [ ] Generar y editar son dos comandos separados, y el de editar corta con un
      mensaje claro si algún artefacto de `docs/` es más viejo que su fuente.
- [ ] Borrar `Proyecto_v6.docx` y `Tesis.docx` y correr el paso 2 los reconstruye
      completos, sin volver a generar nada. Ninguna sección se perdió por estar
      escrita a mano en el Word.
- [ ] `Tesis.docx` tiene un campo TOC real, se actualizó una vez en Word y el
      índice muestra números de página; el anteproyecto está numerado `1.x`.
- [ ] Las capturas —las del manual y las de la evidencia de Pruebas— salieron del
      script, no de recortes a mano, y se pueden volver a sacar corriéndolo.
- [ ] Está resuelto qué pasa con `1.11 Estimación del esfuerzo`, y el Anexo tiene
      las fotos de los cuadernos y planillas que el sistema reemplazó.
- [ ] El documento se leyó entero de corrido una vez, buscando contradicciones
      entre el anteproyecto (que habla en futuro, de lo que se va a hacer) y el
      proyecto (que habla de lo que se hizo).

---

## 12. Lo que queda afuera de este prompt

Terminadas las cuatro fases, con el código andando, **todavía queda esto**. No está
en las fases porque no es escribir secciones: es lo que hace que las 230 y pico de
páginas se
lean como un documento y no como dos pegados.

### 12.1 El frente del documento habla en futuro

El abstract y la introducción del anteproyecto están escritos **antes** de
programar, y se nota en cada verbo: «se propone el desarrollo», «el sistema
facilitará la generación de reportes», «aborda el análisis, diseño y
planificación». El ejemplo del tutor abre al revés: «El presente trabajo abarca
todo el proceso de análisis, desarrollo e implementación», «El sistema que se
implementó es un sistema web».

Un documento que en la página 3 promete lo que en la 150 ya mostró construido está
mal, aunque las dos partes sean correctas por separado. **Hay que reescribir el
abstract y la introducción en pasado**, y agregar las palabras clave que el ejemplo
tiene y el anteproyecto no.

Esto es lo mismo que la Fase 2 pero aplicado al frente: si el anteproyecto y el
proyecto tienen que decir lo mismo, tampoco pueden estar en tiempos verbales
distintos.

### 12.2 El glosario es de herramientas, no del tambo

Las catorce entradas actuales explican *Framework*, *Bootstrap*, *CSS3*, *Git*,
*UML*, *SQA*. Ninguna es del dominio. El tribunal sabe qué es un framework; lo que
no tiene por qué saber es qué es un tacto, ni por qué la leche de una vaca tratada
no se puede vender. Y esas palabras están en cada página.

**El glosario del ejemplo tiene apenas cinco entradas numeradas**, y una de ellas es
*tanda*: la palabra del dominio que su lector no podía adivinar. El criterio es
ése —definir lo que el lector no tiene por qué saber—, no la cantidad. Aplicado a un
sistema de tambo da veinte entradas, no cinco, porque nuestro dominio tiene veinte
palabras así.

**Las definiciones no se inventan: los números salen del código.** Cada entrada
lleva entre paréntesis de dónde sale, para que el glosario quede atado al sistema
como el resto del documento.

#### El rodeo y los animales

| Término | Qué decir |
|---|---|
| **Rodeo** | El conjunto de animales del establecimiento. |
| **Caravana** | La identificación individual del animal, única en el sistema. Es la forma en que la encargada lo nombra: no se usa el identificador interno. |
| **Categoría** | Valor **derivado**, no cargado: el sistema lo calcula con el sexo, la edad y la cantidad de partos, y lo propone al usuario. Las seis salen de la tabla `categorias`: Ternera, Novilla, Vaca, Ternero, Novillo, Toro. |
| **Novilla** | Hembra de más de 12 meses **sin partos registrados**. Cuando pare pasa a Vaca. |
| **Vaquillona** | Sinónimo de novilla de uso corriente en el tambo. **No es una categoría del sistema** (ver más abajo). |
| **Ascendencia / linaje** | La cadena de madres y padres de un animal, que el sistema arma solo a partir de los partos registrados. |
| **Consanguinidad** | Parentesco entre la hembra y el reproductor. El sistema busca un ancestro común y **advierte**; no bloquea el servicio. |

> **Una decisión de vocabulario para la Fase 2.** `bd/DatosPrueba.sql` y los flujos
> de prueba hablan de *vaquillonas*; el código y la base sólo conocen *Novilla*. Las
> dos palabras son correctas en el tambo, pero el documento tiene que usar una y
> declarar la otra en el glosario. Es exactamente el tipo de diferencia que la
> auditoría de tres vías tiene que cazar.

#### Producción

| Término | Qué decir |
|---|---|
| **Lactancia** | El período en que la vaca da leche, entre un parto y el secado siguiente. Se numeran: la primera, la segunda. |
| **Ordeñe por lote** | La medición de los litros de todo el grupo en un turno. Es lo que se hace todos los días. |
| **Control lechero** | La medición **vaca por vaca**, que se hace una vez por mes. Es lo que permite saber cuánto da cada una. |
| **Secado** | El cierre deliberado de la lactancia antes del parto siguiente, para que la vaca descanse. Por defecto, **60 días antes del parto probable** (`DIAS_SECADO_ANTES_PARTO`). |
| **Proyección a 305 días** | Lo que la vaca daría si sostuviera el ritmo hasta el final de una lactancia estándar. **305 días** es el valor de referencia (`DIAS_LACTANCIA_ESTANDAR`). Sirve para comparar vacas que van por distinto momento de su lactancia. |

#### Reproducción

| Término | Qué decir |
|---|---|
| **Celo** | El momento en que la hembra acepta el servicio. Se detecta por observación. Edad mínima: **9 meses** (`EDAD_MINIMA_CELO_MESES`). |
| **Servicio** | El intento de preñar. Por **inseminación artificial** o por **monta natural**. Edad mínima en la hembra: **13 meses** (`EDAD_MINIMA_SERVICIO_HEMBRA_MESES`). |
| **Pajuela** | La dosis de semen congelado de un toro identificado. Es un insumo: al inseminar, el stock baja. |
| **Tacto** | La palpación que confirma o descarta la preñez, a los **35 días** del servicio (`DIAS_PARA_TACTO`). |
| **Gestación** | **283 días** entre el servicio y el parto probable (`GESTACION_DIAS`). De ahí sale la fecha probable de parto. |
| **Período de espera voluntaria** | Los **45 días** después del parto en que no se sirve a la vaca aunque entre en celo (`DIAS_ESPERA_VOLUNTARIA`). |

#### Sanidad e insumos

| Término | Qué decir |
|---|---|
| **Plan sanitario** | La regla que dice qué procedimiento le corresponde a qué categoría, desde qué edad y cada cuántos días. El calendario de pendientes lo arma el sistema solo. |
| **Período de carencia** | Los días que, después de terminado el tratamiento, el producto sigue presente en el animal. Lo trae el insumo. |
| **Descarte de leche** | La ventana en que la leche **no se puede vender**: fin del tratamiento más la carencia del producto (`CalcularDescarte`). Mientras esté vigente, el sistema no deja sumar a esa vaca al ordeñe. |
| **Descorne** | El procedimiento sobre la cría para que no desarrolle cuernos. |

#### Los once parámetros de configuración

Van con su valor por defecto, porque el documento los menciona y la usuaria los
puede cambiar: días de secado antes del parto, edad mínima al servicio, edad de
cambio de categoría, litros máximos por control individual, ordeñes por día, días
de anticipación de secado, de parto, sanitaria y de vencimiento, días de espera
voluntaria y días para el tacto.

**Las seis categorías ya tienen su descripción escrita en `bd/CreacionDb.sql`.** Se
generan, no se copian: es el mismo criterio del resto del documento.

### 12.3 La bibliografía no tiene un solo libro

Hoy son seis sitios de documentación oficial más los materiales de Moodle, y
ninguna entrada tiene URL —el ejemplo del tutor sí las pone—. Faltan dos familias
enteras.

**Falta la bibliografía de ingeniería de software.** El anteproyecto compara el
modelo en cascada, el de prototipos, el incremental y el espiral; define un Plan de
SQA y un Plan de SCM; describe pruebas de caja negra y caja blanca. Nada de eso
sale de `docs.microsoft.com`. Las obras que corresponden:

| Para qué sección | Obra |
|---|---|
| Ciclos de vida, SQA, SCM, plan de testing | **Pressman**, *Ingeniería de software, un enfoque práctico* — el ejemplo cita la 7ma ed., McGraw-Hill, 2010 |
| Normalización, modelo entidad-relación, integridad | **Elmasri y Navathe**, *Fundamentos de Sistemas de Bases de Datos* — el ejemplo cita la 5ta ed., 2007 |
| Requerimientos y modelos de proceso | **Sommerville**, *Ingeniería de software* |
| Casos de uso, diagramas de secuencia y de clases | **Booch, Rumbaugh y Jacobson**, *El lenguaje unificado de modelado* |

**Las dos primeras las cita el propio ejemplo**, así que son exactamente las que el
tutor espera ver. El formato también sale de ahí: `Apellido, Nombre. Título. ed.
Ciudad: Editorial, año.` para libros y `Autor. Título [online]. Disponible en
internet: URL` para lo digital.

**Falta la bibliografía del dominio, y es la que más pesa.** Hoy las reglas del
tambo se apoyan sólo en el relevamiento con la usuaria. Eso alcanza para lo que es
propio del establecimiento —cuántos ordeñes hace, con qué criterio descarta—, pero
no para los números que son estándar de la actividad: **305 días de lactancia, 60
de secado, 283 de gestación, las edades mínimas al celo y al servicio**. Esos
tienen que citar una fuente técnica. En Uruguay las candidatas naturales son
**INALE**, **INIA** y las facultades de Veterinaria y Agronomía de la UdelaR.

> **Dos reglas al armarla.** Verificar edición y año contra el ejemplar que
> efectivamente usen: las obras de arriba tienen muchas ediciones y citar una que
> no se tuvo en la mano se nota. Y **no citar lo que no se leyó** — es preferible
> una bibliografía corta y honesta que una larga de adorno.

Agregar además la URL y la fecha de último acceso a las seis entradas que ya están,
como hace el ejemplo.

### 12.4 Trámite, pero no gratis

- **Actualizar el índice en Word.** Ver la Fase 4: un clic, pero sin él el índice
  sale en blanco.
- **Exportar el PDF final.**
- **`1.11 Estimación del esfuerzo`.** Sigue sin decidirse con el tutor. El Anexo ya
  está resuelto: son las fotos de los cuadernos y planillas del tambo.
- **La ronda de correcciones.** Entregar la v1 no es terminar. Conviene tener
  margen de calendario para una vuelta completa de devolución.

### 12.5 Fuera de alcance

La presentación de la defensa. No es parte del documento y no está en ninguna fase,
pero existe y sale de este mismo material.
