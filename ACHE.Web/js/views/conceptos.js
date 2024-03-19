/*** FORM ***/

function changeTipo(resetear) {
    if ($("#ddlTipo").val() == "S") {
        $("#divStock,#divStockMinimo").hide();
        if (resetear)
            $("#txtStock,#txtStockMinimo").val("0");
    }
    else {
        $("#divStock,#divStockMinimo").show();
        if (resetear)
            $("#txtStock,#txtStockMinimo").val("");
    }
}

function ocultarMensajes() {
    $("#divError, #divOk, #divErrorCat").hide();
}

function grabar() {
    ocultarMensajes();

    var id = ($("#hdnID").val() == "" ? "0" : $("#hdnID").val());
    if ($('#frmEdicion').valid()) {
        Common.mostrarProcesando("actualizarConcepto");

        var idPersona = ($("#ddlPersonas").val() == "" || $("#ddlPersonas").val() == null) ? 0 : $("#ddlPersonas").val();
        var obj = new Object();
        obj.id = parseInt(id);
        obj.nombre = $("#txtNombre").val();
        obj.codigo = $("#txtCodigo").val();
        obj.tipo = $("#ddlTipo").val();
        obj.descripcion = $("#txtDescripcion").val();
        obj.estado = $("#ddlEstado").val();
        obj.precio = $("#txtPrecio").val().replace(".", "");
        obj.iva = $("#ddlIva").val();
        obj.stock = ($("#txtStock").val() == "" || $("#txtStock").val() == null) ? "1" : $("#txtStock").val();
        obj.obs = $("#txtObservaciones").val();
        obj.constoInterno = $("#txtCostoInterno").val().replace(".","");
        obj.stockMinimo = $("#txtStockMinimo").val();
        obj.idPersona = idPersona;
        $.ajax({
            type: "POST",
            url: "conceptose.aspx/guardar",
            data: JSON.stringify(obj),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: false,
            success: function (data, text) {
                $('#divOk').show();
                $("#divError").hide();
                $('html, body').animate({ scrollTop: 0 }, 'slow');
                $("#hdnID").val(data.d);

                if ($("#hdnSinCombioDeFoto").val() == "0") {
                    window.location.href = "/conceptos.aspx";
                }
            },
            error: function (response) {
                var r = jQuery.parseJSON(response.responseText);
                $("#msgError").html(r.Message);
                $("#divError").show();
                $("#divOk").hide();
                $('html, body').animate({ scrollTop: 0 }, 'slow');
                Common.ocultarProcesando("actualizarConcepto", "Aceptar");
            }
        });
    }
    else {
    return false;
    }
}

function cancelar() {
    window.location.href = "/conceptos.aspx";
}


function calcularPrecioFinal() {
    var total = 0;
    if ($("#txtPrecio").val() != "") {
        total += parseFloat($("#txtPrecio").val().replace(",", "."));
    }
    //var iva = 0;
    //var idIva = parseFloat($("#ddlIva").val().replace(",", "."));
    //if ($("#hdnUsaPrecioFinalConIVA").val() == "1") {
    //    switch (idIva) {
    //        case "1":
    //            iva = 0;
    //            break;
    //        case "2":
    //            iva = 0;
    //            break;
    //        case "3":
    //            iva = 0;
    //            break;
    //        case "4":
    //            iva = 1.050;
    //            break;
    //        case "5":
    //            iva = 1.210;
    //            break;
    //        case "6":
    //            iva = 1.270;
    //            break;
    //        case "7":
    //            iva = 1.210;
    //            break;
    //        case "8":
    //            iva = 1.025;
    //            break;
    //    }
    //    if (iva > 0) {            
    //        total = total / parseFloat(iva);
    //    }
    //}
    //else {
    //    switch (idIva) {
    //        case "1":
    //            iva = 0;
    //            break;
    //        case "2":
    //            iva = 0;
    //            break;
    //        case "3":
    //            iva = 0;
    //            break;
    //        case "4":
    //            iva = 1.050;
    //            break;
    //        case "5":
    //            iva = 1.210;
    //            break;
    //        case "6":
    //            iva = 1.270;
    //            break;
    //        case "7":
    //            iva = 1.210;
    //            break;
    //        case "8":
    //            iva = 1.025;
    //            break;
    //    }
    //    total = (total + ((total * iva) / 100));
    //}

    $("#divPrecioIVA").html("$ " + addSeparatorsNF(total.toFixed(2), '.', ',', '.'));
}


function configForm() {
    $(".select2").select2({ width: '100%', allowClear: true });
    Common.obtenerPersonas("ddlPersonas", $("#hdnIDPersona").val(), true);

    $("#txtPrecio,#txtCostoInterno").maskMoney({ thousands: '', decimal: ',', allowZero: true });

    $("#txtStock,#txtStockMinimo").maskMoney({ thousands: '', decimal: ',', allowZero: true, allowNegative: true });

    $("#txtPrecio").blur(function () {
        calcularPrecioFinal();
    });
    $("#txtPrecio, #ddlIva").change(function () {
        calcularPrecioFinal();
    });

    if ($("#txtPrecio").val() != "") {
        var precio = parseFloat($("#txtPrecio").val());
        $("#txtPrecio").val(addSeparatorsNF(precio.toFixed(2), '.', ',', '.'));
    }

    if ($("#txtCostoInterno").val() != "") {
        var costoInterno = parseFloat($("#txtCostoInterno").val());
        $("#txtCostoInterno").val(addSeparatorsNF(costoInterno.toFixed(2), '.', ',', '.'));
    }

    // Validation with select boxes
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

    if ($("#hdnID").val() != "" && $("#hdnID").val() != "0") {
        changeTipo(false);
    }
    adjuntarFoto();
}

/*** SEARCH ***/

function configFilters() {
    $("#txtCondicion").keypress(function (event) {
        var keycode = (event.keyCode ? event.keyCode : event.which);
        if (keycode == '13') {
            resetearPagina();
            filtrar();
            return false;
        }
    });
}

function nuevo() {
    window.location.href = "/conceptose.aspx";
}

function importar(tipo) {
    window.location.href = "/importar.aspx?tipo=" + tipo;
}

function editar(id) {
    window.location.href = "/conceptose.aspx?ID=" + id;
}

function eliminar(id, nombre) {
    $("#divError").hide();
    $("#divOk").hide();
    bootbox.confirm("¿Está seguro que desea eliminar el producto/servicios " + nombre + "?", function (result) {
        if (result) {
            $.ajax({
                type: "POST",
                url: "conceptos.aspx/delete",
                data: "{ id: " + id + "}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data, text) {
                    $('#msgOk').html("Concepto eliminado correctamente.");
                    $("#divOk").show();
                    $('html, body').animate({ scrollTop: 0 }, 'slow');
                    setTimeout(function () {
                        filtrar();
                    }, 3000);  
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

function duplicar(id) {
    window.location.href = "/conceptose.aspx?ID=" + id + "&Duplicar=1";
}

function mostrarPagAnterior() {
    var paginaActual = parseInt($("#hdnPage").val());
    paginaActual--;
    $("#hdnPage").val(paginaActual);
    filtrar();
}

function mostrarPagProxima() {
    var paginaActual = parseInt($("#hdnPage").val());
    paginaActual++;
    $("#hdnPage").val(paginaActual);
    filtrar();
}

function resetearPagina() {
    $("#hdnPage").val("1");
}

function filtrar() {

    $("#divError").hide();
    $("#divOk").hide();
    $("#resultsContainer").html("");
    var currentPage = parseInt($("#hdnPage").val());

    var info = new Object();
    info.condicion = $("#txtCondicion").val();
    info.page = currentPage;
    info.pageSize = PAGE_SIZE;

    $.ajax({
        type: "POST",
        url: "conceptos.aspx/getResults",
        data: JSON.stringify(info),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data, text) {
            if (data.d.TotalPage > 0) {
                $("#divPagination").show();

                $("#lnkNextPage, #lnkPrevPage").removeAttr('disabled')
                if (data.d.TotalPage == 1)
                    $("#lnkNextPage, #lnkPrevPage").attr('disabled', "disabled")
                else if (currentPage == data.d.TotalPage)
                    $("#lnkNextPage").attr("disabled", "disabled");
                else if (currentPage == 1)
                    $("#lnkPrevPage").attr("disabled", "disabled");

                var aux = (currentPage * PAGE_SIZE);
                if (aux > data.d.TotalItems)
                    aux = data.d.TotalItems;
                $("#msjResultados").html("Mostrando " + ((currentPage * PAGE_SIZE) - PAGE_SIZE + 1) + " - " + aux + " de " + data.d.TotalItems);
            }
            else {
                $("#divPagination").hide();
                $("#msjResultados").html("");
            }

            // Render using the template
            if (data.d.Items.length > 0)
                $("#resultTemplate").tmpl({ results: data.d.Items }).appendTo("#resultsContainer");
            else
                $("#noResultTemplate").tmpl({ results: data.d.Items }).appendTo("#resultsContainer");
        },
        error: function (response) {
            var r = jQuery.parseJSON(response.responseText);
            alert(r.Message);
        }
    });
    resetearExportacion();
}

function verTodos() {
    $("#txtNombre, #txtCodigo, #ddlTipo").val("");
    filtrar();
}

function exportar() {
    resetearExportacion();
    $("#imgLoading").show();
    $("#divIconoDescargar").hide();

    var info = "{ condicion: '" + $("#txtCondicion").val()
               + "'}";

    $.ajax({
        type: "POST",
        url: "conceptos.aspx/export",
        data: info,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data, text) {
            if (data.d != "") {

                $("#divError").hide();
                $("#imgLoading").hide();
                $("#lnkDownload").show();
                $("#lnkDownload").attr("href", data.d);
                $("#lnkDownload").attr("download", data.d);
            }
        },
        error: function (response) {
            var r = jQuery.parseJSON(response.responseText);
            $("#msgError").html(r.Message);
            $("#divError").show();
            $('html, body').animate({ scrollTop: 0 }, 'slow');

            resetearExportacion();
        }
    });
}

function resetearExportacion() {
    $("#imgLoading, #lnkDownload").hide();
    $("#divIconoDescargar").show();
}

/*** FOTOS***/

function adjuntarFoto() {

    $('#flpArchivo').fileupload({
        url: '/subirImagenes.ashx?idconcepto=' + $("#hdnID").val() + "&opcionUpload=conceptos" + "&idUsuario=" + $("#hdnIdUsuario").val(),
        success: function (response, status) {
            if (response == "OK") {
                $("#divError").hide();
                $("#divOk").show();
                window.location.href = "/conceptos.aspx";
            }
            else {
                $("#hdnFileName").val("");
                $("#msgError").html(response);
                $("#divError").show();
                $("#divOk").hide();
                Common.ocultarProcesando("actualizarConcepto", "Aceptar");
            }
        },
        error: function (error) {
            $("#hdnFileName").val("");
            $("#msgError").html(error.responseText);
            $("#divError").show();
            $("#divOk").hide();
            $('html, body').animate({ scrollTop: 0 }, 'slow');

        },
        autoUpload: false,
        add: function (e, data) {
            $("#hdnSinCombioDeFoto").val("1");

            $("#actualizarConcepto").on("click", function () {
                $("#imgLoading").show();
                grabar();
                if ($("#hdnID").val() != "0") {
                    data.url = '/subirImagenes.ashx?idconcepto=' + $("#hdnID").val() + "&opcionUpload=conceptos" + "&idUsuario=" + $("#hdnIdUsuario").val();
                    data.submit();
                }
            })
        }
    });

    showBtnEliminar();
}

function showBtnEliminar() {
    if ($("#hdnTieneFoto").val() == "1") {
        $("#divEliminarFoto").show();
        $("#divAdjuntarFoto").removeClass("col-sm-12").addClass("col-sm-6");
    }
    else {
        $("#divEliminarFoto").hide();
        $("#divAdjuntarFoto").removeClass("col-sm-6").addClass("col-sm-12");
    }
}
function showInputFoto() {
    $("#divFoto").slideToggle();
}
function grabarsinImagen() {

    if ($("#hdnSinCombioDeFoto").val() == "0") {
        grabar();
    }
}

function eliminarFotoProducto() {

    var id = ($("#hdnID").val() == "" ? "0" : $("#hdnID").val());
    if (id != "") {


        var info = "{ idConcepto: " + parseInt(id) + "}";

        $.ajax({
            type: "POST",
            url: "conceptose.aspx/eliminarFoto",
            data: info,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            async: false,
            success: function (data, text) {
                $('#divOk').show();
                $("#divError").hide();
                $('html, body').animate({ scrollTop: 0 }, 'slow');
                $("#imgFoto").attr("src", "/files/usuarios/no-producto.png");
                $("#hdnTieneFoto").val("0");
                showBtnEliminar();
            },
            error: function (response) {
                var r = jQuery.parseJSON(response.responseText);
                $("#msgError").html(r.Message);
                $("#divError").show();
                $("#divOk").hide();
                $('html, body').animate({ scrollTop: 0 }, 'slow');
            }
        });
    }
    else {
        $("#msgError").html("El producto no tiene una imagen guardada");
        $("#divError").show();
        return false;
    }
}