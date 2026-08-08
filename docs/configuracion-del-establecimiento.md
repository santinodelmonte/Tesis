# Configuración del establecimiento

Las reglas de manejo dejan de estar fijas en el código y pasan a ser un dato que la
encargada configura. No es un requerimiento del documento de Proyecto: hay que
incorporarlo, y al final está lo que eso implica.

---

## 1. Por qué: no todas las constantes son iguales

El sistema tenía una docena de números fijos en `Controladora`. Revisándolos uno por
uno, no son la misma clase de cosa:

**Decisiones del establecimiento.** El secado es el caso más claro: lo habitual son 60
días antes del parto, pero hay tambos que trabajan con 45–50 y otros que aplican
período seco corto de 30–40. Depende del nivel de producción, de la carga de la vaca y
del asesor con el que trabajan. Lo mismo la edad a la que entra la vaquillona en
servicio, que va de 12 a 15 meses según el sistema de crianza, o cuántas veces por día
se ordeña.

**Preferencias operativas.** Con cuánta anticipación quiere el usuario que le avisen de
un secado, de un parto, de un procedimiento sanitario o del vencimiento de una partida.
No hay un valor correcto: hay uno cómodo.

**Biología.** La duración de la gestación, la edad a la que la hembra empieza a ciclar y
el rango en que una preñez termina en un parto viable. Varían un poco por raza —la
Jersey gesta unos 279 días contra los 283 de la Holando— pero no son una decisión de
nadie. Siguen en el código.

**Datos del producto.** El período de carencia y el stock mínimo ya viven en `insumos`,
que es donde corresponde: son del producto, no del establecimiento.

Se hicieron configurables los dos primeros grupos.

---

## 2. Qué quedó configurable

| Parámetro | Por defecto | Dónde pega |
|---|---|---|
| Días de secado antes del parto | 60 | Fecha recomendada de secado (RF2.10, CU12, CU13) |
| Edad mínima al servicio | 13 meses | Validación de CU15 y piso de `ValidarGenealogia` |
| Edad de cambio de categoría | 12 meses | `CalcularCategoria` (RF1.9) |
| Litros máximos por control individual | 100 | `ValidarLitrosIndividual` y, multiplicado por la cantidad de animales, `ValidarLitrosLote` (RF2.3) |
| Ordeñes por día | 2 | Turnos que ofrecen CU8 y CU9 |
| Anticipación del secado | 15 días | CU13 |
| Anticipación del parto | 15 días | CU17 |
| Anticipación sanitaria | 30 días | CU23 |
| Anticipación de vencimiento | 30 días | CU28 |

Las constantes siguen existiendo en `Controladora` como **valores por defecto**: si la
tabla de configuración está vacía, el sistema se comporta igual que antes. Ninguna
instalación queda trabada por falta de configuración.

---

## 3. Cómo se guarda

Tabla `configuracion` de **una sola fila**, con una columna tipada por parámetro. El
sistema lee siempre la primera fila y escribe sobre ella: no hay alta de
configuraciones, hay una configuración del establecimiento que se modifica.

Se eligió así, y no una tabla clave–valor, porque cada parámetro queda documentado en
2.2.5.4 con su tipo y su restricción, igual que el resto del esquema. El costo es que
agregar un parámetro nuevo requiere un `ALTER TABLE`; con nueve parámetros estables, es
barato.

El script es `bd/tambo_configuracion.sql` e incluye la fila inicial con los valores por
defecto.

---

## 4. Dos decisiones de comportamiento

**Cambiar un parámetro afecta los cálculos de ahí en adelante, no lo ya registrado.**
La fecha recomendada de secado se deriva en cada consulta, así que pasar de 60 a 45
días mueve las alertas de todo el rodeo al instante: eso es lo que se busca. En cambio
la fecha probable de parto se guarda en `servicios` y en `lactancias` cuando se
registra, así que las preñeces en curso conservan la que tenían. Es coherente con lo
que ya decía 2.2.5.2 sobre los valores que el usuario puede ajustar: una vez
almacenados dejan de ser derivables.

**Bajar la cantidad de ordeñes no borra lo registrado.** Si el tambo pasa de tres
ordeñes a dos, los controles que ya se cargaron en "Turno 3" siguen existiendo y se
siguen sumando a la producción. Lo que cambia es lo que las pantallas ofrecen de ahí en
adelante: `EsTurnoValido` valida lo que se está por guardar, no lo guardado.

---

## 5. Lo que cambió en el código

- `Dominio/Configuracion.cs`, `Persistencia/pConfiguracion.cs` y la pantalla
  **Configuración**.
- `Controladora.Parametros()` es el punto único por donde el dominio consulta los
  valores; devuelve los por defecto cuando no hay fila. `ObtenerConfiguracion`,
  `ValidarConfiguracion` y `ModificarConfiguracion` son la interfaz pública.
- `OrdenieLote.TURNO_1` y `TURNO_2` desaparecen. Los turnos se arman con
  `OrdenieLote.NombreTurno(numero)` y la lista sale de `Controladora.ListarTurnos()`,
  que la deriva de los ordeñes por día. `EsTurnoValido` reemplaza a las dos
  comparaciones contra constantes.
- Las pantallas de alertas ya no toman su ventana de una constante sino de la
  configuración.

---

## 6. Qué hay que agregar al Proyecto

1. **Requerimiento funcional nuevo**: el sistema debe permitir configurar los
   parámetros de manejo del establecimiento. Va en el Módulo 0 o como módulo
   transversal.
2. **Caso de uso nuevo**: "Configurar Parámetros del Establecimiento", con el rango
   admitido de cada parámetro como validación.
3. **Modelo de datos**: `configuracion` en el MER (2.2.5.1), en la normalización
   (2.2.5.2), en la tabla de claves (2.2.5.3) y en las restricciones de integridad
   (2.2.5.4), aclarando que es una relación de instancia única.
4. **Diccionario de Clases**: la clase `Configuracion` y los métodos
   `ObtenerConfiguracion`, `ValidarConfiguracion`, `ModificarConfiguracion`,
   `ListarTurnos` y `EsTurnoValido`.
5. **Reglas de negocio**: donde los casos de uso hablan de "60 días", "15 meses" o "dos
   turnos", pasar a decir que el valor es el configurado, con el por defecto entre
   paréntesis. Afecta a CU8, CU9, CU12, CU13, CU15, CU17, CU23 y CU28.
6. **Ordeñes por día**: CU8 y CU9 hoy suponen dos turnos. Reescribir el paso donde se
   elige el turno.

---

## 7. Limitaciones asumidas

- La configuración es **una sola para todo el establecimiento**. En un tambo con dos
  rodeos manejados distinto —por ejemplo, vaquillonas de primera lactancia con un
  período seco más largo— haría falta configurar por categoría o por grupo. No está.
- La **edad mínima de servicio del macho** (15 meses) no se configuró: se dejó fija
  porque es madurez reproductiva del toro más que una decisión de manejo. Queda
  asimétrica con la de la hembra, que sí es configurable.
- Los **rangos admitidos** de cada parámetro están en `ValidarConfiguracion` y no en la
  base. Un `UPDATE` a mano sobre la tabla puede dejar valores fuera de rango.
