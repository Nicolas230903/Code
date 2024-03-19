var caja = {
    /*** FORM ***/
    grabar: function () {
        $("#divError,#divOk,#divErrorAlta").hide();
        $("#divErrorAlta").hide();
        var id = ($("#hdnID").val() == "" ? "0" : $("#hdnID").val());
        if (parseFloat($("#txtImporte").val()) == 0) {
            $("#msgErrorAlta").html("El importe debe ser mayor a 0");
            $("#divErrorAlta").show();
            return false;
        }

        if (!$("#ddlPlanDeCuentas").valid() && $("#hdnUsaPlanCorporativo").val() == "1") {
            return false;
        }

        if ($('#txtImporte,#txtTipoMovimiento,#ddlConcepto').valid()) {
            var idConcepto = ($("#ddlConcepto").val() == "" || $("#ddlConcepto").val() == null) ? "" : parseInt($("#ddlConcepto").val());
            var idPlanDeCuenta = ($("#ddlPlanDeCuentas").val() == "" || $("#ddlPlanDeCuentas").val() == null) ? 0 : parseInt($("#ddlPlanDeCuentas").val());

            Common.mostrarProcesando("btnActualizar");
            var info = jQuery.parseJSON('{ "ID": ' + parseInt(id)
            + ', "Concepto": "' + idConcepto
            + '", "Observaciones": "' + $("#txtObservaciones").val()
            + '", "Importe": ' + $("#txtImporte").val().replace(",",".")
            + ' , "tipoMovimiento": "' + $("#txtTipoMovimiento").val()
            + '", "Fecha": "' + $("#txtFecha").val()
            + '", "MedioDePago": "' + $("#ddlMedioPago").val()
            + '", "Ticket": "' + $("#txtTicket").val()
            + '", "IDPlanDeCuenta": ' + parseInt(idPlanDeCuenta)
            + '}');

            $.ajax({
                type: "POST",
                url: "/modulos/tesoreria/cajase.aspx/guardar",
                data: "{caja: " + JSON.stringify(info) + "}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: false,
                success: function (data, text) {
                    $('#divOk').show();
                    $("#divErrorAlta").hide();
                    $('html, body').animate({ scrollTop: 0 }, 'slow');

                    $("#hdnID").val(data.d);
                    if ($("#hdnUsaPlanCorporativo").val() == "1") {
                        caja.generarAsientosContables();
                    }

                    if ($("#hdnSinCombioDeFoto").val() == "0") {
                        Common.ocultarProcesando("btnActualizar", "Aceptar");
                        
                        window.location.href = "/modulos/tesoreria/caja.aspx";
                        
                    }
                },
                error: function (response) {
                    var r = jQuery.parseJSON(response.responseText);
                    $("#msgErrorAlta").html(r.Message);
                    $("#divErrorAlta").show();
                    $("#divOk").hide();
                    $('html, body').animate({ scrollTop: 0 }, 'slow');
                    Common.ocultarProcesando("btnActualizar", "Aceptar");
                }
            });
        }
        else {
            $("#msgErrorAlta").html("Por favor, complete todos los campos");
            $("#divErrorAlta").show();

            return false;
        }
    },
    cancelar: function () {
        window.location.href = "/modulos/tesoreria/caja.aspx";
    },
    configForm: function () {
        Common.configDatePicker();
        $(".select2").select2({
            width: '100%', allowClear: true
        });
        // Validation with select boxes
        $("#ddlEstadoCaja,#txtImporte,#txtTipoMovimiento,#ddlPlanDeCuentas").validate({
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

        // Validation with select boxes
        $("#frmNuenaCaja").validate({
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

        if ($("#hdnFileName").val() != "") {
            $(".fileinput-filename").html($("#hdnFileName").val());
            $("#iImgFileFoto").show();
        }

        if (parseInt($("#hdnID").val()) > 0) {
            $("#txtImporte,#txtTipoMovimiento,#txtFecha").attr("disabled", true);
        }

        $("#txtImporte").maskMoney({ thousands: '', decimal: ',', allowZero: true });
        foto.adjuntarFoto();
    },
    limpiarCampos: function () {
        $("#txtTipoMovimiento").val("Ingreso");
        $("#ddlConcepto,#txtObservaciones,#txtImporte,#txtFecha,#txtTicket,#ddlPlanDeCuentas").val("");
        $("#ddlMedioPago").val("Efectivo");
        $("#ddlConcepto,#ddlMedioPago,#ddlPlanDeCuentas").trigger("change");

        $("#divError,#divOk,#divErrorAlta").hide();
        $("#hdnID").val("0");
        $("#txtFecha").datepicker("setDate", new Date());

        $(".fileinput-filename").html("");
        $("#iImgFileFoto").hide();
        $("#hdnTieneFoto").val("0");
        $("#hdnSinCombioDeFoto").val("0");
        $("#divLogo").hide();
        foto.showBtnEliminar();
        Common.ocultarProcesando("btnActualizar", "Aceptar");
    },
    generarAsientosContables: function () {
        $.ajax({
            type: "POST",
            url: "/modulos/tesoreria/cajase.aspx/GenerarAsientosContables",
            data: "{ id: " + $("#hdnID").val() + "}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: false,
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
    },
    /*** Conceptos ***/
    ocultarMensajes: function () {
        $("#divError, #divOk, #divErrorCat").hide();
    },
    editarConcepto: function (id, nombre) {
        caja.ocultarMensajes();
        $("#btnConcepto").html("Actualizar");
        $("#txtNuevaCat").val(nombre);
        $("#hdnIDConcepto").val(id);
    },
    eliminarConcepto: function (id) {
        caja.ocultarMensajes();

        var info = "{ id: " + parseInt(id) + "}";

        $.ajax({
            type: "POST",
            url: "/modulos/tesoreria/cajase.aspx/eliminarConcepto",
            data: info,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data, text) {
                caja.obtenerConceptos();
                $("#txtNuevaCat").val("");
                $("#hdnIDConcepto").val("0");
                $("#ddlConcepto").html("");
                caja.obtenerSelectConceptos(true);
            },
            error: function (response) {
                var r = jQuery.parseJSON(response.responseText);
                $("#msgErrorCat").html(r.Message);
                $("#divErrorCat").show();
            }
        });
    },
    grabarConcepto: function () {
        caja.ocultarMensajes();

        if ($("#txtNuevaCat").val() != "") {

            var info = "{id: " + $("#hdnIDConcepto").val() + ", nombre: '" + $("#txtNuevaCat").val() + "'}";

            $.ajax({
                type: "POST",
                url: "/modulos/tesoreria/cajase.aspx/guardarConcepto",
                data: info,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data, text) {
                    caja.obtenerConceptos();
                    $("#txtNuevaCat").val("");
                    $("#hdnIDConcepto").val("0");

                    $("#ddlConcepto").html("");
                    caja.obtenerSelectConceptos(true);
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
    ocultarMensajes: function () {
        $("#divError, #divOk, #divErrorCat").hide();
    },
    obtenerConceptos: function () {
        caja.ocultarMensajes();

        $("#btnConcepto").html("Agregar");
        $("#bodyDetalle").html();

        $.ajax({
            type: "GET",
            url: "/modulos/tesoreria/cajase.aspx/ObtenerConceptosCaja",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                if (data != null) {
                    $("#bodyDetalle").html(data.d);
                }
                $('#modalConceptos').modal('show');
            }
        });
    },
    obtenerSelectConceptos: function (showEmpty) {
        $.ajax({
            type: "GET",
            url: "/modulos/tesoreria/cajase.aspx/ObtenerSelectConceptos",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                if (data != null) {
                    $("#ddlConcepto").html("");
                    if (showEmpty)
                        $("<option/>").attr("value", "").text("").appendTo($("#ddlConcepto"));

                    for (var i = 0; i < data.d.length; i++) {
                        $("<option/>").attr("value", data.d[i].ID).text(data.d[i].Nombre).appendTo($("#ddlConcepto"));
                    }
                    $("#ddlConcepto").trigger("change");
                }
            }
        });
    },
    /*** SEARCH ***/
    configFilters: function () {
        // Date Picker
        Common.configDatePicker();
        Common.configFechasDesdeHasta("txtFechaDesde", "txtFechaHasta");

        $(".select2").select2({
            width: '100%', allowClear: true
        });
        caja.verificarPlanDeCuentas();
    },
    nuevo: function () {
        window.location.href = "/modulos/tesoreria/cajase.aspx";;
    },
    editar: function (id) {
        window.location.href = "/modulos/tesoreria/cajase.aspx?ID=" + id;
    },
    eliminar: function (id, nombre) {
        $("#divError").hide();
        bootbox.confirm("<h4>Ingrese el motivo para anular el " + nombre + ".</h4>"
            + "<div class='alert alert-danger' id='divErrorEliminar' style='display: none'>"
            + "<strong>Lo sentimos!</strong> <span id='msgErrorEliminar'></span></div>"
            + "<input type='text' class='form-control required' id='txtMotivo'></input>", function (result) {
                if (result) {

                    if ($("#txtMotivo").val() == "" || $("#txtMotivo").val() == null) {
                        $("#msgErrorEliminar").html("El motivo es obligatorio");
                        $("#divErrorEliminar").show();
                        $("#txtMotivo").valid();
                        return false;
                    }
                    else {
                        $.ajax({
                            type: "POST",
                            url: "/modulos/tesoreria/caja.aspx/delete",
                            data: "{ id: " + id + " , motivo: '" + $("#txtMotivo").val() + "'}",
                            contentType: "application/json; charset=utf-8",
                            dataType: "json",
                            success: function (data, text) {
                                caja.filtrar();
                            },
                            error: function (response) {
                                var r = jQuery.parseJSON(response.responseText);
                                $("#divError").html(r.Message);
                                $("#divError").show();
                                $('html, body').animate({ scrollTop: 0 }, 'slow');
                            }
                        });
                    }
                }
            });
    },
    mostrarPagAnterior: function () {
        var paginaActual = parseInt($("#hdnPage").val());
        paginaActual--;
        $("#hdnPage").val(paginaActual);
        caja.filtrar();
    },
    mostrarPagProxima: function () {
        var paginaActual = parseInt($("#hdnPage").val());
        paginaActual++;
        $("#hdnPage").val(paginaActual);
        caja.filtrar();
    },
    resetearPagina: function () {
        $("#hdnPage").val("1");
    },
    filtrar: function () {
        $("#divError").hide();
        if ($('#frmSearch').valid()) {
            $("#resultsContainer").html("");
            var currentPage = parseInt($("#hdnPage").val());

            var info = "{ tipoMovimiento: '" + $("#ddlTipoMovimiento").val()
                       + "', fechaDesde: '" + $("#txtFechaDesde").val()
                       + "', fechaHasta: '" + $("#txtFechaHasta").val()
                       + "', periodo: '" + $("#ddlPeriodo").val()
                       + "', medioDePago: '" + $("#ddlMedioDePago").val()
                       + "', page: " + currentPage + ", pageSize: " + PAGE_SIZE
                       + "}";

            $.ajax({
                type: "POST",
                url: "/modulos/tesoreria/caja.aspx/getResults",
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

                        $("#spTotalSinConsolidar").html("Saldo total actual: $" + data.d.TotalSinConsolidar);
                        $("#hdnSaldoTotalActual").val(data.d.TotalSinConsolidar);
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
                    $("#msgError").html(r.Message);
                    $("#divError").show();
                    $("#divOk").hide();
                    $('html, body').animate({ scrollTop: 0 }, 'slow');
                }
            });
            caja.resetearExportacion();
        }
    },
    verTodos: function () {
        $("#txtFechaDesde, #txtFechaHasta").val("");
        $("#ddlTipoMovimiento").val("");//.trigger("change");
        caja.filtrar();
    },
    exportar: function () {
        if ($('#frmSearch').valid()) {

            caja.resetearExportacion();
            $("#imgLoading").show();
            $("#divIconoDescargar").hide();

            var info = "{ tipoMovimiento: '" + $("#ddlTipoMovimiento").val()
                        + "', fechaDesde: '" + $("#txtFechaDesde").val()
                        + "', fechaHasta: '" + $("#txtFechaHasta").val()
                        + "', periodo: '" + $("#ddlPeriodo").val()
                        + "', medioDePago: '" + $("#ddlMedioDePago").val()
                       + "'}";

            $.ajax({
                type: "POST",
                url: "/modulos/tesoreria/caja.aspx/export",
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
                    caja.resetearExportacion();
                }
            });
        }
    },
    resetearExportacion: function () {
        $("#imgLoading, #lnkDownload").hide();
        $("#divIconoDescargar").show();
    },
    imprimir: function () {
        $("#divError").hide();
        if ($('#frmSearch').valid()) {
            $("#resultsContainer").html("");
            var currentPage = parseInt($("#hdnPage").val());

            var info = "{ tipoMovimiento: '" + $("#ddlTipoMovimiento").val()
                + "', fechaDesde: '" + $("#txtFechaDesde").val()
                + "', fechaHasta: '" + $("#txtFechaHasta").val()
                + "', periodo: '" + $("#ddlPeriodo").val()
                + "', medioDePago: '" + $("#ddlMedioDePago").val()
                + "', page: " + currentPage + ", pageSize: " + PAGE_SIZE
                + "}";

            $.ajax({
                type: "POST",
                url: "/modulos/tesoreria/caja.aspx/imprimir",
                data: info,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data, text) {
                    var pathFile = "/files/caja/" + $("#hdnIDUsuario").val() + "/" + data.d + "#view=FitH,top";
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
            caja.filtrar();
        }
    },
    cerrarCajas: function () {
        $("#msgError").html("");
        $("#divError").hide();
        $.ajax({
            type: "POST",
            url: "/modulos/tesoreria/caja.aspx/ObtenerTotalSinConsolidar",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data, text) {

                if (data.d != "0") {
                    caja.consolidarCajas(data.d);
                }
                else {
                    $("#msgError").html("No tiene saldos que consolidar.");
                    $("#divError").show();
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
    },

    consolidarCajas: function (importe) {
        bootbox.confirm("Está a punto de cerrar la caja por lo que todos los movimientos hasta la fecha que no han sido conciliados NO se podrán editar."
                        + "<br/><h4>El importe total actual es: $" + $("#hdnSaldoTotalActual").val()
                        + "</h4> <h4>El importe total sin consolidar es: $" + importe + "</h4> ¿Desea continuar?", function (result) {
                            if (result) {
                                $("#divError").hide();

                                $.ajax({
                                    type: "GET",
                                    url: "/modulos/tesoreria/caja.aspx/cerrarCajas",
                                    contentType: "application/json; charset=utf-8",
                                    dataType: "json",
                                    success: function (data, text) {
                                        $("#divOk").show();
                                        caja.filtrar();
                                    },
                                    error: function (response) {
                                        var r = jQuery.parseJSON(response.responseText);
                                        $("#msgError").html(r.Message);
                                        $("#divError").show();
                                        $("#divOk").hide();
                                        $('html, body').animate({ scrollTop: 0 }, 'slow');
                                    }
                                });
                                caja.resetearExportacion();
                            }
                        });
    },
    otroPeriodo: function () {
        if ($("#ddlPeriodo").val() == "-1")
            $('#divMasFiltros').toggle(600);
        else {
            if ($("#divMasFiltros").is(":visible"))
                $('#divMasFiltros').toggle(600);

            $("#txtFechaDesde,#txtFechaHasta").val("");
            caja.filtrar();
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
}
/*** Adjuntar Foto ***/
var foto = {
    adjuntarFoto: function () {

        $("#divEliminar,#divModificar").hide();
        $("#divSeleccionar").show();

        $('#flpArchivo').fileupload({
            url: "/subirImagenes.ashx?idCaja=" + $("#hdnID").val() + "&opcionUpload=Caja",
            success: function (response, status) {
                if (response == "OK") {
                    $("#divError").hide();
                    $("#divOk").show();
                    window.location.href = "/modulos/tesoreria/caja.aspx";
                }
                else {
                    $("#hdnFileName").val("");
                    $("#msgErrorAlta").html(response);
                    $("#divErrorAlta").show();
                    $("#divOk").hide();
                }
            },
            error: function (error) {
                $("#hdnFileName").val("");
                $("#msgErrorAlta").html(error.responseText);
                $("#divErrorAlta").show();
                $("#divOk").hide();
                $('html, body').animate({ scrollTop: 0 }, 'slow');
                Common.ocultarProcesando("btnActualizar", "Aceptar");
            },
            autoUpload: false,
            add: function (e, data) {
                $("#hdnSinCombioDeFoto").val("1");
                $("#btnActualizar").on("click", function () {

                    if ($("#hdnSinCombioDeFoto").val() == "1") {
                        caja.grabar();
                        if ($("#hdnID").val() != "0") {
                            data.url = "/subirImagenes.ashx?idCaja=" + $("#hdnID").val() + "&opcionUpload=Caja";
                            data.submit();
                        }
                    }
                });
            },
        });

        foto.showBtnEliminar();
    },
    showInputFoto: function () {
        $("#divLogo").slideToggle();
    },
    grabarsinImagen: function () {
        if ($("#hdnSinCombioDeFoto").val() == "0") {
            caja.grabar();
        }
    },
    eliminarFoto: function () {
        var id = ($("#hdnID").val() == "" ? "0" : $("#hdnID").val());
        if (id != "") {
            var info = "{ id: " + parseInt(id) + "}";

            $.ajax({
                type: "POST",
                url: "/modulos/tesoreria/cajase.aspx/eliminarFoto",
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
                    $("#hdnSinCombioDeFoto").val("0");
                    // $('#flpArchivo').fileupload('destroy');
                    foto.showBtnEliminar();


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
            $("#divDescarga").show();
        }
        else {
            $("#divEliminarFoto").hide();
            $("#divAdjuntarFoto").removeClass("col-sm-6").addClass("col-sm-12");
            $("#divDescarga").hide();
        }
    },
}