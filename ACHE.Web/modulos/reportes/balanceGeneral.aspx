<%@ Page Title="" Language="C#" MasterPageFile="~/Front.master" AutoEventWireup="true" CodeFile="balanceGeneral.aspx.cs" Inherits="modulos_reportes_balanceGeneral" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="pageheader">
        <h2><i class="fa fa-bar-chart-o"></i>Balance general de sumas y saldos</h2>
        <div class="breadcrumb-wrapper">
            <span class="label">Estás aquí:</span>
            <ol class="breadcrumb">
                <li><a href="/home.aspx">axanweb</a></li>
                <li><a href="#">Reportes</a></li>
                <li class="active">Balance general de sumas y saldos</li>
            </ol>
        </div>
    </div>

    <div class="contentpanel">
        <div class="row">
            <div class="col-sm-4 col-md-3">
                <form runat="server" id="frmSearch">
                    <input type="hidden" id="hdnPage" runat="server" value="1" />

                    <h4 class="subtitle mb5">Filtros disponibles</h4>

                    <div class="mb20 hide"></div>
                    <h4 class="subtitle mb5 hide">Proveedor/Cliente</h4>
                    <select class="select2 hide" data-placeholder="Seleccione un cliente/proveedor..." id="ddlPersona">
                        <option value=""></option>
                    </select>

                    <div class="mb20"></div>
                    <h4 class="subtitle mb5">Fecha</h4>
                    <div class="row row-pad-5">
                        <div class="col-lg-6">
                            <div class="input-group">
                                <span class="input-group-addon"><i class="glyphicon glyphicon-calendar"></i></span>
                                <asp:TextBox runat="server" ID="txtFechaDesde" CssClass="form-control validDate greaterThan" placeholder="dd/mm/yyyy" MaxLength="10"></asp:TextBox>
                            </div>
                            <label for="txtFechaDesde" class="error" style="display: none">La fecha desde es mayor a la fecha hasta</label>
                        </div>
                        <div class="col-lg-6">
                            <div class="input-group">
                                <span class="input-group-addon"><i class="glyphicon glyphicon-calendar"></i></span>
                                <asp:TextBox runat="server" ID="txtFechaHasta" CssClass="form-control validDate greaterThan" placeholder="dd/mm/yyyy" MaxLength="10"></asp:TextBox>
                            </div>
                            <label for="txtFechaHasta" class="error" style="display: none">La fecha desde es mayor a la fecha hasta</label>
                        </div>
                    </div>

                    <div class="mb20"></div>

                    <a class="btn btn-black" onclick="balanceGeneral.resetearPagina();balanceGeneral.filtrar();">Buscar</a>
                    <a class="btn btn-default" onclick="balanceGeneral.resetearPagina();balanceGeneral.verTodos();">Ver todos</a>

                    <br />
                </form>
            </div>
            <div class="col-sm-8 col-md-9 table-results">
                <div class="panel panel-default">
                    <div class="panel-heading">
                        <div class="pull-right">
                            <div class="btn-group mr10">
                                <div class="btn btn-white tooltips">
                                    <a id="divIconoDescargar" href="javascript:balanceGeneral.exportar();">
                                        <i class="glyphicon glyphicon-save"></i>&nbsp;Exportar
                                    </a>
                                    <img alt="" src="/images/loaders/loader1.gif" id="imgLoading" style="display: none" />
                                    <a href="" id="lnkDownload" onclick="balanceGeneral.resetearExportacion();" download="IvaVentas" style="display: none">Descargar</a>
                                </div>
                            </div>

                            <div class="btn-group mr10" id="divPagination" style="display: none">
                                <a class="btn btn-white" id="lnkPrevPage" style="cursor: pointer" onclick="balanceGeneral.mostrarPagAnterior();"><i class="glyphicon glyphicon-chevron-left"></i>Anterior</a>
                                <a class="btn btn-white" id="lnkNextPage" style="cursor: pointer" onclick="balanceGeneral.mostrarPagProxima();">Siguiente <i class="glyphicon glyphicon-chevron-right"></i></a>
                            </div>
                        </div>

                        <h4 class="panel-title">Resultados</h4>
                        <p id="msjResultados"></p>
                    </div>
                    <!-- panel-heading -->
                    <div class="panel-body">

                        <div class="alert alert-danger" id="divError" style="display: none">
                            <strong>Lo sentimos!</strong> <span id="msgError"></span>
                        </div>

                        <div class="table-responsive">
                            <table class="table mb30">
                                <thead>
                                      <tr>
                                        <th></th>
                                        <th></th>
                                        <th style="text-align: right"></th>
                                        <th style="text-align: right"></th>
                                        <th style="text-align: center;border-left-style: solid;border-right-style: solid;border-bottom-style: hidden;" colspan="2">Saldos</th>
                                        
                                        <th style="text-align: center;border-left-style: solid;border-right-style: solid;border-bottom-style: hidden;" colspan="2">Balance general</th>
                                        
                                        <th style="text-align: center;border-left-style: solid;border-right-style: solid;border-bottom-style: hidden;" colspan="2">Estado de resultado</th>
                                        
                                    </tr>
                                    <tr>
                                        <th>Código</th>
                                        <th>Nombre de la Cuenta</th>
                                        <th style="text-align: right">Débitos</th>
                                        <th style="text-align: right">Créditos</th>
                                        <th style="text-align: right;border-left-style: solid;">Deudor</th>
                                        <th style="text-align: right;border-right-style: solid;">Acreedor</th>

                                        <th style="text-align: right;border-left-style: solid;">Activo</th>
                                        <th style="text-align: right;border-right-style: solid;">Pasivo</th>

                                        <th style="text-align: right;border-left-style: solid;">Pérdidas</th>
                                        <th style="text-align: right;border-right-style: solid;">Ganancias</th>
                                    </tr>
                                </thead>
                                <tbody id="resultsContainer">
                                </tbody>
                            </table>
                        </div>

                    </div>
                </div>
            </div>
        </div>
    </div>
        
    
    <script id="resultTemplate" type="text/x-jQuery-tmpl">
        {{each results}}
            <tr>
                <td>${nroCuenta}</td>
                <td>${NombreCuenta}</td>

                <td style="text-align: right">${TotalDebe}</td>
                <td style="text-align: right">${TotalHaber}</td>

                <td style="text-align: right;border-left-style: solid;">${TotalDeudor}</td>
                <td style="text-align: right;border-right-style: solid;">${TotalAcreedor}</td>

                <td style="text-align: right;border-left-style: solid;">${TotalActivo}</td>
                <td style="text-align: right;border-right-style: solid;">${TotalPasivo}</td>

                <td style="text-align: right;border-left-style: solid;">${TotalPerdidas}</td>
                <td style="text-align: right;border-right-style: solid;">${TotalGanancias}</td>
            </tr>

        {{/each}}
        
        <tr>
            <td class="bgTotal text-danger"></td>
            <td class="bgTotal text-danger">Totales</td>

            <td class="bgTotal text-danger" style="text-align: right"><span id="totalDebe"></span></td>
            <td class="bgTotal text-danger" style="text-align: right"><span id="totalHaber"></span></td>

            <td class="bgTotal text-danger" style="text-align: right"><span id="totalDeudor"></span></td>
            <td class="bgTotal text-danger" style="text-align: right"><span id="totalAcreedor"></span></td>

            <td class="bgTotal text-danger" style="text-align: right"><span id="totalActivo"></span></td>
            <td class="bgTotal text-danger" style="text-align: right"><span id="totalPasivo"></span></td>

            <td class="bgTotal text-danger" style="text-align: right"><span id="totalPerdidas"></span></td>
            <td class="bgTotal text-danger" style="text-align: right;border-right-style: solid;"><span id="totalGanancias"></span></td>
        </tr>
    </script>
    <script id="noResultTemplate" type="text/x-jQuery-tmpl">
        <tr>
            <td colspan="6">No se han encontrado resultados
            </td>
        </tr>
    </script>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FooterContent" runat="Server">
    <script type="text/javascript" src="/js/jquery.tmpl.min.js"></script>
    <script src="/js/jquery.validate.min.js"></script>
    <script src="/js/select2.min.js"></script>
    <script src="/js/views/reportes/balanceGeneral.js?v=<%= ConfigurationManager.AppSettings["JS.Version"] %>"></script>

    <script>
        jQuery(document).ready(function () {
            balanceGeneral.configFilters();
            balanceGeneral.filtrar();
        });
    </script>
</asp:Content>

