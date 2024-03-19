<%@ Page Title="" Language="C#" MasterPageFile="~/Front.master" AutoEventWireup="true" CodeFile="home.aspx.cs" Inherits="home" %>

<%@ Register Assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" Namespace="System.Web.UI.DataVisualization.Charting" TagPrefix="asp" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
    <style type="text/css">
        .row-centered {
            text-align: center;
        }

        .col-centered {
            display: inline-block;
            float: none;
            /* reset the text-align */
            text-align: left;
            /* inline-block space fix */
            margin-right: -4px;
        }

        .panel-stat h1 {
            font-size: 30px !important;
        }

        .panel-stat .stat {
            max-width: 500px;
        }

        .accesos-cc {
            font-size: 17px;
            font-family: "Roboto",sans-serif;
            line-height: normal;
            margin: 0px;
            color: rgb(99, 110, 123);
        }

        @media (max-width: 500px) {
            .homeBloqueIconos {
                padding-bottom: 25px;
            }

            .homeIconosDer {
                margin-left: 30px;
            }
        }

        @media (min-width: 500px) and (max-width: 1450px) {
            .homeBloqueIconos {
                padding-left: 0px;
                padding-right: 0px;
            }

            .homeIconosIzq {
                margin-right: initial;
            }

            .homeIconosDer {
                margin-left: 15px;
            }
        }

        @media (min-width: 1451px) {
            .homeIconosIzq {
                margin-right: 30px;
            }
        }
    </style>
    <link href="css/morris.css" rel="stylesheet" />
    <link href="css/DashboardValores.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">

    <div class="contentpanel">
        <%--<div class="row mb15">
	        <div class="alert alert-warning fade in nomargin">
		        El dia 29/10 a las <b>18:00 hrs</b> se realizará una actualización en el sitio y el mismo no estará disponible 15 minutos.
	        </div>
	    </div>--%>
        <div class="row mb15" id="divMensajeFCE" runat="server">
            <div class="alert alert-info fade in nomargin">
                <button aria-hidden="true" data-dismiss="alert" class="close" type="button">×</button>
                <h4>Hemos detectado que aún no tiene habilitado en axanweb la facturación electrónica</h4>

                <p>Le recordamos que se debe habilitar la factura electronica, para lo cual debe tener habilitado el punto de venta y su CUIT en AFIP</p>

                <%--                <p>Los pasos a seguir son:</p>
                <br />
                <p>
                    Paso 1: Ingresar a la AFIP con su número de CUIT y clave fiscal<br />
                    Paso 2: Bajo la opción <b>Servicios Administrativos Clave Fiscal</b>, ingresar en <b>Administrador de Relaciones de Clave Fiscal</b><br />
                    Paso 3: Ingresar en <b>Nueva relación</b><br />
                    Paso 4: En servicio, cliquear en <b>Buscar</b><br />
                    Paso 5: Bajo la categoría <b>AFIP</b>, elegir <b>Webservices</b> y luego <b>Facturación Electrónica</b><br />
                    Paso 6: El siguiente paso es, en la opción <b>Representante</b>, presionar <b>Buscar</b> e ingrese nuestro CUIT: 30714864269<br />
                    Paso 7: Y haga click en finalizar<br />
                    Paso 1: Habilitar su punto de venta electrónico. Puede descargar los pasos a seguir desde <a href="/ayuda/manuales/Guia-paso-a-paso-para-configurar-nuevo-punto-de-venta-AFIP.pdf" download>aquí</a><br />
                    Paso 2: Habilitar su CUIT en AFIP. Puede descargar los pasos a seguir desde <a href="/ayuda/manuales/Guia-paso-a-paso-para-configurar-FE-en-axanweb.pdf" download>aquí</a>
                </p>--%>

                <p>
                    Este procedimiento habilita a axanweb a poder comunicarse con AFIP para que usted pueda emitir Facturas Electrónicas.
                    <br />
                    Una vez realizado este procedimiento, por favor envie un mail a <b>atencioncliente@axanweb.com </b>ya que debemos Aceptar la designación.
                </p>
            </div>
        </div>

        <div class="row mb15" id="divPagarPlan" runat="server">
            <div class="alert alert-info fade in nomargin">
                <button aria-hidden="true" data-dismiss="alert" class="close" type="button">×</button>
                <h4>Su plan actual es
                    <asp:Literal ID="linombrePlan" runat="server"></asp:Literal>
                    , y vence en
                    <asp:Literal ID="liCantDias" runat="server"></asp:Literal>
                    <a href="#" onclick="dashBoard.pagarPlanActual()">Pagar/Renovar</a></h4>
            </div>
            <input id="hdnNombrePlanActual" runat="server" type="hidden" />
        </div>

        <div class="row mb15" id="divPlanPendiente" runat="server">
            <div class="alert alert-info fade in nomargin">
                <button aria-hidden="true" data-dismiss="alert" class="close" type="button">×</button>
                <h4>Su plan actual es
                    <asp:Literal ID="linombrePlanPendiente" runat="server"></asp:Literal>
                    y se encuentra pendiente de pago </h4>
            </div>
        </div>

        <div runat="server" id="PanelDeControl">
            <div class="row">
                <div class="col-sm-12 col-md-12 col-lg-12">
                    <div class="panel panel-default">
                        <div class="panel-body">
                            <div class="row">

                                <div class="col-sm-6 col-md-6 col-lg-6 " style="padding-top: 15px; padding-bottom: 15px">
                                    <div class="col-sm-6 col-md-4 col-lg-4 homeBloqueIconos" style="padding-bottom: 25px">
                                        <div style="text-align: center">
                                            <a href="/comprobantese.aspx?tipo=PDV" class="accesos-cc homeIconosIzq">
                                                <img src="/images/icoVender.png" height="90" title="VENDER" />
                                            </a>
                                            <a href="/personas.aspx?tipo=c" class="accesos-cc homeIconosDer">
                                                <img src="/images/icoCliente.png" height="90" title="CLIENTE" />
                                            </a>
                                        </div>
                                    </div>
                                    <div class="col-sm-6 col-md-4 col-lg-4 homeBloqueIconos" style="padding-bottom: 25px">
                                        <div style="text-align: center">
                                            <a href="/cobranzase.aspx" class="accesos-cc homeIconosIzq">
                                                <img src="/images/icoCobrar.png" height="90" title="COBRAR" />
                                            </a>
                                            <a href="/personas.aspx?tipo=p" class="accesos-cc homeIconosDer">
                                                <img src="/images/icoProveedor.png" height="90" title="PROVEEDOR" />
                                            </a>
                                        </div>
                                    </div>
                                    <div class="col-sm-6 col-md-4 col-lg-4">
                                        <div style="text-align: center">
                                            <a href="/comprase.aspx" class="accesos-cc homeIconosIzq">
                                                <img src="/images/icoComprar.png" height="90" title="COMPRAR" />
                                            </a>
                                            <a href="/personas.aspx?tipo=p" class="accesos-cc homeIconosDer">
                                                <img src="/images/icoTareas.png" height="90" title="TAREAS" />
                                            </a>
                                        </div>
                                    </div>
                                </div>

                                <div class="col-sm-6 col-md-6 col-lg-6 homeCuadros">
                                    <div class="col-sm-6 col-md-6 col-lg-6">
                                        <div class="wrapper-widget">
                                            <div class="widget-currency">
                                                <div class="widget-currency-col">
                                                    <a href="/comprobantes.aspx?tipo=PDV">
                                                        <div id="divTituloVentasTotal" runat="server" visible="false">
                                                            <span class="title">Ventas del mes (*) 
                                                            </span>
                                                        </div>
                                                        <div id="divTituloVentasConIVA" runat="server" visible="false">
                                                            <span class="title">Ventas del mes sin IVA (*)
                                                            </span>
                                                        </div>
                                                        <div class="wrapper-value">
                                                            <span>$<asp:Literal runat="server" ID="litIngresosMes"></asp:Literal></span>
                                                        </div>
                                                    </a>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-sm-6 col-md-6 col-lg-6">
                                        <div class="wrapper-widget">
                                            <div class="widget-currency">
                                                <div class="widget-currency-col">
                                                    <asp:HyperLink ID="lnkCobranzasPendientes" runat="server" NavigateUrl="/modulos/reportes/cobranzasPendientes.aspx">
                                                        <span class="title">Pendiente de cobro (*) 
                                                        </span>
                                                        <div class="wrapper-value">
                                                            <span>$<asp:Literal runat="server" ID="litPorCobrar"></asp:Literal></span>
                                                        </div>
                                                    </asp:HyperLink>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="row" style="display:none">
                                <div class="col-sm-6 col-md-6 col-lg-6 homeCuadros">
                                    <div class="col-sm-6 col-md-6 col-lg-6">
                                        <div class="wrapper-widget">
                                            <div class="widget-currency">
                                                <div class="widget-currency-col">
                                                    <a href="/compras.aspx">
                                                        <div id="divCompraMes" runat="server">
                                                            <span class="title">Compra del Mes
                                                            </span>
                                                        </div>
                                                        <div class="wrapper-value">
                                                            <span>$<asp:Literal runat="server" ID="litCompraMes"></asp:Literal></span>
                                                        </div>
                                                    </a>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-sm-6 col-md-6 col-lg-6">
                                        <div class="wrapper-widget">
                                            <div class="widget-currency">
                                                <div class="widget-currency-col">
                                                    <a href="/cuentasCorrientes.aspx">
                                                        <span class="title">Cuenta corriente cliente
                                                        </span>
                                                        <div class="wrapper-value">
                                                            <span>$<asp:Literal runat="server" ID="litCuentaCorrienteCliente"></asp:Literal></span>
                                                        </div>
                                                    </a>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-sm-6 col-md-6 col-lg-6 homeCuadros">
                                    <div class="col-sm-6 col-md-6 col-lg-6">
                                        <div class="wrapper-widget">
                                            <div class="widget-currency">
                                                <div class="widget-currency-col">
                                                    <a href="/cuentasCorrientes.aspx">
                                                        <span class="title">Cuenta corriente proveedor
                                                        </span>
                                                        <div class="wrapper-value">
                                                            <span>$<asp:Literal runat="server" ID="litCuentaCorrienteProveedor"></asp:Literal></span>
                                                        </div>
                                                    </a>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-sm-6 col-md-6 col-lg-6">
                                        <div class="wrapper-widget">
                                            <div class="widget-currency">
                                                <div class="widget-currency-col">
                                                    <a href="/comprobantes.aspx?tipo=FAC">
                                                        <div id="divFacturadoDeVentasMes" runat="server">
                                                            <span class="title">Facturado de ventas Mes
                                                            </span>
                                                        </div>
                                                        <div class="wrapper-value">
                                                            <span>$<asp:Literal runat="server" ID="litFacturadoDeVentasMes"></asp:Literal></span>
                                                        </div>
                                                    </a>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>

                        </div>
                    </div>
                </div>
            </div>
            <div class="col-sm-6 col-md-3" runat="server" visible="false">
                <div class="panel panel-success panel-stat">
                    <div class="panel-heading" style="min-height: 183px">
                        <div class="stat">
                            <div class="row">
                                <div class="col-xs-4">
                                    <i class="fa fa-thumbs-up" style="font-size: 50px; opacity: 1; border: none; padding: 0"></i>
                                </div>
                                <div class="col-xs-8">
                                    <small class="stat-label">Ventas del mes sin IVA</small>
                                    <h1>$<asp:Literal runat="server" ID="litIngresosMesBk"></asp:Literal></h1>
                                </div>
                            </div>

                            <div class="mb30">
                                <br />
                            </div>

                            <div class="row">
                                <div class="col-xs-6">
                                    <small class="stat-label">Mes anterior</small>
                                    <h4>$<asp:Literal runat="server" ID="litIngresosMesAnterior"></asp:Literal></h4>
                                </div>
                                <div class="col-xs-6 text-right">
                                    <small class="stat-label">Este año</small>
                                    <h4>$<asp:Literal runat="server" ID="litIngresosAnio"></asp:Literal></h4>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <div class="col-sm-12 col-md-12 col-lg-12" id="divMensajeHome" runat="server" style="padding-bottom: 10px">
                <span>
                    <img src="/images/icoAtencion.png" title="MENSAJE" />
                    (*) Usuario Nuevo!! Tableros con datos simulados (de ejemplo)</span>
            </div>

            <div class="col-sm-6 col-md-3" runat="server" visible="false">
                <div class="panel panel-danger panel-stat">
                    <div class="panel-heading" style="min-height: 183px">
                        <div class="stat">
                            <div class="row">
                                <div class="col-xs-4">
                                    <i class="glyphicon glyphicon-fire" style="font-size: 50px; opacity: 1; border: none; padding: 0"></i>
                                </div>
                                <div class="col-xs-8">
                                    <small class="stat-label">Pendiente de cobro</small>
                                    <h1>$<asp:Literal runat="server" ID="litPorCobrarBk"></asp:Literal></h1>
                                </div>
                            </div>

                            <div class="mb30">
                                <br />
                            </div>

                            <div class="row">
                                <div class="col-xs-7">
                                    <small class="stat-label">Con atraso mayor a 60 días</small>
                                    <h4>$<asp:Literal runat="server" ID="litPorCobrarUrgente"></asp:Literal></h4>
                                </div>
                                <div class="col-xs-5 text-right">
                                    <a href="/modulos/reportes/cobranzasPendientes.aspx" class="btn btn-sm" style="border: 1px solid #ccc; color: #fff;">Ver detalle</a>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <div class="row">
            <%-- [Andy] Change class col-md-6 to col-md-12 --%>
            <div class="col-sm-12 col-md-12">
                <div class="panel panel-warning panel-alt widget-messaging">
                    <div class="panel-heading">
                        <h3 class="panel-title">Ventas vs Compras - Evolución últimos 12 meses (*) <small style="font-size: 10px; color: #fff">No se incluyen las cotizaciones.</small></h3>
                    </div>
                    <div class="panel-body" style="height: 346px; background-color: #fff">
                        <div id="line-chart" style="height: 300px;"></div>
                    </div>
                </div>
            </div>

            <%-- [Andy] Add cladd hidden to hide content --%>
            <div class="col-sm-12 col-md-12" visible="false" runat="server">
                <div class="panel panel-info panel-alt widget-messaging">
                    <div class="panel-heading">
                        <div class="panel-btns">
                            <%--<a href="/modulos/reportes/cuentasPagar.aspx"  class="btn btn-white btn-sm pull-right">Ver todas</a>--%>
                            <%--<a href="/modulos/reportes/cuentasPagar.aspx" class="panel-edit pull-right" style="color: #fff; opacity: inherit; font-size: 12px; font-weight: normal;">--%>
                            <%--<i class="fa fa-file-text"></i>--%>
                            <%--                     Ver todas--%>
                            <%--    </a>--%>
                            <%--<a href="/modulos/reportes/cuentasPagar.aspx" class="pull-right" style="border: 1px solid #ccc;color: #fff;">Ver todas</a>--%>
                        </div>
                        <h3 class="panel-title">Cuentas a pagar (*) (<span id="TotalComprobantes"></span>)</h3>
                    </div>
                    <div class="panel-body" style="height: 346px">
                        <ul id="ulTemplateContainer">
                        </ul>
                    </div>
                </div>
            </div>
        </div>

       <%-- <div class="row">
                <h3>Sales Comparison Report with Chart using ASP.NET.</h3>
                <table width="100%">
                    <tr>
                        <td width="35%" valign="top">
                            <asp:GridView ID="gvSales" runat="server" AutoGenerateColumns="false" Width="95%">
                                <Columns>
                                    <asp:BoundField HeaderText="Year" DataField="Year" />
                                    <asp:BoundField HeaderText="Quarter 1" DataField="Quarter1" />
                                    <asp:BoundField HeaderText="Quarter 2" DataField="Quarter2" />
                                    <asp:BoundField HeaderText="Quarter 3" DataField="Quarter3" />
                                    <asp:BoundField HeaderText="Quarter 4" DataField="Quarter4" />
                                </Columns>
                            </asp:GridView>
                        </td>
                        <td width="50%" valign="top">
                
                        </td>
                    </tr>
                    <tr>
                        <td colspan="2">
                            <asp:Chart ID="Chart1" runat="server" BorderlineWidth="0" Width="800px">
                                <Series>
                                    <asp:Series Name="Series1" XValueMember="Year" YValueMembers="Quarter1"
                                        LegendText="Quarter 1" IsValueShownAsLabel="false" ChartArea="ChartArea1"
                                        MarkerBorderColor="#DBDBDB">
                                    </asp:Series>
 
                                    <asp:Series Name="Series2" XValueMember="Year" YValueMembers="Quarter2"
                                        LegendText="Quarter 2" IsValueShownAsLabel="false" ChartArea="ChartArea1"
                                        MarkerBorderColor="#DBDBDB">
                                    </asp:Series>
 
                                    <asp:Series Name="Series3" XValueMember="Year" YValueMembers="Quarter3"
                                        LegendText="Quarter 3" IsValueShownAsLabel="false" ChartArea="ChartArea1"
                                        MarkerBorderColor="#DBDBDB">
                                    </asp:Series>
 
                                    <asp:Series Name="Series4" XValueMember="Year" YValueMembers="Quarter4"
                                        LegendText="Quarter 4" IsValueShownAsLabel="false" ChartArea="ChartArea1"
                                        MarkerBorderColor="#DBDBDB">
                                    </asp:Series>
                                </Series>
                                <Legends>
                                    <asp:Legend Title="Quarter" />
                                </Legends>
                                <Titles>
                                    <asp:Title Docking="Bottom" Text="Sales Report Quarterly" />
                                </Titles>
                                <ChartAreas>
                                    <asp:ChartArea Name="ChartArea1"></asp:ChartArea>
                                </ChartAreas>
                            </asp:Chart>
                        </td>
                    </tr>
                </table>    
            </div>--%>

        <input type="hidden" id="hdnPanelDeControl" runat="server" value="0" />
    </div>
    <script id="templateFacturasPendientes" type="text/x-jQuery-tmpl">
        {{each results}}
                <li>
                    <%--<small class="pull-right">
                        <a onclick="dashBoard.verFacturasPendientes(${ID})" class="btn btn-white btn-sm" >Ver factura</a>
                        <a onclick="dashBoard.pagarFacturasPendientes(${ID})" class="btn btn-white btn-sm">Pagar</a>
                    </small>--%>
                    <h4 class="sender">${Fecha} - ${RazonSocial}</h4>
                    <small>${Numero} por $ ${ImporteTotalNeto}</small>
                </li>
        {{/each}}
    </script>

    <script id="NotemplateFacturasPendientes" type="text/x-jQuery-tmpl">
        <li>
            <h4 class="sender">No se han registrado cuentas a pagar</h4>
        </li>
    </script>

    </div>

</asp:Content>


<asp:Content ID="Content3" ContentPlaceHolderID="FooterContent" runat="Server">
    <script src="/js/jquery.tmpl.min.js"></script>
    <%--<script src="/js/flot/jquery.flot.js"></script>
    <script src="/js/flot/jquery.flot.resize.min.js"></script>
    <script src="/js/jquery.validate.min.js"></script>
    <script src="/js/flot/flot.pie.min.js"></script>
    <script src="/js/flot/jquery.flot.orderBars.min.js"></script>
    <script src="/js/flot/jquery.flot.time.js"></script>
    <script src="/js/flot/jquery.flot.categories.js"></script>--%>
    <%--<script src="js/jquery.sparkline.min.js"></script>--%>
    <script src="/js/morris.min.js"></script>
    <script src="/js/raphael-2.1.0.min.js"></script>
    <script src="/js/views/home.js?v=<%= ConfigurationManager.AppSettings["JS.Version"] %>"></script>

</asp:Content>
