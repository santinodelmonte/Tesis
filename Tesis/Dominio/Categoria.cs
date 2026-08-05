namespace Tesis.Dominio
{
    public class Categoria
    {
        private int mIdCategoria;
        private string mNombre;
        private string mDescripcion;

        public int IdCategoria { get { return mIdCategoria; } set { mIdCategoria = value; } }
        public string Nombre { get { return mNombre; } set { mNombre = value; } }
        public string Descripcion { get { return mDescripcion; } set { mDescripcion = value; } }

        public Categoria(int pIdCategoria, string pNombre, string pDescripcion)
        {
            mIdCategoria = pIdCategoria;
            mNombre = pNombre;
            mDescripcion = pDescripcion;
        }
    }
}
