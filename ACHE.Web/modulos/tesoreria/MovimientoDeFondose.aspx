<%@ Page Title="" Language="C#" MasterPageFile="~/Front.master" AutoEventWireup="true" CodeFile="MovimientoDeFondose.aspx.cs" Inherits="modulos_tesoreria_MovimientoDeFondose" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
    <style type="text/css">
        .modal.modal-wide .modal-dialog {
            width: 90%;
        }

        .modal-wide .modal-body {
            overflow-y: auto;
        }
    </style>
    <link href="/css/jasny-bootstrap.min.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="pageheader">
        <h2><i class='fa fa-university'></i>Movimiento de fondos</h2>
        <div class="breadcrumb-wrapper">
            <span class="label">Estás aquí:</span>
            <ol class="breadcrumb">
                <li><a href="/home.aspx">axanweb</a></li>
                <li><a href='/Modulos/Tesoreria/MovimientoDeFondos.aspx'>Movimiento de fondos</a></li>
                <li class="active">
                    <asp:Literal runat="server" ID="litPath"></asp:Literal></li>
            </ol>
        </div>
    </div>

    <div class="contentpanel">

        <div class="row mb15">
            <form id="frmEdicion" runat="server" class="col-sm-12">
                <div class="panel panel-default">
                    <div class="panel-body">

                        <div class="alert alert-danger" id="divError" style="display: none">
                            <strong>Lo sentimos! </strong><span id="msgError"></span>
                        </div>

                        <div class="alert alert-success" id="divOk" style="display: none">
                            <strong>Bien hecho! </strong>Los datos se han actualizado correctamente
                        </div>
                        <div class="row mb15">
                            <div class="col-sm-4">
                                <div class="form-group">
                                    <label class="control-label"><span class="asterisk">*</span> Fecha de movimiento</label>
                                    <asp:TextBox ID="txtFechaMovimiento" CssClass="form-control required" runat="server" ClientIDMode="Static" MaxLength="10" placeholder="dd/mm/yyyy"></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-sm-4">
                                <div class="form-group">
                                    <label class="control-label"><span class="asterisk">*</span> Cuenta origen</label>
                                    <asp:DropDownList runat="server" ID="ddlPlanDeCuentaOrigen" CssClass="select2 required" ClientIDMode="Static" data-placeholder="seleccione una cuenta de origen"></asp:DropDownList>
                                </div>
                            </div>
                            <div class="col-sm-4">
                                <div class="form-group">
                                    <label class="control-label"><span class="asterisk">*</span> Cuenta destino</label>
                                    <asp:DropDownList runat="server" ID="ddlPlanDeCuentaDestino" CssClass="select2 required" ClientIDMode="Static" data-placeholder="seleccione una cuenta de destino"></asp:DropDownList>
                                </div>
                            </div>
                        </div>
                        <div class="row mb15">
                            <div class="col-sm-4">
                                <div class="form-group">
                                    <label class="control-label"><span class="asterisk">*</span> Importe</label>
                                    <asp:TextBox ID="txtImporte" CssClass="form-control required" runat="server" ClientIDMode="Static" MaxLength="10"></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-sm-8">
                                <div class="form-group" id="divLogo" style="display: none">
                                    <label class="control-label">Si desea puede adjuntar una foto del comprobante</label>
                                    <div class="fileinput fileinput-new input-group" data-provides="fileinput">
                                        <div class="form-control" data-trigger="fileinput" style="height: 40px">
                                            <i class="glyphicon glyphicon-file fileinput-exists" id="iImgFileFoto"></i>
                                            <span class="fileinput-filename"></span>
                                        </div>
                                        <span class="input-group-addon btn btn-default btn-file">
                                            <span class="fileinput-new">
                                                <i class="glyphicon glyphicon-folder-open"></i>&nbsp;&nbsp;Seleccionar
                                            </span>
                                            <span class="fileinput-exists">
                                                <i class="glyphicon glyphicon-folder-open"></i>&nbsp;&nbsp;Modificar
                                            </span>
                                            <input id="flpArchivo" type="file" />
                                        </span>
                                        <a href="#" class="input-group-addon btn btn-default fileinput-exists" data-dismiss="fileinput">
                                            <i class="glyphicon glyphicon-ban-circle"></i>&nbsp;&nbsp;Remover
                                        </a>
                                    </div>
                                    <asp:HiddenField runat="server" ID="hdnFileName" Value="" />
                                    <div class="col-sm-12" id="divComprobante">
                                        <a href="" id="lnkComprobante" download="" runat="server">Descargar comprobante</a>
                                    </div>
                                </div>
                                <div class="col-sm-6" id="divAdjuntarFoto">
                                    <a class="btn btn-white btn-block" onclick="MovimientoDeFondos.showInputFoto();" style="margin-top:27px">Adjuntar foto</a>
                                </div>
                                <div class="col-sm-6" id="divEliminarFoto">
                                    <a class="btn btn-white btn-block" onclick="MovimientoDeFondos.eliminarFoto();" style="margin-top:27px">Eliminar foto</a>
                                </div>

                            </div>
                        </div>

                        <div class="row mb15">
                            <div class="col-sm-12">
                                <div class="form-group">
                                    <label class="control-label">Observaciones</label>
                                    <asp:TextBox runat="server" ID="txtObservaciones" CssClass="form-control" MaxLength="128" ClientIDMode="Static" TextMode="MultiLine"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="panel-footer">
                        <a class="btn btn-success" id="btnActualizar" onclick="MovimientoDeFondos.grabarsinImagen();">Aceptar</a>
                        <a href="#" onclick="MovimientoDeFondos.cancelar();" style="margin-left: 20px">Cancelar</a>
                    </div>
                </div>
                <asp:HiddenField runat="server" ID="hdnID" Value="0" />
                <asp:HiddenField runat="server" ID="hdnTieneFoto" Value="0" />
                <asp:HiddenField runat="server" ID="hdnSinCombioDeFoto" Value="0" />
            </form>
        </div>
    </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FooterContent" runat="Server">
    <script src="/js/select2.min.js"></script>
    <script src="/js/jquery.validate.min.js"></script>
    <script src="/js/jquery.maskedinput.min.js"></script>
    <script src="/js/jquery.maskMoney.min.js"></script>
    <script src="/js/views/tesoreria/MovimientoDeFondos.js?v=<%= ConfigurationManager.AppSettings["JS.Version"] %>"></script>
    <script src="/js/select2.min.js"></script>
    <script src="/js/jquery.fileupload.js" type="text/javascript"></script>
    <script src="/js/jasny-bootstrap.min.js"></script>
    <script src="/js/jquery.ui.widget.js" type="text/javascript"></script>
    <script src="/js/jquery.iframe-transport.js" type="text/javascript"></script>
    <script>
        jQuery(document).ready(function () {
            MovimientoDeFondos.configForm();
        });
    </script>

</asp:Content>
