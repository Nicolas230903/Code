<%@ Page Title="" Language="C#" MasterPageFile="~/Front.master" AutoEventWireup="true" CodeFile="compras-por-categoria.aspx.cs" Inherits="modulos_reportes_compras_por_categoria" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
    <style type="text/css">
        .legend div
        {
            background-color: transparent;
        }
        .modal.modal-wide .modal-dialog {
          width: 90%;
        }
        .modal-wide .modal-body {
          overflow-y: auto;
        }
    </style>
    
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">

     <div class="pageheader">
        <h2><i class="fa fa-bar-chart-o"></i>Compras por categoría</h2>
        <div class="breadcrumb-wrapper">
            <span class="label">Estás aquí:</span>
            <ol class="breadcrumb">
                <li><a href="/home.aspx">axanweb</a></li>
                <li><a href="#">Reportes</a></li>
                <li class="active">Compras por categoría</li>
            </ol>
        </div>
    </div>

    <div class="contentpanel">

         <div class="col-sm-12 col-md-12">
             <div class="panel panel-default">
                <div class="panel-heading">
                    <h4 class="panel-title">¿Qué información estoy viendo en este gráfico? Las compras por categoría efectuadas en el período de fechas seleccionadas. No se incluyen las cotizaciones</h4>
                    <p>Haciendo click en cada categoría puede ver el detalle de la misma</p>
                </div><!-- panel-heading -->
                
                <div class="panel-body">
                    
                    <div class="col-md-3 mb30">
                        <h4 class="subtitle mb5">Filtros disponibles</h4>
                        <form runat="server" id="frmSearch">
                            <h4 class="subtitle mb5">Fecha</h4>
                            <div class="row row-pad-5">
                                <div class="col-lg-6">
                                    <div class="input-group">
                                        <span class="input-group-addon"><i class="glyphicon glyphicon-calendar"></i></span>
                                        <asp:TextBox runat="server" ID="txtFechaDesde" CssClass="form-control validDate greaterThan" placeholder="dd/mm/yyyy" MaxLength="10"></asp:TextBox>
                                    </div>
                                    <label for="txtFechaDesde" class="error" style="display:none">La fecha desde es mayor a la fecha hasta</label>
                                </div>
                                <div class="col-lg-6">
                                    <div class="input-group">
                                        <span class="input-group-addon"><i class="glyphicon glyphicon-calendar"></i></span>
                                        <asp:TextBox runat="server" ID="txtFechaHasta" CssClass="form-control validDate greaterThan" placeholder="dd/mm/yyyy" MaxLength="10"></asp:TextBox>
                                    </div>
                                    <label for="txtFechaHasta" class="error" style="display:none">La fecha desde es mayor a la fecha hasta</label>
                                </div>
                            </div>
                            <div class="mb20"></div>
                            <a class="btn btn-black" onclick="obtenerResultados();">Buscar</a>
                        </form>
                    </div>
                    <div class="col-md-9 mb30">
                        <h5 class="subtitle mb5">Resultado <!--small>Haga click aquí para ver el detalle</!--small--></h5>
                        <div id="piechart" style="width: 100%; height: 300px"></div>
                        <p></p>
                        <p>Haciendo click en cada categoría puede ver el detalle de la misma</p>
                    </div><!-- col-md-6 -->

                </div>
            </div>
        </div>
    </div>

     <div class="modal modal-wide fade" id="modalDetalle" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title" id="titDetalle" style="display: inline-table;"></h4>

                    <input type="hidden" id="hdnIDPersonaDetalle" />

                    <div class="btn-group" style="margin-left: 20px">
                        <div class="btn btn-white tooltips">

                            <a id="divIconoDescargarDetalle" onclick="exportarDetalle();">
                                <i class="glyphicon glyphicon-save"></i>&nbsp;Exportar
                            </a>
                            <img alt="" src="/images/loaders/loader1.gif" id="imgLoadingDetalle" style="display: none" />
                            <a href="" id="lnkDownloadDetalle" onclick="resetearExportacionDetalle();" download="Egresos-Ingresos" style="display: none">Descargar</a>
                        </div>
                    </div>
                </div>
                <div class="modal-body">
                    <div class="table-responsive">
                        <table class="table table-success mb30 smallTable">
                            <thead>
                                <tr>
                                    <th>Razón Social</th>
                                    <th>Comprobante</th>
                                    <th>Fecha</th>
                                    <th>Total</th>
                                </tr>
                            </thead>
                            <tbody id="bodyDetalle">
                            </tbody>
                        </table>
                    </div>
                </div>
                <div class="modal-footer">
                    <a style="margin-left:20px" href="#" type="button" data-dismiss="modal">Cerrar</a>
                </div>
            </div>
        </div>
    </div>
    <input type="hidden" id="hdnEtiqueta" value="0" />

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FooterContent" Runat="Server">
    <script src="/js/flot/jquery.flot.js"></script>
    <script src="/js/flot/jquery.flot.resize.min.js"></script>
    <script src="/js/jquery.validate.min.js"></script>
    <script src="/js/flot/flot.pie.min.js"></script>
    <%--<script src="/js/flot/jquery.flot.symbol.min.js"></script>
    <script src="/js/flot/jquery.flot.crosshair.min.js"></script>--%>
    <script src="/js/flot/jquery.flot.orderBars.min.js"></script>
    <script src="/js/flot/jquery.flot.time.js"></script>
    <script src="/js/flot/jquery.flot.categories.js"></script>
    <script src="/js/views/reportes/compras-por-categoria.js?v=<%= ConfigurationManager.AppSettings["JS.Version"] %>"></script>

</asp:Content>

