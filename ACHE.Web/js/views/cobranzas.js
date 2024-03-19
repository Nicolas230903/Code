function obtenerComprobantesPendientes(id) {
    $("#ddlComprobante").html("");

    $.ajax({
        type: "POST",
        url: "/cobranzase.aspx/obtenerComprobantesPendientes",
        data: "{ id: " + id + ", idCobranza: " + parseInt($("#hdnID").val()) + "}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            if (data != null) {
                $("<option/>").attr("value", "").text("").appendTo($("#ddlComprobante"));

                for (var i = 0; i < data.d.length; i++) {                    
                    $("<option/>").attr("value", data.d[i].ID).text(data.d[i].Nombre).appendTo($("#ddlComprobante"));                    
                }

                if ($("#hdntieneComprobante").val() != "")
                    $("#ddlComprobante").val($("#hdntieneComprobante").val());

                $("#ddlComprobante").trigger("change");
            }
        }
    });
}

function changeComprobante() {
    if ($("#ddlComprobante").val() != "") {
        var aux = $("#ddlComprobante option:selected").text().split("$")[1];
        if (aux != null) {
            $("#lblSaldo").html("$" + aux.substr(0, aux.length - 1));
            $("#txtImporte").val(aux.substr(0, aux.length - 1).replaceAll(".", ""));
            $("#ddlFormaPago").val("Efectivo").trigger("change");
            $("#txtImporteForma").val(aux.substr(0, aux.length - 1).replaceAll(".", ""));
        }       
    }
    else {
        $("#lblSaldo").html("");
        $("#txtImporte").val("");
    }
}

/*** FORM EDICION ***/

function obtenerTotales() {
    $.ajax({
        type: "GET",
        url: "cobranzase.aspx/obtenerTotales",
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

function grabar() {
    ocultarMensajes();

    if ($("#ddlPersona").val() == "") {
        $("#msgError").html("Debes ingresar un Proveedor/Cliente.");
        $("#divError").show();
        $("#divOk").hide();
        $('html, body').animate({ scrollTop: 0 }, 'slow');
        return false;
    }

    $.ajax({
        type: "POST",
        url: "cobranzase.aspx/obtenerFormasTotal",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            if (data != null) {
                var importeTotal = parseFloat(data.d);
                if (importeTotal >= 0) {
                    if ($('#frmEdicion').valid()) {
                        Common.mostrarProcesando("lnkAceptar");
                        var info = "{id: " + parseInt($("#hdnID").val())
                            + ", idPersona: " + parseInt($("#ddlPersona").val())
                            + ", tipo: '" + $("#ddlTipo").val()
                            //+ "', modo: '" + $("#ddlModo").val()
                            + "', fecha: '" + $("#txtFecha").val()
                            + "', idPuntoVenta: " + $("#ddlPuntoVenta").val()
                            + ", nroComprobante: '" + $("#txtNumero").val()
                            + "', obs: '" + $("#txtObservaciones").val()
                            + "'}";

                        $.ajax({
                            type: "POST",
                            url: "cobranzase.aspx/guardar",
                            data: info,
                            contentType: "application/json; charset=utf-8",
                            dataType: "json",
                            success: function (data, text) {
                                $("#hdnID").val(data.d);
                                $("#litModalOkTitulo").html("Recibo generado correctamente");
                                $("#iCheckEnvio").removeClass("fa fa-check");
                                //if ($("#hdnEnvioCR").val() == "1") {
                                //    enviarCobranzaAutomaticamente();
                                //}
                                //else {
                                generarPDF();
                                //}
                            },
                            error: function (response) {
                                var r = jQuery.parseJSON(response.responseText);
                                $("#msgError").html(r.Message);
                                $("#divError").show();
                                $("#divOk").hide();
                                $('html, body').animate({ scrollTop: 0 }, 'slow');
                                Common.ocultarProcesando("lnkAceptar", "Aceptar");
                            }
                        });
                    }
                    else {
                        return false;
                    }
                } else {
                    $("#msgError").html("El monto restante a pagar no debe ser negativo.");
                    $("#divError").show();
                    $("#divOk").hide();
                    $('html, body').animate({ scrollTop: 0 }, 'slow');
                    return false;
                }
            }
        }
    });

    
}

function previsualizar() {
    //if ($("#hdnFile").val() == "") {//Si no fue generado el archivo

    if ($('#frmEdicion').valid()) {
        var info = "{id: " + parseInt($("#hdnID").val())
                    + ", idPersona: " + parseInt($("#ddlPersona").val())
                    + ", tipo: '" + $("#ddlTipo").val()
                    //+ "', modo: '" + $("#ddlModo").val()
                    + "', fecha: '" + $("#txtFecha").val()
                    + "', idPuntoVenta: " + $("#ddlPuntoVenta").val()
                    + ", nroComprobante: '" + $("#txtNumero").val()
                    + "', obs: '" + $("#txtObservaciones").val()
                    + "'}";

        $.ajax({
            type: "POST",
            url: "cobranzase.aspx/previsualizar",
            data: info,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data, text) {
                $("#divError").hide();
                $('html, body').animate({ scrollTop: 0 }, 'slow');

                var version = new Date().getTime();
                $("#ifrPdf").attr("src", "/files/comprobantes/" + data.d + "?" + version + "#zoom=100&view=FitH,top");
                $("#lnkDescargar").attr("href", "/files/comprobantes/" + data.d);


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
    //}
    //else {
    //    return false;
    //}
}

function abrirModalCheque() {


    var esPropio = 0
    var empresa = 0

    if ($("#ddlFormaPago").val() == "Cheque Propio") {
        esPropio = 1;
    }
    if ($("#ddlFormaPago").val() == "Cheque Empresa") {
        esPropio = 1;
        empresa = 1;
    }

    Common.showModalCheque(esPropio, empresa, "COBRANZA");
}

function cancelar() {
    window.location.href = "/cobranzas.aspx";
}

var cobranzas = {
    configForm: function ()
    {
        $("#hdnModo").val("COBRANZA");
        $(".select2").select2({
            width: '100%', allowClear: true
        });
        
        Common.obtenerBancos('ddlBancos', "");
        // Date Picker
        configDatePicker();
        $("#txtCantidad").numericInput();

        $("#txtNumero").mask("?99999999");
        $("#txtNumero").blur(function () {
            $("#txtNumero").val(padZeros($("#txtNumero").val(), 8));
        });

        $("#txtImporte, #txtImporteForma, #txtImporteRet").maskMoney({ thousands: '', decimal: ',', allowZero: true });

        $("#txtEnvioPara, #txtEnvioAsunto, #txtEnvioMensaje").keypress(function (event) {
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


        $("#txtEnvioPara, #txtEnvioAsunto, #txtEnvioMensaje").blur(function (event) {
            var aux = Toolbar.toggleEnviosError();
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

       

        Common.obtenerPersonas("ddlPersona", "", true);
        
        if ($("#hdnID").val() != "" && $("#hdnID").val() != "0") {
            //Common.obtenerPuntosDeVenta("ddlPuntoVenta");
            cobranzas.loadInfo($("#hdnID").val());
        }
        else {
            Common.obtenerPuntosDeVentaYNroCobranza("ddlPuntoVenta");
            Common.obtenerPersonas("ddlPersona", $("#hdnIDPersona").val(), true);
            $("#ddlPersona").attr("onchange", "changePersona()");
            //setTimeout(changeTipoComprobante(), 5000);

            if (parseInt($("#hdntieneComprobante").val()) > 0) {
                changeFormas();
                setTimeout(Common.obtenerPersonas("ddlPersona", $("#hdnIDPersona").val(), true), 5000);
                $("#divFactura").show()
            }
        }

        $("#txtNumero").attr("disabled", true);
    },
    loadInfo: function (id)
    {
        $.ajax({
            type: "POST",
            url: "cobranzase.aspx/obtenerDatos",
            data: "{ id: " + id + "}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: false,
            success: function (data, text) {
                if (data.d != null) {
                    limpiarNuevoComprobante();

                    var idPersona = 0;
                    idPersona = data.d.IDPersona;
                    Common.obtenerPersonas("ddlPersona", idPersona, true);
                    
                    $("#txtFecha").val(data.d.Fecha);
                    $("#ddlPuntoVenta").val(data.d.IDPuntoVenta);
                    $("#txtNumero").val(data.d.Numero);
                    $("#txtImporteForma").val(data.d.Numero);
                    $("#txtObservaciones").val(data.d.Observaciones);

                    obtenerItems();
                    obtenerFormas();
                    obtenerTotales();
                    CobRetenciones.obtenerRetenciones();
                    
                    obtenerComprobantesPendientes(idPersona);

                    $("#hdnRazonSocial").val(data.d.Personas.RazonSocial);
                    Common.obtenerComprobantesCobranzasPorCondicion("ddlTipo", data.d.Personas.CondicionIva, data.d.Tipo);

                    $("#divFactura,#lnkAceptar,#lnkPrevisualizar").show();
                    setTimeout(function () {
                        $("#ddlPersona").attr("onchange", "changePersona()");
                    }, 1000);
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
}
function enviarCobranzaAutomaticamente(nombre) {
    
    var info = "{ idCobranza: " + parseInt($("#hdnID").val())
              + ",nombre: '" + nombre
              + "'}";
    $.ajax({
        type: "POST",
        url: "/cobranzase.aspx/EnviarCobranzaAutomaticamente",
        data: info,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            $("#spSendMail").html("Enviado");
            $("#imgMailEnvio").attr("style", "color:#17a08c;font-size: 30px;");
            $("#spSendMail").attr("style", "color:#17a08c");
            $("#iCheckEnvio").addClass("fa fa-check");
            $('#modalOk').modal('show');
        },
        error: function (response) {
            var r = jQuery.parseJSON(response.responseText);
            $("#msgErrorEnvioCR").html(r.Message);
            $("#divErrorEnvioCR").show();
            $("#divCajaEmail").show();
            $("#iCheckEnvio").removeClass("fa fa-check");
            $('#modalOk').modal('show');
        }
    });
}

function generarPDF() {

    if ($('#frmEdicion').valid()) {
        var info = "{id: " + parseInt($("#hdnID").val())
                    + ", idPersona: " + parseInt($("#ddlPersona").val())
                    + ", tipo: '" + $("#ddlTipo").val()
                    //+ "', modo: '" + $("#ddlModo").val()
                    + "', fecha: '" + $("#txtFecha").val()
                    + "', idPuntoVenta: " + $("#ddlPuntoVenta").val()
                    + ", nroComprobante: '" + $("#txtNumero").val()
                    + "', obs: '" + $("#txtObservaciones").val()
                    + "'}";

        $.ajax({
            type: "POST",
            url: "cobranzase.aspx/generarPDF",
            data: info,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data, text) {

                //generar CAE
                //Seteo la opcion de imprimir
                var version = new Date().getTime();
                $("#ifrPdf").attr("src", "/files/explorer/" + MI_IDUSUARIO + "/comprobantes/" + new Date().getFullYear() + "/" + data.d + "?" + version + "#zoom=100&view=FitH,top");

                //Seteo el link de download
                var fileName = data.d;
                $("#lnkDownloadPdf").attr("href", "/pdfGenerator.ashx?file=" + fileName);

                //Seteo el nombre del archivo para envio de mail
                $("#hdnFile").val(fileName);

                //Muestro la ventana
                //$('#modalOk').modal('show');

                if ($("#hdnEnvioCR").val() == "1") {
                    enviarCobranzaAutomaticamente(fileName);
                }
                else {
                    $('#modalOk').modal('show');
                }
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

/*** FORM ALTA ***/

function changePersona() {

    limpiarNuevoComprobante();
    if ($("#ddlPersona").val() != "") {
        $("#divFactura,#lnkAceptar,#lnkPrevisualizar").show();
        obtenerInfoPersona(parseInt($("#ddlPersona").val()), '');
        var idPersona = parseInt($("#ddlPersona").val());
        obtenerComprobantesPendientes(idPersona);
        obtenerFormasDePagoCobranzas("", idPersona);
    }
    else {
        $("#divFactura,#lnkAceptar,#lnkPrevisualizar").hide();
    }
}

function limpiarNuevoComprobante() {
    ocultarMensajes();
    //$("#txtNumero").val("");
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
                $("#hdnRazonSocial").val(data.d.RazonSocial);
                $("#txtEnvioPara").val(data.d.Email);
            }
        },
        error: function (response) {
            var r = jQuery.parseJSON(response.responseText);
            $("#msgError").html(r.Message);
            $("#divError").show();
        }
    });
}

function changeTipoComprobante() {
    
    if (parseInt($("#hdnID").val()) == 0) {
        Common.obtenerUltimoNroRecibo("txtNumero", $("#ddlTipo").val(), parseInt($("#ddlPuntoVenta").val()));
    }
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
    var idNotaCredito = ($("#ddlNotaCredito").val() == null ? "" : $("#ddlNotaCredito").val());


    //if ($("#ddlFormaPago").val() != "Efectivo" &&
    //    $("#ddlFormaPago").val() != "Nota de credito" &&
    //    $("#ddlFormaPago").val() != "Tarjeta de credito" &&
    //    $("#ddlFormaPago").val() != "Tarjeta de debito" &&
    //    $("#ddlBancos").val() == "") {

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
                    + "', idNotaCredito: '" + idNotaCredito
                    + "'}";

            $.ajax({
                type: "POST",
                url: "cobranzase.aspx/agregarForma",
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
        url: "cobranzase.aspx/eliminarForma",
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

function modificarForma(id, forma, nroRef, importe,idBanco,idCheque,idNotaCredito) {

    $("#ddlFormaPago").val(forma).trigger("change");
    $("#ddlCheque").val(idCheque).trigger("change");
    $("#ddlBancos").val(idBanco).trigger("change");
    $("#ddlNotaCredito").val(idNotaCredito).trigger("change");

    $("#txtNroRef").val(nroRef);
    $("#txtImporteForma").val(importe);

    $("#hdnIDForma").val(id);
    $("#btnAgregarForma").html("Actualizar");
}

function obtenerFormas() {

    $.ajax({
        type: "GET",
        url: "cobranzase.aspx/obtenerFormas",
        //data: "{idFactura: " + parseInt(dataItem.ID) + "}",
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

function changeFormas() {

    $("#ddlCheque").val("").trigger("change");

    if ($("#ddlFormaPago").val() == "") {
        $("#ddlFormaPago").val("Efectivo");
    }    

    if ($("#ddlFormaPago").val() == "Cheque Tercero") {
        Common.obtenerChequesConResto('ddlCheque', "", true, false, false);
        $("#divCheque,#divNuevoCheque").show();
        //$("#txtImporteForma,#txtNroRef").attr("disabled", true);
        $("#txtNroRef").attr("disabled", true);
    }
    else if ($("#ddlFormaPago").val() == "Cheque Propio") {
        Common.obtenerChequesConResto('ddlCheque', "", true, true, false);
        $("#divCheque,#divNuevoCheque").show();
        //$("#txtImporteForma,#txtNroRef").attr("disabled", true);
        $("#txtNroRef").attr("disabled", true);
    }
    else if ($("#ddlFormaPago").val() == "Cheque Empresa") {
        Common.obtenerChequesConResto('ddlCheque', "", true, true, true);
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

function changeNotaCredito() {

    if ($("#ddlNotaCredito").val() != "" && $("#ddlFormaPago").val() == "Nota de credito") {
        var NotaCredito = $("#ddlNotaCredito option:selected").text().split("$");

        var importe = NotaCredito[1].replace(")", "").trim();
        importe = importe.replace(".", "");
        var nroRef = (NotaCredito[0].split("(Saldo:")[0]).trim();

        $("#txtImporteForma").val(importe)
        $("#txtNroRef").val(nroRef)
    }
    else {
        $("#txtImporteForma,#txtNroRef").val("")
    }
}

function changeChequeTercero() {

    if ($("#ddlCheque").val() != "" && ($("#ddlFormaPago").val() == "Cheque Tercero" || $("#ddlFormaPago").val() == "Cheque Propio" || $("#ddlFormaPago").val() == "Cheque Empresa")) {
        var cheque = $("#ddlCheque option:selected").text().split("$");
        //var importe = cheque[1];


        var posicionResto = cheque[1].search("Resta");
        var importe = cheque[1].substr(posicionResto + 6, cheque[1].length).replace(')','');

        var nroRef = cheque[0].split("Nro:")[1];

        $("#txtImporteForma").val(importe.replace(".", ""))
        $("#txtNroRef").val(nroRef)
    }
    else {
        $("#txtImporteForma,#txtNroRef").val("")
    }
}

function obtenerFormasDePagoCobranzas(idSelected, idPersona) {
    $.ajax({
        type: "POST",
        url: "/cobranzase.aspx/obtenerFormasDePagoCobranzas",
        data: "{ idPersona: " + idPersona + "}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            if (data != null) {
                for (var i = 0; i < data.d.length; i++) {
                    $("<option/>").attr("value", data.d[i].ID).text(data.d[i].Nombre).appendTo($("#ddlNotaCredito"));
                }
            }
            if (idSelected != "")
                $("#ddlNotaCredito").val(idSelected).trigger("change");
        }
    });
}

function obtenerTotalFormas() {

    $.ajax({
        type: "POST",
        url: "cobranzase.aspx/obtenerFormasTotal",
        //data: "{idFactura: " + parseInt(dataItem.ID) + "}",
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
                    url: "cobranzase.aspx/agregarRetencion",
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
            url: "cobranzase.aspx/eliminarRetencion",
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
            url: "cobranzase.aspx/obtenerRetenciones",
            //data: "{idFactura: " + parseInt(dataItem.ID) + "}",
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
        //if (parseFloat($("#txtImporte").val()) == 0) {
        //    $("#msgErrorDetalle").html("El importe debe ser mayor a 0.");
        //    $("#divErrorDetalle").show();
        //}
        //else {
            var idComprobante = $("#ddlComprobante").val();
            var info = "{ id: " + parseInt($("#hdnIDItem").val())
                    + ", idComprobante: '" + $("#ddlComprobante").val()
                    + "', comprobante: '" + $("#ddlComprobante option:selected").text()
                    + "', importe: '" + $("#txtImporte").val()
                    + "'}";

            $.ajax({
                type: "POST",
                url: "cobranzase.aspx/agregarItem",
                data: info,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data, text) {
                    $("#txtImporte").val("");
                    $("#lblSaldo").html("");
                    $("#ddlComprobante").val("").trigger("change");
                    $("#hdnIDItem").val("0");
                    $("#btnAgregarItem").html("Agregar");

                    var selectobject = document.getElementById("ddlComprobante");
                    for (var i = 0; i < selectobject.length; i++) {
                        if (selectobject.options[i].value == idComprobante)
                            selectobject.remove(i);
                    }

                    obtenerItems();
                    obtenerTotales();
                },
                error: function (response) {
                    var r = jQuery.parseJSON(response.responseText);
                    $("#msgErrorDetalle").html(r.Message);
                    $("#divErrorDetalle").show();
                }
            });
        //}
    }
    else {
        $("#msgErrorDetalle").html("Debes ingresar la cantidad, concepto y precio.");
        $("#divErrorDetalle").show();
    }
}

function eliminarItem(id) {

    var info = "{ id: " + parseInt(id) + "}";

    $.ajax({
        type: "POST",
        url: "cobranzase.aspx/eliminarItem",
        data: info,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data, text) {
            var idPersona = parseInt($("#ddlPersona").val());
            obtenerComprobantesPendientes(idPersona);
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

function modificarItem(id, idComprobante, importe, textoImporte) {

    $("#txtImporte").val(importe);
    //$("#txtRetGanancias").val(retGanancias);
    //$("#txtIIBB").val(iibb);
    //$("#txtSuss").val(suss);
    //$("#txtOtros").val(otros);

    $("<option/>").attr("value", idComprobante).text(textoImporte).appendTo($("#ddlComprobante"));
    $("#ddlComprobante").val(idComprobante).trigger("change");

    changeComprobante();

    $("#hdnIDItem").val(id);
    $("#btnAgregarItem").html("Actualizar");

}

function obtenerItems() {

    $.ajax({
        type: "GET",
        url: "cobranzase.aspx/obtenerItems",
        //data: "{idFactura: " + parseInt(dataItem.ID) + "}",
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
    Common.soloNumerosConGuiones("txtNumero");
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
    window.location.href = "/cobranzase.aspx";
}

function editar(id) {
    window.location.href = "/cobranzase.aspx?ID=" + id;
}

function eliminar(id, nombre) {
    bootbox.confirm("¿Está seguro que desea eliminar el recibo realizado a " + nombre + "?", function (result) {
        if (result) {
            $.ajax({
                type: "POST",
                url: "cobranzas.aspx/delete",
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

    if ($('#frmSearch').valid())
    {
        $("#resultsContainer").html("");
        var currentPage = parseInt($("#hdnPage").val());

        var idPersona = 0;
        if ($("#ddlPersona").val() != null && $("#ddlPersona").val() != "")
            idPersona = parseInt($("#ddlPersona").val());

        var info = "{ idPersona: " + idPersona
                   + " , condicion: '" + $("#txtCondicion").val()
                   + "', periodo: '" + $("#ddlPeriodo").val()
                   + "', fechaDesde: '" + $("#txtFechaDesde").val()
                   + "', fechaHasta: '" + $("#txtFechaHasta").val()
                   + "', page: " + currentPage + ", pageSize: " + PAGE_SIZE
                   + "}";

        $.ajax({
            type: "POST",
            url: "cobranzas.aspx/getResults",
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
    $("#txtCondicion, #txtFechaDesde, #txtFechaHasta").val("");
    $("#ddlPersona").val("").trigger("change");
    filtrar();
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

function exportar() {
    resetearExportacion();

    $("#imgLoading").show();
    $("#divIconoDescargar").hide();

    var idPersona = 0;
    if ($("#ddlPersona").val() != "")
        idPersona = parseInt($("#ddlPersona").val());

    var info = "{ idPersona: " + idPersona
              + " , condicion: '" + $("#txtCondicion").val()
              + "', periodo: '" + $("#ddlPeriodo").val()
              + "', fechaDesde: '" + $("#txtFechaDesde").val()
              + "', fechaHasta: '" + $("#txtFechaHasta").val()
              + "'}";

    $.ajax({
        type: "POST",
        url: "cobranzas.aspx/export",
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
