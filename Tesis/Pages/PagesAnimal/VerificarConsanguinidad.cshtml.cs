using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tesis.Dominio;
using System.ComponentModel.DataAnnotations;

namespace Tesis.Pages.PagesAnimal
{
    public class VerificarConsanguinidadModel : PageModel
    {
        // Los animales llegan del selector modal, por eso se validan en el handler y no con atributos
        [BindProperty]
        public int idHembra { get; set; } = 0;
        [BindProperty]
        public int idMacho { get; set; } = 0;

        public List<Hembra> hembras = new List<Hembra>();
        public List<Macho> machos = new List<Macho>();
        public List<Animal> animales = new List<Animal>();

        // Caravanas elegidas, para mostrarlas en el formulario
        public string caravanaHembra = "";
        public string caravanaMacho = "";

        public bool verificado = false;
        public bool hayParentesco = false;
        public bool verificacionParcial = false;
        public string detalle = "";
        public Animal ancestroComun { get; set; }

        public void OnGet()
        {
            Controladora unaControladora = new Controladora();
            hembras = unaControladora.ListarHembras();
            machos = unaControladora.ListarMachos();
            animales = unaControladora.ListarAnimales();
        }

        public void OnPostVerificar()
        {
            Controladora unaControladora = new Controladora();

            // Recargar las listas para el selector
            hembras = unaControladora.ListarHembras();
            machos = unaControladora.ListarMachos();
            animales = unaControladora.ListarAnimales();

            idHembra = Request.Form["idHembra"] != "" ? Convert.ToInt32(Request.Form["idHembra"]) : 0;
            idMacho = Request.Form["idMacho"] != "" ? Convert.ToInt32(Request.Form["idMacho"]) : 0;

            Hembra unaHembra = unaControladora.BuscarHembra(idHembra);
            Macho unMacho = unaControladora.BuscarMacho(idMacho);

            // El selector guarda el id, pero el formulario tiene que volver a mostrar la caravana
            caravanaHembra = unaHembra != null ? unaHembra.NumCaravana : "";
            caravanaMacho = unMacho != null ? unMacho.NumCaravana : "";

            if (unaHembra == null || unMacho == null)
            {
                ModelState.AddModelError(string.Empty, "Seleccione la hembra y el reproductor!");
                return;
            }

            if (unaHembra.IdAnimal == unMacho.IdAnimal)
            {
                ModelState.AddModelError(string.Empty, "No puede verificar un animal contra sí mismo!");
                return;
            }

            verificado = true;
            hayParentesco = unaControladora.VerificarConsanguinidad(unaHembra, unMacho);
            ancestroComun = unaControladora.BuscarAncestroComun(unaHembra, unMacho);

            // Si a alguno le falta la ascendencia, la comparacion es parcial
            if (unaHembra.Madre == null || unaHembra.Padre == null)
            {
                verificacionParcial = true;
                detalle = detalle + "La hembra " + unaHembra.NumCaravana + " no tiene toda su ascendencia registrada. ";
            }
            if (unMacho.Madre == null || unMacho.Padre == null)
            {
                verificacionParcial = true;
                detalle = detalle + "El reproductor " + unMacho.NumCaravana + " no tiene toda su ascendencia registrada. ";
            }
        }
    }
}
