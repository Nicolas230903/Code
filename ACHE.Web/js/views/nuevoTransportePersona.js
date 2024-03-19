function crearTransportePersona() {
    $("#divAyudaTransportePersona").hide();

    //var idPersona;
    //if ($("#ddlPersona").val() != "") {
    //    $("#hdnNuevoDomicilioIdPersona").val($("#ddlPersona").val());
    //    idPersona = $("#ddlPersona").val();
    //}
    //if ($("#hdnID").val() != "") {
    //    $("#hdnNuevoDomicilioIdPersona").val($("#hdnID").val());
    //    idPersona = $("#hdnID").val();
    //}   

    if ($("#frmNuevoTransportePersona").valid()) {

        var idPersona = $("#hdnNuevoTransportePersonaIdUsuario").val();
        var razonSocial = $("#txtTransportePersonaRazonSocial").val();
        var provincia = ($("#ddlTransportePersonaProvincia").val() == "" || $("#ddlTransportePersonaProvincia").val() == null) ? "0" : $("#ddlTransportePersonaProvincia").val();
        var ciudad = ($("#ddlTransportePersonaCiudad").val() == "" || $("#ddlTransportePersonaCiudad").val() == null) ? "0" : $("#ddlTransportePersonaCiudad").val();
        var domicilio = $("#txtTransportePersonaDomicilio").val();
        var pisoDepto = $("#txtTransportePersonaPisoDepto").val();
        var codigoPostal = $("#txtTransportePersonaCp").val();
        var provinciaTexto = $("#txtTransportePersonaProvinciaTexto").val();
        var ciudadTexto = $("#txtTransportePersonaCiudadTexto").val();
        var contacto = $("#txtTransportePersonaContacto").val();
        var telefono = $("#txtTransportePersonaTelefono").val();

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
            url: "/common.aspx/CrearTransportePersona",
            data: info,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data, text) {
                $("#txtAyudaMensaje").val("");
                $("#divAyudaError").hide();

                $('#modalNuevoTransportePersona').modal('hide');

                $("#txtTransportePersonaRazonSocial,#txtTransportePersonaCiudad,#txtTransportePersonaDomicilio,#txtTransportePersonaPisoDepto").val("");
                $("#txtTransportePersonaCp,#txtTransportePersonaProvinciaTexto,#txtTransportePersonaCiudadTexto").val("");
                $("#txtTransportePersonaContacto,#txtTransportePersonaTelefono").val("");

                if (data.d != 0) {
                    Common.obtenerDomicilios('ddlTransportePersonaDomicilio', data.d, false, idPersona);
                }

                Common.obtenerProvincias("ddlTransportePersonaProvincia", "1", true);
                $("#ddlTransportePersonaCiudad").val("");
                $("#ddlTransportePersonaCiudad").trigger("change");
                limpiarControles();
            },
            error: function (response) {
                var r = jQuery.parseJSON(response.responseText);
                $("#msgAyudaTransportePersona").html(r.Message);
                $("#divAyudaTransportePersona").show();
            }
        });
    }
    else
        return false;
}
 
function changeTransportePersonaProvincia() {
    Common.obtenerCiudades("ddlTransportePersonaCiudad", "", $("#ddlTransportePersonaProvincia").val(), true);
    $("#ddlTransportePersonaCiudad").trigger("change");
}

function limpiarControles() {    
    $("#txtTransportePersonaDomicilio").css("background-color", "#ffffff");
    $("#txtTransportePersonaCp").css("background-color", "#ffffff");
    $("#txtTransportePersonaTransportePersona").val('');
    $("#txtTransportePersonaCp").val('');
    $("#txtTransportePersonaPisoDepto").val('');
    $("#txtTransportePersonaProvinciaTexto").val('');
    $("#txtTransportePersonaCiudadTexto").val('');
    $("#txtTransportePersonaContacto").val('');
    $("#txtTransportePersonaTelefono").val('');
    $("#divAyudaTransportePersona").hide();
    $("#divOkTransportePersona").hide();  
    Common.obtenerProvincias("ddlTransportePersonaProvincia", "1", true);
}

$(document).ready(function () {    
    
    // Validation with select boxes
    $("#frmNuevoTransportePersona").validate({
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

    $("#ddlTransportePersonaProvincia").html("");
    $("<option/>").attr("value", "0").text("Sin Cargar").appendTo($("#ddlTransportePersonaProvincia"));
    $("#ddlTransportePersonaProvincia").prop("disabled", true);
    $("#ddlTransportePersonaCiudad").html("");
    $("<option/>").attr("value", "24071").text("Sin Cargar").appendTo($("#ddlTransportePersonaCiudad"));
    $("#ddlTransportePersonaCiudad").prop("disabled", true);

   
});
