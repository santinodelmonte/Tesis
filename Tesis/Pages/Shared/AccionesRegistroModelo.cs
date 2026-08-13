namespace Tesis.Pages.Shared
{
    // Datos que necesitan los dos botones de correccion de una fila,
    // _AccionesRegistro.cshtml.
    //
    // Es un modelo de pantalla y no de negocio, como CampoFotoModelo: vive en Pages y
    // no en Dominio. Existe porque los listados de Reproduccion, Sanidad y Produccion
    // muestran exactamente los mismos dos botones con exactamente el mismo
    // comportamiento, y repetir ese bloque diez veces significaba que arreglarlo en
    // uno no lo arreglaba en los otros nueve.
    public class AccionesRegistroModelo
    {
        // Pantalla que edita el registro. Es la misma que lo da de alta: recibe el
        // identificador por la direccion y se abre con los datos cargados.
        public string PaginaEditar { get; set; } = "";

        public int Id { get; set; } = 0;

        // Como se nombra el registro en el pedido de confirmacion. Tiene que alcanzar
        // para reconocerlo sin volver a la tabla: "el celo del 12/03/2026 de la
        // caravana 4521", no "el registro seleccionado".
        public string Descripcion { get; set; } = "";

        // Motivo por el que este registro no se puede eliminar, o vacio si se puede.
        // Lo devuelve el ValidarEliminar de la Controladora y se le muestra tal cual
        // al usuario: es el que dice que hay que sacar primero.
        public string MotivoBloqueo { get; set; } = "";

        // Manejador que atiende la baja. Se puede cambiar porque el historial de
        // produccion tiene dos listados eliminables en la misma pantalla -el ordenie
        // por lote y el control individual- y cada uno necesita el suyo.
        public string Handler { get; set; } = "Eliminar";

        // Que se deshace al borrar, para el pedido de confirmacion. Lo normal en
        // Reproduccion y Sanidad es que el borrado arrastre estado y stock; los
        // registros de produccion no arrastran nada y decir que si seria mentirle al
        // usuario justo cuando esta por confirmar algo irreversible.
        public string Consecuencia { get; set; } =
            "Los efectos que dejo -el estado del animal, el stock consumido- se deshacen solos.";

        // Datos que la baja tiene que devolver a la pantalla que la pidio. El historial
        // de produccion los usa para no perder el rango de fechas consultado: sin esto
        // el listado vuelve vacio y hay que volver a buscar.
        public Dictionary<string, string> CamposOcultos { get; set; } = new Dictionary<string, string>();

        // Texto del pedido de confirmacion, ya armado.
        public string TextoConfirmacion
        {
            get
            {
                return "Se va a eliminar " + Descripcion + ". " + Consecuencia
                    + " Esta accion no se puede deshacer.";
            }
        }
    }
}
