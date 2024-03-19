<%@ Page Title="" Language="C#" MasterPageFile="~/Front.master" AutoEventWireup="true" CodeFile="listaPreciose.aspx.cs" Inherits="modulos_ventas_listaPreciose" %>

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
        <h2><i class='fa fa-list'></i>Lista de Precios</h2>
        <div class="breadcrumb-wrapper">
            <span class="label">Estás aquí:</span>
            <ol class="breadcrumb">
                <li><a href="/home.aspx">axanweb</a></li>
                <li><a href="/conceptos.aspx">Productos y servicios</a></li>
                <li><a href='/modulos/ventas/listaPrecios.aspx'>Lista de Precios</a></li>
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
                            <div class="col-sm-12">
                                <h3>Datos generales</h3>
                                <label class="control-label">Acá van los datos más generales para crear la lista de precios.</label>
                            </div>
                        </div>

                        <div class="row mb15">
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="control-label"><span class="asterisk">*</span> Nombre de la lista</label>
                                    <asp:TextBox runat="server" ID="txtNombre" CssClass="form-control  required" MaxLength="128" ClientIDMode="Static"></asp:TextBox>

                                </div>
                            </div>
                            <div class="col-sm-6">
                                <div class="form-group">
                                    <label class="control-label"><span class="asterisk">*</span> Estado</label>
                                    <asp:DropDownList runat="server" ID="ddlActivo" CssClass="form-control" ClientIDMode="Static">
                                        <asp:ListItem Text="Activa" Value="1"></asp:ListItem>
                                        <asp:ListItem Text="Inactiva" Value="0"></asp:ListItem>
                                    </asp:DropDownList>
                                </div>
                            </div>
                        </div>

                        <div class="row mb15">
                            <div class="col-sm-12">
                                <hr />
                                <h3>Datos complementarios </h3>
                                <label class="control-label">Estos datos no son necesarios para crear una lista de precios, pero podrían serte de utilidad ;-)</label>
                            </div>
                        </div>

                        <div class="row mb15">
                            <div class="col-sm-12">
                                <div class="form-group">
                                    <label class="control-label">Observaciones</label>
                                    <asp:TextBox runat="server" ID="txtObservaciones" CssClass="form-control" MaxLength="128" ClientIDMode="Static" TextMode="MultiLine" Rows="5"></asp:TextBox>
                                </div>
                            </div>
                        </div>

                        <div class="row mb15">
                            <div class="col-sm-12">
                                <hr />
                                <h3>Productos o servicios</h3>
                                <label class="control-label">Estos son los productos o servicios que diste de alta en el sistema. Los cuales podrás cambiarle el precio, para determinados clientes o proveedores</label>
                            </div>
                        </div>
                    </div>

                    <div class="panel-body">
                        <div class="table-responsive">
                            <table class="table mb30">
                                <thead>
                                    <tr>
                                        <th>Código</th>
                                        <th>Tipo</th>
                                        <th>Nombre del Producto o Servicio</th>
                                        <th><asp:Literal ID="liPrecioUnitario" runat="server"></asp:Literal></th>
                                        <th><asp:Literal ID="liPrecioLista" runat="server"></asp:Literal></th>
                                    </tr>
                                </thead>
                                <tbody id="resultsContainer">
                                </tbody>
                            </table>
                        </div>
                    </div>


                    <div class="panel-footer">
                        <a class="btn btn-success" id="btnActualizar" onclick="listaPrecios.grabar();">Aceptar</a>
                        <a href="#" onclick="listaPrecios.cancelar();" style="margin-left: 20px">Cancelar</a>
                    </div>

                </div>
                <script id="resultTemplate" type="text/x-jQuery-tmpl">
                    {{each results}}
                    <tr>
                        <td>${Codigo}</td>
                        <td>${Tipo}</td>
                        <td>${Nombre}</td>
                        <td>${Precio}</td>
                        <td>
                            <asp:TextBox runat="server" ID="txtPrecioLista" CssClass="form-control ListaDePrecio" MaxLength="100" ClientIDMode="Static" idProducto="${ID}" idConcepto="${IDConcepto}">${PrecioLista}</asp:TextBox>
                        </td>
                    </tr>
                    {{/each}}
                </script>

                <script id="noResultTemplate" type="text/x-jQuery-tmpl">
                    <tr>
                        <td colspan="6">No se han encontrado resultados
                        </td>
                    </tr>
                </script>

                <asp:HiddenField runat="server" ID="hdnID" Value="0" />
            </form>
        </div>
    </div>




</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FooterContent" runat="Server">
    <script type="text/javascript" src="/js/jquery.tmpl.min.js"></script>
    <script src="/js/select2.min.js"></script>
    <script src="/js/jquery.validate.min.js"></script>
    <script src="/js/jquery.maskedinput.min.js"></script>
    <script src="/js/jquery.maskMoney.min.js"></script>
    <script src="/js/views/ventas/listaPrecios.js?v=<%= ConfigurationManager.AppSettings["JS.Version"] %>"></script>

    <script>
        jQuery(document).ready(function () {
            listaPrecios.configForm();
        });
    </script>

</asp:Content>

