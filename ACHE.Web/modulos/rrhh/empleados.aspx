<%@ Page Title="" Language="C#" MasterPageFile="~/Front.master" AutoEventWireup="true" CodeFile="empleados.aspx.cs" Inherits="modulos_rrhh_empleados" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="pageheader">
        <h2><i class='fa fa-child'></i>Empleados <span>Administración</span></h2>
        <div class="breadcrumb-wrapper">
            <span class="label">Estás aquí:</span>
            <ol class="breadcrumb">
                <li><a href="/home.aspx">axanweb</a></li>
                <li class="active">Empleados</li>
            </ol>
        </div>
    </div>
    <div id="divConDatos" runat="server">

        <div class="contentpanel">
            <div class="row">

                <div class="col-sm-4 col-md-3">

                    <a class="btn btn-warning btn-block" onclick="empleados.nuevo();">
                        <i class="glyphicon glyphicon-plus"></i>&nbsp;Nuevo empleado
                    </a>
                    <div class="mb20"></div>

                    <input type="hidden" id="hdnTipo" runat="server" />
                    <input type="hidden" id="hdnPage" runat="server" value="1" />

                    <h4 class="subtitle mb5">Filtros disponibles</h4>

                    <div class="mb20"></div>
                    <h4 class="subtitle mb5">Nombre</h4>
                    <input type="text" class="form-control" id="txtNombre" maxlength="128" />

                    <div class="mb20"></div>
                    <h4 class="subtitle mb5">Apellido</h4>
                    <input type="text" class="form-control" id="txtApellido" maxlength="128" />

                    <div class="mb20"></div>
                    <h4 class="subtitle mb5">CUIT</h4>
                    <input type="text" class="form-control" id="txtCUIT" maxlength="20" />

                    <div class="mb20"></div>
                    <a class="btn btn-black" onclick="empleados.resetearPagina();empleados.filtrar();">Buscar</a>
                    <a class="btn btn-default" onclick="empleados.resetearPagina();empleados.verTodos();">Ver todos</a>
                    <br />
                </div>

                <div class="col-sm-8 col-md-9 table-results">
                    <div class="panel panel-default">
                        <div class="panel-heading">
                            <div class="pull-right">
                                <div class="btn-group mr10">
                                    <div class="btn btn-white tooltips">
                                        <a id="divIconoDescargar" href="javascript:empleados.exportar();">
                                            <i class="glyphicon glyphicon-save"></i>&nbsp;Exportar
                                        </a>
                                        <img alt="" src="/images/loaders/loader1.gif" id="imgLoading" style="display: none" />
                                        <a href="" id="lnkDownload" onclick="empleados.resetearExportacion();" style="display: none">Descargar</a>
                                    </div>
                                </div>

                                <div class="btn-group mr10" id="divPagination" style="display: none">
                                    <a class="btn btn-white" id="lnkPrevPage" style="cursor: pointer" onclick="empleados.mostrarPagAnterior();"><i class="glyphicon glyphicon-chevron-left"></i>Anterior</a>
                                    <a class="btn btn-white" id="lnkNextPage" style="cursor: pointer" onclick="empleados.mostrarPagProxima();">Siguiente <i class="glyphicon glyphicon-chevron-right"></i></a>
                                </div>
                            </div>

                            <h4 class="panel-title">Resultados</h4>
                            <p id="msjResultados"></p>
                        </div>

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
                                            <th>Nombre</th>
                                            <th>Apellido</th>
                                            <th>CUIT</th>
                                            <th>Nro de Legajo</th>
                                            <th>Email</th>
                                            <th>Telefono</th>
                                            <th>Domicilio</th>
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
            <td>${Apellido}</td>
            <td>${CUIT}</td>
            <td>${NroLegajo}</td>
            <td>${Email}</td>
            <td>${Telefono}</td>
            <td>${Domicilio}</td>
            <td class="table-action">
                <a onclick="empleados.editar(${ID});" style="cursor: pointer; font-size: 16px" title="Editar"><i class="fa fa-pencil"></i></a>
                <a onclick="empleados.eliminar(${ID},'${Apellido}');" style="cursor: pointer; font-size: 16px" class="delete-row" title="Eliminar"><i class="fa fa-trash-o"></i></a>
            </td>
        </tr>
            {{/each}}
        </script>

        <script id="noResultTemplate" type="text/x-jQuery-tmpl">
            <tr>
                <td colspan="8">No se han encontrado resultados
                </td>
            </tr>
        </script>
    </div>

    <div id="divSinDatos" runat="server">
        <div class="panel-heading" style="background-color: white; height: 30%; width: 100%; border-radius: 4px; text-align: center;">
            <h4 id="hTitulo">Aún no has creado ningún empleado</h4>
            <br>
            <a class="btn btn-warning" onclick="empleados.nuevo();" id="btnNuevoSinDatos">Crea un empleado</a>
        </div>
    </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FooterContent" runat="Server">
    <script type="text/javascript" src="/js/jquery.tmpl.min.js"></script>
    <script src="/js/views/rrhh/empleados.js?v=<%= ConfigurationManager.AppSettings["JS.Version"] %>"></script>

    <script>
        jQuery(document).ready(function () {
            if ($('#divConDatos').is(":visible")) {
                empleados.filtrar();
                empleados.configFilters();
            }
        });
    </script>
</asp:Content>
