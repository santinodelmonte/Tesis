namespace Tesis.Dominio
{
    public class OrdenieLote
    {
        // Turnos admitidos. Se usan tanto al guardar como al comparar, para que no
        // queden literales sueltos por el codigo.
        // El turno se nombra por su numero. No son constantes fijas porque la cantidad
        // de ordenies por dia es un parametro del establecimiento: hay tambos que
        // ordenian dos veces y otros tres, y la lista de turnos sale de ahi
        // (Controladora.ListarTurnos).
        public const string PREFIJO_TURNO = "Turno ";

        public static string NombreTurno(int pNumero)
        {
            return PREFIJO_TURNO + pNumero;
        }

        private int mIdOrdenieLote;
        private DateTime mFecha;
        private string mTurno;
        private double mLitrosTotales;

        // Animales que integraron el lote de ese turno. El Modelo Entidad-Relacion no
        // los contempla: se persisten en la tabla ordenie_lote_animales para que el
        // paso 4 de CU8, donde el usuario ajusta la lista, deje rastro.
        private List<Hembra> mAnimales;

        public int IdOrdenieLote { get { return mIdOrdenieLote; } set { mIdOrdenieLote = value; } }
        public DateTime Fecha { get { return mFecha; } set { mFecha = value; } }
        public string Turno { get { return mTurno; } set { mTurno = value; } }
        public double LitrosTotales { get { return mLitrosTotales; } set { mLitrosTotales = value; } }
        public List<Hembra> Animales { get { return mAnimales; } set { mAnimales = value; } }

        public OrdenieLote(int pIdOrdenieLote, DateTime pFecha, string pTurno,
            double pLitrosTotales, List<Hembra> pAnimales)
        {
            mIdOrdenieLote = pIdOrdenieLote;
            mFecha = pFecha;
            mTurno = pTurno;
            mLitrosTotales = pLitrosTotales;
            mAnimales = pAnimales;
        }
    }
}
