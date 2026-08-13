namespace Tesis.Pages.Shared
{
    // Datos que necesitan los dos botones de correccion de una fila,
    // _AccionesRegistro.cshtml.
    //
    // Es un modelo de pantalla y no de negocio, como CampoFotoModelo: vive en Pages y
    // no en Dominio. Existe porque los ocho listados de Reproduccion y Sanidad
    // muestran exactamente los mismos dos botones con exactamente el mismo
    // comportamiento, y repetir ese bloque ocho veces significaba que arreglarlo en
    // uno no lo arreglaba en los otros siete.
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

        // Texto del pedido de confirmacion, ya armado.
        public string TextoConfirmacion
        {
            get
            {
                return "Se va a eliminar " + Descripcion
                    + ". Los efectos que dejo -el estado del animal, el stock consumido- se deshacen solos. "
                    + "Esta accion no se puede deshacer.";
            }
        }
    }
}
