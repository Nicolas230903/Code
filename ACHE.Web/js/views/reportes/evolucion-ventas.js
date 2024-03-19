var evolucionVentas = {

    configForm: function () {
        Common.obtenerPersonas("ddlPersona", "", true);

        $(".select2").select2({ width: '100%', allowClear: true });
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

        evolucionVentas.obtenerResultados();
    },

    obtenerResultados: function () {


        var cliente = evolucionVentas.ObtenerProductos();      
        var ticks = evolucionVentas.ObtenerTicks();

        var plot3 = jQuery.plot("#trackingchart", [
            { data: cliente, label: "cos(x) = -0.00", color: '#888' }
        ], {
            series: {
                lines: {
                    show: true,
                    lineWidth: 2,
                },
                shadowSize: 0
            },
            legend: {
                show: false
            },
            crosshair: {
                mode: "xy",
                color: '#D9534F'
            },
            grid: {
                borderColor: '#ddd',
                borderWidth: 1,
                labelMargin: 10
            },
            yaxis: {
                color: '#eee'
            },
            xaxis: {
                color: '#eee',
                ticks: ticks
            }
        });

        var legends = jQuery("#trackingchart .legendLabel");

        legends.each(function () {
            // fix the widths so they don't jump around
            jQuery(this).css('width', jQuery(this).width());
        });

        var updateLegendTimeout = null;
        var latestPosition = null;

        function updateLegend() {

            updateLegendTimeout = null;

            var pos = latestPosition;

            var axes = plot3.getAxes();
            if (pos.x < axes.xaxis.min || pos.x > axes.xaxis.max ||
                  pos.y < axes.yaxis.min || pos.y > axes.yaxis.max) {
                return;
            }

            var i, j, dataset = plot3.getData();
            for (i = 0; i < dataset.length; ++i) {

                var series = dataset[i];

                // Find the nearest points, x-wise
                for (j = 0; j < series.data.length; ++j) {
                    if (series.data[j][0] > pos.x) {
                        break;
                    }
                }

                // Now Interpolate
                var y,
					p1 = series.data[j - 1],
					p2 = series.data[j];

                if (p1 == null) {
                    y = p2[1];
                } else if (p2 == null) {
                    y = p1[1];
                } else {
                    y = p1[1] + (p2[1] - p1[1]) * (pos.x - p1[0]) / (p2[0] - p1[0]);
                }

                legends.eq(i).text(series.label.replace(/=.*/, "= " + y.toFixed(2)));
            }
        }

        jQuery("#trackingchart").bind("plothover", function (event, pos, item) {
            latestPosition = pos;
            if (!updateLegendTimeout) {
                updateLegendTimeout = setTimeout(updateLegend, 50);
            }
        });

        $("#trackingchart").bind("plotclick", function (event, pos, item) {
            if (item) {
                //evolucionVentas.verDetalle(item.series.label, (item.dataIndex));
                alert("yep yep yep");
            }
        });
    },

    ObtenerProductos: function () {
        var ddata = [];

        if ($("#ddlPersona").val() != null && $("#ddlPersona").val() != "") {
            var info = "{idPersona: " + $("#ddlPersona").val() + ", desde: '" + $("#txtFechaDesde").val() + "', hasta:'" + $("#txtFechaHasta").val() + "'}";

            $.ajax({
                type: "POST",
                url: "evolucion-ventas.aspx/ObtenerProductos",
                data: info,
                async: false,//wait for result
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (msg, text) {
                    var data = msg.d;
                    for (i = 0; i < data.length; i++) {
                        ddata.push([data[i].fecha, data[i].data]);
                    }
                },
                error: function (response) {
                    var r = jQuery.parseJSON(response.responseText);
                    alert(r.Message);
                }
            });
        }
        return ddata;
    },

    ObtenerTicks: function () {
        var ddata = [];

        var info = "{idPersona: " + $("#ddlPersona").val() + ", desde: '" + $("#txtFechaDesde").val() + "', hasta:'" + $("#txtFechaHasta").val() + "'}";
        if ($("#ddlPersona").val() != null && $("#ddlPersona").val() != "") {
            $.ajax({
                type: "POST",
                url: "evolucion-ventas.aspx/ObtenerTicks",
                data: info,
                async: false,//wait for result
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (msg, text) {
                    var data = msg.d;
                    for (i = 0; i < data.length; i++) {
                        ddata.push([data[i].data, data[i].label]);
                    }
                },
                error: function (response) {
                    var r = jQuery.parseJSON(response.responseText);
                    alert(r.Message);
                }
            });
        }
        return ddata;
    }
}