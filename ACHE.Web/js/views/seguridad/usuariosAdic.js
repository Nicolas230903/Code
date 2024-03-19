var UsuariosAdic = {
    /*** FORM ***/
    ocultarMensajes: function () {
        $("#divError, #divOk").hide();
    },
    grabar: function () {
        UsuariosAdic.ocultarMensajes();

        var idEmpresas="";
        $("input:checkbox:checked").each(function () {
            //cada elemento seleccionado
            idEmpresas += "#-#" +$(this).attr("id");
        });
       
        var id = ($("#hdnID").val() == "" ? "0" : $("#hdnID").val());
        if ($('#frmEdicion').valid()) {
            Common.mostrarProcesando("btnActualizar");
            var info = "{ id: " + parseInt(id)
                    + ", email: '" + $("#txtEmail").val()
                    + "', tipo: '" + $("#ddlTipo").val()
                    + "', pwd: '" + $("#txtPwd").val()
                    + "', activo: " + $("#chkActivo").is(':checked')
                    + ", idEmpresas: '" + idEmpresas + "'"
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
                    + ", EsVendedor: " + $("#chkEsVendedor").is(':checked')
                    + ", PorcentajeComision: '" + $("#txtPorcentajeComision").val()
                    + "'}";

            $.ajax({
                type: "POST",
                url: "/modulos/seguridad/usuariose.aspx/guardar",
                data: info,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data, text) {
                    $('#divOk').show();
                    $("#divError").hide();
                    $('html, body').animate({ scrollTop: 0 }, 'slow');
                    window.location.href = "/modulos/seguridad/usuarios.aspx";
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
        window.location.href = "/modulos/seguridad/usuarios.aspx";
    },
    configForm: function () {
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

        $("#txtPorcentajeComision").maskMoney({ thousands: '', decimal: ',', allowZero: true });


        UsuariosAdic.changeTipo();
        $.validator.addMethod("validPassword", function (value, element) {
            var check = true;
            return CheckPassword($("#txtPwd").val());
        }, "La clave de contener: Entre 8 y 12 caracteres, letras y números y al menos una mayúscula.");
    },
    changeTipo: function(){
        if ($("#ddlTipo").val() == "A") {
            $("#divChkAccesoModulos").hide();
        }
        else {
            $("#divChkAccesoModulos").show();
        }
    },

    /*** SEARCH ***/
    configFilters: function ()
    {
        $("#txtEmail").keypress(function (event) {
            var keycode = (event.keyCode ? event.keyCode : event.which);
            if (keycode == '13') {
                UsuariosAdic.resetearPagina();
                UsuariosAdic.filtrar();
                return false;
            }
        });
    },
    nuevo: function () {
        window.location.href = "/modulos/seguridad/usuariose.aspx";
    },
    editar: function (id) {
        window.location.href = "/modulos/seguridad/usuariose.aspx?ID=" + id;
    },
    eliminar: function (id, email) {
        bootbox.confirm("¿Está seguro que desea eliminar al usuario " + email + "?", function (result) {
            if (result) {
                $.ajax({
                    type: "POST",
                    url: "/modulos/seguridad/usuarios.aspx/delete",
                    data: "{ id: " + id + "}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data, text) {
                        UsuariosAdic.filtrar();
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
        var currentPage = parseInt($("#hdnPage").val());

        var info = "{ email: '" + $("#txtEmail").val()
                   + "', page: " + currentPage + ", pageSize: " + PAGE_SIZE
                   + "}";

        $.ajax({
            type: "POST",
            url: "/modulos/seguridad/usuarios.aspx/getResults",
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
    },
    verTodos: function () {
        $("#txtEmail").val("");
        UsuariosAdic.filtrar();
    },
    mostrarPagAnterior: function () {
        var paginaActual = parseInt($("#hdnPage").val());
        paginaActual--;
        $("#hdnPage").val(paginaActual);
        UsuariosAdic.filtrar();
    },
    mostrarPagProxima: function () {
        var paginaActual = parseInt($("#hdnPage").val());
        paginaActual++;
        $("#hdnPage").val(paginaActual);
        UsuariosAdic.filtrar();
    },
    resetearPagina: function () {
        $("#hdnPage").val("1");
    }    
}