namespace Tesis.Dominio
{
    public class Raza
    {
        private int mIdRaza;
        private string mNombre;
        private string mDescripcion;

        public int IdRaza { get { return mIdRaza; } set { mIdRaza = value; } }
        public string Nombre { get { return mNombre; } set { mNombre = value; } }
        public string Descripcion { get { return mDescripcion; } set { mDescripcion = value; } }

        public Raza(int pIdRaza, string pNombre, string pDescripcion)
        {
            mIdRaza = pIdRaza;
            mNombre = pNombre;
            mDescripcion = pDescripcion;
        }
    }
}
