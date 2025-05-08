document.addEventListener(onload, start());

function start() {
    var tabla = new DataTable('#tablaListado', {
        language: {
            url: "idiomas/en-ES.json"
        }
    });
}
