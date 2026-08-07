namespace Tesis.Dominio
{
    public class Parto
    {
        // Tipos de parto admitidos.
        public const string NORMAL = "Normal";
        public const string DISTOCICO = "Distócico";
        public const string CESAREA = "Cesárea";

        private int mIdParto;
        private DateTime mFechaParto;
        private string mTipoParto;
        private string mObservaciones;
        private Hembra mMadre;

        public int IdParto { get { return mIdParto; } set { mIdParto = value; } }
        public DateTime FechaParto { get { return mFechaParto; } set { mFechaParto = value; } }
        public string TipoParto { get { return mTipoParto; } set { mTipoParto = value; } }
        public string Observaciones { get { return mObservaciones; } set { mObservaciones = value; } }
        public Hembra Madre { get { return mMadre; } set { mMadre = value; } }

        public Parto(int pIdParto, DateTime pFechaParto, string pTipoParto,
            string pObservaciones, Hembra pMadre)
        {
            mIdParto = pIdParto;
            mFechaParto = pFechaParto;
            mTipoParto = pTipoParto;
            mObservaciones = pObservaciones;
            mMadre = pMadre;
        }
    }
}
