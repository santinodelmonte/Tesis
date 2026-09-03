using MySql.Data.MySqlClient;
using Tesis.Dominio;
using System.Data;

namespace Tesis.Persistencia
{
    public class pAlerta
    {
        private pConexion Conexion = new pConexion();

        // Guarda las alertas de un envio. Van todas juntas en una transaccion porque
        // son el registro de un unico mensaje: si la mitad quedara escrita, el sistema
        // creeria que el resumen del dia ya salio cuando en realidad salio a medias.
        public bool RegistrarAlertas(List<Alerta> pListaAlertas)
        {
            if (pListaAlertas.Count == 0)
            {
                return true;
            }

            string sql = "INSERT INTO alertas (tipo_alerta, fecha_generacion, mensaje, enviada, "
                + "id_preferencia, id_animal, id_insumo) "
                + "VALUES (@tipo_alerta, @fecha_generacion, @mensaje, @enviada, "
                + "@id_preferencia, @id_animal, @id_insumo)";

            using (MySqlConnection conexion = Conexion.AbrirConexion())
            {
                using (MySqlTransaction transaccion = conexion.BeginTransaction())
                {
                    try
                    {
                        foreach (Alerta unaAlerta in pListaAlertas)
                        {
                            Conexion.EjecutarComandoEnTransaccion(sql,
                                new Dictionary<string, object?>
                                {
                                    { "@tipo_alerta", unaAlerta.TipoAlerta },
                                    { "@fecha_generacion", unaAlerta.FechaGeneracion.Date },
                                    { "@mensaje", unaAlerta.Mensaje },
                                    { "@enviada", unaAlerta.Enviada },
                                    { "@id_preferencia", unaAlerta.IdPreferencia },
                                    { "@id_animal", unaAlerta.Animal != null ? (object)unaAlerta.Animal.IdAnimal : null },
                                    { "@id_insumo", unaAlerta.Insumo != null ? (object)unaAlerta.Insumo.IdInsumo : null }
                                },
                                conexion, transaccion);
                        }

                        transaccion.Commit();
                        return true;
                    }
                    catch (Exception e)
                    {
                        transaccion.Rollback();
                        throw new Exception("Error al registrar las alertas enviadas", e);
                    }
                }
            }
        }

        // Cuantos pendientes se avisaron en una fecha. Acompania a UltimoEnvio en la
        // pantalla: un resumen de cero pendientes tambien se envia, y sin este numero
        // no se distinguiria de uno que no se armo.
        public int ContarAlertas(DateTime pFecha)
        {
            string sql = "SELECT COUNT(*) FROM alertas WHERE fecha_generacion = @fecha";

            DataTable datos = Conexion.EjecutarConsulta(sql,
                new Dictionary<string, object?> { { "@fecha", pFecha.Date } });

            return Convert.ToInt32(datos.Rows[0][0]);
        }
    }
}
