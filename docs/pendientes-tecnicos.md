# Pendientes técnicos

Lo que le falta al sistema para estar terminado. Sólo trabajo por hacer: lo que se fue
corrigiendo dejó de figurar acá y vive en la historia del repositorio.

Los siete módulos están completos —CU1 a CU49, verificados uno por uno contra
`catalogo-casos-de-uso.md`—.

Verificado contra el código el **08/09/2026**. `dotnet build` sale con **0 errores**.

---

## 1. No queda trabajo de código

Los siete módulos están completos y el único punto abierto que quedaba —el renombre de
«Novilla» a «Vaquillona»— se aplicó el 08/09/2026. Lo que sigue es ejecución sobre el
sistema andando: sacar las 106 capturas del `guion-capturas.md` y completar la columna
«Resultado» de `seccion-2-3-pruebas.md`. Después, la preparación del repositorio de
entrega.

---

## 2. Lo que no va a tener el sistema, decidido

**No lleva pruebas automatizadas.** Ni de unidad ni de integración: no hay proyecto de
tests en la solución y no lo va a haber. Las pruebas son **funcionales y manuales**,
ejecutadas sobre el sistema andando con el rodeo de `bd/DatosPrueba.sql`, y quedan
registradas en `docs/seccion-2-3-pruebas.md` con sus datos, su resultado esperado y su
captura.

Está alineado con lo que el anteproyecto promete desde la v7: el plan de testing dice
«pruebas de caja negra sobre cada funcionalidad», y la mención de caja blanca se quitó
justamente para no comprometer algo que el proyecto no iba a hacer. Un plan que promete
menos y lo cumple vale más que uno que promete cobertura automatizada y entrega una
carpeta vacía.

Lo que la caja blanca aportaba —los casos de borde de los cálculos que el sistema
resuelve solo— se verifica igual, pero desde la pantalla: la categoría en la edad de
cambio, el alcance de la consanguinidad, la lactancia abierta contra la cerrada.

---

## Lo que se cerró

Anotado acá porque las decisiones se van a defender.

**El home, rehecho.** Era la corrección que pidió el tutor el 20/08/2026 y quedó hecha
el 22/08 (`3ba131e`): el tablero sumó el **registro rápido de eventos reproductivos**.
El relevamiento con la encargada dio que lo que se carga todos los días son el celo, el
servicio, el tacto y el parto, y que llegar a cada uno costaba cuatro clics de menú, una
vez por animal. Ahora dos de esos eventos se resuelven en el propio tablero y los otros
dos abren el formulario completo con la caravana ya puesta; el corte no es por
dificultad sino por lo que arrastra cada evento, y está explicado en `Guardar` y en
`AbrirFormulario`.

**Lo que el rediseño conservó, y es lo importante:** el tablero **no calcula nada
propio**. Junta las listas de trabajo y las alertas que ya viven en la Controladora. El
día que empiece a calcular por su cuenta va a discrepar con los módulos, y nadie va a
saber cuál de los dos números está bien. Ahora hay una razón más: el resumen diario que
sale por Telegram se arma con esas mismas ocho listas, así que un tablero que calcule
aparte no discreparía con un módulo, discreparía con el mensaje que la encargada leyó a
la mañana en el teléfono.

**El renombre de «Novilla» a «Vaquillona»** (08/09/2026). Es el hallazgo H5 de la
auditoría, resuelto el 26/08 al revés de lo que se había propuesto: se usa el nombre
corriente en el tambo, y por eso el cambio no fue sólo del documento. Dos líneas de
comportamiento —`Controladora.cs:1009` y la fila semilla de `categorias` en
`bd/CreacionDb.sql:558`— más cuatro comentarios de RF1.9 que decían «vuelve a ser
novilla».

Los dos cambios van juntos o ninguno: `CalcularCategoria` arma el nombre y después
busca la fila de `categorias` que se llama así, de modo que renombrar sólo uno de los
dos lados devolvería `null` para toda hembra de más de doce meses sin partos. **No se
tocó un solo dato**: las demás tablas apuntan por `id_categoria` y `DatosPrueba.sql`
usa los ids, así que la vaquillona sigue siendo la categoría 2.

**La caché `static` de la Controladora.** Las diecinueve listas y la configuración
pasaron a ser campos de instancia, y `Refrescar()` se invoca en el constructor. Lo
`static` no ahorraba una sola consulta entre peticiones —el refresco era por
Controladora y no por proceso— y sí costaba tres modos de falla: colección modificada
mientras se recorre, objetos compartidos mutados a mitad y caché envenenada por una
operación que falló después de tocarla. Además cerró un error latente: recién levantado
el servidor, una pantalla que llamara a `BuscarAnimal` sin pasar antes por un `Listar`
recibía `null`. Lo único `static` que queda en la clase son las credenciales de acceso,
que no dependen de la base y por eso dejan funcionar el login con el motor apagado.

Era, además, la condición para el Módulo 7: el proceso del resumen diario construye una
Controladora en paralelo a las peticiones web, que es exactamente lo que la memoria
compartida no soportaba.

**El Módulo 7 completo.** Los cuatro reportes (CU44 a CU47) salen en PDF con QuestPDF y
en Excel con ClosedXML. Las notificaciones (CU48 y CU49) están en `Notificaciones/`: un
cliente del bot contra la API de Telegram y un proceso en segundo plano que escucha los
comandos del bot y manda el resumen a la hora configurada. El esquema sumó las dos
tablas que el diseño ya preveía —`preferencias_notificacion` y `alertas`— y tres
columnas en `configuracion`.

Del diseño documentado se corrigió una cosa: `destinatario` estaba en
`preferencias_notificacion`, o sea un destinatario por tipo de aviso, y el sistema tiene
una sola usuaria. El chat pasó a `configuracion`, que es donde vive lo que hay uno solo.

**El manejo de errores del bot** (08/09/2026, `54d98d2`). `EnviarMensaje` y
`ObtenerMensajes` devuelven el motivo de la falla en lugar de un `bool`, y la
cancelación viaja hasta la petición HTTP. Sin lo primero, un token vencido y un
identificador de chat mal copiado se leían igual y la pantalla mandaba a revisar el
número cuando el número estaba bien; sin lo segundo, apagar el sitio esperaba hasta
veinte segundos una respuesta que ya no le importaba a nadie.

**RF7.5, RF7.6 y RF7.7, auditados** el 08/09/2026 contra el código, junto con los cuatro
requerimientos que la pasada de agosto había dejado sin verificar a nivel de línea
—RF1.14, RF4.5, RF4.9 y RF5.8—. Los siete cumplen. Los cinco hallazgos que salieron
(H9 a H13) son todos de redacción del documento y están en `auditoria-tres-vias.md`.
