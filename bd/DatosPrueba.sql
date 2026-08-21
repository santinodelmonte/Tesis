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
-- El juego de datos NO tiene fechas fijas: todas se calculan contra @hoy,
-- que por defecto es el dia en que se corre el script. Asi el rodeo se
-- lee igual hoy que dentro de tres meses, y las listas de trabajo nunca
-- aparecen vacias por haber quedado viejas.
--
-- Parado en el dia de la carga tiene que haber trabajo pendiente en todos
-- los tableros: una vaca con descarte de leche vigente, un parto proximo,
-- un tacto atrasado, cuatro vacas para servicio, una ternera sin vacunar,
-- un ternero sin descornar, dos insumos bajo el minimo y dos partidas por
-- vencer.
--
-- El ultimo ordenie cargado es el de anteayer: el de ayer y el de hoy son
-- los que se cargan a mano al recorrer el sistema.
--
-- Requiere que bd/CreacionDb.sql ya haya corrido: usa las razas, las
-- categorias y la fila de configuracion que ese script deja cargadas.
--
--     mysql -u root -p < bd/DatosPrueba.sql
--
-- Se puede volver a correr: empieza vaciando las tablas de datos, sin
-- tocar razas, categorias ni configuracion.
--
-- Motor: MySQL
-- =====================================================================

-- El juego de caracteres de la conexion. Sin esto, el cliente de linea de
-- comandos interpreta el archivo con el charset que tenga configurado -en
-- Windows suele ser el de la consola- y las enies y las tildes llegan al
-- servidor doble codificadas: 'Preñada' se guarda como 'Pre├▒ada' y despues
-- ninguna comparacion contra la constante del dominio coincide. El archivo
-- esta en UTF-8 y aca se lo declara.
SET NAMES utf8mb4;

USE tambo;

-- ---------------------------------------------------------------------
-- El ancla del juego de datos. Todas las fechas de abajo se escriben como
-- @hoy mas o menos una cantidad de dias, de manera que el rodeo entero se
-- corre solo al dia en que se carga.
--
-- Para reproducir una situacion puntual -o para volver a ver una pantalla
-- tal como estaba en una fecha determinada- alcanza con fijar el ancla a
-- mano:
--
--     SET @hoy = DATE('2026-08-19');
--
-- Con SET @hoy = DATE('2026-08-11') el juego de datos queda identico al
-- que tenia fechas fijas, que es el que describe docs/flujos-de-prueba.md.
-- ---------------------------------------------------------------------
SET @hoy = CURDATE();

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
    ( 1, 'T-01',        @hoy - INTERVAL 1794 DAY, 1, NULL, NULL, 1, 6),

    -- Toros de catalogo. No integran el rodeo: existen como animales
    -- unicamente para que las pajuelas tengan padre y la genealogia de
    -- las crias por inseminacion se pueda reconstruir (en_pie = 0).
    ( 2, '7HO12165',    @hoy - INTERVAL 3783 DAY, 1, NULL, NULL, 1, 6),
    ( 3, '7JE01722',    @hoy - INTERVAL 3186 DAY, 1, NULL, NULL, 2, 6),
    ( 4, '29HO18296',   @hoy - INTERVAL 2979 DAY, 1, NULL, NULL, 1, 6),

    -- Vacas en ordenie
    (10, '101',         @hoy - INTERVAL 2642 DAY, 1, NULL, NULL, 1, 3),
    (11, '102',         @hoy - INTERVAL 2565 DAY, 1, NULL, NULL, 1, 3),
    (12, '108',         @hoy - INTERVAL 2373 DAY, 1, NULL, NULL, 1, 3),
    (13, '115',         @hoy - INTERVAL 2206 DAY, 1, NULL, NULL, 2, 3),
    (14, '121',         @hoy - INTERVAL 2040 DAY, 1, NULL, NULL, 4, 3),
    (15, '124',         @hoy - INTERVAL 1968 DAY, 1, NULL, NULL, 1, 3),
    (16, '130',         @hoy - INTERVAL 1088 DAY, 1, NULL, NULL, 1, 3),
    (17, '133',         @hoy - INTERVAL 1523 DAY, 1, NULL, NULL, 3, 3),

    -- Vacas secas, las dos preniadas y proximas a parir
    (18, '136',         @hoy - INTERVAL 1304 DAY, 1, NULL, NULL, 1, 3),
    (19, '140',         @hoy - INTERVAL 1252 DAY, 1, NULL, NULL, 2, 3),

    -- Vaquillonas
    (20, '152',         @hoy - INTERVAL 702 DAY , 1, NULL, NULL, 1, 2),
    (21, '155',         @hoy - INTERVAL 628 DAY , 1, NULL, NULL, 1, 2),
    (22, '158',         @hoy - INTERVAL 558 DAY , 1, NULL, NULL, 2, 2),

    -- Terneras y terneros nacidos en el establecimiento
    (23, '171',         @hoy - INTERVAL 331 DAY , 1, NULL, NULL, 1, 1),
    (24, '174',         @hoy - INTERVAL 201 DAY , 1, NULL, NULL, 1, 1),
    (25, '177',         @hoy - INTERVAL 127 DAY , 1, NULL, NULL, 2, 1),
    (26, '175',         @hoy - INTERVAL 181 DAY , 1, NULL, NULL, 4, 4),
    (27, '178',         @hoy - INTERVAL 75 DAY  , 1, NULL, NULL, 1, 4),

    -- Novillo
    (28, '160',         @hoy - INTERVAL 526 DAY , 1, NULL, NULL, 5, 5),

    -- Vaca dada de baja. Queda en la base con activo = 0: la baja es
    -- logica y su historia productiva y sanitaria se conserva.
    (29, '112',         @hoy - INTERVAL 2446 DAY, 0, @hoy - INTERVAL 146 DAY , 'Descarte por mastitis cronica reiterada', 1, 3);


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
    (18, 1, 'Seca',         'Preñada'),   -- secada hace 47 dias, pare en 7 dias
    (19, 1, 'Seca',         'Preñada'),   -- secada hace 22 dias, pare en 30 dias
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
    ('Ingreso', 12.00, @hoy - INTERVAL 91 DAY  , @hoy + INTERVAL 476 DAY , 'Compra a veterinaria La Rural',     1),
    ('Ingreso', 15.00, @hoy - INTERVAL 113 DAY , @hoy + INTERVAL 262 DAY , 'Compra a veterinaria La Rural',     2),
    -- Partida chica que ya vencio y se dio de baja del inventario
    ('Ingreso',  2.00, @hoy - INTERVAL 362 DAY , @hoy - INTERVAL 11 DAY  , 'Compra a veterinaria La Rural',     2),
    ('Ingreso', 32.00, @hoy - INTERVAL 62 DAY  , @hoy + INTERVAL 566 DAY , 'Compra para el secado del rodeo',   3),
    ('Ingreso', 10.00, @hoy - INTERVAL 177 DAY , @hoy + INTERVAL 22 DAY  , 'Compra a veterinaria La Rural',     4),
    ('Ingreso', 25.00, @hoy - INTERVAL 159 DAY , @hoy + INTERVAL 20 DAY  , 'Campania antiaftosa de otonio',     5),
    ('Ingreso', 10.00, @hoy - INTERVAL 57 DAY  , @hoy + INTERVAL 323 DAY , 'Compra a veterinaria La Rural',     6),
    ('Ingreso', 40.00, @hoy - INTERVAL 123 DAY , @hoy + INTERVAL 232 DAY , 'Compra a veterinaria La Rural',     7),
    ('Ingreso', 20.00, @hoy - INTERVAL 300 DAY , NULL,         'Compra de semen a CIALE',           8),
    ('Ingreso',  7.00, @hoy - INTERVAL 300 DAY , NULL,         'Compra de semen a CIALE',           9),
    ('Ingreso',  6.00, @hoy - INTERVAL 172 DAY , NULL,         'Compra de semen a CIALE',          10),
    ('Ingreso',  3.00, @hoy - INTERVAL 213 DAY , @hoy + INTERVAL 689 DAY , 'Compra a veterinaria La Rural',    11),
    -- Baja de la partida vencida
    ('Egreso',   2.00, @hoy - INTERVAL 10 DAY  , NULL,         'Baja por vencimiento',              2);


-- =====================================================================
-- Modulo 4: planes sanitarios
--
-- El plan es la regla, no la aplicacion. periodicidad_dias en nulo marca
-- el procedimiento que se aplica una sola vez en la vida del animal: la
-- brucelosis y el descorne. El descorne ademas va sin insumo, que es el
-- curso alternativo 4c de CU30.
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
-- para todo el rodeo (CU30, curso alternativo 4a). La ausencia de fila es
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
    ( 1, 4, @hoy - INTERVAL 281 DAY , NULL,         @hoy + INTERVAL 200 DAY , 10),
    ( 2, 4, @hoy - INTERVAL 331 DAY , NULL,         NULL,         11),  -- servida sin confirmar
    ( 3, 3, @hoy - INTERVAL 162 DAY , NULL,         @hoy + INTERVAL 194 DAY , 12),
    ( 4, 3, @hoy - INTERVAL 127 DAY , NULL,         NULL,         13),
    ( 5, 2, @hoy - INTERVAL 181 DAY , NULL,         NULL,         14),
    ( 6, 2, @hoy - INTERVAL 201 DAY , NULL,         @hoy + INTERVAL 153 DAY , 15),
    ( 7, 1, @hoy - INTERVAL 75 DAY  , NULL,         NULL,         16),
    ( 8, 2, @hoy - INTERVAL 57 DAY  , NULL,         NULL,         17),
    -- Cerradas: las dos vacas secas y la vaca dada de baja
    ( 9, 1, @hoy - INTERVAL 397 DAY , @hoy - INTERVAL 47 DAY  , @hoy + INTERVAL 7 DAY   , 18),
    (10, 1, @hoy - INTERVAL 354 DAY , @hoy - INTERVAL 22 DAY  , @hoy + INTERVAL 30 DAY  , 19),
    (11, 5, @hoy - INTERVAL 432 DAY , @hoy - INTERVAL 146 DAY , NULL,         29);


-- ---------------------------------------------------------------------
-- ordenies_lote
-- Una semana de ordenies, dos turnos por dia, tal como se leen del
-- tanque. Los totales caen unos veintidos litros en los ultimos cuatro dias:
-- ese es el dia en que la vaca 115 sale del lote por el descarte de leche
-- de su tratamiento contra la mastitis. La produccion tiene que poder
-- explicarse por lo que pasa en el rodeo.
-- ---------------------------------------------------------------------
INSERT INTO ordenies_lote (id_ordenie_lote, fecha, turno, litros_totales) VALUES
    ( 1, @hoy - INTERVAL 8 DAY   , 'Turno 1',  97.10),
    ( 2, @hoy - INTERVAL 8 DAY   , 'Turno 2',  72.40),
    ( 3, @hoy - INTERVAL 7 DAY   , 'Turno 1',  99.20),
    ( 4, @hoy - INTERVAL 7 DAY   , 'Turno 2',  74.60),
    -- Dia de control lechero: los litros del lote son la suma exacta de
    -- los controles individuales que se cargan mas abajo.
    ( 5, @hoy - INTERVAL 6 DAY   , 'Turno 1',  98.30),
    ( 6, @hoy - INTERVAL 6 DAY   , 'Turno 2',  73.90),
    ( 7, @hoy - INTERVAL 5 DAY   , 'Turno 1',  96.80),
    ( 8, @hoy - INTERVAL 5 DAY   , 'Turno 2',  71.90),
    -- Desde aca, siete vacas en el lote
    ( 9, @hoy - INTERVAL 4 DAY   , 'Turno 1',  87.20),
    (10, @hoy - INTERVAL 4 DAY   , 'Turno 2',  65.40),
    (11, @hoy - INTERVAL 3 DAY   , 'Turno 1',  85.90),
    (12, @hoy - INTERVAL 3 DAY   , 'Turno 2',  64.10),
    (13, @hoy - INTERVAL 2 DAY   , 'Turno 1',  88.10),
    (14, @hoy - INTERVAL 2 DAY   , 'Turno 2',  66.30);

-- ---------------------------------------------------------------------
-- ordenie_lote_animales
-- Que vacas integraron el lote de cada turno. Se genera cruzando los
-- catorce ordenies con las ocho vacas en lactancia, y se excluye a la
-- 115 (id 13) en los ultimos cuatro dias: su descarte de leche esta
-- vigente y el
-- paso 3 de CU12 la deja afuera del lote.
-- ---------------------------------------------------------------------
INSERT INTO ordenie_lote_animales (id_ordenie_lote, id_animal)
SELECT ol.id_ordenie_lote, v.id_animal
FROM ordenies_lote ol
CROSS JOIN (
    SELECT 10 AS id_animal UNION ALL SELECT 11 UNION ALL SELECT 12 UNION ALL
    SELECT 13 UNION ALL SELECT 14 UNION ALL SELECT 15 UNION ALL
    SELECT 16 UNION ALL SELECT 17
) v
WHERE NOT (v.id_animal = 13 AND ol.fecha >= @hoy - INTERVAL 4 DAY   );

-- ---------------------------------------------------------------------
-- ordenies_individual
--
-- Dos controles lecheros mensuales. Son los mismos ordenies del turno
-- anotados vaca por vaca, no una fuente de leche aparte.
--
--   hace 36 dias: sin lote cargado. id_ordenie_lote va en nulo y la
--          produccion de esos dos turnos es la suma de estos controles.
--   hace 6 dias: con lote cargado. Los controles apuntan a los 5 y 6,
--          y sus litros ya estan dentro de los litros_totales de esos dos
--          registros: sumar las dos cosas contaria la leche dos veces.
--
-- Los litros siguen la curva de lactancia de cada vaca: la 108 y la 124
-- estan en el pico, la 102 lleva once meses en leche y ya bajo, la 133
-- recien empieza a subir.
-- ---------------------------------------------------------------------
INSERT INTO ordenies_individual (fecha, turno, litros, id_animal, id_lactancia, id_ordenie_lote) VALUES
    -- Control de hace 36 dias, sin ordenie de lote asociado
    (@hoy - INTERVAL 36 DAY  , 'Turno 1', 11.20, 10,  1, NULL),
    (@hoy - INTERVAL 36 DAY  , 'Turno 2',  8.40, 10,  1, NULL),
    (@hoy - INTERVAL 36 DAY  , 'Turno 1',  9.00, 11,  2, NULL),
    (@hoy - INTERVAL 36 DAY  , 'Turno 2',  6.80, 11,  2, NULL),
    (@hoy - INTERVAL 36 DAY  , 'Turno 1', 16.00, 12,  3, NULL),
    (@hoy - INTERVAL 36 DAY  , 'Turno 2', 12.10, 12,  3, NULL),
    (@hoy - INTERVAL 36 DAY  , 'Turno 1', 13.30, 13,  4, NULL),
    (@hoy - INTERVAL 36 DAY  , 'Turno 2', 10.10, 13,  4, NULL),
    (@hoy - INTERVAL 36 DAY  , 'Turno 1', 14.30, 14,  5, NULL),
    (@hoy - INTERVAL 36 DAY  , 'Turno 2', 10.70, 14,  5, NULL),
    (@hoy - INTERVAL 36 DAY  , 'Turno 1', 15.00, 15,  6, NULL),
    (@hoy - INTERVAL 36 DAY  , 'Turno 2', 11.20, 15,  6, NULL),
    (@hoy - INTERVAL 36 DAY  , 'Turno 1', 13.80, 16,  7, NULL),
    (@hoy - INTERVAL 36 DAY  , 'Turno 2', 10.30, 16,  7, NULL),
    (@hoy - INTERVAL 36 DAY  , 'Turno 1', 12.30, 17,  8, NULL),
    (@hoy - INTERVAL 36 DAY  , 'Turno 2',  9.20, 17,  8, NULL),

    -- Control de hace 6 dias, dentro de los ordenies de lote 5 y 6
    (@hoy - INTERVAL 6 DAY   , 'Turno 1', 10.20, 10,  1,  5),
    (@hoy - INTERVAL 6 DAY   , 'Turno 2',  7.80, 10,  1,  6),
    (@hoy - INTERVAL 6 DAY   , 'Turno 1',  8.30, 11,  2,  5),
    (@hoy - INTERVAL 6 DAY   , 'Turno 2',  6.20, 11,  2,  6),
    (@hoy - INTERVAL 6 DAY   , 'Turno 1', 15.10, 12,  3,  5),
    (@hoy - INTERVAL 6 DAY   , 'Turno 2', 11.30, 12,  3,  6),
    (@hoy - INTERVAL 6 DAY   , 'Turno 1', 12.40, 13,  4,  5),
    (@hoy - INTERVAL 6 DAY   , 'Turno 2',  9.40, 13,  4,  6),
    (@hoy - INTERVAL 6 DAY   , 'Turno 1', 13.50, 14,  5,  5),
    (@hoy - INTERVAL 6 DAY   , 'Turno 2', 10.10, 14,  5,  6),
    (@hoy - INTERVAL 6 DAY   , 'Turno 1', 14.20, 15,  6,  5),
    (@hoy - INTERVAL 6 DAY   , 'Turno 2', 10.70, 15,  6,  6),
    (@hoy - INTERVAL 6 DAY   , 'Turno 1', 11.60, 16,  7,  5),
    (@hoy - INTERVAL 6 DAY   , 'Turno 2',  8.70, 16,  7,  6),
    (@hoy - INTERVAL 6 DAY   , 'Turno 1', 13.00, 17,  8,  5),
    (@hoy - INTERVAL 6 DAY   , 'Turno 2',  9.70, 17,  8,  6);


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
    ( 1, @hoy - INTERVAL 432 DAY , 'Normal',    'Cria vendida al pie.',                          29),
    ( 2, @hoy - INTERVAL 397 DAY , 'Normal',    'Cria vendida al pie.',                          18),
    ( 3, @hoy - INTERVAL 354 DAY , 'Normal',    'Cria vendida al pie.',                          19),
    ( 4, @hoy - INTERVAL 331 DAY , 'Normal',    NULL,                                            11),  -- cria: 171
    ( 5, @hoy - INTERVAL 281 DAY , 'Normal',    'Cria vendida al pie.',                          10),
    ( 6, @hoy - INTERVAL 201 DAY , 'Normal',    NULL,                                            15),  -- cria: 174
    ( 7, @hoy - INTERVAL 181 DAY , 'Distócico', 'Requirio asistencia. Ternero de 48 kg.',        14),  -- cria: 175
    ( 8, @hoy - INTERVAL 162 DAY , 'Normal',    'Cria vendida al pie.',                          12),
    ( 9, @hoy - INTERVAL 127 DAY , 'Normal',    NULL,                                            13),  -- cria: 177
    (10, @hoy - INTERVAL 75 DAY  , 'Normal',    NULL,                                            16),  -- cria: 178
    (11, @hoy - INTERVAL 57 DAY  , 'Distócico', 'Cria nacida sin vida. Se llamo al veterinario.', 17);

-- ---------------------------------------------------------------------
-- celos
-- Los celos previos a cada servicio, mas los tres de las hembras que hoy
-- estan vacias y esperando ser servidas.
-- ---------------------------------------------------------------------
INSERT INTO celos (fecha, observaciones, id_animal) VALUES
    (@hoy - INTERVAL 141 DAY , 'Primer celo detectado. Entra en edad de servicio.', 20),
    (@hoy - INTERVAL 131 DAY , NULL,                                                15),
    (@hoy - INTERVAL 90 DAY  , 'Monta a otras vacas del lote.',                     12),
    (@hoy - INTERVAL 85 DAY  , NULL,                                                10),
    (@hoy - INTERVAL 80 DAY  , 'Celo firme, mucosa evidente.',                      13),
    (@hoy - INTERVAL 54 DAY  , NULL,                                                11),
    (@hoy - INTERVAL 25 DAY  , NULL,                                                14),
    (@hoy - INTERVAL 16 DAY  , 'Detectado en el ordenie de la tarde.',              21),
    (@hoy - INTERVAL 13 DAY  , 'Repite celo, el tacto del 01/07 dio vacia.',        13),
    (@hoy - INTERVAL 9 DAY   , 'Celo corto, dudoso.',                               16),
    (@hoy - INTERVAL 7 DAY   , NULL,                                                22),
    (@hoy - INTERVAL 3 DAY   , NULL,                                                17);

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
    ( 1, 'Inseminación artificial', @hoy - INTERVAL 276 DAY , @hoy + INTERVAL 7 DAY   , NULL,                              18, NULL,    8),
    ( 2, 'Inseminación artificial', @hoy - INTERVAL 253 DAY , @hoy + INTERVAL 30 DAY  , NULL,                              19, NULL,    9),
    ( 3, 'Inseminación artificial', @hoy - INTERVAL 139 DAY , @hoy + INTERVAL 144 DAY , 'Primer servicio de la vaquillona.', 20, NULL,  10),
    ( 4, 'Inseminación artificial', @hoy - INTERVAL 130 DAY , @hoy + INTERVAL 153 DAY , NULL,                              15, NULL,   10),
    ( 5, 'Inseminación artificial', @hoy - INTERVAL 89 DAY  , @hoy + INTERVAL 194 DAY , NULL,                              12, NULL,    8),
    ( 6, 'Inseminación artificial', @hoy - INTERVAL 83 DAY  , @hoy + INTERVAL 200 DAY , NULL,                              10, NULL,    8),
    ( 7, 'Inseminación artificial', @hoy - INTERVAL 78 DAY  , @hoy + INTERVAL 205 DAY , NULL,                              13, NULL,   10),
    ( 8, 'Monta natural',           @hoy - INTERVAL 52 DAY  , @hoy + INTERVAL 231 DAY , 'Servida por el toro T-01.',        11,    1, NULL),
    ( 9, 'Monta natural',           @hoy - INTERVAL 24 DAY  , @hoy + INTERVAL 259 DAY , NULL,                              14,    1, NULL),
    (10, 'Monta natural',           @hoy - INTERVAL 14 DAY  , @hoy + INTERVAL 269 DAY , 'Primer servicio de la vaquillona.', 21,   1, NULL);

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
    (@hoy - INTERVAL 239 DAY , 'Preñada', NULL,                                       1),
    (@hoy - INTERVAL 215 DAY , 'Dudosa',  'Se repite el tacto en cuatro semanas.',    2),
    (@hoy - INTERVAL 187 DAY , 'Preñada', 'Confirmada en el segundo tacto.',          2),
    (@hoy - INTERVAL 101 DAY , 'Preñada', NULL,                                       3),
    (@hoy - INTERVAL 92 DAY  , 'Preñada', NULL,                                       4),
    (@hoy - INTERVAL 53 DAY  , 'Preñada', NULL,                                       5),
    (@hoy - INTERVAL 47 DAY  , 'Preñada', NULL,                                       6),
    (@hoy - INTERVAL 41 DAY  , 'Vacía',   'Vuelve a servicio en el proximo celo.',    7);


-- =====================================================================
-- Modulo 4: sanidad
-- =====================================================================

-- ---------------------------------------------------------------------
-- diagnosticos
-- ---------------------------------------------------------------------
INSERT INTO diagnosticos (id_diagnostico, fecha_diagnostico, enfermedad, estado, id_animal) VALUES
    (1, @hoy - INTERVAL 46 DAY  , 'Metritis puerperal',                        'Resuelto',       16),
    (2, @hoy - INTERVAL 44 DAY  , 'Dermatitis digital (cojera)',               'Resuelto',       11),
    (3, @hoy - INTERVAL 5 DAY   , 'Mastitis clinica, cuarto anterior izquierdo','En tratamiento', 13);

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
-- El unico descarte vigente el dia de la carga es el de la vaca 115: por
-- eso queda fuera del lote de ordenie en los ultimos cuatro dias.
-- ---------------------------------------------------------------------
INSERT INTO tratamientos (id_tratamiento, fecha_inicio, dias_duracion, dosis_diaria, id_animal, fecha_fin_descarte, id_diagnostico, id_insumo, id_plan) VALUES
    -- Desparasitaciones de la recria, por plan sanitario
    ( 1, @hoy - INTERVAL 144 DAY , 1, '1 ml cada 50 kg, subcutanea', 20, @hoy - INTERVAL 115 DAY , NULL,  4, 4),
    ( 2, @hoy - INTERVAL 144 DAY , 1, '1 ml cada 50 kg, subcutanea', 21, @hoy - INTERVAL 115 DAY , NULL,  4, 4),
    ( 3, @hoy - INTERVAL 144 DAY , 1, '1 ml cada 50 kg, subcutanea', 22, @hoy - INTERVAL 115 DAY , NULL,  4, 4),
    ( 4, @hoy - INTERVAL 144 DAY , 1, '1 ml cada 50 kg, subcutanea', 23, @hoy - INTERVAL 115 DAY , NULL,  4, 4),
    ( 5, @hoy - INTERVAL 37 DAY  , 1, '1 ml cada 50 kg, subcutanea', 24, @hoy - INTERVAL 8 DAY   , NULL,  4, 4),
    ( 6, @hoy - INTERVAL 37 DAY  , 1, '1 ml cada 50 kg, subcutanea', 25, @hoy - INTERVAL 8 DAY   , NULL,  4, 4),
    ( 7, @hoy - INTERVAL 37 DAY  , 1, '1 ml cada 50 kg, subcutanea', 26, @hoy - INTERVAL 8 DAY   , NULL,  4, 4),
    -- Curativos
    ( 8, @hoy - INTERVAL 44 DAY  , 4, '15 ml intramuscular cada 24 h', 16, @hoy - INTERVAL 36 DAY  ,   1,  2, NULL),
    ( 9, @hoy - INTERVAL 44 DAY  , 1, 'Banio de pezuñas y 10 ml IM',   11, @hoy - INTERVAL 36 DAY  ,   2,  1, NULL),
    (10, @hoy - INTERVAL 4 DAY   , 3, '20 ml intramuscular cada 24 h', 13, @hoy + INTERVAL 6 DAY   ,   3,  1, NULL),
    -- Secado: una jeringa por cuarto, aplicacion unica
    (11, @hoy - INTERVAL 47 DAY  , 1, '1 jeringa intramamaria por cuarto', 18, @hoy - INTERVAL 16 DAY  , NULL, 3, NULL),
    (12, @hoy - INTERVAL 22 DAY  , 1, '1 jeringa intramamaria por cuarto', 19, @hoy + INTERVAL 9 DAY   , NULL, 3, NULL);

-- Egreso del producto que consumio cada tratamiento, con el mismo motivo
-- que escribe pTratamiento. Las cantidades no son uniformes: el secado
-- gasta cuatro jeringas y el resto, entre uno y tres frascos.
INSERT INTO movimientos_stock (tipo_movimiento, cantidad, fecha, fecha_vencimiento, motivo, id_insumo) VALUES
    ('Egreso', 1.00, @hoy - INTERVAL 144 DAY , NULL, 'Tratamiento sanitario',  4),
    ('Egreso', 1.00, @hoy - INTERVAL 144 DAY , NULL, 'Tratamiento sanitario',  4),
    ('Egreso', 1.00, @hoy - INTERVAL 144 DAY , NULL, 'Tratamiento sanitario',  4),
    ('Egreso', 1.00, @hoy - INTERVAL 144 DAY , NULL, 'Tratamiento sanitario',  4),
    ('Egreso', 1.00, @hoy - INTERVAL 37 DAY  , NULL, 'Tratamiento sanitario',  4),
    ('Egreso', 1.00, @hoy - INTERVAL 37 DAY  , NULL, 'Tratamiento sanitario',  4),
    ('Egreso', 1.00, @hoy - INTERVAL 37 DAY  , NULL, 'Tratamiento sanitario',  4),
    ('Egreso', 3.00, @hoy - INTERVAL 44 DAY  , NULL, 'Tratamiento sanitario',  2),
    ('Egreso', 1.00, @hoy - INTERVAL 44 DAY  , NULL, 'Tratamiento sanitario',  1),
    ('Egreso', 3.00, @hoy - INTERVAL 4 DAY   , NULL, 'Tratamiento sanitario',  1),
    ('Egreso', 4.00, @hoy - INTERVAL 47 DAY  , NULL, 'Tratamiento sanitario',  3),
    ('Egreso', 4.00, @hoy - INTERVAL 22 DAY  , NULL, 'Tratamiento sanitario',  3);

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
SELECT @hoy - INTERVAL 155 DAY , id_animal, 5, 1
FROM animales
WHERE id_animal NOT IN (2, 3, 4)
  AND fecha_nacimiento <= @hoy - INTERVAL 245 DAY;

-- Reproductiva, a las hembras en edad reproductiva que estaban activas.
INSERT INTO vacunaciones (fecha_aplicacion, id_animal, id_insumo, id_plan)
SELECT @hoy - INTERVAL 118 DAY , a.id_animal, 7, 3
FROM animales a
JOIN hembras h ON h.id_animal = a.id_animal
WHERE a.id_categoria IN (2, 3)
  AND a.activo = 1;

-- Brucelosis, unica en la vida y solo a las terneras que ya tenian tres
-- meses. La 177 nacio en abril y todavia no llega: queda pendiente en el
-- calendario sanitario.
INSERT INTO vacunaciones (fecha_aplicacion, id_animal, id_insumo, id_plan) VALUES
    (@hoy - INTERVAL 52 DAY  , 23, 6, 2),
    (@hoy - INTERVAL 52 DAY  , 24, 6, 2);

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
    (@hoy - INTERVAL 317 DAY , 'Pasta caustica', 'A los catorce dias de vida.', 23, 5),
    (@hoy - INTERVAL 187 DAY , 'Pasta caustica', NULL,                          24, 5),
    (@hoy - INTERVAL 167 DAY , 'Pasta caustica', NULL,                          26, 5),
    (@hoy - INTERVAL 113 DAY , 'Termocauterio',  'Se agoto la pasta.',          25, 5);


-- =====================================================================
-- Verificacion
--
-- Las dos consultas comprueban lo unico que este script no puede
-- garantizar escribiendo los datos a mano: que el stock declarado en
-- insumos coincida con el saldo de sus movimientos, y que los litros de
-- los controles individuales de hace 6 dias den exactamente el total de los
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
