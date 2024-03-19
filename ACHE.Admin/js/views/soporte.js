var soporte = {
    recuperarComprobante: function () {
        $("#divError,#divOk").hide();
        if ($('#txtNroComprobante,#txtCuitUsuario,#txtPunto,#ddlTipoComprobante').valid()) {
            Common.mostrarProcesando("btnActualizar");
            $("#divError").hide();
            var info = "{ nroComprobante: '" + $("#txtNroComprobante").val()
                     + "', cuitUsuario: '" + $("#txtCuitUsuario").val()
                     + "', punto: " + $("#txtPunto").val()
                     + ", tipoComprobante: '" + $("#ddlTipoComprobante").val()
                     + "' }";

            $.ajax({
                type: "POST",
                url: "/Soporte/RecuperarComprobante",
                data: info,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    Common.ocultarProcesando("btnActualizar", "Actualizar");
                    if (data.TieneError) {
                        $("#msgError").html(data.Mensaje);
                        $("#divError").show();
                    }
                    else {
                        $("#divOk").show();
                    }
                }
            });
        }
        else {
            return false;
        }
    },
    config: function () {
        $("#txtNroComprobante,#txtCuitUsuario,#txtPunto").numericInput();
        $(".select2").select2({ width: '100%', allowClear: true, });
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
    },
}