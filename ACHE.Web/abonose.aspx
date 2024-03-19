<%@ Page Title="" Language="C#" MasterPageFile="~/Front.master" AutoEventWireup="true" CodeFile="abonose.aspx.cs" Inherits="abonose" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="pageheader">
        <h2><i class="fa fa-tasks"></i>Abonos </h2>
        <div class="breadcrumb-wrapper">
            <span class="label">Estás aquí:</span>
            <ol class="breadcrumb">
                <li><a href="/home.aspx">axanweb</a></li>
                <li><a href="/comprobantes.aspx">Facturación</a></li>
                <li><a href="/abonos.aspx">Abonos</a></li>
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
                                <h3>Datos generales</h3>
                                <label class="control-label">Ingrese los datos más generales para crear un abono.</label>
                            </div>
                        </div>
                        <div class="row mb15">
                            <div class="col-sm-4 col-md-2">
                                <div class="form-group">
                                    <label class="control-label"><span class="asterisk">*</span> Frecuencia</label>
                                    <asp:DropDownList runat="server" ID="ddlFrecuencia" CssClass="form-control" onchange="changeTipo(true);">
                                        <asp:ListItem Value="S">Semestral</asp:ListItem>
                                        <asp:ListItem Value="T">Trimestral</asp:ListItem>
                                        <asp:ListItem Value="M">Mensual</asp:ListItem>
                                        <asp:ListItem Value="Q">Quincenal</asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>

                            <div class="col-sm-4 col-md-2">
                                <div class="form-group">
                                    <label class="control-label"><span class="asterisk">*</span> Nombre</label>
                                    <asp:TextBox runat="server" ID="txtNombre" CssClass="form-control required" MaxLength="100"></asp:TextBox>

                                </div>
                            </div>

                            <div class="col-sm-4 col-md-2">
                                <div class="form-group">
                                    <label class="control-label"><span class="asterisk">*</span> Producto o Servicio</label>
                                    <asp:DropDownList runat="server" ID="ddlProducto" CssClass="form-control required">
                                        <asp:ListItem Value="1" Text="Producto"></asp:ListItem>
                                        <asp:ListItem Value="2" Text="Servicio"></asp:ListItem>
                                        <asp:ListItem Value="3" Text="Producto y servicio"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>

                        <div class="row mb15">
                            <div class="col-sm-4 col-md-2 divPlanDeCuentas">
                                <div class="form-group">
                                    <label class="control-label"><span class="asterisk">*</span> Cuenta contable</label>
                                    <asp:DropDownList runat="server" ID="ddlPlanDeCuentas" CssClass="form-control required" data-placeholder="Seleccione una cuenta...">
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>
                    <div class="row mb15">
                        <div class="col-sm-12">
                            <h3>Periodo del abono</h3>
                            <label class="control-label">Ingrese el período del abono.</label>
                        </div>
                    </div>
                    <div class="row mb15">
                        <div class="col-sm-4 col-md-2">
                            <div class="form-group">
                                <label class="control-label"><span class="asterisk">*</span> Fecha Inicio</label>
                                <div class="input-group">
                                    <span class="input-group-addon"><i class="glyphicon glyphicon-calendar"></i></span>
                                    <asp:TextBox runat="server" ID="txtFechaInicio" CssClass="form-control required validDate greaterThan validFechaActual" placeholder="dd/mm/yyyy" MaxLength="10"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                        <div class="col-sm-4 col-md-2">
                            <div class="form-group">
                                <label class="control-label">Fecha Fin</label>
                                <div class="input-group">
                                    <span class="input-group-addon"><i class="glyphicon glyphicon-calendar"></i></span>
                                    <asp:TextBox runat="server" ID="txtFechaFin" CssClass="form-control validDate greaterThan" placeholder="dd/mm/yyyy" MaxLength="10"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                        <div class="col-sm-4 col-md-2">
                            <div class="form-group">
                                <label class="control-label"><span class="asterisk">*</span> Estado</label>
                                <asp:DropDownList runat="server" ID="ddlEstado" CssClass="form-control required">
                                    <asp:ListItem Text="" Value=""></asp:ListItem>
                                    <asp:ListItem Text="Activo" Value="A" Selected="True"></asp:ListItem>
                                    <asp:ListItem Text="Inactivo" Value="I"></asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>
                    </div>
                    <div class="row mb15">
                        <div class="col-sm-12">
                            <h3>Precios</h3>
                            <label class="control-label">Ingrese los datos referidos a los importes a facturar.</label>
                        </div>
                    </div>
                    <div class="row mb15">
                        <div class="col-sm-4 col-md-2">
                            <div class="form-group">
                                <label class="control-label"><span class="asterisk">*</span> Precio unitario</label>
                                <div class="input-group">
                                    <span class="input-group-addon">$</span>
                                    <asp:TextBox runat="server" ID="txtPrecio" CssClass="form-control" MaxLength="10"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                        <div class="col-sm-4 col-md-2">
                            <div class="form-group">
                                <label class="control-label"><span class="asterisk">*</span> IVA %</label>
                                <asp:DropDownList runat="server" ID="ddlIva" CssClass="form-control">
                                    <asp:ListItem Value="0,00">0</asp:ListItem>
                                    <asp:ListItem Value="2,50">2,5</asp:ListItem>
                                    <asp:ListItem Value="5,00">5</asp:ListItem>
                                    <asp:ListItem Value="10,50">10,5</asp:ListItem>
                                    <asp:ListItem Value="21,00">21</asp:ListItem>
                                    <asp:ListItem Value="27,00">27</asp:ListItem>
                                </asp:DropDownList>
                            </div>
                        </div>
                        <div class="col-sm-4 col-md-2">
                            <h5 class="subtitle mb10">PRECIO CON IVA</h5>
                            <h4 class="text-primary" style="color: green" id="divPrecioIVA">$
                                    <asp:Literal runat="server" ID="litTotal"></asp:Literal></h4>
                        </div>
                    </div>
                    <div class="row mb15">
                        <div class="col-sm-12">
                            <h3>Clientes</h3>
                            <label class="control-label">Seleccione los clientes a los cuales le va a facturar el abono.</label>
                        </div>
                    </div>
                    <div class="row mb15">
                        <div class="col-sm-4">
                            <div class="form-group">
                                <label class="control-label">&nbsp;</label>
                                <select class="select2" data-placeholder="Seleccione 1 o más clientes/proveedores..." id="ddlPersona">
                                </select>
                                <asp:HiddenField runat="server" ID="hdnPersonasID" ClientIDMode="Static" />

                                <div class="" id="divErrorClientes" style="display: none; margin-top: 20px; color: #a94442;">
                                    <strong>Lo sentimos! </strong><span id="msgErrorClientes"></span>
                                </div>
                            </div>
                        </div>
                        <div class="col-sm-3">
                            <div class="form-group">
                                <a href="#" class="btn btn-warning" data-toggle="modal" data-target="#modalNuevoCliente" style="margin-top: 29px; padding: 7px 15px">
                                    <i class="glyphicon glyphicon-plus"></i>
                                </a>
                                <a class="btn btn-success" style="margin-top: 29px; margin-left: 10px; padding: 7px 15px" onclick="cargarPersona()">agregar al abono</a>
                            </div>
                        </div>

                    </div>

                    <div class="row mb15" style="margin-left: -30px">
                        <div class="col-sm-6">
                            <div class="panel-body">
                                <div class="table-responsive">
                                    <table class="table mb30">
                                        <thead>
                                            <tr>
                                                <th>Razón social</th>
                                                <th>Cantidad</th>
                                                <th>Total</th>
                                                <th class="columnIcons"></th>
                                            </tr>
                                        </thead>
                                        <tbody id="resultsContainer">
                                        </tbody>
                                    </table>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="row mb15">
                        <div class="col-sm-12">
                            <hr />
                            <h3>Datos complementarios </h3>
                            <label class="control-label">Estos datos no son necesarios para crear un abono, pero podrían serte de utilidad ;-)</label>
                        </div>
                    </div>
                    <div class="row mb15">
                        <div class="col-sm-6">
                            <div class="form-group">
                                <label class="control-label">Observaciones</label>
                                <asp:TextBox runat="server" TextMode="MultiLine" Rows="5" ID="txtObservaciones" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                        <div class="col-sm-6">
                        </div>
                    </div>
                    </div>
                    <div class="panel-footer">
                        <a class="btn btn-success" id="btnActualizar" onclick="grabar();">Aceptar</a>
                        <a href="#" onclick="cancelar();" style="margin-left: 20px">Cancelar</a>
                    </div>
                </div>
                <asp:HiddenField runat="server" ID="hdnID" Value="0" />
                <asp:HiddenField runat="server" ID="hdnUsaPlanCorporativo" Value="0" />
            </form>
        </div>
    </div>

    <script id="resultTemplate" type="text/x-jQuery-tmpl">
        {{each results}}
            <tr>
                <td>${RazonSocial}</td>
                <td>
                    <input runat="server" class="form-control ListaClientes" maxlength="4" clientidmode="Static" idpersonascantidad="${IDPersona}" value="${Cantidad}" onchange="recalcularTotal(${IDPersona},this.value);" />
                </td>
                <td>${Total}</td>
                <td class="table-action">
                    <a onclick="eliminarCliente(${IDPersona});" style="cursor: pointer; font-size: 16px" class="delete-row" title="Eliminar"><i class="fa fa-trash-o"></i></a>
                </td>

            </tr>
        {{/each}}
    </script>
    <script id="noResultTemplate" type="text/x-jQuery-tmpl">
        <tr>
            <td colspan="3">No se han encontrado resultados</td>
        </tr>
    </script>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FooterContent" runat="Server">
    <script type="text/javascript" src="/js/jquery.tmpl.min.js"></script>
    <script src="js/numeral.min.js"></script>
    <script src="/js/jquery.maskedinput.min.js"></script>
    <script src="/js/jquery.maskMoney.min.js"></script>
    <script src="/js/jquery.validate.min.js"></script>
    <script src="/js/select2.min.js"></script>
    <script src="/js/views/abonos.js?v=<%= ConfigurationManager.AppSettings["JS.Version"] %>"></script>
    <UC:NuevaPersona runat="server" ID="ucCliente" />
    <script>
        jQuery(document).ready(function () {
            configForm();
        });
    </script>
</asp:Content>
