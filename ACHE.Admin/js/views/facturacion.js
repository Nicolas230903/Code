var facturacion = {
    /*** SEARCH ***/
    configFilters: function () {

        $(".select2").select2({ width: '100%', allowClear: true });

        $("#txtCondicion").keypress(function (event) {
            var keycode = (event.keyCode ? event.keyCode : event.which);
            if (keycode == '13') {
                facturacion.resetearPagina();
                facturacion.filtrar();
                return false;
            }
        });

        // Date Picker
        Common.configDatePicker();
        Common.configFechasDesdeHasta("txtFechaDesde", "txtFechaHasta");
       
    },
    mostrarPagAnterior: function () {
        var paginaActual = parseInt($("#hdnPage").val());
        paginaActual--;
        $("#hdnPage").val(paginaActual);
        facturacion.filtrar();
    },
    mostrarPagProxima: function () {
        var paginaActual = parseInt($("#hdnPage").val());
        paginaActual++;
        $("#hdnPage").val(paginaActual);
        facturacion.filtrar();
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
                   //+ "', fechaDesde: '" + $("#txtFechaDesde").val()
                   //+ "', fechaHasta: '" + $("#txtFechaHasta").val()
                   + "', page: " + currentPage + ", pageSize: " + PAGE_SIZE
                   + "}";

        $.ajax({
            type: "POST",
            url: "/facturacion/ObtenerUsuarios",
            data: info,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {

                if (data.TotalPage > 0) {
                    $("#divPagination").show();

                    $("#lnkNextPage, #lnkPrevPage").removeAttr('disabled')
                    if (data.TotalPage == 1)
                        $("#lnkNextPage, #lnkPrevPage").attr('disabled', "disabled")
                    else if (currentPage == data.TotalPage)
                        $("#lnkNextPage").attr("disabled", "disabled");
                    else if (currentPage == 1)
                        $("#lnkPrevPage").attr("disabled", "disabled");

                    var aux = (currentPage * PAGE_SIZE);
                    if (aux > data.TotalItems)
                        aux = data.TotalItems;
                    $("#msjResultados").html("Mostrando " + ((currentPage * PAGE_SIZE) - PAGE_SIZE + 1) + " - " + aux + " de " + data.TotalItems);
                }
                else {
                    $("#divPagination").hide();
                    $("#msjResultados").html("");
                }

                // Render using the template
                if (data.Items.length > 0)
                    $("#resultTemplate").tmpl({ results: data.Items }).appendTo("#resultsContainer");
                else
                    $("#noResultTemplate").tmpl({ results: data.Items }).appendTo("#resultsContainer");
            }
        });
        facturacion.resetearExportacion();
    },
    exportar: function () {
        facturacion.resetearExportacion();
        $("#imgLoading").show();
        $("#divIconoDescargar").hide();

        var info = "{ condicion: '" + $("#txtCondicion").val()
                + "', periodo: '" + $("#ddlPeriodo").val()
                //+ "', fechaDesde: '" + $("#txtFechaDesde").val()
                //+ "', fechaHasta: '" + $("#txtFechaHasta").val()
                + "'}";

        $.ajax({
            type: "POST",
            url: "/facturacion/export",
            data: info,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                if (data != "") {

                    $("#divError").hide();
                    $("#imgLoading").hide();
                    $("#lnkDownload").show();
                    $("#lnkDownload").attr("href", data);
                    $("#lnkDownload").attr("download", data);
                }
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
            facturacion.resetearPagina();
            facturacion.filtrar();
        }
    },
    importaciones: function (id) {
        window.location.href = "/Importaciones/Index/" + id;
    },
    editar: function (id) {
        window.open("/Usuario/Edicion/" + id);
    }
}