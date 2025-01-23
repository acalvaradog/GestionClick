//----------------------------MODALES--------------------------------//
$(document).ready(function () {
    //-----------Detail jefe (ojito)---------------//
    $.ajaxSetup({ cache: false });
    $("a[data-modalHEJefeDetail]").on("click", function (e) {
        openmodalHE01(this.href);
        

        return false;
    });
    $("a[data-modalHistoricoHEJefeDetail]").on("click", function (e) {
        openmodalHE02(this.href);


        return false;
    });
    //limpiar html''
    $('#modal_HEDetailJefe').on('hidden.bs.modal', function () {
        $('#contentHE1ModalJefe').html('');

    })

    var modalObservacion = document.getElementById('modalObservacion');
    modalObservacion.addEventListener('show.bs.modal', function (event) {
        // Button that triggered the modal
        var button = event.relatedTarget;
        // Extract info from data-bs-* attributes
        var observacion = button.getAttribute('data-observacion');
        // Update the modal's content
        var modalBody = modalObservacion.querySelector('.modal-body p');
        modalBody.textContent = observacion;
    });

    //$("#btnMostrarModal").click(function () {
    //    // Obtenemos la observación del atributo data-observacion del botón
    //    var observacion = $(this).data("observacion");

    //    // Mostramos la observación en la modal
    //    $("#observacion-modal").text(observacion);

    //    // Mostramos la modal (esto ya lo hace data-toggle="modal" pero lo incluyo por claridad)
    //    $("#modalObservacion").modal("show");
    //});

    //$("a[data-modalObservacion").on("click", function (e) {
    //    openmodalObservacion(this.href);
    //    return false;
    //});



    //function openmodalObservacion(url) {
    //    $('#contentModalCerrarCap').load(url, function () {
    //        $('#modal_CerrarCap').modal('show');

    //        //bindForm(this);
    //    });
    //}









});

//---------------------------- OPEN MODAL---------------------------//
//function openmodalHE01(url, modalTitle) {
//    $('#contentModalHE2').load(url, function () {
//        $('#modal_HEDetailJefe .modal-title').text(modalTitle);
//        $('#modal_HEDetailJefe').modal('show');

//        //bindForm(this);
//    });
//}



function openmodalHE01(url) {
    $('#contentModalHE2').load(url, function () {
        /*$('#modal_HEDetailJefe .modal-title').text(modalTitle);*/
        $('#modal_HEDetailJefe').modal('show');

        //bindForm(this);
    });
}

function openmodalHE02(url, modalTitle) {
    $('#contentModalHE2').load(url, function () {
        $('#modal_HEDetailJefe .modal-title').text(modalTitle);
        $('#modal_HEDetailJefe').modal('show');

        //bindForm(this);
    });
}

//-------------------------FUNCIONES--------------------------//

function JefeDirectoHorasExtra() {
    var button = document.getElementById('AprobacionJefe');
    button.setAttribute('disabled', 'disabled');

    try {
        var estado = $("#Estado").val();
        var observaciones = $("#Observaciones").val();

        if (estado === "Denegar" && observaciones.trim() === "") {
            button.removeAttribute('disabled', 'disabled');
            alertify.alert("Por favor, ingrese observaciones al denegar la solicitud.");
            return;
        }

        var frmData = new FormData();
        var HorasExtraId = $("#HorasExtraId").val();

        frmData.append("Estado", estado);
        frmData.append("HorasExtraId", HorasExtraId);
        frmData.append("Observaciones", observaciones);

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
            error: function (xhr, status, error) {
                button.removeAttribute('disabled', 'disabled');
                alertify.alert(error);
            }
        });
    } catch (err) {
        button.removeAttribute('disabled', 'disabled');
        alertify.alert(err);
    }
}


function GestionHumanaHE() {

    var button = document.getElementById('AprobacionJefe');
    button.setAttribute('disabled', 'disabled');
    try {
        var frmData = new FormData();

        var estado = $("#Estado").val();
        var HorasExtraId = $("#HorasExtraId").val();
        var observaciones = $("#Observaciones").val();
        var fechapago = $("#FechaPago").val();

        if (estado === "Denegar" && observaciones.trim() === "") {
            button.removeAttribute('disabled', 'disabled');
            alertify.alert("Por favor, ingrese observaciones al denegar la solicitud.");
            return;
        }

        frmData.append("Estado", estado);
        frmData.append("HorasExtraId", HorasExtraId);
        frmData.append("Observaciones", observaciones);
        frmData.append("FechaPago", fechapago);



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






function HEMostrarObservaciones() {
    var Estado = $("#Estado").val();
    var DivObsevaciones = document.getElementById('ObservacionesDiv');
    if (Estado == "Denegar") {
        DivObsevaciones.hidden = false;
    } else { DivObsevaciones.hidden = true; };

};


//-----------------------------DATATABLE ---------------------//
$(document).ready(function () {

    var table = $('#Tabla_HorasExtra').DataTable({

        /*"bFilter": false,*/

        responsive: false,

        lengthMenu: [[5, 10, 20, -1], [5, 10, 20, 'Todos']],
        //bFilter": false,
        pageLength: 10,

        language: {
            "sProcessing": "Procesando...",
            "sLengthMenu": "Mostrar _MENU_ registros",
            "sZeroRecords": "No se encontraron resultados",
            "sEmptyTable": "No hay datos disponible en esta tabla",
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

   
        var table = $('#Exportar_a_Excel_HorasExtra').DataTable({

            // "bFilter": false,

            responsive: false,

            lengthMenu: [[5, 10, 20, -1], [5, 10, 20, 'Todos']],
            //bFilter": false,
            pageLength: 10,

            language: {
                "sProcessing": "Procesando...",
                "sLengthMenu": "Mostrar _MENU_ registros",
                "sZeroRecords": "No se encontraron resultados",
                "sEmptyTable": "No hay datos disponible en esta tabla",
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
            //dom: 'Brtip',

            //buttons: [
            //    {

            //        extend: 'excelHtml5',
            //        text: '<i class="fas fa-file-excel"></i>',
            //        filename: 'InformeProcesos_Horas_Extra',
            //        titleAttr: 'Exportar a Excel',
            //        className: 'btn btn-success'
            //    }
            //]
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
   // var table = $('#InformeTotalHE').DataTable({
        new DataTable('InformeTotalHE', {
        /*"bFilter": false,*/
        ordering: false,
        responsive: false,

        lengthMenu: [[5, 10, 20, -1], [5, 10, 20, 'Todos']],
        //bFilter": false,
        pageLength: 10,

        language: {
            "sProcessing": "Procesando...",
            "sLengthMenu": "Mostrar _MENU_ registros",
            "sZeroRecords": "No se encontraron resultados",
            "sEmptyTable": "Ningun dato disponible en esta tabla",
            "sInfo": "Mostrando registros del _START_ al _END_ de un total de _TOTAL_ registros",
            "sInfoEmpty": "Mostrando registros del 0 al 0 de un total de 0 registros",
            "sInfoFiltered": "(filtrado de un total de _MAX_ registros)",
            "sInfoPostFix": "",
            "sSearch": "Buscar:",
            "sUrl": "",


            "sInfoThousands": ",",
            "sLoadingRecords": "Cargando...",
            "oPaginate": {

                "sNext": "  - Siguiente",
                "sPrevious": "Anterior -  "
            }
        },
        dom: 'Brtip',
        layout: {

            topStart: {

                buttons: [
                    {

                        extend: 'excelHtml5',
                        text: '<i class="fas fa-file-excel"></i>',
                        filename: 'InformeProcesos_Horas_Extra',
                        titleAttr: 'Exportar a Excel',
                        className: 'btn btn-success',
                        exportOptions: {
                            columns: ':visible',
                            format: {
                                body: function (data, row, column, node) {
                                    data = $('<p>' + data + '</p>').text();
                                    return $.isNumeric(data.replace(',', '.')) ? data.replace(',', '.') : data;
                                }
                            }
                        }
                    }
                ]
            }
        }
       
    });

})

function EnviarAprobados() {
    try {
    // Obtener todos los checkboxes del formulario
    var checkboxes = document.querySelectorAll('input[name="item[]"]:checked');

    // Crear un array para guardar los IDs seleccionados
    var selectedIDs = [];

    // Recorrer los checkboxes seleccionados y obtener sus valores (IDs)
    checkboxes.forEach(function (checkbox) {
        selectedIDs.push(checkbox.value);
    });

    // Aquí puedes enviar los IDs al servidor usando AJAX o un formulario

    $.ajax({
        url: 'EnviarAprobadosJefe', // Reemplaza con la URL de tu controlador
        type: 'POST',
        data: { ids: selectedIDs },
        success: function (json) {
            if (json.respuesta != "") {
               // button.removeAttribute('disabled', 'disabled');
                alertify.alert(json.respuesta);
                location.reload();
                if (json.isRedirect) {
                    setTimeout(function () { window.location.href = json.redirectUrl }, 2500);
                }
            }
        },
        error: function (xhr, status, error) {
            //button.removeAttribute('disabled', 'disabled');
            alertify.alert(error);
        }
    });
    } catch (err) {
        button.removeAttribute('disabled', 'disabled');
        alertify.alert(err);
    
    }



}


function EnviarAprobadosGH() {
    try {
        // Obtener todos los checkboxes del formulario
        var checkboxes = document.querySelectorAll('input[name="item[]"]:checked');

        // Crear un array para guardar los IDs seleccionados
        var selectedIDs = [];
        var fechapago = $("#FechaPago").val();

        if (fechapago == null || fechapago == "") {
            alertify.alert("El campo Fecha Pago  es obligatorio al  el registro de horas extras");
            return;
        }


        // Recorrer los checkboxes seleccionados y obtener sus valores (IDs)
        checkboxes.forEach(function (checkbox) {
            selectedIDs.push(checkbox.value);
        });

        // Aquí puedes enviar los IDs al servidor usando AJAX o un formulario

        $.ajax({
            url: 'EnviarAprobadosGH', // Reemplaza con la URL de tu controlador
            type: 'POST',
            data: { ids: selectedIDs, fechap: fechapago },
            success: function (json) {
                if (json.respuesta != "") {
                    // button.removeAttribute('disabled', 'disabled');
                    alertify.alert(json.respuesta);
                    location.reload();
                    if (json.isRedirect) {
                        setTimeout(function () { window.location.href = json.redirectUrl }, 2500);
                    }
                }
            },
            error: function (xhr, status, error) {
                //button.removeAttribute('disabled', 'disabled');
                alertify.alert(error);
            }
        });
    } catch (err) {
        button.removeAttribute('disabled', 'disabled');
        alertify.alert(err);

    }



}

function EnviarRechazados() {
    try {
        // Obtener todos los checkboxes del formulario
        var checkboxes = document.querySelectorAll('input[name="item[]"]:checked');

        // Crear un array para guardar los IDs seleccionados
        var selectedIDs = [];
        var Observacion = $('#Observaciones').val();

        if (Observacion == null || Observacion == "") {
            alertify.alert("El campo Observaciones es obligatorio al rechazar el registro de horas extras");
            return;
        }

        // Recorrer los checkboxes seleccionados y obtener sus valores (IDs)
        checkboxes.forEach(function (checkbox) {
            selectedIDs.push(checkbox.value);
        });

        // Aquí puedes enviar los IDs al servidor usando AJAX o un formulario

        $.ajax({
            url: 'EnviarRechazadosJefe', // Reemplaza con la URL de tu controlador
            type: 'POST',
            data: { ids: selectedIDs, observaciones: Observacion },
            success: function (json) {
                if (json.respuesta != "") {
                    // button.removeAttribute('disabled', 'disabled');
                    alertify.alert(json.respuesta);
                    location.reload();
                    if (json.isRedirect) {
                        setTimeout(function () { window.location.href = json.redirectUrl }, 2500);
                    }
                }
            },
            error: function (xhr, status, error) {
                //button.removeAttribute('disabled', 'disabled');
                alertify.alert(error);
            }
        });
    } catch (err) {
        button.removeAttribute('disabled', 'disabled');
        alertify.alert(err);

    }



}


function EnviarRechazadosGH() {
    try {
        // Obtener todos los checkboxes del formulario
        var checkboxes = document.querySelectorAll('input[name="item[]"]:checked');

        // Crear un array para guardar los IDs seleccionados
        var selectedIDs = [];

        var Observacion = $('#Observaciones').val();

        if (Observacion == null || Observacion == "") {
            alertify.alert("El campo Observaciones es obligatorio al rechazar el registro de horas extras");
            return;
        }

        // Recorrer los checkboxes seleccionados y obtener sus valores (IDs)
        checkboxes.forEach(function (checkbox) {
            selectedIDs.push(checkbox.value);
        });

        // Aquí puedes enviar los IDs al servidor usando AJAX o un formulario

        $.ajax({
            url: 'EnviarRechazadosGH', // Reemplaza con la URL de tu controlador
            type: 'POST',
            data: { ids: selectedIDs, observaciones: Observacion },
            success: function (json) {
                if (json.respuesta != "") {
                    // button.removeAttribute('disabled', 'disabled');
                    alertify.alert(json.respuesta);
                    location.reload();
                    if (json.isRedirect) {
                        setTimeout(function () { window.location.href = json.redirectUrl }, 2500);
                    }
                }
            },
            error: function (xhr, status, error) {
                //button.removeAttribute('disabled', 'disabled');
                alertify.alert(error);
            }
        });
    } catch (err) {
        button.removeAttribute('disabled', 'disabled');
        alertify.alert(err);

    }



}


function ExportarExcel() {

    // Selecciona la tabla HTML
    var tabla = document.getElementById("InformeTotalHE");

    // Convierte la tabla a un libro de Excel
    var wb = XLSX.utils.table_to_book(tabla);

    // Guarda el libro de Excel
    XLSX.writeFile(wb, 'tabla.xlsx');
}

function ExportarExcelJefe() {

    // Selecciona la tabla HTML
    var tabla = document.getElementById("Tabla_HorasExtra");

    // Convierte la tabla a un libro de Excel
    var wb = XLSX.utils.table_to_book(tabla);

    //var ws = wb.Sheets[wb.SheetNames[0]];

    //for (var cell in ws) {
    //    // Verifica si la celda contiene una fecha (esto dependerá de cómo se 
    //    // identifiquen las celdas con fechas en tu tabla)
    //    if (cell.startsWith('I') && ws[cell].v.match(/^\d{2}\/\d{2}\/\d{4}$/)) {
    //        // Especifica el tipo de dato como fecha
    //        ws[cell].t = 'd';
    //    }
    //}

    // Guarda el libro de Excel
    XLSX.writeFile(wb, 'tabla.xlsx');
}


function ExportarExcelInfJef() {

    // Selecciona la tabla HTML
    var tabla = document.getElementById("Exportar_a_Excel_HorasExtra");

    // Convierte la tabla a un libro de Excel
    var wb = XLSX.utils.table_to_book(tabla);

    //var ws = wb.Sheets[wb.SheetNames[0]];

    //for (var cell in ws) {
    //    // Verifica si la celda contiene una fecha (esto dependerá de cómo se 
    //    // identifiquen las celdas con fechas en tu tabla)
    //    if (/* condición para identificar celdas con fechas */) {
    //        // Especifica el tipo de dato como fecha
    //        ws[cell].t = 'd';
    //    }
    //}

    // Guarda el libro de Excel
    XLSX.writeFile(wb, 'tabla.xlsx');
}

function ExportarExcelGH() {

    // Selecciona la tabla HTML
    var tabla = document.getElementById("Tabla_HorasExtra");

    // Convierte la tabla a un libro de Excel
    var wb = XLSX.utils.table_to_book(tabla);

    // Guarda el libro de Excel
    XLSX.writeFile(wb, 'tabla.xlsx');
}


function ExportarExcelInfGH() {

    // Selecciona la tabla HTML
    var tabla = document.getElementById("Exportar_a_Excel_HorasExtra");

    // Convierte la tabla a un libro de Excel
    var wb = XLSX.utils.table_to_book(tabla);

    // Guarda el libro de Excel
    XLSX.writeFile(wb, 'tabla.xlsx');
}




