namespace Tesis.Dominio
{
    public class Tacto
    {
        // Resultados admitidos del diagnostico de gestacion. El dudoso deja a la
        // hembra como esta, a la espera de un nuevo control.
        public const string PRENADA = "Preñada";
        public const string VACIA = "Vacía";
        public const string DUDOSA = "Dudosa";

        private int mIdTacto;
        private DateTime mFechaTacto;
        private string mResultado;
        private string mObservaciones;
        private Servicio mServicio;

        public int IdTacto { get { return mIdTacto; } set { mIdTacto = value; } }
        public DateTime FechaTacto { get { return mFechaTacto; } set { mFechaTacto = value; } }
        public string Resultado { get { return mResultado; } set { mResultado = value; } }
        public string Observaciones { get { return mObservaciones; } set { mObservaciones = value; } }
        public Servicio Servicio { get { return mServicio; } set { mServicio = value; } }

        public Tacto(int pIdTacto, DateTime pFechaTacto, string pResultado,
            string pObservaciones, Servicio pServicio)
        {
            mIdTacto = pIdTacto;
            mFechaTacto = pFechaTacto;
            mResultado = pResultado;
            mObservaciones = pObservaciones;
            mServicio = pServicio;
        }
    }
}
