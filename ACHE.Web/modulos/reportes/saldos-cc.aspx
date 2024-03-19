<%@ Page Title="" Language="C#" MasterPageFile="~/Front.master" AutoEventWireup="true" CodeFile="saldos-cc.aspx.cs" Inherits="modulos_reportes_saldos_cc" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
    <style type="text/css">
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
        <h2><i class="fa fa-bar-chart-o"></i>Saldos de cuenta corriente <span>Administración</span></h2>
        <div class="breadcrumb-wrapper">
            <span class="label">Estás aquí:</span>
            <ol class="breadcrumb">
                <li><a href="/home.aspx">axanweb</a></li>
                <li><a href="#">Reportes</a></li>
                <li class="active">Saldos de cuenta corriente</li>
            </ol>
        </div>
    </div>

    <div class="contentpanel">
        <div class="row">
            <div class="col-sm-4 col-md-3">
                <form runat="server" id="frmSearch">
                    <input type="hidden" id="hdnPage" runat="server" value="1" />

                    <h4 class="subtitle mb5">Filtros disponibles</h4>
        
                    <div class="mb20"></div>    
                    <h4 class="subtitle mb5">Proveedor/Cliente</h4>
                    <select class="select2" data-placeholder="Seleccione un cliente/proveedor..." id="ddlPersona">
                        <option value=""></option>
                    </select>

                    <div class="mb20"></div>

                    <a class="btn btn-black" onclick="SaldosCC.resetearPagina();SaldosCC.filtrar();">Buscar</a>
                    <a class="btn btn-default" onclick="SaldosCC.resetearPagina();SaldosCC.verTodos();">Ver todos</a>
                
                    <br />
                </form>
            </div>
            <div class="col-sm-8 col-md-9 table-results">
                <div class="panel panel-default">
                    <div class="panel-heading">
                        <div class="pull-right">
                            <div class="btn-group mr10">
                                <div class="btn btn-white tooltips" style="display:none">
                                    <a id="divIconoDescargar" href="javascript:SaldosCC.exportar();">
                                        <i class="glyphicon glyphicon-save"></i>&nbsp;Exportar
                                    </a>
                                    <img alt="" src="/images/loaders/loader1.gif" id="imgLoading" style="display:none" />
                                    <a href="" id="lnkDownload" onclick="SaldosCC.resetearExportacion();" download="SaldosCuentaCorriente" style="display:none">Descargar</a>
                                </div>
                                <%--<button class="btn btn-white tooltips" type="button" data-toggle="tooltip" title="" data-original-title="Nuevo" onclick="nuevo();"><i class="glyphicon glyphicon-add"></i></button>--%>
                                <%--<button class="btn btn-white tooltips" type="button" data-toggle="tooltip" title="" data-original-title="Delete"><i class="glyphicon glyphicon-trash"></i></button>--%>
                            </div>
                            
                            <div class="btn-group mr10" id="divPagination" style="display:none">
                                <a class="btn btn-white" id="lnkPrevPage" style="cursor:pointer" onclick="SaldosCC.mostrarPagAnterior();"><i class="glyphicon glyphicon-chevron-left"></i> Anterior</a>
                                <a class="btn btn-white" id="lnkNextPage" style="cursor:pointer" onclick="SaldosCC.mostrarPagProxima();">Siguiente <i class="glyphicon glyphicon-chevron-right"></i></a>
                            </div>
                        </div>

                        <h4 class="panel-title">Resultados</h4>
                        <p id="msjResultados"></p>
                    </div><!-- panel-heading -->
                    <div class="panel-body">
                        <div class="alert alert-danger" id="divError" style="display:none">
                            <strong>Lo sentimos!</strong> <span id="msgError"></span>
                        </div>

                        <div class="table-responsive">
                            <table class="table mb30">
                                <thead>
                                    <tr>
                                        <th>Razón Social</th>
                                        <th>Saldo Cliente</th>
                                        <th>Saldo Proveedor</th>
                                        <th>Saldo Consolidado</th>
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
            <td>
                ${RazonSocial}
            </td>
            <td>
                ${SaldoCliente}
            </td>
            <td>
                ${SaldoProveedor}
            </td>
            <td>
                ${SaldoConsolidado}
            </td>
            <td class="table-action">
                <a onclick="SaldosCC.verDetalle(${IDPersona},'${RazonSocial}');" style="cursor:pointer;font-size: 16px" title="Ver detalle"><i class="fa fa-search"></i></a>
            </td>
        </tr>
        {{/each}}
    </script>

     <script id="noResultTemplate" type="text/x-jQuery-tmpl">
        <tr>
            <td colspan="4">
               No se han encontrado resultados
            </td>
        </tr>
    </script>


    <!-- Modal -->
    <div class="modal modal-wide fade" id="modalDetalle" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title" id="titDetalle" style="display: inline-table;"></h4>

                    <input type="hidden" id="hdnIDPersonaDetalle" />
                    <input type="hidden" id="hdnNombrePersonaDetalle" />
                    
                    <input type="radio" name="rCtaCte" id="rCtaCteCliente" value="1" onchange="SaldosCC.verDetalle();"/> <label>Ver como Cliente</label>
                    <input type="radio" name="rCtaCte" id="rCtaCteProv" value="0" onchange="SaldosCC.verDetalle();"/> <label>Ver como Proveedor</label>
                    <div class="btn-group">
                        <div class="btn btn-white tooltips">

                            <a id="divIconoDescargarDetalle" href="javascript:SaldosCC.exportar();">
                                <i class="glyphicon glyphicon-save"></i>&nbsp;Exportar
                            </a>
                            <img alt="" src="/images/loaders/loader1.gif" id="imgLoadingDetalle" style="display:none" />
                            <a href="" id="lnkDownloadDetalle" onclick="SaldosCC.resetearExportacionDetalle();" download="DetalleCuentaCorriente" style="display:none">Descargar</a>
                        </div>
                    </div>
                    
                    
                </div>
                <div class="modal-body">
                    <div class="table-responsive">
                        <table class="table table-success mb30 smallTable">
                            <thead>
                                <tr>
                                    <th>Comprobante</th>
                                    <th>Fecha</th>
                                    <th>Comprobante aplicado</th>
                                    <th id="thfecha">Fecha Cobro</th>
                                    <th>Importe</th>
                                    <th id ="thImporte">Cobrado</th>
                                    <th>Total</th>
                                </tr>
                            </thead>
                            <tbody id="bodyDetalle">
                
                            </tbody>
                        </table>
                    </div>
                </div>
                <div class="modal-footer">
                    <a style="margin-left:20px" href="#" data-dismiss="modal">Cerrar</a>
                </div>
            </div>
        </div>
    </div>

     <script id="resultTemplateDetalle" type="text/x-jQuery-tmpl">
        {{each results}}
        <tr>
            {{if $value.Cobrado != "Saldo"}}//Si no es la ultima columna, muestro todo normal. Solo por cuestiones estéticas.
                <td {{if $value.Comprobante != ""}} class="bgRow" {{/if}}>
                    ${Comprobante}
                </td>
                <td {{if $value.Comprobante != ""}} class="bgRow" {{/if}}>
                    ${Fecha}
                </td>
                <td {{if $value.Comprobante != ""}} class="bgRow" {{/if}}>
                    ${ComprobanteAplicado}
                </td>
                <td {{if $value.Comprobante != ""}} class="bgRow" {{/if}}>
                    ${FechaCobro}
                </td>
                <td {{if $value.Comprobante != ""}} class="bgRow" {{/if}}>
                    ${Importe}
                </td>
                <td {{if $value.Comprobante != ""}} class="bgRow" {{/if}}>
                    ${Cobrado}
                </td>
                <td {{if $value.Comprobante != ""}} class="bgRow" {{/if}}>
                    ${Total}
                </td>
            {{else}}
                <td class="bgTotal" colspan="5">&nbsp;</td>
                <td class="bgTotal text-danger">
                    ${Cobrado}
                </td>
                <td class="bgTotal text-danger">
                    ${Total}
                </td>
            {{/if}}
        </tr>
        {{/each}}
    </script>

     <script id="noResultTemplateDetalle" type="text/x-jQuery-tmpl">
        <tr>
            <td colspan="7">
               No se han encontrado resultados
            </td>
        </tr>
    </script>

</asp:Content>


<asp:Content ID="Content3" ContentPlaceHolderID="FooterContent" Runat="Server">
    <script type="text/javascript" src="/js/jquery.tmpl.min.js"></script>
    <script src="/js/select2.min.js"></script>
    <script src="/js/views/reportes/saldos-cc.js?v=<%= ConfigurationManager.AppSettings["JS.Version"] %>"></script>

    <script>
        jQuery(document).ready(function () {
            SaldosCC.filtrar();
            SaldosCC.configFilters();
        });
    </script>
</asp:Content>