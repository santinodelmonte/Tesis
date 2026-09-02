namespace Tesis.Dominio
{
    // Un tipo de aviso y el interruptor que decide si entra en el resumen diario.
    //
    // Los ocho tipos son los ocho contadores del tablero de inicio, y la lista es
    // cerrada a proposito: la regla de negocio de CU49 dice que el resumen, el tablero
    // y el calendario sanitario no pueden discrepar, y la unica forma de garantizarlo
    // es que el aviso no tenga una fuente propia de informacion.
    //
    // Apagar un tipo no cambia lo que el sistema muestra en pantalla: saca ese bloque
    // del mensaje y nada mas.
    public class PreferenciaNotificacion
    {
        // Los ocho tipos. Las cadenas son las mismas que carga bd/CreacionDb.sql en
        // preferencias_notificacion, sin tildes porque son la clave con la que se
        // busca la fila y no el texto que ve el usuario -ese esta en Etiqueta-.
        public const string SANITARIO_PENDIENTE = "Sanitario pendiente";
        public const string PARTO_PROXIMO = "Parto proximo";
        public const string TACTO_PENDIENTE = "Tacto pendiente";
        public const string VACA_PARA_SERVIR = "Vaca para servir";
        public const string SECADO_PROXIMO = "Secado proximo";
        public const string FIN_DESCARTE = "Fin de descarte";
        public const string STOCK_CRITICO = "Stock critico";
        public const string VENCIMIENTO_INSUMO = "Vencimiento de insumo";

        private int mIdPreferencia;
        private string mTipoAlerta;
        private bool mActiva;

        public int IdPreferencia { get { return mIdPreferencia; } set { mIdPreferencia = value; } }
        public string TipoAlerta { get { return mTipoAlerta; } set { mTipoAlerta = value; } }
        public bool Activa { get { return mActiva; } set { mActiva = value; } }

        public PreferenciaNotificacion(int pIdPreferencia, string pTipoAlerta, bool pActiva)
        {
            mIdPreferencia = pIdPreferencia;
            mTipoAlerta = pTipoAlerta;
            mActiva = pActiva;
        }

        // El titulo con el que el tipo de aviso aparece en la pantalla y encabeza su
        // bloque dentro del mensaje. Con tilde, en plural y con el nombre que la
        // encargada usa: el que esta guardado en la base es un identificador.
        public string Etiqueta
        {
            get
            {
                switch (mTipoAlerta)
                {
                    case SANITARIO_PENDIENTE: return "Procedimientos sanitarios pendientes";
                    case PARTO_PROXIMO: return "Partos próximos";
                    case TACTO_PENDIENTE: return "Tactos pendientes";
                    case VACA_PARA_SERVIR: return "Vacas para servir";
                    case SECADO_PROXIMO: return "Secados próximos";
                    case FIN_DESCARTE: return "Fin del descarte de leche";
                    case STOCK_CRITICO: return "Stock crítico";
                    case VENCIMIENTO_INSUMO: return "Partidas por vencer";
                    default: return mTipoAlerta;
                }
            }
        }

        // El modulo del que sale el aviso. Agrupa la pantalla de configuracion y ordena
        // el mensaje, que CU49 pide armado por modulo.
        public string Modulo
        {
            get
            {
                switch (mTipoAlerta)
                {
                    case PARTO_PROXIMO:
                    case TACTO_PENDIENTE:
                    case VACA_PARA_SERVIR: return "Reproducción";
                    case SECADO_PROXIMO:
                    case FIN_DESCARTE: return "Producción";
                    case SANITARIO_PENDIENTE: return "Sanidad";
                    default: return "Insumos";
                }
            }
        }

        // El orden en que los ocho tipos se muestran y se envian: agrupados por modulo
        // y en el mismo orden que el tablero de inicio.
        public static List<string> Tipos()
        {
            return new List<string>
            {
                SANITARIO_PENDIENTE,
                PARTO_PROXIMO,
                TACTO_PENDIENTE,
                VACA_PARA_SERVIR,
                SECADO_PROXIMO,
                FIN_DESCARTE,
                STOCK_CRITICO,
                VENCIMIENTO_INSUMO
            };
        }
    }
}
