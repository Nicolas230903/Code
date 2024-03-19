<%@ Page Title="" Language="C#" MasterPageFile="~/Front.master" AutoEventWireup="true" CodeFile="empresase.aspx.cs" Inherits="empresase" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="pageheader">
        <h2><i class='fa fa-briefcase'></i>Empresas</h2>
        <div class="breadcrumb-wrapper">
            <span class="label">Estás aquí:</span>
            <ol class="breadcrumb">
                <li><a href="/home.aspx">axanweb</a></li>
                <li><a href='/modulos/seguridad/empresas.aspx'>Empresas</a></li>
                <li class="active">
                    <asp:Literal runat="server" ID="litPath"></asp:Literal></li>
            </ol>
        </div>
    </div>

    <div class="contentpanel">

        <div class="row mb15">
            <div class="col-sm-3" id="divfoto">
                <img src="/files/usuarios/no-photo.png" class="thumbnail img-responsive" alt="" runat="server" id="imgLogo" />

                <div class="mb30"></div>

                <h5 class="subtitle"><a onclick="Empresa.cambiarPass();">CAMBIAR CONTRASEÑA</a></h5>

                <label class="control-label">Cambiar Logo</label>
                <input type="hidden" value="" name="" /><input type="file" id="flpArchivo" />

                <%--<button class="btn btn-white">Cambiar logo</button>
                <div class="mb20"></div>

                <p class="mb30">Formato JPG, PNG o GIF. Tamaño máximo recomendado: 100x70px</p>--%>
            </div>
            <!-- col-sm-3 -->
            <div class="col-sm-9">


                <div class="profile-header" id="divProfile">
                    <h2 class="profile-name" id="idName">
                        <asp:Literal runat="server" ID="litRazonSocial"></asp:Literal></h2>
                    <div class="profile-location">
                        <i class="fa fa-map-marker"></i>
                        <asp:Literal runat="server" ID="litDomicilio"></asp:Literal>
                        <span id="idLocation" runat="server"></span>
                    </div>
                    <div class="profile-position">
                        <i class="fa fa-briefcase"></i>
                        <asp:Literal runat="server" ID="litCuit"></asp:Literal>
                        <span id="idPosition" runat="server"></span>
                    </div>

                    <div class="mb20"></div>
                </div>
                <!-- profile-header -->


                <div class="alert alert-danger" id="divError" style="display: none">
                    <strong>Lo sentimos! </strong><span id="msgError"></span>
                </div>
                <div class="alert alert-success" id="divOk" style="display: none">
                    <strong>Bien hecho! </strong>Los datos se han actualizado correctamente
                </div>
                <div id="tabs" class="basic-wizard">
                    <!-- Nav tabs -->
                    <ul class="nav nav-tabs nav-justified nav-profile">
                        <li class="active"><a href="#info" data-toggle="tab"><strong>Datos principales</strong></a></li>
                        <li><a href="#domicilio" data-toggle="tab"><strong>Domicilio fiscal</strong></a></li>
                        <li id="liPuntos"><a href="#puntos" data-toggle="tab"><strong>Puntos de venta</strong></a></li>
                        <li id="liPortalClientes"><a href="#portalClientes" data-toggle="tab"><strong>Portal clientes</strong></a></li>
                        <li id="liTemplate"><a href="#Template" data-toggle="tab"><strong>Template</strong></a></li>
                    </ul>
                    <form id="frmEdicion" runat="server">
                        <!-- Tab panes -->
                        <div class="tab-content">
                            <div class="tab-pane active" id="info">

                                <div class="panel panel-default">
                                    <div class="panel-body">
                                        <p><i>Los campos marcados con <code>*</code> son obligatorios.</i></p>

                                        <div class="row mb15">
                                            <div class="col-sm-6">
                                                <div class="form-group">
                                                    <label class="control-label"><code>*</code> Razón Social</label>
                                                    <asp:TextBox runat="server" ID="txtRazonSocial" CssClass="form-control required" MaxLength="128"></asp:TextBox>

                                                </div>
                                            </div>
                                            <div class="col-sm-6">
                                                <div class="form-group">
                                                    <label class="control-label"><code>*</code> Categoría Impositiva</label>
                                                    <asp:DropDownList runat="server" ID="ddlCondicionIva" CssClass="form-control required">
                                                        <asp:ListItem Text="" Value=""></asp:ListItem>
                                                        <asp:ListItem Text="Exento" Value="EX"></asp:ListItem>
                                                        <asp:ListItem Text="Responsable Inscripto" Value="RI"></asp:ListItem>
                                                        <asp:ListItem Text="Responsable no inscripto" Value="NI"></asp:ListItem>
                                                        <asp:ListItem Text="Monotributista" Value="MO"></asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="row mb15">
                                            <div class="col-sm-6">
                                                <div class="form-group">
                                                    <label class="control-label"><code>*</code> CUIT</label>
                                                    <asp:TextBox runat="server" ID="txtCuit" CssClass="form-control required number validCuit" MaxLength="13"></asp:TextBox>
                                                </div>
                                            </div>
                                            <div class="col-sm-6">
                                                <div class="form-group">
                                                    <label class="control-label">Nro. IIBB</label>
                                                    <asp:TextBox runat="server" ID="txtIIBB" CssClass="form-control number" MaxLength="15"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="row mb15">
                                            <div class="col-sm-6">
                                                <div class="form-group">
                                                    <label class="control-label"><code>*</code> Inicio de actividades</label>
                                                    <div class="input-group">
                                                        <span class="input-group-addon"><i class="glyphicon glyphicon-calendar"></i></span>
                                                        <asp:TextBox runat="server" ID="txtFechaInicioAct" CssClass="form-control validDate required" MaxLength="10"></asp:TextBox>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="col-sm-6">
                                                <div class="form-group">
                                                    <label class="control-label"><code>*</code> Personería</label>
                                                    <asp:DropDownList runat="server" ID="ddlPersoneria" CssClass="form-control">
                                                        <asp:ListItem Text="Física" Value="F"></asp:ListItem>
                                                        <asp:ListItem Text="Jurídica" Value="J"></asp:ListItem>
                                                    </asp:DropDownList>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="row mb15">
                                            <div class="col-sm-6">
                                                <div class="form-group">
                                                    <label class="control-label"><code>*</code> E-mail</label>
                                                    <div class="input-group">
                                                        <span class="input-group-addon">@</span>
                                                        <asp:TextBox runat="server" ID="txtEmail" TextMode="Email" Enabled="false" CssClass="form-control required" MaxLength="128"></asp:TextBox>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="col-sm-6">
                                                <div class="form-group">
                                                    <label class="control-label">E-mail alertas</label>
                                                    <div class="input-group">
                                                        <span class="input-group-addon">@</span>
                                                        <asp:TextBox runat="server" ID="txtEmailAlertas" TextMode="Email" CssClass="form-control" MaxLength="128"></asp:TextBox>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="row mb15">
                                            <div class="col-sm-6">
                                                <div class="form-group">
                                                    <label class="control-label">Contacto</label>
                                                    <asp:TextBox runat="server" ID="txtContacto" CssClass="form-control" MaxLength="200"></asp:TextBox>
                                                </div>
                                            </div>
                                            <div class="col-sm-6">
                                                <div class="form-group">
                                                    <label class="control-label">Celular</label>
                                                    <div class="input-group">
                                                        <span class="input-group-addon"><i class="glyphicon glyphicon-earphone"></i></span>
                                                        <asp:TextBox runat="server" ID="txtCelular" TextMode="Phone" CssClass="form-control" MaxLength="50"></asp:TextBox>
                                                    </div>

                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="panel-footer">
                                        <a id="btnDatosPrincipales" id="btnActualizarInfo" class="btn btn-success" onclick="Empresa.grabar();">Actualizar</a>
                                    </div>

                                </div>

                            </div>
                            <div class="tab-pane" id="domicilio">

                                <div class="panel panel-default">
                                    <div class="panel-body">
                                        <p><i>Los campos marcados con <code>*</code> son obligatorios.</i></p>

                                        <div class="row mb15">
                                            <div class="col-sm-6">
                                                <div class="form-group">
                                                    <label class="control-label"><span class="asterisk">*</span> Provincia</label>
                                                    <asp:DropDownList runat="server" ID="ddlProvincia" CssClass="select2 required"
                                                        data-placeholder="Selecciona una provincia..." onchange="Empresa.changeProvincia();">
                                                    </asp:DropDownList>

                                                </div>

                                            </div>
                                            <div class="col-sm-6">
                                                <div class="form-group">
                                                    <label class="control-label"><span class="asterisk">*</span> Ciudad/Localidad</label>
                                                    <asp:DropDownList runat="server" ID="ddlCiudad" CssClass="select2 required"
                                                        data-placeholder="Selecciona una ciudad...">
                                                    </asp:DropDownList>

                                                </div>
                                            </div>
                                        </div>
                                        <div class="row mb15">
                                            <div class="col-sm-6">
                                                <div class="form-group">
                                                    <label class="control-label"><span class="asterisk">*</span> Domicilio</label>
                                                    <asp:TextBox runat="server" ID="txtDomicilio" CssClass="form-control required" MaxLength="100"></asp:TextBox>
                                                </div>
                                            </div>
                                            <div class="col-sm-6">
                                                <div class="form-group">
                                                    <label class="control-label">Piso/Depto</label>
                                                    <asp:TextBox runat="server" ID="txtPisoDepto" CssClass="form-control" MaxLength="10"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="row mb15">
                                            <div class="col-sm-6">
                                                <div class="form-group">
                                                    <label class="control-label"><span class="asterisk">*</span> Código Postal</label>
                                                    <asp:TextBox runat="server" ID="txtCp" CssClass="form-control required" MaxLength="10"></asp:TextBox>
                                                </div>
                                            </div>
                                            <div class="col-sm-6">
                                                <div class="form-group">
                                                    <label class="control-label"><span class="asterisk">*</span> Teléfono</label>
                                                    <div class="input-group">
                                                        <span class="input-group-addon"><i class="glyphicon glyphicon-earphone"></i></span>
                                                        <asp:TextBox runat="server" ID="txtTelefono" TextMode="Phone" CssClass="form-control required" MaxLength="50"></asp:TextBox>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="panel-footer">
                                        <a id="btnDomicilio" class="btn btn-success" id="btnActualizarDomicilio" onclick="Empresa.grabar();">Actualizar</a>
                                    </div>
                                </div>

                            </div>
                            <div class="tab-pane" id="puntos">
                                <div class="alert alert-danger" id="divErrorPuntos" style="display: none">
                                    <strong>Lo sentimos! </strong><span id="msgErrorPuntos"></span>
                                </div>


                                <div class="col-sm-2">
                                    <div class="form-group">
                                        <asp:TextBox runat="server" ID="txtNuevoPunto" CssClass="form-control" MaxLength="4"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-sm-2"><a class="btn btn-default" onclick="Empresa.agregarPunto();">Agregar</a></div>

                                <br />
                                <br />
                                <br />

                                <div class="table-responsive">
                                    <table class="table mb30">
                                        <thead>
                                            <tr>
                                                <th>#</th>
                                                <th>Punto</th>
                                                <th>Fecha alta</th>
                                                <th>Fecha baja</th>
                                                <th>Por defecto</th>
                                                <th></th>
                                            </tr>
                                        </thead>
                                        <tbody id="bodyDetalle">
                                        </tbody>
                                    </table>
                                </div>
                            </div>

                            <div class="tab-pane" id="portalClientes">

                                <div class="panel panel-default">
                                    <div class="panel-body">
                                        <h3>¿Querés que tus clientes accedan a un portal para descargar las facturas que les emitís?</h3>
                                        <p class="mb30">
                                            Seleccionando "Habilitar el portal de clientes" tus clientes podrán ingresar a <a href="http://clientes.axanweb.com" target="_blank">http://clientes.axanweb.com</a> y descargar todas sus facturas de forma online.
                                        </p>

                                        <div class="row mb15">
                                            <div class="col-sm-6">
                                                <div class="form-group">
                                                    <asp:CheckBox runat="server" ID="chkPortalClientes"></asp:CheckBox><label class="control-label"> &nbsp;Habilitar portal Clientes</label>
                                                </div>

                                                <div class="form-group">
                                                    <asp:CheckBox runat="server" ID="ChkCorreoPortal"></asp:CheckBox><label class="control-label"> &nbsp;Enviar mail automáticamente al emitirles una factura</label>
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="panel-footer">
                                        <a class="btn btn-success" id="btnActualizarPortalClientes" onclick="Empresa.portalClientes();">Actualizar</a>
                                    </div>
                                </div>
                            </div>

                            <div class="tab-pane" id="Template">
                                <div class="panel-body">
                                    <h3>Tu imagen es todo</h3>

                                    <p>Elegí el diseño del comprobante que querés emitir para que se adapta a la imagen de tu empresa.</p>

                                    <div class="row mb15">
                                        <div class="row filemanager">
                                            <div class="col-xs-6 col-sm-4 col-md-2">
                                                <div class="thmb">
                                                    <div class="thmb-prev">
                                                        <a href="#" data-rel="prettyPhoto" rel="prettyPhoto">
                                                            <img src="/images/photos/media5.png" class="img-responsive" alt="" />
                                                        </a>
                                                    </div>
                                                    <h5 class="fm-title"><a href="#" data-toggle="modal" data-target="#">Default</a></h5>
                                                    <input type="radio" name="dlltemplate" runat="server" id="default1" value="default" checked="true" />
                                                </div>
                                            </div>
                                            <div class="col-xs-6 col-sm-4 col-md-2">
                                                <div class="thmb">
                                                    <div class="thmb-prev">
                                                        <a href="#" data-rel="prettyPhoto" rel="prettyPhoto">
                                                            <img src="/images/photos/media5.png" class="img-responsive" alt="" />
                                                        </a>
                                                    </div>
                                                    <h5 class="fm-title"><a href="#" data-toggle="modal" data-target="#">Default2</a></h5>
                                                    <input type="radio" name="dlltemplate" runat="server" id="default2" value="default2" />
                                                </div>
                                            </div>
                                            <div class="col-xs-6 col-sm-4 col-md-2">
                                                <div class="thmb">
                                                    <div class="thmb-prev">
                                                        <a href="#" data-rel="prettyPhoto" rel="prettyPhoto">
                                                            <img src="/images/photos/media5.png" class="img-responsive" alt="" />
                                                        </a>
                                                    </div>
                                                    <h5 class="fm-title"><a href="#" data-toggle="modal" data-target="#">Default3</a></h5>
                                                    <input type="radio" name="dlltemplate" runat="server" id="default3" value="default3" />
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="panel-footer">
                                    <a class="btn btn-success" id="btnActualizarTemplate" onclick="Empresa.ActualizarTemplate();">Actualizar</a>
                                </div>
                            </div>
                        </div>
                        <asp:HiddenField runat="server" ID="hdnCiudad" Value="0" />
                        <asp:HiddenField runat="server" ID="hdnProvincia" Value="1" />
                        <input type="hidden" id="IDEmpresa" runat="server" value="" />
                    </form>
                </div>

            </div>
            <!-- col-sm-9 -->
        </div>
        <!-- row -->
    </div>

</asp:Content>


<asp:Content ID="Content3" ContentPlaceHolderID="FooterContent" runat="Server">
    <script src="/js/select2.min.js"></script>
    <script src="/js/bootstrap-wizard.min.js"></script>
    <script src="/js/jquery.validate.min.js"></script>
    <script src="/js/views/seguridad/empresas.js"></script>
    <script src="/js/jquery.ui.widget.js" type="text/javascript"></script>
    <script src="/js/jquery.iframe-transport.js" type="text/javascript"></script>
    <script src="/js/jquery.fileupload.js" type="text/javascript"></script>

    <script>
        jQuery(document).ready(function () {
            Empresa.configForm();
            Empresa.obtenerPuntos();

            if (IDEmpresa.value == "0") {
                Empresa.ocultarDatosExtras();
            }
        });
    </script>
</asp:Content>
