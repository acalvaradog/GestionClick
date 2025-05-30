


$(document).ready(function () {

    /**************************** VALIDAR FORMULARIO ***********************************/
    $('#frm_aprobar_incapacidad').submit(function (event) {
        var idValue = $('#Id').val();

        if (idValue == '0') {
            // Si el campo está vacío, previene el envío del formulario
            event.preventDefault();
            $('#alert_formulario').show();
            $('#alert_formulario').html(
                `<svg xmlns="http://www.w3.org/2000/svg" style="display: none;">
                    <symbol id="exclamation-triangle-fill" fill="currentColor" viewBox="0 0 16 16">
                        <path d="M8.982 1.566a1.13 1.13 0 0 0-1.96 0L.165 13.233c-.457.778.091 1.767.98 1.767h13.713c.889 0 1.438-.99.98-1.767L8.982 1.566zM8 5c.535 0 .954.462.9.995l-.35 3.507a.552.552 0 0 1-1.1 0L7.1 5.995A.905.905 0 0 1 8 5zm.002 6a1 1 0 1 1 0 2 1 1 0 0 1 0-2z" />
                    </symbol>
                </svg>
                <div class="alert alert-danger d-flex align-items-center" role="alert">
                    <svg class= "bi flex-shrink-0 me-2" width = "24" height = "24" role = "img" aria - label="Danger:" > <use xlink: href="#exclamation-triangle-fill" /></svg>
                    <div>
                        Primero debes hacer clic en <b>"Ver"</b> sobre un registro de incapacidad de la parte inferior. 
                    </div>

                </div>`);

            setTimeout(function () {
                $('#alert_formulario').fadeOut('slow');
            }, 4000);
        } else {
            $("#Search .spinner-border").show();
            $("#Search .bi-save").hide();
            $("#Search").attr("disabled", true);
        }
    });
});
//****************************************************************************************************************** */


//************************------ Datatable Descarga Archivos adjuntos Incapacidades --------------************************


function Descarga_Adjuntos_Incap($Id, Empleado, IndJefe) {
    console.log('prueba');
    var URLactual = location.origin;
    var pr = location.pathname;
    var url = pr.split('/');
    //var ruta = URLactual + "/" + url[1] + "/Tabla_Descarga_Json_Incapacidades/" + $Id + "/" + Empleado + "/" + IndJefe;
    var ruta = `${URLactual}/${url[1]}/Tabla_Descarga_Json_Incapacidades?Id=${$Id}&Empleado=${Empleado}&IndJefe=${IndJefe}`;

    var ruta2 = URLactual + "/" + url[1] + "/Download?archivo=";

    $("#Nom").val();
    $("#Doc").val();

    $('#Modal_Descarga_json_Incap').modal('show');
    $("#Tabla_Descargar_archivos").DataTable({
        "destroy": true,
        "responsive": true,
        "processing": true,
        "serverSide": true,
        "dom": 't',
        "ajax": function (data, callback, settings) {
            $.ajax({
                "url": ruta,
                "type": "POST",
                "dataType": "json",
                "data": data, 
                "success": function (response) {
                    $('#Nom').val(response.Empleado.Nombres);
                    $('#Doc').val(response.Empleado.Documento);

                    callback({
                        
                        data: response.data
                    });
                }
            });
        },
        "pageLength": 10,
        "filter": false,
        "data": null,
        "paging": false,
        "columns": [
            { "data": "ListadoTiposInc.Nombre", "name": "Nombre Incapacidad", "autoWidth": true },
            { "data": "NombreAdjunto", "name": "Tipo Adjunto", "autoWidth": true },
            { "data": "Adjunto", "name": "Documento", "autoWidth": true },
            {
                "data": "Adjunto", "name": "Opciones", "render": function (data) {
                    return '<a class="btn close text-info mr-4" href=" ' + ruta2 + data + '"><i class="bi bi-file-earmark-arrow-down-fill"></i></a>';
                }, "searchable": false, "orderable": false, "autoWidth": true
            }
        ],
        columnDefs: [
            { targets: [0], visible: true, searchable: false, orderable: false },
            { targets: [1], visible: true, searchable: false, orderable: false },
            { targets: [2], visible: true, searchable: false, orderable: false },
        ],
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
}
        //***********************************------------------------------------********************************************

