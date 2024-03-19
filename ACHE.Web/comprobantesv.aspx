<%@ Page Title="" Language="C#" MasterPageFile="~/Front.master" AutoEventWireup="true" CodeFile="comprobantesv.aspx.cs" Inherits="comprobantesv" %>

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
        <h2><i class="fa fa-dollar"></i>Comprobante emitido </h2>
        <div class="breadcrumb-wrapper">
            <span class="label">Estás aquí:</span>
            <ol class="breadcrumb">
                <li><a href="/home.aspx">axanweb</a></li>
                <li><a href="/comprobantes.aspx">Comprobantes</a></li>
                <li class="active">Detalle del comprobante</li>
            </ol>
        </div>
    </div>

    <div class="contentpanel">
        <div class="panel panel-default">
            <div class="panel-body">
                <form id="frmEdicion" runat="server" class="col-sm-12">
                    <div class="row">
                        <h4 class="mb20 text-primary" style="text-transform: uppercase; margin-left: 10px;">Comprobante
                            <asp:Literal runat="server" ID="litComprobante"></asp:Literal></h4>
                    </div>

                    <div class="row">
                        <div class="col-sm-6">
                            <h5 class="subtitle mb10">De</h5>
                            <%--<asp:Image runat="server" ID="imgLogo" ImageUrl="~/files/usuarios/no-photo.png" style="max-width: 100px !important" CssClass="img-responsive mb10" />--%>

                            <address>
                                <strong>
                                    <asp:Literal runat="server" ID="litRazonSocial"></asp:Literal></strong><br />
                                <asp:Literal runat="server" ID="litDomicilio"></asp:Literal><br />
                                <asp:Literal runat="server" ID="litPaisCiudad"></asp:Literal><br />
                                <abbr title="Email">Email:</abbr>
                                <asp:Literal runat="server" ID="litEmail"></asp:Literal><br />
                                <abbr title="Phone">Tel:</abbr>
                                <asp:Literal runat="server" ID="litTelefono"></asp:Literal>
                            </address>

                        </div>
                        <!-- col-sm-6 -->

                        <div class="col-sm-6 text-right">
                            <h5 class="subtitle mb10">Para</h5>
                            <address>
                                <strong>
                                    <asp:Literal runat="server" ID="litPersonaRazonSocial"></asp:Literal></strong><br />
                                <asp:Literal runat="server" ID="litPersonaDomicilio"></asp:Literal><br />
                                <asp:Literal runat="server" ID="litPersonaPaisCiudad"></asp:Literal><br />
                                <asp:Literal runat="server" ID="litPersonaCondicionIva"></asp:Literal><br />
                                <%--<abbr title="Email">Email:</abbr>
                                <asp:Literal runat="server" ID="litPersonaEmail"></asp:Literal><br />
                                <abbr title="Phone">Tel:</abbr>
                                <asp:Literal runat="server" ID="litPersonaTelefono"></asp:Literal>--%>
                                <strong>Fecha:</strong>
                                <asp:Literal runat="server" ID="litFecha"></asp:Literal>
                                <%--<br />
                                <strong>Vencimiento:</strong>
                                <asp:Literal runat="server" ID="litFechaVencimiento"></asp:Literal>--%>
                            </address>

                        </div>
                    </div>
                    <!-- row -->

                    <div class="table-responsive">
                        <table class="table table-invoice">
                            <thead>
                                <tr>
                                    <th>Codigo</th>
                                    <th style="text-align: left">Concepto</th>
                                    <th>Cantidad</th>
                                    <th style="text-align: right">Precio unitario sin IVA</th>
                                    <th style="text-align: right">Bonificación %</th>
                                    <th style="text-align: right">IVA %</th>
                                    <th style="text-align: right">Subtotal</th>

                                </tr>
                            </thead>
                            <tbody>
                                <asp:Repeater runat="server" ID="rptDetalle">
                                    <ItemTemplate>
                                        <tr>
                                            <td><%# (Eval("Codigo")) %></td>
                                            <td style="text-align: left"><%# Eval("Concepto") %></td>
                                            <td><%# Eval("Cantidad") %></td>
                                            <td style="text-align: right"><%# Decimal.Parse(Eval("PrecioUnitario").ToString()).ToString("N2") %></td>
                                            <td style="text-align: right"><%# Eval("Bonificacion") %></td>
                                            <td style="text-align: right"><%# Eval("IVA") %></td>
                                            <td style="text-align: right"><%# Decimal.Parse(Eval("TotalConIva").ToString()).ToString("N2") %></td>
                                        </tr>
                                    </ItemTemplate>
                                </asp:Repeater>
                            </tbody>
                        </table>
                    </div>
                    <!-- table-responsive -->

                    <table class="table table-total">
                        <tbody>
                            <tr>
                                <td><strong>Subtotal :</strong></td>
                                <td>$
                                    <asp:Literal runat="server" ID="litSubtotal"></asp:Literal></td>
                            </tr>
                            <tr>
                                <td><strong>IVA :</strong></td>
                                <td>$
                                    <asp:Literal runat="server" ID="litIva"></asp:Literal></td>
                            </tr>
                            <tr>
                                <td><strong>TOTAL :</strong></td>
                                <td style="color: green">$
                                    <asp:Literal runat="server" ID="litTotal"></asp:Literal></td>
                            </tr>
                        </tbody>
                    </table>

                    <div class="mb40"></div>

                    <div class="text-right btn-invoice">
                        <asp:HyperLink runat="server" ID="lnkDescargar2" CssClass="btn btn-primary"><i class="fa fa-cloud-download fa-fw"></i> Descargar Factura</asp:HyperLink>
                        <a id="lnkDescargarRemito" class="btn btn-primary" href="#" onclick="generarRemito();"><i class="fa fa-cloud-download fa-fw"></i> Descargar Remito</a>
                        <asp:HyperLink runat="server" ID="lnkPrint" CssClass="btn btn-default"><span class="glyphicon glyphicon-print"></span> Imprimir</asp:HyperLink>
                        <asp:HyperLink runat="server" ID="lnkPreview" CssClass="btn btn-white"><i class="fa fa-desktop fa-fw mr5"></i> Ver en PDF</asp:HyperLink>
                        <a onclick="enviarEmail();" class="btn btn-white"><i class="glyphicon glyphicon-envelope"></i>&nbsp&nbsp Enviar por email</a>
                        <a onclick="editarActividad();" class="btn btn-white"><i class="glyphicon glyphicon-lock"></i>&nbsp&nbsp Editar Actividad</a>


                    </div>

                    <div class="mb40"></div>
                    <div class="well">
                        <asp:Literal runat="server" ID="litObservaciones"></asp:Literal>
                    </div>
                    <asp:HiddenField runat="server" ID="hdnTipoComprobante" Value="0" />
                    <asp:HiddenField ID="hdnExisteRemito" runat="server" Value="0"></asp:HiddenField>
                    <asp:HiddenField runat="server" ID="hdnIDUsuario" Value="0" />
                    <asp:HiddenField runat="server" ID="hdnRazonSocial" Value="" />
                    <asp:HiddenField runat="server" ID="hdnID" Value="0" />
                    <asp:HiddenField runat="server" ID="hdnFileName" Value="0" />
                    <asp:HiddenField runat="server" ID="hdnIDActividad" Value="0" />
                </form>
            </div>
        </div>
    </div>

    <!-- Modal -->
    <div class="modal modal-wide fade" id="modalEditarActividad" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title">Editar Actividad</h4>
                </div>
                <div class="modal-body">
                    <div>
                        <div class="alert alert-danger" id="divErrorEditarActividad" style="display: none">
                            <strong>Lo sentimos! </strong><span id="msgErrorEditarActividad"></span>
                        </div>
                    </div>
                    <div class="row">                        
                        <div class="col-sm-3 col-md-3" style="text-align: right;">
                            <label class="control-label">                                     
                                    Actividad
                            </label>
                        </div>
                        <div class="col-sm-6 col-md-6">
                            <select id="ddlEditarActividad" class="form-control required">
                            </select>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <a type="button" class="btn btn-success" onclick="modificarActividad();" id="btnModificarActividad">Aceptar</a>
                    <a style="margin-left: 20px" href="#" type="button" data-dismiss="modal">Cerrar</a>
                </div>
            </div>
        </div>
    </div>



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
                        <iframe id="ifrPdf" runat="server" src="" width="900px" height="500px" frameborder="0"></iframe>
                    </div>
                </div>
                <div class="modal-footer">
                    <asp:HyperLink runat="server" ID="lnkDescargar" CssClass="btn btn-primary" download><i class="fa fa-cloud-download fa-fw"></i> Descargar</asp:HyperLink>
                    <asp:HyperLink runat="server" ID="lnkPrint2" CssClass="btn btn-default"><span class="glyphicon glyphicon-print"></span> Imprimir</asp:HyperLink>
                    <a style="margin-left: 20px" href="#" type="button" data-dismiss="modal">Cerrar</a>
                </div>
            </div>
        </div>
    </div>



    <div class="modal modal fade" id="modalEMail" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true" data-backdrop="static" data-keyboard="false">
        <div class="modal-dialog">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-hidden="true">&times;</button>
                    <h4 class="modal-title" id="litModalOkTitulo">Enviar email </h4>
                </div>
                <div class="modal-body" style="min-height: 200px;">
                    <div class="alert alert-success" id="divOkMail" style="display: none">
                        <strong>Bien hecho! </strong>El mensaje ha sido enviado correctamente
                    </div>
                    <div id="divSendEmail">
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
                                    <br />
                                    <a type="button" class="btn btn-success" onclick="Toolbar.enviarComprobantePorMail();" id="btnEnviar">Enviar</a>
                                    <a style="margin-left: 20px" href="#" onclick="Toolbar.cancelarEnvio();">Cancelar</a>
                                </form>
                            </div>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <a style="margin-left: 20px" href="#" data-dismiss="modal">Cerrar</a>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FooterContent" runat="Server">
    <script src="/js/jquery.validate.min.js"></script>
    <script src="/js/views/comprobantes.js?v=<%= ConfigurationManager.AppSettings["JS.Version"] %>"></script>
    <script>
        jQuery(document).ready(function () {
            Common.obtenerActividades("ddlEditarActividad", $("#hdnIDActividad").val(),false);
            $("#hdnFile").val($("#hdnFileName").val());
            
            if ($("#hdnTipoComprobante").val() == "NCA" || $("#hdnTipoComprobante").val() == "NCB" || $("#hdnTipoComprobante").val() == "NCC"
                    || $("#hdnTipoComprobante").val() == "NDA" || $("#hdnTipoComprobante").val() == "NDB" || $("#hdnTipoComprobante").val() == "NDC") {
                $("#lnkDescargarRemito").hide();
            }
            else {
                $("#lnkDescargarRemito").show();
            }
        });
    </script>
</asp:Content>