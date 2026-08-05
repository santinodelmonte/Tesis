using Tesis.Dominio;

namespace Tesis.Persistencia
{
    public class pControladora
    {
        private pConexion Conexion = new pConexion();

        #region RAZAS
        public List<Raza> ListarRazas()
        {
            return new pRaza().ListarRazas();
        }

        public bool AltaRaza(Raza pRaza)
        {
            return new pRaza().AltaRaza(pRaza);
        }

        public int ProximoRazaId()
        {
            return new pRaza().ProximoRazaId();
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

        public int ProximoCategoriaId()
        {
            return new pCategoria().ProximoCategoriaId();
        }
        #endregion

        #region ANIMALES
        public List<Animal> ListarAnimales()
        {
            return new pAnimal().ListarAnimales();
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

        public bool EliminarAnimal(int pIdAnimal)
        {
            return new pAnimal().EliminarAnimal(pIdAnimal);
        }

        public int ProximoAnimalId()
        {
            return new pAnimal().ProximoAnimalId();
        }
        #endregion

        #region HEMBRAS
        public List<Hembra> ListarHembras()
        {
            return new pHembra().ListarHembras();
        }

        public bool AltaHembra(Hembra pHembra)
        {
            return new pHembra().AltaHembra(pHembra);
        }

        public bool ModificarHembra(Hembra pHembra)
        {
            return new pHembra().ModificarHembra(pHembra);
        }

        public int ProximoHembraId()
        {
            return new pHembra().ProximoHembraId();
        }
        #endregion

        #region MACHOS
        public List<Macho> ListarMachos()
        {
            return new pMacho().ListarMachos();
        }

        public bool AltaMacho(Macho pMacho)
        {
            return new pMacho().AltaMacho(pMacho);
        }

        public int ProximoMachoId()
        {
            return new pMacho().ProximoMachoId();
        }
        #endregion
    }
}
