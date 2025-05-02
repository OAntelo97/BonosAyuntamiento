document.addEventListener(onload, start());

var tabla;

function start() {
    tabla = new DataTable('#tablaListado');
}

function InitializeDataTableWithLanguage(tableId, languageSettings) {
    $(document).ready(function () {
        $(tableId).DataTable({
            "language": languageSettings
        });
    });
}