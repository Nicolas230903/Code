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

var ddataBa = [];
var ddataPy = [];
var ddataPr = [];
var ddataEm = [];
var ddataIn = [];
var home = {
    ObtenerPlanesPorMeses: function () {
        var elem = $('#BarUsuariosRegistrados');
        //var Planes = home.ObtenerPlanes();
        home.ObtenerPlanes();
        var data = [
                        { label: "Plan Basico", data: ddataBa },
                        { label: "Plan Profesional", data: ddataPy },
                        { label: "Plan Pyme", data: ddataPr },
                        { label: "Plan Empresa", data: ddataEm },
                        { label: "Plan Prueba", data: ddataIn }
        ];

        var options = {
            xaxis: {
                min: 0,
                max: 7,
                mode: null,
                ticks: [
                    //[1, MONTH_NAMES_SHORT[m12.getMonth()]],
                    //[2, MONTH_NAMES_SHORT[m11.getMonth()]],
                    //[3, MONTH_NAMES_SHORT[m10.getMonth()]],
                    //[4, MONTH_NAMES_SHORT[m9.getMonth()]],
                    //[5, MONTH_NAMES_SHORT[m8.getMonth()]],
                    //[6, MONTH_NAMES_SHORT[m7.getMonth()]],
                    //[7, MONTH_NAMES_SHORT[m6.getMonth()]],
                    //[8, MONTH_NAMES_SHORT[m5.getMonth()]],
                    //[9, MONTH_NAMES_SHORT[m4.getMonth()]],
                    //[10, MONTH_NAMES_SHORT[m3.getMonth()]],
                    //[11, MONTH_NAMES_SHORT[m2.getMonth()]],
                    //[12, MONTH_NAMES_SHORT[m1.getMonth()]]
                    [1, MONTH_NAMES_SHORT[m6.getMonth()]],
                    [2, MONTH_NAMES_SHORT[m5.getMonth()]],
                    [3, MONTH_NAMES_SHORT[m4.getMonth()]],
                    [4, MONTH_NAMES_SHORT[m3.getMonth()]],
                    [5, MONTH_NAMES_SHORT[m2.getMonth()]],
                    [6, MONTH_NAMES_SHORT[m1.getMonth()]]
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

                    home.showTooltip(item.pageX, item.pageY,
                    "<b>" + item.series.label + "</b> = " + y,
                    //"<b>Importe = </b> $" + y,
                    z);
                }
            } else {
                $("#flot-tooltip").remove();
                previousPoint = null;
            }
        });

        //Click
        //$("#BarUsuariosRegistrados").bind("plotclick", function (event, pos, item) {
        //    if (item) {
        //         //verDetalle(item.series.label, (item.dataIndex));
        //    }
        //});
    },
    showTooltip: function (x, y, contents) {
        jQuery('<div id="flot-tooltip" class="tooltipflot">' + contents + '</div>').css({
            position: 'absolute',
            display: 'none',
            top: y + 5,
            left: x + 5
        }).appendTo("body").fadeIn(200);
    },
    ObtenerPlanes: function () {
        var ddata = [];

        $.ajax({
            type: "POST",
            url: "/Home/ObtenerPlanes",
            //data: "{ tiempo: " + tiempo + "}",
            async: false,//wait for result
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg, text) {
                var data = msg;
                if (data) {

                    for (i = 0; i < data.length; i++) {
                        ddataBa.push([data[i].Fecha, data[i].Basico]);
                    }

                    for (i = 0; i < data.length; i++) {
                        ddataPy.push([data[i].Fecha, data[i].Profesional]);
                    }

                    for (i = 0; i < data.length; i++) {
                        ddataPr.push([data[i].Fecha, data[i].Pyme]);
                    }

                    for (i = 0; i < data.length; i++) {
                        ddataEm.push([data[i].Fecha, data[i].Empresa]);
                    }

                    for (i = 0; i < data.length; i++) {
                        ddataIn.push([data[i].Fecha, data[i].Prueba]);
                    }
                }
            },
            error: function (response) {
                var r = jQuery.parseJSON(response.responseText);
                alert(r.Message);
            }
        });
    },

    ObtenerPlanesPorDias: function () {
        var data = home.ObtenerPlanesDias();

        new Morris.Line({
            element: 'BarUsuariosRegistradosDias',
            data: data,
            xkey: 'Fecha',
            xLabels: 'day',
            ykeys: ['Basico', 'Profesional', 'Pyme', 'Empresa', 'Prueba'],
            labels: ['Basico', 'Profesional', 'Pyme', 'Empresa', 'Prueba'],
            //lineColors: ['#D9534F', '#1CAF9A', '#1CAFFB', '#1CFF9A'],
            lineWidth: '2px',
            hideHover: true
        });
    },
    ObtenerPlanesDias: function () {
        var ddata = [];
        $.ajax({
            type: "POST",
            url: "/Home/ObtenerPlanesDias",
            //data: "{periodo: '" + periodo + "'}",
            async: false,//wait for result
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg, text) {
                var data = msg;

                for (i = 0; i < data.length; i++) {

                    var obj = new Object();
                    obj.Fecha = data[i].Fecha

                    obj.Basico = data[i].Basico
                    obj.Profesional = data[i].Profesional
                    obj.Pyme = data[i].Pyme
                    obj.Empresa = data[i].Empresa
                    obj.Prueba = data[i].Prueba
                    ddata.push(obj);
                }
            },
            error: function (response) {
                var r = jQuery.parseJSON(response.responseText);
                alert(r.Message);
            }
        });
        return ddata;
    },

    ObtenerFormasPago: function () {
        var data = home.ObtenerFormasDePago();

        new Morris.Line({
            element: 'BarFormasDePago',
            data: data,
            xkey: 'Fecha',
            xLabels: 'month',
            ykeys: ['MercadoPago', 'Transferencia'],
            labels: ['Mercado Pago', 'Transferencia'],
            //lineColors: ['#D9534F', '#1CAF9A'],
            lineWidth: '2px',
            hideHover: true
        });
    },
    ObtenerFormasDePago: function () {
        var ddata = [];
        $.ajax({
            type: "POST",
            url: "/Home/ObtenerFormasDePago",
            async: false,//wait for result
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg, text) {
                var data = msg;
                for (i = 0; i < data.length; i++) {
                    var obj = new Object();
                    obj.Fecha = data[i].Fecha
                    obj.MercadoPago = data[i].MercadoPago
                    obj.Transferencia = data[i].Transferencia
                    ddata.push(obj);
                }
            },
            error: function (response) {
                var r = jQuery.parseJSON(response.responseText);
                alert(r.Message);
            }
        });
        return ddata;
    },

    ObtenerFacturacion: function () {
        var data = home.ObtenerDatosFacturacion();

        new Morris.Line({
            element: 'BarFacturacion',
            data: data,
            xkey: 'Fecha',
            xLabels: 'month',
            ykeys: ['ImporteTotal'],
            labels: ['Importe Total'],
            lineColors: ['#1CAF9A'],
            lineWidth: '2px',
            hideHover: true
        });
    },
    ObtenerDatosFacturacion: function () {
        var ddata = [];
        $.ajax({
            type: "POST",
            url: "/Home/ObtenerFacturacion",
            async: false,//wait for result
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg, text) {
                var data = msg;
                for (i = 0; i < data.length; i++) {
                    var obj = new Object();
                    obj.Fecha = data[i].Fecha
                    obj.ImporteTotal = data[i].ImporteTotal
                    ddata.push(obj);
                }
            },
            error: function (response) {
                var r = jQuery.parseJSON(response.responseText);
                alert(r.Message);
            }
        });
        return ddata;
    },

    ObtenerFacturacionDias: function () {
        var data = home.ObtenerDatosFacturacionDias();

        new Morris.Line({
            element: 'BarFacturacionDias',
            data: data,
            xkey: 'Fecha',
            xLabels: 'day',
            ykeys: ['ImporteTotal'],
            labels: ['Importe Total'],
            lineColors: ['#1CAF9A'],
            lineWidth: '2px',
            hideHover: true
        });
    },
    ObtenerDatosFacturacionDias: function () {
        var ddata = [];
        $.ajax({
            type: "POST",
            url: "/Home/ObtenerFacturacionDias",
            async: false,//wait for result
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg, text) {
                var data = msg;
                for (i = 0; i < data.length; i++) {
                    var obj = new Object();
                    obj.Fecha = data[i].Fecha
                    obj.ImporteTotal = data[i].ImporteTotal
                    ddata.push(obj);
                }
            },
            error: function (response) {
                var r = jQuery.parseJSON(response.responseText);
                alert(r.Message);
            }
        });
        return ddata;
    },

    consultarUsuariosSegunPlan: function (plan, opcion) {

        var url = "/Usuario/Index?idPlan=" + plan + "&estado=" + opcion;
        window.open(url, '_blank');
        window.open(url);
    },
}