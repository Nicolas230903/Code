<%@ Control Language="C#" AutoEventWireup="true" CodeFile="nuevoCheque.ascx.cs" Inherits="controls_nuevoCheque" %>

<div class="modal modal-wide fade" id="modalNuevoCheque" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
    <div class="modal-dialog">
        <div class="contentpanel">
            <form id="frmNuenoCheque" class="col-sm-12" onsubmit="return false;">
                <div class="panel panel-default">
                    <div class="panel-body">

                        <div class="alert alert-danger" id="divErrorCheque" style="display: none">
                            <strong>Lo sentimos! </strong><span id="msgErrorCheque"></span>
                        </div>

                        <div class="alert alert-success" id="divOkCheque" style="display: none">
                            <strong>Bien hecho! </strong>Los datos se han actualizado correctamente
                        </div>

                        <div class="row mb15">
                            <div class="col-sm-3">

                                <img id="imgFoto" src="/files/usuarios/no-cheque.png" class="thumbnail img-responsive" alt="" runat="server" />
                                <div class="mb30"></div>

                                <div id="divLogo" style="display: none">
                                    <%--<p class="mb30">Formato JPG, PNG o GIF. Tamaño máximo recomendado: 100x70px</p>--%>
                                    <input type="hidden" value="" name="" /><input type="file" id="flpArchivo" />
                                    <div class="mb20"></div>
                                </div>
                                <a class="btn btn-white btn-block" onclick="cheques.showInputLogo();">Adjuntar foto</a>
                            </div>

                            <div class="col-sm-9">
                                <div class="row mb15">
                                    <div class="col-sm-4">
                                        <div class="form-group">
                                            <label class="control-label"><span class="asterisk">*</span> Emisor</label>
                                            <input id="txtEmisorCheque" class="form-control required" maxlength="128" />
                                        </div>
                                    </div>
                                    <div class="col-sm-4">
                                        <div class="form-group">
                                            <label class="control-label"><span class="asterisk">*</span> CUIT</label>
                                            <input type="tel" id="txtCUIT" class="form-control required" maxlength="11" />
                                        </div>
                                    </div>
                                </div>
                                <div class="mb40"></div>
                                <div class="row mb15">
                                    <%--<div class="col-sm-4">
                                        <div class="form-group">
                                            <label class="control-label"><span class="asterisk">*</span> Nombre del Banco</label>
                                            <input id="txtNombreCheque" class="form-control  required" maxlength="150" />
                                        </div>
                                    </div>--%>
                                    <div class="col-sm-4">
                                        <div class="form-group">
                                            <label class="control-label"><span class="asterisk">*</span> EsPropio</label>
                                            <select id="ddlEsPropio" class="form-control" onchange="cheques.changeEsPropio();">
                                                <option value="N" selected>NO</option>
                                                <option value="S">SI</option>
                                            </select>
                                        </div>
                                    </div>
                                    <div class="col-sm-4" id="divChequeEsPropioEmpresa">
                                        <div class="form-group">
                                            <label class="control-label"><span class="asterisk">*</span> Es de la Empresa</label>
                                            <select id="ddlEsPropioEmpresa" class="form-control" onchange="cheques.changeEsPropioEmpresa();">
                                                <option value="N" selected>NO</option>
                                                <option value="S">SI</option>
                                            </select>
                                        </div>
                                    </div>
                                    <div class="col-sm-4" id="divChequePersona">
                                        <div class="form-group">
                                            <label class="control-label"> Cliente</label>
                                            <select class="select2" data-placeholder="Seleccione un cliente..." id="ddlChequePersona">
                                                <option value=""></option>
                                            </select>
                                        </div>
                                    </div>
                                    <div class="col-sm-4">
                                        <div class="form-group">
                                            <label class="control-label"><span class="asterisk">*</span> Nombre del Banco</label>
                                            <select id="ddlBancosCheque" class="select2 required" data-placeholder="seleccione un banco">
                                            </select>
                                        </div>
                                    </div>

                                </div>
                                <div class="row mb15">
                                    <div class="col-sm-4">
                                        <div class="form-group">
                                            <label class="control-label"><span class="asterisk">*</span> Numero de Cheque</label>
                                            <input id="txtNumeroCheque" class="form-control  required" maxlength="150" />
                                        </div>
                                    </div>
                                    <div class="col-sm-4">
                                        <div class="form-group">
                                            <label class="control-label"><span class="asterisk">*</span> Fecha a emisión</label>
                                            <input id="txtFechaEmisionModal" class="form-control required validDate greaterThan" placeholder="dd/mm/yyyy" maxlength="10" />
                                        </div>
                                    </div>
                                    <div class="col-sm-4">
                                        <div class="form-group">
                                            <label class="control-label"><span class="asterisk">*</span> Fecha de cobro</label>
                                            <input id="txtFechaCobrarModal" class="form-control required validDate greaterThan" placeholder="dd/mm/yyyy" maxlength="10" />
                                        </div>
                                    </div>
                                    <div class="col-sm-4 hide">
                                        <div class="form-group">
                                            <label class="control-label"><span class="asterisk">*</span> Fecha de vencimiento</label>
                                            <input id="txtFechaVencimientoModal" class="form-control required" placeholder="dd/mm/yyyy" maxlength="10" />
                                        </div>
                                    </div>
                                </div>
                                <div class="row mb15">
                                    <div class="col-sm-4">
                                        <div class="form-group">
                                            <label class="control-label"><span class="asterisk">*</span> Importe</label>
                                            <input id="txtImporteCheque" class="form-control required" maxlength="150" />
                                        </div>
                                    </div>
                                    <div class="col-sm-8">
                                        <div class="form-group">
                                            <label class="control-label">Observaciones</label>
                                            <textarea rows="5" id="txtObservacionesCheque" class="form-control"></textarea>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="panel-footer">
                        <a class="btn btn-primary" id="actualizarCheque" onclick="cheques.grabarsinImagenModal();">Actualizar</a>
                        <a style="margin-left: 20px" href="#" onclick="$('#modalNuevoCheque').modal('toggle');" tabindex="14">Cancelar</a>
                    </div>
                </div>
                <input id="hdnSinCombioDeFoto" type="hidden" value="0" />
                <input id="hdnIDCheque" type="hidden" value="0" />
                <input id="hdnIDChequePersona" type="hidden" value="0" />
                <input id="hdnChequePropio" type="hidden" value="0" />
                <input id="hdnFileNameCheque" type="hidden" value="" />
                <input id="hdnModo" type="hidden" value="" />
            </form>
        </div>
    </div>
</div>

<script src="/js/views/tesoreria/cheques.js"></script>
<script src="/js/jquery.fileupload.js" type="text/javascript"></script>

<script>
    jQuery(document).ready(function () {
        cheques.configFormModal();
    });
</script>
