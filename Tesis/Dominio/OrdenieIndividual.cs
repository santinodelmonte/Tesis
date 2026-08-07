namespace Tesis.Dominio
{
    public class OrdenieIndividual
    {
        private int mIdOrdenieInd;
        private DateTime mFecha;
        private string mTurno;
        private double mLitros;
        private Hembra mAnimal;
        private Lactancia mLactancia;
        private OrdenieLote mOrdenieLote;

        public int IdOrdenieInd { get { return mIdOrdenieInd; } set { mIdOrdenieInd = value; } }
        public DateTime Fecha { get { return mFecha; } set { mFecha = value; } }
        public string Turno { get { return mTurno; } set { mTurno = value; } }
        public double Litros { get { return mLitros; } set { mLitros = value; } }
        public Hembra Animal { get { return mAnimal; } set { mAnimal = value; } }
        public Lactancia Lactancia { get { return mLactancia; } set { mLactancia = value; } }
        public OrdenieLote OrdenieLote { get { return mOrdenieLote; } set { mOrdenieLote = value; } }

        public OrdenieIndividual(int pIdOrdenieInd, DateTime pFecha, string pTurno, double pLitros,
            Hembra pAnimal, Lactancia pLactancia, OrdenieLote pOrdenieLote)
        {
            mIdOrdenieInd = pIdOrdenieInd;
            mFecha = pFecha;
            mTurno = pTurno;
            mLitros = pLitros;
            mAnimal = pAnimal;
            mLactancia = pLactancia;
            mOrdenieLote = pOrdenieLote;
        }
    }
}
