<%@ Page Title="" Language="C#" MasterPageFile="~/Front.master" AutoEventWireup="true" CodeFile="file-explorer.aspx.cs" Inherits="file_explorer" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
    <link rel="stylesheet" href="/css/prettyPhoto.css" />
    <link rel="stylesheet" href="/css/dropzone.css" />
    <style type="text/css">
        .smallModalWidth {
            width: 400px;
        }
    </style>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="pageheader">
        <h2><i class="fa fa-folder-open"></i>Explorador de archivos <span>En esta sección se almacerán todas las facturas, cheques, comprobantes, recibos, etc que hayas generado y/o almacenado.</span></h2>
        <div class="breadcrumb-wrapper">
            <span class="label">Estás aquí:</span>
            <ol class="breadcrumb">
                <li><a href="/home.aspx">axanweb</a></li>
                <li class="active">Explorador de archivos</li>
            </ol>
        </div>
    </div>
    <div class="contentpanel">
        <div class="filemanager-options">
            <ul class="filemanager-options">
                <li>
                    <div class="ckbox ckbox-default">
                        <input type="checkbox" id="selectall" value="1" />
                        <label for="selectall">Seleccionar todos</label>
                    </div>
                </li>
                <li>
                    <a href="javascript:downloadAllPathSelected();" class="itemopt disabled"><i class="fa fa-download"></i>Descargar</a>
                </li>
                <li>
                    <a id="lnkDeleteAll" href="javascript:deleteAllPathSelected();" class="itemopt disabled"><i class="fa fa-trash-o"></i>Eliminar</a>
                </li>
                <li class="filter-type" id="divPath"></li>
            </ul>
        </div>
        <div class="row">
            <div class="col-sm-9">
                <div class="row" id="divUpload" style="display: none; margin-bottom: 20px;">
                    <div id="my-awesome-dropzone" class="dropzone">
                        <div class="fallback">
                            <input name="file" type="file" multiple />
                        </div>
                    </div>
                </div>
                <div class="row filemanager" id="resultsContainer"></div>
            </div>
            <!-- col-sm-9 -->
            <div class="col-sm-3">
                <div class="fm-sidebar">
                    <button class="btn btn-warning btn-block" onclick="$('#divUpload').slideDown();">Subir archivos</button>
                    <div class="mb30"></div>
                    <h5 class="subtitle">Carpetas <a href="javascript:showNewFolder();" class="pull-right">+ crear nueva carpeta</a></h5>
                    <div class="form-inline" id="divNuevaCarpeta" style="display: none">
                        <input type="text" class="form-control input-sm" id="txtNuevaCarpeta" maxlength="100" />
                        <button class="btn btn-success btn-sm" onclick="createNewFolder();">Crear</button>
                        <button class="btn btn-danger btn-sm" onclick="cancelNewFolder();">Cancelar</button>
                    </div>
                    <ul class="folder-list" id="resultsFolders">
                    </ul>
                </div>
            </div>
            <!-- col-sm-3 -->
        </div>
    </div>
    <script id="folderTemplate" type="text/x-jQuery-tmpl">
        {{each results}}
            <li><a href="javascript:loadInfo('${Path}')"><i class="fa fa-folder-o"></i>${Nombre}</a></li>
        {{/each}}
    </script>
    <input type="hidden" id="hdnPath" />
    <script id="resultTemplate" type="text/x-jQuery-tmpl">
        {{each results}}
            <div class="col-xs-6 col-sm-4 col-md-2">
                <div class="thmb">
                    <div class="ckbox ckbox-default" style="display: none;">
                        <input type="checkbox" id="chk${ID}" value="1" />
                        <label for="chk${ID}"></label>
                        <input type="hidden" name="hdnPathAux" value="${Path}" />
                        <input type="hidden" name="hdnTipoAux" value="${Tipo}" />
                    </div>
                    <div class="btn-group fm-group" style="display: none;">
                        <button type="button" class="btn btn-default dropdown-toggle fm-toggle" data-toggle="dropdown"><span class="caret"></span></button>
                        <ul class="dropdown-menu fm-menu" role="menu">
                            <%--<li><a href="#"><i class="fa fa-envelope-o"></i>Email</a></li>--%>
                            <li><a href="javascript:downloadPath('${Tipo}','${Path}');"><i class="fa fa-download"></i>Descargar</a></li>
                            <li><a href="javascript:deletePath('${Tipo}','${Path}');"><i class="fa fa-trash-o"></i>Eliminar</a></li>
                            <li><a href="javascript:renamePath('${Tipo}','${Path}');"><i class="fa fa-edit"></i>Renombrar</a></li>
                        </ul>
                    </div>
                    <div class="thmb-prev">
                        {{if Tipo == "I"}}
                            <a href="${Imagen}" data-rel="prettyPhoto" rel="prettyPhoto">
                                <img src="${Icono}" class="img-responsive" alt="" />
                            </a>
                        {{else Tipo == "C"}}    
                            <a href="javascript:loadInfo('${Path}')">
                                <img src="${Icono}" class="img-responsive" alt="" /></a>
                        {{else}}
                            <a href="#">
                                <img src="${Icono}" class="img-responsive" alt="" /></a>
                        {{/if}}
                    </div>
                    {{if Tipo == "C"}}
                        <h5 class="fm-title"><a href="javascript:loadInfo('${Path}')">${Nombre}</a></h5>
                    {{else}}
                        <h5 class="fm-title"><a href="#">${Nombre}</a></h5>
                    {{/if}}
                    <small class="text-muted">Fecha alta: ${FechaAlta}</small>
                </div>
            </div>
        {{/each}}
    </script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FooterContent" runat="Server">
    <script type="text/javascript" src="/js/jquery.tmpl.min.js"></script>
    <script src="/js/dropzone.min.js"></script>
    <script src="/js/views/fileExplorer.js?v=<%= ConfigurationManager.AppSettings["JS.Version"] %>"></script>
    <script src="/js/jquery.prettyPhoto.js"></script>
</asp:Content>

