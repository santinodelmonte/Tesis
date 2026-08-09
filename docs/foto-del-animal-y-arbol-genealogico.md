# Foto del animal y árbol genealógico interactivo

Incremento posterior a las reglas de negocio y al tablero de indicadores. No agrega
casos de uso nuevos: le suma un dato al alta del animal (CU2) y reemplaza la forma en
que CU4 presenta el linaje.

Nada de esto está en el documento de Proyecto todavía. Al final de cada punto queda
anotado qué hay que corregir ahí.

---

## 1. La foto del animal

### 1.1 Por qué

La caravana identifica al animal pero no lo describe. Parado frente al rodeo, la
encargada tiene que confiar en que la caravana que leyó es la del animal que está
mirando, y en el árbol genealógico una pantalla llena de números no dice nada sobre
los animales que la vaca tiene atrás.

La foto es opcional en todos los casos. Un animal sin foto se comporta exactamente
como antes.

### 1.2 Dónde se carga

| Pantalla | Cuándo |
|---|---|
| **Agregar Animal** (CU2) | Al dar de alta el animal |
| **Modificar Animal** (CU3) | Para agregarla, reemplazarla o quitarla |
| **Registrar Parto** (CU18) | La cría, en el momento en que se la caravanea |

En el parto doble hay una foto por cría.

La ficha del animal (**Detalle**) la muestra arriba de todo, con un botón que lleva a
Modificar cuando hay que cambiarla. El listado de animales y el selector modal no la
muestran: son pantallas de mucho volumen y una miniatura por fila las vuelve pesadas
sin agregar nada que la ficha no resuelva.

### 1.3 Dónde se guarda

**La imagen no va a la base.** Se guarda como archivo dentro de `wwwroot/fotos` y en
la columna `foto` de `animales` queda únicamente el nombre de ese archivo.

Es la decisión habitual para imágenes: la base no crece, el respaldo del dump sigue
siendo chico y el navegador cachea la foto como cualquier otro recurso estático. La
contrapartida —y hay que decirla— es que **la carpeta de fotos se respalda aparte del
dump de MySQL**: un dump solo no alcanza para restaurar el sistema completo.

El nombre del archivo lo genera el sistema (`animal_` más un identificador único). No
se usa la caravana: dos altas del mismo animal se pisarían entre sí, y además entraría
texto cargado por el usuario en una ruta del disco.

### 1.4 Cómo viaja la imagen

La foto **no se envía como archivo adjunto** sino como texto dentro de un campo oculto
del formulario. Son dos problemas resueltos de una vez:

- **Los reenvíos.** El alta de animal reenvía el formulario para calcular la categoría
  y para confirmar las advertencias de genealogía; el parto lo reenvía para buscar la
  madre y para confirmar sus propias advertencias. El navegador vacía un campo de
  archivo en cada reenvío, así que la foto elegida se perdía sin aviso. Un campo de
  texto sobrevive.
- **El peso.** Una foto de celular pesa varios megas y en el campo hay poca señal.
  Antes de enviarla, `wwwroot/js/fotoAnimal.js` la redimensiona a 1200 px de lado
  máximo y la recomprime a JPEG. Lo que sube son unos cientos de kilobytes. No agrega
  ninguna dependencia al proyecto: lo hace el propio navegador.

Del lado del servidor, `pFotoAnimal` **reconoce el formato por los primeros bytes del
archivo y no por lo que dijo el navegador**: se está escribiendo dentro de la carpeta
que el servidor publica, y un nombre terminado en `.jpg` no prueba que el contenido sea
una imagen. Se admiten JPEG, PNG y WEBP, con un tope de 3 MB ya decodificados.

### 1.5 Archivos que quedan sin dueño

La foto se escribe en el disco **después** de que pasaron todas las validaciones, y si
la operación igual no prospera se borra. Al reemplazar una foto, la anterior se borra
recién cuando la nueva quedó guardada en la base: al revés, un error de escritura
dejaba al animal sin imagen y sin archivo.

### 1.6 En la base

```sql
ALTER TABLE animales ADD COLUMN foto VARCHAR(120) NULL AFTER motivo_baja;
```

Está en `bd/tambo_foto_animal.sql`, que **solo hay que correr si la base ya existía**.
Una base nueva ya trae la columna, porque `tambo_m0_m1.sql` la crea junto con el resto
de la tabla.

**Corregir en el Proyecto.** Sumar el campo foto a la entidad Animal del modelo de
datos y al flujo de CU2, CU3 y CU18, y aclarar en el plan de respaldo que la carpeta
`wwwroot/fotos` va aparte del dump.

---

## 2. El árbol genealógico

### 2.1 Qué había

CU4 mostraba el linaje como una tabla de siete filas escritas a mano en la pantalla:
nivel, parentesco, caravana, sexo y raza. Llegaba **hasta los abuelos y no más**,
porque las siete filas estaban puestas una por una en el HTML. Un animal con
bisabuelos cargados no tenía forma de mostrarlos.

### 2.2 Qué hay ahora

Un árbol en formato de pedigrí: el animal a la izquierda y las generaciones creciendo
hacia la derecha, la línea materna arriba y la paterna abajo. Es el formato con el que
se lee un pedigrí ganadero, y es el único sentido que escala: el árbol crece a lo
ancho, que es donde el navegador ya sabe desplazar. Hacia abajo, cada generación
duplica el alto y a partir de la cuarta se vuelve ilegible.

Cada nodo muestra la foto del animal —o la inicial de su sexo sobre el color que le
corresponde, si no tiene—, la caravana, el parentesco y la raza. El borde izquierdo
dice el sexo sin que haya que leer.

- **Se despliega sin límite.** Arranca mostrando padres y abuelos, que es hasta donde
  llegaba la tabla. Cada animal con progenitores cargados tiene un botón para seguir
  desplegando: bisabuelos, tatarabuelos, hasta donde llegue el dato. El tope técnico
  son diez generaciones.
- **Pulsar un animal abre un panel al costado** con su foto ampliada, la caravana, el
  parentesco, sexo, raza, categoría, nacimiento, edad, partos y estado, más dos
  botones: ver la ficha completa y centrar el árbol en ese animal. El árbol no se
  pierde de vista.
- **El progenitor no registrado se dibuja igual**, como un casillero punteado. Que ahí
  se corte el registro es información, no un hueco.
- **El animal dado de baja se muestra apagado pero se muestra**: sigue siendo parte del
  linaje.
- Hay zoom, desplegar todo, contraer, y un botón **"Ver como tabla"** que muestra la
  tabla de siempre. La tabla no se eliminó: es la que se imprime como registro
  genealógico y la que muestra el dato crudo.

Desde la ficha del animal, el botón "Ver Linaje" ahora **lleva la caravana**: el árbol
se dibuja directo, sin obligar a elegir de nuevo en el selector al animal que se venía
mirando.

### 2.3 Dos decisiones que vale la pena explicar

**La ascendencia entera viaja de una sola vez.** El servidor serializa todo el árbol en
un bloque JSON y el navegador lo dibuja. La alternativa —pedirle al servidor cada rama
cuando se la despliega— sería un viaje de ida y vuelta por un dato que ya está en la
memoria del servidor, porque la Controladora tiene el rodeo completo en su caché.
Desplegar una rama es, así, instantáneo.

**Las ramas se identifican por su camino y no por el id del animal.** El camino es la
secuencia de pasos desde la raíz: `""` es el animal, `"M"` la madre, `"MP"` el abuelo
materno. Hace falta porque en un rodeo con algo de consanguinidad **el mismo toro
aparece en dos ramas distintas del árbol**, y con el id como identificador desplegar
una habría desplegado la otra.

**Las llaves se miden, no se calculan con CSS.** El primer intento repartía el alto en
partes iguales entre la madre y el padre, lo que deja la geometría resuelta con bordes
fijos. No sirve: cuando una rama tiene cuatro generaciones cargadas y la otra ninguna
—que es el caso normal, porque de la línea materna siempre se sabe más— la rama corta
se lleva la mitad del alto en blanco y el árbol queda con agujeros enormes. Ahora
`arbol.js` mide dónde quedó cada nodo y coloca las líneas: cada rama ocupa lo que
necesita.

**Corregir en el Proyecto.** Reescribir la presentación de CU4: ya no es una tabla de
tres niveles sino un árbol desplegable sin límite de generaciones, con la tabla como
vista alternativa. El alcance del caso de uso no cambia; cambia cómo se muestra.

---

## 3. Archivos

| Archivo | Qué es |
|---|---|
| `bd/tambo_foto_animal.sql` | La columna `foto`, para bases ya creadas |
| `Persistencia/pFotoAnimal.cs` | Guarda y borra el archivo, valida el formato |
| `Pages/Shared/_CampoFoto.cshtml` | El control de carga, compartido por las tres pantallas |
| `Pages/Shared/CampoFotoModelo.cs` | Sus datos, y la decodificación de lo que llega |
| `wwwroot/js/fotoAnimal.js` | Redimensionado y vista previa en el navegador |
| `wwwroot/js/arbol.js` | El árbol: dibujo, despliegue, panel y llaves |
| `wwwroot/css/arbol.css` | Su estilo |

---

## 4. Corrección al margen

Al compilar para verificar este incremento apareció un error que venía de antes, en
`Pages/PagesProduccion/OrdenieLote.cshtml.cs`: **el proyecto no compilaba**.

El commit `ac6fc1b` ("El ordenie por lote guarda el total del turno y deja de restar")
eliminó la propiedad `litrosDelTanque` y su lectura del formulario, pero dejó el `if`
que la usaba —justamente la resta que ese commit decía estar sacando—. Se eliminó ese
bloque, que es lo que el commit se había propuesto hacer. El comportamiento resultante
es el que documenta el desvío D1: se guardan los litros del ordeñe completo del turno y
el control individual no se descuenta.
