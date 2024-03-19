<%@ Page Title="" Language="C#" MasterPageFile="~/Front.master" AutoEventWireup="true" CodeFile="cobranzase.aspx.cs" Inherits="cobranzase" %>

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
        <h2><i class="fa fa-file-text"></i>Cobranzas </h2>
        <div class="breadcrumb-wrapper">
            <span class="label">Estás aquí:</span>
            <ol class="breadcrumb">
                <li><a href="/home.aspx">axanweb</a></li>
                <li><a href="/cobranzas.aspx">Cobranzas</a></li>
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
                                    <h3>1. Datos generales</h3>
                                    <label class="control-label">Acá van los datos más generales de la cobranza</label>
                                </div>
                            </div>
                            <div class="row mb15">
                                <div class="col-sm-5">
                                    <div class="form-group">
                                        <label class="control-label"><span class="asterisk">*</span> Proveedor/Cliente</label>
                                        <select class="select2" data-placeholder="Seleccione un cliente/proveedor..." id="ddlPersona">
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
                                <div class="col-sm-2 col-md-2 hide">
                                    <div class="form-group">
                                        <label class="control-label"><span class="asterisk">*</span> Tipo de comprobante</label>
                                        <select class="form-control required" id="ddlTipo">
                                            <option value="RC">RC</option>
                                        </select>
                                    </div>
                                </div>
                                <div class="col-sm-2 col-md-2">
                                    <label class="control-label"><span class="asterisk">*</span> Nro. Recibo</label>
                                    <div class="form-group">
                                        <div class="col-sm-6">
                                            <select id="ddlPuntoVenta" runat="server" class="form-control required" onchange="changeTipoComprobante();"></select>
                                        </div>
                                        <div class="col-sm-6">
                                            <asp:TextBox runat="server" ID="txtNumero" CssClass="form-control required" MaxLength="9"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>

                                <div class="col-sm-2 col-md-2">
                                    <div class="form-group">
                                        <label class="control-label"><span class="asterisk">*</span> Fecha</label>
                                        <div class="input-group">
                                            <span class="input-group-addon"><i class="glyphicon glyphicon-calendar"></i></span>
                                            <asp:TextBox runat="server" ID="txtFecha" CssClass="form-control required validDate" placeholder="dd/mm/yyyy" MaxLength="10"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div id="divFactura" style="display: none">
                                <div class="row" id="divPuntoYNro">
                                </div>

                                <div class="mb40"></div>

                                <div class="col-sm-12">
                                    <h3>2. Datos de la cobranza</h3>
                                </div>

                                <div class="row mb15">
                                    <div class="col-sm-12">
                                        <h4>Comprobantes a cobrar</h4>
                                        <label class="control-label">Seleccione los comprobantes que desea cobrar.</label>
                                    </div>
                                </div>

                                <div class="well">
                                    <div class="alert alert-danger" id="divErrorDetalle" style="display: none">
                                        <strong>Lo siento! </strong><span id="msgErrorDetalle"></span>
                                    </div>

                                    <div class="row">
                                        <div class="col-sm-4 col-md-2">
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
                                        <div class="col-sm-2 col-md-2">
                                            <span class="asterisk">*</span> Importe
                                            <input type="tel" class="form-control input-sm" maxlength="10" id="txtImporte"/>
                                        </div>
                                        <div class="col-sm-3 col-md-2">
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
                                            <td><strong>TOTAL A COBRAR:</strong></td>
                                            <td id="divTotal" style="color: green">$ 0</td>
                                        </tr>
                                    </tbody>
                                </table>

                                <div class="mb40"></div>

                                <div class="row mb15">
                                    <div class="col-sm-12">
                                        <h4>Forma de cobro</h4>
                                        <label class="control-label">Elija una o varias formas de pago.</label>
                                    </div>
                                </div>

                                <div class="well">
                                    <div class="alert alert-danger" id="divErrorForma" style="display: none">
                                        <strong>Lo siento! </strong><span id="msgErrorForma"></span>
                                    </div>

                                    <div class="row">
                                        <div class="col-sm-4 col-md-2">
                                            <span class="asterisk">*</span> Forma de cobro
                                            <asp:DropDownList runat="server" ID="ddlFormaPago" 
                                                data-placeholder="Seleccione una forma de cobro" CssClass="form-control input-sm chosen-select" 
                                                ClientIDMode="Static" onchange="changeFormas();">
                                                <asp:ListItem Text="" Value=""></asp:ListItem>
                                                <asp:ListItem Text="Cheque de Tercero" Value="Cheque Tercero"></asp:ListItem>
                                                <asp:ListItem Text="Cheque propio" Value="Cheque Propio"></asp:ListItem>
                                                <asp:ListItem Text="Cheque Empresa" Value="Cheque Empresa"></asp:ListItem>
                                                <asp:ListItem Text="Depósito" Value="Depósito"></asp:ListItem>
                                                <asp:ListItem Text="Efectivo" Value="Efectivo"></asp:ListItem>
                                                <asp:ListItem Text="Transferencia" Value="Transferencia"></asp:ListItem>
                                                <asp:ListItem Text="Nota de credito" Value="Nota de credito"></asp:ListItem>
                                                <asp:ListItem Text="Tarjeta de crédito" Value="Tarjeta de credito"></asp:ListItem>
                                                <asp:ListItem Text="Tarjeta de débito" Value="Tarjeta de debito"></asp:ListItem>
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
                                        <div class="col-sm-2 col-md-2" id="divNotasCredito" style="display: none">
                                            <span class="asterisk">*</span> Notas de crédito
                                            <asp:DropDownList runat="server" ID="ddlNotaCredito" data-placeholder="" CssClass="form-control input-sm chosen-select" ClientIDMode="Static" onchange="changeNotaCredito();">
                                                <asp:ListItem Text="" Value=""></asp:ListItem>
                                            </asp:DropDownList>
                                        </div>

                                        <div class="col-sm-1 col-md-1" id="divNuevoCheque" style="display: none">
                                            <div class="form-group">
                                                <a href="#" class="btn btn-warning" data-toggle="modal" data-target="#modalNuevoCheque" style="margin: 25px -10px; padding: 3px 13px" onclick="abrirModalCheque();">
                                                    <i class="glyphicon glyphicon-plus"></i>
                                                </a>
                                            </div>
                                        </div>

                                        <div class="col-sm-3 col-md-2">
                                            Nro Referencia
                                            <br />
                                            <input type="text" class="form-control input-sm" maxlength="20" id="txtNroRef" />
                                        </div>
                                        <div class="col-sm-2 col-md-1">
                                            <span class="asterisk">*</span> Importe
                                            <input type="tel" class="form-control input-sm" maxlength="10" id="txtImporteForma" />
                                        </div>
                                        <div class="col-sm-3 col-md-2">
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
                                                <th style="text-align: left; width: 50%">Forma de cobro</th>
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

                                <div class="row mb15">
                                    <div class="col-sm-12">
                                        <h4>Retenciones sufridas</h4>
                                        <label class="control-label">Si sufrió alguna retención ingresela aquí.</label>
                                    </div>
                                </div>

                                <div class="well">
                                    <div class="alert alert-danger" id="divErrorRet" style="display: none">
                                        <strong>Lo siento! </strong><span id="msgErrorRet"></span>
                                    </div>

                                    <div class="row">
                                        <div class="col-sm-4 col-md-2">
                                            <span class="asterisk">*</span> Tipo
                                            <asp:DropDownList runat="server" ID="ddlTipoRet" data-placeholder="Seleccione una retencion" CssClass="form-control input-sm chosen-select">
                                                <asp:ListItem Text="" Value=""></asp:ListItem>
                                                <asp:ListItem Text="Ganancias" Value="Ganancias"></asp:ListItem>
                                                <asp:ListItem Text="IIBB" Value="IIBB"></asp:ListItem>
                                                <asp:ListItem Text="IVA" Value="IVA"></asp:ListItem>
                                                <asp:ListItem Text="SUSS" Value="SUSS"></asp:ListItem>
                                            </asp:DropDownList>

                                        </div>
                                        <div class="col-sm-3 col-md-2">
                                            Nro Referencia
                                            <br />
                                            <input type="text" class="form-control input-sm" maxlength="20" id="txtNroRefRet" />
                                        </div>
                                        <div class="col-sm-2 col-md-1">
                                            <span class="asterisk">*</span> Importe
                                            <input type="tel" class="form-control input-sm" maxlength="10" id="txtImporteRet" />
                                        </div>
                                        <div class="col-sm-3 col-md-2">
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
                            </div>

                        </asp:Panel>
                    </div>
                    <div class="panel-footer" runat="server" id="divFooter">
                        <a class="btn btn-success" onclick="grabar();" id="lnkAceptar">Aceptar</a>
                        <a href="#" onclick="cancelar();" style="margin-left: 20px">Cancelar</a>
                        <a class="btn btn-white" onclick="previsualizar();" style="float: right;" id="lnkPrevisualizar"><i class="fa fa-desktop fa-fw mr5"></i>Imprimir</a>
                    </div>

                    <asp:HiddenField runat="server" ID="hdntieneComprobante" Value="0" />
                    <asp:HiddenField runat="server" ID="hdnEnvioCR" Value="0" />
                    <asp:HiddenField runat="server" ID="hdnID" Value="0" />
                    <asp:HiddenField runat="server" ID="hdnIDPersona" Value="0" />
                    <asp:HiddenField runat="server" ID="hdnTieneFE" Value="0" ClientIDMode="Static" />
                    <input type="hidden" id="hdnRazonSocial" runat="server" />
                </div>
            </form>
        </div>

    </div>

    <!-- Modal -->
    <div class="modal modal-wide fade" id="modalPdf" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Visualización del recibo</h4>
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

    <div class="modal modal fade" id="modalOk" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true" data-backdrop="static" data-keyboard="false">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true" onclick="window.location.href='cobranzas.aspx'">&times;</button>
                    <h4 class="modal-title" id="litModalOkTitulo">Comprobante emitido correctamente. </h4>
                </div>
                <div class="modal-body" style="min-height: 200px;">
                     <div class="alert alert-danger" id="divErrorEnvioCR" style="display: none">
                        <strong>Lo sentimos! </strong><span id="msgErrorEnvioCR"></span>
                    </div>
                    <div class="alert alert-success" id="divOkMail" style="display: none">
                        <strong>Bien hecho! </strong>El mensaje ha sido enviado correctamente
                    </div>

                    <div id="divToolbar">
                        <div class="col-sm-3 CAJA_BLANCA_AZUL" onclick="Toolbar.mostrarEnvio();" id="divCajaEmail">
                            <a id="lnkSendMail">
                                <span class="glyphicon glyphicon-envelope" id="imgMailEnvio" style="font-size: 30px;"></span>
                                <br />
                                <span id="spSendMail">Enviar por Email</span>&nbsp;<i style="color:#17a08c" id="iCheckEnvio" title="El correo automatico fue enviado correctamente"></i>
                            </a>
                        </div>
                        <div class="col-sm-3 CAJA_BLANCA_AZUL">
                            <a href="#" id="lnkDownloadPdf">
                                <span class="fa fa-file-text" style="font-size: 35px;"></span>
                                <br />
                                Descargar en PDF
                            </a>
                        </div>
                        <div class="col-sm-3 CAJA_BLANCA_AZUL" onclick="Common.imprimirArchivoDesdeIframe('');">
                            <a id="lnkPrintPdf">
                                <span class="glyphicon glyphicon-print" style="font-size: 30px;"></span>
                                <br />
                                Imprimir
                            </a>
                        </div>
                    </div>
                    <div id="divSendEmail" style="display: none">
                        <div class="row">
                            <div class="col-sm-12">
                                <div class="alert alert-danger" id="divErrorMail" style="display: none">
                                    <strong>Lo sentimos! </strong><span id="msgErrorMail"></span>
                                </div>

                                <form id="frmSendMail">
                                    <div class="form-group">
                                        <p><span class="asterisk">*</span> Para: <small>(separa las direcciones mediante una coma) </small></p>
                                        <input id="txtEnvioPara" class="form-control required multiemails" type="text" runat="server" />
                                        <%--<span id="msgErrorEnvioPara" class="help-block" style="display: none">Una de las direcciones ingresadas es inválida.</span>--%>
                                    </div>
                                    <div class="form-group">
                                        <p><span class="asterisk">*</span> Asunto: </p>
                                        <input id="txtEnvioAsunto" class="form-control required" type="text" maxlength="150" runat="server" />
                                        <span id="msgErrorEnvioAsunto" class="help-block" style="display: none">Este campo es obligatorio.</span>
                                    </div>
                                    <div class="form-group">
                                        <p><span class="asterisk">*</span> Mensaje: </p>
                                        <textarea rows="5" id="txtEnvioMensaje" class="form-control required"></textarea>
                                        <span id="msgErrorEnvioMensaje" class="help-block" style="display: none">Este campo es obligatorio.</span>
                                    </div>
                                    <input type="hidden" id="hdnFile" />
                                    <br />
                                    <a type="button" class="btn btn-primary" onclick="Toolbar.enviarComprobantePorMail();" id="btnEnviar">Enviar</a>
                                    <a style="margin-left: 20px" href="#" onclick="Toolbar.cancelarEnvio();">Cancelar</a>
                                </form>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <a style="margin-left: 20px" href="#" data-dismiss="modal" onclick="window.location.href='cobranzas.aspx'">Cerrar</a>
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
    <script src="/js/views/cobranzas.js?v=<%= ConfigurationManager.AppSettings["JS.Version"] %>"></script>
    <UC:NuevoCheque runat="server" ID="ucCheque"/>

    <script>
        jQuery(document).ready(function () {
            cobranzas.configForm();            
            $('#modalNuevoCheque').on('hidden.bs.modal', function (e) {
                changeFormas();
            });
        });
    </script>
</asp:Content>


