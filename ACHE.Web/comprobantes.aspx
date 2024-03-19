<%@ Page Title="" Language="C#" MasterPageFile="~/Front.master" AutoEventWireup="true" CodeFile="comprobantes.aspx.cs" Inherits="comprobantes" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="pageheader">
        <h2><i class="fa fa-file-text"></i>
            <label id="lblTitulo"></label>
            <span>Comercial</span>
        </h2>
        <div class="breadcrumb-wrapper">
            <span class="label">Estás aquí:</span>
            <ol class="breadcrumb">
                <li><a href="/home.aspx">axanweb</a></li>
                <li class="active">
                    <span id="lblSubtitulo"></span>
                </li>
            </ol>
        </div>
    </div>
    <div id="divConDatos" runat="server">
        <div class="contentpanel">
            <div class="row">
                <div class="col-sm-12 col-md-12 table-results">
                    <div class="panel panel-default">
                        <div class="panel-heading">
                            <form runat="server" id="frmSearch">
                                <input type="hidden" id="hdnPage" runat="server" value="1" />
                                <asp:HiddenField runat="server" ID="hdnIDUsuario" Value="0" />
                                <asp:HiddenField runat="server" ID="hdnIDPersona" Value="0" />
                                <asp:HiddenField runat="server" ID="hdnTipo" Value="0" />
                                <asp:HiddenField runat="server" ID="hdnFiltroPorCantidad" Value="0" />
                                <div class="col-sm-12" style="padding-left: inherit">
                                    <div class="col-sm-10 col-md-5">
                                        <div class="row">
                                            <div class="col-sm-5 col-md-5">
                                                <input type="text" class="form-control" id="txtCondicion" maxlength="128" placeholder="Ingrese el número y/o tipo de la factura y/o cliente " />
                                            </div>

                                            <div class="col-sm-5 col-md-5 mb15">
                                                <select class="select2" data-placeholder="Seleccione un periodo de tiempo..." id="ddlPeriodo" onchange="otroPeriodo();">
                                                    <option value="30">Últimos 30 dias</option>
                                                    <option value="15">Últimos 15 dias</option>
                                                    <option value="7">Últimos 7 dias</option>
                                                    <option value="1">Ayer</option>
                                                    <option value="0">Hoy</option>
                                                    <option value="-1">Otro período</option>
                                                    <option value="-2" selected="selected">Todos</option>
                                                </select>
                                            </div>

<%--                                            <div class="col-sm-5 col-md-5" hidden="hidden">                                                
                                                <select class="select2" data-placeholder="Seleccione un cliente/proveedor..." id="ddlPersona">
                                                    <option value=""></option>
                                                </select>
                                            </div>--%>

                                            <div class="col-sm-3 col-md-3">
                                                <div class="btn btn-white" id="divMasFiltros" style="display: none">
                                                    <a onclick="mostrarMasFiltros();">
                                                        <span>Más filtros</span>
                                                    </a>
                                                </div>
                                            </div>       
                                            


                                        </div>

                                        <div class="row mb20"></div>

                                        <div id="divEntreFechas" style="display: none">
                                            <div class="row">
                                                <div class="col-sm-6 col-md-6">
                                                    <div class="input-group">
                                                        <span class="input-group-addon"><i class="glyphicon glyphicon-calendar"></i></span>
                                                        <asp:TextBox runat="server" ID="txtFechaDesde" CssClass="form-control validDate greaterThan" placeholder="Fecha desde" MaxLength="10" onchange="filtrar();"></asp:TextBox>
                                                    </div>
                                                </div>

                                                <div class="col-sm-6 col-md-6">
                                                    <div class="input-group">
                                                        <span class="input-group-addon"><i class="glyphicon glyphicon-calendar"></i></span>
                                                        <asp:TextBox runat="server" ID="txtFechaHasta" CssClass="form-control validDate greaterThan" placeholder="Fecha hasta" MaxLength="10" onchange="filtrar();"></asp:TextBox>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="col-sm-5 col-md-5">
                                        <div class="btn-group" id="btnAcciones" runat="server">
                                            <a class="btn btn-warning mr10" onclick="nuevo();">
                                                <i class="fa fa-plus"></i>&nbsp;
                                                    <span id="lblNuevo"></span>
                                            </a>
                                        </div>
                                        <div class="btn-group dropdown hide">
                                            <button type="button" class="btn btn-btn-default"><i class="fa fa-tasks"></i>Abonos</button>
                                            <button class="btn btn-btn-default dropdown-toggle" data-toggle="dropdown"><span class="fa fa-caret-down"></span></button>
                                            <ul class="dropdown-menu">
                                                <li><a href="/abonos.aspx">Ver abonos actuales</a></li>
                                                <li><a href="/abonose.aspx">Crear abono</a></li>
                                                <li><a href="/generarAbonos.aspx">Facturar abonos</a></li>
                                            </ul>
                                        </div>
                                    </div>
                                </div>
                            </form>

                            <div class="col-sm-12">
                                <hr />
                            </div>
                            <div class="row mb20"></div>
                            <div class="row">
                                <div class="pull-right">

                                    <div id="divCuadroResumen" class="btn-group mr10" hidden="hidden">
                                        <div class="btn btn-white" id="btnCuadroResumen">
                                            <a onclick="cuadroResumen(true);">
                                                <span>Cuadro Resumen</span>
                                            </a>
                                        </div>
                                    </div>

                                    <div id="divReiniciarFiltros" class="btn-group mr10" hidden="hidden">
                                        <div class="btn btn-white" id="btnReiniciarFiltros">
                                            <a onclick="reiniciarFiltros();">
                                                <span>Reiniciar Filtros</span>
                                            </a>
                                        </div>
                                    </div>

                                    <div class="btn-group mr10">
                                        <div class="btn btn-white" id="btnDocumentosRaiz">
                                            <a onclick="documentosRaiz();">
                                                <span>Documentos Raiz</span>
                                            </a>
                                        </div>
                                    </div>

                                    <div class="btn-group mr10">
                                        <div class="btn btn-white tooltips">
                                            <a id="divIconoDescargar" href="javascript:exportar();">
                                                <i class="glyphicon glyphicon-save"></i>&nbsp;Exportar
                                            </a>
                                            <img alt="" src="/images/loaders/loader1.gif" id="imgLoading" style="display: none" />
                                            <a href="" id="lnkDownload" onclick="resetearExportacion();" download="Comprobantes" style="display: none">Descargar</a>
                                        </div>
                                    </div>

                                    <div class="btn-group mr10" id="divPagination" style="display: none">
                                        <a class="btn btn-white" id="lnkPrevPage" style="cursor: pointer" onclick="mostrarPagAnterior();"><i class="glyphicon glyphicon-chevron-left"></i>Anterior</a>
                                        <a class="btn btn-white" id="lnkNextPage" style="cursor: pointer" onclick="mostrarPagProxima();">Siguiente <i class="glyphicon glyphicon-chevron-right"></i></a>
                                    </div>
                                </div>

                                <h4 class="panel-title" style="clear: left; padding-left: 20px">Resultados</h4>
                                <p id="msjResultados" style="padding-left: 20px"></p>
                            </div>
                        </div>
                        <!-- panel-heading -->
                        <div class="panel-body">
                            <div class="alert alert-danger" id="divError" style="display: none">
                                <button type="button" class="close" data-dismiss="alert" aria-hidden="true">×</button>
                                <strong>Lo sentimos!</strong> <span id="msgError"></span>
                            </div>
                            <div class="alert alert-success" id="divOk" style="display: none">
                                <strong>Bien hecho! </strong><span id="msgOk"></span>
                           </div>

                            <div class="table-responsive">
                                <table class="table mb30">
                                    <thead>
                                        <tr>
                                            <th>Tipo</th>
                                            <th>Número</th>
                                            <th>Proveedor/Cliente</th>
                                            <th>Nombre</th>
                                            <th>Fecha</th>
                                            <th>Modo</th>
                                            <th>Imp. Neto Grav</th>
                                            <th>Total Fact.</th>

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

        <script id="resultTemplate" type="text/x-jQuery-tmpl">
            {{each results}}
            <tr>
                <td>${Tipo}</td>
                <td style="min-width: 130px">${Numero}</td>
                <td>${RazonSocial}</td>
                <td>${Nombre}</td>
                <td>${Fecha}</td>
                <td>${Modo}</td>
                <td>${ImporteTotalBruto}</td>
                <td>${ImporteTotalNeto}</td>
                <td class="table-action">{{if $value.PuedeAdm == "T"}}
                    <a onclick="documentosVinculados(${ID});" style="cursor: pointer; font-size: 16px" title="Comprobantes Vinculados"><i class="fa fa-link"></i></a>
                    <a onclick="editar(${ID});" style="cursor: pointer; font-size: 16px" title="Editar"><i class="fa fa-pencil"></i></a>
                    <a onclick="eliminar(${ID},'${RazonSocial}');" style="cursor: pointer; font-size: 16px" class="delete-row" title="Eliminar"><i class="fa fa-trash-o"></i></a>
                    {{/if}} 
                    {{if $value.PuedeAdm == "F"}}
                        <a onclick="editar(${ID});" style="cursor: pointer; font-size: 16px" title="Ver factura"><i class="fa fa-search"></i></a>
                        <a onclick="mostrarEnvioDesdeListado(${ID});" style="cursor: pointer; font-size: 16px" title="Enviar correo electrónico"><i class="fa fa-envelope"></i></a>
                    {{/if}} 
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
            <h2 id="hTitulo">Aún no has creado ningún comprobante</h2>
            <br />
            <a class="btn btn-warning" onclick="nuevo();" id="btnNuevoSinDatos">Crea un comprobante</a>
        </div>
    </div>

    <div class="modal modal fade" id="modalComprobantesVinculados" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true" style="display: none">
        <div class="modal-dialog">
            <div class="modal-content" style="position:fixed">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Comprobantes vinculados</h4>
                </div>
                <div class="modal-body">
                    <div>
                        <div class="modal-header">
                            <h4 class="modal-title">Documento Raíz</h4>
                        </div>

                        <div class="alert alert-danger" id="divErrorDocumentoRaiz" style="display: none">
                            <strong>Lo sentimos! </strong><span id="msgErrorDocumentoRaiz"></span>
                        </div>

                        <div class="table-responsive">
                            <table class="table mb30">
                                <thead>
                                    <tr>
                                        <!--th>#</!--th-->
                                        <th>Número</th>
                                        <th>Proveedor/Cliente</th>
                                        <th>Fecha</th>
                                        <th>Tipo</th>
                                        <th>Imp. Neto</th>
                                        <th>IVA</th>
                                        <%--<th>Modo</th>--%>
                                        <th>Total</th>
                                    </tr>
                                </thead>
                                <tbody id="resultsContainerDocumentoRaiz">
                                </tbody>
                            </table>
                        </div>

                        <script id="resultTemplateDocumentoRaiz" type="text/x-jQuery-tmpl">
                            {{each results}}
                        <tr>
                            <!--td>${ID}</td-->
                            <td>${Numero}</td>
                            <td>${RazonSocial}</td>
                            <td>${Fecha}</td>
                            <td>${Tipo}</td>
                            <td>${ImporteTotalBruto}</td>
                            <td>${IVA} </td>
                            <%--<td>${Modo}</td>--%>
                            <td>${ImporteTotalNeto}</td>
                        </tr>
                            {{/each}}
                        </script>

                        <script id="noResultTemplateDocumentoRaiz" type="text/x-jQuery-tmpl">
                            <tr>
                                <td colspan="8">No se han encontrado resultados
                                </td>
                            </tr>
                        </script>
                    </div>
                    <hr />                   
                    <div class="row">
                        <div class="form-group" style="text-align:right;margin-right:20px">
                            <span id="msgTotalPendiente" style="font-size:small;font-weight:bold"></span>
                        </div>                        
                    </div>                  
                    <hr />
                    <div>
                        <div class="modal-header">
                            <h4 class="modal-title">Comprobante dependiente</h4>
                        </div>

                        <div class="alert alert-danger" id="divErrorComprobantesVinculados" style="display: none">
                            <strong>Lo sentimos! </strong><span id="msgErrorComprobantesVinculados"></span>
                        </div>

                        <div class="table-responsive">
                            <table class="table mb30">
                                <thead>
                                    <tr>
                                        <th>Número</th>                                        
                                        <th>Proveedor/Cliente</th>
                                        <th>Fecha</th>
                                        <th>Tipo</th>
                                        <th>Modo</th>
                                        <th>CAE</th>
                                        <th>Imp. Neto Grav</th>
                                        <th>IVA</th>
                                        <th>Total Fact.</th>

                                        <th class="columnIcons"></th>
                                    </tr>
                                </thead>
                                <tbody id="resultsContainerComprobantesVinculados">
                                </tbody>
                            </table>
                        </div>

                        <script id="resultTemplateComprobantesVinculados" type="text/x-jQuery-tmpl">
                            {{each results}}
                            <tr>
                                <td style="min-width: 130px">${Numero}</td>
                                <td>${RazonSocial}</td>
                                <td>${Fecha}</td>
                                <td>${Tipo}</td>
                                <td>${Modo}</td>
                                <td>${CAE}</td>
                                <td>${ImporteTotalBruto}</td>
                                <td>${IVA}</td>
                                <td>${ImporteTotalNeto}</td>
                                <td class="table-action">{{if $value.PuedeAdm == "T"}}
                                    <a onclick="editar(${ID});" style="cursor: pointer; font-size: 16px" title="Editar"><i class="fa fa-pencil"></i></a>
                                    <a onclick="eliminar(${ID},'${RazonSocial}');" style="cursor: pointer; font-size: 16px" class="delete-row" title="Eliminar"><i class="fa fa-trash-o"></i></a>
                                    {{/if}} 
                                    {{if $value.PuedeAdm == "F"}}
                                    <a onclick="editar(${ID});" style="cursor: pointer; font-size: 16px" title="Ver factura"><i class="fa fa-search"></i></a>
                                    {{/if}} 
                                </td>
                            </tr>
                            {{/each}}
                        </script>

                        <script id="noResultTemplateComprobantesVinculados" type="text/x-jQuery-tmpl">
                            <tr>
                                <td colspan="8">No se han encontrado resultados
                                </td>
                            </tr>
                        </script>
                    </div>
                    <hr />
                    <div>
                        <div class="modal-header">
                            <h4 class="modal-title">Entregas</h4>
                        </div>

                        <div class="alert alert-danger" id="divErrorEntregasVinculadas" style="display: none">
                            <strong>Lo sentimos! </strong><span id="msgErrorEntregasVinculadas"></span>
                        </div>

                        <div class="table-responsive">
                            <table class="table mb30">
                                <thead>
                                    <tr>
                                        <th>Número</th>                                        
                                        <th>Proveedor/Cliente</th>
                                        <th>Fecha</th>
                                        <th>Tipo</th>
                                        <th>Modo</th>
                                        <th>CAE</th>
                                        <th>Imp. Neto Grav</th>
                                        <th>IVA</th>
                                        <th>Total Fact.</th>
                                        <th class="columnIcons"></th>
                                    </tr>
                                </thead>
                                <tbody id="resultsContainerEntregasVinculadas">
                                </tbody>
                            </table>
                        </div>

                        <script id="resultTemplateEntregasVinculadas" type="text/x-jQuery-tmpl">
                            {{each results}}
                        <tr>
                            <td style="min-width: 130px">${Numero}</td>
                            <td>${RazonSocial}</td>
                            <td>${Fecha}</td>
                            <td>${Tipo}</td>
                            <td>${Modo}</td>
                            <td>${CAE}</td>
                            <td>${ImporteTotalBruto}</td>
                            <td>${IVA}</td>
                            <td>${ImporteTotalNeto}</td>
                            <td class="table-action">
                                <a onclick="editar(${ID});" style="cursor: pointer; font-size: 16px" title="Editar"><i class="fa fa-pencil"></i></a>
                            </td>
                        </tr>
                            {{/each}}
                        </script>

                        <script id="noResultTemplateEntregasVinculadas" type="text/x-jQuery-tmpl">
                            <tr>
                                <td colspan="8">No se han encontrado resultados
                                </td>
                            </tr>
                        </script>
                    </div>
                    <hr />
                    <div>
                        <div class="modal-header">
                            <h4 class="modal-title">Cobranza Dependiente</h4>
                        </div>

                        <div class="alert alert-danger" id="divErrorCobranzasVinculadas" style="display: none">
                            <strong>Lo sentimos! </strong><span id="msgErrorCobranzasVinculadas"></span>
                        </div>

                        <div class="table-responsive">
                            <table class="table mb30">
                                <thead>
                                    <tr>
                                        <!--th>#</!--th-->
                                        <th>Número</th>
                                        <th>ComprobanteOrigen</th>
                                        <th>Proveedor/Cliente</th>
                                        <th>Fecha</th>
                                        <%--<th>Tipo</th>
                                        <th>Modo</th>--%>
                                        <th>Total</th>
                                        <th class="columnIcons"></th>
                                    </tr>
                                </thead>
                                <tbody id="resultsContainerCobranzasVinculadas">
                                </tbody>
                            </table>
                        </div>

                        <script id="resultTemplateCobranzasVinculadas" type="text/x-jQuery-tmpl">
                            {{each results}}
                        <tr>
                            <!--td>${ID}</td-->
                            <td>${Numero}</td>
                            <td>${ComprobanteOrigen}</td>
                            <td>${RazonSocial}</td>
                            <td>${Fecha}</td>
                            <!--td>
                                ${Tipo}
                            </!--td>
                            <td>
                                ${Modo}
                            </td -->
                            <td>${ImporteTotalNeto}</td>
                            <td class="table-action">
                                <a onclick="editarCobranza(${ID});" style="cursor: pointer; font-size: 16px" title="Editar"><i class="fa fa-pencil"></i></a>
                            </td>
                        </tr>
                            {{/each}}
                        </script>

                        <script id="noResultTemplateCobranzasVinculadas" type="text/x-jQuery-tmpl">
                            <tr>
                                <td colspan="8">No se han encontrado resultados
                                </td>
                            </tr>
                        </script>
                    </div>
                    <hr />
                    <div>
                        <div class="modal-header">
                            <h4 class="modal-title">Cheques vinculados</h4>
                        </div>

                        <div class="alert alert-danger" id="divErrorChequesVinculados" style="display: none">
                            <strong>Lo sentimos! </strong><span id="msgErrorChequesVinculados"></span>
                        </div>

                        <div class="table-responsive">
                            <table class="table mb30">
                                <thead>
                                    <tr>
                                        <!--th>#</!--th-->
                                        <th>NúmeroCobranza</th>
                                        <th>NúmeroCheque</th>
                                        <th>FechaEmisión</th>
                                        <th>Importe</th>
                                    </tr>
                                </thead>
                                <tbody id="resultsContainerChequesVinculados">
                                </tbody>
                            </table>
                        </div>

                        <script id="resultTemplateChequesVinculados" type="text/x-jQuery-tmpl">
                            {{each results}}
                        <tr>
                            <!--td>${ID}</td-->
                            <td>
                                <a onclick="editarCobranza(${ID});" style="cursor: pointer; font-size: 16px" title="${ComprobanteOrigen}">${ComprobanteOrigen}</a>
                            </td>
                            <td>${Numero}</td>
                            <td>${Fecha}</td>
                            <td>${ImporteTotalNeto}</td>
                        </tr>
                            {{/each}}
                        </script>

                        <script id="noResultTemplateChequesVinculados" type="text/x-jQuery-tmpl">
                            <tr>
                                <td colspan="8">No se han encontrado resultados
                                </td>
                            </tr>
                        </script>
                    </div>
                </div>                
                <hr />
                <div class="modal-footer">
                    <a style="margin-left: 20px" href="#" type="button" data-dismiss="modal">Cerrar</a>
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal fade" id="modalDocumentosRaiz" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true" style="display: none">
        <div class="modal-dialog">
            <div class="modal-content" style="position: fixed">
                <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                        <h4 class="modal-title">Documentos Raiz</h4>                              
                </div>
                <div class="modal-body">
                    <div>
                        <div class="alert alert-danger" id="divErrorDocumentosRaiz" style="display: none">
                            <strong>Lo sentimos! </strong><span id="msgErrorDocumentosRaiz"></span>
                        </div>

                        <div class="table-responsive">
                            <table class="table mb30">
                                <thead>
                                    <tr>
                                        <!--th>#</!--th-->
                                        <th>Número</th>
                                        <th>Proveedor/Cliente</th>
                                        <th>Fecha</th>
                                        <th>Tipo</th>
                                        <%--<th>Modo</th>--%>
                                        <th>Total</th>
                                    </tr>
                                </thead>
                                <tbody id="resultsContainerDocumentosRaiz">
                                </tbody>
                            </table>
                        </div>

                        <script id="resultTemplateDocumentosRaiz" type="text/x-jQuery-tmpl">
                            {{each results}}
                    <tr>
                        <!--td>${ID}</td-->
                        <td>${Numero}</td>
                        <td>${RazonSocial}</td>
                        <td>${Fecha}</td>
                        <td>${Tipo}</td>
                        <%--<td>${Modo}</td>--%>
                        <td>${ImporteTotalNeto}</td>
                    </tr>
                            {{/each}}
                        </script>

                        <script id="noResultTemplateDocumentosRaiz" type="text/x-jQuery-tmpl">
                            <tr>
                                <td colspan="8">No se han encontrado resultados
                                </td>
                            </tr>
                        </script>
                    </div>
                    <hr />                   
                    <div class="row">
                        <div class="col-sm-6 col-md-6">
                            <div class="form-group" style="text-align:left">
                                <div class="btn btn-white tooltips" >
                                    <a id="divIconoDescargarDocumentosRaiz" href="javascript:exportarDocumentosRaiz();">
                                        <i class="glyphicon glyphicon-save"></i>&nbsp;Exportar
                                    </a>
                                    <img alt="" src="/images/loaders/loader1.gif" id="imgLoadingDocumentosRaiz" style="display: none" />
                                    <a href="" id="lnkDownloadDocumentosRaiz" onclick="resetearExportacionDocumentosRaiz();" download="ComprobantesDocumentosRaiz" style="display: none">Descargar</a>
                                </div>
                            </div>
                        </div>
                        <div class="col-sm-6 col-md-6">
                            <div class="form-group" style="text-align:right">
                                <div>
                                    <h4 class="modal-title" ><span id="msgTotalDocumentosRaiz"></span></h4>
                                </div>
                            </div>
                        </div>
                    </div>                    
                </div>
                <div class="modal-footer">                   
                    <a style="margin-left: 20px" href="#" type="button" data-dismiss="modal">Cerrar</a>
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal fade" id="modalMasFiltros" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true" style="display: none">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                        <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                        <h4 class="modal-title">Más filtros</h4>                              
                </div>
                <div class="modal-body">
                    <div class="alert alert-danger" id="divErrorMasFiltros" style="display: none">
                        <strong>Lo sentimos! </strong><span id="msgErrorMasFiltros"></span>
                    </div>
                    <div style="padding-left:5%">
                        <div id="divEntregas">
                            <h4>Entregas</h4>
                            <div class="row" id="divEntregaPendiente">
                                <label><input type="checkbox" id="chkEntregaPendiente" /><span>&nbsp;Mostrar solo PDV con Entregas Pendientes</span></label>
                            </div> 
                            <div class="row" id="divEntregaEstado">
                                <label><input type="checkbox" onchange="changeComboEntrega();" id="chkEntregaEstado" /><span>&nbsp;Mostrar solo Entregas en cierto estado</span></label>
                                <div id="divEstado" style="display: none">
                                    <div class="row mb15">
                                        <div class="col-sm-6 col-md-6">
                                            <div class="form-group">
                                                <select class="form-control required" id="ddlEstado">
                                                    <option value="Iniciado" selected="selected">Iniciado</option>
                                                    <option value="En curso">En curso</option>
                                                    <option value="Terminado">Terminado</option>
                                                    <option value="Cerrado">Cerrado</option>
                                                    <option value="Cancelado">Cancelado</option>
                                                    <option value="Suspendido">Suspendido</option>
                                                </select>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div id="divCobranzas">
                            <h4>Cobranzas</h4>
                            <div class="row" id="divCobranzaPendiente">
                                <label><input type="checkbox" id="chkCobranzaPendiente" /><span>&nbsp;Mostrar solo PDV con cobranzas pendientes</span></label>
                            </div> 
                        </div>
                        <div id="divFacturas">
                            <h4>Facturas</h4>
                            <div class="row" id="divFacturacionPendiente">
                                <label><input type="checkbox" id="chkFacturacionPendiente" /><span>&nbsp;Mostrar solo PDV con facturas pendientes</span></label>
                            </div>
                        </div> 
                    </div>                              
                </div>
                <div class="modal-footer">      
                    <a style="margin-left: 20px" href="#" type="button" data-dismiss="modal"  onclick="otroPeriodo();">Cerrar</a>
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal fade" id="modalEMail" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true" data-backdrop="static" data-keyboard="false">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title" id="litModalOkTitulo">Enviar email </h4>
                </div>
                <div class="modal-body" style="min-height: 200px;">
                    <div class="alert alert-success" id="divOkMail" style="display: none">
                        <strong>Bien hecho! </strong>El mensaje ha sido enviado correctamente
                    </div>
                    <div id="divSendEmail">
                        <div class="row">
                            <div class="col-sm-12">
                                <div class="alert alert-danger" id="divErrorMail" style="display: none">
                                    <strong>Lo sentimos! </strong><span id="msgErrorMail"></span>
                                </div>

                                <form id="frmSendMail">
                                    <div class="form-group">
                                        <p><span class="asterisk">*</span> Para: <small>(separa las direcciones mediante una coma) </small></p>
                                        <input id="txtEnvioPara" class="form-control required multiemails" type="text" runat="server" />
                                        <%--<span id="msgErrorEnvioPara" class="help-block" style="display: none">Una de las direcciones ingresadas es inválida.</span>--%>
                                    </div>
                                    <div class="form-group">
                                        <p><span class="asterisk">*</span> Asunto: </p>
                                        <input id="txtEnvioAsunto" class="form-control required" type="text" maxlength="150" runat="server" />
                                        <span id="msgErrorEnvioAsunto" class="help-block" style="display: none">Este campo es obligatorio.</span>
                                    </div>
                                    <div class="form-group">
                                        <p><span class="asterisk">*</span> Mensaje: </p>
                                        <textarea rows="5" id="txtEnvioMensaje" class="form-control required" runat="server"></textarea>
                                        <span id="msgErrorEnvioMensaje" class="help-block" style="display: none">Este campo es obligatorio.</span>
                                    </div>
                                    <input type="hidden" id="hdnFile" />
                                    <input type="hidden" id="hdnRazonSocial" />
                                    <br />
                                    <a type="button" class="btn btn-success" onclick="Toolbar.enviarComprobantePorMail();" id="btnEnviar">Enviar</a>
                                    <a style="margin-left: 20px" href="#" onclick="Toolbar.cancelarEnvio();">Cancelar</a>
                                </form>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <a style="margin-left: 20px" href="#" data-dismiss="modal">Cerrar</a>
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal-wide fade" id="modalCuadroResumen" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
        <div class="modal-dialog" style="display:contents">
            <div class="modal-content" >
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title" id="titDetalle" style="display: inline-table;"></h4>

                    <div class="btn-group" style="margin-left: 20px">
                        <div class="btn btn-white tooltips">

                            <a id="divIconoDescargarCuadroResumen" onclick="exportarCuadroResumen();">
                                <i class="glyphicon glyphicon-save"></i>&nbsp;Exportar
                            </a>
                            <img alt="" src="/images/loaders/loader1.gif" id="imgLoadingCuadroResumen" style="display: none" />
                            <a href="" id="lnkCuadroResumenDownload" onclick="resetearExportacionCuadroResumen();" download="CuadroResumen" style="display: none">Descargar</a>
                        </div>
                        <div class="btn btn-white tooltips">
                            <a id="divIconoDescargarCuadroResumenOriginal" onclick="exportarCuadroResumenOriginal();">
                                <i class="glyphicon glyphicon-save"></i>&nbsp;Exportar Original
                            </a>
                            <img alt="" src="/images/loaders/loader1.gif" id="imgLoadingCuadroResumenOriginal" style="display: none" />
                            <a href="" id="lnkCuadroResumenDownloadOriginal" onclick="resetearExportacionCuadroResumenOriginal();" download="CuadroResumenOriginal" style="display: none">Descargar Original</a>
                        </div>
                    </div>
                </div>
                <div class="modal-body">
                    <div class="alert alert-danger" id="divErrorCuadroResumen" style="display: none">
                        <strong>Lo sentimos! </strong><span id="msgErrorCuadroResumen"></span>
                    </div>
                     <div class="col-sm-2">
                        <div class="form-group">
                            <label class="control-label">Periodo</label>
                            <select id="ddlCuadroResumenPeriodo" class="form-control required" onchange="cuadroResumenPeriodo(false);">
                                <option value="2301">2301</option>
                                <option value="2302">2302</option>
                                <option value="2303">2303</option>
                                <option value="2304">2304</option>
                                <option value="2305">2305</option>
                                <option value="2306">2306</option>
                                <option value="2307">2307</option>
                                <option value="2308">2308</option>
                                <option value="2309">2309</option>
                                <option value="2310">2310</option>
                                <option value="2311">2311</option>
                                <option value="2312">2312</option>
                            </select>
                        </div>
                     </div>

                    <div class="table-responsive">
                        <table class="table table-success mb30 smallTable" id="tablaCuadroResumen">
                            <thead>
                                <tr>
                                    <th hidden="hidden">IdUsuario</th>
                                    <th hidden="hidden">IdPersona</th>
                                    <th>Razon Social</th>
                                    <th>Total</th>
                                    <th>Ventas</th>
                                    <th>Con Entrega</th>
                                    <th>Sin Entrega</th>
                                    <th>Factura</th>
                                    <th>Con CAE</th>
                                    <th>Sin CAE</th>
                                    <th>IVA</th>
                                    <th>Cobros</th>
                                    <th>Saldo</th>
                                    <th>Detalle</th>
                                </tr>
                            </thead>
                            <tbody id="resultsCuadroResumen">
                            </tbody>
                        </table>
                    </div>
                </div>
                <input type="hidden" id="hdnCuadroResumenPeriodo" />
                <input type="hidden" id="hdnCuadroResumenPersona" />
                <div class="modal-footer">
                    <a style="margin-left:20px" href="#" type="button" data-dismiss="modal">Cerrar</a>
                </div>
            </div>
        </div>
    </div>

    <div id="divEspera" style="display: none;">
        <img src="images/loaders/cargando.gif" width="50%" height="50%" />
    </div>

</asp:Content>

<asp:Content ID="Content4" ContentPlaceHolderID="FooterContent" runat="Server">
    <script type="text/javascript" src="/js/jquery.tmpl.min.js"></script>
    <script src="/js/jquery.validate.min.js"></script>
    <script src="/js/select2.min.js"></script>
    <script src="js/jquery.blockUI.js?v=<%= ConfigurationManager.AppSettings["JS.Version"] %>"></script>
    <script src="/js/views/comprobantes.js?v=<%= ConfigurationManager.AppSettings["JS.Version"] %>"></script>

    <script>
        jQuery(document).ready(function () {
            titulos();
            if ($('#divConDatos').is(":visible")) {
                filtrar();
                configFilters();                              
            }
        });
    </script>
</asp:Content>
