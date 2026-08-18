namespace Tesis.Dominio
{
    // Una fila de la alerta de vencimiento (CU38): la partida ingresada de un insumo
    // con lo que queda de ella.
    //
    // Igual que ProcedimientoPendiente, es un dato derivado y no se almacena. La
    // partida es el movimiento de ingreso -ahi vive la fecha de vencimiento, porque un
    // mismo insumo entra en partidas distintas- y el remanente lo calcula la
    // Controladora imputando los egresos del insumo contra las partidas.
    public class PartidaVencimiento
    {
        private MovimientoStock mPartida;
        private double mRemanente;

        public MovimientoStock Partida { get { return mPartida; } set { mPartida = value; } }
        public double Remanente { get { return mRemanente; } set { mRemanente = value; } }

        public PartidaVencimiento(MovimientoStock pPartida, double pRemanente)
        {
            mPartida = pPartida;
            mRemanente = pRemanente;
        }
    }
}
