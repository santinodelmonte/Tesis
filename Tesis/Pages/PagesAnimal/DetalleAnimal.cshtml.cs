using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tesis.Dominio;

namespace Tesis.Pages.PagesAnimal
{
    // Ficha integral del animal: todo lo que el sistema sabe de el, en una pantalla.
    //
    // Antes mostraba solo los datos de identificacion y el linaje. Para saber en que
    // lactancia estaba, cuando fue el ultimo servicio o si tenia la leche en descarte
    // habia que recorrer cuatro modulos. Es la pantalla que se mira parado frente a la
    // vaca, asi que junta lo productivo, lo reproductivo y lo sanitario.
    public class DetalleAnimalModel : PageModel
    {
        public Animal animal { get; set; }
        public int edadMeses = 0;

        // RF1.9: nombre de la categoria que corresponde, si la guardada quedo vieja
        public string categoriaSugerida = "";

        // Productivo
        public Lactancia lactanciaActual = null;
        public int diasEnLeche = 0;
        public double produccionEstimada = 0;
        public double promedioDiario = 0;
        public double proyeccion305 = 0;
        public List<ControlDiario> controles = new List<ControlDiario>();
        public List<Lactancia> lactancias = new List<Lactancia>();

        // Reproductivo
        public Servicio servicioVigente = null;
        public Tacto ultimoTacto = null;
        public Parto ultimoParto = null;
        public int diasAbiertos = 0;
        public bool diasAbiertosCerrados = false;
        public int intervaloEntrePartos = 0;
        public int serviciosDesdeParto = 0;
        public List<Parto> partos = new List<Parto>();
        public List<Celo> celos = new List<Celo>();

        // Sanitario
        public List<Diagnostico> diagnosticos = new List<Diagnostico>();
        public List<Tratamiento> tratamientos = new List<Tratamiento>();
        public List<Vacunacion> vacunaciones = new List<Vacunacion>();
        public List<Descorne> descornes = new List<Descorne>();
        public DateTime finDescarte = DateTime.MinValue;
        public List<ProcedimientoPendiente> pendientesSanitarios = new List<ProcedimientoPendiente>();

        // Descendencia registrada
        public List<Animal> hijos = new List<Animal>();

        // La pantalla acepta el id o la caravana: el buscador de la barra superior manda
        // la caravana, que es lo que la encargada tiene a mano.
        public void OnGet(int id, string caravana)
        {
            this.CargarDetalle(id, caravana);
        }

        public IActionResult OnPostActualizarCategoria(int id)
        {
            Controladora unaControladora = new Controladora();
            unaControladora.ListarAnimales();
            unaControladora.ActualizarCategoria(id);

            return RedirectToPage(new { id = id });
        }

        // La baja es un dato mas y se puede haber cargado por error: reactivar devuelve
        // el animal al rodeo con todo su historial.
        public IActionResult OnPostReactivar(int id)
        {
            Controladora unaControladora = new Controladora();
            unaControladora.ListarAnimales();

            Animal unAnimal = unaControladora.BuscarAnimal(id);
            if (unAnimal != null)
            {
                unaControladora.ReactivarAnimal(unAnimal.NumCaravana);
            }

            return RedirectToPage(new { id = id });
        }

        private void CargarDetalle(int pId, string pCaravana)
        {
            Controladora unaControladora = new Controladora();
            unaControladora.ListarAnimales();

            animal = pId != 0
                ? unaControladora.BuscarAnimal(pId)
                : unaControladora.BuscarAnimalXCaravana(pCaravana ?? "");

            if (animal == null)
            {
                return;
            }

            edadMeses = unaControladora.CalcularEdadMeses(animal);

            if (!unaControladora.AplicaCategoria(animal.Categoria, animal))
            {
                Categoria unaCategoria = unaControladora.CalcularCategoria(animal);
                if (unaCategoria != null)
                {
                    categoriaSugerida = unaCategoria.Nombre;
                }
            }

            this.CargarSanidad(unaControladora);
            this.CargarDescendencia(unaControladora);

            if (animal is Hembra)
            {
                this.CargarProduccion(unaControladora, (Hembra)animal);
                this.CargarReproduccion(unaControladora, (Hembra)animal);
            }
        }

        private void CargarProduccion(Controladora pControladoraDominio, Hembra pHembra)
        {
            lactancias = pControladoraDominio.FiltrarLactanciasXHembra(pHembra.IdAnimal);
            lactanciaActual = pControladoraDominio.LactanciaActual(pHembra);

            if (lactanciaActual == null)
            {
                return;
            }

            diasEnLeche = pControladoraDominio.DiasEnLeche(lactanciaActual);
            produccionEstimada = pControladoraDominio.EstimarProduccionLactancia(lactanciaActual);
            promedioDiario = pControladoraDominio.PromedioDiarioLactancia(lactanciaActual);
            proyeccion305 = pControladoraDominio.ProyectarProduccion305(lactanciaActual);
            controles = pControladoraDominio.ListarControlesDiarios(lactanciaActual);
        }

        private void CargarReproduccion(Controladora pControladoraDominio, Hembra pHembra)
        {
            pControladoraDominio.ListarServicios();
            pControladoraDominio.ListarTactos();

            servicioVigente = pControladoraDominio.ServicioVigente(pHembra);
            ultimoTacto = pControladoraDominio.UltimoTacto(servicioVigente);
            ultimoParto = pControladoraDominio.UltimoParto(pHembra);

            diasAbiertos = pControladoraDominio.DiasAbiertos(pHembra);
            diasAbiertosCerrados = pControladoraDominio.TieneDiasAbiertosCerrados(pHembra);
            intervaloEntrePartos = pControladoraDominio.IntervaloEntrePartos(pHembra);
            serviciosDesdeParto = pControladoraDominio.ServiciosDesdeUltimoParto(pHembra);

            partos = pControladoraDominio.FiltrarPartosXHembra(pHembra.IdAnimal);
            celos = pControladoraDominio.FiltrarCelosXHembra(pHembra.IdAnimal);
        }

        private void CargarSanidad(Controladora pControladoraDominio)
        {
            diagnosticos = pControladoraDominio.FiltrarDiagnosticosXAnimal(animal.IdAnimal);
            tratamientos = pControladoraDominio.FiltrarTratamientosXAnimal(animal.IdAnimal);
            vacunaciones = pControladoraDominio.FiltrarVacunacionesXAnimal(animal.IdAnimal);
            descornes = pControladoraDominio.FiltrarDescornesXAnimal(animal.IdAnimal);

            if (pControladoraDominio.TieneDescarteVigente(animal))
            {
                finDescarte = pControladoraDominio.FechaFinDescarte(animal);
            }

            // Los procedimientos que los planes activos le exigen a este animal
            foreach (ProcedimientoPendiente unPendiente in pControladoraDominio.ObtenerCalendarioSanitario(
                pControladoraDominio.ObtenerConfiguracion().DiasAnticipacionSanitaria))
            {
                if (unPendiente.Animal.IdAnimal == animal.IdAnimal)
                {
                    pendientesSanitarios.Add(unPendiente);
                }
            }
        }

        private void CargarDescendencia(Controladora pControladoraDominio)
        {
            foreach (Animal unAnimal in pControladoraDominio.ListarAnimales())
            {
                bool vEsHijo = (unAnimal.Madre != null && unAnimal.Madre.IdAnimal == animal.IdAnimal)
                    || (unAnimal.Padre != null && unAnimal.Padre.IdAnimal == animal.IdAnimal);

                if (vEsHijo)
                {
                    hijos.Add(unAnimal);
                }
            }
        }
    }
}
