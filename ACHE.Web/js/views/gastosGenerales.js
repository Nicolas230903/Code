var GastosGenerales = {
    /*** FORM ***/
    configForm: function () {

        Common.configDatePicker();
        
        $("#txtFechaPeriodo, #txtSueldos, #txtSeguridadEHigiene, #txtMunicipales, #txtMonotributos, #txtAportesYContribuciones").numericInput();
        $("#txtGanancias12, #txtCreditoBancario, #txtRetencionesDeIIBB, #txtPlanesAFIP, #txtGastos1").numericInput();
        $("#txtGastos2, #txtGastos3, #txtF1, #txtF2").numericInput();

        $("#txtSueldos, #txtSeguridadEHigiene, #txtMunicipales, #txtMonotributos, #txtAportesYContribuciones").maskMoney({ thousands: '', decimal: ',', allowZero: true });//, #txtRedondeo
        $("#txtGanancias12, #txtCreditoBancario, #txtRetencionesDeIIBB, #txtPlanesAFIP, #txtGastos1").maskMoney({ thousands: '', decimal: ',', allowZero: true });//, #txtRedondeo
        $("#txtGastos2, #txtGastos3, #txtF1, #txtF2").maskMoney({ thousands: '', decimal: ',', allowZero: true });//, #txtRedondeo

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
    grabar: function () {
        GastosGenerales.ocultarMensajes();
        
        if ($("#txtFechaPeriodo").val() == "") {
            $("#msgError").html("El campo periodo es obligatorio");
            $("#divError").show();
            $("#divOk").hide();
            $('html, body').animate({ scrollTop: 0 }, 'slow');
            Common.ocultarProcesando("btnActualizar", "Aceptar");
        } else {
            if (($("#txtFechaPeriodo").val()).length != 6) {
                $("#msgError").html("El campo periodo debe tener 6 numeros");
                $("#divError").show();
                $("#divOk").hide();
                $('html, body').animate({ scrollTop: 0 }, 'slow');
                Common.ocultarProcesando("btnActualizar", "Aceptar");
            } else {
                var id = ($("#hdnID").val() == "" ? "0" : $("#hdnID").val());

                if ($('#frmEdicion').valid()) {
                    Common.mostrarProcesando("btnActualizar");

                    var info = "{ id: " + parseInt(id)
                        + ", fechaPeriodo: '" + $("#txtFechaPeriodo").val()
                        + "', Sueldos: '" + $("#txtSueldos").val()
                        + "', SeguridadEHigiene: '" + $("#txtSeguridadEHigiene").val()
                        + "', Municipales: '" + $("#txtMunicipales").val()
                        + "', Monotributos: '" + $("#txtMonotributos").val()
                        + "', AportesYContribuciones: '" + $("#txtAportesYContribuciones").val()
                        + "', Ganancias12: '" + $("#txtGanancias12").val()
                        + "', CreditoBancario: '" + $("#txtCreditoBancario").val()
                        + "', RetencionesDeIIBB: '" + $("#txtRetencionesDeIIBB").val()
                        + "', PlanesAFIP: '" + $("#txtPlanesAFIP").val()
                        + "', Gastos1: '" + $("#txtGastos1").val()
                        + "', Gastos2: '" + $("#txtGastos2").val()
                        + "', Gastos3: '" + $("#txtGastos3").val()
                        + "', F1: '" + $("#txtF1").val()
                        + "', F2: '" + $("#txtF2").val()
                        + "'}";

                    $.ajax({
                        type: "POST",
                        url: "gastosGeneralese.aspx/guardar",
                        data: info,
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        async: false,
                        success: function (data, text) {
                            $('#divOk').show();
                            $("#divError").hide();
                            $('html, body').animate({ scrollTop: 0 }, 'slow');

                            $("#hdnID").val(data.d);
                            window.setTimeout(function () {
                                window.location = "/gastosGenerales.aspx";
                            }, 2000);


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
            }
        }

        
    },    
    cancelar: function () {
        window.location.href = "/gastosGenerales.aspx";
    },    
    ocultarMensajes: function () {
        $("#divError, #divOk").hide();
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
    /*** SEARCH ***/
    configFilters: function () {

        $(".select2").select2({ width: '100%', allowClear: true });

        // Date Picker
        Common.configDatePicker();
        Common.configFechasDesdeHasta("txtFechaDesde", "txtFechaHasta");

        $("#txtFechaDesde, #txtFechaHasta").keypress(function (event) {
            var keycode = (event.keyCode ? event.keyCode : event.which);
            if (keycode == '13') {
                GastosGenerales.resetearPagina();
                GastosGenerales.filtrar();
                return false;
            }
        });
    },
    nuevo: function () {
        window.location.href = "/gastosGeneralese.aspx";
    },
    editar: function (id) {
        window.location.href = "/gastosGeneralese.aspx?ID=" + id;
    },
    eliminar: function (id, nombre) {
        bootbox.confirm("¿Está seguro que desea eliminar el gasto general del periodo " + nombre + "?", function (result) {
            if (result) {
                $.ajax({
                    type: "POST",
                    url: "gastosGenerales.aspx/delete",
                    data: "{ id: " + id + "}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data, text) {
                        GastosGenerales.filtrar();
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
        GastosGenerales.filtrar();
    },
    mostrarPagProxima: function (id, nombre) {
        var paginaActual = parseInt($("#hdnPage").val());
        paginaActual++;
        $("#hdnPage").val(paginaActual);
        GastosGenerales.filtrar();
    },
    filtrar: function () {
        $("#divError").hide();

        if ($('#frmSearch').valid()) {
            $("#resultsContainer").html("");

            var currentPage = parseInt($("#hdnPage").val());

            var info = "{ periodo: '" + $("#ddlPeriodo").val()
                       + "', fechaDesde: '" + $("#txtFechaDesde").val()
                       + "', fechaHasta: '" + $("#txtFechaHasta").val()
                       + "', page: " + currentPage + ", pageSize: " + PAGE_SIZE
                       + "}";


            $.ajax({
                type: "POST",
                url: "gastosGenerales.aspx/getResults",
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
                    $("#divError").html(r.Message);
                    $("#divError").show();
                    $('html, body').animate({ scrollTop: 0 }, 'slow');
                }
            });
        }
    },
    verTodos: function () {
        $("#txtFechaDesde, #txtFechaHasta").val("");//, #ddlTipo
        GastosGenerales.filtrar();
    },
    exportar: function () {
        GastosGenerales.resetearExportacion();
        $("#imgLoading").show();
        $("#divIconoDescargar").hide();

        var info = "{ periodo: '" + $("#ddlPeriodo").val()
                   + "', fechaDesde: '" + $("#txtFechaDesde").val()
                   + "', fechaHasta: '" + $("#txtFechaHasta").val()
                   + "'}";

        $.ajax({
            type: "POST",
            url: "gastosGenerales.aspx/export",
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

                GastosGenerales.resetearExportacion();
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
        GastosGenerales.filtrar();
    },
    mostrarPagProxima: function () {
        var paginaActual = parseInt($("#hdnPage").val());
        paginaActual++;
        $("#hdnPage").val(paginaActual);
        GastosGenerales.filtrar();
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
            GastosGenerales.filtrar();
        }
    },    
}
