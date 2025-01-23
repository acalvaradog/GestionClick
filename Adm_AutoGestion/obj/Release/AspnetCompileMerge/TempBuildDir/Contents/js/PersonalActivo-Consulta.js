$(document).ready(function () {
    google.charts.load('current', { 'packages': ['table'] });
    google.charts.load("current", { packages: ["calendar"] });
    google.charts.load('current', { 'packages': ['bar'] });
    //google.charts.setOnLoadCallback(GraficarTabla);
  
});



function GraficasNoRealizadas() {
    //var data = JSON.stringify({ 'area': 'area', 'fechainicial': '1', 'fechafinal': '1' });
    var area = document.getElementById("Area").value;
    var fechainicial = document.getElementById("txt_fechainicial").value;
    var fechafinal = document.getElementById("txt_fechafinal").value;
    try {
        $.ajax({
            url: "DatosSinAutoevaluacion",
            data: { 'area': area, 'fechainicial': fechainicial, 'fechafinal': fechafinal },
            type: "post",
            success: function (e) {
                if (e != undefined || e != null) {
                    var data = new google.visualization.DataTable();
                    data.addColumn('string', 'Area');
                    data.addColumn('string', 'Cargo');
                    data.addColumn('string', 'Nombres');
                    for (var i = 0; i < e.length; i++) { data.addRow([e[i].Area, e[i].Cargo, e[i].Nombres]); }

                    var table = new google.visualization.Table(document.getElementById('dashboard_div'));

                    table.draw(data, { showRowNumber: true, width: '100%', height: '100%' });
                }
            },
            error: function (xhr, status, error) { alert(error); }
        });
    }
    catch (e) { alert(e); }
}


function GraficasRealizadas() {
    //var data = JSON.stringify({ 'area': 'area', 'fechainicial': '1', 'fechafinal': '1' });
    var area = document.getElementById("Area").value;
    var fechainicial = document.getElementById("txt_fechainicial").value;
    var fechafinal = document.getElementById("txt_fechafinal").value;
    try {
        $.ajax({
            url: "DatosConAutoevaluacion",
            data: { 'area': area, 'fechainicial': fechainicial, 'fechafinal': fechafinal },
            type: "post",
            success: function (e) {
                if (e != undefined || e != null) {
                    var data = new google.visualization.DataTable();
                    data.addColumn('string', 'Area');
                    data.addColumn('string', 'Cargo');
                    data.addColumn('string', 'Nombres');
                    data.addColumn('string', 'Fecha');
                    for (var i = 0; i < e.length; i++) {
                        var dateValue = e[i].Fecha; // get the value from the object.

                        //update date
                        dateValue = dateValue.replace("/Date(", ""); dateValue = dateValue.replace(")/", "") //strip "/Date(" and ")/"
                        dateValue = new Date(parseInt(dateValue)).toISOString(); //Outputs 1900-01-01T05:00:00.000Z
                        var hoy = new Date(dateValue);

                        var fecha = hoy.getFullYear() + '/' + (hoy.getMonth() + 1) + '/' + hoy.getDate();

                        data.addRow([e[i].Area, e[i].Cargo, e[i].Nombres, fecha]);
                    }

                    var table = new google.visualization.Table(document.getElementById('div_resultados'));

                    table.draw(data, { showRowNumber: true, width: '100%', height: '100%' });
                }
            },
            error: function (xhr, status, error) { alert(error); }
        });
    }
    catch (e) { alert(e); }
}

function TodosPorFecha() {
    //var data = JSON.stringify({ 'area': 'area', 'fechainicial': '1', 'fechafinal': '1' });
    var area = document.getElementById("Area").value;
    var fechainicial = document.getElementById("txt_fechainicial").value;
    var fechafinal = document.getElementById("txt_fechafinal").value;
    try {
        $.ajax({
            url: "TotalPorDias",
            data: { 'area': area },
            type: "post",
            success: function (e) {
                if (e != undefined || e != null) {
                    var data = new google.visualization.DataTable();
                    data.addColumn('date', 'Fecha');
                    data.addColumn('number', 'count');
                
                    for (var i = 0; i < e.length; i++) {
                      
                        var dateValue = e[i].Fecha; // get the value from the object.

                        //update date
                        dateValue = dateValue.replace("/Date(", ""); dateValue = dateValue.replace(")/", "") //strip "/Date(" and ")/"
                        dateValue = new Date(parseInt(dateValue)).toISOString(); //Outputs 1900-01-01T05:00:00.000Z
                        var hoy = new Date(dateValue);
                       
                        data.addRow([new Date(hoy.getFullYear(), hoy.getMonth(), hoy.getDate()), e[i].count]);
                    }

                  
                    var chart = new google.visualization.Calendar(document.getElementById('calendar_basic'));

                    var options = {
                        title: "Total Resultados por dìa",
                        height: 350,
                        calendar: {
                            dayOfWeekLabel: {
                                fontName: 'Times-Roman',
                                fontSize: 12,
                                color: 'green',
                                bold: true,
                                italic: true,
                            },
                            dayOfWeekRightSpace: 10,
                            daysOfWeek: 'DLMMJVS',
                        }
                    };

                    chart.draw(data, options);
                }
            },
            error: function (xhr, status, error) { alert(error); }
        });
    }
    catch (e) { alert(e); }
}

function GraficaSospechoso() {
    var area = document.getElementById("Area").value;
    try {
        $.ajax({
            url: "TotalColores",
            data: { 'area': area },
            type: "post",
            success: function (Results) {
                if (Results != undefined || Results != null) {

         
                  
                  

                    var data2 = [['Fecha', 'Verde', 'Amarillo', 'Rojo']];
                    for (i = 0; i < Results.length; i++) {

                        var dateValue = Results[i].Fecha; // get the value from the object.

                        //update date
                        dateValue = dateValue.replace("/Date(", ""); dateValue = dateValue.replace(")/", "") //strip "/Date(" and ")/"
                        dateValue = new Date(parseInt(dateValue)).toISOString(); //Outputs 1900-01-01T05:00:00.000Z
                        var hoy = new Date(dateValue);

                        var fecha = hoy.getFullYear() + '/' + (hoy.getMonth() + 1) + '/' + hoy.getDate();



                        data2[i + 1] = [fecha, Results[i].Verde, Results[i].Amarillo, Results[i].Rojo];
                    }

                    var datos = new google.visualization.arrayToDataTable(data2);
                    var options = {
                        chart: {
                            title: 'Resultados AutoEvaluaciòn ',
                            subtitle: 'Resultados por fecha resumido',
                        },
                        bars: 'vertical',
                  
                        height: 400,
                        colors: ['green', 'yellow', 'red']
                    };

                    var chart = new google.charts.Bar(document.getElementById('div_totalcolores'));

                    chart.draw(datos, google.charts.Bar.convertOptions(options));
                }
            },
            error: function (xhr, status, error) { alert(error); }
        });
    }
    catch (e) { alert(e); }
}

function MostrarDatos() {

    var area = document.getElementById("Area").value;
    var fechainicial = document.getElementById("txt_fechainicial").value;
    var fechafinal = document.getElementById("txt_fechafinal").value;

    if (area == '' || fechainicial == '' || fechafinal == '') {
        alert('Debe aplicar los filtros para generar la informaciòn');

    } else {
   GraficasNoRealizadas();
    GraficasRealizadas();
    TodosPorFecha();
    GraficaSospechoso();
    }
 
}