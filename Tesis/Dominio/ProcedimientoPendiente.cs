namespace Tesis.Dominio
{
    // Una fila del calendario sanitario (CU31): el procedimiento que un plan le exige
    // a un animal y todavia no se registro.
    //
    // No tiene tabla ni figura en el Modelo Entidad-Relacion, y no debe tenerla: el
    // pendiente es la diferencia entre lo que el plan exige y lo que se aplico, o sea
    // un dato derivado que se calcula en el momento de la consulta. La clase existe
    // solo para que la Controladora devuelva ese calculo armado y la pantalla no
    // tenga que rehacerlo.
    public class ProcedimientoPendiente
    {
        private Animal mAnimal;
        private PlanSanitario mPlan;
        private DateTime mUltimaAplicacion;
        private DateTime mProximaAplicacion;

        public Animal Animal { get { return mAnimal; } set { mAnimal = value; } }
        public PlanSanitario Plan { get { return mPlan; } set { mPlan = value; } }

        // Fecha de la ultima aplicacion registrada del plan sobre el animal.
        // DateTime.MinValue cuando nunca se le aplico.
        public DateTime UltimaAplicacion { get { return mUltimaAplicacion; } set { mUltimaAplicacion = value; } }
        public DateTime ProximaAplicacion { get { return mProximaAplicacion; } set { mProximaAplicacion = value; } }

        public ProcedimientoPendiente(Animal pAnimal, PlanSanitario pPlan,
            DateTime pUltimaAplicacion, DateTime pProximaAplicacion)
        {
            mAnimal = pAnimal;
            mPlan = pPlan;
            mUltimaAplicacion = pUltimaAplicacion;
            mProximaAplicacion = pProximaAplicacion;
        }
    }
}
