// Registro rápido del inicio.
//
// La tarjeta ofrece los cuatro eventos reproductivos en un mismo formulario y muestra
// por vez los campos del que está elegido. Todo lo que hace este script es esconder lo
// que no corresponde y cambiarle el texto al botón: el evento viaja como un campo más
// del formulario y es el servidor el que decide con él, así que sin JavaScript se ven
// los cuatro grupos de campos a la vez y el registro entra igual. Nada de lo que hay
// acá hace falta para que la carga funcione, y por eso no hay ninguna regla escrita
// dos veces.
//
// Los elementos declaran a qué eventos pertenecen con data-registro-para="celo tacto",
// igual que el resto de los scripts del sistema: la pantalla dice qué es cada cosa y
// el script no sabe nada del formulario en particular.
(function () {
    "use strict";

    function eventoElegido(pFormulario) {
        var elegido = pFormulario.querySelector("input[name='evento']:checked");
        return elegido !== null ? elegido.value : "";
    }

    // Un elemento pertenece al evento si figura en su lista separada por espacios. Se
    // compara con los espacios puestos para que "parto" no coincida con "partos".
    function corresponde(pElemento, pEvento) {
        var lista = " " + pElemento.getAttribute("data-registro-para") + " ";
        return lista.indexOf(" " + pEvento + " ") !== -1;
    }

    function mostrarEvento(pFormulario) {
        var evento = eventoElegido(pFormulario);
        var partes = pFormulario.querySelectorAll("[data-registro-para]");

        for (var i = 0; i < partes.length; i = i + 1) {
            // hidden y no display:none: lo escondido tiene que salir también del árbol
            // de accesibilidad, si no el lector de pantalla sigue anunciando campos que
            // no se ven ni se van a guardar.
            partes[i].hidden = !corresponde(partes[i], evento);
        }

        // El botón dice lo que va a pasar. No es un detalle de redacción: guardar el
        // celo acá y abrir el formulario del parto en otra pantalla son dos cosas
        // distintas, y el botón es lo único que las distingue antes de apretarlo.
        var boton = pFormulario.querySelector("[data-registro-boton]");
        var texto = boton !== null ? boton.getAttribute("data-texto-" + evento) : null;

        if (texto !== null) {
            boton.innerText = texto;
        }
    }

    // Las caravanas sugeridas llenan el campo y dejan el foco adentro, para poder
    // seguir tecleando -corregir el número, pasar a la fecha- sin volver al mouse.
    function usarCaravana(pFormulario, pCaravana) {
        var campo = pFormulario.querySelector("[name='numCaravana']");

        if (campo !== null) {
            campo.value = pCaravana;
            campo.focus();
        }
    }

    function iniciar() {
        var formulario = document.querySelector("[data-registro-rapido]");

        if (formulario === null) {
            return;
        }

        var opciones = formulario.querySelectorAll("input[name='evento']");
        for (var i = 0; i < opciones.length; i = i + 1) {
            opciones[i].addEventListener("change", function () {
                mostrarEvento(formulario);
            });
        }

        var sugeridas = formulario.querySelectorAll("[data-registro-caravana]");
        for (var j = 0; j < sugeridas.length; j = j + 1) {
            sugeridas[j].addEventListener("click", function () {
                usarCaravana(formulario, this.getAttribute("data-registro-caravana"));
            });
        }

        mostrarEvento(formulario);
    }

    if (document.readyState === "loading") {
        document.addEventListener("DOMContentLoaded", iniciar);
    } else {
        iniciar();
    }
})();
