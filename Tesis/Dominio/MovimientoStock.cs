namespace Tesis.Dominio
{
    // Adelantada del Modulo 5. Cada alta de stock y cada consumo dejan su
    // movimiento, para que el descuento de una pajuela en CU15 quede trazado y no
    // sea solo un numero que baja en insumos.
    public class MovimientoStock
    {
        public const string INGRESO = "Ingreso";
        public const string EGRESO = "Egreso";

        private int mIdMovimiento;
        private string mTipoMovimiento;
        private double mCantidad;
        private DateTime mFecha;
        private DateTime mFechaVencimiento;
        private string mMotivo;
        private Insumo mInsumo;

        public int IdMovimiento { get { return mIdMovimiento; } set { mIdMovimiento = value; } }
        public string TipoMovimiento { get { return mTipoMovimiento; } set { mTipoMovimiento = value; } }
        public double Cantidad { get { return mCantidad; } set { mCantidad = value; } }
        public DateTime Fecha { get { return mFecha; } set { mFecha = value; } }
        public DateTime FechaVencimiento { get { return mFechaVencimiento; } set { mFechaVencimiento = value; } }
        public string Motivo { get { return mMotivo; } set { mMotivo = value; } }
        public Insumo Insumo { get { return mInsumo; } set { mInsumo = value; } }

        public MovimientoStock(int pIdMovimiento, string pTipoMovimiento, double pCantidad,
            DateTime pFecha, DateTime pFechaVencimiento, string pMotivo, Insumo pInsumo)
        {
            mIdMovimiento = pIdMovimiento;
            mTipoMovimiento = pTipoMovimiento;
            mCantidad = pCantidad;
            mFecha = pFecha;
            mFechaVencimiento = pFechaVencimiento;
            mMotivo = pMotivo;
            mInsumo = pInsumo;
        }
    }
}
