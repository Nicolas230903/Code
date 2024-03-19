<%@ Page Title="" Language="C#" MasterPageFile="~/Front.master" AutoEventWireup="true" CodeFile="gastosGenerales.aspx.cs" Inherits="gastosGenerales" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="pageheader">
        <h2><i class="fa fa-tasks"></i>Gastos Generales realizados <span>Administracion</span></h2>
        <div class="breadcrumb-wrapper">
            <span class="label">Estás aquí:</span>
            <ol class="breadcrumb">
                <li><a href="/home.aspx">axanweb</a></li>
                <li class="active">Administracion</li>
            </ol>
        </div>
    </div>
    <div id="divConDatos" runat="server">
        <div class="contentpanel">
            <div class="row">
                <div class="col-sm-12 col-md-12 table-results">
                    <div class="panel panel-default">
                        <div class="panel-heading">
                            <form runat="server" id="frmSearch">
                                <input type="hidden" id="hdnPage" runat="server" value="1" />
                               <div class="col-sm-12" style="padding-left:inherit">
                                    <div class="col-sm-10 col-md-5">
                                        <div class="row">
                                            <div class="col-sm-6 col-md-6">
                                                <select class="select2" data-placeholder="Seleccione un periodo de tiempo..." id="ddlPeriodo" onchange="GastosGenerales.otroPeriodo();">
                                                    <option value="30">Últimos 30 dias</option>
                                                    <option value="15">Últimos 15 dias</option>
                                                    <option value="7">Últimos 7 dias</option>
                                                    <option value="1">Ayer</option>
                                                    <option value="0">Hoy</option>
                                                    <option value="-1">Otro período</option>
                                                    <option value="-2" selected="selected">Todos</option>
                                                </select>
                                            </div>
                                        </div>

                                        <div class="row mb20"></div>

                                        <div id="divMasFiltros" style="display: none">
                                            <div class="row">
                                                <div class="col-sm-6 col-md-6">
                                                    <div class="input-group">
                                                        <span class="input-group-addon"><i class="glyphicon glyphicon-calendar"></i></span>
                                                        <asp:TextBox runat="server" ID="txtFechaDesde" CssClass="form-control validDate greaterThan" placeholder="Fecha desde" MaxLength="10" onchange="GastosGenerales.filtrar();"></asp:TextBox>
                                                    </div>
                                                </div>

                                                <div class="col-sm-6 col-md-6">
                                                    <div class="input-group">
                                                        <span class="input-group-addon"><i class="glyphicon glyphicon-calendar"></i></span>
                                                        <asp:TextBox runat="server" ID="txtFechaHasta" CssClass="form-control validDate greaterThan" placeholder="Fecha hasta" MaxLength="10" onchange="GastosGenerales.filtrar();"></asp:TextBox>
                                                    </div>
                                                </div>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-sm-2 col-md-2">
                                        <div class="col-sm-2 col-md-2">
                                            <a class="btn btn-warning" onclick="GastosGenerales.nuevo();">
                                                <i class="fa fa-plus"></i>&nbsp;Nuevo gasto
                                            </a>
                                        </div>
                                    </div>
                                </div>
                            </form>

                            <div class="col-sm-12">
                                <hr />
                            </div>
                            <div class="row mb20"></div>
                            <div class="row">
                                <div class="pull-right">
                                    <div class="btn-group mr10">
                                        <div class="btn btn-white tooltips">
                                            <a id="divIconoDescargar" href="javascript:GastosGenerales.exportar();">
                                                <i class="glyphicon glyphicon-save"></i>&nbsp;Exportar
                                            </a>
                                            <img alt="" src="/images/loaders/loader1.gif" id="imgLoading" style="display: none" />
                                            <a href="" id="lnkDownload" onclick="GastosGenerales.resetearExportacion();" download="GastosGenerales" style="display: none">Descargar</a>
                                        </div>
                                    </div>

                                    <div class="btn-group mr10" id="divPagination" style="display: none">
                                        <a class="btn btn-white" id="lnkPrevPage" style="cursor: pointer" onclick="GastosGenerales.mostrarPagAnterior();"><i class="glyphicon glyphicon-chevron-left"></i>Anterior</a>
                                        <a class="btn btn-white" id="lnkNextPage" style="cursor: pointer" onclick="GastosGenerales.mostrarPagProxima();">Siguiente <i class="glyphicon glyphicon-chevron-right"></i></a>
                                    </div>
                                </div>


                                <h4 class="panel-title" style="clear:left;padding-left:20px">Resultados</h4>
                                <p id="msjResultados" style="padding-left:20px"></p>

                            </div>
                        </div>
                        <!-- panel-heading -->
                        <div class="panel-body">

                            <div class="alert alert-danger" id="divError" style="display: none">
                                <button type="button" class="close" data-dismiss="alert" aria-hidden="true">×</button>
                                <strong>Lo sentimos!</strong> <span id="msgError"></span>
                            </div>

                            <div class="table-responsive">
                                <table class="table mb30">
                                    <thead>
                                        <tr>
                                            <!--th>#</!--th-->
                                            <th>Periodo</th>
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
            </div>
        </div>

        <script id="resultTemplate" type="text/x-jQuery-tmpl">
            {{each results}}
        <tr>
            <td>${Periodo}</td>
            <td>${Total}</td>
            <td class="table-action">
                <a onclick="GastosGenerales.editar(${ID});" style="cursor: pointer; font-size: 16px" title="Editar"><i class="fa fa-pencil"></i></a>
                <a onclick="GastosGenerales.eliminar(${ID},'${Periodo}');" style="cursor: pointer; font-size: 16px" class="delete-row" title="Eliminar"><i class="fa fa-trash-o"></i></a>
            </td>
        </tr>
            {{/each}}
        </script>

        <script id="noResultTemplate" type="text/x-jQuery-tmpl">
            <tr>
                <td colspan="10">No se han encontrado resultados
                </td>
            </tr>
        </script>
    </div>

    <div id="divSinDatos" runat="server">
        <div class="panel-heading" style="background-color: white; height: 30%; width: 100%; border-radius: 4px; text-align: center;">
            <h2 id="hTitulo">Aún no has creado ningun gasto general</h2>
            <br />
            <a class="btn btn-warning" onclick="GastosGenerales.nuevo();" id="btnNuevoSinDatos">Crea un nuevo gasto</a>
        </div>
    </div>


</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FooterContent" runat="Server">
    <script type="text/javascript" src="/js/jquery.tmpl.min.js"></script>
    <script src="/js/jquery.validate.min.js"></script>
    <script src="/js/select2.min.js"></script>
    <script src="/js/views/gastosGenerales.js?v=<%= ConfigurationManager.AppSettings["JS.Version"] %>"></script>

    <script>
        jQuery(document).ready(function () {
            if ($('#divConDatos').is(":visible")) {
                GastosGenerales.filtrar();
                GastosGenerales.configFilters();
            }
        });
    </script>
</asp:Content>
