using MySql.Data.MySqlClient;
using Tesis.Dominio;
using System.Data;

namespace Tesis.Persistencia
{
    public class pParto
    {
        private pConexion Conexion = new pConexion();

        public List<Parto> ListarPartos(List<Hembra> pListaHembras)
        {
            string sql = "SELECT * FROM partos ORDER BY fecha_parto DESC, id_parto DESC";
            DataTable datos = Conexion.EjecutarConsulta(sql);
            List<Parto> lista = new List<Parto>();

            foreach (DataRow fila in datos.Rows)
            {
                Parto unParto = new Parto(
                    int.Parse(fila["id_parto"].ToString()),
                    DateTime.Parse(fila["fecha_parto"].ToString()),
                    fila["tipo_parto"].ToString(),
                    fila["observaciones"] != DBNull.Value ? fila["observaciones"].ToString() : "", // Si es NULL, usa un string vacio
                    this.BuscarHembra(pListaHembras, int.Parse(fila["id_animal"].ToString()))
                    );
                lista.Add(unParto);
            }
            return lista;
        }

        // El parto es la operacion que mas tablas toca del sistema: asienta el parto,
        // da de alta la cria con su especializacion, abre la lactancia de la madre y
        // actualiza sus estados y su numero de partos. Todo va en una misma
        // transaccion: un parto a medias deja a la madre en lactancia sin lactancia
        // abierta, o una cria sin madre.
        // La madre llega aparte y no se toma de pParto.Madre: el dominio manda una
        // copia con el numero de partos y los estados nuevos, y recien actualiza el
        // objeto de la cache cuando la escritura salio bien.
        //
        // pLactanciaCerrada puede venir nula. Cuando no lo es, es la lactancia anterior
        // que quedo abierta porque no se registro el secado, y que este parto cierra.
        //
        // La lactancia nueva se llama pNuevaLactancia y no pLactancia porque este
        // ultimo es el nombre de la clase de persistencia que se usa mas abajo.
        public bool AltaParto(Parto pParto, List<Animal> pListaCrias, Lactancia pNuevaLactancia,
            Hembra pMadreActualizada, Lactancia pLactanciaCerrada)
        {
            using (MySqlConnection conexion = Conexion.AbrirConexion())
            {
                using (MySqlTransaction transaccion = conexion.BeginTransaction())
                {
                    try
                    {
                        int vIdParto = Conexion.EjecutarInsercionEnTransaccion(SQL_ALTA,
                            ParametrosAlta(pParto), conexion, transaccion);
                        pParto.IdParto = vIdParto;

                        // Las crias: primero animales y despues su especializacion, igual
                        // que en el alta comun. Son varias cuando el parto fue doble.
                        foreach (Animal unaCria in pListaCrias)
                        {
                            int vIdCria = Conexion.EjecutarInsercionEnTransaccion(pAnimal.SQL_ALTA,
                                pAnimal.ParametrosAlta(unaCria), conexion, transaccion);
                            unaCria.IdAnimal = vIdCria;

                            if (unaCria is Hembra)
                            {
                                Conexion.EjecutarInsercionEnTransaccion(pHembra.SQL_ALTA,
                                    pHembra.ParametrosAlta((Hembra)unaCria), conexion, transaccion);
                            }
                            else
                            {
                                Conexion.EjecutarInsercionEnTransaccion(pMacho.SQL_ALTA,
                                    pMacho.ParametrosAlta((Macho)unaCria), conexion, transaccion);
                            }
                        }

                        // La lactancia anterior que habia quedado abierta
                        if (pLactanciaCerrada != null)
                        {
                            Conexion.EjecutarComandoEnTransaccion(pLactancia.SQL_MODIFICAR,
                                pLactancia.ParametrosModificar(pLactanciaCerrada), conexion, transaccion);
                        }

                        // La lactancia que abre el parto
                        int vIdLactancia = Conexion.EjecutarInsercionEnTransaccion(pLactancia.SQL_ALTA,
                            pLactancia.ParametrosAlta(pNuevaLactancia), conexion, transaccion);
                        pNuevaLactancia.IdLactancia = vIdLactancia;

                        // La madre queda con un parto mas, en lactancia y vacia
                        Conexion.EjecutarComandoEnTransaccion(pHembra.SQL_MODIFICAR,
                            pHembra.ParametrosModificar(pMadreActualizada), conexion, transaccion);

                        transaccion.Commit();
                        return true;
                    }
                    catch (Exception e)
                    {
                        transaccion.Rollback();
                        // Los id que habia asignado la base ya no valen, el alta se deshizo
                        pParto.IdParto = 0;
                        pNuevaLactancia.IdLactancia = 0;

                        foreach (Animal unaCria in pListaCrias)
                        {
                            unaCria.IdAnimal = 0;
                        }

                        throw new Exception("Error al registrar el parto", e);
                    }
                }
            }
        }

        public const string SQL_ALTA = "INSERT INTO partos (fecha_parto, tipo_parto, observaciones, id_animal) " +
            "VALUES (@fecha_parto, @tipo_parto, @observaciones, @id_animal)";

        public static Dictionary<string, object?> ParametrosAlta(Parto pParto)
        {
            return new Dictionary<string, object?>
            {
                { "@fecha_parto", pParto.FechaParto.Date },
                { "@tipo_parto", pParto.TipoParto },
                { "@observaciones", pParto.Observaciones },
                { "@id_animal", pParto.Madre.IdAnimal }
            };
        }

        public const string SQL_MODIFICAR = "UPDATE partos SET "
            + "fecha_parto = @fecha_parto,"
            + "tipo_parto = @tipo_parto,"
            + "observaciones = @observaciones "
            + "WHERE id_parto = @id_parto";

        public static Dictionary<string, object?> ParametrosModificar(Parto pParto)
        {
            return new Dictionary<string, object?>
            {
                { "@fecha_parto", pParto.FechaParto.Date },
                { "@tipo_parto", pParto.TipoParto },
                { "@observaciones", pParto.Observaciones },
                { "@id_parto", pParto.IdParto }
            };
        }

        // La correccion del parto no cambia de madre: para eso esta el borrado. Si se
        // corrige la fecha, arrastra a las lactancias que el parto habia movido -la que
        // abrio empieza ese dia y la anterior quedo secada ese dia- y a las crias, que
        // nacieron ese dia. Las dos listas pueden venir vacias cuando solo se corrigio
        // el tipo o las observaciones.
        public bool ModificarParto(Parto pParto, List<Lactancia> pLactanciasActualizadas,
            List<Animal> pListaCrias)
        {
            using (MySqlConnection conexion = Conexion.AbrirConexion())
            {
                using (MySqlTransaction transaccion = conexion.BeginTransaction())
                {
                    try
                    {
                        Conexion.EjecutarComandoEnTransaccion(SQL_MODIFICAR,
                            ParametrosModificar(pParto), conexion, transaccion);

                        if (pLactanciasActualizadas != null)
                        {
                            foreach (Lactancia unaLactancia in pLactanciasActualizadas)
                            {
                                Conexion.EjecutarComandoEnTransaccion(pLactancia.SQL_MODIFICAR,
                                    pLactancia.ParametrosModificar(unaLactancia), conexion, transaccion);
                            }
                        }

                        if (pListaCrias != null)
                        {
                            foreach (Animal unaCria in pListaCrias)
                            {
                                Conexion.EjecutarComandoEnTransaccion(pAnimal.SQL_MODIFICAR,
                                    pAnimal.ParametrosModificar(unaCria), conexion, transaccion);
                            }
                        }

                        transaccion.Commit();
                        return true;
                    }
                    catch (Exception e)
                    {
                        transaccion.Rollback();
                        throw new Exception("Error al modificar el parto", e);
                    }
                }
            }
        }

        // Deshacer un parto es deshacer todo lo que el alta habia dejado, en el orden
        // inverso: se borran las crias y la lactancia que abrio, se reabre la que el
        // parto habia cerrado y la madre vuelve a tener un parto menos, con el estado
        // y la categoria que le corresponden. El dominio ya verifico que no haya
        // ordenies imputados a esa lactancia ni registros colgando de las crias: si
        // los hubiera, el borrado no llega hasta aca.
        //
        // Las crias se borran de animales y no de hembras ni de machos: la
        // especializacion cae sola por el ON DELETE CASCADE del esquema.
        public bool EliminarParto(int pIdParto, List<Animal> pListaCrias, Lactancia pLactanciaDelParto,
            Hembra pMadreActualizada, Lactancia pLactanciaReabierta)
        {
            using (MySqlConnection conexion = Conexion.AbrirConexion())
            {
                using (MySqlTransaction transaccion = conexion.BeginTransaction())
                {
                    try
                    {
                        Conexion.EjecutarComandoEnTransaccion(
                            "DELETE FROM partos WHERE id_parto = @id_parto",
                            new Dictionary<string, object?> { { "@id_parto", pIdParto } },
                            conexion, transaccion);

                        if (pLactanciaDelParto != null)
                        {
                            Conexion.EjecutarComandoEnTransaccion(
                                "DELETE FROM lactancias WHERE id_lactancia = @id_lactancia",
                                new Dictionary<string, object?> { { "@id_lactancia", pLactanciaDelParto.IdLactancia } },
                                conexion, transaccion);
                        }

                        if (pListaCrias != null)
                        {
                            foreach (Animal unaCria in pListaCrias)
                            {
                                Conexion.EjecutarComandoEnTransaccion(
                                    "DELETE FROM animales WHERE id_animal = @id_animal",
                                    new Dictionary<string, object?> { { "@id_animal", unaCria.IdAnimal } },
                                    conexion, transaccion);
                            }
                        }

                        // La lactancia anterior que el parto habia cerrado vuelve a
                        // quedar abierta: si no, la vaca queda sin ninguna en curso y
                        // los ordenies del dia no se pueden imputar a ningun lado.
                        if (pLactanciaReabierta != null)
                        {
                            Conexion.EjecutarComandoEnTransaccion(pLactancia.SQL_MODIFICAR,
                                pLactancia.ParametrosModificar(pLactanciaReabierta), conexion, transaccion);
                        }

                        if (pMadreActualizada != null)
                        {
                            Conexion.EjecutarComandoEnTransaccion(pHembra.SQL_MODIFICAR,
                                pHembra.ParametrosModificar(pMadreActualizada), conexion, transaccion);

                            // La categoria vive en animales: una vaca que se queda sin
                            // partos vuelve a ser novilla.
                            Conexion.EjecutarComandoEnTransaccion(pAnimal.SQL_MODIFICAR,
                                pAnimal.ParametrosModificar(pMadreActualizada), conexion, transaccion);
                        }

                        transaccion.Commit();
                        return true;
                    }
                    catch (Exception e)
                    {
                        transaccion.Rollback();
                        throw new Exception("Error al eliminar el parto", e);
                    }
                }
            }
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
