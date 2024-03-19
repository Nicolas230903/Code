var activos = {
    /*** FORM ***/
    grabar: function () {
        $("#divError").hide();
        $("#divOk").hide();

        var id = ($("#hdnID").val() == "" ? "0" : $("#hdnID").val());
        var idcompra = ($("#ddlCompra").val() == "" ? "0" : $("#ddlCompra").val());

        if ($('#frmEdicion').valid()) {
            Common.mostrarProcesando("btnActualizar");
            var info = "{ id: " + parseInt(id)
                        + ", idPersona: " + $("#ddlPersona").val()
                        + ", idCompras: " + idcompra
                        + ", fechaInicio: '" + $("#txtFechaInicio").val()
                        + "', fechaCompra: '" + $("#txtFechaCompra").val()

                        + "', garantia: '" + $("#txtGarantia").val()
                        + "', vidaUtil: '" + $("#txtVidaUtil").val()
                        + "', marca: '" + $("#txtMarca").val()
                        + "', nroDeSerie: '" + $("#txtNroDeSerie").val()

                        + "', descripcion: '" + $("#txtDescripcion").val()
                        + "', responsable: '" + $("#txtResponsable").val()
                        + "', ubicacion: '" + $("#txtUbicacion").val()
                        + "', observaciones: '" + $("#txtObservaciones").val()
                        + "'}";

            $.ajax({
                type: "POST",
                url: "/modulos/tesoreria/activose.aspx/guardar",
                data: info,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data, text) {
                    $('#divOk').show();
                    $("#divError").hide();
                    $('html, body').animate({ scrollTop: 0 }, 'slow');
                    window.location.href = "/modulos/tesoreria/activos.aspx";
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
        window.location.href = "/modulos/tesoreria/activos.aspx";
    },

    configForm: function () {

      
        // Date Picker
        Common.configDatePicker();
        Common.configFechasDesdeHasta("txtFechaInicio,txtFechaCompra");

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

        $(".select2").select2({
            width: '100%', allowClear: true,
            formatNoMatches: function (term) {
                return "<a style='cursor:pointer' onclick=\"$('#modalNuevoCliente').modal('show');$('.select2').select2('close');\">+ Agregar</a>";
            }
        });
        Common.obtenerPersonas("ddlPersona", $("#hdnIDPersona").val(), true);
    },

    changePersona: function () {

        var idPersona = ($("#ddlPersona").val() == "" ? "0" : $("#ddlPersona").val());

        var info = "{ idPersona: " + parseInt(idPersona) + " }";

        $.ajax({
            type: "POST",
            url: "/modulos/tesoreria/activose.aspx/cargarCompras",
            data: info,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data, text) {
                $("#ddlCompra").html("");
                $("#divError").hide();

                if ($("#hdnCompra").val() == "") {
                    $("<option/>").attr("value", "").text("").appendTo($("#ddlCompra"));
                }

                for (var i = 0; i < data.d.length; i++) {
                    $("<option/>").attr("value", data.d[i].ID).text(data.d[i].Nombre).appendTo($("#ddlCompra"));
                }

                if ($("#hdnCompra").val() == "") {
                    activos.changeFechaCompra();
                }
            },
            error: function (response) {
                var r = jQuery.parseJSON(response.responseText);
                $("#msgError").html(r.Message);
                $("#divError").show();
                $('html, body').animate({ scrollTop: 0 }, 'slow');
            }
        });
    },
   
    changeFechaCompra: function () {
        var idComrpa = ($("#ddlCompra").val() == "" ? "0" : $("#ddlCompra").val());

        var info = "{ idCompra: " + parseInt(idComrpa) + " }";
      
        $.ajax({
            type: "POST",
            url: "/modulos/tesoreria/activose.aspx/cargarFechaCompra",
            data: info,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data, text) {
                $("#txtFechaCompra").html("");
                $("#txtFechaCompra").val(data.d);
            },
            error: function (response) {
                var r = jQuery.parseJSON(response.responseText);
                $("#msgError").html(r.Message);
                $("#divError").show();
                $('html, body').animate({ scrollTop: 0 }, 'slow');
            }
        });
    },
    /*** SEARCH ***/
    configFilters: function () {
        // Date Picker
        Common.configDatePicker();
        Common.configFechasDesdeHasta("txtFechaDesde", "txtFechaHasta");

        $(".select2").select2({ width: '100%', allowClear: true });
        Common.obtenerPersonas("ddlPersona", "", true);

        $("#idCompra,#txtFechaDesde,#txtFechaHasta").keypress(function (event) {
            var keycode = (event.keyCode ? event.keyCode : event.which);
            if (keycode == '13') {
                activos.resetearPagina();
                activos.filtrar();
                return false;
            }
        });
    },
    mostrarPagAnterior: function () {
        var paginaActual = parseInt($("#hdnPage").val());
        paginaActual--;
        $("#hdnPage").val(paginaActual);
        activos.filtrar();
    },

    mostrarPagProxima: function () {
        var paginaActual = parseInt($("#hdnPage").val());
        paginaActual++;
        $("#hdnPage").val(paginaActual);
        activos.filtrar();
    },

    resetearPagina: function () {
        $("#hdnPage").val("1");
    },

    filtrar: function () {

        $("#divError").hide();
        $("#resultsContainer").html("");
        var currentPage = parseInt($("#hdnPage").val());
        var idPersona = ($("#ddlPersona").val() == "" ? "0" : $("#ddlPersona").val());

        var info = "{ idPersona: " + parseInt(idPersona)
                   + ", fechaDesde: '" + $("#txtFechaDesde").val()
                   + "', fechaHasta: '" + $("#txtFechaHasta").val()
                   + "', page: " + currentPage + ", pageSize: " + PAGE_SIZE
                   + "}";

        $.ajax({
            type: "POST",
            url: "/modulos/tesoreria/activos.aspx/getResults",
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
        activos.resetearExportacion();
    },

    verTodos: function () {
        $("#ddlPersona").val("").trigger("change");
        $("#txtFechaDesde").val("");
        $("#txtFechaHasta").val("");
        activos.filtrar();
    },

    exportar: function () {
        activos.resetearExportacion();
        $("#imgLoading").show();
        $("#divIconoDescargar").hide();
        var idPersona = ($("#idPersona").val() == "" ? "0" : $("#idPersona").val());

        var info = "{ idPersona: " + parseInt(idPersona)
                                 + ", fechaDesde: '" + $("#txtFechaDesde").val()
                                 + "', fechaHasta: '" + $("#txtFechaHasta").val()
                                 + "'}";

        $.ajax({
            type: "POST",
            url: "/modulos/tesoreria/activos.aspx/export",
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
                activos.resetearExportacion();
            }
        });
    },

    resetearExportacion: function () {
        $("#imgLoading, #lnkDownload").hide();
        $("#divIconoDescargar").show();
    },
    /** ABM**/
    nuevo: function () {
        window.location.href = "/modulos/tesoreria/activose.aspx";
    },
    editar: function (id) {
        window.location.href = "/modulos/tesoreria/activose.aspx?ID=" + id;
    },
    eliminar: function (id, nombre) {
        bootbox.confirm("¿Está seguro que desea eliminar el activo  nro:" + nombre + "?", function (result) {
            if (result) {
                $.ajax({
                    type: "POST",
                    url: "/modulos/tesoreria/activos.aspx/delete",
                    data: "{ id: " + id + "}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data, text) {
                        activos.filtrar();
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
    /*****/
}