# Prompt — del anteproyecto y el proyecto al documento final

Este archivo **es** el prompt. Para arrancar una sesión de trabajo alcanza con:

> Leé `docs/prompt-documento-final.md` y seguí con la Fase 2, sección 2.3.

No hace falta pegar nada más: acá están el objetivo, la referencia, las reglas y
el estado de cada sección.

---

## 1. Objetivo

Llegar a **un solo documento** con la forma de `EjemploTesis.pdf` —la tesis que
dio el tutor como modelo—, partiendo de los dos que ya existen:

```
Anteproyecto_v6.docx  ──┐
                        ├──►  Tesis.docx  (1. Anteproyecto / 2. Proyecto / Glosario / Bibliografía / Anexo)
Proyecto_v6.docx      ──┘
```

Tres fases, **en este orden y sin solaparlas**:

- **Fase 1 — Terminar el código.** El Módulo 7 completo y lo que arrastra. Hasta
  que el sistema no esté terminado no se toca la documentación.
- **Fase 2 — Cerrar el Proyecto.** Escribir las secciones 2.3 a 2.9, hoy con la
  palabra «Pendiente.» y nada más.
- **Fase 3 — Unificar.** Renumerar el anteproyecto como `1.x`, concatenarlo con
  el proyecto, rehacer índice, portada y numeración de páginas.

**La regla de las fases es la que ordena todo lo demás.** El documento describe
el sistema que existe; documentar un sistema que todavía se está moviendo es
trabajo que hay que rehacer. Con el Módulo 7 en curso cambian el MER, los
diagramas de secuencia, el diccionario de clases, las pantallas del manual y lo
que se puede probar. Se termina de programar, y recién entonces se escribe.

Lo único que **sí** se puede adelantar en paralelo está en el punto 9: son las
tres cosas que no dependen del código.

---

## 2. La referencia manda

El ejemplo del tutor tiene 229 páginas y esta estructura:

| | Sección | Páginas |
|---|---|---|
| **1** | **Anteproyecto** | 7–60 |
| 1.1 | Introducción | 7 |
| 1.2 | Presentación del Cliente | 8 |
| 1.3 | Presentación del Problema | 12 |
| 1.4 | Lista de Necesidades | 13 |
| 1.5 | Actores Involucrados | 14 |
| 1.6 | Objetivos | 15 |
| 1.7 | Lista de Requerimientos | 16 |
| 1.8 | Descripción del Entorno | 18 |
| 1.9 | Alcances y Limitaciones | 21 |
| 1.10 | Estudio de Alternativas | 23 |
| 1.11 | **Estimación del esfuerzo** | 29 |
| 1.12 | Análisis de riesgo | 35 |
| 1.13 | Plan de Proyecto | 41 |
| 1.14 | Compromiso de trabajo | 59 |
| **2** | **Proyecto** | 61–226 |
| 2.1 | Análisis | 61 |
| 2.2 | Diseño | 62–151 |
| 2.3 | **Pruebas** | 152–168 (17 pág.) |
| 2.4 | **Manual de Usuario** | 169–215 (47 pág.) |
| 2.5 | **Deployment** | 216 (1 pág.) |
| 2.6 | **Política de Seguridad y Respaldos** | 217 (1 pág.) |
| 2.7 | **Plan de contingencia** | 218 (1 pág.) |
| 2.8 | **Grado de satisfacción del cliente** | 219 (1 pág.) |
| 2.9 | **Conclusiones** | 220–226 (7 pág.) |
| | Glosario | 227 |
| | Bibliografía utilizada | 228 |
| | Anexo | 229 |

**El criterio de profundidad lo fija el ejemplo, no nuestro apetito.** Antes de
escribir cualquiera de las secciones 2.3 a 2.9 hay que **leer la sección
equivalente del ejemplo** y copiar su forma: qué tablas usa, cuántas capturas
pone, si numera los casos de prueba, si el manual va por pantalla o por tarea.
Si el ejemplo resuelve el Deployment en una página, nosotros también.

Esto vale sobre todo para **2.3 Pruebas**: se hace del mismo tipo que el
ejemplo. Si el ejemplo documenta pruebas manuales de caja negra y caja blanca,
nosotros documentamos pruebas manuales de caja negra y caja blanca, y si algún
documento nuestro promete pruebas automatizadas, **se corrige el documento**.

> **Bloqueante para 2.3–2.9.** El `EjemploTesis.pdf` está en el Drive de Santino
> (carpeta `Tesis`), no en el repo, y su texto sólo se puede extraer hasta la
> página 80 aproximadamente por la vía de la nube: justo antes de las secciones
> que hay que copiar. **Subir `EjemploTesis.pdf` al repo** (raíz o `docs/`) es el
> primer paso de la Fase 2; con el archivo local se leen las páginas 152 a 229 sin
> problema.

---

## 3. Dos pasos, no uno: generar y después editar

El trabajo sobre el documento tiene **dos etapas separadas, con dos comandos y dos
commits distintos**:

```
paso 1   generar      bd/CreacionDb.sql + Tesis/   ──►   docs/*.md  y  docs/diagramas/*.png
paso 2   editar       docs/*.md + docs/diagramas/  ──►   Proyecto_v6.docx
```

**El paso 2 no genera nada.** Toma los archivos que ya están en `docs/` y los
vuelca al documento. Si un diagrama está mal, se arregla en el paso 1 o a mano
sobre el `.drawio`, se vuelve a exportar, y el paso 2 se limita a colocar el
archivo que encuentre.

Por qué separados, y no como está hoy:

- **Los diagramas se pueden mirar antes de publicarlos.** Un error del generador
  hoy entra derecho al `.docx`; con la separación se ve primero.
- **Los diagramas se pueden retocar a mano y el retoque sobrevive.** Un MER de
  veinticuatro tablas casi nunca queda legible con el acomodo automático: se abre
  el `.drawio`, se mueven las cajas y se vuelve a exportar. El paso 2 usa lo que
  haya, no lo vuelve a calcular.
- **Corregir un párrafo no obliga a regenerar cuarenta y nueve diagramas de
  secuencia.**

**El precio de separar, y cómo se paga.** Al desacoplar aparece la posibilidad de
armar el documento con diagramas viejos. El paso 2 tiene que **avisar**: antes de
escribir, comparar la fecha de cada artefacto de `docs/` contra la de su fuente
—`bd/CreacionDb.sql`, `Tesis/`— y cortar con un mensaje claro si el artefacto es
más viejo. No es opcional: es lo único que reemplaza a la garantía que daba el
acoplamiento.

**Lo que no cambia.** Nada se escribe dos veces: si un dato está en el código, en
el SQL o en un `.md` de `docs/`, la sección lo lee de ahí, no se transcribe. Y
nunca se edita a mano en Word una sección que el paso 2 escribe: se pierde en la
próxima corrida. Si hay que cambiarla, se cambia el `.md`.

**Refactor pendiente.** Hoy `docs/editar_proyecto.py` hace las dos cosas: importa
`modelo_datos`, `diccionario_clases` y `render_casos_de_uso` y vuelve a derivar el
contenido mientras escribe el documento. Los tres módulos ya saben escribir su
`.md` —`modelo-datos-v6.md`, `diccionario-clases-v6.md`, `casos-de-uso-v6.md`—,
así que la separación es corta: un `docs/generar.py` que los corra a todos junto
con los de `docs/diagramas/`, y un `editar_proyecto.py` que lea de disco en lugar
de importar. Hacerlo **al empezar la Fase 2**, antes de escribir las secciones
nuevas, para que nazcan con la forma correcta.

Las secciones nuevas siguen el mismo camino: 2.3 a 2.9 se escriben como markdown
en `docs/`, y el paso 2 las vuelca. No se escriben directamente en Word.

---

## 4. Estado de partida

`Proyecto_v6.docx`, contenido por sección:

| Sección | Estado |
|---|---|
| 2.1 Análisis | escrita |
| 2.2.1 Diagramas de casos de uso | generada |
| 2.2.2 Casos de uso (49) | generada |
| 2.2.3 Diagrama de Dominio | generada |
| 2.2.4 Diagrama de Persistencia | generada |
| 2.2.5 Modelo de Datos (MER, normalización, claves, integridad) | generada |
| 2.2.6 Diagramas de Secuencia (49) | generada |
| 2.2.7 Diccionario de Clases | generada |
| **2.3 – 2.9** | **«Pendiente.»** |

Insumos que ya existen y hay que aprovechar, no rehacer:

- `docs/flujos-de-prueba.md` — guion manual completo del sistema, con datos
  concretos apoyados en `bd/DatosPrueba.sql`. Es el esqueleto de **2.3**.
- `docs/estilos-y-accesibilidad.md` — decisiones de interfaz con sus contrastes
  medidos. Alimenta **2.4** y la parte de accesibilidad.
- `docs/catalogo-casos-de-uso.md` — los 49 CU con su módulo y su pantalla. Es el
  índice natural del **manual de usuario**.
- `bd/LEEME.md` — puesta en marcha con XAMPP. Es la base de **2.5**.
- `docs/pendientes-tecnicos.md` — los dos puntos donde el código todavía no
  acompaña al documento. Alimenta **2.9**.

---

## 5. Fase 1 — Terminar el código

**Nada de documentación hasta que esta fase esté cerrada** (salvo lo del punto 9).

El Módulo 7 se construye: el documento final describe un sistema de ocho módulos
terminados, no siete y una promesa. Implica, en este orden:

1. Las dos tablas que hoy están en el MER dibujadas con otro relleno por
   proyectadas: crearlas en `bd/CreacionDb.sql`.
2. El bot de Telegram y el envío de las alertas de RF7.6 (sanidad pendiente,
   partos próximos, tactos pendientes, secados próximos, stock crítico,
   vencimiento de insumos, fin del período de descarte de leche).
3. El resumen diario, **CU49**, que es un proceso programado.
4. Las pantallas del módulo, que hoy no existen: los seis diagramas de secuencia
   del Módulo 7 están armados con los mensajes previstos, no leídos del código.

> **CU49 obliga a tocar la caché `static` de la Controladora.** Está explicado en
> `docs/pendientes-tecnicos.md`: hay diecinueve listas `static` compartidas por
> todas las peticiones del proceso, y un proceso programado que corre solo a hora
> fija va a reemplazarlas mientras la encargada carga un parto. La limitación
> asumida se sostiene mientras el sistema sea de un usuario y sin procesos de
> fondo; el Módulo 7 termina con las dos condiciones. El arreglo no es borrar
> `static` veintiuna veces: hay que llamar a `Refrescar()` en el constructor,
> porque la mayoría de los métodos leen las listas sin refrescar y hoy funcionan
> sólo porque son `static`.

**Cierre de la fase.** Correr el paso 1 —los generadores— y revisar los artefactos:
los diagramas de secuencia del Módulo 7 ahora se leen del código, el MER pierde el
relleno de «proyectada» y el diccionario incorpora las clases nuevas. Ahí termina
la Fase 1. El documento todavía no se toca.

---

## 6. Fase 2 — Cerrar 2.3 a 2.9

Primero el refactor del punto 3. Después, para **cada** sección, siempre el mismo
procedimiento:

1. Leer la sección equivalente en `EjemploTesis.pdf` y anotar su forma.
2. Escribir `docs/<seccion>.md` desde las fuentes que se listan abajo.
3. Revisarla en el `.md`, que es donde se lee cómodo. Commit.
4. **Aparte**, correr el paso 2 y verificar en el `.docx` que quedó donde va y con
   el formato de las secciones vecinas. Commit.

### 2.3 Pruebas — ~17 páginas

Fuente: `docs/flujos-de-prueba.md` y el sistema corriendo con
`bd/DatosPrueba.sql`.

Del mismo tipo que las del ejemplo. El anteproyecto compromete **caja negra y
caja blanca**, y ninguna de las dos exige automatización: la de caja blanca es
recorrer los caminos de la lógica con datos elegidos a propósito. Cubrir al
menos las cinco piezas delicadas: `CalcularCategoria` en los bordes,
`ListarAscendencia`, `BuscarAncestroComun`, `VerificarConsanguinidad` y
`EstimarProduccionLactancia`.

Cada caso de prueba con su número, la precondición, los datos de entrada, el
resultado esperado, el obtenido y el veredicto. Los errores encontrados van
documentados con su corrección, como pide el «Registro y Corrección de Errores»
del anteproyecto.

Antes de escribir: **corregir `docs/pendientes-tecnicos.md`**, que hoy afirma que
el anteproyecto compromete pruebas automatizadas. No las compromete.

### 2.4 Manual de Usuario — ~47 páginas

Es la sección más larga y **la única con una dependencia que no se resuelve sola:
las capturas de pantalla**. Ver el punto 9.

Fuentes: `docs/catalogo-casos-de-uso.md` para el recorrido,
`docs/estilos-y-accesibilidad.md` para lo que significa cada color y cada
señalización de la interfaz.

Organizado por módulo, en el orden en que la encargada usa el sistema, no en el
orden en que se programó. Cada pantalla con su captura, para qué sirve, qué
campos tiene y qué valida. Lo que el sistema calcula solo —la categoría, la
fecha probable de parto, la fecha de secado, el fin del período de descarte—
tiene que quedar explicado ahí, porque es lo que la usuaria no espera.

### 2.5 Deployment — ~1 página

Fuente: `bd/LEEME.md` y `Tesis/appsettings.json`. Los dos scripts de base, la
cadena de conexión, XAMPP con MariaDB, el `dotnet run` y el hosting elegido en el
anteproyecto. Sumar la puesta en marcha del proceso programado del Módulo 7. Una
página: no convertirlo en un manual de sistemas.

### 2.6 Política de Seguridad y Respaldos — ~1 página

El sistema es de un solo usuario con credenciales fijas: **decirlo, no
disimularlo**. Qué se respalda, con qué frecuencia, dónde queda la copia y quién
la hace. Los respaldos automáticos del hosting ya están comprometidos en el
control preventivo del riesgo R9 del anteproyecto: usar eso, no inventar otra
cosa. El token del bot de Telegram es un secreto: decir dónde vive y quién lo rota.

### 2.7 Plan de contingencia — ~1 página

Qué hacer si se cae el hosting (R9), si Telegram deja de responder (R6) o si se
pierden datos. Tiene que ser coherente con esos riesgos: el plan de contingencia
es la respuesta a los riesgos que el anteproyecto ya identificó, no una lista
nueva.

### 2.8 Grado de satisfacción del cliente — ~1 página

**No se puede escribir sin la usuaria.** Ver el punto 9.

### 2.9 Conclusiones — ~7 páginas

Qué se propuso el proyecto y qué quedó funcionando, módulo por módulo. Qué
resultó más difícil de lo previsto —el dominio ganadero, las reglas que hubo que
relevar con la usuaria— y qué se aprendió. Qué queda como trabajo futuro.

Lo que quede abierto de `docs/pendientes-tecnicos.md` va acá, dicho de frente.
Un documento que reconoce sus límites se defiende mejor que uno que los esconde.

---

## 7. Fase 3 — Unificar y renumerar

**Decisión tomada: se renumera todo como el ejemplo.**

1. El anteproyecto pasa de títulos sueltos en mayúsculas —`INTRODUCCIÓN`,
   `PRESENTACIÓN DEL CLIENTE`, `LISTA DE NECESIDADES`— a `1.1`, `1.2`, `1.3`,
   siguiendo el mapa del punto 2. Sus subtítulos actuales bajan a `1.x.y`.
2. El proyecto queda como está: ya es `2.x`.
3. Portada, declaración de autoría, abstract, **palabras clave** (el ejemplo las
   tiene, el anteproyecto no), índice único, glosario, bibliografía y anexo.
4. El índice se genera, no se escribe. Con números de página reales.

Hacerlo con un script —`docs/armar_tesis.py`— que produzca `Tesis.docx` a partir
de los dos `.docx`. Es un paso 2, no un paso 1: no deriva nada del código, sólo
concatena y renumera lo que ya está escrito.

**Dos huecos del anteproyecto que aparecen al comparar con el ejemplo:**

- **`1.11 Estimación del esfuerzo`** (7 páginas en el ejemplo) no existe en el
  anteproyecto. Hay que escribirla o justificar su ausencia con el tutor.
- **Anexo**: el ejemplo tiene uno, el anteproyecto no. Decidir qué va.

---

## 8. Cómo se escribe

**El texto del documento** sigue la voz del que ya está: afirmativo, concreto,
sin adjetivos de relleno. Nada de «cabe destacar», «es importante mencionar» ni
«la presente sección tiene como objetivo». Los números van con su fuente: no
«numerosas tablas» sino «las veintidós tablas». Si algo no se hizo, se dice que
no se hizo.

**Los commits** siguen la costumbre del repo: el título dice el hallazgo, no la
tarea —«El modelo entidad-relacion se lee del esquema, no se transcribe», no
«Actualizar MER»—, sin tildes, y el cuerpo explica por qué se hizo así y qué
apareció en el camino.

**El trabajo va en la rama `claude/thesis-project-document-prompt-nj7ffh`**, con
commits separados para el contenido (`.md`) y para el documento (`.docx`). No
mezclar los dos pasos en un commit: el diff del `.docx` es ilegible y arrastraría
al `.md` con él.

---

## 9. Lo que se puede adelantar mientras se programa

Estas tres no dependen del código y tienen dependencias externas lentas. Conviene
destrabarlas durante la Fase 1, no esperar a la Fase 2:

1. **Subir `EjemploTesis.pdf` al repo.** Bloquea el criterio de forma de 2.3 a
   2.9. Es el primer paso y no cuesta nada.
2. **La opinión de la usuaria, para 2.8.** Hay que mostrarle el sistema y
   preguntarle. Sirve una pauta corta y por escrito —qué usa, qué le resultó
   difícil, qué le ahorró tiempo, qué le falta— y la sección se escribe con sus
   respuestas. Es una página: no necesita una encuesta formal, pero sí necesita
   ser real. Se puede hacer con los siete módulos que ya andan.
3. **`1.11 Estimación del esfuerzo` y el Anexo.** Son del anteproyecto, hablan de
   lo que se planificó al principio: no cambian con el Módulo 7. Conviene
   preguntarle al tutor temprano si los pide.

Y una que **sí** espera al código, pero que hay que preparar antes porque es la
sección más larga:

4. **Las capturas de pantalla del manual de usuario.** El contenedor remoto no
   tiene `dotnet` ni MySQL, así que el sistema no se puede levantar ahí. Hay dos
   caminos: sacarlas a mano con XAMPP y Visual Studio andando, o correr Claude
   Code **en la máquina de ustedes**, que sí puede levantar la app y sacarlas con
   Playwright de forma consistente —mismo tamaño de ventana, mismos datos de
   `DatosPrueba.sql`, mismo recorte—. El segundo camino es bastante mejor: son
   unas cuarenta capturas, más las del Módulo 7, y a mano salen desparejas.

---

## 10. Criterios de aceptación

El documento está terminado cuando:

- [ ] `EjemploTesis.pdf` está en el repo y las secciones 2.3 a 2.9 se escribieron
      después de leer su equivalente.
- [ ] El Módulo 7 está construido y los diagramas de secuencia del módulo se leen
      del código, no de los mensajes previstos.
- [ ] El MER no tiene tablas dibujadas como proyectadas.
- [ ] Ninguna sección dice «Pendiente.».
- [ ] Ningún documento promete pruebas automatizadas.
- [ ] Generar y editar son dos comandos separados, y el de editar corta con un
      mensaje claro si algún artefacto de `docs/` es más viejo que su fuente.
- [ ] Borrar `Proyecto_v6.docx` y `Tesis.docx` y correr el paso 2 los reconstruye
      completos, sin volver a generar nada. Ninguna sección se perdió por estar
      escrita a mano en el Word.
- [ ] El índice de `Tesis.docx` tiene números de página reales y el anteproyecto
      está numerado `1.x`.
- [ ] Está resuelto qué pasa con `1.11 Estimación del esfuerzo` y con el Anexo.
- [ ] El documento se leyó entero de corrido una vez, buscando contradicciones
      entre el anteproyecto (que habla en futuro, de lo que se va a hacer) y el
      proyecto (que habla de lo que se hizo).
