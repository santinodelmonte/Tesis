using MySql.Data.MySqlClient;
using Tesis.Dominio;
using System.Data;

namespace Tesis.Persistencia
{
    public class pAnimal
    {
        private pConexion Conexion = new pConexion();

        // Las razas y las categorias llegan ya listadas por parametro. Antes esta
        // clase instanciaba una Controladora para resolverlas, con lo cual la capa
        // de persistencia terminaba llamando a la de dominio.
        public List<Animal> ListarAnimales(List<Raza> pListaRazas, List<Categoria> pListaCategorias)
        {
            // Se traen los id de hembras y machos para saber por que tabla esta
            // especializado cada animal, en lugar de deducirlo de un campo en NULL.
            string sql = "SELECT a.id_animal, a.num_caravana, a.fecha_nacimiento, a.activo, a.fecha_baja, " +
                "a.motivo_baja, a.foto, a.id_raza, a.id_categoria, a.id_madre, a.id_padre, " +
                "h.id_animal AS id_hembra, m.id_animal AS id_macho, " +
                "h.numero_partos, h.estado_productivo, h.estado_reproductivo, m.en_pie " +
                "FROM animales a " +
                "LEFT JOIN hembras h ON h.id_animal = a.id_animal " +
                "LEFT JOIN machos m ON m.id_animal = a.id_animal " +
                "ORDER BY a.id_animal";

            DataTable datos = Conexion.EjecutarConsulta(sql);
            List<Animal> lista = new List<Animal>();

            // Primera pasada: se arman las hembras y los machos sin resolver la ascendencia
            foreach (DataRow fila in datos.Rows)
            {
                Animal unAnimal;

                if (fila["id_hembra"] != DBNull.Value)
                {
                    unAnimal = new Hembra(
                        int.Parse(fila["id_animal"].ToString()),
                        fila["num_caravana"].ToString(),
                        DateTime.Parse(fila["fecha_nacimiento"].ToString()),
                        Convert.ToBoolean(fila["activo"]), // MySQL devuelve el tinyint(1) como bool
                        fila["fecha_baja"] != DBNull.Value ? DateTime.Parse(fila["fecha_baja"].ToString()) : DateTime.MinValue, // Si es NULL, usa la fecha minima
                        fila["motivo_baja"] != DBNull.Value ? fila["motivo_baja"].ToString() : "", // Si es NULL, usa un string vacio
                        this.BuscarRaza(pListaRazas, int.Parse(fila["id_raza"].ToString())),
                        this.BuscarCategoria(pListaCategorias, int.Parse(fila["id_categoria"].ToString())),
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
                        this.BuscarRaza(pListaRazas, int.Parse(fila["id_raza"].ToString())),
                        this.BuscarCategoria(pListaCategorias, int.Parse(fila["id_categoria"].ToString())),
                        null,
                        null,
                        fila["en_pie"] != DBNull.Value && Convert.ToBoolean(fila["en_pie"])); // Si es NULL, usa false
                }

                // La foto no va en el constructor: es opcional y se completa aparte
                unAnimal.Foto = fila["foto"] != DBNull.Value ? fila["foto"].ToString() : "";

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

        // El id_animal no se manda: lo asigna MySQL con el AUTO_INCREMENT.
        // El alta la usan dos caminos: esta clase y el registro del parto, que da de
        // alta la cria dentro de su propia transaccion. Por eso el comando y sus
        // parametros se declaran una sola vez aca.
        public const string SQL_ALTA = "INSERT INTO animales (num_caravana, fecha_nacimiento, activo, fecha_baja, " +
            "motivo_baja, foto, id_raza, id_categoria, id_madre, id_padre) " +
            "VALUES (@num_caravana, @fecha_nacimiento, @activo, NULL, NULL, @foto, " +
            "@id_raza, @id_categoria, @id_madre, @id_padre)";

        public static Dictionary<string, object?> ParametrosAlta(Animal pAnimal)
        {
            return new Dictionary<string, object?>
            {
                { "@num_caravana", pAnimal.NumCaravana },
                { "@fecha_nacimiento", pAnimal.FechaNacimiento.Date },
                { "@activo", pAnimal.Activo ? 1 : 0 },
                // El animal sin foto guarda NULL, no la cadena vacia
                { "@foto", pAnimal.TieneFoto ? (object)pAnimal.Foto : null },
                { "@id_raza", pAnimal.Raza.IdRaza },
                { "@id_categoria", pAnimal.Categoria.IdCategoria },
                { "@id_madre", pAnimal.Madre != null ? (object)pAnimal.Madre.IdAnimal : null },
                { "@id_padre", pAnimal.Padre != null ? (object)pAnimal.Padre.IdAnimal : null }
            };
        }

        public bool AltaAnimal(Animal pAnimal)
        {
            string sql = SQL_ALTA;
            Dictionary<string, object?> parametros = ParametrosAlta(pAnimal);

            // El animal y su especializacion se guardan dentro de una misma transaccion.
            // Si falla la segunda insercion se deshace la primera: de lo contrario
            // quedaria una fila en animales sin fila en hembras ni en machos, y
            // ListarAnimales no podria saber de que sexo es.
            using (MySqlConnection conexion = Conexion.AbrirConexion())
            {
                using (MySqlTransaction transaccion = conexion.BeginTransaction())
                {
                    try
                    {
                        int vIdNuevo = Conexion.EjecutarInsercionEnTransaccion(sql, parametros, conexion, transaccion);
                        pAnimal.IdAnimal = vIdNuevo;

                        if (pAnimal is Hembra)
                        {
                            Hembra unaHembra = (Hembra)pAnimal;
                            Conexion.EjecutarInsercionEnTransaccion(pHembra.SQL_ALTA,
                                pHembra.ParametrosAlta(unaHembra), conexion, transaccion);
                        }
                        else
                        {
                            Macho unMacho = (Macho)pAnimal;
                            Conexion.EjecutarInsercionEnTransaccion(pMacho.SQL_ALTA,
                                pMacho.ParametrosAlta(unMacho), conexion, transaccion);
                        }

                        transaccion.Commit();
                        return true;
                    }
                    catch (Exception e)
                    {
                        transaccion.Rollback();
                        // El id que habia asignado la base ya no vale, el alta se deshizo
                        pAnimal.IdAnimal = 0;
                        throw new Exception("Error al dar de alta el animal", e);
                    }
                }
            }
        }

        public bool ModificarAnimal(Animal pAnimal)
        {
            string sql = "UPDATE animales SET "
                + "num_caravana = @num_caravana,"
                + "fecha_nacimiento = @fecha_nacimiento,"
                + "foto = @foto,"
                + "id_raza = @id_raza,"
                + "id_categoria = @id_categoria,"
                + "id_madre = @id_madre,"
                + "id_padre = @id_padre "
                + "WHERE id_animal = @id_animal";

            Dictionary<string, object?> parametros = new Dictionary<string, object?>
            {
                { "@num_caravana", pAnimal.NumCaravana },
                { "@fecha_nacimiento", pAnimal.FechaNacimiento.Date },
                { "@foto", pAnimal.TieneFoto ? (object)pAnimal.Foto : null },
                { "@id_raza", pAnimal.Raza.IdRaza },
                { "@id_categoria", pAnimal.Categoria.IdCategoria },
                { "@id_madre", pAnimal.Madre != null ? (object)pAnimal.Madre.IdAnimal : null },
                { "@id_padre", pAnimal.Padre != null ? (object)pAnimal.Padre.IdAnimal : null },
                { "@id_animal", pAnimal.IdAnimal }
            };

            // Como el alta, la modificacion toca dos tablas: la del animal y la de su
            // especializacion. Van juntas en una transaccion para que no quede el
            // numero de partos viejo con el resto de los datos nuevos.
            using (MySqlConnection conexion = Conexion.AbrirConexion())
            {
                using (MySqlTransaction transaccion = conexion.BeginTransaction())
                {
                    try
                    {
                        Conexion.EjecutarComandoEnTransaccion(sql, parametros, conexion, transaccion);

                        if (pAnimal is Hembra)
                        {
                            Hembra unaHembra = (Hembra)pAnimal;
                            Conexion.EjecutarComandoEnTransaccion(pHembra.SQL_MODIFICAR,
                                pHembra.ParametrosModificar(unaHembra), conexion, transaccion);
                        }
                        else
                        {
                            Macho unMacho = (Macho)pAnimal;
                            Conexion.EjecutarComandoEnTransaccion(pMacho.SQL_MODIFICAR,
                                pMacho.ParametrosModificar(unMacho), conexion, transaccion);
                        }

                        transaccion.Commit();
                        return true;
                    }
                    catch (Exception e)
                    {
                        transaccion.Rollback();
                        throw new Exception("Error al modificar el animal", e);
                    }
                }
            }
        }

        public bool BajaAnimal(int pIdAnimal, string pMotivoBaja)
        {
            string sql = "UPDATE animales SET "
                + "activo = 0,"
                + "fecha_baja = @fecha_baja,"
                + "motivo_baja = @motivo_baja "
                + "WHERE id_animal = @id_animal";

            Dictionary<string, object?> parametros = new Dictionary<string, object?>
            {
                { "@fecha_baja", DateTime.Now.Date },
                { "@motivo_baja", pMotivoBaja },
                { "@id_animal", pIdAnimal }
            };

            return Conexion.EjecutarComando(sql, parametros);
        }

        // Deshace la baja logica: el animal vuelve al rodeo y se limpian la fecha y el
        // motivo, que son los que dan por terminada su vida productiva.
        public bool ReactivarAnimal(int pIdAnimal)
        {
            string sql = "UPDATE animales SET "
                + "activo = 1,"
                + "fecha_baja = NULL,"
                + "motivo_baja = NULL "
                + "WHERE id_animal = @id_animal";

            Dictionary<string, object?> parametros = new Dictionary<string, object?>
            {
                { "@id_animal", pIdAnimal }
            };

            return Conexion.EjecutarComando(sql, parametros);
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

        private Raza BuscarRaza(List<Raza> pLista, int pIdRaza)
        {
            foreach (Raza unaRaza in pLista)
            {
                if (unaRaza.IdRaza == pIdRaza)
                {
                    return unaRaza;
                }
            }
            return null;
        }

        private Categoria BuscarCategoria(List<Categoria> pLista, int pIdCategoria)
        {
            foreach (Categoria unaCategoria in pLista)
            {
                if (unaCategoria.IdCategoria == pIdCategoria)
                {
                    return unaCategoria;
                }
            }
            return null;
        }
    }
}
