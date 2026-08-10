-- =====================================================================
-- Sistema de Gestion de Tambo
-- Actualizacion: foto del animal
--
-- Solo hace falta correr este script si la base ya estaba creada de
-- antes. Una base nueva ya trae la columna, porque tambo_m0_m1.sql la
-- crea junto con el resto de la tabla.
--
-- La imagen no se guarda en la base. Se guarda como archivo dentro de
-- wwwroot/fotos y en esta columna queda unicamente el nombre de ese
-- archivo, que es lo que la pantalla usa para armar la direccion.
-- Un animal sin foto deja la columna en NULL.
-- =====================================================================

USE tambo;

ALTER TABLE animales
    ADD COLUMN foto VARCHAR(120) NULL AFTER motivo_baja;
