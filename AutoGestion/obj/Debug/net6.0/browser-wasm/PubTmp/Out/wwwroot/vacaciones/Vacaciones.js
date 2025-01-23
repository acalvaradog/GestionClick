$(document).ready(function () {
    Vacaciones();
});


document.getElementById('txt_candiassol_vac').addEventListener("keyup", calculardias);
document.getElementById("txt_fechainicialVac").addEventListener("input", calcularfecha);
document.getElementById("txt_fechainicialVac").addEventListener("input", ValidarDias);
document.getElementById("txt_fechainicialVac").addEventListener("input", ValidarFechaini);
document.getElementById("txt_vacacionespagas").addEventListener("input", VacacionesPagas);
document.getElementById("txt_candiassol_vac").addEventListener("input", ValidarDias);
/*document.getElementById("btn_anticipadas_pagas").addEventListener("click", AutorizaAnticipadasPagas);*/
document.getElementById("txt_vacacionespagas").addEventListener("input", validar_Pagas);

/*var servidor = "https://foscal.co/admautogestion";*/
var servidor = "http://localhost:55389";
//var servidor = "http://190.71.21.57/admautogestion";
var Codigo = "";
var Id = "";

var cantdiaspendi = 0;

function validar_Pagas() {

    var Pagas = $("#txt_vacacionespagas").val();
    if (Pagas == "SI") {
        alertify.alert('Recuerde que cuando solicita vacaciones en Dinero, estas pasan a estudio para aprobación por parte de la Jefatura de Gestion Humana.');
    }

}


function Vacaciones() {

    HistorialVacaciones();

    Codigo = new URLSearchParams(window.location.search).get('codigo');
    Id = new URLSearchParams(window.location.search).get('id');

    /*var Id = $("#txt_id").val();*/

    var urlDetalle = servidor + "/api/Vacaciones/Autoriza/" + Id;


    $.ajax({
        url: urlDetalle,
        //data: JSON.stringify(Empleado),
        type: 'GET',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: function (data) {
            if (data == "DsMayorAntPag") {
                $("#AutorizaVacPag").show();
                $("#AutorizaVacAdel").show();
                $("#DiasMayora6").show();
            }
            if (data == "DsMayorAnt") {
                $("#AutorizaVacPag").hide();
                $("#AutorizaVacAdel").show();
                $("#DiasMayora6").show();
            }
            if (data == "DsMayorPag") {
                $("#AutorizaVacPag").show();
                $("#AutorizaVacAdel").hide();
                $("#DiasMayora6").show();
            }
            if (data == "AntPag") {
                $("#AutorizaVacPag").show();
                $("#AutorizaVacAdel").show();
                $("#DiasMayora6").hide();
            }
            if (data == "DsMayor") {
                $("#AutorizaVacPag").hide();
                $("#AutorizaVacAdel").hide();
                $("#DiasMayora6").show();
            }
            if (data == "Ant") {
                $("#AutorizaVacPag").hide();
                $("#AutorizaVacAdel").show();
                $("#DiasMayora6").hide();
            }
            if (data == "Pag") {
                $("#AutorizaVacPag").show();
                $("#AutorizaVacAdel").hide();
                $("#DiasMayora6").hide();
            }
            if (data == "SinAut") {
                $("#AutorizaVacPag").hide();
                $("#AutorizaVacAdel").hide();
                $("#DiasMayora6").hide();
            }

        }
    })

  

    var urlDetalle = servidor + "/api/Historial/" + Codigo;


    $.ajax({
        url: urlDetalle,
        //data: JSON.stringify(Empleado),
        type: 'GET',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: function (data) {

            var datos = data.split(";");

            $("#TabHsVac").html(datos[0]);

            var Resultado = datos[1];

            $("#txt_dias_x_disf").val(Resultado);


            cantdiaspendi = Resultado;
            $("#txt_candiaspen_vac").val(cantdiaspendi);
        }
    })



}


$(function () {
    $("#formuploadajax").on("submit", function (e) {
        e.preventDefault();
        var f = $(this);

        Codigo = new URLSearchParams(window.location.search).get('codigo');
        Id = new URLSearchParams(window.location.search).get('id');
        //var files = $("#archivo1").get(0).files;


        //var formData = new FormData(document.getElementById("formuploadajax"));
        //formData.append("dato", "valor");
        ////formData.append(f.attr("name"), $(this)[0].files[0]);

        try {

            var Pagas = $("#txt_vacacionespagas").val();
            var FecIni = $("#txt_fechainicialVac").val();
            if (FecIni == "") {
                throw 'Para continuar, Primero debe digitar la Fecha Inicial.';
            }

            var FecFin = $("#txt_fechafinalVac").val();
            //if (FecFin == "") {
            //    throw 'Para continuar, Primero debe digitar la Fecha Final.';
            //}

            var CanDiSol = $("#txt_candiassol_vac").val();
            if (CanDiSol == "") {
                throw 'Para continuar, Primero debe digitar la Cantidad de dias solicitados.';
            }

            if (CanDiSol > 7 && Pagas == "SI") {
                throw 'No es posible solicitar mas de 7 dias para disfrutar en dinero.';
            }

            var CanDiPen = $("#txt_candiaspen_vac").val();
            if (CanDiPen == "") {
                throw 'Para continuar, Primero debe digitar la Cantidad de dias pendientes.';
            }

            if (FecFin < FecIni) {
                throw 'La fecha fin de vacaciones deber ser mayor a la fecha inicio de vacaciones.';
            }

            var VacPag = $("#txt_vacacionespagas").val();
            if (VacPag == "") {
                throw 'Para continuar, Primero debe seleccionar si las Vacaciones son pagas.';
            }

            var VacAde = $("#txt_vacacionesadelant").val();
            if (VacAde == "") {
                throw 'Para continuar, Primero debe seleccionar si las Vacaciones son adelantadas.';
            }

            //var Adjun = files.length;
            //if (Adjun == 0) {
            //    throw 'Para continuar, Primero debe adjuntar la carta.';
            //}

            var Pagas = document.getElementById("txt_vacacionespagas").value;
            if (Pagas == "vacio") {

                Pagas = "NO";
            }

            var Adelantadas = document.getElementById("txt_vacacionesadelant").value;
            if (Adelantadas == "vacio") {

                //document.getElementById("txt_vacacionesadelant").value == "NO";
                Adelantadas = "NO";
            }

            var Mayor6 = document.getElementById("txt_vacacionesMayor6").value;
            if (Mayor6 == "vacio") {

                Mayor6 = "NO";
            }

            var Obs = $("#txt_ObsVac").val();
            if (Obs == "") {
                throw 'Para continuar, Primero debe diligenciar el o los periodos a tomar.';
            }




            var frmData = new FormData();
            frmData.append("Vacaciones.EmpleadoId", Id);
            //frmData.append("Vacaciones.EmpleadoId", "3");
            frmData.append("Vacaciones.FechaInicial", document.getElementById("txt_fechainicialVac").value);
            frmData.append("Vacaciones.FechaFin", document.getElementById("txt_fechafinalVac").value);
            frmData.append("Vacaciones.CantDiasSolicitados", document.getElementById("txt_candiassol_vac").value);
            frmData.append("Vacaciones.CantDiasPendientes", document.getElementById("txt_candiaspen_vac").value);
            frmData.append("Vacaciones.VacacionesPagadas", Pagas);
            frmData.append("Vacaciones.VacacionesAdelantadas", Adelantadas);
            frmData.append("Vacaciones.VacacionesDiasMayor6", Mayor6);
            frmData.append("Vacaciones.Observacion", Obs);
            //frmData.append("Adjunto", document.getElementById("formuploadajax"));
            //frmData.append("Adjunto", files[0]);
            frmData.append("CantPendt", cantdiaspendi);





            $.ajax({
                url: servidor + "/api/vacaciones",
                type: 'POST',
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                data: frmData,
                //cache: false,
                contentType: false,
                processData: false,
                beforeSend: function () { $("#processState").modal("show"); },
                success: function (result) {
                    $("#processState").modal("hide");
                    if (result == "true") {
                        alertify.alert('Los datos se registraron correctamente');
                        var archivo = document.getElementById("archivo1");
                        if (archivo != null)
                        {
                            archivo.value = "";
                        }
                        
                        /* atras();*/
                        setTimeout(function () { window.location.reload(true) }, 2500);
                    } else {
                        $("#processState").modal("hide");
                        alertify.alert(result);
                    }
                },
            })


                .done(function (res) {
                    $("#mensaje").html("Respuesta: " + res);
                });

        } catch (err) {
            $("#processState").modal("hide");
            alertify.alert(err);

        };
    });
});

function EstadoVacaciones() {

    $("#form_login").hide();
    $("#menu").hide();
    $("#solicitudes").hide();
    $("#desprendible").hide();
    $("#noticias").hide();
    $("#historial").hide();
    $("#preguntas").hide();
    $("#tbl_pregunta").hide();
    $("#atras_img_pregunta").hide();
    $("#tbl_respuesta").hide();
    $("#atras_img_respuesta").hide();
    $("#FrmVacaciones").hide();
    $("#Menu-Vacaciones").hide();
    $("#Estado_vacaciones").show();
    $("#Historial_Vac").hide();
    $("#FrmSolicitarAntPag").hide();


    //var Id = new FormData();
    //Id.append("Vacaciones.EmpleadoId", document.getElementById("txt_id").value);
    var Id = $("#txt_id").val();

    var urlDetalle = servidor + "/api/Vacaciones/Consulta/" + Id;

    $.ajax({
        url: urlDetalle,
        //data: JSON.stringify(Empleado),
        type: 'GET',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: function (data) {

            if (data != "null") {

                var DivDatos = data.split(';')
                var Datos = DivDatos[0].split(',');
                if (Datos != "null") {
                    var Datos2 = JSON.parse(Datos);

                    var fecha1 = new Date();
                    var dia = fecha1.getDate();
                    var mes = fecha1.getMonth() + 1;
                    var anio = fecha1.getFullYear();
                    if (mes < 10) { mes = "0" + mes };
                    if (dia < 10) { dia = "0" + dia };
                    var fechaactual = anio + "-" + mes + "-" + dia;

                    var fecha = Datos2.Fecha.split('T');
                    var fechaini = Datos2.FechaInicial.split('T');
                    var fechafin = Datos2.FechaFin.split('T');

                    var Result = "<tr><td class = 'table-info'><strong>Fecha de Solicitud</strong></td><td>" + fecha[0] + "</td></tr>";
                    Result += "<tr><td class = 'table-info'><strong>Fecha de Inicio</strong></td><td>" + fechaini[0] + "</td></tr>";
                    Result += "<tr><td class = 'table-info'><strong>Fecha de Reintegro</strong></td><td>" + fechafin[0] + "</td></tr>";
                    Result += "<tr><td class = 'table-info'><strong>Cantidad Dias Solicitados</strong></td><td>" + Datos2.CantDiasSolicitados + "</td></tr>";
                    Result += "<tr><td class = 'table-info'><strong>Cantidad Dias Pendientes</strong></td><td>" + Datos2.CantDiasPendientes + "</td></tr>";
                    Result += "<tr><td class = 'table-info'><strong>Vacaciones Pagadas</strong></td><td>" + Datos2.VacacionesPagadas + "</td></tr>";
                    Result += "<tr><td class = 'table-info'><strong>Vacaciones Anticipadas</strong></td><td>" + Datos2.VacacionesAdelantadas + "</td></tr>";
                    Result += "<tr><td class = 'table-info'><strong>Vacaciones Menores a 6 Dias</strong></td><td>" + Datos2.VacacionesDiasMayor6 + "</td></tr>";

                    if (Datos2.EstadoVacaciones.Nombre == "Solicitado") {
                        if (fecha[0] == fechaactual) {
                            Result += "<tr><td class = 'table-info'><strong>Estado</strong></td><td ><strong> " + Datos2.EstadoVacaciones.Nombre + " </strong></td></tr><tr><td>Favor tener en cuenta que la opcion Anular solo estara disponible durante el dia en el que se realizó el registro.</td><td style='text-align:center'><input type='button' value='Anular' onclick='AnularVac(" + Datos2.Id + ")' class='btn btn-primary'/></td></tr";
                        }
                        else {
                            Result += "<tr><td class = 'table-info'><strong>Estado</strong></td><td ><strong> " + Datos2.EstadoVacaciones.Nombre + " </strong></td></tr";
                        }
                    } else if (Datos2.EstadoVacaciones.Nombre == "Aprobado Jefe Inmediato") {
                        Result += "<tr><td class = 'table-info'><strong>Estado</strong></td><td bgcolor= '#FAAC41'><strong> " + Datos2.EstadoVacaciones.Nombre + " </strong></td></tr";
                    } else if (Datos2.EstadoVacaciones.Nombre == "Recibido G.H") {
                        Result += "<tr><td class = 'table-info'><strong>Estado</strong></td><td bgcolor= '#FCFA81'><strong> " + Datos2.EstadoVacaciones.Nombre + " </strong></td></tr";
                    } else if (Datos2.EstadoVacaciones.Nombre == "Confirmado") {
                        Result += "<tr><td class = 'table-info'><strong>Estado</strong></td><td bgcolor= '#BFFC99'><strong> " + Datos2.EstadoVacaciones.Nombre + " </strong></td></tr";
                    } else if (Datos2.EstadoVacaciones.Nombre == "Rechazado") {
                        Result += "<tr><td class = 'table-info'><strong>Estado</strong></td><td bgcolor= '#FBACAC'><strong> " + Datos2.EstadoVacaciones.Nombre + " </strong></td></tr";
                    } else if (Datos2.EstadoVacaciones.Nombre == "En Aprobaciòn Jefe G.H") {
                        Result += "<tr><td class = 'table-info'><strong>Estado</strong></td><td bgcolor= '#B6DCFD'><strong> " + Datos2.EstadoVacaciones.Nombre + " </strong></td></tr";
                    } else if (Datos2.EstadoVacaciones.Nombre == "Aprobado Jefe Gestion Humana") {
                        Result += "<tr><td class = 'table-info'><strong>Estado</strong></td><td bgcolor= '#ACA4FC'><strong> " + Datos2.EstadoVacaciones.Nombre + " </strong></td></tr";
                    }

                }

                // llena tabla hisotirico en dinero

                var Dato = DivDatos[1].split(',');
                if (Dato != "null") {
                    var Datos3 = JSON.parse(Dato);

                    var fecha2 = Datos3.Fecha.split('T');
                    var fechaini2 = Datos3.FechaInicial.split('T');
                    var fechafin2 = Datos3.FechaFin.split('T');

                    var Result1 = "<tr><td class = 'table-info'><strong>Fecha de Solicitud</strong></td><td>" + fecha2[0] + "</td></tr>";
                    Result1 += "<tr><td class = 'table-info'><strong>Fecha de Inicio</strong></td><td>" + fechaini2[0] + "</td></tr>";
                    Result1 += "<tr><td class = 'table-info'><strong>Fecha de Reintegro</strong></td><td>" + fechafin2[0] + "</td></tr>";
                    Result1 += "<tr><td class = 'table-info'><strong>Cantidad Dias Solicitados</strong></td><td>" + Datos3.CantDiasSolicitados + "</td></tr>";
                    Result1 += "<tr><td class = 'table-info'><strong>Cantidad Dias Pendientes</strong></td><td>" + Datos3.CantDiasPendientes + "</td></tr>";
                    Result1 += "<tr><td class = 'table-info'><strong>Vacaciones Pagadas</strong></td><td>" + Datos3.VacacionesPagadas + "</td></tr>";
                    Result1 += "<tr><td class = 'table-info'><strong>Vacaciones Anticipadas</strong></td><td>" + Datos3.VacacionesAdelantadas + "</td></tr>";
                    Result1 += "<tr><td class = 'table-info'><strong>Vacaciones Menores a 6 Dias</strong></td><td>" + Datos3.VacacionesDiasMayor6 + "</td></tr>";

                    if (Datos3.EstadoVacaciones.Nombre == "Solicitado") {
                        if (fecha2[0] == fechaactual) {
                            Result1 += "<tr><td class = 'table-info'><strong>Estado</strong></td><td ><strong> " + Datos3.EstadoVacaciones.Nombre + " </strong></td></tr><tr><td>Favor tener en cuenta que la opcion Anular solo estara disponible durante el dia en el que se realizó el registro.</td><td style='text-align:center'><input type='button' value='Anular' onclick='AnularVac(" + Datos3.Id + ")' class='btn btn-primary'/></td></tr";
                        }
                        else {
                            Result1 += "<tr><td class = 'table-info'><strong>Estado</strong></td><td ><strong> " + Datos3.EstadoVacaciones.Nombre + " </strong></td></tr";
                        }
                    } else if (Datos3.EstadoVacaciones.Nombre == "Aprobado Jefe Inmediato") {
                        Result1 += "<tr><td class = 'table-info'><strong>Estado</strong></td><td bgcolor= '#FAAC41'><strong> " + Datos3.EstadoVacaciones.Nombre + " </strong></td></tr";
                    } else if (Datos3.EstadoVacaciones.Nombre == "Recibido G.H") {
                        Result1 += "<tr><td class = 'table-info'><strong>Estado</strong></td><td bgcolor= '#FCFA81'><strong> " + Datos3.EstadoVacaciones.Nombre + " </strong></td></tr";
                    } else if (Datos3.EstadoVacaciones.Nombre == "Confirmado") {
                        Result1 += "<tr><td class = 'table-info'><strong>Estado</strong></td><td bgcolor= '#BFFC99'><strong> " + Datos3.EstadoVacaciones.Nombre + " </strong></td></tr";
                    } else if (Datos3.EstadoVacaciones.Nombre == "Rechazado") {
                        Result1 += "<tr><td class = 'table-info'><strong>Estado</strong></td><td bgcolor= '#FBACAC'><strong> " + Datos3.EstadoVacaciones.Nombre + " </strong></td></tr";
                    } else if (Datos3.EstadoVacaciones.Nombre == "En Aprobaciòn Jefe G.H") {
                        Result1 += "<tr><td class = 'table-info'><strong>Estado</strong></td><td bgcolor= '#B6DCFD'><strong> " + Datos3.EstadoVacaciones.Nombre + " </strong></td></tr";
                    } else if (Datos3.EstadoVacaciones.Nombre == "Aprobado Jefe Gestion Humana") {
                        Result1 += "<tr><td class = 'table-info'><strong>Estado</strong></td><td bgcolor= '#ACA4FC'><strong> " + Datos3.EstadoVacaciones.Nombre + " </strong></td></tr";
                    }
                }


            } else {

                Result = "<div class='container'><div class='alert alert-info'><strong>Actualmente no cuenta con registro de solicitud de vacaciones. </strong> </div></div>";
                Result1 = "<div class='container'><div class='alert alert-info'><strong>Actualmente no cuenta con registro de solicitud de vacaciones. </strong> </div></div>";
            }

            $("#TabEstVacaciones").html(Result);
            $("#TabEstVacacionesDin").html(Result1);
            $("#txt_prueba").val(data[0]);
        }
    })

}


function ValidarFechaini() {

    var respuesta = "";
    var fechaini = $("#txt_fechainicialVac").val();
    var fecha = new Date();
    var dia = fecha.getDate();
    var mes = fecha.getMonth() + 1;
    var anio = fecha.getFullYear();
    if (mes < 10) { mes = "0" + mes };
    if (dia < 10) { dia = "0" + dia };
    var fechaactual = anio + "-" + mes + "-" + dia;


    try {

        if (fechaini != "" && fechaini < fechaactual) {
            document.getElementById("txt_fechainicialVac").value = "";
            document.getElementById("txt_fechafinalVac").value = "";
            throw 'No es posible seleccionar una fecha anterior a la actual.';
        }



        var urlDetalle = servidor + "/api/validarFechaini/" + fechaini;

        $.ajax({
            url: urlDetalle,
            //data: JSON.stringify(Empleado),
            type: 'GET',
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (data) {

                if (data == "Error") {

                    respuesta = "No se permite la seleccion de Sabados, Domingos ni festivos";
                    document.getElementById("txt_fechainicialVac").value = "";
                    document.getElementById("txt_fechafinalVac").value = "";
                    alertify.alert(respuesta);

                } else {
                    calcularfecha()


                }


            }
        })

    } catch (err) {
        alertify.alert(err);

    };


}



function ValidarDias() {

 
    var CantSolt = $("#txt_candiassol_vac").val();
    var CantPendt = $("#txt_candiaspen_vac").val();
    var pendientes = parseInt(cantdiaspendi, 0);
    var Menor6 = $("#txt_vacacionesMayor6").val();
    var adelantadas = $("#txt_vacacionesadelant").val();
    var datos2;


    var urlDetalle = servidor + "/api/ValidarDias/" + Codigo;
    try {

        if (adelantadas == "NO" || adelantadas == "vacio") {
            adelantadas = "NO";

            if (CantSolt > pendientes) {
                document.getElementById("txt_fechainicialVac").value = "";
                document.getElementById("txt_fechafinalVac").value = "";
                throw 'No es posible solicitar mas dias de los que tiene pendientes.';
            }
        }

        if (Menor6 == "NO" || Menor6 == "vacio") {
            Menor6 = "NO";
            $.ajax({
                url: urlDetalle,
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: function (data) {

                    var datos = data.split(",");
                    if (datos.length > 1) {
                        //var cant = datos.length - 1;
                        // datos2 = datos[cant].split("-");
                        datos2 = datos;
                    } else if (datos.length == 1) {
                        datos2 = datos[0].split("-");
                    }

                    try {

                        if (adelantadas == "SI") {
                            if (pendientes >= 15) {
                                if (parseInt(datos2[0]) < 15 && parseInt(datos2[1]) == 0) {
                                    var dif = 15 - parseInt(datos2[0]);
                                    var dias = pendientes;
                                    pendientes = pendientes + dif;
                                    if (CantSolt > pendientes) {
                                        var dias = pendientes + 6;
                                    }
                                    //var dias = pendientes;
                                } else {
                                    var dias = 15 + 6;
                                }

                            } if (pendientes < 15) {

                                if (parseInt(datos2[0]) < 15 && parseInt(datos2[1]) == 0) {
                                    if (CantSolt == 15) {
                                        var diferencia = 15 - pendientes;
                                        var dias = pendientes + diferencia;
                                        datos[0] = parseInt(datos[0]) + diferencia;
                                    } else if (CantSolt < 15) {
                                        datos[0] = CantSolt;
                                    } else { var dias = 15 + 6; }
                                }

                            }
                            //else { var dias = pendientes + 6;  }


                            if (CantSolt < dias) {
                                document.getElementById("txt_fechainicialVac").value = "";
                                document.getElementById("txt_fechafinalVac").value = "";
                                throw 'No es posible solicitar menos de 6 dias para disfrutar.';
                            }


                        } else if (datos2[1] == 0 && CantSolt < 6 && CantSolt > 0) {
                            document.getElementById("txt_fechainicialVac").value = "";
                            document.getElementById("txt_fechafinalVac").value = "";
                            throw 'No es posible solicitar menos de 6 dias para disfrutar.';
                        }

                        if (parseInt(datos[0]) == 15 && CantSolt < 6 && CantSolt > 0) {
                            document.getElementById("txt_fechainicialVac").value = "";
                            document.getElementById("txt_fechafinalVac").value = "";
                            throw 'No es posible solicitar menos de 6 dias para disfrutar.';
                        }

                        if (parseInt(datos[0]) <= 15 && CantSolt > parseInt(datos[0]) && adelantadas == "NO") {


                            if (parseInt(datos[1]) != "") {
                                var dif = CantSolt - parseInt(datos[0]);
                                if (dif < 6) {
                                    document.getElementById("txt_fechainicialVac").value = "";
                                    document.getElementById("txt_fechafinalVac").value = "";
                                    throw 'No es posible solicitar menos de 6 dias de un nuevo periodo para disfrutar.';
                                }
                                if (dif > parseInt(datos[1])) {
                                    if (parseInt(datos[2]) != "") {
                                        var dif2 = dif - parseInt(datos[1]);
                                        if (dif2 < 6) {
                                            document.getElementById("txt_fechainicialVac").value = "";
                                            document.getElementById("txt_fechafinalVac").value = "";
                                            throw 'No es posible solicitar menos de 6 dias de un nuevo periodo para disfrutar.';
                                        }
                                        if (dif2 > parseInt(datos[2])) {
                                            if (parseInt(datos[3]) != "") {
                                                var dif3 = dif - parseInt(datos[2]);
                                                if (dif3 < 6) {
                                                    document.getElementById("txt_fechainicialVac").value = "";
                                                    document.getElementById("txt_fechafinalVac").value = "";
                                                    throw 'No es posible solicitar menos de 6 dias de un nuevo periodo para disfrutar.';
                                                }
                                            }
                                        }
                                    }
                                }

                            } else {
                                document.getElementById("txt_fechainicialVac").value = "";
                                document.getElementById("txt_fechafinalVac").value = "";
                                throw 'No es posible solicitar mas dias de los que tiene pendientes.';
                            }
                        }
                    } catch (err) {
                        alertify.alert(err);

                    };
                    var fechaini = $("#txt_fechainicialVac").val();
                    if (fechaini != "") {
                        ValidarFechaini();
                    }
                }
            })

        } else {
            var fechaini = $("#txt_fechainicialVac").val();
            if (fechaini != "") {
                ValidarFechaini();
            }
        }

    } catch (err) {
        alertify.alert(err);

    };


}

function HistorialVacaciones() {




  

    var urlDetalle = servidor + "/api/Historial/" + Codigo;


    $.ajax({
        url: urlDetalle,
        //data: JSON.stringify(Empleado),
        type: 'GET',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: function (data) {

            var datos = data.split(";");

            $("#TabHistVacaciones").html(datos[0]);

            var Resultado = datos[1];

            $("#txt_dias_x_disf").val(Resultado);


            cantdiaspendi = Resultado;
            $("#txt_candiaspen_vac").val(cantdiaspendi);
        }
    })

}

//function AutorizaAnticipadas() {
//    $("#form_login").hide();
//    $("#menu").hide();
//    $("#solicitudes").hide();
//    $("#desprendible").hide();
//    $("#noticias").hide();
//    $("#historial").hide();
//    $("#preguntas").hide();
//    $("#tbl_pregunta").hide();
//    $("#atras_img_pregunta").hide();
//    $("#tbl_respuesta").hide();
//    $("#atras_img_respuesta").hide();
//    $("#FrmVacaciones").hide();
//    $("#Menu-Vacaciones").hide();
//    $("#Estado_vacaciones").hide();
//    $("#Historial_Vac").hide();
//    $("#FrmSolicitarAntPag").show();

//}



//$(function () {
//    $("#formSolicitarVacAntPag").on("submit", function (e) {

//        try {
//            e.preventDefault();
//            var f = $(this);
//            var Id = new URLSearchParams(window.location.search).get('id');

//            //        try{

//            //       var Dinero = (document.getElementById("SltDinero").value);
//            //        var Anticipadas = (document.getElementById("SltAnticipadas").value);
//            //        var Mayor6 = (document.getElementById("SltMayor6").value);
//            //        var EmpleadoId = (document.getElementById("txt_id").value);

//            //        var urlDetalle = servidor + "/api/AutorizaAnticipadasPagas/" + Dinero + "/" + Anticipadas + "/" + Mayor6 + "/" + EmpleadoId;

//            //        $.ajax({
//            //            url: urlDetalle,
//            //            //data: JSON.stringify(Empleado),
//            //            type: 'GET',
//            //            contentType: 'application/json; charset=utf-8',
//            //            dataType: 'json',
//            //            success: function (result) {
//            //                //$("#processState").modal("hide");
//            //                //if (result == "true") {
//            //                //    alertify.alert('Los datos se registraron correctamente');

//            //                //    atras();
//            //                //} else {
//            //                //    $("#processState").modal("hide");
//            //                //    alertify.alert(result);
//            //                //}
//            //            }
//            //        })


//            var frmData = new FormData();
//            frmData.append("Autoriza.EmpleadoId", Id);
//            frmData.append("Autoriza.Dinero", document.getElementById("SltDinero").value);
//            frmData.append("Autoriza.Anticipadas", document.getElementById("SltAnticipadas").value);
//            frmData.append("Autoriza.Mayor6", document.getElementById("SltMayor6").value);

//            $.ajax({
//                url: servidor + "/api/AutorizaAnticipadasPagas",
//                type: 'POST',
//                contentType: 'application/json; charset=utf-8',
//                dataType: 'json',
//                data: frmData,
//                contentType: false,
//                processData: false,
//                //beforeSend: function () { $("#processState").modal("show"); },
//                success: function (result) {

//                    if (result == "true") {
//                        //$("#processState").modal("hide");
//                        alertify.alert('Su solicitud se registro correctamente');
//                        //atras_Vacaciones();
//                    } else {
//                        //$("#processState").modal("hide");
//                        alertify.alert(result);
//                    }
//                },
//            })


//                .done(function (res) {
//                    $("#mensaje").html("Respuesta: " + res);
//                });


//        }

//        catch (err)
//        {
//            alertify.alert("Error: " +err);
//        }
        

//        //    } catch (err) {
//        //            $("#processState").modal("hide");
//        //    alertify.alert(err);
//        //}

//    });
//});







function calculardias() {

    var solicitados = parseInt($("#txt_candiassol_vac").val(), 0);
    document.getElementById("txt_fechainicialVac").value = "";
    document.getElementById("txt_fechafinalVac").value = "";
    var pendientes = parseInt(cantdiaspendi, 0);
    var pagas = $("#txt_vacacionespagas").val();
    var fecha = new Date();
    var dia = fecha.getDate();
    var mes = fecha.getMonth() + 1;
    var anio = fecha.getFullYear();
    if (mes < 10) { mes = "0" + mes };
    if (dia < 10) { dia = "0" + dia };
    var fechaactual = anio + "-" + mes + "-" + dia;


    if (solicitados > 0) {
        var calcular = pendientes - solicitados;
        $("#txt_candiaspen_vac").val(calcular);
        calcular = "";
        if (pagas == "SI") {
            VacacionesPagas()
        }

    } else {
        $("#txt_candiaspen_vac").val(cantdiaspendi);
    }
}

function calcularfecha() {

    var fechaini = $("#txt_fechainicialVac").val();

    var diferencia = parseInt($("#txt_candiassol_vac").val(), 0);

    try {



        if (isNaN(diferencia)) {
            throw 'Para continuar, Primero debe digitar la cantidad de dias que va a solicitar.';
        }

        var urlDetalle = servidor + "/api/Festivos/" + fechaini + "/" + diferencia;

        $.ajax({
            url: urlDetalle,
            //data: JSON.stringify(Empleado),
            type: 'GET',
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            success: function (data) {

                var fecha = data.split(" ");
                var fecha1 = fecha[0].split("/");
                var mes = fecha1[1];
                var dia = fecha1[0];
                var anio = fecha1[2];
                var fechafin = anio + "-" + mes + "-" + dia;

                $("#txt_fechafinalVac").val(fechafin);
                //document.getElementById('txt_fechafinalVac').value = fecha[0];

            }
        })


    } catch (err) {

        alertify.alert(err);

    };

}


function VacacionesPagas() {

    var pagas = $("#txt_vacacionespagas").val();
    var fecha = new Date();
    var dia = fecha.getDate();
    var mes = fecha.getMonth();
    var anio = fecha.getFullYear();



    if (pagas == "SI") {

        if (dia > 0 && dia <= 20) {

            if (mes == 1) {
                var fechaini = anio + "-" + "0" + (mes + 1) + "-" + 28;
                var fechafin = anio + "-" + "0" + (mes + 1) + "-" + 28;
            } else {

                var cantdias = (new Date(mes, anio, 0)).getDate();

                if (cantdias >= 30) {
                    if ((mes + 1) < 10) {
                        var fechaini = anio + "-" + "0" + (mes + 1) + "-" + 30;
                        var fechafin = anio + "-" + "0" + (mes + 1) + "-" + 30;
                    } else {
                        var fechaini = anio + "-" + (mes + 1) + "-" + 30;
                        var fechafin = anio + "-" + (mes + 1) + "-" + 30;
                    }

                    $("#txt_fechainicialVac").val(fechaini);
                    $('#txt_fechainicialVac').attr('disabled', 'disabled');
                    $("#txt_fechafinalVac").val(fechafin);
                } else if (cantdias < 30) {
                    if ((mes + 1) < 10) {
                        var fechaini = anio + "-" + "0" + (mes + 1) + "-" + 28;
                        var fechafin = anio + "-" + "0" + (mes + 1) + "-" + 28;
                    } else {
                        var fechaini = anio + "-" + (mes + 1) + "-" + 28;
                        var fechafin = anio + "-" + (mes + 1) + "-" + 28;
                    }
                }
            }

            $("#txt_fechainicialVac").val(fechaini);
            $('#txt_fechainicialVac').attr('disabled', 'disabled');
            $("#txt_fechafinalVac").val(fechafin);


        } else {

            var cantdias = (new Date((mes + 1), anio, 0)).getDate();

            if (mes == 11) {

                mes = "01";
                anio = anio + 1;

                var cantdias = (new Date(mes, anio, 0)).getDate();

                if (cantdias >= 30) {
                    if ((mes + 1) < 10) {
                        var fechaini = anio + "-" + "0" + mes + "-" + 30;
                        var fechafin = anio + "-" + "0" + mes + "-" + 30;
                    } else {
                        var fechaini = anio + "-" + mes + "-" + 30;
                        var fechafin = anio + "-" + mes + "-" + 30;
                    }

                    $("#txt_fechainicialVac").val(fechaini);
                    $('#txt_fechainicialVac').attr('disabled', 'disabled');
                    $("#txt_fechafinalVac").val(fechafin);
                } else if (cantdias < 30) {
                    if ((mes + 1) < 10) {
                        var fechaini = anio + "-" + "0" + mes + "-" + 28;
                        var fechafin = anio + "-" + "0" + mes + "-" + 28;
                    } else {
                        var fechaini = anio + "-" + (mes + 1) + "-" + 28;
                        var fechafin = anio + "-" + (mes + 1) + "-" + 28;
                    }

                    $("#txt_fechainicialVac").val(fechaini);
                    $('#txt_fechainicialVac').attr('disabled', 'disabled');
                    $("#txt_fechafinalVac").val(fechafin);
                }


            } else {

                mes = mes + 1;

                if (mes == 1) {
                    var fechaini = anio + "-" + "0" + (mes + 1) + "-" + 28;
                    var fechafin = anio + "-" + "0" + (mes + 1) + "-" + 28;
                } else {
                    if ((mes + 1) < 10) {
                        var fechaini = anio + "-" + "0" + (mes + 1) + "-" + 30;
                        var fechafin = anio + "-" + "0" + (mes + 1) + "-" + 30;
                    } else {
                        var fechaini = anio + "-" + (mes + 1) + "-" + 30;
                        var fechafin = anio + "-" + (mes + 1) + "-" + 30;
                    }

                }

                $("#txt_fechainicialVac").val(fechaini);
                $('#txt_fechainicialVac').attr('disabled', 'disabled');
                $("#txt_fechafinalVac").val(fechafin);

            }

        }

    } else {
        $('#txt_fechainicialVac').attr('disabled', false);
        document.getElementById("txt_fechafinalVac").value = "";
    }

}



function AnularVac(id) {

    console.log("este es el id" + id);

    alertify.set({
        labels: {
            ok: "Si",
            cancel: "No"
        }
    });

    alertify.confirm("Esta seguro que desea anular el registro?", function (e) {
        if (e) {

            var urlDetalle = servidor + "/api/Vacaciones/AnularRegistro/" + id;

            $.ajax({
                url: urlDetalle,
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: function (data) {
                    var objdata = $.parseJSON(data);

                    try {

                        if (objdata == "True") {
                            throw 'El registro fue anulado correctamente.';
                        }



                        if (objdata == "False") {
                            throw 'No fue posible anular el registro.';
                        }



                    } catch (err) {
                        alertify.set({
                            labels: {
                                ok: "Ok",
                                cancel: "No"
                            }
                        });
                        alertify.alert(err);

                        EstadoVacaciones()


                    };


                }
            });


        } else {

        }

    });

}
