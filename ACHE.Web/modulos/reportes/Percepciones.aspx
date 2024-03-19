﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Front.master" AutoEventWireup="true" CodeFile="Percepciones.aspx.cs" Inherits="modulos_reportes_Percepciones" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="pageheader">
        <h2><i class="fa fa-bar-chart-o"></i>&nbsp;Percepciones</h2>
        <div class="breadcrumb-wrapper">
            <span class="label">Estás aquí:</span>
            <ol class="breadcrumb">
                <li><a href="/home.aspx">axanweb</a></li>
                <li class="active">Reportes</li>
                <li class="active">Percepciones</li>
            </ol>
        </div>
    </div>

    <div class="contentpanel">
        <div class="row">
            <div class="col-sm-4 col-md-3">
                <form runat="server" id="frmSearch">
                    <input type="hidden" id="hdnPage" runat="server" value="1" />

                    <h4 class="subtitle mb5">Filtros disponibles</h4>

                    <h4 class="subtitle mb5">Proveedor/Cliente</h4>
                    <select class="select2" data-placeholder="Seleccione un cliente/proveedor..." id="ddlPersona">
                        <option value=""></option>
                    </select>
                    <div class="mb20"></div>
                    <h4 class="subtitle mb5">Tipo</h4>
                    <select class="select2" data-placeholder="Seleccione el tipo de percepción" id="ddlTipoPercepcion">
                        <option value="Emitidas">Emitidas</option>
                        <option value="Sufridas">Sufridas</option>
                    </select>
                    <div class="mb20"></div>
                    <h4 class="subtitle mb5">Impuesto</h4>
                    <select class="select2" data-placeholder="Seleccione el impuesto" id="ddlImpuesto">
                        <option value="IIBB">IIBB</option>
                        <option value="IVA">IVA</option>
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

                    <a class="btn btn-black" onclick="percepciones.resetearPagina();percepciones.filtrar();">Buscar</a>
                    <a class="btn btn-default" onclick="percepciones.resetearPagina();percepciones.verTodos();">Ver todos</a>

                    <br />
                </form>
            </div>
            <div class="col-sm-8 col-md-9 table-results">
                <div class="panel panel-default">
                    <div class="panel-heading">
                        <div class="pull-right">
                            <div class="btn-group mr10">
                                <div class="btn btn-white tooltips">
                                    <a id="divIconoDescargar" href="javascript:percepciones.exportar();">
                                        <i class="glyphicon glyphicon-save"></i>&nbsp;Exportar
                                    </a>
                                    <img alt="" src="/images/loaders/loader1.gif" id="imgLoading" style="display: none" />
                                    <a href="" id="lnkDownload" onclick="percepciones.resetearExportacion();" download="Percepciones" style="display: none">Descargar</a>
                                </div>
                            </div>

                            <div class="btn-group mr10" id="divPagination" style="display: none">
                                <a class="btn btn-white" id="lnkPrevPage" style="cursor: pointer" onclick="percepciones.mostrarPagAnterior();"><i class="glyphicon glyphicon-chevron-left"></i>Anterior</a>
                                <a class="btn btn-white" id="lnkNextPage" style="cursor: pointer" onclick="percepciones.mostrarPagProxima();">Siguiente <i class="glyphicon glyphicon-chevron-right"></i></a>
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
                            <table class="table mb30" id="resultsContainer">
                            </table>
                        </div>

                    </div>
                </div>
            </div>
        </div>
    </div>

    <script id="resultTemplateIIBB" type="text/x-jQuery-tmpl">
        <thead>
            <tr>
                <th>Fecha</th>
                <th>Razón Social</th>
                <th>CUIT</th>
                <th>Condición Iva</th>
                <th>Nro de comprobante</th>
                <th>Jurisdicción</th>
                <th style="text-align: right">Importe</th>
            </tr>
        </thead>
        <tbody>
            {{each results}}
            <tr>
                <td>${Fecha}</td>
                <td>${RazonSocial}</td>
                <td>${Cuit}</td>
                <td>${CondicionIVA}</td>
                <td>${NroComprobante}</td>
                <td>${Jurisdiccion}</td>
                <td style="text-align: right">$${Importe}</td>
            </tr>
            {{/each}}
        </tbody>
    </script>

    <script id="resultTemplateIVA" type="text/x-jQuery-tmpl">
        <thead>
            <tr>
                <th>Fecha</th>
                <th>Razón Social</th>
                <th>CUIT</th>
                <th>Condición Iva</th>
                <th>Nro de comprobante</th>
                <th style="text-align: right">Importe</th>
            </tr>
        </thead>
        <tbody>
            {{each results}}
            <tr>
                <td>${Fecha}</td>
                <td>${RazonSocial}</td>
                <td>${Cuit}</td>
                <td>${CondicionIVA}</td>
                <td>${NroComprobante}</td>
                <td style="text-align: right">$${Importe}</td>
            </tr>
            {{/each}}
        </tbody>
    </script>

    <script id="noResultTemplateIIBB" type="text/x-jQuery-tmpl">
        <thead>
            <tr>
                <th>Fecha</th>
                <th>Razón Social</th>
                <th>CUIT</th>
                <th>Condición Iva</th>
                <th>Nro de comprobante</th>
                <th>Jurisdicción</th>
                <th style="text-align: right">Importe</th>
            </tr>
        </thead>
        <tbody>
            <tr>
                <td colspan="7">No se han encontrado resultados
                </td>
            </tr>
        </tbody>
    </script>
        <script id="noResultTemplateIVA" type="text/x-jQuery-tmpl">
        <thead>
            <tr>
                <th>Fecha</th>
                <th>Razón Social</th>
                <th>CUIT</th>
                <th>Condición Iva</th>
                <th>Nro de comprobante</th>
                <th style="text-align: right">Importe</th>
            </tr>
        </thead>
        <tbody>
            <tr>
                <td colspan="6">No se han encontrado resultados
                </td>
            </tr>
        </tbody>
    </script>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FooterContent" runat="Server">
    <script type="text/javascript" src="/js/jquery.tmpl.min.js"></script>
    <script src="/js/jquery.validate.min.js"></script>
    <script src="/js/select2.min.js"></script>
    <script src="/js/views/reportes/percepciones.js?v=<%= ConfigurationManager.AppSettings["JS.Version"] %>"></script>

    <script>
        jQuery(document).ready(function () {
            percepciones.filtrar();
            percepciones.configFilters();
        });
    </script>

</asp:Content>
