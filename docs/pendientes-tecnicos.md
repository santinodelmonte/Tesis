# Pendientes técnicos

Lo que el código debe y los documentos ya dicen bien. No son desvíos de la
documentación —para eso está la auditoría, que corrige el documento contra el
código— sino lo contrario: los dos puntos donde el documento tiene razón y el
código todavía no lo acompaña.

Verificado contra el código el **13/08/2026**.

---

## 1. La caché `static` de la Controladora

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

**Deja de ser hipotético con el Módulo 7.** CU49 —el resumen diario— es un proceso
programado que corre solo, a hora fija y sin coordinarse con nadie: va a reemplazar las
listas mientras la encargada carga un parto. La limitación asumida se sostiene mientras
el sistema sea de un usuario y sin procesos de fondo, y el alcance del propio proyecto
dice que no lo va a ser.

**El cambio no es sólo borrar `static` veintiún veces.** `BuscarAnimal` (`:420`) y la
mayoría de los métodos leen las listas sin llamar antes a `Refrescar()`: sólo 41 de los
244 lo hacen. Hoy funciona porque la lista es static y alguien la cargó en una petición
anterior —lo que además esconde un error latente: recién levantado el servidor, una
pantalla que llame a `BuscarAnimal` sin pasar por un `Listar` recibe `null`—. La forma
segura es invocar `Refrescar()` en el constructor: con eso el comportamiento observable
queda igual y desaparecen los tres modos de falla.

## 2. No hay pruebas automatizadas

No existe proyecto de tests en el repositorio. El anteproyecto compromete un Plan de
Testing y la sección 2.3 del Proyecto figura como pendiente; lo que hay hoy es el guion
manual de `flujos-de-prueba.md`.

La lógica más testeable sin base de datos es la más delicada: `CalcularCategoria` en
los bordes, `ListarAscendencia`, `BuscarAncestroComun`, `VerificarConsanguinidad` y
`EstimarProduccionLactancia`.
