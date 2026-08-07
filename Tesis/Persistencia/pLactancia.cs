using MySql.Data.MySqlClient;
using Tesis.Dominio;
using System.Data;

namespace Tesis.Persistencia
{
    public class pLactancia
    {
        private pConexion Conexion = new pConexion();

        // Las hembras llegan ya armadas por parametro, igual que en el resto de la
        // capa: la persistencia no instancia la Controladora para resolverlas.
        public List<Lactancia> ListarLactancias(List<Hembra> pListaHembras)
        {
            string sql = "SELECT * FROM lactancias ORDER BY id_animal, numero_lactancia";
            DataTable datos = Conexion.EjecutarConsulta(sql);
            List<Lactancia> lista = new List<Lactancia>();

            foreach (DataRow fila in datos.Rows)
            {
                Lactancia unaLactancia = new Lactancia(
                    int.Parse(fila["id_lactancia"].ToString()),
                    int.Parse(fila["numero_lactancia"].ToString()),
                    DateTime.Parse(fila["fecha_inicio"].ToString()),
                    fila["fecha_secado"] != DBNull.Value ? DateTime.Parse(fila["fecha_secado"].ToString()) : DateTime.MinValue, // El nulo identifica a la lactancia en curso
                    fila["fecha_probable_parto"] != DBNull.Value ? DateTime.Parse(fila["fecha_probable_parto"].ToString()) : DateTime.MinValue, // Si es NULL, todavia no hay preniez confirmada
                    this.BuscarHembra(pListaHembras, int.Parse(fila["id_animal"].ToString()))
                    );
                lista.Add(unaLactancia);
            }
            return lista;
        }

        // El alta la usan dos caminos: la apertura manual y el registro del parto, que
        // la ejecuta dentro de su transaccion. Por eso el comando y sus parametros se
        // declaran una sola vez aca.
        public const string SQL_ALTA = "INSERT INTO lactancias (numero_lactancia, fecha_inicio, " +
            "fecha_secado, fecha_probable_parto, id_animal) " +
            "VALUES (@numero_lactancia, @fecha_inicio, @fecha_secado, @fecha_probable_parto, @id_animal)";

        public static Dictionary<string, object?> ParametrosAlta(Lactancia pLactancia)
        {
            return new Dictionary<string, object?>
            {
                { "@numero_lactancia", pLactancia.NumeroLactancia },
                { "@fecha_inicio", pLactancia.FechaInicio.Date },
                { "@fecha_secado", pLactancia.FechaSecado != DateTime.MinValue ? (object)pLactancia.FechaSecado.Date : null },
                { "@fecha_probable_parto", pLactancia.FechaProbableParto != DateTime.MinValue ? (object)pLactancia.FechaProbableParto.Date : null },
                { "@id_animal", pLactancia.Animal.IdAnimal }
            };
        }

        public bool AltaLactancia(Lactancia pLactancia)
        {
            pLactancia.IdLactancia = Conexion.EjecutarInsercion(SQL_ALTA, ParametrosAlta(pLactancia));
            return pLactancia.IdLactancia > 0;
        }

        public const string SQL_MODIFICAR = "UPDATE lactancias SET "
            + "numero_lactancia = @numero_lactancia,"
            + "fecha_inicio = @fecha_inicio,"
            + "fecha_secado = @fecha_secado,"
            + "fecha_probable_parto = @fecha_probable_parto "
            + "WHERE id_lactancia = @id_lactancia";

        public static Dictionary<string, object?> ParametrosModificar(Lactancia pLactancia)
        {
            return new Dictionary<string, object?>
            {
                { "@numero_lactancia", pLactancia.NumeroLactancia },
                { "@fecha_inicio", pLactancia.FechaInicio.Date },
                { "@fecha_secado", pLactancia.FechaSecado != DateTime.MinValue ? (object)pLactancia.FechaSecado.Date : null },
                { "@fecha_probable_parto", pLactancia.FechaProbableParto != DateTime.MinValue ? (object)pLactancia.FechaProbableParto.Date : null },
                { "@id_lactancia", pLactancia.IdLactancia }
            };
        }

        public bool ModificarLactancia(Lactancia pLactancia)
        {
            return Conexion.EjecutarComando(SQL_MODIFICAR, ParametrosModificar(pLactancia));
        }

        // El secado cierra la lactancia y deja seca a la hembra. Las dos escrituras
        // van juntas: una lactancia cerrada con la hembra todavia en lactancia la
        // volveria a meter en el lote de ordenie del turno siguiente.
        //
        // El parametro se llama pHembraSeca y no pHembra porque ese es el nombre de la
        // clase de persistencia que se usa mas abajo.
        public bool RegistrarSecado(Lactancia pLactanciaCerrada, Hembra pHembraSeca)
        {
            using (MySqlConnection conexion = Conexion.AbrirConexion())
            {
                using (MySqlTransaction transaccion = conexion.BeginTransaction())
                {
                    try
                    {
                        Conexion.EjecutarComandoEnTransaccion(SQL_MODIFICAR,
                            ParametrosModificar(pLactanciaCerrada), conexion, transaccion);

                        Conexion.EjecutarComandoEnTransaccion(pHembra.SQL_MODIFICAR,
                            pHembra.ParametrosModificar(pHembraSeca), conexion, transaccion);

                        transaccion.Commit();
                        return true;
                    }
                    catch (Exception e)
                    {
                        transaccion.Rollback();
                        throw new Exception("Error al registrar el secado", e);
                    }
                }
            }
        }

        private Hembra BuscarHembra(List<Hembra> pLista, int pIdAnimal)
        {
            foreach (Hembra unaHembra in pLista)
            {
                if (unaHembra.IdAnimal == pIdAnimal)
                {
                    return unaHembra;
                }
            }
            return null;
        }
    }
}
