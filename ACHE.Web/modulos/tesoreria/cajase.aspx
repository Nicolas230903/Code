<%@ Page Title="" Language="C#" MasterPageFile="~/Front.master" AutoEventWireup="true" CodeFile="cajase.aspx.cs" Inherits="modulos_tesoreria_cajase" %>

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
        <h2><i class='fa fa-university'></i>Caja</h2>
        <div class="breadcrumb-wrapper">
            <span class="label">Estás aquí:</span>
            <ol class="breadcrumb">
                <li><a href="/home.aspx">axanweb</a></li>
                <li><a href='/modulos/tesoreria/Caja.aspx'>Caja</a></li>
                <li class="active">
                    <asp:Literal runat="server" ID="litPath"></asp:Literal></li>
            </ol>
        </div>
    </div>

    <div class="contentpanel">
        <form id="frmNuenaCaja" class="col-sm-12" runat="server">
            <div class="panel panel-default">
                <div class="panel-body">

                    <div class="alert alert-danger" id="divError" style="display: none">
                        <strong>Lo sentimos! </strong><span id="msgError"></span>
                    </div>

                    <div class="alert alert-success" id="divOk" style="display: none">
                        <strong>Bien hecho! </strong>Los datos se han actualizado correctamente
                    </div>

                    <div class="col-sm-12">
                        <div class="row mb15">
                            <div class="col-sm-4">
                                <div class="form-group">
                                    <label class="control-label">Fecha</label>
                                    <br />
                                    <input id="txtFecha" class="form-control validDate required" placeholder="dd/mm/yyyy" maxlength="10" runat="server" />
                                </div>
                            </div>
                            <div class="col-sm-4">
                                <div class="form-group">
                                    <label class="control-label"><span class="asterisk">*</span> Tipo de movimiento</label>
                                    <select id="txtTipoMovimiento" class="form-control required" runat="server">
                                        <option value="Ingreso" selected="selected">Debe (Ingreso)</option>
                                        <option value="Egreso">Haber (Egreso)</option>
                                    </select>
                                </div>
                            </div>
                            <div class="col-sm-4">
                                <div class="form-group">
                                    <label class="control-label"><span class="asterisk">*</span> Importe</label>
                                    <input id="txtImporte" class="form-control required" maxlength="128" runat="server" type="tel" />
                                </div>
                            </div>
                        </div>

                        <div class="mb40"></div>
                        <div class="row mb15">
                            <div class="col-sm-4">
                                <div class="form-group">
                                    <label class="control-label">Concepto</label>
                                    <span class="badge badge-success pull-right tooltips" data-toggle="tooltip" data-original-title="Haga click aquí para agregar/modificar/eliminar Conceptos" style="cursor: pointer" onclick="caja.obtenerConceptos();">administrar</span>
                                    <asp:DropDownList ID="ddlConcepto" runat="server" CssClass="select2" data-placeholder="Seleccione un concepto...">
                                    </asp:DropDownList>
                                    <span class="help-block"><small>Asigná una concepto para poder segmentar tus ingresos o egresos de la caja.</small></span>
                                </div>
                            </div>

                            <div class="col-sm-4">
                                <div class="form-group">
                                    <label class="control-label">Nro de ticket</label>
                                    <input id="txtTicket" class="form-control" maxlength="128" runat="server" />
                                </div>
                            </div>

                            <div class="col-sm-4">
                                <div class="form-group">
                                    <label class="control-label">Medio de pago</label>
                                    <select class="select2" id="ddlMedioPago" data-placeholder="Seleccione un medio de pago" runat="server">
                                        <option value=""></option>
                                        <option value="Efectivo" selected="selected">Efectivo</option>
                                        <option value="Cheque">Cheque</option>
                                        <option value="Cuenta corriente">Cuenta corriente</option>
                                        <option value="Tarjeta de debito">Tarjeta de debito</option>
                                        <option value="Tarjeta de credito">Tarjeta de credito</option>
                                        <option value="Ticket">Ticket</option>
                                        <option value="Transferencia">Transferencia</option>
                                        <option value="Otro">Otro</option>
                                    </select>
                                </div>
                            </div>
                        </div>
                        <div class="row mb15">

                            <div class="col-sm-8">
                                <div class="form-group" id="divLogo" style="display: none">
                                    <label class="control-label">Si desea puede adjuntar una foto del comprobante</label>
                                    <div class="fileinput fileinput-new input-group" data-provides="fileinput">
                                        <div class="form-control" data-trigger="fileinput" style="height: 40px">
                                            <i class="glyphicon glyphicon-file fileinput-exists" id="iImgFileFoto"></i>
                                            <span class="fileinput-filename"></span>
                                        </div>
                                        <span class="input-group-addon btn btn-default btn-file">
                                            <span id="divSeleccionar" class="fileinput-new">
                                                <i class="glyphicon glyphicon-folder-open"></i>&nbsp;&nbsp;Seleccionar
                                            </span>
                                            <span id="divModificar" class="fileinput-exists">
                                                <i class="glyphicon glyphicon-folder-open"></i>&nbsp;&nbsp;Modificar
                                            </span>
                                            <input id="flpArchivo" type="file" />
                                        </span>
                                        <a id="divEliminar" href="#" class="input-group-addon btn btn-default fileinput-exists" data-dismiss="fileinput">
                                            <i class="glyphicon glyphicon-ban-circle"></i>&nbsp;&nbsp;Remover
                                        </a>
                                    </div>
                                     <div class="col-sm-12" id="divDescarga">
                                            <asp:HyperLink runat="server" ID="lnkDescarga">Descargar imagen</asp:HyperLink>
                                        </div>
                                </div>
                                <div class="col-sm-6" id="divAdjuntarFoto">
                                    <a class="btn btn-white btn-block" onclick="foto.showInputFoto();">Adjuntar foto</a>
                                </div>
                                <div class="col-sm-6" id="divEliminarFoto">
                                    <a class="btn btn-white btn-block" onclick="foto.eliminarFoto();">Eliminar foto</a>
                                </div>
                                <asp:HiddenField runat="server" ID="hdnFileName" Value="" />
                                <asp:HiddenField runat="server" ID="hdnTieneFoto" Value="0" />
                                <asp:HiddenField runat="server" ID="hdnSinCombioDeFoto" Value="0" />
                            </div>
                        </div>
                        <div class="row mb15">
                            <div class="col-sm-8 col-md-4 divPlanDeCuentas">
                                <div class="form-group">
                                    <label class="control-label">Cuentas contables</label>
                                    <asp:DropDownList runat="server" ID="ddlPlanDeCuentas" CssClass="select2" data-placeholder="Seleccione una cuenta...">
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>
                        <div class="row mb15">
                            <div class="col-sm-12">
                                <div class="form-group">
                                    <label class="control-label">Observaciones</label>
                                    <textarea rows="5" id="txtObservaciones" class="form-control" runat="server"></textarea>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="panel-footer">
                    <a class="btn btn-success" id="btnActualizar" onclick="foto.grabarsinImagen();">Aceptar</a>
                    <a href="/modulos/tesoreria/caja.aspx" tabindex="14" style="margin-left: 20px">Cancelar</a>
                    <img alt="" src="/images/loaders/loader1.gif" id="img1" style="display: none" />
                </div>
            </div>
            <input id="hdnSaldoTotalActual" type="hidden" value="0" />
            <input id="hdnID" type="hidden" value="0" runat="server" />
            <input id="hdnUsaPlanCorporativo" type="hidden" value="0" runat="server" />
        </form>
    </div>

    <!-- Modal -->
    <div class="modal modal-wide fade" id="modalConceptos" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Administración de Conceptos</h4>
                </div>
                <div class="modal-body">
                    <div>
                        <div class="alert alert-danger" id="divErrorCat" style="display: none">
                            <strong>Lo sentimos! </strong><span id="msgErrorCat"></span>
                        </div>

                        <div class="col-sm-6">
                            <div class="form-group">
                                <input type="text" maxlength="50" id="txtNuevaCat" class="form-control" />
                            </div>
                        </div>
                        <div class="col-sm-2"><a class="btn btn-default" id="btnConcepto" onclick="caja.grabarConcepto();">Agregar</a></div>

                        <br />
                        <br />
                        <br />

                        <div class="table-responsive">
                            <table class="table mb30">
                                <thead>
                                    <tr>
                                        <th>Nombre</th>
                                        <th></th>
                                    </tr>
                                </thead>
                                <tbody id="bodyDetalle">
                                </tbody>
                            </table>
                        </div>
                    </div>
                    <input type="hidden" id="hdnIDConcepto" value="0" />
                </div>
                <div class="modal-footer">
                    <a style="margin-left: 20px" href="#" data-dismiss="modal">Cerrar</a>
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
            caja.configForm();
        });
    </script>

</asp:Content>


