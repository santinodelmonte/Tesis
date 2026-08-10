-- =====================================================================
-- Sistema de Gestion de Tambo
-- Datos de prueba - Modulos 1 a 5
--
-- Un rodeo chico pero completo: 21 animales propios, 3 toros de catalogo
-- que solo aportan pajuelas, una semana de ordenies, dos controles
-- lecheros, la reproduccion en marcha y el stock con sus alertas.
--
-- No es un volcado al azar. Cada dato se apoya en el anterior:
--   - las fechas probables de parto son la fecha de servicio + 283 dias;
--   - los estados de la hembra se corresponden con su ultimo tacto;
--   - el numero de partos coincide con la lactancia abierta;
--   - la raza de cada cria sale de la de sus padres;
--   - el stock_actual de cada insumo es exactamente el ingreso menos los
--     egresos que deja cada aplicacion.
-- Asi las pantallas muestran algo que se puede leer, y no numeros sueltos.
--
-- La fecha de referencia es 2026-08-09. Los datos estan armados para que,
-- parados en ese dia, aparezca trabajo pendiente en todos los tableros:
-- una vaca con descarte de leche vigente, dos partos proximos, un tacto
-- atrasado, tres vacas para servicio, un ternero sin descornar, dos
-- insumos bajo el minimo y dos partidas por vencer.
--
-- Requiere que bd/tambo.sql ya haya corrido: usa las razas, las
-- categorias y la fila de configuracion que ese script deja cargadas.
--
--     mysql -u root -p < bd/tambo_datos_prueba.sql
--
-- Se puede volver a correr: empieza vaciando las tablas de datos, sin
-- tocar razas, categorias ni configuracion.
--
-- Motor: MySQL
-- =====================================================================

USE tambo;

-- ---------------------------------------------------------------------
-- Limpieza previa
-- El orden es el inverso al de las dependencias. El SET desactiva la
-- verificacion de claves foraneas solo mientras dura el borrado, porque
-- animales se referencia a si misma a traves de hembras y machos y no hay
-- un orden que resuelva ese ciclo.
-- ---------------------------------------------------------------------
SET FOREIGN_KEY_CHECKS = 0;

TRUNCATE TABLE ordenie_lote_animales;
TRUNCATE TABLE ordenies_individual;
TRUNCATE TABLE ordenies_lote;
TRUNCATE TABLE lactancias;
TRUNCATE TABLE tactos;
TRUNCATE TABLE servicios;
TRUNCATE TABLE celos;
TRUNCATE TABLE partos;
TRUNCATE TABLE descornes;
TRUNCATE TABLE vacunaciones;
TRUNCATE TABLE tratamientos;
TRUNCATE TABLE diagnosticos;
TRUNCATE TABLE plan_categorias;
TRUNCATE TABLE planes_sanitarios;
TRUNCATE TABLE movimientos_stock;
TRUNCATE TABLE insumos;
TRUNCATE TABLE hembras;
TRUNCATE TABLE machos;
TRUNCATE TABLE animales;

SET FOREIGN_KEY_CHECKS = 1;


-- =====================================================================
-- Modulo 1: animales
--
-- Se cargan primero sin madre ni padre y el linaje se completa al final,
-- una vez que existen las filas de hembras y machos a las que las claves
-- foraneas id_madre e id_padre apuntan.
--
-- Los identificadores van explicitos para que el resto del script pueda
-- referenciarlos, y dejan huecos a proposito: 1 a 4 los machos, 10 en
-- adelante el rodeo.
-- =====================================================================

INSERT INTO animales (id_animal, num_caravana, fecha_nacimiento, activo, fecha_baja, motivo_baja, id_raza, id_categoria) VALUES
    -- Toro de servicio del establecimiento
    ( 1, 'T-01',        '2021-09-12', 1, NULL, NULL, 1, 6),

    -- Toros de catalogo. No integran el rodeo: existen como animales
    -- unicamente para que las pajuelas tengan padre y la genealogia de
    -- las crias por inseminacion se pueda reconstruir (en_pie = 0).
    ( 2, '7HO12165',    '2016-04-02', 1, NULL, NULL, 1, 6),
    ( 3, '7JE01722',    '2017-11-20', 1, NULL, NULL, 2, 6),
    ( 4, '29HO18296',   '2018-06-15', 1, NULL, NULL, 1, 6),

    -- Vacas en ordenie
    (10, '101',         '2019-05-18', 1, NULL, NULL, 1, 3),
    (11, '102',         '2019-08-03', 1, NULL, NULL, 1, 3),
    (12, '108',         '2020-02-11', 1, NULL, NULL, 1, 3),
    (13, '115',         '2020-07-27', 1, NULL, NULL, 2, 3),
    (14, '121',         '2021-01-09', 1, NULL, NULL, 4, 3),
    (15, '124',         '2021-03-22', 1, NULL, NULL, 1, 3),
    (16, '130',         '2023-08-19', 1, NULL, NULL, 1, 3),
    (17, '133',         '2022-06-10', 1, NULL, NULL, 3, 3),

    -- Vacas secas, las dos preniadas y proximas a parir
    (18, '136',         '2023-01-15', 1, NULL, NULL, 1, 3),
    (19, '140',         '2023-03-08', 1, NULL, NULL, 2, 3),

    -- Vaquillonas
    (20, '152',         '2024-09-08', 1, NULL, NULL, 1, 2),
    (21, '155',         '2024-11-21', 1, NULL, NULL, 1, 2),
    (22, '158',         '2025-01-30', 1, NULL, NULL, 2, 2),

    -- Terneras y terneros nacidos en el establecimiento
    (23, '171',         '2025-09-14', 1, NULL, NULL, 1, 1),
    (24, '174',         '2026-01-22', 1, NULL, NULL, 1, 1),
    (25, '177',         '2026-04-06', 1, NULL, NULL, 2, 1),
    (26, '175',         '2026-02-11', 1, NULL, NULL, 4, 4),
    (27, '178',         '2026-05-28', 1, NULL, NULL, 1, 4),

    -- Novillo
    (28, '160',         '2025-03-03', 1, NULL, NULL, 5, 5),

    -- Vaca dada de baja. Queda en la base con activo = 0: la baja es
    -- logica y su historia productiva y sanitaria se conserva.
    (29, '112',         '2019-11-30', 0, '2026-03-18', 'Descarte por mastitis cronica reiterada', 1, 3);


-- ---------------------------------------------------------------------
-- hembras
-- estado_productivo y estado_reproductivo no son adorno: son lo que leen
-- las listas de trabajo. Cada uno se corresponde con la lactancia y el
-- ultimo tacto que se cargan mas abajo.
-- ---------------------------------------------------------------------
INSERT INTO hembras (id_animal, numero_partos, estado_productivo, estado_reproductivo) VALUES
    (10, 4, 'En lactancia', 'Preñada'),   -- servida y confirmada preñada
    (11, 4, 'En lactancia', 'Servida'),   -- servida, tacto pendiente
    (12, 3, 'En lactancia', 'Preñada'),
    (13, 3, 'En lactancia', 'Vacía'),     -- volvio en celo tras un tacto negativo
    (14, 2, 'En lactancia', 'Servida'),
    (15, 2, 'En lactancia', 'Preñada'),
    (16, 1, 'En lactancia', 'Vacía'),     -- primeriza, en celo y sin servicio
    (17, 2, 'En lactancia', 'Vacía'),
    (18, 1, 'Seca',         'Preñada'),   -- secada el 25/06, pare el 18/08
    (19, 1, 'Seca',         'Preñada'),   -- secada el 20/07, pare el 10/09
    (20, 0, 'Sin lactancia', 'Preñada'),  -- vaquillona preñada de su primer servicio
    (21, 0, 'Sin lactancia', 'Servida'),
    (22, 0, 'Sin lactancia', 'Vacía'),    -- en celo, entra en edad de servicio
    (23, 0, 'Sin lactancia', 'Vacía'),
    (24, 0, 'Sin lactancia', 'Vacía'),
    (25, 0, 'Sin lactancia', 'Vacía'),
    (29, 5, 'Seca',          'Vacía');    -- dada de baja


-- ---------------------------------------------------------------------
-- machos
-- en_pie distingue al toro que esta fisicamente en el campo de los toros
-- de catalogo, que solo aportan semen, y de los machos que no se destinan
-- a reproduccion.
-- ---------------------------------------------------------------------
INSERT INTO machos (id_animal, en_pie) VALUES
    ( 1, 1),   -- toro de servicio del rodeo
    ( 2, 0),   -- catalogo
    ( 3, 0),   -- catalogo
    ( 4, 0),   -- catalogo
    (26, 0),   -- ternero
    (27, 0),   -- ternero
    (28, 0);   -- novillo


-- ---------------------------------------------------------------------
-- Linaje
-- Recien ahora, con hembras y machos cargados, se pueden completar las
-- claves foraneas recursivas. Las vacas mas viejas quedan sin padres: son
-- las que ya estaban en el campo cuando arranco el sistema.
-- ---------------------------------------------------------------------
UPDATE animales SET id_madre = 10, id_padre = 2 WHERE id_animal = 20;
UPDATE animales SET id_madre = 12, id_padre = 2 WHERE id_animal = 21;
UPDATE animales SET id_madre = 13, id_padre = 3 WHERE id_animal = 22;
UPDATE animales SET id_madre = 11, id_padre = 2 WHERE id_animal = 23;
UPDATE animales SET id_madre = 15, id_padre = 4 WHERE id_animal = 24;
UPDATE animales SET id_madre = 13, id_padre = 3 WHERE id_animal = 25;
UPDATE animales SET id_madre = 14, id_padre = 4 WHERE id_animal = 26;
UPDATE animales SET id_madre = 16, id_padre = 1 WHERE id_animal = 27;
UPDATE animales SET id_madre = 17, id_padre = 4 WHERE id_animal = 28;


-- =====================================================================
-- Modulo 5: insumos
--
-- stock_actual es el resultado exacto de los movimientos que se cargan a
-- continuacion. Dos insumos quedan por debajo de su minimo a proposito
-- -la ivermectina y la pajuela 29HO18296- para que la pantalla de alertas
-- de stock tenga algo que mostrar.
--
-- periodo_descarte_dias es la carencia en leche del producto. Las vacunas
-- y las pajuelas no la tienen, por eso van en nulo.
-- =====================================================================

INSERT INTO insumos (id_insumo, nombre, tipo_insumo, stock_actual, stock_minimo, periodo_descarte_dias, id_macho) VALUES
    ( 1, 'Oxitetraciclina 20% LA (frasco 100 ml)', 'Medicamento',      8.00,  4.00,    7, NULL),
    ( 2, 'Penicilina + Estreptomicina (frasco 100 ml)', 'Medicamento', 12.00,  6.00,    4, NULL),
    ( 3, 'Cefalonio intramamario de secado (jeringa)', 'Medicamento',  24.00, 12.00,   30, NULL),
    ( 4, 'Ivermectina 1% (frasco 500 ml)', 'Antiparasitario',           3.00,  5.00,   28, NULL),
    ( 5, 'Vacuna Aftosa bivalente (dosis)', 'Vacuna',                   8.00,  5.00, NULL, NULL),
    ( 6, 'Vacuna Brucelosis cepa 19 (dosis)', 'Vacuna',                 8.00,  5.00, NULL, NULL),
    ( 7, 'Vacuna reproductiva IBR-BVD-Lepto (dosis)', 'Vacuna',        27.00, 15.00, NULL, NULL),
    ( 8, 'Pajuela Holando 7HO12165', 'Pajuela',                        17.00,  5.00, NULL,    2),
    ( 9, 'Pajuela Jersey 7JE01722', 'Pajuela',                          6.00,  5.00, NULL,    3),
    (10, 'Pajuela Holando 29HO18296', 'Pajuela',                        3.00,  5.00, NULL,    4),
    (11, 'Pasta caustica para descorne (pomo)', 'Medicamento',          3.00,  1.00, NULL, NULL);


-- ---------------------------------------------------------------------
-- movimientos_stock: ingresos por compra
-- El vencimiento se anota en el movimiento y no en el insumo porque un
-- mismo producto entra en partidas distintas. La ivermectina y la vacuna
-- antiaftosa vencen dentro de los proximos 30 dias, que es la
-- anticipacion configurada: son las dos partidas que dispara la alerta de
-- vencimientos.
-- ---------------------------------------------------------------------
INSERT INTO movimientos_stock (tipo_movimiento, cantidad, fecha, fecha_vencimiento, motivo, id_insumo) VALUES
    ('Ingreso', 12.00, '2026-05-12', '2027-11-30', 'Compra a veterinaria La Rural',     1),
    ('Ingreso', 15.00, '2026-04-20', '2027-04-30', 'Compra a veterinaria La Rural',     2),
    -- Partida chica que ya vencio y se dio de baja del inventario
    ('Ingreso',  2.00, '2025-08-14', '2026-07-31', 'Compra a veterinaria La Rural',     2),
    ('Ingreso', 32.00, '2026-06-10', '2028-02-28', 'Compra para el secado del rodeo',   3),
    ('Ingreso', 10.00, '2026-02-15', '2026-09-02', 'Compra a veterinaria La Rural',     4),
    ('Ingreso', 25.00, '2026-03-05', '2026-08-31', 'Campania antiaftosa de otonio',     5),
    ('Ingreso', 10.00, '2026-06-15', '2027-06-30', 'Compra a veterinaria La Rural',     6),
    ('Ingreso', 40.00, '2026-04-10', '2027-03-31', 'Compra a veterinaria La Rural',     7),
    ('Ingreso', 20.00, '2025-10-15', NULL,         'Compra de semen a CIALE',           8),
    ('Ingreso',  7.00, '2025-10-15', NULL,         'Compra de semen a CIALE',           9),
    ('Ingreso',  6.00, '2026-02-20', NULL,         'Compra de semen a CIALE',          10),
    ('Ingreso',  3.00, '2026-01-10', '2028-06-30', 'Compra a veterinaria La Rural',    11),
    -- Baja de la partida vencida
    ('Egreso',   2.00, '2026-08-01', NULL,         'Baja por vencimiento',              2);


-- =====================================================================
-- Modulo 4: planes sanitarios
--
-- El plan es la regla, no la aplicacion. periodicidad_dias en nulo marca
-- el procedimiento que se aplica una sola vez en la vida del animal: la
-- brucelosis y el descorne. El descorne ademas va sin insumo, que es el
-- curso alternativo 4c de CU22.
-- =====================================================================

INSERT INTO planes_sanitarios (id_plan, nombre, tipo_procedimiento, periodicidad_dias, edad_inicio_meses, activo, id_insumo) VALUES
    (1, 'Aftosa',                      'Vacunacion',      180,  3, 1,    5),
    (2, 'Brucelosis cepa 19',          'Vacunacion',     NULL,  3, 1,    6),
    (3, 'Reproductiva IBR-BVD-Lepto',  'Vacunacion',      180, 12, 1,    7),
    (4, 'Desparasitacion',             'Desparasitacion', 120,  2, 1,    4),
    (5, 'Descorne',                    'Descorne',       NULL,  1, 1, NULL);

-- ---------------------------------------------------------------------
-- plan_categorias
-- El plan de aftosa no lleva ninguna fila: eso es lo que lo hace valido
-- para todo el rodeo (CU22, curso alternativo 4a). La ausencia de fila es
-- informacion, no un dato que falte.
-- ---------------------------------------------------------------------
INSERT INTO plan_categorias (id_plan, id_categoria) VALUES
    (2, 1),                      -- brucelosis: solo terneras
    (3, 2), (3, 3),              -- reproductiva: novillas y vacas
    (4, 1), (4, 2), (4, 4), (4, 5),  -- desparasitacion: la recria
    (5, 1), (5, 4);              -- descorne: terneras y terneros


-- =====================================================================
-- Modulo 2: lactancias
--
-- Cada lactancia arranca el dia del parto que la abrio y su numero
-- coincide con el numero_partos de la hembra. La fecha_secado en nulo es
-- lo que identifica a la lactancia en curso: las ocho vacas en ordenie la
-- tienen vacia y las tres cerradas la tienen cargada.
--
-- fecha_probable_parto se completa solo cuando la preniez esta
-- confirmada: es lo que alimenta la alerta de secados y de partos
-- proximos.
-- =====================================================================

INSERT INTO lactancias (id_lactancia, numero_lactancia, fecha_inicio, fecha_secado, fecha_probable_parto, id_animal) VALUES
    ( 1, 4, '2025-11-03', NULL,         '2027-02-27', 10),
    ( 2, 4, '2025-09-14', NULL,         NULL,         11),  -- servida sin confirmar
    ( 3, 3, '2026-03-02', NULL,         '2027-02-21', 12),
    ( 4, 3, '2026-04-06', NULL,         NULL,         13),
    ( 5, 2, '2026-02-11', NULL,         NULL,         14),
    ( 6, 2, '2026-01-22', NULL,         '2027-01-11', 15),
    ( 7, 1, '2026-05-28', NULL,         NULL,         16),
    ( 8, 2, '2026-06-15', NULL,         NULL,         17),
    -- Cerradas: las dos vacas secas y la vaca dada de baja
    ( 9, 1, '2025-07-10', '2026-06-25', '2026-08-18', 18),
    (10, 1, '2025-08-22', '2026-07-20', '2026-09-10', 19),
    (11, 5, '2025-06-05', '2026-03-18', NULL,         29);


-- ---------------------------------------------------------------------
-- ordenies_lote
-- Una semana de ordenies, dos turnos por dia, tal como se leen del
-- tanque. Los totales caen unos veintidos litros a partir del 07/08:
-- ese es el dia en que la vaca 115 sale del lote por el descarte de leche
-- de su tratamiento contra la mastitis. La produccion tiene que poder
-- explicarse por lo que pasa en el rodeo.
-- ---------------------------------------------------------------------
INSERT INTO ordenies_lote (id_ordenie_lote, fecha, turno, litros_totales) VALUES
    ( 1, '2026-08-03', 'Turno 1',  97.10),
    ( 2, '2026-08-03', 'Turno 2',  72.40),
    ( 3, '2026-08-04', 'Turno 1',  99.20),
    ( 4, '2026-08-04', 'Turno 2',  74.60),
    -- Dia de control lechero: los litros del lote son la suma exacta de
    -- los controles individuales que se cargan mas abajo.
    ( 5, '2026-08-05', 'Turno 1',  98.30),
    ( 6, '2026-08-05', 'Turno 2',  73.90),
    ( 7, '2026-08-06', 'Turno 1',  96.80),
    ( 8, '2026-08-06', 'Turno 2',  71.90),
    -- Desde aca, siete vacas en el lote
    ( 9, '2026-08-07', 'Turno 1',  87.20),
    (10, '2026-08-07', 'Turno 2',  65.40),
    (11, '2026-08-08', 'Turno 1',  85.90),
    (12, '2026-08-08', 'Turno 2',  64.10),
    (13, '2026-08-09', 'Turno 1',  88.10),
    (14, '2026-08-09', 'Turno 2',  66.30);

-- ---------------------------------------------------------------------
-- ordenie_lote_animales
-- Que vacas integraron el lote de cada turno. Se genera cruzando los
-- catorce ordenies con las ocho vacas en lactancia, y se excluye a la
-- 115 (id 13) desde el 07/08: su descarte de leche esta vigente y el
-- paso 3 de CU8 la deja afuera del lote.
-- ---------------------------------------------------------------------
INSERT INTO ordenie_lote_animales (id_ordenie_lote, id_animal)
SELECT ol.id_ordenie_lote, v.id_animal
FROM ordenies_lote ol
CROSS JOIN (
    SELECT 10 AS id_animal UNION ALL SELECT 11 UNION ALL SELECT 12 UNION ALL
    SELECT 13 UNION ALL SELECT 14 UNION ALL SELECT 15 UNION ALL
    SELECT 16 UNION ALL SELECT 17
) v
WHERE NOT (v.id_animal = 13 AND ol.fecha >= '2026-08-07');

-- ---------------------------------------------------------------------
-- ordenies_individual
--
-- Dos controles lecheros mensuales. Son los mismos ordenies del turno
-- anotados vaca por vaca, no una fuente de leche aparte.
--
--   06/07: sin lote cargado. id_ordenie_lote va en nulo y la produccion
--          de esos dos turnos es la suma de estos controles. Es el caso
--          que describe el desvio D1 de docs/desvios-modulos-2-y-3.md.
--   05/08: con lote cargado. Los controles apuntan a los ordenies 5 y 6,
--          y sus litros ya estan dentro de los litros_totales de esos dos
--          registros: sumar las dos cosas contaria la leche dos veces.
--
-- Los litros siguen la curva de lactancia de cada vaca: la 108 y la 124
-- estan en el pico, la 102 lleva once meses en leche y ya bajo, la 133
-- recien empieza a subir.
-- ---------------------------------------------------------------------
INSERT INTO ordenies_individual (fecha, turno, litros, id_animal, id_lactancia, id_ordenie_lote) VALUES
    -- Control del 06/07, sin ordenie de lote asociado
    ('2026-07-06', 'Turno 1', 11.20, 10,  1, NULL),
    ('2026-07-06', 'Turno 2',  8.40, 10,  1, NULL),
    ('2026-07-06', 'Turno 1',  9.00, 11,  2, NULL),
    ('2026-07-06', 'Turno 2',  6.80, 11,  2, NULL),
    ('2026-07-06', 'Turno 1', 16.00, 12,  3, NULL),
    ('2026-07-06', 'Turno 2', 12.10, 12,  3, NULL),
    ('2026-07-06', 'Turno 1', 13.30, 13,  4, NULL),
    ('2026-07-06', 'Turno 2', 10.10, 13,  4, NULL),
    ('2026-07-06', 'Turno 1', 14.30, 14,  5, NULL),
    ('2026-07-06', 'Turno 2', 10.70, 14,  5, NULL),
    ('2026-07-06', 'Turno 1', 15.00, 15,  6, NULL),
    ('2026-07-06', 'Turno 2', 11.20, 15,  6, NULL),
    ('2026-07-06', 'Turno 1', 13.80, 16,  7, NULL),
    ('2026-07-06', 'Turno 2', 10.30, 16,  7, NULL),
    ('2026-07-06', 'Turno 1', 12.30, 17,  8, NULL),
    ('2026-07-06', 'Turno 2',  9.20, 17,  8, NULL),

    -- Control del 05/08, dentro de los ordenies de lote 5 y 6
    ('2026-08-05', 'Turno 1', 10.20, 10,  1,  5),
    ('2026-08-05', 'Turno 2',  7.80, 10,  1,  6),
    ('2026-08-05', 'Turno 1',  8.30, 11,  2,  5),
    ('2026-08-05', 'Turno 2',  6.20, 11,  2,  6),
    ('2026-08-05', 'Turno 1', 15.10, 12,  3,  5),
    ('2026-08-05', 'Turno 2', 11.30, 12,  3,  6),
    ('2026-08-05', 'Turno 1', 12.40, 13,  4,  5),
    ('2026-08-05', 'Turno 2',  9.40, 13,  4,  6),
    ('2026-08-05', 'Turno 1', 13.50, 14,  5,  5),
    ('2026-08-05', 'Turno 2', 10.10, 14,  5,  6),
    ('2026-08-05', 'Turno 1', 14.20, 15,  6,  5),
    ('2026-08-05', 'Turno 2', 10.70, 15,  6,  6),
    ('2026-08-05', 'Turno 1', 11.60, 16,  7,  5),
    ('2026-08-05', 'Turno 2',  8.70, 16,  7,  6),
    ('2026-08-05', 'Turno 1', 13.00, 17,  8,  5),
    ('2026-08-05', 'Turno 2',  9.70, 17,  8,  6);


-- =====================================================================
-- Modulo 3: reproduccion
-- =====================================================================

-- ---------------------------------------------------------------------
-- partos
-- Los once partos que ocurrieron desde que el sistema esta en uso. Los
-- anteriores no estan cargados: viven en el numero_partos de la hembra.
-- La cria no se referencia desde aca, se vincula por animales.id_madre.
-- ---------------------------------------------------------------------
INSERT INTO partos (id_parto, fecha_parto, tipo_parto, observaciones, id_animal) VALUES
    ( 1, '2025-06-05', 'Normal',    'Cria vendida al pie.',                          29),
    ( 2, '2025-07-10', 'Normal',    'Cria vendida al pie.',                          18),
    ( 3, '2025-08-22', 'Normal',    'Cria vendida al pie.',                          19),
    ( 4, '2025-09-14', 'Normal',    NULL,                                            11),  -- cria: 171
    ( 5, '2025-11-03', 'Normal',    'Cria vendida al pie.',                          10),
    ( 6, '2026-01-22', 'Normal',    NULL,                                            15),  -- cria: 174
    ( 7, '2026-02-11', 'Distócico', 'Requirio asistencia. Ternero de 48 kg.',        14),  -- cria: 175
    ( 8, '2026-03-02', 'Normal',    'Cria vendida al pie.',                          12),
    ( 9, '2026-04-06', 'Normal',    NULL,                                            13),  -- cria: 177
    (10, '2026-05-28', 'Normal',    NULL,                                            16),  -- cria: 178
    (11, '2026-06-15', 'Distócico', 'Cria nacida sin vida. Se llamo al veterinario.', 17);

-- ---------------------------------------------------------------------
-- celos
-- Los celos previos a cada servicio, mas los tres de las hembras que hoy
-- estan vacias y esperando ser servidas.
-- ---------------------------------------------------------------------
INSERT INTO celos (fecha, observaciones, id_animal) VALUES
    ('2026-03-23', 'Primer celo detectado. Entra en edad de servicio.', 20),
    ('2026-04-02', NULL,                                                15),
    ('2026-05-13', 'Monta a otras vacas del lote.',                     12),
    ('2026-05-18', NULL,                                                10),
    ('2026-05-23', 'Celo firme, mucosa evidente.',                      13),
    ('2026-06-18', NULL,                                                11),
    ('2026-07-17', NULL,                                                14),
    ('2026-07-26', 'Detectado en el ordenie de la tarde.',              21),
    ('2026-07-29', 'Repite celo, el tacto del 01/07 dio vacia.',        13),
    ('2026-08-02', 'Celo corto, dudoso.',                               16),
    ('2026-08-04', NULL,                                                22),
    ('2026-08-08', NULL,                                                17);

-- ---------------------------------------------------------------------
-- servicios
-- fecha_probable_parto es siempre fecha_servicio + 283 dias.
-- id_macho e id_insumo son excluyentes: el toro de la monta natural o la
-- pajuela de la inseminacion, nunca los dos.
--
-- La vaquillona 152 es hija del toro 7HO12165 y por eso se la insemina
-- con 29HO18296: servirla con semen de su propio padre seria un
-- apareamiento consanguineo.
-- ---------------------------------------------------------------------
INSERT INTO servicios (id_servicio, tipo_servicio, fecha_servicio, fecha_probable_parto, observaciones, id_animal, id_macho, id_insumo) VALUES
    ( 1, 'Inseminación artificial', '2025-11-08', '2026-08-18', NULL,                              18, NULL,    8),
    ( 2, 'Inseminación artificial', '2025-12-01', '2026-09-10', NULL,                              19, NULL,    9),
    ( 3, 'Inseminación artificial', '2026-03-25', '2027-01-02', 'Primer servicio de la vaquillona.', 20, NULL,  10),
    ( 4, 'Inseminación artificial', '2026-04-03', '2027-01-11', NULL,                              15, NULL,   10),
    ( 5, 'Inseminación artificial', '2026-05-14', '2027-02-21', NULL,                              12, NULL,    8),
    ( 6, 'Inseminación artificial', '2026-05-20', '2027-02-27', NULL,                              10, NULL,    8),
    ( 7, 'Inseminación artificial', '2026-05-25', '2027-03-04', NULL,                              13, NULL,   10),
    ( 8, 'Monta natural',           '2026-06-20', '2027-03-30', 'Servida por el toro T-01.',        11,    1, NULL),
    ( 9, 'Monta natural',           '2026-07-18', '2027-04-27', NULL,                              14,    1, NULL),
    (10, 'Monta natural',           '2026-07-28', '2027-05-07', 'Primer servicio de la vaquillona.', 21,   1, NULL);

-- Egreso de las pajuelas. Una por inseminacion, con el mismo motivo que
-- escribe pServicio al registrar el servicio.
INSERT INTO movimientos_stock (tipo_movimiento, cantidad, fecha, fecha_vencimiento, motivo, id_insumo)
SELECT 'Egreso', 1, s.fecha_servicio, NULL,
       CONCAT('Inseminación de la caravana ', a.num_caravana), s.id_insumo
FROM servicios s
JOIN animales a ON a.id_animal = s.id_animal
WHERE s.id_insumo IS NOT NULL;

-- ---------------------------------------------------------------------
-- tactos
-- Cuelgan del servicio, no de la hembra: es el servicio lo que el tacto
-- viene a confirmar.
--
-- El servicio 8 quedo sin tactar y ya pasaron cincuenta dias: aparece en
-- la lista de tactos pendientes. Los servicios 9 y 10 son mas recientes
-- que los 35 dias configurados y todavia no corresponde tactarlos.
-- ---------------------------------------------------------------------
INSERT INTO tactos (fecha_tacto, resultado, observaciones, id_servicio) VALUES
    ('2025-12-15', 'Preñada', NULL,                                       1),
    ('2026-01-08', 'Dudosa',  'Se repite el tacto en cuatro semanas.',    2),
    ('2026-02-05', 'Preñada', 'Confirmada en el segundo tacto.',          2),
    ('2026-05-02', 'Preñada', NULL,                                       3),
    ('2026-05-11', 'Preñada', NULL,                                       4),
    ('2026-06-19', 'Preñada', NULL,                                       5),
    ('2026-06-25', 'Preñada', NULL,                                       6),
    ('2026-07-01', 'Vacía',   'Vuelve a servicio en el proximo celo.',    7);


-- =====================================================================
-- Modulo 4: sanidad
-- =====================================================================

-- ---------------------------------------------------------------------
-- diagnosticos
-- ---------------------------------------------------------------------
INSERT INTO diagnosticos (id_diagnostico, fecha_diagnostico, enfermedad, estado, id_animal) VALUES
    (1, '2026-06-26', 'Metritis puerperal',                        'Resuelto',       16),
    (2, '2026-06-28', 'Dermatitis digital (cojera)',               'Resuelto',       11),
    (3, '2026-08-06', 'Mastitis clinica, cuarto anterior izquierdo','En tratamiento', 13);

-- ---------------------------------------------------------------------
-- tratamientos
--
-- fecha_fin_descarte es fecha_inicio + dias_duracion + la carencia del
-- insumo, que es lo que calcula Controladora.CalcularDescarte.
--
-- Los dos preventivos -desparasitacion y secado- van sin diagnostico: ese
-- nulo es justamente lo que los identifica como preventivos. id_animal
-- esta siempre cargado, porque sin el no habria descarte de leche ni
-- forma de saber cuando se aplico el plan por ultima vez.
--
-- El unico descarte vigente al 09/08 es el de la vaca 115: por eso queda
-- fuera del lote de ordenie desde el 07/08.
-- ---------------------------------------------------------------------
INSERT INTO tratamientos (id_tratamiento, fecha_inicio, dias_duracion, dosis_diaria, id_animal, fecha_fin_descarte, id_diagnostico, id_insumo, id_plan) VALUES
    -- Desparasitaciones de la recria, por plan sanitario
    ( 1, '2026-03-20', 1, '1 ml cada 50 kg, subcutanea', 20, '2026-04-18', NULL,  4, 4),
    ( 2, '2026-03-20', 1, '1 ml cada 50 kg, subcutanea', 21, '2026-04-18', NULL,  4, 4),
    ( 3, '2026-03-20', 1, '1 ml cada 50 kg, subcutanea', 22, '2026-04-18', NULL,  4, 4),
    ( 4, '2026-03-20', 1, '1 ml cada 50 kg, subcutanea', 23, '2026-04-18', NULL,  4, 4),
    ( 5, '2026-07-05', 1, '1 ml cada 50 kg, subcutanea', 24, '2026-08-03', NULL,  4, 4),
    ( 6, '2026-07-05', 1, '1 ml cada 50 kg, subcutanea', 25, '2026-08-03', NULL,  4, 4),
    ( 7, '2026-07-05', 1, '1 ml cada 50 kg, subcutanea', 26, '2026-08-03', NULL,  4, 4),
    -- Curativos
    ( 8, '2026-06-28', 4, '15 ml intramuscular cada 24 h', 16, '2026-07-06',   1,  2, NULL),
    ( 9, '2026-06-28', 1, 'Banio de pezuñas y 10 ml IM',   11, '2026-07-06',   2,  1, NULL),
    (10, '2026-08-07', 3, '20 ml intramuscular cada 24 h', 13, '2026-08-17',   3,  1, NULL),
    -- Secado: una jeringa por cuarto, aplicacion unica
    (11, '2026-06-25', 1, '1 jeringa intramamaria por cuarto', 18, '2026-07-26', NULL, 3, NULL),
    (12, '2026-07-20', 1, '1 jeringa intramamaria por cuarto', 19, '2026-08-20', NULL, 3, NULL);

-- Egreso del producto que consumio cada tratamiento, con el mismo motivo
-- que escribe pTratamiento. Las cantidades no son uniformes: el secado
-- gasta cuatro jeringas y el resto, entre uno y tres frascos.
INSERT INTO movimientos_stock (tipo_movimiento, cantidad, fecha, fecha_vencimiento, motivo, id_insumo) VALUES
    ('Egreso', 1.00, '2026-03-20', NULL, 'Tratamiento sanitario',  4),
    ('Egreso', 1.00, '2026-03-20', NULL, 'Tratamiento sanitario',  4),
    ('Egreso', 1.00, '2026-03-20', NULL, 'Tratamiento sanitario',  4),
    ('Egreso', 1.00, '2026-03-20', NULL, 'Tratamiento sanitario',  4),
    ('Egreso', 1.00, '2026-07-05', NULL, 'Tratamiento sanitario',  4),
    ('Egreso', 1.00, '2026-07-05', NULL, 'Tratamiento sanitario',  4),
    ('Egreso', 1.00, '2026-07-05', NULL, 'Tratamiento sanitario',  4),
    ('Egreso', 3.00, '2026-06-28', NULL, 'Tratamiento sanitario',  2),
    ('Egreso', 1.00, '2026-06-28', NULL, 'Tratamiento sanitario',  1),
    ('Egreso', 3.00, '2026-08-07', NULL, 'Tratamiento sanitario',  1),
    ('Egreso', 4.00, '2026-06-25', NULL, 'Tratamiento sanitario',  3),
    ('Egreso', 4.00, '2026-07-20', NULL, 'Tratamiento sanitario',  3);

-- ---------------------------------------------------------------------
-- vacunaciones
-- Se generan por consulta y no una por una: asi la campania alcanza
-- exactamente a los animales que el plan define, y no a los que uno se
-- acuerde de escribir.
-- ---------------------------------------------------------------------

-- Aftosa, campania de otonio. Alcanza a todo el rodeo mayor de tres
-- meses. Los tres toros de catalogo quedan afuera porque no estan en el
-- campo: existen en la base solo para dar padre a las pajuelas.
INSERT INTO vacunaciones (fecha_aplicacion, id_animal, id_insumo, id_plan)
SELECT '2026-03-09', id_animal, 5, 1
FROM animales
WHERE id_animal NOT IN (2, 3, 4)
  AND fecha_nacimiento <= '2025-12-09';

-- Reproductiva, a las hembras en edad reproductiva que estaban activas.
INSERT INTO vacunaciones (fecha_aplicacion, id_animal, id_insumo, id_plan)
SELECT '2026-04-15', a.id_animal, 7, 3
FROM animales a
JOIN hembras h ON h.id_animal = a.id_animal
WHERE a.id_categoria IN (2, 3)
  AND a.activo = 1;

-- Brucelosis, unica en la vida y solo a las terneras que ya tenian tres
-- meses. La 177 nacio en abril y todavia no llega: queda pendiente en el
-- calendario sanitario.
INSERT INTO vacunaciones (fecha_aplicacion, id_animal, id_insumo, id_plan) VALUES
    ('2026-06-20', 23, 6, 2),
    ('2026-06-20', 24, 6, 2);

-- Un egreso de una dosis por vacunacion aplicada, con el mismo motivo que
-- escribe pVacunacion.
INSERT INTO movimientos_stock (tipo_movimiento, cantidad, fecha, fecha_vencimiento, motivo, id_insumo)
SELECT 'Egreso', 1, fecha_aplicacion, NULL, 'Vacunacion', id_insumo
FROM vacunaciones;

-- ---------------------------------------------------------------------
-- descornes
-- De aplicacion unica: una vez registrado, el plan deja de exigirlo para
-- ese animal. El ternero 178 nacio en mayo y no esta descornado todavia:
-- es el pendiente que muestra el calendario sanitario.
-- ---------------------------------------------------------------------
INSERT INTO descornes (fecha, metodo, observaciones, id_animal, id_plan) VALUES
    ('2025-09-28', 'Pasta caustica', 'A los catorce dias de vida.', 23, 5),
    ('2026-02-05', 'Pasta caustica', NULL,                          24, 5),
    ('2026-02-25', 'Pasta caustica', NULL,                          26, 5),
    ('2026-04-20', 'Termocauterio',  'Se agoto la pasta.',          25, 5);


-- =====================================================================
-- Verificacion
--
-- Las dos consultas comprueban lo unico que este script no puede
-- garantizar escribiendo los datos a mano: que el stock declarado en
-- insumos coincida con el saldo de sus movimientos, y que los litros de
-- los controles individuales del 05/08 den exactamente el total de los
-- ordenies de lote de ese dia. Ambas tienen que devolver cero filas.
-- =====================================================================

SELECT i.id_insumo, i.nombre, i.stock_actual, COALESCE(SUM(
           CASE WHEN m.tipo_movimiento = 'Ingreso' THEN m.cantidad ELSE -m.cantidad END), 0) AS saldo
FROM insumos i
LEFT JOIN movimientos_stock m ON m.id_insumo = i.id_insumo
GROUP BY i.id_insumo, i.nombre, i.stock_actual
HAVING i.stock_actual <> saldo;

SELECT ol.fecha, ol.turno, ol.litros_totales, SUM(oi.litros) AS suma_individual
FROM ordenies_lote ol
JOIN ordenies_individual oi ON oi.id_ordenie_lote = ol.id_ordenie_lote
GROUP BY ol.id_ordenie_lote, ol.fecha, ol.turno, ol.litros_totales
HAVING ol.litros_totales <> SUM(oi.litros);
