<%@ Page Title="" Language="C#" MasterPageFile="~/Front.master" AutoEventWireup="true" CodeFile="pagose.aspx.cs" Inherits="pagose" %>

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
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="pageheader">
        <h2><i class="fa fa-file-text"></i>Pagos </h2>
        <div class="breadcrumb-wrapper">
            <span class="label">Estás aquí:</span>
            <ol class="breadcrumb">
                <li><a href="/home.aspx">axanweb</a></li>
                <li><a href="/pagos.aspx">Pagos</a></li>
                <li class="active">
                    <asp:Literal runat="server" ID="litPath"></asp:Literal></li>
            </ol>
        </div>
    </div>

    <div class="contentpanel">

        <div class="row">
            <form id="frmEdicion" runat="server" class="col-sm-12">
                <div class="panel panel-default">
                    <div class="alert alert-danger" id="divError" style="display: none">
                        <strong>Lo sentimos! </strong><span id="msgError"></span>
                    </div>
                    <div class="alert alert-success" id="divOk" style="display: none">
                        <strong>Bien hecho! </strong>Los datos se han actualizado correctamente
                    </div>

                    <div class="panel-body">

                        <asp:Panel runat="server" ID="pnlContenido">

                            <div class="row mb15">
                                <div class="col-sm-12">
                                    <h3>1. Datos Generales</h3>
                                    <label class="control-label">Acá van los datos más generales del pago.</label>
                                </div>
                            </div>
                            <div class="row mb15">
                                <div class="col-sm-5">
                                    <div class="form-group">
                                        <label class="control-label"><span class="asterisk">*</span> Proveedor</label>
                                        <select class="select2" data-placeholder="Seleccione un proveedor..." id="ddlPersona">
                                            <%--onchange="changePersona();"--%>
                                            <option value=""></option>
                                        </select>
                                    </div>
                                </div>
                                <div class="col-sm-1 hide" id="divNuevoCliente">
                                    <div class="form-group">
                                        <a href="#" class="btn btn-warning" data-toggle="modal" data-target="#modalNuevoCliente" style="margin: 29px -10px; padding: 7px 15px">
                                            <i class="glyphicon glyphicon-plus"></i>
                                        </a>
                                    </div>
                                </div>

                                <div class="col-sm-2 col-md-2">
                                    <div class="form-group">
                                        <label class="control-label"><span class="asterisk">*</span> Fecha de pago</label>
                                        <div class="input-group">
                                            <span class="input-group-addon"><i class="glyphicon glyphicon-calendar"></i></span>
                                            <asp:TextBox runat="server" ID="txtFecha" CssClass="form-control required validDate" placeholder="dd/mm/yyyy" MaxLength="10"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div id="divFactura" style="display: none">

                                <%--    <div class="row mb15 hide">
                                    <div class="col-sm-6">
                                        <h5 class="subtitle mb10">De</h5>
                                        <address>
                                            <strong>
                                                <asp:Literal runat="server" ID="litRazonSocial"></asp:Literal>
                                            </strong>
                                            <br />
                                            <asp:Literal runat="server" ID="litDomicilio"></asp:Literal><br />
                                            <asp:Literal runat="server" ID="litPaisCiudad"></asp:Literal><br />
                                            <abbr title="Email">Email:</abbr>
                                            <asp:Literal runat="server" ID="litEmail"></asp:Literal><br />
                                            <abbr title="Phone">Tel:</abbr>
                                            <asp:Literal runat="server" ID="litTelefono"></asp:Literal>
                                        </address>
                                    </div>

                                    <div class="col-sm-6 text-right">
                                        <h5 class="subtitle mb10">Para</h5>
                                        <address>
                                            <strong id="litPersonaRazonSocial"></strong>
                                            <br />
                                            <span id="litPersonaDomicilio"></span>
                                            <br />
                                            <span id="litPersonaPaisCiudad"></span>
                                            <br />
                                            <abbr title="Email">Email:</abbr>
                                            <span id="litPersonaEmail"></span>
                                            <br />
                                            <abbr title="Phone">Tel:</abbr>
                                            <span id="litPersonaTelefono"></span>
                                        </address>
                                        <p id="litPersonaCondicionIva"></p>
                                    </div>
                                </div>--%>

                                <div class="mb40"></div>

                                <div class="col-sm-12">
                                    <h3>2. Datos del pago</h3>
                                </div>

                                <div class="row mb15">
                                    <div class="col-sm-12">
                                        <h4>Comprobantes a pagar</h4>
                                        <label class="control-label">Seleccione los comprobantes que desea pagar.</label>
                                    </div>
                                </div>

                                <div class="well">
                                    <div class="alert alert-danger" id="divErrorDetalle" style="display: none">
                                        <strong>Lo siento! </strong><span id="msgErrorDetalle"></span>
                                    </div>
                                    <div class="row">
                                        <div class="col-sm-2 col-md-2">
                                            <span class="asterisk">*</span> Comprobante
                                            <select class="form-control input-sm chosen-select" data-placeholder="Seleccione un comprobante" id="ddlComprobante" onchange="changeComprobante();">
                                                <option value=""></option>
                                            </select>
                                        </div>
                                        <div class="col-sm-2 col-md-2">
                                            Saldo
                                            <br />
                                            <p id="lblSaldo" style="color: red; margin-top: 5px; font-size: 12pt;"></p>
                                        </div>                                        
                                        <div class="col-sm-3 col-md-3">
                                            <span class="asterisk">*</span> Importe
                                            <input type="text" class="form-control input-sm" maxlength="15" id="txtImporte" />
                                        </div>
                                        <div class="col-sm-2 col-md-2">
                                            <br />
                                            <a class="btn btn-default btn-sm" id="btnAgregarItem" onclick="agregarItem();">Agregar</a>
                                            <a class="btn btn-default btn-sm" id="btnCancelarItem" onclick="cancelarItem();">Cancelar</a>
                                            <input type="hidden" runat="server" id="hdnIDItem" value="0" />
                                        </div>
                                    </div>
                                </div>

                                <div class="table-responsive">
                                    <table class="table table-invoice">
                                        <thead>
                                            <tr>
                                                <th>#</th>
                                                <th style="text-align: left; width: 50%">Comprobante</th>
                                                <th style="text-align: right">Importe</th>
                                                <%--<th style="text-align:right">Ret Gan.</th>
                                                <th style="text-align:right">Ret IIBB</th>
                                                <th style="text-align:right">Ret SUSS</th>
                                                <th style="text-align:right">Otras Ret</th>
                                                <th style="text-align:right">Subtotal</th>--%>
                                                <th></th>
                                            </tr>
                                        </thead>
                                        <tbody id="bodyDetalle">
                                            <tr>
                                                <td colspan='3'>No tienes items agregados</td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </div>
                                <div class="mb40"></div>

                                <!-- table-responsive -->
                                <table class="table table-total">
                                    <tbody>
                                        <tr>
                                            <td><strong>TOTAL A PAGAR:</strong></td>
                                            <td id="divTotal" style="color: green">$ 0</td>
                                        </tr>
                                    </tbody>
                                </table>

                                <div class="mb40"></div>

                                <div class="row mb15">
                                    <div class="col-sm-12">
                                        <h4>Forma de pago</h4>
                                        <label class="control-label">Elija una o varias formas de pago.</label>
                                    </div>
                                </div>

                                <div class="well">
                                    <div class="alert alert-danger" id="divErrorForma" style="display: none">
                                        <strong>Lo siento! </strong><span id="msgErrorForma"></span>
                                    </div>

                                    <div class="row">
                                        <div class="col-sm-2 col-md-2">
                                            <span class="asterisk">*</span> Forma de pago
                                            <asp:DropDownList runat="server" ID="ddlFormaPago" data-placeholder="Seleccione una forma de pago" CssClass="form-control input-sm chosen-select" ClientIDMode="Static" onchange="changeFormas();">
                                                <asp:ListItem Text="" Value=""></asp:ListItem>
                                                <asp:ListItem Text="Cheque de Terceros" Value="Cheque Propio"></asp:ListItem>
                                                <asp:ListItem Text="Cheque Propio" Value="Cheque Tercero"></asp:ListItem>
                                                <asp:ListItem Text="Cheque Empresa" Value="Cheque Empresa"></asp:ListItem>
                                                <asp:ListItem Text="Débito" Value="Débito"></asp:ListItem>
                                                <asp:ListItem Text="Depósito" Value="Depósito"></asp:ListItem>
                                                <asp:ListItem Text="Efectivo" Value="Efectivo"></asp:ListItem>
                                                <asp:ListItem Text="Transferencia" Value="Transferencia"></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>

                                        <div class="col-sm-2 col-md-2" id="divBancos" style="display: none">
                                            <span class="asterisk">*</span> Banco
                                            <asp:DropDownList runat="server" ID="ddlBancos" data-placeholder="" CssClass="form-control input-sm chosen-select" ClientIDMode="Static">
                                            </asp:DropDownList>
                                        </div>

                                        <div class="col-sm-2 col-md-2" id="divCheque" style="display: none">
                                            <span class="asterisk">*</span> Cheques
                                            <asp:DropDownList runat="server" ID="ddlCheque" data-placeholder="Seleccione un cheque" CssClass="select2" ClientIDMode="Static" onchange="changeChequeTercero();">
                                                <asp:ListItem Text="" Value=""></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>

                                        <div class="col-sm-1" id="divNuevoCheque" style="display: none">
                                            <div class="form-group">
                                                <a href="#" class="btn btn-warning" data-toggle="modal" data-target="#modalNuevoCheque" style="margin: 25px -10px; padding: 3px 13px" onclick="abrirModalCheque();">
                                                    <i class="glyphicon glyphicon-plus"></i>
                                                </a>
                                            </div>
                                        </div>

<%--                                        <div class="col-sm-1" id="divNuevoCheque" style="display: none">
                                            <div class="form-group">
                                                <a href="#" class="btn btn-warning" data-toggle="modal" data-target="#modalNuevoCheque" style="margin: 25px -10px; padding: 3px 13px" onclick="Common.showModalCheque('1');">
                                                    <i class="glyphicon glyphicon-plus"></i>
                                                </a>
                                            </div>
                                        </div>--%>


                                        <div class="col-sm-2 col-md-2">
                                            Nro Referencia
                                            <br />
                                            <input type="text" class="form-control input-sm" maxlength="20" id="txtNroRef" />
                                        </div>
                                        <div class="col-sm-1 col-md-1">
                                            <span class="asterisk">*</span> Importe
                                            <input type="text" class="form-control input-sm" maxlength="10" id="txtImporteForma" />
                                        </div>
                                        <div class="col-sm-2 col-md-2">
                                            <br />
                                            <a class="btn btn-default btn-sm" id="btnAgregarForma" onclick="agregarForma();">Agregar</a>
                                            <a class="btn btn-default btn-sm" id="btnCancelarForma" onclick="cancelarForma();">Cancelar</a>
                                            <input type="hidden" runat="server" id="hdnIDForma" value="0" />
                                        </div>
                                    </div>
                                </div>

                                <div class="table-responsive">
                                    <table class="table table-invoice">
                                        <thead>
                                            <tr>
                                                <th>#</th>
                                                <th style="text-align: left; width: 50%">Forma de pago</th>
                                                <th style="text-align: left">Nro Referencia</th>
                                                <th style="text-align: right">Importe</th>
                                                <th></th>
                                            </tr>
                                        </thead>
                                        <tbody id="bodyFormas">
                                            <tr>
                                                <td colspan='5'>No tienes items agregados</td>
                                            </tr>
                                        </tbody>
                                    </table>
                                </div>

                                <div class="mb40"></div>

                                <!-- table-responsive -->
                                <table class="table table-total">
                                    <tbody>
                                        <tr>
                                            <td><strong>RESTANTE A PAGAR:</strong></td>
                                            <td id="tdTotalFormaDePago" style="color: green">$ 0</td>
                                        </tr>
                                    </tbody>
                                </table>

                                <div class="mb40"></div>

                                
                                <div class="col-sm-12">
                                    <h3>3. Información adicional</h3>
                                </div>


                                <div id="Divretenciones">
                                    <div class="row mb15">
                                        <div class="col-sm-12">
                                            <h4>Retenciones emitidas</h4>
                                            <label class="control-label">Si emitió alguna retención ingréselas aquí.</label>
                                        </div>
                                    </div>

                                    <div class="well">
                                        <div class="alert alert-danger" id="divErrorRet" style="display: none">
                                            <strong>Lo siento! </strong><span id="msgErrorRet"></span>
                                        </div>

                                        <div class="row">
                                            <div class="col-sm-2 col-md-2">
                                                <span class="asterisk">*</span> Tipo
                                                <asp:DropDownList runat="server" ID="ddlTipoRet" data-placeholder="Seleccione una retencion" CssClass="form-control input-sm chosen-select">
                                                    <asp:ListItem Text="" Value=""></asp:ListItem>
                                                    <asp:ListItem Text="Ganancias" Value="Ganancias"></asp:ListItem>
                                                    <asp:ListItem Text="IIBB" Value="IIBB"></asp:ListItem>
                                                    <asp:ListItem Text="IVA" Value="IVA"></asp:ListItem>
                                                    <%--  <asp:ListItem Text="SUSS" Value="SUSS"></asp:ListItem>--%>
                                                </asp:DropDownList>

                                            </div>
                                            <div class="col-sm-2 col-md-2">
                                                Nro Referencia
                                                <br />
                                                <input type="text" class="form-control input-sm" maxlength="20" id="txtNroRefRet" />
                                            </div>
                                            <div class="col-sm-1 col-md-1">
                                                <span class="asterisk">*</span> Importe
                                                <input type="text" class="form-control input-sm" maxlength="10" id="txtImporteRet" />
                                            </div>
                                            <div class="col-sm-2 col-md-2">
                                                <br />
                                                <a class="btn btn-default btn-sm" id="btnAgregarRet" onclick="CobRetenciones.agregarRet();">Agregar</a>
                                                <a class="btn btn-default btn-sm" id="btnCancelarRet" onclick="CobRetenciones.cancelarRet();">Cancelar</a>
                                                <input type="hidden" runat="server" id="hdnIDRet" value="0" />
                                            </div>
                                        </div>
                                    </div>

                                    <div class="table-responsive">
                                        <table class="table table-invoice">
                                            <thead>
                                                <tr>
                                                    <th>#</th>
                                                    <th style="text-align: left; width: 50%">Tipo</th>
                                                    <th style="text-align: left">Nro Referencia</th>
                                                    <th style="text-align: right">Importe</th>
                                                    <th></th>
                                                </tr>
                                            </thead>
                                            <tbody id="bodyRetenciones">
                                                <tr>
                                                    <td colspan='5'>No tienes items agregados</td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </div>
                                </div>
                                <div class="mb40"></div>

                                <div class="row mb15">
                                    <div class="col-sm-12">
                                        <h4>Datos complementarios</h4>
                                        <label class="control-label">Este dato no es necesario pero podría serte de utilidad ;-)</label>
                                    </div>
                                </div>

                                <div class="row">
                                    <div class="form-group">
                                        <label class="control-label">Observaciones</label>
                                        <asp:TextBox runat="server" TextMode="MultiLine" Rows="5" ID="txtObservaciones" CssClass="form-control"></asp:TextBox>
                                    </div>
                                </div>

                                <div class="mb40"></div>

                                <div class="text-right btn-invoice">
                                </div>
                            </div>

                        </asp:Panel>
                    </div>
                    <div class="panel-footer" runat="server" id="divFooter">
                        <a class="btn btn-success" onclick="grabar(false);" id="lnkAceptar">Aceptar</a>                        
                        <a href="#" onclick="cancelar();" style="margin-left: 20px">Cancelar</a>
                        <div id="divImprimirRetencionGanancia"><a class="btn btn-white" onclick="imprimirRetencionGanancia();" style="float: right;" id="lnkImprimirRetencionGanancia"><i class="fa fa-desktop fa-fw mr5"></i>Imprimir Retención Ganancia</a></div>
                        <div id="divImprimir"><a class="btn btn-white" onclick="imprimir();" style="float: right;" id="lnkImprimir"><i class="fa fa-desktop fa-fw mr5"></i>Imprimir</a></div>
                    </div>

                    <%--<asp:HiddenField runat="server" ID="hdnIdusuario" Value="0" ClientIDMode="Static" />--%>
                    <asp:HiddenField runat="server" ID="hdnID" Value="0" />
                    <asp:HiddenField runat="server" ID="hdnIDPersona" Value="0" />
                    <asp:HiddenField runat="server" ID="hdnCargarDatosDesdeCompra" Value="0" />
                    <asp:HiddenField runat="server" ID="hdnEsAgenteRetencionGanancia" Value="0" />
                    <input type="hidden" id="hdnRazonSocial" />
                    <input type="hidden" id="txtImporteNeto" />
                </div>
            </form>
        </div>
    </div>

    <div class="modal modal fade" id="modalOk" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true" data-backdrop="static" data-keyboard="false">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <div class="alert alert-danger" id="divErrorModalOk" style="display: none">
                        <span id="msgErrorModalOk"></span>
                    </div>
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true" onclick="window.location.href='pagos.aspx'">&times;</button>
                    <h4 class="modal-title" id="litModalOkTitulo">Pago generado correctamente. </h4>
                </div>
                <div class="modal-body" style="min-height: 200px;">
                    <div id="divToolbar">
                        <div id="divDownloadPago" class="col-sm-3 CAJA_BLANCA_AZUL">
                            <a href="#" id="lnkDownloadPago" onclick="imprimir();">
                                <span class="fa fa-file-text" style="font-size: 25px;"></span>
                                <br />
                                Descargar Pago
                            </a>
                        </div>
                        <div id="divDownloadRetencionGanancia" class="col-sm-3 CAJA_BLANCA_AZUL">
                            <a href="#" id="lnkDownloadRetencionGanancia" onclick="imprimirRetencionGananciaPopUp();">
                                <span class="fa fa-file-text" style="font-size: 25px;"></span>
                                <br />
                                Descargar Retencion Ganancia
                            </a>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <a style="margin-left: 20px" href="#" data-dismiss="modal" onclick="window.location.href='pagos.aspx'">Cerrar</a>
                </div>
            </div>
        </div>
    </div>

    <!-- Modal -->
    <div class="modal modal-wide fade" id="modalPdf" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Visualización del Recibo de Pago</h4>
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
                    <asp:HyperLink runat="server" ID="lnkDescargar" CssClass="btn btn-primary" download><i class="fa fa-cloud-download fa-fw"></i> Descargar</asp:HyperLink>
                    <asp:HyperLink runat="server" ID="lnkPrint2" CssClass="btn btn-default"><span class="glyphicon glyphicon-print"></span> Imprimir</asp:HyperLink>
                    <a style="margin-left: 20px" href="#" data-dismiss="modal">Cerrar</a>
                </div>
            </div>
        </div>
    </div>

    


</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FooterContent" runat="Server">
    <script src="/js/select2.min.js"></script>
    <script src="/js/jquery.validate.min.js"></script>
    <script src="/js/jquery.maskedinput.min.js"></script>
    <script src="/js/jquery.maskMoney.min.js"></script>
    <script src="/js/views/pagos.js?v=<%= ConfigurationManager.AppSettings["JS.Version"] %>"></script>
    <%--<script src="/js/views/common.js"></script>--%>

    <%--<UC:NuevaPersona runat="server" ID="ucCliente" />--%>
    <UC:NuevoCheque runat="server" ID="ucCheque" />
    <script>
        jQuery(document).ready(function () {
            configForm();
            $("#hdnNuevaPersonaTipo").val("P");
        });
    </script>
</asp:Content>
