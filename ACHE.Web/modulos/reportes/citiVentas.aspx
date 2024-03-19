<%@ Page Title="" Language="C#" MasterPageFile="~/Front.master" AutoEventWireup="true" CodeFile="citiVentas.aspx.cs" Inherits="modulos_reportes_citiVentas" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">

    <style>
        .ui-datepicker-calendar {
            display: none;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="pageheader">
        <h2>
            <i class='fa fa-archive'></i>Sistema informativo de compras y ventas <span>Administración</span>
        </h2>
        <div class="breadcrumb-wrapper">
            <span class="label">Estás aquí:</span>
            <ol class="breadcrumb">
                <li><a href="/home.aspx">axanweb</a></li>
                <li class="active">Sistema informativo de compras y ventas</li>
            </ol>
        </div>
    </div>

    <div class="contentpanel">
        <div class="row">
            <div class="col-sm-4 col-md-3">
                <form runat="server" id="frmSearch">
                    <%--<div class="mb20"></div>
                    <h4 class="subtitle mb5">Filtros disponibles</h4>

                    <div class="mb20"></div>--%>

                    <h4 class="subtitle mb5">Periodo</h4>
                    <div class="row row-pad-5">
                        <div class="col-lg-6">
                            <div class="input-group">
                                <span class="input-group-addon"><i class="glyphicon glyphicon-calendar"></i></span>
                                <asp:TextBox runat="server" ID="txtFechaDesde" CssClass="form-control validDate " placeholder="seleccione el período" MaxLength="10"></asp:TextBox>
                            </div>
                            <%--<label for="txtFechaDesde" class="error" style="display: none">La fecha desde es mayor a la fecha hasta</label>--%>
                        </div>
                    </div>

                    <a class="btn btn-black" onclick="ricv.filtrar();" id="btnGenerar">Generar archivos</a>
                    <br />
                </form>
            </div>
            <div class="col-sm-8 col-md-9">
                <div class="panel panel-default">
                    <div class="panel-heading">
                        <div class="pull-right">
                            <div class="btn-group mr10">
                            </div>
                        </div>

                        <h4 class="panel-title">Resultados</h4>
                        <p id="msjResultados"></p>
                    </div>
                    <!-- panel-heading -->
                    <div class="panel-body">

                        <div class="alert alert-danger" id="divError" style="display: none">
                            <button type="button" class="close" data-dismiss="alert" aria-hidden="true">×</button>
                            <strong>Lo sentimos!</strong> <span id="msgError"></span>
                        </div>

                        <div class="table-responsive">
                            <div id="resultsContainer"></div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <script id="resultTemplate" type="text/x-jQuery-tmpl">
        <table class="table mb30">
            <thead>
                <tr>
                    <th>Nombre del archívo</th>
                    <th>Descargar</th>
                </tr>
            </thead>
            <tbody>
                {{each results}}
                    <tr>
                        <td>${NombreArchivo}</td>
                        <td>

                            {{if Errores ==""}}
                                <div class="btn btn-white tooltips">
                                    <img alt="" src="/images/loaders/loader1.gif" id="imgLoading" style="display: none" />
                                    <a id="lnkDownload_${NombreArchivo}" onclick="ricv.descargar('lnkDownload_${NombreArchivo}','${URL}')" download>Descargar</a>
                                </div>
                            {{else}}
                                <span class='label label-danger dropdown-toggle tooltips' data-placement="bottom" data-toggle="dropdown" data-original-title="${Errores}">ERROR. Ver detalle</span>  
                            {{/if}}
                        </td>
                    </tr>
                {{/each}}
            </tbody>
        </table>
    </script>

    <script id="noResultTemplate" type="text/x-jQuery-tmpl">
        <tr>
            <td colspan="2">No se han encontrado resultados
            </td>
        </tr>
    </script>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FooterContent" runat="Server">
    <script type="text/javascript" src="/js/jquery.tmpl.min.js"></script>
    <script src="/js/jquery.validate.min.js"></script>
    <script src="/js/views/reportes/ricv.js?v=<%= ConfigurationManager.AppSettings["JS.Version"] %>"></script>
    <script>
        jQuery(document).ready(function () {
            ricv.configFilters();
        });
    </script>
</asp:Content>

