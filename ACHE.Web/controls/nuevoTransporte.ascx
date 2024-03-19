<%@ Control Language="C#" AutoEventWireup="true" CodeFile="nuevoTransporte.ascx.cs" Inherits="controls_nuevoTransporte" %>
<div class="modal modal-wide fade" id="modalNuevoTransporte" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
    <div class="modal-dialog">
        <div class="contentpanel">
            <form id="frmNuevoTransporte" class="col-sm-12" onsubmit="return false;">
                <div class="row mb15">
                    <div class="panel panel-default">

                        <div class="panel-body">
                            <div class="alert alert-danger" id="divAyudaTransporte" style="display: none">
                                <strong>Lo sentimos! </strong><span id="msgAyudaTransporte"></span>
                            </div>
                            <div class="alert alert-success" id="divOkTransporte" style="display: none">
                                <span id="msgOkTransporte"></span>
                            </div>     
                            <div class="row mb15">
                                <div class="col-sm-12">
                                    <div class="form-group">
                                        <label class="control-label"> Razón Social</label>
                                        <input type="text" id="txtTransporteRazonSocial" class="form-control"  tabindex="1"/>
                                    </div>
                                </div>
                            </div>                             
                            <div class="row mb15" id="divTransporteGeograficoAfip" hidden="hidden">
                                <div class="col-sm-6">
                                    <div class="form-group">
                                        <label class="control-label"> Provincia</label>
                                        <select id="ddlTransporteProvincia" class="select2" data-placeholder="Selecciona una provincia..." tabindex="0" onchange="changeTransporteProvincia();">
                                        </select>
                                    </div>
                                </div>
                                <div class="col-sm-6">
                                    <div class="form-group">
                                        <label class="control-label"> Ciudad/Localidad</label>
                                        <select id="ddlTransporteCiudad" class="select2" data-placeholder="Selecciona una ciudad..." tabindex="2">
                                        </select>
                                    </div>
                                </div>
                            </div>
                            <div class="row mb15" id="divTransporteGeograficoTexto">
                                <div class="col-sm-6">
                                    <div class="form-group">
                                        <label class="control-label"> Provincia</label>
                                        <input type="text" id="txtTransporteProvinciaTexto" class="form-control"  tabindex="3"/>
                                    </div>
                                </div>
                                <div class="col-sm-6">
                                    <div class="form-group">
                                        <label class="control-label"> Ciudad/Localidad</label>
                                        <input id="txtTransporteCiudadTexto" type="text" class="form-control" tabindex="4"/>
                                    </div>
                                </div>
                            </div> 
                            <div class="row mb15" id="divTransporteDomicilio">
                                <div class="col-sm-6">
                                    <div class="form-group">
                                        <label class="control-label"> Domicilio</label>
                                        <input type="text" id="txtTransporteDomicilio" class="form-control" maxlength="100"  tabindex="5"/>
                                    </div>
                                </div>
                                <div class="col-sm-3">
                                    <div class="form-group">
                                        <label class="control-label">Piso/Depto</label>
                                        <input id="txtTransportePisoDepto" type="text" class="form-control" maxlength="10"  tabindex="6"/>
                                    </div>
                                </div>
                                <div class="col-sm-3">
                                    <div class="form-group">
                                        <label class="control-label">Código Postal</label>
                                        <input id="txtTransporteCp" type="text" class="form-control" maxlength="10"  tabindex="7"/>
                                    </div>
                                </div>
                            </div>   
                            <div class="row mb15" id="divTransporteContacto">
                                <div class="col-sm-6">
                                    <div class="form-group">
                                        <label class="control-label"> Contacto</label>
                                        <input type="text" id="txtTransporteContacto" class="form-control"  tabindex="8"/>
                                    </div>
                                </div>
                                <div class="col-sm-6">
                                    <div class="form-group">
                                        <label class="control-label">Teléfono</label>
                                        <input id="txtTransporteTelefono" type="text" class="form-control" tabindex="9"/>
                                    </div>
                                </div>
                            </div> 
                        </div>
                        <div class="panel-footer">
                            <input type="button" class="btn btn-primary" value="Crear" onclick="crearTransporte();"  tabindex="10"/>
                            <a style="margin-left:20px" href="#" onclick="$('#modalNuevoTransporte').modal('toggle');" tabindex="11">Cancelar</a>
                        </div>
                    </div>
                </div>
                <input type="hidden" id="hdnNuevoTransporteIdUsuario" value="0" />
            </form>
        </div>
    </div>
</div>

<script src="/js/views/nuevoTransporte.js"></script>
