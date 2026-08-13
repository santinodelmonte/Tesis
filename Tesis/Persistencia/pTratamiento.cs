using MySql.Data.MySqlClient;
using Tesis.Dominio;
using System.Data;

namespace Tesis.Persistencia
{
    public class pTratamiento
    {
        private pConexion Conexion = new pConexion();

        // id_animal e id_plan se leen y se escriben desde el Modulo 4: la columna
        // id_plan ya existia y la de animal la agrega tambo_m4_m5.sql. Un tratamiento
        // anterior a ese script puede tener el animal en nulo si era preventivo.
        public List<Tratamiento> ListarTratamientos(List<Diagnostico> pListaDiagnosticos,
            List<Insumo> pListaInsumos, List<Animal> pListaAnimales, List<PlanSanitario> pListaPlanes)
        {
            string sql = "SELECT * FROM tratamientos ORDER BY fecha_inicio DESC, id_tratamiento DESC";
            DataTable datos = Conexion.EjecutarConsulta(sql);
            List<Tratamiento> lista = new List<Tratamiento>();

            foreach (DataRow fila in datos.Rows)
            {
                Tratamiento unTratamiento = new Tratamiento(
                    int.Parse(fila["id_tratamiento"].ToString()),
                    DateTime.Parse(fila["fecha_inicio"].ToString()),
                    int.Parse(fila["dias_duracion"].ToString()),
                    fila["dosis_diaria"].ToString(),
                    Convert.ToDouble(fila["cantidad_insumo"]),
                    fila["fecha_fin_descarte"] != DBNull.Value ? DateTime.Parse(fila["fecha_fin_descarte"].ToString()) : DateTime.MinValue, // Si es NULL, el producto no obliga a descartar leche
                    fila["id_diagnostico"] != DBNull.Value
                        ? this.BuscarDiagnostico(pListaDiagnosticos, int.Parse(fila["id_diagnostico"].ToString()))
                        : null, // El nulo identifica al tratamiento preventivo
                    fila["id_animal"] != DBNull.Value
                        ? this.BuscarAnimal(pListaAnimales, int.Parse(fila["id_animal"].ToString()))
                        : null, // Solo los preventivos anteriores al Modulo 4 quedan sin animal
                    this.BuscarInsumo(pListaInsumos, int.Parse(fila["id_insumo"].ToString())),
                    fila["id_plan"] != DBNull.Value
                        ? this.BuscarPlan(pListaPlanes, int.Parse(fila["id_plan"].ToString()))
                        : null // El nulo es el tratamiento registrado fuera de todo plan
                    );
                lista.Add(unTratamiento);
            }
            return lista;
        }

        // El alta guarda el tratamiento, descuenta el insumo del stock con su
        // movimiento y, cuando el tratamiento viene de un diagnostico, lo pasa a "En
        // tratamiento". Las cuatro escrituras van en una misma transaccion.
        public bool AltaTratamiento(Tratamiento pTratamiento)
        {
            string sql = "INSERT INTO tratamientos (fecha_inicio, dias_duracion, dosis_diaria, " +
                "cantidad_insumo, id_animal, fecha_fin_descarte, id_diagnostico, id_insumo, id_plan) " +
                "VALUES (@fecha_inicio, @dias_duracion, @dosis_diaria, @cantidad_insumo, @id_animal, " +
                "@fecha_fin_descarte, @id_diagnostico, @id_insumo, @id_plan)";

            using (MySqlConnection conexion = Conexion.AbrirConexion())
            {
                using (MySqlTransaction transaccion = conexion.BeginTransaction())
                {
                    try
                    {
                        int vIdNuevo = Conexion.EjecutarInsercionEnTransaccion(sql,
                            ParametrosEscritura(pTratamiento), conexion, transaccion);
                        pTratamiento.IdTratamiento = vIdNuevo;

                        if (pTratamiento.CantidadInsumo > 0)
                        {
                            MovimientoStock unMovimiento = new MovimientoStock(0, MovimientoStock.EGRESO,
                                pTratamiento.CantidadInsumo, pTratamiento.FechaInicio, DateTime.MinValue,
                                "Tratamiento sanitario", pTratamiento.Insumo);

                            pMovimientoStock.AsentarEnTransaccion(Conexion, unMovimiento, conexion, transaccion);

                            pTratamiento.Insumo.StockActual = pTratamiento.Insumo.StockActual
                                - pTratamiento.CantidadInsumo;
                        }

                        if (pTratamiento.Diagnostico != null)
                        {
                            Conexion.EjecutarComandoEnTransaccion(pDiagnostico.SQL_MODIFICAR_ESTADO,
                                pDiagnostico.ParametrosModificarEstado(pTratamiento.Diagnostico.IdDiagnostico,
                                    Diagnostico.EN_TRATAMIENTO),
                                conexion, transaccion);

                            pTratamiento.Diagnostico.Estado = Diagnostico.EN_TRATAMIENTO;
                        }

                        transaccion.Commit();
                        return true;
                    }
                    catch (Exception e)
                    {
                        transaccion.Rollback();
                        // El id que habia asignado la base ya no vale, el alta se deshizo
                        pTratamiento.IdTratamiento = 0;
                        throw new Exception("Error al registrar el tratamiento", e);
                    }
                }
            }
        }

        // Los mismos campos escriben el alta y la correccion, con la diferencia del
        // identificador, que en el INSERT sobra y el UPDATE ignora.
        private static Dictionary<string, object?> ParametrosEscritura(Tratamiento pTratamiento)
        {
            return new Dictionary<string, object?>
            {
                { "@fecha_inicio", pTratamiento.FechaInicio.Date },
                { "@dias_duracion", pTratamiento.DiasDuracion },
                { "@dosis_diaria", pTratamiento.DosisDiaria },
                { "@cantidad_insumo", pTratamiento.CantidadInsumo },
                { "@id_animal", pTratamiento.Animal != null ? (object)pTratamiento.Animal.IdAnimal : null },
                { "@fecha_fin_descarte", pTratamiento.FechaFinDescarte != DateTime.MinValue ? (object)pTratamiento.FechaFinDescarte.Date : null },
                { "@id_diagnostico", pTratamiento.Diagnostico != null ? (object)pTratamiento.Diagnostico.IdDiagnostico : null },
                { "@id_insumo", pTratamiento.Insumo.IdInsumo },
                { "@id_plan", pTratamiento.Plan != null ? (object)pTratamiento.Plan.IdPlan : null },
                { "@id_tratamiento", pTratamiento.IdTratamiento }
            };
        }

        // Corregir un tratamiento puede cambiar el producto o la cantidad -y ahi los
        // movimientos traen la devolucion de lo que no se uso y el egreso de lo que si-
        // y puede cambiar el diagnostico del que viene. Los diagnosticos que llegan en
        // la lista son el que se deja atras, que vuelve a quedar activo si no le queda
        // ningun otro tratamiento, y el nuevo, que pasa a estar en tratamiento.
        //
        // La fecha de fin de descarte se recalcula en el dominio y llega en el objeto:
        // es la que deja al animal fuera del lote de ordenie, asi que corregir la
        // duracion o el producto sin recalcularla dejaria leche descartada de mas o,
        // peor, leche con residuos yendo al tanque.
        public bool ModificarTratamiento(Tratamiento pTratamiento, List<MovimientoStock> pMovimientos,
            List<Diagnostico> pDiagnosticosActualizados)
        {
            string sql = "UPDATE tratamientos SET "
                + "fecha_inicio = @fecha_inicio,"
                + "dias_duracion = @dias_duracion,"
                + "dosis_diaria = @dosis_diaria,"
                + "cantidad_insumo = @cantidad_insumo,"
                + "id_animal = @id_animal,"
                + "fecha_fin_descarte = @fecha_fin_descarte,"
                + "id_diagnostico = @id_diagnostico,"
                + "id_insumo = @id_insumo,"
                + "id_plan = @id_plan "
                + "WHERE id_tratamiento = @id_tratamiento";

            return this.Escribir(sql, ParametrosEscritura(pTratamiento), pMovimientos,
                pDiagnosticosActualizados, "Error al modificar el tratamiento");
        }

        // El borrado devuelve al stock lo que el tratamiento habia consumido y, si el
        // diagnostico se queda sin tratamientos, lo vuelve a dejar activo. El descarte
        // de leche no hay que deshacerlo: sale de los tratamientos que hay, asi que
        // desaparece solo cuando el tratamiento deja de existir.
        public bool EliminarTratamiento(int pIdTratamiento, List<MovimientoStock> pMovimientos,
            List<Diagnostico> pDiagnosticosActualizados)
        {
            return this.Escribir("DELETE FROM tratamientos WHERE id_tratamiento = @id_tratamiento",
                new Dictionary<string, object?> { { "@id_tratamiento", pIdTratamiento } },
                pMovimientos, pDiagnosticosActualizados, "Error al eliminar el tratamiento");
        }

        private bool Escribir(string pSql, Dictionary<string, object?> pParametros,
            List<MovimientoStock> pMovimientos, List<Diagnostico> pDiagnosticosActualizados,
            string pMensajeError)
        {
            using (MySqlConnection conexion = Conexion.AbrirConexion())
            {
                using (MySqlTransaction transaccion = conexion.BeginTransaction())
                {
                    try
                    {
                        Conexion.EjecutarComandoEnTransaccion(pSql, pParametros, conexion, transaccion);

                        pMovimientoStock.AsentarEnTransaccion(Conexion, pMovimientos, conexion, transaccion);

                        if (pDiagnosticosActualizados != null)
                        {
                            foreach (Diagnostico unDiagnostico in pDiagnosticosActualizados)
                            {
                                Conexion.EjecutarComandoEnTransaccion(pDiagnostico.SQL_MODIFICAR_ESTADO,
                                    pDiagnostico.ParametrosModificarEstado(unDiagnostico.IdDiagnostico,
                                        unDiagnostico.Estado),
                                    conexion, transaccion);
                            }
                        }

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

        private Diagnostico BuscarDiagnostico(List<Diagnostico> pLista, int pIdDiagnostico)
        {
            foreach (Diagnostico unDiagnostico in pLista)
            {
                if (unDiagnostico.IdDiagnostico == pIdDiagnostico)
                {
                    return unDiagnostico;
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
