var Acceso = {
    configForm: function () {

        if ($("#tieneDatos").val() == "1")
            Acceso.login();

        $("#txtUsuario, #txtPwd").keypress(function (event) {
            var keycode = (event.keyCode ? event.keyCode : event.which);
            if (keycode == '13') {
                Acceso.login();
                return false;
            }
        });

        $("#txtEmail").keypress(function (event) {
            var keycode = (event.keyCode ? event.keyCode : event.which);
            if (keycode == '13') {
                Acceso.recuperar();
                return false;
            }
        });


        $("#frmLogin").validate({
            onkeyup: false,
            highlight: function (element) {
                jQuery(element).closest('#divLoginForm').removeClass('has-success').addClass('has-error');
            },
            success: function (element) {
                jQuery(element).closest('#divLoginForm').removeClass('has-error');
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



        $("#frmRecupero").validate({
            highlight: function (element) {
                jQuery(element).closest('#divRecuperoForm').removeClass('has-success').addClass('has-error');
            },
            success: function (element) {
                jQuery(element).closest('#divRecuperoForm').removeClass('has-error');
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
    login: function () {
        Acceso.ocultarErrores();

        if ($('#frmLogin').valid()) {
            $("#divLoginForm").hide();
            $("#divLoginLoading").show();

            var info = "{ usuario: '" + $("#txtUsuario").val() + "', pwd: '" + $("#txtPwd").val() + "', recordarme: '" + $("#chkRecordarme").is(':checked') + "'}";

            $.ajax({
                type: "POST",
                url: "common.aspx/ingreso",
                data: info,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data, text) {
                    //alert(data.d);
                    //MI_CONDICION = data.d;
                    // window.location.href = "/home.aspx";
                    Acceso.showWizard();
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
    },
    recuperar: function () {
        Acceso.ocultarErrores();

        if ($('#frmRecupero').valid()) {
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
    },
    showWizard: function () {

        $.ajax({
            type: "GET",
            url: "common.aspx/verificarWizard",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data, text) {

                if (data.d == true) {
                    window.location.href = "/home.aspx";
                }
                else {
                    window.location.href = "/finRegistro.aspx";
                }
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
}

$(document).ready(function () {
    Acceso.configForm();
});