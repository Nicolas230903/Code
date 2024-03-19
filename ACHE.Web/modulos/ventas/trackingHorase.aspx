<%@ Page Title="" Language="C#" MasterPageFile="~/Front.master" AutoEventWireup="true" CodeFile="trackingHorase.aspx.cs" Inherits="modulos_ventas_trackingHorase" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="pageheader">
        <h2>
            <asp:Literal runat="server" ID="litTitulo"></asp:Literal>
        </h2>
        <div class="breadcrumb-wrapper">
            <span class="label">Estás aquí:</span>
            <ol class="breadcrumb">
                <li><a href="/home.aspx">axanweb</a></li>
                <li>
                    <asp:Literal runat="server" ID="litPathPadre"></asp:Literal></li>
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
                            <div class="col-sm-12">
                                <h3>Datos generales</h3>
                                <label class="control-label">Acá van los datos más generales para crear un seguimiento de horas.</label>
                            </div>
                        </div>

                        <div class="row mb15">
                            <div class="col-sm-4">
                                <div class="form-group">
                                    <label class="control-label"><span class="asterisk">*</span> Proveedor/Cliente</label>
                                  <asp:DropDownList runat="server" ID="ddlPersona" CssClass="select2 required" data-placeholder="Seleccione un cliente/proveedor..." ClientIDMode="Static">
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="col-sm-4">
                                <div class="form-group">
                                    <label class="control-label"><span class="asterisk">*</span> Fecha</label>
                                     <asp:TextBox runat="server" ID="txtFecha" CssClass="form-control validDate required" placeholder="dd/mm/yyyy" MaxLength="10"></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-sm-4">
                                <div class="form-group">
                                    <label class="control-label"><span class="asterisk">*</span> Tarea</label>
                                    <%--<asp:DropDownList runat="server" ID="ddlTarea" CssClass="form-control required" ClientIDMode="Static">
                                        <asp:ListItem Text="tarea 1" Value="Tarea1"></asp:ListItem>
                                        <asp:ListItem Text="tarea 2" Value="Tarea2"></asp:ListItem>
                                    </asp:DropDownList>--%>
                                     <asp:TextBox runat="server" ID="ddlTarea" CssClass="form-control required" MaxLength="128" ClientIDMode="Static"></asp:TextBox>
                                </div>
                            </div>
                        </div>

                        <div class="row mb15">
                             <div class="col-sm-4">
                                <div class="form-group">
                                    <label class="control-label"><span class="asterisk">*</span> Usuario </label>
                                    <asp:DropDownList runat="server" ID="ddlUuarios" CssClass="form-control" ClientIDMode="Static">
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="col-sm-4">
                                <div class="form-group">
                                    <label class="control-label"><span class="asterisk">*</span> Cant. horas</label>
                                    <asp:TextBox runat="server" ID="txtCantHoras" CssClass="form-control required" MaxLength="128" ClientIDMode="Static"></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-sm-4">
                                <div class="form-group">
                                    <label class="control-label"><span class="asterisk">*</span> Modo</label>
                                    <asp:DropDownList runat="server" ID="ddlEstado" CssClass="form-control" ClientIDMode="Static">
                                        <asp:ListItem Text="Facturable" Value="Facturable"></asp:ListItem>
                                        <asp:ListItem Text="No Facturable" Value="No Facturable"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>                             
                        </div>
                        <div class="row mb15">
                            <div class="col-sm-0">
                                <div class="form-group">
                                    <label class="control-label"> Observaciones</label>
                                    <asp:TextBox runat="server" TextMode="MultiLine" Rows="5" ID="txtObservaciones" CssClass="form-control" MaxLength="128" ClientIDMode="Static"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="panel-footer">
                        <a class="btn btn-success" id="btnActualizar" onclick="trackingHoras.grabar();">Aceptar</a>
                        <a href="#" onclick="trackingHoras.cancelar();" style="margin-left:20px">Cancelar</a>

                    </div>

                </div>
                <asp:HiddenField runat="server" ID="hdnIDPersona" Value="0" />
                <asp:HiddenField runat="server" ID="hdnID" Value="0" />
            </form>
        </div>
    </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FooterContent" runat="Server">
    <script src="/js/select2.min.js"></script>
    <script src="/js/jquery.validate.min.js"></script>
    <%--<script src="/js/views/common.js"></script>--%>
    <script src="/js/views/ventas/trackingHoras.js?v=<%= ConfigurationManager.AppSettings["JS.Version"] %>"></script>

    <script>
        jQuery(document).ready(function () {
            trackingHoras.configForm();
        });
    </script>

</asp:Content>
