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

                            Conexion.EjecutarInsercionEnTransaccion(pMovimientoStock.SQL_ALTA,
                                pMovimientoStock.ParametrosAlta(unMovimiento), conexion, transaccion);

                            Conexion.EjecutarComandoEnTransaccion(
                                "UPDATE insumos SET stock_actual = stock_actual - @cantidad WHERE id_insumo = @id_insumo",
                                new Dictionary<string, object?>
                                {
                                    { "@cantidad", pCantidadInsumo },
                                    { "@id_insumo", pVacunacion.Insumo.IdInsumo }
                                },
                                conexion, transaccion);

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
