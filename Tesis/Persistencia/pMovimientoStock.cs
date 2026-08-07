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

        public bool AltaMovimiento(MovimientoStock pMovimiento)
        {
            pMovimiento.IdMovimiento = Conexion.EjecutarInsercion(SQL_ALTA, ParametrosAlta(pMovimiento));
            return pMovimiento.IdMovimiento > 0;
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
