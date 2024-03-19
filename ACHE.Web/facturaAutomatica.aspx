<%@ Page Title="" Language="C#" MasterPageFile="~/Front.master" AutoEventWireup="true" CodeFile="facturaAutomatica.aspx.cs" Inherits="facturaAutomatica" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="pageheader">
        <h2><i class="fa fa-file-text"></i>Monitor de Facturación <span>Herramientas</span></h2>
        <div class="breadcrumb-wrapper">
            <span class="label">Estás aquí:</span>
            <ol class="breadcrumb">
                <li><a href="/home.aspx">axanweb</a></li>
                <li class="active">Herramientas</li>
            </ol>
        </div>
    </div>

    <div class="contentpanel">



        <div class="row mb15">

            <div class="col-sm-12">

                <div class="alert alert-danger" id="divError" style="display: none">
                    <button type="button" class="close" data-dismiss="alert" aria-hidden="true">×</button>
                    <strong>Lo sentimos!</strong> <span id="msgError"></span>
                </div>
                <div class="alert alert-success" id="divOk" style="display: none">
                    <strong></strong><span id="msgOk"></span>
                </div>

                <ul class="nav nav-tabs nav-justified nav-profile">
                    <li id="liFacturaDeVenta" class="active" runat="server">
                        <a href="#FacturaDeVenta" data-toggle="tab" style="text-align: left;">
                            <strong><span style="font-size: large; font-weight: bold">Facturas de Venta</span></strong><br />
                            <small></small>
                        </a>
                    </li>
                    <li id="liPedidoDeVenta" runat="server">
                        <a href="#PedidoDeVenta" data-toggle="tab" style="text-align: left;">
                            <strong><span style="font-size: large; font-weight: bold">Pedidos de Venta</span></strong><br />
                            <small></small>
                            <small></small>
                        </a>
                    </li>
                </ul>

                <form runat="server">
                    <div class="tab-content">

                        <div class="tab-pane active" id="FacturaDeVenta" runat="server">
                            <div class="col-sm-12 col-md-12 table-results">
                                <div class="panel panel-default">
                                    <!-- panel-heading -->
                                    <div class="panel-body">

                                        <h3>Facturas pendientes de procesar </h3>
                                        <br />
                                        <div class="row mb15">
                                            <div class="col-sm-10 col-md-8 col-lg-5">
                                                <div class="form-group">
                                                    <label class="control-label">
                                                        Cliente
                                                    </label>
                                                    <select class="select2" data-placeholder="Seleccione un cliente..." id="ddlFacturaDeVentaPersona" onchange="filtrar();">
                                                        <option value=""></option>
                                                    </select>
                                                </div>
                                                <div class="form-group">
                                                    <label class="control-label">
                                                        Desde Fecha Comprobante
                                                    </label>
                                                    <asp:TextBox runat="server" ID="txtFacturaDeVentaDesdeFechaComprobante" CssClass="form-control validDate" placeholder="Fecha desde" MaxLength="10" onchange="filtrar();"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                        <br />
                                        <div class="table-responsive">
                                            <table class="table mb30" id="tablaFacturaDeVentaFacturasPendientesDeProcesar">
                                                <thead>
                                                    <tr>
                                                        <th>FechaComprobante</th>
                                                        <th>Cantidad</th>
                                                        <th>ImporteTotal</th>
                                                        <th></th>
                                                        <th></th>
                                                        <th></th>
                                                    </tr>
                                                </thead>
                                                <tbody id="resultsFacturaDeVentaFacturasPendientesDeProcesar">
                                                </tbody>
                                            </table>
                                        </div>


                                        <br />
                                        <div class="table-responsive" id="divFacturaDeVentaTabla" style="display: none">
                                            <table class="table table-invoice">
                                                <thead>
                                                    <tr>
                                                        <th>Tipo</th>
                                                        <th>Numero</th>
                                                        <th>Importe</th>
                                                        <th>CAE</th>
                                                        <th>Observaciones</th>
                                                    </tr>
                                                </thead>
                                                <tbody id="bodyDetalleFacturaDeVenta">
                                                </tbody>
                                            </table>
                                        </div>

                                        <br />
                                        <hr />

                                        <h3>
                                            <p>Facturas Procesadas</p>
                                        </h3>
                                        <br />
                                        <div class="row mb15">
                                            <div class="col-sm-10 col-md-8 col-lg-5">
                                                <div class="form-group">
                                                    <label class="control-label">
                                                        Código de proceso
                                                    </label>
                                                    <select class="select2" data-placeholder="Seleccione un código de proceso..." id="ddlFacturaDeVentaCodigoProceso" onchange="changeCodigoProceso();">
                                                        <option value=""></option>
                                                    </select>
                                                </div>
                                            </div>
                                        </div>
                                        <br />
                                        <div class="table-responsive">
                                            <table class="table mb30" id="tablaFacturaDeVentaFacturasProcesadas">
                                                <thead>
                                                    <tr>
                                                        <th>ID</th>
                                                        <th>Cliente</th>
                                                        <th>Fecha</th>
                                                        <th>Numero</th>
                                                        <th>Imp. Neto Grav</th>
                                                        <th>Total</th>
                                                        <th></th>
                                                        <th></th>
                                                    </tr>
                                                </thead>
                                                <tbody id="resultsFacturaDeVentaFacturaAutomaticaProcesados">
                                                </tbody>
                                            </table>
                                        </div>

                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="tab-pane" id="PedidoDeVenta" runat="server">
                            <div class="col-sm-12 col-md-12 table-results">
                                <div class="panel panel-default">
                                    <!-- panel-heading -->
                                    <div class="panel-body">

                                        <h3>Pedidos de venta </h3>
                                        <br />
                                        <div class="row mb15">
                                            <div class="col-sm-4 col-md-3 col-lg-2">
                                                <div class="form-group">
                                                    <h5 class="subtitle mb10">Periodo:</h5>
                                                    <select class="select2" data-placeholder="Seleccione un periodo..." id="ddlPeriodo" onclick="obtenerInfoPorPeriodo();">
                                                        <option value=""></option>
                                                    </select>
                                                </div>
                                            </div>
                                            <div class="col-sm-4 col-md-3 col-lg-2">
                                                <div class="form-group">
                                                    <h5 class="subtitle mb10">Facturas del Mes:</h5>
                                                    <h4 class="text-primary" style="color: green" id="divPedidoDeVentaFacturasDelMes">$ <label id="lbPedidoDeVentaFacturasDelMes" style="color: green"></label></h4>
                                                </div>
                                            </div>
                                            <div class="col-sm-4 col-md-3 col-lg-2">
                                                <div class="form-group">
                                                    <h5 class="subtitle mb10">Compras del Mes:</h5>
                                                    <h4 class="text-primary" style="color: green" id="divPedidoDeVentaComprasDelMes">$ <label id="lbPedidoDeVentaComprasDelMes" style="color: green"></label></h4>
                                                </div>
                                            </div>
                                            <div class="col-sm-4 col-md-3 col-lg-2">
                                                <div class="form-group">
                                                    <h5 class="subtitle mb10">Gastos Generales:</h5>
                                                    <h4 class="text-primary" style="color: green" id="divPedidoDeVentaGastosGenerales">$ <label id="lbPedidoDeVentaGastosGenerales" style="color: green"></label></h4>
                                                </div>
                                            </div>
                                        </div>
                                        <br />
                                        <div class="table-responsive">
                                            <table class="table mb30" id="tablaPedidoDeVentaResumen">
                                                <thead>
                                                    <tr>
                                                        <th></th>
                                                        <th></th>
                                                        <th></th>
                                                        <th></th>
                                                        <th>Imp. Neto Gra. Total</th>
                                                        <th>Fac. Borrador Total</th>
                                                        <th>Fac. CAE Total</th>
                                                        <th></th>
                                                    </tr>
                                                </thead>
                                                <tbody id="resultsPedidoDeVentaResumen">
                                                </tbody>
                                            </table>
                                        </div>
                                       
                                    </div>
                                </div>
                            </div>
                        </div>

                    </div>
                </form>
            </div>


        </div>
    </div>

    <div id="divEspera" style="display: none;">
        <img src="images/loaders/cargando.gif" width="50%" height="50%" />
    </div>

    <div class="modal modal fade" id="modalEMail" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true" data-backdrop="static" data-keyboard="false">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title" id="litModalOkTitulo">Enviar email </h4>
                </div>
                <div class="modal-body" style="min-height: 200px;">
                    <div class="alert alert-success" id="divOkMail" style="display: none">
                        <strong>Bien hecho! </strong>El mensaje ha sido enviado correctamente
                    </div>
                    <div id="divSendEmail">
                        <div class="row">
                            <div class="col-sm-12">
                                <div class="alert alert-danger" id="divErrorMail" style="display: none">
                                    <strong>Lo sentimos! </strong><span id="msgErrorMail"></span>
                                </div>

                                <form id="frmSendMail">
                                    <div class="form-group">
                                        <p><span class="asterisk">*</span> Para: <small>(separa las direcciones mediante una coma) </small></p>
                                        <input id="txtEnvioPara" class="form-control required multiemails" type="text" runat="server" />
                                        <%--<span id="msgErrorEnvioPara" class="help-block" style="display: none">Una de las direcciones ingresadas es inválida.</span>--%>
                                    </div>
                                    <div class="form-group">
                                        <p><span class="asterisk">*</span> Asunto: </p>
                                        <input id="txtEnvioAsunto" class="form-control required" type="text" maxlength="150" runat="server" />
                                        <span id="msgErrorEnvioAsunto" class="help-block" style="display: none">Este campo es obligatorio.</span>
                                    </div>
                                    <div class="form-group">
                                        <p><span class="asterisk">*</span> Mensaje: </p>
                                        <textarea rows="5" id="txtEnvioMensaje" class="form-control required" runat="server"></textarea>
                                        <span id="msgErrorEnvioMensaje" class="help-block" style="display: none">Este campo es obligatorio.</span>
                                    </div>
                                    <input type="hidden" id="hdnFile" />
                                    <input type="hidden" id="hdnRazonSocial" />
                                    <br />
                                    <a type="button" class="btn btn-success" onclick="Toolbar.enviarComprobantePorMail();" id="btnEnviar">Enviar</a>
                                    <a style="margin-left: 20px" href="#" onclick="Toolbar.cancelarEnvio();">Cancelar</a>
                                </form>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <a style="margin-left: 20px" href="#" data-dismiss="modal">Cerrar</a>
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-wide fade" id="modalPedidoDeVentaDetalle" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
        <div class="modal-dialog" style="display:contents">
            <div class="modal-content" >
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title" id="titDetalle" style="display: inline-table;"></h4>

                    <div class="btn-group" style="margin-left: 20px">
                        <div class="btn btn-white tooltips">

                            <a id="divIconoDescargarDetalle" onclick="exportarPedidoDeVentaDetalle();">
                                <i class="glyphicon glyphicon-save"></i>&nbsp;Exportar
                            </a>
                            <img alt="" src="/images/loaders/loader1.gif" id="imgLoadingPedidoDeVentaDetalle" style="display: none" />
                            <a href="" id="lnkPedidoDeVentaDownloadDetalle" onclick="resetearExportacionPedidoDeVentaDetalle();" download="PedidoDeVentaDetalle" style="display: none">Descargar</a>
                        </div>
                    </div>
                </div>
                <div class="modal-body">
                    <div class="table-responsive">
                        <table class="table table-success mb30 smallTable" id="tablaPedidoDeVentaDetalle">
                            <thead>
                                <tr>
                                    <th>Rango Cliente</th>
                                    <th>Nro</th>
                                    <th>Proveedor/Cliente</th>
                                    <th>Nombre</th>
                                    <th>Fecha</th>
                                    <th>Imp. Neto Gra.</th>
                                    <th>Fac. Borrador</th>
                                    <th>Fac. CAE</th>
                                    <th></th>
                                </tr>
                            </thead>
                            <tbody id="resultsPedidoDeVentaDetalle">
                            </tbody>
                        </table>
                    </div>
                </div>
                <input type="hidden" id="hdnPedidoDeVentaDetallePeriodo" />
                <input type="hidden" id="hdnPedidoDeVentaDetalleRango" />
                <div class="modal-footer">
                    <a style="margin-left:20px" href="#" type="button" data-dismiss="modal">Cerrar</a>
                </div>
            </div>
        </div>
    </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FooterContent" runat="Server">
    <script type="text/javascript" src="/js/jquery.Datatables.min.js?v=<%= ConfigurationManager.AppSettings["JS.Version"] %>"></script>
    <script type="text/javascript" src="/js/jquery.tmpl.min.js?v=<%= ConfigurationManager.AppSettings["JS.Version"] %>"></script>
    <script src="/js/jquery.ui.widget.js?v=<%= ConfigurationManager.AppSettings["JS.Version"] %>"></script>
    <script src="/js/jquery.iframe-transport.js?v=<%= ConfigurationManager.AppSettings["JS.Version"] %>"></script>
    <script src="/js/jquery.validate.min.js?v=<%= ConfigurationManager.AppSettings["JS.Version"] %>"></script>
    <script src="/js/select2.min.js?v=<%= ConfigurationManager.AppSettings["JS.Version"] %>"></script>
    <script src="/js/views/facturaAutomatica.js?v=<%= ConfigurationManager.AppSettings["JS.Version"] %>"></script>
    <script src="js/jquery.blockUI.js?v=<%= ConfigurationManager.AppSettings["JS.Version"] %>"></script>
    
    <script>
        jQuery(document).ready(function () {
            configForm();
        });
    </script>

</asp:Content>
