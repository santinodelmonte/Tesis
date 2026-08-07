namespace Tesis.Dominio
{
    // Adelantada del Modulo 4. Lo que interesa aca es fecha_fin_descarte: mientras
    // no venza, la hembra tratada queda fuera del lote de ordenie (CU8, paso 3).
    //
    // El atributo mPlan que declara el Diccionario de Clases no esta: la clase
    // PlanSanitario se incorpora con el Modulo 4.
    public class Tratamiento
    {
        private int mIdTratamiento;
        private DateTime mFechaInicio;
        private int mDiasDuracion;
        private string mDosisDiaria;
        private DateTime mFechaFinDescarte;
        private Diagnostico mDiagnostico;
        private Insumo mInsumo;

        public int IdTratamiento { get { return mIdTratamiento; } set { mIdTratamiento = value; } }
        public DateTime FechaInicio { get { return mFechaInicio; } set { mFechaInicio = value; } }
        public int DiasDuracion { get { return mDiasDuracion; } set { mDiasDuracion = value; } }
        public string DosisDiaria { get { return mDosisDiaria; } set { mDosisDiaria = value; } }
        public DateTime FechaFinDescarte { get { return mFechaFinDescarte; } set { mFechaFinDescarte = value; } }
        public Diagnostico Diagnostico { get { return mDiagnostico; } set { mDiagnostico = value; } }
        public Insumo Insumo { get { return mInsumo; } set { mInsumo = value; } }

        public Tratamiento(int pIdTratamiento, DateTime pFechaInicio, int pDiasDuracion,
            string pDosisDiaria, DateTime pFechaFinDescarte, Diagnostico pDiagnostico, Insumo pInsumo)
        {
            mIdTratamiento = pIdTratamiento;
            mFechaInicio = pFechaInicio;
            mDiasDuracion = pDiasDuracion;
            mDosisDiaria = pDosisDiaria;
            mFechaFinDescarte = pFechaFinDescarte;
            mDiagnostico = pDiagnostico;
            mInsumo = pInsumo;
        }
    }
}
