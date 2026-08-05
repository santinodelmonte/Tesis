using Microsoft.AspNetCore.Mvc;
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
            string sql = "INSERT INTO categorias (nombre, descripcion) " +
                "VALUES ('"
                + pCategoria.Nombre + "','"
                + pCategoria.Descripcion + "')";

            return Conexion.EjecutarComando(sql);
        }

        public int ProximoCategoriaId()
        {
            string sql = "SELECT (IFNULL(MAX(id_categoria),0)+1) FROM categorias";
            DataTable datos = Conexion.EjecutarConsulta(sql);
            DataRowCollection filas = datos.Rows;
            var campo = filas[0];
            int Id = int.Parse(campo[0].ToString());
            return Id;
        }
    }
}
