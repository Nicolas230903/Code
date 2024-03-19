<%@ Control Language="C#" AutoEventWireup="true" CodeFile="nuevoTransportePersona.ascx.cs" Inherits="controls_nuevoTransportePersona" %>
<div class="modal modal-wide fade" id="modalNuevoTransportePersona" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
    <div class="modal-dialog">
        <div class="contentpanel">
            <form id="frmNuevoTransportePersona" class="col-sm-12" onsubmit="return false;">
                <div class="row mb15">
                    <div class="panel panel-default">

                        <div class="panel-body">
                            <div class="alert alert-danger" id="divAyudaTransportePersona" style="display: none">
                                <strong>Lo sentimos! </strong><span id="msgAyudaTransportePersona"></span>
                            </div>
                            <div class="alert alert-success" id="divOkTransportePersona" style="display: none">
                                <span id="msgOkTransportePersona"></span>
                            </div>     
                            <div class="row mb15">
                                <div class="col-sm-12">
                                    <div class="form-group">
                                        <label class="control-label"> Razón Social</label>
                                        <input type="text" id="txtTransportePersonaRazonSocial" class="form-control"  tabindex="1"/>
                                    </div>
                                </div>
                            </div>                             
                            <div class="row mb15" id="divTransportePersonaGeograficoAfip" hidden="hidden">
                                <div class="col-sm-6">
                                    <div class="form-group">
                                        <label class="control-label"> Provincia</label>
                                        <select id="ddlTransportePersonaProvincia" class="select2" data-placeholder="Selecciona una provincia..." tabindex="0" onchange="changeTransportePersonaProvincia();">
                                        </select>
                                    </div>
                                </div>
                                <div class="col-sm-6">
                                    <div class="form-group">
                                        <label class="control-label"> Ciudad/Localidad</label>
                                        <select id="ddlTransportePersonaCiudad" class="select2" data-placeholder="Selecciona una ciudad..." tabindex="2">
                                        </select>
                                    </div>
                                </div>
                            </div>
                            <div class="row mb15" id="divTransportePersonaGeograficoTexto">
                                <div class="col-sm-6">
                                    <div class="form-group">
                                        <label class="control-label"> Provincia</label>
                                        <input type="text" id="txtTransportePersonaProvinciaTexto" class="form-control"  tabindex="3"/>
                                    </div>
                                </div>
                                <div class="col-sm-6">
                                    <div class="form-group">
                                        <label class="control-label"> Ciudad/Localidad</label>
                                        <input id="txtTransportePersonaCiudadTexto" type="text" class="form-control" tabindex="4"/>
                                    </div>
                                </div>
                            </div> 
                            <div class="row mb15" id="divTransportePersonaDomicilio">
                                <div class="col-sm-6">
                                    <div class="form-group">
                                        <label class="control-label"> Domicilio</label>
                                        <input type="text" id="txtTransportePersonaDomicilio" class="form-control" maxlength="100"  tabindex="5"/>
                                    </div>
                                </div>
                                <div class="col-sm-3">
                                    <div class="form-group">
                                        <label class="control-label">Piso/Depto</label>
                                        <input id="txtTransportePersonaPisoDepto" type="text" class="form-control" maxlength="10"  tabindex="6"/>
                                    </div>
                                </div>
                                <div class="col-sm-3">
                                    <div class="form-group">
                                        <label class="control-label">Código Postal</label>
                                        <input id="txtTransportePersonaCp" type="text" class="form-control" maxlength="10"  tabindex="7"/>
                                    </div>
                                </div>
                            </div>   
                            <div class="row mb15" id="divTransportePersonaContacto">
                                <div class="col-sm-6">
                                    <div class="form-group">
                                        <label class="control-label"> Contacto</label>
                                        <input type="text" id="txtTransportePersonaContacto" class="form-control"  tabindex="8"/>
                                    </div>
                                </div>
                                <div class="col-sm-6">
                                    <div class="form-group">
                                        <label class="control-label">Teléfono</label>
                                        <input id="txtTransportePersonaTelefono" type="text" class="form-control" tabindex="9"/>
                                    </div>
                                </div>
                            </div> 
                        </div>
                        <div class="panel-footer">
                            <input type="button" class="btn btn-primary" value="Crear" onclick="crearTransportePersona();"  tabindex="10"/>
                            <a style="margin-left:20px" href="#" onclick="$('#modalNuevoTransportePersona').modal('toggle');" tabindex="11">Cancelar</a>
                        </div>
                    </div>
                </div>
                <input type="hidden" id="hdnNuevoTransportePersonaIdUsuario" value="0" />
            </form>
        </div>
    </div>
</div>

<script src="/js/views/nuevoTransportePersona.js"></script>
