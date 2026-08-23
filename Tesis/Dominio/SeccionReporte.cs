namespace Tesis.Dominio
{
    // Una tabla dentro de un reporte: su titulo, los nombres de las columnas y las
    // filas ya convertidas a texto.
    //
    // Las filas van como texto y no como objetos a proposito. El reporte es una foto:
    // una vez armado no tiene que volver a consultarle nada al dominio, y quien lo
    // vuelca a PDF o a Excel no tiene por que saber que un litro se muestra con dos
    // decimales y una fecha en dia/mes/anio. Ese formato se decide una sola vez, donde
    // se arma el contenido.
    //
    // ColumnasNumericas marca cuales se alinean a la derecha y cuales, en Excel, se
    // escriben como numero y no como texto, para que la planilla se pueda sumar y
    // ordenar. Es lo unico de presentacion que viaja aca, y viaja porque el que arma
    // el contenido es el unico que sabe cual columna es un numero.
    public class SeccionReporte
    {
        private string mTitulo;
        private string[] mColumnas;
        private List<string[]> mFilas;
        private List<int> mColumnasNumericas;
        private string mNota;

        public string Titulo { get { return mTitulo; } set { mTitulo = value; } }
        public string[] Columnas { get { return mColumnas; } set { mColumnas = value; } }
        public List<string[]> Filas { get { return mFilas; } set { mFilas = value; } }

        // Indices de las columnas que llevan numeros.
        public List<int> ColumnasNumericas { get { return mColumnasNumericas; } set { mColumnasNumericas = value; } }

        // Aclaracion al pie de la seccion. Sirve para lo que un total no explica por si
        // solo: por ejemplo, que la produccion de un turno anotado vaca por vaca es la
        // suma de lo medido y no la lectura del tanque.
        public string Nota { get { return mNota; } set { mNota = value; } }

        public SeccionReporte(string pTitulo, string[] pColumnas)
        {
            mTitulo = pTitulo;
            mColumnas = pColumnas;
            mFilas = new List<string[]>();
            mColumnasNumericas = new List<int>();
            mNota = "";
        }

        public void AgregarFila(params string[] pValores)
        {
            mFilas.Add(pValores);
        }

        public bool EsNumerica(int pIndice)
        {
            return mColumnasNumericas.Contains(pIndice);
        }
    }
}
