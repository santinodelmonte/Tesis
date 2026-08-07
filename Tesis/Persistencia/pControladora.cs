using Tesis.Dominio;

namespace Tesis.Persistencia
{
    public class pControladora
    {
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

        // La correccion de la fecha probable de parto baja tambien a la lactancia en
        // curso, que es de donde sale la fecha recomendada de secado
        public bool ModificarServicio(Servicio pServicio, Lactancia pLactanciaVigente)
        {
            return new pServicio().ModificarServicio(pServicio, pLactanciaVigente);
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
        #endregion

        #region INSUMOS
        // Adelantado del Modulo 5: CU15 necesita las pajuelas del stock
        public List<Insumo> ListarInsumos(List<Macho> pListaMachos)
        {
            return new pInsumo().ListarInsumos(pListaMachos);
        }

        public bool AltaInsumo(Insumo pInsumoNuevo)
        {
            return new pInsumo().AltaInsumo(pInsumoNuevo);
        }

        public bool ActualizarStock(int pIdInsumo, double pStockActual)
        {
            return new pInsumo().ActualizarStock(pIdInsumo, pStockActual);
        }
        #endregion

        #region MOVIMIENTOS DE STOCK
        public List<MovimientoStock> ListarMovimientosStock(List<Insumo> pListaInsumos)
        {
            return new pMovimientoStock().ListarMovimientos(pListaInsumos);
        }

        public bool AltaMovimientoStock(MovimientoStock pMovimiento)
        {
            return new pMovimientoStock().AltaMovimiento(pMovimiento);
        }
        #endregion

        #region DIAGNOSTICOS
        // Adelantado del Modulo 4: el paso 3 de CU8 excluye del lote a los animales
        // con descarte de leche vigente, y el descarte sale del tratamiento
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
        #endregion

        #region TRATAMIENTOS
        public List<Tratamiento> ListarTratamientos(List<Diagnostico> pListaDiagnosticos,
            List<Insumo> pListaInsumos)
        {
            return new pTratamiento().ListarTratamientos(pListaDiagnosticos, pListaInsumos);
        }

        public bool AltaTratamiento(Tratamiento pTratamientoNuevo, double pCantidadInsumo)
        {
            return new pTratamiento().AltaTratamiento(pTratamientoNuevo, pCantidadInsumo);
        }
        #endregion
    }
}
