<%@ Page Title="" Language="C#" MasterPageFile="~/Front.master" AutoEventWireup="true" CodeFile="cambiar-pwd.aspx.cs" Inherits="cambiar_pwd" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
     <div class="pageheader">
        <h2><i class="fa fa-key"></i>Cambiar contraseña </h2>
        <div class="breadcrumb-wrapper">
            <span class="label">Estás aquí:</span>
            <ol class="breadcrumb">
                <li><a href="/home.aspx">axanweb</a></li>
                <li><a href="/modulos/seguridad/mis-datos.aspx">Mi cuenta</a></li>
                <li class="active">Cambiar contraseña</li>
            </ol>
        </div>
    </div>

    <div class="contentpanel">

        <div class="row mb15">
            <form id="frmEdicion" runat="server" class="col-sm-12">
                <div class="panel panel-default">
                    <div class="panel-body">
                        <div class="alert alert-danger" id="divError" style="display:none">
                            <strong>Lo sentimos! </strong><span id="msgError"></span>
                        </div>

                        <div class="alert alert-success" id="divOk" style="display:none">
                            <strong>Bien hecho! </strong>Los datos se han actualizado correctamente
                        </div>

                        <div class="row mb15">
                            <div class="col-sm-3">
                                <div class="form-group">
                                    <label class="control-label"><span class="asterisk">*</span> Contraseña actual</label>
                                    <asp:TextBox runat="server" TextMode="Password" ID="txtActual" CssClass="form-control required" MaxLength="20" autocomplete="off"></asp:TextBox>
                                                
                                </div>
                            </div>
                            
                        </div>
                        <div class="row mb15">
                            <div class="col-sm-3">
                                <div class="form-group">
                                    <label class="control-label"><span class="asterisk">*</span> Nueva contraseña</label>
                                    <asp:TextBox runat="server" TextMode="Password" ID="txtPwd" CssClass="form-control required validPassword" MaxLength="20" MinLength="8" autocomplete="off"></asp:TextBox>
                                                
                                </div>
                            </div>
                        </div>
                        <div class="row mb15">
                            <div class="col-sm-3">
                                <div class="form-group">
                                    <label class="control-label"><span class="asterisk">*</span> Confirmar contraseña</label>
                                    <asp:TextBox runat="server" TextMode="Password" ID="txtPwd2" CssClass="form-control required validPassword2" MaxLength="20" MinLength="8" autocomplete="off"></asp:TextBox>
                                                
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="panel-footer">
                        <a class="btn btn-success" id="btnActualizar" onclick="CambiarPwd.grabar();">Actualizar</a>
                        <a href="#" onclick="CambiarPwd.cancelar();" style="margin-left:20px">Cancelar</a>
                    </div>
                </div>
                <input type="hidden" id="IdUsuarioPadre" runat="server" value="" />
            </form>
        </div>
    </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FooterContent" Runat="Server">
    <script src="/js/select2.min.js"></script>
    <script src="/js/jquery.validate.min.js"></script>
    <script src="/js/views/seguridad/cambiar-pwd.js?v=<%= ConfigurationManager.AppSettings["JS.Version"] %>"></script>
</asp:Content>

