-- =====================================================================
-- Sistema de Gestion de Tambo
-- Actualizacion de la tabla configuracion
--
-- Solo hace falta si ya se ejecuto tambo_configuracion.sql antes de este
-- incremento. En una base nueva no se corre: tambo_configuracion.sql ya
-- crea la tabla con estas dos columnas.
--
-- Los dos parametros los necesitan las listas de trabajo diarias: las
-- vacas en condiciones de ser servidas y los tactos que le tocan al
-- veterinario.
-- =====================================================================

USE tambo;

ALTER TABLE configuracion
    ADD COLUMN dias_espera_voluntaria INT(11) NOT NULL DEFAULT 45 AFTER dias_anticipacion_vencimiento,
    ADD COLUMN dias_para_tacto        INT(11) NOT NULL DEFAULT 35 AFTER dias_espera_voluntaria;
