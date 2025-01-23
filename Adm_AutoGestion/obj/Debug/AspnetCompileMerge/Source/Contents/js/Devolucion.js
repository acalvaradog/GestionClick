
$(document).ready(function () {
    $.ajaxSetup({ cache: false });
    $("a[data-modalDEV]").on("click", function (e) {
        openmodalDEV(this.href);
        return false;
    });
    $('#modal_DevolucionPrestamo').on('hidden.bs.modal', function () {
        $('#contentModalDEV').html('');



    })


});

function openmodalDEV(url) {

    //ValidarDevolucionActivo();
    $('#contentModalDEV').load(url, function () {
        $('#modal_DevolucionPrestamo').modal('show');
        //bindForm(this);
        html5QrcodeScannerd.render(onScanSuccess);
    });
}


var lectord = document.getElementById('lectord');
var html5QrcodeScannerd = new Html5QrcodeScanner(
		"lectord", { fps: 10, qrbox: 250 }, /* verbose= */ true);
var resultsd = document.getElementById('scanned-resultd');
var lastMessaged="";
var codesFoundd = 0;
function onScanSuccess(qrCodeMessage) {
    //if (lastMessaged !== qrCodeMessage) {
        lastMessaged = qrCodeMessage;
        ++codesFoundd;

        results.innerHTML += '<div>[${codesFound}] - ${qrCodeMessage}</div>';
        //firmaractaepp(qrCodeMessage);
        $("#QRPrestamosd").val(qrCodeMessage);
    //}
}

function SaveDevolucion() {

    
    var llaveQR = $('#QRPrestamosd').val();

    try {


        if (llaveQR == "" || llaveQR == null) {

            throw("Debe validar llave QR")
        } 

            model = {
                "IdEntrega": $("#IdEntrega").val(),

            };

            var datosenviar = JSON.stringify(model);
            $.ajax({
                data: datosenviar,
                url: "GuardarDevolucion",
                type: "post",
                contentType: "application/json; charset=utf-8",
                //success: function (e) {

                //$("#mensaje").val(e);
                success: function (response) {

                    //location.replace('ListSolicitantes')
                    alertify.alert(response);
                    $("#btnDevolucionP").prop('disabled', true);
                    $('#modal_DevolucionPrestamo').modal('hide');
                },

            });

    } catch (err) {

        alertify.alert(err);


    }



}


