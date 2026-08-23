# Guion de capturas para el Manual de Usuario

Qué se fotografía, de qué pantalla, con qué animal y para qué sirve en el manual.
Lo ejecuta un recorrido de Playwright en la máquina de desarrollo —el contenedor
remoto no tiene `dotnet` ni MySQL— y las imágenes van a `docs/capturas/`, de donde
el paso de editar las toma con el mismo `d.imagen()` que ya coloca los 49
diagramas de secuencia.

El guion sale de `docs/flujos-de-prueba.md`: es el mismo recorrido, con una
columna más. Los animales son los del rodeo de `bd/DatosPrueba.sql`, elegidos
porque tienen la historia cargada que hace falta.

---

## 1. Reglas que valen para todas

| | |
|---|---|
| **Tamaño** | 1280 × 800 en escritorio. Las del apartado móvil, 390 × 844. |
| **Estado** | Siempre con el rodeo de `DatosPrueba.sql` cargado. **Ninguna lista vacía**: una pantalla sin datos no enseña nada. |
| **Sesión** | Iniciada como `sofia`. La contraseña nunca se ve escrita. |
| **Nombre del archivo** | `m<módulo>-cu<número>-<slug>.png`, por ejemplo `m1-cu04-alta-categoria.png`. El número de CU es el de `docs/catalogo-casos-de-uso.md`. |
| **Recorte** | Pantalla completa, salvo donde la tabla diga *detalle*: ahí sólo la zona que importa. |
| **Sin anotaciones** | Ni flechas, ni números, ni recuadros. La imagen va limpia y el pie explica. |
| **Datos de la clienta** | El nombre del establecimiento va neutro en la pantalla de configuración. |
| **Una sola sesión** | Todas se sacan de una pasada, en orden. El sistema se va modificando a medida que se cargan cosas y el orden del punto 4 lo tiene en cuenta. |

### La fecha se resuelve sola

`bd/DatosPrueba.sql` ancla el rodeo entero a `@hoy = CURDATE()`: las fechas se
calculan contra el día en que se corre el script, así que **no hay nada que correr a
mano y las alertas nunca quedan viejas**. Donde este guion o `flujos-de-prueba.md`
dicen `11/08/2026`, hay que leer *hoy*; `09/08/2026` es anteayer.

Igual conviene sacar todas las capturas **en una sola sesión**, para que las fechas
que se ven en pantalla sean coherentes entre una imagen y la siguiente.

---

## 2. Qué errores se documentan

Criterio del tutor: **no se documentan los errores que el sistema ya explica
solo.** Si la pantalla dice cuál fue el problema y cómo se arregla, la captura no
agrega nada.

Se documentan **siete**, y ninguno es una validación de formulario: son reglas del
tambo que el sistema conoce y la usuaria no tiene por qué adivinar.

| # | Dónde | Qué muestra |
|---|---|---|
| E1 | Alta de animal | Advertencia de genealogía: la madre `177` no tenía edad para parir. **No bloquea**: ofrece *Guardar de todos modos* |
| E2 | Verificar consanguinidad | `152` × `7HO12165` da parentesco: es su padre |
| E3 | Registrar servicio | Servicio consanguíneo advertido, con *Registrar de todos modos* |
| E4 | Ordeñe por lote | `115` no se puede sumar al lote: tiene descarte de leche vigente |
| E5 | Control lechero puntual | Vaca `136`: «no se encuentra en lactancia, su estado productivo es Seca» |
| E6 | Registrar celo | Ternera `177`, cuatro meses: por debajo de la edad mínima de celo configurada |
| E7 | Control lechero | Litros por encima del máximo por control, con el tope en el mensaje |

**Quedan afuera a propósito**, aunque estén en los flujos de prueba: caravana
duplicada, campos obligatorios, fechas futuras, rangos de fechas invertidos,
credenciales incorrectas, cantidades en cero. El sistema avisa con claridad y el
manual no gana nada repitiéndolo.

Los cuatro primeros son los que más importan: **E1, E3 y E4 no son errores, son
advertencias**. El sistema deja seguir y la decisión queda en la usuaria. Eso hay
que explicarlo con imagen, porque es lo contrario de lo que se espera.

---

## 3. Las capturas, módulo por módulo

### Módulo 0 — Seguridad, Acceso y Configuración

| Archivo | Pantalla | Estado a fotografiar |
|---|---|---|
| `m0-cu01-login` | `PagesSeguridad/Login` | Formulario vacío |
| `m0-cu02-sesion` | Barra superior (*detalle*) | Los seis módulos, `sofia` y **Cerrar Sesion** |
| `m0-cu03-configuracion` | `PagesConfiguracion/Configuracion` | Los once parámetros con sus valores |
| `m0-cu03-efecto` | `PagesReproduccion/AlertasParto` | Después de subir *Parto próximo* de 15 a 30 días: ahora también aparece `140`, que pare el 10/09. **Muestra para qué sirve configurar** |

### Módulo 1 — Gestión de Animales y Genética

| Archivo | Pantalla | Estado a fotografiar |
|---|---|---|
| `m1-cu10-lista` | `ListaAnimales` | El rodeo completo. `112`, dada de baja, no está |
| `m1-cu10-filtros` | `BuscarAnimales` | Categoría *Vaca* + Estado *En lactancia* → las ocho vacas en ordeñe |
| `m1-cu04-alta` | `AltaAnimal` | Formulario con la caravana `200` cargada |
| `m1-cu04-categoria` | `AltaAnimal` (*detalle*) | Después de **Calcular Categoria**: propone **Vaca** porque tiene partos. *La regla de negocio central del sistema* |
| `m1-cu04-genealogia` | `AltaAnimal` | **E1** — advertencia con *Guardar de todos modos* |
| `m1-cu05-foto` | `ModificarAnimal` | Campo de foto con la imagen cargada |
| `m1-cu11-ficha` | `DetalleAnimal` de `115` | La ficha integral: datos, mastitis en tratamiento, descarte de leche vigente, partos, servicios y producción |
| `m1-cu08-linaje` | `ConsultaLinaje` de `152` | El árbol armado: madre `101`, padre `7HO12165` |
| `m1-cu09-consanguinidad` | `VerificarConsanguinidad` | **E2** |
| `m1-cu06-baja` | `BajaAnimal` de `160` | Motivo *Venta*, con la confirmación |
| `m1-cu07-reactivar` | `ListaAnimales` | Filtro de dados de baja, con la acción de reactivar |

### Módulo 2 — Control de Producción

| Archivo | Pantalla | Estado a fotografiar |
|---|---|---|
| `m2-cu12-lote` | `OrdenieLote` | El lote con los animales tildados y los 88.10 litros |
| `m2-cu12-descarte` | `OrdenieLote` (*detalle*) | **E4** — `115` sin tildar y sin poder tildarse |
| `m2-cu13-masiva` | `ControlLechero` | Los siete litros vaca por vaca cargados |
| `m2-cu13-puntual` | `OrdenieIndividual` | Carga de una sola vaca, `130` |
| `m2-cu13-seca` | `OrdenieIndividual` | **E5** |
| `m2-cu13-maximo` | `OrdenieIndividual` | **E7** |
| `m2-cu14-historial` | `HistorialProduccion` | Rango 01/07 al 11/08, por lote |
| `m2-cu15-metrica` | `MetricaMensual` | Agosto 2026: total, promedio por ordeñe y por vaca |
| `m2-cu16-secado` | `RegistrarSecado` | Secado de `102` |
| `m2-cu17-alertas` | `AlertasSecado` | Con vacas listadas. **Requiere orden**: ver punto 4 |
| `m2-cu18-lactancia` | `AltaLactancia` | `200`, con **Proponer numero** sugiriendo `3` |
| `m2-cu19-corregir` | `HistorialProduccion` | Corrección de un control: fecha, turno y caravana visibles pero bloqueados |
| `m2-cu19-eliminar` | `HistorialProduccion` | La confirmación, que nombra caravana, fecha, turno y litros |
| `m2-lactancias` | `ListaLactancias` | El listado de lactancias |

### Módulo 3 — Gestión Reproductiva

| Archivo | Pantalla | Estado a fotografiar |
|---|---|---|
| `m3-cu20-celo` | `RegistrarCelo` | Celo de `115` |
| `m3-cu20-edad` | `RegistrarCelo` | **E6** |
| `m3-cu25-servir` | `VacasParaServir` | `115`, `130`, `133` y la vaquillona `158` |
| `m3-cu21-ia` | `RegistrarServicio` | Inseminación de `115`, con la FPP recalculada al `2027-05-21` |
| `m3-cu21-monta` | `RegistrarServicio` | Monta natural de `158`: al cambiar el tipo, el selector de pajuela se esconde y aparece el de toro |
| `m3-cu21-consanguineo` | `RegistrarServicio` | **E3** |
| `m3-cu22-pendientes` | `TactosPendientes` | Con `102`, servida el 20/06 |
| `m3-cu22-tacto` | `RegistrarTacto` | Con **Ver servicio** abierto: servicio del 20/06 con `T-01` y parto proyectado al 30/03/2027 |
| `m3-cu23-alertas` | `AlertasParto` | `136` (18/08) y `140` (10/09) |
| `m3-cu24-parto` | `RegistrarParto` | Madre `136` con el cartel del servicio, y la cría `180` con el padre ya propuesto |
| `m3-cu24-efecto` | `DetalleAnimal` de `136` | Después del parto: **En lactancia** y **Vacía**, con la lactancia 2 abierta. *Muestra todo lo que un parto dispara solo* |
| `m3-cu24-cria` | `ConsultaLinaje` de `180` | El linaje de la cría armado solo, sin cargarlo |
| `m3-cu26-corregir` | `ModificarParto` | Corrección de un parto |
| `m3-listas` | `ListaServicios` | Muestra del patrón de listado del módulo |

### Módulo 4 — Gestión Sanitaria

| Archivo | Pantalla | Estado a fotografiar |
|---|---|---|
| `m4-cu27-diagnostico` | `RegistrarDiagnostico` | Dermatitis digital de `108` |
| `m4-cu28-tratamiento` | `RegistrarTratamiento` | Con **Calcular** aplicado: descarte hasta el `2026-08-21`, que sale de inicio + 3 días + 7 de carencia |
| `m4-cu28-ficha` | `DetalleAnimal` de `108` (*detalle*) | La ficha sanitaria con el tratamiento colgado del diagnóstico |
| `m4-cu28-preventivo` | `RegistrarTratamiento` | Sin diagnóstico: preventivo sobre `158` con plan *Desparasitacion* |
| `m4-cu31-calendario` | `CalendarioSanitario` | `177` pendiente de brucelosis, `178` de descorne |
| `m4-cu29-vacunacion` | `RegistrarVacunacion` | Brucelosis a `177` |
| `m4-cu31-despues` | `CalendarioSanitario` (*detalle*) | `177` ya no está: la brucelosis es de aplicación única |
| `m4-cu32-descorne` | `RegistrarDescorne` | `178`, pasta cáustica |
| `m4-cu30-plan` | `ConfigurarPlan` | Plan *Vacuna clostridial*: periodicidad, edad de inicio y categorías alcanzadas |
| `m4-cu30-efecto` | `CalendarioSanitario` | Las crías `171`, `174`, `175`, `177` aparecen pendientes del plan nuevo, sin cargarlas una por una |
| `m4-cu33-cerrar` | `ListaDiagnosticos` | Cierre del diagnóstico de `115` |
| `m4-listas` | `ListaTratamientos` | Muestra del patrón de listado del módulo |

### Módulo 5 — Control de Insumos y Stock

| Archivo | Pantalla | Estado a fotografiar |
|---|---|---|
| `m5-cu37-critico` | `AlertasStock` | **Antes de reponer**: ivermectina y pajuela `29HO18296` |
| `m5-cu35-alta` | `AltaInsumo` | Cefquinoma intramamaria, con carencia de 5 días |
| `m5-cu35-ingreso` | `IngresoStock` | Ivermectina +10, motivo *Compra a veterinaria La Rural* |
| `m5-cu36-minimo` | `ConfigurarStockMinimo` | Pajuela `29HO18296` con mínimo `2` |
| `m5-cu37-resuelto` | `AlertasStock` | **Después**: vacía. *El par antes/después es lo que explica la alerta* |
| `m5-cu38-vencimiento` | `AlertasVencimiento` | Antiaftosa (31/08) e ivermectina (02/09), dentro de los 30 días configurados |
| `m5-cu39-movimientos` | `HistorialMovimientos` | Filtrado por oxitetraciclina: el ingreso del 12/05 y los egresos por tratamiento. *La trazabilidad del stock* |
| `m5-insumos` | `ListaInsumos` | El listado con los stocks |

### Módulo 6 — Tablero, Indicadores y Apoyo a la Decisión

| Archivo | Pantalla | Estado a fotografiar |
|---|---|---|
| `m6-cu40-tablero` | `Index` | El inicio con los avisos del día |
| `m6-cu41-indicadores` | `Indicadores` | Composición del rodeo, litros promedio, intervalo entre partos, porcentaje de preñez |
| `m6-cu42-descarte` | `CandidatasDescarte` | Con `133`: dos partos y sin preñez confirmada |
| `m6-cu43-buscar` | `Index` (*detalle*) | Búsqueda rápida por caravana |

### Módulo 7 — Reportes y Notificaciones

**Se define cuando el módulo esté construido.** Van a hacer falta al menos: los
cuatro reportes (productivo, sanitario, reproductivo, genético), la pantalla de
configuración del bot, y —la más valiosa— **una foto del celular con el resumen
diario recibido en Telegram**. Esa última no la puede sacar Playwright: es una
captura del teléfono.

---

## 4. El orden importa

Cuatro capturas dependen del estado del sistema y hay que sacarlas en su momento:

1. **`m5-cu37-critico` antes de `m5-cu35-ingreso`.** Reponer la ivermectina vacía
   la alerta; si se saca después, la pantalla queda en blanco.
2. **`m2-cu12-descarte` mientras el descarte de `115` esté vigente.** El rodeo se
   ancla a `CURDATE()`, así que recién cargado el juego de datos está vigente; si se
   dejan pasar días de trabajo encima, se vence. Es la captura que cierra el circuito
   entre sanidad y producción.
3. **`m2-cu17-alertas` después de confirmar preñeces.** Con el rodeo original la
   pantalla está vacía; se puebla recién cuando hay vacas cuya fecha probable de
   parto entra en la ventana de 60 + 15 días.
4. **`m3-cu24-efecto` y `m3-cu24-cria` después del parto de `136`**, obviamente, y
   `m4-cu31-despues` después de vacunar a `177`.

Fuera de eso, el orden de los módulos es el de este documento, que es el mismo de
`flujos-de-prueba.md`.

---

## 5. Uso desde el celular

El sistema se construyó para usarse en el tambo: están `tablaMovil.js`, las media
queries de `site.css` y `arbol.css`, y `docs/estilos-y-accesibilidad.md` lo
documenta. **Si se construyó y no se muestra, en la defensa lo van a preguntar.**

Siete capturas a 390 × 844, en un apartado corto al final del manual:

| Archivo | Qué muestra |
|---|---|
| `mov-menu` | El menú colapsado |
| `mov-lista-animales` | La lista con `tablaMovil` reacomodando las columnas |
| `mov-ficha` | La ficha de un animal en pantalla angosta |
| `mov-ordenie` | La carga del ordeñe por lote: **el caso de uso móvil real**, se hace en el tambo |
| `mov-celo` | El registro de un celo, que también se carga en el momento |
| `mov-alertas` | Una pantalla de alertas |
| `mov-linaje` | El árbol genealógico, que es lo que peor entra en una pantalla chica y por eso vale mostrarlo |

---

## 6. Las capturas de la sección 2.3 Pruebas

**La sección de Pruebas del ejemplo lleva 69 capturas propias**, y este guion nació
cubriendo sólo el manual. Hay que ampliarlo, pero **no son capturas nuevas del mismo
tipo**: son de otra naturaleza y por eso van aparte.

En el manual la captura muestra **una pantalla y para qué sirve**. En Pruebas la
captura es **la evidencia de un resultado**: se hizo tal cosa y esto es lo que el
sistema devolvió. Muchas son de pantallas ya fotografiadas, pero en otro momento —
después de guardar, con el mensaje de confirmación, con la lista ya modificada.

El criterio, siguiendo al ejemplo:

- Las variantes de entrada —login, caravana repetida, litros fuera de rango, rangos
  de fechas invertidos— van **en tabla, sin captura**.
- Los recorridos completos van con **`Prueba:` una línea y `Resultado:` la captura**
  del estado final: el parto que abrió la lactancia y dio de alta la cría, el
  tratamiento que sacó a la vaca del tanque, la inseminación que descontó la pajuela.

Se sacan en la misma corrida del script, con el sufijo `-resultado`, y se numeran
por prueba y no por pantalla. La lista concreta se arma al escribir 2.3, cuando esté
decidido qué recorridos se documentan.

---

## 7. Cuentas

| | |
|---|---|
| 2.4 Manual — escritorio, módulos 0 a 6 | **67** |
| 2.4 Manual — móvil | **7** |
| 2.4 Manual — Módulo 7 | a definir, unas **7** |
| **Subtotal del manual** | **~81** |
| 2.3 Pruebas — evidencia de resultados | a definir (el ejemplo lleva **69**) |
| **Total estimado** | **~150** |

De ésas, **siete** son las advertencias y errores del punto 2, y **seis** son pares
antes/después que muestran el efecto de una acción: la configuración sobre las
alertas de parto, el plan sanitario sobre el calendario, el ingreso sobre el stock
crítico, la vacunación sobre el calendario, el parto sobre la ficha de la madre y
sobre el linaje de la cría. Ese tipo de par es lo que separa un manual de una
lista de pantallas.

El ejemplo tiene 123 imágenes en las 47 páginas de su manual, algo más de dos y
media por página; las 81 nuestras en la misma extensión dan menos de dos. Es la
proporción que corresponde: cada pantalla con su imagen y el texto que explica qué
hace, qué campos tiene y qué calcula sola.

---

## 8. Decisiones tomadas

- **Sin anotaciones.** Nada de flechas, números en círculos ni recuadros sobre la
  imagen. La captura va limpia, tal como se ve la pantalla, y **la explicación va
  en el pie**, como en el ejemplo del tutor. Consecuencia práctica: el pie tiene que
  ser una descripción de verdad, no una etiqueta. «Pantalla de alta de animal» no
  sirve; sirve «Alta de un animal comprado: al apretar *Calcular Categoria* el
  sistema propone *Vaca*, porque la caravana `200` tiene partos registrados».
- **Sin el nombre del establecimiento.** En la pantalla de configuración el nombre
  va neutro. Es dato de la clienta, no del sistema, y el manual se entiende igual.

Al escribir el manual, entonces, **el trabajo está en los pies de figura**. Son
ochenta y uno y cada uno tiene que decir qué se está mirando y por qué importa. Es
lo que carga el peso que en otros manuales llevan las flechas.
