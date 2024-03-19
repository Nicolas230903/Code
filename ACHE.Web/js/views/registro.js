function grabar() {
    $("#divError").hide();
    if ($('#frmRegistro').valid()) {
        if ($("#txtPwd").val() != $("#txtPwd2").val()) {
            $("#divError").html("Las contraseñas no coinciden");
            $("#divError").show();
        }
        else {
            if ($("#chkTyC").is(':checked')) {
                Common.mostrarProcesando("lnkRegistrarme");
                
                var info = "{ cuit: '" + $("#txtNroDocumento").val()
                        + "', email: '" + $("#txtEmail").val()
                        + "', pwd: '" + $("#txtPwd").val()
                        + "', telefono: '" + $("#txtTelefono").val()
                        + "', codigoPromocion: '" + $("#txtCodigoPromocion").val()
                        + "'}";

                $.ajax({
                    type: "POST",
                    url: "registro.aspx/guardar",
                    data: info,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data, text) {
                        //window.location.href = "mis-datos.aspx";
                        window.location.href = "finRegistro.aspx";
                    },
                    error: function (response) {
                        var r = jQuery.parseJSON(response.responseText);
                        $("#divError").html(r.Message);
                        $("#divError").show();
                        $('html, body').animate({ scrollTop: 0 }, 'slow');
                        Common.ocultarProcesando("lnkRegistrarme", "Registrarme gratis");
                    }
                });
            }
            else {
                $("#divError").html("Debe aceptar los terminos y condiciones");
                $("#divError").show();
            }
        }
    }
    else {
        return false;
    }
}

function configForm() {
    $("#txtNroDocumento").numericInput();

    //$('#txtTelefono,#txtNroDocumento').bind("cut copy paste", function (e) {
    //    e.preventDefault();
    //});

    // Validation with select boxes
    $("#frmRegistro").validate({
        onkeyup: false,
        highlight: function (element) {
            jQuery(element).closest('#divRegistroForm').removeClass('has-success').addClass('has-error');
        },
        success: function (element) {
            jQuery(element).closest('#divRegistroForm').removeClass('has-error');
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
    });
    Common.configTelefono("txtTelefono");
    $.validator.addMethod("validCuit", function (value, element) {
        var check = true;
        return CuitEsValido($("#txtNroDocumento").val());
    }, "CUIT Inválido");
    $.validator.addMethod("validPassword", function (value, element) {
        var check = true;
        return CheckPassword($("#txtPwd").val());
    }, "La clave de contener: Entre 8 y 12 caracteres, letras y números y al menos una mayúscula.");
    $.validator.addMethod("validPassword2", function (value, element) {
        var check = true;
        return CheckPassword($("#txtPwd2").val());
    }, "La clave de contener: Entre 8 y 12 caracteres, letras y números y al menos una mayúscula.");
}
jQuery(document).ready(function () {
    configForm();
});