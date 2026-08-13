# Flujos de prueba

Guion para recorrer el sistema a mano. Cada flujo dice qué hay que hacer, con qué
datos concretos y qué tiene que pasar. Los datos no son de relleno: se apoyan en el
rodeo que carga `bd/tambo_datos_prueba.sql`, así que las pantallas quedan coherentes
después de cargarlos y se puede seguir probando encima.

La fecha de referencia del juego de datos es el **09/08/2026**. Todo lo que sigue se
carga con fecha **11/08/2026** salvo que se aclare otra cosa. Si se prueba en otra
fecha, correr las fechas de carga en el mismo sentido.

Los flujos están en orden: varios se apoyan en el anterior (el celo habilita el
servicio, el tacto confirma la preñez, el tratamiento saca la vaca del lote). Se
pueden hacer sueltos, pero en orden se lee mejor.

---

## 0. Preparación

1. Crear la base y cargar el rodeo de prueba:

```powershell
.\bd\actualizar.ps1 -DatosPrueba
```

   O a mano: `mysql -u root -p < bd/tambo.sql` y después
   `mysql -u root -p < bd/tambo_datos_prueba.sql`.

2. Levantar el sitio:

```bash
dotnet run --project Tesis
```

3. Abrir `https://localhost:7283` (o `http://localhost:5174`).

### Datos del rodeo que conviene tener a mano

| Caravana | Qué es | Estado |
|---|---|---|
| `T-01` | Toro del rodeo, en pie | Sirve para monta natural |
| `101`, `108`, `124` | Vacas en ordeñe | Preñadas |
| `102` | Vaca en ordeñe, 11 meses de lactancia | Servida el 20/06, tacto pendiente |
| `115` | Vaca en ordeñe | Vacía, con mastitis en tratamiento hasta el 17/08 |
| `121` | Vaca en ordeñe | Servida el 18/07 |
| `130`, `133` | Vacas en ordeñe | Vacías |
| `136` | Vaca seca | Preñada, pare el 18/08 |
| `140` | Vaca seca | Preñada, pare el 10/09 |
| `152`, `155`, `158` | Vaquillonas | `152` es hija del toro `7HO12165` |
| `177` | Ternera | Le falta la vacuna de brucelosis |
| `178` | Ternero | Sin descornar |
| `160` | Novillo | Candidato a venta |
| `112` | Vaca dada de baja el 18/03/2026 | No tiene que aparecer en las listas |

---

## 1. Iniciar sesión

**Pasos.** Entrar al sitio. Redirige solo al login.

| Campo | Dato |
|---|---|
| Usuario | `sofia` |
| Contraseña | `tambo2026` |

Botón **Ingresar**.

**Esperado.** Va al inicio, el menú superior muestra los seis módulos y el nombre
`sofia` con el botón **Cerrar Sesion**.

**Variantes que tienen que fallar.**

- Usuario `sofia`, contraseña `tambo2025` → *"Usuario o contraseña incorrectos!"*.
- Campos vacíos → *"El usuario es requerido"*.
- Sin haber iniciado sesión, pegar en el navegador
  `https://localhost:7283/PagesAnimal/ListaAnimales` → manda al login.
- **Cerrar Sesion** y volver atrás con el navegador → manda al login otra vez.

---

## 2. Dar de alta un animal

Una vaca comprada que entra al rodeo ya en producción.

**Pasos.** Menú **Animales → Lista de Animales → Agregar Animal**.

| Campo | Dato |
|---|---|
| Numero de Caravana | `200` |
| Fecha de Nacimiento | `2021-06-10` |
| Sexo | Hembra |
| Raza | Holando |
| Madre / Padre | Dejar vacíos (viene de otro establecimiento) |
| Partos Registrados | `2` |
| Categoria | Dejar en "La calcula el sistema" |

Apretar **Calcular Categoria** antes de guardar: tiene que proponer **Vaca** (tiene
partos). Después **Agregar**.

**Esperado.** Vuelve a la lista con `200` cargada como Vaca Holando.

**Variantes que tienen que fallar.**

- Repetir el alta con la caravana `200` → *"El número de caravana ya existe en el
  sistema!"*.
- Fecha de nacimiento `2027-01-01` → *"La fecha de nacimiento no puede ser futura!"*.
- Caravana vacía o raza sin elegir → *"El número de caravana y la raza son
  obligatorios!"*.

**Variante con advertencia (no bloquea, avisa).** Alta de la caravana `201`, nacida
`2026-07-01`, hembra, Holando, con **Madre** `177` (la ternera nacida el 06/04/2026):
el sistema no la registra y muestra la advertencia de genealogía (la madre no tenía
edad para parir). El botón **Guardar de todos modos** la carga igual.

---

## 3. Buscar, filtrar y ver la ficha

**Pasos.** **Animales → Buscar y Filtrar**.

| Filtro | Dato |
|---|---|
| Categoria | Vaca |
| Estado productivo | En lactancia |

Buscar.

**Esperado.** Salen las ocho vacas en ordeñe (`101`, `102`, `108`, `115`, `121`,
`124`, `130`, `133`). La `112`, dada de baja, no aparece.

Después filtrar por **Edad desde** `5` y **Edad hasta** `2` → *"El rango etario es
incorrecto"*.

**Ficha.** Desde la lista, entrar al detalle de `115`. Tiene que mostrar: los datos
del animal, el diagnóstico de mastitis en tratamiento, el descarte de leche vigente
hasta el 17/08, sus partos, sus servicios y su producción.

---

## 4. Linaje y consanguinidad

**Linaje.** **Animales → Consultar Linaje**, elegir `152`. Tiene que armar el árbol
con madre `101` y padre `7HO12165`.

**Consanguinidad.** **Animales → Verificar Consanguinidad**.

| Campo | Dato |
|---|---|
| Hembra | `152` |
| Reproductor | `7HO12165` |

**Esperado.** Avisa que hay parentesco: `7HO12165` es el padre de `152`.

Repetir con hembra `152` y reproductor `29HO18296` → sin parentesco. Elegir el mismo
animal en los dos campos → *"No puede verificar un animal contra sí mismo!"*.

---

## 5. Modificar un animal

**Pasos.** **Animales → Lista de Animales**, editar `200`. Cargarle una foto desde el
campo de foto (o sacarla con la cámara) y guardar.

**Esperado.** La ficha de `200` muestra la foto, y también aparece en el árbol
genealógico de sus crías cuando las tenga.

---

## 6. Ordeñe por lote

**Pasos.** **Produccion → Ordeñe por Lote**.

| Campo | Dato |
|---|---|
| Fecha | `2026-08-10` |
| Turno | Turno 1 |
| Litros del ordeñe | `88.10` |
| Animales del lote | Las que vienen tildadas |

**Esperado.** La `115` **no** viene tildada ni se puede sumar: tiene descarte de leche
vigente hasta el 17/08. Al guardar, el ordeñe queda en el historial.

**Variantes que tienen que fallar.**

- Repetir fecha `2026-08-10` y Turno 1 → avisa que ya hay un ordeñe cargado para esa
  fecha y ese turno.
- Fecha `2026-12-01` → *"La fecha del ordeñe no puede ser futura!"*.
- Destildar todos los animales → *"El lote tiene que tener al menos un animal!"*.
- Litros `0` o negativos → *"Los litros tienen que ser un valor positivo y
  coherente!"*.

**Modificación.** Desde el historial, editar el ordeñe del `2026-08-10` Turno 1,
sacar a la `133` del lote y guardar. El registro tiene que quedar con una vaca menos.

---

## 7. Ordeñe individual

**Pasos.** **Produccion → Ordeñe Individual**.

| Campo | Dato |
|---|---|
| Fecha | `2026-08-10` |
| Turno | Turno 2 |
| Caravana | `130` |
| Litros | `8.90` |

**Esperado.** Guarda y se ve en el historial de producción de `130`.

**Variantes que tienen que fallar.**

- Repetir el mismo animal, fecha y turno → avisa que ya hay un control cargado, con
  los litros que tiene.
- Caravana `136` (vaca seca) → *"El animal no se encuentra en lactancia: su estado
  productivo es Seca."*.
- Litros `250` → supera el máximo por control configurado (100 litros) y lo rechaza.

---

## 8. Control lechero (carga masiva)

**Pasos.** **Produccion → Control Lechero (carga masiva)**.

| Campo | Dato |
|---|---|
| Fecha del control | `2026-08-11` |
| Turno | Turno 1 |

Litros vaca por vaca:

| Caravana | Litros |
|---|---|
| `101` | `10.10` |
| `102` | `8.20` |
| `108` | `15.00` |
| `121` | `13.40` |
| `124` | `14.10` |
| `130` | `11.50` |
| `133` | `12.90` |

Guardar.

**Esperado.** Carga los siete controles de una sola vez y quedan imputados a la
lactancia en curso de cada vaca.

**Variantes que tienen que fallar.** Guardar sin cargar ningún litro → *"Cargue los
litros de al menos un animal!"*. Fecha futura → la rechaza.

---

## 9. Historial y métrica mensual

- **Produccion → Historial de Produccion**, rango `2026-07-01` a `2026-08-11`, por
  lote: tiene que mostrar los catorce ordeñes del juego de datos más los que se
  cargaron recién.
- Rango invertido (`2026-08-11` a `2026-07-01`) → *"El rango de fechas es
  invalido"*.
- **Produccion → Metrica Mensual**, mes `Agosto` año `2026`: total del mes, promedio
  por ordeñe y promedio por vaca. La leche del 05/08 no se tiene que contar dos veces
  (ese día hay lote y controles individuales del mismo ordeñe).

---

## 10. Tacto: confirmar la preñez de la `102`

**Pasos.** **Reproduccion → Tactos Pendientes**. La `102` tiene que aparecer: fue
servida por monta natural el 20/06 y ya pasaron más de 35 días.

Entrar a **Registrar Tacto**.

| Campo | Dato |
|---|---|
| Caravana | `102` |
| Fecha del tacto | `2026-08-11` |
| Resultado | Preñada |
| Observaciones | `Preñez confirmada. Servicio del toro T-01.` |

Apretar **Ver servicio** antes de guardar: tiene que mostrar el servicio del 20/06 con
el toro `T-01` y el parto proyectado para el 30/03/2027.

**Esperado.** `102` pasa a reproductivo **Preñada** y sigue **En lactancia** (el tacto
no toca el estado productivo). Sale de la lista de tactos pendientes.

**Variante.** Registrar el tacto de `121` con resultado **Vacía**: pasa a Vacía y
vuelve a aparecer en **Vacas para Servir** cuando entre en celo.

---

## 11. Secado de la `102`

Ya está confirmada preñada y lleva casi once meses en leche: se seca.

**Pasos.** **Produccion → Registrar Secado**.

| Campo | Dato |
|---|---|
| Caravana | `102` |
| Fecha de secado | `2026-08-11` |

**Esperado.** Cierra la lactancia abierta con esa fecha, la vaca queda **Seca** y
**Preñada**, y desaparece del lote de ordeñe en las cargas siguientes.

**Variantes que tienen que fallar.**

- Repetir el secado de `102` → *"El animal no se encuentra en lactancia, así que no
  hay nada que secar!"*.
- Fecha futura → la rechaza.

**Alertas de Secado.** Entrar a **Produccion → Alertas de Secado**: lista las vacas en
lactancia cuya fecha probable de parto está a menos de 60 + 15 días. Con los datos de
prueba está vacía; se puebla después de que se confirmen preñeces nuevas.

---

## 12. Abrir una lactancia a mano

Para la vaca comprada en el flujo 2, que entró al rodeo ya en producción.

**Pasos.** **Produccion → Lactancias → Abrir Lactancia**.

| Campo | Dato |
|---|---|
| Caravana | `200` |
| Numero de lactancia | Apretar **Proponer numero** → tiene que sugerir `3` |
| Fecha de inicio | `2026-05-20` |

**Esperado.** `200` pasa a **En lactancia** y aparece en el lote de ordeñe y en el
control lechero.

**Variante que tiene que fallar.** Intentar abrirle una lactancia a `101` → *"El
animal ya tiene una lactancia abierta!"*.

---

## 13. Registrar un celo

**Pasos.** **Reproduccion → Registrar Celo**.

| Campo | Dato |
|---|---|
| Caravana | `115` |
| Fecha de deteccion | `2026-08-11` |
| Observaciones | `Celo firme, monta a otras vacas del lote.` |

**Esperado.** Queda en **Celos Detectados** y `115` aparece en **Vacas para Servir**.

**Variantes que tienen que fallar.**

- Caravana `177` (ternera de 4 meses) → la rechaza por edad mínima de celo (9 meses).
- Caravana `T-01` (macho) → *"La caravana corresponde a un macho: no se puede
  registrar un celo!"*.
- Caravana `112` (dada de baja el 18/03/2026) con fecha de hoy → la rechaza por ser
  posterior a la baja.

---

## 14. Registrar un servicio

**Pasos.** **Reproduccion → Vacas para Servir**: tienen que estar `115`, `130`, `133`
y la vaquillona `158`. Entrar a **Registrar Servicio**.

**Inseminación artificial:**

| Campo | Dato |
|---|---|
| Caravana | `115` |
| Fecha del servicio | `2026-08-11` |
| Tipo de servicio | Inseminación artificial |
| Pajuela | `Pajuela Holando 7HO12165` |
| Fecha probable de parto | Apretar **Recalcular** → `2027-05-21` |
| Observaciones | `Repite servicio, el tacto del 01/07 dio vacía.` |

**Esperado.** Guarda, `115` pasa a **Servida** y el stock de la pajuela baja de 17 a
16, con un egreso en el historial de movimientos con el motivo *"Inseminación de la
caravana 115"*.

**Monta natural:**

| Campo | Dato |
|---|---|
| Caravana | `158` |
| Fecha del servicio | `2026-08-11` |
| Tipo de servicio | Monta natural |
| Toro del rodeo | `T-01` |

Al cambiar el tipo, el selector de pajuela se esconde y aparece el de toro: son
excluyentes.

**Variantes que tienen que fallar o advertir.**

- Servicio a la vaquillona `152` con la pajuela `7HO12165`: no lo guarda y advierte
  que es su propio padre. **Registrar de todos modos** lo fuerza.
- Servicio a la ternera `177` → la rechaza por edad mínima de servicio (13 meses).
- Servicio sin elegir toro ni pajuela → lo rechaza.

---

## 15. Registrar un parto

La `136` está preñada y pare el 18/08: pare una semana antes.

**Pasos.** **Reproduccion → Alertas de Parto**: tienen que aparecer `136` (18/08) y,
según la anticipación configurada, `140` (10/09). Entrar a **Registrar Parto**.

**Madre:**

| Campo | Dato |
|---|---|
| Caravana | `136` — apretar **Cargar datos** |
| Fecha del parto | `2026-08-11` |
| Tipo de parto | Normal |
| Observaciones | `Parto sin asistencia. Cría de 40 kg.` |

Al cargar los datos tiene que mostrar el cartel con el servicio del 08/11/2025 y el
parto proyectado para el 18/08/2026.

**Cría:**

| Campo | Dato |
|---|---|
| Numero de caravana | `180` |
| Sexo | Hembra |
| Raza | Holando |
| Padre | Ya viene propuesto: `7HO12165`, el toro de la pajuela del servicio |
| Foto | Opcional |

Dejar **Parto doble** sin tildar. **Confirmar Parto**.

**Esperado.**

- `136` queda **En lactancia** y **Vacía**, con un parto más.
- Se abre su lactancia número 2 con fecha 11/08.
- Se da de alta `180` como **Ternera**, nacida el 11/08/2026, con madre `136` y padre
  `7HO12165`. El linaje de `180` tiene que armarse solo.
- `136` sale de las alertas de parto y entra al lote de ordeñe.

**Variantes.**

- Tildar **Parto doble** y cargar una segunda cría `181`, macho, Holando: suma **un**
  parto y **una** lactancia a la madre, pero da de alta los dos animales.
- Registrar el parto de `130` (que figura **Vacía**): no lo bloquea, pero avisa que el
  animal no figura preñado. Es a propósito.
- Registrar el parto de una vaca en lactancia: avisa que va a cerrar la lactancia
  abierta con la fecha del parto y abrir la nueva.
- Cría sin caravana → *"El numero de caravana de la cria es obligatorio!"*.

---

## 16. Diagnóstico y tratamiento

**Diagnóstico.** **Sanidad → Registrar Diagnostico**.

| Campo | Dato |
|---|---|
| Caravana | `108` |
| Fecha | `2026-08-11` |
| Estado | En tratamiento |
| Enfermedad | `Dermatitis digital (cojera), miembro posterior derecho` |

**Tratamiento.** **Sanidad → Registrar Tratamiento**.

| Campo | Dato |
|---|---|
| Diagnostico que lo origina | El de `108` del 11/08 |
| Producto aplicado | `Oxitetraciclina 20% LA (frasco 100 ml)` |
| Fecha de inicio | `2026-08-11` |
| Duracion (dias) | `3` |
| Dosis diaria | `20 ml intramuscular cada 24 h` |
| Unidades a descontar | `2` |
| Plan sanitario que cumple | Fuera de plan |
| Descarte de leche hasta | Apretar **Calcular** → `2026-08-21` |

El descarte sale de la fecha de inicio + 3 días de tratamiento + 7 días de carencia
del producto.

**Esperado.**

- El stock de oxitetraciclina baja de 8 a 6, con el egreso anotado.
- `108` queda con descarte de leche vigente hasta el 21/08.
- Al cargar el ordeñe por lote del 11/08, `108` **no** aparece disponible para sumar
  al lote. Ese es el control que cierra el circuito entre sanidad y producción.
- La ficha sanitaria de `108` muestra el diagnóstico con su tratamiento colgado.

**Variante — tratamiento preventivo.** Dejar el diagnóstico en "Sin diagnostico
(tratamiento preventivo)" y elegir la caravana `158`, producto
`Ivermectina 1% (frasco 500 ml)`, plan `Desparasitacion`. Tiene que aceptarlo: el
preventivo no necesita diagnóstico.

---

## 17. Vacunación

La ternera `177` es la que tiene la brucelosis pendiente en el calendario.

**Pasos.** **Sanidad → Calendario Sanitario**: `177` tiene que figurar pendiente de
brucelosis, y `178` pendiente de descorne. Entrar a **Registrar Vacunacion**.

| Campo | Dato |
|---|---|
| Caravana | `177` |
| Vacuna aplicada | `Vacuna Brucelosis cepa 19 (dosis)` |
| Fecha de aplicacion | `2026-08-11` |
| Plan sanitario que cumple | `Brucelosis cepa 19` |

**Esperado.** El stock de la vacuna baja de 8 a 7 con el egreso "Vacunacion", y `177`
sale del calendario: la brucelosis es de aplicación única en la vida.

---

## 18. Descorne

**Pasos.** **Sanidad → Registrar Descorne**.

| Campo | Dato |
|---|---|
| Caravana | `178` |
| Fecha del procedimiento | `2026-08-11` |
| Metodo utilizado | Pasta caustica |
| Plan sanitario que cumple | `Descorne` |
| Observaciones | `A los setenta y cinco dias de vida.` |

**Esperado.** `178` sale del calendario sanitario. El descorne no descuenta insumo:
el plan está configurado sin producto, que es el caso previsto.

---

## 19. Plan sanitario

**Pasos.** **Sanidad → Planes Sanitarios → Agregar Plan**.

| Campo | Dato |
|---|---|
| Nombre del plan | `Vacuna clostridial` |
| Tipo de procedimiento | Vacunacion |
| Insumo a aplicar | Cualquiera de tipo Vacuna |
| Periodicidad (dias) | `365` |
| Edad de inicio (meses) | `4` |
| Categorias alcanzadas | Ternera y Ternero |
| Plan activo | Sí |

**Esperado.** En el **Calendario Sanitario** aparecen como pendientes las crías de más
de 4 meses (`171`, `174`, `175`, `177`). Si se deja el plan **sin** ninguna categoría
tildada, alcanza a todo el rodeo: la ausencia de categorías es lo que lo hace general,
como el plan de aftosa.

---

## 20. Insumos y stock

**Alta de insumo.** **Insumos → Agregar Insumo**.

| Campo | Dato |
|---|---|
| Nombre | `Cefquinoma intramamaria (jeringa)` |
| Tipo | Medicamento |
| Cantidad de la partida | `20` |
| Vencimiento de la partida | `2028-03-31` |
| Stock minimo | `8` |
| Periodo de carencia (dias) | `5` |

**Ingreso de stock.** **Insumos → Ingreso de Stock**. La ivermectina está en 3 con un
mínimo de 5.

| Campo | Dato |
|---|---|
| Insumo | `Ivermectina 1% (frasco 500 ml)` |
| Cantidad | `10` |
| Fecha del ingreso | `2026-08-11` |
| Vencimiento de la partida | `2027-10-31` |
| Motivo | `Compra a veterinaria La Rural` |

**Esperado.** El stock pasa a 13 y la ivermectina desaparece de **Alertas de Stock
Critico**.

**Stock mínimo.** **Insumos → Configurar Stock Minimo**: `Pajuela Holando 29HO18296`,
mínimo `2`. Queda en 3 sobre un mínimo de 2 y sale de la alerta.

**Alertas.**

- **Alertas de Stock Critico**: antes de estos dos cambios tenía que listar la
  ivermectina y la pajuela `29HO18296`. Después tiene que quedar vacía.
- **Alertas de Vencimiento**: tienen que aparecer la partida de vacuna antiaftosa
  (vence 31/08) y la de ivermectina (vence 02/09), las dos dentro de los 30 días
  configurados.
- **Historial de Movimientos**: filtrando por la oxitetraciclina se tiene que ver el
  ingreso del 12/05 y los egresos por tratamiento, incluido el del flujo 16.

**Variantes que tienen que fallar.** Ingreso con cantidad `0` → *"La cantidad tiene
que ser mayor a cero!"*. Ingreso con fecha futura → la rechaza. Alta de insumo sin
nombre → lo rechaza.

---

## 21. Indicadores y descarte

- **Indicadores → Indicadores del Rodeo**: composición del rodeo por categoría,
  promedio de litros, intervalo entre partos, porcentaje de preñez. Los números tienen
  que moverse después de todo lo que se cargó (una vaca menos en ordeñe por el secado
  de `102`, una vaca más por el parto de `136`).
- **Indicadores → Candidatas a Descarte**: tiene que proponer a las vacas con bajo
  rendimiento o problemas reproductivos repetidos. `133`, con dos partos y sin preñez
  confirmada, es la candidata típica del juego de datos.

---

## 22. Configuración del establecimiento

**Pasos.** Menú **Configuracion**. Cambiar **Parto proximo (dias)** de `15` a `30` y
guardar.

**Esperado.** Volver a **Reproduccion → Alertas de Parto**: ahora `140` (pare el
10/09) también tiene que aparecer, porque la ventana se amplió a 30 días.

Otras pruebas rápidas sobre la misma pantalla:

- **Litros maximos por control individual**: bajarlo a `12` y volver a cargar un
  ordeñe individual de `15` litros → lo rechaza.
- **Dias para el tacto**: subirlo a `60` → la lista de **Tactos Pendientes** se achica.
- Cargar un valor fuera de rango (por ejemplo, edad mínima al servicio `3`) → lo
  rechaza con el motivo.

Al terminar, dejar los valores como estaban (`15`, `100`, `35`) para no arrastrar la
configuración a las pruebas siguientes.

---

## 23. Baja de un animal

**Pasos.** **Animales → Lista de Animales**, entrar a `160` (el novillo) y **Registrar
Baja**.

| Campo | Dato |
|---|---|
| Motivo de Salida | Venta |
| Observaciones | `Venta a frigorífico. Remito 4471.` |

**Esperado.** La baja es lógica: `160` sale de las listas y de los desplegables, pero
su ficha, su historia sanitaria y su lugar en el árbol genealógico se conservan. Se lo
puede seguir encontrando con el filtro de animales dados de baja.

**Variante que tiene que fallar.** Volver a dar de baja el mismo animal → *"No se pudo
dar de baja el animal. Verifique que siga activo!"*.

---

## Cómo dejar todo como estaba

Volver a correr el juego de datos borra lo cargado en estas pruebas y deja el rodeo en
el estado original:

```bash
mysql -u root -p < bd/tambo_datos_prueba.sql
```
