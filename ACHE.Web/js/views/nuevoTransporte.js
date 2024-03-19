function crearTransporte() {
    $("#divAyudaTransporte").hide();

    //var idPersona;
    //if ($("#ddlPersona").val() != "") {
    //    $("#hdnNuevoDomicilioIdPersona").val($("#ddlPersona").val());
    //    idPersona = $("#ddlPersona").val();
    //}
    //if ($("#hdnID").val() != "") {
    //    $("#hdnNuevoDomicilioIdPersona").val($("#hdnID").val());
    //    idPersona = $("#hdnID").val();
    //}   

    if ($("#frmNuevoTransporte").valid()) {

        var idPersona = $("#hdnNuevoTransporteIdUsuario").val();
        var razonSocial = $("#txtTransporteRazonSocial").val();
        var provincia = ($("#ddlTransporteProvincia").val() == "" || $("#ddlTransporteProvincia").val() == null) ? "0" : $("#ddlTransporteProvincia").val();
        var ciudad = ($("#ddlTransporteCiudad").val() == "" || $("#ddlTransporteCiudad").val() == null) ? "0" : $("#ddlTransporteCiudad").val();
        var domicilio = $("#txtTransporteDomicilio").val();
        var pisoDepto = $("#txtTransportePisoDepto").val();
        var codigoPostal = $("#txtTransporteCp").val();
        var provinciaTexto = $("#txtTransporteProvinciaTexto").val();
        var ciudadTexto = $("#txtTransporteCiudadTexto").val();
        var contacto = $("#txtTransporteContacto").val();
        var telefono = $("#txtTransporteTelefono").val();

        var info = "{ idProvincia: '" + provincia
            + "', idCiudad: '" + ciudad
            + "', domicilio: '" + domicilio
            + "', pisoDepto: '" + pisoDepto
            + "', codigoPostal: '" + codigoPostal
            + "', idPersona: '" + idPersona
            + "', provinciaTexto: '" + provinciaTexto
            + "', ciudadTexto: '" + ciudadTexto
            + "', contacto: '" + contacto
            + "', telefono: '" + telefono
            + "', razonSocial: '" + razonSocial
            + "' }";

        $.ajax({
            type: "POST",
            url: "/common.aspx/CrearTransporte",
            data: info,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data, text) {
                $("#txtAyudaMensaje").val("");
                $("#divAyudaError").hide();

                $('#modalNuevoTransporte').modal('hide');

                $("#txtTransporteRazonSocial,#txtTransporteCiudad,#txtTransporteDomicilio,#txtTransportePisoDepto").val("");
                $("#txtTransporteCp,#txtTransporteProvinciaTexto,#txtTransporteCiudadTexto").val("");
                $("#txtTransporteContacto,#txtTransporteTelefono").val("");

                if (data.d != 0) {
                    Common.obtenerDomicilios('ddlTransporteDomicilio', data.d, false, idPersona);
                }

                Common.obtenerProvincias("ddlTransporteProvincia", "1", true);
                $("#ddlTransporteCiudad").val("");
                $("#ddlTransporteCiudad").trigger("change");
                limpiarControles();
            },
            error: function (response) {
                var r = jQuery.parseJSON(response.responseText);
                $("#msgAyudaTransporte").html(r.Message);
                $("#divAyudaTransporte").show();
            }
        });
    }
    else
        return false;
}
 
function changeTransporteProvincia() {
    Common.obtenerCiudades("ddlTransporteCiudad", "", $("#ddlTransporteProvincia").val(), true);
    $("#ddlTransporteCiudad").trigger("change");
}

function limpiarControles() {    
    $("#txtTransporteDomicilio").css("background-color", "#ffffff");
    $("#txtTransporteCp").css("background-color", "#ffffff");
    $("#txtTransporteTransporte").val('');
    $("#txtTransporteCp").val('');
    $("#txtTransportePisoDepto").val('');
    $("#txtTransporteProvinciaTexto").val('');
    $("#txtTransporteCiudadTexto").val('');
    $("#txtTransporteContacto").val('');
    $("#txtTransporteTelefono").val('');
    $("#divAyudaTransporte").hide();
    $("#divOkTransporte").hide();  
    Common.obtenerProvincias("ddlTransporteProvincia", "1", true);
}

$(document).ready(function () {    
    
    // Validation with select boxes
    $("#frmNuevoTransporte").validate({
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

    $("#ddlTransporteProvincia").html("");
    $("<option/>").attr("value", "0").text("Sin Cargar").appendTo($("#ddlTransporteProvincia"));
    $("#ddlTransporteProvincia").prop("disabled", true);
    $("#ddlTransporteCiudad").html("");
    $("<option/>").attr("value", "24071").text("Sin Cargar").appendTo($("#ddlTransporteCiudad"));
    $("#ddlTransporteCiudad").prop("disabled", true);

   
});
