var ListaDeCuentas = new Array();
var asientosManuales = {
    configForm: function () {
        Common.configDatePicker();
        $(".select2").select2({ width: '100%', allowClear: true, });
        $("#txtDebe,#txtHaber").maskMoney({ thousands: '', decimal: '.', allowZero: true });
        $("#noResultTemplate").tmpl({ results: ListaDeCuentas }).appendTo("#resultsContainer");

        asientosManuales.limpiarDatos();
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
        })
    },
    agregarAsiento: function () {
        var obj = new Object();

        if ($("#frmEdicion").valid()) {

            if (parseFloat($("#txtDebe").val()) > 0 && parseFloat($("#txtHaber").val()) > 0) {
                $("#msgError").html("Solo puede ingresar un valor.");
                $("#divError").show();
                return false;
            }
            if (parseFloat($("#txtDebe").val()) == 0 && parseFloat($("#txtHaber").val()) == 0) {
                $("#msgError").html("Ingrese un valor al debe o al haber.");
                $("#divError").show();
                return false;
            }

            obj.IDPlanDeCuenta = $("#ddlPlanDeCuentas").val();
            //obj.Codigo = $("#ddlPlanDeCuentas").val();
            obj.NombreCuenta = $("#ddlPlanDeCuentas option:selected").text();
            obj.Debe = $("#txtDebe").val();
            obj.Haber = $("#txtHaber").val();

            ListaDeCuentas.push(obj);

            $("#resultsContainer").html("");
            $("#resultTemplate").tmpl({ results: ListaDeCuentas }).appendTo("#resultsContainer");
            asientosManuales.limpiarDatos();        

		}
    },
    limpiarDatos: function () {
        $("#ddlPlanDeCuentas").val("");
        $("#txtDebe,#txtHaber").val("0.00");
        $("#ddlPlanDeCuentas").trigger("change");
        $("#divOk,#divError").hide();
    },
    guardar: function () {
        $("#divOk,#divError").hide();
        if (asientosManuales.validarPartidaDoble())
            return false;

        if (ListaDeCuentas.length < 1)
        {
            $("#msgError").html("Debe ingresar las cuentas contables");
            $("#divError").show();
            return false;
        }

        if ($('#txtFecha,#txtLeyenda').valid()) {
            Common.mostrarProcesando("btnActualizar");

            var info = "{ listaAsientos: " + JSON.stringify(ListaDeCuentas)
                    + " , fecha: '" + $("#txtFecha").val()
                    + "', leyenda: '" + $("#txtLeyenda").val()
                    + "', id: " + $("#hdnID").val()
                    + "}";
            $.ajax({
                type: "POST",
                url: "/modulos/contabilidad/asientosManuales.aspx/guardar",
                data: info,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data, text) {
                    Common.ocultarProcesando("btnActualizar", "Aceptar");
                    $("#divOk").show();
                    location.href = "/modulos/reportes/LibroDiario.aspx";
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
    },
    validarPartidaDoble: function () {

        var totalDebe = 0;
        var totalHaber = 0;

        for (var i = 0; i < ListaDeCuentas.length; i++) {
            totalDebe = totalDebe + parseFloat(ListaDeCuentas[i].Debe);
            totalHaber = totalHaber + parseFloat(ListaDeCuentas[i].Haber);
        }

        if (totalDebe != totalHaber) {
            $("#msgError").html("El debe y el haber no son iguales");
            $("#divError").show();
            return true;
        }
        else {
            return false;
        }
    },
    eliminar: function (idPlanDeCuenta) {
        ListaDeCuentas = ListaDeCuentas.filter(function (el) {
            return el.IDPlanDeCuenta != idPlanDeCuenta;
        });

        $("#resultsContainer").html("");
        if (ListaDeCuentas.length > 0)
            $("#resultTemplate").tmpl({ results: ListaDeCuentas }).appendTo("#resultsContainer");
        else
            $("#noResultTemplate").tmpl({ results: ListaDeCuentas }).appendTo("#resultsContainer");
    },
    obtenerAsientosManuales: function () {
        var info = "{ id: " + $("#hdnID").val() + "}";
        $.ajax({
            type: "POST",
            url: "/modulos/contabilidad/asientosManuales.aspx/obtenerAsientosManuales",
            data: info,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data, text) {

                $("#txtFecha").val(data.d.Fecha);
                $("#txtLeyenda").val(data.d.Leyenda);

                $("#resultsContainer").html("");
                $("#resultTemplate").tmpl({ results: data.d.items }).appendTo("#resultsContainer");

                ListaDeCuentas = data.d.items;
            },
            error: function (response) {
                var r = jQuery.parseJSON(response.responseText);
                $("#msgError").html(r.Message);
                $("#divError").show();
                $("#divOk").hide();
                $('html, body').animate({ scrollTop: 0 }, 'slow');
            }
        });
    }
}