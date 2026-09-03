# Inventario de pantallas

Generado por `docs/inventario_pantallas.py` leyendo `Tesis/Pages`. No editar a mano.

Es la fuente de la sección 2.4, el Manual de Usuario: de cada pantalla salen su
título, sus campos, sus botones y los mensajes con que rechaza o advierte.

## Modulo 0 - Seguridad y acceso

### Login

- **Título:** Iniciar sesión
- **Campos:** Usuario, Contraseña
- **Acciones:** Ingresar
- **Mensajes al usuario:**
  - Usuario o contraseña incorrectos!

## Modulo 0 - Configuracion

### Configuracion

- **Título:** Configuracion del Establecimiento
- **Campos:** Dias de secado antes del parto, Edad minima al servicio (meses), Edad de cambio de categoria (meses), Ordeñes por dia, Litros maximos por control individual, Espera voluntaria posparto (dias), Dias para el tacto, Secado proximo (dias), Parto proximo (dias), Calendario sanitario (dias), Vencimiento de insumos (dias)
- **Acciones:** Guardar
- **Mensajes al usuario:**
  - No se pudo guardar la configuracion. Verifique que la tabla de configuracion tenga su fila inicial!

## Modulo 1 - Animales y genetica

### AltaAnimal

- **Título:** Agregar Animal
- **Campos:** Numero de Caravana, Fecha de Nacimiento, Sexo, Raza, Madre, Padre, Partos Registrados, En pie (integra el rodeo como reproductor), Categoria
- **Acciones:** Buscar, Calcular Categoria, Guardar de todos modos, Agregar
- **Mensajes al usuario:**
  - El número de caravana y la raza son obligatorios!
  - La fecha de nacimiento no puede ser futura!
  - El número de caravana ya existe en el sistema!
  - No se pudo registrar el animal!

### BajaAnimal

- **Título:** Registrar Baja de Animal
- **Campos:** Motivo de Salida
- **Acciones:** Confirmar Baja
- **Mensajes al usuario:**
  - No se pudo dar de baja el animal. Verifique que siga activo!

### BuscarAnimales

- **Título:** Buscar y Filtrar Animales del Rodeo
- **Campos:** Estado, Categoria, Raza, Edad en meses, Busquedas rapidas, Numero de Caravana
- **Acciones:** Rodeo activo, Todos los inactivos, Crias (0 a 12 meses), Recria (13 a 24 meses), Vacas, Toros, Todo el historico, Elegir de la lista, Aplicar filtros, Limpiar
- **Mensajes al usuario:**
  - El rango etario es incorrecto: la edad desde no puede superar a la edad hasta!
  - No se encontraron animales con los criterios ingresados!

### ConsultaLinaje

- **Título:** Consultar Linaje y Registro Genealogico
- **Campos:** Animal
- **Acciones:** Buscar, Ver Linaje, Desplegar las @Model.generaciones generaciones, Contraer, −, 100%, +, Ver como tabla, Volver a @Model.animal.NumCaravana, Centrar el árbol aquí
- **Mensajes al usuario:**
  - Seleccione un animal!

### DetalleAnimal

- **Título:** Detalle de Animal
- **Campos:** —
- **Acciones:** Actualizar a @Model.categoriaSugerida, Reactivar animal

### ListaAnimales

- **Título:** Lista de Animales
- **Campos:** —
- **Acciones:** Actualizar Categoria

### ModificarAnimal

- **Título:** Modificar Animal
- **Campos:** ID, Numero de Caravana, Fecha de Nacimiento, Sexo, Raza, Madre, Padre, Partos Registrados, En pie (integra el rodeo como reproductor), Categoria
- **Acciones:** Buscar, Calcular Categoria, Guardar de todos modos, Guardar Cambios
- **Mensajes al usuario:**
  - El número de caravana, la raza y la categoría son obligatorios!
  - La fecha de nacimiento no puede ser futura!
  - No se pudo modificar el animal. Verifique que el número de caravana no esté repetido!

### VerificarConsanguinidad

- **Título:** Verificar Consanguinidad
- **Campos:** Hembra, Reproductor
- **Acciones:** Buscar, Verificar
- **Mensajes al usuario:**
  - Seleccione la hembra y el reproductor!
  - No puede verificar un animal contra sí mismo!

## Modulo 2 - Produccion

### AlertasSecado

- **Título:** Alertas de Secado Proximo
- **Campos:** —
- **Acciones:** —

### AltaLactancia

- **Título:** Abrir Lactancia
- **Campos:** Caravana, Numero de lactancia, Fecha de inicio
- **Acciones:** Buscar, Proponer, Abrir Lactancia
- **Mensajes al usuario:**
  - Seleccione un animal!
  - La caravana no corresponde a una hembra del rodeo!
  - El animal ya tiene una lactancia abierta!
  - La fecha de inicio no puede ser futura!
  - La fecha de inicio no puede ser anterior al nacimiento del animal!
  - No se pudo abrir la lactancia!

### ControlLechero

- **Título:** Control Lechero
- **Campos:** Fecha del control, Turno, Buscar caravana en la lista
- **Acciones:** Guardar controles
- **Mensajes al usuario:**
  - La fecha del control no puede ser futura!
  - Seleccione el turno!
  - Cargue los litros de al menos un animal!
  - No se guardo ningun control. Revise que los animales sigan en ordeñe y que no tengan ya un control en esa fecha y turno!

### HistorialProduccion

- **Título:** Historial de Produccion
- **Campos:** Modalidad, Desde, Hasta, &nbsp;
- **Acciones:** Buscar
- **Mensajes al usuario:**
  - No se pudo eliminar el control individual!
  - No se pudo eliminar el ordeñe del lote!
  - El rango de fechas es invalido: la fecha desde es posterior a la fecha hasta!
  - Seleccione la modalidad de visualizacion!

### ListaLactancias

- **Título:** Lactancias
- **Campos:** —
- **Acciones:** —

### MetricaMensual

- **Título:** Metrica de Produccion Mensual
- **Campos:** Mes, Año, &nbsp;
- **Acciones:** Consultar
- **Mensajes al usuario:**
  - Seleccione un mes y un año validos!

### ModificarOrdenieLote

- **Título:** Corregir Ordeñe del Lote
- **Campos:** Litros del ordeñe masivo
- **Acciones:** Guardar Cambios
- **Mensajes al usuario:**
  - No se encontro el ordeñe solicitado.
  - El lote tiene que tener al menos un animal!
  - No se pudo modificar el ordeñe del lote!

### OrdenieIndividual

- **Título:** Corregir Control Individual
- **Campos:** Turno, Fecha, Caravana, Litros
- **Acciones:** Buscar, @(Model.esCorreccion ? "Guardar corrección" : "Guardar")
- **Mensajes al usuario:**
  - Seleccione un animal!
  - La caravana no corresponde a una hembra del rodeo!
  - Los litros tienen que ser un valor positivo y coherente!
  - La fecha del control no puede ser futura!
  - El animal no tenia una lactancia abierta en esa fecha. Registrela desde Lactancias antes de cargar el control.
  - No se pudo registrar el control individual!
  - El control que se quiere corregir ya no existe!
  - No se pudo corregir el control individual!

### OrdenieLote

- **Título:** Registrar Ordeñe General (Por Lote)
- **Campos:** Turno, Fecha, Litros del ordeñe
- **Acciones:** Guardar
- **Mensajes al usuario:**
  - Seleccione el turno!
  - La fecha del ordeñe no puede ser futura!
  - Los litros tienen que ser un valor positivo y coherente!
  - Ya hay un ordeñe registrado para esa fecha y ese turno. Para corregirlo, edítelo desde el historial.
  - El lote tiene que tener al menos un animal!
  - No se pudo registrar el ordeñe del lote!

### RegistrarSecado

- **Título:** Registrar Periodo de Secado
- **Campos:** Caravana, Fecha de secado
- **Acciones:** Buscar, Registrar Secado
- **Mensajes al usuario:**
  - Seleccione un animal!
  - La caravana no corresponde a una hembra del rodeo!
  - El animal no se encuentra en lactancia, asi que no hay nada que secar!
  - La fecha de secado no puede ser futura!
  - No se pudo registrar el secado. Verifique que el animal tenga una lactancia abierta y que la fecha no sea anterior a su inicio.

## Modulo 3 - Reproduccion

### AlertasParto

- **Título:** Alertas de Parto Proximo
- **Campos:** —
- **Acciones:** —

### ListaCelos

- **Título:** Celos
- **Campos:** —
- **Acciones:** —
- **Mensajes al usuario:**
  - No se pudo eliminar el celo!

### ListaPartos

- **Título:** Partos
- **Campos:** —
- **Acciones:** —
- **Mensajes al usuario:**
  - No se pudo eliminar el parto!

### ListaServicios

- **Título:** Servicios
- **Campos:** —
- **Acciones:** Ajustar
- **Mensajes al usuario:**
  - Indique la fecha probable de parto!
  - No se pudo ajustar la fecha: tiene que ser posterior a la fecha del servicio.
  - No se pudo eliminar el servicio!

### ListaTactos

- **Título:** Tactos
- **Campos:** —
- **Acciones:** —
- **Mensajes al usuario:**
  - No se pudo eliminar el tacto!

### ModificarParto

- **Título:** Corregir parto
- **Campos:** Caravana de la madre, Fecha del parto, Tipo de parto, Observaciones, Crias de este parto
- **Acciones:** Guardar cambios
- **Mensajes al usuario:**
  - No se pudo corregir el parto!

### RegistrarCelo

- **Título:** @(Model.esCorreccion ? "Corregir Deteccion de Celo" : "Registrar Deteccion de Celo")
- **Campos:** Caravana, Fecha de deteccion, Observaciones
- **Acciones:** Buscar, @(Model.esCorreccion ? "Guardar cambios" : "Guardar")
- **Mensajes al usuario:**
  - Seleccione un animal!
  - La caravana no existe en el sistema!
  - La caravana corresponde a un macho: no se puede registrar un celo!
  - No se pudo registrar el celo!
  - No se pudo corregir el celo!

### RegistrarParto

- **Título:** Registrar Parto
- **Campos:** Caravana, Fecha del parto, Tipo de parto, Observaciones del parto, Numero de caravana, Sexo, Raza, Padre, Parto doble (mellizos) &mdash; suma un solo parto y una
                sola lactancia a la madre, pero da de alta las dos crías., Caravana de la segunda cría
- **Acciones:** Buscar, Cargar datos, Cambiar, Confirmar de todos modos, Confirmar Parto
- **Mensajes al usuario:**
  - La caravana no corresponde a una hembra del rodeo!
  - Seleccione la madre!
  - El animal figura dado de baja: no se le puede registrar un parto.
  - El numero de caravana de la cria es obligatorio!
  - El numero de caravana de la cria ya existe en el sistema!
  - La raza de la cria es obligatoria!
  - El numero de caravana de la segunda cria es obligatorio!
  - El numero de caravana de la segunda cria ya existe en el sistema!
  - Las dos crias no pueden llevar la misma caravana!
  - La raza de la segunda cria es obligatoria!
  - La fecha del parto no puede ser futura!
  - La fecha del parto no puede ser anterior al nacimiento de la madre!
  - No se pudo registrar el parto!

### RegistrarServicio

- **Título:** @(Model.esCorreccion ? "Corregir Servicio" : "Registrar Servicio")
- **Campos:** Caravana de la hembra, Fecha del servicio, Tipo de servicio, Toro del rodeo, Pajuela del stock, Fecha probable de parto, Observaciones
- **Acciones:** Buscar, Recalcular, Registrar de todos modos, @(Model.esCorreccion ? "Guardar cambios" : "Guardar")
- **Mensajes al usuario:**
  - Seleccione la hembra que recibe el servicio!
  - La caravana no corresponde a una hembra del rodeo!
  - No se pudo registrar el servicio!
  - No se pudo corregir el servicio!

### RegistrarTacto

- **Título:** @(Model.esCorreccion ? "Corregir Tacto" : "Registrar Tacto y Confirmacion de Preñez")
- **Campos:** Caravana, Fecha del tacto, Resultado, Observaciones
- **Acciones:** Buscar, Ver servicio, @(Model.esCorreccion ? "Guardar cambios" : "Guardar")
- **Mensajes al usuario:**
  - El animal no tiene un servicio pendiente: hay que registrar el servicio antes del tacto.
  - Seleccione un animal!
  - El animal no tiene un servicio pendiente sobre el cual registrar el tacto!
  - Es obligatorio definir un resultado para el tacto!
  - La fecha del tacto no puede ser futura!
  - No se pudo registrar el tacto!
  - No se pudo corregir el tacto!

### TactosPendientes

- **Título:** Tactos Pendientes
- **Campos:** —
- **Acciones:** —

### VacasParaServir

- **Título:** Vacas para Servir
- **Campos:** —
- **Acciones:** —

## Modulo 4 - Sanidad

### CalendarioSanitario

- **Título:** Calendario Sanitario
- **Campos:** Horizonte (dias), Procedimiento, Categoria, &nbsp;
- **Acciones:** Buscar
- **Mensajes al usuario:**
  - El horizonte de anticipacion tiene que ser un numero positivo de dias!

### ConfigurarPlan

- **Título:** @(Model.idPlan == 0 ? "Nuevo Plan Sanitario" : "Modificar Plan Sanitario")
- **Campos:** Nombre del plan, Tipo de procedimiento, Insumo a aplicar, Periodicidad (dias), Edad de inicio (meses), Plan activo, Categorias alcanzadas, @categoria.Nombre
- **Acciones:** Guardar
- **Mensajes al usuario:**
  - No se pudo guardar el plan sanitario!

### ListaDescornes

- **Título:** Descornes
- **Campos:** —
- **Acciones:** —
- **Mensajes al usuario:**
  - No se pudo eliminar el descorne!

### ListaDiagnosticos

- **Título:** Diagnósticos
- **Campos:** —
- **Acciones:** Marcar resuelto
- **Mensajes al usuario:**
  - No se pudo marcar el diagnostico como resuelto!
  - No se pudo eliminar el diagnostico!

### ListaPlanes

- **Título:** Planes Sanitarios
- **Campos:** —
- **Acciones:** —

### ListaTratamientos

- **Título:** Tratamientos
- **Campos:** —
- **Acciones:** —
- **Mensajes al usuario:**
  - No se pudo eliminar el tratamiento!

### ListaVacunaciones

- **Título:** Vacunaciones
- **Campos:** —
- **Acciones:** —
- **Mensajes al usuario:**
  - No se pudo eliminar la vacunacion!

### RegistrarDescorne

- **Título:** @(Model.esCorreccion ? "Corregir Descorne" : "Registrar Descorne")
- **Campos:** Caravana, Fecha del procedimiento, Metodo utilizado, Plan sanitario que cumple, Observaciones
- **Acciones:** Buscar, @(Model.esCorreccion ? "Guardar cambios" : "Confirmar")
- **Mensajes al usuario:**
  - Seleccione un animal!
  - La caravana no existe en el sistema!
  - La fecha del procedimiento no puede ser futura!
  - El animal ya tiene un descorne registrado: es un procedimiento de aplicacion unica.
  - No se pudo registrar el descorne!
  - No se pudo corregir el descorne!

### RegistrarDiagnostico

- **Título:** @(Model.esCorreccion ? "Corregir Diagnostico" : "Registrar Diagnostico")
- **Campos:** Caravana, Fecha, Estado, Enfermedad o resultado de la revisacion
- **Acciones:** Buscar, @(Model.esCorreccion ? "Guardar cambios" : "Guardar")
- **Mensajes al usuario:**
  - Seleccione un animal!
  - La caravana no existe en el sistema!
  - La enfermedad o el resultado de la revisacion es obligatorio!
  - La fecha del diagnostico no puede ser futura!
  - No se pudo registrar el diagnostico!
  - No se pudo corregir el diagnostico!

### RegistrarTratamiento

- **Título:** @(Model.esCorreccion ? "Corregir Tratamiento Sanitario" : "Registrar Tratamiento Sanitario")
- **Campos:** Diagnostico que lo origina, Caravana (tratamiento preventivo), Producto aplicado, Fecha de inicio, Duracion (dias), Dosis diaria, Unidades a descontar, Plan sanitario que cumple, Descarte de leche hasta
- **Acciones:** Buscar, Calcular, @(Model.esCorreccion ? "Guardar cambios" : "Guardar")
- **Mensajes al usuario:**
  - Seleccione el producto aplicado!
  - Seleccione el diagnostico a tratar, o la caravana del animal si el tratamiento es preventivo!
  - La duracion del tratamiento tiene que ser de al menos un dia!
  - La dosis diaria es obligatoria!
  - La fecha de inicio no puede ser futura!
  - No se pudo armar el tratamiento!
  - No se pudo registrar el tratamiento!
  - No se pudo corregir el tratamiento!

### RegistrarVacunacion

- **Título:** @(Model.esCorreccion ? "Corregir Vacunacion" : "Registrar Vacunacion")
- **Campos:** Caravana, Vacuna aplicada, Fecha de aplicacion, Plan sanitario que cumple
- **Acciones:** Buscar, @(Model.esCorreccion ? "Guardar cambios" : "Guardar")
- **Mensajes al usuario:**
  - Seleccione un animal!
  - La caravana no existe en el sistema!
  - Seleccione la vacuna aplicada!
  - La fecha de aplicacion no puede ser futura!
  - No se pudo registrar la vacunacion!
  - No se pudo corregir la vacunacion!

## Modulo 5 - Insumos y stock

### AlertasStock

- **Título:** Alertas de Stock Critico
- **Campos:** —
- **Acciones:** —

### AlertasVencimiento

- **Título:** Alertas de Vencimiento
- **Campos:** Ventana (dias), &nbsp;
- **Acciones:** Buscar
- **Mensajes al usuario:**
  - La ventana de anticipacion tiene que ser un numero positivo de dias!

### AltaInsumo

- **Título:** Agregar Insumo
- **Campos:** Nombre, Tipo, Toro que aporta la pajuela, Cantidad de la partida, Vencimiento de la partida, Stock minimo, Periodo de carencia (dias)
- **Acciones:** Buscar, Guardar
- **Mensajes al usuario:**
  - El nombre del insumo es obligatorio!
  - La pajuela tiene que estar vinculada al toro que la aporta. Si el toro no integra el rodeo, cárguelo como animal con 'En pie' desmarcado.
  - Los valores numericos no pueden ser negativos!
  - Ese insumo ya esta registrado. Si es una reposicion, cargue la partida desde Ingreso de Stock.
  - No se pudo registrar el insumo!

### ConfigurarStockMinimo

- **Título:** Configurar Stock Minimo
- **Campos:** Insumo, Stock minimo
- **Acciones:** Guardar
- **Mensajes al usuario:**
  - Seleccione un insumo!
  - El stock minimo tiene que ser mayor o igual a cero!
  - No se pudo guardar el stock minimo!

### HistorialMovimientos

- **Título:** Historial de Movimientos
- **Campos:** Insumo, Tipo, Desde, Hasta, &nbsp;
- **Acciones:** Buscar
- **Mensajes al usuario:**
  - La fecha de inicio tiene que ser anterior o igual a la fecha de fin!

### IngresoStock

- **Título:** Registrar Ingreso de Stock
- **Campos:** Insumo, Cantidad, Fecha del ingreso, Vencimiento de la partida, Motivo
- **Acciones:** Registrar Ingreso
- **Mensajes al usuario:**
  - Seleccione un insumo!
  - La cantidad tiene que ser mayor a cero!
  - La fecha del ingreso no puede ser futura!
  - No se pudo registrar el ingreso de stock!

### ListaInsumos

- **Título:** Insumos
- **Campos:** —
- **Acciones:** —

## Modulo 6 - Indicadores

### CandidatasDescarte

- **Título:** Candidatas a Descarte
- **Campos:** —
- **Acciones:** —

### Indicadores

- **Título:** Indicadores del Rodeo
- **Campos:** —
- **Acciones:** —

## Modulo 7 - Reportes

### ReporteGenetico

- **Título:** Reporte genético
- **Campos:** —
- **Acciones:** Ver en pantalla, Descargar PDF, Descargar Excel

### ReporteProductivo

- **Título:** Reporte productivo
- **Campos:** Desde, Hasta
- **Acciones:** Ver en pantalla, Descargar PDF, Descargar Excel

### ReporteReproductivo

- **Título:** Reporte reproductivo
- **Campos:** Desde, Hasta
- **Acciones:** Ver en pantalla, Descargar PDF, Descargar Excel

### ReporteSanitario

- **Título:** Reporte sanitario
- **Campos:** Desde, Hasta
- **Acciones:** Ver en pantalla, Descargar PDF, Descargar Excel
