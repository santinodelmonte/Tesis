using MySql.Data.MySqlClient;
using System.Data;

namespace Tesis.Persistencia
{
    public class pConexion
    {
        private static string mCadenaConexion = "";

        public static void Configurar(string pCadenaConexion)
        {
            mCadenaConexion = pCadenaConexion;
        }


        public bool EjecutarComando(string pSql)
        {
            return this.EjecutarComando(pSql, null);
        }

        public bool EjecutarComando(string pSql, Dictionary<string, object?>? pParametros)
        {
            try
            {
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
                // El motivo real viaja en la excepcion interna. Se copia al mensaje
                // porque es lo unico que se ve al vuelo en el depurador, y sin eso un
                // "no se puede conectar" y un "no existe la tabla" se leen igual.
                throw new Exception("Error en conexion sql = " + pSql + " -- " + e.Message, e);
            }
        }

        public int EjecutarInsercion(string pSql, Dictionary<string, object?> pParametros)
        {
            try
            {
                using (MySqlConnection conexion = new MySqlConnection(mCadenaConexion))
                {
                    conexion.Open();
                    using (MySqlCommand comando = new MySqlCommand(pSql, conexion))
                    {
                        this.CargarParametros(comando, pParametros);
                        comando.ExecuteNonQuery();
                        return (int)comando.LastInsertedId;
                    }
                }
            }
            catch (Exception e)
            {
                // El motivo real viaja en la excepcion interna. Se copia al mensaje
                // porque es lo unico que se ve al vuelo en el depurador, y sin eso un
                // "no se puede conectar" y un "no existe la tabla" se leen igual.
                throw new Exception("Error en conexion sql = " + pSql + " -- " + e.Message, e);
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
                // El motivo real viaja en la excepcion interna. Se copia al mensaje
                // porque es lo unico que se ve al vuelo en el depurador, y sin eso un
                // "no se puede conectar" y un "no existe la tabla" se leen igual.
                throw new Exception("Error en conexion sql = " + pSql + " -- " + e.Message, e);
            }
        }

        public MySqlConnection AbrirConexion()
        {
            MySqlConnection conexion = new MySqlConnection(mCadenaConexion);
            conexion.Open();
            return conexion;
        }

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
                pComando.Parameters.AddWithValue(unParametro.Key, unParametro.Value ?? DBNull.Value);
            }
        }
    }
}
