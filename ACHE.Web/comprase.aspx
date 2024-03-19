<%@ Page Title="" Language="C#" MasterPageFile="~/Front.master" AutoEventWireup="true" CodeFile="comprase.aspx.cs" Inherits="comprase" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
    <style type="text/css">
        .modal.modal-wide .modal-dialog {
            width: 90%;
            max-width: 900px;
        }

        .modal-wide .modal-body {
            overflow-y: auto;
        }
    </style>

    <link href="/css/jasny-bootstrap.min.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="pageheader">
        <h2><i class="fa fa-tasks"></i>Comprobantes</h2>
        <div class="breadcrumb-wrapper">
            <span class="label">Estás aquí:</span>
            <ol class="breadcrumb">
                <li><a href="/home.aspx">axanweb</a></li>
                <li><a href="/compras.aspx">Comprobantes</a></li>
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
                        <asp:Panel runat="server" ID="pnlContenido">

                            <div class="alert alert-danger" id="divError" style="display: none">
                                <strong>Lo sentimos! </strong><span id="msgError"></span>
                            </div>
                            <div class="alert alert-success" id="divOk" style="display: none">
                                <strong>Bien hecho! </strong>Los datos se han actualizado correctamente
                            </div>

                            <div class="row mb15">
                                <div class="col-sm-12">
                                    <h3>Datos generales</h3>
                                    <label class="control-label">Acá van los datos más generales del comprobante.</label>
                                </div>
                            </div>
                            <div class="row mb15">
                                <div class="col-sm-11 col-md-9 col-lg-6">
                                    <div class="form-group">
                                        <label class="control-label"><span class="asterisk">*</span> Proveedor/Cliente</label>
                                        <select class="select2 required" data-placeholder="Seleccione un cliente/proveedor..." id="ddlPersona" onchange="Compras.changePersona();">
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
                            </div>
                            <div class="row mb15">
                                <div class="col-sm-4 col-md-3 col-lg-2">
                                    <div class="form-group">
                                        <label class="control-label"><span class="asterisk">*</span> Tipo de comprobante</label>
                                        <asp:DropDownList runat="server" ID="ddlTipo" ClientIDMode="Static" CssClass="form-control required">
                                            <asp:ListItem Text="" Value=""></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div class="col-sm-4 col-md-3 col-lg-2">
                                    <div class="form-group">
                                        <label class="control-label"><span class="asterisk">*</span> Nro. Factura</label>
                                        <asp:TextBox runat="server" ID="txtNroFactura" TextMode="Phone" CssClass="form-control required" MaxLength="13"></asp:TextBox>
                                    </div>
                                </div>
                            </div>

                            <!-- Período facturado-->
                            <div class="row mb15">
                                <div class="col-sm-12">
                                    <hr />
                                    <h3>Período a imputar</h3>
                                    <label class="control-label">Ingrese la fecha contable, la fecha de emisión de la factura. y si desea la fecha del primer y segundo vencimiento.</label>
                                </div>
                            </div>


                            <div class="row mb15">
                                <div class="col-sm-4 col-md-3 col-lg-2">
                                    <div class="form-group">
                                        <label class="control-label"><span class="asterisk">*</span> Fecha de emisión</label>
                                        <div class="input-group">
                                            <span class="input-group-addon"><i class="glyphicon glyphicon-calendar"></i></span>
                                            <asp:TextBox runat="server" ID="txtFechaEmision" CssClass="form-control required validDate greaterThan" placeholder="dd/mm/yyyy" MaxLength="10"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-sm-4 col-md-3 col-lg-2">
                                    <div class="form-group">
                                        <label class="control-label"><span class="asterisk">*</span> Fecha contable</label>
                                        <div class="input-group">
                                            <span class="input-group-addon"><i class="glyphicon glyphicon-calendar"></i></span>
                                            <asp:TextBox runat="server" ID="txtFecha" CssClass="form-control required validDate" placeholder="dd/mm/yyyy" MaxLength="10"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-sm-4 col-md-3 col-lg-2">
                                    <div class="form-group">
                                        <label class="control-label">Fecha primer vencimiento</label>
                                        <div class="input-group">
                                            <span class="input-group-addon"><i class="glyphicon glyphicon-calendar"></i></span>
                                            <asp:TextBox runat="server" ID="txtFechaPrimerVencimiento" CssClass="form-control validDate greaterThan" placeholder="dd/mm/yyyy" MaxLength="10"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-sm-4 col-md-3 col-lg-2">
                                    <div class="form-group">
                                        <label class="control-label">Fecha segundo vencimiento</label>
                                        <div class="input-group">
                                            <span class="input-group-addon"><i class="glyphicon glyphicon-calendar"></i></span>
                                            <asp:TextBox runat="server" ID="txtFechaSegundoVencimiento" CssClass="form-control validDate" placeholder="dd/mm/yyyy" MaxLength="10"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>

                            </div>

                            <!-- Importes-->
                            <div class="row mb15">
                                <div class="col-sm-12">
                                    <hr />
                                    <h3>Importes</h3>
                                    <label class="control-label">Ingrese el o los importes detallados en su comprobante.</label>
                                </div>
                            </div>
                            <div class="row mb15">
                                <div class="col-sm-2" id="divImporteMon" style="display: none">
                                    <div class="form-group">
                                        <label class="control-label">Importe</label>
                                        <div class="input-group">
                                            <span class="input-group-addon">$</span>
                                            <asp:TextBox runat="server" ID="txtImporteMon" CssClass="form-control" TextMode="Phone" MaxLength="10"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-sm-2 ivas">
                                    <div class="form-group">
                                        <label class="control-label">Gravado al 21</label>
                                        <div class="input-group">
                                            <span class="input-group-addon">$</span>
                                            <asp:TextBox runat="server" ID="txtImporte21" CssClass="form-control" MaxLength="10" TextMode="Phone" onblur="Compras.changeIVA()"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-sm-2 ivas">
                                    <div class="form-group">
                                        <label class="control-label">Gravado al 27</label>
                                        <div class="input-group">
                                            <span class="input-group-addon">$</span>
                                            <asp:TextBox runat="server" ID="txtImporte27" CssClass="form-control" MaxLength="10" TextMode="Phone" onblur="Compras.changeIVA()"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-sm-3 col-md-2 col-lg-2 ivas">
                                    <div class="form-group">
                                        <label class="control-label">Gravado al 10,5</label>
                                        <div class="input-group">
                                            <span class="input-group-addon">$</span>
                                            <asp:TextBox runat="server" ID="txtImporte10" CssClass="form-control" MaxLength="10" TextMode="Phone" onblur="Compras.changeIVA()"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-sm-2 ivas">
                                    <div class="form-group">
                                        <label class="control-label">Gravado al 2,5</label>
                                        <div class="input-group">
                                            <span class="input-group-addon">$</span>
                                            <asp:TextBox runat="server" ID="txtImporte2" CssClass="form-control" MaxLength="10" TextMode="Phone" onblur="Compras.changeIVA()"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-sm-2 ivas">
                                    <div class="form-group">
                                        <label class="control-label">Gravado al 5</label>
                                        <div class="input-group">
                                            <span class="input-group-addon">$</span>
                                            <asp:TextBox runat="server" ID="txtImporte5" CssClass="form-control" MaxLength="10" TextMode="Phone" onblur="Compras.changeIVA()"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>


                            </div>
                            <div class="row mb15">
                                <div class="col-sm-2 ivas">
                                    <div class="form-group">
                                        <label class="control-label">No Gravado</label>
                                        <div class="input-group">
                                            <span class="input-group-addon">$</span>
                                            <asp:TextBox runat="server" ID="txtNoGravado" CssClass="form-control" TextMode="Phone" MaxLength="10"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-sm-2 ivas">
                                    <div class="form-group">
                                        <label class="control-label">Exento</label>
                                        <div class="input-group">
                                            <span class="input-group-addon">$</span>
                                            <asp:TextBox runat="server" ID="txtExento" CssClass="form-control" TextMode="Phone" MaxLength="10"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <!--Impuestos -->
                            <div id="divImpuestos">
                                <div class="row mb15">
                                    <div class="col-sm-12">
                                        <hr />
                                        <h3>Impuestos y Percepciones</h3>
                                        <label class="control-label">Ingrese el o los Impuestos y Percepciones detallados en su comprobante.</label>
                                    </div>
                                </div>

                                <div class="row mb15 ivas">
                                    <div class="col-sm-4 col-md-3 col-lg-2">
                                        <div class="form-group">
                                            <label class="control-label">IVA</label>
                                            <div class="input-group">
                                                <span class="input-group-addon">$</span>
                                                <asp:TextBox runat="server" ID="txtIva" CssClass="form-control" MaxLength="10" TextMode="Phone" onblur="Compras.verificarIVA();"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-sm-10"></div>
                                </div>
                                <div id="divRetenciones">
                                    <div class="row mb15">
                                        <div class="col-sm-4 col-md-3 col-lg-2">
                                            <div class="form-group">
                                                <label class="control-label">IIBB</label>
                                                <span class="badge badge-success pull-right tooltips" data-toggle="tooltip" data-original-title="Haga click aquí para agregar una jurisdicción" style="cursor: pointer" onclick="Compras.obtenerJurisdicciones();">Jurisdicciones</span>
                                                <div class="input-group">
                                                    <span class="input-group-addon">$</span>
                                                    <asp:TextBox runat="server" ID="txtIIBB" CssClass="form-control" MaxLength="10" TextMode="Phone" disabled="true"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-sm-4 col-md-3 col-lg-2">
                                            <div class="form-group">
                                                <label class="control-label">Percep. IVA</label>
                                                <div class="input-group">
                                                    <span class="input-group-addon">$</span>
                                                    <asp:TextBox runat="server" ID="txtPercepcionIVA" CssClass="form-control" TextMode="Phone" MaxLength="10"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-sm-4 col-md-3 col-lg-2">
                                            <div class="form-group">
                                                <label class="control-label">Nacionales</label>
                                                <div class="input-group">
                                                    <span class="input-group-addon">$</span>
                                                    <asp:TextBox runat="server" ID="txtImpNacionales" CssClass="form-control" TextMode="Phone" MaxLength="10"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row mb15">
                                        <div class="col-sm-4 col-md-3 col-lg-2">
                                            <div class="form-group">
                                                <label class="control-label">Municipales</label>
                                                <div class="input-group">
                                                    <span class="input-group-addon">$</span>
                                                    <asp:TextBox runat="server" ID="txtImpMunicipales" CssClass="form-control" TextMode="Phone" MaxLength="10"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-sm-4 col-md-3 col-lg-2">
                                            <div class="form-group">
                                                <label class="control-label">Internos</label>
                                                <div class="input-group">
                                                    <span class="input-group-addon">$</span>
                                                    <asp:TextBox runat="server" ID="txtImpInternos" CssClass="form-control" TextMode="Phone" MaxLength="10"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                        <div class="col-sm-4 col-md-3 col-lg-2">
                                            <div class="form-group">
                                                <label class="control-label">Otros</label>
                                                <div class="input-group">
                                                    <span class="input-group-addon">$</span>
                                                    <asp:TextBox runat="server" ID="txtOtros" CssClass="form-control" TextMode="Phone" MaxLength="10"></asp:TextBox>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                </div>

                            </div>

                            <!--Datos Complementarios -->
                            <div class="row mb15">
                                <div class="col-sm-12">
                                    <hr />
                                    <h3>Totales </h3>
                                    <label class="control-label">Estos son los totales del comprobante</label>
                                </div>
                            </div>

                            <div class="row mb15">
                                <div class="col-sm-4 col-md-3 col-lg-2">
                                    <div class="form-group">
                                        <h5 class="subtitle mb10">IMPUESTOS</h5>
                                        <h4 class="text-primary" style="color: green" id="divTotalImpuestos">$<asp:Literal runat="server" ID="liTotalImpuestos"></asp:Literal></h4>
                                    </div>
                                </div>

                                <div class="col-sm-4 col-md-3 col-lg-2">
                                    <div class="form-group">
                                        <h5 class="subtitle mb10">IMPORTES</h5>
                                        <h4 class="text-primary" style="color: green" id="divTotalImportes">$<asp:Literal runat="server" ID="liTotalImportes"></asp:Literal></h4>
                                    </div>
                                </div>

                                <div class="col-sm-4 col-md-3 col-lg-2">
                                    <div class="form-group">
                                        <h5 class="subtitle mb10">TOTAL</h5>
                                        <h4 class="text-primary" style="color: green" id="divTotal">$<asp:Literal runat="server" ID="litTotal"></asp:Literal></h4>
                                    </div>
                                </div>
                            </div>

                            <div class="row mb15">
                                <div class="col-sm-10 col-md-8 col-lg-5">
                                    <div class="form-group">
                                        <label class="control-label">                                           
                                                Detalle del Comprobante
                                        </label>
                                        <select class="select4" data-placeholder="Seleccione un detalle de comprobante..." id="ddlDetalleDelComprobante" onchange="Compras.irDetalleDelComprobante();">
                                            <option value=""></option>
                                        </select>
                                    </div>
                                </div>
                                <div class="col-sm-1">
                                    <div class="form-group">
                                        <a href="#" class="btn btn-warning" onclick="Compras.nuevoDetalleDelComprobante();" style="margin: 29px -10px; padding: 7px 15px">
                                            <i class="glyphicon glyphicon-plus"></i>
                                        </a>
                                    </div>
                                </div>
                            </div>

                            <!--Datos Complementarios -->
                            <div class="row mb15">
                                <div class="col-sm-12">
                                    <hr />
                                    <h3>Datos complementarios</h3>
                                    <label class="control-label">Estos datos podrían serte de utilidad para diferenciar tus gastos en los reportes ;-).</label>
                                </div>
                            </div>
                            <div class="row mb15">
                                <div class="col-sm-4 col-md-2">
                                    <div class="form-group">
                                        <label class="control-label">Categoría</label>
                                        <span class="badge badge-success pull-right tooltips" data-toggle="tooltip" data-original-title="Haga click aquí para agregar/modificar/eliminar categorías" style="cursor: pointer" onclick="Compras.obtenerCategorias();">administrar</span>
                                        <asp:DropDownList runat="server" ID="ddlCategoria" CssClass="form-control chosen-select"
                                            data-placeholder="Seleccione una categoria...">
                                        </asp:DropDownList>
                                        <span class="help-block"><small>Asigna una categoría para poder segmentar tus egresos.</small></span>
                                    </div>
                                </div>
                                <div class="col-sm-4 col-md-2">
                                    <div class="form-group">
                                        <label class="control-label">Rubro</label>
                                        <asp:DropDownList runat="server" ID="ddlRubro" CssClass="form-control">
                                            <asp:ListItem Text="" Value=""></asp:ListItem>
                                            <asp:ListItem Text="Bienes" Value="Bienes"></asp:ListItem>
                                            <asp:ListItem Text="Locaciones" Value="Locaciones"></asp:ListItem>
                                            <asp:ListItem Text="Servicios" Value="Servicios"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div class="col-sm-4 col-md-2 divPlanDeCuentas">
                                    <div class="form-group">
                                        <label class="control-label"><span class="asterisk">*</span> Cuenta contable</label>
                                        <asp:DropDownList runat="server" ID="ddlPlanDeCuentas" CssClass="select3 required" data-placeholder="Seleccione una cuenta...">
                                        </asp:DropDownList>
                                    </div>
                                </div>
                            </div>
                            <div class="row mb15">
                                <div class="col-sm-12 col-md-9 col-lg-6">
                                    <div class="form-group" id="divLogo" style="display: none">
                                        <label class="control-label">Si desea puede adjuntar una foto del comprobante</label>
                                        <div class="fileinput fileinput-new input-group" data-provides="fileinput">
                                            <div class="form-control" data-trigger="fileinput" style="height: 40px">
                                                <i class="glyphicon glyphicon-file fileinput-exists" id="iImgFileFoto"></i>
                                                <span class="fileinput-filename"></span>
                                            </div>
                                            <span class="input-group-addon btn btn-default btn-file">
                                                <span class="fileinput-new">
                                                    <i class="glyphicon glyphicon-folder-open"></i>&nbsp;&nbsp;Seleccionar
                                                </span>
                                                <span class="fileinput-exists">
                                                    <i class="glyphicon glyphicon-folder-open"></i>&nbsp;&nbsp;Modificar
                                                </span>
                                                <input id="flpArchivo" type="file" />
                                            </span>
                                            <a href="#" class="input-group-addon btn btn-default fileinput-exists" data-dismiss="fileinput">
                                                <i class="glyphicon glyphicon-ban-circle"></i>&nbsp;&nbsp;Remover
                                            </a>
                                        </div>
                                        <asp:HiddenField runat="server" ID="hdnFileName" Value="" />
                                        <div class="col-sm-12" id="divComprobante">
                                            <asp:HyperLink runat="server" ID="lnkComprobante">Descargar comprobante</asp:HyperLink>
                                        </div>
                                    </div>
                                    <div class="col-sm-6" id="divAdjuntarFoto">
                                        <a class="btn btn-white btn-block" onclick="Compras.showInputFoto();">Adjuntar foto</a>
                                    </div>
                                    <div class="col-sm-6" id="divEliminarFoto">
                                        <a class="btn btn-white btn-block" onclick="Compras.eliminarFoto();">Eliminar foto</a>
                                    </div>

                                </div>
                            </div>
                            <div class="row mb15">
                                <div class="col-sm-10 col-md-8 col-lg-6">
                                    <div class="form-group">
                                        <label class="control-label">Observaciones</label>
                                        <asp:TextBox runat="server" TextMode="MultiLine" Rows="5" ID="txtObservaciones" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>
                            </div>

                        </asp:Panel>
                    </div>

                    <div class="panel-footer" runat="server" id="divFooter">
                        <a class="btn btn-success" id="btnActualizar" onclick="Compras.grabarsinImagen();">Aceptar</a>
                        <a href="#" onclick="Compras.cancelar();" style="margin-left: 20px">Cancelar</a>
                        <a runat="server" class="btn btn-white" onclick="Compras.descargarAdjunto();" style="float: right;" id="lnkDescargarAdjunto"><i class="fa fa-file fa-fw mr5"></i>Descargar adjunto</a>
                    </div>
                </div>

                <asp:HiddenField runat="server" ID="hdnTieneFoto" Value="0" />
                <asp:HiddenField runat="server" ID="hdnSinCombioDeFoto" Value="0" />

                <asp:HiddenField runat="server" ID="hdnUsaPlanCorporativo" Value="" />
                <asp:HiddenField runat="server" ID="hdnIdusuario" Value="0" ClientIDMode="Static" />
                <asp:HiddenField runat="server" ID="hdnID" Value="0" />
                <asp:HiddenField runat="server" ID="hdnIDPersona" Value="0" />
                <asp:HiddenField runat="server" ID="hdnTipoComprobante" Value="0" />
                <asp:HiddenField runat="server" ID="hdnTieneSaldoAPagar" Value="0" />
                <asp:HiddenField runat="server" ID="hdnEsAgenteRetencionGanancia" Value="0" />
<%--                <asp:HiddenField runat="server" ID="hdnPageLoad" Value="1" />--%>
            </form>
        </div>
    </div>

    <!-- Modal -->
    <div class="modal modal-wide fade" id="modalCategorias" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Administración de categorías</h4>
                </div>
                <div class="modal-body">
                    <div>
                        <div class="alert alert-danger" id="divErrorCat" style="display: none">
                            <strong>Lo sentimos! </strong><span id="msgErrorCat"></span>
                        </div>

                        <div class="col-sm-6">
                            <div class="form-group">
                                <input type="text" maxlength="50" id="txtNuevaCat" class="form-control" />
                            </div>
                        </div>
                        <div class="col-sm-2"><a class="btn btn-default" id="btnCategoria" onclick="Compras.grabarCategoria();">Agregar</a></div>

                        <br />
                        <br />
                        <br />

                        <div class="table-responsive">
                            <table class="table mb30">
                                <thead>
                                    <tr>
                                        <th>Nombre</th>
                                        <th></th>
                                    </tr>
                                </thead>
                                <tbody id="bodyDetalle">
                                </tbody>
                            </table>
                        </div>
                    </div>
                    <input type="hidden" id="hdnIDCategoria" value="0" />
                </div>
                <div class="modal-footer">
                    <a style="margin-left: 20px" href="#" data-dismiss="modal">Cerrar</a>
                </div>
            </div>
        </div>
    </div>



    <div class="modal modal-wide fade" id="modalJurisdiccion" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Seleccione las jurisdicciones correspondientes</h4>
                </div>
                <div class="modal-body">
                    <div>
                        <div class="alert alert-danger" id="divErrorJurisdiccion" style="display: none">
                            <strong>Lo sentimos! </strong><span id="msgErrorJurisdiccion"></span>
                        </div>
                        <form id="frmEdicionJurisdiccion">
                            <div class="col-sm-4">
                                <div class="form-group">
                                    <label class="control-label"><span class="asterisk">*</span> Jurisdicción</label>
                                    <select id="ddlJurisdiccion" class="select3 required" data-placeholder="seleccione una jurisdicción">
                                    </select>
                                </div>
                            </div>
                            <div class="col-sm-3">
                                <div class="form-group">
                                    <label class="control-label"><span class="asterisk">*</span> Importe</label>
                                    <input type="text" maxlength="8" id="txtImporteJurisdiccion" class="form-control required" />
                                </div>
                            </div>
                        </form>

                        <div class="col-sm-3">
                            <div class="form-group">
                                <br />
                                <div class="col-sm-2"><a class="btn btn-default" id="btnJurisdiccion" onclick="Compras.grabarJurisdiccion(1);" style="margin-top: 7px;">Agregar</a></div>
                            </div>
                        </div>


                        <br />
                        <br />
                        <br />

                        <div class="table-responsive">
                            <table class="table mb30">
                                <thead>
                                    <tr>
                                        <th>Jurisdiccion</th>
                                        <th>Importe</th>
                                        <th class="columnIcons"></th>
                                    </tr>
                                </thead>
                                <tbody id="bodyDetalleJurisdiccion">
                                </tbody>
                            </table>
                        </div>
                    </div>
                    <input type="hidden" id="hdnIDJurisdiccion" value="0" />
                </div>
                <div class="modal-footer">
                    <a style="margin-left: 20px" href="#" data-dismiss="modal">Cerrar</a>
                </div>
            </div>
        </div>
    </div>




    <!-- modal-->
    <div class="modal modal fade" id="modalPagos" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true" data-backdrop="static" data-keyboard="false">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true" onclick="window.location.href='compras.aspx'">&times;</button>
                    <h4 class="modal-title" id="litModalOkTitulo">Compra cargada correctamente. </h4>
                </div>
                <div class="modal-body" style="min-height: 200px;">
                    <div id="divToolbar">
                        <div class="col-sm-3 CAJA_BLANCA_AZUL">
                            <a id="lnkDetalleDeComprobante" href="javascript:Compras.nuevoDetalleDelComprobante();">
                                <span class="glyphicon glyphicon-plus" style="font-size: 20px;"></span>
                                <br />
                                <span>Cargar Detalle Comprobante</span>
                            </a>
                        </div>
                        <div class="col-sm-3 CAJA_BLANCA_AZUL">
                            <a id="lnk100" href="javascript:Compras.registrarPago(100);">
                                <span class="glyphicon glyphicon-usd" style="font-size: 30px;"></span>
                                <br />
                                <span>Pagar el 100%</span>
                            </a>
                        </div>
                        <div class="col-sm-3 CAJA_BLANCA_AZUL">
                            <a id="lnk50" href="javascript:Compras.registrarPago(50);">
                                <span class="glyphicon glyphicon-usd" style="font-size: 30px;"></span>
                                <br />
                                <span>Pagar el 50%</span>
                            </a>
                        </div>
                        <div class="col-sm-3 CAJA_BLANCA_AZUL">
                            <a id="lnk30" href="javascript:Compras.registrarPago(30);">
                                <span class="glyphicon glyphicon-usd" style="font-size: 30px;"></span>
                                <br />
                                <span>Pagar el 30%</span>
                            </a>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <%--<button type="button" class="btn btn-primary" onclick="Compras.registrarPago()">Registrar pago</button>--%>
                    <a style="margin-left: 20px" href="#" data-dismiss="modal" onclick="window.location.href='compras.aspx'">Cerrar</a>
                </div>
            </div>
        </div>
    </div>

    <script id="resultTemplate" type="text/x-jQuery-tmpl">
        {{each results}}
        <tr>
            <td>${NombreJurisdiccion}</td>
            <td>${Importe}</td>
            <td class="table-action">
                <a onclick="Compras.eliminarJurisdiccion(${IDJurisdicion});" style="cursor: pointer; font-size: 16px" class="delete-row" title="Eliminar"><i class="fa fa-trash-o"></i></a>
            </td>
        </tr>
        {{/each}}
    </script>

    <script id="noResultTemplate" type="text/x-jQuery-tmpl">
        <tr>
            <td colspan="3">No se han encontrado resultados
            </td>
        </tr>
    </script>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FooterContent" runat="Server">
    <script type="text/javascript" src="/js/jquery.tmpl.min.js"></script>
    <script src="js/numeral.min.js"></script>
    <script src="/js/select2.min.js"></script>
    <script src="/js/jquery.validate.min.js"></script>
    <script src="/js/jquery.maskedinput.min.js"></script>
    <script src="/js/jquery.maskMoney.min.js"></script>
    <script src="/js/views/compras.js?v=<%= ConfigurationManager.AppSettings["JS.Version"] %>"></script>
    <script src="/js/jquery.fileupload.js" type="text/javascript"></script>

    <script src="/js/jasny-bootstrap.min.js"></script>
    <script src="/js/jquery.ui.widget.js" type="text/javascript"></script>
    <script src="/js/jquery.iframe-transport.js" type="text/javascript"></script>

    <UC:NuevaPersona runat="server" ID="ucCliente" />

    <script>
        jQuery(document).ready(function () {
            Compras.configForm();
            $("#hdnNuevaPersonaTipo").val("P");
        });
    </script>
</asp:Content>
