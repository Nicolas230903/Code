var planDeCuentas = {
    ObtenerPlanDeCuentas: function () {
        $.jstree.destroy();
        $.ajax({
            type: "POST",
            url: "/modulos/contabilidad/planDecuentas.aspx/ObtenerPlanDeCuentas",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: false,
            success: function (data) {
                planDeCuentas.ArmarPlanDeCuenta(data);
            },
            error: function (response) {
                var r = jQuery.parseJSON(response.responseText);
            }
        });
    },
    ArmarPlanDeCuenta: function (data) {
        $('#divPlanDeCuenta').jstree({
            'core': {
                'data': data
            }
        });

        $('#divPlanDeCuenta').on("changed.jstree", function (e, data) {
            $("#hdnID").val(data.selected);
            $("#spnCuenta").html(data.node.text);
            planDeCuentas.obtenerCuenta();
        });

        $('#divPlanDeCuenta').on("open_node.jstree", function (e, data) {
            $("a.jstree-anchor").attr("style", "width: 100%;");
        });
        setTimeout(function () {
            $("a.jstree-anchor").attr("style", "width: 100%;");
        }, 1000);
    },
    obtenerCuenta: function () {
        $.ajax({
            type: "POST",
            data: "{ idPlanDeCuenta: " + $("#hdnID").val() + "}",
            url: "/modulos/contabilidad/planDecuentas.aspx/ObtenerCuenta",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                $("#txtCodigo").val(data.d.Codigo);
                $("#txtNombre").val(data.d.Nombre);
                $("#dllCuentas").val(data.d.IDPadre).trigger("change");
                $("#ddlTipoDeCuenta").val(data.d.TipoDeCuenta).trigger("change");

                $("#ddlAdminiteAsientoManual").val(data.d.AdminiteAsientoManual).trigger("change");
                planDeCuentas.changePlanDeCuentas();
            }
        });
    },

    grabar: function () {
        $("#divError,#divOk").hide();

        var id = ($("#hdnID").val() == "" || $("#hdnID").val() == null) ? "0" : $("#hdnID").val();
        var idPadre = ($("#dllCuentas").val() == "" || $("#dllCuentas").val() == null) ? "0" : $("#dllCuentas").val();

        if ($('#dllCuentas').valid() && $('#txtNombre').valid() && $('#txtCodigo').valid() && $('#ddlAdminiteAsientoManual').valid()) {
            Common.mostrarProcesando("btnActualizar");
            var info = "{ id: " + parseInt(id)
                    + " , codigo: '" + $("#txtCodigo").val()
                    + "', nombre: '" + $("#txtNombre").val()
                    + "', idPadre: " + idPadre
                    + " , adminiteAsientoManual: '" + $("#ddlAdminiteAsientoManual").val()
                    + "', tipoDeCuenta: '" + $("#ddlTipoDeCuenta").val()
                    + "'}";

            $.ajax({
                type: "POST",
                url: "/modulos/contabilidad/planDecuentas.aspx/Guardar",
                data: info,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data, text) {
                    planDeCuentas.ObtenerPlanDeCuentas();
                    Common.obtenerSelectPlanCuentas("dllCuentas", "", true);
                    planDeCuentas.limpiarDatos();
                    $('#divOk').show();
                    $("#divError").hide();
                    $('html, body').animate({ scrollTop: 0 }, 'slow');
                    Common.ocultarProcesando("btnActualizar", "Aceptar");
                    $("#modalPlanDeCuentas").modal("toggle");
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
    eliminar: function () {
        if ($("#hdnID").val() == "0") {
            $("#msgErrorPlan").html("debe seleccionar una cuenta primero");
            $("#divErrorPlan").show();
            setTimeout(function () {
                $("#divErrorPlan").hide();
            }, 10000);
        }
        else {
            $("#divErrorPlan").hide();
            bootbox.confirm("¿Está seguro que desea eliminar la cuenta " + $("#txtNombre").val() + "?", function (result) {
                if (result) {
                    $.ajax({
                        type: "POST",
                        url: "/modulos/contabilidad/planDecuentas.aspx/delete",
                        data: "{ id: " + $("#hdnID").val() + "}",
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (data, text) {
                            planDeCuentas.ObtenerPlanDeCuentas();
                            $("#divOkPlan").show();
                            setTimeout(function () {

                                $("#divOkPlan").hide();
                            }, 10000);
                        },
                        error: function (response, asd, asda) {
                            var r = jQuery.parseJSON(response.responseText);
                            $("#msgErrorPlan").html(r.Message);
                            $("#divErrorPlan").show();
                            $('html, body').animate({ scrollTop: 0 }, 'slow');
                            setTimeout(function () {

                                $("#divErrorPlan").hide();
                            }, 10000);

                        }
                    });
                }
            });
        }
    },
    nuevo: function () {
        var idCta = $("#hdnID").val();

        $("#divError,#divOk").hide();
        planDeCuentas.limpiarDatos();

        $("#dllCuentas").val(idCta);
        $("#dllCuentas").trigger("change");

        $("#modalPlanDeCuentas").modal("toggle");
    },
    cancelar: function () {
        $("#modalPlanDeCuentas").modal("toggle");
        planDeCuentas.limpiarDatos();
    },
    modificar: function () {
        if ($("#hdnID").val() == "0") {
            $("#msgErrorPlan").html("debe seleccionar una cuenta primero");
            $("#divErrorPlan").show();
            setTimeout(function () {
                $("#divErrorPlan").hide();
            }, 10000);
        }
        else {
            $("#divError,#divOk,#divErrorPlan,#divOkPlan").hide();
            $("#modalPlanDeCuentas").modal("toggle");
        }
    },
    limpiarDatos: function () {
        $("#txtCodigo,#txtNombre,#hdnID,#dllCuentas,#ddlAdminiteAsientoManual").val("");
        $("#spnCuenta").html("");
        $("#dllCuentas,#ddlAdminiteAsientoManual").trigger("change");
    },
    cierreContable: function () {
        $.ajax({
            type: "POST",
            url: "/modulos/contabilidad/planDecuentas.aspx/CierreContable",
            //data: "{ id: " + $("#hdnID").val() + "}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data, text) {
                $("#divOkPlan").show();
                setTimeout(function () {
                    $("#divOkPlan").hide();
                }, 10000);
            },
            error: function (response) {
                var r = jQuery.parseJSON(response.responseText);
                $("#msgErrorPlan").html(r.Message);
                $("#divErrorPlan").show();
                $('html, body').animate({ scrollTop: 0 }, 'slow');
                setTimeout(function () {
                    $("#divErrorPlan").hide();
                }, 10000);

            }
        });
    },
    config: function () {
        planDeCuentas.ObtenerPlanDeCuentas();
        Common.obtenerSelectPlanCuentas("dllCuentas", "", true);
        $(".select2").select2({
            width: '100%', allowClear: true
        });

        setTimeout(function () {
            $("a.jstree-anchor").attr("style", "width: 100%;");
        }, 1000);

        //$("#txtCodigo").numericInput();
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
    changePlanDeCuentas: function () {

        if ($("#dllCuentas").val() == "" || $("#dllCuentas").val() == null) {
            $("#divTipoDeCuenta").show();
        }
        else {
            $("#divTipoDeCuenta").hide();
        }
    }
}