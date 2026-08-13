using Tesis.Dominio;
using System.Data;

namespace Tesis.Persistencia
{
    public class pCelo
    {
        private pConexion Conexion = new pConexion();

        public List<Celo> ListarCelos(List<Hembra> pListaHembras)
        {
            string sql = "SELECT * FROM celos ORDER BY fecha DESC, id_celo DESC";
            DataTable datos = Conexion.EjecutarConsulta(sql);
            List<Celo> lista = new List<Celo>();

            foreach (DataRow fila in datos.Rows)
            {
                Celo unCelo = new Celo(
                    int.Parse(fila["id_celo"].ToString()),
                    DateTime.Parse(fila["fecha"].ToString()),
                    fila["observaciones"] != DBNull.Value ? fila["observaciones"].ToString() : "", // Si es NULL, usa un string vacio
                    this.BuscarHembra(pListaHembras, int.Parse(fila["id_animal"].ToString()))
                    );
                lista.Add(unCelo);
            }
            return lista;
        }

        public bool AltaCelo(Celo pCelo)
        {
            string sql = "INSERT INTO celos (fecha, observaciones, id_animal) " +
                "VALUES (@fecha, @observaciones, @id_animal)";

            Dictionary<string, object?> parametros = new Dictionary<string, object?>
            {
                { "@fecha", pCelo.Fecha.Date },
                { "@observaciones", pCelo.Observaciones },
                { "@id_animal", pCelo.Animal.IdAnimal }
            };

            pCelo.IdCelo = Conexion.EjecutarInsercion(sql, parametros);
            return pCelo.IdCelo > 0;
        }

        // El celo es el unico registro de reproduccion que no arrastra nada: no mueve
        // el estado de la hembra ni consume stock, y ningun otro registro lo
        // referencia. Por eso su correccion y su borrado son una sola escritura.
        public bool ModificarCelo(Celo pCelo)
        {
            string sql = "UPDATE celos SET fecha = @fecha, observaciones = @observaciones, "
                + "id_animal = @id_animal WHERE id_celo = @id_celo";

            Dictionary<string, object?> parametros = new Dictionary<string, object?>
            {
                { "@fecha", pCelo.Fecha.Date },
                { "@observaciones", pCelo.Observaciones },
                { "@id_animal", pCelo.Animal.IdAnimal },
                { "@id_celo", pCelo.IdCelo }
            };

            return Conexion.EjecutarComando(sql, parametros);
        }

        public bool EliminarCelo(int pIdCelo)
        {
            return Conexion.EjecutarComando("DELETE FROM celos WHERE id_celo = @id_celo",
                new Dictionary<string, object?> { { "@id_celo", pIdCelo } });
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
