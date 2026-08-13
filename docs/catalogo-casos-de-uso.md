# Catálogo de casos de uso — v6

Los 35 casos de uso de la v5 pasan a 49. La numeración se rehace completa porque el
Módulo 6 nuevo se intercala antes de Reportes y porque hay casos de uso nuevos dentro
de los módulos 0 a 5. Esta tabla es el mapa entre una versión y la otra: sirve para
verificar que no se perdió ninguno y para rastrear las referencias cruzadas.

Los números de requerimiento son los de la **v6 del anteproyecto**.

## Módulo 0 — Seguridad, Acceso y Configuración

| CU v6 | Nombre | v5 | RF |
|---|---|---|---|
| CU1 | Iniciar Sesión | CU1 | RF0.1 |
| CU2 | Cerrar Sesión | nuevo | RF0.2 |
| CU3 | Configurar Parámetros del Establecimiento | nuevo | RF0.3 |

## Módulo 1 — Gestión de Animales y Genética

| CU v6 | Nombre | v5 | RF |
|---|---|---|---|
| CU4 | Registrar Alta de Animal | CU2 | RF1.1, RF1.4, RF1.5, RF1.8, RF1.12, RF1.14 |
| CU5 | Modificar Datos de Animal | CU3 | RF1.3, RF1.9, RF1.12, RF1.14 |
| CU6 | Registrar Baja de Animal | CU4 | RF1.2 |
| CU7 | Reactivar Animal | nuevo | RF1.11 |
| CU8 | Consultar Linaje y Registro Genealógico | CU5 | RF1.6, RF1.13 |
| CU9 | Verificar Consanguinidad | CU6 | RF1.7 |
| CU10 | Buscar y Filtrar Animales del Rodeo | CU7 | RF1.10 |
| CU11 | Consultar Ficha Integral del Animal | nuevo | RF1.15 |

## Módulo 2 — Control de Producción

| CU v6 | Nombre | v5 | RF |
|---|---|---|---|
| CU12 | Registrar Ordeñe por Lote | CU8 | RF2.1, RF2.2 |
| CU13 | Registrar Control Lechero | CU9 | RF2.2, RF2.3 |
| CU14 | Consultar Historial de Producción y Lactancias | CU10 | RF2.5, RF2.6, RF2.7 |
| CU15 | Consultar Métrica de Producción Mensual | CU11 | RF2.4, RF2.7 |
| CU16 | Registrar Período de Secado Manual | CU12 | RF2.8, RF2.11 |
| CU17 | Consultar Alertas de Secado Próximo | CU13 | RF2.9, RF2.10 |
| CU18 | Abrir Lactancia Manualmente | nuevo | RF2.12 |
| CU19 | Corregir o Eliminar Registro de Producción | nuevo | RF2.13 |

## Módulo 3 — Gestión Reproductiva

| CU v6 | Nombre | v5 | RF |
|---|---|---|---|
| CU20 | Registrar Detección de Celo | CU14 | RF3.1, RF3.10 |
| CU21 | Registrar Servicio | CU15 | RF3.2, RF3.3, RF3.9, RF3.10, RF3.11, RF5.4 |
| CU22 | Registrar Tacto y Confirmación de Preñez | CU16 | RF3.4, RF3.5, RF3.6, RF3.9 |
| CU23 | Consultar Alertas de Parto Próximo | CU17 | RF3.7 |
| CU24 | Registrar Parto | CU18 | RF3.8, RF3.9, RF3.11, RF2.11, RF1.9 |
| CU25 | Consultar Listas de Trabajo Reproductivas | nuevo | RF3.12 |
| CU26 | Corregir o Eliminar Evento Reproductivo | nuevo | RF3.13 |

## Módulo 4 — Gestión Sanitaria

| CU v6 | Nombre | v5 | RF |
|---|---|---|---|
| CU27 | Registrar Diagnóstico o Revisación | CU19 | RF4.1 |
| CU28 | Registrar Tratamiento Sanitario | CU20 | RF4.2, RF4.3, RF5.3 |
| CU29 | Registrar Vacunación | CU21 | RF4.4 |
| CU30 | Configurar Plan Sanitario | CU22 | RF4.7 |
| CU31 | Consultar Calendario Sanitario | CU23 | RF4.5 |
| CU32 | Registrar Procedimiento de Descorne | CU24 | RF4.6 |
| CU33 | Cerrar Diagnóstico | nuevo | RF4.8 |
| CU34 | Corregir o Eliminar Evento Sanitario | nuevo | RF4.9, RF5.10 |

## Módulo 5 — Control de Insumos y Stock

| CU v6 | Nombre | v5 | RF |
|---|---|---|---|
| CU35 | Registrar Alta e Ingreso de Insumo | CU25 | RF5.1, RF5.2, RF5.7 |
| CU36 | Configurar Umbral de Stock Mínimo | CU26 | RF5.5 |
| CU37 | Consultar Alertas de Stock Crítico | CU27 | RF5.6 |
| CU38 | Consultar Alertas de Vencimiento de Insumos | CU28 | RF5.8 |
| CU39 | Consultar Historial de Movimientos de Stock | CU29 | RF5.9, RF5.10 |

## Módulo 6 — Tablero, Indicadores y Apoyo a la Decisión

| CU v6 | Nombre | v5 | RF |
|---|---|---|---|
| CU40 | Consultar Tablero de Inicio | nuevo | RF6.1 |
| CU41 | Consultar Indicadores del Rodeo | nuevo | RF6.2 |
| CU42 | Consultar Candidatas a Descarte | nuevo | RF6.3 |
| CU43 | Buscar Animal por Caravana | nuevo | RF6.4 |

## Módulo 7 — Reportes y Notificaciones

| CU v6 | Nombre | v5 | RF |
|---|---|---|---|
| CU44 | Generar Reporte Productivo | CU30 | RF7.1 |
| CU45 | Generar Reporte Sanitario | CU31 | RF7.2 |
| CU46 | Generar Reporte Reproductivo | CU32 | RF7.3 |
| CU47 | Generar Reporte Genético | CU33 | RF7.4 |
| CU48 | Configurar Integración con Bot de Telegram | CU34 | RF7.5, RF7.6 |
| CU49 | Enviar Resumen Diario de Tareas Pendientes | CU35 | RF7.7 |

---

## Dos decisiones sobre cómo se agrupan

**La corrección de registros es un caso de uso por módulo, no uno por entidad.** El
sistema resuelve el alta y la corrección con la misma pantalla —recibe un
identificador y cambia de modo—, así que escribir un caso de uso de corrección por
cada entidad habría duplicado catorce veces el mismo texto. Queda uno por módulo
(CU19, CU26, CU34) que describe la mecánica común y enumera las entidades que
alcanza, y cada caso de uso de registro menciona que la misma pantalla corrige.

**La carga masiva del control lechero es el curso básico de CU13, no un caso de uso
aparte.** El control lechero se hace una vez por mes midiendo todas las vacas el mismo
día: ése es el camino normal. La carga de a un animal queda como curso alternativo,
que es lo que refleja el menú del sistema.

Los quince casos de uso nuevos son quince porque describen algo que el usuario puede
hacer y que ningún caso de uso de la v5 cubría. La fotografía del animal (RF1.12) y el
árbol interactivo (RF1.13), en cambio, no son casos de uso propios: son pasos dentro
del alta, la modificación y la consulta de linaje, y ahí quedaron.
