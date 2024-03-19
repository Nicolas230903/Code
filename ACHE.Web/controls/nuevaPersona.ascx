<%@ Control Language="C#" AutoEventWireup="true" CodeFile="nuevaPersona.ascx.cs" Inherits="controls_nuevaPersona" %>
<div class="modal modal-wide fade" id="modalNuevoCliente" tabindex="-1" role="dialog" aria-labelledby="myModalLabel" aria-hidden="true">
    <div class="modal-dialog">
        <div class="contentpanel">
            <form id="frmNuevaPersona" class="col-sm-12" onsubmit="return false;">
                <div class="row mb15">
                    <div class="panel panel-default">

                        <div class="panel-body">
                            <div class="alert alert-danger" id="divAyudaCliente" style="display: none">
                                <strong>Lo sentimos! </strong><span id="msgAyudaCliente"></span>
                            </div>
                            <div class="alert alert-success" id="divOkCliente" style="display: none">
                                <span id="msgOkCliente"></span>
                            </div>
                            <div class="row mb15" id="divNumeroNuevaPersona">
                                <div class="col-sm-6">
                                    <div class="form-group">
                                        <label class="control-label"><span class="asterisk">*</span> Número</label>
                                        <input id="txtNroDocumento" type="text" class="form-control number validCuit" maxlength="11" tabindex="1" />
                                    </div>
                                </div>
                                <div class="col-sm-6" >
                                    <div class="form-group">
                                        <label class="control-label"><span class="asterisk" id="spIdentificacionObligatoria">*</span> Tipo Doc</label>
                                        <select id="ddlTipoDoc" class="form-control required" tabindex="2" onchange="changeTipoDoc();">
                                            <option value=""></option>
                                            <option value="DNI">DNI</option>
                                            <option value="CUIT" selected="selected" >CUIT</option>
                                            <option value="SIN CUIT" >SIN CUIT</option>
                                           </select>
                                    </div>
                                </div>                                
                            </div>
                            <div class="row mb15" id="divBuscarDatosAfip">
                                <div class="col-sm-3" >
                                    <div class="form-group">
                                        <input type="image" src="images/icoTraerDatosAfip.png" name="btnTraerDatosAfip" style="height:auto;max-width:100%" id="btnTraerDatosAfip" value="Traer datos AFIP" tabindex="3" onclick="consultarDatosAfip();"/>                                    
                                    </div>
                                </div>   
                                <div class="col-sm-9">
                                    <div class="form-group">
                                        <div id="divProcesando" hidden="hidden"><img src="images/loaders/loader1.gif" /></div>
                                    </div>
                                </div>                             
                            </div>
                            <div class="row mb15">
                                <div class="col-sm-6">
                                    <div class="form-group">
                                        <label class="control-label"><span class="asterisk">*</span> Razón Social</label>
                                        <input type="text" id="txtRazonSocial" name="txtRazonSocial" class="form-control required" maxlength="128" tabindex="4" onkeyup="remplazarCaracteresEspeciales('txtRazonSocial');"/>
                                    </div>
                                </div>
                                <div class="col-sm-6">
                                    <div class="form-group">
                                        <label class="control-label"><span class="asterisk">*</span> Categoría Impositiva</label>
                                        <select id="ddlCondicionIva" class="form-control required" tabindex="5" onchange="changeCondicionIvaNuevaPersona();">
                                            <option value=""></option>
                                            <option value="RI">Responsable Inscripto</option>
                                            <option value="CF">Consumidor Final</option>
                                            <option value="MO">Monotributista</option>
                                            <option value="EX">Exento</option>
                                        </select>
                                    </div>
                                </div>
                            </div>
                            <div class="row mb15" >
                                <div class="col-sm-6" id="divPersoneriaNuevaPersona">
                                    <div class="form-group">
                                        <label class="control-label"><span class="asterisk">*</span> Personería</label>
                                        <select id="ddlPersoneria" class="form-control" tabindex="6">
                                            <option value="F">Física</option>
                                            <option value="J">Jurídica</option>
                                        </select>
                                    </div>
                                </div>
                                <div class="col-sm-6">
                                    <div class="form-group" id="divFantasia">
                                        <label class="control-label">Nombre de Fantansía</label>
                                        <input id="txtNombreFantasia" type="text" class="form-control required" maxlength="128" tabindex="7" />
                                    </div>
                                </div>
                            </div>
                            <div class="row mb15">
                                <div class="col-sm-6">
                                    <div class="form-group">
                                        <label class="control-label"> Provincia</label>
                                        <select id="ddlProvincia" class="select2" data-placeholder="Selecciona una provincia..." tabindex="8" onchange="changeProvincia();">
                                        </select>
                                    </div>
                                </div>
                                <div class="col-sm-6">
                                    <div class="form-group">
                                        <label class="control-label"> Ciudad/Localidad</label>
                                        <select id="ddlCiudadCliente" class="select2" data-placeholder="Selecciona una ciudad..." tabindex="9">
                                        </select>
                                    </div>
                                </div>
                            </div>
                            <div class="row mb15">
                                <div class="col-sm-6">
                                    <div class="form-group">
                                        <label class="control-label"> Provincia</label>
                                        <input type="text" id="txtProvinciaDesc" class="form-control" maxlength="50"  tabindex="10"/>
                                    </div>
                                </div>
                                <div class="col-sm-6">
                                    <div class="form-group">
                                        <label class="control-label"> Ciudad/Localidad</label>
                                        <input type="text" id="txtCiudadDesc" class="form-control" maxlength="50"  tabindex="11"/>
                                    </div>
                                </div>
                            </div>
                            <div class="row mb15" id="divDomicilioNuevaPersona">
                                <div class="col-sm-6">
                                    <div class="form-group">
                                        <label class="control-label"> Domicilio</label>
                                        <input type="text" id="txtDomicilio" class="form-control" maxlength="100"  tabindex="12"/>
                                    </div>
                                </div>
                                <div class="col-sm-3">
                                    <div class="form-group">
                                        <label class="control-label">Piso/Depto</label>
                                        <input id="txtPisoDepto" type="text" class="form-control" maxlength="10"  tabindex="13"/>
                                    </div>
                                </div>
                                <div class="col-sm-3">
                                    <div class="form-group">
                                        <label class="control-label">Código Postal</label>
                                        <input id="txtCp" type="text" class="form-control" maxlength="10"  tabindex="14"/>
                                    </div>
                                </div>
                            </div>
                            <div class="row mb15">
                                <div class="col-sm-3">
                                    <div class="form-group">
                                        <label class="control-label">Código</label>
                                        <input id="txtCodigoPersonas" type="text" class="form-control" maxlength="10"  tabindex="15"/>
                                    </div>
                                </div>
                                <div class="col-sm-6">
                                    <div class="form-group">
                                        <label class="control-label"> Email</label>
                                          <input id="txtEmailNuevaPersona" type="text" class="form-control" maxlength="128" tabindex="16" />
                                    </div>
                                </div>
                            </div>
                        </div>
                        <div class="panel-footer">
                            <input type="button" class="btn btn-primary" value="Crear" onclick="crearPersona();"  tabindex="17"/>
                            <a style="margin-left:20px" href="#" onclick="$('#modalNuevoCliente').modal('toggle');" tabindex="18">Cancelar</a>
                        </div>
                    </div>
                </div>
                <input type="hidden" id="hdnNuevaPersonaTipo" value="C" />
            </form>
        </div>
    </div>
</div>

<script src="/js/views/nuevaPersona.js"></script>

<script>
    $(document).ajaxStart(function () {
        $("#btnTraerDatosAfip").attr("disabled", true);
        document.body.style.cursor = "wait";
        $("#divProcesando").show();
    }).ajaxStop(function () {
        $("#btnTraerDatosAfip").attr("disabled", false);
        document.body.style.cursor = "default";
        $("#divProcesando").hide();
    });
</script>