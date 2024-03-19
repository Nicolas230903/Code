﻿/*** SEARCH ***/
var LibroMayor = {
    configFilters: function () {
        $(".select2").select2({ width: '100%', allowClear: true });

        Common.obtenerPersonas("ddlPersona", "", true);

        // Date Picker
        //configDatePicker();

        $("#txtFechaDesde, #txtFechaHasta").keypress(function (event) {
            var keycode = (event.keyCode ? event.keyCode : event.which);
            if (keycode == '13') {
                LibroMayor.resetearPagina();
                LibroMayor.filtrar();
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
                alert(element.parent('.input-group').length);
                if (element.parent('.input-group').length) {
                    error.insertAfter(element.parent());
                } else {
                    error.insertAfter(element);
                }
            }
        });
    },

    mostrarPagAnterior: function () {
        var paginaActual = parseInt($("#hdnPage").val());
        paginaActual--;
        $("#hdnPage").val(paginaActual);
        LibroMayor.filtrar();
    },

    mostrarPagProxima: function () {
        var paginaActual = parseInt($("#hdnPage").val());
        paginaActual++;
        $("#hdnPage").val(paginaActual);
        LibroMayor.filtrar();
    },

    resetearPagina: function () {
        $("#hdnPage").val("1");
    },

    filtrar: function () {

        $("#divError").hide();

        if ($('#frmSearch').valid()) {
            $("#resultsContainer").html("");
            var currentPage = parseInt($("#hdnPage").val());

            var idPersona = 0;
            if ($("#ddlPersona").val() != "" && $("#ddlPersona").val() != null)
                idPersona = parseInt($("#ddlPersona").val());

            var info = "{ idPersona: " + idPersona
                       + ", fechaDesde: '" + $("#txtFechaDesde").val()
                       + "', fechaHasta: '" + $("#txtFechaHasta").val()
                       + "', page: " + currentPage + ", pageSize: " + PAGE_SIZE
                       + "}";


            $.ajax({
                type: "POST",
                url: "/modulos/reportes/LibroMayor.aspx/getResults",
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
                    if (data.d.Asientos.length > 0) {
                        $("#resultTemplate").tmpl({ results: data.d.Asientos }).appendTo("#resultsContainer");

                        $("#totalDebe").html("$ " + data.d.TotalDebe);
                        $("#totalHaber").html("$ " + data.d.TotalHaber);
                        $("#totalSaldo").html("$ " + data.d.TotalSaldo);

                    }
                    else
                        $("#noResultTemplate").tmpl({ results: data.d.Asientos }).appendTo("#resultsContainer");
                },
                error: function (response) {
                    var r = jQuery.parseJSON(response.responseText);
                    alert(r.Message);
                }
            });
            LibroMayor.resetearExportacion();
        }
    },

    verTodos: function () {
        $("#txtFechaDesde, #txtFechaHasta").val("");
        $("#ddlPersona").val("").trigger("change");
        LibroMayor.filtrar();
    },

    exportar: function () {
        LibroMayor.resetearExportacion();
        $("#imgLoading").show();
        $("#divIconoDescargar").hide();

        var idPersona = 0;
        if ($("#ddlPersona").val() != "")
            idPersona = parseInt($("#ddlPersona").val());

        var info = "{ idPersona: " + idPersona
                   + ", fechaDesde: '" + $("#txtFechaDesde").val()
                   + "', fechaHasta: '" + $("#txtFechaHasta").val()
                   + "'}";

        $.ajax({
            type: "POST",
            url: "/modulos/reportes/LibroMayor.aspx/export",
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
                LibroMayor.resetearExportacion();
            }
        });
    },

    resetearExportacion: function () {
        $("#imgLoading, #lnkDownload").hide();
        $("#divIconoDescargar").show();
    },
}