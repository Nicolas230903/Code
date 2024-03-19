<%@ Page Title="" Language="C#" MasterPageFile="~/Front.master" AutoEventWireup="true" CodeFile="consultaComprobantesAfip.aspx.cs" Inherits="consultaComprobantesAfip" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="pageheader">
        <h2><i class="fa fa-file-text"></i>Consulta Comprobantes AFIP <span>Administración</span></h2>
        <div class="breadcrumb-wrapper">
            <span class="label">Estás aquí:</span>
            <ol class="breadcrumb">
                <li><a href="/home.aspx">axanweb</a></li>
                <li class="active">Consulta Comprobantes AFIP</li>
            </ol>
        </div>
    </div>


    <div class="contentpanel">
        <div class="row">
            <div class="col-sm-12 col-md-12 table-results">
                <div class="panel panel-default">                       
                    <!-- panel-heading -->
                    <div class="panel-body">

                        <h3>Consulta comprobantes AFIP</h3>
                        <h4 style="text-align:right">Estado Servicio Web AFIP: <span style="color:green">ON LINE</span></h4>

                        <div class="alert alert-danger" id="divErrorConsultaComprobantesAfip" style="display: none">
                            <button type="button" class="close" data-dismiss="alert" aria-hidden="true">×</button>
                            <strong>Lo sentimos!</strong> <span id="msgErrorConsultaComprobantesAfip"></span>
                        </div>

                        <div class="table-responsive">
                            <table class="table mb30" id="tablaConsultaComprobantesAfip">
                                <thead>
                                    <tr>                                            
                                        <th>Tipo</th>
                                        <th>Punto de venta</th>
                                        <th>Ultimo Número</th>
                                    </tr>
                                </thead>
                                <tbody id="resultsContainerConsultaComprobantesAfip">
                                </tbody>
                            </table>
                        </div>

                        <div class="panel-footer" runat="server" id="divFooter">
                            <div class="pull-right">
                                <a class="btn btn-success"  onclick="obtenerConsultaComprobantesAfip();" id="lnkConsultaComprobantesAfip">Recargar</a>                                    
                            </div>
                        </div>

                        <br />
                        <br />
                        <hr />

                        <h3><p>Consultar por número de comprobante en AFIP </p></h3>

                        <div class="row mb15">
                            <div class="col-sm-3 col-md-3">
                                <div class="form-group">
                                    <label class="control-label"> Tipo de comprobante</label>
                                    <select class="form-control required" id="ddlTipo" data-placeholder="Seleccione un tipo">
                                        <option value=""></option>
                                    </select>
                                </div>
                            </div>
                            <div class="col-sm-3 col-md-3" id="divPuntoVenta">
                                <div class="form-group">
                                    <label class="control-label"> Punto de venta</label>
                                    <select id="ddlPuntoVenta" class="form-control required" onchange="changePuntoDeVenta();">
                                    </select>
                                </div>
                            </div>
                            <div class="col-sm-3 col-md-3" id="divNroComprobante">
                                <div class="form-group">
                                    <label class="control-label"> Nro. Comprobante</label>
                                    <input id="txtNumeroComprobante" class="form-control" maxlength="9" type="tel" />
                                </div>
                            </div>
                        </div>
                        <div class="row mb15">
                        <div class="col-sm-2 col-md-2">
                            <div class="form-group">
                                <a class="form-control btn btn-info"  onclick="recuperarComprobanteAfip();" id="lnkRecuperarComprobanteAfip">Consultar</a>                                    
                            </div>
                        </div>

                        </div>

                        <div class="alert alert-danger" id="divErrorRecuperarComprobanteAfip" style="display: none">
                            <button type="button" class="close" data-dismiss="alert" aria-hidden="true">×</button>
                            <strong>Lo sentimos!</strong> <span id="msgErrorRecuperarComprobanteAfip"></span>
                        </div>

                        <div class="table-responsive" id="divRespuestaRecuperarComprobanteAfip">
                            <table class="table mb30" id="tablaRecuperarComprobanteAfip">
                                <thead>
                                    <tr>                                            
                                        <th>Campo</th>
                                        <th>Valor</th>
                                    </tr>
                                </thead>
                                <tbody id="resultsContainerRecuperarComprobanteAfip">
                                </tbody>
                            </table>
                        </div>



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
    <script type="text/javascript" src="/js/jquery.Datatables.min.js"></script>
    <script type="text/javascript" src="/js/jquery.tmpl.min.js"></script>
    <script src="/js/views/consultaComprobantesAfip.js?v=<%= ConfigurationManager.AppSettings["JS.Version"] %>"></script>
    <script src="js/jquery.blockUI.js?v=<%= ConfigurationManager.AppSettings["JS.Version"] %>"></script>

    <script>
        jQuery(document).ready(function () {
            configForm();            
        });
    </script>
</asp:Content>
