
$(document).ready(function () {
    google.charts.load('current', { 'packages': ['corechart', 'controls'] });
    google.charts.setOnLoadCallback(CreateGrphics);
    google.charts.load('current', { packages: ['corechart', 'bar'] });
    google.charts.setOnLoadCallback(GraficaSospechoso);
    google.charts.setOnLoadCallback(GraficaNoSospechoso);
    google.charts.setOnLoadCallback(GraficaVerde);
});

function CreateGrphics() {
    try {
        $.ajax({
            url: "DatosGraficas",
            type: "post",
            success: function (e) {
                if (e != undefined || e != null) {
                    var t = new google.visualization.DataTable();
                    t.addColumn('string', 'Clasificación');
                    t.addColumn('string', 'Variable');
                    t.addColumn('number', 'Cuenta');
                    for (var i = 0; i < e.length; i++) { t.addRow([e[i].type,  e[i].var, e[i].count]); }

                    var c = new google.visualization.ControlWrapper({
                        'controlType': 'CategoryFilter',
                        'containerId': 'filter_div',
                        'filterColumnIndex': 0,
                        'options': { 'filterColumnLabel': 'Clasificación', 'ui': { 'caption': 'Seleccione...' } },
                    });

                    var d = new google.visualization.Dashboard(document.getElementById('dashboard_div'));

                    var p = new google.visualization.ChartWrapper({
                        'chartType': 'PieChart',                        
                        'containerId': 'chart_div',
                        'options': {
                            'width': 600,
                            'height': 300,
                            'pieSliceText': 'Tipo',
                            'legend': 'right'
                        },
                        'view': { 'columns': [1, 2] },
                    });
                    d.bind(c, p);
                    d.draw(t);                   
                }
            },
            error: function (xhr, status, error) { alert(error); }
        });
    }
    catch (e) { alert(e); }
}



function GraficaSospechoso() {
    try {
        $.ajax({
            url: "GraficaSospechosos",
            type: "post",
            success: function (Results) {
                if (Results != undefined || Results != null) {

                    var data2 = [['Clasificacion','Cuenta' ]];
                    for (i = 0; i < Results.length; i++) {
                       
                        data2[i + 1] = [Results[i].variable, Results[i].count];
                    }

                    var datos = new google.visualization.arrayToDataTable(data2);

                    var options = {
                        title: "Reporte como sospechosos",
                        colors: ['#b0120a', '#ffab91'],
                        width: 1000,
                        height: 400,
                        bar: { groupWidth: "95%" },
                        legend: { position: "none" },
                            hAxis: {
                                title: 'Fecha ',
                                minValue: 0
                            },
                            vAxis: {
                                title: 'Cantidad'
                            }
                    };

         
                    var chart = new google.visualization.ColumnChart(document.getElementById('chart_div_sospechoso'));
               
                    chart.draw(datos, options);
                }
            },
            error: function (xhr, status, error) { alert(error); }
        });
    }
    catch (e) { alert(e); }
}

function GraficaNoSospechoso() {
    try {
        $.ajax({
            url: "GraficaNoSospechosos",
            type: "post",
            success: function (Results) {
                if (Results != undefined || Results != null) {

                    var data2 = [['Clasificacion', 'Cuenta']];
                    for (i = 0; i < Results.length; i++) {

                        data2[i + 1] = [Results[i].variable, Results[i].count];
                    }

                    var datos = new google.visualization.arrayToDataTable(data2);

                    var options = {
                        title: "Reporte Alerta Amarilla",
                        colors: ['yellow', '#ffab91'],
                        width: 1000,
                        height: 400,
                        bar: { groupWidth: "95%" },
                        legend: { position: "none" },
                        hAxis: {
                            title: 'Fecha ',
                            minValue: 0
                        },
                        vAxis: {
                            title: 'Cantidad'
                        }
                    };


                    var chart = new google.visualization.ColumnChart(document.getElementById('chart_div_Nosospechoso'));

                    chart.draw(datos, options);
                }
            },
            error: function (xhr, status, error) { alert(error); }
        });
    }
    catch (e) { alert(e); }
}

function GraficaVerde() {
    try {
        $.ajax({
            url: "GraficaVerde",
            type: "post",
            success: function (Results) {
                if (Results != undefined || Results != null) {

                    var data2 = [['Clasificacion', 'Cuenta']];
                    for (i = 0; i < Results.length; i++) {

                        data2[i + 1] = [Results[i].variable, Results[i].count];
                    }

                    var datos = new google.visualization.arrayToDataTable(data2);

                    var options = {
                        title: "Reporte No sospechosos",
                        colors: ['green', '#ffab91'],
                        width: 1000,
                        height: 400,
                        bar: { groupWidth: "95%" },
                        legend: { position: "none" },
                        hAxis: {
                            title: 'Fecha ',
                            minValue: 0
                        },
                        vAxis: {
                            title: 'Cantidad'
                        }
                    };


                    var chart = new google.visualization.ColumnChart(document.getElementById('chart_div_verde'));

                    chart.draw(datos, options);
                }
            },
            error: function (xhr, status, error) { alert(error); }
        });
    }
    catch (e) { alert(e); }
}