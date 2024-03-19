function configForm() {

    $(".select2").select2({ width: '100%', allowClear: true });        
    Common.obtenerProveedores("ddlPersona", "", true);
    Common.obtenerProveedores("ddlPersonaProceso", "", true);
    Common.obtenerCodigosCompraAutomatica("ddlCodigoProceso", "", true);
}


function changeProveedor() {
    if ($("#ddlPersona").val() != "") {
        obtenerPedidosDeVentaPendientesDeProcesar($("#ddlPersona").val());
    }
    else {
        obtenerPedidosDeVentaPendientesDeProcesar(0);
    }
}

function obtenerPedidosDeVentaPendientesDeProcesar(idProveedor) {

    $.ajax({
        type: "POST",
        url: "compraAutomatica.aspx/obtenerPedidosDeVentaPendientesDeProcesar",
        data: "{idProveedor:" + idProveedor + "}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            if (data != null) {
                $("#resultsPedidosPendientesDeProcesar").html(data.d);
            }
        }
    });
}

function changeCodigoProceso() {
    //if ($("#ddlCodigoProceso").val() != "" || $("#ddlPersonaProceso").val() != "") {
    //    obtenerPedidosDeVentaProcesados($("#ddlCodigoProceso").val(), $("#ddlPersonaProceso").val());
    //}
    var codProceso = ($("#ddlCodigoProceso").val() == "" || $("#ddlCodigoProceso").val() == null) ? 0 : parseInt($("#ddlCodigoProceso").val());
    var codPersona = ($("#ddlPersonaProceso").val() == "" || $("#ddlPersonaProceso").val() == null) ? 0 : parseInt($("#ddlPersonaProceso").val());

    obtenerPedidosDeVentaProcesados(codProceso, codPersona);
    //else {
    //    obtenerPedidosDeVentaProcesados(0);
    //}
}

function obtenerPedidosDeVentaProcesados(codigoProceso,idPersona) {
    $.blockUI({ message: $('#divEspera') });
    document.body.style.cursor = 'wait';
    $.ajax({
        type: "POST",
        url: "compraAutomatica.aspx/obtenerPedidosDeVentaProcesados",
        data: "{procesoCompraAutomatica:" + codigoProceso + ", idPersona: " + idPersona + "}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            $.unblockUI();
            document.body.style.cursor = 'default';
            if (data != null) {
                $("#resultsPedidosDeVentaProcesados").html(data.d);
            }
        },
        error: function (response) {
            $.unblockUI();
            document.body.style.cursor = 'default';
            var r = jQuery.parseJSON(response.responseText);
            $("#msgError").html(r.Message);
            $("#divError").show();
            $("#divOk").hide();
        }
    });
}

$("#checkAll").change(function () {
    $('.chkTodos').not(this).prop('checked', this.checked);
});

function tableToArray(table) {
    var result = []
    var rows = table.rows;
    var cells, t;

    // Iterate over rows
    for (var i = 0, iLen = rows.length; i < iLen; i++) {
        cells = rows[i].cells;
        t = [];

        // Iterate over cells
        for (var j = 0, jLen = cells.length; j < jLen; j++) {
            t.push(cells[j].textContent);
        }
        result.push(t);
    }
    return result;
}

function procesar(fechaEntrega) {
    $('#divOk').hide();
    $("#divError").hide();
    var Contador = 0;
    var idSeleccionados = [];
    $('#tablaPedidosDeVentaPendientesDeProcesar tr').each(function () {
        if ($(this).find("td").eq(0).find("input").prop('checked')) {
            Contador = + 1;
            var idComprobante = $(this).find("td").eq(1).html();
            idSeleccionados = idSeleccionados.concat(idComprobante);
        }
    });
    if (Contador > 0) {
        $.ajax({
            type: "POST",
            url: "compraAutomatica.aspx/procesar",
            data: "{ id: " + JSON.stringify(idSeleccionados) + ", fechaEntrega: '" + fechaEntrega + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data, text) {
                $("#msgOk").html("Se genero el lote " + data.d);
                $('#divOk').show();
                $("#divError").hide();
                setTimeout('document.location.reload()', 3000);
            },
            error: function (response) {
                var r = jQuery.parseJSON(response.responseText);
                $("#msgError").html(r.Message);
                $("#divError").show();
                $("#divOk").hide();
            }
        });
    }    
}

function verificarCantidadDeProveedores() {
    $('#divOk').hide();
    $("#divError").hide();
    var Contador = 0;
    var idSeleccionados = [];
    $('#tablaPedidosDeVentaPendientesDeProcesar tr').each(function () {
        if ($(this).find("td").eq(0).find("input").prop('checked')) {
            Contador = + 1;
            var idComprobante = $(this).find("td").eq(1).html();
            idSeleccionados = idSeleccionados.concat(idComprobante);
        }
    });
    if (Contador > 0) {
        $.ajax({
            type: "POST",
            url: "compraAutomatica.aspx/obtenerCantidadDeProveedoresDeLaSeleccion",
            data: "{ id: " + JSON.stringify(idSeleccionados) + "}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data, text) {
                const dateObj = new Date();
                var mes = dateObj.getMonth() + 1;
                const month = String(mes).padStart(2, '0');
                const day = String(dateObj.getDate()).padStart(2, '0');
                const year = dateObj.getFullYear();
                const output = year + '-' + month + '-' + day;
                if (data.d > 1) {
                    bootbox.confirm("Más de un proveedor en los Pedidos de venta seleccionados ¿continuar?", function (result) {
                        if (result) {
                            bootbox.prompt({
                                title: "Ingrese una fecha de entrega",
                                inputType: 'date',
                                value: output,
                                callback: function (result) {
                                    if (result != null) {
                                        procesar(result);
                                    }                                   
                                }
                            });  
                        }
                    });
                } else {
                    bootbox.prompt({
                        title: "Ingrese una fecha de entrega",
                        inputType: 'date',
                        value: output,
                        callback: function (result) {
                            if (result != null) {
                                procesar(result);
                            }      
                        }
                    });  
                }
            },
            error: function (response) {
                var r = jQuery.parseJSON(response.responseText);
                $("#msgError").html(r.Message);
                $("#divError").show();
                $("#divOk").hide();
            }
        });
    }
}


function eliminar(id, nombre) {
    bootbox.confirm("¿Está seguro que desea eliminar las compras del pedido " + nombre + "?", function (result) {
        if (result) {
            $.ajax({
                type: "POST",
                url: "compraAutomatica.aspx/delete",
                data: "{ id: " + id + "}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data, text) {
                    $("#msgOk").html("Se eliminaron los pedidos seleccionados");
                    $('#divOk').show();
                    $("#divError").hide();
                    setTimeout('document.location.reload()', 2000);
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
}

function editar(id,tipo) {
    window.location.href = "/comprobantese.aspx?tipo=" + tipo + "&ID=" + id;
}

function documentosVinculados(id) {
    comprobantesVinculados(id);
}

function comprobantesVinculados(id) {

    $("#divErrorPedidoDeVenta").hide();
    $('#modalComprobantesVinculados').modal('hide');

    $("#resultsContainerPedidoDeVenta").html("");

    $.ajax({
        type: "POST",
        url: "compraAutomatica.aspx/getResultsPedidoDeVenta",
        data: "{ id: " + id + "}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data, text) {
            $('#modalComprobantesVinculados').modal('show');
            // Render using the template
            if (data.d.Items.length > 0)
                $("#resultTemplatePedidoDeVenta").tmpl({ results: data.d.Items }).appendTo("#resultsContainerPedidoDeVenta");
            else
                $("#noResultTemplatePedidoDeVenta").tmpl({ results: data.d.Items }).appendTo("#resultsContainerPedidoDeVenta");

        },
        error: function (response) {
            var r = jQuery.parseJSON(response.responseText);
            $("#msgErrorPedidoDeVenta").html(r.Message);
            $("#divErrorPedidoDeVenta").show();
            $('html, body').animate({ scrollTop: 0 }, 'slow');
        }
    });

}