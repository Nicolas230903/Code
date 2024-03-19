/*** FORM ***/
var CambiarPwd = {
    configForm: function () {
        // Validation with select boxes
        $("#frmEdicion").validate({
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
        });
        $.validator.addMethod("validPassword", function (value, element) {
            var check = true;
            return CheckPassword($("#txtPwd").val());
        }, "La clave de contener: Entre 8 y 12 caracteres, letras y números y al menos una mayúscula.");
        $.validator.addMethod("validPassword2", function (value, element) {
            var check = true;
            return CheckPassword($("#txtPwd2").val());
        }, "La clave de contener: Entre 8 y 12 caracteres, letras y números y al menos una mayúscula.");
    },
    grabar: function () {
        $("#divError").hide();
        $("#divOk").hide();

        if ($("#txtPwd").val() != $("#txtPwd2").val()) {
            $("#msgError").html("Las contraseñas nuevas no coinciden");
            $("#divError").show();
            $('html, body').animate({ scrollTop: 0 }, 'slow');
            return false;
        }
        else {

            if ($('#frmEdicion').valid()) {
                Common.mostrarProcesando("btnActualizar");
                var info = "{ actual: '" + $("#txtActual").val() + "', nueva: '" + $("#txtPwd").val() + "'}";

                $.ajax({
                    type: "POST",
                    url: "/modulos/seguridad/cambiar-pwd.aspx/guardar",
                    data: info,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data, text) {
                        $('#divOk').show();
                        $("#divError").hide();
                        $('html, body').animate({ scrollTop: 0 }, 'slow');
                        Common.ocultarProcesando("btnActualizar", "Aceptar");
                        window.location.href = "/modulos/seguridad/mis-datos.aspx";
                        $("#txtActual, #txtPwd, #txtPwd2").val("");
                    },
                    error: function (response) {
                        var r = jQuery.parseJSON(response.responseText);
                        $("#msgError").html(r.Message);
                        $("#divError").show();
                        $("#divOk").hide();
                        $('html, body').animate({ scrollTop: 0 }, 'slow');
                        Common.ocultarProcesando("btnActualizar", "Aceptar");
                    }
                });
            }
            else {
                return false;
            }
        }
    },
    cancelar: function () {
        window.location.href = "/modulos/seguridad/mis-datos.aspx";
    }
}

jQuery(document).ready(function () {
    CambiarPwd.configForm();
});