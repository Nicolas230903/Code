<%@ Page Title="" Language="C#" MasterPageFile="~/Front.master" AutoEventWireup="true" CodeFile="bancose.aspx.cs" Inherits="modulos_Tesoreria_bancose" %>

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
        <h2><i class='fa fa-university'></i>Bancos</h2>
        <div class="breadcrumb-wrapper">
            <span class="label">Estás aquí:</span>
            <ol class="breadcrumb">
                <li><a href="/home.aspx">axanweb</a></li>
                <li><a href='/Modulos/Tesoreria/Bancos.aspx'>Bancos</a></li>
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
                                    <label class="control-label"><span class="asterisk">*</span> Nombre del Banco</label>
                                    <asp:DropDownList runat="server" ID="ddlBanco" CssClass="select2 required" ClientIDMode="Static" data-placeholder="seleccione un banco"></asp:DropDownList>
                                </div>
                            </div>
                            <div class="col-sm-4">
                                <div class="form-group">
                                    <label class="control-label">Número de Cuenta</label>
                                    <asp:TextBox runat="server" ID="txtNroCuenta" CssClass="form-control" MaxLength="128" ClientIDMode="Static"></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-sm-4">
                                <div class="form-group">
                                    <label class="control-label"><span class="asterisk">*</span> Activo</label>
                                    <asp:DropDownList runat="server" ID="ddlActivo" CssClass="form-control" ClientIDMode="Static">
                                        <asp:ListItem Text="SI" Value="1"></asp:ListItem>
                                        <asp:ListItem Text="NO" Value="0"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>

                        <div class="row mb15">
                            <div class="col-sm-4">
                                <div class="form-group">
                                    <label class="control-label">Ejecutivo</label>
                                    <asp:TextBox runat="server" ID="txtEjecutivo" CssClass="form-control" MaxLength="200" ClientIDMode="Static"></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-sm-4">
                                <div class="form-group">
                                    <label class="control-label"><span class="asterisk">*</span> Moneda</label>
                                    <asp:DropDownList runat="server" ID="ddlMoneda" CssClass="form-control required" ClientIDMode="Static">
                                        <asp:ListItem Text="Pesos Argentinos" Value="Pesos Argentinos"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="col-sm-4">
                                <div class="form-group">
                                    <label class="control-label">Saldo Inicial</label>
                                    <asp:TextBox runat="server" ID="txtsaldoInicial" CssClass="form-control" MaxLength="128" ClientIDMode="Static"></asp:TextBox>
                                </div>
                            </div>
                        </div>

                        <div class="row mb15">
                            <div class="col-sm-4">
                                <div class="form-group">
                                    <label class="control-label">Dirección</label>
                                    <asp:TextBox runat="server" ID="txtDireccion" CssClass="form-control" MaxLength="300" ClientIDMode="Static"></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-sm-4">
                                <div class="form-group">
                                    <label class="control-label">Telefono</label>
                                    <asp:TextBox runat="server" ID="txtTelefono" CssClass="form-control" MaxLength="20" ClientIDMode="Static" placeholder="+54911########"></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-sm-4">
                                <div class="form-group">
                                    <label class="control-label">Email</label>
                                    <asp:TextBox runat="server" ID="txtEmail" CssClass="form-control" MaxLength="200" ClientIDMode="Static"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                        <div class="row mb15">
                            <div class="col-sm-12">
                                <div class="form-group">
                                    <label class="control-label">Observación</label>
                                    <asp:TextBox runat="server" ID="txtObservacion" TextMode="MultiLine" CssClass="form-control" MaxLength="300" ClientIDMode="Static"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="panel-footer">
                        <a class="btn btn-success" id="btnActualizar" onclick="Bancos.grabar();">Aceptar</a>
                        <a href="#" onclick="Bancos.cancelar();" style="margin-left: 20px">Cancelar</a>
                    </div>
                </div>
                <asp:HiddenField runat="server" ID="hdnIDBanco" Value="0" />
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
    <script src="/js/views/tesoreria/bancos.js?v=<%= ConfigurationManager.AppSettings["JS.Version"] %>"></script>
    <script src="/js/select2.min.js"></script>
    <script>
        jQuery(document).ready(function () {
            Bancos.configForm();
        });
    </script>

</asp:Content>
