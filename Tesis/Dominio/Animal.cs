namespace Tesis.Dominio
{
    public class Animal
    {
        private int mIdAnimal;
        private string mNumCaravana;
        private DateTime mFechaNacimiento;
        private bool mActivo;
        private DateTime mFechaBaja;
        private string mMotivoBaja;
        private Raza mRaza;
        private Categoria mCategoria;
        private Hembra mMadre;
        private Macho mPadre;

        // Nombre del archivo de la foto dentro de wwwroot/fotos, o vacio si el animal
        // no tiene ninguna. No va en el constructor a proposito: cuando el animal se
        // arma el archivo todavia no se escribio en el disco, asi que su nombre no
        // existe. La pantalla guarda primero la imagen y recien despues completa esta
        // propiedad con el nombre que devolvio el guardado.
        private string mFoto = "";

        public int IdAnimal { get { return mIdAnimal; } set { mIdAnimal = value; } }
        public string NumCaravana { get { return mNumCaravana; } set { mNumCaravana = value; } }
        public DateTime FechaNacimiento { get { return mFechaNacimiento; } set { mFechaNacimiento = value; } }
        public bool Activo { get { return mActivo; } set { mActivo = value; } }
        public DateTime FechaBaja { get { return mFechaBaja; } set { mFechaBaja = value; } }
        public string MotivoBaja { get { return mMotivoBaja; } set { mMotivoBaja = value; } }
        public Raza Raza { get { return mRaza; } set { mRaza = value; } }
        public Categoria Categoria { get { return mCategoria; } set { mCategoria = value; } }
        public Hembra Madre { get { return mMadre; } set { mMadre = value; } }
        public Macho Padre { get { return mPadre; } set { mPadre = value; } }
        public string Foto { get { return mFoto; } set { mFoto = value ?? ""; } }

        // Verdadero cuando el animal tiene una foto cargada. Lo usan las pantallas
        // para decidir entre la imagen y la silueta de reemplazo.
        public bool TieneFoto { get { return mFoto != ""; } }

        public Animal(int pIdAnimal, string pNumCaravana, DateTime pFechaNacimiento, bool pActivo,
            DateTime pFechaBaja, string pMotivoBaja, Raza pRaza, Categoria pCategoria, Hembra pMadre, Macho pPadre)
        {
            mIdAnimal = pIdAnimal;
            mNumCaravana = pNumCaravana;
            mFechaNacimiento = pFechaNacimiento;
            mActivo = pActivo;
            mFechaBaja = pFechaBaja;
            mMotivoBaja = pMotivoBaja;
            mRaza = pRaza;
            mCategoria = pCategoria;
            mMadre = pMadre;
            mPadre = pPadre;
        }
    }
}
