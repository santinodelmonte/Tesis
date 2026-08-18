namespace Tesis.Dominio
{
    // Adelantada del Modulo 5. Se incorpora ahora porque CU21 registra la
    // inseminacion artificial con una pajuela del stock y le descuenta una unidad,
    // y CU28 necesita el periodo de carencia para calcular el descarte de leche.
    public class Insumo
    {
        // Tipos de insumo admitidos.
        public const string MEDICAMENTO = "Medicamento";
        public const string VACUNA = "Vacuna";
        public const string ANTIPARASITARIO = "Antiparasitario";
        public const string PAJUELA = "Pajuela";

        private int mIdInsumo;
        private string mNombre;
        private string mTipoInsumo;
        private double mStockActual;
        private double mStockMinimo;
        private int mPeriodoDescarteDias;
        private Macho mToro;

        public int IdInsumo { get { return mIdInsumo; } set { mIdInsumo = value; } }
        public string Nombre { get { return mNombre; } set { mNombre = value; } }
        public string TipoInsumo { get { return mTipoInsumo; } set { mTipoInsumo = value; } }
        public double StockActual { get { return mStockActual; } set { mStockActual = value; } }
        public double StockMinimo { get { return mStockMinimo; } set { mStockMinimo = value; } }
        public int PeriodoDescarteDias { get { return mPeriodoDescarteDias; } set { mPeriodoDescarteDias = value; } }
        public Macho Toro { get { return mToro; } set { mToro = value; } }

        public Insumo(int pIdInsumo, string pNombre, string pTipoInsumo, double pStockActual,
            double pStockMinimo, int pPeriodoDescarteDias, Macho pToro)
        {
            mIdInsumo = pIdInsumo;
            mNombre = pNombre;
            mTipoInsumo = pTipoInsumo;
            mStockActual = pStockActual;
            mStockMinimo = pStockMinimo;
            mPeriodoDescarteDias = pPeriodoDescarteDias;
            mToro = pToro;
        }
    }
}
