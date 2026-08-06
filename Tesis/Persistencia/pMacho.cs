using Tesis.Dominio;
using System.Data;

namespace Tesis.Persistencia
{
    public class pMacho
    {
        private pConexion Conexion = new pConexion();

        // El alta de macho la usan dos caminos: el alta de animal, que la ejecuta
        // dentro de su transaccion, y AltaMacho. Por eso el comando y sus parametros
        // se declaran una sola vez aca.
        public const string SQL_ALTA = "INSERT INTO machos (id_animal, en_pie) " +
            "VALUES (@id_animal, @en_pie)";

        public static Dictionary<string, object?> ParametrosAlta(Macho pMacho)
        {
            return new Dictionary<string, object?>
            {
                { "@id_animal", pMacho.IdAnimal },
                { "@en_pie", pMacho.EnPie ? 1 : 0 }
            };
        }

        // Los animales ya armados llegan por parametro. Antes esta clase instanciaba
        // una Controladora para sacarlos de su cache, con lo cual la capa de
        // persistencia terminaba llamando a la de dominio.
        public List<Macho> ListarMachos(List<Animal> pListaAnimales)
        {
            string sql = "SELECT id_animal FROM machos ORDER BY id_animal";
            DataTable datos = Conexion.EjecutarConsulta(sql);
            List<Macho> lista = new List<Macho>();

            // El objeto ya fue armado por pAnimal, aca solo se selecciona
            foreach (DataRow fila in datos.Rows)
            {
                Macho unMacho = this.BuscarEnLista(pListaAnimales, int.Parse(fila["id_animal"].ToString())) as Macho;
                if (unMacho != null)
                {
                    lista.Add(unMacho);
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

        public bool AltaMacho(Macho pMacho)
        {
            return Conexion.EjecutarComando(SQL_ALTA, ParametrosAlta(pMacho));
        }

        // Igual que el alta, la modificacion la usan dos caminos: la modificacion de
        // animal, que la ejecuta dentro de su transaccion, y ModificarMacho.
        public const string SQL_MODIFICAR = "UPDATE machos SET en_pie = @en_pie "
            + "WHERE id_animal = @id_animal";

        public static Dictionary<string, object?> ParametrosModificar(Macho pMacho)
        {
            return new Dictionary<string, object?>
            {
                { "@en_pie", pMacho.EnPie ? 1 : 0 },
                { "@id_animal", pMacho.IdAnimal }
            };
        }

        public bool ModificarMacho(Macho pMacho)
        {
            return Conexion.EjecutarComando(SQL_MODIFICAR, ParametrosModificar(pMacho));
        }
    }
}
