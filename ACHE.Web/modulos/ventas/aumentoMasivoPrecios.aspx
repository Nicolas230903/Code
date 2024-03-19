<%@ Page Title="" Language="C#" MasterPageFile="~/Front.master" AutoEventWireup="true" CodeFile="aumentoMasivoPrecios.aspx.cs" Inherits="modulos_ventas_aumentoMasivoPrecios" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="pageheader">
        <h2><i class="fa fa-tasks"></i>Actualización masiva de precios</h2>
        <div class="breadcrumb-wrapper">
            <span class="label">Estás aquí:</span>
            <ol class="breadcrumb">
                <li><a href="/home.aspx">axanweb</a></li>
                <li><a href="/conceptos.aspx">Productos y servicios</a></li>
                <li class="active">Actualización masiva de precios</li>
            </ol>
        </div>
    </div>
    <div class="contentpanel">
        <div class="panel panel-default">
            <div class="panel-body">
                <div class="alert alert-danger" id="divError" style="display: none">
                    <strong>Lo sentimos! </strong><span id="msgError"></span>
                </div>
                <div class="alert alert-success" id="divOk" style="display: none">
                    <strong>Bien hecho! </strong>Los datos se han actualizado correctamente
                </div>

                <form runat="server" id="frmSearch">
                    <input type="hidden" id="hdnPage" runat="server" value="1" />
                    <input type="hidden" id="hdnporcentajePrecio" runat="server" value="1" />

                    <%-- Datos principales--%>
                    <div class="row mb15">
                        <div class="col-sm-12">
                            <h3>¿Quiere actualizar los precios de todos los productos o de una lista de precio en particular? </h3>
                        </div>
                    </div>

                    <div class="row mb15">
                        <div class="col-sm-4">
                            <div class="form-group">
                                <h5 class="fm-title">
                                    <input type="radio" id="chkTodos" name="radioProductos" value="1" onclick="aumentoMasivoPrecio.ocultarLista();" />
                                    Si, actualizar todos.</h5>
                                <h5 class="fm-title">
                                    <input type="radio" id="chkLista" name="radioProductos" value="0" onclick="aumentoMasivoPrecio.ocultarLista();" />
                                    No, solo de una lista.</h5>
                            </div>
                        </div>
                    </div>

                    <div class="row mb15" id="divListaPrecios" style="display: none">
                        <div class="col-sm-2">
                            <div class="form-group">
                                <label class="control-label"><span class="asterisk">*</span> Lista de precios</label>
                                <asp:DropDownList runat="server" ID="ddlListaPrecios" CssClass="form-control required" data-placeHolder="Seleccione un una lista de precios">
                                </asp:DropDownList>
                            </div>
                        </div>
                    </div>
                    <div id="divPaso2" style="display: none">
                        <div class="row mb15">
                            <div class="col-sm-12">
                                <hr />
                                <h3>¿Quiere actualizar los precios por porcentaje o de forma individual a través de un archivo?</h3>
                            </div>
                        </div>

                        <div class="row mb15">
                            <div class="col-sm-4">
                                <div class="form-group">
                                    <h5 class="fm-title">
                                        <input type="radio" id="cheTodos" name="radioPorcentaje" value="1" onclick="aumentoMasivoPrecio.ocultarPorcentaje();" checked="checked" />
                                        Si, actualizar los precios por porcentaje.
                                    </h5>
                                    <h5 class="fm-title">
                                        <input type="radio" id="cheIndividual" name="radioPorcentaje" value="0" onclick="aumentoMasivoPrecio.ocultarPorcentaje();" />
                                        No, actualizar los precios de forma individual.
                                    </h5>
                                </div>
                            </div>
                        </div>

                        <div class="row mb15" id="divPorcentaje">
                            <div class="col-sm-2">
                                <label class="control-label"><span class="asterisk">*</span> Porcentaje %</label>
                                <input type="text" class="form-control required" id="txtPorcentaje" maxlength="6" />
                            </div>



                            <div class="row mb15">
                                <div class="col-sm-12">
                                    <hr />
                                    <h3>¿Quiere actualizar los precios de un cliente/proveedor determinado?</h3>
                                </div>
                            </div>

                            <div class="col-sm-4">
                                <label class="control-label"></label>
                                <select id="ddlPersonas" class="select2" data-placeholder="Seleccione un cliente/proveedor..." runat="server"></select>
                            </div>
                        </div>
                    </div>
                </form>
            </div>
        </div>
        <div class="panel-footer">
            <a class="btn btn-success mr10" onclick="aumentoMasivoPrecio.guardar();" style="margin-top: 4px; display: none;" id="btnActualizar">Actualizar precios </a>
            <a href="/conceptos.aspx" style="margin-left: 20px">Cancelar</a>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FooterContent" runat="Server">
    <script type="text/javascript" src="/js/jquery.tmpl.min.js"></script>
    <script src="/js/jquery.maskMoney.min.js"></script>
    <script src="/js/jquery.validate.min.js"></script>
    <script src="/js/select2.min.js"></script>
    <script src="/js/views/ventas/aumentoMasivoPrecios.js?v=<%= ConfigurationManager.AppSettings["JS.Version"] %>"></script>

    <script>
        jQuery(document).ready(function () {
            aumentoMasivoPrecio.configFilters();
        });
    </script>
</asp:Content>
