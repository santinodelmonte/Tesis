namespace Tesis.Dominio
{
    // Un reporte armado y listo para volcar a PDF o a Excel (CU44 a CU47).
    //
    // No tiene tabla ni figura en el Modelo Entidad-Relacion: es un dato derivado, la
    // foto de lo que las listas del sistema dicen en el momento de pedirlo.
    //
    // Lo que decide el contenido -que secciones, que columnas, que filas y en que
    // orden- es la Controladora, porque es una regla de negocio: un reporte sanitario
    // lleva diagnosticos, tratamientos y vacunaciones y no cualquier cosa. Lo que
    // decide como se ve son los generadores de Reportes/, que reciben esto ya resuelto
    // y no vuelven a preguntarle nada al dominio.
    //
    // Esa separacion es la que evita el problema clasico de los reportes: que la
    // version PDF y la version Excel del mismo reporte terminen mostrando numeros
    // distintos porque cada una los calculo por su cuenta. Aca los calcula uno solo.
    public class Reporte
    {
        private string mTitulo;
        private string mPeriodo;
        private DateTime mFechaEmision;
        private List<SeccionReporte> mSecciones;

        public string Titulo { get { return mTitulo; } set { mTitulo = value; } }

        // Que abarca el reporte, en texto: un rango de fechas, un mes, o el rodeo
        // entero cuando no depende del tiempo.
        public string Periodo { get { return mPeriodo; } set { mPeriodo = value; } }

        public DateTime FechaEmision { get { return mFechaEmision; } set { mFechaEmision = value; } }

        public List<SeccionReporte> Secciones { get { return mSecciones; } set { mSecciones = value; } }

        public Reporte(string pTitulo, string pPeriodo)
        {
            mTitulo = pTitulo;
            mPeriodo = pPeriodo;
            mFechaEmision = DateTime.Now;
            mSecciones = new List<SeccionReporte>();
        }

        // Agrega una seccion y la devuelve, para que la Controladora la vaya llenando
        // sin tener que declarar una variable por cada una.
        public SeccionReporte AgregarSeccion(string pTitulo, string[] pColumnas)
        {
            SeccionReporte unaSeccion = new SeccionReporte(pTitulo, pColumnas);
            mSecciones.Add(unaSeccion);
            return unaSeccion;
        }
    }
}
