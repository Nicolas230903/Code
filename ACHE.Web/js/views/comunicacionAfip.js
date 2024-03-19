function configForm() {    
    obtenerItemsNoLeidos();
    obtenerItemsLeidos();
}

function obtenerComunicacionesAfip() {
    $.ajax({
        type: "POST",
        url: "/common.aspx/consultarComunicacionesAfip",
        //data: info,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data, text) {
        },
        error: function (response) {
        }
    });
}

function obtenerItemsNoLeidos() {

    $.ajax({
        type: "GET",
        url: "comunicacionAfip.aspx/obtenerItems?leidos=0",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            if (data != null) {
                $("#resultsContainerNoLeidos").html(data.d);
            }
        }
    });
}

function obtenerItemsLeidos() {

    $.ajax({
        type: "GET",
        url: "comunicacionAfip.aspx/obtenerItems?leidos=1",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            if (data != null) {
                $("#resultsContainerLeidos").html(data.d);
            }
        }
    });
}

$("#checkAll").change(function () {
    $('.chkTodos').not(this).prop('checked', this.checked);
});

function marcarLeidosSeleccionados() {
    var Contador = 0;
    var idSeleccionados = [];
    $('#tablaComunicacionesAfipNoLeidos tr').each(function () {
        if ($(this).find("td").eq(9).find("input").prop('checked')) {
            Contador = + 1;  
            var idOperacion = $(this).find("td").eq(1).html();
            idSeleccionados = idSeleccionados.concat(idOperacion);
        }
    });
    if (Contador > 0) {
        $.ajax({
            type: "POST",
            url: "comunicacionAfip.aspx/marcarComoLeidas",
            data: "{ id: " + JSON.stringify(idSeleccionados) + "}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data, text) {
                $('#divOk').show();
                $("#divError").hide();
                setTimeout('document.location.reload()', 2000); 
            },
            error: function (response) {
                var r = jQuery.parseJSON(response.responseText);
                $("#msgError").html(r.Message);
                $("#divError").show();
                $("#divOk").hide();
            }
        });
    }    
}

function marcarLeidosTodos() {
    var Contador = 0;
    var idAll = [];
    $('#tablaComunicacionesAfipNoLeidos tr').each(function () {
        var idOperacion = $(this).find("td").eq(1).html();
        idAll.push(idOperacion);
        Contador = + 1;  
    });
    if (Contador > 0) {
        $.ajax({
            type: "POST",
            url: "comunicacionAfip.aspx/marcarComoLeidas",
            data: "{ id: " + JSON.stringify(idAll) + "}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data, text) {
                $('#divOk').show();
                $("#divError").hide();
                setTimeout('document.location.reload()', 2000);
            },
            error: function (response) {
                var r = jQuery.parseJSON(response.responseText);
                $("#msgError").html(r.Message);
                $("#divError").show();
                $("#divOk").hide();
            }
        });
    }
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

function descargarAdjunto(id) {
    $('#divOk').hide();
    $("#divError").hide();

    var info = "{ id: '" + id + "'}";
    $.ajax({       
        type: "POST",
        url: "comunicacionAfip.aspx/descargarAdjunto",
        data: info,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data, text) {
            if (data.d != "") {
                var bytes = new Uint8Array(data.d.Contenido); // pass your byte response to this constructor
                var blob = new Blob([bytes], { type: "application/pdf" });// change resultByte to bytes
                var link = document.createElement('a');
                link.href = window.URL.createObjectURL(blob);
                link.download = data.d.NombreArchivo;
                link.click();
            }
        },
        error: function (response) {
            var r = jQuery.parseJSON(response.responseText);
            $("#msgError").html(r.Message);
            $("#divError").show();
            $("#divOk").hide();
        }
    });
}
