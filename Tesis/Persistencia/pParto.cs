using MySql.Data.MySqlClient;
using Tesis.Dominio;
using System.Data;

namespace Tesis.Persistencia
{
    public class pParto
    {
        private pConexion Conexion = new pConexion();

        public List<Parto> ListarPartos(List<Hembra> pListaHembras)
        {
            string sql = "SELECT * FROM partos ORDER BY fecha_parto DESC, id_parto DESC";
            DataTable datos = Conexion.EjecutarConsulta(sql);
            List<Parto> lista = new List<Parto>();

            foreach (DataRow fila in datos.Rows)
            {
                Parto unParto = new Parto(
                    int.Parse(fila["id_parto"].ToString()),
                    DateTime.Parse(fila["fecha_parto"].ToString()),
                    fila["tipo_parto"].ToString(),
                    fila["observaciones"] != DBNull.Value ? fila["observaciones"].ToString() : "", // Si es NULL, usa un string vacio
                    this.BuscarHembra(pListaHembras, int.Parse(fila["id_animal"].ToString()))
                    );
                lista.Add(unParto);
            }
            return lista;
        }

        // El parto es la operacion que mas tablas toca del sistema: asienta el parto,
        // da de alta la cria con su especializacion, abre la lactancia de la madre y
        // actualiza sus estados y su numero de partos. Todo va en una misma
        // transaccion: un parto a medias deja a la madre en lactancia sin lactancia
        // abierta, o una cria sin madre.
        // La madre llega aparte y no se toma de pParto.Madre: el dominio manda una
        // copia con el numero de partos y los estados nuevos, y recien actualiza el
        // objeto de la cache cuando la escritura salio bien.
        //
        // pLactanciaCerrada puede venir nula. Cuando no lo es, es la lactancia anterior
        // que quedo abierta porque no se registro el secado, y que este parto cierra.
        //
        // La lactancia nueva se llama pNuevaLactancia y no pLactancia porque este
        // ultimo es el nombre de la clase de persistencia que se usa mas abajo.
        public bool AltaParto(Parto pParto, List<Animal> pListaCrias, Lactancia pNuevaLactancia,
            Hembra pMadreActualizada, Lactancia pLactanciaCerrada)
        {
            using (MySqlConnection conexion = Conexion.AbrirConexion())
            {
                using (MySqlTransaction transaccion = conexion.BeginTransaction())
                {
                    try
                    {
                        int vIdParto = Conexion.EjecutarInsercionEnTransaccion(SQL_ALTA,
                            ParametrosAlta(pParto), conexion, transaccion);
                        pParto.IdParto = vIdParto;

                        // Las crias: primero animales y despues su especializacion, igual
                        // que en el alta comun. Son varias cuando el parto fue doble.
                        foreach (Animal unaCria in pListaCrias)
                        {
                            int vIdCria = Conexion.EjecutarInsercionEnTransaccion(pAnimal.SQL_ALTA,
                                pAnimal.ParametrosAlta(unaCria), conexion, transaccion);
                            unaCria.IdAnimal = vIdCria;

                            if (unaCria is Hembra)
                            {
                                Conexion.EjecutarInsercionEnTransaccion(pHembra.SQL_ALTA,
                                    pHembra.ParametrosAlta((Hembra)unaCria), conexion, transaccion);
                            }
                            else
                            {
                                Conexion.EjecutarInsercionEnTransaccion(pMacho.SQL_ALTA,
                                    pMacho.ParametrosAlta((Macho)unaCria), conexion, transaccion);
                            }
                        }

                        // La lactancia anterior que habia quedado abierta
                        if (pLactanciaCerrada != null)
                        {
                            Conexion.EjecutarComandoEnTransaccion(pLactancia.SQL_MODIFICAR,
                                pLactancia.ParametrosModificar(pLactanciaCerrada), conexion, transaccion);
                        }

                        // La lactancia que abre el parto
                        int vIdLactancia = Conexion.EjecutarInsercionEnTransaccion(pLactancia.SQL_ALTA,
                            pLactancia.ParametrosAlta(pNuevaLactancia), conexion, transaccion);
                        pNuevaLactancia.IdLactancia = vIdLactancia;

                        // La madre queda con un parto mas, en lactancia y vacia
                        Conexion.EjecutarComandoEnTransaccion(pHembra.SQL_MODIFICAR,
                            pHembra.ParametrosModificar(pMadreActualizada), conexion, transaccion);

                        transaccion.Commit();
                        return true;
                    }
                    catch (Exception e)
                    {
                        transaccion.Rollback();
                        // Los id que habia asignado la base ya no valen, el alta se deshizo
                        pParto.IdParto = 0;
                        pNuevaLactancia.IdLactancia = 0;

                        foreach (Animal unaCria in pListaCrias)
                        {
                            unaCria.IdAnimal = 0;
                        }

                        throw new Exception("Error al registrar el parto", e);
                    }
                }
            }
        }

        public const string SQL_ALTA = "INSERT INTO partos (fecha_parto, tipo_parto, observaciones, id_animal) " +
            "VALUES (@fecha_parto, @tipo_parto, @observaciones, @id_animal)";

        public static Dictionary<string, object?> ParametrosAlta(Parto pParto)
        {
            return new Dictionary<string, object?>
            {
                { "@fecha_parto", pParto.FechaParto.Date },
                { "@tipo_parto", pParto.TipoParto },
                { "@observaciones", pParto.Observaciones },
                { "@id_animal", pParto.Madre.IdAnimal }
            };
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
    }
}
