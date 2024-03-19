<%@ Control Language="C#" AutoEventWireup="true" CodeFile="nuevoConcepto.ascx.cs" Inherits="controls_nuevoConcepto" %>
<div class="modal modal-wide fade" id="modalNuevoConcepto" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
    <div class="modal-dialog">
        <div class="contentpanel">
            <form id="frmNuevoConcepto" class="col-sm-12" onsubmit="return false;">
                <div class="row mb15">
                    <div class="panel panel-default">

                        <div class="panel-body">
                            <div class="alert alert-danger" id="divAyudaConcepto" style="display: none">
                                <strong>Lo sentimos! </strong><span id="msgAyudaConcepto"></span>
                            </div>
                            <div class="alert alert-success" id="divOkConcepto" style="display: none">
                                <span id="msgOkConcepto"></span>
                            </div>
                            <div class="row mb15">
                                <div class="col-sm-6">
                                    <div class="form-group">
                                        <label class="control-label"><span class="asterisk">*</span> Código interno</label>
                                        <input type="text" id="txtCodigo" name="txtCodigo" class="form-control" maxlength="50" tabindex="1" />
                                    </div>
                                </div>
                                <div class="col-sm-6" >
                                    <div class="form-group">
                                        <label class="control-label"><span class="asterisk">*</span> Nombre Producto o Servicio</label>                                        
                                        <input type="text" id="txtNombre" name="txtNombre" class="form-control required" maxlength="100" tabindex="2" />
                                    </div>
                                </div>                                
                            </div>
                            <div class="row mb15">
                                <div class="col-sm-6">
                                    <div class="form-group">
                                        <label class="control-label"><span class="asterisk">*</span> Estado</label>
                                        <select id="ddlEstado" class="form-control required" tabindex="3" disabled="disabled">
                                            <option value=""></option>
                                            <option value="A" selected>Activo</option>
                                            <option value="I">Inactivo</option>
                                        </select>
                                    </div>
                                </div>
                                <div class="col-sm-6">
                                    <div class="form-group">
                                        <label class="control-label"><span class="asterisk">*</span> Tipo</label>
                                        <select id="ddlTipo" class="form-control required" tabindex="4" disabled="disabled">
                                            <option value=""></option>
                                            <option value="P" selected>Producto</option>
                                            <option value="S">Servicio</option>
                                        </select>
                                    </div>
                                </div>
                            </div>
                            <div id="divMontos" style="display: none">
                                <div class="row mb15" >
                                    <div class="col-sm-6">
                                        <div class="form-group">
                                            <label class="control-label">Costo interno</label>
                                            <div class="input-group">
                                                <span class="input-group-addon">$</span>                                            
                                                <input type="text" id="txtCostoInterno" name="txtCostoInterno" value="1,00" class="form-control" maxlength="10" tabindex="5" />
                                            </div>
                                        </div>
                                    </div>
                                    <div class="col-sm-6">
                                        <div class="form-group">
                                            <label class="control-label">
                                                <span class="asterisk">*</span>
                                                Precio unit. con IVA
                                            </label>
                                            <div class="input-group">
                                                <span class="input-group-addon">$</span>
                                                <input type="text" id="txtPrecio" name="txtPrecio" value="1,00" class="form-control required" maxlength="10" tabindex="6" />                                            
                                            </div>
                                        </div>
                                    </div>
                                </div>
                            </div>
                            <div class="row mb15">
                                <div class="col-sm-6">
                                    <div class="form-group">
                                        <label class="control-label"><span class="asterisk">*</span> IVA %</label>
                                        <select id="ddlIva" class="form-control required" tabindex="7">
<%--                                            <option value="0,00">NG</option>
                                            <option value="0,00">EX</option>--%>
                                            <option value="0,00">0</option>
                                            <option value="2,50">2,5</option>
                                            <option value="5,00">5</option>
                                            <option value="10,50">10,5</option>
                                            <option value="21,00">21</option>
                                            <option value="27,00">27</option>
                                        </select>
                                    </div>
                                </div>                               
                            </div>
                        </div>
                        <div class="panel-footer">
                            <input type="button" class="btn btn-primary" value="Crear" onclick="crearConcepto();"  tabindex="15"/>
                            <a style="margin-left:20px" href="#" onclick="$('#modalNuevoConcepto').modal('toggle');" tabindex="16">Cancelar</a>
                        </div>
                    </div>
                </div>
                <input type="hidden" id="hdnNuevoConceptoTipo" value="P" />
            </form>
        </div>
    </div>
</div>

<script src="/js/views/nuevoConcepto.js"></script>

<script>
    $(document).ajaxStart(function () {
        document.body.style.cursor = "wait";
        $("#divProcesando").show();
    }).ajaxStop(function () {
        document.body.style.cursor = "default";
        $("#divProcesando").hide();
    });
</script>