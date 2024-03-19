var m1 = new Date();
var m2 = new Date().addMonths(-1);
var m3 = new Date().addMonths(-2);
var m4 = new Date().addMonths(-3);
var m5 = new Date().addMonths(-4);
var m6 = new Date().addMonths(-5);
var m7 = new Date().addMonths(-6);
var m8 = new Date().addMonths(-7);
var m9 = new Date().addMonths(-8);
var m10 = new Date().addMonths(-9);
var m11 = new Date().addMonths(-10);
var m12 = new Date().addMonths(-11);

jQuery(document).ready(function () {

    // Date Picker
    //configDatePicker();

    $("#txtFechaDesde, #txtFechaHasta").keypress(function (event) {
        var keycode = (event.keyCode ? event.keyCode : event.which);
        if (keycode == '13') {
            obtenerResultados();
            return false;
        }
    });

    // Date Picker
    Common.configDatePicker();
    Common.configFechasDesdeHasta("txtFechaDesde", "txtFechaHasta");

    // Validation with select boxes
    $("#frmSearch").validate({
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

    obtenerResultados();


    $("#piechart").bind("plotclick", function (event, pos, item) {
        if (item) {
            verDetalle(item.series.label, (item.dataIndex));
        }
    });
});

function obtenerResultados()
{
    
    if ($('#frmSearch').valid())
    {

        var data = ObtenerCompras($("#txtFechaDesde").val(), $("#txtFechaHasta").val());

        /*var piedata = [
            { label: "Series 1", data: [[1, 10]] },
            { label: "Series 2", data: [[1, 30]] },
            { label: "Series 3", data: [[1, 90]] },
            { label: "Series 4", data: [[1, 70]] },
            { label: "Series 5", data: [[1, 80]] }
        ];*/

        jQuery.plot('#piechart', data, {
            series: {
                pie: {
                    show: true,
                    radius: 1,
                    tilt: 0.5,
                    label: {
                        show: true,
                        radius: 2 / 3,
                        formatter: labelFormatter,
                        background: {
                            opacity: 0.5,
                            color: '#000'
                        },
                        threshold: 0.1
                    }/*,
                    combine: {
                        color: '#999',
                        threshold: 0.1
                    }*/

                    /*label: {
                        show: true,
                        radius: 2 / 3,
                        formatter: labelFormatter,
                        threshold: 0.1
                    }*/
                }
            },
            legend: {
                position: "nw",
                background: {
                    color: 'red'
                },
                labelFormatter: function (label, series) {
                    var percent = Math.round(series.percent);
                    var number = series.data[0][1]; //kinda weird, but this is what it takes
                    //return ("<a href=\"javascript:verDetalle('" + label + "')\"><b>" + label + "</b>: " + number + " (" + percent + "%)</a>");
                    return ("<b>" + label + "</b>: " + number + " (" + percent + "%)");
                }
            },
            grid: {
                hoverable: true,
                clickable: true
            }
        });
    }
}

function ObtenerCompras(desde, hasta) {
    var ddata = [];
    //var totalTerminales = 0;

    $.ajax({
        type: "POST",
        url: "/modulos/reportes/compras-por-categoria.aspx/obtenerCompras",
        data: "{desde: '" + desde + "', hasta:'" + hasta + "'}",
        async: false,//wait for result
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg, text) {
            var data = msg.d;
            for (i = 0; i < data.length; i++) {
                //totalTerminales += parseInt(data[i].data);
                ddata.push(data[i]);
            }
        },
        error: function (response) {
            var r = jQuery.parseJSON(response.responseText);
            alert(r.Message);
        }
    });
    return ddata;
}

function labelFormatter(label, series) {
    return "<div style='font-size:8pt; text-align:center; padding:2px; color:white;cursor:pointer'>" + label + "</div>";
}

function verDetalle(Etiqueta) {
    resetearExportacionDetalle();

    $("#bodyDetalle").html();
    $("#titDetalle").html("Detalle de " + Etiqueta + " del período " + $("#txtFechaDesde").val() + " hasta " + $("#txtFechaHasta").val());


    var info = "{fechaDesde: '" + $("#txtFechaDesde").val()
            + "',fechaHasta: '" + $("#txtFechaHasta").val()
            + "',Etiqueta: '" + Etiqueta
            + "'}";
    $.ajax({
        type: "POST",
        url: "/modulos/reportes/compras-por-categoria.aspx/getDetail",
        data: info,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            if (data != null) {
                $("#bodyDetalle").html(data.d);
                $("#hdnEtiqueta").val(Etiqueta);
            }
            $('#modalDetalle').modal('show');
        }
    });
}

function exportarDetalle() {
    resetearExportacionDetalle();

    $("#imgLoadingDetalle").show();
    $("#divIconoDescargarDetalle").hide();

    var info = "{fechaDesde: '" + $("#txtFechaDesde").val()
            + "',fechaHasta: '" + $("#txtFechaHasta").val()
            + "',Etiqueta: '" + $("#hdnEtiqueta").val()
            + "'}";

    $.ajax({
        type: "POST",
        url: "/modulos/reportes/compras-por-categoria.aspx/exportDetalle",
        data: info,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data, text) {
            if (data.d != "") {
                $("#divError").hide();
                $("#imgLoadingDetalle").hide();
                $("#lnkDownloadDetalle").show();
                var link = document.createElement("a");
                link.download = data.d;
                link.href = data.d;
                link.click();
            }
        }
    });
}
function resetearExportacionDetalle() {
    $("#imgLoadingDetalle, #lnkDownloadDetalle").hide();
    $("#divIconoDescargarDetalle").show();
}