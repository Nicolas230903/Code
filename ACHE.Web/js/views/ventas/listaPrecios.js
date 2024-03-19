var listaPrecios = {
    /*** FORM ***/
    grabar: function () {
        $("#divError").hide();
        $("#divOk").hide();


        var lista = new Array();
        $("[idproducto]").each(function () {
            var obj = new Object();
            obj.IDPrecioConcepto = $(this).attr("idproducto");
            obj.IDConceptos = $(this).attr("idConcepto");
            obj.Precio = $(this).attr("value");
            lista.push(obj);
        });

        var id = ($("#hdnID").val() == "" ? "0" : $("#hdnID").val());
        if ($('#frmEdicion').valid()) {
            Common.mostrarProcesando("btnActualizar");
            var info = "{ id: " + parseInt(id)
                    + ", nombre: '" + $("#txtNombre").val()
                    + "', Observaciones: '" + $("#txtObservaciones").val()
                    + "', activo: " + parseInt($("#ddlActivo").val())
                    + ",listaDePrecios: " + JSON.stringify(lista)
                    + "}";

            $.ajax({
                type: "POST",
                url: "/modulos/ventas/listaPreciose.aspx/guardar",
                data: info,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data, text) {
                    $('#divOk').show();
                    $("#divError").hide();
                    $('html, body').animate({ scrollTop: 0 }, 'slow');
                    window.location.href = "/modulos/ventas/listaPrecios.aspx";
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
        window.location.href = "/modulos/ventas/listaPrecios.aspx";
    },
    configForm: function () {
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

        //if (parseInt($("#hdnID").val()) > 0) {
        listaPrecios.obtenerListaPrecios();
        //}

       
    },
    obtenerListaPrecios: function () {

        var id = parseInt($("#hdnID").val());
        var info = "{ id: " + id + "}";

        $.ajax({
            type: "POST",
            url: "/modulos/ventas/listaPreciose.aspx/ObtenerListaPrecios",
            data: info,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data, text) {
                if (data.d.Conceptos.length > 0)
                    $("#resultTemplate").tmpl({ results: data.d.Conceptos }).appendTo("#resultsContainer");
                else
                    $("#noResultTemplate").tmpl({ results: data.d.Conceptos }).appendTo("#resultsContainer");

                $(".ListaDePrecio").maskMoney({ thousands: '', decimal: '.', allowZero: true });
               
            },
            error: function (response) {
                var r = jQuery.parseJSON(response.responseText);
                alert(r.Message);
            }
        });
    },

    /*** SEARCH ***/
    configFilters: function () {

        $("#txtNombre").keypress(function (event) {
            var keycode = (event.keyCode ? event.keyCode : event.which);
            if (keycode == '13') {
                listaPrecios.resetearPagina();
                listaPrecios.filtrar();
                return false;
            }
        });
    },
    nuevo: function () {
        window.location.href = "/modulos/ventas/listaPreciose.aspx";
    },
    editar: function (id) {
        window.location.href = "/modulos/ventas/listaPreciose.aspx?ID=" + id;
    },
    eliminar: function (id, nombre) {
        bootbox.confirm("¿Está seguro que desea eliminar a " + nombre + "?", function (result) {
            if (result) {
                $.ajax({
                    type: "POST",
                    url: "/modulos/ventas/listaPrecios.aspx/delete",
                    data: "{ id: " + id + "}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data, text) {
                        listaPrecios.filtrar();
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
        listaPrecios.filtrar();
    },
    mostrarPagProxima: function () {
        var paginaActual = parseInt($("#hdnPage").val());
        paginaActual++;
        $("#hdnPage").val(paginaActual);
        listaPrecios.filtrar();
    },
    resetearPagina: function () {
        $("#hdnPage").val("1");
    },
    filtrar: function () {

        $("#divError").hide();
        $("#resultsContainer").html("");
        var currentPage = parseInt($("#hdnPage").val());

        var info = "{ nombre: '" + $("#txtNombre").val()
                   + "', page: " + currentPage + ", pageSize: " + PAGE_SIZE
                   + "}";

        $.ajax({
            type: "POST",
            url: "/modulos/ventas/listaPrecios.aspx/getResults",
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
        listaPrecios.resetearExportacion();
    },
    verTodos: function () {
        $("#txtNombre").val("");
        listaPrecios.filtrar();
    },
    exportar: function () {
        listaPrecios.resetearExportacion();
        $("#imgLoading").show();
        $("#divIconoDescargar").hide();

        var info = "{ nombre: '" + $("#txtNombre").val() + "'}";

        $.ajax({
            type: "POST",
            url: "/modulos/ventas/listaPrecios.aspx/export",
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
                listaPrecios.resetearExportacion();
            }
        });
    },
    resetearExportacion: function () {
        $("#imgLoading, #lnkDownload").hide();
        $("#divIconoDescargar").show();
    }
}