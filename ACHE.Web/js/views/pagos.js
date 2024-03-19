function obtenerComprasPendientes(id) {
    $("#ddlComprobante").html("");

    $.ajax({
        type: "POST",
        url: "/pagose.aspx/obtenerComprasPendientes",
        data: "{ id: " + id + ", idPago: " + parseInt($("#hdnID").val()) + "}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            if (data != null) {
                $("<option/>").attr("value", "").text("").appendTo($("#ddlComprobante"));

                for (var i = 0; i < data.d.length; i++) {
                    $("<option/>").attr("value", data.d[i].ID).text(data.d[i].Nombre).appendTo($("#ddlComprobante"));
                }

                $("#ddlComprobante").trigger("change");
            }
        }
    });
}

function changeComprobante() {
    if ($("#ddlComprobante").val() != "" && $("#ddlComprobante").val() != null) {

        var aux1 = $("#ddlComprobante option:selected").text().split("$")[1];
        var importeIva = aux1.substring(0, aux1.indexOf("-"));
        $("#txtImporteNeto").val(importeIva.replace(".", "").replace(".", "").trim());

        var aux2 = $("#ddlComprobante option:selected").text().split("$")[2];
        $("#lblSaldo").html("$" + aux2.substr(0, aux2.length - 1));
        $("#txtImporte").val(aux2.substr(0, aux2.length - 1).replace(".", "").replace(".", ""));
    }
    else {
        $("#lblSaldo").html("");
        $("#txtImporte").val("");
    }
}

/*** FORM EDICION ***/

function loadInfo(id) {

    $.ajax({
        type: "POST",
        url: "pagose.aspx/obtenerDatos",
        data: "{ id: " + id + "}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data, text) {
            if (data.d != null) {
                limpiarNuevoComprobante();
                var idPersona = 0;
                idPersona = data.d.IDPersona;
                //Common.obtenerProveedores("ddlPersona", idPersona, true);
                Common.obtenerPersonas("ddlPersona", idPersona, true);
                $("#txtFecha").val(data.d.Fecha);
                $("#txtObservaciones").val(data.d.Observaciones);              
                obtenerComprasPendientes(idPersona);
                $("#hdnRazonSocial").val(data.d.Personas.RazonSocial);

                obtenerItems();
                obtenerFormas();
                obtenerTotales();
                CobRetenciones.obtenerRetenciones();

                $("#lnkAceptar").show();
                $("#divFactura").show();

                setTimeout(function () {
                    $("#ddlPersona").attr("onchange", "changePersona()");
                }, 1000);
                Common.obtenerBancos('ddlBancos', "");

                $("#ddlPersona").attr("disabled", true);

            }
        },
        error: function (response) {
            var r = jQuery.parseJSON(response.responseText);
            $("#msgError").html(r.Message);
            $("#divError").show();
        }
    });
}

function obtenerTotales() {
    $.ajax({
        type: "GET",
        url: "pagose.aspx/obtenerTotales",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data, text) {
            if (data.d != null) {
                $("#divTotal").html("$ " + data.d.Total);
            }
        },
        error: function (response) {
            var r = jQuery.parseJSON(response.responseText);
            $("#msgErrorDetalle").html(r.Message);
            $("#divErrorDetalle").show();
        }
    });
}

function ocultarMensajes() {
    $("#divError, #divOk, #divErrorDetalle, #divErrorForma, #divErrorRet").hide();
}

function grabar(generarCAE) {
    ocultarMensajes();

    if ($('#frmEdicion').valid()) {

        Common.mostrarProcesando("lnkAceptar");
        var info = "{ id: " + parseInt($("#hdnID").val())
                + ", idPersona: " + parseInt($("#ddlPersona").val())
                + ", obs: '" + $("#txtObservaciones").val()
                + " ', fechaPago:'" + $("#txtFecha").val()
                + "'}";

        $.ajax({
            type: "POST",
            url: "pagose.aspx/guardar",
            data: info,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data, text) {
                $("#hdnID").val(data.d);
                if (generarCAE) {                   
                    $("#litModalOkTitulo").html("Comprobante emitido correctamente");
                    generarCae();
                }
                else {
                    $('#divOk').show();
                    $("#divError").hide();
                    $('html, body').animate({ scrollTop: 0 }, 'slow');
                    $('#modalOk').modal('show');
                }
            },
            error: function (response) {
                var r = jQuery.parseJSON(response.responseText);
                $("#msgError").html(r.Message);
                $("#divError").show();
                $("#divOk").hide();
                $('html, body').animate({ scrollTop: 0 }, 'slow');
                Common.ocultarProcesando("lnkAceptar", "Generar");
            }
        });
    }
    else {
        return false;
    }
}

function imprimir() {
    //if ($("#hdnFile").val() == "") {//Si no fue generado el archivo

    if ($('#frmEdicion').valid()) {
        var info = "{id: " + parseInt($("#hdnID").val())
            + ", idPersona: " + parseInt($("#ddlPersona").val())
            + ", tipo: '" + $("#ddlTipo").val()
            + "', fecha: '" + $("#txtFecha").val()
            + "', obs: '" + $("#txtObservaciones").val()
            + "'}";

        $.ajax({
            type: "POST",
            url: "pagose.aspx/imprimir",
            data: info,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data, text) {
                $("#divError").hide();
                $('html, body').animate({ scrollTop: 0 }, 'slow');

                $("#ifrPdf").attr("src", "/files/pagos/" + data.d + "#zoom=100&view=FitH,top");
                $("#lnkDescargar").attr("href", "/files/pagos/" + data.d);

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
    else {
        return false;
    }
}

function imprimirRetencionGanancia() {
    //if ($("#hdnFile").val() == "") {//Si no fue generado el archivo

    if ($("#hdnEsAgenteRetencionGanancia").val() == "1") {
        if ($('#frmEdicion').valid()) {
            var info = "{id: " + parseInt($("#hdnID").val())
                + ", idPersona: " + parseInt($("#ddlPersona").val())
                + ", tipo: '" + $("#ddlTipo").val()
                + "', fecha: '" + $("#txtFecha").val()
                + "', obs: '" + $("#txtObservaciones").val()
                + "'}";

            $.ajax({
                type: "POST",
                url: "pagose.aspx/imprimirRetencionGanancia",
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
        else {
            return false;
        }        
    } else {
        var r = jQuery.parseJSON(response.responseText);
        $("#msgError").html("Por las condiciones de esta compra, no corresponde retención de ganancias");
        $("#divError").show();
        $("#divOk").hide();
        $('html, body').animate({ scrollTop: 0 }, 'slow');
    }
    
}

function imprimirRetencionGananciaPopUp() {
    //if ($("#hdnFile").val() == "") {//Si no fue generado el archivo

    if ($("#hdnEsAgenteRetencionGanancia").val() == "1") {
        if ($('#frmEdicion').valid()) {
            var info = "{id: " + parseInt($("#hdnID").val())
                + ", idPersona: " + parseInt($("#ddlPersona").val())
                + ", tipo: '" + $("#ddlTipo").val()
                + "', fecha: '" + $("#txtFecha").val()
                + "', obs: '" + $("#txtObservaciones").val()
                + "'}";

            $.ajax({
                type: "POST",
                url: "pagose.aspx/imprimirRetencionGanancia",
                data: info,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data, text) {
                    $("#divErrorModalOk").hide();
                    $('html, body').animate({ scrollTop: 0 }, 'slow');

                    $("#ifrPdf").attr("src", "/files/retencionGanancia/" + data.d + "#zoom=100&view=FitH,top");
                    $("#lnkDescargar").attr("href", "/files/retencionGanancia/" + data.d);

                    $('#modalPdf').modal('show');
                },
                error: function (response) {
                    var r = jQuery.parseJSON(response.responseText);
                    $("#msgErrorModalOk").html(r.Message);
                    $("#divErrorModalOk").show();
                }
            });
        }
        else {
            return false;
        }
    } else {
        var r = jQuery.parseJSON(response.responseText);
        $("#msgErrorModalOk").html("Por las condiciones de esta compra, no corresponde retención de ganancias");
        $("#divErrorModalOk").show();
    }

}

function cancelar() {
    window.location.href = "/pagos.aspx";
}

function configForm() {
    $("#hdnModo").val("PAGO");
    $(".select2").select2({
        width: '100%', allowClear: true
    });
   
    $("#lnkAceptar").hide();

    // Date Picker
    Common.configDatePicker();
    $("#txtCantidad").numericInput();

    $("#txtNumero").mask("?99999999");
    $("#txtNumero").blur(function () {
        $("#txtNumero").val(padZeros($("#txtNumero").val(), 8));
    });

    $("#txtImporte, #txtImporteForma, #txtImporteRet").maskMoney({ thousands: '', decimal: ',', allowZero: true });

    $("#txtEnvioPara, #txtEnvioAsunto, #txtEnvioMensaje").keypress(function (event) {
        var aux = Toolbar.toggleEnviosError();
    });
    $("#txtEnvioPara, #txtEnvioAsunto, #txtEnvioMensaje").blur(function (event) {
        var aux = Toolbar.toggleEnviosError();
    });


    $("#txtImporte").keypress(function (event) {

        var keycode = (event.keyCode ? event.keyCode : event.which);
        if (keycode == '13') {
            agregarItem();
            return false;
        }
    });
    $("#txtImporteForma").keypress(function (event) {

        var keycode = (event.keyCode ? event.keyCode : event.which);
        if (keycode == '13') {
            agregarForma();
            return false;
        }
    });
    $("#txtImporteRet").keypress(function (event) {

        var keycode = (event.keyCode ? event.keyCode : event.which);
        if (keycode == '13') {
            CobRetenciones.agregarRet();
            return false;
        }
    });

    // Validation with select boxes
    $("#frmEdicion").validate({
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

    if (MI_CONDICION == "RI")
        $("#Divretenciones").show()
    else
        $("#Divretenciones").hide()

    if ($("#hdnID").val() != "" && $("#hdnID").val() != "0") {
        loadInfo($("#hdnID").val());
    }
    else {
        Common.obtenerPersonas("ddlPersona", $("#hdnIDPersona").val(), true);
        //Common.obtenerProveedores("ddlPersona", $("#hdnIDPersona").val(), true);

        $("#ddlPersona").attr("onchange", "changePersona()");

        if ($("#hdnCargarDatosDesdeCompra").val() == "1") {
            obtenerItems();
            obtenerTotales();
            $("#lnkAceptar,#divFactura").show();
        }
        Common.obtenerBancos('ddlBancos', "");
    }

    $("#divImprimir").hide();
    if ($("#hdnEsAgenteRetencionGanancia").val() != "1") {
        $("#txtNroRefRet").attr("disabled", true);
        $("#divImprimirRetencionGanancia").hide();
        $("#divDownloadRetencionGanancia").hide();
    }
    else {
        $("#txtNroRefRet").attr("disabled", false);
        $("#divImprimirRetencionGanancia").show();
        $("#divDownloadRetencionGanancia").show();        
    }
}

/*** FORM ALTA ***/

function changePersona() {

    limpiarNuevoComprobante();
    if ($("#ddlPersona").val() != "" && $("#ddlPersona").val() != null) {
        $("#divFactura").show();
        $("#lnkAceptar").show();
        $("#divImprimir").show();
        var idPersona = parseInt($("#ddlPersona").val());
        //obtenerInfoPersona(parseInt($("#ddlPersona").val()), '');
        obtenerComprasPendientes(idPersona);
    }
    else {
        $("#lnkAceptar,#divFactura").hide();
    }
}

function limpiarNuevoComprobante() {
    ocultarMensajes();
    $("#txtNumero").val("");
    //$("#ddlTipo").html("<option value=''></option>");
    $("#hdnIDItem, #hdnIDForma").val("0");
    $("#bodyDetalle").html("<tr><td colspan='9' style='text-align:center'>No tienes items agregados</td></tr>");
    $("#bodyFormas").html("<tr><td colspan='5' style='text-align:center'>No tienes items agregados</td></tr>");
    $("#bodyRetenciones").html("<tr><td colspan='5' style='text-align:center'>No tienes items agregados</td></tr>");
    $("#divTotal").html("0");
    $("#lblSaldo").html("");
    $("#txtObservaciones").val("");
}

function obtenerInfoPersona(idPersona, tipo) {
    $.ajax({
        type: "POST",
        url: "personase.aspx/obtenerDatos",
        data: "{ id: " + idPersona + "}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data, text) {
            if (data.d != null) {

                obtenerComprasPendientes(idPersona);

                
                $("#hdnRazonSocial").val(data.d.RazonSocial);
                //$("#litPersonaRazonSocial").html(data.d.RazonSocial + " - " + data.d.NroDoc);
                //$("#litPersonaDomicilio").html(data.d.Domicilio);
                //$("#litPersonaPaisCiudad").html(data.d.Provincia + ", " + data.d.Ciudad);
                //$("#litPersonaTelefono").html(data.d.Telefono);
                //$("#litPersonaEmail").html(data.d.Email);
                //$("#litPersonaCondicionIva").html("<strong>Condición IVA:</strong> " + Common.obtenerCondicionIvaDesc(data.d.CondicionIva));
            }
        },
        error: function (response) {
            var r = jQuery.parseJSON(response.responseText);
            $("#msgError").html(r.Message);
            $("#divError").show();
        }
    });
}


/*** FORMAS DE PAGO ***/
function cancelarForma() {
    $("#txtNroRef, #txtImporteForma").val("");
    $("#hdnIDForma").val("0");
    $("#ddlFormaPago").val("").trigger("change");
    $("#btnAgregarForma").html("Agregar");
}

function agregarForma() {
    ocultarMensajes();

    var idCheque = ($("#ddlCheque").val() == null ? "" : $("#ddlCheque").val());
    var idbanco = ($("#ddlBancos").val() == null ? "" : $("#ddlBancos").val());

    //if (($("#ddlFormaPago").val() != "Efectivo" && $("#ddlFormaPago").val() != "Cheque de Terceros") && $("#ddlBancos").val() == "") {
    //    $("#msgErrorForma").html("Debes ingresar el Nombre del banco");
    //    $("#divErrorForma").show();
    //    return false;
    //}

    if ($("#txtImporteForma").val() != "" && $("#ddlFormaPago").val() != "") {

        if (parseFloat($("#txtImporteForma").val()) == 0) {
            $("#msgErrorForma").html("El importe debe ser mayor a 0.");
            $("#divErrorForma").show();
        }
        else {

            var info = "{ id: " + parseInt($("#hdnIDForma").val())
                    + ", forma: '" + $("#ddlFormaPago").val()
                    + "', nroRef: '" + $("#txtNroRef").val()
                    + "', importe: '" + $("#txtImporteForma").val()
                    + "', idcheque: '" + idCheque
                    + "', idBanco: '" + idbanco
                    + "'}";

            $.ajax({
                type: "POST",
                url: "pagose.aspx/agregarForma",
                data: info,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data, text) {
                    $("#txtImporteForma, #txtNroRef").val("");
                    $("#ddlFormaPago").val("").trigger("change");
                    $("#hdnIDForma").val("0");
                    $("#btnAgregarForma").html("Agregar");
                    $("#divBancos").hide();

                    obtenerFormas();
                },
                error: function (response) {
                    var r = jQuery.parseJSON(response.responseText);
                    $("#msgErrorForma").html(r.Message);
                    $("#divErrorForma").show();
                }
            });
        }
    }
    else {
        $("#msgErrorForma").html("Debes ingresar la forma de pago y precio.");
        $("#divErrorForma").show();
    }
}

function eliminarForma(id) {

    var info = "{ id: " + parseInt(id) + "}";

    $.ajax({
        type: "POST",
        url: "pagose.aspx/eliminarForma",
        data: info,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data, text) {
            obtenerFormas();
        },
        error: function (response) {
            var r = jQuery.parseJSON(response.responseText);
            $("#msgErrorForma").html(r.Message);
            $("#divErrorForma").show();
        }
    });
}

function modificarForma(id, forma, nroRef, importe, idBanco, idCheque) {

    $("#ddlFormaPago").val(forma).trigger("change");
    $("#ddlCheque").val(idCheque).trigger("change");
    $("#ddlBancos").val(idBanco).trigger("change");

    $("#txtNroRef").val(nroRef);
    $("#txtImporteForma").val(importe);

    $("#hdnIDForma").val(id);
    $("#btnAgregarForma").html("Actualizar");

}

function obtenerFormas() {

    $.ajax({
        type: "GET",
        url: "pagose.aspx/obtenerFormas",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            if (data != null) {
                $("#bodyFormas").html(data.d);
                obtenerTotalFormas();
            }
        }
    });


}

function abrirModalCheque() {

    var esPropio = 0;
    var empresa = 0;

    if ($("#ddlFormaPago").val() == "Cheque de Terceros") {
        esPropio = 1;
    }
    if ($("#ddlFormaPago").val() == "Cheque Empresa") {
        esPropio = 1;
        empresa = 1;
    }

    Common.showModalCheque(esPropio, empresa,  "pago");
}

function changeFormas() {

    //$("#ddlCheque").val("").trigger("change");
    //if ($("#ddlFormaPago").val() == "Cheque Propio") {
    //    $("#divCheque,#divNuevoCheque").show();
    //    $("#txtImporteForma,#txtNroRef").attr("disabled", true);
    //    Common.obtenerCheques('ddlCheque', "", true, true);
    //}
    //else {
    //    $("#divCheque,#divNuevoCheque").hide();
    //    $("#txtImporteForma,#txtNroRef").attr("disabled", false);
    //}

    //if ($("#ddlFormaPago").val() == "Cheque de Terceros") {
    //    Common.obtenerCheques('ddlCheque', "", true, false);
    //    $("#divCheque").show();
    //    $("#txtImporteForma,#txtNroRef").attr("disabled", true);
    //}

    //if ($("#ddlFormaPago").val() != "Efectivo" && $("#ddlFormaPago").val() != "Cheque de Terceros") {
    //    $("#divBancos").show();
    //    $("#txtImporteForma,#txtNroRef").val("");
    //    $("#ddlBancos").val($("#ddlBancos option:first").val());
    //}
    //else {
    //    $("#ddlBancos").val("").trigger("change");
    //    $("#divBancos").hide();
    //}

    $("#ddlCheque").val("").trigger("change");

    if ($("#ddlFormaPago").val() == "") {
        $("#ddlFormaPago").val("Efectivo");
    }

    if ($("#ddlFormaPago").val() == "Cheque Tercero") {
        Common.obtenerCheques('ddlCheque', "", true, false, false);
        $("#divCheque,#divNuevoCheque").show();
        //$("#txtImporteForma,#txtNroRef").attr("disabled", true);
        $("#txtNroRef").attr("disabled", true);
    }
    else if ($("#ddlFormaPago").val() == "Cheque Propio") {
        Common.obtenerCheques('ddlCheque', "", true, true, false);
        $("#divCheque,#divNuevoCheque").show();
        //$("#txtImporteForma,#txtNroRef").attr("disabled", true);
        $("#txtNroRef").attr("disabled", true);
    }
    else if ($("#ddlFormaPago").val() == "Cheque Empresa") {
        Common.obtenerCheques('ddlCheque', "", true, true, true);
        $("#divCheque,#divNuevoCheque").show();
        //$("#txtImporteForma,#txtNroRef").attr("disabled", true);
        $("#txtNroRef").attr("disabled", true);
    }
    else if ($("#ddlFormaPago").val() == "Nota de credito") {
        $("#divCheque,#divNuevoCheque,#divBancos").hide();
        $("#divNotasCredito").show();
        $("#txtImporteForma,#txtNroRef").attr("disabled", true);
    }
    else {
        $("#divCheque,#divNuevoCheque,#divNotasCredito").hide();
        if ($("#txtImporte").val() != "") {
            $("#txtImporteForma").val($("#txtImporte").val());
        }
        $("#txtImporteForma,#txtNroRef").attr("disabled", false);
    }

    if ($("#ddlFormaPago").val() != "Efectivo" &&
        $("#ddlFormaPago").val() != "Tarjeta de credito" &&
        $("#ddlFormaPago").val() != "Tarjeta de debito" &&
        $("#ddlFormaPago").val() != "Nota de credito") {
        //$("#divBancos").show();
        //$("#txtImporteForma,#txtNroRef").val("");
        $("#ddlBancos").val($("#ddlBancos option:first").val());
    }
    else {
        $("#ddlBancos").val("").trigger("change");
        $("#divBancos").hide();
    }

}

function changeChequeTercero() {
    if ($("#ddlCheque").val() != "" && ($("#ddlFormaPago").val() == "Cheque Tercero" || $("#ddlFormaPago").val() == "Cheque Propio" || $("#ddlFormaPago").val() == "Cheque Empresa")) {
        var cheque = $("#ddlCheque option:selected").text().split("$");
        var importe = cheque[1];
        var nroRef = cheque[0].split("Nro:")[1];

        $("#txtImporteForma").val(importe)
        $("#txtNroRef").val(nroRef)
    }
    else {
        $("#txtImporteForma,#txtNroRef").val("")
    }

    //if ($("#ddlCheque").val() != "" && ($("#ddlFormaPago").val() == "Cheque Tercero" || $("#ddlFormaPago").val() == "Cheque Propio")) {
    //    var cheque = $("#ddlCheque option:selected").text().split("$");
    //    //var importe = cheque[1];


    //    var posicionResto = cheque[1].search("Resta");
    //    var importe = cheque[1].substr(posicionResto + 6, cheque[1].length).replace(')', '');

    //    var nroRef = cheque[0].split("Nro:")[1];

    //    $("#txtImporteForma").val(importe.replace(".", ""))
    //    $("#txtNroRef").val(nroRef)
    //}
    //else {
    //    $("#txtImporteForma,#txtNroRef").val("")
    //}

}

function obtenerTotalFormas() {

    $.ajax({
        type: "GET",
        url: "pagose.aspx/obtenerFormasTotal",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            if (data != null) {
                $("#tdTotalFormaDePago").html("$ " + data.d);
            }
        }
    });
}

/*** FIN FORMAS DE PAGO ***/

/*** RETENCIONES ***/

var CobRetenciones = {
    cancelarRet: function () {
        $("#txtNroRefRet, #txtImporteRet").val("");
        $("#hdnIDRet").val("0");
        $("#ddlTipoRet").val("").trigger("change");
        $("#btnAgregarRet").html("Agregar");
    },
    agregarRet: function () {
        ocultarMensajes();

        if ($("#txtImporteRet").val() != "" && $("#ddlTipoRet").val() != "") {

            if (parseFloat($("#txtImporteRet").val()) == 0) {
                $("#msgErrorRet").html("El importe debe ser mayor a 0.");
                $("#divErrorRet").show();
            }
            else {

                var info = "{ id: " + parseInt($("#hdnIDRet").val())
                        + ", tipo: '" + $("#ddlTipoRet").val()
                        + "', nroRef: '" + $("#txtNroRefRet").val()
                        + "', importe: '" + $("#txtImporteRet").val()
                        + "'}";

                $.ajax({
                    type: "POST",
                    url: "pagose.aspx/agregarRetencion",
                    data: info,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data, text) {
                        $("#txtImporteRet, #txtNroRefRet").val("");
                        $("#ddlTipoRet").val("").trigger("change");
                        $("#hdnIDRet").val("0");
                        $("#btnAgregarRet").html("Agregar");

                        CobRetenciones.obtenerRetenciones();
                    },
                    error: function (response) {
                        var r = jQuery.parseJSON(response.responseText);
                        $("#msgErrorRet").html(r.Message);
                        $("#divErrorRet").show();
                    }
                });
            }
        }
        else {
            $("#msgErrorRet").html("Debes ingresar el tipo de retencion e importe.");
            $("#divErrorRet").show();
        }
    },
    eliminarRet: function (id) {
        var info = "{ id: " + parseInt(id) + "}";

        $.ajax({
            type: "POST",
            url: "pagose.aspx/eliminarRetencion",
            data: info,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data, text) {
                CobRetenciones.obtenerRetenciones();
            },
            error: function (response) {
                var r = jQuery.parseJSON(response.responseText);
                $("#msgErrorRet").html(r.Message);
                $("#divErrorRet").show();
            }
        });
    },
    modificarRet: function (id, tipo, nroRef, importe) {

        $("#txtNroRefRet").val(nroRef);
        $("#txtImporteRet").val(importe);
        $("#ddlTipoRet").val(tipo).trigger("change");

        $("#hdnIDRet").val(id);
        $("#btnAgregarRet").html("Actualizar");
    },
    obtenerRetenciones: function (id) {
        $.ajax({
            type: "GET",
            url: "pagose.aspx/obtenerRetenciones",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                if (data != null) {
                    $("#bodyRetenciones").html(data.d);
                }
            }
        });
    }
}

/*** FIN RETENCIONES ***/

/*** ITEMS ***/
function cancelarItem() {
    $("#txtImporte").val("");
    $("#lblSaldo").html("");
    $("#hdnIDItem").val("0");
    $("#ddlComprobante").val("").trigger("change");
    $("#btnAgregarItem").html("Agregar");
}

function agregarItem() {
    ocultarMensajes();

    if ($("#txtImporte").val() != "" && $("#ddlComprobante").val() != "") {

        if (parseFloat($("#txtImporte").val()) == 0) {
            $("#msgErrorDetalle").html("El importe debe ser mayor a 0.");
            $("#divErrorDetalle").show();
        }
        else {

            var info = "{ id: " + parseInt($("#hdnIDItem").val())
                    + ", idComprobante: '" + $("#ddlComprobante").val()
                    + "', comprobante: '" + $("#ddlComprobante option:selected").text()
                    + "', importe: '" + $("#txtImporte").val()
                    + "', importeNeto: '" + $("#txtImporteNeto").val()
                    + "', importeCompra: '" + $("#lblSaldo").html()
                    + "', idPago: " + parseInt($("#hdnID").val())
                    + "}";

            $.ajax({
                type: "POST",
                url: "pagose.aspx/agregarItem",
                data: info,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data, text) {
                    $("#txtImporte").val("");
                    $("#lblSaldo").html("");
                    $("#ddlComprobante").val("").trigger("change");
                    $("#hdnIDItem").val("0");
                    $("#btnAgregarItem").html("Agregar");

                    $("#txtImporteRet, #txtNroRefRet").val("");
                    $("#ddlTipoRet").val("").trigger("change");
                    $("#hdnIDRet").val("0");
                    $("#btnAgregarRet").html("Agregar");

                    CobRetenciones.obtenerRetenciones();

                    obtenerItems();
                    obtenerTotales();
                },
                error: function (response) {
                    var r = jQuery.parseJSON(response.responseText);
                    $("#msgErrorDetalle").html(r.Message);
                    $("#divErrorDetalle").show();
                }
            });
        }
    }
    else {
        $("#msgErrorDetalle").html("Debes ingresar la cantidad, concepto y precio.");
        $("#divErrorDetalle").show();
    }
}

function eliminarItem(id) {

    var info = "{ id: " + parseInt(id) + ", idPago: " + parseInt($("#hdnID").val()) + "}";

    $.ajax({
        type: "POST",
        url: "pagose.aspx/eliminarItem",
        data: info,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data, text) {

            $("#txtImporteRet, #txtNroRefRet").val("");
            $("#ddlTipoRet").val("").trigger("change");
            $("#hdnIDRet").val("0");
            $("#btnAgregarRet").html("Agregar");

            CobRetenciones.obtenerRetenciones();

            obtenerItems();
            obtenerTotales();
        },
        error: function (response) {
            var r = jQuery.parseJSON(response.responseText);
            $("#msgErrorDetalle").html(r.Message);
            $("#divErrorDetalle").show();
        }
    });
}

function modificarItem(id, idComprobante, importe) {

    $("#txtImporte").val(importe);
    //$("#txtRetGanancias").val(retGanancias);
    //$("#txtIIBB").val(iibb);
    //$("#txtSuss").val(suss);
    //$("#txtOtros").val(otros);
    $("#ddlComprobante").val(idComprobante).trigger("change");

    changeComprobante();

    $("#hdnIDItem").val(id);
    $("#btnAgregarItem").html("Actualizar");
}

function obtenerItems() {

    $.ajax({
        type: "GET",
        url: "pagose.aspx/obtenerItems",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            if (data != null) {
                $("#bodyDetalle").html(data.d);
                obtenerTotalFormas();
            }
        }
    });
}

/*** FIN ITEMS ***/

/*** SEARCH ***/

function configFilters() {
    $(".select2").select2({ width: '100%', allowClear: true });

    //Common.obtenerPersonas("ddlPersona", "", true);
    
    $("#txtFechaDesde, #txtFechaHasta, #txtCondicion").keypress(function (event) {
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
    //Common.soloNumerosConGuiones("txtNumero");
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

function nuevo() {
    window.location.href = "/pagose.aspx";
}

function editar(id) {
    window.location.href = "/pagose.aspx?ID=" + id;
}

function eliminar(id, nombre) {
    bootbox.confirm("¿Está seguro que desea eliminar el recibo realizado a " + nombre + "?", function (result) {
        if (result) {
            $.ajax({
                type: "POST",
                url: "pagos.aspx/delete",
                data: "{ id: " + id + "}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data, text) {
                    filtrar();
                },
                error: function (response) {
                    var r = jQuery.parseJSON(response.responseText);
                    $("#divError").html(r.Message);
                    $("#divError").show();
                    $('html, body').animate({ scrollTop: 0 }, 'slow');
                }
            });
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

        var info = "{ condicion: '" + $("#txtCondicion").val()
                   + "', periodo: '" + $("#ddlPeriodo").val()
                   + "', fechaDesde: '" + $("#txtFechaDesde").val()
                   + "', fechaHasta: '" + $("#txtFechaHasta").val()
                   + "', page: " + currentPage + ", pageSize: " + PAGE_SIZE
                   + "}";

        $.ajax({
            type: "POST",
            url: "pagos.aspx/getResults",
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
    //$("#txtNumero, #txtFechaDesde, #txtFechaHasta, #ddlTipo, #ddlModo").val("");
    $("#txtNumero, #txtFechaDesde, #txtFechaHasta, #ddlTipo").val("");
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

    var info = "{ condicion: '" + $("#txtCondicion").val()
              + "', periodo: '" + $("#ddlPeriodo").val()
              + "', fechaDesde: '" + $("#txtFechaDesde").val()
              + "', fechaHasta: '" + $("#txtFechaHasta").val()
              + "'}";

    $.ajax({
        type: "POST",
        url: "pagos.aspx/export",
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

function otroPeriodo() {
    if ($("#ddlPeriodo").val() == "-1")
        $('#divMasFiltros').toggle(600);
    else {
        if ($("#divMasFiltros").is(":visible"))
            $('#divMasFiltros').toggle(600);

        $("#txtFechaDesde,#txtFechaHasta").val("");
        filtrar();
    }
}