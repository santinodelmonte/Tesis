using Tesis.Dominio;
using System.Data;

namespace Tesis.Persistencia
{
    public class pOrdenieIndividual
    {
        private pConexion Conexion = new pConexion();

        public List<OrdenieIndividual> ListarOrdeniesIndividual(List<Hembra> pListaHembras,
            List<Lactancia> pListaLactancias, List<OrdenieLote> pListaOrdeniesLote)
        {
            string sql = "SELECT * FROM ordenies_individual ORDER BY fecha DESC, id_ordenie_ind DESC";
            DataTable datos = Conexion.EjecutarConsulta(sql);
            List<OrdenieIndividual> lista = new List<OrdenieIndividual>();

            foreach (DataRow fila in datos.Rows)
            {
                OrdenieIndividual unOrdenie = new OrdenieIndividual(
                    int.Parse(fila["id_ordenie_ind"].ToString()),
                    DateTime.Parse(fila["fecha"].ToString()),
                    fila["turno"].ToString(),
                    Convert.ToDouble(fila["litros"]),
                    this.BuscarHembra(pListaHembras, int.Parse(fila["id_animal"].ToString())),
                    this.BuscarLactancia(pListaLactancias, int.Parse(fila["id_lactancia"].ToString())),
                    fila["id_ordenie_lote"] != DBNull.Value
                        ? this.BuscarOrdenieLote(pListaOrdeniesLote, int.Parse(fila["id_ordenie_lote"].ToString()))
                        : null // El control individual no exige que el lote de ese turno este cargado
                    );
                lista.Add(unOrdenie);
            }
            return lista;
        }

        public bool AltaOrdenieIndividual(OrdenieIndividual pOrdenie)
        {
            string sql = "INSERT INTO ordenies_individual (fecha, turno, litros, id_animal, " +
                "id_lactancia, id_ordenie_lote) " +
                "VALUES (@fecha, @turno, @litros, @id_animal, @id_lactancia, @id_ordenie_lote)";

            Dictionary<string, object?> parametros = new Dictionary<string, object?>
            {
                { "@fecha", pOrdenie.Fecha.Date },
                { "@turno", pOrdenie.Turno },
                { "@litros", pOrdenie.Litros },
                { "@id_animal", pOrdenie.Animal.IdAnimal },
                { "@id_lactancia", pOrdenie.Lactancia.IdLactancia },
                { "@id_ordenie_lote", pOrdenie.OrdenieLote != null ? (object)pOrdenie.OrdenieLote.IdOrdenieLote : null }
            };

            pOrdenie.IdOrdenieInd = Conexion.EjecutarInsercion(sql, parametros);
            return pOrdenie.IdOrdenieInd > 0;
        }

        // La correccion reescribe unicamente los litros. La fecha, el turno y el animal
        // son la clave alterna del control: cambiarlos no es corregir este registro, es
        // borrarlo y cargar otro, que ademas es lo que la persona entiende que hizo
        // cuando anoto el control en la vaca de al lado.
        public bool ModificarOrdenieIndividual(OrdenieIndividual pOrdenie)
        {
            string sql = "UPDATE ordenies_individual SET litros = @litros " +
                "WHERE id_ordenie_ind = @id_ordenie_ind";

            Dictionary<string, object?> parametros = new Dictionary<string, object?>
            {
                { "@litros", pOrdenie.Litros },
                { "@id_ordenie_ind", pOrdenie.IdOrdenieInd }
            };

            return Conexion.EjecutarComando(sql, parametros);
        }

        public bool EliminarOrdenieIndividual(int pIdOrdenieInd)
        {
            return Conexion.EjecutarComando(
                "DELETE FROM ordenies_individual WHERE id_ordenie_ind = @id_ordenie_ind",
                new Dictionary<string, object?> { { "@id_ordenie_ind", pIdOrdenieInd } });
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

        private Lactancia BuscarLactancia(List<Lactancia> pLista, int pIdLactancia)
        {
            foreach (Lactancia unaLactancia in pLista)
            {
                if (unaLactancia.IdLactancia == pIdLactancia)
                {
                    return unaLactancia;
                }
            }
            return null;
        }

        private OrdenieLote BuscarOrdenieLote(List<OrdenieLote> pLista, int pIdOrdenieLote)
        {
            foreach (OrdenieLote unOrdenie in pLista)
            {
                if (unOrdenie.IdOrdenieLote == pIdOrdenieLote)
                {
                    return unOrdenie;
                }
            }
            return null;
        }
    }
}
