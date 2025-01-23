//$(document).ready(function () {
//    Vacaciones();
//});


//document.getElementById('txt_candiassol_vac').addEventListener("keyup", calculardias);
//document.getElementById("txt_fechainicialVac").addEventListener("input", calcularfecha);
//document.getElementById("txt_fechainicialVac").addEventListener("input", ValidarDias);
//document.getElementById("txt_fechainicialVac").addEventListener("input", ValidarFechaini);
//document.getElementById("txt_vacacionespagas").addEventListener("input", VacacionesPagas);
//document.getElementById("txt_candiassol_vac").addEventListener("input", ValidarDias);
//document.getElementById("btn_anticipadas_pagas").addEventListener("click", AutorizaAnticipadasPagas);
//document.getElementById("txt_vacacionespagas").addEventListener("input", validar_Pagas);

/*var servidor = "https://foscal.co/admautogestion";*/
var servidor = "http://localhost:55389";
//var servidor = "http://190.71.21.57/admautogestion";
var Codigo = "";
var Id = "";

var cantdiaspendi = 0;


$(function () {
    $("#formSolicitarVacAntPag").on("submit", function (e) {

        try {
            e.preventDefault();
            var f = $(this);
            var Id = new URLSearchParams(window.location.search).get('id');

            //        try{

            var Dinero = (document.getElementById("SltDinero").value);
            var Anticipadas = (document.getElementById("SltAnticipadas").value);
            var Mayor6 = (document.getElementById("SltMayor6").value);
            if (Dinero == "vacio" || Dinero ==null || Dinero=="")
            {
                throw "Falta definir si las vacaciones son en dinero";
            }
            if (Anticipadas == "vacio" || Anticipadas == null || Anticipadas == "") 
            {
                throw "Falta definir si son Anticipadas";

            }
            if (Mayor6 == "vacio" || Mayor6 == null || Mayor6 == "")
            {
                throw "Falta definir sin es mayor o menor a 6 dias";
            }
                    //var EmpleadoId = (document.getElementById("txt_id").value);

            //        var urlDetalle = servidor + "/api/AutorizaAnticipadasPagas/" + Dinero + "/" + Anticipadas + "/" + Mayor6 + "/" + EmpleadoId;

            //        $.ajax({
            //            url: urlDetalle,
            //            //data: JSON.stringify(Empleado),
            //            type: 'GET',
            //            contentType: 'application/json; charset=utf-8',
            //            dataType: 'json',
            //            success: function (result) {
            //                //$("#processState").modal("hide");
            //                //if (result == "true") {
            //                //    alertify.alert('Los datos se registraron correctamente');

            //                //    atras();
            //                //} else {
            //                //    $("#processState").modal("hide");
            //                //    alertify.alert(result);
            //                //}
            //            }
            //        })


            var frmData = new FormData();
            frmData.append("Autoriza.EmpleadoId", Id);
            frmData.append("Autoriza.Dinero", document.getElementById("SltDinero").value);
            frmData.append("Autoriza.Anticipadas", document.getElementById("SltAnticipadas").value);
            frmData.append("Autoriza.Mayor6", document.getElementById("SltMayor6").value);

            $.ajax({
                url: servidor + "/api/AutorizaAnticipadasPagas",
                type: 'POST',
                contentType: 'application/json; charset=utf-8',
                dataType: 'json',
                data: frmData,
                contentType: false,
                processData: false,
                //beforeSend: function () { $("#processState").modal("show"); },
                success: function (result) {

                    if (result == "true") {
                        //$("#processState").modal("hide");
                        alertify.alert('Su solicitud se registro correctamente');
                        setTimeout(function () { window.location.reload(true) }, 2500);
                        //atras_Vacaciones();
                    } else {
                        //$("#processState").modal("hide");
                        alertify.alert(result);
                    }
                },
            })


                .done(function (res) {
                    $("#mensaje").html("Respuesta: " + res);
                });


        }

        catch (err) {
            alertify.alert("Error: " + err);
        }


        //    } catch (err) {
        //            $("#processState").modal("hide");
        //    alertify.alert(err);
        //}

    });
});


function Vacaciones() {

    HistorialVacaciones();

    Codigo = new URLSearchParams(window.location.search).get('codigo');
    Id = new URLSearchParams(window.location.search).get('id');

    /*var Id = $("#txt_id").val();*/

    var urlDetalle = servidor + "/api/Vacaciones/Autoriza/" + Id;


    $.ajax({
        url: urlDetalle,
        //data: JSON.stringify(Empleado),
        type: 'GET',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: function (data) {
            if (data == "DsMayorAntPag") {
                $("#AutorizaVacPag").show();
                $("#AutorizaVacAdel").show();
                $("#DiasMayora6").show();
            }
            if (data == "DsMayorAnt") {
                $("#AutorizaVacPag").hide();
                $("#AutorizaVacAdel").show();
                $("#DiasMayora6").show();
            }
            if (data == "DsMayorPag") {
                $("#AutorizaVacPag").show();
                $("#AutorizaVacAdel").hide();
                $("#DiasMayora6").show();
            }
            if (data == "AntPag") {
                $("#AutorizaVacPag").show();
                $("#AutorizaVacAdel").show();
                $("#DiasMayora6").hide();
            }
            if (data == "DsMayor") {
                $("#AutorizaVacPag").hide();
                $("#AutorizaVacAdel").hide();
                $("#DiasMayora6").show();
            }
            if (data == "Ant") {
                $("#AutorizaVacPag").hide();
                $("#AutorizaVacAdel").show();
                $("#DiasMayora6").hide();
            }
            if (data == "Pag") {
                $("#AutorizaVacPag").show();
                $("#AutorizaVacAdel").hide();
                $("#DiasMayora6").hide();
            }
            if (data == "SinAut") {
                $("#AutorizaVacPag").hide();
                $("#AutorizaVacAdel").hide();
                $("#DiasMayora6").hide();
            }

        }
    })



    var urlDetalle = servidor + "/api/Historial/" + Codigo;


    $.ajax({
        url: urlDetalle,
        //data: JSON.stringify(Empleado),
        type: 'GET',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: function (data) {

            var datos = data.split(";");

            $("#TabHsVac").html(datos[0]);

            var Resultado = datos[1];

            $("#txt_dias_x_disf").val(Resultado);


            cantdiaspendi = Resultado;
            $("#txt_candiaspen_vac").val(cantdiaspendi);
        }
    })



}

function HistorialVacaciones() {






    var urlDetalle = servidor + "/api/Historial/" + Codigo;


    $.ajax({
        url: urlDetalle,
        //data: JSON.stringify(Empleado),
        type: 'GET',
        contentType: 'application/json; charset=utf-8',
        dataType: 'json',
        success: function (data) {

            var datos = data.split(";");

            $("#TabHistVacaciones").html(datos[0]);

            var Resultado = datos[1];

            $("#txt_dias_x_disf").val(Resultado);


            cantdiaspendi = Resultado;
            $("#txt_candiaspen_vac").val(cantdiaspendi);
        }
    })

}