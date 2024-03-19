<%@ Control Language="C#" AutoEventWireup="true" CodeFile="header.ascx.cs" Inherits="controls_header" %>

<div class="headerbar">
    <div class="header-left">
        <div class="helper-links">

            <asp:Literal ID="litSinMultiEmpresa" runat="server"></asp:Literal>
            <div class="btn-group" id="divUsuariosMultiempresas" runat="server">
                <a href="#" data-toggle="dropdown">Empresa Actual:
                    <asp:Literal ID="litEmpresaActual" runat="server"></asp:Literal></a>
                <div class="dropdown-menu dropdown-menu-head pull-right ayuda">
                    <h5 class="title">Empresas a las que puede acceder</h5>
                    <ul class="dropdown-list gen-list">
                        <asp:Literal ID="liEmpresasHeader" runat="server"></asp:Literal>
                    </ul>
                </div>
            </div>
            <div class="btn-group pull-left" id="divDatos" style="font-size: 12px;text-decoration: none; padding: 3px 5px 1px; color: #777;" runat="server"></div>
            <div class="btn-group" id="divAyuda" runat="server">                
                <a href="#" data-toggle="dropdown">Ayuda</a>
                <div class="dropdown-menu dropdown-menu-head pull-right ayuda">
                    <h5 class="title">Centro de Ayuda</h5>
                    <ul class="dropdown-list gen-list">                        
                        <li>
                            <a href="/ayuda/faq.aspx">
                                <span class="desc" style="margin-left: 0px">
                                    <span class="name">Preguntas frecuentes</span>
                                    <span class="msg">Despejá todas tus dudas. ¿Qué estás esperando?
                                    </span>
                                </span>
                            </a>
                        </li>
                        <li>
                            <a href="/ayuda/tutoriales.aspx">
                                <span class="desc" style="margin-left: 0px">
                                    <span class="name">Tutoriales</span>
                                    <span class="msg">Accedé a un listado de videos donde podrás aprender cómo usar las funcionalidades del sistema.
                                    </span>
                                </span>
                            </a>
                        </li>
                        <li>
                            <a href="#" data-toggle="modal" data-target="#modalAyuda">
                                <span class="desc" style="margin-left: 0px">
                                    <span class="name">Ayuda</span>
                                    <span class="msg">Despejá todas tus dudas. Mandandonos un mensaje
                                    </span>
                                </span>
                            </a>
                        </li>
                    </ul>
                </div>
            </div>
            <div class="btn-group" id="divMensajes" runat="server">
                <a href="#" data-toggle="dropdown">
                    <asp:Literal runat="server" ID="LitCant"></asp:Literal>
                </a>
                <div class="dropdown-menu dropdown-menu-head pull-right avisos">
                    <span class="badge">
                        <asp:Literal runat="server" ID="litMsjCant"></asp:Literal>
                    </span>
                </div>
                <div id="btnMensaje" runat="server" class="dropdown-menu dropdown-menu-head pull-right">
                    <h5 class="title">
                        <asp:Literal runat="server" ID="litMsjTitulo"></asp:Literal></h5>
                    <ul class="dropdown-list gen-list">
                        <li class="new" runat="server" id="liFE" visible="true">
                            <a href="#" data-toggle="modal" data-target="#modalAyuda">
                                <span class="desc" style="margin-left: 0px">
                                    <span class="name">Facturación electrónica <span class="badge badge-success">contactanos</span></span>
                                    <span class="msg">Aún no configuraste tu cuenta para poder emitir comprobantes de forma electrónica.<br />
                                        ¿Qué estás esperando?
                                    </span>
                                </span>
                            </a>
                        </li>
                        <li class="new" runat="server" id="liComunicacionesAFIP" visible="true">
                            <a href="/comunicacionAfip.aspx" >
                                <span class="desc" style="margin-left: 0px">
                                    <span class="name">Comunicaciones AFIP <span class="badge badge-success">ver</span></span>
                                    <span class="msg">Comunicaciones pendientes de leer: <asp:Literal runat="server" ID="litCantMsjCmAfip"></asp:Literal><br />
                                    <span class="msg">Comunicaciones Leídas: <asp:Literal runat="server" ID="litCantMsjCmAfipLeidas"></asp:Literal><br />
                                    </span>
                                </span>
                            </a>
                        </li>
                        <li class="new" runat="server" id="liComunicacionesAFIPContador" visible="true">
                            <a href="/Home.aspx" >
                                <span class="desc" style="margin-left: 0px">
                                    <span class="name">Comunicaciones del Contador <span class="badge badge-success">ver</span></span>
                                    <span class="msg">Comunicaciones pendientes de leer: <asp:Literal runat="server" ID="litCantMsjCmAfipContador" Text="0"></asp:Literal><br />
                                    <span class="msg">Comunicaciones Leídas: <asp:Literal runat="server" ID="litCantMsjCmAfipLeidasContador" Text="0"></asp:Literal><br />
                                    </span>
                                </span>
                            </a>
                        </li>
                        <li class="new" runat="server" id="liConsultaComprobantesAFIP" visible="true">
                            <a href="/consultaComprobantesAfip.aspx" >
                                <span class="desc" style="margin-left: 0px">
                                    <span class="name">Consulta Comprobantes AFIP <span class="badge badge-success">ver</span></span>
                                    <span class="msg">Ingresar para consultar el estado de los servicios de AFIP<br />
                                    </span>
                                </span>
                            </a>
                        </li>
                        <li class="new" runat="server" id="liPuntosDeVenta">
                            <a href="/modulos/seguridad/mis-datos.aspx?pdv=active">
                                <span class="desc" style="margin-left: 0px">
                                    <span class="name">Puntos de venta <span class="badge badge-success">configurar</span></span>
                                    <span class="msg">Aún no configuraste tus puntos de venta.<br />
                                        ¿Qué estás esperando?
                                    </span>
                                </span>
                            </a>
                        </li>

                        <asp:Literal runat="server" ID="liPagarPlan"></asp:Literal>

                        <%--<li>
                            <a href="/alertas.aspx" data-toggle="modal">
                                <span class="desc" style="margin-left: 0px">
                                    <span class="name">Administrar alertas</span>
                                    <span class="msg">Administra y modifica las todas tus alertas existentes.
                                    </span>
                                </span>
                            </a>
                        </li>--%>
                        <asp:Literal runat="server" ID="litAlertas"></asp:Literal>
                    </ul>
                </div>
            </div>
            <a href="/modulos/seguridad/mis-datos.aspx">Mi cuenta</a>
            <a href="/login.aspx?logOut=true">Salir</a>
        </div>
        <%-- Andy 20191121 Cambio de las etiquetas de la barra de navegacion --%>
        <div class="topnav navbar yamm navbar-default">
            <nav>
                <div class="container-fluid">
                    <div class="navbar-header">
                        <button type="button" class="navbar-toggle" data-toggle="collapse" data-target="#setupFinalizado">
                            <span class="sr-only">Toggle navigation</span>
                            <span class="icon-bar"></span>
                            <span class="icon-bar"></span>
                            <span class="icon-bar"></span>
                        </button>
                        <a href="/home.aspx" class="navbar-brand">
                            <img id="imgLogoMenu" src="/images/logo-insideAxan.png" alt="" runat="server" />
                        </a>
                    </div>
                    <div class="collapse navbar-collapse" id="setupFinalizado" runat="server">
                        <ul class="nav navbar-nav">

                             <asp:Literal runat="server" ID="liHome">
                                <li>
                                    <a href="/home.aspx" title="Home"><img src="/images/icoHomeEdit.png" title="Home" /></a>
                                </li>
                            </asp:Literal>

                            <asp:Literal runat="server" ID="liVentas">
                                <li class="nav-parent">
                                    <a href="#">Comercial <span class="caret"></span></a>
                                    <ul class="dropdown-menu children">
                                        <li><a href="/comprobantes.aspx?tipo=PDV"><span>Pedido de Venta</span></a></li>
                                        <li><a href="/comprobantes.aspx?tipo=FAC"><span>Factura de Venta</span></a></li>
                                        <li><a href="/cobranzas.aspx"><span>Cobranzas</span></a></li>
                                        <li><a href="/presupuestos.aspx"><span>Presupuestos</span></a></li>
                                        <li><a href="/comprobantes.aspx?tipo=EDA"><span>Entregas</span></a></li>
                                        <li><a href="/Abonos.aspx"><span>Abonos</span></a></li>
                                        <li><a href="/conceptos.aspx"><span>Productos y servicios</span></a>
                                        <li><a href="/personas.aspx?tipo=c"><span>Clientes</span></a></li>
                                    </ul>
                                </li>
                            </asp:Literal>

                            <asp:Literal runat="server" ID="liCompras">
                                <li class="nav-parent" >
                                    <a href="#">Suministro <span class="caret"></span></a>
                                    <ul class="dropdown-menu children">
                                        <li><a href="/comprobantes.aspx?tipo=PDC"><span>Pedido de Compra</span></a></li>
                                        <li><a href="/compras.aspx"><span>Comprobante de Compra</span></a></li>
<%--                                    <li><a href="/compras.aspx"><span>Comprobantes</span></a></li>--%>
                                        <li><a href="/pagos.aspx"><span>Pagos</span></a></li>                                    
                                        <li><a href="/personas.aspx?tipo=p"><span>Proveedores</span></a></li>
                                    </ul>
                                </li>
                            </asp:Literal>

                            <asp:Literal runat="server" ID="liTesoreria">
                                <li class="nav-parent">
                                    <a href="#">Administración <span class="caret"></span></a>
                                    <ul class="dropdown-menu children">
                                        <li><a href="/modulos/tesoreria/bancos.aspx"><span>Bancos</span></a></li>
                                        <li><a href="/modulos/tesoreria/bancos.aspx"><span>+ Instituciones</span></a></li>
                                        <li><a href="/modulos/tesoreria/gastosBancarios.aspx"><span>+ Gastos</span></a></li>
                                        <li><a href="/modulos/tesoreria/MovimientoDeFondos.aspx"><span>+ Movimientos</span></a></li>
                                        <li><a href="/modulos/reportes/DetalleBancario.aspx">+ Detalle bancario</a></li>
                                        <li><a href="/modulos/tesoreria/cheques.aspx"><span>Cheques</span></a></li>
                                        <li><a href="/modulos/tesoreria/caja.aspx"><span>Caja</span></a></li>
                                        <li><a href="/cuentasCorrientes.aspx?tipo=C">Cuentas Corrientes</a></li>
                                        <li><a href="/gastosGenerales.aspx">Gastos Generales</a></li>
                                    </ul>
                                </li>
                            </asp:Literal>

                            <asp:Literal runat="server" ID="liProduccion">
                                <li class="nav-parent">
                                    <a href="#">Producción <span class="caret"></span></a>
                                    <ul class="dropdown-menu children">
                                        <li><a href="/modulos/seguridad/elegir-plan.aspx" ><span>Materiales</span></a></li>
                                        <li><a href="/modulos/seguridad/elegir-plan.aspx"><span>Almacenes</span></a></li>
                                        <li><a href="/modulos/seguridad/elegir-plan.aspx"><span>Costos</span></a></li>
                                        <li><a href="/modulos/seguridad/elegir-plan.aspx"><span>Recursos</span></a></li>
                                        <li><a href="/modulos/seguridad/elegir-plan.aspx"><span>Planificación (MRP)</span></a></li>
                                    </ul>
                                </li>
                            </asp:Literal>

                            <asp:Literal runat="server" ID="liPlaneamiento">
                                <li class="nav-parent">
                                    <a href="#">Planeamiento <span class="caret"></span></a>
                                    <ul class="dropdown-menu children">
                                        <li><a href="/modulos/seguridad/elegir-plan.aspx" ><span>Objetivos</span></a></li>
                                        <li><a href="/modulos/seguridad/elegir-plan.aspx"><span>Programas</span></a></li>
                                        <li><a href="/modulos/seguridad/elegir-plan.aspx"><span>Presupuestos</span></a></li>
                                    </ul>
                                </li>
                            </asp:Literal>

                            <asp:Literal runat="server" ID="liReportes">
                                <li class="nav-parent">
                                    <a href="#"><img src="/images/icoInfoEdit.png"  title="Información" /><span class="caret"></span></a>
                                    <%--<ul class="dropdown-menu children" style="min-width: 200px;">--%>
                                        <ul class="dropdown-menu children pull-right" style="min-width: 200px;">
                                        <div class="container">
                                            <div class="row">
                                                <li class="nav-parent">
                                                    <div class="col-md-7 col-sm-12 col-lg-3">
                                                        <span style="padding: 6px 10px; font-size: 13px; color: #2b9b8f">Financieros</span>
                                                        <ul>
                                                            <li><a href="/modulos/reportes/ventas-vs-compras.aspx">Ventas vs Compras</a></li>
                                                            <li><a href="/modulos/reportes/compras-por-categoria.aspx">Compras por categoría</a></li>
                                                        </ul>
                                                    </div>
                                                </li>
                                                <li class="nav-parent">
                                                    <div class="col-md-7 col-sm-12 col-lg-3">
                                                        <span style="padding: 6px 10px; font-size: 13px; color: #2b9b8f">Ganancias vs Perdidas</span>
                                                        <ul class="">
                                                            <li><a href="/modulos/reportes/cobrado-vs-pagado.aspx">Cobrado vs Pagado</a></li>
                                                        </ul>
                                                    </div>
                                                </li>
                                                <div class="clearfix visible-md-block"></div>
                                                <li class="nav-parent">
                                                    <div class="col-md-7 col-sm-12 col-lg-3">
                                                        <span style="padding: 6px 10px; font-size: 13px; color: #2b9b8f">Impositivos</span>
                                                        <ul class="">
                                                            <li><a href="/modulos/reportes/iva-ventas.aspx">IVA Ventas</a></li>
                                                            <li><a href="/modulos/reportes/iva-compras.aspx">IVA Compras</a></li>
                                                            <li><a href="/modulos/reportes/iva-saldo.aspx">IVA Saldo</a></li>
                                                            <li><a href="/modulos/reportes/retenciones.aspx">Retenciones</a></li>
                                                            <li><a href="/modulos/reportes/percepciones.aspx">Percepciones</a></li>
                                                            <li><a href="/modulos/reportes/citiVentas.aspx">C.I.T.I: compras y ventas</a></li>
                                                        </ul>
                                                    </div>
                                                </li>
                                                <li class="nav-parent">
                                                    <div class="col-md-7 col-sm-12 col-lg-3">
                                                        <span style="padding: 6px 10px; font-size: 13px; color: #2b9b8f">Gestión</span>
                                                        <ul class="">
                                                            <li><a href="/modulos/reportes/cc.aspx">Cuenta corriente</a></li>
                                                            <li><a href="/modulos/reportes/cobranzasPendientes.aspx">Cobranzas pendientes</a></li>
                                                            <li><%--<a href="/modulos/reportes/saldos-cc.aspx">Saldos de cuenta corriente</a>--%></li>
                                                            <li><a href="/modulos/reportes/pagoprov.aspx">Pago a proveedores</a></li>
                                                            <li><a href="/modulos/reportes/stock.aspx">Stock productos</a></li>
                                                            <li><a href="/modulos/reportes/stock-detalle.aspx">Movimientos de Stock</a></li>
                                                            <li><a href="/modulos/reportes/cuentasPagar.aspx">Cuentas a pagar</a></li>
                                                            <li><a href="/modulos/reportes/rnk-clientes.aspx">Ranking por cliente</a></li>
                                                            <li><a href="/modulos/reportes/rnk-conceptos.aspx">Ranking por producto/servicio</a></li>
                                                            <li><a href="/modulos/reportes/trackingHora.aspx">Tracking horas</a></li>
                                                            <li><a href="/modulos/reportes/lista-facturas.aspx">Lista masiva de facturas</a></li>
                                                            <li><a href="/modulos/reportes/comisiones.aspx">Comisiones</a></li>
                                                        </ul>
                                                    </div>
                                                </li>
                                            </div>
                                        </div>
                                    </ul>
                                </li>
                            </asp:Literal>

                        </ul>

                        <div class="header-right" id="divHerramientas" runat="server">
                            <ul class="nav navbar-nav navbar-right headermenu">
                                 <li id="liHerramientas" runat="server">
                                        <div class="btn-group">
                                            <button type="button" class="btn btn-default dropdown-toggle cog-icon-dropdown tooltips handle-dropdown-manually" data-placement="bottom" data-original-title="Herramientas">
                                            <i class="glyphicon glyphicon-cog"></i>
                                            <span class="caret"></span>
                                        </button>                                       
                                        <ul class="dropdown-menu dropdown-menu-usermenu pull-right">
                                            <li><a href="/file-explorer.aspx"><span>Explorador de archivos</span></a></li>
                                            <li>
                                                <a href="#" class="toggle-sub">Importación masiva <span class="caret"></span></a>
                                                <ul class="submenu">
                                                    <li><a href="/importar.aspx?tipo=Clientes">Importación masiva de clientes</a></li>
                                                    <li><a href="/importar.aspx?tipo=Proveedores">Importación masiva de proveedores</a></li>
                                                    <li><a href="/importar.aspx?tipo=Productos">Importación masiva de productos</a></li>
                                                    <li><a href="/importar.aspx?tipo=Servicios">Importación masiva de servicios</a></li>
                                                        <li><a href="/importar.aspx?tipo=Facturas">Importación masiva de facturas</a></li>
                                                    <li runat="server" id="liImportarPlanDeCuentas"><a href="/importar.aspx?tipo=PlanDeCuentas">Importación masiva de cuentas contables</a></li>
                                                </ul>
                                            </li>
                                            <li><a href="/modulos/ventas/trackingHoras.aspx">Tracking de horas</a></li>
                                            <li><a href="/alertas.aspx">Configurar alertas</a></li>
                                            <li><a href="/compraAutomatica.aspx">Generación compra automática</a></li>
                                            <li><a href="/liquidoProducto.aspx">Generación Liquido Producto</a></li>
                                            <li><a href="/facturaAutomatica.aspx">Monitor de Facturación</a></li>
                                            <li><a href="/auditoria.aspx">Auditoria</a></li>
                                        </ul>
                                    </div>
                                </li>
                                <li class="search-input">
                                    <div class="input-group">
                                        <%--<input type="text" id="autocompleteHeader" class="form-control" placeholder="Buscar ..." />--%>
                                        <span role="status" aria-live="polite" class="ui-helper-hidden-accessible"></span><input type="text" id="autocompleteHeader" class="form-control ui-autocomplete-input" placeholder="Buscar ..." autocomplete="off">
                                        <span class="input-group-btn">
                                            <button class="btn btn-default" type="button">
                                                <i class="glyphicon glyphicon-search"></i>
                                            </button>
                                        </span>
                                    </div>
                                </li>
                            </ul>
                        </div>
                    </div>
                </div>
            </nav>
        </div>
    </div>
</div>

