using Tesis.Persistencia;

namespace Tesis.Dominio
{
    public class Controladora
    {
        private pControladora Persistencia;

        // Edad minima a la que un animal puede entrar en servicio y duracion de la
        // gestacion, en meses. La suma es la diferencia minima de edad que puede
        // haber entre un progenitor y su cria.
        //
        // La hembra y el macho entran en servicio a edades distintas. La vaquillona
        // Holando bien criada se sirve entre los 13 y los 15 meses y pare a los 22, asi
        // que exigirle los 15 del toro rechazaba partos legitimos.
        public const int EDAD_MINIMA_SERVICIO_MESES = 15;
        public const int EDAD_MINIMA_SERVICIO_HEMBRA_MESES = 13;
        public const int GESTACION_MESES = 9;

        // Constantes de los modulos 2 y 3. El documento no las fija con numeros: son
        // los valores habituales de un tambo Holando.
        //
        // La gestacion en dias es la que usa el calculo de la fecha probable de parto.
        // GESTACION_MESES queda para la validacion de genealogia del Modulo 1, que
        // razona en meses.
        public const int GESTACION_DIAS = 283;
        public const int DIAS_SECADO_ANTES_PARTO = 60;
        public const int DIAS_ANTICIPACION_SECADO = 15;
        public const int DIAS_ANTICIPACION_PARTO = 15;

        // Topes de coherencia para el control de litros. Una vaca no da cien litros en
        // un turno, y el lote no llega a cien mil.
        public const double LITROS_MAXIMOS_INDIVIDUAL = 100;
        public const double LITROS_MAXIMOS_LOTE = 100000;

        // Constantes de los modulos 4 y 5. El documento tampoco las fija con numeros:
        // CU23 habla de "horizonte de anticipacion" y CU28 de "ventana de anticipacion"
        // sin decir de cuantos dias.
        //
        // El mes es el margen con el que se planifica en el establecimiento: alcanza
        // para comprar el insumo y juntar los animales antes de que el procedimiento se
        // atrase, y para dar de baja una partida antes de que venza en el estante.
        // Las dos pantallas admiten cambiarlo.
        public const int DIAS_ANTICIPACION_SANITARIA = 30;
        public const int DIAS_ANTICIPACION_VENCIMIENTO = 30;

        // Cada aplicacion consume una dosis del biologico (CU21, paso 6).
        public const double UNIDADES_POR_VACUNACION = 1;

        private static List<Raza> mListaRazas = new List<Raza>();
        private static List<Categoria> mListaCategorias = new List<Categoria>();
        private static List<Animal> mListaAnimales = new List<Animal>();
        private static List<Hembra> mListaHembras = new List<Hembra>();
        private static List<Macho> mListaMachos = new List<Macho>();

        private static List<Lactancia> mListaLactancias = new List<Lactancia>();
        private static List<OrdenieLote> mListaOrdeniesLote = new List<OrdenieLote>();
        private static List<OrdenieIndividual> mListaOrdeniesIndividual = new List<OrdenieIndividual>();
        private static List<Celo> mListaCelos = new List<Celo>();
        private static List<Servicio> mListaServicios = new List<Servicio>();
        private static List<Tacto> mListaTactos = new List<Tacto>();
        private static List<Parto> mListaPartos = new List<Parto>();
        private static List<Insumo> mListaInsumos = new List<Insumo>();
        private static List<MovimientoStock> mListaMovimientosStock = new List<MovimientoStock>();
        private static List<PlanSanitario> mListaPlanes = new List<PlanSanitario>();
        private static List<Diagnostico> mListaDiagnosticos = new List<Diagnostico>();
        private static List<Tratamiento> mListaTratamientos = new List<Tratamiento>();
        private static List<Vacunacion> mListaVacunaciones = new List<Vacunacion>();
        private static List<Descorne> mListaDescornes = new List<Descorne>();

        // Marca si esta Controladora ya refresco las listas. No es static: cada
        // peticion arma su propia Controladora y refresca una vez, aunque la pantalla
        // llame a varios Listar. Las altas y las modificaciones actualizan la cache en
        // memoria, asi que un Listar posterior dentro de la misma peticion sigue viendo
        // el dato recien guardado.
        private bool mRefrescado = false;

        public Controladora()
        {
            Persistencia = new pControladora();
        }

        // Refresca todas las listas cache de una sola vez y en orden de dependencia.
        //
        // Todos los Listar lo invocan. Con el Modulo 1 alcanzaba con que cada uno
        // refrescara su propia lista, pero ahora las entidades se referencian entre si
        // -el servicio apunta a una hembra, el ordenie individual a una lactancia- y si
        // cada Listar recargara por su cuenta, dos listas cargadas uno detras del otro
        // terminarian apuntando a objetos distintos que representan la misma fila. El
        // caso concreto: el tacto actualiza la hembra que cuelga de su servicio, y esa
        // hembra tiene que ser la misma que esta en mListaHembras o la actualizacion se
        // escribe sobre un objeto que despues nadie mira.
        //
        // El costo es una decena de consultas por peticion. Con el volumen de un tambo
        // -un rodeo de cientos de animales- es despreciable frente a la alternativa.
        private void Refrescar()
        {
            if (mRefrescado)
            {
                return;
            }
            mRefrescado = true;

            mListaRazas = Persistencia.ListarRazas();
            mListaCategorias = Persistencia.ListarCategorias();
            mListaAnimales = Persistencia.ListarAnimales(mListaRazas, mListaCategorias);
            mListaHembras = Persistencia.ListarHembras(mListaAnimales);
            mListaMachos = Persistencia.ListarMachos(mListaAnimales);

            mListaInsumos = Persistencia.ListarInsumos(mListaMachos);
            mListaMovimientosStock = Persistencia.ListarMovimientosStock(mListaInsumos);

            // Los planes van despues de los insumos y antes de las aplicaciones: el
            // plan referencia al insumo que aplica, y la vacunacion, el tratamiento y
            // el descorne referencian al plan que dan por cumplido.
            mListaPlanes = Persistencia.ListarPlanesSanitarios(mListaInsumos, mListaCategorias);

            mListaLactancias = Persistencia.ListarLactancias(mListaHembras);
            mListaOrdeniesLote = Persistencia.ListarOrdeniesLote(mListaHembras);
            mListaOrdeniesIndividual = Persistencia.ListarOrdeniesIndividual(mListaHembras,
                mListaLactancias, mListaOrdeniesLote);

            mListaCelos = Persistencia.ListarCelos(mListaHembras);
            mListaServicios = Persistencia.ListarServicios(mListaHembras, mListaMachos, mListaInsumos);
            mListaTactos = Persistencia.ListarTactos(mListaServicios);
            mListaPartos = Persistencia.ListarPartos(mListaHembras);

            mListaDiagnosticos = Persistencia.ListarDiagnosticos(mListaAnimales);
            mListaTratamientos = Persistencia.ListarTratamientos(mListaDiagnosticos, mListaInsumos,
                mListaAnimales, mListaPlanes);
            mListaVacunaciones = Persistencia.ListarVacunaciones(mListaAnimales, mListaInsumos, mListaPlanes);
            mListaDescornes = Persistencia.ListarDescornes(mListaAnimales, mListaPlanes);
        }

        #region SEGURIDAD
        // Credenciales fijas del sistema. Un unico par para la encargada del sector.
        // No se escriben aca: las carga Program.cs desde appsettings.json.
        private static string mUsuarioSistema = "";
        private static string mContrasenaSistema = "";

        public static void ConfigurarCredenciales(string pUsuario, string pContrasena)
        {
            mUsuarioSistema = pUsuario;
            mContrasenaSistema = pContrasena;
        }

        public bool ValidarCredenciales(string pUsuario, string pContrasena)
        {
            // Si la configuracion no trajo credenciales no se habilita el acceso,
            // para que una carga incompleta no deje pasar con los campos vacios.
            if (mUsuarioSistema == "" || mContrasenaSistema == "")
            {
                return false;
            }

            if (pUsuario == mUsuarioSistema && pContrasena == mContrasenaSistema)
            {
                return true;
            }
            return false;

        }
        #endregion

        #region RAZAS
        public List<Raza> ListarRazas()
        {
            this.Refrescar();
            return mListaRazas;
        }

        public Raza BuscarRaza(int pId)
        {
            foreach (Raza unaRaza in mListaRazas)
            {
                if (unaRaza.IdRaza == pId)
                {
                    return unaRaza;
                }
            }
            return null;
        }
        #endregion

        #region CATEGORIAS
        public List<Categoria> ListarCategorias()
        {
            this.Refrescar();
            return mListaCategorias;
        }

        public Categoria BuscarCategoria(int pId)
        {
            foreach (Categoria unaCategoria in mListaCategorias)
            {
                if (unaCategoria.IdCategoria == pId)
                {
                    return unaCategoria;
                }
            }
            return null;
        }
        #endregion

        #region ANIMALES
        public List<Animal> ListarAnimales()
        {
            this.Refrescar();
            return mListaAnimales;
        }

        public Animal BuscarAnimal(int pId)
        {
            foreach (Animal unAnimal in mListaAnimales)
            {
                if (unAnimal.IdAnimal == pId)
                {
                    return unAnimal;
                }
            }
            return null;
        }

        public Animal BuscarAnimalXCaravana(string pNumCaravana)
        {
            foreach (Animal unAnimal in mListaAnimales)
            {
                if (unAnimal.NumCaravana == pNumCaravana)
                {
                    return unAnimal;
                }
            }
            return null;
        }

        public bool ExisteCaravana(string pNumCaravana)
        {
            bool flag = false;
            foreach (Animal unAnimal in mListaAnimales)
            {
                if (unAnimal.NumCaravana == pNumCaravana)
                {
                    flag = true;
                }
            }
            return flag;
        }

        public bool AltaAnimal(Animal pAnimal)
        {
            if (this.ValidarGenealogia(pAnimal.IdAnimal, pAnimal.FechaNacimiento,
                pAnimal.Madre, pAnimal.Padre) != "")
            {
                return false;
            }

            if (!this.ExisteCaravana(pAnimal.NumCaravana))
            {
                // La persistencia guarda el animal y su especializacion dentro de una
                // misma transaccion, y le asigna el id que genero la base.
                if (Persistencia.AltaAnimal(pAnimal))
                {
                    if (pAnimal is Hembra)
                    {
                        mListaHembras.Add((Hembra)pAnimal);
                    }
                    else
                    {
                        mListaMachos.Add((Macho)pAnimal);
                    }

                    mListaAnimales.Add(pAnimal);
                    return true;
                }
            }
            return false;

        }

        // Ademas de los datos del animal recibe los de su especializacion: el numero
        // de partos de la hembra y el en pie del macho, que CU3 pide poder corregir
        // y que ademas determinan la categoria.
        public bool ModificarAnimal(int pIdAnimal, string pNumCaravana, DateTime pFechaNacimiento,
            Raza pRaza, Categoria pCategoria, Hembra pMadre, Macho pPadre,
            int pNumeroPartos, bool pEnPie)
        {
            Animal unAnimal = this.BuscarAnimal(pIdAnimal);
            if (unAnimal == null)
            {
                return false;
            }

            // La validacion de unicidad excluye al propio animal
            Animal otroAnimal = this.BuscarAnimalXCaravana(pNumCaravana);
            if (otroAnimal != null && otroAnimal.IdAnimal != pIdAnimal)
            {
                return false;
            }

            if (this.ValidarGenealogia(pIdAnimal, pFechaNacimiento, pMadre, pPadre) != "")
            {
                return false;
            }

            // Se persiste sobre una copia con los datos nuevos. La cache se actualiza
            // recien cuando la escritura salio bien, que es el orden que describen
            // 2.2.3 y 2.2.7: si la base falla, en memoria no queda un dato que no
            // esta guardado.
            Animal unAnimalNuevo = this.CopiarAnimal(unAnimal, pNumCaravana, pFechaNacimiento,
                pRaza, pCategoria, pMadre, pPadre, pNumeroPartos, pEnPie);

            if (!Persistencia.ModificarAnimal(unAnimalNuevo))
            {
                return false;
            }

            unAnimal.NumCaravana = pNumCaravana;
            unAnimal.FechaNacimiento = pFechaNacimiento;
            unAnimal.Raza = pRaza;
            unAnimal.Categoria = pCategoria;
            unAnimal.Madre = pMadre;
            unAnimal.Padre = pPadre;

            if (unAnimal is Hembra)
            {
                ((Hembra)unAnimal).NumeroPartos = pNumeroPartos;
            }
            else
            {
                ((Macho)unAnimal).EnPie = pEnPie;
            }

            return true;

        }

        // RF1.9: reemplaza la categoria almacenada por la que corresponde a la
        // condicion actual del animal. No toca la genealogia, por eso no pasa por
        // ModificarAnimal.
        public bool ActualizarCategoria(int pIdAnimal)
        {
            Animal unAnimal = this.BuscarAnimal(pIdAnimal);
            if (unAnimal == null)
            {
                return false;
            }

            Categoria unaCategoria = this.CalcularCategoria(unAnimal);
            if (unaCategoria == null || this.AplicaCategoria(unAnimal.Categoria, unAnimal))
            {
                return false;
            }

            int vNumeroPartos = unAnimal is Hembra ? ((Hembra)unAnimal).NumeroPartos : 0;
            bool vEnPie = unAnimal is Macho ? ((Macho)unAnimal).EnPie : false;

            Animal unAnimalNuevo = this.CopiarAnimal(unAnimal, unAnimal.NumCaravana,
                unAnimal.FechaNacimiento, unAnimal.Raza, unaCategoria, unAnimal.Madre,
                unAnimal.Padre, vNumeroPartos, vEnPie);

            if (Persistencia.ModificarAnimal(unAnimalNuevo))
            {
                unAnimal.Categoria = unaCategoria;
                return true;
            }
            return false;

        }

        // Arma una copia del animal con los datos nuevos, para poder escribir en la
        // base sin tocar todavia el objeto que esta en la cache.
        private Animal CopiarAnimal(Animal pAnimal, string pNumCaravana, DateTime pFechaNacimiento,
            Raza pRaza, Categoria pCategoria, Hembra pMadre, Macho pPadre,
            int pNumeroPartos, bool pEnPie)
        {
            if (pAnimal is Hembra)
            {
                Hembra unaHembra = (Hembra)pAnimal;
                return new Hembra(pAnimal.IdAnimal, pNumCaravana, pFechaNacimiento, pAnimal.Activo,
                    pAnimal.FechaBaja, pAnimal.MotivoBaja, pRaza, pCategoria, pMadre, pPadre,
                    pNumeroPartos, unaHembra.EstadoProductivo, unaHembra.EstadoReproductivo);
            }

            return new Macho(pAnimal.IdAnimal, pNumCaravana, pFechaNacimiento, pAnimal.Activo,
                pAnimal.FechaBaja, pAnimal.MotivoBaja, pRaza, pCategoria, pMadre, pPadre, pEnPie);
        }

        public bool BajaAnimal(string pNumCaravana, string pMotivoBaja)
        {
            Animal unAnimal = this.BuscarAnimalXCaravana(pNumCaravana);
            if (unAnimal != null && unAnimal.Activo)
            {
                if (Persistencia.BajaAnimal(unAnimal.IdAnimal, pMotivoBaja))
                {
                    // La baja es logica: el animal conserva su historial y su linaje
                    unAnimal.Activo = false;
                    unAnimal.FechaBaja = DateTime.Now;
                    unAnimal.MotivoBaja = pMotivoBaja;
                    return true;
                }
            }
            return false;

        }

        public int CalcularEdadMeses(Animal pAnimal)
        {
            int meses = this.DiferenciaMeses(pAnimal.FechaNacimiento, DateTime.Now);
            if (meses < 0)
            {
                meses = 0;
            }
            return meses;
        }

        private int DiferenciaMeses(DateTime pDesde, DateTime pHasta)
        {
            int meses = ((pHasta.Year - pDesde.Year) * 12) + (pHasta.Month - pDesde.Month);

            // Todavia no se cumplio el mes
            if (pHasta.Day < pDesde.Day)
            {
                meses = meses - 1;
            }
            return meses;
        }

        // Recorre la descendencia completa del animal, no solo los hijos directos.
        // Sirve para no dejar que un descendiente termine siendo su progenitor.
        public List<Animal> ListarDescendencia(Animal pAnimal)
        {
            List<Animal> _listaDescendencia = new List<Animal>();
            if (pAnimal != null)
            {
                this.AgregarHijos(pAnimal, _listaDescendencia);
            }
            return _listaDescendencia;
        }

        private void AgregarHijos(Animal pAnimal, List<Animal> pListaDescendencia)
        {
            foreach (Animal unAnimal in mListaAnimales)
            {
                bool esHijo = (unAnimal.Madre != null && unAnimal.Madre.IdAnimal == pAnimal.IdAnimal)
                    || (unAnimal.Padre != null && unAnimal.Padre.IdAnimal == pAnimal.IdAnimal);

                // El control de repetidos ademas corta el recorrido si los datos ya
                // vienen con un ciclo cargado de antes
                if (esHijo && this.BuscarEnLista(pListaDescendencia, unAnimal.IdAnimal) == null)
                {
                    pListaDescendencia.Add(unAnimal);
                    this.AgregarHijos(unAnimal, pListaDescendencia);
                }
            }
        }

        private Animal BuscarEnLista(List<Animal> pLista, int pIdAnimal)
        {
            foreach (Animal unAnimal in pLista)
            {
                if (unAnimal.IdAnimal == pIdAnimal)
                {
                    return unAnimal;
                }
            }
            return null;
        }

        // Devuelve el motivo por el que la genealogia elegida no sirve, o una cadena
        // vacia si esta bien. La usan el alta y la modificacion antes de guardar.
        public string ValidarGenealogia(int pIdAnimal, DateTime pFechaNacimiento,
            Hembra pMadre, Macho pPadre)
        {
            // 1. Un animal no puede ser progenitor de si mismo
            if (pMadre != null && pMadre.IdAnimal == pIdAnimal)
            {
                return "Un animal no puede ser su propia madre!";
            }
            if (pPadre != null && pPadre.IdAnimal == pIdAnimal)
            {
                return "Un animal no puede ser su propio padre!";
            }

            // 2. El progenitor elegido no puede descender del animal: seria un ciclo
            Animal unAnimal = this.BuscarAnimal(pIdAnimal);
            if (unAnimal != null)
            {
                List<Animal> _listaDescendencia = this.ListarDescendencia(unAnimal);

                if (pMadre != null && this.BuscarEnLista(_listaDescendencia, pMadre.IdAnimal) != null)
                {
                    return "La madre elegida desciende del animal!";
                }
                if (pPadre != null && this.BuscarEnLista(_listaDescendencia, pPadre.IdAnimal) != null)
                {
                    return "El padre elegido desciende del animal!";
                }
            }

            // 3. El progenitor tiene que haber nacido con la antelacion suficiente
            // para haber estado en condiciones de servicio: la edad minima al
            // servicio mas los meses de gestacion.
            int vMesesMinimosMadre = EDAD_MINIMA_SERVICIO_HEMBRA_MESES + GESTACION_MESES;
            int vMesesMinimosPadre = EDAD_MINIMA_SERVICIO_MESES + GESTACION_MESES;

            if (pMadre != null && this.DiferenciaMeses(pMadre.FechaNacimiento, pFechaNacimiento) < vMesesMinimosMadre)
            {
                return "La madre tiene que haber nacido al menos " + vMesesMinimosMadre + " meses antes que la cria!";
            }
            if (pPadre != null && this.DiferenciaMeses(pPadre.FechaNacimiento, pFechaNacimiento) < vMesesMinimosPadre)
            {
                return "El padre tiene que haber nacido al menos " + vMesesMinimosPadre + " meses antes que la cria!";
            }

            return "";
        }

        public Categoria CalcularCategoria(Animal pAnimal)
        {
            int vEdadMeses = this.CalcularEdadMeses(pAnimal);
            string vNombre = "";

            if (pAnimal is Hembra)
            {
                Hembra unaHembra = (Hembra)pAnimal;
                if (unaHembra.NumeroPartos >= 1)
                {
                    vNombre = "Vaca";
                }
                else if (vEdadMeses > 12)
                {
                    vNombre = "Novilla";
                }
                else
                {
                    vNombre = "Ternera";
                }
            }
            else
            {
                Macho unMacho = (Macho)pAnimal;
                if (vEdadMeses > EDAD_MINIMA_SERVICIO_MESES && unMacho.EnPie)
                {
                    vNombre = "Toro";
                }
                else if (vEdadMeses > 12)
                {
                    vNombre = "Novillo";
                }
                else
                {
                    vNombre = "Ternero";
                }
            }

            foreach (Categoria unaCategoria in mListaCategorias)
            {
                if (unaCategoria.Nombre == vNombre)
                {
                    return unaCategoria;
                }
            }
            return null;
        }

        public bool AplicaCategoria(Categoria pCategoria, Animal pAnimal)
        {
            Categoria unaCategoria = this.CalcularCategoria(pAnimal);
            if (unaCategoria != null && pCategoria != null)
            {
                if (unaCategoria.IdCategoria == pCategoria.IdCategoria)
                {
                    return true;
                }
            }
            return false;

        }

        public bool EsHembra(string pNumCaravana)
        {
            Animal unAnimal = this.BuscarAnimalXCaravana(pNumCaravana);
            if (unAnimal != null && unAnimal is Hembra)
            {
                return true;
            }
            return false;

        }

        public Animal ObtenerLinaje(string pNumCaravana)
        {
            // El recorrido de la ascendencia se resuelve en memoria sobre la cache
            return this.BuscarAnimalXCaravana(pNumCaravana);
        }

        public List<Animal> ListarAscendencia(Animal pAnimal)
        {
            List<Animal> _listaAscendencia = new List<Animal>();

            if (pAnimal != null)
            {
                _listaAscendencia.Add(pAnimal);

                if (pAnimal.Madre != null)
                {
                    _listaAscendencia.Add(pAnimal.Madre);
                    if (pAnimal.Madre.Madre != null)
                    {
                        _listaAscendencia.Add(pAnimal.Madre.Madre);
                    }
                    if (pAnimal.Madre.Padre != null)
                    {
                        _listaAscendencia.Add(pAnimal.Madre.Padre);
                    }
                }

                if (pAnimal.Padre != null)
                {
                    _listaAscendencia.Add(pAnimal.Padre);
                    if (pAnimal.Padre.Madre != null)
                    {
                        _listaAscendencia.Add(pAnimal.Padre.Madre);
                    }
                    if (pAnimal.Padre.Padre != null)
                    {
                        _listaAscendencia.Add(pAnimal.Padre.Padre);
                    }
                }
            }
            return _listaAscendencia;
        }

        public Animal BuscarAncestroComun(Animal pAnimal, Animal pPareja)
        {
            List<Animal> _ascendenciaAnimal = this.ListarAscendencia(pAnimal);
            List<Animal> _ascendenciaPareja = this.ListarAscendencia(pPareja);

            // Hay parentesco directo si coincide algun ancestro hasta el nivel de los abuelos
            foreach (Animal unAncestro in _ascendenciaAnimal)
            {
                foreach (Animal otroAncestro in _ascendenciaPareja)
                {
                    if (unAncestro.IdAnimal == otroAncestro.IdAnimal)
                    {
                        return unAncestro;
                    }
                }
            }
            return null;
        }

        public bool VerificarConsanguinidad(Animal pAnimal, Animal pPareja)
        {
            // La verificacion es informativa: no bloquea el registro del servicio
            if (this.BuscarAncestroComun(pAnimal, pPareja) != null)
            {
                return true;
            }
            return false;

        }

        public List<Animal> FiltrarAnimalesXRaza(int pIdRaza)
        {
            List<Animal> _listaAnimalesXRaza = new List<Animal>();

            foreach (Animal unAnimal in mListaAnimales)
            {
                if (unAnimal.Raza != null && unAnimal.Raza.IdRaza == pIdRaza)
                {
                    _listaAnimalesXRaza.Add(unAnimal);
                }
            }
            return _listaAnimalesXRaza;
        }

        public List<Animal> FiltrarAnimalesXCategoria(int pIdCategoria)
        {
            List<Animal> _listaAnimalesXCategoria = new List<Animal>();

            foreach (Animal unAnimal in mListaAnimales)
            {
                if (unAnimal.Categoria != null && unAnimal.Categoria.IdCategoria == pIdCategoria)
                {
                    _listaAnimalesXCategoria.Add(unAnimal);
                }
            }
            return _listaAnimalesXCategoria;
        }

        public List<Animal> FiltrarAnimalesXEstado(bool pActivo)
        {
            List<Animal> _listaAnimalesXEstado = new List<Animal>();

            foreach (Animal unAnimal in mListaAnimales)
            {
                if (unAnimal.Activo == pActivo)
                {
                    _listaAnimalesXEstado.Add(unAnimal);
                }
            }
            return _listaAnimalesXEstado;
        }

        public List<Animal> FiltrarAnimales(string pNumCaravana, int pIdRaza, int pIdCategoria,
            int pActivo, int pEdadDesde, int pEdadHasta)
        {
            List<Animal> _listaAnimalesFiltrada = new List<Animal>();

            // Los filtros se encadenan sobre la coleccion en memoria
            foreach (Animal unAnimal in mListaAnimales)
            {
                bool flag = true;

                if (pNumCaravana != null && pNumCaravana != "" && !unAnimal.NumCaravana.Contains(pNumCaravana))
                {
                    flag = false;
                }
                if (pIdRaza > 0 && (unAnimal.Raza == null || unAnimal.Raza.IdRaza != pIdRaza))
                {
                    flag = false;
                }
                if (pIdCategoria > 0 && (unAnimal.Categoria == null || unAnimal.Categoria.IdCategoria != pIdCategoria))
                {
                    flag = false;
                }
                if (pActivo == 1 && !unAnimal.Activo)
                {
                    flag = false;
                }
                if (pActivo == 0 && unAnimal.Activo)
                {
                    flag = false;
                }
                if (pEdadDesde > 0 && this.CalcularEdadMeses(unAnimal) < pEdadDesde)
                {
                    flag = false;
                }
                if (pEdadHasta > 0 && this.CalcularEdadMeses(unAnimal) > pEdadHasta)
                {
                    flag = false;
                }

                if (flag)
                {
                    _listaAnimalesFiltrada.Add(unAnimal);
                }
            }

            for (int i = 0; i < _listaAnimalesFiltrada.Count - 1; i++)
            {
                for (int j = 0; j < _listaAnimalesFiltrada.Count - i - 1; j++)
                {
                    if (string.Compare(_listaAnimalesFiltrada[j].NumCaravana, _listaAnimalesFiltrada[j + 1].NumCaravana) > 0)
                    {
                        // Intercambiar elementos
                        var temp = _listaAnimalesFiltrada[j];
                        _listaAnimalesFiltrada[j] = _listaAnimalesFiltrada[j + 1];
                        _listaAnimalesFiltrada[j + 1] = temp;
                    }
                }
            }

            return _listaAnimalesFiltrada;
        }
        #endregion

        #region HEMBRAS
        public List<Hembra> ListarHembras()
        {
            this.Refrescar();
            return mListaHembras;
        }

        public Hembra BuscarHembra(int pId)
        {
            foreach (Hembra unaHembra in mListaHembras)
            {
                if (unaHembra.IdAnimal == pId)
                {
                    return unaHembra;
                }
            }
            return null;
        }

        public bool AltaHembra(Hembra pHembra)
        {
            Hembra unaHembra = this.BuscarHembra(pHembra.IdAnimal);
            if (unaHembra == null)
            {
                if (Persistencia.AltaHembra(pHembra))
                {
                    mListaHembras.Add(pHembra);
                    return true;
                }
            }
            return false;

        }

        public bool EstaEnLactancia(string pNumCaravana)
        {
            Animal unAnimal = this.BuscarAnimalXCaravana(pNumCaravana);
            if (unAnimal != null && unAnimal is Hembra)
            {
                Hembra unaHembra = (Hembra)unAnimal;
                if (unaHembra.EstadoProductivo == Hembra.EN_LACTANCIA)
                {
                    return true;
                }
            }
            return false;

        }

        // Arma una copia de la hembra con los estados nuevos, para poder escribir en la
        // base sin tocar todavia el objeto que esta en la cache. Es el mismo criterio
        // que usa CopiarAnimal en la modificacion del Modulo 1.
        private Hembra CopiarHembra(Hembra pHembra, int pNumeroPartos, string pEstadoProductivo,
            string pEstadoReproductivo)
        {
            return new Hembra(pHembra.IdAnimal, pHembra.NumCaravana, pHembra.FechaNacimiento,
                pHembra.Activo, pHembra.FechaBaja, pHembra.MotivoBaja, pHembra.Raza,
                pHembra.Categoria, pHembra.Madre, pHembra.Padre, pNumeroPartos,
                pEstadoProductivo, pEstadoReproductivo);
        }

        // RF2.12: lo disparan el parto, que inicia la lactancia, y el secado, que la cierra.
        public bool ModificarEstadoProductivo(int pIdHembra, string pEstadoProductivo)
        {
            Hembra unaHembra = this.BuscarHembra(pIdHembra);
            if (unaHembra == null)
            {
                return false;
            }

            Hembra unaHembraNueva = this.CopiarHembra(unaHembra, unaHembra.NumeroPartos,
                pEstadoProductivo, unaHembra.EstadoReproductivo);

            if (Persistencia.ModificarHembra(unaHembraNueva))
            {
                unaHembra.EstadoProductivo = pEstadoProductivo;
                return true;
            }
            return false;

        }

        // RF3.9: lo disparan el servicio, que la deja servida, el tacto, que la confirma
        // prenada o vacia, y el parto, que la devuelve a vacia.
        public bool ModificarEstadoReproductivo(int pIdHembra, string pEstadoReproductivo)
        {
            Hembra unaHembra = this.BuscarHembra(pIdHembra);
            if (unaHembra == null)
            {
                return false;
            }

            Hembra unaHembraNueva = this.CopiarHembra(unaHembra, unaHembra.NumeroPartos,
                unaHembra.EstadoProductivo, pEstadoReproductivo);

            if (Persistencia.ModificarHembra(unaHembraNueva))
            {
                unaHembra.EstadoReproductivo = pEstadoReproductivo;
                return true;
            }
            return false;

        }

        public List<Hembra> ListarHembrasEnLactancia()
        {
            List<Hembra> _listaEnLactancia = new List<Hembra>();

            foreach (Hembra unaHembra in mListaHembras)
            {
                if (unaHembra.Activo && unaHembra.EstadoProductivo == Hembra.EN_LACTANCIA)
                {
                    _listaEnLactancia.Add(unaHembra);
                }
            }
            return _listaEnLactancia;
        }

        public List<Hembra> ListarHembrasPrenadas()
        {
            List<Hembra> _listaPrenadas = new List<Hembra>();

            foreach (Hembra unaHembra in mListaHembras)
            {
                if (unaHembra.Activo && unaHembra.EstadoReproductivo == Hembra.PRENADA)
                {
                    _listaPrenadas.Add(unaHembra);
                }
            }
            return _listaPrenadas;
        }
        #endregion

        #region MACHOS
        public List<Macho> ListarMachos()
        {
            this.Refrescar();
            return mListaMachos;
        }

        public Macho BuscarMacho(int pId)
        {
            foreach (Macho unMacho in mListaMachos)
            {
                if (unMacho.IdAnimal == pId)
                {
                    return unMacho;
                }
            }
            return null;
        }

        public bool AltaMacho(Macho pMacho)
        {
            Macho unMacho = this.BuscarMacho(pMacho.IdAnimal);
            if (unMacho == null)
            {
                if (Persistencia.AltaMacho(pMacho))
                {
                    mListaMachos.Add(pMacho);
                    return true;
                }
            }
            return false;

        }

        public bool EsToro(Macho pMacho)
        {
            // Puede usarse como reproductor a partir de la edad minima al servicio
            if (pMacho != null && this.CalcularEdadMeses(pMacho) > EDAD_MINIMA_SERVICIO_MESES)
            {
                return true;
            }
            return false;

        }
        #endregion

        #region LACTANCIAS
        public List<Lactancia> ListarLactancias()
        {
            this.Refrescar();
            return mListaLactancias;
        }

        public Lactancia BuscarLactancia(int pId)
        {
            foreach (Lactancia unaLactancia in mListaLactancias)
            {
                if (unaLactancia.IdLactancia == pId)
                {
                    return unaLactancia;
                }
            }
            return null;
        }

        // La fecha de secado en nulo -DateTime.MinValue en memoria- es lo que
        // identifica a la lactancia que sigue en curso.
        public bool LactanciaEstaActiva(Lactancia pLactancia)
        {
            if (pLactancia != null && pLactancia.FechaSecado == DateTime.MinValue)
            {
                return true;
            }
            return false;

        }

        // Si por un error de carga quedara mas de una lactancia abierta, se devuelve la
        // que empezo ultima: es la que corresponde al ciclo vigente. El alta del parto
        // cierra la anterior, asi que en condiciones normales hay una sola.
        public Lactancia LactanciaActual(Hembra pHembra)
        {
            if (pHembra == null)
            {
                return null;
            }

            Lactancia unaLactanciaActual = null;
            foreach (Lactancia unaLactancia in mListaLactancias)
            {
                if (unaLactancia.Animal != null && unaLactancia.Animal.IdAnimal == pHembra.IdAnimal
                    && this.LactanciaEstaActiva(unaLactancia))
                {
                    if (unaLactanciaActual == null || unaLactancia.FechaInicio > unaLactanciaActual.FechaInicio)
                    {
                        unaLactanciaActual = unaLactancia;
                    }
                }
            }
            return unaLactanciaActual;
        }

        // La lactancia que estaba en curso en una fecha dada. La usa el ordenie
        // individual: cargar el control de ayer para una vaca que pario hoy tiene que
        // imputarse a la lactancia vieja y no a la que abrio el parto.
        public Lactancia LactanciaDeLaFecha(Hembra pHembra, DateTime pFecha)
        {
            if (pHembra == null)
            {
                return null;
            }

            Lactancia unaLactanciaDeLaFecha = null;
            foreach (Lactancia unaLactancia in mListaLactancias)
            {
                if (unaLactancia.Animal == null || unaLactancia.Animal.IdAnimal != pHembra.IdAnimal)
                {
                    continue;
                }

                if (unaLactancia.FechaInicio.Date > pFecha.Date)
                {
                    continue;
                }

                // La lactancia cerrada solo cubre hasta el dia del secado
                if (unaLactancia.FechaSecado != DateTime.MinValue && unaLactancia.FechaSecado.Date < pFecha.Date)
                {
                    continue;
                }

                if (unaLactanciaDeLaFecha == null || unaLactancia.FechaInicio > unaLactanciaDeLaFecha.FechaInicio)
                {
                    unaLactanciaDeLaFecha = unaLactancia;
                }
            }
            return unaLactanciaDeLaFecha;
        }

        public List<Lactancia> ListarLactanciasActivas()
        {
            List<Lactancia> _listaActivas = new List<Lactancia>();

            foreach (Lactancia unaLactancia in mListaLactancias)
            {
                if (this.LactanciaEstaActiva(unaLactancia))
                {
                    _listaActivas.Add(unaLactancia);
                }
            }
            return _listaActivas;
        }

        public List<Lactancia> FiltrarLactanciasXHembra(int pIdHembra)
        {
            List<Lactancia> _listaXHembra = new List<Lactancia>();

            foreach (Lactancia unaLactancia in mListaLactancias)
            {
                if (unaLactancia.Animal != null && unaLactancia.Animal.IdAnimal == pIdHembra)
                {
                    _listaXHembra.Add(unaLactancia);
                }
            }
            return _listaXHembra;
        }

        // La primera lactancia de una vaca de la carga inicial no tiene por que ser la
        // numero uno: se cuenta a partir de las que ya estan registradas y del numero
        // de partos que trae la hembra.
        public int ProximoNumeroLactancia(Hembra pHembra)
        {
            int vMaximo = 0;

            foreach (Lactancia unaLactancia in this.FiltrarLactanciasXHembra(pHembra.IdAnimal))
            {
                if (unaLactancia.NumeroLactancia > vMaximo)
                {
                    vMaximo = unaLactancia.NumeroLactancia;
                }
            }

            if (pHembra.NumeroPartos > vMaximo)
            {
                vMaximo = pHembra.NumeroPartos;
            }
            return vMaximo + 1;
        }

        // Apertura manual de la lactancia. No sale de ningun caso de uso del documento:
        // hace falta para las vacas que ya estaban en ordenie cuando arranco el sistema
        // y que, por lo tanto, no tienen un parto registrado que les abra la lactancia.
        public bool AltaLactancia(Lactancia pLactanciaNueva)
        {
            if (pLactanciaNueva.Animal == null)
            {
                return false;
            }

            // Una hembra no puede tener dos lactancias abiertas a la vez
            if (this.LactanciaActual(pLactanciaNueva.Animal) != null)
            {
                return false;
            }

            if (pLactanciaNueva.FechaInicio > DateTime.Now)
            {
                return false;
            }

            if (Persistencia.AltaLactancia(pLactanciaNueva))
            {
                mListaLactancias.Add(pLactanciaNueva);

                // Abrir la lactancia es ponerla en ordenie: sin esto la vaca no entra
                // en el lote de CU8 ni admite el control individual de CU9.
                if (pLactanciaNueva.Animal.EstadoProductivo != Hembra.EN_LACTANCIA)
                {
                    this.ModificarEstadoProductivo(pLactanciaNueva.Animal.IdAnimal, Hembra.EN_LACTANCIA);
                }
                return true;
            }
            return false;

        }

        public bool ModificarLactancia(Lactancia pLactanciaNueva)
        {
            Lactancia unaLactancia = this.BuscarLactancia(pLactanciaNueva.IdLactancia);
            if (unaLactancia == null)
            {
                return false;
            }
            return Persistencia.ModificarLactancia(pLactanciaNueva);
        }

        // Arma una copia de la lactancia con los datos nuevos, para poder escribir en
        // la base sin tocar todavia el objeto que esta en la cache.
        private Lactancia CopiarLactancia(Lactancia pLactancia, DateTime pFechaSecado,
            DateTime pFechaProbableParto)
        {
            return new Lactancia(pLactancia.IdLactancia, pLactancia.NumeroLactancia,
                pLactancia.FechaInicio, pFechaSecado, pFechaProbableParto, pLactancia.Animal);
        }

        // CU12. Cierra la lactancia vigente y pasa la hembra a seca. El estado
        // reproductivo no se toca: una hembra seca puede estar prenada, y de hecho es
        // lo habitual.
        public bool RegistrarSecado(string pNumCaravana, DateTime pFecha)
        {
            Animal unAnimal = this.BuscarAnimalXCaravana(pNumCaravana);
            if (unAnimal == null || !(unAnimal is Hembra))
            {
                return false;
            }

            Hembra unaHembra = (Hembra)unAnimal;
            if (unaHembra.EstadoProductivo != Hembra.EN_LACTANCIA)
            {
                return false;
            }

            Lactancia unaLactancia = this.LactanciaActual(unaHembra);
            if (unaLactancia == null)
            {
                return false;
            }

            if (pFecha > DateTime.Now || pFecha < unaLactancia.FechaInicio)
            {
                return false;
            }

            Lactancia unaLactanciaNueva = this.CopiarLactancia(unaLactancia, pFecha,
                unaLactancia.FechaProbableParto);
            Hembra unaHembraNueva = this.CopiarHembra(unaHembra, unaHembra.NumeroPartos,
                Hembra.SECA, unaHembra.EstadoReproductivo);

            if (Persistencia.RegistrarSecado(unaLactanciaNueva, unaHembraNueva))
            {
                unaLactancia.FechaSecado = pFecha;
                unaHembra.EstadoProductivo = Hembra.SECA;
                return true;
            }
            return false;

        }

        // RF2.10. La fecha recomendada de secado se cuenta hacia atras desde la fecha
        // probable de parto: la vaca necesita ese descanso preparto. Devuelve la fecha
        // minima cuando la hembra no tiene una preniez confirmada, porque sin fecha
        // probable de parto no hay nada que calcular.
        public DateTime CalcularFechaSecado(Hembra pHembra)
        {
            Lactancia unaLactancia = this.LactanciaActual(pHembra);
            if (unaLactancia == null || unaLactancia.FechaProbableParto == DateTime.MinValue)
            {
                return DateTime.MinValue;
            }
            return unaLactancia.FechaProbableParto.AddDays(-DIAS_SECADO_ANTES_PARTO);
        }

        // CU13. Vacas en produccion que ya entraron en la ventana critica para iniciar
        // el secado.
        public List<Hembra> ListarAlertasSecado()
        {
            List<Hembra> _listaAlertas = new List<Hembra>();

            foreach (Hembra unaHembra in this.ListarHembrasEnLactancia())
            {
                DateTime vFechaSecado = this.CalcularFechaSecado(unaHembra);

                if (vFechaSecado != DateTime.MinValue
                    && DateTime.Now.Date >= vFechaSecado.AddDays(-DIAS_ANTICIPACION_SECADO).Date)
                {
                    _listaAlertas.Add(unaHembra);
                }
            }
            return _listaAlertas;
        }

        // Litros acumulados de la lactancia. Solo suma los controles individuales: el
        // ordenie por lote no discrimina por animal.
        public double CalcularProduccionTotal(Lactancia pLactancia)
        {
            if (pLactancia == null)
            {
                return 0;
            }
            return this.SumarLitros(this.FiltrarOrdeniesXLactancia(pLactancia.IdLactancia));
        }
        #endregion

        #region ORDENIES POR LOTE
        public List<OrdenieLote> ListarOrdeniesLote()
        {
            this.Refrescar();
            return mListaOrdeniesLote;
        }

        public OrdenieLote BuscarOrdenieLote(int pId)
        {
            foreach (OrdenieLote unOrdenie in mListaOrdeniesLote)
            {
                if (unOrdenie.IdOrdenieLote == pId)
                {
                    return unOrdenie;
                }
            }
            return null;
        }

        // La fecha y el turno son clave alterna: no puede haber dos lotes del mismo
        // turno del mismo dia.
        public OrdenieLote BuscarOrdenieLoteXFechaTurno(DateTime pFecha, string pTurno)
        {
            foreach (OrdenieLote unOrdenie in mListaOrdeniesLote)
            {
                if (unOrdenie.Fecha.Date == pFecha.Date && unOrdenie.Turno == pTurno)
                {
                    return unOrdenie;
                }
            }
            return null;
        }

        // RF2.3. El tope superior es un control de coherencia: atrapa el error de tipeo
        // que mete un cero de mas.
        public bool ValidarLitros(double pLitros)
        {
            if (pLitros > 0 && pLitros <= LITROS_MAXIMOS_LOTE)
            {
                return true;
            }
            return false;

        }

        // El tope fijo del lote es demasiado laxo para servir de control: un rodeo de
        // doscientas vacas da unos dos mil quinientos litros por turno, asi que un cero
        // de mas pasa igual. El tope real sale de la cantidad de animales ordeniados.
        public bool ValidarLitrosLote(double pLitros, int pCantidadAnimales)
        {
            if (!this.ValidarLitros(pLitros))
            {
                return false;
            }

            int vAnimales = pCantidadAnimales > 0 ? pCantidadAnimales : 1;
            if (pLitros <= vAnimales * LITROS_MAXIMOS_INDIVIDUAL)
            {
                return true;
            }
            return false;

        }

        public bool ValidarLitrosIndividual(double pLitros)
        {
            if (pLitros > 0 && pLitros <= LITROS_MAXIMOS_INDIVIDUAL)
            {
                return true;
            }
            return false;

        }

        // CU8, pasos 2 y 3: las hembras en lactancia, menos las que estan dentro de un
        // periodo de descarte de leche vigente.
        public List<Hembra> ListarAnimalesParaOrdenie()
        {
            List<Hembra> _listaLote = new List<Hembra>();

            foreach (Hembra unaHembra in this.ListarHembrasEnLactancia())
            {
                if (!this.TieneDescarteVigente(unaHembra))
                {
                    _listaLote.Add(unaHembra);
                }
            }
            return _listaLote;
        }

        // Las que quedaron afuera por descarte. La pantalla las muestra aparte porque
        // el curso alternativo 3a de CU8 permite incluirlas a mano.
        public List<Hembra> ListarHembrasEnDescarte()
        {
            List<Hembra> _listaEnDescarte = new List<Hembra>();

            foreach (Hembra unaHembra in this.ListarHembrasEnLactancia())
            {
                if (this.TieneDescarteVigente(unaHembra))
                {
                    _listaEnDescarte.Add(unaHembra);
                }
            }
            return _listaEnDescarte;
        }

        public bool AltaOrdenieLote(OrdenieLote pOrdenieLote)
        {
            if (!this.ValidarLitrosLote(pOrdenieLote.LitrosTotales, pOrdenieLote.Animales.Count))
            {
                return false;
            }

            if (pOrdenieLote.Turno != OrdenieLote.TURNO_1 && pOrdenieLote.Turno != OrdenieLote.TURNO_2)
            {
                return false;
            }

            if (pOrdenieLote.Fecha > DateTime.Now)
            {
                return false;
            }

            if (this.BuscarOrdenieLoteXFechaTurno(pOrdenieLote.Fecha, pOrdenieLote.Turno) != null)
            {
                return false;
            }

            if (Persistencia.AltaOrdenieLote(pOrdenieLote))
            {
                mListaOrdeniesLote.Add(pOrdenieLote);
                return true;
            }
            return false;

        }

        // Correccion del ordenie ya cargado. No sale de ningun caso de uso, pero sin
        // esto un error de tipeo en los litros queda fijo para siempre: la clave alterna
        // de fecha y turno impide volver a cargarlo y no hay baja.
        public bool ModificarOrdenieLote(int pIdOrdenieLote, double pLitrosTotales, List<Hembra> pAnimales)
        {
            OrdenieLote unOrdenie = this.BuscarOrdenieLote(pIdOrdenieLote);
            if (unOrdenie == null)
            {
                return false;
            }

            if (!this.ValidarLitrosLote(pLitrosTotales, pAnimales.Count))
            {
                return false;
            }

            if (pAnimales.Count == 0)
            {
                return false;
            }

            OrdenieLote unOrdenieNuevo = new OrdenieLote(unOrdenie.IdOrdenieLote, unOrdenie.Fecha,
                unOrdenie.Turno, pLitrosTotales, pAnimales);

            if (Persistencia.ModificarOrdenieLote(unOrdenieNuevo))
            {
                unOrdenie.LitrosTotales = pLitrosTotales;
                unOrdenie.Animales = pAnimales;
                return true;
            }
            return false;

        }

        // Litros del turno: los del ordenie masivo mas los de las vacas que se
        // controlaron individualmente ese mismo dia y turno, que no estan incluidas en
        // litros_totales.
        public double CalcularTotalDiario(OrdenieLote pOrdenieLote)
        {
            if (pOrdenieLote == null)
            {
                return 0;
            }

            double vTotal = pOrdenieLote.LitrosTotales;

            foreach (OrdenieIndividual unOrdenie in mListaOrdeniesIndividual)
            {
                if (unOrdenie.Fecha.Date == pOrdenieLote.Fecha.Date && unOrdenie.Turno == pOrdenieLote.Turno)
                {
                    vTotal = vTotal + unOrdenie.Litros;
                }
            }
            return vTotal;
        }

        public List<OrdenieLote> FiltrarOrdeniesLoteXFecha(DateTime pDesde, DateTime pHasta)
        {
            List<OrdenieLote> _listaXFecha = new List<OrdenieLote>();

            foreach (OrdenieLote unOrdenie in mListaOrdeniesLote)
            {
                if (unOrdenie.Fecha.Date >= pDesde.Date && unOrdenie.Fecha.Date <= pHasta.Date)
                {
                    _listaXFecha.Add(unOrdenie);
                }
            }
            return _listaXFecha;
        }
        #endregion

        #region ORDENIES INDIVIDUALES
        public List<OrdenieIndividual> ListarOrdeniesIndividual()
        {
            this.Refrescar();
            return mListaOrdeniesIndividual;
        }

        public OrdenieIndividual BuscarOrdenieIndividual(int pId)
        {
            foreach (OrdenieIndividual unOrdenie in mListaOrdeniesIndividual)
            {
                if (unOrdenie.IdOrdenieInd == pId)
                {
                    return unOrdenie;
                }
            }
            return null;
        }

        // CU9. Se imputa a la lactancia vigente de la hembra y, si el ordenie del lote
        // de esa fecha y turno ya esta cargado, queda ademas enganchado a el.
        public bool AltaOrdenieIndividual(OrdenieIndividual pOrdenie)
        {
            if (pOrdenie.Animal == null)
            {
                return false;
            }

            // Curso de excepcion 2a: solo se controla a las vacas en ordenie
            if (pOrdenie.Animal.EstadoProductivo != Hembra.EN_LACTANCIA)
            {
                return false;
            }

            if (!this.ValidarLitrosIndividual(pOrdenie.Litros))
            {
                return false;
            }

            if (pOrdenie.Turno != OrdenieLote.TURNO_1 && pOrdenie.Turno != OrdenieLote.TURNO_2)
            {
                return false;
            }

            if (pOrdenie.Fecha > DateTime.Now)
            {
                return false;
            }

            // Un animal tiene un solo control por fecha y turno. Sin esto, la doble
            // carga de un dia de control lechero infla la produccion de la lactancia.
            if (this.BuscarOrdenieIndividualXFechaTurno(pOrdenie.Fecha, pOrdenie.Turno,
                pOrdenie.Animal.IdAnimal) != null)
            {
                return false;
            }

            // Se imputa a la lactancia que estaba en curso en la fecha del control, no
            // a la actual: la carga puede ser retroactiva.
            pOrdenie.Lactancia = this.LactanciaDeLaFecha(pOrdenie.Animal, pOrdenie.Fecha);
            if (pOrdenie.Lactancia == null)
            {
                return false;
            }

            pOrdenie.OrdenieLote = this.BuscarOrdenieLoteXFechaTurno(pOrdenie.Fecha, pOrdenie.Turno);

            if (Persistencia.AltaOrdenieIndividual(pOrdenie))
            {
                mListaOrdeniesIndividual.Add(pOrdenie);
                return true;
            }
            return false;

        }

        public OrdenieIndividual BuscarOrdenieIndividualXFechaTurno(DateTime pFecha, string pTurno, int pIdAnimal)
        {
            foreach (OrdenieIndividual unOrdenie in mListaOrdeniesIndividual)
            {
                if (unOrdenie.Fecha.Date == pFecha.Date && unOrdenie.Turno == pTurno
                    && unOrdenie.Animal != null && unOrdenie.Animal.IdAnimal == pIdAnimal)
                {
                    return unOrdenie;
                }
            }
            return null;
        }

        // Litros ya cargados como control individual en ese turno. El ordenie por lote
        // los necesita para poder descontarlos del total del tanque.
        public double SumarLitrosIndividualesDelTurno(DateTime pFecha, string pTurno)
        {
            double vTotal = 0;

            foreach (OrdenieIndividual unOrdenie in mListaOrdeniesIndividual)
            {
                if (unOrdenie.Fecha.Date == pFecha.Date && unOrdenie.Turno == pTurno)
                {
                    vTotal = vTotal + unOrdenie.Litros;
                }
            }
            return vTotal;
        }

        public double SumarLitros(List<OrdenieIndividual> pOrdenies)
        {
            double vTotal = 0;

            foreach (OrdenieIndividual unOrdenie in pOrdenies)
            {
                vTotal = vTotal + unOrdenie.Litros;
            }
            return vTotal;
        }

        public List<OrdenieIndividual> FiltrarOrdeniesXLactancia(int pIdLactancia)
        {
            List<OrdenieIndividual> _listaXLactancia = new List<OrdenieIndividual>();

            foreach (OrdenieIndividual unOrdenie in mListaOrdeniesIndividual)
            {
                if (unOrdenie.Lactancia != null && unOrdenie.Lactancia.IdLactancia == pIdLactancia)
                {
                    _listaXLactancia.Add(unOrdenie);
                }
            }
            return _listaXLactancia;
        }

        public List<OrdenieIndividual> FiltrarOrdeniesIndividualXFecha(DateTime pDesde, DateTime pHasta)
        {
            List<OrdenieIndividual> _listaXFecha = new List<OrdenieIndividual>();

            foreach (OrdenieIndividual unOrdenie in mListaOrdeniesIndividual)
            {
                if (unOrdenie.Fecha.Date >= pDesde.Date && unOrdenie.Fecha.Date <= pHasta.Date)
                {
                    _listaXFecha.Add(unOrdenie);
                }
            }
            return _listaXFecha;
        }

        // CU10. Las tres modalidades del historial. En "Totales" se suman las dos
        // fuentes sin miedo a duplicar: los litros del control individual no estan
        // incluidos en los del lote.
        public const string MODALIDAD_INDIVIDUAL = "Individual";
        public const string MODALIDAD_LOTE = "Lote";
        public const string MODALIDAD_TOTALES = "Totales";

        public double CalcularProduccionEnRango(DateTime pDesde, DateTime pHasta, string pModalidad)
        {
            double vTotal = 0;

            if (pModalidad == MODALIDAD_LOTE || pModalidad == MODALIDAD_TOTALES)
            {
                foreach (OrdenieLote unOrdenie in this.FiltrarOrdeniesLoteXFecha(pDesde, pHasta))
                {
                    vTotal = vTotal + unOrdenie.LitrosTotales;
                }
            }

            if (pModalidad == MODALIDAD_INDIVIDUAL || pModalidad == MODALIDAD_TOTALES)
            {
                vTotal = vTotal + this.SumarLitros(this.FiltrarOrdeniesIndividualXFecha(pDesde, pHasta));
            }

            return vTotal;
        }

        // CU11. El mes calendario completo, sumando obligatoriamente las dos fuentes.
        public double CalcularProduccionMensual(int pMes, int pAnio)
        {
            DateTime vDesde = new DateTime(pAnio, pMes, 1);
            DateTime vHasta = vDesde.AddMonths(1).AddDays(-1);

            return this.CalcularProduccionEnRango(vDesde, vHasta, MODALIDAD_TOTALES);
        }
        #endregion

        #region CELOS
        public List<Celo> ListarCelos()
        {
            this.Refrescar();
            return mListaCelos;
        }

        public Celo BuscarCelo(int pId)
        {
            foreach (Celo unCelo in mListaCelos)
            {
                if (unCelo.IdCelo == pId)
                {
                    return unCelo;
                }
            }
            return null;
        }

        // CU14. La validacion de que el animal sea hembra ya quedo resuelta al armar el
        // objeto: Celo solo admite una Hembra.
        public bool AltaCelo(Celo pCelo)
        {
            if (pCelo.Animal == null)
            {
                return false;
            }

            if (pCelo.Fecha > DateTime.Now)
            {
                return false;
            }

            if (Persistencia.AltaCelo(pCelo))
            {
                mListaCelos.Add(pCelo);
                return true;
            }
            return false;

        }

        public List<Celo> FiltrarCelosXHembra(int pIdHembra)
        {
            List<Celo> _listaXHembra = new List<Celo>();

            foreach (Celo unCelo in mListaCelos)
            {
                if (unCelo.Animal != null && unCelo.Animal.IdAnimal == pIdHembra)
                {
                    _listaXHembra.Add(unCelo);
                }
            }
            return _listaXHembra;
        }
        #endregion

        #region SERVICIOS
        public List<Servicio> ListarServicios()
        {
            this.Refrescar();
            return mListaServicios;
        }

        public Servicio BuscarServicio(int pId)
        {
            foreach (Servicio unServicio in mListaServicios)
            {
                if (unServicio.IdServicio == pId)
                {
                    return unServicio;
                }
            }
            return null;
        }

        // RF3.6. La gestacion se cuenta en dias y no en meses: el promedio bovino son
        // 283 dias, y redondear a nueve meses corre la fecha probable de parto una
        // semana larga.
        public DateTime CalcularFechaParto(DateTime pFechaServicio)
        {
            return pFechaServicio.AddDays(GESTACION_DIAS);
        }

        public bool EsInseminacion(Servicio pServicio)
        {
            if (pServicio != null && pServicio.TipoServicio == Servicio.INSEMINACION)
            {
                return true;
            }
            return false;

        }

        // RF3.3. El toro del rodeo en la monta natural, o el toro de catalogo que aporta
        // la pajuela en la inseminacion. Es lo que permite reconstruir la genealogia de
        // la cria aunque el reproductor no integre el rodeo.
        public Macho ToroDelServicio(Servicio pServicio)
        {
            if (pServicio == null)
            {
                return null;
            }

            if (pServicio.Toro != null)
            {
                return pServicio.Toro;
            }

            if (pServicio.Pajuela != null)
            {
                return pServicio.Pajuela.Toro;
            }
            return null;
        }

        // CU15. El reproductor es uno solo: el toro del rodeo y la pajuela son
        // mutuamente excluyentes. Devuelve el motivo por el que el servicio no se puede
        // registrar, o una cadena vacia si esta bien.
        public string ValidarServicio(Servicio pServicio)
        {
            if (pServicio.Animal == null)
            {
                return "Hay que indicar la hembra que recibe el servicio!";
            }

            if (pServicio.FechaServicio > DateTime.Now)
            {
                return "La fecha del servicio no puede ser posterior a la fecha actual!";
            }

            // Servir una hembra prenada le provoca el aborto. Si la preniez estaba mal
            // confirmada, el camino es registrar el tacto vacio y despues el servicio.
            if (pServicio.Animal.EstadoReproductivo == Hembra.PRENADA)
            {
                return "El animal figura prenado: servirlo le provoca el aborto. "
                    + "Si la preniez estaba mal confirmada, registre primero un tacto con resultado vacia!";
            }

            if (pServicio.TipoServicio != Servicio.MONTA_NATURAL && pServicio.TipoServicio != Servicio.INSEMINACION)
            {
                return "Hay que indicar el tipo de servicio!";
            }

            if (pServicio.Toro != null && pServicio.Pajuela != null)
            {
                return "El servicio tiene un unico reproductor: el toro del rodeo y la pajuela son excluyentes!";
            }

            // Curso de excepcion 5b
            if (pServicio.Toro == null && pServicio.Pajuela == null)
            {
                return "Hay que indicar el reproductor!";
            }

            if (pServicio.TipoServicio == Servicio.MONTA_NATURAL && pServicio.Toro == null)
            {
                return "La monta natural necesita un toro del rodeo!";
            }

            if (pServicio.TipoServicio == Servicio.INSEMINACION)
            {
                if (pServicio.Pajuela == null)
                {
                    return "La inseminacion artificial necesita una pajuela del stock!";
                }
                // Curso de excepcion 5a
                if (pServicio.Pajuela.StockActual < 1)
                {
                    return "La pajuela seleccionada no tiene stock disponible!";
                }
            }

            return "";
        }

        public bool AltaServicio(Servicio pServicio)
        {
            if (this.ValidarServicio(pServicio) != "")
            {
                return false;
            }

            if (pServicio.FechaProbableParto == DateTime.MinValue)
            {
                pServicio.FechaProbableParto = this.CalcularFechaParto(pServicio.FechaServicio);
            }

            Hembra unaHembra = pServicio.Animal;
            Hembra unaHembraNueva = this.CopiarHembra(unaHembra, unaHembra.NumeroPartos,
                unaHembra.EstadoProductivo, Hembra.SERVIDA);

            if (Persistencia.AltaServicio(pServicio, unaHembraNueva))
            {
                unaHembra.EstadoReproductivo = Hembra.SERVIDA;
                mListaServicios.Add(pServicio);
                return true;
            }
            return false;

        }

        // Curso alternativo 7a de CU15: el usuario ajusta la fecha probable de parto
        // que propuso el sistema.
        public bool ModificarServicio(int pIdServicio, DateTime pFechaProbableParto)
        {
            Servicio unServicio = this.BuscarServicio(pIdServicio);
            if (unServicio == null)
            {
                return false;
            }

            if (pFechaProbableParto <= unServicio.FechaServicio)
            {
                return false;
            }

            Servicio unServicioNuevo = new Servicio(unServicio.IdServicio, unServicio.TipoServicio,
                unServicio.FechaServicio, pFechaProbableParto, unServicio.Observaciones,
                unServicio.Animal, unServicio.Toro, unServicio.Pajuela);

            // La fecha recomendada de secado se calcula sobre la lactancia, no sobre el
            // servicio. Si la correccion no baja tambien a la lactancia, ajustar la fecha
            // probable de parto no cambia nada en las alertas de secado.
            //
            // Solo se propaga cuando la preniez en curso es la de este servicio: ajustar
            // un servicio viejo no tiene que tocar la proyeccion vigente.
            Servicio unServicioVigente = this.ServicioVigente(unServicio.Animal);
            Lactancia unaLactancia = this.LactanciaActual(unServicio.Animal);
            Lactancia unaLactanciaNueva = null;

            if (unaLactancia != null && unServicioVigente != null
                && unServicioVigente.IdServicio == unServicio.IdServicio
                && unServicio.Animal.EstadoReproductivo == Hembra.PRENADA)
            {
                unaLactanciaNueva = this.CopiarLactancia(unaLactancia, unaLactancia.FechaSecado,
                    pFechaProbableParto);
            }

            if (Persistencia.ModificarServicio(unServicioNuevo, unaLactanciaNueva))
            {
                unServicio.FechaProbableParto = pFechaProbableParto;
                if (unaLactanciaNueva != null)
                {
                    unaLactancia.FechaProbableParto = pFechaProbableParto;
                }
                return true;
            }
            return false;

        }

        public List<Servicio> FiltrarServiciosXHembra(int pIdHembra)
        {
            List<Servicio> _listaXHembra = new List<Servicio>();

            foreach (Servicio unServicio in mListaServicios)
            {
                if (unServicio.Animal != null && unServicio.Animal.IdAnimal == pIdHembra)
                {
                    _listaXHembra.Add(unServicio);
                }
            }
            return _listaXHembra;
        }

        // El servicio sobre el que trabajan el tacto y el parto: el mas reciente de la
        // hembra posterior a su ultimo parto. El documento habla de un servicio "en
        // estado pendiente", pero la tabla servicios no guarda ningun estado, asi que
        // la vigencia se deduce de las fechas.
        public Servicio ServicioVigente(Hembra pHembra)
        {
            if (pHembra == null)
            {
                return null;
            }

            DateTime vUltimoParto = DateTime.MinValue;
            foreach (Parto unParto in this.FiltrarPartosXHembra(pHembra.IdAnimal))
            {
                if (unParto.FechaParto > vUltimoParto)
                {
                    vUltimoParto = unParto.FechaParto;
                }
            }

            Servicio unServicioVigente = null;
            foreach (Servicio unServicio in this.FiltrarServiciosXHembra(pHembra.IdAnimal))
            {
                if (unServicio.FechaServicio > vUltimoParto)
                {
                    if (unServicioVigente == null || unServicio.FechaServicio > unServicioVigente.FechaServicio)
                    {
                        unServicioVigente = unServicio;
                    }
                }
            }
            return unServicioVigente;
        }

        // Preniez confirmada quiere decir preniez en curso, no preniez que hubo alguna
        // vez: solo cuenta el servicio vigente de cada hembra. Sin esa condicion, cada
        // servicio historico con tacto positivo volvia a aparecer -con su fecha probable
        // de parto ya vencida- apenas la vaca se prenaba de nuevo, y las alertas de
        // parto se llenaban de filas fantasma.
        public List<Servicio> ListarServiciosConPrenez()
        {
            List<Servicio> _listaConPrenez = new List<Servicio>();

            foreach (Servicio unServicio in mListaServicios)
            {
                if (!this.EsPositivo(this.UltimoTacto(unServicio)))
                {
                    continue;
                }

                Servicio unServicioVigente = this.ServicioVigente(unServicio.Animal);
                if (unServicioVigente != null && unServicioVigente.IdServicio == unServicio.IdServicio)
                {
                    _listaConPrenez.Add(unServicio);
                }
            }
            return _listaConPrenez;
        }

        public List<Servicio> FiltrarServiciosXFechaParto(DateTime pDesde, DateTime pHasta)
        {
            List<Servicio> _listaXFechaParto = new List<Servicio>();

            foreach (Servicio unServicio in this.ListarServiciosConPrenez())
            {
                if (unServicio.FechaProbableParto.Date >= pDesde.Date
                    && unServicio.FechaProbableParto.Date <= pHasta.Date)
                {
                    _listaXFechaParto.Add(unServicio);
                }
            }
            return _listaXFechaParto;
        }

        // CU17. Hembras prenadas cuya fecha probable de parto entra en la ventana de
        // alerta. Se devuelven los servicios y no las hembras porque la pantalla
        // necesita mostrar la fecha proyectada, que vive en el servicio.
        public List<Servicio> ListarAlertasParto()
        {
            List<Servicio> _listaAlertas = new List<Servicio>();

            foreach (Servicio unServicio in this.ListarServiciosConPrenez())
            {
                if (unServicio.Animal == null || unServicio.Animal.EstadoReproductivo != Hembra.PRENADA)
                {
                    continue;
                }

                if (unServicio.FechaProbableParto.Date <= DateTime.Now.AddDays(DIAS_ANTICIPACION_PARTO).Date)
                {
                    _listaAlertas.Add(unServicio);
                }
            }
            return _listaAlertas;
        }
        #endregion

        #region TACTOS
        public List<Tacto> ListarTactos()
        {
            this.Refrescar();
            return mListaTactos;
        }

        public Tacto BuscarTacto(int pId)
        {
            foreach (Tacto unTacto in mListaTactos)
            {
                if (unTacto.IdTacto == pId)
                {
                    return unTacto;
                }
            }
            return null;
        }

        public bool EsPositivo(Tacto pTacto)
        {
            if (pTacto != null && pTacto.Resultado == Tacto.PRENADA)
            {
                return true;
            }
            return false;

        }

        public List<Tacto> FiltrarTactosXServicio(int pIdServicio)
        {
            List<Tacto> _listaXServicio = new List<Tacto>();

            foreach (Tacto unTacto in mListaTactos)
            {
                if (unTacto.Servicio != null && unTacto.Servicio.IdServicio == pIdServicio)
                {
                    _listaXServicio.Add(unTacto);
                }
            }
            return _listaXServicio;
        }

        public Tacto UltimoTacto(Servicio pServicio)
        {
            if (pServicio == null)
            {
                return null;
            }

            Tacto unUltimoTacto = null;
            foreach (Tacto unTacto in this.FiltrarTactosXServicio(pServicio.IdServicio))
            {
                if (unUltimoTacto == null || unTacto.FechaTacto > unUltimoTacto.FechaTacto)
                {
                    unUltimoTacto = unTacto;
                }
            }
            return unUltimoTacto;
        }

        // CU16. El tacto mueve el estado reproductivo y nunca el productivo: una vaca en
        // lactancia confirmada prenada sigue produciendo. Con resultado positivo, ademas,
        // baja la fecha probable de parto a la lactancia en curso, que es de donde CU13
        // saca la fecha recomendada de secado.
        public bool AltaTacto(Tacto pTacto)
        {
            if (pTacto.Servicio == null || pTacto.Servicio.Animal == null)
            {
                return false;
            }

            if (pTacto.Resultado != Tacto.PRENADA && pTacto.Resultado != Tacto.VACIA
                && pTacto.Resultado != Tacto.DUDOSA)
            {
                return false;
            }

            if (pTacto.FechaTacto > DateTime.Now || pTacto.FechaTacto < pTacto.Servicio.FechaServicio)
            {
                return false;
            }

            Hembra unaHembra = pTacto.Servicio.Animal;

            // Curso alternativo 5a: el dudoso deja el estado como esta, a la espera de
            // un nuevo control.
            string vEstadoNuevo = unaHembra.EstadoReproductivo;
            if (pTacto.Resultado == Tacto.PRENADA)
            {
                vEstadoNuevo = Hembra.PRENADA;
            }
            else if (pTacto.Resultado == Tacto.VACIA)
            {
                vEstadoNuevo = Hembra.VACIA;
            }

            Hembra unaHembraNueva = this.CopiarHembra(unaHembra, unaHembra.NumeroPartos,
                unaHembra.EstadoProductivo, vEstadoNuevo);

            // La lactancia puede no existir: una vaquillona de primer servicio todavia
            // no tiene ninguna abierta.
            Lactancia unaLactancia = this.LactanciaActual(unaHembra);
            Lactancia unaLactanciaNueva = null;
            DateTime vFechaProbableParto = DateTime.MinValue;

            if (unaLactancia != null && pTacto.Resultado == Tacto.PRENADA)
            {
                vFechaProbableParto = pTacto.Servicio.FechaProbableParto;
                unaLactanciaNueva = this.CopiarLactancia(unaLactancia, unaLactancia.FechaSecado,
                    vFechaProbableParto);
            }

            // Un tacto vacio despues de uno positivo -aborto, o el primero mal leido-
            // tiene que borrar la fecha probable de parto proyectada. Si no, la vaca
            // sigue apareciendo en las alertas de secado y se termina secando por una
            // preniez que ya no existe.
            if (unaLactancia != null && pTacto.Resultado == Tacto.VACIA
                && unaLactancia.FechaProbableParto != DateTime.MinValue)
            {
                unaLactanciaNueva = this.CopiarLactancia(unaLactancia, unaLactancia.FechaSecado,
                    DateTime.MinValue);
            }

            if (Persistencia.AltaTacto(pTacto, unaHembraNueva, unaLactanciaNueva))
            {
                unaHembra.EstadoReproductivo = vEstadoNuevo;
                if (unaLactanciaNueva != null)
                {
                    unaLactancia.FechaProbableParto = vFechaProbableParto;
                }
                mListaTactos.Add(pTacto);
                return true;
            }
            return false;

        }
        #endregion

        #region PARTOS
        public List<Parto> ListarPartos()
        {
            this.Refrescar();
            return mListaPartos;
        }

        public Parto BuscarParto(int pId)
        {
            foreach (Parto unParto in mListaPartos)
            {
                if (unParto.IdParto == pId)
                {
                    return unParto;
                }
            }
            return null;
        }

        public List<Parto> FiltrarPartosXHembra(int pIdHembra)
        {
            List<Parto> _listaXHembra = new List<Parto>();

            foreach (Parto unParto in mListaPartos)
            {
                if (unParto.Madre != null && unParto.Madre.IdAnimal == pIdHembra)
                {
                    _listaXHembra.Add(unParto);
                }
            }
            return _listaXHembra;
        }

        // RF3.3. El padre que el sistema propone para la cria: el toro del servicio del
        // que viene la preniez. El usuario puede cambiarlo desde la pantalla.
        public Macho PadreSugerido(Hembra pMadre)
        {
            return this.ToroDelServicio(this.ServicioVigente(pMadre));
        }

        // CU18. El parto actua sobre los dos ejes a la vez: cierra el ciclo reproductivo
        // devolviendo la hembra a vacia e inicia el productivo llevandola a lactancia.
        // Ademas da de alta la cria y abre la lactancia nueva.
        // Recibe una lista de crias y no una sola: alrededor del cuatro por ciento de
        // los partos Holando son dobles. El parto es uno solo -la madre suma un parto y
        // abre una lactancia- aunque nazcan dos terneros.
        public bool AltaParto(Parto pParto, List<Animal> pListaCrias)
        {
            if (pParto.Madre == null || pListaCrias == null || pListaCrias.Count == 0)
            {
                return false;
            }

            if (pParto.FechaParto > DateTime.Now)
            {
                return false;
            }

            if (pParto.TipoParto != Parto.NORMAL && pParto.TipoParto != Parto.DISTOCICO
                && pParto.TipoParto != Parto.CESAREA)
            {
                return false;
            }

            Hembra unaMadre = pParto.Madre;

            // Un animal dado de baja no pare: si figura inactivo, el dato esta mal
            if (!unaMadre.Activo)
            {
                return false;
            }

            foreach (Animal unaCria in pListaCrias)
            {
                // Curso de excepcion 5a
                if (this.ExisteCaravana(unaCria.NumCaravana))
                {
                    return false;
                }

                // Las crias del mismo parto tampoco pueden repetir caravana entre si
                foreach (Animal otraCria in pListaCrias)
                {
                    if (!ReferenceEquals(unaCria, otraCria) && unaCria.NumCaravana == otraCria.NumCaravana)
                    {
                        return false;
                    }
                }

                // La cria nace de este parto: la fecha de nacimiento es la del parto y
                // la madre es la vaca que pario.
                unaCria.FechaNacimiento = pParto.FechaParto;
                unaCria.Madre = unaMadre;

                if (unaCria.Categoria == null)
                {
                    unaCria.Categoria = this.CalcularCategoria(unaCria);
                }

                if (unaCria.Raza == null || unaCria.Categoria == null)
                {
                    return false;
                }

                if (this.ValidarGenealogia(unaCria.IdAnimal, unaCria.FechaNacimiento,
                    unaCria.Madre, unaCria.Padre) != "")
                {
                    return false;
                }
            }

            // La madre suma un parto, vuelve al ordenie y queda vacia
            int vNumeroPartos = unaMadre.NumeroPartos + 1;
            Hembra unaMadreNueva = this.CopiarHembra(unaMadre, vNumeroPartos,
                Hembra.EN_LACTANCIA, Hembra.VACIA);

            // El parto cierra la lactancia anterior si quedo abierta. Pasa cuando no se
            // registro el secado, que es de los olvidos mas comunes: sin este cierre la
            // vaca queda con dos lactancias en curso y los ordenies siguientes se
            // imputan a la vieja.
            Lactancia unaLactanciaAnterior = this.LactanciaActual(unaMadre);
            Lactancia unaLactanciaCerrada = null;

            if (unaLactanciaAnterior != null)
            {
                unaLactanciaCerrada = this.CopiarLactancia(unaLactanciaAnterior, pParto.FechaParto,
                    unaLactanciaAnterior.FechaProbableParto);
            }

            // La lactancia que abre el parto. Todavia no hay preniez nueva, asi que no
            // hay fecha probable de parto que proyectar.
            Lactancia unaLactancia = new Lactancia(0, this.ProximoNumeroLactancia(unaMadre),
                pParto.FechaParto, DateTime.MinValue, DateTime.MinValue, unaMadre);

            if (!Persistencia.AltaParto(pParto, pListaCrias, unaLactancia, unaMadreNueva, unaLactanciaCerrada))
            {
                return false;
            }

            unaMadre.NumeroPartos = vNumeroPartos;
            unaMadre.EstadoProductivo = Hembra.EN_LACTANCIA;
            unaMadre.EstadoReproductivo = Hembra.VACIA;

            if (unaLactanciaCerrada != null)
            {
                unaLactanciaAnterior.FechaSecado = pParto.FechaParto;
            }

            mListaPartos.Add(pParto);
            mListaLactancias.Add(unaLactancia);

            foreach (Animal unaCria in pListaCrias)
            {
                mListaAnimales.Add(unaCria);

                if (unaCria is Hembra)
                {
                    mListaHembras.Add((Hembra)unaCria);
                }
                else
                {
                    mListaMachos.Add((Macho)unaCria);
                }
            }

            // RF1.9: el parto que estrena a la vaca la saca de novilla. Como la
            // categoria vive en animales y no en hembras, hay que actualizarla aparte.
            this.ActualizarCategoria(unaMadre.IdAnimal);

            return true;
        }
        #endregion

        #region INSUMOS
        // Modulo 5: Control de Insumos y Stock (CU25 a CU29).
        //
        // El stock disponible se guarda en insumos.stock_actual y lo mueven los
        // ingresos de partida y los egresos automaticos de la inseminacion, el
        // tratamiento y la vacunacion. Todo lo demas -si el stock es critico, cuanto
        // queda de cada partida, el saldo despues de un movimiento- es derivado y se
        // calcula en el momento de la consulta, que es lo que pide 2.2.5.2.
        public List<Insumo> ListarInsumos()
        {
            this.Refrescar();
            return mListaInsumos;
        }

        public Insumo BuscarInsumo(int pId)
        {
            foreach (Insumo unInsumo in mListaInsumos)
            {
                if (unInsumo.IdInsumo == pId)
                {
                    return unInsumo;
                }
            }
            return null;
        }

        // Los tres listados filtrados recorren ListarInsumos y no la lista cache
        // directamente: asi refrescan igual que cualquier otro Listar, y una pantalla
        // que solo pide pajuelas o solo productos sanitarios no depende de que otra
        // haya cargado la cache antes.
        public List<Insumo> ListarPajuelas()
        {
            List<Insumo> _listaPajuelas = new List<Insumo>();

            foreach (Insumo unInsumo in this.ListarInsumos())
            {
                if (unInsumo.TipoInsumo == Insumo.PAJUELA)
                {
                    _listaPajuelas.Add(unInsumo);
                }
            }
            return _listaPajuelas;
        }

        public List<Insumo> ListarInsumosSanitarios()
        {
            List<Insumo> _listaSanitarios = new List<Insumo>();

            foreach (Insumo unInsumo in this.ListarInsumos())
            {
                if (unInsumo.TipoInsumo != Insumo.PAJUELA)
                {
                    _listaSanitarios.Add(unInsumo);
                }
            }
            return _listaSanitarios;
        }

        // La vacunacion aplica una vacuna y no cualquier producto sanitario, asi que la
        // pantalla de CU21 ofrece solo ese tipo.
        public List<Insumo> ListarInsumosXTipo(string pTipoInsumo)
        {
            List<Insumo> _listaXTipo = new List<Insumo>();

            foreach (Insumo unInsumo in this.ListarInsumos())
            {
                if (unInsumo.TipoInsumo == pTipoInsumo)
                {
                    _listaXTipo.Add(unInsumo);
                }
            }
            return _listaXTipo;
        }

        // CU25, paso 3: el usuario declara si el insumo es nuevo o si repone uno
        // existente. El mismo producto no se da de alta dos veces; la segunda compra
        // entra como partida del insumo que ya existe.
        public bool ExisteInsumo(string pNombre, string pTipoInsumo)
        {
            foreach (Insumo unInsumo in mListaInsumos)
            {
                if (unInsumo.Nombre == pNombre && unInsumo.TipoInsumo == pTipoInsumo)
                {
                    return true;
                }
            }
            return false;

        }

        public bool AltaInsumo(Insumo pInsumoNuevo)
        {
            if (pInsumoNuevo.Nombre == "" || pInsumoNuevo.TipoInsumo == "")
            {
                return false;
            }

            // La pajuela existe para aportar material genetico: sin toro no sirve para
            // reconstruir la genealogia de la cria.
            if (pInsumoNuevo.TipoInsumo == Insumo.PAJUELA && pInsumoNuevo.Toro == null)
            {
                return false;
            }

            if (pInsumoNuevo.StockMinimo < 0 || pInsumoNuevo.PeriodoDescarteDias < 0)
            {
                return false;
            }

            if (this.ExisteInsumo(pInsumoNuevo.Nombre, pInsumoNuevo.TipoInsumo))
            {
                return false;
            }

            if (Persistencia.AltaInsumo(pInsumoNuevo))
            {
                mListaInsumos.Add(pInsumoNuevo);
                return true;
            }
            return false;

        }

        // CU25: el alta del insumo y el ingreso de la primera partida son un solo
        // tramite. El insumo nace con stock cero y la existencia entra como movimiento,
        // asi la partida queda con su vencimiento (CU28) y el historial de CU29 arranca
        // completo. Sin esto el stock inicial seria un numero sin respaldo.
        public bool RegistrarIngreso(Insumo pInsumoNuevo, double pCantidad, DateTime pFecha,
            DateTime pFechaVencimiento)
        {
            double vCantidadInicial = pCantidad;
            pInsumoNuevo.StockActual = 0;

            if (!this.AltaInsumo(pInsumoNuevo))
            {
                return false;
            }

            if (vCantidadInicial <= 0)
            {
                return true;
            }

            return this.IngresarStock(pInsumoNuevo.IdInsumo, vCantidadInicial, pFecha,
                pFechaVencimiento, "Stock inicial");
        }

        public bool IngresarStock(int pIdInsumo, double pCantidad, DateTime pFecha,
            DateTime pFechaVencimiento, string pMotivo)
        {
            Insumo unInsumo = this.BuscarInsumo(pIdInsumo);
            if (unInsumo == null || pCantidad <= 0)
            {
                return false;
            }

            if (pFecha > DateTime.Now)
            {
                return false;
            }

            MovimientoStock unMovimiento = new MovimientoStock(0, MovimientoStock.INGRESO, pCantidad,
                pFecha, pFechaVencimiento, pMotivo, unInsumo);

            // El movimiento y la suma al stock van en una misma transaccion, adentro de
            // la persistencia. Aca solo se refleja el resultado en la cache.
            if (Persistencia.RegistrarIngresoStock(unMovimiento))
            {
                unInsumo.StockActual = unInsumo.StockActual + pCantidad;
                this.AgregarMovimientoOrdenado(unMovimiento);
                return true;
            }
            return false;

        }

        // CU20 y CU21 lo consultan antes de guardar: el egreso automatico no puede
        // dejar el stock en negativo.
        public bool VerificarStock(int pIdInsumo, double pCantidad)
        {
            Insumo unInsumo = this.BuscarInsumo(pIdInsumo);

            if (unInsumo != null && unInsumo.StockActual >= pCantidad)
            {
                return true;
            }
            return false;

        }

        // CU26. El umbral es lo unico del insumo que se configura a mano: el stock lo
        // mueven los movimientos.
        public bool ModificarStockMinimo(int pIdInsumo, double pStockMinimo)
        {
            Insumo unInsumo = this.BuscarInsumo(pIdInsumo);
            if (unInsumo == null || pStockMinimo < 0)
            {
                return false;
            }

            if (Persistencia.ModificarStockMinimo(pIdInsumo, pStockMinimo))
            {
                unInsumo.StockMinimo = pStockMinimo;
                return true;
            }
            return false;

        }

        public bool StockCritico(Insumo pInsumo)
        {
            if (pInsumo != null && pInsumo.StockActual <= pInsumo.StockMinimo)
            {
                return true;
            }
            return false;

        }

        // CU27. La condicion se evalua contra el stock del momento, que ya viene
        // descontado por los egresos automaticos de la inseminacion, el tratamiento y
        // la vacunacion.
        public List<Insumo> ListarAlertasStock()
        {
            List<Insumo> _listaCriticos = new List<Insumo>();

            foreach (Insumo unInsumo in this.ListarInsumos())
            {
                if (this.StockCritico(unInsumo))
                {
                    _listaCriticos.Add(unInsumo);
                }
            }
            return _listaCriticos;
        }

        public List<MovimientoStock> ListarMovimientosStock()
        {
            this.Refrescar();
            return mListaMovimientosStock;
        }

        // CU29. El insumo en cero y el tipo vacio significan "todos"; las fechas en
        // DateTime.MinValue, "sin limite".
        public List<MovimientoStock> FiltrarMovimientos(int pIdInsumo, string pTipoMovimiento,
            DateTime pDesde, DateTime pHasta)
        {
            List<MovimientoStock> _listaFiltrada = new List<MovimientoStock>();

            foreach (MovimientoStock unMovimiento in this.ListarMovimientosStock())
            {
                if (pIdInsumo != 0 && (unMovimiento.Insumo == null
                    || unMovimiento.Insumo.IdInsumo != pIdInsumo))
                {
                    continue;
                }

                if (pTipoMovimiento != "" && unMovimiento.TipoMovimiento != pTipoMovimiento)
                {
                    continue;
                }

                if (pDesde != DateTime.MinValue && unMovimiento.Fecha.Date < pDesde.Date)
                {
                    continue;
                }

                if (pHasta != DateTime.MinValue && unMovimiento.Fecha.Date > pHasta.Date)
                {
                    continue;
                }

                _listaFiltrada.Add(unMovimiento);
            }
            return _listaFiltrada;
        }

        // CU29, paso 5: el stock que quedo despues de cada movimiento. No se almacena
        // -seria un derivado mas- y tampoco se reconstruye sumando desde cero, porque
        // el saldo bueno es el que tiene el insumo hoy: se le restan los movimientos
        // posteriores al que se esta mostrando.
        public double StockResultante(MovimientoStock pMovimiento)
        {
            if (pMovimiento == null || pMovimiento.Insumo == null)
            {
                return 0;
            }

            double vStock = pMovimiento.Insumo.StockActual;

            foreach (MovimientoStock unMovimiento in mListaMovimientosStock)
            {
                if (unMovimiento.Insumo == null
                    || unMovimiento.Insumo.IdInsumo != pMovimiento.Insumo.IdInsumo)
                {
                    continue;
                }

                if (this.EsPosterior(unMovimiento, pMovimiento))
                {
                    vStock = unMovimiento.TipoMovimiento == MovimientoStock.INGRESO
                        ? vStock - unMovimiento.Cantidad
                        : vStock + unMovimiento.Cantidad;
                }
            }
            return vStock;
        }

        // CU28. Las partidas de un insumo son sus movimientos de ingreso. Como los
        // egresos no dicen de que partida salieron -el modelo no los vincula-, el
        // consumo se imputa a la partida que vence primero, que es el orden en el que
        // se usan los productos en el tambo. El remanente que queda es el de esa
        // imputacion.
        public List<PartidaVencimiento> ListarPartidas(Insumo pInsumo)
        {
            List<PartidaVencimiento> _listaPartidas = new List<PartidaVencimiento>();

            if (pInsumo == null)
            {
                return _listaPartidas;
            }

            double vConsumo = 0;

            foreach (MovimientoStock unMovimiento in this.ListarMovimientosStock())
            {
                if (unMovimiento.Insumo == null || unMovimiento.Insumo.IdInsumo != pInsumo.IdInsumo)
                {
                    continue;
                }

                if (unMovimiento.TipoMovimiento == MovimientoStock.EGRESO)
                {
                    vConsumo = vConsumo + unMovimiento.Cantidad;
                }
                else
                {
                    this.InsertarPartidaOrdenada(_listaPartidas,
                        new PartidaVencimiento(unMovimiento, unMovimiento.Cantidad));
                }
            }

            foreach (PartidaVencimiento unaPartida in _listaPartidas)
            {
                if (vConsumo <= 0)
                {
                    break;
                }

                double vDescontado = vConsumo < unaPartida.Remanente ? vConsumo : unaPartida.Remanente;
                unaPartida.Remanente = unaPartida.Remanente - vDescontado;
                vConsumo = vConsumo - vDescontado;
            }

            return _listaPartidas;
        }

        // CU28. Partidas vencidas y por vencer dentro del horizonte, de todos los
        // insumos, ordenadas por fecha de vencimiento. Las partidas agotadas y las que
        // no declaran vencimiento no alertan.
        public List<PartidaVencimiento> ListarAlertasVencimiento(int pAnticipacionDias)
        {
            List<PartidaVencimiento> _listaAlertas = new List<PartidaVencimiento>();
            DateTime vLimite = DateTime.Now.Date.AddDays(pAnticipacionDias);

            foreach (Insumo unInsumo in this.ListarInsumos())
            {
                foreach (PartidaVencimiento unaPartida in this.ListarPartidas(unInsumo))
                {
                    if (unaPartida.Remanente <= 0
                        || unaPartida.Partida.FechaVencimiento == DateTime.MinValue)
                    {
                        continue;
                    }

                    if (unaPartida.Partida.FechaVencimiento.Date <= vLimite)
                    {
                        this.InsertarPartidaOrdenada(_listaAlertas, unaPartida);
                    }
                }
            }
            return _listaAlertas;
        }

        public int DiasParaVencer(PartidaVencimiento pPartida)
        {
            if (pPartida == null || pPartida.Partida.FechaVencimiento == DateTime.MinValue)
            {
                return 0;
            }
            return (int)(pPartida.Partida.FechaVencimiento.Date - DateTime.Now.Date).TotalDays;
        }

        public bool EstaVencida(PartidaVencimiento pPartida)
        {
            if (pPartida != null && pPartida.Partida.FechaVencimiento != DateTime.MinValue
                && pPartida.Partida.FechaVencimiento.Date < DateTime.Now.Date)
            {
                return true;
            }
            return false;

        }

        // La cache viene ordenada por fecha descendente desde la persistencia. El
        // ingreso nuevo se inserta donde le toca y no al final, para que el historial
        // de CU29 no muestre un movimiento fuera de lugar hasta la proxima recarga.
        private void AgregarMovimientoOrdenado(MovimientoStock pMovimiento)
        {
            int vPosicion = 0;

            while (vPosicion < mListaMovimientosStock.Count
                && mListaMovimientosStock[vPosicion].Fecha.Date >= pMovimiento.Fecha.Date)
            {
                vPosicion = vPosicion + 1;
            }
            mListaMovimientosStock.Insert(vPosicion, pMovimiento);
        }

        // Las partidas se ordenan por vencimiento ascendente. La que no declara
        // vencimiento va al final: es la ultima que conviene usar.
        private void InsertarPartidaOrdenada(List<PartidaVencimiento> pLista, PartidaVencimiento pPartida)
        {
            int vPosicion = 0;

            while (vPosicion < pLista.Count
                && this.ClaveVencimiento(pLista[vPosicion]) <= this.ClaveVencimiento(pPartida))
            {
                vPosicion = vPosicion + 1;
            }
            pLista.Insert(vPosicion, pPartida);
        }

        private DateTime ClaveVencimiento(PartidaVencimiento pPartida)
        {
            if (pPartida.Partida.FechaVencimiento == DateTime.MinValue)
            {
                return DateTime.MaxValue;
            }
            return pPartida.Partida.FechaVencimiento.Date;
        }

        private bool EsPosterior(MovimientoStock pMovimiento, MovimientoStock pReferencia)
        {
            if (pMovimiento.Fecha.Date > pReferencia.Fecha.Date)
            {
                return true;
            }

            // Dos movimientos del mismo dia se ordenan por identificador, que es el
            // orden en que se registraron.
            if (pMovimiento.Fecha.Date == pReferencia.Fecha.Date
                && pMovimiento.IdMovimiento > pReferencia.IdMovimiento)
            {
                return true;
            }
            return false;

        }
        #endregion

        #region SANIDAD
        // Modulo 4: Gestion Sanitaria (CU19 a CU24).
        //
        // El diagnostico origina el tratamiento, el tratamiento define el descarte de
        // leche que el paso 3 de CU8 usa para excluir animales del lote de ordenie, y
        // las tres aplicaciones -tratamiento, vacunacion y descorne- declaran que plan
        // sanitario dan por cumplido. De esa declaracion sale el calendario de CU23.
        public List<Diagnostico> ListarDiagnosticos()
        {
            this.Refrescar();
            return mListaDiagnosticos;
        }

        public Diagnostico BuscarDiagnostico(int pId)
        {
            foreach (Diagnostico unDiagnostico in mListaDiagnosticos)
            {
                if (unDiagnostico.IdDiagnostico == pId)
                {
                    return unDiagnostico;
                }
            }
            return null;
        }

        public bool DiagnosticoEstaActivo(Diagnostico pDiagnostico)
        {
            if (pDiagnostico != null && pDiagnostico.Estado != Diagnostico.RESUELTO)
            {
                return true;
            }
            return false;

        }

        public bool AltaDiagnostico(Diagnostico pDiagnosticoNuevo)
        {
            if (pDiagnosticoNuevo.Animal == null || pDiagnosticoNuevo.Enfermedad == "")
            {
                return false;
            }

            if (pDiagnosticoNuevo.FechaDiagnostico > DateTime.Now)
            {
                return false;
            }

            if (Persistencia.AltaDiagnostico(pDiagnosticoNuevo))
            {
                mListaDiagnosticos.Add(pDiagnosticoNuevo);
                return true;
            }
            return false;

        }

        // El diagnostico arranca activo, el alta del tratamiento lo pasa a "En
        // tratamiento" y aca se cierra cuando el animal se recupera. Sin este ultimo
        // paso una afeccion ya resuelta seguiria figurando como abierta y se ofreceria
        // para tratar indefinidamente.
        public bool ResolverDiagnostico(int pIdDiagnostico)
        {
            Diagnostico unDiagnostico = this.BuscarDiagnostico(pIdDiagnostico);
            if (unDiagnostico == null || unDiagnostico.Estado == Diagnostico.RESUELTO)
            {
                return false;
            }

            if (Persistencia.ModificarDiagnostico(pIdDiagnostico, Diagnostico.RESUELTO))
            {
                unDiagnostico.Estado = Diagnostico.RESUELTO;
                return true;
            }
            return false;

        }

        public List<Diagnostico> FiltrarDiagnosticosXAnimal(int pIdAnimal)
        {
            List<Diagnostico> _listaXAnimal = new List<Diagnostico>();

            foreach (Diagnostico unDiagnostico in mListaDiagnosticos)
            {
                if (unDiagnostico.Animal != null && unDiagnostico.Animal.IdAnimal == pIdAnimal)
                {
                    _listaXAnimal.Add(unDiagnostico);
                }
            }
            return _listaXAnimal;
        }

        public List<Tratamiento> ListarTratamientos()
        {
            this.Refrescar();
            return mListaTratamientos;
        }

        public Tratamiento BuscarTratamiento(int pId)
        {
            foreach (Tratamiento unTratamiento in mListaTratamientos)
            {
                if (unTratamiento.IdTratamiento == pId)
                {
                    return unTratamiento;
                }
            }
            return null;
        }

        public DateTime CalcularFechaFin(Tratamiento pTratamiento)
        {
            return pTratamiento.FechaInicio.AddDays(pTratamiento.DiasDuracion);
        }

        // La leche se descarta hasta que termina el tratamiento y ademas pasa el periodo
        // de carencia del producto. Si el insumo no obliga a descartar, no hay fecha.
        public DateTime CalcularDescarte(Tratamiento pTratamiento)
        {
            if (pTratamiento.Insumo == null || pTratamiento.Insumo.PeriodoDescarteDias <= 0)
            {
                return DateTime.MinValue;
            }
            return this.CalcularFechaFin(pTratamiento).AddDays(pTratamiento.Insumo.PeriodoDescarteDias);
        }

        public bool AltaTratamiento(Tratamiento pTratamientoNuevo, double pCantidadInsumo)
        {
            if (pTratamientoNuevo.Insumo == null || pTratamientoNuevo.DiasDuracion <= 0)
            {
                return false;
            }

            // El tratamiento se aplica sobre un animal, venga de un diagnostico o sea
            // preventivo. Sin animal no genera descarte de leche ni cumple ningun plan.
            if (pTratamientoNuevo.Animal == null)
            {
                return false;
            }

            if (pTratamientoNuevo.FechaInicio > DateTime.Now)
            {
                return false;
            }

            // Un tratamiento cumple un plan de desparasitacion: la vacunacion tiene su
            // propio registro y el descorne no consume producto.
            if (pTratamientoNuevo.Plan != null
                && pTratamientoNuevo.Plan.TipoProcedimiento != PlanSanitario.DESPARASITACION)
            {
                return false;
            }

            if (!this.VerificarStock(pTratamientoNuevo.Insumo.IdInsumo, pCantidadInsumo))
            {
                return false;
            }

            // La fecha se propone y el usuario puede ajustarla, por eso solo se calcula
            // cuando llega vacia.
            if (pTratamientoNuevo.FechaFinDescarte == DateTime.MinValue)
            {
                pTratamientoNuevo.FechaFinDescarte = this.CalcularDescarte(pTratamientoNuevo);
            }

            if (Persistencia.AltaTratamiento(pTratamientoNuevo, pCantidadInsumo))
            {
                mListaTratamientos.Add(pTratamientoNuevo);
                return true;
            }
            return false;

        }

        // CU8, paso 3. El animal queda fuera del lote mientras su descarte siga vigente.
        //
        // El tratamiento apunta ahora al animal de forma directa, asi que el preventivo
        // -que va sin diagnostico- tambien bloquea la leche. Con el vinculo anterior,
        // que pasaba por el diagnostico, una desparasitacion no excluia a nadie del
        // lote de ordenie.
        public DateTime FechaFinDescarte(Animal pAnimal)
        {
            DateTime vFechaFin = DateTime.MinValue;

            foreach (Tratamiento unTratamiento in this.FiltrarTratamientosXAnimal(pAnimal.IdAnimal))
            {
                if (unTratamiento.FechaFinDescarte > vFechaFin)
                {
                    vFechaFin = unTratamiento.FechaFinDescarte;
                }
            }
            return vFechaFin;
        }

        public List<Tratamiento> FiltrarTratamientosXAnimal(int pIdAnimal)
        {
            List<Tratamiento> _listaXAnimal = new List<Tratamiento>();

            foreach (Tratamiento unTratamiento in mListaTratamientos)
            {
                if (unTratamiento.Animal != null && unTratamiento.Animal.IdAnimal == pIdAnimal)
                {
                    _listaXAnimal.Add(unTratamiento);
                }
            }
            return _listaXAnimal;
        }

        public bool TieneDescarteVigente(Animal pAnimal)
        {
            DateTime vFechaFin = this.FechaFinDescarte(pAnimal);

            if (vFechaFin != DateTime.MinValue && vFechaFin.Date >= DateTime.Now.Date)
            {
                return true;
            }
            return false;

        }
        #endregion

        #region VACUNACIONES
        public List<Vacunacion> ListarVacunaciones()
        {
            this.Refrescar();
            return mListaVacunaciones;
        }

        public Vacunacion BuscarVacunacion(int pId)
        {
            foreach (Vacunacion unaVacunacion in mListaVacunaciones)
            {
                if (unaVacunacion.IdVacunacion == pId)
                {
                    return unaVacunacion;
                }
            }
            return null;
        }

        // CU21. La aplicacion descuenta una dosis del stock y declara, si corresponde,
        // que plan sanitario da por cumplido. El plan tiene que ser de vacunacion: no
        // se cumple un plan de desparasitacion aplicando una vacuna.
        public bool AltaVacunacion(Vacunacion pVacunacionNueva)
        {
            if (pVacunacionNueva.Animal == null || pVacunacionNueva.Insumo == null)
            {
                return false;
            }

            if (pVacunacionNueva.FechaAplicacion > DateTime.Now)
            {
                return false;
            }

            if (pVacunacionNueva.Plan != null
                && pVacunacionNueva.Plan.TipoProcedimiento != PlanSanitario.VACUNACION)
            {
                return false;
            }

            if (!this.VerificarStock(pVacunacionNueva.Insumo.IdInsumo, UNIDADES_POR_VACUNACION))
            {
                return false;
            }

            if (Persistencia.AltaVacunacion(pVacunacionNueva, UNIDADES_POR_VACUNACION))
            {
                mListaVacunaciones.Add(pVacunacionNueva);
                return true;
            }
            return false;

        }

        public List<Vacunacion> FiltrarVacunacionesXAnimal(int pIdAnimal)
        {
            List<Vacunacion> _listaXAnimal = new List<Vacunacion>();

            foreach (Vacunacion unaVacunacion in mListaVacunaciones)
            {
                if (unaVacunacion.Animal != null && unaVacunacion.Animal.IdAnimal == pIdAnimal)
                {
                    _listaXAnimal.Add(unaVacunacion);
                }
            }
            return _listaXAnimal;
        }
        #endregion

        #region DESCORNES
        public List<Descorne> ListarDescornes()
        {
            this.Refrescar();
            return mListaDescornes;
        }

        public Descorne BuscarDescorne(int pId)
        {
            foreach (Descorne unDescorne in mListaDescornes)
            {
                if (unDescorne.IdDescorne == pId)
                {
                    return unDescorne;
                }
            }
            return null;
        }

        // CU24. El descorne es de aplicacion unica: un animal ya descornado no se
        // vuelve a descornar, y ese registro es lo que hace que el plan deje de
        // exigirlo.
        public bool AltaDescorne(Descorne pDescorneNuevo)
        {
            if (pDescorneNuevo.Animal == null || pDescorneNuevo.Metodo == "")
            {
                return false;
            }

            if (pDescorneNuevo.Fecha > DateTime.Now)
            {
                return false;
            }

            if (pDescorneNuevo.Plan != null
                && pDescorneNuevo.Plan.TipoProcedimiento != PlanSanitario.DESCORNE)
            {
                return false;
            }

            if (this.TieneDescorne(pDescorneNuevo.Animal))
            {
                return false;
            }

            if (Persistencia.AltaDescorne(pDescorneNuevo))
            {
                mListaDescornes.Add(pDescorneNuevo);
                return true;
            }
            return false;

        }

        public bool TieneDescorne(Animal pAnimal)
        {
            if (pAnimal == null)
            {
                return false;
            }
            return this.FiltrarDescornesXAnimal(pAnimal.IdAnimal).Count > 0;
        }

        public List<Descorne> FiltrarDescornesXAnimal(int pIdAnimal)
        {
            List<Descorne> _listaXAnimal = new List<Descorne>();

            foreach (Descorne unDescorne in mListaDescornes)
            {
                if (unDescorne.Animal != null && unDescorne.Animal.IdAnimal == pIdAnimal)
                {
                    _listaXAnimal.Add(unDescorne);
                }
            }
            return _listaXAnimal;
        }

        public List<Descorne> FiltrarDescornesXFecha(DateTime pFechaInicio, DateTime pFechaFin)
        {
            List<Descorne> _listaXFecha = new List<Descorne>();

            foreach (Descorne unDescorne in mListaDescornes)
            {
                if (unDescorne.Fecha.Date >= pFechaInicio.Date && unDescorne.Fecha.Date <= pFechaFin.Date)
                {
                    _listaXFecha.Add(unDescorne);
                }
            }
            return _listaXFecha;
        }
        #endregion

        #region PLANES SANITARIOS
        public List<PlanSanitario> ListarPlanesSanitarios()
        {
            this.Refrescar();
            return mListaPlanes;
        }

        // Solo los planes activos generan pendientes en el calendario (CU22, reglas de
        // negocio). Desactivar un plan es la forma de dejar de exigir un procedimiento
        // sin perder las aplicaciones ya registradas.
        public List<PlanSanitario> ListarPlanesActivos()
        {
            List<PlanSanitario> _listaActivos = new List<PlanSanitario>();

            foreach (PlanSanitario unPlan in this.ListarPlanesSanitarios())
            {
                if (unPlan.Activo)
                {
                    _listaActivos.Add(unPlan);
                }
            }
            return _listaActivos;
        }

        // Los planes que puede cumplir una aplicacion de este tipo, para que la
        // pantalla no ofrezca un plan de descorne al registrar una vacunacion.
        public List<PlanSanitario> ListarPlanesXTipo(string pTipoProcedimiento)
        {
            List<PlanSanitario> _listaXTipo = new List<PlanSanitario>();

            foreach (PlanSanitario unPlan in this.ListarPlanesActivos())
            {
                if (unPlan.TipoProcedimiento == pTipoProcedimiento)
                {
                    _listaXTipo.Add(unPlan);
                }
            }
            return _listaXTipo;
        }

        public PlanSanitario BuscarPlanSanitario(int pId)
        {
            foreach (PlanSanitario unPlan in mListaPlanes)
            {
                if (unPlan.IdPlan == pId)
                {
                    return unPlan;
                }
            }
            return null;
        }

        // El nombre del plan es unico (CU22, curso de excepcion 6a). Al modificar, el
        // propio plan no se cuenta como duplicado: por eso el id se recibe y se excluye.
        public bool ExistePlanSanitario(string pNombre, int pIdPlan)
        {
            foreach (PlanSanitario unPlan in mListaPlanes)
            {
                if (unPlan.Nombre == pNombre && unPlan.IdPlan != pIdPlan)
                {
                    return true;
                }
            }
            return false;

        }

        // Devuelve el motivo por el que el plan no se puede guardar, o el string vacio
        // si esta bien. La pantalla informa cual de las validaciones de CU22 fallo.
        public string ValidarPlanSanitario(PlanSanitario pPlan)
        {
            if (pPlan.Nombre == "")
            {
                return "El nombre del plan es obligatorio.";
            }

            if (pPlan.TipoProcedimiento != PlanSanitario.VACUNACION
                && pPlan.TipoProcedimiento != PlanSanitario.DESPARASITACION
                && pPlan.TipoProcedimiento != PlanSanitario.DESCORNE)
            {
                return "El tipo de procedimiento tiene que ser vacunacion, desparasitacion o descorne.";
            }

            if (pPlan.EdadInicioMeses <= 0)
            {
                return "La edad de inicio tiene que ser un numero positivo de meses.";
            }

            // La periodicidad vacia es valida: significa que el procedimiento se aplica
            // una unica vez en la vida del animal (CU22, curso alternativo 4b). Lo que
            // no se admite es un numero negativo.
            if (pPlan.PeriodicidadDias < PlanSanitario.SIN_PERIODICIDAD)
            {
                return "La periodicidad tiene que ser un numero positivo de dias, o quedar vacia.";
            }

            // El descorne no consume insumo (CU22, curso alternativo 4c). El resto de
            // los procedimientos aplican un producto y sin el no se puede descontar del
            // stock ni calcular el descarte de leche.
            if (pPlan.TipoProcedimiento != PlanSanitario.DESCORNE && pPlan.Insumo == null)
            {
                return "El insumo es obligatorio salvo que el procedimiento sea un descorne.";
            }

            if (this.ExistePlanSanitario(pPlan.Nombre, pPlan.IdPlan))
            {
                return "Ya existe un plan sanitario con ese nombre.";
            }

            return "";
        }

        public bool AltaPlanSanitario(PlanSanitario pPlanNuevo)
        {
            if (this.ValidarPlanSanitario(pPlanNuevo) != "")
            {
                return false;
            }

            if (pPlanNuevo.TipoProcedimiento == PlanSanitario.DESCORNE)
            {
                pPlanNuevo.Insumo = null;
            }

            if (Persistencia.AltaPlanSanitario(pPlanNuevo))
            {
                mListaPlanes.Add(pPlanNuevo);
                return true;
            }
            return false;

        }

        public bool ModificarPlanSanitario(PlanSanitario pPlan)
        {
            PlanSanitario unPlan = this.BuscarPlanSanitario(pPlan.IdPlan);
            if (unPlan == null)
            {
                return false;
            }

            if (this.ValidarPlanSanitario(pPlan) != "")
            {
                return false;
            }

            if (pPlan.TipoProcedimiento == PlanSanitario.DESCORNE)
            {
                pPlan.Insumo = null;
            }

            if (!Persistencia.ModificarPlanSanitario(pPlan))
            {
                return false;
            }

            // La cache se actualiza recien cuando la escritura salio bien, y sobre el
            // objeto que ya estaba en la lista: las vacunaciones, los tratamientos y los
            // descornes ya cargados lo referencian y tienen que seguir viendo el mismo.
            unPlan.Nombre = pPlan.Nombre;
            unPlan.TipoProcedimiento = pPlan.TipoProcedimiento;
            unPlan.PeriodicidadDias = pPlan.PeriodicidadDias;
            unPlan.EdadInicioMeses = pPlan.EdadInicioMeses;
            unPlan.Activo = pPlan.Activo;
            unPlan.Insumo = pPlan.Insumo;
            unPlan.Categorias = pPlan.Categorias;

            return true;
        }
        #endregion

        #region CALENDARIO SANITARIO
        // CU23. El calendario no se almacena: es la diferencia entre lo que los planes
        // activos exigen y lo que efectivamente se aplico, calculada en el momento de
        // la consulta. Es el mismo calculo que va a alimentar el resumen diario del
        // Modulo 6, para que las dos vistas no puedan discrepar.

        // Paso 3: el animal queda alcanzado si esta activo, si pertenece a alguna de
        // las categorias del plan -o el plan no declara ninguna, y entonces alcanza a
        // todo el rodeo- y si supera la edad de inicio.
        public bool PlanAlcanzaAnimal(PlanSanitario pPlan, Animal pAnimal)
        {
            if (pPlan == null || pAnimal == null || !pAnimal.Activo)
            {
                return false;
            }

            if (this.CalcularEdadMeses(pAnimal) < pPlan.EdadInicioMeses)
            {
                return false;
            }

            if (pPlan.Categorias.Count == 0)
            {
                return true;
            }

            foreach (Categoria unaCategoria in pPlan.Categorias)
            {
                if (pAnimal.Categoria != null && pAnimal.Categoria.IdCategoria == unaCategoria.IdCategoria)
                {
                    return true;
                }
            }
            return false;

        }

        // Paso 4. La aplicacion se busca donde corresponde al tipo de procedimiento, y
        // solo cuenta si declara explicitamente este plan: dos planes distintos pueden
        // aplicar el mismo insumo, asi que inferirlo del producto seria adivinar.
        public DateTime UltimaAplicacion(PlanSanitario pPlan, Animal pAnimal)
        {
            DateTime vUltima = DateTime.MinValue;

            if (pPlan == null || pAnimal == null)
            {
                return vUltima;
            }

            if (pPlan.TipoProcedimiento == PlanSanitario.VACUNACION)
            {
                foreach (Vacunacion unaVacunacion in this.FiltrarVacunacionesXAnimal(pAnimal.IdAnimal))
                {
                    if (unaVacunacion.Plan != null && unaVacunacion.Plan.IdPlan == pPlan.IdPlan
                        && unaVacunacion.FechaAplicacion > vUltima)
                    {
                        vUltima = unaVacunacion.FechaAplicacion;
                    }
                }
            }
            else if (pPlan.TipoProcedimiento == PlanSanitario.DESPARASITACION)
            {
                foreach (Tratamiento unTratamiento in this.FiltrarTratamientosXAnimal(pAnimal.IdAnimal))
                {
                    if (unTratamiento.Plan != null && unTratamiento.Plan.IdPlan == pPlan.IdPlan
                        && unTratamiento.FechaInicio > vUltima)
                    {
                        vUltima = unTratamiento.FechaInicio;
                    }
                }
            }
            else
            {
                foreach (Descorne unDescorne in this.FiltrarDescornesXAnimal(pAnimal.IdAnimal))
                {
                    if (unDescorne.Plan != null && unDescorne.Plan.IdPlan == pPlan.IdPlan
                        && unDescorne.Fecha > vUltima)
                    {
                        vUltima = unDescorne.Fecha;
                    }
                }
            }
            return vUltima;
        }

        // Paso 5. Nunca aplicado: vence el dia en que el animal alcanza la edad de
        // inicio. Ya aplicado: la periodicidad se suma a la ultima aplicacion.
        //
        // DateTime.MinValue significa que no hay proxima aplicacion, y eso pasa cuando
        // el plan no tiene periodicidad y ya se cumplio: el procedimiento de aplicacion
        // unica no vuelve a pedirse (paso 6).
        public DateTime ProximaAplicacion(PlanSanitario pPlan, Animal pAnimal)
        {
            if (pPlan == null || pAnimal == null)
            {
                return DateTime.MinValue;
            }

            DateTime vUltima = this.UltimaAplicacion(pPlan, pAnimal);

            if (vUltima == DateTime.MinValue)
            {
                return pAnimal.FechaNacimiento.Date.AddMonths(pPlan.EdadInicioMeses);
            }

            if (pPlan.PeriodicidadDias <= PlanSanitario.SIN_PERIODICIDAD)
            {
                return DateTime.MinValue;
            }

            return vUltima.Date.AddDays(pPlan.PeriodicidadDias);
        }

        // Los animales alcanzados por el plan cuya proxima aplicacion esta vencida o
        // cae dentro del horizonte de anticipacion.
        public List<ProcedimientoPendiente> CalcularPendientes(PlanSanitario pPlan, int pAnticipacionDias)
        {
            List<ProcedimientoPendiente> _listaPendientes = new List<ProcedimientoPendiente>();

            if (pPlan == null || !pPlan.Activo)
            {
                return _listaPendientes;
            }

            DateTime vLimite = DateTime.Now.Date.AddDays(pAnticipacionDias);

            foreach (Animal unAnimal in this.ListarAnimales())
            {
                if (!this.PlanAlcanzaAnimal(pPlan, unAnimal))
                {
                    continue;
                }

                DateTime vProxima = this.ProximaAplicacion(pPlan, unAnimal);

                if (vProxima == DateTime.MinValue || vProxima.Date > vLimite)
                {
                    continue;
                }

                _listaPendientes.Add(new ProcedimientoPendiente(unAnimal, pPlan,
                    this.UltimaAplicacion(pPlan, unAnimal), vProxima));
            }
            return _listaPendientes;
        }

        // Paso 7: el cronograma completo del rodeo, ordenado por fecha. Los vencidos
        // quedan primero porque su fecha ya paso.
        public List<ProcedimientoPendiente> ObtenerCalendarioSanitario(int pAnticipacionDias)
        {
            List<ProcedimientoPendiente> _listaCalendario = new List<ProcedimientoPendiente>();

            foreach (PlanSanitario unPlan in this.ListarPlanesActivos())
            {
                foreach (ProcedimientoPendiente unPendiente in this.CalcularPendientes(unPlan, pAnticipacionDias))
                {
                    this.InsertarPendienteOrdenado(_listaCalendario, unPendiente);
                }
            }
            return _listaCalendario;
        }

        // Curso alternativo 7a: el usuario restringe el cronograma por tipo de
        // procedimiento o por categoria. El tipo vacio y la categoria en cero son
        // "todos".
        public List<ProcedimientoPendiente> FiltrarCalendario(List<ProcedimientoPendiente> pCalendario,
            string pTipoProcedimiento, int pIdCategoria)
        {
            List<ProcedimientoPendiente> _listaFiltrada = new List<ProcedimientoPendiente>();

            foreach (ProcedimientoPendiente unPendiente in pCalendario)
            {
                if (pTipoProcedimiento != "" && unPendiente.Plan.TipoProcedimiento != pTipoProcedimiento)
                {
                    continue;
                }

                if (pIdCategoria != 0 && (unPendiente.Animal.Categoria == null
                    || unPendiente.Animal.Categoria.IdCategoria != pIdCategoria))
                {
                    continue;
                }

                _listaFiltrada.Add(unPendiente);
            }
            return _listaFiltrada;
        }

        public bool EstaVencido(ProcedimientoPendiente pPendiente)
        {
            if (pPendiente != null && pPendiente.ProximaAplicacion.Date < DateTime.Now.Date)
            {
                return true;
            }
            return false;

        }

        public int DiasParaAplicar(ProcedimientoPendiente pPendiente)
        {
            if (pPendiente == null)
            {
                return 0;
            }
            return (int)(pPendiente.ProximaAplicacion.Date - DateTime.Now.Date).TotalDays;
        }

        // El cronograma se arma ordenado en lugar de ordenarlo al final: se recorre un
        // plan por vez y cada pendiente entra en la posicion que le toca por fecha.
        private void InsertarPendienteOrdenado(List<ProcedimientoPendiente> pLista,
            ProcedimientoPendiente pPendiente)
        {
            int vPosicion = 0;

            while (vPosicion < pLista.Count
                && pLista[vPosicion].ProximaAplicacion.Date <= pPendiente.ProximaAplicacion.Date)
            {
                vPosicion = vPosicion + 1;
            }
            pLista.Insert(vPosicion, pPendiente);
        }
        #endregion
    }
}
