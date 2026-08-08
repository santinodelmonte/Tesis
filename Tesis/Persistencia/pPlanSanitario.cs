using MySql.Data.MySqlClient;
using Tesis.Dominio;
using System.Data;

namespace Tesis.Persistencia
{
    public class pPlanSanitario
    {
        private pConexion Conexion = new pConexion();

        // Ademas del plan se traen las categorias que alcanza, que viven en
        // plan_categorias. Se resuelve con una sola consulta adicional en lugar de una
        // por plan, igual que el detalle del ordenie por lote.
        public List<PlanSanitario> ListarPlanes(List<Insumo> pListaInsumos,
            List<Categoria> pListaCategorias)
        {
            string sql = "SELECT * FROM planes_sanitarios ORDER BY nombre";
            DataTable datos = Conexion.EjecutarConsulta(sql);
            List<PlanSanitario> lista = new List<PlanSanitario>();

            foreach (DataRow fila in datos.Rows)
            {
                PlanSanitario unPlan = new PlanSanitario(
                    int.Parse(fila["id_plan"].ToString()),
                    fila["nombre"].ToString(),
                    fila["tipo_procedimiento"].ToString(),
                    fila["periodicidad_dias"] != DBNull.Value ? int.Parse(fila["periodicidad_dias"].ToString()) : PlanSanitario.SIN_PERIODICIDAD, // El nulo es el procedimiento de aplicacion unica
                    int.Parse(fila["edad_inicio_meses"].ToString()),
                    Convert.ToBoolean(fila["activo"]),
                    fila["id_insumo"] != DBNull.Value ? this.BuscarInsumo(pListaInsumos, int.Parse(fila["id_insumo"].ToString())) : null, // El descorne no consume insumo
                    new List<Categoria>()
                    );
                lista.Add(unPlan);
            }

            DataTable detalle = Conexion.EjecutarConsulta("SELECT * FROM plan_categorias");
            foreach (DataRow fila in detalle.Rows)
            {
                PlanSanitario unPlan = this.BuscarEnLista(lista, int.Parse(fila["id_plan"].ToString()));
                Categoria unaCategoria = this.BuscarCategoria(pListaCategorias,
                    int.Parse(fila["id_categoria"].ToString()));

                if (unPlan != null && unaCategoria != null)
                {
                    unPlan.Categorias.Add(unaCategoria);
                }
            }

            return lista;
        }

        // El plan y las categorias que alcanza van en una misma transaccion: un plan
        // guardado a medias cambiaria el alcance de la regla sin que nadie lo note.
        public bool AltaPlan(PlanSanitario pPlan)
        {
            string sql = "INSERT INTO planes_sanitarios (nombre, tipo_procedimiento, periodicidad_dias, " +
                "edad_inicio_meses, activo, id_insumo) " +
                "VALUES (@nombre, @tipo_procedimiento, @periodicidad_dias, @edad_inicio_meses, " +
                "@activo, @id_insumo)";

            using (MySqlConnection conexion = Conexion.AbrirConexion())
            {
                using (MySqlTransaction transaccion = conexion.BeginTransaction())
                {
                    try
                    {
                        int vIdNuevo = Conexion.EjecutarInsercionEnTransaccion(sql,
                            this.Parametros(pPlan), conexion, transaccion);
                        pPlan.IdPlan = vIdNuevo;

                        this.GuardarCategorias(pPlan, vIdNuevo, conexion, transaccion);

                        transaccion.Commit();
                        return true;
                    }
                    catch (Exception e)
                    {
                        transaccion.Rollback();
                        // El id que habia asignado la base ya no vale, el alta se deshizo
                        pPlan.IdPlan = 0;
                        throw new Exception("Error al registrar el plan sanitario", e);
                    }
                }
            }
        }

        // Las categorias se borran y se vuelven a insertar: la tabla no tiene mas que
        // las dos claves, no hay nada que actualizar fila por fila.
        public bool ModificarPlan(PlanSanitario pPlan)
        {
            string sql = "UPDATE planes_sanitarios SET nombre = @nombre, " +
                "tipo_procedimiento = @tipo_procedimiento, periodicidad_dias = @periodicidad_dias, " +
                "edad_inicio_meses = @edad_inicio_meses, activo = @activo, id_insumo = @id_insumo " +
                "WHERE id_plan = @id_plan";

            Dictionary<string, object?> parametros = this.Parametros(pPlan);
            parametros.Add("@id_plan", pPlan.IdPlan);

            using (MySqlConnection conexion = Conexion.AbrirConexion())
            {
                using (MySqlTransaction transaccion = conexion.BeginTransaction())
                {
                    try
                    {
                        Conexion.EjecutarComandoEnTransaccion(sql, parametros, conexion, transaccion);

                        Conexion.EjecutarComandoEnTransaccion(
                            "DELETE FROM plan_categorias WHERE id_plan = @id_plan",
                            new Dictionary<string, object?> { { "@id_plan", pPlan.IdPlan } },
                            conexion, transaccion);

                        this.GuardarCategorias(pPlan, pPlan.IdPlan, conexion, transaccion);

                        transaccion.Commit();
                        return true;
                    }
                    catch (Exception e)
                    {
                        transaccion.Rollback();
                        throw new Exception("Error al modificar el plan sanitario", e);
                    }
                }
            }
        }

        private void GuardarCategorias(PlanSanitario pPlan, int pIdPlan,
            MySqlConnection pConexion, MySqlTransaction pTransaccion)
        {
            foreach (Categoria unaCategoria in pPlan.Categorias)
            {
                Conexion.EjecutarComandoEnTransaccion(
                    "INSERT INTO plan_categorias (id_plan, id_categoria) " +
                    "VALUES (@id_plan, @id_categoria)",
                    new Dictionary<string, object?>
                    {
                        { "@id_plan", pIdPlan },
                        { "@id_categoria", unaCategoria.IdCategoria }
                    },
                    pConexion, pTransaccion);
            }
        }

        private Dictionary<string, object?> Parametros(PlanSanitario pPlan)
        {
            return new Dictionary<string, object?>
            {
                { "@nombre", pPlan.Nombre },
                { "@tipo_procedimiento", pPlan.TipoProcedimiento },
                { "@periodicidad_dias", pPlan.PeriodicidadDias > PlanSanitario.SIN_PERIODICIDAD ? (object)pPlan.PeriodicidadDias : null },
                { "@edad_inicio_meses", pPlan.EdadInicioMeses },
                { "@activo", pPlan.Activo ? 1 : 0 },
                { "@id_insumo", pPlan.Insumo != null ? (object)pPlan.Insumo.IdInsumo : null }
            };
        }

        private PlanSanitario BuscarEnLista(List<PlanSanitario> pLista, int pIdPlan)
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
