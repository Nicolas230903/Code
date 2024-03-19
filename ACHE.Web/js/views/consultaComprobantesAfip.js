function configForm() {    
    Common.obtenerPuntosDeVenta("ddlPuntoVenta");
    Common.obtenerTipoComprobanteAfip("ddlTipo", "", false);
    obtenerConsultaComprobantesAfip();
}

function limpiarControles() {
    $("#divErrorConsultaComprobantesAfip").hide();
    $("#divErrorRecuperarComprobanteAfip").hide();
    $("#divRespuestaRecuperarComprobanteAfip").hide();
}

function obtenerConsultaComprobantesAfip() {
    limpiarControles();

    $.blockUI({ message: $('#divEspera') });
    document.body.style.cursor = 'wait';

    $.ajax({
        type: "POST",
        url: "/common.aspx/consultarComprobantesAfip",
        //data: info,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            $.unblockUI();
            document.body.style.cursor = 'default';
            if (data != null) {
                $("#resultsContainerConsultaComprobantesAfip").html(data.d);
            }
        },
        error: function (response) {
            $.unblockUI();
            document.body.style.cursor = 'default';
            var r = jQuery.parseJSON(response.responseText);
            $("#msgErrorConsultaComprobantesAfip").html(r.Message);
            $("#divErrorConsultaComprobantesAfip").show();
        }
    });
}

function recuperarComprobanteAfip() {
    limpiarControles();

    if ($("#txtNumeroComprobante").val() == "") {
        $("#msgErrorRecuperarComprobanteAfip").html("Debe completar el campo número de comprobante");
        $("#divErrorRecuperarComprobanteAfip").show();
        $('html, body').animate({ scrollTop: 0 }, 'slow');
        return;
    } 

    $.blockUI({ message: $('#divEspera') });
    document.body.style.cursor = 'wait';

    var info = "{idTipo: '" + $("#ddlTipo").val()
        + "', idPuntoVenta: " + $("#ddlPuntoVenta").val()
        + " , numeroComprobante: '" + $("#txtNumeroComprobante").val()
        + "'}";

    $.ajax({
        type: "POST",
        url: "/common.aspx/recuperarComprobanteAfip",
        data: info,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            $.unblockUI();
            document.body.style.cursor = 'default';
            if (data != null) {
                $("#divRespuestaRecuperarComprobanteAfip").show();
                $("#resultsContainerRecuperarComprobanteAfip").html(data.d);
            }
        },
        error: function (response) {
            $.unblockUI();
            document.body.style.cursor = 'default';
            var r = jQuery.parseJSON(response.responseText);
            $("#msgErrorRecuperarComprobanteAfip").html(r.Message);
            $("#divErrorRecuperarComprobanteAfip").show();
        }
    });
}

function tableToArray(table) {
    var result = []
    var rows = table.rows;
    var cells, t;

    // Iterate over rows
    for (var i = 0, iLen = rows.length; i < iLen; i++) {
        cells = rows[i].cells;
        t = [];

        // Iterate over cells
        for (var j = 0, jLen = cells.length; j < jLen; j++) {
            t.push(cells[j].textContent);
        }
        result.push(t);
    }
    return result;
}
