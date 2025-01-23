
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

$(document).ready(function () {
    $.ajaxSetup({ cache: false });
    $("a[data-modaldetalles2]").on("click", function (e) {
        openmodalDetalles2(this.href);
        return false;
    });
    $('#modal_Detalles2').on('hidden.bs.modal', function () {
        $('#contentModalDetalles2').html('');
    })
});
function openmodalDetalles2(url) {
    $('#contentModalDetalles2').load(url, function () {
        $('#modal_Detalles2').modal('show');
    });
}
function openmodalDetalles(url) {
    $('#contentModalDetalles').load(url, function () {
        $('#modal_Detalles').modal('show');
    });
}

$(document).ready(function () {
    $.ajaxSetup({ cache: false });
    $("a[data-registros]").on("click", function (e) {
        openmodalRegistros(this.href);
        return false;
    });
    $('#modal_Registros').on('hidden.bs.modal', function () {
        $('#contentModalRegistros').html('');
    })
});

function openmodalRegistros(url) {
    $('#contentModalRegistros').load(url, function () {
        $('#modal_Registros').modal('show');
    });
}
//___________________________________________________________________//

function mostrarInputRdP() {
    //seleccionando elemento
    var div1 = document.getElementById('archivodiv');

    var div2 = document.getElementById('Mtv');
    var div3 = document.getElementById('MtvNovedad');
    var select = document.getElementById('MtvSolicitudID');
    var select2 = $("#MtvSolicitudID  option:selected").text();

    var valorSeleccionado = select.value;
    if (valorSeleccionado != '2') {
        //ocultar input numero en caso de estar mostrandolo
        div1.hidden = true;
    } else {
        //mostrar Dias de suspencion
        div1.hidden = false;
    }

    if (select2 == 'NOVEDAD') {        
        div2.hidden = true;
        div3.hidden = false;        
    } else {
        div2.hidden = false;
        div3.hidden = true ;
    }

    return false;
}

//___________________________________________________________________//

function CambioEstadoRdP(Id) {
    /*alert("Id");*/
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
            url: "../RequerimientosPersonal/CambioEstadoRdP", //"CambioEstadoRdP",
            type: "POST",
            contentType: "application/json; charset=utf-8",
            dataType: 'json',
            data: frmData,
            contentType: false,
            processData: false,
            success: function (json) {
                /*alert(Id);*/
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
    } catch (err) { alert("error" + err); alertify.alert("" + err); }
};

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
            url: "../RequerimientosPersonal/AnularSolicitudRdP",
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

function ModifiEstSelec(id)
{
    try
    {
        var frmData = new FormData();
        var EstadoSel = document.getElementById("EstadoSelec").value; 
        frmData.append("EstadoSelec", EstadoSel);
        frmData.append("IdSolicitud", id);
        
        $.ajax({
            url: "../RequerimientosPersonal/ModifiEstadoSelec",
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
        var filasemp2 = $("#Tabla_Anteriores").find("tr");
        if (filasemp.length != 0) {
            $("#AddItemCont tr").each(function (index, item, array) {
                var fila = item.cells[2].innerText + ";" + item.cells[3].innerText + ";" + item.cells[4].innerText;
                emp.push(fila);
            });
            //$("#Tabla_Anteriores tr").each(function (index, item, array) {
            //    var fila2 = item.cells[0].innerText + ";" + item.cells[1].innerText + ";" + item.cells[2].innerText;
            //    emp.push(fila2);
            //});
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
            url: "../RequerimientosPersonal/DetalleContratacion2",
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
        var contratacion = document.getElementById('Contratacion');
        if (motivo.value == 2 || motivo.value == 3 || motivo.value == 7) {
            $("#EmplMotvSalida").hide();
        } else {
            $("#EmplMotvSalida").show();
        }

        if (motivo.value == 7 || motivo.value == 9) {
            contratacion.value = "TEMPORAL";
            $("#Contratacion option:not(:selected)").attr("disabled", "disabled");
        } else {
            contratacion.value = "";
            $("#Contratacion option:eq(0)").removeAttr("disabled");
            $("#Contratacion option:eq(1)").removeAttr("disabled");
        }

        if (motivo.value == 2) {
            $("#Todos").hide();
            $("#Nuevo").show();
        } else {
            $("#Todos").show();
            $("#Nuevo").hide();
        }


        if (motivo.value == 2) {
            alertify.alert(`Si es un NUEVO CARGO por favor recuerde adjuntar el Manual de responsabilidades correspondiente. Si no lo ha creado por favor comunicarse con claudia.ramirez@foscal.com.co o asistente.procesostalentohumano@foscal.com.co y al tener el Manual realice nuevamente el proceso de la solicitud del requerimiento ya que este es indispensable para este proceso.
                            </hr><strong>Recuerde que este tipo de solicitudes debe contar con la autorización de Dirección Ejecutiva, por lo tanto una vez se cuente con la respectiva aprobación se dará inicio al proceso de selección.</strong>`);
        } else if (motivo.value == 3 || motivo.value == 13) {
            alertify.alert('Recuerde que este tipo de solicitudes debe contar con la autorización de Dirección Ejecutiva, por lo tanto una vez se cuente con la respectiva aprobación se dará inicio al proceso de selección. ');

        }

        if (motivo.value == 10) {
            $("#MtvSolicitudID1").show();
        }
        else {
            $("#MtvSolicitudID1").hide();
        }
    })

    $("#Contratacion").change(function () {
        var tiemporequer = document.getElementById('Contratacion');
        if (tiemporequer.value == "PERMANENTE") {
            var tiemporequerMeses = 4;
            document.getElementById("TiempoContratacion").value = tiemporequerMeses;
            var InputContratacion = document.getElementById("TiempoContratacion");
            InputContratacion.disabled = true;
        } else {
            var tiemporequerMeses = "";
            document.getElementById("TiempoContratacion").value = tiemporequerMeses;
            var InputContratacion = document.getElementById("TiempoContratacion");
            InputContratacion.disabled = false;
        }
    })
});
//--------------- Funcion modal detalles empleado saliente -------------
function Cerrar_Modal() {
    $('#modal_Detalles').modal('hide');
}
function vermas(Id) {    
    var ruta = "/" + "RequerimientosPersonal" + "/DatosUsuEgrePartiV/" + Id;
    openmodalDetalles(ruta);  
    return false;
}
function openmodalDetalles(url) {
    $('#contentModalDetalles').load(url, function () {
        $('#modal_Detalles').modal('show');
    });
}

//-----------------------------------------------------------------------------


function AddRow3(dialog, insert) {
    var field = ["", ""];
    var count = 0;
    var em = "";
    var mt = 0;
    var tabla = [];
    if (dialog == "Addemp") {
        try {
            var em = $("#EmpSaliente option:selected").text();
            if (em == "Seleccione...") {
                throw "Para continuar, Primero debe seleccionar el Empleado Saliente.";
            }
            var m = $("#MtvSolicitudID option:selected").text();
            
            if (m == "NOVEDAD") {
                em = $("#MotivoEgresoId2 option:selected").text();
                mt = $("#MotivoEgresoId2 option:selected").val();
            } else {
                em = $("#MotivoEgresoId1 option:selected").text();
                mt = $("#MotivoEgresoId1 option:selected").val();
            }
            
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
            field[0] = "AddItemsemp"; field[1] = "<tr><td><a href = '' onclick = '$(this).parent().parent().remove(); return false;'><img alt='Quitar' src='../Contents/image/Trash.png' title='Quitar'/><a/></td><td hidden>" + $("#EmpSaliente  option:selected").val() + "</td><td>" + $("#EmpSaliente  option:selected").text() + "</td><td hidden>" + mt + "</td><td>" + em + "</td><td style='text-align:center'><button id='ver_Det' onclick = 'vermas(" + $("#EmpSaliente  option:selected").val() + ")' type='button' class='btn btn-outline-primary'><i class='fa fa-eye'></i></button></td>"; 
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
        var m = $('#MtvSolicitudID').val();
        if (m != 2 && m != 3 && m != 7) {
            var tabla = [];
            var num = $("#NumeroPresonas").val();
            var cant = parseInt(num);
            $("#AddItemsemp tr").each(function (index, item, array) {
                tabla.push([item.cells[2].innerText]);
            });
            if (cant > tabla.length) {
                throw "Debe ingresar mas personas salientes para continuar";
            }
        }
        var frmData1 = new FormData();
        //-------- Mensaje si no trae Empresa ---------
        var empresatxt = $("#EmpresaId  option:selected").text();
        if (empresatxt != "Seleccione...")
        {
            var empresa = $("#EmpresaId  option:selected").val();
            frmData1.append("Empresa", empresa);
        }else{throw "El Proceso no se ha podido enviar. Debe seleccionar la empresa.";}
        //-------- Mensaje si no trae motivo ---------
        var motivotxt = $("#MtvSolicitudID  option:selected").text();        
        if (motivotxt != "Seleccione...")
        {
            var motivo = $("#MtvSolicitudID  option:selected").val();
            frmData1.append("MotivoSolt", motivo);
        }
        else{throw "El Proceso no se ha podido enviar. Debe seleccionar el motivo de la solicitud.";}
        //--------------------------------------------

        //---------------- Campo Cual ----------------
        var cual = document.getElementById("Cual").value;
        frmData1.append("Cual", cual);


        //-------- Mensaje si no trae Cargo ---------
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
            { throw "El campo Cargo es Requerido."; }
            else { frmData1.append("Cargo", document.getElementById("Cargo").value); }
        }
        //-------- Mensaje si no trae Tipo Concurso ---------
        var tipoConcursotxt = $("#TipoConcurso  option:selected").text();
        if (tipoConcurso != "Seleccione...") {
            var tipoConcurso = $("#TipoConcurso  option:selected").val();
            frmData1.append("TipoConcurso", tipoConcurso);
        } else { throw " Debe seleccionar el tipo de concurso."; }
        //-------- Mensaje si no trae Area ---------
        var areatxt = $("#Area  option:selected").text();
        if (areatxt != "Seleccione...") {
            var area = $("#Area  option:selected").val();
            frmData1.append("Area", area);
        } else { throw " Debe seleccionar el area a la que pertenece."; }
        //-------- Mensaje si no trae Direccion ---------
        var direcciontxt = $("#Direccion  option:selected").text();
        if (direcciontxt != "Seleccione...") {
            var direccion = $("#Direccion  option:selected").val();
            frmData1.append("Direccion", direccion);
        } else { throw " Debe seleccionar el dirección a la que pertenece."; }
        //-------- Mensaje si no trae Numero Presonas ---------
        var Numeropersonas = document.getElementById("NumeroPresonas").value;
        if (Numeropersonas != "" || Numeropersonas != undefined) {
            frmData1.append("NumeroPresonas", Numeropersonas);
        } else { throw "Para continuar, Primero debe ingresar el número de personas."; }
        //-------- Mensaje si no trae Genero ---------
        var sexotxt = $("#Sexo  option:selected").text();
        if (sexotxt != "Seleccione...") {
            var sexo = $("#Sexo  option:selected").val();
            frmData1.append("Sexo", sexo);
        } else { throw " Debe seleccionar el sexo."; }
        //-------- Mensaje si no trae Jornada ---------
        var jornadatxt = $("#JornadaRequeridaId  option:selected").text();
        if (jornadatxt != "Seleccione...") {
            var jornada = $("#JornadaRequeridaId  option:selected").val();
            frmData1.append("JornadaRequeridaId", jornada);
        } else { throw " Debe seleccionar la Jornada."; }
        //-------- Mensaje si no trae Horario ---------
        var horariotxt = $("#HorarioTrabajoId  option:selected").text();
        if (horariotxt != "Seleccione...") {
            var horario = $("#HorarioTrabajoId  option:selected").val();
            frmData1.append("HorarioTrabajoId", horario);
        } else { throw " Debe seleccionar la Jornada."; }
        //-------- Mensaje si no trae Horario ---------
        var tiemporequertxt = $("#Contratacion  option:selected").text();
        if (tiemporequertxt != "Seleccione...") {
            var tiemporequer = $("#Contratacion  option:selected").val();
            frmData1.append("Contratacion", tiemporequer);
        } else { throw " Debe seleccionar la Jornada."; }
        //-------- Mensaje si no trae Tiempo Contratacion ---------
        var tiemporequerMeses = document.getElementById("TiempoContratacion").value;
        if (tiemporequerMeses != "" && tiemporequerMeses != undefined) {
            frmData1.append("TiempoContratacion", tiemporequerMeses);
        } else { throw "Para continuar, Primero debe ingresar el tiempo requerido en meses."; }
        //-------- Mensaje si no trae Tiempo Contratacion ---------
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
        $.ajax({
            url: "../RequerimientosPersonal/CreateRequeri",
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
                else {
                    alertify.alert('Error al guardar, ' + json.respuesta);                    
                }
            },
        })
    } catch (err) { alertify.alert(err); }
}




$(document).ready(function () {  

    

    //-------- Tabla Semaforo Requerimientos --------
    $('#Tabla_Semaforo').DataTable({
        dom: 'Brtip',
        responsive: {
            details: {
                display: $.fn.dataTable.Responsive.display.modal({
                }),
                renderer: $.fn.dataTable.Responsive.renderer.tableAll({
                    tableClass: 'ui table'
                })
            }
        },
        columnDefs: [
            { targets: [0], visible: true, searchable: false, orderable: false },
            { targets: [1], visible: true, searchable: false, orderable: true },
            { targets: [2], visible: true, searchable: false, orderable: true },
            { targets: [3], visible: true, searchable: false, orderable: true },
            { targets: [4], visible: true, searchable: false, orderable: false },
            { targets: [5], visible: true, searchable: false, orderable: true },
            { targets: [6], visible: true, searchable: false, orderable: false },
            { targets: [7], visible: true, searchable: false, orderable: false },
            { targets: [8], visible: true, searchable: false, orderable: true },
            { targets: [9], visible: true, searchable: false, orderable: true },
            { targets: [10], visible: true, searchable: false, orderable: false },
            { targets: [11], visible: true, searchable: false, orderable: true },
            { targets: [12], visible: true, searchable: false, orderable: false },
            { targets: [13], visible: true, searchable: false, orderable: false },
            { targets: [14], visible: true, searchable: false, orderable: false }

        ],
        buttons: [
            {
                extend: 'excelHtml5',
                text: '<i class="fas fa-file-excel"></i>',
                filename: 'SemaforoRequerimientos',
                titleAttr: 'Exportar a Excel',
                className: 'btn btn-success',
                exportOptions: {
                    columns: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13] //exportar de la primera a la 13 columna
                },
            },
            {
                extend: 'pdfHtml5',
                text: '<i class="fas fa-file-pdf"></i>',
                filename: 'SemaforoRequerimientos',
                titleAttr: 'Exportar a PDF',
                className: 'btn btn-danger',
                orientation: 'landscape',
                customize: function (doc) {
                    //Se limpia el titulo por defecto
                    doc.content.splice(0, 1);
                    //Se crea la fecha del footer
                    var now = new Date();
                    var jsDate = now.getDate() + "-" + (now.getMonth() + 1) + "-" + now.getFullYear();
                    //Se crea el logo del header codificado en base64
                    var logo = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAY0AAAIoCAYAAACVnrcRAAAACXBIWXMAABcMAAAXDAGKAo5mAAAAIGNIUk0AAHolAACAgwAA+f8AAIDpAAB1MAAA6mAAADqYAAAXb5JfxUYAARZkSURBVHja7L13mBzHeef/eauqu2c2IDGJUSRFihSjspUzFZzkKDnJWT77zrac7nc+h3OUZftsyZLPSbKVrJyzZJFikChSzBRzBBNIEETGhpnurqr390fPLhaL3Z0dLBZYAPV9nnoI7kz3dKh6v/VmUVUSEhJWFu56ZELPPnlY0pNIWGkw6REkJKw8fO6LF6eHkJBIIyEhYXG4eufR3Lu1TGaAhEQaCQkJ/fGsU0f56/ddkx5EQiKNhISE/njlBUfxoSse4Y4tSdtISKSRkJDQBy889yTRdov/+6FvpYeRkEgjISGhPy48c4SPX3o/927uJm0jIZFGQkLCwvie00foyDB//aFL0sNISKSRkJCwMJ5z1lFICHz8yoe4bhNJ20hIpJGQkDA/nnneqRQamQiref/HvpIeSEIijYSEhPlx/pqWjKxVrPf88xVbWL95LGkbCYk0EhIS5sc5px4HeYapSt72ie+kB5KQSCMhIWF+vOCcEwlRKcTy75eu596tO5O2kZBIIyEhYW4876nHgYCaHGKbd37xjvRQEhJpJCQkzI3zT1kNwVNpDRE+8I0H0kNJSKSRkJAwN04/dkjWjVqMjaCR8XH4s0/emkxUCYk0EhIS5sZ5p63Blx6T5xAn+KfPXZceSkIijYSEhLnxPReeSpaPEKuKzEU27xLe+smbk7aRkEgjISFhb7zk5FXgA6jDK+Ay/uazySGekEgjISFhDpx12lHYzCJWEDMEPtKd6PKeS+5N2kZCIo2EhIQ9ceZxI1KHcSAS60ghhjoIf/OJb6eHk5BIIyEhYW8857zTMOJBPcZZsBn3byp572UPJ20jIZFGQkLCnvieM44hUCKmplNXIBGX5fz1B5O2kZBIIyEhYRaeslrBGsQacAaTO3w5yfptls9clbSNhEQaCQkJM/Ccs08AhVg5jFdi1QExYJR/S6VFEhJpJCQkzMTzzn2SgIV8LS0MNnpojyJhF5ffvo1v3Pl40jYSEmkkJCTsxlHGA1uZ1EjIhykmJ/HGUTnlbR+4Jj2ghEQaCQkJu3HBWafjjEEMECNqBIyB4Pn2HQ9zz5bUEjYhkUZCQkIPTz1lHb4Ozf8oKIIBTObo2hHe9rFvpYeUkEgjISGhwWnrsuYfNgJCQIkxErtdEMv7L74tPaSERBoJCQkNnv6Uo7HGYFAQQTGICLbIIHjIV/EnH74+magSEmkkJCTAa55xgoQqEEKFiKDSLN/oA4hgQsU7v/Td9KASEmkkJCQ0aA8NARGjBsRCVFQFZwRVZWfV4p1fvCNpGwmJNBISEuCEE07AZBkhBOgFS0mWEcouSguI/MPnrk0PKiGRRkJCApyxahsa2uC7iFVUwIaItkcY8duhgge3DfOvlz6atI2ERBoJCUc6nnHeWYgoeVGgdY2I4CPgxxh3a2m7CPU47/3iVelhJSTSSEg40lHELhoDVdkF59Ao4AQrBoIh+ooiE667dwuX3rk5aRsJiTQSEo5kPO+CpyB48vYwhAiq4LsEMwJWKaMQBciHeefnbkoPLCGRRkLCkYzjhgVnlKr2ED3GOoQAISMzHqyjrmuwji9d+xB3PxGTtpGQSCMh4UjFM568VvAloIgxxBgR5yB0qaOCUcQCQYk+4+2fuDw9tIREGgkJRzLWrBpFsgIRARViVDIBMQZjmixxQk3ucj5y5f3pgSUk0khIOJJx6mlPRusaVcVYCxZAoJ4kqhAjtFykqgLjcYgPfuGbyUSVkEgjIeFIxToep8kKz4jikejwNoDNESJiHGU0SB6QuuYtX9mcHlpCIo2EhCMV5z/tqWAtIYZFfX/nlp188OuptEhCIo2EhCMSQ9ZD8FiXs5i2S1JZ3n/5Q+nBJSTSSEg4IjWNM08E0ab+lPRnDY2Gb9+7g5se2J60jYREGgkJRxqOG3FA04hJtD8PGGepuiX//JUb08NLSKSRkHCkodBJhAg2W4yiQSSAEz56xV3p4SUk0khIONLwnLNPFWcMeI8uRtOQJnN8YjzjrZ+9PZmoEhJpJCQcaYixp2mI9P2uxJpY1TC0jg988br08BKWBJceQcLBwC0Pb9fHd3luvPVedslRbNy8lUce30aljizL8ZMTjBSW0085kTWrKo4ZbXHh6cfz0qcdLenpwfHHrmHDxi4mD4RoF/yuz4S8Ewko9+7w/Oc379A3veSc9BwTEmkkrFxcecfDesktm/jajRv47gM76XYEk2VEX2Pt7agxxGBADdgMNEL0uNs3YdVShgjGgkQ9ZkR43lnHcNEzn8JznnYSzzt96IgTgCcffxwbNj7UaBwsTBpCRrCW0B2DtuMjX76PN73knDQpExJpJKwsfPq6jfqJazfy5W/dzsTYJHlrFVWtQAa5EGMHWoZYK6KKdYIqxFhO11DyviYYEAPWKjEKm3cqX7puM1+6YRvoDTzlqKjf9+IzeMMrz+QFp6w7IgjEhQ4ASv/b1S7EwkA02Dryjdt3cN+2Ss9YlydtI2FgyGIcaQkJi8UDW8b1Q5fez7s+dyPbOyDVLnToGEJZIQquJ6a8KCoGrEFQtK4BxVpLjBEN2mgc1kCwQMTiUWoAonOAgEJGpA4Wo5Gnn9zmN3/4Gfzcq846rAXiD/zl1/Ur121CTdl37+eCpbYek1lkPBBcxptffTrv/vUXJNJISKSRcHCw/olK//pDV/Of33yAruZgA3QnMcPriPVOMAamiEBBopJFQUOgzh2I0KQ367RRBTGN6UV3gXGNhoI0pqvgcRqwGih1Hc51iFoSbQEBzjphiN95w4X8ysufelgKxj/4+J36Nx+5nmi6CNnCpBEjtVHAICGg7RZDQZn41JsSaSQMjBQ9lbBk/NFHb9Nn/eq7ec83H6UbW2AEIxlZewTX2Y74AgkWYw1oCXESskBlA3XLYahx4rFaYmKJ1QoTu+A7WO1QxIy8Nphuhanrhiwkoga8EzC78GKI2SiEiDEV9z+2mV9/x6Wc+Muf1/df8eBhtzMakoqoYVFL2KvvkTLgFFvDJMIHL34k7RgTEmkkHDh85sZH9cJf+Ji+9ZP3ssOPgu1CPoGEMezkdrTsUJk2uUw0RICAtEALjDoIFVnsEqPBBwiaEW27GZIDFsVRRk9lDTEviG4IL0OE2CLEHA0tjB0C9eB3IZkQY4H3o3i3lse2jvOL77iY1/7Rp/T2x8vDRkg632kS+xYRcmtdC0KjvRkpCGUNVcXffSqF3yYk0kg4QPiZv/qa/tgffZVbxh0mPoqxFTZaKMEVLWpn8K0cnFJmOd4W+BpMzEBzYrDg2tQ4jAER05iwYuz5N0CsEEMJ1mENELtI3IVlHEyJ2EA0HqHEolgtMFWGUECeoXEcYScaWlxyS82rfvtj/P2X7zosiOMpp56Ecw5i7K9p1E3PDQFClWFzaDHMrRu3ceXt9ydtIyGRRsLy4er1QU//+Y/rh2+YQNttbLkZlRGUjIgizuJ9RFyBhIhoRNQ0fR4cqHQRV4PUzd9EUAEkIgREImJBevUxxDpEpGlrahxIRlSLiAUMIpaIEMUQDURbg+kgdHvfGQJjCHaMx2vL7/3btfzg315+yAvKE0aHqKXEGNt/kdsSoqAYMCURoas7GHaRv7s0pEmdkEgjYXnwLxffqxf9znt5bEeJCxuhHkft6Mo35ZgO0Ia6gzWer1+xgaf90n/qtx8eP2TJQzSCOGJPK1sI08EuqiANeWR5i4nSc8lll6eJnZBII2H/488+eof+7j9eyUS0lFRERnBumBirFX/tMQpiBEKbTArKvOSurfBTv/8prnzAH5LEEXwXVHFFsXjS6GluhEAdIllrmIla+fOPXZ9MVAmJNBL2H37hnZfpX3z4ckpbNL4FyYhqENMLiV3ppOEM2oG8yCjpgEYMgQ3jyuv/6PP862XrDzmhOdx2UFb4RXTvc87hjG38RargMohQR0FcwYcuTdVvExJpJOwn/Ld3Xabvv+IJrM0wYYIQwYlgItR1AONX/k2oxxU1VXcHGi2ZW0VLHVE928pxfvOd3+Az1zx2SBHHM5/2JMmHRpuyK33gqx65GMD3ggyyHFTRCPdu6vL56x9K2kZCIo2EpeGtn7pd333xeog1lQ7hXRsyxZcdwCEqOLvySUM0w8duY8pxLepyB5MEnGmTU1JLi99457e44sFDy8dRVxX4/s/f5I0JK8uyaeLQ0DvOGMgKPvDlpG0kJNJIWAL+7dK79a8+cCVkLbJQw5CDaoIiWqRYR8w8YsYJZbHi70VVMHIU3kesjgEOWhbPJEzk4Ls81vX81B98misf2HHoEEdUzCJ8GoIF76l7TnPnmrIsEEED1JFLb3mc2x4bT9pGQiKNhMFxzYPo7739ciatReIEddGGXU8w3G5T1oLqJEhFpI2RbOXfkK4iMoGzEalbiBRQNomGYcjgxAAdHptU/uc7v3XoLF5jFhU9FYLH5m2MMT3zVIQYEI0YFBHHzrrmvy67NU3+hEQaCYPjot9/H+PF0bgQEFpI6CLFKJM+IpkiKBIdItrkRaxwiNmJiBBwBBfBekRqRCIxRnxeQGUxUvOdDeP83DuvOiR23FHoBSP0u3+I2nT5kywnaC+KylhUDKKTCEP87XceSZM/IZFGwmD44T/7gnYmJhE6BDtEjEeAxaJSspYCAnWHT192O+/56n1HjKlGQ460K55Yv5X/ui45xBMSaSQsEp+69jH98u2BIAWODqoGsiOg5UoMhLKLydoQSiZkDX/60e9w7xM7jwgBKpIRY4eWGeZj3340LYSERBoJi8Ovv+vL1BMlSoF6BatwCCTvLRVZYbG08ZNdTGsV0OGxbYE/fs9NR8R7FxshtADPxy+9Py2EhEQaCf3xJ++7SjftzCCvcVlGiDkSO4j4w/7eQx3xRRuMIVZNaXVaLb5w3eN87dbHDnttI2gXwgjeCR0P//z1e5KJKiGRRsL8uG971L/+xE1YZ0ErxE9iXdG0QwqHP2lQg2YRFHKBWAbEl3SqXfyf9117+N+/VYgej4WhwD98+Pq0KBISaSTMj7/8z6up8mEEDyYnRo+PJWLbqOSH/f3nrQzGJzGFo5YumBYWJWsr1923i3+7/DB3DvsIJvTKrHse3Oq59uGYtI2ERBoJc+MDlzyEs5N4MnIfCDbD5ILWHkzrsL//LhMMM0wMNerAicXXnqgOXME7P3LF4S0EXA4SQEqIlpAJ7/34t9PCSEikkbA3fvQfv6tiK4IOIXhqS9PjwmvT10Lrw/4ZCBmTrkQwSMgIxiO5bfp2aIc7n4BLbusetjvvCDgMNkJW50QyPnZTKiuSkEgjYQ5cdc1N6SH0gyrv/+I3D2vaDOpRcoKJICVVV3n/xYN3Ofzjm7v6h4+QTFuJNBIOR3zgsod0865OehB9YE3kM1fdc/gKAbWoBIxkqAkQa7oePvT1wbWNN5zW4h33PM7Rl2zSf9mayCORRsJhhU9edS9q2ulB9EFE6MQhPnLlg4elEJSoIBY1ioqHkIFkXHf/Dm7Z1Bnons9fjZxJTdBR/vj67Tz9sk360Yc6iTwSaSQcDvj6dfcSjUsPog80RPKhIT5w6eGpbUyRRqQGFKGNWsOuWPGZKwZP9vvJs06gqjxbJXJvNcKv3jXBD14/oVfsDIk8EmkkHKp472XrtaYFIaaH0XeVGIKvuezWxw5P0hAFDKoRUUFEAAWUT15yx8Dn+/2TrJApIyZHZZKJPOcrWyLfe/UYP3vTdn1grErkkUgj4VDDN767AbEWNJFGX6HqIsErdSfytes3HHYCL5qIRAFyTHSonQC1iEbu2DjBFbdsGfieX3qMUEbDsDcUk0LLVOQ28rGNludeU/KH93cTcSTSSDiUcPWdD6Khxtm0dvuShq/AFuStnGvu3HDY3V/E9zr5tRunuBkDY8mChWIVX7r46oHP+cYnCc5bKlcjeYcYI3W05BIoRXnHfTt44Tce0E9sSc7yRBoJhwQe2dyFaFHN0sPoA5UW+J0EjXzu3pVRVuXqOzap4hrH9ZJZMUecIjKBtwGJo0isqLMcCWP8882D/8bPHT8quI2YcpgQBaMQqKgKwQdhSEe5dXKEn/3uFr7v2jG9bzKRRyKNhBWLb932mPo6gnNEDemB9CMNVVyeE+rI+vUProhrKmsPocbK8gcyTG7fzsevvHtgof76E47GF4IVR24ycnVYHwm+IhKQzIEKX5+Y4LnffJy/vTtFWSXSSFiRuPuRbWAyRIBUYmgxqgYaIMtzxnZNrIhLEteGLCP45dd8nMv48GWDk+VPHdemChNEtWgAG4RMBYh4CYg1iDe4sg3tVfzv+yKvuLKrN45rmpSJNBJWEh7YPAGuQH2FSTNgEavEAlB3S4xzfOf2g18ufbKWJurpAMhXX3kuuWnw5kw/cIyTUwpBraEUwWvEGEOWG7wEvEZsNBRas2us5OgscuNExYuu3MZf3lMn4kikkbBScP+mnaDSmwGSHsgilkn0FXm7TagDu8LBr/z78MZtaFViswPgk8pbdL3j/35l8LIir1w3jJqKmEGdOepetF4koqpYcYy3WrRtSSmTaN3FasEfPzrO82/YpDdOJF9HIo2Eg47Ht01CjDhrCCnfalGwVqi6HbAZN9710EG/ni07J0AUPSCWHI8Vw4e+evPAR77++GHa1SQauoTMEBQ0KKbHBarKaB3wwVDFjJBnGOsZ9hnXb27z4m+P8ecP7EyTNJFGwsFEUNO8eUk5Got8YqARyTNcnrOzPPjPbaIbcLkjHgCfBlITRHngkXFu2zzYzv91xyFPHWrT9hUSA0KGjQaH9PSNQNlVsqGc6DyowVQF1MpQnlFUQ/zj7TXf+91H9c5u0joSaSQcFGR50yPDVzVkeXogfYWmYK2gIeDLSVx7+KBf0vadE4QQmgTNZZcSAUXwZeCzl9w98OFPf9JRrDEWExUrDWGYII2FNBNaRaSqFRvaiAa6xTi4iliOMTn0OBNZzmWPrea1V27k/RuSryORRsIBh8dBiBhbYI6Edq5L5YwQ8dZh6y7OOGoOvqZx1xOTqOaIHoAlHJXMKp2i4D8uu33gw3/nuDG2ZUdhUYZCl26e402L4dqiscIj5IBVj4iQRYdRg5iMvB6iFYRcSzbJML926zZ+85bJRByJNBIOqLElBJp4W4BkouqvaAghKCoGYzN8d/KgX9N4VZM5e4B8Gnnj+4qGx7ds5/r7Ng/0o2etHpWn2EkKdahGolbkMVJmgSJziMheYyZUugRnkFCxumjz3g07ecW3t+s1Y8lclUgj4QC99ADiAIPGpGn03WhHBTHYrKCqPKuHDr5J754HNlB3JjEHImZaM1QVI5ZalY9+88GBT/GDRwnGGKwBqzV5bhAJRB8wotNDiAhxj7/5IsNqwagZYlN3jJi1uK7M+JnrHuR9D/tEHIk0Epb9pWsFIdCriJ3QB9Y5iEJd1RhrcfX4wdc0vIJwYEKmBZQA6okCn77q4YFP8ROnrSHELhbBeKgs5F6xJt9Lq5utcWTBUTlPt644WVYxZA3BT7Btcoj/eW/Fb303+TkSaSQsK45f24LeolRJ660fQl1BnpG328QQeMbZpx3U63lga6XRZlibEeoDoSlGwGOdgq95aHvJjY+MDzRxnjaKnJJ7orG0sjahChAsmbiGSJQ5h0UIoWad71Dknl1iiZ2MQjJCy9CNkQ9u3MlLr9uu940nc1UijYRlwVNOWIsTRVXSKlsEnDUQoJqcRIxhTXZwTXr3P7odNKIRnDsQ5imPGINKU68MY3nvl24Y+DQ/csIIXj1GcoYiSJETyxpBMcIeQ9Dp0TaeSavgIeSebktRLGqEVfUYPoPrdxS86vod3LYzTelEGgn7HScdPYTWXVBDqiOyCJkZ6kYzyzIEeN45Tz6oafQbto4hzhGxhFgv++81yqjDh4jVHKoOn7rywYHP80MntskJhLImM4Esy1BbYZDpIdr83sy/BTHYOqdyjsJ3GTJdolWyiYhvtchVGKkrwmSbF12zmQ8/lvwciTQS9itOOXYtRWYbm3jCIhaJQowYlOHhg99T/ZHHt6K+RozDHICXKAoaHdgc9YIVz6aJFhd/Z7DKtxeMGDl2ZIghZ6mp0bJC8j2j9+aKpPLaorCW0bLCYJG6YKhbYNttqlChwSNWGXddrBvmLbdv4bdv3ZaII5FGwv7C9z3jBFH1GMaRKvXT6Cs0XYA6EhCec8aag3491z9YA6tQdhJZ/kiuaC0udEEjMcuJFYiM8d5rdg6ubaztEk2JFwuZYLoOp4JlzzHTr1EYTzA0/T3EYsSjWQnqKSjIwggTWQ12nFa3S9Q1vG9L5Je++0QijkQaCfsLTz15DWTDaDIB90XlIWtlgOW55z7poF/Pph2ToHXjZzgQeRpRsS6HugRViqJAEa6+/paBT/XqJx+DDYGWyzAxkBkHEqdDbekNEZ0eRiNGI5bd/545ciMMaUZLcnKjtPFobbj48ZrvvzrVrUqkkbBf8LILTyRqlkxUi4DNh6jrEhHhpecefdCv5/b1GxHVpkXrgSgjMpVsl1lAqYIHk/PQpl188dbB+oc/fxg5OQcbTTOM2cMUZUQQ5p+Wcyb/UWFqUDLGbcSbLsMuY1LW8u2xIZ5+6WN6/Y7k50ikkbAkvPJZp0DZRUxaS/0QvIIx5GGM1557/EGn2XFvUF83+SMHovGiKmVVN1Fk6pu+XcaQrz6ej3z95oFP95oTh6ljwNoMVb+XWcqym0BmjikyadJTdv89ui4Gj1GH2hw1itEuw9Jljal4sB7i52+d5PrUUjaRRsK+4weeebIMFTWa2r32h7NIDLzmBacf9Eu57LZHlVrJC0fwB+bdibNI5pryMyGQ5Tl0S6ogXHLj/QOf74dOadEWD2JA/F6Z4CKKZfeYaZ6ac1iLdUIelZFgaWmbqNCJk9S6g3XOsqWyvPbKbXz+kSoRRyKNhH3FKy44FidpCvRFDGShy+uff9pBv5Rv3vYwFBlVt0ZsxoGoHaa+av4bAljTaBpWIAS2TRo+d92mgQTx+S3k9JbiNeCs2SPEdi4No9+QOiM4R8eV1NRYFSzDiF2FulHGwjg2TlJY+N07d/HFh1IGeSKNhH3CT7ziHGJdpQfRV2pGWozziy8786Cbpm7fOIHVgGsNobFJglt2TQNFjG38J9YQ6honEStCjJZLbn584HO+dF0bpMLpnuG1M3M1pn9/jjDcPY4JBhMN4izRQcRjQ6DwBlPBiFlF1MCQloxn8CsPlHz4gWSqSqSRMDB++sVPkVUjo+lB9FskxvC617xsRVzLfY9tg7qL9x6Mne6At7yk0QhuYgQfmx4evsL1tJxvXHPnwOd87jGjZBbEK4Y4PWZGThkU0T0jqZgRYTU1MicUtTDcMQxFQ0bA2QpXdCHrkvucdhxlwo2SxYyi8vzR+h38n7uSqSqRRsLA+N1XrUGtwYQSZQitLeoVLBSxg4bDP7zKhhpVh4bQJLKRgxpy9ahXXNfwy684ZUVc600PdgnBM4QBCQTrll/RMpaoNSJFEzghgeDaVNKUF7lrW4fr1g8WnXTRMcgxZhwrOfQiqAxCroILkSwqmQhWwOru4ZDpMWXSUomIjdALJFOxqDgIjizmdPIxoqsofMQiaCbgK760YSc/c0/SOBJpJAyEH/mBV2BCB4yFehJXZGSZoN2SrhnGucOfNKJtQQw4Z1EMELBSU1YeiiFe+fRjeNW5xx30B3H1bRuVuiYfGcXH2EROrYTXEz2fuGzwnI3nH7sOzfYMGRYBa+10yfcpMpkas4XXTAE2lQw426yVqWBjQzgaIhIVnGVnWXLHw5t5y93bE3Ek0khYLM45riU//cpzCAo2s4RqstdG1IEdwvv6sH8GMTqyrCLWBiVgpcKadrNzDZ7f+vFnrojrvOKOx0GVoE0aHMYi4eA30bIS+cS37xn4uFcfO4KPNVZo8k40Tgv7KKCqe+Vl7FGnajq3o9eLwzRjprnLEDExNomEArmxGCxZ3kaGhtmpymc3ZPzW3VsScSTSSFgs/vgnXkyLLkFM07fZ5CAZlGPgiiNgFQQkCmIiYAkeym6XvN3mpees4dXPOHZFqFtX3bsNbE4oK7TXCMXowScNo4aHt5Xc/Gh3IMH7urXIGvEIEWfASZPhPjPL3SAYVSxMD5FmTGkaU6Qyu9jhtKaRZZjMNU2gxDTte8uKqAZcTjtmfGGD8H9u25yII5FGwmJw5tHI//zJF0DdxY6sJXY7GGPJCwtHQHSVNRW+cmCrRiyZYVjdxnQm+IM3PmfFXOc1dz3eVCU2hkiAGFgJuZleHYjhY1c9OPCxz1rbxgTFIFjTWEnFKE4MmbFNwt+sENu9+2/sSSpTxGJoNJiaSE2gjBVRPYU15MZiI43WYUpGfMEHn4A/uz2ZqhJpJCwKf/4Tz5TTTxoilB2KwhCqkqpWxB0BPcQDRFehoQ2xButhbIKfeuVTePUFK0PLuG87+sSOCkIEZ1ANiImEFVDaXsViguezVw1uonrN8W1cjEhoNI7pkFuarG9C3DsEFxbM22i0n91NnCQqWZYhzkxHZWWqWG3MVqV2CFmgHYb5wBM1//5wyuNIpJGwKPzlm15IXj9BMC1ya0EDyuHfDzaowzgwMtRImnqCM0ci//Fbr1kxUQCX3fwQ2BZEhVghBozExsRy0Fmj8T/c+8hmbt84MZDA/aEnIcNFjtWI+F5GuAIxNJqAsncIru4Zcju7+eQeiX8iOK84r41WZiBqTYyhafhkBC0cqmCkxoaCv3tgOx/bkGpVJdJI6IuffMEp8rOvfiY+FKgqLSkbQXUErIKoR+HjZpA2Lgp/9Ts/sKIu8eKrbsb0CNwgGAPBdxt7zkGGoIh1aFVx5S2DlxUZHRkhd1kj4MXgjMGKYIzBWbtXlBTs9l+IskdS4N7XBiOSY3xEQ8RawWYOsSCWphthnTVmLVXUKUFz3nHHFj71eArHTaSR0Bfv+fWXyNNP7VC3Ril1GKknAd84FaNFYpPHoWJRHw9IE6Alb4StQ0NFDpiqxPgKEzJsnWPriiwIWdwGdjW5rfj11z+DH3vO8Svqxi6+aSfR1RBrIhYbK4wcA1qugOcrCCWYFl+4dsvAx79pVYcxE3C2TTCRUDdO68gENmTTjm0rgjNmelhp/jYVJWVFcTQayczRtR5xQiGCrZsExSnicRFa6vBO8HlkpFSGKHh4OOPv79zAlxJxJNJI6I93/95PcpxsRU0Lay14Q4yRaCpUO2TOQm3AtQi+s/JvqCpxeZtSS0LWIubHEHLw+QTerUbpUrMOI13OOq7FO978nBVFGLc8sEvLECBWjSNcMjzSFJtcCe16QyTgMFi+ffN9Ax/+9OOGGbWWIpZNPoVVxFSIWqLze5UYmdYi5ikrYkXmFXLT35vhJ7GhJtcwbQaznZo1PmdzUP7pzke5vkzEkUgjYUE85+RM/vbNFzEsu/C1R/OicUgGRY0liGIyD3UXyVav+PtxFnxVYbI1EEHjDvAlqAM3js9aEHdxdGa45Z/esOJUp89e/SCdaJrGSwioJYoAEVkJcQoKWTFErDw7q4JPXn7/QEL2vFHkpAxM9LhocDaAVAgFmBorip3ZlGmGFmFl79wNYFoLsTOIpNFK9iQPg4CNmOjRUBEKxRhoe0vWXscdxvCHV2/m9h2JOBJpJCyIn33VafKPv34RQ0UALHnuyEVAXVPEOlZYF5uSIyscPirYjNjxZHmG03GyWOHUgHZBPMcMCxe/9aIVef2f+879TRlxCzYCXsFaxIDGlZDcB3VnEtMeArF89eZHBz7Hs1cXlM6Q4TASkZhjjJl2ZktP4M8mg+b3Z5CKxr3MU9OE0vN/2FkjWgcux4oDZ6lbUIcK1w2s1iE2+Jy33rs1CYVEGgn98AuvfLL8xS9cRB4mqKpJShPACUQw2iIGgzHdlX8j4hqHse3gq0kkrkGsI0oXqrWsdZGv/tX3ccEZJ6w4LePuzaXe+siu3d3zgCZqyDT/1YNP2lE9xnpiXeIy4QvXDu4Mf/ExQ5R2yncBVptItty76YipmUTQaByNL2MhUmlIYjepmDl8HsYLGi0GRygrIgFTGAqNjFZgc+W2ceFN30lZ44k0Evrid77vdPnn33g1LTqQr4E60NIJRGo0y5veCisdChLrJjRUcmoctR0hBse61Rlf/puf41mnHbMiPfqfvfoBghpQAQ9qBGNDQxZRWQkR0WJc0987E6gn2ToBNz4wWJLci45CVhtPUN+E36pFNeDQvbr1TfkkdpdRn5tUbI9YdmtEMieprLYZEpXC5AzHjNwHHAF1kdJ5NHoyk/HdruWP7951xBNHIo2EvvilV54s//jbP85oZws2KioZIZSAB8lXvqJhwKjHSQtcBraD8ZOcbCf43N/8OM9/8sqtyvjJb93TM0tF0Lyp6EqAoBg1YA7+pSuWEAWjNaoCts1/XnLbwOd5xqgjmqagoMM3hGH8dPvX6ffZI5Cpku0z28BOf2dWSZG5alZN+ThCnCTEEqPQsgVZVoA2eR0+M+TaIoSaIYHPPa78v3uP7OS/RBoJi8Ivv+Jkec/v/whrhy0la8GtbXbvZuVHT2kQYjCodJoIJHVceGqLhz/7m/LiE1duzPD6Hbv0xnuegNDFxQjWNbWZYtXbOZtGjTropOHAtrC+RiXDRPjmrYP7NZ5z9ChZZhqB7UqsWoKUjYDvlQ7ZYzMwU/OYqj8116ZhVh+OmQmCIko3L3FDBomBjq+orSBYWmS0giUXR65KaQqKyvGFDWN8+rEjtxdHIo2EReONL3iSfPlv3sizTgFja5QMdAgjFRobgaFmGPVM93ZGTS+LuSlEpygqjajTKRNLP6EUHapNMUWVGpXQ/FsztHaoKVAxGK3JiZhaIOQIDlUPBDSzBNq4uuZXXnkqN/zDT634BJMvXvFYT7BlVJki2jReUpMhxlDZgOjBvw2j0Kq61PkQkUhkJzc/NMHd2wZjtJ85MZMQM6IDW3vEdjGyBlFFtCla73oWOaO615iqO+VmjT1MWzO0kiltJAst8sqgErEZZHWj5dQSgEhgEvIRqCpaI4GHTMl/PDDB9duPzIiqRBoJA+G5p7bk+nf9qLzloifTDh6oCd5CPtrURap20MoUHw1BXZNYR4barDENGTfFGKCm17+ij3lJJ7GmxhKQCBIac02mJYWLGD9JHgMhZpTqCA60ZYh1xYi0ME4hDHFM7vnwn38v//YbLzkkmoT8y+dupDgErjQSGjNZBDG2aawklm/dtH7gc51SKEYLxDpaavDe75GDMW16WqD961ykNjX2aBM7u9f4TEKZMQoiMXhcVlB2Kk7wQ2zrBN51/6NHpAxIpJGwT3j7f3uJfP5vfoBz19imfHp3jMxFsMN0tQVak7kSox1y7ZL5SVw5gakmMLFuTCt2am/Yb5K2MXEIrQs0tBBTEIylNp7SToAaggpOFYNHnEI1gWm3Ga+aCKMfe+4wT3zi5+UNzzzhkCCMax8p9e5toP4QuFiJBAMEBYkQLVHgS1ffNfCpnrVaEHEgGS6CEKd9D1Nl0Gc3ZtrDXzGHH2OqlLoIc5ZYn2numouAnAGCZ4SMtjpqUWI+zH2TBW+54ciriptII2GfcdE5x8lt73uT/PEbLuCEVYG6KrGmQ4txCIE6FMTgqNxqarcan40Ss2FUikbDUIVF9IOIRqm1IooHCWjomb20BXEYjCCZxUskmhwthba1FNUmLjir4Mtv/RE++Yffe0i1IPz7j9wAhVLpIVD7y0RqUawaiAF8s0f/zj2D5za8YJ1DRNAIqkKW2d2RUrOin+asSTWH4J9NIrM1kNll1WeavyxQxUDbZviypBBL3TKEGCmqES7vwD/f2z2iiENUU+hxwv7B2z79XX3bp65jbLLpeudQNELQCkR6jttI9I1fwxjXGIX7pDUrFjTibGiyE7xHIxjJURW8EYxVjIn4mEGMnLFO+F8/+Rx++aKzD8l+tUM/8iHt6BjGHIPG8RV9rSoRYqQlbcpYkZFR2QBRueIvX8tLznvSQO/ge7+9U7u1QUwFto2ERt2aLavmc+fsLdPMLHPaAvcylzyUCJrRNUIeuhQWxtUQa0fRbiOdMf7kwjW86hiRI2GdJ00jYb/hf//ohbLro78s//FbL+TcYzziJ1HfIXNN3aQQasAgtilIJ3g09I++EjUQBB8MVbR4UxCyFrVr4W0BVolBMaHmtReu4ZN/8DLufe+b5FAljD/+6E3aVY+oI/qJQ+CKI+CoY0BVUBN7dh/Ll699aOCznd5SjJGenyTuladhZtWimj32Nlvtzt2Ymb8xMzlwIU0lw+FNpLAQrBBqzyprsG0l3zmGd23e+fBY0jQSEpaKax+o9D8vv4PPfPsuto916HS1aSWrBkQx4kHrJmRzwa1st3GkR9cYD6xFfI34kkyUp571JN744vP4qReexmlHH/q7vVPf/DF9eEsXwaLUK39vJzWqLahrEItxFVEKEOHpJ67ipne9fqB38uH1tX5gYwcTa4Jm2BkmpSgLaATzaqp7axOLCTqb+o0YQXID3RKfF6hA3qkwhcUYQ+mhVMP5a0v+/elHH/baRiKNhAOCm+/fod+67VG+dccTXHvfEzyydYKoFlwTH7/g4o0RcTlaetpGOfvktTz7rLW88MLjefZ5T+bco4rDZqF+4LK79Zf+8UpCGEbMJBYlrPBGWBo7mPxoYjmOsy0820GHQTx5dJSf/7mB3s+t29DfvXs7eRVR02qis2YI8jiAwJ9pUNF59STto0cZiljhDaAFlRrUKC31TNBlRC1ecryr+bETct5y2uhhTRyJNBIOGr5z2wbdvmuC6x6pm6KCkrFzbJwHH3yQC847F2sixMBonODp553Jk0Yt556+9rBekKf84sf1kV0WujtxJuKzYSQe2n3a/+23Xs2bXzpY1NqPfWun1ggBT+yjic4nwULvE9PrbhhlcRqKziIdEVnQnxIFcgxjGoCcPzk347VHZYftPHVJdCUcLDzvvJME4HV7ffLsI/J5/Ot/3aNbduwEhiBGNG9DqOEQFz9X3Xwvb37pCQMd89S1ObfuiphIr/rU/JqEzEMgU2Yt7bm+pzLK99ZUzB7nlYYp9jz/jP9X1eleHCrg1NLRcUbMKDZO8r57Ha89as1hO0+TIzwhYYXg7Z+/iU4ZMKHE5TnB2CYL/BDH5bc8MPAxzzk6I6pFtfHsTA2jEaNxj7/NTtibXYfKzCo3Ypk9dp93umfHjDFFHNNjVjivRs+wtDFS4xG277L8zu0Th60JJ5FGQsIKwL9/Y73e++gktFYhsSmTQl2vhBbgS8bD2wN3buwMJER/+ElObChR2fMBzJV8N1PozyX4Z2eA71XcsEcsswlldx7HTILak1CESIbBGEcdlZYdRluBG7cKnzpM61Ml0khIWAH4m49/p+n5ES1qLFGbznxRD33WiLS45PqHBz7uWBtxxswbWjuzPPrMENu9BX/cY8wW/tMkNI9GMVNT2ZtQBGMySg2YmCF08M4yQsXHH+lyX+fwq0+VSCMh4SDjrR+9Qe97osZoE4IccagxWBsWkzB/CLCGcvFNjwx82FmjDXEiETG6x2iKkDVjtvlp4ZyNHhHozJpUc5m8dmsSc2kqM38viGJwDDvPuI+sUkdHKrSjfOC+8cNuvibSSEg4iLhrI/pXH70acVlTHM8piAXvm3Ip7tDXNEysue6BnQMfd+FROUq9V22p2WQwXfK8N4zsSTBmdsLejONnlxmZJoiZmsQsTWW2+ctJjY0GsUrLtunELqtYTaVdrtla86FHD6/+G4k0EhIOIv707z/EpDsRV42jGOLkzl41YNOQx2EQEt8i8PjmDrc9NljXu9PWtveoFQVzZ2xPmYz2+N4MbWMmoczWWvYwT/UIZb7fmfKhTBHKFBxKJpOM1UPkbpKWOuoA6kaI7YwvP/TEYTVnU8jtDFx350Yd71TYrOlGZwyEUBNshpWm+9vqdsEFpx8r6WklLBUf/Ppt+vEHBNGdeNvMOV+MIqEDAorpNQs6tOGNgBO+dt0mznv9qkUfd+aQyHG20h11QdSa4CBKhvWWTCI1gWCFqdxQmSc2eY/w3DloS2fUPhNhb6LW3XtrmVXwsDleEGMYpkRDDkYxpgRgqIZOvZr/fscu/edzVh0WcuOwJo3bH9mmm3eMc8MDFTvGJ9m0Y4IHN25ny3jk4c3j7OwEfFBsnECsw9cCWYaIQ71vyl1qhGh6symCRorMafQVxEZNPemEtawZabNmOOOoIcupT1rFUSOW0048hrWjQ1x07rpEMgl7YP1jpf72e2+iFQ3lYX6vAQEC19z6ELz+zIGOPXW4xY27mu6ATVOvphPfVOKeaFP6fKGkvbl6bMz8vkFm5X3ILM7QPkS0sMEmyyKbtlV8/pGorz/ZHPKy4LAgjevum9SHNm3jlvWbufaeDdy1YRubxwOTZQCbYbHE2BRR0xiwRUaoq6YgmhVC1KbVctY42FQ8SJNU1cRpCzFGrLVEhDLUTVZPloEq6zduxdisqWnjA1mWUZcleU9jqTTT3CrHrck58/jVnPGkEU4/dpgzTjmKU550DM85rZ1I5QjDL/3zZWybtBA9kh3mpBEEssi3bhm8adGFRw1x/dgkGIvx0rS9zQQfwUTBxamEPtlLmM9FGrs/n1HPah5imfr+XMfv+X2Zl1AAag2YkPGxRyd4/cmjh/z7PCTLiHzlhgf00tu2cMXtW7n1ge2UVQ00PZNjjE2Mu2S9dM0MdMfulxsjzmWEqWMQvCsaFdT7xo5sDGho/hNDz0zV606mZipFtGe/UloIEW2qfBLIWgW19833vUckn55I0zbaGW1OJZacf/bpPPmoggtPO5oLTlvFU44pePpTjkpkchjiDz96m/7VJ67GIERpHRYJfAtCDaol1g5x0ztew/lPHqyo3xu+vUOjLciCYkKJz4VaDXmwiEa8nV+GNRneZmFNYY4yITMxV62rQeRmVIu6iHjD9xxr+YOzDu1N4iGhaXzkmk16wx2PcNlND3L7wzupvIXMYqUkVOOQr4XoiURAm74NpiYG0GCwtGdUrPSoGFQyxChqDPgKMQaxBhEPUYnqe8VqAkEzmhwjRQjNRDSKRsVYQzdq08ZUFKKh9hGCNGqrtFDGe+cWYtCGzKz0JqtgihY333Evt48M8YVr7kajJ7cOl1l92lln85KnRM46+Wiee/aJPOOk4UQkhzAuvmNc/+pjV2GtbeZk6DZz5zCGMYZQCyFzXH7zRs5/8tEDHb/GBrZpQMiwEWoCGIuNBtWwl7N8IVPTbO1jtuYwFxmYOflh9/fVLKxpWJtRM0GLyHe2tbhxW9BnrrOH7DpekZrG+i0T+qXrHuAr127gm999GF8VYARvY/NCjEz3mW60Cw+SocY1AlkDRI+RgCNS4XsTw/amkYXYCGwAzUJzvimVUwP0nFuKgs/BSdMDG51SaJu6QMZA6EW79Iil+Tww1e7F0iZMeetMY98lNmGCZK7pdDbtM+mFfEQPISKZJStr8vYIE51J2pnlGWefyHPPPJpnP2Utz3raKZx1XJGI5FAwo96/Uy/64y/QmayotIDgca6J8z+sERW1DnzkR553Ep/+g1cONF//+tZd+t2xADJKXo3TKQJRWrS9JVDCjKzxueTZYgsVziv4xSx8fJ/3FzXDSElJl9E4wppVwj9fOJJIY6m4e+O4fvX6jfzn12/hjkd30Q2COIeEiMRO00pFssbAI4JSoxpAAtGsgVAh0eMIWBFUMryapqhy5nutRaXZIOiUZtAQhfUQY2zitKfMRtYQoqIxIKZodkuhng69c8bifUNGMeuRgIaeo06aUL7euQKKc42JytcRwZFlBUEjofKQ9whLY68RcmyaQ0/9zRfQ25iYXox4oNcq1TqOziqed85pPP+Mdbz4nBN58QXHJRJZgbjgd76ot67fguCwRHxs+oqIiYf3jcdmN26i5ei1wqb3/8xA8/NzG7x+9qHtlGYtRZik42qQYQoPtZ3ExHzxBDDHZzM1hbm/Kwuf2yz8e1l0TEpFXjjoVHRMm588FX76pKFDcp0edNL4/Deu03+9xnPltTfTjQ5vXPOSRADTbNgtzS5cegn+IYBMxbELyMTuf0+9QQ2988SmeY/ItEAnKtY0dkxVJdpGO3GmcZjHoIi1jelKFQmTGGOJKGIcsQ7gXPNbAhJ6JiuxjbMd7ZWEiDO0F220EAOiAY0RRBvnegi9vsiCSFOkTWOvHaoqZLHJDI67F6GIIAZUGz8Krt0zvkaGXMUzT1vNi887nqc/9QTe8LxTE4kcZDz7LZ/UGx/qoFgMAbQEJ0STI/7wJg0TITjFloaQdbjzHT/O2acu3l93w070X257nJ3mKIZiRdeViA7hvOLzDjYUiyaJxXw+l6ay0PHax+eRRUeFp8oshTcE57Gh5lMvPCqRxqInwcMT+v8+fR2fv24D2yuHhG6SKsu5aDXnvCev4aXnHMVrnnE83/vcUxKJHEA85bcv0UceeYS6W2PyrNlLYBthEwJiD/PXoTWq7SY6sa752ze/gv/5/YNtZH7xml3alYCphnC2QyCSh2HGXUmmC4fU9u3Sp2ZBQumnifSToaIBG1t0bQ2mSxYyfJ5zfmuCP73g0Mv5OqAeuA9d/Yi+53Pf5Zq7NlFK4yCTEFJe+nJbB8qd3LEh8N1HtvGuL91BK1d92vGjvPzCp/D8C87kx75nNJHIMuG5v/VZfWTDDurSY5xr/Ggam+A7hOhcTys+nEnDIKJobEyut92/ATh1oFOcPFxwz2SJsZGAoAJR4rzuhJmO7blDbhc+bo/Ln/X57HMsFK4LUBuPU8ViMWSYaInBsbEL39kc9XnHHFq5GwdE0/ibr2zVD3zyq9y5cRusWQXBgObY6NF6B+pGknRZTmS+kUvRMNVju3G0V4hERGqed9ZJ/OiLnsZFF57I+ScnEtkfOP+3P6O3rR+H0MU41xS3i1PROq5nopSmAN/hzBmqGCxRK7AFZx9tuPPdPz3QHPvEI+hnHhujIBKiARNw0eHRBSOfFmWm6hP9tBhSXEj76LouI36UOirGBoxXJm0L1QkuWGX4swsOrUzxZdU0/vzjN+vffeG7jE0ARnEjq/HjAVBsVhJ8jcvXEQ7xdpYrfmdQNuG/TgxgqL1v/C42QzOLo8tV92znqntugvo7HLvG6kUXHs8PPvcU3vCSpyYCGRDffgD9gT/6GNvHJpCsQGyGapMgCmBs43/T6V3q4d5y2aASkdh0475v09jAZzh7GCRE1HpECoI0mdqZGkKvh7jM53voRVfNqyHMevx7ZYT38WnMRVoztQ875WZVIVpBYiCjpmMd905ELt4S9KKjDx0b5bJoGn/1oWv1Hz5/F5vrHKzHxRINTX6EkRZR6eVUNC9UxCdJs6w7PQOxCSFuSrtFFA/4Jh8lDIMETGGJ0/4lC3WEvMUFJ7d50QVP5kdffDqveOqaRCIL4P9d8YT+xv/9HJnJUZvhQ6cJiphK7rSNAIteGy1DesEah/P8awxx2BDwNgNf8bk/eh2v/54TB5pLb75qh5a2BlrUohQhYkw+TRrzyrJ5ynxMfV+XOKNnHj+3TySQVS26ArQCrioxWILLiT5wYl7z9mevOzJJ448/fqP+02dvZkfX4egQjSWEYfJYEygIWkO7F5paNULMSkVIdROXd9FmrskpCTUSFQNNgiM5KkKuitcmdVGJYDKME2JVYYwB222EnMKxa1fx7LOO5zXPPJlXXHgi5x3fSiTSw4/+7bX6matuJbMRHzO0U+FaRa/20u4d6FS0nDEQ6hrJDv/5r6pYtQRRxAq/+bpz+YdfftZAc+dPbtqlD5QBJ5YOSis0UYpRFjY/9TVfzSKVfj6LftFTsz9vkg8ttShkYOoSiYq1GTFAbSw/cbLywycdGrkb+4U0/u3yjfquD17KHRtL7JAjhA6YDOqILZTgBTKBYCEM9RLpOljrUe9Qq0niLOtLnlLRGx3DYJuw48YfS2U7iBisWkxs8ksCgWgF4wRfKSZzREwvwbE5biTPWTMyzE+8bB3Pedrp/PhzTzgiCeTLNz6sP/W2i9lVjSK6AyQALcS1ifU42LzZa4epcqyCNQZrhbqu97KpH37zLxLFYqIj4nFWOevkY7jtnd830I2/596OXrnNk0mgq46hGClN42BeSvJeXGIkTqRf9JRFJfSq6RpEDUF9U7kiKsEVrNNt/NPzjj/8SeOa+yp9+3s+xSfuGkesotgme1pyjPFobBaEaGySrI0231MlqmtMICIIgYTlg/WCGu1lHocme72Z7dBrPKMiEMPu2luRpj5WFKxtsm4bB6RgbM+MFSqsswTfxliwocN5px3Na5/zVF77rFN4ydNWHdbS8K6N6J/+/Yf4+L2Bwig+QnCCCYrxDi8ecoWaJq8nxN27Xu+RqVIyhznVSqyJNkdCjlJhYgfNR4ifHjzJ7/OP1GS2Q4eCVVGZMBHTJ2S2v9Bf2JG9VPMUalFTIiimyoiuoBZFJGAVUE8QwxtPGeaHT5QVPxv2mTTe8oFb9J8+eSPRtsF0kmQ+ks1fPQ1EjEHVY02zq85zx9lnn80bzobnP/0sXn7e2sNCPN7+RK1v//hVvP9rtxHzYxEm0iRYUKgqWQ11ZsliSdAW4ib46p/+KBedf8xAc+JNV2/Ttl0FsSTYgA122qex75rCwpcQB7jCuUljYU1GQ8DkAUOb9z135RczHNiY+p7LHtA/ePe32bJjF1krEkNKskiYsroISpPVLkYpJzrcdddd/OGdAp+9izx29XvOPplXXXgqLznvOF523tGHFIlcfs+YvuPj1/Jf1zyMNw4dWgVxB8QsvfyFYAxGDOAJSs/SYLnzoS1cdP4xA51qVSZUtUdkdx8MMbIkTcHQ53gZcA3M/u0+gQ4ma9H1XVZJh/feGfUXn7ayi5IuWtO4b0epv/F3X+dr392FaQdit0JkGLXjiKZFc2TvJC2EiDGGGGtQcK5Xpj5GCimpyIjZ0HSpE2JJy3mectLxvO78NZx72vE868wncf7JK8+k9bsfu02/dPlt3PPIdmzeIlpp8l6C0pZA16RAjoU10UAWMmrp9loWWHCeH3vuyXzyf1000Pv+y9vH9KFdFmtiLwJT+ibvxT6EIv0KHvbxWcQBZ+zs3/ChABdoxwmMH+Y9Lx469EnjH798j/7F+77FZm/B1FA6pN1GmSCrLN6mhXFEC4WmjCiI9EpuRYiKcw7VgNDC113ITK9XiTZ1uULEuAzjlaBNxvDoSMb5px3F95x1LM84dS2nHD3ESy84sA72L934qH75uoe55LuPcd+jO8AWUE1ic4viiHVsSp+JEI1FNPnkFp4fNaIFyARqR5tgigxOWeV46L0/NdC7/eD6Sf32lsYLEcJUY7TBS3ssRCqzj1cZzJoyqMnfa45zntp7hJyXHB1485krN5KqL2m8+g++qBffuQu0izGO6MFmSigrjGuhlBz2rccS+qySZlFpCIiT6bBSesUhMYJzDt8pKVwGRHwMuMJRdichz5rorBBpWrJNFZhsiMjVNWuGC8568jGcefwwZ5+0mjOPH+XoEcu6kZzzTt+3qJPv3P6o7trZ4eqHJ3hkyxjX37eJ2x/Zhq8tuByrAXyJFYchUBOb8HBXYLTpzYJzSEx5RgsL0YCQY3UMb9c04d/OYHxF+PwvDfTuLt/Y1Y8/4hGjxBrEmgU1gdllRAYV6qraaNJLIIl+IblWoPKKWoOziul2+Y8XH7NiSWNevfqzN2zUX/zbr7Czk4Gte2pYC6wjdHYyUhjG/SQUo0io08o4ws0P1lqCGtTHpgy2yXp8oqh6VBVXtCjLEskyxGWU3QrbXkvoxqaIn3pMCBgjqLH4Xjl77wxbOsqWe7fy7bu29LY7rtFWrKXld2oIgaF2wZmnPZnRoZxQV8RQM9xqMVGPIyanqi133vMApYdohDrUmDxH1U4783FDmMIQq4rga9p5QUc9mFUQLaITmNghiAGEvO5S22SeWgiGRhuI0iSZIhGMEKPj8rvG9WVnL35XfUrbgHhUDVYMQSNmgX4XguxFKf16hs/+rhAHMk/tfS6z8O/Xk4hbRRa7BCPUWcYXHhnXHzx5ZWobc872X3rnt/V9VzyMUoD1SPBY18Z3dpK129RZzjgWshKq8UZ9TzhyEWNTwd5mRBE0Ns2ytFc2AzvU9GS3AdpZ87kIZC1Cx4OzIIoaSzBKJPZi2vdclRJ12ikpEnsR24GuafqN7OyW3PjARjTU2F7iXLPTy8hyh1ePxqZkvhkahdIT1SChS+Yc0USC7yARXK9/fFVVUBQIuxqTiIwQdBgbS6LtUNnYpGUkLDQ9mvcrvZ7oVkE9ThwX3/QgLzv7vEWf6/Q1ueRZqVUdMJITxQOKLJDgZ+iXnCeL7hE+Jynu5fiWWeavPpqOMxQuI1Y1IdS4POdbG0p+8OSVWZNvD9K4af2Y/sq7vsr1D04CGUKkFWo6Zi0xdmm1DV0/DlkLvGJwZFqTKkcd2ciKgrosCRp6ZTGkKSc9ZQsOO7FZgYaIVlMOc48CtnCIdghRUDUIWfNfBasgqngbe/1GphZwJPYWcwwR1JHlGXUEowbvwTsHtmfmCkLdjYDDZgVYJUx0sNYQtQTTova9HvDWTVUyanIrnEXCBGpbBDzIGGKUEAW0wHjZk+ASmGfLjhjXK+gb0RDBK1fd/iBw3kCnKoqM2jei2Ji9tYz5akDN/NtC3+l3/D7Q5l6csofPJGvhJ8eQ1ips2I4CO3WEix/q6kVPXnkVF6Z9Gv926d36J++/gU3bLKYNcXwrRTZKhUWnamn5GskytC4xrmkeZG0vTjoKIm46DC6GCmMj1kLdJ7rKxC7GWILGpme2KZrHHCPYDBubekjGGKIKwTfqrbhm12p6zZSaF6KIzighYBTBEmKNdQ5fdxGTYYwj1AFjM9TUgJlu/yr0VFINWIQasM4RQmgaQLmeMIq9RLnYtJ3V6LE9ZTioRawjxCOgM1tCQj8Tpu5djdZai3OO7scHc4Z/4n6vl2/bgehQrxHZ0ipKxD6d/ejTT0P6ZGf2S94Mask0UFswoSbHsMO2OEEn+NvnrryaVA7gzf94tX7sazcwbgoYahG7nnykTVnuALcGK4FQdRhu55x16pNY1TYY3yWXwFHr1nDGiXkTmx8NIUSIgXa74Nhj1nL0MWtZo5MLXkRthvDeoxiyrGCyW/LY45vYunUrEcNkUSCSUQfYuaPD44/vYGy8RMjBOC69Z8O0fVt7heAoCqjrGQ5Viw8GscOoSlMNI8+aEFFxTWkT43uEE5qICWuJveND9GAtxjnwAULEYtCgeAPBGDAFUXrF6WKNEHrZ7im8LOFIVzRkDrNVbDZiA+LYlgWxWISgfu7ciD23pQtvWhcI2W3OPeP/50raGKBfx3yaiMTGHIpr2kiLwK4A39zU1Zcct7K0Dfd37/6wPvLYcfzamy6iZT1tKp551nGMOohlzYue8eT9cMFrBvz+MLBun37p7ke26uM7u0xqzgOPbeHxLdvZHtZw2533UckwN9/xILXJqDUgqk2rVGk1ttYoTY2gaJqSKJJBhHZW4r2i4gkoqoI4izcG9b7ZiMS62U1Zgxgh0phUogyUG5SQcKSoHigQQuDimx/Vi56++Iq3Tx4V5HGLDTSmS+w+99RYDLnNqX3s+Ze5+GPG9xY+p5gIwWA0A6uo1jiUSWO5ctM4LzmutTLNU0carrxtg5ZxmJtuu4dHdzju3/AEdz62nQ07unSDQJZhrSFUkxDzpnER2pijRCD46VlSUFErRG0iaqZt+dI4bkWTeSrhyDZNzRbKM//2tp95Br//Y08faG/12zftVClzyqzGRrMos9h8pNKvnWs/TUWXpGVAsJ68blOKgKtwVYm4YSZR8ljy309fw9OPWTl7zyM2VvBF550kAK+84Hv2+uyex3bp7Q9t56b7H+e+h5/gm/fD1u3b6FYlWZ7jo0eDhzwDjZSshlhh8VgqBMFjCNGCzSCFCiQk7CHEZwry79z5+MDnWeuU7WXEIAM5sud0jO8h6/cmtX4Z3zLPuaf7dSzCPIU1TcuIqOTqsEGJxlBKm29t7fD0Y9orxzyVpvHeeOoJq+SpJ6zih5//5D3+ftWdD+k1dz7BnY9V3HDPVu5+eDsTtQHXadRtJ0R1TfkMjaCKNYakZyQcyZi905+9+7/pgW0Dn/OEIcuOScVh97D+zJnMtygT0/zXbNEFj1XTpzNgv+cTmwg8IWBQBIdoxGkAKbhrogRWDmkcseap/YUbbn1Av3DnJN+9dwPX3rORjVsqcG0oWlB3wJeISxnzCQkzBetMM5W1Fv/pnx3I/HLxpq7+14YA0RIGiE6c0zy1D6SyJxYurR77hGRbhRqDZgGJivNtkJogJaItSlPzqnVtfuL0fEWYqJKmsUQ86/zT5FnnA5wLwC0Pjes3v/sgF9/8CNfc22HTWHrECUc4UcSmbP58O/oQHNfevlGfe+7iy8Ec3bJgFRP27rE+W+DHGbWj5vJvzBV9NfN70odUtI+m0g8ZjjoGwAPS822AiMf5mkKUG7YFfuL0pGkcEbjvwUf0g9eM8a2b13PN3U/QqYAsw0lAYyQEbRr1RG3KVIhD6NXrEe11eyuQ3KDUUHqwOc4KMXQRUxMZTg864dAlFV/wrl87n9/43vMG2kn/zrU7VF0bq34JWsKepDL38bGP1mQX/N1+mky/yC8Vx4Sp+Nknr+blRx98h3jaBi8zzjj1ZPnzU4E3ngPAV2/YoB+6fD1fvWED23eVZEVGHXLQGlxT6VWl6ZinXshaq5Cwk6pSMBm2PUyINb72YEZAc0TG04NOOGRhDNz76I6BjxspLONRpxOK59vl96MQy2xNY1YZkHmirxbjZJ/ysSzG3zEveRihHZTbt2zn5UevPejvK5HGAcbrnnWSvO5ZJwFwye1j+pWr7uAz33mILVs71MFRaVM63OQZkS51vbPJQG8VUCmm9Bj1RAJCjdeYEkESDmk4o1y/fvPAxx3bypiYbDSBhWpP0bcHeL/jFxb2LNDEaer7C5mrYp/fUKAF3L9jAjj4pJHMUysEN6zfpZ+49A4+feWdPLA9Ng2L6hJMxMQ2ShelAiJGiiYbnl7vCpvisxIOYfhIa9gw+bGfH2j78+mHSv3O1oARXdAkpf3qePTNKF+YTPo3YTKLIqT5PvMIbVEq3+GFx6/ljU92B3WbmEhjBeK/bnlcP/i1G7n0tq08vstArMFoU3sr2J6CGCGMQyFIajeacCgjeFRy9PODRVBdtbHSz22ssPTp3DeAUF9sdNVihP3034xdFFHMe/3ahBaLCxSq/NUzRw8qaawI89Qd2yoNISC9NqFnryuOaIPLay54krzmgu8F4JMX36Zv/fx67n9kIxNlr6YWnaYOFgWx1FTaKuGQhrGWIBmX3fyEvvzpxy567T+pEKxGmNEkaS6/QL9IJp1RsWHqqzOPN33awe7hDZkzeXBhS0CYpwzJ9O8bwAsRYayCm3bW+ozV2UGTkStC0/jXe4LG2JRLFgFne7VkQuyZXwzqQ1MbXxrHktGIM5YsyxgpxrFiaFnLkHMMO8eQMZy55vCx9t+5Kej/+9R3+MQVt7OltoBArMmsw2tijYRDFxpqoMXbfuo8fv8nnjHQmv2DG8cUWfxOfi5EWXinvy8FEfuF5A6ibagRCAZUCLbkaWtzfvX09kGTbStC01AqjJVmxxyVoDX0eicYAxUFkjkQbTpxaSBGwROpQ8WO8dGmJDoRUIQKQ+SyjVEBKnXkFkYyYZWDVVlkTWG44BDSaJ52nJV/+h8v5J/+xwt5z8UP6T9+/ArufEKp4xAiZZI8CYcubIaNyvrHdgyupdC/Cm0cQGzPp5Us1O1vMdrNIGS29/kM6iLteojJYpwHtgGnH7wM8YOuaXxwfVc7lfadGktialn4OyKBocyyJrcc1TIcVQhPW2VXPKF85dYx/aePfI2v3akoJYpHRNFQN02JSiAbBe2ClEj0GCkI0YE1ZEZgsoMvkqaScPAgsSLatTznnOO59i9fNNC6e/cdO/TBTrZbSEuvYVdoTEJNG+KF8yzok2fRT770K4jYz6fSz1EfTIbxHnER8RALx6uPMbz6+IOTIX7QSeN993a06ltS3wx83j3uyyysXkY1NA3ndLr5klFFTBMDfuKoYU0r53nrshVJJDfd/4T+y5fv5gNXrKfy0jSN8QoGxJZozLAxwyjUUmKdYr1QhRJGBSlbSXIlHDQY8QTfYthNMv6ZXxlojX1s/aTetmt3LoTobnNSjD0C6Se0+5DCUjet+xpdtfv6HEE6uFhgQpfKZZw1Ivy3pw4deaRx85auXr81Evs9NF2eZzN17yb22pRiUAxBe4qvWBAh9xOoEaIVRCJDVjmmZThhuMW5a82KIZIHtkf9iw9ew39+4w5ClmFjhUSDc4ZODKAW5x1CpM4iSMTGSJSUrpNw8KChxBTrkM5m/Jf++0Dr6Wsba/3249UefofZ5p3Z4mNgmacLy6d+5q9+v9ePVKxmlK5LXrexOk5XcnKjvO0gRVEdVNL4/EMT+njHYJfYhzf2fTkLv/RgFZmacNrrPay7SyZPmGGMKhbffE8DKhCNBSOMxJpjhjJOXeU4d7UcdBK5+4lK3/aRq/nkZXdSeiGYDFdkEAMeAQRKjy3ahKCISaXbEw4iYkBNm0wn+Mpbf5BXnXvcotfQtdu8fvmh7h5EEfuUS99LqPc1X/fpx7GfNZW9hTRULmJrQ2E8k6aFD/CTJ1q+50kHPmfjoJLGe++bUA150z1vGTWJfppM7O0kmsmjqFEgNjsIiQx7CAK1KNE2QtcFg6kN4hU/pAQVgoKNkVUucPKQ5eUnDR1UArl1C/qWd3yOq24fx2skSFPjyuVK9CVRMtA2Iok0Eg4enBHqrkds4J3/7SX8xvedveh188AE+p/3jvd6dFgijVlqakH39yf0b9LUT3voZwiJS7WUSA1xGJjEGENJi1yFE4dK/sfZwwdcxhw0u8St22utoyUX6Wt+6psQM8/hU5Nhqh7+fOfJ1YMaVJpEHI1C6D2aqIq3JYrFqEXqqbIAiskCJo+Ib+FEieJRq4yr5a5xw113VioqHNse46Q1a3nu2gOrhZx/NHLpW3+Ii+/YrH/z4Wu49PbtaCzRKKDDCIrYXWhMPo2Eg4c6RExeIMbz8JbBNjCnDSPEphCHml7EJTJtMjK6u5HmnPJkryK5ey/RgM4rW+Y6ZLacMaJLIpVAIIuCuoDHIcGTW8uDE/7gkPzBmij3j5cYCiQq2qfevMjCmoL0IZipdzJf2Fzp6t7ntpeII4gawJABZT2KGMWZgNgmX8QrVGoJ5IxgEI2Y2PhAbK83OMagCjsmV7G1U3LLJq8jBZy4quDFRx04p/pF5xwjF731+/nXb9ylf/W+G3h0p+LMGCHUqB4Fpk6SK+HgQQ1iINSB2+9/HLhgoMNzZ6l8DRHUNC2Xm7UucxqQ9pYDC5uybJ90rzA7mmr2+fqsdNNXE7GI8UBGEEOmFV4zVC1XPxH0+cce2EjPg0Yam8qSjBbEuF8zmvtVlJzrs0xHex/G6c5iU5NNJZJlY0gUbBSk1484F0FFiVRMZjUGi1ODaEMUKqAEoolEEeoYCFjK2rB9c809Gzu6tuU4eniIlxx/YJIQf/WVZ8uvvvJsfv+Td+nffuQ7qF0HcRxJKeUJBxEiQqhrJC/YvG1s4ONHR4eZmOhQ+RqRJnMabaIH0d0ifP7S47Nlwp7fF/qVTl/u52NRU4NmiIEho4wZZRTHgzsnef6xowf2eg6GT+PGMfS7G7pY12s7so+XMF90xBz6X58vmH0yfy12EsW+l5dzbNtw1lrhvNUHhkCuu3+n/ukHruQrN22BvAX1JM5GiHUTNaaNjbgKBpzsXk0h7iZm9WRFgfc+Sb6EfUcwqOtSxBZlDOgXfmGgNfDlB8f0hp1NUU+nNWoK1CuSFUgYJ0qxpPU7iEtiX+Rp6HOIysLnfuszRg6opmEOxhzZOlHjIniaPIil7FBEpInNXmhMfW8fx6KvY55heg96vqE6wdaqw1Ubd/L+O7foxY9NLjuTP+cpq+XLf/598vZffhbHme2MZoqv23h3DD5YQvRUzkIBhNiQRVSm0m+ttWAMdZmy0ROWaJ2SZj/vNfYKLQ2GoSzrBcvvXq9idL9pBP3ky8KmL/ZJfsz1+4bGXzP79+/c0TmgO/+DQhrbxidxPTOcAWzPDzDfkGX3H8c+Y2mk0W/k1hLVUssok+5o1o/l/Met2/VL92/XW8ZZ1gnx268/Tx7/5K/Ka59/MsgEhBJ1qzD5KppmBTliDGaqHpgIGgIhhGniSEhYOmwT9STK5bc8NtCcX9XOMESMMZheRVkrBogItvlsgSGifcb+3UQu9P2F/jbf+e/aemBbIxwUn0ZdebLcYGVKJmt/ZlugqFhcIqn025H004aWKtWFHBMUEz3O5VjrqN0Qj3ZqNj02yY209czV8MLjlo89P/F7F8kHn32f/t57vsnmXSUeixlaQxa6lDE2RdNUMdaiRlAficH2/CGpn0fCklZgE3himhyo9ZsnedkgpNEyuBiojTDVcmkqYmlqo7Pg+h7QvLyXJtBHnsw2b+19vO51soU6Cc7+fP34gY26PeCkcdXWqBk5tSgugDcW0ydPo1/3rH7RDf0zMhdfOnnf1Ns+0RcxYJ0gooQwjq+U3Obkboi6G+kWXW7ZZrlvDD19BF78pOWpOfOzLztDnvfUJ+mvvfubXHrHZmJ3BxCxto2IxXtPjM2OTo2gMYIIkjoHJixt1wRRECtoqHlo62DRfGcOiVi8+pj3Ihab/CojNISh/TWDhXUgXVCezM4DG5RU5n4kM7La5wjkmfkbO8KBDWQ54LaFx7seYwsqrbG6OJthf9VQlziWaH5aok8lz6Guu3hf45zDFI7aRirrCYWnMJ7MlkxWNXfsED56v+rljy5PBMNTTxiRb/zp98of/eiFGDHUZoQYAjF6nDMQlRgCxphmMttknkpYKmk0NaNiL/R+/abuwKcoZHdr16Z1wp6WgoXGwJc7a/02+vbuYZQ9xmLkyYK/hU4P0ystN3Mgkcu2hgPm1zjgmsZEpVjjcFSozP1Q55hTczPwPOrfoDuJftcQWdr5+2kqMSrtrInw0KAQI2oFbz3Bga0dzggua6KWJuuSB8rA5gnR0Zbjtafsf83jL974dHnFOWfoG972ecYqRznRgaLAOYePYerGIHV+TNgPmobFEDUgAvc/tn3gU7ScZVdokvqawg5NlN9ibMd9zc/9alfNyjObLQ5sP01DzZwmqKnvqlk4j6SQmvu2W15+1IHROA74NlE9VEbJjUP3oTfvXIJ6qTuJpUZPLRUxFgRyam0ShTJryQO0OpFVXYuJSgxCiKYnsCPRCNvV8FAXPnr7Fr1yU73fpffLzx+RzR/5aTn77LMphttojKgGUCW1CU7Yv4JIoFeVduOWHQMfnzmDMWZ6zTbzMy7LPF2qTNjLfLVQ5NQCjvBpUhLPxvFwAN/VAcR/PRY0zwradRe0iXgQt+8hr/N9PpWgNzWWSgZW+o2Fw2r73Z81NUKFFZo+4IBaA7kjWCUaabLmxTfBSj291IriCIzlR3PrVs/H7tmpN27b/2rqzX/5PPm1152BGksIBTiDcwHUYCOoDYh2cVqiWiMKWYhoCGgr9S9P6K9qVOrBtIjB8uDWyYHPsG5IQTxDdReljWQZBk8hdklrf7rc+qyx1E1pE+nVDCXsMZC4x7Cic0Z9WVGsKF5HiFngtp3lAdnJHVDzVLfbxWtOZk2vdas0VWX7Jbcs4qUs2BNY55uq0jM/6aLNY3Orr7qXStov+mGgJdXn99v1JJI7JmOLGx4b58EdVs85doSzR/ZfouA7fulF8synnKC/+Z5L2bHTUBfDWN2KMSOE4NCYEcT2HJFNMygXIn6iCzZlnCcsKJmn17C1lrAPyaItJ4iY3trX6fOqhjnLEO2R9b1gxYj+63d/ax6L+c6e9bMiNsLmyQpWF4eXplHVHg296BtVooAQ++4AFmN+Wkib6PdSlproN59audjkviWvuSJS1hVohmuvZqsvuOahrVz+0I79uvN408tOl6+/9cd50tohbHcHIR+l1ggCBgthqgNapOwJgJwUWpXQByFg3O6NhRjLFd99cKC5O5JnjSwwgpnhhVzM2u0nCxazqVts8t9S5M98x1iJiBoe2nVgqlUfMNJYP64qxpHZJsbACqAG0ys/PjXE6PSYrabNN+bKkpypRvabPINmfPZLuplrV7PQ6Gf+6jeZJtVh8oLMRoyvMDEQ8yEeqQs+fO/EfjVZPefU1bLxP39CznnKMVB7yIdASzAlIuOIjIOrQSyVOHxK/kvoZ/4tCryvwDfBMapKKUMDnWNV1sgABUQiIpYMM2/C3B4+UZHpMVXmcI/RJ/mvX3JwY02ZfwyikcwpX/A4LJvLw8wRvnkyotYhRgnaaBvNBcR5Q9Wm/r4YBl5wN7GAPXIQm+S8O4Ql7jT6Lqo+GfOjMUeCpxsnqaWDdR4jllILxuIQd2zq8MWHJ/W+zv7LLr/lXT8izz5jFVJtAy0IJic6MFo1japMjoojkiroJvRRNLp1Y99vF8RQA4br73hooHOcu7opMRElIjStDkRBjfSVL32tBv3GUssQ6VJHs6xL8sONNDwBgVhN9+7FmD2Ydr4HPftl7xUHPYemMoj2stjoq8VOhL12MkuOE184z8SESaxRsizD2gwTBXxAtMbYQJ1lbCod1z46yQ1PVPuNOK57x4/KL7/8qc091KAMESkwtTQjRrKULZ7QBy7PiXVN9HWjekelK+3BhZkaMIIQGtnA3h345tQ05pApM/Mu+pqOlkwqcaCxt4bTNIkzJuOKJ/yyO8MPmCO8Ew2RSIagIhADYiwGnVOsDBTdNFO8Tk+S3cfHOc671FC8PdRenev3Z+SU9GXuhe+1n6M+FqbJ7ygVZx1WDNE2JRksig8RGx1dk3H7rpLtZVdfdfKq/eJsePdbXiFluES/ev1GNo9FNGuBrZAYiBqaboEJCQvAew+Z622bm2KYdz2wEThv4HNF6ZmTokViQDXO2S6hn4yZ+f1+eVz9gl2lvzBZ9P3NFfAjNGRpvPLwRFx2sX7ANA2PwYjDWmkiJELoOXHMfjXvDOK83v0/C/tM+l7fPFrNtGaz5Je0sKYSosVJTu5yVKBjIt4oDsNILYxKRuYUY2tqCh7sDvGJ+8b0ri1j+0Wif+B3XiWve/bxGBshBoJ4gmu6GEbrklRM6CcJp6OexFpCXbNj1+BZ4VPVF+gF1+yuehsX9D/M9fc9fRaDy5z9WUV7od+aJi4B4wNbDkDU7QFZ0dfvRFdLl24Q1OSAYDMLwRPV9aVy6asZmD6aiCyK4ef7jdjn+ha6tD07iO3rmtK+G5XQ06cEyHVqIimTbvdeyKjB9RKeOsFy046Me2r0B/dDE6gP/M6rRLNv6Ie+cS9GVxFKj2Q1howgBYQJsA4qmnLrMgnBQViN2MkkOI9kZBZTTxIYQf0k5C3u7wzegvi40Q6bxoapXYnoGN5kPfP1wtpF//XVp6CpLrFfzxItHiY4jNRYUcbD8vs1DoimMd5VfNDph6O9+GkkYswiHE3sGeEwO8phf2G5sr+X21G+UOSYYf4QZO89k5OeLzywf/p3fPA3XilvfsWFhFjhhhSVFqGqgRLEI7EiywskGKgE4xxIlYTmkY4YscY1VZSNQYzh4Q2PDHwa28vHMDOS5/allPm+yI3l7MfT9/wiWAU1FiuBu7Yvbx2qA0QaJZ7mplSYVgeNNuacxT7U+R/00kLi+v1WP0f8Ul/6/mgitRhSmepgKNKYCKeIY2vt+NT6if0y0f7tN58vb37lGXitQDtk2TCEskno9J5YlUiwiLSJQcGkJk7JPBUJKETFiEODx3cH30wMWYOYPdezkcE3bfu7pNBym6+iFfKoCA4nkccml3dNHRDzVBkB63qZmY2Qp1cbRtQs+mXM58SWRR4370PXpWkdfevx9/Fr9P2p2L+g2kJ90Ger4lNlDKZqf1knjPkhPnJXqRcel3HuWrMklevdb3mRBBP0vZfdSgxrQFrUBExumtIjXlEyfF0jbUuKyk2IM6sChho3NDzwOYadaSoRAKghotio0NNA5lsXhoWDY3Qe89big2liH/mx8N69n6NdJWIDRLGIRh7tBGBo2d7VASENxUxngTfdtKaScBqh1U/dicucVDzbZjloZNVcZQoGI5Wlm6f2JNF5ip71EqfirOuxASSU1M5xw5Yuk97pc45ZWuXc//iNl8r4ZNBPXPkQkq9G60mi1GBrgkCmDusKgo97NqFJOBIpAxGLIvgYMEbwdc23bt+kLz73uEXPw1FrMdRE7a0pbSpO6LRZdp41Lrsl1dykYubZrMpepDLfRm1p8qlfiH/j+LeqBFV2LLMzfNlJ47ZtdfNMVdGp/AzRnnPaEjUiA0YYyV7drZZmZZMFXtLs2jRzT4qlvfS+pGIWPwnmmqDa28lMlzVRiDPq/eRBCYVBQ0UU4fadFZN11Jee0FoScXz8f71CHn78E/qduzcjrVWoONAONlNCHRDNwAdwKWv8SIY1EHxAsgytm6ZHMQodHUw8DVtBTMBEaZqEiWCjmbOdwlwh+f3C8fdNy1h664T+y1+JWIwoGg0ly1vxdtlX666qxKIYbXwYzb+nzCS2N2TJ9VeWy2a4GDOVEV1wLGZSLTT6JjfOca69XnKM06apPQjEGHwmWD/lVKtoac5D45b/2rD0hvVXv+MNcv6JhqyzpelgGA0BRQkEDZCnKrhHOkSb7XozHx0am3mxozOY8GvbJu/LiDDVjGmmFWGxyX2zmyrN5QgRw+4xo0nSnGOJPsv+O3/FG9dUytYMEeGencunvi87aUx6jzVmOurJmikLYvNS4yIuYaEXavZH9NEy981Y7oJm/cjOWjtdtmWuDNkKwUuN855Mh6htjiUyWUa++MjSdd2//5M3c8Iqg61qkBx8xDrX5LBoip460hGCR3pZ4VMmbOrIPfc/ONB5TlvTWIpFd2sDsoBGsFy5E/u7H89iNpW+p3GIOESEneXyVWJYdtKY8EPTgmrKr9H0kAA0kJm45Ac6m0j6jfl27PN9f7FlAhbFenOM/hFefdT7ea7bicGJQTVgTGMFmqrB71BMDFiNFISmB7jLiRJxWmIsdHGMV/Dx9UsL4bvoJOQ//uSnMa6EsAusNHZgr6B5kppHvHlqCNXG32V8RcyPAg3UbvBSIoEClRZOBYdHTd43ObZfv4x9lSe7yxwtvGucq4XrzNFv0xloMyIdujaAq8n8au6vDmHSODBMPXjtlkGE8mKvb95dPysjD2UQk9se8DVfuHfrkojjFeeskb/5xech2TCijlB3kSziNNWmOtIRY2wcgyIYZ4l1Dc4xvg+ho0YjQtyzEkO/StnLLL/6kc7spnGzx6LJVxwSG9NvWftle1/LThqLuenlriK5Ly96qarqUo9ZrjyUuc7fb6cUpWYiW8UXHti1JOL4rR88V970klNRMsQ0vqyoPknNIxxNsImD0Jur0UOMbNi0bV9OtudG7hAwL/U93iw8pta47QUZESOdcvni2Jc1eureraXOdkbN/Z6Xv/PVwgQW+5h/2IfrnxGdIYPdx/7OQ1EWHxUy5/Euw6ilU+dc+vAOfcUpa/b5hX3gLS+Sux/drtfeA7HqQkuQQMKRDKNgLISm4anJLLGueGL7xOCnigFsrwJ0b9Ojpt/6iEvaWes+yYcZQlj65Wn036upNtGIkYixkSos36JaVk1jl4/7J41+kSae+cZ8v7Nc5rU518Qio5+WyQDA7CJsi9NQmqFRcL6ksIatfpiLH1pa2ZF/+pUXMeoC0hpudpgJRzREBLQpXhrFYIyAsUQ7vA8CrRexJIIzhpVQZHmpgTD95MdUhGbTCVRwRgni2J/9cw4YaYzXLLJJ0eLqx89vnlmasN9fQnnuCpmR/VEvf0mTloX9KP1Iu1AH4lHj8dGysx7iW0sIx33WGWvlT3/uOWgsoUzRU0e8eUqA0Ntxq+LrDsbYfWoq1HIOo1PrSBYl4JZsPuq3ae1XRZs+YxFrVA2Y2GTEiwlEY9neXR7GXFbS6EZld7z0QQxJ69POcbYTevZYzms7EHko++NZR2Pp1BWFVVQim6vINZv2XeP47R94mvzQ806hSD6NRBoaAIuG2GvCFCEqj23aMThpFBlilMjuDeW+RE/tzzyK5e7sN/UbRqcqbwQQw87u8tSgWlbSqKIsWx7CQjv9wSKnlh49tVSBvNx5KHuZAgaEtxExbfJsCK+TkJVUwbJ5Aq7bXu/zFX72/3u5DK8aTVLzSEeMiLVoCOAyslaT8PnYxicGJ43M9YioV5wzLqYz3vJuWpd/oxf3/K5Gohgmq+XxaywraeRiMabq+xCW2iN3qS+5L9Gwb2Gz+0tTWWqceL98lL7qtRrqOElAyRmlKB2F1FQoD20zrB/bd9vpp379fNRnZHhULLauUNNGTYaGiI0pj+Owh8mg7FBnDhtL6toRe22YB0XmO9RYWlgkZATXxfbMVPONfubvvprKPmwaBwmp7WtJiREJBZhxVAO1KxgKys6OPfRIQ2VuR+/+zrhccmP2FR5yt9w+n37XNxxLCgNWPJFxoniCtKhpEQTu27zvTZRe/vwL5EdesI7KG1yM+HyoSQCsSxCDlWS+OvxJw+BcoyGEEJoyHdZijNuntTalPUyVyZl7o7j/saguofugnfTzidgZvUNMz3KhqoRlyoFa5jwNmVeV6Bd7vEccch+pf9CboCziKeyLhrI/NJX9cX+1RLLakVUZEUuZe0I+iTUlOYEtCJc8uu/lRj79v18nxw57jDQFDK2LYMAVBWXoJKF6uCP4hixQnHONqaqu8XFw84qzvXJFwj75C/dpfc3V+GyBDPPZ3xlk/S5ESmpmkAy7014OGdK4Z0fDc0b23YxyoEJSV2p014HSVKaKSc43lBZqlOgitWsjjFJUozgveOdxrs2O8TFu3bhtn2/23b/3Q1RVB9fKCXUGCr7sYFNBw8Nf0ZgqK6NCDDQFC0WxWTG4SdwaDLrgOppLCE4NKzI9Fmu+6icT+q7rAczvc1aiiE2zZ1UQbI+UIMryiPdlI42u903CzoK+hP2zE+6rrSyz+Wm5hf7ByJifCW+6ROcwWNrVBO2qSy4GYwtMDJzeuZkXPfx2TvnSGey6/u37RByvf/ax8opnrsOXFegQORbEEerUa+NwR/S+macuJ0518LOWqqz4zm0PDjQBMmexoghNRe1BfAbz/X0hf4hh+dvF9tNepkuh9BraTYXpRmNZP7H/F9CyZVaVXkENRiNhHsZb7AO2fRxiYZmb+JglVoCKLK1z31Iz5peKNjleHcGAihItDFXrOfOh/+SE+97DRHiUUQ9Sg9zwZ7DuZcrpzxz4ov/lt7+Xc3/uvWhQJM96KzLVpjoSNI1mq9zbLqvHioO8oBpwX+usTJcjp6e97NUvZ1aFBJn195mfQf8mS2aBBd0UaNW9zjnbDrCYX5ivn0fjjLdgbJNWECFIJKpl0u//9bNspFEH7amc+968ZBCmXviZL+03D3ZNPenPKks6vt/TCeqxKEeXj/GkTV9g9OH30x67GRcB78htzpiraeWOstxFcdOvsOr06we+z6euK+QPf/Yl+rcf+jad6Bp/VZKpR4Cqob1+8U3JfA1Q1zWooCbbh7WyO7mPWWWM9mwNMP8KmLlk+qUIRnRRa2+udbiozqV9zmum2j1P5WzQRHzVYqjCIUQaU55709Sc3ONm9zdx9OOEfj/XrzZU/85buqzHc5A1ladv/jKt9e9l9eavkQlU0TQT1IC3ltVaM1EqoagZiYZq4+1Mfuu3dOjF/zDwD//pj58r//D5b2un08L6cYLoATEhJhzMTRFY5wiqBF9jpIkGimp6Ae+DyII4LQ+mSGOhtbg/ZNFSOv4tam732bVOhe6KmmkT1dS5/TJsu5aNNLwxtHwgiNvbvDPrQcXYhw3N0oim34uxfYV4v3asC5OB9G0i3OfjPocHcpxUxDiJcRbICd5RRIcNsHlokiHNcT4QNKKu3VgB6prcOCozQVsjQsGYFNgIp07ewMmPvIv83g9O74QCELRnMpIpKisZE8ilhcYuk4WypurSveU/4LTXKie9duBZ+0+/+Ap+4e8voc5GQXqNeXqls6c2HdMLNQTE2iR5D2lFI0dkEsRhpteNgNSYMNi7VWvwUWhLThlAxWMXEnMifc1DukhTw7xiZmaV9n0gHduvuyhCFruIzYgKagqMr2lph0cmRnnuukOENKYF5owQsHlvuo+qoMiy7BAWIpn9uwNZXvOTNU0ZBtFRJDqiNjbNyVZACRzbGaK24wQx5OLwYQKVHJcZ8rgdJ6OUZKwqN3PWE59g3cP/juy8uWkHFofBLFxtVHJLLLs4bUHVpZSCsjtBeeX/x7qfeO3Az+2nX3GG/N1Hv6k3b7NQT2Jcvtf70NiUnEiEkTATebcid0KZCb72rAlKxyyveXcRC3xZ5Y4xhhin2xWiKNYYrGbTeSqHBGlMP4RF1L3u26db92bi+ZxW+7JT36eJMmtOLHQN/SLf+pq36OeTqIjRIMZiqXGxJtMaFUMVlQkTyNwQdVVRa8aI5BjdRSeveSI7inN23sa6+97F0Y98EFuXlCIYaTQwpdO3TX3lA7mAqZR2AR2FtlP847fir/sjdc/5y4HfwNt+84d43f/5dLN7mtGkZypha+bfEhKm10KWYUNFjCCqlC729TT328mHJW4g+7eG2NvPsqdMMH2PF5GewSI2EqNnmgvLsLdePk3DCNJjP7NEn8KUpjL3A12M3Fgc2+6rdtHc3747+5daIaZlDN2mfQAeQTAYKVCKnjNxF6bOWC05Zd5lG4E1oebCJy5m9Ja3k5fXUXqIAYYAp8oum+OtYSjUfSeejZBlI/h6nGAs0i3JM+gyzNi1b2Pt8a9WTnrJQNL9teevk1eef4x+47vjvbAsaXZROpUt3BCHavJ5JOxGESJZbNFtCcYosbKL8PlpH+nRxzy9DPNvz0ZS/XyisVn1RnvfbYgjxki3UwHFfr22ZSMN6VW3bcoUH9yusrJIn8H+MoENXCZliV+oK8VgsdZhVJsNuFGidrFGcGYVXVvRBY4vt3P2+n/jqAf+keF6O+O1wSgUFqocNgs4D0O+QiKUi/HTBUHDJCaD8RhoW4c3nmAmWN2ByWv+nKGTLhn4Of7vH38hV9z2X8TeXIoxojESQ0CMSWSRsBcmo8GbMbIoGA+q7f7bsiXOo74pAQOav/aWO3HRMqcx9TeRVEGhXoZmTMtGGhHFTt+M9Nmp92PyuR/QHn6TBZl4aWzfPzpK+xCNWdqk6+cpdzkZEWcqvILHEMVBFKwBxXPG2G2suuuvWfXYJxkBxkLGdnGsKiyTWmFDQV6DMyXBKN3MYmKgFzW98PXnFtcJxAJMgNoJJTDioc6HkEe/yfjt/64j5/7yQKvzlecfIy952jq99LbHewvCIMagMzSMpGkk7LHSqjEyK4xnw9iqQ4FD+5BG301l/23TsmkZi5/fsTHjz7DFW5stSwbbsrdNE9ElO4Jnfryn/U8Gfgn9NvJzCf3+PgUdmGj2p+aiGgnqqYLSIYM8o62BVRMPs6raxinf+RmyagOUY0SBTgYto0gQqm6JySBqDVicKiZCkIBgyVSo+hQNlOgpJUPrQCGRkhr1ULiCLWGStSZj141/wci5vzy4tvHGZ/DNuy/BVxUaI8ZatGeqYuZ/ExKA03d8ErNLuXPd91PmR5OxhdqPLsrSsFRLxbybqqWE3C8iukvENtt01RkmfGkSOJaDmJePLGaGRw5eu2W+evfLXQJkf51zf15bv9pcKooxjtwMsUoyTvLbOXPTZzj7ul/g/K89i6K8k1iPITajMA7jDTWerqsJuaXwkJuA2ooag0ZDFsBooCP9o5MyD2XL9BrfwFBo02YV203JKiPU1BQ7NzBx0x8NzKSvuuB4Wbt2LS7P91QZReaMw084slHsWs9Trvkdvu+bL+MlG/4FH4eWvaBpv9Lpiz3/UjClkczWUvqmM6wU0rhnHG1hMdJZVNGsvi9lgWJ6Fu1bcM+w8Nir/aLRPUa/0sQGO+ew4rDi+hYky1TImhJjRKtEq9R4olWC8ThqbLTYaPEm4m2F+EhOhqsnMaaiNhmtepKnbfhnzvn6Mzn1ijcw8sTlTGYjxNBsOpS6Oa9ERBsHtomByoKfuk0iUSK1QBCw9O/+VVvIQ4kRxStUpoO3u8gCVCiqGRYL171tn+bTP7zxVIyvAEcMJZhVEA256YAZSpIyYbeRxgjeddDOPRx961v4oa8fyzlbPsRxYRPGWIJGjPWoqRBrKY1HsrK3AXHY2MFJl2ANUQpUi4G0hJl9MqaT7np7m9mjVzelrxy0wl7DoNPDik4XWJRehrkVxaFky7CnWhbSaKIhd/sb9ncTpUFJZ6k7+aXWu59NQrOHUYeJFvEW8SAaccZShJx2p02QRtDHwlAEDyFncrhggq2UI6tph4Jz7n07z7n8HI65/n/QHXuQyQJiAZHxg76Qg9QEX6OVolf/7cBv/KdedZ4cu24IlwUwLah3AYFunSNhPEnKhDk0fJ3eaT/lhl/h/G++gPM2fYh25omVZbhjkNhBzBASa5zmjNYWn61h0mW4OIHIGC3ZvnADucWUXJ9H7u3WRHTB0a8fx4GwwCw7aYTQq6mi7JceuAe7R+9yk9r2oUkm2hW0DM5YnGY4FDFdNJvAhWGsRHRyB7Fo4bTmuAk4vsw5/d53c+HXT2bdHb9LOfYQQTJGTYvhYHAebFwByW8WrMsoPFS3/f0+neLnXnMu3jeaY24jmXVAQYuQJGXCHGutsWbGGAhisGzn9Gt+nlf/1zlcsO0ThMLjY4t2qDEcBVqzoxijLeOsjTli1pCHFhNO+vfF6NeZtK/Ptr98Wkhu9esXckiQxlz1VZazsXr/8un9/Cr9zGMsafRDqza4ECB2iFqhHkJd0KWgU+RM5hajjhxLSyGPNUdt+ldO/eY5nHzzr7Jq+xYkFoRccKbGxS6T3jIhltg6+EK1jhCNIwtKPfEE5Q1/ObC28ZdvvEBMniHBU4WagDTJW6mkYcJeQni6ch+qSlbnjExMULfBxA2cfc3P8uJrXsFZE5fgtaIOEe8gC0OMBccO73EexHhGfNHXVN2v4VI/Ib6vlpjd55vSSGabvfY0f6140pi2x9Gr777A6GfeWTm7l+XRVFZVGa4ygMXYjDwztFxFSyfIdYIiVEzkymR7lOMe/RrnX/ICTrj+12B8A7UoPg84qUCUHRYmW5A5pVCHVitD05BQ4jMImTB+89/t02l+6AVnoMUQiCViyESJtpWkZMKcG1aRJm/BmUDXQR5zpFaqDIa23sAZV/0w33/d93FqvIvVsaZlKtaZglERopuko208E/MI6gHkxyJIZUmb5mU2/x8Q0mjkxNwsuy/RB8vfI1yXOJamqXRzRV1T2RMN+FhSR6UKOTGuwqlwxhNX86KLX8jJ3/5+dPwOANrOkHVh0kIehHXdNiNdCwE0eiwlbgWYbxyC00jpCjxKMbaTcM3/GXg6/94PnEf0NS6zED01vvHWJyTMSxyCSJfMg6kqFMEbR20hC12KLd/kGZeczzPu/mNGOl26LtKRDsIQo6akzlfNuWanupFapG/gTD9NZbHyb9BN7nLhgJinlrzT76Op9HM0r3RNRYPDxUgRaggRb1pktmBt7HL8+EOcd9OrOeZbLyZuvwqLY5WD6IAy0pYWw7Wh8v8/e+8dJ1lW1v+/n3POvRW6e3rihtmcd9nMsgtLTgsSFFAEFTErKoqgXxQRMecsKl8jRsxf/amAgGQXFlji5pzj5Onuqrr3nnOe3x+3qrtid9fM9EzPbJ3X6+5Od3XdunXvOc/nPOnzMTQkIERq0WDUILh1EbyxLcUDIQSMh2o6y8Jtfzz2ea4+e1bO2RoJXjE2QiJg/MRKTsYytiiw3xpibSNZpSRUM9ET1EBlK7lXbLBsvfW3ufrTW7jogXezOSZUJaPllXoRV4wuLAco/Y27B+OpjNIZH6Y73ptoP8pAQ/Tgq5NWNLpDyty6j4N1/1YTQz2YPpToItFmBCsEV8MamF34Kife/MOc8pEz2XjbJ6lolSQRMg34IsE2wdcsuyqeVlUpEo8mOdFB5pSWjRTij7iAFEA1gK9A6j11W6FZ7EP2zeNv+aexEf0Hv/Zy1FSx0aMhx2oxsYyT0WOU2zvNxbaeSlQ020vFU/LhqaUWKoTWTmQKfCzw1Rq+Aad+5Ye59PoXcuquj7PRRHKXrli9dKDFOUs/rFB9eRDVW0eNp2HbdNVWBHMYqpOsmGWPlTyRlW7ywYJKMAGxKZGEllXUWCqxIJpAwwr1UGC9pZA6U/lOnnTnL3DKR57Jprv/mqq3tKoR1VYpg2qU6AqiAykC1eARX1K2aABUEa8k7RzYeiieWqhArQVZKsQiw8YNGMnJrn/H2Od68ysuE6tKYSxJ7gkyyWkc9d6AaaEBDA6JCUqOmgjM4LQ11rnS6AhBycW1VUPbWiym7BmKAkog2ibGADlEV6VSNKkaJZMqG3Zfx/mf/houuOmHOLH5GJEEow28KTuvk+jbu3pDFE9IcmwIWO8JNsOnBSV/jyU3duRmud8jGXUoYfEYBipqRh/YoyQRPvZFrHF10jjoe0DezkrNe77BVLEH0RZpVByBjCpThWOaiE+FwtW44MF/4qr/eTbVL72T6OeoGfA+PQZ2f7YUfVIlcYDNKKLHt3YSH//M2LP6mqecCZKQ1uplU9BkHN3DlmxGi6zFSQJOIOTYccPLsYW1illBl7tnU6cFKuAUrCqFTVCF4+97D5d9/HQu3/GHmFjBqkVMlUZqKWxGUSuo5UqST2OpIbaKUEWylCQXqhqpUax5y8AxkQjvJvBbTaJ4rauTVqpeGLcDfNzqrordxJ60Djal6nKiNDBiCS4hZpaN++/n0k+/guM++1pae29hk8C0wL4AdWkd/TYhOjBdsrOSEQ1Icw+tW/9u7PO99rmnQx5o5B6cmxjdo31EC20mY1UtBbZiBAk87aJTxzIARWsHgmI1loSpurKJczYQTIr6GWqNSC0vUGcQA9Us5+RP/hDP+8LzOGHhBiyBegG5m0YbKaHSorCtUuTMLGBihpMCcQWtRJmzbsXw9MEW0hzuPrQ1556CUoRouWMlqFzppnTcuJVu6rigsvrqq+Wvb14NNlRIXE4eNhDzKpoEbGsHF9zzM1zwvkswD32YlqkgFdgvFVpqSQSaiR79RkEDYstnEdoFT9ZBGsHf+f6xT/cdz9wuNRPKkxSTnMbRDxqAdq3RCPiAOwCDly08jgLidTFEviJoGMiSnIX6HEXVg0AIkUwcuZvF1KD6+Gd5zscv4bK7fgI1+9jQbLHJFsSwhVBVitQRZAriNFYrQCQlUC+VkZc9DtbTOOyRobUKN3We/uHs2F6rG7miN7RCddcGLUhNQdG0JH6BalJl+z3v45JPXE7lhl/GmiYpGZpnJFKhJhXSaHFrpLx12DFDA9rOrSigEYSk5NHZew9673+NH6K66lw0eJjIvR79QyxiIBLKtWst2KRUBRsXf4q9JVhoKY6kuvI5Qu6QVrsQT5SQgnGQRk/F72N3RWgI7FPLKV/8LV72qZewvfG/7DIJEgtqC03SrMDFQCIRtQGPoiFQ99kq7Fs8qGOlnMjR42l0dSIeNAukWd3R4510/X6J2GuQ7Gs5Va5DFXPclwpJ0zJVS9nYuItzP/Vytn/xFfjGQ8xkkIUpTAVwoCGDRgNb5FjRA9OqXW/DKDG054RzGCNoBqoJiYE9t/7J2Kf8+mefhWl3407G0Q4a0k5Yx1JjOApEbVd2jDeclJ6CFUtUWVkVFDA2oSZV6iHBFBCz0h5H61hwFTYsONIkQWJCqwbNfV/kwmtfxDW3/AQVt4eqbAYzg7cQzH6MzoMk5LZOw628qTHtgqFRx7pbzmsZmjLtkNGR8DSWDUeNCG+tFOY60ET81pajqM9z4h0/zRkfuYr0/veBOqIIeVqhcE3SJtQKiElCayriawYvlroe/VYxCoulv75DwKYFhQn4FOS+j499zm9/1napmoCGSSL8qB/Bt/OD7Z6boICh4pLxDZqN7b13QljlLtv7Jk3bolHx5JXSGzbBYDVgTEa0gaQRSF2LzG4oE+aFYu/+XZ774YvZPn89034OlYScOhoc1ZBQU4sTd8js6YHSJB01nkb3F17xb9aoTf5AgWjcRP1KoFJv3cg5H3opm677RaxGqKdI9NQKJVBQDcJeK8xVEmIwUJR08IUPLLijv6S0FKMt74WPAVBSC9FGGgLVhTmyO/927Nl96inbSc0kPHX0j9iJY5ZGzhgSm+LcAVQORiV4WazWYxXTIxWLCZAUynQh1BHERFQNLk9xREgjGqAS9hOcJRqYDYGY7+asT1/JaXf/PFvy+0lSQRNLNJ5ITtD9a1/ow8HlTNYFaJy/SSQR8JJg1K+iye3gkHbFNnxdvoJqpVEpHE480RREcahxBBPwcYEEmM4Tmglk1QQ1e7DBoiZh1u/h7Nu+l+0fvJipvddCAkUskJATgdwAEilMoCpK6gsgQ0wgaKmBYeORr55KYgUXKwhJqSRWFrsg2LIyqh32S3AkKrjYBsz2jFZVgjqMsVREkKB4KTeULiYYC8XtHxn7ur7vGZvIXdqOW0c0eowpKVQ0RjSCipnY5KMhQmVS8EIwBoktNM+57MITxz6Pv++zVK2nZVoYG3GrqJPwElBTFmi0RMlMm8GAUpgsAAXg21PJxEAEMgHBExXOvvU3uerTr+Hc3TeBrdIygcIWODsDsUkwKUKCiXNEaYEmZW+W9UPzED0bz75w1TBQGAVAqkeRp7F0sXHs8NHhn7DLHz5pEk2Co4Z4TygWEFPBVWYppGDvlFLPElw2h9Pj8JXI1j2fZ9uHriL90t8f9Qu6kWbkSQZSYKKWcd8AKoHoPBIEHyxNFZrGkiWCd6Ai2OCoJIo1voxRq2IMJECiIL5g/7aTyWc3j31dz7jqMgiNsg8kltM5titxXJKUSDbJeRwFiCElxYyzECLWOkSEonXgWinGAHp4Hr8Jhnlbw+//PKddewUX3/vnVGxKjRTyFGdr1HwDiQvEdCNV3YAiNCuW6SJblV7GaqI3w1gw1sLOrlmRu6qClBdtRulmt3+/1hVCK964FV5uuRo2FCS2SVIz1PI60lDyJDBXs2xc2EVR2YzRGrbV5Mwv/SSzt/4ezZkEm8Sj3m7V8nKXpabcEaahgvGKNy289cSKQgwkMWAVTHufECNEDI3ogIAxiqQbYON5VE56LvWTX4yc9oIDntVXnbFBjptWfXyhzUBgXbloCKXMZeHLOWYn3sb6Bg2LRo/YNtmmGKIEagdQbi7OLOa5FMEYIbC2ea86QlOaVBVaCqd+9Q1seuDvuePyf2J3bSPNIsUlDoNHixYBIcWSFEpOvVTcG9NL6Jd17f+9jLK56xk0Sg/DYrrqqDrNO4M3QFfptayNw7RSrigx8xiZJhRKpEASIZqI0SazjSrNZJYEw9Ydn2frZ17OpoVdNCtCuqfAT8PRrhPkTdtNjkDMCZqTG1BrUJvi8pyOAJnHEDUiYhBTOtvTJzyT5PgrkNO/Bk5+/iHd+jzj/O382+cfLXerqm0+bEf0AURwViYyTet9qGKMIaqAgRAUQTl1+5bxTxXL8I0tyynbx9qCxnwIbIgJTVdgPRQBNj/2CS743yu492l/wL2zX8e0L6nWCzLq1YIkzODnQaeSdrnWUAes3AytYAc7mhravT1dwwKaNW2nHYWGY5v8FTyFtfZU1E9jNOCcwdsqPipFkZG4OiGx1P1eTr/pl9lww2/RNJHHDMwaYMbiiog/yutCxW6CokHwGdFBqJWFymkRqeY586aG1QKDR5IqZuOZ2JOeQ+X0l2BPfdmaxiGfc8nJ/PvnH8baFJ/n5VxxDkLpYMToYZIsX+egISABxGBUiGIREzlx83j677rjBvUxkALGmjIprp61pnrWZJpc5zFNsPVtFLqLeY344jHO+OQr2HTuD3Hn6T+LumkqSYVGrFJpNXEzG8pkySqvr98bGSZ2t/qN9joEjbLprddtWqsvstaeSgIUMSfVKhJyCtdE0llmrRD3f4btH3kj1eZNECNOpqgkC/gAEmt4k+M0P6rXdFLsKdtFkhTUEBcyoipqYCERtFojOf45VE9/JZz0Qth01mFLWD3z4lOR8L9lYt5Jmdvo0qcPMSIT0FjfmxKxRG2BJEQfEGOJGqiY8br9pbUbcQJeCUUE261st4brI8wzLzCzYYZiYQeJTYjVSPQ5aYDNd/wJF+y4nbuu+iN2urNImnOE6Q3E1jzWpIuoMa597A9Bdf/c/7ujAjSMdIcF4lCHYekmHVx46WA9lbgC1KeZR6YSxGSYVk7NbsS3Wszc89dMf+HN1MlphYBUQPMFanGGLMxRY55qdLSO8v68Qtq9FqZAomKqILPnkp7yUtwJz8Wc84oj9g2vOGNGtm2e0cd27cUkVdS0uX4EIop1jkknx9EAHOWm0qBlmEpgy4baeCfJ92NsWZYXQkScKUsQ1zgU4U2daWkQwxyFLQlEZL5gOoU96QyJb7J554eY+ciLuPuK3+ThE1+ENHajlc3kfg9GpkYaeO2EXJfZAJtlEul2DRgT1tjTKOOJInZZd+rI968tb1YW6gWVYMl8BWyVk/bdT+36H2L60f/EecNCEnGVCq0QIA3YfI4Zk9AySobnaKePUlcmJ2X2Qqqnvpz0zFfD9ievGyi88ILz2fvZz5FHD862u7OE6AuMM5MKqvU+vwLYRAgacc6RB7DOccrJ45XcNuZ24n0kMYJYJRpDCGsv0iW2hW+VU89V6uRhgQ0p5DlMZRkYTzRVbH43p13/rdTPeRv3nv7jzBWQ2lpPfGqYjdQh4Lpc2KozrLVHF2i4kBOSOlErmBGeRrcjcDChq4MNe6XBklnIjLJJC4yPNMWQY0hsQoU5WlKj5jzH3/9+Nn7+HST7b0TrkMeIU8BnZcIqABYaFKBlaGvNFx0GpyVvv2iBl3aYGDCSQvAUrkZmCozmpF00+1ESQlCsLdl9o1dcAl6h0Cqzm89n7xX/h60nXAwbL1mXPtPXnNbgo9c7iFMQoCJz5LGCmkDUKsKE1HA9D+MahHyGpNqgyCIkEArHBtsANq3e5uT3kEdQW3orViOFlpXXa3r9PqKuTE9QNEiBVuyEQPKyvym2sBWQ0OSUr7yTDfu/yg2X/Dq5PRETIkUOeS0F5klzR6qW4NrM/8KyIGGC4G2ZHIlGSKIiWqOhyqm2BdQO6fddU0/DtPVPVsXkyOgkz1rTL+XJfoypMZsX7C0s9SmHxohVC7FJ9JvY7AJbv/xOqjf8Ms0UGjPTVBrzkKyKHX1tQcMoOQWGMozUmVPGg8acUAXJFpgOIM6S20CIYILDa8FUBfKmwQHBQTFzOtMXfCf2gm+B6bNl6zo3Oldccj780y1AxDhLkUfUKCKubPybyIiv89CUAVuyiWAUgyGqMjs7nrGTsIC1ZQ+RMRCiLPZrHMlhFWICC7mjFjxZOk31rn/laftu4itP/f/YXTuHUIV6LIi5QUxCXlOybA/V9Dgols+JGmMwxiAqqIBqLAWoiLg1mPtrl9Moyewpq6VX99R66427Z8PB5TxW9ESiR2NC5lK2mIxm7vHUSDXQSGqcnN3H7KfeSPrY+/BThloesY15QpoQw5HfxVYLXeR4MyKoJO3SU09IApUF0OoUUTw0Mqas4F2VhrZIK4Y8j/jjzkDPfC3T53wXbDvrqDKzz79oq1hjNMSCiipNLWPZBkuIYe23mpNxUCN4AechJBibEzXgTMLl2zeP9eDy/Q8iKmXJbqLEYLAuEI8waDRQqlqhRkYSIdV5fAq6914u+tQreejqn+GR2WtohFnq1Rotr4RmRm3mOLJ9+0nT5cHTt8WrjMqiZo0IODHU0kMf61i78JQpY8uG1VcvjGpIWaliVVbYSq7EdJnqJlpesCaSW0WpkTooQsEFj70f+4nvJ9cdRAdJEQlxCqOeap6RyxK9wJEamTWIiRgEFy1ELak0TJkQ9hUwYaHskE2gMJYQm4T6ZsxpL0Cf9F3MnvI1R7VlPWnTDPfvXiAWTbAVsBCLzuSZgMb6djUsEBBNCaEJ3pMm4y8qXXgIY0zZVGwCtDmojvSYosJCWGCDgaw2BVlOzRQs1Avq+27mxGvfgDvnh7n9nLeR5RGnFi9T2AymK4Zclw9lWJsQTcQGW7J2ixA1gnqSo6l6ylqLFLGduDlY9/XgSmpXvG9eoZZjgwGpAnPMJynn3fjLVL74KyTGUxOQBHInFNogCUo0UFiOOD23K9WDya0CHqOC4ICEiFDRFiFanEmJRIot5zN16Y8wdeF3HjPW9PKzTuD+vXcRY4DEQswwWKKZdIOv//CUgOagVXBCYuucfsKmsc+jcw+V1X0mwWtArCDxyKsLRLPAVAEtUyFPW5gkIJnDtjxzm4TZ/XtJbv0dan4nd138G8RmgFSZy5pMpaso5GhLvhIVbYvWgSAaOHdjTQ69vVmjccasla/sDHogVdLjtsCvWIu8wnnyusEuJEhqybL91I1y5vufj3v801Qd5GLBpIS8iXilVoFYS2lEReKRD0+luZI7CFqWmzoiRgoUCGV4k3ymRjzrlcxc+GPItsuOua33FWdu5P/7bCAmKWVbcUSMQaMpJXwnY10PDZTJQVVUhe3bZsY/yfxOQoiYNhW/MQGJpRdzJMe8WI4jgGSE3OAs+CnFFAl2oaBIDd7Ps+3Wd1N5/DZue/7fEgrFuRmymLMS129Uj2okwZTFMKa0unaNPOy17QhvewkyxsWvNdnW0JvQMrhaQSg8J2d3sOW/vxYTdxCisGAVFwNRmsS0irMWCQvkMadwwhRHniWkUQE1KQawwWMDqIVQ30BROw53xTvYfP63H9MxmktOm0YUoquABgwGVV/G4yZjfQMGWpbla1lyFIvI9m31sc+ThhahK/GteNBK2W1+BEfNBuYNVEKFpJqRRajNBxIbaCWGlkaqFdBo2bT7o1zw/mdw8ws/gjXTaGiRmeXlEcQYRA0GA4TSs4q6ZpbJre3tKifBONZqWKv8we4TVwIfo2CjYfbRvyH5zFsoiv1ohFCtYnwL6yAEg/gWCOQWUoQKSm7BHGHUEAETCwiKBghTW3CnvoDpJ30Pcto1T4iA/pknzODEUMRSAS4VISOAqcOk5HZ9DwMEQ9QCrMW5hOO2bRzrFHO7btBK4aECGkNZxt8JMhzhFWBbEJyl6TKCh1SqSJKRB5AQmaHGXKuJswFrDGnjHi78xEt45Mp3c8/G5+FWKLYJMWLQdvquBA4k4taIxHxNA74pftXqWcsZ/IOVQxQ8WYBgLTUKYrB4LJFAVEua7GXmS/+H6WvfiC32450lMVAtWuSmQhHK8jWRpTbAgOLjoQEMAyQYUp+S+hSjEB1oSnmdCk4Ep4Jp/71tN4oWrqRmJig6u430Ge9g6g07pfKSf5QnCmAAXHzGNknwEBtgLFmRY800aDYxyut8iC6gdqa9yUzxNLl883imaSZ7iD21TVBAmpQaFxIrBMlIpAMeFnGdNoAEI2nZB7HGI9gy/iax3KWrtihECa6semxJTiVMkxjYZyLIFrbsvo0TP/NKnvTIfyA2IcmagKMiiiWUzbZWCHaOCgrWUtgWFbFUQsRIjth0Tb7PmoKGqh4S4fSVjxW+ZBBmEktKiz1RqJkcSSAJ+5mKO9nywdey8da/YbrZIhWD4ogKNkIa1t7oJCGhILKQ5MQKOKljmw7bhJRALSY0VWklWnoVQOFSck2oBUe+5SRqL/hTZr7jcUmf+gtP2FKhc04/GWMtiGDTBI2l6z4Z6z08ZcpEuJGyyQLD9m2z451k+9fI8T+4W+yFP8A+D6kRDBkVMTTcNE7rJC0DLcqeECnAF0xTP+Lfv3ABnzZKWwCQ7CMC9V372fjl13PObT+PmVGqMWcht9hY9mU0i8BU0aVDo4aSFMkiYknXiHJtjVluV0eadbAd3SudP7hI00B9wbFtyrEjZExlLSrBs/V9V1FtPLCYB0ArGB9KfXvX1pBY4/BTZgpSSoLHeZujJmdKDUms4H1Oq1pQyxwCLKgnUcEa0O3PoXblT1E79bmTmlJgU7U0OPgCjCF4jySRw6BqPBkHtYBTJGRoGQdGsDz3suMPaE5XXvRHUnnRH5F/7ld17su/RL1oYIp5cGXvWBoh9yW5pSQpc77BkaazrHiLEspNaoTMeZoVRw1Pbf9+5LZfp7qwk+uf8ltMxQjqMPkC024a731po7QdlUHRtj2su6MwPGUQ5BDIua4cforLHsZVSVsNZMpRLOxjqpJi5z7Gqf9wMjP7Hia3VRaqCc2KQclJNWJI8Tj8YSi8MdaURGcCFQ/TvgTczHmyimHapzSMp4GnnibMnPdSpl/9KTZ+w4eFCWAsjmc/5byyuclI6WEYWVOK6Mk4VK5GgtDRZPFU3cGb8fSqt8lx3zcn7invxNY20LIpC7HkLkxlmhylQYazU0ceM4OiEQoFJylJcOR45muO/W4LWxoN6o//IVfe+IMEMeRhDpPWkLibUM0Q0XbmuKwCUCnztNNrRHq3pp6Gs0KBcLCp7IP1VEIeqRtH4T1z07Oc+uVfZuZLP0VegTTWqPgmocNzR6kV7CXFqlIPa9+8p60KmliMFLgiIyTQcobEBzY14VGn1OvTTJ3+TdjLfhK2nXlMAcVCcYvOze9hd+PjPLr7c2ThYXK9j2ee+69smXrmqr9rWuxCo4Ao3gfEVEEmSfD1PwwWIcaIGGXrzKGreKs+9Wek+tSfIfvft2m8+T3Q2EmuGTWqGAut0OBIy8hn1YiEhBALoslJQ4rxUCSe3O5if4SZZoXk9j/j4r0PcMPT/hsJTbRaL3PeRin7eMtNb5AEo4aLNpo1sRNrChpnbqrJzbsK1SP8VHKX4ahhi4JTP/MjVG79MypSgsN+22RG2g16oQSO6CCSY6PFaoJf6+qbWgvnywcfXIKKI8lbhAhzU7Dx7O+kevkbYcvlRz1YzGW36N6Fr7Jz7jp27r+ehex+Mr8T0SrWWnxcwNk6edhZ0kuMMZ5+6Vnw97dD1YCPGDElRfYkOrXOPY2wFPQQ4eyTNx3yj6g881eFZ/4q8bqf1eZXf5tifo7UVKhZpXWEnVFVYUoFD+QRDDmpKYtdqkAjhbxoUQkpGx76IFd+5vnccuWHWNjv0JoniseogloMkWAMEtYupu7Ww5w52N6ruIInMkUFE/Zwyie+ER7+CKZSpVFAaoA8Zc4ZLBEXA2Ij1ihWIMRAoWbNI+I5SlqqlKLqIDShkpCe+zqmrnwrzDzpqASLueadOrewk13ND7J7/kvsmv8MreJxDFNY2QCSo9JEqCC2ifd1TFIQo8foBuZat3Pc7DNW/XlTtkknp6GxXSIxAYz1P6TN6KcetXDBGdvWztY87Wdl6mk/S/N/f0z3f/X3qecW7JGtmbfNEihcmpKLEn1BLhCiweV1kuo8OEPDehIvTD/8MS687hpueea/g6+yPyltqIqUpcYiJQnkUQsaZX/7mru3y42ZhQeY+eg3wq7P4xy0shbWpIRMcEnGdABnytK4XA2+MCQEnECsFGVBxxqOii+rIULuiS6nduG34y77Sdhy3lEFFnsbX9XH917P43uvY2/jy7TCPXjdjWGG0K41d3a2VGqLTTQWZbe27MewAcUjYsl9gzR1NLIHx/r8q84/TaxNNJhQEjeGDkXFxC6v7xEpuwqEiHLKCZvX/BNrz/wtqZ377br3lndjv/p/j+i3d1VhoQXBFhhRbOFQlOAChibqE6AgdZBXhUIh2fFxTr/21dzyjP9s02xbuhtS1rIxes1Bw2vESLKCp3CQSI1SaI7TOimQS8CLpeoz6v5+tv7npcQYCdaQx7Z2BDnesZjI8LBIJyzS1Uu5CsAQLF4DLgHx4EIpYZlrKHstgiUxggm+9GACRGsJNhACbIyRfTYlvex1zFzx40eNZ7G7eb3u2H0bDy/8CXv2PkYz242xOUrWnrgJMUyVlYBScshHFIm+/boFtWh0BAlYVxDzjVi3ixCUZn7f2NcUZAECGJcicZ4i1hDxE7u8joeRSNACkiq0HM849TB98HGXyMbj3g3nf7/uufbncA/9GwnQEoeKx7QJV01IEJeXuU012DCF+AYYxScGOUihpxAikpREvwDelv9wnrKqql2FqgGcKqoQxTG76zqe/v6TuOWaz7Ozug2Cx1emca0GG6pJG0iOQtBwchj8jBgRW8GzQEGCYKkkSu2x/6b6sW8haix1FWCRh0jkECoGFnWSJOJjjlqPJpYQAqV+ilAh4BWiClFSxJQNR9KC6YrQuPj72XTRd8PWK9Y1WMxl9+ije/6XB3Z8gB37P4uPe0mSBLRGnjcQE9o7nATUIBiMkSXkbdcGqvSSPFqbAL6sfJKypFDEkBW7x77GjRum2bt3AV8UpTiWmYi9rns/IwRwpVgYmvOMi08/vOvg+Etl09f/P3jgfdr62DvZ8OgX0aplb7DUKgFbRLIAEqtAi8gcserQGEgKR1zjEGiHgJCukiLVQIwFeNj28ZfAsz/OfOVEiuY+pDpF1XrWSgJuzUEjcYZsjTd6PgqkFuenMEVGnDLU7/lLtl77BmgW+DbNskiXQhF6yIAjJnOkxqB5RGypUxijUnUlj06eApoQUg8xY6oAown5yS+m+swfZXr7C9YlWDTDHbq/cSd3PPyv7Jm7lX2N21BaODNTtqtjyYsFojQwiUGMISiomhIgtOxft12NLmV1U1xi5gR8iIjxpfCVRNAEY5VG8fDY13z2aSdy/d47MdYSshyTOjTkE8u87t0NB9k8M/XKkbuGU14m1W97GQtf+UPVz/8amxYeYL4F2Dqqhmhb2OCQ4EnxIEIu+WFJm5WN0j2/IWqOxsCmhbvY8uHLuOVFN7FgNpFn+6nVZtbsWtYcNM7ZkMqNu9dW2d3UEmgquAwSx6Yv/B5TN76dPERSV0e12Y6d9/NaHRqPIzrwRSS14KylGQLOGMSZ0hAGKBKhFhy2WdA48QLqL/hlNp3wynUJFrc89Ed6/473s2fhhlL43qVALONuBDx7Fz0HkbJiw1pDjNpWTbNIu0dCJJYMZN33WA2qoIuShwZjhaAJYiIaLDF45psPjj8Xsv2YJCEWAZdW8MFP1DTWP2JA9IhRLjr7hCN+NVOXvlG49I3Mf+rH1dz2e0jWKBXwPDiJFAI5kFqwmaDp2ibNOi0FqkvpubKYtpRCsBGIj3Pahy7hoa/9IjuLEzh3w9pNe3csTLkiNkhsQmIsla++lfpX3oVgiTagvqzD7k4MHeqGrzTWKLRJZqGpBcGAkYSiiBhjmMJR25/T3LQV98KfZtN5b1pXdmxX80a9+5F/574d/0zD344Rh+hUu1HO4X0s9VFwaEjbYb6ImICSo7GkZNdYNhiJCEogamhPbruYtzLd917N0vNQKcFEIqplq5KPc2N/l+OmHLEoSkMktuzmmlRQreshpgz1WGs5f/vUurmu6Wf9uuw/+1WaXPuTNB68lrp6QgXUJdhQ0GooSW0Wwr7DB6/aK+kMEK0jFh7quzn73y6i+fW3AGtXgXZMgIanTIAf97kfIbn13eSJYMhJMohVC0VgVAnNocAPKTKMLfs+QoDphHJSZVCvG/Y1K2y45qeYufyd6wYsHt73Gb3zkb/lod0fIIuPE4PFmhRjp/CxheocRioYU8HZJjH60jMzScm3ohU0RIypouKhy5FT1XYeqaQY7Qbp2NaO730ekRjLsFaMARUFiVgzPuHaJeeczH999n5MbRqf5xgzKZ5a70PFAhmqhrO2pevq2jaceLXw6o9TueVPdO66n8Hue5RIJBPYOF3Dt/aVdOxrCapd6pOlrOtSnZQqiHpMDUzmkWIXZ3/8lfC6ayegsdzYmLWY+sS34h55HzEW2BbYpEZBkyIPiyyXa1WFpkQMNcBSYQHJlVzBz55Ea+t5TF/zbuz0uUccMB7Y8zG997H/4IFdH6blHwAb0WhJ2NZetBkxKqJVjAhilBgXMDqD0kSML2U01RB8WSWlEjBaXzT+ixMZizGubLCLoV0W2Ka6127PTzFWkXYBg8bSgwka0ADzrXt0unrGqu9dVSKkKTFGjLVEnyPOMhnreEQtK4RMytUXnro+r/GC75OZC76P/CM/pNz6FyTaZF/WpCZ1oLHm4SkZGlovk+NRwM2D3WBwMTBr1paE8ZgAjeM+9mzy3TcSYwt1ZSkrsYlgqToI7e7IDnAc0sopgATy2ETU4azSKCAe/xQ2Pe2nkDOOfN7iU7d/rz70+BdZyG/HVYBYBTZiVPEsEO1+ovp2xZND1bb/XZT8/LoAGDQ6vJdSWEsEY5Iy6U3bq5DY7osoE+AxGIIXjA3tyqnY9ja6iwGlrALRzvsUYyIaI6qWZmsf09XVf1eLbwd/I1Ej1jnixNdY5/Epi8SA98rzLzp+Xaeg0hf8gci5L9PGdb9I/aFPk5gGa82DHQC7CBxdRT3tTZcTR556zHyBplWyEy9iLaXHDgtoXLTZyi17CvViMR6sQIHHJq6UBl6hJVyCLYuWg8E5Q563CJUq03EXJ7z/Vejuz5PQYakNWN8OSbT7IA42HKWmw6UmOOeIWUFqhdxCIUqmUHMGvCdPT2DqKW8keeo7jtjk353frLc99M/c89h/0PT3sug/2yohtC9LmkQiRqStKbA0zcQE2p0roK4nviMmslj6F0ozXZ5SMOra9zciEsu8R/+NBJBIYIkB2WCJ6ktPUCuEAoxTMJ7I/FjfPct2Y5I6MWtB4oi+6GQNJ2OdDivlbvqkk2tHxfUmp7xEZk95CXzmN3TP9T9DJWZEFCMKtkajaJK60rhKAG+F4C3G+LIEPU8xNsOlkLfKJbZ8eGrRh++tVS+XHUFzooFZU2FPaDF90lOOLU+j41ItuV2riDk7TyU45lNPlgVq9Souyznhg68m2/0F1joKWok1siDlrjXfT23G0mgFbA6uWmfKZ7R8ID3v65h6+q/C7AVHxErd9NCf6B0P/QO75m8CqWGMwUfFGcPQbpmu3f+QJ8VyT0akS8R3adPTHgbV2LcjGu16RzGoCAZZfJ9G2onx8QLGGzdMExsPlt8rSrvLeDLW8wh5Bi7ljK3jh1Uen/t3PW7mCHnzV79VNl34Qi0+9lZ2P/Axas5hmk2mrLDgFO+hYqsk6rEYjIFgFV/JySKQQS0pazUO0lFDPGQhkiTgtpxzbICGaCw7gNuGAlsmSI0YVoSNYJhPPZUiImlBXAic/rGX0dr5cWaATNZ6Ujdx1QQfGsQKNJuBqFDUNzCV7ae1+UI2PPtX4bSXH/bJe/fjH9J79/wFDz5+IwutR6hUEsQJIe4FElK7gajNRbd2wJhrl1GWuJi0XmoogtA2uxbp8hbKJr0eAOhp2lsKWw0ARx9YlWHDsr/FmFCeWy1EOzYdwtlnnoGt3Ikt67oWk/GTsY49DWsIaY1nXzB+ue2nb/4JKrW36pnbvpPzTnz74X/QGy6X5BX/w/Ff/kNtXPtWxAlNKahmQqgoWdEiTdpbMIXQAnGWSgLe+JJrjoNvZHMqeJSkugGz+ao1vQ+HDTSslOR/IsmS8Lt2dG1XuEhnyLKApAVJFjnhIy8n2/1xqkDLuYNu419pxGmBoqAApmOFQjzqApYCufLtbHj6Lx32yfr5e35d73r0P1nI7kalRtAFXKr4WBAjOGcxBvJ8D852JwV6wWMogHSBR7fXEWWp30JjH89NJ9E9ABzLfFYHONSUoBEj0cRSFSCWaizjLqjpaoXgcwQFa1ACx0jq7pgdYhSKnKvPHa9MdM/Cterd7Wi+iZsf/iVueeSXdfP0kzlr2/dz0qZvObxr8rI3Sv2MF2v2sTcyff+HUAdmHpLqBhp+P8ZAVSyVWCVmHhPKisvCC+Ygp2cIkDpD9BG36ZI1/6qHDzSMgFfEdOqMS/781Yw8b1GrV4kLgRM+8nJa+z9NJVYRaRGCX/MvETJFnWXKBPI8I1WH2/osKs/7VeSEpx+2yfngvg/qV+7+Yx7e+ymi5BBrWDtFwRxCFbSGmAwThZCnqA3YJLDEcxAHwKMvrjTU++j2Okrg0OGhrg7gSH/odenzezyZTp9G27CrhjZNtkXElPkWmuMBvIaS9k4Ba7FqFvMnk7E+R0nxo7z8ilPGWks75z9VKlgW8xhTUvbsnPsMO+c/RXrv2/TUzd/GJaf94uEDj9mzpfLKD+Kve4c2r/0lkpqBsJ8ZnSGYBrkGKraJEwhF6WHFSoEeJCGqKHgJpR086UXHDmicOVuTG3Zn2jEiqqUx0qgrehqhUuYwTv/Yy8h2f5xKrKJJgGbJcaZrzUJrhNwHGgZsfStc/GNUr37bYZuMX3noN/XmO9/PQnFbqW/sp3GJEMwchT6G1ePAtAhxAXyCtQnGNkuagWAxi0ZTujTbuxdtt9cwCCDa9/cBhvRadD7BjACOpZDVYoiya9arCqoWIZabimhQKcj9nvEMUIhlsULhIYKPikwqbtc3aGA4/bjxk+C7G9cR8inSJBI1IwTBMItzjiLu4PbHf4W7d/2+njz7Cs4+8XVsrH/NYVmz7mm/KFNnvkYXPvqdtHZ/kY3NOVwVQnA0Y8QlkfYyQf3BtwIkRsijIirIqdccO6BBuX/sqV5abbx6Ou7ihA++mtbOMiQl0oImFDPAfKlDsZZjb3BMpVA/8dlUn/2bsPmyNZ98j7du0Vse/Atuve8fMDYQg8VYR9QMsQsELCEIztWBOQTFmjJwqhrwQREjJCnEohsgynigdFWsdRv3DoD0gEeXF7G4oxcwuuR90AaSbo6cDhYNeh39HmZc7AoXMWUvhxgktmi0Hh/P00BLBbi2d2SsQ5mw3K7nEUPkynPGz2fsmb8RYyw+b5Kk01gHPuwjDwFrUqx1eOZ4YNe/88Duv2dD9SI9Z/ubOHXLd635+jXHXSIz3/QF7Cd/Unfd+seY1h5mjZLEGk2/QEzAeqiECl4OrmhXOlHc+jTpSU9b8+92WEFDpCRPKRu8FLOo4bz89zzh/a8i2/0FZihzGCF4kmoJGEkd/Brz0W2pbiB5yjvgijev+QO5+/H/1pvu+zsenbsWzz6iARvriPUELRBj0WghGhJTLdvQVUEK1GRABjicq6CakDVzkmFPednKqf7wgfZ4Ef2gr8PCWz2Jbl12g6ASkbgUAtNYiskEKWj58ahEjDFtUkQDMaKYSR58nQ9JUy4975Sx39doPY7YjMRZfJGjGIxN2pEMQaPDaBWVFs5sYK51J5+/53v46gM/rSdv+UbO2/4D1OzaatbUn/0rUj/l2Xr/J7+fqf33M6UtgncUqQeNWD34NHjUUsTNTm86LM/rsIJGGoXCBIIJJTpGQ7QWKQRXaeC9Q5KUEBaIMkU9Njn9A1fT2vMVUsoqKWnnMDSUHsZqAKMShUKEmEa8B6eQOCgiqBjSYEACjaikUiHVjCJCkU5R2/J0ktd+aM3Nzlce/hu9+d6/ZvfCDZQuQ5XANEgLqxGNBqh0JZUjQVtl6WtnK69u8ZGWceJI4pYagobt7BddhD6vYylkFXsNfhcgdHIFHfZxpdM42XVu2v2BPTHY3hyL0elSgMkYoi8Zbq3JyDPDlB2zfDCAkwQ1+wg6C1qsHRXAZKwOyNWjJiFmHqnV0KJA8FhjiKrEkPCcs8aLIT6+78PqjCFGIYgpy04XPVhbPnIpCTZj8KWHLg4TK8S4i3se+VMeePRfOfG48/S8bX/ITH0NweOMl8ipJ35U933yZ9l92z9RdUIsPKlxBM0wAWxiaRKohHK6tqTsnEo85CvcmjxCxSW0TnsllWMNNM7aksotu71KW5awY0mkXgoVqUlIWhGTTNGKC5zwwZewb9+9B30jcqegirRSqiKIZBTzCUqCqzYgROYrUE0spsgooqOVpmy86p3Yp/z4mlqcL9/3Lv3i3e+h5XcSdK5M6EkdNGAkQ6Toe0y94R1dbHMfluwt8xEysNPvO0eUxQqoYUnsTv5pdV7KYH/HIh36QEiyvI6yIKJkxKVdRaVAkqRMTY1H8WxtQu49SWLKZy4ySYMf6fCTtr3hxKHBAwERi1eFIFRsk2deeMZY62zvwk3EmIMpYIVOLecSYtD2HBPEBIyN5PkeHnn8Nu5+7HxO2/pKPWPLD3Lc7DVrs96rZ8nsi/6GYtvlOvfpH2NDlhKt0jBQp8I+nzElFXKXoyh1n5LjCS6u2JHgxEAs2HDSs449T6PclUaM7aRmDUZLb0EEqlIQnMPrfs75+GvJH/s0Uy4ctPvmTZvVm7zd8JZCxZNWodWAJLVMNwPeBcQYihMvYONz/hp7/NrlLq6755f05vveS5Zl5MxjbYo1W9pGtkDVAxYNtSEsrUPyAsMTCCyVvWqf0R4CPjo8UT70/SOAY+nvhoTDKHm6urU0OtdRUpOUoBNjWSAhxpAmYyZITbu3wzrwE7hYD0NxEME6IRQehHbYUMDBU87dOPY5H9//SWKMpQrnCo85xpIQ09oyBBpCwJiIS8GH3VTTWe7f9e/cv/M/OG7meXrmCd/NKZu+eU3WfnL5j8rmE56pjf/5Rth9P1UH80bZmNXIQpNqBBLLQmKwmcGbuKKRdsaQk1A/49WHJ9F/2OOX+HISdZVh1guhWbN4HwnGc8rnfhB54L+pOUPTdgz+QUxaL+UG2CreRgqjFBopfKSS2pLKJHWk3qMX/yCzz3vXmt38D37lx/TeHf9KICOSYCsFCSkaDT4UoB4xBU4MQpUYUoK0CdEk9hn9jvvflWdgGHj0duEvGf/BUlxVRZGSEqEPeMrXuzyGNhAsvnuE92H6ymxVy5BY5yPEGmL0SNS2pyVtxltDEcZ7FHPzTcRJySkjE9BYF8NaKAokgpE2AwBQUhArL33qWWOfcm/z+vY80VWkrBRjyo7sGA0xlHPfWhAJ+CIlNdtACnYsfIQdd36E22q/p2cd/wbO2Padh94WnHiV1F9/H40PfJPO3/OPOJ/jgRpCjqBeEJORp8qGWGp3rGhTN156+Db+h3v+nLu1LqYtfdoJhagrSyVbNuXUL7wDveUf8WkKqoeEAiJ1QggKVPBe0FAwbR1ToYbJAgs2optOIX3VR6isEWC8/0tv0T/5yFl6186/ItMFQnvhZN5RaAMvDYwrsInBmgrgiFqANHt26xpliB6I6fEolvQppCu3sJRo7v678lym5xydkEJU6SnH7f6s3vd3eRM6OKWiCFF6r1ujoBiUdjhKlUinQKKtyREZ8vnLj4ce2UFUpfBxkstYL8NYMFpuDESQDl2lRsiaXHnucWOdbr55u+bxEYxJQVdJzScR7z2qijU1RKuEwhG8QGwS4z40LuCswznHvuZX+Mp9P84HvnqO3r/z79dk91F/yT/Ixqf9HpaElqsSjYKNNFKhKrClgHwVG2avUD/7FccuaCwZrKWdasuWZH8nf/UX0VveDc7jiwhimTkEj0uJRFNW1Jio1BTS6AmhSUxSps57NfXX3S2c9vxDamX25vfoh7/6dv2D/zlV73j0P8his0we6wY0TJWU6jZA2IzxGyBW0OiIIcGHpMw1mGLIF2qDxwCADIJHqW1R1je1o7o9eKJSlqku2WbTd+86htuMAKohANIj99oLHj0YswhwJY16J/+gGhYrrsyY9dT79jdKgkK1E9BYL6NNU98pyVaR0ssoWkynyjUXnTDWg9q18Kl28Udsyw6vZG/Kys3OvDLGtIMsDmNSkkrEmLZN8glGE8R4guykFe7kS/f8AB/88pP0/h3/cMjBwz35TTLzdR8BV2FeIHeOalagRtgfwbFy6K6lkJ7x0mMbNAyyqD4lGqiGObbf88ckn3snyDw2FapS7griISin9TmkNiXEJs5AJRV25bB745lMPf1tTL/onw65dfngDW/TP//QM7lj59+VVUXpHIoFnUHcHCaZx0odzasYPCotIk1UGuBaWBfBWiLpUE+gZ3e/isc64BUMeX2JQrz3s5SyrK/jHfYfwwBk1PXFobT0BsEiYnuu44AUFlXACGIdoIdcpXEyDgQ0AtLRfxC76P0mseDJTzp97NM9tv/DiDhiu3pwRXsjKdYmWKelVksoCCGjLHb1hCwpdenbG5QYQEIV42fQYjPY/WTxLr543+v57xvO1ft3vefQTqpTnyWb3rBXKic/m6zlydI6WkSqSZ3MLawMPDMnwfFXyOGz30dgdPQYOmPTjg+g1/80tapSoYLOlY1pXhKKQ0BhW7XgvcdIHTHC3pYyfeoLOPnFfwFP/rlDerM/ces79ffff6be+th7sFVHK1iyADHO4KMrd0Z+G75ZJcZ5rFuAGLAkWK2hISUGQySgEsr/qy77yEKbF6r3MIv/7geHoUyDnfXNcM+lO6x0KKaUdvXrlB4Ti2EqaQtAIVrqfIyDGaaMn5e603FswsPJWJMFTwhFu9LPtp9NIHXwlEsuHPt0expfAk3bdPorP98QhOC1XaUXQAqMjRgbCbGFmDateXujKqRYazEuw9jdaKgTfR2hTqt4mM/e/V18+ObL9bG5DxxS8Kh//Sdkw6VvpO4bWAcLsUElFiu+b3rr+Yf1cR4RJrfzN1fk9p25miJlw97/ofI/rwU6CZ8MSSnL8WixmqSGbbtoTkqbrEClBRV1NMSTpeBCANtkIdaoXvw91F/4e4fUmlx75x/oTfe/l7nsDpK0AsyWBk8yErGgRakbQATJMAkoSdmoJxB62GcXCfRZbJ8bqH5aMtGxk1zuit92v3toeUlXtZX0AMcwWpduIIiDZIXLVFtZEeKAt7F0fSKCRE/QCipZWYoZBaNVYtzHSbPPGus5tZwB5zDqISSEpInECWHhkRxJCBSS4G2ChAwpMpJ6nYVQ49VPGi+U0Mhv02b2GDG0qKRTFLliVuhjsO1Kmv5QpypYm7YD2OU8L8+VLVZ3ClUQi7GeEDPQQMVWmJu7jc/d/u1smj1Oz9/252ydfeohsSfV5/+BxK3n6dxnf450/y5CtTTSeQCbQG1+A/um9zMToZVDXqmQPentbD3WQQPAa8oGdmM+8rKDPxdQa2+gTQbRQ6tqWcBT1wq1PGNBIKlsYPbqX8dd+H2HDDBufeyv9RM3vovc7yXofpxzaEgpgkdMxKa2y2jGIWF2wzANqu4++cFq2qVy2Th8u91XydRbJdXToa39fQyC0usJLleEpKqLpx92fWEFavRO9YtqKEsxtazhF5H2gh5vPPDwY2VYL8+R0p2ZWO114W1YCDmSVMoeiYUFXE25+uLx+jN2zX2pTXAZCCGHNdWoa6+a6HvmrpiyoTbPG+zZ9zif2vdCTtj4PL305N+mXjn7oG2LueSHZXbDefr4J76VDft24D3Ukxnm/BxFbT8VrbCQCVIVpvIm1bOef1jd6SMGGk/ahiz88WWlsuhBUwMZEiP4CAHBpRYTspJhw2TskypTm0+l8vy/gu2Hhpvllsfep5+99dfZM/8AkjRKf0eqZa+DCaTtNmn12iUktLI4UZl0jotJ4UUvoKNfsehx9O7+VwaOXvAY5bn0ewqDrw/rERlGUrgMi+7Q64vtctwyXR/Vt3XXxxsP79gHhS+1xqEs95xQTx3REdvzNjWePHfEkONmpnnuRePvjx/b9z9AGVpSwJi1Bw1Z1G8o52eMtJPpgcLPYxLDw3s+wCP7/pPTt3yHPvm09xy8jTn9RXJc7b91x8e+l207bmZHmCNxZRujjxkxhbpa5LgLD/vzNEdqIjX//mkaWg+ixcFfwlRRZacEGjYQTMSj2BymZTOtCPa8V1H51tvkUADGg/u/qP/6uW/VD3zxe9jrHyIkQqBGlDpRKxRByINvV4fVIGwcest7qpsGcg+mp/Jp0d1QoVOV3v3+kWWpI0pge/sulgldDXyODp02i3mJdhntMCDUYeW77evrdKObtrBAhxYlsZvHfj651MoFbts7xGKi23ekR1AwEpY2FM7iY+DlV54x9rl2Nz6zuJkpS3cPgwmTWPZIYUENMZS/cwm4JBK8wZoEYoV7H/9X/uvLJ+stD/3iwbu4xz9Ztn3TF2TP9heQmBrT7TB0GoVapUYeA/GCb3vigEZyxotJjJLKwS/qedtgVlOKBBJRbJ5jnKFl9jP7jD9k9sXvPSTexYduepP+y7Vfx717Pgl2A1nuCdIk0EJNhkk8LrWl6JE6YixQWb0eRD9wLDOL6U88DDXKyxrtZUpmu0BqmAey1NvBiNdlmdcHr7N8T6TMcdh2I5Zjqnri2M9oT0vBGKy1uGRSdrsuhrXYmJNrUrZs2Aj5PC+54tSxTrO/cZM28nvKQpFgAEs8LHuCJc/cWtsOWUVC8EQtqCSG4JuIesRmFGEPNz7803zohov1kT3vP2jw2PQN/yX1i19LS5TEA7FGNlcgAWqX//hhn+BHDDT8+d+KY4ZwCB66pNCwOduaDh8UtVBMVZn+2g/iLv/Bg76pn7z9d/WPPni13njv+4nW4jUS2lKklgQrGwjekue+3UBUlhSrLRDbWMVOXXq8jn7D3m/0dQXwGJzyS0dQGai2Gtoc2AUeo3o7IkJE6C8J1i7wWAIQMxLIOgtfo12soooxUk3GD1/cfu+jUHhC0Sq/cZyAxhEf0imhdkj0RJ9zzvZpzj0uHevh7Jj/CFGz9sxP257G2qNGpxR8iZ3bLBasCI5Wvo9qpU7iNhALj0iDxE6xv3EPn7nnZXzmtldpK7/hoMAjfd57xD3rF8lqEGlQr1nSbZcdkcd5xECjuvEcada34+3UIfB/oVbAzkRwVIknPpnZ71kQd/LBJYju3v8R/b8feo5ed+evkZsFCpuTe8FIhSi7MBhsmEYDGJE2BYYS1OMDxFhHZeMqd+pLRns5pblFgGHQKRh1DAtbje4sZyR4jDOldIzpVvZoSLl71HaHeBSq6Zaxn1kjLwnqcK5s8pt4Gkd++Izo6m3OsYCkFV525aljn+bRfe8HLMZGpK2sZexhSFhp2UdUrsywGKoSEoykJNbh8xrN5gKJm8GygRgWqKRlEchj+z/Mh258Cjc/+NaDAo7qpW+X2jP/iOb0FHNZhjz5zU8s0ADYcNazyc3CQZ8nSoJR2Jp79OLXMPWaLxyUpdjXvEv/6/Nv1n/7+BtY4HZMUmW+9QjGVTCJpYgexwwiGVF3YlwT8IuGVkzAJh5MkxAbbboMhhymLwdgVhG26jXMOgJYVvWY2+Axqr8jStkDot15leHO+7LXt1KfSbmbk8XwVBmictRrB6APYFN8Xta2e58vUVZMxpGLTjkheIdtc6epD7z86ReNfZ7dC9ejMVnspwgxAynW/PpjLOdnmfyOS1GBWHJZCQ7VOarVOlEX8KGBZRO+iFhNEVPgC+HWR36TD3zlXH1wz98cMHi4i35Atrz847jpU0lPfMoTDzR47p9IkhvUQIVZFFdSGVpLy4ENUImWnCrBQqI1jE9oE0EioexxqOcFseIwV/0c9ef/1UEBxsfv+T390098PTfu+VdiHQpfIQYhTWZQHyBEnBiCBoImBKkTQ0lJIJIADo2WGExbLKlkdBU1i4cRWQwsydBHYtp7Gm0r5I3yOGQEzxQ9nX69oaIhBjtK+1CMglGzeNi20e3htOqKOnSOwY7z3nBVbCf1S0qTEiwXL9sowQNmHiEhBiEGwXH8eGHE2x4pN4KGdudximgxsdpHeESfgssI0aG02DZV4wUXbB5rnT627781FA2sbWC1ArQ52nTtFSSMjV3U/nax+bT8fV5WDVrT7lB3ZUGHaWJsuU7FeDAZidlGK3uIz9z2bVx/92u0mX31wMDjhKfI7PfcJ2y8UJ54oAGkJz0dKxCKfYvlnTGUIumlelskcRlTTfA0ibWCoAafW+oamVHYs+lkKs/7R3jGTx/wTbx7zwf17679Jv30l38fNftQDeTNFtbU+4zhkJyB9mvXmWWjOqujtujOMcgyj8oMDSONLKha9rPNwOsrVVct3RNZ9RQb9hnls7eLO1PrIrMzJ4/1DBeycmE758rtoe2URk7GkRxiYklO6ApiOsULnzK+St9jez+OaslFFqPHGEPUrJ0QX+fRuQKsbiPofmySU022cNcj/8xHb3wZtz7480ddI9GRB42zX13WPQuAJ1KCutG2Mh+WtFEjpoK1kGGpGMeUCexyyp7KmRz30n/Hnv/1BwwY/3PT2/XfP/k2Htr3KdIpIS8SrKngUk/Qud6dNqONcVxFGGYxLDSsYmmZx7NsaW3/9S0LHGaFxz6MpdYshsi6PQ5lqQS4Ax7Dv9OwRHhXma52auBTvPfEGMiLBerpeMblK7fdXzbTR1/Gz63F54HJOMLhKdP+T7GfGByvfPpJY5/j0f3vK3ftWl+iDxG/HkzYyqApgnWK+hmKzIDbRaUKzfwxbnvsV/jwV6/Wx/d99KgBjyN+x+X0F1PYGkli0XavpyNB2/RBHkORtMiNMh/AtQJRUhaMsGHbM9j0fXeJHHdgZF337f8ffdd/XqVfuvfvKdwegjqKGDDWk2dK9DWcrQ0xzgzZaXd5HdpL/FdWGmlvgro7zzGUsXbpEfXmGGQI15QhynBqdB1aYdVLh95dXbVYYdVVZbV0HcNZdHvzNNJVXaVDuKxGAF7ZiQemU1BgOH76qrGe64JO09FOwCsaPEm1NrHaR3qnHRRixCYJtmjymqeeOdZznW/doPP5zW35DV2ksVHlqOAWs9ZRxF0Y18K5Kr4QrGygVtlKDIa5/Et84rZr+Mr9P3pUAMcRBw2z6XypnPRcWgRKRgqLocAI+EhJUVyJmGCYBWr1CoQcd+brqXzT/x7wjPn/vvQj+t5PvY55t4dWhChVVCxRF1D1JG4DVjaQ5/kIQ7dEujcsRDNMr2KwUa5vB65mFd7HsPMveQSjjfqSZ9Bt1PtZagc8or4sfH+J7iGZcmIXgRMiIShWZ8c+8xduewixjuhzTFotNx3FxNM40kOxEMo58/Irzxn7/Q/v+W+UiHVl8tuYMkQlOETW//PtJNLVzIO0EJ0mL+Zp+UewJgWbUa1Mc9eOd/GhGy/QR/f9x7oGj3Xh201d8HrmOvxKVAkK1jiiT0hMRAuQoBAM+UKGvfRNVF96YAnvmx/7D/3d9z1Zb3ro31A5nlbMcUkFr3nZuKM1YhSiNsn9LlyiBBVi345/6ZCBnIeu8nYvGvUhHkhP4niZ0tglz2GYUR9VNbWUNF9VKKnjCbUBJErv3w7qafQBTZfXsdTf0eV9dd4rEWNLL2N26syxn+0+n6KhQ6lfQNHCJhOywiOPGkJiE2Ke8Y3PGr8L/KE978NQL/nJiKUnKb7cpMn67/hXVQxTEKYIQTEmUklncbZKoXuxfhNFNo9RYSF7kP+97dV8/u7vWLfAsT4Cgud8s7jZszBQdlUKRLWIKDbQ1hRWfKXK1DXvpv6c3zggwPiX696i//b572FOHiGYKfLYoGrrpWYwQpYVJK6CdZEi7CatJYRQWzLwcZU5BXpLYQe9i/Efy/DqpKV/d1dHDWsQHK7F0Q5h6SBdSE91Vj+ADDQhDrlOXZlCvRNa6EjPqnpi9ARvmamdPfbzvemOh8AYjDG4JIXU4f2ERuSIh2ecULRyTphNed3zTh1r7e5rfkX3tb7YbvwUhApITof8U+P6L6m21uKLcu0nrkbwrh3BiDipE7xgTRVrE4g5FTfFPTv/iv/6wnm6a+4z6w481k0WaePWi9tgkeFMQu4Fm3qkgGkPraltJM/+fcxF3z82YNy88//p7/3nC/SOnf+Flw0UMUHNHInJicU80MIYSJKEohWgqJAmsxRFLCVX+4zmgPsp/bv+pc5tHWJUWcXv+sNEw/92eKf1gT3u1Vc/DfJZLe/RDAu5dceiFx2fdh2/EcfGmdPG9zQaOYSAiOKLoqzYmYwjPkJsYqsVnve0y8d+7659txOZKwkxo2Ck1l6T2p4v6x80YozYtImYjBAzxO3DJq1y3UjEVeZBSrVSIUFlnsQlZPFBPn770/nKfT+jGbesG/BYP777lb9AuOUDmDSjFQ2pjUgrxSQ5e+vHse0l74ft4yW893Grfvi6P+aWnf+5aBMNsU0u1jboFojaprIoyzQjEQltQrQhvOCyiBLDRX46TK0d49hfsNtt2DvvX7KpvVoYpnPharo+q9cY9vZxRGRo4rv7CmLf54+ej6VMpgx8tkYhsHRPO9e79H2W7o10QEUp9Tu084U7VV4Ra2qEMIfoFEHnqCfjeRofu3GPOpPgfZPCVjGq4B3RFMsA4mQckvBLVCRNkeDREOlUOZea72XDZog533L5+OwP9+/+DaxWQdrU+cxjaPdmqG17Het7iMiilnm5JKo9Sy6EdvOgpfS2g20zTHhUE+7a8Ws8tveDPOWcX9DN9WuO+GReN6Bht1wktVPO1+LRr+ATwOc0LNgNZ7LtRf8Cx18+1s26c++H9f997GfRpDGwS9Y+Yz+M/rvb6K0Urxz+d6Ux7n9d+0xYGcuXkXv9iC4Cx6jPGgxH9V670WHewxBtDYZ5Qf2fPYQafYSH0rmHo+/REjAFr2WCU8syytmZ7WPNn93zzba2hwFr0aKYQMXhGkmCNluoFYwYYlQwbT14VRTLtprnZc+8YKxHkud36/6F+46GqtqDBpX+9VGuGUMpEJWxr3kjReagPvE0eufeBW+leOhbSUKGGNDtV7Dx668fe+2//6u/pF+8589xUwnz2RwVqY8MCY02Zt3aF7Znlx0XAzJmpLHtBo5V79gWd+VxWZAqNTt0RTBb6fstAkd/iKrPu+r8ONr4m6EeUHkPoUd5kI5OiA668NaWjVtSGVux79Z7HyeEDgDHdiMYY93/yTiwYWIgWiFxFUIIaPDlmjEl9UYM8LKnjt+b8dDe/0cRd+BM5Zi+f8PWVPfvEpdg3QzHb3reutgHrSsMlwtfJ9V0hphCdcuzxgaMh+a+rH/2iVfqlx74S0g2sZC1cG5YS3Zf01w3pUXPYcpjiG5F7NuND0sadwxxf3XRsDxHd3nsQJ5iyLVFlcXqquUea0mAKD20JP39HStVPw2CUZ986+JhlmHR7fuu/UQqEhcrqCoHwG77lTsfRjq6n0XZ3IeRiXLf4QhPhYgRUwKGKiZJy12yD8Qsx2jGD3zDs8Y+77073lt29x/r929Uo2+bKNEXlpM2fuO6ud5190Sal76duPOjyNd9aCzA+Nz979UPfv5niEkDzAbyfAeVdBoJxaLH0IPgHeDoGCtdMl5DYH9xx92trBcHTPQoZb7u/o3Bc3bv7LVHmS/2hIg6Py+du9OTMeT7Dez+e/+29/q6jXpc9Ah6faDVehftEODi94zEdljLdOVtupl6Rds70hgwVpipjl85dctDe1CxGJMQQwE2Lf8vE9BY852ngRjLZkqTpOWz9R7TLnc+a1PgqlMqY++S51q3I6Ttlt8nEgoveejlWkw5fct3TUBj1Jh++tsE3jbWe977+TfqXQ9/kmAFcRavO6hWtxDyFol1hCW17cGwjQ42tI2q/V6SMzU9hpoBAxx7y29NO6mtg+EjXTTQfeW6XYlikdHnLpNsfQA4FDxGn6M3mzLiM7rAozdM1X/+QSnYTg5piXyxX7NcsAaCj6COrbOXjT1vHtjVhFAD1zZibc/PWDNxNg7DThljFsO46j1iDc4qeavBD7zyuWOf8+YHf0FjNGXz3jGenOrP+y01DZfFL1tnL2XTzGXr5i4c1b7f3Xs/px+47pfZEb5AEVKqdSVrCc4cTxEaJE4JMbR3uh3jqD0GUKRkdx0VW1wOOEbnHoa/NoqxdvDaBhPmo86tqkteQRfgrZzTWE3UckjOoyOow9Ik793MD/FAOjunttdh+nXENaIaysUTDVs3XDrW1d5w327dl7WThqEEDWIpH6vqmVRPrTFoSAc4ZNHDcFYJeZOKM7zl684f+wHcv+sfFj36Yx/zYzuy0NnEloBh2j1Hp275jnV1tUctaHzynj/Tj97wO0T7CDFsIK0YsoYhSWuEkCFSJQBBM+xAqEj7jC69O+gu47ua6p9xXh8GHAHFLsb3hzQIrgAAKiuByrBwVPffyKIu93Dw0GGouez3HuqptL2OQSNuiBowImi0bJgaT6DngYceAVsBUgiNkrcBg7HgiwyxE6bbtfU02p60Me3qN8hbDSrO8Iyrrxz7fLvnv6yteBdiM2KoIeZYv39dJfpda8YYg3OO07d817ra9RyVoPF3175Zb9/x75AGQn4CVvKyZNMGQgjtyqIcjaWi3uCuuzfss+wTiVrWh3edQ/vCRVFNl12Ni6Wpnblu+rv8+gy00XLn3mtkB8FjCQD6G/p6dy1maEc5i81znWvvDZd17snosttu2JWui+snhhfpKzPuBhc1vRFqKUNSiVWKHKpJhe3T47ni/3P/LHW/h4bdAMYSNcHFBjHYNphMmvzWNr4yjdW5UmI3ncawQJSSMuNN14xf+XTPrt9AJBLDhnYFXH5s3z4tPYroE3A5MRbYpEKeJ1x46k+su+s9qkDjwcb1+i+feCd7iruIJkVyi4uG6AaNXLfxkxEeQefn0A619HgAbW9DhZ6cwejd+IF5Hsu/PsSAtxFgufd393asyjmWboM/GFrr94y07W3IMjvPXvAxbUbSYdoc7QR9NFgrbJk5b+x58aWbbqWIgBOIbWEo0975xkl0aq1HEpoUiS03P8UcXg3VNOHkzY5XPPlJY9/9R/Z8FJ9XsSYjqmKOcVfDMAOyHzU5xJQ0TcgKT7Uyw5lb3yrr73qPkvHFB9+rf/7f382OeCuNAGod3u8ltflIw7VUQmqWjFM/uWBXD8KANveQJPmw0rhe0bwurW/VEXtcsyp9juUSuKPfbxaBYzHZ3rm+vutfKoldzXkH729cTObLYEkwQkRQMSM9lqXfh1JTBWXrhivGnhtfvf1+vEnpENhJO5EfyxbiyVhr0Ig5RE8sPFVX7myyIud7Xn7B2Oe6b8ffah52IiZiLRjzBMhoxJwQCsQkqAohZkTNOXPb967L6z0qPI1/+/LP6Zfu+GdCdT8h1MEqSoaKUGgLpNqzzx7Y4fZ7DwM79aXqoEApebpSddWwmP2wJHlJ7jeiIU4iJXG0HXLtS2Gl/rdrn/eE9BOHtCug2kSPQvf1jb7+3vsWuzwRW4a9hnSWx44H0Q0IMjxnNNwLAaVApEKMytbpS8aaG3c/3tA9LUVdrZ38VkS07GMJoQxPTTio1nQEpN0X0yYatQom8hOvGF+O9L7df4aRGkKrbNZEkGPcU1RplFlNNSVvWqhQq9a44KS3r8tvvu49jfd95qf12jv/Gl+fw4jDRI+LLWIRqdS2kGmyvEYFfc0zOqg3MQgerFJZb/Bv+z2OJa9Hhjbv9Kr4jdqJLx2djMKial77vHGVyoE94bYVp0av5GxAVvz+Q1yvVX1UFI/ECpunzx9rftxw/z7UpB01q8XrMcb1P9LJWKORu5REEnCQFRG107zmOeNToDeyu3Vf8wYKPwdabiKOeQ4RQKzDmRl8bJTVnKScvOnV6zicts7Hhqmt2JqCWnzRRGKg7rZhqdAqFhA3NRB+6lem6w2baK9q3pCQ1aiQTU+4a0QYqwyL6IDhXX1YahBcusNIAxTs0msZhykHDnSk9wHHSsqB/bvK0RocZlXgMZBYF4P3BdXkOI6bGS8Gft3tj5VoGgPG2DaAWrQU5gDvJ1Z9rXfKlpIdWgWxAhr4iVdcPPZ5bn/sNykKT+oEjQ5rLUJy7IOGVtu9TKCSICbnzM3/ZwIaBzqedcmPSD1OExdqpPY4rKsy39yDmqIUUi3ykcZ70Uj25yCGUGH0GuNe4OiVW23TcXTJpA4Dku6cwrBbvjx4DAeXfgr20Q16/a8PUfQTesC28x1W411FWTmn0g9Sw3Iei5gSK4CwecO5Y8+PT9/8EKhHDIu5DIwlhHYIMuZMxlqjRlY+dXWQOp5zbpXLz9g8to/3wO5/QvGkyRRRi7Ih1B/73eBRC0Js4myNGCMnbXolG6bOXbc+8lHh+z3j/O9kylbI/WPkNJEqFEGxkpKabLT2dnvnPRo42v8e8fulHb6sGMIZ6oGwslbGuK8P84L6gUPHOP+AUe/3wlaYNroK7itGht3arLlaJXE1tmwaP6Rx872PYQQsSgil9om4BKJircXaSXxq7XfKecnxVjg0Kv/nVePrZty3470aggfTIC+aJXFnVKx7AlQymAWsSUpadM256sy/X9eT9qgAjRee8xbBpKjbiEWwBThxBIlEXFliqbTrdWIPFZ70hGlkSChIyuTwgJeylEcoqYtN1yHDw1Ys7d6lQ5hnZDQRopShs6CyeHST/3WuW6IsHkZHq/r1A2E3cJaH9HgCHcnVvn1Pec96EylDvY3OMTz/0+3dlJ3eGs3iTZD2QQR186hvcfrWF441L75w927d2SjvY0AQ68r7HnLEQgiBKBO51zV3NHDkYkhtzqmzFV7+tNPGNnq3P/zroC1s2IASENsiaoJqbaz84rr0JGLsiXosfY+SCYE4RdAFwHLm8a9b/xh3tNz4p1/0XVBkRFK0zdWvhVuiMDgUX3lEgnhQCrWLgmQIFz70KvxplGWSz4f6EcgKC3ylzzSrUhvsPkeHLXe4hrpZ5jra/y4cqdvE9g3PHMvYfPGOR9rd35NxZIclMTk5Kb/wrU8f+907935U8/A4i8FeNaAOMQHta+w7GoFjlF5G98bK6CZ8aHH+iT86AY1DNV5w7vfJ1spZxJhTEHE2xUpKiEsVM8tpePcDwEhQiLKK96zOtnUnl5e7vu58cXcivCdn0nMtMtb3G/66WcZTMSPp25efPuMl+pcySAmz9fHzGR//6iNlFnYyjvBWWihCYLaW8W3PP2VsL+OeHX9FHh5fmjuxUirdSYFSHP3huxX0MozU8HEvZ5/4/dTTC9d9PPWoqmd71kXf396IKHnRAi3AuqE6EyvF5LsNZUQHztEd2x+2y+kwUS5pgsuqvJfRVUq9DYK918fA0dGtCKxGw0MGdvkd8NH2dx1IlC/eAzOiOm00cPQD0WAT4dL3i6oYU3DSlvF3qNfdsRcjk5zFEfczjAUzw1u/bvxu/la4Sx/b9zFCDGUiXQIinTL6MHLjc1SF71bQywjSoFrbyqWnvPuomMxHFWg89fRXy3G1C3BGMUYJZCOSrGYVcdDuJLZZNKCDO/rhRrM779Ex3oHuf+syRn30RBrW5zGs92TJjTfLhKlk5M/dgNgx3tr3OUPzNSsthOU3pEOnXgg5p57w3LHmwm079+t9uxpEPwlPHXFHI3pmZhN+6luuHtvo3fnI35KHnW3dbwfioZNtU0GoHJBHvb5RxCxuHksmigZnb3vzUXP5R13nzDMv+E7iXIVKpQJGe5JMo75a6X0MeiDlYQbCJwOGc5nKIh1qrFfYGY3wPnr6KLrAYuXmv+WVAzvgNcrroB88Fu9BFyXJcvdgSHhrWEiq43X0g2Eqmzl5ejyDc+1NOwgBxEwYbI+8EQy85eVPOqC33vXYXyJiEaqLocbSw4jlzypLm6Mhx2pC0uslPNWd2+jelNaSUzjvxLcfNS7zUbfinnrqN8qZW55Dq6nkMWC1E45hmaazQQ9kWSPeHboZSICbVex6ZGCHPSC3OqLPY7CznKEGmBXCbqN2+sNfl0FPpA+olgOF4fdidNiq3/s6YfNVY8+Dj3zhYZCIcZOcxpEex0/Dz716fGLCux7/v9rI72VJydGDliSTpUSBO3q9id6VN4SWqGS2tdZyzrafOKq+zVG5Tbvmad8NWsEkblEtbOSufrVfeZXVTSslmYfvMmTE36zc53GoH+nyfSdtAF5uU6nDmHeHe2CLOZEVru2UE5429rf80m2PQMyIk+qpIz6+7ZXXHND7bnngj0mSaklYqRngUbVoLA1qWZBx9D/fYaHcbr2Mc09841GVmDsqQeOsDU+VK05+NqYlLDiPGI9qxFqHiqFQIVAm00Rbg2ZRTWfbvWIdVEcbY6DvoOtYzqiqlgSIRqXroOcYaohH9lEMTsT+JPlSjkbbn2/axxLN+eg2jKXKpqXwm2GQKqT93aO0+2TM0tH+gEVZKYngLYmEst/EOIKBqBVQzxUnvWGsRXPbYy2949EckRoqzYnVXmujRxPV2TKMag3WK6p1TBHZMJ3wa685Y3wv47H3aDPehEZHUAvGEXFEiaiNeI3lEjU6sN665zb00uAMK40/4kZWwCAIlhgsYiuQLJD5jCtPe99RNx+O2oDwMy79ASqmjouQB4NNqrRyT57nWKMYCRhj0CHcNSqr90xWrhYatRuX8Xbro3itYBWlrqu5ntW+PnjdnVzQsM9fjpewp4zZGEJbVrfjHQgNNtbGL7X92PV3LNG0yCSnsdZDVECaoAbrPd4IJi4glTrf/9JLDuicdzz6x2i0aLS9JalDJQxGm65RhRqrjzwcjuhUhSJ4nNmIJHOE2MTnFU7a+iy2zj7tqCv/O2pX3PbaBXL1+d9GkgtqLLkvcM6RpikSFQ053geiVBlWEjuKe2n14NEX2hqaF2lz6Q00vElPjmO1oa9R4NWfCI9Dwl+Hor8jjvz8FXRIMBgrxLiUVxEskQYnb3ne2M/+fZ+5D3UdavkJaKz1sNFiWCi9yqhgAsbC1mSBX/3mi8c2enc/9ne6d/42kHz0euqak8ObRllsKF1+7g1vzj2c3khEsCYh97sxOoWYiHMpF2//7aNyPhzVK+4lF75Vjq+egzGKWk+gIEYDsQyRqIFCIXSVyPYDR4f5tnOsxL00zPj2vnc13E1LO/qhnFj9ANTF1Lv4WV10JN2PcxA4hvd3dC++YWA37FoXvY5+Ft3+e9hXZVVeV2iH+7Stuy6IWs44Yfx4+Ofvnm/fp1CGGidjTUeQlFhkTFUTgik9dx8Nb37FuQd0vlsf+XWcS9seRkRF2kc/B1r7WIGFeSgL9TqaF2KKcqNkW0QxBO84/8Q3M129+KhsMjrqV9zzL/8BtBlIRVAt2jHEKhItZS4t9BhO1dXs6FfTWT5Y5trTGNfN8zRy0pce0NBdFGaxwmrYQuhvTuymY1+uo7xzrpV2Wks5D+251mGfz4j+jkVuL5GyNNrYUghKLBqgZk7ljM3jldr+783362MLQOyA0KS5b62HmgTsNHnWQH0GUufs4yv8xDc/a+ybf/+uf9C983cR2AVxZlEQrJfvTUfyuo1mJhhGzb8+wCMGSFJbbsYkY2P9Is4/8eeP2ol71IPGJSe/XC466Tnk+wVjwaUWXwSMcXjvB3S9O9H0lY1mV29BX4f4YJ+HYbT4k6xi0i9fYbVS3LYn5DZk9zX0O2q/DOvoJsEBidyR7r+MdNA7HtQiyMXICbPPGPt5/+vHbikzi1BqR088jcNg9QqkMk2hGWktgRh5x2uefECnuuWh38Za2xZZWs4LH8GysKyIGgzVkFmmz+NwzB+RhOCFoBbvA086+S1H9XQ4Jlbc11z5wxxXfxJ5Fmn5BbzmGGexCERdNjw0zGgP7mBYoblueeXA5Sb9qGsaxY81emLKAHD0As9o5cDVeFjDWHSXA9semxM9KmbRG1JVLMK5p7x87Gf94S8+DibHSBmCjBMR8MPgaihazINz+Ey5+mTDt79wfL2H+3b9je6c+zxiciybQLKl7uiRpJ6y7IZntBkz68a8WRdoNQuqyTZOmH0B2zd+81HtHh8ToHFC5XK59PznUnE1VDxpzdFqNXAuJQYzaMSGGMTVr58VWGTHTKyNAxyLv+tbNKNCNKPIGcf9foPJ+iXdDl3F91JCO4+hix3kzjkuOPFFB0Bu5yEsQIeXSI99kZ4jbvSsxek8uApIlV/73gPry/jqvb9GNd2MxhSvuxAxq5rbq6v+M+vWxBW+wdRsnaANLj/tt476+XDM+PYvvuAtcurUU7AxZaEo0OoG8pBTlWKoO1vG+6VPRvXQlOj15zUGQkV9rvFizmPROi+559264D0A0Uel0FOr3mHNNRbaEqj9rns/iHYfw/I1w9QL+6u/BkME5YsVEkLIkKqSRY9V4cSpK8e+r7/6H7drwzksKVGlTSEyCU8ddPhkUVtdUCwaQWMktQYpMmwIeDeLKXK+9mnH8axLt44N9rc98mfa9I+TFTnG1ImSARVCNMtLDLQ3Kd19Rf1rqCzw0J4ij0HvVwZyjYuv9/V5LBdJGF59FfGFwRgtQ+KdTnZ1KC1UBF8I5x3/40xVLpIJaKyj8ZJnvIXU16mkhqzYh5gKqgnDErVLP6yCK2oVO/hl/7ZLcGmgAmqMjvPVfeAgKAwuRjPW9S8X3ltU5luGFytEgzVVvPekZiOiGWee/JzxQ1PX3wnBYzSCKj4IE+qpQxB9kvZ8iIqokiQpYMmyAkmq5BpJNRKt8qvf9dwD+oy7d/wxRdhHIseT+30YScuKIpGVaX0GRMykhw9t2PyPcgCeyap7RPpXQoJzELzBWjAuI0ZfKknGKYwxbJ65gPO3/9QxUbVxTC25M6eulGec8x2Qz5MkDqVK6ImJDmdw7d1pywBX1KjbtlrwGMXHNIoHa5jE67D+jmEls6sHNjMyDzOsz2NZ4Fgsk5ShxQI+SpnT8IrFoT7ljONfOPbz/dTND5fek8a2l2EXd8mTcTCecbuz35ahxKJosygYRxCLVKAoEt7+mis4//jx9XNvfvAPde/CTVjjiCygeJyZwYfWkLnIgCc9au4t1zc1jONsWDRgOeBYLXiEWJSCUWoRcXhfYIzD2RrGlsBx4fG/f8zMl2Nun/ayy35MTpq+GKueEJQorWW/8vDKIlnFTrubRVcXj+VYcJfCViskudWsaoe/usoo7enzGCo7O6T6KY5w30ct3lHaHZ3bGaPHmjoh7Gfb1CVsqpw5lvH5x/+9R4tMEesIbf1vrCVM8uCHwArYEoRjuzNCDCZxYC1ERUPgjG2OX/qmyw9op3zLQ7+HYbasIooNnK0RtcUw+v9lCzYGAKQdtmr3eQx7T+hjbB4KmMPWzxheh0gEFJFSQCp4cK4NXLHg/BPfypbZJx8zteHHpHP/wsvfjmlNkbhWWxp2pfLaYTXh3cDBKll0x/c+hvV5dAPHKEO98vX3VUYtw8+zauVANWMoA3bfylB2wVKDMM9FZ37j2M/0PR+7E5sYNCrRJO2wmGeS0zgEI3iccyhlKDVqJGZNKHKwQIDf+f4rD+jUNzzw65rHR/GhQVE0MWxAQxUf50lcZYQEvRnglxo2r/vnXByyNqCT7yhzHsN6okbxyK22JFfElHr0MYIoidtA1CZe97Gl+jVccNI7jqlmomNyxV10/LPlWed+D5q3INZW4ZaOEB4aCJrKMtod/aR+IybikNse6e2G7e2qHtWIN7xkeDnhpKE/D3WwzLJqYyt6On3ehxpBTSDPcyp2K5ef9vqxF9FHbnwYR2wbsmq5QEPGJKlxKEakKIoy3GfLPgybOpKKQDbPCy/ZztddOT4p4Z75G/WWB9+FkQpIgbVJyWgbFdEZQixAQlcflBnhCQ/pIxrhfZScZL1rgn6veUU54vG+agyuXQlmENPCGEOrBVPV47j4lJ899hzTY3UZvOzyN8vJ01ev2Ly3XNhqVM6jP69wKG79qO7yoV5DX5d2d86jv8JptIbGysAx+o9WM22k61+uvRCbnLJ5/FzGez7xoPpGThEt4MEki+6fYZLTONhRSRxEX871WOa4jCi+Mcd0Cn/05pcc0Hnv2fF3ZOFB8pBh7BRi6nizG1xBjNMUYYn4vKeycIQHu6ysQfcaEZYNlw56zOagwKPUxSiT4argwxwVexxnbPlBNs1edcxRFhzT27SXPvPnEJOzXA1378QYjyVztaRnB6M3MX411Wiq9mFMuquvg2fZ3dnoaxGiWmKAtBI4a/v4Nf7/8b83kFaEKClVAXws75xNJonwQzCKVhPrlnIYpAlFq8VUNeH13/QNnLPVjG34Ht93rd5873uwMouxkaieLAtYU0Nsk6BzJHYGNBmcayOqpyLaU4U4qnpqvLm7vLDYaoAjaovglUiTUKQYFzhxy1Wcd9LPHJMcN3JsKGONHl+58WP613e8Huc3EV1OUMGGnCqWaKvMyTxpe+KKdpenxhHxS+m/g5iuiTXk5Z6J3SFpG23mBz+j3zCKWamktvO7Nl3g4gUMF6fVLqqVwXPFPlLAOPA3A+lJ6f2c3BW4IlIPjh/52vvHXkjmVX85SXev5QgZmkxD4UhoUiQK3nDaRuHev/q2AzJ8//WFZ+mC/xJZXsWY5eevGWWDJA5ZC/Ssz8H1GAfXwSIrhA5vhB0y/03X5xhb8kd1tHVUtS0SFYkxYg0oHtUKmAWSZBsvu+TxY5YU7ZgPCF960fPk3M3XoNGSFVrGVt0UjSAsFHuom9lV3JplSk9VRhMDsrzexGp39v07qzIGvJxGd29IanTpcBtGlk34rez99DcH9pcES6yQJpZzTnnF2Pfi7z557wQw1nrnWEkhK3DaoCBAnmFN5K/f+c0HdL6bH3y37pu/Dx8TRMIqPFcZGXZaqXpxVT1VI/o7Fj2VYbmRrjBt8CVXWod8s/TOw1KfCR7EIKQE4LKT/vSYni9PiCziG57zHplJtuGcgZjjo+LTBFfdQmjN9RrnZWOdK2hwcyhZdHUVCbuVcx6Lf6LKioQbq6iOWlWPSv95vCHkhvNPft3Yz+5fPnnXxKqv8Ygh4hQkEWxSgK3wwy+9iGeflRzQbvmG+38J4zKKPAx4FaPn2TIEhX35imU3cytWPMny878rUd4BDpEEkaRnpfdsyoIFTYmyhzM2/xAnbXnFMU29/IQpPXnFM36cGhYhpan7QeoUWYF1fuikHM3cunz10IGw6K7sdZgR3k5faaIuI4zUiUSstDsbUh01vL9jKc680nkqLrIhOZ0zt46vUvbBz905seprHp6KRJdQ+EjwkTOPr/A733vlARm+z9z2Y+rjfrzZjTVVYgyrn2eLiesRJeVtUkOVVaylZZoDl6qrej35YaAWUASDtNdf6XFoTyjL2RlaYZ4tG67myWe865jn6n/CgMYlx18jz7vge9EYSNJpot9DYg3euMGJfEge++pZdMfrI1kdi+5K1zaOctnwvx2tHNi/cEPR4LxTv2bsO/i7/3mHNjWdGPU1HtY4om0bQZvyrh9+0QGd5+E9n9S7drwXZYYYhCi+ZAlYYX4sasf0VP3J4jEwF6N0SRL0r6XVNQcuiY118bz1y862D99W3wyqi9rlWIMaS0AI5NQr27jwhN95QsyXJ1SR+wsveLOcs+UakhBIaEGEKNVBMkEGdcSXC1v173RiH1joCOXAxfd3qZRFld66dcyI8FNXT8jQ/o6lTvVhtCSjBZVYtrdDhyj3rRS+c3Ejl5z9dWM/r7/87xux1crEqq+1oxEM1u9HxfCDX3sFL71o6wFtm7507y+htCh8A2dORGkgVFacH8NCSz0iYzKor6GLLAwy1BOOSJ842fDw1WCj6zAuura3JEvZyxgheIhB8LHgopN/hS0bnvqEUAR7wnVG/cDz/1Rm4kZsnIU0I3jT02+x6AYzvKRvFI9Uf9hqtNwqB84dNcJ4D0+Am5Gx2l7gkKEMth3VwDjCgxm9+AdzHqdsfjpb3IVjLagvP4re+vAeQjObWPU1twKWFHjS8Y4//M4Dowr56v2/pjv3f6HklUqqRG3iXBVVs+L8WI23vLROhnjtI/MYZrRnPjJ0NdhZ3lOJ2G4mjaHcLCauxmlbvp0ztn73E0ZC8gnZTvv6F/4BuICXFk7tQdympZ3+sL9brqpqtWMgp9DX8xG7gGO49KwOeE3dC7Hf6xi+6EYv5uE5j97a+vPOfMHY3/u9/34tGYqxbmLU13qIJ2eaP/mJ1x7wKW59+D0gASM1IvvbxIdNkOqqQp/LAUd/YnrYOUbNz6XN3wj+NZYnRSwjBZEQiy6d+1K61dmUqakNXHXWu59QmsNPSNA4Y/ZKec2l76SazdCo7kejw0rAuAA4NFbRkGKtJcRWTyNeh+qgpCsoj962hAPJU4wapZMt2u4h6fxMvyDm6EQ5mL7rG6XB3HvGUQDUvUPrlB8GLYnaRBYw1IgkBLuHVCrUdJbLtn/L2Ivqb264HwkVoDkx6geLCWpQUZQZLB6NOUodDTmJKojjZ19/Ac840x2Q8Xv/zU/V3GfEmBIVNJa054ndiNAc0ow3LKwpgwntfs0Y1T72g6WcR0+yvOe7U/ZpxHZiIurSeupeV32/K88XUY2AxVpFKRCdIUaPuAaJm+aaC+56wonUP2GJe648+1vk8tNfS9J0WGvJQ0ERIhHBVZVC9hFCgdENB9gxvUwlU1/Ma6U+ikOmLDimHnI/qIkM1zu3TlAKgneEmJUNT2GWhcY+nnT61479bP7t+rv18QcbqG0AtYnVP1jQkABFBcJjGFsFqYOZw5o6uSpXn6i84xufcmDltQ/+ke7e/Th5kSFGF5tbNQoxls1v4/QALb+mRmyt+sK83cAxuBwGez9Gs0CX+RIkIcYqRjaT+f2ISRFmeOa573tiRjOfyIvptVf+jJw3+0yKYg8VswlnpgkyT+6bGHEU0aO6pK/R7wb3ssOOErkf8XpPzEjQyBD+/z6N8lUSXY2iXh/F7bOaZquhnbTtc3nvy6ZJqSMmYsRhzTQ1N8VTzv3OsZ/Ln3/wbhKtQWpAJonwgx0qBiMZaVIlz9pyuTJDoMm2uvLXP/26Az73Tff9IYVvYUzEGHpKUst5ZHpYlgcNuRljs7VCn1QfcCzXGLgcK3T/WjBugdzPYY0jTR2tbI6nnP7HbJy6UJ6I8+kJTxH6xhf/hZy06UnETAlFgXMJPgaMrZciKi7rmZRDeyL6wla9XaeGVbPotuUql+sIL6urdKhex7BF0Z9ziGhXhdbS+4eHq5YWjuoI2cu2MRBRRGypm4El5HOcuu1KtiVnjr2wPnz944TYgtwRNZ9Y/YMc0XvEKAUVMDnV6mbwe6hKlT9409dw9gnmgIzf+65/reb6OOIUSZRAxKsvd+imLb3aD2Ajei3GAY7B+boUWo39/RZxeQbd2M9cPQQ8Cp9RTbbSyB8DLBds/3FO3/bN8kSdTxNeaeCVT/95NlY2YmniW45KOkXuM3whlL1JS4a/v8qqZ/c/opSv32sY/nqvB7J8zsOsIifS7aWM0Ebu/H5kGWLZCDWMRbf7cJKgavEhW6RaQHMuO/s1Yz+L333fVzUP4DXDyTTGtCYT9GDDU0mFgEPzDLRCK7ZAKnz/y8/gNVefdGDCSo/+X318/uPkXgixIARPjBERizElVfgSjQ09bASdTdcgYecqTdOISr7VNgcOX08ysjnQxK2oNKm6bcxWnsaTz/g1eSLPpwloAOdNP1VefvWPkRSbcdIibxVITEhSA1oZ6M+Iskx4Z1i5YJ/nMRA7Hegql1XEd7uVA2WoB9JJ4PcunkHK5453NMBpxSiKQwbAJ8YyPGFNgkbP5umzedJxrxp7cf3J/7uRtJKBq0DwxDDxNA46PBUAE6hYR5omEBo8/aRZfue7n3lAxm9fdqN+8a7fBmtAbHsOlHOjBIulMFU3jchSv0VfH8bQXothypIj1llff0cZ5pXRABJlRDjKDM0JBp0jBmGmdhYvvPiD8kSfTxPQaI/Lt3+dvOp5b8fFOkYUayrkeQuh6AsrjQaO5Ur/xlW8iyPCQ6O8iW4PZKWdWbeXMZj/kOVzIiOAAyKm3V3vQ8YFp4xfvvmh2+f0lkdzipCDM/jQwppJR/jBj4AVwWPI811sT6d4z89dc8Bnu+7Wn6Pw+wkhICZgxCFYBFt2eAcWyfwWZ3D736NKvJfP1/Xl9oZwrvXP894w7/DmwNH6Hb2/S1wdK7Ncfc5fTabSBDR6x1XbXy1PedK3IVqguoCjhri5Vez6x33dDItIDbx/WPPfylUlqweO5abAcqXCA75HWddIDEIIAefgeef+8Ng7sj/7x89CPUF9CuIhscRV9dFMxrLhKSmQLCdQILKVd731Gs7dMn1AO+Yv3/Ob+tCOz7Y9XIfSIsayQ7r0ONuzShwiQoxheBEFg3xReoCKZv28Ud3rZ7C4ZIjnMaS3qDsiIFLj+Zf8f8xUz5XJbJqAxsB4zSU/Ic845ZuwoUJIcppaR+IciRGiT7BYTBCMT1FXakX0dHdLHOD0H9yULyUKRE1br2JwN6WRAbnZ1fBGiZpO1r6tl9FpZBoSehIZIk3b6e8wfVoaw70iI+C1TiGCSMalJxxYNc4/f+kOKBJqwUOYwpqAhmIyKVcYNrZwwaBaR00F43NMyFCJqDjUWrwkTGnkp19zFl9/5fEHZPweXrheP3fvbxOqe4nGYm2GDbUeT2KJZiOWGhS4AX36RW1uY1BjevW64+D8Htbf0f16fx9SvwcSVMpjkYlBeg4lR0MKsV72Z7mI6gL4OmLmec7Zn2BT7ckTwJiAxujxjVf/spx3wlOhWMBpgsgMzTxB3X7EKVENPu5D1CHRDUzY8foqdNlHMkqDe7nXR3kuK31+R2RmdSG1pcVd+IB1ESMJTjfy5AtfPvY9f/tfXafEBGzAOwsxI3jBpdXJhFxpuBkK8RhaSHOB6AwhqSMxwRTzEAzOJLz8Kafwc687cPnRT9/yFoxUAUdetJC4YUUTMiwPseL6WJGtebVzdJVrMBrEeMTk+DiHL4REjidqzlWn/y1bN509AYwJaKw83vDcP5ezNj4b53MilkAT4zYx19wHLpCkG/BxDqtTvU6E0FNPvpJRX82IQw/powGRwc8YSPaNCEW1PY5h16nCMlUn5eeKM8SgGG1xyuZns706PgX6ez56F2iKDU0KJxhywJXNYZOx7PDRgBgScioOsHXIPUkMxGQKEw1PPb3OP/z0iw7Y+H3khjfo7v034cMcMShW6oSYwUD4cHUszKNyZIsRqmHMtCP7O1Zgt+3b0PX/nZFaSRNCTqVSASI+5Fx11h9x+rZvmQDGBDRWP970ovfK6ZuejMYG9WqdEJQk3UzuM6wrIEyjujBQkRSFQQqETnlu53fdLLVjsuj2W/UwgkW3uzprNNHi0o4tdmmI9Pd5DOvv6Bw+FhgS1Dd52oXfPfZ9/stP3aOP7i6wNiGEHMSWErXWEuMkPLUK2CibUTXSEoO2oGIMSRqhSDmxmvNbP/bSAz77rY/8rd792L+QJtOICFY2EnQeaxM8xYj5uTqywP7y7oFeC+hZP8uLMK1CRGyYjkcUNJo2/YngQ5PzTnwTZ534TRPAmIDG+OOHX/xPctrmKwnNFiHfh0ZLmqbsX5jvo33urUjqTsANsOh2YrUMlrSO1s0wyy6KjgdysMqBuoodY3cORFVRDInzbKs/iXM2jU9H8Yf/9lWQSJQALm0rRaXY6MuyzslYfhQFEj3ROkhSIKJpjYX5jIpb4Hd/7CU89dTqARnAHfOf0evv/lWQGfIwhzEJRMVIShEXEGN6uNmiDpFUHciZjfAEWNK5iCIjQknlxmUlk7Zs3q9f2pUFKukMqoGiKDjvxB/hsjPeMQGMCWgc+PjRa/5JNtptzFZnkNikKALVdBNKCx/cUH4bMG2BFh10vft3OkM6Vhd7JzgcyoF9jYd9nzdy0bdfdyalaOQ87bw3jH1vP3bDDr3+zv3UJUdjAFcrCwWMJYQmmMkUXXER2wriAoQAvsCmGflci1qtyu+/8Xm8+qrtB9aPkd+hn73tt5nL7gMxQFktVYS9JHbTYmntMKPcndAetuHo3agMF0laagocDL9GXUbcieWa/waBw9qEVrYf1HHu9u/iqrN+ewIYE9A4+PH6a36LSrEVG4WKq5e7LJtg7dLuZ3jJ4CjBelbcAR3cOBDlQBkBHMsv+hgKttYv5/LTvmHsxfbXn7gXJMFTVnvhyyuNNpRVaH4yRVcaqkJU2hStQkCoVVp81/PO5PteeMYBG8Av3vVn3Lv7Yzi3gSg5qEFEMcaRFXupJFsI2lhFGMqsOI9URq+BURT+w2hwhq+7Ffo8NMEwyxnHvZ6rznzXBDAmoHFoxlmbnirf8vJfYbo+Q5Yv4OwMkTmC+p6JPww4xurtWBWh4PIVVnHMz+9fzKv7u+5AVsaTzx2fMuTuHV7/5oNfwUhOrg4kheBRiUTNkDTBxckaXnF7IIFYGIwFnIFQ5VVXns4fvOk5B3zzbnzoL/T2h/4Zk4AvOmHIki7GMIWRKnl4HOemR+biVmNquj2FUcDRO39l9WtplcOHOc45+Ru4+tzfmUy2CWgc2nFe/WnyPS/4IzYmx1GEPQgVUq0RpcA6RbTs3A6hKCevpHiVgRDPMP6mjmJev2ZFZzEthav6uayGh6u6K6yWXh8TQNpHIYHQ1iKwJiAmJ+IREkxwTPsTePpZrxt7wf3SP1xHTFNUImItQo7YshlMSCEYgptUT2moYbAQMiyh7L8wHpUc1YLochIRYpFCrlx1bp2/e8eBV0o9sOczet2d78QnD6NxA8bExQZOxaHkIBlGqkTfLwJmBuZm7JJmHZYgX+wr6mamVTOUHmRJP7yrt6Nb66YrzNv5HCMVgge0glBF8Si+HW4LnHfCj3DVmb83AYwJaKzNOGP6SvmuF/0Z25JTifkChZbehfdV1ARsJSdxdaJPKHyDSmrHvu3jd50PkiEO+v0Hfn4hkCZ1XKLkRSTPSw2SovAYo1x5wYE18/3XtTeixaQ6aqVhbU6MviSFJIXCIaGG0xmSUMdkniKpgqty9raUz/7GKw/YAO7zd+r/fOmHKfIKRmpYm61ioyEsr/7IimHP8efnMsUcfd5pCIpLPeLmCLGFlSmUDCOW07Z8C1ee/esTwJiAxtqOM2cukp991bVy5ranEWIDV6lShN0glqJIyUODpOpITI2i2DWCy0mGGv0BrfJh4SoZT79jWH/H0q5t+LTo6bgVocgb+BBBLGl1hojFOkjNFp5zwY+Oveje+c836eNNg7ETbqmVRgwNNAZsbRoAlzo0zwmFJyCkyQzEjJkq/PNPv/CgPuuDX/ghGuFBop1H/DTR93GvqaAjDbYsMy+H9WeMVptclmOqy3MuS3SHl+mW5eEGk5Q8WcEL1imRJhK3cObW1/Ps8989AYwJaBy+8ZYX/rOcffxFZAvzVJI6MXqMMWUHeXMe4xpYKiObigaTyoNkiD3hqu7ej8XFJcvv7IZWt8gqdnVLi9zEKpF5NFZLDyO0yPOIiQVXnPeNB3Tvfv/fPo2YlDjJWazC03DgKoSiACnwxV5cJeCmhWibtPKC7XXHB37lGi47bdsB39CP3voT+uj+L+Aq04g4QghYnR66GdEuEsDevh0ZEU7tmlOspndoyPweAR4dwOpm0e253ugwUsOZjWhMMDHl3OO/g6edO/EwJqBxBMaPPv/f5OwtV5MWNQwZwWcYY6hUk1LRLtZ6Jvyw8r9hC6jblvbkO/pc/cEFONydX045sNNYNWqHqKGCc2U9PtYRYotaAnVzIi869y1jL7z/++E7dK5VapdbOyEkXGl4EfCKMxZ8iyQRvArFQg6pY9rl/M3bXsYzTttywEbwC/f+nt50319RSbfR9PuJarEmLUkHBaIM5tA6uYoDMTndlU+dfEfHMxgmHTBKO7x/fg9jSNA40ybTLLAyzcWn/iRPPfeXJ4AxAY0jN/7Pi98r22sXkBRVKsbh/V58UCyb0Nh3i4eJHcnw0FU/y22U1VKsy6rCVgPKgSPDCxBCBZUGwYOzggmWK8/55gO6X7/+z9dTMRB9TvDZZAKtNNqFAr6Z4WxKEVJEpzGkpFmD3/nhF/P8izcesBG8Zcd79bpbf4ukalloPY7FYpMWAU8Rm0thTmFoWGnQqA/XaxlWZTW0uXUgLGsY2dza74H3NNa2aW7sfFk8oIErzvlJLjntRyeAMQGNdRCqesV75ennvY7YciRumhgVTwOMGyEw06cxPkCBPlzjOC6rSb7c+4c3PJWLevmch5IBBmtNmZD1ypQ5gxc86U1jL76//eSjes9jOcGDGINUksnkWWmoYsUgxiGVKgRBYsHxuot3venlfM8LTz1gI/jo/Kf1Uzf+NqYaiTFSr1aIhYKv4uMcYpOeZr24LI1Hb+5jWFR0eEhqxHmG5vSWZ0bo/H23Zx58inMpV5/3R5x33PdNAGMCGutnfMMV75QrLvh6iB6kALUEWRi6GIaGqWS457C8PdFlHqMZmjMZ+/wSsU4pgkOMp5JUufDsFx3QPXrnez4K4si1QmJBvZ9MnJVGBPGllGrRyqBmcWaOn3vL1/N9LzjlgI3g3ux2/dB1byHnQWKYxnuPiSm1ZDNZK5KmG/ESegzySqyzgygxhEhzDHN0QPo1fd58mtR54aX/zrknvGYCGIdguMktOLTj9U/5eTlr9hz9z8/9DvvcDjSZot5qIXaGQkFswMeAlQqoB8kh2K5FFbu0CUyXm93tbcSBnWjnb0RkUc+jozPQSUaWDcNmyRINWXilvnfX54lCsIiNqHjU1kl1mpc+6cfGXoB/+vE79P7dOfgMV0/ISak2hSwJT2xHIkyBm8NKTsgt2BQkg+hxWiHVlIYrwICEKrVWzu+/+aV89/NPOSgj+IGv/Ch7zIPYkKAstBv2PCLzJKkheuX/Z+/OwyyrynuPf9ew9z6nqrqLHmkaGnqgmWeZQRFaAZVBBCVGjAMSZ8QBUXBIgjdEE3Mdcp0SMblXBS44GxRFRSJEUUBAQERBFASZeqw6Z++91nrzx64equp0Nz1BD+/neXgeuymraNh7/c67hnc5xtzCJ3bUfd+YhGPVlvpp1DPV83li5bbZ8Rc0WcYeT7WuoKqHcU4wNLumrPErvr+NOckMIzZhXNPqREKGYxrtYoAzDrtZw0Irjc3bkfNfaU5fcDGTs7nYqkvlB1kWFpPsMCShcBmpLjGknk3cnsrtfBv3P71d46dF6wq6VQdjMnwoOWS3V63XT/zYl24iSqKVZYQyQO0pfWebf15yanyISHBg22DaYNsYP5FAP8NeyM0wiMPIMJ96x3M5+7hdNmggvOInp8qiJQ+NtJ4ffxXq+Omg1fy4ntNH61YprK3jQVl2sNZirW9OpafmZHpKlhgNYjtEEsZ4qlKQaiJGaiYNzNbA0NDYchw840Xm1Sd+jElxVyrp4Io+nMtIUpJCSe6bu5S9a6/1Zeq9O2r0tZbjznf0mB0Yf77D9pjOGv8SRyKS9WGomCA7cPzub1r3q1x/+Bu5+xEHLiMhGNvGGYe4Sl9C6YKzJDJMlkOoICSk2+zEwyUqBuhPy/j0WxbwqufO2aCB8Lu3vVme6NxCt36Ywg2M3i47Zsvs6E0aZsOHldWc81jT821dU1nE4IjBYPAYPM5mTUv4FHA2J9YZmW2Te9h1xl9y0kHXaGBsAmZDb71Sa/cP3z9DHnn8PmrzBC6zxLo575BSjZUWxvYYOFeZgjK4nlfIjr1isxmAxn8Pgxv9a1nd92Hc91sxveXb5Gkxp+33YQ6Z+/J1fhnnv+Yy+d1Sh9QdMDVW+km2wtoaSdv2LKlIAOeblq7OYEOJM4LJCqpOxPgWRob59FsW8NfPn7tBA+EP7rpQ7nrwCrARTCQGi3PSa2To9T9HnpfVjBmjplYZ80ymcc/ailZTPX7I6K8LIJ4UDc45jE2kWGJMszbofE4IQuYcFthn5zfyrNkXamBopbHles/zrzJHzDuBVtgOiX2krCTYSJIcl1Wrb9286j70HjtJerU2H7W7pcfNZcsrjnUa1FzCScXUbK/1CowvXP8Huf+JgMQOxkawOYkEBKh19xRZCyLY3MPwEnxmqetEVUboa9NOFZe+/XkbHBjX//piuf2BSzFZTh1dc6GSk96t78du73tKD8rKXVajPnSscs5jVINCVtlyO+Znjb0kKSXBmObDU0wdxNYYH3FemutZvZC7NkfsfokGhobG1uHMQy8xpxx+Ia40ZOSkWGL9Uspy5cnanmX62HMdPV7Ksa3Nx7VUWDWUegTH2m4ONHhMmThqrzes15/9kkv/G9wySB4fq+behyJho8foIwhliROLGQ741gB1yCGfBOJw1RCfOO+4DZ6SuuOhy+T2B/6VojWhWSNwiSp2SZRjnhXzFNYhzFMLjzHP6qrnPMbeDjkqrMYWMK7ZbmytJcaASMQ5Q4qGEDKsDUzun8/zDvgCu21/lgaGhsbW45hdzzRvPPkzTJYpFHWBl+3xrWrU8Dy+iuh9tqN3m4aVL/3YQ4FPZc1jfPWyfNZBmDXxMA6bs+6N8D573QNy/+JATIB1WFNgrYdUk0KFOH3HrTWYJOQ+IyTb7A5KNdPNQj557gs3eNH7loe/LD+87WLwGVUZcDhCWkyrGCTWvVt4jG1ZPv75M+O6ya7xTg3pUQmvtifVaE2zxgjWjFRHGTF4usMZmZ3OztOO5tSDrjfb9x+lD5OGxtZn9+2ONn9zxg1mrx2fQx0epY5j22iM753T80V8Wh6DkZdaAgsO+ev1+s4f//KPCQyBm4YLS6hpkUIiKwUz4EhRu9wmI4hzlJLAAV6YOiB86M2n8MbjdtyggfDe+38q1995AZKVVFXVLKybipYbpDs0jHPTWOs5n9VUFrKi7b+s8Vka1f5jbTNcvQ4GmpGPRxKby6bEk0JGf2t79ph/OMfvfZWGhYbG1u/1z/2sedF+5zHQyUmuadWQmTYiQnCBmDnqRLMGMLKfXYhY19yU5/B404LoV04FJDPqZW+umzXjKo5VT3xHmv5BMXkEjzOCTxZnliHklCax/8RT2WPKc9b5xfzcD+6V+/5cYMgxsozkc8SWGC8E76C2mG2g0jDBgjjEeKQWJBgcGRIjYiMmWZKpSKYGUzK9gG/83V9wzgnzNmxK6pGvyLd/c9rIFlXBeEc0EbGOOgnGZySzZMXUpqwhHCTZpiXOSIiYkb9I9Fz87hUGy6vcKKv8hSDWEe0QLm8Wt1MCZyyODEKBpAxsRogZ2DZ1MAy2d+CEAz7CETM/q4HxdD/PunvqmXXvwl/IF659F8NpmDItIfMWkzxIB5NDndpIXWHtyEEma0AiIkJd17RaLdIqp61WPdy38vdWv9sFwJlEChlGLNYlhC7eTqCuI5lv8faTLmX7/IB1fjlnvfLz8qcnBCm28d1RoYSsH+pI0fIQu5R1DVkbC5hUE+1EoGSmN3zzI8fxrNkzN2gwvPPhK+V7t7wb4wdILOv5XPR8JIxh3OE66f0smRW/17N+6vHZtPfPdy4nBodhGOtKvOsnRiHEDlnmiDFijGvuwMCw0+TjedH+l2tYaKWxbZo/6WDz9y+9zuw+/QBaThATqVMX41qkqoWEIfI8x7kMsNR1aM53eI/zMtIXavwnunGfFHv2KGnmmgMRZ8HbRIgR8oLhIGQ2cPCcF6xXYHzwqjvkoaUtpNBHjKKFTRWFDVTlEKVEaDUdZCUYIi2QDofuMJXrP3XyBgfGL//wVbn2l+9FskBieOVzkZ56RbD6vz9691Pze2N2O8lqdmStZrhJdY2RGusLEp4yLiOKYGwfCYuxEYmBwg5w+Ly/08DQSkMt9927PiY/uOtzLDMVxgxiyi79NjJEwOCwtln/kJiaxVPbTDCR8tVWESs/Pa6+8kg24CpPkQdqcZTkZLZmkkzg/af9Yr1e0P6zrpThhUuhnWPitt1fSqJQ2KYcLG0BSTA+IVUEOxFkKUfuMsgNnzh9gwfD2x/6qlx727uIVnDWk6TGMn6NYO1TSuOfn1FngFY9GzTue8lqKhh6VhxGapIRrHWkaDHGNbujpATbJU/T2G5gFsfs9kmmDe6tgaGVhlruxL3OM69b8AV2n3wIVIvwhWFIEt4anIUUAhaDy3LqKMSUIVL07p676n3J4+7vMCsWMZvfyMEaYmxOf1tnSHWXw/b8q/X6c7zrMzfJ8NIaCiDpHd/91lImS2nypkOtzzHBYm0GDHPOgnkbJTDuePDrcu1t7yTaGpdBt+oiZD0rzLXdwdJrN9T4nU8jvy8y/iKmUbur1lLBOIMxHmtaeAuGLqSEkxbeeHab8VLOOOSHRgNDKw21Bt/71Sfl6l99hm6rxg4JrVZBCIEQAllRNGOxjLzYplrxCa/XJ8hVCwtje53+9ThKiDm4ihhrdhzcl/NP+NY6v6T3Pz4ke776KiBQFg6qgPHb9pqGqYSUZVgvmGoY4yYSgqXPPMabTz2Ij5x95AYPhr/60zflB798D920BNcSUmw+3TuTk9LqKovlHQLsU6hAVp7otrLmCmVcZUuv095pVOVjTUEdlmLFUfh+6vAkUybuzyHzPsDcaQs0LDYj2uV2M3X8Pm81O886WL78w4tYmg1RlR2Kog+RIeq6GrlW1o/ZxmhGdxddHiW9+getMkD4BMEkvHdkrsB2c150yNvW65/7HZ+7gdIYMmugFnxREOO23cU2tQqIJbasKHxGJw1TyDCfeNdLOPuYHTd4QLz1ka/KtbdciLGBvNWmqpc1XWDFk2wF5CP3aY8d0MdfECY92n2s/No0cskRI51tV1ehjFlTG/ulZuX3ayqVRJDFtPxMUlUhqcNBc97G4fP/VsNCKw21Pq74/YVy439fR5RlOF8TqUYOOEnz+o7bHSVrn7NeZc0jS45oA7iIdIX9dzqO1z7739b5hf3JrY/JsRd/h0AFtoXrBqIDs41Pgoqt8MkSUg4UTOlbyjf+8XUctRMbPCjee9/P5Cv3nYgPOxHjUlwWMClv+nktLwlW3ImxukBYc7VgelQcbsxB1LWtm62pt5QYj6UmxqVM6z+Aw+a/l7lTT9DA0NBQG+KPi2+Rq278MPctvoOUCTFGvBicJGAi0XSItmxOzZLjBIglzkRq8Svu4l7+gi9vKWKMwZCwFkIS2mkCHzlz/dpJH3vR9+W6u56AeqhpVxGfxMV+ktu6Kw1bJ3CWmEq8sYhtE5MFG8AmEAd0MQaO3mWQz11wCnvMaG3woHjdff8iP7/3481p6VU+MIz751tDQ8rmjhXT8wPFmsNl5deklJrW5ZJW7JyyGKwbmc4iNhc8+ZwgJUZybNYixKU4l+FjjdQT2W/2yzl6D60uNDTURvW93/yr/NevLmVZeJxaLBiPlSV428a4PuoqEqmwIy3Xk+3g0sqFT2OlORW8yic+kzKCdMkzw3PmvJFT9z9v3S9YuvZeOfeT19BN/RR5RtkZhnZ75CDY1r17SsRgjOBsRkqJFCrwOSD4NEwwBRNo8RfP2ZHPveM5G2VQ/O7tF8gvf38lJg/NPRujBvjeu5fMaoKj1+6q1VUFPYNDWk1wIBgbsTYBFkkOGflgYl1NTF2saYPtkFLCyTSSWcQO/QdzzIEXML11hAaGhobaJFXH0nvkB7/8FLc9eB3BJlyWU1WLSLFDfzFIDJY61eStNp1OJM/TyGIoWGubc+Crrn2kgsRSdpq4F+858T/X68Xd85wr5dePPkFhc8pocT4gdYuURcxW/oyJt1BVGMmbNaPCQL0Ib1sEWkyMFee+bF8uPuuQjTIoXn37uXLXH75BMBHjEkgxZoBf/b9v22Pr65qqjadScThbNesnZCMfTprKx1qPsTkhLcTLJEQc8CTtoo9QRgo3hYP3fAsHzHythoWGhno63LXox/K1H3yYP5R309eaTkxdUlpCZvsgOlIscQ4qHJBGFs9pFqZNGqk4EsYW0K141TEf56AdX7DOL/AHrvi5XPz/7oa2xVfDRNPC2Igpc2LRxYjbukMjdkfuGwlEcUSXQ6rIpcPghAlcfsFJHLfvdhtlYPzCj14ijyy5lazIiVJiaI074NkM9KN+NSpIDGs5NzHmnpa17a6ysup23IRxFmshpUiSCmv7CHWXzLWxMWKlyx6zXs6CvT6mYaGhoZ4J37/vUrn+1v9gSfVnyHO6aRneZpg6w1GSbL5iYbK53rOZoho5K0iMNYfNfDFnHfVP6/wS3/XoYnnuO7/KY0scxoCksrkfokxkeUYtyzDkW/lbVODjEHUUTNEHocLkfew7I+PyC09hjx38RhkcP/+jE2RheR/dEHHO4YyMVJBreodllXvhR37dY+qqV3j0nL7qsd4hoQ/jKjDdptLAY20OJFIKGBLOFliWMGvSc9lv5wuYO+1ZGhgaGuqZ9rWbL5Yb7v0yHRKSOQJDODuAqyLWWlJqFiqXL4qnkYN3k/w03nTipezQP3+dX+TTL/mOfPVnT2LSMpztIyzfqZNa4CqyVBLM1n3RUjtGhqUPWgaqYQiJ97zkQC45e+MMjPcvuUG+c+NFDIVHCHYJPm8xPDyMtxNXrCP06hnVOxh6B8fKr1vbae4elQmhWTMxprkoCdu0MydgCeQyQH97Bw6c81722+kkDQsNDbW5ufIXF8mNv/4mMTeUZpgsuRWhAc2VmTE2C+N51uLk+W/huP3OXueX+erbH5ZTPvg9YvRkeUVdAlJTtFqUwUPq0rJQbuV7bqW2bNcOLKpgu4GcT772cM5asOtGGRzvffJq+fZNHyDaRXQ6HXzeR52eoJVPJgaDpBxrOmsc9McHgKyYpuptzJbbNRwMBJqbBqumj5bPBGs6pBDxdpC+fDL77XwGh8w7X8NCQ0Ntzu59/Ca55tbPcP/Cn1GZugmN2AwVxjhiELIsY9KkKXxwwQ/Wb/H7tZfJrxdCO3bpSIJ2H354aXMArGiDtfiqJLqtfE3DALVwxOwJ/MeFpzN/B7NRBsjb/3SFfPen76dqd0n1AN5XGNfFxn6qWI00rOzipG+N1UIyYzvVjl/j6BUIaw6OtMqfP0HM8TbDmiFiqJmQz2OvnV/BkfPfpGGhoaG2JH965Hdy5a8v4cGHbyQKVN7QxZG7mv5heNupX2OniXus84v94S/dLO+54h7IhptW7luyWGJ9c59ICgl8BiE2i8kpkhlLLQnJfNPSNdYUuacMBnxOXi/hjScfwMded9hGGyC/decFcsvvvohrOaj8uIXrUWsKYw7XWUa36kisXMjuWXmIh5hwttlJlaTGOYNxUMcKfI2TAUhF0yDTlM0/g1jAIyY1gRZLXD2F/ee+iufscZGGhYaG2pL99vFb5drbPs49j99Cx5a0XOKYOa/htAMvXOeX++5HKnneOy7jyWVdurbAbOHDQ+Y8VbfCZivXXpKEkd1mMrLbLMelZtdZxICLFN0hpg9O4qPnPY+XHjpzo/1buOyGs+WPi35CwFPWw+SZXesU0qrsmGpAjFttaIzETnPCW2LTENNlxCSEaLBZjhdDSEMYE7HWE0cuB7PWI8mSI4QK9plzOsfv/RENCw0NtTW568lb5Npb/oFuZynvPvk76/WCn/r3P5Rv/vzPzVmEYiIxbeFXtqYCSTVZ5qlDt5m6WT6jZhxYR06JBKGOnqzoo+4OceIB0/jOh07eaIPkg0tvl2tuvpBHl/6eMi4jK0BSjmF1J+qXTyO5cVNGo/ZLmfG7n0YNAinhM4vEQEw1zjmM9dSx6VbrUiLLMmLqIHTJfJtQZ6QU6J9gOXDaezlqjzdoWGhoKDXeN3/1hJz6vm/gXYsQBNIwxm/Zu6MkeYxpGnqnFPC5I4pBYmqm/k2EJHjXHIQcyHPe9/LDeOdLdtloA+U9j/+nfPeWi1g0PIz4iPeWuhwiczmYbI237o0+0T0+ONLY6agx38thCCE0F3s5R13XRATvXRMgCaoy4W2Od4lYL2RCMZN957yGI3d9m4aFhoZSqzf39ZfL/Y8twsQCKzniA1v6MyQ0J6VTqLDeN23nqwh5jvE50EUqAzbnkFk5X3zfaey2vd1og+WP7/60/OK3n2RZPYwUgrE5wjBecqz0EaVcc6UwUkmsXP9e+yVLq7bI98YSQsKQgXXNTRpWwATKsou4SDsvSBVMae/FIfPPZt+dTtew2EZpa3T1lH3g8t/I/Q8P4QqBFKGuSJnHyBbekDBVJGPAGrAekwRyh7MQukMgbQaKine9ZD4fPOvgjTpYfvEnL5cHFt6IpEmQLcGYAVIqSdGQ+YxQC2bVt1TsuOBYEdrGjATH+Pbk46urlRVHiAGbOURqUuo06zgpYgT6c08V+9h56tHsP+cv2W1Qu89qpaGVhnoKbv4TcsSb/y8xlSSTcFgSLURqjN2yz2E4A1GajVGIAWfxpiYMLWFCu2D+znP4/DuP44Cd8402YP5u8S1yzc0f4LGhX4LpR+xSjPQ3A3rMsb5LqLoU2WRiGh5VUayp2hhbcTR/b/yax9j/rxAwdmQ9o86Qbh9T+mcza+buHL/PRzUolIaGWjfPOvebcsvvF4OPmFAhrtW0YQ+h+YS+JQsJsR6cbyooK5g4xIzBFi859UT+5fQdNuof8Ke/+5z86M6P0DEdkpmKlYQxNcZ0sClb0breWkOQgBU7PhzW0L58bGiAXeOJcWOypsqQQG4msMOUAzlg3qnsO/WlGhZqHJ2eUmv1/qufkN8++CDNpd8txFmIFVZyUswwdvNufe5TQmw/sQ64PBKkAltgK8EZSFkk1oAXjIn4OnLMPjvysXeeyN5T8406cF7+izfJnX/8Oi7vB/pBHsfQGplNykYa/wkYEARjHIamRYekSIoRaxze5CMt2d3I4U1PXZd4Kzi//OtaEByS1SRJI00E04qtxCEEfObw1RBT+/di9vQF7Df3xUzp213DQmmlodbPr55AXvj2S/njnxbjJk4npmaB1EiNsR4xOWzuW27F4g3UUSBrY8vF+LymkgwfCkJmwQ5DJ2f6hD7efdbevPOkvTfqwPmHRTfLt/77Qp7s/g5T9FPFZcRocDbDrLjjvUePKGNIREQixsrIgcPljQot3nskCNb6FZdqAU2F4pp2+EIJocDRj7eJGDpIsEyeMJudpu/Dvru8gV0G99KgUBoaasMdd+G35Me3PYjNBkhkpNjBZglJdbOY6nIMm/d4I8mC65JZj6kdlfdQd3HOEp3HhS6kPl589FSuevfGX+j9yX2fl+tv/TgpL+nUiURNUbQRKggFmHqNU07GCTEKhry5+xsQaoQKocZLiXMTILWpQwm+xntLip4YM7ypMKYDoWZCPoP5Mxaw1y4vZPbk52tQKJ2eUhvPP3/7Hrnh7j+Tsj6SNRBqjHMgEWxz6I0toNLIsoq666kLA7bElGDdBGJaCjLMvO0G+MT5z+eEvadv9EH0S//1Grnnie8RrYdoyIpB6joS4pKR8xVmZeiO7Iwad+d2LXhjsTaRUkWMTddi7wogIzJICgnrAtaCkwKbDCYOk7kORZzC7jufxu47ncquk4/WoFBaaahNY8ZffE0eLRciI8ejLRZDItZdXNEmJgPBYPzmvaYhwVG0I2UlkAqwgYyMaVmHs1+2H3/30oM3+kB6x2Pfkf/82d+ypLsQ2l0ktkjSJbOOFC3ONtNMZsUd4qtMT43ZHWWNH9lWu3L6yRJoVj0ixuXUdYUnYk1NbtpMbu/F3BnPZ4/ZC5jZv48GhdJKQ21aZ/7v6+TPSzqQO8gE6oCIIYkBlyF2pKmfdcBmvhCeGcpuC1yHLC+hazn5kAH+6dwzmTNp42/9+srNF8rtf/wyFQaKFlRTSfYxMiYQ6g7ttqHsGHxmMIRRe5oEwdDcqLhcRSIRwEQya3DGUtfNPdytrE3VWcSU/rnsvP1RzJn+XA6Y+QINCaWhoZ4+V970sHzl+gcwRY2kFtQVJIOznmSE5BypKmGkJ1G9uRerdY23S4i02GVixmc/uIDj9p250QfWe568Qa65+W/487J7EAYxLpBkGG8rTN0c3iuyNsNLhb6BSF2HkakoenStXRkcLnmKzCJSIlUgswNMKXZixqQ92X7aXHab/iK275+nQaGeFjo9pcaZecbl8nC1DHKDD55kbHNKGogpQtacZ/BekKpLcu3N+s8jrmB6EXnfSw/lrS/ebZMMrt+/4xL5yd3/TsgjYjNiqpp1CAHEYVzV3GdiaqwpqKs+shySXQqpOYcxbgF85MBFEXJ2nL4XO0w6kOkT9uPAWadoQCitNNTm4bjzvy0PJw8EXN1HcDVGHMvPlxnbtNDGQIwGnobAyCRSVxbjMoyvidLF+RypPE48tavwJhFjM2B76yCUgCXanA+dMMBF55y8SQbaBx67Ub506/ksG1qEZIK3llh3yKzHWU+oaqqsS+YKQox4O0BKibwVqeuK3BaElJrzIZJoZxOZOnEuMycfyMxJBzFpwhxmTdDtsEpDQ22G/vGbd8jP73uo2Qbq+jGxwpgC5Jlds6iw2FbCmERM/VB7oqkxribGBEWLMFyS+QJCSU0BFk7Yp49PnftC5k7v2ySD7td/9c9y8z1fos464BIxRGIUirwgJajqhM/a5PUwuU146WJThZM+JrVmMThlNlO325nJxW5M3m5H5k85QsNBaWioLcNvnqjlo5fdxDIKsEJm+6irpRjaK++WeMYEEm2oKly2hLw/o64LQghIK8FQSdYqqIMBgefvN4X3v2x/nr3PjE0yCN/zxLVy9U3/wGOdu6mNx3YT7bxNFIc3A0zu34F2MZVWPokpg9Ppq2cyaXAmgxNmscvgnhoMaoumaxoKgMPPu1Ju+t0Q4mswfdgQsFkkVBnGP7NdbFvRULq6mfo3HlMFvAi1cZi8D18PUxvD/OkZF738MF517PxNOjA//PidUrKQiMHaAeZM3l+DQGmlobYdF111s9z02wq8wxJIVkimJMUcn0XiM/y5okMbQoI8QqiR5DB5AfUwVEuYuf0AH3jFs3ntc2c9LYP3DlP31pBQWmmobdN//XaRHHv+ZVD3EftamO5icgKlb4NpQ1iIsc/s7iixBUYsUnWwmcNkbeKSxcyeEjnrlIO4+MyDdRBXSisNtand30Xe8JGvE2WQvDVM7AScK8hsRpk82MRmsKCBDUsxVhDf3B43tdXl9eccwYdO111FSmloqKfNJf/nau76cxubSipfY3NHqCzLyDEhIC7RtEN/ZqvR5FsQYWohvOUVB/HBl+r0kFIaGuppdeWtD8m/Xvc4zkLyCRPbCIxcLVpDxkbrXStSg82aA87GNBVMCiO35DmoI5hElvdRV805EJsbUl1jaLN9vpQ3/+VRvP90DQulnmm6prEN+u2jj8nhb7uGJVWHuspG30G9CUiyWIlkzhAkNRceZTnWeVLVAevw3hO6S8HXZO3tqBd32HtWH+e+4ij++pjZGhZKaaWhnil/9fc38MRwHPnUb9jU00/WQBJLmQzW+uY+jhhwKZA5R2kyQt3BFf3EOrHLILz79UdyzrFaWSiloaGeUe+74ib56W8jZM19DCEG2MSXKCWTwApIc+OcNRbrDfWKu68DSOQ5ewzy9tOfxcnPmqlhoZSGhnqmfee2R+XDl9+LZB3yOJEqLMVkAaTYpD/XSU0UA1mrCY5YjaxpCCYvOOngmZx76pE8b88BDQulNnO6prENmfLKL8qiZZYUhyB5TN6HS48SzYRN+nMleezIjXQigHNM7Yu87NBZnHvmsew+w2hYKKWVhtqcnPDeb8uTwwaky0RbsMQFSBXCpj+414oLqbLtEBHmzmjz1pP247xT9tCgUEpDQ22O/tflt8iP7vwTxvbjQmCJD+D6EBKxbmHMpu1i221vz5G7TeSC0/bnlIN30LBQagum01NbuR/d+agseN/Va/wawTRr4aEGk5rtryOXB2EsVkpSNJisBWKRUIIVLAaSIZOSyhaI9ZBqrBNsLBlsC89/9sFc9uZDNCiU0tBQm7v7Hlssz37Ll3i46lvj11mEGAVrLdZ6YhUhJby1zW3V1hBChc0hxhpcC+v6SfUwuApkIsZ0ESuQHPvOGuC80w7gtcfO1bBQSkNDbSkOetuX5NbfB4xd839jkyLG+qabbUpgDTYTTCqRGEgMgAdTRzJyMBWVVOAGIEyE7HEmt1qcdNBM3nHGgey/y3YaFkptpXRNYyt1zkevk1sfyKGIUK/lg4FxxCjgmpbjEoVUJhCHyzKs65C6ghhHlZV443DdNnnqsuvcAc4/+dm88nm7aVAopaGhtkSf/eG98m83PEIrdukmu/ZyMyug28XGQEakLEuKoiCKI4SSrGwhWUC8hZAzodXlrFPn8baXHcm8Qa9hodQ2RKentjI/v78jx37guwwtfBTX8sQumGzN7c0lCD5zpBRISHN621nodKHdgioy0VqO329nXvOiPXjhobM0KJTS0FBbg3lv/Jrc91CXLO9QJ8HXlriWYsBKwPo+6pCa4jMFnAn0mcBus3fi/FN34cxj9W5rpZROT21Vjr/wKnno4UVgM+qY4WOXkOUYWfMd34KnrqtmAZwO82fknHPy4bz4sHnMm55pWCilNDS2Nm/91PXyo3uXEJY3rbWGIIZ2FalzCEHA5U1n21ABqbk8w3kkDrHHToOceMBOvOK4fTl41wkaFEopDY2t1aU/fFD+/Zp7CTIIvoMjEOsS8oJOFKiEvLCQOlQBcB5HzZ7bW449cGfOfsFu7L/zdA0KpdRa6ZrGFu6XDyyWA9/6/8Fth5GKAbOEbqembk8CY3EhkSgRm4OF2dvBGUfO4RXP25cDZut5CqWUhsY2ZdrLPyNLO1AyANZDuZCWs6SsRVVHnCQO3GN7jpg/lbOO25tDd52oQaGUWm86PbUFO/gtl8nj5QAGA0SQGvoG6UrFnjtN4NXH7c67T9Hb75RSGhrbvNd9+ka5+aEOzpcYO8CEbIhj99uFkw+dz6sXzNegUEppaKjGxVfcLP/+7TuYv+M0nn3EfF48P+Pko/V+CqWUhobqYfKg4e7Pv4r5M1oaFEqpp5UuhCullHrKrP4rUEoppaGhlFJKQ0MppZSGhlJKKQ0NpZRSGhpKKaU0NJRSSikNDaWUUhoaSimlNDSUUkppaCillNLQUEoppaGhlFJKaWgopZTS0FBKKaWhoZRSSkNDKaWUhoZSSikNDaWUUkpDQymllIaGUkopDQ2llFIaGkoppTQ0lFJKaWgopZTS0FBKKaU0NJRSSmloKKWU0tBQSimloaGUUkpDQymllIaGUkoppaGhlFJKQ0MppZSGhlJKKQ0NpZRSGhpKKaU0NJRSSikNDaWUUhoaSimlNDSUUkppaCillNLQUEoppaGhlFJKQ0MppZTS0FBKKaWhoZRSSkNDKaWUhoZSSikNDaWUUhoaSimllIaGUkopDQ2llFIaGkoppTQ0lFJKaWgopZTS0FBKKaU0NJRSSmloKKWU0tBQSimloaGUUkpDQymllIaGUkoppaGhlFJKQ0MppZSGhlJKKQ0NpZRSGhpKKaU0NJRSSmloKKWUUhoaSimlNDSUUkppaCillNLQUEoppaGhlFJKQ0MppZTS0FBKKaWhoZRSSkNDKaWUhoZSSikNDaWUUhoaSimllIaGUkopDQ2llFIaGkoppTQ0lFJKaWgopZTS0FBKKaWhoZRSSmloKKWU0tBQSimloaGUUkpDQyml1NbifwYAExQuML/afxUAAAAASUVORK5CYII=";
                    //Se agregan los margenes de las paginas
                    doc.pageMargins = [20, 60, 20, 30];
                    //Se crean los estilos
                    doc.defaultStyle.fontSize = 10;
                    doc.defaultStyle.alignment = 'center';
                    doc.styles.tableHeader = {
                        fillColor: '#07ABDF',
                        color: 'white',
                        bold: true
                    }
                    doc.styles.tableHeader.fontSize = 10;
                    //Se crea el Header de todas las paginas
                    doc["header"] = function () {
                        return {
                            columns: [
                                {
                                    alignment: "right",
                                    image: logo,
                                    width: 30,
                                    margin: [0, -9, -110, 0]
                                },
                                {
                                    alignment: "center",
                                    //italics: true,
                                    text: "Semaforo Requerimientos de Personal",
                                    fontSize: 18,
                                    //margin: [12, 0]
                                },
                                //{
                                //    alignment: "right",
                                //    fontSize: 14,
                                //    text: ""
                                //}
                            ],
                            margin: 20
                        };
                    };
                    //Se crea el Footer de todas las paginas
                    doc["footer"] = function (page, pages) {
                        return {
                            columns: [
                                {
                                    alignment: "left",
                                    fontSize: 10,
                                    text: ["Fecha de Creación: ", { text: jsDate.toString() }]
                                },
                                {
                                    alignment: "right",
                                    fontSize: 10,
                                    text: [
                                        "Pagina ",
                                        { text: page.toString() },
                                        " de ",
                                        { text: pages.toString() }
                                    ]
                                }
                            ],
                            margin: 10
                        };
                    };
                    //Se crean los bordes del pdf
                    var objLayout = {};
                    objLayout["hLineWidth"] = function (i) {
                        return 0.3;
                    };
                    objLayout["vLineWidth"] = function (i) {
                        return 0.3;
                    };
                    objLayout["hLineColor"] = function (i) {
                        return "#aaa";
                    };
                    objLayout["vLineColor"] = function (i) {
                        return "#aaa";
                    };
                    objLayout["paddingLeft"] = function (i) {
                        return 2;
                    };
                    objLayout["paddingRight"] = function (i) {
                        return 2;
                    };
                    doc.content[0].layout = objLayout;
                },
                exportOptions: {
                    modifier: { page: 'current', title: 'current' },
                    columns: [1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13] //exportar de la primera a la 13 columna
                },
            },
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
    //************************************************

    
})




   