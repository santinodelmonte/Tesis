// Paginado de tablas.
//
// Cualquier tabla que declare data-paginar="N" se pagina sola: el script arma los
// controles debajo, muestra de a N filas y esconde el resto. No hace falta tocar la
// pantalla que la contiene.
//
// El paginado es del lado del navegador y no de la base. Con el volumen de un tambo
// -un rodeo de cientos de animales, unos miles de movimientos- la consulta ya trae
// todo a memoria de una sola vez, asi que partir la consulta no ahorraria trabajo:
// lo que molesta es la tabla de trescientas filas en pantalla, y eso es lo que
// resuelve.
//
// Las pantallas que ademas filtran por su cuenta -el selector de animal- marcan las
// filas descartadas con data-filtrada="1" y llaman a paginarTabla(tabla) para que el
// paginado se rehaga sobre lo que quedo.
(function () {
    "use strict";

    var TAMANIO_POR_DEFECTO = 10;

    // Las filas que el paginador reparte: todas las del cuerpo salvo las que el filtro
    // de la propia pantalla ya dejo afuera.
    function filasPaginables(tabla) {
        var filas = [];

        if (tabla.tBodies.length === 0) {
            return filas;
        }

        for (var i = 0; i < tabla.tBodies[0].rows.length; i = i + 1) {
            var fila = tabla.tBodies[0].rows[i];

            if (fila.getAttribute("data-filtrada") !== "1") {
                filas.push(fila);
            }
        }
        return filas;
    }

    function tamanioPagina(tabla) {
        var valor = parseInt(tabla.getAttribute("data-paginar"), 10);

        if (isNaN(valor) || valor < 1) {
            return TAMANIO_POR_DEFECTO;
        }
        return valor;
    }

    function controlesDe(tabla) {
        var id = tabla.getAttribute("data-paginador");
        return id === null ? null : document.getElementById(id);
    }

    // Los botones van con type="button" a proposito: varias de estas tablas viven
    // adentro de un formulario y un submit accidental guardaria la pantalla.
    function crearControles(tabla, indice) {
        var contenedor = document.createElement("div");
        contenedor.id = "paginador" + indice;
        contenedor.className = "d-flex justify-content-between align-items-center flex-wrap mb-3";
        contenedor.innerHTML =
            '<small class="text-muted" data-rol="detalle"></small>' +
            '<div class="btn-group">' +
            '<button type="button" class="btn btn-outline-secondary btn-sm" data-rol="anterior">Anterior</button>' +
            '<button type="button" class="btn btn-outline-secondary btn-sm" data-rol="pagina" disabled></button>' +
            '<button type="button" class="btn btn-outline-secondary btn-sm" data-rol="siguiente">Siguiente</button>' +
            '</div>';

        tabla.parentNode.insertBefore(contenedor, tabla.nextSibling);
        tabla.setAttribute("data-paginador", contenedor.id);

        contenedor.querySelector('[data-rol="anterior"]').onclick = function () {
            irAPagina(tabla, paginaActual(tabla) - 1);
        };
        contenedor.querySelector('[data-rol="siguiente"]').onclick = function () {
            irAPagina(tabla, paginaActual(tabla) + 1);
        };
    }

    function paginaActual(tabla) {
        var valor = parseInt(tabla.getAttribute("data-pagina"), 10);
        return isNaN(valor) ? 1 : valor;
    }

    function irAPagina(tabla, pagina) {
        tabla.setAttribute("data-pagina", pagina);
        pintar(tabla);
    }

    function pintar(tabla) {
        var filas = filasPaginables(tabla);
        var tamanio = tamanioPagina(tabla);
        var paginas = Math.max(1, Math.ceil(filas.length / tamanio));
        var pagina = paginaActual(tabla);

        if (pagina > paginas) {
            pagina = paginas;
        }
        if (pagina < 1) {
            pagina = 1;
        }
        tabla.setAttribute("data-pagina", pagina);

        var desde = (pagina - 1) * tamanio;
        for (var i = 0; i < filas.length; i = i + 1) {
            filas[i].style.display = (i >= desde && i < desde + tamanio) ? "" : "none";
        }

        var controles = controlesDe(tabla);
        if (controles === null) {
            return;
        }

        // Con una sola pagina los controles no aportan nada
        controles.style.display = filas.length > tamanio ? "" : "none";

        var hasta = Math.min(desde + tamanio, filas.length);
        controles.querySelector('[data-rol="detalle"]').textContent =
            "Mostrando " + (filas.length === 0 ? 0 : desde + 1) + " a " + hasta + " de " + filas.length + " registros";
        controles.querySelector('[data-rol="pagina"]').textContent = "Pagina " + pagina + " de " + paginas;
        controles.querySelector('[data-rol="anterior"]').disabled = pagina === 1;
        controles.querySelector('[data-rol="siguiente"]').disabled = pagina === paginas;
    }

    // La usan las pantallas que filtran por su cuenta: vuelve a la primera pagina y
    // reparte de nuevo lo que quedo despues del filtro.
    window.paginarTabla = function (tabla) {
        if (tabla === null || tabla.getAttribute("data-paginar") === null) {
            return;
        }

        if (controlesDe(tabla) === null) {
            crearControles(tabla, document.querySelectorAll("[data-paginador]").length);
        }

        tabla.setAttribute("data-pagina", 1);
        pintar(tabla);
    };

    document.addEventListener("DOMContentLoaded", function () {
        var tablas = document.querySelectorAll("table[data-paginar]");

        for (var i = 0; i < tablas.length; i = i + 1) {
            crearControles(tablas[i], i);
            irAPagina(tablas[i], 1);
        }
    });
})();
