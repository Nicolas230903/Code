var generarAbonos = {
    configFilters: function () {
        $('.txtFecha').datepicker();
        configDatePicker();

        $("#ddlModo").attr("onchange", "generarAbonos.changeModoFacturacion()");

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
        $.validator.addMethod("validFechaComprobanteActual", function (value, element) {

            var fInicio = $("#txtFechaComprobante").val().split("/");
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

        $("#txtNumeroFacturaA").mask("?99999999");
        $("#txtNumeroFacturaA").blur(function () {
            $("#txtNumeroFacturaA").val(padZeros($("#txtNumeroFacturaA").val(), 8));
        });

        $("#txtNumeroFacturaB").mask("?99999999");
        $("#txtNumeroFacturaB").blur(function () {
            $("#txtNumeroFacturaB").val(padZeros($("#txtNumeroFacturaB").val(), 8));
        });

        $("#txtNumeroFacturaC").mask("?99999999");
        $("#txtNumeroFacturaC").blur(function () {
            $("#txtNumeroFacturaC").val(padZeros($("#txtNumeroFacturaC").val(), 8));
        });


        if ($("#CondicionIva").val() == "MO") {
            $("#divNroComprobanteA,#divNroComprobanteB").hide();
            $("#divNroComprobanteC").show();
        }
        else {
            $("#divNroComprobanteA,#divNroComprobanteB").show();
            $("#divNroComprobanteC").hide();
        }

        // Validation with select boxes
        $("#fromEdicion").validate({
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
    },
    resetearPagina: function () {
        $("#hdnPage").val("1");
    },
    filtrar: function () {
        $("#divError,#divOk").hide();
        $("#resultsContainer").html("");
        $("#divfooter").hide();
        $("#txtFechaComprobante,#txtNumeroFacturaC,#txtNumeroFacturaA,#txtNumeroFacturaB").val("");
        if ($("#txtFecha").valid()) {

            Common.mostrarProcesando("btnBuscar");
            var info = "{ fecha: '" + $("#txtFecha").val() + "'}";

            $.ajax({
                type: "POST",
                url: "generarAbonos.aspx/getResults",
                data: info,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data, text) {
                    generarAbonos.changePuntoDeVenta();

                    if (data.d.Items.length > 0)
                        $("#resultTemplate").tmpl({ results: data.d.Items }).appendTo("#resultsContainer");
                    else
                        $("#noResultTemplate").tmpl({ results: data.d.Items }).appendTo("#resultsContainer");


                    if (data.d.Items.length > 0) {
                        $("#ContResultados").show();
                        $("#divfooter").show();
                        $("#idTotalImportes").html("Total Importe: $" + data.d.totalImporte);
                        $("#idTotalIva").html("Total IVA: $" + data.d.TotalIva);
                        $("#idTotal").html("Total : $" + data.d.Total);
                    }

                    Common.ocultarProcesando("btnBuscar", "Buscar");
                },
                error: function (response) {
                    var r = jQuery.parseJSON(response.responseText);
                    $("#msgError").html(r.Message);
                    $("#divError").show();
                    Common.ocultarProcesando("btnBuscar", "Buscar");
                }
            });
        }
        else {
            return false;

        }
    },
    changeModoFacturacion: function () {

        if ($("#CondicionIva").val() == "MO") {
            $("#divNroComprobanteC").toggle();
        }
        else {
            $("#divNroComprobanteA,#divNroComprobanteB").toggle();
        }
    },
    ingresarDatos: function () {
        $("#imgLoading2").show();
        $("#btnGenerar").attr("disabled", true);

        if (generarAbonos.validarGenerarAbonos()) {
            generarAbonos.generarCAE();
        }
        else {
            $("#btnGenerar").attr("disabled", false);
            $("#imgLoading2").hide();
        }
    },
    validarGenerarAbonos: function () {

        if (!$("#ddlPuntoVenta,#ddlModo").valid()) {
            $("#msgError").html("Todos los campos son obligatorios.");
            $("#divError").show();
            return false;
        }

        if (!$("#txtFechaComprobante").valid()) {
            return false;
        }

        if ($("#txtFechaComprobante").val() == "" || $("#txtFechaComprobante").val() == null) {
            $("#msgError").html("La fecha del comprobante es obligatorios.");
            $("#divError").show();
            return false;
        }

        if ($("#CondicionIva").val() == "MO") {
            if (parseInt($("#txtNumeroFacturaC").val()) == 0) {
                $("#msgError").html("El numero del comprobante tiene que ser mayor a 0");
                $("#divError").show();
                return false;   //remarcar los comprobantes con el error
            }
        }
        else {
            if (parseInt($("#txtNumeroFacturaA").val()) == 0 || parseInt($("#txtNumeroFacturaB").val()) == 0) {
                $("#msgError").html("El numero del comprobante tiene que ser mayor a 0");
                $("#divError").show();
                return false;   //remarcar los comprobantes con el error
            }
        }

        var datos = 0;
        $("input:checkbox:checked").each(function () {
            datos++;
        });

        if (datos == 0) {
            $("#msgError").html("Debe elejir al menos un cliente. ");
            return false;   //remarcar los comprobantes con el error
        }
        return true;
    },
    generarCAE: function () {
        $("#divError").hide();
        var info = "{idAbonos: '";
        $("input:checkbox:checked").each(function () {
            info += $(this).attr("id");
        });
        info += "',modo:'" + $("#ddlModo").val()
        info += "',idPuntoVenta:" + $("#ddlPuntoVenta").val()
        info += ",numeroFacturaA:'" + $("#txtNumeroFacturaA").val()
        info += "',numeroFacturaB:'" + $("#txtNumeroFacturaB").val()
        info += "',numeroFacturaC:'" + $("#txtNumeroFacturaC").val()
        info += "',fechaComprobante:'" + $("#txtFechaComprobante").val()
        info += "'}";

        $.ajax({
            type: "POST",
            url: "generarAbonos.aspx/generarComprobanteAbono",
            data: info,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data, text) {
                $("#divOk").show();
                $("#btnGenerar").attr("disabled", false);
                $("#resultsContainer").html("");
                $("#divfooter").hide();
                $("#imgLoading2").hide();

                if ($("#ddlModo").val() == "E")
                    $("#EsFacturaElectronica").val("1");
                else
                    $("#EsFacturaElectronica").val("0");

                if (data.d.Items.length > 0)
                    $("#resultComprobanteFETemplate").tmpl({ results: data.d.Items }).appendTo("#resultsContainer");
                else
                    $("#noResultTemplate").tmpl({ results: data.d.Items }).appendTo("#resultsContainer");
            },
            error: function (response) {
                var r = jQuery.parseJSON(response.responseText);
                $("#msgError").html(r.Message);
                $("#divError").show();
                $("#btnGenerar").attr("disabled", false);
                $("#imgLoading2").hide();
            }
        });
    },
    showModalFacturacion: function (archivo, razonSocial, email, envioFE) {
        $("#divErrorEnvioFE,#divErrorMail").hide();
        if ($("#ddlModo").val() == "E") {
            $("#hdnRazonSocial").val(razonSocial);
            $("#txtEnvioPara").val(email);
            //Seteo la opcion de imprimir
            var version = new Date().getTime();
            $("#ifrPdf").attr("src", "/files/comprobantes/" + archivo + "?" + version + "#zoom=100&view=FitH,top");

            //Seteo el link de download
            var fileName = archivo.split("/")[1];
            $("#lnkDownloadPdf").attr("href", "/pdfGenerator.ashx?file=" + fileName);

            //Seteo el nombre del archivo para envio de mail
            $("#hdnFile").val(fileName);

            //Muestro la ventana
            $('#ModalFacturacion').modal('show');

            if (envioFE != "") {
                $("#msgErrorEnvioFE").html(envioFE);
                $("#divErrorEnvioFE").show();
            }

            if ($("#hdnEnvioFE").val() == "1" && envioFE == "") {
                $("#spSendMail").html("Enviado");
                $("#imgMailEnvio").attr("style", "color:#17a08c;font-size: 30px;");
                $("#spSendMail").attr("style", "color:#17a08c");
                $("#iCheckEnvio").addClass("fa fa-check");
            }
            else {
                $("#iCheckEnvio").removeClass("fa fa-check");
            }
        }
    },
    ActualizarTotales: function () {

        var totalImporte = 0;
        var totalIVA = 0;
        $("input:checkbox:checked").each(function () {
            var cantidad = parseInt($(this).attr("cantidad"));
            totalImporte += (parseFloat($(this).attr("importe").replace(",", ".")) * cantidad);
            totalIVA += (parseFloat($(this).attr("Iva").replace(",", ".")) * cantidad);
        })

        $("#idTotalImportes").html("Total Importe: $" + numeral((totalImporte)).format('0,0.00'));
        $("#idTotalIva").html("Total IVA: $" + numeral((totalIVA)).format('0,0.00'));
        $("#idTotal").html("Total : $" + numeral((totalImporte + totalIVA)).format('0,0.00'));
    },
    changePuntoDeVenta: function () {
        if ($("#ddlModo").val() != "COT") {
            if ($("#CondicionIva").val() == "MO") {
                Common.obtenerProxNroComprobante("txtNumeroFacturaC", "FCC", $("#ddlPuntoVenta").val());
            }
            else {
                Common.obtenerProxNroComprobante("txtNumeroFacturaA", "FCA", $("#ddlPuntoVenta").val());
                Common.obtenerProxNroComprobante("txtNumeroFacturaB", "FCB", $("#ddlPuntoVenta").val());
            }
        }
    }

}