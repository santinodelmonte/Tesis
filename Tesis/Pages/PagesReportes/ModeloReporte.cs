using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tesis.Dominio;
using Tesis.Reportes;

namespace Tesis.Pages.PagesReportes
{
    // Lo que tienen en comun las cuatro pantallas de reportes (CU44 a CU47): el rango
    // de fechas, la vista previa en pantalla y los dos botones de descarga.
    //
    // Es una clase base y no cuatro copias porque lo unico que cambia entre un reporte
    // y otro es que metodo de la Controladora lo arma y como se llama el archivo. Con
    // cuatro copias, arreglar la validacion del rango significaria acordarse de
    // arreglarla en las cuatro.
    //
    // La vista previa importa: un reporte que solo se puede ver descargandolo obliga a
    // abrir un archivo para descubrir que el periodo estaba mal.
    public abstract class ModeloReporte : PageModel
    {
        public DateTime fechaDesde { get; set; } = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
        public DateTime fechaHasta { get; set; } = DateTime.Now;

        // El reporte armado, o nulo mientras no se consulto nada.
        public Reporte reporte = null;

        // Cada reporte concreto dice como se arma y como se llama su archivo.
        protected abstract Reporte Armar(Controladora pControladoraDominio);
        protected abstract string NombreDelArchivo { get; }

        // El reporte genetico es una foto del rodeo y no de un periodo, asi que su
        // pantalla no muestra el rango de fechas.
        public virtual bool UsaPeriodo { get { return true; } }

        public void OnGet()
        {
        }

        public void OnPostConsultar()
        {
            this.Consultar();
        }

        public IActionResult OnPostPdf()
        {
            if (!this.Consultar())
            {
                return Page();
            }

            return File(GeneradorPdf.Generar(reporte), "application/pdf",
                this.NombreCompleto("pdf"));
        }

        public IActionResult OnPostExcel()
        {
            if (!this.Consultar())
            {
                return Page();
            }

            return File(GeneradorExcel.Generar(reporte),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                this.NombreCompleto("xlsx"));
        }

        // Arma el reporte y avisa si se pudo. Las tres acciones -consultar, descargar en
        // PDF y descargar en planilla- pasan por aca, de modo que los tres resultados
        // salen del mismo contenido y no de tres consultas distintas.
        private bool Consultar()
        {
            this.LeerFormulario();

            if (this.UsaPeriodo && fechaDesde > fechaHasta)
            {
                ModelState.AddModelError(string.Empty,
                    "El rango de fechas es invalido: la fecha desde es posterior a la fecha hasta!");
                return false;
            }

            reporte = this.Armar(new Controladora());
            return true;
        }

        private void LeerFormulario()
        {
            if (!this.UsaPeriodo)
            {
                return;
            }

            fechaDesde = Request.Form["fechaDesde"] != ""
                ? Convert.ToDateTime(Request.Form["fechaDesde"])
                : fechaDesde;

            fechaHasta = Request.Form["fechaHasta"] != ""
                ? Convert.ToDateTime(Request.Form["fechaHasta"])
                : fechaHasta;
        }

        // El nombre con el que el archivo llega a la computadora del usuario. Lleva la
        // fecha de emision para que dos descargas del mismo reporte no se pisen en la
        // carpeta de descargas.
        private string NombreCompleto(string pExtension)
        {
            return NombreDelArchivo + "-" + DateTime.Now.ToString("yyyy-MM-dd") + "." + pExtension;
        }
    }
}
