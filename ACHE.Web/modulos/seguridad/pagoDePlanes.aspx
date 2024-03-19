<%@ Page Title="" Language="C#" MasterPageFile="~/Front.master" AutoEventWireup="true" CodeFile="pagoDePlanes.aspx.cs" Inherits="modulos_ventas_pagoDePlanes" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
    <link href="/css/jasny-bootstrap.min.css" rel="stylesheet" />

    <style>
        .activo {
            background-color: #e4e7ea;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="pageheader">
        <h2><i class='fa fa-money'></i>Pago del plan</h2>
        <div class="breadcrumb-wrapper">
            <span class="label">Estás aquí:</span>
            <ol class="breadcrumb">
                <li><a href="/home.aspx">axanweb</a></li>
                <li><a href="/modulos/seguridad/mis-datos.aspx">Mi cuenta</a></li>
                <li class="active">Cambio de plan</li>
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
                        <%-- Datos principales--%>
                        <div class="row mb15">
                            <div class="col-sm-12">
                                <h3>Confirmar y activar el <b><span style="color: #bf315f" id="planNombre"></span></b>&nbsp;&nbsp;<a href="javascript:history.back();" style="font-size: 14px; text-decoration: underline">modificar plan</a></h3>
                                <h4 class="text-success">El importe a pagar es <b>$<span id="planImporte"></span></b></h4>
                            </div>
                        </div>

                        <%--<div class="row mb15">
                            <div class="col-sm-12 col-md-6">
                               
                                <p>Concepto: <b><span id="planNombre"></span></b> &nbsp &nbsp &nbsp importe: </p>
                            </div>
                        </div>--%>

                        <div class="row mb15">
                            <div class="col-sm-12">
                                <hr />
                                <h3>1 - Seleccione el método de pago</h3>
                                <%--<label class="control-label">Ingrese la forma de pago.</label>--%>
                            </div>
                        </div>

                        <div class="row mb15">

                            <div class="col-sm-12 col-md-8">

                                <div class="panel-footer mb15" id="divPanelEfectivo">
                                    <div class="col-sm-8 col-md-8">
                                        <h4 class="fm-title">
                                            <input type="radio" name="chkForma" runat="server" id="rEfectivo" value="Mercado Pago" onclick="PlanesPagos.changeFormaDePago();" />
                                            Pago Fácil, Rapipago, Red link y Provincia pagos
                                        </h4>
                                        <span style="margin-left: 16px">Tiempo de acreditación: 3 días hábiles</span>
                                    </div>
                                    <div class="col-sm-4 col-md-4">
                                        <img src="../../images/tarjetas/opcion1.png" style="float: right;" />
                                    </div>
                                </div>

                                <div class="panel-footer mb15" id="divPanelTarjeta">
                                    <div class="col-sm-8 col-md-8">
                                        <h4 class="fm-title">
                                            <input type="radio" name="chkForma" runat="server" id="rTarjetas" value="Mercado Pago" onclick="PlanesPagos.changeFormaDePago();" />
                                            Tarjetas de crédito
                                        </h4>
                                        <span style="margin-left: 16px">Acreditación inmediata, 100% online, comienza ya!!</span>
                                    </div>
                                    <div class="col-sm-4 col-md-4">
                                        <img src="../../images/tarjetas/opcion2.png" style="float: right;" />
                                    </div>
                                </div>

                                <div class="panel-footer mb15" id="divPanelMercadoPago">
                                    <div class="col-sm-8 col-md-8">
                                        <h4 class="fm-title">
                                            <input type="radio" name="chkForma" runat="server" id="rMP" value="Mercado Pago" onclick="PlanesPagos.changeFormaDePago();" />
                                            Mercado Pago
                                        </h4>
                                        <span style="margin-left: 16px">Acreditación inmediata. Puedes usar crédito de tu cuenta para pagar</span>
                                    </div>
                                    <div class="col-sm-4 col-md-4">
                                        <img src="../../images/tarjetas/MercadoPago.png" style="float: right;" />
                                    </div>
                                </div>

                                <div class="panel-footer mb15" id="divPanelTransferencia">
                                    <div class="col-sm-8 col-md-8">
                                        <h4 class="fm-title">
                                            <input type="radio" name="chkForma" runat="server" id="rTransferencia" value="Transferencia" onclick="PlanesPagos.changeFormaDePago();" />
                                            Transferencia
                                        </h4>
                                        <span style="margin-left: 16px">Tiempo de acreditación: 24/48 horas hábiles.</span>
                                    </div>
                                    <div class="mb15"></div>
                                    <div id="divDatosTransferencia" style="display: none; margin-left: 50px">
                                        <h4>Ingresa la siguiente información al momento de realizar la transferencia.</h4>
                                        <p>
                                            Razón Social: <b>axanweb SA</b><br />
                                            CUIT: <b>30-71486426-9</b><br />
                                            Cuenta corriente en pesos : <b>097-011107/0 (Santander Río)</b><br />
                                            CBU: <b>07200977-20000001110704</b><br />
                                            Concepto: <b><span id="spTransfNombre"></span></b>
                                            <br />
                                            Importe: <b>AR$ <span id="spTransfImporte"></span></b>
                                            <br />
                                        </p>
                                    </div>
                                </div>

                            </div>

                        </div>

                        <%-- MercadoPago--%>
                        <div id="divMercadoPago" style="display: none">
                            <div class="row mb15">
                                <div class="col-sm-12 col-md-6">
                                    <hr />
                                    <h3>2 - Realiza el pago</h3>
                                    <label class="control-label">Haga click en pagar y siga los pasos que le brinda MercadoPago</label>
                                </div>
                            </div>
                            <div class="row mb15">
                                <div class="col-sm-12 col-md-6">
                                    <div class="form-group">
                                        <%--<asp:Literal ID="btnMercadoPago" runat="server"></asp:Literal>--%>
                                        <asp:Literal ID="btnMercadoPago" runat="server"></asp:Literal>
                                    </div>
                                </div>
                            </div>
                        </div>

                        <%-- Transferencia --%>
                        <div id="divTransferencia" style="display: none">
                            <div class="row mb15">
                                <div class="col-sm-12 col-md-6">
                                    <hr />
                                    <h3>2 - Informa los datos de la transferencia </h3>
                                    <label class="control-label">Adjunta el comprobante de transferencia emitido por la entidad bancaria.</label>
                                </div>
                            </div>
                            <div class="row mb15">
                                <div class="col-sm-6 col-md-3">
                                    <div class="form-group">
                                        <label class="control-label"><span class="asterisk">*</span> Importe Pagado</label>
                                        <div class="input-group">
                                            <span class="input-group-addon">$</span>
                                            <asp:TextBox runat="server" ID="txtImporte" CssClass="form-control required" MaxLength="8" TabIndex="10"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-sm-6 col-md-3">
                                    <div class="form-group">
                                        <label class="control-label"><span class="asterisk">*</span> Número de referencia</label>
                                        <asp:TextBox runat="server" ID="txtNroReferencia" CssClass="form-control  required" MaxLength="128" placeholder="" TabIndex="11"></asp:TextBox>
                                    </div>
                                </div>
                            </div>
                            <div class="row mb15">
                                <div class="col-sm-6 col-md-3">
                                    <div class="form-group">
                                        <label class="control-label"><span class="asterisk">*</span> Fecha de pago</label>
                                        <div class="input-group">
                                            <span class="input-group-addon"><i class="glyphicon glyphicon-calendar"></i></span>
                                            <asp:TextBox runat="server" ID="txtFechaDePago" CssClass="form-control required validDate" placeholder="dd/mm/yyyy" MaxLength="10" TabIndex="12"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>

                                <div class="col-sm-6 col-md-3">
                                    <div class="form-group">
                                        <label class="control-label"><span class="asterisk">*</span> Adjunta el comprobante de pago</label>
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
                                    </div>
                                    <asp:HiddenField runat="server" ID="hdnFileName" Value="" />
                                </div>
                            </div>
                        </div>

                    </div>
                    <div class="panel-footer">
                        <a class="btn btn-success" id="btnActualizar" onclick="PlanesPagos.grabarsinImagen();" style="display: none">Informar Pago</a>
                        <a href="#" onclick="PlanesPagos.cancelar();" style="margin-left: 20px">Cancelar</a>
                    </div>
                </div>

                <asp:HiddenField runat="server" ID="hdnIdPlan" Value="0" />
                <asp:HiddenField runat="server" ID="hdnImporteTotal" Value="0" />
                <asp:HiddenField runat="server" ID="hdnNombrePlan" Value="0" />
                <asp:HiddenField runat="server" ID="hdnModo" Value="0" />
                <asp:HiddenField runat="server" ID="hdnTieneFoto" Value="0" />
                <asp:HiddenField runat="server" ID="hdnSinCombioDeFoto" Value="0" />
                <asp:HiddenField runat="server" ID="hdnID" Value="0" />
                <asp:HiddenField runat="server" ID="hdnGuardarFoto" Value="0" />
            </form>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FooterContent" runat="Server">
    <script src="/js/select2.min.js"></script>
    <%--<script src="/js/views/common.js"></script>--%>
    <script src="/js/views/seguridad/PlanesPagos.js?v=<%= ConfigurationManager.AppSettings["JS.Version"] %>"></script>

    <script src="/js/jquery.validate.min.js"></script>
    <script src="/js/jquery.maskedinput.min.js"></script>
    <script src="/js/jquery.maskMoney.min.js"></script>

    <script src="/js/jquery.fileupload.js" type="text/javascript"></script>
    <script src="/js/jasny-bootstrap.min.js"></script>
    <script src="/js/jquery.ui.widget.js" type="text/javascript"></script>
    <script src="/js/jquery.iframe-transport.js" type="text/javascript"></script>

    <script>
        $(document).ready(function () {
            PlanesPagos.configForm();
        });
    </script>
</asp:Content>


