//************ Funcion  Graficos Dashboard **************************************
$(document).ready(function () {
    $('#loading').show();
    google.charts.load('current', { packages: ['corechart', 'bar'] });
    google.charts.setOnLoadCallback(Grafico_Principal);
    //google.charts.setOnLoadCallback(Chart_Aprob_Director);
    //google.charts.setOnLoadCallback(Chart_Apro_Gerencia);
    //google.charts.setOnLoadCallback(Chart_Cantidad_Seleccion);
    //google.charts.setOnLoadCallback(Chart_Total);
    //google.charts.setOnLoadCallback(Chart_Cubiertos);
    //google.charts.setOnLoadCallback(Chart_Cerrados);
    //google.charts.setOnLoadCallback(Chart_Anulados);
    google.charts.load('current', { 'packages': ['corechart', 'controls'] });
    google.charts.setOnLoadCallback(Chart_Total_Cargo);
    google.charts.setOnLoadCallback(Chart_Total_Mes);
    google.charts.setOnLoadCallback(Chart_Total_Encargado);
    google.charts.setOnLoadCallback(Chart_Total_Motivo);
    google.charts.setOnLoadCallback(Chart_Total_Area);
    google.charts.setOnLoadCallback(Chart_Total_CargosXArea);
    google.charts.setOnLoadCallback(Chart_Total_CargosXMes);    
});
//----------- Graficos Dashboard Consulta General --------------
function Grafico_Principal() {
    try {
        var URLactual = location.origin;
        var pr = location.pathname;
        var url = pr.split('/');
        var ruta = URLactual + "/" + "RequerimientosPersonal" + "/Dashboard_Graficos_Principal";
        $.ajax({
            url: ruta,
            type: "post",
            contentType: "application/json; charset=utf-8",
            dataType: 'json',
            contentType: false,
            processData: false, 
            success: function (Results) {
                if (Results != undefined || Results != null) {
                    Grafico_Solicitados(Results);
                    Chart_Aprob_Director(Results);
                    Chart_Apro_Gerencia(Results);
                    Chart_Cantidad_Seleccion(Results);
                    Chart_Cerrados(Results);
                    Chart_Anulados(Results);
                    Chart_Total(Results);
                    Chart_Cubiertos(Results);
                }
            },
        });
    }
    catch (e) { alertify.alert(e); }
}

//----------------------****************--------------------------
    //************************ Graficos Circulares ***********************

            //----------- Requerimientos Solicitados ---------------------
            function Grafico_Solicitados(Results) {                
                            var tot = Results[0].Total_Circular - Results[0].Cantidad_Solicitados;
                            var data = google.visualization.arrayToDataTable([
                                ['', ''],
                                ['Cantidad Solicitados', Results[0].Cantidad_Solicitados],
                                ['Total Otros Estados', tot],
                            ]);
                            var options = {
                                title: '',
                                pieHole: 0.2,
                                width: 360,
                                height: 200,
                                legend: 'right',
                                pieSliceTextStyle: {
                                    color: 'black',
                                },
                                colors: ['#7addff', '#c1c1c1']
                            };
                            var chart = new google.visualization.PieChart(document.getElementById('Div_Chart_Solicitados'));
                            chart.draw(data, options);                        
            }
            //----------------------****************----------------------
            //----------- Requerimientos AprobadosPor el Director --------
            function Chart_Aprob_Director(Results) {
                            var tot = Results[0].Total_Circular - Results[0].Cantidad_Apro_Director;
                            var data = google.visualization.arrayToDataTable([
                                ['', ''],
                                ['Cantidad Aprobados Director', Results[0].Cantidad_Apro_Director],
                                ['Total Otros Estados', tot],
                            ]);
                            var options = {
                                title: '',
                                pieHole: 0.2,
                                width: 360,
                                height: 200,
                                legend: 'right',
                                pieSliceTextStyle: {
                                    color: 'black',
                                },
                                colors: ['#f0e87c', '#c1c1c1']
                            };
                            var chart = new google.visualization.PieChart(document.getElementById('Div_Chart_Aprob_Director'));
                            chart.draw(data, options);                       
            }
            //----------------------****************----------------------
            //----------- Requerimientos Aprobados por Gerencia ----------
            function Chart_Apro_Gerencia(Results) {               
                            var tot = Results[0].Total_Circular - Results[0].Cantidad_Apro_Gerencia;
                            var data = google.visualization.arrayToDataTable([
                                ['', ''],
                                ['Cantidad Aprobados Dirección Ejecutiva', Results[0].Cantidad_Apro_Gerencia],
                                ['Total Otros Estados', tot],
                            ]);
                            var options = {
                                title: '',
                                pieHole: 0.2,
                                width: 360,
                                height: 200,
                                legend: 'right',
                                pieSliceTextStyle: {
                                    color: 'black',
                                },
                                colors: ['#bb6e6e', '#c1c1c1']
                            };
                            var chart = new google.visualization.PieChart(document.getElementById('Div_Chart_Apro_Gerencia'));
                            chart.draw(data, options);
            }
            //----------------------****************----------------------
            //----------- Requerimientos en Selección --------------------
            function Chart_Cantidad_Seleccion(Results) {                
                            var tot = Results[0].Total_Circular - Results[0].Cantidad_Seleccion;
                            var data = google.visualization.arrayToDataTable([
                                ['', ''],
                                ['Cantidad Selección', Results[0].Cantidad_Seleccion],
                                ['Total Otros Estados', tot],
                            ]);
                            var options = {
                                title: '',
                                pieHole: 0.2,
                                width: 360,
                                height: 200,
                                legend: 'right',
                                pieSliceTextStyle: {
                                    color: 'black',
                                },
                                colors: ['#f5b976', '#c1c1c1']
                            };
                            var chart = new google.visualization.PieChart(document.getElementById('Div_Chart_Cantidad_Seleccion'));
                            chart.draw(data, options);                        
            }
            //----------------------****************----------------------
            //----------- Requerimientos Cerrados ------------------------
            function Chart_Cerrados(Results) {                
                            var tot = Results[0].Total_Circular - Results[0].Cantidad_Cerrados;
                            var data = google.visualization.arrayToDataTable([
                                ['', ''],
                                ['Cantidad Cerrados', Results[0].Cantidad_Cerrados],
                                ['Total Otros Estados', tot],
                            ]);
                            var options = {
                                title: '',
                                pieHole: 0.2,
                                width: 360,
                                height: 200,
                                legend: 'right',
                                pieSliceTextStyle: {
                                    color: 'black',
                                },
                                colors: ['#b7e7d1', '#c1c1c1']
                            };
                            var chart = new google.visualization.PieChart(document.getElementById('Div_Chart_Cant_Cerrados'));
                            chart.draw(data, options);                        
            }
            //----------------------****************----------------------
            //----------- Requerimientos Anulados ------------------------
            function Chart_Anulados(Results) {
                            var tot = Results[0].Total_Circular - Results[0].Cantidad_Anulados;
                            var data = google.visualization.arrayToDataTable([
                                ['', ''],
                                ['Cantidad Cerrados', Results[0].Cantidad_Anulados],
                                ['Total Otros Estados', tot],
                            ]);
                            var options = {
                                title: '',
                                pieHole: 0.2,
                                width: 360,
                                height: 200,
                                legend: 'right',
                                pieSliceTextStyle: {
                                    color: 'black',
                                },
                                colors: ['#cddfff', '#c1c1c1']
                            };
                            var chart = new google.visualization.PieChart(document.getElementById('Div_Chart_Cant_Anulados'));
                            chart.draw(data, options);
            }
            //----------------------****************----------------------
            //----------- Total de Requerimientos ------------------------
            function Chart_Total(Results) {
                            var tot = Results[0].Total_Circular - Results[0].Cantidad_Cerrados;
                            var data = google.visualization.arrayToDataTable([
                                ['', ''],
                                //['Cantidad Cerrados', Results[0].Cantidad_Cerrados],
                                ['Total Requerimientos', Results[0].Total_Circular],
                            ]);
                            var options = {
                                title: '',
                                pieHole: 0.2,
                                width: 360,
                                height: 200,
                                legend: 'right',
                                pieSliceTextStyle: {
                                    color: 'black',
                                },
                                colors: ['#a3c3cd', '#c1c1c1']
                            };
                            var chart = new google.visualization.PieChart(document.getElementById('Div_Chart_Total'));
                            chart.draw(data, options);
            }
            //----------------------****************----------------------
            //------------ Total Cubiertos y Por Cubrir ------------------
            function Chart_Cubiertos(Results) {
                            var data = google.visualization.arrayToDataTable([
                                ['', ''],
                                ['Total Seleccionados', Results[0].Cant_Cubiertos],
                                ['Pendientes por Seleccionar', Results[0].Cant_Por_Cubrir],
                            ]);
                            var options = {
                                title: '',
                                pieHole: 0.2,
                                width: 360,
                                height: 200,
                                legend: 'right',
                                pieSliceTextStyle: {
                                    color: 'black',
                                },
                                colors: ['#3b83bd', '#cccccc']
                            };
                            var chart = new google.visualization.PieChart(document.getElementById('Div_Chart_Cant_Cubiertos'));
                            chart.draw(data, options);
            }

            //----------------------****************----------------------
    //---------------***************************************--------------


    //************************ Graficos Lineales *************************

            //---------------------- Grafico por Cargo --------------------
            function Chart_Total_Cargo() {
                try {
                    var URLactual = location.origin;
                    var pr = location.pathname;
                    var url = pr.split('/');
                    var ruta = URLactual + "/" + "RequerimientosPersonal" + "/Dashboard_Chart_Total_Cargo";
                    $.ajax({
                        url: ruta,
                        type: "post",
                        contentType: "application/json; charset=utf-8",
                        dataType: 'json',
                        contentType: false,
                        processData: false,
                        beforeSend: function () { $('#loading').show(); },
                        complete: function () { $('#loading').hide(); },
                        success: function (Results) {                            
                            if (Results != undefined || Results != null) {
                                var data2 = [['', '']];
                                for (i = 0; i < Results.length; i++) { 
                                    if (i <= 10) {
                                        data2[i + 1] = [Results[i].Total_Por_Cargo_Nomb, Results[i].Total_Por_Cargo_Cont];
                                    } else {
                                    }   
                                    
                                }
                                var datos = new google.visualization.arrayToDataTable(data2);
                                var options = {
                                    width: '-webkit-fill-available',
                                    allowHtml: true,
                                    hAxis: {
                                        title: 'Nombre Cargo'
                                    },
                                    vAxis: {
                                        title: 'Cantidad'
                                    },
                                    colors: ['#3b83bd', '#ffab91'],
                                    bar: { groupWidth: "85%" },
                                    legend: { position: "none" }
                                };
                                var chart = new google.visualization.ColumnChart(document.getElementById('Div_Chart_Total_Cargo'));
                                chart.draw(datos, options);
                            }
                        },
                    });
                }
                catch (e) { alertify.alert(e); }


            }
            //----------------------****************-----------------------
            //----------------------Grafico por Area-----------------------
            function Chart_Total_Area() {
                try {
                    var URLactual = location.origin;
                    var pr = location.pathname;
                    var url = pr.split('/');
                    var ruta = URLactual + "/" + "RequerimientosPersonal" + "/Dashboard_Chart_Total_Area";
                    $.ajax({
                        url: ruta,
                        type: "post",
                        contentType: "application/json; charset=utf-8",
                        dataType: 'json',
                        contentType: false,
                        processData: false,
                        beforeSend: function () { $('#loading').show(); },
                        complete: function () { $('#loading').hide(); },
                        success: function (Results) {
                            if (Results != undefined || Results != null) {
                                var data2 = [['', '']];
                                for (i = 0; i < Results.length; i++) {
                                    if (i <= 10) {
                                        data2[i + 1] = [Results[i].Total_Por_Area_Nomb, Results[i].Total_Por_Area_Cont];
                                    } else {
                                    }                                    
                                }
                                var datos = new google.visualization.arrayToDataTable(data2);
                                var options = {
                                    width: '-webkit-fill-available',
                                    allowHtml: true,
                                    hAxis: {
                                        title: 'Nombre Área'
                                    },
                                    vAxis: {
                                        title: 'Cantidad',
                                        minValue: 0,
                                    },
                                    colors: ['#3b83bd', '#ffab91'],
                                    bar: { groupWidth: "85%" },
                                    legend: { position: 'none' },
                                };
                                var chart = new google.visualization.ColumnChart(document.getElementById('Div_Chart_Total_Area'));
                                chart.draw(datos, options);
                            }
                        },
                    });
                }
                catch (e) { alertify.alert(e); }
            }
            //----------------------****************-----------------------
            //---------------------- Grafico por Mes ----------------------
            function Chart_Total_Mes() {
                try {
                    var URLactual = location.origin;
                    var pr = location.pathname;
                    var url = pr.split('/');
                    var ruta = URLactual + "/" + "RequerimientosPersonal" + "/Dashboard_Chart_Total_Mes";
                    $.ajax({
                        url: ruta,
                        type: "post",
                        contentType: "application/json; charset=utf-8",
                        dataType: 'json',
                        contentType: false,
                        processData: false,
                        success: function (Results) {
                            if (Results != undefined || Results != null) {
                                var t = new google.visualization.DataTable();
                                t.addColumn('string', 'Mes');
                                t.addColumn('number', 'Total');
                                for (var i = 0; i < Results.length; i++) {
                                    t.addRow([Results[i].Total_Por_Mes_Nomb, Results[i].Total_Por_Mes_Cont]);
                                }
                                var c = new google.visualization.ControlWrapper({
                                    'controlType': 'CategoryFilter',
                                    'containerId': 'filter_pormes',
                                    'filterColumnIndex': 0,
                                    'options': { 'filterColumnLabel': 'Mes', 'ui': { 'caption': 'Seleccione...' } },
                                });
                                var d = new google.visualization.Dashboard(document.getElementById('Div_Chart_Total_Mes'));
                                var p = new google.visualization.ChartWrapper({
                                    'chartType': 'ColumnChart',
                                    'containerId': 'Div_Chart_Total_Mes',
                                    'dataTable': t,
                                    'options': {
                                        'pieSliceText': 'Total',
                                        'legend': 'right',
                                        'bar': { groupWidth: "85%" },
                                        'colors': ['#3c84bc']
                                    },
                                    'view': { 'columns': [0, 1] },
                                });
                                d.bind(c, p);
                                d.draw(t);


                                //var data2 = [['Mes', 'Cantidad']];
                                //for (i = 0; i < Results.length; i++) {
                                //    if (i <= 10) {
                                //        data2[i + 1] = [Results[i].Total_Por_Mes_Nomb, Results[i].Total_Por_Mes_Cont];
                                //    } else {

                                //    }
                                    
                                //}
                                //var c = new google.visualization.ControlWrapper({
                                //    'controlType': 'CategoryFilter',
                                //    'containerId': 'filter_pormes',
                                //    'filterColumnIndex': 0,
                                //    'options': { 'filterColumnLabel': 'Mes', 'ui': { 'caption': 'Seleccione...' } },
                                //});
                                //var datos = new google.visualization.arrayToDataTable(data2);
                                //var options = {
                                //    width: '-webkit-fill-available',
                                //    allowHtml: true,
                                //    hAxis: {
                                //        title: 'Mes'
                                //    },
                                //    vAxis: {
                                //        title: 'Cantidad'
                                //    },
                                //    colors: ['#3b83bd', '#ffab91'],
                                //    bar: { groupWidth: "85%" },
                                //    legend: { position: "none" }
                                //};
                                //var chart = new google.visualization.ColumnChart(document.getElementById('Div_Chart_Total_Mes'));
                                ////chart.bind(c, p);
                                ////chart.bind(c, options);
                                //chart.draw(datos,c, options);
                            }
                        },
                    });
                }
                catch (e) { alertify.alert(e); }


            }
            //----------------------****************-----------------------
            //---------------------- Grafico por Encargado ----------------
            function Chart_Total_Encargado() {
                try {
                    var URLactual = location.origin;
                    var pr = location.pathname;
                    var url = pr.split('/');
                    var ruta = URLactual + "/" + "RequerimientosPersonal" + "/Dashboard_Chart_Total_Encargado";
                    $.ajax({
                        url: ruta,
                        type: "post",
                        contentType: "application/json; charset=utf-8",
                        dataType: 'json',
                        contentType: false,
                        processData: false,
                        success: function (Results) {
                            if (Results != undefined || Results != null) {

                                var data2 = [['', '']];
                                for (i = 0; i < Results.length; i++) {
                                    if (i <= 10) {
                                        data2[i + 1] = [Results[i].Total_Por_Enca_Nomb, Results[i].Total_Por_Enca_Cont];
                                    } else {

                                    }
                                    
                                }
                                var datos = new google.visualization.arrayToDataTable(data2);
                                var options = {
                                    width: '-webkit-fill-available',
                                    allowHtml: true,
                                    hAxis: {
                                        title: 'Nombre Encargado'
                                    },
                                    vAxis: {
                                        title: 'Cantidad',
                                        minValue: 0,
                                    },
                                    colors: ['#3b83bd', '#ffab91'],
                                    bar: { groupWidth: "85%" },
                                    legend: { position: 'none' },
                                };
                                var chart = new google.visualization.ColumnChart(document.getElementById('Div_Chart_Total_Encargado'));
                                chart.draw(datos, options);
                            }
                        },
                    });
                }
                catch (e) { alertify.alert(e); }


            }
            //----------------------****************-----------------------
            //---------------------- Grafico por Motivo -------------------
            function Chart_Total_Motivo() {
                try {
                    var URLactual = location.origin;
                    var pr = location.pathname;
                    var url = pr.split('/');
                    var ruta = URLactual + "/" + "RequerimientosPersonal" + "/Dashboard_Chart_Total_Motivo";
                    $.ajax({
                        url: ruta,
                        type: "post",
                        contentType: "application/json; charset=utf-8",
                        dataType: 'json',
                        contentType: false,
                        processData: false,
                        success: function (Results) {
                            if (Results != undefined || Results != null) {
                                var data2 = [['', '']];
                                for (i = 0; i < Results.length; i++) {
                                    if (i <= 10) {
                                        data2[i + 1] = [Results[i].Total_Por_Mtv_Nomb, Results[i].Total_Por_Mtv_Cont];
                                    } else {

                                    }
                                }
                                var datos = new google.visualization.arrayToDataTable(data2);
                                var options = {
                                    width: '-webkit-fill-available',
                                    allowHtml: true,
                                    hAxis: {
                                        title: 'Nombre Motivo'
                                    },
                                    vAxis: {
                                        title: 'Cantidad',
                                        minValue: 0,
                                    },
                                    colors: ['#3b83bd', '#ffab91'],
                                    bar: { groupWidth: "85%" },
                                    legend: { position: 'none' },
                                };
                                var chart = new google.visualization.ColumnChart(document.getElementById('Div_Chart_Total_Motivo'));
                                chart.draw(datos, options);
                            }
                        },
                    });
                }
                catch (e) { alertify.alert(e); }


            }
            //----------------------****************-----------------------
            

            //---------------------- Grafico Cargos por Area --------------
            function Chart_Total_CargosXArea() {
                try {
                    var URLactual = location.origin;
                    var pr = location.pathname;
                    var url = pr.split('/');
                    var ruta = URLactual + "/" + "RequerimientosPersonal" + "/Dashboard_Chart_Total_CargosXArea";
                    $.ajax({
                        url: ruta,
                        type: "post",
                        contentType: "application/json; charset=utf-8",
                        dataType: 'json',
                        contentType: false,
                        processData: false,
                        success: function (Results) {
                            if (Results != undefined || Results != null) {
                                var data2 = [['', '']];
                                for (i = 0; i < Results.length; i++) {
                                    if (i <= 10) {
                                        data2[i + 1] = [Results[i].Total_Cargos_Por_Area_Nomb, Results[i].Total_Cargos_Por_Area_Cont];
                                    } else {

                                    }
                                }
                                var datos = new google.visualization.arrayToDataTable(data2);
                                var options = {
                                    width: '-webkit-fill-available',
                                    allowHtml: true,
                                    hAxis: {
                                        title: 'Nombre Cargo por Área'
                                    },
                                    vAxis: {
                                        title: 'Cantidad',
                                        minValue: 0,
                                    },
                                    colors: ['#3b83bd', '#ffab91'],
                                    bar: { groupWidth: "85%" },
                                    legend: { position: 'none' },
                                };
                                var chart = new google.visualization.ColumnChart(document.getElementById('Div_Chart_Total_CargosXArea'));
                                chart.draw(datos, options);
                            }
                        }
                    });
                }
                catch (e) { alertify.alert(e); }
            }
            //----------------------****************-----------------------
            //---------------------- Grafico Cargos por Mes ---------------
            function Chart_Total_CargosXMes() {
                try {
                    var URLactual = location.origin;
                    var pr = location.pathname;
                    var url = pr.split('/');
                    var ruta = URLactual + "/" + "RequerimientosPersonal" + "/Dashboard_Chart_Total_CargosXMes";
                    $.ajax({
                        url: ruta,
                        type: "post",
                        dataType: 'json',
                        contentType: false,
                        processData: false,
                        success: function (Results) {

                            if (Results != undefined || Results != null) { 
                                var t = new google.visualization.DataTable();
                                t.addColumn('string', 'Mes');
                                t.addColumn('string', 'Cargo');
                                t.addColumn('number', 'Total');
                                for (var i = 0; i < Results.length; i++) {
                                    t.addRow([Results[i].Total_Cargos_Por_Mes_Nomb2, Results[i].Total_Cargos_Por_Mes_Nomb, Results[i].Total_Cargos_Por_Mes_Cont]);
                                }
                                var c = new google.visualization.ControlWrapper({
                                    'controlType': 'CategoryFilter',
                                    'containerId': 'filter_Cargospormes',
                                    'filterColumnIndex': 0,
                                    'options': { 'filterColumnLabel': 'Mes', 'ui': { 'caption': 'Seleccione...' } },
                                });
                                var d = new google.visualization.Dashboard(document.getElementById('Div_Chart_Total_CargosXMes'));
                                var p = new google.visualization.ChartWrapper({
                                    'chartType': 'ColumnChart',
                                    'containerId': 'Div_Chart_Total_CargosXMes',
                                    'dataTable':t,
                                    'options': {
                                        'pieSliceText': 'Total',
                                        'bar': { groupWidth: "85%" },
                                        'legend': 'right',
                                        'colors': ['#3c84bc']
                                    },
                                    'view': { 'columns': [1 + 0, 2] },
                                });
                                d.bind(c, p);                                
                                d.draw(t);
                            }
                        },
                    });
                }
                catch (e) { alertify.alert(e); }
            }
            //----------------------****************-----------------------
     //---------------***************************************--------------

//********************************************************************************

function Cerrar_Modal() {
    $('#Novedades').modal('hide');
}

function Novedad(Id) {
    var model;
    try {
        model = { "Id": Id };

        var datosenviar = JSON.stringify(model);
        var IdUsu = [{ 'IdUsu1': Id, 'IdUsua': Id }];
        var URLactual = location.origin;
        var pr = location.pathname;
        var url = pr.split('/');
        var ruta = URLactual + "/" + "RequerimientosPersonal" + "/Datos_Usuario_Egreso";

        $.ajax({
            url: ruta,
            type: "post",
            contentType: "application/json; charset=utf-8",
            dataType: 'json',
            data: datosenviar,
            success: function (Results) {

                if (Results != undefined || Results != null) {
                    document.getElementById("Cargo2").innerHTML = Results[0].Cargo;
                    document.getElementById("FecContra").innerHTML = Results[0].Fecha;
                    document.getElementById("Nombre").innerHTML = Results[0].Nombre;
                    $('#Novedades').modal('show');
                }
            },
        });
    }
    catch (e) { alertify.alert(e); }
}