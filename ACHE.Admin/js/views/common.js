/*** GLOBAL VARIABLES ***/
var PAGE_SIZE = 50;

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


/*** NUMBERS ***/
function addSeparatorsNF(nStr, inD, outD, sep) {
    nStr += '';
    var dpos = nStr.indexOf(inD);
    var nStrEnd = '';
    if (dpos != -1) {
        nStrEnd = outD + nStr.substring(dpos + 1, nStr.length);
        nStr = nStr.substring(0, dpos);
    }
    var rgx = /(\d+)(\d{3})/;
    while (rgx.test(nStr)) {
        nStr = nStr.replace(rgx, '$1' + sep + '$2');
    }
    return nStr + nStrEnd;
}

/*** DATES ***/
var MONTH_NAMES = ['Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio', 'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'];
var MONTH_NAMES_SHORT = ['ene', 'feb', 'mar', 'abr', 'may', 'jun', 'jul', 'ago', 'sep', 'oct', 'nov', 'dic'];

/*
Date.prototype.getMonthName = function () {
    return this.monthNames[this.getMonth()];
};
Date.prototype.getShortMonthName = function () {
    return this.getMonthName().substr(0, 3);
};
*/

Date.isLeapYear = function (year) {
    return (((year % 4 === 0) && (year % 100 !== 0)) || (year % 400 === 0));
};

Date.getDaysInMonth = function (year, month) {
    return [31, (Date.isLeapYear(year) ? 29 : 28), 31, 30, 31, 30, 31, 31, 30, 31, 30, 31][month];
};

Date.prototype.isLeapYear = function () {
    var y = this.getFullYear();
    return (((y % 4 === 0) && (y % 100 !== 0)) || (y % 400 === 0));
};

Date.prototype.getDaysInMonth = function () {
    return Date.getDaysInMonth(this.getFullYear(), this.getMonth());
};

Date.prototype.addMonths = function (value) {
    var n = this.getDate();
    this.setDate(1);
    this.setMonth(this.getMonth() + value);
    this.setDate(Math.min(n, this.getDaysInMonth()));
    return this;
};

function gd(year, month, day) {
    return new Date(year, month, day).getTime();
}

function convertToDate(timestamp) {
    var newDate = new Date(timestamp);
    var dateString = newDate.getMonth();
    var monthName = getMonthName(dateString);

    return monthName;
}



var Common = {
    configDatePicker: function () {
        $.datepicker.regional['es'] = {
            closeText: 'Cerrar',
            prevText: '<Ant',
            nextText: 'Sig>',
            currentText: 'Hoy',
            monthNames: ['Enero', 'Febrero', 'Marzo', 'Abril', 'Mayo', 'Junio', 'Julio', 'Agosto', 'Septiembre', 'Octubre', 'Noviembre', 'Diciembre'],
            monthNamesShort: ['Ene', 'Feb', 'Mar', 'Abr', 'May', 'Jun', 'Jul', 'Ago', 'Sep', 'Oct', 'Nov', 'Dic'],
            dayNames: ['Domingo', 'Lunes', 'Martes', 'Miércoles', 'Jueves', 'Viernes', 'Sábado'],
            dayNamesShort: ['Dom', 'Lun', 'Mar', 'Mié', 'Juv', 'Vie', 'Sáb'],
            dayNamesMin: ['Do', 'Lu', 'Ma', 'Mi', 'Ju', 'Vi', 'Sá'],
            weekHeader: 'Sm',
            dateFormat: 'dd/mm/yy',
            firstDay: 1,
            isRTL: false,
            showMonthAfterYear: false,
            yearSuffix: ''
        };
        $.datepicker.setDefaults($.datepicker.regional['es']);

        $('.validDate').datepicker({
            dateFormat: "dd/mm/yy",
            changeMonth: true,
            changeYear: true,
            showButtonPanel: true,
            maxDate: '0'
        });

        Common.configFechas();
    },
    configFechas: function (format) {
        $.validator.addMethod("validDate", function (value, element) {
            /*var bits = value.match(/([0-9]+)/gi), str;
            if (!bits) return this.optional(element) || false; str = bits[1] + '/' + bits[0] + '/' + bits[2];
            return this.optional(element) || !/Invalid|NaN/.test(new Date(str));*/

            var check = false;
            var re = /^\d{1,2}\/\d{1,2}\/\d{4}$/;
            if (format == 'mm/yy')
                re = /^\d{1,2}\/\d{4}$/;
            if (re.test(value)) {
                var adata = value.split('/');
                var dd = 0;
                var mm = 0;
                var yyyy = 1900;
                if (format != 'mm/yy') {
                    dd = parseInt(adata[0], 10);
                    mm = parseInt(adata[1], 10);
                    yyyy = parseInt(adata[2], 10);
                }
                else {
                    dd = 1;
                    mm = parseInt(adata[0], 10);
                    yyyy = parseInt(adata[1], 10);
                }

                var xdata = new Date(yyyy, mm - 1, dd);

                if ((xdata.getFullYear() == yyyy) && (xdata.getMonth() == mm - 1) && (xdata.getDate() == dd))
                    check = true;
                else
                    check = false;
            }
            else
                check = false;
            return this.optional(element) || check;

        }, "La fecha no es válida.");
    },
    configFechasDesdeHasta: function (date, end) {
        $.validator.addMethod("greaterThan", function () {
            var valid = true;
            var desde = $("#" + date).val();
            var hasta = $("#" + end).val();
            if (isNaN(hasta) && isNaN(desde)) {
                var fDesde = parseEnDate(desde);
                var fHasta = parseEnDate(hasta);
                if (fDesde > fHasta) {
                    valid = false;
                }
            }
            return valid;
        }, "La fecha desde es mayor a la fecha hasta");
        $('form').validate({
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

        //$("#" + date).datepicker({ dateFormat: 'dd/mm/yy' });
        //$("#" + end).datepicker({ dateFormat: 'dd/mm/yy' });

        $('#' + date).change(Common.cleanErrors);
        $('#' + end).change(Common.cleanErrors);
    },
    cleanErrors: function () {
        $('form').validate();
        if ($('form').valid()) {
            $('label .error').hide();
        }
    },
    mostrarProcesando: function (controlName) {
        $("#" + controlName).attr("disabled", true);
        $("#" + controlName).html("Procesando...");
        $("#" + controlName).val("Procesando...");
    },
    ocultarProcesando: function (controlName, texto) {
        $("#" + controlName).attr("disabled", false);
        $("#" + controlName).html(texto);
        $("#" + controlName).val(texto);
    },
    configTelefono: function (controlName) {
        $("#" + controlName).mask("?(9999) 9999-9999");
        $("#" + controlName).on("blur", function () {
            var last = $(this).val().substr($(this).val().indexOf("-") + 1);
            if (last.length == 3) {
                var move = $(this).val().substr($(this).val().indexOf("-") - 1, 1);
                var lastfour = move + last;
                var first = $(this).val().substr(0, 10);
                $(this).val(first + '-' + lastfour);
            }
        });
    },
}