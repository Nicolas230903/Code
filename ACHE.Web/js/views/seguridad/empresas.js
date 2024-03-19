var Empresa = {
    configForm: function () {


        $(".select2").select2({ width: '100%', allowClear: true });
        Common.obtenerProvincias("ddlProvincia", $("#hdnProvincia").val(), true);
        Common.obtenerCiudades("ddlCiudad", $("#hdnCiudad").val(), $("#hdnProvincia").val(), true);



        // Date Picker
        //jQuery('.validDate').datepicker();
        configDatePicker();

        $("#txtNroDocumento, #txtNuevoPunto").numericInput();

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
            return CuitEsValido($("#txtCuit").val());
        }, "CUIT Inválido");


        $('#flpArchivo').fileupload({
            url: 'subirImagenes.ashx?IDUsuario=' + $("#IDEmpresa").val + "&opcionUpload=LogoEmpresas",
            success: function (response, status) {
                if (response.name != "ERROR") {
                    $("#hdnFileName").val(response.name);
                    $("#imgLogo").attr("src", "/files/usuarios/" + response.name);
                }
                else {
                    $("#hdnFileName").val("");
                    $("#msgError").html("logo no ingresado");
                    $("#divError").show();
                    $("#divOk").hide();
                }
            },
            error: function (error) {
                $("#hdnFileName").val("");
            }
        });

    },
    grabar: function () {
        Empresa.ocultarMensajes();
        $("#btnDomicilio,#btnDatosPrincipales").attr("disabled", true);

        if ($('#frmEdicion').valid()) {
            Common.mostrarProcesando("btnDatosPrincipales,#btnDomicilio,#btnActualizarPortalClientes,#btnActualizarTemplate");

            var info = "{ razonSocial: '" + $("#txtRazonSocial").val()
                    + "', condicionIva: '" + $("#ddlCondicionIva").val()
                    + "', cuit: '" + $("#txtCuit").val()
                    + "', iibb: '" + $("#txtIIBB").val()
                    + "', fechaInicio: '" + $("#txtFechaInicioAct").val()
                    + "', personeria: '" + $("#ddlPersoneria").val()
                    + "', email: '" + $("#txtEmail").val()
                    + "', emailAlertas: '" + $("#txtEmailAlertas").val()
                    + "', telefono: '" + $("#txtTelefono").val()
                    + "', celular: '" + $("#txtCelular").val()
                    + "', contacto: '" + $("#txtContacto").val()
                    + "', idProvincia: '" + $("#ddlProvincia").val()
                    + "', idCiudad: '" + $("#ddlCiudad").val()
                    + "', domicilio: '" + $("#txtDomicilio").val()
                    + "', pisoDepto: '" + $("#txtPisoDepto").val()
                    + "', cp: '" + $("#txtCp").val()
                    + "'}";

            $.ajax({
                type: "POST",
                url: "/modulos/seguridad/empresase.aspx/guardar",
                data: info,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data, text) {
                    $('#divOk').show();
                    $("#divError").hide();
                    $('html, body').animate({ scrollTop: 0 }, 'slow');
                    $("#btnDomicilio,#btnDatosPrincipales").attr("disabled", false);

                    Empresa.obtenerPuntos();
                    Empresa.ObtenerDatosEmpresa();
                },
                error: function (response) {
                    var r = jQuery.parseJSON(response.responseText);
                    $("#msgError").html(r.Message);
                    $("#divError").show();
                    $("#divOk").hide();
                    $('html, body').animate({ scrollTop: 0 }, 'slow');
                    $("#btnDomicilio,#btnDatosPrincipales").attr("disabled", false);
                    Common.ocultarProcesando("btnDatosPrincipales,#btnDomicilio,#btnActualizarPortalClientes,#btnActualizarTemplate", "Actualizar");
                }
            });
            
        }
        else {
            $("#btnDomicilio,#btnDatosPrincipales").attr("disabled", false);
            return false;
        }
    },
    changeProvincia: function () {
        Common.obtenerCiudades("ddlCiudad", "", $("#ddlProvincia").val(), true);
    },

    eliminarPunto: function (id) {
        Empresa.ocultarMensajes();

        //var info = "{ punto: " + parseInt(id) + "}";

        $.ajax({
            type: "POST",
            url: "/modulos/seguridad/empresase.aspx/eliminarPunto",
            data: "{}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data, text) {
                Empresa.obtenerPuntos();
            },
            error: function (response) {
                var r = jQuery.parseJSON(response.responseText);
                $("#msgErrorPuntos").html(r.Message);
                $("#divErrorPuntos").show();
            }
        });
    },
    agregarPunto: function () {
        Empresa.ocultarMensajes();

        if ($("#txtNuevoPunto").val() != "") {

            var info = "{ punto: " + parseInt($("#txtNuevoPunto").val()) + "}";

            $.ajax({
                type: "POST",
                url: "/modulos/seguridad/empresase.aspx/agregarPunto",
                data: info,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data, text) {
                    Empresa.obtenerPuntos()
                },
                error: function (response) {
                    var r = jQuery.parseJSON(response.responseText);
                    $("#msgErrorPuntos").html(r.Message);
                    $("#divErrorPuntos").show();
                }
            });

        }
        else {
            $("#msgErrorPuntos").html("Debes ingresar un valor");
            $("#divErrorPuntos").show();
        }
    },
    obtenerPuntos: function () {
        $.ajax({
            type: "POST",
            url: "/modulos/seguridad/empresase.aspx/obtenerPuntos",
            //data: "{idFactura: " + parseInt(dataItem.ID) + "}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                if (data != null) {
                    $("#bodyDetalle").html(data.d);
                }
            }
        });
    },
    ponerPorDefecto: function (idPunto) {
        $.ajax({
            type: "POST",
            data: "{ idPunto: " + idPunto + " }",
            url: "/modulos/seguridad/empresase.aspx/GuardarPorDefecto",
            //data: "{idFactura: " + parseInt(dataItem.ID) + "}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                if (data != null) {
                    $("#bodyDetalle").html(data.d);
                }
            }
        });
    },
    ocultarMensajes: function () {
        $("#divError, #divOk, #divErrorPuntos").hide();
    },
    ocultarDatosExtras: function () {
        $("#divProfile, #divfoto,#liPuntos,#liPortalClientes,#liTemplate").hide();
    },
    mostrarDatosExtras: function () {
        $("#divProfile, #divfoto,#liPuntos,#liPortalClientes,#liTemplate").show();
    },

    ObtenerDatosEmpresa: function () {
        
        $.ajax({
            type: "POST",
            url: "/modulos/seguridad/empresase.aspx/ObtenerDatosEmpresa",
            data: "{}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data, text) {

                $("#IDEmpresa").val(data.d.Items[0].ID);
                $("#idName").text(data.d.Items[0].RazonSocial);
                $("#idPosition").text("CUIT: " + data.d.Items[0].CUIT);

                if (data.d.Items[0].Domicilio != "" && data.d.Items[0].Ciudad != "" && data.d.Items[0].Provincia != "") {
                    $("#idLocation").text(data.d.Items[0].Domicilio + ", " + data.d.Items[0].Ciudad + ", " + data.d.Items[0].Provincia);
                }
                else {
                    $("#idLocation").text("Domicilio fiscal no informado");
                }

                $("#chkPortalClientes").prop("checked", data.d.Items[0].CorreoPortal);
                $("#ChkCorreoPortal").prop("checked", data.d.Items[0].PortalClientes);

                Empresa.mostrarDatosExtras();
            },
            error: function (response) {
                var r = jQuery.parseJSON(response.responseText);
                $("#msgErrorPuntos").html(r.Message);
                $("#divErrorPuntos").show();
                return false;
            }
        });

    },
    portalClientes: function () {
        Empresa.ocultarMensajes();

        var info = "{ ChkCorreoPortal: " + $("#ChkCorreoPortal").is(':checked') + ", chkPortalClientes: " + $("#chkPortalClientes").is(':checked') + " }";

        $.ajax({
            type: "POST",
            url: "/modulos/seguridad/empresase.aspx/portalClientes",
            data: info,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data, text) {
                $('#divOk').show();
                $("#divError").hide();
                $('html, body').animate({ scrollTop: 0 }, 'slow');
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
    ActualizarTemplate: function () {

        Empresa.ocultarMensajes();

        var info = "{ ddlTemplate: '";

        if ($("#default1").is(":checked")) {
            info += $("#default1").attr('value');
        } else if ($("#default2").is(":checked")) {
            info += $("#default2").attr('value');
        } else {
            info += $("#default3").attr('value');
        }
        info += "'}";

        $.ajax({
            type: "POST",
            url: "/modulos/seguridad/empresase.aspx/ActualizarTemplate",
            data: info,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data, text) {
                $('#divOk').show();
                $("#divError").hide();
                $('html, body').animate({ scrollTop: 0 }, 'slow');
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

    //SEARCH
    configFilters: function () {
        /*$("#txtNroDocumento").numericInput();

        $("#txtRazonSocial, #txtNroDocumento").keypress(function (event) {
            var keycode = (event.keyCode ? event.keyCode : event.which);
            if (keycode == '13') {
                resetearPagina();
                Empresa.filtrar();
                return false;
            }
        });

        */
    },
    cambiarSesion: function (id, nombre) {

        bootbox.confirm("¿Está seguro que desea iniciar sesión con la empresa " + nombre + "?", function (result) {
            if (result) {

                var info = "{ idEmpresa: '" + id + "'}";
                $.ajax({
                    type: "POST",
                    url: "/common.aspx/cambiarEmpresa",
                    data: info,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data, text) {
                        window.location.href = "/home.aspx";
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
    cambiarPass: function () {
        window.location.href = "/modulos/seguridad/cambiar-pwd.aspx?ID= " + $("#IDEmpresa").val();
    },
    nuevo: function () {
        window.location.href = "/modulos/seguridad/empresase.aspx";
    },
    editar: function (id) {
        window.location.href = "/modulos/seguridad/empresase.aspx?ID=" + id;
    },
    eliminar: function (id, nombre) {
        bootbox.confirm("¿Está seguro que desea eliminar a " + nombre + "?", function (result) {
            if (result) {
                $.ajax({
                    type: "POST",
                    url: "/modulos/seguridad/empresas.aspx/delete",
                    data: "{ id: " + id + "}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data, text) {
                        Empresa.filtrar();
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
    filtrar: function () {
        $("#divError").hide();
        $("#resultsContainer").html("");
       
        $.ajax({
            type: "POST",
            url: "/modulos/seguridad/empresas.aspx/getResults",
            data: {},
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data, text) {
                if (data.d.TotalPage > 0) {

                    if (parseInt($("#IDUsuarioAdicional").val()) == 0) {
                        $("#btnNuevo").show()
                    }
                    else {
                        $("#btnNuevo").hide()
                    }
                }
                else {
                    //$("#divPagination").hide();
                    $("#msjResultados").html("");
                }

                $("#IDUsuarioActual").val(data.d.UsuLogiado);
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
    },
    verTodos: function () {
        Empresa.filtrar();
    }
}