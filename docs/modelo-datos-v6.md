# 2.2.5 Modelo de Datos — v6

Generado desde `bd/CreacionDb.sql` por `modelo_datos.py`. No editar a mano.

## 2.2.5.2 Normalización

- **razas** = {id_raza, nombre, descripcion}
- **categorias** = {id_categoria, nombre, descripcion}
- **animales** = {id_animal, num_caravana, fecha_nacimiento, activo, fecha_baja, motivo_baja, foto, id_raza, id_categoria, id_madre, id_padre}
- **hembras** = {id_animal, numero_partos, estado_productivo, estado_reproductivo}
- **machos** = {id_animal, en_pie}
- **insumos** = {id_insumo, nombre, tipo_insumo, stock_actual, stock_minimo, periodo_descarte_dias, id_macho}
- **movimientos_stock** = {id_movimiento, tipo_movimiento, cantidad, fecha, fecha_vencimiento, motivo, id_insumo}
- **lactancias** = {id_lactancia, numero_lactancia, fecha_inicio, fecha_secado, fecha_probable_parto, id_animal}
- **ordenies_lote** = {id_ordenie_lote, fecha, turno, litros_totales}
- **ordenie_lote_animales** = {id_ordenie_lote, id_animal}
- **ordenies_individual** = {id_ordenie_ind, fecha, turno, litros, id_animal, id_lactancia, id_ordenie_lote}
- **celos** = {id_celo, fecha, observaciones, id_animal}
- **servicios** = {id_servicio, tipo_servicio, fecha_servicio, fecha_probable_parto, observaciones, id_animal, id_macho, id_insumo}
- **tactos** = {id_tacto, fecha_tacto, resultado, observaciones, id_servicio}
- **partos** = {id_parto, fecha_parto, tipo_parto, observaciones, id_animal}
- **planes_sanitarios** = {id_plan, nombre, tipo_procedimiento, periodicidad_dias, edad_inicio_meses, activo, id_insumo}
- **plan_categorias** = {id_plan, id_categoria}
- **diagnosticos** = {id_diagnostico, fecha_diagnostico, enfermedad, estado, id_animal}
- **tratamientos** = {id_tratamiento, fecha_inicio, dias_duracion, dosis_diaria, cantidad_insumo, id_animal, fecha_fin_descarte, id_diagnostico, id_insumo, id_plan}
- **vacunaciones** = {id_vacunacion, fecha_aplicacion, id_animal, id_insumo, id_plan}
- **descornes** = {id_descorne, fecha, metodo, observaciones, id_animal, id_plan}
- **configuracion** = {id_configuracion, dias_secado_antes_parto, edad_minima_servicio_meses, edad_cambio_categoria_meses, litros_maximos_individual, ordenies_por_dia, dias_anticipacion_secado, dias_anticipacion_parto, dias_anticipacion_sanitaria, dias_anticipacion_vencimiento, dias_espera_voluntaria, dias_para_tacto, hora_resumen, chat_telegram, fecha_ultimo_resumen}
- **preferencias_notificacion** = {id_preferencia, tipo_alerta, activa}
- **alertas** = {id_alerta, tipo_alerta, fecha_generacion, mensaje, enviada, id_preferencia, id_animal, id_insumo}

## 2.2.5.3 Tabla de Claves

| Tabla | Claves primarias | Claves alternas | Claves foráneas |
|---|---|---|---|
| razas | id_raza | nombre | — |
| categorias | id_categoria | nombre | — |
| animales | id_animal | num_caravana | id_raza, id_categoria, id_madre, id_padre |
| hembras | id_animal | — | id_animal |
| machos | id_animal | — | id_animal |
| insumos | id_insumo | — | id_macho |
| movimientos_stock | id_movimiento | — | id_insumo |
| lactancias | id_lactancia | — | id_animal |
| ordenies_lote | id_ordenie_lote | fecha, turno | — |
| ordenie_lote_animales | id_ordenie_lote, id_animal | — | id_ordenie_lote, id_animal |
| ordenies_individual | id_ordenie_ind | fecha, turno, id_animal | id_animal, id_lactancia, id_ordenie_lote |
| celos | id_celo | — | id_animal |
| servicios | id_servicio | — | id_animal, id_macho, id_insumo |
| tactos | id_tacto | — | id_servicio |
| partos | id_parto | — | id_animal |
| planes_sanitarios | id_plan | nombre | id_insumo |
| plan_categorias | id_plan, id_categoria | — | id_plan, id_categoria |
| diagnosticos | id_diagnostico | — | id_animal |
| tratamientos | id_tratamiento | — | id_animal, id_diagnostico, id_insumo, id_plan |
| vacunaciones | id_vacunacion | — | id_animal, id_insumo, id_plan |
| descornes | id_descorne | — | id_animal, id_plan |
| configuracion | id_configuracion | — | — |
| preferencias_notificacion | id_preferencia | tipo_alerta | — |
| alertas | id_alerta | — | id_preferencia, id_animal, id_insumo |

## 2.2.5.4 Restricciones de Integridad


### Tabla: razas

| Campo | Tipo | Restricciones | Observaciones |
|---|---|---|---|
| id_raza | INT(11) | PK, Auto increment, No nulo |  |
| nombre | VARCHAR(60) | Único, No nulo |  |
| descripcion | VARCHAR(200) | Nulo |  |

### Tabla: categorias

| Campo | Tipo | Restricciones | Observaciones |
|---|---|---|---|
| id_categoria | INT(11) | PK, Auto increment, No nulo |  |
| nombre | VARCHAR(60) | Único, No nulo |  |
| descripcion | VARCHAR(200) | Nulo |  |

### Tabla: animales

| Campo | Tipo | Restricciones | Observaciones |
|---|---|---|---|
| id_animal | INT(11) | PK, Auto increment, No nulo |  |
| num_caravana | VARCHAR(20) | Único, No nulo |  |
| fecha_nacimiento | DATE | No nulo |  |
| activo | TINYINT(1) | No nulo | Marca la baja lógica. La baja nunca elimina la fila: el historial y el linaje de los descendientes dependen de ella. |
| fecha_baja | DATE | Nulo |  |
| motivo_baja | VARCHAR(100) | Nulo |  |
| foto | VARCHAR(120) | Nulo | Nombre del archivo dentro de wwwroot. La imagen no se guarda en la base. |
| id_raza | INT(11) | FK → razas, No nulo |  |
| id_categoria | INT(11) | FK → categorias, No nulo | Valor propuesto por el sistema a partir del sexo, la edad y la cantidad de partos, ajustable por el usuario. |
| id_madre | INT(11) | FK → hembras, Nulo | Clave foránea recursiva hacia hembras. Admite nulo cuando el progenitor no está registrado, que es el caso de la carga inicial del rodeo. |
| id_padre | INT(11) | FK → machos, Nulo | Clave foránea recursiva hacia machos. Admite nulo por el mismo motivo que id_madre. |

### Tabla: hembras

| Campo | Tipo | Restricciones | Observaciones |
|---|---|---|---|
| id_animal | INT(11) | PK, FK → animales, Auto increment, No nulo | Comparte la clave primaria con animales: así se resuelve la especialización. |
| numero_partos | INT(11) | No nulo |  |
| estado_productivo | VARCHAR(20) | No nulo |  |
| estado_reproductivo | VARCHAR(20) | No nulo |  |

### Tabla: machos

| Campo | Tipo | Restricciones | Observaciones |
|---|---|---|---|
| id_animal | INT(11) | PK, FK → animales, Auto increment, No nulo | Comparte la clave primaria con animales: así se resuelve la especialización. |
| en_pie | TINYINT(1) | No nulo | Vale falso en el toro de catálogo, que aporta material genético sin integrar el rodeo. |

### Tabla: insumos

| Campo | Tipo | Restricciones | Observaciones |
|---|---|---|---|
| id_insumo | INT(11) | PK, Auto increment, No nulo |  |
| nombre | VARCHAR(100) | No nulo |  |
| tipo_insumo | VARCHAR(30) | No nulo |  |
| stock_actual | DECIMAL(10,2) | No nulo | Resulta de la suma de los movimientos registrados. |
| stock_minimo | DECIMAL(10,2) | No nulo |  |
| periodo_descarte_dias | INT(11) | Nulo | Días de carencia del producto. Nulo en los insumos que no obligan a descartar leche. |
| id_macho | INT(11) | FK → machos, Nulo | Sólo en las pajuelas: vincula la dosis con el toro que la aporta, y es lo que permite reconstruir la genealogía de la cría. |

### Tabla: movimientos_stock

| Campo | Tipo | Restricciones | Observaciones |
|---|---|---|---|
| id_movimiento | INT(11) | PK, Auto increment, No nulo |  |
| tipo_movimiento | VARCHAR(20) | No nulo |  |
| cantidad | DECIMAL(10,2) | No nulo |  |
| fecha | DATE | No nulo |  |
| fecha_vencimiento | DATE | Nulo | El vencimiento se registra por partida y no por insumo: un mismo insumo ingresa en partidas con vencimientos distintos. |
| motivo | VARCHAR(100) | Nulo |  |
| id_insumo | INT(11) | FK → insumos, No nulo |  |

### Tabla: lactancias

| Campo | Tipo | Restricciones | Observaciones |
|---|---|---|---|
| id_lactancia | INT(11) | PK, Auto increment, No nulo |  |
| numero_lactancia | INT(11) | No nulo |  |
| fecha_inicio | DATE | No nulo |  |
| fecha_secado | DATE | Nulo | Nulo mientras la lactancia está en curso: ese nulo es lo que la identifica como abierta. |
| fecha_probable_parto | DATE | Nulo |  |
| id_animal | INT(11) | FK → hembras, No nulo |  |

### Tabla: ordenies_lote

| Campo | Tipo | Restricciones | Observaciones |
|---|---|---|---|
| id_ordenie_lote | INT(11) | PK, Auto increment, No nulo |  |
| fecha | DATE | Único, No nulo |  |
| turno | VARCHAR(10) | Único, No nulo |  |
| litros_totales | DECIMAL(8,2) | No nulo | La leche completa del turno, tal como se lee del tanque, incluida la de los animales que además se midieron uno por uno. |

### Tabla: ordenie_lote_animales

| Campo | Tipo | Restricciones | Observaciones |
|---|---|---|---|
| id_ordenie_lote | INT(11) | PK, FK → ordenies_lote, Clave compuesta, No nulo |  |
| id_animal | INT(11) | PK, FK → hembras, Clave compuesta, No nulo | Deja asentado qué animales integraron el lote del turno, que es lo que el usuario ajusta en el paso 4 de CU12. |

### Tabla: ordenies_individual

| Campo | Tipo | Restricciones | Observaciones |
|---|---|---|---|
| id_ordenie_ind | INT(11) | PK, Auto increment, No nulo |  |
| fecha | DATE | Único, No nulo |  |
| turno | VARCHAR(10) | Único, No nulo |  |
| litros | DECIMAL(6,2) | No nulo |  |
| id_animal | INT(11) | FK → hembras, Único, No nulo |  |
| id_lactancia | INT(11) | FK → lactancias, No nulo |  |
| id_ordenie_lote | INT(11) | FK → ordenies_lote, Nulo | Admite nulo: el control individual no exige que el total del turno esté cargado. Cuando falta, la producción de ese turno es la suma de los controles. |

### Tabla: celos

| Campo | Tipo | Restricciones | Observaciones |
|---|---|---|---|
| id_celo | INT(11) | PK, Auto increment, No nulo |  |
| fecha | DATE | No nulo |  |
| observaciones | VARCHAR(200) | Nulo |  |
| id_animal | INT(11) | FK → hembras, No nulo |  |

### Tabla: servicios

| Campo | Tipo | Restricciones | Observaciones |
|---|---|---|---|
| id_servicio | INT(11) | PK, Auto increment, No nulo |  |
| tipo_servicio | VARCHAR(20) | No nulo |  |
| fecha_servicio | DATE | No nulo |  |
| fecha_probable_parto | DATE | No nulo | Calculada a partir de la fecha del servicio y ajustable por el usuario. |
| observaciones | VARCHAR(200) | Nulo |  |
| id_animal | INT(11) | FK → hembras, No nulo |  |
| id_macho | INT(11) | FK → machos, Nulo | Toro de la monta natural. Mutuamente excluyente con id_insumo. |
| id_insumo | INT(11) | FK → insumos, Nulo | Pajuela de la inseminación artificial. Mutuamente excluyente con id_macho. La exclusión se controla en la Controladora, porque MySQL no admite un CHECK sobre columnas de otras tablas. |

### Tabla: tactos

| Campo | Tipo | Restricciones | Observaciones |
|---|---|---|---|
| id_tacto | INT(11) | PK, Auto increment, No nulo |  |
| fecha_tacto | DATE | No nulo |  |
| resultado | VARCHAR(20) | No nulo |  |
| observaciones | VARCHAR(200) | Nulo |  |
| id_servicio | INT(11) | FK → servicios, No nulo | El tacto cuelga del servicio y no de la hembra: es el servicio lo que viene a confirmar. |

### Tabla: partos

| Campo | Tipo | Restricciones | Observaciones |
|---|---|---|---|
| id_parto | INT(11) | PK, Auto increment, No nulo |  |
| fecha_parto | DATE | No nulo |  |
| tipo_parto | VARCHAR(30) | No nulo |  |
| observaciones | VARCHAR(200) | Nulo |  |
| id_animal | INT(11) | FK → hembras, No nulo |  |

### Tabla: planes_sanitarios

| Campo | Tipo | Restricciones | Observaciones |
|---|---|---|---|
| id_plan | INT(11) | PK, Auto increment, No nulo |  |
| nombre | VARCHAR(60) | Único, No nulo |  |
| tipo_procedimiento | VARCHAR(20) | No nulo |  |
| periodicidad_dias | INT(11) | Nulo | Nulo indica que el procedimiento se aplica una única vez en la vida del animal, como el descorne. |
| edad_inicio_meses | INT(11) | No nulo |  |
| activo | TINYINT(1) | No nulo |  |
| id_insumo | INT(11) | FK → insumos, Nulo | Nulo en los planes de descorne, que no consumen insumo. |

### Tabla: plan_categorias

| Campo | Tipo | Restricciones | Observaciones |
|---|---|---|---|
| id_plan | INT(11) | PK, FK → planes_sanitarios, Clave compuesta, No nulo |  |
| id_categoria | INT(11) | PK, FK → categorias, Clave compuesta, No nulo | Un plan sin filas en esta tabla alcanza a todo el rodeo: la ausencia de fila es información, no un dato faltante. |

### Tabla: diagnosticos

| Campo | Tipo | Restricciones | Observaciones |
|---|---|---|---|
| id_diagnostico | INT(11) | PK, Auto increment, No nulo |  |
| fecha_diagnostico | DATE | No nulo |  |
| enfermedad | VARCHAR(100) | No nulo |  |
| estado | VARCHAR(20) | No nulo | Distingue el cuadro activo del cerrado (CU33). |
| id_animal | INT(11) | FK → animales, No nulo |  |

### Tabla: tratamientos

| Campo | Tipo | Restricciones | Observaciones |
|---|---|---|---|
| id_tratamiento | INT(11) | PK, Auto increment, No nulo |  |
| fecha_inicio | DATE | No nulo |  |
| dias_duracion | INT(11) | No nulo |  |
| dosis_diaria | VARCHAR(60) | No nulo |  |
| cantidad_insumo | DECIMAL(10,2) | No nulo | Cuánto producto consumió la aplicación. Es lo que permite devolver la cantidad exacta al stock si el tratamiento se corrige o se elimina. |
| id_animal | INT(11) | FK → animales, Nulo | Permite atribuir el tratamiento preventivo a un animal, sin lo cual no generaría descarte de leche para nadie. |
| fecha_fin_descarte | DATE | Nulo | Calculada sumando los días de duración y el período de carencia del insumo, y ajustable por el usuario. |
| id_diagnostico | INT(11) | FK → diagnosticos, Nulo | Nulo identifica al tratamiento preventivo, como la desparasitación, que no se origina en un diagnóstico. |
| id_insumo | INT(11) | FK → insumos, No nulo |  |
| id_plan | INT(11) | FK → planes_sanitarios, Nulo |  |

### Tabla: vacunaciones

| Campo | Tipo | Restricciones | Observaciones |
|---|---|---|---|
| id_vacunacion | INT(11) | PK, Auto increment, No nulo |  |
| fecha_aplicacion | DATE | No nulo |  |
| id_animal | INT(11) | FK → animales, No nulo |  |
| id_insumo | INT(11) | FK → insumos, No nulo |  |
| id_plan | INT(11) | FK → planes_sanitarios, Nulo | Declara explícitamente qué plan da por cumplido la aplicación: el calendario no lo infiere del insumo, porque dos planes pueden usar la misma vacuna. |

### Tabla: descornes

| Campo | Tipo | Restricciones | Observaciones |
|---|---|---|---|
| id_descorne | INT(11) | PK, Auto increment, No nulo |  |
| fecha | DATE | No nulo |  |
| metodo | VARCHAR(60) | No nulo |  |
| observaciones | VARCHAR(200) | Nulo |  |
| id_animal | INT(11) | FK → animales, No nulo |  |
| id_plan | INT(11) | FK → planes_sanitarios, Nulo | Ídem vacunaciones. El descorne es de aplicación única. |

### Tabla: configuracion

| Campo | Tipo | Restricciones | Observaciones |
|---|---|---|---|
| id_configuracion | INT(11) | PK, Auto increment, No nulo | Tabla de una sola fila: el sistema lee siempre la primera y escribe sobre ella. No hay alta de configuraciones. |
| dias_secado_antes_parto | INT(11) | No nulo |  |
| edad_minima_servicio_meses | INT(11) | No nulo |  |
| edad_cambio_categoria_meses | INT(11) | No nulo |  |
| litros_maximos_individual | DECIMAL(6,2) | No nulo |  |
| ordenies_por_dia | INT(11) | No nulo |  |
| dias_anticipacion_secado | INT(11) | No nulo |  |
| dias_anticipacion_parto | INT(11) | No nulo |  |
| dias_anticipacion_sanitaria | INT(11) | No nulo |  |
| dias_anticipacion_vencimiento | INT(11) | No nulo |  |
| dias_espera_voluntaria | INT(11) | No nulo |  |
| dias_para_tacto | INT(11) | No nulo |  |
| hora_resumen | TIME | No nulo | Hora a la que sale el resumen diario de Telegram. Es un parámetro de manejo como los demás: hay una sola para todo el establecimiento. |
| chat_telegram | VARCHAR(40) | Nulo | Destinatario único de los avisos. Nulo mientras nadie haya vinculado una cuenta, y ese nulo es lo que el sistema lee como integración sin configurar. El token del bot no está en la base: es una credencial y vive en la configuración de la aplicación. |
| fecha_ultimo_resumen | DATE | Nulo | Día en que salió el último resumen. Evita el mensaje repetido cuando el sitio se reinicia: la tabla de alertas no puede responder por un día sin pendientes, porque ese día no genera filas. |

### Tabla: preferencias_notificacion

| Campo | Tipo | Restricciones | Observaciones |
|---|---|---|---|
| id_preferencia | INT(11) | PK, Auto increment, No nulo |  |
| tipo_alerta | VARCHAR(40) | Único, No nulo | Los ocho tipos se cargan con el esquema y el sistema no da de alta ninguno: son los ocho contadores del tablero de inicio, y que la lista sea cerrada es lo que garantiza que el resumen y las pantallas no puedan discrepar. |
| activa | TINYINT(1) | No nulo | Un aviso apagado deja de enviarse por Telegram y se sigue viendo en el sistema. |

### Tabla: alertas

| Campo | Tipo | Restricciones | Observaciones |
|---|---|---|---|
| id_alerta | INT(11) | PK, Auto increment, No nulo |  |
| tipo_alerta | VARCHAR(40) | No nulo |  |
| fecha_generacion | DATE | No nulo | Un pendiente que no se resuelve genera su fila otra vez al día siguiente: el resumen es la lista de tareas del día, no un aviso de novedades. |
| mensaje | VARCHAR(200) | No nulo | El renglón tal como se envió. Se guarda armado para que el historial no dependa de que el cálculo siga dando lo mismo meses después. |
| enviada | TINYINT(1) | No nulo |  |
| id_preferencia | INT(11) | FK → preferencias_notificacion, No nulo |  |
| id_animal | INT(11) | FK → animales, Nulo | Nulo según el tipo de alerta: unas se originan en un animal y otras en un insumo. |
| id_insumo | INT(11) | FK → insumos, Nulo | Nulo por el mismo motivo que id_animal. |
