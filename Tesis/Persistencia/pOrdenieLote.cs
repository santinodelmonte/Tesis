using MySql.Data.MySqlClient;
using Tesis.Dominio;
using System.Data;

namespace Tesis.Persistencia
{
    public class pOrdenieLote
    {
        private pConexion Conexion = new pConexion();

        // Ademas de la cabecera se traen los animales que integraron cada lote, que
        // viven en ordenie_lote_animales. Se resuelve con una sola consulta adicional
        // en lugar de una por lote.
        public List<OrdenieLote> ListarOrdeniesLote(List<Hembra> pListaHembras)
        {
            string sql = "SELECT * FROM ordenies_lote ORDER BY fecha DESC, turno DESC";
            DataTable datos = Conexion.EjecutarConsulta(sql);
            List<OrdenieLote> lista = new List<OrdenieLote>();

            foreach (DataRow fila in datos.Rows)
            {
                OrdenieLote unOrdenie = new OrdenieLote(
                    int.Parse(fila["id_ordenie_lote"].ToString()),
                    DateTime.Parse(fila["fecha"].ToString()),
                    fila["turno"].ToString(),
                    Convert.ToDouble(fila["litros_totales"]),
                    new List<Hembra>()
                    );
                lista.Add(unOrdenie);
            }

            DataTable detalle = Conexion.EjecutarConsulta("SELECT * FROM ordenie_lote_animales");
            foreach (DataRow fila in detalle.Rows)
            {
                OrdenieLote unOrdenie = this.BuscarEnLista(lista, int.Parse(fila["id_ordenie_lote"].ToString()));
                Hembra unaHembra = this.BuscarHembra(pListaHembras, int.Parse(fila["id_animal"].ToString()));

                if (unOrdenie != null && unaHembra != null)
                {
                    unOrdenie.Animales.Add(unaHembra);
                }
            }

            return lista;
        }

        // La cabecera y el detalle de animales van en una misma transaccion: un lote
        // sin sus animales no deja constancia de que vacas se ordeniaron, que es
        // justamente lo que pide el paso 4 de CU12.
        public bool AltaOrdenieLote(OrdenieLote pOrdenieLote)
        {
            string sql = "INSERT INTO ordenies_lote (fecha, turno, litros_totales) " +
                "VALUES (@fecha, @turno, @litros_totales)";

            Dictionary<string, object?> parametros = new Dictionary<string, object?>
            {
                { "@fecha", pOrdenieLote.Fecha.Date },
                { "@turno", pOrdenieLote.Turno },
                { "@litros_totales", pOrdenieLote.LitrosTotales }
            };

            using (MySqlConnection conexion = Conexion.AbrirConexion())
            {
                using (MySqlTransaction transaccion = conexion.BeginTransaction())
                {
                    try
                    {
                        int vIdNuevo = Conexion.EjecutarInsercionEnTransaccion(sql, parametros, conexion, transaccion);
                        pOrdenieLote.IdOrdenieLote = vIdNuevo;

                        foreach (Hembra unaHembra in pOrdenieLote.Animales)
                        {
                            Conexion.EjecutarComandoEnTransaccion(
                                "INSERT INTO ordenie_lote_animales (id_ordenie_lote, id_animal) " +
                                "VALUES (@id_ordenie_lote, @id_animal)",
                                new Dictionary<string, object?>
                                {
                                    { "@id_ordenie_lote", vIdNuevo },
                                    { "@id_animal", unaHembra.IdAnimal }
                                },
                                conexion, transaccion);
                        }

                        transaccion.Commit();
                        return true;
                    }
                    catch (Exception e)
                    {
                        transaccion.Rollback();
                        // El id que habia asignado la base ya no vale, el alta se deshizo
                        pOrdenieLote.IdOrdenieLote = 0;
                        throw new Exception("Error al registrar el ordeñe del lote", e);
                    }
                }
            }
        }

        // La correccion reescribe los litros y rehace el detalle de animales. El detalle
        // se borra y se vuelve a insertar porque la tabla no tiene mas que las dos
        // claves: no hay nada que actualizar fila por fila.
        public bool ModificarOrdenieLote(OrdenieLote pOrdenieLote)
        {
            string sql = "UPDATE ordenies_lote SET litros_totales = @litros_totales " +
                "WHERE id_ordenie_lote = @id_ordenie_lote";

            Dictionary<string, object?> parametros = new Dictionary<string, object?>
            {
                { "@litros_totales", pOrdenieLote.LitrosTotales },
                { "@id_ordenie_lote", pOrdenieLote.IdOrdenieLote }
            };

            using (MySqlConnection conexion = Conexion.AbrirConexion())
            {
                using (MySqlTransaction transaccion = conexion.BeginTransaction())
                {
                    try
                    {
                        Conexion.EjecutarComandoEnTransaccion(sql, parametros, conexion, transaccion);

                        Conexion.EjecutarComandoEnTransaccion(
                            "DELETE FROM ordenie_lote_animales WHERE id_ordenie_lote = @id_ordenie_lote",
                            new Dictionary<string, object?> { { "@id_ordenie_lote", pOrdenieLote.IdOrdenieLote } },
                            conexion, transaccion);

                        foreach (Hembra unaHembra in pOrdenieLote.Animales)
                        {
                            Conexion.EjecutarComandoEnTransaccion(
                                "INSERT INTO ordenie_lote_animales (id_ordenie_lote, id_animal) " +
                                "VALUES (@id_ordenie_lote, @id_animal)",
                                new Dictionary<string, object?>
                                {
                                    { "@id_ordenie_lote", pOrdenieLote.IdOrdenieLote },
                                    { "@id_animal", unaHembra.IdAnimal }
                                },
                                conexion, transaccion);
                        }

                        transaccion.Commit();
                        return true;
                    }
                    catch (Exception e)
                    {
                        transaccion.Rollback();
                        throw new Exception("Error al modificar el ordeñe del lote", e);
                    }
                }
            }
        }

        // La baja del ordenie del turno. Los controles individuales de ese turno NO se
        // borran: son mediciones de cada vaca, validas por si solas, y lo unico que
        // pierden es el enganche con el total del tanque. Por eso la foranea se pone en
        // nulo -que es el mismo estado en el que nace un control cargado antes que el
        // lote- en lugar de arrastrarlos.
        //
        // Las tres escrituras van juntas: un lote borrado a medias dejaria controles
        // apuntando a un turno que ya no existe.
        public bool EliminarOrdenieLote(int pIdOrdenieLote)
        {
            Dictionary<string, object?> parametros = new Dictionary<string, object?>
            {
                { "@id_ordenie_lote", pIdOrdenieLote }
            };

            using (MySqlConnection conexion = Conexion.AbrirConexion())
            {
                using (MySqlTransaction transaccion = conexion.BeginTransaction())
                {
                    try
                    {
                        Conexion.EjecutarComandoEnTransaccion(
                            "UPDATE ordenies_individual SET id_ordenie_lote = NULL " +
                            "WHERE id_ordenie_lote = @id_ordenie_lote",
                            parametros, conexion, transaccion);

                        Conexion.EjecutarComandoEnTransaccion(
                            "DELETE FROM ordenie_lote_animales WHERE id_ordenie_lote = @id_ordenie_lote",
                            parametros, conexion, transaccion);

                        Conexion.EjecutarComandoEnTransaccion(
                            "DELETE FROM ordenies_lote WHERE id_ordenie_lote = @id_ordenie_lote",
                            parametros, conexion, transaccion);

                        transaccion.Commit();
                        return true;
                    }
                    catch (Exception e)
                    {
                        transaccion.Rollback();
                        throw new Exception("Error al eliminar el ordeñe del lote", e);
                    }
                }
            }
        }

        private OrdenieLote BuscarEnLista(List<OrdenieLote> pLista, int pIdOrdenieLote)
        {
            foreach (OrdenieLote unOrdenie in pLista)
            {
                if (unOrdenie.IdOrdenieLote == pIdOrdenieLote)
                {
                    return unOrdenie;
                }
            }
            return null;
        }

        private Hembra BuscarHembra(List<Hembra> pLista, int pIdAnimal)
        {
            foreach (Hembra unaHembra in pLista)
            {
                if (unaHembra.IdAnimal == pIdAnimal)
                {
                    return unaHembra;
                }
            }
            return null;
        }
    }
}
