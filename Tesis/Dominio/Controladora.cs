using Tesis.Persistencia;

namespace Tesis.Dominio
{
    public class Controladora
    {
        private pControladora Persistencia;

        // Las constantes que siguen se dividen en dos. Unas son biologia y no se tocan:
        // la duracion de la gestacion, la edad a la que la hembra empieza a ciclar, el
        // rango en que una preniez termina en un parto viable. Otras son decisiones de
        // manejo -cuantos dias antes del parto se seca, a que edad entra la vaquillona
        // en servicio, con cuanta anticipacion avisa cada alerta- y esas viven en la
        // tabla de configuracion: aca quedan nada mas que como valor por defecto, para
        // que el sistema funcione antes de que nadie configure nada.
        //
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

        // Edad a la que la cria deja de ser ternera o ternero (RF1.9), y cantidad de
        // ordenies por dia. Las dos son decisiones del establecimiento: hay tambos que
        // ordenian dos veces y otros tres.
        public const int EDAD_CAMBIO_CATEGORIA_MESES = 12;
        public const int ORDENIES_POR_DIA = 2;

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

        // Edad a la que la vaquillona empieza a manifestar celo. Es anterior a la edad
        // minima de servicio: el celo se detecta y se anota bastante antes de que el
        // animal este en condiciones de ser servido.
        public const int EDAD_MINIMA_CELO_MESES = 9;

        // Largo estandar de una lactancia. No es una decision del establecimiento: es la
        // referencia con la que se comparan las vacas entre si y con la que trabaja
        // cualquier control lechero.
        public const int DIAS_LACTANCIA_ESTANDAR = 305;

        // Valores por defecto de los dos parametros de trabajo reproductivo. El periodo
        // de espera voluntario son los dias posparto que se dejan pasar antes de volver
        // a servir; el otro, cuando el tacto ya se puede hacer.
        public const int DIAS_ESPERA_VOLUNTARIA = 45;
        public const int DIAS_PARA_TACTO = 35;

        // Criterios con los que el sistema propone revisar una vaca para descarte. No
        // deciden nada: arman la lista que la encargada mira. Se dejan como constantes
        // porque son un criterio de analisis y no una regla de manejo diaria.
        public const double PORCENTAJE_PRODUCCION_BAJA = 0.7;
        public const int SERVICIOS_SIN_PRENIEZ_DESCARTE = 3;
        public const int DIAS_ABIERTOS_EXCESIVOS = 150;
        public const int DIAGNOSTICOS_REPETIDOS_DESCARTE = 3;
        public const int PARTOS_PARA_DESCARTE = 7;

        // Rango en el que una gestacion Holando puede terminar en un parto viable. Por
        // debajo del minimo es un aborto o un prematuro, y por encima del maximo la
        // fecha de servicio o la de parto estan mal cargadas. La gestacion normal son
        // los GESTACION_DIAS.
        public const int GESTACION_DIAS_MINIMA = 240;
        public const int GESTACION_DIAS_MAXIMA = 320;

        // Parametros de manejo del establecimiento. Las constantes de arriba son los
        // valores por defecto: si la tabla de configuracion esta vacia, el sistema se
        // comporta como antes de que la configuracion existiera.
        private static Configuracion mConfiguracion = null;

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

            // Primero los parametros: las reglas que dependen de ellos -la fecha
            // recomendada de secado, la edad minima al servicio, los turnos de ordenie-
            // se evaluan sobre las listas que se cargan despues.
            mConfiguracion = Persistencia.ObtenerConfiguracion();

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

        #region CONFIGURACION
        // Los parametros de manejo, siempre con un valor utilizable. Es lo que consulta
        // el resto de la Controladora en lugar de las constantes: si la tabla esta
        // vacia devuelve los valores por defecto, asi que ninguna regla se queda sin
        // numero por falta de configuracion.
        private Configuracion Parametros()
        {
            if (mConfiguracion == null)
            {
                this.Refrescar();
            }

            if (mConfiguracion == null)
            {
                mConfiguracion = this.ConfiguracionPorDefecto();
            }
            return mConfiguracion;
        }

        private Configuracion ConfiguracionPorDefecto()
        {
            return new Configuracion(0, DIAS_SECADO_ANTES_PARTO, EDAD_MINIMA_SERVICIO_HEMBRA_MESES,
                EDAD_CAMBIO_CATEGORIA_MESES, LITROS_MAXIMOS_INDIVIDUAL, ORDENIES_POR_DIA,
                DIAS_ANTICIPACION_SECADO, DIAS_ANTICIPACION_PARTO, DIAS_ANTICIPACION_SANITARIA,
                DIAS_ANTICIPACION_VENCIMIENTO, DIAS_ESPERA_VOLUNTARIA, DIAS_PARA_TACTO);
        }

        public Configuracion ObtenerConfiguracion()
        {
            this.Refrescar();
            return this.Parametros();
        }

        // Devuelve el motivo por el que la configuracion no se puede guardar, o una
        // cadena vacia si esta bien. Los topes no son caprichosos: son el rango dentro
        // del cual el parametro sigue teniendo sentido en un tambo.
        public string ValidarConfiguracion(Configuracion pConfiguracion)
        {
            if (pConfiguracion.DiasSecadoAntesParto < 1 || pConfiguracion.DiasSecadoAntesParto > 120)
            {
                return "Los dias de secado antes del parto tienen que estar entre 1 y 120.";
            }

            // Por debajo de la edad de pubertad el parametro no tendria sentido: la
            // hembra ni siquiera cicla.
            if (pConfiguracion.EdadMinimaServicioMeses < EDAD_MINIMA_CELO_MESES
                || pConfiguracion.EdadMinimaServicioMeses > 36)
            {
                return "La edad minima al servicio tiene que estar entre " + EDAD_MINIMA_CELO_MESES
                    + " y 36 meses.";
            }

            if (pConfiguracion.EdadCambioCategoriaMeses < 1 || pConfiguracion.EdadCambioCategoriaMeses > 36)
            {
                return "La edad de cambio de categoria tiene que estar entre 1 y 36 meses.";
            }

            if (pConfiguracion.LitrosMaximosIndividual <= 0)
            {
                return "El tope de litros por control individual tiene que ser mayor a cero.";
            }

            if (pConfiguracion.OrdeniesPorDia < 1 || pConfiguracion.OrdeniesPorDia > 3)
            {
                return "La cantidad de ordenies por dia tiene que ser 1, 2 o 3.";
            }

            if (pConfiguracion.DiasAnticipacionSecado < 1 || pConfiguracion.DiasAnticipacionParto < 1
                || pConfiguracion.DiasAnticipacionSanitaria < 1 || pConfiguracion.DiasAnticipacionVencimiento < 1)
            {
                return "Las ventanas de anticipacion tienen que ser numeros positivos de dias.";
            }

            if (pConfiguracion.DiasAnticipacionSecado > 365 || pConfiguracion.DiasAnticipacionParto > 365
                || pConfiguracion.DiasAnticipacionSanitaria > 365 || pConfiguracion.DiasAnticipacionVencimiento > 365)
            {
                return "Las ventanas de anticipacion no pueden superar los 365 dias.";
            }

            if (pConfiguracion.DiasEsperaVoluntaria < 1 || pConfiguracion.DiasEsperaVoluntaria > 200)
            {
                return "El periodo de espera voluntario tiene que estar entre 1 y 200 dias.";
            }

            // Antes de los 25 dias no hay gestacion palpable ni ecografia confiable
            if (pConfiguracion.DiasParaTacto < 25 || pConfiguracion.DiasParaTacto > 200)
            {
                return "Los dias para el tacto tienen que estar entre 25 y 200.";
            }

            return "";
        }

        // Cambiar un parametro mueve los calculos de ahi en adelante, no lo ya
        // guardado: la fecha recomendada de secado se deriva en cada consulta y cambia
        // al instante para todo el rodeo, pero la fecha probable de parto que quedo
        // escrita en un servicio o en una lactancia sigue siendo la que era.
        public bool ModificarConfiguracion(Configuracion pConfiguracionNueva)
        {
            if (this.ValidarConfiguracion(pConfiguracionNueva) != "")
            {
                return false;
            }

            // La configuracion no se da de alta: se modifica la fila que ya existe. Si
            // la tabla estaba vacia no hay nada que actualizar.
            Configuracion unaConfiguracion = this.ObtenerConfiguracion();
            if (unaConfiguracion.IdConfiguracion == 0)
            {
                return false;
            }

            pConfiguracionNueva.IdConfiguracion = unaConfiguracion.IdConfiguracion;

            if (Persistencia.ModificarConfiguracion(pConfiguracionNueva))
            {
                mConfiguracion = pConfiguracionNueva;
                return true;
            }
            return false;

        }

        // Los turnos de ordenie que ofrece el sistema, armados a partir de la cantidad
        // de ordenies por dia. Un tambo que ordenia tres veces tiene tres turnos.
        public List<string> ListarTurnos()
        {
            List<string> _listaTurnos = new List<string>();

            for (int vNumero = 1; vNumero <= this.Parametros().OrdeniesPorDia; vNumero = vNumero + 1)
            {
                _listaTurnos.Add(OrdenieLote.NombreTurno(vNumero));
            }
            return _listaTurnos;
        }

        // Un ordenie registrado antes de bajar la cantidad de turnos conserva el suyo:
        // esto valida lo que se esta por guardar, no lo que ya esta guardado.
        public bool EsTurnoValido(string pTurno)
        {
            foreach (string unTurno in this.ListarTurnos())
            {
                if (unTurno == pTurno)
                {
                    return true;
                }
            }
            return false;

        }
        #endregion

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
        //
        // La foto llega ya resuelta: la pantalla manda el nombre del archivo nuevo si
        // se cargo uno, vacio si se pidio quitarla, o el que ya tenia si no se toco.
        public bool ModificarAnimal(int pIdAnimal, string pNumCaravana, DateTime pFechaNacimiento,
            Raza pRaza, Categoria pCategoria, Hembra pMadre, Macho pPadre,
            int pNumeroPartos, bool pEnPie, string pFoto)
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
            unAnimalNuevo.Foto = pFoto ?? "";

            if (!Persistencia.ModificarAnimal(unAnimalNuevo))
            {
                return false;
            }

            // La foto vieja se borra recien cuando la nueva quedo guardada: si la
            // escritura fallaba, el animal se quedaba sin imagen y sin archivo.
            if (unAnimal.Foto != unAnimalNuevo.Foto)
            {
                Persistencia.BorrarFoto(unAnimal.Foto);
            }

            unAnimal.NumCaravana = pNumCaravana;
            unAnimal.FechaNacimiento = pFechaNacimiento;
            unAnimal.Raza = pRaza;
            unAnimal.Categoria = pCategoria;
            unAnimal.Madre = pMadre;
            unAnimal.Padre = pPadre;
            unAnimal.Foto = unAnimalNuevo.Foto;

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
            Animal unAnimalNuevo;

            if (pAnimal is Hembra)
            {
                Hembra unaHembra = (Hembra)pAnimal;
                unAnimalNuevo = new Hembra(pAnimal.IdAnimal, pNumCaravana, pFechaNacimiento, pAnimal.Activo,
                    pAnimal.FechaBaja, pAnimal.MotivoBaja, pRaza, pCategoria, pMadre, pPadre,
                    pNumeroPartos, unaHembra.EstadoProductivo, unaHembra.EstadoReproductivo);
            }
            else
            {
                unAnimalNuevo = new Macho(pAnimal.IdAnimal, pNumCaravana, pFechaNacimiento, pAnimal.Activo,
                    pAnimal.FechaBaja, pAnimal.MotivoBaja, pRaza, pCategoria, pMadre, pPadre, pEnPie);
            }

            // La copia arrastra la foto del original. El unico que la cambia es
            // ModificarAnimal, que la pisa despues con la que mando la pantalla: asi
            // una actualizacion de categoria no le borra la imagen al animal.
            unAnimalNuevo.Foto = pAnimal.Foto;

            return unAnimalNuevo;
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

        // Deshace la baja logica. Un error de carga -la caravana equivocada, un motivo
        // mal elegido- dejaba al animal fuera del rodeo sin vuelta atras, y con el a
        // todo su historial productivo y reproductivo: no aparecia mas en el lote de
        // ordenie, ni en las alertas, ni en el calendario sanitario.
        public bool ReactivarAnimal(string pNumCaravana)
        {
            Animal unAnimal = this.BuscarAnimalXCaravana(pNumCaravana);
            if (unAnimal == null || unAnimal.Activo)
            {
                return false;
            }

            if (Persistencia.ReactivarAnimal(unAnimal.IdAnimal))
            {
                unAnimal.Activo = true;
                unAnimal.FechaBaja = DateTime.MinValue;
                unAnimal.MotivoBaja = "";
                return true;
            }
            return false;

        }

        // Guarda la imagen y devuelve el nombre del archivo, que es lo que despues se
        // escribe en la columna foto del animal. Devuelve vacio si el contenido no es
        // una imagen valida: la foto es opcional y el alta no se cae por eso.
        public string GuardarFotoAnimal(byte[] pContenido)
        {
            return Persistencia.GuardarFoto(pContenido);
        }

        // Borra un archivo que quedo sin dueno: la foto que se guardo para un alta que
        // despues no prospero.
        public void BorrarFotoAnimal(string pNombreArchivo)
        {
            Persistencia.BorrarFoto(pNombreArchivo);
        }

        // Direccion con la que la pantalla le pide la foto al servidor. Vacio cuando el
        // animal no tiene ninguna, que es la senal para dibujar la silueta de reemplazo.
        public static string UrlFoto(Animal pAnimal)
        {
            if (pAnimal == null || !pAnimal.TieneFoto)
            {
                return "";
            }
            return "/" + pFotoAnimal.CARPETA + "/" + pAnimal.Foto;
        }

        // Si el animal integraba el rodeo en esa fecha. Un animal dado de baja no puede
        // protagonizar un evento posterior a su baja, pero si uno anterior: cargar hoy
        // el servicio del mes pasado de una vaca que despues se vendio es lo normal en
        // el tambo, no un error.
        public bool EstabaActivo(Animal pAnimal, DateTime pFecha)
        {
            if (pAnimal == null)
            {
                return false;
            }

            if (pAnimal.Activo)
            {
                return true;
            }

            // Inactivo y sin fecha de baja: no hay contra que comparar
            if (pAnimal.FechaBaja == DateTime.MinValue)
            {
                return false;
            }
            return pFecha.Date <= pAnimal.FechaBaja.Date;
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
            int vMesesMinimosMadre = this.Parametros().EdadMinimaServicioMeses + GESTACION_MESES;
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

        // Lo que hace ruido en la genealogia elegida pero no la vuelve imposible. No
        // impide guardar: la pantalla las muestra y el usuario decide.
        //
        // La diferencia con ValidarGenealogia es deliberada. Alla estan las
        // imposibilidades -un animal padre de si mismo, un progenitor mas joven que su
        // cria-; aca, los datos que en un tambo real pueden ser ciertos aunque parezcan
        // errores, y que trabar de entrada haria imposible la carga inicial del rodeo.
        public List<string> AdvertenciasGenealogia(DateTime pFechaNacimiento, Hembra pMadre, Macho pPadre)
        {
            List<string> _listaAdvertencias = new List<string>();

            // La madre tiene que haber estado en el rodeo el dia del parto. Si ya
            // figuraba de baja, o la fecha de baja o la de nacimiento estan mal.
            if (pMadre != null && !this.EstabaActivo(pMadre, pFechaNacimiento))
            {
                _listaAdvertencias.Add("La madre " + pMadre.NumCaravana + " figura dada de baja el "
                    + pMadre.FechaBaja.ToShortDateString() + ", antes de la fecha de nacimiento de la cria.");
            }

            // El padre solo tiene que haber estado vivo en la concepcion, no en el
            // parto, y ni siquiera eso si la cria vino de una pajuela: el semen
            // congelado sigue sirviendo anios despues de que el toro murio.
            if (pPadre != null && !this.EstabaActivo(pPadre, pFechaNacimiento.AddMonths(-GESTACION_MESES)))
            {
                _listaAdvertencias.Add("El padre " + pPadre.NumCaravana + " figura dado de baja el "
                    + pPadre.FechaBaja.ToShortDateString() + ", antes de la concepcion. "
                    + "Es correcto si la cria vino de una pajuela suya.");
            }

            // Padre y madre emparentados: la cria nace consanguinea (CU6)
            if (pMadre != null && pPadre != null && this.VerificarConsanguinidad(pMadre, pPadre))
            {
                _listaAdvertencias.Add("Los progenitores tienen parentesco entre si: "
                    + this.TextoAncestroComun(pMadre, pPadre) + " La cria nace consanguinea.");
            }

            return _listaAdvertencias;
        }

        // El ancestro que comparten dos animales, listo para mostrar. Se arma aca y no
        // en la pantalla para que las tres advertencias de consanguinidad -alta,
        // modificacion y servicio- digan lo mismo.
        private string TextoAncestroComun(Animal pAnimal, Animal pPareja)
        {
            Animal unAncestro = this.BuscarAncestroComun(pAnimal, pPareja);

            if (unAncestro != null)
            {
                return "el ancestro comun es la caravana " + unAncestro.NumCaravana + ".";
            }
            return "no se identifico el ancestro comun.";
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
                else if (vEdadMeses > this.Parametros().EdadCambioCategoriaMeses)
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
                else if (vEdadMeses > this.Parametros().EdadCambioCategoriaMeses)
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
            Hembra unaHembraNueva = new Hembra(pHembra.IdAnimal, pHembra.NumCaravana,
                pHembra.FechaNacimiento, pHembra.Activo, pHembra.FechaBaja, pHembra.MotivoBaja,
                pHembra.Raza, pHembra.Categoria, pHembra.Madre, pHembra.Padre, pNumeroPartos,
                pEstadoProductivo, pEstadoReproductivo);

            // La copia arrastra la foto, por el mismo motivo que CopiarAnimal. La
            // mayoria de las escrituras que usan esta copia tocan solo la tabla de
            // hembras, donde la foto no vive, pero el borrado del parto tambien
            // reescribe la fila de animales para devolverle la categoria a la madre: sin
            // esta linea, deshacer un parto le borraria la foto a la vaca.
            unaHembraNueva.Foto = pHembra.Foto;

            return unaHembraNueva;
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
            return unaLactancia.FechaProbableParto.AddDays(-this.Parametros().DiasSecadoAntesParto);
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
                    && DateTime.Now.Date >= vFechaSecado.AddDays(-this.Parametros().DiasAnticipacionSecado).Date)
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
            if (pLitros <= vAnimales * this.Parametros().LitrosMaximosIndividual)
            {
                return true;
            }
            return false;

        }

        public bool ValidarLitrosIndividual(double pLitros)
        {
            if (pLitros > 0 && pLitros <= this.Parametros().LitrosMaximosIndividual)
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

            if (!this.EsTurnoValido(pOrdenieLote.Turno))
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

        // La baja del ordenie del turno. Nunca se bloquea: el lote no deduce ningun
        // estado, no consume stock y lo unico que lo referencia son los controles
        // individuales de ese turno, que sobreviven solos.
        //
        // Borrarlo es decir "ese ordenie no se cargo", y hace falta para el caso que la
        // correccion no cubre: el turno cargado en la fecha equivocada. La fecha y el
        // turno son clave alterna, asi que no se pueden reescribir; el unico camino es
        // dar de baja y volver a cargar.
        public string ValidarEliminarOrdenieLote(int pIdOrdenieLote)
        {
            this.Refrescar();

            if (this.BuscarOrdenieLote(pIdOrdenieLote) == null)
            {
                return "El ordeñe que se quiere eliminar ya no existe!";
            }
            return "";
        }

        public bool EliminarOrdenieLote(int pIdOrdenieLote)
        {
            if (this.ValidarEliminarOrdenieLote(pIdOrdenieLote) != "")
            {
                return false;
            }

            OrdenieLote unOrdenie = this.BuscarOrdenieLote(pIdOrdenieLote);

            if (Persistencia.EliminarOrdenieLote(pIdOrdenieLote))
            {
                // Los controles de ese turno quedan como los que se cargan antes que el
                // total: medidos, contados y sin lote al que engancharse.
                foreach (OrdenieIndividual unControl in mListaOrdeniesIndividual)
                {
                    if (unControl.OrdenieLote != null
                        && unControl.OrdenieLote.IdOrdenieLote == pIdOrdenieLote)
                    {
                        unControl.OrdenieLote = null;
                    }
                }

                mListaOrdeniesLote.Remove(unOrdenie);
                return true;
            }
            return false;

        }

        // Litros del turno: los del ordenie masivo mas los de las vacas que se
        // controlaron individualmente ese mismo dia y turno, que no estan incluidas en
        // litros_totales.
        // Turnos que se registraron unicamente con control individual, sin el total del
        // ordenie. Su produccion es la suma de lo medido vaca por vaca y esta contada
        // como tal, asi que no son un faltante: son otra forma de anotar el mismo
        // ordenie.
        //
        // Se listan igual porque el dato es mas fragil que la lectura del tanque -si
        // una vaca no se midio, esos litros no estan en ningun lado-, y porque saber
        // que un turno no tiene total ayuda a interpretar los numeros del periodo.
        //
        // Se devuelven como OrdenieLote sin guardar -el identificador en cero- porque es
        // exactamente eso: el ordenie del turno, reconstruido con las vacas medidas y
        // los litros que sumaron entre ellas.
        public List<OrdenieLote> ListarTurnosSoloConControlIndividual()
        {
            List<OrdenieLote> _listaFaltantes = new List<OrdenieLote>();

            foreach (OrdenieIndividual unOrdenie in this.ListarOrdeniesIndividual())
            {
                if (this.BuscarOrdenieLoteXFechaTurno(unOrdenie.Fecha, unOrdenie.Turno) != null)
                {
                    continue;
                }

                OrdenieLote unFaltante = this.BuscarFaltante(_listaFaltantes, unOrdenie.Fecha, unOrdenie.Turno);

                if (unFaltante == null)
                {
                    unFaltante = new OrdenieLote(0, unOrdenie.Fecha.Date, unOrdenie.Turno, 0, new List<Hembra>());
                    _listaFaltantes.Add(unFaltante);
                }

                unFaltante.LitrosTotales = unFaltante.LitrosTotales + unOrdenie.Litros;

                if (unOrdenie.Animal != null)
                {
                    unFaltante.Animales.Add(unOrdenie.Animal);
                }
            }
            return _listaFaltantes;
        }

        private OrdenieLote BuscarFaltante(List<OrdenieLote> pLista, DateTime pFecha, string pTurno)
        {
            foreach (OrdenieLote unOrdenie in pLista)
            {
                if (unOrdenie.Fecha.Date == pFecha.Date && unOrdenie.Turno == pTurno)
                {
                    return unOrdenie;
                }
            }
            return null;
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

            if (!this.EsTurnoValido(pOrdenie.Turno))
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

        // Correccion del control ya cargado. Se reescriben los litros y nada mas.
        //
        // La fecha, el turno y el animal son la clave alterna del control: cambiarlos no
        // es corregir este registro sino cargar otro distinto, y el caso real que hay
        // detras -"lo anote en la vaca de al lado"- se resuelve mejor con la baja, que
        // es lo que la persona entiende que tiene que deshacer.
        //
        // Hace falta por lo mismo que ModificarOrdenieLote: BuscarOrdenieIndividualX
        // FechaTurno impide volver a cargar el mismo animal en la misma fecha y turno,
        // asi que sin correccion un error de tipeo quedaba fijo para siempre. Y acá pesa
        // mas que en el lote, porque estos litros alimentan la produccion de la
        // lactancia, la proyeccion a 305 dias y el criterio de produccion baja de las
        // candidatas a descarte: un cero de mas mueve decisiones sobre el animal.
        public string ValidarModificarOrdenieIndividual(int pIdOrdenieInd, double pLitros)
        {
            this.Refrescar();

            if (this.BuscarOrdenieIndividual(pIdOrdenieInd) == null)
            {
                return "El control que se quiere corregir ya no existe!";
            }

            if (!this.ValidarLitrosIndividual(pLitros))
            {
                return "Los litros tienen que ser un valor positivo y no pueden superar los "
                    + this.Parametros().LitrosMaximosIndividual.ToString("N2") + " litros por control!";
            }
            return "";
        }

        public bool ModificarOrdenieIndividual(int pIdOrdenieInd, double pLitros)
        {
            if (this.ValidarModificarOrdenieIndividual(pIdOrdenieInd, pLitros) != "")
            {
                return false;
            }

            OrdenieIndividual unOrdenie = this.BuscarOrdenieIndividual(pIdOrdenieInd);

            OrdenieIndividual unOrdenieNuevo = new OrdenieIndividual(unOrdenie.IdOrdenieInd,
                unOrdenie.Fecha, unOrdenie.Turno, pLitros, unOrdenie.Animal,
                unOrdenie.Lactancia, unOrdenie.OrdenieLote);

            if (Persistencia.ModificarOrdenieIndividual(unOrdenieNuevo))
            {
                unOrdenie.Litros = pLitros;
                return true;
            }
            return false;

        }

        // El control individual no se bloquea nunca. No deduce ningun estado, no
        // consume stock y ningun otro registro lo referencia: lo unico que sale de el
        // son sumas, y las sumas se rehacen solas con los controles que quedan.
        //
        // Al reves, borrarlos destraba: EliminarParto se niega mientras la lactancia que
        // el parto abrio tenga ordenies cargados, asi que sacar el control mal cargado
        // es justamente el paso previo que ese mensaje pide.
        public string ValidarEliminarOrdenieIndividual(int pIdOrdenieInd)
        {
            this.Refrescar();

            if (this.BuscarOrdenieIndividual(pIdOrdenieInd) == null)
            {
                return "El control que se quiere eliminar ya no existe!";
            }
            return "";
        }

        public bool EliminarOrdenieIndividual(int pIdOrdenieInd)
        {
            if (this.ValidarEliminarOrdenieIndividual(pIdOrdenieInd) != "")
            {
                return false;
            }

            OrdenieIndividual unOrdenie = this.BuscarOrdenieIndividual(pIdOrdenieInd);

            if (Persistencia.EliminarOrdenieIndividual(pIdOrdenieInd))
            {
                mListaOrdeniesIndividual.Remove(unOrdenie);
                return true;
            }
            return false;

        }

        // Litros ya cargados como control individual en ese turno. Es informacion de
        // contexto para la pantalla del ordenie por lote: sirve para comparar lo medido
        // vaca por vaca contra lo que dio el tanque, pero no se le resta ni se le suma
        // al lote, porque es la misma leche.
        // Litros de los turnos que se registraron unicamente con control individual, o
        // sea sin ordenie por lote cargado. Esos turnos son ordenies completos: la
        // unica diferencia es como se anotaron, asi que su leche es produccion.
        public double SumarLitrosSinOrdenieLote(DateTime pDesde, DateTime pHasta)
        {
            double vTotal = 0;

            foreach (OrdenieIndividual unOrdenie in this.FiltrarOrdeniesIndividualXFecha(pDesde, pHasta))
            {
                if (this.BuscarOrdenieLoteXFechaTurno(unOrdenie.Fecha, unOrdenie.Turno) == null)
                {
                    vTotal = vTotal + unOrdenie.Litros;
                }
            }
            return vTotal;
        }

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
        // Las dos formas de mirar la produccion. No son dos sumandos: el ordenie por
        // lote es la leche que salio del tambo -el tanque completo- y el control
        // individual es la medicion, vaca por vaca, de ese mismo ordenie. Sumarlos
        // contaria dos veces la misma leche.
        public const string MODALIDAD_LOTE = "Lote";
        public const string MODALIDAD_INDIVIDUAL = "Individual";

        public double CalcularProduccionEnRango(DateTime pDesde, DateTime pHasta, string pModalidad)
        {
            double vTotal = 0;

            // La produccion se resuelve turno por turno. El ordenie individual no es
            // otra fuente de leche: es el mismo ordenie del turno, anotado vaca por vaca
            // en lugar de con un solo numero. Entonces, si el turno tiene su ordenie por
            // lote, la produccion es ese total -que ya incluye a las vacas medidas-; y
            // si se registro unicamente con controles individuales, la produccion es la
            // suma de esos controles, porque el ordenie ocurrio igual.
            //
            // Lo que nunca se hace es sumar las dos cosas en un mismo turno: eso
            // contaria dos veces la leche de las vacas controladas.
            if (pModalidad == MODALIDAD_LOTE)
            {
                foreach (OrdenieLote unOrdenie in this.FiltrarOrdeniesLoteXFecha(pDesde, pHasta))
                {
                    vTotal = vTotal + unOrdenie.LitrosTotales;
                }

                vTotal = vTotal + this.SumarLitrosSinOrdenieLote(pDesde, pHasta);
            }

            // La otra vista: cuanta leche se midio vaca por vaca en el periodo. Es una
            // porcion de la produccion, no un volumen aparte.
            if (pModalidad == MODALIDAD_INDIVIDUAL)
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

            return this.CalcularProduccionEnRango(vDesde, vHasta, MODALIDAD_LOTE);
        }
        #endregion

        #region CORRECCION DE REGISTROS
        // Los registros de reproduccion y sanidad son hechos: pasaron una sola vez y en
        // un momento determinado. Por eso el sistema no los preveia modificables. Pero
        // el que los carga es una persona que trabaja con las manos ocupadas, y una
        // caravana mal tecleada o un resultado de tacto invertido no se puede quedar
        // adentro del sistema para siempre.
        //
        // La correccion se apoya en dos ideas, que son las que evitan que arreglar un
        // dato rompa otros tres:
        //
        // 1. El estado derivado no se deshace, se vuelve a deducir. El sistema no
        //    guarda "en que estado estaba la vaca antes de este tacto" para poder
        //    revertirlo: despues de cada correccion mira los registros que quedaron y
        //    deduce de cero el estado que corresponde. Asi corregir un registro viejo
        //    no pisa lo que dicen los mas nuevos, que es exactamente lo que pasaria
        //    con un deshacer paso a paso.
        //
        // 2. Lo que tiene algo colgando no se borra: se avisa que hay que sacar eso
        //    primero. Un servicio con tactos, un diagnostico con tratamientos. El
        //    usuario deshace en el orden inverso al que cargo, que es el orden en el
        //    que se acuerda de lo que hizo.
        //
        // El borrado es fisico. La alternativa -marcar el registro como anulado y
        // dejarlo- obligaria a filtrarlo en las cuarenta y pico de consultas que
        // recorren estas listas, y bastaria con olvidarse el filtro en una sola para
        // que un indicador empiece a mentir sin que nadie lo note.

        // El estado reproductivo que le corresponde a la hembra segun los hechos que
        // quedan registrados: su ultimo parto, el servicio posterior a ese parto y el
        // ultimo tacto de ese servicio.
        //
        // Los tres identificadores son los del registro que se esta por eliminar, para
        // que la deduccion lo ignore aunque todavia figure en la cache. Van en cero
        // cuando no hay nada que ignorar, que es el caso de las modificaciones.
        private string EstadoReproductivoDeducido(Hembra pHembra, int pIdServicioIgnorado,
            int pIdTactoIgnorado, int pIdPartoIgnorado)
        {
            if (pHembra == null)
            {
                return Hembra.VACIA;
            }

            Servicio unServicioVigente = this.ServicioVigente(pHembra, pIdServicioIgnorado,
                pIdPartoIgnorado);

            // Sin servicio posterior al ultimo parto la hembra esta vacia: es el estado
            // con el que queda la recien parida y con el que llega la vaquillona que
            // todavia no se sirvio.
            if (unServicioVigente == null)
            {
                return Hembra.VACIA;
            }

            Tacto unUltimoTacto = this.UltimoTacto(unServicioVigente, pIdTactoIgnorado);

            // El dudoso no define nada: la hembra sigue servida a la espera del control
            // siguiente. Es el mismo criterio del alta del tacto.
            if (unUltimoTacto == null || unUltimoTacto.Resultado == Tacto.DUDOSA)
            {
                return Hembra.SERVIDA;
            }

            if (unUltimoTacto.Resultado == Tacto.PRENADA)
            {
                return Hembra.PRENADA;
            }
            return Hembra.VACIA;
        }

        // La hembra con el estado reproductivo que le queda despues de la correccion, o
        // nulo si no cambia. Devolver nulo es lo que le permite a la persistencia
        // saltearse la escritura: la mayoria de las correcciones -una observacion, una
        // fecha- no mueven ningun estado.
        private Hembra HembraRecalculada(Hembra pHembra, int pIdServicioIgnorado,
            int pIdTactoIgnorado, int pIdPartoIgnorado)
        {
            if (pHembra == null)
            {
                return null;
            }

            string vEstadoNuevo = this.EstadoReproductivoDeducido(pHembra, pIdServicioIgnorado,
                pIdTactoIgnorado, pIdPartoIgnorado);

            if (vEstadoNuevo == pHembra.EstadoReproductivo)
            {
                return null;
            }

            return this.CopiarHembra(pHembra, pHembra.NumeroPartos, pHembra.EstadoProductivo,
                vEstadoNuevo);
        }

        // La lactancia en curso con la fecha probable de parto que le corresponde
        // despues de la correccion, o nulo si no cambia. La fecha es la del servicio
        // vigente cuando la hembra queda prenada, y ninguna cuando no.
        //
        // Es de donde CU13 saca la fecha recomendada de secado. Una correccion que la
        // dejara vieja secaria vacas por una preniez que ya no existe, o dejaria de
        // avisar por una que si.
        private Lactancia LactanciaRecalculada(Hembra pHembra, int pIdServicioIgnorado,
            int pIdTactoIgnorado, int pIdPartoIgnorado)
        {
            Lactancia unaLactancia = this.LactanciaActual(pHembra);
            if (unaLactancia == null)
            {
                return null;
            }

            DateTime vFechaProbableParto = DateTime.MinValue;

            if (this.EstadoReproductivoDeducido(pHembra, pIdServicioIgnorado, pIdTactoIgnorado,
                pIdPartoIgnorado) == Hembra.PRENADA)
            {
                Servicio unServicioVigente = this.ServicioVigente(pHembra, pIdServicioIgnorado,
                    pIdPartoIgnorado);

                if (unServicioVigente != null)
                {
                    vFechaProbableParto = unServicioVigente.FechaProbableParto;
                }
            }

            if (unaLactancia.FechaProbableParto.Date == vFechaProbableParto.Date)
            {
                return null;
            }

            return this.CopiarLactancia(unaLactancia, unaLactancia.FechaSecado, vFechaProbableParto);
        }

        // El contra-movimiento que devuelve al stock lo que habia consumido un registro
        // que se elimina o se corrige.
        //
        // Es un ingreso sin fecha de vencimiento a proposito: no es una partida nueva
        // que entra al deposito, es producto que en realidad nunca salio. Sin
        // vencimiento no aparece en las alertas de CU28, que es lo correcto, porque el
        // producto sigue estando en el envase de la partida original.
        //
        // El egreso original no se borra. El historial de movimientos tiene que poder
        // explicar por que el saldo del 12 de marzo era el que era, y para eso el error
        // y su correccion tienen que estar los dos.
        private MovimientoStock Devolucion(Insumo pInsumo, double pCantidad, string pMotivo)
        {
            if (pInsumo == null || pCantidad <= 0)
            {
                return null;
            }

            return new MovimientoStock(0, MovimientoStock.INGRESO, pCantidad, DateTime.Now,
                DateTime.MinValue, pMotivo, pInsumo);
        }

        // El egreso que deja el consumo nuevo cuando una correccion cambia el producto
        // aplicado o la cantidad.
        private MovimientoStock Consumo(Insumo pInsumo, double pCantidad, DateTime pFecha, string pMotivo)
        {
            if (pInsumo == null || pCantidad <= 0)
            {
                return null;
            }

            return new MovimientoStock(0, MovimientoStock.EGRESO, pCantidad, pFecha,
                DateTime.MinValue, pMotivo, pInsumo);
        }

        // Arma la lista de movimientos que va a la persistencia salteando los nulos,
        // que son los casos en que no hay nada que devolver ni que consumir.
        private List<MovimientoStock> ArmarMovimientos(MovimientoStock pDevolucion,
            MovimientoStock pConsumo)
        {
            List<MovimientoStock> _listaMovimientos = new List<MovimientoStock>();

            if (pDevolucion != null)
            {
                _listaMovimientos.Add(pDevolucion);
            }

            if (pConsumo != null)
            {
                _listaMovimientos.Add(pConsumo);
            }
            return _listaMovimientos;
        }

        // Las hembras que una correccion puede dejar con otro estado: la que tenia el
        // registro y la que pasa a tenerlo. Cuando la correccion no cambio de animal
        // las dos son la misma y la lista queda con una sola.
        private List<Hembra> HembrasAfectadas(Hembra pAnterior, Hembra pNueva)
        {
            List<Hembra> _listaAfectadas = new List<Hembra>();

            if (pAnterior != null)
            {
                _listaAfectadas.Add(pAnterior);
            }

            if (pNueva != null && (pAnterior == null || pNueva.IdAnimal != pAnterior.IdAnimal))
            {
                _listaAfectadas.Add(pNueva);
            }
            return _listaAfectadas;
        }

        // Por cada hembra afectada, el estado reproductivo y la fecha probable de parto
        // de la lactancia que le quedan despues de la correccion. A las listas de
        // salida solo entran las que efectivamente cambian: lo que no cambio no se
        // reescribe.
        private void RecalcularHembras(List<Hembra> pAfectadas, int pIdServicioIgnorado,
            int pIdTactoIgnorado, int pIdPartoIgnorado, List<Hembra> pHembrasActualizadas,
            List<Lactancia> pLactanciasActualizadas)
        {
            foreach (Hembra unaHembra in pAfectadas)
            {
                Hembra unaHembraNueva = this.HembraRecalculada(unaHembra, pIdServicioIgnorado,
                    pIdTactoIgnorado, pIdPartoIgnorado);

                if (unaHembraNueva != null)
                {
                    pHembrasActualizadas.Add(unaHembraNueva);
                }

                Lactancia unaLactanciaNueva = this.LactanciaRecalculada(unaHembra, pIdServicioIgnorado,
                    pIdTactoIgnorado, pIdPartoIgnorado);

                if (unaLactanciaNueva != null)
                {
                    pLactanciasActualizadas.Add(unaLactanciaNueva);
                }
            }
        }

        // Vuelca a la cache los estados recalculados, una vez que la escritura salio
        // bien. Hasta aca las copias eran objetos aparte, para no dejar la cache
        // adelantada a la base si la transaccion se caia.
        private void AplicarRecalculoEnCache(List<Hembra> pHembrasActualizadas,
            List<Lactancia> pLactanciasActualizadas)
        {
            foreach (Hembra unaHembraNueva in pHembrasActualizadas)
            {
                Hembra unaHembra = this.BuscarHembra(unaHembraNueva.IdAnimal);
                if (unaHembra != null)
                {
                    unaHembra.NumeroPartos = unaHembraNueva.NumeroPartos;
                    unaHembra.EstadoProductivo = unaHembraNueva.EstadoProductivo;
                    unaHembra.EstadoReproductivo = unaHembraNueva.EstadoReproductivo;
                }
            }

            foreach (Lactancia unaLactanciaNueva in pLactanciasActualizadas)
            {
                Lactancia unaLactancia = this.BuscarLactancia(unaLactanciaNueva.IdLactancia);
                if (unaLactancia != null)
                {
                    unaLactancia.FechaInicio = unaLactanciaNueva.FechaInicio;
                    unaLactancia.FechaSecado = unaLactanciaNueva.FechaSecado;
                    unaLactancia.FechaProbableParto = unaLactanciaNueva.FechaProbableParto;
                }
            }
        }

        // Aplica en la cache los movimientos que se acaban de asentar, para que el
        // stock que ve la pantalla despues de la correccion sea el nuevo y no el que
        // habia al empezar la peticion.
        private void AplicarMovimientosEnCache(List<MovimientoStock> pMovimientos)
        {
            foreach (MovimientoStock unMovimiento in pMovimientos)
            {
                if (unMovimiento.Insumo == null)
                {
                    continue;
                }

                unMovimiento.Insumo.StockActual = unMovimiento.TipoMovimiento == MovimientoStock.INGRESO
                    ? unMovimiento.Insumo.StockActual + unMovimiento.Cantidad
                    : unMovimiento.Insumo.StockActual - unMovimiento.Cantidad;

                this.AgregarMovimientoOrdenado(unMovimiento);
            }
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
        // Devuelve el motivo por el que el celo no se puede registrar, o una cadena
        // vacia si esta bien.
        public string ValidarCelo(Celo pCelo)
        {
            if (pCelo.Animal == null)
            {
                return "Hay que indicar la hembra en celo!";
            }

            if (pCelo.Fecha > DateTime.Now)
            {
                return "La fecha del celo no puede ser posterior a la fecha actual!";
            }

            // Una ternera no cicla. Anotarle un celo es un error de caravana, y el
            // servicio que vendria despues seria imposible.
            int vEdadAlCelo = this.DiferenciaMeses(pCelo.Animal.FechaNacimiento, pCelo.Fecha);
            if (vEdadAlCelo < EDAD_MINIMA_CELO_MESES)
            {
                return "El animal tenia " + vEdadAlCelo + " meses en esa fecha: la hembra empieza a manifestar celo "
                    + "a partir de los " + EDAD_MINIMA_CELO_MESES + " meses. Revise la caravana o la fecha!";
            }

            if (!this.EstabaActivo(pCelo.Animal, pCelo.Fecha))
            {
                return "El animal figura dado de baja el " + pCelo.Animal.FechaBaja.ToShortDateString()
                    + ": no se le puede registrar un celo posterior a esa fecha!";
            }

            return "";
        }

        public bool AltaCelo(Celo pCelo)
        {
            if (this.ValidarCelo(pCelo) != "")
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

        // La correccion revalida con la misma regla del alta: un celo corregido tiene
        // que seguir siendo un celo posible -la hembra en edad de ciclar, la fecha no
        // futura, el animal activo a esa fecha-, si no el error se cambia por otro.
        public string ValidarModificarCelo(int pIdCelo, DateTime pFecha, int pIdHembra)
        {
            this.Refrescar();

            if (this.BuscarCelo(pIdCelo) == null)
            {
                return "El celo que se quiere corregir ya no existe!";
            }

            Hembra unaHembra = this.BuscarHembra(pIdHembra);
            if (unaHembra == null)
            {
                return "Hay que indicar la hembra en celo!";
            }

            return this.ValidarCelo(new Celo(pIdCelo, pFecha, "", unaHembra));
        }

        public bool ModificarCelo(int pIdCelo, DateTime pFecha, string pObservaciones, int pIdHembra)
        {
            if (this.ValidarModificarCelo(pIdCelo, pFecha, pIdHembra) != "")
            {
                return false;
            }

            Celo unCelo = this.BuscarCelo(pIdCelo);
            Hembra unaHembra = this.BuscarHembra(pIdHembra);
            Celo unCeloNuevo = new Celo(pIdCelo, pFecha, pObservaciones, unaHembra);

            if (Persistencia.ModificarCelo(unCeloNuevo))
            {
                unCelo.Fecha = pFecha;
                unCelo.Observaciones = pObservaciones;
                unCelo.Animal = unaHembra;
                return true;
            }
            return false;

        }

        // El celo es el unico registro de reproduccion que no se bloquea nunca: no mueve
        // el estado de la hembra, no consume stock y ningun otro registro lo referencia.
        // Anotarlo es dejar constancia de que se vio a la vaca en celo, y borrarlo es
        // decir que no se la vio.
        public string ValidarEliminarCelo(int pIdCelo)
        {
            this.Refrescar();

            if (this.BuscarCelo(pIdCelo) == null)
            {
                return "El celo que se quiere eliminar ya no existe!";
            }
            return "";
        }

        public bool EliminarCelo(int pIdCelo)
        {
            if (this.ValidarEliminarCelo(pIdCelo) != "")
            {
                return false;
            }

            Celo unCelo = this.BuscarCelo(pIdCelo);

            if (Persistencia.EliminarCelo(pIdCelo))
            {
                mListaCelos.Remove(unCelo);
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
            return this.ValidarServicio(pServicio, 0);
        }

        // La misma validacion, con el identificador del servicio que se esta
        // corrigiendo. En cero es la del alta. Con un identificador hay dos reglas que
        // se evaluan distinto, porque son las que un servicio ya registrado viola
        // contra si mismo:
        //
        //   - la preniez de la hembra se mira ignorando este servicio. Si la vaca esta
        //     prenada por el servicio que se quiere corregir, corregirlo es justamente
        //     lo que hay que poder hacer.
        //   - el stock de la pajuela solo se exige cuando la correccion cambia de
        //     pajuela. La que ya se habia descontado no se vuelve a descontar, y
        //     exigirle stock impediria corregirle la fecha al ultimo servicio hecho con
        //     la ultima pajuela de una partida.
        private string ValidarServicio(Servicio pServicio, int pIdServicioIgnorado)
        {
            if (pServicio.Animal == null)
            {
                return "Hay que indicar la hembra que recibe el servicio!";
            }

            if (pServicio.FechaServicio > DateTime.Now)
            {
                return "La fecha del servicio no puede ser posterior a la fecha actual!";
            }

            // Una ternera no entra en servicio: no esta desarrollada para gestar y el
            // servicio compromete su crecimiento. Es el mismo umbral con el que
            // ValidarGenealogia rechaza a una madre demasiado joven, aplicado ahora en
            // el momento en que el dato se carga y no recien cuando nace la cria.
            int vEdadAlServicio = this.DiferenciaMeses(pServicio.Animal.FechaNacimiento, pServicio.FechaServicio);
            if (vEdadAlServicio < this.Parametros().EdadMinimaServicioMeses)
            {
                return "El animal tenia " + vEdadAlServicio + " meses en esa fecha: la edad minima para entrar "
                    + "en servicio es de " + this.Parametros().EdadMinimaServicioMeses + " meses. Revise la caravana o la fecha!";
            }

            // La hembra tiene que haber estado en el rodeo ese dia
            if (!this.EstabaActivo(pServicio.Animal, pServicio.FechaServicio))
            {
                return "El animal figura dado de baja el " + pServicio.Animal.FechaBaja.ToShortDateString()
                    + ": no se le puede registrar un servicio posterior a esa fecha!";
            }

            // Servir una hembra prenada le provoca el aborto. Si la preniez estaba mal
            // confirmada, el camino es registrar el tacto vacio y despues el servicio.
            string vEstadoReproductivo = pIdServicioIgnorado == 0
                ? pServicio.Animal.EstadoReproductivo
                : this.EstadoReproductivoDeducido(pServicio.Animal, pIdServicioIgnorado, 0, 0);

            if (vEstadoReproductivo == Hembra.PRENADA)
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
                // Curso de excepcion 5a. La pajuela que este servicio ya tenia
                // descontada no se vuelve a descontar, asi que tampoco se le exige
                // stock: solo se controla cuando la correccion cambia de pajuela.
                Servicio unServicioActual = this.BuscarServicio(pIdServicioIgnorado);
                bool vMismaPajuela = unServicioActual != null && unServicioActual.Pajuela != null
                    && unServicioActual.Pajuela.IdInsumo == pServicio.Pajuela.IdInsumo;

                if (!vMismaPajuela && pServicio.Pajuela.StockActual < 1)
                {
                    return "La pajuela seleccionada no tiene stock disponible!";
                }
            }

            return "";
        }

        // Lo que hace ruido en el servicio pero no lo vuelve imposible. No impide
        // guardar: la pantalla las muestra y el usuario decide si registrarlo igual.
        public List<string> AdvertenciasServicio(Servicio pServicio)
        {
            List<string> _listaAdvertencias = new List<string>();

            if (pServicio.Animal == null)
            {
                return _listaAdvertencias;
            }

            // El toro de la monta natural tiene que haber estado en el campo ese dia.
            // Si figura dado de baja antes, o el servicio es de otro toro o la fecha de
            // baja quedo mal cargada. Se advierte y no se bloquea porque la baja suele
            // registrarse dias despues del hecho, asi que la fecha almacenada puede ser
            // posterior a la real.
            //
            // La inseminacion no entra en esta advertencia: la pajuela sobrevive al
            // toro, y usar semen de un reproductor muerto es lo habitual.
            if (pServicio.Toro != null && !this.EstabaActivo(pServicio.Toro, pServicio.FechaServicio))
            {
                _listaAdvertencias.Add("El toro " + pServicio.Toro.NumCaravana + " figura dado de baja el "
                    + pServicio.Toro.FechaBaja.ToShortDateString() + ", antes de la fecha del servicio. "
                    + "Revise la fecha del servicio o la de la baja.");
            }

            // Parentesco entre la hembra y el reproductor, sea el toro del rodeo o el
            // que aporto la pajuela (CU6). La cria nace consanguinea.
            Macho unReproductor = this.ToroDelServicio(pServicio);
            if (unReproductor != null && this.VerificarConsanguinidad(pServicio.Animal, unReproductor))
            {
                _listaAdvertencias.Add("La hembra y el reproductor tienen parentesco: "
                    + this.TextoAncestroComun(pServicio.Animal, unReproductor)
                    + " La cria nace consanguinea.");
            }

            return _listaAdvertencias;
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

        // Curso alternativo 7a de CU15: el usuario ajusta la fecha probable de parto que
        // propuso el sistema. Es la correccion mas frecuente y por eso el listado la
        // ofrece a mano, pero por dentro es la correccion completa dejando el resto de
        // los campos como estaban.
        public bool ModificarServicio(int pIdServicio, DateTime pFechaProbableParto)
        {
            this.Refrescar();

            Servicio unServicio = this.BuscarServicio(pIdServicio);
            if (unServicio == null || unServicio.Animal == null)
            {
                return false;
            }

            return this.ModificarServicio(pIdServicio, unServicio.TipoServicio,
                unServicio.FechaServicio, pFechaProbableParto, unServicio.Observaciones,
                unServicio.Animal.IdAnimal,
                unServicio.Toro != null ? unServicio.Toro.IdAnimal : 0,
                unServicio.Pajuela != null ? unServicio.Pajuela.IdInsumo : 0);
        }

        // Lo que impide corregir el servicio, o una cadena vacia si se puede. Ademas de
        // las reglas del alta hay dos que solo existen en la correccion, y las dos
        // vienen del tacto: el control se hace sobre un servicio concreto de un animal
        // concreto, asi que ni el animal ni la fecha pueden quedar en contra de un
        // tacto ya registrado.
        public string ValidarModificarServicio(int pIdServicio, string pTipoServicio,
            DateTime pFechaServicio, DateTime pFechaProbableParto, int pIdHembra,
            int pIdToro, int pIdPajuela)
        {
            this.Refrescar();

            Servicio unServicio = this.BuscarServicio(pIdServicio);
            if (unServicio == null)
            {
                return "El servicio que se quiere corregir ya no existe!";
            }

            Hembra unaHembra = this.BuscarHembra(pIdHembra);
            if (unaHembra == null)
            {
                return "Hay que indicar la hembra que recibe el servicio!";
            }

            List<Tacto> _listaTactos = this.FiltrarTactosXServicio(pIdServicio);

            // Pasar el servicio a otro animal dejaria esos controles confirmando la
            // preniez de una vaca que nunca fue servida.
            if (_listaTactos.Count > 0 && unServicio.Animal != null
                && unaHembra.IdAnimal != unServicio.Animal.IdAnimal)
            {
                return "El servicio tiene " + _listaTactos.Count + " tacto(s) registrado(s) sobre la caravana "
                    + unServicio.Animal.NumCaravana + ": para pasarlo a otro animal hay que eliminar "
                    + "antes esos tactos!";
            }

            // El tacto se hace despues del servicio, nunca antes
            foreach (Tacto unTacto in _listaTactos)
            {
                if (unTacto.FechaTacto.Date < pFechaServicio.Date)
                {
                    return "El servicio tiene un tacto del " + unTacto.FechaTacto.ToShortDateString()
                        + ": la fecha del servicio no puede ser posterior a la del tacto!";
                }
            }

            if (pFechaProbableParto.Date <= pFechaServicio.Date)
            {
                return "La fecha probable de parto tiene que ser posterior a la del servicio!";
            }

            Servicio unServicioNuevo = new Servicio(pIdServicio, pTipoServicio, pFechaServicio,
                pFechaProbableParto, "", unaHembra, this.BuscarMacho(pIdToro),
                this.BuscarInsumo(pIdPajuela));

            return this.ValidarServicio(unServicioNuevo, pIdServicio);
        }

        // La correccion completa del servicio. Vuelve a escribir todos los campos y
        // arrastra los tres efectos que el alta habia dejado: el estado reproductivo de
        // la hembra, la fecha probable de parto proyectada sobre la lactancia en curso
        // y, si cambio la pajuela, el stock.
        public bool ModificarServicio(int pIdServicio, string pTipoServicio, DateTime pFechaServicio,
            DateTime pFechaProbableParto, string pObservaciones, int pIdHembra, int pIdToro,
            int pIdPajuela)
        {
            if (this.ValidarModificarServicio(pIdServicio, pTipoServicio, pFechaServicio,
                pFechaProbableParto, pIdHembra, pIdToro, pIdPajuela) != "")
            {
                return false;
            }

            Servicio unServicio = this.BuscarServicio(pIdServicio);
            Hembra unaHembraAnterior = unServicio.Animal;
            Insumo unaPajuelaAnterior = unServicio.Pajuela;

            Hembra unaHembra = this.BuscarHembra(pIdHembra);
            Macho unToro = this.BuscarMacho(pIdToro);
            Insumo unaPajuela = this.BuscarInsumo(pIdPajuela);

            Servicio unServicioNuevo = new Servicio(pIdServicio, pTipoServicio, pFechaServicio,
                pFechaProbableParto, pObservaciones, unaHembra, unToro, unaPajuela);

            // Los datos nuevos se vuelcan a la cache antes de deducir los estados. La
            // deduccion mira los registros que hay, no un historial de cambios, asi que
            // para saber como queda la hembra despues de la correccion el servicio ya
            // tiene que ser el corregido. Si la escritura falla se vuelve atras.
            Servicio unServicioAnterior = this.CopiarServicio(unServicio);
            this.VolcarServicio(unServicioNuevo, unServicio);

            List<Hembra> _listaHembras = new List<Hembra>();
            List<Lactancia> _listaLactancias = new List<Lactancia>();
            this.RecalcularHembras(this.HembrasAfectadas(unaHembraAnterior, unaHembra), 0, 0, 0,
                _listaHembras, _listaLactancias);

            List<MovimientoStock> _listaMovimientos = this.MovimientosPorCambioDeInsumo(
                unaPajuelaAnterior, unaPajuela, 1, 1, pFechaServicio,
                "Ajuste por correccion del servicio de la caravana " + unaHembra.NumCaravana,
                "Inseminación de la caravana " + unaHembra.NumCaravana);

            try
            {
                if (Persistencia.ModificarServicio(unServicioNuevo, _listaLactancias,
                    _listaHembras, _listaMovimientos))
                {
                    this.AplicarRecalculoEnCache(_listaHembras, _listaLactancias);
                    this.AplicarMovimientosEnCache(_listaMovimientos);
                    return true;
                }

                this.VolcarServicio(unServicioAnterior, unServicio);
                return false;
            }
            catch (Exception)
            {
                this.VolcarServicio(unServicioAnterior, unServicio);
                throw;
            }
        }

        // Lo que impide eliminar el servicio, o una cadena vacia si se puede. El unico
        // bloqueo es el tacto: un control de gestacion sin el servicio que lo origino
        // no significa nada, y borrarlos en cascada seria borrar el trabajo del
        // veterinario sin avisar.
        public string ValidarEliminarServicio(int pIdServicio)
        {
            this.Refrescar();

            if (this.BuscarServicio(pIdServicio) == null)
            {
                return "El servicio que se quiere eliminar ya no existe!";
            }

            List<Tacto> _listaTactos = this.FiltrarTactosXServicio(pIdServicio);
            if (_listaTactos.Count > 0)
            {
                return "El servicio tiene " + _listaTactos.Count + " tacto(s) registrado(s): "
                    + "elimine primero esos tactos desde Reproduccion, Tactos!";
            }
            return "";
        }

        // El borrado deja a la hembra con el estado que le dan los servicios que le
        // quedan y devuelve al stock la pajuela que en realidad nunca se uso.
        public bool EliminarServicio(int pIdServicio)
        {
            if (this.ValidarEliminarServicio(pIdServicio) != "")
            {
                return false;
            }

            Servicio unServicio = this.BuscarServicio(pIdServicio);

            List<Hembra> _listaHembras = new List<Hembra>();
            List<Lactancia> _listaLactancias = new List<Lactancia>();
            this.RecalcularHembras(this.HembrasAfectadas(unServicio.Animal, null), pIdServicio, 0, 0,
                _listaHembras, _listaLactancias);

            List<MovimientoStock> _listaMovimientos = this.ArmarMovimientos(
                this.Devolucion(unServicio.Pajuela, 1,
                    "Devolucion por eliminacion del servicio del "
                    + unServicio.FechaServicio.ToShortDateString()),
                null);

            if (Persistencia.EliminarServicio(pIdServicio, _listaLactancias, _listaHembras,
                _listaMovimientos))
            {
                mListaServicios.Remove(unServicio);
                this.AplicarRecalculoEnCache(_listaHembras, _listaLactancias);
                this.AplicarMovimientosEnCache(_listaMovimientos);
                return true;
            }
            return false;

        }

        // Los movimientos que deja un cambio de producto o de cantidad: se devuelve lo
        // que se habia consumido y se descuenta lo nuevo. Cuando el producto y la
        // cantidad quedan iguales no hay nada que mover, que es el caso habitual.
        private List<MovimientoStock> MovimientosPorCambioDeInsumo(Insumo pInsumoAnterior,
            Insumo pInsumoNuevo, double pCantidadAnterior, double pCantidadNueva, DateTime pFecha,
            string pMotivoDevolucion, string pMotivoConsumo)
        {
            bool vMismoInsumo = pInsumoAnterior != null && pInsumoNuevo != null
                && pInsumoAnterior.IdInsumo == pInsumoNuevo.IdInsumo;

            if (vMismoInsumo && pCantidadAnterior == pCantidadNueva)
            {
                return new List<MovimientoStock>();
            }

            return this.ArmarMovimientos(
                this.Devolucion(pInsumoAnterior, pCantidadAnterior, pMotivoDevolucion),
                this.Consumo(pInsumoNuevo, pCantidadNueva, pFecha, pMotivoConsumo));
        }

        private Servicio CopiarServicio(Servicio pServicio)
        {
            return new Servicio(pServicio.IdServicio, pServicio.TipoServicio, pServicio.FechaServicio,
                pServicio.FechaProbableParto, pServicio.Observaciones, pServicio.Animal,
                pServicio.Toro, pServicio.Pajuela);
        }

        private void VolcarServicio(Servicio pOrigen, Servicio pDestino)
        {
            pDestino.TipoServicio = pOrigen.TipoServicio;
            pDestino.FechaServicio = pOrigen.FechaServicio;
            pDestino.FechaProbableParto = pOrigen.FechaProbableParto;
            pDestino.Observaciones = pOrigen.Observaciones;
            pDestino.Animal = pOrigen.Animal;
            pDestino.Toro = pOrigen.Toro;
            pDestino.Pajuela = pOrigen.Pajuela;
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
            return this.ServicioVigente(pHembra, 0, 0);
        }

        // El mismo servicio vigente, pero salteando el servicio y el parto que se estan
        // por eliminar. La correccion necesita saber como queda la hembra cuando esos
        // registros ya no esten, y en ese momento todavia figuran en la cache. Los dos
        // identificadores van en cero cuando no hay nada que saltear.
        private Servicio ServicioVigente(Hembra pHembra, int pIdServicioIgnorado, int pIdPartoIgnorado)
        {
            if (pHembra == null)
            {
                return null;
            }

            DateTime vUltimoParto = DateTime.MinValue;
            foreach (Parto unParto in this.FiltrarPartosXHembra(pHembra.IdAnimal))
            {
                if (unParto.IdParto == pIdPartoIgnorado)
                {
                    continue;
                }

                if (unParto.FechaParto > vUltimoParto)
                {
                    vUltimoParto = unParto.FechaParto;
                }
            }

            Servicio unServicioVigente = null;
            foreach (Servicio unServicio in this.FiltrarServiciosXHembra(pHembra.IdAnimal))
            {
                if (unServicio.IdServicio == pIdServicioIgnorado)
                {
                    continue;
                }

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

                if (unServicio.FechaProbableParto.Date <= DateTime.Now.AddDays(this.Parametros().DiasAnticipacionParto).Date)
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
            return this.UltimoTacto(pServicio, 0);
        }

        // El ultimo tacto salteando el que se esta por eliminar, para deducir con que
        // estado queda la hembra cuando ese control ya no exista. Va en cero cuando no
        // hay nada que saltear.
        private Tacto UltimoTacto(Servicio pServicio, int pIdTactoIgnorado)
        {
            if (pServicio == null)
            {
                return null;
            }

            Tacto unUltimoTacto = null;
            foreach (Tacto unTacto in this.FiltrarTactosXServicio(pServicio.IdServicio))
            {
                if (unTacto.IdTacto == pIdTactoIgnorado)
                {
                    continue;
                }

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

        // Lo que impide corregir el tacto, o una cadena vacia si se puede. Son las
        // mismas condiciones que valida el alta: el control se hace sobre un servicio,
        // despues de ese servicio y no en el futuro, y con uno de los tres resultados.
        public string ValidarModificarTacto(int pIdTacto, int pIdServicio, DateTime pFechaTacto,
            string pResultado)
        {
            this.Refrescar();

            if (this.BuscarTacto(pIdTacto) == null)
            {
                return "El tacto que se quiere corregir ya no existe!";
            }

            Servicio unServicio = this.BuscarServicio(pIdServicio);
            if (unServicio == null)
            {
                return "Hay que indicar el servicio al que corresponde el tacto!";
            }

            if (pResultado != Tacto.PRENADA && pResultado != Tacto.VACIA && pResultado != Tacto.DUDOSA)
            {
                return "Hay que indicar el resultado del tacto!";
            }

            if (pFechaTacto > DateTime.Now)
            {
                return "La fecha del tacto no puede ser posterior a la fecha actual!";
            }

            if (pFechaTacto.Date < unServicio.FechaServicio.Date)
            {
                return "El tacto no puede ser anterior al servicio del "
                    + unServicio.FechaServicio.ToShortDateString() + "!";
            }
            return "";
        }

        // Corregir un tacto es la correccion que mas se nota: cambia si la vaca figura
        // prenada o vacia, y con eso si vuelve a la lista para servir, si aparece en las
        // alertas de parto y con que fecha se la seca. Todo eso se recalcula solo,
        // porque sale de los tactos que hay y no de un estado guardado aparte.
        public bool ModificarTacto(int pIdTacto, int pIdServicio, DateTime pFechaTacto,
            string pResultado, string pObservaciones)
        {
            if (this.ValidarModificarTacto(pIdTacto, pIdServicio, pFechaTacto, pResultado) != "")
            {
                return false;
            }

            Tacto unTacto = this.BuscarTacto(pIdTacto);
            Servicio unServicio = this.BuscarServicio(pIdServicio);

            Hembra unaHembraAnterior = unTacto.Servicio != null ? unTacto.Servicio.Animal : null;
            Hembra unaHembra = unServicio.Animal;

            Tacto unTactoAnterior = this.CopiarTacto(unTacto);
            Tacto unTactoNuevo = new Tacto(pIdTacto, pFechaTacto, pResultado, pObservaciones, unServicio);
            this.VolcarTacto(unTactoNuevo, unTacto);

            List<Hembra> _listaHembras = new List<Hembra>();
            List<Lactancia> _listaLactancias = new List<Lactancia>();
            this.RecalcularHembras(this.HembrasAfectadas(unaHembraAnterior, unaHembra), 0, 0, 0,
                _listaHembras, _listaLactancias);

            try
            {
                if (Persistencia.ModificarTacto(unTactoNuevo, _listaHembras, _listaLactancias))
                {
                    this.AplicarRecalculoEnCache(_listaHembras, _listaLactancias);
                    return true;
                }

                this.VolcarTacto(unTactoAnterior, unTacto);
                return false;
            }
            catch (Exception)
            {
                this.VolcarTacto(unTactoAnterior, unTacto);
                throw;
            }
        }

        // Del tacto no cuelga nada, asi que nunca se bloquea. La hembra vuelve al
        // estado que le dan los tactos que le quedan a ese servicio: servida si no
        // queda ninguno, que es como estaba antes del control.
        public string ValidarEliminarTacto(int pIdTacto)
        {
            this.Refrescar();

            if (this.BuscarTacto(pIdTacto) == null)
            {
                return "El tacto que se quiere eliminar ya no existe!";
            }
            return "";
        }

        public bool EliminarTacto(int pIdTacto)
        {
            if (this.ValidarEliminarTacto(pIdTacto) != "")
            {
                return false;
            }

            Tacto unTacto = this.BuscarTacto(pIdTacto);
            Hembra unaHembra = unTacto.Servicio != null ? unTacto.Servicio.Animal : null;

            List<Hembra> _listaHembras = new List<Hembra>();
            List<Lactancia> _listaLactancias = new List<Lactancia>();
            this.RecalcularHembras(this.HembrasAfectadas(unaHembra, null), 0, pIdTacto, 0,
                _listaHembras, _listaLactancias);

            if (Persistencia.EliminarTacto(pIdTacto, _listaHembras, _listaLactancias))
            {
                mListaTactos.Remove(unTacto);
                this.AplicarRecalculoEnCache(_listaHembras, _listaLactancias);
                return true;
            }
            return false;

        }

        private Tacto CopiarTacto(Tacto pTacto)
        {
            return new Tacto(pTacto.IdTacto, pTacto.FechaTacto, pTacto.Resultado,
                pTacto.Observaciones, pTacto.Servicio);
        }

        private void VolcarTacto(Tacto pOrigen, Tacto pDestino)
        {
            pDestino.FechaTacto = pOrigen.FechaTacto;
            pDestino.Resultado = pOrigen.Resultado;
            pDestino.Observaciones = pOrigen.Observaciones;
            pDestino.Servicio = pOrigen.Servicio;
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

        // Lo que hace ruido en el parto pero no lo vuelve imposible. Igual que en el
        // servicio, no impide guardar: son datos que en el campo pasan y que el usuario
        // tiene que ver antes de confirmar.
        public List<string> AdvertenciasParto(Parto pParto, List<Animal> pListaCrias)
        {
            List<string> _listaAdvertencias = new List<string>();

            if (pParto.Madre == null || pListaCrias == null || pListaCrias.Count == 0)
            {
                return _listaAdvertencias;
            }

            // Mellizos de distinto sexo: la hembra nace freemartin. Comparte
            // circulacion con el hermano macho durante la gestacion y en la enorme
            // mayoria de los casos queda esteril, asi que no conviene criarla como
            // futura vaca lechera. Es la advertencia que evita descubrirlo dos anios
            // despues, cuando la vaquillona no prende con ningun servicio.
            if (pListaCrias.Count > 1)
            {
                bool vHayHembra = false;
                bool vHayMacho = false;

                foreach (Animal unaCria in pListaCrias)
                {
                    if (unaCria is Hembra)
                    {
                        vHayHembra = true;
                    }
                    else
                    {
                        vHayMacho = true;
                    }
                }

                if (vHayHembra && vHayMacho)
                {
                    _listaAdvertencias.Add("Mellizos de distinto sexo: la cria hembra nace freemartin y "
                        + "lo mas probable es que sea esteril. Conviene no destinarla a reposicion.");
                }
            }

            // El parto de una vaca que no figuraba prenada significa que falto
            // registrar el servicio o el tacto. No impide el parto -el ternero esta
            // ahi-, pero deja el historial reproductivo incompleto.
            if (pParto.Madre.EstadoReproductivo != Hembra.PRENADA)
            {
                _listaAdvertencias.Add("El animal no figuraba prenado, sino \"" + pParto.Madre.EstadoReproductivo
                    + "\". Falta registrar el servicio o el tacto que confirmo la preniez.");
            }

            // Duracion de la gestacion contra el servicio del que viene la preniez. Muy
            // corta es un aborto o un prematuro; muy larga, una fecha mal cargada.
            Servicio unServicio = this.ServicioVigente(pParto.Madre);
            if (unServicio != null)
            {
                int vDiasGestacion = (int)(pParto.FechaParto.Date - unServicio.FechaServicio.Date).TotalDays;

                if (vDiasGestacion < GESTACION_DIAS_MINIMA)
                {
                    _listaAdvertencias.Add("Entre el servicio del " + unServicio.FechaServicio.ToShortDateString()
                        + " y este parto pasaron " + vDiasGestacion + " dias, menos que los "
                        + GESTACION_DIAS_MINIMA + " de una gestacion viable. Revise las fechas.");
                }
                else if (vDiasGestacion > GESTACION_DIAS_MAXIMA)
                {
                    _listaAdvertencias.Add("Entre el servicio del " + unServicio.FechaServicio.ToShortDateString()
                        + " y este parto pasaron " + vDiasGestacion + " dias, mas que los "
                        + GESTACION_DIAS_MAXIMA + " de una gestacion normal. Puede faltar registrar un servicio.");
                }
            }

            return _listaAdvertencias;
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

        // La lactancia que abrio el parto: la de esa vaca que empieza el mismo dia.
        // partos y lactancias son tablas independientes -el modelo no las vincula con
        // una foranea-, asi que el vinculo se reconstruye por la fecha. No queda
        // ambiguo: una vaca no puede tener dos lactancias que empiecen el mismo dia.
        public Lactancia LactanciaDelParto(Parto pParto)
        {
            if (pParto == null || pParto.Madre == null)
            {
                return null;
            }

            foreach (Lactancia unaLactancia in this.FiltrarLactanciasXHembra(pParto.Madre.IdAnimal))
            {
                if (unaLactancia.FechaInicio.Date == pParto.FechaParto.Date)
                {
                    return unaLactancia;
                }
            }
            return null;
        }

        // La lactancia que el parto habia cerrado, para poder volver a abrirla. Se la
        // reconoce porque quedo secada justo el dia del parto y habia empezado antes.
        //
        // Es la misma reconstruccion por fecha que la anterior, con una salvedad: si el
        // secado se hubiera registrado a mano ese mismo dia, el borrado del parto la
        // reabre igual. Es poco probable -el parto cierra la lactancia solo cuando el
        // secado no se registro- y la pantalla lo muestra antes de confirmar.
        public Lactancia LactanciaCerradaPorParto(Parto pParto)
        {
            if (pParto == null || pParto.Madre == null)
            {
                return null;
            }

            foreach (Lactancia unaLactancia in this.FiltrarLactanciasXHembra(pParto.Madre.IdAnimal))
            {
                if (unaLactancia.FechaSecado != DateTime.MinValue
                    && unaLactancia.FechaSecado.Date == pParto.FechaParto.Date
                    && unaLactancia.FechaInicio.Date < pParto.FechaParto.Date)
                {
                    return unaLactancia;
                }
            }
            return null;
        }

        // Las crias que dio ese parto: los animales de esa madre nacidos el dia del
        // parto. Tampoco hay foranea entre partos y animales, y tampoco es ambiguo:
        // una vaca no pare dos veces el mismo dia.
        public List<Animal> CriasDelParto(Parto pParto)
        {
            List<Animal> _listaCrias = new List<Animal>();

            if (pParto == null || pParto.Madre == null)
            {
                return _listaCrias;
            }

            foreach (Animal unAnimal in this.ListarAnimales())
            {
                if (unAnimal.Madre != null && unAnimal.Madre.IdAnimal == pParto.Madre.IdAnimal
                    && unAnimal.FechaNacimiento.Date == pParto.FechaParto.Date)
                {
                    _listaCrias.Add(unAnimal);
                }
            }
            return _listaCrias;
        }

        // Lo que impide borrar fisicamente a un animal, o una cadena vacia si no hay
        // nada. Lo usa el borrado del parto, que se lleva las crias que ese parto habia
        // dado de alta: una cria que ya tiene historia propia no es un error de carga,
        // es un animal del rodeo, y el parto no se puede deshacer sin decidir antes que
        // hacer con ella.
        public string MotivoAnimalNoEliminable(Animal pAnimal)
        {
            if (pAnimal == null)
            {
                return "";
            }

            if (this.ListarDescendencia(pAnimal).Count > 0)
            {
                return "ya tiene descendencia registrada";
            }

            if (this.FiltrarCelosXHembra(pAnimal.IdAnimal).Count > 0)
            {
                return "tiene celos registrados";
            }

            if (this.FiltrarServiciosXHembra(pAnimal.IdAnimal).Count > 0)
            {
                return "tiene servicios registrados";
            }

            if (this.FiltrarPartosXHembra(pAnimal.IdAnimal).Count > 0)
            {
                return "tiene partos registrados";
            }

            if (this.FiltrarLactanciasXHembra(pAnimal.IdAnimal).Count > 0)
            {
                return "tiene lactancias registradas";
            }

            if (this.FiltrarDiagnosticosXAnimal(pAnimal.IdAnimal).Count > 0)
            {
                return "tiene diagnosticos registrados";
            }

            if (this.FiltrarTratamientosXAnimal(pAnimal.IdAnimal).Count > 0)
            {
                return "tiene tratamientos registrados";
            }

            if (this.FiltrarVacunacionesXAnimal(pAnimal.IdAnimal).Count > 0)
            {
                return "tiene vacunaciones registradas";
            }

            if (this.FiltrarDescornesXAnimal(pAnimal.IdAnimal).Count > 0)
            {
                return "tiene descornes registrados";
            }

            // Un macho puede estar declarado como el toro que aporta una pajuela del
            // stock, aunque no tenga ningun otro registro propio.
            foreach (Insumo unInsumo in this.ListarInsumos())
            {
                if (unInsumo.Toro != null && unInsumo.Toro.IdAnimal == pAnimal.IdAnimal)
                {
                    return "figura como el toro de la pajuela " + unInsumo.Nombre;
                }
            }

            return "";
        }

        // Lo que impide corregir el parto, o una cadena vacia si se puede. La fecha es
        // lo unico delicado: arrastra el inicio de la lactancia que el parto abrio, el
        // secado de la que cerro y el nacimiento de las crias, y ninguna de las tres
        // cosas puede terminar contradiciendo un registro posterior.
        public string ValidarModificarParto(int pIdParto, DateTime pFechaParto, string pTipoParto)
        {
            this.Refrescar();

            Parto unParto = this.BuscarParto(pIdParto);
            if (unParto == null)
            {
                return "El parto que se quiere corregir ya no existe!";
            }

            if (unParto.Madre == null)
            {
                return "El parto no tiene madre registrada: no se puede corregir!";
            }

            if (pTipoParto != Parto.NORMAL && pTipoParto != Parto.DISTOCICO && pTipoParto != Parto.CESAREA)
            {
                return "Hay que indicar el tipo de parto!";
            }

            if (pFechaParto > DateTime.Now)
            {
                return "La fecha del parto no puede ser posterior a la fecha actual!";
            }

            // El parto anterior de la misma vaca
            foreach (Parto otroParto in this.FiltrarPartosXHembra(unParto.Madre.IdAnimal))
            {
                if (otroParto.IdParto != pIdParto && otroParto.FechaParto.Date >= pFechaParto.Date)
                {
                    return "La caravana " + unParto.Madre.NumCaravana + " tiene otro parto el "
                        + otroParto.FechaParto.ToShortDateString()
                        + ": la fecha corregida tiene que ser posterior!";
                }
            }

            // La lactancia que abrio el parto no puede empezar despues de los ordenies
            // que ya tiene imputados
            Lactancia unaLactancia = this.LactanciaDelParto(unParto);
            if (unaLactancia != null)
            {
                foreach (OrdenieIndividual unOrdenie in this.FiltrarOrdeniesXLactancia(unaLactancia.IdLactancia))
                {
                    if (unOrdenie.Fecha.Date < pFechaParto.Date)
                    {
                        return "La lactancia que abrio este parto tiene un control de ordenie del "
                            + unOrdenie.Fecha.ToShortDateString()
                            + ": la fecha del parto no puede ser posterior!";
                    }
                }
            }

            // Y la que el parto cerro no puede quedar secada antes de sus propios
            // ordenies
            Lactancia unaLactanciaCerrada = this.LactanciaCerradaPorParto(unParto);
            if (unaLactanciaCerrada != null)
            {
                foreach (OrdenieIndividual unOrdenie in this.FiltrarOrdeniesXLactancia(unaLactanciaCerrada.IdLactancia))
                {
                    if (unOrdenie.Fecha.Date > pFechaParto.Date)
                    {
                        return "La lactancia anterior tiene un control de ordenie del "
                            + unOrdenie.Fecha.ToShortDateString()
                            + ": el parto no puede ser anterior a ese control!";
                    }
                }
            }

            // Las crias nacieron el dia del parto: correrlo les corre la fecha de
            // nacimiento, y con otra fecha la genealogia puede dejar de cerrar.
            foreach (Animal unaCria in this.CriasDelParto(unParto))
            {
                string vMotivo = this.ValidarGenealogia(unaCria.IdAnimal, pFechaParto,
                    unaCria.Madre, unaCria.Padre);

                if (vMotivo != "")
                {
                    return "Con esa fecha, la cria " + unaCria.NumCaravana + " queda mal: " + vMotivo;
                }
            }

            return "";
        }

        public bool ModificarParto(int pIdParto, DateTime pFechaParto, string pTipoParto,
            string pObservaciones)
        {
            if (this.ValidarModificarParto(pIdParto, pFechaParto, pTipoParto) != "")
            {
                return false;
            }

            Parto unParto = this.BuscarParto(pIdParto);
            Parto unPartoNuevo = new Parto(pIdParto, pFechaParto, pTipoParto, pObservaciones,
                unParto.Madre);

            // La lactancia que el parto abrio empieza el dia del parto, y la que cerro
            // quedo secada ese dia: las dos se corren con el.
            List<Lactancia> _listaLactancias = new List<Lactancia>();

            Lactancia unaLactancia = this.LactanciaDelParto(unParto);
            if (unaLactancia != null)
            {
                _listaLactancias.Add(new Lactancia(unaLactancia.IdLactancia, unaLactancia.NumeroLactancia,
                    pFechaParto, unaLactancia.FechaSecado, unaLactancia.FechaProbableParto,
                    unaLactancia.Animal));
            }

            Lactancia unaLactanciaCerrada = this.LactanciaCerradaPorParto(unParto);
            if (unaLactanciaCerrada != null)
            {
                _listaLactancias.Add(this.CopiarLactancia(unaLactanciaCerrada, pFechaParto,
                    unaLactanciaCerrada.FechaProbableParto));
            }

            // Y las crias nacieron ese dia. Se resuelven antes de tocar nada: las crias
            // se reconocen por coincidir con la fecha del parto, asi que una vez movida
            // esa fecha ya no habria como encontrarlas.
            List<Animal> _listaCriasEnCache = this.CriasDelParto(unParto);
            List<Animal> _listaCrias = new List<Animal>();

            foreach (Animal unaCria in _listaCriasEnCache)
            {
                int vNumeroPartos = unaCria is Hembra ? ((Hembra)unaCria).NumeroPartos : 0;
                bool vEnPie = unaCria is Macho ? ((Macho)unaCria).EnPie : false;

                _listaCrias.Add(this.CopiarAnimal(unaCria, unaCria.NumCaravana, pFechaParto,
                    unaCria.Raza, unaCria.Categoria, unaCria.Madre, unaCria.Padre, vNumeroPartos,
                    vEnPie));
            }

            if (Persistencia.ModificarParto(unPartoNuevo, _listaLactancias, _listaCrias))
            {
                unParto.FechaParto = pFechaParto;
                unParto.TipoParto = pTipoParto;
                unParto.Observaciones = pObservaciones;

                this.AplicarRecalculoEnCache(new List<Hembra>(), _listaLactancias);

                foreach (Animal unaCria in _listaCriasEnCache)
                {
                    unaCria.FechaNacimiento = pFechaParto;
                }
                return true;
            }
            return false;

        }

        // Lo que impide eliminar el parto, o una cadena vacia si se puede. Deshacer un
        // parto es deshacer el alta de las crias, la apertura de la lactancia y el
        // cierre de la anterior, asi que se permite unicamente mientras nada de eso se
        // haya usado todavia.
        public string ValidarEliminarParto(int pIdParto)
        {
            this.Refrescar();

            Parto unParto = this.BuscarParto(pIdParto);
            if (unParto == null)
            {
                return "El parto que se quiere eliminar ya no existe!";
            }

            if (unParto.Madre == null)
            {
                return "El parto no tiene madre registrada: no se puede deshacer!";
            }

            // Deshacer uno del medio dejaria las lactancias numeradas con un salto y el
            // numero de partos de la vaca sin correspondencia con su historial.
            Parto unUltimoParto = this.UltimoParto(unParto.Madre);
            if (unUltimoParto != null && unUltimoParto.IdParto != pIdParto)
            {
                return "Solo se puede eliminar el ultimo parto de la vaca: el de la caravana "
                    + unParto.Madre.NumCaravana + " es el del "
                    + unUltimoParto.FechaParto.ToShortDateString() + "!";
            }

            Lactancia unaLactancia = this.LactanciaDelParto(unParto);
            if (unaLactancia != null)
            {
                List<OrdenieIndividual> _listaOrdenies = this.FiltrarOrdeniesXLactancia(unaLactancia.IdLactancia);

                if (_listaOrdenies.Count > 0)
                {
                    // Ahora que el control individual se puede dar de baja, el aviso
                    // dice donde: sin eso queda en "no se puede" y el usuario no tiene
                    // como avanzar, que es justo lo que este mensaje existe para evitar.
                    return "La lactancia que abrio este parto tiene " + _listaOrdenies.Count
                        + " control(es) de ordenie cargado(s): deshacer el parto borraria esa produccion. "
                        + "Eliminelos primero desde el historial de produccion!";
                }
            }

            foreach (Animal unaCria in this.CriasDelParto(unParto))
            {
                string vMotivo = this.MotivoAnimalNoEliminable(unaCria);

                if (vMotivo != "")
                {
                    return "La cria " + unaCria.NumCaravana + " " + vMotivo
                        + ": ya no es un error de carga, hay que darla de baja por Animales!";
                }
            }
            return "";
        }

        // El borrado del parto deshace todo lo que el alta habia dejado, en el orden
        // inverso: se van las crias y la lactancia que abrio, se reabre la que habia
        // cerrado y la madre vuelve a tener un parto menos, con el estado y la
        // categoria que le corresponden.
        public bool EliminarParto(int pIdParto)
        {
            if (this.ValidarEliminarParto(pIdParto) != "")
            {
                return false;
            }

            Parto unParto = this.BuscarParto(pIdParto);
            Hembra unaMadre = unParto.Madre;

            List<Animal> _listaCrias = this.CriasDelParto(unParto);
            Lactancia unaLactanciaDelParto = this.LactanciaDelParto(unParto);
            Lactancia unaLactanciaCerrada = this.LactanciaCerradaPorParto(unParto);

            Lactancia unaLactanciaReabierta = null;
            if (unaLactanciaCerrada != null)
            {
                unaLactanciaReabierta = this.CopiarLactancia(unaLactanciaCerrada, DateTime.MinValue,
                    unaLactanciaCerrada.FechaProbableParto);
            }

            // La madre pierde el parto. Queda en lactancia si se le reabre la anterior y
            // sin lactancia si no le queda ninguna, y con el estado reproductivo que le
            // dan los servicios y tactos que quedan.
            int vNumeroPartos = unaMadre.NumeroPartos > 0 ? unaMadre.NumeroPartos - 1 : 0;
            string vEstadoProductivo = unaLactanciaReabierta != null
                ? Hembra.EN_LACTANCIA
                : Hembra.SIN_LACTANCIA;

            Hembra unaMadreNueva = this.CopiarHembra(unaMadre, vNumeroPartos, vEstadoProductivo,
                this.EstadoReproductivoDeducido(unaMadre, 0, 0, pIdParto));

            // RF1.9 al reves: la vaca de un solo parto vuelve a ser novilla
            Categoria unaCategoria = this.CalcularCategoria(unaMadreNueva);
            if (unaCategoria != null)
            {
                unaMadreNueva.Categoria = unaCategoria;
            }

            if (Persistencia.EliminarParto(pIdParto, _listaCrias, unaLactanciaDelParto,
                unaMadreNueva, unaLactanciaReabierta))
            {
                mListaPartos.Remove(unParto);

                if (unaLactanciaDelParto != null)
                {
                    mListaLactancias.Remove(unaLactanciaDelParto);
                }

                if (unaLactanciaCerrada != null)
                {
                    unaLactanciaCerrada.FechaSecado = DateTime.MinValue;
                }

                foreach (Animal unaCria in _listaCrias)
                {
                    mListaAnimales.Remove(unaCria);

                    if (unaCria is Hembra)
                    {
                        mListaHembras.Remove((Hembra)unaCria);
                    }
                    else if (unaCria is Macho)
                    {
                        mListaMachos.Remove((Macho)unaCria);
                    }

                    // La foto es un archivo en el disco: si la fila se va, el archivo
                    // tambien, o wwwroot/fotos junta imagenes que ya no son de nadie.
                    if (unaCria.TieneFoto)
                    {
                        Persistencia.BorrarFoto(unaCria.Foto);
                    }
                }

                unaMadre.NumeroPartos = unaMadreNueva.NumeroPartos;
                unaMadre.EstadoProductivo = unaMadreNueva.EstadoProductivo;
                unaMadre.EstadoReproductivo = unaMadreNueva.EstadoReproductivo;
                unaMadre.Categoria = unaMadreNueva.Categoria;
                return true;
            }
            return false;

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

        // La correccion del diagnostico toca la fecha, la enfermedad y el animal. El
        // estado no: lo maneja el sistema -el alta del tratamiento lo pasa a "En
        // tratamiento" y el boton de resolver lo cierra-, y dejarlo escribir a mano
        // permitiria marcar como resuelto un diagnostico con tratamiento en curso.
        public string ValidarModificarDiagnostico(int pIdDiagnostico, DateTime pFechaDiagnostico,
            string pEnfermedad, int pIdAnimal)
        {
            this.Refrescar();

            if (this.BuscarDiagnostico(pIdDiagnostico) == null)
            {
                return "El diagnostico que se quiere corregir ya no existe!";
            }

            Animal unAnimal = this.BuscarAnimal(pIdAnimal);
            if (unAnimal == null)
            {
                return "Hay que indicar el animal diagnosticado!";
            }

            if (pEnfermedad == null || pEnfermedad == "")
            {
                return "Hay que indicar la enfermedad!";
            }

            if (pFechaDiagnostico > DateTime.Now)
            {
                return "La fecha del diagnostico no puede ser posterior a la fecha actual!";
            }

            if (!this.EstabaActivo(unAnimal, pFechaDiagnostico))
            {
                return "El animal figura dado de baja el " + unAnimal.FechaBaja.ToShortDateString()
                    + ": no se le puede registrar un diagnostico posterior a esa fecha!";
            }

            // El tratamiento se aplica sobre el animal diagnosticado: pasarle el
            // diagnostico a otro animal dejaria el tratamiento tratando a la vaca
            // equivocada, con su descarte de leche incluido.
            List<Tratamiento> _listaTratamientos = this.FiltrarTratamientosXDiagnostico(pIdDiagnostico);
            Diagnostico unDiagnostico = this.BuscarDiagnostico(pIdDiagnostico);

            if (_listaTratamientos.Count > 0 && unDiagnostico.Animal != null
                && unAnimal.IdAnimal != unDiagnostico.Animal.IdAnimal)
            {
                return "El diagnostico tiene " + _listaTratamientos.Count + " tratamiento(s) aplicado(s) "
                    + "sobre la caravana " + unDiagnostico.Animal.NumCaravana + ": para pasarlo a otro "
                    + "animal hay que eliminar antes esos tratamientos!";
            }
            return "";
        }

        public bool ModificarDiagnostico(int pIdDiagnostico, DateTime pFechaDiagnostico,
            string pEnfermedad, int pIdAnimal)
        {
            if (this.ValidarModificarDiagnostico(pIdDiagnostico, pFechaDiagnostico, pEnfermedad,
                pIdAnimal) != "")
            {
                return false;
            }

            Diagnostico unDiagnostico = this.BuscarDiagnostico(pIdDiagnostico);
            Animal unAnimal = this.BuscarAnimal(pIdAnimal);

            Diagnostico unDiagnosticoNuevo = new Diagnostico(pIdDiagnostico, pFechaDiagnostico,
                pEnfermedad, unDiagnostico.Estado, unAnimal);

            if (Persistencia.ModificarDiagnostico(unDiagnosticoNuevo))
            {
                unDiagnostico.FechaDiagnostico = pFechaDiagnostico;
                unDiagnostico.Enfermedad = pEnfermedad;
                unDiagnostico.Animal = unAnimal;
                return true;
            }
            return false;

        }

        // El diagnostico no se elimina mientras tenga tratamientos: un tratamiento sin
        // el diagnostico que lo origino queda sin explicacion, y ademas es el que fija
        // el descarte de leche.
        public string ValidarEliminarDiagnostico(int pIdDiagnostico)
        {
            this.Refrescar();

            if (this.BuscarDiagnostico(pIdDiagnostico) == null)
            {
                return "El diagnostico que se quiere eliminar ya no existe!";
            }

            List<Tratamiento> _listaTratamientos = this.FiltrarTratamientosXDiagnostico(pIdDiagnostico);
            if (_listaTratamientos.Count > 0)
            {
                return "El diagnostico tiene " + _listaTratamientos.Count + " tratamiento(s) aplicado(s): "
                    + "elimine primero esos tratamientos desde Sanidad, Tratamientos!";
            }
            return "";
        }

        public bool EliminarDiagnostico(int pIdDiagnostico)
        {
            if (this.ValidarEliminarDiagnostico(pIdDiagnostico) != "")
            {
                return false;
            }

            Diagnostico unDiagnostico = this.BuscarDiagnostico(pIdDiagnostico);

            if (Persistencia.EliminarDiagnostico(pIdDiagnostico))
            {
                mListaDiagnosticos.Remove(unDiagnostico);
                return true;
            }
            return false;

        }

        public List<Tratamiento> FiltrarTratamientosXDiagnostico(int pIdDiagnostico)
        {
            List<Tratamiento> _listaXDiagnostico = new List<Tratamiento>();

            foreach (Tratamiento unTratamiento in mListaTratamientos)
            {
                if (unTratamiento.Diagnostico != null
                    && unTratamiento.Diagnostico.IdDiagnostico == pIdDiagnostico)
                {
                    _listaXDiagnostico.Add(unTratamiento);
                }
            }
            return _listaXDiagnostico;
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

            // La cantidad viaja con el tratamiento y no solo en el movimiento de stock:
            // es lo que despues permite devolverle al inventario lo que nunca se uso si
            // el registro resulta estar mal.
            pTratamientoNuevo.CantidadInsumo = pCantidadInsumo;

            if (Persistencia.AltaTratamiento(pTratamientoNuevo))
            {
                mListaTratamientos.Add(pTratamientoNuevo);
                return true;
            }
            return false;

        }

        // Lo que impide corregir el tratamiento, o una cadena vacia si se puede. Es la
        // misma validacion del alta con una diferencia: el stock que se exige es
        // unicamente el que falta, porque lo que este tratamiento ya tenia descontado
        // se devuelve en la misma operacion.
        public string ValidarModificarTratamiento(int pIdTratamiento, DateTime pFechaInicio,
            int pDiasDuracion, string pDosisDiaria, double pCantidadInsumo, int pIdDiagnostico,
            int pIdAnimal, int pIdInsumo, int pIdPlan)
        {
            this.Refrescar();

            Tratamiento unTratamiento = this.BuscarTratamiento(pIdTratamiento);
            if (unTratamiento == null)
            {
                return "El tratamiento que se quiere corregir ya no existe!";
            }

            Insumo unInsumo = this.BuscarInsumo(pIdInsumo);
            if (unInsumo == null)
            {
                return "Hay que indicar el producto aplicado!";
            }

            // El animal sale del diagnostico cuando el tratamiento viene de uno, y se
            // elige por caravana cuando es preventivo.
            Diagnostico unDiagnostico = this.BuscarDiagnostico(pIdDiagnostico);
            Animal unAnimal = unDiagnostico != null ? unDiagnostico.Animal : this.BuscarAnimal(pIdAnimal);

            if (unAnimal == null)
            {
                return "Hay que indicar el diagnostico a tratar, o el animal si el tratamiento es preventivo!";
            }

            if (pDiasDuracion <= 0)
            {
                return "La duracion del tratamiento tiene que ser de al menos un dia!";
            }

            if (pDosisDiaria == null || pDosisDiaria == "")
            {
                return "La dosis diaria es obligatoria!";
            }

            if (pFechaInicio > DateTime.Now)
            {
                return "La fecha de inicio no puede ser posterior a la fecha actual!";
            }

            PlanSanitario unPlan = this.BuscarPlanSanitario(pIdPlan);
            if (unPlan != null && unPlan.TipoProcedimiento != PlanSanitario.DESPARASITACION)
            {
                return "Un tratamiento solo puede dar por cumplido un plan de desparasitacion!";
            }

            // Solo se pide el stock que falta: lo que este mismo tratamiento tenia
            // descontado vuelve al inventario en la misma transaccion.
            double vDevuelto = unTratamiento.Insumo != null
                && unTratamiento.Insumo.IdInsumo == unInsumo.IdInsumo
                ? unTratamiento.CantidadInsumo
                : 0;

            if (pCantidadInsumo - vDevuelto > unInsumo.StockActual)
            {
                return "No hay stock suficiente del producto: quedan "
                    + unInsumo.StockActual.ToString("N2") + " unidades!";
            }
            return "";
        }

        // La correccion recalcula el descarte de leche y mueve el stock. Las dos cosas
        // importan: el descarte es lo que deja al animal fuera del lote de ordenie, y
        // una duracion o un producto corregidos sin recalcularlo dejarian leche
        // descartada de mas o, peor, leche con residuos yendo al tanque.
        public bool ModificarTratamiento(int pIdTratamiento, DateTime pFechaInicio, int pDiasDuracion,
            string pDosisDiaria, double pCantidadInsumo, DateTime pFechaFinDescarte,
            int pIdDiagnostico, int pIdAnimal, int pIdInsumo, int pIdPlan)
        {
            if (this.ValidarModificarTratamiento(pIdTratamiento, pFechaInicio, pDiasDuracion,
                pDosisDiaria, pCantidadInsumo, pIdDiagnostico, pIdAnimal, pIdInsumo, pIdPlan) != "")
            {
                return false;
            }

            Tratamiento unTratamiento = this.BuscarTratamiento(pIdTratamiento);
            Insumo unInsumoAnterior = unTratamiento.Insumo;
            Diagnostico unDiagnosticoAnterior = unTratamiento.Diagnostico;
            double vCantidadAnterior = unTratamiento.CantidadInsumo;

            Insumo unInsumo = this.BuscarInsumo(pIdInsumo);
            Diagnostico unDiagnostico = this.BuscarDiagnostico(pIdDiagnostico);
            Animal unAnimal = unDiagnostico != null ? unDiagnostico.Animal : this.BuscarAnimal(pIdAnimal);
            PlanSanitario unPlan = this.BuscarPlanSanitario(pIdPlan);

            Tratamiento unTratamientoNuevo = new Tratamiento(pIdTratamiento, pFechaInicio,
                pDiasDuracion, pDosisDiaria, pCantidadInsumo, pFechaFinDescarte, unDiagnostico,
                unAnimal, unInsumo, unPlan);

            // Como en el alta, la fecha se propone cuando llega vacia y el usuario la
            // puede ajustar.
            if (unTratamientoNuevo.FechaFinDescarte == DateTime.MinValue)
            {
                unTratamientoNuevo.FechaFinDescarte = this.CalcularDescarte(unTratamientoNuevo);
            }

            List<MovimientoStock> _listaMovimientos = this.MovimientosPorCambioDeInsumo(
                unInsumoAnterior, unInsumo, vCantidadAnterior, pCantidadInsumo, pFechaInicio,
                "Ajuste por correccion del tratamiento del " + unTratamiento.FechaInicio.ToShortDateString(),
                "Tratamiento sanitario");

            List<Diagnostico> _listaDiagnosticos = this.DiagnosticosPorCambioDeTratamiento(
                pIdTratamiento, unDiagnosticoAnterior, unDiagnostico);

            if (Persistencia.ModificarTratamiento(unTratamientoNuevo, _listaMovimientos,
                _listaDiagnosticos))
            {
                unTratamiento.FechaInicio = pFechaInicio;
                unTratamiento.DiasDuracion = pDiasDuracion;
                unTratamiento.DosisDiaria = pDosisDiaria;
                unTratamiento.CantidadInsumo = pCantidadInsumo;
                unTratamiento.FechaFinDescarte = unTratamientoNuevo.FechaFinDescarte;
                unTratamiento.Diagnostico = unDiagnostico;
                unTratamiento.Animal = unAnimal;
                unTratamiento.Insumo = unInsumo;
                unTratamiento.Plan = unPlan;

                this.AplicarMovimientosEnCache(_listaMovimientos);
                this.AplicarEstadosDeDiagnosticoEnCache(_listaDiagnosticos);
                return true;
            }
            return false;

        }

        // Del tratamiento no cuelga ningun otro registro, asi que nunca se bloquea. El
        // descarte de leche desaparece solo: no es un dato guardado en el animal, es lo
        // que dicen sus tratamientos, y sin el tratamiento no dicen nada.
        public string ValidarEliminarTratamiento(int pIdTratamiento)
        {
            this.Refrescar();

            if (this.BuscarTratamiento(pIdTratamiento) == null)
            {
                return "El tratamiento que se quiere eliminar ya no existe!";
            }
            return "";
        }

        public bool EliminarTratamiento(int pIdTratamiento)
        {
            if (this.ValidarEliminarTratamiento(pIdTratamiento) != "")
            {
                return false;
            }

            Tratamiento unTratamiento = this.BuscarTratamiento(pIdTratamiento);

            List<MovimientoStock> _listaMovimientos = this.ArmarMovimientos(
                this.Devolucion(unTratamiento.Insumo, unTratamiento.CantidadInsumo,
                    "Devolucion por eliminacion del tratamiento del "
                    + unTratamiento.FechaInicio.ToShortDateString()),
                null);

            List<Diagnostico> _listaDiagnosticos = this.DiagnosticosPorCambioDeTratamiento(
                pIdTratamiento, unTratamiento.Diagnostico, null);

            if (Persistencia.EliminarTratamiento(pIdTratamiento, _listaMovimientos, _listaDiagnosticos))
            {
                mListaTratamientos.Remove(unTratamiento);
                this.AplicarMovimientosEnCache(_listaMovimientos);
                this.AplicarEstadosDeDiagnosticoEnCache(_listaDiagnosticos);
                return true;
            }
            return false;

        }

        // Los diagnosticos que cambian de estado por una correccion de tratamiento. El
        // que se deja atras vuelve a quedar activo si no le queda ningun otro
        // tratamiento -si no, seguiria figurando "En tratamiento" sin tratamiento
        // alguno- y el nuevo pasa a estar en tratamiento, como en el alta.
        private List<Diagnostico> DiagnosticosPorCambioDeTratamiento(int pIdTratamiento,
            Diagnostico pAnterior, Diagnostico pNuevo)
        {
            List<Diagnostico> _listaDiagnosticos = new List<Diagnostico>();

            bool vMismoDiagnostico = pAnterior != null && pNuevo != null
                && pAnterior.IdDiagnostico == pNuevo.IdDiagnostico;

            if (vMismoDiagnostico)
            {
                return _listaDiagnosticos;
            }

            if (pAnterior != null && pAnterior.Estado == Diagnostico.EN_TRATAMIENTO
                && !this.TieneOtroTratamiento(pAnterior.IdDiagnostico, pIdTratamiento))
            {
                _listaDiagnosticos.Add(new Diagnostico(pAnterior.IdDiagnostico,
                    pAnterior.FechaDiagnostico, pAnterior.Enfermedad, Diagnostico.ACTIVO,
                    pAnterior.Animal));
            }

            if (pNuevo != null && pNuevo.Estado != Diagnostico.EN_TRATAMIENTO)
            {
                _listaDiagnosticos.Add(new Diagnostico(pNuevo.IdDiagnostico, pNuevo.FechaDiagnostico,
                    pNuevo.Enfermedad, Diagnostico.EN_TRATAMIENTO, pNuevo.Animal));
            }
            return _listaDiagnosticos;
        }

        private bool TieneOtroTratamiento(int pIdDiagnostico, int pIdTratamientoExcluido)
        {
            foreach (Tratamiento unTratamiento in this.FiltrarTratamientosXDiagnostico(pIdDiagnostico))
            {
                if (unTratamiento.IdTratamiento != pIdTratamientoExcluido)
                {
                    return true;
                }
            }
            return false;
        }

        private void AplicarEstadosDeDiagnosticoEnCache(List<Diagnostico> pDiagnosticosActualizados)
        {
            foreach (Diagnostico unDiagnosticoNuevo in pDiagnosticosActualizados)
            {
                Diagnostico unDiagnostico = this.BuscarDiagnostico(unDiagnosticoNuevo.IdDiagnostico);
                if (unDiagnostico != null)
                {
                    unDiagnostico.Estado = unDiagnosticoNuevo.Estado;
                }
            }
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

        // Corregir la vacunacion puede cambiar la vacuna aplicada. Como en el
        // tratamiento, el stock que se exige es solo el que falta: la dosis que esta
        // misma aplicacion tenia descontada vuelve al inventario en la operacion.
        public string ValidarModificarVacunacion(int pIdVacunacion, DateTime pFechaAplicacion,
            int pIdAnimal, int pIdInsumo, int pIdPlan)
        {
            this.Refrescar();

            Vacunacion unaVacunacion = this.BuscarVacunacion(pIdVacunacion);
            if (unaVacunacion == null)
            {
                return "La vacunacion que se quiere corregir ya no existe!";
            }

            Animal unAnimal = this.BuscarAnimal(pIdAnimal);
            if (unAnimal == null)
            {
                return "Hay que indicar el animal vacunado!";
            }

            Insumo unInsumo = this.BuscarInsumo(pIdInsumo);
            if (unInsumo == null)
            {
                return "Hay que indicar la vacuna aplicada!";
            }

            if (pFechaAplicacion > DateTime.Now)
            {
                return "La fecha de aplicacion no puede ser posterior a la fecha actual!";
            }

            if (!this.EstabaActivo(unAnimal, pFechaAplicacion))
            {
                return "El animal figura dado de baja el " + unAnimal.FechaBaja.ToShortDateString()
                    + ": no se le puede registrar una vacunacion posterior a esa fecha!";
            }

            PlanSanitario unPlan = this.BuscarPlanSanitario(pIdPlan);
            if (unPlan != null && unPlan.TipoProcedimiento != PlanSanitario.VACUNACION)
            {
                return "Una vacunacion solo puede dar por cumplido un plan de vacunacion!";
            }

            bool vMismaVacuna = unaVacunacion.Insumo != null
                && unaVacunacion.Insumo.IdInsumo == unInsumo.IdInsumo;

            if (!vMismaVacuna && unInsumo.StockActual < UNIDADES_POR_VACUNACION)
            {
                return "No hay stock disponible de la vacuna seleccionada!";
            }
            return "";
        }

        public bool ModificarVacunacion(int pIdVacunacion, DateTime pFechaAplicacion, int pIdAnimal,
            int pIdInsumo, int pIdPlan)
        {
            if (this.ValidarModificarVacunacion(pIdVacunacion, pFechaAplicacion, pIdAnimal,
                pIdInsumo, pIdPlan) != "")
            {
                return false;
            }

            Vacunacion unaVacunacion = this.BuscarVacunacion(pIdVacunacion);
            Insumo unInsumoAnterior = unaVacunacion.Insumo;

            Animal unAnimal = this.BuscarAnimal(pIdAnimal);
            Insumo unInsumo = this.BuscarInsumo(pIdInsumo);
            PlanSanitario unPlan = this.BuscarPlanSanitario(pIdPlan);

            Vacunacion unaVacunacionNueva = new Vacunacion(pIdVacunacion, pFechaAplicacion,
                unAnimal, unInsumo, unPlan);

            List<MovimientoStock> _listaMovimientos = this.MovimientosPorCambioDeInsumo(
                unInsumoAnterior, unInsumo, UNIDADES_POR_VACUNACION, UNIDADES_POR_VACUNACION,
                pFechaAplicacion,
                "Ajuste por correccion de la vacunacion del "
                + unaVacunacion.FechaAplicacion.ToShortDateString(),
                "Vacunacion");

            if (Persistencia.ModificarVacunacion(unaVacunacionNueva, _listaMovimientos))
            {
                unaVacunacion.FechaAplicacion = pFechaAplicacion;
                unaVacunacion.Animal = unAnimal;
                unaVacunacion.Insumo = unInsumo;
                unaVacunacion.Plan = unPlan;

                this.AplicarMovimientosEnCache(_listaMovimientos);
                return true;
            }
            return false;

        }

        // De la vacunacion no cuelga nada: se elimina siempre y el animal vuelve a
        // figurar pendiente en el plan que esta aplicacion daba por cumplido.
        public string ValidarEliminarVacunacion(int pIdVacunacion)
        {
            this.Refrescar();

            if (this.BuscarVacunacion(pIdVacunacion) == null)
            {
                return "La vacunacion que se quiere eliminar ya no existe!";
            }
            return "";
        }

        public bool EliminarVacunacion(int pIdVacunacion)
        {
            if (this.ValidarEliminarVacunacion(pIdVacunacion) != "")
            {
                return false;
            }

            Vacunacion unaVacunacion = this.BuscarVacunacion(pIdVacunacion);

            List<MovimientoStock> _listaMovimientos = this.ArmarMovimientos(
                this.Devolucion(unaVacunacion.Insumo, UNIDADES_POR_VACUNACION,
                    "Devolucion por eliminacion de la vacunacion del "
                    + unaVacunacion.FechaAplicacion.ToShortDateString()),
                null);

            if (Persistencia.EliminarVacunacion(pIdVacunacion, _listaMovimientos))
            {
                mListaVacunaciones.Remove(unaVacunacion);
                this.AplicarMovimientosEnCache(_listaMovimientos);
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

        // Corregir el descorne no puede dejar dos descornes sobre el mismo animal: el
        // procedimiento es de aplicacion unica y esa es la regla que hace que el plan
        // deje de exigirlo.
        public string ValidarModificarDescorne(int pIdDescorne, DateTime pFecha, string pMetodo,
            int pIdAnimal, int pIdPlan)
        {
            this.Refrescar();

            if (this.BuscarDescorne(pIdDescorne) == null)
            {
                return "El descorne que se quiere corregir ya no existe!";
            }

            Animal unAnimal = this.BuscarAnimal(pIdAnimal);
            if (unAnimal == null)
            {
                return "Hay que indicar el animal descornado!";
            }

            if (pMetodo == null || pMetodo == "")
            {
                return "Hay que indicar el metodo de descorne!";
            }

            if (pFecha > DateTime.Now)
            {
                return "La fecha del descorne no puede ser posterior a la fecha actual!";
            }

            if (!this.EstabaActivo(unAnimal, pFecha))
            {
                return "El animal figura dado de baja el " + unAnimal.FechaBaja.ToShortDateString()
                    + ": no se le puede registrar un descorne posterior a esa fecha!";
            }

            PlanSanitario unPlan = this.BuscarPlanSanitario(pIdPlan);
            if (unPlan != null && unPlan.TipoProcedimiento != PlanSanitario.DESCORNE)
            {
                return "Un descorne solo puede dar por cumplido un plan de descorne!";
            }

            if (this.TieneDescorne(unAnimal, pIdDescorne))
            {
                return "La caravana " + unAnimal.NumCaravana + " ya tiene otro descorne registrado!";
            }
            return "";
        }

        public bool ModificarDescorne(int pIdDescorne, DateTime pFecha, string pMetodo,
            string pObservaciones, int pIdAnimal, int pIdPlan)
        {
            if (this.ValidarModificarDescorne(pIdDescorne, pFecha, pMetodo, pIdAnimal, pIdPlan) != "")
            {
                return false;
            }

            Descorne unDescorne = this.BuscarDescorne(pIdDescorne);
            Animal unAnimal = this.BuscarAnimal(pIdAnimal);
            PlanSanitario unPlan = this.BuscarPlanSanitario(pIdPlan);

            Descorne unDescorneNuevo = new Descorne(pIdDescorne, pFecha, pMetodo, pObservaciones,
                unAnimal, unPlan);

            if (Persistencia.ModificarDescorne(unDescorneNuevo))
            {
                unDescorne.Fecha = pFecha;
                unDescorne.Metodo = pMetodo;
                unDescorne.Observaciones = pObservaciones;
                unDescorne.Animal = unAnimal;
                unDescorne.Plan = unPlan;
                return true;
            }
            return false;

        }

        // El descorne no consume insumo y nada cuelga de el: se elimina siempre. Lo
        // unico que cambia es que el animal vuelve a figurar pendiente en el plan de
        // descorne, que es lo correcto si el registro estaba mal.
        public string ValidarEliminarDescorne(int pIdDescorne)
        {
            this.Refrescar();

            if (this.BuscarDescorne(pIdDescorne) == null)
            {
                return "El descorne que se quiere eliminar ya no existe!";
            }
            return "";
        }

        public bool EliminarDescorne(int pIdDescorne)
        {
            if (this.ValidarEliminarDescorne(pIdDescorne) != "")
            {
                return false;
            }

            Descorne unDescorne = this.BuscarDescorne(pIdDescorne);

            if (Persistencia.EliminarDescorne(pIdDescorne))
            {
                mListaDescornes.Remove(unDescorne);
                return true;
            }
            return false;

        }

        public bool TieneDescorne(Animal pAnimal)
        {
            return this.TieneDescorne(pAnimal, 0);
        }

        // El mismo control salteando el descorne que se esta corrigiendo: si no, un
        // descorne se bloquearia a si mismo apenas se le quiere arreglar la fecha.
        private bool TieneDescorne(Animal pAnimal, int pIdDescorneIgnorado)
        {
            if (pAnimal == null)
            {
                return false;
            }

            foreach (Descorne unDescorne in this.FiltrarDescornesXAnimal(pAnimal.IdAnimal))
            {
                if (unDescorne.IdDescorne != pIdDescorneIgnorado)
                {
                    return true;
                }
            }
            return false;

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

        #region INDICADORES
        // Los numeros con los que se sabe si el tambo va bien. Ninguno se almacena: se
        // calculan sobre lo registrado, que es lo que corresponde a un dato derivado.

        // Los controles de una lactancia agrupados por jornada y ordenados por fecha.
        // Una vaca puede tener un control por turno el mismo dia; lo que interesa es lo
        // que dio ese dia.
        public List<ControlDiario> ListarControlesDiarios(Lactancia pLactancia)
        {
            List<ControlDiario> _listaControles = new List<ControlDiario>();

            if (pLactancia == null)
            {
                return _listaControles;
            }

            foreach (OrdenieIndividual unOrdenie in this.FiltrarOrdeniesXLactancia(pLactancia.IdLactancia))
            {
                ControlDiario unControl = this.BuscarControlDiario(_listaControles, unOrdenie.Fecha);

                if (unControl == null)
                {
                    unControl = new ControlDiario(unOrdenie.Fecha.Date, 0);
                    this.InsertarControlOrdenado(_listaControles, unControl);
                }
                unControl.Litros = unControl.Litros + unOrdenie.Litros;
            }
            return _listaControles;
        }

        // Produccion estimada de la lactancia por el metodo de intervalos de control.
        //
        // Sumar los controles daria la leche de diez jornadas sueltas y no la de la
        // lactancia: el control lechero se hace una vez por mes, asi que cada medicion
        // representa a los dias que la rodean. Se multiplica el promedio de dos
        // controles consecutivos por los dias que hay entre ellos, se agrega el tramo
        // del parto al primer control y el del ultimo control al secado -o a hoy, si la
        // vaca sigue en ordenie-.
        //
        // Es el mismo criterio con el que trabaja cualquier control lechero oficial.
        public double EstimarProduccionLactancia(Lactancia pLactancia)
        {
            List<ControlDiario> _listaControles = this.ListarControlesDiarios(pLactancia);

            if (_listaControles.Count == 0)
            {
                return 0;
            }

            double vTotal = 0;

            // Del parto al primer control, con el valor de ese primer control
            double vDiasIniciales = (_listaControles[0].Fecha.Date - pLactancia.FechaInicio.Date).TotalDays;
            if (vDiasIniciales > 0)
            {
                vTotal = vTotal + _listaControles[0].Litros * vDiasIniciales;
            }

            for (int vIndice = 0; vIndice < _listaControles.Count - 1; vIndice = vIndice + 1)
            {
                double vDias = (_listaControles[vIndice + 1].Fecha.Date - _listaControles[vIndice].Fecha.Date).TotalDays;
                double vPromedio = (_listaControles[vIndice].Litros + _listaControles[vIndice + 1].Litros) / 2;

                vTotal = vTotal + vPromedio * vDias;
            }

            // Del ultimo control al cierre de la lactancia
            ControlDiario unUltimo = _listaControles[_listaControles.Count - 1];
            DateTime vFin = pLactancia.FechaSecado != DateTime.MinValue ? pLactancia.FechaSecado : DateTime.Now;
            double vDiasFinales = (vFin.Date - unUltimo.Fecha.Date).TotalDays;

            if (vDiasFinales > 0)
            {
                vTotal = vTotal + unUltimo.Litros * vDiasFinales;
            }

            return vTotal;
        }

        // Dias en leche: los que la vaca lleva en esta lactancia. Si ya se seco, los que
        // duro.
        public int DiasEnLeche(Lactancia pLactancia)
        {
            if (pLactancia == null)
            {
                return 0;
            }

            DateTime vFin = pLactancia.FechaSecado != DateTime.MinValue ? pLactancia.FechaSecado : DateTime.Now;
            int vDias = (int)(vFin.Date - pLactancia.FechaInicio.Date).TotalDays;

            return vDias < 0 ? 0 : vDias;
        }

        public double PromedioDiarioLactancia(Lactancia pLactancia)
        {
            int vDias = this.DiasEnLeche(pLactancia);

            if (vDias <= 0)
            {
                return 0;
            }
            return this.EstimarProduccionLactancia(pLactancia) / vDias;
        }

        // Proyeccion a 305 dias: lo que la vaca daria si sostuviera hasta el final de la
        // lactancia estandar el ultimo nivel de produccion controlado. Es una proyeccion
        // lineal y por lo tanto optimista -la curva de lactancia baja sobre el final-,
        // pero alcanza para comparar vacas entre si, que es para lo que se usa.
        public double ProyectarProduccion305(Lactancia pLactancia)
        {
            double vEstimada = this.EstimarProduccionLactancia(pLactancia);
            int vDias = this.DiasEnLeche(pLactancia);

            if (vDias <= 0 || vDias >= DIAS_LACTANCIA_ESTANDAR)
            {
                return vEstimada;
            }

            List<ControlDiario> _listaControles = this.ListarControlesDiarios(pLactancia);
            if (_listaControles.Count == 0)
            {
                return vEstimada;
            }

            double vUltimo = _listaControles[_listaControles.Count - 1].Litros;
            return vEstimada + vUltimo * (DIAS_LACTANCIA_ESTANDAR - vDias);
        }

        public Parto UltimoParto(Hembra pHembra)
        {
            Parto unUltimo = null;

            if (pHembra == null)
            {
                return null;
            }

            foreach (Parto unParto in this.FiltrarPartosXHembra(pHembra.IdAnimal))
            {
                if (unUltimo == null || unParto.FechaParto > unUltimo.FechaParto)
                {
                    unUltimo = unParto;
                }
            }
            return unUltimo;
        }

        // Dias abiertos: los que pasan entre el parto y la concepcion. Es el indicador
        // central de la eficiencia reproductiva, porque cada dia abierto de mas es un
        // dia que la vaca pasa dando menos leche y sin preparar la lactancia siguiente.
        //
        // Mientras la vaca no queda prenada el numero sigue corriendo, y eso tambien es
        // informacion: una vaca con muchos dias abiertos sin preniez es la que hay que
        // mirar.
        public int DiasAbiertos(Hembra pHembra)
        {
            Parto unParto = this.UltimoParto(pHembra);

            if (unParto == null)
            {
                return 0;
            }

            DateTime vHasta = DateTime.Now;
            Servicio unServicio = this.ServicioVigente(pHembra);

            if (pHembra.EstadoReproductivo == Hembra.PRENADA && unServicio != null)
            {
                vHasta = unServicio.FechaServicio;
            }

            int vDias = (int)(vHasta.Date - unParto.FechaParto.Date).TotalDays;
            return vDias < 0 ? 0 : vDias;
        }

        public bool TieneDiasAbiertosCerrados(Hembra pHembra)
        {
            if (pHembra != null && pHembra.EstadoReproductivo == Hembra.PRENADA)
            {
                return true;
            }
            return false;

        }

        // Intervalo entre partos: lo que tarda la vaca en volver a parir. El objetivo de
        // un tambo esta alrededor de los trece meses; si se estira, la vaca produce
        // menos leche por anio de vida.
        public int IntervaloEntrePartos(Hembra pHembra)
        {
            Parto unUltimo = this.UltimoParto(pHembra);

            if (unUltimo == null)
            {
                return 0;
            }

            Parto unAnterior = null;
            foreach (Parto unParto in this.FiltrarPartosXHembra(pHembra.IdAnimal))
            {
                if (unParto.FechaParto < unUltimo.FechaParto
                    && (unAnterior == null || unParto.FechaParto > unAnterior.FechaParto))
                {
                    unAnterior = unParto;
                }
            }

            if (unAnterior == null)
            {
                return 0;
            }
            return (int)(unUltimo.FechaParto.Date - unAnterior.FechaParto.Date).TotalDays;
        }

        // Servicios que lleva la vaca desde su ultimo parto. Con tres o mas sin preniez,
        // la vaca ya es cara.
        public int ServiciosDesdeUltimoParto(Hembra pHembra)
        {
            Parto unParto = this.UltimoParto(pHembra);
            DateTime vDesde = unParto != null ? unParto.FechaParto : DateTime.MinValue;
            int vCantidad = 0;

            foreach (Servicio unServicio in this.FiltrarServiciosXHembra(pHembra.IdAnimal))
            {
                if (unServicio.FechaServicio > vDesde)
                {
                    vCantidad = vCantidad + 1;
                }
            }
            return vCantidad;
        }

        // Servicios por preniez del rodeo: cuantos servicios hicieron falta, en
        // promedio, por cada preniez confirmada. Es lo que mide el gasto en pajuelas y
        // el acierto en la deteccion de celo.
        public double ServiciosPorPrenez()
        {
            int vServicios = 0;
            int vPrenieces = 0;

            foreach (Servicio unServicio in this.ListarServicios())
            {
                vServicios = vServicios + 1;

                Tacto unTacto = this.UltimoTacto(unServicio);
                if (unTacto != null && this.EsPositivo(unTacto))
                {
                    vPrenieces = vPrenieces + 1;
                }
            }

            if (vPrenieces == 0)
            {
                return 0;
            }
            return (double)vServicios / vPrenieces;
        }

        public double PromedioDiasAbiertos()
        {
            int vTotal = 0;
            int vCantidad = 0;

            foreach (Hembra unaHembra in this.ListarHembras())
            {
                if (!unaHembra.Activo || this.UltimoParto(unaHembra) == null)
                {
                    continue;
                }

                vTotal = vTotal + this.DiasAbiertos(unaHembra);
                vCantidad = vCantidad + 1;
            }

            if (vCantidad == 0)
            {
                return 0;
            }
            return (double)vTotal / vCantidad;
        }

        public double PromedioIntervaloEntrePartos()
        {
            int vTotal = 0;
            int vCantidad = 0;

            foreach (Hembra unaHembra in this.ListarHembras())
            {
                int vIntervalo = this.IntervaloEntrePartos(unaHembra);

                if (vIntervalo > 0)
                {
                    vTotal = vTotal + vIntervalo;
                    vCantidad = vCantidad + 1;
                }
            }

            if (vCantidad == 0)
            {
                return 0;
            }
            return (double)vTotal / vCantidad;
        }

        // Promedio de litros por vaca y por dia del rodeo en ordenie. Es el numero que
        // se mira todos los dias.
        public double PromedioDiarioRodeo()
        {
            double vTotal = 0;
            int vCantidad = 0;

            foreach (Lactancia unaLactancia in this.ListarLactanciasActivas())
            {
                double vPromedio = this.PromedioDiarioLactancia(unaLactancia);

                if (vPromedio > 0)
                {
                    vTotal = vTotal + vPromedio;
                    vCantidad = vCantidad + 1;
                }
            }

            if (vCantidad == 0)
            {
                return 0;
            }
            return vTotal / vCantidad;
        }

        public double PromedioDiasEnLeche()
        {
            int vTotal = 0;
            int vCantidad = 0;

            foreach (Lactancia unaLactancia in this.ListarLactanciasActivas())
            {
                vTotal = vTotal + this.DiasEnLeche(unaLactancia);
                vCantidad = vCantidad + 1;
            }

            if (vCantidad == 0)
            {
                return 0;
            }
            return (double)vTotal / vCantidad;
        }

        public int ContarHembrasXEstadoProductivo(string pEstadoProductivo)
        {
            int vCantidad = 0;

            foreach (Hembra unaHembra in this.ListarHembras())
            {
                if (unaHembra.Activo && unaHembra.EstadoProductivo == pEstadoProductivo)
                {
                    vCantidad = vCantidad + 1;
                }
            }
            return vCantidad;
        }

        public int ContarHembrasXEstadoReproductivo(string pEstadoReproductivo)
        {
            int vCantidad = 0;

            foreach (Hembra unaHembra in this.ListarHembras())
            {
                if (unaHembra.Activo && unaHembra.EstadoReproductivo == pEstadoReproductivo)
                {
                    vCantidad = vCantidad + 1;
                }
            }
            return vCantidad;
        }

        private ControlDiario BuscarControlDiario(List<ControlDiario> pLista, DateTime pFecha)
        {
            foreach (ControlDiario unControl in pLista)
            {
                if (unControl.Fecha.Date == pFecha.Date)
                {
                    return unControl;
                }
            }
            return null;
        }

        // Los controles se guardan ordenados por fecha ascendente: el calculo por
        // intervalos recorre la lactancia de principio a fin.
        private void InsertarControlOrdenado(List<ControlDiario> pLista, ControlDiario pControl)
        {
            int vPosicion = 0;

            while (vPosicion < pLista.Count && pLista[vPosicion].Fecha.Date <= pControl.Fecha.Date)
            {
                vPosicion = vPosicion + 1;
            }
            pLista.Insert(vPosicion, pControl);
        }
        #endregion

        #region LISTAS DE TRABAJO
        // Las tres listas que arman el trabajo de la semana. Son filtros sobre datos que
        // ya estan registrados: lo que aportan es no tener que acordarse animal por
        // animal.

        // Servicios que ya estan en condiciones de tactarse y todavia no tienen tacto.
        // Es la lista con la que se arma la visita del veterinario.
        public List<Servicio> ListarTactosPendientes()
        {
            List<Servicio> _listaPendientes = new List<Servicio>();
            int vDiasParaTacto = this.Parametros().DiasParaTacto;

            foreach (Hembra unaHembra in this.ListarHembras())
            {
                // Solo las servidas: la prenada ya se tacto y la vacia no tiene servicio
                // pendiente de confirmar.
                if (!unaHembra.Activo || unaHembra.EstadoReproductivo != Hembra.SERVIDA)
                {
                    continue;
                }

                Servicio unServicio = this.ServicioVigente(unaHembra);
                if (unServicio == null || this.UltimoTacto(unServicio) != null)
                {
                    continue;
                }

                if (DateTime.Now.Date >= unServicio.FechaServicio.Date.AddDays(vDiasParaTacto))
                {
                    _listaPendientes.Add(unServicio);
                }
            }
            return _listaPendientes;
        }

        // Vacas en condiciones de recibir servicio: las que ya pasaron el periodo de
        // espera voluntario despues de parir, y las vaquillonas que alcanzaron la edad
        // minima y nunca fueron servidas.
        public List<Hembra> ListarVacasParaServir()
        {
            List<Hembra> _listaParaServir = new List<Hembra>();

            foreach (Hembra unaHembra in this.ListarHembras())
            {
                if (this.EstaParaServir(unaHembra))
                {
                    _listaParaServir.Add(unaHembra);
                }
            }
            return _listaParaServir;
        }

        public bool EstaParaServir(Hembra pHembra)
        {
            // Vacia quiere decir sin servicio vigente ni preniez confirmada
            if (pHembra == null || !pHembra.Activo || pHembra.EstadoReproductivo != Hembra.VACIA)
            {
                return false;
            }

            Parto unParto = this.UltimoParto(pHembra);

            // Nunca pario: es una vaquillona, y entra cuando alcanza la edad minima
            if (unParto == null)
            {
                return this.CalcularEdadMeses(pHembra) >= this.Parametros().EdadMinimaServicioMeses;
            }

            return DateTime.Now.Date >= unParto.FechaParto.Date.AddDays(this.Parametros().DiasEsperaVoluntaria);
        }

        // Por que la vaca aparece en la lista, para que la pantalla lo muestre sin
        // rehacer la cuenta.
        public string MotivoParaServir(Hembra pHembra)
        {
            Parto unParto = this.UltimoParto(pHembra);

            if (unParto == null)
            {
                return "Vaquillona de " + this.CalcularEdadMeses(pHembra) + " meses, sin servicios registrados";
            }

            int vDias = (int)(DateTime.Now.Date - unParto.FechaParto.Date).TotalDays;
            return "Pario hace " + vDias + " dias";
        }

        // Vacas que conviene revisar para descarte. El sistema no decide: junta lo que
        // ya sabe de cada una y muestra los motivos. La lista sale ordenada por cantidad
        // de motivos, que es una forma simple de poner primero los casos mas claros.
        public List<CandidataDescarte> ListarCandidatasDescarte()
        {
            List<CandidataDescarte> _listaCandidatas = new List<CandidataDescarte>();
            double vPromedioRodeo = this.PromedioDiarioRodeo();

            foreach (Hembra unaHembra in this.ListarHembras())
            {
                if (!unaHembra.Activo)
                {
                    continue;
                }

                List<string> _listaMotivos = this.MotivosDeDescarte(unaHembra, vPromedioRodeo);

                if (_listaMotivos.Count > 0)
                {
                    this.InsertarCandidataOrdenada(_listaCandidatas,
                        new CandidataDescarte(unaHembra, _listaMotivos));
                }
            }
            return _listaCandidatas;
        }

        private List<string> MotivosDeDescarte(Hembra pHembra, double pPromedioRodeo)
        {
            List<string> _listaMotivos = new List<string>();

            // Produccion muy por debajo del rodeo. Se compara contra la lactancia en
            // curso, y solo si hay controles: sin control lechero no hay con que juzgar.
            Lactancia unaLactancia = this.LactanciaActual(pHembra);
            if (unaLactancia != null && pPromedioRodeo > 0)
            {
                double vPromedio = this.PromedioDiarioLactancia(unaLactancia);

                if (vPromedio > 0 && vPromedio < pPromedioRodeo * PORCENTAJE_PRODUCCION_BAJA)
                {
                    _listaMotivos.Add("Produce " + vPromedio.ToString("N1") + " litros por dia, contra "
                        + pPromedioRodeo.ToString("N1") + " del rodeo.");
                }
            }

            int vServicios = this.ServiciosDesdeUltimoParto(pHembra);
            if (vServicios >= SERVICIOS_SIN_PRENIEZ_DESCARTE && pHembra.EstadoReproductivo != Hembra.PRENADA)
            {
                _listaMotivos.Add(vServicios + " servicios desde el ultimo parto sin preniez confirmada.");
            }

            int vDiasAbiertos = this.DiasAbiertos(pHembra);
            if (vDiasAbiertos > DIAS_ABIERTOS_EXCESIVOS && !this.TieneDiasAbiertosCerrados(pHembra))
            {
                _listaMotivos.Add(vDiasAbiertos + " dias abiertos.");
            }

            int vDiagnosticos = this.ContarDiagnosticosDelAnio(pHembra);
            if (vDiagnosticos >= DIAGNOSTICOS_REPETIDOS_DESCARTE)
            {
                _listaMotivos.Add(vDiagnosticos + " diagnosticos sanitarios en el ultimo anio.");
            }

            if (pHembra.NumeroPartos >= PARTOS_PARA_DESCARTE)
            {
                _listaMotivos.Add(pHembra.NumeroPartos + " partos: es una vaca de edad avanzada.");
            }

            return _listaMotivos;
        }

        private int ContarDiagnosticosDelAnio(Hembra pHembra)
        {
            int vCantidad = 0;
            DateTime vDesde = DateTime.Now.Date.AddYears(-1);

            foreach (Diagnostico unDiagnostico in this.FiltrarDiagnosticosXAnimal(pHembra.IdAnimal))
            {
                if (unDiagnostico.FechaDiagnostico.Date >= vDesde)
                {
                    vCantidad = vCantidad + 1;
                }
            }
            return vCantidad;
        }

        private void InsertarCandidataOrdenada(List<CandidataDescarte> pLista, CandidataDescarte pCandidata)
        {
            int vPosicion = 0;

            while (vPosicion < pLista.Count && pLista[vPosicion].Motivos.Count >= pCandidata.Motivos.Count)
            {
                vPosicion = vPosicion + 1;
            }
            pLista.Insert(vPosicion, pCandidata);
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
