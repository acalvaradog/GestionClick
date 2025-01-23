$(document).ready(function () {

    //----------------------------- MODALE DELETE --------------------------//
    $.ajaxSetup({ cache: false });
    $("a[data-modalSTRDelete]").on("click", function (e) {
        openmodalSTR01(this.href);
        return false;
    });

    $('#modal_STRDelete').on('hidden.bs.modal', function () {
        $('#contentModalSTRDelete').html('');

    })
});
//---------------------------- OPEN MODAL---------------------------//
function openmodalSTR01(url) {
    $('#contentModalSTRDelete').load(url, function () {
        $('#modal_STRDelete').modal('show');

        //bindForm(this);
    });
}

//--------------DATATABLE---------------//

$(document).ready(function () {

    var table = $('#Informe_STRJerarquica').DataTable({

        /*"bFilter": false,*/

        responsive: false,

        lengthMenu: [[5, 10, 20, -1], [5, 10, 20, 'Todos']],
        //bFilter": false,
        pageLength: 300,

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
            },
        },
        "pagingType": "simple",
        dom: 'Brtip',

        buttons: [
            {

                extend: 'excelHtml5',
                text: '<i class="fas fa-file-excel"></i>',
                filename: 'InformeEstructurarJerarquicas',
                titleAttr: 'Exportar a Excel',
                className: 'btn btn-success'
            }
        ]
    });

})
