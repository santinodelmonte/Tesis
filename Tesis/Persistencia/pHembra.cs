using Microsoft.AspNetCore.Mvc;
using Tesis.Dominio;
using System.Data;

namespace Tesis.Persistencia
{
    public class pHembra
    {
        private pConexion Conexion = new pConexion();

        public List<Hembra> ListarHembras()
        {
            string sql = "SELECT id_animal FROM hembras ORDER BY id_animal";
            DataTable datos = Conexion.EjecutarConsulta(sql);
            List<Hembra> lista = new List<Hembra>();
            Controladora unaControladora = new Controladora();

            // El objeto ya fue armado por pAnimal, aca solo se recupera de la cache
            foreach (DataRow fila in datos.Rows)
            {
                Hembra unaHembra = unaControladora.BuscarAnimal(int.Parse(fila["id_animal"].ToString())) as Hembra;
                if (unaHembra != null)
                {
                    lista.Add(unaHembra);
                }
            }
            return lista;
        }

        public bool AltaHembra(Hembra pHembra)
        {
            string sql = "INSERT INTO hembras (id_animal, numero_partos, estado_productivo, estado_reproductivo) " +
                "VALUES ("
                + pHembra.IdAnimal + ","
                + pHembra.NumeroPartos + ",'"
                + pHembra.EstadoProductivo + "','"
                + pHembra.EstadoReproductivo + "')";

            return Conexion.EjecutarComando(sql);
        }

        public bool ModificarHembra(Hembra pHembra)
        {
            string sql = "UPDATE hembras SET "
                + "numero_partos = " + pHembra.NumeroPartos + ","
                + "estado_productivo = '" + pHembra.EstadoProductivo + "',"
                + "estado_reproductivo = '" + pHembra.EstadoReproductivo + "' "
                + "WHERE id_animal = " + pHembra.IdAnimal.ToString();

            return Conexion.EjecutarComando(sql);
        }

        public int ProximoHembraId()
        {
            string sql = "SELECT (IFNULL(MAX(id_animal),0)+1) FROM hembras";
            DataTable datos = Conexion.EjecutarConsulta(sql);
            DataRowCollection filas = datos.Rows;
            var campo = filas[0];
            int Id = int.Parse(campo[0].ToString());
            return Id;
        }
    }
}
