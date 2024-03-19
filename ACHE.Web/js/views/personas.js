/*** FORM ***/
var geocoder;
var map;
function armarSelectSegunCondicionIVA() {

    var valor = $("#ddlTipoDoc").val();
    var tipo = $("#hdnTipo").val();

    if ($("#ddlCondicionIva").val() == "CF") {
        $("#ddlTipoDoc").html("");
        $("<option/>").attr("value", "").text("").appendTo($("#ddlTipoDoc"));
        $("<option/>").attr("value", "DNI").text("DNI").appendTo($("#ddlTipoDoc"));
        $("<option/>").attr("value", "CUIT").text("CUIT").appendTo($("#ddlTipoDoc"));
    }
    else {
        $("#ddlTipoDoc").html("");
        $("<option/>").attr("value", "").text("").appendTo($("#ddlTipoDoc"));

        $("<option/>").attr("value", "DNI").text("DNI").appendTo($("#ddlTipoDoc"));
        $("<option/>").attr("value", "CUIT").text("CUIT").appendTo($("#ddlTipoDoc"));
    }

    $("<option/>").attr("value", "SIN CUIT").text("SIN CUIT").appendTo($("#ddlTipoDoc"));
   

    if (valor != null && valor != "") {
        $("#ddlTipoDoc").val(valor);
        $("#ddlTipoDoc").trigger("change");
    }
}

function changeTipoDoc() {
    //$("#txtNroDocumento").val("");
}

function sugerirNumeroCuitGenerico() {
    $.ajax({
        type: "POST",
        url: "common.aspx/sugerirNumeroCuitGenerico",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data, text) {
            if (data != null) {
                $("#txtNroDocumento").val(data.d);
            }
        },
        error: function (response) {
            var r = jQuery.parseJSON(response.responseText);
            $("#msgError").html(r.Message);
            $("#divError").show();
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
        + ", verComo: " + $("input[name='rCtaCte']:checked").attr('value') + "}";

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
            alert(r.Message);
        }
    });
}

function RealizarCobranza() {
    window.location.href = "/cobranzase.aspx";
}

function changeDomiciliosProvincia() {
    Common.obtenerCiudades("ddlDomiciliosCiudad", $("#ddlDomiciliosCiudad").val(), $("#ddlDomiciliosProvincia").val(), true);
}


function changeDomicilio() {
    if ($("#ddlDomicilio").val() != "") {
        $.ajax({
            type: "POST",
            url: "common.aspx/ObtenerDetalleDomicilio",
            data: "{ id: " + parseInt($("#ddlDomicilio").val()) + "}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data, text) {
                if (data.d != null) {
                    $("#txtDomiciliosDomicilio").val(data.d.Domicilio);
                    $("#txtDomiciliosPisoDepto").val(data.d.PisoDepto);
                    $("#txtDomiciliosCodigoPostal").val(data.d.CodigoPostal);
                    $("#txtDomiciliosProvinciaTexto").val(data.d.Provincia);
                    $("#txtDomiciliosCiudadTexto").val(data.d.CiudadTexto);
                    $("#txtDomiciliosContacto").val(data.d.Contacto);
                    $("#txtDomiciliosTelefono").val(data.d.Telefono);
                    $("#ddlDomiciliosProvincia").val(data.d.IDProvincia).trigger("change");
                    $("#ddlDomiciliosCiudad").val(data.d.IDCiudad).trigger("change");
                    $("#divDomiciliosDetalle").show();
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

function changeDomicilioTransporte() {
    if ($("#ddlDomicilioTransporte").val() != "") {
        $.ajax({
            type: "POST",
            url: "common.aspx/ObtenerDetalleDomicilioTransporte",
            data: "{ id: " + parseInt($("#ddlDomicilioTransporte").val()) + "}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data, text) {
                if (data.d != null) {
                    $("#txtDomiciliosTransporteRazonSocial").val(data.d.RazonSocial);
                    $("#txtDomiciliosTransporteDomicilio").val(data.d.Domicilio);
                    $("#txtDomiciliosTransportePisoDepto").val(data.d.PisoDepto);
                    $("#txtDomiciliosTransporteCodigoPostal").val(data.d.CodigoPostal);
                    $("#txtDomiciliosTransporteProvinciaTexto").val(data.d.Provincia);
                    $("#txtDomiciliosTransporteCiudadTexto").val(data.d.CiudadTexto);
                    $("#txtDomiciliosTransporteContacto").val(data.d.Contacto);
                    $("#txtDomiciliosTransporteTelefono").val(data.d.Telefono);                    
                    $("#divDomiciliosTransporteDetalle").show();
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

function consultarDatosAfip() {
    limpiarCampos();
    //alert("RECARGAR CLIENTES");
    var cuit = $("#txtNroDocumento").val();
    var tipoDoc = $("#ddlTipoDoc").val();

    if (tipoDoc == 'CUIT') {
        if (cuit != '') {
            if (cuit.length == 11) {
                $.ajax({
                    type: "POST",
                    url: "common.aspx/consultarDatosAfip",
                    data: "{ cuit: " + cuit + "}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data, text) {
                        if (data.d != null) {

                            if (data.d.Mensaje == null) {
                                if (data.d.CUIT != null) {
                                    $("#msgOk").html('Encontramos datos en AFIP con el CUIT ingresado.');
                                    $("#divOk").show();
                                    $("#ddlTipoDoc").val('CUIT');
                                    $("#txtRazonSocial").val(data.d.RazonSocial);
                                    $("#ddlCondicionIva").val(data.d.CategoriaImpositiva);
                                    $("#ddlPersoneria").val(data.d.Personeria = 'FISICA' ? 'F' : 'J');
                                    $("#txtDomicilio").val(data.d.DomicilioFiscalDomicilio);
                                    $("#txtCp").val(data.d.DomicilioFiscalCP);
                                    $("#ddlProvincia").val(data.d.IdProvincia).trigger("change");
                                    $("#ddlCiudad").val(data.d.IdCiudad).trigger("change");
                                    $("#txtProvinciaDesc").val(data.d.DomicilioFiscalProvincia);
                                    $("#txtCiudadDesc").val(data.d.DomicilioFiscalCiudad);
                                    //Common.obtenerProvincias("ddlProvincia", data.d.IdProvincia, false);
                                    //Common.obtenerCiudades("ddlCiudad", data.d.IdCiudad, data.d.IdProvincia, false);
                                    //$("#ddlCiudad").trigger("change");
                                    $("#txtNroDocumento").css("background-color", "#e8ffe3");
                                    $("#ddlTipoDoc").css("background-color", "#e8ffe3");
                                    $("#txtRazonSocial").css("background-color", "#e8ffe3");
                                    $("#ddlCondicionIva").css("background-color", "#e8ffe3");
                                    $("#ddlPersoneria").css("background-color", "#e8ffe3");
                                    $("#txtDomicilio").css("background-color", "#e8ffe3");
                                    $("#txtProvinciaDesc").css("background-color", "#e8ffe3");
                                    $("#txtCiudadDesc").css("background-color", "#e8ffe3");
                                    $("#txtCp").css("background-color", "#e8ffe3");
                                } else {
                                    $("#msgError").html('No encontramos datos en AFIP con el CUIT ingresado.');
                                    $("#divError").show();
                                }
                            } else {
                                $("#msgError").html(data.d.Mensaje);
                                $("#divError").show();
                            }
                        }
                    },
                    error: function (response) {
                        var r = jQuery.parseJSON(response.responseText);
                        $("#msgError").html(r.Message);
                        $("#divError").show();
                    }
                });
            } else {
                $("#msgError").html('CUIT Invalido.');
                $("#divError").show();
            }
        } else {
            $("#msgError").html('Debe completar el campo CUIT.');
            $("#divError").show();
        }
    } else {
        $("#msgError").html('Búsqueda solo por CUIT.');
        $("#divError").show();
    }
}

function limpiarCampos() {
    $("#txtNroDocumento").css("background-color", "#ffffff");
    $("#ddlTipoDoc").css("background-color", "#ffffff");
    $("#txtRazonSocial").css("background-color", "#ffffff");
    $("#ddlCondicionIva").css("background-color", "#ffffff");
    $("#ddlPersoneria").css("background-color", "#ffffff");
    $("#txtDomicilio").css("background-color", "#ffffff");
    $("#txtProvinciaDesc").css("background-color", "#ffffff");
    $("#txtCiudadDesc").css("background-color", "#ffffff");
    $("#txtCp").css("background-color", "#ffffff");
    $("#txtRazonSocial").val('');
    $("#ddlCondicionIva").val('');
    $("#ddlPersoneria").val('');
    $("#txtDomicilio").val('');
    $("#txtProvinciaDesc").val('');
    $("#txtCiudadDesc").val('');
    $("#txtCp").val('');
    $("#txtPisoDepto").val('');
    $("#txtObservaciones").val('');
    $("#txtSaldo").val('');
    $("#txtTelefono").val('');
    $("#txtEmail").val('');
    $("#txtCodigo").val('');
    $("#txtNombreFantasia").val('');
    $("#divError").hide();
    $("#divOk").hide();
}

function grabar() {
    $("#divError").hide();
    $("#divOk").hide();

    if ($("#ddlTipoDoc").val() != "SIN CUIT" && $("#ddlTipoDoc").val() != "DNI") {
        if ($("#txtNroDocumento").val() == "") {
            $("#msgError").html("Debe completar el campo número");
            $("#divError").show();
            return false;
        }
    }

    var id = ($("#hdnID").val() == "" ? "0" : $("#hdnID").val());
    var ciudad = (($("#ddlCiudad").val() == "" || $("#ddlCiudad").val() == null) ? "5003" : $("#ddlCiudad").val());

    var saldoInicial = ($("#txtSaldo").val() == "") ? 0 : parseFloat($("#txtSaldo").val().replace(",", "."));
    var porcentajeDescuento = ($("#txtPorcentajeDescuento").val() == "") ? 0 : parseFloat($("#txtPorcentajeDescuento").val().replace(",", "."));

    if ($("#ddlCondicionIva").val() == "CF") {
        $("#ddlPersoneria").val("F");
        $("#ddlTipoDoc").val("DNI");
        $("#ddlTipoDoc").trigger("change");
    }    

    if ($('#frmEdicion').valid()) {
        Common.mostrarProcesando("btnActualizar");
        var info = "{ id: " + parseInt(id)
                + " , razonSocial: '" + $("#txtRazonSocial").val()
                + "', nombreFantasia: '" + $("#txtNombreFantasia").val()
                + "', condicionIva: '" + $("#ddlCondicionIva").val()
                + "', personeria: '" + $("#ddlPersoneria").val()
                + "', tipoDoc: '" + $("#ddlTipoDoc").val()
                + "', nroDoc: '" + $("#txtNroDocumento").val()
                + "', telefono: '" + $("#txtTelefono").val()
                + "', email: '" + $("#txtEmail").val()
                + "', tipo: '" + $("#hdnTipo").val()
                + "', idProvincia: " + $("#ddlProvincia").val()
                + " , idCiudad: " + ciudad
                + " , provinciaDesc: '" + $("#txtProvinciaDesc").val()
                + "', ciudadDesc: '" + $("#txtCiudadDesc").val()
                + "', domicilio: '" + $("#txtDomicilio").val()
                + "', pisoDepto: '" + $("#txtPisoDepto").val()
                + "', cp: '" + $("#txtCp").val()
                + "', obs: '" + $("#txtObservaciones").val()
                + "', listaPrecio: " + parseInt($("#ddlListaPrecios").val())
                + " , codigo: '" + $("#txtCodigo").val()
                + "', saldoInicial: " + saldoInicial
                + ", porcentajeDescuento: " + porcentajeDescuento
                + ", avisos: '" + $("#txtAvisos").val()
                + "', idRango: " + $("#ddlRango").val()
                + "}";

        $.ajax({
            type: "POST",
            url: "personase.aspx/guardar",
            data: info,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: false,
            success: function (data, text) {
                $("#hdnID").val(data.d);

                // if ($("#hdnSinCombioDeFoto").val() == "0") {
                $('#divOk').show();
                $("#divError").hide();
                $('html, body').animate({ scrollTop: 0 }, 'slow');

                window.location.href = "/personas.aspx?tipo=" + $("#hdnTipo").val();
                //}

                Common.ocultarProcesando("btnActualizar", "Guardar cliente");
            },
            error: function (response) {
                var r = jQuery.parseJSON(response.responseText);
                $("#msgError").html(r.Message);
                $("#divError").show();
                $("#divOk").hide();
                $('html, body').animate({ scrollTop: 0 }, 'slow');
                Common.ocultarProcesando("btnActualizar", "Guardar cliente");
            }
        });
    }
    else {
        return false;
    }
}

function domiciliosGrabar() {
    $("#divDomiciliosError").hide();
    $("#divDomiciliosOk").hide();

    if ($("#ddlDomicilio").val() == "") {
        $("#msgDomiciliosError").html("Debe seleccionar un domicilio");
        $("#divDomiciliosError").show();
        $("#divDomiciliosOk").hide();
        return;
    }
    if ($("#ddlDomiciliosProvincia").val() == "" || $("#ddlDomiciliosProvincia").val() == null) {
        $("#msgDomiciliosError").html("Debe seleccionar una provincia");
        $("#divDomiciliosError").show();
        $("#divDomiciliosOk").hide();
        return;
    }
    if ($("#ddlDomiciliosCiudad").val() == "" || $("#ddlDomiciliosCiudad").val() == null) {
        $("#msgDomiciliosError").html("Debe seleccionar una ciudad");
        $("#divDomiciliosError").show();
        $("#divDomiciliosOk").hide();
        return;
    }

    var info = "{ id: " + parseInt($("#ddlDomicilio").val())
        + " , idPersona: " + parseInt($("#hdnID").val())
        + " , domicilio: '" + $("#txtDomiciliosDomicilio").val()
        + "', pisoDepto: '" + $("#txtDomiciliosPisoDepto").val()
        + "', codigoPostal: '" + $("#txtDomiciliosCodigoPostal").val()
        + "', idProvincia: " + $("#ddlDomiciliosProvincia").val()
        + " , idCiudad: " + $("#ddlDomiciliosCiudad").val()
        + " , provinciaTexto: '" + $("#txtDomiciliosProvinciaTexto").val()
        + "' , ciudadTexto: '" + $("#txtDomiciliosCiudadTexto").val()
        + "' , contacto: '" + $("#txtDomiciliosContacto").val()
        + "' , telefono: '" + $("#txtDomiciliosTelefono").val()
        + "'}";
    $.ajax({
        type: "POST",
        url: "personase.aspx/guardarDomicilio",
        data: info,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,
        success: function (data, text) {
            $('#divDomiciliosOk').show();
            $("#divDomiciliosError").hide();
            $("#divDomiciliosDetalle").hide();
            setTimeout('document.location.reload()', 4000);
        },
        error: function (response) {
            var r = jQuery.parseJSON(response.responseText);
            $("#msgDomiciliosError").html(r.Message);
            $("#divDomiciliosError").show();
            $("#divDomiciliosOk").hide();
        }
    });
    

}

function domiciliosTransporteGrabar() {
    $("#divDomiciliosTransporteError").hide();
    $("#divDomiciliosTransporteOk").hide();

    if ($("#ddlDomicilioTransporte").val() == "") {
        $("#msgDomiciliosTransporteError").html("Debe seleccionar un domicilio");
        $("#divDomiciliosTransporteError").show();
        $("#divDomiciliosTransporteOk").hide();
        return;
    }

    var info = "{ id: " + parseInt($("#ddlDomicilioTransporte").val())
        + " , idPersona: " + parseInt($("#hdnID").val())
        + " , razonSocial: '" + $("#txtDomiciliosTransporteRazonSocial").val()
        + "', domicilio: '" + $("#txtDomiciliosTransporteDomicilio").val()
        + "', pisoDepto: '" + $("#txtDomiciliosTransportePisoDepto").val()
        + "', codigoPostal: '" + $("#txtDomiciliosTransporteCodigoPostal").val()        
        + "', provinciaTexto: '" + $("#txtDomiciliosTransporteProvinciaTexto").val()
        + "', ciudadTexto: '" + $("#txtDomiciliosTransporteCiudadTexto").val()
        + "', contacto: '" + $("#txtDomiciliosTransporteContacto").val()
        + "', telefono: '" + $("#txtDomiciliosTransporteTelefono").val()
        + "'}";
    $.ajax({
        type: "POST",
        url: "personase.aspx/guardarDomicilioTransporte",
        data: info,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,
        success: function (data, text) {
            $('#divDomiciliosTransporteOk').show();
            $("#divDomiciliosTransporteError").hide();
            $("#divDomiciliosTransporteDetalle").hide();
            setTimeout('document.location.reload()', 4000);
        },
        error: function (response) {
            var r = jQuery.parseJSON(response.responseText);
            $("#msgDomiciliosTransporteError").html(r.Message);
            $("#divDomiciliosTransporteError").show();
            $("#divDomiciliosTransporteOk").hide();
        }
    });


}

function domiciliosEliminar() {
    $("#divDomiciliosError").hide();
    $("#divDomiciliosOk").hide();

    bootbox.confirm("¿Está seguro que desea eliminar el domicilio?", function (result) {
        if (result) {
            var info = "{ id: " + parseInt($("#ddlDomicilio").val())
                + "}";
            $.ajax({
                type: "POST",
                url: "personase.aspx/eliminarDomicilio",
                data: info,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: false,
                success: function (data, text) {
                    $('#divDomiciliosOk').show();
                    $("#divDomiciliosError").hide();
                    $("#divDomiciliosDetalle").hide();
                    setTimeout('document.location.reload()', 4000);
                },
                error: function (response) {
                    var r = jQuery.parseJSON(response.responseText);
                    $("#msgDomiciliosError").html(r.Message);
                    $("#divDomiciliosError").show();
                    $("#divDomiciliosOk").hide();
                }
            });
        }
    });
}

function domiciliosTransporteEliminar() {
    $("#divDomiciliosTransporteError").hide();
    $("#divDomiciliosTransporteOk").hide();

    bootbox.confirm("¿Está seguro que desea eliminar el domicilio transporte?", function (result) {
        if (result) {
            var info = "{ id: " + parseInt($("#ddlDomicilioTransporte").val())
                + "}";
            $.ajax({
                type: "POST",
                url: "personase.aspx/eliminarDomicilioTransporte",
                data: info,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: false,
                success: function (data, text) {
                    $('#divDomiciliosTransporteOk').show();
                    $("#divDomiciliosTransporteError").hide();
                    $("#divDomiciliosTransporteDetalle").hide();
                    setTimeout('document.location.reload()', 4000);
                },
                error: function (response) {
                    var r = jQuery.parseJSON(response.responseText);
                    $("#msgDomiciliosTransporteError").html(r.Message);
                    $("#divDomiciliosTransporteError").show();
                    $("#divDomiciliosTransporteOk").hide();
                }
            });
        }
    });
}



function cancelar() {
    window.location.href = "/personas.aspx?tipo=" + $("#hdnTipo").val();
}

function configForm() {
    $(".select2").select2({ width: '100%', allowClear: true });

    $("#txtNroDocumento").numericInput();

    Common.obtenerProvincias("ddlProvincia", $("#hdnProvincia").val(), true);
    Common.obtenerCiudades("ddlCiudad", $("#hdnCiudad").val(), $("#hdnProvincia").val(), true);

    Common.obtenerProvincias("ddlDomiciliosProvincia", $("#hdnDomiciliosProvincia").val(), true);
    Common.obtenerCiudades("ddlDomiciliosCiudad", $("#hdnDomiciliosCiudad").val(), $("#hdnDomiciliosProvincia").val(), true);

    if ($("#hdnTipo").val() == "c") {
        Common.obtenerListaPrecios("ddlListaPrecios", $("#hdnIDListaPrecio").val(), true);
        $("#divListaClientes").show()
        $("#rCtaCteCliente").attr("checked", true)
    }
    else {
        $("#divListaClientes").hide()
        $("#rCtaCteProv").attr("checked", true)
    }

    $("#txtSaldo,#txtPorcentajeDescuento").maskMoney({ thousands: '', decimal: ',', allowZero: true, allowNegative: true });

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

    if ($("#txtPorcentajeDescuento").val() != "") {
        var porcentajeDescuento = parseFloat($("#txtPorcentajeDescuento").val());
        $("#txtPorcentajeDescuento").val(addSeparatorsNF(porcentajeDescuento.toFixed(2), '.', ',', '.'));
    }
    if ($("#txtSaldo").val() != "") {
        var saldo = parseFloat($("#txtSaldo").val());
        $("#txtSaldo").val(addSeparatorsNF(saldo.toFixed(2), '.', ',', '.'));
    }

    if ($("#hdnID").val() != "" && $("#hdnID").val() != "0") {
        $("#btnAcciones").show();
        Common.obtenerDomicilios("ddlDomicilio", "", true, $("#hdnID").val());
        Common.obtenerTransportePersona("ddlDomicilioTransporte", "", true, $("#hdnID").val());
    }       
    else {
        $("#btnAcciones, #divStats, #divDomicilios").hide();
    }

    $.validator.addMethod("validCuit", function (value, element) {
        var check = true;
        if ($("#ddlTipoDoc").val() == "CUIT") {
            return CuitEsValido($("#txtNroDocumento").val());
        }
        else
            return check;

    }, "CUIT Inválido");

    

    fotos.adjuntarFoto();
    //armarSelectSegunCondicionIVA();

    if ($("#ddlCondicionIva").val() == "CF") {
        $("#divPersoneria").hide();
        $("#spIdentificacionObligatoria").hide();
        //$("#ddlTipoDoc,#txtNroDocumento").removeClass("required");
    }
    // google.maps.event.addDomListener(window, 'load', initialize);
    Common.configTelefono("txtTelefono");

  
}
function changeProvincia() {
    Common.obtenerCiudades("ddlCiudad", $("#ddlCiudad").val(), $("#ddlProvincia").val(), true);
}

function realizarAccion(pag) {
    window.location.href = pag + "?IDPersona=" + $("#hdnID").val();
}

function CambiarReporte() {

    if ($("input[name='rCtaCte']:checked").attr('value') == "0") {
        $("#rCtaCteProv").attr("checked", true)
    } else {
        $("#rCtaCteCliente").attr("checked", true)
    }

    var info = "{ idPersona: " + parseInt($("#hdnID").val())
        + ", verComo: " + $("input[name='rCtaCte']:checked").attr('value')
        + ", saldoPendiente: false " 
        + ", deudaPorEDM: false " 
        + "} ";

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
}

function changeCondicionIva() {
    //armarSelectSegunCondicionIVA();
    if ($("#ddlCondicionIva").val() == "CF") {
        //$("#ddlTipoDoc").val("");
        //$("#ddlTipoDoc").trigger("change");
        $("#divPersoneria").hide();
        $("#spIdentificacionObligatoria").hide();
        //$("#ddlTipoDoc,#txtNroDocumento").removeClass("required");
    }
    else if ($("#ddlCondicionIva").val() == "RI") {
        //$("#ddlTipoDoc").val("CUIT");
        //$("#ddlTipoDoc").trigger("change");
        $("#divPersoneria").show();
        $("#spIdentificacionObligatoria").show();
        //$("#ddlTipoDoc,#txtNroDocumento").addClass("required");
    }
    else {
        //$("#ddlTipoDoc").val("");
        //$("#ddlTipoDoc").trigger("change");
        $("#divPersoneria").show();
        $("#spIdentificacionObligatoria").show();
        //$("#ddlTipoDoc,#txtNroDocumento").addClass("required");
    }
}
//*** MAPS***/
function initialize() {
    geocoder = new google.maps.Geocoder();
    var latlng = new google.maps.LatLng(-34.397, 150.644);
    var myOptions = {
        zoom: 15,
        center: latlng,
        mapTypeId: google.maps.MapTypeId.ROADMAP
    }
    if ($("#hdnDireccion").val() != "") {
        codeAddress($("#hdnDireccion").val());
    }
    else {
        codeAddress("Buenos aires");
    }
    map = new google.maps.Map(document.getElementById("gmap-marker"), myOptions);
}

function codeAddress(address) {

    if (geocoder) {
        geocoder.geocode({ 'address': address }, function (results, status) {
            if (status == google.maps.GeocoderStatus.OK) {
                map.setCenter(results[0].geometry.location);
                var marker = new google.maps.Marker({
                    map: map,
                    position: results[0].geometry.location
                });
            } else {
                alert("Geocode was not successful for the following reason: " + status);
            }
        });
    }
}
/*** SEARCH ***/

function configFilters() {
    $("#txtNroDocumento").numericInput();

    $("#txtRazonSocial, #txtNroDocumento").keypress(function (event) {
        var keycode = (event.keyCode ? event.keyCode : event.which);
        if (keycode == '13') {
            resetearPagina();
            filtrar();
            return false;
        }
    });

}

function nuevo() {
    window.location.href = "/personase.aspx?tipo=" + $("#hdnTipo").val();
}
function importar() {

    var tipo = "";

    if ($("#hdnTipo").val() == "c") {
        tipo = "Clientes";
    }
    else {
        tipo = "Proveedores";
    }
    window.location.href = "/importar.aspx?tipo=" + tipo;
}

function editar(id) {
    window.location.href = "/personase.aspx?ID=" + id + "&tipo=" + $("#hdnTipo").val();
}

function eliminar(id, nombre) {
    bootbox.confirm("¿Está seguro que desea eliminar a " + nombre + "?", function (result) {
        if (result) {
            $.ajax({
                type: "POST",
                url: "personas.aspx/delete",
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
    $("#resultsContainer").html("");
    var currentPage = parseInt($("#hdnPage").val());

    var info = "{ condicion: '" + $("#txtRazonSocial").val()
               + "', tipo: '" + $("#hdnTipo").val()
               + "', page: " + currentPage + ", pageSize: " + PAGE_SIZE
               + "}";

    $.ajax({
        type: "POST",
        url: "personas.aspx/getResults",
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
    $("#txtRazonSocial, #txtNroDocumento").val("");
    filtrar();
}

function exportar() {
    resetearExportacion();
    $("#imgLoading").show();
    $("#divIconoDescargar").hide();

    var info = "{ condicion: '" + $("#txtRazonSocial").val()
               + "', tipo: '" + $("#hdnTipo").val()
               + "'}";

    $.ajax({
        type: "POST",
        url: "personas.aspx/export",
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

function configFormSinDatos() {
    if ($("#hdnTipo").val() == "c") {
        $("#hTitulo").text("Aún no has creado ningún cliente");
        $("#btnNuevoSinDatos").text("Crea un Cliente");
    }
    else {
        $("#hTitulo").text("Aún no has creado ningún proveedor");
        $("#btnNuevoSinDatos").text("Crea un proveedor");
    }
}
/*** Adjuntar Foto ***/
var fotos = {
    adjuntarFoto: function () {

        $('#flpArchivo').fileupload({
            url: "/subirImagenes.ashx?idPersona=" + $("#hdnID").val() + "&opcionUpload=persona",
            success: function (response, status) {
                if (response == "OK") {
                    $("#divError").hide();
                    $("#msgOk").html('Los datos se han actualizado correctamente');
                    $("#divOk").show();
                    $("#btnActualizar").attr("disabled", false);
                    window.location.href = "/personas.aspx?tipo=" + $("#hdnTipo").val();
                }
                else {
                    $("#hdnFileName").val("");
                    $("#msgError").html(response);
                    $("#divError").show();
                    $("#divOk").hide();
                    $("#btnActualizar").attr("disabled", false);
                }
            },
            error: function (error) {
                $("#hdnFileName").val("");
                $("#msgError").html(error.responseText);
                $("#imgLoading").hide();
                $("#divError").show();
                $("#divOk").hide();

                $('html, body').animate({ scrollTop: 0 }, 'slow');
                Common.ocultarProcesando("btnActualizar", "Guardar cliente");
            },
            autoUpload: false,
            add: function (e, data) {
                Common.ocultarProcesando("btnActualizar", "Guardar cliente");
                $("#hdnSinCombioDeFoto").val("1");
                $("#btnActualizar").on("click", function () {
                    $("#imgLoading").show();
                    grabar();
                    if ($("#hdnID").val() != "0") {
                        data.url = '/subirImagenes.ashx?idPersona=' + $("#hdnID").val() + "&opcionUpload=persona";
                        data.submit();
                    }
                })
            }
        });
        fotos.showBtnEliminar();
    },

    showInputFoto: function () {
        $("#divLogo").slideToggle();
    },
    grabarsinImagen: function () {
        if ($("#hdnSinCombioDeFoto").val() == "0") {
            grabar();
        }
    },
    eliminarFoto: function () {
        var id = ($("#hdnID").val() == "" ? "0" : $("#hdnID").val());
        if (id != "") {
            var info = "{ idPersona: " + parseInt(id) + "}";

            $.ajax({
                type: "POST",
                url: "personase.aspx/eliminarFoto",
                data: info,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: false,
                success: function (data, text) {
                    $('#divOk').show();
                    $("#divError").hide();
                    $('html, body').animate({ scrollTop: 0 }, 'slow');
                    $("#imgFoto").attr("src", "/files/usuarios/no-cheque.png");
                    $("#hdnTieneFoto").val("0");
                    fotos.showBtnEliminar();
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
            $("#msgError").html("El producto no tiene una imagen guardada");
            $("#divError").show();
            return false;
        }
    },
    showBtnEliminar: function () {
        if ($("#hdnTieneFoto").val() == "1") {
            $("#divEliminarFoto").show();
            $("#divAdjuntarFoto").removeClass("col-sm-12").addClass("col-sm-6");
        }
        else {
            $("#divEliminarFoto").hide();
            $("#divAdjuntarFoto").removeClass("col-sm-6").addClass("col-sm-12");
        }
    },
}