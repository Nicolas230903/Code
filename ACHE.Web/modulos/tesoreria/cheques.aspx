<%@ Page Title="" Language="C#" MasterPageFile="~/Front.master" AutoEventWireup="true" CodeFile="cheques.aspx.cs" Inherits="modulos_Tesoreria_cheques" %>

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
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="pageheader">
        <h2><i class='fa fa-credit-card'></i>Cheques <span>Administración</span></h2>
        <div class="breadcrumb-wrapper">
            <span class="label">Estás aquí:</span>
            <ol class="breadcrumb">
                <li><a href="/home.aspx">axanweb</a></li>
                <li class="active">Cheques</li>
            </ol>
        </div>
    </div>
    <div id="divConDatos" runat="server">

        <div class="contentpanel">
            <div class="row">
                <div class="col-sm-12 col-md-12 table-results">
                    <div class="panel panel-default">
                        <div class="panel-heading">
                            <input type="hidden" id="hdnTipo" runat="server" />
                            <input type="hidden" id="hdnPage" runat="server" value="1" />

                            <div class="col-sm-4 col-md-4">
                                <input type="text" class="form-control" id="txtCondicion" maxlength="128" placeholder="Ingresá el nro del cheque o el emisor del mismo" />
                            </div>

                            <div class="col-sm-2 col-md-2">
                                <select class="select2" data-placeholder="Seleccione el estado del cheque..." id="ddlvendicos" onchange="cheques.filtrar();">
                                    <option value="" selected="selected"></option>
                                    <option value="a vencer">A vencer</option>
                                    <option value="Rechazados">Rechazados</option>
                                    <option value="vencidos">Vencidos</option>
                                </select>
                            </div>

                            <div class="col-sm-6 col-md-6">
                                <a class="btn btn-warning hide" onclick="cheques.nuevo();" style="margin-top: -11px;">
                                    <i class="fa fa-plus"></i>&nbsp;Nuevo cheque
                                </a>

                                <div class="btn-group dropdown" id="btnOtrasAcciones" style="display: none">
                                    <button type="button" class="btn btn-btn-default"><i class="fa fa-list"></i>&nbsp;Acciones</button>
                                    <button class="btn btn-btn-default dropdown-toggle" data-toggle="dropdown"><span class="fa fa-caret-down"></span></button>
                                    <ul class="dropdown-menu">
                                        <li><a href="javascript:chequesAcciones.nuevo('Rechazado');">&nbsp;Rechazar cheque</a></li>
                                        <li><a href="javascript:chequesAcciones.nuevo('Depositado');">&nbsp;Depositar cheque</a></li>
                                        <li><a href="javascript:chequesAcciones.nuevo('Acreditado');">&nbsp;Acreditar cheque</a></li>
                                    </ul>
                                </div>
                            </div>
                            <div class="col-sm-12">
                                <hr />
                            </div>
                            <div class="row mb20"></div>
                            <div class="row">
                                <div class="pull-right">
                                    <div class="btn-group mr10">
                                        <div class="btn btn-white tooltips">
                                            <a id="divIconoDescargar" href="javascript:cheques.exportar();">
                                                <i class="glyphicon glyphicon-save"></i>&nbsp;Exportar
                                            </a>
                                            <img alt="" src="/images/loaders/loader1.gif" id="imgLoading" style="display: none" />
                                            <a href="" id="lnkDownload" onclick="cheques.resetearExportacion();" download="ClientesProv" style="display: none">Descargar</a>
                                        </div>
                                    </div>

                                    <div class="btn-group mr10" id="divPagination" style="display: none">
                                        <a class="btn btn-white" id="lnkPrevPage" style="cursor: pointer" onclick="cheques.mostrarPagAnterior();"><i class="glyphicon glyphicon-chevron-left"></i>Anterior</a>
                                        <a class="btn btn-white" id="lnkNextPage" style="cursor: pointer" onclick="cheques.mostrarPagProxima();">Siguiente <i class="glyphicon glyphicon-chevron-right"></i></a>
                                    </div>
                                </div>

                                <h4 class="panel-title" style="clear: left; padding-left: 20px">Resultados</h4>
                                <p id="msjResultados" style="padding-left: 20px"></p>
                            </div>
                        </div>

                        <div class="panel-body">

                            <div class="alert alert-danger" id="divError" style="display: none">
                                <button type="button" class="close" data-dismiss="alert" aria-hidden="true">×</button>
                                <strong>Lo sentimos!</strong> <span id="msgError"></span>
                            </div>

                            <div class="table-responsive">
                                <table class="table mb30">
                                    <thead>
                                        <tr>
                                            <th>Banco</th>
                                            <th>Nro. de Cheque</th>
                                            <th>Emisor</th>
                                            <th>Cliente</th>
                                            <th>Fecha de emisión</th>
                                            <th>Fecha de cobro</th>
                                            <th>Días para cobrar</th>
                                            <th>Importe</th>
                                            <%--  <th>Estado</th>--%>
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
            <td>${Banco}</td>
            <td>${Numero}</td>
            <td>${Emisor}</td>
            <td>${Cliente}</td>
            <td>${FechaEmision}</td>
            <td>${FechaCobro}</td>
            <td>${CantDiasVencimientos}</td>
            <td>${Importe}</td>
            <td class="table-action">
                <a onclick="cheques.editar(${ID});" style="cursor: pointer; font-size: 16px" title="Editar"><i class="fa fa-pencil"></i></a>
                <a onclick="cheques.eliminar(${ID},'${Numero}');" style="cursor: pointer; font-size: 16px" class="delete-row" title="Eliminar"><i class="fa fa-trash-o"></i></a>
                <a onclick="chequesAcciones.verDetalle(${ID});" style="cursor: pointer; font-size: 16px" title="Eliminar"><i class="fa fa-search"></i></a>
            </td>
        </tr>
            {{/each}}
        </script>

        <script id="noResultTemplate" type="text/x-jQuery-tmpl">
            <tr>
                <td colspan="5">No se han encontrado resultados
                </td>
            </tr>
        </script>
    </div>

    <!-- MODAL ALTA CHEQUES ACCIONES-->
    <div class="modal modal-wide fade" id="modalNuevaChequesAcciones" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
        <div class="modal-dialog">
            <div class="contentpanel">
                <form id="frmNuenaChequesAcciones" class="col-sm-12">
                    <div class="panel panel-default">
                        <div class="panel-body">

                            <div class="alert alert-danger" id="divErrorAlta" style="display: none">
                                <strong>Lo sentimos! </strong><span id="msgErrorAlta"></span>
                            </div>
                            <div class="alert alert-success" id="divOkAlta" style="display: none">
                                <strong>Bien hecho! </strong>Los datos se han actualizado correctamente
                            </div>

                            <div class="col-sm-12">

                                <div class="row mb15">
                                    <div class="col-sm-12">
                                        <h3 id="tituloChequeAccion"></h3>
                                        <label class="control-label" id="subTituloChequeAccion"></label>
                                        <hr />
                                    </div>
                                </div>

                                <div class="row mb15">
                                    <div class="col-sm-6">
                                        <div class="form-group">
                                            <label class="control-label"><span class="asterisk">*</span> Cheque</label>
                                            <select class="select2 required" data-placeholder="Seleccione un cheque..." id="ddlCheques">
                                                <option value=""></option>
                                            </select>
                                        </div>
                                    </div>
                                     <div class="col-sm-6">
                                        <div class="form-group">
                                            <label class="control-label"><span class="asterisk">*</span> <span id="spFechaAccion"> </span></label>
                                            <div class="input-group">
                                                <span class="input-group-addon"><i class="glyphicon glyphicon-calendar"></i></span>
                                                <input id="txtFechaAcciones" class="form-control required validDate" placeholder="dd/mm/yyyy" maxlength="10" onfocus="chequesAcciones.abrirCalendario();" />
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="row mb15 ">
                                    <div class="col-sm-6" id="divBancos" style="display:none">
                                        <div class="form-group">
                                            <label class="control-label"><span class="asterisk">*</span> Banco</label>
                                            <select class="select2 required" data-placeholder="Seleccione un Banco..." id="ddlBancos" runat="server">
                                                <option value=""></option>
                                            </select>
                                        </div>
                                    </div>
                                    <div class="col-sm-6 hide">
                                        <div class="form-group">
                                            <label class="control-label"><span class="asterisk">*</span> Seleccione un tipo de movimiento</label>
                                            <select class="select2 required" data-placeholder="Seleccione un tipo de movimiento..." id="ddlAcciones" disabled="disabled">
                                                <option value=""></option>
                                                <option value="Rechazado">Rechazado</option>
                                                <option value="Depositado">Depositado</option>
                                                <option value="Acreditado">Acreditado</option>
                                            </select>
                                        </div>
                                    </div>
                                </div>
                            </div>

                        </div>
                        <div class="panel-footer">
                            <a class="btn btn-success" id="btnActualizar" onclick="chequesAcciones.grabar();">Aceptar</a>
                            <a href="#" onclick="$('#modalNuevaChequesAcciones').modal('toggle');" tabindex="14" style="margin-left: 20px">Cancelar</a>
                        </div>
                    </div>
                    <input id="hdnID" type="hidden" value="0" />
                </form>
            </div>
        </div>
    </div>
    <!-- MODAL CONSULTA CHEQUES ACCIONES -->
    <div class="modal modal-wide fade" id="modalDetalleAcciones" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Historial de movimientos</h4>
                </div>
                <div class="modal-body">
                    <div>
                        <div class="alert alert-danger" id="divErrorCat" style="display: none">
                            <strong>Lo sentimos! </strong><span id="msgErrorCat"></span>
                        </div>
                        <div class="table-responsive">
                            <table class="table mb30">
                                <thead>
                                    <tr>
                                        <th>Movimiento</th>
                                        <th>Fecha</th>
                                        
                                        <%--<th class="columnIcons"></th>--%>
                                    </tr>
                                </thead>
                                <tbody id="resultsContainerAcciones">
                                </tbody>
                            </table>
                        </div>
                    </div>
                    <input type="hidden" id="hdnIDChequesAcciones" value="0" />
                </div>
                <div class="modal-footer">
                    <a style="margin-left: 20px" href="#" data-dismiss="modal">Cerrar</a>
                </div>
            </div>
        </div>
    </div>

    <script id="resultTemplateAcciones" type="text/x-jQuery-tmpl">
        {{each results}}
        <tr>
            <td>${Accion}</td>
            <td>${FechaADepositar}</td>
        </tr>
        {{/each}}
    </script>
    <script id="noResultTemplateAcciones" type="text/x-jQuery-tmpl">
        <tr>
            <td colspan="2">No se han encontrado resultados</td>
        </tr>
    </script>

    <div id="divSinDatos" runat="server">
        <div class="panel-heading" style="background-color: white; height: 30%; width: 100%; border-radius: 4px; text-align: center;">
            <h4 id="hTitulo">Aún no has creado ningún cheque</h4>
            <br />
        </div>
    </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FooterContent" runat="Server">
    <script type="text/javascript" src="/js/jquery.tmpl.min.js"></script>
    <script src="/js/select2.min.js"></script>
    <script src="/js/jquery.validate.min.js"></script>
    <script src="/js/views/tesoreria/cheques.js?v=<%= ConfigurationManager.AppSettings["JS.Version"] %>"></script>

    <script>
        jQuery(document).ready(function () {
            if ($('#divConDatos').is(":visible")) {
                cheques.filtrar();
                cheques.configFilters();
                chequesAcciones.configForm();
                $("#btnOtrasAcciones").show();
            }
        });
    </script>
</asp:Content>
