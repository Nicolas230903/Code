var CC = {
    configFilters: function () {
        $(".select2").select2({ width: '100%', allowClear: true });

        Common.obtenerPersonas("ddlPersona", "", true);
        $("<option/>").attr("value", -1).text("TODOS").appendTo($("#ddlPersona"));
        $("#ddlPersona").val(-1);
        $("#ddlPersona").trigger("change");

    },
    filtrar: function () {
        document.body.style.cursor = 'wait';
        $("#divError").hide();
        $("#resultsContainer").html("");
        var currentPage = parseInt($("#hdnPage").val());

        var idPersona = 0;
        if ($("#ddlPersona").val() != "")
            idPersona = parseInt($("#ddlPersona").val());

        if (idPersona == 0) {
            $("#msgError").html("Debe seleccionar un cliente/proveedor");
            $("#divError").show();
            $('html, body').animate({ scrollTop: 0 }, 'slow');
        }
        else {
            var info = "{ idPersona: " + idPersona
                + ", verComo: " + $("input[name='rCtaCte']:checked").attr('value')
                + ", saldoPendiente: " + $("#chkSaldoPendiente").is(":checked")
                + ", deudaPorEDM: false "
                + "} ";

            $.ajax({
                type: "POST",
                url: "/common.aspx/getResultsCtaCte",
                data: info,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data, text) {
                    document.body.style.cursor = 'default';
                    if (data.d.TotalPage > 0) {

                        var aux = (currentPage * PAGE_SIZE);
                        if (aux > data.d.TotalItems)
                            aux = data.d.TotalItems;

                        if ($("input[name='rCtaCte']:checked").attr('value') == "1") {
                            $("#thfecha").html("Fecha Cobro");
                            $("#thImporte").html("Cobrado");
                        } else {
                            $("#thfecha").html("Fecha Pago");
                            $("#thImporte").html("Pagado");
                        }
                    }
                    else {
                        //$("#msjResultados").html("");
                    }

                    // Render using the template
                    if (data.d.Items.length > 0)
                        $("#resultTemplate").tmpl({ results: data.d.Items }).appendTo("#resultsContainer");
                    else
                        $("#noResultTemplate").tmpl({ results: data.d.Items }).appendTo("#resultsContainer");
                },
                error: function (response) {
                    document.body.style.cursor = 'default';
                    var r = jQuery.parseJSON(response.responseText);
                    $("#msgError").html(r.Message);
                    $("#divError").show();
                    $('html, body').animate({ scrollTop: 0 }, 'slow');
                }
            });            
        }
        CC.resetearExportacion();
    },

    exportar: function () {
        CC.resetearExportacion();

        $("#imgLoading").show();
        $("#divIconoDescargar").hide();

        if ($("input[name='rCtaCte']:checked").attr('value') == "0") {
            CC.exportarDetalleProveedor();
        }
        else {
            CC.exportarDetalleCliente();
        }
    },
    exportarDetalleProveedor: function () {

        $("#imgLoading").show();
        $("#divIconoDescargar").hide();

        var idPersona = 0;
        if ($("#ddlPersona").val() != null && $("#ddlPersona").val() != "")
            idPersona = parseInt($("#ddlPersona").val());

        var info = "{ idPersona: " + idPersona
            + ", saldoPendiente: " + $("#chkSaldoPendiente").is(":checked")
            + ", deudaPorEDM: false "
            + "} ";

        $.ajax({
            type: "POST",
            url: "/common.aspx/exportarProveedor",
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
            }
        });
    },
    exportarDetalleCliente: function () {
        $("#imgLoading").show();
        $("#divIconoDescargar").hide();

        var idPersona = 0;
        if ($("#ddlPersona").val() != null && $("#ddlPersona").val() != "")
            idPersona = parseInt($("#ddlPersona").val());

        var info = "{ idPersona: " + idPersona
            + ", saldoPendiente: " + $("#chkSaldoPendiente").is(":checked")
            + ", deudaPorEDM: false "
            + "} ";

        $.ajax({
            type: "POST",
            url: "/common.aspx/exportarCliente",
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
            }
        });
    },
    resetearExportacion: function () {
        $("#imgLoading, #lnkDownload").hide();
        $("#divIconoDescargar").show();
    },
    changePersona: function () {
        var idPersona = 0;
        if ($("#ddlPersona").val() != "")
            idPersona = parseInt($("#ddlPersona").val());

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

                },
                error: function (response) {
                    var r = jQuery.parseJSON(response.responseText);
                    alert(r.Message);
                }
            });
        }
    },
}
