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
    }
}
