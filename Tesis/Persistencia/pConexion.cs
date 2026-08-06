using MySql.Data.MySqlClient;
using System.Data;

namespace Tesis.Persistencia
{
    public class pConexion
    {
        // ---------------------------------------------------------------------
        // La cadena de conexion no se escribe aca: se carga desde appsettings.json
        // al iniciar la aplicacion (ver Program.cs). De esa forma los datos del
        // servidor y la contrasena de MySQL no quedan versionados en el repositorio.
        // ---------------------------------------------------------------------
        private static string mCadenaConexion = "";

        public static void Configurar(string pCadenaConexion)
        {
            mCadenaConexion = pCadenaConexion;
        }
        // ---------------------------------------------------------------------

        public bool EjecutarComando(string pSql)
        {
            return this.EjecutarComando(pSql, null);
        }

        // Las consultas se arman con parametros y no concatenando texto: asi un
        // numero de caravana o un motivo de baja con apostrofo no rompe el comando
        // ni permite inyectar SQL.
        public bool EjecutarComando(string pSql, Dictionary<string, object?>? pParametros)
        {
            try
            {
                // El using cierra la conexion y el comando aunque el comando falle
                using (MySqlConnection conexion = new MySqlConnection(mCadenaConexion))
                {
                    conexion.Open();
                    using (MySqlCommand comando = new MySqlCommand(pSql, conexion))
                    {
                        this.CargarParametros(comando, pParametros);
                        comando.ExecuteNonQuery();
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                throw new Exception("Error en conexion sql = " + pSql, e);
            }
        }

        public DataTable EjecutarConsulta(string pSql)
        {
            return this.EjecutarConsulta(pSql, null);
        }

        public DataTable EjecutarConsulta(string pSql, Dictionary<string, object?>? pParametros)
        {
            try
            {
                using (MySqlConnection conexion = new MySqlConnection(mCadenaConexion))
                {
                    conexion.Open();
                    using (MySqlCommand comando = new MySqlCommand(pSql, conexion))
                    {
                        this.CargarParametros(comando, pParametros);
                        using (MySqlDataAdapter adaptador = new MySqlDataAdapter(comando))
                        {
                            DataTable resultado = new DataTable();
                            adaptador.Fill(resultado);
                            return resultado;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error en conexion sql = " + pSql, e);
            }
        }

        // Devuelve una conexion ya abierta. La usa el alta de animal, que necesita
        // escribir en dos tablas dentro de una misma transaccion. El que la pide se
        // encarga de cerrarla (con using).
        public MySqlConnection AbrirConexion()
        {
            MySqlConnection conexion = new MySqlConnection(mCadenaConexion);
            conexion.Open();
            return conexion;
        }

        // Ejecuta un comando sobre una conexion y una transaccion que ya estan abiertas
        public int EjecutarInsercionEnTransaccion(string pSql, Dictionary<string, object?> pParametros,
            MySqlConnection pConexion, MySqlTransaction pTransaccion)
        {
            using (MySqlCommand comando = new MySqlCommand(pSql, pConexion, pTransaccion))
            {
                this.CargarParametros(comando, pParametros);
                comando.ExecuteNonQuery();
                return (int)comando.LastInsertedId;
            }
        }

        public void EjecutarComandoEnTransaccion(string pSql, Dictionary<string, object?> pParametros,
            MySqlConnection pConexion, MySqlTransaction pTransaccion)
        {
            using (MySqlCommand comando = new MySqlCommand(pSql, pConexion, pTransaccion))
            {
                this.CargarParametros(comando, pParametros);
                comando.ExecuteNonQuery();
            }
        }

        private void CargarParametros(MySqlCommand pComando, Dictionary<string, object?>? pParametros)
        {
            if (pParametros == null)
            {
                return;
            }

            foreach (KeyValuePair<string, object?> unParametro in pParametros)
            {
                // Un valor nulo se manda como NULL de la base, no como texto vacio
                pComando.Parameters.AddWithValue(unParametro.Key, unParametro.Value ?? DBNull.Value);
            }
        }
    }
}
