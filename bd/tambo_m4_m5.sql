-- =====================================================================
-- Sistema de Gestion de Tambo
-- Script de creacion de la base de datos - Modulos 4 y 5
--
--   Modulo 4: Gestion Sanitaria          (planes sanitarios, vacunaciones,
--             descornes y el calendario que se deriva de ellos)
--   Modulo 5: Control de Insumos y Stock (umbrales, vencimientos por
--             partida e historial de movimientos)
--
-- Las tablas insumos, movimientos_stock, diagnosticos y tratamientos ya
-- las creo tambo_m2_m3.sql: se adelantaron porque los casos de uso de los
-- modulos 2 y 3 las consumen. Este script no las vuelve a crear, agrega
-- lo que faltaba de esos dos modulos y cierra las restricciones que
-- habian quedado pendientes.
--
-- Motor: MySQL
-- Requiere que tambo_m0_m1.sql y tambo_m2_m3.sql ya se hayan ejecutado.
-- =====================================================================

USE tambo;

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

-- ---------------------------------------------------------------------
-- tratamientos: se cierran las dos deudas del Modulo 4
--
-- 1. id_plan ya existia como columna pero sin clave foranea, porque
--    planes_sanitarios se crea recien aca.
--
-- 2. id_animal es nuevo. El modelo vinculaba el tratamiento con el
--    animal unicamente a traves del diagnostico, y el tratamiento
--    preventivo va sin diagnostico (2.2.5.2): asi, la desparasitacion no
--    se podia atribuir a nadie, no generaba descarte de leche para
--    ningun animal y el calendario sanitario nunca encontraba su ultima
--    aplicacion. El curso alternativo 2a de CU20 dice que el preventivo
--    "se registra directamente sobre el animal", que es exactamente esta
--    columna.
--
--    Queda como Null y no como Not null por las filas cargadas antes del
--    Modulo 4: las que venian de un diagnostico se completan con el
--    UPDATE de abajo, pero un preventivo anterior no tiene de donde
--    sacar el animal. De aca en adelante la Controladora siempre la
--    completa.
-- ---------------------------------------------------------------------
ALTER TABLE tratamientos
    ADD FOREIGN KEY (id_plan) REFERENCES planes_sanitarios (id_plan);

ALTER TABLE tratamientos
    ADD COLUMN id_animal INT(11) NULL AFTER dosis_diaria;

UPDATE tratamientos t
    JOIN diagnosticos d ON d.id_diagnostico = t.id_diagnostico
    SET t.id_animal = d.id_animal
    WHERE t.id_animal IS NULL;

ALTER TABLE tratamientos
    ADD FOREIGN KEY (id_animal) REFERENCES animales (id_animal);
