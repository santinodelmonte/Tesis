using MySql.Data.MySqlClient;
using Tesis.Dominio;
using System.Data;

namespace Tesis.Persistencia
{
    public class pServicio
    {
        private pConexion Conexion = new pConexion();

        // Hembras, machos e insumos llegan ya armados por parametro. El servicio
        // referencia a uno de los dos reproductores segun el tipo.
        public List<Servicio> ListarServicios(List<Hembra> pListaHembras, List<Macho> pListaMachos,
            List<Insumo> pListaInsumos)
        {
            string sql = "SELECT * FROM servicios ORDER BY fecha_servicio DESC, id_servicio DESC";
            DataTable datos = Conexion.EjecutarConsulta(sql);
            List<Servicio> lista = new List<Servicio>();

            foreach (DataRow fila in datos.Rows)
            {
                Servicio unServicio = new Servicio(
                    int.Parse(fila["id_servicio"].ToString()),
                    fila["tipo_servicio"].ToString(),
                    DateTime.Parse(fila["fecha_servicio"].ToString()),
                    DateTime.Parse(fila["fecha_probable_parto"].ToString()),
                    fila["observaciones"] != DBNull.Value ? fila["observaciones"].ToString() : "", // Si es NULL, usa un string vacio
                    this.BuscarHembra(pListaHembras, int.Parse(fila["id_animal"].ToString())),
                    fila["id_macho"] != DBNull.Value ? this.BuscarMacho(pListaMachos, int.Parse(fila["id_macho"].ToString())) : null, // Solo en la monta natural
                    fila["id_insumo"] != DBNull.Value ? this.BuscarInsumo(pListaInsumos, int.Parse(fila["id_insumo"].ToString())) : null // Solo en la inseminacion artificial
                    );
                lista.Add(unServicio);
            }
            return lista;
        }

        // El alta guarda el servicio y deja servida a la hembra. Si ademas fue una
        // inseminacion artificial, asienta el egreso de la pajuela y baja el stock del
        // insumo. Todo va en una misma transaccion para que no quede un servicio
        // registrado con una pajuela que nunca se descontó, ni al reves.
        //
        // El parametro se llama pHembraServida y no pHembra porque ese es el nombre de
        // la clase de persistencia que se usa mas abajo.
        public bool AltaServicio(Servicio pServicio, Hembra pHembraServida)
        {
            string sql = "INSERT INTO servicios (tipo_servicio, fecha_servicio, fecha_probable_parto, " +
                "observaciones, id_animal, id_macho, id_insumo) " +
                "VALUES (@tipo_servicio, @fecha_servicio, @fecha_probable_parto, " +
                "@observaciones, @id_animal, @id_macho, @id_insumo)";

            Dictionary<string, object?> parametros = new Dictionary<string, object?>
            {
                { "@tipo_servicio", pServicio.TipoServicio },
                { "@fecha_servicio", pServicio.FechaServicio.Date },
                { "@fecha_probable_parto", pServicio.FechaProbableParto.Date },
                { "@observaciones", pServicio.Observaciones },
                { "@id_animal", pServicio.Animal.IdAnimal },
                { "@id_macho", pServicio.Toro != null ? (object)pServicio.Toro.IdAnimal : null },
                { "@id_insumo", pServicio.Pajuela != null ? (object)pServicio.Pajuela.IdInsumo : null }
            };

            using (MySqlConnection conexion = Conexion.AbrirConexion())
            {
                using (MySqlTransaction transaccion = conexion.BeginTransaction())
                {
                    try
                    {
                        int vIdNuevo = Conexion.EjecutarInsercionEnTransaccion(sql, parametros, conexion, transaccion);
                        pServicio.IdServicio = vIdNuevo;

                        // La hembra llega con el estado reproductivo nuevo ya puesto
                        // desde el dominio
                        Conexion.EjecutarComandoEnTransaccion(pHembra.SQL_MODIFICAR,
                            pHembra.ParametrosModificar(pHembraServida), conexion, transaccion);

                        if (pServicio.Pajuela != null)
                        {
                            MovimientoStock unMovimiento = new MovimientoStock(0, MovimientoStock.EGRESO, 1,
                                pServicio.FechaServicio, DateTime.MinValue,
                                "Inseminación de la caravana " + pServicio.Animal.NumCaravana, pServicio.Pajuela);

                            pMovimientoStock.AsentarEnTransaccion(Conexion, unMovimiento, conexion, transaccion);

                            pServicio.Pajuela.StockActual = pServicio.Pajuela.StockActual - 1;
                        }

                        transaccion.Commit();
                        return true;
                    }
                    catch (Exception e)
                    {
                        transaccion.Rollback();
                        // El id que habia asignado la base ya no vale, el alta se deshizo
                        pServicio.IdServicio = 0;
                        throw new Exception("Error al registrar el servicio", e);
                    }
                }
            }
        }

        // La correccion escribe el servicio entero y no solo la fecha probable de
        // parto: al servicio mal cargado se le puede haber errado la hembra, el
        // reproductor o el tipo, y esos son justamente los campos que no se podian
        // arreglar sin borrar y volver a cargar.
        public const string SQL_MODIFICAR = "UPDATE servicios SET "
            + "tipo_servicio = @tipo_servicio,"
            + "fecha_servicio = @fecha_servicio,"
            + "fecha_probable_parto = @fecha_probable_parto,"
            + "observaciones = @observaciones,"
            + "id_animal = @id_animal,"
            + "id_macho = @id_macho,"
            + "id_insumo = @id_insumo "
            + "WHERE id_servicio = @id_servicio";

        public static Dictionary<string, object?> ParametrosModificar(Servicio pServicio)
        {
            return new Dictionary<string, object?>
            {
                { "@tipo_servicio", pServicio.TipoServicio },
                { "@fecha_servicio", pServicio.FechaServicio.Date },
                { "@fecha_probable_parto", pServicio.FechaProbableParto.Date },
                { "@observaciones", pServicio.Observaciones },
                { "@id_animal", pServicio.Animal.IdAnimal },
                { "@id_macho", pServicio.Toro != null ? (object)pServicio.Toro.IdAnimal : null },
                { "@id_insumo", pServicio.Pajuela != null ? (object)pServicio.Pajuela.IdInsumo : null },
                { "@id_servicio", pServicio.IdServicio }
            };
        }

        // La correccion de un servicio arrastra hasta tres cosas mas. La fecha probable
        // de parto vive tambien en la lactancia en curso, que es de donde sale la fecha
        // recomendada de secado; el estado reproductivo de la hembra puede cambiar si se
        // corrigio de que animal era el servicio; y si se cambio la pajuela hay que
        // devolver la que no se uso y descontar la que si. Va todo en una transaccion:
        // un servicio corregido a medias deja una vaca servida que no lo esta, o una
        // pajuela descontada dos veces.
        //
        // Las hembras y las lactancias llegan en lista y no de a una porque la
        // correccion puede haber cambiado de animal: en ese caso la que estaba mal deja
        // de figurar servida y la nueva pasa a estarlo, y las dos escrituras tienen que
        // ir juntas. Las tres listas pueden llegar vacias cuando la correccion no toca
        // nada de eso, que es lo mas comun.
        public bool ModificarServicio(Servicio pServicio, List<Lactancia> pLactanciasActualizadas,
            List<Hembra> pHembrasActualizadas, List<MovimientoStock> pMovimientos)
        {
            return this.Escribir(SQL_MODIFICAR, ParametrosModificar(pServicio),
                pLactanciasActualizadas, pHembrasActualizadas, pMovimientos,
                "Error al modificar el servicio");
        }

        // El borrado del servicio deja a la hembra con el estado que le corresponde
        // segun los servicios que le quedan -eso lo deduce el dominio- y devuelve al
        // stock la pajuela que nunca se uso. El servicio no se elimina si tiene tactos
        // colgados: eso lo controla el dominio antes de llegar aca.
        public bool EliminarServicio(int pIdServicio, List<Lactancia> pLactanciasActualizadas,
            List<Hembra> pHembrasActualizadas, List<MovimientoStock> pMovimientos)
        {
            return this.Escribir("DELETE FROM servicios WHERE id_servicio = @id_servicio",
                new Dictionary<string, object?> { { "@id_servicio", pIdServicio } },
                pLactanciasActualizadas, pHembrasActualizadas, pMovimientos,
                "Error al eliminar el servicio");
        }

        // La correccion y el borrado arrastran lo mismo -las lactancias en curso, las
        // hembras que cambian de estado y los movimientos de stock-, asi que comparten
        // la transaccion y solo cambia el comando de adelante.
        private bool Escribir(string pSql, Dictionary<string, object?> pParametros,
            List<Lactancia> pLactanciasActualizadas, List<Hembra> pHembrasActualizadas,
            List<MovimientoStock> pMovimientos, string pMensajeError)
        {
            using (MySqlConnection conexion = Conexion.AbrirConexion())
            {
                using (MySqlTransaction transaccion = conexion.BeginTransaction())
                {
                    try
                    {
                        Conexion.EjecutarComandoEnTransaccion(pSql, pParametros, conexion, transaccion);

                        if (pLactanciasActualizadas != null)
                        {
                            foreach (Lactancia unaLactancia in pLactanciasActualizadas)
                            {
                                Conexion.EjecutarComandoEnTransaccion(pLactancia.SQL_MODIFICAR,
                                    pLactancia.ParametrosModificar(unaLactancia), conexion, transaccion);
                            }
                        }

                        if (pHembrasActualizadas != null)
                        {
                            foreach (Hembra unaHembra in pHembrasActualizadas)
                            {
                                Conexion.EjecutarComandoEnTransaccion(pHembra.SQL_MODIFICAR,
                                    pHembra.ParametrosModificar(unaHembra), conexion, transaccion);
                            }
                        }

                        pMovimientoStock.AsentarEnTransaccion(Conexion, pMovimientos, conexion, transaccion);

                        transaccion.Commit();
                        return true;
                    }
                    catch (Exception e)
                    {
                        transaccion.Rollback();
                        throw new Exception(pMensajeError, e);
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

        private Macho BuscarMacho(List<Macho> pLista, int pIdAnimal)
        {
            foreach (Macho unMacho in pLista)
            {
                if (unMacho.IdAnimal == pIdAnimal)
                {
                    return unMacho;
                }
            }
            return null;
        }

        private Insumo BuscarInsumo(List<Insumo> pLista, int pIdInsumo)
        {
            foreach (Insumo unInsumo in pLista)
            {
                if (unInsumo.IdInsumo == pIdInsumo)
                {
                    return unInsumo;
                }
            }
            return null;
        }
    }
}
