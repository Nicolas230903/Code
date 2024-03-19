<%@ Page Title="" Language="C#" MasterPageFile="~/Front.master" AutoEventWireup="true" CodeFile="asientosManuales.aspx.cs" Inherits="modulos_contabilidad_asientosManuales" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="pageheader">
        <h2><i class='fa fa-list'></i>Asientos Manuales</h2>
        <div class="breadcrumb-wrapper">
            <span class="label">Estás aquí:</span>
            <ol class="breadcrumb">
                <li><a href="/home.aspx">axanweb</a></li>
                <li><a href='/modulos/ventas/listaPrecios.aspx'>Asientos Manuales</a></li>
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
                                <label class="control-label">Acá podra dar de alta los asientos contables manualmente.</label>
                            </div>
                        </div>

                        <div class="row mb15">
                            <div class="col-sm-4">
                                <div class="form-group">
                                    <label class="control-label"><span class="asterisk">*</span> Leyenda</label>
                                    <asp:TextBox runat="server" ID="txtLeyenda" CssClass="form-control  required" MaxLength="128" ClientIDMode="Static"></asp:TextBox>

                                </div>
                            </div>
                            <div class="col-sm-2">
                                <label class="control-label"><span class="asterisk">*</span> Fecha</label>
                                <div class="input-group">
                                    <span class="input-group-addon"><i class="glyphicon glyphicon-calendar"></i></span>
                                    <asp:TextBox runat="server" ID="txtFecha" CssClass="form-control required validDate validFechaActual" placeholder="dd/mm/yyyy" MaxLength="10"></asp:TextBox>
                                </div>
                            </div>
                        </div>

                        <div class="row mb15">
                            <div class="col-sm-12">
                                <h3>Datos generales</h3>
                                <label class="control-label">Acá ingrese todas las cuentas contables.</label>
                            </div>
                        </div>

                        <div class="row mb15">
                            <div class="col-sm-2">
                                <div class="form-group">
                                    <label class="control-label"><span class="asterisk">*</span> Plan de cuentas</label>
                                    <asp:DropDownList runat="server" ID="ddlPlanDeCuentas" CssClass="select2 required" ClientIDMode="Static" data-placeholder="Seleccione una cuenta">
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="col-sm-2">
                                <div class="form-group">
                                    <label class="control-label"><span class="asterisk">*</span> Debe</label>
                                    <asp:TextBox runat="server" ID="txtDebe" CssClass="form-control  required" MaxLength="128" ClientIDMode="Static"></asp:TextBox>
                                </div>
                            </div>
                            <div class="col-sm-2">
                                <div class="form-group">
                                    <label class="control-label"><span class="asterisk">*</span> Haber</label>
                                    <asp:TextBox runat="server" ID="txtHaber" CssClass="form-control  required" MaxLength="128" ClientIDMode="Static"></asp:TextBox>
                                </div>
                            </div>
                        </div>

                    </div>
                    <div class="panel-footer">
                        <a class="btn btn-success" onclick="asientosManuales.agregarAsiento();">Agregar asiento</a>
                    </div>

                    <div class="panel-body">
                        <div class="table-responsive">
                            <table class="table mb30">
                                <thead>
                                    <tr>
                                        <%--<th>Codigo</th>--%>
                                        <th>Cuenta</th>
                                        <th>Debe</th>
                                        <th>Haber</th>
                                        <th class="columnIcons"></th>
                                    </tr>
                                </thead>
                                <tbody id="resultsContainer">
                                </tbody>
                            </table>
                        </div>
                    </div>

                    <div class="panel-footer">
                        <a class="btn btn-success" id="btnActualizar" onclick="asientosManuales.guardar();">Aceptar</a>
                        <a href="#" onclick="asientosManuales.cancelar();" style="margin-left: 20px">Cancelar</a>
                    </div>

                </div>
                <script id="resultTemplate" type="text/x-jQuery-tmpl">
                    {{each results}}
                    <tr>
                        <%--<td>${Codigo}</td>--%>
                        <td>${NombreCuenta}</td>
                        <td>${Debe}</td>
                        <td>${Haber}</td>
                        <td class="table-action">
                            <a onclick="asientosManuales.eliminar(${IDPlanDeCuenta});" style="cursor: pointer; font-size: 16px" class="delete-row" title="Eliminar"><i class="fa fa-trash-o"></i></a>
                        </td>
                    </tr>
                    {{/each}}
                </script>

                <script id="noResultTemplate" type="text/x-jQuery-tmpl">
                    <tr>
                        <td colspan="5">No se han encontrado resultados
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
    <script src="/js/views/contabilidad/asientosManuales.js?v=<%= ConfigurationManager.AppSettings["JS.Version"] %>"></script>

    <script>
        jQuery(document).ready(function () {
            asientosManuales.configForm();
            if (parseInt($("#hdnID").val()) != 0) {
                asientosManuales.obtenerAsientosManuales();
            }
        });
    </script>
</asp:Content>

