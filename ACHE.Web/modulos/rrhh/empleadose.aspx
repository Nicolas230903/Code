<%@ Page Title="" Language="C#" MasterPageFile="~/Front.master" AutoEventWireup="true" CodeFile="empleadose.aspx.cs" Inherits="modulos_rrhh_empleadose" %>

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
            <i class='fa fa-child'></i>Empleado
        </h2>
        <div class="breadcrumb-wrapper">
            <span class="label">Estás aquí:</span>
            <ol class="breadcrumb">
                <li><a href="/home.aspx">axanweb</a></li>
                <li><a href='/Modulos/rrhh/empleados.aspx'>Empleados</a></li>
                <li class="active">
                    <asp:Literal runat="server" ID="litPath"></asp:Literal></li>
            </ol>
        </div>
    </div>

    <div class="contentpanel">
        <div class="row mb15">

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
                               <%-- <p class="mb30">Formato JPG, PNG o GIF. Tamaño máximo recomendado: 100x70px</p>--%>
                                <input type="hidden" value="" name="" /><input type="file" id="flpArchivo" />
                                <div class="mb20"></div>
                            </div>

                            <div class="col-sm-6" id="divAdjuntarFoto">
                                <a class="btn btn-white btn-block" onclick="empleados.showInputFoto();">Adjuntar foto</a>
                            </div>
                            <div class="col-sm-6" id="divEliminarFoto">
                                <a class="btn btn-white btn-block" onclick="empleados.eliminarFoto();">Eliminar foto</a>
                            </div>
                        </div>
                        <div class="col-sm-9">

                            <ul class="nav nav-tabs nav-justified nav-profile">
                                <li id="liDAtosPrincipales" class="active"><a href="#datosPrincipales" data-toggle="tab"><strong>Datos principales</strong></a></li>
                                <li id="liDomicilio"><a href="#domicilio" data-toggle="tab"><strong>Domicilio</strong></a></li>
                            </ul>
                            <form id="frmEdicion" runat="server" class="col-sm-12">
                                <div class="tab-content">
                                    <div class="tab-pane active" id="datosPrincipales">

                                        <div class="row mb15">

                                            <div class="col-sm-4">
                                                <div class="form-group">
                                                    <label class="control-label"><span class="asterisk">*</span> Nombre</label>
                                                    <asp:TextBox runat="server" ID="txtNombre" CssClass="form-control required" MaxLength="128" ClientIDMode="Static"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-sm-4">
                                                <div class="form-group">
                                                    <label class="control-label"><span class="asterisk">*</span> Apellido</label>
                                                    <asp:TextBox runat="server" ID="txtApellido" CssClass="form-control required" MaxLength="128" ClientIDMode="Static"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-sm-4">
                                                <div class="form-group">
                                                    <label class="control-label"><span class="asterisk">*</span> CUIT</label>
                                                    <asp:TextBox runat="server" ID="txtCUIT" CssClass="form-control required number validCuit" MaxLength="13" ClientIDMode="Static"></asp:TextBox>
                                                </div>
                                            </div>

                                        </div>

                                        <div class="row mb15">

                                            <div class="col-sm-4">
                                                <div class="form-group">
                                                    <label class="control-label">Teléfono</label>
                                                    <asp:TextBox runat="server" ID="txtTelefono" CssClass="form-control" MaxLength="128" ClientIDMode="Static" placeholder="+54911########"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-sm-4">
                                                <div class="form-group">
                                                    <label class="control-label">Celular</label>
                                                    <asp:TextBox runat="server" ID="txtCelular" CssClass="form-control" MaxLength="20" ClientIDMode="Static" placeholder="+54911########"></asp:TextBox>
                                                </div>
                                            </div>

                                            <div class="col-sm-4">
                                                <div class="form-group">
                                                    <label class="control-label">Nro de legajo</label>
                                                    <asp:TextBox runat="server" ID="txtNroLegajo" CssClass="form-control" MaxLength="50" ClientIDMode="Static"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="row mb15">
                                            <div class="col-sm-6">
                                                <div class="form-group">
                                                    <label class="control-label"><span class="asterisk">*</span> Fecha de alta</label>
                                                    <div class="input-group">
                                                        <span class="input-group-addon"><i class="glyphicon glyphicon-calendar"></i></span>
                                                        <asp:TextBox runat="server" ID="txtFechaAlta" CssClass="form-control validDate required" placeholder="dd/mm/yyyy" MaxLength="10"></asp:TextBox>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="col-sm-6">
                                                <div class="form-group">
                                                    <label class="control-label">Fecha de baja</label>
                                                    <div class="input-group">
                                                        <span class="input-group-addon"><i class="glyphicon glyphicon-calendar"></i></span>
                                                        <asp:TextBox runat="server" ID="txtFechaBaja" CssClass="form-control validDate" placeholder="dd/mm/yyyy" MaxLength="10"></asp:TextBox>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="row mb15">
                                            <div class="col-sm-12">
                                                <div class="form-group">
                                                    <label class="control-label">Contacto de Emergencia</label>
                                                    <asp:TextBox runat="server" TextMode="MultiLine" ID="txtContactoEmergencia" CssClass="form-control" Rows="5" ClientIDMode="Static"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>

                                    </div>
                                    <div class="tab-pane" id="domicilio">
                                        <div class="row mb15">

                                            <div class="col-sm-3">
                                                <div class="form-group">
                                                    <label class="control-label"><span class="asterisk">*</span> Provincia</label>
                                                    <asp:DropDownList runat="server" ID="ddlProvincia" CssClass="select2 required" data-placeholder="Selecciona una provincia..." onchange="empleados.changeProvincia();">
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                            <div class="col-sm-3">
                                                <div class="form-group">
                                                    <label class="control-label"><span class="asterisk">*</span> Cuidad</label>
                                                      <asp:DropDownList runat="server" ID="ddlCiudad" CssClass="select2 required" data-placeholder="Selecciona una ciudad...">
                                                      </asp:DropDownList>
                                                </div>
                                            </div>
                                            <div class="col-sm-3">
                                                <div class="form-group">
                                                    <label class="control-label"><span class="asterisk">*</span> Domicilio</label>
                                                    <asp:TextBox runat="server" ID="txtDomicilio" CssClass="form-control required" MaxLength="150"></asp:TextBox>
                                                </div>
                                            </div>
                                            <div class="col-sm-3">
                                                <div class="form-group">
                                                    <label class="control-label">Piso/Depto</label>
                                                    <asp:TextBox runat="server" ID="txtPisoDepto" CssClass="form-control" MaxLength="150"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>

                                        <div class="row mb15">
                                            <div class="col-sm-4">
                                                <div class="form-group">
                                                    <label class="control-label">Obra Social</label>
                                                    <asp:TextBox runat="server" ID="txtObraSocial" CssClass="form-control" MaxLength="150"></asp:TextBox>
                                                </div>
                                            </div>
                                            <div class="col-sm-4">
                                                <div class="form-group">
                                                    <label class="control-label">Sueldo</label>
                                                    <asp:TextBox runat="server" ID="txtSueldo" CssClass="form-control" MaxLength="150"></asp:TextBox>
                                                </div>
                                            </div>
                                            <div class="col-sm-4">
                                                <div class="form-group">
                                                    <label class="control-label">Email</label>
                                                    <asp:TextBox runat="server" ID="txtEmail" CssClass="form-control email" MaxLength="150"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <asp:HiddenField runat="server" ID="hdnCiudad" Value="0" />
                                <asp:HiddenField runat="server" ID="hdnProvincia" Value="1" />
                                <asp:HiddenField runat="server" ID="hdnTieneFoto" Value="0" />
                                <asp:HiddenField runat="server" ID="hdnSinCombioDeFoto" Value="0" />
                                <asp:HiddenField runat="server" ID="Idusuario" Value="0" ClientIDMode="Static" />
                                <asp:HiddenField runat="server" ID="hdnFileName" Value="" />
                                <asp:HiddenField runat="server" ID="hdnID" Value="0" />
                            </form>
                        </div>
                    </div>
                </div>
                <div class="panel-footer">
                    <a class="btn btn-success" id="btnActualizar" onclick="empleados.grabarsinImagen();">Aceptar</a>
                    <a href="#" onclick="empleados.cancelar();" style="margin-left:20px">Cancelar</a>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FooterContent" runat="Server">
    <script src="/js/select2.min.js"></script>
    <script src="/js/bootstrap-wizard.min.js"></script>
    <script src="/js/jquery.validate.min.js"></script>
    <script src="/js/jquery.maskedinput.min.js"></script>
    <script src="/js/jquery.maskMoney.min.js"></script>
    <script src="/js/views/rrhh/empleados.js?v=<%= ConfigurationManager.AppSettings["JS.Version"] %>"></script>
    <script src="/js/jquery.ui.widget.js" type="text/javascript"></script>
    <script src="/js/jquery.fileupload.js" type="text/javascript"></script>
    <script>
        jQuery(document).ready(function () {
            empleados.configForm();
        });
    </script>

</asp:Content>
