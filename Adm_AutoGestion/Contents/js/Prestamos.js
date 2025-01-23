
$(document).ready(function () {
    $.ajaxSetup({ cache: false });
    $("a[data-modalPRES]").on("click", function (e) {
        openmodalPRES(this.href);
        return false;
    });
    $('#modal_SolicitudPrestamo').on('hidden.bs.modal', function () {
        $('#contentModalPRES').html('');

      

    })

    $.ajaxSetup({ cache: false });
    $("a[data-modalCP]").on("click", function (e) {
        openmodal60(this.href);

        return false;
    });

    $('#modal_Solicitante').on('hidden.bs.modal', function () {
        $('#contenido').html('');

    })

  

    function openmodal60(url) {
        $('#contenido').load(url, function () {
            $('#modal_Solicitante').modal('show');
            
                var documento = $("#Documentos").val();
                $("#Documento").val(documento);
                $("#FechaIngreso").prop('disabled', true);
                $("#FechaFin").prop('disabled', true);


           
            //bindForm(this);
        });
    }
  
});

$(document).ready(function () {
    $.ajaxSetup({ cache: false });
    $("a[data-modalQRP]").on("click", function (e) {
        openmodalQRTercero(this.href);
        return false;
    });

    $('#modal_QRTercero').on('hidden.bs.modal', function () {
        $('#QRTerceroContent').html('');

    })


});
function openmodalQRTercero(url) {
    $('#QRTerceroContent').load(url, function () {
        $('#modal_QRTercero').modal('show');

        //bindForm(this);
    });
}


function openmodalPRES(url) {
    $('#contentModalPRES').load(url, function () {


        
        var fecha = new Date(); //Fecha actual
        var mes = fecha.getMonth() + 1; //obteniendo mes
        var dia = fecha.getDate(); //obteniendo dia
        var ano = fecha.getFullYear(); //obteniendo año
        if (dia < 10)
            dia = '0' + dia; //agrega cero si el menor de 10
        if (mes < 10)
            mes = '0' + mes //agrega cero si el menor de 10
        // document.getElementById("FechaEntrega").value = ano + "-" + mes + "-" + dia;

        fechaR = ano + "-" + mes + "-" + dia;
        $("#FechaRegistro").val(fechaR);

        $('#modal_SolicitudPrestamo').modal('show');
        //bindForm(this);
        html5QrcodeScanner.render(onScanSuccessd);
    });
}

var lector = document.getElementById('lector');
var html5QrcodeScanner = new Html5QrcodeScanner(
		"lector", { fps: 10, qrbox: 250 }, /* verbose= */ true);
var results = document.getElementById('scanned-result');
var lastMessage = "";
var codesFound = 0;
function onScanSuccessd(qrCodeMessage) {
    //if (lastMessage !== qrCodeMessage) {
    lastMessage = qrCodeMessage;
    ++codesFound;

    results.innerHTML += '<div>[${codesFound}] - ${qrCodeMessage}</div>';
    //firmaractaepp(qrCodeMessage);
    $("#QRPrestamos").val(qrCodeMessage);
    //}
}


function SaveTercero() {

    var model;
    var nombres = "";
   

    var PNombre = $("#PrimerNombre").val();
    var SNombre = $("#SegundoNombre").val();
    var PApellido = $("#PrimerApellido").val();
    var SApellido = $("#SegundoApellido").val();
    var FechaNacimiento = $("#FechaNacimiento").val();
    var Direccion = $("#Direccion").val();
    var Telefono = $("#Telefono").val();
    var correo = $("#CorreoPersonal").val();
    var Area = $("#Area").val();
    var superior = $("#Superior").val();
    var Estudiante = $("#Estudiante").val();
    var FechaIngreso = $("#FechaIngreso").val();
    var FechaFin = $("#FechaFin").val(); 
    var Sociedad = $("#SociedadCOD").val(); 
   


    try {

      
            if (PApellido == "" || PApellido == null) { throw("Digitar Primer Apellido"); $("#PrimerApellido").focus(); }
            if (PNombre == "" || PNombre == null) { throw ("Digitar Primer Nombre"); $("#PrimerNombre").focus(); }
            if (FechaNacimiento == "" || FechaNacimiento == null) { throw ("Digitar Fecha de Nacimiento"); $("#FechaNacimiento").focus(); }
            if (FechaNacimiento != "" || FechaNacimiento != null) { var edad = validar_fechaNacimiento(FechaNacimiento); }
            
            if( edad <= 14){
                throw ("Fecha Nacimiento no valida");
            
            }
            if (Direccion == "" || Direccion == null) { throw ("Digitar  Dirección"); $("#Direccion").focus(); }
            if (Telefono == "" || Telefono == null) { throw ("Digitar Telefono"); $("#Telefono").focus(); }
            if (correo == "" || correo == null) { throw ("Digitar Correo"); $("#CorreoPersonal").focus(); }
            if (Area == "--Seleccione Area--") { throw ("Seleccione Area"); $("#Area").focus(); }
            if (superior == "--Seleccione Jefe--") { throw ("Seleccione Jefe"); $("#Superior").focus(); }
            if (Estudiante == "" || Estudiante == null) { throw ("Seleccione Estudiante"); $("#Estudiante").focus(); }
        if (Sociedad == "--Seleccione...--") { throw ("Seleccione Jefe"); $("#Superior").focus(); }
            if (Estudiante == "SI") {

             if (FechaIngreso == null || FechaIngreso == "" || FechaFin == null || FechaFin == "")

                { throw ("Digite Fecha Ingreso y/ o FechaFin"); }

            }
            if (correo != "" || correo == null) {
                var resul_validarcorreo = validarcorreo(correo);
            }
            
            if (resul_validarcorreo == true) {

                throw ("Correo no es valido");
            }

            if (FechaIngreso != null || FechaIngreso != "" || FechaFin != null || FechaFin != "") {
                var resul_validar_fecha = validar_fecha(FechaIngreso, FechaFin);
            };

            if (resul_validar_fecha == true) {

                throw ("Fecha Inicio es mayor a Fecha Fin");
            }

       

            nombres = PApellido + " " + SApellido + " " + PNombre + " " + SNombre;




            model = {
                "Documento": $("#Documento").val(), "Nombres": nombres, "Activo": $("#Activo").val(),
                "FechaNacimiento": $("#FechaNacimiento").val(), "Direccion": $("#Direccion").val(), "Telefono": $("#Telefono").val(), "CorreoPersonal": $("#CorreoPersonal").val(), "Area": $("#Area").val(), "Superior": $("#Superior").val(),
                "Cargo": $("#Cargo").val(), "Estudiante": $("#Estudiante").val(), "FechaIngreso": $("#FechaIngreso").val(), "FechaFin": $("#FechaFin").val(), "SociedadCOD": $("#SociedadCOD").val()

            };

            var datosenviar = JSON.stringify(model);

            $.ajax({
                data: datosenviar,
                url: "GuardarTercero",
                type: "post",
                contentType: "application/json; charset=utf-8",
                //success: function (e) {

                //$("#mensaje").val(e);
                success: function (response) {

                    //location.replace('ListSolicitantes')
                    alertify.alert(response);
                    $("#btnTercero").prop('disabled', true);
                    $('#modal_Solicitante').modal('hide');

                },


            });


        

    }

    catch (err) {

        alertify.alert(err);


    }
 
    
}


function validarcorreo(email) {
    re = /^([\da-z_\.-]+)@([\da-z\.-]+)\.([a-z\.]{2,6})$/
   

    if (!re.exec(email)) {

        return true; 

    }
    return false; 
}

function isValidDate(day, month, year) {
    var dteDate;
    month = month - 1;
    dteDate = new Date(year, month, day);
    return ((day == dteDate.getDate()) && (month == dteDate.getMonth()) && (year == dteDate.getFullYear()));
}


function validate_fecha(fecha) {
    var patron = new RegExp("^(19|20)+([0-9]{2})([-])([0-9]{1,2})([-])([0-9]{1,2})$");
  

        if (fecha.search(patron) == 0) {
            var values = fecha.split("-");
            if (isValidDate(values[2], values[1], values[0])) {
                return true;
            }
        }
        return false;
    
    
    

    

    
}

function validar_fecha(fechaInicial,fechaFinal) {
        if (validate_fecha(fechaInicial) && validate_fecha(fechaFinal)) {
            inicial = fechaInicial.split("-");
            final = fechaFinal.split("-");
            // obtenemos las fechas en milisegundos
            var dateStart = new Date(inicial[0], (inicial[1] - 1), inicial[2]);
            var dateEnd = new Date(final[0], (final[1] - 1), final[2]);

            if (dateStart > dateEnd) {
                return true;
            }
        }
        return false;
}


function validar_fechaNacimiento(fechaN) {
    if (validate_fecha(fechaN)) {

        var hoy = new Date();   
        var cumpleanos = new Date(fechaN);
        var edad = hoy.getFullYear() - cumpleanos.getFullYear();
        var m = hoy.getMonth() - cumpleanos.getMonth();
        if (m < 0 || (m === 0 && hoy.getDate() < cumpleanos.getDate())) {
            edad--;
        }
        return edad;
    }


}

function AddRow(dialog, insert) {

    var field = ["", ""];

    if (dialog == "Addfc") { field[0] = "AddItems"; field[1] = "<tr><td><a href = '' onclick = '$(this).parent().parent().remove(); return false;'><img alt='Quitar' src='../../Contents/image/Trash.png' title='Quitar'/><a/></td><td>" + $("#TipoElemento option:selected").val() + "</td><td>" + $("#TipoElemento option:selected").text() + "</td><td>" + $("#Archivo").val() + "</td>"; }
    if (dialog == "AddElemento") {


        var Documento = $("#Documento").val();
        var FechaRegistro = $("#FechaRegistro").val();
        var Sociedad = $("#Sociedad").val();
        var LugarEntrega = $("#LugarEntrega").val();
        var TipoArea = $("#TipoArea").val();
        var AreaDirige = $("#AreaDirige").val();
        var Llave = $("#QRPrestamos").val();





        try {
            var TipoElemento = $("#TipoElemento option:selected").text();
            if (TipoElemento == "Seleccione...") {
                throw "Para continuar, Primero debe seleccionar el Elemento de elemento.";
            }

            var Talla = $("#Talla option:selected").text();
            if (Talla == "Seleccione...") {
                throw "Para continuar, Primero debe seleccionar Talla.";
            }

            if ($("#Cantidad").val() == "" || $("#Cantidad").val() <= 0) {
                throw "Para continuar, Primero debe ingresar la cantidad.";
            }

            if ($("#Sociedad option:selected").text() == "Seleccione...") {
                throw "Para continuar, Primero debe seleccionar Sociedad.";
            }

            if ($("#LugarEntrega option:selected").text() == "Seleccione...") {
                throw "Para continuar, Primero debe seleccionar Lugar de Entrega.";
            }

            if ($("#TipoArea option:selected").text() == "Seleccione...") {
                throw "Para continuar, Primero debe seleccionar TipoArea.";
            }

            if ($("#AreaDirige option:selected").text() == "Seleccione...") {
                throw "Para continuar, Primero debe seleccionar Area donde se dirige.";
            }





            field[0] = "AddItems1"; field[1] = "<tr><td style='text-align: center'><a href = '' onclick = '$(this).parent().parent().remove(); return false;'><img alt='Quitar' src='../Contents/image/Trash.png' title='Quitar'/><a/></td><td>" + $("#TipoElemento option:selected").val() + "</td><td>" + $("#TipoElemento option:selected").text() + "</td><td>" + $("#Talla option:selected").val() + "</td><td>" + $("#Cantidad").val() + "</td>";


        }
        catch (err) { alertify.alert(err); }


    }


    if (insert == true) { $("#" + field[0]).append(field[1]); }
    if (insert == false) { $("#" + field[0]).html(field[1]); }
    return false;
}


function SavePrestamo() {
    var model;
    var ListadoElementos = [];
    var fila = [];
    var requis = [];
    var i = 0;
    var e = 0;
    var fila = "";
    
    var llaveQR = $('#QRPrestamos').val();

    try {

        if (llaveQR == "" || llaveQR == null) {

            throw("Debe validar llave QR")
        }


      


        $("#AddItems1 tr").each(function (index, item, array) {
            ListadoElementos.push({ "IdTipoElementos": item.cells[1].innerText, "Talla": item.cells[3].innerText, "Cantidad": item.cells[4].innerText })
        });

        model = {
            "Documento": $("#Documento").val(), "FechaRegistro": $("#FechaRegistro").val(), "AreaDirige": $("#AreaDirige").val(),
            "TipoArea": $("#TipoArea").val(), "LugarEntrega": $("#LugarEntrega").val(),
            "Sociedad": $("#Sociedad").val(), "QRPrestamos": $("#QRPrestamos").val(), "ListadoElementos": ListadoElementos
        };


        //reqs.each(function (index, item, array) {e = e + 1; requis += (requis == "" ? "" : String.fromCharCode(10)) + item.cells[1].innerText });



        var datosenviar = JSON.stringify(model);
        $.ajax({
            data: datosenviar,
            url: "GuardarPrestamo",
            type: "post",
            contentType: "application/json; charset=utf-8",
            //success: function (e) {

            //$("#mensaje").val(e);
            success: function (response) {

                alertify.alert(response);
                //location.replace('ListSolicitantes')
                $("#btnPrestamo").prop('disabled', true);
                $('#modal_SolicitudPrestamo').modal('hide');
               


            },
            

        });

    }
    catch (err) {
        alertify.alert(err);
    }


}

$(document).ready(function () {
    $('#reader').html5_qrcode(function (data) {
        $('#QRPrestamos').val(data);
    },
        function (error) {

        }, function (videoError) {
            alert("No hay camara");
        }
    );
});

function AnularPrestamo1(id) {

    alertify.confirm("Esta Seguro que Desea anular la Solicitud de Prestamo?", function (e) {
        if (e) {

            AnularPrestamo(id);
        } else {

        }
    });
}

function AnularPrestamo(id) {


    try {


        model = {
            "Documento": id,

        };

        var datosenviar = JSON.stringify(model);
        $.ajax({
            data: datosenviar,
            url: "AnularPrestamo",
            type: "post",
            contentType: "application/json; charset=utf-8",
            //success: function (e) {

            //    $("#mensaje").val(e);
                success: function (response) {

                    //location.replace('ListSolicitantes')
                    alertify.alert(response);
                
                }

            });

    } catch (err) { alertify.alert(err); }
    



}

function ActualizarQR(id)
{
    try {


        model = {
            "Documento": id,
        };

        var datosenviar = JSON.stringify(model);
        $.ajax({
            data: datosenviar,
            url: "ActualizarQR",
            type: "post",
            contentType: "application/json; charset=utf-8",
            //success: function (e) {

            //    $("#mensaje").val(e);
            success: function (response) {

                //location.replace('ListSolicitantes')
                alertify.alert(response);

            }

        });

    } catch (err) { alertify.alert(err); }


}































