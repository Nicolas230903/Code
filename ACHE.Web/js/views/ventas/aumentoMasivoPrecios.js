var aumentoMasivoPrecio = {
    configFilters: function () {
        $("#txtPorcentaje").maskMoney({ thousands: '', decimal: '.', allowZero: true });
        $(".select2").select2({ width: '100%', allowClear: true });
        //Common.obtenerPersonas("ddlPersonas", "", true);
        $("#frmsearch").validate({
            highlight: function (element) {
                jquery(element).closest('.form-group').removeclass('has-success').addclass('has-error');
            },
            success: function (element) {
                jquery(element).closest('.form-group').removeclass('has-error');
            },
            errorelement: 'span',
            errorclass: 'help-block',
            errorplacement: function (error, element) {
                if (element.parent('.input-group').length) {
                    error.insertafter(element.parent());
                } else {
                    error.insertafter(element);
                }
            }
        });

        Common.obtenerListaPrecios("ddlListaPrecios", $("#hdnIDListaPrecio").val(), false);

    },
    grabar: function () {
        $("#divError").hide();
        if ($('#frmSearch').valid()) {
            Common.mostrarProcesando("btnActualizar");
            var idListaPrecios = ($("#ddlListaPrecios").val() == null || $("#ddlListaPrecios").val() == "") ? 0 : parseInt($("#ddlListaPrecios").val());
            var idPersona = ($("#ddlPersonas").val() == null || $("#ddlPersonas").val() == "") ? 0 : parseInt($("#ddlPersonas").val());

            var info = "{ idListaPrecios: " + idListaPrecios
                       + ", porcentaje: " + $("#txtPorcentaje").val()
                       + ", actualizarTodos: " + $("input[name='radioProductos']:checked").attr('value')
                       + ", idPersona: " + idPersona
                       + "}";

            $.ajax({
                type: "POST",
                url: "/modulos/ventas/aumentoMasivoPrecios.aspx/Guardar",
                data: info,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data, text) {
                    $('#divOk').show();
                    $("#divError").hide();
                    $('html, body').animate({ scrollTop: 0 }, 'slow');
                    Common.ocultarProcesando("btnActualizar", "Actualizar precios");
                },
                error: function (response) {
                    var r = jQuery.parseJSON(response.responseText);
                    $("#msgError").html(r.responseText);
                    $("#divError").show();
                    $("#divOk").hide();
                    $('html, body').animate({ scrollTop: 0 }, 'slow');
                    Common.ocultarProcesando("btnActualizar", "Actualizar precios");
                }
            });
        }
    },
    ocultarLista: function () {
        if ($("input[name='radioProductos']:checked").attr('value') == "1")
            $("#divListaPrecios").hide();
        else
            $("#divListaPrecios").show();
        $("#divPaso2,#btnActualizar").show();
    },
    ocultarPorcentaje: function () {
        if ($("input[name='radioPorcentaje']:checked").attr('value') == "1") {
            $("#divPorcentaje").show();
            $("#btnActualizar").text("Actualizar precios");
            $("#hdnporcentajePrecio").val("1");
        }
        else {
            $("#divPorcentaje").hide();
            $("#btnActualizar").text("Actualización masiva de precios");
            $("#hdnporcentajePrecio").val("0");
        }
    },
    actualizarDeFormaIndividual: function () {
        if ($("#divListaPrecios").is(":visible")) {

            if ($("#ddlListaPrecios").valid())
                window.location.href = "/importar.aspx?tipo=ListaPrecios&lista=" + $("#ddlListaPrecios").val();
        }
        else
            window.location.href = "/importar.aspx?tipo=ListaPrecios";
    },
    guardar: function () {
        if ($("#hdnporcentajePrecio").val() == 1)
            aumentoMasivoPrecio.grabar();
        else
            aumentoMasivoPrecio.actualizarDeFormaIndividual();
    },
}