-- =====================================================================
-- Sistema de Gestion de Tambo
-- Script de creacion de la base de datos - Modulos 2 y 3
--
--   Modulo 2: Control de Produccion   (lactancias, ordenies por lote,
--             ordenies individuales)
--   Modulo 3: Gestion Reproductiva    (celos, servicios, tactos, partos)
--
-- Se adelantan ademas cuatro tablas que los casos de uso de estos dos
-- modulos necesitan para funcionar y que pertenecen a modulos posteriores:
--
--   insumos, movimientos_stock  (Modulo 5) -- CU15 registra la inseminacion
--       artificial con una pajuela del stock y le descuenta una unidad.
--   diagnosticos, tratamientos  (Modulo 4) -- CU8 excluye del lote a los
--       animales con periodo de descarte de leche vigente.
--
-- Motor: MySQL
-- Requiere que tambo_m0_m1.sql ya se haya ejecutado.
-- Las tablas restantes de los modulos 4 a 6 se agregan a medida que se
-- desarrollan.
-- =====================================================================

USE tambo;

-- =====================================================================
-- Tablas adelantadas del Modulo 5: insumos y stock
-- =====================================================================

-- ---------------------------------------------------------------------
-- insumos
-- La pajuela es un insumo mas. id_macho la vincula con el toro que la
-- aporta, que puede ser un toro de catalogo (en_pie = 0) y no integrar
-- el rodeo: eso es lo que permite reconstruir la genealogia de la cria.
-- ---------------------------------------------------------------------
CREATE TABLE insumos (
    id_insumo             INT(11)       NOT NULL AUTO_INCREMENT,
    nombre                VARCHAR(100)  NOT NULL,
    tipo_insumo           VARCHAR(30)   NOT NULL,
    stock_actual          DECIMAL(10,2) NOT NULL DEFAULT 0,
    stock_minimo          DECIMAL(10,2) NOT NULL DEFAULT 0,
    periodo_descarte_dias INT(11)       NULL,
    id_macho              INT(11)       NULL,
    PRIMARY KEY (id_insumo),
    FOREIGN KEY (id_macho) REFERENCES machos (id_animal)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ---------------------------------------------------------------------
-- movimientos_stock
-- El vencimiento se registra aca y no en insumos porque un mismo insumo
-- ingresa en partidas distintas con vencimientos distintos.
-- ---------------------------------------------------------------------
CREATE TABLE movimientos_stock (
    id_movimiento     INT(11)       NOT NULL AUTO_INCREMENT,
    tipo_movimiento   VARCHAR(20)   NOT NULL,
    cantidad          DECIMAL(10,2) NOT NULL,
    fecha             DATE          NOT NULL,
    fecha_vencimiento DATE          NULL,
    motivo            VARCHAR(100)  NULL,
    id_insumo         INT(11)       NOT NULL,
    PRIMARY KEY (id_movimiento),
    FOREIGN KEY (id_insumo) REFERENCES insumos (id_insumo)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- =====================================================================
-- Modulo 2: Control de Produccion
-- =====================================================================

-- ---------------------------------------------------------------------
-- lactancias
-- La abre el parto (CU18) o la carga manual de las vacas que ya estaban
-- en ordenie cuando arranco el sistema. fecha_secado se completa recien
-- al cerrarla, asi que el nulo es lo que identifica a la lactancia en
-- curso.
-- ---------------------------------------------------------------------
CREATE TABLE lactancias (
    id_lactancia         INT(11) NOT NULL AUTO_INCREMENT,
    numero_lactancia     INT(11) NOT NULL,
    fecha_inicio         DATE    NOT NULL,
    fecha_secado         DATE    NULL,
    fecha_probable_parto DATE    NULL,
    id_animal            INT(11) NOT NULL,
    PRIMARY KEY (id_lactancia),
    FOREIGN KEY (id_animal) REFERENCES hembras (id_animal)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ---------------------------------------------------------------------
-- ordenies_lote
-- Un unico registro por fecha y turno: esa es la clave alterna.
-- litros_totales son los litros del ordenie masivo del lote, sin la
-- leche de las vacas que se controlaron aparte (ver ordenies_individual).
-- ---------------------------------------------------------------------
CREATE TABLE ordenies_lote (
    id_ordenie_lote INT(11)      NOT NULL AUTO_INCREMENT,
    fecha           DATE         NOT NULL,
    turno           VARCHAR(10)  NOT NULL,
    litros_totales  DECIMAL(8,2) NOT NULL,
    PRIMARY KEY (id_ordenie_lote),
    UNIQUE (fecha, turno)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ---------------------------------------------------------------------
-- ordenie_lote_animales
-- Deja asentado que animales integraron el lote de cada turno. El Modelo
-- Entidad-Relacion no la contempla, pero sin ella el paso 4 de CU8 -donde
-- el usuario ajusta la lista de animales ordeniados- no deja rastro y no
-- hay forma de saber despues que vacas se ordeniaron.
-- ---------------------------------------------------------------------
CREATE TABLE ordenie_lote_animales (
    id_ordenie_lote INT(11) NOT NULL,
    id_animal       INT(11) NOT NULL,
    PRIMARY KEY (id_ordenie_lote, id_animal),
    FOREIGN KEY (id_ordenie_lote) REFERENCES ordenies_lote (id_ordenie_lote) ON DELETE CASCADE,
    FOREIGN KEY (id_animal) REFERENCES hembras (id_animal)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ---------------------------------------------------------------------
-- ordenies_individual
-- id_ordenie_lote admite nulo, a diferencia de lo que declara 2.2.5.4.
-- El control individual es puntual y no exige que el ordenie del lote de
-- esa fecha y turno ya este cargado. Ademas es lo que hace correcta la
-- regla de negocio de CU10 y CU11, que suman las dos fuentes para obtener
-- el neto: los litros del control individual no estan incluidos en los
-- litros_totales del lote.
-- ---------------------------------------------------------------------
-- Un animal tiene un solo control por fecha y turno: la carga a mano en un dia de
-- control lechero es propensa a la doble carga y la base la tiene que rechazar.
CREATE TABLE ordenies_individual (
    id_ordenie_ind  INT(11)      NOT NULL AUTO_INCREMENT,
    fecha           DATE         NOT NULL,
    turno           VARCHAR(10)  NOT NULL,
    litros          DECIMAL(6,2) NOT NULL,
    id_animal       INT(11)      NOT NULL,
    id_lactancia    INT(11)      NOT NULL,
    id_ordenie_lote INT(11)      NULL,
    PRIMARY KEY (id_ordenie_ind),
    UNIQUE (fecha, turno, id_animal),
    FOREIGN KEY (id_animal) REFERENCES hembras (id_animal),
    FOREIGN KEY (id_lactancia) REFERENCES lactancias (id_lactancia),
    FOREIGN KEY (id_ordenie_lote) REFERENCES ordenies_lote (id_ordenie_lote)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- =====================================================================
-- Modulo 3: Gestion Reproductiva
-- =====================================================================

-- ---------------------------------------------------------------------
-- celos
-- ---------------------------------------------------------------------
CREATE TABLE celos (
    id_celo       INT(11)      NOT NULL AUTO_INCREMENT,
    fecha         DATE         NOT NULL,
    observaciones VARCHAR(200) NULL,
    id_animal     INT(11)      NOT NULL,
    PRIMARY KEY (id_celo),
    FOREIGN KEY (id_animal) REFERENCES hembras (id_animal)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ---------------------------------------------------------------------
-- servicios
-- id_macho identifica al toro de la monta natural e id_insumo a la
-- pajuela de la inseminacion artificial. Son mutuamente excluyentes: la
-- restriccion se controla en la Controladora, porque MySQL no admite una
-- expresion de este tipo en un CHECK sobre columnas de otras tablas.
-- ---------------------------------------------------------------------
CREATE TABLE servicios (
    id_servicio          INT(11)      NOT NULL AUTO_INCREMENT,
    tipo_servicio        VARCHAR(20)  NOT NULL,
    fecha_servicio       DATE         NOT NULL,
    fecha_probable_parto DATE         NOT NULL,
    observaciones        VARCHAR(200) NULL,
    id_animal            INT(11)      NOT NULL,
    id_macho             INT(11)      NULL,
    id_insumo            INT(11)      NULL,
    PRIMARY KEY (id_servicio),
    FOREIGN KEY (id_animal) REFERENCES hembras (id_animal),
    FOREIGN KEY (id_macho) REFERENCES machos (id_animal),
    FOREIGN KEY (id_insumo) REFERENCES insumos (id_insumo)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ---------------------------------------------------------------------
-- tactos
-- Siempre cuelgan de un servicio: es el servicio, y no la hembra, lo que
-- el tacto viene a confirmar.
-- ---------------------------------------------------------------------
CREATE TABLE tactos (
    id_tacto      INT(11)      NOT NULL AUTO_INCREMENT,
    fecha_tacto   DATE         NOT NULL,
    resultado     VARCHAR(20)  NOT NULL,
    observaciones VARCHAR(200) NULL,
    id_servicio   INT(11)      NOT NULL,
    PRIMARY KEY (id_tacto),
    FOREIGN KEY (id_servicio) REFERENCES servicios (id_servicio)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ---------------------------------------------------------------------
-- partos
-- La cria no se referencia desde aca: queda vinculada por
-- animales.id_madre, que es lo que declara el modelo. El parto y su cria
-- se corresponden por la madre y por la fecha de nacimiento.
-- ---------------------------------------------------------------------
CREATE TABLE partos (
    id_parto      INT(11)      NOT NULL AUTO_INCREMENT,
    fecha_parto   DATE         NOT NULL,
    tipo_parto    VARCHAR(30)  NOT NULL,
    observaciones VARCHAR(200) NULL,
    id_animal     INT(11)      NOT NULL,
    PRIMARY KEY (id_parto),
    FOREIGN KEY (id_animal) REFERENCES hembras (id_animal)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- =====================================================================
-- Tablas adelantadas del Modulo 4: sanidad
-- Se crean porque el paso 3 de CU8 excluye del lote de ordenie a los
-- animales con periodo de descarte de leche vigente, y ese dato sale de
-- tratamientos.fecha_fin_descarte.
-- =====================================================================

-- ---------------------------------------------------------------------
-- diagnosticos
-- ---------------------------------------------------------------------
CREATE TABLE diagnosticos (
    id_diagnostico    INT(11)      NOT NULL AUTO_INCREMENT,
    fecha_diagnostico DATE         NOT NULL,
    enfermedad        VARCHAR(100) NOT NULL,
    estado            VARCHAR(20)  NOT NULL,
    id_animal         INT(11)      NOT NULL,
    PRIMARY KEY (id_diagnostico),
    FOREIGN KEY (id_animal) REFERENCES animales (id_animal)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ---------------------------------------------------------------------
-- tratamientos
-- id_diagnostico admite nulo: ese nulo es lo que identifica al
-- tratamiento preventivo, como la desparasitacion, que no se origina en
-- un diagnostico.
--
-- id_plan queda declarado pero todavia sin clave foranea: la tabla
-- planes_sanitarios se crea con el Modulo 4 y la restriccion se agrega
-- ahi con un ALTER TABLE.
-- ---------------------------------------------------------------------
CREATE TABLE tratamientos (
    id_tratamiento     INT(11)     NOT NULL AUTO_INCREMENT,
    fecha_inicio       DATE        NOT NULL,
    dias_duracion      INT(11)     NOT NULL,
    dosis_diaria       VARCHAR(60) NOT NULL,
    fecha_fin_descarte DATE        NULL,
    id_diagnostico     INT(11)     NULL,
    id_insumo          INT(11)     NOT NULL,
    id_plan            INT(11)     NULL,
    PRIMARY KEY (id_tratamiento),
    FOREIGN KEY (id_diagnostico) REFERENCES diagnosticos (id_diagnostico),
    FOREIGN KEY (id_insumo) REFERENCES insumos (id_insumo)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
