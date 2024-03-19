<%@ Page Title="" Language="C#" MasterPageFile="~/Front.master" AutoEventWireup="true" CodeFile="trackingHora.aspx.cs" Inherits="modulos_reportes_trackingHora" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="pageheader">
        <h2><i class='fa fa-clock-o'></i> Tracking Horas <span>Administración</span></h2>
        <div class="breadcrumb-wrapper">
            <span class="label">Estás aquí:</span>
            <ol class="breadcrumb">
                <li><a href="/home.aspx">axanweb</a></li>
                <li class="active">Tracking Horas</li>
            </ol>
        </div>
    </div>

    <div class="contentpanel">
        <div class="row">
            <div class="col-sm-4 col-md-3">
                <input type="hidden" id="hdnPage" runat="server" value="1" />

                <h4 class="subtitle mb5">Filtros disponibles</h4>
                <form runat="server" id="frmSearch">
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
                    <a class="btn btn-black" onclick="rptTrackingHoras.resetearPagina();rptTrackingHoras.filtrar();">Buscar</a>
                    <a class="btn btn-default" onclick="rptTrackingHoras.resetearPagina();rptTrackingHoras.verTodos();">Ver todos</a>
                    <br />
                </form>
            </div>
            <div class="col-sm-8 col-md-9 table-results">
                <div class="panel panel-default">
                    <div class="panel-heading">
                        <div class="pull-right">
                            <div class="btn-group mr10">
                                <div class="btn btn-white tooltips">
                                    <a id="divIconoDescargar" href="javascript:rptTrackingHoras.exportar();">
                                        <i class="glyphicon glyphicon-save"></i>&nbsp;Exportar
                                    </a>
                                    <img alt="" src="/images/loaders/loader1.gif" id="imgLoading" style="display: none" />
                                    <a href="" id="lnkDownload" onclick="rptTrackingHoras.resetearExportacion();" download="ClientesProv" style="display: none">Descargar</a>
                                </div>
                            </div>

                            <div class="btn-group mr10" id="divPagination" style="display: none">
                                <a class="btn btn-white" id="lnkPrevPage" style="cursor: pointer" onclick="rptTrackingHoras.mostrarPagAnterior();"><i class="glyphicon glyphicon-chevron-left"></i> Anterior</a>
                                <a class="btn btn-white" id="lnkNextPage" style="cursor: pointer" onclick="rptTrackingHoras.mostrarPagProxima();">Siguiente <i class="glyphicon glyphicon-chevron-right"></i></a>
                            </div>
                        </div>

                        <h4 class="panel-title">Resultados</h4>
                        <p id="msjResultados"></p>
                    </div>
                    <!-- panel-heading -->
                    <div class="panel-body">

                        <div class="alert alert-danger" id="divError" style="display: none">
                            <button type="button" class="close" data-dismiss="alert" aria-hidden="true">×</button>
                            <strong>Lo sentimos!</strong> <span id="msgError"></span>
                        </div>

                        <div class="table-responsive">
                            <table class="table mb30">
                                <thead>
                                    <tr>
                                        <th>Razón Social</th>
                                        <th>Usuario</th>
                                        <th  style="text-align:right">Facturables</th>
                                        <th  style="text-align:right">No facturables</th>
                                        <th  style="text-align:right">Total</th>
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
            <td>${RazonSocial}</td>
            <td>${NombreUsuario}</td>
            <td style="text-align:right">${CantHorasFacturables}</td>
            <td style="text-align:right">${CantHorasNOFacturables}</td>
            <td style="text-align:right">${SubTotal}</td>
        </tr>
        {{/each}}
    </script>

    <script id="resultTemplateTotales" type="text/x-jQuery-tmpl">
        {{each results}}
         <tr>
            <td>
            </td>
             <td>
            </td>
            <td style="text-align:right">Total horas facturadas: ${TotalHorasFacturables}
            </td>
            <td style="text-align:right">Total horas NO facturadas: ${TotalHorasNoFacturables}
            </td>
             <td style="text-align:right">Total: ${Total}
            </td>
        </tr>
       {{/each}}
    </script>

    <script id="noResultTemplate" type="text/x-jQuery-tmpl">
        <tr>
            <td colspan="5">No se han encontrado resultados</td>
        </tr>
    </script>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FooterContent" runat="Server">
    <script type="text/javascript" src="/js/jquery.tmpl.min.js"></script>
    <script src="/js/jquery.validate.min.js"></script>
    <script src="/js/select2.min.js"></script>

    <script src="/js/views/reportes/rpt-trackingHoras.js?v=<%= ConfigurationManager.AppSettings["JS.Version"] %>"></script>
    <script>
        jQuery(document).ready(function () {
            rptTrackingHoras.configFilters();
            rptTrackingHoras.filtrar();
        });
    </script>
</asp:Content>
