//----------------------------MODALES--------------------------------//
$(document).ready(function () {
    //-----------Detail jefe (ojito)---------------//
        $.ajaxSetup({ cache: false });
        $("a[data-modalVTJefeDetail]").on("click", function (e) {
            openmodalVT01(this.href);


            return false;
        });

        $('#modal_VTDetailJefe').on('hidden.bs.modal', function () {
            $('#contentVT1ModalJefe').html('');

        })
    //-------------Log GestiónHumana (rueda) , Log InformeViaticos (rueda), Log InformeViaticosGH (rueda)------------------//
        $.ajaxSetup({ cache: false });
        $("a[data-modalVTGH1]").on("click", function (e) {
            openmodalVT02(this.href);
            return false;
        });

        $('#modal_ViaticosLog').on('hidden.bs.modal', function () {
            $('#contentModalViaticosLog').html('');

        });



    //-----------------Detail GestionHumana (ojito) , Detail Nomina (ojito) , Detail Tesoreria (ojito)--------------------//
        $.ajaxSetup({ cache: false });
        $("a[data-modalVTGH2]").on("click", function (e) {
            openmodalVT03(this.href);
            return false;
        });

        $('#modal_ViaticoDetail2').on('hidden.bs.modal', function () {
            $('#contentVT03').html('');

        });

        //----------------------------- VERIFICAR VIAJE JEFE --------------------------//
            $.ajaxSetup({ cache: false });
            $("a[data-modalVTCHECKJEFE]").on("click", function (e) {
                openmodalVT04(this.href);
                return false;
            });

        $('#modal_ViaticosCHECKJEFE').on('hidden.bs.modal', function () {
            $('#contentModalViaticosCHECKJEFE').html('');

        });

        //----------------------------- ANULAR VIAJE JEFE --------------------------//
        $.ajaxSetup({ cache: false });
        $("a[data-modalVTAnularJEFE]").on("click", function (e) {
            openmodalVT05(this.href);
            return false;
        });

        $('#modal_ViaticosAnularEFE').on('hidden.bs.modal', function () {
            $('#contentModalViaticosAnularJEFE').html('');

        });




});

//---------------------------- OPEN MODAL---------------------------//
function openmodalVT01(url) {
    $('#contentModalVT2').load(url, function () {
        $('#modal_VTDetailJefe').modal('show');

        //bindForm(this);
    });
}

function openmodalVT02(url) {
    $('#contentModalViaticosLog').load(url, function () {
        $('#modal_ViaticosLog').modal('show');

        //bindForm(this);
    });
}

function openmodalVT03(url) {
    $('#contentVT03').load(url, function () {
        $('#modal_ViaticoDetail2').modal('show');

        //bindForm(this);
    });
}

function openmodalVT04(url) {
    $('#contentModalViaticosCHECKJEFE').load(url, function () {
        $('#modal_ViaticosCHECKJEFE').modal('show');

        //bindForm(this);
    });
}
function openmodalVT05(url) {
    $('#contentModalViaticosAnularJEFE').load(url, function () {
        $('#modal_ViaticosAnularJEFE').modal('show');

        //bindForm(this);
    });
}
//-------------------------FUNCIONES--------------------------//

function JefeDirectoViatico() {
    
    var button = document.getElementById('AprobacionJefe');
    button.setAttribute('disabled', 'disabled');
    try {
        var frmData = new FormData();

        var estado = $("#Estado").val();
        var Id = $("#Id").val();   
        var Obsevaciones = document.getElementById('Observaciones');
        if ($("#Estado").val() == 'Seleccione...') {
            throw 'Debe seleccionar un estado Válido.';
        }
        var DivObsevaciones = document.getElementById('ObservacionesDiv');
        if ($("#Estado").val() == 'Denegar') {
            if (DivObsevaciones.hidden == false) {
              
                
                if (Obsevaciones.value == "" || Obsevaciones.value == null)
                {
                    throw 'Debe escribir una observación';
                };
            };
        };

        frmData.append("Estado", estado);
        frmData.append("Id", Id);
        frmData.append("Observaciones", Obsevaciones.value);


        $.ajax({

            url: "RespuestajsonJefe",
            type: "POST",
            contentType: "application/json; charset=utf-8",
            dataType: 'json',
            data: frmData,
            contentType: false,
            processData: false,
            success: function (json) {
                if (json.respuesta != "") {
                    button.removeAttribute('disabled', 'disabled');
                    alertify.alert(json.respuesta);
                    if (json.isRedirect) {

                        setTimeout(function () { window.location.href = json.redirectUrl }, 2500);

                    }
                }

            },
            error: function (xhr, status, error) { button.removeAttribute('disabled', 'disabled'); alertify.alert(error); }

        });
    } catch (err)
    { button.removeAttribute('disabled', 'disabled'); alertify.alert(err); }

}

function GestionViatico() {

    var button = document.getElementById('AprobacionGestion');
    button.setAttribute('disabled', 'disabled');
    try {
        var frmData = new FormData();

        var estado = $("#Estado").val();
        var IdViatico = $("#Id").val();
        var GastoA = $("#GrastoAlimentacion").val();
        var GastoT = $("#GastosTransporte").val();
        var Observaciones = $("#Observaciones").val();
        var DivObsevaciones = document.getElementById('ObservacionesDiv');

        if ($("#Estado").val() == 'Seleccione...') {
            throw 'Debe seleccionar un estado Válido.';
        }
        if (GastoT == "") { throw 'El gasto del trasporte no puede estar vacío.'; }
        if (GastoA == "") { throw 'El gasto de alimentación no puede estar vacío.'; }       
        if ($("#Estado").val() == 'Denegar') {
            if (DivObsevaciones.hidden == false) {


                if (Observaciones == "" || Observaciones == null) {
                    throw 'Debe escribir una observación';
                };
            };
        };
        frmData.append("GastoTransporte", GastoT);
        frmData.append("GastoAlimentacion", GastoA);
        frmData.append("Estado", estado);
        frmData.append("IdViatico", IdViatico);
        frmData.append("Observaciones", Observaciones);


        $.ajax({

            url: "RespuestajsonGestion",
            type: "POST",
            contentType: "application/json; charset=utf-8",
            dataType: 'json',
            data: frmData,
            contentType: false,
            processData: false,
            success: function (json) {
                if (json.respuesta != "") {
                    button.removeAttribute('disabled', 'disabled');
                    alertify.alert(json.respuesta);
                    if (json.isRedirect) {

                        setTimeout(function () { window.location.href = json.redirectUrl }, 2500);

                    }
                }

            },
            error: function (xhr, status, error) { button.removeAttribute('disabled', 'disabled'); alertify.alert(error); }

        });
    } catch (err) { button.removeAttribute('disabled', 'disabled'); alertify.alert(err); }

}

function TesoreriaNomina(a) {
    var url;
    var button = document.getElementById('CheckTesNom');
    button.setAttribute('disabled', 'disabled');
    try {
        var frmData = new FormData();
        var Id = $("#Id").val();

     
        frmData.append("check", a);
        frmData.append("Id", Id);
        

        if (a == "Tesoreria") { url = "RespuestajsonTes" }
        if (a == "Nomina") {
            url = "RespuestajsonNom";
            var Cod = $('#CodContabilizacionSAP').val();
            if (Cod == "" || Cod == null)
            {
                throw 'Debe digitar el codigo de Contabilización';
            }
            frmData.append("CodContabilizacionSAP", Cod);
        }
        if (a == "NominaCargue") { url = "RespuestajsonNom" }

        $.ajax({

            url: url,
            type: "POST",
            contentType: "application/json; charset=utf-8",
            dataType: 'json',
            data: frmData,
            contentType: false,
            processData: false,
            success: function (json) {
                if (json.respuesta != "") {
                    button.removeAttribute('disabled', 'disabled');
                    alertify.alert(json.respuesta);
                    if (json.isRedirect) {

                        setTimeout(function () { window.location.href = json.redirectUrl }, 2500);

                    }
                }

            },
            error: function (xhr, status, error) { button.removeAttribute('disabled', 'disabled'); alertify.alert(error); }

        });
    } catch (err) { button.removeAttribute('disabled', 'disabled'); alertify.alert(err); }

}

function TotalDias() {




    var fechaInicio = $("#FechaInicio").val().split("/");
    var fechaFin = $("#FechaFin").val().split("/");
    var fechadesde = new Date(fechaInicio[2], fechaInicio[1] - 1, fechaInicio[0]).getTime();
    var fechahasta = new Date(fechaFin[2], fechaFin[1] - 1, fechaFin[0]).getTime();

    var dias = fechahasta - fechadesde;
    var diff_ = dias / (1000 * 60 * 60 * 24);

    $("#TotalDias").val(diff_)

    return diff_;
}

 function VTMostrarObservaciones () {
     var Estado = $("#Estado").val();
     var DivObsevaciones = document.getElementById('ObservacionesDiv');
     if (Estado == "Denegar") {
         DivObsevaciones.hidden = false;
     } else { DivObsevaciones.hidden = true; };
        
    };


//-----------------------------DATATABLE ---------------------//
$(document).ready(function () {

    var table = $('#Tabla_Viaticos').DataTable({

        /*"bFilter": false,*/

        responsive: false,

        lengthMenu: [[5, 10, 20, -1], [5, 10, 20, 'Todos']],
        //bFilter": false,
        pageLength: 10,

        language: {
            "sProcessing": "Procesando...",
            "sLengthMenu": "Mostrar _MENU_ registros",
            "sZeroRecords": "No se encontraron resultados",
            "sEmptyTable": "Ningún dato disponible en esta tabla",
            "sInfo": "Mostrando registros del _START_ al _END_ de un total de _TOTAL_ registros",
            "sInfoEmpty": "Mostrando registros del 0 al 0 de un total de 0 registros",
            "sInfoFiltered": "(filtrado de un total de _MAX_ registros)",
            "sInfoPostFix": "",
            "sSearch": "Buscar:",
            "sUrl": "",


            "sInfoThousands": ",",
            "sLoadingRecords": "Cargando...",
            "oPaginate": {
                "sFirst": "Primero-",
                "sLast": "Último",
                "sNext": "  - Siguiente",
                "sPrevious": "Anterior -  "
            },
            "oAria": {
                "sSortAscending": ": Activar para ordenar la columna de manera ascendente",
                "sSortDescending": ": Activar para ordenar la columna de manera descendente"
            }
        },
    });
    var table = $('#Exportar_a_Excel_Viaticos').DataTable({

        /*"bFilter": false,*/

        responsive: false,

        lengthMenu: [[5, 10, 20, -1], [5, 10, 20, 'Todos']],
        //bFilter": false,
        pageLength: 5,

        language: {
            "sProcessing": "Procesando...",
            "sLengthMenu": "Mostrar _MENU_ registros",
            "sZeroRecords": "No se encontraron resultados",
            "sEmptyTable": "Ningún dato disponible en esta tabla",
            "sInfo": "Mostrando registros del _START_ al _END_ de un total de _TOTAL_ registros",
            "sInfoEmpty": "Mostrando registros del 0 al 0 de un total de 0 registros",
            "sInfoFiltered": "(filtrado de un total de _MAX_ registros)",
            "sInfoPostFix": "",
            "sSearch": "Buscar:",
            "sUrl": "",


            "sInfoThousands": ",",
            "sLoadingRecords": "Cargando...",
            "oPaginate": {
                "sFirst": "Primero-",
                "sLast": "Último",
                "sNext": "  - Siguiente",
                "sPrevious": "Anterior -  "
            },
            "oAria": {
                "sSortAscending": ": Activar para ordenar la columna de manera ascendente",
                "sSortDescending": ": Activar para ordenar la columna de manera descendente"
            }
        },
    });
    var table = $('#Tabla_GH').DataTable({

        /*"bFilter": false,*/

        responsive: false,

        lengthMenu: [[5, 10, 20, -1], [5, 10, 20, 'Todos']],
        //bFilter": false,
        pageLength: 10,

        language: {
            "sProcessing": "Procesando...",
            "sLengthMenu": "Mostrar _MENU_ registros",
            "sZeroRecords": "No se encontraron resultados",
            "sEmptyTable": "Ningún dato disponible en esta tabla",
            "sInfo": "Mostrando registros del _START_ al _END_ de un total de _TOTAL_ registros",
            "sInfoEmpty": "Mostrando registros del 0 al 0 de un total de 0 registros",
            "sInfoFiltered": "(filtrado de un total de _MAX_ registros)",
            "sInfoPostFix": "",
            "sSearch": "Buscar:",
            "sUrl": "",


            "sInfoThousands": ",",
            "sLoadingRecords": "Cargando...",
            "oPaginate": {
                "sFirst": "Primero-",
                "sLast": "Último",
                "sNext": "  - Siguiente",
                "sPrevious": "Anterior -  "
            },
            "oAria": {
                "sSortAscending": ": Activar para ordenar la columna de manera ascendente",
                "sSortDescending": ": Activar para ordenar la columna de manera descendente"
            }
        },
    });


})


