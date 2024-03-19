<%@ Page Title="" Language="C#" MasterPageFile="~/Front.master" AutoEventWireup="true" CodeFile="compraAutomatica.aspx.cs" Inherits="compraAutomatica" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="pageheader">
        <h2><i class="fa fa-file-text"></i>Generación Compra Automática <span>Herramientas</span></h2>
        <div class="breadcrumb-wrapper">
            <span class="label">Estás aquí:</span>
            <ol class="breadcrumb">
                <li><a href="/home.aspx">axanweb</a></li>
                <li class="active">Herramientas</li>
            </ol>
        </div>
    </div>

    <div class="contentpanel">
        <div class="row">
            <div class="col-sm-12 col-md-12 table-results">
                <div class="panel panel-default">
                    <!-- panel-heading -->
                    <div class="panel-body">

                        <h3>
                            <p>Pedidos de venta pendientes de procesar </p>
                        </h3>
                        <br />
                        <div class="row mb15">
                            <div class="col-sm-10 col-md-8 col-lg-5">
                                <div class="form-group">
                                    <label class="control-label">
                                        Proveedor
                                    </label>
                                    <select class="select2" data-placeholder="Seleccione un proveedor..." id="ddlPersona" onchange="changeProveedor();">
                                        <option value=""></option>
                                    </select>
                                </div>
                            </div>
                        </div>
                        <br />
                        <div class="table-responsive">
                            <table class="table mb30" id="tablaPedidosDeVentaPendientesDeProcesar">
                                <thead>
                                    <tr>
                                        <th>
                                            <input type="checkbox" id="checkAll" class="checkbox-inline" /></th>
                                        <th>ID</th>
                                        <th>Cliente</th>
                                        <th>Fecha</th>
                                        <th>Nombre</th>
                                        <th>Numero</th>
                                        <th>Imp. Neto Grav</th>
                                        <th>Total</th>
                                        <th>Estado</th>
                                        <th class="columnIcons"></th>
                                    </tr>
                                </thead>
                                <tbody id="resultsPedidosPendientesDeProcesar">
                                </tbody>
                            </table>
                        </div>

                        <div class="panel-footer" runat="server" id="divFooter">
                            <div class="pull-right">
                                <a class="btn btn-success" onclick="verificarCantidadDeProveedores();" id="lnkProcesar">Procesar</a>
                            </div>
                        </div>

                        <br />

                        <div class="alert alert-danger" id="divError" style="display: none">
                            <button type="button" class="close" data-dismiss="alert" aria-hidden="true">×</button>
                            <strong>Lo sentimos!</strong> <span id="msgError"></span>
                        </div>
                        <div class="alert alert-success" id="divOk" style="display: none">
                            <strong>Bien hecho! </strong><span id="msgOk"></span>
                        </div>

                        <br />
                        <hr />

                        <h3>
                            <p>Pedidos Procesados</p>
                        </h3>
                        <br />
                        <div class="row mb15">
                            <div class="col-sm-10 col-md-8 col-lg-5">
                                <div class="form-group">
                                    <label class="control-label">
                                        Código de proceso
                                    </label>
                                    <select class="select2" data-placeholder="Seleccione un código de proceso..." id="ddlCodigoProceso" onchange="changeCodigoProceso();">
                                        <option value=""></option>
                                    </select>
                                </div>
                            </div>
                            <div class="col-sm-10 col-md-8 col-lg-5">
                                <div class="form-group">
                                    <label class="control-label">
                                        Proveedor
                                    </label>
                                    <select class="select2" data-placeholder="Seleccione un proveedor de proceso..." id="ddlPersonaProceso" onchange="changeCodigoProceso();">
                                        <option value=""></option>
                                    </select>
                                </div>
                            </div>
                        </div>
                        <br />
                        <div class="table-responsive">
                            <table class="table mb30" id="tablaPedidosProcesados">
                                <thead>
                                    <tr>
                                        <th>ID</th>
                                        <th>Proveedor</th>
                                        <th>Fecha</th>
                                        <%-- <th>Nombre</th>--%>
                                        <th>Numero</th>
                                        <th>Imp. Neto Grav</th>
                                        <th>Total</th>
                                        <th></th>
                                        <th></th>
                                        <th></th>
                                    </tr>
                                </thead>
                                <tbody id="resultsPedidosDeVentaProcesados">
                                </tbody>
                            </table>
                        </div>

                    </div>
                </div>
            </div>

        </div>
    </div>


    <script id="noResultTemplate" type="text/x-jQuery-tmpl">
        <tr>
            <td colspan="8">No se han encontrado resultados
            </td>
        </tr>
    </script>

    <div id="divEspera" style="display: none;">
        <img src="images/loaders/cargando.gif" width="50%" height="50%" />
    </div>

    <div class="modal modal fade" id="modalComprobantesVinculados" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true" style="display: none">
        <div class="modal-dialog">
            <div class="modal-content" style="position: fixed">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Comprobantes vinculados</h4>
                </div>
                <div class="modal-body">
                    <div>
                        <div class="modal-header">
                            <h4 class="modal-title">Pedidos de venta</h4>
                        </div>

                        <div class="alert alert-danger" id="divErrorPedidoDeVenta" style="display: none">
                            <strong>Lo sentimos! </strong><span id="msgErrorPedidoDeVenta"></span>
                        </div>

                        <div class="table-responsive">
                            <table class="table mb30">
                                <thead>
                                    <tr>
                                        <!--th>#</!--th-->
                                        <th>Número</th>
                                        <th>Proveedor/Cliente</th>
                                        <th>Fecha</th>
                                        <th>Tipo</th>
                                        <th>Total</th>
                                    </tr>
                                </thead>
                                <tbody id="resultsContainerPedidoDeVenta">
                                </tbody>
                            </table>
                        </div>

                        <script id="resultTemplatePedidoDeVenta" type="text/x-jQuery-tmpl">
                            {{each results}}
                        <tr>
                            <!--td>${ID}</td-->
                            <td>${Numero}</td>
                            <td>${RazonSocial}</td>
                            <td>${Fecha}</td>
                            <td>${Tipo}</td>
                            <td>${ImporteTotalNeto}</td>
                        </tr>
                            {{/each}}
                        </script>

                        <script id="noResultTemplatePedidoDeVenta" type="text/x-jQuery-tmpl">
                            <tr>
                                <td colspan="8">No se han encontrado resultados
                                </td>
                            </tr>
                        </script>
                    </div>
                </div>
                <hr />
                <div class="modal-footer">
                    <a style="margin-left: 20px" href="#" type="button" data-dismiss="modal">Cerrar</a>
                </div>
            </div>
        </div>
    </div>


</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FooterContent" runat="Server">
    <script type="text/javascript" src="/js/jquery.Datatables.min.js"></script>
    <script type="text/javascript" src="/js/jquery.tmpl.min.js"></script>
    <script src="/js/select2.min.js"></script>
    <script src="/js/views/compraAutomatica.js?v=<%= ConfigurationManager.AppSettings["JS.Version"] %>"></script>
    <script src="js/jquery.blockUI.js?v=<%= ConfigurationManager.AppSettings["JS.Version"] %>"></script>

    <script>
        jQuery(document).ready(function () {
            configForm();
        });
    </script>
</asp:Content>

