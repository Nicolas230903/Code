function crearPersona() {
    $("#divClienteError").hide();

    if ($("#ddlTipoDoc").val() != "SIN CUIT" && $("#ddlTipoDoc").val() != "DNI") {
        if ($("#txtNroDocumento").val() == "") {
            $("#msgAyudaCliente").html("Debe completar el campo número");
            $("#divAyudaCliente").show();
            return false;
        }
    }  

    if ($("#frmNuevaPersona").valid()) {

        var razonSocial = $("#txtRazonSocial").val();
        var condicionIva = $("#ddlCondicionIva").val();
        var personeria = $("#ddlPersoneria").val();
        var nombre = $("#txtNombreFantasia").val();
        var tipoDocumento = $("#ddlTipoDoc").val();
        var nroDocumento = $("#txtNroDocumento").val();
        var provincia = ($("#ddlProvincia").val() == "" || $("#ddlProvincia").val() == null) ? "0" : $("#ddlProvincia").val();
        var ciudad = ($("#ddlCiudadCliente").val() == "" || $("#ddlCiudadCliente").val() == null) ? "0" : $("#ddlCiudadCliente").val();
        var provinciaDesc = $("#txtProvinciaDesc").val();
        var ciudadDesc = $("#txtCiudadDesc").val();
        var domicilio = $("#txtDomicilio").val();
        var pisoDepto = $("#txtPisoDepto").val();
        var codigoPostal = $("#txtCp").val();
        var tipo = $("#hdnNuevaPersonaTipo").val();
        var codigo = $("#txtCodigoPersonas").val();
        var emailNuevaPersona = $("#txtEmailNuevaPersona").val();
        if ($("#ddlCondicionIva").val() =="CF")
        {
            personeria = "F";
            $("#ddlTipoDoc").val("DNI");
            $("#ddlTipoDoc").trigger("change");
            tipoDocumento = "DNI";
        }

        var info = "{ razonSocial: '" + razonSocial
            + "', condicionIva: '" + condicionIva
            + "', personeria: '" + personeria
            + "', nombreFantasia: '" + nombre
            + "', tipoDocumento: '" + tipoDocumento
            + "', nroDocumento: '" + nroDocumento
            + "', idProvincia: '" + provincia
            + "', idCiudad: '" + ciudad
            + "', provinciaDesc: '" + provinciaDesc
            + "', ciudadDesc: '" + ciudadDesc
            + "', domicilio: '" + domicilio
            + "', pisoDepto: '" + pisoDepto
            + "', codigoPostal: '" + codigoPostal
            + "', tipo: '" + tipo
            + "', codigo: '" + codigo
            + "', email: '" + emailNuevaPersona
            + "' }";

        $.ajax({
            type: "POST",
            url: "/common.aspx/CrearPersona",
            data: info,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data, text) {
                $("#txtAyudaMensaje").val("");
                $("#divAyudaError").hide();

                $('#modalNuevoCliente').modal('hide');

                $("#txtRazonSocial,#ddlCondicionIva, #txtNombreFantasia,#ddlTipoDoc,#txtNroDocumento,#txtCiudadCliente,#txtDomicilio,#txtPisoDepto,#txtCp,#txtCodigoPersonas,#txtEmailNuevaPersona").val("");

                if (data.d != 0) {
                    Common.obtenerPersonas('ddlPersona', data.d, false);
                }

                Common.obtenerProvincias("ddlProvincia", "1", true);
                $("#ddlCiudadCliente").val("");
                $("#ddlCiudadCliente").trigger("change");
                limpiarControles();
            },
            error: function (response) {
                var r = jQuery.parseJSON(response.responseText);
                $("#msgAyudaCliente").html(r.Message);
                $("#divAyudaCliente").show();
            }
        });
    }
    else
        return false;
}

function actualizarClientes() {
    //alert("RECARGAR CLIENTES");
    $.ajax({
        type: "GET",
        url: "common.aspx/ActualizarClientes",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data, text) {
            if (data.d != null) {
                for (var i = 0; u < data.d.length; i += 2) {
                    $("#ddlPersona").select2('data', { id: data.d[i], text: data.d[i + 1] });
                }
            }
        },
        error: function (response) {
            //var r = jQuery.parseJSON(response.responseText);
            //$("#msgAyudaCliente").html(r.Message);
            //$("#divAyudaCliente").show();
        }
    });
}

function changeProvincia() {
    Common.obtenerCiudades("ddlCiudadCliente", "", $("#ddlProvincia").val(), true);
    $("#ddlCiudadCliente").trigger("change");
}

function changeCondicionIvaNuevaPersona() {
    if ($("#ddlCondicionIva").val() == "CF") {
        $("#divPersoneriaNuevaPersona").hide();
        $("#spIdentificacionObligatoria,#spNuevaPersonaDomicilio").hide();
        //$("#ddlTipoDoc,#txtNroDocumento,#txtDomicilio").removeClass("required");
        //$("#ddlTipoDoc").val("DNI");
        //$("#ddlTipoDoc").trigger("change");
    }
    else {
        //$("#ddlTipoDoc").val("");
        //$("#ddlTipoDoc").trigger("change");
        $("#divPersoneriaNuevaPersona").show();
        $("#spIdentificacionObligatoria,#spNuevaPersonaDomicilio").show();
        //$("#ddlTipoDoc,#txtNroDocumento,#txtDomicilio").addClass("required");
    }
}

function changeTipoDoc() {
    //$("#txtNroDocumento").val("");       
}

function sugerirNumeroCuitGenerico() {
    $.ajax({
        type: "POST",
        url: "common.aspx/sugerirNumeroCuitGenerico",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data, text) {
            if (data != null) {
                $("#txtNroDocumento").val(data.d);                
            }
        },
        error: function (response) {
            var r = jQuery.parseJSON(response.responseText);
            $("#msgAyudaCliente").html(r.Message);
            $("#divAyudaCliente").show();
        }
    });
}

function consultarDatosAfip() {
    limpiarControles();
    //alert("RECARGAR CLIENTES");
    var cuit = $("#txtNroDocumento").val();
    var tipoDoc = $("#ddlTipoDoc").val();


    if (tipoDoc == 'CUIT') {
        if (cuit != '') {
            if (cuit.length == 11) {
                $.ajax({
                    type: "POST",
                    url: "common.aspx/consultarDatosAfip",
                    data: "{ cuit: " + cuit + "}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data, text) {
                        if (data.d != null) {

                            if (data.d.Mensaje == null) {
                                if (data.d.CUIT != null) {
                                    $("#msgOkCliente").html('Encontramos datos en AFIP con el CUIT ingresado.');
                                    $("#divOkCliente").show();
                                    $("#ddlTipoDoc").val('CUIT');
                                    $("#txtRazonSocial").val(data.d.RazonSocial);
                                    $("#ddlCondicionIva").val(data.d.CategoriaImpositiva);
                                    $("#ddlPersoneria").val(data.d.Personeria = 'FISICA' ? 'F' : 'J');
                                    $("#txtDomicilio").val(data.d.DomicilioFiscalDomicilio);
                                    $("#txtCp").val(data.d.DomicilioFiscalCP);
                                    $("#ddlProvincia").val(data.d.IdProvincia).trigger("change");
                                    $("#ddlCiudadCliente").val(data.d.IdCiudad).trigger("change");
                                    $("#txtProvinciaDesc").val(data.d.DomicilioFiscalProvincia);
                                    $("#txtCiudadDesc").val(data.d.DomicilioFiscalCiudad);
                                    //Common.obtenerProvincias("ddlProvincia", data.d.IdProvincia, true);
                                    //Common.obtenerCiudades("ddlCiudadCliente", data.d.IdCiudad, $("#ddlProvincia").val(), true);
                                    //$("#ddlCiudadCliente").trigger("change");
                                    $("#txtNroDocumento").css("background-color", "#e8ffe3");
                                    $("#ddlTipoDoc").css("background-color", "#e8ffe3");
                                    $("#txtRazonSocial").css("background-color", "#e8ffe3");
                                    $("#ddlCondicionIva").css("background-color", "#e8ffe3");
                                    $("#ddlPersoneria").css("background-color", "#e8ffe3");
                                    $("#txtDomicilio").css("background-color", "#e8ffe3");
                                    $("#txtProvinciaDesc").css("background-color", "#e8ffe3");
                                    $("#txtCiudadDesc").css("background-color", "#e8ffe3");
                                    $("#txtCp").css("background-color", "#e8ffe3");
                                } else {
                                    $("#msgAyudaCliente").html('No encontramos datos en AFIP con el CUIT ingresado.');
                                    $("#divAyudaCliente").show();
                                }
                            } else {
                                $("#msgAyudaCliente").html(data.d.Mensaje);
                                $("#divAyudaCliente").show();
                            }
                            
                        }
                    },
                    error: function (response) {
                        var r = jQuery.parseJSON(response.responseText);
                        $("#msgAyudaCliente").html(r.Message);
                        $("#divAyudaCliente").show();
                    }
                });
            } else {
                $("#msgAyudaCliente").html('CUIT Invalido.');
                $("#divAyudaCliente").show();
            }
        } else {
            $("#msgAyudaCliente").html('Debe completar el campo CUIT.');
            $("#divAyudaCliente").show();
        }
    } else {
        $("#msgAyudaCliente").html('Búsqueda solo por CUIT.');
        $("#divAyudaCliente").show();
    }

     
}

function limpiarControles() {
    $("#txtNroDocumento").css("background-color", "#ffffff");
    $("#ddlTipoDoc").css("background-color", "#ffffff");
    $("#txtRazonSocial").css("background-color", "#ffffff");
    $("#ddlCondicionIva").css("background-color", "#ffffff");
    $("#ddlPersoneria").css("background-color", "#ffffff");
    $("#txtDomicilio").css("background-color", "#ffffff");
    $("#txtProvinciaDesc").css("background-color", "#ffffff");
    $("#txtCiudadDesc").css("background-color", "#ffffff");
    $("#txtCp").css("background-color", "#ffffff");
    $("#txtRazonSocial").val('');
    $("#ddlCondicionIva").val('');
    $("#ddlPersoneria").val('');
    $("#txtDomicilio").val('');
    $("#txtProvinciaDesc").val('');
    $("#txtCiudadDesc").val('');
    $("#txtCp").val('');
    $("#txtPisoDepto").val('');
    $("#txtObservaciones").val('');
    $("#txtSaldo").val('');
    $("#txtTelefono").val('');
    $("#txtEmail").val('');
    $("#txtCodigo").val('');
    $("#txtNombreFantasia").val('');
    $("#divAyudaCliente").hide();
    $("#divOkCliente").hide();  
    Common.obtenerProvincias("ddlProvincia", "1", true);
}

$(document).ready(function () {    
    $("#txtNroDocumento").numericInput();

    /*$("#ddlCondicionIva").change(function () {
        if ($("#ddlCondicionIva").val() == "RI" || $("#ddlCondicionIva").val() == "EX")
            $("#divFantasia").show();
        else
            $("#divFantasia").hide();
    });*/

    $("#ddlCondicionIva").change();
    // Validation with select boxes
    $("#frmNuevaPersona").validate({
        highlight: function (element) {
            jQuery(element).closest('.form-group').removeClass('has-success').addClass('has-error');
        },
        success: function (element) {
            jQuery(element).closest('.form-group').removeClass('has-error');
        },
        errorElement: 'span',
        errorClass: 'help-block',
        errorPlacement: function (error, element) {
            if (element.parent('.input-group').length) {
                error.insertAfter(element.parent());
            } else {
                error.insertAfter(element);
            }
        }
    })

    //Common.obtenerProvincias("ddlProvincia", "1", true);
    //Common.obtenerProvinciaYCiudadGenerica("ddlProvincia", "ddlCiudadCliente", "1", "317", false, false);

    $("#ddlProvincia").html("");
    $("<option/>").attr("value", "0").text("Generico").appendTo($("#ddlProvincia"));
    $("#ddlProvincia").prop("disabled", true);
    $("#ddlCiudadCliente").html("");
    $("<option/>").attr("value", "24071").text("Generico").appendTo($("#ddlCiudadCliente"));
    $("#ddlCiudadCliente").prop("disabled", true);

    $.validator.addMethod("validCuit", function (value, element) {
        var check = true;
        if ($("#ddlTipoDoc").val() == "CUIT") {
            return CuitEsValido($("#txtNroDocumento").val());
        }
        else
            return check;

    }, "CUIT Inválido");
});
