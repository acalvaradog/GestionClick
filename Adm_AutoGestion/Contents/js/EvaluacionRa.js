$(document).ready(function () {
    //$('#Indicador').change(function () {
    //    var indicadorId = $(this).val();

    //    if (indicadorId) {
    //        var indicadores = @Html.Raw(Json.Encode(ViewBag.Indicador));
    //        var indicadorSeleccionado = indicadores.find(x => x.Id == indicadorId);

    //        if (indicadorSeleccionado) {
    //            $('#CriterioId').val(indicadorSeleccionado.CriterioId);
    //            $('#Evidencia').val(indicadorSeleccionado.Evidencia);
    //        } else {
    //            $('#CriterioId').val(''); CriterioId
    //            $('#Evidencia').val('');
    //        }
    //    } else {
    //        $('#criterioInput').val('');
    //        $('#evidenciaInput').val('');
    //    }
    //});


    

});

function guardarEvaluacion() {
    // Deshabilitar el botón y mostrar un indicador de carga (opcional)
    const btnGuardar = $('#btnGuardarEv');
    btnGuardar.prop('disabled', true);

    const empleadoId = $('#Id').val();
    const evaluadorId = $('#EvaluadorId').val();
    const periodo = $('#Periodo').val();
    const retroalimentacion = $('#retroalimentacion').val();
    const planMejora = $('#planmejora').val();

    const detalles = $('#Tabla_Evaluacion tbody tr').map(function () {
        const inputs = $(this).find('input');
        return {
            IndicadorId: inputs.eq(0).val(),
            BaseNumerador: parseInt(inputs.eq(1).val()),
            BaseDenominador: parseInt(inputs.eq(2).val()),
            IndicadorNumerador: parseInt(inputs.eq(3).val()),
            IndicadorDenominador: parseInt(inputs.eq(4).val()),
            Porcentaje: parseFloat(inputs.eq(5).val())
        };
    }).get();

    const datos = {
        EmpleadoId: empleadoId,
        EvaluadorId: evaluadorId,
        PeriodoEvaluacion: periodo,
        FechaRegistro : "",
        Retroalimentacion: retroalimentacion,
        PlandeMejora: planMejora,
        PuntajeFinal:"",
        EvaluacionDetalle: detalles
    };

    $.ajax({
        url: "GuardarEvaluacion",
        type: "POST",
        contentType: "application/json",
        data: { jsonData: JSON.stringify(datos) },
        success: function (response) {
            btnGuardar.prop('disabled', false);
            alert('Evaluación guardada correctamente');
            $('#formularioEvaluacion')[0].reset();
        },
        error: function (error) {
            btnGuardar.prop('disabled', false);
            alert('Error al guardar la evaluación' + JSON.stringify(error));
        }
    });
}
/*//2. Adjuntar el Evento al Botón:*/

//Ahora, vamos a adjuntar la función guardarEvaluacion como un controlador de eventos al botón #btnGuardarEv:

//JavaScript

//$(document).ready(function () { //asegura que el DOM este cargado
//    $('#btnGuardarEv').click(guardarEvaluacion);
//});



function SaveEvaluacion() {

    try {

        var frmData = new FormData();

        const empleadoId = $('#Id').val();
        const evaluadorId = $('#EvaluadorId').val();
        const periodo = $('#Periodo').val();
        const retroalimentacion = $('#retroalimentacion').val();
        const planMejora = $('#planmejora').val();
        const puntajeTotal = $('#puntajetotal').val();

        if (empleadoId == '' || empleadoId == null) {

            Swal.fire({
                title: "Oops...",
                text: "Contrato es Obligatorio",
                icon: "error"
            });

        } else {

            frmData.append("EmpleadoId", empleadoId);
        }


        if (evaluadorId == '' || evaluadorId == null) {

            Swal.fire({
                title: "Oops...",
                text: "Contrato es Obligatorio",
                icon: "error"
            });

        } else {

            frmData.append("EvaluadorId", evaluadorId);
        }


        if (periodo == '' || periodo == null) {

            Swal.fire({
                title: "Oops...",
                text: "Contrato es Obligatorio",
                icon: "error"
            });

        } else {

            frmData.append("PeriodoEvaluacion", periodo);
        }



        if (retroalimentacion == '' || retroalimentacion == null) {

            Swal.fire({
                title: "Oops...",
                text: "Retroalimentacion  es Obligatorio",
                icon: "error"
            });

        } else {

            frmData.append("Retroalimentacion", retroalimentacion);
        }


        if (planMejora == '' || planMejora == null) {

            Swal.fire({
                title: "Oops...",
                text: "Plan de Mejora es Obligatorio",
                icon: "error"
            });

        } else {

            frmData.append("PlandeMejora", planMejora);
        }


        if (puntajeTotal == '' || puntajeTotal == null) {

            Swal.fire({
                title: "Oops...",
                text: "Puntaje Total es Obligatorio",
                icon: "error"
            });

        } else {

            frmData.append("PuntajeFinal", puntajeTotal);
        }

     


      
        // Anexo Detalle evaluacion

        // Recorrer las filas de la tabla y agregar datos a FormData
        $('#Tabla_Evaluacion tbody tr').each(function (index) {
            const fila = $(this);
            frmData.append(`DetallesEvaluacion[${index}].IndicadorId`, fila.find('td:eq(1)').text());
            frmData.append(`DetallesEvaluacion[${index}].BaseNumerador`, parseInt(fila.find('td:eq(3)').text()) || 0);
            frmData.append(`DetallesEvaluacion[${index}].BaseDenominador`, parseInt(fila.find('td:eq(4)').text()) || 0);
            frmData.append(`DetallesEvaluacion[${index}].IndicadorNumerador`, parseInt(fila.find('td:eq(5)').text()) || 0);
            frmData.append(`DetallesEvaluacion[${index}].IndicadorDenominador`, parseInt(fila.find('td:eq(6)').text()) || 0);
            frmData.append(`DetallesEvaluacion[${index}].Porcentaje`, parseFloat(fila.find('td:eq(7)').text()) || 0);
        });


        console.log(FormData);

        $.ajax({

            url: "SaveEvaluacion",
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
                    Swal.fire({
                        /* title: "Oops...",*/
                        text: json.respuesta,
                        icon: "success"
                    });
                    if (json.isRedirect) {
                        setTimeout(function () { window.location.href = json.redirectUrl }, 2500);

                    }
                } else {

                    Swal.fire({
                        /* title: "Oops...",*/
                        text: json.respuesta,
                        icon: "error"
                    });

                }

            },
            error: function (xhr, status, error) {
                console.log(error); Swal.fire({
                    /*title: "Oops...",*/
                    text: error,
                    icon: "error"
                });
            }

        });



    } catch (err) {
        Swal.fire({
            /*title: "Oops...",*/
            text: err,
            icon: "error"
        });
    }



};
