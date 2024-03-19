var SaldosCC = {
    /*** Modal ***/
    verDetalle: function (id, nombre) {

        $("#divError").hide();

        if (nombre != undefined) {
            $("#hdnNombrePersonaDetalle").val(nombre);
            $("#titDetalle").html("Detalle de " + nombre);
        }
        if (id != undefined) {
            $("#hdnIDPersonaDetalle").val(id);
            SaldosCC.obtenerTipoPersona(id);
        }
        else {
            SaldosCC.obtenerDetalle();
        }
        SaldosCC.resetearExportacionDetalle();
    },
    obtenerDetalle: function () {
        var info = "{ idPersona: " + $("#hdnIDPersonaDetalle").val()
            + ", verComo: " + $("input[name='rCtaCte']:checked").attr('value') + "}";

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
                alert(r.Message);
            }
        });
    },
    obtenerTipoPersona: function (idPersona) {
        if (idPersona > 0) {
            var info = "{ idPersona: " + idPersona + "}";

            $.ajax({
                type: "POST",
                url: "/common.aspx/obtenerTipoPersona",
                data: info,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data, text) {

                    if (data.d == "C") {
                        $("#rCtaCteCliente").attr('checked', "checked")
                    }
                    else {
                        $("#rCtaCteProv").attr('checked', "checked")
                    }
                    SaldosCC.obtenerDetalle();
                },
                error: function (response) {
                    var r = jQuery.parseJSON(response.responseText);
                    alert(r.Message);
                }
            });
        }
    },
    exportarDetalle: function () {
        SaldosCC.resetearExportacionDetalle();

        $("#imgLoadingDetalle").show();
        $("#divIconoDescargarDetalle").hide();

        var info = "{ idPersona: " + parseInt($("#hdnIDPersonaDetalle").val()) + "}";

        $.ajax({
            type: "POST",
            url: "/common.aspx/exportarCliente",
            data: info,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data, text) {
                if (data.d != "") {
                    $("#divError").hide();
                    $("#imgLoadingDetalle").hide();
                    $("#lnkDownloadDetalle").show();
                    $("#lnkDownloadDetalle").attr("href", data.d);
                    $("#lnkDownloadDetalle").attr("download", data.d);
                }
            },
            error: function (response) {
                var r = jQuery.parseJSON(response.responseText);
                $("#msgError").html(r.Message);
                $("#divError").show();
                $('html, body').animate({ scrollTop: 0 }, 'slow');
                SaldosCC.resetearExportacion();
            }
        });
    },
    resetearExportacionDetalle: function () {

        $("#imgLoadingDetalle, #lnkDownloadDetalle").hide();
        $("#divIconoDescargarDetalle").show();
    },
    exportarDetalleProveedor: function () {
        SaldosCC.resetearExportacionDetalle();

        $("#imgLoadingDetalle").show();
        $("#divIconoDescargarDetalle").hide();

        var info = "{ idPersona: " + parseInt($("#hdnIDPersonaDetalle").val()) + "}";

        $.ajax({
            type: "POST",
            url: "/common.aspx/exportarProveedor",
            data: info,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data, text) {
                if (data.d != "") {
                    $("#divError").hide();
                    $("#imgLoadingDetalle").hide();
                    $("#lnkDownloadDetalle").show();
                    $("#lnkDownloadDetalle").attr("href", data.d);
                    $("#lnkDownloadDetalle").attr("download", data.d);
                }
            },
            error: function (response) {
                var r = jQuery.parseJSON(response.responseText);
                $("#msgError").html(r.Message);
                $("#divError").show();
                $('html, body').animate({ scrollTop: 0 }, 'slow');
                SaldosCC.resetearExportacion();
            }
        });
    },
    exportar: function () {

        if ($("input[name='rCtaCte']:checked").attr('value') == "0") {
            SaldosCC.exportarDetalleProveedor();
        }
        else {
            SaldosCC.exportarDetalle();
        }

    },
    /*** SEARCH ***/
    configFilters: function () {
        $(".select2").select2({ width: '100%', allowClear: true });

        Common.obtenerPersonas("ddlPersona", "", true);
    },
    filtrar: function () {
        $("#divError").hide();
        $("#resultsContainer").html("");
        var currentPage = parseInt($("#hdnPage").val());

        var idPersona = 0;
        if ($("#ddlPersona").val() != null && $("#ddlPersona").val() != "")
            idPersona = parseInt($("#ddlPersona").val());

        var info = "{ idPersona: " + idPersona
                   + ", page: " + currentPage + ", pageSize: " + PAGE_SIZE
                   + "}";

        $.ajax({
            type: "POST",
            url: "/modulos/reportes/saldos-cc.aspx/getResults",
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
        SaldosCC.resetearExportacion();
    },
    verTodos: function () {
        $("#ddlPersona").val("").trigger("change");
        SaldosCC.filtrar();
    },
    resetearExportacion: function () {
        $("#imgLoading, #lnkDownload").hide();
        $("#divIconoDescargar").show();
    },
    mostrarPagAnterior: function () {
        var paginaActual = parseInt($("#hdnPage").val());
        paginaActual--;
        $("#hdnPage").val(paginaActual);
        SaldosCC.filtrar();
    },
    mostrarPagProxima: function () {
        var paginaActual = parseInt($("#hdnPage").val());
        paginaActual++;
        $("#hdnPage").val(paginaActual);
        SaldosCC.filtrar();
    },
    resetearPagina: function () {
        $("#hdnPage").val("1");
    }
}