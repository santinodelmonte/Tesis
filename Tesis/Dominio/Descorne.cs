namespace Tesis.Dominio
{
    // Procedimiento de descorne (CU32). Es de aplicacion unica: una vez registrado,
    // el plan de descorne deja de exigirlo para ese animal.
    public class Descorne
    {
        // Metodos habituales de la recria.
        public const string PASTA_CAUSTICA = "Pasta caustica";
        public const string TERMOCAUTERIO = "Termocauterio";
        public const string QUIRURGICO = "Quirurgico";
        public const string OTRO = "Otro";

        private int mIdDescorne;
        private DateTime mFecha;
        private string mMetodo;
        private string mObservaciones;
        private Animal mAnimal;
        private PlanSanitario mPlan;

        public int IdDescorne { get { return mIdDescorne; } set { mIdDescorne = value; } }
        public DateTime Fecha { get { return mFecha; } set { mFecha = value; } }
        public string Metodo { get { return mMetodo; } set { mMetodo = value; } }
        public string Observaciones { get { return mObservaciones; } set { mObservaciones = value; } }
        public Animal Animal { get { return mAnimal; } set { mAnimal = value; } }
        public PlanSanitario Plan { get { return mPlan; } set { mPlan = value; } }

        public Descorne(int pIdDescorne, DateTime pFecha, string pMetodo, string pObservaciones,
            Animal pAnimal, PlanSanitario pPlan)
        {
            mIdDescorne = pIdDescorne;
            mFecha = pFecha;
            mMetodo = pMetodo;
            mObservaciones = pObservaciones;
            mAnimal = pAnimal;
            mPlan = pPlan;
        }
    }
}
