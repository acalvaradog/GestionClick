function Initialize(target, prop, value) {
    var Names = target.split(",");
    Names.forEach(function (item) { $("#" + item).prop(prop, value); });
}

function Message(value, value1) {
    $("#messageBtn").text(value1 == null ? "Aceptar" : value1);
    $("#messageText").html(value);
    $("#message").modal();
}


function AddRow(dialog, insert) {

    var field = ["", ""];
   
    if (dialog == "Addfc") { field[0] = "AddItems"; field[1] = "<tr><td><a href = '' onclick = '$(this).parent().parent().remove(); return false;'><img alt='Quitar' src='./Contents/image/Trash.png' title='Quitar'/><a/></td><td>" + $("#ListadoTiposInc option:selected").val() + "</td><td>" + $("#ListadoTiposInc option:selected").text() + "</td><td>" + $("#Archivo").val() + "</td>"; }
    if (dialog == "AddElemento") {
        

        var codigo = $("#EmpleadoId option:selected").val();

       



        try {
        var epp = $("#EPP option:selected").text(); 
        if (epp == "Seleccione...") {
            throw "Para continuar, Primero debe seleccionar el Elemento de Proteccion Personal.";
        }

        var motivo = $("#MotivoEntrega option:selected").text();
        if (motivo == "Seleccione...") {
            throw "Para continuar, Primero debe seleccionar el Motivo de Entrega.";
        }

        if ($("#Cantidad").val() == "" || $("#Cantidad").val() <= 0) {
            throw "Para continuar, Primero debe ingresar la cantidad.";
        }

        if ($("#EmpleadoId option:selected").text() == "Seleccione...") {
            throw "Para continuar, Primero debe seleccionar el Empleado.";
        }
        
        if ($("#Fecha").val() > $("#FechaFin").val()) {
            throw "La fecha de registro no puede ser mayor a la fecha de vencimiento.";
        }
       
      
    
        field[0] = "AddItems1"; field[1] = "<tr><td><a href = '' onclick = '$(this).parent().parent().remove(); return false;'><img alt='Quitar' src='./Contents/image/Trash.png' title='Quitar'/><a/></td><td>" + $("#NumeroEntrega").val() + "</td><td>" + $("#Fecha").val() + "</td><td>" + $("#FechaFin").val() + "</td><td>" + $("#EPP option:selected").val() + "</td><td>" + $("#EPP option:selected").text() + "</td><td>" + $("#MotivoEntrega option:selected").text() + "</td><td>" + $("#Cantidad").val() + "</td><td>" + $("#EmpleadoId option:selected").text() + "</td><td>" + $("#Observacion").val() + "</td><td style='display:none'>" + $("#EmpleadoId option:selected").val() + "</td><td></td>";
        
    
        }
        catch (err) { alertify.alert(err); }
        
        
        }
  

    if (insert == true) { $("#" + field[0]).append(field[1]); }
    if (insert == false) { $("#" + field[0]).html(field[1]); }
    return false;
}



function Save() {
    var model;
    var ListadoAdjuntos = [];
    var fila = [];
    var requis = [];
    var i = 0;
    var e = 0;
    var fila="";

    try{

        $("#AddItems tr").each(function (index, item, array) {
            ListadoAdjuntos.push({ "TipoIncapacidad": item.cells[1].innerText, "Adjunto": item.cells[3].innerText })
        });
        
       

        //reqs.each(function (index, item, array) {e = e + 1; requis += (requis == "" ? "" : String.fromCharCode(10)) + item.cells[1].innerText });

        model = {"EmpleadoId": $("#EmpleadoId").val(), "Fecha": $("#Fecha").val(), "FechaInicio": $("#FechaInicio").val(),
            "FechaFin": $("#FechaFin").val(), "CantidadDias": $("#CantidadDias").val(), "Diagnostico": $("#Diagnostico").val(),
            "Area": $("#Area").val(), "ListadoAdjuntos": ListadoAdjuntos
        };


        $.ajax({
            data: '{model:'  + JSON.stringify (model) +  '}' , 
            url: "Create",
            type: "post",
            contentType: "application/json; charset=utf-8",
            success: function (response) { location.replace('Index') }
           
        });

    }
    catch (err) { Message(err); }

    
}

function Save1() {

    var Param;
    var requis = [];

    if ($("#Req1").prop('checked')) { requis.push({ "resq": $("#Req1").val() }) }
    if ($("#Req2").prop('checked')) { requis.push({ "resq": $("#Req2").val() }) }
    if ($("#Req3").prop('checked')) { requis.push({ "resq": $("#Req3").val() }) }
    if ($("#Req4").prop('checked')) { requis.push({ "resq": $("#Req4").val() }) }
    if ($("#Req5").prop('checked')) { requis.push({ "resq": $("#Req5").val() }) }
    if ($("#Req6").prop('checked')) { requis.push({ "resq": $("#Req6").val() }) }

    Param = {
        "Usuario": 1, "FechaAct": $("#Areaplan").val(), "Observaciones": $("#ObsValida").val(),
        "Jefe": $("#AddjefeValida option:selected").val(), "Requisitos": requis
    };


    if (Param.Cargo == "")
        throw "Para continuar, Primero debe ingresar un cargo.";

    if (Param.Area == "")
        throw "Para continuar, Primero debe seleccionar un area";

    if (Param.Descripcion == "")
        throw "Para continuar, Primero debe ingresar la Descripci\u00f3nn.";

    if (Param.Jefe == "")
        throw "Para continuar, Primero debe seleccionar el Jefe.";

    $.ajax({
        data: '{Param:' + JSON.stringify(Param) + '}',
        url: "GrabarPlantillas",
        type: "post",
        contentType: "application/json; charset=utf-8",
        beforeSend: function () { $("#processState").modal("show"); },
        success: function (response) {
            $("#processState").modal("hide");

            if (response == false) {
                Message("Se presento un error al guardar los datos");
            }
            else {
                $("#message").on("hidden.bs.modal", function () { location.reload(true); });
                Message("Los datos fueron guardados de manera correcta");
            }
        },
        error: function (xhr, status, error) { Message(error); $("#processState").modal("hide"); }
    });
}




function SaveEPP() {
    var model;
    var ListadoElementos = [];
    var fila = [];
    var requis = [];
    var i = 0;
    var e = 0;
    var fila = "";

    try {
        //model = {
        //    "NumeroEntrega": $("#NumeroEntrega").val(), "Fecha": $("#Fecha").val(), "EmpleadoId": $("#EmpleadoId").val(),
        //    "ListadoElementos": []
        //};


        $("#AddItems1 tr").each(function (index, item, array) {
            ListadoElementos.push({ "NumeroEntrega": item.cells[1].innerText, "Fecha": item.cells[2].innerText, "FechaFin": item.cells[3].innerText, "EPP": item.cells[4].innerText, "MotivoEntrega": item.cells[6].innerText, "Cantidad": item.cells[7].innerText, "EmpleadoId": item.cells[10].innerText, "FechaFirma": item.cells[11].innerText, "Observacion": item.cells[9].innerText })
        });



        //reqs.each(function (index, item, array) {e = e + 1; requis += (requis == "" ? "" : String.fromCharCode(10)) + item.cells[1].innerText });

       

        var datosenviar = JSON.stringify(ListadoElementos);
        $.ajax({
            data: datosenviar,
            url: "RegistrarEntrega",
            type: "post",
            contentType: "application/json; charset=utf-8",
            success: function (e) {

                $("#mensaje").val(e);
                $("#processState").modal("show");

            },
                 
            error: function (xhr, status, error) { alert(error); }    

        });

    }
    catch (err) { Message(err); alert(err); }
    

}




function Search(source){
    

    var fields = ""; var objects = ""; var view = ""; var value = "";



    try {
        if (source == "SearchPlantilla") { fields = "Tabcargo,Tabarea,Tabjefe"; objects = "Tabcargo,Tabarea,Tabjefe,SearchPlantilla"; view = "SearchPlanTblview";  }

        if (fields == "" || objects == "" || view == "")
            throw "Los criterios de la consulta no son validos.";

        //var Names = fields.split(",");
        //Names.forEach(function (item, index, items) { value += (index == 0 ? "" : String.fromCharCode(9)) + $("#" + item).val(); });



        var Param = { "Cargo": $("#Tabcargo").val(), "Area": $("#Tabarea").val(), "Jefe": $("#Tabjefe option:selected").val() };


        //if (Param.Cargo == "" || Param.Area == "" || Param.Jefe == "")
        //    throw "Para continuar, Primero debe ingresar un parametro de busqueda.";


        //$.ajax({
        //    data: '{Param:'  + JSON.stringify (Param) +  '}',
        //    url: "BuscarPlantillas",
        //    type: "post",
        //    contentType: "application/json; charset=utf-8",
        //    dataType: "json",
        $.ajax({
            data: Param,
            url: "BuscarPlantilla",
            type: "post",
            beforeSend: function () {  },
            success: function (responsive) {
                $("#" + view).html(responsive);
            },
            error: function (xhr, status, error) { Message(error);  }
        });

    }
    catch (err) { Message(err); }
    return false;
}

function SelectRow(row) {
    $('#tablaprueba tbody tr').css("background-color", ""); $('#tablaprueba tbody tr').css("color", "");
    $(row.parentNode).prop("value", row.id); $(row).css("background-color", "#337AB7"); $(row).css("color", "#fff"); AddRowCommand(row.cells[0].innerHTML);
}


function AddRowCommand(source) {
    $("#menuOption").html("");
    $("#menuOption").prop("class", "hidden");
    if (source == "Default.aspx") { source = "<li><a href = '' onclick = 'return SaveOrCancel(true);'> Guardar </a></li><li><a href= '' onclick = 'return SaveOrCancel2(false);'> Cancelar</a></li><li><a href = ''  onclick = 'return Anular1(\"Default.aspx\");'>Anular documento</a></li><li><a href = ''  onclick = 'return ViewFirma(\"Default.aspx\");'>Firmar</a></li>    <li class = 'dropdown' ><a href = '' class = 'dropdown-toggle' data-bs-toggle = 'dropdown' role = 'button' aria-haspopup = 'true' aria-expanded ='true'> Vista Impresion <span class ='caret'></span></a><ul class = 'dropdown-menu'><li><a href='' onclick = 'return winOpen(); return false;'>Imprimir Autorizados</a></li><li><a href='' onclick = 'return winOpen2(); return false;'>Imprimir No Autorizados</a></li></ul></li> "; }


    if (source != null) { $("#menuOption").html(source); $("#menuOption").prop("class", "nav navbar-nav"); }
}



function View(site, mode, id)
{

}




function Guardar() {

    var button = document.getElementById('GuardarMT');
    button.setAttribute('disabled', 'disabled');

    try {

        var modo = $("#ModoTrabajo").val();
        var obs = $("#ObservacionModoTrabajo").val();
        var reqdes = $("#RequiereDesplazamiento :selected").val(); 

        if (modo == "") {
            throw 'Debe llenar el campo Modo de Trabajo antes de guardar.';
        }

        if (modo == "ALTERNANCIA" && obs == "") {
            throw 'Debe llenar el campo Horario de trabajo antes de guardar.';
        }


        var frmData = new FormData();
        frmData.append("Empleado.ModoTrabajo", document.getElementById("ModoTrabajo").value);
        frmData.append("Empleado.ObservacionModoTrabajo", document.getElementById("ObservacionModoTrabajo").value);
        frmData.append("Empleado.RequiereDesplazamiento", reqdes);

        $.ajax({
            url: "GuardarModoTrabajo",
            type: 'POST',
            contentType: 'application/json; charset=utf-8',
            dataType: 'json',
            data: frmData,
            //cache: false,
            contentType: false,
            processData: false,
            beforeSend: function () {
                $("#processState").modal("show");
                $("#modal_EditarModoT").modal("hide");
            },
            success: function (json) {
                if (json.respuesta != "") {
                    $("#processState").modal("hide");
                    if (json.isRedirect) {
                        
                        
                        setTimeout(function () { window.location.href = json.redirectUrl + window.location.search }, 2500);

                    }
                }
            },
            error: function (xhr, status, error)
        { button.removeAttribute('disabled', 'disabled'); alertify.alert(error); }
        });


    } catch (err) {
        button.removeAttribute('disabled', 'disabled'); alertify.alert(err);
    }

}


$(document).ready(function () {
    $.ajaxSetup({ cache: false });
    $("a[data-modal]").on("click", function (e) {
        openmodal(this.href);
        return false;
    });
    $('#modal_empleado').on('hidden.bs.modal', function () {
        $('#contentModal').html('');
    })
});

$(document).ready(function () {
    $.ajaxSetup({ cache: false });
    $("a[data-modal0]").on("click", function (e) {
        openmodal0(this.href);
        return false;
    });
    $('#modal_detalle').on('hidden.bs.modal', function () {
        $('#contentModal0').html('');
    })
});

$(document).ready(function () {
    $.ajaxSetup({ cache: false });
    $("a[data-modal1]").on("click", function (e) {
        openmodal1(this.href);
        return false;
    });
    $('#modal_Retiro').on('hidden.bs.modal', function () {
        $('#contentModal1').html('');
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

$(document).ready(function () {
    $.ajaxSetup({ cache: false });
    $("a[data-modal3]").on("click", function (e) {
        openmodal3(this.href);
        return false;
    });
    $('#modal_ModificarFechas').on('hidden.bs.modal', function () {
        $('#contentModal3').html('');
    })
});

$(document).ready(function () {
    $.ajaxSetup({ cache: false });
    $("a[data-modal4]").on("click", function (e) {
        openmodal4(this.href);
        return false;
    });
    $('#modal_EditarModoT').on('hidden.bs.modal', function () {
        $('#contentModal4').html('');
    })
});


$(document).ready(function () {
    $.ajaxSetup({ cache: false });
    $("a[data-modal5]").on("click", function (e) {
        openmodal5(this.href);
        return false;
    });
    $('#modal_Anexos').on('hidden.bs.modal', function () {
        $('#contentModal5').html('');
    })
});

$(document).ready(function () {
    $.ajaxSetup({ cache: false });
    $("a[data-modal6]").on("click", function (e) {
        openmodal6(this.href);
        return false;
    });
    $('#modal_Pruebas').on('hidden.bs.modal', function () {
        $('#contentModal6').html('');
    })
});

//$(document).ready(function () {
//    $.ajaxSetup({ cache: false });
//    $("a[data-modal7]").on("click", function (e) {
//        openmodal7(this.href);
//        return false;
//    });
//    $('#modal_Implicados').on('hidden.bs.modal', function () {
//        $('#contentModal7').html('');
//    })
//});

//$(document).ready(function () {
//    $.ajaxSetup({ cache: false });
//    $("a[data-modal8]").on("click", function (e) {
//        openmodal8(this.href);
//        return false;
//    });
//    $('#modal_RespuestaJ').on('hidden.bs.modal', function () {
//        $('#contentModal8').html('');
//    })
//});

$(document).ready(function () {
    $.ajaxSetup({ cache: false });
    $("a[data-modal9]").on("click", function (e) {
        openmodal9(this.href);
        return false;
    });
    $('#modal_Gestion').on('hidden.bs.modal', function () {
        $('#contentModal9').html('');
    })
});


$(document).ready(function () {
    $.ajaxSetup({ cache: false });
    $("a[data-modal10]").on("click", function (e) {
        openmodal10(this.href);
        return false;
    });
    $('#modal_EliminarResp').on('hidden.bs.modal', function () {
        $('#contentModal10').html('');
    })
});

$(document).ready(function () {
    $.ajaxSetup({ cache: false });
    $("a[data-modal11]").on("click", function (e) {
        openmodal11(this.href);
        return false;
    });
    $('#modal_EditarRetiros').on('hidden.bs.modal', function () {
        $('#contentModal11').html('');
    })
});

$(document).ready(function () {
    $.ajaxSetup({ cache: false });
    $("a[data-modal12]").on("click", function (e) {
        openmodal12(this.href);
        return false;
    });
    $('#modal_Pretencion').on('hidden.bs.modal', function () {
        $('#contentModal12').html('');
    })
});

$(document).ready(function () {
    $.ajaxSetup({ cache: false });
    $("a[data-modal13]").on("click", function (e) {
        openmodal13(this.href);
        return false;
    });
    $('#modal_Fundamento').on('hidden.bs.modal', function () {
        $('#contentModal13').html('');
    })
});

$(document).ready(function () {
    $.ajaxSetup({ cache: false });
    $("a[modal140]").on("click", function (e) {
        openmodal140(this.href);
        return false;
    });
    $('#modal_Aut_Registro_Vac').on('hidden.bs.modal', function () {
        $('#contentModal14').html('');
    })
});

$(document).ready(function () {
    $.ajaxSetup({ cache: false });
    $("a[data_modal102]").on("click", function (e) {
        openmodal102(this.href);
        return false;
    });
    $('#modal_DetVac').on('hidden.bs.modal', function () {
        $('#contentModal102').html('');
    })
});



//$(document).ready(function () {
//    $.ajaxSetup({ cache: false });
//    $("a[data-modal5]").on("click", function (e) {
//        openmodal5(this.href);
//        return false;
//    });
//    $('#modal_DetallePS').on('hidden.bs.modal', function () {
//        $('#contentModal5').html('');
//    })
//});



/////////////////////////////////////////////////////////////////////////////////////////

function openmodal(url) {
    $('#contentModal').load(url, function () {
        $('#modal_empleado').modal('show');
        //bindForm(this);




    });
}

function openmodal0(url) {
    $('#contentModal0').load(url, function () {
        $('#modal_detalle').modal('show');
        //bindForm(this);

    });
}

function openmodal1(url) {
    $('#contentModal1').load(url, function () {
        $('#modal_Retiro').modal('show');

    });
}

function openmodal2(url) {
    $('#contentModal2').load(url, function () {
        $('#modal_Pendienteporfirma').modal('show');
        //bindForm(this);
    });
}

function openmodal3(url) {
    $('#contentModal3').load(url, function () {
        $('#modal_ModificarFechas').modal('show');
        //bindForm(this);
    });
}

function openmodal4(url) {
    $('#contentModal4').load(url, function () {
        $('#modal_EditarModoT').modal('show');
        //bindForm(this);
    });
}

function openmodal5(url) {
    $('#contentModal5').load(url, function () {
        $('#modal_Anexos').modal('show');
        //bindForm(this);
    });
}

function openmodal6(url) {
    $('#contentModal6').load(url, function () {
        $('#modal_Pruebas').modal('show');
        //bindForm(this);
    });
}

//function openmodal7(url) {
//    $('#contentModal7').load(url, function () {
//        $('#modal_Implicados').modal({
//            keyboard: true
//        }, 'show');
//        //bindForm(this);
//    });
//}

//function openmodal8(url) {
//    $('#contentModal8').load(url, function () {
//        $('#modal_RespuestaJ').modal({
//            keyboard: true
//        }, 'show');
//        //bindForm(this);
//    });
//}

function openmodal9(url) {
    $('#contentModal9').load(url, function () {
        $('#modal_Gestion').modal('show');
        //bindForm(this);
    });
}


function openmodal10(url) {
    $('#contentModal10').load(url, function () {
        $('#modal_EliminarResp').modal('show');
        //bindForm(this);
    });
}

function openmodal11(url) {
    $('#contentModal11').load(url, function () {
        $('#modal_EditarRetiros').modal('show');
        //bindForm(this);
    });
}

function openmodal12(url) {
    $('#contentModal12').load(url, function () {
        $('#modal_Pretencion').modal('show');
        //bindForm(this);
    });
}

function openmodal13(url) {
    $('#contentModal13').load(url, function () {
        $('#modal_Fundamento').modal('show');
        //bindForm(this);
    });
}

function openmodal140(url) {
    $('#contentModal14').load(url, function () {
        $('#modal_Aut_Registro_Vac').modal('show');
        //bindForm(this);
    });
}


//********** certificado  **************
$(document).ready(function () {
    $.ajaxSetup({ cache: false });
    $("a[data-modal20]").on("click", function (e) {
        openmodal20(this.href);
        return false;
    });
    $('#modal_certificado').on('hidden.bs.modal', function () {
        $('#contentModal20').html('');
    })
});


function openmodal20(url) {
    $('#contentModal20').load(url, function () {
        $('#modal_certificado').modal('show');
        //bindForm(this);

    });

}

//*******************************************




