<%@ Page Title="" Language="C#" MasterPageFile="~/Front.master" AutoEventWireup="true" CodeFile="alertas.aspx.cs" Inherits="alertas" %>


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
        <h2><i class='fa fa-bell'></i>Alertas <span>Administración</span></h2>
        <div class="breadcrumb-wrapper">
            <span class="label">Estás aquí:</span>
            <ol class="breadcrumb">
                <li><a href="/home.aspx">axanweb</a></li>
                <li class="active">Alertas</li>
            </ol>
        </div>
    </div>
    <div id="divConDatos" runat="server">

        <div class="contentpanel">
            <div class="row">

                <div class="col-sm-4 col-md-3">
                    <a class="btn btn-warning btn-block" onclick="alertas.nuevo();">
                        <i class="glyphicon glyphicon-plus"></i>&nbsp;Nueva Alerta
                    </a>
                    <div class="mb20"></div>

                    <input type="hidden" id="hdnTipo" runat="server" />
                    <input type="hidden" id="hdnPage" runat="server" value="1" />

                    <h4 class="subtitle mb5">Filtros disponibles</h4>
                    <form runat="server" id="frmSearch">
                        <div class="mb20"></div>

                        <h4 class="subtitle mb5">Tipo de Alerta</h4>
                        <select class="form-control" id="ddlAvisoAlertas">
                            <option value="">Ver todos</option>
                            <option value="El pago a un proveedor es">El pago a un proveedor es</option>
                            <option value="El cobro a un cliente es">El cobro a un cliente es</option>
                        </select>

                        <div class="mb20"></div>
                        <a class="btn btn-black" onclick="alertas.resetearPagina();alertas.filtrar();">Buscar</a>
                        <a class="btn btn-default" onclick="alertas.resetearPagina();alertas.verTodos();">Ver todos</a>
                        <img alt="" src="/images/loaders/loader1.gif" id="imgLoading2" style="display: none" />
                        <br />
                        <asp:HiddenField runat="server" ID="hdnID" ClientIDMode="Static" />
                    </form>
                </div>


                <div class="col-sm-8 col-md-9 table-results">
                    <div class="panel panel-default">
                        <div class="panel-heading">
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
                                            <th>#</th>
                                            <th>Avisarme cuando</th>
                                            <th>Condición</th>
                                            <th>Importe</th>
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
            <td>${ID}</td>
            <td>${AvisoAlerta}</td>
            <td>${Condicion}</td>
            <td>${Importe}</td>

            <td class="table-action">
                <a onclick="alertas.editar(${ID});" style="cursor: pointer; font-size: 16px" title="Editar"><i class="fa fa-pencil"></i></a>
                <a onclick="alertas.eliminar(${ID},'${AvisoAlerta}');" style="cursor: pointer; font-size: 16px" class="delete-row" title="Eliminar"><i class="fa fa-trash-o"></i></a>
            </td>
        </tr>
            {{/each}}
        </script>

        <script id="noResultTemplate" type="text/x-jQuery-tmpl">
            <tr>
                <td colspan="8">No se han encontrado resultados</td>
            </tr>
        </script>
    </div>


    <!--Modal Alertas-->
    <div class="modal fade" id="modalAlertas" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title" id="titDetalleAlertas">Definición de alertas</h4>
                </div>
                <div class="modal-body">
                    <form id="frmEdicion">
                        <div class="alert alert-danger" id="divAyudaErrorAlertas" style="display: none">
                            <strong>Lo sentimos! </strong><span id="msgAyudaErrorAlertas"></span>
                        </div>
                        <div class="row">
                            <h4 class="subtitle" style="margin-left: 10px;">Avisarme cuando:</h4>
                            <div class="col-md-6">
                                <div class="form-group">
                                    <select class="form-control required" id="ddlAvisoAlertasModal">
                                        <option value="El pago a un proveedor es">El pago a un proveedor es</option>
                                        <option value="El cobro a un cliente es">El cobro a un cliente es</option>
                                    </select>
                                </div>
                            </div>
                            <div class="col-md-6">
                                <div class="form-group">
                                    <select class="form-control required" id="ddlCondicionAlertas">
                                        <option value="Mayor o igual que">Mayor o igual que</option>
                                        <option value="Menor o igual que">Menor o igual que</option>
                                    </select>
                                </div>
                            </div>
                        </div>
                        <div class="mb20"></div>
                        <div class="row">
                            <h4 class="subtitle" style="margin-left: 10px;">Importe:</h4>

                            <div class="col-md-6">
                                <div class="form-group">
                                    <input type="text" class="form-control required" name="name" id="txtimporteAlerta" />
                                </div>
                            </div>
                            <div class="col-md-6"></div>
                        </div>
                    </form>

                </div>
                <div class="modal-footer">
                    <button type="button" class="btn btn-success" id="btnActualizar" onclick="alertas.guardarAlerta();">Aceptar</button>
                    <a style="margin-left:20px" href="#" data-dismiss="modal">Cerrar</a>
                    <img alt="" src="/images/loaders/loader1.gif" id="imgLoading" style="display: none" />
                </div>
            </div>
        </div>
    </div>

    <div id="divSinDatos" runat="server">
        <div class="panel-heading" style="background-color: white; height: 30%; width: 100%; border-radius: 4px; text-align: center;">
            <h2 id="hTitulo">Aún no has creado ninguna alerta</h2>
            <br />
            <a class="btn btn-warning" onclick="alertas.nuevo();" id="btnNuevoSinDatos">Crea una alerta</a>
        </div>
    </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FooterContent" runat="Server">
    <script type="text/javascript" src="/js/jquery.tmpl.min.js"></script>
    <script src="/js/jquery.validate.min.js"></script>
    <script src="/js/views/alertas.js?v=<%= ConfigurationManager.AppSettings["JS.Version"] %>"></script>
    <script src="/js/jquery.maskedinput.min.js"></script>
    <script src="/js/jquery.maskMoney.min.js"></script>
    <script>
        jQuery(document).ready(function () {
            if ($('#divConDatos').is(":visible")) {
                alertas.filtrar();
            }
        });
    </script>
</asp:Content>
