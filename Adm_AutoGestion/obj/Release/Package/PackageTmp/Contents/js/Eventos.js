function mostrarCupo() {
    var e = document.getElementById('DirigidoA');
    var c = document.getElementById('Cupo');
    var rr = document.getElementById('RegistroRequerido');
    var filaCierre = document.getElementById('filaCierre');
    var filaLink1 = document.getElementById('filaLink1');
    var filaLink2 = document.getElementById('filaLink2');
    var configEvento = document.getElementById('configEvento');
    var parentesco = document.getElementById('ParentescoPermitido');
    var EdadLimite = document.getElementById('EdadLimite');
    var tipoE = document.getElementById('TipoEvento');
    var labelR = document.getElementById('labelRelacionar');
    var R = document.getElementById('Relacionar');
    var P = document.getElementById('EsEventoPrincipal');

    if (e.value == "General") {
        c.disabled = true;
        c.placeholder = "Sin limite de cupos.";
        c.value = "";
        rr.checked = false;
        rr.disabled = true;
        filaCierre.hidden = true;
        filaLink1.hidden = true;
        filaLink2.hidden = true;
        configEvento.hidden = true;
        P.checked = false;
    } else {
        c.disabled = false;
        c.placeholder = "";
        rr.disabled = false;
    }
    if (e.value == "Familiares" || e.value == "Ambos" && rr.checked == true) {
        configEvento.hidden = false;
        parentesco.value = "";
        EdadLimite.value = "";
        parentesco.disabled = false;
        EdadLimite.disabled = false;
    }if (e.value == "Trabajadores" && tipoE.value == '3') {
        labelR.hidden = false;
        R.hidden = false;
        parentesco.disabled = true;
        EdadLimite.disabled = true;
    } if (e.value != "Familiares" && e.value != "Ambos" && tipoE.value != '3') {
        parentesco.disabled = true;
        EdadLimite.disabled = true;
    }
}

function mostrarFechaFin() {
    var e = document.getElementById('DirigidoA');
    var tipoE = document.getElementById('TipoEvento');
    var fechaFin = document.getElementById('FechaFin');
    var rr = document.getElementById('RegistroRequerido');
    var filaCierre = document.getElementById('filaCierre');
    var filaLink1 = document.getElementById('filaLink1');
    var filaLink2 = document.getElementById('filaLink2');
    var configEvento = document.getElementById('configEvento');
    var labelR = document.getElementById('labelRelacionar');
    var R = document.getElementById('Relacionar');
    var labelP = document.getElementById('labelP');
    var P = document.getElementById('EsEventoPrincipal');

    if (tipoE.value == '2' && e.value != "General") {
        fechaFin.disabled = false;
        fechaFin.placeholder = "";
        fechaFin.type = "date";
        fechaFin.value = "";
        labelR.hidden = true;
        R.hidden = true;
        P.hidden = true;
        labelP.hidden = true;
    } else if (tipoE.value == '3' && e.value != "General") {
        rr.checked = true;
        filaCierre.hidden = false;
        filaLink1.hidden = false;
        filaLink2.hidden = false;
        configEvento.hidden = false;
        labelR.hidden = false;
        R.hidden = false;
        P.hidden = false;
        labelP.hidden = false;
        fechaFin.disabled = true;
    }
    else if (tipoE.value == '2' && e.value == "General") {
        P.hidden = true;
        labelR.hidden = true;
        R.hidden = true;
        P.checked = false;
        labelP.hidden = true;
        fechaFin.disabled = false;
        fechaFin.placeholder = "";
        fechaFin.type = "date";
        fechaFin.value = "";
    }else{
        P.hidden = true;
        labelR.hidden = true;
        R.hidden = true;
        P.checked = false;
        labelP.hidden = true;
        fechaFin.disabled = true;
    }
}

function ocultarRelaciones() {
    var R = document.getElementById('Relacionar');
    var labelR = document.getElementById('labelRelacionar');
    var P = document.getElementById('EsEventoPrincipal');

    if (P.checked) {
        labelR.hidden = true;
        R.hidden = true;
    } else {
        R.hidden = false;
        labelR.hidden = false;
        R.value = "";
    }
}

function cambiarFechaFin() {
    var tipoE = document.getElementById('TipoEvento');
    var fechaInicio = document.getElementById('FechaInicio');
    var fechaFin = document.getElementById('FechaFin');

    if (tipoE.value != '2') {
        fechaFin.type = "text";
        fechaFin.value = "";
        fechaFin.placeholder = fechaInicio.value;
    }
}

function cambiarFechaFinDetalles() {
    var tipoE = document.getElementById('TipoEvento');
    var fechaInicio = document.getElementById('FechaInicio');
    var fechaFin = document.getElementById('FechaFin');

    if (tipoE.value == "Unico") {
        fechaFin.value = fechaInicio.value;
    }
}

function registroReq() {

    var rr = document.getElementById('RegistroRequerido');
    var filaCierre = document.getElementById('filaCierre');
    var filaLink1 = document.getElementById('filaLink1');
    var filaLink2 = document.getElementById('filaLink2');
    var configEvento = document.getElementById('configEvento');
    var e = document.getElementById('DirigidoA');
    var parentesco = document.getElementById('ParentescoPermitido');
    var EdadLimite = document.getElementById('EdadLimite');

    if (rr.checked && e.value == "Familiares" || e.value == "Ambos") {
        parentesco.disabled = false;
        EdadLimite.disabled = false;
    }
    if (rr.checked) {
        filaCierre.hidden = false;
        filaLink1.hidden = false;
        filaLink2.hidden = false;
        configEvento.hidden = false;
    } else {
        filaCierre.hidden = true;
        filaLink1.hidden = true;
        filaLink2.hidden = true;
        configEvento.hidden = true;
    }
}

//Modal Ver Detalles
$(document).ready(function () {
    $.ajaxSetup({ cache: false });
    $("a[data-detalles]").on("click", function (e) {
        openmodaldetalles(this.href);
        return false;
    });
    $('#modal_VerDetalles').on('hidden.bs.modal', function () {
        $('#contentModalDetalles').html('');
    })
});

function openmodaldetalles(url) {
    $('#contentModalDetalles').load(url, function () {
        $('#modal_VerDetalles').modal('show');
    });
}

//Modal Link Encuesta Asistidos
$(document).ready(function () {
    $.ajaxSetup({ cache: false });
    $("a[data-linkasistidos]").on("click", function (e) {
        openmodallinkasistidos(this.href);
        return false;
    });
    $('#modal_LinkAsistidos').on('hidden.bs.modal', function () {
        $('#contentModalLinkAsistidos').html('');
    })
});

function openmodallinkasistidos(url) {
    $('#contentModalLinkAsistidos').load(url, function () {
        $('#modal_LinkAsistidos').modal('show');
    });
}

//Modal Link Encuesta No Asistidos
$(document).ready(function () {
    $.ajaxSetup({ cache: false });
    $("a[data-linknoasistidos]").on("click", function (e) {
        openmodallinknoasistidos(this.href);
        return false;
    });
    $('#modal_LinkNoAsistidos').on('hidden.bs.modal', function () {
        $('#contentModalLinkNoAsistidos').html('');
    })
});

function openmodallinknoasistidos(url) {
    $('#contentModalLinkNoAsistidos').load(url, function () {
        $('#modal_LinkNoAsistidos').modal('show');
    });
}

//Modal Cerrar Evento
$(document).ready(function () {
    $.ajaxSetup({ cache: false });
    $("a[data-cerrarevento]").on("click", function (e) {
        openmodalcerrarevento(this.href);
        return false;
    });
    $('#modal_CerrarEvento').on('hidden.bs.modal', function () {
        $('#contentModalCerrarEvento').html('');
    })
});

function openmodalcerrarevento(url) {
    $('#contentModalCerrarEvento').load(url, function () {
        $('#modal_CerrarEvento').modal('show');
    });
}

//Modal Confirmar Cierre
$(document).ready(function () {
    $.ajaxSetup({ cache: false });
    $("a[data-confirmarcierre]").on("click", function (e) {
        openmodalconfirmarcierre(this.href);
        return false;
    });
    $('#modal_ConfirmarCierre').on('hidden.bs.modal', function () {
        $('#contentModalConfirmarCierre').html('');
    })
});

function openmodalconfirmarcierre(url) {
    $('#contentModalConfirmarCierre').load(url, function () {
        $('#modal_ConfirmarCierre').modal('show');
        
    });
}

//Modal Confirmar Cambio de Estado
$(document).ready(function () {
    $.ajaxSetup({ cache: false });
    $("a[data-estadoevento]").on("click", function (e) {
        openmodalcambiarestado(this.href);
        return false;
    });
    $('#modal_CambiarEstado').on('hidden.bs.modal', function () {
        $('#contentModalCambiarEstado').html('');
    })
});

function openmodalcambiarestado(url) {
    $('#contentModalCambiarEstado').load(url, function () {
        $('#modal_CambiarEstado').modal('show');

    });
}

//Modal Pendiente por Firma
$(document).ready(function () {
    $.ajaxSetup({ cache: false });
    $("a[data-pendientefirma]").on("click", function (e) {
        openmodalpendientefirma(this.href);
        return false;
    });
    $('#modal_PendienteFirma').on('hidden.bs.modal', function () {
        $('#contentModalPendienteFirma').html('');
    })
});

function openmodalpendientefirma(url) {
    $('#contentModalPendienteFirma').load(url, function () {
        $('#modal_PendienteFirma').modal('show');

    });
}

//Modal Generar Enlace
$(document).ready(function () {
    $.ajaxSetup({ cache: false });
    $("a[data-generarenlace]").on("click", function (e) {
        openmodalgenerarenlace(this.href);
        return false;
    });
    $('#modal_GenerarEnlace').on('hidden.bs.modal', function () {
        $('#contentModalGenerarEnlace').html('');
    })
});

function openmodalgenerarenlace(url) {
    $('#contentModalGenerarEnlace').load(url, function () {
        $('#modal_GenerarEnlace').modal('show');

    });
}


//Modal Anular Evento
$(document).ready(function () {
    $.ajaxSetup({ cache: false });
    $("a[data-anularevento]").on("click", function (e) {
        openmodalanularevento(this.href);
        return false;
    });
    $('#modal_AnularEvento').on('hidden.bs.modal', function () {
        $('#contentModalAnularEvento').html('');
    })
});

function openmodalanularevento(url) {
    $('#contentModalAnularEvento').load(url, function () {
        $('#modal_AnularEvento').modal('show');

    });
}


//Modal Envio Correo
$(document).ready(function () {
    $.ajaxSetup({ cache: false });
    $("a[data-enviocorreo]").on("click", function (e) {
        openmodalenviocorreo(this.href);
        return false;
    });
    $('#modal_EnvioCorreo').on('hidden.bs.modal', function () {
        $('#contentModalEnvioCorreo').html('');
    })
});

function openmodalenviocorreo(url) {
    $('#contentModalEnvioCorreo').load(url, function () {
        $('#modal_EnvioCorreo').modal('show');

    });
}