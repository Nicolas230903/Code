<%@ Page Title="" Language="C#" MasterPageFile="~/Front.master" AutoEventWireup="true" CodeFile="mis-datos.aspx.cs" Inherits="mis_datos" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
    <style>
        #tabsPerfil.nav-tabs > li.active > a, #tabsPerfil.nav-tabs > li.active > a:hover, #tabsPerfil.nav-tabs > li.active > a:focus {
            color: #bf315f;
            text-decoration: none !important;
        }

        #tabsPerfil.nav-tabs.nav-justified > li > a {
            border-bottom: 0;
            text-decoration: underline;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="pageheader">
        <h2 id="spanMisDatos"><i class="fa fa-user"></i>Mi cuenta</h2>
        <div class="breadcrumb-wrapper">
            <span class="label">Estás aquí:</span>
            <ol class="breadcrumb">
                <li><a href="/home.aspx">axanweb</a></li>
                <li class="active"><span id="spanMisDatosUbicacion">Mi cuenta</span></li>
            </ol>
        </div>
    </div>

    <div class="contentpanel">

        <div class="row mb15">
            <div class="col-sm-12" id="divDatosPersonales">
                <!-- profile-header -->
                <div class="alert alert-danger" id="divError" style="display: none">
                    <strong>Lo sentimos! </strong><span id="msgError"></span>
                </div>
                <div class="alert alert-success" id="divOk" style="display: none">
                    <strong>Bien hecho! </strong>Los datos se han actualizado correctamente
                </div>

                <ul class="nav nav-tabs nav-justified nav-profile">

                    <li id="liPerfilUsuario" class="active" runat="server">
                        <a href="#PerfilUsuario" data-toggle="tab" style="text-align: left;">
                            <%--<span class="fa fa-info" style="font-size: 18px;"></span>--%>
                            <strong><span style="font-size:large;font-weight: bold">Mis Datos</span></strong><br />
                            <small>Información de los datos de tu usuario</small>
                        </a>
                    </li>
                    <li id="liplanPagos" runat="server" visible="false">
                        <a href="#planPagos" data-toggle="tab" style="text-align: left;">
                            <span class="fa fa-credit-card" style="font-size: 18px;"></span>
                            <strong>PLAN Y PAGOS</strong><br />
                            <small>Información sobre tu plan y los pagos realizados</small>
                        </a>
                    </li>
                    <li id="liEmpresas" runat="server">
                        <a href="#Empresas" data-toggle="tab" style="text-align: left;">
                            <span class="fa fa-briefcase" style="font-size: 18px;"></span>
                            <strong>EMPRESAS</strong><br />
                            <small>Administre los CUIT con los que desea operar</small>
                        </a>
                    </li>
                    <li id="liAlertasyAvisos" runat="server" visible="false">
                        <a href="#AlertasyAvisos" data-toggle="tab" style="text-align: left;">
                            <span class="fa fa-bell-o" style="font-size: 18px;"></span>
                            <strong>Alertas y avisos</strong><br />
                            <small>Configure los envíos automáticos que desea programar</small>
                        </a>
                    </li>
                    <li id="li3"><a href="#" data-toggle="tab"><strong></strong></a></li>
                    <li id="li4"><a href="#" data-toggle="tab"><strong></strong></a></li>
                </ul>
                <form id="frmEdicion" runat="server" clientidmode="Static">
                    <div class="tab-content">
                        <div class="tab-pane active" id="PerfilUsuario" runat="server">
                            <ul class="nav nav-tabs nav-justified nav-profile tab-content" id="tabsPerfil">
                                <li id="liDatosPrincipales" class="active" runat="server"><a href="#info" data-toggle="tab" style="text-align: left;"><strong>Identificación</strong></a></li>
                                <li id="liDomicilio"><a href="#domicilio" data-toggle="tab" style="text-align: left;"><strong>Contacto</strong></a></li>
                                <li id="liDatosFiscales" runat="server"><a href="#datosFiscales" data-toggle="tab" style="text-align: left;"><strong>Impositivos</strong></a></li>
                                <li id="liportalClientes" runat="server"><a href="#portalClientes" data-toggle="tab" style="text-align: left;"><strong>Acceso clientes</strong></a></li>
                                <li id="liTemplate" runat="server"><a href="#template" data-toggle="tab" style="text-align: left;"><strong>Formato de factura</strong></a></li>
                                <li id="liConfiguracion"><a href="#configuracion" data-toggle="tab" style="text-align: left;"><strong>Preferencias</strong></a></li>
                                <li id="liCambiarpwd"><a href="/modulos/seguridad/cambiar-pwd.aspx" style="text-align: left;"><strong>Contraseña</strong></a></li>
                                <li id="Usuarios"><a href="/modulos/seguridad/usuarios.aspx" style="text-align: left;"><strong>Usuarios del sistema</strong></a></li>
                                <li id="liPlandeCuentas" runat="server"><a href="#plandeCuentas" data-toggle="tab" style="text-align: left;"><strong>Plan de cuenta</strong></a></li>                                
                            </ul>
                            <hr />
                            <div class="tab-content">
                                <div class="tab-pane active" id="info" runat="server">
                                    <div class="panel panel-default">
                                        <div class="panel-body">
                                            <div class="col-sm-12">
                                                <div class="col-sm-2" id="divFotoPerfil">
                                                    <img id="imgLogo" src="/files/usuarios/no-photo.png" class="thumbnail img-responsive" alt="" runat="server" />
                                                    <div class="mb30"></div>

                                                    <div id="divLogo" style="display: none">
                                                        <p class="mb30">Formato JPG, PNG o GIF. Tamaño máximo recomendado: 200x60px</p>
                                                        <input type="hidden" value="" name="" /><input type="file" id="flpArchivo" />
                                                        <div class="mb20"></div>
                                                    </div>

                                                    <div class="col-sm-12" id="divAdjuntarFoto">
                                                        <a class="btn btn-white" onclick="foto.showInputLogo();">Adjuntar foto</a>
                                                    </div>
                                                    <div class="col-sm-12" id="divEliminarFoto">
                                                        <a class="btn btn-white" onclick="foto.eliminarLogo();">Eliminar foto</a>
                                                    </div>
                                                    <div class="col-sm-12" id="divDescargarTemplate">
                                                        <a class="btn btn-white" href="../../fe/TemplateFactura_default.pdf" target="_blank">Descargar Factura Borrador</a>
                                                    </div>
                                                    <div class="col-sm-12" id="divEliminarUsuario" style="padding-top: 20px">
                                                        <a class="btn btn-danger" onclick="MisDatos.eliminarUsuario();">ELIMINAR USUARIO</a>
                                                    </div>
                                                    <div class="col-sm-12 alert alert-success" id="divOkEliminarUsuario" style="display: none">
                                                        <strong></strong><span id="msgOkEliminarUsuario"></span>
                                                    </div>
                                                    <div class="col-sm-12 alert alert-danger" id="divErrorEliminarUsuario" style="display: none">
                                                        <strong>Lo sentimos! </strong><span id="msgErrorEliminarUsuario"></span>
                                                    </div>
                                                </div>
                                                <div class="col-sm-1"></div>
                                                <div class="col-sm-9">
                                                    <div class="row mb15">
                                                        <div class="col-sm-12">
                                                            <h3>Datos principales </h3>
                                                            <label class="control-label">Estos son los datos básicos y obligatorios que el sistema necesita para poder facturar.</label>
                                                        </div>
                                                    </div>
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
                                                                <asp:TextBox runat="server" ID="txtCuit" CssClass="form-control required number validCuitMisDatos" MaxLength="13"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                        <div class="col-sm-6">
                                                            <div class="form-group">
                                                                <label class="control-label"><code>*</code> E-mail</label>
                                                                <div class="input-group">
                                                                    <span class="input-group-addon">@</span>
                                                                    <asp:TextBox runat="server" ID="txtEmail" TextMode="Email" Enabled="false" CssClass="form-control required" MaxLength="128"></asp:TextBox>
                                                                </div>
                                                            </div>
                                                        </div>
                                                    </div>
                                                    <div class="row mb15">
                                                        <div class="col-sm-6">
                                                            <div class="form-group">
                                                                <label class="control-label">Inicio de actividades</label>
                                                                <div class="input-group">
                                                                    <span class="input-group-addon"><i class="glyphicon glyphicon-calendar"></i></span>
                                                                    <asp:TextBox runat="server" ID="txtFechaInicioAct" CssClass="form-control validDate" MaxLength="10"></asp:TextBox>
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
                                                </div>
                                            </div>
                                        </div>
                                        <div class="panel-footer">
                                            <div class="col-sm-3"></div>
                                            <div class="col-sm-9">
                                                <a class="btn btn-success" id="btnActualizarInfo" onclick="MisDatos.grabar();">Actualizar</a>
                                            </div>
                                        </div>
                                    </div>

                                </div>
                                <div class="tab-pane" id="domicilio">

                                    <div class="panel panel-default">
                                        <div class="panel-body">
                                            <div class="row mb15">
                                                <div class="col-sm-6">
                                                    <div class="form-group">
                                                        <label class="control-label"><span class="asterisk">*</span> Provincia</label>
                                                        <asp:DropDownList runat="server" ID="ddlProvincia" CssClass="select2 required"
                                                            data-placeholder="Selecciona una provincia..." onchange="MisDatos.changeProvincia();">
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
                                                        <label class="control-label">Código Postal</label>
                                                        <asp:TextBox runat="server" ID="txtCp" CssClass="form-control" MaxLength="10"></asp:TextBox>
                                                    </div>
                                                </div>
                                                <div class="col-sm-6">
                                                    <div class="form-group">
                                                        <label class="control-label">Teléfono</label>
                                                        <div class="input-group">
                                                            <span class="input-group-addon"><i class="glyphicon glyphicon-earphone"></i></span>
                                                            <asp:TextBox runat="server" ID="txtTelefono" CssClass="form-control" MaxLength="50" placeholder="+54911########"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row mb15">                                                        
                                                <div class="col-sm-6">
                                                    <div class="form-group">
                                                        <label class="control-label">E-mail alertas</label>
                                                        <div class="input-group">
                                                            <span class="input-group-addon">@</span>
                                                            <asp:TextBox runat="server" ID="txtEmailAlertas" CssClass="form-control multiemails" MaxLength="128"></asp:TextBox>
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
                                                            <asp:TextBox runat="server" ID="txtCelular" TextMode="Phone" CssClass="form-control" MaxLength="50" placeholder="+54911########"></asp:TextBox>
                                                        </div>

                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="panel-footer">
                                            <a class="btn btn-success" id="btnActualizarDomicilio" onclick="MisDatos.grabar();">Actualizar</a>
                                        </div>
                                    </div>

                                </div>
                                <div class="tab-pane" id="portalClientes">

                                    <div class="panel panel-default">
                                        <div class="panel-body">
                                            <h3>¿Querés que tus clientes accedan a un portal para descargar las facturas que les emitís? (VÁLIDO para plan PYME en adelante)</h3>
                                            <p class="mb30">
                                                Seleccionando "Habilitar acceso de clientes" tus clientes podrán ingresar a <a href="http://clientes.axanweb.com" target="_blank">http://clientes.axanweb.com</a> y descargar todas sus facturas de forma online.
                                            </p>

                                            <div class="row mb15">
                                                <div class="col-sm-6">
                                                    <div class="form-group">
                                                        <asp:CheckBox runat="server" ID="chkPortalClientes"></asp:CheckBox><label class="control-label"> &nbsp;Habilitar acceso de clientes</label>
                                                    </div>

                                                    <div class="form-group hide">
                                                        <asp:CheckBox runat="server" ID="ChkCorreoPortal"></asp:CheckBox><label class="control-label"> &nbsp;Enviar mail automáticamente al emitirles una factura</label>
                                                    </div>
                                                </div>
                                            </div>
                                            <div>
                                                <hr />
                                                <h3>¿Querés cobrarle a tus clientes las facturas que les emitís a través de MercadoPago?</h3>
                                                <p>Sigue los siguientes pasos:</p>
                                                <br />
                                                <p>
                                                    1.- Ingresa a <a href="https://www.mercadopago.com/mla/herramientas/aplicaciones" target="_blank">www.mercadopago.com/mla/herramientas/aplicaciones</a>
                                                </p>
                                                <p>
                                                    2.- Completa el usuario y contraseña de Mercado Pago en caso de ser necesario.
                                                </p>
                                                <p>
                                                    3.- Copia los valores de Client_id y Client_secret a los campos correspondientes.
                                                </p>

                                                <div>
                                                    <div class="row">
                                                        <div class="col-sm-9 col-md-5 col-lg-3">
                                                            <div class="form-group">
                                                                <label class="control-label">Client ID</label>
                                                                <asp:TextBox runat="server" ID="txtClientId" CssClass="form-control" MaxLength="150"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                    </div>

                                                    <div class="row">
                                                        <div class="col-sm-9 col-md-5 col-lg-3">
                                                            <div class="form-group">
                                                                <label class="control-label">Client Secret</label>
                                                                <asp:TextBox runat="server" ID="txtClientSecret" CssClass="form-control" MaxLength="150"></asp:TextBox>
                                                            </div>
                                                        </div>
                                                    </div>
                                                </div>
                                                <br />
                                                <p>
                                                    4 - Ingresa a <a href="https://www.mercadopago.com/mla/herramientas/notificaciones" target="_blank">www.mercadopago.com/mla/herramientas/notificaciones</a> y completa el campo Url con el siguiente valor: <strong style="color: #000">
                                                        <asp:Literal ID="liUrlMercadoPago" runat="server"></asp:Literal></strong>
                                                </p>
                                            </div>
                                        </div>

                                        <div class="panel-footer">
                                            <a class="btn btn-success" id="btnActualizarPortalClientes" onclick="MisDatos.portalClientes();">Actualizar</a>
                                        </div>
                                    </div>
                                </div>
                                <div class="tab-pane" id="template">
                                    <div class="panel-body">
                                        <h3>Tu imagen es todo</h3>

                                        <p>Elegí el diseño de factura que querés emitir para que se adapte a la imagen de tu empresa.</p>

                                        <div class="row mb15">
                                            <div class="row filemanager">
                                                <div class="col-xs-6 col-sm-4 col-md-2">
                                                    <div class="thmb">
                                                        <div class="thmb-prev">
                                                            <img src="/images/templates/1.png" class="img-responsive" alt="" />
                                                        </div>
                                                        <h5 class="fm-title">
                                                            <input type="radio" name="dlltemplate" runat="server" id="default1" value="default" checked="true" />
                                                            Default</h5>
                                                    </div>
                                                </div>
                                                <div class="col-xs-6 col-sm-4 col-md-2">
                                                    <div class="thmb">
                                                        <div class="thmb-prev">
                                                            <img src="/images/templates/2.png" class="img-responsive" alt="" />
                                                        </div>
                                                        <h5 class="fm-title">
                                                            <input type="radio" name="dlltemplate" runat="server" id="default2" value="amarillo" />
                                                            Amarillo</h5>

                                                    </div>
                                                </div>
                                                <div class="col-xs-6 col-sm-4 col-md-2">
                                                    <div class="thmb">
                                                        <div class="thmb-prev">
                                                            <img src="/images/templates/3.png" class="img-responsive" alt="" />
                                                        </div>
                                                        <h5 class="fm-title">
                                                            <input type="radio" name="dlltemplate" runat="server" id="default3" value="celeste" />
                                                            Celeste</h5>
                                                    </div>
                                                </div>
                                                <div class="col-xs-6 col-sm-4 col-md-2 hide">
                                                    <div class="thmb">
                                                        <div class="thmb-prev">
                                                            <img src="/images/templates/4.png" class="img-responsive" alt="" />
                                                        </div>
                                                        <h5 class="fm-title">
                                                            <input type="radio" name="dlltemplate" runat="server" id="default4" value="negro" />
                                                            Negro</h5>
                                                    </div>
                                                </div>
                                                <div class="col-xs-6 col-sm-4 col-md-2">
                                                    <div class="thmb">
                                                        <div class="thmb-prev">
                                                            <img src="/images/templates/5.png" class="img-responsive" alt="" />
                                                        </div>
                                                        <h5 class="fm-title">
                                                            <input type="radio" name="dlltemplate" runat="server" id="default5" value="rojo" />
                                                            Rojo</h5>

                                                    </div>
                                                </div>
                                                <div class="col-xs-6 col-sm-4 col-md-2">
                                                    <div class="thmb">
                                                        <div class="thmb-prev">
                                                            <img src="/images/templates/6.png" class="img-responsive" alt="" />
                                                        </div>
                                                        <h5 class="fm-title">
                                                            <input type="radio" name="dlltemplate" runat="server" id="default6" value="verde" />
                                                            Verde</h5>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="row mb15">
                                            <h3>¿Necesitás un diseño a medida? Consultános los costos!</h3>
                                        </div>
                                    </div>
                                    <div class="panel-footer">
                                        <a class="btn btn-success" id="btnActualizarTemplate" onclick="MisDatos.ActualizarTemplate();">Actualizar</a>
                                    </div>
                                </div>
                                <div class="tab-pane" id="datosFiscales" runat="server">

                                    <div class="panel panel-default">
                                        <div class="panel-body">

                                            <div class="row mb15">
                                                <div class="col-sm-12">
                                                    <h3>Puntos de venta </h3>
                                                    <label class="control-label">Estos son los Puntos de ventas habilitados para facturar.</label>
                                                </div>
                                            </div>
                                            <div id="puntos">
                                                <div class="alert alert-danger" id="divErrorPuntos" style="display: none">
                                                    <strong>Lo sentimos! </strong><span id="msgErrorPuntos"></span>
                                                </div>


                                                <div class="col-sm-2">
                                                    <div class="form-group">
                                                        <asp:TextBox runat="server" ID="txtNuevoPunto" CssClass="form-control" MaxLength="4"></asp:TextBox>
                                                    </div>
                                                </div>
                                                <div class="col-sm-2"><a class="btn btn-default" id="btnActualizarPunto" onclick="MisDatos.agregarPunto();">Agregar</a></div>

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

                                            <div class="row mb15">
                                                <div class="col-sm-12">
                                                    <h3>Soy agente de percepción </h3>
                                                    <hr />
                                                    <label class="control-label">
                                                        El agente de percepción recibe el importe correspondiente al tributo en el momento que el contribuyente paga la factura que se le extiende por la compra de un bien o la prestación de servicio.
                                                    </label>
                                                </div>
                                            </div>

                                            <div class="row mb15">
                                                <div class="col-sm-2">
                                                    <div class="form-group">
                                                        <asp:CheckBox runat="server" ID="esAgentePersepcionIVA"></asp:CheckBox><label class="control-label"> &nbsp;IVA</label>
                                                    </div>
                                                    <div class="form-group">
                                                        <asp:CheckBox runat="server" ID="esAgentePersepcionIIBB"></asp:CheckBox><label class="control-label"> &nbsp;IIBB</label>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="row mb15">
                                                <div class="col-sm-12">
                                                    <h3>Soy agente de retención de ganancias </h3>
                                                    <hr />
                                                </div>
                                            </div>
                                            <div class="row mb15">
                                                <div class="col-sm-6">
                                                    <div class="form-group">
                                                        <asp:CheckBox runat="server" ID="esAgenteRetencionGanancia"></asp:CheckBox><label class="control-label"> &nbsp; Soy agente de retención de ganancias</label>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="row mb15">
                                                <div class="col-sm-12">
                                                    <h3>Soy agente de retención </h3>
                                                    <hr />
                                                    <label class="control-label">
                                                        El agente de retención, casi siempre debe entregar o ser partícipe de alguna manera de la entrega de un monto destinado al contribuyente, del cual detrae, amputa o resta a dicho importe la parte que le corresponde al fisco en concepto de tributo.
                                                    </label>
                                                </div>
                                            </div>
                                            <div class="row mb15">
                                                <div class="col-sm-6">
                                                    <div class="form-group">
                                                        <asp:CheckBox runat="server" ID="esAgenteRetencion"></asp:CheckBox><label class="control-label"> &nbsp; Soy agente de retención</label>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row mb15">
                                                <div class="col-sm-6">
                                                    <div class="form-group">
                                                        <label class="control-label">Jurisdicción IIBB</label>
                                                        <select id="ddlJuresdiccion" class="select3" data-placeholder="Seleccione una jurisdicción" multiple="multiple">
                                                        </select>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row mb15">
                                                <div class="col-sm-12">
                                                    <h3>Fecha de cierre del ejercicio</h3>
                                                    <hr />
                                                    <label class="control-label">
                                                        Fecha en la cual se cierra el ejercicio contable de la empresa.
                                                    </label>
                                                </div>
                                            </div>
                                            <div>
                                                <div class="col-sm-2">
                                                    <div class="form-group">
                                                        <label class="control-label">Fecha</label>
                                                        <div class="input-group">
                                                            <span class="input-group-addon"><i class="glyphicon glyphicon-calendar"></i></span>
                                                            <asp:TextBox runat="server" ID="txtFechaCierreContable" CssClass="form-control validDate" MaxLength="10"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row mb15">
                                                <div class="col-sm-12">
                                                    <h3>Ingresos Brutos </h3>
                                                    <hr />
                                                </div>
                                            </div>
                                            <div class="row mb15">
                                                <div class="col-sm-12">
                                                   <div class="col-sm-3">
                                                        <div class="form-group">
                                                            <label class="control-label">IIBB</label>
                                                            <asp:CheckBox runat="server" ID="chkExento" CssClass="form-control" Text="&nbsp;Soy exento" onclick="MisDatos.changeIIBB();" Checked="false" Style="border: 0; background-color: transparent;"></asp:CheckBox>
                                                        </div>
                                                    </div>
                                                    <div class="col-sm-3">
                                                        <div class="form-group">
                                                            <label class="control-label">Número</label>
                                                            <asp:TextBox runat="server" ID="txtIIBB" CssClass="form-control number" MaxLength="15"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                            <hr />
                                            <div class="row mb15">
                                                <div class="col-sm-12">
                                                    <h3>Actividad </h3>
                                                    <label class="control-label"></label>
                                                </div>
                                            </div>
                                            <div id="actividades">
                                                <div class="alert alert-danger" id="divErrorActividades" style="display: none">
                                                    <strong>Lo sentimos! </strong><span id="msgErrorActividades"></span>
                                                </div>
                                                <div class="col-sm-2">
                                                    <div class="form-group">
                                                        <asp:TextBox runat="server" ID="txtNuevaActividad" CssClass="form-control" MaxLength="6"></asp:TextBox>
                                                    </div>
                                                </div>
                                                <div class="col-sm-2"><a class="btn btn-default" id="btnActualizarActividad" onclick="MisDatos.agregarActividad();">Agregar</a></div>

                                                <br />
                                                <br />
                                                <br />

                                                <div class="table-responsive">
                                                    <table class="table mb30">
                                                        <thead>
                                                            <tr>
                                                                <th>#</th>
                                                                <th>Codigo</th>
                                                                <th>Fecha alta</th>
                                                                <th>Fecha baja</th>
                                                                <th>Por defecto</th>
                                                                <th></th>
                                                            </tr>
                                                        </thead>
                                                        <tbody id="bodyDetalleActividades">
                                                        </tbody>
                                                    </table>
                                                </div>
                                            </div>
                                            <hr />
                                            <hr />
                                            <div class="row mb15">
                                                <div class="col-sm-12">
                                                    <h3>Factura Crédito Mí Pymes </h3>
                                                    <label class="control-label"></label>
                                                </div>
                                            </div>
                                            <div class="row mb15">
                                                <div class="col-sm-12">
                                                    <div class="col-sm-6">
                                                        <div class="form-group">
                                                            <label class="control-label">CBU</label>
                                                            <asp:TextBox runat="server" ID="txtCBU" CssClass="form-control number" MaxLength="22"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row mb15">
                                                <div class="col-sm-12">
                                                    <div class="col-sm-12">
                                                        <div class="form-group">
                                                            <label class="control-label">Texto al final de la factura</label>
                                                            <asp:TextBox runat="server" ID="txtTextoFinalFactura" CssClass="form-control"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                            <hr />
                                        </div>
                                        
                                        <div class="panel-footer">
                                            <a class="btn btn-success" id="btnActualizarDatosFiscales" onclick="MisDatos.grabar();">Actualizar</a>
                                        </div>
                                    </div>
                                </div>
                                <div class="tab-pane" id="plandeCuentas">
                                    <div class="alert alert-danger" id="divErrorPlanDeCuenta" style="display: none">
                                        <strong>Lo sentimos! </strong><span id="msgErrorPlanDeCuenta"></span>
                                    </div>
                                    <div class="alert alert-success" id="divOkPlanDeCuenta" style="display: none">
                                        <strong>Bien hecho! </strong>Los datos se han actualizado correctamente
                                    </div>
                                    <div class="panel panel-default">
                                        <div class="panel-body">

                                            <div class="row mb15">
                                                <div class="col-sm-12">
                                                    <hr />
                                                    <h3>Cuentas por defecto de comprobantes </h3>
                                                    <label class="control-label">
                                                        Son las cuentas en donde se registrarán todas las operaciones contables que se realizan mediante la compra de un bien o servicio. 
                                                    </label>
                                                </div>
                                            </div>
                                            <div class="row mb15">
                                                <div class="col-sm-3">
                                                    <div class="form-group">
                                                        <label class="control-label"><code>*</code> Proveedores</label>
                                                        <asp:DropDownList runat="server" ID="ddlCtaProveedoresComprobante" CssClass="select2" data-placeholder="Seleccione una cuenta...">
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>

                                                <div class="col-sm-3">
                                                    <div class="form-group">
                                                        <label class="control-label"><code>*</code> IVA Crédito Fiscal</label>
                                                        <asp:DropDownList runat="server" ID="ddlIVACreditoFiscalComprobante" CssClass="select2" data-placeholder="Seleccione una cuenta...">
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>

                                                <div class="col-sm-3">
                                                    <div class="form-group">
                                                        <label class="control-label"><code>*</code> Conceptos no gravados por compras</label>
                                                        <asp:DropDownList runat="server" ID="ddlNoGravadoCompras" CssClass="select2" data-placeholder="Seleccione una cuenta...">
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="row mb15">
                                                <div class="col-sm-12">
                                                    <hr />
                                                    <h3>Ventas</h3>
                                                    <label class="control-label">
                                                        Son las cuentas en donde se registrarán todas las operaciones contables que se realizan mediante la venta de un bien o servicio. 
                                                    </label>
                                                </div>
                                            </div>
                                            <div class="row mb15">
                                                <div class="col-sm-3">
                                                    <div class="form-group">
                                                        <label class="control-label"><code>*</code> Deudores por ventas</label>
                                                        <asp:DropDownList runat="server" ID="ddlDeudoresPorVenta" CssClass="select2" data-placeholder="Seleccione una cuenta...">
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>
                                                <div class="col-sm-3">
                                                    <div class="form-group">
                                                        <label class="control-label"><code>*</code> IVA Débito Fiscal</label>
                                                        <asp:DropDownList runat="server" ID="ddlIVADebitoFiscal" CssClass="select2" data-placeholder="Seleccione una cuenta...">
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>
                                                <div class="col-sm-3">
                                                    <div class="form-group">
                                                        <label class="control-label"><code>*</code> Conceptos no gravados por ventas</label>
                                                        <asp:DropDownList runat="server" ID="ddlNoGravadoVentas" CssClass="select2" data-placeholder="Seleccione una cuenta...">
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="row mb15">
                                                <div class="col-sm-12">
                                                    <hr />
                                                    <h3>Percepciones</h3>
                                                    <label class="control-label">
                                                        Son las cuentas en donde se registrarán todas las operaciones contables relacionadas a percepciones. 
                                                    </label>
                                                </div>
                                            </div>

                                            <div class="row mb15">
                                                <div class="col-sm-3">
                                                    <div class="form-group">
                                                        <label class="control-label"><code>*</code> IIBB</label>
                                                        <asp:DropDownList runat="server" ID="ddlCtaIIBBComprobante" CssClass="select2" data-placeholder="Seleccione una cuenta...">
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>
                                                <div class="col-sm-3">
                                                    <div class="form-group">
                                                        <label class="control-label"><code>*</code> Percepción IVA </label>
                                                        <asp:DropDownList runat="server" ID="ddlPercepcionIVAComprobante" CssClass="select2" data-placeholder="Seleccione una cuenta...">
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>
                                            </div>

                                            <div class="row mb15">
                                                <div class="col-sm-12">
                                                    <hr />
                                                    <h3>Pagos y cobranzas</h3>
                                                    <label class="control-label">
                                                        Son las cuentas en donde se registrarán todas las operaciones contables que se realizan mediante el pago o cobranza de un comprobante. 
                                                    </label>
                                                </div>
                                            </div>
                                            <div class="row mb15">
                                                <div class="col-sm-3">
                                                    <div class="form-group">
                                                        <label class="control-label"><code>*</code> Valores a depositar</label>
                                                        <asp:DropDownList runat="server" ID="ddlValoresADepositar" CssClass="select2" data-placeholder="Seleccione una cuenta...">
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>
                                                <div class="col-sm-3">
                                                    <div class="form-group">
                                                        <label class="control-label"><code>*</code> Caja</label>
                                                        <asp:DropDownList runat="server" ID="ddlCaja" CssClass="select2" data-placeholder="Seleccione una cuenta...">
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>
                                                <div class="col-sm-3">
                                                    <div class="form-group">
                                                        <label class="control-label"><code>*</code> Bancos</label>
                                                        <asp:DropDownList runat="server" ID="ddlBanco" CssClass="select2" data-placeholder="Seleccione una cuenta...">
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row mb15">
                                                <div class="col-sm-3">
                                                    <div class="form-group">
                                                        <label class="control-label"><code>*</code> Ret. emitidas de IIBB</label>
                                                        <asp:DropDownList runat="server" ID="ddlRetIIBB" CssClass="select2" data-placeholder="Seleccione una cuenta...">
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>
                                                <div class="col-sm-3">
                                                    <div class="form-group">
                                                        <label class="control-label"><code>*</code> Ret. emitidas de IVA</label>
                                                        <asp:DropDownList runat="server" ID="ddlRetIVA" CssClass="select2" data-placeholder="Seleccione una cuenta...">
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>
                                                <div class="col-sm-3">
                                                    <div class="form-group">
                                                        <label class="control-label"><code>*</code> Ret. emitidas de imp. a las ganancias</label>
                                                        <asp:DropDownList runat="server" ID="ddlRetGanancias" CssClass="select2" data-placeholder="Seleccione una cuenta...">
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row mb15">
                                                <div class="col-sm-3">
                                                    <div class="form-group">
                                                        <label class="control-label"><code>*</code> Ret. emitidas de SUSS</label>
                                                        <asp:DropDownList runat="server" ID="ddlRetSUSS" CssClass="select2" data-placeholder="Seleccione una cuenta...">
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row mb15">
                                                <div class="col-sm-12">
                                                    <hr />
                                                    <h3>Filtros de busqueda</h3>
                                                    <label class="control-label">
                                                        Agregue aqui las cuentas que apareceran para elejir en las pantallas de compra y venta.
                                                    </label>
                                                </div>
                                            </div>
                                            <div class="row mb15">
                                                <div class="col-sm-12">
                                                    <div class="form-group">
                                                        <label class="control-label"><code>*</code> Compras</label>
                                                        <asp:DropDownList runat="server" ID="ddlCuentasCompras" CssClass="select2" data-placeholder="Seleccione una cuenta..." multiple>
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>
                                                <div class="col-sm-12">
                                                    <div class="form-group">
                                                        <label class="control-label"><code>*</code> Ventas</label>
                                                        <asp:DropDownList runat="server" ID="ddlCuentasVentas" CssClass="select2" data-placeholder="Seleccione una cuenta..." multiple>
                                                        </asp:DropDownList>
                                                    </div>
                                                </div>
                                            </div>
                                            <input runat="server" id="hdnCuentasCompras" value="0" type="hidden" />
                                            <input runat="server" id="hdnCuentasVentas" value="0" type="hidden" />
                                        </div>
                                        <div class="panel-footer">
                                            <a class="btn btn-success" id="A2" onclick="MisDatos.guardarConfiguracionPlanDeCuenta();">Actualizar</a>
                                        </div>
                                    </div>
                                </div>
                                <div class="tab-pane" id="configuracion">
                                    <div class="alert alert-danger" id="divErrorConfiguracion" style="display: none">
                                        <strong>Lo sentimos! </strong><span id="msgErrorConfiguracion"></span>
                                    </div>
                                    <div class="alert alert-success" id="divOkConfiguracion" style="display: none">
                                        <strong>Bien hecho! </strong>Los datos se han actualizado correctamente
                                    </div>
                                    <div class="panel panel-default">
                                        <div class="panel-body">
                                            <div class="row mb15">
                                                <div class="col-sm-6">
                                                    <div class="form-group">
                                                        <h3>¿Desea usar el precio unitario con el IVA incluído por defecto?</h3>
                                                        <input type="radio" name="rPrecioUnitario" runat="server" id="rPrecioUnitarioConIVA" value="1" />
                                                        Si, usar el precio unitario con IVA incluído.<br />
                                                        <input type="radio" name="rPrecioUnitario" runat="server" id="rPrecioUnitarioSinIVA" value="0" />
                                                        No, usar el precio unitario sin incluir IVA.
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row mb15" id="divPedidoDeVentaDefecto" runat="server" visible="false">
                                                <div class="col-sm-6">
                                                    <div class="form-group">
                                                        <h3>¿Pedido de venta por defecto?</h3>
                                                        <input type="radio" name="rPedidoDeVenta" runat="server" id="rPedidoDeVentaDefectoSi" value="1" />
                                                        Si.<br />
                                                        <input type="radio" name="rPedidoDeVenta" runat="server" id="rPedidoDeVentaDefectoNo" value="0" />
                                                        No.
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row mb15" id="divParaPDVSolicitarCompletarContacto" runat="server">
                                                <div class="col-sm-6">
                                                    <div class="form-group">
                                                        <h3>¿Cargar Pedidos de Ventas, solo para clientes que tenga sus datos de contactos completos?</h3>
                                                        <input type="radio" name="rParaPDVSolicitarCompletarContacto" runat="server" id="rParaPDVSolicitarCompletarContactoSi" value="1" />
                                                        Si.<br />
                                                        <input type="radio" name="rParaPDVSolicitarCompletarContacto" runat="server" id="rParaPDVSolicitarCompletarContactoNo" value="0" />
                                                        No.
                                                    </div>
                                                </div>
                                            </div>
                                            <div class="row mb15">
                                                <div class="col-sm-12">
                                                    <h3>Comisiones </h3>
                                                    <hr />
                                                </div>
                                            </div>
                                            <div class="row mb15">
                                                <div class="col-sm-12">
                                                   <div class="col-sm-3">
                                                        <div class="form-group">
                                                            <label class="control-label">Es Vendedor</label>
                                                            <asp:CheckBox runat="server" ID="chkEsVendedor" CssClass="form-control" Text="&nbsp;Soy Vendedor"  Checked="false" Style="border: 0; background-color: transparent;"></asp:CheckBox>
                                                        </div>
                                                    </div>
                                                    <div class="col-sm-3">
                                                        <div class="form-group">
                                                            <label class="control-label">Porcentaje Comisión</label>
                                                            <asp:TextBox runat="server" ID="txtPorcentajeComision" CssClass="form-control number" MaxLength="15"></asp:TextBox>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                            <hr />
                                            <div class="row mb15">
                                                <div class="col-sm-12">
                                                   <div class="col-sm-3">
                                                        <div class="form-group">
                                                            <label class="control-label">Factura solo contra entrega</label>
                                                            <asp:CheckBox runat="server" ID="chkFacturaSoloContraEntrega" CssClass="form-control" Text="&nbsp;Factura solo contra entrega"  Checked="false" Style="border: 0; background-color: transparent;"></asp:CheckBox>
                                                        </div>
                                                    </div>

                                                </div>
                                            </div>
                                            <hr />
                                            <div class="row mb15">
                                                <div class="col-sm-12">
                                                     <div class="col-sm-3">
                                                        <div class="form-group">
                                                            <label class="control-label">Usa cantidad con decimales</label>
                                                            <asp:CheckBox runat="server" ID="chkUsaCantidadConDecimales" CssClass="form-control" Text="&nbsp;Usa cantidad con decimales"  Checked="false" Style="border: 0; background-color: transparent;"></asp:CheckBox>
                                                        </div>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="panel-footer">
                                            <a class="btn btn-success" id="A1" onclick="MisDatos.guardarConfiguracion();">Actualizar</a>
                                        </div>
                                    </div>
                                </div>

                            </div>


                            <asp:HiddenField runat="server" ID="hdnJuresdiccion" Value="0" />
                            <asp:HiddenField runat="server" ID="hdnTieneFoto" Value="0" />
                            <asp:HiddenField runat="server" ID="hdnSinCombioDeFoto" Value="0" />
                            <asp:HiddenField runat="server" ID="hdnIdPlan" Value="0" />
                            <asp:HiddenField runat="server" ID="hdnAccion" Value="0" />
                            <asp:HiddenField runat="server" ID="hdnCiudad" Value="0" />
                            <asp:HiddenField runat="server" ID="hdnProvincia" Value="1" />
                            <input type="hidden" name="IDusuario" id="IDusuario" value="" runat="server" />

                        </div>
                        <div class="tab-pane" id="planPagos" runat="server">
                            <div class="panel panel-default">
                                <div class="panel-body">
                                    <div class="row mb15">
                                        <div class="col-sm-12">
                                            <h3>Tu plan actual es: <span id="spNombrePlan"></span></h3>
                                            <label class="control-label">La fecha de vencimiento es: <span id="spFechaPlan"></span></label>
                                        </div>
                                    </div>

                                    <div class="row mb15">

                                        <div class="col-sm-12">
                                            <a href="#" class="btn btn-success" onclick="MisDatos.upgradePlanActual();">Cambiar de plan</a>
                                            <a href="#" class="btn btn-success" onclick="MisDatos.upgradePlanActual();">Informar pago</a>
                                        </div>

                                    </div>

                                    <div class="row mb15">
                                        <div class="col-sm-12">
                                            <hr />
                                            <h3>Historial de pagos </h3>
                                            <label class="control-label">Estos son todos los pagos registrados en el sistema.</label>
                                        </div>
                                    </div>

                                    <div class="table-responsive">
                                        <table class="table mb30">
                                            <thead>
                                                <tr>
                                                    <th>Tipo de plan</th>
                                                    <th>Fecha Inicio</th>
                                                    <th>Fecha Vencimiento</th>
                                                    <th>Foma de pago</th>
                                                    <th>Nro referencia</th>
                                                    <th>Importe pagado</th>
                                                    <th>Fecha de pago</th>
                                                    <th>Estado</th>
                                                </tr>
                                            </thead>
                                            <tbody id="resultsContainer">
                                            </tbody>
                                        </table>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="tab-pane" id="Empresas" runat="server">
                            <div class="panel panel-default">
                                <div class="panel-body">
                                    <div class="row mb15">
                                        <div class="col-sm-5">
                                            <h3>Seleccione el CUIT con el que desea operar</h3>
                                        </div>
                                        <div class="col-sm-7" id="btnNuevo" runat="server" style="margin-top: 10px">
                                            <button class="btn btn-warning" onclick="Empresa.nuevo();" type="button"><i class="fa fa-plus mr5"></i>Agregar CUIT</button>
                                        </div>
                                        <div class="col-sm-7" style="margin-top: 15px; font-style: italic;">
                                            <h5 id='spMsgCantEmpresas' style="display: none">Superó el máximo de empresas permitidas. Si desea obtener más por favor envíe por correo a axan.sistemas@gmail.com.
                                            </h5>
                                        </div>
                                    </div>

                                    <hr />
                                    <div class="row">
                                        <div class="people-list">
                                            <div class="row" id="resultsContainerEmpresas">
                                            </div>
                                        </div>
                                    </div>

                                </div>
                            </div>
                            <input type="hidden" name="name" value="" id="IDUsuarioAdicional" runat="server" />
                            <input type="hidden" name="name" value="" id="IDUsuarioActual" runat="server" />
                        </div>
                        <div class="tab-pane" id="AlertasyAvisos" runat="server">
                            <div class="alert alert-danger" id="divErrorAlertasyAvisos" style="display: none">
                                <strong>Lo sentimos! </strong><span id="msgErrorAlertasyAvisos"></span>
                            </div>
                            <div class="alert alert-success" id="divOkAlertasyAvisos" style="display: none">
                                <strong>Bien hecho! </strong>Los datos se han actualizado correctamente
                            </div>
                            <div class="panel panel-default">
                                <div class="panel-body">
                                    <div class="row mb15">
                                        <div class="col-sm-12">
                                            <h3>Envio automático de comprobantes electrónicos (VÁLIDO para plan PYME en adelante)</h3>
                                            <label class="control-label">
                                                Cada vez que genere un comprobante electrónico, una cobranza o un abono se enviará automáticamente un correo con el comprobante Adjunto y un mensaje configurable desde aquí.
                                            </label>
                                            <hr />
                                        </div>
                                    </div>

                                    <div class="row mb15">
                                        <div class="col-sm-12">
                                            <div class="col-sm-2" style="text-align: right;">
                                                <label>
                                                    <input type="checkbox" id="chkEnvioFE" onclick="MisDatos.habilitarEnvioFE();" /><span style="margin-right: -114px; display: inline-block;">&nbsp;Enviar comprobante automáticamente</span></label>
                                            </div>
                                            <div class="divEnvioFE col-sm-10">
                                                <div class="col-sm-5"></div>
                                                <div class="col-sm-3">
                                                    <input id="btnEnvioFE" class="btn btn-success btn-sm" onclick="MisDatos.configurarMensaje('divEnvioFE');" value="Configurar mensaje" />
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="row mb15">
                                        <div class="col-sm-12">
                                            <div class="col-sm-2" style="text-align: right;">
                                                <label>
                                                    <input type="checkbox" id="chkEnvioCR" onclick="MisDatos.habilitarEnvioCR();" /><span style="margin-right: -69px; display: inline-block;">&nbsp;Enviar recibo automáticamente</span></label>
                                            </div>
                                            <div class="divEnvioCR col-sm-10">
                                                <div class="col-sm-5"></div>
                                                <div class="col-sm-3">
                                                    <input id="btnEnvioCR" class="btn btn-success btn-sm" onclick="MisDatos.configurarMensaje('divEnvioCR');" value="Configurar mensaje" />
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="row mb15">
                                        <div class="col-sm-12">
                                            <h3>Avisos de vencimiento para sus facturas de ventas (VÁLIDO para plan EMPRESA en adelante)</h3>
                                            <label class="control-label">
                                                axanweb enviará a sus clientes un email avisando la proximidad del vencimiento de las facturas con su número, 
                                                        saldo y fecha de vencimiento de acuerdo a los días de aviso que se configuren aquí. 
                                            </label>
                                            <hr />
                                        </div>
                                    </div>
                                    <div class="row mb15" style="display: -webkit-box;">
                                        <div class="col-sm-12">
                                            <div class="col-sm-2" style="text-align: right;">
                                                <label>
                                                    <input type="checkbox" id="chkPrimerAviso" onclick="MisDatos.habilitarPrimerAviso();" /><span style="margin-right: 29px;">&nbsp;1er. Aviso: Días</span></label>
                                            </div>


                                            <div class="divPrimerAviso col-sm-10">
                                                <div class="col-sm-4">
                                                    <label>
                                                        <input type="radio" name="rPrimerAviso" runat="server" id="rPrimerAvisoAntes" value="Antes" /><span style="margin-right: 10px;">Antes</span></label>
                                                    <label>
                                                        <input type="radio" name="rPrimerAviso" runat="server" id="rPrimerAvisoDespues" value="Despues" /><span style="margin-right: 10px;">Después</span></label>

                                                    <span>del vencimiento</span>
                                                </div>
                                                <div class="col-sm-1" style="min-width: 100px; margin-left: -20px;">
                                                    <input id="txtDiasPrimerAviso" class="col-sm-1 form-control" maxlength="2" placeholder="Cant días" />
                                                </div>
                                                <div class="col-sm-2">
                                                    <input id="btnPrimerAviso" class="btn btn-success btn-sm" onclick="MisDatos.configurarMensaje('divPrimerAviso');" value="Configurar mensaje" />
                                                </div>
                                            </div>


                                        </div>
                                    </div>
                                    <div class="row mb15">
                                        <div class="col-sm-12">
                                            <div class="col-sm-2" style="text-align: right;">
                                                <label>
                                                    <input type="checkbox" id="chkSegundoAviso" onclick="MisDatos.habilitarSegundoAviso();" /><span style="margin-right: 29px;">&nbsp;2er. Aviso: Días</span></label>
                                            </div>
                                            <div class="divSegundoAviso col-sm-10">
                                                <div class="col-sm-4">
                                                    <label>
                                                        <input type="radio" name="rSegundoAviso" runat="server" id="rSegundoAvisoAntes" value="Antes" /><span style="margin-right: 10px;">Antes</span></label>

                                                    <label>
                                                        <input type="radio" name="rSegundoAviso" runat="server" id="rSegundoAvisoDespues" value="Despues" /><span style="margin-right: 10px;">Después</span></label>
                                                    <span>del vencimiento</span>
                                                </div>
                                                <div class="col-sm-1" style="min-width: 100px; margin-left: -20px;">
                                                    <input id="txtDiasSegundoAviso" class="col-sm-1 form-control" maxlength="2" placeholder="Cant días" />
                                                </div>
                                                <div class="col-sm-3">
                                                    <input id="btnSegundoAviso" class="btn btn-success btn-sm" onclick="MisDatos.configurarMensaje('divSegundoAviso');" value="Configurar mensaje" />
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row mb15">
                                        <div class="col-sm-12">
                                            <div class="col-sm-2" style="text-align: right;">
                                                <label>
                                                    <input type="checkbox" id="chkTercerAviso" onclick="MisDatos.habilitarTercerAviso();" /><span style="margin-right: 29px;">&nbsp;3er. Aviso: Días</span></label>
                                            </div>
                                            <div class="divTercerAviso col-sm-10">
                                                <div class="col-sm-4">
                                                    <label>
                                                        <input type="radio" name="rTercerAviso" runat="server" id="rTercerAvisoAntes" value="Antes" /><span style="margin-right: 10px;">Antes</span></label>
                                                    <label>
                                                        <input type="radio" name="rTercerAviso" runat="server" id="rTercerAvisoDespues" value="Despues" /><span style="margin-right: 10px;">Después</span></label>
                                                    <span>del vencimiento</span>
                                                </div>
                                                <div class="col-sm-1" style="min-width: 100px; margin-left: -20px;">
                                                    <input id="txtDiasTercerAviso" class="col-sm-1 form-control" maxlength="2" placeholder="Cant días" />
                                                </div>
                                                <div class="col-sm-3">
                                                    <input id="btnTercerAviso" class="btn btn-success btn-sm" onclick="MisDatos.configurarMensaje('divTercerAviso');" value="Configurar mensaje" />
                                                </div>
                                            </div>
                                        </div>
                                    </div>

                                    <div class="row mb15">
                                        <div class="col-sm-12">
                                            <h3>Alertas de stock (VÁLIDO para plan PYME en adelante)</h3>
                                            <label class="control-label">
                                                axanweb le enviará un email avisando que su producto llego al limite de stock configurado. 
                                            </label>
                                            <hr />
                                        </div>
                                    </div>

                                     <div class="row mb15">
                                        <div class="col-sm-12">
                                            <div class="col-sm-2" style="text-align: right;">
                                                <label>
                                                    <input type="checkbox" id="chkStock" /><span style="margin-right: -213px; display: inline-block;">&nbsp;Enviar alerta cuando el stock sea igual al stock minimo</span></label>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="panel-footer">
                                    <div class="col-sm-12" style="margin-left: -62px">
                                        <div class="col-sm-2" style="text-align: right;">
                                            <a class="btn btn-success" id="btnAlertasyAvisos" onclick="MisDatos.guardarAlertasyAvisos();">Actualizar</a>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </form>
            </div>
            <!-- col-sm-9 -->
        </div>
        <!-- row -->
    </div>

    <!-- MODAL -->
    <div class="modal modal-wide fade" id="modalAlertasYAvisos" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
        <div class="modal-dialog">
            <div class="contentpanel">
                <form id="frmNuevaAlertasyAvisos" class="col-sm-12">
                    <div class="panel panel-default">
                        <div class="modal-header">
                            <h4 class="modal-title" id="litModalOkTitulo"></h4>
                        </div>
                        <div class="panel-body" id="modalPrimerAviso">
                            <div class="row mb15">
                                <div class="col-sm-12">
                                    <div class="form-group">
                                        <label class="control-label"><span class="asterisk">*</span> Asunto</label>
                                        <input id="txtAsuntoPrimerAviso" class="form-control required" maxlength="150" />
                                    </div>
                                </div>

                                <div class="col-sm-12">
                                    <div class="form-group">
                                        <label class="control-label"><span class="asterisk">*</span> Mensaje</label>
                                        <textarea rows="5" id="txtMensajePrimerAviso" class="form-control required"></textarea>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="panel-body" id="modalSegundoAviso">
                            <div class="row mb15">
                                <div class="col-sm-12">
                                    <div class="form-group">
                                        <label class="control-label"><span class="asterisk">*</span> Asunto</label>
                                        <input id="txtAsuntoSegundoAviso" class="form-control required" maxlength="150" />
                                    </div>
                                </div>

                                <div class="col-sm-12">
                                    <div class="form-group">
                                        <label class="control-label"><span class="asterisk">*</span> Mensaje</label>
                                        <textarea rows="5" id="txtMensajeSegundoAviso" class="form-control required"></textarea>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="panel-body" id="modalTercerAviso">
                            <div class="row mb15">
                                <div class="col-sm-12">
                                    <div class="form-group">
                                        <label class="control-label"><span class="asterisk">*</span> Asunto</label>
                                        <input id="txtAsuntoTercerAviso" class="form-control required" maxlength="150" />
                                    </div>
                                </div>

                                <div class="col-sm-12">
                                    <div class="form-group">
                                        <label class="control-label"><span class="asterisk">*</span> Mensaje</label>
                                        <textarea rows="5" id="txtMensajeTercerAviso" class="form-control required"></textarea>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="panel-body" id="modalEnvioFE">
                            <div class="row mb15">
                                <div class="col-sm-12">
                                    <div class="form-group">
                                        <label class="control-label"><span class="asterisk">*</span> Asunto</label>
                                        <input id="txtAsuntoEnvioFE" class="form-control required" maxlength="150" />
                                    </div>
                                </div>

                                <div class="col-sm-12">
                                    <div class="form-group">
                                        <label class="control-label"><span class="asterisk">*</span> Mensaje</label>
                                        <textarea rows="5" id="txtMensajeEnvioFE" class="form-control required"></textarea>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="panel-body" id="modalEnvioCR">
                            <div class="row mb15">
                                <div class="col-sm-12">
                                    <div class="form-group">
                                        <label class="control-label"><span class="asterisk">*</span> Asunto</label>
                                        <input id="txtAsuntoEnvioCR" class="form-control required" maxlength="150" />
                                    </div>
                                </div>

                                <div class="col-sm-12">
                                    <div class="form-group">
                                        <label class="control-label"><span class="asterisk">*</span> Mensaje</label>
                                        <textarea rows="5" id="txtMensajeEnvioCR" class="form-control required"></textarea>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="panel-footer">
                            <a class="btn btn-success" id="btnActualizar" onclick="MisDatos.guardarAsuntoYMensaje();">Aceptar</a>
                            <a href="#" onclick="MisDatos.cancelarAsuntoYMensaje();" tabindex="14" style="margin-left: 20px">Cerrar</a>
                        </div>
                    </div>
                    <input id="hdnSaldoTotalActual" type="hidden" value="0" />
                    <input id="hdnID" type="hidden" value="0" />
                </form>
            </div>
        </div>
    </div>
    <div class="modal modal-wide fade" id="modalNuevaEmpresa" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
        <div class="modal-dialog">
            <div class="contentpanel">
                <form id="frmNuevaEmpresa" class="col-sm-12" onsubmit="return false;">
                    <div class="row mb15">
                        <div class="panel panel-default">

                            <div class="panel-body">
                                <div class="alert alert-danger" id="divErrorEmpresa" style="display: none">
                                    <strong>Lo sentimos! </strong><span id="msgErrorEmpresa"></span>
                                </div>
                                <div class="alert alert-success" id="divOkEmpresa" style="display: none">
                                    <span id="msgOkEmpresa"></span>
                                </div>

                                <div class="row mb15">
                                    <div class="col-sm-12">
                                        <h3>Datos principales </h3>
                                        <label class="control-label">Estos son los datos básicos y obligatorios que el sistema necesita para poder facturar.</label>
                                    </div>
                                </div>

                                <div class="row mb15">
                                    <div class="col-sm-6">
                                        <div class="form-group">
                                            <label class="control-label"><span class="asterisk">*</span> CUIT</label>
                                            <input id="txtCuitEmpresa" type="text" class="form-control required number validCuit" maxlength="13" tabindex="2" />
                                        </div>
                                    </div>
                                    <div class="col-sm-6">
                                        <div class="form-group">
                                            <label class="control-label"><span class="asterisk">*</span> Razón Social</label>
                                            <input type="text" id="txtRazonSocialEmpresa" name="txtRazonSocial" class="form-control" maxlength="128" tabindex="1" />
                                        </div>
                                    </div>
                                </div>
                                <div class="row mb15" id="divBuscarDatosAfip">
                                    <div class="col-sm-6" >
                                        <div class="form-group">
                                            <input type="image" src="../../images/icoTraerDatosAfip.png" name="btnTraerDatosAfip" style="height:auto;max-width:100%" id="btnTraerDatosAfip" value="Traer datos AFIP" tabindex="3" onclick="Empresa.consultarDatosAfip();"/>                                    
                                        </div>
                                    </div>   
                                    <div class="col-sm-6">
                                        <div class="form-group">
                                            <div id="divProcesando" hidden="hidden"><img src="../../images/loaders/loader1.gif" /></div>
                                        </div>
                                    </div>                             
                                </div>
                                <div class="row mb15">
                                    <div class="col-sm-6">
                                        <div class="form-group">
                                            <label class="control-label"><span class="asterisk">*</span> Categoría Impositiva</label>
                                            <select id="ddlCondicionIvaEmpresa" class="form-control required" tabindex="3">
                                                <option value=""></option>
                                                <option value="RI">Responsable Inscripto</option>
                                                <%--<option value="CF">Consumidor Final</option>--%>
                                                <option value="MO">Monotributista</option>
                                                <option value="EX">Exento</option>
                                            </select>
                                        </div>
                                    </div>
                                    <div class="col-sm-6">
                                        <div class="form-group">
                                            <label class="control-label"><span class="asterisk">*</span> Personería</label>
                                            <select id="ddlPersoneriaEmpresa" class="form-control required" tabindex="4">
                                                <option value="F">Física</option>
                                                <option value="J">Jurídica</option>
                                            </select>
                                        </div>
                                    </div>
                                </div>
                                <div class="row mb15">
                                    <div class="col-sm-6">
                                        <div class="form-group">
                                            <label class="control-label"><code>*</code> E-mail</label>
                                            <div class="input-group">
                                                <span class="input-group-addon">@</span>
                                                <input type="email" id="txtEmailEmpresa" class="form-control required" maxlength="128" tabindex="5" />
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-sm-6">
                                        <div class="form-group">
                                            <label class="control-label"><code>*</code> Password</label>
                                            <input runat="server" id="txtPwdEmpresa" type="password" class="form-control validPassword" maxlength="20" autocomplete="off" />
                                        </div>
                                    </div>
                                </div>

                                <div class="row mb15">
                                    <div class="col-sm-12">
                                        <hr />
                                        <h3>Domicilio Fiscal </h3>
                                        <label class="control-label">Estos son los datos básicos y obligatorios que el sistema necesita para poder facturar.</label>
                                    </div>
                                </div>

                                <div class="row mb15">
                                    <div class="col-sm-6">
                                        <div class="form-group">
                                            <label class="control-label"><span class="asterisk">*</span> Provincia</label>
                                            <select id="ddlProvinciaEmpresa" class="select2 required" data-placeholder="Selecciona una provincia..." tabindex="6" onchange="Empresa.changeProvincia();">
                                            </select>
                                        </div>
                                    </div>
                                    <div class="col-sm-6">
                                        <div class="form-group">
                                            <label class="control-label"><span class="asterisk">*</span> Ciudad/Localidad</label>
                                            <select id="ddlCiudadEmpresa" class="select2 required" data-placeholder="Selecciona una ciudad...">
                                            </select>
                                        </div>
                                    </div>
                                </div>
                                <div class="row mb15">
                                    <div class="col-sm-9">
                                        <div class="form-group">
                                            <label class="control-label"><span class="asterisk">*</span> Domicilio</label>
                                            <input type="text" id="txtDomicilioEmpresa" class="form-control required" maxlength="100" tabindex="7" />
                                        </div>
                                    </div>
                                    <div class="col-sm-3">
                                        <div class="form-group">
                                            <label class="control-label">Piso/Depto</label>
                                            <input type="text" id="txtPisoDeptoEmpresa" class="form-control" maxlength="10" tabindex="8" />
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="panel-footer">
                                <input type="button" class="btn btn-primary" value="Crear empresa" onclick="Empresa.grabar();" tabindex="9" id="btnCrearEmpresa" />
                                <a style="margin-left: 20px" href="#" onclick="$('#modalNuevaEmpresa').modal('toggle');" tabindex="10">Cancelar</a>
                            </div>
                        </div>
                    </div>
                </form>
            </div>
        </div>
    </div>

    <script id="resultTemplate" type="text/x-jQuery-tmpl">
        {{each results}}
        <tr>
            <td>${TipoDePlan}</td>
            <td>${FechaInicio}</td>
            <td>${FechaVencimiento}</td>
            <td>${FomaDePago}</td>
            <td>${NroReferencia}</td>
            <td>${ImportePagado}</td>
            <td>${FechaDePago}</td>
            <td>${Estado}</td>
        </tr>
        {{/each}}
    </script>
    <script id="noResultTemplate" type="text/x-jQuery-tmpl">
        <tr>
            <td colspan="8">No se han encontrado resultados</td>
        </tr>
    </script>
    <!--Empresas-->
    <script id="resultTemplateEmpresas" type="text/x-jQuery-tmpl">
        {{each results}}
            <div class="col-md-6">
                <div class="people-item">
                    <div class="media">
                        <a href="#" class="pull-left">
                            <img alt="" src="${Logo}" class="thumbnail media-object" />
                        </a>
                        <div class="media-body">
                            <h4 class="person-name">${RazonSocial}</h4>
                            <div class="text-muted"><i class="fa fa-map-marker"></i>${Domicilio}, ${Ciudad}, ${Provincia}</div>
                            <div class="text-muted"><i class="fa fa-briefcase"></i>${CUIT}, ${CondicionIva}</div>
                            <ul class="social-list">
                                {{if ID==$("#IDUsuarioActual").val()}}
                                    <li><a class="btn btn-default" style="width: 100% !important; text-align: left; background-color: #ccc">Usted se encuentra usando ${RazonSocial}</a></li>
                                {{else}}
                                    <li><a onclick="Empresa.cambiarSesion(${ID},'${RazonSocial}');" title="Acceder" class="btn btn-success" style="width: 100% !important; text-align: left;"><i class="fa  fa-sign-in"></i>Usar axanweb como ${RazonSocial}</a></li>
                                {{/if}}
                            </ul>
                        </div>
                    </div>
                </div>
            </div>
        {{/each}}
    </script>
    <script id="noResultTemplateEmpresas" type="text/x-jQuery-tmpl">
        <h4>No se han encontrado otras empresas</h4>
    </script>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FooterContent" runat="Server">
    <script type="text/javascript" src="/js/jquery.tmpl.min.js"></script>
    <script src="/js/select2.min.js"></script>
    <script src="/js/jquery.validate.min.js"></script>
    <script src="/js/jquery.ui.widget.js" type="text/javascript"></script>
    <script src="/js/jquery.iframe-transport.js" type="text/javascript"></script>
    <script src="/js/jquery.fileupload.js" type="text/javascript"></script>
    <script src="/js/jquery.maskedinput.min.js"></script>
    <script src="/js/jquery.maskMoney.min.js"></script>
    <script src="/js/views/seguridad/mis-datos.js?v=<%= ConfigurationManager.AppSettings["JS.Version"] %>"></script>
    <script>
        jQuery(document).ready(function () {
            MisDatos.configForm();
            MisDatos.obtenerPuntos();
            MisDatos.obtenerActividades();

            //EMPRESAS
            Empresa.configForm();
            Empresa.filtrar();

            $('#modalNuevaEmpresa').on('hidden.bs.modal', function (e) {
                Empresa.limpiarControlesEmpresa();
            });
        });
    </script>


</asp:Content>
