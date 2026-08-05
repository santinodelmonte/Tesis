namespace Tesis.Dominio
{
    public class Hembra : Animal
    {
        private int mNumeroPartos;
        private string mEstadoProductivo;
        private string mEstadoReproductivo;

        public int NumeroPartos { get { return mNumeroPartos; } set { mNumeroPartos = value; } }
        public string EstadoProductivo { get { return mEstadoProductivo; } set { mEstadoProductivo = value; } }
        public string EstadoReproductivo { get { return mEstadoReproductivo; } set { mEstadoReproductivo = value; } }

        public Hembra(int pIdAnimal, string pNumCaravana, DateTime pFechaNacimiento, bool pActivo,
            DateTime pFechaBaja, string pMotivoBaja, Raza pRaza, Categoria pCategoria, Hembra pMadre, Macho pPadre,
            int pNumeroPartos, string pEstadoProductivo, string pEstadoReproductivo)
            : base(pIdAnimal, pNumCaravana, pFechaNacimiento, pActivo, pFechaBaja, pMotivoBaja, pRaza, pCategoria, pMadre, pPadre)
        {
            mNumeroPartos = pNumeroPartos;
            mEstadoProductivo = pEstadoProductivo;
            mEstadoReproductivo = pEstadoReproductivo;
        }
    }
}
