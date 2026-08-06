using Tesis.Persistencia;

namespace Tesis.Dominio
{
    public class Controladora
    {
        private pControladora Persistencia;

        // Edad minima a la que un animal puede entrar en servicio y duracion de la
        // gestacion, en meses. La suma es la diferencia minima de edad que puede
        // haber entre un progenitor y su cria.
        public const int EDAD_MINIMA_SERVICIO_MESES = 15;
        public const int GESTACION_MESES = 9;

        private static List<Raza> mListaRazas = new List<Raza>();
        private static List<Categoria> mListaCategorias = new List<Categoria>();
        private static List<Animal> mListaAnimales = new List<Animal>();
        private static List<Hembra> mListaHembras = new List<Hembra>();
        private static List<Macho> mListaMachos = new List<Macho>();

        public Controladora()
        {
            Persistencia = new pControladora();
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
            mListaRazas = Persistencia.ListarRazas();
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
            mListaCategorias = Persistencia.ListarCategorias();
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
            // pAnimal necesita razas y categorias para resolver cada animal: se listan
            // primero y se le pasan. Antes las buscaba solo, instanciando una
            // Controladora desde la capa de persistencia.
            mListaRazas = Persistencia.ListarRazas();
            mListaCategorias = Persistencia.ListarCategorias();
            mListaAnimales = Persistencia.ListarAnimales(mListaRazas, mListaCategorias);
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
            int vMesesMinimos = EDAD_MINIMA_SERVICIO_MESES + GESTACION_MESES;

            if (pMadre != null && this.DiferenciaMeses(pMadre.FechaNacimiento, pFechaNacimiento) < vMesesMinimos)
            {
                return "La madre tiene que haber nacido al menos " + vMesesMinimos + " meses antes que la cria!";
            }
            if (pPadre != null && this.DiferenciaMeses(pPadre.FechaNacimiento, pFechaNacimiento) < vMesesMinimos)
            {
                return "El padre tiene que haber nacido al menos " + vMesesMinimos + " meses antes que la cria!";
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
            // pHembra selecciona sobre los animales ya armados, por eso se refrescan primero
            this.ListarAnimales();
            mListaHembras = Persistencia.ListarHembras(mListaAnimales);
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
        #endregion

        #region MACHOS
        public List<Macho> ListarMachos()
        {
            // pMacho selecciona sobre los animales ya armados, por eso se refrescan primero
            this.ListarAnimales();
            mListaMachos = Persistencia.ListarMachos(mListaAnimales);
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
    }
}
