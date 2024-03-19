/*** FORM EDICION ***/
var nroActual = "";//guarda nro del comprobante actual
var tipoActual = "";//guarda tipo del comprobante actual
var esModificacion = false;
function loadInfo(id) {

    $.ajax({
        type: "POST",
        url: "comprobantese.aspx/obtenerDatos",
        data: "{ id: " + id + "}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data, text) {
            if (data.d != null) {
                limpiarNuevoComprobante();


                var idPersona = 0;
                idPersona = data.d.IDPersona;

                $("#ddlModo").val(data.d.Modo);
                if (data.d.Modo == "E")
                    $("#lnkGenerarCAE").show();
                else
                    $("#lnkGenerarCAE").hide();


                $("#txtFecha").val(data.d.Fecha);
                $("#ddlCondicionVenta").val(data.d.CondicionVenta);
                $("#ddlProducto").val(data.d.TipoConcepto);
                $("#txtFechaVencimiento").val(data.d.FechaVencimiento);
                $("#ddlPuntoVenta").val(data.d.IDPuntoVenta);
                $("#ddlModalidadPagoAFIP").val(data.d.ModalidadPagoAFIP);
                //$("#ddlActividad").val(data.d.IDActividad);
                $("#ddlPuntoVentaFac1").val(data.d.IDPuntoVenta);
                $("#ddlPuntoVentaFac2").val(data.d.IDPuntoVenta);
                $("#txtNumero").val(data.d.Numero);
                nroActual = $("#txtNumero").val();
                tipoActual = data.d.Tipo;
                $("#txtObservaciones").val(data.d.Observaciones);
                $("#txtNombre").val(data.d.Nombre);
                $("#txtVendedor").val(data.d.Vendedor);
                $("#ddlEnvio").val(data.d.Envio);
                $("#txtFechaEntrega").val(data.d.FechaEntrega);
                $("#txtFechaAlta").val(data.d.FechaAlta);
                $("#ddlEstado").val(data.d.Estado);

                $("#txtImporteNoGravado").val(data.d.Personas.ImporteNoGravado);
                $("#txtPercepcionIVA").val(data.d.Personas.PercepcionIVA);
                $("#txtPercepcionIIBB").val(data.d.Personas.PercepcionIIBB);
                $("#hdnRazonSocial").val(data.d.Personas.RazonSocial);
                $("#txtEnvioPara").val(data.d.Personas.Email);
                $("#hdnIdCompraVinculada").val(data.d.IDCompraVinculada);
                $("#divDescuento").val(data.d.Descuento);

                $("#litPersonaCondicionIva").html("<strong>Condición IVA:</strong> " + Common.obtenerCondicionIvaDesc(data.d.Personas.CondicionIva));
                $("#ddlTipo").html("<option value=''></option>");
                Common.obtenerComprobantesVentaPorCondicion("ddlTipo", data.d.Personas.CondicionIva, data.d.Tipo);
                changelblPrecioUnitario(data.d.Personas.CondicionIva);
                Common.obtenerProvincias("ddlJuresdiccion", data.d.Personas.IDJuresdiccion, true);
                Common.obtenerDomicilios("ddlDomicilio", data.d.IDDomicilio, false, $("#ddlPersona").val());
                Common.obtenerActividades("ddlActividad", data.d.IDActividad, false);
                if (data.d.IDTransporte != 0) {
                    Common.obtenerTransportes("ddlTransporte", data.d.IDTransporte, false);
                } else {
                    Common.obtenerTransportes("ddlTransporte", data.d.IDTransporte, true);
                }
                if (data.d.IDTransportePersona != 0) {
                    Common.obtenerTransportePersona("ddlTransportePersona", data.d.IDTransportePersona, false, $("#ddlPersona").val());
                } else {
                    Common.obtenerTransportePersona("ddlTransportePersona", data.d.IDTransportePersona, true, $("#ddlPersona").val());
                }
                if (data.d.IDUsuarioAdicional != 0) {
                    Common.obtenerVendedores("ddlVendedorComision", data.d.IDUsuarioAdicional, false);
                } else {
                    Common.obtenerVendedores("ddlVendedorComision", data.d.IDUsuarioAdicional, true);
                }
                
                
                if (data.d.Tipo != "") {                   
                    changeTipoComprobante();
                }                

                setTimeout(function () {
                    if ($("#ddlTipo").val() == "NCA" || $("#ddlTipo").val() == "NCB" || $("#ddlTipo").val() == "NCC"
                        || $("#ddlTipo").val() == "NDA" || $("#ddlTipo").val() == "NDB" || $("#ddlTipo").val() == "NDC"
                        || $("#ddlTipo").val() == "NCAMP" || $("#ddlTipo").val() == "NCBMP" || $("#ddlTipo").val() == "NCCMP"
                        || $("#ddlTipo").val() == "NDAMP" || $("#ddlTipo").val() == "NDBMP" || $("#ddlTipo").val() == "NDCMP") {
                        $("#ddlComprobanteAsociado").html("");
                        $("<option/>").attr("value", data.d.IDComprobanteAsociado).text(data.d.NumeroComprobanteAsociado).appendTo($("#ddlComprobanteAsociado"));
                    }
                }, 1000);

                obtenerItems();
                obtenerTotales();

                $("#divFactura,#lnkAceptar").show();

                //$("#divFactura,#lnkAceptar,#divFactura,#lnkPrevisualizar,#lnkPrevisualizarTicket").show();

                setTimeout(function () {
                    $("#ddlPersona").attr("onchange", "changePersona()");
                }, 1000);

                setTimeout(function () {
                    if ($("#ddlTipo").val() == "EDA") {
                        document.getElementById("ddlPersona").setAttribute("disabled", "disabled");
                        $("#ddlComprobantePersona").html("");
                        $("<option/>").attr("value", data.d.IDComprobanteVinculado).text(data.d.NumeroComprobanteVinculado).appendTo($("#ddlComprobantePersona"));
                    }
                }, 1000);


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



function verConcepto(id) {
    window.location.href = "/conceptose.aspx?ID=" + id;
}

function cobrarComprobante() {
    window.location.href = "/cobranzase.aspx?IDComprobante=" + $("#hdnID").val();
}
/*** FORM ALTA ***/

function changeModo() {
    if ($("#ddlModo").val() == "E" && $("#ddlTipo").val() != "COT" && $("#ddlTipo").val() != "PDV" && $("#ddlTipo").val() != "PDC" && $("#ddlTipo").val() != "DDC") {
        $("#divNroComprobante").hide();
        $("#lnkGenerarCAE").show();
        $("#lnkAceptar").text("Guardar borrador");
    }
    else {
        if ($("#ddlTipo").val() != "PDV" && $("#ddlTipo").val() != "PDC" && $("#ddlTipo").val() != "DDC") {
            $("#divNroComprobante").show();
        }        
        $("#lnkGenerarCAE").hide();
        $("#lnkAceptar").text("Guardar");
        if ($("#txtNumero").val() == "0")
            Common.obtenerProxNroComprobante("txtNumero", $("#ddlTipo").val(), $("#ddlPuntoVenta").val());
    }

    if ($("#ddlTipo").val() == "COT" || $("#ddlTipo").val() == "PDV" || $("#ddlTipo").val() == "PDC" || $("#ddlTipo").val() == "DDC") {
        $("#lnkAceptar").text("Guardar");
    }
}

function changeComboEntrega() {
    if ($("#chkEntregaEstado").is(":checked")) {
        $("#divEstado").show();
    } else {
        $("#divEstado").hide();
    }
}

function changePuntoDeVenta() {
    Common.obtenerProxNroComprobante("txtNumero", $("#ddlTipo").val(), $("#ddlPuntoVenta").val());
}
function changePersona() {
    limpiarNuevoComprobante();
    if ($("#ddlPersona").val() != "") {
        $("#lnkAceptar,#lnkGenerarCAE,#lnkPrevisualizar,#lnkPrevisualizarTicket,#divFactura").show();
        obtenerInfoPersona(parseInt($("#ddlPersona").val()), '');
    }
    else {
        $("#lnkAceptar,#lnkGenerarCAE,#lnkPrevisualizar,#lnkPrevisualizarTicket,#divFactura").hide();
    }
}

function changeConcepto() {
    if (esModificacion == false) {
        if ($("#ddlProductos").val() != "" && $("#ddlProductos").val() != null) {
            $("#txtConcepto").attr("disabled", true);

            $.ajax({
                type: "POST",
                url: "conceptose.aspx/obtenerDatos",
                data: "{id: " + parseInt($("#ddlProductos").val()) + ",idPersona: " + parseInt($("#ddlPersona").val()) + "}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data, text) {
                    if (data.d != null) {
                        $("#txtConcepto").val(data.d.Nombre);
                        $("#txtPrecio").val(data.d.Precio.replace(".", ","));
                        //$("#ddlIva").val(data.d.Iva.replace(".", ","));
                        $("#ddlIva").val(data.d.TipoIva);
                        $("#hdnCodigo").val(data.d.Codigo);
                        $("#ddlIva").trigger("change");
                    }
                },
                error: function (response) {
                    var r = jQuery.parseJSON(response.responseText);
                    $("#msgErrorDetalle").html(r.Message);
                    $("#divErrorDetalle").show();
                }
            });
        }
        else {
            $("#txtConcepto").attr("disabled", false);
        }
    }
}

function limpiarNuevoComprobante() {
    ocultarMensajes();
    $("#ddlProducto").val("3");
    //$("#ddlPuntoVenta").html("<option value=''></option>");
    $("#txtNumero").val("");
    $("#ddlTipo").html("<option value=''></option>");
    $("#txtCantidad, #txtConcepto, #txtPrecio, #txtBonificacion").val("");

    //if ($("#hdnIDUsuario").val() != "5188" && $("#hdnIDUsuario").val() != "5190"
    //    && $("#hdnIDUsuario").val() != "5248" && $("#hdnIDUsuario").val() != "3155" && $("#hdnIDUsuario").val() != "5339") { //FIOL
    //    $("#txtCantidad").numericInput();
    //    $("#txtCantidad").val("1");
    //} else {
    //    $("#txtCantidad").maskMoney({ thousands: '', decimal: ',', allowZero: true });
    //    $("#txtCantidad").val("1,00");
    //}
    if ($("#hdnUsaCantidadConDecimales").val() == "1") {
        $("#txtCantidad").maskMoney({ thousands: '', decimal: ',', allowZero: true });
        $("#txtCantidad").val("1,00");
    } else {
        $("#txtCantidad").numericInput();
        $("#txtCantidad").val("1");
    }

    //$("#ddlIva").val("0,00");

    if (MI_CONDICION == "MO") {
        $("#ddlIva").val("3");
    }
    else if (MI_CONDICION == "RI") {
        $("#ddlIva").val("5");
    }

    $("#hdnIDItem").val("0");
    $("#bodyDetalle").html("<tr><td colspan='9' style='text-align:center'>No tienes items agregados</td></tr>");
    $("#divImporteNoGravado, #divImporteGravado, #divImporteExento, #divIVA, #divTotal, #divTotalCompras").html("0");
    $("#txtObservaciones").val("");
}

function modificarTipoComprobante() {
    $("#lnkGenerarCAE,#divModo,#divPuntoVenta,#divBonif,#divIva,#divFechas,#divNroComprobante,#divDescripcion").show();
    $("#txtNumero").removeAttr("readonly");
    if ($("#ddlTipo").val() == tipoActual)
        $("#txtNumero").val(nroActual)
    else
        $("#txtNumero").val("")
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
                $("#ddlTipo").html("<option value=''></option>");

                var tipo = getParameterByName('tipo');
                Common.obtenerComprobantesVentaPorCondicion("ddlTipo", data.d.CondicionIva, tipo);
                $("#divNombre,#divVendedor,#divEnvio,#divFechaEntrega").hide();

                $("#hdnRazonSocial").val(data.d.RazonSocial);
                $("#txtEnvioPara").val(data.d.Email);
                $("#hdnPorcentajeDescuentoCliente").val(data.d.PorcentajeDescuento);                
                $("#txtBonificacion").val(data.d.PorcentajeDescuento);
                changelblPrecioUnitario(data.d.CondicionIva);

                if (tipo != "") {
                    changeTipoComprobante();

                    //if ($("#ddlTipo").val() == "NCA" || $("#ddlTipo").val() == "NCB" || $("#ddlTipo").val() == "NCC"
                    //    || $("#ddlTipo").val() == "NDA" || $("#ddlTipo").val() == "NDB" || $("#ddlTipo").val() == "NDC") {
                        
                    //}      
                    //if ($("#ddlTipo").val() == "EDA") {
                        
                    //}                      
                    Common.obtenerTransportePersona("ddlTransportePersona", "", true, $("#ddlPersona").val());
                    if (tipo == "PDV") {                        
                        Common.obtenerDomicilios("ddlDomicilio", false, true, $("#ddlPersona").val());
                    } else {
                        Common.obtenerDomicilios("ddlDomicilio", false, false, $("#ddlPersona").val());
                    }                    
                } else {
                    ObtenerUltimoTipoFacturacionCliente();
                    if ($("#ddlModo").val() == "E") {
                        $("#txtNumero").val("0");
                        $("#divNroComprobante").hide();
                        $("#lnkAceptar").text("Guardar borrador");
                    }
                }

                if ($("#hdnParaPDVSolicitarCompletarContacto").val() == "1") {
                    if (data.d.ProvinciaDesc != "" && data.d.CiudadDesc != ""
                        && data.d.Email != "" && data.d.Telefono != "") {
                        $("#divClienteDatosPendientes").hide();
                    }
                    else {
                        $("#hdnClienteTipo").val(data.d.Tipo);
                        $("#divClienteDatosPendientes").show();
                        $("#divClienteDatosPendientesTexto").html("Cliente con datos de contacto pendientes de completar!! Clic <a onclick='editarPersona(" + data.d.ID + ");' style='cursor: pointer;'><span>AQUI</span></a> para actualizar.");
                    }
                }

                if (data.d.Avisos != "") {                    
                    $("#divClienteAvisosTexto").html("<i class='fa fa-exclamation-triangle' aria-hidden='true'></i> " + data.d.Avisos);
                    $("#divClienteAvisos").show();
                } else {
                    $("#divClienteAvisos").hide();
                }

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

function editarPersona(id) {
    window.location.href = "/personase.aspx?ID=" + id + "&tipo=" + $("#hdnClienteTipo").val();
}

function obtenerFacturasDelCliente() {
    var info = "{idPersona: " + $("#ddlPersona").val() + "}";
    $.ajax({
        type: "POST",
        url: "/common.aspx/ObtenerFacturasDelCliente",
        data: info,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            if (data != null) {
                $("#ddlComprobanteAsociado").html("");
                $("<option/>").attr("value", "").text("").appendTo($("#ddlComprobanteAsociado"));

                for (var i = 0; i < data.d.length; i++) {
                    $("<option/>").attr("value", data.d[i].ID).text(data.d[i].Nombre).appendTo($("#ddlComprobanteAsociado"));
                }
            }
        }
    });
}

function changeComprobanteAsociado() {

    if ($("#ddlComprobanteAsociado").val() != "") {
        var info = "{idComprobante: " + $("#ddlComprobanteAsociado").val() + "}";
        $.ajax({
            type: "POST",
            url: "/comprobantese.aspx/CargarDatosDelComprobanteAsociado",
            data: info,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                if (data != null) {
                    ocultarMensajes();
                    $("#ddlProducto").val("3");
                    $("#txtCantidad, #txtConcepto, #txtPrecio, #txtBonificacion").val("");
                    //if ($("#hdnIDUsuario").val() != "5188" && $("#hdnIDUsuario").val() != "5190"
                    //    && $("#hdnIDUsuario").val() != "5248" && $("#hdnIDUsuario").val() != "3155" && $("#hdnIDUsuario").val() != "5339") { //FIOL
                    //    $("#txtCantidad").numericInput();
                    //    $("#txtCantidad").val("1");
                    //} else {
                    //    $("#txtCantidad").maskMoney({ thousands: '', decimal: ',', allowZero: true });
                    //    $("#txtCantidad").val("1,00");
                    //}
                    if ($("#hdnUsaCantidadConDecimales").val() == "1") {
                        $("#txtCantidad").maskMoney({ thousands: '', decimal: ',', allowZero: true });
                        $("#txtCantidad").val("1,00");
                    } else {
                        $("#txtCantidad").numericInput();
                        $("#txtCantidad").val("1");
                    }
                    if (MI_CONDICION == "MO") {
                        $("#ddlIva").val("3");
                    }
                    else if (MI_CONDICION == "RI") {
                        $("#ddlIva").val("5");
                    }
                    $("#hdnIDItem").val("0");
                    $("#bodyDetalle").html("<tr><td colspan='9' style='text-align:center'>No tienes items agregados</td></tr>");
                    $("#divImporteNoGravado, #divImporteGravado, #divImporteExento, #divIVA, #divTotal, #divTotalCompras").html("0");
                    $("#txtObservaciones").val("");
                    obtenerPuntoDeVenta();
                    obtenerItems();
                    obtenerTotales();
                }
            }
        });
    } 
    
}

function obtenerPuntoDeVenta() {
    $.ajax({
        type: "POST",
        url: "/comprobantese.aspx/obtenerPuntoDeVenta",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            if (data != null) {
                $("#ddlPuntoVenta").val(data.d);
            }
        }
    });
}

function obtenerComprobantesDelCliente() {
    var info = "{idPersona: " + $("#ddlPersona").val() + "}";
    $.ajax({
        type: "POST",
        url: "/common.aspx/ObtenerComprobantesDelCliente",
        data: info,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            if (data != null) {
                $("#ddlComprobantePersona").html("");
                $("<option/>").attr("value", "").text("").appendTo($("#ddlComprobantePersona"));

                for (var i = 0; i < data.d.length; i++) {
                    $("<option/>").attr("value", data.d[i].ID).text(data.d[i].Nombre).appendTo($("#ddlComprobantePersona"));
                }

                if ($("#hdnIdPedidoDeVenta").val() != "0") {
                    $("#ddlComprobantePersona").val($("#hdnIdPedidoDeVenta").val());
                    changeComprobantePersona();
                    $("#hdnIdPedidoDeVenta").val("0");
                }

            }
        }
    });
}

function changeComprobantePersona() {
    $("#hdnNotaDeCreditoPorServicio").val("0");
    if ($("#ddlComprobantePersona").val() != "") {
        var info = "{idComprobante: " + $("#ddlComprobantePersona").val() + "}";
        $.ajax({
            type: "POST",
            url: "/comprobantese.aspx/CargarDatosDelComprobanteAsociado",
            data: info,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                if (data != null) {
                    ocultarMensajes();
                    $("#ddlProducto").val("3");
                    $("#txtCantidad, #txtConcepto, #txtPrecio, #txtBonificacion").val("");
                    //if ($("#hdnIDUsuario").val() != "5188" && $("#hdnIDUsuario").val() != "5190"
                    //    && $("#hdnIDUsuario").val() != "5248" && $("#hdnIDUsuario").val() != "3155" && $("#hdnIDUsuario").val() != "5339") { //FIOL
                    //    $("#txtCantidad").numericInput();
                    //    $("#txtCantidad").val("1");
                    //} else {
                    //    $("#txtCantidad").maskMoney({ thousands: '', decimal: ',', allowZero: true });
                    //    $("#txtCantidad").val("1,00");
                    //}
                    if ($("#hdnUsaCantidadConDecimales").val() == "1") {
                        $("#txtCantidad").maskMoney({ thousands: '', decimal: ',', allowZero: true });
                        $("#txtCantidad").val("1,00");
                    } else {
                        $("#txtCantidad").numericInput();
                        $("#txtCantidad").val("1");
                    }
                    if (MI_CONDICION == "MO") {
                        $("#ddlIva").val("3");
                    }
                    else if (MI_CONDICION == "RI") {
                        $("#ddlIva").val("5");
                    }
                    $("#hdnIDItem").val("0");
                    $("#bodyDetalle").html("<tr><td colspan='9' style='text-align:center'>No tienes items agregados</td></tr>");
                    $("#divImporteNoGravado, #divImporteGravado, #divImporteExento, #divIVA, #divTotal, #divTotalCompras").html("0");
                    $("#txtObservaciones").val("");
                    obtenerPuntoDeVenta();
                    obtenerItems();
                    obtenerTotales();
                }
            }
        });
    }

}


function changelblPrecioUnitario(condicionIVA) {
    //if (condicionIVA == "MO" || condicionIVA == "CF") {
    //    $("#spPrecioUnitario").text("Precio Unit. con IVA");
    //}
    //else {
        if ($("#hdnUsaPrecioConIVA").val() == "1") {
            $("#spPrecioUnitario").text("Precio Unit. con IVA");
        } else {
            $("#spPrecioUnitario").text("Precio Unit. sin IVA");
        }
    //}
}

function ObtenerUltimoTipoFacturacionCliente() {
    var info = "{idPersona: " + $("#ddlPersona").val() + "}";
    $.ajax({
        type: "POST",
        url: "/common.aspx/ObtenerUltimoTipoFacturacionCliente",
        data: info,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            if (data.d != null && data.d != "") {
                var tipoComprobante = getParameterByName('tipo');    
                if (tipoComprobante != "") {
                    $("#ddlTipo").val(tipoComprobante);
                } else {
                    $("#ddlTipo").val(data.d);
                }
                changeTipoComprobante();
            }
        }
    });
}

function EditarPorcentajeDescuento() {
    var idPersona = ($("#ddlPersona").val() == "" || $("#ddlPersona").val() == null) ? 0 : parseInt($("#ddlPersona").val());

    bootbox.prompt({
        title: "Actualizar Descuento",
        inputType: 'number',
        value: $("#lblPorcentajeDescuento").text(),
        buttons: {
            confirm: {
                label: 'Aceptar'
            },
            cancel: {
                label: 'Cancelar'
            }
        },
        callback: function (result) {

            var info = "{ idPersona: " + idPersona + ", porcentajeDescuento: '" + result + "'}";

            $.ajax({
                type: "POST",
                url: "personase.aspx/editarDescuento",
                data: info,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data, text) {
                    $("#lblPorcentajeDescuento").text(result);
                    obtenerTotales();
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
    });
}

function changeTipoComprobante() {
    modificarTipoComprobante();
    if ($("#ddlTipo").val() == "COT") {
        $("#lnkGenerarCAE,#divModo,#trTotalCompras,#trDescuento,#divActividad,#divModalidadPagoAFIP").hide();
    }
    else {
        if ($("#ddlTipo").val() == "PDV") {
            $("#lnkGenerarCAE,#divModo,#divCondicionVenta,#divFechas,#divEstado,#divModalidadPagoAFIP").hide();
            $("#divNroComprobante,#liJuntarComprobantes,#liDesvincularComprobantes").hide();
            $("#liVincularCompra,#liImprimirPreLiquidoProducto,#liImprimirLiquidoProducto,#liGenerarNotaDeCreditoPorServicio").hide();
            $("#divComprobanteAsociado,#divComprobantePersona,#divDomicilio,#divTransporte").hide();
            $("#divTransportePersona,#trTotalCompras,#trDescuento").hide();
            $("#liVincularComprobanteEdaFac").hide();

            $("#divNombre,#divVendedor,#divEnvio,#divFechaEntrega,#divPuntoVenta,#divDomicilio,#divTransporte,#divTransportePersona,#divVendedorComision").show();
            $("#divFechaAlta,#divMasOpciones").show();
            $("#ddlModo").val("T");
            if ($("#hdnFacturaSoloContraEntrega").val() == "1") {
                $("#liVincularComprobante,#liVincularComprobanteDividido,#divVincularComprobante").hide();
            } else {
                $("#liVincularComprobante,#liVincularComprobanteDividido,#divVincularComprobante").show();
            }

            if ($("#hdnID").val() == "0") {
                $("#liActualizarPrecios").hide();
            } else {
                $("#liActualizarPrecios").show();
            }
            if ($("#hdnMasDeUnaActividad").val() == "0") {
                $("#divActividad").hide();
            } else {
                $("#divActividad").show();
            }
        } else {
            $("#divNombre,#divVendedor,#divEnvio,#divFechaEntrega,#divFechaAlta,#liGenerarNotaDeCreditoPorServicio,#divModalidadPagoAFIP").hide();
            if ($("#ddlTipo").val() == "PDC") {
                $("#lnkGenerarCAE,#divModo,#divCondicionVenta,#divPuntoVenta,#divFechas,#divNroComprobante,#divModalidadPagoAFIP").hide();
                $("#liJuntarComprobantes,#liDesvincularComprobantes,#liVincularComprobante,#liVincularComprobanteDividido,#liGenerarNotaDeCreditoPorServicio").hide();
                $("#divComprobanteAsociado,#divComprobante,#divComprobantePersona,#divDomicilio,#divTransporte,#divTransportePersona,#trTotalCompras,#trDescuento").hide();
                $("#liVincularComprobanteEdaFac,#divActividad").hide();

                $("#divMasOpciones,#divFechaEntrega,#divFechaAlta").show();
                $("#ddlModo").val("T");
                if ($("#hdnID").val() == "0") {
                    $("#liVincularCompra,#liActualizarPrecios,#liImprimirPreLiquidoProducto,#liImprimirLiquidoProducto").hide();
                } else {
                    $("#liVincularCompra,#liActualizarPrecios,#liImprimirPreLiquidoProducto,#liImprimirLiquidoProducto").show();
                }
            } else {
                if ($("#ddlTipo").val() == "EDA") {
                    $("#lnkGenerarCAE,#divModo,#divCondicionVenta,#divPuntoVenta,#divFechas,#divNroComprobante,#divModalidadPagoAFIP").hide();
                    $("#liActualizarPrecios,#liJuntarComprobantes,#liDesvincularComprobantes,#liGenerarNotaDeCreditoPorServicio").hide();
                    $("#liVincularComprobante,#liVincularComprobanteDividido,#liVincularCompra,#liImprimirPreLiquidoProducto").hide();
                    $("#liImprimirLiquidoProducto").hide();
                    $("#divComprobanteAsociado,#divComprobante,#divAgregarArticulo,#divTotales").hide();
                    $("#lnkPrevisualizar,#lnkPrevisualizarTicket,#trTotalCompras,#trDescuento").hide();
                    $("#liVincularComprobanteEdaFac,#divActividad").hide();

                    $("#txtNumero").prop('readonly', true);

                    $("#divMasOpciones,#divFechaEntrega,#divEstado,#divDomicilio,#divTransporte,#divTransportePersona,#liVincularComprobanteEdaFac").show();
                    $("#ddlModo").val("T");
                    if ($("#hdnMasDeUnaActividad").val() == "0") {
                        $("#divActividad").hide();
                    } else {
                        $("#divActividad").show();
                    }
                    obtenerComprobantesDelCliente();
                } else {
                    if ($("#ddlTipo").val() == "DDC") {
                        $("#lnkGenerarCAE,#divModo,#divCondicionVenta,#divPuntoVenta,#divFechas,#divNroComprobante,#divComprobantePersona").hide();
                        $("#liActualizarPrecios,#liJuntarComprobantes,#liDesvincularComprobantes,#liGenerarNotaDeCreditoPorServicio").hide();
                        $("#liVincularComprobante,#liVincularComprobanteDividido,#liVincularCompra,#liImprimirPreLiquidoProducto").hide();
                        $("#liImprimirLiquidoProducto,#divDomicilio, #divTransporte,#divTransportePersona,#liVincularComprobanteEdaFac").hide();
                        $("#divComprobanteAsociado,#divComprobante,#divAgregarArticulo,#divTotales,#divFechaEntrega,#divEstado").hide();
                        $("#lnkPrevisualizar,#lnkPrevisualizarTicket,#liImprimirRemitoSinLogo,#liImprimirRemitoTalonario").hide();
                        $("#divModalNuevoCliente,#trTotalCompras,#trDescuento,#divActividad,#divModalidadPagoAFIP").hide();
                        document.getElementById('ddlPersona').disabled = true;

                        $("#divMasOpciones,#divAgregarArticulo,#divTotales,#trTotalCompras,#trDescuento").show();
                        $("#ddlModo").val("T");
                    } else {
                        $("#liJuntarComprobante,#liVincularComprobante,#liVincularComprobanteDividido,#liVincularCompra,#liImprimirPreLiquidoProducto").hide();
                        $("#liImprimirLiquidoProducto,#liActualizarPrecios,#divComprobanteAsociado,#divEstado").hide();
                        $("#divComprobantePersona,#divTransporte,#divTransportePersona,#trTotalCompras,#trDescuento,#divModalidadPagoAFIP").hide();

                        $("#divMasOpciones,#liDesvincularComprobantes,#divModo").show();

                        if ($("#ddlTipo").val() == "NCA" || $("#ddlTipo").val() == "NCB" || $("#ddlTipo").val() == "NCC"
                            || $("#ddlTipo").val() == "NDA" || $("#ddlTipo").val() == "NDB" || $("#ddlTipo").val() == "NDC"
                            || $("#ddlTipo").val() == "NCAMP" || $("#ddlTipo").val() == "NCBMP" || $("#ddlTipo").val() == "NCCMP"
                            || $("#ddlTipo").val() == "NDAMP" || $("#ddlTipo").val() == "NDBMP" || $("#ddlTipo").val() == "NDCMP")
                        {
                            $("#divComprobanteAsociado,#liGenerarNotaDeCreditoPorServicio").show();
                            obtenerFacturasDelCliente();
                        }

                        if ($("#ddlTipo").val() == "FCAMP" || $("#ddlTipo").val() == "FCBMP" || $("#ddlTipo").val() == "FCCMP")
                        {
                            $("#divModalidadPagoAFIP").show();
                        }

                        if ($("#hdnTieneFE").val() == "1") {
                            if ($("#hdnID").val() == "0") {
                                $("#ddlModo").val("E");
                                $("#lnkAceptar").text("Guardar borrador");
                            }
                            //changeModo();
                        }
                        else {
                            $("#ddlModo").val("T");
                            $("#lnkAceptar").text("Aceptar");
                        }
                        if ($("#hdnMasDeUnaActividad").val() == "0") {
                            $("#divActividad").hide();
                        } else {
                            $("#divActividad").show();
                        }
                    }
                }
            }            
        }
    }
    changeModo();
    if ($("#txtNumero").val() == "")
        Common.obtenerProxNroComprobante("txtNumero", $("#ddlTipo").val(), $("#ddlPuntoVenta").val());

    //if ($("#ddlTipo").val() == "PDV") {
    //    $("#lblTitulo").text("Pedidos de Ventas");
    //    $("#lblSubtitulo").text("Pedidos de Ventas");
    //    $("#lblCondicion").text("Condición de Venta");
    //    $("#lblDatosGenerales").text("Acá van los datos más generales del pedido.");
    //    $("#lblTextoFactura").text("Agrega a continuación los productos o servicios que vas a incluir en el pedido.");
    //    $("#htextoFactura").html("2. Productos o servicios");
    //} else {
    //    if ($("#ddlTipo").val() == "PDC") {
    //        $("#lblTitulo").text("Pedidos de Compras");
    //        $("#lblSubtitulo").text("Pedidos de Compras");
    //        $("#lblCondicion").text("Condición de Compra");
    //        $("#lblDatosGenerales").text("Acá van los datos más generales del pedido.");
    //        $("#lblTextoFactura").text("Agrega a continuación los productos o servicios que vas a incluir en el pedido.");
    //        $("#htextoFactura").html("2. Productos o servicios");
    //    } else {
    //        $("#lblTitulo").text("Facturación");
    //        $("#lblSubtitulo").text("Facturación");
    //        $("#lblCondicion").text("Condición de Venta");
    //        $("#lblDatosGenerales").text("Acá van los datos más generales de la factura.");
    //        $("#lblTextoFactura").text("Agrega a continuación los productos o servicios que vas a incluir en la factura.");
    //        if ($("#hdnPercepcionIVA").val() == "1" || $("#hdnPercepcionIIBB").val() == "1") {
    //            $("#htextoFactura").html("4. Productos o servicios");
    //        } else {
    //            $("#htextoFactura").html("3. Productos o servicios");
    //        }
    //    }
    //}
    
}

/*** ITEMS ***/
function cancelarItem() {
    $("#txtConcepto, #txtCantidad, #txtPrecio, #txtBonificacion").val("");
    //if ($("#hdnIDUsuario").val() != "5188" && $("#hdnIDUsuario").val() != "5190"
    //    && $("#hdnIDUsuario").val() != "5248" && $("#hdnIDUsuario").val() != "3155" && $("#hdnIDUsuario").val() != "5339") { //FIOL
    //    $("#txtCantidad").numericInput();
    //    $("#txtCantidad").val("1");
    //} else {
    //    $("#txtCantidad").maskMoney({ thousands: '', decimal: ',', allowZero: true });
    //    $("#txtCantidad").val("1,00");
    //}
    if ($("#hdnUsaCantidadConDecimales").val() == "1") {
        $("#txtCantidad").maskMoney({ thousands: '', decimal: ',', allowZero: true });
        $("#txtCantidad").val("1,00");
    } else {
        $("#txtCantidad").numericInput();
        $("#txtCantidad").val("1");
    }
    $("#hdnIDItem").val("0");
    $("#ddlProductos").val("").trigger("change");
    $("#btnAgregarItem").html("Agregar");
}

function descuento() {
    var descuento = $("#divDescuento").val();
    bootbox.prompt({
        title: "Descuento (En porcentaje)",
        value: descuento,
        callback: function (result) {
            if (result != null) {
                $("#divDescuento").val(result);
                verificarPlanDeCuentas();
                obtenerItems();
                obtenerTotales();
                //var info = "{ descuento:' " + descuento + "'}";
                //$.ajax({
                //    type: "POST",
                //    url: "comprobantese.aspx/aplicarDescuento",
                //    data: info,
                //    contentType: "application/json; charset=utf-8",
                //    dataType: "json",
                //    success: function (data, text) {
                //    },
                //    error: function (response) {
                //        var r = jQuery.parseJSON(response.responseText);
                //        $("#msgErrorDetalle").html(r.Message);
                //        $("#divErrorDetalle").show();
                //    }
                //});
            }
        }
    });
}

function agregarItem() {
    ocultarMensajes();
    esModificacion = false;
    if ($("#hdnUsaPlanCorporativo").val() == "1") {
        if ($("#ddlPlanDeCuentas").val() == "") {
            $("#msgErrorDetalle").html("Debes ingresar la cuenta contable.");
            $("#divErrorDetalle").show();
            return false;
        }
    }

    if ($("#txtCantidad").val() != "" && $("#txtConcepto").val() != "" && $("#txtPrecio").val() != "") {

        if ($("#hdnPedidoDeVenta").val() == "1") {
            if ($("#ddlProductos").val() == "") {
                $("#msgErrorDetalle").html("Debe seleccionar un producto.");
                $("#divErrorDetalle").show();
                return false;
            }
        }

        if (parseFloat($("#txtPrecio").val()) == 0) {
            $("#msgErrorDetalle").html("El precio debe ser mayor a 0.");
            $("#divErrorDetalle").show();
        }
        else {           

            var idPersona = ($("#ddlPersona").val() == "" || $("#ddlPersona").val() == null) ? 0 : parseInt($("#ddlPersona").val());
            var idPlanDeCuenta = ($("#ddlPlanDeCuentas").val() == "" || $("#ddlPlanDeCuentas").val() == null) ? 0 : parseInt($("#ddlPlanDeCuentas").val());
            var codigoCta = (idPlanDeCuenta == 0) ? "" : $("#ddlPlanDeCuentas option:selected").text();

            var subtotal = 0;
            if ($("#chkAjusteSubtotal").is(":checked")) {
                var info = "{ id: " + parseInt($("#hdnIDItem").val())
                    + ", idConcepto: '" + $("#ddlProductos").val()
                    + "', concepto: '" + $("#txtConcepto").val()
                    + "', iva: '" + $("#ddlIva").val()
                    + "', precio: '" + $("#txtPrecio").val()
                    + "', bonif: '" + $("#txtBonificacion").val()
                    + "', cantidad: '" + $("#txtCantidad").val()
                    + "', idPersona: " + idPersona
                    + " , codigo:'" + $("#hdnCodigo").val()
                    + "', idPlanDeCuenta:' " + idPlanDeCuenta
                    + "', codigoCta:' " + codigoCta
                    + "'}";
                $.ajax({
                    type: "POST",
                    url: "comprobantese.aspx/obtenerSubtotal",
                    data: info,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data, text) {
                        bootbox.prompt({
                            title: "Subtotal",
                            value: data.d.replace(".", "").replace(",", "."),
                            callback: function (result) {
                                if (result != null) {
                                    subtotal = result;
                                    var info = "{ id: " + parseInt($("#hdnIDItem").val())
                                        + ", idConcepto: '" + $("#ddlProductos").val()
                                        + "', concepto: '" + $("#txtConcepto").val()
                                        + "', iva: '" + $("#ddlIva").val()
                                        + "', precio: '" + $("#txtPrecio").val()
                                        + "', bonif: '" + $("#txtBonificacion").val()
                                        + "', cantidad: '" + $("#txtCantidad").val()
                                        + "', idPersona: " + idPersona
                                        + " , codigo:'" + $("#hdnCodigo").val()
                                        + "', idPlanDeCuenta:' " + idPlanDeCuenta
                                        + "', codigoCta:' " + codigoCta
                                        + "', ajuste: " + $("#chkAjusteSubtotal").is(":checked")
                                        + ", ajusteSubtotal:' " + subtotal
                                        + "'}";
                                    $.ajax({
                                        type: "POST",
                                        url: "comprobantese.aspx/agregarItem",
                                        data: info,
                                        contentType: "application/json; charset=utf-8",
                                        dataType: "json",
                                        success: function (data, text) {
                                            $("#txtConcepto, #txtCantidad, #txtPrecio, #txtBonificacion").val("");
                                            $("#txtConcepto").attr("disabled", false);
                                            $("#ddlProductos").val("").trigger("change");
                                            $("#txtBonificacion").val($("#hdnPorcentajeDescuentoCliente").val());
                                            $("#hdnIDItem").val("0");
                                            $("#btnAgregarItem").html("Agregar");
                                            $("#txtCantidad").focus();
                                            //if ($("#hdnIDUsuario").val() != "5188" && $("#hdnIDUsuario").val() != "5190"
                                            //    && $("#hdnIDUsuario").val() != "5248" && $("#hdnIDUsuario").val() != "3155" && $("#hdnIDUsuario").val() != "5339") { //FIOL
                                            //    $("#txtCantidad").numericInput();
                                            //    $("#txtCantidad").val("1");
                                            //} else {
                                            //    $("#txtCantidad").maskMoney({ thousands: '', decimal: ',', allowZero: true });
                                            //    $("#txtCantidad").val("1,00");
                                            //}
                                            if ($("#hdnUsaCantidadConDecimales").val() == "1") {
                                                $("#txtCantidad").maskMoney({ thousands: '', decimal: ',', allowZero: true });
                                                $("#txtCantidad").val("1,00");
                                            } else {
                                                $("#txtCantidad").numericInput();
                                                $("#txtCantidad").val("1");
                                            }

                                            if ($("#hdnUsaPlanCorporativo").val() == "1") {
                                                $("#ddlPlanDeCuentas").val("").trigger("change");
                                            }

                                            verificarPlanDeCuentas();
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
                        });
                    },
                    error: function (response) {
                        var r = jQuery.parseJSON(response.responseText);
                        $("#msgErrorDetalle").html(r.Message);
                        $("#divErrorDetalle").show();
                    }
                });
            }
            else {
                var info = "{ id: " + parseInt($("#hdnIDItem").val())
                    + ", idConcepto: '" + $("#ddlProductos").val()
                    + "', concepto: '" + $("#txtConcepto").val()
                    + "', iva: '" + $("#ddlIva").val()
                    + "', precio: '" + $("#txtPrecio").val()
                    + "', bonif: '" + $("#txtBonificacion").val()
                    + "', cantidad: '" + $("#txtCantidad").val()
                    + "', idPersona: " + idPersona
                    + " , codigo:'" + $("#hdnCodigo").val()
                    + "', idPlanDeCuenta:' " + idPlanDeCuenta
                    + "', codigoCta:' " + codigoCta
                    + "', ajuste: " + $("#chkAjusteSubtotal").is(":checked")
                    + ", ajusteSubtotal:' " + subtotal
                    + "'}";
                $.ajax({
                    type: "POST",
                    url: "comprobantese.aspx/agregarItem",
                    data: info,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data, text) {
                        $("#txtConcepto, #txtCantidad, #txtPrecio, #txtBonificacion").val("");
                        $("#txtConcepto").attr("disabled", false);
                        $("#ddlProductos").val("").trigger("change");
                        $("#txtBonificacion").val($("#hdnPorcentajeDescuentoCliente").val());
                        $("#hdnIDItem").val("0");
                        $("#btnAgregarItem").html("Agregar");
                        $("#txtCantidad").focus();
                        //if ($("#hdnIDUsuario").val() != "5188" && $("#hdnIDUsuario").val() != "5190"
                        //    && $("#hdnIDUsuario").val() != "5248" && $("#hdnIDUsuario").val() != "3155" && $("#hdnIDUsuario").val() != "5339") { //FIOL
                        //    $("#txtCantidad").numericInput();
                        //    $("#txtCantidad").val("1");
                        //} else {
                        //    $("#txtCantidad").maskMoney({ thousands: '', decimal: ',', allowZero: true });
                        //    $("#txtCantidad").val("1,00");
                        //}
                        if ($("#hdnUsaCantidadConDecimales").val() == "1") {
                            $("#txtCantidad").maskMoney({ thousands: '', decimal: ',', allowZero: true });
                            $("#txtCantidad").val("1,00");
                        } else {
                            $("#txtCantidad").numericInput();
                            $("#txtCantidad").val("1");
                        }

                        if ($("#hdnUsaPlanCorporativo").val() == "1") {
                            $("#ddlPlanDeCuentas").val("").trigger("change");
                        }

                        verificarPlanDeCuentas();
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
        url: "comprobantese.aspx/eliminarItem",
        data: info,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data, text) {
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

function modificarItem(id, codigo, idConcepto, cantidad, concepto, precio, iva, bonif, idPlanDeCuenta) {

    //if ($("#hdnIDUsuario").val() != "5188" && $("#hdnIDUsuario").val() != "5190"
    //    && $("#hdnIDUsuario").val() != "5248" && $("#hdnIDUsuario").val() != "3155" && $("#hdnIDUsuario").val() != "5339") { //FIOL
    //    $("#txtCantidad").numericInput();
    //    $("#txtCantidad").val(parseInt(cantidad));        
    //} else {
    //    $("#txtCantidad").maskMoney({ thousands: '', decimal: ',', allowZero: true });
    //    $("#txtCantidad").val(cantidad);
    //}
    if ($("#hdnUsaCantidadConDecimales").val() == "1") {
        $("#txtCantidad").maskMoney({ thousands: '', decimal: ',', allowZero: true });
        $("#txtCantidad").val("1,00");
    } else {
        $("#txtCantidad").numericInput();
        $("#txtCantidad").val("1");
    }

    $("#hdnCodigo").val(codigo);
    $("#txtConcepto").val(concepto);
    $("#txtPrecio").val(precio);
    $("#ddlIva").val(iva);
    $("#txtBonificacion").val(bonif);

    //$("#divIva").show();
    //$("#ddlIva").attr("disabled", false);
    //$("#divBonif").show();

    $("#ddlIva").trigger("change");

    if ($("#hdnUsaPlanCorporativo").val() == "1") {
        $("#ddlPlanDeCuentas").val(idPlanDeCuenta);
        $("#ddlPlanDeCuentas").trigger("change");
    }

    esModificacion = true;
    if (idConcepto != "") {
        $("#ddlProductos").val(idConcepto).trigger("change");
        $("#txtConcepto").attr("disabled", true);
    }

    $("#hdnIDItem").val(id);
    $("#btnAgregarItem").html("Actualizar");
}

function moverItem(id, concepto, accion) {

    $.ajax({
        type: "POST",
        url: "comprobantese.aspx/moverItem",
        data: "{id: " + id + ", concepto: '" + concepto + "', accion: '" + accion + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            $("#txtConcepto, #txtCantidad, #txtPrecio, #txtBonificacion").val("");
            $("#txtConcepto").attr("disabled", false);
            $("#ddlProductos").val("").trigger("change");
            $("#txtBonificacion").val($("#hdnPorcentajeDescuentoCliente").val());
            $("#hdnIDItem").val("0");
            $("#btnAgregarItem").html("Agregar");
            $("#txtCantidad").focus();
            //if ($("#hdnIDUsuario").val() != "5188" && $("#hdnIDUsuario").val() != "5190"
            //    && $("#hdnIDUsuario").val() != "5248" && $("#hdnIDUsuario").val() != "3155" && $("#hdnIDUsuario").val() != "5339") { //FIOL
            //    $("#txtCantidad").numericInput();
            //    $("#txtCantidad").val("1");
            //} else {
            //    $("#txtCantidad").maskMoney({ thousands: '', decimal: ',', allowZero: true });
            //    $("#txtCantidad").val("1,00");
            //}
            if ($("#hdnUsaCantidadConDecimales").val() == "1") {
                $("#txtCantidad").maskMoney({ thousands: '', decimal: ',', allowZero: true });
                $("#txtCantidad").val("1,00");
            } else {
                $("#txtCantidad").numericInput();
                $("#txtCantidad").val("1");
            }

            if ($("#hdnUsaPlanCorporativo").val() == "1") {
                $("#ddlPlanDeCuentas").val("").trigger("change");
            }

            verificarPlanDeCuentas();
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


function obtenerItems() {

    var idPersona = ($("#ddlPersona").val() == "" || $("#ddlPersona").val() == null) ? 0 : parseInt($("#ddlPersona").val());

    $.ajax({
        type: "POST",
        url: "comprobantese.aspx/obtenerItems",
        data: "{idPersona: " + idPersona + ", tipo: '" + $("#ddlTipo").val() + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            if (data != null) {
                $("#bodyDetalle").html(data.d);
            }
        }
    });
}

function obtenerTotales() {

    var PercepcionIIBB = $("#txtPercepcionIIBB").val() == "" ? "0" : $("#txtPercepcionIIBB").val();
    var PercepcionIVA = $("#txtPercepcionIVA").val() == "" ? "0" : $("#txtPercepcionIVA").val();
    var Descuento = $("#divDescuento").val() == "" ? "0" : $("#divDescuento").val();

    var info = "{ percepcionesIIBB: '" + PercepcionIIBB
        + "', percepcionesIVA: '" + PercepcionIVA
        + "', descuento: '" + Descuento
            + "'}";

    $.ajax({
        type: "POST",
        url: "comprobantese.aspx/obtenerTotales",
        data: info,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data, text) {
            if (data.d != null) {

                $("#divImporteNoGravado").html("$ " + data.d.ImporteNoGravado);
                $("#divExento").html("$ " + data.d.ImporteExento);

                $("#divImporteGravado").html("$ " + data.d.Subtotal);
                $("#divIVA").html("$ " + data.d.Iva);

                $("#trIVATotal").html("$ " + data.d.PercepcionIVA);
                $("#trIIBBTotal").html("$ " + data.d.PercepcionIIBB);

                $("#divDescuento").html("% " + data.d.Descuento);

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
    $("#divError, #divOk, #divErrorDetalle").hide();
}

function grabar(generarCAE) {
    ocultarMensajes();

    if ($("#ddlPuntoVenta").val() == null) {
        $("#msgError").html("Debe registrar al menos un punto de venta");
        $("#divError").show();
        $("#divOk").hide();
        $('html, body').animate({ scrollTop: 0 }, 'slow');
        return;
    }

    if ($("#ddlTipo").val() == "NCA" || $("#ddlTipo").val() == "NCB" || $("#ddlTipo").val() == "NCC"
        || $("#ddlTipo").val() == "NDA" || $("#ddlTipo").val() == "NDB" || $("#ddlTipo").val() == "NDC"
        || $("#ddlTipo").val() == "NCAMP" || $("#ddlTipo").val() == "NCBMP" || $("#ddlTipo").val() == "NCCMP"
        || $("#ddlTipo").val() == "NDAMP" || $("#ddlTipo").val() == "NDBMP" || $("#ddlTipo").val() == "NDCMP")
    {
        if ($("#ddlComprobanteAsociado").val() == null || $("#ddlComprobanteAsociado").val() == "") {            
            $("#msgError").html("Debe seleccionar un comprobante asociado para los tipo de comprobante Nota de credito/debito.");
            $("#divError").show();
            $("#divOk").hide();
            $('html, body').animate({ scrollTop: 0 }, 'slow');
            return;
        }
    }

    if ($("#ddlTipo").val() == "FCAMP" || $("#ddlTipo").val() == "FCBMP" || $("#ddlTipo").val() == "FCCMP") {
        if($("#ddlModalidadPagoAFIP").val() == null || $("#ddlModalidadPagoAFIP").val() == "") {
            $("#msgError").html("Debe seleccionar una modalidad de pago AFIP.");
            $("#divError").show();
            $("#divOk").hide();
            $('html, body').animate({ scrollTop: 0 }, 'slow');
            return;
        }
    }

    if ($("#ddlTipo").val() == "EDA") {

        if ($("#ddlComprobantePersona").val() == null || $("#ddlComprobantePersona").val() == "") {
            $("#msgError").html("Debe seleccionar un comprobante de origen");
            $("#divError").show();
            $("#divOk").hide();
            $('html, body').animate({ scrollTop: 0 }, 'slow');
            return;
        }

        var itemIdPersona = ($("#ddlPersona").val() == "" || $("#ddlPersona").val() == null) ? 0 : parseInt($("#ddlPersona").val());

        $('#tbItems tr').each(function () {

            if ($(this).find("td").eq(0).html() != null) {

                var info = "{ id: " + parseInt($(this).find("td").eq(0).html())
                    + ", idConcepto: '" + $(this).find("td").eq(3).find("input").prop('value')
                    + "', concepto: '" + $(this).find("td").eq(3).find("a").prop('title')
                    + "', iva: '" + $(this).find("td").eq(7).html().replace(".", "").replace(",",".")
                    + "', precio: '" + $(this).find("td").eq(4).html().replace(".", "").replace(",", ".")
                    + "', bonif: '" + $(this).find("td").eq(5).html().replace(".", "").replace(",", ".")
                    + "', cantidad: '" + $(this).find("td").find("input").prop('value').replace(".", "").replace(",", ".")
                    + "', idPersona: " + itemIdPersona
                    + " , codigo:'" + $(this).find("td").eq(2).html()
                    + "', idPlanDeCuenta:''"
                    + " , codigoCta: ''}";
                $.ajax({
                    type: "POST",
                    url: "comprobantese.aspx/agregarItemPorCodigoYConcepto",
                    data: info,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data, text) {
                        verificarPlanDeCuentas();
                        obtenerTotales();
                    },
                    error: function (response) {
                        var r = jQuery.parseJSON(response.responseText);
                        $("#msgErrorDetalle").html(r.Message);
                        $("#divErrorDetalle").show();
                    }
                });
            }            
            
        });
    }

    if ($('#frmEdicion').valid()) {
        if ($("#ddlTipo").val() != "COT" && $("#ddlModo").val() == "O") {
            $("#msgError").html("El modo del comprobante es inválido para un " + $("#ddlTipo option:selected").text());
            $("#divError").show();
            $("#divOk").hide();
            $('html, body').animate({ scrollTop: 0 }, 'slow');
        }
        else {
            Common.mostrarProcesando("lnkAceptar");
            Common.mostrarProcesando("lnkGenerarCAE");

            var idJurisdiccion = ($("#ddlJuresdiccion").val() != null && $("#ddlJuresdiccion").val() != "") ? parseInt($("#ddlJuresdiccion").val()) : 0;
            var idComprobanteAsociado = ($("#ddlComprobanteAsociado").val() != null && $("#ddlComprobanteAsociado").val() != "") ? parseInt($("#ddlComprobanteAsociado").val()) : 0;
            var idComprobanteOrigen = ($("#ddlComprobantePersona").val() != null && $("#ddlComprobantePersona").val() != "") ? parseInt($("#ddlComprobantePersona").val()) : 0;
            var idDomicilio = ($("#ddlDomicilio").val() != null && $("#ddlDomicilio").val() != "") ? parseInt($("#ddlDomicilio").val()) : 0;
            var idTransporte = ($("#ddlTransporte").val() != null && $("#ddlTransporte").val() != "") ? parseInt($("#ddlTransporte").val()) : 0;
            var idTransportePersona = ($("#ddlTransportePersona").val() != null && $("#ddlTransportePersona").val() != "") ? parseInt($("#ddlTransportePersona").val()) : 0;
            var idVendedorComision = ($("#ddlVendedorComision").val() != null && $("#ddlVendedorComision").val() != "") ? parseInt($("#ddlVendedorComision").val()) : 0;
            //var idActividad = ($("#ddlActividad").val() != null && $("#ddlActividad").val() != "") ? parseInt($("#ddlActividad").val()) : 0;

            var info = "{id: " + parseInt($("#hdnID").val())
                    + " , idPersona: " + parseInt($("#ddlPersona").val())
                    + " , tipo: '" + $("#ddlTipo").val()
                    + "', modo: '" + $("#ddlModo").val()
                    + "', fecha: '" + $("#txtFecha").val()
                    + "', condicionVenta: '" + $("#ddlCondicionVenta").val()
                    + "', tipoConcepto: " + parseInt($("#ddlProducto").val())
                    + " , fechaVencimiento: '" + $("#txtFechaVencimiento").val()
                    + "', idPuntoVenta: " + $("#ddlPuntoVenta").val()
                    + " , nroComprobante: '" + $("#txtNumero").val()
                    + "', obs: '" + $("#txtObservaciones").val()
                    + "', idJurisdiccion: " + idJurisdiccion
                    + " , nombre: '" + $("#txtNombre").val()
                    + "', vendedor: '" + $("#txtVendedor").val()
                    + "', envio: '" + $("#ddlEnvio").val()
                    + "', fechaEntrega: '" + $("#txtFechaEntrega").val()
                    + "', fechaAlta: '" + $("#txtFechaAlta").val()
                    + "', idComprobanteAsociado: " + idComprobanteAsociado
                    + ", idComprobanteOrigen: " + idComprobanteOrigen
                + ", idDomicilio: " + idDomicilio
                + ", idTransporte: " + idTransporte
                + ", idTransportePersona: " + idTransportePersona
                + ", idVendedorComision: " + idVendedorComision
                + ", estado: '" + $("#ddlEstado").val()
                + "', idCompraVinculada: " + parseInt($("#hdnIdCompraVinculada").val())
                + ", idActividad: " + $("#ddlActividad").val()
                + ", modalidadPagoAfip: '" + $("#ddlModalidadPagoAFIP").val()
                + "'}";
            
            $.ajax({
                type: "POST",
                url: "comprobantese.aspx/guardar",
                data: info,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data, text) {
                    Common.ocultarProcesando("lnkAceptar", "Aceptar");
                    Common.ocultarProcesando("lnkGenerarCAE", "Emitir factura eléctronica");
                    $("#hdnID").val(data.d);

                    if ($("#ddlTipo").val() == "NCA" || $("#ddlTipo").val() == "NCB" || $("#ddlTipo").val() == "NCC"
                        || $("#ddlTipo").val() == "NDA" || $("#ddlTipo").val() == "NDB" || $("#ddlTipo").val() == "NDC"
                        || $("#ddlTipo").val() == "NCAMP" || $("#ddlTipo").val() == "NCBMP" || $("#ddlTipo").val() == "NCCMP"
                        || $("#ddlTipo").val() == "NDAMP" || $("#ddlTipo").val() == "NDBMP" || $("#ddlTipo").val() == "NDCMP")
                    {
                        $("#divRemito,#divRemitoSinLogo").hide();
                    } else {
                        $("#divRemito,#divRemitoSinLogo").show();
                    }
                    if (generarCAE) {
                        // $("#hdnID").val(data.d);
                        //$("#litModalOkTitulo").html("Comprobante emitido correctamente");
                        generarCae();
                        //$('#modalOk').modal('show');
                    }
                    else {
                        $('#msgOk').html("Los datos se han actualizado correctamente");
                        $('#divOk').show();
                        $("#divError").hide();
                        $('html, body').animate({ scrollTop: 0 }, 'slow');
                        //Muestro la ventana                        
                        $('#divPrintpdf,#divDownloadPdf,#divDownloadPdfTicket,#divSendMail,#divRemito,#divCargarEntrega,#divCopiarAFacturaEdaFac').hide();

                        if ($("#ddlTipo").val() == "FCA" || $("#ddlTipo").val() == "FCB" || $("#ddlTipo").val() == "FCC"
                            || $("#ddlTipo").val() == "FCAMP" || $("#ddlTipo").val() == "FCBMP" || $("#ddlTipo").val() == "FCCMP"
                            ) {
                            $("#divVincularComprobante").hide();
                        }

                        if ($("#ddlTipo").val() == "PDV") {
                            $("#divVincularCompra").hide();
                            $("#divCargarEntrega").show();
                            $("#divRemito,#divRemitoSinLogo").show();
                        } else {
                            if ($("#ddlTipo").val() == "PDC") {
                                $("#divVincularComprobante").hide();
                            } else {
                                if ($("#ddlTipo").val() == "EDA") {
                                    $("#divVincularCompra").hide();
                                    $("#divVincularComprobante").hide();
                                    $("#divRemito,#divCopiarAFacturaEdaFac").show();
                                } else {
                                    if ($("#ddlTipo").val() == "DDC") {
                                        $("#divVincularComprobante").hide();
                                        $("#divVincularCompra").hide();
                                        $("#divCargarEntrega").hide();
                                        $("#divRemitoSinLogo").hide();                                        
                                        $("#divRemito").show();
                                    } else {
                                        $("#divCargarEntrega").show();
                                    }
                                }
                            }
                        }

                        //Muestro la ventana           
                        if ($("#hdnUsaPlanCorporativo").val() == "1" && $("#ddlModo").val() == "T" && $("#ddlTipo").val() != "COT") {
                            generarAsientosContables();
                        }
                        else {
                            $('#modalOk').modal('show');
                        }

                    }

                },
                error: function (response) {
                    var r = jQuery.parseJSON(response.responseText);
                    $("#msgError").html(r.Message);
                    $("#divError").show();
                    $("#divOk").hide();
                    $('html, body').animate({ scrollTop: 0 }, 'slow');
                    Common.ocultarProcesando("lnkAceptar", "Aceptar");
                    Common.ocultarProcesando("lnkGenerarCAE", "Emitir factura eléctronica");
                }
            });
        }
    }
    else {
        return false;
    }
}

function generarAsientosContables() {
    $.ajax({
        type: "POST",
        url: "comprobantese.aspx/GenerarAsientosContables",
        data: "{ id: " + $("#hdnID").val() + "}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data, text) {
            $("#divOkAsientos").show();
            $('#modalOk').modal('show');
        },
        error: function (response) {
            var r = jQuery.parseJSON(response.responseText);
            $("#msgErrorAsientos").html(r.Message);
            $("#divErrorAsientos").show();
            $('html, body').animate({ scrollTop: 0 }, 'slow');
            $('#modalOk').modal('show');
        }
    });
}

function enviarComprobanteAutomaticamente() {
    var info = "{ idComprobante: " + $("#hdnID").val() + " }";
    $.ajax({
        type: "POST",
        url: "/comprobantese.aspx/enviarComprobanteAutomaticamente",
        data: info,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        async: false,
        success: function (data) {
            $("#spSendMail").html("Enviado");
            $("#imgMailEnvio").attr("style", "color:#17a08c;font-size: 30px;");
            $("#spSendMail").attr("style", "color:#17a08c");
            $("#iCheckEnvio").addClass("fa fa-check");
        },
        error: function (response) {
            var r = jQuery.parseJSON(response.responseText);
            $("#msgErrorEnvioFE").html(r.Message);
            $("#divErrorEnvioFE").show();
            $("#divSendMail").show();
            $("#iCheckEnvio").removeClass("fa fa-check");
            $('#modalOk').modal('show');
        }
    });
}

function previsualizar() {

    if ($("#ddlTipo").val() == "NCA" || $("#ddlTipo").val() == "NCB" || $("#ddlTipo").val() == "NCC"
        || $("#ddlTipo").val() == "NDA" || $("#ddlTipo").val() == "NDB" || $("#ddlTipo").val() == "NDC"
        || $("#ddlTipo").val() == "NCAMP" || $("#ddlTipo").val() == "NCBMP" || $("#ddlTipo").val() == "NCCMP"
        || $("#ddlTipo").val() == "NDAMP" || $("#ddlTipo").val() == "NDBMP" || $("#ddlTipo").val() == "NDCMP") {
        if ($("#ddlComprobanteAsociado").val() == null || $("#ddlComprobanteAsociado").val() == "") {
            $("#msgError").html("Debe seleccionar un comprobante asociado para los tipo de comprobante Nota de credito/debito.");
            $("#divError").show();
            $("#divOk").hide();
            $('html, body').animate({ scrollTop: 0 }, 'slow');
            return;
        }
    }

    if ($("#ddlTipo").val() == "FCAMP" || $("#ddlTipo").val() == "FCBMP" || $("#ddlTipo").val() == "FCCMP"
        || $("#ddlTipo").val() == "NCAMP" || $("#ddlTipo").val() == "NCBMP" || $("#ddlTipo").val() == "NCCMP"
        || $("#ddlTipo").val() == "NDAMP" || $("#ddlTipo").val() == "NDBMP" || $("#ddlTipo").val() == "NDCMP") {
        if ($("#ddlModalidadPagoAFIP").val() == null || $("#ddlModalidadPagoAFIP").val() == "") {
            $("#msgError").html("Debe seleccionar una modalidad de pago AFIP.");
            $("#divError").show();
            $("#divOk").hide();
            $('html, body').animate({ scrollTop: 0 }, 'slow');
            return;
        }
    }

    var idComprobanteAsociado = ($("#ddlComprobanteAsociado").val() != null && $("#ddlComprobanteAsociado").val() != "") ? parseInt($("#ddlComprobanteAsociado").val()) : 0;

    if ($("#hdnFile").val() == "") {//Si no fue generado el archivo
        if ($('#frmEdicion').valid()) {
            var info = "{id: " + parseInt($("#hdnID").val())
                    + ", idPersona: " + parseInt($("#ddlPersona").val())
                    + ", tipo: '" + $("#ddlTipo").val()
                    + "', modo: '" + $("#ddlModo").val()
                    + "', fecha: '" + $("#txtFecha").val()
                    + "', condicionVenta: '" + $("#ddlCondicionVenta").val()
                    + "', tipoConcepto: " + parseInt($("#ddlProducto").val())
                    + ", fechaVencimiento: '" + $("#txtFechaVencimiento").val()
                    + "', idPuntoVenta: " + $("#ddlPuntoVenta").val()
                    + ", nroComprobante: '" + $("#txtNumero").val()
                    + "', obs: '" + $("#txtObservaciones").val()
                    + "', idComprobanteAsociado: " + idComprobanteAsociado
                    + ", fechaEntrega: '" + $("#txtFechaEntrega").val()
                    + "', notaDeCreditoPorServicio: '" + $("#hdnNotaDeCreditoPorServicio").val()
                    + "', idActividad: " + $("#ddlActividad").val()
                    + ", modalidadPagoAfip: '" + $("#ddlModalidadPagoAFIP").val()
                    + "'}";

            $.ajax({
                type: "POST",
                url: "comprobantese.aspx/previsualizar",
                data: info,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data, text) {
                    $("#divError").hide();
                    $('html, body').animate({ scrollTop: 0 }, 'slow');

                    var version = new Date().getTime();
                    $("#ifrPdf").attr("src", "/files/comprobantes/" + data.d + "?" + version + "#zoom=100&view=FitH,top");

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
            $("#msgError").html("Formulario Invalido Complete todos los datos obligatorios");
            $("#divError").show();
            $("#divOk").hide();
            return false;
        }
    }
    else {
        return false;
    }
}

function previsualizarTicket() {
    if ($("#hdnFile").val() == "") {//Si no fue generado el archivo
        if ($('#frmEdicion').valid()) {
            var info = "{id: " + parseInt($("#hdnID").val())
                + ", idPersona: " + parseInt($("#ddlPersona").val())
                + ", tipo: '" + $("#ddlTipo").val()
                + "', modo: '" + $("#ddlModo").val()
                + "', fecha: '" + $("#txtFecha").val()
                + "', condicionVenta: '" + $("#ddlCondicionVenta").val()
                + "', tipoConcepto: " + parseInt($("#ddlProducto").val())
                + ", fechaVencimiento: '" + $("#txtFechaVencimiento").val()
                + "', idPuntoVenta: " + $("#ddlPuntoVenta").val()
                + ", nroComprobante: '" + $("#txtNumero").val()
                + "', obs: '" + $("#txtObservaciones").val()
                + "'}";

            $.ajax({
                type: "POST",
                url: "comprobantese.aspx/previsualizarTicket",
                data: info,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data, text) {
                    $("#divError").hide();
                    $('html, body').animate({ scrollTop: 0 }, 'slow');

                    var version = new Date().getTime();
                    $("#ifrPdf").attr("src", "/files/comprobantes/" + data.d + "?" + version + "&view=FitH,top");

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
            $("#msgError").html("Formulario Invalido Complete todos los datos obligatorios");
            $("#divError").show();
            $("#divOk").hide();
            return false;
        }
    }
    else {
        return false;
    }
}

function descargarAdjunto() {
    var info = "{ id: '" + parseInt($("#hdnID").val()) + "'}";
    $.ajax({
        type: "POST",
        url: "comprobantese.aspx/descargarAdjunto",
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

function generarCae() {

    if ($("#ddlTipo").val() == "NCA" || $("#ddlTipo").val() == "NCB" || $("#ddlTipo").val() == "NCC"
        || $("#ddlTipo").val() == "NDA" || $("#ddlTipo").val() == "NDB" || $("#ddlTipo").val() == "NDC"
        || $("#ddlTipo").val() == "NCAMP" || $("#ddlTipo").val() == "NCBMP" || $("#ddlTipo").val() == "NCCMP"
        || $("#ddlTipo").val() == "NDAMP" || $("#ddlTipo").val() == "NDBMP" || $("#ddlTipo").val() == "NDCMP") {
        if ($("#ddlComprobanteAsociado").val() == null || $("#ddlComprobanteAsociado").val() == "") {
            $("#msgError").html("Debe seleccionar un comprobante asociado para los tipo de comprobante Nota de credito/debito.");
            $("#divError").show();
            $("#divOk").hide();
            $('html, body').animate({ scrollTop: 0 }, 'slow');
            return;
        }
    }

    if ($("#ddlTipo").val() == "FCAMP" || $("#ddlTipo").val() == "FCBMP" || $("#ddlTipo").val() == "FCCMP"
        || $("#ddlTipo").val() == "NCAMP" || $("#ddlTipo").val() == "NCBMP" || $("#ddlTipo").val() == "NCCMP"
        || $("#ddlTipo").val() == "NDAMP" || $("#ddlTipo").val() == "NDBMP" || $("#ddlTipo").val() == "NDCMP") {
        if ($("#ddlModalidadPagoAFIP").val() == null || $("#ddlModalidadPagoAFIP").val() == "") {
            $("#msgError").html("Debe seleccionar una modalidad de pago AFIP.");
            $("#divError").show();
            $("#divOk").hide();
            $('html, body').animate({ scrollTop: 0 }, 'slow');
            return;
        }
    }

    var idComprobanteAsociado = ($("#ddlComprobanteAsociado").val() != null && $("#ddlComprobanteAsociado").val() != "") ? parseInt($("#ddlComprobanteAsociado").val()) : 0;

    Common.mostrarProcesando("lnkAceptar");
    Common.mostrarProcesando("lnkGenerarCAE");
    var info = "{id: " + parseInt($("#hdnID").val())
            + ", idPersona: " + parseInt($("#ddlPersona").val())
            + ", tipo: '" + $("#ddlTipo").val()
            + "', modo: '" + $("#ddlModo").val()
            + "', fecha: '" + $("#txtFecha").val()
            + "', condicionVenta: '" + $("#ddlCondicionVenta").val()
            + "', tipoConcepto: " + parseInt($("#ddlProducto").val())
            + ", fechaVencimiento: '" + $("#txtFechaVencimiento").val()
            + "', idPuntoVenta: " + $("#ddlPuntoVenta").val()
            + ", nroComprobante: '" + $("#txtNumero").val()
            + "', obs: '" + $("#txtObservaciones").val()
            + "', idComprobanteAsociado: " + idComprobanteAsociado
            + ", fechaEntrega: '" + $("#txtFechaEntrega").val()
            + "', notaDeCreditoPorServicio: '" + $("#hdnNotaDeCreditoPorServicio").val()
            + "', idActividad: " + $("#ddlActividad").val()
            + ", modalidadPagoAfip: '" + $("#ddlModalidadPagoAFIP").val()
            + "'}";

    $.ajax({
        type: "POST",
        url: "comprobantese.aspx/generarCae",
        data: info,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data, text) {


            
            //Seteo el link de download
            //var fileName = data.d.split("/")[1];
            var fileName = data.d;
            $("#lnkDownloadPdf").attr("href", "/pdfGenerator.ashx?file=" + fileName.Comprobante);
            //$("#lnkDownloadRemito").attr("href", "/pdfGenerator.ashx?file=" + fileName.Remito);

            var d = new Date();
            var n = d.getFullYear();
            var version = new Date().getTime();
            var pathFile = "/files/explorer/" + $("#hdnIDUsuario").val() + "/comprobantes/" + n + "/" + fileName.Comprobante + "?" + version + "#zoom=100&view=FitH,top";
            $("#ifrPdf").attr("src", pathFile);

            //Seteo el nombre del archivo para envio de mail
            $("#hdnFile").val(fileName.Comprobante);

            //Cargo Datos en el cuerpo del mail para enviar
            obtenerMailPersona();
            completarAsuntoMailConDatosDelUsuario(fileName.Comprobante.toUpperCase());
            completarCuerpoMailConDatosDeLaPersona();

            $("#litModalOkTitulo").html("Comprobante emitido correctamente");

            if ($("#hdnEnvioFE").val() == "1") {
                enviarComprobanteAutomaticamente();
            }
            else {
                $("#iCheckEnvio").removeClass("fa fa-check");
            }

            //Muestro la ventana           
            if ($("#hdnUsaPlanCorporativo").val() == "1") {
                generarAsientosContables();
            }
            else {
                $('#modalOk').modal('show');
            }

            Common.ocultarProcesando("lnkAceptar", "Aceptar");
            Common.ocultarProcesando("lnkGenerarCAE", "Emitir factura eléctronica");
        },
        error: function (response) {
            var r = jQuery.parseJSON(response.responseText);
            $("#msgError").html(r.Message);
            $("#divError").show();
            $("#divOk").hide();
            $('html, body').animate({ scrollTop: 0 }, 'slow');
            Common.ocultarProcesando("lnkAceptar", "Aceptar");
            Common.ocultarProcesando("lnkGenerarCAE", "Emitir factura eléctronica");
        }
    });
}


function obtenerMailPersona() {

    var info = "{idPersona: " + parseInt($("#ddlPersona").val()) + "}";

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
            Common.ocultarProcesando("lnkAceptar", "Aceptar");
            Common.ocultarProcesando("lnkGenerarCAE", "Emitir factura eléctronica");
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
                    Common.ocultarProcesando("lnkAceptar", "Aceptar");
                    Common.ocultarProcesando("lnkGenerarCAE", "Emitir factura eléctronica");
                }
            });
        },
        error: function (response) {
            var r = jQuery.parseJSON(response.responseText);
            $("#msgError").html(r.Message);
            $("#divError").show();
            $("#divOk").hide();
            $('html, body').animate({ scrollTop: 0 }, 'slow');
            Common.ocultarProcesando("lnkAceptar", "Aceptar");
            Common.ocultarProcesando("lnkGenerarCAE", "Emitir factura eléctronica");
        }
    });
}


function completarAsuntoMailConDatosDelUsuarioDesdeListado(nroComprobante) {

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

function completarCuerpoMailConDatosDeLaPersona() {

    var info = "{idPersona: " + parseInt($("#ddlPersona").val()) + "}";

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
                    Common.ocultarProcesando("lnkAceptar", "Aceptar");
                    Common.ocultarProcesando("lnkGenerarCAE", "Emitir factura eléctronica");
                }
            });
        },
        error: function (response) {
            var r = jQuery.parseJSON(response.responseText);
            $("#msgError").html(r.Message);
            $("#divError").show();
            $("#divOk").hide();
            $('html, body').animate({ scrollTop: 0 }, 'slow');
            Common.ocultarProcesando("lnkAceptar", "Aceptar");
            Common.ocultarProcesando("lnkGenerarCAE", "Emitir factura eléctronica");
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

function cancelar() {
    var tipoComprobante = getParameterByName('tipo');
    if (tipoComprobante != "") {
        if (tipoComprobante == "DDC") {
            var idCompra = $("#hdnIdCompraVinculada").val();
            window.location.href = "/comprase.aspx?ID=" + idCompra;
        }
        else
            window.location.href = "/comprobantes.aspx?tipo=" + tipoComprobante;
    } else {
        window.location.href = "/comprobantes.aspx";
    }
}

function cerrarModelOk() {
    var tipoComprobante = getParameterByName('tipo');    
    if (tipoComprobante != "") {
        if (tipoComprobante == "DDC") {
            var idCompra = $("#hdnIdCompraVinculada").val();
            window.location.href = "/comprase.aspx?ID=" + idCompra;
        }           
        else
           window.location.href = "/comprobantes.aspx?tipo=" + tipoComprobante;
    } else {
        window.location.href = "/comprobantes.aspx";
    }
}

$(document).ajaxStart(function () {
    $(document.body).css({ 'cursor': 'wait' });
}).ajaxStop(function () {
    $(document.body).css({ 'cursor': 'default' });
});

function configForm() {

    $(".select2").select2({
        width: '100%', allowClear: true,
        formatNoMatches: function (term) {
            return "<a style='cursor:pointer' onclick=\"$('#modalNuevoCliente').modal('show');$('.select2').select2('close');\">+ Agregar</a>";
        }
    });
    $(".select3").select2({ width: '100%', allowClear: true, });
    $(".select4").select2({
        width: '100%', allowClear: true,
        formatNoMatches: function (term) {
            return "<a style='cursor:pointer' onclick=\"$('#modalNuevoDomicilio').modal('show');$('.select2').select2('close');\">+ Agregar</a>";
        }
    });
    $(".select5").select2({
        width: '100%', allowClear: true,
        formatNoMatches: function (term) {
            return "<a style='cursor:pointer' onclick=\"$('#modalNuevoTransporte').modal('show');$('.select2').select2('close');\">+ Agregar</a>";
        }
    });
    $(".select6").select2({
        width: '100%', allowClear: true,
        formatNoMatches: function (term) {
            return "<a style='cursor:pointer' onclick=\"$('#modalNuevoTransportePersona').modal('show');$('.select2').select2('close');\">+ Agregar</a>";
        }
    });
    //$(".select7").select2({
    //    width: '100%', allowClear: true,
    //    formatNoMatches: function (term) {
    //        return "<a style='cursor:pointer' onclick=\"$('#modalNuevaActividad').modal('show');$('.select2').select2('close');\">+ Agregar</a>";
    //    }
    //});

    if (MI_CONDICION == "MO") {
        $("#ddlIva").attr("disabled", true);
    }
    else if (MI_CONDICION == "RI") {
        $("#ddlIva").val("5");
    }

    $.validator.addMethod("validFechaActual", function (value, element) {

        var fInicio = $("#txtFecha").val().split("/");
        var fechainicio = new Date(fInicio[2], (fInicio[1] - 1), fInicio[0]);

        var today = new Date();
        var fechaFin = new Date(today);
        fechaFin.setDate(today.getDate() + 5);

        if (fechainicio > fechaFin) {
            return false;
        }
        else {
            return true;
        }
    }, "Solo puede facturar hasta 5 dias despues de la fecha actual");

    $('#txtFechaVencimiento').datepicker();
    Common.configDatePicker();

    $("#txtPrecio,#txtBonificacion").keypress(function (event) {
        var keycode = (event.keyCode ? event.keyCode : event.which);
        if (keycode == '13') {
            $("#txtCantidad").focus();
            agregarItem();
            return false;
        }
    });

    //if ($("#hdnIDUsuario").val() != "5188" && $("#hdnIDUsuario").val() != "5190"
    //    && $("#hdnIDUsuario").val() != "5248" && $("#hdnIDUsuario").val() != "3155" && $("#hdnIDUsuario").val() != "5339") { //FIOL
    //    $("#txtCantidad").numericInput();
    //} else {
    //    $("#txtCantidad").maskMoney({ thousands: '', decimal: ',', allowZero: true });
    //    $("#txtCantidad").val("1,00");
    //}
    if ($("#hdnUsaCantidadConDecimales").val() == "1") {
        $("#txtCantidad").maskMoney({ thousands: '', decimal: ',', allowZero: true });
        $("#txtCantidad").val("1,00");
    } else {
        $("#txtCantidad").numericInput();
        $("#txtCantidad").val("1");
    }

    if ($("#hdnHabilitarCambioIvaEnArticulosDesdeComprobante").val() == "1") {
        $('#ddlIva').prop('disabled', false);
    } else {
        $('#ddlIva').prop('disabled', true);
    }
    
    //$("#txtCantidad").numericInput();

    $("#txtNumero").mask("?99999999");
    $("#txtNumero").blur(function () {
        $("#txtNumero").val(padZeros($("#txtNumero").val(), 8));
    });

    $("#txtPrecio, #txtBonificacion,#txtImporteNoGravado,#txtPercepcionIVA,#txtPercepcionIIBB").maskMoney({ thousands: '', decimal: ',', allowZero: true });

    $("#txtEnvioPara, #txtEnvioAsunto, #txtEnvioMensaje").keypress(function (event) {
        var aux = Toolbar.toggleEnviosError();
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
    })

    $("#lnkAceptar,#lnkGenerarCAE,#lnkDescargarAdjunto,#lnkPrevisualizar").hide();
    if ($("#hdnPedidoDeVenta").val() == "1") { 
        limpiarNuevoComprobante();
        $("#lnkAceptar,#lnkPrevisualizar,#lnkPrevisualizarTicket,#divFactura").show();
        //Common.obtenerClientes("ddlPersona", $("#hdnIDPersona").val(), false);
        Common.obtenerPersonas("ddlPersona", $("#hdnIDPersona").val(), false);
        Common.obtenerPuntosDeVenta("ddlPuntoVenta");
        Common.obtenerPuntosDeVenta("ddlPuntoVentaFac1");
        Common.obtenerPuntosDeVenta("ddlPuntoVentaFac2");
        Common.obtenerActividades("ddlActividad","",false);
        obtenerInfoPersona(parseInt($("#ddlPersona").val()), '');
        $("#lnkGenerarCAE,#divModo,#divPuntoVenta,#divFechas,#divNroComprobante,#divDescripcion").hide();
        $("#txtNumero").attr("readonly", "readonly");
        $("#txtNumero").val(nroActual)
    } else {
        Common.obtenerPersonas("ddlPersona", $("#hdnIDPersona").val(), true);
        Common.obtenerPuntosDeVenta("ddlPuntoVenta");
        Common.obtenerPuntosDeVenta("ddlPuntoVentaFac1");
        Common.obtenerPuntosDeVenta("ddlPuntoVentaFac2");
        Common.obtenerActividades("ddlActividad","", false);
    }
    
    
    Common.obtenerConceptosCodigoyNombre("ddlProductos", 3, true);
    Common.obtenerTransportes("ddlTransporte", $("#hdnIDTransporte").val(), false);
    Common.obtenerVendedores("ddlVendedorComision", "", true);

    if ($("#hdnID").val() != "" && $("#hdnID").val() != "0") {
        loadInfo($("#hdnID").val());
        $("#lnkAceptar").html("Actualizar");
    }
    else {
        Common.obtenerProvincias("ddlJuresdiccion", "", true);
        $("#ddlPersona").attr("onchange", "changePersona()");
        $("#lnkPrevisualizar").hide();
        $("#lnkPrevisualizarTicket").hide();        
    } 

    mostrarPresupuesto();
    mostrarPercepciones();
    verificarPlanDeCuentas();   

    var tipoComprobante = getParameterByName('tipo');    
    if (tipoComprobante != "") {
        if (tipoComprobante == "PDV") {
            $("#lblTitulo").text("Pedidos de Ventas");
            $("#lblSubtitulo").text("Pedidos de Ventas");
            $("#lblCondicion").text("Condición de Venta");
            $("#htextoFactura").html("2. Productos o servicios");
            $("#lblDatosGenerales").text("Acá van los datos más generales del pedido.");
            $("#lblTextoFactura").text("Agrega a continuación los productos o servicios que vas a incluir en el pedido.");
        } else {
            if (tipoComprobante == "PDC") {
                $("#lblTitulo").text("Pedidos de Compras");
                $("#lblSubtitulo").text("Pedidos de Compras");
                $("#lblCondicion").text("Condición de Compra");
                $("#lblDatosGenerales").text("Acá van los datos más generales del pedido.");
                $("#lblTextoFactura").text("Agrega a continuación los productos o servicios que vas a incluir en el pedido.");
            } else {
                if (tipoComprobante == "EDA") {
                    $("#lblTitulo").text("Entregas");
                    $("#lblSubtitulo").text("Entregas");
                    $("#lblCondicion").text("Condición de Entrega");
                    $("#htextoFactura").html("2. Productos o servicios");
                    $("#lblDatosGenerales").text("Acá van los datos más generales de la entrega.");
                    $("#lblTextoFactura").text("");
                } else {
                    if (tipoComprobante == "DDC") {
                        $("#lblTitulo").text("Detalle del Comprobante");
                        $("#lblSubtitulo").text("Detalle");
                        $("#lblCondicion").text("Condición del Comprobante");
                        $("#htextoFactura").html("2. Productos o servicios");
                        $("#lblDatosGenerales").text("Acá van los datos más generales del detalle del comprobante.");
                        $("#lblTextoFactura").text("");
                    } else {
                        $("#lblTitulo").text("Facturación");
                        $("#lblSubtitulo").text("Facturación");
                        $("#lblCondicion").text("Condición de Venta");
                        $("#lblDatosGenerales").text("Acá van los datos más generales de la factura.");
                        $("#lblTextoFactura").text("Agrega a continuación los productos o servicios que vas a incluir en la factura.");
                    }
                }
            }
        }
    } else {
        $("#lblTitulo").text("Facturación");
        $("#lblSubtitulo").text("Facturación");
        $("#lblCondicion").text("Condición de Venta");
        $("#lblDatosGenerales").text("Acá van los datos más generales de la factura.");
        $("#lblTextoFactura").text("Agrega a continuación los productos o servicios que vas a incluir en la factura.");
    }


    if ($("#hdnIdPedidoDeVenta").val() != "0") {
        $("#ddlPersona").val($("#hdnIDPersona").val());
        changePersona();
    }

    if (tipoComprobante == "DDC" && $("#hdnIdCompraVinculada").val() != "0") {
        $("#ddlPersona").val($("#hdnIDPersona").val());
        changePersona();
        cargarTotalCompras();
    }       
}

function cargarTotalCompras() {

    $.ajax({
        type: "POST",
        url: "comprase.aspx/getTotal",
        data: "{ idCompra: " + $("#hdnIdCompraVinculada").val() + "}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data, text) {
            $("#divTotalCompras").html("$ " + data.d);
        },
        error: function (response) {
            var r = jQuery.parseJSON(response.responseText);
            $("#msgError").html(r.Message);
            $("#divError").show();
            $('html, body').animate({ scrollTop: 0 }, 'slow');
        }
    });

}

function verificarPlanDeCuentas() {
    if ($("#hdnUsaPlanCorporativo").val() == "1") {
        $(".divPlanDeCuentas").show();
    }
    else {
        $(".divPlanDeCuentas").hide();
    }
}

function mostrarPresupuesto() {
    if (parseInt($("#hdnPresupuesto").val()) > 0) {
        $("#ddlPersona").trigger("change");
        obtenerItems();
        obtenerTotales();
        $("#ddlPersona").attr("onchange", "changePersona()");
    }
}

function mostrarPercepciones() {
    if (MI_CONDICION == "MO") {
        $("#trIVA,#trIIBB,#divPercepcionesIIBB,#divPercepcionesIVA,#divPercepciones").hide();
    }
    else {
        //$("#trImpNoGravado").show();
        $("#divPercepciones").hide();

        if ($("#hdnPercepcionIVA").val() == "1")
            $("#trIVA,#divPercepcionesIVA,#divPercepciones").show();
        else
            $("#trIVA,#divPercepcionesIVA").hide();

        if ($("#hdnPercepcionIIBB").val() == "1")
            $("#trIIBB,#divPercepcionesIIBB,#divPercepciones").show();
        else
            $("#trIIBB,#divPercepcionesIIBB").hide();

        if ($("#ddlTipo").val() == "PDV" || $("#ddlTipo").val() == "PDC" || $("#ddlTipo").val() == "EDA") {
            $("#htextoFactura").html("2. Productos o servicios");
        } else {
            if ($("#hdnPercepcionIVA").val() == "1" || $("#hdnPercepcionIIBB").val() == "1") {
                $("#htextoFactura").html("4. Productos o servicios");
            }
            else {
                $("#htextoFactura").html("3. Productos o servicios");
            }  
        }
              
    }
    if ($("#hdnPedidoDeVenta").val() == "1") {
        $("#htextoFactura").html("2. Productos");
    } 
    //Lo oculto hasta que se agregue lo de impuesto no gravado
    //$("#trImpNoGravado").hide();
}
/*** SEARCH ***/

function configFilters() {
    $(".select2").select2({ width: '100%', allowClear: true });

    $("#txtCondicion").keypress(function (event) {
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

    // validation with select boxes
    $("#frmsearch").validate({
        highlight: function (element) {
            jquery(element).closest('.form-group').removeclass('has-success').addclass('has-error');
        },
        success: function (element) {
            jquery(element).closest('.form-group').removeclass('has-error');
        },
        errorelement: 'span',
        errorclass: 'help-block',
        errorplacement: function (error, element) {
            if (element.parent('.input-group').length) {
                error.insertafter(element.parent());
            } else {
                error.insertafter(element);
            }
        }
    });

}

function nuevo() {
    var tipoComprobante = getParameterByName('tipo');    
    if (tipoComprobante != "") {
        window.location.href = "/comprobantese.aspx?tipo=" + tipoComprobante;
    } else {
        window.location.href = "/comprobantese.aspx";
    }    
}

function editar(id) {
    var tipoComprobante = getParameterByName('tipo');
    if (tipoComprobante != "") {
        window.location.href = "/comprobantese.aspx?tipo=" + tipoComprobante + "&ID=" + id;
    } else {
        window.location.href = "/comprobantese.aspx?ID=" + id;
    }       
}

function editarCobranza(id) {
    window.location.href = "/cobranzase.aspx?ID=" + id;
}

function anular() {
    var id = $("#hdnID").val();
    bootbox.confirm("¿Está seguro que desea eliminar el comprobante actual?", function (result) {
        if (result) {
            $.ajax({
                type: "POST",
                url: "comprobantes.aspx/delete",
                data: "{ id: " + id + "}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data, text) {
                    $('#msgOk').html("Comprobante anulado correctamente.");
                    $("#divOk").show();
                    $('html, body').animate({ scrollTop: 0 }, 'slow');
                    setTimeout(function () {
                        window.location.href = "/comprobantes.aspx";
                    }, 3000);  
                },
                error: function (response) {
                    var r = jQuery.parseJSON(response.responseText);
                    $("#divError").html(r.Message);
                    $("#divError").show();
                    $("#divOk").hide();
                    $('html, body').animate({ scrollTop: 0 }, 'slow');
                }
            });
        }
    });
}

function eliminar(id, nombre) {
    bootbox.confirm("¿Está seguro que desea eliminar el comprobante realizado a " + nombre + "? Tambien se eliminaran los comprobantes vinculados al mismo", function (result) {
        if (result) {
            $.ajax({
                type: "POST",
                url: "comprobantes.aspx/delete",
                data: "{ id: " + id + "}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data, text) {
                    $('#msgOk').html("Comprobante eliminado correctamente.");
                    $("#divOk").show();
                    $('html, body').animate({ scrollTop: 0 }, 'slow');
                    setTimeout(function () {
                        filtrar();
                    }, 3000);                    
                },
                error: function (response) {
                    var r = jQuery.parseJSON(response.responseText);
                    $("#divError").html(r.Message);
                    $("#divError").show();
                    $("#divOk").hide();
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

function titulos() {
    //Common.obtenerClientes("ddlPersona", $("#hdnIDPersona").val(), false);
    //Common.obtenerPeriodos("ddlCuadroResumenPeriodo", $("#hdnIDPeriodo").val(), true);
    var tipoComprobante = getParameterByName('tipo');    
    if (tipoComprobante != "") {
        if (tipoComprobante == "PDV" || tipoComprobante == "PDC") {
            $("#lblTitulo").text("Pedidos realizados");
            $("#lblSubtitulo").text("Pedidos");
            $("#lblNuevo").text("Nuevo Pedido");
            if (tipoComprobante == "PDV") {
                $("#divMasFiltros").show();
                $("#divEntregas").show();
                $("#divEntregaPendiente").show();
                $("#divEntregaEstado").hide();
                $("#divCobranzas").show();
                $("#divFacturas").show();
                $("#divCuadroResumen,#divReiniciarFiltros").show();
                //$("#divReiniciarFiltros").show();
                if ($("#hdnIDUsuario").val() == "5188") { //FIOL
                    $('#chkEntregaPendiente').prop('checked', true);
                    $('#chkCobranzaPendiente').prop('checked', true);
                    $('#chkFacturacionPendiente').prop('checked', true);
                } else {
                    $('#chkEntregaPendiente').prop('checked', false);
                    $('#chkCobranzaPendiente').prop('checked', false);
                    $('#chkFacturacionPendiente').prop('checked', false);
                }
            }
        } else {
            if (tipoComprobante == "EDA") {
                $("#divMasFiltros").show();
                $("#divEntregas").show();
                $("#divEntregaPendiente").hide();
                $("#divEntregaEstado").show();
                $("#divCobranzas").hide();
                $("#divFacturas").hide();
                $('#chkEntregaPendiente').prop('checked', false);
                $('#chkCobranzaPendiente').prop('checked', false);
                $('#chkFacturacionPendiente').prop('checked', false);

                $("#lblTitulo").text("Entregas realizadas");
                $("#lblSubtitulo").text("Entregas");
                $("#lblNuevo").text("Nuevo Entrega");
            } else {
                if (tipoComprobante == "DDC") {
                    $("#divMasFiltros").show();
                    $("#divEntregas").show();
                    $("#divEntregaPendiente").hide();
                    $("#divEntregaEstado").show();
                    $("#divCobranzas").hide();
                    $("#divFacturas").hide();
                    $('#chkEntregaPendiente').prop('checked', false);
                    $('#chkCobranzaPendiente').prop('checked', false);
                    $('#chkFacturacionPendiente').prop('checked', false);

                    $("#lblTitulo").text("Detalle de comprobante realizados");
                    $("#lblSubtitulo").text("Detalle");
                    $("#btnAcciones").hide();
                } else {
                    $("#lblTitulo").text("Facturas realizadas");
                    $("#lblSubtitulo").text("Facturación");
                    $("#lblNuevo").text("Nueva Factura");
                }
            }
        }
    } else {
        $("#lblTitulo").text("Facturas realizadas");
        $("#lblSubtitulo").text("Facturación");
        $("#lblNuevo").text("Nueva Factura");
    }


}

function filtrar() {
    $("#divError").hide();
    $("#divOk").hide();

    if ($("#hdnFiltroPorCantidad"otroPeriodo).val() == "1") {
        $("#ddlPeriodo").val(30);
        $("#hdnFiltroPorCantidad").val("0");
    }

    var tipoComprobante = getParameterByName('tipo'); 

    if ($('#frmSearch').valid()) {
        $("#resultsContainer").html("");

        var currentPage = parseInt($("#hdnPage").val());

        var info = "{ condicion: '" + $("#txtCondicion").val()
                   + "', periodo: '" + $("#ddlPeriodo").val()
                   + "', fechaDesde: '" + $("#txtFechaDesde").val()
                   + "', fechaHasta: '" + $("#txtFechaHasta").val()
                   + "', page: " + currentPage + ", pageSize: " + PAGE_SIZE
                   + ", tipo: '" + tipoComprobante
                   + "', entregaPendiente: " + $("#chkEntregaPendiente").is(":checked")
                   + ", entregaEstado: " + $("#chkEntregaEstado").is(":checked")
                   + ", estado: '" + $("#ddlEstado").val()
                   + "', cobranzaPendiente: " + $("#chkCobranzaPendiente").is(":checked")
                   + ", facturaPendiente: " + $("#chkFacturacionPendiente").is(":checked")
                   + "}";

        $.ajax({
            type: "POST",
            url: "comprobantes.aspx/getResults",
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

function documentosVinculados(id) {

    var tipoComprobante = getParameterByName('tipo');

    if (tipoComprobante == "PDV") {
        documentoRaizVinculado(id);
        comprobantesVinculados(id);
        cobranzasVinculadas(id);
        chequesVinculados(id);
        entregasVinculadas(id);
        calcularPendiente(id);
    } else {
        $.ajax({
            type: "POST",
            url: "comprobantes.aspx/getResultsDocumentoRaizDeComprobante",
            data: "{ id: " + id + "}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data, text) {
                if (data.d != null) {
                    var id = data.d;
                    documentoRaizVinculado(id);
                    comprobantesVinculados(id);
                    cobranzasVinculadas(id);
                    chequesVinculados(id);
                    entregasVinculadas(id);
                    calcularPendiente(id);
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

}

function documentoRaizVinculado(id) {

    $("#divErrorDocumentoRaiz").hide();

    $("#resultsContainerDocumentoRaiz").html("");

    $.ajax({
        type: "POST",
        url: "comprobantes.aspx/getResultsDocumentoRaiz",
        data: "{ id: " + id + "}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data, text) {
            // Render using the template
            if (data.d.Items.length > 0)
                $("#resultTemplateDocumentoRaiz").tmpl({ results: data.d.Items }).appendTo("#resultsContainerDocumentoRaiz");
            else
                $("#noResultTemplateDocumentoRaiz").tmpl({ results: data.d.Items }).appendTo("#resultsContainerDocumentoRaiz");
        },
        error: function (response) {
            var r = jQuery.parseJSON(response.responseText);
            $("#msgErrorDocumentoRaiz").html(r.Message);
            $("#divErrorDocumentoRaiz").show();
            $('html, body').animate({ scrollTop: 0 }, 'slow');
        }
    });

}

function comprobantesVinculados(id) {

    $("#divErrorComprobantesVinculados").hide();
    $('#modalComprobantesVinculados').modal('hide');

    $("#resultsContainerComprobantesVinculados").html("");

    $.ajax({
        type: "POST",
        url: "comprobantes.aspx/getResultsComprobantesVinculados",
        data: "{ id: " + id + "}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data, text) {
            $('#modalComprobantesVinculados').modal('show');
            // Render using the template
            if (data.d.Items.length > 0)
                $("#resultTemplateComprobantesVinculados").tmpl({ results: data.d.Items }).appendTo("#resultsContainerComprobantesVinculados");
            else
                $("#noResultTemplateComprobantesVinculados").tmpl({ results: data.d.Items }).appendTo("#resultsContainerComprobantesVinculados");

        },
        error: function (response) {
            var r = jQuery.parseJSON(response.responseText);
            $("#msgErrorComprobantesVinculados").html(r.Message);
            $("#divErrorComprobantesVinculados").show();
            $('html, body').animate({ scrollTop: 0 }, 'slow');
        }
    });
    
}

function cobranzasVinculadas(id) {

    $("#divErrorCobranzasVinculadas").hide();

    $("#resultsContainerCobranzasVinculadas").html("");

    $.ajax({
        type: "POST",
        url: "cobranzas.aspx/getResultsCobranzasVinculadas",
        data: "{ id: " + id + "}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data, text) {
            // Render using the template
            if (data.d.Items.length > 0)
                $("#resultTemplateCobranzasVinculadas").tmpl({ results: data.d.Items }).appendTo("#resultsContainerCobranzasVinculadas");
            else
                $("#noResultTemplateCobranzasVinculadas").tmpl({ results: data.d.Items }).appendTo("#resultsContainerCobranzasVinculadas");
        },
        error: function (response) {
            var r = jQuery.parseJSON(response.responseText);
            $("#msgErrorCobranzasVinculadas").html(r.Message);
            $("#divErrorCobranzasVinculadas").show();
            $('html, body').animate({ scrollTop: 0 }, 'slow');
        }
    });

}

function chequesVinculados(id) {

    $("#divErrorChequesVinculados").hide();

    $("#resultsContainerChequesVinculados").html("");

    $.ajax({
        type: "POST",
        url: "cobranzas.aspx/getResultsChequesVinculados",
        data: "{ id: " + id + "}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data, text) {
            // Render using the template
            if (data.d.Items.length > 0)
                $("#resultTemplateChequesVinculados").tmpl({ results: data.d.Items }).appendTo("#resultsContainerChequesVinculados");
            else
                $("#noResultTemplateChequesVinculados").tmpl({ results: data.d.Items }).appendTo("#resultsContainerChequesVinculados");
        },
        error: function (response) {
            var r = jQuery.parseJSON(response.responseText);
            $("#msgErrorChequesVinculados").html(r.Message);
            $("#divErrorChequesVinculados").show();
            $('html, body').animate({ scrollTop: 0 }, 'slow');
        }
    });

}

function entregasVinculadas(id) {

    $("#divErrorEntregasVinculadas").hide();

    $("#resultsContainerEntregasVinculadas").html("");

    $.ajax({
        type: "POST",
        url: "comprobantes.aspx/getResultsEntregasVinculadas",
        data: "{ id: " + id + "}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data, text) {
            // Render using the template
            if (data.d.Items.length > 0)
                $("#resultTemplateEntregasVinculadas").tmpl({ results: data.d.Items }).appendTo("#resultsContainerEntregasVinculadas");
            else
                $("#noResultTemplateEntregasVinculadas").tmpl({ results: data.d.Items }).appendTo("#resultsContainerEntregasVinculadas");
        },
        error: function (response) {
            var r = jQuery.parseJSON(response.responseText);
            $("#msgErrorEntregasVinculadas").html(r.Message);
            $("#divErrorEntregasVinculadas").show();
            $('html, body').animate({ scrollTop: 0 }, 'slow');
        }
    });

}

function calcularPendiente(id) {

    var mensaje = "";

    $.ajax({
        type: "POST",
        url: "comprobantes.aspx/calcularPendienteCobranza",
        data: "{ id: " + id + "}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data, text) {
            mensaje = "Pendiente de Cobranza: $ " + number_format(data.d, 2) + "";

            $.ajax({
                type: "POST",
                url: "comprobantes.aspx/calcularPendienteFacturar",
                data: "{ id: " + id + "}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data, text) {
                    mensaje = mensaje + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Pendiente de Facturar: $ " + number_format(data.d, 2);

                    $.ajax({
                        type: "POST",
                        url: "comprobantes.aspx/calcularPendienteCAE",
                        data: "{ id: " + id + "}",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (data, text) {
                            mensaje = mensaje + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Pendiente CAE: $ " + number_format(data.d, 2);

                            $.ajax({
                                type: "POST",
                                url: "comprobantes.aspx/calcularPendienteEntrega",
                                data: "{ id: " + id + "}",
                                contentType: "application/json; charset=utf-8",
                                dataType: "json",
                                success: function (data, text) {
                                    mensaje = mensaje + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Pendiente Entrega: " + number_format(data.d, 0) + " % ";
                                    $("#msgTotalPendiente").html(mensaje);
                                },
                                error: function (response) {
                                    var r = jQuery.parseJSON(response.responseText);
                                    $("#msgErrorDocumentoRaiz").html(r.Message);
                                    $("#divErrorDocumentoRaiz").show();
                                    $('html, body').animate({ scrollTop: 0 }, 'slow');
                                }
                            });
                        },
                        error: function (response) {
                            var r = jQuery.parseJSON(response.responseText);
                            $("#msgErrorDocumentoRaiz").html(r.Message);
                            $("#divErrorDocumentoRaiz").show();
                            $('html, body').animate({ scrollTop: 0 }, 'slow');
                        }
                    });
                    
                },
                error: function (response) {
                    var r = jQuery.parseJSON(response.responseText);
                    $("#msgErrorDocumentoRaiz").html(r.Message);
                    $("#divErrorDocumentoRaiz").show();
                    $('html, body').animate({ scrollTop: 0 }, 'slow');
                }
            });

        },
        error: function (response) {
            var r = jQuery.parseJSON(response.responseText);
            $("#msgErrorDocumentoRaiz").html(r.Message);
            $("#divErrorDocumentoRaiz").show();
            $('html, body').animate({ scrollTop: 0 }, 'slow');
        }
    });

}


function verTodos() {
    $("#txtCondicion, #txtFechaDesde, #txtFechaHasta").val("");
    $("#ddlPersona").val("").trigger("change");
    filtrar();
}

function exportar() {
    resetearExportacion();

    $("#imgLoading").show();
    $("#divIconoDescargar").hide();
    var tipoComprobante = getParameterByName('tipo');    

    var info = "{  condicion: '" + $("#txtCondicion").val()
              + "', periodo: '" + $("#ddlPeriodo").val()
              + "', fechaDesde: '" + $("#txtFechaDesde").val()
              + "', fechaHasta: '" + $("#txtFechaHasta").val()
              + "', tipo: '" + tipoComprobante
              + "'}";

    $.ajax({
        type: "POST",
        url: "comprobantes.aspx/export",
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
            $("#divOk").hide();
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
        $('#divEntreFechas').toggle(600);
    else {
        if ($("#divEntreFechas").is(":visible"))
            $('#divEntreFechas').toggle(600);

        $("#txtFechaDesde,#txtFechaHasta").val("");
        filtrar();
    }
}

function enviarEmail() {
    $("#divErrorMail").hide();
    Toolbar.mostrarEnvio();
    $('#modalEMail').modal('show');
}

//function editarActividad() {
//    $("#divErrorEditarActividad").hide();
//    $('#modalEditarActividad').modal('show');
//}

//function modificarActividad() {

//    var info = "{idComprobante: " + parseInt($("#hdnID").val()) + ", idActividad: " + parseInt($("#ddlEditarActividad").val()) + "}";

//    $.ajax({
//        type: "POST",
//        url: "comprobantese.aspx/modificarActividad",
//        data: info,
//        contentType: "application/json; charset=utf-8",
//        dataType: "json",
//        success: function (data, text) {
//            $("#hdnIDActividad").val($("#ddlEditarActividad").val());
//            $('#modalEditarActividad').modal('hide');
//        },
//        error: function (response) {
//            var r = jQuery.parseJSON(response.responseText);
//            $("#msgErrorEditarActividad").html(r.Message);
//            $("#divErrorEditarActividad").show();
//        }
//    });
//}

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
            completarAsuntoMailConDatosDelUsuarioDesdeListado(fileName.Comprobante.toUpperCase());
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


function generarRemito(frame, conLogo) {

    if ($("#hdnID").val() == "0") {
        bootbox.confirm("Para imprimir el remito el comprobante se guardará, ¿Continuar?", function (result) {
            if (result) {
                ocultarMensajes();

                if ($("#ddlPuntoVenta").val() == null) {
                    $("#msgError").html("Debe registrar al menos un punto de venta");
                    $("#divError").show();
                    $("#divOk").hide();
                    $('html, body').animate({ scrollTop: 0 }, 'slow');
                    return;
                }

                if ($("#ddlTipo").val() == "NCA" || $("#ddlTipo").val() == "NCB" || $("#ddlTipo").val() == "NCC"
                    || $("#ddlTipo").val() == "NDA" || $("#ddlTipo").val() == "NDB" || $("#ddlTipo").val() == "NDC"
                    || $("#ddlTipo").val() == "NCAMP" || $("#ddlTipo").val() == "NCBMP" || $("#ddlTipo").val() == "NCCMP"
                    || $("#ddlTipo").val() == "NDAMP" || $("#ddlTipo").val() == "NDBMP" || $("#ddlTipo").val() == "NDCMP") {
                    if ($("#ddlComprobanteAsociado").val() == null || $("#ddlComprobanteAsociado").val() == "") {
                        $("#msgError").html("Debe seleccionar un comprobante asociado para los tipo de comprobante Nota de credito/debito.");
                        $("#divError").show();
                        $("#divOk").hide();
                        $('html, body').animate({ scrollTop: 0 }, 'slow');
                        return;
                    }
                }

                if ($("#ddlTipo").val() == "FCAMP" || $("#ddlTipo").val() == "FCBMP" || $("#ddlTipo").val() == "FCCMP"
                    || $("#ddlTipo").val() == "NCAMP" || $("#ddlTipo").val() == "NCBMP" || $("#ddlTipo").val() == "NCCMP"
                    || $("#ddlTipo").val() == "NDAMP" || $("#ddlTipo").val() == "NDBMP" || $("#ddlTipo").val() == "NDCMP") {
                    if($("#ddlModalidadPagoAFIP").val() == null || $("#ddlModalidadPagoAFIP").val() == "") {
                        $("#msgError").html("Debe seleccionar una modalidad de pago AFIP.");
                        $("#divError").show();
                        $("#divOk").hide();
                        $('html, body').animate({ scrollTop: 0 }, 'slow');
                        return;
                    }
                }

                if ($("#ddlTipo").val() == "EDA") {
                    var itemIdPersona = ($("#ddlPersona").val() == "" || $("#ddlPersona").val() == null) ? 0 : parseInt($("#ddlPersona").val());

                    $('#tbItems tr').each(function () {

                        if ($(this).find("td").eq(0).html() != null) {

                            var info = "{ id: " + parseInt($(this).find("td").eq(0).html())
                                + ", idConcepto: '" + $(this).find("td").eq(3).find("input").prop('value')
                                + "', concepto: '" + $(this).find("td").eq(3).find("a").prop('title')
                                + "', iva: '" + $(this).find("td").eq(7).html().replace(".", "").replace(",", ".")
                                + "', precio: '" + $(this).find("td").eq(4).html().replace(".", "").replace(",", ".")
                                + "', bonif: '" + $(this).find("td").eq(5).html().replace(".", "").replace(",", ".")
                                + "', cantidad: '" + $(this).find("td").find("input").prop('value').replace(".", "").replace(",", ".")
                                + "', idPersona: " + itemIdPersona
                                + " , codigo:'" + $(this).find("td").eq(2).html()
                                + "', idPlanDeCuenta:''"
                                + " , codigoCta: ''}";
                            $.ajax({
                                type: "POST",
                                url: "comprobantese.aspx/agregarItemPorCodigoYConcepto",
                                data: info,
                                contentType: "application/json; charset=utf-8",
                                dataType: "json",
                                success: function (data, text) {
                                    verificarPlanDeCuentas();
                                    obtenerTotales();
                                },
                                error: function (response) {
                                    var r = jQuery.parseJSON(response.responseText);
                                    $("#msgErrorDetalle").html(r.Message);
                                    $("#divErrorDetalle").show();
                                }
                            });
                        }

                    });
                }

                if ($('#frmEdicion').valid()) {
                    if ($("#ddlTipo").val() != "COT" && $("#ddlModo").val() == "O") {
                        $("#msgError").html("El modo del comprobante es inválido para un " + $("#ddlTipo option:selected").text());
                        $("#divError").show();
                        $("#divOk").hide();
                        $('html, body').animate({ scrollTop: 0 }, 'slow');
                    }
                    else {
                        Common.mostrarProcesando("lnkAceptar");
                        Common.mostrarProcesando("lnkGenerarCAE");

                        var idJurisdiccion = ($("#ddlJuresdiccion").val() != null && $("#ddlJuresdiccion").val() != "") ? parseInt($("#ddlJuresdiccion").val()) : 0;
                        var idComprobanteAsociado = ($("#ddlComprobanteAsociado").val() != null && $("#ddlComprobanteAsociado").val() != "") ? parseInt($("#ddlComprobanteAsociado").val()) : 0;
                        var idComprobanteOrigen = ($("#ddlComprobantePersona").val() != null && $("#ddlComprobantePersona").val() != "") ? parseInt($("#ddlComprobantePersona").val()) : 0;
                        var idDomicilio = ($("#ddlDomicilio").val() != null && $("#ddlDomicilio").val() != "") ? parseInt($("#ddlDomicilio").val()) : 0;
                        var idTransporte = ($("#ddlTransporte").val() != null && $("#ddlTransporte").val() != "") ? parseInt($("#ddlTransporte").val()) : 0;
                        var idTransportePersona = ($("#ddlTransportePersona").val() != null && $("#ddlTransportePersona").val() != "") ? parseInt($("#ddlTransportePersona").val()) : 0;
                        var idVendedorComision = ($("#ddlVendedorComision").val() != null && $("#ddlVendedorComision").val() != "") ? parseInt($("#ddlVendedorComision").val()) : 0;
                        //var idActividad = ($("#ddlActividad").val() != null && $("#ddlActividad").val() != "") ? parseInt($("#ddlActividad").val()) : 0;


                        var info = "{id: " + parseInt($("#hdnID").val())
                            + " , idPersona: " + parseInt($("#ddlPersona").val())
                            + " , tipo: '" + $("#ddlTipo").val()
                            + "', modo: '" + $("#ddlModo").val()
                            + "', fecha: '" + $("#txtFecha").val()
                            + "', condicionVenta: '" + $("#ddlCondicionVenta").val()
                            + "', tipoConcepto: " + parseInt($("#ddlProducto").val())
                            + " , fechaVencimiento: '" + $("#txtFechaVencimiento").val()
                            + "', idPuntoVenta: " + $("#ddlPuntoVenta").val()
                            + " , nroComprobante: '" + $("#txtNumero").val()
                            + "', obs: '" + $("#txtObservaciones").val()
                            + "', idJurisdiccion: " + idJurisdiccion
                            + " , nombre: '" + $("#txtNombre").val()
                            + "', vendedor: '" + $("#txtVendedor").val()
                            + "', envio: '" + $("#ddlEnvio").val()
                            + "', fechaEntrega: '" + $("#txtFechaEntrega").val()
                            + "', fechaAlta: '" + $("#txtFechaAlta").val()
                            + "', idComprobanteAsociado: " + idComprobanteAsociado
                            + ", idComprobanteOrigen: " + idComprobanteOrigen
                            + ", idDomicilio: " + idDomicilio
                            + ", idTransporte: " + idTransporte
                            + ", idTransportePersona: " + idTransportePersona
                            + ", idVendedorComision: " + idVendedorComision
                            + ", estado: '" + $("#ddlEstado").val()
                            + "', idCompraVinculada: " + parseInt($("#hdnIdCompraVinculada").val())
                            + ", idActividad: " + $("#ddlActividad").val()
                            + ", modalidadPagoAfip: '" + $("#ddlModalidadPagoAFIP").val()
                            + "'}";

                        $.ajax({
                            type: "POST",
                            url: "comprobantese.aspx/guardar",
                            data: info,
                            contentType: "application/json; charset=utf-8",
                            dataType: "json",
                            success: function (data, text) {           

                                $("#hdnID").val(data.d);

                                var info = "{ id: " + $("#hdnID").val() + ", conLogo: " + conLogo + "}";

                                $.ajax({
                                    type: "POST",
                                    url: "comprobantesv.aspx/generarRemito",
                                    data: info,
                                    contentType: "application/json; charset=utf-8",
                                    dataType: "json",
                                    success: function (data, text) {
                                        window.location.href = "/pdfGenerator.ashx?file=" + data.d + "&tipoDeArchivo=remito";
                                    },
                                    error: function (response) {
                                        var r = jQuery.parseJSON(response.responseText);
                                        $('#msgError' + frame).html(r.Message);
                                        $('#divError' + frame).show();
                                        $('#divOk' + frame).hide();
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
                                Common.ocultarProcesando("lnkAceptar", "Aceptar");
                                Common.ocultarProcesando("lnkGenerarCAE", "Emitir factura eléctronica");
                            }
                        });
                    }
                }
                else {
                    return false;
                }
            }
        });
    } else {
        var info = "{ id: " + $("#hdnID").val() + ", conLogo: " + conLogo + "}";

        $.ajax({
            type: "POST",
            url: "comprobantesv.aspx/generarRemito",
            data: info,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data, text) {
                window.location.href = "/pdfGenerator.ashx?file=" + data.d + "&tipoDeArchivo=remito";
            },
            error: function (response) {
                var r = jQuery.parseJSON(response.responseText);
                $('#msgError' + frame).html(r.Message);
                $('#divError' + frame).show();
                $('#divOk' + frame).hide();
                $('html, body').animate({ scrollTop: 0 }, 'slow');
            }
        });
    }

}

function abrirModalRemitoSinLogo(frame) {

    if ($("#hdnID").val() == "0") {
        bootbox.confirm("Para imprimir el remito el comprobante se guardará, ¿Continuar?", function (result) {
            if (result) {
                ocultarMensajes();

                if ($("#ddlPuntoVenta").val() == null) {
                    $("#msgError").html("Debe registrar al menos un punto de venta");
                    $("#divError").show();
                    $("#divOk").hide();
                    $('html, body').animate({ scrollTop: 0 }, 'slow');
                    return;
                }

                if ($("#ddlTipo").val() == "NCA" || $("#ddlTipo").val() == "NCB" || $("#ddlTipo").val() == "NCC"
                    || $("#ddlTipo").val() == "NDA" || $("#ddlTipo").val() == "NDB" || $("#ddlTipo").val() == "NDC"
                    || $("#ddlTipo").val() == "NCAMP" || $("#ddlTipo").val() == "NCBMP" || $("#ddlTipo").val() == "NCCMP"
                    || $("#ddlTipo").val() == "NDAMP" || $("#ddlTipo").val() == "NDBMP" || $("#ddlTipo").val() == "NDCMP"                ) {
                    if ($("#ddlComprobanteAsociado").val() == null || $("#ddlComprobanteAsociado").val() == "") {
                        $("#msgError").html("Debe seleccionar un comprobante asociado para los tipo de comprobante Nota de credito/debito.");
                        $("#divError").show();
                        $("#divOk").hide();
                        $('html, body').animate({ scrollTop: 0 }, 'slow');
                        return;
                    }
                }

                if ($("#ddlTipo").val() == "FCAMP" || $("#ddlTipo").val() == "FCBMP" || $("#ddlTipo").val() == "FCCMP"
                    || $("#ddlTipo").val() == "NCAMP" || $("#ddlTipo").val() == "NCBMP" || $("#ddlTipo").val() == "NCCMP"
                    || $("#ddlTipo").val() == "NDAMP" || $("#ddlTipo").val() == "NDBMP" || $("#ddlTipo").val() == "NDCMP") {
                    if($("#ddlModalidadPagoAFIP").val() == null || $("#ddlModalidadPagoAFIP").val() == "") {
                        $("#msgError").html("Debe seleccionar una modalidad de pago AFIP.");
                        $("#divError").show();
                        $("#divOk").hide();
                        $('html, body').animate({ scrollTop: 0 }, 'slow');
                        return;
                    }
                }

                if ($("#ddlTipo").val() == "EDA") {
                    var itemIdPersona = ($("#ddlPersona").val() == "" || $("#ddlPersona").val() == null) ? 0 : parseInt($("#ddlPersona").val());

                    $('#tbItems tr').each(function () {

                        if ($(this).find("td").eq(0).html() != null) {

                            var info = "{ id: " + parseInt($(this).find("td").eq(0).html())
                                + ", idConcepto: '" + $(this).find("td").eq(3).find("input").prop('value')
                                + "', concepto: '" + $(this).find("td").eq(3).find("a").prop('title')
                                + "', iva: '" + $(this).find("td").eq(7).html().replace(".", "").replace(",", ".")
                                + "', precio: '" + $(this).find("td").eq(4).html().replace(".", "").replace(",", ".")
                                + "', bonif: '" + $(this).find("td").eq(5).html().replace(".", "").replace(",", ".")
                                + "', cantidad: '" + $(this).find("td").find("input").prop('value').replace(".", "").replace(",", ".")
                                + "', idPersona: " + itemIdPersona
                                + " , codigo:'" + $(this).find("td").eq(2).html()
                                + "', idPlanDeCuenta:''"
                                + " , codigoCta: ''}";
                            $.ajax({
                                type: "POST",
                                url: "comprobantese.aspx/agregarItemPorCodigoYConcepto",
                                data: info,
                                contentType: "application/json; charset=utf-8",
                                dataType: "json",
                                success: function (data, text) {
                                    verificarPlanDeCuentas();
                                    obtenerTotales();
                                },
                                error: function (response) {
                                    var r = jQuery.parseJSON(response.responseText);
                                    $("#msgErrorDetalle").html(r.Message);
                                    $("#divErrorDetalle").show();
                                }
                            });
                        }

                    });
                }

                if ($('#frmEdicion').valid()) {
                    if ($("#ddlTipo").val() != "COT" && $("#ddlModo").val() == "O") {
                        $("#msgError").html("El modo del comprobante es inválido para un " + $("#ddlTipo option:selected").text());
                        $("#divError").show();
                        $("#divOk").hide();
                        $('html, body').animate({ scrollTop: 0 }, 'slow');
                    }
                    else {
                        Common.mostrarProcesando("lnkAceptar");
                        Common.mostrarProcesando("lnkGenerarCAE");

                        var idJurisdiccion = ($("#ddlJuresdiccion").val() != null && $("#ddlJuresdiccion").val() != "") ? parseInt($("#ddlJuresdiccion").val()) : 0;
                        var idComprobanteAsociado = ($("#ddlComprobanteAsociado").val() != null && $("#ddlComprobanteAsociado").val() != "") ? parseInt($("#ddlComprobanteAsociado").val()) : 0;
                        var idComprobanteOrigen = ($("#ddlComprobantePersona").val() != null && $("#ddlComprobantePersona").val() != "") ? parseInt($("#ddlComprobantePersona").val()) : 0;
                        var idDomicilio = ($("#ddlDomicilio").val() != null && $("#ddlDomicilio").val() != "") ? parseInt($("#ddlDomicilio").val()) : 0;
                        var idTransporte = ($("#ddlTransporte").val() != null && $("#ddlTransporte").val() != "") ? parseInt($("#ddlTransporte").val()) : 0;
                        var idTransportePersona = ($("#ddlTransportePersona").val() != null && $("#ddlTransportePersona").val() != "") ? parseInt($("#ddlTransportePersona").val()) : 0;
                        var idVendedorComision = ($("#ddlVendedorComision").val() != null && $("#ddlVendedorComision").val() != "") ? parseInt($("#ddlVendedorComision").val()) : 0;
                        //var idActividad = ($("#ddlActividad").val() != null && $("#ddlActividad").val() != "") ? parseInt($("#ddlActividad").val()) : 0;


                        var info = "{id: " + parseInt($("#hdnID").val())
                            + " , idPersona: " + parseInt($("#ddlPersona").val())
                            + " , tipo: '" + $("#ddlTipo").val()
                            + "', modo: '" + $("#ddlModo").val()
                            + "', fecha: '" + $("#txtFecha").val()
                            + "', condicionVenta: '" + $("#ddlCondicionVenta").val()
                            + "', tipoConcepto: " + parseInt($("#ddlProducto").val())
                            + " , fechaVencimiento: '" + $("#txtFechaVencimiento").val()
                            + "', idPuntoVenta: " + $("#ddlPuntoVenta").val()
                            + " , nroComprobante: '" + $("#txtNumero").val()
                            + "', obs: '" + $("#txtObservaciones").val()
                            + "', idJurisdiccion: " + idJurisdiccion
                            + " , nombre: '" + $("#txtNombre").val()
                            + "', vendedor: '" + $("#txtVendedor").val()
                            + "', envio: '" + $("#ddlEnvio").val()
                            + "', fechaEntrega: '" + $("#txtFechaEntrega").val()
                            + "', fechaAlta: '" + $("#txtFechaAlta").val()
                            + "', idComprobanteAsociado: " + idComprobanteAsociado
                            + ", idComprobanteOrigen: " + idComprobanteOrigen
                            + ", idDomicilio: " + idDomicilio
                            + ", idTransporte: " + idTransporte
                            + ", idTransportePersona: " + idTransportePersona
                            + ", idVendedorComision: " + idVendedorComision
                            + ", estado: '" + $("#ddlEstado").val()
                            + "', idCompraVinculada: " + parseInt($("#hdnIdCompraVinculada").val())
                            + ", idActividad: " + $("#ddlActividad").val()
                            + ", modalidadPagoAfip: '" + $("#ddlModalidadPagoAFIP").val()
                            + "'}";

                        $.ajax({
                            type: "POST",
                            url: "comprobantese.aspx/guardar",
                            data: info,
                            contentType: "application/json; charset=utf-8",
                            dataType: "json",
                            success: function (data, text) {

                                $("#hdnID").val(data.d);

                                $.ajax({
                                    type: "POST",
                                    url: "comprobantese.aspx/getMargenesRemitoSinLogo",
                                    contentType: "application/json; charset=utf-8",
                                    dataType: "json",
                                    success: function (data, text) {
                                        if (data != null) {
                                            $("#txtRemitoSinLogoMargenHorizontal").val(data.d.Horizontal);
                                            $("#txtRemitoSinLogoMargenVertical").val(data.d.Vertical);
                                            $('#modalRemitoSinLogo').modal('show');
                                        }
                                    },
                                    error: function (response) {
                                        var r = jQuery.parseJSON(response.responseText);
                                        $('#msgError' + frame).html(r.Message);
                                        $('#divError' + frame).show();
                                        $('#divOk' + frame).hide();
                                    }
                                });
                            },
                            error: function (response) {
                                var r = jQuery.parseJSON(response.responseText);
                                $("#msgError").html(r.Message);
                                $("#divError").show();
                                $("#divOk").hide();
                                $('html, body').animate({ scrollTop: 0 }, 'slow');
                                Common.ocultarProcesando("lnkAceptar", "Aceptar");
                                Common.ocultarProcesando("lnkGenerarCAE", "Emitir factura eléctronica");
                            }
                        });
                    }
                }
                else {
                    return false;
                }
            }
        });
    } else {

        $.ajax({
            type: "POST",
            url: "comprobantese.aspx/getMargenesRemitoSinLogo",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data, text) {
                if (data != null) {
                    $("#txtRemitoSinLogoMargenHorizontal").val(data.d.Horizontal);
                    $("#txtRemitoSinLogoMargenVertical").val(data.d.Vertical);
                    $('#modalRemitoSinLogo').modal('show');
                }
            },
            error: function (response) {
                var r = jQuery.parseJSON(response.responseText);
                $('#msgError' + frame).html(r.Message);
                $('#divError' + frame).show();
                $('#divOk' + frame).hide();
            }
        });

        
    }




}


function abrirModalRemitoTalonario(frame) {

    if ($("#hdnID").val() == "0") {
        bootbox.confirm("Para imprimir el remito el comprobante se guardará, ¿Continuar?", function (result) {
            if (result) {
                ocultarMensajes();

                if ($("#ddlPuntoVenta").val() == null) {
                    $("#msgError").html("Debe registrar al menos un punto de venta");
                    $("#divError").show();
                    $("#divOk").hide();
                    $('html, body').animate({ scrollTop: 0 }, 'slow');
                    return;
                }

                if ($("#ddlTipo").val() == "NCA" || $("#ddlTipo").val() == "NCB" || $("#ddlTipo").val() == "NCC"
                    || $("#ddlTipo").val() == "NDA" || $("#ddlTipo").val() == "NDB" || $("#ddlTipo").val() == "NDC"
                    || $("#ddlTipo").val() == "NCAMP" || $("#ddlTipo").val() == "NCBMP" || $("#ddlTipo").val() == "NCCMP"
                    || $("#ddlTipo").val() == "NDAMP" || $("#ddlTipo").val() == "NDBMP" || $("#ddlTipo").val() == "NDCMP"                ) {
                    if ($("#ddlComprobanteAsociado").val() == null || $("#ddlComprobanteAsociado").val() == "") {
                        $("#msgError").html("Debe seleccionar un comprobante asociado para los tipo de comprobante Nota de credito/debito.");
                        $("#divError").show();
                        $("#divOk").hide();
                        $('html, body').animate({ scrollTop: 0 }, 'slow');
                        return;
                    }
                }

                if ($("#ddlTipo").val() == "FCAMP" || $("#ddlTipo").val() == "FCBMP" || $("#ddlTipo").val() == "FCCMP"
                    || $("#ddlTipo").val() == "NCAMP" || $("#ddlTipo").val() == "NCBMP" || $("#ddlTipo").val() == "NCCMP"
                    || $("#ddlTipo").val() == "NDAMP" || $("#ddlTipo").val() == "NDBMP" || $("#ddlTipo").val() == "NDCMP") {
                    if($("#ddlModalidadPagoAFIP").val() == null || $("#ddlModalidadPagoAFIP").val() == "") {
                        $("#msgError").html("Debe seleccionar una modalidad de pago AFIP.");
                        $("#divError").show();
                        $("#divOk").hide();
                        $('html, body').animate({ scrollTop: 0 }, 'slow');
                        return;
                    }
                }

                if ($("#ddlTipo").val() == "EDA") {
                    var itemIdPersona = ($("#ddlPersona").val() == "" || $("#ddlPersona").val() == null) ? 0 : parseInt($("#ddlPersona").val());

                    $('#tbItems tr').each(function () {

                        if ($(this).find("td").eq(0).html() != null) {

                            var info = "{ id: " + parseInt($(this).find("td").eq(0).html())
                                + ", idConcepto: '" + $(this).find("td").eq(3).find("input").prop('value')
                                + "', concepto: '" + $(this).find("td").eq(3).find("a").prop('title')
                                + "', iva: '" + $(this).find("td").eq(7).html().replace(".", "").replace(",", ".")
                                + "', precio: '" + $(this).find("td").eq(4).html().replace(".", "").replace(",", ".")
                                + "', bonif: '" + $(this).find("td").eq(5).html().replace(".", "").replace(",", ".")
                                + "', cantidad: '" + $(this).find("td").find("input").prop('value').replace(".", "").replace(",", ".")
                                + "', idPersona: " + itemIdPersona
                                + " , codigo:'" + $(this).find("td").eq(2).html()
                                + "', idPlanDeCuenta:''"
                                + " , codigoCta: ''}";
                            $.ajax({
                                type: "POST",
                                url: "comprobantese.aspx/agregarItemPorCodigoYConcepto",
                                data: info,
                                contentType: "application/json; charset=utf-8",
                                dataType: "json",
                                success: function (data, text) {
                                    verificarPlanDeCuentas();
                                    obtenerTotales();
                                },
                                error: function (response) {
                                    var r = jQuery.parseJSON(response.responseText);
                                    $("#msgErrorDetalle").html(r.Message);
                                    $("#divErrorDetalle").show();
                                }
                            });
                        }

                    });
                }

                if ($('#frmEdicion').valid()) {
                    if ($("#ddlTipo").val() != "COT" && $("#ddlModo").val() == "O") {
                        $("#msgError").html("El modo del comprobante es inválido para un " + $("#ddlTipo option:selected").text());
                        $("#divError").show();
                        $("#divOk").hide();
                        $('html, body').animate({ scrollTop: 0 }, 'slow');
                    }
                    else {
                        Common.mostrarProcesando("lnkAceptar");
                        Common.mostrarProcesando("lnkGenerarCAE");

                        var idJurisdiccion = ($("#ddlJuresdiccion").val() != null && $("#ddlJuresdiccion").val() != "") ? parseInt($("#ddlJuresdiccion").val()) : 0;
                        var idComprobanteAsociado = ($("#ddlComprobanteAsociado").val() != null && $("#ddlComprobanteAsociado").val() != "") ? parseInt($("#ddlComprobanteAsociado").val()) : 0;
                        var idComprobanteOrigen = ($("#ddlComprobantePersona").val() != null && $("#ddlComprobantePersona").val() != "") ? parseInt($("#ddlComprobantePersona").val()) : 0;
                        var idDomicilio = ($("#ddlDomicilio").val() != null && $("#ddlDomicilio").val() != "") ? parseInt($("#ddlDomicilio").val()) : 0;
                        var idTransporte = ($("#ddlTransporte").val() != null && $("#ddlTransporte").val() != "") ? parseInt($("#ddlTransporte").val()) : 0;
                        var idTransportePersona = ($("#ddlTransportePersona").val() != null && $("#ddlTransportePersona").val() != "") ? parseInt($("#ddlTransportePersona").val()) : 0;
                        var idVendedorComision = ($("#ddlVendedorComision").val() != null && $("#ddlVendedorComision").val() != "") ? parseInt($("#ddlVendedorComision").val()) : 0;
                        //var idActividad = ($("#ddlActividad").val() != null && $("#ddlActividad").val() != "") ? parseInt($("#ddlActividad").val()) : 0;


                        var info = "{id: " + parseInt($("#hdnID").val())
                            + " , idPersona: " + parseInt($("#ddlPersona").val())
                            + " , tipo: '" + $("#ddlTipo").val()
                            + "', modo: '" + $("#ddlModo").val()
                            + "', fecha: '" + $("#txtFecha").val()
                            + "', condicionVenta: '" + $("#ddlCondicionVenta").val()
                            + "', tipoConcepto: " + parseInt($("#ddlProducto").val())
                            + " , fechaVencimiento: '" + $("#txtFechaVencimiento").val()
                            + "', idPuntoVenta: " + $("#ddlPuntoVenta").val()
                            + " , nroComprobante: '" + $("#txtNumero").val()
                            + "', obs: '" + $("#txtObservaciones").val()
                            + "', idJurisdiccion: " + idJurisdiccion
                            + " , nombre: '" + $("#txtNombre").val()
                            + "', vendedor: '" + $("#txtVendedor").val()
                            + "', envio: '" + $("#ddlEnvio").val()
                            + "', fechaEntrega: '" + $("#txtFechaEntrega").val()
                            + "', fechaAlta: '" + $("#txtFechaAlta").val()
                            + "', idComprobanteAsociado: " + idComprobanteAsociado
                            + ", idComprobanteOrigen: " + idComprobanteOrigen
                            + ", idDomicilio: " + idDomicilio
                            + ", idTransporte: " + idTransporte
                            + ", idTransportePersona: " + idTransportePersona
                            + ", idVendedorComision: " + idVendedorComision
                            + ", estado: '" + $("#ddlEstado").val()
                            + "', idCompraVinculada: " + parseInt($("#hdnIdCompraVinculada").val())
                            + ", idActividad: " + $("#ddlActividad").val()
                            + ", modalidadPagoAfip: '" + $("#ddlModalidadPagoAFIP").val()
                            + "'}";

                        $.ajax({
                            type: "POST",
                            url: "comprobantese.aspx/guardar",
                            data: info,
                            contentType: "application/json; charset=utf-8",
                            dataType: "json",
                            success: function (data, text) {

                                $("#hdnID").val(data.d);

                                $.ajax({
                                    type: "POST",
                                    url: "comprobantese.aspx/getMargenesRemitoTalonario",
                                    contentType: "application/json; charset=utf-8",
                                    dataType: "json",
                                    success: function (data, text) {
                                        if (data != null) {
                                            $("#txtRemitoTalonarioMargenHorizontal").val(data.d.Horizontal);
                                            $("#txtRemitoTalonarioMargenVertical").val(data.d.Vertical);
                                            $('#modalRemitoTalonario').modal('show');
                                        }
                                    },
                                    error: function (response) {
                                        var r = jQuery.parseJSON(response.responseText);
                                        $('#msgError' + frame).html(r.Message);
                                        $('#divError' + frame).show();
                                        $('#divOk' + frame).hide();
                                    }
                                });
                            },
                            error: function (response) {
                                var r = jQuery.parseJSON(response.responseText);
                                $("#msgError").html(r.Message);
                                $("#divError").show();
                                $("#divOk").hide();
                                $('html, body').animate({ scrollTop: 0 }, 'slow');
                                Common.ocultarProcesando("lnkAceptar", "Aceptar");
                                Common.ocultarProcesando("lnkGenerarCAE", "Emitir factura eléctronica");
                            }
                        });
                    }
                }
                else {
                    return false;
                }
            }
        });
    } else {

        $.ajax({
            type: "POST",
            url: "comprobantese.aspx/getMargenesRemitoTalonario",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data, text) {
                if (data != null) {
                    $("#txtRemitoTalonarioMargenHorizontal").val(data.d.Horizontal);
                    $("#txtRemitoTalonarioMargenVertical").val(data.d.Vertical);
                    $('#modalRemitoTalonario').modal('show');
                }
            },
            error: function (response) {
                var r = jQuery.parseJSON(response.responseText);
                $('#msgError' + frame).html(r.Message);
                $('#divError' + frame).show();
                $('#divOk' + frame).hide();
            }
        });


    }




}


function imprimirRemitoSinLogo() {

    $("#divError").hide();

    var datos = "{ vertical: '" + $("#txtRemitoSinLogoMargenVertical").val()
        + "', horizontal: '" + $("#txtRemitoSinLogoMargenHorizontal").val()
        + "'}";

    $.ajax({
        type: "POST",
        url: "comprobantesv.aspx/updateMargenesRemitoSinLogo",
        data: datos,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data, text) {

            var info = "{ id: " + $("#hdnID").val() + "}";

            $.ajax({
                type: "POST",
                url: "comprobantesv.aspx/generarRemitoSinLogo",
                data: info,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data, text) {
                    window.location.href = "/pdfGenerator.ashx?file=" + data.d + "&tipoDeArchivo=remito";
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


function imprimirRemitoTalonario() {

    $("#divError").hide();

    var datos = "{ vertical: '" + $("#txtRemitoTalonarioMargenVertical").val()
        + "', horizontal: '" + $("#txtRemitoTalonarioMargenHorizontal").val()
        + "'}";

    $.ajax({
        type: "POST",
        url: "comprobantesv.aspx/updateMargenesRemitoTalonario",
        data: datos,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data, text) {

            var info = "{ id: " + $("#hdnID").val()
                + ", verTotal: " + $("#chkRemitoTalonarioVerTotal").is(":checked") 
                + "}";

            $.ajax({
                type: "POST",
                url: "comprobantesv.aspx/generarRemitoTalonario",
                data: info,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data, text) {
                    window.location.href = "/pdfGenerator.ashx?file=" + data.d + "&tipoDeArchivo=remito";
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

function generarEntrega(frame) {

    var info = "{ id: " + $("#hdnID").val() + "}";

    $.ajax({
        type: "POST",
        url: "comprobantese.aspx/getEntregasDeUnPedido",
        data: info,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data, text) {
            if (data.d != "0") {
                $('#msgError' + frame).html("Ya existe una entrega vinculada al pedido de venta actual.");
                $('#divError' + frame).show();
                $('#divOk' + frame).hide();
                $('html, body').animate({ scrollTop: 0 }, 'slow');
            } else {
                window.location.href = "comprobantese.aspx?tipo=EDA&ID" + $("#hdnTipo").val().toUpperCase() + "=" + $("#hdnID").val();
            }            
        },
        error: function (response) {
            var r = jQuery.parseJSON(response.responseText);
            $("#msgErrorDocumentosRaiz").html(r.Message);
            $("#divErrorDocumentosRaiz").show();
            $('html, body').animate({ scrollTop: 0 }, 'slow');
            resetearExportacionDocumentosRaiz();
        }
    });
    
}

function generarEntregaTotal() {
    var info = "{ id: " + $("#hdnID").val() + "}";

    $.ajax({
        type: "POST",
        url: "comprobantese.aspx/generarEntregaTotal",
        data: info,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data, text) {
            window.location.href = "/comprobantes.aspx?tipo=EDA";
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

function generarNotaDeCreditoPorServicio() {
    bootbox.confirm("¿Está seguro que desea generar una nota de crédito como servicio?", function (result) {
        if (result) {
            $("#hdnNotaDeCreditoPorServicio").val("1");
            grabar(true);
        }
    });
}

function generarTicket() {
    var info = "{ id: " + $("#hdnID").val() + "}";

    $.ajax({
        type: "POST",
        url: "comprobantesv.aspx/generarTicket",
        data: info,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data, text) {
            window.location.href = "/pdfGenerator.ashx?file=" + data.d;
            //window.open(
            //    "/pdfGenerator.ashx?file=" + data.d,
            //    '_blank' // <- This is what makes it open in a new window.
            //);        
            //$("#divError").hide();
            //$("#lnkDownloadPdfTicket").attr("href", "/pdfGenerator.ashx?file=" + data.d);
            //var d = new Date();
            //var n = d.getFullYear();
            //var pathFile = "/files/explorer/" + $("#hdnIDUsuario").val() + "/comprobantes/" + n + "/" + data.d + "#view=FitH,top";            
            //$("#ifrPdf").attr("src", pathFile); 
            //$("#modalPdf").modal("show");    
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

function vincularComprobante(frame) {
    var info = "{ id: " + $("#hdnID").val() + "}";
    var superaSuma = false;

    $.ajax({
        type: "POST",
        url: "comprobantese.aspx/sumatoriaDeFacturasDeUnPedido",
        data: info,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data, text) {
            superaSuma = data.d;
            $.ajax({
                type: "POST",
                url: "comprobantese.aspx/vincularComprobante",
                data: info,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data, text) {
                    var mensaje = "Pedido de venta copiado a factura generado exitosamente.";
                    if (superaSuma) {
                        mensaje = mensaje + " <strong>¡Atencion! La sumatoria de las facturas vinculadas al pedido es superior al total del mismo.</strong>";
                    }
                    $('#msgOk' + frame).html(mensaje);
                    $('#divOk' + frame).show();
                    $('html, body').animate({ scrollTop: 0 }, 'slow');
                    setTimeout(function () {
                        window.location.href = "/comprobantese.aspx?tipo=FAC&ID=" + data.d;
                    }, 3000);                    
                },
                error: function (response) {
                    var r = jQuery.parseJSON(response.responseText);
                    $('#msgError' + frame).html(r.Message);
                    $('#divError' + frame).show();
                    $('#divOk' + frame).hide();
                    $('html, body').animate({ scrollTop: 0 }, 'slow');
                }
            });
        },
        error: function (response) {
            var r = jQuery.parseJSON(response.responseText);
            $('#msgError' + frame).html(r.Message);
            $('#divError' + frame).show();
            $('#divOk' + frame).hide();
            $('html, body').animate({ scrollTop: 0 }, 'slow');
        }
    });
}

function vincularComprobanteDividido(frame) {
    var info = "{ id: " + $("#hdnID").val() + "}";
    var superaSuma = false;

    $.ajax({
        type: "POST",
        url: "comprobantese.aspx/sumatoriaDeFacturasDeUnPedido",
        data: info,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data, text) {
            superaSuma = data.d;        
            $("#txtSuperaSuma").val(superaSuma);
            $('#modalFacturaDividida').modal('show');
        },
        error: function (response) {
            var r = jQuery.parseJSON(response.responseText);
            $('#msgError' + frame).html(r.Message);
            $('#divError' + frame).show();
            $('#divOk' + frame).hide();
            $('html, body').animate({ scrollTop: 0 }, 'slow');
        }
    });
}

function vincularComprobanteDivididoGenerar(frame) {
    var info = "{ id: " + $("#hdnID").val() + "}";
    var superaSuma = $("#hdnSuperaSuma").val();

    if ($("#ddlPuntoVentaFac1").val() == $("#ddlPuntoVentaFac2").val()) {
        $('#msgError' + frame).html("Los puntos de venta no pueden ser iguales.");
        $('#divError' + frame).show();
        $('#divOk' + frame).hide();
    } else {
        var info = "{id: " + parseInt($("#hdnID").val())
            + " , rangoFacturas: '" + $("#txtRangoFacturas").val()
            + "', idPuntoVentaFac1: " + $("#ddlPuntoVentaFac1").val()
            + " , idPuntoVentaFac2: " + $("#ddlPuntoVentaFac2").val()
            + " }";


        $.ajax({
            type: "POST",
            url: "comprobantese.aspx/vincularComprobanteDivididoGenerar",
            data: info,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data, text) {
                var mensaje = "Pedido de venta copiado a dos facturas exitosamente.";
                if (superaSuma) {
                    mensaje = mensaje + " <strong>¡Atencion! La sumatoria de las facturas vinculadas al pedido es superior al total del mismo.</strong>";
                }
                $('#msgOk' + frame).html(mensaje);
                $('#divOk' + frame).show();
                $('html, body').animate({ scrollTop: 0 }, 'slow');
                setTimeout(function () {
                    window.location.href = "/comprobantese.aspx?tipo=FAC&ID=" + data.d;
                }, 3000);
            },
            error: function (response) {
                var r = jQuery.parseJSON(response.responseText);
                $('#msgError' + frame).html(r.Message);
                $('#divError' + frame).show();
                $('#divOk' + frame).hide();
                $('html, body').animate({ scrollTop: 0 }, 'slow');
            }
        });
    }

    
}


function vincularComprobanteEdaFac(frame) {

    if ($("#hdnID").val() == "0") {
        bootbox.confirm("Para copiar a una factura desde EDA comprobante se guardará, ¿Continuar?", function (result) {
            if (result) {
                ocultarMensajes();

                if ($("#ddlPuntoVenta").val() == null) {
                    $("#msgError").html("Debe registrar al menos un punto de venta");
                    $("#divError").show();
                    $("#divOk").hide();
                    $('html, body').animate({ scrollTop: 0 }, 'slow');
                    return;
                }

                if ($("#ddlTipo").val() == "NCA" || $("#ddlTipo").val() == "NCB" || $("#ddlTipo").val() == "NCC"
                    || $("#ddlTipo").val() == "NDA" || $("#ddlTipo").val() == "NDB" || $("#ddlTipo").val() == "NDC"
                    || $("#ddlTipo").val() == "NCAMP" || $("#ddlTipo").val() == "NCBMP" || $("#ddlTipo").val() == "NCCMP"
                    || $("#ddlTipo").val() == "NDAMP" || $("#ddlTipo").val() == "NDBMP" || $("#ddlTipo").val() == "NDCMP") {
                    if ($("#ddlComprobanteAsociado").val() == null || $("#ddlComprobanteAsociado").val() == "") {
                        $("#msgError").html("Debe seleccionar un comprobante asociado para los tipo de comprobante Nota de credito.");
                        $("#divError").show();
                        $("#divOk").hide();
                        $('html, body').animate({ scrollTop: 0 }, 'slow');
                        return;
                    }
                }

                if ($("#ddlTipo").val() == "FCAMP" || $("#ddlTipo").val() == "FCBMP" || $("#ddlTipo").val() == "FCCMP"
                    || $("#ddlTipo").val() == "NCAMP" || $("#ddlTipo").val() == "NCBMP" || $("#ddlTipo").val() == "NCCMP"
                    || $("#ddlTipo").val() == "NDAMP" || $("#ddlTipo").val() == "NDBMP" || $("#ddlTipo").val() == "NDCMP") {
                    if($("#ddlModalidadPagoAFIP").val() == null || $("#ddlModalidadPagoAFIP").val() == "") {
                        $("#msgError").html("Debe seleccionar una modalidad de pago AFIP.");
                        $("#divError").show();
                        $("#divOk").hide();
                        $('html, body').animate({ scrollTop: 0 }, 'slow');
                        return;
                    }
                }

                if ($("#ddlTipo").val() == "EDA") {
                    var itemIdPersona = ($("#ddlPersona").val() == "" || $("#ddlPersona").val() == null) ? 0 : parseInt($("#ddlPersona").val());

                    $('#tbItems tr').each(function () {

                        if ($(this).find("td").eq(0).html() != null) {

                            var info = "{ id: " + parseInt($(this).find("td").eq(0).html())
                                + ", idConcepto: '" + $(this).find("td").eq(3).find("input").prop('value')
                                + "', concepto: '" + $(this).find("td").eq(3).find("a").prop('title')
                                + "', iva: '" + $(this).find("td").eq(7).html().replace(".", "").replace(",", ".")
                                + "', precio: '" + $(this).find("td").eq(4).html().replace(".", "").replace(",", ".")
                                + "', bonif: '" + $(this).find("td").eq(5).html().replace(".", "").replace(",", ".")
                                + "', cantidad: '" + $(this).find("td").find("input").prop('value').replace(".", "").replace(",", ".")
                                + "', idPersona: " + itemIdPersona
                                + " , codigo:'" + $(this).find("td").eq(2).html()
                                + "', idPlanDeCuenta:''"
                                + " , codigoCta: ''}";
                            $.ajax({
                                type: "POST",
                                url: "comprobantese.aspx/agregarItemPorCodigoYConcepto",
                                data: info,
                                contentType: "application/json; charset=utf-8",
                                dataType: "json",
                                success: function (data, text) {
                                    verificarPlanDeCuentas();
                                    obtenerTotales();
                                },
                                error: function (response) {
                                    var r = jQuery.parseJSON(response.responseText);
                                    $("#msgErrorDetalle").html(r.Message);
                                    $("#divErrorDetalle").show();
                                }
                            });
                        }

                    });
                }

                if ($('#frmEdicion').valid()) {
                    if ($("#ddlTipo").val() != "COT" && $("#ddlModo").val() == "O") {
                        $("#msgError").html("El modo del comprobante es inválido para un " + $("#ddlTipo option:selected").text());
                        $("#divError").show();
                        $("#divOk").hide();
                        $('html, body').animate({ scrollTop: 0 }, 'slow');
                    }
                    else {
                        Common.mostrarProcesando("lnkAceptar");
                        Common.mostrarProcesando("lnkGenerarCAE");

                        var idJurisdiccion = ($("#ddlJuresdiccion").val() != null && $("#ddlJuresdiccion").val() != "") ? parseInt($("#ddlJuresdiccion").val()) : 0;
                        var idComprobanteAsociado = ($("#ddlComprobanteAsociado").val() != null && $("#ddlComprobanteAsociado").val() != "") ? parseInt($("#ddlComprobanteAsociado").val()) : 0;
                        var idComprobanteOrigen = ($("#ddlComprobantePersona").val() != null && $("#ddlComprobantePersona").val() != "") ? parseInt($("#ddlComprobantePersona").val()) : 0;
                        var idDomicilio = ($("#ddlDomicilio").val() != null && $("#ddlDomicilio").val() != "") ? parseInt($("#ddlDomicilio").val()) : 0;
                        var idTransporte = ($("#ddlTransporte").val() != null && $("#ddlTransporte").val() != "") ? parseInt($("#ddlTransporte").val()) : 0;
                        var idTransportePersona = ($("#ddlTransportePersona").val() != null && $("#ddlTransportePersona").val() != "") ? parseInt($("#ddlTransportePersona").val()) : 0;
                        var idVendedorComision = ($("#ddlVendedorComision").val() != null && $("#ddlVendedorComision").val() != "") ? parseInt($("#ddlVendedorComision").val()) : 0;

                        var info = "{id: " + parseInt($("#hdnID").val())
                            + " , idPersona: " + parseInt($("#ddlPersona").val())
                            + " , tipo: '" + $("#ddlTipo").val()
                            + "', modo: '" + $("#ddlModo").val()
                            + "', fecha: '" + $("#txtFecha").val()
                            + "', condicionVenta: '" + $("#ddlCondicionVenta").val()
                            + "', tipoConcepto: " + parseInt($("#ddlProducto").val())
                            + " , fechaVencimiento: '" + $("#txtFechaVencimiento").val()
                            + "', idPuntoVenta: " + $("#ddlPuntoVenta").val()
                            + " , nroComprobante: '" + $("#txtNumero").val()
                            + "', obs: '" + $("#txtObservaciones").val()
                            + "', idJurisdiccion: " + idJurisdiccion
                            + " , nombre: '" + $("#txtNombre").val()
                            + "', vendedor: '" + $("#txtVendedor").val()
                            + "', envio: '" + $("#ddlEnvio").val()
                            + "', fechaEntrega: '" + $("#txtFechaEntrega").val()
                            + "', fechaAlta: '" + $("#txtFechaAlta").val()
                            + "', idComprobanteAsociado: " + idComprobanteAsociado
                            + ", idComprobanteOrigen: " + idComprobanteOrigen
                            + ", idDomicilio: " + idDomicilio
                            + ", idTransporte: " + idTransporte
                            + ", idTransportePersona: " + idTransportePersona
                            + ", idVendedorComision: " + idVendedorComision
                            + ", estado: '" + $("#ddlEstado").val()
                            + "', idCompraVinculada: " + parseInt($("#hdnIdCompraVinculada").val())
                            + ", idActividad: " + $("#ddlActividad").val()
                            + ", modalidadPagoAfip: '" + $("#ddlModalidadPagoAFIP").val()
                            + "'}";

                        $.ajax({
                            type: "POST",
                            url: "comprobantese.aspx/guardar",
                            data: info,
                            contentType: "application/json; charset=utf-8",
                            dataType: "json",
                            success: function (data, text) {

                                $("#hdnID").val(data.d);

                                var idComprobanteOrigen = ($("#ddlComprobantePersona").val() != null && $("#ddlComprobantePersona").val() != "") ? parseInt($("#ddlComprobantePersona").val()) : 0;
                                var info = "{ id: " + idComprobanteOrigen + "}";
                                var info2 = "{ id: " + $("#hdnID").val() + ", idComprobanteOrigen: " + idComprobanteOrigen + "}";
                                var superaSuma = false;

                                $.ajax({
                                    type: "POST",
                                    url: "comprobantese.aspx/sumatoriaDeFacturasDeUnPedido",
                                    data: info,
                                    contentType: "application/json; charset=utf-8",
                                    dataType: "json",
                                    success: function (data, text) {
                                        superaSuma = data.d;
                                        $.ajax({
                                            type: "POST",
                                            url: "comprobantese.aspx/vincularComprobanteEdaFac",
                                            data: info2,
                                            contentType: "application/json; charset=utf-8",
                                            dataType: "json",
                                            success: function (data, text) {
                                                var mensaje = "Entrega de Articulos copiado a factura generado exitosamente.";
                                                //if (superaSuma) {
                                                //    mensaje = mensaje + " <strong>¡Atencion! La sumatoria de las facturas vinculadas al pedido es superior al total del mismo.</strong>";
                                                //}
                                                $('#msgOk' + frame).html(mensaje);
                                                $("#divOk" + frame).show();
                                                $('html, body').animate({ scrollTop: 0 }, 'slow');
                                                setTimeout(function () {
                                                    window.location.href = "/comprobantese.aspx?tipo=FAC&ID=" + data.d;
                                                }, 3000);
                                            },
                                            error: function (response) {
                                                var r = jQuery.parseJSON(response.responseText);
                                                $('#msgError' + frame).html(r.Message);
                                                $('#divError' + frame).show();
                                                $('#divOk' + frame).hide();
                                                $('html, body').animate({ scrollTop: 0 }, 'slow');
                                            }
                                        });
                                    },
                                    error: function (response) {
                                        var r = jQuery.parseJSON(response.responseText);
                                        $('#msgError' + frame).html(r.Message);
                                        $('#divError' + frame).show();
                                        $('#divOk' + frame).hide();
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
                                Common.ocultarProcesando("lnkAceptar", "Aceptar");
                                Common.ocultarProcesando("lnkGenerarCAE", "Emitir factura eléctronica");
                            }
                        });
                    }
                }
                else {
                    return false;
                }
            }
        });
    } else {

        var idComprobanteOrigen = ($("#ddlComprobantePersona").val() != null && $("#ddlComprobantePersona").val() != "") ? parseInt($("#ddlComprobantePersona").val()) : 0;
        var info = "{ id: " + idComprobanteOrigen + "}";
        var info2 = "{ id: " + $("#hdnID").val() + ", idComprobanteOrigen: " + idComprobanteOrigen + "}";
        var superaSuma = false;

        $.ajax({
            type: "POST",
            url: "comprobantese.aspx/sumatoriaDeFacturasDeUnPedido",
            data: info,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data, text) {
                superaSuma = data.d;
                $.ajax({
                    type: "POST",
                    url: "comprobantese.aspx/vincularComprobanteEdaFac",
                    data: info2,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data, text) {
                        var mensaje = "Entrega de Articulos copiado a factura generado exitosamente.";
                        //if (superaSuma) {
                        //    mensaje = mensaje + " <strong>¡Atencion! La sumatoria de las facturas vinculadas al pedido es superior al total del mismo.</strong>";
                        //}
                        $('#msgOk' + frame).html(mensaje);
                        $("#divOk" + frame).show();
                        $('html, body').animate({ scrollTop: 0 }, 'slow');
                        setTimeout(function () {
                            window.location.href = "/comprobantese.aspx?tipo=FAC&ID=" + data.d;
                        }, 3000);
                    },
                    error: function (response) {
                        var r = jQuery.parseJSON(response.responseText);
                        $('#msgError' + frame).html(r.Message);
                        $('#divError' + frame).show();
                        $('#divOk' + frame).hide();
                        $('html, body').animate({ scrollTop: 0 }, 'slow');
                    }
                });
            },
            error: function (response) {
                var r = jQuery.parseJSON(response.responseText);
                $('#msgError' + frame).html(r.Message);
                $('#divError' + frame).show();
                $('#divOk' + frame).hide();
                $('html, body').animate({ scrollTop: 0 }, 'slow');
            }
        });


    }




}

function vincularCompra(frame) {
    var info = "{ id: " + $("#hdnID").val() + "}";

    $.ajax({
        type: "POST",
        url: "comprase.aspx/vincularCompra",
        data: info,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data, text) {
            window.location.href = "/comprase.aspx?ID=" + data.d;
        },
        error: function (response) {
            var r = jQuery.parseJSON(response.responseText);
            $('#msgError' + frame).html(r.Message);
            $('#divError' + frame).show();
            $('#divOk' + frame).hide();
            $('html, body').animate({ scrollTop: 0 }, 'slow');
        }
    });
}

function JuntarComprobantes() {

    var info = "{idPersona: " + parseInt($("#ddlPersona").val()) + "}";

    $("#divErrorJuntarComprobantes").hide();
    $('#modalJuntarComprobantes').modal('hide');

    $("#resultsContainerJuntarComprobantes").html("");

    $.ajax({
        type: "POST",
        url: "comprobantese.aspx/getResultsJuntarComprobantes",
        contentType: "application/json; charset=utf-8",
        data: info,
        dataType: "json",
        success: function (data, text) {
            $('#modalJuntarComprobantes').modal('show');
            // Render using the template
            if (data.d.Items.length > 0)
                $("#resultTemplateJuntarComprobantes").tmpl({ results: data.d.Items }).appendTo("#resultsContainerJuntarComprobantes");
            else
                $("#noResultTemplateJuntarComprobantes").tmpl({ results: data.d.Items }).appendTo("#resultsContainerJuntarComprobantes");
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

function DesvincularComprobantes() {
    var info = "{ id: " + $("#hdnID").val() + "}";

    $.ajax({
        type: "POST",
        url: "comprobantese.aspx/desvincularComprobante",
        data: info,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data, text) {
            window.location.href = "/comprobantes.aspx?tipo=FAC";
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


function grabarJuntarComprobantes() {

    $("#divErrorJuntarComprobantes").hide();

    var Contador = 0;
    var idSeleccionados = [];
    $('#tablaJuntarComprobantes tr').each(function () {
        if ($(this).find("td").eq(0).find("input").prop('checked')) {
            Contador = + 1;
            var idComprobante = $(this).find("td").eq(0).find("input").prop('value');
            idSeleccionados = idSeleccionados.concat(idComprobante);
        }
    });
    if (Contador > 0) {
        $.ajax({
            type: "POST",
            url: "comprobantese.aspx/juntarComprobantes",
            data: "{ id: " + JSON.stringify(idSeleccionados) + ",idPersona: " + parseInt($("#ddlPersona").val()) + "}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data, text) {
                $('#modalJuntarComprobantes').modal('hide');
                $("#txtConcepto, #txtCantidad, #txtPrecio, #txtBonificacion").val("");
                $("#txtConcepto").attr("disabled", false);
                $("#ddlProductos").val("").trigger("change");
                $("#hdnIDItem").val("0");
                $("#btnAgregarItem").html("Agregar");
                $("#txtCantidad").focus();
                //if ($("#hdnIDUsuario").val() != "5188" && $("#hdnIDUsuario").val() != "5190"
                //    && $("#hdnIDUsuario").val() != "5248" && $("#hdnIDUsuario").val() != "3155" && $("#hdnIDUsuario").val() != "5339") { //FIOL
                //    $("#txtCantidad").numericInput();
                //    $("#txtCantidad").val("1");
                //} else {
                //    $("#txtCantidad").maskMoney({ thousands: '', decimal: ',', allowZero: true });
                //    $("#txtCantidad").val("1,00");
                //}
                if ($("#hdnUsaCantidadConDecimales").val() == "1") {
                    $("#txtCantidad").maskMoney({ thousands: '', decimal: ',', allowZero: true });
                    $("#txtCantidad").val("1,00");
                } else {
                    $("#txtCantidad").numericInput();
                    $("#txtCantidad").val("1");
                }

                if ($("#hdnUsaPlanCorporativo").val() == "1") {
                    $("#ddlPlanDeCuentas").val("").trigger("change");
                }
                verificarPlanDeCuentas();
                obtenerItems();
                obtenerTotales();
            },
            error: function (response) {
                var r = jQuery.parseJSON(response.responseText);
                $("#msgErrorJuntarComprobantes").html(r.Message);
                $("#divErrorJuntarComprobantes").show();
            }
        });
    } else {      
        $("#msgErrorJuntarComprobantes").html("No ha seleccionado ningun registro.");
        $("#divErrorJuntarComprobantes").show();
    }

}


function ActualizarPrecios() {

    var info = "{ id: " + $("#hdnID").val() + "}";

    $.ajax({
        type: "POST",
        url: "comprobantese.aspx/actualizarPrecios",
        data: info,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data, text) {
            $("#msgOk").html("Precios de los items actualizados correctamente.");
            $("#divOk").show();
            $('html, body').animate({ scrollTop: 0 }, 'slow');
            setTimeout(function () {
                window.location.href = "/comprobantese.aspx?tipo=" + $("#hdnTipo").val() + "&ID=" + $("#hdnID").val();
            }, 3000);             
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

function ImprimirLiquidoProducto(tipoImpresion) {

    const dateObj = new Date();
    var mes = dateObj.getMonth() + 1;
    const month = String(mes).padStart(2, '0');
    const day = String(dateObj.getDate()).padStart(2, '0');
    const year = dateObj.getFullYear();
    const output = year + '-' + month + '-' + day;

    bootbox.prompt({
        title: "Ingrese una fecha de entrega",
        inputType: 'date',
        value: output,
        buttons: {
            confirm: {
                label: 'Aceptar'
            },
            cancel: {
                label: 'Omitir'
            }
        },
        callback: function (result) {

            var info = "{ id: " + $("#hdnID").val() + ", tipoImpresion: '" + tipoImpresion + "', fechaEntrega: '" + result + "'}";

            $.ajax({
                type: "POST",
                url: "liquidoProducto.aspx/imprimir",
                data: info,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data, text) {
                    //window.location.href = "/pdfGenerator.ashx?file=" + data.d + "&tipoDeArchivo=liquidoProducto";
                    //window.open(
                    //    "/pdfGenerator.ashx?file=" + data.d + "&tipoDeArchivo=liquidoProducto",
                    //    '_blank' // <- This is what makes it open in a new window.
                    //);
                    $("#divError").hide();
                    //$("#lnkDownloadPdfTicket").attr("href", "/pdfGenerator.ashx?file=" + data.d + "&tipoDeArchivo=liquidoProducto");
                    var pathFile = "/files/liquidoProducto/" + $("#hdnIDUsuario").val() + "/" + data.d + "#view=FitH,top";
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
    }); 

}

function mostrarMasFiltros() {
    $("#divErrorMasFiltros").hide();
    $('#modalMasFiltros').modal('show');
}

function documentosRaiz() {

    var tipoComprobante = getParameterByName('tipo'); 

    $("#divErrorDocumentosRaiz").hide();
    $('#modalDocumentosRaiz').modal('hide');

    $("#resultsContainerDocumentosRaiz").html("");

    $.ajax({
        type: "POST",
        url: "comprobantes.aspx/getResultsDocumentosRaiz",
        data: "{ tipo: '" + tipoComprobante + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data, text) {
            $('#modalDocumentosRaiz').modal('show');
            // Render using the template
            if (data.d.Items.length > 0) {
                $("#resultTemplateDocumentosRaiz").tmpl({ results: data.d.Items }).appendTo("#resultsContainerDocumentosRaiz");
                var total = 0.0;
                for (var i = 0; i < data.d.Items.length; i++) {
                    total = total + parseFloat(data.d.Items[i].ImporteTotalNeto.replace('.','').replace(',','.'));
                }
                $("#msgTotalDocumentosRaiz").html("Total: " + number_format(total,2));
            }
            else {
                $("#noResultTemplateDocumentosRaiz").tmpl({ results: data.d.Items }).appendTo("#resultsContainerDocumentosRaiz");
            }
                
        },
        error: function (response) {
            var r = jQuery.parseJSON(response.responseText);
            $("#msgErrorDocumentosRaiz").html(r.Message);
            $("#divErrorDocumentosRaiz").show();
            $('html, body').animate({ scrollTop: 0 }, 'slow');
        }
    });

  
}

function number_format(amount, decimals) {
    amount += ''; // por si pasan un numero en vez de un string
    amount = parseFloat(amount.replace(/[^0-9\.]/g, '')); // elimino cualquier cosa que no sea numero o punto
    decimals = decimals || 0; // por si la variable no fue fue pasada
    // si no es un numero o es igual a cero retorno el mismo cero
    if (isNaN(amount) || amount === 0)
        return parseFloat(0).toFixed(decimals);
    // si es mayor o menor que cero retorno el valor formateado como numero
    amount = '' + amount.toFixed(decimals);
    var amount_parts = amount.split('.'),
        regexp = /(\d+)(\d{3})/;
    while (regexp.test(amount_parts[0]))
        amount_parts[0] = amount_parts[0].replace(regexp, '$1' + '.' + '$2');
    return amount_parts.join(',');
}

function exportarDocumentosRaiz() {
    resetearExportacionDocumentosRaiz();

    $("#imgLoadingDocumentosRaiz").show();
    $("#divIconoDescargarDocumentosRaiz").hide();
    var tipoComprobante = getParameterByName('tipo');

    $.ajax({
        type: "POST",
        url: "comprobantes.aspx/exportDocumentosRaiz",
        data: "{ tipo: '" + tipoComprobante + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data, text) {
            if (data.d != "") {

                $("#divErrorDocumentosRaiz").hide();
                $("#imgLoadingDocumentosRaiz").hide();
                $("#lnkDownloadDocumentosRaiz").show();
                $("#lnkDownloadDocumentosRaiz").attr("href", data.d);
                $("#lnkDownloadDocumentosRaiz").attr("download", data.d);
            }
        },
        error: function (response) {
            var r = jQuery.parseJSON(response.responseText);
            $("#msgErrorDocumentosRaiz").html(r.Message);
            $("#divErrorDocumentosRaiz").show();
            $('html, body').animate({ scrollTop: 0 }, 'slow');
            resetearExportacionDocumentosRaiz();
        }
    });
}

function resetearExportacionDocumentosRaiz() {
    $("#imgLoadingDocumentosRaiz, #lnkDownloadDocumentosRaiz").hide();
    $("#divIconoDescargarDocumentosRaiz").show();
}

function enviarCorreoElectronico(idComprobante) {
    $.blockUI({ message: $('#divEspera') });
    $.ajax({
        type: "POST",
        url: "facturaAutomatica.aspx/enviarFacturaPorCorreoElectronico",
        data: "{idComprobante:" + idComprobante + "}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data, text) {
            $.unblockUI();
            document.body.style.cursor = 'default';
            if (data != null) {
                $("#msgOk").html("Comprobante enviado por correo electrónico correctamente.");
                $('#divOk').show();
                $("#divError").hide();
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
        }
    });
}


function cuadroResumen(modal) {

    $("#divErrorCuadroResumen").hide();
    $("#divOkCuadroResumen").hide();

    if (event.shiftKey && event.ctrlKey) {
        $.blockUI({ message: $('#divEspera') });
        document.body.style.cursor = 'wait';

        var periodo = ($("#ddlCuadroResumenPeriodo").val() != null && $("#ddlCuadroResumenPeriodo").val() != "") ? $("#ddlCuadroResumenPeriodo").val() : getYYMMCurrent();
        $("#ddlCuadroResumenPeriodo").val(periodo);

        $.ajax({
            type: "POST",
            url: "comprobantes.aspx/obtenerCuadroResumen",
            data: "{periodo: '" + periodo + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                $.unblockUI();
                document.body.style.cursor = 'default';
                if (data != null) {
                    $("#hdnCuadroResumenPeriodo").val(periodo);
                    $("#resultsCuadroResumen").html(data.d);
                    if (modal)
                        $('#modalCuadroResumen').modal('show');
                }
            },
            error: function (response) {
                $.unblockUI();
                document.body.style.cursor = 'default';
                var r = jQuery.parseJSON(response.responseText);
                $("#msgErrorCuadroResumen").html(r.Message);
                $("#divErrorCuadroResumen").show();
                $("#divOkCuadroResumen").hide();
            }
        });
    } else {
        $("#msgError").html("No Disponible");
        $("#divError").show();
        $("#divOk").hide();
    }
}


function cuadroResumenPeriodo(modal) {

    $.blockUI({ message: $('#divEspera') });
    document.body.style.cursor = 'wait';

    var periodo = ($("#ddlCuadroResumenPeriodo").val() != null && $("#ddlCuadroResumenPeriodo").val() != "") ? $("#ddlCuadroResumenPeriodo").val() : getYYMMCurrent();
    $("#ddlCuadroResumenPeriodo").val(periodo);

    $.ajax({
        type: "POST",
        url: "comprobantes.aspx/obtenerCuadroResumen",
        data: "{periodo: '" + periodo + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            $.unblockUI();
            document.body.style.cursor = 'default';
            if (data != null) {
                $("#hdnCuadroResumenPeriodo").val(periodo);
                $("#resultsCuadroResumen").html(data.d);
                if (modal)
                    $('#modalCuadroResumen').modal('show');
            }
        },
        error: function (response) {
            $.unblockUI();
            document.body.style.cursor = 'default';
            var r = jQuery.parseJSON(response.responseText);
            $("#msgErrorCuadroResumen").html(r.Message);
            $("#divErrorCuadroResumen").show();
            $("#divOkCuadroResumen").hide();
        }
    });

}

function exportarCuadroResumen() {
    $.blockUI({ message: $('#divEspera') });
    document.body.style.cursor = 'wait';
    resetearExportacionCuadroResumen();

    $("#imgLoadingCuadroResumen").show();
    $("#divIconoDescargarCuadroResumen").hide();

    var periodo = ($("#ddlCuadroResumenPeriodo").val() != null && $("#ddlCuadroResumenPeriodo").val() != "") ? $("#ddlCuadroResumenPeriodo").val() : getYYMMCurrent();
    $("#ddlCuadroResumenPeriodo").val(periodo);

    var info = "{periodo: '" + periodo + "'}";

    $.ajax({
        type: "POST",
        url: "comprobantes.aspx/exportarCuadroResumen",
        data: info,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data, text) {
            $.unblockUI();
            document.body.style.cursor = 'default';
            if (data.d != "") {
                $("#divError").hide();
                $("#imgLoadingCuadroResumen").hide();
                $("#lnkCuadroResumenDownload").show();
                $("#lnkCuadroResumenDownload").attr("href", data.d);
                $("#lnkCuadroResumenDownload").attr("download", data.d);
            }
        },
        error: function (response) {
            $.unblockUI();
            document.body.style.cursor = 'default';
            var r = jQuery.parseJSON(response.responseText);
            $("#msgErrorCuadroResumen").html(r.Message);
            $("#divErrorCuadroResumen").show();
            $("#divOkCuadroResumen").hide();
        }
    });
}

function exportarCuadroResumenOriginal() {
    $.blockUI({ message: $('#divEspera') });
    document.body.style.cursor = 'wait';
    resetearExportacionCuadroResumenOriginal();

    $("#imgLoadingCuadroResumenOriginal").show();
    $("#divIconoDescargarCuadroResumenOriginal").hide();

    var periodo = ($("#ddlCuadroResumenPeriodo").val() != null && $("#ddlCuadroResumenPeriodo").val() != "") ? $("#ddlCuadroResumenPeriodo").val() : getYYMMCurrent();
    $("#ddlCuadroResumenPeriodo").val(periodo);

    var info = "{periodo: '" + periodo + "'}";

    $.ajax({
        type: "POST",
        url: "comprobantes.aspx/exportarCuadroResumenOriginal",
        data: info,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data, text) {
            $.unblockUI();
            document.body.style.cursor = 'default';
            if (data.d != "") {
                $("#divError").hide();
                $("#imgLoadingCuadroResumenOriginal").hide();
                $("#lnkCuadroResumenDownloadOriginal").show();
                $("#lnkCuadroResumenDownloadOriginal").attr("href", data.d);
                $("#lnkCuadroResumenDownloadOriginal").attr("download", data.d);
            }
        },
        error: function (response) {
            $.unblockUI();
            document.body.style.cursor = 'default';
            var r = jQuery.parseJSON(response.responseText);
            $("#msgErrorCuadroResumen").html(r.Message);
            $("#divErrorCuadroResumen").show();
            $("#divOkCuadroResumen").hide();
        }
    });
}

function resetearExportacionCuadroResumen() {
    $("#imgLoadingCuadroResumen, #lnkCuadroResumenDownload").hide();
    $("#divIconoDescargarCuadroResumen").show();
}

function resetearExportacionCuadroResumenOriginal() {
    $("#imgLoadingCuadroResumenOriginal, #lnkCuadroResumenDownloadOriginal").hide();
    $("#divIconoDescargarCuadroResumenOriginal").show();
}

function reiniciarFiltros() {
    setTimeout(function () {
        window.location.href = "/comprobantes.aspx?tipo=" + $("#hdnTipo").val();
    }, 1000);
}

function getYYMMCurrent() {
    const currentDate = new Date();
    const year = currentDate.getFullYear().toString().slice(-2);
    const month = String(currentDate.getMonth() + 1).padStart(2, '0');
    const yyMM = year + month;
    return yyMM;
}