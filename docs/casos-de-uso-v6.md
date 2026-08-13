# 2.2.2 Casos de uso — v6

Generado desde `casos_de_uso_parte1.py` y `casos_de_uso_parte2.py`. No editar a mano: editar los datos y volver a generar.

## Listado


**Modulo 0: Seguridad, Acceso y Configuracion**

- CU 1 — Iniciar Sesión (Log In)
- CU 2 — Cerrar Sesión
- CU 3 — Configurar Parámetros del Establecimiento

**Modulo 1: Gestion de Animales y Genetica**

- CU 4 — Registrar Alta de Animal
- CU 5 — Modificar Datos de Animal
- CU 6 — Registrar Baja de Animal
- CU 7 — Reactivar Animal
- CU 8 — Consultar Linaje y Registro Genealógico
- CU 9 — Verificar Consanguinidad
- CU 10 — Buscar y Filtrar Animales del Rodeo
- CU 11 — Consultar Ficha Integral del Animal

**Modulo 2: Control de Produccion**

- CU 12 — Registrar Ordeñe por Lote
- CU 13 — Registrar Control Lechero
- CU 14 — Consultar Historial de Producción y Lactancias
- CU 15 — Consultar Métrica de Producción Mensual
- CU 16 — Registrar Período de Secado Manual
- CU 17 — Consultar Alertas de Secado Próximo
- CU 18 — Abrir Lactancia Manualmente
- CU 19 — Corregir o Eliminar Registro de Producción

**Modulo 3: Gestion Reproductiva**

- CU 20 — Registrar Detección de Celo
- CU 21 — Registrar Servicio
- CU 22 — Registrar Tacto y Confirmación de Preñez
- CU 23 — Consultar Alertas de Parto Próximo
- CU 24 — Registrar Parto
- CU 25 — Consultar Listas de Trabajo Reproductivas
- CU 26 — Corregir o Eliminar Evento Reproductivo

**Modulo 4: Gestion Sanitaria**

- CU 27 — Registrar Diagnóstico o Revisación
- CU 28 — Registrar Tratamiento Sanitario
- CU 29 — Registrar Vacunación
- CU 30 — Configurar Plan Sanitario
- CU 31 — Consultar Calendario Sanitario
- CU 32 — Registrar Procedimiento de Descorne
- CU 33 — Cerrar Diagnóstico
- CU 34 — Corregir o Eliminar Evento Sanitario

**Modulo 5: Control de Insumos y Stock**

- CU 35 — Registrar Alta e Ingreso de Insumo
- CU 36 — Configurar Umbral de Stock Mínimo
- CU 37 — Consultar Alertas de Stock Crítico
- CU 38 — Consultar Alertas de Vencimiento de Insumos
- CU 39 — Consultar Historial de Movimientos de Stock

**Modulo 6: Tablero, Indicadores y Apoyo a la Decision**

- CU 40 — Consultar Tablero de Inicio
- CU 41 — Consultar Indicadores del Rodeo
- CU 42 — Consultar Candidatas a Descarte
- CU 43 — Buscar Animal por Caravana

**Modulo 7: Reportes y Notificaciones**

- CU 44 — Generar Reporte Productivo
- CU 45 — Generar Reporte Sanitario
- CU 46 — Generar Reporte Reproductivo
- CU 47 — Generar Reporte Genético
- CU 48 — Configurar Integración con Bot de Telegram
- CU 49 — Enviar Resumen Diario de Tareas Pendientes

---


## Modulo 0: Seguridad, Acceso y Configuracion

### CU 1 — Iniciar Sesión (Log In)

Nombre del CU: Iniciar Sesión (Log In)

Actores: Encargada del sector

Tipo: Primario

Descripción: El usuario ingresa las credenciales fijas para acceder al sistema con control total.

Referencia a Requerimientos Funcionales: RF0.1

Pre-condición: El sistema debe estar operativo en la pantalla de acceso.

Desencadenante: El usuario se dispone a ingresar al sistema.

Curso Básico:

1. El usuario ingresa a la pantalla de Log In.

2. El sistema despliega la interfaz para ingresar usuario y contraseña.

3. El usuario introduce las credenciales y presiona “Ingresar”.

4. El sistema valida las credenciales y abre la sesión, que se mantiene mediante una cookie de autenticación.

5. El sistema redirige al tablero de inicio.

Cursos Alternativos: 1a. El usuario intenta acceder directamente a una dirección del sistema sin haber iniciado sesión: el sistema lo redirige a la pantalla de Log In y, una vez autenticado, continúa hacia la dirección solicitada.

Cursos de Excepción: 4a. Credenciales incorrectas: el sistema muestra un mensaje de error y deniega el acceso.

Post-condición: El usuario queda logueado en el sistema con acceso a todos los módulos.

Reglas de Negocio: El acceso se realiza mediante un único par de credenciales fijas. Todas las páginas del sistema exigen sesión iniciada, con la única excepción de la propia pantalla de acceso y la de error. Las credenciales no residen en el código fuente: se leen de la configuración de la aplicación.

Validaciones: El ingreso de los campos “Usuario” y “Contraseña” es obligatorio.

Frecuencia de Uso: Alta, al inicio de cada jornada de trabajo.

### CU 2 — Cerrar Sesión

Nombre del CU: Cerrar Sesión

Actores: Encargada del sector

Tipo: Primario

Descripción: El usuario finaliza su sesión de trabajo, de modo que el sistema deje de ser accesible desde ese navegador sin volver a autenticarse.

Referencia a Requerimientos Funcionales: RF0.2

Pre-condición: El usuario debe estar logueado en el sistema.

Desencadenante: El usuario termina su jornada o deja el equipo disponible para otra persona.

Curso Básico:

1. El usuario presiona “Cerrar sesión” en la barra superior.

2. El sistema cierra la sesión y descarta la cookie de autenticación.

3. El sistema redirige a la pantalla de Log In.

Cursos Alternativos: —

Cursos de Excepción: —

Post-condición: La sesión queda cerrada. Cualquier intento posterior de acceder a una pantalla del sistema deriva en el Log In.

Reglas de Negocio: La opción de cerrar sesión está disponible desde cualquier pantalla del sistema.

Validaciones: —

Frecuencia de Uso: Alta, al finalizar cada jornada de trabajo.

### CU 3 — Configurar Parámetros del Establecimiento

Nombre del CU: Configurar Parámetros del Establecimiento

Actores: Encargada del sector

Tipo: Primario

Descripción: El usuario ajusta los parámetros de manejo con los que el sistema calcula fechas recomendadas, valida cargas y arma los avisos, de modo que las reglas respondan al criterio del establecimiento y no a constantes fijas del sistema.

Referencia a Requerimientos Funcionales: RF0.3

Pre-condición: El usuario debe estar logueado en el sistema.

Desencadenante: El establecimiento adopta un criterio de manejo distinto del que el sistema tiene configurado.

Curso Básico:

1. El usuario ingresa a la sección “Configuración”.

2. El sistema despliega el formulario con los valores vigentes de cada parámetro: días de secado previos al parto, edad mínima al servicio, edad de cambio de categoría, litros máximos por control individual, cantidad de ordeñes diarios, y los días de anticipación de los avisos de secado, de parto, del calendario sanitario y de vencimiento de insumos.

3. El usuario modifica los valores que correspondan y presiona “Guardar”.

4. El sistema valida que cada valor se encuentre dentro de su rango admitido.

5. El sistema almacena la configuración y confirma la operación.

Cursos Alternativos: 2a. El establecimiento nunca configuró los parámetros: el sistema muestra los valores por defecto y opera con ellos hasta que se guarde una configuración propia.

Cursos de Excepción: 4a. Algún valor queda fuera del rango admitido: el sistema informa cuál es y no guarda ningún cambio.

Post-condición: Los nuevos valores rigen de inmediato para todos los cálculos, validaciones y avisos del sistema.

Reglas de Negocio: La configuración es única para todo el establecimiento. Los parámetros no alteran los registros ya guardados: cambian la forma en que el sistema calcula de ahí en adelante. Reducir la cantidad de ordeñes diarios no borra los registros de turnos que dejan de ofrecerse.

Validaciones: Todos los parámetros son obligatorios y numéricos. Cada uno tiene un rango admitido, informado en el propio formulario.

Frecuencia de Uso: Baja, al poner el sistema en marcha y ante cambios de criterio de manejo.


## Modulo 1: Gestion de Animales y Genetica

### CU 4 — Registrar Alta de Animal

Nombre del CU: Registrar Alta de Animal

Actores: Encargada del sector

Tipo: Primario

Descripción: Permite ingresar un nuevo animal al sistema con sus datos básicos, su fotografía y su registro genealógico.

Referencia a Requerimientos Funcionales: RF1.1, RF1.4, RF1.5, RF1.8, RF1.12, RF1.14

Pre-condición: El usuario debe estar logueado en el sistema.

Desencadenante: El usuario ingresa a la sección de alta de animales.

Curso Básico:

1. El usuario ingresa a la pantalla de Alta de Animal.

2. El sistema despliega el formulario solicitando: número de caravana, fecha de nacimiento, sexo, raza, categoría, fotografía, padre y madre.

3. El sistema calcula la categoría que corresponde al animal y la propone en el formulario, permitiendo que el usuario la ajuste manualmente si el caso lo requiere.

4. El usuario completa la información y presiona “Guardar”.

5. (Incluye: Validar Unicidad de Caravana).

6. El sistema valida que la genealogía indicada sea posible.

7. El sistema almacena los datos y confirma el registro exitoso.

Cursos Alternativos: 2a. No se conocen el padre y la madre del animal, como ocurre en la carga inicial del rodeo: el usuario deja los progenitores sin indicar y el sistema registra al animal igual. 2b. El usuario no carga fotografía: el sistema muestra una silueta genérica en la ficha y en el árbol genealógico. 6a. La genealogía es posible pero inusual —un progenitor dado de baja—: el sistema advierte y permite continuar.

Cursos de Excepción: 5a. El número de caravana ya existe en el sistema: el sistema muestra una alerta de duplicado e impide el guardado. 6a. La genealogía indicada es imposible: el sistema informa el motivo e impide el guardado.

Post-condición: El animal queda registrado correctamente en la base de datos del establecimiento.

Reglas de Negocio: El registro del padre y de la madre es opcional: un animal puede no tener progenitores conocidos, y de hecho así ingresan los animales del rodeo inicial. La categoría se calcula automáticamente a partir del sexo, la edad y la cantidad de partos del animal; el valor calculado se propone por defecto y puede ser ajustado por el usuario ante situaciones puntuales que así lo requieran. La fecha de nacimiento del progenitor debe admitir la edad mínima al servicio respecto de la cría.

Validaciones: El número de caravana es obligatorio y único. La raza es obligatoria. La fecha de nacimiento no puede ser futura. La fotografía, cuando se carga, debe ser un archivo de imagen.

Frecuencia de Uso: Media, cada vez que nace un animal o ingresa una nueva adquisición al tambo.

### CU 5 — Modificar Datos de Animal

Nombre del CU: Modificar Datos de Animal

Actores: Encargada del sector

Tipo: Primario

Descripción: Permite actualizar la información de un animal previamente registrado en el sistema, incluida su fotografía y su genealogía.

Referencia a Requerimientos Funcionales: RF1.3, RF1.9, RF1.12, RF1.14

Pre-condición: El usuario debe estar logueado en el sistema y el animal a modificar debe existir.

Desencadenante: El usuario selecciona la opción de editar un animal dentro del sistema.

Curso Básico:

1. El usuario busca y selecciona el animal que desea modificar.

2. El sistema despliega la interfaz con los datos actuales del animal cargados en un formulario.

3. El usuario edita los campos correspondientes y presiona “Guardar Cambios”.

4. El sistema valida que la genealogía resultante sea posible.

5. El sistema actualiza el registro en la base de datos y confirma el éxito de la operación.

Cursos Alternativos: 3a. El usuario reemplaza la fotografía: el sistema guarda la nueva y descarta la anterior. 3b. La categoría almacenada no coincide con la que corresponde a la edad y a la cantidad de partos del animal: el sistema lo señala y ofrece actualizarla; el usuario acepta la categoría recalculada o conserva la que había.

Cursos de Excepción: 4a. La genealogía resultante es imposible: el sistema informa el motivo e impide el guardado.

Post-condición: Los datos del animal quedan actualizados correctamente en el sistema.

Reglas de Negocio: Un animal no puede ser progenitor de sí mismo ni tener como progenitor a un animal que figure en su propia descendencia. La categoría recalculada nunca se aplica sin intervención del usuario, salvo al registrarse un parto (CU24).

Validaciones: Se mantienen las validaciones de unicidad de caravana y de coherencia genealógica del alta.

Frecuencia de Uso: Media, cuando ocurren correcciones de datos o actualizaciones puntuales.

### CU 6 — Registrar Baja de Animal

Nombre del CU: Registrar Baja de Animal

Actores: Encargada del sector

Tipo: Primario

Descripción: Permite registrar la baja lógica de un animal del rodeo activo, especificando la fecha y la causa de su salida.

Referencia a Requerimientos Funcionales: RF1.2

Pre-condición: El usuario debe estar logueado en el sistema y el animal debe figurar en estado activo.

Desencadenante: El usuario selecciona la opción de dar de baja a un animal.

Curso Básico:

1. El usuario busca y selecciona el animal al que desea dar de baja.

2. El sistema despliega el formulario solicitando la fecha y el motivo de la salida.

3. El usuario indica la fecha, selecciona el motivo (venta, fallecimiento, descarte sanitario u otros) y presiona “Confirmar Baja”.

4. El sistema actualiza el estado del animal a inactivo, almacena la fecha y el motivo, y confirma la operación.

Cursos Alternativos: —

Cursos de Excepción: —

Post-condición: El animal cambia su estado a inactivo y deja de formar parte del rodeo actual: el sistema no lo ofrece en los lotes de ordeñe, en las listas de trabajo ni en el calendario sanitario. Su registro histórico y su lugar en el linaje se conservan íntegros.

Reglas de Negocio: Las bajas son siempre lógicas: no existe borrado definitivo de animales, porque destruiría el historial productivo y el linaje genealógico de los animales emparentados. La baja no impide registrar eventos anteriores a su fecha, pero sí posteriores.

Validaciones: La fecha y el motivo de salida son obligatorios. La fecha no puede ser futura ni anterior a la fecha de nacimiento del animal.

Frecuencia de Uso: Baja, sólo ante la pérdida, descarte o comercialización de un vientre o cría.

### CU 7 — Reactivar Animal

Nombre del CU: Reactivar Animal

Actores: Encargada del sector

Tipo: Primario

Descripción: Permite revertir la baja de un animal, devolviéndolo al rodeo activo. Resuelve el caso de la baja registrada sobre la caravana equivocada.

Referencia a Requerimientos Funcionales: RF1.11

Pre-condición: El usuario debe estar logueado y el animal debe figurar dado de baja.

Desencadenante: El usuario advierte que la baja fue un error o que el animal regresó al establecimiento.

Curso Básico:

1. El usuario ingresa a la ficha del animal.

2. El sistema informa que el animal está dado de baja, desde cuándo y por qué motivo, y ofrece la opción de reactivarlo.

3. El usuario presiona “Reactivar” y confirma.

4. El sistema devuelve el animal al estado activo, limpia la fecha y el motivo de salida, y confirma la operación.

Cursos Alternativos: —

Cursos de Excepción: —

Post-condición: El animal vuelve a integrar el rodeo activo y el sistema lo considera nuevamente en los lotes de ordeñe, las listas de trabajo y el calendario sanitario.

Reglas de Negocio: La reactivación no reconstruye los eventos que el animal no protagonizó mientras estuvo dado de baja: sólo lo devuelve al rodeo.

Validaciones: —

Frecuencia de Uso: Baja, ante una corrección o el reingreso de un animal.

### CU 8 — Consultar Linaje y Registro Genealógico

Nombre del CU: Consultar Linaje y Registro Genealógico

Actores: Encargada del sector

Tipo: Primario

Descripción: Permite recorrer el árbol genealógico de un animal, desplegando cada rama de la ascendencia y accediendo a la ficha de cualquier ancestro.

Referencia a Requerimientos Funcionales: RF1.6, RF1.13

Pre-condición: El usuario debe estar logueado en el sistema.

Desencadenante: El usuario solicita ver la genealogía de un animal específico.

Curso Básico:

1. El usuario busca y selecciona el animal dentro del rodeo.

2. El usuario selecciona la opción “Ver Linaje”.

3. El sistema recupera los datos de los progenitores vinculados.

4. El sistema despliega el árbol genealógico mostrando al animal y a sus progenitores directos, cada uno con su fotografía y su caravana.

5. El usuario despliega la rama de cualquier ancestro y el sistema incorpora al árbol la generación siguiente.

Cursos Alternativos: 5a. El usuario selecciona un ancestro: el sistema abre la ficha integral de ese animal.

Cursos de Excepción: 3a. El animal no tiene progenitores registrados: el sistema muestra el árbol con el padre y la madre como “No registrado”, sin rama para desplegar.

Post-condición: El sistema despliega la información solicitada sin alterar los registros existentes.

Reglas de Negocio: El árbol se construye a demanda: cada rama se resuelve cuando el usuario la despliega, de modo que un linaje profundo no obliga a recorrer todo el rodeo.

Validaciones: —

Frecuencia de Uso: Media, consultada principalmente durante la planificación de servicios o el análisis de descarte.

### CU 9 — Verificar Consanguinidad

Nombre del CU: Verificar Consanguinidad

Actores: Encargada del sector

Tipo: Primario

Descripción: El usuario consulta el grado de parentesco entre una hembra y un posible reproductor antes de asignar el servicio, de modo de evitar cruzamientos entre animales emparentados.

Referencia a Requerimientos Funcionales: RF1.7

Pre-condición: Ambos animales deben estar registrados en el sistema con su genealogía cargada.

Desencadenante: El usuario planifica el servicio de una hembra.

Curso Básico:

1. El usuario ingresa a la sección “Verificar Consanguinidad”.

2. El usuario selecciona la hembra por su número de caravana.

3. El usuario selecciona el reproductor, que puede ser un toro del rodeo o un toro de catálogo asociado a una pajuela.

4. El usuario presiona “Verificar”.

5. El sistema recorre la ascendencia registrada de ambos animales y busca progenitores comunes.

6. El sistema despliega el resultado indicando si existe parentesco y qué animal lo origina.

Cursos Alternativos: 3a. El usuario selecciona una pajuela: el sistema toma como reproductor de la verificación al toro de catálogo vinculado a esa pajuela.

Cursos de Excepción: 5a. Alguno de los animales no tiene toda su ascendencia registrada: el sistema informa que la verificación es parcial.

Post-condición: El usuario conoce el riesgo de consanguinidad antes de registrar el servicio.

Reglas de Negocio: Se considera parentesco la existencia de un ancestro común entre ambos animales. La verificación es informativa y no bloquea el registro del servicio; el sistema vuelve a advertirlo al registrarse el servicio (CU21).

Validaciones: La selección de la hembra y del reproductor es obligatoria. Ambos no pueden ser el mismo animal.

Frecuencia de Uso: Media, cada vez que se planifica un servicio.

### CU 10 — Buscar y Filtrar Animales del Rodeo

Nombre del CU: Buscar y Filtrar Animales del Rodeo

Actores: Encargada del sector

Tipo: Primario

Descripción: Permite realizar búsquedas y aplicar filtros combinados sobre los animales registrados.

Referencia a Requerimientos Funcionales: RF1.10

Pre-condición: El usuario debe estar logueado en el sistema.

Desencadenante: El usuario ingresa a la pantalla de consulta de animales.

Curso Básico:

1. El usuario ingresa a la pantalla de Consulta de Animales.

2. El sistema despliega el listado paginado y la barra de filtros de búsqueda.

3. El usuario ingresa o selecciona los criterios deseados —número de caravana, raza, categoría, estado o rango etario— y presiona “Buscar”.

4. El sistema procesa la solicitud y muestra únicamente los animales que coinciden con todos los filtros aplicados.

Cursos Alternativos: 4a. El resultado excede el tamaño de página: el sistema pagina el listado e informa la cantidad total de animales encontrados. 4b. Algún animal tiene la categoría desactualizada respecto de su edad: el sistema lo señala en el listado y ofrece actualizarla.

Cursos de Excepción: 4a. No existen registros que coincidan con los criterios: el sistema muestra un mensaje indicando que no se encontraron resultados.

Post-condición: El sistema muestra en pantalla la lista filtrada de animales sin modificar ningún registro.

Reglas de Negocio: Los filtros son acumulativos: el animal debe cumplir todos los criterios indicados. Por defecto el listado muestra los animales activos.

Validaciones: —

Frecuencia de Uso: Alta, cada vez que se requiere consultar la ficha o el estado de algún animal.

### CU 11 — Consultar Ficha Integral del Animal

Nombre del CU: Consultar Ficha Integral del Animal

Actores: Encargada del sector

Tipo: Primario

Descripción: Reúne en una sola pantalla todo lo que el sistema sabe de un animal: su identificación, su estado, su linaje y su historial productivo, reproductivo y sanitario.

Referencia a Requerimientos Funcionales: RF1.15

Pre-condición: El usuario debe estar logueado y el animal debe existir en el sistema.

Desencadenante: El usuario necesita conocer la situación completa de un animal, habitualmente antes de tomar una decisión de manejo.

Curso Básico:

1. El usuario llega a la ficha desde el listado de animales, desde el buscador de caravana o desde cualquier pantalla que mencione al animal.

2. El sistema despliega la identificación, la fotografía, la categoría, la edad y el estado productivo y reproductivo del animal.

3. El sistema despliega sus progenitores y el acceso al árbol genealógico.

4. El sistema despliega su historial: lactancias y controles de producción, celos, servicios, tactos y partos, y diagnósticos, tratamientos, vacunaciones y descornes.

5. El sistema ofrece, junto a cada sección, las acciones que corresponden al estado actual del animal.

Cursos Alternativos: 2a. El animal está dado de baja: el sistema lo informa junto con la fecha y el motivo, y ofrece reactivarlo (CU7). 4a. El animal tiene un período de descarte de leche vigente: el sistema lo advierte con la fecha en que finaliza.

Cursos de Excepción: —

Post-condición: El sistema expone la información sin alterar ningún registro.

Reglas de Negocio: La ficha es de sólo lectura: toda modificación se realiza a través del caso de uso correspondiente, al que la ficha da acceso.

Validaciones: —

Frecuencia de Uso: Alta, es la pantalla de consulta habitual del sistema.


## Modulo 2: Control de Produccion

### CU 12 — Registrar Ordeñe por Lote

Nombre del CU: Registrar Ordeñe por Lote

Actores: Encargada del sector

Tipo: Primario

Descripción: El usuario registra los litros totales obtenidos por el rodeo en un turno de ordeñe, tal como se leen del tanque.

Referencia a Requerimientos Funcionales: RF2.1, RF2.2

Pre-condición: El usuario debe estar logueado en el sistema.

Desencadenante: El usuario ingresa a la pantalla de ordeñe por lote al terminar el ordeñe.

Curso Básico:

1. El usuario selecciona la fecha y el turno, entre los turnos que corresponden a la cantidad de ordeñes diarios configurada.

2. El sistema carga automáticamente la lista de los animales con estado productivo “en lactancia”.

3. El sistema excluye de la lista a los animales que se encuentran dentro de un período de descarte de leche vigente.

4. El usuario modifica la lista si es necesario, removiendo los animales que no se ordeñaron o agregando los que faltan.

5. El usuario ingresa los litros totales obtenidos y presiona “Guardar”.

6. El sistema valida la consistencia de los litros ingresados.

7. El sistema almacena el registro del turno junto con los animales que lo integraron y confirma la operación.

Cursos Alternativos: 3a. El usuario decide incluir manualmente un animal excluido por descarte: el sistema advierte que la leche de ese animal no debe destinarse a consumo y solicita confirmación.

Cursos de Excepción: 5a. Ya existe un registro de ordeñe por lote para esa fecha y turno: el sistema informa la duplicación y remite a la corrección del registro existente (CU19). 6a. Los litros ingresados son negativos o superan el máximo admitido: el sistema muestra una alerta de error e impide el guardado.

Post-condición: El ordeñe del turno queda guardado en el historial de producción del establecimiento.

Reglas de Negocio: El ordeñe por lote es la medida de la leche que salió del tambo. Se registra una vez por fecha y turno. Los animales con un tratamiento cuyo período de descarte sigue vigente quedan excluidos del lote sin que se modifique su estado productivo.

Validaciones: La fecha, el turno y los litros totales son obligatorios. Los litros deben ser positivos y no superar el máximo por control individual configurado multiplicado por la cantidad de animales del lote.

Frecuencia de Uso: Alta, una vez por turno de ordeñe.

### CU 13 — Registrar Control Lechero

Nombre del CU: Registrar Control Lechero

Actores: Encargada del sector

Tipo: Primario

Descripción: Permite registrar cuántos litros produjo cada animal en un turno medido. El camino habitual es la carga masiva de todo el rodeo en ordeñe, que es como se realiza el control lechero en el establecimiento.

Referencia a Requerimientos Funcionales: RF2.2, RF2.3

Pre-condición: El usuario debe estar logueado y deben existir animales en lactancia.

Desencadenante: Se realiza el control lechero del mes.

Curso Básico:

1. El usuario ingresa a la pantalla de Control Lechero.

2. El usuario selecciona la fecha y el turno del control.

3. El sistema despliega la lista completa de los animales en ordeñe, cada uno con un campo para sus litros, junto con un filtro por caravana.

4. El usuario completa los litros de los animales medidos y presiona “Guardar”.

5. El sistema valida cada control por separado, lo imputa a la lactancia vigente del animal y lo almacena.

6. El sistema informa cuántos controles se guardaron y detalla cuáles no pudieron guardarse y por qué.

Cursos Alternativos: 1a. El usuario necesita registrar un solo animal —una vaca que faltó, una medición suelta o una carga retroactiva—: accede a “Control de una vaca” desde el control lechero o desde la ficha del animal, y registra el control individual con las mismas reglas.

Cursos de Excepción: 5a. El animal ya tiene un control registrado para esa fecha y turno: ese control no se guarda y el sistema lo informa, sin afectar a los demás. 5b. Los litros de un animal superan el máximo admitido: ese control no se guarda y el sistema lo informa.

Post-condición: Cada control queda asentado en el historial productivo de su animal e imputado a la lactancia vigente.

Reglas de Negocio: El control lechero mide al animal, no la leche vendible: a una vaca en tratamiento se la mide igual, aunque esa leche se descarte. Por eso el control individual y el ordeñe por lote no se suman dentro de un mismo turno. Cada control se guarda de forma independiente: el fallo de una fila no impide guardar las demás.

Validaciones: La fecha y el turno son obligatorios. Los litros de cada animal deben ser positivos y no superar el máximo por control configurado. Un animal no puede tener dos controles en la misma fecha y turno.

Frecuencia de Uso: Media, habitualmente una vez al mes.

### CU 14 — Consultar Historial de Producción y Lactancias

Nombre del CU: Consultar Historial de Producción y Lactancias

Actores: Encargada del sector

Tipo: Primario

Descripción: Permite consultar cronológicamente la producción del establecimiento y las lactancias de cada animal dentro de un rango de fechas.

Referencia a Requerimientos Funcionales: RF2.5, RF2.6, RF2.7

Pre-condición: El usuario debe estar logueado en el sistema.

Desencadenante: El usuario solicita consultar los historiales de producción.

Curso Básico:

1. El usuario ingresa a la sección de Historial de Producción.

2. El usuario selecciona la modalidad de visualización: producción del establecimiento o controles individuales.

3. El usuario define el rango de fechas y presiona “Buscar”.

4. El sistema recupera la información correspondiente al período.

5. El sistema despliega el listado y el acumulado de litros según la modalidad elegida.

Cursos Alternativos: 2a. El usuario consulta las lactancias: el sistema lista las lactancias registradas con su fecha de inicio y de cierre, los días en leche, la cantidad de controles y la producción estimada de cada una.

Cursos de Excepción: 3a. Rango de fechas inválido: el sistema muestra un mensaje de error y no procesa la consulta.

Post-condición: El sistema expone la información histórica sin alterar los registros de la base de datos.

Reglas de Negocio: La producción del establecimiento se resuelve turno por turno: si el turno tiene registro por lote, la producción de ese turno es ese total, que ya incluye a los animales medidos individualmente; si el turno se registró únicamente con controles individuales, la producción es la suma de esos controles. Las dos fuentes nunca se suman dentro de un mismo turno, porque eso contaría dos veces la leche de los animales controlados. La producción de una lactancia no es la suma de sus controles —que son mediciones puntuales— sino una estimación construida a partir de los intervalos entre controles sucesivos.

Validaciones: La selección de la modalidad y los campos de fecha son obligatorios. La fecha de inicio debe ser anterior o igual a la de fin.

Frecuencia de Uso: Media.

### CU 15 — Consultar Métrica de Producción Mensual

Nombre del CU: Consultar Métrica de Producción Mensual

Actores: Encargada del sector

Tipo: Primario

Descripción: Permite visualizar los litros totales producidos por el establecimiento en un mes calendario.

Referencia a Requerimientos Funcionales: RF2.4, RF2.7

Pre-condición: El usuario debe estar logueado en el sistema.

Desencadenante: El usuario accede al panel de producción mensual.

Curso Básico:

1. El usuario selecciona el mes y el año que desea consultar.

2. El sistema recorre los turnos del mes.

3. El sistema toma, para cada turno, el registro por lote cuando existe, y la suma de los controles individuales cuando el turno se registró únicamente de ese modo.

4. El sistema acumula los totales de cada turno.

5. El sistema despliega el total de litros del mes.

Cursos Alternativos: —

Cursos de Excepción: 2a. No existen registros de producción en el mes seleccionado: el sistema informa que no hay datos para el período.

Post-condición: Se visualiza el acumulado mensual sin realizar modificaciones en la base de datos.

Reglas de Negocio: El cálculo aplica la misma regla que el historial: un turno aporta su registro por lote o sus controles individuales, nunca ambos.

Validaciones: El mes y el año son obligatorios.

Frecuencia de Uso: Media, habitualmente al finalizar cada mes calendario.

### CU 16 — Registrar Período de Secado Manual

Nombre del CU: Registrar Período de Secado Manual

Actores: Encargada del sector

Tipo: Primario

Descripción: Permite registrar el inicio del período de secado de una vaca, removiéndola de la etapa productiva.

Referencia a Requerimientos Funcionales: RF2.8, RF2.11

Pre-condición: El usuario debe estar logueado y el animal debe estar en estado de lactancia.

Desencadenante: El usuario decide pasar un animal a descanso preparto.

Curso Básico:

1. El usuario busca al animal por su número de caravana.

2. El usuario selecciona la opción “Registrar Secado”.

3. El usuario ingresa la fecha correspondiente y confirma la acción.

4. El sistema guarda la fecha de cese del ordeñe y cierra la lactancia vigente.

5. El sistema cambia el estado productivo del animal a “seca”.

Cursos Alternativos: —

Cursos de Excepción: 2a. El animal no está en lactancia: el sistema informa la situación e impide el registro. 3a. La fecha de secado es anterior al inicio de la lactancia vigente o posterior al día de hoy: el sistema muestra el error y no guarda.

Post-condición: El animal queda con estado productivo “seca”, por lo cual el sistema deja de incluirlo automáticamente en los próximos ordeñes por lote. Su estado reproductivo no se modifica.

Reglas de Negocio: El cambio de estado productivo es automático e inmediato tras el guardado. El secado afecta únicamente el eje productivo: una hembra seca puede estar preñada, y de hecho es lo habitual.

Validaciones: El ingreso de la fecha de secado es obligatorio.

Frecuencia de Uso: Media.

### CU 17 — Consultar Alertas de Secado Próximo

Nombre del CU: Consultar Alertas de Secado Próximo

Actores: Encargada del sector

Tipo: Primario

Descripción: Despliega las vacas en producción que están próximas a cumplir el tiempo sugerido para iniciar su secado.

Referencia a Requerimientos Funcionales: RF2.9, RF2.10

Pre-condición: El usuario debe estar logueado en el sistema.

Desencadenante: El usuario ingresa a la sección de alertas de producción.

Curso Básico:

1. El usuario accede a la sección “Alertas de Secado”.

2. El sistema calcula, para cada hembra preñada y en lactancia, la fecha recomendada de secado restando los días de secado configurados a su fecha probable de parto.

3. El sistema selecciona aquellas cuya fecha recomendada cae dentro de la anticipación configurada.

4. El sistema despliega el listado con la fecha recomendada de cada animal y los días que restan.

Cursos Alternativos: —

Cursos de Excepción: —

Post-condición: Se exponen las alertas en la interfaz sin alterar ningún registro.

Reglas de Negocio: La fecha recomendada de secado depende de dos parámetros configurables: los días de secado previos al parto y la anticipación con la que se desea el aviso. Sólo se consideran hembras activas, preñadas y en lactancia.

Validaciones: —

Frecuencia de Uso: Alta, revisada periódicamente.

### CU 18 — Abrir Lactancia Manualmente

Nombre del CU: Abrir Lactancia Manualmente

Actores: Encargada del sector

Tipo: Primario

Descripción: Permite abrir la lactancia de una vaca que ya estaba en ordeñe cuando se comenzó a usar el sistema, sin un parto registrado que la origine.

Referencia a Requerimientos Funcionales: RF2.12

Pre-condición: El usuario debe estar logueado, el animal debe ser una hembra activa y no debe tener una lactancia abierta.

Desencadenante: Se pone el sistema en marcha con el rodeo ya en producción, o se incorpora una vaca en ordeñe sin registrar su parto.

Curso Básico:

1. El usuario ingresa a la sección de Lactancias y selecciona “Abrir Lactancia”.

2. El usuario selecciona el animal e ingresa la fecha de inicio.

3. El sistema propone el número de lactancia que corresponde según los partos registrados del animal.

4. El usuario confirma y el sistema abre la lactancia, deja al animal en estado productivo “en lactancia” y confirma la operación.

Cursos Alternativos: 3a. El usuario corrige el número de lactancia propuesto: el sistema conserva el valor ingresado.

Cursos de Excepción: 2a. El animal ya tiene una lactancia abierta: el sistema informa la situación e impide el registro. 2b. La fecha de inicio es futura: el sistema muestra el error y no guarda.

Post-condición: La lactancia queda abierta y los controles lecheros del animal se imputan a ella.

Reglas de Negocio: La vía normal de apertura de una lactancia es el parto (CU24). La apertura manual existe para la carga inicial del rodeo, donde no hay parto que registrar.

Validaciones: El animal y la fecha de inicio son obligatorios. Un animal no puede tener dos lactancias abiertas a la vez.

Frecuencia de Uso: Baja, concentrada en la puesta en marcha del sistema.

### CU 19 — Corregir o Eliminar Registro de Producción

Nombre del CU: Corregir o Eliminar Registro de Producción

Actores: Encargada del sector

Tipo: Primario

Descripción: Permite corregir o eliminar un ordeñe por lote o un control lechero ya registrado, informando qué registros dependen de aquel que se pretende eliminar.

Referencia a Requerimientos Funcionales: RF2.13

Pre-condición: El usuario debe estar logueado y el registro a corregir debe existir.

Desencadenante: El usuario advierte un error en un registro de producción ya guardado.

Curso Básico:

1. El usuario localiza el registro en el historial de producción.

2. El usuario selecciona “Editar” o “Eliminar”.

3. Al editar, el sistema despliega la misma pantalla que dio de alta el registro, con los datos cargados, y aplica las mismas validaciones.

4. Al eliminar, el sistema verifica que ningún otro registro dependa de éste.

5. El sistema aplica el cambio y confirma la operación.

Cursos Alternativos: 2a. El registro es un ordeñe por lote: la corrección alcanza los litros totales y la lista de animales del turno.

Cursos de Excepción: 4a. El registro tiene otros que dependen de él: el sistema informa cuáles son y qué debe eliminarse primero, sin borrar nada.

Post-condición: El registro queda corregido o eliminado, y los totales del período reflejan el cambio.

Reglas de Negocio: La corrección reusa la pantalla del alta: los campos y las reglas son los mismos. La eliminación de un ordeñe por lote no arrastra los controles individuales del turno, que siguen siendo mediciones válidas de sus animales.

Validaciones: Las mismas del registro original.

Frecuencia de Uso: Baja, ante errores de carga.


## Modulo 3: Gestion Reproductiva

### CU 20 — Registrar Detección de Celo

Nombre del CU: Registrar Detección de Celo

Actores: Encargada del sector

Tipo: Primario

Descripción: Permite asentar la fecha y las observaciones de la detección de celo de una hembra para el seguimiento de su ciclo reproductivo.

Referencia a Requerimientos Funcionales: RF3.1, RF3.10

Pre-condición: El usuario debe estar logueado y el animal debe ser una hembra.

Desencadenante: El usuario observa un animal en celo y accede a registrar el evento.

Curso Básico:

1. El usuario busca al animal por su número de caravana.

2. El usuario selecciona la opción “Registrar Celo”.

3. El sistema solicita la fecha de detección y las observaciones.

4. El usuario completa los datos y presiona “Guardar”.

5. El sistema valida que el animal esté en condiciones de manifestar celo.

6. El sistema almacena la novedad reproductiva y confirma el registro exitoso.

Cursos Alternativos: —

Cursos de Excepción: 1a. La caravana corresponde a un macho: el sistema emite un mensaje de error e impide la operación. 5a. El animal no alcanzó la edad mínima al servicio a la fecha del celo: el sistema informa la situación e impide el registro. 5b. La fecha del celo es posterior a la baja del animal: el sistema informa la situación e impide el registro.

Post-condición: El celo queda registrado en la ficha reproductiva histórica de la hembra.

Reglas de Negocio: Una ternera no manifiesta celo a efectos del manejo: el sistema exige que el animal haya alcanzado la edad mínima al servicio configurada. Un animal dado de baja no protagoniza eventos posteriores a su baja.

Validaciones: La fecha de detección es obligatoria y no puede ser futura.

Frecuencia de Uso: Alta, según los ciclos observados en el rodeo.

### CU 21 — Registrar Servicio

Nombre del CU: Registrar Servicio

Actores: Encargada del sector

Tipo: Primario

Descripción: El usuario registra el servicio de una hembra, ya sea por monta natural con un toro del rodeo o por inseminación artificial con una pajuela del stock.

Referencia a Requerimientos Funcionales: RF3.2, RF3.3, RF3.9, RF3.10, RF3.11, RF5.4

Pre-condición: La hembra debe estar registrada y en condiciones de recibir servicio. En la inseminación artificial debe existir stock de la pajuela seleccionada.

Desencadenante: El usuario detectó el celo de la hembra y decide darle servicio.

Curso Básico:

1. El usuario busca e ingresa la caravana de la hembra.

2. El sistema despliega el formulario de servicio solicitando la fecha y el tipo de servicio.

3. El usuario selecciona el tipo de servicio: monta natural o inseminación artificial.

4. El usuario indica el reproductor: un toro del rodeo si es monta natural, o una pajuela del stock si es inseminación artificial.

5. El usuario presiona “Guardar”.

6. El sistema valida que la hembra esté en condiciones de recibir servicio y presenta las advertencias que correspondan.

7. (Incluye: Descontar Automáticamente Semen de Stock, sólo en la inseminación artificial).

8. El sistema calcula la fecha probable de parto sumando el período de gestación a la fecha del servicio.

9. El sistema vincula genéticamente el servicio al toro, guarda el registro, deja a la hembra en estado reproductivo “servida” y confirma la operación.

Cursos Alternativos: 4a. El servicio es por monta natural: el sistema registra el toro del rodeo y no descuenta stock. 4b. El servicio es por inseminación artificial: el sistema registra la pajuela, descuenta una unidad del stock y toma el toro de catálogo vinculado a esa pajuela como reproductor. 6a. El sistema advierte que el toro elegido está dado de baja, o que existe parentesco entre la hembra y el reproductor: el usuario confirma y el registro continúa.

Cursos de Excepción: 6a. La hembra no alcanzó la edad mínima al servicio: el sistema informa la situación e impide el registro. 6b. La fecha del servicio es posterior a la baja de la hembra o del toro: el sistema informa la situación e impide el registro. 7a. La pajuela seleccionada no tiene stock disponible: el sistema informa la falta y no registra el servicio. 5a. El usuario no indicó reproductor: el sistema solicita completarlo antes de guardar.

Post-condición: El servicio queda registrado y asociado a su reproductor, con la fecha probable de parto calculada, y la hembra queda en estado reproductivo “servida”.

Reglas de Negocio: Todo servicio tiene un único reproductor: el toro del rodeo y la pajuela son mutuamente excluyentes. La pajuela conserva el vínculo con el toro que la aporta, de modo que la genealogía de la cría puede reconstruirse aunque el reproductor no integre el rodeo. El parentesco entre la hembra y el reproductor se advierte pero no bloquea: la decisión es del establecimiento.

Validaciones: La fecha, el tipo de servicio y el reproductor son obligatorios. La fecha del servicio no puede ser posterior a la fecha actual.

Frecuencia de Uso: Media.

### CU 22 — Registrar Tacto y Confirmación de Preñez

Nombre del CU: Registrar Tacto y Confirmación de Preñez

Actores: Encargada del sector

Tipo: Primario

Descripción: Permite registrar el resultado de un control reproductivo posterior al servicio y, en caso positivo, confirmar la preñez de la hembra.

Referencia a Requerimientos Funcionales: RF3.4, RF3.5, RF3.6, RF3.9

Pre-condición: El usuario debe estar logueado y el animal debe contar con un servicio vigente registrado.

Desencadenante: Se realiza el tacto clínico al animal.

Curso Básico:

1. El usuario busca la caravana del animal sometido a control.

2. El usuario accede a “Registrar Tacto”.

3. El usuario ingresa la fecha del tacto y selecciona el resultado: preñada, vacía o dudosa.

4. El usuario presiona “Guardar”.

5. El sistema actualiza el estado reproductivo de la hembra según el resultado: “preñada” si el tacto fue positivo, “vacía” si fue negativo.

6. El sistema conserva la fecha probable de parto calculada a partir del servicio cuando el resultado es positivo.

7. El sistema guarda el control y confirma la operación.

Cursos Alternativos: 5a. El resultado es dudoso: el sistema conserva el estado reproductivo “servida” y no confirma la preñez, a la espera de un nuevo control.

Cursos de Excepción: 3a. La fecha del tacto es anterior a la del servicio o posterior al día de hoy: el sistema muestra el error y no guarda. 3b. La fecha del tacto es posterior a la baja del animal: el sistema informa la situación e impide el registro.

Post-condición: El control queda registrado y el estado reproductivo de la hembra refleja el resultado del último tacto.

Reglas de Negocio: El tacto modifica el estado reproductivo y nunca el productivo: una vaca en lactancia confirmada preñada sigue produciendo. El estado reproductivo vigente es el que resulta del último tacto del servicio vigente: si un tacto se corrige o se elimina, el estado se vuelve a deducir.

Validaciones: La fecha y el resultado del tacto son obligatorios.

Frecuencia de Uso: Media, concentrada habitualmente en días de control veterinario programado.

### CU 23 — Consultar Alertas de Parto Próximo

Nombre del CU: Consultar Alertas de Parto Próximo

Actores: Encargada del sector

Tipo: Primario

Descripción: Lista las vacas preñadas que se encuentran próximas a su fecha probable de parto.

Referencia a Requerimientos Funcionales: RF3.7

Pre-condición: El usuario debe estar logueado.

Desencadenante: El usuario ingresa a la sección de alertas reproductivas.

Curso Básico:

1. El usuario selecciona la opción “Alertas de Parto”.

2. El sistema evalúa las fechas probables de parto de los servicios con preñez confirmada.

3. El sistema selecciona aquellos cuya fecha probable de parto cae dentro de la anticipación configurada.

4. El sistema despliega el listado con la fecha probable de cada animal y los días que restan.

Cursos Alternativos: —

Cursos de Excepción: —

Post-condición: Se exponen las alertas sin alterar datos en la base de datos.

Reglas de Negocio: La ventana de anticipación del aviso es un parámetro configurable del establecimiento. Sólo se consideran animales activos.

Validaciones: —

Frecuencia de Uso: Alta, de revisión cotidiana para preparar la maternidad.

### CU 24 — Registrar Parto

Nombre del CU: Registrar Parto

Actores: Encargada del sector

Tipo: Primario

Descripción: Permite documentar el parto de una vaca, dar de alta las crías nacidas como animales del rodeo y reactivar el estado productivo de la madre.

Referencia a Requerimientos Funcionales: RF3.8, RF3.9, RF3.11, RF2.11, RF1.9

Pre-condición: El usuario debe estar logueado y la madre debe figurar en el sistema.

Desencadenante: Ocurre un nacimiento en el establecimiento y el usuario procede a ingresarlo.

Curso Básico:

1. El usuario busca la caravana de la madre.

2. El usuario selecciona la opción “Registrar Parto”.

3. El sistema habilita el formulario solicitando la fecha del parto, el tipo y las observaciones, junto con los datos de cada cría: caravana, sexo, raza y fotografía.

4. El sistema propone como padre de las crías el toro del servicio que originó la preñez.

5. El usuario completa los datos y presiona “Confirmar Parto”.

6. El sistema presenta las advertencias que correspondan y solicita confirmación.

7. El sistema da de alta cada cría como animal del rodeo, enlazando a esta vaca como su madre.

8. El sistema cierra la lactancia anterior si seguía abierta, abre la nueva y deja a la madre en estado productivo “en lactancia” y reproductivo “vacía”.

9. El sistema incrementa la cantidad de partos de la madre, actualiza su categoría y confirma la operación.

Cursos Alternativos: 4a. El usuario indica otro padre, o ninguno: el sistema conserva lo indicado. 5a. El parto no tuvo crías vivas: el usuario registra el parto sin crías y el sistema aplica igualmente los cambios de estado de la madre. 6a. El sistema advierte que la gestación duró menos o más de lo normal, que la madre no figuraba preñada, o que las crías mellizas son de distinto sexo —caso en que la hembra puede resultar estéril—: el usuario confirma y el registro continúa.

Cursos de Excepción: 7a. El número de caravana asignado a una cría ya se encuentra registrado: el sistema muestra un mensaje de error por duplicado e impide guardar hasta corregirlo. 3a. La fecha del parto es futura o posterior a la baja de la madre: el sistema muestra el error y no guarda.

Post-condición: Las crías ingresan al rodeo activo. La madre queda con estado productivo “en lactancia” y reproductivo “vacía”, habilitada nuevamente para los lotes de ordeñe, y con una lactancia abierta.

Reglas de Negocio: El parto actúa sobre los dos ejes a la vez: cierra el ciclo reproductivo devolviendo la hembra a “vacía” e inicia el ciclo productivo llevándola a “en lactancia”. Es el único evento que actualiza la categoría del animal sin intervención del usuario, porque el cambio de condición biológica no admite ambigüedad. Las crías son animales del rodeo desde el momento del registro y se corrigen desde el módulo de animales.

Validaciones: La fecha del parto es obligatoria y no puede ser futura. La caravana, el sexo y la raza de cada cría son obligatorios, y la caravana debe ser única.

Frecuencia de Uso: Media.

### CU 25 — Consultar Listas de Trabajo Reproductivas

Nombre del CU: Consultar Listas de Trabajo Reproductivas

Actores: Encargada del sector

Tipo: Primario

Descripción: Presenta las dos listas que ordenan el trabajo reproductivo de la jornada: los servicios que esperan tacto y las hembras en condiciones de ser servidas.

Referencia a Requerimientos Funcionales: RF3.12

Pre-condición: El usuario debe estar logueado en el sistema.

Desencadenante: El usuario planifica el trabajo reproductivo del día.

Curso Básico:

1. El usuario accede a “Tactos Pendientes” o a “Vacas para Servir”.

2. En la primera, el sistema selecciona los servicios vigentes cuya preñez no fue confirmada ni descartada y sobre los que ya transcurrió el plazo habitual para tactar.

3. En la segunda, el sistema selecciona las hembras activas que están en condiciones de recibir servicio.

4. El sistema despliega cada lista indicando, para cada animal, el motivo por el cual figura.

Cursos Alternativos: 4a. El usuario selecciona un animal de la lista: el sistema abre el registro del evento que corresponde, con el animal ya seleccionado.

Cursos de Excepción: —

Post-condición: El usuario conoce el trabajo reproductivo pendiente sin que se altere ningún registro.

Reglas de Negocio: Una hembra está para servir si alcanzó la edad mínima al servicio, no está preñada y no tiene un servicio a la espera de tacto. El motivo se muestra en texto: el sistema informa, no decide.

Validaciones: —

Frecuencia de Uso: Alta, de revisión cotidiana.

### CU 26 — Corregir o Eliminar Evento Reproductivo

Nombre del CU: Corregir o Eliminar Evento Reproductivo

Actores: Encargada del sector

Tipo: Primario

Descripción: Permite corregir o eliminar un celo, un servicio, un tacto o un parto ya registrado, volviendo a deducir el estado reproductivo del animal a partir de los eventos que permanecen.

Referencia a Requerimientos Funcionales: RF3.13

Pre-condición: El usuario debe estar logueado y el evento a corregir debe existir.

Desencadenante: El usuario advierte un error en un evento reproductivo ya guardado.

Curso Básico:

1. El usuario localiza el evento en el listado del módulo o en la ficha del animal.

2. El usuario selecciona “Editar” o “Eliminar”.

3. Al editar, el sistema despliega la pantalla de registro con los datos cargados y aplica las mismas validaciones y advertencias.

4. Al eliminar, el sistema verifica que ningún otro registro dependa de éste.

5. El sistema aplica el cambio, vuelve a deducir el estado reproductivo del animal y confirma la operación.

Cursos Alternativos: 2a. El evento es un parto: la corrección se realiza en una pantalla propia, que alcanza la fecha, el tipo y las observaciones. Las crías ya son animales del rodeo y se corrigen desde el módulo de animales. 5a. El evento eliminado había descontado una pajuela del stock: el sistema la devuelve mediante un contra-movimiento.

Cursos de Excepción: 4a. El evento tiene otros que dependen de él —un servicio con tactos registrados, un parto con crías o con controles de producción imputados a su lactancia—: el sistema informa cuáles son y qué debe eliminarse primero, sin borrar nada.

Post-condición: El evento queda corregido o eliminado y el estado reproductivo del animal es coherente con los eventos que permanecen registrados.

Reglas de Negocio: El estado reproductivo no se deshace paso a paso: se vuelve a deducir del servicio vigente y de su último tacto. Es lo que evita que una secuencia de correcciones deje al animal en un estado que no corresponde a ningún evento real.

Validaciones: Las mismas del registro original.

Frecuencia de Uso: Baja, ante errores de carga.


## Modulo 4: Gestion Sanitaria

### CU 27 — Registrar Diagnóstico o Revisación

Nombre del CU: Registrar Diagnóstico o Revisación

Actores: Encargada del sector

Tipo: Primario

Descripción: Permite registrar el hallazgo de una patología o el resultado de una revisación clínica en un animal, dejando asentado el antecedente para su posterior seguimiento o tratamiento.

Referencia a Requerimientos Funcionales: RF4.1

Pre-condición: El usuario debe estar logueado y el animal debe existir en el sistema.

Desencadenante: Un animal es revisado o se le detecta una enfermedad en el campo.

Curso Básico:

1. El usuario busca al animal por su número de caravana.

2. El usuario selecciona la opción “Registrar Diagnóstico”.

3. El usuario ingresa la patología detectada o las notas de la revisación y la fecha del hallazgo.

4. El usuario presiona “Guardar”.

5. El sistema almacena el diagnóstico en el historial del animal como cuadro activo y confirma el éxito.

Cursos Alternativos: —

Cursos de Excepción: 3a. La fecha del diagnóstico es futura: el sistema muestra el error y no guarda.

Post-condición: El diagnóstico queda registrado en la ficha del animal, habilitándolo para la posterior asignación de tratamientos.

Reglas de Negocio: Un diagnóstico permanece activo hasta que se lo cierra explícitamente (CU33). Un mismo animal puede tener más de un cuadro activo a la vez.

Validaciones: La patología y la fecha son obligatorias.

Frecuencia de Uso: Alta, cada vez que se detectan anomalías sanitarias.

### CU 28 — Registrar Tratamiento Sanitario

Nombre del CU: Registrar Tratamiento Sanitario

Actores: Encargada del sector

Tipo: Primario

Descripción: Permite registrar el tratamiento aplicado a un animal, indicando el insumo, la cantidad utilizada y los días de duración, y calculando el período de descarte de leche resultante.

Referencia a Requerimientos Funcionales: RF4.2, RF4.3, RF5.3

Pre-condición: El usuario debe estar logueado y el animal debe existir en el sistema.

Desencadenante: Se define el protocolo médico para combatir la patología del animal, o se aplica un tratamiento preventivo.

Curso Básico:

1. El usuario busca la caravana del animal.

2. El usuario selecciona la opción “Registrar Tratamiento” e indica el diagnóstico que lo motiva, si corresponde.

3. El usuario elige el insumo desde el stock, ingresa la cantidad a aplicar, la dosis y los días de duración.

4. El usuario presiona “Guardar”.

5. El sistema verifica que exista stock suficiente del insumo.

6. El sistema descuenta del stock la cantidad indicada, imputándola a la partida que vence primero.

7. El sistema calcula la fecha de fin del período de descarte sumando los días de duración y el período de carencia del insumo, y la propone al usuario.

8. El sistema guarda el tratamiento y confirma la operación.

Cursos Alternativos: 2a. El tratamiento es preventivo, como una desparasitación, y no proviene de un diagnóstico: el usuario lo registra directamente sobre el animal y el sistema deja el diagnóstico sin asociar. 2b. El tratamiento cumple un plan sanitario configurado: el usuario lo selecciona y el sistema lo registra como aplicación de ese plan. 7a. El usuario ajusta la fecha de fin de descarte propuesta: el sistema conserva el valor ingresado.

Cursos de Excepción: 5a. El stock disponible del insumo es menor a la cantidad indicada: el sistema muestra una alerta de stock insuficiente e impide el guardado. 3a. La fecha de inicio es futura o posterior a la baja del animal: el sistema muestra el error y no guarda.

Post-condición: El tratamiento queda registrado, el stock del insumo refleja el consumo y el animal queda excluido del lote de ordeñe mientras el descarte esté vigente.

Reglas de Negocio: La cantidad de insumo a descontar se ingresa y no se calcula: depende del peso del animal y de la presentación del producto, datos que el sistema no administra. El tratamiento se registra sobre el animal, con el diagnóstico como dato opcional, de modo que el tratamiento preventivo no obligue a inventar un diagnóstico. La fecha de fin de descarte se calcula automáticamente pero admite ajuste manual.

Validaciones: El animal, el insumo, la cantidad y los días de duración son obligatorios. La cantidad y los días deben ser positivos.

Frecuencia de Uso: Media.

### CU 29 — Registrar Vacunación

Nombre del CU: Registrar Vacunación

Actores: Encargada del sector

Tipo: Primario

Descripción: Permite asentar la aplicación de una vacuna a un animal, indicando la fecha y el biológico utilizado.

Referencia a Requerimientos Funcionales: RF4.4

Pre-condición: El usuario debe estar logueado en el sistema.

Desencadenante: Se realiza una jornada de vacunación obligatoria o preventiva.

Curso Básico:

1. El usuario busca al animal por su número de caravana.

2. El usuario selecciona la opción “Registrar Vacunación”.

3. El usuario selecciona la vacuna aplicada y la fecha de ejecución.

4. El usuario indica, si corresponde, el plan sanitario que la aplicación da por cumplido.

5. El usuario presiona “Guardar”.

6. El sistema descuenta la unidad de vacuna del stock, guarda el registro y confirma la operación.

Cursos Alternativos: 4a. La vacunación se aplica fuera de todo plan: el usuario no selecciona plan y el sistema registra la aplicación sin vincularla.

Cursos de Excepción: 6a. No hay stock disponible de la vacuna: el sistema informa la falta e impide el guardado. 3a. La fecha es futura o posterior a la baja del animal: el sistema muestra el error y no guarda.

Post-condición: El registro queda guardado en la ficha sanitaria cronológica del animal y el calendario sanitario lo considera como aplicación cumplida del plan indicado.

Reglas de Negocio: La aplicación declara explícitamente qué plan sanitario cumple, de modo que el calendario no deba inferirlo a partir del insumo utilizado. Una aplicación sólo puede cumplir un plan de su propio tipo.

Validaciones: La selección de la vacuna y la fecha de aplicación son obligatorias.

Frecuencia de Uso: Baja/Media, sujeta al plan sanitario estacional.

### CU 30 — Configurar Plan Sanitario

Nombre del CU: Configurar Plan Sanitario

Actores: Encargada del sector

Tipo: Primario

Descripción: El usuario define las reglas de los procedimientos sanitarios periódicos —vacunaciones, desparasitaciones y descornes— que el establecimiento debe cumplir, y que el sistema utiliza para calcular el calendario sanitario.

Referencia a Requerimientos Funcionales: RF4.7

Pre-condición: Los insumos y las categorías deben estar registrados en el sistema.

Desencadenante: El usuario necesita dar de alta o ajustar un esquema sanitario del establecimiento.

Curso Básico:

1. El usuario ingresa a la sección “Planes Sanitarios”.

2. El sistema despliega los planes ya configurados junto con su estado.

3. El usuario selecciona “Nuevo Plan” o elige un plan existente para modificarlo.

4. El sistema despliega el formulario solicitando: nombre, tipo de procedimiento (vacunación, desparasitación o descorne), insumo a aplicar, periodicidad en días, edad de inicio en meses y categorías alcanzadas.

5. El usuario completa los datos y presiona “Guardar”.

6. El sistema valida la consistencia de los parámetros y almacena el plan junto con las categorías asociadas.

7. El sistema confirma la operación e informa que el calendario sanitario se recalculará con la nueva regla.

Cursos Alternativos: 4a. El usuario no selecciona ninguna categoría: el plan queda alcanzando a todo el rodeo. 4b. El usuario deja la periodicidad vacía: el sistema interpreta que el procedimiento se aplica una única vez en la vida del animal. 4c. El tipo de procedimiento es descorne: el sistema no solicita insumo. 3a. El usuario desactiva un plan: el sistema deja de generar procedimientos pendientes a partir de él, sin borrar las aplicaciones ya registradas.

Cursos de Excepción: 6a. Ya existe un plan con el mismo nombre: el sistema informa la duplicación y no guarda. 6b. La periodicidad o la edad de inicio no son valores positivos: el sistema muestra el error y no guarda.

Post-condición: El plan queda registrado y pasa a generar procedimientos pendientes en el calendario sanitario.

Reglas de Negocio: Sólo los planes activos generan procedimientos pendientes. Un plan sin categorías asociadas alcanza a todo el rodeo. Un plan sin periodicidad se considera cumplido para el animal una vez que se registró la aplicación. La misma pantalla crea y modifica los planes.

Validaciones: El nombre, el tipo de procedimiento y la edad de inicio son obligatorios. El insumo es obligatorio salvo que el tipo de procedimiento sea descorne. El nombre del plan es único.

Frecuencia de Uso: Baja, únicamente cuando se define o se ajusta un esquema sanitario.

### CU 31 — Consultar Calendario Sanitario

Nombre del CU: Consultar Calendario Sanitario

Actores: Encargada del sector

Tipo: Primario

Descripción: El usuario consulta el cronograma de procedimientos sanitarios pendientes y vencidos del rodeo, derivado de los planes sanitarios configurados.

Referencia a Requerimientos Funcionales: RF4.5

Pre-condición: Deben existir planes sanitarios activos y animales registrados en el rodeo.

Desencadenante: El usuario necesita saber qué procedimientos sanitarios están pendientes.

Curso Básico:

1. El usuario accede a la sección “Calendario Sanitario”.

2. El sistema recupera los planes sanitarios activos.

3. Para cada plan, el sistema selecciona los animales activos que pertenecen a las categorías alcanzadas y que superan la edad de inicio.

4. El sistema busca, para cada animal, la última aplicación registrada de ese plan.

5. El sistema proyecta la fecha del próximo procedimiento sumando la periodicidad a la última aplicación, o toma la fecha en que el animal alcanzó la edad de inicio cuando nunca se aplicó.

6. El sistema descarta los planes sin periodicidad que ya fueron aplicados al animal.

7. El sistema despliega el cronograma ordenado por fecha, distinguiendo los procedimientos vencidos de los próximos a vencer dentro de la anticipación configurada.

Cursos Alternativos: 7a. El usuario filtra por tipo de procedimiento o por categoría: el sistema restringe el cronograma a los pendientes que cumplen el filtro.

Cursos de Excepción: 2a. No existen planes sanitarios activos: el sistema informa que no hay procedimientos programados y sugiere configurar un plan.

Post-condición: El usuario conoce los procedimientos sanitarios vencidos y próximos a vencer del rodeo.

Reglas de Negocio: El pendiente resulta de la diferencia entre lo que el plan exige y lo que efectivamente se aplicó. El cálculo es el mismo que alimentará el resumen diario enviado por Telegram, de modo que ambas vistas no puedan discrepar. Los animales inactivos no se consideran.

Validaciones: El horizonte de anticipación debe ser un número positivo de días.

Frecuencia de Uso: Alta, es la consulta de planificación sanitaria del establecimiento.

### CU 32 — Registrar Procedimiento de Descorne

Nombre del CU: Registrar Procedimiento de Descorne

Actores: Encargada del sector

Tipo: Primario

Descripción: Permite registrar los procedimientos de descorne efectuados en los animales, para el control del bienestar y el manejo del rodeo.

Referencia a Requerimientos Funcionales: RF4.6

Pre-condición: El usuario debe estar logueado y el animal debe estar registrado.

Desencadenante: Se realiza el descorne físico de un animal.

Curso Básico:

1. El usuario busca la caravana del animal.

2. El usuario selecciona la opción “Registrar Descorne”.

3. El usuario ingresa la fecha del procedimiento y el método utilizado.

4. El usuario indica, si corresponde, el plan sanitario que la aplicación da por cumplido.

5. El usuario presiona “Confirmar”.

6. El sistema graba la información en el expediente del animal y confirma la operación.

Cursos Alternativos: —

Cursos de Excepción: 5a. El animal ya tiene un descorne registrado: el sistema informa la situación e impide el registro. 3a. La fecha es futura o posterior a la baja del animal: el sistema muestra el error y no guarda.

Post-condición: La intervención queda registrada en la ficha del animal y el calendario sanitario deja de exigirla.

Reglas de Negocio: El descorne es un procedimiento de umbral de edad y de aplicación única: un animal se descorna una sola vez, y un plan sanitario de descorne deja de exigirlo una vez registrado.

Validaciones: La fecha es obligatoria y no puede ser futura.

Frecuencia de Uso: Baja/Media, según la cantidad de terneros del rodeo.

### CU 33 — Cerrar Diagnóstico

Nombre del CU: Cerrar Diagnóstico

Actores: Encargada del sector

Tipo: Primario

Descripción: Permite dar por resuelto un cuadro sanitario, de modo que el sistema distinga los diagnósticos activos de los ya cerrados.

Referencia a Requerimientos Funcionales: RF4.8

Pre-condición: El usuario debe estar logueado y el diagnóstico debe estar activo.

Desencadenante: El animal se recupera del cuadro registrado.

Curso Básico:

1. El usuario localiza el diagnóstico en el listado de diagnósticos o en la ficha del animal.

2. El usuario selecciona la opción de cerrarlo y confirma.

3. El sistema registra la resolución del diagnóstico y confirma la operación.

Cursos Alternativos: —

Cursos de Excepción: —

Post-condición: El diagnóstico deja de figurar entre los cuadros activos del animal, conservándose en su historial sanitario junto con sus tratamientos.

Reglas de Negocio: Cerrar el diagnóstico no cierra los tratamientos asociados ni modifica el período de descarte de leche, que depende de la fecha de fin del tratamiento.

Validaciones: —

Frecuencia de Uso: Media, al resolverse cada cuadro sanitario.

### CU 34 — Corregir o Eliminar Evento Sanitario

Nombre del CU: Corregir o Eliminar Evento Sanitario

Actores: Encargada del sector

Tipo: Primario

Descripción: Permite corregir o eliminar un diagnóstico, un tratamiento, una vacunación o un descorne ya registrado, devolviendo al stock los insumos que se habían descontado.

Referencia a Requerimientos Funcionales: RF4.9, RF5.10

Pre-condición: El usuario debe estar logueado y el evento a corregir debe existir.

Desencadenante: El usuario advierte un error en un evento sanitario ya guardado.

Curso Básico:

1. El usuario localiza el evento en el listado del módulo o en la ficha del animal.

2. El usuario selecciona “Editar” o “Eliminar”.

3. Al editar, el sistema despliega la pantalla de registro con los datos cargados y aplica las mismas validaciones.

4. Al eliminar, el sistema verifica que ningún otro registro dependa de éste.

5. El sistema devuelve al stock los insumos consumidos por el evento, mediante un contra-movimiento.

6. El sistema aplica el cambio y confirma la operación.

Cursos Alternativos: 3a. La corrección cambia la cantidad de insumo aplicada: el sistema ajusta el stock por la diferencia. 3b. La corrección cambia los días de duración o el insumo del tratamiento: el sistema recalcula la fecha de fin del período de descarte.

Cursos de Excepción: 4a. El diagnóstico tiene tratamientos registrados: el sistema informa cuáles son y que deben eliminarse primero, sin borrar nada.

Post-condición: El evento queda corregido o eliminado, el stock refleja la devolución y el período de descarte del animal es coherente con los tratamientos que permanecen.

Reglas de Negocio: El movimiento de stock original nunca se borra: la devolución se registra como un movimiento nuevo de signo contrario, de modo que el historial conserve lo que efectivamente ocurrió.

Validaciones: Las mismas del registro original.

Frecuencia de Uso: Baja, ante errores de carga.


## Modulo 5: Control de Insumos y Stock

### CU 35 — Registrar Alta e Ingreso de Insumo

Nombre del CU: Registrar Alta e Ingreso de Insumo

Actores: Encargada del sector

Tipo: Primario

Descripción: Permite dar de alta un nuevo insumo —medicamento, vacuna, antiparasitario o pajuela— o registrar el ingreso de una nueva partida de uno existente.

Referencia a Requerimientos Funcionales: RF5.1, RF5.2, RF5.7

Pre-condición: El usuario debe estar logueado en el sistema.

Desencadenante: El establecimiento adquiere insumos o necesita inventariar el stock disponible.

Curso Básico:

1. El usuario accede a la sección de Insumos y selecciona el alta o el ingreso de stock.

2. El usuario selecciona el tipo de insumo: medicamento, vacuna, antiparasitario o pajuela.

3. El usuario ingresa los datos identificatorios del insumo, la cantidad que ingresa y la fecha de vencimiento de la partida.

4. El usuario presiona “Guardar”.

5. El sistema valida la consistencia de los datos.

6. El sistema registra el movimiento de ingreso con su fecha de vencimiento, actualiza el stock disponible y confirma la operación.

Cursos Alternativos: 2a. El insumo es una pajuela: el sistema solicita además el toro que la aporta, de modo de conservar el vínculo genético. 2b. El insumo obliga a descartar leche: el sistema solicita el período de carencia en días. 1a. El insumo ya existe: el usuario registra únicamente el ingreso de la nueva partida.

Cursos de Excepción: 5a. La cantidad ingresada es menor o igual a cero: el sistema muestra un mensaje de error e impide el guardado. 5b. Ya existe un insumo con el mismo nombre y tipo: el sistema informa la duplicación y no da de alta uno nuevo.

Post-condición: El stock disponible del insumo se actualiza de forma inmediata y la partida queda registrada con su vencimiento.

Reglas de Negocio: El alta de un insumo con cantidad inicial registra esa cantidad como un movimiento de ingreso, de modo que el stock siempre sea la suma de sus movimientos y no un número guardado aparte. El alta y el ingreso se resuelven en una única transacción: si el movimiento no se registra, el insumo tampoco.

Validaciones: El tipo, el nombre y la cantidad son obligatorios. La cantidad debe ser positiva. La fecha de vencimiento, cuando corresponde, no puede ser anterior a la fecha de ingreso.

Frecuencia de Uso: Baja/Media, sujeta a la frecuencia de compras o auditorías de stock.

### CU 36 — Configurar Umbral de Stock Mínimo

Nombre del CU: Configurar Umbral de Stock Mínimo

Actores: Encargada del sector

Tipo: Primario

Descripción: Permite definir la cantidad mínima de reserva de cada insumo, con el fin de activar alertas antes de que ocurra un desabastecimiento.

Referencia a Requerimientos Funcionales: RF5.5

Pre-condición: El usuario debe estar logueado y el insumo debe estar dado de alta.

Desencadenante: El usuario desea establecer o ajustar la barrera de seguridad de stock de un insumo.

Curso Básico:

1. El usuario busca el insumo en el listado del sistema.

2. El usuario selecciona la opción “Configurar Stock Mínimo”.

3. El usuario ingresa el valor que representará el umbral de alerta.

4. El usuario presiona “Guardar”.

5. El sistema almacena el umbral asignado a ese insumo y confirma la operación.

Cursos Alternativos: —

Cursos de Excepción: 3a. El valor ingresado es negativo: el sistema emite una alerta de error e impide el almacenamiento.

Post-condición: Queda fijado el límite crítico de existencias para el insumo seleccionado.

Reglas de Negocio: —

Validaciones: El stock mínimo es obligatorio y debe ser mayor o igual a cero.

Frecuencia de Uso: Baja, por lo general se configura una única vez por insumo.

### CU 37 — Consultar Alertas de Stock Crítico

Nombre del CU: Consultar Alertas de Stock Crítico

Actores: Encargada del sector

Tipo: Primario

Descripción: Despliega los insumos cuyas existencias son iguales o inferiores al umbral mínimo configurado.

Referencia a Requerimientos Funcionales: RF5.6

Pre-condición: El usuario debe estar logueado en el sistema.

Desencadenante: El usuario accede al panel de inventario para planificar futuras compras.

Curso Básico:

1. El usuario ingresa a la sección “Alertas de Stock”.

2. El sistema calcula el stock disponible de cada insumo.

3. El sistema selecciona aquellos cuyo stock actual es menor o igual al stock mínimo configurado.

4. El sistema muestra el listado con la cantidad disponible y el umbral de cada insumo.

Cursos Alternativos: —

Cursos de Excepción: —

Post-condición: El sistema expone el estado de alerta sin alterar ningún registro.

Reglas de Negocio: La condición de stock crítico se evalúa sobre el stock vigente, que resulta de los movimientos registrados: los ingresos de compra, los egresos automáticos al registrarse un tratamiento (CU28), una vacunación (CU29) o una inseminación (CU21), y las devoluciones por corrección.

Validaciones: —

Frecuencia de Uso: Alta, revisada de manera frecuente para evitar quiebres de stock.

### CU 38 — Consultar Alertas de Vencimiento de Insumos

Nombre del CU: Consultar Alertas de Vencimiento de Insumos

Actores: Encargada del sector

Tipo: Primario

Descripción: Despliega las partidas de insumos vencidas o próximas a vencer, para retirarlas de uso o priorizar su aplicación.

Referencia a Requerimientos Funcionales: RF5.8

Pre-condición: El usuario debe estar logueado en el sistema.

Desencadenante: El usuario revisa el estado de los insumos almacenados.

Curso Básico:

1. El usuario ingresa a la sección “Alertas de Vencimiento”.

2. El sistema recorre las partidas ingresadas de cada insumo y calcula el remanente de cada una.

3. El sistema identifica las partidas ya vencidas y aquellas cuyo vencimiento se produce dentro de la anticipación configurada.

4. El sistema despliega el listado ordenado por fecha de vencimiento, indicando el insumo, la partida, la cantidad remanente y los días restantes.

Cursos Alternativos: —

Cursos de Excepción: —

Post-condición: Se exponen las alertas de vencimiento sin alterar ningún registro.

Reglas de Negocio: El vencimiento se registra a nivel de cada ingreso, por lo que un mismo insumo puede tener varias partidas con vencimientos distintos. El consumo se imputa a la partida que vence primero, de modo que el remanente de cada partida se calcula descontando los egresos en ese orden. Las partidas agotadas no figuran.

Validaciones: —

Frecuencia de Uso: Media, revisada de forma periódica y antes de cada jornada sanitaria.

### CU 39 — Consultar Historial de Movimientos de Stock

Nombre del CU: Consultar Historial de Movimientos de Stock

Actores: Encargada del sector

Tipo: Primario

Descripción: Permite consultar cronológicamente todos los ingresos y egresos de stock registrados sobre los insumos del establecimiento.

Referencia a Requerimientos Funcionales: RF5.9, RF5.10

Pre-condición: El usuario debe estar logueado en el sistema.

Desencadenante: El usuario necesita auditar el consumo o el reabastecimiento de un insumo.

Curso Básico:

1. El usuario ingresa a la sección “Historial de Movimientos”.

2. El sistema despliega los filtros disponibles: insumo, tipo de movimiento y rango de fechas.

3. El usuario selecciona los criterios deseados y presiona “Buscar”.

4. El sistema recupera los movimientos que coinciden con los filtros aplicados.

5. El sistema despliega el listado paginado detallando fecha, tipo de movimiento, cantidad, motivo y stock resultante.

Cursos Alternativos: —

Cursos de Excepción: 4a. No existen movimientos que coincidan con los criterios: el sistema muestra un mensaje indicando que no se encontraron resultados.

Post-condición: El sistema expone la información histórica sin alterar los registros de la base de datos.

Reglas de Negocio: Todo egreso automático generado por el sistema queda asentado como un movimiento más del historial, indicando la operación que lo originó. Las devoluciones por corrección figuran como movimientos propios: el movimiento original nunca se borra. El stock resultante que se muestra en cada fila se reconstruye hacia atrás desde el stock actual, de modo que refleje la situación en el momento de cada movimiento.

Validaciones: El rango de fechas debe ser válido, con fecha de inicio anterior o igual a la fecha de fin.

Frecuencia de Uso: Media, ante auditorías de inventario o control de consumos.


## Modulo 6: Tablero, Indicadores y Apoyo a la Decision

### CU 40 — Consultar Tablero de Inicio

Nombre del CU: Consultar Tablero de Inicio

Actores: Encargada del sector

Tipo: Primario

Descripción: Presenta, como pantalla de entrada al sistema, el estado del día del establecimiento: lo que hay pendiente y lo que vence.

Referencia a Requerimientos Funcionales: RF6.1

Pre-condición: El usuario debe estar logueado en el sistema.

Desencadenante: El usuario inicia sesión o vuelve a la pantalla principal.

Curso Básico:

1. El sistema recupera los avisos vigentes de cada módulo: secados próximos, partos próximos, tactos pendientes, hembras para servir, procedimientos sanitarios vencidos y por vencer, insumos en stock crítico y partidas próximas a vencer.

2. El sistema recupera las cifras del día: producción registrada y cantidad de animales en ordeñe.

3. El sistema despliega cada grupo de avisos con su cantidad y el acceso directo a la pantalla que lo resuelve.

Cursos Alternativos: 3a. No hay avisos pendientes en un grupo: el sistema lo informa en lugar de mostrar una lista vacía.

Cursos de Excepción: —

Post-condición: El usuario conoce el estado del establecimiento sin que se altere ningún registro.

Reglas de Negocio: El tablero no calcula nada propio: reúne los mismos avisos que producen los casos de uso de cada módulo, de modo que no puedan discrepar entre una vista y la otra.

Validaciones: —

Frecuencia de Uso: Muy alta, es la pantalla de entrada al sistema.

### CU 41 — Consultar Indicadores del Rodeo

Nombre del CU: Consultar Indicadores del Rodeo

Actores: Encargada del sector

Tipo: Primario

Descripción: Presenta los indicadores de desempeño reproductivo y productivo del rodeo, que permiten evaluar la marcha del establecimiento más allá del registro diario.

Referencia a Requerimientos Funcionales: RF6.2

Pre-condición: El usuario debe estar logueado y debe existir historial registrado.

Desencadenante: El usuario necesita evaluar el desempeño del rodeo.

Curso Básico:

1. El usuario ingresa a la sección “Indicadores”.

2. El sistema calcula los indicadores reproductivos: días abiertos promedio, intervalo entre partos y servicios por preñez.

3. El sistema calcula los indicadores productivos: litros por vaca y por día, y días en leche promedio.

4. El sistema calcula la composición del rodeo por estado productivo y reproductivo.

5. El sistema arma el ranking de las lactancias en curso por producción diaria, con la proyección a 305 días de cada una.

6. El sistema despliega los indicadores junto con la aclaración de cómo se calcula cada uno.

Cursos Alternativos: 2a. No hay partos registrados suficientes para calcular un indicador: el sistema informa que no hay datos en lugar de mostrar un valor engañoso.

Cursos de Excepción: —

Post-condición: El usuario dispone de los indicadores sin que se altere ningún registro.

Reglas de Negocio: Los días abiertos se cuentan del parto a la concepción; mientras la hembra no queda preñada el número sigue corriendo, y eso también es información. La proyección a 305 días supone que el animal sostiene su último nivel controlado: es lineal y por lo tanto optimista, y sirve para comparar animales entre sí, no como pronóstico.

Validaciones: —

Frecuencia de Uso: Media, de revisión periódica.

### CU 42 — Consultar Candidatas a Descarte

Nombre del CU: Consultar Candidatas a Descarte

Actores: Encargada del sector

Tipo: Primario

Descripción: Lista las hembras que cumplen alguno de los criterios de descarte, indicando por cuál de ellos figura cada una, como apoyo a la decisión de refugo del rodeo.

Referencia a Requerimientos Funcionales: RF6.3

Pre-condición: El usuario debe estar logueado y debe existir historial registrado.

Desencadenante: El usuario evalúa qué animales conviene retirar del rodeo.

Curso Básico:

1. El usuario ingresa a la sección “Candidatas a Descarte”.

2. El sistema evalúa, para cada hembra activa, los criterios de descarte: producción por debajo del 70 % del promedio del rodeo, tres o más servicios desde el último parto sin preñez, más de 150 días abiertos, tres o más diagnósticos en el último año, y siete o más partos.

3. El sistema selecciona las hembras que cumplen al menos un criterio.

4. El sistema despliega el listado ordenado por cantidad de motivos, detallando en cada caso cuáles son.

Cursos Alternativos: 4a. El usuario selecciona un animal: el sistema abre su ficha integral (CU11).

Cursos de Excepción: 2a. No hay animales que cumplan ningún criterio: el sistema lo informa.

Post-condición: El usuario dispone de la lista y de sus motivos sin que se altere ningún registro.

Reglas de Negocio: El sistema informa y no decide: por eso presenta los motivos en texto y no un puntaje único. La decisión de descarte corresponde a quien conoce al animal. Los umbrales son criterios fijos del sistema y no parámetros configurables.

Validaciones: —

Frecuencia de Uso: Baja, en las instancias de evaluación del rodeo.

### CU 43 — Buscar Animal por Caravana

Nombre del CU: Buscar Animal por Caravana

Actores: Encargada del sector

Tipo: Primario

Descripción: Permite llegar directamente a la ficha de un animal desde cualquier pantalla del sistema, ingresando su número de caravana.

Referencia a Requerimientos Funcionales: RF6.4

Pre-condición: El usuario debe estar logueado en el sistema.

Desencadenante: El usuario necesita consultar un animal mientras está trabajando en otra pantalla.

Curso Básico:

1. El usuario ingresa el número de caravana en el buscador de la barra superior y confirma.

2. El sistema localiza el animal.

3. El sistema abre la ficha integral del animal (CU11).

Cursos Alternativos: —

Cursos de Excepción: 2a. La caravana no corresponde a ningún animal registrado: el sistema informa que no lo encontró y ofrece el listado de animales para buscarlo por otros criterios.

Post-condición: El usuario accede a la ficha del animal sin perder el contexto de trabajo.

Reglas de Negocio: La búsqueda es por caravana exacta: es el identificador con el que se trabaja en el establecimiento.

Validaciones: El número de caravana es obligatorio.

Frecuencia de Uso: Alta, es el atajo de consulta del sistema.


## Modulo 7: Reportes y Notificaciones

### CU 44 — Generar Reporte Productivo

Nombre del CU: Generar Reporte Productivo

Actores: Encargada del sector

Tipo: Primario

Descripción: Permite generar y descargar un reporte en formato PDF o Excel con la producción lechera individual y general del establecimiento para un período determinado.

Referencia a Requerimientos Funcionales: RF7.1

Pre-condición: El usuario debe estar logueado en el sistema.

Desencadenante: El usuario necesita disponer de la información productiva fuera del sistema.

Curso Básico:

1. El usuario ingresa a la sección “Reportes” y selecciona “Reporte Productivo”.

2. El sistema despliega el formulario solicitando el rango de fechas, el alcance del reporte (general o por animal) y el formato de salida (PDF o Excel).

3. El usuario completa los parámetros y presiona “Generar”.

4. El sistema recupera la producción del período aplicando la regla de consolidación por turno.

5. El sistema construye el documento en el formato solicitado y lo ofrece para su descarga.

Cursos Alternativos: —

Cursos de Excepción: 4a. No existen registros de producción en el período seleccionado: el sistema informa la situación y no genera el archivo.

Post-condición: El archivo queda descargado en el dispositivo del usuario sin que se modifique ningún registro del sistema.

Reglas de Negocio: El reporte aplica la misma regla que el historial de producción: cada turno aporta su registro por lote, o la suma de sus controles individuales cuando no tiene registro por lote. Las dos fuentes nunca se suman dentro de un mismo turno.

Validaciones: El rango de fechas y el formato de salida son obligatorios.

Frecuencia de Uso: Media, habitualmente al cierre de cada período.

### CU 45 — Generar Reporte Sanitario

Nombre del CU: Generar Reporte Sanitario

Actores: Encargada del sector

Tipo: Primario

Descripción: Permite generar y descargar un reporte con los diagnósticos, tratamientos y vacunaciones registrados en un período determinado.

Referencia a Requerimientos Funcionales: RF7.2

Pre-condición: El usuario debe estar logueado en el sistema.

Desencadenante: El usuario necesita documentar la sanidad del rodeo fuera del sistema.

Curso Básico:

1. El usuario ingresa a la sección “Reportes” y selecciona “Reporte Sanitario”.

2. El sistema despliega el formulario solicitando el rango de fechas, el alcance (rodeo completo o un animal) y el formato de salida.

3. El usuario completa los parámetros y presiona “Generar”.

4. El sistema recupera los diagnósticos, tratamientos y vacunaciones del período.

5. El sistema construye el documento en el formato solicitado y lo ofrece para su descarga.

Cursos Alternativos: —

Cursos de Excepción: 4a. No existen registros sanitarios en el período seleccionado: el sistema informa la situación y no genera el archivo.

Post-condición: El archivo queda descargado sin que se modifique ningún registro del sistema.

Reglas de Negocio: El reporte incluye el período de descarte de leche resultante de cada tratamiento, que es el dato de interés sanitario para la remisión de leche.

Validaciones: El rango de fechas y el formato de salida son obligatorios.

Frecuencia de Uso: Baja/Media.

### CU 46 — Generar Reporte Reproductivo

Nombre del CU: Generar Reporte Reproductivo

Actores: Encargada del sector

Tipo: Primario

Descripción: Permite generar y descargar un reporte con los servicios, preñeces, partos y secados de un período determinado.

Referencia a Requerimientos Funcionales: RF7.3

Pre-condición: El usuario debe estar logueado en el sistema.

Desencadenante: El usuario necesita analizar la marcha reproductiva del rodeo fuera del sistema.

Curso Básico:

1. El usuario ingresa a la sección “Reportes” y selecciona “Reporte Reproductivo”.

2. El sistema despliega el formulario solicitando el rango de fechas, el alcance y el formato de salida.

3. El usuario completa los parámetros y presiona “Generar”.

4. El sistema recupera los servicios, tactos, partos y secados del período.

5. El sistema construye el documento en el formato solicitado y lo ofrece para su descarga.

Cursos Alternativos: —

Cursos de Excepción: 4a. No existen registros reproductivos en el período seleccionado: el sistema informa la situación y no genera el archivo.

Post-condición: El archivo queda descargado sin que se modifique ningún registro del sistema.

Reglas de Negocio: El reporte acompaña cada evento con los indicadores reproductivos del período, calculados con las mismas reglas que CU41.

Validaciones: El rango de fechas y el formato de salida son obligatorios.

Frecuencia de Uso: Baja/Media.

### CU 47 — Generar Reporte Genético

Nombre del CU: Generar Reporte Genético

Actores: Encargada del sector

Tipo: Primario

Descripción: Permite generar y descargar un reporte de genealogía y de rendimiento por línea genética.

Referencia a Requerimientos Funcionales: RF7.4

Pre-condición: El usuario debe estar logueado y los animales deben tener genealogía registrada.

Desencadenante: El usuario necesita evaluar el aporte de cada línea genética del rodeo.

Curso Básico:

1. El usuario ingresa a la sección “Reportes” y selecciona “Reporte Genético”.

2. El sistema despliega el formulario solicitando el alcance —un animal, un reproductor o el rodeo completo— y el formato de salida.

3. El usuario completa los parámetros y presiona “Generar”.

4. El sistema recupera la genealogía y la producción de los animales alcanzados.

5. El sistema construye el documento en el formato solicitado y lo ofrece para su descarga.

Cursos Alternativos: 2a. El alcance es un reproductor: el sistema reúne su descendencia y el desempeño productivo de sus hijas.

Cursos de Excepción: 4a. Los animales alcanzados no tienen genealogía registrada: el sistema informa la situación y no genera el archivo.

Post-condición: El archivo queda descargado sin que se modifique ningún registro del sistema.

Reglas de Negocio: El rendimiento por línea genética sólo considera hijas con al menos una lactancia registrada: una línea sin producción medida no admite comparación.

Validaciones: El alcance y el formato de salida son obligatorios.

Frecuencia de Uso: Baja.

### CU 48 — Configurar Integración con Bot de Telegram

Nombre del CU: Configurar Integración con Bot de Telegram

Actores: Encargada del sector

Tipo: Primario

Descripción: Permite vincular el sistema con un bot de Telegram y elegir qué avisos automáticos se desean recibir.

Referencia a Requerimientos Funcionales: RF7.5, RF7.6

Pre-condición: El usuario debe estar logueado y debe disponer de una cuenta de Telegram.

Desencadenante: El usuario desea recibir los avisos del sistema en su teléfono.

Curso Básico:

1. El usuario ingresa a la sección de configuración de notificaciones.

2. El sistema despliega las instrucciones para vincular la cuenta con el bot.

3. El usuario completa la vinculación desde Telegram.

4. El sistema confirma el vínculo y despliega los tipos de aviso disponibles: procedimientos sanitarios pendientes, partos próximos, tactos pendientes, secados próximos, stock crítico, vencimiento de insumos y fin del período de descarte.

5. El usuario selecciona los avisos que desea recibir y presiona “Guardar”.

6. El sistema almacena las preferencias y envía un mensaje de prueba.

Cursos Alternativos: 5a. El usuario desactiva un tipo de aviso: el sistema deja de enviarlo, sin afectar su visualización dentro del sistema.

Cursos de Excepción: 3a. La vinculación no se completa: el sistema informa que no pudo establecerse y conserva la configuración anterior. 6a. El envío de prueba falla: el sistema informa la situación e indica revisar la vinculación.

Post-condición: El sistema queda habilitado para enviar los avisos seleccionados al destinatario vinculado.

Reglas de Negocio: Los avisos enviados son los mismos que el sistema muestra en pantalla: la notificación es un canal de entrega, no una fuente de información distinta. La falla del envío no interrumpe la operación del sistema.

Validaciones: La vinculación con el bot es obligatoria antes de seleccionar avisos.

Frecuencia de Uso: Muy baja, una vez al poner el sistema en marcha.

### CU 49 — Enviar Resumen Diario de Tareas Pendientes

Nombre del CU: Enviar Resumen Diario de Tareas Pendientes

Actores: Sistema (actor principal); Encargada del sector (destinataria)

Tipo: Primario

Descripción: El sistema envía, mediante un proceso programado, un resumen diario con las tareas pendientes del establecimiento.

Referencia a Requerimientos Funcionales: RF7.7

Pre-condición: La integración con el bot de Telegram debe estar configurada y activa.

Desencadenante: Se alcanza la hora programada para el envío del resumen.

Curso Básico:

1. El proceso programado se ejecuta a la hora configurada.

2. El sistema reúne las tareas pendientes del día: procedimientos sanitarios vencidos y por vencer, partos y secados próximos, tactos pendientes, hembras para servir, insumos en stock crítico y partidas próximas a vencer.

3. El sistema arma el mensaje agrupando las tareas por módulo.

4. El sistema envía el mensaje al destinatario vinculado.

5. El sistema registra el envío.

Cursos Alternativos: 2a. No hay tareas pendientes: el sistema envía igualmente el resumen indicando que no hay pendientes, de modo que el silencio no se confunda con una falla del envío.

Cursos de Excepción: 4a. El envío falla: el sistema registra el error y reintenta en el siguiente ciclo, sin interrumpir su funcionamiento.

Post-condición: La encargada recibe el resumen del día en su teléfono, sin necesidad de ingresar al sistema.

Reglas de Negocio: El resumen se construye con los mismos cálculos que alimentan el tablero de inicio (CU40) y el calendario sanitario (CU31), de modo que las tres vistas no puedan discrepar.

Validaciones: —

Frecuencia de Uso: Diaria, de forma automática.
