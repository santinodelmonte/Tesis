using Tesis.Dominio;
using System.Data;

namespace Tesis.Persistencia
{
    public class pRaza
    {
        private pConexion Conexion = new pConexion();

        public List<Raza> ListarRazas()
        {
            string sql = "SELECT * FROM razas ORDER BY nombre";
            DataTable datos = Conexion.EjecutarConsulta(sql);
            List<Raza> lista = new List<Raza>();

            foreach (DataRow fila in datos.Rows)
            {
                Raza unaRaza = new Raza(
                    int.Parse(fila["id_raza"].ToString()),
                    fila["nombre"].ToString(),
                    fila["descripcion"] != DBNull.Value ? fila["descripcion"].ToString() : "" // Si es NULL, usa un string vacio
                    );
                lista.Add(unaRaza);
            }
            return lista;
        }

        public bool AltaRaza(Raza pRaza)
        {
            // El id_raza lo asigna MySQL con el AUTO_INCREMENT
            string sql = "INSERT INTO razas (nombre, descripcion) VALUES (@nombre, @descripcion)";

            Dictionary<string, object?> parametros = new Dictionary<string, object?>
            {
                { "@nombre", pRaza.Nombre },
                { "@descripcion", pRaza.Descripcion }
            };

            return Conexion.EjecutarComando(sql, parametros);
        }
    }
}
