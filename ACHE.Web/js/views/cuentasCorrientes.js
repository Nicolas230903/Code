function detalleCuentaCorriente(id, razonSocial) {
    $("#hdnID").val(id);
    $("#hdnRazonSocial").val(razonSocial);
    CambiarReporte();
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

function cambiarTipo(tipo) {
    window.location.href = "/cuentasCorrientes.aspx?tipo=" + tipo;
}

function configFilters() {
    $("#txtNroDocumento").numericInput();

    if ($("#hdnTipo").val() == "C") {
        $("#rCtaCteClienteMain").attr("checked", true)
    } else {
        $("#rCtaCteProvMain").attr("checked", true)
    }

    $("#txtRazonSocial, #txtNroDocumento").keypress(function (event) {
        var keycode = (event.keyCode ? event.keyCode : event.which);
        if (keycode == '13') {
            resetearPagina();
            filtrar();
            return false;
        }
    });

}

function CambiarReporte() {

    if ($("input[name='rCtaCte']:checked").attr('value') == "0") {
        $("#rCtaCteProv").attr("checked", true)
    } else {
        $("#rCtaCteCliente").attr("checked", true)
    }

    if ($("#chkDeudaPorEDM").is(":checked")) {
        $("#lblVista").html("Deuda por EDM");
    } else {
        $("#lblVista").html("Deuda por PDV");
    }

    $("#lblRazonSocial").html($("#hdnRazonSocial").val());

    var info = "{ idPersona: " + parseInt($("#hdnID").val())
        + ", verComo: " + $("input[name='rCtaCte']:checked").attr('value')
        + ", saldoPendiente: " + $("#chkSaldoPendiente").is(":checked")
        + ", deudaPorEDM: " + $("#chkDeudaPorEDM").is(":checked")
        + "}";

    $.ajax({
        type: "POST",
        url: "/common.aspx/getResultsCtaCte",
        data: info,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data, text) {
            if (data.d.TotalPage > 0) {
                if ($("input[name='rCtaCte']:checked").attr('value') == "1") {
                    $("#thfecha").html("Fecha Cobro");
                    $("#thImporte").html("Cobrado");
                } else {
                    $("#thfecha").html("Fecha Pago");
                    $("#thImporte").html("Pagado");
                }
            }

            $("#bodyDetalle").html("");
            // Render using the template
            if (data.d.Items.length > 0)
                $("#resultTemplateDetalle").tmpl({ results: data.d.Items }).appendTo("#bodyDetalle");
            else
                $("#noResultTemplateDetalle").tmpl({ results: data.d.Items }).appendTo("#bodyDetalle");

            $('#modalDetalle').modal('show');
        },
        error: function (response) {
            var r = jQuery.parseJSON(response.responseText);
            $("#msgError").html(r.Message);
            $("#divError").show();
            $("#divOk").hide();
            $('html, body').animate({ scrollTop: 0 }, 'slow');
        }
    });
}

function ImprimirDetalleCuentaCorriente() {

    if ($("input[name='rCtaCte']:checked").attr('value') == "0") {
        $("#rCtaCteProv").attr("checked", true)
    } else {
        $("#rCtaCteCliente").attr("checked", true)
    }

    var info = "{ idPersona: " + parseInt($("#hdnID").val())
        + ", verComo: " + $("input[name='rCtaCte']:checked").attr('value')
        + ", saldoPendiente: " + $("#chkSaldoPendiente").is(":checked")
        + ", deudaPorEDM: " + $("#chkDeudaPorEDM").is(":checked")
        + "}";

    $.ajax({
        type: "POST",
        url: "/common.aspx/imprimirResultsCtaCte",
        data: info,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data, text) {
            var pathFile = "/files/detalleCuentaCorriente/" + $("#hdnIDUsuario").val() + "/" + data.d + "#view=FitH,top";
            $("#ifrPdf").attr("src", pathFile);
            $("#modalPdf").modal("show");
        },
        error: function (response) {
            var r = jQuery.parseJSON(response.responseText);
            $("#msgError").html(r.Message);
            $("#divError").show();
            $("#divOk").hide();
            $('html, body').animate({ scrollTop: 0 }, 'slow');
        }
    });
}


function exportarResumen() {
    resetearExportacionResumen();

    $("#imgLoadingExportacionResumen").show();
    $("#divIconoDescargarExportacionResumen").hide();

    var info = "{ condicion: '" + $("#txtRazonSocial").val()
        + "', tipo: '" + $("#hdnTipo").val()
        + "', saldoPendiente: " + $("#chkSaldoPendiente").is(":checked")
        + ", deudaPorEDM: " + $("#chkDeudaPorEDM").is(":checked")
        + "}";

    $.ajax({
        type: "POST",
        url: "cuentasCorrientes.aspx/exportResumen",
        data: info,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data, text) {
            if (data.d != "") {

                $("#divError").hide();
                $("#imgLoadingExportacionResumen").hide();
                $("#lnkDownloadExportacionResumen").show();
                $("#lnkDownloadExportacionResumen").attr("href", data.d);
                $("#lnkDownloadExportacionResumen").attr("download", data.d);
            }
        },
        error: function (response) {
            var r = jQuery.parseJSON(response.responseText);
            $("#msgError").html(r.Message);
            $("#divError").show();
            $("#divOk").hide();
            $('html, body').animate({ scrollTop: 0 }, 'slow');
            resetearExportacionResumen();
        }
    });
}

function resetearExportacionResumen() {
    $("#imgLoadingExportacionResumen, #lnkDownloadExportacionResumen").hide();
    $("#divIconoDescargarExportacionResumen").show();
}


function exportarDetalle() {
    resetearExportacionDetalle();

    $("#imgLoadingExportacionDetalle").show();
    $("#divIconoDescargarExportacionDetalle").hide();

    var info = "{ condicion: '" + $("#txtRazonSocial").val()
        + "', tipo: '" + $("#hdnTipo").val()
        + "', saldoPendiente: " + $("#chkSaldoPendiente").is(":checked")
        + ", deudaPorEDM: " + $("#chkDeudaPorEDM").is(":checked")
        + "}";

    $.ajax({
        type: "POST",
        url: "cuentasCorrientes.aspx/exportDetalle",
        data: info,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data, text) {
            if (data.d != "") {

                $("#divError").hide();
                $("#imgLoadingExportacionDetalle").hide();
                $("#lnkDownloadExportacionDetalle").show();
                $("#lnkDownloadExportacionDetalle").attr("href", data.d);
                $("#lnkDownloadExportacionDetalle").attr("download", data.d);
            }
        },
        error: function (response) {
            var r = jQuery.parseJSON(response.responseText);
            $("#msgError").html(r.Message);
            $("#divError").show();
            $("#divOk").hide();
            $('html, body').animate({ scrollTop: 0 }, 'slow');
            resetearExportacionDetalle();
        }
    });
}

function resetearExportacionDetalle() {
    $("#imgLoadingExportacionDetalle, #lnkDownloadExportacionDetalle").hide();
    $("#divIconoDescargarExportacionDetalle").show();
}


function RealizarCobranza() {
    window.location.href = "/cobranzase.aspx";
}

function filtrar() {

    $.blockUI({ message: $('#divEspera') });
    document.body.style.cursor = 'wait';
    $("#divError").hide();
    $("#resultsContainer").html("");
    var currentPage = parseInt($("#hdnPage").val());

    var info = "{ condicion: '" + $("#txtRazonSocial").val()
        + "', tipo: '" + $("#hdnTipo").val()
        + "', saldoPendiente: " + $("#chkSaldoPendiente").is(":checked")
        + ", deudaPorEDM: " + $("#chkDeudaPorEDM").is(":checked")
        + ", page: " + currentPage + ", pageSize: " + PAGE_SIZE
        + "}";

    $.ajax({
        type: "POST",
        url: "cuentasCorrientes.aspx/getResultsCuenta",
        data: info,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data, text) {
            $.unblockUI();
            document.body.style.cursor = 'default';
            if (data.d.TotalPage > 0) {
                $("#divPagination,#divDownloadExportacionResumen,#divDownloadExportacionDetalle").show();

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
                $("#divPagination,#divDownloadExportacionResumen,#divDownloadExportacionDetalle").hide();
                $("#msjResultados").html("");
            }

            // Render using the template
            if (data.d.Items.length > 0)
                $("#resultTemplate").tmpl({ results: data.d.Items }).appendTo("#resultsContainer");
            else
                $("#noResultTemplate").tmpl({ results: data.d.Items }).appendTo("#resultsContainer");
        },
        error: function (response) {
            $.unblockUI();
            document.body.style.cursor = 'default';
            var r = jQuery.parseJSON(response.responseText);
            $("#msgError").html(r.Message);
            $("#divError").show();
            $("#divOk").hide();
            $('html, body').animate({ scrollTop: 0 }, 'slow');
        }
    });
    
}

function configFormSinDatos() {
    if ($("#hdnTipo").val() == "C") {
        $("#hTitulo").text("No hay clientes con saldo");
    }
    else {
        $("#hTitulo").text("No hay proveedores con saldo");
    }
}