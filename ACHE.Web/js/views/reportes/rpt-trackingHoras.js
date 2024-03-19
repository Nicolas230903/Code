﻿var rptTrackingHoras = {
    /*** SEARCH ***/
    configFilters: function () {

        Common.obtenerPersonas("ddlPersona", "", true);
        $(".select2").select2({ width: '100%', allowClear: true });

        // Date Picker
        Common.configDatePicker();
        Common.configFechasDesdeHasta("txtFechaDesde", "txtFechaHasta");
 
    },

    mostrarPagAnterior: function () {
        var paginaActual = parseInt($("#hdnPage").val());
        paginaActual--;
        $("#hdnPage").val(paginaActual);
        rptTrackingHoras.filtrar();
    },

    mostrarPagProxima: function () {
        var paginaActual = parseInt($("#hdnPage").val());
        paginaActual++;
        $("#hdnPage").val(paginaActual);
        rptTrackingHoras.filtrar();
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
            if ($("#ddlPersona").val() != "")
                idPersona = parseInt($("#ddlPersona").val());

            var info = "{ idPersona: '" + idPersona
                       + "', fechaDesde: '" + $("#txtFechaDesde").val()
                       + "', fechaHasta: '" + $("#txtFechaHasta").val()
                       + "', page: " + currentPage + ", pageSize: " + PAGE_SIZE
                       + "}";

            $.ajax({
                type: "POST",
                url: "/modulos/reportes/trackingHora.aspx/getResults",
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
                    if (data.d.Items.length > 0) {
                        $("#resultTemplate").tmpl({ results: data.d.Items }).appendTo("#resultsContainer");
                        $("#resultTemplateTotales").tmpl({ results: data }).appendTo("#resultsContainer");
                    } else
                        $("#noResultTemplate").tmpl({ results: data.d.Items }).appendTo("#resultsContainer");

                },
                error: function (response) {
                    var r = jQuery.parseJSON(response.responseText);
                    alert(r.Message);
                }
            });
            rptTrackingHoras.resetearExportacion();
        }
    },

    verTodos: function () {
        $("#txtFechaDesde, #txtFechaHasta").val("");
        $("#ddlPersona").val("").trigger("change");
        rptTrackingHoras.filtrar();
    },

    exportar: function () {
        if ($('#frmSearch').valid()) {

            var idPersona = 0;
            if ($("#ddlPersona").val() != "" && $("#ddlPersona").val() != null)
                idPersona = parseInt($("#ddlPersona").val());

            rptTrackingHoras.resetearExportacion();
            $("#imgLoading").show();
            $("#divIconoDescargar").hide();

            var info = "{ idPersona: '" + idPersona
                        + "', fechaDesde: '" + $("#txtFechaDesde").val()
                        + "', fechaHasta: '" + $("#txtFechaHasta").val()
                       + "'}";

            $.ajax({
                type: "POST",
                url: "/modulos/reportes/trackingHora.aspx/export",
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
    }
}