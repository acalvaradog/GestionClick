

        //****************************---------------- Datatable Historico -------------------*******************************
function Historico($Id) {

    //var URLactual = location.origin;
    //var pr = location.pathname;
    //var url = pr.split('/');
    //var ruta = URLactual + "/" + url[1] + "/Permisos/Tabla_Historico_Json/" + $Id;

                $('#Modal_Historico_json').modal('show');
                $("#Tabla_Historico").DataTable({
                    "destroy": true,
                        "processing": true,
                        "serverSide": true,
                        "ajax": {
                            "url": "../../Permisos/Tabla_Historico_Json/" + $Id,
                            "type": "POST",
                            "datatype": "json"
                        },
                        "pageLength": 10,
                        "filter": false,
                        "data": null,
                        "paging": false,
                        "columns": [
                            { "data": "HistPer.hipe.PermisoId", "name": "Id Permiso", "autoWidth": true },
                            { "data": "HistPer.hipe.Fecha_Permiso", "name": "Fecha de Modificación", "autoWidth": true },
                            { "data": "HistPer.emple.Nombres", "name": "Usuario Que Modificó", "autoWidth": true },
                            { "data": "estper.Nombre", "name": "Estado Permiso", "autoWidth": true },
                            { "data": "HistPer.hipe.Observaciones_Permiso", "name": "Observaciones", "autoWidth": true }
                        ],
                        columnDefs: [
                            { targets: [0], visible: true },
                            {
                                targets: [1], visible: true,
                                render: function (value) {
                                    if (value === null) return "";
                                    var pattern = /Date\(([^)]+)\)/;
                                    var results = pattern.exec(value);
                                    var dt = new Date(parseFloat(results[1]));
                                    if (dt.getFullYear() === 9999) return "";
                                    return ('0' + dt.getDate()).slice(-2) + "/" + ('0' + (dt.getMonth() + 1)).slice(-2) + "/" + dt.getFullYear();
                                }
                            },
                            { targets: [2], visible: true, render: function (value) { return value.toLowerCase() } },
                            { targets: [3], visible: true, render: function (value) { return value.toLowerCase() } },
                            { targets: [4], visible: true, render: function (value) { return value.toLowerCase() } },

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
            $(document).ready(function () {
                    $('#cerrar_mod_his1').click(function () {
                        $('#Modal_Historico_json').modal('hide');
                    });
                    $('#cerrar_mod_his2').click(function () {
                        $('#Modal_Historico_json').modal('hide');
                    });
            });
        //***********************************------------------------------------********************************************

        //************************------ Datatable Descarga Archivos adjuntos Permisos --------------************************


            function Descarga_Adjuntos_Per($Id) {
                var URLactual = location.origin;
                var pr = location.pathname;
                var url = pr.split('/');
                var ruta = URLactual + "/" + url[1] + "/Tabla_Descarga_Json/" + $Id;
                var ruta2 = URLactual + "/" + url[1] + "/Permisos//Download?archivo=";


                $('#Modal_Descarga_json').modal('show');
                $("#Tabla_Descargar_archivos").DataTable({
                    "destroy": true,
                    "responsive": true,
                    "processing": true,
                    "serverSide": true,
                    "ajax": {
                        "url": ruta,
                        "type": "POST",
                        "datatype": "json"
                    },
                    "pageLength": 10,
                    "filter": false,
                    "data": null,
                    "paging": false,
                    "columns": [
                        { "data": "PermisosId", "name": "Id", "autoWidth": true },
                        { "data": "Adjunto", "name": "Documento", "autoWidth": true },
                        {
                            "data": "Adjunto", "name": "Opciones", "render": function (data) {
                                return '<a class="btn  btn-outline-primary btn-sm fas fa-file-download" href=" ' + ruta2  + data + '"></a>';
                            }, "searchable": false, "orderable": false, "autoWidth": true
                        }
                    ],
                    columnDefs: [
                                    { targets: [0], visible: true, searchable: false, orderable: true },
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
            $(document).ready(function () {

                $('#cerrar_mod_desc1').click(function () {
                    $('#Modal_Descarga_json').modal('hide');
                });
                $('#cerrar_mod_desc2').click(function () {
                    $('#Modal_Descarga_json').modal('hide');
                });
            });
        //***********************************------------------------------------********************************************

           
            //************************* Modal Anular ********************************

            function Modal_Anular($Id) {
                var URLactual = location.origin;
                var pr = location.pathname;
                var url = pr.split('/');
                var ruta = URLactual + "/" + url[1] + "/Obtener_Per/" + $Id;


                $.ajax({
                    data: '{id:' + JSON.stringify($Id) + '}',
                    url: ruta,
                    type: "post",
                    contentType: "application/json; charset=utf-8",
                    beforeSend: function () { },
                    success: function (response) {
                        if (response == false) {

                            alertify.error('¡Error! Seleccione un permiso valido');
                        }
                        else {
                            $("#Id").val(response.Id);
                            $("#Id1").val(response.Id);
                            $("#Empleado1").val(response.EmpleadoId);
                            $("#NroEmpleado1").val(response.NroEmpleado);
                            $("#FechaPermiso1").val(response.FechaPermiso);
                            $("#HoraInicioPermiso1").val(response.HoraInicioPermiso);
                            $("#HoraFinPermiso1").val(response.HoraFinPermiso);
                            $("#Jornada1").val(response.Jornada);
                            $('#Modal_Anulado').modal('show');
                        }
                    },

                });
            }

//***********************************------------------------------------********************************************

            //************************* Modal Anular Nomina ********************************
            function Modal_Anularnom($Id) {
                var URLactual = location.origin;
                var pr = location.pathname;
                var url = pr.split('/');
                var ruta = URLactual + "/" + url[1] + "/Obtener_Per/" + $Id;

                $.ajax({
                    data: '{id:' + JSON.stringify($Id) + '}',
                    url: ruta,
                    type: "post",
                    contentType: "application/json; charset=utf-8",
                    beforeSend: function () { },
                    success: function (response) {

                        if (response == false || response == null) {

                            alertify.error('¡Error! Seleccione un permiso valido');
                        }
                        else {
                            $("#Id_1").val(response.Id);
                            $("#Id_2").val(response.Id);
                            $("#Empleado1").val(response.EmpleadoId);
                            $("#NroEmpleado1").val(response.NroEmpleado);
                            $("#FechaPermiso1").val(response.FechaPermiso);
                            $("#HoraInicioPermiso1").val(response.HoraInicioPermiso);
                            $("#HoraFinPermiso1").val(response.HoraFinPermiso);
                            $("#Jornada1").val(response.Jornada);
                            $('#Modal_Anulado').modal('show');
                        }
                    },

                });
            }
            $(document).ready(function () {
                $('#cerrar_mod_1').click(function () {
                    //alertify.danger('¡Error! El Permiso No Fue Editado');
                    $('#Modal_Anulado').modal('hide');
                });
                $('#cerrar_mod_2').click(function () {
                    /*alertify.danger('¡Error! El Permiso No Fue Editado');*/
                    $('#Modal_Anulado').modal('hide');
                });
            });


//***********************************------------------------------------********************************************


//************************* Modal Detalle ********************************

            $(document).ready(function () {
                $.ajaxSetup({ cache: false });
                $("a[data-modal25]").on("click", function (e) {
                    openmodal25(this.href);
                    return false;
                });
                $('#modal_detalle').on('hidden.bs.modal', function () {
                    $('#contentModal25').html('');
                })
            });


            function openmodal25(url) {
                $('#contentModal25').load(url, function () {
                    $('#modal_detalle').modal('show');
                    //bindForm(this);

                });
            }

//***********************************------------------------------------********************************************