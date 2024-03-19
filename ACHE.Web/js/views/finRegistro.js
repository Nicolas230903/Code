var finRegistro = {
    grabar: function () {
        bootbox.dialog({
            message: "Bienvenido, usted podrá a partir de este momento acceder a las funciones básicas del sistema, cualquier ampliación que necesite para adaptarlo a su negocio, por favor envíe por correo a axan.sistemas@gmail.com",
            buttons: {
                success: {
                    label: "Aceptar",
                    className: "btn-success",
                    callback: function () {
                        finRegistro.guardar();
                    }
                },
                danger: {
                    label: "Cancelar",
                    className: "btn-default",
                    callback: function () {

                    }
                }
            }
        });
    },
    guardar: function () {
        if ($('#frmEdicion').valid()) {

            var esContador = ($("input[name='ctl00$MainContent$rEsContador']:checked").attr('value') == "1") ? true : false;
            var usaPlanCorporativo = ($("input[name='ctl00$MainContent$rUsaContabilidad']:checked").attr('value') == "1") ? true : false;
            Common.mostrarProcesando("btnActualizar");
            var info = "{ razonSocial: '" + $("#txtRazonSocial").val()
                    + "', condicionIva: '" + $("#ddlCondicionIva").val()
                    + "', personeria: '" + $("#ddlPersoneria").val()
                    + "', idProvincia: '" + $("#ddlProvincia").val()
                    + "', idCiudad: '" + $("#ddlCiudad").val()
                    + "', domicilio: '" + $("#txtDomicilio").val()
                    + "', pisoDepto: '" + $("#txtPisoDepto").val()
                    + "', cp: '" + $("#txtCp").val()
                    + "', esContador: " + esContador
                    + " , usaPlanCorporativo: " + usaPlanCorporativo
                    + " , fechaInicioActividades: '" + $("#hdnFechaInicioActividades").val()
                    + "' }";

            $.ajax({
                type: "POST",
                url: "/finRegistro.aspx/guardar",
                data: info,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data, text) {
                    consultarPuntosDeVentaAfip();
                    $('#msgOk').html('Bien hecho! Los datos se han actualizado correctamente.');
                    $('#divOk').show();
                    $("#divError").hide();
                    $('html, body').animate({ scrollTop: 0 }, 'slow');
                    window.location.href = "home.aspx";
                },
                error: function (response) {
                    var r = jQuery.parseJSON(response.responseText);
                    $("#msgError").html(r.Message);
                    $("#divError").show();
                    $("#divOk").hide();
                    $('html, body').animate({ scrollTop: 0 }, 'slow');
                    Common.ocultarProcesando("btnActualizar", "Finalizar");
                }
            });
        }
        else {
            return false;
        }
    },
    configForm: function () {
        $(".select2").select2({ width: '100%', allowClear: true });
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
        Common.obtenerProvincias("ddlProvincia", $("#hdnProvincia").val(), true);

        Common.configTelefono("txtTelefono");
        finRegistro.consultarDatosAfip();
    },
    changeProvincia: function () {
        Common.obtenerCiudades("ddlCiudad", $("#ddlCiudad").val(), $("#ddlProvincia").val(), true);               
    },
    changeCondicionIVA: function () {
        if ($("#ddlCondicionIva").val() != "RI") {
            $('#rUsaContabilidadNO').attr('checked', true);
        }
    },
    consultarDatosAfip: function () {

        var cuit = $("#hdnCUIT").val();
        
        $.ajax({
            type: "POST",
            url: "common.aspx/consultarDatosAfip",
            data: "{ cuit: " + cuit + "}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data, text) {
                if (data.d != null) {

                    if (data.d.Mensaje == null) {
                        if (data.d.CUIT != null) {
                            $("#msgOk").html('Encontramos datos en AFIP con el CUIT ingresado.');
                            $("#divOk").show();
                            $("#txtRazonSocial").val(data.d.RazonSocial);
                            $("#ddlCondicionIva").val(data.d.CategoriaImpositiva).trigger("change");                            
                            $("#ddlPersoneria").val(data.d.Personeria = 'FISICA' ? 'F' : 'J');
                            $("#txtDomicilio").val(data.d.DomicilioFiscalDomicilio);
                            $("#txtCp").val(data.d.DomicilioFiscalCP);
                            $("#ddlProvincia").val(data.d.IdProvincia).trigger("change");
                            $("#ddlCiudad").val(data.d.IdCiudad).trigger("change");
                            if (data.d.FechaInicioActividades != null) {
                                $("#hdnFechaInicioActividades").val(data.d.FechaInicioActividades);
                            }

                            $("#txtRazonSocial").css("background-color", "#e8ffe3");
                            $("#ddlCondicionIva").css("background-color", "#e8ffe3");
                            $("#ddlPersoneria").css("background-color", "#e8ffe3");
                            $("#txtDomicilio").css("background-color", "#e8ffe3");
                            $("#txtCp").css("background-color", "#e8ffe3");
                        } else {
                            $("#msgAdvertencia").html('No encontramos datos en AFIP con el CUIT ingresado.');
                            $("#divAdvertencia").show();
                        }
                    } else {
                        $("#msgAdvertencia").html(data.d.Mensaje);
                        $("#divAdvertencia").show();
                    }

                }
            },
            error: function (response) {
                var r = jQuery.parseJSON(response.responseText);
                $("#msgAdvertencia").html(r.Message);
                $("#divAdvertencia").show();
            }
        });
                
    }
}