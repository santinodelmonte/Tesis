using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Text.Json;
using Tesis.Dominio;

namespace Tesis.Pages.PagesAnimal
{
    public class ConsultaLinajeModel : PageModel
    {
        // Hasta donde se recorre la ascendencia. La validacion de genealogia impide
        // que un animal sea ancestro de si mismo, asi que el recorrido no puede
        // ciclar; el tope esta por si un dato cargado a mano en la base burlara esa
        // validacion, y porque diez generaciones son mas de las que ningun tambo
        // tiene registradas.
        public const int PROFUNDIDAD_MAXIMA = 10;

        // La caravana llega del selector modal, por eso se valida en el handler y no con atributos
        [BindProperty]
        public string? numCaravana { get; set; } = "";

        public Animal animal { get; set; }
        public List<Animal> animales = new List<Animal>();

        // La ascendencia entera, serializada para que la dibuje el navegador. Va toda
        // junta y no de a una generacion por consulta: los animales ya estan en
        // memoria, asi que ir a buscar cada rama al servidor cuando se la despliega
        // seria un viaje de ida y vuelta por un dato que ya se tiene.
        public string arbolJson = "";

        // Cuantas generaciones de ascendencia hay cargadas. La pantalla la usa para no
        // ofrecer "expandir todo" cuando no hay nada mas que expandir.
        public int generaciones = 0;

        public void OnGet(string caravana)
        {
            Controladora unaControladora = new Controladora();
            animales = unaControladora.ListarAnimales();

            // La ficha del animal entra por aca con la caravana puesta, para no obligar
            // a elegir de nuevo en el selector al animal que se venia mirando
            if (caravana != null && caravana != "")
            {
                numCaravana = caravana;
                this.CargarLinaje(unaControladora);
            }
        }

        public void OnPostConsultarLinaje()
        {
            Controladora unaControladora = new Controladora();

            // Recargar la lista de animales para el select
            animales = unaControladora.ListarAnimales();

            numCaravana = Request.Form["numCaravana"];

            this.CargarLinaje(unaControladora);

            if (animal == null)
            {
                ModelState.AddModelError(string.Empty, "Seleccione un animal!");
            }
        }

        private void CargarLinaje(Controladora pControladoraDominio)
        {
            if (numCaravana == null || numCaravana == "")
            {
                return;
            }

            // El recorrido de la ascendencia se resuelve en memoria sobre la cache
            animal = pControladoraDominio.ObtenerLinaje(numCaravana);

            if (animal == null)
            {
                return;
            }

            NodoLinaje unNodo = this.ArmarNodo(pControladoraDominio, animal, 0);
            generaciones = this.ContarGeneraciones(unNodo) - 1;

            // El serializador escapa por su cuenta los caracteres que tienen sentido en
            // HTML, asi que el texto puede ir tal cual adentro de la etiqueta script
            arbolJson = JsonSerializer.Serialize(unNodo);
        }

        // Arma el nodo del animal y, colgando de el, los de su madre y su padre. Un
        // progenitor no registrado se devuelve como nulo: la pantalla lo dibuja como
        // un casillero vacio, que es informacion -ahi se corta el registro- y no un
        // hueco.
        private NodoLinaje ArmarNodo(Controladora pControladoraDominio, Animal pAnimal, int pNivel)
        {
            NodoLinaje unNodo = new NodoLinaje();

            unNodo.id = pAnimal.IdAnimal;
            unNodo.caravana = pAnimal.NumCaravana;
            unNodo.sexo = pAnimal is Hembra ? "H" : "M";
            unNodo.raza = pAnimal.Raza != null ? pAnimal.Raza.Nombre : "";
            unNodo.categoria = pAnimal.Categoria != null ? pAnimal.Categoria.Nombre : "";
            unNodo.nacimiento = pAnimal.FechaNacimiento.ToShortDateString();
            unNodo.edadMeses = pControladoraDominio.CalcularEdadMeses(pAnimal);
            unNodo.activo = pAnimal.Activo;
            unNodo.motivoBaja = pAnimal.MotivoBaja ?? "";
            unNodo.foto = Controladora.UrlFoto(pAnimal);

            if (pAnimal is Hembra)
            {
                Hembra unaHembra = (Hembra)pAnimal;
                unNodo.partos = unaHembra.NumeroPartos;
                unNodo.estado = unaHembra.EstadoProductivo;
            }
            else
            {
                Macho unMacho = (Macho)pAnimal;
                unNodo.estado = unMacho.EnPie ? "En pie" : "Toro de catalogo";
            }

            if (pNivel < PROFUNDIDAD_MAXIMA)
            {
                if (pAnimal.Madre != null)
                {
                    unNodo.madre = this.ArmarNodo(pControladoraDominio, pAnimal.Madre, pNivel + 1);
                }
                if (pAnimal.Padre != null)
                {
                    unNodo.padre = this.ArmarNodo(pControladoraDominio, pAnimal.Padre, pNivel + 1);
                }
            }

            return unNodo;
        }

        // Profundidad del arbol contando al propio animal, o sea la rama mas larga
        private int ContarGeneraciones(NodoLinaje pNodo)
        {
            if (pNodo == null)
            {
                return 0;
            }

            int vMadre = this.ContarGeneraciones(pNodo.madre);
            int vPadre = this.ContarGeneraciones(pNodo.padre);

            return 1 + (vMadre > vPadre ? vMadre : vPadre);
        }

        // Un animal del arbol, con lo que hace falta para dibujar el nodo y para
        // llenar el panel lateral sin volver a consultar el servidor. Son propiedades
        // y no campos porque el serializador solo mira las propiedades, y van en
        // minuscula porque asi se leen despues desde el JavaScript.
        public class NodoLinaje
        {
            public int id { get; set; } = 0;
            public string caravana { get; set; } = "";
            public string sexo { get; set; } = "H";
            public string raza { get; set; } = "";
            public string categoria { get; set; } = "";
            public string nacimiento { get; set; } = "";
            public int edadMeses { get; set; } = 0;
            public bool activo { get; set; } = true;
            public string motivoBaja { get; set; } = "";
            public string foto { get; set; } = "";

            // Numero de partos de la hembra. En el macho queda en cero y no se muestra.
            public int partos { get; set; } = 0;

            // Estado productivo de la hembra, o si el macho integra el rodeo
            public string estado { get; set; } = "";

            // Nulos cuando el progenitor no esta registrado
            public NodoLinaje madre { get; set; } = null;
            public NodoLinaje padre { get; set; } = null;
        }
    }
}
