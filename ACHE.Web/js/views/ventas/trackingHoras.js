var trackingHoras = {
    /*** FORM ***/
    grabar: function () {
        $("#divError").hide();
        $("#divOk").hide();

        var id = ($("#hdnID").val() == "" ? "0" : $("#hdnID").val());
        if ($('#frmEdicion').valid()) {
            Common.mostrarProcesando("btnActualizar");

            var info = "{ id: " + parseInt(id)
                    + ", IDPersona: '" + $("#ddlPersona").val()
                    + "', Fecha: '" + $("#txtFecha").val()
                    + "', Horas: '" + $("#txtCantHoras").val()
                    + "', Tarea: '" + $("#ddlTarea").val()
                    + "', estado: '" + $("#ddlEstado").val()
                    + "', Observaciones: '" + $("#txtObservaciones").val()
                    + "', idUsuarioAdicional: '" + $("#ddlUuarios").val()
                    + "'}";

            $.ajax({
                type: "POST",
                url: "/modulos/ventas/trackingHorase.aspx/guardar",
                data: info,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data, text) {
                    $('#divOk').show();
                    $("#divError").hide();
                    $('html, body').animate({ scrollTop: 0 }, 'slow');
                    window.location.href = "/modulos/ventas/trackingHoras.aspx";
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
        window.location.href = "/modulos/ventas/trackingHoras.aspx";
    },

    configForm: function () {
       
        $("#txtCantHoras").numericInput();
        Common.obtenerPersonas("ddlPersona", $("#hdnIDPersona").val(), true);
        $(".select2").select2({ width: '100%', allowClear: true });

        $('.txtFecha').datepicker();

        // Date Picker
        Common.configDatePicker();
        Common.configFechasDesdeHasta("txtFecha");

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

    /*** SEARCH ***/
    configFilters: function () {
        $(".select2").select2({ width: '100%', allowClear: true });

        $("#txtCondicion, #txtFechaHasta, #txtNombre").keypress(function (event) {
            var keycode = (event.keyCode ? event.keyCode : event.which);
            if (keycode == '13') {
                trackingHoras.resetearPagina();
                trackingHoras.filtrar();
                return false;
            }
        });
        // Date Picker
        Common.configDatePicker();
        Common.configFechasDesdeHasta("txtFechaDesde", "txtFechaHasta");

    },

    nuevo: function () {
        window.location.href = "/modulos/ventas/trackingHorase.aspx";
    },

    editar: function (id) {
        window.location.href = "/modulos/ventas/trackingHorase.aspx?ID=" + id;
    },

    eliminar: function (id, nombre) {
        bootbox.confirm("¿Está seguro que desea eliminar la tarea " + nombre + "?", function (result) {
            if (result) {
                $.ajax({
                    type: "POST",
                    url: "/modulos/ventas/trackingHoras.aspx/delete",
                    data: "{ id: " + id + "}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data, text) {
                        trackingHoras.filtrar();
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
        trackingHoras.filtrar();
    },

    mostrarPagProxima: function () {
        var paginaActual = parseInt($("#hdnPage").val());
        paginaActual++;
        $("#hdnPage").val(paginaActual);
        trackingHoras.filtrar();
    },

    resetearPagina: function () {
        $("#hdnPage").val("1");
    },

    filtrar: function () {
        $("#divError").hide();
        if ($('#frmSearch').valid()) {
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
                url: "/modulos/ventas/trackingHoras.aspx/getResults",
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
            trackingHoras.resetearExportacion();
        }
    },

    verTodos: function () {
        $("#txtFechaDesde, #txtFechaHasta").val("");
        $("#ddlPersona").val("").trigger("change");
        trackingHoras.filtrar();
    },

    exportar: function () {
        if ($('#frmSearch').valid()) {

            trackingHoras.resetearExportacion();
            $("#imgLoading").show();
            $("#divIconoDescargar").hide();

            var info = "{ condicion: '" + $("#txtCondicion").val()
                        + "', periodo: '" + $("#ddlPeriodo").val()
                        + "', fechaDesde: '" + $("#txtFechaDesde").val()
                        + "', fechaHasta: '" + $("#txtFechaHasta").val()
                       + "'}";

            $.ajax({
                type: "POST",
                url: "/modulos/ventas/trackingHoras.aspx/export",
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
                    trackingHoras.resetearExportacion();
                }
            });
        }
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
            trackingHoras.filtrar();
        }
    },
}