# Pendientes técnicos

Lo que le falta al sistema para estar terminado. Sólo trabajo por hacer: lo que se fue
corrigiendo dejó de figurar acá y vive en la historia del repositorio.

Los módulos 0 a 6 están completos —CU1 a CU43, verificados uno por uno contra
`catalogo-casos-de-uso.md`—. Lo que sigue es todo lo que queda.

Verificado contra el código el **20/08/2026**.

Los tres puntos están en el orden en que conviene encararlos, y el motivo del orden
está escrito en cada uno: no es una lista de deseos sino un plan.

---

## 1. La caché `static` de la Controladora

**Por qué va primero.** No porque sea lo más grande, sino porque es la base de lo que
viene. El resumen diario del Módulo 7 es un proceso que corre solo, y es exactamente lo
que esta caché no soporta. Hacerlo después significa volver a probar todo dos veces.

`Dominio/Controladora.cs:101-120` declara **diecinueve listas `static`**, más la
configuración (`:99`) y las credenciales (`:346-347`), compartidas por todas las
peticiones del proceso.

**El argumento no es de estilo, es que lo `static` no aporta nada.** `mRefrescado`
(`:127`) no es static, así que cada `new Controladora()` que toca un `Listar` ejecuta
`Refrescar()` y recarga las diecinueve listas. La caché compartida no evita una sola
consulta entre peticiones: sólo sirve dentro de una, y para eso alcanzan campos de
instancia. Se paga el riesgo de la memoria compartida sin el beneficio.

Tres modos de falla concretos:

1. **Colección modificada mientras se recorre.** Hay 36 mutaciones (`Add`, `Remove`)
   sobre esas listas y decenas de `foreach`. Una petición iterando mientras otra agrega
   produce `InvalidOperationException`, es decir un error 500.
2. **Objetos compartidos mutados a mitad.** Las listas guardan las instancias, no
   copias: al cambiar el estado de una hembra se muta el objeto que otra petición ya
   tiene en la mano.
3. **Caché envenenada.** Si una operación compuesta falla después de tocar la caché,
   los datos que no llegaron a la base quedan visibles para todo el proceso. Con listas
   de instancia, el desastre muere con la petición.

**El cambio no es sólo borrar `static` veintiún veces.** `BuscarAnimal` (`:420`) y la
mayoría de los métodos leen las listas sin llamar antes a `Refrescar()`: sólo 41 lo
hacen. Hoy funciona porque la lista es static y alguien la cargó en una petición
anterior —lo que además esconde un error latente: recién levantado el servidor, una
pantalla que llame a `BuscarAnimal` sin pasar por un `Listar` recibe `null`—. La forma
segura es invocar `Refrescar()` en el constructor: con eso el comportamiento observable
queda igual, desaparecen los tres modos de falla y se cierra el error latente.

**Costo estimado:** medio día, más recorrer entero el guion de `flujos-de-prueba.md`.

## 2. El Módulo 7 completo

**Es lo más grande que queda: seis casos de uso sin una línea escrita.** No hay ningún
archivo en `Tesis/` que mencione "Reporte" ni "Telegram". No es alcance opcional: los
requerimientos RF7.1 a RF7.7 están declarados en el Anteproyecto v6.

| CU | Qué | RF |
|---|---|---|
| CU44 | Generar reporte productivo, en PDF y Excel | RF7.1 |
| CU45 | Generar reporte sanitario | RF7.2 |
| CU46 | Generar reporte reproductivo | RF7.3 |
| CU47 | Generar reporte genético | RF7.4 |
| CU48 | Configurar integración con bot de Telegram | RF7.5 |
| CU49 | Enviar resumen diario de tareas pendientes | RF7.6, RF7.7 |

**Los reportes (CU44 a CU47) primero**, porque son autocontenidos: los datos ya están
calculados en la Controladora y lo único nuevo es el formato de salida. Hace falta
elegir una biblioteca de PDF y otra de Excel, que serían las primeras dependencias
externas del proyecto más allá del conector de MySQL.

**Telegram y el resumen diario (CU48 y CU49) al final**, que es donde está el riesgo:
un bot necesita un token, un proceso corriendo y una forma de probarlo sin depender de
que alguien mire el celular.

Lo que alivia el trabajo de RF7.6 —notificar procedimientos sanitarios pendientes,
partos próximos, tactos pendientes, secados próximos, stock crítico, vencimientos y fin
del descarte de leche— es que **esas siete listas ya existen y están calculadas**: son
las mismas que alimentan el tablero de inicio. Lo que falta construir es el canal, no
la lógica.

## 3. Rehacer el home

Corrección pedida por el tutor en la reunión del 20/08/2026. **Pendiente de definir qué
cambia**: hasta que esté escrito, este punto es un recordatorio y no una tarea.

El tablero actual (`Pages/Index.cshtml`) no calcula nada propio: junta las listas de
trabajo y las alertas que ya viven en la Controladora. Esa decisión conviene conservarla
en el rediseño, sea cual sea la forma que tome la pantalla. El día que el tablero
empiece a calcular por su cuenta va a discrepar con los módulos, y nadie va a saber cuál
de los dos números está bien.

Entra en cualquier hueco del plan: no depende de los otros dos puntos ni los bloquea.
