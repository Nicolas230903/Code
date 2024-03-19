var ricv = {
    configFilters: function () {

        $("#txtFechaDesde").keypress(function (event) {
            var keycode = (event.keyCode ? event.keyCode : event.which);
            if (keycode == '13') {
                ricv.filtrar();
                return false;
            }
        });

        $('#txtFechaDesde').datepicker({
            changeMonth: true,
            changeYear: true,
            showButtonPanel: true,
            dateFormat: 'MM yy',
            onClose: function (dateText, inst) {
                var month = $("#ui-datepicker-div .ui-datepicker-month :selected").val();
                var year = $("#ui-datepicker-div .ui-datepicker-year :selected").val();
                $(this).datepicker('setDate', new Date(year, month, 1));
            }
        });
        configDatePicker();


    },
    filtrar: function () {

        Common.mostrarProcesando("btnGenerar");

        $("#divError").hide();
        $("#resultsContainer").html("");
        var idPersona = ($("#ddlPersona").val() == "" ? "0" : $("#ddlPersona").val());

        var info = "{ periodo: '" + $("#txtFechaDesde").val()
                + "'}";

        $.ajax({
            type: "POST",
            url: "/modulos/reportes/citiVentas.aspx/generarExportaciones",
            data: info,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data, text) {
                Common.ocultarProcesando("btnGenerar", "Generar Exportación");
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
                    jQuery('.tooltips').tooltip({ container: 'body' });
                }
                else
                    $("#noResultTemplate").tmpl({ results: data.d.Items }).appendTo("#resultsContainer");
            },
            error: function (response) {
                var r = jQuery.parseJSON(response.responseText);
                $("#msgError").html(r.Message);
                $("#divError").show();
                $("#divOk").hide();
                $('html, body').animate({ scrollTop: 0 }, 'slow');
                Common.ocultarProcesando("btnGenerar", "Generar Exportación");
            }
        });
    },
    verTodos: function () {
        $("#ddlPersona").val("").trigger("change");
        $("#txtFechaDesde").val("");
        $("#txtFechaHasta").val("");
        ricv.filtrar();
    },

    descargar: function (obj, url) {

        var nn = window.navigator.userAgent.toLowerCase();
        if (nn.indexOf('chrome') != -1 || (nn.indexOf('firefox') != -1) || (nn.indexOf('iceweasel') != -1)) {
            $("#" + obj).attr("href", url);
        } else {

            window.open(url);
        }
    },
}