//--------------DATATABLE---------------//

$(document).ready(function () {

    var table = $('#Informe_evaDesempeño').DataTable({

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
            },
        },
        //"pagingType": "simple",
        dom: 'Brtip',

        buttons: [
            {

                extend: 'excelHtml5',
                text: '<i class="fas fa-file-excel"></i>',
                filename: 'InformeProcesos_Evaluaciones_Periodicas',
                titleAttr: 'Exportar a Excel',
                className: 'btn btn-success'
            }
        ]
    });

})

//---------------------------------MODAL----------------------------------//
$(document).ready(function () {
    $.ajaxSetup({ cache: false });
    $("a[data-modalEVADE01]").on("click", function (e) {
        openmodalEVADE01(this.href);
        return false;
    });

    $('#modal_EVADE01').on('hidden.bs.modal', function () {
        $('#contentModalEVADE01').html('');

    })


});


function openmodalEVADE01(url) {
    $('#contentModalEVADE01').load(url, function () {
        $('#modal_EVADE01').modal('show');

        //bindForm(this);
    });
}



