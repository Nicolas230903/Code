var tipo = "";

function descargarModelo() {

    if ($("#ddlTipo").val() == "Clientes" || $("#ddlTipo").val() == "Proveedores") {
        $("#DescargarModelo").attr("href", "/ayuda/Modelo/personas.csv");
    } else if ($("#ddlTipo").val() == "Productos" || $("#ddlTipo").val() == "Servicios") {
        $("#DescargarModelo").attr("href", "/ayuda/Modelo/productos.csv");
    } else {
        $("#DescargarModelo").attr("href", "/ayuda/Modelo/ListaPrecios.csv");
    }
}

function descargarEjemplo() {
    $("#DescargarEjemplo").attr("href", "/ayuda/Ejemplo/" + $("#ddlTipo").val() + ".csv");
}

function ocultarMensajes() {
    $("#divError, #divOk, #divErrorCat").hide();
}

function importar(idUsuario) {
    ocultarMensajes();
    Common.mostrarProcesando("btnContinuar");

    if ($('#frmEdicion').valid()) {

        var nombre = $("#hdnFileName").val();
        if (nombre != "") {
            $("#imgLoading").show();
            Common.ocultarProcesando("btnContinuar", "Continuar");
            leerArchivoCSV(nombre, idUsuario);
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

    //$('.file').fileinput()

    $('#flpArchivo').fileupload({
        url: '/importarDatos.ashx?upload=start',
        /*add: function (e, data) {
            console.log('add', data);
            $('#progressbar').show();
            data.submit();
        },
        progress: function (e, data) {
            var progress = parseInt(data.loaded / data.total * 100, 10);
            $('#progressbar div').css('width', progress + '%');
        },*/
        success: function (response, status) {
            if (response.name != "ERROR")
                $("#hdnFileName").val(response.name);
            else {
                $("#hdnFileName").val("");
                $("#msgError").html("Extensión no permitida. El archivo debe ser CSV");
                $("#divError").show();
                $("#divOk").hide();
            }
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
}

function leerArchivoCSV(nombre,idUsuario) {

    tipo = $("#ddlTipo").val();

    var info = "";
    var URL = "";

    if (tipo == "Productos" || tipo == "Servicios") {
        var info = "{ nombre: '" + nombre + "', tipo:'" + tipo + "', idUsuario: " + idUsuario + "}";
        URL = "/Importaciones/leerArchivoCSVProductos";
    }
    else if (tipo == "Clientes" || tipo == "Proveedores") {
        var info = "{ nombre: '" + nombre + "', tipo:'" + tipo + "', idUsuario: " + idUsuario + "}";
        URL = "/Importaciones/leerArchivoCSVPersonas";
    }

    $("#resultThead").empty();
    $("#btnContinuar").attr("disabled", true);

    $.ajax({
        type: "POST",
        url: URL,
        data: info,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            if (data != false) {
                if (tipo == "Productos" || tipo == "Servicios") {
                    ArmarTablaResultadosProductos(data)
                }
                else if (tipo == "Clientes" || tipo == "Proveedores") {
                    ArmarTablaResultadosPersonas(data)
                }
                else if (tipo == "ListaPrecios") {
                    ArmarTablaResultadosListaPrecios(data)
                }

                $('[data-toggle="tooltip"]').tooltip({ 'placement': 'bottom' });
                $("#imgLoading").hide();
                $("#btnContinuar").attr("disabled", false);
            }
            else {
                alert("Error");
            }
        }
    });
}

function ArmarTablaResultadosProductos(mtxProd) {
    var totalOK = 0;
    $("#resultThead").empty();
    $("#resultThead").append("<thead><tr><th>Resultado</th><th>Codigo</th><th>Nombre</th><th>Descripcion</th><th>Observaciones</th><th>Precio Unitario</th><th>Costo Interno</th><th>Stok</th><th>Stok Minimo</th><th>Iva</th></tr></thead>");
    $("#resultThead").append("<tbody id='resultsContainer'></tbody>");
    //$("#resultsContainer").empty();
    for (var i = 0; i < mtxProd.length; i++) {

        if (mtxProd[i].Estado == "A")
        { totalOK++ }

        $("#resultsContainer").append("<tr><td>" + mtxProd[i].resultados + "</td><td>" + mtxProd[i].codigo + "</td><td>" + mtxProd[i].nombre + "</td><td>" + mtxProd[i].descripcion + "</td><td>" + mtxProd[i].observaciones + "</td><td>" + mtxProd[i].precioUnitario + "</td><td>" + mtxProd[i].CostoInterno + "</td><td>" + mtxProd[i].stock + "</td><td>" + mtxProd[i].StockMinimo + "</td><td>" + mtxProd[i].iva + "</td></tr>");
    }

    if (totalOK == 0) {
        $("#divIconoDescargarOK").hide();
    }
    else {
        $("#divIconoDescargarOK").show();
    }

    $("#msjResultados").html("Cantidad de productos a importar " + totalOK + "/" + mtxProd.length + "- Presione importar para continuar o bien corrija los cambios e intente nuevamente");
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

    $("#msjResultados").html("Cantidad de productos a importar " + totalOK + "/" + mtxPersonas.length + "- Presione importar para continuar o bien corrija los cambios e intente nuevamente");
    $("#ContResultados").show();
}

function ArmarTablaResultadosListaPrecios(mtxPersonas) {
    var totalOK = 0;
    $("#resultThead").empty();
    $("#resultThead").append("<thead><tr><th>Resultado</th><th>Código</th><th>Precio</th></tr></thead>");
    $("#resultThead").append("<tbody id='resultsContainer'></tbody>");
    //$("#resultsContainer").empty();
    for (var i = 0; i < mtxPersonas.length; i++) {

        if (mtxPersonas[i].Estado == "A")
        { totalOK++ }

        $("#resultsContainer").append("<tr><td>" + mtxPersonas[i].resultados + "</td><td>" + mtxPersonas[i].Codigo + "</td><td>" + mtxPersonas[i].Precio + "</td></tr>");
    }

    if (totalOK == 0) {
        $("#divIconoDescargarOK").hide();
    }
    else {
        $("#divIconoDescargarOK").show();
    }

    $("#msjResultados").html("Cantidad de productos a importar " + totalOK + "/" + mtxPersonas.length + "- Presione importar para continuar o bien corrija los cambios e intente nuevamente");
    $("#ContResultados").show();
}

function importarDatos() {

    var URL = "";

    if (tipo == "Productos" || tipo == "Servicios") {
        URL = "/Importaciones/RealizarImportacionProductos";
    }
    else if (tipo == "Clientes" || tipo == "Proveedores") {
        URL = "/Importaciones/RealizarImportacionPersonas";
    }

    $("#divbtnImportar").attr("disabled", true);
    $("#imgLoading2").show();
    $.ajax({
        type: "POST",
        data: "{id:" + $("#hdnID").val() + "}",
        url: URL,
        data: "{}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            $("#resultsContainer").empty();
            $("#divOk").show();
            $("#imgLoading2").hide();
            $("#ContResultados").hide();
            $("#divbtnImportar").attr("disabled", false);
        }
    });
}