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

                            Conexion.EjecutarInsercionEnTransaccion(pMovimientoStock.SQL_ALTA,
                                pMovimientoStock.ParametrosAlta(unMovimiento), conexion, transaccion);

                            // El stock se descuenta con la resta hecha en la base y no con
                            // el valor que trae el objeto, para que dos altas simultaneas
                            // no se pisen el saldo.
                            Conexion.EjecutarComandoEnTransaccion(
                                "UPDATE insumos SET stock_actual = stock_actual - 1 WHERE id_insumo = @id_insumo",
                                new Dictionary<string, object?> { { "@id_insumo", pServicio.Pajuela.IdInsumo } },
                                conexion, transaccion);

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

        public const string SQL_MODIFICAR = "UPDATE servicios SET "
            + "fecha_probable_parto = @fecha_probable_parto,"
            + "observaciones = @observaciones "
            + "WHERE id_servicio = @id_servicio";

        public static Dictionary<string, object?> ParametrosModificar(Servicio pServicio)
        {
            return new Dictionary<string, object?>
            {
                { "@fecha_probable_parto", pServicio.FechaProbableParto.Date },
                { "@observaciones", pServicio.Observaciones },
                { "@id_servicio", pServicio.IdServicio }
            };
        }

        // La fecha probable de parto vive en dos lados: en el servicio y en la lactancia
        // en curso, que es de donde sale la fecha recomendada de secado. Cuando se
        // corrige, las dos escrituras van juntas o el ajuste no llega a las alertas.
        //
        // El parametro se llama pLactanciaVigente y no pLactancia porque ese es el
        // nombre de la clase de persistencia que se usa mas abajo.
        public bool ModificarServicio(Servicio pServicio, Lactancia pLactanciaVigente)
        {
            if (pLactanciaVigente == null)
            {
                return Conexion.EjecutarComando(SQL_MODIFICAR, ParametrosModificar(pServicio));
            }

            using (MySqlConnection conexion = Conexion.AbrirConexion())
            {
                using (MySqlTransaction transaccion = conexion.BeginTransaction())
                {
                    try
                    {
                        Conexion.EjecutarComandoEnTransaccion(SQL_MODIFICAR,
                            ParametrosModificar(pServicio), conexion, transaccion);

                        Conexion.EjecutarComandoEnTransaccion(pLactancia.SQL_MODIFICAR,
                            pLactancia.ParametrosModificar(pLactanciaVigente), conexion, transaccion);

                        transaccion.Commit();
                        return true;
                    }
                    catch (Exception e)
                    {
                        transaccion.Rollback();
                        throw new Exception("Error al modificar el servicio", e);
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
