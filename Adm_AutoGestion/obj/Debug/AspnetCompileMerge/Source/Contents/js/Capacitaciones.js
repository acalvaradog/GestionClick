//document.getElementById("conocimientos").addEventListener("keyup", dessabledconcocimientos);
var archivo = document.getElementById("conocimientos");
//if (archivo) {
//    archivo.addEventListener("input", dessabledconcocimientos, false);
//}


function AddRowCap(dialog, insert) {
    var field = ["", ""];
    var count = 0;
    var tabla = [];
    if (dialog == "AddEmp2") {
        try {
            var em = $("#EmpleadoId option:selected").text();
            if (em == "Seleccione...") {
                throw "Para continuar, Primero debe seleccionar un Empleado";
            }
            $("#AddItemsCap tr").each(function (index, item, array) {
                tabla.push([item.cells[2].innerText]);
            });
            for (i = 0; i < tabla.length; i++) {
                if (em == tabla[i]) {
                    throw "No es posible asignar el mismo proceso dos veces al mismo empleado";
                }
            }

            var nombre = $("#EmpleadoId  option:selected").text();
            var valor = $("#EmpleadoId  option:selected").val();
            var arreglo = valor.split("-");

            field[0] = "AddItemsCap"; field[1] = "<tr><td><a href = '' onclick = '$(this).parent().parent().remove(); return false;'><img alt='Quitar' src='../../Contents/image/Trash.png' title='Quitar'/><a/></td><td hidden>" + $("#EmpleadoRegistraId  option:selected").val() + "</td><td>" + $("#EmpleadoId  option:selected").text() + "</td><td hidden>" + arreglo[0] + "</td><td>" + arreglo[1] + "</td><td>" + arreglo[2] + "</td><td>" + arreglo[3] + "</td><td hidden>" + arreglo[4] + "</td><td>" + arreglo[5] + "</td></tr>";
        }
        catch (err) { alertify.alert(err); }
    }

    if (insert == true) { $("#" + field[0]).append(field[1]); }
    if (insert == false) { $("#" + field[0]).html(field[1]); }
    return false;


}
function AddRowExpo(dialog, insert) {
    var field = ["", ""];
    var count = 0;
    var tabla = [];
    if (dialog == "AddEmp2") {
        try {
            var em = $("#EmpleadoId").select2('data');
            
            var em2 = em[0];
            var em3 = Object.values(em2);
            var nombre = em3[2];
            var valor = em3[3];
            if (em == "Seleccione...") {
                throw "Para continuar, Primero debe seleccionar un Empleado";
            }
            $("#AddItemsCap tr").each(function (index, item, array) {
                tabla.push([item.cells[2].innerText]);
            });
            for (i = 0; i < tabla.length; i++) {
                if (nombre == tabla[i]) {
                    throw "No es posible asignar el mismo proceso dos veces al mismo empleado";
                }
            }

          
            var arreglo = valor.split("-");

            field[0] = "AddItemsCap"; field[1] = "<tr><td><a href = '' onclick = '$(this).parent().parent().remove(); return false;'><img alt='Quitar' src='../Contents/image/Trash.png' title='Quitar'/><a/></td><td hidden>" + valor + "</td><td>" + nombre + "</td><td hidden>" + arreglo[0] + "</td><td>" + arreglo[1] + "</td><td>" + arreglo[2] + "</td><td>" + arreglo[3] + "</td><td hidden>" + arreglo[4] + "</td><td >" + arreglo[5] + "</td><td ></td>";
        }
        catch (err) { alertify.alert(err); }
    }

    if (insert == true) { $("#" + field[0]).append(field[1]); }
    if (insert == false) { $("#" + field[0]).html(field[1]); }
    return false;


}

function mostrarInputCierreCap()
{
    //seleccionando elemento
    var div1 = document.getElementById('oculto1');
    var div2 = document.getElementById('oculto2');
    var div3 = document.getElementById('oculto3');
    var select = document.getElementById('Medicion');

    //ocultar input fecha y numero
    //inputNumero.style.display = "none";

    var valorSeleccionado = select.value;
    if (valorSeleccionado == 'NO') {
        //ocultar input numero en caso de estar mostrandolo
        div1.hidden = true;
        div2.hidden = true;
        div3.hidden = true;
        //inputNumero.style.display = "none";

    } else {
        //mostrar Dias de suspencion
        div1.hidden = false;
        div2.hidden = false;
        div3.hidden = false;
        //inputNumero.style.display = "block";
    }
    return false;
}



function SaveCap()
{
    var frmData = new FormData();
    var emp = [];
    var NmrCap = $("#NmrCapacitacion").val();
    try
    {
        var filasd = $("#AddItemsCap").find("tr");

        if (filasd.length != 0) {           
            $("#AddItemsCap tr").each(function (index, item) {
                emp.push([item.cells[2].innerText + "/" + item.cells[3].innerText + "/" + item.cells[4].innerText + "/" + item.cells[5].innerText + "/" + item.cells[6].innerText + "/" + item.cells[8].innerText + "/"]);
            });
        } else { throw "Para continuar, Primero debe ingresar las Pruebas."; }
        emp;
        frmData.append("CantidadDetalles", filasd.length);
        frmData.append("Empleados", emp);
        frmData.append("NmrCapacitacion", NmrCap);
        $.ajax({
            url: "RegistrarCapacitacion",
            type: "POST",
            contentType: "application/json; charset=utf-8",
            dataType: 'json',
            data: frmData,
            contentType: false,
            processData: false,
            success: function (json) {
                if (json.respuesta != "") {
                    alertify.alert(json.respuesta);
                    if (json.isRedirect) {
                        setTimeout(function () { window.location.reload(true)}, 2000);

                    }
                }

            },

            error: function (xhr, status, error)
            { alertify.alert(error); }

        });
    }
    catch(err)
    {
        alertify.alert(err);
    };

}

function SaveExpo() {
    var frmData = new FormData();
    var emp = [];
    var NmrCap = $("#NmrCapacitacion").val();
    try {
        var filasd = $("#AddItemsCap").find("tr");

        if (filasd.length != 0) {
            $("#AddItemsCap tr").each(function (index, item) {
                emp.push([item.cells[2].innerText + "/" + item.cells[3].innerText + "/" + item.cells[4].innerText + "/" + item.cells[5].innerText + "/" + item.cells[6].innerText + "/" + item.cells[8].innerText + "/"]);
            });
        } else { throw "Para continuar, Primero debe ingresar las Pruebas."; }
        emp;
        frmData.append("CantidadDetalles", filasd.length);
        frmData.append("Empleados", emp);
        frmData.append("NmrCapacitacion", NmrCap);
        $.ajax({
            url: "Expositores2",
            type: "POST",
            contentType: "application/json; charset=utf-8",
            dataType: 'json',
            data: frmData,
            contentType: false,
            processData: false,
            success: function (json) {
                if (json.respuesta != "") {
                    alertify.alert(json.respuesta);
                    if (json.isRedirect) {
                        setTimeout(function () { window.location.reload(true) }, 1800);

                    }
                }

            },

            error: function (xhr, status, error)
            { alertify.alert(error); }

        });
    }
    catch (err) {
        alertify.alert(err);
    };

}

function CerrarCapacitacion() {
    try {

        var frmData = new FormData();
        var IdCap = $("#NmrCapacitacion").val();
        var conocimientos = $("#Conocimientos").val();
        var cobertura = $("#Cobertura").val();
        var participantes = $("#CtnAsistentes").val();
        var programados = $("#CtnProgramados").val();
        var medicion = $("#Medicion option:selected").text();
        var competencia = $("#Competencia option:selected").val();
        var cultura = $("#CulturaO option:selected").val();
        var meta = $("#MetaE2").val();
        var resultado = $("#ResultadoM2").val();
        var metodologia = $("#Metodologia2 option:selected").val();
        if (conocimientos == "") {
            throw 'Debe indicar los Conocimientos'
        }
        if (cobertura == "") {
            throw 'Debe indicar la Cobertura'
        }
        if (participantes == "") {
            throw 'Debe indicar la cantidad de Participantes'
        }
        if (programados == "") {
            throw 'Debe indicar la cantidad Programada'
        }
        if (medicion == "" || medicion == "Seleccione...") {
            throw 'Debe indicar la Medicion'
        }
        if (competencia == "") {
            throw 'Debe indicar la Competencia'
        }
        if (cultura == "") {
            throw 'Debe indicar la Cultura'
        }


        //--------------------Condiciones en caso de mostrar los div--------------------------------//
        var div1 = document.getElementById('oculto1');
        var div2 = document.getElementById('oculto2');
        var div3 = document.getElementById('oculto3');
        if (div1.hidden != true) {
            if (meta == "") {
                throw 'Debe indicar la Meta'
            }
        }
        if (div2.hidden != true) {
            if (resultado == "") {
                throw 'Debe indicar el Resultado de medición'
            }

        }
        if (div3.hidden != true) {
            if (metodologia == "") {
                throw 'Debe indicar la Metodología'
            }
        }
        //----------------------------------------------------//
        frmData.append("IdCapacitacion", IdCap);
        frmData.append("Conocimientos", conocimientos);
        frmData.append("Cobertura", cobertura);
        frmData.append("CantidadAsistentes", participantes);
        frmData.append("CantidadProgramados", programados);
        frmData.append("Medicion", medicion);
        frmData.append("Competencia", competencia);
        frmData.append("Cultura", cultura);
        frmData.append("Meta", meta);
        frmData.append("Resultado", resultado);
        frmData.append("Metodología", metodologia);

        $.ajax({

            url: "CerrarCap2",
            type: "POST",
            contentType: "application/json; charset=utf-8",
            dataType: 'json',
            data: frmData,
            contentType: false,
            processData: false,
            success: function (json) {
                if (json.respuesta != "") {
                    alertify.alert(json.respuesta);
                    if (json.isRedirect) {
                        //setTimeout(function () { window.location.href = json.redirectUrl }, 2500);
                        setTimeout(function () { window.location.reload(true) }, 1800);

                    }
                }

            },
            error: function (xhr, status, error)
            { alertify.alert(error); }

        });

    } catch (err) { alertify.alert(err); }//(err) { Message(err); alert(err);}

}

$(document).ready(function () {
    $.ajaxSetup({ cache: false });
    $("a[data-modalCerrarCap]").on("click", function (e) {
        openmodalCerrarCap(this.href);
        return false;
    });

    $('#modal_modalCerrarCap').on('hidden.bs.modal', function () {
        $('#contentModalCerrarCap').html('');

    })

});

$(document).ready(function () {
    $.ajaxSetup({ cache: false });
    $("a[data-modalExposi]").on("click", function (e) {
        openmodalExposi(this.href);
        return false;
    });

    $('#modal_modalExposi').on('hidden.bs.modal', function () {
        $('#contentModalExposi').html('');

    })

});

$(document).ready(function () {
    $.ajaxSetup({ cache: false });
    $("a[data-modalCapacitacion]").on("click", function (e) {
        openmodalCapacitacion(this.href);
        return false;
    });

    $('#modal_modalCapacitacion').on('hidden.bs.modal', function () {
        $('#contentModalCapacitacion').html('');

    })

});


$(document).ready(function () {
    $.ajaxSetup({ cache: false });
    $("a[data-modalEncuestas]").on("click", function (e) {
        openmodalListaEncuesta(this.href);
        return false;
    });

    $('#modal_Lista_Encuestas').on('hidden.bs.modal', function () {
        $('#contentModalEncuestas').html('');

    })

});


$(document).ready(function () {
    $.ajaxSetup({ cache: false });
    $("a[data-modal2]").on("click", function (e) {
        openmodal2(this.href);
        return false;
    });
    $('#modal_Pendienteporfirma').on('hidden.bs.modal', function () {
        $('#contentModal2').html('');
    })
});

function openmodalCerrarCap(url) {
    $('#contentModalCerrarCap').load(url, function () {
        $('#modal_CerrarCap').modal('show');

        //bindForm(this);
    });
}


function openmodalExposi(url) {
    $('#contentModalExposi').load(url, function () {
        $('#modal_Exposi').modal('show');

        //bindForm(this);
    });
}


function openmodalCapacitacion(url) {
    $('#contentModalCapacitacion').load(url, function () {
        $('#modal_Capacitacion').modal('show');

        //bindForm(this);
    });
}

function openmodalListaEncuesta(url) {
    $('#contentModalEncuestas').load(url, function () {
        $('#modal_Lista_Encuestas').modal('show');

        //bindForm(this);
    });
}





function ConfirmarEnc(id, page) {

    var Encuesta = $('#Encuesta').val();

    alertify.confirm("Confirme si desea enviar la encuesta",
            function (e) {
                if (e) {
                    EnviarEncuesta(Encuesta, id, page)
                } else {

                }
            });


}

function EnviarEncuesta(Encuesta, id, page) {
    //var data = JSON.stringify({ 'area': 'area', 'fechainicial': '1', 'fechafinal': '1' });

    if (page == 1) {
        var URL = "EnvioEncuestaCap";
    } else { var URL = "../../Capacitacion/EnvioEncuestaCap" }

    try {
        $.ajax({
            url: URL,
            data: { 'id': id, 'Encuesta': Encuesta , 'Page': page },
            type: "post",
            success: function (e) {
                if (e != undefined || e != null) {
                    if (e == 'OK') {
                        //alertify.alert('Se ha enviado la encuesta');
                        alertify.confirm("Se ha enviado la encuesta.", function (s) {
                            if (s) {
                                $("#modal_Lista_Encuestas").modal("hide");
                                var x = document.getElementById(id);
                                var y = x.getElementsByTagName("a");
                                y[0].style.color = "green";
                            } else {

                            }
                        });


                    } else if (e == 'ERROR') {
                        alertify.alert('Error al enviar encuesta');

                    } else if (e == 'SINCORREO') {
                        alertify.alert('No existe ');

                    }
                }
            },
            error: function (xhr, status, error) { alert(error); }
        });
    }
    catch (e) { alert(e); }
}


function dessabledconcocimientos () {
    conct = $("#conocimientos").val(); 
    if (conct != "") {
        $("#competencia").prop('disabled', true);
        $("#cultura").prop('disabled', true);
    } else {
        $("#competencia").prop('disabled', false);
        $("#cultura").prop('disabled', false);
    }

}

function desabledcompetencia () {
    conct = $("#competencia").val();
    if (conct != "") {
        $("#conocimientos").prop('disabled', true);
        $("#cultura").prop('disabled', true);
    } else {
        $("#conocimientos").prop('disabled', false);
        $("#cultura").prop('disabled', false);
    }

}

function desabledcultura() {
    conct = $("#cultura").val();
    if (conct != "") {
        $("#conocimientos").prop('disabled', true);
        $("#competencia").prop('disabled', true);
    } else {
        $("#conocimientos").prop('disabled', false);
        $("#competencia").prop('disabled', false);
    }

}

function openmodal2(url) {
    $('#contentModal2').load(url, function () {
        $('#modal_Pendienteporfirma').modal('show');
        //bindForm(this);
    });
}

$(document).ready(function () {

    var table = $('#tbl_registro_cap').DataTable({

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



})

$(document).ready(function () {

    var table = $('#tbl_registro').DataTable({

        /*"bFilter": false,*/
      
        responsive: true,

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



})

$(document).ready(function () {

    var table = $('#tbl_asistentes').DataTable({

        /*"bFilter": false,*/

        responsive: true,

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



})



