var MisDatos = {
    configForm: function () {

        $(".select2").select2({ width: '100%', allowClear: true });

        Common.obtenerProvincias("ddlProvincia", $("#hdnProvincia").val(), true);
        if ($("#hdnProvincia").val() != null)
            Common.obtenerCiudades("ddlCiudad", $("#hdnCiudad").val(), $("#hdnProvincia").val(), true);
        $("#hdnCiudad").val("");

        $('#flpArchivo').fileupload({
            url: '/subirImagenes.ashx?IDUsuario=' + $("#IDusuario").val() + "&opcionUpload=LogoEmpresas",
            success: function (response, status) {
                if (response.name != "ERROR") {

                    //MisDatos.ActualizarLogoSesion(response.name);
                    $("#hdnFileName").val(response.name);
                    var aux = new Date();
                    $("#imgLogo").attr("src", "/files/usuarios/" + response.name + "?" + aux.getTime());//agrego fecha para cuando se actualiza el logo
                    $("#imgLogo")
                    $("#divError").hide();
                    $("#divOk").show();
                    $("#divLogo").slideToggle();
                    $("#hdnTieneFoto").val("1");
                    foto.showBtnEliminar();
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
                $("#msgError").html(error.responseText);
                $("#divError").show();
                $("#divOk").hide();
                $('html, body').animate({ scrollTop: 0 }, 'slow');

            }
        });
        Common.configTelefono("txtTelefono");
        Common.configTelefono("txtCelular");
        // Date Picker
        //jQuery('.validDate').datepicker();
        configDatePicker();

        $("#txtNroDocumento, #txtNuevoPunto,#txtIIBB").numericInput();

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

        // Validation with select boxes
        $("#modalPrimerAviso").validate({
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

        $.validator.addMethod("validCuitMisDatos", function (value, element) {
            return CuitEsValido($("#txtCuit").val());
        }, "CUIT Inválido");
        MisDatos.obtenerHistorialDePagos();

        if ($("#hdnAccion").val() == "completaTusDatos") {
            $("#liTemplate,#liportalClientes,#liDatosFiscales,#liCambiarpwd,#Usuarios,#liEmpresas,#liplanPagos,#liConfiguracion").hide()
        }
        else if ($("#hdnAccion").val() == "preferencias") {
            $("#liDatosPrincipales,#liDomicilio,#divFotoPerfil,#liCambiarpwd,#Usuarios,#liEmpresas").hide()
            $("#liTemplate,#liportalClientes,#liDatosFiscales").show()

            $("#divDatosPersonales").removeClass("col-sm-9").addClass("col-sm-12");
            $("#liDatosPrincipales").removeClass("active");
            $("#liportalClientes").addClass("active");
            $("#info").removeClass("tab-pane active").addClass("tab-pane");
            $("#portalClientes").removeClass("tab-pane").addClass("tab-pane active");

            $("#spanMisDatosUbicacion").html("Preferencias");
            $("#spanMisDatos").html("<i class='fa fa-user'></i> Preferencias");

            if (MI_CONDICION == "MO") {
                $("#liDatosFiscales").hide();
            }
        }
        MisDatos.changeIIBB();
        foto.showBtnEliminar();

        MisDatos.habilitarPrimerAviso();
        MisDatos.habilitarSegundoAviso();
        MisDatos.habilitarTercerAviso();
        MisDatos.habilitarEnvioFE();
        MisDatos.habilitarEnvioCR();
        MisDatos.ObtenerAvisosVencimientos();

        $("#txtDiasPrimerAviso,#txtDiasSegundoAviso,#txtDiasTercerAviso").numericInput();

        var CtasVentas = $("#hdnCuentasVentas").val().split(",");
        var CtasCompras = $("#hdnCuentasCompras").val().split(",");
        $("#ddlCuentasVentas").val(CtasVentas);
        $("#ddlCuentasCompras").val(CtasCompras);
        $("#ddlCuentasVentas").trigger("change");
        $("#ddlCuentasCompras").trigger("change");


        var jurisdicciones = $("#hdnJuresdiccion").val().split(",");
        Common.obtenerProvincias("ddlJuresdiccion", jurisdicciones, true);
        $(".select3").select2({ width: '100%', allowClear: true });
    },
    changeProvincia: function () {
        if ($("#hdnCiudad").val() == "")
            Common.obtenerCiudades("ddlCiudad", $("#ddlCiudad").val(), $("#ddlProvincia").val(), true);
        // $("#hdnCiudad").val("");
    },
    ActualizarLogoSesion: function (nombre) {

        var info = "{ Nombrelogo: '" + nombre + "'}";

        $.ajax({
            type: "POST",
            url: "/modulos/seguridad/mis-datos.aspx/ActualizarLogoSesion",
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
    grabar: function () {
        MisDatos.ocultarMensajes();

        if ($('#frmEdicion').valid()) {

            Common.mostrarProcesando("btnActualizarInfo");
            Common.mostrarProcesando("btnActualizarDomicilio");
            Common.mostrarProcesando("btnActualizarDatosFiscales");

            var jurisdicciones = ($("#ddlJuresdiccion").val() != null && $("#ddlJuresdiccion").val() != "") ? $("#ddlJuresdiccion").val() : "";
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
                    + "', esAgentePersepcionIVA: " + $("#esAgentePersepcionIVA").is(':checked')
                    + " , esAgentePersepcionIIBB: " + $("#esAgentePersepcionIIBB").is(':checked')
                    + " , esAgenteRetencionGanancia: " + $("#esAgenteRetencionGanancia").is(':checked')
                    + " , esAgenteRetencion: " + $("#esAgenteRetencion").is(':checked')
                    + " , exentoIIBB: " + $("#chkExento").is(':checked')
                    + " , fechaCierreContable: '" + $("#txtFechaCierreContable").val()
                    + "', idJurisdiccion: '" + jurisdicciones
                    + "', cbu: '" + $("#txtCBU").val()
                    + "', textoFinalFactura: '" + $("#txtTextoFinalFactura").val()
                    + "'}";

            $.ajax({
                type: "POST",
                url: "/modulos/seguridad/mis-datos.aspx/guardar",
                data: info,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data, text) {
                    $('#divOk').show();
                    $("#divError").hide();
                    $('html, body').animate({ scrollTop: 0 }, 'slow');
                    Common.ocultarProcesando("btnActualizarInfo", "Actualizar");
                    Common.ocultarProcesando("btnActualizarDomicilio", "Actualizar");
                    Common.ocultarProcesando("btnActualizarDatosFiscales", "Actualizar");
                },
                error: function (response) {
                    var r = jQuery.parseJSON(response.responseText);
                    $("#msgError").html(r.Message);
                    $("#divError").show();
                    $("#divOk").hide();
                    $('html, body').animate({ scrollTop: 0 }, 'slow');
                    Common.ocultarProcesando("btnActualizarInfo", "Actualizar");
                    Common.ocultarProcesando("btnActualizarDomicilio", "Actualizar");
                    Common.ocultarProcesando("btnActualizarDatosFiscales", "Actualizar");
                }
            });
        }
        else {
            return false;
        }
    },
    eliminarPunto: function (id) {
        MisDatos.ocultarMensajes();

        var info = "{ punto: " + parseInt(id) + "}";

        $.ajax({
            type: "POST",
            url: "/modulos/seguridad/mis-datos.aspx/eliminarPunto",
            data: info,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data, text) {
                MisDatos.obtenerPuntos();
            },
            error: function (response) {
                var r = jQuery.parseJSON(response.responseText);
                $("#msgErrorPuntos").html(r.Message);
                $("#divErrorPuntos").show();
            }
        });
    },
    eliminarUsuario: function (id) {
        var opcion = confirm("Eliminar Usuario?");
        if (opcion == true) {
            MisDatos.ocultarMensajes();

            var info = "{ idUsuario: " + parseInt($("#IDusuario").val()) + "}";

            $.ajax({
                type: "POST",
                url: "/modulos/seguridad/mis-datos.aspx/eliminarUsuario",
                contentType: "application/json; charset=utf-8",
                data: info,
                dataType: "json",
                success: function (data, text) {
                    $("#msgOkEliminarUsuario").html('Usuario eliminado correctamente! Se cerrara la sesión.');
                    $("#divOkEliminarUsuario").show();
                    setTimeout('window.location.replace("../../login.aspx?logOut=true");', 3000);
                },
                error: function (response) {
                    var r = jQuery.parseJSON(response.responseText);
                    $("#msgErrorEliminarUsuario").html(r.Message);
                    $("#divErrorEliminarUsuario").show();
                }
            });
        }         
    },
    habilitarPuntoVenta: function (id) {
        var info = "{ id: " + id + " }";
        $.ajax({
            type: "POST",
            url: "/modulos/seguridad/mis-datos.aspx/HabilitarPuntoVenta",
            data: info,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                MisDatos.obtenerPuntos();
            },
            error: function (response) {
                var r = jQuery.parseJSON(response.responseText);
                $("#msgErrorPuntos").html(r.Message);
                $("#divErrorPuntos").show();
            }
        });
    },
    agregarPunto: function () {
        MisDatos.ocultarMensajes();

        if ($("#txtNuevoPunto").val() != "") {

            Common.mostrarProcesando("btnActualizarPunto");
            var info = "{ punto: " + parseInt($("#txtNuevoPunto").val()) + "}";

            $.ajax({
                type: "POST",
                url: "/modulos/seguridad/mis-datos.aspx/agregarPunto",
                data: info,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data, text) {
                    MisDatos.obtenerPuntos();
                    Common.ocultarProcesando("btnActualizarPunto", "Agregar");
                },
                error: function (response) {
                    var r = jQuery.parseJSON(response.responseText);
                    $("#msgErrorPuntos").html(r.Message);
                    $("#divErrorPuntos").show();
                    Common.ocultarProcesando("btnActualizarPunto", "Agregar");
                }
            });

        }
        else {
            $("#msgErrorPuntos").html("Debes ingresar un valor");
            $("#divErrorPuntos").show();
        }
    },    
    obtenerPuntos: function () {
        consultarPuntosDeVentaAfip();
        $.ajax({
            type: "GET",
            url: "/modulos/seguridad/mis-datos.aspx/obtenerPuntos",
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
            url: "/modulos/seguridad/mis-datos.aspx/GuardarPorDefecto",
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
    agregarActividad: function () {
        MisDatos.ocultarMensajes();

        if ($("#txtNuevaActividad").val() != "") {

            Common.mostrarProcesando("btnActualizarActividad");
            var info = "{ codigo: '" + $("#txtNuevaActividad").val() + "'}";

            $.ajax({
                type: "POST",
                url: "/modulos/seguridad/mis-datos.aspx/agregarActividad",
                data: info,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data, text) {
                    MisDatos.obtenerActividades();
                    Common.ocultarProcesando("btnActualizarActividad", "Agregar");
                },
                error: function (response) {
                    var r = jQuery.parseJSON(response.responseText);
                    $("#msgErrorActividades").html(r.Message);
                    $("#divErrorActividades").show();
                    Common.ocultarProcesando("btnActualizarActividad", "Agregar");
                }
            });

        }
        else {
            $("#msgErrorActividades").html("Debes ingresar un valor");
            $("#divErrorActividades").show();
        }
    }, 
    obtenerActividades: function () {
        $.ajax({
            type: "GET",
            url: "/modulos/seguridad/mis-datos.aspx/obtenerActividades",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                if (data != null) {
                    $("#bodyDetalleActividades").html(data.d);
                }
            }
        });
    },
    ponerActividadPorDefecto: function (idActividad) {
        $.ajax({
            type: "POST",
            data: "{ idActividad: " + idActividad + " }",
            url: "/modulos/seguridad/mis-datos.aspx/GuardarActividadPorDefecto",
            //data: "{idFactura: " + parseInt(dataItem.ID) + "}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                if (data != null) {
                    $("#bodyDetalleActividades").html(data.d);
                }
            }
        });
    },
    ocultarMensajes: function () {
        $("#divError, #divOk, #divErrorPuntos, #divOkEliminarUsuario, #divErrorEliminarUsuario, #divErrorActividades").hide();
    },
    verificarClientes: function () {
        if ($("#txtClientSecret").val() == "" && $("#txtClientId").val() != "") {
            $("#msgError").html("Si engresa El client ID debe ingresar el Client Secret");
            $("#divError").show();
            $('html, body').animate({ scrollTop: 0 }, 'slow');
            return false;
        }
        if ($("#txtClientId").val() == "" && $("#txtClientSecret").val() != "") {
            $("#msgError").html("Si engresa El Client Secret, debe ingresar el clientID");
            $("#divError").show();
            $('html, body').animate({ scrollTop: 0 }, 'slow');
            return false;
        }

        return true;
    },
    portalClientes: function () {
        MisDatos.ocultarMensajes();
        if (MisDatos.verificarClientes()) {


            var info = "{ ChkCorreoPortal: " + $("#ChkCorreoPortal").is(':checked')
                     + ", chkPortalClientes: " + $("#chkPortalClientes").is(':checked')
                     + ", clientId: '" + $("#txtClientId").val()
                     + "', clientSecret: '" + $("#txtClientSecret").val()
                     + "'}";

            Common.mostrarProcesando("btnActualizarPortalClientes");
            $.ajax({
                type: "POST",
                url: "/modulos/seguridad/mis-datos.aspx/portalClientes",
                data: info,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data, text) {
                    $('#divOk').show();
                    $("#divError").hide();
                    $('html, body').animate({ scrollTop: 0 }, 'slow');
                    Common.ocultarProcesando("btnActualizarPortalClientes", "Actualizar");
                },
                error: function (response) {
                    var r = jQuery.parseJSON(response.responseText);
                    $("#msgError").html(r.Message);
                    $("#divError").show();
                    $("#divOk").hide();
                    $('html, body').animate({ scrollTop: 0 }, 'slow');
                    Common.ocultarProcesando("btnActualizarPortalClientes", "Actualizar");
                }
            });
        }
    },
    ActualizarTemplate: function () {
        MisDatos.ocultarMensajes();
        Common.mostrarProcesando("btnActualizarTemplate");
        var info = "{ ddlTemplate: '" + $("input[name='ctl00$MainContent$dlltemplate']:checked").attr('value') + "'}";

        $.ajax({
            type: "POST",
            url: "/modulos/seguridad/mis-datos.aspx/ActualizarTemplate",
            data: info,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data, text) {
                $('#divOk').show();
                $("#divError").hide();
                $('html, body').animate({ scrollTop: 0 }, 'slow');
                Common.ocultarProcesando("btnActualizarTemplate", "Actualizar");
            },
            error: function (response) {
                var r = jQuery.parseJSON(response.responseText);
                $("#msgError").html(r.Message);
                $("#divError").show();
                $("#divOk").hide();
                $('html, body').animate({ scrollTop: 0 }, 'slow');
                Common.ocultarProcesando("btnActualizarTemplate", "Actualizar");
            }
        });
    },
    obtenerHistorialDePagos: function () {
        $("#resultsContainer").html("");
        $.ajax({
            type: "GET",
            url: "/modulos/seguridad/mis-datos.aspx/ObtenerHistorialPagos",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data, text) {

                if (data.d.Items.length > 0) {
                    $("#resultTemplate").tmpl({ results: data.d.Items }).appendTo("#resultsContainer");

                    $("#hdnIdPlan").val(data.d.IDPlanActual);
                    $("#spNombrePlan").html(data.d.NombrePlanActual);
                    $("#spFechaPlan").html(data.d.FechaVencimiento);
                }
                else
                    $("#noResultTemplate").tmpl({ results: data.d.Items }).appendTo("#resultsContainer");
            },
            error: function (response) {
                var r = jQuery.parseJSON(response.responseText);
                alert(r.Message);
            }
        });

    },
    upgradePlanActual: function () {
        window.location.href = "/modulos/seguridad/elegir-plan.aspx?upgrade=1";
    },
    changeIIBB: function () {
        if ($("#chkExento").is(':checked')) {
            $("#txtIIBB").val("");
            $("#txtIIBB").attr("disabled", "true");
        }
        else {
            $("#txtIIBB").removeAttr("disabled")
        }
    },
    guardarConfiguracion: function (id) {
        $("#divErrorConfiguracion").hide();
        var precioUnitario = ($("input[name='ctl00$MainContent$rPrecioUnitario']:checked").attr('value') == "1") ? true : false;
        var pedidoDeVenta = ($("input[name='ctl00$MainContent$rPedidoDeVenta']:checked").attr('value') == "1") ? true : false;
        var paraPDVSolicitarCompletarContacto = ($("input[name='ctl00$MainContent$rParaPDVSolicitarCompletarContacto']:checked").attr('value') == "1") ? true : false;
        var esVendedor = ($("#chkEsVendedor").is(":checked")) ? true : false;
        var porcentajeComision = $("#txtPorcentajeComision").val();
        var facturaSoloContraEntrega = ($("#chkFacturaSoloContraEntrega").is(":checked")) ? true : false;
        var usaCantidadConDecimales = ($("#chkUsaCantidadConDecimales").is(":checked")) ? true : false;

        var info = "{ usaPrecioUnitarioConIva: " + precioUnitario
            + ", usaPedidoDeVenta: " + pedidoDeVenta
            + ", paraPDVSolicitarCompletarContacto: " + paraPDVSolicitarCompletarContacto 
            + ", esVendedor: " + esVendedor 
            + ", porcentajeComision: '" + porcentajeComision
            + "', facturaSoloContraEntrega: " + facturaSoloContraEntrega
            + ", usaCantidadConDecimales: " + usaCantidadConDecimales
            + "}";
        $.ajax({
            type: "POST",
            url: "/modulos/seguridad/mis-datos.aspx/guardarConfiguracion",
            data: info,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                $("#divOkConfiguracion").show();
                setTimeout('document.location.reload()', 2000);
            },
            error: function (response) {
                var r = jQuery.parseJSON(response.responseText);
                $("#msgErrorConfiguracion").html(r.Message);
                $("#divErrorConfiguracion").show();
            }
        });
    },    
    guardarConfiguracionPlanDeCuenta: function () {
        $("#divErrorPlanDeCuenta").hide();

        //Comprobantes
        var IDCtaProveedores = ($("#ddlCtaProveedoresComprobante").val() == "" || $("#ddlCtaProveedoresComprobante").val() == null) ? 0 : parseInt($("#ddlCtaProveedoresComprobante").val());
        var IDCtaIIBB = ($("#ddlCtaIIBBComprobante").val() == "" || $("#ddlCtaIIBBComprobante").val() == null) ? 0 : parseInt($("#ddlCtaIIBBComprobante").val());
        var IDCtaIVACreditoFiscal = ($("#ddlIVACreditoFiscalComprobante").val() == "" || $("#ddlIVACreditoFiscalComprobante").val() == null) ? 0 : parseInt($("#ddlIVACreditoFiscalComprobante").val());
        var IDCtaPercepcionIVA = ($("#ddlPercepcionIVAComprobante").val() == "" || $("#ddlPercepcionIVAComprobante").val() == null) ? 0 : parseInt($("#ddlPercepcionIVAComprobante").val());
        var IDCtaConceptosNoGravadosxCompras = ($("#ddlNoGravadoCompras").val() == "" || $("#ddlNoGravadoCompras").val() == null) ? 0 : parseInt($("#ddlNoGravadoCompras").val());
        
        //Pagos
        var IDCtaBancos = ($("#ddlBanco").val() == "" || $("#ddlBanco").val() == null) ? 0 : parseInt($("#ddlBanco").val());
        var IDCtaCaja = ($("#ddlCaja").val() == "" || $("#ddlCaja").val() == null) ? 0 : parseInt($("#ddlCaja").val());
        var IDCtaValoresADepositar = ($("#ddlValoresADepositar").val() == "" || $("#ddlValoresADepositar").val() == null) ? 0 : parseInt($("#ddlValoresADepositar").val());
        var IDctaRetIIBB = ($("#ddlRetIIBB").val() == "" || $("#ddlRetIIBB").val() == null) ? 0 : parseInt($("#ddlRetIIBB").val());
        var IDctaRetIVA = ($("#ddlRetIVA").val() == "" || $("#ddlRetIVA").val() == null) ? 0 : parseInt($("#ddlRetIVA").val());
        var IDctaRetGanancias = ($("#ddlRetGanancias").val() == "" || $("#ddlRetGanancias").val() == null) ? 0 : parseInt($("#ddlRetGanancias").val());
        var IDctaRetSUSS = ($("#ddlRetSUSS").val() == "" || $("#ddlRetSUSS").val() == null) ? 0 : parseInt($("#ddlRetSUSS").val());

        //ventas
        var IDCtaIVADebitoFiscal = ($("#ddlIVADebitoFiscal").val() == "" || $("#ddlIVADebitoFiscal").val() == null) ? 0 : parseInt($("#ddlIVADebitoFiscal").val());
        var IDCtaDeudoresPorVentas = ($("#ddlDeudoresPorVenta").val() == "" || $("#ddlDeudoresPorVenta").val() == null) ? 0 : parseInt($("#ddlDeudoresPorVenta").val());
        var IDCtaConceptosNoGravadosxVentas = ($("#ddlNoGravadoVentas").val() == "" || $("#ddlNoGravadoVentas").val() == null) ? 0 : parseInt($("#ddlNoGravadoVentas").val());

        var info = "{ IDCtaProveedores: " + IDCtaProveedores
                 + " ,IDCtaIIBB: " + IDCtaIIBB
                 + " ,IDCtaIVACreditoFiscal: " + IDCtaIVACreditoFiscal
                 + " ,IDCtaPercepcionIVA: " + IDCtaPercepcionIVA
                 + " ,IDCtaBancos: " + IDCtaBancos
                 + " ,IDCtaCaja: " + IDCtaCaja
                 + " ,IDCtaValoresADepositar: " + IDCtaValoresADepositar
                 + " ,IDctaRetIIBB: " + IDctaRetIIBB
                 + " ,IDctaRetIVA: " + IDctaRetIVA
                 + " ,IDctaRetGanancias: " + IDctaRetGanancias
                 + " ,IDCtaIVADebitoFiscal: " + IDCtaIVADebitoFiscal
                 + " ,IDCtaDeudoresPorVentas: " + IDCtaDeudoresPorVentas
                 + " ,IDctaRetSUSS: " + IDctaRetSUSS
                 + " ,ctasFiltrosCompras: '" + $("#ddlCuentasCompras").val()
                 + "',ctasFiltrosVentas: '" + $("#ddlCuentasVentas").val()
                 + "',IDCtaConceptosNoGravadosxCompras: " + IDCtaConceptosNoGravadosxCompras
                 + " ,IDCtaConceptosNoGravadosxVentas: " + IDCtaConceptosNoGravadosxVentas
                 + " }";

        $.ajax({
            type: "POST",
            url: "/modulos/seguridad/mis-datos.aspx/guardarConfiguracionPlanDeCuenta",
            data: info,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                $("#divOkPlanDeCuenta").show();
            },
            error: function (response) {
                var r = jQuery.parseJSON(response.responseText);
                $("#msgErrorPlanDeCuenta").html(r.Message);
                $("#divErrorPlanDeCuenta").show();
            }
        });
    },
    //*** AvisosVencimientos***/
    guardarAlertasyAvisos: function () {
        $("#divOkAlertasyAvisos,#divErrorAlertasyAvisos").hide();
        var modoPrimeAviso = ($("#rPrimerAvisoAntes").is(":checked") == true) ? "1" : "0";
        var modoSegundoAviso = ($("#rSegundoAvisoAntes").is(":checked") == true) ? "1" : "0";
        var modoTercerAviso = ($("#rTercerAvisoAntes").is(":checked") == true) ? "1" : "0";

        var diasPrimeAviso = ($("#txtDiasPrimerAviso").val() == "" || $("#txtDiasPrimerAviso").val() == null) ? "0" : parseInt($("#txtDiasPrimerAviso").val())
        var diasSegundoAviso = ($("#txtDiasSegundoAviso").val() == "" || $("#txtDiasSegundoAviso").val() == null) ? "0" : parseInt($("#txtDiasSegundoAviso").val())
        var diasTercerAviso = ($("#txtDiasTercerAviso").val() == "" || $("#txtDiasTercerAviso").val() == null) ? "0" : parseInt($("#txtDiasTercerAviso").val())

        if (MisDatos.validarAvisosVencimientos() == false) {
            $("#divErrorAlertasyAvisos").show();
            return false;
        }

        var datos = new Array();
        var obj = new Object();

        obj.TipoAlerta = "Primer aviso";
        obj.Activa = $("#chkPrimerAviso").is(":checked");
        obj.ModoDeEnvio = modoPrimeAviso;
        obj.CantDias = diasPrimeAviso;
        obj.Asunto = $("#txtAsuntoPrimerAviso").val();
        obj.Mensaje = $("#txtMensajePrimerAviso").val();
        datos.push(obj);
        var obj = new Object();
        obj.TipoAlerta = "Segundo aviso";
        obj.Activa = $("#chkSegundoAviso").is(":checked");
        obj.ModoDeEnvio = modoSegundoAviso;
        obj.CantDias = diasSegundoAviso;
        obj.Asunto = $("#txtAsuntoSegundoAviso").val();
        obj.Mensaje = $("#txtMensajeSegundoAviso").val();
        datos.push(obj);
        var obj = new Object();
        obj.TipoAlerta = "Tercer aviso";
        obj.Activa = $("#chkTercerAviso").is(":checked");
        obj.ModoDeEnvio = modoTercerAviso;
        obj.CantDias = diasTercerAviso;
        obj.Asunto = $("#txtAsuntoTercerAviso").val();
        obj.Mensaje = $("#txtMensajeTercerAviso").val();
        datos.push(obj);

        var obj = new Object();
        obj.TipoAlerta = "Envio FE";
        obj.Activa = $("#chkEnvioFE").is(":checked");
        obj.ModoDeEnvio = 0;
        obj.CantDias = 0;
        obj.Asunto = $("#txtAsuntoEnvioFE").val();
        obj.Mensaje = $("#txtMensajeEnvioFE").val();
        datos.push(obj);

        var obj = new Object();
        obj.TipoAlerta = "Envio CR";
        obj.Activa = $("#chkEnvioCR").is(":checked");
        obj.ModoDeEnvio = 0;
        obj.CantDias = 0;
        obj.Asunto = $("#txtAsuntoEnvioCR").val();
        obj.Mensaje = $("#txtMensajeEnvioCR").val();
        datos.push(obj);

        var obj = new Object();
        obj.TipoAlerta = "Stock";
        obj.Activa = $("#chkStock").is(":checked");
        obj.ModoDeEnvio = 0;
        obj.CantDias = 0;
        obj.Asunto = "";
        obj.Mensaje = "";
        datos.push(obj);

        $.ajax({
            type: "POST",
            url: "/modulos/seguridad/mis-datos.aspx/GuardarAlertasyAvisos",
            data: "{listaAvisos: " + JSON.stringify(datos) + "}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                $("#divOkAlertasyAvisos").show();
            },
            error: function (response) {
                var r = jQuery.parseJSON(response.responseText);
                $("#msgErrorAlertasyAvisos").html(r.Message);
                $("#divErrorAlertasyAvisos").show();
            }
        });
    },
    guardarAsuntoYMensaje: function () {
        if ($("#frmNuevaAlertasyAvisos").valid()) {
            $('#modalAlertasYAvisos').modal('toggle');
        } else {
            return false;
        }
    },
    habilitarPrimerAviso: function () {
        if ($(".divPrimerAviso :input").is(":disabled")) {
            $(".divPrimerAviso :input").attr("disabled", false);
        }
        else {
            $(".divPrimerAviso :input").attr("disabled", true);
            //$("#rPrimerAvisoAntes,#rPrimerAvisoDespues").attr("checked", false);
            //$("#txtDiasPrimerAviso,#txtAsuntoPrimerAviso,#txtMensajePrimerAviso").val("");
        }
    },
    habilitarSegundoAviso: function () {
        if ($(".divSegundoAviso :input").is(":disabled")) {
            $(".divSegundoAviso :input").attr("disabled", false);
        }
        else {
            $(".divSegundoAviso :input").attr("disabled", true);
            //$("#rSegundoAvisoAntes,#rSegundoAvisoDespues").attr("checked", false);
            //$("#txtDiasSegundoAviso,#txtAsuntoSegundoAviso,#txtMensajeSegundoAviso").val("");
        }
    },
    habilitarTercerAviso: function () {
        if ($(".divTercerAviso :input").is(":disabled")) {
            $(".divTercerAviso :input").attr("disabled", false);
        }
        else {
            $(".divTercerAviso :input").attr("disabled", true);
            //$("#rTercerAvisoAntes,#rTercerAvisoDespues").attr("checked", false);
            //$("#txtDiasTercerAviso,#txtAsuntoTercerAviso,#txtMensajeTercerAviso").val("");
        }
    },
    habilitarEnvioFE: function () {
        if ($(".divEnvioFE :input").is(":disabled")) {
            $(".divEnvioFE :input").attr("disabled", false);
        }
        else {
            $(".divEnvioFE :input").attr("disabled", true);
            //$("#txtAsuntoEnvioFE,#txtMensajeEnvioFE").val("");
        }
    },
    habilitarEnvioCR: function () {
        if ($(".divEnvioCR :input").is(":disabled")) {
            $(".divEnvioCR :input").attr("disabled", false);
        }
        else {
            $(".divEnvioCR :input").attr("disabled", true);
            //$("#txtAsuntoEnvioCR,#txtMensajeEnvioCR").val("");
        }
    },
    configurarMensaje: function (tipoMensaje) {
        if (tipoMensaje == "divPrimerAviso") {
            $("#modalPrimerAviso").show();
            $("#modalSegundoAviso,#modalTercerAviso,#modalEnvioCR,#modalEnvioFE").hide();
            $("#litModalOkTitulo").html("Configuración: 1er. Aviso");
        }
        else if (tipoMensaje == "divSegundoAviso") {
            $("#modalSegundoAviso").show();
            $("#modalPrimerAviso,#modalTercerAviso,#modalEnvioCR,#modalEnvioFE").hide();
            $("#litModalOkTitulo").html("Configuración: 2er. Aviso");
        }
        else if (tipoMensaje == "divTercerAviso") {
            $("#modalTercerAviso").show();
            $("#modalPrimerAviso,#modalSegundoAviso,#modalEnvioCR,#modalEnvioFE").hide();
            $("#litModalOkTitulo").html("Configuración: 3er. Aviso");
        }
        else if (tipoMensaje == "divEnvioFE") {
            $("#modalEnvioFE").show();
            $("#modalPrimerAviso,#modalSegundoAviso,#modalTercerAviso,#modalEnvioCR").hide();
            $("#litModalOkTitulo").html("Configuración: envío automático comprobantes");
        }
        else if (tipoMensaje == "divEnvioCR") {
            $("#modalEnvioCR").show();
            $("#modalPrimerAviso,#modalSegundoAviso,#modalTercerAviso,#modalEnvioFE").hide();
            $("#litModalOkTitulo").html("Configuración: envío automático de recibos");
        }

        $('#modalAlertasYAvisos').modal('toggle');

    },
    validarAvisosVencimientos: function () {
        if ($("#chkPrimerAviso").is(":checked")) {
            if ($("#txtAsuntoPrimerAviso").val() == "" || $("#txtMensajePrimerAviso").val() == "" || $("#txtDiasPrimerAviso").val() == "0" || $("#txtDiasPrimerAviso").val() == null || $("#txtDiasPrimerAviso").val() == "" || (!$("#rPrimerAvisoAntes").is(":checked") && !$("#rPrimerAvisoDespues").is(":checked"))) {
                $("#msgErrorAlertasyAvisos").html("Debe configurar todos los parámetros del primer aviso.");
                return false;
            }
        }
        if ($("#chkSegundoAviso").is(":checked")) {
            if ($("#txtAsuntoSegundoAviso").val() == "" || $("#txtMensajeSegundoAviso").val() == "" || $("#txtDiasSegundoAviso").val() == "0" || $("#txtDiasSegundoAviso").val() == null || $("#txtDiasSegundoAviso").val() == "" || (!$("#rSegundoAvisoAntes").is(":checked") && !$("#rSegundoAvisoDespues").is(":checked"))) {
                $("#msgErrorAlertasyAvisos").html("Debe configurar todos los parámetros del segundo aviso.");
                return false;
            }
        }
        if ($("#chkTercerAviso").is(":checked")) {
            if ($("#txtAsuntoTercerAviso").val() == "" || $("#txtMensajeTercerAviso").val() == "" || $("#txtDiasTercerAviso").val() == "0" || $("#txtDiasTercerAviso").val() == null || $("#txtDiasTercerAviso").val() == "" || (!$("#rTercerAvisoAntes").is(":checked") && !$("#rTercerAvisoDespues").is(":checked"))) {
                $("#msgErrorAlertasyAvisos").html("Debe configurar todos los parámetros del tercer aviso.");
                return false;
            }
        }
        if ($("#chkEnvioFE").is(":checked")) {
            if ($("#txtAsuntoEnvioFE").val() == "" || $("#txtMensajeEnvioFE").val() == "") {
                $("#msgErrorAlertasyAvisos").html("Debe configurar todos los parámetros para el envío automático de facturas electrónicas.");
                return false;
            }
        }
        if ($("#chkEnvioCR").is(":checked")) {
            if ($("#txtAsuntoEnvioCR").val() == "" || $("#txtMensajeEnvioCR").val() == "") {
                $("#msgErrorAlertasyAvisos").html("Debe configurar todos los parámetros para el envío automático de las cobranzas.");
                return false;
            }
        }
        return true;
    },
    ObtenerAvisosVencimientos: function () {
        $.ajax({
            type: "GET",
            url: "/modulos/seguridad/mis-datos.aspx/ObtenerAvisosVencimientos",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                for (var i = 0; i < data.d.length; i++) {
                    if (data.d[i].TipoAlerta == "Primer aviso") {
                        if (data.d[i].ModoDeEnvio == "1") {
                            $("#rPrimerAvisoAntes").attr("checked", true);
                        }
                        else {
                            $("#rPrimerAvisoDespues").attr("checked", true);
                        }

                        $("#txtDiasPrimerAviso").val(data.d[i].CantDias);
                        $("#txtAsuntoPrimerAviso").val(data.d[i].Asunto);
                        $("#txtMensajePrimerAviso").val(data.d[i].Mensaje);

                        if (data.d[i].Activa) {
                            $("#chkPrimerAviso").attr("checked", true);
                            MisDatos.habilitarPrimerAviso();
                        }
                    }
                    else if (data.d[i].TipoAlerta == "Segundo aviso") {
                        if (data.d[i].ModoDeEnvio == "1") {
                            $("#rSegundoAvisoAntes").attr("checked", true);
                        }
                        else {
                            $("#rSegundoAvisoDespues").attr("checked", true);
                        }

                        $("#txtDiasSegundoAviso").val(data.d[i].CantDias);
                        $("#txtAsuntoSegundoAviso").val(data.d[i].Asunto);
                        $("#txtMensajeSegundoAviso").val(data.d[i].Mensaje);
                        if (data.d[i].Activa) {
                            $("#chkSegundoAviso").attr("checked", true);
                            MisDatos.habilitarSegundoAviso();
                        }
                    }
                    else if (data.d[i].TipoAlerta == "Tercer aviso") {
                        if (data.d[i].ModoDeEnvio == "1") {
                            $("#rTercerAvisoAntes").attr("checked", true);
                        } else {
                            $("#rTercerAvisoDespues").attr("checked", true);
                        }

                        $("#txtDiasTercerAviso").val(data.d[i].CantDias);
                        $("#txtAsuntoTercerAviso").val(data.d[i].Asunto);
                        $("#txtMensajeTercerAviso").val(data.d[i].Mensaje);

                        if (data.d[i].Activa) {
                            $("#chkTercerAviso").attr("checked", true);
                            MisDatos.habilitarTercerAviso();
                        }
                    }
                    else if (data.d[i].TipoAlerta == "Envio FE") {
                        $("#txtAsuntoEnvioFE").val(data.d[i].Asunto);
                        $("#txtMensajeEnvioFE").val(data.d[i].Mensaje);
                        if (data.d[i].Activa) {
                            $("#chkEnvioFE").attr("checked", true);
                            MisDatos.habilitarEnvioFE();
                        }
                    }
                    else if (data.d[i].TipoAlerta == "Envio CR") {
                        $("#txtAsuntoEnvioCR").val(data.d[i].Asunto);
                        $("#txtMensajeEnvioCR").val(data.d[i].Mensaje);
                        if (data.d[i].Activa) {
                            $("#chkEnvioCR").attr("checked", true);
                            MisDatos.habilitarEnvioCR();
                        }
                    }
                    else if (data.d[i].TipoAlerta == "Stock") {
                        $("#chkStock").attr("checked", data.d[i].Activa);
                    }
                }
            },
            error: function (response) {
                var r = jQuery.parseJSON(response.responseText);
                $("#msgError").html(r.Message);
                $("#divError").show();
            }
        });

    },
    cancelarAsuntoYMensaje: function () {
        $('#modalAlertasYAvisos').modal('toggle');
    },
}

var Empresa = {
    configForm: function () {
        // Validation with select boxes
        $("#frmNuevaEmpresa").validate({
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

        $.validator.addMethod("validCuit", function (value, element) {
            return CuitEsValido($("#txtCuitEmpresa").val());
        }, "CUIT Inválido");
        $.validator.addMethod("validPassword", function (value, element) {
            var check = true;
            return CheckPassword($("#txtPwdEmpresa").val());
        }, "La clave de contener: Entre 8 y 12 caracteres, letras y números y al menos una mayúscula.");

        $("#txtCuitEmpresa").numericInput();
        Common.obtenerProvincias("ddlProvinciaEmpresa", "", true);
    },
    consultarDatosAfip: function () {
        Empresa.limpiarControlesEmpresa();
        var cuit = $("#txtCuitEmpresa").val();
        if (cuit != '') {
            if (cuit.length == 11) {
                $.ajax({
                    type: "POST",
                    url: "../../common.aspx/consultarDatosAfip",
                    data: "{ cuit: " + cuit + "}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data, text) {
                        if (data.d != null) {

                            if (data.d.Mensaje == null) {
                                if (data.d.CUIT != null) {
                                    $("#msgOkEmpresa").html('Encontramos datos en AFIP con el CUIT ingresado.');
                                    $("#divOkEmpresa").show();
                                    $("#txtRazonSocialEmpresa").val(data.d.RazonSocial);
                                    $("#ddlCondicionIvaEmpresa").val(data.d.CategoriaImpositiva);
                                    $("#ddlPersoneriaEmpresa").val(data.d.Personeria = 'FISICA' ? 'F' : 'J');
                                    $("#txtDomicilioEmpresa").val(data.d.DomicilioFiscalDomicilio);
                                    $("#ddlProvinciaEmpresa").val(data.d.IdProvincia).trigger("change");
                                    $("#ddlCiudadEmpresa").val(data.d.IdCiudad).trigger("change");
                                    //Common.obtenerProvincias("ddlProvincia", data.d.IdProvincia, true);
                                    //Common.obtenerCiudades("ddlCiudadCliente", data.d.IdCiudad, $("#ddlProvincia").val(), true);
                                    //$("#ddlCiudadCliente").trigger("change");
                                    $("#txtCuitEmpresa").css("background-color", "#e8ffe3");
                                    $("#txtRazonSocialEmpresa").css("background-color", "#e8ffe3");
                                    $("#ddlCondicionIvaEmpresa").css("background-color", "#e8ffe3");
                                    $("#ddlPersoneriaEmpresa").css("background-color", "#e8ffe3");
                                    $("#txtDomicilioEmpresa").css("background-color", "#e8ffe3");
                                } else {
                                    $("#msgErrorEmpresa").html('No encontramos datos en AFIP con el CUIT ingresado.');
                                    $("#divErrorEmpresa").show();
                                }
                            } else {
                                $("#msgErrorEmpresa").html(data.d.Mensaje);
                                $("#divErrorEmpresa").show();
                            }

                        }
                    },
                    error: function (response) {
                        var r = jQuery.parseJSON(response.responseText);
                        $("#msgErrorEmpresa").html(r.Message);
                        $("#divErrorEmpresa").show();
                    }
                });
            } else {
                $("#msgErrorEmpresa").html('CUIT Invalido.');
                $("#divErrorEmpresa").show();
            }
        } else {
            $("#msgErrorEmpresa").html('Debe completar el campo CUIT.');
            $("#divErrorEmpresa").show();
        }

    },
    limpiarControlesEmpresa: function () {
        $("#txtCuitEmpresa").css("background-color", "#ffffff");
        $("#txtRazonSocialEmpresa").css("background-color", "#ffffff");
        $("#ddlCondicionIvaEmpresa").css("background-color", "#ffffff");
        $("#ddlPersoneriaEmpresa").css("background-color", "#ffffff");
        $("#txtDomicilioEmpresa").css("background-color", "#ffffff");
        $("#txtRazonSocialEmpresa").val('');
        $("#ddlCondicionIvaEmpresa").val('');
        $("#ddlPersoneriaEmpresa").val('');
        $("#txtDomicilioEmpresa").val('');
        $("#txtPisoDeptoEmpresa").val('');
        $("#txtPwdEmpresa").val('');        
        $("#txtEmailEmpresa").val('');
        $("#divErrorEmpresa").hide();
        $("#divOkEmpresa").hide();
        Common.obtenerProvincias("ddlProvinciaEmpresa", "1", true);
    },
    grabar: function () {
        if ($('#frmNuevaEmpresa').valid() && $("#ddlCondicionIvaEmpresa").valid() && $("#txtEmailEmpresa").valid() && $("#ddlProvinciaEmpresa").valid() && $("#ddlCiudadEmpresa").valid() && $("#txtDomicilioEmpresa").valid() && $("#txtPwdEmpresa").valid()) {
            $("#divErrorEmpresa").hide();
            $("#divOkEmpresa").hide();
            Common.mostrarProcesando("btnCrearEmpresa");

            var info = "{ razonSocial: '" + $("#txtRazonSocialEmpresa").val()
                    + "', condicionIva: '" + $("#ddlCondicionIvaEmpresa").val()
                    + "', cuit: '" + $("#txtCuitEmpresa").val()
                    + "', personeria: '" + $("#ddlPersoneriaEmpresa").val()
                    + "', email: '" + $("#txtEmailEmpresa").val()
                    + "', pwd: '" + $("#txtPwdEmpresa").val()
                    + "', idProvincia: " + $("#ddlProvinciaEmpresa").val()
                    + " , idCiudad: " + $("#ddlCiudadEmpresa").val()
                    + " , domicilio: '" + $("#txtDomicilioEmpresa").val()
                    + "', pisoDepto: '" + $("#txtPisoDeptoEmpresa").val()
                    + "'}";

            $.ajax({
                type: "POST",
                url: "/modulos/seguridad/mis-datos.aspx/CrearEmpresa",
                data: info,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data, text) {
                    $('#divOk').show();
                    $("#divErrorEmpresa").hide();
                    $('html, body').animate({ scrollTop: 0 }, 'slow');
                    $('#modalNuevaEmpresa').modal('toggle');

                    Empresa.filtrar();
                    Common.ocultarProcesando("btnCrearEmpresa", "Crear empresa");
                },
                error: function (response) {
                    var r = jQuery.parseJSON(response.responseText);
                    $("#msgErrorEmpresa").html(r.Message);
                    $("#divErrorEmpresa").show();
                    $('html, body').animate({ scrollTop: 0 }, 'slow');
                    Common.ocultarProcesando("btnCrearEmpresa", "Crear empresa");
                }
            });

        }
        else {
            return false;
        }
    },
    cambiarSesion: function (id, nombre) {
        Common.cambiarSesion(id, nombre);
    },
    nuevo: function () {
        Empresa.limpiarDatos();
        $('#modalNuevaEmpresa').modal('toggle');
    },
    limpiarDatos: function () {
        $("#divErrorEmpresa").hide();
        $("#txtRazonSocialEmpresa,#ddlCondicionIvaEmpresa,#txtCuitEmpresa,#txtEmailEmpresa,#txtDomicilioEmpresa,#txtPisoDeptoEmpresa,#ddlProvinciaEmpresa,#ddlCiudadEmpresa,#txtPwdEmpresa").val("");
    },
    changeProvincia: function () {
        Common.obtenerCiudades("ddlCiudadEmpresa", $("#ddlCiudadEmpresa").val(), $("#ddlProvinciaEmpresa").val(), true);
    },
    filtrar: function () {
        $("#divError").hide();
        $("#resultsContainerEmpresas").html("");

        $.ajax({
            type: "GET",
            url: "/modulos/seguridad/mis-datos.aspx/getResults",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data, text) {
                if (data.d.TotalPage > 0) {
                }
                else {
                    //$("#divPagination").hide();
                    $("#msjResultados").html("");
                }

                if (parseInt($("#IDUsuarioAdicional").val()) == 0) {
                    $("#btnNuevo").show()
                }
                else {
                    $("#btnNuevo").hide()
                }

                if (data.d.SuperoLimite) {
                    $("#btnNuevo").hide();
                    $("#spMsgCantEmpresas").show();
                }

                $("#IDUsuarioActual").val(data.d.UsuLogiado);
                // Render using the template
                if (data.d.Items.length > 0)
                    $("#resultTemplateEmpresas").tmpl({ results: data.d.Items }).appendTo("#resultsContainerEmpresas");
                else
                    $("#noResultTemplateEmpresas").tmpl({ results: data.d.Items }).appendTo("#resultsContainerEmpresas");
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

var foto = {
    showInputLogo: function () {
        $("#divLogo").slideToggle();
    },
    eliminarLogo: function () {
        $.ajax({
            type: "GET",
            url: "mis-datos.aspx/eliminarFoto",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: false,
            success: function (data, text) {
                $('#divOk').show();
                $("#divError").hide();
                $('html, body').animate({ scrollTop: 0 }, 'slow');
                $("#imgFoto").attr("src", "/files/usuarios/no-photo.png");
                $("#hdnTieneFoto").val("0");
                foto.showBtnEliminar();
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
    showBtnEliminar: function () {
        if ($("#hdnTieneFoto").val() == "1") {
            $("#divEliminarFoto").show();
            $("#divAdjuntarFoto").removeClass("col-sm-12").addClass("col-sm-6");
        }
        else {
            $("#imgLogo").attr("src", "/files/usuarios/no-photo.png");
            $("#divEliminarFoto").hide();
            $("#divAdjuntarFoto").removeClass("col-sm-6").addClass("col-sm-12");
        }
    },
}