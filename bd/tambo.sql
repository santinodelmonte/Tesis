-- =====================================================================
-- Sistema de Gestion de Tambo
-- Script unico de creacion de la base de datos - Modulos 0 a 5
--
--   Modulo 0: Seguridad y Acceso al Sistema  (no requiere tablas: las
--             credenciales se leen de appsettings.json y se validan en
--             memoria)
--   Modulo 1: Gestion de Animales y Genetica
--   Modulo 2: Control de Produccion
--   Modulo 3: Gestion Reproductiva
--   Modulo 4: Gestion Sanitaria
--   Modulo 5: Control de Insumos y Stock
--   Configuracion del establecimiento
--
-- Este script reemplaza a la secuencia de scripts por modulo. Deja la
-- base en el mismo estado en el que la dejaban los cinco corridos en
-- orden, pero de una sola vez y sin ALTER TABLE intermedios: las tablas
-- se crean directamente con todas sus columnas y restricciones.
--
-- ATENCION: la primera sentencia BORRA la base tambo si ya existe, con
-- todos sus datos. Es lo que permite que el script se pueda volver a
-- correr y siempre deje el mismo esquema. Para conservar lo cargado hay
-- que respaldar antes:
--
--     mysqldump -u root -p tambo > respaldo_tambo.sql
--
-- Motor: MySQL
-- =====================================================================

DROP DATABASE IF EXISTS tambo;

CREATE DATABASE tambo
    DEFAULT CHARACTER SET utf8mb4
    DEFAULT COLLATE utf8mb4_general_ci;

USE tambo;

-- =====================================================================
-- Modulo 1: Gestion de Animales y Genetica
-- =====================================================================

-- ---------------------------------------------------------------------
-- razas
-- ---------------------------------------------------------------------
CREATE TABLE razas (
    id_raza     INT(11)      NOT NULL AUTO_INCREMENT,
    nombre      VARCHAR(60)  NOT NULL,
    descripcion VARCHAR(200) NULL,
    PRIMARY KEY (id_raza),
    UNIQUE (nombre)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ---------------------------------------------------------------------
-- categorias
-- ---------------------------------------------------------------------
CREATE TABLE categorias (
    id_categoria INT(11)      NOT NULL AUTO_INCREMENT,
    nombre       VARCHAR(60)  NOT NULL,
    descripcion  VARCHAR(200) NULL,
    PRIMARY KEY (id_categoria),
    UNIQUE (nombre)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ---------------------------------------------------------------------
-- animales
-- Las claves foraneas id_madre e id_padre se agregan mas abajo, porque
-- referencian a hembras y machos, que a su vez referencian a animales.
-- ---------------------------------------------------------------------
CREATE TABLE animales (
    id_animal        INT(11)      NOT NULL AUTO_INCREMENT,
    num_caravana     VARCHAR(20)  NOT NULL,
    fecha_nacimiento DATE         NOT NULL,
    activo           TINYINT(1)   NOT NULL DEFAULT 1,
    fecha_baja       DATE         NULL,
    motivo_baja      VARCHAR(100) NULL,
    id_raza          INT(11)      NOT NULL,
    id_categoria     INT(11)      NOT NULL,
    id_madre         INT(11)      NULL,
    id_padre         INT(11)      NULL,
    PRIMARY KEY (id_animal),
    UNIQUE (num_caravana),
    FOREIGN KEY (id_raza) REFERENCES razas (id_raza),
    FOREIGN KEY (id_categoria) REFERENCES categorias (id_categoria)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ---------------------------------------------------------------------
-- hembras
-- Comparte la clave primaria con animales: resuelve la especializacion.
-- ---------------------------------------------------------------------
CREATE TABLE hembras (
    id_animal           INT(11)     NOT NULL,
    numero_partos       INT(11)     NOT NULL DEFAULT 0,
    estado_productivo   VARCHAR(20) NOT NULL,
    estado_reproductivo VARCHAR(20) NOT NULL,
    PRIMARY KEY (id_animal),
    FOREIGN KEY (id_animal) REFERENCES animales (id_animal) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ---------------------------------------------------------------------
-- machos
-- Comparte la clave primaria con animales: resuelve la especializacion.
-- ---------------------------------------------------------------------
CREATE TABLE machos (
    id_animal INT(11)    NOT NULL,
    en_pie    TINYINT(1) NOT NULL,
    PRIMARY KEY (id_animal),
    FOREIGN KEY (id_animal) REFERENCES animales (id_animal) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ---------------------------------------------------------------------
-- Claves foraneas recursivas de animales
-- Admiten nulo cuando el progenitor no se encuentra registrado.
-- ---------------------------------------------------------------------
ALTER TABLE animales
    ADD FOREIGN KEY (id_madre) REFERENCES hembras (id_animal);

ALTER TABLE animales
    ADD FOREIGN KEY (id_padre) REFERENCES machos (id_animal);

-- =====================================================================
-- Modulo 5: Control de Insumos y Stock
--
-- Va antes que los modulos 2, 3 y 4 porque los tres dependen de estas
-- dos tablas: la inseminacion artificial consume una pajuela (CU15), el
-- tratamiento y la vacunacion descuentan el producto que aplican (CU20,
-- CU21) y el plan sanitario declara el insumo que exige (CU22).
--
-- El modulo no agrega nada mas: los umbrales, los vencimientos por
-- partida y el historial salen de estas mismas dos tablas.
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
-- litros_totales es la leche completa que salio en ese ordenie, tal como
-- se lee del tanque, incluida la de las vacas que ademas se midieron una
-- por una (ver ordenies_individual).
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
-- El ordenie individual no es otra fuente de leche: es el mismo ordenie
-- del turno, anotado vaca por vaca en lugar de con un solo numero. Sus
-- litros ya estan dentro de los litros_totales del lote de esa fecha y
-- turno, cuando ese lote existe.
--
-- id_ordenie_lote admite nulo, a diferencia de lo que declara 2.2.5.4,
-- porque el control no exige que el total del ordenie ya este cargado.
-- Cuando el lote no existe, la produccion de ese turno es la suma de sus
-- controles individuales: el ordenie ocurrio igual, lo unico distinto es
-- como se anoto. La produccion se resuelve asi turno por turno, y nunca
-- sumando las dos cosas dentro del mismo turno, que contaria dos veces la
-- leche de las vacas controladas. Esta explicado en el desvio D1 de
-- docs/desvios-modulos-2-y-3.md.
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
-- Modulo 4: Gestion Sanitaria
-- =====================================================================

-- ---------------------------------------------------------------------
-- planes_sanitarios
-- La regla del procedimiento periodico, no su aplicacion. El calendario
-- de CU23 sale de comparar lo que el plan exige contra lo que se aplico,
-- asi que aca no se guarda nada derivado.
--
-- periodicidad_dias admite nulo: el nulo indica que el procedimiento se
-- aplica una unica vez en la vida del animal, como el descorne.
-- id_insumo admite nulo por el mismo motivo: el descorne no consume
-- insumo (CU22, curso alternativo 4c).
-- ---------------------------------------------------------------------
CREATE TABLE planes_sanitarios (
    id_plan            INT(11)     NOT NULL AUTO_INCREMENT,
    nombre             VARCHAR(60) NOT NULL,
    tipo_procedimiento VARCHAR(20) NOT NULL,
    periodicidad_dias  INT(11)     NULL,
    edad_inicio_meses  INT(11)     NOT NULL,
    activo             TINYINT(1)  NOT NULL DEFAULT 1,
    id_insumo          INT(11)     NULL,
    PRIMARY KEY (id_plan),
    UNIQUE (nombre),
    FOREIGN KEY (id_insumo) REFERENCES insumos (id_insumo)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ---------------------------------------------------------------------
-- plan_categorias
-- Resuelve el vinculo muchos a muchos entre planes_sanitarios y
-- categorias. Un plan sin filas aca alcanza a todo el rodeo (CU22,
-- curso alternativo 4a), por eso la ausencia de fila es informacion y no
-- un dato faltante.
-- ---------------------------------------------------------------------
CREATE TABLE plan_categorias (
    id_plan      INT(11) NOT NULL,
    id_categoria INT(11) NOT NULL,
    PRIMARY KEY (id_plan, id_categoria),
    FOREIGN KEY (id_plan) REFERENCES planes_sanitarios (id_plan) ON DELETE CASCADE,
    FOREIGN KEY (id_categoria) REFERENCES categorias (id_categoria)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

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
-- id_animal es lo que permite atribuir ese preventivo a un animal: sin
-- la columna, la desparasitacion no genera descarte de leche para nadie
-- y el calendario sanitario nunca encuentra su ultima aplicacion. El
-- curso alternativo 2a de CU20 dice que el preventivo "se registra
-- directamente sobre el animal", que es exactamente esta columna. Queda
-- como Null porque asi la dejaron los scripts por modulo, donde se
-- agrego despues de que ya hubiera tratamientos cargados; de todos
-- modos la Controladora siempre la completa.
--
-- id_plan admite nulo cuando el tratamiento se registra fuera de todo
-- plan sanitario.
-- ---------------------------------------------------------------------
CREATE TABLE tratamientos (
    id_tratamiento     INT(11)     NOT NULL AUTO_INCREMENT,
    fecha_inicio       DATE        NOT NULL,
    dias_duracion      INT(11)     NOT NULL,
    dosis_diaria       VARCHAR(60) NOT NULL,
    id_animal          INT(11)     NULL,
    fecha_fin_descarte DATE        NULL,
    id_diagnostico     INT(11)     NULL,
    id_insumo          INT(11)     NOT NULL,
    id_plan            INT(11)     NULL,
    PRIMARY KEY (id_tratamiento),
    FOREIGN KEY (id_animal) REFERENCES animales (id_animal),
    FOREIGN KEY (id_diagnostico) REFERENCES diagnosticos (id_diagnostico),
    FOREIGN KEY (id_insumo) REFERENCES insumos (id_insumo),
    FOREIGN KEY (id_plan) REFERENCES planes_sanitarios (id_plan)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ---------------------------------------------------------------------
-- vacunaciones
-- id_plan declara explicitamente que plan da por cumplido la aplicacion.
-- Es nulo cuando se vacuna fuera de todo plan (CU21, curso alternativo
-- 4a). El calendario no lo infiere del insumo: dos planes distintos
-- pueden usar la misma vacuna.
-- ---------------------------------------------------------------------
CREATE TABLE vacunaciones (
    id_vacunacion    INT(11) NOT NULL AUTO_INCREMENT,
    fecha_aplicacion DATE    NOT NULL,
    id_animal        INT(11) NOT NULL,
    id_insumo        INT(11) NOT NULL,
    id_plan          INT(11) NULL,
    PRIMARY KEY (id_vacunacion),
    FOREIGN KEY (id_animal) REFERENCES animales (id_animal),
    FOREIGN KEY (id_insumo) REFERENCES insumos (id_insumo),
    FOREIGN KEY (id_plan) REFERENCES planes_sanitarios (id_plan)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- ---------------------------------------------------------------------
-- descornes
-- El descorne es de aplicacion unica: un plan de descorne deja de
-- exigirlo para el animal una vez que se registro (CU24, regla de
-- negocio).
-- ---------------------------------------------------------------------
CREATE TABLE descornes (
    id_descorne   INT(11)      NOT NULL AUTO_INCREMENT,
    fecha         DATE         NOT NULL,
    metodo        VARCHAR(60)  NOT NULL,
    observaciones VARCHAR(200) NULL,
    id_animal     INT(11)      NOT NULL,
    id_plan       INT(11)      NULL,
    PRIMARY KEY (id_descorne),
    FOREIGN KEY (id_animal) REFERENCES animales (id_animal),
    FOREIGN KEY (id_plan) REFERENCES planes_sanitarios (id_plan)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- =====================================================================
-- Configuracion del establecimiento
--
-- Los parametros de manejo que antes vivian como constantes en la
-- Controladora. No son hechos biologicos sino decisiones del
-- establecimiento: cuantos dias antes del parto se seca una vaca, a que
-- edad entra la vaquillona en servicio, cuantos ordenies se hacen por
-- dia y con cuanta anticipacion avisa cada alerta. Dos tambos vecinos
-- las resuelven distinto.
--
-- La gestacion, la edad de pubertad y el rango de duracion de una
-- preniez viable NO estan aca: no son decisiones, son biologia, y siguen
-- en el codigo. Lo mismo el periodo de carencia y el stock minimo, que
-- son datos del producto y ya viven en insumos.
-- =====================================================================

-- ---------------------------------------------------------------------
-- configuracion
-- Tabla de una sola fila. El sistema lee siempre la primera y escribe
-- sobre ella: no hay un alta de configuraciones, hay una configuracion
-- del establecimiento que se modifica.
--
-- Cada columna trae como valor por defecto la constante que reemplaza,
-- asi que una base recien creada se comporta igual que antes de que la
-- configuracion existiera.
-- ---------------------------------------------------------------------
CREATE TABLE configuracion (
    id_configuracion              INT(11)      NOT NULL AUTO_INCREMENT,
    dias_secado_antes_parto       INT(11)      NOT NULL DEFAULT 60,
    edad_minima_servicio_meses    INT(11)      NOT NULL DEFAULT 13,
    edad_cambio_categoria_meses   INT(11)      NOT NULL DEFAULT 12,
    litros_maximos_individual     DECIMAL(6,2) NOT NULL DEFAULT 100,
    ordenies_por_dia              INT(11)      NOT NULL DEFAULT 2,
    dias_anticipacion_secado      INT(11)      NOT NULL DEFAULT 15,
    dias_anticipacion_parto       INT(11)      NOT NULL DEFAULT 15,
    dias_anticipacion_sanitaria   INT(11)      NOT NULL DEFAULT 30,
    dias_anticipacion_vencimiento INT(11)      NOT NULL DEFAULT 30,
    dias_espera_voluntaria        INT(11)      NOT NULL DEFAULT 45,
    dias_para_tacto               INT(11)      NOT NULL DEFAULT 35,
    PRIMARY KEY (id_configuracion)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- =====================================================================
-- Datos semilla
-- Razas y categorias no tienen alta desde el sistema: se cargan aca.
-- =====================================================================

INSERT INTO razas (nombre, descripcion) VALUES
    ('Holando', 'Raza lechera predominante en el rodeo.'),
    ('Jersey', 'Raza lechera de menor porte y alto tenor graso.'),
    ('Normando', 'Raza de doble proposito.'),
    ('Cruza Holando-Jersey', 'Cruzamiento entre Holando y Jersey.'),
    ('Otra', 'Raza no contemplada en el listado.');

INSERT INTO categorias (nombre, descripcion) VALUES
    ('Ternera', 'Hembra de hasta 12 meses de edad.'),
    ('Novilla', 'Hembra de mas de 12 meses sin partos registrados.'),
    ('Vaca', 'Hembra con uno o mas partos registrados.'),
    ('Ternero', 'Macho de hasta 12 meses de edad.'),
    ('Novillo', 'Macho de mas de 12 meses que no se destina a reproduccion.'),
    ('Toro', 'Macho de mas de 15 meses que integra el rodeo como reproductor.');

-- La fila unica de configuracion, con los valores por defecto de cada
-- columna. Si esta fila no existiera el sistema igual funciona: la
-- Controladora usa las constantes como respaldo.
INSERT INTO configuracion (id_configuracion) VALUES (1);
