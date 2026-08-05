using Microsoft.AspNetCore.Mvc;
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
            string sql = "INSERT INTO razas (nombre, descripcion) " +
                "VALUES ('"
                + pRaza.Nombre + "','"
                + pRaza.Descripcion + "')";

            return Conexion.EjecutarComando(sql);
        }

        public int ProximoRazaId()
        {
            string sql = "SELECT (IFNULL(MAX(id_raza),0)+1) FROM razas";
            DataTable datos = Conexion.EjecutarConsulta(sql);
            DataRowCollection filas = datos.Rows;
            var campo = filas[0];
            int Id = int.Parse(campo[0].ToString());
            return Id;
        }
    }
}
