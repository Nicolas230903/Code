function configForm() {

    $(".select2").select2({ width: '100%', allowClear: true });        
    Common.obtenerPersonas("ddlFacturaDeVentaPersona", "", true);
    Common.configDatePicker();
    Common.obtenerCodigosFacturaAutomatica("ddlFacturaDeVentaCodigoProceso", "", true);
    Common.obtenerPeriodosPdv("ddlPeriodo", "", true);
}

function changeCodigoProceso() {
    if ($("#ddlFacturaDeVentaCodigoProceso").val() != "") {
        obtenerFacturaAutomaticaProcesados($("#ddlFacturaDeVentaCodigoProceso").val());
    }
    else {
        obtenerFacturaAutomaticaProcesados(0);
    }
}


function editarPedidoDeVenta(id) {
    window.location.href = "/comprobantese.aspx?tipo=PDV&ID=" + id;
}

function obtenerInfoPorPeriodo() {
    obtenerFacturasDelMesPedidos();
    obtenerComprasDelMesPedidos();
    obtenerGastosGeneralesDelMesPedidos();
    obtenerResumenPedidos();
}

function obtenerFacturasDelMesPedidos() {
    var periodo = $("#ddlPeriodo").val();
    if (periodo != "") {
        $.ajax({
            type: "POST",
            url: "facturaAutomatica.aspx/obtenerFacturasDelMesPedidos",
            data: "{periodo:" + periodo + "}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                if (data != null) {
                    $("#lbPedidoDeVentaFacturasDelMes").text(data.d);
                }
            },
            error: function (response) {
                var r = jQuery.parseJSON(response.responseText);
                $("#msgError").html(r.Message);
                $("#divError").show();
                $("#divOk").hide();
            }
        });
    } else {
        $("#msgError").html("Debe seleccionar un periodo");
        $("#divError").show();
        $("#divOk").hide();
    }
}

function obtenerComprasDelMesPedidos() {
    var periodo = $("#ddlPeriodo").val();

    $.ajax({
        type: "POST",
        url: "facturaAutomatica.aspx/obtenerComprasDelMesPedidos",
        data: "{periodo:" + periodo + "}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            if (data != null) {
                $("#lbPedidoDeVentaComprasDelMes").text(data.d);
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

function obtenerGastosGeneralesDelMesPedidos() {
    var periodo = $("#ddlPeriodo").val();
    $.ajax({
        type: "POST",
        url: "facturaAutomatica.aspx/obtenerGastosGeneralesDelMesPedidos",
        data: "{periodo:" + periodo + "}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            if (data != null) {
                $("#lbPedidoDeVentaGastosGenerales").text(data.d);
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

function obtenerResumenPedidos() {
    var periodo = $("#ddlPeriodo").val();
    $.blockUI({ message: $('#divEspera') });
    document.body.style.cursor = 'wait';
    $.ajax({
        type: "POST",
        url: "facturaAutomatica.aspx/obtenerResumenPedidos",
        data: "{periodo:" + periodo + "}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            $.unblockUI();
            document.body.style.cursor = 'default';
            if (data != null) {
                $("#resultsPedidoDeVentaResumen").html(data.d);
            }
        },
        error: function (response) {
            $.unblockUI();
            document.body.style.cursor = 'default';
            var r = jQuery.parseJSON(response.responseText);
            $("#msgError").html(r.Message);
            $("#divError").show();
            $("#divOk").hide();
        }
    });
}

function obtenerDetallePedidos(periodo, rango) {
    $.blockUI({ message: $('#divEspera') });
    document.body.style.cursor = 'wait';
    $.ajax({
        type: "POST",
        url: "facturaAutomatica.aspx/obtenerDetallePedidos",
        data: "{periodo: '" + periodo + "', rango: '" + rango + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            $.unblockUI();
            document.body.style.cursor = 'default';
            if (data != null) {
                $("#hdnPedidoDeVentaDetallePeriodo").val(periodo);
                $("#hdnPedidoDeVentaDetalleRango").val(rango);
                $("#resultsPedidoDeVentaDetalle").html(data.d);
                $('#modalPedidoDeVentaDetalle').modal('show');
            }
        },
        error: function (response) {
            $.unblockUI();
            document.body.style.cursor = 'default';
            var r = jQuery.parseJSON(response.responseText);
            $("#msgError").html(r.Message);
            $("#divError").show();
            $("#divOk").hide();
        }
    });
}

function editar(id) {
    window.location.href = "/comprobantese.aspx?tipo=FAC&ID=" + id;
}

function exportarPedidoDeVentaDetalle() {
    resetearExportacionPedidoDeVentaDetalle();

    $("#imgLoadingPedidoDeVentaDetalle").show();
    $("#divIconoDescargarDetalle").hide();

    var periodo = $("#hdnPedidoDeVentaDetallePeriodo").val();
    var rango = $("#hdnPedidoDeVentaDetalleRango").val();

    var info = "{periodo: '" + periodo + "', rango: '" + rango + "'}";

    $.ajax({
        type: "POST",
        url: "facturaAutomatica.aspx/exportarPedidoDeVentaDetalle",
        data: info,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data, text) {
            if (data.d != "") {
                $("#divError").hide();
                $("#imgLoadingPedidoDeVentaDetalle").hide();
                $("#lnkPedidoDeVentaDownloadDetalle").show();
                $("#lnkPedidoDeVentaDownloadDetalle").attr("href", data.d);
                $("#lnkPedidoDeVentaDownloadDetalle").attr("download", data.d);
            }
        }
    });
}

function resetearExportacionPedidoDeVentaDetalle() {
    $("#imgLoadingPedidoDeVentaDetalle, #lnkDownloadDetalle").hide();
    $("#divIconoDescargarDetalle").show();
}

function obtenerFacturaAutomaticaProcesados(codigoProceso) {

    $.ajax({
        type: "POST",
        url: "facturaAutomatica.aspx/obtenerFacturaAutomaticaProcesados",
        data: "{procesoFacturaAutomatica:" + codigoProceso + "}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            if (data != null) {
                $("#resultsFacturaDeVentaFacturaAutomaticaProcesados").html(data.d);
            }
        }
    });
}

function filtrar() {
    var idPersona = 0
    if ($("#ddlFacturaDeVentaPersona").val() != "") {
        idPersona = $("#ddlFacturaDeVentaPersona").val();
    }
    obtenerFacturasPendientesDeProcesar(idPersona, $("#txtFacturaDeVentaDesdeFechaComprobante").val());
}

function obtenerFacturasPendientesDeProcesar(idPersona, fechaDesdeComprobante) {

    $.ajax({
        type: "POST",
        url: "facturaAutomatica.aspx/obtenerFacturasSinCAE",
        data: "{idPersona:" + idPersona + ", fechaDesdeComprobante: '" + fechaDesdeComprobante + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            if (data != null) {
                $("#resultsFacturaDeVentaFacturasPendientesDeProcesar").html(data.d);
            }
        }
    });
}

function procesar(fechaComprobante) {
    $.blockUI({ message: $('#divEspera') });
    $('#divOk').hide();
    $("#divFacturaDeVentaTabla").hide();
    $("#divError").hide();
    var idPersona = 0;
    if ($("#ddlFacturaDeVentaPersona").val() != "") {
        idPersona = $("#ddlFacturaDeVentaPersona").val();
    }
    document.body.style.cursor = 'wait';
    $.ajax({
        type: "POST",
        url: "facturaAutomatica.aspx/procesar",
        data: "{ idPersona: " + idPersona + ", fechaComprobante: '" + fechaComprobante + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data, text) {
            $.unblockUI();
            document.body.style.cursor = 'default';
            if (data != null) {
                $("#msgOk").html("Proceso generado correctamente.");
                $('#divOk').show();
                $('#divFacturaDeVentaTabla').show();
                $("#divError").hide();
                $("#bodyDetalleFacturaDeVenta").html(data.d);                
            }
            setTimeout('document.location.reload()', 10000);
        },
        error: function (response) {
            $.unblockUI();
            document.body.style.cursor = 'default';
            var r = jQuery.parseJSON(response.responseText);
            $("#msgError").html(r.Message);
            $("#divError").show();
            $("#divOk").hide();
        }
    });
    
}


function modificarFechaComprobante(fechaComprobante) {
    $.blockUI({ message: $('#divEspera') });
    document.body.style.cursor = 'wait';

    $('#divOk').hide();
    $("#divFacturaDeVentaTabla").hide();
    $("#divError").hide();
    var idPersona = 0;
    if ($("#ddlFacturaDeVentaPersona").val() != "") {
        idPersona = $("#ddlFacturaDeVentaPersona").val();
    }
    const output = fechaComprobante.substring(6, 10) + '-' + fechaComprobante.substring(3, 5) + '-' + fechaComprobante.substring(0, 2);
    bootbox.prompt({
        title: "Ingrese la nueva fecha de comprobante",
        inputType: 'date',
        value: output,
        callback: function (result) {
            if (result != null) {
                $.ajax({
                    type: "POST",
                    url: "facturaAutomatica.aspx/modificarFechaComprobante",
                    data: "{ idPersona: " + idPersona + ", fechaComprobante: '" + fechaComprobante + "', fechaComprobanteModificada: '" + result + "'}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data, text) {
                        $.unblockUI();
                        document.body.style.cursor = 'default';
                        if (data != null) {
                            $("#msgOk").html("Se actualizo la fecha de comprobante correctamente.");
                            $('#divOk').show();
                            $("#divError").hide();
                            $('html, body').animate({ scrollTop: 0 }, 'slow');
                            setTimeout('document.location.reload()', 3000);
                        }
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
        }
    });

}


function eliminarComprobante(fechaComprobante) {
    $.blockUI({ message: $('#divEspera') });
    document.body.style.cursor = 'wait';

    $('#divOk').hide();
    $("#divFacturaDeVentaTabla").hide();
    $("#divError").hide();
    var idPersona = 0;
    if ($("#ddlFacturaDeVentaPersona").val() != "") {
        idPersona = $("#ddlFacturaDeVentaPersona").val();
    }
    $.ajax({
        type: "POST",
        url: "facturaAutomatica.aspx/eliminarComprobante",
        data: "{ idPersona: " + idPersona + ", fechaComprobante: '" + fechaComprobante + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data, text) {
            $.unblockUI();
            document.body.style.cursor = 'default';
            if (data != null) {
                $("#msgOk").html("Se eliminaron los comprobantes correctamente.");
                $('#divOk').show();
                $("#divError").hide();
                $('html, body').animate({ scrollTop: 0 }, 'slow');
                setTimeout('document.location.reload()', 3000);
            }
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

function mostrarEnvioDesdeListado(idComprobante) {

    var info = "{ id: " + idComprobante + "}";

    $.ajax({
        type: "POST",
        url: "comprobantesv.aspx/obtenerDatosParaEnvioDeCorreo",
        data: info,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data, text) {
            var fileName = data.d;
            $("#hdnFile").val(fileName.Comprobante);
            $("#hdnRazonSocial").val(data.d.RazonSocialCliente);
            obtenerMailPersonaDesdeSuId(data.d.IdPersona);
            completarAsuntoMailConDatosDelUsuario(fileName.Comprobante.toUpperCase());
            completarCuerpoMailConDatosDeLaPersonaDesdeSuId(data.d.IdPersona);
            $("#divErrorMail").hide();
            Toolbar.mostrarEnvio();
            $('#modalEMail').modal('show');
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


function obtenerMailPersonaDesdeSuId(idPersona) {

    var info = "{idPersona: " + idPersona + "}";

    $.ajax({
        type: "POST",
        url: "common.aspx/obtenerMailPersona",
        data: info,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data, text) {
            $("#txtEnvioPara").val(data.d);
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

function completarAsuntoMailConDatosDelUsuario(nroComprobante) {

    var comprobante = nroComprobante.substring(nroComprobante.length - 21, nroComprobante.length - 4);
    var info = "";

    $.ajax({
        type: "POST",
        url: "common.aspx/obtenerCUILUsuario",
        data: info,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data, text) {
            $("#txtEnvioAsunto").val(data.d);
            var info2 = "";

            $.ajax({
                type: "POST",
                url: "common.aspx/obtenerRazonSocialUsuario",
                data: info2,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data, text) {
                    var datos = $("#txtEnvioAsunto").val() + ' - ' + data.d + ' - ' + comprobante;
                    $("#txtEnvioAsunto").val(datos.toUpperCase());
                },
                error: function (response) {
                    var r = jQuery.parseJSON(response.responseText);
                    $("#msgError").html(r.Message);
                    $("#divError").show();
                    $("#divOk").hide();
                    $('html, body').animate({ scrollTop: 0 }, 'slow');
                }
            });
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


function completarCuerpoMailConDatosDeLaPersonaDesdeSuId(idPersona) {

    var info = "{idPersona: " + idPersona + "}";

    $.ajax({
        type: "POST",
        url: "common.aspx/obtenerRazonSocialPersona",
        data: info,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data, text) {
            $("#txtEnvioMensaje").val('SE ADJUNTA COMPROBANTE: ' + data.d);

            $.ajax({
                type: "POST",
                url: "common.aspx/obtenerNroDocumentoPersona",
                data: info,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data, text) {
                    var datos = $("#txtEnvioMensaje").val() + ' (' + data.d + ')';
                    $("#txtEnvioMensaje").val(datos.toUpperCase());
                },
                error: function (response) {
                    var r = jQuery.parseJSON(response.responseText);
                    $("#msgError").html(r.Message);
                    $("#divError").show();
                    $("#divOk").hide();
                    $('html, body').animate({ scrollTop: 0 }, 'slow');
                }
            });
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

