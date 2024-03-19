<%@ Page Title="" Language="C#" MasterPageFile="~/Front.master" AutoEventWireup="true" CodeFile="conceptos.aspx.cs" Inherits="conceptos" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="pageheader">
        <h2><i class="fa fa-file-text"></i>Productos y servicios <span>Administración</span></h2>
        <div class="breadcrumb-wrapper">
            <span class="label">Estás aquí:</span>
            <ol class="breadcrumb">
                <li><a href="/home.aspx">axanweb</a></li>
                <li class="active">Productos y servicios</li>
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
                                <div class="col-sm-12 col-md-4 col-lg-5">
                                    <input type="text" class="form-control" id="txtCondicion" maxlength="128" placeholder="Ingresá el nombre o codigo del del producto/servicio" />
                                </div>
                                <div class="col-sm-12 col-md-8 col-lg-5">
                                    <div class="btn-group" id="btnAcciones">
                                        <a class="btn btn-warning mr10" onclick="nuevo();">
                                            <i class="fa fa-plus"></i>&nbsp;Nuevo Producto o servicio
                                        </a>
                                    </div>
                                    <div class="btn-group dropdown" id="Div1">
                                        <button type="button" class="btn btn-btn-default"><i class="fa fa-list"></i>&nbsp;Otras acciones</button>
                                        <button class="btn btn-btn-default dropdown-toggle" data-toggle="dropdown"><span class="fa fa-caret-down"></span></button>
                                        <ul class="dropdown-menu">
                                            <li><a href="/importar.aspx?tipo=ListaPrecios">Aumento masivo de precios y stock</a></li>
                                        </ul>
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
                                            <a id="divIconoDescargar" href="javascript:exportar();">
                                                <i class="glyphicon glyphicon-save"></i>&nbsp;Exportar
                                            </a>
                                            <img alt="" src="/images/loaders/loader1.gif" id="imgLoading" style="display: none" />
                                            <a href="" id="lnkDownload" onclick="resetearExportacion();" download="Productos" style="display: none">Descargar</a>
                                        </div>
                                    </div>

                                    <div class="btn-group mr10" id="divPagination" style="display: none">
                                        <a class="btn btn-white" id="lnkPrevPage" style="cursor: pointer" onclick="mostrarPagAnterior();"><i class="glyphicon glyphicon-chevron-left"></i>Anterior</a>
                                        <a class="btn btn-white" id="lnkNextPage" style="cursor: pointer" onclick="mostrarPagProxima();">Siguiente <i class="glyphicon glyphicon-chevron-right"></i></a>
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
                            <div class="alert alert-success" id="divOk" style="display: none">
                                <strong>Bien hecho! </strong><span id="msgOk"></span>
                            </div>
                            <div class="table-responsive">
                                <table class="table mb30">
                                    <thead>
                                        <tr>
                                            <th>Tipo</th>
                                            <th>Nombre</th>
                                            <th>Código</th>
                                            <th>Precio Unitario</th>
                                            <th>Costo Interno</th>
                                            <th>IVA %</th>
                                            <th>CodigoProveedor</th>
                                            <th>Stock</th>
                                            <th>Estado</th>
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
            <td>${Tipo}</td>
            <td>${Nombre}</td>
            <td>${Codigo}</td>
            <td>${Precio}</td>
            <td>${CostoInterno}</td>
            <td>${Iva}</td>
            <td>${CodigoProveedor}</td>
            <td>${Stock}</td>
            <td>${Estado}</td>
            <td class="table-action">
                <a onclick="editar(${ID});" style="cursor: pointer; font-size: 16px" title="Editar"><i class="fa fa-pencil"></i></a>
                <a onclick="eliminar(${ID},'${Nombre}');" style="cursor: pointer; font-size: 16px" class="delete-row" title="Eliminar"><i class="fa fa-trash-o"></i></a>
                <a onclick="duplicar(${ID});" style="cursor: pointer; font-size: 16px" class="delete-row" title="Duplicar"><i class="fa fa-files-o"></i></a>
            </td>
        </tr>
            {{/each}}
        </script>

        <script id="noResultTemplate" type="text/x-jQuery-tmpl">
            <tr>
                <td colspan="7">No se han encontrado resultados</td>
            </tr>
        </script>
    </div>

    <div id="divSinDatos" runat="server">
        <div class="panel-heading" style="background-color: white; height: 30%; width: 100%; border-radius: 4px; text-align: center;">
            <h2 id="hTitulo">Aún no has creado ningún producto o servicio</h2>
            <br />
            <a class="btn btn-warning" onclick="nuevo();" id="btnNuevoSinDatos">Crea un producto o servicio</a>
        </div>
    </div>


</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FooterContent" runat="Server">
    <script type="text/javascript" src="/js/jquery.tmpl.min.js"></script>
    <script src="/js/select2.min.js"></script>
    <script src="/js/views/conceptos.js?v=<%= ConfigurationManager.AppSettings["JS.Version"] %>"></script>

    <script>
        jQuery(document).ready(function () {
            if ($('#divConDatos').is(":visible")) {
                filtrar();
                configFilters();
            }
        });
    </script>
</asp:Content>
