var PlanesPagos = {
    grabar: function () {
        $("#divError").hide();
        if ($("#frmEdicion").valid()) {

            if ($("#hdnSinCombioDeFoto").val() == "0") {
                $("#msgError").html("El archivo solo puede ser gif, png, jpg, jpeg y es obligatorio.");
                $("#divError").show();
                return false
            }

            if (parseFloat($("#txtImporte").val()) == 0) {
                $("#msgError").html("El importe debe ser mayor a 0");
                $("#divError").show();
                return false
            }

            Common.mostrarProcesando("btnActualizar");
            var info = "{ id: " + parseInt($("#hdnID").val())
                    + " , idPlan: '" + $("#hdnIdPlan").val()
                    + "', formaDePago: '" + $("input[name='ctl00$MainContent$chkForma']:checked").attr('value')
                    + "', fechaDePago: '" + $("#txtFechaDePago").val()
                    + "', NroReferencia: '" + $("#txtNroReferencia").val()
                    + "', importePagado: " + $("#txtImporte").val()
                    + " , pagoAnual: " + $("#hdnModo").val()
                    + "}";
            $.ajax({
                type: "POST",
                url: "/modulos/seguridad/pagoDePlanes.aspx/GuardarPago",
                data: info,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: false,
                success: function (data, text) {
                    $("#hdnID").val(data.d);
                },
                error: function (response) {
                    var r = jQuery.parseJSON(response.responseText);
                    $("#msgError").html(r.Message);
                    $("#divError").show();
                    $("#divOk").hide();
                    $('html, body').animate({ scrollTop: 0 }, 'slow');
                    Common.ocultarProcesando("btnActualizar", "Informar Pago");
                }
            });
        }
        else
            return false;
    },
    cancelar: function () {
        window.location.href = "/modulos/seguridad/elegir-plan.aspx";
    },
    changeFormaDePago: function () {

        var forma = $("input[name='ctl00$MainContent$chkForma']:checked").attr('value');

        if (forma == "Mercado Pago") {
            $("#divMercadoPago").show();
            $("#divTransferencia,#btnActualizar").hide();
            $("#divDatosTransferencia").hide();
        }
        else if (forma == "Transferencia") {
            $("#divMercadoPago").hide();
            $("#divTransferencia,#btnActualizar").show();
            $("#divDatosTransferencia").show()
        }
        else {
            $("#divMercadoPago,#divTransferencia,#btnActualizar").hide();
            $("#divDatosTransferencia").hide();
        }


        var idPanel = $("input[name='ctl00$MainContent$chkForma']:checked").attr('id');
        switch (idPanel) {
            case "rEfectivo":
                $("#divPanelEfectivo").addClass("activo");
                $("#divPanelTarjeta,#divPanelMercadoPago,#divPanelTransferencia").removeClass("activo");
                break;
            case "rTarjetas":
                $("#divPanelTarjeta").addClass("activo");
                $("#divPanelEfectivo,#divPanelMercadoPago,#divPanelTransferencia").removeClass("activo");
                break;
            case "rMP":
                $("#divPanelMercadoPago").addClass("activo");
                $("#divPanelEfectivo,#divPanelTarjeta,#divPanelTransferencia").removeClass("activo");
                break;
            case "rTransferencia":
                $("#divPanelTransferencia").addClass("activo");
                $("#divPanelEfectivo,#divPanelTarjeta,#divPanelMercadoPago").removeClass("activo");
                break;
        }
    },
    configForm: function () {
        $(".select2").select2({ width: '100%', allowClear: true });

        Common.configDatePicker();
        $('#txtFechaDePago').datepicker();
        $("#txtNroReferencia").numericInput();


        $("#txtImporte").maskMoney({ thousands: '', decimal: '.', allowZero: true });

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

        $("#spTransfImporte,#planImporte").html(parseFloat($("#hdnImporteTotal").val().replace(",", ".")).toFixed(2));
        $("#spTransfNombre,#planNombre").html("Plan " + $("#hdnNombrePlan").val());

        PlanesPagos.adjuntarFoto();
    },

    /*** Adjuntar Foto ***/
    adjuntarFoto: function () {

        $('#flpArchivo').fileupload({
            url: "/subirImagenes.ashx?idPagosPlan=" + $("#hdnID").val() + "&opcionUpload=PagosPlan",
            success: function (response, status) {
                if (response == "OK") {
                    $("#divError").hide();
                    $("#divOk").show();
                    window.location.href = "/modulos/seguridad/ComprasMercadoPago.aspx?tipo=2&external_reference=" + $("#hdnIdPlan").val() + "P";
                }
                else {
                    $("#hdnFileName").val("");
                    $("#msgError").html(response);
                    $("#divError").show();
                    $("#divOk").hide();
                }
            },
            error: function (error) {
                $("#hdnFileName").val("");
                $("#msgError").html(error.responseText);
                $("#imgLoading").hide();
                $("#divError").show();
                $("#divOk").hide();
                Common.ocultarProcesando("btnActualizar", "Informar Pago");
                $('html, body').animate({ scrollTop: 0 }, 'slow');
            },
            autoUpload: false,
            add: function (e, data) {
                if (PlanesPagos.validarExtencion(data.files[0].name)) {
                    $("#hdnSinCombioDeFoto").val("1");
                    $("#btnActualizar").on("click", function () {
                        $("#imgLoading").show();
                        PlanesPagos.grabar();

                        if ($("#hdnID").val() != "0") {
                            data.url = "/subirImagenes.ashx?idPagosPlan=" + $("#hdnID").val() + "&opcionUpload=PagosPlan";
                            data.submit();
                        }
                    });
                }
            }
        });
        PlanesPagos.showBtnEliminar();
    },
    showInputFoto: function () {
        $("#divLogo").slideToggle();
    },
    grabarsinImagen: function () {
        if ($("#hdnSinCombioDeFoto").val() == "0") {
            PlanesPagos.grabar();
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
                    PlanesPagos.showBtnEliminar();
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
    validarExtencion: function (name) {
        var ext = name.split('.').pop().toLowerCase();
        if ($.inArray(ext, ['gif', 'png', 'jpg', 'jpeg']) == -1) {
            return false;
        }
        else {
            return true;
        }
    }
}