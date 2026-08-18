namespace Tesis.Dominio
{
    // Adelantada del Modulo 4. Es el origen del tratamiento, y el tratamiento es lo
    // que define el periodo de descarte de leche que el paso 3 de CU12 usa para
    // excluir animales del lote de ordenie.
    public class Diagnostico
    {
        // Estados admitidos del diagnostico.
        public const string ACTIVO = "Activo";
        public const string EN_TRATAMIENTO = "En tratamiento";
        public const string RESUELTO = "Resuelto";

        private int mIdDiagnostico;
        private DateTime mFechaDiagnostico;
        private string mEnfermedad;
        private string mEstado;
        private Animal mAnimal;

        public int IdDiagnostico { get { return mIdDiagnostico; } set { mIdDiagnostico = value; } }
        public DateTime FechaDiagnostico { get { return mFechaDiagnostico; } set { mFechaDiagnostico = value; } }
        public string Enfermedad { get { return mEnfermedad; } set { mEnfermedad = value; } }
        public string Estado { get { return mEstado; } set { mEstado = value; } }
        public Animal Animal { get { return mAnimal; } set { mAnimal = value; } }

        public Diagnostico(int pIdDiagnostico, DateTime pFechaDiagnostico, string pEnfermedad,
            string pEstado, Animal pAnimal)
        {
            mIdDiagnostico = pIdDiagnostico;
            mFechaDiagnostico = pFechaDiagnostico;
            mEnfermedad = pEnfermedad;
            mEstado = pEstado;
            mAnimal = pAnimal;
        }
    }
}
