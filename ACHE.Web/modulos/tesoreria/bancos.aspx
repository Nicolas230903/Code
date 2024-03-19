<%@ Page Title="" Language="C#" MasterPageFile="~/Front.master" AutoEventWireup="true" CodeFile="bancos.aspx.cs" Inherits="modulos_Tesoreria_bancos" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="pageheader">
        <h2><i class='fa fa-university'></i>Bancos <span>Administración</span></h2>
        <div class="breadcrumb-wrapper">
            <span class="label">Estás aquí:</span>
            <ol class="breadcrumb">
                <li><a href="/home.aspx">axanweb</a></li>
                <li class="active">Bancos</li>
            </ol>
        </div>
    </div>
    <div id="divConDatos" runat="server">

        <div class="contentpanel">
            <div class="panel panel-default">
                <div class="panel-body">
                    <div class="alert alert-danger" id="divError" style="display: none">
                        <button type="button" class="close" data-dismiss="alert" aria-hidden="true">×</button>
                        <strong>Lo sentimos!</strong> <span id="msgError"></span>
                    </div>
                    <div class="row mb15">
                        <div class="col-sm-5">
                        </div>
                        <div class="col-sm-7" id="btnNuevo" runat="server" style="margin-top: 10px">
                            <button class="btn btn-warning" onclick="Bancos.nuevo();" type="button"><i class="fa fa-plus mr5"></i>Nuevo banco</button>
                        </div>
                    </div>
                    <hr />
                    <div class="row">
                        <div class="people-list">
                            <div class="row" id="resultsContainer">
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!--Empresas-->
        <script id="resultTemplate" type="text/x-jQuery-tmpl">
            {{each results}}
            <div class="col-md-6">
                <div class="people-item">
                    <div class="media">
                        <a href="#" class="pull-left">
                            <img alt="" src="../../images/bancos/bancobase_${IDBancoBase}.png" class="thumbnail media-object" />
                        </a>
                        <div class="media-body">
                            <h4 class="person-name">${Nombre}</h4>
                            <div class="text-muted"><i class="fa fa-tag"></i> Nro Cuenta: ${NroCuenta}</div>
                            <div class="text-muted"><i class="fa fa-usd"></i><b>Saldo Actual: ${SaldoActual}</b></div>
                            <ul class="social-list">
                                <li><a onclick="Bancos.editar(${ID});" title="Editar" class="btn btn-success" style="width: 100% !important; text-align: left;"><i class="fa fa-pencil"></i>Editar</a></li>
                                <li><a onclick="Bancos.eliminar(${ID},'${Nombre}');" title="Eliminar" class="btn btn-success" style="width: 100% !important; text-align: left;"><i class="fa fa-trash-o"></i>Eliminar</a></li>
                            </ul>
                        </div>
                    </div>
                </div>
            </div>
            {{/each}}
        </script>
        <script id="noResultTemplate" type="text/x-jQuery-tmpl">
            <h4>No se han encontrado otras empresas</h4>
        </script>
    </div>

    <div id="divSinDatos" runat="server">
        <div class="panel-heading" style="background-color: white; height: 30%; width: 100%; border-radius: 4px; text-align: center;">
            <h4 id="hTitulo">Aún no has creado ningún banco</h4>
            <br>
            <a class="btn btn-warning" onclick="Bancos.nuevo();" id="btnNuevoSinDatos">Crea un banco</a>
        </div>
    </div>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FooterContent" runat="Server">
    <script type="text/javascript" src="/js/jquery.tmpl.min.js"></script>
    <script src="/js/views/tesoreria/bancos.js?v=<%= ConfigurationManager.AppSettings["JS.Version"] %>"></script>

    <script>
        jQuery(document).ready(function () {
            if ($('#divConDatos').is(":visible")) {
                Bancos.filtrar();
                Bancos.configFilters();
            }
        });
    </script>
</asp:Content>
