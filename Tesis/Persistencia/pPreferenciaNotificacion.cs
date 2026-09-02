using Tesis.Dominio;
using System.Data;

namespace Tesis.Persistencia
{
    public class pPreferenciaNotificacion
    {
        private pConexion Conexion = new pConexion();

        // Los ocho tipos de aviso con su interruptor. Se cargan con el esquema, asi que
        // esta clase lee y actualiza pero no da de alta: un tipo de aviso nuevo es una
        // funcionalidad nueva del sistema, no un dato que la usuaria agregue.
        public List<PreferenciaNotificacion> ListarPreferencias()
        {
            string sql = "SELECT * FROM preferencias_notificacion ORDER BY id_preferencia";
            DataTable datos = Conexion.EjecutarConsulta(sql);
            List<PreferenciaNotificacion> lista = new List<PreferenciaNotificacion>();

            foreach (DataRow fila in datos.Rows)
            {
                PreferenciaNotificacion unaPreferencia = new PreferenciaNotificacion(
                    int.Parse(fila["id_preferencia"].ToString()),
                    fila["tipo_alerta"].ToString(),
                    Convert.ToBoolean(fila["activa"])
                    );
                lista.Add(unaPreferencia);
            }
            return lista;
        }

        public bool ModificarPreferencia(PreferenciaNotificacion pPreferencia)
        {
            string sql = "UPDATE preferencias_notificacion SET activa = @activa "
                + "WHERE id_preferencia = @id_preferencia";

            Dictionary<string, object?> parametros = new Dictionary<string, object?>
            {
                { "@activa", pPreferencia.Activa },
                { "@id_preferencia", pPreferencia.IdPreferencia }
            };

            return Conexion.EjecutarComando(sql, parametros);
        }
    }
}
