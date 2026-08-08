using MySql.Data.MySqlClient;
using Tesis.Dominio;
using System.Data;

namespace Tesis.Persistencia
{
    public class pMovimientoStock
    {
        private pConexion Conexion = new pConexion();

        public List<MovimientoStock> ListarMovimientos(List<Insumo> pListaInsumos)
        {
            string sql = "SELECT * FROM movimientos_stock ORDER BY fecha DESC, id_movimiento DESC";
            DataTable datos = Conexion.EjecutarConsulta(sql);
            List<MovimientoStock> lista = new List<MovimientoStock>();

            foreach (DataRow fila in datos.Rows)
            {
                MovimientoStock unMovimiento = new MovimientoStock(
                    int.Parse(fila["id_movimiento"].ToString()),
                    fila["tipo_movimiento"].ToString(),
                    Convert.ToDouble(fila["cantidad"]),
                    DateTime.Parse(fila["fecha"].ToString()),
                    fila["fecha_vencimiento"] != DBNull.Value ? DateTime.Parse(fila["fecha_vencimiento"].ToString()) : DateTime.MinValue, // Si es NULL, usa la fecha minima
                    fila["motivo"] != DBNull.Value ? fila["motivo"].ToString() : "", // Si es NULL, usa un string vacio
                    this.BuscarInsumo(pListaInsumos, int.Parse(fila["id_insumo"].ToString()))
                    );
                lista.Add(unMovimiento);
            }
            return lista;
        }

        public const string SQL_ALTA = "INSERT INTO movimientos_stock (tipo_movimiento, cantidad, fecha, " +
            "fecha_vencimiento, motivo, id_insumo) " +
            "VALUES (@tipo_movimiento, @cantidad, @fecha, @fecha_vencimiento, @motivo, @id_insumo)";

        public static Dictionary<string, object?> ParametrosAlta(MovimientoStock pMovimiento)
        {
            return new Dictionary<string, object?>
            {
                { "@tipo_movimiento", pMovimiento.TipoMovimiento },
                { "@cantidad", pMovimiento.Cantidad },
                { "@fecha", pMovimiento.Fecha.Date },
                { "@fecha_vencimiento", pMovimiento.FechaVencimiento != DateTime.MinValue ? (object)pMovimiento.FechaVencimiento.Date : null },
                { "@motivo", pMovimiento.Motivo },
                { "@id_insumo", pMovimiento.Insumo.IdInsumo }
            };
        }

        // El ingreso de una partida deja su movimiento y suma el stock del insumo en
        // una misma transaccion. Antes eran dos escrituras sueltas: si la segunda
        // fallaba, el movimiento quedaba asentado y el inventario no lo reflejaba.
        //
        // El stock se suma con la cuenta hecha en la base y no con el valor que trae el
        // objeto, para que dos ingresos simultaneos no se pisen el saldo. Es el mismo
        // criterio con el que los egresos automaticos lo descuentan.
        public bool RegistrarIngreso(MovimientoStock pMovimiento)
        {
            using (MySqlConnection conexion = Conexion.AbrirConexion())
            {
                using (MySqlTransaction transaccion = conexion.BeginTransaction())
                {
                    try
                    {
                        int vIdNuevo = Conexion.EjecutarInsercionEnTransaccion(SQL_ALTA,
                            ParametrosAlta(pMovimiento), conexion, transaccion);
                        pMovimiento.IdMovimiento = vIdNuevo;

                        Conexion.EjecutarComandoEnTransaccion(
                            "UPDATE insumos SET stock_actual = stock_actual + @cantidad WHERE id_insumo = @id_insumo",
                            new Dictionary<string, object?>
                            {
                                { "@cantidad", pMovimiento.Cantidad },
                                { "@id_insumo", pMovimiento.Insumo.IdInsumo }
                            },
                            conexion, transaccion);

                        transaccion.Commit();
                        return true;
                    }
                    catch (Exception e)
                    {
                        transaccion.Rollback();
                        // El id que habia asignado la base ya no vale, el alta se deshizo
                        pMovimiento.IdMovimiento = 0;
                        throw new Exception("Error al registrar el ingreso de stock", e);
                    }
                }
            }
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
