var MovimientoDeFondos = {
    /*** FORM ***/
    grabar: function () {
        $("#divError").hide();
        $("#divOk").hide();
        var id = ($("#hdnID").val() == "" ? "0" : $("#hdnID").val());

        if (parseFloat($("#txtImporte").val()) == 0) {
            $("#txtImporte").val("");
            $("#txtImporte").valid();
        }
        
        if ($('#frmEdicion').valid()) {
            Common.mostrarProcesando("btnActualizar");
            var info = "{ id: " + parseInt(id)
                    + " , idCuentaOrigen: '" + $("#ddlPlanDeCuentaOrigen").val()
                    + "', idCuentaDestino: '" + $("#ddlPlanDeCuentaDestino").val()
                    + "', importe: '" + $("#txtImporte").val().replace('.','').replace(',','.')
                    + "', fechaMovimiento: '" + $("#txtFechaMovimiento").val()
                    + "', observaciones: '" + $("#txtObservaciones").val()
                    + " '}";
            $.ajax({
                type: "POST",
                url: "/modulos/tesoreria/movimientoDeFondose.aspx/guardar",
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
                        window.location.href = "/modulos/Tesoreria/movimientoDeFondos.aspx";
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
            return false;
        }
    },
    cancelar: function () {
        window.location.href = "/modulos/Tesoreria/movimientoDeFondos.aspx";
    },
    configForm: function () {
        $("#txtImporte").maskMoney({ thousands: '', decimal: ',', allowZero: true });
        $(".select2").select2({ width: '100%', allowClear: true });

        // Date Picker
        $('#txtFechaMovimiento').datepicker();
        Common.configDatePicker();

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

        if ($("#hdnFileName").val() != "") {
            $(".fileinput-filename").html($("#hdnFileName").val());
            $("#iImgFileFoto").show();
        }
        MovimientoDeFondos.adjuntarFoto();
    },
    /*** SEARCH ***/
    configFilters: function () {

        // Date Picker
        Common.configDatePicker();
        Common.configFechasDesdeHasta("txtFechaDesde", "txtFechaHasta");

        $("#txtNroCuenta").numericInput();
        $(".select2").select2({ width: '100%', allowClear: true });
        $("#txtCondicion").keypress(function (event) {
            var keycode = (event.keyCode ? event.keyCode : event.which);
            if (keycode == '13') {
                MovimientoDeFondos.resetearPagina();
                MovimientoDeFondos.filtrar();
                return false;
            }
        });
    },
    nuevo: function () {
        window.location.href = "/modulos/tesoreria/movimientoDeFondose.aspx";
    },
    editar: function (id) {
        window.location.href = "/modulos/tesoreria/movimientoDeFondose.aspx?ID=" + id;
    },
    eliminar: function (id, nombre) {
        bootbox.confirm("¿Está seguro que desea eliminar el movimiento de fondo?", function (result) {
            if (result) {
                $.ajax({
                    type: "POST",
                    url: "/modulos/Tesoreria/movimientoDeFondos.aspx/delete",
                    data: "{ id: " + id + "}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data, text) {
                        MovimientoDeFondos.filtrar();
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
        MovimientoDeFondos.filtrar();
    },
    mostrarPagProxima: function () {
        var paginaActual = parseInt($("#hdnPage").val());
        paginaActual++;
        $("#hdnPage").val(paginaActual);
        MovimientoDeFondos.filtrar();
    },
    resetearPagina: function () {
        $("#hdnPage").val("1");
    },
    filtrar: function () {

        $("#divError").hide();
        $("#resultsContainer").html("");
        var currentPage = parseInt($("#hdnPage").val());
        var info = "{ condicion: '" + $("#txtCondicion").val()
                   + "', periodo: '" + $("#ddlPeriodo").val()
                   + "', fechaDesde: '" + $("#txtFechaDesde").val()
                   + "', fechaHasta: '" + $("#txtFechaHasta").val()
                   + "', page: " + currentPage + ", pageSize: " + PAGE_SIZE
                   + "}";

        $.ajax({
            type: "POST",
            url: "/modulos/Tesoreria/movimientoDeFondos.aspx/getResults",
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
        MovimientoDeFondos.resetearExportacion();
    },
    verTodos: function () {
        $("#txtNombreBanco, #txtNroCuenta").val("");
        MovimientoDeFondos.filtrar();
    },
    exportar: function () {
        MovimientoDeFondos.resetearExportacion();
        $("#imgLoading").show();
        $("#divIconoDescargar").hide();

        var info = "{ condicion: '" + $("#txtCondicion").val()
                   + "', periodo: '" + $("#ddlPeriodo").val()
                   + "', fechaDesde: '" + $("#txtFechaDesde").val()
                   + "', fechaHasta: '" + $("#txtFechaHasta").val()
                   + "'}";

        $.ajax({
            type: "POST",
            url: "/modulos/Tesoreria/movimientoDeFondos.aspx/export",
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
                MovimientoDeFondos.resetearExportacion();
            }
        });
    },
    resetearExportacion: function () {
        $("#imgLoading, #lnkDownload").hide();
        $("#divIconoDescargar").show();
    },
    otroPeriodo:function() {
        if ($("#ddlPeriodo").val() == "-1")
            $('#divMasFiltros').toggle(600);
        else {
            if ($("#divMasFiltros").is(":visible"))
                $('#divMasFiltros').toggle(600);

            $("#txtFechaDesde,#txtFechaHasta").val("");
            MovimientoDeFondos.filtrar();
        }
    },
    /*** Adjuntar Foto ***/
    adjuntarFoto: function () {
        $('#flpArchivo').fileupload({
            url: "/subirImagenes.ashx?idMovimientoDeFondos=" + $("#hdnID").val() + "&opcionUpload=MovimientoDeFondos",
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
                    MovimientoDeFondos.grabar();
                    if ($("#hdnID").val() != "0") {
                        data.url = "/subirImagenes.ashx?idMovimientoDeFondos=" + $("#hdnID").val() + "&opcionUpload=MovimientoDeFondos";
                        data.submit();
                        window.location.href = "/modulos/tesoreria/MovimientoDeFondos.aspx";
                    }
                })
            }
        });
        MovimientoDeFondos.showBtnEliminar();
    },
    showInputFoto: function () {
        $("#divLogo").slideToggle();
    },
    grabarsinImagen: function () {
        if ($("#hdnSinCombioDeFoto").val() == "0") {
            MovimientoDeFondos.grabar();
        }
    },
    eliminarFoto: function () {
        var id = ($("#hdnID").val() == "" ? "0" : $("#hdnID").val());
        if (id != "") {
            var info = "{ idMovimiento: " + parseInt(id) + "}";

            $.ajax({
                type: "POST",
                url: "MovimientoDeFondose.aspx/eliminarFoto",
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
                    MovimientoDeFondos.showBtnEliminar();
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
}