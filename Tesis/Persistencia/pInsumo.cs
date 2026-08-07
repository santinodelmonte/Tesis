using Tesis.Dominio;
using System.Data;

namespace Tesis.Persistencia
{
    public class pInsumo
    {
        private pConexion Conexion = new pConexion();

        // Los machos llegan ya listados por parametro: la pajuela referencia al toro
        // que la aporta y esta capa no instancia la Controladora para resolverlo.
        public List<Insumo> ListarInsumos(List<Macho> pListaMachos)
        {
            string sql = "SELECT * FROM insumos ORDER BY nombre";
            DataTable datos = Conexion.EjecutarConsulta(sql);
            List<Insumo> lista = new List<Insumo>();

            foreach (DataRow fila in datos.Rows)
            {
                Insumo unInsumo = new Insumo(
                    int.Parse(fila["id_insumo"].ToString()),
                    fila["nombre"].ToString(),
                    fila["tipo_insumo"].ToString(),
                    Convert.ToDouble(fila["stock_actual"]),
                    Convert.ToDouble(fila["stock_minimo"]),
                    fila["periodo_descarte_dias"] != DBNull.Value ? int.Parse(fila["periodo_descarte_dias"].ToString()) : 0, // Si es NULL, no obliga a descartar leche
                    fila["id_macho"] != DBNull.Value ? this.BuscarMacho(pListaMachos, int.Parse(fila["id_macho"].ToString())) : null
                    );
                lista.Add(unInsumo);
            }
            return lista;
        }

        public bool AltaInsumo(Insumo pInsumo)
        {
            string sql = "INSERT INTO insumos (nombre, tipo_insumo, stock_actual, stock_minimo, " +
                "periodo_descarte_dias, id_macho) " +
                "VALUES (@nombre, @tipo_insumo, @stock_actual, @stock_minimo, " +
                "@periodo_descarte_dias, @id_macho)";

            Dictionary<string, object?> parametros = new Dictionary<string, object?>
            {
                { "@nombre", pInsumo.Nombre },
                { "@tipo_insumo", pInsumo.TipoInsumo },
                { "@stock_actual", pInsumo.StockActual },
                { "@stock_minimo", pInsumo.StockMinimo },
                { "@periodo_descarte_dias", pInsumo.PeriodoDescarteDias > 0 ? (object)pInsumo.PeriodoDescarteDias : null },
                { "@id_macho", pInsumo.Toro != null ? (object)pInsumo.Toro.IdAnimal : null }
            };

            pInsumo.IdInsumo = Conexion.EjecutarInsercion(sql, parametros);
            return pInsumo.IdInsumo > 0;
        }

        // Solo el stock: el resto de los datos del insumo no cambia con un movimiento.
        public bool ActualizarStock(int pIdInsumo, double pStockActual)
        {
            string sql = "UPDATE insumos SET stock_actual = @stock_actual WHERE id_insumo = @id_insumo";

            Dictionary<string, object?> parametros = new Dictionary<string, object?>
            {
                { "@stock_actual", pStockActual },
                { "@id_insumo", pIdInsumo }
            };

            return Conexion.EjecutarComando(sql, parametros);
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
    }
}
