<%@ Page Title="" Language="C#" MasterPageFile="~/Front.master" AutoEventWireup="true" CodeFile="cobrado-vs-pagado.aspx.cs" Inherits="modulos_reportes_cobrado_vs_pagado" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
    <%--<link href="/css/morris.css" rel="stylesheet" />--%>
    <style type="text/css">
        .modal.modal-wide .modal-dialog {
          width: 90%;
        }
        .modal-wide .modal-body {
          overflow-y: auto;
        }
        
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">

    <div class="pageheader">
        <h2><i class="fa fa-bar-chart-o"></i>Cobrado vs Pagado</h2>
        <div class="breadcrumb-wrapper">
            <span class="label">Estás aquí:</span>
            <ol class="breadcrumb">
                <li><a href="/home.aspx">axanweb</a></li>
                <li><a href="#">Reportes</a></li>
                <li class="active">Cobrado vs Pagado</li>
            </ol>
        </div>
    </div>

    <div class="contentpanel">

        <div class="col-sm-12 col-md-12">
            <div class="panel panel-default">
                <div class="panel-heading">
                    <h4 class="panel-title">¿Qué información estoy viendo en este gráfico? Las cobranzas y pagos efectuados en los últimos 12 meses.</h4>
                    <p>Haciendo click en cada barra puede ver el detalle de la misma</p>
                </div>
                <!-- panel-heading -->

                <div class="panel-body">
                    <div id="barchart" style="width: 100%; height: 300px;" ></div>
                    <p></p>
                    <p style="text-align:center">Haga click en cada barra para ver el detalle de la misma</p>
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
                            <a href="" id="lnkDownloadDetalle" onclick="resetearExportacionDetalle();" download="Ingresos-Egresos" style="display: none">Descargar</a>
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
    <input type="hidden" id="hdnPeriodo" value="0" />
    <input type="hidden" id="hdnEtiqueta" value="0" />
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FooterContent" runat="Server">
    <script src="/js/flot/jquery.flot.js"></script>
    <script src="/js/flot/jquery.flot.resize.min.js"></script>
    <%--<script src="/js/flot/jquery.flot.symbol.min.js"></script>
    <script src="/js/flot/jquery.flot.crosshair.min.js"></script>--%>
    <script src="/js/flot/jquery.flot.orderBars.min.js"></script>
    <script src="/js/flot/jquery.flot.time.js"></script>
    <script src="/js/flot/jquery.flot.categories.js"></script>
    <script src="/js/views/reportes/cobrado-vs-pagado.js?v=<%= ConfigurationManager.AppSettings["JS.Version"] %>"></script>

</asp:Content>

