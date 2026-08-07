namespace Tesis.Dominio
{
    public class Servicio
    {
        // Tipos de servicio admitidos. Determinan cual de los dos reproductores se
        // completa: el toro del rodeo o la pajuela del stock.
        public const string MONTA_NATURAL = "Monta natural";
        public const string INSEMINACION = "Inseminación artificial";

        private int mIdServicio;
        private string mTipoServicio;
        private DateTime mFechaServicio;
        private DateTime mFechaProbableParto;
        private string mObservaciones;
        private Hembra mAnimal;
        private Macho mToro;
        private Insumo mPajuela;

        public int IdServicio { get { return mIdServicio; } set { mIdServicio = value; } }
        public string TipoServicio { get { return mTipoServicio; } set { mTipoServicio = value; } }
        public DateTime FechaServicio { get { return mFechaServicio; } set { mFechaServicio = value; } }
        public DateTime FechaProbableParto { get { return mFechaProbableParto; } set { mFechaProbableParto = value; } }
        public string Observaciones { get { return mObservaciones; } set { mObservaciones = value; } }
        public Hembra Animal { get { return mAnimal; } set { mAnimal = value; } }
        public Macho Toro { get { return mToro; } set { mToro = value; } }
        public Insumo Pajuela { get { return mPajuela; } set { mPajuela = value; } }

        public Servicio(int pIdServicio, string pTipoServicio, DateTime pFechaServicio,
            DateTime pFechaProbableParto, string pObservaciones, Hembra pAnimal,
            Macho pToro, Insumo pPajuela)
        {
            mIdServicio = pIdServicio;
            mTipoServicio = pTipoServicio;
            mFechaServicio = pFechaServicio;
            mFechaProbableParto = pFechaProbableParto;
            mObservaciones = pObservaciones;
            mAnimal = pAnimal;
            mToro = pToro;
            mPajuela = pPajuela;
        }
    }
}
