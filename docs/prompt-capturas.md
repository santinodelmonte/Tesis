# La sesión de capturas — todo lo que hay que tener a mano

Esto se hace **en una sola sesión**, con el sistema andando en tu máquina. Son 112
capturas: **42 salen solas** y en **68 el script te espera** a que cargues, filtres o
confirmes algo. Las **2 restantes son fotos del teléfono**.

Calculá entre una hora y media y dos horas. Se puede cortar y seguir: una captura que ya
existe no se vuelve a sacar.

---

## 1. Antes de arrancar — cinco cosas

**a) Las credenciales, una sola vez.** Desde el 15/09 no viajan en el repositorio:

```bash
dotnet user-secrets init --project Tesis/Tesis.csproj
dotnet user-secrets set "ConnectionStrings:Tambo" "server=localhost; port=3306; database=tambo; uid=root; pwd=; CharSet=utf8mb4;" --project Tesis/Tesis.csproj
dotnet user-secrets set "Seguridad:Usuario" "sofia" --project Tesis/Tesis.csproj
dotnet user-secrets set "Seguridad:Contrasena" "tambo2026" --project Tesis/Tesis.csproj
```

Si quedan vacías el sistema arranca y no deja entrar a nadie. Detalle en `bd/LEEME.md`,
puntos 5 y 6.

**b) La base, recién cargada.** `bd/CreacionDb.sql` y después `bd/DatosPrueba.sql`. **Esto
importa más de lo que parece**: el juego de datos ancla el rodeo a la fecha en que se
carga, así que las alertas y los vencimientos quedan vigentes. Si la base lleva días
cargada, el descarte de leche de la `115` puede haberse vencido y **`m2-cu12-descarte` es
la captura que cierra el circuito entre sanidad y producción**: sin ella falta la mejor
evidencia del sistema.

**c) El token del bot**, si querés las capturas de Telegram:

```bash
dotnet user-secrets set "Telegram:Token" "EL_TOKEN" --project Tesis/Tesis.csproj
```

Sin token la pantalla de notificaciones avisa y no deja vincular; `m7-configuracion-bot`,
`t-telegram-vinculado` y las dos del teléfono quedan afuera.

**d) Playwright**, una sola vez:

```bash
pip install playwright
python -m playwright install chromium
```

**e) El sistema levantado** desde Visual Studio. Anotá el puerto: si no es 5000, pasalo
con `--base`.

---

## 2. Cómo se corre

```bash
python docs/sacar_capturas.py --base http://localhost:5000
```

Se abre un Chromium — **no lo cierres**. El script inicia sesión solo y va avisando:

- **Las que salen solas** no te piden nada: aparece `[12/110] m1-cu10-lista` y sigue.
- **Las que te esperan** imprimen la instrucción y se quedan en
  `[Enter cuando la pantalla este lista, "s" para saltarla]`. Hacé lo que pide **en el
  navegador que abrió el script** y volvé a la terminal a apretar Enter.
- Si una no te sale, escribí `s` y seguí. Al final el script te dice cuáles salteaste y
  las podés repetir después con `--rehacer`.

Para trabajar por partes: `--solo m5` hace sólo el módulo 5, `--solo t-` sólo la evidencia
de las pruebas, `--solo mov` sólo el celular. Para repetir una o varias:

```bash
python docs/sacar_capturas.py --rehacer m3-cu24-parto,t-parto-efecto
```

Para ver el plan entero sin abrir nada: `python docs/sacar_capturas.py --guion`
(y `--listar` dice qué falta).

---

## 3. Chuleta del rodeo

Los animales que te va a pedir, todos de `DatosPrueba.sql`:

| Caravana | Para qué aparece |
|---|---|
| `115` | Vaca con mastitis en tratamiento y **descarte de leche vigente**: la ficha integral, el ordeñe del que queda afuera, el celo y la inseminación |
| `152` | Su padre es `7HO12165`: es el par con el que se muestra la consanguinidad |
| `7HO12165` | Toro, padre de `152` |
| `136` | Vaca **seca** y próxima a parir: el rechazo del control lechero y después el parto |
| `180` | La cría que nace del parto de `136` |
| `102` | Servida el 20/06: el tacto pendiente. Después se le registra el secado |
| `108` | Dermatitis digital: el diagnóstico y el tratamiento con su carencia |
| `177` | Ternera de cuatro meses: el celo rechazado por edad, y la brucelosis |
| `178` | El descorne con pasta cáustica |
| `158` | Vaquillona: la monta natural y el tratamiento preventivo |
| `130` | El control lechero puntual |
| `160` | La baja por venta |
| `140` | Aparece en alertas de parto **sólo si subís «Parto próximo» a 30 días** |
| `133` | Candidata a descarte: dos partos y sin preñez |
| `200`, `201` | Las dos altas que vas a cargar vos. La `200` se guarda (hace falta después para la lactancia); la `201` **no** |
| `29HO18296` | La pajuela con stock crítico |

Y dos productos: **ivermectina** (stock crítico, se repone con +10) y
**oxitetraciclina** (para el historial de movimientos).

---

## 4. Tres momentos en los que el orden importa

El script ya los tiene en cuenta, pero conviene que sepas por qué, porque si te salteás
uno la captura siguiente sale vacía:

1. **`m5-cu37-critico` y `t-stock-antes` salen antes de reponer la ivermectina.** El
   ingreso vacía la alerta; el par antes/después es lo que explica para qué sirve.
2. **Las alertas de secado se sacan después de confirmar las preñeces** en el tacto. Con
   el rodeo original esa pantalla está vacía.
3. **El efecto del parto va después del parto**: la ficha de `136` en lactancia y el
   linaje de `180` armado solo. Lo mismo con el calendario sanitario después de vacunar
   a `177` y después de guardar el plan clostridial.

Las dos últimas del recorrido **cierran la sesión** (`t-acceso-directo` y
`t-acceso-atras`): van al final a propósito.

---

## 5. Aprovechá la misma sesión para tres cosas más

Vas a tener el sistema andando con el rodeo cargado. Es el único momento en que se pueden
hacer, y volver a armarlo después cuesta lo mismo que hacerlo ahora:

**a) La columna «Resultado» de las pruebas.** `docs/seccion-2-3-pruebas.md` tiene 131
renglones con su dato y su resultado esperado, y la última columna vacía. Se completa con
`Ok` o con lo que haya pasado. Muchos de los recorridos coinciden con las capturas `t-`,
así que se hace en la misma pasada.

**b) Las dos fotos del teléfono.** Guardalas en `docs/capturas/` con estos nombres
exactos: `m7-resumen-telegram.png` y `t-telegram-resumen.png`.

**c) Las fotos del anexo.** Los cuadernos y el pizarrón con que se lleva el tambo hoy. Es
la evidencia de la «Presentación del problema» y son cinco minutos.

---

## 6. Cuando termines

```bash
python docs/verificar_capturas.py
```

Tiene que decir **sacadas: 112 de 112**. Si faltan, las nombra.

```bash
python docs/editar_proyecto.py
```

Regenera `Proyecto_v7.docx` colocando cada captura en su lugar con su pie. Los
`[FALTA LA CAPTURA: …]` que hoy están en el documento desaparecen solos: el script busca
el `.png` y, si lo encuentra, pone la imagen.

Después, `git add` y commit. Las capturas pesan, así que van en un commit propio.

---

## 7. Si algo se rompe

| Qué pasa | Qué es |
|---|---|
| El script no encuentra `#usuario` | El sistema no está levantado, o el puerto no es el de `--base` |
| Inicia sesión y rebota al login | Las credenciales de user-secrets no coinciden con las que le pasás. Usá `--usuario` y `--clave` |
| Una lista aparece vacía | La base no tiene `DatosPrueba.sql`, o es una de las tres del punto 4 y te salteaste el paso previo |
| Una captura sale cortada | El script fotografía la página entera; si querés sólo un pedazo, sacala a mano y guardala con el mismo nombre |
| Se cerró el navegador a mitad | Volvé a correrlo: retoma donde quedó, las que ya existen no se repiten |

Y si preferís que te acompañe mientras la corrés, pegá esto en un chat nuevo de Claude
Code abierto en el repositorio:

> Estoy corriendo `docs/sacar_capturas.py` para sacar las 112 capturas del manual y de
> las pruebas. Leé `docs/prompt-capturas.md`, `docs/guion-capturas.md` y
> `docs/seccion-2-3-pruebas.md`. Voy a ir contándote qué sale distinto de lo que el guion
> espera; ayudame a decidir si es un problema del juego de datos, del guion o del sistema.
> No levantes el sistema vos: lo tengo abierto en Visual Studio.
