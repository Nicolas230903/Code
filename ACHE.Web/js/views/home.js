jQuery(document).ready(function () {
    if ($("#hdnPanelDeControl").val() == "1") {
        dashBoard.obtenerFacturasPendientes();
        dashBoard.ventasVsCompras();
        $("#PanelDeControl").show()
    }
    else {
        $("#PanelDeControl").hide()
    }
});

var dashBoard = {

    obtenerFacturasPendientes: function () {
        $.ajax({
            type: "GET",
            url: "home.aspx/obtenerFacturasPendientes",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data, text) {
                $("#ulTemplateContainer").html("");
                if (data.d.Items.length > 0) {
                    $("#templateFacturasPendientes").tmpl({ results: data.d.Items }).appendTo("#ulTemplateContainer");
                    $("#TotalComprobantes").html(data.d.TotalItems);
                }
                else {
                    $("#NotemplateFacturasPendientes").tmpl({ results: data.d.Items }).appendTo("#ulTemplateContainer");
                    $("#TotalComprobantes").html("0");
                }
            },
            error: function (response) {
                var r = jQuery.parseJSON(response.responseText);
                alert(r.Message);
            }
        });
    },
    verFacturasPendientes: function (id) {
        window.location.href = "/comprase.aspx?ID=" + id;
    },
    pagarFacturasPendientes: function (id) {
        window.location.href = "/pagose.aspx?IDCompra=" + id + "&Pago=100";
    },

    ventasVsCompras: function () {
        var data = dashBoard.obtenerventasVsCompras();

        new Morris.Line({
            element: 'line-chart',
            data: data,
            xkey: 'Fecha',
            ykeys: ['Compras', 'Ventas'],
            labels: ['Ventas', 'Compras'],
            lineColors: ['#2b9b8f', '#D9534F'],
            lineWidth: '2px',
            hideHover: true
        });
    },
    obtenerventasVsCompras: function (periodo) {
        var ddata = [];
        $.ajax({
            type: "POST",
            url: "/home.aspx/obtenerVentasVsCompras",
            data: "{periodo: '" + periodo + "'}",
            async: false,//wait for result
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (msg, text) {
                var data = msg.d.Items;
                for (i = 0; i < data.length; i++) {
                    var obj = new Object();
                    obj.Fecha = data[i].Fecha

                    //if (data[i].Uno != 0)
                    obj.Compras = data[i].Uno
                    //if (data[i].Dos != 0)
                    obj.Ventas = data[i].Dos

                    ddata.push(obj);
                }
            },
            error: function (response) {
                var r = jQuery.parseJSON(response.responseText);
                //alert(r.Message);
            }
        });
        return ddata;
    },

    pagarPlanActual: function () {
        //window.location.href = "/modulos/seguridad/pagoDePlanes.aspx?plan=" + $("#hdnNombrePlanActual").val() +"&modo=false";
        window.location.href = "/modulos/seguridad/elegir-plan.aspx?upgrade=0";
    },
}