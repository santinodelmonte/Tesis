using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tesis.Dominio;

namespace Tesis.Pages.PagesSanidad
{
    // CU22. Una sola pantalla para el alta y para la modificacion, como plantea el paso
    // 3 del caso de uso: sin id llega un plan nuevo, con id se edita el existente.
    public class ConfigurarPlanModel : PageModel
    {
        [BindProperty]
        public int idPlan { get; set; } = 0;
        [BindProperty]
        public string? nombre { get; set; } = "";
        [BindProperty]
        public string tipoProcedimiento { get; set; } = PlanSanitario.VACUNACION;
        [BindProperty]
        public int periodicidadDias { get; set; } = PlanSanitario.SIN_PERIODICIDAD;
        [BindProperty]
        public int edadInicioMeses { get; set; } = 1;
        [BindProperty]
        public bool activo { get; set; } = true;
        [BindProperty]
        public int idInsumo { get; set; } = 0;

        // Categorias tildadas. La lista vacia deja el plan alcanzando a todo el rodeo.
        public List<int> idsCategorias = new List<int>();

        public List<Insumo> insumos = new List<Insumo>();
        public List<Categoria> categorias = new List<Categoria>();

        public void OnGet(int id)
        {
            Controladora unaControladora = new Controladora();
            this.CargarListados(unaControladora);

            PlanSanitario unPlan = unaControladora.BuscarPlanSanitario(id);
            if (unPlan == null)
            {
                return;
            }

            idPlan = unPlan.IdPlan;
            nombre = unPlan.Nombre;
            tipoProcedimiento = unPlan.TipoProcedimiento;
            periodicidadDias = unPlan.PeriodicidadDias;
            edadInicioMeses = unPlan.EdadInicioMeses;
            activo = unPlan.Activo;
            idInsumo = unPlan.Insumo != null ? unPlan.Insumo.IdInsumo : 0;

            foreach (Categoria unaCategoria in unPlan.Categorias)
            {
                idsCategorias.Add(unaCategoria.IdCategoria);
            }
        }

        public IActionResult OnPostGuardar()
        {
            Controladora unaControladora = new Controladora();
            this.CargarListados(unaControladora);
            this.LeerFormulario();

            PlanSanitario unPlan = this.ArmarPlan(unaControladora);

            // La Controladora devuelve el motivo por el que el plan no se puede
            // guardar, para que la pantalla informe cual validacion de CU22 fallo.
            string vMotivo = unaControladora.ValidarPlanSanitario(unPlan);
            if (vMotivo != "")
            {
                ModelState.AddModelError(string.Empty, vMotivo);
                return Page();
            }

            bool vGuardado = idPlan == 0
                ? unaControladora.AltaPlanSanitario(unPlan)
                : unaControladora.ModificarPlanSanitario(unPlan);

            if (vGuardado)
            {
                return Redirect("./ListaPlanes");
            }

            ModelState.AddModelError(string.Empty, "No se pudo guardar el plan sanitario!");
            return Page();
        }

        private PlanSanitario ArmarPlan(Controladora pControladoraDominio)
        {
            // El descorne no consume insumo (curso alternativo 4c)
            Insumo unInsumo = tipoProcedimiento != PlanSanitario.DESCORNE
                ? pControladoraDominio.BuscarInsumo(idInsumo)
                : null;

            List<Categoria> _listaCategorias = new List<Categoria>();
            foreach (int vIdCategoria in idsCategorias)
            {
                Categoria unaCategoria = pControladoraDominio.BuscarCategoria(vIdCategoria);
                if (unaCategoria != null)
                {
                    _listaCategorias.Add(unaCategoria);
                }
            }

            return new PlanSanitario(idPlan, nombre ?? "", tipoProcedimiento, periodicidadDias,
                edadInicioMeses, activo, unInsumo, _listaCategorias);
        }

        private void CargarListados(Controladora pControladoraDominio)
        {
            insumos = pControladoraDominio.ListarInsumosSanitarios();
            categorias = pControladoraDominio.ListarCategorias();
        }

        private void LeerFormulario()
        {
            int vIdPlan = 0;
            int.TryParse(Request.Form["idPlan"], out vIdPlan);
            idPlan = vIdPlan;

            nombre = Request.Form["nombre"];
            tipoProcedimiento = Request.Form["tipoProcedimiento"] != ""
                ? Request.Form["tipoProcedimiento"]
                : PlanSanitario.VACUNACION;

            // La periodicidad vacia es el procedimiento de aplicacion unica
            int vPeriodicidad = PlanSanitario.SIN_PERIODICIDAD;
            int.TryParse(Request.Form["periodicidadDias"], out vPeriodicidad);
            periodicidadDias = vPeriodicidad;

            int vEdad = 0;
            int.TryParse(Request.Form["edadInicioMeses"], out vEdad);
            edadInicioMeses = vEdad;

            activo = Request.Form["activo"].ToString().Contains("true");

            int vIdInsumo = 0;
            int.TryParse(Request.Form["idInsumo"], out vIdInsumo);
            idInsumo = vIdInsumo;

            idsCategorias = new List<int>();
            foreach (string? vValor in Request.Form["categorias"])
            {
                int vIdCategoria = 0;
                if (int.TryParse(vValor, out vIdCategoria) && vIdCategoria != 0)
                {
                    idsCategorias.Add(vIdCategoria);
                }
            }
        }
    }
}
