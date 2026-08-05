using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tesis.Dominio;
using System.ComponentModel.DataAnnotations;

namespace Tesis.Pages.PagesAnimal
{
    public class BajaAnimalModel : PageModel
    {
        public Animal animal { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "El número de caravana es requerido")]
        public string numCaravana { get; set; } = "";
        [BindProperty]
        [Required(ErrorMessage = "El motivo de salida es requerido")]
        public string motivoBaja { get; set; } = "";

        public List<Motivo> motivos = new List<Motivo>()
        {
            new Motivo(){ id = 1, nombre = "Venta" },
            new Motivo(){ id = 2, nombre = "Fallecimiento" },
            new Motivo(){ id = 3, nombre = "Descarte sanitario" },
            new Motivo(){ id = 4, nombre = "Otro" }
        };

        public void OnGet(int id)
        {
            Controladora unaControladora = new Controladora();
            unaControladora.ListarAnimales();
            animal = unaControladora.BuscarAnimal(id);
        }

        public IActionResult OnPostConfirmarBaja()
        {
            Controladora unaControladora = new Controladora();
            unaControladora.ListarAnimales();

            numCaravana = Request.Form["numCaravana"];
            motivoBaja = Request.Form["motivoBaja"];

            animal = unaControladora.BuscarAnimalXCaravana(numCaravana);

            // La baja es logica: el animal deja el rodeo activo pero conserva su historial
            if (unaControladora.BajaAnimal(numCaravana, motivoBaja))
            {
                return Redirect("./ListaAnimales");
            }

            ModelState.AddModelError(string.Empty, "No se pudo dar de baja el animal. Verifique que siga activo!");
            return Page();
        }

        public class Motivo
        {
            public int id;
            public string nombre;

            public int Id { get { return id; } set { id = value; } }
            public string Nombre { get { return nombre; } set { nombre = value; } }
            public Motivo() { }
        }
    }
}
