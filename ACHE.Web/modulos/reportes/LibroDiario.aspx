﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Front.master" AutoEventWireup="true" CodeFile="LibroDiario.aspx.cs" Inherits="modulos_reportes_LibroDiario" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="pageheader">
        <h2><i class="fa fa-bar-chart-o"></i>LibroDiario</h2>
        <div class="breadcrumb-wrapper">
            <span class="label">Estás aquí:</span>
            <ol class="breadcrumb">
                <li><a href="/home.aspx">axanweb</a></li>
                <li><a href="#">Reportes</a></li>
                <li class="active">Libro diario</li>
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

                    <a class="btn btn-black" onclick="LibroDiario.resetearPagina();LibroDiario.filtrar();">Buscar</a>
                    <a class="btn btn-default" onclick="LibroDiario.resetearPagina();LibroDiario.verTodos();">Ver todos</a>

                    <br />
                </form>
            </div>
            <div class="col-sm-8 col-md-9 table-results">
                <div class="panel panel-default">
                    <div class="panel-heading">
                        <div class="pull-right">
                            <div class="btn-group mr10">
                                <div class="btn btn-white tooltips">
                                    <a id="divIconoDescargar" href="javascript:LibroDiario.exportar();">
                                        <i class="glyphicon glyphicon-save"></i>&nbsp;Exportar
                                    </a>
                                    <img alt="" src="/images/loaders/loader1.gif" id="imgLoading" style="display: none" />
                                    <a href="" id="lnkDownload" onclick="LibroDiario.resetearExportacion();" download="IvaVentas" style="display: none">Descargar</a>
                                </div>
                            </div>

                            <div class="btn-group mr10" id="divPagination" style="display: none">
                                <a class="btn btn-white" id="lnkPrevPage" style="cursor: pointer" onclick="LibroDiario.mostrarPagAnterior();"><i class="glyphicon glyphicon-chevron-left"></i>Anterior</a>
                                <a class="btn btn-white" id="lnkNextPage" style="cursor: pointer" onclick="LibroDiario.mostrarPagProxima();">Siguiente <i class="glyphicon glyphicon-chevron-right"></i></a>
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
                                        <th>Nro asiento</th>
                                        <th>Fecha</th>
                                        <th>Leyenda</th>
                                        <th>Código</th>
                                        <th>Descripción</th>
                                        <th style="text-align: right">Débito</th>
                                        <th style="text-align: right">Crédito</th>
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
                <td class="bgTotal">${NroAsiento}</td>
                <td class="bgTotal">${Fecha}</td>
                <td class="bgTotal">${Leyenda}</td>
                <td class="bgTotal"></td>
                <td class="bgTotal"></td>
                <td class="bgTotal"></td>
                <td style="text-align: right" class="bgTotal">
                    <a onclick="LibroDiario.editar(${IDAsiento});" style="cursor: pointer; font-size: 16px; color: currentColor" title="Editar">
                        <i class="fa fa-pencil"></i>
                    </a>
                    <a onclick="LibroDiario.eliminar(${IDAsiento},'${Leyenda}');" style="cursor: pointer; font-size: 16px; color: currentColor" class="delete-row" title="Eliminar">
                        <i class="fa fa-trash-o"></i>
                    </a>
                </td>
            </tr>

        {{each Items}}
        <tr>
            <td></td>
            <td></td>
            <td></td>
            <td>${nroCuenta}</td>
            <td>${NombreCuenta}</td>
            <td style="text-align: right;">${Debe}</td>
            <td style="text-align: right">${Haber}</td>
        </tr>
        {{/each}}
        {{/each}}
        
        <tr>
            <td class="bgTotal text-danger"></td>
            <td class="bgTotal text-danger"></td>
            <td class="bgTotal text-danger"></td>
            <td class="bgTotal text-danger"></td>
            <td class="bgTotal text-danger">Totales</td>
            <td style="text-align: right" class="bgTotal text-danger"><span id="totalDebe"></span></td>
            <td style="text-align: right" class="bgTotal text-danger"><span id="totalHaber"></span></td>
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
    <script src="/js/views/reportes/LibroDiario.js?v=<%= ConfigurationManager.AppSettings["JS.Version"] %>"></script>

    <script>
        jQuery(document).ready(function () {
            LibroDiario.configFilters();
            LibroDiario.filtrar();
        });
    </script>
</asp:Content>

