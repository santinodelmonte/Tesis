-- =====================================================================
-- Sistema de Gestion de Tambo
-- Script de creacion de la base de datos - Modulos 0 y 1
--
--   Modulo 0: Seguridad y Acceso al Sistema  (no requiere tablas: las
--             credenciales son fijas y se validan en memoria)
--   Modulo 1: Gestion de Animales y Genetica (razas, categorias,
--             animales, hembras, machos)
--
-- Motor: MySQL
-- Las tablas de los modulos 2 a 6 se agregan a medida que se desarrollan.
-- =====================================================================

CREATE DATABASE IF NOT EXISTS tambo
    DEFAULT CHARACTER SET utf8mb4
    DEFAULT COLLATE utf8mb4_general_ci;

USE tambo;

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
    -- Nombre del archivo de la foto dentro de wwwroot/fotos. La imagen no se
    -- guarda en la base: aca queda nada mas que como se llama el archivo.
    foto             VARCHAR(120) NULL,
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
