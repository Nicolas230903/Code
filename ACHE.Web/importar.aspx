<%@ Page Title="" Language="C#" MasterPageFile="~/Front.master" AutoEventWireup="true" CodeFile="importar.aspx.cs" Inherits="importar" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
    <link href="/css/jasny-bootstrap.min.css" rel="stylesheet" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="pageheader">
        <h2><i class="fa fa-file-text"></i>Importación masiva de información </h2>
        <div class="breadcrumb-wrapper">
            <span class="label">Estás aquí:</span>
            <ol class="breadcrumb">
                <li><a href="/home.aspx">axanweb</a></li>
                <li class="active">Importación masiva</li>
            </ol>
        </div>
    </div>

    <div class="contentpanel">
        <div class="alert alert-info fade in nomargin">
            <button aria-hidden="true" data-dismiss="alert" class="close" type="button">×</button>
            <h4>¿Cómo funciona la importación masiva?</h4>
            <p>
                Paso 1: Descargá el archivo modelo en formato excel que contiene los titulos de las columnas que axanweb necesita para importar los datos.<br />
                <b>También podés descargar el archivo de ejemplo para que puedas ver cómo completarlo</b>
            </p>
            <p>
                Paso 2: Ingresá los datos de tu sistema anterior o de forma manual dentro del archivo descargado en el Paso 1. Asegúrate que los datos que coincidan con los títulos de las columnas del archivo modelo.
            </p>
            <p>
                Paso 3: Una vez completado el archivo modelo con los datos, debes cambiar su formato a ".csv" (delimitado por comas). Para esto es necesario ir al menú "Archivo -> Guardar Como" y cambiar el tipo de archivo a CSV (delimitado por comas).
            </p>
            <p>
                Paso 4:  Selecioná el archivo .csv generado y hacer click en <b>importar</b>.
            </p>
            <p id="msgListaPrecios">
                <b>Aclaración: Si no desea actualizar el stock deje el campo vacio</b>
            </p>
        </div>
        <br />
        <br />
        <div class="row mb15">
            <form id="frmEdicion" runat="server" class="col-sm-12">
                <div class="panel panel-default">
                    <div class="panel-body">
                        <div class="alert alert-danger" id="divError" style="display: none">
                            <strong>Lo sentimos! </strong><span id="msgError"></span>
                        </div>

                        <div class="alert alert-success" id="divOk" style="display: none">
                            <strong>Bien hecho! </strong>Los datos se han actualizado correctamente
                        </div>

                        <div class="row mb15">
                            <div class="col-sm-3">
                                <div class="form-group">
                                    <label class="control-label"><span class="asterisk">*</span> Tipo de información a importar</label>
                                    <asp:DropDownList runat="server" ID="ddlTipo" CssClass="form-control required" disabled="true">
                                        <asp:ListItem Text="" Value="" Selected="True"></asp:ListItem>
                                        <asp:ListItem Text="Clientes" Value="Clientes"></asp:ListItem>
                                        <asp:ListItem Text="Proveedores" Value="Proveedores"></asp:ListItem>
                                        <asp:ListItem Text="Productos" Value="Productos"></asp:ListItem>
                                        <asp:ListItem Text="Servicios" Value="Servicios"></asp:ListItem>
                                        <asp:ListItem Text="Facturas" Value="Facturas"></asp:ListItem>
                                        <asp:ListItem Text="Cuentas contables" Value="PlanDeCuentas"></asp:ListItem>
                                        <asp:ListItem Text="Actualización de precios y stock" Value="ListaPrecios"></asp:ListItem>
                                    </asp:DropDownList>
                                    <br />
                                    <div id="divArchivos">
                                        <a href="#" id="DescargarModelo" onclick="descargarModelo();">Descargar modelo</a>&nbsp;|&nbsp;
                                        <a href="#" id="DescargarEjemplo" onclick="descargarEjemplo();">Descargar ejemplo</a>                                     
                                    </div>        
                                    <br />
                                </div>
                            </div>
                            <div class="col-sm-6">
                                <%-- <div class="form-group">
                                    <label class="control-label"><span class="asterisk">*</span> Archivo</label>
                                    <asp:FileUpload runat="server" ID="flpArchivo" CssClass="file" data-show-preview="false"/>
                                </div>--%>
                                <div class="form-group">
                                    <label class="control-label"><span class="asterisk">*</span> Archivo</label>
                                    <div class="fileinput fileinput-new input-group" data-provides="fileinput">
                                        <div class="form-control" data-trigger="fileinput" style="height: 40px">
                                            <i class="glyphicon glyphicon-file fileinput-exists"></i>
                                            <span class="fileinput-filename"></span>
                                        </div>
                                        <span class="input-group-addon btn btn-default btn-file">
                                            <span class="fileinput-new">
                                                <i class="glyphicon glyphicon-folder-open"></i>&nbsp;&nbsp;Seleccionar
                                            </span>
                                            <span class="fileinput-exists">
                                                <i class="glyphicon glyphicon-folder-open"></i>&nbsp;&nbsp;Modificar
                                            </span>
                                            <input id="flpArchivo" type="file" />
                                        </span>
                                        <a href="#" class="input-group-addon btn btn-default fileinput-exists" data-dismiss="fileinput">
                                            <i class="glyphicon glyphicon-ban-circle"></i>&nbsp;&nbsp;Remover
                                        </a>

                                    </div>
                                         <div class="col-sm-6 col-md-6" id="ddlPeriodoContainer">
                                                 <select id="ddlPeriodo" onchange="DescargarLote();" runat="server">
                                                     <option value="">Selecciona una opción</option>
                                                     <option value="0">Descargar Actual</option>
                                                 </select>
                                       </div>

                                </div>
                                <input type="hidden" id="hdnFileName" />
                              
                            </div>

                        </div>a
                        <br />
                         <div class="alert alert-info fade in nomargin" id="infoAlerta">
     <button aria-hidden="true" data-dismiss="alert" class="close" type="button">×</button>
     <h4>¿Cómo funciona la importación masiva?</h4>
     <p>
         Paso 1: Descargá el archivo modelo en formato excel que contiene los titulos de las columnas que axanweb necesita para importar los datos.<br />
         <b>También podés descargar el archivo de ejemplo para que puedas ver cómo completarlo</b>
     </p>
     <p>
         Paso 2: Ingresá los datos que quieras actualizar de forma manual dentro del archivo descargado en el Paso 1. Asegúrate que los datos que coincidan con los títulos de las columnas del archivo.
     </p>
      <p>
          Paso 3: Una vez abierto el Archivo descargado, en caso de que el codigo de sus conceptos contenga "0" iniciales tendra que cambiar el formato de la columna. Para esto es necesario seleccionar la columna de Codigo, luego de Click derecho, busque la opcion de Formato de columna y coloquelo en General.<br />
          Lo mismo puede suceder con el stock, el cual usted quiera tener mas de "1000" de stock tendra que cambiar el formato de la columna tambien en general.
      </p>
     <p>
         Paso 4: Una vez completado el archivo descargado con los datos y los formatos correspondientes, <b>debes cambiar su formato a ".csv" (delimitado por comas).</b> Para esto es necesario ir al menú "Archivo -> Guardar Como" y cambiar el tipo de archivo a CSV (delimitado por comas).
     </p>
     <p>
         Paso 5: Selecioná el archivo .csv generado y hacer click en <b>importar</b>.
     </p>
     <p>
         <b>Aclaración: Si no desea actualizar el stock deje el campo como esté </b>

     </p>
 </div>
                    </div>
                    <div class="panel-footer">
                        <asp:Button runat="server" CssClass="btn btn-primary" ID="btnContinuar" Text="Continuar" OnClientClick="return importar();" />
                        <img alt="" src="/images/loaders/loader1.gif" id="imgLoading" style="display: none" />
                        <%--<a class="" onclick="importar();">Importar</a>--%>
                    </div>
                </div>
                <input type="hidden" id="hdnIDLista" value="" runat="server" />
                <input type="hidden" id="hdnTieneCuentasContables" value="" runat="server" />
            </form>
        </div>

        <div id="ContResultados" style="display: none;">
            <div class="panel panel-default">
                <div class="panel-heading">
                    <div class="pull-right">
                        <div class="btn-group mr10" id="divIconoDescargarOK">
                            <div class="btn btn-white tooltips" id="divbtnImportar">
                                <a id="divIconoDescargar" href="javascript:importarDatos();">
                                    <i class="glyphicon glyphicon-save"></i>&nbsp;Importar
                                </a>
                                <img alt="" src="/images/loaders/loader1.gif" id="imgLoading2" style="display: none" />
                                <a href="#" id="lnkDownload" onclick="" download="ImportarDatos" style="display: none">Importar Datos</a>
                            </div>
                        </div>

                        <div class="btn-group mr10" id="divPagination" style="display: none">
                            <a class="btn btn-white" id="lnkPrevPage" style="cursor: pointer" onclick="mostrarPagAnterior();"><i class="glyphicon glyphicon-chevron-left"></i>Anterior</a>
                            <a class="btn btn-white" id="lnkNextPage" style="cursor: pointer" onclick="mostrarPagProxima();">Siguiente <i class="glyphicon glyphicon-chevron-right"></i></a>
                        </div>
                    </div>

                    <h4 class="panel-title">Resultados previos a la importación</h4>
                    <p id="msjResultados"></p>
                </div>
                <!-- panel-heading -->
                <div class="panel-body">
                    <div class="alert alert-danger" id="div1" style="display: none">
                        <button type="button" class="close" data-dismiss="alert" aria-hidden="true">×</button>
                        <strong>Lo sentimos!</strong> <span id="Span1"></span>
                    </div>
                    <asp:Literal runat="server" ID="litTabla"></asp:Literal>
                    <div class="table-responsive" style="overflow: auto;">
                        <table class="table mb30" id="resultThead">
                            <!-- <thead>
                                <tr>
                                    <th>Codigo</th>
                                    <th>Nombre</th>
                                    <th>Descripcion</th>
                                    <th>Observaciones</th>
                                    <th>Precio Unitario</th>
                                    <th>Stock</th>
                                    <th>Iva</th>
                                    <th>Tipo</th>
                                    <th>Resultado</th>
                                </tr>
                            </thead> 
                            <tbody id="resultsContainer">
                            </tbody>-->
                        </table>
                    </div>

                </div>
            </div>
        </div>
    </div>

    <div id="divEspera" style="display: none;">
        <img src="images/loaders/cargando.gif" width="50%" height="50%" />
    </div>

</asp:Content>


<asp:Content ID="Content3" ContentPlaceHolderID="FooterContent" runat="Server">
    <script src="/js/jquery.validate.min.js"></script>
    <script src="/js/jasny-bootstrap.min.js"></script>
    <script src="/js/views/importar.js?v=<%= ConfigurationManager.AppSettings["JS.Version"] %>"></script>

    <script src="/js/jquery.ui.widget.js" type="text/javascript"></script>
    <script src="/js/jquery.iframe-transport.js" type="text/javascript"></script>
    <script src="/js/jquery.fileupload.js" type="text/javascript"></script>
    <script src="js/jquery.blockUI.js"></script>

    <script>
        jQuery(document).ready(function () {
            configForm();
        });
    </script>
</asp:Content>

