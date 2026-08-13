using MySql.Data.MySqlClient;
using Tesis.Dominio;
using System.Data;

namespace Tesis.Persistencia
{
    public class pVacunacion
    {
        private pConexion Conexion = new pConexion();

        public List<Vacunacion> ListarVacunaciones(List<Animal> pListaAnimales,
            List<Insumo> pListaInsumos, List<PlanSanitario> pListaPlanes)
        {
            string sql = "SELECT * FROM vacunaciones ORDER BY fecha_aplicacion DESC, id_vacunacion DESC";
            DataTable datos = Conexion.EjecutarConsulta(sql);
            List<Vacunacion> lista = new List<Vacunacion>();

            foreach (DataRow fila in datos.Rows)
            {
                Vacunacion unaVacunacion = new Vacunacion(
                    int.Parse(fila["id_vacunacion"].ToString()),
                    DateTime.Parse(fila["fecha_aplicacion"].ToString()),
                    this.BuscarAnimal(pListaAnimales, int.Parse(fila["id_animal"].ToString())),
                    this.BuscarInsumo(pListaInsumos, int.Parse(fila["id_insumo"].ToString())),
                    fila["id_plan"] != DBNull.Value
                        ? this.BuscarPlan(pListaPlanes, int.Parse(fila["id_plan"].ToString()))
                        : null // El nulo es la vacunacion aplicada fuera de todo plan
                    );
                lista.Add(unaVacunacion);
            }
            return lista;
        }

        // El alta guarda la vacunacion, descuenta la vacuna del stock y deja su
        // movimiento de egreso. Las tres escrituras van en una misma transaccion: un
        // descuento sin registro, o al reves, deja el inventario mintiendo.
        public bool AltaVacunacion(Vacunacion pVacunacion, double pCantidadInsumo)
        {
            string sql = "INSERT INTO vacunaciones (fecha_aplicacion, id_animal, id_insumo, id_plan) " +
                "VALUES (@fecha_aplicacion, @id_animal, @id_insumo, @id_plan)";

            Dictionary<string, object?> parametros = new Dictionary<string, object?>
            {
                { "@fecha_aplicacion", pVacunacion.FechaAplicacion.Date },
                { "@id_animal", pVacunacion.Animal.IdAnimal },
                { "@id_insumo", pVacunacion.Insumo.IdInsumo },
                { "@id_plan", pVacunacion.Plan != null ? (object)pVacunacion.Plan.IdPlan : null }
            };

            using (MySqlConnection conexion = Conexion.AbrirConexion())
            {
                using (MySqlTransaction transaccion = conexion.BeginTransaction())
                {
                    try
                    {
                        int vIdNuevo = Conexion.EjecutarInsercionEnTransaccion(sql, parametros,
                            conexion, transaccion);
                        pVacunacion.IdVacunacion = vIdNuevo;

                        if (pCantidadInsumo > 0)
                        {
                            MovimientoStock unMovimiento = new MovimientoStock(0, MovimientoStock.EGRESO,
                                pCantidadInsumo, pVacunacion.FechaAplicacion, DateTime.MinValue,
                                "Vacunacion", pVacunacion.Insumo);

                            pMovimientoStock.AsentarEnTransaccion(Conexion, unMovimiento, conexion, transaccion);

                            pVacunacion.Insumo.StockActual = pVacunacion.Insumo.StockActual - pCantidadInsumo;
                        }

                        transaccion.Commit();
                        return true;
                    }
                    catch (Exception e)
                    {
                        transaccion.Rollback();
                        // El id que habia asignado la base ya no vale, el alta se deshizo
                        pVacunacion.IdVacunacion = 0;
                        throw new Exception("Error al registrar la vacunacion", e);
                    }
                }
            }
        }

        // Corregir la vacunacion puede cambiar la vacuna aplicada: en ese caso los
        // movimientos traen la devolucion de la dosis que no se uso y el egreso de la
        // que si. Cuando solo se corrigio la fecha, el animal o el plan, la lista llega
        // vacia y el stock no se toca.
        public bool ModificarVacunacion(Vacunacion pVacunacion, List<MovimientoStock> pMovimientos)
        {
            string sql = "UPDATE vacunaciones SET "
                + "fecha_aplicacion = @fecha_aplicacion,"
                + "id_animal = @id_animal,"
                + "id_insumo = @id_insumo,"
                + "id_plan = @id_plan "
                + "WHERE id_vacunacion = @id_vacunacion";

            Dictionary<string, object?> parametros = new Dictionary<string, object?>
            {
                { "@fecha_aplicacion", pVacunacion.FechaAplicacion.Date },
                { "@id_animal", pVacunacion.Animal.IdAnimal },
                { "@id_insumo", pVacunacion.Insumo.IdInsumo },
                { "@id_plan", pVacunacion.Plan != null ? (object)pVacunacion.Plan.IdPlan : null },
                { "@id_vacunacion", pVacunacion.IdVacunacion }
            };

            return this.Escribir(sql, parametros, pMovimientos, "Error al modificar la vacunacion");
        }

        // Al eliminar la vacunacion vuelve la dosis al stock y el animal queda otra vez
        // pendiente en el plan que esta aplicacion daba por cumplido.
        public bool EliminarVacunacion(int pIdVacunacion, List<MovimientoStock> pMovimientos)
        {
            return this.Escribir("DELETE FROM vacunaciones WHERE id_vacunacion = @id_vacunacion",
                new Dictionary<string, object?> { { "@id_vacunacion", pIdVacunacion } },
                pMovimientos, "Error al eliminar la vacunacion");
        }

        private bool Escribir(string pSql, Dictionary<string, object?> pParametros,
            List<MovimientoStock> pMovimientos, string pMensajeError)
        {
            using (MySqlConnection conexion = Conexion.AbrirConexion())
            {
                using (MySqlTransaction transaccion = conexion.BeginTransaction())
                {
                    try
                    {
                        Conexion.EjecutarComandoEnTransaccion(pSql, pParametros, conexion, transaccion);

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

        private Animal BuscarAnimal(List<Animal> pLista, int pIdAnimal)
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

        private PlanSanitario BuscarPlan(List<PlanSanitario> pLista, int pIdPlan)
        {
            foreach (PlanSanitario unPlan in pLista)
            {
                if (unPlan.IdPlan == pIdPlan)
                {
                    return unPlan;
                }
            }
            return null;
        }
    }
}
