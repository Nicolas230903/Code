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

    var elem = $('#barchart');

    var cobros = ObtenerCobrado();
    var pagos = ObtenerPagado();

    var data = [
        { label: "Pagado", data: pagos, color: "#D9534F" },
        { label: "Cobrado", data: cobros, color: "#1CAF9A" }
    ];

    var options = {
        xaxis: {
            min: 0,
            max: 13,
            mode: null,
            ticks: [
                [1, MONTH_NAMES_SHORT[m12.getMonth()]],
                [2, MONTH_NAMES_SHORT[m11.getMonth()]],
                [3, MONTH_NAMES_SHORT[m10.getMonth()]],
                [4, MONTH_NAMES_SHORT[m9.getMonth()]],
                [5, MONTH_NAMES_SHORT[m8.getMonth()]],
                [6, MONTH_NAMES_SHORT[m7.getMonth()]],
                [7, MONTH_NAMES_SHORT[m6.getMonth()]],
                [8, MONTH_NAMES_SHORT[m5.getMonth()]],
                [9, MONTH_NAMES_SHORT[m4.getMonth()]],
                [10, MONTH_NAMES_SHORT[m3.getMonth()]],
                [11, MONTH_NAMES_SHORT[m2.getMonth()]],
                [12, MONTH_NAMES_SHORT[m1.getMonth()]]
            ],
            tickLength: 0
        }, grid: {
            borderColor: '#ddd',
            hoverable: true,
            clickable: true,
            borderWidth: 1,
            backgroundColor: '#fff'
        }, legend: {
            labelBoxBorderColor: "none",
            position: "nw"
        }, series: {
            shadowSize: 1,
            bars: {
                show: true,
                barWidth: 0.2,
                fillColor: { colors: [{ opacity: 0.5 }, { opacity: 1 }] },
                order: 1,
                align: "left"
            }
        }
    };

    $.plot(elem, data, options);

    elem.bind("plothover", function (event, pos, item) {
        if (item) {
            if (previousPoint != item.datapoint) {
                previousPoint = item.datapoint;
                $("#flot-tooltip").remove();

                y = item.datapoint[1];
                z = item.series.color;

                showTooltip(item.pageX, item.pageY,
                "<b>" + item.series.label + "</b> = $ " + y,
                //"<b>Importe = </b> $" + y,
                z);
            }
        } else {
            $("#flot-tooltip").remove();
            previousPoint = null;
        }
    });

    $("#barchart").bind("plotclick", function (event, pos, item) {
        if (item) {
            verDetalle(item.series.label, (item.dataIndex));
        }
    });
});

function showTooltip(x, y, contents) {
    jQuery('<div id="flot-tooltip" class="tooltipflot">' + contents + '</div>').css({
        position: 'absolute',
        display: 'none',
        top: y + 5,
        left: x + 5
    }).appendTo("body").fadeIn(200);
}

function ObtenerCobrado() {
    var ddata = [];

    $.ajax({
        type: "GET",
        url: "cobrado-vs-pagado.aspx/obtenerCobrado",
        async: false,//wait for result
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg, text) {
            var data = msg.d;
            for (i = 0; i < data.length; i++) {
                ddata.push([data[i].label, data[i].data]);
            }
        },
        error: function (response) {
            var r = jQuery.parseJSON(response.responseText);
            alert(r.Message);
        }
    });

    return ddata;
}

function ObtenerPagado() {
    var ddata = [];

    $.ajax({
        type: "GET",
        url: "cobrado-vs-pagado.aspx/obtenerPagado",
        async: false,//wait for result
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (msg, text) {
            var data = msg.d;
            for (i = 0; i < data.length; i++) {
                ddata.push([data[i].label, data[i].data]);
            }
        },
        error: function (response) {
            var r = jQuery.parseJSON(response.responseText);
            alert(r.Message);
        }
    });

    return ddata;
}

function verDetalle(tipoDeEgresoIngreso, index) {
    resetearExportacionDetalle();

    $("#bodyDetalle").html();
    var periodo = new Date().addMonths(index - 11);
    var fechadesde = "01" + "/" + (periodo.getMonth() + 1) + "/" + periodo.getFullYear();
    var fechaHasta = periodo.getDaysInMonth() + "/" + (periodo.getMonth() + 1) + "/" + periodo.getFullYear();
    $("#titDetalle").html("Detalle de " + tipoDeEgresoIngreso + " del período " + fechadesde + " hasta " + fechaHasta);



    var info = "{Periodo: " + parseInt(index) + ", Etiqueta: '" + tipoDeEgresoIngreso + "'}";
    $.ajax({
        type: "POST",
        url: "cobrado-vs-pagado.aspx/getDetail",
        data: info,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            if (data != null) {
                $("#bodyDetalle").html(data.d);

                $("#hdnPeriodo").val(index);
                $("#hdnEtiqueta").val(tipoDeEgresoIngreso);
            }
            $('#modalDetalle').modal('show');
        }
    });
}

function exportarDetalle() {
    resetearExportacionDetalle();

    $("#imgLoadingDetalle").show();
    $("#divIconoDescargarDetalle").hide();

    //var info = "{Periodo: " + parseInt(index) + ", Etiqueta: '" + tipoDeEgresoIngreso + "'}";
    var info = "{ Periodo: " + parseInt($("#hdnPeriodo").val()) + ", Etiqueta: '" + $("#hdnEtiqueta").val() + "'}";

    $.ajax({
        type: "POST",
        url: "cobrado-vs-pagado.aspx/exportDetalle",
        data: info,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data, text) {
            if (data.d != "") {
                $("#divError").hide();
                $("#imgLoadingDetalle").hide();
                $("#lnkDownloadDetalle").show();
                $("#lnkDownloadDetalle").attr("href", data.d);
                $("#lnkDownloadDetalle").attr("download", data.d);
            }
        }
    });
}

function resetearExportacionDetalle() {
    $("#imgLoadingDetalle, #lnkDownloadDetalle").hide();
    $("#divIconoDescargarDetalle").show();
}