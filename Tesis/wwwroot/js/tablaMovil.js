// Tablas que se leen en un celular.
//
// Abajo de 768px, la hoja de estilos convierte cada fila en una tarjeta con
// "etiqueta: valor". Para eso cada celda necesita saber de qué columna es, y
// eso es lo que hace este script: copia el encabezado de cada columna dentro de
// cada celda. Las treinta y cuatro tablas del sistema no tuvieron que tocarse.
//
// Es el mismo criterio que paginador.js: una conducta que vale para todas las
// tablas se resuelve una vez acá y no pantalla por pantalla.
//
// La parte que no se ve, y que es la que importa: pasar las celdas a
// display:block le quita a la tabla sus roles implícitos, y para un lector de
// pantalla deja de ser una tabla para ser una pila de textos sueltos. Este
// script repone esos roles de forma explícita, así que la tabla sigue siendo
// una tabla en cualquier ancho de pantalla.
//
// Una tabla que declare data-movil="tabla" queda afuera: son las de carga
// -el control lechero, el ordeñe por lote-, de cuatro columnas y con el campo
// de escritura adentro, donde apilar alarga la pantalla sin ganar nada.
(function () {
    "use strict";

    // Los encabezados de la tabla, en orden. Solo se toma la primera fila del
    // encabezado: ninguna tabla del sistema usa encabezados de dos niveles.
    function encabezados(tabla) {
        var textos = [];

        if (tabla.tHead === null || tabla.tHead.rows.length === 0) {
            return textos;
        }

        var celdas = tabla.tHead.rows[0].cells;
        for (var i = 0; i < celdas.length; i = i + 1) {
            textos.push(celdas[i].textContent.trim());
            celdas[i].setAttribute("role", "columnheader");
        }
        return textos;
    }

    // La etiqueta va en un elemento propio y no en un ::before de la hoja de
    // estilos, para poder marcarla como decorativa: el lector de pantalla ya
    // anuncia la columna a partir del encabezado, y sin este aria-hidden la
    // escucharía dos veces.
    function ponerEtiqueta(celda, texto) {
        if (texto === "" || celda.querySelector(".tabla-etiqueta") !== null) {
            return;
        }

        var etiqueta = document.createElement("span");
        etiqueta.className = "tabla-etiqueta";
        etiqueta.setAttribute("aria-hidden", "true");
        etiqueta.textContent = texto;

        celda.insertBefore(etiqueta, celda.firstChild);
    }

    function prepararTabla(tabla) {
        if (tabla.getAttribute("data-movil") === "tabla") {
            return;
        }

        var titulos = encabezados(tabla);
        if (titulos.length === 0) {
            return;
        }

        tabla.classList.add("tabla-adaptable");

        // Los roles explícitos que reponen la semántica de tabla
        tabla.setAttribute("role", "table");

        if (tabla.tHead !== null) {
            tabla.tHead.setAttribute("role", "rowgroup");
            tabla.tHead.rows[0].setAttribute("role", "row");
        }

        for (var t = 0; t < tabla.tBodies.length; t = t + 1) {
            var cuerpo = tabla.tBodies[t];
            cuerpo.setAttribute("role", "rowgroup");

            for (var f = 0; f < cuerpo.rows.length; f = f + 1) {
                var fila = cuerpo.rows[f];
                fila.setAttribute("role", "row");

                for (var c = 0; c < fila.cells.length; c = c + 1) {
                    var celda = fila.cells[c];
                    celda.setAttribute("role", "cell");

                    // La celda que abarca varias columnas -el "no hay
                    // registros"- no corresponde a ninguna en particular, así
                    // que no lleva etiqueta.
                    if (celda.colSpan > 1) {
                        continue;
                    }

                    if (c < titulos.length) {
                        ponerEtiqueta(celda, titulos[c]);
                    }
                }
            }
        }
    }

    function prepararTodas() {
        var tablas = document.querySelectorAll("table");

        for (var i = 0; i < tablas.length; i = i + 1) {
            prepararTabla(tablas[i]);
        }
    }

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", prepararTodas);
    } else {
        prepararTodas();
    }

    // El selector de animal arma su tabla después de cargar la pantalla, así
    // que hay que poder pedirle al script que la prepare cuando aparezca.
    window.prepararTablaMovil = prepararTabla;
})();
