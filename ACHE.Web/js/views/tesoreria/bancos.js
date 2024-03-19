var Bancos = {
    /*** FORM ***/
    grabar: function () {
        $("#divError").hide();
        $("#divOk").hide();

        var id = ($("#hdnID").val() == "" ? "0" : $("#hdnID").val());
        if ($('#frmEdicion').valid()) {
            Common.mostrarProcesando("btnActualizar");
            var info = "{ id: " + parseInt(id)
                    + ", idBancoBase: '" + $("#ddlBanco").val()
                    + "', moneda: '" + $("#ddlMoneda").val()
                    + "', nroCuenta: '" + $("#txtNroCuenta").val()
                    + "', activo: " + parseInt($("#ddlActivo").val())
                    + " , saldoInicial: '" + $("#txtsaldoInicial").val()
                    + "', ejecutivo: '" + $("#txtEjecutivo").val()
                    + "', direccion: '" + $("#txtDireccion").val()
                    + "', telefono: '" + $("#txtTelefono").val()
                    + "', email: '" + $("#txtEmail").val()
                    + "', observacion: '" + $("#txtObservacion").val()
                    + "'}";
            $.ajax({
                type: "POST",
                url: "/modulos/tesoreria/bancose.aspx/guardar",
                data: info,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data, text) {
                    $('#divOk').show();
                    $("#divError").hide();
                    $('html, body').animate({ scrollTop: 0 }, 'slow');
                    window.location.href = "/modulos/Tesoreria/bancos.aspx";
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
        window.location.href = "/modulos/Tesoreria/bancos.aspx";
    },
    configForm: function () {
        $("#txtNroCuenta").numericInput();
        $("#txtsaldoInicial").maskMoney({ thousands: '', decimal: '.', allowZero: true });
        $(".select2").select2({ width: '100%', allowClear: true });
        Common.configTelefono("txtTelefono");
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
        $("#txtNroCuenta").numericInput();

        $("#txtCondicion").keypress(function (event) {
            var keycode = (event.keyCode ? event.keyCode : event.which);
            if (keycode == '13') {
                Bancos.resetearPagina();
                Bancos.filtrar();
                return false;
            }
        });        
    },
    nuevo: function () {
        window.location.href = "/modulos/tesoreria/bancose.aspx";
    },
    editar: function (id) {
        window.location.href = "/modulos/tesoreria/bancose.aspx?ID=" + id;
    },
    eliminar: function (id, nombre) {
        bootbox.confirm("¿Está seguro que desea eliminar a " + nombre + "?", function (result) {
            if (result) {
                $.ajax({
                    type: "POST",
                    url: "/modulos/Tesoreria/bancos.aspx/delete",
                    data: "{ id: " + id + "}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data, text) {
                        Bancos.filtrar();
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
        Bancos.filtrar();
    },
    mostrarPagProxima: function () {
        var paginaActual = parseInt($("#hdnPage").val());
        paginaActual++;
        $("#hdnPage").val(paginaActual);
        Bancos.filtrar();
    },
    resetearPagina: function () {
        $("#hdnPage").val("1");
    },
    filtrar: function () {

        $("#divError").hide();
        $("#resultsContainer").html("");
        var currentPage = 1;// parseInt($("#hdnPage").val());

        var info = "{ condicion: '" + ''//$("#txtCondicion").val()
                   + "', page: " + currentPage + ", pageSize: " + PAGE_SIZE
                   + "}";

        $.ajax({
            type: "POST",
            url: "/modulos/Tesoreria/bancos.aspx/getResults",
            data: info,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data, text) {
                //if (data.d.TotalPage > 0) {
                //    $("#divPagination").show();

                //    $("#lnkNextPage, #lnkPrevPage").removeAttr('disabled')
                //    if (data.d.TotalPage == 1)
                //        $("#lnkNextPage, #lnkPrevPage").attr('disabled', "disabled")
                //    else if (currentPage == data.d.TotalPage)
                //        $("#lnkNextPage").attr("disabled", "disabled");
                //    else if (currentPage == 1)
                //        $("#lnkPrevPage").attr("disabled", "disabled");

                //    var aux = (currentPage * PAGE_SIZE);
                //    if (aux > data.d.TotalItems)
                //        aux = data.d.TotalItems;
                //    $("#msjResultados").html("Mostrando " + ((currentPage * PAGE_SIZE) - PAGE_SIZE + 1) + " - " + aux + " de " + data.d.TotalItems);
                //}
                //else {
                //    $("#divPagination").hide();
                //    $("#msjResultados").html("");
                //}

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
        Bancos.resetearExportacion();
    },
    verTodos: function () {
        $("#txtNombreBanco, #txtNroCuenta").val("");
        Bancos.filtrar();
    },
    exportar: function () {
        Bancos.resetearExportacion();
        $("#imgLoading").show();
        $("#divIconoDescargar").hide();

        var info = "{ condicion: '" + $("#txtCondicion").val()
                   + "'}";

        $.ajax({
            type: "POST",
            url: "/modulos/Tesoreria/bancos.aspx/export",
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
                Bancos.resetearExportacion();
            }
        });
    },
    resetearExportacion: function () {
        $("#imgLoading, #lnkDownload").hide();
        $("#divIconoDescargar").show();
    }
}