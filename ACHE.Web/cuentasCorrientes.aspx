<%@ Page Title="" Language="C#" MasterPageFile="~/Front.master" AutoEventWireup="true" CodeFile="cuentasCorrientes.aspx.cs" Inherits="cuentasCorrientes" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
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
        <h2>
            <asp:Literal runat="server" ID="litTitulo"></asp:Literal>
            <span>Administración</span></h2>
        <div class="breadcrumb-wrapper">
            <span class="label">Estás aquí:</span>
            <ol class="breadcrumb">
                <li><a href="/home.aspx">axanweb</a></li>
                <li class="active">
                    <asp:Literal runat="server" ID="litPath"></asp:Literal></li>
            </ol>
        </div>
    </div>

    <div id="divConDatos" runat="server">
        <div class="contentpanel">
            <div class="row">
                <div class="col-sm-12 col-md-12 table-results">
                    <div class="panel panel-default">
                        <div class="panel-heading">
                            <input type="hidden" id="hdnPage" runat="server" value="1" />
                            <input type="hidden" id="hdnID" runat="server" value="1" />
                            <div class="col-sm-5 col-md-5">
                                <input type="text" class="form-control" id="txtRazonSocial" maxlength="128" placeholder="Ingresá Nombre,Código, CUIT o DNI" />
                            </div>
                            <div class="col-sm-12">
                                <hr />
                            </div>
                            <div class="col-sm-12">                                                   
                                <input type="radio" name="rCtaCte" id="rCtaCteClienteMain" value="1" onchange="cambiarTipo('C');" checked="checked" />
                                <label>Ver como Cliente</label>
                                <input type="radio" name="rCtaCte" id="rCtaCteProvMain" value="0" onchange="cambiarTipo('P');" />
                                <label>Ver como Proveedor</label>
                            </div>
                            <div class="col-sm-12">
                                <label><input type="checkbox" checked="checked" id="chkSaldoPendiente" onchange="filtrar();"/><span>&nbsp;Con Saldo Pendiente</span></label>
                            </div>
                            <div class="col-sm-12">
                                <label><input type="checkbox" id="chkDeudaPorEDM" onchange="filtrar();"/><span>&nbsp;Deuda por EDM</span></label>
                            </div>
                            <div class="col-sm-12">
                                <hr />
                            </div>
                            <div class="row mb20"></div>
                            <div class="row">
                                <div class="pull-right">
                                    <div class="btn-group mr10" id="divDownloadExportacionResumen" style="display: none">
                                        <div class="btn btn-white tooltips">
                                            <a id="divIconoDescargarExportacionResumen" href="javascript:exportarResumen();">
                                                <i class="glyphicon glyphicon-save"></i>&nbsp;Exportar Resumen
                                            </a>
                                            <img alt="" src="/images/loaders/loader1.gif" id="imgLoadingExportacionResumen" style="display: none" />
                                            <a href="" id="lnkDownloadExportacionResumen" onclick="resetearExportacionResumen();" download="CuentasCorrientesResumen" style="display: none">Descargar Resumen</a>
                                        </div>
                                    </div>
                                    <div class="btn-group mr10" id="divDownloadExportacionDetalle" style="display: none">
                                        <div class="btn btn-white tooltips">
                                            <a id="divIconoDescargarExportacionDetalle" href="javascript:exportarDetalle();">
                                                <i class="glyphicon glyphicon-save"></i>&nbsp;Exportar Detalle
                                            </a>
                                            <img alt="" src="/images/loaders/loader1.gif" id="imgLoadingExportacionDetalle" style="display: none" />
                                            <a href="" id="lnkDownloadExportacionDetalle" onclick="resetearExportacionDetalle();" download="CuentasCorrientesDetalle" style="display: none">Descargar Detalle</a>
                                        </div>
                                    </div>
                                    <div class="btn-group mr10" id="divPagination" style="display: none">
                                        <a class="btn btn-white" id="lnkPrevPage" style="cursor: pointer" onclick="mostrarPagAnterior();"><i class="glyphicon glyphicon-chevron-left"></i>Anterior</a>
                                        <a class="btn btn-white" id="lnkNextPage" style="cursor: pointer" onclick="mostrarPagProxima();">Siguiente <i class="glyphicon glyphicon-chevron-right"></i></a>
                                    </div>
                                </div>
                               <h4 class="panel-title" style="clear:left;padding-left:20px">Resultados</h4>
                                <p id="msjResultados" style="padding-left:20px"></p>
                            </div>
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
     <%--                                       <th>Logo</th>
                                            <th>Razón Social</th>
                                            <th>Código</th>
                                            <th>DNI/CUIT</th>
                                            <th>Cat. Impositiva</th>
                                            <th>Teléfono</th>
                                            <th>Email</th>
                                            <th class="columnIcons"></th>--%>
                                            <th>ID</th>
                                            <th>Razón Social</th>
                                            <th>DNI/CUIT</th>
                                            <th>Cat. Impositiva</th>
                                            <th>Saldo</th>
                                            <th class="columnIcons"></th>
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

<%--        <script id="resultTemplate" type="text/x-jQuery-tmpl">
            {{each results}}
            <tr>
                <td>{{if TieneFoto == 1}}
                        <img src="${Foto}" alt="" style="width: 40px; height: 40px;" />
                    {{else}}
                        <img src="/files/usuarios/no-photo.png" alt="" style="width: 25px; height: 25px;" />
                    {{/if}}
                </td>
                <td>${RazonSocial} &nbsp;<small><i>${NombreFantasia}</i></small></td>
                <td>${Codigo}</td>
                <td>${NroDoc}</td>
                <td>${CondicionIva}</td>
                <td>${Telefono}</td>
                <td>${Email}</td>
                <td class="table-action">
                    <a onclick="detalleCuentaCorriente(${ID});" style="cursor: pointer; font-size: 16px" title="Editar"><i class="fa fa-pencil"></i></a>                    
                </td>
            </tr>
            {{/each}}
        </script>--%>

        <script id="resultTemplate" type="text/x-jQuery-tmpl">
            {{each results}}
            <tr>
                <td>${ID}</td>
                <td>${RazonSocial} &nbsp;<small><i>${NombreFantasia}</i></small></td>
                <td>${NroDoc}</td>
                <td>${CondicionIva}</td>
                <td>${Saldo}</td>
                <td class="table-action">
                    <a onclick="detalleCuentaCorriente(${ID},'${RazonSocial}');" style="cursor: pointer; font-size: 16px" title="Editar"><i class="fa fa-pencil"></i></a>                    
                </td>
            </tr>
            {{/each}}
        </script>

        <script id="noResultTemplate" type="text/x-jQuery-tmpl">
            <tr>
                <td colspan="8">No se han encontrado resultados
                </td>
            </tr>
        </script>
    </div>

    <div id="divSinDatos" runat="server">
        <div class="panel-heading" style="background-color: white; height: 30%; width: 100%; border-radius: 4px; text-align: center;">
            <h2 id="hTitulo"></h2>
        </div>
    </div>
    <input type="hidden" id="hdnTipo" runat="server" />
    <input type="hidden" id="hdnIDUsuario" runat="server" />
    <input type="hidden" id="hdnRazonSocial" runat="server" />

     <div id="divEspera" style="display: none;">
        <img src="images/loaders/cargando.gif" width="50%" height="50%" />
    </div>

     <!-- Modal -->
    <div class="modal modal-wide fade" id="modalDetalle" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title" id="titDetalle"></h4>
                    <%--style="display: inline-table;"--%>

                    <br />
                    <input type="radio" name="rCtaCte" id="rCtaCteCliente" value="1" onchange="CambiarReporte();" />
                    <label>Ver como Cliente</label>
                    <input type="radio" name="rCtaCte" id="rCtaCteProv" value="0" onchange="CambiarReporte();" />
                    <label>Ver como Proveedor</label>
                </div>
                <div class="modal-body">
                    <h4><span id="lblVista"></span></h4>
                    <h4><span id="lblRazonSocial"></span></h4>
                    <div class="table-responsive">
                        <table class="table table-success mb30 smallTable">
                            <thead>
                                <tr>
                                    <th>PDC</th>
                                    <th>Raiz</th>
                                    <th>Comprobante</th>
                                    <th>CAE</th>
                                    <th>Fecha</th>
                                    <th>Comprobante aplicado</th>
                                    <th>Imp. Neto</th>
                                    <th>IVA</th>
                                    <th>Va a Deuda</th>
                                    <th>Fecha Cobro</th>
                                    <th>Cobrado</th>
                                    <th>Total</th>
                                </tr>
                            </thead>
                            <tbody id="bodyDetalle">
                            </tbody>
                        </table>
                    </div>
                </div>
                <div class="modal-footer">
                    <a href="#" type="button" style="margin-left: 20px"  onclick="RealizarCobranza();" class="btn btn-info">Cobranza</a> 
                    <a href="#" type="button" style="margin-left: 20px"  onclick="ImprimirDetalleCuentaCorriente();" class="btn btn-success">Imprimir</a> 
                    <a style="margin-left:20px" href="#" data-dismiss="modal">Cerrar</a>
                </div>
                
            </div>            
        </div>        
    </div>

    <div class="modal modal-wide fade" id="modalPdf" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true" >
        <div class="modal-dialog">
            <div class="modal-content" style="position:fixed">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Visualización del detalle de cuenta corriente</h4>
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
                    <div id="divPrevisualizarCerrar" class="col-sm-3 CAJA_BLANCA_AZUL">
                        <a id="lnkPrevisualizarCerrar" data-dismiss="modal">
                            <i class="glyphicon glyphicon-log-in" style="font-size: 30px; opacity: 1; border: none; padding: 0"></i>
                            <br />
                            Cerrar
                        </a>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <script id="resultTemplateDetalle" type="text/x-jQuery-tmpl">
        {{each results}}
        <tr>
            {{if $value.Cobrado != "Saldo"}}//Si no es la ultima columna, muestro todo normal. Solo por cuestiones estéticas.
                <td {{if $value.Comprobante != ""}} class="bgRow" {{/if}}>
                    ${PDC}
                </td>
                <td {{if $value.Comprobante != ""}} class="bgRow" {{/if}}>
                    ${Raiz}
                </td>
                <td {{if $value.Comprobante != ""}} class="bgRow" {{/if}}>
                    ${Comprobante}
                </td>
                <td {{if $value.Comprobante != ""}} class="bgRow" {{/if}}>
                    ${CAE}
                </td>
                <td {{if $value.Comprobante != ""}} class="bgRow" {{/if}}>
                    ${Fecha}
                </td>
                <td {{if $value.Comprobante != ""}} class="bgRow" {{/if}}>
                    ${ComprobanteAplicado}
                </td>
                <td {{if $value.Comprobante != ""}} class="bgRow" {{/if}}>
                    ${Importe}
                </td>
                <td {{if $value.Comprobante != ""}} class="bgRow" {{/if}}>
                    ${IVA}
                </td>
                <td {{if $value.Comprobante != ""}} class="bgRow" {{/if}}>
                    ${VaADeuda}
                </td>
                <td {{if $value.Comprobante != ""}} class="bgRow" {{/if}}>
                    ${FechaCobro}
                </td>
                <td {{if $value.Comprobante != ""}} class="bgRow" {{/if}}>
                    ${Cobrado}
                </td>
                <td {{if $value.Comprobante != ""}} class="bgRow" {{/if}}>
                    ${Total}
                </td>
            {{else}}
                <td class="bgTotal" colspan="10">&nbsp;</td>
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
            <td colspan="7">No se han encontrado resultados
            </td>
        </tr>
    </script>
    
</asp:Content>

<asp:Content ID="Content6" ContentPlaceHolderID="FooterContent" runat="Server">
    <script type="text/javascript" src="/js/jquery.tmpl.min.js"></script>
    <script src="js/jquery.blockUI.js?v=<%= ConfigurationManager.AppSettings["JS.Version"] %>"></script>
    <script src="/js/views/cuentasCorrientes.js?v=<%= ConfigurationManager.AppSettings["JS.Version"] %>"></script>

    <script>
        jQuery(document).ready(function () {
            if ($('#divConDatos').is(":visible")) {
                filtrar();
                configFilters();
            }
            else {
                configFormSinDatos();
            }
        });
    </script>
</asp:Content>
