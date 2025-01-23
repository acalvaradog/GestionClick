$(document).ready(function () {
    $('#Vacaciones').css('cursor', 'pointer');
    $('#Incapacidades').css('cursor', 'pointer');
    $('#Permisos').css('cursor', 'pointer');
    $('#Procesos').css('cursor', 'pointer');
    $('#VPendientes').css('cursor', 'pointer');
    $('#Vacaciones').click(function ()
    { window.location.href = '../Vacaciones/AprobacionSuperior?All=SI'; });
    $('#Incapacidades').click(function ()
    { window.location.href = '../Incapacidades/Informe?FechaIni=&FechaFin=&Empleado=&CodigoEmpleado=&Empresa=&Estado=1'; });
    $('#Permisos').click(function ()
    { window.location.href = '../Permisos/AprobacionPer'; });
    $('#Procesos').click(function ()
    { window.location.href = '../ProcesoDisciplinario/DetalleProcesoDisciplinario1?EmpleadoI=&FechaProcesoI=&FechaProcesoF=&NmrProceso=&Estado=Cerrado'; });
    $('#VPendientes').click(function () { window.location.href = '../Viaticos/InformeViaticos'; });

});
//google.charts.load('current', { 'packages': ['bar'] });
//google.charts.setOnLoadCallback(drawChart);




//function drawChart() {

//    try {

//        var fechainicial = document.getElementById("FechaInicial").value;
//        var fecha = new Date();
//        var dia = fecha.getDate();
//        var mes = fecha.getMonth() + 1;
//        var anio = fecha.getFullYear();
//        if (mes < 10) { mes = "0" + mes };
//        if (dia < 10) { dia = "0" + dia };
//        var fechaactual = anio + "-" + mes + "-" + dia;
//        fecha = fechainicial;

//        if (fechainicial == "")
//        {
//            fecha = fechaactual;
//        }
//        if (fechainicial != "" && fechainicial > fechaactual) {
//            document.getElementById("FechaInicial").value = "";
//            throw 'No es posible seleccionar una fecha mayor a la actual.';
//        }
        
       
            


//        $.ajax({
//            url: "DatosGraficas",
//            data: { 'fecha': fecha },
//            type: "post",
//            success: function (e) {
//                if (e != undefined || e != null) {
//                    var t = new google.visualization.DataTable();
//                    t.addColumn('string', 'Area');
//                    t.addColumn('number', 'Personal Con Autoevaluación');
//                    t.addColumn('number', 'Personal Sin Autoevaluación');
                    
//                    for (var i = 0; i < e.length; i++) { t.addRow([e[i].type, e[i].countsi, e[i].countno]); }

//                    var options = {
//                        chart: {
//                            title: 'Personal que Presentó Autoevaluacion COVID-19',  
//                        },
//                        colors: ['#16A8C2', '#AEEB55'],
//                    };

//                    var d = new google.charts.Bar(document.getElementById('TotalxArea'));
//                    d.draw(t, google.charts.Bar.convertOptions(options));                   
//                }

//            },
//            error: function (xhr, status, error) { alert(error); }
//        });


//    }
//    catch (e) { alert(e); }
//}