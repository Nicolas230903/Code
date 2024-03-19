var empleados = {
    //*** FORM ***/
    grabar: function () {
        $("#divError").hide();
        $("#divOk").hide();

        var id = ($("#hdnID").val() == "" ? "0" : $("#hdnID").val());
        if ($("#txtNombre,#txtApellido,#txtCUIT,#txtFechaAlta,#txtDomicilio,#ddlCiudad,#ddlProvincia").valid()) {
            Common.mostrarProcesando("btnActualizar");
            if (0 == parseInt($("#txtImporte").val())) {
                $("#msgError").html("El importe debve ser mayor a 0");
                $("#divError").show();
                $("#divOk").hide();
                $('html, body').animate({ scrollTop: 0 }, 'slow');
                return false;
            }

            var info = "{ id: " + parseInt(id)
                    + ", Nombre: '" + $("#txtNombre").val()
                    + "', Apellido: '" + $("#txtApellido").val()
                    + "', CUIT: '" + $("#txtCUIT").val()
                    + "', Telefono: '" + $("#txtTelefono").val()
                    + "', Celular: '" + $("#txtCelular").val()
                    + "', NroLegajo: '" + $("#txtNroLegajo").val()
                    + "', ContactoEmergencia: '" + $("#txtContactoEmergencia").val()
                    + "', idProvincia: " + $("#ddlProvincia").val()
                    + ", idCiudad: " + $("#ddlCiudad").val()
                    + ", Domicilio: '" + $("#txtDomicilio").val()
                    + "', PisoDepto: '" + $("#txtPisoDepto").val()
                    + "', ObraSocial: '" + $("#txtObraSocial").val()
                    + "', Sueldo: '" + $("#txtSueldo").val()
                    + "', Email: '" + $("#txtEmail").val()
                    + "', fechaAlta: '" + $("#txtFechaAlta").val()
                    + "', fechaBaja: '" + $("#txtFechaBaja").val()
                    + "'}";

            $.ajax({
                type: "POST",
                url: "/modulos/rrhh/empleadose.aspx/guardar",
                data: info,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: false,
                success: function (data, text) {
                    $("#hdnID").val(data.d);

                    if ($("#hdnSinCombioDeFoto").val() == "0") {
                        $('#divOk').show();
                        $("#divError").hide();
                        $('html, body').animate({ scrollTop: 0 }, 'slow');

                        window.location.href = "/modulos/rrhh/empleados.aspx";
                    }
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
            $("#msgError").html("Complete todos los datos obligatorios");
            $("#divError").show();
            return false;
        }
    },

    cancelar: function () {
        window.location.href = "/modulos/rrhh/empleados.aspx";
    },

    configForm: function () {

        $(".select2").select2({ width: '100%', allowClear: true });
        Common.obtenerProvincias("ddlProvincia", $("#hdnProvincia").val(), true);
        Common.obtenerCiudades("ddlCiudad", $("#hdnCiudad").val(), $("#hdnProvincia").val(), true);


        $("#txtNroLegajo,#txtCUIT").numericInput();
        $("#txtSueldo").maskMoney({ thousands: '', decimal: '.', allowZero: true });

        // Date Picker
        $('#txtFechaAlta,#txtFechaBaja').datepicker();
        Common.configDatePicker();
        Common.configFechasDesdeHasta("txtFechaAlta,#txtFechaBaja");

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
            },
            ignore: ".ignore",
            invalidHandler: function (e, validator) {
                if (validator.errorList.length)
                    $('#tabs a[href="#' + $(validator.errorList[0].element).closest(".tab-pane").attr('id') + '"]').tab('show')
            }
        });

        $.validator.addMethod("validCuit", function (value, element) {
            return CuitEsValido($("#txtCUIT").val());
        }, "CUIT Inválido");


        empleados.adjuntarFoto();
        Common.configTelefono("txtTelefono");
        Common.configTelefono("txtCelular");

    },
    changeProvincia:function() {
    Common.obtenerCiudades("ddlCiudad", "", $("#ddlProvincia").val(), true);
    },

    /*** Adjuntar Foto ***/
    adjuntarFoto: function () {

        $('#flpArchivo').fileupload({
            //url: '/subirImagenes.ashx?idEmpleado=' + $("#hdnID").val() + "&opcionUpload=empleados",
            success: function (response, status) {
                if (response == "OK") {
                    $("#divError").hide();
                    $("#divOk").show();
                    window.location.href = "/modulos/rrhh/empleados.aspx";
                }
                else {
                    $("#hdnFileName").val("");
                    $("#msgError").html(response);
                    $("#divError").show();
                    $("#divOk").hide();
                    $("#btnActualizar").attr("disabled", false);
                }
            },
            error: function (error) {
                $("#hdnFileName").val("");
                $("#msgError").html(error.responseText);
                $("#imgLoading").hide();
                $("#divError").show();
                $("#divOk").hide();
                $('html, body').animate({ scrollTop: 0 }, 'slow');
                $("#btnActualizar").attr("disabled", false);
            },
            autoUpload: false,
            add: function (e, data) {
                Common.ocultarProcesando("btnActualizar", "Aceptar");
                $("#hdnSinCombioDeFoto").val("1");
                $("#btnActualizar").on("click", function () {
                    $("#imgLoading").show();
                    empleados.grabar();
                    if ($("#hdnID").val() != "0") {
                        data.url = '/subirImagenes.ashx?idEmpleado=' + $("#hdnID").val() + "&opcionUpload=empleados";
                        data.submit();
                    }
                })
            }
        });
        empleados.showBtnEliminar();
    },
    showInputFoto: function () {
        $("#divLogo").slideToggle();
    },
    grabarsinImagen: function () {
        if ($("#hdnSinCombioDeFoto").val() == "0") {
            empleados.grabar();
        }
    },
    eliminarFoto: function () {
        var id = ($("#hdnID").val() == "" ? "0" : $("#hdnID").val());
        if (id != "") {
            var info = "{ idEmpleado: " + parseInt(id) + "}";

            $.ajax({
                type: "POST",
                url: "empleadose.aspx/eliminarFoto",
                data: info,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                async: false,
                success: function (data, text) {
                    $('#divOk').show();
                    $("#divError").hide();
                    $('html, body').animate({ scrollTop: 0 }, 'slow');
                    $("#imgFoto").attr("src", "/files/usuarios/no-cheque.png");
                    $("#hdnTieneFoto").val("0");
                    empleados.showBtnEliminar();
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
        else {
            $("#msgError").html("El producto no tiene una imagen guardada");
            $("#divError").show();
            return false;
        }
    },
    showBtnEliminar: function () {
        if ($("#hdnTieneFoto").val() == "1") {
            $("#divEliminarFoto").show();
            $("#divAdjuntarFoto").removeClass("col-sm-12").addClass("col-sm-6");
        }
        else {
            $("#divEliminarFoto").hide();
            $("#divAdjuntarFoto").removeClass("col-sm-6").addClass("col-sm-12");
        }
    },
    //*** SEARCH***/

    configFilters: function () {
        $("#txtCUIT").numericInput();

        $("#txtCUIT, #txtApellido,#txtNombre").keypress(function (event) {
            var keycode = (event.keyCode ? event.keyCode : event.which);
            if (keycode == '13') {
                empleados.resetearPagina();
                empleados.filtrar();
                return false;
            }
        });
    },

    nuevo: function () {
        window.location.href = "/modulos/rrhh/empleadose.aspx";
    },

    editar: function (id) {
        window.location.href = "/modulos/rrhh/empleadose.aspx?ID=" + id;
    },

    eliminar: function (id, nombre) {
        bootbox.confirm("¿Está seguro que desea dar de baja al empleado: " + nombre + "?", function (result) {
            if (result) {
                $.ajax({
                    type: "POST",
                    url: "/modulos/rrhh/empleados.aspx/delete",
                    data: "{ id: " + id + "}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data, text) {
                        empleados.filtrar();
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
        empleados.filtrar();
    },

    mostrarPagProxima: function () {
        var paginaActual = parseInt($("#hdnPage").val());
        paginaActual++;
        $("#hdnPage").val(paginaActual);
        empleados.filtrar();
    },

    resetearPagina: function () {
        $("#hdnPage").val("1");
    },

    filtrar: function () {

        $("#divError").hide();
        $("#resultsContainer").html("");
        var currentPage = parseInt($("#hdnPage").val());
      
        var info = "{ nombre: '" + $("#txtNombre").val()
                   + "', apellido: '" + $("#txtApellido").val()
                   + "', cuit: '" + $("#txtCUIT").val()
                   + "', page: " + currentPage + ", pageSize: " + PAGE_SIZE
                   + "}";

        $.ajax({
            type: "POST",
            url: "/modulos/rrhh/empleados.aspx/getResults",
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
                $("#imgLoading").hide();
                $("#divError").show();
                $('html, body').animate({ scrollTop: 0 }, 'slow');
            }
        });
        empleados.resetearExportacion();
    },

    verTodos: function () {
        $("#txtCUIT, #txtApellido,#txtNombre").val("");
        empleados.filtrar();
    },

    exportar: function () {
        empleados.resetearExportacion();
        $("#imgLoading").show();
        $("#divIconoDescargar").hide();

        var info = "{ nombre: '" + $("#txtNombre").val()
                    + "', apellido: '" + $("#txtApellido").val()
                    + "', cuit: '" + $("#txtCUIT").val()
                   + "'}";

        $.ajax({
            type: "POST",
            url: "/modulos/rrhh/empleados.aspx/export",
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
                $("#imgLoading").hide();
                $("#divError").show();
                $('html, body').animate({ scrollTop: 0 }, 'slow');
                empleados.resetearExportacion();
            }
        });
    },
    resetearExportacion: function () {
        $("#imgLoading, #lnkDownload").hide();
        $("#divIconoDescargar").show();
    }
}