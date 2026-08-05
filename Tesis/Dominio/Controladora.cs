using Tesis.Persistencia;

namespace Tesis.Dominio
{
    public class Controladora
    {
        private pControladora Persistencia;

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
        private static string mUsuarioSistema = "sofia";
        private static string mContrasenaSistema = "tambo2026";

        public bool ValidarCredenciales(string pUsuario, string pContrasena)
        {
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
            // pAnimal resuelve raza y categoria contra la cache, por eso se refrescan primero
            mListaRazas = Persistencia.ListarRazas();
            mListaCategorias = Persistencia.ListarCategorias();
            mListaAnimales = Persistencia.ListarAnimales();
            return mListaAnimales;
        }

        public int ProximoAnimalId()
        {
            return Persistencia.ProximoAnimalId();
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
            if (!this.ExisteCaravana(pAnimal.NumCaravana))
            {
                if (Persistencia.AltaAnimal(pAnimal))
                {
                    // La especializacion se guarda en su propia tabla
                    if (pAnimal is Hembra)
                    {
                        Hembra unaHembra = (Hembra)pAnimal;
                        Persistencia.AltaHembra(unaHembra);
                        mListaHembras.Add(unaHembra);
                    }
                    else
                    {
                        Macho unMacho = (Macho)pAnimal;
                        Persistencia.AltaMacho(unMacho);
                        mListaMachos.Add(unMacho);
                    }

                    mListaAnimales.Add(pAnimal);
                    return true;
                }
            }
            return false;

        }

        public bool ModificarAnimal(int pIdAnimal, string pNumCaravana, DateTime pFechaNacimiento,
            Raza pRaza, Categoria pCategoria, Hembra pMadre, Macho pPadre)
        {
            Animal unAnimal = this.BuscarAnimal(pIdAnimal);
            if (unAnimal != null)
            {
                // La validacion de unicidad excluye al propio animal
                Animal otroAnimal = this.BuscarAnimalXCaravana(pNumCaravana);
                if (otroAnimal == null || otroAnimal.IdAnimal == pIdAnimal)
                {
                    unAnimal.NumCaravana = pNumCaravana;
                    unAnimal.FechaNacimiento = pFechaNacimiento;
                    unAnimal.Raza = pRaza;
                    unAnimal.Categoria = pCategoria;
                    unAnimal.Madre = pMadre;
                    unAnimal.Padre = pPadre;

                    if (Persistencia.ModificarAnimal(unAnimal))
                    {
                        return true;
                    }
                }
            }
            return false;

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

        public bool EliminarAnimal(int pIdAnimal)
        {
            Animal unAnimal = this.BuscarAnimal(pIdAnimal);
            if (unAnimal != null && !this.EsProgenitor(pIdAnimal))
            {
                if (Persistencia.EliminarAnimal(pIdAnimal))
                {
                    if (unAnimal is Hembra)
                    {
                        mListaHembras.Remove((Hembra)unAnimal);
                    }
                    else
                    {
                        mListaMachos.Remove((Macho)unAnimal);
                    }

                    mListaAnimales.Remove(unAnimal);
                    return true;
                }
            }
            return false;

        }

        public bool EsProgenitor(int pIdAnimal)
        {
            bool flag = false;
            foreach (Animal unAnimal in mListaAnimales)
            {
                if (unAnimal.Madre != null && unAnimal.Madre.IdAnimal == pIdAnimal)
                {
                    flag = true;
                }
                if (unAnimal.Padre != null && unAnimal.Padre.IdAnimal == pIdAnimal)
                {
                    flag = true;
                }
            }
            return flag;
        }

        public int CalcularEdadMeses(Animal pAnimal)
        {
            int meses = ((DateTime.Now.Year - pAnimal.FechaNacimiento.Year) * 12)
                + (DateTime.Now.Month - pAnimal.FechaNacimiento.Month);

            // Todavia no cumplio meses este mes
            if (DateTime.Now.Day < pAnimal.FechaNacimiento.Day)
            {
                meses = meses - 1;
            }
            if (meses < 0)
            {
                meses = 0;
            }
            return meses;
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
                if (vEdadMeses > 15 && unMacho.EnPie)
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
            // pHembra recupera los objetos de la cache de animales, por eso se refresca primero
            this.ListarAnimales();
            mListaHembras = Persistencia.ListarHembras();
            return mListaHembras;
        }

        public int ProximoHembraId()
        {
            return Persistencia.ProximoHembraId();
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
                if (unaHembra.EstadoProductivo == "En lactancia")
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
            // pMacho recupera los objetos de la cache de animales, por eso se refresca primero
            this.ListarAnimales();
            mListaMachos = Persistencia.ListarMachos();
            return mListaMachos;
        }

        public int ProximoMachoId()
        {
            return Persistencia.ProximoMachoId();
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
            // Puede usarse como reproductor a partir de los quince meses
            if (pMacho != null && this.CalcularEdadMeses(pMacho) > 15)
            {
                return true;
            }
            return false;

        }
        #endregion
    }
}
