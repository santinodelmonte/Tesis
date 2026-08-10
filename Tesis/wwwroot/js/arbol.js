// Arbol genealogico interactivo (CU4 - Consultar Linaje).
//
// El servidor manda la ascendencia entera en un bloque JSON y aca se dibuja. Que
// venga toda junta y no de a una generacion por pedido es a proposito: los animales
// ya estan en memoria del lado del servidor, asi que ir a buscarle la madre a un
// animal cuando se despliega su rama seria un viaje de ida y vuelta por un dato que
// el navegador podria tener desde el principio. Desplegar una rama es, entonces,
// instantaneo.
//
// Las ramas se identifican por su camino desde la raiz -"" el animal, "M" la madre,
// "MP" el abuelo materno- y no por el id del animal. En un rodeo con algo de
// consanguinidad el mismo toro aparece en dos ramas distintas, y con el id como
// identificador desplegar una habria desplegado la otra.

// Arbol completo tal como llego del servidor, y el que se esta mostrando. Son
// distintos cuando se recentro la vista en un ancestro.
var arbolOriginal = null;
var arbolActual = null;

// Caminos de las ramas desplegadas
var ramasDesplegadas = {};

// Camino del nodo abierto en el panel lateral
var rutaElegida = null;

var zoomActual = 1;

var ZOOM_MINIMO = 0.5;
var ZOOM_MAXIMO = 1.5;
var ZOOM_PASO = 0.15;

document.addEventListener("DOMContentLoaded", function () {
    var datos = document.getElementById("datosArbol");
    if (!datos) {
        return;
    }

    arbolOriginal = JSON.parse(datos.textContent);
    centrarArbol(arbolOriginal, false);
});

// Deja al nodo recibido como raiz de la vista. pEsRecentrado distingue el dibujo
// inicial del que pide el usuario desde el panel, que ademas tiene que avisar que lo
// que se ve ya no es el animal consultado.
function centrarArbol(pNodo, pEsRecentrado) {
    arbolActual = pNodo;

    // Al recentrar se vuelve a la apertura por defecto: el animal, sus padres y sus
    // abuelos, que es hasta donde llega el registro genealogico de la mayoria
    ramasDesplegadas = { "": true, "M": true, "P": true };
    rutaElegida = null;

    document.getElementById("linajeAviso").style.display = pEsRecentrado ? "block" : "none";
    if (pEsRecentrado) {
        document.getElementById("linajeAvisoAnimal").textContent = pNodo.caravana;
    }

    dibujarArbol();
    limpiarPanel();
}

function volverAlAnimalConsultado() {
    centrarArbol(arbolOriginal, false);
}

function dibujarArbol() {
    var lienzo = document.getElementById("linajeLienzo");
    lienzo.textContent = "";
    lienzo.appendChild(dibujarRama(arbolActual, ""));

    // Las llaves se miden sobre el arbol ya armado y sin ampliar. Si se midiera con
    // el zoom puesto, las medidas vendrian multiplicadas por el zoom y al aplicarlas
    // adentro del lienzo -que tambien se amplia- quedarian multiplicadas dos veces.
    lienzo.style.transform = "none";
    dibujarLlaves();
    aplicarZoom();

    mostrarRaiz();
}

// Une cada animal con su madre y su padre. Se hace midiendo y no con bordes fijos
// porque el centro de cada progenitor depende de cuanta ascendencia tenga colgando,
// que es algo que recien se sabe con el arbol dibujado.
function dibujarLlaves() {
    var contenedores = document.querySelectorAll(".linaje-progenitores");

    for (var i = 0; i < contenedores.length; i++) {
        dibujarLlave(contenedores[i]);
    }
}

function dibujarLlave(pContenedor) {
    var hijos = pContenedor.querySelectorAll(":scope > .linaje-hijo");
    if (hijos.length < 2) {
        return;
    }

    var base = pContenedor.getBoundingClientRect();
    var centroMadre = centroDelNodo(hijos[0], base);
    var centroPadre = centroDelNodo(hijos[1], base);

    // El animal esta centrado sobre todo el bloque de sus progenitores, asi que su
    // centro siempre cae entre el de la madre y el del padre: el tramo vertical que
    // los une pasa por el, y la llave cierra sin diagonales.
    var nodoHijo = pContenedor.previousElementSibling;
    var centroHijo = centroDelNodo(nodoHijo, base);

    var union = document.createElement("span");
    union.className = "linaje-union";
    union.style.top = centroMadre + "px";
    union.style.height = (centroPadre - centroMadre) + "px";
    pContenedor.appendChild(union);

    // Del animal hasta la vertical, y de la vertical hasta cada progenitor
    pContenedor.appendChild(dibujarBrazo(centroHijo, -28, 14));
    pContenedor.appendChild(dibujarBrazo(centroMadre, -14, 14));
    pContenedor.appendChild(dibujarBrazo(centroPadre, -14, 14));
}

function dibujarBrazo(pAlto, pIzquierda, pAncho) {
    var brazo = document.createElement("span");
    brazo.className = "linaje-brazo";
    brazo.style.top = pAlto + "px";
    brazo.style.left = pIzquierda + "px";
    brazo.style.width = pAncho + "px";
    return brazo;
}

// Alto del centro del nodo, medido desde el borde de arriba del contenedor. De una
// rama interesa su propio nodo y no toda la rama: la rama incluye la ascendencia,
// que se extiende bastante mas arriba y mas abajo.
function centroDelNodo(pElemento, pCajaBase) {
    var nodo = pElemento;

    if (pElemento.classList.contains("linaje-rama")) {
        nodo = pElemento.firstElementChild;
    }

    var caja = nodo.getBoundingClientRect();

    return (caja.top + caja.height / 2) - pCajaBase.top;
}

// Deja a la vista el animal por el que se consulto. Hace falta porque el nodo queda
// centrado sobre el alto de toda su ascendencia: al desplegar unas cuantas
// generaciones el arbol crece para arriba y para abajo, y sin esto el animal se va
// del recuadro y da la impresion de haber desaparecido.
function mostrarRaiz() {
    var marco = document.querySelector(".linaje-marco");
    var raiz = document.querySelector(".linaje-nodo-raiz");

    if (marco === null || raiz === null) {
        return;
    }

    // Se miden las posiciones en pantalla y no las del documento: el nodo cuelga de
    // contenedores posicionados, asi que su desplazamiento no se cuenta desde el
    // recuadro
    var marcoEnPantalla = marco.getBoundingClientRect();
    var raizEnPantalla = raiz.getBoundingClientRect();

    marco.scrollLeft = 0;
    marco.scrollTop = marco.scrollTop
        + (raizEnPantalla.top - marcoEnPantalla.top)
        - (marco.clientHeight / 2)
        + (raizEnPantalla.height / 2);
}

// Dibuja el nodo y, si su rama esta desplegada, la de sus dos progenitores
function dibujarRama(pNodo, pRuta) {
    var rama = document.createElement("div");
    rama.className = "linaje-rama";
    rama.appendChild(dibujarNodo(pNodo, pRuta));

    if (!ramasDesplegadas[pRuta] || !tieneProgenitores(pNodo)) {
        return rama;
    }

    var progenitores = document.createElement("div");
    progenitores.className = "linaje-progenitores";

    // Siempre van los dos, aunque uno no este registrado: el casillero vacio dice
    // que ahi se corta lo que se sabe del animal, que no es lo mismo que no
    // mostrar nada. La clase linaje-hijo los distingue de las lineas de la llave,
    // que se agregan a este mismo contenedor una vez medido el arbol.
    var madre = pNodo.madre
        ? dibujarRama(pNodo.madre, pRuta + "M")
        : dibujarNodoVacio("Madre no registrada");
    madre.className += " linaje-hijo";
    progenitores.appendChild(madre);

    var padre = pNodo.padre
        ? dibujarRama(pNodo.padre, pRuta + "P")
        : dibujarNodoVacio("Padre no registrado");
    padre.className += " linaje-hijo";
    progenitores.appendChild(padre);

    rama.appendChild(progenitores);

    return rama;
}

function dibujarNodo(pNodo, pRuta) {
    var esHembra = pNodo.sexo === "H";

    var nodo = document.createElement("div");
    nodo.className = "linaje-nodo " + (esHembra ? "linaje-nodo-hembra" : "linaje-nodo-macho");
    if (pRuta === "") {
        nodo.className += " linaje-nodo-raiz";
    }
    if (!pNodo.activo) {
        nodo.className += " linaje-nodo-inactivo";
    }
    if (pRuta === rutaElegida) {
        nodo.className += " linaje-nodo-elegido";
    }
    nodo.title = "Ver los datos de " + pNodo.caravana;
    nodo.onclick = function () {
        elegirNodo(pNodo, pRuta);
    };

    nodo.appendChild(dibujarFoto(pNodo, esHembra));

    var datos = document.createElement("div");
    datos.className = "linaje-datos";

    var caravana = document.createElement("div");
    caravana.className = "linaje-caravana";
    // textContent y no innerHTML: la caravana y la raza son texto cargado por el
    // usuario y no tienen por que interpretarse como HTML
    caravana.textContent = pNodo.caravana;
    datos.appendChild(caravana);

    var detalle = document.createElement("div");
    detalle.className = "linaje-detalle";
    detalle.textContent = describirParentesco(pRuta) + (pNodo.raza !== "" ? " · " + pNodo.raza : "");
    datos.appendChild(detalle);

    nodo.appendChild(datos);

    // El boton solo aparece donde hay algo mas para ver
    if (tieneProgenitores(pNodo)) {
        var boton = document.createElement("button");
        boton.type = "button";
        boton.className = "linaje-expandir";
        boton.textContent = ramasDesplegadas[pRuta] ? "−" : "+";
        boton.title = ramasDesplegadas[pRuta]
            ? "Ocultar la ascendencia de " + pNodo.caravana
            : "Ver la ascendencia de " + pNodo.caravana;
        boton.onclick = function (evento) {
            // Sin esto el clic del boton abriria ademas el panel del nodo
            evento.stopPropagation();
            alternarRama(pRuta);
        };
        nodo.appendChild(boton);
    }

    return nodo;
}

function dibujarFoto(pNodo, pEsHembra) {
    if (pNodo.foto !== "") {
        var imagen = document.createElement("img");
        imagen.className = "linaje-foto";
        imagen.src = pNodo.foto;
        imagen.alt = "Foto de " + pNodo.caravana;
        return imagen;
    }

    // Sin foto va la inicial del sexo sobre el color que le corresponde, para que el
    // nodo se siga leyendo de un vistazo
    var vacia = document.createElement("div");
    vacia.className = "linaje-foto linaje-foto-vacia "
        + (pEsHembra ? "linaje-foto-vacia-hembra" : "linaje-foto-vacia-macho");
    vacia.textContent = pEsHembra ? "H" : "M";
    return vacia;
}

function dibujarNodoVacio(pTexto) {
    var vacio = document.createElement("div");
    vacio.className = "linaje-nodo-vacio";
    vacio.textContent = pTexto;
    return vacio;
}

function tieneProgenitores(pNodo) {
    return pNodo.madre !== null || pNodo.padre !== null;
}

function alternarRama(pRuta) {
    if (ramasDesplegadas[pRuta]) {
        delete ramasDesplegadas[pRuta];
    } else {
        ramasDesplegadas[pRuta] = true;
    }
    dibujarArbol();
}

function desplegarTodo() {
    ramasDesplegadas = {};
    marcarRamaCompleta(arbolActual, "");
    dibujarArbol();
}

function marcarRamaCompleta(pNodo, pRuta) {
    if (pNodo === null) {
        return;
    }
    ramasDesplegadas[pRuta] = true;
    marcarRamaCompleta(pNodo.madre, pRuta + "M");
    marcarRamaCompleta(pNodo.padre, pRuta + "P");
}

function contraerTodo() {
    ramasDesplegadas = {};
    dibujarArbol();
}

// --------------------------------------------------------------------
// Panel lateral
// --------------------------------------------------------------------

function elegirNodo(pNodo, pRuta) {
    rutaElegida = pRuta;
    dibujarArbol();

    document.getElementById("panelVacio").style.display = "none";
    document.getElementById("panelDatos").style.display = "block";

    var foto = document.getElementById("panelFoto");
    var fotoVacia = document.getElementById("panelFotoVacia");

    if (pNodo.foto !== "") {
        foto.src = pNodo.foto;
        foto.alt = "Foto de " + pNodo.caravana;
        foto.style.display = "block";
        fotoVacia.style.display = "none";
    } else {
        foto.removeAttribute("src");
        foto.style.display = "none";
        fotoVacia.style.display = "flex";
    }

    document.getElementById("panelCaravana").textContent = pNodo.caravana;
    document.getElementById("panelParentesco").textContent = describirParentesco(pRuta);
    document.getElementById("panelSexo").textContent = pNodo.sexo === "H" ? "Hembra" : "Macho";
    document.getElementById("panelRaza").textContent = pNodo.raza !== "" ? pNodo.raza : "Sin registrar";
    document.getElementById("panelCategoria").textContent = pNodo.categoria !== "" ? pNodo.categoria : "Sin registrar";
    document.getElementById("panelNacimiento").textContent = pNodo.nacimiento;
    document.getElementById("panelEdad").textContent = pNodo.edadMeses + " meses";

    var estado = document.getElementById("panelEstado");
    if (pNodo.activo) {
        estado.textContent = "Activo";
        estado.className = "badge bg-success";
    } else {
        estado.textContent = pNodo.motivoBaja !== "" ? "Baja: " + pNodo.motivoBaja : "Dado de baja";
        estado.className = "badge bg-secondary";
    }

    // Los partos son de la hembra: en el macho la fila no tiene sentido
    document.getElementById("panelFilaPartos").style.display = pNodo.sexo === "H" ? "" : "none";
    document.getElementById("panelPartos").textContent = pNodo.partos;
    document.getElementById("panelCondicion").textContent = pNodo.estado;

    document.getElementById("panelFicha").href = "/PagesAnimal/DetalleAnimal?id=" + pNodo.id;

    // Recentrar en el animal que ya es la raiz no hace nada, asi que no se ofrece
    var centrar = document.getElementById("panelCentrar");
    centrar.style.display = pRuta === "" ? "none" : "inline-block";
    centrar.onclick = function () {
        centrarArbol(pNodo, true);
    };
}

function limpiarPanel() {
    document.getElementById("panelVacio").style.display = "block";
    document.getElementById("panelDatos").style.display = "none";
}

// Como se llama el parentesco de una rama respecto del animal consultado. A partir
// de la tercera generacion los nombres se multiplican -hay ocho bisabuelos- asi que
// se dice el grado y de que linea viene, que es como se habla en el tambo.
function describirParentesco(pRuta) {
    if (pRuta === "") {
        return "Animal consultado";
    }

    // El ultimo paso del camino dice el sexo -la M final es la madre- y el primero
    // dice por que linea se llega
    var esHembra = pRuta.charAt(pRuta.length - 1) === "M";
    var esLineaMaterna = pRuta.charAt(0) === "M";
    var linea = esLineaMaterna ? "materna" : "paterna";

    if (pRuta.length === 1) {
        return esHembra ? "Madre" : "Padre";
    }

    // El adjetivo concuerda con el abuelo, no con la linea: el padre de la madre es
    // el abuelo materno, no "materna"
    if (pRuta.length === 2) {
        if (esHembra) {
            return esLineaMaterna ? "Abuela materna" : "Abuela paterna";
        }
        return esLineaMaterna ? "Abuelo materno" : "Abuelo paterno";
    }

    if (pRuta.length === 3) {
        return (esHembra ? "Bisabuela" : "Bisabuelo") + " (línea " + linea + ")";
    }

    if (pRuta.length === 4) {
        return (esHembra ? "Tatarabuela" : "Tatarabuelo") + " (línea " + linea + ")";
    }

    return "Ascendiente de " + pRuta.length + "ª generación (línea " + linea + ")";
}

// --------------------------------------------------------------------
// Zoom y vista de tabla
// --------------------------------------------------------------------

function acercarArbol(pPaso) {
    zoomActual = zoomActual + pPaso;

    if (zoomActual < ZOOM_MINIMO) {
        zoomActual = ZOOM_MINIMO;
    }
    if (zoomActual > ZOOM_MAXIMO) {
        zoomActual = ZOOM_MAXIMO;
    }

    aplicarZoom();
}

function restablecerZoom() {
    zoomActual = 1;
    aplicarZoom();
}

function aplicarZoom() {
    document.getElementById("linajeLienzo").style.transform = "scale(" + zoomActual + ")";
    document.getElementById("linajeZoom").textContent = Math.round(zoomActual * 100) + "%";
}

// La tabla sigue estando para imprimir el registro genealogico y para leer el dato
// crudo, pero ya no es la vista principal
function alternarTabla() {
    var tabla = document.getElementById("linajeTabla");
    var boton = document.getElementById("botonTabla");
    var oculta = tabla.style.display === "none";

    tabla.style.display = oculta ? "block" : "none";
    boton.textContent = oculta ? "Ocultar la tabla" : "Ver como tabla";
}
