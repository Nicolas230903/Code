<%@ Page Title="" Language="C#" MasterPageFile="~/Front.master" AutoEventWireup="true" CodeFile="gastosGeneralese.aspx.cs" Inherits="gastosGeneralese" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="pageheader">
        <h2><i class="fa fa-tasks"></i>Gastos Generales</h2>
        <div class="breadcrumb-wrapper">
            <span class="label">Estás aquí:</span>
            <ol class="breadcrumb">
                <li><a href="/home.aspx">axanweb</a></li>
                <li><a href="/gastosGenerales.aspx">Gastos Generales</a></li>
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

                            <!-- Período facturado-->
                            <div class="row mb15">
                                <div class="col-sm-12">
                                    <h3>Período</h3>
                                </div>
                            </div>


                            <div class="row mb15">
                                <div class="col-sm-4 col-md-3 col-lg-2">
                                    <div class="form-group">
                                        <label class="control-label"><span class="asterisk">*</span> Periodo</label>
                                        <div class="input-group">
                                            <span class="input-group-addon"><i class="glyphicon glyphicon-calendar"></i></span>
                                            <asp:TextBox runat="server" ID="txtFechaPeriodo" CssClass="form-control required" placeholder="yyyymm" MaxLength="6"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <!-- Importes-->
                            <div class="row mb15">
                                <div class="col-sm-12">
                                    <hr />
                                    <h3>Importes</h3>
                                </div>
                            </div>
                            <div class="row mb15">
                                <div class="col-sm-3">
                                    <div class="form-group">
                                        <label class="control-label">Sueldos</label>
                                        <div class="input-group">
                                            <span class="input-group-addon">$</span>
                                            <asp:TextBox runat="server" ID="txtSueldos" CssClass="form-control" TextMode="Phone" MaxLength="10"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-sm-3">
                                    <div class="form-group">
                                        <label class="control-label">Seguridad e Higiene</label>
                                        <div class="input-group">
                                            <span class="input-group-addon">$</span>
                                            <asp:TextBox runat="server" ID="txtSeguridadEHigiene" CssClass="form-control" MaxLength="10" TextMode="Phone"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-sm-3">
                                    <div class="form-group">
                                        <label class="control-label">Municipales</label>
                                        <div class="input-group">
                                            <span class="input-group-addon">$</span>
                                            <asp:TextBox runat="server" ID="txtMunicipales" CssClass="form-control" MaxLength="10" TextMode="Phone"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-sm-3">
                                    <div class="form-group">
                                        <label class="control-label">Monotributos</label>
                                        <div class="input-group">
                                            <span class="input-group-addon">$</span>
                                            <asp:TextBox runat="server" ID="txtMonotributos" CssClass="form-control" MaxLength="10" TextMode="Phone"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="row mb15">
                                <div class="col-md-3">
                                    <div class="form-group">
                                        <label class="control-label">Aportes y Contribuciones</label>
                                        <div class="input-group">
                                            <span class="input-group-addon">$</span>
                                            <asp:TextBox runat="server" ID="txtAportesYContribuciones" CssClass="form-control" MaxLength="10" TextMode="Phone"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-md-3">
                                    <div class="form-group">
                                        <label class="control-label">Ganancias 12%</label>
                                        <div class="input-group">
                                            <span class="input-group-addon">$</span>
                                            <asp:TextBox runat="server" ID="txtGanancias12" CssClass="form-control" MaxLength="10" TextMode="Phone"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-sm-3">
                                    <div class="form-group">
                                        <label class="control-label">Crédito Bancario</label>
                                        <div class="input-group">
                                            <span class="input-group-addon">$</span>
                                            <asp:TextBox runat="server" ID="txtCreditoBancario" CssClass="form-control" MaxLength="10" TextMode="Phone"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-sm-3">
                                    <div class="form-group">
                                        <label class="control-label">Retenciones de IIBB</label>
                                        <div class="input-group">
                                            <span class="input-group-addon">$</span>
                                            <asp:TextBox runat="server" ID="txtRetencionesDeIIBB" CssClass="form-control" MaxLength="10" TextMode="Phone"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="row mb15">
                                <div class="col-sm-3">
                                    <div class="form-group">
                                        <label class="control-label">Planes AFIP</label>
                                        <div class="input-group">
                                            <span class="input-group-addon">$</span>
                                            <asp:TextBox runat="server" ID="txtPlanesAFIP" CssClass="form-control" TextMode="Phone" MaxLength="10"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-sm-3">
                                    <div class="form-group">
                                        <label class="control-label">Gastos 1</label>
                                        <div class="input-group">
                                            <span class="input-group-addon">$</span>
                                            <asp:TextBox runat="server" ID="txtGastos1" CssClass="form-control" TextMode="Phone" MaxLength="10"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-sm-3">
                                    <div class="form-group">
                                        <label class="control-label">Gastos 2</label>
                                        <div class="input-group">
                                            <span class="input-group-addon">$</span>
                                            <asp:TextBox runat="server" ID="txtGastos2" CssClass="form-control" TextMode="Phone" MaxLength="10"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-sm-3">
                                    <div class="form-group">
                                        <label class="control-label">Gastos 3</label>
                                        <div class="input-group">
                                            <span class="input-group-addon">$</span>
                                            <asp:TextBox runat="server" ID="txtGastos3" CssClass="form-control" TextMode="Phone" MaxLength="10"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="row mb15">
                                <div class="col-sm-3">
                                    <div class="form-group">
                                        <label class="control-label">F1</label>
                                        <div class="input-group">
                                            <span class="input-group-addon">$</span>
                                            <asp:TextBox runat="server" ID="txtF1" CssClass="form-control" TextMode="Phone" MaxLength="10"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-sm-3">
                                    <div class="form-group">
                                        <label class="control-label">F2</label>
                                        <div class="input-group">
                                            <span class="input-group-addon">$</span>
                                            <asp:TextBox runat="server" ID="txtF2" CssClass="form-control" TextMode="Phone" MaxLength="10"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                            </div>

                            <div class="panel-footer" runat="server" id="divFooter">
                                <a class="btn btn-success" id="btnActualizar" onclick="GastosGenerales.grabar();">Aceptar</a>
                                <a href="#" onclick="GastosGenerales.cancelar();" style="margin-left: 20px">Cancelar</a>                                
                            </div>

                        </asp:Panel>
                    </div>

                    <asp:HiddenField runat="server" ID="hdnIdusuario" Value="0" ClientIDMode="Static" />
                    <asp:HiddenField runat="server" ID="hdnID" Value="0" />
                </div>
            </form>
        </div>
    </div>


</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FooterContent" runat="Server">
    <script type="text/javascript" src="/js/jquery.tmpl.min.js"></script>
    <script src="js/numeral.min.js"></script>
    <script src="/js/select2.min.js"></script>
    <script src="/js/jquery.validate.min.js"></script>
    <script src="/js/jquery.maskedinput.min.js"></script>
    <script src="/js/jquery.maskMoney.min.js"></script>
    <script src="/js/views/gastosGenerales.js?v=<%= ConfigurationManager.AppSettings["JS.Version"] %>"></script>
    <script src="/js/jquery.fileupload.js" type="text/javascript"></script>

    <script src="/js/jasny-bootstrap.min.js"></script>
    <script src="/js/jquery.ui.widget.js" type="text/javascript"></script>
    <script src="/js/jquery.iframe-transport.js" type="text/javascript"></script>


    <script>
        jQuery(document).ready(function () {
            GastosGenerales.configForm();
        });
    </script>
</asp:Content>
