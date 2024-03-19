/*** SEARCH ***/

function configFilters() {
    $(".select2").select2({ width: '100%', allowClear: true });

    Common.obtenerPersonas("ddlPersona", "", true);

    $("#txtFechaDesde, #txtFechaHasta").keypress(function (event) {
        var keycode = (event.keyCode ? event.keyCode : event.which);
        if (keycode == '13') {
            resetearPagina();
            filtrar();
            return false;
        }
    });

    // Date Picker
    Common.configDatePicker();
    Common.configFechasDesdeHasta("txtFechaDesde", "txtFechaHasta");

    // Validation with select boxes
    $("#frmSearch").validate({
        highlight: function (element) {
            jQuery(element).closest('.form-group').removeClass('has-success').addClass('has-error');
        },
        success: function (element) {
            jQuery(element).closest('.form-group').removeClass('has-error');
        },
        errorElement: 'span',
        errorClass: 'help-block',
        errorPlacement: function (error, element) {
            if (element.parent('.input-group').length) {
                error.insertAfter(element.parent());
            } else {
                error.insertAfter(element);
            }
        }
    });
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

    if ($('#frmSearch').valid()) {
        $("#resultsContainer").html("");

        var currentPage = parseInt($("#hdnPage").val());

        var idPersona = 0;
        if ($("#ddlPersona").val() != null && $("#ddlPersona").val() != "")
            idPersona = parseInt($("#ddlPersona").val());

        var info = "{ idPersona: " + idPersona
                   + ", fechaDesde: '" + $("#txtFechaDesde").val()
                   + "', fechaHasta: '" + $("#txtFechaHasta").val()
                   + "', tipo: '" + $("#ddlTipoPercepcion").val()
                   + "', impuesto: '" + $("#ddlImpuesto").val()
                   + "', page: " + currentPage + ", pageSize: " + PAGE_SIZE
                   + "}";


        $.ajax({
            type: "POST",
            url: "retenciones.aspx/getResults",
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
}

function verTodos() {
    $("#txtFechaDesde, #txtFechaHasta").val("");
    $("#ddlPersona").val("").trigger("change");
    filtrar();
}

function exportar() {
    resetearExportacion();
    $("#imgLoading").show();
    $("#divIconoDescargar").hide();

    var idPersona = 0;
    if ($("#ddlPersona").val() != "")
        idPersona = parseInt($("#ddlPersona").val());

    var info = "{ idPersona: " + idPersona
               + ", fechaDesde: '" + $("#txtFechaDesde").val()
               + "', fechaHasta: '" + $("#txtFechaHasta").val()
               + "', tipo: '" + $("#ddlTipoPercepcion").val()
               + "', impuesto: '" + $("#ddlImpuesto").val()
               + "'}";

    $.ajax({
        type: "POST",
        url: "retenciones.aspx/export",
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

function imprimirRetencionGanancia(id) {

    var info = "{ idPagoRetencion: " + id + "}";

    $.ajax({
        type: "POST",
        url: "retenciones.aspx/imprimirRetencionGananciaPorId",
        data: info,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data, text) {
            $("#divError").hide();
            $('html, body').animate({ scrollTop: 0 }, 'slow');

            $("#ifrPdf").attr("src", "/files/retencionGanancia/" + data.d + "#zoom=100&view=FitH,top");
            $("#lnkDescargar").attr("href", "/files/retencionGanancia/" + data.d);

            $('#modalPdf').modal('show');
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