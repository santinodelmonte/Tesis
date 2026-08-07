namespace Tesis.Dominio
{
    public class Celo
    {
        private int mIdCelo;
        private DateTime mFecha;
        private string mObservaciones;
        private Hembra mAnimal;

        public int IdCelo { get { return mIdCelo; } set { mIdCelo = value; } }
        public DateTime Fecha { get { return mFecha; } set { mFecha = value; } }
        public string Observaciones { get { return mObservaciones; } set { mObservaciones = value; } }
        public Hembra Animal { get { return mAnimal; } set { mAnimal = value; } }

        public Celo(int pIdCelo, DateTime pFecha, string pObservaciones, Hembra pAnimal)
        {
            mIdCelo = pIdCelo;
            mFecha = pFecha;
            mObservaciones = pObservaciones;
            mAnimal = pAnimal;
        }
    }
}
