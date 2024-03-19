<%@ Page Title="" Language="C#" MasterPageFile="~/Front.master" AutoEventWireup="true" CodeFile="planDeCuentas.aspx.cs" Inherits="modulos_contabilidad_planDeCuentas" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="Server">
    <link href="/css/dist/themes/default/style.min.css" rel="stylesheet" />

    <style>
        .accesos-cc {
            font-size: 17px;
            font-family: "Roboto",sans-serif;
            line-height: normal;
            margin: 0px;
            color: rgb(99, 110, 123);
        }

        .modal-dialog {
            width: 850px;
        }
    </style>

</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="Server">

    <div class="pageheader">
        <h2><i class='fa fa-university'></i>Plan de cuentas <span>Administración</span></h2>
        <div class="breadcrumb-wrapper">
            <span class="label">Estás aquí:</span>
            <ol class="breadcrumb">
                <li><a href="/home.aspx">axanweb</a></li>
                <li class="active">Plan de cuentas</li>
            </ol>
        </div>
    </div>

    <div class="contentpanel">
        <div class="row">
            <div class="alert alert-danger" id="divErrorPlan" style="display: none">
                <strong>Lo sentimos! </strong><span id="msgErrorPlan"></span>
            </div>
            <div class="alert alert-success" id="divOkPlan" style="display: none">
                <strong>Bien hecho! </strong>Los datos se han actualizado correctamente
            </div>

            <div class="col-sm-12 col-md-12 table-results">
                <div class="panel panel-default">
                    <div class="col-sm-6">
                        <div class="panel panel-success">
                            <div class="panel-heading">
                                <h3 class="panel-title">Plan de cuenta
                                    <div class="btn-group dropup" style="float: right; margin-top: -10px" id="btnOpciones">
                                        <button type="button" class="btn btn-warning">Opciones</button>
                                        <button class="btn btn-warning dropdown-toggle" data-toggle="dropdown"><span class="fa fa-caret-down"></span></button>
                                        <ul class="dropdown-menu">
                                            <li><a href="#" onclick="planDeCuentas.nuevo()">Nueva cuenta</a></li>
                                            <li><a href="#" onclick="planDeCuentas.modificar()">Modificar cuenta</a></li>
                                            <li><a href="#" onclick="planDeCuentas.eliminar()">Eliminar cuenta</a></li>
                                        </ul>
                                    </div>
                                </h3>
                            </div>
                            <div class="panel-body">
                                <div id="divPlanDeCuenta"></div>
                            </div>
                        </div>
                    </div>

                    <div class="col-sm-6">
                        <div class="panel panel-success">
                            <div class="panel-heading">
                                <h3 class="panel-title">Accesos directos contables</h3>
                            </div>
                            <div class="panel-body">

                                <div class="col-sm-12 col-md-12">
                                    <div class="panel panel-default" style="min-height: 300px">
                                        <div class="panel-body" style="padding-top: 40px;">

                                            <div class="col-sm-2 col-md-2" style="text-align: center; margin-right: 15px">
                                                <a href="#" class="accesos-cc" onclick="planDeCuentas.cierreContable();">
                                                    <i class="fa fa-file-text fa-4x"></i>
                                                    <span style="display: block">Crear asiento de cierre</span>
                                                </a>
                                            </div>

                                            <div class="col-sm-2 col-md-2" style="text-align: center; margin-right: 15px">
                                                <a href="/modulos/reportes/LibroDiario.aspx" class="accesos-cc">
                                                    <i class="fa fa-book fa-4x"></i>
                                                    <span style="display: block">Libro diario</span>
                                                </a>
                                            </div>

                                            <div class="col-sm-2 col-md-2" style="text-align: center; margin-right: 15px">
                                                <a href="/modulos/reportes/LibroMayor.aspx" class="accesos-cc">
                                                    <i class="fa fa-book fa-4x"></i>
                                                    <span style="display: block">Libro mayor</span>
                                                </a>
                                            </div>

                                            <div class="col-sm-2 col-md-2" style="text-align: center; margin-right: 15px">
                                                <a href="/modulos/reportes/balanceGeneral.aspx" class="accesos-cc">
                                                    <i class="fa fa-refresh fa-4x"></i>
                                                    <span style="display: block">Balance general</span>
                                                </a>
                                            </div>

                                            <div class="col-sm-2 col-md-2" style="text-align: center; margin-right: 15px">
                                                <a href="/modulos/contabilidad/asientosManuales.aspx" class="accesos-cc">
                                                    <i class="fa fa-table fa-4x"></i>
                                                    <span style="display: block">Asientos manuales</span>
                                                </a>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>

    <input type="hidden" id="hdnID" value="0" />


    <!-- Modal -->
    <div class="modal modal-wide fade" id="modalPlanDeCuentas" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
        <div class="modal-dialog">
            <div class="panel panel-success">
                <div class="panel-heading">
                    <h3 class="panel-title">Cuenta <span id="spnCuenta"></span></h3>
                </div>
                <div class="panel-body">
                    <div class="alert alert-danger" id="divError" style="display: none">
                        <strong>Lo sentimos! </strong><span id="msgError"></span>
                    </div>
                    <div class="alert alert-success" id="divOk" style="display: none">
                        <strong>Bien hecho! </strong>Los datos se han actualizado correctamente
                    </div>
                    <form id="frmEdicion">

                        <div class="row mb15">
                            <div class="col-sm-12">
                                <hr />
                                <h3>Cuentas contables </h3>
                                <label class="control-label">Estos son los datos básicos que el sistema necesita para poder crear una cuenta contable.</label>
                            </div>
                        </div>
                        <div class="row mb15">
                            <div class="col-sm-3">
                                <div class="form-group">
                                    <label class="control-label"><span class="asterisk">*</span> Código</label>
                                    <input id="txtCodigo" class="form-control required" />
                                </div>
                            </div>
                            <div class="col-sm-3">
                                <div class="form-group">
                                    <label class="control-label"><span class="asterisk">*</span> Descripción</label>
                                    <input id="txtNombre" class="form-control required" />
                                </div>
                            </div>
                            <div class="col-sm-4">
                                <div class="form-group">
                                    <label class="control-label"> Cuenta superior</label>
                                    <select id="dllCuentas" class="select2" data-placeholder="Seleccione una cuenta..." onchange="planDeCuentas.changePlanDeCuentas();">
                                    </select>
                                </div>
                            </div>
                        </div>
                        <div class="row mb15">
                            <div class="col-sm-3">
                                <div class="form-group">
                                    <label class="control-label"><span class="asterisk">*</span> Admite Asiento Manual</label>
                                    <select id="ddlAdminiteAsientoManual" class="select2 required" data-placeholder="Seleccione una opción">
                                        <option value=""></option>
                                        <option value="SI">SI</option>
                                        <option value="NO">NO</option>
                                    </select>
                                </div>
                            </div>
                            <div class="col-sm-3" id="divTipoDeCuenta">
                                <div class="form-group">
                                    <label class="control-label"><span class="asterisk">*</span> Tipo de cuenta</label>
                                    <select id="ddlTipoDeCuenta" class="select2 required" data-placeholder="Seleccione una opción">
                                        <option value=""></option>
                                        <option value="ACTIVO">Activo</option>
                                        <option value="PASIVO">Pasivo</option>
                                        <option value="RESULTADO">Resultado</option>
                                        <option value="RESULTADO">Patrimonio neto</option>
                                    </select>
                                </div>
                            </div>
                        </div>

                    </form>
                </div>
                <div class="panel-footer">
                    <a class="btn btn-success" id="btnActualizar" onclick="planDeCuentas.grabar();">Aceptar</a>
                    <a href="#" onclick="planDeCuentas.cancelar();" style="margin-left: 20px">Cancelar</a>
                </div>
            </div>
        </div>
    </div>

</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="FooterContent" runat="Server">
    <script type="text/javascript" src="/js/jquery.tmpl.min.js"></script>
    <script src="/js/jquery.validate.min.js"></script>
    <script src="/js/select2.min.js"></script>
    <script src="/js/jstree.min.js"></script>
    <script src="/js/jstree.js"></script>
    <script src="/js/views/contabilidad/planDeCuentas.js"></script>
    <script>
        jQuery(document).ready(function () {
            planDeCuentas.config();
        });
    </script>
</asp:Content>

