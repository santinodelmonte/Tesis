using Tesis.Dominio;
using System.Data;

namespace Tesis.Persistencia
{
    public class pHembra
    {
        private pConexion Conexion = new pConexion();

        // El alta de hembra la usan dos caminos: el alta de animal, que la ejecuta
        // dentro de su transaccion, y AltaHembra. Por eso el comando y sus parametros
        // se declaran una sola vez aca.
        public const string SQL_ALTA = "INSERT INTO hembras (id_animal, numero_partos, estado_productivo, estado_reproductivo) " +
            "VALUES (@id_animal, @numero_partos, @estado_productivo, @estado_reproductivo)";

        public static Dictionary<string, object?> ParametrosAlta(Hembra pHembra)
        {
            return new Dictionary<string, object?>
            {
                { "@id_animal", pHembra.IdAnimal },
                { "@numero_partos", pHembra.NumeroPartos },
                { "@estado_productivo", pHembra.EstadoProductivo },
                { "@estado_reproductivo", pHembra.EstadoReproductivo }
            };
        }

        // Los animales ya armados llegan por parametro. Antes esta clase instanciaba
        // una Controladora para sacarlos de su cache, con lo cual la capa de
        // persistencia terminaba llamando a la de dominio.
        public List<Hembra> ListarHembras(List<Animal> pListaAnimales)
        {
            string sql = "SELECT id_animal FROM hembras ORDER BY id_animal";
            DataTable datos = Conexion.EjecutarConsulta(sql);
            List<Hembra> lista = new List<Hembra>();

            // El objeto ya fue armado por pAnimal, aca solo se selecciona
            foreach (DataRow fila in datos.Rows)
            {
                Hembra unaHembra = this.BuscarEnLista(pListaAnimales, int.Parse(fila["id_animal"].ToString())) as Hembra;
                if (unaHembra != null)
                {
                    lista.Add(unaHembra);
                }
            }
            return lista;
        }

        private Animal BuscarEnLista(List<Animal> pLista, int pIdAnimal)
        {
            foreach (Animal unAnimal in pLista)
            {
                if (unAnimal.IdAnimal == pIdAnimal)
                {
                    return unAnimal;
                }
            }
            return null;
        }

        public bool AltaHembra(Hembra pHembra)
        {
            return Conexion.EjecutarComando(SQL_ALTA, ParametrosAlta(pHembra));
        }

        // Igual que el alta, la modificacion la usan dos caminos: la modificacion de
        // animal, que la ejecuta dentro de su transaccion, y ModificarHembra.
        public const string SQL_MODIFICAR = "UPDATE hembras SET "
            + "numero_partos = @numero_partos,"
            + "estado_productivo = @estado_productivo,"
            + "estado_reproductivo = @estado_reproductivo "
            + "WHERE id_animal = @id_animal";

        public static Dictionary<string, object?> ParametrosModificar(Hembra pHembra)
        {
            return new Dictionary<string, object?>
            {
                { "@numero_partos", pHembra.NumeroPartos },
                { "@estado_productivo", pHembra.EstadoProductivo },
                { "@estado_reproductivo", pHembra.EstadoReproductivo },
                { "@id_animal", pHembra.IdAnimal }
            };
        }

        public bool ModificarHembra(Hembra pHembra)
        {
            return Conexion.EjecutarComando(SQL_MODIFICAR, ParametrosModificar(pHembra));
        }
    }
}
