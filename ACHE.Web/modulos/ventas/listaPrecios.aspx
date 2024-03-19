<%@ Page Title="" Language="C#" MasterPageFile="~/Front.master" AutoEventWireup="true" CodeFile="listaPrecios.aspx.cs" Inherits="modulos_ventas_listaPrecios" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="pageheader">
        <h2><i class='fa fa-list'></i>Lista de precios <span>Administración</span></h2>
        <div class="breadcrumb-wrapper">
            <span class="label">Estás aquí:</span>
            <ol class="breadcrumb">
                <li><a href="/home.aspx">axanweb</a></li>
                <li><a href="/conceptos.aspx">Productos y servicios</a></li>
                <li class="active">Lista de precios</li>
            </ol>
        </div>
    </div>

    <div id="divConDatos" runat="server">

        <div class="contentpanel">
            <div class="row">
                <div class="col-sm-12 col-md-12 table-results">
                    <div class="panel panel-default">
                        <div class="panel-heading">
                            <input type="hidden" id="hdnTipo" runat="server" />
                            <input type="hidden" id="hdnPage" runat="server" value="1" />
                            <div class="col-sm-5 col-md-5">
                                <input type="text" class="form-control" id="txtNombre" maxlength="128" placeholder="Ingresá el nombre de la lista" />
                            </div>
                            <div class="col-sm-3 col-md-3">
                                <a class="btn btn-warning" onclick="listaPrecios.nuevo();">
                                    <i class="fa fa-plus"></i>&nbsp;Nueva lista de precios
                                </a>
                            </div>
                            <div class="col-sm-2 col-md-2"></div>

                            <div class="col-sm-12">
                                <hr />
                            </div>
                            <div class="row mb20"></div>
                            <div class="row">
                                <div class="pull-right">
                                    <div class="btn-group mr10">
                                        <div class="btn btn-white tooltips">
                                            <a id="divIconoDescargar" href="javascript:listaPrecios.exportar();">
                                                <i class="glyphicon glyphicon-save"></i>&nbsp;Exportar
                                            </a>
                                            <img alt="" src="/images/loaders/loader1.gif" id="imgLoading" style="display: none" />
                                            <a href="" id="lnkDownload" onclick="listaPrecios.resetearExportacion();" download="ClientesProv" style="display: none">Descargar</a>
                                        </div>
                                    </div>

                                    <div class="btn-group mr10" id="divPagination" style="display: none">
                                        <a class="btn btn-white" id="lnkPrevPage" style="cursor: pointer" onclick="listaPrecios.mostrarPagAnterior();"><i class="glyphicon glyphicon-chevron-left"></i>Anterior</a>
                                        <a class="btn btn-white" id="lnkNextPage" style="cursor: pointer" onclick="listaPrecios.mostrarPagProxima();">Siguiente <i class="glyphicon glyphicon-chevron-right"></i></a>
                                    </div>
                                </div>

                                <h4 class="panel-title" style="clear: left; padding-left: 20px">Resultados</h4>
                                <p id="msjResultados" style="padding-left: 20px"></p>
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
                                            <!--th>#</th-->
                                            <th>Nombre</th>
                                            <th>Observaciones</th>
                                            <th>Activa</th>
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
            <!--td>${ID}</td-->
            <td>${Nombre}</td>
            <td>${Observaciones}</td>
            <td>${Activa}</td>
            <td class="table-action">
                <a onclick="listaPrecios.editar(${ID});" style="cursor: pointer; font-size: 16px" title="Editar"><i class="fa fa-pencil"></i></a>
                <a onclick="listaPrecios.eliminar(${ID},'${Nombre}');" style="cursor: pointer; font-size: 16px" class="delete-row" title="Eliminar"><i class="fa fa-trash-o"></i></a>
            </td>
        </tr>
            {{/each}}
        </script>

        <script id="noResultTemplate" type="text/x-jQuery-tmpl">
            <tr>
                <td colspan="4">No se han encontrado resultados
                </td>
            </tr>
        </script>
    </div>

    <div id="divSinDatos" runat="server">
        <div class="panel-heading" style="background-color: white; height: 30%; width: 100%; border-radius: 4px; text-align: center;">
            <h4 id="hTitulo">Aún no has creado ninguna lista de precios</h4>
            <br>
            <a class="btn btn-warning" onclick="listaPrecios.nuevo();" id="btnNuevoSinDatos">Crea una lista</a>
        </div>
    </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FooterContent" runat="Server">
    <script type="text/javascript" src="/js/jquery.tmpl.min.js"></script>
    <script src="/js/views/ventas/listaPrecios.js?v=<%= ConfigurationManager.AppSettings["JS.Version"] %>"></script>

    <script>
        jQuery(document).ready(function () {
            if ($('#divConDatos').is(":visible")) {
                listaPrecios.filtrar();
                listaPrecios.configFilters();
            }
        });
    </script>
</asp:Content>
