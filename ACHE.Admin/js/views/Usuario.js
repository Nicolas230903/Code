var usuario = {
    resetearPwd: function (id, nombre) {
        bootbox.confirm("¿Está seguro que desea resetear la contraseña y enviarla por email a " + nombre + "?", function (result) {
            if (result) {
                $.ajax({
                    type: "POST",
                    url: "/Usuario/ResetearPwd",
                    data: "{ id: " + id + "}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data) {
                        if (data == true) {
                            alert("El pwd fue renviado.");
                        }
                        else {
                            alert("El pwd no pudo ser renviado.");
                        }
                    }
                });
            }
        });
    },
    grabar: async function (id) {

        if ($('#frmEdicion').valid()) {

            var file64;
            var fileName;
            if ($("#flpArchivoCertificado").val() != "") {
                var archivo = document.querySelector('#flpArchivoCertificado').files[0];
                fileName = archivo.name;
                file64 = await toBase64(archivo);
            }                        

            Common.mostrarProcesando("btnActualizarInfo");
            Common.mostrarProcesando("btnActualizarDatosInternos");
            $("#divError").hide();
            var info = "{ id: " + id
                    + ", condicionIva: '" + $("#ddlCondicionIva").val()
                    + "', celular: '" + $("#txtCelular").val()
                    + "', email: '" + $("#txtEmail").val()
                    + "', observaciones: '" + $("#txtObservaciones").val()
                    + "', factElectronica: " + $("#chkFactElectronica").is(':checked')
                    + ", usaProd: " + $("#chkUsaProd").is(':checked')
                    + ", cantEmpresasHabilitadas: " + $("#txtCantEmpresasHabilitadas").val()
                    + ", activo: " + $("#ddlActivo").val()
                    + ", motivoBaja: '" + $("#txtMotivoBaja").val()
                    + "', accesoGestor: " + $("#ddlAccesoGestor").val()
                    + ", estaBloqueado: " + $("#ddlBloqueado").val()
                    + ", CUITAfip: '" + $("#txtCUITAfip").val()
                    + "', flpNombreArchivoCertificado: '" + fileName
                    + "', flpArchivoCertificado: '" + file64
                    + "', esVendedor: " + $("#ddlEsVendedor").val()
                    + ", porcentajeComision: '" + $("#txtPorcentajeComision").val()
                    + "' }";

            $.ajax({
                type: "POST",
                url: "/Usuario/Guardar",
                data: info,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    Common.ocultarProcesando("btnActualizarInfo", "Actualizar");
                    Common.ocultarProcesando("btnActualizarDatosInternos", "Actualizar");

                    $('html, body').animate({ scrollTop: 0 }, 'slow');
                    if (data) {                        
                        $("#divOk").show();
                        setTimeout('document.location.reload()', 2000);
                    }
                    else {
                        $("#divError").show();
                        $("#msgError").html("no se guardaron los cambios");
                    }
                }
            });

        }
        else {
            return false;
        }
    },    
    grabarAccesos: async function (id) {

        if ($('#frmEdicion').valid()) {

            Common.mostrarProcesando("btnActualizarAccesos");
            $("#divError").hide();
            var info = "{ id: " + id
                + ", HomeValores: " + $("#chkHomeValores").is(':checked')
                + ", ComercialPedidoDeVenta: " + $("#chkComercialPedidoDeVenta").is(':checked')
                + ", ComercialFacturaDeVenta: " + $("#chkComercialFacturaDeVenta").is(':checked')
                + ", ComercialCobranzas: " + $("#chkComercialCobranzas").is(':checked')
                + ", ComercialPresupuestos: " + $("#chkComercialPresupuestos").is(':checked')
                + ", ComercialEntregas: " + $("#chkComercialEntregas").is(':checked')
                + ", ComercialAbonos: " + $("#chkComercialAbonos").is(':checked')
                + ", ComercialProductosYServicios: " + $("#chkComercialProductosYServicios").is(':checked')
                + ", ComercialClientes: " + $("#chkComercialClientes").is(':checked')
                + ", SuministroPedidoDeCompra: " + $("#chkSuministroPedidoDeCompra").is(':checked')
                + ", SuministroComprobanteDeCompra: " + $("#chkSuministroComprobanteDeCompra").is(':checked')
                + ", SuministroPagos: " + $("#chkSuministroPagos").is(':checked')
                + ", SuministroProveedores: " + $("#chkSuministroProveedores").is(':checked')
                + ", AdministracionBancos: " + $("#chkAdministracionBancos").is(':checked')
                + ", AdministracionInstituciones: " + $("#chkAdministracionInstituciones").is(':checked')
                + ", AdministracionGastos: " + $("#chkAdministracionGastos").is(':checked')
                + ", AdministracionMovimientos: " + $("#chkAdministracionMovimientos").is(':checked')
                + ", AdministracionDetalleBancario: " + $("#chkAdministracionDetalleBancario").is(':checked')
                + ", AdministracionCheques: " + $("#chkAdministracionCheques").is(':checked')
                + ", AdministracionCaja: " + $("#chkAdministracionCaja").is(':checked')
                + ", AdministracionCuentasCorrientes: " + $("#chkAdministracionCuentasCorrientes").is(':checked')
                + ", ProduccionMateriales: " + $("#chkProduccionMateriales").is(':checked')
                + ", ProduccionAlmacenes: " + $("#chkProduccionAlmacenes").is(':checked')
                + ", ProduccionCostos: " + $("#chkProduccionCostos").is(':checked')
                + ", ProduccionRecursos: " + $("#chkProduccionRecursos").is(':checked')
                + ", ProduccionPlanificacion: " + $("#chkProduccionPlanificacion").is(':checked')
                + ", PlaneamientoObjetivos: " + $("#chkPlaneamientoObjetivos").is(':checked')
                + ", PlaneamientoProgramas: " + $("#chkPlaneamientoProgramas").is(':checked')
                + ", PlaneamientoPresupuestos: " + $("#chkPlaneamientoPresupuestos").is(':checked')
                + ", InfoFinancierosVentasVsCompras: " + $("#chkInfoFinancierosVentasVsCompras").is(':checked')
                + ", InfoFinancierosComprasPorCategoria: " + $("#chkInfoFinancierosComprasPorCategoria").is(':checked')
                + ", InfoGananciasVsPerdidasCobradoVsPagado: " + $("#chkInfoGananciasVsPerdidasCobradoVsPagado").is(':checked')
                + ", InfoImpositivosIVAVentas: " + $("#chkInfoImpositivosIVAVentas").is(':checked')
                + ", InfoImpositivosIVACompras: " + $("#chkInfoImpositivosIVACompras").is(':checked')
                + ", InfoImpositivosIVASaldo: " + $("#chkInfoImpositivosIVASaldo").is(':checked')
                + ", InfoImpositivosRetenciones: " + $("#chkInfoImpositivosRetenciones").is(':checked')
                + ", InfoImpositivosPercepciones: " + $("#chkInfoImpositivosPercepciones").is(':checked')
                + ", InfoImpositivosCITIComprasYVentas: " + $("#chkInfoImpositivosCITIComprasYVentas").is(':checked')
                + ", InfoGestionCuentaCorriente: " + $("#chkInfoGestionCuentaCorriente").is(':checked')
                + ", InfoGestionCobranzaPendientes: " + $("#chkInfoGestionCobranzaPendientes").is(':checked')
                + ", InfoGestionPagoAProveedores: " + $("#chkInfoGestionPagoAProveedores").is(':checked')
                + ", InfoGestionStockProductos: " + $("#chkInfoGestionStockProductos").is(':checked')
                + ", InfoGestionStockProductosDetalle: " + $("#chkInfoGestionStockProductosDetalle").is(':checked')
                + ", InfoGestionCuentasAPagar: " + $("#chkInfoGestionCuentasAPagar").is(':checked')
                + ", InfoGestionRankingPorCliente: " + $("#chkInfoGestionRankingPorCliente").is(':checked')
                + ", InfoGestionRankingPorProductoServicio: " + $("#chkInfoGestionRankingPorProductoServicio").is(':checked')
                + ", InfoGestionTrackingHoras: " + $("#chkInfoGestionTrackingHoras").is(':checked')
                + ", InfoGestionListaFacturas: " + $("#chkInfoGestionListaFacturas").is(':checked')
                + ", InfoGestionComisiones: " + $("#chkInfoGestionComisiones").is(':checked')
                + ", HerramientasExploradorDeArchivos: " + $("#chkHerramientasExploradorDeArchivos").is(':checked')
                + ", HerramientasImportacionMasiva: " + $("#chkHerramientasImportacionMasiva").is(':checked')
                + ", HerramientasTrackingDeHoras: " + $("#chkHerramientasTrackingDeHoras").is(':checked')
                + ", HerramientasConfigurarAlertas: " + $("#chkHerramientasConfigurarAlertas").is(':checked')
                + ", HerramientasGeneracionCompraAutomatica: " + $("#chkHerramientasGeneracionCompraAutomatica").is(':checked')
                + ", HerramientasGeneracionLiquidoProducto: " + $("#chkHerramientasGeneracionLiquidoProducto").is(':checked')
                + ", HerramientasGeneracionFacturaAutomatica: " + $("#chkHerramientasGeneracionFacturaAutomatica").is(':checked')
                + ", HerramientasAuditoria: " + $("#chkHerramientasAuditoria").is(':checked')
                + ", HabilitarCambioIvaEnArticulosDesdeComprobante: " + $("#chkHabilitarCambioIvaEnArticulosDesdeComprobante").is(':checked')           
                + " }";

            $.ajax({
                type: "POST",
                url: "/Usuario/GuardarAccesos",
                data: info,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    Common.ocultarProcesando("btnActualizarAccesos", "Actualizar");

                    if (data) {
                        $('html, body').animate({ scrollTop: 0 }, 'slow');
                        $("#divOk").show();
                    }
                    else {
                        $("#divError").show();
                        $("#msgError").html("no se guardaron los cambios");
                    }
                }
            });

        }
        else {
            return false;
        }
    },
    config: function () {
        $("#txtCantEmpresasHabilitadas").numericInput();
        $(".select2").select2({ width: '100%', allowClear: true });
        //$("#lbPwd").mouseenter(function (evento) {
        //    $("#txtPwd").attr("type", "text");
        //});

        //$("#lbPwd").mouseleave(function (e) {
        //    $("#txtPwd").attr("type", "password");
        //});

        Common.configTelefono("txtTelefono");
    },
    CheckPassword: function (inputtxt) {
        var passw = /^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{8,12}$/;
        if (inputtxt.match) {
            if (inputtxt.length > 0) {
                if (inputtxt.match(passw)) {
                    return true;
                }
                else {
                    return false;
                }
            } else {
                return true;
            }
        } else {
            return true;
        }    
    },
    fijarNuevaClave: function (id) {
        $("#divErrorFijarNuevaClave").hide();
        $("#divOkFijarNuevaClave").hide();

        if (!usuario.CheckPassword($("#txtPwd").val())) {
            $("#msgErrorFijarNuevaClave").html("La clave de contener: Entre 8 y 12 caracteres, letras y números y al menos una mayúscula.");
            $("#divErrorFijarNuevaClave").show();
            $('html, body').animate({ scrollTop: 0 }, 'slow');
            return false;
        }

        if (!usuario.CheckPassword($("#txtPwd2").val())) {
            $("#msgErrorFijarNuevaClave").html("La clave de contener: Entre 8 y 12 caracteres, letras y números y al menos una mayúscula.");
            $("#divErrorFijarNuevaClave").show();
            $('html, body').animate({ scrollTop: 0 }, 'slow');
            return false;
        }

        if ($("#txtPwd").val() != $("#txtPwd2").val()) {
            $("#msgErrorFijarNuevaClave").html("Las contraseñas nuevas no coinciden");
            $("#divErrorFijarNuevaClave").show();
            $('html, body').animate({ scrollTop: 0 }, 'slow');
            return false;
        }
        else {

            var info = "{ id: " + id + ", nueva: '" + $("#txtPwd").val() + "'}";

            $.ajax({
                type: "POST",
                url: "/Usuario/FijarNuevaClave",
                data: info,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    $("#divOkFijarNuevaClave").show();
                },
                error: function (response) {
                    var r = jQuery.parseJSON(response.responseText);
                    $("#msgErrorFijarNuevaClave").html(r.Message);
                    $("#divErrorFijarNuevaClave").show();
                }
            });
           
        }
    },
    habilitarFCEmpresas: function (id, nombre, tipo) {
        var accion = "";
        if (tipo == true)
            accion = "habilitar";
        else
            accion = "inabilitar";

        bootbox.confirm("¿Está seguro que desea <b> " + accion + "</b> la FCE y usaProd a la empresa: " + nombre + "?", function (result) {
            if (result) {

                var info = "{ id: " + id + ", tipo:" + tipo + " }";
                $.ajax({
                    type: "POST",
                    url: "/Usuario/HabilitarFCEmpresas",
                    data: info,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data) {
                        location.reload();
                    }
                });
            }
        });
    },
    guardarMenu: function (id) {
        $("#divErrorMenu").hide();
        var home = $("#txtHome").val();
        var comercialAcumuladoDeVentas = $("#txtComercialAcumuladoDeVentas").val();
        var comercialClientes = $("#txtComercialClientes").val();
        var comercialOportunidades = $("#txtComercialOportunidades").val();
        var comercialProspecto = $("#txtComercialProspecto").val();
        var info = "{ id: " + id
            + ", home: '" + home
            + "', comercialAcumuladoDeVentas: '" + comercialAcumuladoDeVentas
            + "', comercialClientes: '" + comercialClientes
            + "', comercialOportunidades: '" + comercialOportunidades
            + "', comercialProspecto: '" + comercialProspecto + "' }";
        $.ajax({
            type: "POST",
            url: "/Usuario/guardarMenu",
            data: info,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                $("#divOkMenu").show();
            },
            error: function (response) {
                var r = jQuery.parseJSON(response.responseText);
                $("#msgErrorMenu").html(r.Message);
                $("#divErrorMenu").show();
            }
        });
    },
    eliminarUsuario: function (id) {
        $("#divOkEliminarUsuario").hide();
        $("#divErrorEliminarUsuario").hide();
        var opcion = confirm("Eliminar Usuario?");
        if (opcion == true) {

            var info = "{ id: " + id + "}";

            $.ajax({
                type: "POST",
                url: "/Usuario/eliminarUsuario",
                contentType: "application/json; charset=utf-8",
                data: info,
                dataType: "json",
                success: function (data, text) {
                    $("#msgOkEliminarUsuario").html('Usuario eliminado correctamente!');
                    $("#divOkEliminarUsuario").show();
                    setTimeout('window.location.replace("/usuario");', 3000);
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
            url: "/Usuario/HabilitarPuntoVenta",
            data: info,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                location.reload();
            }
        });
    },
    aceptarPago: function (id, idUsuario, nombre) {
        bootbox.confirm("¿Está seguro que desea <b> Aceptar </b> el pago de la empresa: " + nombre + "?", function (result) {
            if (result) {

                var info = "{ id: " + id + " ,idUsuario:" + idUsuario + "}";
                $.ajax({
                    type: "POST",
                    url: "/Usuario/AceptarPago",
                    data: info,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data) {

                        if (data.TieneError) {
                            alert(data.Mensaje)
                        }
                        else {
                            alert("Comprobante electronico y cobranza fueron generados correctamente.")
                            location.reload();
                        }

                    }
                });
            }
        });
    },
    /*** SEARCH ***/
    editar: function (id) {
        window.location.href = "/Usuario/Edicion/" + id;
        //window.open("/Usuario/Edicion/" + id);
    },
    configFilters: function () {

        $(".select2").select2({ width: '100%', allowClear: true });

        $("#txtCondicion").keypress(function (event) {
            var keycode = (event.keyCode ? event.keyCode : event.which);
            if (keycode == '13') {
                usuario.resetearPagina();
                usuario.filtrar();
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
        usuario.filtrar();
    },
    mostrarPagProxima: function () {
        var paginaActual = parseInt($("#hdnPage").val());
        paginaActual++;
        $("#hdnPage").val(paginaActual);
        usuario.filtrar();
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
                   + "', fechaDesde: '" + $("#txtFechaDesde").val()
                   + "', fechaHasta: '" + $("#txtFechaHasta").val()
                   + "', tipoPlan: '" + $("#ddlTipoPlan").val()
                   + "', EstadoUsuario: '" + $("#ddlEstado").val()
                   + "' , page: " + currentPage + ", pageSize: " + PAGE_SIZE
                   + "}";

        $.ajax({
            type: "POST",
            url: "/Usuario/ObtenerUsuarios",
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
        usuario.resetearExportacion();
    },
    exportar: function () {
        usuario.resetearExportacion();
        $("#imgLoading").show();
        $("#divIconoDescargar").hide();
        var info = "{ condicion: '" + $("#txtCondicion").val()
                    + "', periodo: '" + $("#ddlPeriodo").val()
                    + "', fechaDesde: '" + $("#txtFechaDesde").val()
                    + "', fechaHasta: '" + $("#txtFechaHasta").val()
                    + "', tipoPlan: '" + $("#ddlTipoPlan").val()
                    + "', EstadoUsuario: '" + $("#ddlEstado").val()
                    + "'}";

        $.ajax({
            type: "POST",
            url: "/Usuario/export",
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
            usuario.resetearPagina();
            usuario.filtrar();
        }
    },
    importaciones: function (id) {
        window.location.href = "/Importaciones/Index/" + id;
    },
    configurarPlanCorporativo: function (id) {
        $("#divError,#divOk").hide();
        var info = "{ idUsuario: " + id + " }";
        $.ajax({
            type: "POST",
            url: "/Usuario/ConfigurarPlanCorporativo",
            data: info,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                if (data.TieneError) {
                    $("#divError").show();
                    $("#msgError").html(data.Mensaje);
                    usuario.showBtnPlanCorporativo("False");
                }
                else {
                    $("#divOk").show();
                    usuario.showBtnPlanCorporativo("True");
                }
            }
        });
    },    
    showBtnPlanCorporativo: function (tienePlanDeCuentas) {
        if (tienePlanDeCuentas == "True") {
            $("#divbtnPlanCorporativo").hide();
            $("#divTextPlanCorporativo").show();
        }
        else {
            $("#divbtnPlanCorporativo").show();
            $("#divTextPlanCorporativo").hide();
        }
    }
}
const toBase64 = file => new Promise((resolve, reject) => {
    const reader = new FileReader();
    reader.readAsDataURL(file);
    reader.onload = () => resolve(reader.result);
    reader.onerror = error => reject(error);
});