namespace Tesis.Dominio
{
    public class Macho : Animal
    {
        private bool mEnPie;

        public bool EnPie { get { return mEnPie; } set { mEnPie = value; } }

        public Macho(int pIdAnimal, string pNumCaravana, DateTime pFechaNacimiento, bool pActivo,
            DateTime pFechaBaja, string pMotivoBaja, Raza pRaza, Categoria pCategoria, Hembra pMadre, Macho pPadre,
            bool pEnPie)
            : base(pIdAnimal, pNumCaravana, pFechaNacimiento, pActivo, pFechaBaja, pMotivoBaja, pRaza, pCategoria, pMadre, pPadre)
        {
            mEnPie = pEnPie;
        }
    }
}
