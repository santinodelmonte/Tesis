using MySql.Data.MySqlClient;
using System.Data;

namespace Tesis.Persistencia
{
    public class pConexion
    {
        // ---------------------------------------------------------------------
        // DATOS DE CONEXION A MySQL - completar con los datos del equipo
        // ---------------------------------------------------------------------
        private static string servidor = "localhost";
        private static string puerto = "3306";
        private static string baseDeDatos = "tambo";
        private static string usuario = "root";
        private static string contrasena = "";
        // ---------------------------------------------------------------------

        private string mCadenaConexion = "server=" + servidor + "; port=" + puerto + "; " +
       "database=" + baseDeDatos + "; uid=" + usuario + "; pwd=" + contrasena + "; CharSet=utf8mb4;";
        private MySqlConnection mConexion;

        public bool Abrir()
        {
            mConexion = new MySqlConnection(this.mCadenaConexion);
            mConexion.Open();
            return true;
        }

        public bool Cerrar()
        {
            mConexion.Close();
            return true;
        }

        public bool EjecutarComando(string pSql)
        {
            try
            {
                this.Abrir();
                MySqlCommand comando = new MySqlCommand(pSql, mConexion);
                comando.ExecuteNonQuery();
                comando.Dispose();
                this.Cerrar();
                return true;
            }
            catch (Exception e)
            {
                throw new Exception("Error en conexion sql = " + pSql, e);
            }
        }

        public DataTable EjecutarConsulta(string pSql)
        {
            try
            {
                this.Abrir();
                MySqlDataAdapter adaptador = new MySqlDataAdapter(pSql, mConexion);
                DataTable resultado = new DataTable();
                adaptador.Fill(resultado);
                adaptador.Dispose();
                this.Cerrar();
                return resultado;
            }
            catch (Exception e)
            {
                throw new Exception("Error en conexion sql = " + pSql, e);
            }
        }
    }
}
