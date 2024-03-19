<%@ Page Title="" Language="C#" MasterPageFile="~/Front.master" AutoEventWireup="true" CodeFile="elegir-plan.aspx.cs" Inherits="modulos_seguridad_elegir_plan" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
    <style type="text/css">
        .blog-meta li {
            float: inherit;
            padding: 0;
            font-size: 13px;
            border-right: 0;
        }
    </style>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">
    <div class="pageheader">
        <h2><i class='fa fa-comment'></i>
            <asp:Literal ID="liTitulo" runat="server"></asp:Literal></h2>
    </div>

    <div class="contentpanel" runat="server" visible="false">
        <div class="row">
            <div class="col-sm-12 col-md-12">
                <div class="row mb15" style="margin-top: 20px; margin-left: 5%; margin-right: 5%">
                    <div class="col-lg-3 col-sm-4 col-md-3" id="PlanBasico" runat="server">
                        <div class="blog-item blog-quote">
                            <div class="quote quote-success">
                                <a href="#" style="cursor: text;">PLAN BÁSICO</a>
                            </div>
                            <div class="blog-details">
                                <ul class="blog-meta">
                                    <li>HASTA 100 comprobantes de ventas y compras</li>
                                    <li>Ventas</li>
                                    <li>Cobranzas</li>
                                    <li>Pago a proveedores</li>
                                    <li>Tablero de control</li>
                                </ul>
                                <hr />
                                <div style="text-align: center; min-height: 53px;">
                                    <h4>GRATIS</h4>
                                </div>
                                <div style="text-align: center">
                                    <button class="btn btn-white" onclick="elegirPlan.ActualizarPlanBasico()" id="btnPlanBasico">CONTRATAR</button>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="col-lg-3 col-sm-4 col-md-3" id="planProfesional" runat="server">
                        <div class="blog-item blog-quote">
                            <div class="quote quote-success">
                                <a href="#" style="cursor: text;">PLAN PROFESIONAL</a>
                            </div>
                            <div class="blog-details">
                                <ul class="blog-meta">
                                    <li>FACTURAS ILIMITADAS</li>
                                    <li>Exportacion AFIP</li>
                                    <li>Presupuestos</li>
                                    <li>Explorador de Archivos</li>
                                    <li>Y todas las funciones del plan Básico</li>
                                </ul>
                                <hr />
                                <div class="form-group">
                                    <span>
                                        <input id="Radio1" type="radio" name="rProfesional" runat="server" value="false" onchange="PlanesPagos.changeFormaDePago();" checked />
                                        <asp:Literal ID="liPlanProfesionalMes" runat="server"></asp:Literal>
                                    </span>
                                    <br />
                                    <span>
                                        <input id="Radio2" type="radio" name="rProfesional" runat="server" value="true" />
                                        <asp:Literal ID="liPlanProfesionalAnual" runat="server"></asp:Literal>
                                    </span>
                                </div>
                                <div style="text-align: center">
                                    <button class="btn btn-white" onclick="elegirPlan.subirPlan('Profesional')">CONTRATAR</button>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="col-lg-3 col-sm-4 col-md-3" id="planPyme" runat="server">
                        <div class="blog-item blog-quote">
                            <div class="quote quote-success">
                                <a href="#" style="cursor: text;">PLAN PYME</a>
                            </div>
                            <div class="blog-details">
                                <ul class="blog-meta">
                                    <li>FACTURAS ILIMITADAS</li>
                                    <li>Listas de precios</li>
                                    <li>Caja Diaria</li>
                                    <li>Acceso Clientes</li>
                                    <li>Y todas las funciones del plan Profesional</li>
                                </ul>
                                <hr />
                                <div class="form-group">
                                    <span>
                                        <input id="Radio3" type="radio" name="rPyme" runat="server" value="false" checked />
                                        <asp:Literal ID="liPlanPymeMes" runat="server"></asp:Literal>
                                    </span>
                                    <br />
                                    <span>
                                        <input id="Radio4" type="radio" name="rPyme" runat="server" value="true" />
                                        <asp:Literal ID="liPlanPymeAnual" runat="server"></asp:Literal>
                                    </span>
                                </div>
                                <div style="text-align: center">
                                    <button class="btn btn-white" onclick="elegirPlan.subirPlan('Pyme')">CONTRATAR</button>
                                </div>
                            </div>
                        </div>
                    </div>

                    <div class="col-lg-3 col-sm-4 col-md-3" id="planEmpresa" runat="server">
                        <div class="blog-item blog-quote">
                            <div class="quote quote-success">
                                <a href="#" style="cursor: text;">PLAN EMPRESA</a>
                            </div>
                            <div class="blog-details">
                                <ul class="blog-meta">
                                    <li>FACTURAS ILIMITADAS</li>
                                    <li>Multi CUIT (hasta 5)</li>
                                    <li>RRHH</li>
                                    <li>Traking Horas</li>
                                    <li>Y todas las funciones del plan PYME</li>
                                </ul>
                                <hr />
                                <div class="form-group">
                                    <span>
                                        <input id="Radio5" type="radio" name="rEmpresa" runat="server" value="false" checked />
                                        <asp:Literal ID="liPlanEmpresaMes" runat="server"></asp:Literal>
                                    </span>
                                    <br />
                                    <span>
                                        <input id="Radio6" type="radio" name="rEmpresa" runat="server" value="true" />
                                        <asp:Literal ID="liPlanEmpresaAnual" runat="server"></asp:Literal>
                                    </span>
                                </div>
                                <div style="text-align: center">
                                    <button class="btn btn-white" onclick="elegirPlan.subirPlan('Empresa')">CONTRATAR</button>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="FooterContent" runat="Server">
    <%--<script src="/js/views/common.js"></script>--%>
    <script src="/js/views/seguridad/elegirPlan.js?v=<%= ConfigurationManager.AppSettings["JS.Version"] %>"></script>
</asp:Content>



