/*** GLOBAL VARIABLES ***/
var PAGE_SIZE = 50;

/*** METHODS ***/

function enviarAyuda() {
    $("#divAyudaError").hide();

    if ($("#txtAyudaMensaje").val() == "") {
        $("#msgAyudaError").html("Por favor, describenos tu consulta y/o problema");
        $("#divAyudaError").show();
        $('html, body').animate({ scrollTop: 0 }, 'slow');
        return false;
    }
    else {

        var info = "{ mensaje: '" + $("#txtAyudaMensaje").val() + "'}";

        $.ajax({
            type: "POST",
            url: "common.aspx/ayuda",
            data: info,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data, text) {
                $("#txtAyudaMensaje").val("");
                $("#divAyudaError").hide();

                $('#modalAyuda').modal('hide');
            },
            error: function (response) {
                var r = jQuery.parseJSON(response.responseText);
                $("#msgAyudaError").html(r.Message);
                $("#divAyudaError").show();
                $('html, body').animate({ scrollTop: 0 }, 'slow');
            }
        });
    }
}

function consultarPuntosDeVentaAfip() {
    $.ajax({
        type: "POST",
        url: "/common.aspx/consultarPuntosDeVentaAfip",
        //data: info,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data, text) {
        },
        error: function (response) {
        }
    });
}

function getParameterByName(name) {
    name = name.replace(/[\[]/, "\\[").replace(/[\]]/, "\\]");
    var regex = new RegExp("[\\?&]" + name + "=([^&#]*)"),
        results = regex.exec(location.search);
    return results === null ? "" : decodeURIComponent(results[1].replace(/\+/g, " "));
}

function marcarComunicacionesAfipVistas() {
    $("#divAyudaComunicacionesAfipError").hide();

    var table = document.getElementById('tablaComunicacionesAfip');
    var info = "{ tabla: " + table + "}";

    $.ajax({
        type: "POST",
        url: "common.aspx/marcarComunicacionesAfipVistas",
        data: info,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data, text) {
            $("#divAyudaComunicacionesAfipError").hide();
            $('#modalComunicacionesAfip').modal('hide');
        },
        error: function (response) {
            var r = jQuery.parseJSON(response.responseText);
            $("#msgAyudaComunicacionesAfipError").html(r.Message);
            $("#divAyudaComunicacionesAfipError").show();
            $('html, body').animate({ scrollTop: 0 }, 'slow');
        }
    });
    
} 

function obtenerPuntosDeVenta(controlName) {
    $.ajax({
        type: "GET",
        url: "/common.aspx/obtenerPuntosDeVenta",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            if (data != null) {
                for (var i = 0; i < data.d.length; i++) {
                    $("<option/>").attr("value", data.d[i].ID).text(data.d[i].Nombre).appendTo($("#" + controlName));
                }
            }
        }
    });
}

function obtenerActividades(controlName, idSelected, showEmpty) {
    $.ajax({
        type: "GET",
        url: "/common.aspx/obtenerActividades",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            if (data != null) {

                $("#" + controlName).html("");

                if (showEmpty)
                    $("<option/>").attr("value", "").text("").appendTo($("#" + controlName));

                for (var i = 0; i < data.d.length; i++) {
                    $("<option/>").attr("value", data.d[i].ID).text(data.d[i].Nombre).appendTo($("#" + controlName));
                }

                if (idSelected != "")
                    $("#" + controlName).val(idSelected);

                $("#" + controlName).trigger("change");
            }
        }
    });
}

function obtenerTipoComprobanteAfip(controlName, idSelected, showEmpty) {
    $.ajax({
        type: "GET",
        url: "/common.aspx/obtenerTipoComprobanteAfip",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            if (data != null) {

                $("#" + controlName).html("");

                if (showEmpty)
                    $("<option/>").attr("value", "").text("").appendTo($("#" + controlName));

                for (var i = 0; i < data.d.length; i++) {
                    $("<option/>").attr("value", data.d[i].ID).text(data.d[i].Nombre).appendTo($("#" + controlName));
                }

                if (idSelected != "")
                    $("#" + controlName).val(idSelected);

                $("#" + controlName).trigger("change");
            }
        }
    });
}

function obtenerPuntosDeVentaYNroCobranza(controlName) {
    $.ajax({
        type: "GET",
        url: "/common.aspx/obtenerPuntosDeVenta",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            if (data != null) {
                for (var i = 0; i < data.d.length; i++) {
                    $("<option/>").attr("value", data.d[i].ID).text(data.d[i].Nombre).appendTo($("#" + controlName));
                }

                Common.obtenerUltimoNroRecibo("txtNumero", $("#ddlTipo").val(), parseInt($("#ddlPuntoVenta").val()));
            }
        }
    });
}

function obtenerCategorias(controlName, showEmpty) {
    $.ajax({
        type: "GET",
        url: "/common.aspx/obtenerCategorias",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            if (data != null) {
                if (showEmpty)
                    $("<option/>").attr("value", "").text("").appendTo($("#" + controlName));

                for (var i = 0; i < data.d.length; i++) {
                    $("<option/>").attr("value", data.d[i].ID).text(data.d[i].Nombre).appendTo($("#" + controlName));
                }

                $("#" + controlName).trigger("change");
            }
        }
    });
}

function obtenerConceptos(controlName, tipo, showEmpty) {

    var aux = "";
    if (tipo == 1)
        aux = "P";
    else if (tipo == 2)
        aux = "S";

    $("#" + controlName).html("");

    $.ajax({
        type: "POST",
        url: "/common.aspx/obtenerConceptos",
        data: "{tipo: '" + aux + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            if (data != null) {
                if (showEmpty)
                    $("<option/>").attr("value", "").text("-").appendTo($("#" + controlName));

                for (var i = 0; i < data.d.length; i++) {
                    $("<option/>").attr("value", data.d[i].ID).text(data.d[i].Nombre).appendTo($("#" + controlName));
                }

                $("#" + controlName).trigger("change");
            }
            else {
                $("<option/>").attr("value", "").text("-").appendTo($("#" + controlName));
                $("#" + controlName).trigger("change");
            }
        }
    });
}

function obtenerPersonas(controlName, idSelected, showEmpty) {

    $.ajax({
        type: "GET",
        url: "/common.aspx/obtenerPersonas",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            if (data != null) {
                $("#" + controlName).html("");

                if (showEmpty)
                    $("<option/>").attr("value", "").text("").appendTo($("#" + controlName));

                for (var i = 0; i < data.d.length; i++) {
                    $("<option/>").attr("value", data.d[i].ID).text(data.d[i].Nombre).appendTo($("#" + controlName));
                }

                if (idSelected != "")
                    $("#" + controlName).val(idSelected);

                //$("#" + controlName).trigger("change");
                $("#" + controlName).trigger("change");
                //$("#" + controlName).select2({width: '100%'});
            }
        }
    });
}

function obtenerClientes(controlName, idSelected, showEmpty) {

    $.ajax({
        type: "GET",
        url: "/common.aspx/obtenerClientes",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            if (data != null) {
                $("#" + controlName).html("");

                if (showEmpty)
                    $("<option/>").attr("value", "").text("").appendTo($("#" + controlName));

                for (var i = 0; i < data.d.length; i++) {
                    $("<option/>").attr("value", data.d[i].ID).text(data.d[i].Nombre).appendTo($("#" + controlName));
                }

                if (idSelected != "")
                    $("#" + controlName).val(idSelected);

                //$("#" + controlName).trigger("change");
                $("#" + controlName).trigger("change");
                //$("#" + controlName).select2({width: '100%'});
            }
        }
    });
}

function obtenerPeriodos(controlName, idSelected, showEmpty) {

    $.ajax({
        type: "GET",
        url: "/common.aspx/obtenerPeriodos",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            if (data != null) {
                $("#" + controlName).html("");

                if (showEmpty)
                    $("<option/>").attr("value", "").text("").appendTo($("#" + controlName));

                for (var i = 0; i < data.d.length; i++) {
                    $("<option/>").attr("value", data.d[i].ID).text(data.d[i].Nombre).appendTo($("#" + controlName));
                }

                if (idSelected != "")
                    $("#" + controlName).val(idSelected);

                //$("#" + controlName).trigger("change");
                $("#" + controlName).trigger("change");
                //$("#" + controlName).select2({width: '100%'});
            }
        }
    });
}

function obtenerDomicilios(controlName, idSelected, showEmpty, idPersona) {

    $.ajax({
        type: "POST",
        url: "/common.aspx/obtenerDomicilios",
        data: "{idPersona: '" + idPersona + "'}",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            if (data != null) {
                $("#" + controlName).html("");

                if (showEmpty)
                    $("<option/>").attr("value", "").text("").appendTo($("#" + controlName));

                for (var i = 0; i < data.d.length; i++) {
                    $("<option/>").attr("value", data.d[i].ID).text(data.d[i].Nombre).appendTo($("#" + controlName));
                }

                if (idSelected != "")
                    $("#" + controlName).val(idSelected);

                //$("#" + controlName).trigger("change");
                $("#" + controlName).trigger("change");
                //$("#" + controlName).select2({width: '100%'});
            }
        }
    });
}

function obtenerTransportes(controlName, idSelected, showEmpty) {

    $.ajax({
        type: "POST",
        url: "/common.aspx/obtenerTransportes",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            if (data != null) {
                $("#" + controlName).html("");

                if (showEmpty)
                    $("<option/>").attr("value", "").text("").appendTo($("#" + controlName));

                for (var i = 0; i < data.d.length; i++) {
                    $("<option/>").attr("value", data.d[i].ID).text(data.d[i].Nombre).appendTo($("#" + controlName));
                }

                if (idSelected != "")
                    $("#" + controlName).val(idSelected);

                //$("#" + controlName).trigger("change");
                $("#" + controlName).trigger("change");
                //$("#" + controlName).select2({width: '100%'});
            }
        }
    });
}


//function obtenerActividades(controlName, idSelected, showEmpty) {

//    $.ajax({
//        type: "POST",
//        url: "/common.aspx/obtenerActividades",
//        async: false,
//        contentType: "application/json; charset=utf-8",
//        dataType: "json",
//        success: function (data) {
//            if (data != null) {
//                $("#" + controlName).html("");

//                if (showEmpty)
//                    $("<option/>").attr("value", "").text("").appendTo($("#" + controlName));

//                for (var i = 0; i < data.d.length; i++) {
//                    $("<option/>").attr("value", data.d[i].ID).text(data.d[i].Nombre).appendTo($("#" + controlName));
//                }

//                if (idSelected != "")
//                    $("#" + controlName).val(idSelected);

//                //$("#" + controlName).trigger("change");
//                $("#" + controlName).trigger("change");
//                //$("#" + controlName).select2({width: '100%'});
//            }
//        }
//    });
//}

function obtenerTransportePersona(controlName, idSelected, showEmpty, idPersona) {

    $.ajax({
        type: "POST",
        url: "/common.aspx/obtenerTransportePersona",
        data: "{idPersona: '" + idPersona + "'}",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            if (data != null) {
                $("#" + controlName).html("");

                if (showEmpty)
                    $("<option/>").attr("value", "").text("").appendTo($("#" + controlName));

                for (var i = 0; i < data.d.length; i++) {
                    $("<option/>").attr("value", data.d[i].ID).text(data.d[i].Nombre).appendTo($("#" + controlName));
                }

                if (idSelected != "")
                    $("#" + controlName).val(idSelected);

                //$("#" + controlName).trigger("change");
                $("#" + controlName).trigger("change");
                //$("#" + controlName).select2({width: '100%'});
            }
        }
    });
}

function obtenerProveedores(controlName, idSelected, showEmpty) {

    $.ajax({
        type: "GET",
        url: "/common.aspx/obtenerProveedores",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            if (data != null) {
                $("#" + controlName).html("");

                if (showEmpty)
                    $("<option/>").attr("value", "").text("").appendTo($("#" + controlName));

                for (var i = 0; i < data.d.length; i++) {
                    $("<option/>").attr("value", data.d[i].ID).text(data.d[i].Nombre).appendTo($("#" + controlName));
                }

                if (idSelected != "")
                    $("#" + controlName).val(idSelected);

                //$("#" + controlName).trigger("change");
                $("#" + controlName).trigger("change");
                //$("#" + controlName).select2({width: '100%'});
            }
        }
    });
}

function obtenerVendedores(controlName, idSelected, showEmpty) {

    $.ajax({
        type: "GET",
        url: "/common.aspx/obtenerVendedores",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            if (data != null) {
                $("#" + controlName).html("");

                if (showEmpty)
                    $("<option/>").attr("value", "").text("").appendTo($("#" + controlName));

                for (var i = 0; i < data.d.length; i++) {
                    $("<option/>").attr("value", data.d[i].ID).text(data.d[i].Nombre).appendTo($("#" + controlName));
                }

                if (idSelected != "")
                    $("#" + controlName).val(idSelected);

                //$("#" + controlName).trigger("change");
                $("#" + controlName).trigger("change");
                //$("#" + controlName).select2({width: '100%'});
            }
        }
    });
}

function obtenerCodigosCompraAutomatica(controlName, idSelected, showEmpty) {

    $.ajax({
        type: "GET",
        url: "/common.aspx/obtenerCodigosCompraAutomatica",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            if (data != null) {
                $("#" + controlName).html("");

                if (showEmpty)
                    $("<option/>").attr("value", "").text("").appendTo($("#" + controlName));

                for (var i = 0; i < data.d.length; i++) {
                    $("<option/>").attr("value", data.d[i].ID).text(data.d[i].Nombre).appendTo($("#" + controlName));
                }

                if (idSelected != "")
                    $("#" + controlName).val(idSelected);

                //$("#" + controlName).trigger("change");
                $("#" + controlName).trigger("change");
                //$("#" + controlName).select2({width: '100%'});
            }
        }
    });
}

function obtenerPeriodosPdv(controlName, idSelected, showEmpty) {

    $.ajax({
        type: "GET",
        url: "/common.aspx/obtenerPeriodosPdv",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            if (data != null) {
                $("#" + controlName).html("");

                if (showEmpty)
                    $("<option/>").attr("value", "").text("").appendTo($("#" + controlName));

                for (var i = 0; i < data.d.length; i++) {
                    $("<option/>").attr("value", data.d[i].ID).text(data.d[i].Nombre).appendTo($("#" + controlName));
                }

                if (idSelected != "")
                    $("#" + controlName).val(idSelected);

                //$("#" + controlName).trigger("change");
                $("#" + controlName).trigger("change");
                //$("#" + controlName).select2({width: '100%'});
            }
        }
    });
}


function obtenerCodigosFacturaAutomatica(controlName, idSelected, showEmpty) {

    $.ajax({
        type: "GET",
        url: "/common.aspx/obtenerCodigosFacturaAutomatica",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            if (data != null) {
                $("#" + controlName).html("");

                if (showEmpty)
                    $("<option/>").attr("value", "").text("").appendTo($("#" + controlName));

                for (var i = 0; i < data.d.length; i++) {
                    $("<option/>").attr("value", data.d[i].ID).text(data.d[i].Nombre).appendTo($("#" + controlName));
                }

                if (idSelected != "")
                    $("#" + controlName).val(idSelected);

                //$("#" + controlName).trigger("change");
                $("#" + controlName).trigger("change");
                //$("#" + controlName).select2({width: '100%'});
            }
        }
    });
}


function obtenerProveedoresSinCuit(controlName, idSelected, showEmpty) {

    $.ajax({
        type: "GET",
        url: "/common.aspx/obtenerProveedoresSinCuit",
        async: false,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            if (data != null) {
                $("#" + controlName).html("");

                if (showEmpty)
                    $("<option/>").attr("value", "").text("").appendTo($("#" + controlName));

                for (var i = 0; i < data.d.length; i++) {
                    $("<option/>").attr("value", data.d[i].ID).text(data.d[i].Nombre).appendTo($("#" + controlName));
                }

                if (idSelected != "")
                    $("#" + controlName).val(idSelected);

                //$("#" + controlName).trigger("change");
                $("#" + controlName).trigger("change");
                //$("#" + controlName).select2({width: '100%'});
            }
        }
    });
}

function obtenerProxNroComprobante(controlName, tipo, idpuntoDeVenta) {

    $.ajax({
        type: "POST",
        url: "/common.aspx/obtenerProxNroComprobante",
        data: "{tipo: '" + tipo + "', idPuntoDeVenta: " + idpuntoDeVenta + "}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            if (data != null) {
                $("#" + controlName).val(data.d);
            }
        }
    });
}

function obtenerProxNroCobranza(controlName, tipo) {

    $.ajax({
        type: "POST",
        url: "/common.aspx/obtenerProxNroCobranza",
        data: "{tipo: '" + tipo + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            if (data != null) {
                $("#" + controlName).val(data.d);
            }
        }
    });
}

function padZeros(str, max) {
    str = str.toString();
    return str.length < max ? padZeros("0" + str, max) : str;
}


/*** VALIDATIONS ***/
function validateEmail(field) {
    var regex = /\b[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}\b/i;
    return (regex.test(field)) ? true : false;
}

function EmailsAreValid(value) {
    var result = value.split(",");
    for (var i = 0; i < result.length; i++)
        if (!validateEmail(result[i]))
            return false;
    return true;
}


function remplazarCaracteresEspeciales(nombre) {

    var valor = $("#" + nombre).val();
    var specialChars = "!@#$^&%*()+=-[]\/{}|:<>?,.";
    for (var i = 0; i < specialChars.length; i++) {
        valor = valor.replace(new RegExp("\\" + specialChars[i], 'gi'), '');
    }
    $("#" + nombre).val(valor);
}

function CheckPassword(inputtxt) {
    var passw = /^(?=.*\d)(?=.*[a-z])(?=.*[A-Z]).{8,12}$/;
    if (inputtxt.match) {
        if (inputtxt.length > 0) {
            if (inputtxt.match(passw)) {
                return true;
            }
            else {
                return false;
            }
        } else {
            return true;
        }   
    } else {
        return true;
    }    
}

function CuitEsValido(value) {
    var cuit = value;
    if (typeof (cuit) == 'undefined')
        return true;

    cuit = cuit.toString().replace(/[-_]/g, "");
    if (cuit == '')
        return true; //No estamos validando si el campo esta vacio, eso queda para el "required"

    if (cuit.length != 11)
        return false;
    else {

        var mult = [5, 4, 3, 2, 7, 6, 5, 4, 3, 2];
        var total = 0;

        for (var i = 0; i < mult.length; i++) {
            total += parseInt(cuit.charAt(i)) * mult[i];
        }

        var mod = total % 11;
        var digito = mod == 0 ? 0 : mod == 1 ? 9 : 11 - mod;
    }

    return digito == parseInt(cuit.charAt(10));
}

function configDatePicker() {
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

    Common.configFechas('dd/mm/yy');
}
/*
function configFechas(format) {
    $.validator.addMethod("validDate", function (value, element) {
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
}

function configFechasDesdeHasta(date, end) {

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
        errorClass: 'error',
        validClass: 'valid',
        highlight: function (element) {
            $(element).closest('div').addClass("f_error");
        },
        unhighlight: function (element) {
            $(element).closest('div').removeClass("f_error");
        },
        errorPlacement: function (error, element) {
            $(element).closest('div').append(error);
        }
    });

    //$("#" + date).datepicker({ dateFormat: 'dd/mm/yy' });
    //$("#" + end).datepicker({ dateFormat: 'dd/mm/yy' });

    $('#' + date).change(cleanErrors);
    $('#' + end).change(cleanErrors);
}
*/
function cleanErrors() {
    $('form').validate();
    if ($('form').valid()) {
        $('label .error').hide();
    }
}

function parseEnDate(value) {
    var bits = value.match(/([0-9]+)/gi), str;
    str = bits[1] + '/' + bits[0] + '/' + bits[2];
    return new Date(str);
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

        $('.validDateFull').datepicker({
            dateFormat: "dd/mm/yy",
            changeMonth: true,
            changeYear: true,
            showButtonPanel: true,
            maxDate: '120'
        });

        Common.configFechas();
    },
    configDatePickerDosMeses: function () {
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
            numberOfMonths: 2,
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

        $('#' + date).change(cleanErrors);
        $('#' + end).change(cleanErrors);
    },
    obtenerPersonas: function (controlName, idSelected, showEmpty) {
        obtenerPersonas(controlName, idSelected, showEmpty);
    },
    obtenerClientes: function (controlName, idSelected, showEmpty) {
        obtenerClientes(controlName, idSelected, showEmpty);
    },
    obtenerDomicilios: function (controlName, idSelected, showEmpty, idPersona) {
        obtenerDomicilios(controlName, idSelected, showEmpty, idPersona);
    },
    obtenerTransportes: function (controlName, idSelected, showEmpty) {
        obtenerTransportes(controlName, idSelected, showEmpty);
    },
    obtenerTransportePersona: function (controlName, idSelected, showEmpty, idPersona) {
        obtenerTransportePersona(controlName, idSelected, showEmpty, idPersona);
    },
    obtenerProveedores: function (controlName, idSelected, showEmpty) {
        obtenerProveedores(controlName, idSelected, showEmpty);
    },
    obtenerVendedores: function (controlName, idSelected, showEmpty) {
        obtenerVendedores(controlName, idSelected, showEmpty);
    },
    obtenerPeriodos: function (controlName, idSelected, showEmpty) {
        obtenerPeriodos(controlName, idSelected, showEmpty);
    }, 
    obtenerPeriodosPdv: function (controlName, idSelected, showEmpty) {
        obtenerPeriodosPdv(controlName, idSelected, showEmpty);
    },    
    obtenerCodigosCompraAutomatica: function (controlName, idSelected, showEmpty) {
        obtenerCodigosCompraAutomatica(controlName, idSelected, showEmpty);
    },
    obtenerCodigosFacturaAutomatica: function (controlName, idSelected, showEmpty) {
        obtenerCodigosFacturaAutomatica(controlName, idSelected, showEmpty);
    },
    obtenerProveedoresSinCuit: function (controlName, idSelected, showEmpty) {
        obtenerProveedoresSinCuit(controlName, idSelected, showEmpty);
    },
    obtenerComprobantesVentaPorCondicion: function (controlName, condicionIva, selectedValue) {
        var tipoComprobante = getParameterByName('tipo');
        if (tipoComprobante != "") {
            if (tipoComprobante == "PDV") {
                $("#" + controlName).append('<option value="PDV">Pedido de venta</option>');
                $("#" + controlName + " option[value='']").remove();
            } else {
                if (tipoComprobante == "PDC") {
                    $("#" + controlName).append('<option value="PDC">Pedido de compra</option>');
                    $("#" + controlName + " option[value='']").remove();
                } else {
                    if (tipoComprobante == "EDA") {
                        $("#" + controlName).append('<option value="EDA">Entrega de articulos</option>');
                        $("#" + controlName + " option[value='']").remove();
                    } else {
                        if (tipoComprobante == "DDC") {
                            $("#" + controlName).append('<option value="DDC">Detalle del Comprobante</option>');
                            $("#" + controlName + " option[value='']").remove();
                        } else {
                            if (MI_CONDICION == "RI") {
                                if (condicionIva == "CF" || condicionIva == "EX" || condicionIva == "MO") {
                                    $("#" + controlName).append('<option value="FCA">Factura A</option>');                                    
                                    $("#" + controlName).append('<option value="FCB">Factura B</option>');                                    
                                    $("#" + controlName).append('<option value="NDB">Nota débito B</option>');
                                    $("#" + controlName).append('<option value="NCA">Nota crédito A</option>');
                                    $("#" + controlName).append('<option value="NCB">Nota crédito B</option>');
                                    $("#" + controlName).append('<option value="RCB">Recibo B</option>');

                                    $("#" + controlName).append('<option value="FCAMP">Factura A MiPyMES</option>');
                                    $("#" + controlName).append('<option value="FCBMP">Factura B MiPyMES</option>');
                                    $("#" + controlName).append('<option value="NDBMP">Nota débito B MiPyMES</option>');
                                    $("#" + controlName).append('<option value="NCAMP">Nota crédito A MiPyMES</option>');
                                    $("#" + controlName).append('<option value="NCBMP">Nota crédito B MiPyMES</option>');
                                    $("#" + controlName).append('<option value="RCBMP">Recibo B MiPyMES</option>');
                                }
                                else {
                                    $("#" + controlName).append('<option value="FCA">Factura A</option>');
                                    //$("#" + controlName).append('<option value="FCB">Factura B</option>');
                                    $("#" + controlName).append('<option value="NDA">Nota débito A</option>');
                                    //$("#" + controlName).append('<option value="NDB">Nota débito B</option>');

                                    $("#" + controlName).append('<option value="NCA">Nota crédito A</option>');
                                    //$("#" + controlName).append('<option value="NCB">Nota crédito B</option>');
                                    $("#" + controlName).append('<option value="RCA">Recibo A</option>');
                                    //$("#" + controlName).append('<option value="RCB">Recibo B</option>');

                                    $("#" + controlName).append('<option value="FCAMP">Factura A MiPyMES</option>');
                                    $("#" + controlName).append('<option value="NDAMP">Nota débito A MiPyMES</option>');
                                    $("#" + controlName).append('<option value="NCAMP">Nota crédito A MiPyMES</option>');
                                    $("#" + controlName).append('<option value="RCAMP">Recibo A MiPyMES</option>');
                                }
                                //$("#" + controlName).append('<option value="COT">Cotización</option>');
                            }
                            else if (MI_CONDICION == "MO" || MI_CONDICION == "EX") {
                                $("#" + controlName).append('<option value="FCC">Factura C</option>');
                                $("#" + controlName).append('<option value="NDC">Nota débito C</option>');

                                $("#" + controlName).append('<option value="NCC">Nota crédito C</option>');
                                $("#" + controlName).append('<option value="RCC">Recibo C</option>');

                                $("#" + controlName).append('<option value="FCCMP">Factura C MiPyMES</option>');
                                $("#" + controlName).append('<option value="NDCMP">Nota débito C MiPyMES</option>');
                                $("#" + controlName).append('<option value="NCCMP">Nota crédito C MiPyMES</option>');
                                $("#" + controlName).append('<option value="RCCMP">Recibo C MiPyMES</option>');
                            }
                            //else if (MI_CONDICION == "EX") {
                            //    $("#" + controlName).append('<option value="FCB">Factura B</option>');
                            //    $("#" + controlName).append('<option value="NDB">Nota débito B</option>');
                            //    //$("#" + controlName).append('<option value="RCB">Recibo B</option>');
                            //}
                            $("#" + controlName).append('<option value="COT">Cotización</option>');
                        }
                    }
                }
            }
        } else {
            if (MI_CONDICION == "RI") {
                if (condicionIva == "CF" || condicionIva == "EX" || condicionIva == "MO") {
                    $("#" + controlName).append('<option value="FCA">Factura A</option>');
                    $("#" + controlName).append('<option value="FCB">Factura B</option>');
                    $("#" + controlName).append('<option value="NDB">Nota débito B</option>');
                    $("#" + controlName).append('<option value="NCA">Nota crédito A</option>');
                    $("#" + controlName).append('<option value="NCB">Nota crédito B</option>');
                    $("#" + controlName).append('<option value="RCB">Recibo B</option>');


                    $("#" + controlName).append('<option value="FCAMP">Factura A MiPyMES</option>');
                    $("#" + controlName).append('<option value="FCBMP">Factura B MiPyMES</option>');
                    $("#" + controlName).append('<option value="NDBMP">Nota débito B MiPyMES</option>');
                    $("#" + controlName).append('<option value="NCAMP">Nota crédito A MiPyMES</option>');
                    $("#" + controlName).append('<option value="NCBMP">Nota crédito B MiPyMES</option>');
                    $("#" + controlName).append('<option value="RCBMP">Recibo B MiPyMES</option>');

                }
                else {
                    $("#" + controlName).append('<option value="FCA">Factura A</option>');
                    //$("#" + controlName).append('<option value="FCB">Factura B</option>');
                    $("#" + controlName).append('<option value="NDA">Nota débito A</option>');
                    //$("#" + controlName).append('<option value="NDB">Nota débito B</option>');

                    $("#" + controlName).append('<option value="NCA">Nota crédito A</option>');
                    //$("#" + controlName).append('<option value="NCB">Nota crédito B</option>');
                    $("#" + controlName).append('<option value="RCA">Recibo A</option>');
                    //$("#" + controlName).append('<option value="RCB">Recibo B</option>');

                    $("#" + controlName).append('<option value="FCAMP">Factura A MiPyMES</option>');
                    $("#" + controlName).append('<option value="NDAMP">Nota débito A MiPyMES</option>');
                    $("#" + controlName).append('<option value="NCAMP">Nota crédito A MiPyMES</option>');
                    $("#" + controlName).append('<option value="RCAMP">Recibo A MiPyMES</option>');

                }
                //$("#" + controlName).append('<option value="COT">Cotización</option>');
            }
            else if (MI_CONDICION == "MO" || MI_CONDICION == "EX") {
                $("#" + controlName).append('<option value="FCC">Factura C</option>');
                $("#" + controlName).append('<option value="NDC">Nota débito C</option>');

                $("#" + controlName).append('<option value="NCC">Nota crédito C</option>');
                $("#" + controlName).append('<option value="RCC">Recibo C</option>');

                $("#" + controlName).append('<option value="FCCMP">Factura C MiPyMES</option>');
                $("#" + controlName).append('<option value="NDCMP">Nota débito C MiPyMES</option>');
                $("#" + controlName).append('<option value="NCCMP">Nota crédito C MiPyMES</option>');
                $("#" + controlName).append('<option value="RCCMP">Recibo C MiPyMES</option>');

            }
            //else if (MI_CONDICION == "EX") {
            //    $("#" + controlName).append('<option value="FCB">Factura B</option>');
            //    $("#" + controlName).append('<option value="NDB">Nota débito B</option>');
            //    //$("#" + controlName).append('<option value="RCB">Recibo B</option>');
            //}
            $("#" + controlName).append('<option value="COT">Cotización</option>');
        }
       

        if (selectedValue != "")
            $("#" + controlName).val(selectedValue);
    },    
    obtenerComprobantesPagoPorCondicion: function (controlName, condicionIva, selectedValue) {
        //if (MI_CONDICION == "RI") {
        if (condicionIva == "MO" || condicionIva == "EX") {
            $("#" + controlName).append('<option value="FCC">Factura C</option>');
            $("#" + controlName).append('<option value="NCC">Nota crédito C</option>');
            $("#" + controlName).append('<option value="NDC">Nota débito C</option>');
            $("#" + controlName).append('<option value="RCC">Recibo C</option>');
        }
        else if (condicionIva == "RI") {
            $("#" + controlName).append('<option value="FCA">Factura A</option>');
            $("#" + controlName).append('<option value="FCB">Factura B</option>');
            $("#" + controlName).append('<option value="NCA">Nota crédito A</option>');
            $("#" + controlName).append('<option value="NCB">Nota crédito B</option>');
            $("#" + controlName).append('<option value="NDA">Nota débito A</option>');
            $("#" + controlName).append('<option value="NDB">Nota débito B</option>');
            $("#" + controlName).append('<option value="RCA">Recibo A</option>');
            $("#" + controlName).append('<option value="RCB">Recibo B</option>');
        }

        $("#" + controlName).append('<option value="CDC">Comprobante de compra</option>');
        $("#" + controlName).append('<option value="COT">Cotización</option>');
        $("#" + controlName).append('<option value="BOR">Borrador</option>');

        if (selectedValue != "")
            $("#" + controlName).val(selectedValue);
    },
    obtenerComprobantesCobranzasPorCondicion: function (controlName, condicionIva, selectedValue) {//TODO: TMB AGREGAR CONDICION EMISOR
        $("#" + controlName).html("");
        $("#" + controlName).append('<option value="RC">Recibo</option>');
        /*if (MI_CONDICION == "RI") {
            if (condicionIva != "RI") {
                //    $("#" + controlName).append('<option value="RCC">Recibo C</option>');
                //} else if (condicionIva == "CF" || condicionIva == "EX") {
                $("#" + controlName).append('<option value="RCB">Recibo</option>');
            }
            else {
                $("#" + controlName).append('<option value="RCA">Recibo</option>');
            }
        }
        else if (MI_CONDICION == "MO") {
            $("#" + controlName).append('<option value="RCC">Recibo</option>');
        }
        else if (MI_CONDICION == "EX") {
            $("#" + controlName).append('<option value="RCB">Recibo</option>');
        }

        $("#" + controlName).append('<option value="SIN">Ninguno</option>');

        if (selectedValue != "")
            $("#" + controlName).val(selectedValue);*/
    },
    obtenerCondicionIvaDesc: function (tipo) {
        if (tipo == "RI")
            return "Responsable Inscripto";
        else if (tipo == "MO")
            return "Monotributo";
        else if (tipo == "EX")
            return "Exento";
        else
            return "Cliente final";
        //else if (tipo == "NI")
        //    return "Responsable NO inscripto";

    },
    obtenerProxNroComprobante: function (controlName, tipo, idpuntoDeVenta) {
        $.ajax({
            type: "POST",
            url: "/common.aspx/obtenerProxNroComprobante",
            data: "{tipo: '" + tipo + "', idPuntoDeVenta: " + idpuntoDeVenta + "}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                if (data != null) {
                    $("#" + controlName).val(data.d);
                }
            }
        });
    },
    obtenerProxNroCobranza: function (controlName, tipo) {
        $.ajax({
            type: "POST",
            url: "/common.aspx/obtenerProxNroCobranza",
            data: "{tipo: '" + tipo + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                if (data != null) {
                    $("#" + controlName).val(data.d);
                }
            }
        });
    },
    obtenerPuntosDeVenta: function (controlName) {
        obtenerPuntosDeVenta(controlName);
    },
    obtenerActividades: function (controlName, idSelected, showEmpty) {
        obtenerActividades(controlName, idSelected, showEmpty);
    },
    obtenerTipoComprobanteAfip: function (controlName, idSelected, showEmpty) {
        obtenerTipoComprobanteAfip(controlName, idSelected, showEmpty);
    },
    obtenerPuntosDeVentaYNroCobranza: function (controlName) {
        obtenerPuntosDeVentaYNroCobranza(controlName);
    },
    previsualizarComprobanteGenerado: function (comprobante) {
        $("#divError").hide();
        var year = new Date().getFullYear().toString();
        if (comprobante != "") {
            var version = new Date().getTime();

            $("#ifrPdf").attr("src", "/files/explorer/" + MI_IDUSUARIO + "/comprobantes/" + year + "/" + comprobante + "?" + version + "#zoom=100&view=FitH,top");
        }
        $('#modalPdf').modal('show');
    },
    imprimirArchivoDesdeIframe: function (comprobante) {
        if (comprobante != "") {
            var d = new Date();
            var n = d.getFullYear();
            var version = new Date().getTime();
            if ($("#fileName").val() != null || $("#fileName").val() != undefined)
                var pathFile = "/files/explorer/" + $("#hdnIDUsuario").val() + "/comprobantes/" + n + "/" + fileName + "?" + version + "#zoom=100&view=FitH,top";
            else
                var pathFile = "/files/explorer/" + $("#hdnIDUsuario").val() + "/comprobantes/" + n + "/" + comprobante + "?" + version + "#zoom=100&view=FitH,top";
            $("#ifrPdf").attr("src", pathFile);

            //$("#ifrPdf").attr("src", "/files/comprobantes/" + comprobante + "#zoom=100&view=FitH,top");
        }

        var getMyFrame = document.getElementById("ifrPdf");
        getMyFrame.focus();
        getMyFrame.contentWindow.print();

    },
    obtenerCategorias: function (controlName, showEmpty) {
        $.ajax({
            type: "GET",
            url: "/common.aspx/obtenerCategorias",
            data: "{}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                if (data != null) {
                    if (showEmpty)
                        $("<option/>").attr("value", "").text("").appendTo($("#" + controlName));

                    for (var i = 0; i < data.d.length; i++) {
                        $("<option/>").attr("value", data.d[i].ID).text(data.d[i].Nombre).appendTo($("#" + controlName));
                    }

                    $("#" + controlName).trigger("change");
                }
            }
        });
    },
    obtenerConceptosCodigoyNombre: function (controlName, tipo, showEmpty) {

        var aux = "";
        if (tipo == 1)
            aux = "P";
        else if (tipo == 2)
            aux = "S";

        $("#" + controlName).html("");

        $.ajax({
            type: "POST",
            url: "/common.aspx/obtenerConceptosCodigoyNombre",
            data: "{tipo: '" + aux + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                if (data != null) {
                    if (showEmpty)
                        $("<option/>").attr("value", "").text("-").appendTo($("#" + controlName));

                    for (var i = 0; i < data.d.length; i++) {
                        $("<option/>").attr("value", data.d[i].ID).text(data.d[i].Nombre).appendTo($("#" + controlName));
                    }

                    $("#" + controlName).trigger("change");
                }
                else {
                    $("<option/>").attr("value", "").text("-").appendTo($("#" + controlName));
                    $("#" + controlName).trigger("change");
                }
            }
        });
    },
    obtenerConceptos: function (controlName, tipo, idSelected, showEmpty) {

        var aux = "";
        if (tipo == 1)
            aux = "P";
        else if (tipo == 2)
            aux = "S";

        $("#" + controlName).html("");

        $.ajax({
            type: "POST",
            url: "/common.aspx/obtenerConceptosCodigoyNombre",
            data: "{tipo: '" + aux + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                if (data != null) {
                    $("#" + controlName).html("");

                    if (showEmpty)
                        $("<option/>").attr("value", "").text("").appendTo($("#" + controlName));

                    for (var i = 0; i < data.d.length; i++) {
                        $("<option/>").attr("value", data.d[i].ID).text(data.d[i].Nombre).appendTo($("#" + controlName));
                    }

                    if (idSelected != "")
                        $("#" + controlName).val(idSelected);

                    //$("#" + controlName).trigger("change");
                    $("#" + controlName).trigger("change");
                    //$("#" + controlName).select2({width: '100%'});
                }
            }
        });
    },
    obtenerCheques: function (controlName, idSelected, showEmpty, propios, empresa) {
        $.ajax({
            type: "POST",
            url: "/common.aspx/obtenerCheques",
            data: "{ EsPropio :" + propios + ", EsEmpresa: " + empresa + "}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                if (data != null) {
                    $("#" + controlName).html("");

                    if (showEmpty)
                        $("<option/>").attr("value", "").text("").appendTo($("#" + controlName));

                    for (var i = 0; i < data.d.length; i++) {
                        $("<option/>").attr("value", data.d[i].ID).text(data.d[i].Nombre).appendTo($("#" + controlName));
                    }

                    if (idSelected != "")
                        $("#" + controlName).val(idSelected);
                }
            }
        });
    },
    obtenerChequesConResto: function (controlName, idSelected, showEmpty, propios, empresa) {
        $.ajax({
            type: "POST",
            url: "/common.aspx/obtenerChequesConResto",
            data: "{ EsPropio :" + propios + ", EsEmpresa:" + empresa + "}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                if (data != null) {
                    $("#" + controlName).html("");

                    if (showEmpty)
                        $("<option/>").attr("value", "").text("").appendTo($("#" + controlName));

                    for (var i = 0; i < data.d.length; i++) {
                        $("<option/>").attr("value", data.d[i].ID).text(data.d[i].Nombre).appendTo($("#" + controlName));
                    }

                    if (idSelected != "")
                        $("#" + controlName).val(idSelected);
                }
            }
        });
    },
    obtenerBancos: function (controlName, idSelected) {
        $.ajax({
            type: "GET",
            url: "/common.aspx/obtenerBancos",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                if (data != null) {
                    $("#" + controlName).html("");

                    if (data.d.length > 1)
                        $("<option/>").attr("value", "").text("").appendTo($("#" + controlName));

                    for (var i = 0; i < data.d.length; i++) {
                        $("<option/>").attr("value", data.d[i].ID).text(data.d[i].Nombre).appendTo($("#" + controlName));
                    }

                    if (idSelected != "")
                        $("#" + controlName).val(idSelected);

                }
            }
        });

    },
    obtenerTodosBancos: function (controlName, idSelected) {
        $.ajax({
            type: "GET",
            url: "/common.aspx/obtenerTodosBancos",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                if (data != null) {
                    $("#" + controlName).html("");

                    if (data.d.length > 1)
                        $("<option/>").attr("value", "").text("").appendTo($("#" + controlName));

                    for (var i = 0; i < data.d.length; i++) {
                        $("<option/>").attr("value", data.d[i].ID).text(data.d[i].Nombre).appendTo($("#" + controlName));
                    }

                    if (idSelected != "")
                        $("#" + controlName).val(idSelected);

                }
            }
        });

    },
    obtenerUltimoNroRecibo: function (controlName, tipoComprobante, idPunto) {
        var info = "{tipoComprobante: '" + tipoComprobante + "',idPunto:" + idPunto + "}";
        $.ajax({
            type: "POST",
            url: "/common.aspx/obtenerUltimoNroRecibo",
            data: info,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                if (data != null) {
                    $("#" + controlName).val("");

                    $("#" + controlName).val(data.d);
                }
            }
        });

    },
    soloNumerosConGuiones: function (input) {
        $("#" + input).keydown(function (e) {
            if ($.inArray(e.keyCode, [46, 8, 9, 27, 13, 110, 190]) !== -1 ||
                (e.keyCode == 65 && e.ctrlKey === true) || (e.keyCode >= 35 && e.keyCode <= 39)) {
                return;
            }
            if ((e.shiftKey || (e.keyCode < 48 || e.keyCode > 57)) && (e.keyCode < 96 || e.keyCode > 105) && (e.keyCode != 173 && e.keyCode != 109)) {
                e.preventDefault();
            }
        });
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
    obtenerListaPrecios: function (controlName, idSelected, showEmpty) {

        $.ajax({
            type: "GET",
            url: "/common.aspx/obtenerListaPrecios",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                if (data != null) {
                    $("#" + controlName).html("");

                    if (showEmpty)
                        $("<option/>").attr("value", "0").text("Default").appendTo($("#" + controlName));

                    for (var i = 0; i < data.d.length; i++) {
                        $("<option/>").attr("value", data.d[i].ID).text(data.d[i].Nombre).appendTo($("#" + controlName));
                    }

                    if (idSelected != "")
                        $("#" + controlName).val(idSelected);
                }
            }
        });
    },
    obtenerProvincias: function (controlName, idSelected, showEmpty) {

        $.ajax({
            type: "GET",
            url: "/common.aspx/obtenerProvincias",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: false,
            success: function (data) {
                if (data != null) {
                    $("#" + controlName).html("");

                    if (showEmpty)
                        $("<option/>").attr("value", "").text("").appendTo($("#" + controlName));

                    for (var i = 0; i < data.d.length; i++) {
                        $("<option/>").attr("value", data.d[i].ID).text(data.d[i].Nombre).appendTo($("#" + controlName));
                    }

                    if (idSelected != "")
                        $("#" + controlName).val(idSelected).trigger("change");
                }
            }
        });
    },
    obtenerProvinciaYCiudadGenerica: function (controlNameProvincia, controlNameCiudad, idSelectedProvincia, idSelectedCiudad, showEmptyProvincia, showEmptyCiudad) {
        $.ajax({
            type: "GET",
            url: "/common.aspx/obtenerProvincias",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: false,
            success: function (data) {
                if (data != null) {
                    $("#" + controlNameProvincia).html("");

                    if (showEmptyProvincia)
                        $("<option/>").attr("value", "").text("").appendTo($("#" + controlNameProvincia));

                    for (var i = 0; i < data.d.length; i++) {
                        $("<option/>").attr("value", data.d[i].ID).text(data.d[i].Nombre).appendTo($("#" + controlNameProvincia));
                    }

                    if (idSelectedProvincia != "")
                        $("#" + controlNameProvincia).val(idSelectedProvincia).trigger("change");

                    Common.obtenerCiudades(controlNameCiudad, idSelectedCiudad, idSelectedProvincia, showEmptyCiudad);

                }
            }
        });
    },
    obtenerCiudades: function (controlName, idSelected, idProvincia, showEmpty) {

        $.ajax({
            type: "POST",
            url: "/common.aspx/obtenerCiudades",
            data: "{idProvincia: " + idProvincia + "}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: false,
            success: function (data) {
                if (data != null) {
                    $("#" + controlName).html("");

                    if (showEmpty)
                        $("<option/>").attr("value", "").text("").appendTo($("#" + controlName));

                    for (var i = 0; i < data.d.length; i++) {
                        $("<option/>").attr("value", data.d[i].ID).text(data.d[i].Nombre).appendTo($("#" + controlName));
                    }

                    if (idSelected != "")
                        $("#" + controlName).val(idSelected).trigger("change");
                }
            }
        });
    },
    obtenerProxNroPresupuesto: function (controlName) {
        $.ajax({
            type: "GET",
            url: "/common.aspx/obtenerProxNroPresupuesto",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                if (data != null) {
                    $("#" + controlName).val(data.d);
                }
            }
        });
    },
    pagarPlanActual: function (nombre) {
        window.location.href = "/modulos/seguridad/pagoDePlanes.aspx?plan=" + nombre;
    },
    cambiarSesion: function (id, nombre) {

        bootbox.confirm("¿Está seguro que desea iniciar sesión con la empresa " + nombre + "?", function (result) {
            if (result) {

                var info = "{ idEmpresa: '" + id + "'}";
                $.ajax({
                    type: "POST",
                    url: "/common.aspx/cambiarEmpresa",
                    data: info,
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data, text) {
                        window.location.href = "/home.aspx";
                    },
                    error: function (response) {
                        var r = jQuery.parseJSON(response.responseText);
                        $("#divError").html(r.Message);
                        $("#divError").show();
                        $('html, body').animate({ scrollTop: 0 }, 'slow');
                    }
                });
            }
        });
    },
    obtenerSelectPlanCuentas: function (controlName, idSelected, showEmpty) {

        $.ajax({
            type: "POST",
            url: "/common.aspx/obtenerSelectPlanCuentas",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                if (data != null) {
                    $("#" + controlName).html("");

                    if (showEmpty)
                        $("<option/>").attr("value", "").text("").appendTo($("#" + controlName));

                    for (var i = 0; i < data.d.length; i++) {
                        $("<option/>").attr("value", data.d[i].ID).text(data.d[i].Nombre).appendTo($("#" + controlName));
                    }

                    if (idSelected != "")
                        $("#" + controlName).val(idSelected).trigger("change");
                }
            }
        });
    },
    showModalCheque: function (propio, empresa, tipo) {

        if (tipo == "COBRANZA") {
            if (propio == "1") {
                if (empresa == "1") {
                    $("#divChequePersona").hide();
                    $("#divChequeEsPropioEmpresa").hide();
                    $("#txtEmisorCheque").val(MI_RAZONSOCIAL);
                    $("#txtCUIT").val(MI_RAZONSOCIAL);
                    $("#ddlEsPropio").val("S");
                    $("#ddlEsPropioEmpresa").val("S");
                } else {
                    $("#divChequePersona").hide();
                    $("#divChequeEsPropioEmpresa").hide();
                    var CuitCliente = $("#ddlPersona option:selected").text().split("-")[0];
                    var NombreCliente = $("#ddlPersona option:selected").text().split("-")[1];
                    //$("#txtEmisorCheque").val(MI_RAZONSOCIAL);
                    //$("#txtCUIT").val(MI_RAZONSOCIAL);
                    $("#txtEmisorCheque").val(NombreCliente);
                    $("#txtCUIT").val(CuitCliente);
                    $("#ddlEsPropio").val("S");
                    $("#ddlEsPropioEmpresa").val("N");
                }
            } else { // de terceros
                if (empresa == "1") {
                    $("#divChequeEsPropioEmpresa").show();                    
                    $("#ddlEsPropio").val("N");
                    $("#ddlEsPropioEmpresa").val("S");
                    //var CuitCliente = $("#ddlPersona option:selected").text().split("-")[0];
                    //var NombreCliente = $("#ddlPersona option:selected").text().split("-")[1];
                    $("#txtEmisorCheque").val(MI_RAZONSOCIAL);
                    $("#txtCUIT").val(MI_RAZONSOCIAL);
                    //$("#txtEmisorCheque").val(NombreCliente);
                    //$("#txtCUIT").val(CuitCliente);
                } else {
                    $("#divChequeEsPropioEmpresa").show();
                    $("#txtEmisorCheque").val("");
                    $("#divChequePersona").show();
                    if ($("#ddlPersona").val() != "" || $("#ddlPersona").val() != null) {
                        //var NombreCliente = $("#ddlPersona option:selected").text().split("-")[1];
                        //$("#txtEmisorCheque").val(NombreCliente);
                        $("#txtEmisorCheque").val("");
                        $("#txtCUIT").val("");
                    }
                    $("#ddlEsPropio").val("N");
                    $("#ddlEsPropioEmpresa").val("N");
                }
            }
        } else { //PAGOS
            if (propio == "1") {
                if (empresa == "1") {
                    $("#divChequePersona").hide();
                    $("#divChequeEsPropioEmpresa").hide();
                    $("#txtEmisorCheque").val(MI_RAZONSOCIAL);
                    $("#txtCUIT").val(MI_RAZONSOCIAL);
                    $("#ddlEsPropio").val("S");
                    $("#ddlEsPropioEmpresa").val("S");
                } else {
                    $("#divChequePersona").show();
                    $("#divChequeEsPropioEmpresa").hide();
                    var CuitCliente = $("#ddlPersona option:selected").text().split("-")[0];
                    var NombreCliente = $("#ddlPersona option:selected").text().split("-")[1];
                    //$("#txtEmisorCheque").val(MI_RAZONSOCIAL);
                    //$("#txtCUIT").val(MI_RAZONSOCIAL);
                    $("#txtEmisorCheque").val(NombreCliente);
                    $("#txtCUIT").val(CuitCliente);
                    $("#ddlEsPropio").val("S");
                    $("#ddlEsPropioEmpresa").val("N");
                }
            } else { // de terceros
                if (empresa == "1") {
                    $("#divChequePersona").hide();
                    $("#divChequeEsPropioEmpresa").show();
                    $("#ddlEsPropio").val("S");
                    $("#ddlEsPropioEmpresa").val("S");
                    //var CuitCliente = $("#ddlPersona option:selected").text().split("-")[0];
                    //var NombreCliente = $("#ddlPersona option:selected").text().split("-")[1];
                    $("#txtEmisorCheque").val(MI_RAZONSOCIAL);
                    $("#txtCUIT").val(MI_RAZONSOCIAL);
                    $("#txtEmisorCheque").val(NombreCliente);
                    $("#txtCUIT").val(CuitCliente);
                } else {
                    $("#divChequePersona").show();
                    $("#divChequeEsPropioEmpresa").hide();
                    $("#txtEmisorCheque").val("");
                    $("#divChequePersona").show();
                    if ($("#ddlPersona").val() != "" || $("#ddlPersona").val() != null) {
                        //var NombreCliente = $("#ddlPersona option:selected").text().split("-")[1];
                        //$("#txtEmisorCheque").val(NombreCliente);
                        $("#txtEmisorCheque").val("");
                        $("#txtCUIT").val("");
                    }
                    $("#ddlEsPropio").val("N");
                    $("#ddlEsPropioEmpresa").val("N");
                }
            }
        }





        if (propio == "1") {
            if (tipo == "cobranza") {
                $("#divChequePersona").hide();
                $("#divChequeEsPropioEmpresa").hide();
                var CuitCliente = $("#ddlPersona option:selected").text().split("-")[0];
                var NombreCliente = $("#ddlPersona option:selected").text().split("-")[1];
                //$("#txtEmisorCheque").val(MI_RAZONSOCIAL);
                //$("#txtCUIT").val(MI_RAZONSOCIAL);
                $("#txtEmisorCheque").val(NombreCliente);
                $("#txtCUIT").val(CuitCliente);
                $("#ddlEsPropio").val("S");
            } else {
                $("#divChequeEsPropioEmpresa").show();
                $("#txtEmisorCheque").val("");
                $("#divChequePersona").show();
                if ($("#ddlPersona").val() != "" || $("#ddlPersona").val() != null) {
                    //var NombreCliente = $("#ddlPersona option:selected").text().split("-")[1];
                    //$("#txtEmisorCheque").val(NombreCliente);
                    $("#txtEmisorCheque").val("");
                    $("#txtCUIT").val("");
                }
                $("#ddlEsPropio").val("S");
            }
        } else {
            if (tipo == "cobranza") {
                $("#divChequeEsPropioEmpresa").show();
                $("#txtEmisorCheque").val("");
                $("#divChequePersona").show();
                if ($("#ddlPersona").val() != "" || $("#ddlPersona").val() != null) {
                    //var NombreCliente = $("#ddlPersona option:selected").text().split("-")[1];
                    //$("#txtEmisorCheque").val(NombreCliente);
                    $("#txtEmisorCheque").val("");
                    $("#txtCUIT").val("");
                }
                $("#ddlEsPropio").val("N");
            } else {
                $("#divChequeEsPropioEmpresa").hide();
                $("#txtEmisorCheque").val("");
                $("#divChequePersona").show();
                if ($("#ddlPersona").val() != "" || $("#ddlPersona").val() != null) {
                    //var NombreCliente = $("#ddlPersona option:selected").text().split("-")[1];
                    //$("#txtEmisorCheque").val(NombreCliente);
                    $("#txtEmisorCheque").val("");
                    $("#txtCUIT").val("");
                }
                $("#ddlEsPropio").val("N");
            }
        }

        if (tipo == "COBRANZA") {
            if (propio == "1") {
                $("#divChequePersona").hide();
                $("#divChequeEsPropioEmpresa").hide();
                var CuitCliente = $("#ddlPersona option:selected").text().split("-")[0];
                var NombreCliente = $("#ddlPersona option:selected").text().split("-")[1];
                //$("#txtEmisorCheque").val(MI_RAZONSOCIAL);
                //$("#txtCUIT").val(MI_RAZONSOCIAL);
                $("#txtEmisorCheque").val(NombreCliente);
                $("#txtCUIT").val(CuitCliente);
            } else {
                $("#divChequeEsPropioEmpresa").show();
                $("#txtEmisorCheque").val("");
                $("#divChequePersona").show();
                if ($("#ddlPersona").val() != "" || $("#ddlPersona").val() != null) {
                    //var NombreCliente = $("#ddlPersona option:selected").text().split("-")[1];
                    //$("#txtEmisorCheque").val(NombreCliente);
                    $("#txtEmisorCheque").val("");
                    $("#txtCUIT").val("");
                }
            }
        } else {  /// POR PAGOS
            if (propio == "1") {
                $("#divChequeEsPropioEmpresa").show();
            } else {
                $("#divChequeEsPropioEmpresa").hide();
            }
            $("#txtEmisorCheque").val("");
            $("#divChequePersona").show();
            if ($("#ddlPersona").val() != "" || $("#ddlPersona").val() != null) {
                //var NombreCliente = $("#ddlPersona option:selected").text().split("-")[1];
                //$("#txtEmisorCheque").val(NombreCliente);
                $("#txtEmisorCheque").val("");
                $("#txtCUIT").val("");
            }
        }


        Common.obtenerTodosBancos("ddlBancosCheque");
        $("#divErrorCheque").hide();
        $("#divOkCheque").hide();
        $("#hdnChequePropio").val(propio);
        $("#modalNuevoCheque").show("modal");
    },
    sumarDiasFecha: function (days, fecha) {
        var date = new Date();

        var tiempo = date.getTime();
        var milisegundos = parseInt(days * 24 * 60 * 60 * 1000);
        var total = date.setTime(tiempo + milisegundos);
        var day = total.getDate();
        var month = total.getMonth() + 1;
        var year = total.getFullYear();
        return (day + "/" + month + "/" + year).toString();
    },
    configTelefono: function (controlName) {
        //$("#" + controlName).mask("?(9999) 9999-9999");
        $("#" + controlName).mask("?+99999999999999");
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

var Toolbar = {
    mostrarEnvio: function () {
        $("#divToolbar").slideUp("slow");
        //$("#txtEnvioAsunto, #txtEnvioMensaje").val("");
        //$("#txtEnvioMensaje").val("");
        $("#divSendEmail").slideDown("slow");
        $("#litModalOkTitulo").html("Enviar comprobante por mail");
    },
    cancelarEnvio: function () {
        Common.ocultarProcesando("btnEnviar", "Enviar");
        $("#litModalOkTitulo").html("Comprobante emitido correctamente");
        $("#divSendEmail").slideUp("slow");
        $("#divToolbar").slideDown("slow");
    },
    toggleEnviosError: function () {
        var isValid = true;

        if ($("#txtEnvioPara").val() == "") {
            $("#msgErrorEnvioPara").show();
            $("#txtEnvioPara").closest('.form-group').removeClass('has-success').addClass('has-error');
            isValid = false;
        }
        else if (!$("#txtEnvioPara").valid()) {
            $("#msgErrorEnvioPara").show();
            $("#txtEnvioPara").closest('.form-group').removeClass('has-success').addClass('has-error');
            isValid = false;
        }
        else {
            $("#msgErrorEnvioPara").hide();
            $("#txtEnvioPara").closest('.form-group').removeClass('has-error');
        }

        if ($("#txtEnvioAsunto").val() == "") {
            $("#msgErrorEnvioAsunto").show();
            $("#txtEnvioAsunto").closest('.form-group').removeClass('has-success').addClass('has-error');
            isValid = false;
        }
        else {
            $("#msgErrorEnvioAsunto").hide();
            $("#txtEnvioAsunto").closest('.form-group').removeClass('has-error');
        }

        if ($("#txtEnvioMensaje").val() == "") {
            $("#msgErrorEnvioMensaje").show();
            $("#txtEnvioMensaje").closest('.form-group').removeClass('has-success').addClass('has-error');
            isValid = false;
        }
        else {
            $("#msgErrorEnvioMensaje").hide();
            $("#txtEnvioMensaje").closest('.form-group').removeClass('has-error');
        }

        if (isValid) {
            $("#txtEnvioPara").closest('.form-group').removeClass('has-error');
            $("#txtEnvioAsunto").closest('.form-group').removeClass('has-error');
            $("#txtEnvioMensaje").closest('.form-group').removeClass('has-error');
        }

        return isValid;
    },
    enviarComprobantePorMail: function () {
        var isValid = Toolbar.toggleEnviosError();

        if (isValid) {
            Common.mostrarProcesando("btnEnviar");
            var info = "{ nombre:'" + $("#hdnRazonSocial").val() + "', para:'" + $("#txtEnvioPara").val() + "', asunto: '" + $("#txtEnvioAsunto").val() + "', mensaje: '" + $("#txtEnvioMensaje").val() + "', file: '" + $("#hdnFile").val() + "'}";
            $.ajax({
                type: "POST",
                url: "common.aspx/enviarComprobantePorMail",
                data: info,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data, text) {
                    Toolbar.cancelarEnvio();
                    $("#divOkMail").show();

                    setTimeout(function () {
                        $("#divOkMail").fadeOut(1500);
                    }, 3000);
                },
                error: function (response) {
                    var r = jQuery.parseJSON(response.responseText);
                    Common.ocultarProcesando("btnEnviar", "Enviar");
                    $("#msgErrorMail").html(r.Message);
                    $("#divErrorMail").show();
                    $("#divOkMail").hide();
                    //$('html, body').animate({ scrollTop: 0 }, 'slow');
                }
            });
        }
        else {
            return false;
        }
    },
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



//** AUTO COMPLETE HEADER **//
$(document).ready(function () {
    $(function () {
        var data = [
           { label: 'Buscar facturas', value: '/comprobantes.aspx' },
           { label: 'Buscar cobranzas', value: '/cobranzas.aspx' },
           { label: 'Buscar presupuestos', value: '/presupuestos.aspx' },
           { label: 'Buscar productos y servicios', value: '/conceptos.aspx' },
           { label: 'Buscar clientes', value: '/personas.aspx?tipo=c' },
           { label: 'Nueva factura', value: '/comprobantese.aspx' },
           { label: 'Nueva cobranza', value: '/cobranzase.aspx' },
           { label: 'Nuevo presupuesto', value: '/presupuestose.aspx' },
           { label: 'Nuevo producto o servicio', value: '/conceptose.aspx' },
           { label: 'Nuevo cliente', value: '/personase.aspx?tipo=c' },

           { label: 'Buscar comprobantes', value: '/compras.aspx' },
           { label: 'Buscar pagos', value: '/pagos.aspx' },
           { label: 'Buscar proveedores', value: '/personas.aspx?tipo=p' },
           { label: 'Nuevo comprobante', value: '/comprase.aspx' },
           { label: 'Nuevo pago', value: '/pagose.aspx' },
           { label: 'Nuevo proveedor', value: '/personase.aspx?tipo=p' },

           { label: 'Buscar bancos', value: '/modulos/tesoreria/bancos.aspx' },
           { label: 'Buscar gastos bancarios', value: '/modulos/tesoreria/gastosBancarios.aspx' },
           { label: 'Buscar cheque', value: '/modulos/tesoreria/cheques.aspx' },
           { label: 'Buscar caja', value: '/modulos/tesoreria/caja.aspx' },
           { label: 'Nuevo banco', value: '/modulos/tesoreria/bancose.aspx' },
           { label: 'Nuevo gastos bancarios', value: '/modulos/tesoreria/gastosBancariose.aspx' },
           { label: 'Nuevo cheque', value: '/modulos/tesoreria/chequese.aspx' },
        ];

        $("#autocompleteHeader").autocomplete({
            source: data,
            focus: function (event, ui) {
                $(event.target).val(ui.item.label);
                return false;
            },
            select: function (event, ui) {
                $(event.target).val(ui.item.label);
                window.location = ui.item.value;
                return false;
            }
        });
    });
});

