using MySql.Data.MySqlClient;
using Tesis.Dominio;
using System.Data;

namespace Tesis.Persistencia
{
    public class pTacto
    {
        private pConexion Conexion = new pConexion();

        // Los servicios llegan ya armados: el tacto cuelga del servicio y no de la
        // hembra, porque es el servicio lo que el control viene a confirmar.
        public List<Tacto> ListarTactos(List<Servicio> pListaServicios)
        {
            string sql = "SELECT * FROM tactos ORDER BY fecha_tacto DESC, id_tacto DESC";
            DataTable datos = Conexion.EjecutarConsulta(sql);
            List<Tacto> lista = new List<Tacto>();

            foreach (DataRow fila in datos.Rows)
            {
                Tacto unTacto = new Tacto(
                    int.Parse(fila["id_tacto"].ToString()),
                    DateTime.Parse(fila["fecha_tacto"].ToString()),
                    fila["resultado"].ToString(),
                    fila["observaciones"] != DBNull.Value ? fila["observaciones"].ToString() : "", // Si es NULL, usa un string vacio
                    this.BuscarServicio(pListaServicios, int.Parse(fila["id_servicio"].ToString()))
                    );
                lista.Add(unTacto);
            }
            return lista;
        }

        // El tacto se guarda dentro de la transaccion que ademas actualiza el estado
        // reproductivo de la hembra y la fecha probable de parto, por eso el comando y
        // sus parametros se declaran aparte.
        public const string SQL_ALTA = "INSERT INTO tactos (fecha_tacto, resultado, observaciones, id_servicio) " +
            "VALUES (@fecha_tacto, @resultado, @observaciones, @id_servicio)";

        public static Dictionary<string, object?> ParametrosAlta(Tacto pTacto)
        {
            return new Dictionary<string, object?>
            {
                { "@fecha_tacto", pTacto.FechaTacto.Date },
                { "@resultado", pTacto.Resultado },
                { "@observaciones", pTacto.Observaciones },
                { "@id_servicio", pTacto.Servicio.IdServicio }
            };
        }

        // El tacto no se guarda solo: mueve el estado reproductivo de la hembra y,
        // cuando confirma la preniez, baja la fecha probable de parto a la lactancia
        // en curso, que es de donde CU13 saca la fecha recomendada de secado. La
        // hembra y la lactancia llegan con los valores nuevos ya puestos desde el
        // dominio. La lactancia puede venir nula: una vaquillona de primer servicio
        // todavia no tiene ninguna abierta.
        //
        // Los parametros se llaman pHembraTactada y pLactanciaVigente y no pHembra ni
        // pLactancia porque esos son los nombres de las clases de persistencia que se
        // usan mas abajo.
        public bool AltaTacto(Tacto pTacto, Hembra pHembraTactada, Lactancia pLactanciaVigente)
        {
            using (MySqlConnection conexion = Conexion.AbrirConexion())
            {
                using (MySqlTransaction transaccion = conexion.BeginTransaction())
                {
                    try
                    {
                        int vIdNuevo = Conexion.EjecutarInsercionEnTransaccion(SQL_ALTA,
                            ParametrosAlta(pTacto), conexion, transaccion);
                        pTacto.IdTacto = vIdNuevo;

                        Conexion.EjecutarComandoEnTransaccion(pHembra.SQL_MODIFICAR,
                            pHembra.ParametrosModificar(pHembraTactada), conexion, transaccion);

                        if (pLactanciaVigente != null)
                        {
                            Conexion.EjecutarComandoEnTransaccion(pLactancia.SQL_MODIFICAR,
                                pLactancia.ParametrosModificar(pLactanciaVigente), conexion, transaccion);
                        }

                        transaccion.Commit();
                        return true;
                    }
                    catch (Exception e)
                    {
                        transaccion.Rollback();
                        // El id que habia asignado la base ya no vale, el alta se deshizo
                        pTacto.IdTacto = 0;
                        throw new Exception("Error al registrar el tacto", e);
                    }
                }
            }
        }

        private Servicio BuscarServicio(List<Servicio> pLista, int pIdServicio)
        {
            foreach (Servicio unServicio in pLista)
            {
                if (unServicio.IdServicio == pIdServicio)
                {
                    return unServicio;
                }
            }
            return null;
        }
    }
}
