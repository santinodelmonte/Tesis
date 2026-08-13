-- =====================================================================
-- Sistema de Gestion de Tambo
-- Actualizacion de la base de datos - Modulos 0 a 5
--
-- Lleva una base tambo existente al esquema actual SIN BORRAR NADA. Es
-- la alternativa a tambo.sql para una instalacion que ya tiene datos
-- cargados: aquel empieza con un DROP DATABASE y sirve para crear de
-- cero, este compara lo que hay contra lo que tiene que haber y agrega
-- unicamente lo que falta.
--
-- Reemplaza a los dos scripts de actualizacion sueltos que habia
-- (tambo_foto_animal.sql y tambo_configuracion_actualizacion.sql), que
-- fallaban si la columna ya estaba. Este no: se puede correr las veces
-- que haga falta y sobre una base en cualquier estado anterior -recien
-- creada, a mitad de los modulos, o ya al dia-. Si no hay nada que
-- hacer, no hace nada.
--
--     mysql -u root -p < bd/tambo_actualizacion.sql
--
-- Al terminar imprime tres informes: que cambio, cuantas filas tiene
-- cada tabla y si quedo algo pendiente. Lo que no puede resolver solo
-- -una restriccion que los datos actuales violan- lo deja anotado en el
-- primer informe con el motivo, en lugar de cortar el script.
--
-- Aun asi, respaldar antes es barato:
--
--     mysqldump -u root -p tambo > respaldo_tambo.sql
--
-- Y respaldar tambien Tesis/wwwroot/fotos, que no esta en la base.
--
-- Motor: MySQL
-- =====================================================================

CREATE DATABASE IF NOT EXISTS tambo
    DEFAULT CHARACTER SET utf8mb4
    DEFAULT COLLATE utf8mb4_general_ci;

USE tambo;


-- =====================================================================
-- Bitacora
-- Cada cambio que el script aplica -o que decide no aplicar- deja una
-- linea aca, y al final se imprimen todas juntas. Es temporal: vive lo
-- que dura la conexion y no queda en la base.
-- =====================================================================

DROP TEMPORARY TABLE IF EXISTS tambo_bitacora;

CREATE TEMPORARY TABLE tambo_bitacora (
    orden  INT AUTO_INCREMENT PRIMARY KEY,
    accion VARCHAR(20)  NOT NULL,
    objeto VARCHAR(120) NOT NULL,
    detalle VARCHAR(200) NULL
) ENGINE=MEMORY;


-- =====================================================================
-- Herramientas
--
-- MySQL no tiene ADD COLUMN IF NOT EXISTS ni ADD FOREIGN KEY IF NOT
-- EXISTS, asi que la unica forma de escribir una actualizacion que se
-- pueda repetir es consultar information_schema y armar el ALTER en el
-- momento. Estos cuatro procedimientos encapsulan esa consulta para que
-- el cuerpo del script -mas abajo- se lea como una lista de lo que la
-- base tiene que tener, y no como una maraña de condicionales.
--
-- Se borran al final: son andamios, no parte del esquema.
-- =====================================================================

DROP PROCEDURE IF EXISTS tambo_agregar_columna;
DROP PROCEDURE IF EXISTS tambo_permitir_nulo;
DROP PROCEDURE IF EXISTS tambo_agregar_unico;
DROP PROCEDURE IF EXISTS tambo_agregar_foranea;

DELIMITER $$

-- ---------------------------------------------------------------------
-- Agrega una columna si todavia no existe.
-- pDefinicion es el tipo completo, incluida la posicion: por ejemplo
-- 'VARCHAR(120) NULL AFTER motivo_baja'.
-- ---------------------------------------------------------------------
CREATE PROCEDURE tambo_agregar_columna(
    IN pTabla      VARCHAR(64),
    IN pColumna    VARCHAR(64),
    IN pDefinicion VARCHAR(255))
BEGIN
    DECLARE vExiste INT DEFAULT 0;

    SELECT COUNT(*) INTO vExiste
      FROM information_schema.COLUMNS
     WHERE TABLE_SCHEMA = DATABASE()
       AND TABLE_NAME   = pTabla
       AND COLUMN_NAME  = pColumna;

    IF vExiste = 0 THEN
        SET @tambo_sql = CONCAT('ALTER TABLE `', pTabla, '` ADD COLUMN `',
                                pColumna, '` ', pDefinicion);
        PREPARE tambo_stmt FROM @tambo_sql;
        EXECUTE tambo_stmt;
        DEALLOCATE PREPARE tambo_stmt;

        INSERT INTO tambo_bitacora (accion, objeto, detalle)
        VALUES ('Columna nueva', CONCAT(pTabla, '.', pColumna), pDefinicion);
    END IF;
END$$

-- ---------------------------------------------------------------------
-- Afloja una columna declarada Not null para que admita nulo.
-- Hace falta porque el nulo, en varias columnas de este esquema, es
-- informacion: el ordenie individual sin lote, el plan sin periodicidad.
-- ---------------------------------------------------------------------
CREATE PROCEDURE tambo_permitir_nulo(
    IN pTabla      VARCHAR(64),
    IN pColumna    VARCHAR(64),
    IN pDefinicion VARCHAR(255))
BEGIN
    DECLARE vNoAdmite INT DEFAULT 0;

    SELECT COUNT(*) INTO vNoAdmite
      FROM information_schema.COLUMNS
     WHERE TABLE_SCHEMA = DATABASE()
       AND TABLE_NAME   = pTabla
       AND COLUMN_NAME  = pColumna
       AND IS_NULLABLE  = 'NO';

    IF vNoAdmite = 1 THEN
        SET @tambo_sql = CONCAT('ALTER TABLE `', pTabla, '` MODIFY COLUMN `',
                                pColumna, '` ', pDefinicion);
        PREPARE tambo_stmt FROM @tambo_sql;
        EXECUTE tambo_stmt;
        DEALLOCATE PREPARE tambo_stmt;

        INSERT INTO tambo_bitacora (accion, objeto, detalle)
        VALUES ('Columna aflojada', CONCAT(pTabla, '.', pColumna),
                'Pasa a admitir nulo');
    END IF;
END$$

-- ---------------------------------------------------------------------
-- Agrega una clave alterna si no hay ya un indice unico sobre esas
-- mismas columnas, en ese mismo orden. El nombre del indice no entra en
-- la comparacion: los scripts viejos declaraban UNIQUE sin nombrarlo y
-- MySQL les puso el nombre de la primera columna.
--
-- Antes de agregarla comprueba que los datos actuales no la violen. Si
-- la violan no fuerza nada: anota el conflicto y sigue, porque cual de
-- las filas repetidas sobra es una decision del establecimiento y no de
-- este script.
--
-- pColumnas va sin espacios: 'fecha,turno'.
-- ---------------------------------------------------------------------
CREATE PROCEDURE tambo_agregar_unico(
    IN pTabla    VARCHAR(64),
    IN pColumnas VARCHAR(255))
BEGIN
    DECLARE vExiste     INT DEFAULT 0;
    DECLARE vRepetidas  INT DEFAULT 0;

    SELECT COUNT(*) INTO vExiste FROM (
        SELECT INDEX_NAME
          FROM information_schema.STATISTICS
         WHERE TABLE_SCHEMA = DATABASE()
           AND TABLE_NAME   = pTabla
           AND NON_UNIQUE   = 0
         GROUP BY INDEX_NAME
        HAVING GROUP_CONCAT(COLUMN_NAME ORDER BY SEQ_IN_INDEX) = pColumnas
    ) AS indices_equivalentes;

    IF vExiste = 0 THEN
        SET @tambo_sql = CONCAT('SELECT COUNT(*) INTO @tambo_repetidas FROM (',
                                'SELECT 1 FROM `', pTabla, '` GROUP BY ', pColumnas,
                                ' HAVING COUNT(*) > 1) AS duplicados');
        PREPARE tambo_stmt FROM @tambo_sql;
        EXECUTE tambo_stmt;
        DEALLOCATE PREPARE tambo_stmt;
        SET vRepetidas = @tambo_repetidas;

        IF vRepetidas > 0 THEN
            INSERT INTO tambo_bitacora (accion, objeto, detalle)
            VALUES ('PENDIENTE', CONCAT(pTabla, ' UNIQUE (', pColumnas, ')'),
                    CONCAT('No se agrego: hay ', vRepetidas,
                           ' combinaciones repetidas para depurar a mano'));
        ELSE
            SET @tambo_sql = CONCAT('ALTER TABLE `', pTabla, '` ADD UNIQUE (', pColumnas, ')');
            PREPARE tambo_stmt FROM @tambo_sql;
            EXECUTE tambo_stmt;
            DEALLOCATE PREPARE tambo_stmt;

            INSERT INTO tambo_bitacora (accion, objeto, detalle)
            VALUES ('Clave alterna', pTabla, CONCAT('UNIQUE (', pColumnas, ')'));
        END IF;
    END IF;
END$$

-- ---------------------------------------------------------------------
-- Agrega una clave foranea si la columna todavia no tiene ninguna.
--
-- Igual que la clave alterna, primero comprueba que los datos la
-- soporten: cuenta las filas que apuntan a un padre inexistente y, si
-- hay alguna, no agrega la restriccion y deja anotado cuantas son. Un
-- ALTER que revienta a mitad del script dejaria la base a medio
-- actualizar; un pendiente anotado, no.
--
-- pAlBorrar es '' o 'ON DELETE CASCADE'.
-- ---------------------------------------------------------------------
CREATE PROCEDURE tambo_agregar_foranea(
    IN pTabla      VARCHAR(64),
    IN pColumna    VARCHAR(64),
    IN pTablaRef   VARCHAR(64),
    IN pColumnaRef VARCHAR(64),
    IN pAlBorrar   VARCHAR(30))
BEGIN
    DECLARE vExiste    INT DEFAULT 0;
    DECLARE vHuerfanas INT DEFAULT 0;

    SELECT COUNT(*) INTO vExiste
      FROM information_schema.KEY_COLUMN_USAGE
     WHERE TABLE_SCHEMA = DATABASE()
       AND TABLE_NAME   = pTabla
       AND COLUMN_NAME  = pColumna
       AND REFERENCED_TABLE_NAME IS NOT NULL;

    IF vExiste = 0 THEN
        SET @tambo_sql = CONCAT('SELECT COUNT(*) INTO @tambo_huerfanas',
                                ' FROM `', pTabla, '` h',
                                ' LEFT JOIN `', pTablaRef, '` p',
                                ' ON p.`', pColumnaRef, '` = h.`', pColumna, '`',
                                ' WHERE h.`', pColumna, '` IS NOT NULL',
                                '   AND p.`', pColumnaRef, '` IS NULL');
        PREPARE tambo_stmt FROM @tambo_sql;
        EXECUTE tambo_stmt;
        DEALLOCATE PREPARE tambo_stmt;
        SET vHuerfanas = @tambo_huerfanas;

        IF vHuerfanas > 0 THEN
            INSERT INTO tambo_bitacora (accion, objeto, detalle)
            VALUES ('PENDIENTE', CONCAT(pTabla, '.', pColumna, ' -> ', pTablaRef),
                    CONCAT('No se agrego: ', vHuerfanas,
                           ' filas apuntan a un registro que no existe'));
        ELSE
            SET @tambo_sql = CONCAT('ALTER TABLE `', pTabla, '` ADD FOREIGN KEY (`',
                                    pColumna, '`) REFERENCES `', pTablaRef, '` (`',
                                    pColumnaRef, '`) ', pAlBorrar);
            PREPARE tambo_stmt FROM @tambo_sql;
            EXECUTE tambo_stmt;
            DEALLOCATE PREPARE tambo_stmt;

            INSERT INTO tambo_bitacora (accion, objeto, detalle)
            VALUES ('Clave foranea', CONCAT(pTabla, '.', pColumna),
                    CONCAT('Referencia a ', pTablaRef, '.', pColumnaRef));
        END IF;
    END IF;
END$$

DELIMITER ;


-- =====================================================================
-- Paso 1 de 4: las tablas
--
-- Van en el orden en que se pueden crear: cada una despues de aquellas
-- a las que referencia. Las definiciones son identicas a las de
-- tambo.sql -que es el documento de referencia del esquema y donde
-- estan explicadas una por una-, salvo por dos diferencias:
--
--   - IF NOT EXISTS, para que la tabla que ya esta quede intacta con
--     sus datos;
--   - las claves foraneas que cierran ciclos (animales -> hembras ->
--     animales) no van declaradas aca sino en el paso 2, que es el que
--     sabe agregarlas sin repetirlas.
--
-- Una tabla que ya existia pero le falta una columna no se arregla en
-- este paso: IF NOT EXISTS la saltea entera. De eso se ocupa el paso 2.
-- =====================================================================

-- --- Modulo 1: Gestion de Animales y Genetica ------------------------

CREATE TABLE IF NOT EXISTS razas (
    id_raza     INT(11)      NOT NULL AUTO_INCREMENT,
    nombre      VARCHAR(60)  NOT NULL,
    descripcion VARCHAR(200) NULL,
    PRIMARY KEY (id_raza),
    UNIQUE (nombre)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS categorias (
    id_categoria INT(11)      NOT NULL AUTO_INCREMENT,
    nombre       VARCHAR(60)  NOT NULL,
    descripcion  VARCHAR(200) NULL,
    PRIMARY KEY (id_categoria),
    UNIQUE (nombre)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS animales (
    id_animal        INT(11)      NOT NULL AUTO_INCREMENT,
    num_caravana     VARCHAR(20)  NOT NULL,
    fecha_nacimiento DATE         NOT NULL,
    activo           TINYINT(1)   NOT NULL DEFAULT 1,
    fecha_baja       DATE         NULL,
    motivo_baja      VARCHAR(100) NULL,
    -- Nombre del archivo dentro de wwwroot/fotos, no la imagen.
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

CREATE TABLE IF NOT EXISTS hembras (
    id_animal           INT(11)     NOT NULL,
    numero_partos       INT(11)     NOT NULL DEFAULT 0,
    estado_productivo   VARCHAR(20) NOT NULL,
    estado_reproductivo VARCHAR(20) NOT NULL,
    PRIMARY KEY (id_animal),
    FOREIGN KEY (id_animal) REFERENCES animales (id_animal) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS machos (
    id_animal INT(11)    NOT NULL,
    en_pie    TINYINT(1) NOT NULL,
    PRIMARY KEY (id_animal),
    FOREIGN KEY (id_animal) REFERENCES animales (id_animal) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --- Modulo 5: Control de Insumos y Stock ----------------------------
-- Van antes que los modulos 2, 3 y 4 porque los tres dependen de ellas.

CREATE TABLE IF NOT EXISTS insumos (
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

CREATE TABLE IF NOT EXISTS movimientos_stock (
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

-- --- Modulo 2: Control de Produccion ---------------------------------

CREATE TABLE IF NOT EXISTS lactancias (
    id_lactancia         INT(11) NOT NULL AUTO_INCREMENT,
    numero_lactancia     INT(11) NOT NULL,
    fecha_inicio         DATE    NOT NULL,
    fecha_secado         DATE    NULL,
    fecha_probable_parto DATE    NULL,
    id_animal            INT(11) NOT NULL,
    PRIMARY KEY (id_lactancia),
    FOREIGN KEY (id_animal) REFERENCES hembras (id_animal)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS ordenies_lote (
    id_ordenie_lote INT(11)      NOT NULL AUTO_INCREMENT,
    fecha           DATE         NOT NULL,
    turno           VARCHAR(10)  NOT NULL,
    litros_totales  DECIMAL(8,2) NOT NULL,
    PRIMARY KEY (id_ordenie_lote),
    UNIQUE (fecha, turno)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS ordenie_lote_animales (
    id_ordenie_lote INT(11) NOT NULL,
    id_animal       INT(11) NOT NULL,
    PRIMARY KEY (id_ordenie_lote, id_animal),
    FOREIGN KEY (id_ordenie_lote) REFERENCES ordenies_lote (id_ordenie_lote) ON DELETE CASCADE,
    FOREIGN KEY (id_animal) REFERENCES hembras (id_animal)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS ordenies_individual (
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

-- --- Modulo 3: Gestion Reproductiva ----------------------------------

CREATE TABLE IF NOT EXISTS celos (
    id_celo       INT(11)      NOT NULL AUTO_INCREMENT,
    fecha         DATE         NOT NULL,
    observaciones VARCHAR(200) NULL,
    id_animal     INT(11)      NOT NULL,
    PRIMARY KEY (id_celo),
    FOREIGN KEY (id_animal) REFERENCES hembras (id_animal)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS servicios (
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

CREATE TABLE IF NOT EXISTS tactos (
    id_tacto      INT(11)      NOT NULL AUTO_INCREMENT,
    fecha_tacto   DATE         NOT NULL,
    resultado     VARCHAR(20)  NOT NULL,
    observaciones VARCHAR(200) NULL,
    id_servicio   INT(11)      NOT NULL,
    PRIMARY KEY (id_tacto),
    FOREIGN KEY (id_servicio) REFERENCES servicios (id_servicio)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS partos (
    id_parto      INT(11)      NOT NULL AUTO_INCREMENT,
    fecha_parto   DATE         NOT NULL,
    tipo_parto    VARCHAR(30)  NOT NULL,
    observaciones VARCHAR(200) NULL,
    id_animal     INT(11)      NOT NULL,
    PRIMARY KEY (id_parto),
    FOREIGN KEY (id_animal) REFERENCES hembras (id_animal)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --- Modulo 4: Gestion Sanitaria -------------------------------------

CREATE TABLE IF NOT EXISTS planes_sanitarios (
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

CREATE TABLE IF NOT EXISTS plan_categorias (
    id_plan      INT(11) NOT NULL,
    id_categoria INT(11) NOT NULL,
    PRIMARY KEY (id_plan, id_categoria),
    FOREIGN KEY (id_plan) REFERENCES planes_sanitarios (id_plan) ON DELETE CASCADE,
    FOREIGN KEY (id_categoria) REFERENCES categorias (id_categoria)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS diagnosticos (
    id_diagnostico    INT(11)      NOT NULL AUTO_INCREMENT,
    fecha_diagnostico DATE         NOT NULL,
    enfermedad        VARCHAR(100) NOT NULL,
    estado            VARCHAR(20)  NOT NULL,
    id_animal         INT(11)      NOT NULL,
    PRIMARY KEY (id_diagnostico),
    FOREIGN KEY (id_animal) REFERENCES animales (id_animal)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS tratamientos (
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
    FOREIGN KEY (id_diagnostico) REFERENCES diagnosticos (id_diagnostico),
    FOREIGN KEY (id_insumo) REFERENCES insumos (id_insumo)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

CREATE TABLE IF NOT EXISTS vacunaciones (
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

CREATE TABLE IF NOT EXISTS descornes (
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

-- --- Configuracion del establecimiento -------------------------------

CREATE TABLE IF NOT EXISTS configuracion (
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
-- Paso 2 de 4: lo que le falta a una tabla que ya existia
--
-- Cada linea de esta seccion se corresponde con un incremento del
-- desarrollo que llego despues de que la tabla estuviera creada. En una
-- base al dia ninguna hace nada; en una base vieja, cada una repone
-- exactamente lo que le falta.
-- =====================================================================

-- --- La foto del animal ----------------------------------------------
-- Llego con el arbol genealogico. Reemplaza a bd/tambo_foto_animal.sql.
CALL tambo_agregar_columna('animales', 'foto',
     'VARCHAR(120) NULL AFTER motivo_baja');

-- --- El linaje ---------------------------------------------------------
-- Las dos foraneas recursivas de animales. No pueden ir en el CREATE
-- porque apuntan a hembras y machos, que a su vez apuntan a animales.
CALL tambo_agregar_foranea('animales', 'id_madre', 'hembras', 'id_animal', '');
CALL tambo_agregar_foranea('animales', 'id_padre', 'machos',  'id_animal', '');

-- --- El tratamiento preventivo -----------------------------------------
-- id_animal es lo que permite atribuirle un preventivo a un animal
-- cuando no hay diagnostico del que deducirlo. El UPDATE completa los
-- tratamientos ya cargados que si venian de uno, y por eso va entre el
-- ADD COLUMN y el ADD FOREIGN KEY: la restriccion tiene que encontrar la
-- columna ya poblada. Esta explicado en docs/desvios-modulos-4-y-5.md.
CALL tambo_agregar_columna('tratamientos', 'id_animal',
     'INT(11) NULL AFTER dosis_diaria');

UPDATE tratamientos t
  JOIN diagnosticos d ON d.id_diagnostico = t.id_diagnostico
   SET t.id_animal = d.id_animal
 WHERE t.id_animal IS NULL;

CALL tambo_agregar_foranea('tratamientos', 'id_animal', 'animales',          'id_animal', '');
CALL tambo_agregar_foranea('tratamientos', 'id_plan',   'planes_sanitarios', 'id_plan',   '');

-- --- Lo que consumio el tratamiento ------------------------------------
-- Llego con la correccion de registros de Sanidad. La cantidad aplicada
-- vivia unicamente en movimientos_stock, que no dice de que tratamiento
-- salio: al eliminar o corregir uno no habia con que saber cuanto
-- devolverle al inventario. Los tratamientos ya cargados quedan en cero
-- y por eso no devuelven stock cuando se los elimina, que es preferible
-- a inventar una cantidad. Esta explicado en
-- docs/correccion-de-registros-y-navegacion.md.
CALL tambo_agregar_columna('tratamientos', 'cantidad_insumo',
     'DECIMAL(10,2) NOT NULL DEFAULT 0 AFTER dosis_diaria');

-- --- Los dos parametros de trabajo reproductivo ------------------------
-- Los usan las listas de trabajo diarias: las vacas en condiciones de
-- ser servidas y los tactos que le tocan al veterinario. Reemplazan a
-- bd/tambo_configuracion_actualizacion.sql.
CALL tambo_agregar_columna('configuracion', 'dias_espera_voluntaria',
     'INT(11) NOT NULL DEFAULT 45 AFTER dias_anticipacion_vencimiento');
CALL tambo_agregar_columna('configuracion', 'dias_para_tacto',
     'INT(11) NOT NULL DEFAULT 35 AFTER dias_espera_voluntaria');

-- --- El ordenie individual sin lote ------------------------------------
-- El control individual dejo de exigir que el total del turno ya este
-- cargado: cuando no hay lote, la produccion del turno es la suma de los
-- controles. Es el desvio D1 de docs/desvios-modulos-2-y-3.md.
CALL tambo_permitir_nulo('ordenies_individual', 'id_ordenie_lote', 'INT(11) NULL');

-- --- Las dos claves alternas del ordenie -------------------------------
-- Un solo registro de lote por fecha y turno, y un solo control
-- individual por animal en ese turno: la carga a mano en un dia de
-- control lechero es propensa a la doble carga y la base la tiene que
-- rechazar.
CALL tambo_agregar_unico('ordenies_lote',       'fecha,turno');
CALL tambo_agregar_unico('ordenies_individual', 'fecha,turno,id_animal');


-- =====================================================================
-- Paso 3 de 4: los datos que no se dan de alta desde el sistema
--
-- Razas y categorias no tienen pantalla de alta y la configuracion es
-- una fila unica: los tres los tiene que poner el script. Van con
-- INSERT IGNORE contra la clave alterna nombre, asi que una descripcion
-- que el establecimiento haya editado a mano no se pisa.
-- =====================================================================

INSERT IGNORE INTO razas (nombre, descripcion) VALUES
    ('Holando',              'Raza lechera predominante en el rodeo.'),
    ('Jersey',               'Raza lechera de menor porte y alto tenor graso.'),
    ('Normando',             'Raza de doble proposito.'),
    ('Cruza Holando-Jersey', 'Cruzamiento entre Holando y Jersey.'),
    ('Otra',                 'Raza no contemplada en el listado.');

INSERT IGNORE INTO categorias (nombre, descripcion) VALUES
    ('Ternera', 'Hembra de hasta 12 meses de edad.'),
    ('Novilla', 'Hembra de mas de 12 meses sin partos registrados.'),
    ('Vaca',    'Hembra con uno o mas partos registrados.'),
    ('Ternero', 'Macho de hasta 12 meses de edad.'),
    ('Novillo', 'Macho de mas de 12 meses que no se destina a reproduccion.'),
    ('Toro',    'Macho de mas de 15 meses que integra el rodeo como reproductor.');

-- La fila unica de configuracion, solo si la tabla esta vacia. Los
-- valores los pone el DEFAULT de cada columna, que es la constante que
-- ese parametro vino a reemplazar.
SELECT COUNT(*) INTO @tambo_configuraciones FROM configuracion;

INSERT INTO configuracion (id_configuracion)
SELECT 1 FROM DUAL WHERE @tambo_configuraciones = 0;


-- =====================================================================
-- Paso 4 de 4: informes
-- =====================================================================

-- --- Que cambio --------------------------------------------------------
-- Las lineas marcadas PENDIENTE son las unicas que piden intervencion:
-- una restriccion que los datos actuales no soportan. El resto es el
-- registro de lo que el script aplico. Sin filas significa que la base
-- ya estaba al dia.
--
-- El conteo va en una sentencia aparte y no como subconsulta del INSERT
-- porque MySQL no admite nombrar dos veces a una tabla temporal dentro
-- de la misma sentencia.
SELECT COUNT(*) INTO @tambo_cambios FROM tambo_bitacora;

INSERT INTO tambo_bitacora (accion, objeto, detalle)
SELECT 'Sin cambios', 'La base ya estaba al dia', NULL
  FROM DUAL WHERE @tambo_cambios = 0;

SELECT accion, objeto, detalle FROM tambo_bitacora ORDER BY orden;

-- --- Como quedo el esquema ---------------------------------------------
-- Las veintidos tablas de los Modulos 1 a 5 mas la de configuracion, con
-- lo que tiene cargado cada una. La columna faltante avisa si alguna no
-- llego a crearse.
SELECT esperada.tabla,
       CASE WHEN t.TABLE_NAME IS NULL THEN 'FALTA' ELSE 'OK' END AS estado,
       COALESCE(t.TABLE_ROWS, 0)                                 AS filas_aprox
  FROM (
        SELECT 'razas' AS tabla UNION ALL SELECT 'categorias'
         UNION ALL SELECT 'animales'            UNION ALL SELECT 'hembras'
         UNION ALL SELECT 'machos'              UNION ALL SELECT 'insumos'
         UNION ALL SELECT 'movimientos_stock'   UNION ALL SELECT 'lactancias'
         UNION ALL SELECT 'ordenies_lote'       UNION ALL SELECT 'ordenie_lote_animales'
         UNION ALL SELECT 'ordenies_individual' UNION ALL SELECT 'celos'
         UNION ALL SELECT 'servicios'           UNION ALL SELECT 'tactos'
         UNION ALL SELECT 'partos'              UNION ALL SELECT 'planes_sanitarios'
         UNION ALL SELECT 'plan_categorias'     UNION ALL SELECT 'diagnosticos'
         UNION ALL SELECT 'tratamientos'        UNION ALL SELECT 'vacunaciones'
         UNION ALL SELECT 'descornes'           UNION ALL SELECT 'configuracion'
       ) AS esperada
  LEFT JOIN information_schema.TABLES t
         ON t.TABLE_SCHEMA = DATABASE()
        AND t.TABLE_NAME   = esperada.tabla
 ORDER BY estado, esperada.tabla;

-- filas_aprox sale de las estadisticas de InnoDB y es una estimacion, no
-- un conteo. Sirve para ver de un vistazo que tabla quedo vacia, no para
-- informar volumenes.


-- =====================================================================
-- Los andamios se retiran
-- =====================================================================

DROP PROCEDURE IF EXISTS tambo_agregar_columna;
DROP PROCEDURE IF EXISTS tambo_permitir_nulo;
DROP PROCEDURE IF EXISTS tambo_agregar_unico;
DROP PROCEDURE IF EXISTS tambo_agregar_foranea;
DROP TEMPORARY TABLE IF EXISTS tambo_bitacora;
