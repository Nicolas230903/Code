<%@ Page Title="" Language="C#" MasterPageFile="~/Front.master" AutoEventWireup="true" CodeFile="personase.aspx.cs" Inherits="personase" %>

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
                        <div class="row hide" id="divStats">
                            <div class="col-sm-3 col-md-3">
                                <div class="panel panel-success panel-stat">
                                    <div class="panel-heading">

                                        <div class="stat">
                                            <div class="row">
                                                <div class="col-xs-4">
                                                    <i class="fa fa-thumbs-up" style="font-size: 30px; opacity: 1; border: none; padding: 0"></i>
                                                </div>
                                                <div class="col-xs-8">
                                                    <small class="stat-label">Importe facturado</small>
                                                    <h4>$
                                                        <asp:Literal runat="server" ID="litImporteFacturado"></asp:Literal></h4>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="col-sm-3 col-md-3">
                                <div class="panel panel-info panel-stat">
                                    <div class="panel-heading">
                                        <div class="stat">
                                            <div class="row">
                                                <div class="col-xs-4">
                                                    <i class="fa fa-thumbs-down" style="font-size: 30px; opacity: 1; border: none; padding: 0"></i>
                                                </div>
                                                <div class="col-xs-8">
                                                    <small class="stat-label">Importe pagado</small>
                                                    <h4>$
                                                        <asp:Literal runat="server" ID="litImportePagado"></asp:Literal></h4>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                            </div>
                            <div class="col-sm-3 col-md-3">
                                <div class="panel panel-danger panel-stat">
                                    <div class="panel-heading">
                                        <div class="stat">
                                            <div class="row">
                                                <div class="col-xs-4">
                                                    <i class="glyphicon glyphicon-fire" style="font-size: 30px; opacity: 1; border: none; padding: 0"></i>
                                                </div>
                                                <div class="col-xs-8">
                                                    <small class="stat-label">Saldo pendiente</small>
                                                    <h4>$
                                                        <asp:Literal runat="server" ID="litSaldoPendiente"></asp:Literal></h4>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="col-sm-3 col-md-3"></div>
                        </div>

                        <div class="alert alert-danger" id="divError" style="display: none">
                            <strong>Lo sentimos! </strong><span id="msgError"></span>
                        </div>

                        <div class="alert alert-success" id="divOk" style="display: none">
                            <strong>Bien hecho! </strong><span id="msgOk"></span>
                        </div>

                        <div class="row mb15 hide">

                            <div class="col-sm-6 hide">
                                <img id="imgFoto" src="/files/usuarios/no-cheque.png" class="thumbnail img-responsive" alt="" runat="server" />
                                <div class="mb30"></div>

                                <div id="divLogo" style="display: none">
                                    <%--<p class="mb30">Formato JPG, PNG o GIF. Tamaño máximo recomendado: 100x70px</p>--%>
                                    <input type="hidden" value="" name="" /><input type="file" id="flpArchivo" />
                                    <div class="mb20"></div>
                                </div>

                                <div class="col-sm-6" id="divAdjuntarFoto">
                                    <a class="btn btn-white btn-block" onclick="fotos.showInputFoto();">Adjuntar foto</a>
                                </div>
                                <div class="col-sm-6" id="divEliminarFoto">
                                    <a class="btn btn-white btn-block" onclick="fotos.eliminarFoto();">Eliminar foto</a>
                                </div>
                            </div>
                        </div>

                        <%-- Datos principales--%>
                        <div class="row mb15">
                            <div class="col-sm-12">
                                <h3>Datos principales </h3>
                                <label class="control-label">Estos son los datos básicos que el sistema necesita para poder realizar operaciones con el <asp:Literal ID="liDatosGenerales" runat="server" ></asp:Literal>.</label>
                            </div>
                        </div>
                        <div id="divNumero">
                            <div class="row">
                                <div class="col-sm-6">
                                    <label class="control-label"><span class="asterisk" id="spIdentificacionObligatoria">*</span> Indicanos como identificarlo</label>
                                </div>
                            </div>
                            <div class="row mb15">
                                <div class="col-sm-3">
                                    <div class="form-group">
                                        <asp:TextBox runat="server" ID="txtNroDocumento" CssClass="form-control number validCuit" MaxLength="11" placeholder="Número"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-sm-3">
                                    <div class="form-group">
                                        <asp:DropDownList runat="server" ID="ddlTipoDoc" CssClass="form-control required" onchange="changeTipoDoc();">
                                            <asp:ListItem Text="" Value=""></asp:ListItem>
                                            <asp:ListItem Text="DNI" Value="DNI"></asp:ListItem>
                                            <asp:ListItem Text="CUIT" Value="CUIT" Selected="True"></asp:ListItem>
                                            <asp:ListItem Text="SIN CUIT" Value="SIN CUIT"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="row mb15" id="divBuscarDatosAfip">
                            <div class="col-sm-3" >
                                <div class="form-group">
                                    <input type="image" src="images/icoTraerDatosAfip.png" name="btnTraerDatosAfip" style="height:auto;max-width:100%" id="btnTraerDatosAfip" value="Traer datos AFIP" onclick="consultarDatosAfip();"/>                                    
                                </div>
                            </div> 
                            <div class="col-sm-9">
                                <div class="form-group">
                                    <div id="divProcesando" hidden="hidden"><img src="images/loaders/loader1.gif" /></div>
                                </div>
                            </div>
                        </div>
                        <div class="row mb15">
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="control-label"><span class="asterisk">*</span> Razón Social</label>
                                    <asp:TextBox runat="server" ID="txtRazonSocial" CssClass="form-control  required" MaxLength="128" placeholder="Pepas SRL o José Suárez" onkeyup="remplazarCaracteresEspeciales('txtRazonSocial');"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                        <div class="row mb15" id="divFantasia">
                            <div class="col-sm-3">
                                <div class="form-group">
                                    <label class="control-label">Nombre de Fantansía</label>
                                    <asp:TextBox runat="server" ID="txtNombreFantasia" CssClass="form-control" MaxLength="128" placeholder="Ej: Pepas"></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-sm-3">
                                <div class="form-group">
                                    <label class="control-label">Código</label>
                                    <asp:TextBox runat="server" ID="txtCodigo" CssClass="form-control" MaxLength="50"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                        <div class="row mb15">
                            <div class="col-sm-3">
                                <div class="form-group">
                                    <label class="control-label"><span class="asterisk">*</span> Categoría Impositiva</label>
                                    <asp:DropDownList runat="server" ID="ddlCondicionIva" CssClass="form-control required" onchange="changeCondicionIva();">
                                        <asp:ListItem Text="" Value=""></asp:ListItem>
                                        <asp:ListItem Text="Responsable Inscripto" Value="RI"></asp:ListItem>
                                        <asp:ListItem Text="Consumidor Final" Value="CF"></asp:ListItem>
                                        <asp:ListItem Text="Monotributista" Value="MO"></asp:ListItem>
                                        <asp:ListItem Text="Exento" Value="EX"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="col-sm-3" id="divPersoneria">
                                <div class="form-group">
                                    <label class="control-label"><span class="asterisk">*</span> Personería</label>
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
                                    <label class="control-label">Email (para poder enviarle la factura electrónica)</label>
                                    <div class="input-group">
                                        <span class="input-group-addon">@</span>
                                        <asp:TextBox runat="server" ID="txtEmail" CssClass="form-control multiemails" MaxLength="128" placeholder="ejemplo@ejemplo.com"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="row mb15">
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="control-label">Rango de facturación</label>
                                    <asp:DropDownList runat="server" ID="ddlRango" CssClass="form-control">
                                        <asp:ListItem Text="Sin Definir" Value="0"></asp:ListItem>
                                        <asp:ListItem Text="0 a 20%" Value="1"></asp:ListItem>
                                        <asp:ListItem Text="21 a 40%" Value="2"></asp:ListItem>
                                        <asp:ListItem Text="41 a 60%" Value="3"></asp:ListItem>
                                        <asp:ListItem Text="61 a 80%" Value="4"></asp:ListItem>
                                        <asp:ListItem Text="81 a 100%" Value="5"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>

                        <%-- Datos complementarios--%>
                        <div class="row mb15">
                            <div class="col-sm-12">
                                <hr />
                                <h3>Datos complementarios </h3>
                                <label class="control-label">Estos datos no son necesarios para facturar, pero podrían serte de utilidad ;-)</label>
                            </div>
                        </div>

                        <div class="row mb15">
                            <div class="col-sm-3">
                                <div class="form-group">
                                    <label class="control-label">Teléfono</label>
                                    <div class="input-group">
                                        <span class="input-group-addon"><i class="glyphicon glyphicon-earphone"></i></span>
                                        <asp:TextBox runat="server" ID="txtTelefono" TextMode="Phone" CssClass="form-control" MaxLength="50" placeholder="+54911########"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                            <div class="col-sm-3">
                            </div>
                        </div>
                        <div class="row mb15" id="divDomicilio">
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="control-label">Domicilio</label>
                                    <asp:TextBox runat="server" ID="txtDomicilio" CssClass="form-control" MaxLength="100" placeholder=" Ej: Av. Corrientes"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                        <div class="row mb15">
                            <div class="col-sm-3">
                                <div class="form-group">
                                    <label class="control-label">Piso/Depto</label>
                                    <asp:TextBox runat="server" ID="txtPisoDepto" CssClass="form-control" MaxLength="10" placeholder=" Ej: 9B"></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-sm-3">
                                <div class="form-group">
                                    <label class="control-label">Código Postal</label>
                                    <asp:TextBox runat="server" ID="txtCp" CssClass="form-control" MaxLength="10" placeholder=" Ej: 1414"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                        <div class="row">
                            <div class="col-sm-6">
                                <label class="control-label">Seleccioná la provincia y localidad o barrio (para CABA)</label>
                            </div>
                        </div>
                        <div class="row mb15">
                            <div class="col-sm-3">
                                <div class="form-group">
                                    <select class="select2 required" data-placeholder="Selecciona una provincia..." id="ddlProvincia" onchange="changeProvincia();">
                                    </select>
                                </div>
                            </div>
                            <div class="col-sm-3">
                                <div class="form-group">
                                    <select id="ddlCiudad" class="select2" data-placeholder="Selecciona una Ciudad/Localidad...">
                                    </select>
                                </div>
                            </div>
                        </div>
                         <div class="row">
                            <div class="col-sm-6">
                                <label class="control-label">Ingresá opcionalmente la provincia y localidad o barrio (para CABA)</label>
                            </div>
                        </div>
                        <div class="row mb15">
                            <div class="col-sm-3">
                                <div class="form-group">
                                    <asp:TextBox runat="server" ID="txtProvinciaDesc" CssClass="form-control" MaxLength="50" placeholder=" Ej: Buenos Aires"></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-sm-3">
                                <div class="form-group">
                                    <asp:TextBox runat="server" ID="txtCiudadDesc" CssClass="form-control" MaxLength="50" placeholder=" Ej: Quilmes"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                        <div class="row mb15">
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="control-label">Observaciones</label>
                                    <asp:TextBox runat="server" TextMode="MultiLine" Rows="5" ID="txtObservaciones" CssClass="form-control" placeholder="Ej: Datos bancarios, tipo de comprobante que necesita el cliente, nombres de contactos,etc."></asp:TextBox>
                                </div>
                            </div>
                        </div>
                        <div class="row mb15">
                            <div class="col-sm-3">
                                <div class="form-group">
                                    <label class="control-label">Saldo inicial</label>
                                    <div class="input-group">
                                        <span class="input-group-addon">$</span>
                                        <asp:TextBox runat="server" ID="txtSaldo" CssClass="form-control" MaxLength="10" placeholder="Saldo inicial"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                            <div class="col-sm-3" id="divListaClientes">
                                <div class="form-group">
                                    <label class="control-label">Lista de precios</label>
                                    <asp:DropDownList runat="server" ID="ddlListaPrecios" CssClass="form-control">
                                        <asp:ListItem Text="Default" Value="0"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>

                        <div class="row mb15">
                            <div class="col-sm-6 hide">
                                <h5 class="control-label">Ubicación de Contacto</h5>
                                <div id="gmap-marker" style="height: 300px"></div>
                            </div>
                        </div>

                        <div class="row mb15">
                            <div class="col-sm-12">
                                <hr />
                                <h3>Porcentaje de descuento </h3>
                                <label class="control-label">Administración del porcentaje de descuento en la venta</label>
                            </div>
                        </div>
                        <div class="row mb15">
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="control-label">Descuento (%)</label>
                                    <asp:TextBox runat="server" ID="txtPorcentajeDescuento" CssClass="form-control" MaxLength="10" ></asp:TextBox>
                                </div>
                            </div>
                        </div>

                        <div id="divDomicilios" runat="server">
                            <div class="row mb15">
                                <div class="col-sm-12">
                                    <hr />
                                    <h3>Domicilios </h3>
                                    <label class="control-label">Administración de los domicilios de entrega, se precarga como domicilio principal el registrado en AFIP</label>
                                </div>
                            </div>
                            <div class="alert alert-danger" id="divDomiciliosError" style="display: none">
                                <strong>Lo sentimos! </strong><span id="msgDomiciliosError"></span>
                            </div>

                            <div class="alert alert-success" id="divDomiciliosOk" style="display: none">
                                <strong>Bien hecho! </strong><span id="msgDomiciliosOk"></span>
                            </div>                            
                            <div class="row mb15">
                                <div class="col-sm-10 col-md-10 col-lg-10">
                                    <div class="form-group">
                                        <select class="select2" data-placeholder="Seleccione un domicilio..." id="ddlDomicilio" onclick="changeDomicilio();">
                                            <option value=""></option>
                                        </select>
                                    </div>
                                </div>
                                <div class="col-sm-1">
                                    <div class="form-group">
                                        <a href="#" class="btn btn-warning" data-toggle="modal" data-target="#modalNuevoDomicilio">
                                            <i class="glyphicon glyphicon-plus"></i>
                                        </a>
                                    </div>
                                </div>
                            </div>
                            <div id="divDomiciliosDetalle" hidden="hidden">
                                <div class="row mb15">
                                    <div class="col-sm-6">
                                        <div class="form-group">
                                            <label class="control-label">Domicilio</label>
                                            <asp:TextBox runat="server" ID="txtDomiciliosDomicilio" CssClass="form-control" MaxLength="100" placeholder=" Ej: Av. Corrientes"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>                                                        
                                <div class="row mb15">
                                    <div class="col-sm-3">
                                        <div class="form-group">
                                            <label class="control-label">Piso/Depto</label>
                                            <asp:TextBox runat="server" ID="txtDomiciliosPisoDepto" CssClass="form-control" MaxLength="10" placeholder=" Ej: 9B"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="col-sm-3">
                                        <div class="form-group">
                                            <label class="control-label">Código Postal</label>
                                            <asp:TextBox runat="server" ID="txtDomiciliosCodigoPostal" CssClass="form-control" MaxLength="10" placeholder=" Ej: 1414"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                                <div class="row">
                                    <div class="col-sm-6">
                                        <label class="control-label">Seleccioná la provincia y localidad o barrio (para CABA)</label>
                                    </div>
                                </div>
                                <div id="divDomiciliosGeograficoAfip" >
                                    <div class="row mb15">
                                        <div class="col-sm-3">
                                            <div class="form-group">
                                                <select class="select2 required" data-placeholder="Selecciona una provincia..." id="ddlDomiciliosProvincia" onchange="changeDomiciliosProvincia();">
                                                </select>
                                            </div>
                                        </div>
                                        <div class="col-sm-3">
                                            <div class="form-group">
                                                <select id="ddlDomiciliosCiudad" class="select2" data-placeholder="Selecciona una Ciudad/Localidad...">
                                                </select>
                                            </div>
                                        </div>
                                    </div>
                                </div>     
                                <div class="row mb15">
                                    <div class="col-sm-3">
                                        <div class="form-group">
                                            <label class="control-label">Provincia</label>
                                            <asp:TextBox runat="server" ID="txtDomiciliosProvinciaTexto" CssClass="form-control" placeholder=" Ej: Buenos Aires"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="col-sm-3">
                                        <div class="form-group">
                                            <label class="control-label">Ciudad</label>
                                            <asp:TextBox runat="server" ID="txtDomiciliosCiudadTexto" CssClass="form-control"  placeholder=" Ej: La Plata"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                                <div class="row mb15">
                                    <div class="col-sm-3">
                                        <div class="form-group">
                                            <label class="control-label">Contacto</label>
                                            <asp:TextBox runat="server" ID="txtDomiciliosContacto" CssClass="form-control" ></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="col-sm-3">
                                        <div class="form-group">
                                            <label class="control-label">Teléfono</label>
                                            <asp:TextBox runat="server" ID="txtDomiciliosTelefono" CssClass="form-control" ></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                                <div class="row mb15">
                                    <div class="col-sm-3">
                                        <div class="form-group">
                                            <a class="btn btn-info" id="btnDomiciliosActualizar" onclick="domiciliosGrabar();"> Guardar Domicilio</a>
                                        </div>
                                    </div> 
                                </div>
                                <div class="row mb15">
                                    <div class="col-sm-3">
                                        <div class="form-group">
                                            <a class="btn btn-danger" id="btnDomiciliosEliminar" onclick="domiciliosEliminar();"> Eliminar Domicilio</a>
                                        </div>
                                    </div>   
                                </div>
                            </div>                            
                        </div>   
                        <div id="divDomiciliosTransporte" runat="server">
                            <div class="row mb15">
                                <div class="col-sm-12">
                                    <hr />
                                    <h3>Domicilios Transporte</h3>
                                    <label class="control-label">Administración de los domicilios transporte</label>
                                </div>
                            </div>
                            <div class="alert alert-danger" id="divDomiciliosTransporteError" style="display: none">
                                <strong>Lo sentimos! </strong><span id="msgDomiciliosTransporteError"></span>
                            </div>

                            <div class="alert alert-success" id="divDomiciliosTransporteOk" style="display: none">
                                <strong>Bien hecho! </strong><span id="msgDomiciliosTransporteOk"></span>
                            </div>                            
                            <div class="row mb15">
                                <div class="col-sm-10 col-md-10 col-lg-10">
                                    <div class="form-group">
                                        <select class="select2" data-placeholder="Seleccione un domicilio..." id="ddlDomicilioTransporte" onclick="changeDomicilioTransporte();">
                                            <option value=""></option>
                                        </select>
                                    </div>
                                </div>
                                <div class="col-sm-1">
                                    <div class="form-group">
                                        <a href="#" class="btn btn-warning" data-toggle="modal" data-target="#modalNuevoTransportePersona">
                                            <i class="glyphicon glyphicon-plus"></i>
                                        </a>
                                    </div>
                                </div>
                            </div>
                            <div id="divDomiciliosTransporteDetalle" hidden="hidden">
                                <div class="row mb15">
                                    <div class="col-sm-6">
                                        <div class="form-group">
                                            <label class="control-label">Razón Social</label>
                                            <asp:TextBox runat="server" ID="txtDomiciliosTransporteRazonSocial" CssClass="form-control" MaxLength="100" placeholder=" Ej: Av. Corrientes"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>  
                                <div class="row mb15">
                                    <div class="col-sm-3">
                                        <div class="form-group">
                                            <label class="control-label">Provincia</label>
                                            <asp:TextBox runat="server" ID="txtDomiciliosTransporteProvinciaTexto" CssClass="form-control" placeholder=" Ej: Buenos Aires"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="col-sm-3">
                                        <div class="form-group">
                                            <label class="control-label">Ciudad</label>
                                            <asp:TextBox runat="server" ID="txtDomiciliosTransporteCiudadTexto" CssClass="form-control"  placeholder=" Ej: La Plata"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>  
                                <div class="row mb15">
                                    <div class="col-sm-6">
                                        <div class="form-group">
                                            <label class="control-label">Domicilio</label>
                                            <asp:TextBox runat="server" ID="txtDomiciliosTransporteDomicilio" CssClass="form-control" MaxLength="100" placeholder=" Ej: Av. Corrientes"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                                <div class="row mb15">
                                    <div class="col-sm-3">
                                        <div class="form-group">
                                            <label class="control-label">Piso/Depto</label>
                                            <asp:TextBox runat="server" ID="txtDomiciliosTransportePisoDepto" CssClass="form-control" MaxLength="10" placeholder=" Ej: 9B"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="col-sm-3">
                                        <div class="form-group">
                                            <label class="control-label">Código Postal</label>
                                            <asp:TextBox runat="server" ID="txtDomiciliosTransporteCodigoPostal" CssClass="form-control" MaxLength="10" placeholder=" Ej: 1414"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>                                  
                                <div class="row mb15">
                                    <div class="col-sm-3">
                                        <div class="form-group">
                                            <label class="control-label">Contacto</label>
                                            <asp:TextBox runat="server" ID="txtDomiciliosTransporteContacto" CssClass="form-control" ></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="col-sm-3">
                                        <div class="form-group">
                                            <label class="control-label">Teléfono</label>
                                            <asp:TextBox runat="server" ID="txtDomiciliosTransporteTelefono" CssClass="form-control" ></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                                <div class="row mb15">
                                    <div class="col-sm-3">
                                        <div class="form-group">
                                            <a class="btn btn-info" id="btnDomiciliosTransporteActualizar" onclick="domiciliosTransporteGrabar();"> Guardar Domicilio Transporte</a>
                                        </div>
                                    </div>   
                                </div>
                                <div class="row mb15">
                                    <div class="col-sm-3">
                                        <div class="form-group">
                                            <a class="btn btn-danger" id="btnDomiciliosTransporteEliminar" onclick="domiciliosTransporteEliminar();"> Eliminar Domicilio Transporte</a>
                                        </div>
                                    </div>   
                                </div>
                            </div>                            
                        </div>   
                        <div class="row mb15">
                            <div class="col-sm-12">
                                <hr />
                                <h3>Avisos </h3>
                                <label class="control-label">Deje un mensaje que se visualizará el seleccionar el cliente en los pedidos</label>
                            </div>
                        </div>
                        <div class="row mb15">
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="control-label">Mensaje</label>
                                    <asp:TextBox runat="server" ID="txtAvisos" CssClass="form-control" ></asp:TextBox>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="panel-footer">
                        <a class="btn btn-success" id="btnActualizar" onclick="grabar();"> <asp:Literal ID="libtnGuardar" runat="server"></asp:Literal></a>
                        <a href="#" onclick="cancelar();" style="margin-left:20px">No quiero guardarlo</a>

                        <div class="btn-group dropup" style="margin-bottom: 0; float:right" id="btnAcciones">
                            <button type="button" class="btn btn-warning">Más opciones</button>
                            <button class="btn btn-warning dropdown-toggle" data-toggle="dropdown"><span class="caret"></span></button>
                            <ul class="dropdown-menu">
                                <li><a href="javascript:CambiarReporte()">Ver cuenta corriente</a></li>
                            </ul>
                        </div>
                    </div>
                </div>

                <asp:HiddenField runat="server" ID="hdnTieneFoto" Value="0" />
                <asp:HiddenField runat="server" ID="hdnSinCombioDeFoto" Value="0" />
                <asp:HiddenField runat="server" ID="hdnFileName" Value="" />

                <asp:HiddenField runat="server" ID="hdnDireccion" Value="" />
                <asp:HiddenField runat="server" ID="hdnCiudad" Value="0" />
                <asp:HiddenField runat="server" ID="hdnProvincia" Value="1" />

                <asp:HiddenField runat="server" ID="hdnIDListaPrecio" Value="0" />
                <asp:HiddenField runat="server" ID="hdnID" Value="0" />
                <asp:HiddenField runat="server" ID="hdnIDUsuario" Value="0" />
                <asp:HiddenField runat="server" ID="hdnTipo" />

                <asp:HiddenField runat="server" ID="hdnDomiciliosProvincia" Value="1" />
                <asp:HiddenField runat="server" ID="hdnDomiciliosCiudad" Value="0" />
            </form>
        </div>
    </div>

    <!-- Modal -->
    <div class="modal modal-wide fade" id="modalDetalle" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title" id="titDetalle"></h4>
                    <%--style="display: inline-table;"--%>

                    <br />
                    <input type="radio" name="rCtaCte" id="rCtaCteCliente" value="1" onchange="CambiarReporte();" />
                    <label>Ver como Cliente</label>
                    <input type="radio" name="rCtaCte" id="rCtaCteProv" value="0" onchange="CambiarReporte();" />
                    <label>Ver como Proveedor</label>
                </div>
                <div class="modal-body">
                    <div class="table-responsive">
                        <table class="table table-success mb30 smallTable">
                            <thead>
                                <tr>
                                    <th>PDC</th>
                                    <th>Comprobante</th>
                                    <th>Fecha</th>
                                    <th>Comprobante aplicado</th>
                                    <th>Fecha Cobro</th>
                                    <th>Importe</th>
                                    <th>Cobrado</th>
                                    <th>Total</th>
                                </tr>
                            </thead>
                            <tbody id="bodyDetalle">
                            </tbody>
                        </table>
                    </div>
                </div>
                <div class="modal-footer">
                    <a href="#" type="button" style="margin-left: 20px"  onclick="RealizarCobranza();" class="btn btn-info">Cobranza</a>
                    <a href="#" type="button" style="margin-left: 20px"  onclick="ImprimirDetalleCuentaCorriente();" class="btn btn-success">Imprimir</a> 
                    <a style="margin-left:20px" href="#" data-dismiss="modal">Cerrar</a>
                </div>
                
            </div>
        </div>
    </div>

    <div class="modal modal-wide fade" id="modalPdf" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true" >
        <div class="modal-dialog">
            <div class="modal-content" style="position:fixed">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Visualización del detalle de cuenta corriente</h4>
                </div>
                <div class="modal-body">
                    <div>
                        <div class="alert alert-danger" id="divErrorCat" style="display: none">
                            <strong>Lo sentimos! </strong><span id="msgErrorCat"></span>
                        </div>

                        <iframe id="ifrPdf" src="" width="900px" height="500px" frameborder="0"></iframe>
                    </div>
                </div>
                <div class="modal-footer">
                    <div id="divPrevisualizarCerrar" class="col-sm-3 CAJA_BLANCA_AZUL">
                        <a id="lnkPrevisualizarCerrar" data-dismiss="modal">
                            <i class="glyphicon glyphicon-log-in" style="font-size: 30px; opacity: 1; border: none; padding: 0"></i>
                            <br />
                            Cerrar
                        </a>
                    </div>
                </div>
            </div>
        </div>
    </div>
    
    <script id="resultTemplateDetalle" type="text/x-jQuery-tmpl">
        {{each results}}
        <tr>
            {{if $value.Cobrado != "Saldo"}}//Si no es la ultima columna, muestro todo normal. Solo por cuestiones estéticas.
                <td {{if $value.Comprobante != ""}} class="bgRow" {{/if}}>
                    ${PDC}
                </td>
                <td {{if $value.Comprobante != ""}} class="bgRow" {{/if}}>
                    ${Comprobante}
                </td>
                <td {{if $value.Comprobante != ""}} class="bgRow" {{/if}}>
                    ${Fecha}
                </td>
                <td {{if $value.Comprobante != ""}} class="bgRow" {{/if}}>
                    ${ComprobanteAplicado}
                </td>
                <td {{if $value.Comprobante != ""}} class="bgRow" {{/if}}>
                    ${FechaCobro}
                </td>
                <td {{if $value.Comprobante != ""}} class="bgRow" {{/if}}>
                    ${Importe}
                </td>
                <td {{if $value.Comprobante != ""}} class="bgRow" {{/if}}>
                    ${Cobrado}
                </td>
                <td {{if $value.Comprobante != ""}} class="bgRow" {{/if}}>
                    ${Total}
                </td>
            {{else}}
                <td class="bgTotal" colspan="6">&nbsp;</td>
                <td class="bgTotal text-danger">
                    ${Cobrado}
                </td>
                <td class="bgTotal text-danger">
                    ${Total}
                </td>
            {{/if}}
        </tr>
        {{/each}}
    </script>

    <script id="noResultTemplateDetalle" type="text/x-jQuery-tmpl">
        <tr>
            <td colspan="7">No se han encontrado resultados
            </td>
        </tr>
    </script>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FooterContent" runat="Server">
    <script src="/js/jquery.tmpl.min.js"></script>
    <script src="/js/select2.min.js"></script>
    <script src="/js/jquery.validate.min.js"></script>
    <script src="/js/views/personas.js?v=<%= ConfigurationManager.AppSettings["JS.Version"] %>"></script>
    <script src="/js/jquery.fileupload.js" type="text/javascript"></script>
    <script src="/js/jquery.maskedinput.min.js"></script>
    <script src="/js/jquery.maskMoney.min.js"></script>
    <script src="/js/views/nuevoDomicilio.js?v=<%= ConfigurationManager.AppSettings["JS.Version"] %>"></script>

    <UC:NuevoDomicilio runat="server" ID="ucDomicilio" />
    <UC:NuevoTransportePersona runat="server" ID="ucTransportePersona" />

   <%-- <script src="js/jquery-jvectormap-1.2.2.min.js"></script>
   <%-- <script src="js/jquery-jvectormap-1.2.2.min.js"></script>
    <script src="js/jquery-jvectormap-world-mill-en.js"></script>
    <script src="js/jquery-jvectormap-us-aea-en.js"></script>
    <script src="http://maps.google.com/maps/api/js?sensor=true"></script>
    <script src="js/gmaps.js"></script>--%>
    <script>
        jQuery(document).ready(function () {
            configForm();
        });
        $(document).ajaxStart(function () {
            $("#btnTraerDatosAfip").attr("disabled", true);
            document.body.style.cursor = "wait";
            $("#divProcesando").show();
        }).ajaxStop(function () {
            $("#btnTraerDatosAfip").attr("disabled", false);
            document.body.style.cursor = "default";
            $("#divProcesando").hide();
        });
        $('#modalNuevoDomicilio').on('show.bs.modal', function (e) {   
            $("#hdnNuevoDomicilioIdPersona").val($("#hdnID").val());
        });
        $('#modalNuevoDomicilio').on('hidden.bs.modal', function (e) {
            $("#divDomiciliosDetalle").hide();
            Common.obtenerDomicilios("ddlDomicilio", "", false, $("#hdnID").val());            
        });
        $('#modalNuevoTransportePersona').on('show.bs.modal', function (e) {
            $("#hdnNuevoTransportePersonaIdUsuario").val($("#hdnID").val());
        });
        $('#modalNuevoTransportePersona').on('hidden.bs.modal', function (e) {
            $("#divDomiciliosTransporteDetalle").hide();
            Common.obtenerTransportePersona("ddlDomicilioTransporte", "", false, $("#hdnID").val());
        });
    </script>

</asp:Content>
