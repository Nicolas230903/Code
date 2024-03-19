var Auditoria = {
    /*** SEARCH ***/
    configFilters: function () {
        $(".select2").select2({ width: '100%', allowClear: true });

        //Common.obtenerPersonas("ddlPersona", "", true);

        // Date Picker
        Common.configDatePicker();
        Common.configFechasDesdeHasta("txtFechaDesde", "txtFechaHasta");
        Common.soloNumerosConGuiones("txtNumero");
        $("#txtNroDocumento").numericInput();

        $("#txtCondicion, #txtFechaDesde, #txtFechaHasta").keypress(function (event) {
            var keycode = (event.keyCode ? event.keyCode : event.which);
            if (keycode == '13') {
                Auditoria.resetearPagina();
                Auditoria.filtrar();
                return false;
            }
        });

        
    },
    mostrarPagAnterior: function () {
        var paginaActual = parseInt($("#hdnPage").val());
        paginaActual--;
        $("#hdnPage").val(paginaActual);
        Auditoria.filtrar();
    },
    mostrarPagProxima: function (id, nombre) {
        var paginaActual = parseInt($("#hdnPage").val());
        paginaActual++;
        $("#hdnPage").val(paginaActual);
        Auditoria.filtrar();
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
                url: "auditoria.aspx/getResults",
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
        }
    },
    verTodos: function () {
        $("#txtNumero, #txtFechaDesde, #txtFechaHasta").val("");//, #ddlTipo
        Auditoria.filtrar();
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
            Auditoria.filtrar();
        }
    }    
}
