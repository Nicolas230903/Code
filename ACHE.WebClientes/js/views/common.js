function cambiarPwd() {
    var isValid = true;
    var msg = "";

    var pwdOld = $("#txtPwdOld").val();
    var pwd = $("#txtPwdNew").val();
    var pwd2 = $("#txtPwdNew2").val();

    if (pwdOld == "") {
        isValid = false;
        msg = "Debe ingresar su actual contraseña";
    }
    else if (pwd == "") {
        isValid = false;
        msg = "Debe ingresar una nueva contraseña";
    }
    else if (pwd2 == "") {
        isValid = false;
        msg = "Debe confirmar la nueva contraseña";
    }
    else if (pwd != pwd2) {
        isValid = false;
        msg = "La nueva contraseña debe coincidir";
    }
    else if (pwdOld == pwd) {
        isValid = false;
        msg = "La nueva contraseña debe ser diferente de la actual";
    }

    if (!isValid) {
        $("#divOk").hide();
        $("#msgError").text(msg);
        $("#divError").show();
    }
    else {
        var info = "{ pwdOld: '" + pwdOld + "', pwd: '" + pwd + "' }";

        $.ajax({
            type: "POST",
            url: "/Account/CambiarPwd",
            data: info,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (result) {
                if (result == "true") {
                    $("#divError").hide();
                    $("#divOk").show();
                    $("#txtPwdOld").val("");
                    $("#txtPwdNew").val("");
                    $("#txtPwdNew2").val("");
                }
                else if (result == "false") {
                    $("#divOk").hide();
                    $("#msgError").html("Hubo un error, por favor intente nuevamente");
                    $("#divError").show();
                }
                else if (result == "incorrecta") {
                    $("#divOk").hide();
                    $("#msgError").html("Contraseña actual incorrecta");
                    $("#divError").show();
                }
                else if (result == "usuario") {
                    window.location.href = "/Account/Login";
                }
            },
            error: function () { }
        });
    }
}

function validarPwd(id) {
    var isValid = true;
    var msg = "";

    var pwd = $("#txtPwd").val();
    var pwd2 = $("#txtPwd2").val();

    if (pwd == "") {
        isValid = false;
        msg = "Debe ingresar una contraseña";
    }
    else if (pwd2 == "") {
        isValid = false;
        msg = "Debe confirmar la contraseña";
    }
    else if (pwd != pwd2) {
        isValid = false;
        msg = "La contraseña debe coincidir";
    }

    if (!isValid) {
        $("#msgError").text(msg);
        $("#divError").show();
    }
    else {
        var info = "{ pwd: '" + pwd + "' }";//', id: " + id + "

        $.ajax({
            type: "POST",
            url: "/Account/CrearLogin",
            data: info,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (result) {
                if (result == "false") {
                    $("#msgError").text("Hubo un error, por favor intente nuevamente");
                    $("#divError").show();
                }
                else if (result == "true") {
                    window.location.href = "/Home/Index";
                }
            },
            error: function () { }
        });
    }
}
