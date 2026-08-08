using Tesis.Dominio;
using System.Data;

namespace Tesis.Persistencia
{
    public class pDescorne
    {
        private pConexion Conexion = new pConexion();

        public List<Descorne> ListarDescornes(List<Animal> pListaAnimales,
            List<PlanSanitario> pListaPlanes)
        {
            string sql = "SELECT * FROM descornes ORDER BY fecha DESC, id_descorne DESC";
            DataTable datos = Conexion.EjecutarConsulta(sql);
            List<Descorne> lista = new List<Descorne>();

            foreach (DataRow fila in datos.Rows)
            {
                Descorne unDescorne = new Descorne(
                    int.Parse(fila["id_descorne"].ToString()),
                    DateTime.Parse(fila["fecha"].ToString()),
                    fila["metodo"].ToString(),
                    fila["observaciones"] != DBNull.Value ? fila["observaciones"].ToString() : "", // Si es NULL, usa un string vacio
                    this.BuscarAnimal(pListaAnimales, int.Parse(fila["id_animal"].ToString())),
                    fila["id_plan"] != DBNull.Value
                        ? this.BuscarPlan(pListaPlanes, int.Parse(fila["id_plan"].ToString()))
                        : null // El nulo es el descorne registrado fuera de todo plan
                    );
                lista.Add(unDescorne);
            }
            return lista;
        }

        // El descorne no consume insumo, asi que el alta es una sola escritura y no
        // necesita transaccion.
        public bool AltaDescorne(Descorne pDescorne)
        {
            string sql = "INSERT INTO descornes (fecha, metodo, observaciones, id_animal, id_plan) " +
                "VALUES (@fecha, @metodo, @observaciones, @id_animal, @id_plan)";

            Dictionary<string, object?> parametros = new Dictionary<string, object?>
            {
                { "@fecha", pDescorne.Fecha.Date },
                { "@metodo", pDescorne.Metodo },
                { "@observaciones", pDescorne.Observaciones },
                { "@id_animal", pDescorne.Animal.IdAnimal },
                { "@id_plan", pDescorne.Plan != null ? (object)pDescorne.Plan.IdPlan : null }
            };

            pDescorne.IdDescorne = Conexion.EjecutarInsercion(sql, parametros);
            return pDescorne.IdDescorne > 0;
        }

        private Animal BuscarAnimal(List<Animal> pLista, int pIdAnimal)
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

        private PlanSanitario BuscarPlan(List<PlanSanitario> pLista, int pIdPlan)
        {
            foreach (PlanSanitario unPlan in pLista)
            {
                if (unPlan.IdPlan == pIdPlan)
                {
                    return unPlan;
                }
            }
            return null;
        }
    }
}
