/*** SEARCH ***/

function configFilters() {
    Common.obtenerConceptosCodigoyNombre("ddlConcepto", 1, false);
}

function mostrarPagAnterior() {
    var paginaActual = parseInt($("#hdnPage").val());
    paginaActual--;
    $("#hdnPage").val(paginaActual);
    filtrar();
}

function mostrarPagProxima() {
    var paginaActual = parseInt($("#hdnPage").val());
    paginaActual++;
    $("#hdnPage").val(paginaActual);
    filtrar();
}

function resetearPagina() {
    $("#hdnPage").val("1");
}

function filtrar() {

    $("#divError").hide();
    $("#resultsContainer").html("");
    var currentPage = parseInt($("#hdnPage").val());

    var info = "{ idConcepto: " + $("#ddlConcepto").val()
                + ", page: " + currentPage + ", pageSize: " + PAGE_SIZE
                + "}";

    $.ajax({
        type: "POST",
        url: "stock-detalle.aspx/getResults",
        data: info,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data, text) {
            if (data.d.TotalPage > 0) {
                $("#divPagination").show();

                $("#lnkNextPage, #lnkPrevPage").removeAttr('disabled')
                if (data.d.TotalPage == 1)
                    $("#lnkNextPage, #lnkPrevPage").attr('disabled', "disabled")
                else if (currentPage == data.d.TotalPage)
                    $("#lnkNextPage").attr("disabled", "disabled");
                else if (currentPage == 1)
                    $("#lnkPrevPage").attr("disabled", "disabled");

                var aux = (currentPage * PAGE_SIZE);
                if (aux > data.d.TotalItems)
                    aux = data.d.TotalItems;
                $("#msjResultados").html("Mostrando " + ((currentPage * PAGE_SIZE) - PAGE_SIZE + 1) + " - " + aux + " de " + data.d.TotalItems);
            }
            else {
                $("#divPagination").hide();
                $("#msjResultados").html("");
            }

            // Render using the template
            if (data.d.Items.length > 0)
                $("#resultTemplate").tmpl({ results: data.d.Items }).appendTo("#resultsContainer");
            else
                $("#noResultTemplate").tmpl({ results: data.d.Items }).appendTo("#resultsContainer");
        },
        error: function (response) {
            var r = jQuery.parseJSON(response.responseText);
            alert(r.Message);
        }
    });

    resetearExportacion();
}

function verTodos() {
    $("#ddlEstado").val("");
    filtrar();
}

function exportar() {
    resetearExportacion();
    $("#imgLoading").show();
    $("#divIconoDescargar").hide();

    var info = "{ idConcepto: " + $("#ddlConcepto").val()
                + "}";

    $.ajax({
        type: "POST",
        url: "stock-detalle.aspx/export",
        data: info,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data, text) {
            if (data.d != "") {
                
                $("#divError").hide();
                $("#imgLoading").hide();
                $("#lnkDownload").show();
                $("#lnkDownload").attr("href", data.d);
                $("#lnkDownload").attr("download", data.d);
            }
        },
        error: function (response) {
            var r = jQuery.parseJSON(response.responseText);
            $("#msgError").html(r.Message);
            $("#divError").show();
            $('html, body').animate({ scrollTop: 0 }, 'slow');
            resetearExportacion();
        }
    });
}

function resetearExportacion() {
    $("#imgLoading, #lnkDownload").hide();
    $("#divIconoDescargar").show();
}