function crearConcepto() {
    $("#divConceptoError").hide();

    var idTipo = ($("#ddlTipo").val() == "" || $("#ddlTipo").val() == null) ? 'P' : $("#ddlTipo").val();
    var idEstado = ($("#ddlEstado").val() == "" || $("#ddlEstado").val() == null) ? 'A' : $("#ddlEstado").val();

    if ($("#frmNuevoConcepto").valid()) {
        var obj = new Object();
        obj.id = 0;
        obj.nombre = $("#txtNombre").val();
        obj.codigo = $("#txtCodigo").val();
        obj.tipo = idTipo;
        obj.descripcion = '';
        obj.estado = idEstado;
        obj.precio = $("#txtPrecio").val();
        obj.iva = $("#ddlIva").val();
        obj.stock = 0;
        obj.obs = '';
        obj.constoInterno = $("#txtCostoInterno").val();
        obj.stockMinimo = '0';
        obj.idPersona = 0;     
        $.ajax({
            type: "POST",
            url: "conceptose.aspx/guardar",
            data: JSON.stringify(obj),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: false,
            success: function (data, text) {
                $("#txtAyudaMensaje").val("");
                $("#divAyudaError").hide();

                $('#modalNuevoConcepto').modal('hide');

                $("#txtNombre,#txtCodigo, #ddlTipo,#ddlEstado,#txtPrecio,#ddlIva,#txtCostoInterno").val("");

                if (data.d != 0)
                    Common.obtenerConceptos("ddlConcepto", 1, data.d, false);

                limpiarControles();
            },
            error: function (response) {
                var r = jQuery.parseJSON(response.responseText);
                $("#msgAyudaConcepto").html(r.Message);
                $("#divAyudaConcepto").show();
            }
        });
    }
    else
        return false;
}

function limpiarControles() {
    $("#txtNombre").val('');
    $("#txtCodigo").val('');
    $("#txtPrecio").val('1,00');
    $("#ddlIva").val('');
    $("#txtCostoInterno").val('1,00');
    $("#divAyudaConcepto").hide();
    $("#divOkConcepto").hide();
    $("#ddlEstado").val('A');
    $("#ddlTipo").val('P');
    $("#ddlIva").val('10,50');  
}

$(document).ready(function () {
    $("#txtPrecio,#txtCostoInterno").maskMoney({ thousands: '', decimal: ',', allowZero: true });

    if ($("#txtPrecio").val() != "") {
        var precio = parseFloat($("#txtPrecio").val());
        $("#txtPrecio").val(addSeparatorsNF(precio.toFixed(2), '.', ',', '.'));
    }

    if ($("#txtCostoInterno").val() != "") {
        var costoInterno = parseFloat($("#txtCostoInterno").val());
        $("#txtCostoInterno").val(addSeparatorsNF(costoInterno.toFixed(2), '.', ',', '.'));
    }

    // Validation with select boxes
    $("#frmNuevoConcepto").validate({
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

    $("#ddlIva").val('10,50');  
});
