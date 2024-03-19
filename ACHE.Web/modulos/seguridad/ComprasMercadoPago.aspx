<%@ Page Title="" Language="C#" MasterPageFile="~/Front.master" AutoEventWireup="true" CodeFile="ComprasMercadoPago.aspx.cs" Inherits="modulos_ventas_ComprasMercadoPago" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" Runat="Server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" Runat="Server">
  <div class="pageheader">
        <h2><i class='fa fa-list'></i>Mercado Pago <span>Administración</span></h2>
        <div class="breadcrumb-wrapper">
            <span class="label">Estás aquí:</span>
            <ol class="breadcrumb">
                <li><a href="/home.aspx">axanweb</a></li>
                <li class="active"> Mercado Pago</li>
            </ol>
        </div>
    </div>

<div class="contentpanel">
    <div class="row">
        <div class="col-sm-12 col-md-12">
            <div class="panel panel-default">
                <div class="panel-heading">
                    <h4 class="panel-title">Resultado de la operación :  <asp:Literal runat="server" ID="liResultadoOperacion"/></h4>
                    <p id="msjResultados"></p>
                </div>
                <!-- panel-heading -->
                <div class="panel-body">
                    <div class="alert alert-danger" id="divError" style="display: none">
                        <button type="button" class="close" data-dismiss="alert" aria-hidden="true">×</button>
                        <strong>Lo sentimos!</strong> <span id="msgError"></span>
                    </div>

                    <p>
                      <asp:Literal runat="server" ID="liMensajeResultadoOperacion"/>
                    </p>
                      <p>
                        Datos de la operación
                    </p>
                    <p>
                        Plan: <asp:Literal runat="server" ID="liPlan"/>
                    </p>
                    <p>
                        Importe del plan: <asp:Literal runat="server" ID="liImporte"/> + IVA por mes
                    </p>

                    <a href="/home.aspx"> Volver atras</a>
                </div>
            </div>
        </div>
    </div>
</div>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FooterContent" Runat="Server">
</asp:Content>



