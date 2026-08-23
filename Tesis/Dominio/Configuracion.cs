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
        private int mDiasEsperaVoluntaria;
        private int mDiasParaTacto;
        private TimeSpan mHoraResumen;
        private string mChatTelegram = "";
        private DateTime mFechaUltimoResumen;

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
        // pantallas de CU12 y CU13.
        public int OrdeniesPorDia { get { return mOrdeniesPorDia; } set { mOrdeniesPorDia = value; } }

        public int DiasAnticipacionSecado { get { return mDiasAnticipacionSecado; } set { mDiasAnticipacionSecado = value; } }
        public int DiasAnticipacionParto { get { return mDiasAnticipacionParto; } set { mDiasAnticipacionParto = value; } }
        public int DiasAnticipacionSanitaria { get { return mDiasAnticipacionSanitaria; } set { mDiasAnticipacionSanitaria = value; } }
        public int DiasAnticipacionVencimiento { get { return mDiasAnticipacionVencimiento; } set { mDiasAnticipacionVencimiento = value; } }

        // Dias posparto que el establecimiento deja pasar antes de volver a servir a la
        // vaca. Es el periodo de espera voluntario: antes de eso el utero todavia se
        // esta recuperando y el servicio rinde poco.
        public int DiasEsperaVoluntaria { get { return mDiasEsperaVoluntaria; } set { mDiasEsperaVoluntaria = value; } }

        // Dias despues del servicio a partir de los cuales el tacto ya se puede hacer.
        // De aca sale la lista con la que se arma la visita del veterinario.
        public int DiasParaTacto { get { return mDiasParaTacto; } set { mDiasParaTacto = value; } }

        // Hora a la que sale el resumen diario de Telegram (RF7.7). Es un parametro de
        // manejo como los demas: hay tambos que quieren el mensaje antes del ordenie de
        // la maniana y otros la noche anterior.
        public TimeSpan HoraResumen { get { return mHoraResumen; } set { mHoraResumen = value; } }

        // El chat de Telegram al que se le manda. Vacio mientras nadie haya vinculado
        // una cuenta, que es como el sistema sabe que la integracion no esta lista.
        //
        // Es uno solo para todo el establecimiento: el sistema tiene una unica usuaria.
        // El token del bot no esta aca ni en la base -es una credencial- y lo carga
        // Program.cs desde la configuracion de la aplicacion.
        public string ChatTelegram { get { return mChatTelegram; } set { mChatTelegram = value ?? ""; } }

        // El dia en que salio el ultimo resumen. Es lo unico de esta clase que el
        // sistema escribe solo: lo demas lo decide la encargada.
        //
        // Sirve para no mandar dos veces el mismo resumen si el sitio se reinicia. La
        // tabla de alertas no alcanza para saberlo, porque el dia sin pendientes manda
        // mensaje igual y no genera ninguna alerta.
        public DateTime FechaUltimoResumen { get { return mFechaUltimoResumen; } set { mFechaUltimoResumen = value; } }

        // La integracion esta lista cuando hay a quien escribirle.
        public bool TelegramVinculado { get { return mChatTelegram != ""; } }

        public Configuracion(int pIdConfiguracion, int pDiasSecadoAntesParto,
            int pEdadMinimaServicioMeses, int pEdadCambioCategoriaMeses,
            double pLitrosMaximosIndividual, int pOrdeniesPorDia, int pDiasAnticipacionSecado,
            int pDiasAnticipacionParto, int pDiasAnticipacionSanitaria, int pDiasAnticipacionVencimiento,
            int pDiasEsperaVoluntaria, int pDiasParaTacto, TimeSpan pHoraResumen,
            string pChatTelegram, DateTime pFechaUltimoResumen)
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
            mDiasEsperaVoluntaria = pDiasEsperaVoluntaria;
            mDiasParaTacto = pDiasParaTacto;
            mHoraResumen = pHoraResumen;
            mChatTelegram = pChatTelegram ?? "";
            mFechaUltimoResumen = pFechaUltimoResumen;
        }
    }
}
