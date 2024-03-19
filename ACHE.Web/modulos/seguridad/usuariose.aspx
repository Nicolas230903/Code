<%@ Page Title="" Language="C#" MasterPageFile="~/Front.master" AutoEventWireup="true" CodeFile="usuariose.aspx.cs" Inherits="usuariose" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="pageheader">
        <h2><i class="fa fa-users"></i>Usuarios </h2>
        <div class="breadcrumb-wrapper">
            <span class="label">Estás aquí:</span>
            <ol class="breadcrumb">
                <li><a href="/home.aspx">axanweb</a></li>
                <li><a href="/modulos/seguridad/mis-datos.aspx">Mi cuenta</a></li>
                <li><a href="/modulos/seguridad/usuarios.aspx">Usuarios</a></li>
                <li class="active">
                    <asp:Literal runat="server" ID="litPath"></asp:Literal></li>
            </ol>
        </div>
    </div>

    <div class="contentpanel">

        <div class="row mb15">
            <form id="frmEdicion" runat="server" class="col-sm-12" autocomplete="off">
                <div class="panel panel-default">
                    <div class="panel-body">
                        <div class="alert alert-danger" id="divError" style="display: none">
                            <strong>Lo sentimos! </strong><span id="msgError"></span>
                        </div>

                        <div class="alert alert-success" id="divOk" style="display: none">
                            <strong>Bien hecho! </strong>Los datos se han actualizado correctamente
                        </div>

                        <div class="row mb15">
                            <div class="col-sm-3">
                                <div class="form-group">
                                    <label class="control-label"><span class="asterisk">*</span> Email</label>
                                    <div class="input-group">
                                        <span class="input-group-addon">@</span>
                                        <asp:TextBox runat="server" ID="txtEmail" CssClass="form-control required email" MaxLength="128" autocomplete="off" AutoCompleteType="Search" TextMode="SingleLine"></asp:TextBox>
                                        <%--<input runat="server" id="txtEmail" class="form-control" maxlength="128" autocomplete="off" type="search"/>--%>
                                    </div>
                                </div>
                            </div>
                            <div class="col-sm-3">
                                <div class="form-group">
                                    <asp:Literal ID="litPwd" runat="server"></asp:Literal>
                                    <asp:TextBox runat="server" ID="txtPwd" TextMode="Password" CssClass="form-control required validPassword" MaxLength="20" MinLength="8" autocomplete="off"></asp:TextBox>
                                </div>
                            </div>
                        </div>

                        <div class="row mb15">
                            <div class="col-sm-3" hidden="hidden">
                                <div class="form-group">
                                    <label class="control-label"><span class="asterisk">*</span> Nivel de seguridad</label>
                                    <asp:DropDownList runat="server" ID="ddlTipo" CssClass="form-control" onchange="UsuariosAdic.changeTipo();">
                                        <asp:ListItem Value="A">Acceso total</asp:ListItem>
                                        <asp:ListItem Value="B">Acceso restringido</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="col-sm-1">
                                <div class="form-group">
                                    <label class="control-label">Activo</label><br />
                                    <asp:CheckBox runat="server" ID="chkActivo"></asp:CheckBox>
                                </div>
                            </div>

                            <div class="col-sm-3">
                                <div class="form-group">
                                    <asp:Label ID="lblNombre" runat="server"></asp:Label>
                                    <asp:Panel ID="panelChk" runat="server"></asp:Panel>
                                </div>
                            </div>
                        </div>

                        <div class="row mb15">
                            <div class="col-sm-6" id="divChkAccesoModulos">
                                <div class="form-group">
                                    <label class="control-label">Acceso a los siguientes módulos:</label><br />
                                    <asp:Panel ID="pnlChkAccesoModulos" runat="server"></asp:Panel>
                                </div>
                            </div>
                        </div>
                        <div class="row mb15">
                            <div class="col-sm-12">
                                <hr />
                                <h3>Accesos</h3>
                                <label class="control-label">Gestión de los accesos a los items del sistema</label>
                            </div>
                        </div>
                        <hr />
                        <h4>Home</h4>
                        <div class="row mb15">
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="control-label">Home Valores</label>
                                    <asp:CheckBox runat="server" ID="chkHomeValores" Checked="true"></asp:CheckBox>
                                </div>
                            </div>
                        </div>
                        <hr />
                        <h4>Comercial</h4>
                        <div class="row mb15">
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="control-label">Comercial - Pedido De Venta</label>
                                    <asp:CheckBox runat="server" ID="chkComercialPedidoDeVenta" Checked="true"></asp:CheckBox>                                                    
                                </div>
                            </div>
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="control-label">Comercial - Factura De Venta</label>
                                    <asp:CheckBox runat="server" ID="chkComercialFacturaDeVenta" Checked="true"></asp:CheckBox>
                                                    
                                </div>
                            </div>
                        </div>
                        <div class="row mb15">
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="control-label">Comercial - Cobranzas</label>
                                    <asp:CheckBox runat="server" ID="chkComercialCobranzas" Checked="true"></asp:CheckBox>                                                    
                                </div>
                            </div>
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="control-label">Comercial - Presupuestos</label>
                                    <asp:CheckBox runat="server" ID="chkComercialPresupuestos" Checked="true"></asp:CheckBox>                                                    
                                </div>
                            </div>
                        </div>
                        <div class="row mb15">
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="control-label">Comercial - Entregas</label>
                                    <asp:CheckBox runat="server" ID="chkComercialEntregas" Checked="true"></asp:CheckBox>                                                    
                                </div>
                            </div>
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="control-label">Comercial - Abonos</label>
                                    <asp:CheckBox runat="server" ID="chkComercialAbonos" Checked="true"></asp:CheckBox>                                                    
                                </div>
                            </div>
                        </div>
                        <div class="row mb15">
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="control-label">Comercial - Productos y Servicios</label>
                                    <asp:CheckBox runat="server" ID="chkComercialProductosYServicios" Checked="true"></asp:CheckBox>                                                    
                                </div>
                            </div>
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="control-label">Comercial - Clientes</label>
                                    <asp:CheckBox runat="server" ID="chkComercialClientes" Checked="true"></asp:CheckBox>                                                    
                                </div>
                            </div>
                        </div>
                        <hr />
                        <h4>Suministro</h4>
                        <div class="row mb15">
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="control-label">Suministro - Pedido De Compra</label>
                                    <asp:CheckBox runat="server" ID="chkSuministroPedidoDeCompra" Checked="true"></asp:CheckBox>
                                                    
                                </div>
                            </div>
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="control-label">Suministro - Comprobante De Compra</label>
                                    <asp:CheckBox runat="server" ID="chkSuministroComprobanteDeCompra" Checked="true"></asp:CheckBox>
                                                    
                                </div>
                            </div>
                        </div>
                        <div class="row mb15">
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="control-label">Suministro - Pagos</label>
                                    <asp:CheckBox runat="server" ID="chkSuministroPagos" Checked="true"></asp:CheckBox>
                                                    
                                </div>
                            </div>
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="control-label">Suministro - Proveedores</label>
                                    <asp:CheckBox runat="server" ID="chkSuministroProveedores" Checked="true"></asp:CheckBox>
                                                    
                                </div>
                            </div>
                        </div>
                        <hr />
                        <h4>Administración</h4>
                        <div class="row mb15">
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="control-label">Administracion - Bancos</label>
                                    <asp:CheckBox runat="server" ID="chkAdministracionBancos" Checked="true"></asp:CheckBox>
                                                    
                                </div>
                            </div>
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="control-label">Administracion - Instituciones</label>
                                    <asp:CheckBox runat="server" ID="chkAdministracionInstituciones" Checked="true"></asp:CheckBox>
                                                    
                                </div>
                            </div>
                        </div>
                        <div class="row mb15">
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="control-label">Administracion - Gastos</label>
                                    <asp:CheckBox runat="server" ID="chkAdministracionGastos" Checked="true"></asp:CheckBox>
                                                    
                                </div>
                            </div>
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="control-label">Administracion - Movimientos</label>
                                    <asp:CheckBox runat="server" ID="chkAdministracionMovimientos" Checked="true"></asp:CheckBox>
                                                    
                                </div>
                            </div>
                        </div>
                        <div class="row mb15">
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="control-label">Administracion - Detalle Bancario</label>
                                    <asp:CheckBox runat="server" ID="chkAdministracionDetalleBancario" Checked="true"></asp:CheckBox>
                                                    
                                </div>
                            </div>
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="control-label">Administracion - Cheques</label>
                                    <asp:CheckBox runat="server" ID="chkAdministracionCheques" Checked="true"></asp:CheckBox>
                                                    
                                </div>
                            </div>
                        </div>
                        <div class="row mb15">
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="control-label">Administracion - Caja</label>
                                    <asp:CheckBox runat="server" ID="chkAdministracionCaja" Checked="true"></asp:CheckBox>
                                                    
                                </div>
                            </div>
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="control-label">Administracion - Cuentas Corrientes</label>
                                    <asp:CheckBox runat="server" ID="chkAdministracionCuentasCorrientes" Checked="true"></asp:CheckBox>
                                                    
                                </div>
                            </div>
                        </div>
                        <hr />
                        <h4>Producción</h4>
                        <div class="row mb15">
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="control-label">Producción - Materiales</label>
                                    <asp:CheckBox runat="server" ID="chkProduccionMateriales" Checked="true"></asp:CheckBox>
                                                    
                                </div>
                            </div>
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="control-label">Producción - Almacenes</label>
                                    <asp:CheckBox runat="server" ID="chkProduccionAlmacenes" Checked="true"></asp:CheckBox>
                                                    
                                </div>
                            </div>
                        </div>
                        <div class="row mb15">
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="control-label">Producción - Costos</label>
                                    <asp:CheckBox runat="server" ID="chkProduccionCostos" Checked="true"></asp:CheckBox>
                                                    
                                </div>
                            </div>
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="control-label">Producción - Recursos</label>
                                    <asp:CheckBox runat="server" ID="chkProduccionRecursos" Checked="true"></asp:CheckBox>
                                                    
                                </div>
                            </div>
                        </div>
                        <div class="row mb15">
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="control-label">Producción - Planificacion</label>
                                    <asp:CheckBox runat="server" ID="chkProduccionPlanificacion" Checked="true"></asp:CheckBox>
                                                    
                                </div>
                            </div>
                        </div>
                        <hr />
                        <h4>Planeamiento</h4>
                        <div class="row mb15">
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="control-label">Planeamiento - Objetivos</label>
                                    <asp:CheckBox runat="server" ID="chkPlaneamientoObjetivos" Checked="true"></asp:CheckBox>
                                                    
                                </div>
                            </div>
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="control-label">Planeamiento - Programas</label>
                                    <asp:CheckBox runat="server" ID="chkPlaneamientoProgramas" Checked="true"></asp:CheckBox>
                                                    
                                </div>
                            </div>
                        </div>
                        <div class="row mb15">
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="control-label">Planeamiento - Presupuestos</label>
                                    <asp:CheckBox runat="server" ID="chkPlaneamientoPresupuestos" Checked="true"></asp:CheckBox>
                                                    
                                </div>
                            </div>
                        </div>
                        <h4>Info</h4>
                        <div class="row mb15">
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="control-label">Info - Financieros - Ventas Vs Compras</label>
                                    <asp:CheckBox runat="server" ID="chkInfoFinancierosVentasVsCompras" Checked="true"></asp:CheckBox>
                                                    
                                </div>
                            </div>
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="control-label">Info - Financieros - Compras Por Categoria</label>
                                    <asp:CheckBox runat="server" ID="chkInfoFinancierosComprasPorCategoria" Checked="true"></asp:CheckBox>
                                                    
                                </div>
                            </div>
                        </div>
                        <div class="row mb15">
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="control-label">Info - Ganancias Vs Perdidas - Cobrado Vs Pagado</label>
                                    <asp:CheckBox runat="server" ID="chkInfoGananciasVsPerdidasCobradoVsPagado" Checked="true"></asp:CheckBox>
                                                    
                                </div>
                            </div>
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="control-label">Info - Impositivos - IVA Ventas</label>
                                    <asp:CheckBox runat="server" ID="chkInfoImpositivosIVAVentas" Checked="true"></asp:CheckBox>
                                                    
                                </div>
                            </div>
                        </div>
                        <div class="row mb15">
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="control-label">Info - Impositivos - IVA Compras</label>
                                    <asp:CheckBox runat="server" ID="chkInfoImpositivosIVACompras" Checked="true"></asp:CheckBox>
                                                    
                                </div>
                            </div>
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="control-label">Info - Impositivos - IVA Saldo</label>
                                    <asp:CheckBox runat="server" ID="chkInfoImpositivosIVASaldo" Checked="true"></asp:CheckBox>
                                                    
                                </div>
                            </div>
                        </div>
                        <div class="row mb15">
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="control-label">Info - Impositivos - Retenciones</label>
                                    <asp:CheckBox runat="server" ID="chkInfoImpositivosRetenciones" Checked="true"></asp:CheckBox>
                                                    
                                </div>
                            </div>
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="control-label">Info - Impositivos - Percepciones</label>
                                    <asp:CheckBox runat="server" ID="chkInfoImpositivosPercepciones" Checked="true"></asp:CheckBox>
                                                    
                                </div>
                            </div>
                        </div>
                        <div class="row mb15">
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="control-label">Info - Impositivos - CITI Compras Y Ventas</label>
                                    <asp:CheckBox runat="server" ID="chkInfoImpositivosCITIComprasYVentas" Checked="true"></asp:CheckBox>
                                                    
                                </div>
                            </div>
                        </div>
                        <div class="row mb15">
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="control-label">Info - Gestion - Cuenta Corriente</label>
                                    <asp:CheckBox runat="server" ID="chkInfoGestionCuentaCorriente" Checked="true"></asp:CheckBox>
                                                    
                                </div>
                            </div>
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="control-label">Info - Gestion - Cobranza Pendientes</label>
                                    <asp:CheckBox runat="server" ID="chkInfoGestionCobranzaPendientes" Checked="true"></asp:CheckBox>
                                                    
                                </div>
                            </div>
                        </div>
                        <div class="row mb15">
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="control-label">Info - Gestion - Pago A Proveedores</label>
                                    <asp:CheckBox runat="server" ID="chkInfoGestionPagoAProveedores" Checked="true"></asp:CheckBox>
                                                    
                                </div>
                            </div>
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="control-label">Info - Gestion - Stock Productos</label>
                                    <asp:CheckBox runat="server" ID="chkInfoGestionStockProductos" Checked="true"></asp:CheckBox>
                                </div>
                            </div>
                        </div>
                        <div class="row mb15">
                                                        <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="control-label">Info - Gestion - Movimientos de Stock</label>
                                    <asp:CheckBox runat="server" ID="chkInfoGestionStockProductosDetalle" Checked="true"></asp:CheckBox>
                                </div>
                            </div>
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="control-label">Info - Gestion - Cuentas A Pagar</label>
                                    <asp:CheckBox runat="server" ID="chkInfoGestionCuentasAPagar" Checked="true"></asp:CheckBox>
                                                    
                                </div>
                            </div>

                        </div>
                        <div class="row mb15">
                                                        <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="control-label">Info - Gestion - Ranking Por Cliente</label>
                                    <asp:CheckBox runat="server" ID="chkInfoGestionRankingPorCliente" Checked="true"></asp:CheckBox>
                                                    
                                </div>
                            </div>
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="control-label">Info - Gestion - Ranking Por Producto Servicio</label>
                                    <asp:CheckBox runat="server" ID="chkInfoGestionRankingPorProductoServicio" Checked="true"></asp:CheckBox>
                                                    
                                </div>
                            </div>

                        </div>
                        <div class="row mb15">
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="control-label">Info - Gestion - Tracking Horas</label>
                                    <asp:CheckBox runat="server" ID="chkInfoGestionTrackingHoras" Checked="true"></asp:CheckBox>
                                                    
                                </div>
                            </div>
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="control-label">Info - Gestion - Lista Facturas</label>
                                    <asp:CheckBox runat="server" ID="chkInfoGestionListaFacturas" Checked="true"></asp:CheckBox>
                                                    
                                </div>
                            </div>
                        </div>
                        <div class="row mb15">
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="control-label">Info - Gestion - Comisiones</label>
                                    <asp:CheckBox runat="server" ID="chkInfoGestionComisiones" Checked="true"></asp:CheckBox>                                                    
                                </div>
                            </div>
                        </div>
                        <hr />
                        <h4>Herramientas</h4>
                        <div class="row mb15">
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="control-label">Herramientas - Explorador De Archivos</label>
                                    <asp:CheckBox runat="server" ID="chkHerramientasExploradorDeArchivos" Checked="true"></asp:CheckBox>
                                                    
                                </div>
                            </div>
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="control-label">Herramientas - Importacion Masiva</label>
                                    <asp:CheckBox runat="server" ID="chkHerramientasImportacionMasiva" Checked="true"></asp:CheckBox>
                                                    
                                </div>
                            </div>
                        </div>
                        <div class="row mb15">
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="control-label">Herramientas - Tracking De Horas</label>
                                    <asp:CheckBox runat="server" ID="chkHerramientasTrackingDeHoras" Checked="true"></asp:CheckBox>
                                </div>
                            </div>
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="control-label">Herramientas - Configurar Alertas</label>
                                    <asp:CheckBox runat="server" ID="chkHerramientasConfigurarAlertas" Checked="true"></asp:CheckBox>
                                </div>
                            </div>
                        </div>
                        <div class="row mb15">
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="control-label">Herramientas - Generación Compra Automatica</label>
                                    <asp:CheckBox runat="server" ID="chkHerramientasGeneracionCompraAutomatica" Checked="true"></asp:CheckBox>
                                </div>
                            </div>
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="control-label">Herramientas - Generación Líquido Producto</label>
                                    <asp:CheckBox runat="server" ID="chkHerramientasGeneracionLiquidoProducto" Checked="true"></asp:CheckBox>
                                </div>
                            </div>
                        </div>
                        <div class="row mb15">
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="control-label">Herramientas - Generación Factura Automatica</label>
                                    <asp:CheckBox runat="server" ID="chkHerramientasGeneracionFacturaAutomatica" Checked="true"></asp:CheckBox>
                                </div>
                            </div>
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="control-label">Herramientas - Auditoria</label>
                                    <asp:CheckBox runat="server" ID="chkHerramientasAuditoria" Checked="true"></asp:CheckBox>
                                </div>
                            </div>
                        </div>
                        <hr />
                        <h4>Mas Opciones</h4>
                        <div class="row mb15">
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="control-label">Habilitar cambio IVA en articulos desde comprobante</label>
                                    <asp:CheckBox runat="server" ID="chkHabilitarCambioIvaEnArticulosDesdeComprobante" Checked="true"></asp:CheckBox>                                                    
                                </div>
                            </div>
                        </div>
                        <hr />
                        <div class="row mb15">
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="control-label">Comisiones - Es Vendedor</label>
                                    <asp:CheckBox runat="server" ID="chkEsVendedor" Checked="false"></asp:CheckBox>
                                </div>
                            </div>
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="control-label">Comisiones - Porcentaje Comisión</label>
                                    <asp:TextBox runat="server" ID="txtPorcentajeComision" CssClass="form-control" MaxLength="15"></asp:TextBox>
                                </div>
                            </div>
                        </div>   
                    </div>

                    <div class="panel-footer">
                        <a class="btn btn-success" id="btnActualizar" onclick="UsuariosAdic.grabar();">Aceptar</a>
                        <a href="#" onclick="UsuariosAdic.cancelar();" style="margin-left: 20px">Cancelar</a>
                    </div>
                </div>

                <asp:HiddenField runat="server" ID="hdnID" Value="0" />
            </form>
        </div>
    </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FooterContent" runat="Server">
    <script src="/js/jquery.validate.min.js"></script>
    <script src="/js/jquery.maskMoney.min.js"></script>
    <script src="/js/views/seguridad/usuariosAdic.js?v=<%= ConfigurationManager.AppSettings["JS.Version"] %>"></script>

    <script>
        jQuery(document).ready(function () {
            UsuariosAdic.configForm();
        });
    </script>

</asp:Content>
