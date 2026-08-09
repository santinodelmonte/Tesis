namespace Tesis.Dominio
{
    // Lo que dio una vaca en una jornada de control: la suma de los turnos de esa
    // fecha. No tiene tabla ni la necesita, porque sale de agrupar los ordenies
    // individuales que ya estan guardados.
    //
    // Existe porque el control lechero se hace una vez por mes y los calculos de
    // produccion trabajan sobre el dia completo, no sobre cada turno por separado: una
    // vaca controlada en los dos turnos tiene un solo dato de produccion diaria.
    public class ControlDiario
    {
        private DateTime mFecha;
        private double mLitros;

        public DateTime Fecha { get { return mFecha; } set { mFecha = value; } }
        public double Litros { get { return mLitros; } set { mLitros = value; } }

        public ControlDiario(DateTime pFecha, double pLitros)
        {
            mFecha = pFecha;
            mLitros = pLitros;
        }
    }
}
