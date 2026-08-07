namespace Tesis.Dominio
{
    public class Lactancia
    {
        private int mIdLactancia;
        private int mNumeroLactancia;
        private DateTime mFechaInicio;
        private DateTime mFechaSecado;
        private DateTime mFechaProbableParto;
        private Hembra mAnimal;

        public int IdLactancia { get { return mIdLactancia; } set { mIdLactancia = value; } }
        public int NumeroLactancia { get { return mNumeroLactancia; } set { mNumeroLactancia = value; } }
        public DateTime FechaInicio { get { return mFechaInicio; } set { mFechaInicio = value; } }
        public DateTime FechaSecado { get { return mFechaSecado; } set { mFechaSecado = value; } }
        public DateTime FechaProbableParto { get { return mFechaProbableParto; } set { mFechaProbableParto = value; } }
        public Hembra Animal { get { return mAnimal; } set { mAnimal = value; } }

        public Lactancia(int pIdLactancia, int pNumeroLactancia, DateTime pFechaInicio,
            DateTime pFechaSecado, DateTime pFechaProbableParto, Hembra pAnimal)
        {
            mIdLactancia = pIdLactancia;
            mNumeroLactancia = pNumeroLactancia;
            mFechaInicio = pFechaInicio;
            mFechaSecado = pFechaSecado;
            mFechaProbableParto = pFechaProbableParto;
            mAnimal = pAnimal;
        }
    }
}
