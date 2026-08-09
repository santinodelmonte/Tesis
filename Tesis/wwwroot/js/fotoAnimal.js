// Carga de la foto del animal.
//
// La foto no viaja como archivo adjunto sino como texto dentro de un campo oculto
// del propio formulario. La razon es que las pantallas de alta reenvian el
// formulario varias veces -calcular la categoria, confirmar las advertencias de
// genealogia- y el navegador vacia un campo de archivo en cada reenvio: la usuaria
// elegia la foto, apretaba "Calcular Categoria" y la foto se perdia sin aviso.
//
// De paso resuelve el peso. Una foto sacada con el celular pesa varios megas y en el
// campo hay poca senal, asi que antes de enviarla se la achica y se la recomprime
// aca, en el navegador. Lo que sube son unos cientos de kilobytes.

// Lado maximo de la imagen que se envia. Alcanza de sobra para reconocer al animal
// en la ficha y en el arbol, que es para lo que esta la foto.
var FOTO_LADO_MAXIMO = 1200;

// Calidad del JPEG con el que se recomprime. Por debajo de 0.8 se empiezan a ver los
// bloques en el pelaje.
var FOTO_CALIDAD = 0.8;

// Toma el archivo que eligio el usuario, lo achica y lo deja listo para enviar.
// pCampo es el nombre base del control: hay un campo oculto pCampo con el contenido
// y una vista previa con id vista_pCampo.
function prepararFoto(pInputArchivo, pCampo) {
    var archivo = pInputArchivo.files && pInputArchivo.files[0];
    if (!archivo) {
        return;
    }

    if (archivo.type.indexOf("image/") !== 0) {
        alert("El archivo elegido no es una imagen.");
        pInputArchivo.value = "";
        return;
    }

    var lector = new FileReader();

    lector.onload = function (evento) {
        var imagen = new Image();

        imagen.onload = function () {
            // El lado mas largo se lleva al maximo y el otro acompana, para que la
            // foto no quede deformada
            var ancho = imagen.width;
            var alto = imagen.height;
            var escala = Math.min(1, FOTO_LADO_MAXIMO / Math.max(ancho, alto));

            var lienzo = document.createElement("canvas");
            lienzo.width = Math.round(ancho * escala);
            lienzo.height = Math.round(alto * escala);

            var contexto = lienzo.getContext("2d");
            contexto.drawImage(imagen, 0, 0, lienzo.width, lienzo.height);

            var contenido = lienzo.toDataURL("image/jpeg", FOTO_CALIDAD);

            document.getElementById(pCampo).value = contenido;
            mostrarVistaPreviaFoto(pCampo, contenido);

            // Si venia tildado "quitar", elegir una foto nueva lo destilda solo
            var quitar = document.getElementById(pCampo + "Quitar");
            if (quitar) {
                quitar.value = "false";
            }
        };

        imagen.onerror = function () {
            alert("No se pudo leer la imagen elegida.");
            pInputArchivo.value = "";
        };

        imagen.src = evento.target.result;
    };

    lector.readAsDataURL(archivo);
}

// Descarta la foto: la que se acababa de elegir y, si el animal ya tenia una
// guardada, tambien esa. El campo oculto avisa al servidor que hay que borrarla.
function quitarFoto(pCampo) {
    document.getElementById(pCampo).value = "";

    var quitar = document.getElementById(pCampo + "Quitar");
    if (quitar) {
        quitar.value = "true";
    }

    var archivo = document.getElementById(pCampo + "Archivo");
    if (archivo) {
        archivo.value = "";
    }

    mostrarVistaPreviaFoto(pCampo, "");
}

// Cuando el formulario se reenvia, la foto elegida vuelve del servidor dentro del
// campo oculto pero el recuadro se dibuja vacio. Esto la vuelve a mostrar.
document.addEventListener("DOMContentLoaded", function () {
    var campos = document.querySelectorAll("input[data-foto]");

    for (var i = 0; i < campos.length; i++) {
        if (campos[i].value !== "") {
            mostrarVistaPreviaFoto(campos[i].id, campos[i].value);
        }
    }
});

// Muestra la foto elegida o, si no hay ninguna, el recuadro vacio de siempre
function mostrarVistaPreviaFoto(pCampo, pContenido) {
    var vista = document.getElementById("vista_" + pCampo);
    var vacio = document.getElementById("vacio_" + pCampo);
    var boton = document.getElementById("quitar_" + pCampo);

    if (pContenido !== "") {
        vista.src = pContenido;
        vista.style.display = "block";
        if (vacio) {
            vacio.style.display = "none";
        }
        if (boton) {
            boton.style.display = "inline-block";
        }
    } else {
        vista.removeAttribute("src");
        vista.style.display = "none";
        if (vacio) {
            vacio.style.display = "flex";
        }
        if (boton) {
            boton.style.display = "none";
        }
    }
}
