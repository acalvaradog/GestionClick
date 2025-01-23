$(document).ready(function () {

    //----------------------------- MODALE DELETE --------------------------//
    $.ajaxSetup({ cache: false });
    $("a[data-modalEMPDetail]").on("click", function (e) {
        openmodalEMP01(this.href);
        return false;
    });

    $('#modal_EMPDetail').on('hidden.bs.modal', function () {
        $('#contentModalEMPDetail').html('');

    })
});
//---------------------------- OPEN MODAL---------------------------//
function openmodalEMP01(url) {
    $('#contentModalEMPDetail').load(url, function () {
        $('#modal_EMPDetail').modal('show');

        //bindForm(this);
    });

    
}

