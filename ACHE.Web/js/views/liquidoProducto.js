var LiquidoProducto = {
    /*** SEARCH ***/
    configFilters: function () {
        $(".select2").select2({ width: '100%', allowClear: true });

        //Common.obtenerPersonas("ddlPersona", "", true);

        // Date Picker
        Common.configDatePicker();
        Common.configFechasDesdeHasta("txtFechaDesde", "txtFechaHasta");
        Common.soloNumerosConGuiones("txtNumero");
        LiquidoProducto.obtenerFechaUltimoLiquidoProducto();
        $("#txtNroDocumento").numericInput();
        LiquidoProducto.obtenerMargenes();

        $("#txtCondicion, #txtFechaDesde, #txtFechaHasta, #txtFechaUltimoLiquidoProducto").keypress(function (event) {
            var keycode = (event.keyCode ? event.keyCode : event.which);
            if (keycode == '13') {
                LiquidoProducto.resetearPagina();
                LiquidoProducto.filtrar();
                return false;
            }
        });

        
    },
    obtenerFechaUltimoLiquidoProducto: function() {
        $.ajax({
            type: "POST",
            url: "liquidoProducto.aspx/obtenerFechaUltimoLiquidoProducto",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                if (data != null) {
                    $("#txtFechaUltimoLiquidoProducto").val(data.d);
                }
            }
        });
    },
    mostrarPagAnterior: function () {
        var paginaActual = parseInt($("#hdnPage").val());
        paginaActual--;
        $("#hdnPage").val(paginaActual);
        LiquidoProducto.filtrar();
    },
    mostrarPagProxima: function (id, nombre) {
        var paginaActual = parseInt($("#hdnPage").val());
        paginaActual++;
        $("#hdnPage").val(paginaActual);
        LiquidoProducto.filtrar();
    },
    filtrar: function () {
        $("#divError").hide();

        if ($('#frmSearch').valid()) {
            $("#resultsContainer").html("");

            var currentPage = parseInt($("#hdnPage").val());

            var idPersona = 0;
            if ($("#ddlPersona").val() != null && $("#ddlPersona").val() != "")
                idPersona = parseInt($("#ddlPersona").val());

            var info = "{ condicion: '" + $("#txtCondicion").val()
                + "', periodo: '" + $("#ddlPeriodo").val()
                + "', fechaDesde: '" + $("#txtFechaDesde").val()
                + "', fechaHasta: '" + $("#txtFechaHasta").val()
                + "', fechaUltimoLiquidoProducto: '" + $("#txtFechaUltimoLiquidoProducto").val()
                + "', page: " + currentPage + ", pageSize: " + PAGE_SIZE
                + ", tipo: 'PDC'}";


            $.ajax({
                type: "POST",
                url: "liquidoProducto.aspx/getResults",
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
                    $("#msgError").html(r.Message);
                    $("#divError").show();
                    $("#divOk").hide();
                    $('html, body').animate({ scrollTop: 0 }, 'slow');
                }
            });
        }
    },
    verTodos: function () {
        $("#txtNumero, #txtFechaDesde, #txtFechaHasta").val("");//, #ddlTipo
        $("#ddlPersona").val("").trigger("change");
        LiquidoProducto.filtrar();
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
            LiquidoProducto.filtrar();
        }
    },   
    imprimir: function (id, tipoImpresion) {

        $("#divError").hide();

        var datos = "{ vertical: '" + $("#txtMargenVertical").val()
            + "', horizontal: '" + $("#txtMargenHorizontal").val()
            + "'}";

        $.ajax({
            type: "POST",
            url: "liquidoProducto.aspx/updateMargenes",
            data: datos,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data, text) {
                const dateObj = new Date();
                var mes = dateObj.getMonth() + 1;
                const month = String(mes).padStart(2, '0');
                const day = String(dateObj.getDate()).padStart(2, '0');
                const year = dateObj.getFullYear();
                const output = year + '-' + month + '-' + day;


                bootbox.prompt({
                    title: "Ingrese una fecha de entrega",
                    inputType: 'date',
                    value: output,
                    buttons: {
                        confirm: {
                            label: 'Aceptar'
                        },
                        cancel: {
                            label: 'Omitir'
                        }
                    },
                    callback: function (result) {

                        var info = "{ id: " + id + ", tipoImpresion: '" + tipoImpresion + "', fechaEntrega: '" + result + "'}";

                        $.ajax({
                            type: "POST",
                            url: "liquidoProducto.aspx/imprimir",
                            data: info,
                            contentType: "application/json; charset=utf-8",
                            dataType: "json",
                            success: function (data, text) {
                                //window.location.href = "/pdfGenerator.ashx?file=" + data.d + "&tipoDeArchivo=liquidoProducto";
                                //window.open(
                                //    "/pdfGenerator.ashx?file=" + data.d + "&tipoDeArchivo=liquidoProducto",
                                //    '_blank' // <- This is what makes it open in a new window.
                                //);
                                $("#divError").hide();
                                //$("#lnkDownloadPdfTicket").attr("href", "/pdfGenerator.ashx?file=" + data.d + "&tipoDeArchivo=liquidoProducto");
                                var pathFile = "/files/liquidoProducto/" + $("#hdnIDUsuario").val() + "/" + data.d + "#view=FitH,top";
                                $("#ifrPdf").attr("src", pathFile);
                                $("#modalPdf").modal("show");
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
                });
            },
            error: function (response) {
                var r = jQuery.parseJSON(response.responseText);
                $("#msgError").html(r.Message);
                $("#divError").show();
                $("#divOk").hide();
                $('html, body').animate({ scrollTop: 0 }, 'slow');
            }
        });
    },
    imprimirFiltrados: function (tipoImpresion) {
        $("#divError").hide();

        var datos = "{ vertical: '" + $("#txtMargenVertical").val()
            + "', horizontal: '" + $("#txtMargenHorizontal").val()
            + "'}";

        $.ajax({
            type: "POST",
            url: "liquidoProducto.aspx/updateMargenes",
            data: datos,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data, text) {
                if ($('#frmSearch').valid()) {

                    var currentPage = parseInt($("#hdnPage").val());

                    if ($("#ddlPersona").val() != null && $("#ddlPersona").val() != "")
                        idPersona = parseInt($("#ddlPersona").val());

                    const dateObj = new Date();
                    var mes = dateObj.getMonth() + 1;
                    const month = String(mes).padStart(2, '0');
                    const day = String(dateObj.getDate()).padStart(2, '0');
                    const year = dateObj.getFullYear();
                    const output = year + '-' + month + '-' + day;

                    bootbox.prompt({
                        title: "Ingrese una fecha de entrega",
                        inputType: 'date',
                        value: output,
                        buttons: {
                            confirm: {
                                label: 'Aceptar'
                            },
                            cancel: {
                                label: 'Omitir'
                            }
                        },
                        callback: function (result) {
                            var info = "{ condicion: '" + $("#txtCondicion").val()
                                + "', periodo: '" + $("#ddlPeriodo").val()
                                + "', fechaDesde: '" + $("#txtFechaDesde").val()
                                + "', fechaHasta: '" + $("#txtFechaHasta").val()
                                + "', fechaUltimoLiquidoProducto: '" + $("#txtFechaUltimoLiquidoProducto").val()
                                + "', page: " + currentPage + ", pageSize: " + PAGE_SIZE
                                + ", tipo: 'PDC', tipoImpresion: '" + tipoImpresion + "', fechaEntrega: '" + result + "'} ";

                            $.ajax({
                                type: "POST",
                                url: "liquidoProducto.aspx/imprimirFiltrados",
                                data: info,
                                contentType: "application/json; charset=utf-8",
                                dataType: "json",
                                success: function (data, text) {
                                    //window.location.href = "/pdfGenerator.ashx?file=" + data.d + "&tipoDeArchivo=liquidoProducto";
                                    //window.open(
                                    //    "/pdfGenerator.ashx?file=" + data.d + "&tipoDeArchivo=liquidoProducto",
                                    //    '_blank' // <- This is what makes it open in a new window.
                                    //);        
                                    $("#divError").hide();
                                    //$("#lnkDownloadPdfTicket").attr("href", "/pdfGenerator.ashx?file=" + data.d + "&tipoDeArchivo=liquidoProducto");
                                    var pathFile = "/files/liquidoProducto/" + $("#hdnIDUsuario").val() + "/" + data.d + "#view=FitH,top";
                                    $("#ifrPdf").attr("src", pathFile);
                                    $("#modalPdf").modal("show");
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
                    });
                }
            },
            error: function (response) {
                var r = jQuery.parseJSON(response.responseText);
                $("#msgError").html(r.Message);
                $("#divError").show();
                $("#divOk").hide();
                $('html, body').animate({ scrollTop: 0 }, 'slow');
            }
        });              
    },
    obtenerMargenes: function () {
        $("#divError").hide();
        $.ajax({
            type: "POST",
            url: "liquidoProducto.aspx/getMargenes",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                if (data != null) {
                    $("#txtMargenHorizontal").val(data.d.Horizontal);
                    $("#txtMargenVertical").val(data.d.Vertical);
                }
            },
            error: function (response) {
                var r = jQuery.parseJSON(response.responseText);
                $("#msgError").html(r.Message);
                $("#divError").show();
                $("#divOk").hide();
                $('html, body').animate({ scrollTop: 0 }, 'slow');
            }
        });
    },
}
