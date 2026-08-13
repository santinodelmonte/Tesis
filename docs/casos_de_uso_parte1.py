# -*- coding: utf-8 -*-
"""Casos de uso 1 a 26 — Modulos 0 a 3. Ver casos_de_uso_parte2.py para el resto.

Cada caso de uso conserva los catorce campos del documento. Los numeros de
requerimiento son los de la v6 del anteproyecto.
"""

MODULOS = {
    0: 'Módulo 0: Seguridad, Acceso y Configuración',
    1: 'Módulo 1: Gestión de Animales y Genética',
    2: 'Módulo 2: Control de Producción',
    3: 'Módulo 3: Gestión Reproductiva',
    4: 'Módulo 4: Gestión Sanitaria',
    5: 'Módulo 5: Control de Insumos y Stock',
    6: 'Módulo 6: Tablero, Indicadores y Apoyo a la Decisión',
    7: 'Módulo 7: Reportes y Notificaciones',
}

CASOS = [

# ----------------------------------------------------------------- Modulo 0

dict(
    modulo=0, num=1,
    nombre='Iniciar Sesión (Log In)',
    actores='Encargada del sector',
    tipo='Primario',
    descripcion='El usuario ingresa las credenciales fijas para acceder al sistema con '
        'control total.',
    rf='RF0.1',
    precondicion='El sistema debe estar operativo en la pantalla de acceso.',
    desencadenante='El usuario se dispone a ingresar al sistema.',
    curso=[
        'El usuario ingresa a la pantalla de Log In.',
        'El sistema despliega la interfaz para ingresar usuario y contraseña.',
        'El usuario introduce las credenciales y presiona “Ingresar”.',
        'El sistema valida las credenciales y abre la sesión, que se mantiene mediante '
        'una cookie de autenticación.',
        'El sistema redirige al tablero de inicio.',
    ],
    alternativos='1a. El usuario intenta acceder directamente a una dirección del sistema '
        'sin haber iniciado sesión: el sistema lo redirige a la pantalla de Log In y, una '
        'vez autenticado, continúa hacia la dirección solicitada.',
    excepcion='4a. Credenciales incorrectas: el sistema muestra un mensaje de error y '
        'deniega el acceso.',
    postcondicion='El usuario queda logueado en el sistema con acceso a todos los módulos.',
    reglas='El acceso se realiza mediante un único par de credenciales fijas. Todas las '
        'páginas del sistema exigen sesión iniciada, con la única excepción de la propia '
        'pantalla de acceso y la de error. Las credenciales no residen en el código '
        'fuente: se leen de la configuración de la aplicación.',
    validaciones='El ingreso de los campos “Usuario” y “Contraseña” es obligatorio.',
    frecuencia='Alta, al inicio de cada jornada de trabajo.',
),

dict(
    modulo=0, num=2,
    nombre='Cerrar Sesión',
    actores='Encargada del sector',
    tipo='Primario',
    descripcion='El usuario finaliza su sesión de trabajo, de modo que el sistema deje de '
        'ser accesible desde ese navegador sin volver a autenticarse.',
    rf='RF0.2',
    precondicion='El usuario debe estar logueado en el sistema.',
    desencadenante='El usuario termina su jornada o deja el equipo disponible para otra '
        'persona.',
    curso=[
        'El usuario presiona “Cerrar sesión” en la barra superior.',
        'El sistema cierra la sesión y descarta la cookie de autenticación.',
        'El sistema redirige a la pantalla de Log In.',
    ],
    alternativos='—',
    excepcion='—',
    postcondicion='La sesión queda cerrada. Cualquier intento posterior de acceder a una '
        'pantalla del sistema deriva en el Log In.',
    reglas='La opción de cerrar sesión está disponible desde cualquier pantalla del '
        'sistema.',
    validaciones='—',
    frecuencia='Alta, al finalizar cada jornada de trabajo.',
),

dict(
    modulo=0, num=3,
    nombre='Configurar Parámetros del Establecimiento',
    actores='Encargada del sector',
    tipo='Primario',
    descripcion='El usuario ajusta los parámetros de manejo con los que el sistema calcula '
        'fechas recomendadas, valida cargas y arma los avisos, de modo que las reglas '
        'respondan al criterio del establecimiento y no a constantes fijas del sistema.',
    rf='RF0.3',
    precondicion='El usuario debe estar logueado en el sistema.',
    desencadenante='El establecimiento adopta un criterio de manejo distinto del que el '
        'sistema tiene configurado.',
    curso=[
        'El usuario ingresa a la sección “Configuración”.',
        'El sistema despliega el formulario con los valores vigentes de cada uno de los '
        'once parámetros: días de secado previos al parto, edad mínima al servicio, edad '
        'de cambio de categoría, litros máximos por control individual, cantidad de '
        'ordeñes diarios, espera voluntaria posparto, días para el tacto, y los días de '
        'anticipación de los avisos de secado, de parto, del calendario sanitario y de '
        'vencimiento de insumos.',
        'El usuario modifica los valores que correspondan y presiona “Guardar”.',
        'El sistema valida que cada valor se encuentre dentro de su rango admitido.',
        'El sistema almacena la configuración y confirma la operación.',
    ],
    alternativos='2a. El establecimiento nunca configuró los parámetros: el sistema muestra '
        'los valores por defecto y opera con ellos hasta que se guarde una configuración '
        'propia.',
    excepcion='4a. Algún valor queda fuera del rango admitido: el sistema informa cuál es '
        'y no guarda ningún cambio.',
    postcondicion='Los nuevos valores rigen de inmediato para todos los cálculos, '
        'validaciones y avisos del sistema.',
    reglas='La configuración es única para todo el establecimiento. Los parámetros no '
        'alteran los registros ya guardados: cambian la forma en que el sistema calcula '
        'de ahí en adelante. Reducir la cantidad de ordeñes diarios no borra los registros '
        'de turnos que dejan de ofrecerse.',
    validaciones='Todos los parámetros son obligatorios y numéricos. Cada uno tiene un '
        'rango admitido, informado en el propio formulario.',
    frecuencia='Baja, al poner el sistema en marcha y ante cambios de criterio de manejo.',
),

# ----------------------------------------------------------------- Modulo 1

dict(
    modulo=1, num=4,
    nombre='Registrar Alta de Animal',
    actores='Encargada del sector',
    tipo='Primario',
    descripcion='Permite ingresar un nuevo animal al sistema con sus datos básicos, su '
        'fotografía y su registro genealógico.',
    rf='RF1.1, RF1.4, RF1.5, RF1.8, RF1.12, RF1.14',
    precondicion='El usuario debe estar logueado en el sistema.',
    desencadenante='El usuario ingresa a la sección de alta de animales.',
    curso=[
        'El usuario ingresa a la pantalla de Alta de Animal.',
        'El sistema despliega el formulario solicitando: número de caravana, fecha de '
        'nacimiento, sexo, raza, categoría, fotografía, padre y madre.',
        'El sistema calcula la categoría que corresponde al animal y la propone en el '
        'formulario, permitiendo que el usuario la ajuste manualmente si el caso lo '
        'requiere.',
        'El usuario completa la información y presiona “Guardar”.',
        '(Incluye: Validar Unicidad de Caravana).',
        'El sistema valida que la genealogía indicada sea posible.',
        'El sistema almacena los datos y confirma el registro exitoso.',
    ],
    alternativos='2a. No se conocen el padre y la madre del animal, como ocurre en la '
        'carga inicial del rodeo: el usuario deja los progenitores sin indicar y el '
        'sistema registra al animal igual. 2b. El usuario no carga fotografía: el sistema '
        'muestra una silueta genérica en la ficha y en el árbol genealógico. 6a. La '
        'genealogía es posible pero inusual —un progenitor dado de baja—: el sistema '
        'advierte y permite continuar.',
    excepcion='5a. El número de caravana ya existe en el sistema: el sistema muestra una '
        'alerta de duplicado e impide el guardado. 6a. La genealogía indicada es '
        'imposible: el sistema informa el motivo e impide el guardado.',
    postcondicion='El animal queda registrado correctamente en la base de datos del '
        'establecimiento.',
    reglas='El registro del padre y de la madre es opcional: un animal puede no tener '
        'progenitores conocidos, y de hecho así ingresan los animales del rodeo inicial. '
        'La categoría se calcula automáticamente a partir del sexo, la edad y la cantidad '
        'de partos del animal; el valor calculado se propone por defecto y puede ser '
        'ajustado por el usuario ante situaciones puntuales que así lo requieran. La '
        'fecha de nacimiento del progenitor debe admitir la edad mínima al servicio '
        'respecto de la cría.',
    validaciones='El número de caravana es obligatorio y único. La raza es obligatoria. '
        'La fecha de nacimiento no puede ser futura. La fotografía, cuando se carga, debe '
        'ser un archivo de imagen.',
    frecuencia='Media, cada vez que nace un animal o ingresa una nueva adquisición al '
        'tambo.',
),

dict(
    modulo=1, num=5,
    nombre='Modificar Datos de Animal',
    actores='Encargada del sector',
    tipo='Primario',
    descripcion='Permite actualizar la información de un animal previamente registrado en '
        'el sistema, incluida su fotografía y su genealogía.',
    rf='RF1.3, RF1.9, RF1.12, RF1.14',
    precondicion='El usuario debe estar logueado en el sistema y el animal a modificar '
        'debe existir.',
    desencadenante='El usuario selecciona la opción de editar un animal dentro del sistema.',
    curso=[
        'El usuario busca y selecciona el animal que desea modificar.',
        'El sistema despliega la interfaz con los datos actuales del animal cargados en '
        'un formulario.',
        'El usuario edita los campos correspondientes y presiona “Guardar Cambios”.',
        'El sistema valida que la genealogía resultante sea posible.',
        'El sistema actualiza el registro en la base de datos y confirma el éxito de la '
        'operación.',
    ],
    alternativos='3a. El usuario reemplaza la fotografía: el sistema guarda la nueva y '
        'descarta la anterior. 3b. La categoría almacenada no coincide con la que '
        'corresponde a la edad y a la cantidad de partos del animal: el sistema lo señala '
        'y ofrece actualizarla; el usuario acepta la categoría recalculada o conserva la '
        'que había.',
    excepcion='4a. La genealogía resultante es imposible: el sistema informa el motivo e '
        'impide el guardado.',
    postcondicion='Los datos del animal quedan actualizados correctamente en el sistema.',
    reglas='Un animal no puede ser progenitor de sí mismo ni tener como progenitor a un '
        'animal que figure en su propia descendencia. La categoría recalculada nunca se '
        'aplica sin intervención del usuario, salvo al registrarse un parto (CU24).',
    validaciones='Se mantienen las validaciones de unicidad de caravana y de coherencia '
        'genealógica del alta.',
    frecuencia='Media, cuando ocurren correcciones de datos o actualizaciones puntuales.',
),

dict(
    modulo=1, num=6,
    nombre='Registrar Baja de Animal',
    actores='Encargada del sector',
    tipo='Primario',
    descripcion='Permite registrar la baja lógica de un animal del rodeo activo, '
        'especificando la fecha y la causa de su salida.',
    rf='RF1.2',
    precondicion='El usuario debe estar logueado en el sistema y el animal debe figurar '
        'en estado activo.',
    desencadenante='El usuario selecciona la opción de dar de baja a un animal.',
    curso=[
        'El usuario busca y selecciona el animal al que desea dar de baja.',
        'El sistema despliega el formulario solicitando la fecha y el motivo de la salida.',
        'El usuario indica la fecha, selecciona el motivo (venta, fallecimiento, descarte '
        'sanitario u otros) y presiona “Confirmar Baja”.',
        'El sistema actualiza el estado del animal a inactivo, almacena la fecha y el '
        'motivo, y confirma la operación.',
    ],
    alternativos='—',
    excepcion='—',
    postcondicion='El animal cambia su estado a inactivo y deja de formar parte del rodeo '
        'actual: el sistema no lo ofrece en los lotes de ordeñe, en las listas de trabajo '
        'ni en el calendario sanitario. Su registro histórico y su lugar en el linaje se '
        'conservan íntegros.',
    reglas='Las bajas son siempre lógicas: no existe borrado definitivo de animales, '
        'porque destruiría el historial productivo y el linaje genealógico de los animales '
        'emparentados. La baja no impide registrar eventos anteriores a su fecha, pero sí '
        'posteriores.',
    validaciones='La fecha y el motivo de salida son obligatorios. La fecha no puede ser '
        'futura ni anterior a la fecha de nacimiento del animal.',
    frecuencia='Baja, sólo ante la pérdida, descarte o comercialización de un vientre o '
        'cría.',
),

dict(
    modulo=1, num=7,
    nombre='Reactivar Animal',
    actores='Encargada del sector',
    tipo='Primario',
    descripcion='Permite revertir la baja de un animal, devolviéndolo al rodeo activo. '
        'Resuelve el caso de la baja registrada sobre la caravana equivocada.',
    rf='RF1.11',
    precondicion='El usuario debe estar logueado y el animal debe figurar dado de baja.',
    desencadenante='El usuario advierte que la baja fue un error o que el animal regresó '
        'al establecimiento.',
    curso=[
        'El usuario ingresa a la ficha del animal.',
        'El sistema informa que el animal está dado de baja, desde cuándo y por qué motivo, '
        'y ofrece la opción de reactivarlo.',
        'El usuario presiona “Reactivar” y confirma.',
        'El sistema devuelve el animal al estado activo, limpia la fecha y el motivo de '
        'salida, y confirma la operación.',
    ],
    alternativos='—',
    excepcion='—',
    postcondicion='El animal vuelve a integrar el rodeo activo y el sistema lo considera '
        'nuevamente en los lotes de ordeñe, las listas de trabajo y el calendario '
        'sanitario.',
    reglas='La reactivación no reconstruye los eventos que el animal no protagonizó '
        'mientras estuvo dado de baja: sólo lo devuelve al rodeo.',
    validaciones='—',
    frecuencia='Baja, ante una corrección o el reingreso de un animal.',
),

dict(
    modulo=1, num=8,
    nombre='Consultar Linaje y Registro Genealógico',
    actores='Encargada del sector',
    tipo='Primario',
    descripcion='Permite recorrer el árbol genealógico de un animal, desplegando cada rama '
        'de la ascendencia y accediendo a la ficha de cualquier ancestro.',
    rf='RF1.6, RF1.13',
    precondicion='El usuario debe estar logueado en el sistema.',
    desencadenante='El usuario solicita ver la genealogía de un animal específico.',
    curso=[
        'El usuario busca y selecciona el animal dentro del rodeo.',
        'El usuario selecciona la opción “Ver Linaje”.',
        'El sistema recupera los datos de los progenitores vinculados.',
        'El sistema despliega el árbol genealógico mostrando al animal y a sus '
        'progenitores directos, cada uno con su fotografía y su caravana.',
        'El usuario despliega la rama de cualquier ancestro y el sistema incorpora al '
        'árbol la generación siguiente.',
    ],
    alternativos='5a. El usuario selecciona un ancestro: el sistema abre la ficha integral '
        'de ese animal.',
    excepcion='3a. El animal no tiene progenitores registrados: el sistema muestra el árbol '
        'con el padre y la madre como “No registrado”, sin rama para desplegar.',
    postcondicion='El sistema despliega la información solicitada sin alterar los registros '
        'existentes.',
    reglas='El árbol se construye a demanda: cada rama se resuelve cuando el usuario la '
        'despliega, de modo que un linaje profundo no obliga a recorrer todo el rodeo.',
    validaciones='—',
    frecuencia='Media, consultada principalmente durante la planificación de servicios o '
        'el análisis de descarte.',
),

dict(
    modulo=1, num=9,
    nombre='Verificar Consanguinidad',
    actores='Encargada del sector',
    tipo='Primario',
    descripcion='El usuario consulta el grado de parentesco entre una hembra y un posible '
        'reproductor antes de asignar el servicio, de modo de evitar cruzamientos entre '
        'animales emparentados.',
    rf='RF1.7',
    precondicion='Ambos animales deben estar registrados en el sistema con su genealogía '
        'cargada.',
    desencadenante='El usuario planifica el servicio de una hembra.',
    curso=[
        'El usuario ingresa a la sección “Verificar Consanguinidad”.',
        'El usuario selecciona la hembra por su número de caravana.',
        'El usuario selecciona el reproductor, que puede ser un toro del rodeo o un toro de '
        'catálogo asociado a una pajuela.',
        'El usuario presiona “Verificar”.',
        'El sistema recorre la ascendencia registrada de ambos animales y busca '
        'progenitores comunes.',
        'El sistema despliega el resultado indicando si existe parentesco y qué animal lo '
        'origina.',
    ],
    alternativos='3a. El usuario selecciona una pajuela: el sistema toma como reproductor '
        'de la verificación al toro de catálogo vinculado a esa pajuela.',
    excepcion='5a. Alguno de los animales no tiene toda su ascendencia registrada: el '
        'sistema informa que la verificación es parcial.',
    postcondicion='El usuario conoce el riesgo de consanguinidad antes de registrar el '
        'servicio.',
    reglas='Se considera parentesco la existencia de un ancestro común entre ambos '
        'animales. La verificación es informativa y no bloquea el registro del servicio; '
        'el sistema vuelve a advertirlo al registrarse el servicio (CU21).',
    validaciones='La selección de la hembra y del reproductor es obligatoria. Ambos no '
        'pueden ser el mismo animal.',
    frecuencia='Media, cada vez que se planifica un servicio.',
),

dict(
    modulo=1, num=10,
    nombre='Buscar y Filtrar Animales del Rodeo',
    actores='Encargada del sector',
    tipo='Primario',
    descripcion='Permite realizar búsquedas y aplicar filtros combinados sobre los animales '
        'registrados.',
    rf='RF1.10',
    precondicion='El usuario debe estar logueado en el sistema.',
    desencadenante='El usuario ingresa a la pantalla de consulta de animales.',
    curso=[
        'El usuario ingresa a la pantalla de Consulta de Animales.',
        'El sistema despliega el listado paginado y la barra de filtros de búsqueda.',
        'El usuario ingresa o selecciona los criterios deseados —número de caravana, raza, '
        'categoría, estado o rango etario— y presiona “Buscar”.',
        'El sistema procesa la solicitud y muestra únicamente los animales que coinciden '
        'con todos los filtros aplicados.',
    ],
    alternativos='4a. El resultado excede el tamaño de página: el sistema pagina el '
        'listado e informa la cantidad total de animales encontrados. 4b. Algún animal '
        'tiene la categoría desactualizada respecto de su edad: el sistema lo señala en el '
        'listado y ofrece actualizarla.',
    excepcion='4a. No existen registros que coincidan con los criterios: el sistema muestra '
        'un mensaje indicando que no se encontraron resultados.',
    postcondicion='El sistema muestra en pantalla la lista filtrada de animales sin '
        'modificar ningún registro.',
    reglas='Los filtros son acumulativos: el animal debe cumplir todos los criterios '
        'indicados. Por defecto el listado muestra los animales activos.',
    validaciones='—',
    frecuencia='Alta, cada vez que se requiere consultar la ficha o el estado de algún '
        'animal.',
),

dict(
    modulo=1, num=11,
    nombre='Consultar Ficha Integral del Animal',
    actores='Encargada del sector',
    tipo='Primario',
    descripcion='Reúne en una sola pantalla todo lo que el sistema sabe de un animal: su '
        'identificación, su estado, su linaje y su historial productivo, reproductivo y '
        'sanitario.',
    rf='RF1.15',
    precondicion='El usuario debe estar logueado y el animal debe existir en el sistema.',
    desencadenante='El usuario necesita conocer la situación completa de un animal, '
        'habitualmente antes de tomar una decisión de manejo.',
    curso=[
        'El usuario llega a la ficha desde el listado de animales, desde el buscador de '
        'caravana o desde cualquier pantalla que mencione al animal.',
        'El sistema despliega la identificación, la fotografía, la categoría, la edad y el '
        'estado productivo y reproductivo del animal.',
        'El sistema despliega sus progenitores y el acceso al árbol genealógico.',
        'El sistema despliega su historial: lactancias y controles de producción, celos, '
        'servicios, tactos y partos, y diagnósticos, tratamientos, vacunaciones y '
        'descornes.',
        'El sistema ofrece, junto a cada sección, las acciones que corresponden al estado '
        'actual del animal.',
    ],
    alternativos='2a. El animal está dado de baja: el sistema lo informa junto con la fecha '
        'y el motivo, y ofrece reactivarlo (CU7). 4a. El animal tiene un período de '
        'descarte de leche vigente: el sistema lo advierte con la fecha en que finaliza.',
    excepcion='—',
    postcondicion='El sistema expone la información sin alterar ningún registro.',
    reglas='La ficha es de sólo lectura: toda modificación se realiza a través del caso de '
        'uso correspondiente, al que la ficha da acceso.',
    validaciones='—',
    frecuencia='Alta, es la pantalla de consulta habitual del sistema.',
),

# ----------------------------------------------------------------- Modulo 2

dict(
    modulo=2, num=12,
    nombre='Registrar Ordeñe por Lote',
    actores='Encargada del sector',
    tipo='Primario',
    descripcion='El usuario registra los litros totales obtenidos por el rodeo en un turno '
        'de ordeñe, tal como se leen del tanque.',
    rf='RF2.1, RF2.2',
    precondicion='El usuario debe estar logueado en el sistema.',
    desencadenante='El usuario ingresa a la pantalla de ordeñe por lote al terminar el '
        'ordeñe.',
    curso=[
        'El usuario selecciona la fecha y el turno, entre los turnos que corresponden a la '
        'cantidad de ordeñes diarios configurada.',
        'El sistema carga automáticamente la lista de los animales con estado productivo '
        '“en lactancia”.',
        'El sistema excluye de la lista a los animales que se encuentran dentro de un '
        'período de descarte de leche vigente.',
        'El usuario modifica la lista si es necesario, removiendo los animales que no se '
        'ordeñaron o agregando los que faltan.',
        'El usuario ingresa los litros totales obtenidos y presiona “Guardar”.',
        'El sistema valida la consistencia de los litros ingresados.',
        'El sistema almacena el registro del turno junto con los animales que lo '
        'integraron y confirma la operación.',
    ],
    alternativos='3a. El usuario decide incluir manualmente un animal excluido por '
        'descarte: el sistema advierte que la leche de ese animal no debe destinarse a '
        'consumo y solicita confirmación.',
    excepcion='5a. Ya existe un registro de ordeñe por lote para esa fecha y turno: el '
        'sistema informa la duplicación y remite a la corrección del registro existente '
        '(CU19). 6a. Los litros ingresados son negativos o superan el máximo admitido: el '
        'sistema muestra una alerta de error e impide el guardado.',
    postcondicion='El ordeñe del turno queda guardado en el historial de producción del '
        'establecimiento.',
    reglas='El ordeñe por lote es la medida de la leche que salió del tambo. Se registra '
        'una vez por fecha y turno. Los animales con un tratamiento cuyo período de '
        'descarte sigue vigente quedan excluidos del lote sin que se modifique su estado '
        'productivo.',
    validaciones='La fecha, el turno y los litros totales son obligatorios. Los litros '
        'deben ser positivos y no superar el máximo por control individual configurado '
        'multiplicado por la cantidad de animales del lote.',
    frecuencia='Alta, una vez por turno de ordeñe.',
),

dict(
    modulo=2, num=13,
    nombre='Registrar Control Lechero',
    actores='Encargada del sector',
    tipo='Primario',
    descripcion='Permite registrar cuántos litros produjo cada animal en un turno medido. '
        'El camino habitual es la carga masiva de todo el rodeo en ordeñe, que es como se '
        'realiza el control lechero en el establecimiento.',
    rf='RF2.2, RF2.3',
    precondicion='El usuario debe estar logueado y deben existir animales en lactancia.',
    desencadenante='Se realiza el control lechero del mes.',
    curso=[
        'El usuario ingresa a la pantalla de Control Lechero.',
        'El usuario selecciona la fecha y el turno del control.',
        'El sistema despliega la lista completa de los animales en ordeñe, cada uno con un '
        'campo para sus litros, junto con un filtro por caravana.',
        'El usuario completa los litros de los animales medidos y presiona “Guardar”.',
        'El sistema valida cada control por separado, lo imputa a la lactancia vigente del '
        'animal y lo almacena.',
        'El sistema informa cuántos controles se guardaron y detalla cuáles no pudieron '
        'guardarse y por qué.',
    ],
    alternativos='1a. El usuario necesita registrar un solo animal —una vaca que faltó, una '
        'medición suelta o una carga retroactiva—: accede a “Control de una vaca” desde el '
        'control lechero o desde la ficha del animal, y registra el control individual con '
        'las mismas reglas.',
    excepcion='5a. El animal ya tiene un control registrado para esa fecha y turno: ese '
        'control no se guarda y el sistema lo informa, sin afectar a los demás. 5b. Los '
        'litros de un animal superan el máximo admitido: ese control no se guarda y el '
        'sistema lo informa.',
    postcondicion='Cada control queda asentado en el historial productivo de su animal e '
        'imputado a la lactancia vigente.',
    reglas='El control lechero mide al animal, no la leche vendible: a una vaca en '
        'tratamiento se la mide igual, aunque esa leche se descarte. Por eso el control '
        'individual y el ordeñe por lote no se suman dentro de un mismo turno. Cada '
        'control se guarda de forma independiente: el fallo de una fila no impide guardar '
        'las demás.',
    validaciones='La fecha y el turno son obligatorios. Los litros de cada animal deben ser '
        'positivos y no superar el máximo por control configurado. Un animal no puede '
        'tener dos controles en la misma fecha y turno.',
    frecuencia='Media, habitualmente una vez al mes.',
),

dict(
    modulo=2, num=14,
    nombre='Consultar Historial de Producción y Lactancias',
    actores='Encargada del sector',
    tipo='Primario',
    descripcion='Permite consultar cronológicamente la producción del establecimiento y las '
        'lactancias de cada animal dentro de un rango de fechas.',
    rf='RF2.5, RF2.6, RF2.7',
    precondicion='El usuario debe estar logueado en el sistema.',
    desencadenante='El usuario solicita consultar los historiales de producción.',
    curso=[
        'El usuario ingresa a la sección de Historial de Producción.',
        'El usuario selecciona la modalidad de visualización: producción del '
        'establecimiento o controles individuales.',
        'El usuario define el rango de fechas y presiona “Buscar”.',
        'El sistema recupera la información correspondiente al período.',
        'El sistema despliega el listado y el acumulado de litros según la modalidad '
        'elegida.',
    ],
    alternativos='2a. El usuario consulta las lactancias: el sistema lista las lactancias '
        'registradas con su fecha de inicio y de cierre, los días en leche, la cantidad de '
        'controles y la producción estimada de cada una.',
    excepcion='3a. Rango de fechas inválido: el sistema muestra un mensaje de error y no '
        'procesa la consulta.',
    postcondicion='El sistema expone la información histórica sin alterar los registros de '
        'la base de datos.',
    reglas='La producción del establecimiento se resuelve turno por turno: si el turno '
        'tiene registro por lote, la producción de ese turno es ese total, que ya incluye '
        'a los animales medidos individualmente; si el turno se registró únicamente con '
        'controles individuales, la producción es la suma de esos controles. Las dos '
        'fuentes nunca se suman dentro de un mismo turno, porque eso contaría dos veces la '
        'leche de los animales controlados. La producción de una lactancia no es la suma '
        'de sus controles —que son mediciones puntuales— sino una estimación construida a '
        'partir de los intervalos entre controles sucesivos.',
    validaciones='La selección de la modalidad y los campos de fecha son obligatorios. La '
        'fecha de inicio debe ser anterior o igual a la de fin.',
    frecuencia='Media.',
),

dict(
    modulo=2, num=15,
    nombre='Consultar Métrica de Producción Mensual',
    actores='Encargada del sector',
    tipo='Primario',
    descripcion='Permite visualizar los litros totales producidos por el establecimiento en '
        'un mes calendario.',
    rf='RF2.4, RF2.7',
    precondicion='El usuario debe estar logueado en el sistema.',
    desencadenante='El usuario accede al panel de producción mensual.',
    curso=[
        'El usuario selecciona el mes y el año que desea consultar.',
        'El sistema recorre los turnos del mes.',
        'El sistema toma, para cada turno, el registro por lote cuando existe, y la suma '
        'de los controles individuales cuando el turno se registró únicamente de ese modo.',
        'El sistema acumula los totales de cada turno.',
        'El sistema despliega el total de litros del mes.',
    ],
    alternativos='—',
    excepcion='2a. No existen registros de producción en el mes seleccionado: el sistema '
        'informa que no hay datos para el período.',
    postcondicion='Se visualiza el acumulado mensual sin realizar modificaciones en la base '
        'de datos.',
    reglas='El cálculo aplica la misma regla que el historial: un turno aporta su registro '
        'por lote o sus controles individuales, nunca ambos.',
    validaciones='El mes y el año son obligatorios.',
    frecuencia='Media, habitualmente al finalizar cada mes calendario.',
),

dict(
    modulo=2, num=16,
    nombre='Registrar Período de Secado Manual',
    actores='Encargada del sector',
    tipo='Primario',
    descripcion='Permite registrar el inicio del período de secado de una vaca, '
        'removiéndola de la etapa productiva.',
    rf='RF2.8, RF2.11',
    precondicion='El usuario debe estar logueado y el animal debe estar en estado de '
        'lactancia.',
    desencadenante='El usuario decide pasar un animal a descanso preparto.',
    curso=[
        'El usuario busca al animal por su número de caravana.',
        'El usuario selecciona la opción “Registrar Secado”.',
        'El usuario ingresa la fecha correspondiente y confirma la acción.',
        'El sistema guarda la fecha de cese del ordeñe y cierra la lactancia vigente.',
        'El sistema cambia el estado productivo del animal a “seca”.',
    ],
    alternativos='—',
    excepcion='2a. El animal no está en lactancia: el sistema informa la situación e impide '
        'el registro. 3a. La fecha de secado es anterior al inicio de la lactancia vigente '
        'o posterior al día de hoy: el sistema muestra el error y no guarda.',
    postcondicion='El animal queda con estado productivo “seca”, por lo cual el sistema '
        'deja de incluirlo automáticamente en los próximos ordeñes por lote. Su estado '
        'reproductivo no se modifica.',
    reglas='El cambio de estado productivo es automático e inmediato tras el guardado. El '
        'secado afecta únicamente el eje productivo: una hembra seca puede estar preñada, '
        'y de hecho es lo habitual.',
    validaciones='El ingreso de la fecha de secado es obligatorio.',
    frecuencia='Media.',
),

dict(
    modulo=2, num=17,
    nombre='Consultar Alertas de Secado Próximo',
    actores='Encargada del sector',
    tipo='Primario',
    descripcion='Despliega las vacas en producción que están próximas a cumplir el tiempo '
        'sugerido para iniciar su secado.',
    rf='RF2.9, RF2.10',
    precondicion='El usuario debe estar logueado en el sistema.',
    desencadenante='El usuario ingresa a la sección de alertas de producción.',
    curso=[
        'El usuario accede a la sección “Alertas de Secado”.',
        'El sistema calcula, para cada hembra preñada y en lactancia, la fecha recomendada '
        'de secado restando los días de secado configurados a su fecha probable de parto.',
        'El sistema selecciona aquellas cuya fecha recomendada cae dentro de la '
        'anticipación configurada.',
        'El sistema despliega el listado con la fecha recomendada de cada animal y los días '
        'que restan.',
    ],
    alternativos='—',
    excepcion='—',
    postcondicion='Se exponen las alertas en la interfaz sin alterar ningún registro.',
    reglas='La fecha recomendada de secado depende de dos parámetros configurables: los '
        'días de secado previos al parto y la anticipación con la que se desea el aviso. '
        'Sólo se consideran hembras activas, preñadas y en lactancia.',
    validaciones='—',
    frecuencia='Alta, revisada periódicamente.',
),

dict(
    modulo=2, num=18,
    nombre='Abrir Lactancia Manualmente',
    actores='Encargada del sector',
    tipo='Primario',
    descripcion='Permite abrir la lactancia de una vaca que ya estaba en ordeñe cuando se '
        'comenzó a usar el sistema, sin un parto registrado que la origine.',
    rf='RF2.12',
    precondicion='El usuario debe estar logueado, el animal debe ser una hembra activa y no '
        'debe tener una lactancia abierta.',
    desencadenante='Se pone el sistema en marcha con el rodeo ya en producción, o se '
        'incorpora una vaca en ordeñe sin registrar su parto.',
    curso=[
        'El usuario ingresa a la sección de Lactancias y selecciona “Abrir Lactancia”.',
        'El usuario selecciona el animal e ingresa la fecha de inicio.',
        'El sistema propone el número de lactancia que corresponde según los partos '
        'registrados del animal.',
        'El usuario confirma y el sistema abre la lactancia, deja al animal en estado '
        'productivo “en lactancia” y confirma la operación.',
    ],
    alternativos='3a. El usuario corrige el número de lactancia propuesto: el sistema '
        'conserva el valor ingresado.',
    excepcion='2a. El animal ya tiene una lactancia abierta: el sistema informa la '
        'situación e impide el registro. 2b. La fecha de inicio es futura: el sistema '
        'muestra el error y no guarda.',
    postcondicion='La lactancia queda abierta y los controles lecheros del animal se '
        'imputan a ella.',
    reglas='La vía normal de apertura de una lactancia es el parto (CU24). La apertura '
        'manual existe para la carga inicial del rodeo, donde no hay parto que registrar.',
    validaciones='El animal y la fecha de inicio son obligatorios. Un animal no puede tener '
        'dos lactancias abiertas a la vez.',
    frecuencia='Baja, concentrada en la puesta en marcha del sistema.',
),

dict(
    modulo=2, num=19,
    nombre='Corregir o Eliminar Registro de Producción',
    actores='Encargada del sector',
    tipo='Primario',
    descripcion='Permite corregir o eliminar un ordeñe por lote o un control lechero ya '
        'registrado, informando qué registros dependen de aquel que se pretende eliminar.',
    rf='RF2.13',
    precondicion='El usuario debe estar logueado y el registro a corregir debe existir.',
    desencadenante='El usuario advierte un error en un registro de producción ya guardado.',
    curso=[
        'El usuario localiza el registro en el historial de producción.',
        'El usuario selecciona “Editar” o “Eliminar”.',
        'Al editar, el sistema despliega la misma pantalla que dio de alta el registro, con '
        'los datos cargados, y aplica las mismas validaciones.',
        'Al eliminar, el sistema verifica que ningún otro registro dependa de éste.',
        'El sistema aplica el cambio y confirma la operación.',
    ],
    alternativos='2a. El registro es un ordeñe por lote: la corrección alcanza los litros '
        'totales y la lista de animales del turno.',
    excepcion='4a. El registro tiene otros que dependen de él: el sistema informa cuáles '
        'son y qué debe eliminarse primero, sin borrar nada.',
    postcondicion='El registro queda corregido o eliminado, y los totales del período '
        'reflejan el cambio.',
    reglas='La corrección reusa la pantalla del alta: los campos y las reglas son los '
        'mismos. La eliminación de un ordeñe por lote no arrastra los controles '
        'individuales del turno, que siguen siendo mediciones válidas de sus animales.',
    validaciones='Las mismas del registro original.',
    frecuencia='Baja, ante errores de carga.',
),

# ----------------------------------------------------------------- Modulo 3

dict(
    modulo=3, num=20,
    nombre='Registrar Detección de Celo',
    actores='Encargada del sector',
    tipo='Primario',
    descripcion='Permite asentar la fecha y las observaciones de la detección de celo de '
        'una hembra para el seguimiento de su ciclo reproductivo.',
    rf='RF3.1, RF3.10',
    precondicion='El usuario debe estar logueado y el animal debe ser una hembra.',
    desencadenante='El usuario observa un animal en celo y accede a registrar el evento.',
    curso=[
        'El usuario busca al animal por su número de caravana.',
        'El usuario selecciona la opción “Registrar Celo”.',
        'El sistema solicita la fecha de detección y las observaciones.',
        'El usuario completa los datos y presiona “Guardar”.',
        'El sistema valida que el animal esté en condiciones de manifestar celo.',
        'El sistema almacena la novedad reproductiva y confirma el registro exitoso.',
    ],
    alternativos='—',
    excepcion='1a. La caravana corresponde a un macho: el sistema emite un mensaje de error '
        'e impide la operación. 5a. El animal no alcanzó la edad mínima al servicio a la '
        'fecha del celo: el sistema informa la situación e impide el registro. 5b. La fecha '
        'del celo es posterior a la baja del animal: el sistema informa la situación e '
        'impide el registro.',
    postcondicion='El celo queda registrado en la ficha reproductiva histórica de la hembra.',
    reglas='Una ternera no manifiesta celo a efectos del manejo: el sistema exige que el '
        'animal haya alcanzado la edad mínima al servicio configurada. Un animal dado de '
        'baja no protagoniza eventos posteriores a su baja.',
    validaciones='La fecha de detección es obligatoria y no puede ser futura.',
    frecuencia='Alta, según los ciclos observados en el rodeo.',
),

dict(
    modulo=3, num=21,
    nombre='Registrar Servicio',
    actores='Encargada del sector',
    tipo='Primario',
    descripcion='El usuario registra el servicio de una hembra, ya sea por monta natural '
        'con un toro del rodeo o por inseminación artificial con una pajuela del stock.',
    rf='RF3.2, RF3.3, RF3.9, RF3.10, RF3.11, RF5.4',
    precondicion='La hembra debe estar registrada y en condiciones de recibir servicio. En '
        'la inseminación artificial debe existir stock de la pajuela seleccionada.',
    desencadenante='El usuario detectó el celo de la hembra y decide darle servicio.',
    curso=[
        'El usuario busca e ingresa la caravana de la hembra.',
        'El sistema despliega el formulario de servicio solicitando la fecha y el tipo de '
        'servicio.',
        'El usuario selecciona el tipo de servicio: monta natural o inseminación artificial.',
        'El usuario indica el reproductor: un toro del rodeo si es monta natural, o una '
        'pajuela del stock si es inseminación artificial.',
        'El usuario presiona “Guardar”.',
        'El sistema valida que la hembra esté en condiciones de recibir servicio y presenta '
        'las advertencias que correspondan.',
        '(Incluye: Descontar Automáticamente Semen de Stock, sólo en la inseminación '
        'artificial).',
        'El sistema calcula la fecha probable de parto sumando el período de gestación a la '
        'fecha del servicio.',
        'El sistema vincula genéticamente el servicio al toro, guarda el registro, deja a '
        'la hembra en estado reproductivo “servida” y confirma la operación.',
    ],
    alternativos='4a. El servicio es por monta natural: el sistema registra el toro del '
        'rodeo y no descuenta stock. 4b. El servicio es por inseminación artificial: el '
        'sistema registra la pajuela, descuenta una unidad del stock y toma el toro de '
        'catálogo vinculado a esa pajuela como reproductor. 6a. El sistema advierte que el '
        'toro elegido está dado de baja, o que existe parentesco entre la hembra y el '
        'reproductor: el usuario confirma y el registro continúa.',
    excepcion='6a. La hembra no alcanzó la edad mínima al servicio: el sistema informa la '
        'situación e impide el registro. 6b. La fecha del servicio es posterior a la baja '
        'de la hembra o del toro: el sistema informa la situación e impide el registro. '
        '7a. La pajuela seleccionada no tiene stock disponible: el sistema informa la falta '
        'y no registra el servicio. 5a. El usuario no indicó reproductor: el sistema '
        'solicita completarlo antes de guardar.',
    postcondicion='El servicio queda registrado y asociado a su reproductor, con la fecha '
        'probable de parto calculada, y la hembra queda en estado reproductivo “servida”.',
    reglas='Todo servicio tiene un único reproductor: el toro del rodeo y la pajuela son '
        'mutuamente excluyentes. La pajuela conserva el vínculo con el toro que la aporta, '
        'de modo que la genealogía de la cría puede reconstruirse aunque el reproductor no '
        'integre el rodeo. El parentesco entre la hembra y el reproductor se advierte pero '
        'no bloquea: la decisión es del establecimiento.',
    validaciones='La fecha, el tipo de servicio y el reproductor son obligatorios. La fecha '
        'del servicio no puede ser posterior a la fecha actual.',
    frecuencia='Media.',
),

dict(
    modulo=3, num=22,
    nombre='Registrar Tacto y Confirmación de Preñez',
    actores='Encargada del sector',
    tipo='Primario',
    descripcion='Permite registrar el resultado de un control reproductivo posterior al '
        'servicio y, en caso positivo, confirmar la preñez de la hembra.',
    rf='RF3.4, RF3.5, RF3.6, RF3.9',
    precondicion='El usuario debe estar logueado y el animal debe contar con un servicio '
        'vigente registrado.',
    desencadenante='Se realiza el tacto clínico al animal.',
    curso=[
        'El usuario busca la caravana del animal sometido a control.',
        'El usuario accede a “Registrar Tacto”.',
        'El usuario ingresa la fecha del tacto y selecciona el resultado: preñada, vacía o '
        'dudosa.',
        'El usuario presiona “Guardar”.',
        'El sistema actualiza el estado reproductivo de la hembra según el resultado: '
        '“preñada” si el tacto fue positivo, “vacía” si fue negativo.',
        'El sistema conserva la fecha probable de parto calculada a partir del servicio '
        'cuando el resultado es positivo.',
        'El sistema guarda el control y confirma la operación.',
    ],
    alternativos='5a. El resultado es dudoso: el sistema conserva el estado reproductivo '
        '“servida” y no confirma la preñez, a la espera de un nuevo control.',
    excepcion='3a. La fecha del tacto es anterior a la del servicio o posterior al día de '
        'hoy: el sistema muestra el error y no guarda. 3b. La fecha del tacto es posterior '
        'a la baja del animal: el sistema informa la situación e impide el registro.',
    postcondicion='El control queda registrado y el estado reproductivo de la hembra '
        'refleja el resultado del último tacto.',
    reglas='El tacto modifica el estado reproductivo y nunca el productivo: una vaca en '
        'lactancia confirmada preñada sigue produciendo. El estado reproductivo vigente '
        'es el que resulta del último tacto del servicio vigente: si un tacto se corrige o '
        'se elimina, el estado se vuelve a deducir.',
    validaciones='La fecha y el resultado del tacto son obligatorios.',
    frecuencia='Media, concentrada habitualmente en días de control veterinario programado.',
),

dict(
    modulo=3, num=23,
    nombre='Consultar Alertas de Parto Próximo',
    actores='Encargada del sector',
    tipo='Primario',
    descripcion='Lista las vacas preñadas que se encuentran próximas a su fecha probable de '
        'parto.',
    rf='RF3.7',
    precondicion='El usuario debe estar logueado.',
    desencadenante='El usuario ingresa a la sección de alertas reproductivas.',
    curso=[
        'El usuario selecciona la opción “Alertas de Parto”.',
        'El sistema evalúa las fechas probables de parto de los servicios con preñez '
        'confirmada.',
        'El sistema selecciona aquellos cuya fecha probable de parto cae dentro de la '
        'anticipación configurada.',
        'El sistema despliega el listado con la fecha probable de cada animal y los días '
        'que restan.',
    ],
    alternativos='—',
    excepcion='—',
    postcondicion='Se exponen las alertas sin alterar datos en la base de datos.',
    reglas='La ventana de anticipación del aviso es un parámetro configurable del '
        'establecimiento. Sólo se consideran animales activos.',
    validaciones='—',
    frecuencia='Alta, de revisión cotidiana para preparar la maternidad.',
),

dict(
    modulo=3, num=24,
    nombre='Registrar Parto',
    actores='Encargada del sector',
    tipo='Primario',
    descripcion='Permite documentar el parto de una vaca, dar de alta las crías nacidas '
        'como animales del rodeo y reactivar el estado productivo de la madre.',
    rf='RF3.8, RF3.9, RF3.11, RF2.11, RF1.9',
    precondicion='El usuario debe estar logueado y la madre debe figurar en el sistema.',
    desencadenante='Ocurre un nacimiento en el establecimiento y el usuario procede a '
        'ingresarlo.',
    curso=[
        'El usuario busca la caravana de la madre.',
        'El usuario selecciona la opción “Registrar Parto”.',
        'El sistema habilita el formulario solicitando la fecha del parto, el tipo y las '
        'observaciones, junto con los datos de cada cría: caravana, sexo, raza y '
        'fotografía.',
        'El sistema propone como padre de las crías el toro del servicio que originó la '
        'preñez.',
        'El usuario completa los datos y presiona “Confirmar Parto”.',
        'El sistema presenta las advertencias que correspondan y solicita confirmación.',
        'El sistema da de alta cada cría como animal del rodeo, enlazando a esta vaca como '
        'su madre.',
        'El sistema cierra la lactancia anterior si seguía abierta, abre la nueva y deja a '
        'la madre en estado productivo “en lactancia” y reproductivo “vacía”.',
        'El sistema incrementa la cantidad de partos de la madre, actualiza su categoría y '
        'confirma la operación.',
    ],
    alternativos='4a. El usuario indica otro padre, o ninguno: el sistema conserva lo '
        'indicado. 5a. El parto no tuvo crías vivas: el usuario registra el parto sin '
        'crías y el sistema aplica igualmente los cambios de estado de la madre. 6a. El '
        'sistema advierte que la gestación duró menos o más de lo normal, que la madre no '
        'figuraba preñada, o que las crías mellizas son de distinto sexo —caso en que la '
        'hembra puede resultar estéril—: el usuario confirma y el registro continúa.',
    excepcion='7a. El número de caravana asignado a una cría ya se encuentra registrado: el '
        'sistema muestra un mensaje de error por duplicado e impide guardar hasta '
        'corregirlo. 3a. La fecha del parto es futura o posterior a la baja de la madre: el '
        'sistema muestra el error y no guarda.',
    postcondicion='Las crías ingresan al rodeo activo. La madre queda con estado productivo '
        '“en lactancia” y reproductivo “vacía”, habilitada nuevamente para los lotes de '
        'ordeñe, y con una lactancia abierta.',
    reglas='El parto actúa sobre los dos ejes a la vez: cierra el ciclo reproductivo '
        'devolviendo la hembra a “vacía” e inicia el ciclo productivo llevándola a “en '
        'lactancia”. Es el único evento que actualiza la categoría del animal sin '
        'intervención del usuario, porque el cambio de condición biológica no admite '
        'ambigüedad. Las crías son animales del rodeo desde el momento del registro y se '
        'corrigen desde el módulo de animales.',
    validaciones='La fecha del parto es obligatoria y no puede ser futura. La caravana, el '
        'sexo y la raza de cada cría son obligatorios, y la caravana debe ser única.',
    frecuencia='Media.',
),

dict(
    modulo=3, num=25,
    nombre='Consultar Listas de Trabajo Reproductivas',
    actores='Encargada del sector',
    tipo='Primario',
    descripcion='Presenta las dos listas que ordenan el trabajo reproductivo de la jornada: '
        'los servicios que esperan tacto y las hembras en condiciones de ser servidas.',
    rf='RF3.12',
    precondicion='El usuario debe estar logueado en el sistema.',
    desencadenante='El usuario planifica el trabajo reproductivo del día.',
    curso=[
        'El usuario accede a “Tactos Pendientes” o a “Vacas para Servir”.',
        'En la primera, el sistema selecciona los servicios vigentes cuya preñez no fue '
        'confirmada ni descartada y sobre los que ya transcurrieron los días para el tacto '
        'configurados.',
        'En la segunda, el sistema selecciona las hembras activas que están en condiciones '
        'de recibir servicio, es decir aquellas que superaron la espera voluntaria '
        'posparto configurada.',
        'El sistema despliega cada lista indicando, para cada animal, el motivo por el cual '
        'figura.',
    ],
    alternativos='4a. El usuario selecciona un animal de la lista: el sistema abre el '
        'registro del evento que corresponde, con el animal ya seleccionado.',
    excepcion='—',
    postcondicion='El usuario conoce el trabajo reproductivo pendiente sin que se altere '
        'ningún registro.',
    reglas='Una hembra está para servir si alcanzó la edad mínima al servicio, no está '
        'preñada y no tiene un servicio a la espera de tacto; si además parió, debe haber '
        'superado la espera voluntaria posparto. Los dos plazos son parámetros '
        'configurables (RF0.3). El motivo se muestra en texto: el sistema informa, no '
        'decide.',
    validaciones='—',
    frecuencia='Alta, de revisión cotidiana.',
),

dict(
    modulo=3, num=26,
    nombre='Corregir o Eliminar Evento Reproductivo',
    actores='Encargada del sector',
    tipo='Primario',
    descripcion='Permite corregir o eliminar un celo, un servicio, un tacto o un parto ya '
        'registrado, volviendo a deducir el estado reproductivo del animal a partir de los '
        'eventos que permanecen.',
    rf='RF3.13',
    precondicion='El usuario debe estar logueado y el evento a corregir debe existir.',
    desencadenante='El usuario advierte un error en un evento reproductivo ya guardado.',
    curso=[
        'El usuario localiza el evento en el listado del módulo o en la ficha del animal.',
        'El usuario selecciona “Editar” o “Eliminar”.',
        'Al editar, el sistema despliega la pantalla de registro con los datos cargados y '
        'aplica las mismas validaciones y advertencias.',
        'Al eliminar, el sistema verifica que ningún otro registro dependa de éste.',
        'El sistema aplica el cambio, vuelve a deducir el estado reproductivo del animal y '
        'confirma la operación.',
    ],
    alternativos='2a. El evento es un parto: la corrección se realiza en una pantalla '
        'propia, que alcanza la fecha, el tipo y las observaciones. Las crías ya son '
        'animales del rodeo y se corrigen desde el módulo de animales. 5a. El evento '
        'eliminado había descontado una pajuela del stock: el sistema la devuelve mediante '
        'un contra-movimiento.',
    excepcion='4a. El evento tiene otros que dependen de él —un servicio con tactos '
        'registrados, un parto con crías o con controles de producción imputados a su '
        'lactancia—: el sistema informa cuáles son y qué debe eliminarse primero, sin '
        'borrar nada.',
    postcondicion='El evento queda corregido o eliminado y el estado reproductivo del '
        'animal es coherente con los eventos que permanecen registrados.',
    reglas='El estado reproductivo no se deshace paso a paso: se vuelve a deducir del '
        'servicio vigente y de su último tacto. Es lo que evita que una secuencia de '
        'correcciones deje al animal en un estado que no corresponde a ningún evento real.',
    validaciones='Las mismas del registro original.',
    frecuencia='Baja, ante errores de carga.',
),

]
