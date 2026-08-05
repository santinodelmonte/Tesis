using Microsoft.AspNetCore.Mvc;
using Tesis.Dominio;
using System.Data;

namespace Tesis.Persistencia
{
    public class pAnimal
    {
        private pConexion Conexion = new pConexion();

        public List<Animal> ListarAnimales()
        {
            string sql = "SELECT a.id_animal, a.num_caravana, a.fecha_nacimiento, a.activo, a.fecha_baja, " +
                "a.motivo_baja, a.id_raza, a.id_categoria, a.id_madre, a.id_padre, " +
                "h.numero_partos, h.estado_productivo, h.estado_reproductivo, m.en_pie " +
                "FROM animales a " +
                "LEFT JOIN hembras h ON h.id_animal = a.id_animal " +
                "LEFT JOIN machos m ON m.id_animal = a.id_animal " +
                "ORDER BY a.id_animal";

            DataTable datos = Conexion.EjecutarConsulta(sql);
            List<Animal> lista = new List<Animal>();
            Controladora unaControladora = new Controladora();

            // Primera pasada: se arman las hembras y los machos sin resolver la ascendencia
            foreach (DataRow fila in datos.Rows)
            {
                Animal unAnimal;

                if (fila["estado_productivo"] != DBNull.Value)
                {
                    unAnimal = new Hembra(
                        int.Parse(fila["id_animal"].ToString()),
                        fila["num_caravana"].ToString(),
                        DateTime.Parse(fila["fecha_nacimiento"].ToString()),
                        Convert.ToBoolean(fila["activo"]), // MySQL devuelve el tinyint(1) como bool
                        fila["fecha_baja"] != DBNull.Value ? DateTime.Parse(fila["fecha_baja"].ToString()) : DateTime.MinValue, // Si es NULL, usa la fecha minima
                        fila["motivo_baja"] != DBNull.Value ? fila["motivo_baja"].ToString() : "", // Si es NULL, usa un string vacio
                        unaControladora.BuscarRaza(int.Parse(fila["id_raza"].ToString())),
                        unaControladora.BuscarCategoria(int.Parse(fila["id_categoria"].ToString())),
                        null,
                        null,
                        int.Parse(fila["numero_partos"].ToString()),
                        fila["estado_productivo"].ToString(),
                        fila["estado_reproductivo"].ToString());
                }
                else
                {
                    unAnimal = new Macho(
                        int.Parse(fila["id_animal"].ToString()),
                        fila["num_caravana"].ToString(),
                        DateTime.Parse(fila["fecha_nacimiento"].ToString()),
                        Convert.ToBoolean(fila["activo"]), // MySQL devuelve el tinyint(1) como bool
                        fila["fecha_baja"] != DBNull.Value ? DateTime.Parse(fila["fecha_baja"].ToString()) : DateTime.MinValue, // Si es NULL, usa la fecha minima
                        fila["motivo_baja"] != DBNull.Value ? fila["motivo_baja"].ToString() : "", // Si es NULL, usa un string vacio
                        unaControladora.BuscarRaza(int.Parse(fila["id_raza"].ToString())),
                        unaControladora.BuscarCategoria(int.Parse(fila["id_categoria"].ToString())),
                        null,
                        null,
                        fila["en_pie"] != DBNull.Value && Convert.ToBoolean(fila["en_pie"])); // Si es NULL, usa false
                }

                lista.Add(unAnimal);
            }

            // Segunda pasada: ya estan todos los objetos creados, se enganchan madre y padre
            int i = 0;
            foreach (DataRow fila in datos.Rows)
            {
                Animal unAnimal = lista[i];

                if (fila["id_madre"] != DBNull.Value)
                {
                    unAnimal.Madre = this.BuscarEnLista(lista, int.Parse(fila["id_madre"].ToString())) as Hembra;
                }
                if (fila["id_padre"] != DBNull.Value)
                {
                    unAnimal.Padre = this.BuscarEnLista(lista, int.Parse(fila["id_padre"].ToString())) as Macho;
                }

                i = i + 1;
            }

            return lista;
        }

        public bool AltaAnimal(Animal pAnimal)
        {
            string vIdMadre = "NULL";
            if (pAnimal.Madre != null)
            {
                vIdMadre = pAnimal.Madre.IdAnimal.ToString();
            }

            string vIdPadre = "NULL";
            if (pAnimal.Padre != null)
            {
                vIdPadre = pAnimal.Padre.IdAnimal.ToString();
            }

            string sql = "INSERT INTO animales (id_animal, num_caravana, fecha_nacimiento, activo, fecha_baja, " +
                "motivo_baja, id_raza, id_categoria, id_madre, id_padre) " +
                "VALUES ("
                + pAnimal.IdAnimal + ",'"
                + pAnimal.NumCaravana + "','"
                + pAnimal.FechaNacimiento.ToString("yyyy-MM-dd") + "',"
                + (pAnimal.Activo ? 1 : 0) + ","
                + "NULL,"
                + "NULL,"
                + pAnimal.Raza.IdRaza + ","
                + pAnimal.Categoria.IdCategoria + ","
                + vIdMadre + ","
                + vIdPadre + ")";

            return Conexion.EjecutarComando(sql);
        }

        public bool ModificarAnimal(Animal pAnimal)
        {
            string vIdMadre = "NULL";
            if (pAnimal.Madre != null)
            {
                vIdMadre = pAnimal.Madre.IdAnimal.ToString();
            }

            string vIdPadre = "NULL";
            if (pAnimal.Padre != null)
            {
                vIdPadre = pAnimal.Padre.IdAnimal.ToString();
            }

            string sql = "UPDATE animales SET "
                + "num_caravana = '" + pAnimal.NumCaravana + "',"
                + "fecha_nacimiento = '" + pAnimal.FechaNacimiento.ToString("yyyy-MM-dd") + "',"
                + "id_raza = " + pAnimal.Raza.IdRaza + ","
                + "id_categoria = " + pAnimal.Categoria.IdCategoria + ","
                + "id_madre = " + vIdMadre + ","
                + "id_padre = " + vIdPadre + " "
                + "WHERE id_animal = " + pAnimal.IdAnimal.ToString();

            return Conexion.EjecutarComando(sql);
        }

        public bool BajaAnimal(int pIdAnimal, string pMotivoBaja)
        {
            string sql = "UPDATE animales SET "
                + "activo = 0,"
                + "fecha_baja = '" + DateTime.Now.ToString("yyyy-MM-dd") + "',"
                + "motivo_baja = '" + pMotivoBaja + "' "
                + "WHERE id_animal = " + pIdAnimal.ToString();

            return Conexion.EjecutarComando(sql);
        }

        public bool EliminarAnimal(int pIdAnimal)
        {
            string sql = "DELETE FROM animales Where id_animal = " + pIdAnimal.ToString();

            return Conexion.EjecutarComando(sql);
        }

        public int ProximoAnimalId()
        {
            string sql = "SELECT (IFNULL(MAX(id_animal),0)+1) FROM animales";
            DataTable datos = Conexion.EjecutarConsulta(sql);
            DataRowCollection filas = datos.Rows;
            var campo = filas[0];
            int Id = int.Parse(campo[0].ToString());
            return Id;
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
