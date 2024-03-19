var tipo = "";

function descargarModelo() {

    if ($("#ddlTipo").val() == "Clientes" || $("#ddlTipo").val() == "Proveedores") {
        $("#DescargarModelo").attr("href", "/ayuda/Modelo/personas.csv");
    } else if ($("#ddlTipo").val() == "Productos" || $("#ddlTipo").val() == "Servicios") {
        $("#DescargarModelo").attr("href", "/ayuda/Modelo/productos.csv");
    } else if ($("#ddlTipo").val() == "PlanDeCuentas") {
        $("#DescargarModelo").attr("href", "/ayuda/Modelo/PlanDeCuentas.csv");
    } else if ($("#ddlTipo").val() == "Facturas") {
        $("#DescargarModelo").attr("href", "/ayuda/Modelo/Facturas.csv");
    }
    else {
        $("#DescargarModelo").attr("href", "/ayuda/Modelo/ListaPrecios.csv");
    }
}

function descargarEjemplo() {
    $("#DescargarEjemplo").attr("href", "/ayuda/Ejemplo/" + $("#ddlTipo").val() + ".csv");
}

function ocultarMensajes() {
    $("#divError, #divOk, #divErrorCat").hide();
}

function importar() {
    
    ocultarMensajes();
    Common.mostrarProcesando("btnContinuar");

    if ($('#frmEdicion').valid()) {

        var nombre = $("#hdnFileName").val();
        if (nombre != "") {
            $("#imgLoading").show();
            Common.ocultarProcesando("btnContinuar", "Continuar");
            leerArchivoCSV(nombre);
            return false;
        }
        else {
            $("#msgError").html("Extensión no permitida. El archivo debe ser CSV");
            $("#divError").show();
            $("#divOk").hide();
            Common.ocultarProcesando("btnContinuar", "Continuar");

            return false;
        }
    }
    else {
        Common.ocultarProcesando("btnContinuar", "Continuar");
        return false;
    }
}

function configForm() {

    $('#ddlPeriodo').css({
        'height': '40px',
        'width': '250px',
        'background-color': '#F0F0F0',
        'color': '#333333',
        'border': '1px solid #cccccc',
        'border-radius': '10px'
    });


    $('#flpArchivo').fileupload({
        url: 'importarDatos.ashx?upload=start',
        success: function (response, status) {
            if (response.name != "ERROR")
                $("#hdnFileName").val(response.name);
            else {
                $("#hdnFileName").val("");
                $("#msgError").html("Extensión no permitida. El archivo debe ser CSV");
                $("#divError").show();
                $("#divOk").hide();
            }
        },
        error: function (error) {
            $("#hdnFileName").val("");
        }
    });

    $("#ddlTipo").change(function () {
        if (this.value == "") {
            $("#divArchivos").hide();
        }
        else {
            $("#divArchivos").show();
        }
    });

    $("#frmEdicion").validate({
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

    if ($("#ddlTipo").val() == "ListaPrecios") {
        $("#msgListaPrecios").show();
        CargarLotes();
        $('#ddlPeriodoContainer').show(); // Asegúrate de que este ID sea el correcto para tu segundo DropDownList
        $('#infoAlerta').show();
    }
    else {
    $("#msgListaPrecios").hide();
    $('#ddlPeriodoContainer').hide();
    $('#infoAlerta').hide();
    }
}

function eliminarConcepto(id) {
    var info = "{ idUsuario: " + parseInt($("#IDusuario").val()) + "}";

    $.ajax({
        type: "POST",
        url: "importar/eliminarBackUp",
        contentType: "application/json; charset=utf-8",
        data: info,
        dataType: "json",
        success: function (data, text) {
            $("#divOkEliminarConcepto").show();
        },
        error: function (response) {
            var r = jQuery.parseJSON(response.responseText);
            $("#msgErrorEliminarConcepto").html(r.Message);
            $("#divErrorEliminarConcepto").show();
        }
    });
}

function CargarLotes()
{
    $.ajax({
        type: "POST",
        url: "importar.aspx/LlenarDropDownListLotes",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (response) {
            var select = $("#ddlPeriodo"); // Asegúrate de que este es el ID correcto
            select.empty();
            $.each(response.d, function (key, item) {
                select.append($('<option></option>').val(item.Value).html(item.Text));
            });
        },
        error: function (response) {
            var r = jQuery.parseJSON(response.responseText);
            $("#msgError").html(r.Message);
            $("#divError").show();
            $('html, body').animate({ scrollTop: 0 }, 'slow');
        }
    });
}

function DescargarLote() {

  

    if ($("#ddlPeriodo").val() != "") {

        var nroLote = $('#ddlPeriodo').val();
        var info = JSON.stringify({ NroLote: nroLote });

        $.ajax({
            type: "POST",
            url: "/importar.aspx/ExportLote",
            data: info,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (response.d != null) {

                    var downloadUrl = response.d;

                    window.location.href = downloadUrl;

                    //setTimeout(function () {
                    //    $('#ddlPeriodo').val($('#ddlPeriodo option:first').val()).trigger('change');
                    //}, 1000);
                }
                else {
                    $("#msgError").html("Error al descargar el archivo. Por favor, intente nuevamente.");
                    $("#divError").show();
                    $('html, body').animate({ scrollTop: 0 }, 'slow');
                }
            },
            error: function (response) {
                var r = jQuery.parseJSON(response.responseText);
                $("#msgError").html(r.Message);
                $("#divError").show();
                $('html, body').animate({ scrollTop: 0 }, 'slow');
                
            }

        });

       
    }

}

function leerArchivoCSV(nombre) {
   
    tipo = $("#ddlTipo").val();

    var idLista = ($("#hdnIDLista").val() == "" || $("#hdnIDLista").val() == null) ? "0" : $("#hdnIDLista").val();
    var info = "";
    var URL = "";

    if (tipo == "Productos" || tipo == "Servicios") {
        var info = "{ nombre: '" + nombre + "', tipo:'" + tipo + "'}";
        URL = "importar.aspx/leerArchivoCSVProductos";
    }
    else if (tipo == "Clientes" || tipo == "Proveedores") {
        var info = "{ nombre: '" + nombre + "', tipo:'" + tipo + "'}";
        URL = "importar.aspx/leerArchivoCSVPersonas";
    }
    else if (tipo == "ListaPrecios") {
        var info = "{ nombre: '" + nombre + "', idLista:" + idLista + "}";
        URL = "importar.aspx/leerArchivoCSVListaPrecios";
    }
    else if (tipo == "PlanDeCuentas") {
        var info = "{ nombre: '" + nombre + "', idLista:" + idLista + "}";
        URL = "importar.aspx/leerArchivoCSVPlanDeCuentas";
    }
    else if (tipo == "Facturas") {
        var info = "{ nombre: '" + nombre + "'}";
        URL = "importar.aspx/leerArchivoCSVFacturas";
    }

    $("#resultThead").empty();
    $("#btnContinuar").attr("disabled", true);

    $.ajax({
        type: "POST",
        url: URL,
        data: info,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data, text) {
            
            if (tipo == "Productos" || tipo == "Servicios") {
                ArmarTablaResultadosProductos(data.d)
            }
            else if (tipo == "Clientes" || tipo == "Proveedores") {
                ArmarTablaResultadosPersonas(data.d)
            }
            else if (tipo == "ListaPrecios") {
                ArmarTablaResultadosListaPrecios(data.d)
            }
            else if (tipo == "PlanDeCuentas") {
                ArmarTablaResultadosPlanDeCuentas(data.d)
            }
            else if (tipo == "Facturas") {
                ArmarTablaResultadosFacturas(data.d)
            }

            $('[data-toggle="tooltip"]').tooltip({ 'placement': 'bottom' });
            $("#imgLoading").hide();
            $("#btnContinuar").attr("disabled", false);
        },
        error: function (response) {
            
            $("#imgLoading").hide();
            $("#btnContinuar").attr("disabled", false);
            var r = jQuery.parseJSON(response.responseText);

            $("#msgError").html(r.Message.toString());
            $("#divError").show();
            $("#divOk").hide();
            return false;
        }
    });
}

function ArmarTablaResultadosProductos(mtxProd) {
    var totalOK = 0;
    $("#resultThead").empty();
    $("#resultThead").append("<thead><tr><th>Resultado</th><th>Codigo</th><th>Nombre</th><th>Descripcion</th><th>Observaciones</th><th>Precio Unitario</th><th>Costo Interno</th><th>Stok</th><th>Stok Minimo</th><th>Iva</th><th>Codigo Proveedor</th></tr></thead>");
    $("#resultThead").append("<tbody id='resultsContainer'></tbody>");
    //$("#resultsContainer").empty();
    for (var i = 0; i < mtxProd.length; i++) {

        if (mtxProd[i].Estado == "A")
        { totalOK++ }

        $("#resultsContainer").append("<tr><td>" + mtxProd[i].resultados + "</td><td>" + mtxProd[i].codigo + "</td><td>" + mtxProd[i].nombre + "</td><td>" + mtxProd[i].descripcion + "</td><td>" + mtxProd[i].observaciones + "</td><td>" + mtxProd[i].precioUnitario + "</td><td>" + mtxProd[i].CostoInterno + "</td><td>" + mtxProd[i].stock + "</td><td>" + mtxProd[i].StockMinimo + "</td><td>" + mtxProd[i].iva + "</td><td>" + mtxProd[i].CodigoProveedor + "</td></tr>");
    }

    if (totalOK == 0) {
        $("#divIconoDescargarOK").hide();
    }
    else {
        $("#divIconoDescargarOK").show();
    }

    $("#msjResultados").html("Cantidad de " + tipo + " a importar " + totalOK + "/" + mtxProd.length + "- Presione importar para continuar o bien corrija los cambios e intente nuevamente");
    $("#ContResultados").show();
}

function ArmarTablaResultadosPersonas(mtxPersonas) {
    var totalOK = 0;
    $("#resultThead").empty();
    $("#resultThead").append("<thead><tr><th>Resultado</th><th>Código</th><th>RazonSocial</th><th>NombreFantasia</th><th>TipoDocumento</th><th>NroDocumento</th><th>CondicionIva</th>"
        + "<th>Telefono</th><th>Celular</th><th>Web</th><th>Email</th><th>Provincia</th><th>Ciudad</th><th>Domicilio</th>"
        + "<th>PisoDepto</th><th>CodigoPostal</th><th>EmailsEnvioFc</th><th>Personeria</th><th>AlicuotaIvaDefecto</th><th>CBU</th>"
        + "<th>Banco</th><th>Contacto</th></tr></thead>");
    $("#resultThead").append("<tbody id='resultsContainer'></tbody>");
    //$("#resultsContainer").empty();
    for (var i = 0; i < mtxPersonas.length; i++) {

        if (mtxPersonas[i].Estado == "A")
        { totalOK++ }

        $("#resultsContainer").append("<tr><td>" + mtxPersonas[i].resultados + "</td><td>" + mtxPersonas[i].Codigo + "</td><td>" + mtxPersonas[i].RazonSocial + "</td><td>" + mtxPersonas[i].NombreFantansia + "</td><td>" + mtxPersonas[i].TipoDocumento + "</td><td>" + mtxPersonas[i].NroDocumento + "</td><td>" + mtxPersonas[i].CondicionIva + "</td>"
                                    + "<td>" + mtxPersonas[i].Telefono + "</td><td>" + mtxPersonas[i].Celular + "</td><td>" + mtxPersonas[i].Web + "</td><td>" + mtxPersonas[i].Email + "</td><td>" + mtxPersonas[i].Provincia + "</td><td>" + mtxPersonas[i].Ciudad + "</td><td>" + mtxPersonas[i].Domicilio + "</td>"
                                    + "<td>" + mtxPersonas[i].PisoDepto + "</td><td>" + mtxPersonas[i].CodigoPostal + "</td><td>" + mtxPersonas[i].EmailsEnvioFc + "</td><td>" + mtxPersonas[i].Personeria + "</td><td>" + mtxPersonas[i].AlicuotaIvaDefecto + "</td><td>" + mtxPersonas[i].CBU + "</td>"
                                    + "<td>" + mtxPersonas[i].Banco + "</td><td>" + mtxPersonas[i].Contacto + "</td></tr>");
    }

    if (totalOK == 0) {
        $("#divIconoDescargarOK").hide();
    }
    else {
        $("#divIconoDescargarOK").show();
    }

    $("#msjResultados").html("Cantidad de " + tipo + " a importar " + totalOK + "/" + mtxPersonas.length + "- Presione importar para continuar o bien corrija los cambios e intente nuevamente");
    $("#ContResultados").show();
}

function ArmarTablaResultadosListaPrecios(mtxPersonas) {
    var totalOK = 0;
    $("#resultThead").empty();
    $("#resultThead").append("<thead><tr><th>Resultado</th><th>Código</th><th>Costo</th><th>Precio</th><th>Stock</th></tr></thead>");
    $("#resultThead").append("<tbody id='resultsContainer'></tbody>");
    //$("#resultsContainer").empty();
    for (var i = 0; i < mtxPersonas.length; i++) {

        if (mtxPersonas[i].Estado == "A")
        { totalOK++ }

        $("#resultsContainer").append("<tr><td>" + mtxPersonas[i].resultados + "</td><td>" + mtxPersonas[i].Codigo + "</td><td>" + mtxPersonas[i].Costo + "</td><td>" + mtxPersonas[i].Precio + "</td><td>" + mtxPersonas[i].Stock + "</td></tr>");
    }

    if (totalOK == 0) {
        $("#divIconoDescargarOK").hide();
    }
    else {
        $("#divIconoDescargarOK").show();
    }

    $("#msjResultados").html("Cantidad de Precios a importar " + totalOK + "/" + mtxPersonas.length + "- Presione importar para continuar o bien corrija los cambios e intente nuevamente");
    $("#ContResultados").show();
}

function ArmarTablaResultadosPlanDeCuentas(mtxPlanDeCuentas) {
    var totalOK = 0;
    $("#resultThead").empty();
    $("#resultThead").append("<thead><tr><th>Resultado</th><th>Código</th><th>Nombre</th><th>AdmiteAsientoManual</th><th>TipoDeCuenta</th><th>CodigoPadre</th></tr></thead>");
    $("#resultThead").append("<tbody id='resultsContainer'></tbody>");
    for (var i = 0; i < mtxPlanDeCuentas.length; i++) {

        if (mtxPlanDeCuentas[i].Estado == "A")
        { totalOK++ }

        $("#resultsContainer").append("<tr><td>" + mtxPlanDeCuentas[i].resultados + "</td><td>" + mtxPlanDeCuentas[i].Codigo + "</td><td>" + mtxPlanDeCuentas[i].Nombre + "</td><td>" + mtxPlanDeCuentas[i].AdmiteAsientoManual + "</td><td>" + mtxPlanDeCuentas[i].TipoDeCuenta + "</td><td>" + mtxPlanDeCuentas[i].CodigoPadre + "</td></tr>");
    }

    if (totalOK == 0) {
        $("#divIconoDescargarOK").hide();
    }
    else {
        $("#divIconoDescargarOK").show();
    }

    $("#msjResultados").html("Cantidad de Precios a importar " + totalOK + "/" + mtxPlanDeCuentas.length + "- Presione importar para continuar o bien corrija los cambios e intente nuevamente");
    $("#ContResultados").show();
}

function ArmarTablaResultadosFacturas(mtxFacturas) {
    var totalOK = 0;
    $("#resultThead").empty();
    $("#resultThead").append("<thead><tr><th>Resultado</th><th>Razón social</th><th>Email</th><th>Tipo documento</th><th>Nro. documento</th><th>Condicion IVA</th><th>Provincia</th><th>Fecha</th><th>TipoComprobante</th><th>PuntoDeVenta</th><th>NroComprobante</th><th>CAE</th><th>ImporteNeto</th><th>ImporteNoGravado</th><th>IVA 27,00%</th><th>IVA 21,00%</th><th>IVA 10,50%</th><th>IVA 05,00%</th><th>IVA 02,50%</th><th>IVA 00,00%</th><th>Percepciones IVA</th><th>Percepciones IIBB</th><th>Total</th><th>Codigo Cuenta Contable</th><th>Observaciones</th> </tr></thead>");
    $("#resultThead").append("<tbody id='resultsContainer'></tbody>");
    for (var i = 0; i < mtxFacturas.length; i++) {

        if (mtxFacturas[i].Estado == "A")
        { totalOK++ }

        $("#resultsContainer").append("<tr><td>" + mtxFacturas[i].resultados + "</td><td>" + mtxFacturas[i].RazonSocial + "</td><td>" + mtxFacturas[i].Email + "</td><td>" + mtxFacturas[i].TipoDocumento + "</td><td>" + mtxFacturas[i].NroDocumento + "</td><td>" + mtxFacturas[i].CondicionIva + "</td><td>" + mtxFacturas[i].Provincia + "</td><td>" + mtxFacturas[i].Fecha + "</td><td>" + mtxFacturas[i].TipoComprobante + "</td><td>" + mtxFacturas[i].PuntoDeVenta + "</td><td>" + mtxFacturas[i].NroComprobante + "</td><td>" + mtxFacturas[i].CAE + "</td><td>" + mtxFacturas[i].ImporteNeto + "</td><td>" + mtxFacturas[i].ImporteNoGravado + "</td><td>" + mtxFacturas[i].IVA2700 + "</td><td>" + mtxFacturas[i].IVA2100 + "</td><td>" + mtxFacturas[i].IVA1005 + "</td><td>" + mtxFacturas[i].IVA0500 + "</td><td>" + mtxFacturas[i].IVA0205 + "</td><td>" + mtxFacturas[i].IVA0000 + "</td><td>" + mtxFacturas[i].PercepcionesIVA + "</td><td>" + mtxFacturas[i].PercepcionesIIBB + "</td><td>" + mtxFacturas[i].Total + "</td><td>" + mtxFacturas[i].CodigoCuentaContable + "</td><td>" + mtxFacturas[i].Observaciones + "</td></tr>");
    }

    if (totalOK == 0) {
        $("#divIconoDescargarOK").hide();
    }
    else {
        $("#divIconoDescargarOK").show();
    }

    $("#msjResultados").html("Cantidad de Precios a importar " + totalOK + "/" + mtxFacturas.length + "- Presione importar para continuar o bien corrija los cambios e intente nuevamente");
    $("#ContResultados").show();
}

function importarDatos() {
    var URL = "";
    if (tipo == "Productos" || tipo == "Servicios") {
        URL = "importar.aspx/RealizarImportacionProductos";
    }
    else if (tipo == "Clientes" || tipo == "Proveedores") {
        URL = "importar.aspx/RealizarImportacionPersonas";
    }
    else if (tipo == "ListaPrecios") {
        URL = "importar.aspx/RealizarImportacionListaPrecios";
    }
    else if (tipo == "PlanDeCuentas") {
        URL = "importar.aspx/RealizarImportacionPlanDeCuentas";
    }
    else if (tipo == "Facturas") {
        URL = "importar.aspx/RealizarImportacionFacturas";
    }

    if ($("#hdnTieneCuentasContables").val() == "1" && tipo == "PlanDeCuentas") {
        bootbox.confirm("Al importar este plan de cuentas eliminara todos los datos anteriores <br/>¿Está seguro que desea eliminar el plan de cuentas actual?", function (result) {
            if (result) {
                importarCSV(URL);
            }
        });
    }
    else {
        importarCSV(URL);
    }
    //recargar combo
    CargarLotes();
}

function importarCSV(URL) {
    $.blockUI({ message: $('#divEspera') });
    $("#divbtnImportar").attr("disabled", true);
    $("#imgLoading2").show();

    $.ajax({
        type: "GET",
        url: URL,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data, text) {
            $.unblockUI();
            $("#resultsContainer").empty();
            $("#divOk").show();
            $("#imgLoading2,#ContResultados").hide();
            $("#divbtnImportar").attr("disabled", false);
        },
        error: function (response) {
            $.unblockUI();
            $("#imgLoading2,#divOk").hide();
            $("#divbtnImportar").attr("disabled", false);
            var r = jQuery.parseJSON(response.responseText);
            $("#msgError").html(r.Message.toString());
            $("#divError").show();
            return false;
        }
    });
}