# -*- coding: utf-8 -*-
"""Casos de uso 27 a 49 — Modulos 4 a 7. Ver casos_de_uso_parte1.py para el resto."""

CASOS = [

# ----------------------------------------------------------------- Modulo 4

dict(
    modulo=4, num=27,
    nombre='Registrar Diagnóstico o Revisación',
    actores='Encargada del sector',
    tipo='Primario',
    descripcion='Permite registrar el hallazgo de una patología o el resultado de una '
        'revisación clínica en un animal, dejando asentado el antecedente para su '
        'posterior seguimiento o tratamiento.',
    rf='RF4.1',
    precondicion='El usuario debe estar logueado y el animal debe existir en el sistema.',
    desencadenante='Un animal es revisado o se le detecta una enfermedad en el campo.',
    curso=[
        'El usuario busca al animal por su número de caravana.',
        'El usuario selecciona la opción “Registrar Diagnóstico”.',
        'El usuario ingresa la patología detectada o las notas de la revisación y la fecha '
        'del hallazgo.',
        'El usuario presiona “Guardar”.',
        'El sistema almacena el diagnóstico en el historial del animal como cuadro activo '
        'y confirma el éxito.',
    ],
    alternativos='—',
    excepcion='3a. La fecha del diagnóstico es futura: el sistema muestra el error y no '
        'guarda.',
    postcondicion='El diagnóstico queda registrado en la ficha del animal, habilitándolo '
        'para la posterior asignación de tratamientos.',
    reglas='Un diagnóstico permanece activo hasta que se lo cierra explícitamente (CU33). '
        'Un mismo animal puede tener más de un cuadro activo a la vez.',
    validaciones='La patología y la fecha son obligatorias.',
    frecuencia='Alta, cada vez que se detectan anomalías sanitarias.',
),

dict(
    modulo=4, num=28,
    nombre='Registrar Tratamiento Sanitario',
    actores='Encargada del sector',
    tipo='Primario',
    descripcion='Permite registrar el tratamiento aplicado a un animal, indicando el '
        'insumo, la cantidad utilizada y los días de duración, y calculando el período de '
        'descarte de leche resultante.',
    rf='RF4.2, RF4.3, RF5.3',
    precondicion='El usuario debe estar logueado y el animal debe existir en el sistema.',
    desencadenante='Se define el protocolo médico para combatir la patología del animal, o '
        'se aplica un tratamiento preventivo.',
    curso=[
        'El usuario busca la caravana del animal.',
        'El usuario selecciona la opción “Registrar Tratamiento” e indica el diagnóstico '
        'que lo motiva, si corresponde.',
        'El usuario elige el insumo desde el stock, ingresa la cantidad a aplicar, la dosis '
        'y los días de duración.',
        'El usuario presiona “Guardar”.',
        'El sistema verifica que exista stock suficiente del insumo.',
        'El sistema descuenta del stock la cantidad indicada, imputándola a la partida que '
        'vence primero.',
        'El sistema calcula la fecha de fin del período de descarte sumando los días de '
        'duración y el período de carencia del insumo, y la propone al usuario.',
        'El sistema guarda el tratamiento y confirma la operación.',
    ],
    alternativos='2a. El tratamiento es preventivo, como una desparasitación, y no proviene '
        'de un diagnóstico: el usuario lo registra directamente sobre el animal y el '
        'sistema deja el diagnóstico sin asociar. 2b. El tratamiento cumple un plan '
        'sanitario configurado: el usuario lo selecciona y el sistema lo registra como '
        'aplicación de ese plan. 7a. El usuario ajusta la fecha de fin de descarte '
        'propuesta: el sistema conserva el valor ingresado.',
    excepcion='5a. El stock disponible del insumo es menor a la cantidad indicada: el '
        'sistema muestra una alerta de stock insuficiente e impide el guardado. 3a. La '
        'fecha de inicio es futura o posterior a la baja del animal: el sistema muestra el '
        'error y no guarda.',
    postcondicion='El tratamiento queda registrado, el stock del insumo refleja el consumo '
        'y el animal queda excluido del lote de ordeñe mientras el descarte esté vigente.',
    reglas='La cantidad de insumo a descontar se ingresa y no se calcula: depende del peso '
        'del animal y de la presentación del producto, datos que el sistema no administra. '
        'El tratamiento se registra sobre el animal, con el diagnóstico como dato opcional, '
        'de modo que el tratamiento preventivo no obligue a inventar un diagnóstico. La '
        'fecha de fin de descarte se calcula automáticamente pero admite ajuste manual.',
    validaciones='El animal, el insumo, la cantidad y los días de duración son '
        'obligatorios. La cantidad y los días deben ser positivos.',
    frecuencia='Media.',
),

dict(
    modulo=4, num=29,
    nombre='Registrar Vacunación',
    actores='Encargada del sector',
    tipo='Primario',
    descripcion='Permite asentar la aplicación de una vacuna a un animal, indicando la '
        'fecha y el biológico utilizado.',
    rf='RF4.4',
    precondicion='El usuario debe estar logueado en el sistema.',
    desencadenante='Se realiza una jornada de vacunación obligatoria o preventiva.',
    curso=[
        'El usuario busca al animal por su número de caravana.',
        'El usuario selecciona la opción “Registrar Vacunación”.',
        'El usuario selecciona la vacuna aplicada y la fecha de ejecución.',
        'El usuario indica, si corresponde, el plan sanitario que la aplicación da por '
        'cumplido.',
        'El usuario presiona “Guardar”.',
        'El sistema descuenta la unidad de vacuna del stock, guarda el registro y confirma '
        'la operación.',
    ],
    alternativos='4a. La vacunación se aplica fuera de todo plan: el usuario no selecciona '
        'plan y el sistema registra la aplicación sin vincularla.',
    excepcion='6a. No hay stock disponible de la vacuna: el sistema informa la falta e '
        'impide el guardado. 3a. La fecha es futura o posterior a la baja del animal: el '
        'sistema muestra el error y no guarda.',
    postcondicion='El registro queda guardado en la ficha sanitaria cronológica del animal '
        'y el calendario sanitario lo considera como aplicación cumplida del plan indicado.',
    reglas='La aplicación declara explícitamente qué plan sanitario cumple, de modo que el '
        'calendario no deba inferirlo a partir del insumo utilizado. Una aplicación sólo '
        'puede cumplir un plan de su propio tipo.',
    validaciones='La selección de la vacuna y la fecha de aplicación son obligatorias.',
    frecuencia='Baja/Media, sujeta al plan sanitario estacional.',
),

dict(
    modulo=4, num=30,
    nombre='Configurar Plan Sanitario',
    actores='Encargada del sector',
    tipo='Primario',
    descripcion='El usuario define las reglas de los procedimientos sanitarios periódicos '
        '—vacunaciones, desparasitaciones y descornes— que el establecimiento debe cumplir, '
        'y que el sistema utiliza para calcular el calendario sanitario.',
    rf='RF4.7',
    precondicion='Los insumos y las categorías deben estar registrados en el sistema.',
    desencadenante='El usuario necesita dar de alta o ajustar un esquema sanitario del '
        'establecimiento.',
    curso=[
        'El usuario ingresa a la sección “Planes Sanitarios”.',
        'El sistema despliega los planes ya configurados junto con su estado.',
        'El usuario selecciona “Nuevo Plan” o elige un plan existente para modificarlo.',
        'El sistema despliega el formulario solicitando: nombre, tipo de procedimiento '
        '(vacunación, desparasitación o descorne), insumo a aplicar, periodicidad en días, '
        'edad de inicio en meses y categorías alcanzadas.',
        'El usuario completa los datos y presiona “Guardar”.',
        'El sistema valida la consistencia de los parámetros y almacena el plan junto con '
        'las categorías asociadas.',
        'El sistema confirma la operación e informa que el calendario sanitario se '
        'recalculará con la nueva regla.',
    ],
    alternativos='4a. El usuario no selecciona ninguna categoría: el plan queda alcanzando '
        'a todo el rodeo. 4b. El usuario deja la periodicidad vacía: el sistema interpreta '
        'que el procedimiento se aplica una única vez en la vida del animal. 4c. El tipo de '
        'procedimiento es descorne: el sistema no solicita insumo. 3a. El usuario desactiva '
        'un plan: el sistema deja de generar procedimientos pendientes a partir de él, sin '
        'borrar las aplicaciones ya registradas.',
    excepcion='6a. Ya existe un plan con el mismo nombre: el sistema informa la duplicación '
        'y no guarda. 6b. La periodicidad o la edad de inicio no son valores positivos: el '
        'sistema muestra el error y no guarda.',
    postcondicion='El plan queda registrado y pasa a generar procedimientos pendientes en '
        'el calendario sanitario.',
    reglas='Sólo los planes activos generan procedimientos pendientes. Un plan sin '
        'categorías asociadas alcanza a todo el rodeo. Un plan sin periodicidad se '
        'considera cumplido para el animal una vez que se registró la aplicación. La misma '
        'pantalla crea y modifica los planes.',
    validaciones='El nombre, el tipo de procedimiento y la edad de inicio son obligatorios. '
        'El insumo es obligatorio salvo que el tipo de procedimiento sea descorne. El '
        'nombre del plan es único.',
    frecuencia='Baja, únicamente cuando se define o se ajusta un esquema sanitario.',
),

dict(
    modulo=4, num=31,
    nombre='Consultar Calendario Sanitario',
    actores='Encargada del sector',
    tipo='Primario',
    descripcion='El usuario consulta el cronograma de procedimientos sanitarios pendientes '
        'y vencidos del rodeo, derivado de los planes sanitarios configurados.',
    rf='RF4.5',
    precondicion='Deben existir planes sanitarios activos y animales registrados en el '
        'rodeo.',
    desencadenante='El usuario necesita saber qué procedimientos sanitarios están '
        'pendientes.',
    curso=[
        'El usuario accede a la sección “Calendario Sanitario”.',
        'El sistema recupera los planes sanitarios activos.',
        'Para cada plan, el sistema selecciona los animales activos que pertenecen a las '
        'categorías alcanzadas y que superan la edad de inicio.',
        'El sistema busca, para cada animal, la última aplicación registrada de ese plan.',
        'El sistema proyecta la fecha del próximo procedimiento sumando la periodicidad a '
        'la última aplicación, o toma la fecha en que el animal alcanzó la edad de inicio '
        'cuando nunca se aplicó.',
        'El sistema descarta los planes sin periodicidad que ya fueron aplicados al animal.',
        'El sistema despliega el cronograma ordenado por fecha, distinguiendo los '
        'procedimientos vencidos de los próximos a vencer dentro de la anticipación '
        'configurada.',
    ],
    alternativos='7a. El usuario filtra por tipo de procedimiento o por categoría: el '
        'sistema restringe el cronograma a los pendientes que cumplen el filtro.',
    excepcion='2a. No existen planes sanitarios activos: el sistema informa que no hay '
        'procedimientos programados y sugiere configurar un plan.',
    postcondicion='El usuario conoce los procedimientos sanitarios vencidos y próximos a '
        'vencer del rodeo.',
    reglas='El pendiente resulta de la diferencia entre lo que el plan exige y lo que '
        'efectivamente se aplicó. El cálculo es el mismo que alimentará el resumen diario '
        'enviado por Telegram, de modo que ambas vistas no puedan discrepar. Los animales '
        'inactivos no se consideran.',
    validaciones='El horizonte de anticipación debe ser un número positivo de días.',
    frecuencia='Alta, es la consulta de planificación sanitaria del establecimiento.',
),

dict(
    modulo=4, num=32,
    nombre='Registrar Procedimiento de Descorne',
    actores='Encargada del sector',
    tipo='Primario',
    descripcion='Permite registrar los procedimientos de descorne efectuados en los '
        'animales, para el control del bienestar y el manejo del rodeo.',
    rf='RF4.6',
    precondicion='El usuario debe estar logueado y el animal debe estar registrado.',
    desencadenante='Se realiza el descorne físico de un animal.',
    curso=[
        'El usuario busca la caravana del animal.',
        'El usuario selecciona la opción “Registrar Descorne”.',
        'El usuario ingresa la fecha del procedimiento y el método utilizado.',
        'El usuario indica, si corresponde, el plan sanitario que la aplicación da por '
        'cumplido.',
        'El usuario presiona “Confirmar”.',
        'El sistema graba la información en el expediente del animal y confirma la '
        'operación.',
    ],
    alternativos='—',
    excepcion='5a. El animal ya tiene un descorne registrado: el sistema informa la '
        'situación e impide el registro. 3a. La fecha es futura o posterior a la baja del '
        'animal: el sistema muestra el error y no guarda.',
    postcondicion='La intervención queda registrada en la ficha del animal y el calendario '
        'sanitario deja de exigirla.',
    reglas='El descorne es un procedimiento de umbral de edad y de aplicación única: un '
        'animal se descorna una sola vez, y un plan sanitario de descorne deja de exigirlo '
        'una vez registrado.',
    validaciones='La fecha es obligatoria y no puede ser futura.',
    frecuencia='Baja/Media, según la cantidad de terneros del rodeo.',
),

dict(
    modulo=4, num=33,
    nombre='Cerrar Diagnóstico',
    actores='Encargada del sector',
    tipo='Primario',
    descripcion='Permite dar por resuelto un cuadro sanitario, de modo que el sistema '
        'distinga los diagnósticos activos de los ya cerrados.',
    rf='RF4.8',
    precondicion='El usuario debe estar logueado y el diagnóstico debe estar activo.',
    desencadenante='El animal se recupera del cuadro registrado.',
    curso=[
        'El usuario localiza el diagnóstico en el listado de diagnósticos o en la ficha del '
        'animal.',
        'El usuario selecciona la opción de cerrarlo y confirma.',
        'El sistema registra la resolución del diagnóstico y confirma la operación.',
    ],
    alternativos='—',
    excepcion='—',
    postcondicion='El diagnóstico deja de figurar entre los cuadros activos del animal, '
        'conservándose en su historial sanitario junto con sus tratamientos.',
    reglas='Cerrar el diagnóstico no cierra los tratamientos asociados ni modifica el '
        'período de descarte de leche, que depende de la fecha de fin del tratamiento.',
    validaciones='—',
    frecuencia='Media, al resolverse cada cuadro sanitario.',
),

dict(
    modulo=4, num=34,
    nombre='Corregir o Eliminar Evento Sanitario',
    actores='Encargada del sector',
    tipo='Primario',
    descripcion='Permite corregir o eliminar un diagnóstico, un tratamiento, una vacunación '
        'o un descorne ya registrado, devolviendo al stock los insumos que se habían '
        'descontado.',
    rf='RF4.9, RF5.10',
    precondicion='El usuario debe estar logueado y el evento a corregir debe existir.',
    desencadenante='El usuario advierte un error en un evento sanitario ya guardado.',
    curso=[
        'El usuario localiza el evento en el listado del módulo o en la ficha del animal.',
        'El usuario selecciona “Editar” o “Eliminar”.',
        'Al editar, el sistema despliega la pantalla de registro con los datos cargados y '
        'aplica las mismas validaciones.',
        'Al eliminar, el sistema verifica que ningún otro registro dependa de éste.',
        'El sistema devuelve al stock los insumos consumidos por el evento, mediante un '
        'contra-movimiento.',
        'El sistema aplica el cambio y confirma la operación.',
    ],
    alternativos='3a. La corrección cambia la cantidad de insumo aplicada: el sistema '
        'ajusta el stock por la diferencia. 3b. La corrección cambia los días de duración '
        'o el insumo del tratamiento: el sistema recalcula la fecha de fin del período de '
        'descarte.',
    excepcion='4a. El diagnóstico tiene tratamientos registrados: el sistema informa cuáles '
        'son y que deben eliminarse primero, sin borrar nada.',
    postcondicion='El evento queda corregido o eliminado, el stock refleja la devolución y '
        'el período de descarte del animal es coherente con los tratamientos que '
        'permanecen.',
    reglas='El movimiento de stock original nunca se borra: la devolución se registra como '
        'un movimiento nuevo de signo contrario, de modo que el historial conserve lo que '
        'efectivamente ocurrió.',
    validaciones='Las mismas del registro original.',
    frecuencia='Baja, ante errores de carga.',
),

# ----------------------------------------------------------------- Modulo 5

dict(
    modulo=5, num=35,
    nombre='Registrar Alta e Ingreso de Insumo',
    actores='Encargada del sector',
    tipo='Primario',
    descripcion='Permite dar de alta un nuevo insumo —medicamento, vacuna, antiparasitario '
        'o pajuela— o registrar el ingreso de una nueva partida de uno existente.',
    rf='RF5.1, RF5.2, RF5.7',
    precondicion='El usuario debe estar logueado en el sistema.',
    desencadenante='El establecimiento adquiere insumos o necesita inventariar el stock '
        'disponible.',
    curso=[
        'El usuario accede a la sección de Insumos y selecciona el alta o el ingreso de '
        'stock.',
        'El usuario selecciona el tipo de insumo: medicamento, vacuna, antiparasitario o '
        'pajuela.',
        'El usuario ingresa los datos identificatorios del insumo, la cantidad que ingresa '
        'y la fecha de vencimiento de la partida.',
        'El usuario presiona “Guardar”.',
        'El sistema valida la consistencia de los datos.',
        'El sistema registra el movimiento de ingreso con su fecha de vencimiento, '
        'actualiza el stock disponible y confirma la operación.',
    ],
    alternativos='2a. El insumo es una pajuela: el sistema solicita además el toro que la '
        'aporta, de modo de conservar el vínculo genético. 2b. El insumo obliga a descartar '
        'leche: el sistema solicita el período de carencia en días. 1a. El insumo ya '
        'existe: el usuario registra únicamente el ingreso de la nueva partida.',
    excepcion='5a. La cantidad ingresada es menor o igual a cero: el sistema muestra un '
        'mensaje de error e impide el guardado. 5b. Ya existe un insumo con el mismo nombre '
        'y tipo: el sistema informa la duplicación y no da de alta uno nuevo.',
    postcondicion='El stock disponible del insumo se actualiza de forma inmediata y la '
        'partida queda registrada con su vencimiento.',
    reglas='El alta de un insumo con cantidad inicial registra esa cantidad como un '
        'movimiento de ingreso, de modo que el stock siempre sea la suma de sus '
        'movimientos y no un número guardado aparte. El alta y el ingreso se resuelven en '
        'una única transacción: si el movimiento no se registra, el insumo tampoco.',
    validaciones='El tipo, el nombre y la cantidad son obligatorios. La cantidad debe ser '
        'positiva. La fecha de vencimiento, cuando corresponde, no puede ser anterior a la '
        'fecha de ingreso.',
    frecuencia='Baja/Media, sujeta a la frecuencia de compras o auditorías de stock.',
),

dict(
    modulo=5, num=36,
    nombre='Configurar Umbral de Stock Mínimo',
    actores='Encargada del sector',
    tipo='Primario',
    descripcion='Permite definir la cantidad mínima de reserva de cada insumo, con el fin '
        'de activar alertas antes de que ocurra un desabastecimiento.',
    rf='RF5.5',
    precondicion='El usuario debe estar logueado y el insumo debe estar dado de alta.',
    desencadenante='El usuario desea establecer o ajustar la barrera de seguridad de stock '
        'de un insumo.',
    curso=[
        'El usuario busca el insumo en el listado del sistema.',
        'El usuario selecciona la opción “Configurar Stock Mínimo”.',
        'El usuario ingresa el valor que representará el umbral de alerta.',
        'El usuario presiona “Guardar”.',
        'El sistema almacena el umbral asignado a ese insumo y confirma la operación.',
    ],
    alternativos='—',
    excepcion='3a. El valor ingresado es negativo: el sistema emite una alerta de error e '
        'impide el almacenamiento.',
    postcondicion='Queda fijado el límite crítico de existencias para el insumo '
        'seleccionado.',
    reglas='—',
    validaciones='El stock mínimo es obligatorio y debe ser mayor o igual a cero.',
    frecuencia='Baja, por lo general se configura una única vez por insumo.',
),

dict(
    modulo=5, num=37,
    nombre='Consultar Alertas de Stock Crítico',
    actores='Encargada del sector',
    tipo='Primario',
    descripcion='Despliega los insumos cuyas existencias son iguales o inferiores al umbral '
        'mínimo configurado.',
    rf='RF5.6',
    precondicion='El usuario debe estar logueado en el sistema.',
    desencadenante='El usuario accede al panel de inventario para planificar futuras '
        'compras.',
    curso=[
        'El usuario ingresa a la sección “Alertas de Stock”.',
        'El sistema calcula el stock disponible de cada insumo.',
        'El sistema selecciona aquellos cuyo stock actual es menor o igual al stock mínimo '
        'configurado.',
        'El sistema muestra el listado con la cantidad disponible y el umbral de cada '
        'insumo.',
    ],
    alternativos='—',
    excepcion='—',
    postcondicion='El sistema expone el estado de alerta sin alterar ningún registro.',
    reglas='La condición de stock crítico se evalúa sobre el stock vigente, que resulta de '
        'los movimientos registrados: los ingresos de compra, los egresos automáticos al '
        'registrarse un tratamiento (CU28), una vacunación (CU29) o una inseminación '
        '(CU21), y las devoluciones por corrección.',
    validaciones='—',
    frecuencia='Alta, revisada de manera frecuente para evitar quiebres de stock.',
),

dict(
    modulo=5, num=38,
    nombre='Consultar Alertas de Vencimiento de Insumos',
    actores='Encargada del sector',
    tipo='Primario',
    descripcion='Despliega las partidas de insumos vencidas o próximas a vencer, para '
        'retirarlas de uso o priorizar su aplicación.',
    rf='RF5.8',
    precondicion='El usuario debe estar logueado en el sistema.',
    desencadenante='El usuario revisa el estado de los insumos almacenados.',
    curso=[
        'El usuario ingresa a la sección “Alertas de Vencimiento”.',
        'El sistema recorre las partidas ingresadas de cada insumo y calcula el remanente '
        'de cada una.',
        'El sistema identifica las partidas ya vencidas y aquellas cuyo vencimiento se '
        'produce dentro de la anticipación configurada.',
        'El sistema despliega el listado ordenado por fecha de vencimiento, indicando el '
        'insumo, la partida, la cantidad remanente y los días restantes.',
    ],
    alternativos='—',
    excepcion='—',
    postcondicion='Se exponen las alertas de vencimiento sin alterar ningún registro.',
    reglas='El vencimiento se registra a nivel de cada ingreso, por lo que un mismo insumo '
        'puede tener varias partidas con vencimientos distintos. El consumo se imputa a la '
        'partida que vence primero, de modo que el remanente de cada partida se calcula '
        'descontando los egresos en ese orden. Las partidas agotadas no figuran.',
    validaciones='—',
    frecuencia='Media, revisada de forma periódica y antes de cada jornada sanitaria.',
),

dict(
    modulo=5, num=39,
    nombre='Consultar Historial de Movimientos de Stock',
    actores='Encargada del sector',
    tipo='Primario',
    descripcion='Permite consultar cronológicamente todos los ingresos y egresos de stock '
        'registrados sobre los insumos del establecimiento.',
    rf='RF5.9, RF5.10',
    precondicion='El usuario debe estar logueado en el sistema.',
    desencadenante='El usuario necesita auditar el consumo o el reabastecimiento de un '
        'insumo.',
    curso=[
        'El usuario ingresa a la sección “Historial de Movimientos”.',
        'El sistema despliega los filtros disponibles: insumo, tipo de movimiento y rango '
        'de fechas.',
        'El usuario selecciona los criterios deseados y presiona “Buscar”.',
        'El sistema recupera los movimientos que coinciden con los filtros aplicados.',
        'El sistema despliega el listado paginado detallando fecha, tipo de movimiento, '
        'cantidad, motivo y stock resultante.',
    ],
    alternativos='—',
    excepcion='4a. No existen movimientos que coincidan con los criterios: el sistema '
        'muestra un mensaje indicando que no se encontraron resultados.',
    postcondicion='El sistema expone la información histórica sin alterar los registros de '
        'la base de datos.',
    reglas='Todo egreso automático generado por el sistema queda asentado como un '
        'movimiento más del historial, indicando la operación que lo originó. Las '
        'devoluciones por corrección figuran como movimientos propios: el movimiento '
        'original nunca se borra. El stock resultante que se muestra en cada fila se '
        'reconstruye hacia atrás desde el stock actual, de modo que refleje la situación '
        'en el momento de cada movimiento.',
    validaciones='El rango de fechas debe ser válido, con fecha de inicio anterior o igual '
        'a la fecha de fin.',
    frecuencia='Media, ante auditorías de inventario o control de consumos.',
),

# ----------------------------------------------------------------- Modulo 6

dict(
    modulo=6, num=40,
    nombre='Consultar Tablero de Inicio',
    actores='Encargada del sector',
    tipo='Primario',
    descripcion='Presenta, como pantalla de entrada al sistema, el estado del día del '
        'establecimiento: lo que hay pendiente y lo que vence.',
    rf='RF6.1',
    precondicion='El usuario debe estar logueado en el sistema.',
    desencadenante='El usuario inicia sesión o vuelve a la pantalla principal.',
    curso=[
        'El sistema recupera los avisos vigentes de cada módulo: secados próximos, partos '
        'próximos, tactos pendientes, hembras para servir, procedimientos sanitarios '
        'vencidos y por vencer, insumos en stock crítico y partidas próximas a vencer.',
        'El sistema recupera la composición del rodeo —animales activos, hembras en '
        'lactancia, secas, preñadas y vacías— junto con los litros por vaca y por día y '
        'los días en leche promedio.',
        'El sistema despliega cada grupo de avisos con su cantidad y el acceso directo a la '
        'pantalla que lo resuelve.',
        'El sistema ofrece, sobre los avisos, el registro rápido de los eventos '
        'reproductivos que el establecimiento carga a diario —celo, servicio, tacto y '
        'parto—, con las caravanas que ya figuran en la lista de trabajo de cada uno como '
        'acceso directo al campo.',
        'El usuario elige el evento e indica la caravana: el celo y el tacto quedan '
        'registrados en el propio tablero, con el curso y las reglas de CU20 y CU22.',
    ],
    alternativos='3a. No hay avisos pendientes en un grupo: el sistema lo informa en lugar '
        'de mostrar una lista vacía. 1a. Todavía no hay ningún animal cargado: el tablero '
        'reemplaza los avisos por la indicación de por dónde empezar la puesta en marcha. '
        '5a. El evento elegido es un servicio o un parto: el sistema no lo registra en el '
        'tablero y abre CU21 o CU24 con la caravana ya cargada. 5b. El registro rápido se '
        'guarda: el sistema lo confirma en el tablero y deja el campo de caravana vacío y '
        'con el foco, conservando el evento y la fecha para el registro siguiente.',
    excepcion='5c. La caravana no existe o corresponde a un macho: el sistema informa el '
        'motivo sin abandonar el tablero y conserva lo cargado. 5d. Se quiere registrar un '
        'tacto sobre una hembra sin servicio pendiente: el sistema informa que hay que '
        'registrar antes el servicio e impide el registro.',
    postcondicion='El usuario conoce el estado del establecimiento. La consulta no altera '
        'ningún registro; si se usó el registro rápido, el celo o el tacto quedan asentados '
        'con las mismas consecuencias que tienen en su propio caso de uso.',
    reglas='El tablero no calcula nada propio: reúne los mismos avisos que producen los '
        'casos de uso de cada módulo, de modo que no puedan discrepar entre una vista y la '
        'otra. El registro rápido tampoco valida por su cuenta: aplica las mismas reglas de '
        'CU20 y CU22. Sólo se registran en el tablero los eventos que no consumen insumos '
        'ni dan de alta animales; el servicio descuenta una pajuela del stock y el parto da '
        'de alta la cría, así que se completan en su propia pantalla.',
    validaciones='Las de CU20 y CU22 según el evento elegido. La caravana es obligatoria '
        'para registrar, y opcional para abrir el formulario de servicio o de parto.',
    frecuencia='Muy alta, es la pantalla de entrada al sistema y la vía habitual de carga '
        'de los eventos reproductivos diarios.',
),

dict(
    modulo=6, num=41,
    nombre='Consultar Indicadores del Rodeo',
    actores='Encargada del sector',
    tipo='Primario',
    descripcion='Presenta los indicadores de desempeño reproductivo y productivo del rodeo, '
        'que permiten evaluar la marcha del establecimiento más allá del registro diario.',
    rf='RF6.2',
    precondicion='El usuario debe estar logueado y debe existir historial registrado.',
    desencadenante='El usuario necesita evaluar el desempeño del rodeo.',
    curso=[
        'El usuario ingresa a la sección “Indicadores”.',
        'El sistema calcula los indicadores reproductivos: días abiertos promedio, '
        'intervalo entre partos y servicios por preñez.',
        'El sistema calcula los indicadores productivos: litros por vaca y por día, y días '
        'en leche promedio.',
        'El sistema calcula la composición del rodeo por estado productivo y reproductivo.',
        'El sistema arma el ranking de las lactancias en curso por producción diaria, con '
        'la proyección a 305 días de cada una.',
        'El sistema despliega los indicadores junto con la aclaración de cómo se calcula '
        'cada uno.',
    ],
    alternativos='2a. No hay partos registrados suficientes para calcular un indicador: el '
        'sistema informa que no hay datos en lugar de mostrar un valor engañoso.',
    excepcion='—',
    postcondicion='El usuario dispone de los indicadores sin que se altere ningún registro.',
    reglas='Los días abiertos se cuentan del parto a la concepción; mientras la hembra no '
        'queda preñada el número sigue corriendo, y eso también es información. La '
        'proyección a 305 días supone que el animal sostiene su último nivel controlado: '
        'es lineal y por lo tanto optimista, y sirve para comparar animales entre sí, no '
        'como pronóstico.',
    validaciones='—',
    frecuencia='Media, de revisión periódica.',
),

dict(
    modulo=6, num=42,
    nombre='Consultar Candidatas a Descarte',
    actores='Encargada del sector',
    tipo='Primario',
    descripcion='Lista las hembras que cumplen alguno de los criterios de descarte, '
        'indicando por cuál de ellos figura cada una, como apoyo a la decisión de '
        'refugo del rodeo.',
    rf='RF6.3',
    precondicion='El usuario debe estar logueado y debe existir historial registrado.',
    desencadenante='El usuario evalúa qué animales conviene retirar del rodeo.',
    curso=[
        'El usuario ingresa a la sección “Candidatas a Descarte”.',
        'El sistema evalúa, para cada hembra activa, los criterios de descarte: producción '
        'por debajo del 70 % del promedio del rodeo, tres o más servicios desde el último '
        'parto sin preñez, más de 150 días abiertos, tres o más diagnósticos en el último '
        'año, y siete o más partos.',
        'El sistema selecciona las hembras que cumplen al menos un criterio.',
        'El sistema despliega el listado ordenado por cantidad de motivos, detallando en '
        'cada caso cuáles son.',
    ],
    alternativos='4a. El usuario selecciona un animal: el sistema abre su ficha integral '
        '(CU11).',
    excepcion='2a. No hay animales que cumplan ningún criterio: el sistema lo informa.',
    postcondicion='El usuario dispone de la lista y de sus motivos sin que se altere ningún '
        'registro.',
    reglas='El sistema informa y no decide: por eso presenta los motivos en texto y no un '
        'puntaje único. La decisión de descarte corresponde a quien conoce al animal. Los '
        'umbrales son criterios fijos del sistema y no parámetros configurables.',
    validaciones='—',
    frecuencia='Baja, en las instancias de evaluación del rodeo.',
),

dict(
    modulo=6, num=43,
    nombre='Buscar Animal por Caravana',
    actores='Encargada del sector',
    tipo='Primario',
    descripcion='Permite llegar directamente a la ficha de un animal desde cualquier '
        'pantalla del sistema, ingresando su número de caravana.',
    rf='RF6.4',
    precondicion='El usuario debe estar logueado en el sistema.',
    desencadenante='El usuario necesita consultar un animal mientras está trabajando en '
        'otra pantalla.',
    curso=[
        'El usuario ingresa el número de caravana en el buscador de la barra superior y '
        'confirma.',
        'El sistema localiza el animal.',
        'El sistema abre la ficha integral del animal (CU11).',
    ],
    alternativos='—',
    excepcion='2a. La caravana no corresponde a ningún animal registrado: el sistema '
        'informa que no lo encontró y ofrece el listado de animales para buscarlo por otros '
        'criterios.',
    postcondicion='El usuario accede a la ficha del animal sin perder el contexto de '
        'trabajo.',
    reglas='La búsqueda es por caravana exacta: es el identificador con el que se trabaja '
        'en el establecimiento.',
    validaciones='El número de caravana es obligatorio.',
    frecuencia='Alta, es el atajo de consulta del sistema.',
),

# ----------------------------------------------------------------- Modulo 7

dict(
    modulo=7, num=44,
    nombre='Generar Reporte Productivo',
    actores='Encargada del sector',
    tipo='Primario',
    descripcion='Permite generar y descargar un reporte en formato PDF o Excel con la '
        'producción lechera individual y general del establecimiento para un período '
        'determinado.',
    rf='RF7.1',
    precondicion='El usuario debe estar logueado en el sistema.',
    desencadenante='El usuario necesita disponer de la información productiva fuera del '
        'sistema.',
    curso=[
        'El usuario ingresa a la sección “Reportes” y selecciona “Reporte Productivo”.',
        'El sistema despliega el formulario solicitando el rango de fechas, el alcance del '
        'reporte (general o por animal) y el formato de salida (PDF o Excel).',
        'El usuario completa los parámetros y presiona “Generar”.',
        'El sistema recupera la producción del período aplicando la regla de consolidación '
        'por turno.',
        'El sistema construye el documento en el formato solicitado y lo ofrece para su '
        'descarga.',
    ],
    alternativos='—',
    excepcion='4a. No existen registros de producción en el período seleccionado: el '
        'sistema informa la situación y no genera el archivo.',
    postcondicion='El archivo queda descargado en el dispositivo del usuario sin que se '
        'modifique ningún registro del sistema.',
    reglas='El reporte aplica la misma regla que el historial de producción: cada turno '
        'aporta su registro por lote, o la suma de sus controles individuales cuando no '
        'tiene registro por lote. Las dos fuentes nunca se suman dentro de un mismo turno.',
    validaciones='El rango de fechas y el formato de salida son obligatorios.',
    frecuencia='Media, habitualmente al cierre de cada período.',
),

dict(
    modulo=7, num=45,
    nombre='Generar Reporte Sanitario',
    actores='Encargada del sector',
    tipo='Primario',
    descripcion='Permite generar y descargar un reporte con los diagnósticos, tratamientos '
        'y vacunaciones registrados en un período determinado.',
    rf='RF7.2',
    precondicion='El usuario debe estar logueado en el sistema.',
    desencadenante='El usuario necesita documentar la sanidad del rodeo fuera del sistema.',
    curso=[
        'El usuario ingresa a la sección “Reportes” y selecciona “Reporte Sanitario”.',
        'El sistema despliega el formulario solicitando el rango de fechas, el alcance '
        '(rodeo completo o un animal) y el formato de salida.',
        'El usuario completa los parámetros y presiona “Generar”.',
        'El sistema recupera los diagnósticos, tratamientos y vacunaciones del período.',
        'El sistema construye el documento en el formato solicitado y lo ofrece para su '
        'descarga.',
    ],
    alternativos='—',
    excepcion='4a. No existen registros sanitarios en el período seleccionado: el sistema '
        'informa la situación y no genera el archivo.',
    postcondicion='El archivo queda descargado sin que se modifique ningún registro del '
        'sistema.',
    reglas='El reporte incluye el período de descarte de leche resultante de cada '
        'tratamiento, que es el dato de interés sanitario para la remisión de leche.',
    validaciones='El rango de fechas y el formato de salida son obligatorios.',
    frecuencia='Baja/Media.',
),

dict(
    modulo=7, num=46,
    nombre='Generar Reporte Reproductivo',
    actores='Encargada del sector',
    tipo='Primario',
    descripcion='Permite generar y descargar un reporte con los servicios, preñeces, partos '
        'y secados de un período determinado.',
    rf='RF7.3',
    precondicion='El usuario debe estar logueado en el sistema.',
    desencadenante='El usuario necesita analizar la marcha reproductiva del rodeo fuera del '
        'sistema.',
    curso=[
        'El usuario ingresa a la sección “Reportes” y selecciona “Reporte Reproductivo”.',
        'El sistema despliega el formulario solicitando el rango de fechas, el alcance y el '
        'formato de salida.',
        'El usuario completa los parámetros y presiona “Generar”.',
        'El sistema recupera los servicios, tactos, partos y secados del período.',
        'El sistema construye el documento en el formato solicitado y lo ofrece para su '
        'descarga.',
    ],
    alternativos='—',
    excepcion='4a. No existen registros reproductivos en el período seleccionado: el '
        'sistema informa la situación y no genera el archivo.',
    postcondicion='El archivo queda descargado sin que se modifique ningún registro del '
        'sistema.',
    reglas='El reporte acompaña cada evento con los indicadores reproductivos del período, '
        'calculados con las mismas reglas que CU41.',
    validaciones='El rango de fechas y el formato de salida son obligatorios.',
    frecuencia='Baja/Media.',
),

dict(
    modulo=7, num=47,
    nombre='Generar Reporte Genético',
    actores='Encargada del sector',
    tipo='Primario',
    descripcion='Permite generar y descargar un reporte de genealogía y de rendimiento por '
        'línea genética.',
    rf='RF7.4',
    precondicion='El usuario debe estar logueado y los animales deben tener genealogía '
        'registrada.',
    desencadenante='El usuario necesita evaluar el aporte de cada línea genética del rodeo.',
    curso=[
        'El usuario ingresa a la sección “Reportes” y selecciona “Reporte Genético”.',
        'El sistema despliega el formulario solicitando el alcance —un animal, un '
        'reproductor o el rodeo completo— y el formato de salida.',
        'El usuario completa los parámetros y presiona “Generar”.',
        'El sistema recupera la genealogía y la producción de los animales alcanzados.',
        'El sistema construye el documento en el formato solicitado y lo ofrece para su '
        'descarga.',
    ],
    alternativos='2a. El alcance es un reproductor: el sistema reúne su descendencia y el '
        'desempeño productivo de sus hijas.',
    excepcion='4a. Los animales alcanzados no tienen genealogía registrada: el sistema '
        'informa la situación y no genera el archivo.',
    postcondicion='El archivo queda descargado sin que se modifique ningún registro del '
        'sistema.',
    reglas='El rendimiento por línea genética sólo considera hijas con al menos una '
        'lactancia registrada: una línea sin producción medida no admite comparación.',
    validaciones='El alcance y el formato de salida son obligatorios.',
    frecuencia='Baja.',
),

dict(
    modulo=7, num=48,
    nombre='Configurar Integración con Bot de Telegram',
    actores='Encargada del sector',
    tipo='Primario',
    descripcion='Permite vincular el sistema con un bot de Telegram y elegir qué avisos '
        'automáticos se desean recibir.',
    rf='RF7.5, RF7.6',
    precondicion='El usuario debe estar logueado y debe disponer de una cuenta de Telegram.',
    desencadenante='El usuario desea recibir los avisos del sistema en su teléfono.',
    curso=[
        'El usuario ingresa a la sección de configuración de notificaciones.',
        'El sistema despliega las instrucciones para vincular la cuenta con el bot.',
        'El usuario le escribe al bot desde Telegram, recibe el identificador de su chat '
        'y lo ingresa en el sistema.',
        'El sistema envía un mensaje de prueba a ese chat y, si llega, guarda la '
        'vinculación y despliega la hora del resumen y los ocho tipos de aviso '
        'disponibles: procedimientos sanitarios pendientes, partos próximos, tactos '
        'pendientes, vacas para servir, secados próximos, fin del período de descarte, '
        'stock crítico y vencimiento de insumos.',
        'El usuario elige la hora del resumen, selecciona los avisos que desea recibir y '
        'presiona “Guardar”.',
        'El sistema almacena las preferencias.',
    ],
    alternativos='3a. El sistema ya estaba vinculado a otro chat: el nuevo reemplaza al '
        'anterior, porque el destinatario es uno solo. 5a. El usuario desactiva un tipo '
        'de aviso: el sistema deja de enviarlo, sin afectar su visualización dentro del '
        'sistema.',
    excepcion='4a. El mensaje de prueba no llega: el sistema informa la situación, indica '
        'revisar el identificador y conserva la vinculación anterior, de modo que un '
        'intento fallido no deja al establecimiento sin avisos.',
    postcondicion='El sistema queda habilitado para enviar los avisos seleccionados al '
        'destinatario vinculado, a la hora elegida.',
    reglas='Los avisos enviados son los mismos que el sistema muestra en pantalla: la '
        'notificación es un canal de entrega, no una fuente de información distinta. El '
        'destinatario es uno solo para todo el establecimiento, que tiene una sola '
        'usuaria. La falla del envío no interrumpe la operación del sistema.',
    validaciones='La vinculación con el bot es obligatoria antes de seleccionar avisos. El '
        'identificador de chat es el número que devuelve Telegram. La hora del resumen '
        'debe estar entre las 00:00 y las 23:59.',
    frecuencia='Muy baja, una vez al poner el sistema en marcha.',
),

dict(
    modulo=7, num=49,
    nombre='Enviar Resumen Diario de Tareas Pendientes',
    actores='Encargada del sector',
    tipo='Primario',
    descripcion='La encargada recibe, sin tener que pedirlo, un resumen diario con las '
        'tareas pendientes del establecimiento, de modo que empiece la jornada sabiendo '
        'qué animales necesitan atención.',
    rf='RF7.7',
    precondicion='La integración con el bot de mensajería debe estar configurada y activa.',
    desencadenante='Se alcanza la hora configurada para el envío del resumen. El caso de '
        'uso lo dispara el tiempo, no una acción de la encargada.',
    curso=[
        'El proceso programado se ejecuta a la hora configurada.',
        'El sistema reúne las tareas pendientes del día de los tipos de aviso activos: '
        'procedimientos sanitarios vencidos y por vencer, partos y secados próximos, '
        'tactos pendientes, vacas para servir, animales que terminan el descarte de '
        'leche, insumos en stock crítico y partidas próximas a vencer.',
        'El sistema arma el mensaje agrupando las tareas por módulo.',
        'El sistema envía el mensaje al destinatario vinculado.',
        'El sistema registra el envío.',
    ],
    alternativos='1a. El sistema no estaba en funcionamiento a la hora configurada: el '
        'resumen sale cuando vuelve a estarlo, en lugar de saltear el día. 1b. El usuario '
        'le escribe “/resumen” al bot: el sistema responde con el mismo mensaje en el '
        'momento, sin que eso reemplace al envío automático del día. 2a. No hay tareas '
        'pendientes: el sistema envía igualmente el resumen indicando que no hay '
        'pendientes, de modo que el silencio no se confunda con una falla del envío.',
    excepcion='4a. El envío falla: el sistema registra el error y reintenta más tarde, sin '
        'interrumpir su funcionamiento y sin dar el día por enviado.',
    postcondicion='La encargada recibe el resumen del día en su teléfono, sin necesidad de '
        'ingresar al sistema.',
    reglas='El resumen se construye con los mismos cálculos que alimentan el tablero de '
        'inicio (CU40) y el calendario sanitario (CU31), de modo que las tres vistas no '
        'puedan discrepar. Un pendiente que no se resuelve vuelve a aparecer al día '
        'siguiente: el resumen es la lista de tareas del día y no un aviso de novedades. '
        'El resumen se envía una sola vez por día, aunque el sistema se reinicie.',
    validaciones='—',
    frecuencia='Diaria, de forma automática.',
),

]
