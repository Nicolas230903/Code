<%@ Page Title="" Language="C#" MasterPageFile="~/Front.master" AutoEventWireup="true" CodeFile="gastosBancariose.aspx.cs" Inherits="modulos_Tesoreria_gastosBancariose" %>

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
        <h2><i class='fa fa-university'></i> Gastos bancarios</h2>
        <div class="breadcrumb-wrapper">
            <span class="label">Estás aquí:</span>
            <ol class="breadcrumb">
                <li><a href="/home.aspx">axanweb</a></li>
                <li><a href='/modulos/tesoreria/gastosBancarios.aspx'>Gastos bancarios</a></li>
                <li class="active"><asp:Literal runat="server" ID="litPath"></asp:Literal></li>
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

                            <div class="col-sm-3">
                                <div class="form-group">
                                    <label class="control-label"><span class="asterisk">*</span> Banco</label>
                                    <asp:DropDownList runat="server" ID="ddlBanco" CssClass="form-control required" ClientIDMode="Static">
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="col-sm-3">
                                <div class="form-group">
                                    <label class="control-label"><span class="asterisk">*</span> Fecha</label>
                                    <asp:TextBox runat="server" ID="txtFecha" CssClass="form-control validDate required" placeholder="dd/mm/yyyy" MaxLength="10" ClientIDMode="Static"></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-sm-3">
                                <div class="form-group">
                                    <label class="control-label"><span class="asterisk">*</span> Importe</label>
                                    <asp:TextBox runat="server" ID="txtImporte" CssClass="form-control  required" MaxLength="128" ClientIDMode="Static" onblur="gastosBancarios.changeTotal()"></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-sm-3">
                                <div class="form-group">
                                    <label class="control-label"> Concepto</label>
                                    <asp:TextBox runat="server" ID="txtConcepto" CssClass="form-control " MaxLength="128" ClientIDMode="Static"></asp:TextBox>
                                </div>
                            </div>
                        </div>

                        <div class="row mb15">
                            <div class="col-sm-3">
                                <div class="form-group">
                                    <label class="control-label"> IVA</label>
                                    <asp:TextBox runat="server" ID="txtIVA" CssClass="form-control " MaxLength="128" ClientIDMode="Static" onblur="gastosBancarios.changeTotal()"></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-sm-3">
                                <div class="form-group">
                                    <label class="control-label"> Débito</label>
                                    <asp:TextBox runat="server" ID="txtDebito" CssClass="form-control " MaxLength="128" ClientIDMode="Static" onblur="gastosBancarios.changeTotal()"></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-sm-3">
                                <div class="form-group">
                                    <label class="control-label"> Crédito</label>
                                    <asp:TextBox runat="server" ID="txtCredito" CssClass="form-control " MaxLength="128" ClientIDMode="Static" onblur="gastosBancarios.changeTotal()"></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-sm-3">
                                <div class="form-group">
                                    <label class="control-label"> Comisión al 21,00</label>
                                    <asp:TextBox runat="server" ID="txtImporte21" CssClass="form-control " MaxLength="128" ClientIDMode="Static" onblur="gastosBancarios.changeTotal()"></asp:TextBox>
                                </div>
                            </div>
                        </div>


                         <div class="row mb15">
                            <div class="col-sm-3">
                                <div class="form-group">
                                    <label class="control-label">Percepción IVA</label>
                                    <asp:TextBox runat="server" ID="txtPercepcionIVA" CssClass="form-control " MaxLength="128" ClientIDMode="Static" onblur="gastosBancarios.changeTotal()"></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-sm-3">
                                <div class="form-group">
                                    <label class="control-label"> SIRCREB</label>
                                    <asp:TextBox runat="server" ID="txtSIRCREB" CssClass="form-control " MaxLength="128" ClientIDMode="Static" onblur="gastosBancarios.changeTotal()"></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-sm-3">
                                <div class="form-group">
                                    <label class="control-label"> Comisión al 10,5</label>
                                    <asp:TextBox runat="server" ID="txtImporte10" CssClass="form-control " MaxLength="128" ClientIDMode="Static" onblur="gastosBancarios.changeTotal()"></asp:TextBox>
                                </div>
                            </div>
                        </div>


                        <div class="row mb15">
                            <div class="col-sm-3">
                                <div class="form-group">
                                    <label class="control-label"> IIBB</label>
                                    <asp:TextBox runat="server" ID="txtIIBB" CssClass="form-control " MaxLength="128" ClientIDMode="Static" onblur="gastosBancarios.changeTotal()"></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-sm-3">
                                <div class="form-group">
                                    <label class="control-label"> Crédito Computable</label>
                                    <asp:TextBox runat="server" ID="txtCreditoComputable" CssClass="form-control " MaxLength="128" ClientIDMode="Static" onblur="gastosBancarios.changeTotal()"></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-sm-3">
                                <div class="form-group">
                                    <label class="control-label"> Otros</label>
                                    <asp:TextBox runat="server" ID="txtOtros" CssClass="form-control " MaxLength="128" ClientIDMode="Static" onblur="gastosBancarios.changeTotal()"></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-sm-3">
                                <div class="form-group">
                                    <h5 class="subtitle mb10"> Total</h5>
                                    <asp:Label runat="server" ID="txtTotal" ClientIDMode="Static"></asp:Label>
                                    <h4 class="text-primary" style="color: green" id="divTotal">$
                                        <asp:Literal runat="server" ID="litTotal" ClientIDMode="Static"></asp:Literal>
                                    </h4>
                                </div>
                            </div>
                        </div>

                    </div>

                    <div class="panel-footer">
                        <a class="btn btn-success" id="btnActualizar" onclick="gastosBancarios.grabar();">Aceptar</a>
                        <a href="#" onclick="gastosBancarios.cancelar();" style="margin-left:20px">Cancelar</a>
                    </div>

                </div>
                <asp:HiddenField runat="server" ID="hdnID" Value="0" />
            </form>
        </div>
    </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FooterContent" runat="Server">
    <script src="/js/select2.min.js"></script>
    <script src="/js/jquery.validate.min.js"></script>
    <script src="/js/jquery.maskedinput.min.js"></script>
    <script src="/js/jquery.maskMoney.min.js"></script>
    <script src="/js/views/tesoreria/gastosBancarios.js?v=<%= ConfigurationManager.AppSettings["JS.Version"] %>"></script>

    <script>
        jQuery(document).ready(function () {
            gastosBancarios.configForm();
        });
    </script>

</asp:Content>
