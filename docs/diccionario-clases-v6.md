# 2.2.7 Diccionario de Clases — v6

Generado desde `Tesis/Dominio` y `Tesis/Persistencia` por `diccionario_clases.py`. No editar a mano.

## Clases de negocio


### Animal

| Atributo | Tipo | Descripción |
|---|---|---|
| IdAnimal | int |  |
| NumCaravana | string |  |
| FechaNacimiento | DateTime |  |
| Activo | bool | Falso cuando el animal está dado de baja. |
| FechaBaja | DateTime | Fecha de salida del rodeo. Sin valor mientras el animal está activo. |
| MotivoBaja | string |  |
| Raza | Raza |  |
| Categoria | Categoria |  |
| Madre | Hembra | Progenitora registrada. Puede no haberla. |
| Padre | Macho | Progenitor registrado. Puede no haberlo. |
| Foto | string | Nombre del archivo de la fotografía. La imagen se guarda en disco, no en la base. |
| TieneFoto | bool |  |

### CandidataDescarte

No tiene tabla: reúne un animal con los motivos por los que aparece como candidata al descarte.

| Atributo | Tipo | Descripción |
|---|---|---|
| Hembra | Hembra |  |
| Motivos | List<string> | Los criterios de descarte que el animal cumple, en texto: el sistema informa y no decide. |

### Categoria

| Atributo | Tipo | Descripción |
|---|---|---|
| IdCategoria | int |  |
| Nombre | string |  |
| Descripcion | string |  |

### Celo

| Atributo | Tipo | Descripción |
|---|---|---|
| IdCelo | int |  |
| Fecha | DateTime |  |
| Observaciones | string |  |
| Animal | Hembra |  |

### Configuracion

| Atributo | Tipo | Descripción |
|---|---|---|
| IdConfiguracion | int |  |
| DiasSecadoAntesParto | int |  |
| EdadMinimaServicioMeses | int |  |
| EdadCambioCategoriaMeses | int |  |
| LitrosMaximosIndividual | double |  |
| OrdeniesPorDia | int |  |
| DiasAnticipacionSecado | int |  |
| DiasAnticipacionParto | int |  |
| DiasAnticipacionSanitaria | int |  |
| DiasAnticipacionVencimiento | int |  |
| DiasEsperaVoluntaria | int |  |
| DiasParaTacto | int |  |

### ControlDiario

No tiene tabla: es un punto de la curva de lactancia que el sistema arma en memoria a partir de los controles registrados.

| Atributo | Tipo | Descripción |
|---|---|---|
| Fecha | DateTime |  |
| Litros | double | Un punto de la curva de lactancia, usado para estimar la producción por intervalos. |

### Descorne

| Atributo | Tipo | Descripción |
|---|---|---|
| IdDescorne | int |  |
| Fecha | DateTime |  |
| Metodo | string |  |
| Observaciones | string |  |
| Animal | Animal |  |
| Plan | PlanSanitario |  |

### Diagnostico

| Atributo | Tipo | Descripción |
|---|---|---|
| IdDiagnostico | int |  |
| FechaDiagnostico | DateTime |  |
| Enfermedad | string |  |
| Estado | string | Distingue el cuadro activo del cerrado. |
| Animal | Animal |  |

### Hembra (hereda de Animal)

| Atributo | Tipo | Descripción |
|---|---|---|
| NumeroPartos | int | Cantidad de partos registrados. Junto con la edad determina la categoría. |
| EstadoProductivo | string | Sin lactancia, En lactancia o Seca. Eje productivo, independiente del reproductivo. |
| EstadoReproductivo | string | Vacía, Servida o Preñada. Se deduce del servicio vigente y de su último tacto. |

### Insumo

| Atributo | Tipo | Descripción |
|---|---|---|
| IdInsumo | int |  |
| Nombre | string |  |
| TipoInsumo | string |  |
| StockActual | double |  |
| StockMinimo | double |  |
| PeriodoDescarteDias | int | Días de carencia del producto. |
| Toro | Macho |  |

### Lactancia

| Atributo | Tipo | Descripción |
|---|---|---|
| IdLactancia | int |  |
| NumeroLactancia | int |  |
| FechaInicio | DateTime |  |
| FechaSecado | DateTime | Sin valor mientras la lactancia está en curso. |
| FechaProbableParto | DateTime |  |
| Animal | Hembra |  |

### Macho (hereda de Animal)

| Atributo | Tipo | Descripción |
|---|---|---|
| EnPie | bool | Falso en el toro de catálogo, que aporta material genético sin integrar el rodeo. |

### MovimientoStock

| Atributo | Tipo | Descripción |
|---|---|---|
| IdMovimiento | int |  |
| TipoMovimiento | string |  |
| Cantidad | double |  |
| Fecha | DateTime |  |
| FechaVencimiento | DateTime | Vencimiento de la partida ingresada. |
| Motivo | string |  |
| Insumo | Insumo |  |

### OrdenieIndividual

| Atributo | Tipo | Descripción |
|---|---|---|
| IdOrdenieInd | int |  |
| Fecha | DateTime |  |
| Turno | string |  |
| Litros | double | Producción medida de un animal en un turno. No se suma al total del lote del mismo turno. |
| Animal | Hembra |  |
| Lactancia | Lactancia |  |
| OrdenieLote | OrdenieLote |  |

### OrdenieLote

| Atributo | Tipo | Descripción |
|---|---|---|
| IdOrdenieLote | int |  |
| Fecha | DateTime |  |
| Turno | string |  |
| LitrosTotales | double | La leche completa del turno, tal como se lee del tanque. |
| Animales | List<Hembra> |  |

### PartidaVencimiento

No tiene tabla: es el remanente de una partida, derivado de los movimientos de stock.

| Atributo | Tipo | Descripción |
|---|---|---|
| Partida | MovimientoStock |  |
| Remanente | double |  |

### Parto

| Atributo | Tipo | Descripción |
|---|---|---|
| IdParto | int |  |
| FechaParto | DateTime |  |
| TipoParto | string |  |
| Observaciones | string |  |
| Madre | Hembra |  |

### PlanSanitario

| Atributo | Tipo | Descripción |
|---|---|---|
| IdPlan | int |  |
| Nombre | string |  |
| TipoProcedimiento | string |  |
| PeriodicidadDias | int | Sin valor cuando el procedimiento se aplica una única vez en la vida del animal. |
| EdadInicioMeses | int |  |
| Activo | bool |  |
| Insumo | Insumo |  |
| Categorias | List<Categoria> |  |

### ProcedimientoPendiente

No tiene tabla: es un pendiente del calendario sanitario, derivado de comparar el plan con lo aplicado.

| Atributo | Tipo | Descripción |
|---|---|---|
| Animal | Animal |  |
| Plan | PlanSanitario |  |
| UltimaAplicacion | DateTime |  |
| ProximaAplicacion | DateTime |  |

### Raza

| Atributo | Tipo | Descripción |
|---|---|---|
| IdRaza | int |  |
| Nombre | string |  |
| Descripcion | string |  |

### Servicio

| Atributo | Tipo | Descripción |
|---|---|---|
| IdServicio | int |  |
| TipoServicio | string |  |
| FechaServicio | DateTime |  |
| FechaProbableParto | DateTime |  |
| Observaciones | string |  |
| Animal | Hembra |  |
| Toro | Macho |  |
| Pajuela | Insumo |  |

### Tacto

| Atributo | Tipo | Descripción |
|---|---|---|
| IdTacto | int |  |
| FechaTacto | DateTime |  |
| Resultado | string |  |
| Observaciones | string |  |
| Servicio | Servicio |  |

### Tratamiento

| Atributo | Tipo | Descripción |
|---|---|---|
| IdTratamiento | int |  |
| FechaInicio | DateTime |  |
| DiasDuracion | int |  |
| DosisDiaria | string |  |
| CantidadInsumo | double | Cantidad de producto consumida, que es lo que se devuelve al stock si el tratamiento se corrige o se elimina. |
| FechaFinDescarte | DateTime | Fin del período en que la leche del animal no se destina a consumo. |
| Diagnostico | Diagnostico |  |
| Animal | Animal |  |
| Insumo | Insumo |  |
| Plan | PlanSanitario |  |

### Vacunacion

| Atributo | Tipo | Descripción |
|---|---|---|
| IdVacunacion | int |  |
| FechaAplicacion | DateTime |  |
| Animal | Animal |  |
| Insumo | Insumo |  |
| Plan | PlanSanitario |  |

## Controladora


### Configuracion (5 métodos)

| Método | Devuelve | Parámetros |
|---|---|---|
| ObtenerConfiguracion | Configuracion | — |
| ValidarConfiguracion | string | Configuracion pConfiguracion |
| ModificarConfiguracion | bool | Configuracion pConfiguracionNueva |
| ListarTurnos | List<string> | — |
| EsTurnoValido | bool | string pTurno |

### Seguridad (2 métodos)

| Método | Devuelve | Parámetros |
|---|---|---|
| ConfigurarCredenciales | void | string pUsuario, string pContrasena |
| ValidarCredenciales | bool | string pUsuario, string pContrasena |

### Razas (2 métodos)

| Método | Devuelve | Parámetros |
|---|---|---|
| ListarRazas | List<Raza> | — |
| BuscarRaza | Raza | int pId |

### Categorias (2 métodos)

| Método | Devuelve | Parámetros |
|---|---|---|
| ListarCategorias | List<Categoria> | — |
| BuscarCategoria | Categoria | int pId |

### Animales (28 métodos)

| Método | Devuelve | Parámetros |
|---|---|---|
| ListarAnimales | List<Animal> | — |
| BuscarAnimal | Animal | int pId |
| BuscarAnimalXCaravana | Animal | string pNumCaravana |
| ExisteCaravana | bool | string pNumCaravana |
| AltaAnimal | bool | Animal pAnimal |
| ModificarAnimal | bool | int pIdAnimal, string pNumCaravana, DateTime pFechaNacimiento, Raza pRaza, Categoria pCategoria, Hembra pMadre, Macho pPadre, int pNumeroPartos, bool pEnPie, string pFoto |
| ActualizarCategoria | bool | int pIdAnimal |
| BajaAnimal | bool | string pNumCaravana, string pMotivoBaja |
| ReactivarAnimal | bool | string pNumCaravana |
| GuardarFotoAnimal | string | byte[] pContenido |
| BorrarFotoAnimal | void | string pNombreArchivo |
| UrlFoto | string | Animal pAnimal |
| EstabaActivo | bool | Animal pAnimal, DateTime pFecha |
| CalcularEdadMeses | int | Animal pAnimal |
| ListarDescendencia | List<Animal> | Animal pAnimal |
| ValidarGenealogia | string | int pIdAnimal, DateTime pFechaNacimiento, Hembra pMadre, Macho pPadre |
| AdvertenciasGenealogia | List<string> | DateTime pFechaNacimiento, Hembra pMadre, Macho pPadre |
| CalcularCategoria | Categoria | Animal pAnimal |
| AplicaCategoria | bool | Categoria pCategoria, Animal pAnimal |
| EsHembra | bool | string pNumCaravana |
| ObtenerLinaje | Animal | string pNumCaravana |
| ListarAscendencia | List<Animal> | Animal pAnimal |
| BuscarAncestroComun | Animal | Animal pAnimal, Animal pPareja |
| VerificarConsanguinidad | bool | Animal pAnimal, Animal pPareja |
| FiltrarAnimalesXRaza | List<Animal> | int pIdRaza |
| FiltrarAnimalesXCategoria | List<Animal> | int pIdCategoria |
| FiltrarAnimalesXEstado | List<Animal> | bool pActivo |
| FiltrarAnimales | List<Animal> | string pNumCaravana, int pIdRaza, int pIdCategoria, int pActivo, int pEdadDesde, int pEdadHasta |

### Hembras (8 métodos)

| Método | Devuelve | Parámetros |
|---|---|---|
| ListarHembras | List<Hembra> | — |
| BuscarHembra | Hembra | int pId |
| AltaHembra | bool | Hembra pHembra |
| EstaEnLactancia | bool | string pNumCaravana |
| ModificarEstadoProductivo | bool | int pIdHembra, string pEstadoProductivo |
| ModificarEstadoReproductivo | bool | int pIdHembra, string pEstadoReproductivo |
| ListarHembrasEnLactancia | List<Hembra> | — |
| ListarHembrasPrenadas | List<Hembra> | — |

### Machos (4 métodos)

| Método | Devuelve | Parámetros |
|---|---|---|
| ListarMachos | List<Macho> | — |
| BuscarMacho | Macho | int pId |
| AltaMacho | bool | Macho pMacho |
| EsToro | bool | Macho pMacho |

### Lactancias (14 métodos)

| Método | Devuelve | Parámetros |
|---|---|---|
| ListarLactancias | List<Lactancia> | — |
| BuscarLactancia | Lactancia | int pId |
| LactanciaEstaActiva | bool | Lactancia pLactancia |
| LactanciaActual | Lactancia | Hembra pHembra |
| LactanciaDeLaFecha | Lactancia | Hembra pHembra, DateTime pFecha |
| ListarLactanciasActivas | List<Lactancia> | — |
| FiltrarLactanciasXHembra | List<Lactancia> | int pIdHembra |
| ProximoNumeroLactancia | int | Hembra pHembra |
| AltaLactancia | bool | Lactancia pLactanciaNueva |
| ModificarLactancia | bool | Lactancia pLactanciaNueva |
| RegistrarSecado | bool | string pNumCaravana, DateTime pFecha |
| CalcularFechaSecado | DateTime | Hembra pHembra |
| ListarAlertasSecado | List<Hembra> | — |
| CalcularProduccionTotal | double | Lactancia pLactancia |

### Ordenies Por Lote (14 métodos)

| Método | Devuelve | Parámetros |
|---|---|---|
| ListarOrdeniesLote | List<OrdenieLote> | — |
| BuscarOrdenieLote | OrdenieLote | int pId |
| BuscarOrdenieLoteXFechaTurno | OrdenieLote | DateTime pFecha, string pTurno |
| ValidarLitros | bool | double pLitros |
| ValidarLitrosLote | bool | double pLitros, int pCantidadAnimales |
| ValidarLitrosIndividual | bool | double pLitros |
| ListarAnimalesParaOrdenie | List<Hembra> | — |
| ListarHembrasEnDescarte | List<Hembra> | — |
| AltaOrdenieLote | bool | OrdenieLote pOrdenieLote |
| ModificarOrdenieLote | bool | int pIdOrdenieLote, double pLitrosTotales, List<Hembra> pAnimales |
| ValidarEliminarOrdenieLote | string | int pIdOrdenieLote |
| EliminarOrdenieLote | bool | int pIdOrdenieLote |
| ListarTurnosSoloConControlIndividual | List<OrdenieLote> | — |
| FiltrarOrdeniesLoteXFecha | List<OrdenieLote> | DateTime pDesde, DateTime pHasta |

### Ordenies Individuales (15 métodos)

| Método | Devuelve | Parámetros |
|---|---|---|
| ListarOrdeniesIndividual | List<OrdenieIndividual> | — |
| BuscarOrdenieIndividual | OrdenieIndividual | int pId |
| AltaOrdenieIndividual | bool | OrdenieIndividual pOrdenie |
| BuscarOrdenieIndividualXFechaTurno | OrdenieIndividual | DateTime pFecha, string pTurno, int pIdAnimal |
| ValidarModificarOrdenieIndividual | string | int pIdOrdenieInd, double pLitros |
| ModificarOrdenieIndividual | bool | int pIdOrdenieInd, double pLitros |
| ValidarEliminarOrdenieIndividual | string | int pIdOrdenieInd |
| EliminarOrdenieIndividual | bool | int pIdOrdenieInd |
| SumarLitrosSinOrdenieLote | double | DateTime pDesde, DateTime pHasta |
| SumarLitrosIndividualesDelTurno | double | DateTime pFecha, string pTurno |
| SumarLitros | double | List<OrdenieIndividual> pOrdenies |
| FiltrarOrdeniesXLactancia | List<OrdenieIndividual> | int pIdLactancia |
| FiltrarOrdeniesIndividualXFecha | List<OrdenieIndividual> | DateTime pDesde, DateTime pHasta |
| CalcularProduccionEnRango | double | DateTime pDesde, DateTime pHasta, string pModalidad |
| CalcularProduccionMensual | double | int pMes, int pAnio |

### Celos (9 métodos)

| Método | Devuelve | Parámetros |
|---|---|---|
| ListarCelos | List<Celo> | — |
| BuscarCelo | Celo | int pId |
| ValidarCelo | string | Celo pCelo |
| AltaCelo | bool | Celo pCelo |
| ValidarModificarCelo | string | int pIdCelo, DateTime pFecha, int pIdHembra |
| ModificarCelo | bool | int pIdCelo, DateTime pFecha, string pObservaciones, int pIdHembra |
| ValidarEliminarCelo | string | int pIdCelo |
| EliminarCelo | bool | int pIdCelo |
| FiltrarCelosXHembra | List<Celo> | int pIdHembra |

### Servicios (18 métodos)

| Método | Devuelve | Parámetros |
|---|---|---|
| ListarServicios | List<Servicio> | — |
| BuscarServicio | Servicio | int pId |
| CalcularFechaParto | DateTime | DateTime pFechaServicio |
| EsInseminacion | bool | Servicio pServicio |
| ToroDelServicio | Macho | Servicio pServicio |
| ValidarServicio | string | Servicio pServicio |
| AdvertenciasServicio | List<string> | Servicio pServicio |
| AltaServicio | bool | Servicio pServicio |
| ModificarServicio | bool | int pIdServicio, DateTime pFechaProbableParto |
| ValidarModificarServicio | string | int pIdServicio, string pTipoServicio, DateTime pFechaServicio, DateTime pFechaProbableParto, int pIdHembra, int pIdToro, int pIdPajuela |
| ModificarServicio | bool | int pIdServicio, string pTipoServicio, DateTime pFechaServicio, DateTime pFechaProbableParto, string pObservaciones, int pIdHembra, int pIdToro, int pIdPajuela |
| ValidarEliminarServicio | string | int pIdServicio |
| EliminarServicio | bool | int pIdServicio |
| FiltrarServiciosXHembra | List<Servicio> | int pIdHembra |
| ServicioVigente | Servicio | Hembra pHembra |
| ListarServiciosConPrenez | List<Servicio> | — |
| FiltrarServiciosXFechaParto | List<Servicio> | DateTime pDesde, DateTime pHasta |
| ListarAlertasParto | List<Servicio> | — |

### Tactos (10 métodos)

| Método | Devuelve | Parámetros |
|---|---|---|
| ListarTactos | List<Tacto> | — |
| BuscarTacto | Tacto | int pId |
| EsPositivo | bool | Tacto pTacto |
| FiltrarTactosXServicio | List<Tacto> | int pIdServicio |
| UltimoTacto | Tacto | Servicio pServicio |
| AltaTacto | bool | Tacto pTacto |
| ValidarModificarTacto | string | int pIdTacto, int pIdServicio, DateTime pFechaTacto, string pResultado |
| ModificarTacto | bool | int pIdTacto, int pIdServicio, DateTime pFechaTacto, string pResultado, string pObservaciones |
| ValidarEliminarTacto | string | int pIdTacto |
| EliminarTacto | bool | int pIdTacto |

### Partos (14 métodos)

| Método | Devuelve | Parámetros |
|---|---|---|
| ListarPartos | List<Parto> | — |
| BuscarParto | Parto | int pId |
| FiltrarPartosXHembra | List<Parto> | int pIdHembra |
| PadreSugerido | Macho | Hembra pMadre |
| AdvertenciasParto | List<string> | Parto pParto, List<Animal> pListaCrias |
| AltaParto | bool | Parto pParto, List<Animal> pListaCrias |
| LactanciaDelParto | Lactancia | Parto pParto |
| LactanciaCerradaPorParto | Lactancia | Parto pParto |
| CriasDelParto | List<Animal> | Parto pParto |
| MotivoAnimalNoEliminable | string | Animal pAnimal |
| ValidarModificarParto | string | int pIdParto, DateTime pFechaParto, string pTipoParto |
| ModificarParto | bool | int pIdParto, DateTime pFechaParto, string pTipoParto, string pObservaciones |
| ValidarEliminarParto | string | int pIdParto |
| EliminarParto | bool | int pIdParto |

### Insumos (20 métodos)

| Método | Devuelve | Parámetros |
|---|---|---|
| ListarInsumos | List<Insumo> | — |
| BuscarInsumo | Insumo | int pId |
| ListarPajuelas | List<Insumo> | — |
| ListarInsumosSanitarios | List<Insumo> | — |
| ListarInsumosXTipo | List<Insumo> | string pTipoInsumo |
| ExisteInsumo | bool | string pNombre, string pTipoInsumo |
| AltaInsumo | bool | Insumo pInsumoNuevo |
| RegistrarIngreso | bool | Insumo pInsumoNuevo, double pCantidad, DateTime pFecha, DateTime pFechaVencimiento |
| IngresarStock | bool | int pIdInsumo, double pCantidad, DateTime pFecha, DateTime pFechaVencimiento, string pMotivo |
| VerificarStock | bool | int pIdInsumo, double pCantidad |
| ModificarStockMinimo | bool | int pIdInsumo, double pStockMinimo |
| StockCritico | bool | Insumo pInsumo |
| ListarAlertasStock | List<Insumo> | — |
| ListarMovimientosStock | List<MovimientoStock> | — |
| FiltrarMovimientos | List<MovimientoStock> | int pIdInsumo, string pTipoMovimiento, DateTime pDesde, DateTime pHasta |
| StockResultante | double | MovimientoStock pMovimiento |
| ListarPartidas | List<PartidaVencimiento> | Insumo pInsumo |
| ListarAlertasVencimiento | List<PartidaVencimiento> | int pAnticipacionDias |
| DiasParaVencer | int | PartidaVencimiento pPartida |
| EstaVencida | bool | PartidaVencimiento pPartida |

### Sanidad (23 métodos)

| Método | Devuelve | Parámetros |
|---|---|---|
| ListarDiagnosticos | List<Diagnostico> | — |
| BuscarDiagnostico | Diagnostico | int pId |
| DiagnosticoEstaActivo | bool | Diagnostico pDiagnostico |
| AltaDiagnostico | bool | Diagnostico pDiagnosticoNuevo |
| ResolverDiagnostico | bool | int pIdDiagnostico |
| FiltrarDiagnosticosXAnimal | List<Diagnostico> | int pIdAnimal |
| ValidarModificarDiagnostico | string | int pIdDiagnostico, DateTime pFechaDiagnostico, string pEnfermedad, int pIdAnimal |
| ModificarDiagnostico | bool | int pIdDiagnostico, DateTime pFechaDiagnostico, string pEnfermedad, int pIdAnimal |
| ValidarEliminarDiagnostico | string | int pIdDiagnostico |
| EliminarDiagnostico | bool | int pIdDiagnostico |
| FiltrarTratamientosXDiagnostico | List<Tratamiento> | int pIdDiagnostico |
| ListarTratamientos | List<Tratamiento> | — |
| BuscarTratamiento | Tratamiento | int pId |
| CalcularFechaFin | DateTime | Tratamiento pTratamiento |
| CalcularDescarte | DateTime | Tratamiento pTratamiento |
| AltaTratamiento | bool | Tratamiento pTratamientoNuevo, double pCantidadInsumo |
| ValidarModificarTratamiento | string | int pIdTratamiento, DateTime pFechaInicio, int pDiasDuracion, string pDosisDiaria, double pCantidadInsumo, int pIdDiagnostico, int pIdAnimal, int pIdInsumo, int pIdPlan |
| ModificarTratamiento | bool | int pIdTratamiento, DateTime pFechaInicio, int pDiasDuracion, string pDosisDiaria, double pCantidadInsumo, DateTime pFechaFinDescarte, int pIdDiagnostico, int pIdAnimal, int pIdInsumo, int pIdPlan |
| ValidarEliminarTratamiento | string | int pIdTratamiento |
| EliminarTratamiento | bool | int pIdTratamiento |
| FechaFinDescarte | DateTime | Animal pAnimal |
| FiltrarTratamientosXAnimal | List<Tratamiento> | int pIdAnimal |
| TieneDescarteVigente | bool | Animal pAnimal |

### Vacunaciones (8 métodos)

| Método | Devuelve | Parámetros |
|---|---|---|
| ListarVacunaciones | List<Vacunacion> | — |
| BuscarVacunacion | Vacunacion | int pId |
| AltaVacunacion | bool | Vacunacion pVacunacionNueva |
| ValidarModificarVacunacion | string | int pIdVacunacion, DateTime pFechaAplicacion, int pIdAnimal, int pIdInsumo, int pIdPlan |
| ModificarVacunacion | bool | int pIdVacunacion, DateTime pFechaAplicacion, int pIdAnimal, int pIdInsumo, int pIdPlan |
| ValidarEliminarVacunacion | string | int pIdVacunacion |
| EliminarVacunacion | bool | int pIdVacunacion |
| FiltrarVacunacionesXAnimal | List<Vacunacion> | int pIdAnimal |

### Descornes (10 métodos)

| Método | Devuelve | Parámetros |
|---|---|---|
| ListarDescornes | List<Descorne> | — |
| BuscarDescorne | Descorne | int pId |
| AltaDescorne | bool | Descorne pDescorneNuevo |
| ValidarModificarDescorne | string | int pIdDescorne, DateTime pFecha, string pMetodo, int pIdAnimal, int pIdPlan |
| ModificarDescorne | bool | int pIdDescorne, DateTime pFecha, string pMetodo, string pObservaciones, int pIdAnimal, int pIdPlan |
| ValidarEliminarDescorne | string | int pIdDescorne |
| EliminarDescorne | bool | int pIdDescorne |
| TieneDescorne | bool | Animal pAnimal |
| FiltrarDescornesXAnimal | List<Descorne> | int pIdAnimal |
| FiltrarDescornesXFecha | List<Descorne> | DateTime pFechaInicio, DateTime pFechaFin |

### Planes Sanitarios (8 métodos)

| Método | Devuelve | Parámetros |
|---|---|---|
| ListarPlanesSanitarios | List<PlanSanitario> | — |
| ListarPlanesActivos | List<PlanSanitario> | — |
| ListarPlanesXTipo | List<PlanSanitario> | string pTipoProcedimiento |
| BuscarPlanSanitario | PlanSanitario | int pId |
| ExistePlanSanitario | bool | string pNombre, int pIdPlan |
| ValidarPlanSanitario | string | PlanSanitario pPlan |
| AltaPlanSanitario | bool | PlanSanitario pPlanNuevo |
| ModificarPlanSanitario | bool | PlanSanitario pPlan |

### Indicadores (17 métodos)

| Método | Devuelve | Parámetros |
|---|---|---|
| ListarControlesDiarios | List<ControlDiario> | Lactancia pLactancia |
| EstimarProduccionLactancia | double | Lactancia pLactancia |
| DiasEnLeche | int | Lactancia pLactancia |
| PromedioDiarioLactancia | double | Lactancia pLactancia |
| ProyectarProduccion305 | double | Lactancia pLactancia |
| UltimoParto | Parto | Hembra pHembra |
| DiasAbiertos | int | Hembra pHembra |
| TieneDiasAbiertosCerrados | bool | Hembra pHembra |
| IntervaloEntrePartos | int | Hembra pHembra |
| ServiciosDesdeUltimoParto | int | Hembra pHembra |
| ServiciosPorPrenez | double | — |
| PromedioDiasAbiertos | double | — |
| PromedioIntervaloEntrePartos | double | — |
| PromedioDiarioRodeo | double | — |
| PromedioDiasEnLeche | double | — |
| ContarHembrasXEstadoProductivo | int | string pEstadoProductivo |
| ContarHembrasXEstadoReproductivo | int | string pEstadoReproductivo |

### Listas De Trabajo (5 métodos)

| Método | Devuelve | Parámetros |
|---|---|---|
| ListarTactosPendientes | List<Servicio> | — |
| ListarVacasParaServir | List<Hembra> | — |
| EstaParaServir | bool | Hembra pHembra |
| MotivoParaServir | string | Hembra pHembra |
| ListarCandidatasDescarte | List<CandidataDescarte> | — |

### Calendario Sanitario (8 métodos)

| Método | Devuelve | Parámetros |
|---|---|---|
| PlanAlcanzaAnimal | bool | PlanSanitario pPlan, Animal pAnimal |
| UltimaAplicacion | DateTime | PlanSanitario pPlan, Animal pAnimal |
| ProximaAplicacion | DateTime | PlanSanitario pPlan, Animal pAnimal |
| CalcularPendientes | List<ProcedimientoPendiente> | PlanSanitario pPlan, int pAnticipacionDias |
| ObtenerCalendarioSanitario | List<ProcedimientoPendiente> | int pAnticipacionDias |
| FiltrarCalendario | List<ProcedimientoPendiente> | List<ProcedimientoPendiente> pCalendario, string pTipoProcedimiento, int pIdCategoria |
| EstaVencido | bool | ProcedimientoPendiente pPendiente |
| DiasParaAplicar | int | ProcedimientoPendiente pPendiente |

## pControladora


### Configuracion (2 métodos)

| Método | Devuelve | Parámetros |
|---|---|---|
| ObtenerConfiguracion | Configuracion | — |
| ModificarConfiguracion | bool | Configuracion pConfiguracionNueva |

### Razas (2 métodos)

| Método | Devuelve | Parámetros |
|---|---|---|
| ListarRazas | List<Raza> | — |
| AltaRaza | bool | Raza pRaza |

### Categorias (2 métodos)

| Método | Devuelve | Parámetros |
|---|---|---|
| ListarCategorias | List<Categoria> | — |
| AltaCategoria | bool | Categoria pCategoria |

### Animales (7 métodos)

| Método | Devuelve | Parámetros |
|---|---|---|
| ListarAnimales | List<Animal> | List<Raza> pListaRazas, List<Categoria> pListaCategorias |
| AltaAnimal | bool | Animal pAnimal |
| ModificarAnimal | bool | Animal pAnimal |
| BajaAnimal | bool | int pIdAnimal, string pMotivoBaja |
| ReactivarAnimal | bool | int pIdAnimal |
| GuardarFoto | string | byte[] pContenido |
| BorrarFoto | void | string pNombreArchivo |

### Hembras (3 métodos)

| Método | Devuelve | Parámetros |
|---|---|---|
| ListarHembras | List<Hembra> | List<Animal> pListaAnimales |
| AltaHembra | bool | Hembra pHembra |
| ModificarHembra | bool | Hembra pHembra |

### Machos (3 métodos)

| Método | Devuelve | Parámetros |
|---|---|---|
| ListarMachos | List<Macho> | List<Animal> pListaAnimales |
| AltaMacho | bool | Macho pMacho |
| ModificarMacho | bool | Macho pMacho |

### Lactancias (4 métodos)

| Método | Devuelve | Parámetros |
|---|---|---|
| ListarLactancias | List<Lactancia> | List<Hembra> pListaHembras |
| AltaLactancia | bool | Lactancia pLactanciaNueva |
| ModificarLactancia | bool | Lactancia pLactanciaNueva |
| RegistrarSecado | bool | Lactancia pLactanciaCerrada, Hembra pHembraSeca |

### Ordenies Por Lote (4 métodos)

| Método | Devuelve | Parámetros |
|---|---|---|
| ListarOrdeniesLote | List<OrdenieLote> | List<Hembra> pListaHembras |
| AltaOrdenieLote | bool | OrdenieLote pOrdenieLote |
| ModificarOrdenieLote | bool | OrdenieLote pOrdenieLote |
| EliminarOrdenieLote | bool | int pIdOrdenieLote |

### Ordenies Individuales (4 métodos)

| Método | Devuelve | Parámetros |
|---|---|---|
| ListarOrdeniesIndividual | List<OrdenieIndividual> | List<Hembra> pListaHembras, List<Lactancia> pListaLactancias, List<OrdenieLote> pListaOrdeniesLote |
| AltaOrdenieIndividual | bool | OrdenieIndividual pOrdenie |
| ModificarOrdenieIndividual | bool | OrdenieIndividual pOrdenie |
| EliminarOrdenieIndividual | bool | int pIdOrdenieInd |

### Celos (4 métodos)

| Método | Devuelve | Parámetros |
|---|---|---|
| ListarCelos | List<Celo> | List<Hembra> pListaHembras |
| AltaCelo | bool | Celo pCelo |
| ModificarCelo | bool | Celo pCelo |
| EliminarCelo | bool | int pIdCelo |

### Servicios (4 métodos)

| Método | Devuelve | Parámetros |
|---|---|---|
| ListarServicios | List<Servicio> | List<Hembra> pListaHembras, List<Macho> pListaMachos, List<Insumo> pListaInsumos |
| AltaServicio | bool | Servicio pServicio, Hembra pHembraServida |
| ModificarServicio | bool | Servicio pServicio, List<Lactancia> pLactanciasActualizadas, List<Hembra> pHembrasActualizadas, List<MovimientoStock> pMovimientos |
| EliminarServicio | bool | int pIdServicio, List<Lactancia> pLactanciasActualizadas, List<Hembra> pHembrasActualizadas, List<MovimientoStock> pMovimientos |

### Tactos (4 métodos)

| Método | Devuelve | Parámetros |
|---|---|---|
| ListarTactos | List<Tacto> | List<Servicio> pListaServicios |
| AltaTacto | bool | Tacto pTacto, Hembra pHembraTactada, Lactancia pLactanciaVigente |
| ModificarTacto | bool | Tacto pTacto, List<Hembra> pHembrasActualizadas, List<Lactancia> pLactanciasActualizadas |
| EliminarTacto | bool | int pIdTacto, List<Hembra> pHembrasActualizadas, List<Lactancia> pLactanciasActualizadas |

### Partos (4 métodos)

| Método | Devuelve | Parámetros |
|---|---|---|
| ListarPartos | List<Parto> | List<Hembra> pListaHembras |
| AltaParto | bool | Parto pParto, List<Animal> pListaCrias, Lactancia pNuevaLactancia, Hembra pMadreActualizada, Lactancia pLactanciaCerrada |
| ModificarParto | bool | Parto pParto, List<Lactancia> pLactanciasActualizadas, List<Animal> pListaCrias |
| EliminarParto | bool | int pIdParto, List<Animal> pListaCrias, Lactancia pLactanciaDelParto, Hembra pMadreActualizada, Lactancia pLactanciaReabierta |

### Insumos (3 métodos)

| Método | Devuelve | Parámetros |
|---|---|---|
| ListarInsumos | List<Insumo> | List<Macho> pListaMachos |
| AltaInsumo | bool | Insumo pInsumoNuevo |
| ModificarStockMinimo | bool | int pIdInsumo, double pStockMinimo |

### Movimientos De Stock (2 métodos)

| Método | Devuelve | Parámetros |
|---|---|---|
| ListarMovimientosStock | List<MovimientoStock> | List<Insumo> pListaInsumos |
| RegistrarIngresoStock | bool | MovimientoStock pMovimiento |

### Planes Sanitarios (3 métodos)

| Método | Devuelve | Parámetros |
|---|---|---|
| ListarPlanesSanitarios | List<PlanSanitario> | List<Insumo> pListaInsumos, List<Categoria> pListaCategorias |
| AltaPlanSanitario | bool | PlanSanitario pPlan |
| ModificarPlanSanitario | bool | PlanSanitario pPlan |

### Vacunaciones (4 métodos)

| Método | Devuelve | Parámetros |
|---|---|---|
| ListarVacunaciones | List<Vacunacion> | List<Animal> pListaAnimales, List<Insumo> pListaInsumos, List<PlanSanitario> pListaPlanes |
| AltaVacunacion | bool | Vacunacion pVacunacion, double pCantidadInsumo |
| ModificarVacunacion | bool | Vacunacion pVacunacion, List<MovimientoStock> pMovimientos |
| EliminarVacunacion | bool | int pIdVacunacion, List<MovimientoStock> pMovimientos |

### Descornes (4 métodos)

| Método | Devuelve | Parámetros |
|---|---|---|
| ListarDescornes | List<Descorne> | List<Animal> pListaAnimales, List<PlanSanitario> pListaPlanes |
| AltaDescorne | bool | Descorne pDescorne |
| ModificarDescorne | bool | Descorne pDescorne |
| EliminarDescorne | bool | int pIdDescorne |

### Diagnosticos (5 métodos)

| Método | Devuelve | Parámetros |
|---|---|---|
| ListarDiagnosticos | List<Diagnostico> | List<Animal> pListaAnimales |
| AltaDiagnostico | bool | Diagnostico pDiagnosticoNuevo |
| ModificarDiagnostico | bool | int pIdDiagnostico, string pEstado |
| ModificarDiagnostico | bool | Diagnostico pDiagnostico |
| EliminarDiagnostico | bool | int pIdDiagnostico |

### Tratamientos (4 métodos)

| Método | Devuelve | Parámetros |
|---|---|---|
| ListarTratamientos | List<Tratamiento> | List<Diagnostico> pListaDiagnosticos, List<Insumo> pListaInsumos, List<Animal> pListaAnimales, List<PlanSanitario> pListaPlanes |
| AltaTratamiento | bool | Tratamiento pTratamientoNuevo |
| ModificarTratamiento | bool | Tratamiento pTratamiento, List<MovimientoStock> pMovimientos, List<Diagnostico> pDiagnosticosActualizados |
| EliminarTratamiento | bool | int pIdTratamiento, List<MovimientoStock> pMovimientos, List<Diagnostico> pDiagnosticosActualizados |

## Clases de acceso a datos


### pAnimal

| Método | Devuelve | Parámetros |
|---|---|---|
| ListarAnimales | List<Animal> | List<Raza> pListaRazas, List<Categoria> pListaCategorias |
| ParametrosAlta | Dictionary<string, object?> | Animal pAnimal |
| AltaAnimal | bool | Animal pAnimal |
| ParametrosModificar | Dictionary<string, object?> | Animal pAnimal |
| ModificarAnimal | bool | Animal pAnimal |
| BajaAnimal | bool | int pIdAnimal, string pMotivoBaja |
| ReactivarAnimal | bool | int pIdAnimal |

### pCategoria

| Método | Devuelve | Parámetros |
|---|---|---|
| ListarCategorias | List<Categoria> | — |
| AltaCategoria | bool | Categoria pCategoria |

### pCelo

| Método | Devuelve | Parámetros |
|---|---|---|
| ListarCelos | List<Celo> | List<Hembra> pListaHembras |
| AltaCelo | bool | Celo pCelo |
| ModificarCelo | bool | Celo pCelo |
| EliminarCelo | bool | int pIdCelo |

### pConexion

| Método | Devuelve | Parámetros |
|---|---|---|
| Configurar | void | string pCadenaConexion |
| EjecutarComando | bool | string pSql |
| EjecutarComando | bool | string pSql, Dictionary<string, object?>? pParametros |
| EjecutarInsercion | int | string pSql, Dictionary<string, object?> pParametros |
| EjecutarConsulta | DataTable | string pSql |
| EjecutarConsulta | DataTable | string pSql, Dictionary<string, object?>? pParametros |
| AbrirConexion | MySqlConnection | — |
| EjecutarInsercionEnTransaccion | int | string pSql, Dictionary<string, object?> pParametros, MySqlConnection pConexion, MySqlTransaction pTransaccion |
| EjecutarComandoEnTransaccion | void | string pSql, Dictionary<string, object?> pParametros, MySqlConnection pConexion, MySqlTransaction pTransaccion |

### pConfiguracion

| Método | Devuelve | Parámetros |
|---|---|---|
| ObtenerConfiguracion | Configuracion | — |
| ModificarConfiguracion | bool | Configuracion pConfiguracion |

### pDescorne

| Método | Devuelve | Parámetros |
|---|---|---|
| ListarDescornes | List<Descorne> | List<Animal> pListaAnimales, List<PlanSanitario> pListaPlanes |
| AltaDescorne | bool | Descorne pDescorne |
| ModificarDescorne | bool | Descorne pDescorne |
| EliminarDescorne | bool | int pIdDescorne |

### pDiagnostico

| Método | Devuelve | Parámetros |
|---|---|---|
| ListarDiagnosticos | List<Diagnostico> | List<Animal> pListaAnimales |
| AltaDiagnostico | bool | Diagnostico pDiagnostico |
| ParametrosModificarEstado | Dictionary<string, object?> | int pIdDiagnostico, string pEstado |
| ModificarDiagnostico | bool | int pIdDiagnostico, string pEstado |
| ModificarDiagnostico | bool | Diagnostico pDiagnostico |
| EliminarDiagnostico | bool | int pIdDiagnostico |

### pFotoAnimal

| Método | Devuelve | Parámetros |
|---|---|---|
| Configurar | void | string pRutaWwwRoot |
| Guardar | string | byte[] pContenido |
| Borrar | void | string pNombreArchivo |

### pHembra

| Método | Devuelve | Parámetros |
|---|---|---|
| ParametrosAlta | Dictionary<string, object?> | Hembra pHembra |
| ListarHembras | List<Hembra> | List<Animal> pListaAnimales |
| AltaHembra | bool | Hembra pHembra |
| ParametrosModificar | Dictionary<string, object?> | Hembra pHembra |
| ModificarHembra | bool | Hembra pHembra |

### pInsumo

| Método | Devuelve | Parámetros |
|---|---|---|
| ListarInsumos | List<Insumo> | List<Macho> pListaMachos |
| AltaInsumo | bool | Insumo pInsumo |
| ModificarStockMinimo | bool | int pIdInsumo, double pStockMinimo |

### pLactancia

| Método | Devuelve | Parámetros |
|---|---|---|
| ListarLactancias | List<Lactancia> | List<Hembra> pListaHembras |
| ParametrosAlta | Dictionary<string, object?> | Lactancia pLactancia |
| AltaLactancia | bool | Lactancia pLactancia |
| ParametrosModificar | Dictionary<string, object?> | Lactancia pLactancia |
| ModificarLactancia | bool | Lactancia pLactancia |
| RegistrarSecado | bool | Lactancia pLactanciaCerrada, Hembra pHembraSeca |

### pMacho

| Método | Devuelve | Parámetros |
|---|---|---|
| ParametrosAlta | Dictionary<string, object?> | Macho pMacho |
| ListarMachos | List<Macho> | List<Animal> pListaAnimales |
| AltaMacho | bool | Macho pMacho |
| ParametrosModificar | Dictionary<string, object?> | Macho pMacho |
| ModificarMacho | bool | Macho pMacho |

### pMovimientoStock

| Método | Devuelve | Parámetros |
|---|---|---|
| ListarMovimientos | List<MovimientoStock> | List<Insumo> pListaInsumos |
| ParametrosAlta | Dictionary<string, object?> | MovimientoStock pMovimiento |
| AsentarEnTransaccion | void | pConexion pConexionDatos, MovimientoStock pMovimiento, MySqlConnection pConexion, MySqlTransaction pTransaccion |
| AsentarEnTransaccion | void | pConexion pConexionDatos, List<MovimientoStock> pMovimientos, MySqlConnection pConexion, MySqlTransaction pTransaccion |
| RegistrarIngreso | bool | MovimientoStock pMovimiento |

### pOrdenieIndividual

| Método | Devuelve | Parámetros |
|---|---|---|
| ListarOrdeniesIndividual | List<OrdenieIndividual> | List<Hembra> pListaHembras, List<Lactancia> pListaLactancias, List<OrdenieLote> pListaOrdeniesLote |
| AltaOrdenieIndividual | bool | OrdenieIndividual pOrdenie |
| ModificarOrdenieIndividual | bool | OrdenieIndividual pOrdenie |
| EliminarOrdenieIndividual | bool | int pIdOrdenieInd |

### pOrdenieLote

| Método | Devuelve | Parámetros |
|---|---|---|
| ListarOrdeniesLote | List<OrdenieLote> | List<Hembra> pListaHembras |
| AltaOrdenieLote | bool | OrdenieLote pOrdenieLote |
| ModificarOrdenieLote | bool | OrdenieLote pOrdenieLote |
| EliminarOrdenieLote | bool | int pIdOrdenieLote |

### pParto

| Método | Devuelve | Parámetros |
|---|---|---|
| ListarPartos | List<Parto> | List<Hembra> pListaHembras |
| AltaParto | bool | Parto pParto, List<Animal> pListaCrias, Lactancia pNuevaLactancia, Hembra pMadreActualizada, Lactancia pLactanciaCerrada |
| ParametrosAlta | Dictionary<string, object?> | Parto pParto |
| ParametrosModificar | Dictionary<string, object?> | Parto pParto |
| ModificarParto | bool | Parto pParto, List<Lactancia> pLactanciasActualizadas, List<Animal> pListaCrias |
| EliminarParto | bool | int pIdParto, List<Animal> pListaCrias, Lactancia pLactanciaDelParto, Hembra pMadreActualizada, Lactancia pLactanciaReabierta |

### pPlanSanitario

| Método | Devuelve | Parámetros |
|---|---|---|
| ListarPlanes | List<PlanSanitario> | List<Insumo> pListaInsumos, List<Categoria> pListaCategorias |
| AltaPlan | bool | PlanSanitario pPlan |
| ModificarPlan | bool | PlanSanitario pPlan |

### pRaza

| Método | Devuelve | Parámetros |
|---|---|---|
| ListarRazas | List<Raza> | — |
| AltaRaza | bool | Raza pRaza |

### pServicio

| Método | Devuelve | Parámetros |
|---|---|---|
| ListarServicios | List<Servicio> | List<Hembra> pListaHembras, List<Macho> pListaMachos, List<Insumo> pListaInsumos |
| AltaServicio | bool | Servicio pServicio, Hembra pHembraServida |
| ParametrosModificar | Dictionary<string, object?> | Servicio pServicio |
| ModificarServicio | bool | Servicio pServicio, List<Lactancia> pLactanciasActualizadas, List<Hembra> pHembrasActualizadas, List<MovimientoStock> pMovimientos |
| EliminarServicio | bool | int pIdServicio, List<Lactancia> pLactanciasActualizadas, List<Hembra> pHembrasActualizadas, List<MovimientoStock> pMovimientos |

### pTacto

| Método | Devuelve | Parámetros |
|---|---|---|
| ListarTactos | List<Tacto> | List<Servicio> pListaServicios |
| ParametrosAlta | Dictionary<string, object?> | Tacto pTacto |
| AltaTacto | bool | Tacto pTacto, Hembra pHembraTactada, Lactancia pLactanciaVigente |
| ParametrosModificar | Dictionary<string, object?> | Tacto pTacto |
| ModificarTacto | bool | Tacto pTacto, List<Hembra> pHembrasActualizadas, List<Lactancia> pLactanciasActualizadas |
| EliminarTacto | bool | int pIdTacto, List<Hembra> pHembrasActualizadas, List<Lactancia> pLactanciasActualizadas |

### pTratamiento

| Método | Devuelve | Parámetros |
|---|---|---|
| ListarTratamientos | List<Tratamiento> | List<Diagnostico> pListaDiagnosticos, List<Insumo> pListaInsumos, List<Animal> pListaAnimales, List<PlanSanitario> pListaPlanes |
| AltaTratamiento | bool | Tratamiento pTratamiento |
| ModificarTratamiento | bool | Tratamiento pTratamiento, List<MovimientoStock> pMovimientos, List<Diagnostico> pDiagnosticosActualizados |
| EliminarTratamiento | bool | int pIdTratamiento, List<MovimientoStock> pMovimientos, List<Diagnostico> pDiagnosticosActualizados |

### pVacunacion

| Método | Devuelve | Parámetros |
|---|---|---|
| ListarVacunaciones | List<Vacunacion> | List<Animal> pListaAnimales, List<Insumo> pListaInsumos, List<PlanSanitario> pListaPlanes |
| AltaVacunacion | bool | Vacunacion pVacunacion, double pCantidadInsumo |
| ModificarVacunacion | bool | Vacunacion pVacunacion, List<MovimientoStock> pMovimientos |
| EliminarVacunacion | bool | int pIdVacunacion, List<MovimientoStock> pMovimientos |
