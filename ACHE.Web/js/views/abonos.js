var listaClientes = new Array();
/*** FORM ***/
function ocultarMensajes() {
    $("#divError, #divOk").hide();
}
function grabar() {
    $("#divError,#divErrorClientes,#divOk").hide();
    ocultarMensajes();

    if (parseFloat($("#txtPrecio").val()) == 0) {
        $("#msgError").html("El importe debe ser mayor a 0");
        $("#divError").show();
        return false;
    }
    if (listaClientes.length == 0) {
        $("#msgError").html("Ingrese al menos un cliente");
        $("#divError").show();
        return false;
    }

    var id = ($("#hdnID").val() == "" ? "0" : $("#hdnID").val());
    if ($('#frmEdicion').valid()) {
        Common.mostrarProcesando("btnActualizar");

        var personas = ($("#ddlPersona").val() == null) ? "" : $("#ddlPersona").val();
        var planDeCuenta = ($("#ddlPlanDeCuentas").val() == null || $("#ddlPlanDeCuentas").val() == "") ? "0" : $("#ddlPlanDeCuentas").val();

        
        var info = "{ id: " + parseInt(id)
                + ", nombre: '" + $("#txtNombre").val()
                + "', frecuencia: '" + $("#ddlFrecuencia").val()
                + "', fechaInicio: '" + $("#txtFechaInicio").val()
                + "', fechaFin: '" + $("#txtFechaFin").val()
                + "', estado: '" + $("#ddlEstado").val()
                + "', precio: '" + $("#txtPrecio").val()
                + "', iva: '" + $("#ddlIva").val()
                + "', obs: '" + $("#txtObservaciones").val()
                + "', personas: " + JSON.stringify(listaClientes)
                + " , tipo: " + $("#ddlProducto").val()
                + ", idPlanDeCuenta: " + planDeCuenta
                + "}";

        $.ajax({
            type: "POST",
            url: "abonose.aspx/guardar",
            data: info,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data, text) {
                $('#divOk').show();
                $('html, body').animate({ scrollTop: 0 }, 'slow');
                window.location.href = "/abonos.aspx";
            },
            error: function (response) {
                var r = jQuery.parseJSON(response.responseText);
                $("#msgError").html(r.Message);
                $("#divError").show();
                $("#divOk").hide();
                $('html, body').animate({ scrollTop: 0 }, 'slow');
                Common.ocultarProcesando("btnActualizar", "Aceptar");
            }
        });
    }
    else {
        return false;
    }
}
function cancelar() {
    window.location.href = "/abonos.aspx";
}
function calcularPrecioFinal() {
    $("#divPrecioIVA").html("$ " + addSeparatorsNF(calcularPrecio(), '.', ',', '.'));

    recalcularTotal();
}
function calcularPrecio() {
    var total = 0;
    if ($("#txtPrecio").val() != "") {
        total += parseFloat($("#txtPrecio").val().replace(",", "."));
    }
    var iva = parseFloat($("#ddlIva").val().replace(",", "."));
    total = (total + ((total * iva) / 100));
    return total.toFixed(2);
}
function configForm() {
   // configDatePicker();
    // Date Picker
    $("#txtFechaFin").datepicker();

    Common.configDatePicker();
    Common.configFechasDesdeHasta("txtFechaInicio", "txtFechaFin");

    $.validator.addMethod("validFechaActual", function (value, element) {

        var fInicio = $("#txtFechaInicio").val().split("/");
        var fechainicio = new Date(fInicio[2], (fInicio[1] - 1), fInicio[0]);

        var today = new Date();
        var fechaFin = new Date(today);
        fechaFin.setDate(today.getDate() + 5);

        if (fechainicio > fechaFin) {
            return false;
        }
        else {
            return true;
        }
    }, "Solo puede facturar hasta 5 dias despues de la fecha actual");

    $(".select2").select2({
        width: '100%', allowClear: true,
        formatNoMatches: function (term) {
            return "<a style='cursor:pointer' onclick=\"$('#modalNuevoCliente').modal('show');$('.select2').select2('close');\">+ Agregar</a>";
        }
    });

    //Seteo las personas que corresponden
    //var personas = $("#hdnPersonasID").val().split(",");
    //    Common.obtenerPersonas("ddlPersona", personas, true);
    Common.obtenerPersonas("ddlPersona", "", true);
    if (parseInt($("#hdnID").val()) > 0) {
        obtenerClientes()
    }


    $("#txtPrecio").maskMoney({ thousands: '', decimal: ',', allowZero: true });

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
    verificarPlanDeCuentas();
    parsearNumeros();
}
/*** SEARCH ***/
function configFilters() {

    $("#txtCondicion, #txtFechaHasta, #txtNombre").keypress(function (event) {
        var keycode = (event.keyCode ? event.keyCode : event.which);
        if (keycode == '13') {
            resetearPagina();
            filtrar();
            return false;
        }
    });

    $(".select2").select2({ width: '100%', allowClear: true });
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
}
function nuevo() {
    window.location.href = "/abonose.aspx";
}
function editar(id) {
    window.location.href = "/abonose.aspx?ID=" + id;
}
function eliminar(id, nombre) {
    bootbox.confirm("¿Está seguro que desea eliminar el abonos " + nombre + "?", function (result) {
        if (result) {
            $.ajax({
                type: "POST",
                url: "abonos.aspx/delete",
                data: "{ id: " + id + "}",
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data, text) {
                    filtrar();
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
    //createLoading();
    $("#divError").hide();
    
    if ($('#frmSearch').valid()) {
        $("#resultsContainer").html("");

        var currentPage = parseInt($("#hdnPage").val());

        var info = "{ condicion: '" + $("#txtCondicion").val()
                   + "', periodo: '" + $("#ddlPeriodo").val()
                   + "', fechaDesde: '" + $("#txtFechaDesde").val()
                   + "', fechaHasta: '" + $("#txtFechaHasta").val()
                   + "', page: " + currentPage + ", pageSize: " + PAGE_SIZE
                   + "}";

        $.ajax({
            type: "POST",
            url: "abonos.aspx/getResults",
            data: info,
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
}
function verTodos() {
    $("#txtNombre, #txtFechaDesde, #txtFechaHasta").val("");
    filtrar();
}
function exportar() {
    resetearExportacion();

    $("#imgLoading").show();
    $("#divIconoDescargar").hide();

    var idPersona = 0;
    if ($("#ddlPersona").val() != "")
        idPersona = parseInt($("#ddlPersona").val());

    var info = "{ condicion: '" + $("#txtCondicion").val()
                + "', periodo: '" + $("#ddlPeriodo").val()
                + "', fechaDesde: '" + $("#txtFechaDesde").val()
                + "', fechaHasta: '" + $("#txtFechaHasta").val()
                + "'}";

    $.ajax({
        type: "POST",
        url: "abonos.aspx/export",
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
function otroPeriodo() {
    if ($("#ddlPeriodo").val() == "-1")
        $('#divMasFiltros').toggle(600);
    else {
        if ($("#divMasFiltros").is(":visible"))
            $('#divMasFiltros').toggle(600);

        $("#txtFechaDesde,#txtFechaHasta").val("");
        filtrar();
    }
}

function duplicar(id) {
    window.location.href = "/abonose.aspx?ID=" + id + "&Duplicar=1";
}
/*** CLIENTES ***/
function cargarPersona() {
    $("#divError,#divErrorClientes").hide();

    if ($("#ddlPersona").val() == "" || $("#ddlPersona") == null) {

        $("#msgErrorClientes").html("Debe seleccionar un cliente");
        $("#divErrorClientes").show();
    }
    else {

        var result = listaClientes.filter(function (el) {
            return el.IDPersona == $("#ddlPersona").val();
        });

        if (result.length > 0) {
            $("#msgErrorClientes").html("El cliente ya se encuentra cargado");
            $("#divErrorClientes").show();
        }
        else {
            var razon = "";
            var aux = $("#ddlPersona option:selected").text().split("-");
            for (var i = 1; i < aux.length; i++) {
                razon = razon + " " + aux[i];
            }

            var obj = new Object();
            obj.IDAbono = parseInt($("#hdnID").val());
            obj.IDPersona = parseInt($("#ddlPersona").val());
            obj.Cantidad = 1;
            obj.RazonSocial = razon;//$("#ddlPersona option:selected").text().split("-")[1];
            obj.Total = addSeparatorsNF(calcularPrecio(), '.', ',', '.');
            listaClientes.push(obj);
            $("#ddlPersona").val("");
            $("#ddlPersona").trigger("change");
            actualizarTemplate();
        }
    }
}

function obtenerClientes() {
    var id = parseInt($("#hdnID").val());
    var info = "{ id: " + id + "}";

    $.ajax({
        type: "POST",
        url: "/abonose.aspx/ObtenerClientes",
        data: info,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data, text) {
            if (data.d.length > 0) {
                listaClientes = data.d;
            }
            actualizarTemplate();
        },
        error: function (response) {
            var r = jQuery.parseJSON(response.responseText);
            alert(r.Message);
        }
    });
}
function eliminarCliente(idPersona) {
    listaClientes = listaClientes.filter(function (el) {
        return el.IDPersona != idPersona;
    });
    actualizarTemplate();
}
function actualizarTemplate() {
    $("#resultsContainer").html("");
    if (listaClientes.length > 0)
        $("#resultTemplate").tmpl({ results: listaClientes }).appendTo("#resultsContainer");
    else
        $("#noResultTemplate").tmpl({ results: listaClientes }).appendTo("#resultsContainer");

    $(".ListaClientes").numericInput();
    $('.ListaClientes').bind("cut copy paste", function (e) {
        e.preventDefault();
    });
}
function recalcularTotal(idPersona, cantidad) {
    var arrayAux = new Array();
    var total = parseFloat(calcularPrecio());
    for (var i = 0; i < listaClientes.length; i++) {
        var c = 1;
        if (idPersona != null && idPersona != "" && idPersona > 0 && idPersona == listaClientes[i].IDPersona) {
            c = parseInt(cantidad);
        }
        else {
            var c = parseInt(listaClientes[i].Cantidad);
        }
        
        var obj = new Object();
        obj.IDAbono = listaClientes[i].IDAbono;
        obj.IDPersona = listaClientes[i].IDPersona;
        obj.RazonSocial = listaClientes[i].RazonSocial;
        obj.Cantidad = c;
        obj.Total = numeral((c * total)).format('0,0.00');
        arrayAux.push(obj);
    }

    listaClientes = arrayAux;
    actualizarTemplate();
}


function parsearNumeros() {
    numeral.language('fr', {
        delimiters: {
            thousands: '.',
            decimal: ','
        },
        abbreviations: {
            thousand: 'k',
            million: 'm',
            billion: 'b',
            trillion: 't'
        },
        ordinal: function (number) {
            return number === 1 ? 'er' : 'ème';
        },
        currency: {
            symbol: '€'
        }
    });
    numeral.language('fr');
}

function verificarPlanDeCuentas() {
    if ($("#hdnUsaPlanCorporativo").val() == "1") {
        $(".divPlanDeCuentas").show();
    }
    else {
        $(".divPlanDeCuentas").hide();
    }
}