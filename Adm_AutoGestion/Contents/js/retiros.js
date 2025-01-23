/// <reference path="retiros.js" />

$(document).ready(function () {

   
   

    });

 

function EnviarEncuesta(id) {
    //var data = JSON.stringify({ 'area': 'area', 'fechainicial': '1', 'fechafinal': '1' });


    try {
        $.ajax({
            url: "EnvioEncuesta",
            data: { 'id': id},
            type: "post",
            success: function (e) {
                if (e != undefined || e != null) {
                    if (e == 'OK') {
                        //alertify.alert('Se ha enviado la encuesta');
                        alertify.confirm("Se ha enviado la encuesta.", function (s) {
                            if (s) {

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


function Confirmar(id) {

    alertify.confirm("Confirme si desea enviar la encuesta",
            function (e) {
                if (e) {
                    EnviarEncuesta(id)
                } else {
                   
                }
            });


}







