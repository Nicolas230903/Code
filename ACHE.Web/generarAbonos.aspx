<%@ Page Title="" Language="C#" MasterPageFile="~/Front.master" AutoEventWireup="true" CodeFile="generarAbonos.aspx.cs" Inherits="generarAbonos" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="pageheader">
        <h2><i class="fa fa-tasks"></i>Generar Abonos</h2>
        <div class="breadcrumb-wrapper">
            <span class="label">Estás aquí:</span>
            <ol class="breadcrumb">
                <li><a href="/home.aspx">axanweb</a></li>
                <li><a href="/comprobantes.aspx">Facturación</a></li>
                <li class="active">Generar Abonos</li>
            </ol>
        </div>
    </div>
    <div class="contentpanel">

        <div class="row">
            <form id="fromEdicion" class="col-sm-12" runat="server">
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
                                <div class="form-group">
                                    <label class="control-label"><span class="asterisk">*</span> Fecha de inicio del abono</label>
                                    <div class="input-group">
                                        <span class="input-group-addon"><i class="glyphicon glyphicon-calendar"></i></span>
                                        <input class="form-control validDate required validFechaActual" id="txtFecha" placeholder="dd/mm/yyyy" maxlength="10" />
                                    </div>
                                </div>
                            </div>
                            <div class="col-sm-1">
                                <div class="form-group">
                                    <a id="btnBuscar" class="btn btn-black" style="margin: 29px -10px; padding: 7px 15px" onclick="generarAbonos.filtrar();">Buscar</a>
                                </div>

                            </div>
                        </div>
                        <div class="row mb15" id="ContResultados" style="display: block">
                            <div class="alert alert-danger" id="div1" style="display: none">
                                <button type="button" class="close" data-dismiss="alert" aria-hidden="true">×</button>
                                <strong>Lo sentimos!</strong> <span id="Span1"></span>
                            </div>
                            <asp:Literal runat="server" ID="litTabla"></asp:Literal>

                            <div id="resultsContainer"></div>
                        </div>

                        <div class="row mb15" id="divfooter" style="display: none">
                            <div class="col-sm-6">

                                <div class="row mb15">
                                    <div class="col-sm-4">
                                        <div class="form-group">
                                            <label class="control-label"><span class="asterisk">*</span> Fecha Comprobante</label>
                                            <div class="input-group">
                                                <span class="input-group-addon"><i class="glyphicon glyphicon-calendar"></i></span>
                                                <input class="form-control validDate required validFechaComprobanteActual" id="txtFechaComprobante" placeholder="dd/mm/yyyy" maxlength="10" />
                                            </div>
                                        </div>
                                    </div>
                                </div>

                                <div class="row mb15">
                                    <div class="col-sm-4">
                                        <div class="form-group">
                                            <label class="control-label"><span class="asterisk">*</span> Modo</label>

                                            <asp:DropDownList runat="server" ID="ddlModo" CssClass="form-control required">
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                    <div class="col-sm-4" id="divPuntoVenta">
                                        <div class="form-group">
                                            <label class="control-label"><span class="asterisk">*</span> Punto de venta</label>
                                            <asp:DropDownList runat="server" ID="ddlPuntoVenta" CssClass="form-control required" onchange="generarAbonos.changePuntoDeVenta();">
                                            </asp:DropDownList>
                                        </div>
                                    </div>
                                </div>

                                <div class="row mb15">
                                    <div class="col-sm-4" id="divNroComprobanteA">
                                        <div class="form-group">
                                            <label class="control-label" id="lblNroComprobante"><span class="asterisk" id="NroComprobante">*</span> Proximo Nro. Comprobante Factura A</label>
                                            <asp:TextBox runat="server" ID="txtNumeroFacturaA" CssClass="form-control required" MaxLength="9"></asp:TextBox>
                                        </div>
                                    </div>
                                    <div class="col-sm-4" id="divNroComprobanteB">
                                        <div class="form-group">
                                            <label class="control-label" id="Label1"><span class="asterisk" id="Span2">*</span> Proximo Nro. Comprobante Factura B</label>
                                            <asp:TextBox runat="server" ID="txtNumeroFacturaB" CssClass="form-control required" MaxLength="9"></asp:TextBox>
                                        </div>
                                    </div>

                                    <div class="col-sm-4" id="divNroComprobanteC" style="display: none">
                                        <div class="form-group">
                                            <label class="control-label" id="Label2"><span class="asterisk" id="Span3">*</span> Proximo Nro. Comprobante Factura C</label>
                                            <asp:TextBox runat="server" ID="txtNumeroFacturaC" CssClass="form-control required" MaxLength="9"></asp:TextBox>
                                        </div>
                                    </div>

                                </div>
                                <div class="row mb15">
                                    <div class="col-sm-4">
                                        <div class="form-group">
                                            <a id="btnGenerar" class="btn btn-success" onclick="generarAbonos.ingresarDatos();">Generar</a>
                                            <img alt="" src="/images/loaders/loader1.gif" id="imgLoading2" style="display: none; margin: 29px -10px; padding: 7px 33px" />
                                        </div>
                                    </div>
                                </div>
                                <input type="hidden" id="CondicionIva" runat="server" value="" />

                            </div>
                            <div class="col-sm-6">

                                <div class="panel panel-primary">
                                    <div class="panel-heading">
                                        <div class="panel-btns">
                                            <a class="minimize" href="#">−</a>
                                        </div>
                                        <!-- panel-btns -->
                                        <h3 class="panel-title">Totales</h3>
                                    </div>
                                    <div class="panel-footer">
                                        <h5 class="control-label" id="idTotalImportes"></h5>
                                        <h5 class="control-label" id="idTotalIva"></h5>
                                        <h5 class="control-label" id="idTotal"></h5>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>

                </div>
                <input type="hidden" value="0" id="EsFacturaElectronica" runat="server" />
                <input type="hidden" value="0" id="hdnEnvioFE" runat="server" />
            </form>
        </div>
    </div>

    <!-- MODAL -->
    <div class="modal modal-wide fade" id="modalPdf" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
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
                    <a style="margin-left: 20px" href="#" type="button" data-dismiss="modal">Cerrar</a>
                </div>
            </div>
        </div>
    </div>
    <div class="modal modal fade" id="ModalFacturacion" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true" data-backdrop="static" data-keyboard="false">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true" onclick="window.location.href='comprobantes.aspx'">&times;</button>
                    <h4 class="modal-title" id="litModalOkTitulo">Comprobante emitido correctamente. </h4>
                </div>
                <div class="modal-body" style="min-height: 200px;">
                    <div class="alert alert-danger" id="divErrorEnvioFE" style="display: none">
                        <strong>Lo sentimos! </strong><span id="msgErrorEnvioFE"></span>
                    </div>
                    <div class="alert alert-success" id="divOkMail" style="display: none">
                        <strong>Bien hecho! </strong>El mensaje ha sido enviado correctamente
                    </div>
                    <div id="divToolbar">
                        <div class="col-sm-3 CAJA_BLANCA_AZUL" onclick="Toolbar.mostrarEnvio();">
                            <a id="lnkSendMail">
                                <span class="glyphicon glyphicon-envelope" id="imgMailEnvio" style="font-size: 30px;"></span>
                                <br />
                                <span id="spSendMail">Enviar por Email</span>&nbsp;<i style="color: #17a08c" id="iCheckEnvio" title="El correo automatico fue enviado correctamente"></i>
                            </a>
                        </div>
                        <div class="col-sm-3 CAJA_BLANCA_AZUL">
                            <a href="#" id="lnkDownloadPdf" download>
                                <span class="fa fa-file-text" style="font-size: 35px;"></span>
                                <br />
                                Descargar en PDF
                            </a>
                        </div>
                        <div class="col-sm-3 CAJA_BLANCA_AZUL hide">
                            <i class="glyphicon glyphicon-usd" style="font-size: 30px; opacity: 1; border: none; padding: 0"></i>
                            <br />
                            Cobrar
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
                                        <textarea rows="5" id="txtEnvioMensaje" class="form-control required" runat="server"></textarea>
                                        <span id="msgErrorEnvioMensaje" class="help-block" style="display: none">Este campo es obligatorio.</span>
                                    </div>
                                    <input type="hidden" id="hdnFile" />
                                    <input type="hidden" id="hdnRazonSocial" />
                                    <br />
                                    <a id="btnEnviar" type="button" class="btn btn-success" onclick="Toolbar.enviarComprobantePorMail();">Enviar</a>
                                    <a style="margin-left: 20px" href="#" type="button" onclick="Toolbar.cancelarEnvio();">Cancelar</a>
                                </form>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <a style="margin-left: 20px" href="#" data-dismiss="modal" onclick="">Cerrar</a>
                </div>
            </div>
        </div>
    </div>

    <script id="resultTemplate" type="text/x-jQuery-tmpl">
        {{each results}}
        <div class="panel panel-success">
            <div class="panel-heading">
                <div class="panel-btns">
                    <a href="#" class="minimize">−</a>
                </div>
                <!-- panel-btns -->
                <h5 class="panel-title">${Nombre} - CANT: ${Cantidad} - TOTAL: $ ${TotalCant} </h5>
            </div>
            <!-- panel-heading -->
            <div class="panel-body panel-table">
                <div class="table-responsive">
                    <table class="table mb30">
                        <thead>
                            <tr>
                                <th style="width: 50px">Selección</th>
                                <th style="width: 30%">Razón Social</th>
                                <th style="width: 10%">CUIT</th>
                                <th style="width: 10%">Condicion Iva</th>
                                <th style="width: 10%">Cantidad</th>
                                <th style="width: 10%">Importe</th>
                                <th style="width: 10%">IVA</th>
                                <th style="width: 10%">Total</th>
                                <th style="width: 12%">Estado</th>
                            </tr>
                        </thead>
                        <tbody>
                            {{each Items}}
                                <tr>
                                    <td style="width: 50px">

                                        <input type='checkbox' id='chkAbono_${nroRegistro}' checked='checked' importe="${Importe}" iva="${ivaCalculado}" cantidad="${Cantidad}" onchange="generarAbonos.ActualizarTotales()" />

                                    </td>
                                    <td style="width: 30%">${RazonSocial}</td>
                                    <td style="width: 10%">${Cuit}</td>
                                    <td style="width: 10%">${CondicionIva}</td>
                                    <td style="width: 10%">${Cantidad}</td>
                                    <td style="width: 10%">$${ImportaPantalla}</td>
                                    <td style="width: 10%">$${ivaCalculado}</td>
                                    <td style="width: 10%">$${TotalPantalla}</td>
                                    <td style="width: 12%">${FEGenerada}</td>
                                </tr>
                            {{/each}}
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
        {{/each}}
        
    </script>

    <script id="resultComprobanteFETemplate" type="text/x-jQuery-tmpl">
        {{each results}}
        <div class="panel panel-success">
            <div class="panel-heading">
                <div class="panel-btns">
                    <a href="#" class="minimize">−</a>
                </div>
                <!-- panel-btns -->
                <h5 class="panel-title">${Nombre} - CANT: ${Cantidad} - TOTAL: $ ${TotalCant} </h5>
            </div>
            <!-- panel-heading -->
            <div class="panel-body panel-table">
                <div class="table-responsive">
                    <table class="table mb30">
                        <thead>
                            <tr>
                                <th style="width: 15%">Nro de Factura</th>
                                <th style="width: 25%">Razón Social</th>
                                <th style="width: 10%">CUIT</th>
                                <th style="width: 5%">Condicion Iva</th>
                                <th style="width: 5%">Importe</th>
                                <th style="width: 5%">IVA</th>
                                <th style="width: 5%">Total</th>
                                <th style="width: 10%">Estado</th>


                                {{if  $("#EsFacturaElectronica").val() == "1"}}
                                    <th style='width: 10%'>Opciones envío</th>
                                {{/if}}


                            </tr>
                        </thead>
                        <tbody>
                            {{each Items}}
                                <tr>
                                    <td style="width: 15%">${nroComprobante}</td>
                                    <td style="width: 25%">${RazonSocial}</td>
                                    <td style="width: 10%">${Cuit}</td>
                                    <td style="width: 5%">${CondicionIva}</td>
                                    <td style="width: 5%">$${ImportaPantalla}</td>
                                    <td style="width: 5%">$${ivaCalculado}</td>
                                    <td style="width: 5%">$${TotalPantalla}</td>

                                    {{if Estado != "comprobante generado. " }}
                                         {{if Estado == "Comprobante no seleccionado." }}
                                            <td style="width: 10%">${Estado}</td>
                                    {{else}}
                                            <td style="width: 10%">
                                                <label class="label label-danger">${Estado}</label></td>
                                    {{/if}}
                                    {{else}}
                                        <td style="width: 10%">
                                            <label class="label label-success">${Estado}</label></td>
                                    {{/if}}

                                    {{if $("#EsFacturaElectronica").val() == "1" }}
                                        <td style='width: 10%'>
                                            <a href='#' onclick="generarAbonos.showModalFacturacion( '${URL}' , '${RazonSocial}' , '${ClienteEmail}' , '${EnvioFE}' );">${FEGenerada}</a>
                                        </td>
                                    {{/if}}
                                </tr>
                            {{/each}}
                        </tbody>
                    </table>
                </div>
            </div>
        </div>
        {{/each}}
        
    </script>

    <script id="noResultTemplate" type="text/x-jQuery-tmpl">
        <p>No se han encontrado resultados</p>
    </script>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FooterContent" runat="Server">
    <script type="text/javascript" src="/js/jquery.tmpl.min.js"></script>
    <script src="/js/jquery.validate.min.js"></script>
    <script src="/js/numeral.min.js"></script>
    <script src="/js/views/generarAbonos.js?v=<%= ConfigurationManager.AppSettings["JS.Version"] %>"></script>
    <script src="/js/jquery.maskedinput.min.js"></script>
    <script src="/js/jquery.maskMoney.min.js"></script>
    <script>
        jQuery(document).ready(function () {
            generarAbonos.configFilters();
        });
    </script>
</asp:Content>

