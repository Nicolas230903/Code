var cheques = {
    /*** FORM ***/
    grabar: function () {
        $("#divError").hide();
        $("#divOk").hide();

        var id = ($("#hdnID").val() == "" ? "0" : $("#hdnID").val());
        var idChequePersona = 0;

        if ($('#frmEdicion').valid()) {
            Common.mostrarProcesando("actualizarCheque");
            if (0 == parseInt($("#txtImporte").val())) {
                $("#msgError").html("El importe debve ser mayor a 0");
                $("#divError").show();
                $("#divOk").hide();
                $('html, body').animate({ scrollTop: 0 }, 'slow');
                return false;
            }

            if (!esPropio && !esPropioEmpresa) {
                if ($("#ddlPersona").val() == "" || $("#ddlPersona").val() == null) {
                    $("#msgError").html("Debe seleccionar un cliente si el cheque es de terceros");
                    $("#divError").show();
                    $("#divOk").hide();
                    $('html, body').animate({ scrollTop: 0 }, 'slow');
                    return false;
                } else {
                    idChequePersona = $("#ddlPersona").val();
                }
            } 

            var esPropio = ($("#ddlEsPropio").val() == "S" ? true : false);
            var esPropioEmpresa = ($("#ddlEsPropioEmpresa").val() == "S" ? true : false);

            var info = "{ id: " + parseInt(id)
                    + ", idBanco: " + $("#ddlBancos").val()
                    + ", numero: '" + $("#txtNumero").val()
                    + "', importe: '" + $("#txtImporte").val()
                    + "', fechaEmision: '" + $("#txtFechaEmision").val()
                    + "', fechaCobro: '" + $("#txtFechaCobrar").val()
                    + "', fechaVencimiento: '" + $("#txtFechaVencimiento").val()
                    + "', estado: '" + $("#ddlEstado").val()
                    + "', emisor: '" + $("#txtEmisor").val()
                    + "', observaciones: '" + $("#txtObservaciones").val()
                + "', esPropio: " + esPropio
                + " ,idChequePersona: " + idChequePersona
                + ", cuit: '" + $("#txtCUIT").val()
                + "' ,esPropioEmpresa: " + esPropioEmpresa
                + "}";


            $.ajax({
                type: "POST",
                url: "/modulos/tesoreria/chequese.aspx/guardar",
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
                        window.location.href = "/modulos/Tesoreria/cheques.aspx";
                    }
                },
                error: function (response) {
                    var r = jQuery.parseJSON(response.responseText);
                    $("#msgError").html(r.Message);
                    $("#divError").show();
                    $("#divOk").hide();
                    $('html, body').animate({ scrollTop: 0 }, 'slow');
                    Common.ocultarProcesando("actualizarCheque", "Aceptar");
                }
            });
        }
        else {
            return false;
        }
    },
    cancelar: function () {
        window.location.href = "/modulos/Tesoreria/cheques.aspx";
    },
    configForm: function () {

        $(".select2").select2({
            width: '100%', allowClear: true
        });

        $("#txtNumero").numericInput();

        $("#txtImporte").maskMoney({ thousands: '', decimal: ',', allowZero: true });

        // Date Picker
        $('#txtFechaEmision,#txtFechaCobrar,#txtFechaVencimiento').datepicker();
        Common.configDatePicker();
        Common.configFechasDesdeHasta("txtFechaEmision", "txtFechaCobrar");

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
        cheques.adjuntarCheque();
    },
    /*** Adjuntar Foto ***/
    adjuntarCheque: function () {

        $('#flpArchivo').fileupload({
            url: '/subirImagenes.ashx?idCheque=' + $("#hdnID").val() + "&opcionUpload=cheques" + "&IDUsuario=" + $("#Idusuario").val(),
            success: function (response, status) {
                if (response == "OK") {
                    $("#divError").hide();
                    $("#divOk").show();
                    Common.ocultarProcesando("actualizarCheque", "Aceptar");
                    window.location.href = "/modulos/Tesoreria/cheques.aspx";
                }
                else {
                    $("#hdnFileName").val("");
                    $("#msgError").html(response);
                    $("#divError").show();
                    $("#divOk").hide();
                    Common.ocultarProcesando("actualizarCheque", "Aceptar");
                }
            },
            error: function (error) {
                $("#hdnFileName").val("");
                $("#msgError").html(error.responseText);
                $("#imgLoading").hide();
                $("#divError").show();
                $("#divOk").hide();

                $('html, body').animate({ scrollTop: 0 }, 'slow');

            },
            autoUpload: false,
            add: function (e, data) {
                $("#hdnSinCombioDeFoto").val("1");
                $("#actualizarCheque").on("click", function () {
                    $("#imgLoading").show();
                    cheques.grabar();
                    if ($("#hdnID").val() != "0") {
                        data.url = '/subirImagenes.ashx?idCheque=' + $("#hdnID").val() + "&opcionUpload=cheques" + "&IDUsuario=" + $("#Idusuario").val();
                        data.submit();
                    }
                })
            }
        });
        cheques.showBtnEliminar();
    },
    showInputLogo: function () {
        $("#divLogo").slideToggle();
    },
    grabarsinImagen: function () {
        if ($("#hdnSinCombioDeFoto").val() == "0") {
            cheques.grabar();
        }
    },
    eliminarFotoCheque: function () {
        var id = ($("#hdnID").val() == "" ? "0" : $("#hdnID").val());
        if (id != "") {
            var info = "{ idCheque: " + parseInt(id) + "}";

            $.ajax({
                type: "POST",
                url: "chequese.aspx/eliminarFoto",
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
                    cheques.showBtnEliminar();
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
    /*** SEARCH ***/
    configFilters: function () {
        $("#txtNroCheque").numericInput();

        $("#txtCondicion").keypress(function (event) {
            var keycode = (event.keyCode ? event.keyCode : event.which);
            if (keycode == '13') {
                cheques.resetearPagina();
                cheques.filtrar();
                return false;
            }
        });
    },
    nuevo: function () {
        window.location.href = "/modulos/tesoreria/chequese.aspx";
    },
    editar: function (id) {
        window.location.href = "/modulos/tesoreria/chequese.aspx?ID=" + id;
    },
    eliminar: function (id, nombre) {
        bootbox.confirm("¿Está seguro que desea eliminar el cheque numero: " + nombre + "?", function (result) {
            if (result) {
                $.ajax({
                    type: "POST",
                    url: "/modulos/Tesoreria/cheques.aspx/delete",
                    data: "{ id: " + id + "}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data, text) {
                        cheques.filtrar();
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
        cheques.filtrar();
    },
    mostrarPagProxima: function () {
        var paginaActual = parseInt($("#hdnPage").val());
        paginaActual++;
        $("#hdnPage").val(paginaActual);
        cheques.filtrar();
    },
    resetearPagina: function () {
        $("#hdnPage").val("1");
    },
    filtrar: function () {

        $("#divError").hide();
        $("#resultsContainer").html("");
        var currentPage = parseInt($("#hdnPage").val());

        var info = "{ condicion: '" + $("#txtCondicion").val()
                + "', page: " + currentPage + ", pageSize: " + PAGE_SIZE
                + " , vencidos: '" + $("#ddlvendicos").val()
                + "' }";
        $.ajax({
            type: "POST",
            url: "/modulos/Tesoreria/cheques.aspx/getResults",
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
        cheques.resetearExportacion();
    },
    verTodos: function () {
        $("#txtNombreBanco, #txtNroCheque,#txtEmisor").val("");
        cheques.filtrar();
    },
    exportar: function () {
        cheques.resetearExportacion();
        $("#imgLoading").show();
        $("#divIconoDescargar").hide();

        var info = "{ condicion: '" + $("#txtCondicion").val()
                   + "'}";

        $.ajax({
            type: "POST",
            url: "/modulos/Tesoreria/cheques.aspx/export",
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
                $("#imgLoading").hide();
                $("#divError").show();
                $('html, body').animate({ scrollTop: 0 }, 'slow');
                cheques.resetearExportacion();
            }
        });
    },
    resetearExportacion: function () {
        $("#imgLoading, #lnkDownload").hide();
        $("#divIconoDescargar").show();
    },
    /*** MODAL  **/
    configFormModal: function () {
        $("#divErrorCheque,#divOkCheque").hide();
        $("#txtNumeroCheque,#txtCUIT").numericInput();
        $("#txtImporteCheque").maskMoney({ thousands: '', decimal: ',', allowZero: true });

        //$("#txtFechaCobrarModal").attr("onblur", "changeFechaCobro()");
        // Date Picker
        $('#txtFechaEmisionModal,#txtFechaCobrarModal,#txtFechaVencimientoModal').datepicker();
        Common.configDatePicker();
        Common.configFechasDesdeHasta("txtFechaEmisionModal", "txtFechaCobrarModal");
        //Common.obtenerClientes("ddlChequePersona", $("#hdnIDChequePersona").val(), false);
        Common.obtenerPersonas("ddlChequePersona", $("#hdnIDChequePersona").val(), false);

        // Validation with select boxes
        $("#frmNuenoCheque").validate({
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

        $('#flpArchivo').fileupload({
            url: '/subirImagenes.ashx?idCheque=' + $("#hdnIDCheque").val() + "&opcionUpload=cheques",
            success: function (response, status) {
                if (response == "OK") {
                    $("#divErrorCheque").hide();
                    $("#divOkCheque").show();
                    $('#modalNuevoCheque').modal('hide');
                }
                else {
                    $("#hdnFileName").val("");
                    $("#msgErrorCheque").html(response);
                    $("#divErrorCheque").show();
                    $("#divOkCheque").hide();
                    Common.ocultarProcesando("actualizarCheque", "Aceptar");
                }
            },
            error: function (error) {
                $("#hdnFileName").val("");
                $("#msgErrorCheque").html(error.responseText);
                $("#divErrorCheque").show();
                $("#divOkCheque").hide();
                $('html, body').animate({ scrollTop: 0 }, 'slow');

            },
            autoUpload: false
            ,
            add: function (e, data) {
                $("#hdnSinCombioDeFoto").val("1");
                $("#actualizarCheque").on("click", function () {
                    cheques.grabarModal();
                    data.url = '/subirImagenes.ashx?idCheque=' + $("#hdnIDCheque").val() + "&opcionUpload=cheques";
                    if ($("#hdnIDCheque").val() != "0") {
                        data.submit();
                    }
                })
            }
        });
    },
    grabarModal: function () {
        $("#divErrorCheque").hide();
        $("#divOkCheque").hide();


        //var esPropio = ($("#hdnChequePropio").val() == "1") ? true : false;
        var id = ($("#hdnIDCheque").val() == "" ? "0" : $("#hdnIDCheque").val());

        if ($('#txtNumeroCheque,#txtImporteCheque,#txtFechaEmisionModal,#txtFechaCobrarModal,#ddlBancosCheque,#ddlEstadoCheque').valid()) {
            if (0 == parseInt($("#txtImporteCheque").val())) {
                $("#msgErrorCheque").html("El importe debe ser mayor a 0");
                $("#divErrorCheque").show();
                $("#divOkCheque").hide();
                $('html, body').animate({ scrollTop: 0 }, 'slow');
                return false;
            }

            var esPropio = ($("#ddlEsPropio").val() == "S" ? true : false);
            var esPropioEmpresa = ($("#ddlEsPropioEmpresa").val() == "S" ? true : false);
            var idChequePersona = 0;

            if ($("#txtCUIT").val() == "" || $("#txtCUIT").val() == null) {
                $("#msgErrorCheque").html("Debe ingresar un CUIT");
                $("#divErrorCheque").show();
                $("#divOkCheque").hide();
                $('html, body').animate({ scrollTop: 0 }, 'slow');
                return false;
            }

            if ($("#txtEmisorCheque").val() == "" || $("#txtEmisorCheque").val() == null) {
                $("#msgErrorCheque").html("Debe ingresar un EMISOR");
                $("#divErrorCheque").show();
                $("#divOkCheque").hide();
                $('html, body').animate({ scrollTop: 0 }, 'slow');
                return false;
            }            

            if (!esPropio && !esPropioEmpresa) {
                if ($("#ddlChequePersona").val() == "" || $("#ddlChequePersona").val() == null) {
                    $("#msgErrorCheque").html("Debe seleccionar un cliente si el cheque es de terceros");
                    $("#divErrorCheque").show();
                    $("#divOkCheque").hide();
                    $('html, body').animate({ scrollTop: 0 }, 'slow');
                    return false;
                } else {
                    idChequePersona = $("#ddlChequePersona").val();
                }
            } 

            var info = "{ id: 0"
                    + ", idBanco: '" + $("#ddlBancosCheque").val()
                    + "', numero: '" + $("#txtNumeroCheque").val()
                    + "', importe: '" + $("#txtImporteCheque").val()
                    + "', fechaEmision: '" + $("#txtFechaEmisionModal").val()
                    + "', fechaCobro: '" + $("#txtFechaCobrarModal").val()
                    + "', fechaVencimiento: '" + $("#txtFechaVencimientoModal").val()
                    + "', estado: '" + "Libre"
                    + "', emisor: '" + $("#txtEmisorCheque").val()
                    + "', observaciones: '" + $("#txtObservacionesCheque").val()
                    + "', esPropio: " + esPropio
                    + " ,idChequePersona: " + idChequePersona
                    + ", cuit: '" + $("#txtCUIT").val()
                    + "', esPropioEmpresa: " + esPropioEmpresa
                    + "}";

            $.ajax({
                type: "POST",
                url: "/modulos/tesoreria/chequese.aspx/guardar",
                data: info,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: false,
                success: function (data, text) {
                    $('#divOkCheque').show();
                    $("#divErrorCheque").hide();
                    //$('html, body').animate({ scrollTop: 0 }, 'slow');

                    $("#hdnIDCheque").val(data.d);
                    $("#txtNombreCheque,#txtNumeroCheque,#txtImporteCheque,#txtFechaCheque,#txtEmisorCheque,#txtObservacionesCheque,#hdnFileNameCheque").val("");
                    $("#txtFechaEmisionModal,#txtFechaCobrarModal,#txtFechaVencimientoModal").val("");

                    $('#modalNuevoCheque').modal('hide');

                    if (data.d != 0) {
                        Common.obtenerChequesConResto('ddlCheque', "", true, esPropio);
                        setTimeout(function () {
                            changeChequeTercero();
                        }, 700);
                    }
                },
                error: function (response) {
                    var r = jQuery.parseJSON(response.responseText);
                    $("#msgErrorCheque").html(r.Message);
                    $("#divErrorCheque").show();
                    $("#divOkCheque").hide();
                    //$('html, body').animate({ scrollTop: 0 }, 'slow');
                }
            });
        }
        else {
            return false;
        }
    },
    grabarsinImagenModal: function () {
        if ($("#hdnSinCombioDeFoto").val() == "0") {
            cheques.grabarModal();
        }
    },
    changeFechaCobro: function () {
        var fecha = Common.sumarDiasFecha(30, $("#txtFechaCobrarModal").val());
        $("#txtFechaVencimientoModal").val(fecha);
    },
    changeEsPropio: function () {
        if ($("#hdnModo").val() == "COBRANZA") {
            if ($("#ddlEsPropio").val() == "S") {
                $("#divChequePersona").hide();
                $("#divChequeEsPropioEmpresa").hide();
                var CuitCliente = $("#ddlPersona option:selected").text().split("-")[0];
                var NombreCliente = $("#ddlPersona option:selected").text().split("-")[1];
                //$("#txtEmisorCheque").val(MI_RAZONSOCIAL);
                //$("#txtCUIT").val(MI_RAZONSOCIAL);
                $("#txtEmisorCheque").val(NombreCliente);
                $("#txtCUIT").val(CuitCliente);
            } else {
                $("#divChequeEsPropioEmpresa").show();
                $("#txtEmisorCheque").val("");
                $("#divChequePersona").show();
                if ($("#ddlPersona").val() != "" || $("#ddlPersona").val() != null) {
                    //var NombreCliente = $("#ddlPersona option:selected").text().split("-")[1];
                    //$("#txtEmisorCheque").val(NombreCliente);
                    $("#txtEmisorCheque").val("");
                    $("#txtCUIT").val("");
                }
            }
        } else {  /// POR PAGOS
            if ($("#ddlEsPropio").val() == "S") {
                $("#divChequeEsPropioEmpresa").show();
            } else {
                $("#divChequeEsPropioEmpresa").hide();
            }
            $("#txtEmisorCheque").val("");
            $("#divChequePersona").show();
            if ($("#ddlPersona").val() != "" || $("#ddlPersona").val() != null) {
                //var NombreCliente = $("#ddlPersona option:selected").text().split("-")[1];
                //$("#txtEmisorCheque").val(NombreCliente);
                $("#txtEmisorCheque").val("");
                $("#txtCUIT").val("");
            }
        }
    },
    changeEsPropioEmpresa: function () {
        if ($("#ddlEsPropioEmpresa").val() == "S") {
            $("#divChequePersona").hide();
            $("#txtEmisorCheque").val(MI_RAZONSOCIAL);
            $("#txtCUIT").val(MI_RAZONSOCIAL);            
        } else {
            $("#txtEmisorCheque").val("");
            $("#divChequePersona").show();
            if ($("#ddlPersona").val() != "" || $("#ddlPersona").val() != null) {
                $("#txtEmisorCheque").val("");
                $("#txtCUIT").val("");
            }
        }
    },
}

var chequesAcciones = {
    /*** FORM ***/
    grabar: function () {
        $("#divErrorAlta").hide();
        $("#divOkAlta").hide();

        if ($("#ddlAcciones").val() == "Depositado") {
            if (!$("#ddlBancos").valid()) {
                return false;
            }
        }

        if ($('#txtFechaAcciones,#ddlAcciones,#ddlCheques').valid()) {
            if ($("#ddlAcciones").val() == "Rechazado") {
                bootbox.confirm("¿Está seguro que desea rechazar el cheque?, una ves rechazado debera corregir de forma manual las cobranzas o pagos realizados con el mismo", function (result) {
                    if (result) {
                        chequesAcciones.guardar();
                    }
                });
            }
            else {
                chequesAcciones.guardar();
            }
        }
        else {
            return false;
        }
    },
    guardar: function () {
        var idCheques = ($("#ddlCheques").val() == "" || $("#ddlCheques").val() == null) ? "" : parseInt($("#ddlCheques").val());
        var idBancos = ($("#ddlBancos").val() == "" || $("#ddlBancos").val() == null) ? "0" : parseInt($("#ddlBancos").val());

        Common.mostrarProcesando("btnActualizar");
        var info = "{ accion: '" + $("#ddlAcciones").val()
                 + "',idCheque: " + idCheques
                 + ", fechaDeposito: '" + $("#txtFechaAcciones").val()
                 + "',idBanco: " + idBancos
                 + "}";
        $.ajax({
            type: "POST",
            url: "/modulos/tesoreria/cheques.aspx/guardarAccion",
            data: info,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data, text) {
                $('#divOkAlta').show();
                $("#divErrorAlta").hide();
                $('html, body').animate({ scrollTop: 0 }, 'slow');
                $('#modalNuevaChequesAcciones').modal('toggle');
                Common.ocultarProcesando("btnActualizar", "Aceptar");
            },
            error: function (response) {
                var r = jQuery.parseJSON(response.responseText);
                $("#msgErrorAlta").html(r.Message);
                $("#divErrorAlta").show();
                $("#divOkAlta").hide();
                $('html, body').animate({ scrollTop: 0 }, 'slow');
                Common.ocultarProcesando("btnActualizar", "Aceptar");
            }
        });
    },
    abrirCalendario: function () {
        $('#ui-datepicker-div').css({ 'z-index': "999999" });
    },
    cancelar: function () {
        window.location.href = "/modulos/Tesoreria/cheques.aspx";
    },
    configForm: function () {
        Common.configDatePicker();
        // Validation with select boxes
        $("#frmNuenaChequesAcciones").validate({
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

        $(".select2").select2({
            width: '100%', allowClear: true
        });
    },
    /*** SEARCH ***/
    limpiarCampos: function () {

        $("#ddlAcciones,#ddlCheques,#ddlBancos,#txtFechaAcciones").val("");
        $("#ddlAcciones,#ddlCheques,#ddlBancos").trigger("change");

        $("#hdnID").val("0");
        $("#divErrorAlta").hide();
        $("#divOkAlta").hide();
        Common.ocultarProcesando("btnActualizar", "Aceptar");
    },
    nuevo: function (accion) {
        chequesAcciones.limpiarCampos();
        $("#ddlAcciones").val(accion);
        $("#ddlAcciones").trigger("change");
        chequesAcciones.setearTitulos(accion);
        chequesAcciones.obtenerChequesSegunAcciones(accion);
        $('#modalNuevaChequesAcciones').modal('toggle');
    },
    setearTitulos: function (accion) {
        switch (accion) {
            case "Rechazado":
                $("#tituloChequeAccion").html("Rechazar cheque");
                $("#subTituloChequeAccion").html("Seleccione el cheque que quiera rechazar");
                $("#spFechaAccion").html("Fecha de rechazo");
                $("#divBancos").hide();
                break;
            case "Depositado":
                $("#tituloChequeAccion").html("Depositar cheque");
                $("#subTituloChequeAccion").html("Seleccione el cheque que quiera depositar");
                $("#spFechaAccion").html("Fecha de depósito");
                $("#divBancos").show();
                break;
            case "Acreditado":
                $("#tituloChequeAccion").html("Acreditar cheque");
                $("#subTituloChequeAccion").html("Seleccione el cheque que quiera Acreditar");
                $("#spFechaAccion").html("Fecha de acreditación");
                $("#divBancos").hide();
                break;
            default:
                break;
        }
    },
    editar: function (id) {
        chequesAcciones.limpiarCampos();
        $("#divErrorAlta").hide();
        $("#divOkAlta").hide();

        var info = "{ id: " + parseInt(id) + "}";

        $.ajax({
            type: "POST",
            url: "/modulos/tesoreria/cheques.aspx/cargarEntidadAccion",
            data: info,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data, text) {
                chequesAcciones.limpiarCampos();
                $('#hdnIDChequesAcciones').val(data.d.ID);
                $("#ddlAcciones").val(data.d.Accion);
                $("#ddlCheques").val(data.d.IDCheque);
                $("#txtFechaAcciones").val(data.d.FechaADepositar);
                $("#ddlCheques,#ddlAcciones").trigger("change");
                $('#modalNuevaChequesAcciones').modal('toggle');
            },
            error: function (response) {
                var r = jQuery.parseJSON(response.responseText);
                $("#msgErrorAlta").html(r.Message);
                $("#divErrorAlta").show();
                $("#divOkAlta").hide();
                $('html, body').animate({ scrollTop: 0 }, 'slow');
            }
        });
    },
    eliminar: function (id, nombre) {
        bootbox.confirm("¿Está seguro que desea eliminar la accion del cheque numero: " + nombre + "?", function (result) {
            if (result) {

                $.ajax({
                    type: "POST",
                    url: "/modulos/Tesoreria/cheques.aspx/deleteAccion",
                    data: "{ id: " + id + "}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data, text) {
                        chequesAcciones.filtrar();
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
        $("#resultsContainerAcciones").html("");
        var currentPage = parseInt($("#hdnPage").val());

        var info = "{ idCheque: " + parseInt($('#hdnIDChequesAcciones').val())
                   + ", page: " + currentPage + ", pageSize: " + PAGE_SIZE
            + "}";


        $.ajax({
            type: "POST",
            url: "/modulos/Tesoreria/cheques.aspx/getResultsAccion",
            data: info,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data, text) {
                if (data.d.Items.length > 0)
                    $("#resultTemplateAcciones").tmpl({ results: data.d.Items }).appendTo("#resultsContainerAcciones");
                else
                    $("#noResultTemplateAcciones").tmpl({ results: data.d.Items }).appendTo("#resultsContainerAcciones");
            },
            error: function (response) {
                var r = jQuery.parseJSON(response.responseText);
                alert(r.Message);
            }
        });
    },
    verDetalle: function (id) {
        $('#hdnIDChequesAcciones').val(id);
        chequesAcciones.filtrar();
        $("#modalDetalleAcciones").modal("toggle");
    },
    obtenerChequesSegunAcciones: function (accion) {
        $.ajax({
            type: "POST",
            url: "/modulos/Tesoreria/cheques.aspx/obtenerChequesSegunAcciones",
            data: "{ accion: '" + accion + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                if (data != null) {
                    $("#ddlCheques").html("");

                    $("<option/>").attr("value", "").text("").appendTo($("#ddlCheques"));

                    for (var i = 0; i < data.d.length; i++) {
                        $("<option/>").attr("value", data.d[i].ID).text(data.d[i].Nombre).appendTo($("#ddlCheques"));
                    }

                    $("#ddlCheques").val("").trigger("change");
                }
            }
        });
    },
}