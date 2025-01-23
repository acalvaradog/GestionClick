
$(document).ready(function () {
    $.ajaxSetup({ cache: false });
    $("a[data-modaldetalles]").on("click", function (e) {
        openmodalDetalles(this.href);
        return false;
    });
    $('#modal_Detalles').on('hidden.bs.modal', function () {
        $('#contentModalDetalles').html('');
    })
});

function openmodalDetalles(url) {
    $('#contentModalDetalles').load(url, function () {
        $('#modal_Detalles').modal('show');
        //bindForm(this);
    });
}

$(document).on('show.bs.modal', '#modal_Registros', function () {
    $('#contentModalResgistros').load('../RequerimientosPersonal/Registros/' + $("#Id").val(), function () {
        $('#modal_Registros').modal('show');

        //bindForm(this);
    });
});

$(document).on('show.bs.modal', '#modal_Registros2', function () {
    $('#contentModalResgistros2').load('../Registros/' + $("#Id").val(), function () {
        $('#modal_Registros2').modal('show');

        //bindForm(this);
    });
});
$(document).on('hide.bs.modal', '#modal_Registros', function () {
    $('#contentModalResgistros').html('');
});
//___________________________________________________________________//

function mostrarInputRdP() {
    //seleccionando elemento
    var div1 = document.getElementById('archivodiv');
    var select = document.getElementById('MtvSolicitudID');

    var valorSeleccionado = select.value;
    if (valorSeleccionado != '2') {
        //ocultar input numero en caso de estar mostrandolo
        div1.hidden = true;


    } else {
        //mostrar Dias de suspencion
        div1.hidden = false;

    }
    return false;
}

//___________________________________________________________________//

function CambioEstadoRdP(Id) {
    try {
        var frmData = new FormData();
        var Accion = $("#Accion").val();
        var observacion = document.getElementById("Observaciones").value;
        var EmpContratacion = "";
        if (Accion == "GestiónHumana") {
            EmpContratacion = $("#Empleados option:selected").val();
            var TipoConcurso = $("#TipoConcurso  option:selected").text();

        }

        frmData.append("EmpContratacion", EmpContratacion);
        frmData.append("IdSolicitud", Id);
        frmData.append("Accion", Accion);
        frmData.append("Observación", observacion);
        frmData.append("TipoConcurso", TipoConcurso);
        $.ajax({
            //url: '@Url.Action("CambioEstadoRdP", "RequerimientosPersonal")',//"CambioEstadoRdP",
            url: "CambioEstadoRdP", //"CambioEstadoRdP",
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
                        setTimeout(function () { window.location.reload(true) }, 2000);

                    }
                }

            },

            error: function (xhr, status, error)
            { alertify.alert("" + error); }

        });
    } catch (err) { alertify.alert("" + err); }
}

function AnularRdP(Id)
{
    try
    {
        var frmData = new FormData();
        var observacion = document.getElementById("Observaciones").value;
       
            if (observacion == "" || observacion == null)
            {
                throw "Si desea Anular la Solicitud, primero debe añadir una observación";
            }
            frmData.append("Observación", observacion);
            frmData.append("IdSolicitud", Id);
        
        $.ajax({
            url: "AnularSolicitudRdP",
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
                        setTimeout(function () { window.location.reload(true) }, 2000);

                    }
                }

            },

            error: function (xhr, status, error)
            { alertify.alert(""+error); }

        });
    } catch (err) { alertify.alert(""+err); }


}


function CerrarRdP(Id) {
    try {
        var frmData = new FormData();
        var observacion = document.getElementById("Observaciones").value;
        var tipoEmp = $("#TipoEmpleado option:selected").val();
        var Emp = $("#Contratado").val();
        var Fecha = $("#FechaIngreso").val();


        //-------------EMPLEADOS CONTRATADOS---------//
        var emp = [];
        var filasemp = $("#AddItemCont").find("tr");

        if (filasemp.length != 0) {
            $("#AddItemCont tr").each(function (index, item, array) {
                var fila = item.cells[2].innerText + ";" + item.cells[3].innerText + ";" + item.cells[4].innerText;
                emp.push(fila);
            });



            frmData.append("Contratados", emp);
            frmData.append("Cantidadempleados", filasemp.length);
        } else { throw "Para continuar, Primero debe ingresar los datos."; }
        //******************************************//



        //if (Emp == "" || Emp == null) {
        //    throw "Si desea Cerrar la Solicitud, primero debe colocar el nombre de la persona contratada";
        //}
        //if (Fecha == "" || Fecha == null) {
        //    throw "Si desea Cerrar la Solicitud, primero debe añadir la Fecha de Ingreso del empleado";
        //}
        //if (tipoEmp == "" || tipoEmp == null) {
        //    throw "Si desea Cerrar la Solicitud, primero debe definir el tipo de empleado que es contratado";
        //}
        frmData.append("Observacion", observacion);
        //frmData.append("TipoEmpleado", tipoEmp);
        //frmData.append("Contratado", Emp);
        //frmData.append("FechaIngreso", Fecha);
        frmData.append("IdSolicitud", Id);

        $.ajax({
            url: "DetalleContratacion2",
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
                        setTimeout(function () { window.location.reload(true) }, 2000);

                    }
                }

            },

            error: function (xhr, status, error)
            { alertify.alert(""+error); }

        });
    } catch (err) { alertify.alert(""+err); }


}



$(document).ready(function () {
    $("#MtvSolicitudID").change(function () {
        var motivo = document.getElementById('MtvSolicitudID');
        if (motivo.value == 2 || motivo.value == 3 || motivo.value == 4 || motivo.value == 7 || motivo.value == 8) {
            $("#EmplMotvSalida").hide();
        } else {
            $("#EmplMotvSalida").show();
        }

        if (motivo.value == 2) {
            $("#Todos").hide();
            $("#Nuevo").show();
        } else {
            $("#Todos").show();
            $("#Nuevo").hide();
        }


        if (motivo.value == 2) {
            alertify.alert('Si es un NUEVO CARGO por favor recuerde adjuntar el Manual de responsabilidades correspondiente. Si no lo ha creado por favor comunicarse con claudia.ramirez@foscal.com.co o asistente.procesostalentohumano@foscal.com.co y al tener el Manual realice nuevamente el proceso de la solicitud del requerimiento ya que este es indispensable para este proceso.');
        }

        if (motivo.value == 10) {
            $("#Cual").show();
        }
        else {
            $("#Cual").hide();
        }
    })
});

$(document).ready(function () {
    $("#Contratacion").change(function () {
        var tiemporequer = document.getElementById('Contratacion');
        if (tiemporequer.value == "PERMANENTE") {
            var tiemporequerMeses = 4;
            document.getElementById("TiempoContratacion").value = tiemporequerMeses;
        } else {
            var tiemporequerMeses = "";
            document.getElementById("TiempoContratacion").value = tiemporequerMeses;
        }
    })
});

function AddRow3(dialog, insert) {
    var field = ["", ""];
    var count = 0;
    var tabla = [];
    if (dialog == "Addemp") {
        try {
            var em = $("#EmpSaliente option:selected").text();
            if (em == "Seleccione...") {
                throw "Para continuar, Primero debe seleccionar el Empleado Saliente.";
            }

            var em = $("#MotivoEgresoId option:selected").text();
            if (em == "Seleccione...") {
                throw "Para continuar, Primero debe seleccionar el Motivo de Egreso.";
            }


            $("#AddItemsemp tr").each(function (index, item, array) {
                tabla.push([item.cells[2].innerText]);
            });
            for (i = 0; i < tabla.length; i++) {
                if (em == tabla[i]) {
                    throw "No es posible asignar el mismo proceso dos veces al mismo empleado";
                }
            }

            var nombre = $("#EmpSaliente  option:selected").text();
            var valor = $("#EmpSaliente  option:selected").val();
            var motivon = $("#MotivoEgresoId  option:selected").text();
            var motivov = $("#MotivoEgresoId  option:selected").val();
            var arreglo = valor.split("-");

            field[0] = "AddItemsemp"; field[1] = "<tr><td><a href = '' onclick = '$(this).parent().parent().remove(); return false;'><img alt='Quitar' src='../Contents/image/Trash.png' title='Quitar'/><a/></td><td hidden>" + $("#EmpSaliente  option:selected").val() + "</td><td>" + $("#EmpSaliente  option:selected").text() + "</td><td hidden>" + $("#MotivoEgresoId  option:selected").val() + "</td><td>" + $("#MotivoEgresoId  option:selected").text() + "</td>";
        }
        catch (err) { alertify.alert(err); }
    }

    if (dialog == "Addecont") {
        try {
            
            var em = document.getElementById("Contratado").value;
            if (em != "" || em != undefined) {
            } else { throw "Para continuar, Primero debe ingresar el nombre del empleado contratado."; }

            var fecha = document.getElementById("FechaIngreso").value;
            if (fecha != "" || fecha != undefined) {
            } else { throw "Para continuar, Primero debe ingresar la fecha de ingreso."; }

            var tipo = $("#TipoEmpleado option:selected").text();
            if (tipo == "Seleccione...") {
                throw "Para continuar, Primero debe seleccionar el tipo de empleado.";
            }


            $("#AddItemCont tr").each(function (index, item, array) {
                tabla.push([item.cells[2].innerText]);
            });
            for (i = 0; i < tabla.length; i++) {
                if (em == tabla[i]) {
                    throw "No es posible asignar el mismo proceso dos veces al mismo empleado";
                }
            }

            //var nombre = $("#EmpSaliente  option:selected").text();
            //var valor = $("#EmpSaliente  option:selected").val();
            //var motivon = $("#MotivoEgresoId  option:selected").text();
            //var motivov = $("#MotivoEgresoId  option:selected").val();
            //var arreglo = valor.split("-");

            field[0] = "AddItemCont"; field[1] = "<tr><td><a href = '' onclick = '$(this).parent().parent().remove(); return false;'><img alt='Quitar' src='../Contents/image/Trash.png' title='Quitar'/><a/></td><td hidden></td><td>" + em + "</td><td>" + fecha + "</td><td>" + tipo + "</td>";
        }
        catch (err) { alertify.alert(err); }
    }

    if (insert == true) { $("#" + field[0]).append(field[1]); }
    if (insert == false) { $("#" + field[0]).html(field[1]); }
    return false;


}


function SaveReqPers() {
    try {
        var frmData1 = new FormData();

        var empresa = $("#EmpresaId  option:selected").val();
        if (empresa != "Seleccione...") {
            frmData1.append("Empresa", empresa);
        } else { throw "El Proceso no se ha podido enviar. Debe seleccionar la empresa."; }

        var motivo = $("#MtvSolicitudID  option:selected").val();
        if (motivo != "Seleccione...") {
            frmData1.append("MotivoSolt", motivo);
        } else { throw "El Proceso no se ha podido enviar. Debe seleccionar el motivo de la solicitud."; }


        var cual = document.getElementById("Cual").value;
        frmData1.append("Cual", cual);

        if (motivo == 2) {
            if (document.getElementById("Cargo1").value == "" || document.getElementById("Cargo1").value == null) {
                throw "El campo Cargo es Requerido.";
            }

            else if (document.getElementById("Archivo2").value == "" || document.getElementById("Archivo2").value == null) {
                throw "Debe cargar el manual de responsabilidades.";
            }
            else {
                frmData1.append("Cargo", document.getElementById("Cargo1").value); frmData1.append("Adjunto", $("#Archivo2").get(0).files[0])
            }
        }
        else {
            if (document.getElementById("Cargo").value == "" || document.getElementById("Cargo").value == null)
            { throw + "El campo Cargo es Requerido."; }
            else { frmData1.append("Cargo", document.getElementById("Cargo").value); }
        }



        //if (motivo == 2) {
        //    if (document.getElementById("nombrearchivo").value == "")
        //    { throw "El campo Cargo es Requerido."; }

        //}



        var tipoConcurso = $("#TipoConcurso  option:selected").val();
        if (tipoConcurso != "Seleccione...") {
            frmData1.append("TipoConcurso", tipoConcurso);
        } else { throw " Debe seleccionar el tipo de concurso."; }

        var area = $("#Area  option:selected").val();
        if (area != "Seleccione...") {
            frmData1.append("Area", area);
        } else { throw " Debe seleccionar el area a la que pertenece."; }

        var direccion = $("#Direccion  option:selected").val();
        if (direccion != "Seleccione...") {
            frmData1.append("Direccion", direccion);
        } else { throw " Debe seleccionar el dirección a la que pertenece."; }

        var Numeropersonas = document.getElementById("NumeroPresonas").value;
        if (Numeropersonas != "" || Numeropersonas != undefined) {
            frmData1.append("NumeroPresonas", Numeropersonas);
        } else { throw "Para continuar, Primero debe ingresar el número de personas."; }

        var sexo = $("#Sexo  option:selected").val();
        if (sexo != "Seleccione...") {
            frmData1.append("Sexo", sexo);
        } else { throw " Debe seleccionar el sexo."; }

        var jornada = $("#JornadaRequeridaId  option:selected").val();
        if (jornada != "Seleccione...") {
            frmData1.append("JornadaRequeridaId", jornada);
        } else { throw " Debe seleccionar la Jornada."; }

        var horario = $("#HorarioTrabajoId  option:selected").val();
        if (horario != "Seleccione...") {
            frmData1.append("HorarioTrabajoId", horario);
        } else { throw " Debe seleccionar la Jornada."; }

        //var FechaSIngreso = document.getElementById("FechaSugeridaIngreso").value;
        //if (FechaSIngreso != "" && FechaSIngreso != undefined) {
        //    frmData1.append("FechaSugeridaIngreso", FechaSIngreso);
        //} else { throw "Para continuar, Primero debe ingresar la fecha sugerida de ingreso."; }

        var tiemporequer = $("#Contratacion  option:selected").val();
        if (tiemporequer != "Seleccione...") {
            frmData1.append("Contratacion", tiemporequer);
        } else { throw " Debe seleccionar la Jornada."; }



        var tiemporequerMeses = document.getElementById("TiempoContratacion").value;
        if (tiemporequerMeses != "" && tiemporequerMeses != undefined) {
            frmData1.append("TiempoContratacion", tiemporequerMeses);
        } else { throw "Para continuar, Primero debe ingresar el tiempo requerido en meses."; }



        var justificacion = document.getElementById("JustificacionCargo").value;
        frmData1.append("JustificacionCargo", justificacion);
        var requisitos = document.getElementById("RequisitosAdicionales").value;
        frmData1.append("RequisitosAdicionales", requisitos);

        //-------------EMPLEADOS IMPLICADOS---------//
        if (motivo == 2 || motivo == 3 || motivo == 4 || motivo == 7 || motivo == 8) {
        }
        else {
            var emp = [];
            var filasemp = $("#AddItemsemp").find("tr");

            if (filasemp.length != 0) {
                $("#AddItemsemp tr").each(function (index, item, array) {
                    var fila = item.cells[1].innerText + ";" + item.cells[3].innerText;
                    emp.push(fila);
                });



                frmData1.append("Empleados", emp);
                frmData1.append("Cantidadempleados", filasemp.length);
            } else { throw "Para continuar, Primero debe ingresar los Empleados salientes."; }
        }

        //******************************************//


        //$.ajax({

        //    url: "CreateRequeri",
        //    type: "POST",
        //    contentType: "application/json; charset=utf-8",
        //    dataType: 'json',
        //    data: frmData,
        //    contentType: false,
        //    processData: false,
        //    success: function (json) {
        //        if (json.respuesta != "") {
        //            alertify.alert(json.respuesta);
        //            if (json.isRedirect) {
        //                setTimeout(function () { window.location.href = json.redirectUrl }, 2500);

        //            }
        //        }

        //    },
        //    error: function (xhr, status, error)
        //    { alertify.alert(error); }

        //});


        $.ajax({
            url: "CreateRequeri",
            type: 'POST',
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            data: frmData1,
            //cache: false,
            contentType: false,
            processData: false,
            beforeSend: function () { $("#processState").modal("show"); },
            success: function (json) {
                $("#processState").modal("hide");

                if (json.respuesta == "Ok") {
                    alertify.alert('Los datos se registraron correctamente');
                    if (json.isRedirect) {
                        setTimeout(function () { window.location.href = json.RedirectUrl }, 2500);

                    } else {
                        $("#processState").modal("hide");
                        alertify.alert(respuesta);
                    }
                }
                // if (result == "Ok") {
                //    alertify.alert('Los datos se registraron correctamente');
                //    document.getElementById("Archivo1").value = "";

            },

        })


    } catch (err) { alertify.alert(err); }



}