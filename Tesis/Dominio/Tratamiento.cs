namespace Tesis.Dominio
{
    // Tratamiento sanitario aplicado a un animal (CU20). fecha_fin_descarte es lo que
    // mira el Modulo 2: mientras no venza, la hembra tratada queda fuera del lote de
    // ordenie (CU8, paso 3).
    //
    // mAnimal no figura en el Diccionario de Clases. Se agrega porque el diagnostico
    // -el unico vinculo que el modelo preveia- es opcional: el tratamiento preventivo
    // va sin diagnostico y sin este campo no se puede atribuir a ningun animal, con lo
    // que no genera descarte de leche ni cumple ningun plan sanitario.
    //
    // mCantidadInsumo tampoco figura, y se agrega por lo mismo que la anterior: la
    // cantidad aplicada quedaba solo en el movimiento de stock, que no dice de que
    // tratamiento salio. Sin ella, corregir o eliminar un tratamiento mal cargado no
    // puede devolverle al inventario lo que nunca se uso.
    public class Tratamiento
    {
        private int mIdTratamiento;
        private DateTime mFechaInicio;
        private int mDiasDuracion;
        private string mDosisDiaria;
        private double mCantidadInsumo;
        private DateTime mFechaFinDescarte;
        private Diagnostico mDiagnostico;
        private Animal mAnimal;
        private Insumo mInsumo;
        private PlanSanitario mPlan;

        public int IdTratamiento { get { return mIdTratamiento; } set { mIdTratamiento = value; } }
        public DateTime FechaInicio { get { return mFechaInicio; } set { mFechaInicio = value; } }
        public int DiasDuracion { get { return mDiasDuracion; } set { mDiasDuracion = value; } }
        public string DosisDiaria { get { return mDosisDiaria; } set { mDosisDiaria = value; } }
        public double CantidadInsumo { get { return mCantidadInsumo; } set { mCantidadInsumo = value; } }
        public DateTime FechaFinDescarte { get { return mFechaFinDescarte; } set { mFechaFinDescarte = value; } }
        public Diagnostico Diagnostico { get { return mDiagnostico; } set { mDiagnostico = value; } }
        public Animal Animal { get { return mAnimal; } set { mAnimal = value; } }
        public Insumo Insumo { get { return mInsumo; } set { mInsumo = value; } }
        public PlanSanitario Plan { get { return mPlan; } set { mPlan = value; } }

        public Tratamiento(int pIdTratamiento, DateTime pFechaInicio, int pDiasDuracion,
            string pDosisDiaria, double pCantidadInsumo, DateTime pFechaFinDescarte,
            Diagnostico pDiagnostico, Animal pAnimal, Insumo pInsumo, PlanSanitario pPlan)
        {
            mIdTratamiento = pIdTratamiento;
            mFechaInicio = pFechaInicio;
            mDiasDuracion = pDiasDuracion;
            mDosisDiaria = pDosisDiaria;
            mCantidadInsumo = pCantidadInsumo;
            mFechaFinDescarte = pFechaFinDescarte;
            mDiagnostico = pDiagnostico;
            mAnimal = pAnimal;
            mInsumo = pInsumo;
            mPlan = pPlan;
        }
    }
}
