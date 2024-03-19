<%@ Page Title="" Language="C#" MasterPageFile="~/Front.master" AutoEventWireup="true" CodeFile="activose.aspx.cs" Inherits="modulos_tesoreria_activose" %>

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
        <h2><i class='fa fa-archive'></i>Activos</h2>
        <div class="breadcrumb-wrapper">
            <span class="label">Estás aquí:</span>
            <ol class="breadcrumb">
                <li><a href="/home.aspx">axanweb</a></li>
                <li><a href='/modulos/tesoreria/gastosBancarios.aspx'>Activos</a></li>
                <li class="active"><asp:Literal runat="server" ID="litPath"></asp:Literal></li>
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

                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="control-label"><span class="asterisk">*</span> Clientes/Proveedores</label>
                                    <asp:DropDownList runat="server" ID="ddlPersona" CssClass="select2 required" data-placeholder="Seleccione un cliente/proveedor..." ClientIDMode="Static" onchange="activos.changePersona();">
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="col-sm-3">
                                <div class="form-group">
                                    <label class="control-label"><span class="asterisk">*</span> FC Compra</label>
                                    <asp:DropDownList runat="server" ID="ddlCompra" CssClass="form-control required" ClientIDMode="Static" onchange="activos.changeFechaCompra();">
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="col-sm-3">
                                <div class="form-group">
                                    <label class="control-label"><span class="asterisk">*</span> Número de serie</label>
                                    <asp:TextBox runat="server" ID="txtNroDeSerie" CssClass="form-control required" MaxLength="150" ClientIDMode="Static"></asp:TextBox>
                                </div>
                            </div>
                        </div>

                        <div class="row mb15">
                            <div class="col-sm-3">
                                <div class="form-group">
                                    <label class="control-label">Fecha de inicio de uso</label>
                                    <asp:TextBox runat="server" ID="txtFechaInicio" CssClass="form-control validDate" placeholder="dd/mm/yyyy" MaxLength="10" ClientIDMode="Static"></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-sm-3">
                                <div class="form-group">
                                    <label class="control-label">Fecha de compra</label>
                                    <asp:TextBox runat="server" ID="txtFechaCompra" CssClass="form-control validDate" placeholder="dd/mm/yyyy" MaxLength="10" ClientIDMode="Static"></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-sm-3">
                                <div class="form-group">
                                    <label class="control-label">Garantía</label>
                                    <asp:TextBox runat="server" ID="txtGarantia" CssClass="form-control " MaxLength="150" ClientIDMode="Static"></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-sm-3">
                                <div class="form-group">
                                    <label class="control-label">Vida útil</label>
                                    <asp:TextBox runat="server" ID="txtVidaUtil" CssClass="form-control " MaxLength="150" ClientIDMode="Static"></asp:TextBox>
                                </div>
                            </div>                            
                        </div>

                        <div class="row mb15">
                            <div class="col-sm-3">
                                <div class="form-group">
                                    <label class="control-label">Descripción</label>
                                    <asp:TextBox runat="server" ID="txtDescripcion" CssClass="form-control " MaxLength="150" ClientIDMode="Static"></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-sm-3">
                                <div class="form-group">
                                    <label class="control-label">Responsable</label>
                                    <asp:TextBox runat="server" ID="txtResponsable" CssClass="form-control " MaxLength="150" ClientIDMode="Static"></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-sm-3">
                                <div class="form-group">
                                    <label class="control-label">Ubicaciones</label>
                                    <asp:TextBox runat="server" ID="txtUbicacion" CssClass="form-control " MaxLength="150" ClientIDMode="Static"></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-sm-3">
                                <div class="form-group">
                                    <label class="control-label">Marca</label>
                                    <asp:TextBox runat="server" ID="txtMarca" CssClass="form-control " MaxLength="150" ClientIDMode="Static"></asp:TextBox>
                                </div>
                            </div>

                        </div>
                        <div class="row mb15">
                            <div class="col-sm-12">
                                <div class="form-group">
                                    <label class="control-label">Observaciones</label>
                                    <asp:TextBox runat="server" TextMode="MultiLine" Rows="5" ID="txtObservaciones" CssClass="form-control " ClientIDMode="Static"></asp:TextBox>
                                </div>
                            </div>
                        </div>

                    </div>

                    <div class="panel-footer">
                        <a class="btn btn-success" id="btnActualizar" onclick="activos.grabar();">Aceptar</a>
                        <a href="#" onclick="activos.cancelar();" style="margin-left:20px">Cancelar</a>
                    </div>

                </div>

                <asp:HiddenField runat="server" ID="hdnCompra" Value="" />
                <asp:HiddenField runat="server" ID="hdnIDPersona" Value="" />
                <asp:HiddenField runat="server" ID="hdnID" Value="0" />
            </form>
        </div>
    </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FooterContent" runat="Server">
    <script src="/js/select2.min.js"></script>
    <script src="/js/jquery.validate.min.js"></script>
    <script src="/js/views/tesoreria/activos.js?v=<%= ConfigurationManager.AppSettings["JS.Version"] %>"></script>

    <script>
        jQuery(document).ready(function () {
            activos.configForm();
        });
    </script>

</asp:Content>
