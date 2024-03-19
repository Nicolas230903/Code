var Acceso = {
    configForm: function () {
        $("#txtDocumentoRecuperar").keypress(function (event) {
            var keycode = (event.keyCode ? event.keyCode : event.which);
            if (keycode == '13') {
                recuperar();
                return false;
            }
        });

        $(".numeric").numericInput();

        // Validation with select boxes
        $("#frmLogin").validate({
            highlight: function (element, errorClass, validClass) {
                if (element.type === 'radio') {
                    this.findByName(element.name).addClass(errorClass).removeClass(validClass);
                } else {
                    $(element).addClass(errorClass).removeClass(validClass);
                    $(element).closest('.control-group').removeClass('success').addClass('error');
                }
            },
            unhighlight: function (element, errorClass, validClass) {
                if (element.type === 'radio') {
                    this.findByName(element.name).removeClass(errorClass).addClass(validClass);
                } else {
                    $(element).removeClass(errorClass).addClass(validClass);
                    $(element).closest('.control-group').removeClass('error').addClass('success');
                }
            }
        });
    },
    recuperar: function () {
        Acceso.ocultarErrores();

        var isValid = true;
        var msg = "";

        var tipo = $("#cmbTipo").val();
        var documento = $("#txtDocumentoRecuperar").val();

        if (documento == "") {
            isValid = false;
            msg = "Debe ingresar su documento";
        }

        if (!isValid) {
            $("#divError2").html(msg);
            $("#divError2").show();
        }
        else {

            //if ($('#frmRecupero').valid()) {
            var info = "{ documento: '" + documento + "', tipo: '" + tipo + "' }";

            $("#divRecuperoLoading").show();
            $("#divRecuperoForm, #divRecuperoFin").hide();

            $.ajax({
                type: "POST",
                url: "/Account/RecuperarPwd",
                data: info,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (result) {
                    if (result == "true") {
                        $("#txtDocumentoRecuperar").val("");
                        $("#lnkVolver").click();
                        $("#divRecuperoFin").show();
                        $("#divRecuperoLoading, #divRecuperoForm").hide();
                    }
                    else if (result == "false") {
                        //$("#lblError").html("Hubo un error, por favor intente nuevamente");
                        //$("#divError2").show();
                        $("#divRecuperoLoading").hide();
                        $("#divError2").html("Hubo un error, por favor intente nuevamente");
                        $("#divError2").show();
                        $("#divRecuperoForm").show();
                    }
                    else if (result == "pwd") {
                        //$("#lblError").html("Tipo y/o número de documento incorrecto");
                        //$("#divError2").show();
                        $("#divRecuperoLoading").hide();
                        $("#divError2").html("Tipo y/o número de documento incorrecto");
                        $("#divError2").show();
                        $("#divRecuperoForm").show();
                    }
                },
                error: function () { }
            });
        }
    },
    ocultarErrores: function () {
        $("#divError").hide();
        $("#divError2").hide();
    },
    showForm: function (tipo) {
        Acceso.ocultarErrores();

        if (tipo == "login") {
            $("#divRecupero").hide();
            $("#divLogin").fadeIn(1500);
        }
        else {
            $("#divLogin").hide();

            $("#divRecuperoForm").show();
            $("#divRecuperoLoading, #divRecuperoFin").hide();

            $("#divRecupero").fadeIn(1500);
        }
    }
}

$(document).ready(function () {
    Acceso.configForm();
});

/*login: function () {
    Acceso.ocultarErrores();

    if ($('#frmLogin').valid()) {
        $("#divLoginForm").hide();
        $("#divLoginLoading").show();

        var info = "{ usuario: '" + $("#txtUsuario").val() + "', pwd: '" + $("#txtPwd").val() + "', recordarme: '" + $("#chkRecordarme").is(':checked') + "'}";

        $.ajax({
            type: "POST",
            url: "login.aspx/ingreso",
            data: info,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data, text) {
                //alert(data.d);
                //MI_CONDICION = data.d;
                window.location.href = "/home.aspx";
            },
            error: function (response) {
                var r = jQuery.parseJSON(response.responseText);
                $("#divError").html(r.Message);
                $("#divError").show();

                $("#divLoginForm").show();
                $("#divLoginLoading").hide();
            }
        });
    }
    else {
        $("#divLoginForm").show();
        $("#divLoginLoading").hide();

        return false;
    }
},*/

/*if ($('#frmRecupero').valid()) {
                var info = "{ email: '" + $("#txtEmail").val() + "'}";

                $("#divRecuperoLoading").show();
                $("#divRecuperoForm, #divRecuperoFin").hide();

                $.ajax({
                    type: "POST",
                    url: "login.aspx/recuperar",
                    data: info,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data, text) {
                        $("#txtEmail").val("");
                        $("#lnkVolver").click();

                        $("#divRecuperoFin").show();
                        $("#divRecuperoLoading, #divRecuperoForm").hide();
                    },
                    error: function (response) {
                        var r = jQuery.parseJSON(response.responseText);
                        //$("#divTituloRecuperarDatos").hide();
                        $("#divError2").html(r.Message);
                        $("#divError2").show();

                        $("#divRecuperoForm").show();
                        $("#divRecuperoLoading, #divRecuperoFin").hide();
                    }
                });
            }
            else {
                $("#divRecuperoForm").show();
                $("#divRecuperoLoading, #divRecuperoFin").hide();

                return false;
            }
}*/