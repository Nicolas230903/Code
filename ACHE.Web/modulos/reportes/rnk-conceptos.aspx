<%@ Page Title="" Language="C#" MasterPageFile="~/Front.master" AutoEventWireup="true" CodeFile="rnk-conceptos.aspx.cs" Inherits="modulos_reportes_rnk_conceptos" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
    <div class="pageheader">
        <h2><i class="fa fa-bar-chart-o"></i>Ranking de productos y servicios</h2>
        <div class="breadcrumb-wrapper">
            <span class="label">Estás aquí:</span>
            <ol class="breadcrumb">
                <li><a href="/home.aspx">axanweb</a></li>
                <li><a href="#">Reportes</a></li>
                <li class="active">Ranking de productos y servicios</li>
            </ol>
        </div>
    </div>

    <div class="contentpanel">
        <div class="row">
            <div class="col-sm-4 col-md-3">
                <form runat="server" id="frmSearch">
                    <input type="hidden" id="hdnPage" runat="server" value="1" />

                    <h4 class="subtitle mb5">Filtros disponibles</h4>
        
                    <div class="mb20"></div>    
                    <h4 class="subtitle mb5">Tipo</h4>
                    <asp:DropDownList runat="server" ID="ddlTipo" CssClass="form-control">
                        <asp:ListItem value="">Producto y servicios</asp:ListItem>
                        <asp:ListItem value="P">Productos</asp:ListItem>
                        <asp:ListItem value="S">Servicios</asp:ListItem>
                    </asp:DropDownList>

                    <div class="mb20"></div>    
                    <h4 class="subtitle mb5">Estado</h4>
                    <asp:DropDownList runat="server" ID="ddlEstado" CssClass="form-control">
                        <asp:ListItem Text="" Value=""></asp:ListItem>
                        <asp:ListItem Text="Activo" Value="A"></asp:ListItem>
                        <asp:ListItem Text="Inactivo" Value="I"></asp:ListItem>
                    </asp:DropDownList>

                    <div class="mb20"></div>
                    <h4 class="subtitle mb5">Fecha</h4>
                    <div class="row row-pad-5">
                        <div class="col-lg-6">
                            <div class="input-group">
                                <span class="input-group-addon"><i class="glyphicon glyphicon-calendar"></i></span>
                                <asp:TextBox runat="server" ID="txtFechaDesde" CssClass="form-control validDate greaterThan" placeholder="dd/mm/yyyy" MaxLength="10"></asp:TextBox>
                            </div>
                            <label for="txtFechaDesde" class="error" style="display:none">La fecha desde es mayor a la fecha hasta</label>
                        </div>
                        <div class="col-lg-6">
                            <div class="input-group">
                                <span class="input-group-addon"><i class="glyphicon glyphicon-calendar"></i></span>
                                <asp:TextBox runat="server" ID="txtFechaHasta" CssClass="form-control validDate greaterThan" placeholder="dd/mm/yyyy" MaxLength="10"></asp:TextBox>
                            </div>
                            <label for="txtFechaHasta" class="error" style="display:none">La fecha desde es mayor a la fecha hasta</label>
                        </div>
                    </div>

                    <div class="mb20"></div>

                    <a class="btn btn-black" onclick="resetearPagina();filtrar();">Buscar</a>
                    <a class="btn btn-default" onclick="resetearPagina();verTodos();">Ver todos</a>
                
                    <br />
                </form>
            </div>
            <div class="col-sm-8 col-md-9 table-results">
                <div class="panel panel-default">
                    <div class="panel-heading">
                        <div class="pull-right">
                            <div class="btn-group mr10">
                                <div class="btn btn-white tooltips">
                                    <a  id="divIconoDescargar" href="javascript:exportar();">
                                        <i class="glyphicon glyphicon-save"></i>&nbsp;Exportar
                                    </a>
                                    <img alt="" src="/images/loaders/loader1.gif" id="imgLoading" style="display:none" />
                                    <a href="" id="lnkDownload" onclick="resetearExportacion();" download="RankingConceptos" style="display:none">Descargar</a>
                                </div>
                                <%--<button class="btn btn-white tooltips" type="button" data-toggle="tooltip" title="" data-original-title="Nuevo" onclick="nuevo();"><i class="glyphicon glyphicon-add"></i></button>--%>
                                <%--<button class="btn btn-white tooltips" type="button" data-toggle="tooltip" title="" data-original-title="Delete"><i class="glyphicon glyphicon-trash"></i></button>--%>
                            </div>
                            
                            <div class="btn-group mr10" id="divPagination" style="display:none">
                                <a class="btn btn-white" id="lnkPrevPage" style="cursor:pointer" onclick="mostrarPagAnterior();"><i class="glyphicon glyphicon-chevron-left"></i> Anterior</a>
                                <a class="btn btn-white" id="lnkNextPage" style="cursor:pointer" onclick="mostrarPagProxima();">Siguiente <i class="glyphicon glyphicon-chevron-right"></i></a>
                            </div>
                        </div>

                        <h4 class="panel-title">Resultados</h4>
                        <p id="msjResultados"></p>
                    </div><!-- panel-heading -->
                    <div class="panel-body">

                        <div class="alert alert-danger" id="divError" style="display:none">
                            <strong>Lo sentimos!</strong> <span id="msgError"></span>
                        </div>

                        <div class="table-responsive">
                            <table class="table mb30">
                                <thead>
                                    <tr>
                                        <th>Concepto</th>
                                        <th>Código</th>
                                        <th>Cantidad</th>
                                        <th style="text-align:right">Total sin IVA</th>
                                    </tr>
                                </thead>
                                <tbody id="resultsContainer">
                                
                                
                                </tbody>
                            </table>
                        </div>

                    </div>
                </div>
            </div>
        </div>
    </div>

    <script id="resultTemplate" type="text/x-jQuery-tmpl">
        {{each results}}
        <tr>
            <td>
                ${Valor1}
            </td>
            <td>
                ${Valor2}
            </td>
            <td>
                ${Cantidad}
            </td>
            <td style="text-align:right">
                $${Total}
            </td>
        </tr>
        {{/each}}
    </script>

     <script id="noResultTemplate" type="text/x-jQuery-tmpl">
        <tr>
            <td colspan="4">
               No se han encontrado resultados
            </td>
        </tr>
    </script>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FooterContent" Runat="Server">
    <script type="text/javascript" src="/js/jquery.tmpl.min.js"></script>
    <script src="/js/jquery.validate.min.js"></script>
    <script src="/js/views/reportes/rnk-conceptos.js?v=<%= ConfigurationManager.AppSettings["JS.Version"] %>"></script>

    <script>
        jQuery(document).ready(function () {
            configFilters();
        });
    </script>
</asp:Content>

