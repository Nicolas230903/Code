<%@ Page Title="" Language="C#" MasterPageFile="~/Front.master" AutoEventWireup="true" CodeFile="empresas.aspx.cs" Inherits="empresas" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="pageheader">
        <h2><i class='fa fa-briefcase'></i>Empresas <span>Administración</span></h2>
        <div class="breadcrumb-wrapper">
            <span class="label">Estás aquí:</span>
            <ol class="breadcrumb">
                <li><a href="/home.aspx">axanweb</a></li>
                <li class="active">Empresas</li>
            </ol>
        </div>
    </div>

    <div class="contentpanel">

        <div class="panel">

            <div class="panel-heading">
                <%--<h5 class="bug-key-title"></h5>--%>
                <div class="panel-title" id="btnNuevo">
                    <button class="btn btn-success btn-xs" onclick="Empresa.nuevo();" type="button" style="margin-left: 30px"><i class="fa fa-plus mr5"></i>Agregar empresa</button>
                </div>
            </div>
            <div class="panel-body">

                <div class="row">

                    <div class="people-list">
                        <div class="row" id="resultsContainer">
                        </div>
                    </div>

                </div>
            </div>
        </div>
    </div>

    <script id="resultTemplateEmpresas" type="text/x-jQuery-tmpl">
        {{each results}}
            <div class="col-md-6">
                <div class="people-item">
                    <div class="media">
                        <a href="#" class="pull-left">
                            <img alt="" src="${Logo}" class="thumbnail media-object" />
                        </a>
                        <div class="media-body">
                            <h4 class="person-name">${RazonSocial}
                              <%--  &nbsp;&nbsp;|&nbsp;&nbsp;
                                 {{if  $("#IDUsuarioAdicional").val() == "0"}}
                                <a id="btnEditar" onclick="Empresa.editar(${ID});" style="cursor: pointer; font-size: 16px" title="Editar"><i class="fa fa-pencil"></i></a>
                                <a id="btnEliminar" onclick="Empresa.eliminar(${ID},'${RazonSocial}');" style="cursor: pointer; font-size: 16px" class="delete-row" title="Eliminar"><i class="fa fa-trash-o"></i></a>
                                {{/if}}--%>
                            </h4>
                            <div class="text-muted"><i class="fa fa-map-marker"></i>${Domicilio}, ${Ciudad}, ${Provincia}</div>
                            <div class="text-muted"><i class="fa fa-briefcase"></i>${CUIT}, ${CondicionIva}</div>
                            <ul class="social-list">
                                {{if ID==$("#IDUsuarioActual").val()}}
                                    <li><a class="btn btn-success" style="width: 100% !important; text-align: left;">Usted se encuentra usando ${RazonSocial}</a></li>
                                {{else}}
                                    <li><a onclick="Empresa.cambiarSesion(${ID},'${RazonSocial}');" title="Acceder" class="btn btn-default" style="width: 100% !important; text-align: left;"><i class="fa  fa-sign-in"></i>Usar axanweb como ${RazonSocial}</a></li>
                                {{/if}}
                            </ul>
                        </div>
                    </div>
                </div>
            </div>
        {{/each}}
    </script>

    <script id="noResultTemplateEmpresas" type="text/x-jQuery-tmpl">
        <h4>No se han encontrado ptras empresas</h4>

    </script>

    <form runat="server">
        <input type="hidden" name="name" value="" id="IDUsuarioAdicional" runat="server" />
        <input type="hidden" name="name" value="" id="IDUsuarioActual" runat="server" />
    </form>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FooterContent" runat="Server">
    <script type="text/javascript" src="/js/jquery.tmpl.min.js"></script>
    <script src="/js/views/seguridad/empresas.js"></script>
    <script>
        jQuery(document).ready(function () {
            Empresa.filtrar();
            Empresa.configFilters();
        });
    </script>
</asp:Content>
