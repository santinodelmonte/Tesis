namespace Tesis.Dominio
{
    // La regla de un procedimiento sanitario periodico (CU30). No guarda
    // aplicaciones: el calendario de CU31 sale de comparar lo que el plan exige
    // contra lo que efectivamente se registro.
    public class PlanSanitario
    {
        // Tipos de procedimiento admitidos. Determinan donde busca el sistema la
        // ultima aplicacion del plan sobre un animal: la vacunacion en vacunaciones,
        // la desparasitacion en tratamientos y el descorne en descornes.
        public const string VACUNACION = "Vacunacion";
        public const string DESPARASITACION = "Desparasitacion";
        public const string DESCORNE = "Descorne";

        // Un plan sin periodicidad se aplica una unica vez en la vida del animal.
        // Se representa con cero, que es el valor que la persistencia traduce al
        // nulo de la columna periodicidad_dias.
        public const int SIN_PERIODICIDAD = 0;

        private int mIdPlan;
        private string mNombre;
        private string mTipoProcedimiento;
        private int mPeriodicidadDias;
        private int mEdadInicioMeses;
        private bool mActivo;
        private Insumo mInsumo;

        // Categorias alcanzadas por el plan. La lista vacia significa que alcanza a
        // todo el rodeo (CU30, curso alternativo 4a). A diferencia del resto de las
        // colecciones del Diccionario de Clases, esta si se materializa: el vinculo
        // no es circular y el calendario la necesita en cada iteracion.
        private List<Categoria> mCategorias;

        public int IdPlan { get { return mIdPlan; } set { mIdPlan = value; } }
        public string Nombre { get { return mNombre; } set { mNombre = value; } }
        public string TipoProcedimiento { get { return mTipoProcedimiento; } set { mTipoProcedimiento = value; } }
        public int PeriodicidadDias { get { return mPeriodicidadDias; } set { mPeriodicidadDias = value; } }
        public int EdadInicioMeses { get { return mEdadInicioMeses; } set { mEdadInicioMeses = value; } }
        public bool Activo { get { return mActivo; } set { mActivo = value; } }
        public Insumo Insumo { get { return mInsumo; } set { mInsumo = value; } }
        public List<Categoria> Categorias { get { return mCategorias; } set { mCategorias = value; } }

        public PlanSanitario(int pIdPlan, string pNombre, string pTipoProcedimiento,
            int pPeriodicidadDias, int pEdadInicioMeses, bool pActivo, Insumo pInsumo,
            List<Categoria> pCategorias)
        {
            mIdPlan = pIdPlan;
            mNombre = pNombre;
            mTipoProcedimiento = pTipoProcedimiento;
            mPeriodicidadDias = pPeriodicidadDias;
            mEdadInicioMeses = pEdadInicioMeses;
            mActivo = pActivo;
            mInsumo = pInsumo;
            mCategorias = pCategorias;
        }
    }
}
