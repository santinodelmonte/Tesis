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

        // Asienta un movimiento y mueve el saldo del insumo, dentro de una transaccion
        // que ya viene abierta. Lo usan el ingreso de partidas, los consumos
        // automaticos -la pajuela de la inseminacion, el producto del tratamiento, la
        // dosis de la vacunacion- y las devoluciones que dejan las correcciones.
        //
        // El saldo se mueve con la cuenta hecha en la base y no con el valor que trae
        // el objeto, para que dos operaciones simultaneas no se pisen el stock. Antes
        // esta misma pareja de escrituras estaba repetida en pServicio, pTratamiento y
        // pVacunacion, cada una con su UPDATE escrito a mano.
        public static void AsentarEnTransaccion(pConexion pConexionDatos, MovimientoStock pMovimiento,
            MySqlConnection pConexion, MySqlTransaction pTransaccion)
        {
            int vIdNuevo = pConexionDatos.EjecutarInsercionEnTransaccion(SQL_ALTA,
                ParametrosAlta(pMovimiento), pConexion, pTransaccion);
            pMovimiento.IdMovimiento = vIdNuevo;

            Dictionary<string, object?> parametros = new Dictionary<string, object?>
            {
                { "@cantidad", pMovimiento.Cantidad },
                { "@id_insumo", pMovimiento.Insumo.IdInsumo }
            };

            if (pMovimiento.TipoMovimiento == MovimientoStock.INGRESO)
            {
                pConexionDatos.EjecutarComandoEnTransaccion(
                    "UPDATE insumos SET stock_actual = stock_actual + @cantidad WHERE id_insumo = @id_insumo",
                    parametros, pConexion, pTransaccion);
            }
            else
            {
                pConexionDatos.EjecutarComandoEnTransaccion(
                    "UPDATE insumos SET stock_actual = stock_actual - @cantidad WHERE id_insumo = @id_insumo",
                    parametros, pConexion, pTransaccion);
            }
        }

        // Los movimientos que deja una correccion: ninguno cuando no toco el consumo,
        // la devolucion sola cuando el registro se elimina, y la devolucion mas el
        // consumo nuevo cuando se cambio el producto o la cantidad. La lista puede
        // venir nula.
        public static void AsentarEnTransaccion(pConexion pConexionDatos,
            List<MovimientoStock> pMovimientos, MySqlConnection pConexion, MySqlTransaction pTransaccion)
        {
            if (pMovimientos == null)
            {
                return;
            }

            foreach (MovimientoStock unMovimiento in pMovimientos)
            {
                AsentarEnTransaccion(pConexionDatos, unMovimiento, pConexion, pTransaccion);
            }
        }

        // El ingreso de una partida deja su movimiento y suma el stock del insumo en
        // una misma transaccion. Antes eran dos escrituras sueltas: si la segunda
        // fallaba, el movimiento quedaba asentado y el inventario no lo reflejaba.
        public bool RegistrarIngreso(MovimientoStock pMovimiento)
        {
            using (MySqlConnection conexion = Conexion.AbrirConexion())
            {
                using (MySqlTransaction transaccion = conexion.BeginTransaction())
                {
                    try
                    {
                        AsentarEnTransaccion(Conexion, pMovimiento, conexion, transaccion);

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
