using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tesis.Dominio;

namespace Tesis.Pages.PagesAnimal
{
    public class BuscarAnimalesModel : PageModel
    {
        // La caravana es un filtro opcional: se declara anulable para que no salte el requerido implicito
        [BindProperty]
        public string? numCaravana { get; set; } = "";
        [BindProperty]
        public int idRaza { get; set; } = 0;
        [BindProperty]
        public int idCategoria { get; set; } = 0;
        [BindProperty]
        public int estado { get; set; } = 1;
        [BindProperty]
        public int edadDesde { get; set; } = 0;
        [BindProperty]
        public int edadHasta { get; set; } = 0;

        public List<Animal> animales = new List<Animal>();
        public List<Animal> todosLosAnimales = new List<Animal>();
        public List<Raza> razas = new List<Raza>();
        public List<Categoria> categorias = new List<Categoria>();
        public bool filtrado = false;

        public void OnGet()
        {
            Controladora unaControladora = new Controladora();
            todosLosAnimales = unaControladora.ListarAnimales();
            razas = unaControladora.ListarRazas();
            categorias = unaControladora.ListarCategorias();

            // Por defecto se muestra el rodeo activo
            animales = unaControladora.FiltrarAnimalesXEstado(true);
        }

        public void OnPostFiltrarAnimales()
        {
            Controladora unaControladora = new Controladora();

            // Recargar las listas para los filtros y para el selector
            todosLosAnimales = unaControladora.ListarAnimales();
            razas = unaControladora.ListarRazas();
            categorias = unaControladora.ListarCategorias();

            numCaravana = Request.Form["numCaravana"];
            idRaza = Request.Form["idRaza"] != "" ? Convert.ToInt32(Request.Form["idRaza"]) : 0;
            idCategoria = Request.Form["idCategoria"] != "" ? Convert.ToInt32(Request.Form["idCategoria"]) : 0;
            estado = Request.Form["estado"] != "" ? Convert.ToInt32(Request.Form["estado"]) : -1;
            edadDesde = Request.Form["edadDesde"] != "" ? Convert.ToInt32(Request.Form["edadDesde"]) : 0;
            edadHasta = Request.Form["edadHasta"] != "" ? Convert.ToInt32(Request.Form["edadHasta"]) : 0;

            filtrado = true;

            if (edadHasta > 0 && edadDesde > edadHasta)
            {
                animales = new List<Animal>();
                ModelState.AddModelError(string.Empty, "El rango etario es incorrecto: la edad desde no puede superar a la edad hasta!");
                return;
            }

            // Los filtros se encadenan sobre la coleccion en memoria
            animales = unaControladora.FiltrarAnimales(numCaravana, idRaza, idCategoria, estado, edadDesde, edadHasta);

            if (animales.Count == 0)
            {
                ModelState.AddModelError(string.Empty, "No se encontraron animales con los criterios ingresados!");
            }
        }

        public void OnPostLimpiarFiltros()
        {
            // Restablecer el listado por defecto
            OnGet();
            numCaravana = "";   // Restablecer los valores de los filtros
            idRaza = 0;
            idCategoria = 0;
            estado = 1;
            edadDesde = 0;
            edadHasta = 0;
            filtrado = false;

            // Sin esto el formulario vuelve a mostrar los filtros que se enviaron
            ModelState.Clear();
        }
    }
}
