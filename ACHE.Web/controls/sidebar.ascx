<%@ Control Language="C#" AutoEventWireup="true" CodeFile="sidebar.ascx.cs" Inherits="controls_sidebar" %>

<div class="leftpanel">

    <div class="logopanel">
        <h1><span>[</span> hoyfacturo <span>]</span></h1>
    </div>
    <!-- logopanel -->

    <div class="leftpanelinner">

        <!-- This is only visible to small devices -->
        <div class="visible-xs hidden-sm hidden-md hidden-lg">
            <div class="media userlogged">
                <img alt="" src="images/photos/loggeduser.png" class="media-object">
                <div class="media-body">
                    <h4><asp:Literal runat="server" ID="litUsuario"></asp:Literal></h4>
                    <%--<span>"Life is so..."</span>--%>
                </div>
            </div>

            <h5 class="sidebartitle actitle">Mi cuenta</h5>
            <ul class="nav nav-pills nav-stacked nav-bracket mb30">
                <li><a href="/modulos/seguridad/mis-datos.aspx"><i class="fa fa-user"></i><span>Mis datos</span></a></li>
                <%--<li><a href="#"><i class="fa fa-cog"></i><span>Account Settings</span></a></li>--%>
                <li><a href="#"><i class="fa fa-question-circle"></i><span>Ayuda</span></a></li>
                <li><a href="/login.aspx?logOut=true"><i class="fa fa-sign-out"></i><span>Cerrar sesión</span></a></li>
            </ul>
        </div>

        <h5 class="sidebartitle">Menú</h5>
        <ul class="nav nav-pills nav-stacked nav-bracket">
            <li><a href="/home.aspx"><i class="fa fa-home"></i><span>Home</span></a></li>
            <li><a href="/comprobantes.aspx"><i class="fa fa-file-text"></i><span>Comprobantes</span></a>
            <li><a href="/pagos.aspx"><i class="fa fa-edit"></i><span>Pagos</span></a>
            <li><a href="/personas.aspx?tipo=c"><i class="fa fa-suitcase"></i><span>Clientes</span></a>
            <li><a href="/personas.aspx?tipo=p"><i class="fa fa-users"></i><span>Proveedores</span></a>
            <li><a href="/productos.aspx"><i class="fa fa-shopping-cart"></i><span>Productos</span></a>
            <li class="nav-parent"><a href="#"><i class="fa fa-bar-chart-o"></i><span>Reportes</span></a>
                <ul class="children">
                    <li><a href="/rpt-cc.aspx"><i class="fa fa-caret-right"></i>Cuenta corriente</a></li>
                    <li><a href="/rpt-pagoprov.aspx"><i class="fa fa-caret-right"></i>Pago a proveedores</a></li>
                    <li><a href="/rpt-iva-ventas.aspx"><i class="fa fa-caret-right"></i>IVA Ventas</a></li>
                    <li><a href="/rpt-iva-compras.aspx"><i class="fa fa-caret-right"></i>IVA Compras</a></li>
                    <li><a href="/rpt-iva-saldo.aspx"><i class="fa fa-caret-right"></i>IVA Saldo</a></li>
                </ul>
            </li>
            <li class="nav-parent"><a href="#"><i class="fa fa-cloud-upload"></i><span>Importación masiva</span></a>
                <ul class="children">
                    <li><a href="/importar.aspx?tipo=pe"><i class="fa fa-caret-right"></i>Clientes/Proveedores</a></li>
                    <li><a href="/importar.aspx?tipo=pr"><i class="fa fa-caret-right"></i>Productos</a></li>
                    <li><a href="/importar.aspx?tipo=ab"><i class="fa fa-caret-right"></i>Abonos</a></li>
                </ul>
            </li>
        </ul>

        <%--<div class="infosummary">
            <h5 class="sidebartitle">Information Summary</h5>
            <ul>
                <li>
                    <div class="datainfo">
                        <span class="text-muted">Daily Traffic</span>
                        <h4>630, 201</h4>
                    </div>
                    <div id="sidebar-chart" class="chart"></div>
                </li>
                <li>
                    <div class="datainfo">
                        <span class="text-muted">Average Users</span>
                        <h4>1, 332, 801</h4>
                    </div>
                    <div id="sidebar-chart2" class="chart"></div>
                </li>
                <li>
                    <div class="datainfo">
                        <span class="text-muted">Disk Usage</span>
                        <h4>82.2%</h4>
                    </div>
                    <div id="sidebar-chart3" class="chart"></div>
                </li>
                <li>
                    <div class="datainfo">
                        <span class="text-muted">CPU Usage</span>
                        <h4>140.05 - 32</h4>
                    </div>
                    <div id="sidebar-chart4" class="chart"></div>
                </li>
                <li>
                    <div class="datainfo">
                        <span class="text-muted">Memory Usage</span>
                        <h4>32.2%</h4>
                    </div>
                    <div id="sidebar-chart5" class="chart"></div>
                </li>
            </ul>
        </div>--%>
        <!-- infosummary -->
    </div>
    <!-- leftpanelinner -->
</div>
<!-- leftpanel -->