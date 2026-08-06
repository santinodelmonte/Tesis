using Tesis.Dominio;
using System.Data;

namespace Tesis.Persistencia
{
    public class pCategoria
    {
        private pConexion Conexion = new pConexion();

        public List<Categoria> ListarCategorias()
        {
            string sql = "SELECT * FROM categorias ORDER BY id_categoria";
            DataTable datos = Conexion.EjecutarConsulta(sql);
            List<Categoria> lista = new List<Categoria>();

            foreach (DataRow fila in datos.Rows)
            {
                Categoria unaCategoria = new Categoria(
                    int.Parse(fila["id_categoria"].ToString()),
                    fila["nombre"].ToString(),
                    fila["descripcion"] != DBNull.Value ? fila["descripcion"].ToString() : "" // Si es NULL, usa un string vacio
                    );
                lista.Add(unaCategoria);
            }
            return lista;
        }

        public bool AltaCategoria(Categoria pCategoria)
        {
            // El id_categoria lo asigna MySQL con el AUTO_INCREMENT
            string sql = "INSERT INTO categorias (nombre, descripcion) VALUES (@nombre, @descripcion)";

            Dictionary<string, object?> parametros = new Dictionary<string, object?>
            {
                { "@nombre", pCategoria.Nombre },
                { "@descripcion", pCategoria.Descripcion }
            };

            return Conexion.EjecutarComando(sql, parametros);
        }
    }
}
