<%@ Page Title="" Language="C#" MasterPageFile="~/Front.master" AutoEventWireup="true" CodeFile="retenciones.aspx.cs" Inherits="modulos_reportes_retenciones" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="pageheader">
        <h2><i class="fa fa-bar-chart-o"></i>&nbsp;Retenciones </h2>
        <div class="breadcrumb-wrapper">
            <span class="label">Estás aquí:</span>
            <ol class="breadcrumb">
                <li><a href="/home.aspx">axanweb</a></li>
                <li><a href="#">Reportes</a></li>
                <li class="active">Retenciones</li>
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
                        <option value="Ganancias">Ganancias</option>
                        <option value="SUSS">SUSS</option>
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
            <div class="col-sm-8 col-md-9 table-results">
                <div class="panel panel-default">
                    <div class="panel-heading">
                        <div class="pull-right">
                            <div class="btn-group mr10">
                                <div class="btn btn-white tooltips">
                                    <a id="divIconoDescargar" href="javascript:exportar();">
                                        <i class="glyphicon glyphicon-save"></i>&nbsp;Exportar
                                    </a>
                                    <img alt="" src="/images/loaders/loader1.gif" id="imgLoading" style="display: none" />
                                    <a href="" id="lnkDownload" onclick="resetearExportacion();" download="Retenciones" style="display: none">Descargar</a>
                                </div>
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
                                        <th>Razón Social</th>
                                        <th>CUIT</th>
                                        <th>Condición Iva</th>
                                        <%--<th>Tipo ret.</th>--%>
                                        <th>Nro ref</th>
                                        <th style="text-align: right">Importe</th>
                                        <th></th>
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
            <td>${RazonSocial}</td>
            <td>${Cuit}</td>
            <td>${CondicionIVA}</td>
            <%--<td>${Tipo}</td>--%>
            <td>${NroReferencia}</td>
            <td style="text-align: right">${Importe}</td>
            <td><a onclick="imprimirRetencionGanancia(${IDRetencion});" style="cursor: pointer; font-size: 16px" title="Ver Comprobante"><i class="fa fa-file-pdf-o fa-2x text-center"></i></a></td>
        </tr>
        {{/each}}
    </script>

    <script id="noResultTemplate" type="text/x-jQuery-tmpl">
        <tr>
            <td colspan="6">No se han encontrado resultados
            </td>
        </tr>
    </script>

    <!-- Modal -->
    <div class="modal modal-wide fade" id="modalPdf" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content" style="position:fixed">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Visualización del Recibo de Pago</h4>
                </div>
                <div class="modal-body">
                    <div>
                        <div class="alert alert-danger" id="divErrorCat" style="display: none">
                            <strong>Lo sentimos! </strong><span id="msgErrorCat"></span>
                        </div>

                        <iframe id="ifrPdf" src="" width="900px" height="500px" frameborder="0"></iframe>
                    </div>
                </div>
                <div class="modal-footer">
                    <asp:HyperLink runat="server" ID="lnkDescargar" CssClass="btn btn-primary" download><i class="fa fa-cloud-download fa-fw"></i> Descargar</asp:HyperLink>
                    <asp:HyperLink runat="server" ID="lnkPrint2" CssClass="btn btn-default"><span class="glyphicon glyphicon-print"></span> Imprimir</asp:HyperLink>
                    <a style="margin-left: 20px" href="#" data-dismiss="modal">Cerrar</a>
                </div>
            </div>
        </div>
    </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FooterContent" runat="Server">
    <script type="text/javascript" src="/js/jquery.tmpl.min.js"></script>
    <script src="/js/jquery.validate.min.js"></script>
    <script src="/js/select2.min.js"></script>
    <script src="/js/views/reportes/retenciones.js?v=<%= ConfigurationManager.AppSettings["JS.Version"] %>"></script>

    <script>
        jQuery(document).ready(function () {
            filtrar();
            configFilters();
        });
    </script>

</asp:Content>

