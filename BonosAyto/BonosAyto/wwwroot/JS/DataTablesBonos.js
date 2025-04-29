document.addEventListener(onload, start());

var tablaB;

function start() {
    tablaB = new DataTable('#listaBonosBeneficiario');
}



function CheckFilterDate() {
    const fMin = document.getElementById('minDate');
    const fMax = document.getElementById('maxDate');

    const minValue = fMin.value;
    const maxValue = fMax.value;

    if (minValue || maxValue) {
        //filtrar

    }


    if (minValue && maxValue) {
        const minDate = new Date(minValue);
        const MinMax = new Date(minDate);
        MinMax.setDate(minDate.getDate() + 1);
        const maxDate = maxValue ? new Date(maxValue) : null;

        if (maxDate <= minDate) {
            const strung = MinMax.toISOString().split('T')[0];
            maxInput.value = strung;
        }
    }
        if (window.tablaB) {
            window.tablaB.draw(); 
        }
}


