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
