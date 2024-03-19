<%@ Page Title="" Language="C#" MasterPageFile="~/Front.master" AutoEventWireup="true" CodeFile="finRegistro.aspx.cs" Inherits="finRegistro" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="contentpanel">
        <div class="row mb15">
            <form id="frmEdicion" runat="server" class="col-sm-12">
                <div class="panel panel-default">
                    <div class="panel-body">
                        <div class="alert alert-danger" id="divError"  runat="server" style="display: none">
                            <strong>Lo sentimos! </strong><span id="msgError"  runat="server"></span>
                        </div>
                        <div class="alert alert-success" runat="server" id="divOk" style="display: none">
                            <span runat="server" id="msgOk"></span>
                        </div>
                        <div class="alert alert-warning" runat="server" id="divAdvertencia" style="display: none">
                            <span runat="server" id="msgAdvertencia"></span>
                        </div>
                        <div style="height: 30%; width: 100%; border-radius: 4px; text-align: center;">
                            <h1 id="hTitulo" class="text-success">Bienvenido a axanweb</h1>
                            <br />
                            <h4>Para comenzar a utilizar todas las funciones de axanweb debes completar los items que se muestran a continuación.
                                <br />
                                Ante cualquier duda puedes ponerte en contacto a través del chat online, mediante mail a <a href="mailto:axan.sistemas@gmail.com">axan.sistemas@gmail.com</a>
                            </h4>
                            <br />
                        </div>
                        <div>
                            <div class="row mb15">
                                <div class="col-sm-3"></div>
                                <div class="col-sm-6">
                                    <h3>Datos principales </h3>
                                    <label class="control-label">
                                        Estos son los datos básicos que el sistema necesita para poder utilizarlo. Todos los datos ingresados a continuacion podran ser modificados.</label>
                                </div>
                                <div class="col-sm-3"></div>
                            </div>
                            <asp:HiddenField runat="server" ID="hdnCUIT" Value="" />
                            <asp:HiddenField runat="server" ID="hdnFechaInicioActividades" Value="" />
                            <div class="row mb15">
                                <div class="col-sm-3"></div>
                                <div class="col-sm-2">
                                    <div class="form-group">
                                        <label class="control-label"><span class="asterisk">*</span> Razón Social</label>
                                        <asp:TextBox runat="server" ID="txtRazonSocial" CssClass="form-control required validCaracteres" MaxLength="128" placeholder="Pepas SRL o José Suárez"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-sm-2">
                                    <div class="form-group">
                                        <label class="control-label"><code>*</code> Categoría Impositiva</label>
                                        <asp:DropDownList runat="server" ID="ddlCondicionIva" CssClass="select2 required" data-placeholder="Selecione su condicion frente al IVA" onchange="finRegistro.changeCondicionIVA();">
                                            <asp:ListItem Text="" Value=""></asp:ListItem>
                                            <asp:ListItem Text="Exento" Value="EX"></asp:ListItem>
                                            <asp:ListItem Text="Responsable Inscripto" Value="RI"></asp:ListItem>
                                            <asp:ListItem Text="Monotributista" Value="MO"></asp:ListItem>
                                        </asp:DropDownList>
                                    </div>
                                </div>
                                <div class="col-sm-2">
                                    <div class="form-group">
                                        <label class="control-label"><span class="asterisk">*</span> Personería</label>
                                        <select id="ddlPersoneria" class="form-control required" tabindex="4" runat="server">
                                            <option value="F">Física</option>
                                            <option value="J">Jurídica</option>
                                        </select>
                                    </div>
                                </div>
                                <div class="col-sm-3"></div>
                            </div>
                            <div class="row mb15">
                                <div class="col-sm-3"></div>
                                <div class="col-sm-6">
                                    <h3>Domicio fiscal </h3>
                                    <label class="control-label">Estos son los datos aparecerán en la factura emitidas por tu empresa. Todos los datos ingresados a continuacion podran ser modificados.</label>
                                </div>
                                <div class="col-sm-3"></div>
                            </div>
                            <div class="row mb15">
                                <div class="col-sm-3"></div>
                                <div class="col-sm-2">
                                    <div class="form-group">
                                        <label class="control-label"><span class="asterisk">*</span> Provincia</label>
                                        <asp:DropDownList runat="server" ID="ddlProvincia" CssClass="select2 required"
                                            data-placeholder="Selecciona una provincia..." onchange="finRegistro.changeProvincia();">
                                        </asp:DropDownList>
                                    </div>
                                    <asp:HiddenField runat="server" ID="hdnProvincia" Value="1" />
                                </div>
                                <div class="col-sm-2">
                                    <div class="form-group">
                                        <label class="control-label"><span class="asterisk">*</span> Ciudad/Localidad</label>
                                        <asp:DropDownList runat="server" ID="ddlCiudad" CssClass="select2 required"
                                            data-placeholder="Selecciona una ciudad...">
                                        </asp:DropDownList>
                                    </div>
                                    <asp:HiddenField runat="server" ID="hdnCiudad" Value="0" />
                                </div>
                                <div class="col-sm-2">
                                    <div class="form-group">
                                        <label class="control-label"><span class="asterisk">*</span> Domicilio</label>
                                        <asp:TextBox runat="server" ID="txtDomicilio" CssClass="form-control required" MaxLength="100"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-sm-3"></div>
                            </div>
                            <div class="row mb15">
                                <div class="col-sm-3"></div>
                                <div class="col-sm-2">
                                    <div class="form-group">
                                        <label class="control-label">Piso/Depto</label>
                                        <asp:TextBox runat="server" ID="txtPisoDepto" CssClass="form-control" MaxLength="10"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-sm-2">
                                    <div class="form-group">
                                        <label class="control-label">Código Postal</label>
                                        <asp:TextBox runat="server" ID="txtCp" CssClass="form-control" MaxLength="10"></asp:TextBox>
                                    </div>
                                </div>
                                <div class="col-sm-2">
                                    <div class="form-group">
                                        <label class="control-label"><span class="asterisk">*</span> Teléfono</label>
                                        <div class="input-group">
                                            <span class="input-group-addon"><i class="glyphicon glyphicon-earphone"></i></span>
                                            <asp:TextBox runat="server" ID="txtTelefono" TextMode="Phone" CssClass="form-control required" MaxLength="50" placeholder="+54911########"></asp:TextBox>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-sm-3"></div>
                            </div>
                           <%-- <div class="row mb15">
                                <div class="col-sm-3"></div>
                                <div class="col-sm-3">
                                    <div class="form-group">
                                        <label class="control-label"><span class="asterisk">*</span> Telefono</label>
                                        <div class="input-group" style="display: flex">
                                            <span class="input-group-addon" style="width: 39px"><i class="glyphicon glyphicon-earphone"></i></span>
                                            <div class="col-sm-4" style="margin-left: -10px;">
                                                <asp:TextBox runat="server" ID="txtTelefonoCodigoArea" CssClass="form-control required" MaxLength="4" placeholder="223"></asp:TextBox>
                                            </div>
                                            <div style="margin-top: 10px;">-</div>
                                            <div class="col-sm-4">
                                                <asp:TextBox runat="server" ID="txtTelefonoPrimeraParte" CssClass="form-control required" MaxLength="4" placeholder="123"></asp:TextBox>
                                            </div>
                                            <div style="margin-top: 10px;">-</div>
                                            <div class="col-sm-4">
                                                <asp:TextBox runat="server" ID="txtTelefonoSegundaParte" CssClass="form-control required" MaxLength="4" placeholder="4567"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class="col-sm-3">
                                    <div class="form-group">
                                        <label class="control-label">Celular</label>
                                        <div class="input-group" style="display: flex">
                                            <span class="input-group-addon" style="width: 39px"><i class="glyphicon glyphicon-earphone"></i></span>
                                            <div class="col-sm-4">
                                                <asp:TextBox runat="server" ID="txtCelularCodigoArea" CssClass="form-control" MaxLength="4" placeholder="11"></asp:TextBox>
                                            </div>
                                            <div style="margin-top: 10px;">15-</div>
                                            <div class="col-sm-4">
                                                <asp:TextBox runat="server" ID="txtCelularPrimeraparte" CssClass="form-control" MaxLength="4" placeholder="1234"></asp:TextBox>
                                            </div>
                                            <div style="margin-top: 10px;">-</div>
                                            <div class="col-sm-4">
                                                <asp:TextBox runat="server" ID="txtCelularSegundaparte" CssClass="form-control" MaxLength="4" placeholder="5678"></asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>--%>
                            <div class="row mb15">
                                <div class="col-sm-3"></div>
                                <div class="col-sm-3">
                                    <div class="form-group">
                                        <h3>¿Es un contador?</h3>
                                        <input type="radio" name="rEsContador" runat="server" id="esContadorSI" value="1" disabled/>
                                        Si, soy un contador.<br />
                                        <input type="radio" name="rEsContador" runat="server" id="esContadorNO" value="0" checked="true" disabled/>
                                        No, no soy un contador.
                                    </div>
                                </div>
                                <div class="col-sm-3">
                                    <div class="form-group">
                                        <h3>¿Quiere usar contabilidad?</h3>
                                        <input type="radio" name="rUsaContabilidad" runat="server" id="rUsaContabilidadSI" value="1" onchange="finRegistro.changeCondicionIVA();" disabled/>
                                        Si, usar un plan contable.<br />
                                        <input type="radio" name="rUsaContabilidad" runat="server" id="rUsaContabilidadNO" value="0" checked="true" onchange="finRegistro.changeCondicionIVA();" disabled/>
                                        No, no usar un plan contable.
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="panel-footer text-center">
                        <a class="btn btn-success" id="btnActualizar" onclick="finRegistro.grabar();">Finalizar</a>
                        <br />
                        <br />
                    </div>
                </div>
            </form>
        </div>
    </div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FooterContent" runat="Server">
    <script src="/js/select2.min.js"></script>
    <script src="/js/jquery.validate.min.js"></script>
    <script src="/js/jquery.maskedinput.min.js"></script>
    <script src="/js/jquery.maskMoney.min.js"></script>
    <script src="/js/views/finRegistro.js?v=<%= ConfigurationManager.AppSettings["JS.Version"] %>"></script>
    <script>
        $(document).ready(function () {
            finRegistro.configForm();
        });
    </script>
</asp:Content>
