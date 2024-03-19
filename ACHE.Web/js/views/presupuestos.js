var esModificacion = false;

var Presupuestos = {
    /*** FORM ***/
    ocultarMensajes: function () {
        $("#divError, #divOk").hide();
    },
    grabar: function () {
        Presupuestos.ocultarMensajes();

        var id = ($("#hdnID").val() == "" ? "0" : $("#hdnID").val());
        if ($('#frmEdicion').valid()) {
            Common.mostrarProcesando("btnActualizar");
            var idPersona = 0;
            if ($("#ddlPersona").val() != null && $("#ddlPersona").val() != "")
                idPersona = parseInt($("#ddlPersona").val());

            var info = "{ id: " + parseInt(id)
                    + ", idPersona: '" + idPersona
                    + "', fecha: '" + $("#txtFechaValidez").val()
                    + "', nombre: '" + $("#txtNombre").val()
                    + "', numero: " + $("#txtNumero").val()
                    + ", condicionesPago: '" + $("#ddlFormaPago").val()
                    + "', obs: '" + $("#txtObservaciones").val()
                    + "', estado: '" + $("#ddlEstado").val()
                    + "', vendedor: '" + $("#txtVendedor").val()
                    + "'}";

            $.ajax({
                type: "POST",
                url: "presupuestose.aspx/guardar",
                data: info,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data, text) {
                    $('#divOk').show();
                    $("#divError").hide();
                    $('html, body').animate({ scrollTop: 0 }, 'slow');
                    //window.location.href = "/presupuestos.aspx";
                    $("#hdnID").val(data.d);
                    //Muestro la ventana
                    $('#modalOk').modal('show');
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
            return false;
        }
    },
    cancelar: function () {
        window.location.href = "/presupuestos.aspx";
    },
    calcularPrecioFinal: function () {
        var total = 0;

        if ($("#txtPrecio").val() != "") {
            total += parseFloat($("#txtPrecio").val().replace(",", "."));
        }
        var iva = parseFloat($("#ddlIva").val().replace(",", "."));

        total = (total + ((total * iva) / 100));

        $("#divPrecioIVA").html("$ " + addSeparatorsNF(total.toFixed(2), '.', ',', '.'));
    },
    configForm: function () {
        Common.configDatePicker();

       

        $(".select2").select2({
            width: '100%', allowClear: true,
            formatNoMatches: function (term) {
                return "<a style='cursor:pointer' onclick=\"$('#modalNuevoCliente').modal('show');$('.select2').select2('close');\">+ Agregar</a>";
            }
        });

        $(".select3").select2({ width: '100%', allowClear: true, });

        if (MI_CONDICION == "MO") {
            $("#ddlIva").attr("disabled", true);
        }
        else if (MI_CONDICION == "RI") {
            $("#ddlIva").val("21");
        }
        //Seteo las personas que corresponden
        Common.obtenerClientes("ddlPersona", $("#hdnIDPersona").val(), true);

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

        $("#txtPrecio, #txtBonificacion").maskMoney({ thousands: '', decimal: ',', allowZero: true });

        $("#txtPrecio,#txtBonificacion").keypress(function (event) {
            var keycode = (event.keyCode ? event.keyCode : event.which);
            if (keycode == '13') {
                $("#txtCantidad").focus();
                Presupuestos.agregarItem();
                return false;
            }
        });

        $("#txtPrecio").blur(function () {
            Presupuestos.calcularPrecioFinal();
        });
        $("#txtPrecio, #ddlIva").change(function () {
            Presupuestos.calcularPrecioFinal();
        });

        $("#txtNumero").mask("?99999999");
        $("#txtNumero").blur(function () {
            $("#txtNumero").val(padZeros($("#txtNumero").val(), 8));
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

        Presupuestos.showbtnfacturarPresupuesto();
        Common.obtenerConceptosCodigoyNombre("ddlProductos", 3, true);


        Presupuestos.obtenerItems();
        Presupuestos.obtenerTotales();


        if (parseInt($("#hdnID").val()) == 0)
            Common.obtenerProxNroPresupuesto("txtNumero")
        else
            $("#divMasOpciones").show();

        $("#ddlPersona").attr("onchange", "Presupuestos.changePersona()");
       
    },
    showbtnfacturarPresupuesto: function () {
        if ($("#hdnPresupuestoVinculado").val() == "0") {
            if ($("#ddlEstado").val() == "A") {
                $("#btnFacturarPresupuesto").show();
            }
            else {
                $("#btnFacturarPresupuesto").hide();
            }
        }        
    },
    facturarPresupuesto: function () {

        Presupuestos.ocultarMensajes();

        var id = ($("#hdnID").val() == "" ? "0" : $("#hdnID").val());
        if ($('#frmEdicion').valid()) {

            var idPersona = 0;
            if ($("#ddlPersona").val() != null && $("#ddlPersona").val() != "")
                idPersona = parseInt($("#ddlPersona").val());

            var nroPresupuesto = 0;
            if ($("#txtNumero").val() != null && $("#txtNumero").val() != "")
                nroPresupuesto = parseInt($("#txtNumero").val());

            var info = "{ id: " + parseInt(id)
                    + ", idPersona: '" + idPersona
                    + "', fecha: '" + $("#txtFechaValidez").val()
                    + "', nombre: '" + $("#txtNombre").val()
                    + "', numero: " + nroPresupuesto
                    + ", condicionesPago: '" + $("#ddlFormaPago").val()
                    + "', precio: '" + $("#txtPrecio").val()
                    + "', iva: '" + $("#ddlIva").val()
                    + "', obs: '" + $("#txtObservaciones").val()
                    + "', estado: '" + $("#ddlEstado").val()
                    + "', vendedor: '" + $("#txtVendedor").val()
                    + "'}";

            $.ajax({
                type: "POST",
                url: "presupuestose.aspx/guardar",
                data: info,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data, text) {
                    $('#divOk').show();
                    $("#divError").hide();
                    $('html, body').animate({ scrollTop: 0 }, 'slow');

                    window.location.href = 'comprobantese.aspx?IDPresupuesto=' + data.d;
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
    },
    changeConcepto: function () {
        if (esModificacion == false) {
            if ($("#ddlProductos").val() != "" && $("#ddlProductos").val() != null) {
                $("#txtConcepto").attr("disabled", true);

                var idPersona = 0;
                if ($("#ddlPersona").val() != null && $("#ddlPersona").val() != "")
                    idPersona = parseInt($("#ddlPersona").val());

                $.ajax({
                    type: "POST",
                    url: "conceptose.aspx/obtenerDatos",
                    data: "{id: " + parseInt($("#ddlProductos").val()) + ",idPersona: " + idPersona + "}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data, text) {
                        if (data.d != null) {
                            $("#txtConcepto").val(data.d.Nombre);
                            $("#txtPrecio").val(data.d.Precio.replace('.',','));
                            //$("#ddlIva").val(data.d.Iva.replace(".", ","));
                            $("#ddlIva").val(data.d.TipoIva);
                            $("#hdnCodigo").val(data.d.Codigo);
                        }
                        $("#divErrorDetalle").hide();
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
    },
    previsualizar: function () {
        if ($('#frmEdicion').valid()) {
            if (parseInt($("#txtNumero").val()) == 0) {
                $("#msgError").html("El número de presupuesto debe ser mayor a cero");
                $("#divError").show();
                return false;
            }

            var info = "{ id: " + parseInt($("#hdnID").val())
                        + ", idPersona: " + parseInt($("#ddlPersona").val())
                        + ", fechaVencimiento: '" + $("#txtFechaValidez").val()
                        + "', nombre: '" + $("#txtNombre").val()
                        + "', numero: '" + $("#txtNumero").val()
                        + "', condicionesPago: '" + $("#ddlFormaPago").val()
                        + "', obs: '" + $("#txtObservaciones").val()
                        + "', vendedor: '" + $("#txtVendedor").val()
                        + "'}";

            $.ajax({
                type: "POST",
                url: "presupuestose.aspx/previsualizar",
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
            return false;
        }
    },    
    generarRemito: function () {
        var info = "{ id: " + $("#hdnID").val() + "}";

        $.ajax({
            type: "POST",
            url: "/presupuestose.aspx/generarRemito",
            data: info,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data, text) {
                window.location.href = "/pdfGenerator.ashx?file=" + data.d + "&tipoDeArchivo=remito";
            },
            error: function (response) {
                var r = jQuery.parseJSON(response.responseText);
                alert(r.Message);
            }
        });
    },
    generarPresupuesto: function (extension) {
        var info = "{ id: " + $("#hdnID").val() + ", extension: '" + extension + "'}";

        $.ajax({
            type: "POST",
            url: "/presupuestose.aspx/generarPresupuesto",
            data: info,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data, text) {
                window.location.href = "/pdfGenerator.ashx?file=" + data.d + "&tipoDeArchivo=presupuestos";
            },
            error: function (response) {
                var r = jQuery.parseJSON(response.responseText);
                alert(r.Message);
            }
        });
    },
    changePersona:function()
    {
        setTimeout(Presupuestos.obtenerInfoPersona($("#ddlPersona").val()), 5000);
    },
    obtenerInfoPersona: function (idPersona) {

        if (idPersona == "" || idPersona == null) {
            Presupuestos.changelblPrecioUnitario("");
        }
        else {
            $.ajax({
                type: "POST",
                url: "personase.aspx/obtenerDatos",
                data: "{ id: " + idPersona + "}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data, text) {
                    if (data.d != null) {
                        Presupuestos.changelblPrecioUnitario(data.d.CondicionIva);

                        if ($("#hdnParaPDVSolicitarCompletarContacto").val() == "1") {
                            if (data.d.ProvinciaDesc != "" && data.d.CiudadDesc != ""
                                && data.d.Email != "" && data.d.Telefono != "") {
                                $("#divClienteDatosPendientes").hide();
                            }
                            else {
                                $("#hdnClienteTipo").val(data.d.Tipo);
                                $("#divClienteDatosPendientes").show();
                                $("#divClienteDatosPendientesTexto").html("Cliente con datos de contacto pendientes de completar!! Clic <a onclick='Presupuestos.editarPersona(" + data.d.ID + ");' style='cursor: pointer;'><span>AQUI</span></a> para actualizar.");
                            }
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
    },
    editarPersona: function(id) {
        window.location.href = "/personase.aspx?ID=" + id + "&tipo=" + $("#hdnClienteTipo").val();
    },
    changelblPrecioUnitario: function (condicionIVA) {
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
    },
    /*** ITEM***/

    cancelarItem: function () {
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
    },

    agregarItem: function () {
        Presupuestos.ocultarMensajes();
        esModificacion = false;
        if ($("#txtCantidad").val() != "" && $("#txtConcepto").val() != "" && $("#txtPrecio").val() != "") {

            if (parseFloat($("#txtPrecio").val()) == 0) {
                $("#msgErrorDetalle").html("El precio debe ser mayor a 0.");
                $("#divErrorDetalle").show();
            }
            else {
                var idPersona = ($("#ddlPersona").val() == "" || $("#ddlPersona").val() == null) ? 0 : parseInt($("#ddlPersona").val());

                var info = "{ id: " + parseInt($("#hdnIDItem").val())
                        + ", idConcepto: '" + $("#ddlProductos").val()
                        + "', concepto: '" + $("#txtConcepto").val()
                        + "', iva: '" + $("#ddlIva").val()
                        + "', precio: '" + $("#txtPrecio").val()
                        + "', bonif: '" + $("#txtBonificacion").val()
                        + "', cantidad: '" + $("#txtCantidad").val()
                        + "', idPersona: " + idPersona
                         + ", codigo:'" + $("#hdnCodigo").val()
                        + "'}";

                $.ajax({
                    type: "POST",
                    url: "presupuestose.aspx/agregarItem",
                    data: info,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data, text) {
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
                        $("#txtConcepto").attr("disabled", false);
                        $("#ddlProductos").val("").trigger("change");
                        $("#hdnIDItem").val("0");
                        $("#hdnCodigo").val("");
                        $("#btnAgregarItem").html("Agregar");
                        $("#txtCantidad").focus();
                        Presupuestos.obtenerItems();
                        Presupuestos.obtenerTotales();
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
    },

    eliminarItem: function (id) {

        var info = "{ id: " + parseInt(id) + "}";

        $.ajax({
            type: "POST",
            url: "presupuestose.aspx/eliminarItem",
            data: info,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data, text) {
                Presupuestos.obtenerItems();
                Presupuestos.obtenerTotales();
            },
            error: function (response) {
                var r = jQuery.parseJSON(response.responseText);
                $("#msgErrorDetalle").html(r.Message);
                $("#divErrorDetalle").show();
            }
        });
    },

    modificarItem: function (id, idConcepto, cantidad, concepto, precio, iva, bonif) {

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

        $("#txtConcepto").val(concepto);
        $("#txtPrecio").val(precio);
        $("#ddlIva").val(iva);
        $("#txtBonificacion").val(bonif);
        esModificacion = true;
        if (idConcepto != "") {
            $("#ddlProductos").val(idConcepto).trigger("change");
            $("#txtConcepto").attr("disabled", true);
        }
        $("#hdnIDItem").val(id);
        $("#btnAgregarItem").html("Actualizar");
    },

    obtenerItems: function () {
        var idPersona = ($("#ddlPersona").val() == "" || $("#ddlPersona").val() == null) ? 0 : parseInt($("#ddlPersona").val());
        $.ajax({
            type: "POST",
            data: "{idPersona: " + idPersona + "}",
            url: "presupuestose.aspx/obtenerItems",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                if (data != null) {
                    $("#bodyDetalle").html(data.d);
                }
            }
        });
    },

    obtenerTotales: function () {
        $.ajax({
            type: "POST",
            url: "presupuestose.aspx/obtenerTotales",
            data: "{}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data, text) {
                if (data.d != null) {
                    $("#divSubtotal").html("$ " + data.d.Subtotal);
                    $("#divIVA").html("$ " + data.d.Iva);
                    $("#divTotal").html("$ " + data.d.Total);
                }
            },
            error: function (response) {
                var r = jQuery.parseJSON(response.responseText);
                $("#msgErrorDetalle").html(r.Message);
                $("#divErrorDetalle").show();
            }
        });
    },
    ocultarMensajes: function () {
        $("#divError, #divOk, #divErrorDetalle").hide();
    },

    /*** SEARCH ***/
    configFilters: function () {
        $(".select2").select2({ width: '100%', allowClear: true });

        //Common.obtenerPersonas("ddlPersona", "", true);

        $("#txtFechaDesde, #txtFechaHasta, #txtCondicion").keypress(function (event) {
            var keycode = (event.keyCode ? event.keyCode : event.which);
            if (keycode == '13') {
                Presupuestos.resetearPagina();
                Presupuestos.filtrar();
                return false;
            }
        });

        // Date Picker
        Common.configDatePicker();
        Common.configFechasDesdeHasta("txtFechaDesde", "txtFechaHasta");

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
    },
    nuevo: function () {
        window.location.href = "/presupuestose.aspx";
    },
    editar: function (id) {
        window.location.href = "/presupuestose.aspx?ID=" + id;
    },
    eliminar: function (id, numero) {
        bootbox.confirm("¿Está seguro que desea eliminar el presupuesto #" + numero + "?", function (result) {
            if (result) {
                $.ajax({
                    type: "POST",
                    url: "presupuestos.aspx/delete",
                    data: "{ id: " + id + "}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data, text) {
                        Presupuestos.filtrar();
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
    filtrar: function () {
        $("#divError").hide();

        if ($('#frmSearch').valid()) {
            $("#resultsContainer").html("");
            var currentPage = parseInt($("#hdnPage").val());

            var nroPresupuesto = 0;
            if ($("#txtNumero").val() != null && $("#txtNumero").val() != "")
                nroPresupuesto = parseInt($("#txtNumero").val());

            var info = "{condicion: '" + $("#txtCondicion").val()
                       + "', periodo: '" + $("#ddlPeriodo").val()
                       + "', fechaDesde: '" + $("#txtFechaDesde").val()
                       + "', fechaHasta: '" + $("#txtFechaHasta").val()
                       + "', page: " + currentPage + ", pageSize: " + PAGE_SIZE
                       + "}";

            $.ajax({
                type: "POST",
                url: "presupuestos.aspx/getResults",
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
            Presupuestos.resetearExportacion();
        }
    },
    verTodos: function () {
        $("#txtCondicion, #txtFechaDesde, #txtFechaHasta").val("");
        Presupuestos.filtrar();
    },
    exportar: function () {
        Presupuestos.resetearExportacion();

        $("#imgLoading").show();
        $("#divIconoDescargar").hide();

        var nroPresupuesto = 0;
        if ($("#txtNumero").val() != null && $("#txtNumero").val() != "")
            nroPresupuesto = parseInt($("#txtNumero").val());

        var info = "{ condicion: '" + $("#txtCondicion").val()
                + "', periodo: '" + $("#ddlPeriodo").val()
                + "', fechaDesde: '" + $("#txtFechaDesde").val()
                + "', fechaHasta: '" + $("#txtFechaHasta").val()
                + "'}";

        $.ajax({
            type: "POST",
            url: "presupuestos.aspx/export",
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
                Presupuestos.resetearExportacion();
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
        Presupuestos.filtrar();
    },
    mostrarPagProxima: function () {
        var paginaActual = parseInt($("#hdnPage").val());
        paginaActual++;
        $("#hdnPage").val(paginaActual);
        Presupuestos.filtrar();
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
            Presupuestos.filtrar();
        }
    },
    vincularPresupuesto: function () {
        if ($("#ddlEstado").val() != "A") {

            bootbox.confirm("Para genera la copia a pedido el prespuesto cambiará a estado APROBADO, ¿Continuar?", function (result) {
                if (result) {
                    Presupuestos.generarPedido();
                }
            });
            
        } else {
            Presupuestos.generarPedido();
        }       
    },
    generarPedido: function () {
        var info = "{ id: " + $("#hdnID").val() + "}";
        $.ajax({
            type: "POST",
            url: "presupuestose.aspx/vincularPresupuesto",
            data: info,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data, text) {
                window.location.href = "/comprobantese.aspx?tipo=PDV&ID=" + data.d;
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