<%@ Page Title="" Language="C#" MasterPageFile="~/Front.master" AutoEventWireup="true" CodeFile="personas.aspx.cs" Inherits="personas" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">

    <div class="pageheader">
        <h2>
            <asp:Literal runat="server" ID="litTitulo"></asp:Literal>
            <span>Administración</span></h2>
        <div class="breadcrumb-wrapper">
            <span class="label">Estás aquí:</span>
            <ol class="breadcrumb">
                <li><a href="/home.aspx">axanweb</a></li>
                <li class="active">
                    <asp:Literal runat="server" ID="litPath"></asp:Literal></li>
            </ol>
        </div>
    </div>

    <div id="divConDatos" runat="server">
        <div class="contentpanel">
            <div class="row">
                <div class="col-sm-12 col-md-12 table-results">
                    <div class="panel panel-default">
                        <div class="panel-heading">
                            <input type="hidden" id="hdnPage" runat="server" value="1" />
                            <div class="col-sm-5 col-md-5">
                                <input type="text" class="form-control" id="txtRazonSocial" maxlength="128" placeholder="Ingresá Nombre,Código, CUIT o DNI" />
                            </div>
                            <div class="col-sm-5 col-md-5">
                                <div class="btn-group mr10">
                                    <a class="btn btn-warning mr10" onclick="nuevo();">
                                        <i class="fa fa-plus"></i>&nbsp;Nuevo  <asp:Literal runat="server" ID="litTipo"></asp:Literal>
                                    </a>
                                    <a class="btn btn-default mr10" onclick="importar();">
                                        <i class="fa fa-upload"></i> Importación masiva
                                    </a>
                                </div>
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
                                            <a id="divIconoDescargar" href="javascript:exportar();">
                                                <i class="glyphicon glyphicon-save"></i>&nbsp;Exportar
                                            </a>
                                            <img alt="" src="/images/loaders/loader1.gif" id="imgLoading" style="display: none" />
                                            <a href="" id="lnkDownload" onclick="resetearExportacion();" download="ClientesProv" style="display: none">Descargar</a>
                                        </div>
                                    </div>

                                    <div class="btn-group mr10" id="divPagination" style="display: none">
                                        <a class="btn btn-white" id="lnkPrevPage" style="cursor: pointer" onclick="mostrarPagAnterior();"><i class="glyphicon glyphicon-chevron-left"></i>Anterior</a>
                                        <a class="btn btn-white" id="lnkNextPage" style="cursor: pointer" onclick="mostrarPagProxima();">Siguiente <i class="glyphicon glyphicon-chevron-right"></i></a>
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
                                            <th>Logo</th>
                                            <th>Razón Social</th>
                                            <th>Código</th>
                                            <th>DNI/CUIT</th>
                                            <th>Cat. Impositiva</th>
                                            <th>Teléfono</th>
                                            <th>Email</th>
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
                <td>{{if TieneFoto == 1}}
                        <img src="${Foto}" alt="" style="width: 40px; height: 40px;" />
                    {{else}}
                        <img src="/files/usuarios/no-photo.png" alt="" style="width: 25px; height: 25px;" />
                    {{/if}}
                </td>
                <td>${RazonSocial} &nbsp;<small><i>${NombreFantasia}</i></small></td>
                <td>${Codigo}</td>
                <td>${NroDoc}</td>
                <td>${CondicionIva}</td>
                <td>${Telefono}</td>
                <td>${Email}</td>
                <td class="table-action">
                    <a onclick="editar(${ID});" style="cursor: pointer; font-size: 16px" title="Editar"><i class="fa fa-pencil"></i></a>
                    <a onclick="eliminar(${ID},'${RazonSocial}');" style="cursor: pointer; font-size: 16px" class="delete-row" title="Eliminar"><i class="fa fa-trash-o"></i></a>
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
            <h2 id="hTitulo"></h2>
            <br />
            <a class="btn btn-warning" onclick="nuevo();" id="btnNuevoSinDatos"></a>
        </div>
    </div>
    <input type="hidden" id="hdnTipo" runat="server" />
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FooterContent" runat="Server">
    <script type="text/javascript" src="/js/jquery.tmpl.min.js"></script>
    <script src="/js/views/personas.js?v=<%= ConfigurationManager.AppSettings["JS.Version"] %>"></script>

    <script>
        jQuery(document).ready(function () {
            if ($('#divConDatos').is(":visible")) {
                filtrar();
                configFilters();
            }
            else {
                configFormSinDatos();
            }
        });
    </script>
</asp:Content>
