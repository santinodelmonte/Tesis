using Tesis.Dominio;
using System.Data;

namespace Tesis.Persistencia
{
    public class pConfiguracion
    {
        private pConexion Conexion = new pConexion();

        // Devuelve la unica fila de configuracion, o nulo si la tabla esta vacia. Ese
        // nulo no es un error: la Controladora responde con los valores por defecto,
        // para que el sistema ande aunque nadie haya configurado nada todavia.
        public Configuracion ObtenerConfiguracion()
        {
            string sql = "SELECT * FROM configuracion ORDER BY id_configuracion LIMIT 1";
            DataTable datos = Conexion.EjecutarConsulta(sql);

            if (datos.Rows.Count == 0)
            {
                return null;
            }

            DataRow fila = datos.Rows[0];

            return new Configuracion(
                int.Parse(fila["id_configuracion"].ToString()),
                int.Parse(fila["dias_secado_antes_parto"].ToString()),
                int.Parse(fila["edad_minima_servicio_meses"].ToString()),
                int.Parse(fila["edad_cambio_categoria_meses"].ToString()),
                Convert.ToDouble(fila["litros_maximos_individual"]),
                int.Parse(fila["ordenies_por_dia"].ToString()),
                int.Parse(fila["dias_anticipacion_secado"].ToString()),
                int.Parse(fila["dias_anticipacion_parto"].ToString()),
                int.Parse(fila["dias_anticipacion_sanitaria"].ToString()),
                int.Parse(fila["dias_anticipacion_vencimiento"].ToString()),
                int.Parse(fila["dias_espera_voluntaria"].ToString()),
                int.Parse(fila["dias_para_tacto"].ToString()),
                // MySQL devuelve TIME como TimeSpan, no como DateTime
                (TimeSpan)fila["hora_resumen"],
                fila["chat_telegram"] != DBNull.Value ? fila["chat_telegram"].ToString() : "",
                fila["fecha_ultimo_resumen"] != DBNull.Value
                    ? Convert.ToDateTime(fila["fecha_ultimo_resumen"]) : DateTime.MinValue
                );
        }

        // La configuracion no se da de alta: se modifica la fila que ya existe.
        public bool ModificarConfiguracion(Configuracion pConfiguracion)
        {
            string sql = "UPDATE configuracion SET "
                + "dias_secado_antes_parto = @dias_secado_antes_parto,"
                + "edad_minima_servicio_meses = @edad_minima_servicio_meses,"
                + "edad_cambio_categoria_meses = @edad_cambio_categoria_meses,"
                + "litros_maximos_individual = @litros_maximos_individual,"
                + "ordenies_por_dia = @ordenies_por_dia,"
                + "dias_anticipacion_secado = @dias_anticipacion_secado,"
                + "dias_anticipacion_parto = @dias_anticipacion_parto,"
                + "dias_anticipacion_sanitaria = @dias_anticipacion_sanitaria,"
                + "dias_anticipacion_vencimiento = @dias_anticipacion_vencimiento,"
                + "dias_espera_voluntaria = @dias_espera_voluntaria,"
                + "dias_para_tacto = @dias_para_tacto,"
                + "hora_resumen = @hora_resumen,"
                + "chat_telegram = @chat_telegram,"
                + "fecha_ultimo_resumen = @fecha_ultimo_resumen "
                + "WHERE id_configuracion = @id_configuracion";

            Dictionary<string, object?> parametros = new Dictionary<string, object?>
            {
                { "@dias_secado_antes_parto", pConfiguracion.DiasSecadoAntesParto },
                { "@edad_minima_servicio_meses", pConfiguracion.EdadMinimaServicioMeses },
                { "@edad_cambio_categoria_meses", pConfiguracion.EdadCambioCategoriaMeses },
                { "@litros_maximos_individual", pConfiguracion.LitrosMaximosIndividual },
                { "@ordenies_por_dia", pConfiguracion.OrdeniesPorDia },
                { "@dias_anticipacion_secado", pConfiguracion.DiasAnticipacionSecado },
                { "@dias_anticipacion_parto", pConfiguracion.DiasAnticipacionParto },
                { "@dias_anticipacion_sanitaria", pConfiguracion.DiasAnticipacionSanitaria },
                { "@dias_anticipacion_vencimiento", pConfiguracion.DiasAnticipacionVencimiento },
                { "@dias_espera_voluntaria", pConfiguracion.DiasEsperaVoluntaria },
                { "@dias_para_tacto", pConfiguracion.DiasParaTacto },
                { "@hora_resumen", pConfiguracion.HoraResumen },
                // El vacio se guarda como NULL: la columna admite nulo justamente para
                // representar el establecimiento que todavia no vinculo ninguna cuenta.
                { "@chat_telegram", pConfiguracion.ChatTelegram != "" ? (object)pConfiguracion.ChatTelegram : null },
                { "@fecha_ultimo_resumen", pConfiguracion.FechaUltimoResumen != DateTime.MinValue
                    ? (object)pConfiguracion.FechaUltimoResumen.Date : null },
                { "@id_configuracion", pConfiguracion.IdConfiguracion }
            };

            return Conexion.EjecutarComando(sql, parametros);
        }
    }
}
