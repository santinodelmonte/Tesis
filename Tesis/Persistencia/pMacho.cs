using Microsoft.AspNetCore.Mvc;
using Tesis.Dominio;
using System.Data;

namespace Tesis.Persistencia
{
    public class pMacho
    {
        private pConexion Conexion = new pConexion();

        public List<Macho> ListarMachos()
        {
            string sql = "SELECT id_animal FROM machos ORDER BY id_animal";
            DataTable datos = Conexion.EjecutarConsulta(sql);
            List<Macho> lista = new List<Macho>();
            Controladora unaControladora = new Controladora();

            // El objeto ya fue armado por pAnimal, aca solo se recupera de la cache
            foreach (DataRow fila in datos.Rows)
            {
                Macho unMacho = unaControladora.BuscarAnimal(int.Parse(fila["id_animal"].ToString())) as Macho;
                if (unMacho != null)
                {
                    lista.Add(unMacho);
                }
            }
            return lista;
        }

        public bool AltaMacho(Macho pMacho)
        {
            string sql = "INSERT INTO machos (id_animal, en_pie) " +
                "VALUES ("
                + pMacho.IdAnimal + ","
                + (pMacho.EnPie ? 1 : 0) + ")";

            return Conexion.EjecutarComando(sql);
        }

        public int ProximoMachoId()
        {
            string sql = "SELECT (IFNULL(MAX(id_animal),0)+1) FROM machos";
            DataTable datos = Conexion.EjecutarConsulta(sql);
            DataRowCollection filas = datos.Rows;
            var campo = filas[0];
            int Id = int.Parse(campo[0].ToString());
            return Id;
        }
    }
}
