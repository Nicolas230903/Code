<%@ Control Language="C#" AutoEventWireup="true" CodeFile="nuevoDomicilio.ascx.cs" Inherits="controls_nuevoDomicilio" %>
<div class="modal modal-wide fade" id="modalNuevoDomicilio" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
    <div class="modal-dialog">
        <div class="contentpanel">
            <form id="frmNuevoDomicilio" class="col-sm-12" onsubmit="return false;">
                <div class="row mb15">
                    <div class="panel panel-default">

                        <div class="panel-body">
                            <div class="alert alert-danger" id="divAyudaDomicilio" style="display: none">
                                <strong>Lo sentimos! </strong><span id="msgAyudaDomicilio"></span>
                            </div>
                            <div class="alert alert-success" id="divOkDomicilio" style="display: none">
                                <span id="msgOkDomicilio"></span>
                            </div>                            
                            <div class="row mb15" id="divGeograficoAfip" hidden="hidden">
                                <div class="col-sm-6">
                                    <div class="form-group">
                                        <label class="control-label"> Provincia</label>
                                        <select id="ddlDomicilioProvincia" class="select2" data-placeholder="Selecciona una provincia..." tabindex="0" onchange="changeDomicilioProvincia();">
                                        </select>
                                    </div>
                                </div>
                                <div class="col-sm-6">
                                    <div class="form-group">
                                        <label class="control-label"> Ciudad/Localidad</label>
                                        <select id="ddlDomicilioCiudad" class="select2" data-placeholder="Selecciona una ciudad..." tabindex="1">
                                        </select>
                                    </div>
                                </div>
                            </div>
                            <div class="row mb15" id="divGeograficoTexto">
                                <div class="col-sm-6">
                                    <div class="form-group">
                                        <label class="control-label"> Provincia</label>
                                        <input type="text" id="txtDomicilioProvinciaTexto" class="form-control"  tabindex="2"/>
                                    </div>
                                </div>
                                <div class="col-sm-6">
                                    <div class="form-group">
                                        <label class="control-label"> Ciudad/Localidad</label>
                                        <input id="txtDomicilioCiudadTexto" type="text" class="form-control" tabindex="3"/>
                                    </div>
                                </div>
                            </div> 
                            <div class="row mb15" id="divDomicilio">
                                <div class="col-sm-6">
                                    <div class="form-group">
                                        <label class="control-label"> Domicilio</label>
                                        <input type="text" id="txtDomicilioDomicilio" class="form-control" maxlength="100"  tabindex="4"/>
                                    </div>
                                </div>
                                <div class="col-sm-3">
                                    <div class="form-group">
                                        <label class="control-label">Piso/Depto</label>
                                        <input id="txtDomicilioPisoDepto" type="text" class="form-control" maxlength="10"  tabindex="5"/>
                                    </div>
                                </div>
                                <div class="col-sm-3">
                                    <div class="form-group">
                                        <label class="control-label">Código Postal</label>
                                        <input id="txtDomicilioCp" type="text" class="form-control" maxlength="10"  tabindex="6"/>
                                    </div>
                                </div>
                            </div>   
                            <div class="row mb15" id="divContacto">
                                <div class="col-sm-6">
                                    <div class="form-group">
                                        <label class="control-label"> Contacto</label>
                                        <input type="text" id="txtDomicilioContacto" class="form-control"  tabindex="7"/>
                                    </div>
                                </div>
                                <div class="col-sm-6">
                                    <div class="form-group">
                                        <label class="control-label">Teléfono</label>
                                        <input id="txtDomicilioTelefono" type="text" class="form-control" tabindex="8"/>
                                    </div>
                                </div>
                            </div> 
                        </div>
                        <div class="panel-footer">
                            <input type="button" class="btn btn-primary" value="Crear" onclick="crearDomicilio();"  tabindex="9"/>
                            <a style="margin-left:20px" href="#" onclick="$('#modalNuevoDomicilio').modal('toggle');" tabindex="10">Cancelar</a>
                        </div>
                    </div>
                </div>
                <input type="hidden" id="hdnNuevoDomicilioIdPersona" value="0" />
            </form>
        </div>
    </div>
</div>

<script src="/js/views/nuevoDomicilio.js"></script>
