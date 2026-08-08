namespace Tesis.Dominio
{
    // Parametros de manejo del establecimiento. Son las reglas que cada tambo resuelve
    // a su manera -cuantos dias antes del parto seca, a que edad sirve la vaquillona,
    // con cuanta anticipacion quiere que le avisen- y que hasta ahora estaban fijas en
    // el codigo.
    //
    // No entra aca lo que no es una decision: la duracion de la gestacion, la edad de
    // pubertad y el rango de una preniez viable son biologia y siguen siendo
    // constantes. Tampoco el periodo de carencia ni el stock minimo, que son datos del
    // producto y viven en Insumo.
    public class Configuracion
    {
        private int mIdConfiguracion;
        private int mDiasSecadoAntesParto;
        private int mEdadMinimaServicioMeses;
        private int mEdadCambioCategoriaMeses;
        private double mLitrosMaximosIndividual;
        private int mOrdeniesPorDia;
        private int mDiasAnticipacionSecado;
        private int mDiasAnticipacionParto;
        private int mDiasAnticipacionSanitaria;
        private int mDiasAnticipacionVencimiento;

        public int IdConfiguracion { get { return mIdConfiguracion; } set { mIdConfiguracion = value; } }

        // Dias antes de la fecha probable de parto en que se recomienda secar (RF2.10)
        public int DiasSecadoAntesParto { get { return mDiasSecadoAntesParto; } set { mDiasSecadoAntesParto = value; } }

        // Edad a la que la vaquillona queda habilitada para entrar en servicio
        public int EdadMinimaServicioMeses { get { return mEdadMinimaServicioMeses; } set { mEdadMinimaServicioMeses = value; } }

        // Edad a la que la cria deja de ser ternera o ternero (RF1.9)
        public int EdadCambioCategoriaMeses { get { return mEdadCambioCategoriaMeses; } set { mEdadCambioCategoriaMeses = value; } }

        // Tope de coherencia de un control individual, en litros por turno
        public double LitrosMaximosIndividual { get { return mLitrosMaximosIndividual; } set { mLitrosMaximosIndividual = value; } }

        // Cantidad de orden;es diarios. De aca salen los turnos que ofrecen las
        // pantallas de CU8 y CU9.
        public int OrdeniesPorDia { get { return mOrdeniesPorDia; } set { mOrdeniesPorDia = value; } }

        public int DiasAnticipacionSecado { get { return mDiasAnticipacionSecado; } set { mDiasAnticipacionSecado = value; } }
        public int DiasAnticipacionParto { get { return mDiasAnticipacionParto; } set { mDiasAnticipacionParto = value; } }
        public int DiasAnticipacionSanitaria { get { return mDiasAnticipacionSanitaria; } set { mDiasAnticipacionSanitaria = value; } }
        public int DiasAnticipacionVencimiento { get { return mDiasAnticipacionVencimiento; } set { mDiasAnticipacionVencimiento = value; } }

        public Configuracion(int pIdConfiguracion, int pDiasSecadoAntesParto,
            int pEdadMinimaServicioMeses, int pEdadCambioCategoriaMeses,
            double pLitrosMaximosIndividual, int pOrdeniesPorDia, int pDiasAnticipacionSecado,
            int pDiasAnticipacionParto, int pDiasAnticipacionSanitaria, int pDiasAnticipacionVencimiento)
        {
            mIdConfiguracion = pIdConfiguracion;
            mDiasSecadoAntesParto = pDiasSecadoAntesParto;
            mEdadMinimaServicioMeses = pEdadMinimaServicioMeses;
            mEdadCambioCategoriaMeses = pEdadCambioCategoriaMeses;
            mLitrosMaximosIndividual = pLitrosMaximosIndividual;
            mOrdeniesPorDia = pOrdeniesPorDia;
            mDiasAnticipacionSecado = pDiasAnticipacionSecado;
            mDiasAnticipacionParto = pDiasAnticipacionParto;
            mDiasAnticipacionSanitaria = pDiasAnticipacionSanitaria;
            mDiasAnticipacionVencimiento = pDiasAnticipacionVencimiento;
        }
    }
}
