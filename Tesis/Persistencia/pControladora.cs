using Tesis.Dominio;

namespace Tesis.Persistencia
{
    public class pControladora
    {
        #region CONFIGURACION
        // Parametros de manejo del establecimiento. Devuelve nulo cuando la tabla esta
        // vacia: el dominio responde con los valores por defecto.
        public Configuracion ObtenerConfiguracion()
        {
            return new pConfiguracion().ObtenerConfiguracion();
        }

        public bool ModificarConfiguracion(Configuracion pConfiguracionNueva)
        {
            return new pConfiguracion().ModificarConfiguracion(pConfiguracionNueva);
        }
        #endregion

        #region RAZAS
        public List<Raza> ListarRazas()
        {
            return new pRaza().ListarRazas();
        }

        public bool AltaRaza(Raza pRaza)
        {
            return new pRaza().AltaRaza(pRaza);
        }
        #endregion

        #region CATEGORIAS
        public List<Categoria> ListarCategorias()
        {
            return new pCategoria().ListarCategorias();
        }

        public bool AltaCategoria(Categoria pCategoria)
        {
            return new pCategoria().AltaCategoria(pCategoria);
        }
        #endregion

        #region ANIMALES
        // Razas y categorias van como parametro: pAnimal las necesita para resolver
        // cada animal y ya no se las pide al dominio.
        public List<Animal> ListarAnimales(List<Raza> pListaRazas, List<Categoria> pListaCategorias)
        {
            return new pAnimal().ListarAnimales(pListaRazas, pListaCategorias);
        }

        public bool AltaAnimal(Animal pAnimal)
        {
            return new pAnimal().AltaAnimal(pAnimal);
        }

        public bool ModificarAnimal(Animal pAnimal)
        {
            return new pAnimal().ModificarAnimal(pAnimal);
        }

        public bool BajaAnimal(int pIdAnimal, string pMotivoBaja)
        {
            return new pAnimal().BajaAnimal(pIdAnimal, pMotivoBaja);
        }

        // La baja es reversible: un error de carga no puede dejar al animal fuera del
        // rodeo para siempre
        public bool ReactivarAnimal(int pIdAnimal)
        {
            return new pAnimal().ReactivarAnimal(pIdAnimal);
        }

        // La foto es un archivo, no una fila: se guarda en el disco y a la base va
        // nada mas que su nombre.
        public string GuardarFoto(byte[] pContenido)
        {
            return new pFotoAnimal().Guardar(pContenido);
        }

        public void BorrarFoto(string pNombreArchivo)
        {
            new pFotoAnimal().Borrar(pNombreArchivo);
        }
        #endregion

        #region HEMBRAS
        public List<Hembra> ListarHembras(List<Animal> pListaAnimales)
        {
            return new pHembra().ListarHembras(pListaAnimales);
        }

        public bool AltaHembra(Hembra pHembra)
        {
            return new pHembra().AltaHembra(pHembra);
        }

        public bool ModificarHembra(Hembra pHembra)
        {
            return new pHembra().ModificarHembra(pHembra);
        }
        #endregion

        #region MACHOS
        public List<Macho> ListarMachos(List<Animal> pListaAnimales)
        {
            return new pMacho().ListarMachos(pListaAnimales);
        }

        public bool AltaMacho(Macho pMacho)
        {
            return new pMacho().AltaMacho(pMacho);
        }

        public bool ModificarMacho(Macho pMacho)
        {
            return new pMacho().ModificarMacho(pMacho);
        }
        #endregion

        #region LACTANCIAS
        public List<Lactancia> ListarLactancias(List<Hembra> pListaHembras)
        {
            return new pLactancia().ListarLactancias(pListaHembras);
        }

        public bool AltaLactancia(Lactancia pLactanciaNueva)
        {
            return new pLactancia().AltaLactancia(pLactanciaNueva);
        }

        public bool ModificarLactancia(Lactancia pLactanciaNueva)
        {
            return new pLactancia().ModificarLactancia(pLactanciaNueva);
        }

        // Cierra la lactancia y deja seca a la hembra dentro de una misma transaccion
        public bool RegistrarSecado(Lactancia pLactanciaCerrada, Hembra pHembraSeca)
        {
            return new pLactancia().RegistrarSecado(pLactanciaCerrada, pHembraSeca);
        }
        #endregion

        #region ORDENIES POR LOTE
        public List<OrdenieLote> ListarOrdeniesLote(List<Hembra> pListaHembras)
        {
            return new pOrdenieLote().ListarOrdeniesLote(pListaHembras);
        }

        public bool AltaOrdenieLote(OrdenieLote pOrdenieLote)
        {
            return new pOrdenieLote().AltaOrdenieLote(pOrdenieLote);
        }

        public bool ModificarOrdenieLote(OrdenieLote pOrdenieLote)
        {
            return new pOrdenieLote().ModificarOrdenieLote(pOrdenieLote);
        }

        public bool EliminarOrdenieLote(int pIdOrdenieLote)
        {
            return new pOrdenieLote().EliminarOrdenieLote(pIdOrdenieLote);
        }
        #endregion

        #region ORDENIES INDIVIDUALES
        public List<OrdenieIndividual> ListarOrdeniesIndividual(List<Hembra> pListaHembras,
            List<Lactancia> pListaLactancias, List<OrdenieLote> pListaOrdeniesLote)
        {
            return new pOrdenieIndividual().ListarOrdeniesIndividual(pListaHembras,
                pListaLactancias, pListaOrdeniesLote);
        }

        public bool AltaOrdenieIndividual(OrdenieIndividual pOrdenie)
        {
            return new pOrdenieIndividual().AltaOrdenieIndividual(pOrdenie);
        }

        public bool ModificarOrdenieIndividual(OrdenieIndividual pOrdenie)
        {
            return new pOrdenieIndividual().ModificarOrdenieIndividual(pOrdenie);
        }

        public bool EliminarOrdenieIndividual(int pIdOrdenieInd)
        {
            return new pOrdenieIndividual().EliminarOrdenieIndividual(pIdOrdenieInd);
        }
        #endregion

        #region CELOS
        public List<Celo> ListarCelos(List<Hembra> pListaHembras)
        {
            return new pCelo().ListarCelos(pListaHembras);
        }

        public bool AltaCelo(Celo pCelo)
        {
            return new pCelo().AltaCelo(pCelo);
        }

        public bool ModificarCelo(Celo pCelo)
        {
            return new pCelo().ModificarCelo(pCelo);
        }

        public bool EliminarCelo(int pIdCelo)
        {
            return new pCelo().EliminarCelo(pIdCelo);
        }
        #endregion

        #region SERVICIOS
        public List<Servicio> ListarServicios(List<Hembra> pListaHembras, List<Macho> pListaMachos,
            List<Insumo> pListaInsumos)
        {
            return new pServicio().ListarServicios(pListaHembras, pListaMachos, pListaInsumos);
        }

        // Deja servida a la hembra y, en la inseminacion, descuenta la pajuela del
        // stock dentro de la misma transaccion
        public bool AltaServicio(Servicio pServicio, Hembra pHembraServida)
        {
            return new pServicio().AltaServicio(pServicio, pHembraServida);
        }

        // La correccion baja a la lactancia en curso -de donde sale la fecha recomendada
        // de secado-, al estado de la hembra y, si cambio la pajuela, al stock
        public bool ModificarServicio(Servicio pServicio, List<Lactancia> pLactanciasActualizadas,
            List<Hembra> pHembrasActualizadas, List<MovimientoStock> pMovimientos)
        {
            return new pServicio().ModificarServicio(pServicio, pLactanciasActualizadas,
                pHembrasActualizadas, pMovimientos);
        }

        // Borra el servicio, deja a la hembra con el estado que le dan los servicios
        // que le quedan y devuelve al stock la pajuela que no se uso
        public bool EliminarServicio(int pIdServicio, List<Lactancia> pLactanciasActualizadas,
            List<Hembra> pHembrasActualizadas, List<MovimientoStock> pMovimientos)
        {
            return new pServicio().EliminarServicio(pIdServicio, pLactanciasActualizadas,
                pHembrasActualizadas, pMovimientos);
        }
        #endregion

        #region TACTOS
        public List<Tacto> ListarTactos(List<Servicio> pListaServicios)
        {
            return new pTacto().ListarTactos(pListaServicios);
        }

        // Guarda el tacto, actualiza el estado reproductivo de la hembra y baja la
        // fecha probable de parto a la lactancia en curso, todo en una transaccion
        public bool AltaTacto(Tacto pTacto, Hembra pHembraTactada, Lactancia pLactanciaVigente)
        {
            return new pTacto().AltaTacto(pTacto, pHembraTactada, pLactanciaVigente);
        }

        public bool ModificarTacto(Tacto pTacto, List<Hembra> pHembrasActualizadas,
            List<Lactancia> pLactanciasActualizadas)
        {
            return new pTacto().ModificarTacto(pTacto, pHembrasActualizadas, pLactanciasActualizadas);
        }

        public bool EliminarTacto(int pIdTacto, List<Hembra> pHembrasActualizadas,
            List<Lactancia> pLactanciasActualizadas)
        {
            return new pTacto().EliminarTacto(pIdTacto, pHembrasActualizadas, pLactanciasActualizadas);
        }
        #endregion

        #region PARTOS
        public List<Parto> ListarPartos(List<Hembra> pListaHembras)
        {
            return new pParto().ListarPartos(pListaHembras);
        }

        // Asienta el parto, da de alta las crias, cierra la lactancia anterior si habia
        // quedado abierta, abre la nueva y actualiza a la madre, todo en una transaccion
        public bool AltaParto(Parto pParto, List<Animal> pListaCrias, Lactancia pNuevaLactancia,
            Hembra pMadreActualizada, Lactancia pLactanciaCerrada)
        {
            return new pParto().AltaParto(pParto, pListaCrias, pNuevaLactancia,
                pMadreActualizada, pLactanciaCerrada);
        }

        // Corregir la fecha del parto corre tambien las lactancias que movio y la fecha
        // de nacimiento de las crias, que nacieron ese dia
        public bool ModificarParto(Parto pParto, List<Lactancia> pLactanciasActualizadas,
            List<Animal> pListaCrias)
        {
            return new pParto().ModificarParto(pParto, pLactanciasActualizadas, pListaCrias);
        }

        // Deshace el parto entero: borra las crias y la lactancia que abrio, reabre la
        // que habia cerrado y le devuelve a la madre el parto, el estado y la categoria
        public bool EliminarParto(int pIdParto, List<Animal> pListaCrias, Lactancia pLactanciaDelParto,
            Hembra pMadreActualizada, Lactancia pLactanciaReabierta)
        {
            return new pParto().EliminarParto(pIdParto, pListaCrias, pLactanciaDelParto,
                pMadreActualizada, pLactanciaReabierta);
        }
        #endregion

        #region INSUMOS
        public List<Insumo> ListarInsumos(List<Macho> pListaMachos)
        {
            return new pInsumo().ListarInsumos(pListaMachos);
        }

        public bool AltaInsumo(Insumo pInsumoNuevo)
        {
            return new pInsumo().AltaInsumo(pInsumoNuevo);
        }

        // CU36: el umbral que dispara la alerta de reposicion
        public bool ModificarStockMinimo(int pIdInsumo, double pStockMinimo)
        {
            return new pInsumo().ModificarStockMinimo(pIdInsumo, pStockMinimo);
        }
        #endregion

        #region MOVIMIENTOS DE STOCK
        public List<MovimientoStock> ListarMovimientosStock(List<Insumo> pListaInsumos)
        {
            return new pMovimientoStock().ListarMovimientos(pListaInsumos);
        }

        // Asienta la partida y suma el stock del insumo en una misma transaccion
        public bool RegistrarIngresoStock(MovimientoStock pMovimiento)
        {
            return new pMovimientoStock().RegistrarIngreso(pMovimiento);
        }
        #endregion

        #region PLANES SANITARIOS
        public List<PlanSanitario> ListarPlanesSanitarios(List<Insumo> pListaInsumos,
            List<Categoria> pListaCategorias)
        {
            return new pPlanSanitario().ListarPlanes(pListaInsumos, pListaCategorias);
        }

        // El plan y las categorias que alcanza se guardan en una misma transaccion
        public bool AltaPlanSanitario(PlanSanitario pPlan)
        {
            return new pPlanSanitario().AltaPlan(pPlan);
        }

        public bool ModificarPlanSanitario(PlanSanitario pPlan)
        {
            return new pPlanSanitario().ModificarPlan(pPlan);
        }
        #endregion

        #region VACUNACIONES
        public List<Vacunacion> ListarVacunaciones(List<Animal> pListaAnimales,
            List<Insumo> pListaInsumos, List<PlanSanitario> pListaPlanes)
        {
            return new pVacunacion().ListarVacunaciones(pListaAnimales, pListaInsumos, pListaPlanes);
        }

        // Registra la aplicacion y descuenta la vacuna del stock en una transaccion
        public bool AltaVacunacion(Vacunacion pVacunacion, double pCantidadInsumo)
        {
            return new pVacunacion().AltaVacunacion(pVacunacion, pCantidadInsumo);
        }

        public bool ModificarVacunacion(Vacunacion pVacunacion, List<MovimientoStock> pMovimientos)
        {
            return new pVacunacion().ModificarVacunacion(pVacunacion, pMovimientos);
        }

        public bool EliminarVacunacion(int pIdVacunacion, List<MovimientoStock> pMovimientos)
        {
            return new pVacunacion().EliminarVacunacion(pIdVacunacion, pMovimientos);
        }
        #endregion

        #region DESCORNES
        public List<Descorne> ListarDescornes(List<Animal> pListaAnimales,
            List<PlanSanitario> pListaPlanes)
        {
            return new pDescorne().ListarDescornes(pListaAnimales, pListaPlanes);
        }

        public bool AltaDescorne(Descorne pDescorne)
        {
            return new pDescorne().AltaDescorne(pDescorne);
        }

        public bool ModificarDescorne(Descorne pDescorne)
        {
            return new pDescorne().ModificarDescorne(pDescorne);
        }

        public bool EliminarDescorne(int pIdDescorne)
        {
            return new pDescorne().EliminarDescorne(pIdDescorne);
        }
        #endregion

        #region DIAGNOSTICOS
        public List<Diagnostico> ListarDiagnosticos(List<Animal> pListaAnimales)
        {
            return new pDiagnostico().ListarDiagnosticos(pListaAnimales);
        }

        public bool AltaDiagnostico(Diagnostico pDiagnosticoNuevo)
        {
            return new pDiagnostico().AltaDiagnostico(pDiagnosticoNuevo);
        }

        public bool ModificarDiagnostico(int pIdDiagnostico, string pEstado)
        {
            return new pDiagnostico().ModificarDiagnostico(pIdDiagnostico, pEstado);
        }

        public bool ModificarDiagnostico(Diagnostico pDiagnostico)
        {
            return new pDiagnostico().ModificarDiagnostico(pDiagnostico);
        }

        public bool EliminarDiagnostico(int pIdDiagnostico)
        {
            return new pDiagnostico().EliminarDiagnostico(pIdDiagnostico);
        }
        #endregion

        #region TRATAMIENTOS
        public List<Tratamiento> ListarTratamientos(List<Diagnostico> pListaDiagnosticos,
            List<Insumo> pListaInsumos, List<Animal> pListaAnimales, List<PlanSanitario> pListaPlanes)
        {
            return new pTratamiento().ListarTratamientos(pListaDiagnosticos, pListaInsumos,
                pListaAnimales, pListaPlanes);
        }

        // La cantidad aplicada viaja en el tratamiento: es lo que despues permite
        // devolverle al inventario lo que nunca se uso si el registro se corrige
        public bool AltaTratamiento(Tratamiento pTratamientoNuevo)
        {
            return new pTratamiento().AltaTratamiento(pTratamientoNuevo);
        }

        public bool ModificarTratamiento(Tratamiento pTratamiento, List<MovimientoStock> pMovimientos,
            List<Diagnostico> pDiagnosticosActualizados)
        {
            return new pTratamiento().ModificarTratamiento(pTratamiento, pMovimientos,
                pDiagnosticosActualizados);
        }

        public bool EliminarTratamiento(int pIdTratamiento, List<MovimientoStock> pMovimientos,
            List<Diagnostico> pDiagnosticosActualizados)
        {
            return new pTratamiento().EliminarTratamiento(pIdTratamiento, pMovimientos,
                pDiagnosticosActualizados);
        }
        #endregion
    }
}
