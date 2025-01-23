$(document).on('show.bs.modal', '.modal', function () {
    var cantidadmodal = $('.modal:visible').length;
    var zIndex = 1040 + (10 * cantidadmodal);

    $(this).css('z-index', zIndex);
    setTimeout(function () {
        $('.modal-backdrop').not('.modal-stack').css('z-index', zIndex - 1).addClass('modal-stack');
    }, 0);
});
$(document).on('hidden.bs.modal', function () {
    var cantidadmodal = $('.modal:visible').length;
    if (cantidadmodal >= 1)
    {
        $('body').addClass('modal-open')
    }
});

$(document).ready(function () {
    $("#FechaRegistro").prop('disabled', true);
    $("#Emp").prop('disabled', true);
    var arch = 0;
    var contp = 0;
    $("#add").click(function () {
        contp++;
        arch++;
        var fila = '<tr class ="selected" id="fila' + contp + '"><td>' + contp + '</td> <td><select  name="select" class="btn btn-ligth dropdown-toggle"  id="TipoPrueba' + contp + '"><option selected >Seleccione...</option>  <option value="documental" >Documental</option>' +
            '<option value="testimonial">Testimonial</option><option value="Otro">Otro</option></select></td><td><input type="file" class="form-control-file" name="archivo' + arch + '" onchange="comprobar(this)"  id="Prueba' + contp + '"></td><td hidden><span  id="archivo' + arch + '" class="as" >0</span>  </td> <td> <textarea id="Descripcion' + contp + '" style=" border: 2px solid #ffffff;" placeholder="Descripcion del documento"></textarea></td></tr>';


        $("#pruebat").append(fila);
        return false;
    });

    var conta = 0;
    var arch2 = 0;

    /************** Funcion que se encarga de agregar los anexos **************************/
    $("#adda").click(function () {

        conta++;
        arch2++;

        var fila = '<tr class ="selected" id="fila' + conta + '"><td>' + conta + '</td><td><input type="file" class="form-control-file" id="anexo' + conta + '"name="anexos' + arch2 + '" onchange="comprobar2(this)"></td><td hidden><span  id="anexos' + arch2 + '" class="as" >0</span>  </td></tr>';


        $("#Tanexos").append(fila);
        return false;
    });

    /**
     * Funcion para eliminar la ultima columna de la tabla.
     * Si unicamente queda una columna, esta no sera eliminada
     */
    $("#del").click(function () {
        var num = 1;
        // Obtenemos el total de columnas (tr) del id "tabla"
        var trs = $("#bodyprueba tr").length;
        if (trs >= 1) {

            // Eliminamos el valor la ultima columna
            var pesoarch = $("#bodyprueba tr:last").find('td:eq(3)').text()
            restar_total(pesoarch, num);

            // Eliminamos la ultima columna    
            $("#bodyprueba tr:last").remove();
            contp--;
            arch--;

        }
        return false;
    });

    $("#del2").click(function () {
        var num = 2;//Le damos un valor de identificacion de la tabla
        // Obtenemos el total de columnas (tr) del id "tabla"
        var trs = $("#bodyanexos tr").length;
        if (trs >= 1) {

            // Eliminamos el valor la ultima columna
            var pesoarch = $("#bodyanexos tr:last").find('td:eq(2)').text()
            restar_total(pesoarch, num);

            // Eliminamos la ultima columna
            $("#bodyanexos tr:last").remove();
            conta--;
            arch2--;
        }
        return false;
    });
});

function AddRow2(dialog, insert) {
    var field = ["", ""];
    var count = 0;
    var tabla = [];
    if (dialog == "Addemp") {
        try {
            var em = $("#EmpleadoRegistraId option:selected").text();
            if (em == "Seleccione...") {
                throw "Para continuar, Primero debe seleccionar el Empleado Implicado.";
            }
            $("#AddItemsemp tr").each(function (index, item, array) {
                tabla.push([item.cells[2].innerText]);
            });
            for (i = 0; i < tabla.length; i++) {
                if (em == tabla[i]) {
                    throw "No es posible asignar el mismo proceso dos veces al mismo empleado";
                }
            }

            var nombre = $("#EmpleadoRegistraId  option:selected").text();
            var valor = $("#EmpleadoRegistraId  option:selected").val();
            var arreglo = valor.split("-");

            field[0] = "AddItemsemp"; field[1] = "<tr><td><a href = '' onclick = '$(this).parent().parent().remove(); return false;'><img alt='Quitar' src='../Contents/image/Trash.png' title='Quitar'/><a/></td><td hidden>" + $("#EmpleadoRegistraId  option:selected").val() + "</td><td>" + $("#EmpleadoRegistraId  option:selected").text() + "</td><td hidden>" + arreglo[0] + "</td><td>" + arreglo[1] + "</td><td>" + arreglo[2] + "</td><td>" + arreglo[3] + "</td>";
        }
        catch (err) { alertify.alert(err); }
    }

    if (insert == true) { $("#" + field[0]).append(field[1]); }
    if (insert == false) { $("#" + field[0]).html(field[1]); }
    return false;


}


function MostrarJ()
{
    var inputJ = document.getElementById('Justificacion');
    var label = document.getElementById('JustificacionLb')
    var select = document.getElementById('Estado');

    var valorSeleccionado = select.value;
    if (valorSeleccionado != 'Anulado') {
        //ocultar input numero en caso de estar mostrandolo
        inputJ.hidden = true;
        label.hidden = true;
        //inputNumero.style.display = "none";

    } else {
        //mostrar Dias de suspencion
        inputJ.hidden = false;
        label.hidden = false;
        //inputNumero.style.display = "block";
    }
    return false;
}

function mostrarInput() {

    //seleccionando elementos
    var inputNumero = document.getElementById('Suspension');
    var select = document.getElementById('Sancion');
    var label = document.getElementById('Suspensionlb')

    //ocultar input fecha y numero
    //inputNumero.style.display = "none";

    var valorSeleccionado = select.value;
    if (valorSeleccionado != 'Suspension') {
        //ocultar input numero en caso de estar mostrandolo
        inputNumero.hidden = true;
        label.hidden = true;
        //inputNumero.style.display = "none";
        
    } else {
        //mostrar Dias de suspencion
        inputNumero.hidden = false;
        label.hidden = false;
        //inputNumero.style.display = "block";
    }
    return false;
}

function Anular()
{
    try {
        var frmData = new FormData();
        var justificacion = document.getElementById("Justificacion").value;
        var Estado = "Anulado";
        var Id = $("#Id").val();
            if(justificacion==""|| justificacion==null)
            {
                throw ("Para anular el Proceso es necesario una justicicacón");
        }
        frmData.append("Justificacion", justificacion);
        frmData.append("Estado", Estado);
        frmData.append("Id", Id);
        $.ajax({

            url: "Anular",
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
                        setTimeout(function () { window.location.reload(true) }, 1500);
                       
                    }
                }

            },
            error: function (xhr, status, error)
            { alertify.alert(error); }

        });

    } catch (err) { alertify.alert(err); }
}

function Respuesta()
{
    var inputNumero = document.getElementById('Suspension');
    var button = document.getElementById('EnvioRespuesta');
    button.setAttribute('disabled', 'disabled');
    try {
        var frmData = new FormData();

            var sancion = $("#Sancion").val();
            var motvs = $("#MotivoSan").val();
            var fechad = $("#Fechades").val();
            var fechacd = $("#FechaCdes").val();
            var diasS = $("#Suspension").val();
            var falta = $("#Falta").val();
            var Rj = $("#RJuridica").val();
            var Estado = $("#Estado").val();
            var Id = $("#identificador").val()
            
            if ($("#AdjuntoJuridica").val() == '')
            {
                throw 'Debe adjuntar todos los Documentos solicitados.';
            }
            if (inputNumero.hidden != true) {
                if (inputNumero.value == 0) {
                    throw 'Debe indicar los dias de suspension'
                }
                if (inputNumero.value <= 0) {
                    throw 'Número de días invalido'
                } 
            }
            if (sancion == "Seleccione...") {
                throw 'Seleccione una Sanción';
            }
            if (falta == "Seleccione...") {
                throw 'Seleccione la gravedad de la Falta';
            }
            if (Estado == "Seleccione...") {
                throw 'Seleccione un Estado';  
            }
            if (motvs == "") {
                throw 'Debe digitar el motivo de la Sanción';
            }
            if (fechacd == "") {
                throw 'Seleccione una Fecha de Citación a Descargo';
            }
            if (fechad == "") {
                throw 'Seleccione una Fecha de Descargo';
            }
            if (Rj == "") {
                throw 'Debe digitar la Respuesta Juridíca'
            }

            frmData.append("TipoSancion", sancion);
            frmData.append("MotivoSancion", motvs);
            frmData.append("Fdescargo", fechad);
            frmData.append("FCitaciondescargo", fechacd);
            frmData.append("DiasSuspension", diasS);
            frmData.append("Falta", falta);
            frmData.append("RespuestaJ", Rj);
            frmData.append("Estado", Estado);
            frmData.append("Id", Id);
            frmData.append("Adjunto", $("#AdjuntoJuridica").get(0).files[0]);


            $.ajax({

                url: "Respuesta1",
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
                error: function (xhr, status, error)
                { button.removeAttribute('disabled', 'disabled'); alertify.alert(error); }

            });
    } catch (err) { button.removeAttribute('disabled', 'disabled'); alertify.alert(err); }

    
}

function DefinicionFecha() {
    var Id = $("#identificador").val();
    var fecha = document.getElementById('fechasSus');
    try {
        var frmData = new FormData();
 
        var Id = $("#identificador").val()

        if (fecha.hidden != true) {
            if (fecha.value == "") {
                throw 'Debe indicar la Fecha de Suspension'
            }

        }

        frmData.append("FechaSuspencion", fecha.value);
        frmData.append("Id", Id);

        $.ajax({

            url: "RespuestaJefe1",
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
                        $('#modal_Gestion').modal('hide');

                    }
                }

            },
            error: function (xhr, status, error)
            { alertify.alert(error); }

        });
    } catch (err) { alertify.alert(err); }
  

}

function SaveNote() {
    
    try {

        var Anotacion = $("#Anotacion").val();
        var Anexo = $("#Anexo").val();
        var Anexo1 = $("#Anexo").get(0).files[0];
        var Id = $("#IdProceso").val();
        if(Anotacion==""){
            throw 'Primero debe digitar una Anotación';
        }
        if (Anexo1 == undefined) {
            Anexo1 = "Sin anexar";
        }
        var frmData = new FormData();
        frmData.append("Anotacion", Anotacion);
        frmData.append("Anexo", Anexo1);
        frmData.append("IdProceso", Id);
        //return false;

    $.ajax({       
        url: "NotesUpload",
        type: "POST",
        contentType: "application/json; charset=utf-8",
        dataType: 'json',
        data: frmData,
        contentType: false,
        processData: false,
        success: function (json) {
            var interrupcion=0;
            if (json.respuesta != "") {
                alertify.alert(json.respuesta);
                if (json.isRedirect) {
                    //setTimeout(function () { window.location.reload(true)}, 1500);
                    
                    $('#modal_NotasCreate').modal('hide');
                    $('#modal_NotasCreate2').modal('hide');
                    

                }
            }

        },
        error: function (xhr, status, error)
        { alertify.alert(error); }

    });
} catch (err) { alertify.alert(err); }
    return false;
}

function SavePrueba()
{
    try {

        var TipoPrueba = $("#TipoPrueba").val();
        var Descripcion = $("#Descripcion").val();
        var Anexo = $("#Adjunto").val();
        var Anexo1 = $("#Adjunto").get(0).files[0];
        var Id = $("#IdProceso").val();
        if (Descripcion == "") {
            throw 'Primero debe digitar una Descripción';
        }
        if (Anexo1 == undefined) {
            Anexo1 = "Sin anexar";
        }
        var frmData = new FormData();
        frmData.append("TipoPrueba", TipoPrueba);
        frmData.append("Descripcion", Descripcion);
        frmData.append("Anexo", Anexo1);
        frmData.append("IdProceso", Id);
        //return false;

        $.ajax({
            url: "AgregarPruebas1",
            type: "POST",
            contentType: "application/json; charset=utf-8",
            dataType: 'json',
            data: frmData,
            contentType: false,
            processData: false,
            beforeSend: function () { $("#processState").modal("show"); },
            success: function (json) {
                $("#processState").modal("hide");
                var interrupcion = 0;
                if (json.respuesta != "") {
                    alertify.alert(json.respuesta);
                    if (json.isRedirect) {
                        //setTimeout(function () { window.location.reload(true)}, 1500);

                        $('#modalPrueba63').modal('hide');
                        //$('#modal_NotasCreate2').modal('hide');


                    }
                }

            },
            error: function (xhr, status, error)
            { alertify.alert(error); }

        });
    } catch (err) { alertify.alert(err); }
    return false;
}


function SaveAnexo() {
    try {

        var Anexo = $("#Anexo").val();
        var Anexo1 = $("#Anexo").get(0).files[0];
        var Id = $("#IdProceso").val();
        
        if (Anexo1 == undefined) {
            Anexo1 = "Sin anexar";
        }
        var frmData = new FormData();
        frmData.append("Anexo", Anexo1);
        frmData.append("IdProceso", Id);
        //return false;

        $.ajax({
            url: "AgregarAnexos1",
            type: "POST",
            contentType: "application/json; charset=utf-8",
            dataType: 'json',
            data: frmData,
            contentType: false,
            processData: false,
            beforeSend: function () { $("#processState").modal("show"); },
            success: function (json) {
                $("#processState").modal("hide");
                var interrupcion = 0;
                if (json.respuesta != "") {
                    alertify.alert(json.respuesta);
                    if (json.isRedirect) {
                        //setTimeout(function () { window.location.reload(true)}, 1500);

                        $('#modalAnexo64').modal('hide');
                        //$('#modal_NotasCreate2').modal('hide');


                    }
                }

            },
            error: function (xhr, status, error)
            { alertify.alert(error); }

        });
    } catch (err) { alertify.alert(err); }
    return false;
}

function SaveA()
{
    var frmData = new FormData();
    try {
        //ANEXOS
        var IdProceso = $("#identificador").val();
        var tipoaccion = "ANEXO";
        frmData.append("Id", IdProceso);
        frmData.append("TipoAccion", tipoaccion);

        var filasa = $("#bodyanexos").find("tr");

        if (filasa.length != 0) {
            frmData.append("Cantidadanexos", filasa.length);

            for (i = 1; i <= filasa.length; i++) {
                frmData.append("Anexo" + i, $("#anexo" + i).get(0).files[0]);
                // alert(frmData.get("Anexo" + i));
                if ($("#anexo" + i).val().length == 0) {
                    throw 'Debe adjuntar todos los Documentos solicitados.';
                }
            }

        } else { throw "Para continuar, Primero debe ingresar los Anexos."; }
    //************
    $.ajax({

        url: "LoadFiles",
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
                    //setTimeout(function () { window.location.href = json.redirectUrl }, 1000);
                    $("#Tanexos tbody  tr").remove();
                }
            }

        },
        error: function (xhr, status, error)
        { alertify.alert(error); }

    });
} catch (err) { alertify.alert(err); }//(err) { Message(err); alert(err);}
}

function SaveP()
{
    var frmData = new FormData();
    
    try {
      
        //PRUEBAS
        var IdProceso = $("#identificador").val();
        frmData.append("Id", IdProceso);
        var tipoaccion = "PRUEBA";
        frmData.append("TipoAccion", tipoaccion);
        var filasp = $("#bodyprueba").find("tr");
        if (filasp.length != 0) {
            frmData.append("Cantidadpruebas", filasp.length);
            for (i = 1; i <= filasp.length; i++) {
                frmData.append("PDPruebas.TipoPrueba" + i, $("#TipoPrueba" + i + " option:selected").val());

                frmData.append("Adjunto" + i, $("#Prueba" + i).get(0).files[0]);

                var T = $("#TipoPrueba" + i + " option:selected").val();

                if (T == "Seleccione...") { throw "Para continuar, Primero debe ingresar El tipo de prueba."; }

                if ($("#Prueba" + i).val() == '') { throw 'Debe adjuntar todos los Documentos solicitados.'; }
            }
        } else { throw "Para continuar, Primero debe ingresar las Pruebas."; }

    $.ajax({

        url: "LoadFiles",
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
                    //setTimeout(function () { window.location.href = json.redirectUrl }, 1000);
                   
                    $("#pruebat tbody tr").empty();
                    
            
                }
            }

        },
        error: function (xhr, status, error)
        { alertify.alert(error); }

    });
} catch (err) { alertify.alert(err); }//(err) { Message(err); alert(err);}
}

function SavePD() {
    try {

        var frmData = new FormData();
        //----------FECHA DE REGISTRO---------//
        frmData.append("ProcesoDisciplinario.FechaRegistro", document.getElementById("FechaRegistro").value);//alert(frmData.get("ProcesoDisciplinario.FechaRegistro"));
        //**************************************//
        //-------------FechaHechos---------//
        var fecha = document.getElementById("FechaHechos").value;
        var hora = document.getElementById("HoraHechos").value;
        if (fecha != "" && fecha != undefined) {
            if (hora != "" && hora != undefined) {
                var fechah = fecha + " " + hora;
                frmData.append("FechaHechos", fechah);
            } else { throw "Para continuar, Primero debe ingresar la Hora de los Hechos"; }

        } else { throw "Para continuar, Primero debe ingresar la Fecha de los Hechos"; }
        //**************************************//

        //------------Nivel de Prioridad---------//
        var nivel = $("#NivelP").val();
        if (nivel != "Seleccione...") {
            frmData.append("NivelPrioridad", nivel);
        } else { throw "Para continuar, Primero debe ingresar el Nivel de prioridad"; }
        //**************************************//

        //----------------Empresa---------------//
        var empresa = $("#Empresa  option:selected").val();
        if (empresa != "Seleccione...") {
            frmData.append("Empresa", empresa);
        } else { throw "El Proceso no se ha podido enviar. Debe seleccionar la empresa"; }
        //**************************************//

        //---------------Lugar------------------//
        var lugar = document.getElementById("Lugar").value;
        if (lugar != "" && lugar != undefined) {
            frmData.append("Lugar", lugar);
        } else { throw "Para continuar, Primero debe ingresar el lugar de los Hechos"; }
        //**************************************//

        //-------------EMPLEADOS IMPLICADOS---------//
        var emp = [];
        var filasemp = $("#AddItemsemp").find("tr");

        if (filasemp.length != 0) {
            $("#AddItemsemp tr").each(function (index, item, array) {
                emp.push([item.cells[3].innerText]);
            });

            frmData.append("Empleados", emp);
            frmData.append("Cantidadempleados", filasemp.length);
        } else { throw "Para continuar, Primero debe ingresar los Empleados Implicados"; }
        //******************************************//

        //-------------------PRUEBAS---------//
        var filasp = $("#bodyprueba").find("tr");
        //if (filasp.length != 0) {
            frmData.append("Cantidadpruebas", filasp.length);
            for (i = 1; i <= filasp.length; i++) {
                //alert($("#TipoPrueba"+i+" option:selected").val());
                frmData.append("PDPruebas.TipoPrueba" + i, $("#TipoPrueba" + i + " option:selected").val());
                frmData.append("Adjunto" + i, $("#Prueba" + i).get(0).files[0]);
                var x = $("#Descripcion" + i).val();
                frmData.append("Descripcion" + i, $("#Descripcion" + i).val());
                //alert(frmData.get("Adjunto" + i));
                // alert(frmData.get("PDPruebas.TipoPrueba" + i));
                var T = $("#TipoPrueba" + i + " option:selected").val();
                if (T == "Seleccione...") { throw "Para continuar, Primero debe ingresar El tipo de prueba."; }
                if (T != "testimonial") {
                    if ($("#Prueba" + i).val() == '') { throw 'Debe adjuntar todos los Documentos solicitados.'; }
                }
            }
        //} else { throw "Para continuar, Primero debe ingresar las Pruebas."; }
        //***************************************//

        //--------------ANEXOS---------//
        var filasa = $("#bodyanexos").find("tr");
        //if (filasa.length != 0) {
            frmData.append("Cantidadanexos", filasa.length);
            for (i = 1; i <= filasa.length; i++) {
                frmData.append("Anexo" + i, $("#anexo" + i).get(0).files[0]);
                // alert(frmData.get("Anexo" + i));
                if ($("#anexo" + i).val().length == 0) {
                    throw 'Debe adjuntar todos los Documentos solicitados.';
                }
            }
        //} else { throw "Para continuar, Primero debe ingresar los Anexos."; }
        //***************************************//

        //FUNDAMENTOS
        var F = document.getElementById("Fundamentos").value;
        if (F != '') {
            frmData.append("ProcesoDisciplinario.Fundamentos", document.getElementById("Fundamentos").value);
        } else { throw 'Debe ingresar los Fundamentos'; }
        //***************************************//

        console.log(FormData);

        $.ajax({

            url: "LoadUpdate",
            type: "POST",
            contentType: "application/json; charset=utf-8",
            dataType: 'json',
            data: frmData,
            contentType: false,
            processData: false,
            beforeSend: function () { $("#processState").modal("show"); },
            success: function (json) {
                $("#processState").modal("hide");
                if (json.respuesta != "") {
                    alertify.alert(json.respuesta);
                    if (json.isRedirect) {
                        setTimeout(function () { window.location.href = json.redirectUrl }, 2500);

                    }
                }

            },
            error: function (xhr, status, error)
            { console.log(error); alertify.alert(error); }

        });
    } catch (err) { alertify.alert(err); }//(err) { Message(err); alert(err);}


}


//-----------------------------MODALES---------------------------//
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
    $("a[data-modal14]").on("click", function (e) {
        openmodal14(this.href);
        return false;
    });
    $('#modal_NotasDetails').on('hidden.bs.modal', function () {
        $('#contentModal14').html('');
    })

});

$(document).ready(function () { 
    $.ajaxSetup({ cache: false });
    $("a[data-modal15]").on("click", function (e) {
        openmodal15(this.href);
        return false;
    });

    $('#modal_NotasCreate').on('hidden.bs.modal', function () {
        $('#contentModal15').html('');
     
    })

});

$(document).ready(function () {
    $.ajaxSetup({ cache: false });
    $("a[data-modal14-2]").on("click", function (e) {
        openmodal14(this.href);
        return false;
    });
    $('#modal_NotasDetails').on('hidden.bs.modal', function () {
        $('#contentModal14').html('');
    })

});

$(document).ready(function () {
    $.ajaxSetup({ cache: false });
    $("a[data-modal15-2]").on("click", function (e) {
        openmodal15(this.href);
        return false;
    });

    $('#modal_NotasCreate').on('hidden.bs.modal', function () {
        $('#contentModal15').html('');

    })

});

$(document).ready(function () {
    $.ajaxSetup({ cache: false });
    $("a[data-modalAnular]").on("click", function (e) {
        openmodalAnular(this.href);
        return false;
    });

    $('#modal_Anular').on('hidden.bs.modal', function () {
        $('#contentModalAnular').html('');

    })

});


$(document).ready(function () {
    $.ajaxSetup({ cache: false });
    $("a[data-modal62]").on("click", function (e) {
        openmodal62(this.href);
        return false;
    });
    $('#modal_Respusta').on('hidden.bs.modal', function () {
        $('#contentModal16-2').html('');
    })
});

$(document).ready(function () {
    $.ajaxSetup({ cache: false });
    $("a[data-modal63]").on("click", function (e) {
        openmodal63(this.href);
        return false;
    });
    $('#modalPrueba63').on('hidden.bs.modal', function () {
        $('#contentModal63').html('');
    })
});

$(document).ready(function () {
    $.ajaxSetup({ cache: false });
    $("a[data-modal64]").on("click", function (e) {
        openmodal64(this.href);
        return false;
    });
    $('#modalAnexo64').on('hidden.bs.modal', function () {
        $('#contentModal64').html('');
    })
});


function openmodalAnular(url) {
    $('#contentModalAnular').load(url, function () {
        $('#modal_Anular').modal('show');
        var s = 0;
        //bindForm(this);
    });
}

function openmodal9(url) {
    $('#contentModal9').load(url, function () {
        $('#modal_Gestion').modal('show');
        //bindForm(this);
    });
}
function openmodal14(url) {
    $('#contentModal14').load(url, function () {
        $('#modal_NotasDetails').modal('show');
        var s = 0;
        //bindForm(this);
    });
}

function openmodal15(url) {
    $('#contentModal15').load(url, function () {
        $('#modal_NotasCreate').modal('show');
     
        //bindForm(this);
    });
}

function openmodal62(url) {
    $('#contentModal16-2').load(url, function () {
        $('#modal_Respusta').modal('show');

        //bindForm(this);
    });
}

function openmodal63(url) {
    $('#contentModal63').load(url, function () {
        $('#modalPrueba63').modal('show');

        //bindForm(this);
    });
}

function openmodal64(url) {
    $('#contentModal64').load(url, function () {
        $('#modalAnexo64').modal('show');

        //bindForm(this);
    });
}


$("#table tbody tr").click(function () {
    var total = $(this).find("td:last-child").text();
    alert(total);
});
    //$(document).on('show.bs.modal', '#modal_NotasCreate2', function () {
    //    $('#contentModal15-2').load('../ProcesoDisciplinario/NotasCreate/' + $("#Id").val(), function () {
    //        $('#modal_NotasCreate2').modal('show');

    //        //bindForm(this);
    //    });
    //});
    //$(document).on('hide.bs.modal', '#modal_NotasCreate2', function () {
    //    $('#contentModal15-2').html('');
    //});

    //$(document).on('show.bs.modal', '#modal_NotasDetails2', function () {
    //        $('#contentModal14-2').load('../ProcesoDisciplinario/NotaDetails/' + $("#Id").val(), function () {
    //            $('#modal_NotasDetails2').modal('show');

    //            //bindForm(this);
    //        });
    //    });
    //$(document).on('hide.bs.modal', '#modal_NotasCreate2', function () {
    //       $('#contentModal14-2').html('');
    //   });

