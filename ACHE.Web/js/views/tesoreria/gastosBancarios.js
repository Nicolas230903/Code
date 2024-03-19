var gastosBancarios = {
    /*** FORM ***/
    grabar: function () {
        $("#divError").hide();
        $("#divOk").hide();

        var id = ($("#hdnID").val() == "" ? "0" : $("#hdnID").val());
        if ($('#frmEdicion').valid()) {
            Common.mostrarProcesando("btnActualizar");
            var info = "{ id: " + parseInt(id)
                    + ", IdBanco: " + $("#ddlBanco").val()
                     + ", fecha: '" + $("#txtFecha").val()
                    + "', importe: '" + $("#txtImporte").val()
                    + "', iva: '" + $("#txtIVA").val()
                    + "', debito: '" + $("#txtDebito").val()
                    + "', credito: '" + $("#txtCredito").val()
                    + "', IIBB: '" + $("#txtIIBB").val()
                    + "', otros: '" + $("#txtOtros").val()
                    + "', Importe21: '" + $("#txtImporte21").val()
                    + "', creditoComputable: '" + $("#txtCreditoComputable").val()
                    + "', concepto: '" + $("#txtConcepto").val()
                    + "', percepcionIVA: '" + $("#txtPercepcionIVA").val()
                    + "', SIRCREB: '" + $("#txtSIRCREB").val()
                    + "', Importe10: '" + $("#txtImporte10").val()
                    + "'}";

            $.ajax({
                type: "POST",
                url: "/modulos/tesoreria/gastosBancariose.aspx/guardar",
                data: info,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data, text) {
                    $('#divOk').show();
                    $("#divError").hide();
                    $('html, body').animate({ scrollTop: 0 }, 'slow');
                    window.location.href = "/modulos/tesoreria/gastosBancarios.aspx";
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
        window.location.href = "/modulos/tesoreria/gastosBancarios.aspx";
    },

    configForm: function () {
       
        // Date Picker
        Common.configDatePicker();
        Common.configFechasDesdeHasta("txtFecha");

        $("#txtImporte,#txtIVA,#txtDebito,#txtCredito,#txtIIBB,#txtOtros,#txtImporte21,#txtCreditoComputable").maskMoney({ thousands: '', decimal: '.', allowZero: true });
        $("#txtPercepcionIVA,#txtSIRCREB,#txtImporte10").maskMoney({ thousands: '', decimal: '.', allowZero: true });

        if ($("#txtImporte").val() == "") {
            $("#txtImporte").val("0.00");
        }
        if ($("#txtIVA").val() == "") {
            $("#txtIVA").val("0.00");
        }
        if ($("#txtDebito").val() == "") {
            $("#txtDebito").val("0.00");
        }
        if ($("#txtCredito").val() == "") {
            $("#txtCredito").val("0.00");
        }
        if ($("#txtIIBB").val() == "") {
            $("#txtIIBB").val("0.00");
        }
        if ($("#txtOtros").val() == "") {
            $("#txtOtros").val("0.00");
        }
        if ($("#txtImporte21").val() == "") {
            $("#txtImporte21").val("0.00");
        }
        if ($("#txtCreditoComputable").val() == "") {
            $("#txtCreditoComputable").val("0.00");
        }

        if ($("#txtPercepcionIVA").val() == "") {
            $("#txtPercepcionIVA").val("0.00");
        }
        if ($("#txtSIRCREB").val() == "") {
            $("#txtSIRCREB").val("0.00");
        }
        if ($("#txtImporte10").val() == "") {
            $("#txtImporte10").val("0.00");
        }
       
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
    },
    changeTotal: function () {
        var total = 0;

        if ($("#txtImporte").val() != "") {
            total += parseFloat($("#txtImporte").val());
        }
        if ($("#txtIVA").val() != "") {
            total += parseFloat($("#txtIVA").val());
        }
        if ($("#txtDebito").val() != "") {
            total += parseFloat($("#txtDebito").val());
        }
        if ($("#txtCredito").val() != "") {
            total += parseFloat($("#txtCredito").val());
        }
        if ($("#txtIIBB").val() != "") {
            total += parseFloat($("#txtIIBB").val());
        }
        if ($("#txtOtros").val() != "") {
            total += parseFloat($("#txtOtros").val());
        }
        if ($("#txtImporte21").val() != "") {
            total += parseFloat($("#txtImporte21").val());
        }
        if ($("#txtCreditoComputable").val() != "") {
            total += parseFloat($("#txtCreditoComputable").val());
        }


        if ($("#txtPercepcionIVA").val() != "") {
            total += parseFloat($("#txtPercepcionIVA").val());
        }
        if ($("#txtSIRCREB").val() != "") {
            total += parseFloat($("#txtSIRCREB").val());
        }
        if ($("#txtImporte10").val() != "") {
            total += parseFloat($("#txtImporte10").val());
        }
        
        $("#divTotal").html("$ " + total.toFixed(2));
    },
    /*** SEARCH ***/
    configFilters: function () {
        // Date Picker
        Common.configDatePicker();
        Common.configFechasDesdeHasta("txtFechaDesde", "txtFechaHasta");

        $("#ddlBanco").keypress(function (event) {
            var keycode = (event.keyCode ? event.keyCode : event.which);
            if (keycode == '13') {
                gastosBancarios.resetearPagina();
                gastosBancarios.filtrar();
                return false;
            }
        });

        $(".select2").select2({ width: '100%', allowClear: true });

    },

    nuevo: function () {
        window.location.href = "/modulos/tesoreria/gastosBancariose.aspx";
    },

    editar: function (id) {
        window.location.href = "/modulos/tesoreria/gastosBancariose.aspx?ID=" + id;
    },

    eliminar: function (id, nombre) {
        bootbox.confirm("¿Está seguro que desea eliminar el gasto bancario del Banco " + nombre + "?", function (result) {
            if (result) {
                $.ajax({
                    type: "POST",
                    url: "/modulos/tesoreria/gastosBancarios.aspx/delete",
                    data: "{ id: " + id + "}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data, text) {
                        gastosBancarios.filtrar();
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
        gastosBancarios.filtrar();
    },

    mostrarPagProxima: function () {
        var paginaActual = parseInt($("#hdnPage").val());
        paginaActual++;
        $("#hdnPage").val(paginaActual);
        gastosBancarios.filtrar();
    },

    resetearPagina: function () {
        $("#hdnPage").val("1");
    },

    filtrar: function () {

        $("#divError").hide();
        $("#resultsContainer").html("");
        var currentPage = parseInt($("#hdnPage").val());

        var info = "{ idBanco: " + parseInt($("#ddlBanco").val())
                   + ", fechaDesde: '" + $("#txtFechaDesde").val()
                   + "', fechaHasta: '" + $("#txtFechaHasta").val()
                   + "', periodo: '" + $("#ddlPeriodo").val()
                   + "', page: " + currentPage + ", pageSize: " + PAGE_SIZE
                   + "}";

        $.ajax({
            type: "POST",
            url: "/modulos/tesoreria/gastosBancarios.aspx/getResults",
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
        gastosBancarios.resetearExportacion();
    },

    verTodos: function () {
        $("#ddlBanco").val("-1").trigger("change");
        $("#txtFechaDesde").val("");
        $("#txtFechaHasta").val("");
        gastosBancarios.filtrar();
    },

    exportar: function () {
        gastosBancarios.resetearExportacion();
        $("#imgLoading").show();
        $("#divIconoDescargar").hide();

        var info = "{ idBanco: " + parseInt($("#ddlBanco").val())
                                 + ", fechaDesde: '" + $("#txtFechaDesde").val()
                                 + "', fechaHasta: '" + $("#txtFechaHasta").val()
                                  + "', periodo: '" + $("#ddlPeriodo").val()
                                 + "'}";

        $.ajax({
            type: "POST",
            url: "/modulos/tesoreria/gastosBancarios.aspx/export",
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
                gastosBancarios.resetearExportacion();
            }
        });
    },

    resetearExportacion: function () {
        $("#imgLoading, #lnkDownload").hide();
        $("#divIconoDescargar").show();
    },
    otroPeriodo: function () {
        if ($("#ddlPeriodo").val() == "-1")
            $('#divMasFiltros').toggle(600);
        else {
            if ($("#divMasFiltros").is(":visible"))
                $('#divMasFiltros').toggle(600);

            $("#txtFechaDesde,#txtFechaHasta").val("");
            gastosBancarios.filtrar();
        }
    },
}