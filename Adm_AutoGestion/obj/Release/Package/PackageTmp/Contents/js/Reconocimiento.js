//----------------------------MODALES--------------------------------//
$(document).ready(function () {
    //-----------Detail jefe (ojito)---------------//
    $.ajaxSetup({ cache: false });
    $("a[data-modalRecoJefeDetail]").on("click", function (e) {
        openmodalReco01(this.href);


        return false;
    });
    $("a[data-modalHistoricoRecoJefeDetail]").on("click", function (e) {
        openmodalReco02(this.href);


        return false;
    });
    //limpiar html''
    $('#modal_RecoDetailJefe').on('hidden.bs.modal', function () {
        $('#contentReco1ModalJefe').html('');

    })











});

//---------------------------- OPEN MODAL---------------------------//
function openmodalReco01(url, modalTitle) {
    $('#contentModalReco2').load(url, function () {
        $('#modal_RecoDetailJefe .modal-title').text(modalTitle);
        $('#modal_RecoDetailJefe').modal('show');

        //bindForm(this);
    });
}
function openmodalReco02(url, modalTitle) {
    $('#contentModalReco2').load(url, function () {
        $('#modal_RecoDetailJefe .modal-title').text(modalTitle);
        $('#modal_RecoDetailJefe').modal('show');

        //bindForm(this);
    });
}

//-------------------------FUNCIONES--------------------------//




function GestionHumana() {

    var button = document.getElementById('AprobacionJefe');
    button.setAttribute('disabled', 'disabled');
    try {
        var frmData = new FormData();

        var activo = $("#Activo").val();
        var ReconocimientoId = $("#ReconocimientoId").val();
        

        frmData.append("Activo", activo);
        frmData.append("ReconocimientoId", ReconocimientoId);
        



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






function RecoMostrarObservaciones() {
    var Estado = $("#Estado").val();
    var DivObsevaciones = document.getElementById('ObservacionesDiv');
    if (Estado == "Denegar") {
        DivObsevaciones.hidden = false;
    } else { DivObsevaciones.hidden = true; };

};


//-----------------------------DATATABLE ---------------------//
$(document).ready(function () {

    var table = $('#Tabla_Reconocimiento').DataTable({

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
    var table = $('#Exportar_a_Excel_Reconocimiento').DataTable({

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
        //exel
        //"pagingType": "simple",
        dom: 'Brtip',

        buttons: [
            {

                extend: 'excelHtml5',
                text: '<i class="fas fa-file-excel"></i>',
                filename: 'InformeProcesos_Reconocimiento',
                titleAttr: 'Exportar a Excel',
                className: 'btn btn-success'
            }
        ]
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

