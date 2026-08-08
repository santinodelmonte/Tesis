namespace Tesis.Dominio
{
    // Aplicacion de una vacuna a un animal (CU21). El plan es opcional: en nulo
    // queda la vacunacion registrada fuera de todo plan.
    public class Vacunacion
    {
        private int mIdVacunacion;
        private DateTime mFechaAplicacion;
        private Animal mAnimal;
        private Insumo mInsumo;
        private PlanSanitario mPlan;

        public int IdVacunacion { get { return mIdVacunacion; } set { mIdVacunacion = value; } }
        public DateTime FechaAplicacion { get { return mFechaAplicacion; } set { mFechaAplicacion = value; } }
        public Animal Animal { get { return mAnimal; } set { mAnimal = value; } }
        public Insumo Insumo { get { return mInsumo; } set { mInsumo = value; } }
        public PlanSanitario Plan { get { return mPlan; } set { mPlan = value; } }

        public Vacunacion(int pIdVacunacion, DateTime pFechaAplicacion, Animal pAnimal,
            Insumo pInsumo, PlanSanitario pPlan)
        {
            mIdVacunacion = pIdVacunacion;
            mFechaAplicacion = pFechaAplicacion;
            mAnimal = pAnimal;
            mInsumo = pInsumo;
            mPlan = pPlan;
        }
    }
}
