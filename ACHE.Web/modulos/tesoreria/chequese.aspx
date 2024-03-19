<%@ Page Title="" Language="C#" MasterPageFile="~/Front.master" AutoEventWireup="true" CodeFile="chequese.aspx.cs" Inherits="modulos_Tesoreria_chequese" %>

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
            <i class='fa fa-credit-card'></i>Cheques
        </h2>
        <div class="breadcrumb-wrapper">
            <span class="label">Estás aquí:</span>
            <ol class="breadcrumb">
                <li><a href="/home.aspx">axanweb</a></li>
                <li><a href='/Modulos/Tesoreria/cheques.aspx'>Cheques</a></li>
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
                            <div class="col-sm-3">

                                <img id="imgFoto" src="/files/usuarios/no-cheque.png" class="thumbnail img-responsive" alt="" runat="server" />
                                <div class="mb30"></div>

                                <div id="divLogo" style="display: none">
                                    <%--<p class="mb30">Formato JPG, PNG o GIF. Tamaño máximo recomendado: 100x70px</p>--%>
                                    <input type="hidden" value="" name="" /><input type="file" id="flpArchivo" />
                                    <div class="mb20"></div>
                                </div>

                                <div class="col-sm-6" id="divAdjuntarFoto">
                                    <a class="btn btn-white btn-block" onclick="cheques.showInputLogo();">Adjuntar foto</a>
                                </div>
                                <div class="col-sm-6" id="divEliminarFoto">
                                    <a class="btn btn-white btn-block" onclick="cheques.eliminarFotoCheque();">Eliminar foto</a>
                                </div>
                            </div>

                            <div class="col-sm-9">
                                <div class="row mb15">
                                    <div class="col-sm-4">
                                        <div class="form-group">
                                            <label class="control-label"><span class="asterisk">*</span> Nombre del Banco</label>
                                            <asp:DropDownList runat="server" ID="ddlBancos" CssClass="select2 required" ClientIDMode="Static">
                                            </asp:DropDownList>
                                        </div>
                                    </div>

                                    <div class="col-sm-4">
                                        <div class="form-group">
                                            <label class="control-label"><span class="asterisk">*</span> Nro. de Cheque</label>
                                            <asp:TextBox runat="server" ID="txtNumero" CssClass="form-control  required" MaxLength="128" ClientIDMode="Static"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-sm-4">
                                        <div class="form-group">
                                            <label class="control-label"><span class="asterisk">*</span> Importe</label>
                                            <asp:TextBox runat="server" ID="txtImporte" CssClass="form-control  required" MaxLength="128" ClientIDMode="Static"></asp:TextBox>
                                        </div>
                                    </div>

                                </div>

                                <div class="mb40"></div>

                                <div class="row mb15">

                                    <div class="col-sm-4">
                                        <div class="form-group">
                                            <label class="control-label">Emisor</label>
                                            <asp:TextBox runat="server" ID="txtEmisor" CssClass="form-control" MaxLength="128" ClientIDMode="Static"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-sm-4">
                                        <div class="form-group">
                                            <label class="control-label">CUIT</label>
                                            <asp:TextBox runat="server" ID="txtCUIT" TextMode="Number" CssClass="form-control" MaxLength="11" ClientIDMode="Static"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-sm-4">
                                        <div class="form-group">
                                            <label class="control-label">Cliente</label>
                                            <asp:DropDownList runat="server" ID="ddlPersona" CssClass="select2" ClientIDMode="Static">
                                            </asp:DropDownList>
                                        </div>
                                    </div>

                                    <div class="col-sm-4 hide">
                                        <div class="form-group">
                                            <label class="control-label"><span class="asterisk">*</span> Estado</label>
                                            <asp:DropDownList runat="server" ID="ddlEstado" CssClass="form-control" ClientIDMode="Static">
                                                <asp:ListItem Value="Libre" Text="Libre" />
                                                <asp:ListItem Value="Usado" Text="Usado" />
                                                <asp:ListItem Value="Depositado" Text="Depositado" />
                                                <asp:ListItem Value="Rechazado" Text="Rechazado" />
                                            </asp:DropDownList>
                                        </div>
                                    </div>

                                </div>
                                <div class="mb40"></div>

                                <div class="row mb15">

                                    <div class="col-sm-4">
                                        <div class="form-group">
                                            <label class="control-label"><span class="asterisk">*</span> Fecha emision</label>
                                            <asp:TextBox runat="server" ID="txtFechaEmision" CssClass="form-control required validDate greaterThan" placeholder="dd/mm/yyyy" MaxLength="10" ClientIDMode="Static"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="col-sm-4">
                                        <div class="form-group">
                                            <label class="control-label"><span class="asterisk">*</span> Fecha de cobro</label>
                                            <asp:TextBox runat="server" ID="txtFechaCobrar" CssClass="form-control required validDate greaterThan" placeholder="dd/mm/yyyy" MaxLength="10" ClientIDMode="Static"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="col-sm-4">
                                        <div class="form-group">
                                            <label class="control-label"><span class="asterisk">*</span> Fecha de vencimiento</label>
                                            <asp:TextBox runat="server" ID="txtFechaVencimiento" CssClass="form-control required" placeholder="dd/mm/yyyy" MaxLength="10" ClientIDMode="Static" disabled="true"></asp:TextBox>
                                        </div>
                                    </div>

                                </div>

                                <div class="row mb15">
                                    <div class="col-sm-12">
                                        <div class="form-group">
                                            <label class="control-label">Observaciones</label>
                                            <asp:TextBox runat="server" TextMode="MultiLine" ID="txtObservaciones" CssClass="form-control" Rows="5" ClientIDMode="Static"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>

                            </div>

                        </div>
                    </div>

                    <div class="panel-footer">
                        <a class="btn btn-success" id="actualizarCheque" onclick="cheques.grabarsinImagen();">Aceptar</a>
                        <a href="#" onclick="cheques.cancelar();" style="margin-left: 20px">Cancelar</a>
                    </div>

                </div>
                <asp:HiddenField runat="server" ID="hdnTieneFoto" Value="0" />
                <asp:HiddenField runat="server" ID="hdnSinCombioDeFoto" Value="0" />
                <asp:HiddenField runat="server" ID="Idusuario" Value="0" ClientIDMode="Static" />
                <asp:HiddenField runat="server" ID="hdnFileName" Value="" />
                <asp:HiddenField runat="server" ID="hdnID" Value="0" />
            </form>
        </div>
    </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FooterContent" runat="Server">
    <script src="/js/select2.min.js"></script>
    <script src="/js/jquery.validate.min.js"></script>
    <script src="/js/jquery.maskedinput.min.js"></script>
    <script src="/js/jquery.maskMoney.min.js"></script>
    <script src="/js/views/tesoreria/cheques.js?v=<%= ConfigurationManager.AppSettings["JS.Version"] %>"></script>
    <script src="/js/jquery.fileupload.js" type="text/javascript"></script>

    <script>
        jQuery(document).ready(function () {
            cheques.configForm();
        });
    </script>

</asp:Content>
