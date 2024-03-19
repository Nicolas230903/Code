<%@ Page Title="" Language="C#" MasterPageFile="~/Front.master" AutoEventWireup="true" CodeFile="cc.aspx.cs" Inherits="modulos_reportes_cc" %>

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
        <h2><i class="fa fa-bar-chart-o"></i>Cuenta corriente <span>Administración</span></h2>
        <div class="breadcrumb-wrapper">
            <span class="label">Estás aquí:</span>
            <ol class="breadcrumb">
                <li><a href="/home.aspx">axanweb</a></li>
                <li><a href="#">Reportes</a></li>
                <li class="active">Cuenta corriente</li>
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
                    <select class="select2" data-placeholder="Seleccione un cliente/proveedor..." id="ddlPersona" onchange="CC.changePersona();">
                        <option value=""></option>
                    </select>
                    <div class="mb20"></div>    
                    <h4 class="subtitle mb5">Vistas</h4>
                    
                    <input type="radio" name="rCtaCte" id="rCtaCteCliente" value="1" checked="checked"/> <label>Ver como Cliente</label> &nbsp; 
                    <input type="radio" name="rCtaCte" id="rCtaCteProv" value="0"/> <label>Ver como Proveedor</label>


                     <label><input type="checkbox" checked="checked" id="chkSaldoPendiente"/><span>&nbsp;Con Saldo Pendiente</span></label>
       

                    <div class="mb20"></div>

                    <a class="btn btn-black" onclick="CC.filtrar();">Buscar</a>
                    <br />
                </form>
            </div>
            <div class="col-sm-8 col-md-9 table-results">
                <div class="panel panel-default">
                    <div class="panel-heading">
                        <div class="pull-right">
                            <div class="btn-group mr10">
                                <div class="btn btn-white tooltips">
                                    <a  id="divIconoDescargar" href="javascript:CC.exportar();">
                                        <i class="glyphicon glyphicon-save"></i>&nbsp;Exportar
                                    </a>
                                    <img alt="" src="/images/loaders/loader1.gif" id="imgLoading" style="display:none" />
                                    <a href="" id="lnkDownload" onclick="CC.resetearExportacion();" download="CuentaCorriente" style="display:none">Descargar</a>
                                </div>
                                <%--<button class="btn btn-white tooltips" type="button" data-toggle="tooltip" title="" data-original-title="Nuevo" onclick="nuevo();"><i class="glyphicon glyphicon-add"></i></button>--%>
                                <%--<button class="btn btn-white tooltips" type="button" data-toggle="tooltip" title="" data-original-title="Delete"><i class="glyphicon glyphicon-trash"></i></button>--%>
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
                                        <th>Comprobante</th>
                                        <th>Fecha</th>
                                        <th>Comprobante aplicado</th>
                                        <th id="thfecha">Fecha Cobro</th>
                                        <th>Importe</th>
                                        <th id ="thImporte">Cobrado</th>
                                        <th>Total</th>
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
            {{if $value.Cobrado != "Saldo"}}//Si no es la ultima columna, muestro todo normal. Solo por cuestiones estéticas.
                <td {{if $value.Comprobante != ""}} class="bgRow" {{/if}}>
                    ${RazonSocial}
                </td>
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
                <td class="bgTotal" colspan="6">&nbsp;</td>
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

     <script id="noResultTemplate" type="text/x-jQuery-tmpl">
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
    <script src="/js/views/reportes/cc.js"></script>

    <script>
        jQuery(document).ready(function () {
            CC.configFilters();
        });
    </script>
</asp:Content>