# Pendientes técnicos

Lo que le falta al sistema para estar terminado. Sólo trabajo por hacer: lo que se fue
corrigiendo dejó de figurar acá y vive en la historia del repositorio.

Los siete módulos están completos —CU1 a CU49, verificados uno por uno contra
`catalogo-casos-de-uso.md`—.

Verificado contra el código el **23/08/2026**.

---

## 1. Rehacer el home

Corrección pedida por el tutor en la reunión del 20/08/2026. **Pendiente de definir qué
cambia**: hasta que esté escrito, este punto es un recordatorio y no una tarea.

El tablero actual (`Pages/Index.cshtml`) no calcula nada propio: junta las listas de
trabajo y las alertas que ya viven en la Controladora. Esa decisión conviene conservarla
en el rediseño, sea cual sea la forma que tome la pantalla. El día que el tablero
empiece a calcular por su cuenta va a discrepar con los módulos, y nadie va a saber cuál
de los dos números está bien.

Ahora hay una razón más para conservarla: el resumen diario que sale por Telegram se
arma con esas mismas ocho listas. Un tablero que calcule aparte no discreparía con un
módulo, discreparía con el mensaje que la encargada leyó a la mañana en el teléfono.

---

## Lo que se cerró

Los dos puntos que ocupaban este documento hasta el 20/08/2026 están hechos, y quedan
anotados acá porque las dos decisiones se van a defender.

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
cliente del bot de setenta líneas contra la API de Telegram y un proceso en segundo
plano que escucha los comandos del bot y manda el resumen a la hora configurada. El
esquema sumó las dos tablas que el diseño ya preveía —`preferencias_notificacion` y
`alertas`— y tres columnas en `configuracion`.

Del diseño documentado se corrigió una cosa: `destinatario` estaba en
`preferencias_notificacion`, o sea un destinatario por tipo de aviso, y el sistema tiene
una sola usuaria. El chat pasó a `configuracion`, que es donde vive lo que hay uno solo.
