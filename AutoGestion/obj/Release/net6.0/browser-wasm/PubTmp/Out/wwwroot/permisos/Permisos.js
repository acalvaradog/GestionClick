
document.getElementById("txt_fechafinpermiso").addEventListener("input", ValidarDias);
document.getElementById("txt_MotivoPermiso").addEventListener("input", ValidarBono);
document.getElementById("txt_MotivoPermiso").addEventListener("input", ValidarSalarioEmocional);
document.getElementById("txt_parentesco").addEventListener("input", ValidarTipoLicencia);


$(document).ready(function () {
    Codigo = new URLSearchParams(window.location.search).get('codigo');
    Id = new URLSearchParams(window.location.search).get('id');
    llenarComboMotivoPermiso();



});

/*var servidor = "https://foscal.co/admautogestion";*/
var servidor = "http://localhost:55389/";
/*var servidor = "http://190.71.21.57/admautogestion"*/;
var Codigo = "";
var Id = "";


$(function () {
    $("#formuploadajax_permisos").on("submit", function (e) {
        e.preventDefault();
        var f = $(this);

        //var files = $("#adjuntopermiso").get(0).files;

        try {
            var FecPer = $("#txt_fechapermiso").val();
            if (FecPer == "") {
                throw 'Para continuar, Primero debe digitar la Fecha de inicio del permiso.';
            }

            var FecFinPer = $("#txt_fechafinpermiso").val();
            if (FecFinPer == "") {
                throw 'Para continuar, Primero debe digitar la Fecha de fin del permiso.';
            }

            if (FecPer == FecFinPer) {
                var HorIni = $("#txt_horaInicioPer").val();
                if (HorIni == "") {
                    throw 'Para continuar, Primero debe digitar la hora de inicio del permiso.';
                }

                var HorFin = $("#txt_horaFinPer").val();
                if (HorFin == "") {
                    throw 'Para continuar, Primero debe digitar la hora de fin del permiso.';
                }
            } else {
                document.getElementById("txt_horaInicioPer").value = "00:00";
                document.getElementById("txt_horaFinPer").value = "00:00";
            }

            var Jornada = $("#Jornada").val();
            if (Jornada == "" || Jornada == "Vacio") {
                throw 'Para continuar, Primero debe seleccionar si es jornada laboral completa.';
            }


            var motivo = $("#txt_MotivoPermiso").val();
            if (motivo == "" || motivo == "Vacio") {
                throw 'Para continuar, Primero debe seleccionar el motivo del permiso.';
            }

            var parentesco = $("#txt_parentesco").val();
            if (motivo == 5 && (parentesco == "Vacio" || parentesco == "")) {
                parentesco = "NO APLICA";
                throw 'Para continuar, Primero debe seleccionar el parentesco.';
            }



            var frmData = new FormData();
            frmData.append("Permiso.EmpleadoId", Id);

            frmData.append("Permiso.FechaPermiso", document.getElementById("txt_fechapermiso").value);
            frmData.append("Permiso.FechaFinPermiso", document.getElementById("txt_fechafinpermiso").value);
            frmData.append("Permiso.HoraInicioPermiso", document.getElementById("txt_horaInicioPer").value);
            frmData.append("Permiso.HoraFinPermiso", document.getElementById("txt_horaFinPer").value);
            frmData.append("Permiso.MotivoPermiso", document.getElementById("txt_MotivoPermiso").value);
            frmData.append("Permiso.Jornada", document.getElementById("Jornada").value);
            frmData.append("Permiso.Parentesco", $("#txt_parentesco option:selected").text());
            frmData.append("Permiso.Observacion", document.getElementById("txt_observacion").value);

            //frmData.append("Adjunto", files[0]);

            var filas = $("#detallesper").find("tr"); //devulve las filas del body de tu tabla segun el ejemplo que brindaste
            if (filas.length > 0) {
                frmData.append("Permiso.CantidadAdjuntos", filas.length);
                var error = '';ram
                for (i = 0; i < filas.length; i++) { //Recorre las filas 1 a 1
                    frmData.append("Adjunto" + i, $("#anexo" + i).get(0).files[0]);

                    if ($("#anexo" + i).get(0).files.length == 0) {
                        error = 'SI';
                    }
                }

                if (error == 'SI') {
                    if (motivo != 3) {
                        throw 'Debe adjuntar todos los soportes solicitados.';
                    } else { frmData.append("Permiso.ArchivoVacio", "SI"); }
                }
            }



            $.ajax({
                url: servidor + "/api/permisos",
                type: 'POST',
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                data: frmData,
                contentType: false,
                processData: false,
                beforeSend: function () { $("#processState").modal("show"); },
                success: function (result) {
                    $("#processState").modal("hide");
                    if (result == "true") {
                        alertify.alert('Los datos se registraron correctamente');
                       /* atras_Permisos();*/
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


function llenarComboMotivoPermiso() {

    var urlDetalle = servidor + "/api/motivopermiso";

    $.ajax({
        url: urlDetalle,
        type: 'GET',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: function (data) {

            var objdata = $.parseJSON(data);

            objdata.forEach(function (valor, indice) {


                $('#txt_MotivoPermiso').append('<option value="' + valor.Id + '" selected>' + valor.Nombre + '</option>');

            });

            $('#txt_MotivoPermiso').append('<option value="" selected>Seleccione..</option>');
        }
    })

}


function Permisos() {

    llenarComboMotivoPermiso();


}


$("#Jornada").change(function (event) {

    document.getElementById("txt_MotivoPermiso").innerHTML = "";
    llenarComboMotivoPermiso();
});


function ValidarBono() {

    var motivo = $("#txt_MotivoPermiso").val();
    var parentesco = "Vacio";

    try {

        if (motivo == 10) {

            var empleado = Id;

            var urlDetalle = servidor + "/api/Permisos/ValidarBono/" + empleado;

            $.ajax({
                url: urlDetalle,
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: function (data) {
                    var objdata = $.parseJSON(data);

                    try {

                        if (objdata == "True") {
                            throw 'No es posible solicitar el bono de navidad, debido a que ya fue disfrutado.';
                        }

                        else { ValidarMotivo(motivo); }

                        if (objdata == "True2") {
                            throw 'No es posible solicitar el bono de navidad, debido a que ya tiene una solicitud por el mismo motivo en proceso.';
                        }

                        else { ValidarMotivo(motivo); }

                    } catch (err) {
                        alertify.alert(err);
                    };


                }
            });
        } else { ValidarMotivo(motivo); }


    } catch (err) {
        alertify.alert(err);
    };


}


function ValidarSalarioEmocional() {

    var motivo = $("#txt_MotivoPermiso").val();
    var parentesco = "Vacio";

    try {

        if (motivo == 9) {

            var empleado = Id;

            var urlDetalle = servidor + "/api/Permisos/ValidarSalarioEmocional/" + empleado;

            $.ajax({
                url: urlDetalle,
                type: 'GET',
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                success: function (data) {
                    var objdata = $.parseJSON(data);

                    try {

                        if (objdata == "True1") {
                            throw 'No es posible solicitar el Salario Emocional, debido a que ya fue disfrutado.';
                        }

                        else { ValidarMotivo(motivo); }
                        if (objdata == "True2") {
                            throw 'No es posible solicitar el Salario Emocional, debido a que ya tiene una solicitud por el mismo motivo en proceso.';
                        }

                        else { ValidarMotivo(motivo); }

                    } catch (err) {
                        alertify.alert(err);
                    };


                }
            });
        } else { ValidarMotivo(motivo); }


    } catch (err) {
        alertify.alert(err);
    };


}


function ValidarMotivo(motivo) {

    var motivo = $("#txt_MotivoPermiso").val();
    var jornada = $("#Jornada").val();
    var parentesco = "Vacio";
    $("#Mensaje_Anexos").hide();

    try {

        if (motivo != 5) {


            $("#Parentesco").hide();
            $("#detallesper tr").remove();


            if (motivo == 9 && jornada == "SI") {
                throw 'No es posible solicitar jornada completa. El tiempo maximo que se puede solicitar por salario emocional son 6 horas.';
            }



            var urlDetalle = servidor + "/api/Permisos/TipoAdjunto/" + motivo + "/" + parentesco;


            $.getJSON(urlDetalle,
                function (tipoAdj) {

                    $("#detallesper tr").remove();
                    var objdata = $.parseJSON(tipoAdj);
                    $("#Tipo_Adjunto").empty();

                    if (objdata.length == 0) {
                        $("#Mensaje_Anexos").show();

                    }

                    objdata.forEach(function (valor, indice) {

                        var fila = '<tr class ="selected" id="fila' + indice + '"><td></td><td>"' + valor.Nombre + '"</td><td><input type="file" class="form-control-file" id="anexo' + indice + '"></td></tr>';
                        //cont++;



                        //limpiar();
                        $("#detallesper").append(fila);

                        //evaluar();
                        // $("#Tipo_Adjunto").append('<option value=' + valor.Id + '>' + valor.Nombre + '</option>');
                    });


                });

        } else {
            $("#Parentesco").show();
            $("#detallesper tr").remove();
        }



    } catch (err) {
        alertify.alert(err);
    };


}

function ValidarTipoLicencia() {

    var motivo = $("#txt_MotivoPermiso").val();
    var parentesco = "Vacio";

    try {


        parentesco = $("#txt_parentesco").val();
        if (parentesco == "" || parentesco == "Vacio") {
            throw 'Para continuar, Primero debe digitar la Fecha Inicial.';
        }


        var urlDetalle = servidor + "/api/Permisos/TipoAdjunto/" + motivo + "/" + parentesco;


        $.getJSON(urlDetalle,
            function (tipoAdj) {
                $("#detallesper tr").remove();
                var objdata = $.parseJSON(tipoAdj);
                $("#Tipo_Adjunto").empty();

                objdata.forEach(function (valor, indice) {

                    var fila = '<tr class ="selected" id="fila' + indice + '"><td></td><td>"' + valor.Nombre + '"</td><td><input type="file" class="form-control-file" id="anexo' + indice + '"></td></tr>';
                    //cont++;



                    //limpiar();
                    $("#detallesper").append(fila);

                    //evaluar();
                    // $("#Tipo_Adjunto").append('<option value=' + valor.Id + '>' + valor.Nombre + '</option>');
                });


            });



    } catch (err) {
        alertify.alert(err);
    };

}



function ValidarDias() {
    var fechainicio = $("#txt_fechapermiso").val();
    var fechafin = $("#txt_fechafinpermiso").val();

    try {

        if (fechainicio == '' || fechainicio == null) {
            throw 'Para continuar, Primero debe digitar la Fecha de inicio del permiso.';
        }

        if (fechafin < fechainicio) {
            document.getElementById("txt_fechafinpermiso").value = "";
            throw 'La fecha Final no puede ser menor a la Fecha de inicio del permiso.';
        }

        if (fechafin > fechainicio) {
            $("#DivHoras").hide();
        } else { $("#DivHoras").show(); }


    } catch (err) {
        alertify.alert(err);
    };

}