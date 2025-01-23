//----------------------------MODALES--------------------------------//

$(document).ready(function () {
    $.ajaxSetup({ cache: false });
    $("a[data-modal33]").on("click", function (e) {
        openmodal33(this.href);
       
       
        return false;
    });

    $('#modal_ViaticoDetail').on('hidden.bs.modal', function () {
        $('#contentModal33').html('');

    })
       

});


function openmodal33(url) {
    $('#contentModal33').load(url, function () {
        $('#modal_ViaticoDetail').modal('show');
        
        //bindForm(this);
    });
}


$(document).ready(function () {
    $.ajaxSetup({ cache: false });
    $("a[data-modal34]").on("click", function (e) {
        openmodal34(this.href);
        return false;
    });

    $('#modal_ViaticoDetail2').on('hidden.bs.modal', function () {
        $('#contentModal34').html('');

    })


});


function openmodal34(url) {
    $('#contentModal34').load(url, function () {
        $('#modal_ViaticoDetail2').modal('show');

        //bindForm(this);
    });
}


$(document).ready(function () {
    $.ajaxSetup({ cache: false });
    $("a[data-modal35]").on("click", function (e) {
        openmodal35(this.href);
        return false;
    });

    $('#modal_ViaticoDetail3').on('hidden.bs.modal', function () {
        $('#contentModal35').html('');

    })


});


function openmodal35(url) {
    $('#contentModal35').load(url, function () {
        $('#modal_ViaticoDetail3').modal('show');

        //bindForm(this);
    });
}

$(document).ready(function () {
    $.ajaxSetup({ cache: false });
    $("a[data-modal36]").on("click", function (e) {
        openmodal36(this.href);
        return false;
    });

    $('#modal_ViaticosLog').on('hidden.bs.modal', function () {
        $('#contentModal36').html('');

    })


});


function openmodal36(url) {
    $('#contentModal36').load(url, function () {
        $('#modal_ViaticosLog').modal('show');

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
        if ($("#Estado").val() == 'Seleccione...') {
            throw 'Debe seleccionar un estado Válido.';
        }

        frmData.append("Estado", estado);
        frmData.append("Id", Id);


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

        if ($("#Estado").val() == 'Seleccione...') {
            throw 'Debe seleccionar un estado Válido.';
        }
        if (GastoT == "") { throw 'El gasto del trasporte no puede estar vacío.'; }
        if (GastoA == "") { throw 'El gasto de alimentación no puede estar vacío.'; }

        frmData.append("GastoTransporte", GastoT);
        frmData.append("GastoAlimentacion", GastoA);
        frmData.append("Estado", estado);
        frmData.append("IdViatico", IdViatico);


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
        if (a == "Nomina") { url = "RespuestajsonNom" }
       

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

//-----------------------------DATATABLE ---------------------//
$(document).ready(function () {

    var table = $('#Tabla_Viaticos').DataTable({

        /*"bFilter": false,*/

        responsive: true,

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

        responsive: true,

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
    var table = $('#Tabla_GH').DataTable({

        /*"bFilter": false,*/

        responsive: true,

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
