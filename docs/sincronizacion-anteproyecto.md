# Sincronización del Anteproyecto con el sistema construido

Inventario de todo lo que el Anteproyecto v5 dice y el código no hace, y de todo lo
que el código hace y el anteproyecto no dice. Es la lista de cambios que se va a
aplicar sobre `Anteproyecto_v5.docx` para producir la v6.

**Fuentes.** `Anteproyecto_v5.docx` (Drive) contra `Tesis/Dominio`, `Tesis/Persistencia`,
`Tesis/Pages`, `bd/*.sql` y las notas de `docs/`.

**Criterio.** Cuando el código y el documento se contradicen, manda el código: el
documento se ajusta a lo construido. Los cuatro casos donde me parece que el
documento tenía razón y conviene cambiar el código están juntos al final, en la
sección 8, y no se tocan en esta pasada.

**Qué queda afuera.** El Módulo 6 (reportes y notificaciones por Telegram) se va a
implementar, así que RF6.1 a RF6.7 no se tocan. Lo único que cambia respecto de él
es la sección de iteraciones, donde pasa a figurar como el incremento pendiente.

---

## 1. Módulo 0 — Seguridad y Acceso al Sistema

El módulo pasa a llamarse **Seguridad, Acceso y Configuración**.

| | |
|---|---|
| **RF0.1** | Sin cambios. La autenticación por cookie está implementada (`Program.cs`), el sitio entero queda detrás del login y las credenciales se leen de la configuración, no del código fuente. |

**RF0.2 Cierre de sesión** (nuevo). El sistema debe permitir cerrar la sesión activa
desde cualquier pantalla.

**RF0.3 Configuración de parámetros de manejo** (nuevo). El sistema debe permitir
configurar los parámetros de manejo del establecimiento, validando que cada uno quede
dentro de su rango admitido. Son nueve, con estos valores por defecto:

| Parámetro | Por defecto |
|---|---|
| Días de secado antes del parto | 60 |
| Edad mínima al servicio | 13 meses |
| Edad de cambio de categoría | 12 meses |
| Litros máximos por control individual | 100 |
| Ordeñes por día | 2 |
| Anticipación del aviso de secado | 15 días |
| Anticipación del aviso de parto | 15 días |
| Anticipación del calendario sanitario | 30 días |
| Anticipación del aviso de vencimiento | 30 días |

Esto es lo que sostiene que el resto de los requerimientos dejen de hablar de "60
días" o "dos turnos" como si fueran constantes del sistema.

---

## 2. Módulo 1 — Gestión de Animales y Genética

### Requerimientos que cambian

**RF1.2 Baja de animales.** Dice "baja lógica o definitiva". La baja definitiva no
existe: no hay borrado físico de animales en ninguna pantalla, y la regla de negocio
del propio documento lo prohíbe porque rompería el linaje y el historial productivo.
Queda: *baja lógica, indicando motivo y fecha de salida*.

**RF1.5 Registro genealógico.** Dice que padre y madre son **obligatorios** al alta.
El código los toma como opcionales, y hace falta que lo sean: con la regla como está
escrita sería imposible cargar el primer animal del rodeo. Además el propio documento
se contradice —CU5 curso de excepción 3a contempla un animal sin progenitores, y el
modelo relacional declara `id_madre` e `id_padre` nulables—. Queda: *el sistema debe
permitir registrar padre y madre de cada animal, admitiendo que no se conozcan*.

**RF1.9 Actualización automática de categoría.** Dice "actualizar automáticamente".
Lo que hace el sistema es más matizado, y es mejor: recalcula la categoría que
corresponde, marca en el listado y en la ficha los animales cuya categoría quedó
desactualizada, y ofrece la acción para aceptarla. Automática de verdad es sólo al
registrar un parto, que es el evento que cambia la condición biológica sin
ambigüedad. Se reescribe así, porque un cambio de categoría a espaldas de la usuaria
sobre un dato que ella puede haber ajustado a mano no es un comportamiento deseable.

**RF1.10 Consulta y búsqueda.** Enumera caravana, categoría, estado y rango etario.
Falta **raza**, que el buscador ofrece. Se agrega.

### Requerimientos nuevos

**RF1.11 Reactivación de un animal dado de baja.** El sistema debe permitir revertir
la baja de un animal, devolviéndolo al rodeo y limpiando la fecha y el motivo de
salida. Una caravana equivocada sacaba del rodeo al animal equivocado sin forma de
deshacerlo.

**RF1.12 Fotografía del animal.** El sistema debe permitir asociar una fotografía a
cada animal y mostrarla en su ficha y en el árbol genealógico.

**RF1.13 Árbol genealógico interactivo.** El sistema debe presentar la ascendencia del
animal en forma de árbol navegable, permitiendo desplegar cada rama y acceder a la
ficha de cualquier ancestro. Hoy RF1.6 sólo pide "ascendencia directa", que es menos
de lo que hay.

**RF1.14 Validación del árbol genealógico.** El sistema debe impedir que se registre
una genealogía imposible: un animal como progenitor de sí mismo, un progenitor que
figure en su propia descendencia, o un progenitor cuya fecha de nacimiento no admita
la edad mínima al servicio. Y debe advertir —sin bloquear— cuando el progenitor
elegido está dado de baja.

**RF1.15 Ficha integral del animal.** El sistema debe presentar en una sola pantalla
el estado y el historial completo de cada animal: datos de identificación, categoría,
linaje, producción, eventos reproductivos y eventos sanitarios.

---

## 3. Módulo 2 — Control de Producción

### Requerimientos que cambian

**RF2.1 y RF2.2 (ordeñe del Turno 1 y del Turno 2).** Están escritos como dos
requerimientos porque se dieron por fijos los dos ordeñes diarios. La cantidad de
turnos es configurable (RF0.3). Se unifican en un solo requerimiento:

> **RF2.1 Registro de ordeñe por lote.** El sistema debe permitir registrar los litros
> totales producidos por el rodeo en cada turno de ordeñe de la jornada, según la
> cantidad de turnos configurada.

Con esto se libera la numeración; el resto del módulo se corre un lugar. (La
alternativa, si preferís no renumerar, es dejar RF2.1 para el turno y RF2.2 para la
validación de que el turno exista. Lo charlamos.)

**RF2.3 Validación de producción.** Agregar que el tope de litros por control es el
configurado, y que en el ordeñe por lote el tope se calcula multiplicándolo por la
cantidad de animales del lote.

**RF2.4 Ordeñe individual.** El registro individual se llama **control lechero** en
todo el sistema, y admite dos formas de carga: masiva —todas las vacas en ordeñe el
mismo día, que es como se hace el control real— y de a una vaca. Se aclaran las dos
en el requerimiento. Vale dejar dicho lo que hoy no está en ningún lado: el control
lechero mide al animal, no la leche vendible, así que el par lote/individual **no se
suma** dentro de un mismo turno.

**RF2.7 Historial de lactancias.** El documento define la producción de una lactancia
como la suma de los controles. No puede serlo: los controles son mediciones puntuales,
más o menos mensuales, y sumarlas da un número sin sentido físico. El sistema estima
la producción por intervalos entre controles y la proyecta a 305 días. Se reescribe, y
se aclara que la proyección es lineal y sirve para comparar vacas entre sí, no como
pronóstico.

### Requerimientos nuevos

**RF2.13 Apertura manual de lactancia.** El sistema debe permitir abrir una lactancia
sin un parto registrado, para el caso de la vaca que ya estaba en ordeñe cuando se
empezó a usar el sistema.

**RF2.14 Corrección y eliminación de registros de producción.** El sistema debe
permitir corregir y eliminar los ordeñes por lote y los controles lecheros ya
registrados, informando qué registros dependen de uno que se quiere eliminar.

---

## 4. Módulo 3 — Gestión Reproductiva

### Requerimientos que cambian

**RF3.8 Registro de parto.** Agregar que las crías se dan de alta como animales del
rodeo en el mismo acto, con su caravana, raza y fotografía, y que el sistema propone
como padre el toro del servicio que originó la preñez.

### Requerimientos nuevos

**RF3.10 Validaciones reproductivas bloqueantes.** El sistema debe impedir registrar
un celo o un servicio a un animal que no alcanzó la edad mínima al servicio, y
cualquier evento con fecha posterior a la baja del animal.

**RF3.11 Advertencias reproductivas.** El sistema debe advertir —sin impedir el
registro— ante situaciones inusuales pero posibles: servicio con un toro dado de baja,
parentesco entre la hembra y el reproductor, mellizos de distinto sexo (freemartin),
duración de la gestación fuera del rango normal, y parto de una vaca que no figuraba
preñada.

**RF3.12 Listas de trabajo.** El sistema debe presentar las dos listas que ordenan la
jornada: los servicios con tacto pendiente y las vacas en condiciones de ser servidas,
indicando en cada caso el motivo por el que figuran.

**RF3.13 Corrección y eliminación de eventos reproductivos.** Celos, servicios, tactos
y partos deben poder corregirse y eliminarse, con el estado reproductivo del animal
volviéndose a deducir de los eventos que quedan.

---

## 5. Módulo 4 — Gestión Sanitaria

**RF4.8 Cierre del diagnóstico** (nuevo). El sistema debe permitir dar por resuelto un
diagnóstico, distinguiendo los cuadros activos de los cerrados.

**RF4.9 Corrección y eliminación de eventos sanitarios** (nuevo). Diagnósticos,
tratamientos, vacunaciones y descornes deben poder corregirse y eliminarse,
devolviendo al stock los insumos que se habían descontado.

RF4.1 a RF4.7 quedan como están: el calendario sanitario, los planes configurables y
el cálculo del período de descarte están implementados como los describe el documento.

---

## 6. Módulo 5 — Control de Insumos y Stock

**RF5.3 Descuento automático de medicamentos.** Dice "descontar stock al registrar
tratamientos", lo que sugiere que el sistema calcula la cantidad. No lo hace: la
cantidad a descontar se **ingresa** al registrar el tratamiento, porque la dosis
depende del peso del animal y de la presentación del producto. Se corrige.

**RF5.7 y RF5.8 (vencimientos).** Agregar que el stock de un insumo se lleva por
partidas con su propia fecha de vencimiento, y que el consumo se imputa a la partida
que vence primero.

**RF5.10 Reversión de movimientos de stock** (nuevo). Al corregirse o eliminarse el
evento que consumió un insumo, el sistema debe devolver la cantidad mediante un
contra-movimiento, sin borrar el movimiento original.

---

## 7. Módulo 7 — Tablero, Indicadores y Apoyo a la Decisión (nuevo)

Todo esto está construido y no figura en ningún requerimiento. Va como módulo nuevo
para no renumerar el Módulo 6, que se mantiene tal cual.

**RF7.1 Tablero de inicio.** Pantalla de entrada con el estado del día: lo que hay
pendiente y lo que vence.

**RF7.2 Indicadores del rodeo.** Días abiertos, intervalo entre partos, servicios por
preñez, litros por vaca y por día, días en leche promedio, composición del rodeo por
estado productivo y reproductivo, y ranking de las lactancias en curso con su
proyección a 305 días.

**RF7.3 Candidatas a descarte.** Lista de las vacas que cumplen al menos uno de los
criterios de descarte —producción bajo el 70 % del promedio del rodeo, tres o más
servicios sin preñez, más de 150 días abiertos, tres o más diagnósticos en el último
año, siete o más partos— mostrando por cuál de ellos aparece cada una. El sistema
informa; la decisión es de la encargada.

**RF7.4 Buscador global de caravana.** Campo permanente en la barra superior que lleva
a la ficha del animal desde cualquier pantalla.

---

## 8. Requerimientos no funcionales

Los nueve RNF actuales se conservan. Cambia lo que se agrega a cada uno:

| RNF | Qué se agrega |
|---|---|
| **Usabilidad** | Menú lateral por módulo con el criterio de "listado primero"; paginado de todos los listados; buscador global de caravana. |
| **Accesibilidad** | Cumplimiento de WCAG 2.1 AA: contrastes verificados, y el color nunca como único medio para transmitir información. Hoy el RNF sólo habla de navegadores, que es compatibilidad, no accesibilidad. |
| **Compatibilidad** | Uso verificado a 375 px de ancho, con las tablas adaptadas a la lectura en celular. |
| **Seguridad** | Autenticación por cookie sobre el sitio completo; credenciales y cadena de conexión fuera del código fuente; consultas parametrizadas. |
| **Fiabilidad** | Las operaciones que escriben en más de una tabla se resuelven en una transacción. |

---

## 9. Alcance y limitaciones

**Al alcance se agregan** tres viñetas: configuración de los parámetros de manejo del
establecimiento; tablero, indicadores y apoyo a la decisión de descarte; y corrección
de los registros ya cargados.

**A las limitaciones se agregan** cuatro, todas asumidas a conciencia y hoy sólo
anotadas en `docs/`:

- La configuración es una sola para todo el establecimiento: no se puede manejar
  distinto a un grupo de animales.
- El paginado de los listados es del lado del navegador; con el volumen de un tambo
  alcanza, pero no escala a decenas de miles de registros.
- La proyección de producción a 305 días es lineal.
- Los umbrales de descarte son criterios fijos del sistema, no parámetros
  configurables.

---

## 10. Incrementos o iteraciones definidas

La sección describe seis iteraciones que no son las que ocurrieron. El orden real
agrupó de a dos módulos y dejó una iteración entera para lo transversal, que el plan
original no preveía. Se reescribe así:

| Iteración | Contenido | Estado |
|---|---|---|
| 1ª | Módulo 0 y Módulo 1: acceso al sistema, gestión de animales, linaje y consanguinidad | Completada |
| 2ª | Módulo 2 y Módulo 3: producción y reproducción | Completada |
| 3ª | Módulo 4 y Módulo 5: sanidad, insumos y stock | Completada |
| 4ª | Reglas de negocio, validaciones y advertencias transversales; parámetros configurables; baja reversible; paginado | Completada |
| 5ª | Tablero, indicadores, listas de trabajo y apoyo al descarte; estilos, accesibilidad y uso desde el celular; corrección de registros y navegación | Completada |
| 6ª | Módulo 6: reportes, integración con Telegram y notificaciones automáticas; pruebas y ajustes finales | Pendiente |

Que la producción y la reproducción hayan ido juntas no fue una decisión de
conveniencia: el estado productivo de una vaca depende de sus partos y sus secados, y
separarlas obligaba a construir dos veces la misma máquina de estados.

---

## 11. Secciones que no se tocan, y por qué

- **Descripción y selección de herramientas.** Todo lo elegido se usó: .NET con C#,
  Razor Pages, MySQL, acceso a datos con consultas SQL sobre ADO.NET, HTML/CSS/JS con
  Bootstrap, Visual Studio. Telegram sigue en pie porque el Módulo 6 se va a hacer.
- **Análisis y plan de riesgo.** Los catorce riesgos siguen vigentes. R6 (dependencia
  de servicios externos de notificación) recién se materializa con el Módulo 6.
- **Estudio de alternativas, metodología, ciclo de vida, plan SQA, plan de testing,
  plan de SCM, control de versionado, plan de capacitación, cronograma.** Son
  decisiones y compromisos previos al desarrollo; no los invalidó nada de lo
  construido.
- **Introducción, presentación del cliente y del problema, necesidades, actores,
  objetivos, entorno, particularidades, glosario, bibliografía.** Sin cambios.

---

## 12. Los cuatro casos donde el documento tenía razón

Se listan acá para no perderlos, pero **no se aplican al anteproyecto**: son cambios
de código, y el documento ya dice lo correcto.

1. **Estados productivos sin normalizar.** El documento fija los valores admitidos en
   2.2.5.4 del Proyecto y el código escribe variantes de grafía. Corresponde
   normalizar el código contra el documento, no al revés.
2. **La persistencia no debe depender del dominio.** `pAnimal`, `pHembra` y `pMacho`
   instancian `Controladora`. El diagrama de persistencia muestra lo correcto.
3. **Los identificadores son auto-incrementales** en el modelo de datos, y varias
   inserciones los calculan a mano con `MAX(id)+1`.
4. **La caché estática de la Controladora** es compartida por todas las peticiones del
   proceso. Con una sola usuaria funciona; conviene dejarlo anotado como limitación
   asumida en el Proyecto.

Los cuatro están desarrollados en `docs/discrepancias-codigo-documentacion.md`.
