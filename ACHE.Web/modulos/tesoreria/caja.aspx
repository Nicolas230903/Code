<%@ Page Title="" Language="C#" MasterPageFile="~/Front.master" AutoEventWireup="true" CodeFile="caja.aspx.cs" Inherits="modulos_Tesoreria_caja" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
    <style type="text/css">
        .modal.modal-wide .modal-dialog {
            width: 90%;
            max-width: 900px;
        }

        .modal-wide .modal-body {
            overflow-y: auto;
        }
    </style>
    <link href="/css/jasny-bootstrap.min.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="pageheader">
        <h2><i class='fa fa-university'></i>Caja<span>Administración</span></h2>
        <div class="breadcrumb-wrapper">
            <span class="label">Estás aquí:</span>
            <ol class="breadcrumb">
                <li><a href="/home.aspx">axanweb</a></li>
                <li class="active">Caja</li>
            </ol>
        </div>
    </div>
    <div id="divConDatos" runat="server">
        <div class="contentpanel">
            <div class="row">
                <div class="col-sm-12 col-md-12 table-results">
                    <div class="panel panel-default">
                        <div class="panel-heading">
                            <form id="frmSearch">
                                <input type="hidden" id="hdnTipo" runat="server" />
                                <input type="hidden" id="hdnSaldoTotalActual" runat="server" />
                                <input type="hidden" id="hdnPage" runat="server" value="1" />
                                <input type="hidden" id="hdnIDUsuario" runat="server" value="0" />

                                <div class="col-sm-12" style="padding-left: inherit">
                                    <div class="col-sm-8 col-md-5">
                                        <div class="row">
                                            <div class="col-sm-4 col-md-4">
                                                <select class="select2" data-placeholder="seleccione un tipo de movimiento" id="ddlTipoMovimiento" onchange="caja.filtrar();">
                                                    <option value=""></option>
                                                    <option value="Ingreso">Debe (Ingreso)</option>
                                                    <option value="Egreso">Haber (Egreso)</option>
                                                </select>
                                            </div>

                                            <div class="col-sm-4 col-md-4">
                                                <select class="select2" data-placeholder="Seleccione un periodo de tiempo..." id="ddlPeriodo" onchange="caja.otroPeriodo();">
                                                    <option value="30" >Últimos 30 dias</option>
                                                    <option value="15">Últimos 15 dias</option>
                                                    <option value="7">Últimos 7 dias</option>
                                                    <option value="1">Ayer</option>
                                                    <option value="0">Hoy</option>
                                                    <option value="-1">Otro período</option>
                                                    <option value="-2" selected="selected">Todos</option>
                                                </select>
                                            </div>
                                            <div class="col-sm-4 col-md-4">
                                                <select class="select2" data-placeholder="seleccione el medio de pago" id="ddlMedioDePago" onchange="caja.filtrar();">
                                                    <option value="Efectivo">Efectivo</option>
                                                    <option value="Depósito">Depósito</option>
                                                    <option value="Transferencia">Transferencia</option>
                                                    <option value="Nota de credito">Nota de credito</option>
                                                    <option value="Tarjeta de crédito">Tarjeta de crédito</option>
                                                    <option value="Tarjeta de débito">Tarjeta de débito</option>
                                                    <option value="Todos" selected="selected">Todos</option>                                                                                               
                                                </select>
                                            </div>
                                        </div>

                                        <div class="row mb20"></div>

                                        <div id="divMasFiltros" style="display: none">
                                            <div class="row">
                                                <div class="col-sm-6 col-md-6">
                                                    <div class="input-group">
                                                        <span class="input-group-addon"><i class="glyphicon glyphicon-calendar"></i></span>
                                                        <input runat="server" id="txtFechaDesde" class="form-control validDate greaterThan" placeholder="Fecha desde" maxlength="10" onchange="caja.filtrar();" />
                                                    </div>
                                                </div>

                                                <div class="col-sm-6 col-md-6">
                                                    <div class="input-group">
                                                        <span class="input-group-addon"><i class="glyphicon glyphicon-calendar"></i></span>
                                                        <input runat="server" id="txtFechaHasta" class="form-control validDate greaterThan" placeholder="Fecha hasta" maxlength="10" onchange="caja.filtrar();" />
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-sm-2 col-md-4">
                                        <div class="col-sm-12 col-md-12">
                                            <a class="btn btn-warning" onclick="caja.nuevo();">
                                                <i class="fa fa-plus"></i>&nbsp;Nuevo movimiento de caja
                                            </a>
                                            <a class="btn btn-success" onclick="caja.resetearPagina();caja.cerrarCajas();">Arqueo de caja</a>
                                        </div>
                                    </div>
                                    <div class="col-sm-2 col-md-3">
                                        <h4 class="panel-title" style="clear: left; padding-left: 20px" id="spTotalSinConsolidar"></h4>
                                    </div>
                                </div>
                            </form>

                            <div class="col-sm-12">
                                <hr />
                            </div>
                            <div class="row mb20"></div>
                            <div class="row">
                                <div class="pull-right">

                                    <div class="btn-group mr10">
                                        <a href="#" type="button" style="margin-left: 20px"  onclick="caja.imprimir();" class="btn btn-success">Imprimir</a>
                                    </div>

                                    <div class="btn-group mr10">
                                        <div class="btn btn-white tooltips">
                                            <a id="divIconoDescargar" href="javascript:caja.exportar();">
                                                <i class="glyphicon glyphicon-save"></i>&nbsp;Exportar
                                            </a>
                                            <img alt="" src="/images/loaders/loader1.gif" id="imgLoading" style="display: none" />
                                            <a href="" id="lnkDownload" onclick="caja.resetearExportacion();" download="ClientesProv" style="display: none">Descargar</a>
                                        </div>
                                    </div>

                                    <div class="btn-group mr10" id="divPagination" style="display: none">
                                        <a class="btn btn-white" id="lnkPrevPage" style="cursor: pointer" onclick="caja.mostrarPagAnterior();"><i class="glyphicon glyphicon-chevron-left"></i>Anterior</a>
                                        <a class="btn btn-white" id="lnkNextPage" style="cursor: pointer" onclick="caja.mostrarPagProxima();">Siguiente <i class="glyphicon glyphicon-chevron-right"></i></a>
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
                                <strong>Bien hecho! </strong>Arqueo de caja procesado correctamente.
                            </div>

                            <div class="table-responsive">
                                <table class="table mb30">
                                    <thead>
                                        <tr>
                                            <th>Fecha</th>
                                            <th>Concepto</th>
                                            <th>Medio de Pago</th>
                                            <th>Estado</th>
                                            <th>Observaciones</th>
                                            <th style="color: #2b9b8f">Debe</th>
                                            <th style="color: #bf315f">Haber</th>
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
        <script id="resultTemplate" type="text/x-jQuery-tmpl">
            {{each results}}
        <tr>
            <td class="bgTotal" colspan="9">${concepto}</td>
        </tr>
            {{each Conceptos}}
                    <tr>
                        <td>${Fecha}</td>
                        <td>${Concepto}</td>
                        <td>${MedioDePago}</td>
                        <td>${Estado}</td>
                        <td style="max-width: 400px">${Observaciones}</td>
                        <td style="color: #2b9b8f">${Ingreso}</td>
                        <td style="color: #bf315f">${Egreso}</td>
                        <td>${Saldo}</td>
                        <td class="table-action">
                            {{if Estado == "Cargado" && ID > 0}}
                                <a onclick="caja.editar(${ID});" style="cursor: pointer; font-size: 16px" title="Editar"><i class="fa fa-pencil"></i></a>
                                <a onclick="caja.eliminar(${ID},'${tipoMovimiento}');" style="cursor: pointer; font-size: 16px" class="delete-row" title="Eliminar"><i class="fa fa-trash-o"></i></a>
                            {{/if}}
                        </td>
                    </tr>
            {{/each}}
            {{/each}}

        </script>
        <script id="noResultTemplate" type="text/x-jQuery-tmpl">
            <tr>
                <td colspan="9">No se han encontrado resultados</td>
            </tr>
        </script>
    </div>
    <!-- MODAL -->
    <div class="modal modal-wide fade" id="modalNuevaCaja" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
        <div class="modal-dialog">
         
        </div>
    </div>

    <div id="divSinDatos" runat="server">
        <div class="panel-heading" style="background-color: white; height: 30%; width: 100%; border-radius: 4px; text-align: center;">
            <h4 id="hTitulo">Aún no has creado ninguna caja</h4>
            <br />
            <a class="btn btn-warning" onclick="caja.nuevo();" id="btnNuevoSinDatos">&nbsp Crea un movimiento</a>
        </div>
    </div>

    <div class="modal modal-wide fade" id="modalPdf" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true" >
        <div class="modal-dialog">
            <div class="modal-content" style="position:fixed">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Visualización del detalle de caja</h4>
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

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FooterContent" runat="Server">
    <script type="text/javascript" src="/js/jquery.tmpl.min.js"></script>
    <script src="/js/select2.min.js"></script>
    <script src="/js/jquery.validate.min.js"></script>
    <script src="/js/jquery.maskedinput.min.js"></script>
    <script src="/js/jquery.maskMoney.min.js"></script>

    <script src="/js/jquery.fileupload.js" type="text/javascript"></script>
    <script src="/js/jasny-bootstrap.min.js"></script>

    <%--<script src="/js/views/common.js"></script>--%>
    <script src="/js/views/tesoreria/caja.js?v=<%= ConfigurationManager.AppSettings["JS.Version"] %>"></script>
    <script>
        jQuery(document).ready(function () {
            if ($('#divConDatos').is(":visible")) {
                caja.filtrar();
            }
            caja.configFilters();
            //caja.configForm();
        });
    </script>
</asp:Content>
