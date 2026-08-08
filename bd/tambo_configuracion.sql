-- =====================================================================
-- Sistema de Gestion de Tambo
-- Script de creacion de la base de datos - Configuracion del establecimiento
--
-- Los parametros de manejo que hasta ahora vivian como constantes en la
-- Controladora. No son hechos biologicos sino decisiones del
-- establecimiento: cuantos dias antes del parto se seca una vaca, a que
-- edad entra la vaquillona en servicio, cuantos orden;es se hacen por dia
-- y con cuanta anticipacion avisa cada alerta. Dos tambos vecinos las
-- resuelven distinto.
--
-- La gestacion, la edad de pubertad y el rango de duracion de una preniez
-- viable NO estan aca: no son decisiones, son biologia, y siguen en el
-- codigo. Lo mismo el periodo de carencia y el stock minimo, que son
-- datos del producto y ya viven en insumos.
--
-- Motor: MySQL
-- Requiere que los scripts anteriores ya se hayan ejecutado.
-- =====================================================================

USE tambo;

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
    PRIMARY KEY (id_configuracion)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- La fila unica, con los valores por defecto de cada columna. Si esta
-- fila no existiera el sistema igual funciona: la Controladora usa las
-- constantes como respaldo.
INSERT INTO configuracion (id_configuracion) VALUES (1);
