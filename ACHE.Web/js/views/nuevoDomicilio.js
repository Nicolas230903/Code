function crearDomicilio() {
    $("#divAyudaDomicilio").hide();

    //var idPersona;
    //if ($("#ddlPersona").val() != "") {
    //    $("#hdnNuevoDomicilioIdPersona").val($("#ddlPersona").val());
    //    idPersona = $("#ddlPersona").val();
    //}
    //if ($("#hdnID").val() != "") {
    //    $("#hdnNuevoDomicilioIdPersona").val($("#hdnID").val());
    //    idPersona = $("#hdnID").val();
    //}   

    if ($("#frmNuevoDomicilio").valid()) {

        var idPersona = $("#hdnNuevoDomicilioIdPersona").val();
        var provincia = ($("#ddlDomicilioProvincia").val() == "" || $("#ddlDomicilioProvincia").val() == null) ? "0" : $("#ddlDomicilioProvincia").val();
        var ciudad = ($("#ddlDomicilioCiudad").val() == "" || $("#ddlDomicilioCiudad").val() == null) ? "0" : $("#ddlDomicilioCiudad").val();
        var domicilio = $("#txtDomicilioDomicilio").val();
        var pisoDepto = $("#txtDomicilioPisoDepto").val();
        var codigoPostal = $("#txtDomicilioCp").val();        
        var provinciaTexto = $("#txtDomicilioProvinciaTexto").val();
        var ciudadTexto = $("#txtDomicilioCiudadTexto").val();
        var contacto = $("#txtDomicilioContacto").val();
        var telefono = $("#txtDomicilioTelefono").val();

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
            + "' }";

        $.ajax({
            type: "POST",
            url: "/common.aspx/CrearDomicilio",
            data: info,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data, text) {
                $("#txtAyudaMensaje").val("");
                $("#divAyudaError").hide();

                $('#modalNuevoDomicilio').modal('hide');

                $("#txtDomicilioCiudad,#txtDomicilioDomicilio,#txtDomicilioPisoDepto").val("");
                $("#txtDomicilioCp,#txtDomicilioProvinciaTexto,#txtDomicilioCiudadTexto").val("");
                $("#txtDomicilioContacto,#txtDomicilioTelefono").val("");

                if (data.d != 0) {
                    Common.obtenerDomicilios('ddlDomicilioDomicilio', data.d, false, idPersona);
                }

                Common.obtenerProvincias("ddlDomicilioProvincia", "1", true);
                $("#ddlDomicilioCiudad").val("");
                $("#ddlDomicilioCiudad").trigger("change");
                limpiarControles();
            },
            error: function (response) {
                var r = jQuery.parseJSON(response.responseText);
                $("#msgAyudaDomicilio").html(r.Message);
                $("#divAyudaDomicilio").show();
            }
        });
    }
    else
        return false;
}
 
function changeDomicilioProvincia() {
    Common.obtenerCiudades("ddlDomicilioCiudad", "", $("#ddlDomicilioProvincia").val(), true);
    $("#ddlDomicilioCiudad").trigger("change");
}

function limpiarControles() {    
    $("#txtDomicilioDomicilio").css("background-color", "#ffffff");
    $("#txtDomicilioCp").css("background-color", "#ffffff");
    $("#txtDomicilioDomicilio").val('');
    $("#txtDomicilioCp").val('');
    $("#txtDomicilioPisoDepto").val('');
    $("#txtDomicilioProvinciaTexto").val('');
    $("#txtDomicilioCiudadTexto").val('');
    $("#txtDomicilioContacto").val('');
    $("#txtDomicilioTelefono").val('');
    $("#divAyudaDomicilio").hide();
    $("#divOkDomicilio").hide();  
    Common.obtenerProvincias("ddlDomicilioProvincia", "1", true);
}

$(document).ready(function () {    
    
    // Validation with select boxes
    $("#frmNuevoDomicilio").validate({
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

    $("#ddlDomicilioProvincia").html("");
    $("<option/>").attr("value", "0").text("Sin Cargar").appendTo($("#ddlDomicilioProvincia"));
    $("#ddlDomicilioProvincia").prop("disabled", true);
    $("#ddlDomicilioCiudad").html("");
    $("<option/>").attr("value", "24071").text("Sin Cargar").appendTo($("#ddlDomicilioCiudad"));
    $("#ddlDomicilioCiudad").prop("disabled", true);

   
});
