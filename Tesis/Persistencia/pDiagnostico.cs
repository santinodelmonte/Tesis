using Tesis.Dominio;
using System.Data;

namespace Tesis.Persistencia
{
    public class pDiagnostico
    {
        private pConexion Conexion = new pConexion();

        public List<Diagnostico> ListarDiagnosticos(List<Animal> pListaAnimales)
        {
            string sql = "SELECT * FROM diagnosticos ORDER BY fecha_diagnostico DESC, id_diagnostico DESC";
            DataTable datos = Conexion.EjecutarConsulta(sql);
            List<Diagnostico> lista = new List<Diagnostico>();

            foreach (DataRow fila in datos.Rows)
            {
                Diagnostico unDiagnostico = new Diagnostico(
                    int.Parse(fila["id_diagnostico"].ToString()),
                    DateTime.Parse(fila["fecha_diagnostico"].ToString()),
                    fila["enfermedad"].ToString(),
                    fila["estado"].ToString(),
                    this.BuscarEnLista(pListaAnimales, int.Parse(fila["id_animal"].ToString()))
                    );
                lista.Add(unDiagnostico);
            }
            return lista;
        }

        public bool AltaDiagnostico(Diagnostico pDiagnostico)
        {
            string sql = "INSERT INTO diagnosticos (fecha_diagnostico, enfermedad, estado, id_animal) " +
                "VALUES (@fecha_diagnostico, @enfermedad, @estado, @id_animal)";

            Dictionary<string, object?> parametros = new Dictionary<string, object?>
            {
                { "@fecha_diagnostico", pDiagnostico.FechaDiagnostico.Date },
                { "@enfermedad", pDiagnostico.Enfermedad },
                { "@estado", pDiagnostico.Estado },
                { "@id_animal", pDiagnostico.Animal.IdAnimal }
            };

            pDiagnostico.IdDiagnostico = Conexion.EjecutarInsercion(sql, parametros);
            return pDiagnostico.IdDiagnostico > 0;
        }

        // El estado lo mueve el alta del tratamiento, que lo pasa a "En tratamiento",
        // por eso el comando se declara aparte: va dentro de esa transaccion.
        public const string SQL_MODIFICAR_ESTADO = "UPDATE diagnosticos SET estado = @estado " +
            "WHERE id_diagnostico = @id_diagnostico";

        public static Dictionary<string, object?> ParametrosModificarEstado(int pIdDiagnostico, string pEstado)
        {
            return new Dictionary<string, object?>
            {
                { "@estado", pEstado },
                { "@id_diagnostico", pIdDiagnostico }
            };
        }

        public bool ModificarDiagnostico(int pIdDiagnostico, string pEstado)
        {
            return Conexion.EjecutarComando(SQL_MODIFICAR_ESTADO,
                ParametrosModificarEstado(pIdDiagnostico, pEstado));
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
    }
}
