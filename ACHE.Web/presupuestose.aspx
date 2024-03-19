<%@ Page Title="" Language="C#" MasterPageFile="~/Front.master" AutoEventWireup="true" CodeFile="presupuestose.aspx.cs" Inherits="presupuestose" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
    <style type="text/css">
        .modal.modal-wide .modal-dialog {
            width: 102%;
            max-width: 900px;
        }

        .modal-wide .modal-body {
            overflow-y: auto;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="pageheader">
        <h2><i class="fa fa-file-text"></i>Presupuestos </h2>
        <div class="breadcrumb-wrapper">
            <span class="label">Estás aquí:</span>
            <ol class="breadcrumb">
                <li><a href="/home.aspx">axanweb</a></li>
                <li><a href="/presupuestos.aspx">Presupuestos</a></li>
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
                                <h3>Datos Principales</h3>
                                <label class="control-label">Estos son los datos básicos que el sistema necesita para poder generar un presupuesto.</label>
                            </div>
                        </div>

                        <div class="row mb15">
                            <div class="col-sm-5">
                                <div class="form-group">
                                    <label class="control-label"><span class="asterisk">*</span> Proveedor/Cliente</label>
                                    <select class="select2 required" data-placeholder="Seleccione un cliente/proveedor..." id="ddlPersona">
                                        <option value=""></option>
                                    </select>

                                </div>
                            </div>
                            <div class="col-sm-1">
                                <div class="form-group">
                                    <a href="#" class="btn btn-warning" data-toggle="modal" data-target="#modalNuevoCliente" style="margin: 29px -10px; padding: 7px 15px">
                                        <i class="glyphicon glyphicon-plus"></i>
                                    </a>
                                </div>
                            </div>

                            <div class="col-sm-2 hide">
                                <div class="form-group">
                                    <label class="control-label"><span class="asterisk">*</span> Nombre</label>
                                    <asp:TextBox runat="server" ID="txtNombre" CssClass="form-control" MaxLength="100"></asp:TextBox>
                                </div>
                            </div>

                            <div class="col-sm-10" id="divClienteDatosPendientes" style="display: none">
                                <div class="form-group">
                                    <div id="divClienteDatosPendientesTexto" style="color:red"></div>
                                    <label class="control-label" style="color:red" id="lblClienteDatosPendientes"></label>
                                </div>
                            </div>

                        </div>
                        <div class="row mb15">
                            <div class="col-sm-2">
                                <div class="form-group">
                                    <label class="control-label"><span class="asterisk">*</span> Fecha validez</label>
                                    <div class="input-group">
                                        <span class="input-group-addon"><i class="glyphicon glyphicon-calendar"></i></span>
                                        <asp:TextBox runat="server" ID="txtFechaValidez" CssClass="form-control required validDate" placeholder="dd/mm/yyyy" MaxLength="10"></asp:TextBox>

                                    </div>
                                </div>
                            </div>
                            <div class="col-sm-2">
                                <div class="form-group">
                                    <label class="control-label"><span class="asterisk">*</span> Numero</label>
                                    <asp:TextBox runat="server" ID="txtNumero" CssClass="form-control required" MaxLength="8"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                        <div class="row mb15">
                            <div class="col-sm-12">
                                <h3>Datos generales</h3>
                                <label class="control-label">Estos son los datos adicionales para poder gestionar su presupuesto</label>
                            </div>
                        </div>

                        <div class="row mb15">
                            <div class="col-sm-2">
                                <div class="form-group">
                                    <label class="control-label">Estado</label>
                                    <asp:DropDownList runat="server" ID="ddlEstado" CssClass="form-control required" onchange="Presupuestos.showbtnfacturarPresupuesto();">
                                        <asp:ListItem Text="Borrador" Value="B"></asp:ListItem>
                                        <asp:ListItem Text="Enviado" Value="E"></asp:ListItem>
                                        <asp:ListItem Text="Aprobado" Value="A"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="col-sm-2">
                                <div class="form-group">
                                    <label class="control-label">Condiciones de pago</label>
                                    <asp:DropDownList runat="server" ID="ddlFormaPago" CssClass="form-control required">
                                        <asp:ListItem Text="A 15 días" Value="A 15 dias"></asp:ListItem>
                                        <asp:ListItem Text="A 30 días" Value="A 30 dias"></asp:ListItem>
                                        <asp:ListItem Text="A 60 días" Value="A 60 dias"></asp:ListItem>
                                        <asp:ListItem Text="50% y 50%" Value="50% y 50%"></asp:ListItem>
                                        <asp:ListItem Text="30%, 40% y 30%" Value="30%, 40% y 30%"></asp:ListItem>
                                        <asp:ListItem Text="A convenir" Value="A convenir" Selected="True"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>

                            <div class="col-sm-2 col-md-2">
                                <div class="form-group">
                                    <label class="control-label">Producto o Servicio</label>
                                    <select id="ddlProducto" class="form-control required" onchange="Common.obtenerConceptosCodigoyNombre('ddlProductos', this.value, true);Presupuestos.changeConcepto();">
                                        <option value="2">Servicio</option>
                                        <option value="1">Producto</option>
                                        <option value="3" selected="selected">Producto y servicio</option>
                                    </select>
                                </div>
                            </div>
                        </div>


                        <div class="mb40"></div>

                        <div class="row mb15">
                            <div class="col-sm-12">
                                <h3>Detalle del presupuesto</h3>
                                <label class="control-label">Agrega a continuación los productos o servicios que vas a incluir en el presupuesto.</label>
                            </div>
                        </div>
                        <div class="well">
                            <div class="alert alert-danger" id="divErrorDetalle" style="display: none">
                                <strong>Lo siento! </strong><span id="msgErrorDetalle"></span>
                            </div>

                            <div class="row">
                                <div class="col-sm-1 col-md-1">
                                    <span class="asterisk">*</span> Cantidad
                                    <input type="tel" class="form-control" id="txtCantidad" />
                                </div>
                                <div class="col-sm-4 col-md-4">
                                    <span class="asterisk">*</span> Producto/Servicio/Concepto
                                    <div class="row">
                                        <div class="col-lg-6 col-lg-6">
                                            <select class="select3" data-placeholder="-" id="ddlProductos" onchange="Presupuestos.changeConcepto();">
                                            </select>
                                        </div>
                                        <div class="col-lg-6 col-lg-6">
                                            <input type="text" class="form-control" maxlength="200" id="txtConcepto" />
                                        </div>
                                    </div>
                                </div>
                                <div class="col-sm-2 col-md-2">
                                    <span class="asterisk">*</span><asp:Literal ID="liPrecioUnitario" runat="server"></asp:Literal>
                                    <input type="tel" class="form-control"  id="txtPrecio" />
                                </div>
                                <div class="col-sm-1 col-md-1">
                                    <span class="asterisk">*</span> Bonif %
                                            <input type="tel" class="form-control"  id="txtBonificacion" />
                                </div>
                                <div class="col-sm-1 col-md-1">
                                    <span class="asterisk">*</span> IVA %
                                    <select class="select2" id="ddlIva" runat="server">
                                        <option value="1">NG</option>
                                        <option value="2">Exento</option>
                                        <option value="3">0</option>
                                        <option value="4">10,5</option>
                                        <option value="5">21</option>
                                        <option value="6">27</option>
                                        <option value="8">5</option>
                                        <option value="9">2,5</option>
                                    </select>
                                </div>
                                <div class="col-sm-2 col-md-2">
                                    <br />
                                    <a class="btn btn-default btn-sm" id="btnAgregarItem" onclick="Presupuestos.agregarItem();">Agregar</a>
                                    <a class="btn btn-default btn-sm" id="btnCancelarItem" onclick="Presupuestos.cancelarItem();">Cancelar</a>
                                    <input type="hidden" runat="server" id="hdnIDItem" value="0" />
                                </div>
                            </div>
                        </div>

                        <div class="table-responsive">
                            <table class="table table-invoice">
                                <thead>
                                    <tr>
                                        <th>#</th>
                                        <th style="text-align: left">Cantidad</th>
                                        <th style="text-align: left">Código</th>
                                        <th style="text-align: left">Concepto</th>
                                        <th style="text-align: right">Precio unitario sin IVA</th>
                                        <th style="text-align: right">Bonificación %</th>
                                        <th style="text-align: right">IVA %</th>
                                        <th style="text-align: right">Subtotal</th>
                                    </tr>
                                </thead>
                                <tbody id="bodyDetalle">
                                    <tr>
                                        <td colspan='8'>No tienes items agregados</td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>
                        <!-- table-responsive -->
                        <div id="divVendedor">
                            <div class="row mb15">
                                <div class="col-sm-10 col-md-8 col-lg-5">
                                    <div class="form-group">
                                        <label class="control-label"><span class="asterisk">*</span> Vendedor</label>
                                        <asp:TextBox runat="server" ID="txtVendedor" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="col-sm-6">
                            <div class="form-group">
                                <label class="control-label">Observaciones</label>
                                <asp:TextBox runat="server" TextMode="MultiLine" Rows="5" ID="txtObservaciones" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-sm-6">
                            <table class="table table-total">
                                <tbody>
                                    <tr>
                                        <td><strong>Neto gravado :</strong></td>
                                        <td id="divSubtotal">$ 0</td>
                                    </tr>
                                    <tr>
                                        <td><strong>IVA :</strong></td>
                                        <td id="divIVA">$ 0</td>
                                    </tr>
                                    <tr>
                                        <td><strong>TOTAL :</strong></td>
                                        <td id="divTotal" style="color: green">$ 0</td>
                                    </tr>
                                </tbody>
                            </table>
                        </div>

                        <div class="mb40"></div>

                    </div>
                    <div class="panel-footer">
                        <a class="btn btn-success" id="btnActualizar" onclick="Presupuestos.grabar();">Aceptar</a>
                        <a id="btnFacturarPresupuesto" class="btn btn-primary" onclick="Presupuestos.facturarPresupuesto();" style="display: none" runat="server">Facturar Presupuesto</a>
                        <a href="#" onclick="Presupuestos.cancelar();" style="margin-left: 20px">Cancelar</a>
                        <a class="btn btn-white" onclick="Presupuestos.previsualizar();" style="float: right;"><i class="fa fa-desktop fa-fw mr5"></i>Previsualizar en PDF</a>
                        <div class="btn-group dropup" style="float:right; padding-right: 10px; display: none" id="divMasOpciones">
                            <button type="button" class="btn btn-warning">Más opciones</button>
                            <button class="btn btn-warning dropdown-toggle" data-toggle="dropdown"><span class="caret"></span></button>
                            <ul class="dropdown-menu">
                                <li id="liVincularPresupuesto"><a href="javascript:Presupuestos.vincularPresupuesto()">Copiar a Pedido</a></li>
                            </ul>
                        </div>
                    </div>
                </div>

                <asp:HiddenField runat="server" ID="hdnCodigo" Value="" />
                <asp:HiddenField runat="server" ID="hdnUsaPrecioConIVA" Value="0" />
                <asp:HiddenField runat="server" ID="hdnID" Value="0" />
                <asp:HiddenField runat="server" ID="hdnIDUsuario" Value="0" />
                <asp:HiddenField runat="server" ID="hdnIDPersona" Value="0" />
                <asp:HiddenField runat="server" ID="hdnPresupuestoVinculado" Value="0" />
                <asp:HiddenField runat="server" ID="hdnParaPDVSolicitarCompletarContacto" Value="0" />
                <asp:HiddenField runat="server" ID="hdnUsaCantidadConDecimales" Value="0" />
                <input type="hidden" id="hdnClienteTipo" runat="server" />
            </form>
        </div>
    </div>

    <!-- Modal -->
    <div class="modal modal-wide fade" id="modalPdf" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content" style="width: 102%">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Visualización de comprobante</h4>
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
                    <a style="margin-left: 20px" href="#" data-dismiss="modal">Cerrar</a>
                </div>
            </div>
        </div>
    </div>

    <div class="modal modal fade" id="modalOk" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true" data-backdrop="static" data-keyboard="false">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true" onclick="window.location.href='presupuestos.aspx'">&times;</button>
                    <h4 class="modal-title" id="litModalOkTitulo">Comprobante emitido correctamente. </h4>
                </div>
                <div class="modal-body" style="min-height: 200px;">
                    <div class="alert alert-success" id="divOkMail" style="display: none">
                        <strong>Bien hecho! </strong>El mensaje ha sido enviado correctamente
                    </div>
                    <div id="divToolbar">
                        <div id="divRemito" class="col-sm-3 CAJA_BLANCA_AZUL">
                            <a href="#" id="lnkDownloadRemito" onclick="Presupuestos.generarRemito();">
                                <span class="fa fa-file-text" style="font-size: 28px;"></span>
                                <br />
                                Descargar Remito
                            </a>
                        </div>
                        <div id="divDownloadPdf" class="col-sm-3 CAJA_BLANCA_AZUL">
                            <a href="#" id="lnkDownloadPresupuestoPdf" onclick="Presupuestos.generarPresupuesto('PDF');">
                                <span class="fa fa-file-text" style="font-size: 28px;"></span>
                                <br />
                                Descargar presupuesto PDF
                            </a>
                        </div>
                        <div id="divDownloadJpg" class="col-sm-3 CAJA_BLANCA_AZUL">
                            <a href="#" id="lnkDownloadPresupuestoPng" onclick="Presupuestos.generarPresupuesto('PNG');">
                                <span class="fa fa-file-text" style="font-size: 28px;"></span>
                                <br />
                                Descargar presupuesto PNG
                            </a>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <a style="margin-left: 20px" href="#" data-dismiss="modal" onclick="window.location.href='presupuestos.aspx'">Cerrar</a>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FooterContent" runat="Server">
    <script src="/js/jquery.maskMoney.min.js"></script>
    <script src="/js/jquery.maskedinput.min.js"></script>
    <script src="/js/jquery.validate.min.js"></script>
    <script src="/js/select2.min.js"></script>
    <script src="/js/views/presupuestos.js?v=<%= ConfigurationManager.AppSettings["JS.Version"] %>"></script>
    <UC:NuevaPersona runat="server" ID="ucCliente" />

    <script>
        jQuery(document).ready(function () {
            Presupuestos.configForm();
        });
    </script>

</asp:Content>
