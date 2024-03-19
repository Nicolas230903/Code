<%@ Page Title="" Language="C#" MasterPageFile="~/Front.master" AutoEventWireup="true" CodeFile="comunicacionAfip.aspx.cs" Inherits="comunicacionAfip" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="pageheader">
        <h2><i class="fa fa-file-text"></i>Comunicaciones AFIP <span>Administración</span></h2>
        <div class="breadcrumb-wrapper">
            <span class="label">Estás aquí:</span>
            <ol class="breadcrumb">
                <li><a href="/home.aspx">axanweb</a></li>
                <li class="active">Comunicaciones</li>
            </ol>
        </div>
    </div>

    <div id="divConDatos" runat="server">
        <div class="contentpanel">
            <div class="row">
                <div class="col-sm-12 col-md-12 table-results">
                    <div class="panel panel-default">                       
                        <!-- panel-heading -->
                        <div class="panel-body">

                            <h3><p>Comunicaciones NO Leídas</p></h3>

                            <div class="table-responsive">
                                <table class="table mb30" id="tablaComunicacionesAfipNoLeidos">
                                    <thead>
                                        <tr>                                            
                                            <th></th>
                                            <th>CodComunicación</th>
                                            <th>FechaPublicación</th>
                                            <th>FechaVencimiento</th>
                                            <th>SistemaPublicador</th>
                                            <th>Estado</th>
                                            <th>Asunto</th>
                                            <th>Prioridad</th> 
                                            <th>Adjuntos</th> 
                                            <th><input type="checkbox" id="checkAll" class="checkbox-inline" /></th>
                                        </tr>
                                    </thead>
                                    <tbody id="resultsContainerNoLeidos">
                                    </tbody>
                                </table>
                            </div>

                            <div class="panel-footer" runat="server" id="divFooter">
                                <div class="pull-right">
                                    <a class="btn btn-success"  onclick="marcarLeidosTodos();" id="lnkAceptarTodos">Actualizar TODOS como Leídos</a>                                    
                                    <a class="btn btn-success"  onclick="marcarLeidosSeleccionados();" id="lnkAceptar">Actualizar como Leído los seleccionados</a>                                                                        
                                </div>
                            </div>

                            <br />

                            <div class="alert alert-danger" id="divError" style="display: none">
                                <button type="button" class="close" data-dismiss="alert" aria-hidden="true">×</button>
                                <strong>Lo sentimos!</strong> <span id="msgError"></span>
                            </div>
                            <div class="alert alert-success" id="divOk" style="display: none">
                                <strong>Bien hecho! </strong>Los datos se han actualizado correctamente
                            </div>

                            <br />
                            <hr />

                            <h3><p>Comunicaciones Leídas</p></h3>

                            <div class="table-responsive">
                                <table class="table mb30" id="tablaComunicacionesAfipLeidos">
                                    <thead>
                                        <tr>                                            
                                            <th></th>
                                            <th>CodComunicación</th>
                                            <th>FechaPublicación</th>
                                            <th>FechaVencimiento</th>
                                            <th>SistemaPublicador</th>
                                            <th>Estado</th>
                                            <th>Asunto</th>
                                            <th>Prioridad</th>
                                            <th>Adjuntos</th> 
                                        </tr>
                                    </thead>
                                    <tbody id="resultsContainerLeidos">
                                    </tbody>
                                </table>
                            </div>



                        </div>
                    </div>
                </div>

            </div>
        </div>

        
        <script id="noResultTemplate" type="text/x-jQuery-tmpl">
            <tr>
                <td colspan="8">No se han encontrado resultados
                </td>
            </tr>
        </script>

    </div>
    
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FooterContent" runat="Server">
    <script type="text/javascript" src="/js/jquery.Datatables.min.js"></script>
    <script type="text/javascript" src="/js/jquery.tmpl.min.js"></script>
    <script src="/js/views/comunicacionAfip.js?v=<%= ConfigurationManager.AppSettings["JS.Version"] %>"></script>

    <script>
        jQuery(document).ready(function () {
            obtenerComunicacionesAfip();
            if ($('#divConDatos').is(":visible")) {
                configForm();
            }
        });
    </script>
</asp:Content>
