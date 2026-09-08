# Estilos, accesibilidad y uso desde el celular

Este documento es la fuente para el capítulo de interfaz del informe. Están acá
las decisiones de diseño con su porqué, la paleta con los contrastes **medidos**
sobre la interfaz ya construida, y la lista de criterios de accesibilidad con el
lugar del código donde cada uno está resuelto.

---

## 1. De qué se partía

La interfaz era Bootstrap de fábrica sin retocar, y eso se notaba en tres cosas
concretas, ninguna cosmética:

- **El color no significaba nada.** `btn-warning` —el amarillo que en el resto
  del sistema quiere decir "está por vencer"— era el botón **"Volver"**, 28
  veces. `btn-success` era "Guardar", 39 veces. Con dos colores fuertes usados
  para navegar, no quedaba ningún color libre para señalar lo que importa.
- **Había 52 alineaciones escritas que no se aplicaban.** `text-left` y
  `text-right` son clases de Bootstrap 4; en la 5 no existen. Estaban en 33 y
  19 lugares respectivamente, sin efecto. Lo mismo `form-group`, que dejaba los
  campos de los formularios pegados uno al otro.
- **Cambiar el tamaño de un título era imposible desde un solo lado.** 45
  pantallas tenían `<h1 class="display-4" style="font-size: 40px">`, y un estilo
  escrito en el atributo `style` le gana a cualquier hoja de estilos.

Y ninguna de las 34 tablas del sistema —de hasta **diez columnas**— estaba
preparada para una pantalla angosta.

## 2. Cómo está organizado

Dos hojas, con una división que es la que permite cambiar la identidad visual
sin tocar ninguna pantalla:

| Archivo | Qué define |
|---|---|
| `wwwroot/css/tema.css` | De qué está hecho: colores, tipografía, densidad, y el retema de los componentes de Bootstrap |
| `wwwroot/css/site.css` | Qué forma tiene: disposición, menú lateral, encabezado de pantalla, tablas adaptables, control de foto |

Bootstrap 5.3 define sus componentes con variables CSS locales (`--bs-btn-bg` y
compañía), así que se los retematiza dándole otro valor a esas variables en la
clase que corresponde: sin recompilar Sass y sin pelear con `!important`.

## 3. La paleta

Verde de campo como color de marca, neutros levemente fríos para el resto. El
motivo del verde es práctico antes que estético: el sistema ya usaba rojo para
lo crítico, ámbar para lo que está por vencer y azul para lo informativo, así
que la marca tenía que ocupar un lugar del círculo cromático que no compitiera
con ninguno de esos tres significados.

Los neutros llevan una pizca de azul. Un gris puro al lado de un verde se ve
sucio; estos se leen como parte de la misma paleta.

### Contrastes medidos

No son valores calculados a mano sobre los códigos de color: se midieron sobre
la interfaz ya armada, leyendo el color que el navegador terminó aplicando a
cada elemento. WCAG 2.1 nivel AA pide **4,5:1** para texto normal y **3:1** para
texto grande y para elementos de interfaz.

| Combinación | Medido | AA |
|---|---:|---|
| Texto principal sobre el fondo de la aplicación | 14,49:1 | ✔ |
| Ítem del menú marcado como activo | 11,47:1 | ✔ |
| Etiqueta de preñez confirmada | 10,10:1 | ✔ |
| Etiqueta de vaca vacía | 7,97:1 | ✔ |
| Aviso ámbar (descarte de leche, vencimientos) | 7,21:1 | ✔ |
| Error de validación de un campo | 6,57:1 | ✔ |
| Texto del botón de acción principal | 6,47:1 | ✔ |
| Texto atenuado sobre blanco | 5,87:1 | ✔ |
| Encabezado de columna de una tabla | 5,31:1 | ✔ |

El más ajustado es el encabezado de columna, con 5,31:1 sobre el mínimo de 4,5.

Un detalle que valía la pena: el gris de `.text-muted` de Bootstrap no llega a
4,5:1 sobre fondo claro. Como esa clase aparece **135 veces** en el sistema, era
el incumplimiento más repetido de todos; se reemplazó por un gris propio que
mide 5,87:1.

### El color no es el único medio

Criterio 1.4.1. Cada estado se distingue además por otra vía:

- El ítem activo del menú lleva una **barra a la izquierda**, no solo un fondo
  distinto. Se sigue viendo en una impresión en blanco y negro del informe.
- Los errores de validación llevan un **símbolo de advertencia** delante del
  texto, además de ir en rojo y en negrita.
- Los estados de una fila van en etiquetas **con texto** ("Preñada", "Vacía",
  "Sin tacto"), no en puntos de color.

## 4. Tipografía y densidad

La tipografía es la del sistema operativo: Segoe UI en Windows, San Francisco en
el celular. Carga instantánea, cero archivos nuevos en el repositorio y aspecto
nativo en cada dispositivo. Lo que hace que se vea cuidada no es la familia sino
la escala, los pesos y el interlineado, que sí están definidos a mano.

Las medidas van en `rem` para que respeten el tamaño de letra que la persona
tenga configurado en el navegador, que es un criterio de accesibilidad. La regla
anterior fijaba `html { font-size: 14px }`, lo que además de achicar todo rompía
esa equivalencia.

**La densidad se decide por dispositivo de entrada, no por ancho de pantalla.**
La consulta `@media (pointer: coarse)` identifica al dedo: una tablet apaisada de
1024px se toca igual que un celular, y una ventana angosta en una PC se sigue
usando con mouse. Con mouse las filas van compactas —en una pantalla de 900px
entran doce animales—; al tacto, las mismas variables se agrandan y todo objetivo
táctil pasa a medir 44px, que es el mínimo recomendado.

Los campos de formulario pasan a 16px al tacto. No es estético: abajo de 16px el
navegador del celular hace zoom al enfocar un campo y descoloca el formulario
entero.

## 5. Las tablas en el celular

Es el problema central: diez columnas no entran en 375px de ancho.

Abajo de 768px **cada fila se convierte en una tarjeta** con "etiqueta: valor",
una debajo de otra. Un servicio se lee así, sin arrastrar al costado y sin perder
de vista la caravana:

```
Fecha                     12/03/2026
Caravana                        4521
Tipo          Inseminación artificial
Reproductor    Pajuela Holando Elite
Toro genético                  T-108
Fecha probable de parto   20/12/2026
Último tacto                 Preñada
Acciones          [Editar] [Eliminar]
```

La etiqueta la pone `wwwroot/js/tablaMovil.js`, copiando el encabezado de cada
columna dentro de cada celda. Por eso **ninguna de las 34 tablas tuvo que
tocarse**, y una tabla nueva queda adaptada sola. Es el mismo criterio de
`paginador.js`: una conducta que vale para todas las tablas se resuelve una vez.

Una tabla puede declarar `data-movil="tabla"` y quedar como tabla en cualquier
ancho. Es para las de carga —el control lechero, el ordeñe por lote—, que son de
cuatro columnas y una de ellas es el campo donde se escribe.

### Los dos detalles que hacen que siga siendo accesible

Esta transformación es donde más fácil se rompe la accesibilidad, y se rompe en
silencio:

1. **El encabezado no se esconde con `display: none`.** Eso lo sacaría del árbol
   de accesibilidad y con él se perdería la relación entre cada celda y su
   columna. Se usa la técnica de ocultamiento visual, que lo saca de la vista
   pero lo deja disponible para el lector de pantalla.
2. **Los roles de tabla se reponen explícitamente.** Pasar las celdas a
   `display: block` le quita a la tabla sus roles implícitos, y para un lector de
   pantalla deja de ser una tabla para ser una pila de textos sueltos.
   `tablaMovil.js` escribe `role="table"`, `role="row"`, `role="cell"` y
   `role="columnheader"`, así que la tabla sigue siendo una tabla en cualquier
   ancho de pantalla.

La etiqueta visible va en un elemento propio marcado `aria-hidden="true"` y no en
un `::before` de la hoja de estilos: el lector de pantalla ya anuncia la columna
a partir del encabezado, y sin ese `aria-hidden` la escucharía dos veces.

### Verificado a 375px

- Ningún desplazamiento horizontal: `scrollWidth` igual a `clientWidth`.
- Botones de acción de 44px de alto exactos.
- Campos de formulario de 45px y letra de 16px.
- Los roles de tabla siguen puestos.

## 6. Íconos

Un sprite propio en `wwwroot/img/iconos.svg` con las 21 figuras que el sistema
usa, tomadas de Bootstrap Icons 1.11.3 (licencia MIT). Son 9,8 KB contra los 200
de la fuente completa, no hay que cargar ninguna fuente y funciona sin internet,
que para un tambo importa.

Van siempre con `aria-hidden="true"` y **acompañados de texto visible**, nunca
reemplazándolo: el ícono ayuda a encontrar, el texto es el que dice qué hace. Un
botón que solo dice 🗑 obliga a adivinar, y en una acción que no se puede
deshacer eso es justamente lo que no se quiere.

## 7. Criterios de WCAG 2.1 AA y dónde están resueltos

| Criterio | Cómo se cumple | Dónde |
|---|---|---|
| 1.1.1 Contenido no textual | Todo ícono es decorativo y va con texto al lado | `_Layout`, `_AccionesRegistro` |
| 1.3.1 Información y relaciones | Encabezados anidados sin saltos (h1 → h2 → h3); tablas con sus roles; etiquetas asociadas a cada campo | todas las pantallas, `tablaMovil.js` |
| 1.4.1 Uso del color | Barra en el ítem activo, símbolo en los errores, texto en las etiquetas de estado | `tema.css`, `site.css` |
| 1.4.3 Contraste mínimo | Tabla de la sección 3, todas por encima de 4,5:1 | `tema.css` |
| 1.4.4 Cambio de tamaño del texto | Todas las medidas en `rem`, sin `font-size` fijo en píxeles en la raíz | `tema.css` |
| 1.4.10 Reajuste | A 375px no hay desplazamiento horizontal: las tablas se apilan | `site.css`, `tablaMovil.js` |
| 1.4.12 Espaciado del texto | Interlineado 1,55 y espaciados en unidades relativas | `tema.css` |
| 2.1.1 Teclado | Navegación completa por teclado; los desplegables del menú son `<button>` | `_Layout` |
| 2.3.3 Animación por interacción | Las transiciones se anulan con `prefers-reduced-motion` | `tema.css` |
| 2.4.1 Evitar bloques | Enlace "Saltar al contenido", visible al recibir el foco | `_Layout`, `tema.css` |
| 2.4.7 Foco visible | Anillo de dos capas con `:focus-visible`, con variante propia sobre la barra verde | `tema.css` |
| 2.5.5 Tamaño del objetivo | 44px mínimo en todo lo que se toca, con `pointer: coarse` | `tema.css` |
| 3.1.1 Idioma de la página | `<html lang="es">` | `_Layout` |
| 3.3.1 Identificación de errores | El motivo se muestra en texto junto al campo o arriba del formulario | todas las pantallas |
| 3.3.2 Etiquetas o instrucciones | Todo campo con su `<label>`; los que no lo llevan visible usan `visually-hidden` | `_Layout` y formularios |
| 4.1.2 Nombre, función, valor | `aria-current` en el ítem activo, `aria-expanded` en los desplegables, `aria-label` en los botones sin texto | `_Layout` |
| 4.1.3 Mensajes de estado | La confirmación del registro rápido va en una región `role="status"`: se anuncia sola al volver del guardado, sin que haya que ir a buscarla. Los campos que el evento elegido no usa se esconden con `hidden`, que los saca también del árbol de accesibilidad | `Index.cshtml`, `registroRapido.js` |

## 8. Lo que queda afuera

- **No hay tema oscuro.** Fue una decisión de alcance: un tema se verifica una
  vez y las capturas del informe salen todas del mismo lugar. La base de
  variables de `tema.css` lo deja agregable sin rehacer nada.
- **El contraste se midió sobre la guía de estilos, no sobre las 45 pantallas.**
  Las combinaciones son las mismas porque todas salen de las mismas variables,
  pero una pantalla que introduzca un color propio quedaría fuera de la
  medición.
- **La carga masiva sigue pensada para PC.** El control lechero y el ordeñe por
  lote se pueden usar en un celular, pero cargar cincuenta animales con el pulgar
  no es cómodo por más que la pantalla se adapte. La recomendación de usar una PC
  para eso sigue en pie; lo que el celular resuelve bien es consultar y corregir
  un registro suelto, que es para lo que se lo pidió.
