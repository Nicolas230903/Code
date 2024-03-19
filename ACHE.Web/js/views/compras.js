var dataJurisdicciones = new Array();
var Compras = {
    /*** FORM ***/
    configForm: function () {
        
        $(".select2").select2({
            width: '100%', allowClear: true,
            formatNoMatches: function (term) {
                return "<a style='cursor:pointer' onclick=\"$('#modalNuevoCliente').modal('show');$('.select2').select2('close');\">+ Agregar</a>";
            }
        });

        $(".select3").select2({ width: '100%', allowClear: true, });
        $(".select4").select2({ width: '100%', allowClear: true, });
        $('#txtFechaPrimerVencimiento,#txtFechaSegundoVencimiento').datepicker();
        Common.configDatePicker();
        Common.configFechasDesdeHasta("txtFechaEmision", "txtFechaPrimerVencimiento");

        //$("#txtNroDocumento, #txtImporteMon, #txtIva, #txtIIBB, #txtImpNacionales, #txtImpMunicipales, #txtImpInternos, #txtPercepcionIVA,#txtOtros").numericInput();
        //$("#txtImporte2, #txtImporte5, #txtImporte10, #txtImporte21, #txtImporte27, #txtNoGravado,#txtExento, #txtImporteMon,#txtImporteJurisdiccion").numericInput();

        $("#txtIva, #txtImpNacionales, #txtImpMunicipales").maskMoney({ thousands: '', decimal: ',', allowZero: true });//, #txtRedondeo 
        $("#txtIIBB, #txtImpInternos, #txtPercepcionIVA, #txtOtros").maskMoney({ thousands: '', decimal: ',', allowZero: true });//, #txtRedondeo
        $("#txtImporte2, #txtImporte5, #txtImporte10, #txtImporte21").maskMoney({ thousands: '', decimal: ',', allowZero: true });//, #txtRedondeo
        $("#txtImporte27, #txtNoGravado, #txtExento, #txtImporteMon, #txtImporteJurisdiccion").maskMoney({ thousands: '', decimal: ',', allowZero: true });


        $("#txtIva").val($("#txtIva").val().replace(".", ","));
        $("#txtImpNacionales").val($("#txtImpNacionales").val().replace(".", ","));
        $("#txtImpMunicipales").val($("#txtImpMunicipales").val().replace(".", ","));
        $("#txtIIBB").val($("#txtIIBB").val().replace(".", ","));
        $("#txtImpInternos").val($("#txtImpInternos").val().replace(".", ","));
        $("#txtPercepcionIVA").val($("#txtPercepcionIVA").val().replace(".", ","));
        $("#txtOtros").val($("#txtOtros").val().replace(".", ","));
        $("#txtImporte2").val($("#txtImporte2").val().replace(".", ","));
        $("#txtImporte5").val($("#txtImporte5").val().replace(".", ","));
        $("#txtImporte10").val($("#txtImporte10").val().replace(".", ","));
        $("#txtImporte21").val($("#txtImporte21").val().replace(".", ","));
        $("#txtImporte27").val($("#txtImporte27").val().replace(".", ","));
        $("#txtNoGravado").val($("#txtNoGravado").val().replace(".", ","));
        $("#txtExento").val($("#txtExento").val().replace(".", ","));
        $("#txtImporteMon").val($("#txtImporteMon").val().replace(".", ","));
        $("#txtImporteJurisdiccion").val($("#txtImporteJurisdiccion").val().replace(".", ","));

        $("#txtNroFactura").mask("?9999-99999999");
        $("#txtNroFactura").blur(function () {
            var pto = $("#txtNroFactura").val().split("-")[0];
            var nro = $("#txtNroFactura").val().split("-")[1];
            $("#txtNroFactura").val(pto + "-" + padZeros(nro, 8));
        });

        $("#txtIva, #txtImporte2, #txtImporte5, #txtImporte10, #txtImporte21, #txtImporte27, #txtNoGravado,#txtExento, #txtImpNacionales, #txtImpMunicipales, #txtImpInternos, #txtIIBB,#txtPercepcionIVA,#txtOtros, #txtImporteMon").blur(function () {//, #txtRedondeo
            Compras.changeImportes();
        });

        $("#ddlTipo").attr("onchange", "Compras.ocultarIva();Compras.changeComprobante();");

        //if ($("#txtImporteMon").val() != "") {
        //    var importeMon = parseFloat($("#txtImporteMon").val());
        //    $("#txtImporteMon").val(addSeparatorsNF(importeMon.toFixed(2), '.', ',', '.'));
        //}

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

        $("#frmEdicionJurisdiccion").validate({
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

        Common.obtenerPersonas("ddlPersona", $("#hdnIDPersona").val(), true);
        $("#txtImporte").change();//Obtengo el total


        var id = ($("#hdnID").val() == "" ? "0" : $("#hdnID").val());
        if (parseInt(id) == 0) {
            Compras.ocultarIva();
            Compras.changeComprobante();
        }
        else {
            if ($("#hdnFileName").val() != "") {
                $(".fileinput-filename").html($("#hdnFileName").val());
                $("#iImgFileFoto").show();
            }
            Compras.obtenerJurisdicciones(1);
            Compras.obtenerDetalleDelComprobante();
        }

        //obtener las del USUARIO
        Compras.obtenerJurisdiccionUsuario();
        Compras.parsearNumeros();
        Compras.verificarPlanDeCuentas();
        Compras.adjuntarFoto();
        Compras.changeImportes();        
    },
    changeImportes: function () {
        var total = Compras.getTotal();
        $("#divTotal").html("$ " + addSeparatorsNF(total.toFixed(2), '.', ',', '.'));

        var totalImportes = Compras.getImportes();
        $("#divTotalImportes").html("$ " + addSeparatorsNF(totalImportes.toFixed(2), '.', ',', '.'));

        var totalImpuestos = Compras.getImpuestos();
        $("#divTotalImpuestos").html("$ " + addSeparatorsNF(totalImpuestos.toFixed(2), '.', ',', '.'));
    },
    toggleRetenciones: function () {
        $("#divRetenciones").slideToggle();
    },
    changePersona: function () {
        if ($("#ddlPersona").val() != "") {
            $.ajax({
                type: "POST",
                url: "personase.aspx/obtenerDatos",
                data: "{ id: " + parseInt($("#ddlPersona").val()) + "}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data, text) {
                    if (data.d != null) {
                        $("#ddlTipo").html("<option value=''></option>");
                        Common.obtenerComprobantesPagoPorCondicion("ddlTipo", data.d.CondicionIva, $("#hdnTipoComprobante").val());
                        //$("#hdnCondicion").val(data.d.CondicionIva);
                        Compras.ocultarIva();
                        Compras.changeComprobante();

                        if (parseInt($("#hdnID").val()) == 0) {
                            Compras.ObtenerUltimoTipoComprobanteCliente();
                            Compras.obtenerUltimoRubroComprobanteCliente();
                            Compras.obtenerUltimaCategoriaComprobanteCliente();
                        }
                    }
                },
                error: function (response) {
                    var r = jQuery.parseJSON(response.responseText);
                    $("#msgError").html(r.Message);
                    $("#divError").show();
                }
            });
        }
        else {
            //$("#divFactura").hide();
            $("<option/>").attr("value", "").text("Seleccione un cliente/proveedor").appendTo($("#ddlTipo"));
        }
    },
    ObtenerUltimoTipoComprobanteCliente: function (idPersona, controlName) {
        var info = "{idPersona: " + $("#ddlPersona").val() + "}";
        $.ajax({
            type: "POST",
            url: "/common.aspx/ObtenerUltimoTipoComprobanteCliente",
            data: info,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                if (data.d != null && data.d != "") {
                    $("#ddlTipo").val(data.d);
                    Compras.ocultarIva();
                    Compras.changeComprobante();
                }
            }
        });
    },
    changeIVA: function () {

        var iva = 0;
        var iva02 = 0;
        var iva05 = 0;
        var iva10 = 0;
        var iva21 = 0;
        var iva27 = 0;

        if ($("#txtImporte2").val() > 0) {
            iva02 = ($("#txtImporte2").val() * 2.5) / 100;
        }

        if ($("#txtImporte5").val() > 0) {
            iva05 = ($("#txtImporte5").val() * 5) / 100;
        }

        if ($("#txtImporte10").val() > 0) {
            iva10 = ($("#txtImporte10").val() * 10.5) / 100;
        }

        if ($("#txtImporte21").val() > 0) {
            iva21 = ($("#txtImporte21").val() * 21) / 100;
        }

        if ($("#txtImporte27").val() > 0) {
            iva27 = ($("#txtImporte27").val() * 27) / 100;
        }

        iva = parseFloat(iva02 + iva05 + iva10 + iva21 + iva27);

        if (iva != 0) {
            $("#txtIva").val(iva.toFixed(2));
        }
        else {
            $("#txtIva").val("0");
        }
    },
    verificarIVA: function () {
        var iva = 0;
        var iva02 = 0;
        var iva05 = 0;
        var iva10 = 0;
        var iva21 = 0;
        var iva27 = 0;
        var IVAOK = false;
        var ivaMAS = 0;
        var IVAMenos = 0;
        var IVAActual = 0;
        if ($("#txtImporte2").val() > 0) {
            iva02 = ($("#txtImporte2").val() * 2.5) / 100;
        }

        if ($("#txtImporte5").val() > 0) {
            iva05 = ($("#txtImporte5").val() * 5) / 100;
        }

        if ($("#txtImporte10").val() > 0) {
            iva10 = ($("#txtImporte10").val() * 10.5) / 100;
        }

        if ($("#txtImporte21").val() > 0) {
            iva21 = ($("#txtImporte21").val() * 21) / 100;
        }

        if ($("#txtImporte27").val() > 0) {
            iva27 = ($("#txtImporte27").val() * 27) / 100;
        }

        iva = parseFloat(iva02 + iva05 + iva10 + iva21 + iva27);
        iva = iva.toFixed(2);
        ivaMAS = parseFloat(iva) + 1.00;
        IVAMenos = parseFloat(iva) - 1.00;
        IVAActual = parseFloat($("#txtIva").val());

        if (parseFloat(iva) == 0) {
            if (IVAActual == 0) {
                IVAOK = true;
            }
        }
        else {
            if (IVAActual >= IVAMenos && IVAActual <= ivaMAS) {
                IVAOK = true;
            }
        }


        //if (!IVAOK) {
        //    $("#msgError").html("El IVA solo puede modificar + o - $1");
        //    $("#divError").show();
        //    $("#txtIva").closest('.form-group').removeClass('has-success').addClass("has-error")
        //}
        //else {
        //    $("#msgError").html("");
        //    $("#divError").hide();
        //    $("#txtIva").closest('.form-group').removeClass('has-error')
        //}
    },
    irDetalleDelComprobante: function () {
        if ($("#ddlDetalleDelComprobante").val() == null) {
            $("#msgError").html("Debe seleccionar un detalle del comprobante");
            $("#divError").show();
            $("#divOk").hide();
            $('html, body').animate({ scrollTop: 0 }, 'slow');
            return;
        }

        window.location.href = "/comprobantese.aspx?tipo=DDC&ID=" + $("#ddlDetalleDelComprobante").val();
        //var pageLoad = ($("#hdnPageLoad").val() == "" ? "0" : $("#hdnPageLoad").val());
        //if (pageLoad != "1") {
        //}
    },
    grabar: function () {
        Compras.ocultarMensajes();

        if ($("#hdnEsAgenteRetencionGanancia").val() == "1") {
            if ($("#ddlRubro").val() == "") {
                $("#msgError").html("El agente es retención de ganancias, campo RUBRO obligatorio.");
                $("#divError").show();
                $("#divOk").hide();
                $('html, body').animate({ scrollTop: 0 }, 'slow');
                return false;
            } 
        }

        var id = ($("#hdnID").val() == "" ? "0" : $("#hdnID").val());
        var idPlanDeCuenta = (($("#ddlPlanDeCuentas").val() == "" || $("#ddlPlanDeCuentas").val() == null) ? "0" : $("#ddlPlanDeCuentas").val());
        if ($('#frmEdicion').valid()) {
            Common.mostrarProcesando("btnActualizar");


            var total = Compras.getTotal();            
            if (total > 0) {
                var info = "{ id: " + parseInt(id)
                        + ", idPersona: " + $("#ddlPersona").val()
                        + ", fecha: '" + $("#txtFecha").val()
                        + "', nroFactura: '" + $("#txtNroFactura").val()
                        + "', iva: '" + $("#txtIva").val()
                        + "', importe2: '" + $("#txtImporte2").val()
                        + "', importe5: '" + $("#txtImporte5").val()
                        + "', importe10: '" + $("#txtImporte10").val()
                        + "', importe21: '" + $("#txtImporte21").val()
                        + "', importe27: '" + $("#txtImporte27").val()
                        + "', noGrav: '" + $("#txtNoGravado").val()
                        + "', importeMon: '" + $("#txtImporteMon").val()
                        + "', impNacional: '" + $("#txtImpNacionales").val()
                        + "', impMunicipal: '" + $("#txtImpMunicipales").val()
                        + "', impInterno: '" + $("#txtImpInternos").val()
                        //+ "', iibb: '" + $("#txtIIBB").val()
                        + "', percepcionIva: '" + $("#txtPercepcionIVA").val()
                        + "', otros: '" + $("#txtOtros").val()
                        + "', obs: '" + $("#txtObservaciones").val()
                        + "', tipo: '" + $("#ddlTipo").val()
                        + "', idCategoria: '" + $("#ddlCategoria").val()
                        + "', rubro: '" + $("#ddlRubro").val()
                        + "', exento: '" + $("#txtExento").val()
                        + "', FechaEmision: '" + $("#txtFechaEmision").val()
                        + "', idPlanDeCuenta: " + idPlanDeCuenta
                        + " , Jurisdicciones: " + JSON.stringify(dataJurisdicciones)
                        + " , fechaPrimerVencimiento: '" + $("#txtFechaPrimerVencimiento").val()
                        + "', fechaSegundoVencimiento: '" + $("#txtFechaSegundoVencimiento").val()
                    + "'}";

                $.ajax({
                    type: "POST",
                    url: "comprase.aspx/guardar",
                    data: info,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    async: false,
                    success: function (data, text) {
                        $('#divOk').show();
                        $("#divError").hide();
                        $('html, body').animate({ scrollTop: 0 }, 'slow');

                        $("#hdnID").val(data.d);

                        if ($("#hdnSinCombioDeFoto").val() == "0") {
                            if ($("#hdnTieneSaldoAPagar").val() == "1") {
                                $('#modalPagos').modal('show');
                            } else {
                                window.location.href = "/compras.aspx";
                            }
                        }
                    },
                    error: function (response) {
                        var r = jQuery.parseJSON(response.responseText);
                        $("#msgError").html(r.Message);
                        $("#divError").show();
                        $("#divOk").hide();
                        $('html, body').animate({ scrollTop: 0 }, 'slow');
                        Common.ocultarProcesando("btnActualizar", "Aceptar");
                    }
                });
            }
            else {
                Common.ocultarProcesando("btnActualizar", "Aceptar");
                $("#msgError").html("El importe debe ser mayor a 0");
                $("#divError").show();
                $("#divOk").hide();
                $('html, body').animate({ scrollTop: 0 }, 'slow');
                return false;
            }
        }
        else {
            return false;
        }
    },
    getTotal: function () {
            var total = 0;

        //if ($("#txtIva").val() != "") {
        //    total += parseFloat($("#txtIva").val().replace('.', '').replace(',', '.'));
        //}
        //if ($("#txtImporte2").val() != "") {
        //    total += parseFloat($("#txtImporte2").val().replace('.', '').replace(',', '.'));
        //}
        //if ($("#txtImporte5").val() != "") {
        //    total += parseFloat($("#txtImporte5").val().replace('.', '').replace(',', '.'));
        //}
        //if ($("#txtImporte10").val() != "") {
        //    total += parseFloat($("#txtImporte10").val().replace('.', '').replace(',', '.'));
        //}
        //if ($("#txtImporte21").val() != "") {
        //    total += parseFloat($("#txtImporte21").val().replace('.', '').replace(',', '.'));
        //}
        //if ($("#txtImporte27").val() != "") {
        //    total += parseFloat($("#txtImporte27").val().replace('.', '').replace(',', '.'));
        //}
        //if ($("#txtNoGravado").val() != "") {
        //    total += parseFloat($("#txtNoGravado").val().replace('.', '').replace(',', '.'));
        //}
        //if ($("#txtExento").val() != "") {
        //    total += parseFloat($("#txtExento").val().replace('.', '').replace(',', '.'));
        //}


        //// IMPUESTOS
        //if ($("#txtImpNacionales").val() != "") {
        //    total += parseFloat($("#txtImpNacionales").val().replace('.', '').replace(',', '.'));
        //}
        //if ($("#txtImpMunicipales").val() != "") {
        //    total += parseFloat($("#txtImpMunicipales").val().replace('.', '').replace(',', '.'));
        //}
        //if ($("#txtImpInternos").val() != "") {
        //    total += parseFloat($("#txtImpInternos").val().replace('.', '').replace(',', '.'));
        //}
        //if ($("#txtIIBB").val() != "") {
        //    total += parseFloat($("#txtIIBB").val().replace('.', '').replace(',', '.'));
        //}
        //if ($("#txtPercepcionIVA").val() != "") {
        //    total += parseFloat($("#txtPercepcionIVA").val().replace('.', '').replace(',', '.'));
        //}
        //if ($("#txtOtros").val() != "") {
        //    total += parseFloat($("#txtOtros").val().replace('.', '').replace(',', '.'));
        //}


        //if ($("#txtImporteMon").val() != "") {
        //    total += parseFloat($("#txtImporteMon").val().replace('.', '').replace(',', '.'));
        //}
        ///*if ($("#txtRedondeo").val() != "") {
        //    total += parseFloat($("#txtRedondeo").val());
        //}*/

        if ($("#txtIva").val() != "") {
            total += parseFloat($("#txtIva").val().replace(',', '.'));
        }
        if ($("#txtImporte2").val() != "") {
            total += parseFloat($("#txtImporte2").val().replace(',', '.'));
        }
        if ($("#txtImporte5").val() != "") {
            total += parseFloat($("#txtImporte5").val().replace(',', '.'));
        }
        if ($("#txtImporte10").val() != "") {
            total += parseFloat($("#txtImporte10").val().replace(',', '.'));
        }
        if ($("#txtImporte21").val() != "") {
            total += parseFloat($("#txtImporte21").val().replace(',', '.'));
        }
        if ($("#txtImporte27").val() != "") {
            total += parseFloat($("#txtImporte27").val().replace(',', '.'));
        }
        if ($("#txtNoGravado").val() != "") {
            total += parseFloat($("#txtNoGravado").val().replace(',', '.'));
        }
        if ($("#txtExento").val() != "") {
            total += parseFloat($("#txtExento").val().replace(',', '.'));
        }


        // IMPUESTOS
        if ($("#txtImpNacionales").val() != "") {
            total += parseFloat($("#txtImpNacionales").val().replace(',', '.'));
        }
        if ($("#txtImpMunicipales").val() != "") {
            total += parseFloat($("#txtImpMunicipales").val().replace(',', '.'));
        }
        if ($("#txtImpInternos").val() != "") {
            total += parseFloat($("#txtImpInternos").val().replace(',', '.'));
        }
        if ($("#txtIIBB").val() != "") {
            total += parseFloat($("#txtIIBB").val().replace(',', '.'));
        }
        if ($("#txtPercepcionIVA").val() != "") {
            total += parseFloat($("#txtPercepcionIVA").val().replace(',', '.'));
        }
        if ($("#txtOtros").val() != "") {
            total += parseFloat($("#txtOtros").val().replace(',', '.'));
        }


        if ($("#txtImporteMon").val() != "") {
            total += parseFloat($("#txtImporteMon").val().replace(',', '.'));
        }
        /*if ($("#txtRedondeo").val() != "") {
            total += parseFloat($("#txtRedondeo").val());
        }*/

        return total;
    },
    getImpuestos: function () {

        //var total = 0;
        //if ($("#txtIva").val() != "") {
        //    total += parseFloat($("#txtIva").val().replace('.', '').replace(',', '.'));
        //}
        //// IMPUESTOS
        //if ($("#txtImpNacionales").val() != "") {
        //    total += parseFloat($("#txtImpNacionales").val().replace('.', '').replace(',', '.'));
        //}
        //if ($("#txtImpMunicipales").val() != "") {
        //    total += parseFloat($("#txtImpMunicipales").val().replace('.', '').replace(',', '.'));
        //}
        //if ($("#txtImpInternos").val() != "") {
        //    total += parseFloat($("#txtImpInternos").val().replace('.', '').replace(',', '.'));
        //}
        //if ($("#txtIIBB").val() != "") {
        //    total += parseFloat($("#txtIIBB").val().replace('.', '').replace(',', '.'));
        //}
        //if ($("#txtPercepcionIVA").val() != "") {
        //    total += parseFloat($("#txtPercepcionIVA").val().replace('.', '').replace(',', '.'));
        //}
        //if ($("#txtOtros").val() != "") {
        //    total += parseFloat($("#txtOtros").val().replace('.', '').replace(',', '.'));
        //}
        //return total;

        var total = 0;
        if ($("#txtIva").val() != "") {
            total += parseFloat($("#txtIva").val().replace(',', '.'));
        }
        // IMPUESTOS
        if ($("#txtImpNacionales").val() != "") {
            total += parseFloat($("#txtImpNacionales").val().replace(',', '.'));
        }
        if ($("#txtImpMunicipales").val() != "") {
            total += parseFloat($("#txtImpMunicipales").val().replace(',', '.'));
        }
        if ($("#txtImpInternos").val() != "") {
            total += parseFloat($("#txtImpInternos").val().replace(',', '.'));
        }
        if ($("#txtIIBB").val() != "") {
            total += parseFloat($("#txtIIBB").val().replace(',', '.'));
        }
        if ($("#txtPercepcionIVA").val() != "") {
            total += parseFloat($("#txtPercepcionIVA").val().replace(',', '.'));
        }
        if ($("#txtOtros").val() != "") {
            total += parseFloat($("#txtOtros").val().replace(',', '.'));
        }
        return total;

    },
    getImportes: function () {

        //var total = 0;
        //if ($("#txtImporte2").val() != "") {
        //    total += parseFloat($("#txtImporte2").val().replace('.', '').replace(',', '.'));
        //}
        //if ($("#txtImporte5").val() != "") {
        //    total += parseFloat($("#txtImporte5").val().replace('.', '').replace(',', '.'));
        //}
        //if ($("#txtImporte10").val() != "") {
        //    total += parseFloat($("#txtImporte10").val().replace('.', '').replace(',', '.'));
        //}
        //if ($("#txtImporte21").val() != "") {
        //    total += parseFloat($("#txtImporte21").val().replace('.', '').replace(',', '.'));
        //}
        //if ($("#txtImporte27").val() != "") {
        //    total += parseFloat($("#txtImporte27").val().replace('.', '').replace(',', '.'));
        //}
        //if ($("#txtNoGravado").val() != "") {
        //    total += parseFloat($("#txtNoGravado").val().replace('.', '').replace(',', '.'));
        //}
        //if ($("#txtExento").val() != "") {
        //    total += parseFloat($("#txtExento").val().replace('.', '').replace(',', '.'));
        //}

        //if ($("#txtImporteMon").val() != "") {
        //    total += parseFloat($("#txtImporteMon").val().replace('.', '').replace(',', '.'));
        //}

        //return total;


        var total = 0;
        if ($("#txtImporte2").val() != "") {
            total += parseFloat($("#txtImporte2").val().replace(',', '.'));
        }
        if ($("#txtImporte5").val() != "") {
            total += parseFloat($("#txtImporte5").val().replace(',', '.'));
        }
        if ($("#txtImporte10").val() != "") {
            total += parseFloat($("#txtImporte10").val().replace(',', '.'));
        }
        if ($("#txtImporte21").val() != "") {
            total += parseFloat($("#txtImporte21").val().replace(',', '.'));
        }
        if ($("#txtImporte27").val() != "") {
            total += parseFloat($("#txtImporte27").val().replace(',', '.'));
        }
        if ($("#txtNoGravado").val() != "") {
            total += parseFloat($("#txtNoGravado").val().replace(',', '.'));
        }
        if ($("#txtExento").val() != "") {
            total += parseFloat($("#txtExento").val().replace(',', '.'));
        }

        if ($("#txtImporteMon").val() != "") {
            total += parseFloat($("#txtImporteMon").val().replace(',', '.'));
        }

        return total;

    },
    cancelar: function () {
        window.location.href = "/compras.aspx";
    },
    nuevoDetalleDelComprobante: function () {
        var id = ($("#hdnID").val() == "" ? "0" : $("#hdnID").val());
        if (parseInt(id) == 0) {
            $("#msgError").html("Primero debe generar una compra para crear un detalle del comprobante");
            $("#divError").show();
            $("#divOk").hide();
            $('html, body').animate({ scrollTop: 0 }, 'slow');
            return;
        }
        else {
            window.location.href = "/comprobantese.aspx?tipo=DDC&idCompra=" + $("#hdnID").val();
        }        
    },
    verificarPlanDeCuentas: function () {
        if ($("#hdnUsaPlanCorporativo").val() == "1") {
            $(".divPlanDeCuentas").show();
        }
        else {
            $(".divPlanDeCuentas").hide();
        }
    },
    ocultarIva: function () {
        if (MI_CONDICION == "RI" && ($("#ddlTipo").val() == "FCA" || $("#ddlTipo").val() == "NCA" || $("#ddlTipo").val() == "NDA" || $("#ddlTipo").val() == "RCA")) {
            $(".ivas").show();
            $("#divImporteMon").hide();
            $("#txtImporteMon").val('');
        }
        else {
            $(".ivas").hide();
            $("#divImporteMon").show();
            $("#txtIva,#txtImporte2, #txtImporte5, #txtImporte10, #txtImporte21, #txtImporte27, #txtNoGravado,#txtExento").val('');
        }
    },
    registrarPago: function (pago) {
        //var pago = $("input[name='PagoParcial']:checked").attr('value');
        window.location.href = 'pagose.aspx?IDPersona=' + $("#ddlPersona").val() + "&IDCompra=" + $("#hdnID").val() + "&Pago=" + pago;
    },
    changeComprobante: function () {
        if ($("#ddlTipo").val() == "FCC" || $("#ddlTipo").val() == "NDC" || $("#ddlTipo").val() == "NCC" || $("#ddlTipo").val() == "COT" || MI_CONDICION != "RI") {
            $("#divImpuestos").hide();
            $("#txtIIBB,#txtPercepcionIVA, #txtImpNacionales, #txtImpMunicipales, #txtImpInternos, #txtOtros").val('');
            var total = Compras.getTotal();
            $("#divTotal").html("$ " + addSeparatorsNF(total.toFixed(2), '.', ',', '.'));
        }
        else {
            $("#divImpuestos").show();

            var totalImpuestos = Compras.getImpuestos();
            $("#divTotalImpuestos").html("$ " + addSeparatorsNF(totalImpuestos.toFixed(2), '.', ',', '.'));
        }

        var totalImportes = Compras.getImportes();
        $("#divTotalImportes").html("$ " + addSeparatorsNF(totalImportes.toFixed(2), '.', ',', '.'));
    },
    obtenerUltimoRubroComprobanteCliente: function () {
        var info = "{idPersona: " + $("#ddlPersona").val() + "}";
        $.ajax({
            type: "POST",
            url: "/common.aspx/ObtenerUltimoRubroComprobanteCliente",
            data: info,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                $("#ddlRubro").val(data.d);
            }
        });
    },
    obtenerUltimaCategoriaComprobanteCliente: function () {
        var info = "{idPersona: " + $("#ddlPersona").val() + "}";
        $.ajax({
            type: "POST",
            url: "/common.aspx/ObtenerUltimaCategoriaComprobanteCliente",
            data: info,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                $("#ddlCategoria").val(data.d);
            }
        });
    },
    //*** Categorias***/
    editarCategoria: function (id, nombre) {
        Compras.ocultarMensajes();
        $("#btnCategoria").html("Actualizar");
        $("#txtNuevaCat").val(nombre);
        $("#hdnIDCategoria").val(id);
    },
    eliminarCategoria: function (id) {
        Compras.ocultarMensajes();

        var info = "{ id: " + parseInt(id) + "}";

        $.ajax({
            type: "POST",
            url: "comprase.aspx/eliminarCategoria",
            data: info,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data, text) {

                $("#txtNuevaCat").val("");
                $("#hdnIDCategoria").val("0");

                $("#ddlCategoria").html("");
                Compras.obtenerCategorias("ddlCategoria", true);
            },
            error: function (response) {
                var r = jQuery.parseJSON(response.responseText);
                $("#msgErrorCat").html(r.Message);
                $("#divErrorCat").show();
            }
        });
    },
    grabarCategoria: function () {
        Compras.ocultarMensajes();

        if ($("#txtNuevaCat").val() != "") {

            var info = "{id: " + $("#hdnIDCategoria").val() + ", nombre: '" + $("#txtNuevaCat").val() + "'}";

            $.ajax({
                type: "POST",
                url: "comprase.aspx/guardarCategoria",
                data: info,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data, text) {
                    Compras.obtenerCategorias();
                    $("#txtNuevaCat").val("");
                    $("#hdnIDCategoria").val("0");

                    $("#ddlCategoria").html("");
                    Common.obtenerCategorias("ddlCategoria", true);
                },
                error: function (response) {
                    var r = jQuery.parseJSON(response.responseText);
                    $("#msgErrorCat").html(r.Message);
                    $("#divErrorCat").show();
                }
            });

        }
        else {
            $("#msgErrorCat").html("Debes ingresar un valor");
            $("#divErrorCat").show();
        }
    },
    obtenerCategorias: function () {
        Compras.ocultarMensajes();

        $("#btnCategoria").html("Agregar");
        $("#bodyDetalle").html();

        $.ajax({
            type: "GET",
            url: "comprase.aspx/getCategories",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                if (data != null) {
                    $("#bodyDetalle").html(data.d);
                }
                $('#modalCategorias').modal('show');
            }
        });
    },
    ocultarMensajes: function () {
        $("#divError, #divOk, #divErrorCat,#divErrorJurisdiccion").hide();
    },
    //*** Jurisdicciones ***/
    obtenerJurisdicciones: function (opcion) {

        Compras.ocultarMensajes();
        $("#btnJurisdiccion").html("Agregar");
        $("#bodyDetalleJurisdiccion").html();

        if (opcion == 1) {

            var info = "{idCompra: " + parseInt($("#hdnID").val()) + "}";
            $.ajax({
                type: "POST",
                data: info,
                url: "comprase.aspx/getJurisdicciones",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: false,
                success: function (data) {
                    if (data.d != null) {
                        for (var i = 0; i < data.d.length; i++) {
                            dataJurisdicciones.push(data.d[i]);
                        }
                    }
                },
                error: function (response) {
                }
            });
        }
        else {
            Compras.armarTablaJurisdicciones();
            $('#modalJurisdiccion').modal('show');
        }

    },
    armarTablaJurisdicciones: function () {

        var total = 0;
        for (var i = 0; i < dataJurisdicciones.length; i++) {
            var importe = parseFloat(dataJurisdicciones[i].Importe);
            total += importe;
            dataJurisdicciones[i].Importe = importe.toString().replace(',','.');
            //dataJurisdicciones[i].Importe = importe.toString().replace('.', ',');
        }

        $("#txtIIBB").val(total.toFixed(2).replace('.',',').toString());
        Compras.changeImportes();

        $("#bodyDetalleJurisdiccion").html("");
        if (dataJurisdicciones.length > 0)
            $("#resultTemplate").tmpl({ results: dataJurisdicciones }).appendTo("#bodyDetalleJurisdiccion");
        else
            $("#noResultTemplate").tmpl({ results: dataJurisdicciones }).appendTo("#bodyDetalleJurisdiccion");
    },
    grabarJurisdiccion: function () {
        $("#divErrorJurisdiccion").hide();
        if ($("#txtImporteJurisdiccion,#ddlJurisdiccion").valid() && Compras.validarJurisdicciones()) {
            var obj = new Object();
            obj.IDJurisdicion = $("#ddlJurisdiccion").val();
            obj.Importe = $("#txtImporteJurisdiccion").val().replace(',', '.');
            obj.IDCompra = $("#hdnID").val();
            obj.NombreJurisdiccion = $("#ddlJurisdiccion option:selected").text();
            dataJurisdicciones.push(obj);
            Compras.obtenerJurisdicciones();

            $("#txtImporteJurisdiccion,#ddlJurisdiccion").val("");
            $("#ddlJurisdiccion").trigger("change");
        }
        else {
            return false;
        }
    },
    validarJurisdicciones: function () {
        $("#ddlJurisdiccion").val()
        validJurisdicion = dataJurisdicciones.filter(function (el) {
            return el.IDJurisdicion == parseInt($("#ddlJurisdiccion").val());
        });

        if (validJurisdicion.length > 0) {
            $("#msgErrorJurisdiccion").html("La jurisdicción seleccionada ya se encuentra ingresada.")
            $("#divErrorJurisdiccion").show()
            return false;
        }
        if (!parseFloat($("#txtImporteJurisdiccion").val().replace(',','.')) > 0) {
            $("#msgErrorJurisdiccion").html("El importe debe ser mayotr a 0.")
            $("#divErrorJurisdiccion").show()
            return false;
        }

        return true
    },
    eliminarJurisdiccion: function (id) {
        dataJurisdicciones = dataJurisdicciones.filter(function (el) {
            return el.IDJurisdicion != id;
        });
        Compras.armarTablaJurisdicciones();
    },
    parsearNumeros: function () {
        numeral.language('fr', {
            delimiters: {
                thousands: ',',
                decimal: '.'
            },
            abbreviations: {
                thousand: 'k',
                million: 'm',
                billion: 'b',
                trillion: 't'
            },
            ordinal: function (number) {
                return number === 1 ? 'er' : 'ème';
            },
            currency: {
                symbol: '€'
            }
        });
        numeral.language('fr');
    },
    obtenerJurisdiccionUsuario: function () {
        $.ajax({
            type: "GET",
            url: "/comprase.aspx/obtenerJurisdiccionUsuario",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                if (data != null) {
                    $("#ddlJurisdiccion").html("");
                    $("<option/>").attr("value", "").text("").appendTo($("#ddlJurisdiccion"));
                    for (var i = 0; i < data.d.length; i++) {
                        $("<option/>").attr("value", data.d[i].ID).text(data.d[i].Nombre).appendTo($("#ddlJurisdiccion"));
                    }
                }
            }
        });
    },
    obtenerDetalleDelComprobante: function () {
        var info = "{idCompra:" + parseInt($("#hdnID").val()) + "}";
        $.ajax({
            type: "POST",
            url: "comprase.aspx/obtenerDetalleDelComprobante",
            contentType: "application/json; charset=utf-8",
            data: info,
            dataType: "json",
            success: function (data) {
                if (data != null) {
                    $("#ddlDetalleDelComprobante").html("");
                    $("<option/>").attr("value", "").text("").appendTo($("#ddlDetalleDelComprobante"));
                    for (var i = 0; i < data.d.length; i++) {
                        $("<option/>").attr("value", data.d[i].ID).text(data.d[i].Nombre).appendTo($("#ddlDetalleDelComprobante"));
                        //$("#ddlDetalleDelComprobante").val(data.d[i].ID);
                    }
                    //$("#ddlDetalleDelComprobante").trigger("change");
                    //$("#hdnPageLoad").val("0");
                }
            }
        });
    },
    /*** SEARCH ***/
    configFilters: function () {
        $(".select2").select2({ width: '100%', allowClear: true });

        //Common.obtenerPersonas("ddlPersona", "", true);
       
        // Date Picker
        Common.configDatePicker();
        Common.configFechasDesdeHasta("txtFechaDesde", "txtFechaHasta");
        Common.soloNumerosConGuiones("txtNumero");
        $("#txtNroDocumento").numericInput();

        $("#txtCondicion, #txtFechaDesde, #txtFechaHasta").keypress(function (event) {
            var keycode = (event.keyCode ? event.keyCode : event.which);
            if (keycode == '13') {
                Compras.resetearPagina();
                Compras.filtrar();
                return false;
            }
        });
    },
    nuevo: function () {
        window.location.href = "/comprase.aspx";
    },
    editar: function (id) {
        window.location.href = "/comprase.aspx?ID=" + id;
    },
    eliminar: function (id, nombre) {
        bootbox.confirm("¿Está seguro que desea eliminar la compra realizada a " + nombre + "?", function (result) {
            if (result) {
                $.ajax({
                    type: "POST",
                    url: "compras.aspx/delete",
                    data: "{ id: " + id + "}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data, text) {
                        Compras.filtrar();
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
    },
    mostrarPagAnterior: function () {
        var paginaActual = parseInt($("#hdnPage").val());
        paginaActual--;
        $("#hdnPage").val(paginaActual);
        Compras.filtrar();
    },
    mostrarPagProxima: function (id, nombre) {
        var paginaActual = parseInt($("#hdnPage").val());
        paginaActual++;
        $("#hdnPage").val(paginaActual);
        Compras.filtrar();
    },
    filtrar: function () {
        $("#divError").hide();

        if ($('#frmSearch').valid()) {
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
                url: "compras.aspx/getResults",
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
        }
    },
    verTodos: function () {
        $("#txtNumero, #txtFechaDesde, #txtFechaHasta").val("");//, #ddlTipo
        $("#ddlPersona").val("").trigger("change");
        Compras.filtrar();
    },
    exportar: function () {
        Compras.resetearExportacion();
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
            url: "compras.aspx/export",
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

                Compras.resetearExportacion();
            }
        });
    },
    resetearExportacion: function () {
        $("#imgLoading, #lnkDownload").hide();
        $("#divIconoDescargar").show();
    },
    mostrarPagAnterior: function () {
        var paginaActual = parseInt($("#hdnPage").val());
        paginaActual--;
        $("#hdnPage").val(paginaActual);
        Compras.filtrar();
    },
    mostrarPagProxima: function () {
        var paginaActual = parseInt($("#hdnPage").val());
        paginaActual++;
        $("#hdnPage").val(paginaActual);
        Compras.filtrar();
    },
    resetearPagina: function () {
        $("#hdnPage").val("1");
    },
    otroPeriodo: function () {
        if ($("#ddlPeriodo").val() == "-1")
            $('#divMasFiltros').toggle(600);
        else {
            if ($("#divMasFiltros").is(":visible"))
                $('#divMasFiltros').toggle(600);

            $("#txtFechaDesde,#txtFechaHasta").val("");
            Compras.filtrar();
        }
    },
    /*** Adjuntar Foto ***/
    adjuntarFoto: function () {

        $('#flpArchivo').fileupload({
            url: "/subirImagenes.ashx?idCompras=" + $("#hdnID").val() + "&opcionUpload=compras",
            success: function (response, status) {
                if (response == "OK") {
                    $("#divError").hide();
                    $("#divOk").show();
                    $("#btnActualizar").attr("disabled", false);
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
                Common.ocultarProcesando("btnActualizar", "Aceptar");
            },
            autoUpload: false,
            add: function (e, data) {
                $("#hdnSinCombioDeFoto").val("1");
                $("#btnActualizar").on("click", function () {
                    $("#imgLoading").show();
                    Compras.grabar();

                    if ($("#hdnID").val() != "0") {
                        data.url = "/subirImagenes.ashx?idCompras=" + $("#hdnID").val() + "&opcionUpload=compras";
                        data.submit();

                        if ($("#hdnTieneSaldoAPagar").val() == "1") {
                            $('#modalPagos').modal('show');
                        } else {
                            setTimeout(function () {
                                window.location.href = "/compras.aspx";
                            }, 2000);
                        }
                    }
                });
            }
        });
        Compras.showBtnEliminar();
    },
    showInputFoto: function () {
        $("#divLogo").slideToggle();
    },
    grabarsinImagen: function () {
        if ($("#hdnSinCombioDeFoto").val() == "0") {
            Compras.grabar();
        }
    },
    eliminarFoto: function () {
        var id = ($("#hdnID").val() == "" ? "0" : $("#hdnID").val());
        if (id != "") {
            var info = "{ idCheque: " + parseInt(id) + "}";

            $.ajax({
                type: "POST",
                url: "comprase.aspx/eliminarFoto",
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
                    Compras.showBtnEliminar();
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
            $("#divComprobante").show();
        }
        else {
            $("#divEliminarFoto").hide();
            $("#divAdjuntarFoto").removeClass("col-sm-6").addClass("col-sm-12");
            $("#divComprobante").hide();
        }
    },
    descargarAdjunto: function () {
        var info = "{ id: '" + parseInt($("#hdnID").val()) + "'}";
        $.ajax({
            type: "POST",
            url: "comprase.aspx/descargarAdjunto",
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
            }
        });
    },
}
