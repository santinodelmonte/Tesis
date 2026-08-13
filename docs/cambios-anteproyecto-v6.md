# Anteproyecto v6 — qué cambió respecto de la v5

Resumen de lo que se modificó sobre `Anteproyecto_v5.docx` para producir
`Anteproyecto_v6.docx`. El detalle de por qué cambia cada cosa está en
`sincronizacion-anteproyecto.md`; acá está lo que hay que mirar al abrir el documento.

El archivo se editó sobre el `.docx` original, conservando estilos, numeraciones,
encabezados, imágenes y tablas. El script que aplica los cambios quedó en
`docs/editar_anteproyecto.py`, de modo que la operación es repetible y auditable.

**Al abrirlo en Word conviene actualizar el índice** (clic derecho sobre él →
Actualizar campos): las secciones conservan su nombre, pero los números de página se
corrieron.

---

## Lo que se tocó

Cinco secciones: **Requerimientos Funcionales**, **Requerimientos No Funcionales**,
**Alcance y Limitaciones**, **Incrementos o Iteraciones Definidas**, y el encabezado
del Módulo 0. El resto del documento quedó intacto.

## Requerimientos funcionales

De 55 requerimientos se pasó a 74, en siete módulos en lugar de seis: 20 nuevos y dos
—los dos ordeñes por turno— unificados en uno.

| Módulo | v5 | v6 | |
|---|---|---|---|
| 0 — Seguridad, Acceso y **Configuración** | 1 | 3 | renombrado |
| 1 — Gestión de Animales y Genética | 10 | 15 | |
| 2 — Control de Producción | 12 | 13 | renumerado |
| 3 — Gestión Reproductiva | 9 | 13 | |
| 4 — Gestión Sanitaria | 7 | 9 | |
| 5 — Control de Insumos y Stock | 9 | 10 | |
| 6 — **Tablero, Indicadores y Apoyo a la Decisión** | — | 4 | nuevo |
| 7 — Reportes y Notificaciones | 7 | 7 | era el Módulo 6 |

### Requerimientos nuevos (20)

| | |
|---|---|
| RF0.2 | Cierre de sesión |
| RF0.3 | Configuración de parámetros de manejo |
| RF1.11 | Reactivación de animales |
| RF1.12 | Fotografía del animal |
| RF1.13 | Árbol genealógico interactivo |
| RF1.14 | Validación del árbol genealógico |
| RF1.15 | Ficha integral del animal |
| RF2.12 | Apertura manual de lactancia |
| RF2.13 | Corrección y eliminación de registros de producción |
| RF3.10 | Validaciones reproductivas |
| RF3.11 | Advertencias reproductivas |
| RF3.12 | Listas de trabajo |
| RF3.13 | Corrección y eliminación de eventos reproductivos |
| RF4.8 | Cierre del diagnóstico |
| RF4.9 | Corrección y eliminación de eventos sanitarios |
| RF5.10 | Reversión de movimientos de stock |
| RF6.1 | Tablero de inicio |
| RF6.2 | Indicadores del rodeo |
| RF6.3 | Candidatas a descarte |
| RF6.4 | Buscador de caravana |

### Requerimientos corregidos (11)

| | Qué decía | Qué dice |
|---|---|---|
| RF1.2 | baja lógica **o definitiva** | sólo baja lógica, con fecha y motivo |
| RF1.5 | padre y madre **obligatorios** | opcionales: admite que no estén registrados |
| RF1.9 | actualización **automática** de categoría | recalcula y señala; automática al registrarse un parto |
| RF1.10 | caravana, categoría, estado, edad | agrega **raza** |
| RF2.1 | dos requerimientos, uno por turno | uno solo, para la cantidad de turnos configurada |
| RF2.2 | litros positivos y coherentes | agrega el tope configurado, y el tope por lote |
| RF2.3 | "ordeñe individual" | **control lechero**, con carga masiva y puntual |
| RF2.6 | producción = suma de los controles | estimación por intervalos y proyección a 305 días |
| RF3.8 | registra los datos de la cría | da de alta la cría como animal del rodeo |
| RF5.3 | descuenta el stock | la cantidad se **ingresa**, no se calcula |
| RF5.7 y RF5.8 | vencimiento del insumo | vencimiento **por partida**, consumo a la que vence primero |

### Renumeración

El Módulo 2 se corre un lugar desde RF2.3 en adelante, porque los dos requerimientos
de ordeñe por turno se unificaron en uno. El Módulo 6 anterior pasa a ser el 7, con
RF6.1–RF6.7 renumerados a RF7.1–RF7.7. Es la única renumeración: los módulos 1, 3, 4 y
5 conservan la numeración de la v5.

**Esto impacta en el Proyecto**: cada caso de uso referencia sus requerimientos por
número. La actualización de esas referencias es parte de la segunda etapa.

## Requerimientos no funcionales

Los nueve se conservan con su redacción; a cinco se les sumó una frase.

- **Usabilidad**: menú por módulo con listado primero, paginado de listados, buscador
  de caravana permanente.
- **Accesibilidad**: cumplimiento de WCAG 2.1 AA, contrastes verificados, el color
  nunca como único medio.
- **Compatibilidad**: uso verificado desde 375 px, tablas adaptadas al celular.
- **Fiabilidad**: transacciones en las operaciones que escriben en más de una tabla.
- **Seguridad**: credenciales y cadena de conexión fuera del código fuente, consultas
  parametrizadas.

## Alcance y limitaciones

Tres puntos nuevos al alcance —configuración de parámetros, tablero e indicadores,
corrección de registros— y cuatro limitaciones asumidas que hasta ahora sólo estaban
anotadas en `docs/`: configuración única por establecimiento, paginado del lado del
navegador, proyección a 305 días lineal, y umbrales de descarte fijos.

## Iteraciones

Las seis iteraciones se reescribieron para reflejar el orden en que efectivamente se
trabajó. El cambio de fondo es que las seis del plan original iban de a un módulo por
iteración, y el desarrollo real las agrupó de a dos, dejando dos iteraciones enteras
—la cuarta y la quinta— para trabajo transversal que el plan no preveía: reglas de
negocio, parámetros configurables, tablero, accesibilidad y corrección de registros.

Las cinco primeras figuran como completadas. La sexta —reportes, Telegram y
notificaciones— es la que resta.

---

## Lo que queda pendiente de este documento

Nada bloqueante, pero conviene decidirlo antes de la entrega:

1. **El índice** hay que actualizarlo en Word.
2. **La portada** dice "Abril 2026". Si la v6 se entrega, corresponde revisar la fecha.
3. **El cronograma de trabajo** (sección propia) no se tocó. Si el orden real de las
   iteraciones lo desactualizó, avisame y lo ajustamos.
