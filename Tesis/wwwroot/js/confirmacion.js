// Confirmacion de las acciones que no se pueden deshacer.
//
// Cualquier formulario que declare data-confirmar="texto" pide confirmacion antes
// de enviarse, y cualquier boton que declare data-aviso="texto" muestra ese texto
// sin hacer nada mas. Es lo que usan los listados de Reproduccion y Sanidad para
// eliminar un registro mal cargado, y para explicar por que a veces no se puede.
//
// El dialogo lo arma el script y no cada pantalla: son ocho listados y ninguno
// necesita saber como se ve. Si el navegador no ejecuta el script, el formulario se
// envia igual y la accion se hace sin confirmar; el listado sigue funcionando.
(function () {
    "use strict";

    var ID_DIALOGO = "dialogoConfirmacion";

    // El dialogo se crea una sola vez, la primera vez que hace falta, y se reusa.
    function obtenerDialogo() {
        var dialogo = document.getElementById(ID_DIALOGO);

        if (dialogo !== null) {
            return dialogo;
        }

        dialogo = document.createElement("div");
        dialogo.id = ID_DIALOGO;
        dialogo.className = "modal fade";
        dialogo.setAttribute("tabindex", "-1");
        dialogo.innerHTML =
            '<div class="modal-dialog modal-dialog-centered">' +
            '  <div class="modal-content">' +
            '    <div class="modal-header">' +
            '      <h5 class="modal-title"></h5>' +
            '      <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Cerrar"></button>' +
            '    </div>' +
            '    <div class="modal-body"></div>' +
            '    <div class="modal-footer">' +
            '      <button type="button" class="btn btn-secondary" data-bs-dismiss="modal">Cancelar</button>' +
            '      <button type="button" class="btn btn-danger" data-papel="aceptar">Eliminar</button>' +
            '    </div>' +
            '  </div>' +
            '</div>';

        document.body.appendChild(dialogo);
        return dialogo;
    }

    function mostrar(pTitulo, pTexto, pAlAceptar) {
        var dialogo = obtenerDialogo();

        dialogo.querySelector(".modal-title").textContent = pTitulo;
        dialogo.querySelector(".modal-body").textContent = pTexto;

        var botonAceptar = dialogo.querySelector('[data-papel="aceptar"]');
        var botonCancelar = dialogo.querySelector(".modal-footer .btn-secondary");

        // Sin accion que confirmar el dialogo es informativo: no tiene sentido
        // ofrecer un boton que elimine nada.
        if (pAlAceptar === null) {
            botonAceptar.classList.add("d-none");
            botonCancelar.textContent = "Entendido";
        } else {
            botonAceptar.classList.remove("d-none");
            botonCancelar.textContent = "Cancelar";
        }

        // El boton se reemplaza por una copia para que no le queden enganchados los
        // manejadores de la fila anterior.
        var botonNuevo = botonAceptar.cloneNode(true);
        botonAceptar.parentNode.replaceChild(botonNuevo, botonAceptar);

        if (pAlAceptar !== null) {
            botonNuevo.addEventListener("click", function () {
                bootstrap.Modal.getInstance(dialogo).hide();
                pAlAceptar();
            });
        }

        bootstrap.Modal.getOrCreateInstance(dialogo).show();
    }

    document.addEventListener("submit", function (evento) {
        var formulario = evento.target;
        var texto = formulario.getAttribute("data-confirmar");

        if (texto === null) {
            return;
        }

        evento.preventDefault();

        mostrar("Confirmar la eliminacion", texto, function () {
            // El formulario se envia sin volver a pasar por aca: el atributo ya
            // cumplio su funcion.
            formulario.removeAttribute("data-confirmar");
            formulario.submit();
        });
    });

    document.addEventListener("click", function (evento) {
        var boton = evento.target.closest("[data-aviso]");

        if (boton === null) {
            return;
        }

        evento.preventDefault();
        mostrar("No se puede eliminar", boton.getAttribute("data-aviso"), null);
    });
})();
