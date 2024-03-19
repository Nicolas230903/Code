<%@ Page Title="" Language="C#" MasterPageFile="~/Front.master" AutoEventWireup="true" CodeFile="conceptose.aspx.cs" Inherits="conceptose" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="pageheader">
        <h2><i class="fa fa-file-text"></i>Productos y servicios </h2>
        <div class="breadcrumb-wrapper">
            <span class="label">Estás aquí:</span>
            <ol class="breadcrumb">
                <li><a href="/home.aspx">axanweb</a></li>
                <li><a href="/conceptos.aspx">Productos y servicios</a></li>
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

                        <div class="col-sm-12">
                            <%-- Datos principales--%>
                            <div class="row mb15">
                                <div class="col-sm-12">
                                    <h3>1. Datos Generales </h3>
                                    <label class="control-label">Estos son los datos básicos que el sistema necesita para poder facturar un producto.</label>
                                </div>
                            </div>
                            <div class="row mb15">
                                <div class="col-sm-3 col-md-2">
                                    <div class="form-group">
                                        <label class="control-label"><span class="asterisk">*</span> Código interno</label>
                                        <asp:TextBox runat="server" ID="txtCodigo" CssClass="form-control" MaxLength="50"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="col-sm-6 col-md-4">
                                    <div class="form-group">
                                        <label class="control-label"><span class="asterisk">*</span> Nombre Producto o Servicio</label>
                                        <asp:TextBox runat="server" ID="txtNombre" CssClass="form-control required" MaxLength="100"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                            <div class="row mb15">
                                <div class="col-sm-3 col-md-2">
                                    <div class="form-group">
                                        <label class="control-label"><span class="asterisk">*</span> Estado</label>
                                        <asp:DropDownList runat="server" ID="ddlEstado" CssClass="form-control required">
                                            <asp:ListItem Text="Activo" Value="A"></asp:ListItem>
                                            <asp:ListItem Text="Inactivo" Value="I"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div class="col-sm-3 col-md-2">
                                    <div class="form-group">
                                        <label class="control-label"><span class="asterisk">*</span> Tipo</label>
                                        <asp:DropDownList runat="server" ID="ddlTipo" CssClass="form-control" onchange="changeTipo(true);">
                                            <asp:ListItem Value="P">Producto</asp:ListItem>
                                            <asp:ListItem Value="S">Servicio</asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>
                            </div>

                            <div class="row mb15">
                                <div class="col-sm-12">
                                    <hr />
                                    <h3>2. Importes de Productos y Servicios </h3>
                                    <label class="control-label">Ingresa los importes para que en tus ventas no lo tengas que hacer :)</label>
                                </div>
                            </div>
                            <div class="row mb15">
                                <div class="col-sm-4 col-md-2">
                                    <div class="form-group">
                                        <label class="control-label">Costo interno</label>
                                        <div class="input-group">
                                            <span class="input-group-addon">$</span>
                                            <asp:TextBox runat="server" ID="txtCostoInterno" CssClass="form-control" MaxLength="10"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-sm-3 col-md-2">
                                    <div class="form-group">
                                        <label class="control-label">
                                            <span class="asterisk">*</span>
                                            <asp:Literal ID="liPrecioUnitario" runat="server"></asp:Literal></label>
                                        <div class="input-group">
                                            <span class="input-group-addon">$</span>
                                            <asp:TextBox runat="server" ID="txtPrecio" CssClass="form-control required" MaxLength="10"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-sm-2 col-md-1">
                                    <div class="form-group">
                                        <label class="control-label"><span class="asterisk">*</span> IVA %</label>
                                        <asp:DropDownList runat="server" ID="ddlIva" CssClass="form-control">
                                            <asp:ListItem Value="1">NG</asp:ListItem>
                                            <asp:ListItem Value="2">Exento</asp:ListItem>
                                            <asp:ListItem Value="3">0</asp:ListItem>
                                            <asp:ListItem Value="4">10,5</asp:ListItem>
                                            <asp:ListItem Value="5">21</asp:ListItem>
                                            <asp:ListItem Value="6">27</asp:ListItem>
                                            <asp:ListItem Value="8">5</asp:ListItem>
                                            <asp:ListItem Value="9">2,5</asp:ListItem>    
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div class="col-sm-3 col-md-2" hidden="hidden">
                                    <h5 class="subtitle mb10">
                                        <asp:Literal ID="liPrecioTotal" runat="server"></asp:Literal></h5>
                                    <h4 class="text-primary" style="color: green" id="divPrecioIVA">$
                                        <asp:Literal runat="server" ID="litTotal"></asp:Literal></h4>
                                </div>
                            </div>

                            <%-- Datos Generales--%>
                            <div class="row mb15">
                                <div class="col-sm-12">
                                    <hr />
                                    <h3>3. Información adicional</h3>
                                    <label class="control-label">Estos datos no son necesarios para el sistema, pero podrían serte de utilidad.</label>
                                </div>
                            </div>

                            <div class="row mb15">
                                <div class="col-sm-4 col-md-4">
                                    <div class="form-group">
                                        <label class="control-label">Descripción</label>
                                        <asp:TextBox runat="server" ID="txtDescripcion" CssClass="form-control" MaxLength="500"></asp:TextBox>
                                    </div>
                                </div>
                            </div>

                            <div class="row mb15">
                                <div class="col-sm-2 col-md-2" id="divStock">
                                    <div class="form-group">
                                        <label class="control-label">Stock</label>
                                        <asp:TextBox runat="server" ID="txtStock" CssClass="form-control" MaxLength="10"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-sm-2 col-md-2" id="divStockFisico">
                                    <div class="form-group">
                                        <label class="control-label">Stock Físico</label>
                                        <asp:TextBox runat="server" ID="txtStockFisico" CssClass="form-control" MaxLength="10" Enabled="false"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-sm-2 col-md-2" id="divStockMinimo">
                                    <div class="form-group">
                                        <label class="control-label">Stock mínimo</label>
                                        <asp:TextBox runat="server" ID="txtStockMinimo" CssClass="form-control" MaxLength="10"></asp:TextBox>
                                        <span class="help-block"><small>Te servirá para recibir alertas en tu mail.</small></span>
                                    </div>
                                </div>
                            </div>
                            <div class="row mb15">
                                <div class="col-sm-3">
                                    <label class="control-label">Cliente/proveedor origen</label>
                                    <select id="ddlPersonas" class="select2" data-placeholder="Seleccione un cliente/proveedor...">
                                    </select>
                                </div>
                            </div>

                            <div class="row mb15">
                                <span style="margin-left: 15px">Foto del producto</span>
                            </div>
                            <div class="row mb15">
                                <div class="col-sm-3 col-md-1">
                                    <img id="imgFoto" src="/files/usuarios/no-producto.png" class="thumbnail img-responsive" alt="" runat="server" />
                                    <div class="mb30"></div>

                                </div>
                                <div class="col-sm-4 col-md-2" id="divAdjuntarFoto">
                                    <a class="btn btn-white btn-block" onclick="showInputFoto();">Adjuntar foto</a>
                                    <div id="divFoto" style="display: none">
                                        <p class="mb30">Formato JPG, PNG o GIF. Tamaño máximo recomendado: 100x70px</p>
                                        <input type="hidden" value="" name="" /><input type="file" id="flpArchivo" />
                                        <div class="mb20"></div>
                                    </div>
                                </div>
                                <div class="col-sm-4 col-md-2" id="divEliminarFoto">
                                    <a class="btn btn-white btn-block" onclick="eliminarFotoProducto();">Eliminar foto</a>
                                </div>
                            </div>

                            <div class="row mb15">
                                <div class="col-sm-12 col-md-6">
                                    <div class="form-group">
                                        <label class="control-label">Observaciones</label>
                                        <asp:TextBox runat="server" TextMode="MultiLine" Rows="5" ID="txtObservaciones" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="panel-footer">
                        <a class="btn btn-success" id="actualizarConcepto" onclick="grabarsinImagen();">Aceptar</a>
                        <a href="#" onclick="cancelar();" style="margin-left: 20px">Cancelar</a>
                    </div>
                </div>
                
                <asp:HiddenField runat="server" ID="hdnIDPersona" Value="0" />
                <asp:HiddenField runat="server" ID="hdnUsaPrecioFinalConIVA" Value="0" />
                <asp:HiddenField runat="server" ID="hdnTieneFoto" Value="0" />
                <asp:HiddenField runat="server" ID="hdnSinCombioDeFoto" Value="0" />
                <asp:HiddenField runat="server" ID="hdnIdUsuario" Value="0" />
                <asp:HiddenField runat="server" ID="hdnID" Value="0" />
            </form>
        </div>
    </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FooterContent" runat="Server">
    <script src="/js/jquery.maskMoney.min.js"></script>
    <script src="/js/jquery.validate.min.js"></script>
    <script src="/js/views/conceptos.js?v=<%= ConfigurationManager.AppSettings["JS.Version"] %>"></script>    
    <script src="/js/jquery.fileupload.js" type="text/javascript"></script>
    <script src="js/select2.min.js"></script>
    <script>
        jQuery(document).ready(function () {
            configForm();
        });
    </script>

</asp:Content>
