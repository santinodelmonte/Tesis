# Comparación con `EjemploTesis.pdf` — qué tenemos, qué falta, cómo está escrito

Lectura de `Anteproyecto_v8.docx` y `Proyecto_v7.docx` contra el modelo que dio la
cátedra, con el criterio de un tutor que va a tener que defender el documento en la mesa.
Verificado contra los archivos el **14/09/2026**.

El modelo tiene **231 páginas**: anteproyecto hasta la 59, proyecto de la 61 a la 226,
glosario, bibliografía y anexo al final. De esas 231, unas **60 son el manual de usuario**
y casi todas son capturas. Es un dato que conviene tener presente antes de leer la lista
de faltantes: el documento del modelo es, en volumen, mitad imagen.

---

## 1. El mapa: sección por sección

### 1. Anteproyecto

| Sección del modelo | En nuestro anteproyecto | Estado |
|---|---|---|
| Declaración de autoría | Sí | Completo |
| Abstract | Sí | Completo |
| Palabras clave | **No existe** | Falta |
| Índice | Título sin contenido | Falta |
| 1.1 Introducción | Sí | Completo |
| 1.2 Presentación del cliente | Sí, sin las subsecciones del modelo | Completo con reparo (§4.6) |
| 1.3 Presentación del problema | Sí | Completo |
| 1.4 Lista de necesidades | Sí | Completo |
| 1.5 Actores involucrados | Sí, con actor y demás interesados separados | Mejor que el modelo |
| 1.6 Objetivos | General + 10 específicos | Completo (§3.1) |
| 1.7 Requerimientos | 74 RF en 8 módulos + 9 RNF | Muy por encima del modelo |
| 1.8 Descripción del entorno | Sí, con el flujo actual y su diagrama | Completo |
| 1.9 Alcances y limitaciones | Sí, 11 y 11 viñetas | Completo |
| 1.10 Estudio de alternativas | 3 de negocio + 2 técnicas con las cuatro factibilidades | Mejor que el modelo |
| **1.11 Estimación del esfuerzo** | **No existe** | **Falta — bloqueante** |
| 1.12 Análisis de riesgo | Estrategia + 14 riesgos fichados | Mejor que el modelo |
| 1.13.1 Metodología | 4 modelos evaluados | Completo |
| 1.13.2 Ciclo de vida | Sí | Completo |
| 1.13.3 Iteraciones | 6 iteraciones | Completo |
| 1.13.4 Integrantes y roles | Sí | Completo |
| 1.13.5 Herramientas | 7 bloques | Completo |
| 1.13.6 Plan de SQA | Sí + plan de testing con 6 tipos de prueba | Completo |
| 1.13.7 Plan de SCM | Sí + control de versionado | Completo |
| 1.13.8 Plan de capacitación | Equipo y usuarios | Completo |
| 1.13.9 Cronograma y criticidad | Título + 3 imágenes, cero texto | Igual que el modelo (§4.5) |
| 1.14 Compromiso de trabajo | Sí, firmado por las tres partes | Completo |
| Glosario | Sí, 15 términos | Completo |
| Bibliografía | Sí, sin normalizar | Completo con reparo (§4.1) |

### 2. Proyecto

| Sección del modelo | En nuestro proyecto | Estado |
|---|---|---|
| 2.1 Análisis | 5 párrafos | Completo |
| 2.2.1 Diagramas de casos de uso | 8 diagramas, uno por módulo | Completo |
| 2.2.2 Casos de uso | **49 CU** con los 14 campos del modelo | Completo (el modelo tiene 59) |
| 2.2.3 Diagrama de dominio | Sí | Completo |
| 2.2.4 Diagrama de persistencia | Sí | Completo |
| *(el modelo no lo tiene)* | **2.2.5 Modelo de datos**: MER, normalización a 3FN, tabla de claves y restricciones de integridad | Agregado nuestro |
| 2.2.6 Diagramas de secuencia | **49 diagramas**, uno por cada CU | Completo — cobertura total |
| 2.2.7 Diccionario de clases | Sí, negocio + persistencia | Completo con reparo (§4.2) |
| **2.3 Pruebas** | «Pendiente.» en el Word; escrita en `docs/seccion-2-3-pruebas.md` | **Falta en el documento** |
| **2.4 Manual de usuario** | «Pendiente.» en el Word; escrita en `docs/seccion-2-4-manual.md` | **Falta en el documento** |
| **2.5 Deployment** | «Pendiente.» en el Word; escrita en `docs/seccion-2-5-deployment.md` | **Falta en el documento** |
| **2.6 Seguridad y respaldos** | «Pendiente.» en el Word; escrita en `docs/seccion-2-6-seguridad.md` | **Falta en el documento** |
| **2.7 Plan de contingencia** | «Pendiente.» en el Word; escrita en `docs/seccion-2-7-contingencia.md` | **Falta en el documento** |
| **2.8 Grado de satisfacción** | No existe, ni en borrador | **Falta — depende de la clienta** |
| **2.9 Conclusiones** | No existe, ni en borrador | **Falta — bloqueante** |
| Anexo | No existe | Falta |

---

## 2. Lo que falta, ordenado por lo que cuesta

### Bloqueantes de entrega

**1. Las secciones 2.3 a 2.7 no están en el Word.** Están escritas —13.491 palabras de
markdown— pero `editar_proyecto.py` arma el documento hasta la 2.2.7 y deja el resto como
estaba en el v5, es decir, la palabra «Pendiente.». Hoy, el `.docx` que se entregaría no
tiene pruebas, ni manual, ni deployment, ni seguridad, ni contingencia. Es el paso que
destraba todo lo demás y no depende de nadie más que de ustedes.

**2. Las 112 capturas no existen.** `docs/capturas/` está vacío. El manual tiene **79
marcas** `[captura: …]` y las pruebas **34** más. En el modelo, la 2.4 son 47 páginas y
casi todas son una imagen con dos renglones de texto: sin las capturas, nuestro manual es
un texto que describe pantallas que el tribunal nunca ve. El script de Playwright que el
guion describe tampoco existe todavía.

**3. La columna «Resultado» de las pruebas está vacía en los 131 renglones.** El protocolo
está escrito con sus datos y su resultado esperado; lo que falta es correrlo. Hasta que no
se corra, la 2.3 es un plan de pruebas, no un registro de pruebas, y en la mesa esa
diferencia se nota enseguida.

**4. La 2.9 Conclusiones no existe.** Son las siete páginas que más pesan en la defensa,
con los diez subtítulos del modelo: dinámica del equipo, relación con el cliente, relación
con el tutor, **riesgos** (los catorce del anteproyecto, uno por uno, si se dio y cómo se
resolvió), metodología, herramientas, trabajo colaborativo, producto final, puntos a
mejorar y conclusión final. El bloque de riesgos es el largo y es el que el tribunal lee
con más atención, porque es donde se ve si el análisis de riesgo del anteproyecto sirvió
para algo o fue un trámite.

**5. La 1.11 Estimación del esfuerzo no existe.** Es el único apartado del anteproyecto del
modelo que nosotros no tenemos, y el modelo le dedica **seis páginas**: puntos de función
—entradas, salidas y consultas externas, archivos lógicos internos y de interfaz—, el
cuestionario de catorce factores de ajuste, la fórmula `PF = conteo × [0,65 + 0,01 × ΣFi]`
y la conversión a horas. No es opcional: es el apartado que justifica el cronograma, y sin
él el cronograma es un dibujo. La buena noticia es que se puede escribir hoy, porque el
conteo sale del sistema terminado: 49 casos de uso, 24 tablas y el inventario de pantallas
ya están hechos.

**6. El documento único no existe.** `Tesis.docx` —anteproyecto renumerado como `1.x`,
proyecto como `2.x`, portada, declaración, abstract, palabras clave, índice, glosario,
bibliografía y anexo— es la Fase 4 y todavía no se empezó. Hoy hay dos documentos
separados; el modelo es uno solo.

### Faltantes menores pero visibles

**7. El anexo.** El modelo cierra con fotos de las planillas con que el laboratorio
trabajaba antes. El nuestro sería la foto de los cuadernos y el pizarrón del tambo: es la
evidencia de la «Presentación del problema», y sacarla cuesta cinco minutos con un
teléfono. Es de lo más barato que queda por hacer y de lo que más se ve.

**8. Palabras clave.** El modelo tiene una página. Nosotros no la tenemos.

**9. El índice.** El del anteproyecto es un título sin contenido. El del proyecto es peor:
hoy es una tabla de estado interno con una columna que dice «Realizado» y «Pendiente». Eso
es andamiaje de trabajo y **no puede quedar en la entrega** —le está diciendo al tribunal,
en la página 2, qué partes no terminamos—. Va reemplazado por un índice de verdad.

**10. La 2.8 Grado de satisfacción.** Una página, y no la pueden escribir ustedes solos:
hace falta mostrarle el sistema a Sofía y anotar lo que dice. En el modelo es el relato de
la relación a lo largo de las iteraciones, no una encuesta. De esa misma sesión sale la
verificación del RNF de usabilidad, así que conviene hacer las dos cosas juntas.

---

## 3. Calidad del documento: dónde estamos mejor que el modelo

Conviene decirlo antes de la lista de reparos, porque es cierto y porque conviene saber
qué defender.

**3.1 Los requerimientos.** El modelo tiene **17 RF** en una lista plana —con huecos de
numeración: no existen RF8, RF15 ni RF18— y cada uno agrupa un módulo entero («debe proveer
medios para realizar mantenimiento de usuarios»). Nosotros tenemos **74 RF agrupados en 8
módulos, numerados `RFx.y`**, atómicos, y cada caso de uso referencia el suyo. Esa trazabilidad requerimiento → caso de uso → diagrama de secuencia es lo que un
tribunal busca y el modelo no la tiene. Además, los objetivos específicos ya no traen los
porcentajes inventados que tenían las versiones anteriores: hoy dicen qué se logra, no un
número sin línea base. Eso estaba bien marcado y quedó bien resuelto.

**3.2 El modelo de datos.** La 2.2.5 —MER, normalización, tabla de claves y restricciones
de integridad, con 24 tablas detalladas campo por campo con tipo, restricción y
observación— **no existe en el modelo**. Es el agregado más sólido del documento.

**3.3 Los casos de uso y las secuencias.** El modelo tiene 59 casos de uso y 49 diagramas
de secuencia: **diez casos de uso se quedaron sin diagrama**. Nosotros tenemos 49 y 49, uno
por uno, con el mismo formato de catorce campos. La ventaja no es la cantidad —ahí el
modelo tiene más— sino que la cobertura es completa y se puede afirmar en el texto. Y las
secuencias son honestas: el texto explica el patrón común entre capas y nombra los tres
casos que se apartan de él, en vez de fingir que todos son iguales.

**3.4 El estudio de alternativas y los riesgos.** Tres alternativas de negocio más dos
técnicas con las cuatro factibilidades cada una, y catorce riesgos fichados con
probabilidad, impacto y controles. El modelo hace menos en las dos secciones.

**3.5 La prosa.** El anteproyecto está escrito en párrafos que argumentan, no en viñetas
sueltas. Se lee mejor que el modelo.

---

## 4. Calidad del documento: los reparos

### 4.1 La bibliografía no cita un solo libro — grave

Nuestra bibliografía son seis sitios oficiales de herramientas, sin autor, sin URL, sin
edición y sin norma bibliográfica, más un renglón que remite a «materiales de Moodle».

El problema no es la forma: es que **el anteproyecto usa teoría que no cita**. La
clasificación de riesgos en catastrófico, crítico, marginal y despreciable; los cuatro
modelos de ciclo de vida; el plan de SQA; los tipos de prueba de caja negra: todo eso sale
de Pressman, y Pressman no figura. El modelo cita Pressman y Elmasri-Navathe con edición y
editorial, al lado de los sitios de herramientas.

Si se agrega la 1.11, el problema se agrava: los puntos de función son de Albrecht y llegan
por Pressman. Un tribunal que pregunte «¿de dónde sacaron esta tabla de ponderación?» tiene
que encontrar la respuesta en la bibliografía.

**Se arregla en una tarde**: agregar Pressman (*Ingeniería de software, un enfoque
práctico*), un libro de bases de datos, y normalizar los seis sitios con autor, título,
URL y fecha de acceso.

### 4.2 El documento se contradice con el código — grave

La 2.2.7 dice que la Controladora «valida contra sus listas **static** en memoria»
(`docs/editar_proyecto.py:451`). En el código, esas listas son **campos de instancia**
desde que se sacó la caché `static` (`Tesis/Dominio/Controladora.cs:122`). Es un renglón,
pero es exactamente el renglón que un tribunal verifica al abrir el código, y además
describe una decisión de diseño que ustedes tomaron a propósito y que conviene poder
contar. Hay que corregir el generador y regenerar.

### 4.3 Los RNF no están numerados — medio

Los 74 RF están numerados y trazados. Los 9 RNF son viñetas con nombre —Usabilidad,
Accesibilidad, Compatibilidad…— sin identificador. **El modelo sí los numera**, RNF1 a RNF7,
así que acá estamos por debajo de la pauta de la cátedra en lo único que es gratis. No se puede escribir «este caso de uso
cumple RNF3» ni «la prueba T-12 verifica RNF5», y en la 2.9 no se va a poder decir si se
cumplieron uno por uno. Numerarlos `RNF1` a `RNF9` cuesta diez minutos y mejora la
trazabilidad del documento entero.

Vale la pena decir lo bueno: el contenido de los RNF ya es verificable —«que la encargada
complete sin asistencia las cinco tareas de uso diario después de una única
capacitación», «WCAG 2.1 AA», «verificado desde 375 píxeles»—. Lo único que falta es el
número que los haga citables.

### 4.4 Problemas de formato que se ven en el índice automático — medio

Son de máquina, no de contenido, pero todos se ven en el PDF final:

- El **primer párrafo del anteproyecto** es la palabra «Indice» con estilo Título, suelta,
  antes de la portada.
- La línea de firmas «Santino Delmonte — Sofía Vila — Alejo De León» está marcada como
  **Heading 2**: va a aparecer en el índice automático como si fuera una sección del
  documento, entre «Compromiso de trabajo» y «Glosario».
- En «Alcance y limitaciones», *Limitaciones* es Heading 4 y el bloque de alcances no tiene
  subtítulo: el índice va a mostrar una limitación sin su alcance.
- Todas las viñetas son el carácter `●` **pegado a mano con un espacio invisible**, heredado
  de Google Docs, en lugar de listas de Word. Se ve raro al convertir a PDF y rompe la
  sangría.

### 4.5 El cronograma son tres imágenes sin una línea de texto — menor

El título «Cronograma de trabajo y criticidad» va seguido de tres imágenes y nada más. La
palabra «criticidad» aparece en el título y en ningún otro lado.

**No lo marco como riesgo de nota**, porque el modelo hace exactamente lo mismo: su 1.13.9
es el título y dos páginas de imagen. Pero un párrafo que diga cuáles son las tareas del
camino crítico y qué pasa si se atrasan cuesta diez renglones y es de las pocas cosas donde
se puede estar por encima del modelo sin trabajo.

### 4.6 La presentación del cliente no tiene organigrama — menor

El modelo dedica cinco subsecciones al cliente, con estructura organizacional y misión.
El nuestro son párrafos corridos. En un tambo familiar con una encargada, cuatro
subsecciones serían relleno; pero **un organigrama chico** —dueño, encargada, tamberos,
veterinario externo— cierra la sección de actores y le da al tribunal en un golpe de vista
por qué el sistema tiene un solo usuario. Vale la pena.

---

## 5. Dónde estamos, en una línea

**El diseño está terminado y está por encima del modelo. El documento está a mitad de
camino, y lo que falta es casi todo ejecución sobre el sistema andando.**

De las 231 páginas del modelo, tenemos escrito el equivalente a las 160 primeras y las
mejores. Lo que falta se reparte en tres pilas de tamaño muy distinto:

| Pila | Qué es | Depende de |
|---|---|---|
| **Escribir** | 1.11 Estimación del esfuerzo, 2.9 Conclusiones, bibliografía, palabras clave, numerar RNF, corregir formato | Nadie más. Se puede hacer ya |
| **Ensamblar** | Meter 2.3 a 2.7 en el Word, armar `Tesis.docx`, el índice de verdad | Nadie más. Es el primer paso |
| **Ejecutar** | Las 112 capturas, la columna «Resultado», la 2.8 con Sofía, las fotos del anexo | Levantar el sistema y una visita al tambo |

La tercera pila es la única que no se puede adelantar, y es la que más páginas produce.
Conviene resolverla en una sola jornada: sistema levantado con `DatosPrueba.sql` recién
cargado, script de capturas, protocolo de pruebas y la reunión con Sofía, todo el mismo
día.

**El orden que recomiendo**: ensamblar 2.3–2.7 en el Word (sin eso nada de lo escrito
existe para el tribunal) → el script de capturas → la jornada de ejecución → 1.11 y 2.9 →
`Tesis.docx`. Los reparos de calidad de §4 entran en cualquier hueco; ninguno lleva más de
una tarde y todos se ven.
