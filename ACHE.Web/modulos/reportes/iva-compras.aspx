﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Front.master" AutoEventWireup="true" CodeFile="iva-compras.aspx.cs" Inherits="modulos_reportes_iva_compras" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="pageheader">
        <h2><i class="fa fa-bar-chart-o"></i>IVA Compras</h2>
        <div class="breadcrumb-wrapper">
            <span class="label">Estás aquí:</span>
            <ol class="breadcrumb">
                <li><a href="/home.aspx">axanweb</a></li>
                <li><a href="#">Reportes</a></li>
                <li class="active">IVA Compras</li>
            </ol>
        </div>
    </div>

    <div class="contentpanel">
        <div class="row">
            <div class="col-sm-2 col-md-2">
                <form runat="server" id="frmSearch">
                    <input type="hidden" id="hdnPage" runat="server" value="1" />

                    <h4 class="subtitle mb5">Filtros disponibles</h4>

                    <div class="mb20"></div>
                    <h4 class="subtitle mb5">Proveedor/Cliente</h4>
                    <select class="select2" data-placeholder="Seleccione un cliente/proveedor..." id="ddlPersona">
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

                    <a class="btn btn-black" onclick="resetearPagina();filtrar();">Buscar</a>
                    <a class="btn btn-default" onclick="resetearPagina();verTodos();">Ver todos</a>

                    <br />
                </form>
            </div>
            <div class="col-sm-10 col-md-10 table-results">
                <div class="panel panel-default">
                    <div class="panel-heading">
                        <div class="pull-right">
                            <div class="btn-group mr10">
                                <div class="btn btn-white tooltips">
                                    <a id="divIconoDescargar" href="javascript:exportar();">
                                        <i class="glyphicon glyphicon-save"></i>&nbsp;Exportar
                                    </a>
                                    <img alt="" src="/images/loaders/loader1.gif" id="imgLoading" style="display: none" />
                                    <a href="" id="lnkDownload" onclick="resetearExportacion();" download="IvaCompras" style="display: none">Descargar</a>
                                </div>
                                <%--<button class="btn btn-white tooltips" type="button" data-toggle="tooltip" title="" data-original-title="Nuevo" onclick="nuevo();"><i class="glyphicon glyphicon-add"></i></button>--%>
                                <%--<button class="btn btn-white tooltips" type="button" data-toggle="tooltip" title="" data-original-title="Delete"><i class="glyphicon glyphicon-trash"></i></button>--%>
                            </div>

                            <div class="btn-group mr10" id="divPagination" style="display: none">
                                <a class="btn btn-white" id="lnkPrevPage" style="cursor: pointer" onclick="mostrarPagAnterior();"><i class="glyphicon glyphicon-chevron-left"></i>Anterior</a>
                                <a class="btn btn-white" id="lnkNextPage" style="cursor: pointer" onclick="mostrarPagProxima();">Siguiente <i class="glyphicon glyphicon-chevron-right"></i></a>
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
                                        <th>Fecha</th>
                                        <th>Tipo</th>
                                        <th>Nro Factura</th>
                                        <th>Razón Social</th>
                                        <th>CUIT</th>

                                        <th style="text-align: right">Monto Grav Iva 2</th>
                                        <th style="text-align: right">Monto Grav Iva 5</th>
                                        <th style="text-align: right">Monto Grav Iva 10</th>
                                        <th style="text-align: right">Monto Grav Iva 21</th>
                                        <th style="text-align: right">Monto Grav Iva 27</th>
                                        <th style="text-align: right">Monto No Grav y Ex</th>
                                        <th style="text-align: right">Monto Grav Mon</th>
                                        <th style="text-align: right">Iva Fact</th>
                                        <%--<th style="text-align: right">Iva Perc</th>--%>
                                        <th style="text-align: right">Imp Interno</th>
                                        <th style="text-align: right">Imp Municipal</th>
                                        <th style="text-align: right">Imp Nacional</th>

                                        <th style="text-align: right">Otros</th>
                                        <th style="text-align: right">Percepción IVA</th>
                                        <th style="text-align: right">IIBB</th>

                                        <th style="text-align: right">Total Fact</th>
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
            <td>${Fecha}</td>
            <td>${Tipo}</td>
            <td style="min-width: 100px">${NroFactura}</td>
            <td>${RazonSocial}</td>
            <td>${Cuit}</td>
            <td style="text-align: right">${MontoGravadoIva2}</td>
            <td style="text-align: right">${MontoGravadoIva5}</td>
            <td style="text-align: right">${MontoGravadoIva10}</td>
            <td style="text-align: right">${MontoGravadoIva21}</td>
            <td style="text-align: right">${MontoGravadoIva27}</td>
            <td style="text-align: right">${MontoNoGravadoYExentos}</td>
            <td style="text-align: right">${MontoGravadoMonotributistas}</td>
            <td style="text-align: right">${IvaFacturado}</td>
            <%--<td style="text-align: right">${IvaPercepcion}</td>--%>
            <td style="text-align: right">${ImpInterno}</td>
            <td style="text-align: right">${ImpMunicipal}</td>
            <td style="text-align: right">${ImpNacional}</td>

            <td style="text-align: right">${Otros}</td>
            <td style="text-align: right">${PercepcionIVA}</td>
            <td style="text-align: right">${IIBB}</td>

            <td style="text-align: right">${TotalFacturado}</td>
        </tr>
        {{/each}}
    </script>

    <script id="noResultTemplate" type="text/x-jQuery-tmpl">
        <tr>
            <td colspan="14">No se han encontrado resultados
            </td>
        </tr>
    </script>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FooterContent" runat="Server">
    <script type="text/javascript" src="/js/jquery.tmpl.min.js"></script>
    <script src="/js/jquery.validate.min.js"></script>
    <script src="/js/select2.min.js"></script>
    <script src="/js/views/reportes/iva-compras.js?v=<%= ConfigurationManager.AppSettings["JS.Version"] %>"></script>

    <script>
        jQuery(document).ready(function () {
            filtrar();
            configFilters();
        });
    </script>
</asp:Content>

