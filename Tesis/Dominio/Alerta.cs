namespace Tesis.Dominio
{
    // Un pendiente concreto de un dia concreto: la vaca 136 con parto probable el 27,
    // la partida de aftosa que vence en seis dias. Es el renglon del mensaje y, una vez
    // guardada, el registro de que ese aviso salio.
    //
    // No es una fuente de informacion: el pendiente se calcula donde siempre -en las
    // listas de trabajo de la Controladora- y la alerta es la foto de ese calculo en el
    // momento en que se envio. Por eso guarda el texto ya armado: el historial no puede
    // depender de que el calculo siga dando lo mismo dentro de seis meses.
    //
    // Animal e Insumo son excluyentes: unos avisos nacen de un animal y otros de un
    // insumo, y el que no corresponde queda nulo.
    public class Alerta
    {
        private int mIdAlerta;
        private string mTipoAlerta;
        private DateTime mFechaGeneracion;
        private string mMensaje;
        private bool mEnviada;
        private int mIdPreferencia;
        private Animal mAnimal;
        private Insumo mInsumo;

        public int IdAlerta { get { return mIdAlerta; } set { mIdAlerta = value; } }
        public string TipoAlerta { get { return mTipoAlerta; } set { mTipoAlerta = value; } }
        public DateTime FechaGeneracion { get { return mFechaGeneracion; } set { mFechaGeneracion = value; } }
        public string Mensaje { get { return mMensaje; } set { mMensaje = value; } }
        public bool Enviada { get { return mEnviada; } set { mEnviada = value; } }
        public int IdPreferencia { get { return mIdPreferencia; } set { mIdPreferencia = value; } }
        public Animal Animal { get { return mAnimal; } set { mAnimal = value; } }
        public Insumo Insumo { get { return mInsumo; } set { mInsumo = value; } }

        public Alerta(int pIdAlerta, string pTipoAlerta, DateTime pFechaGeneracion, string pMensaje,
            bool pEnviada, int pIdPreferencia, Animal pAnimal, Insumo pInsumo)
        {
            mIdAlerta = pIdAlerta;
            mTipoAlerta = pTipoAlerta;
            mFechaGeneracion = pFechaGeneracion;
            mMensaje = pMensaje;
            mEnviada = pEnviada;
            mIdPreferencia = pIdPreferencia;
            mAnimal = pAnimal;
            mInsumo = pInsumo;
        }
    }
}
