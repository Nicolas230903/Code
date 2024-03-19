var alertas = {
    //*** FORM***/
    guardarAlerta: function () {
        if ($("#frmEdicion").valid()) {
            Common.mostrarProcesando("btnActualizar");

            var id = "0";
            if ($("#hdnID").val() !== undefined) {
                id = ($("#hdnID").val() == "" ? "0" : $("#hdnID").val());
            }
            var info = "{ id: " + id
                        + ", importe: " + $("#txtimporteAlerta").val()
                        + ",avisos: '" + $("#ddlAvisoAlertasModal").val()
                        + "', condiciones: '" + $("#ddlCondicionAlertas").val()
                        + "'}";

            $.ajax({
                type: "POST",
                url: "/alertas.aspx/guardarAlertas",
                data: info,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    $("#divAyudaErrorAlertas").hide();
                    $('html, body').animate({ scrollTop: 0 }, 'slow');

                    if ($('#divSinDatos').is(":visible")) {
                        window.location.href = "/alertas.aspx";
                    }
                    $('#modalAlertas').modal('toggle');
                    alertas.filtrar();
                    Common.ocultarProcesando("btnActualizar", "Aceptar");
                },
                error: function (response) {
                    var r = jQuery.parseJSON(response.responseText);
                    $("#msgAyudaErrorAlertas").html(r.Message);
                    $("#divAyudaErrorAlertas").show();
                    $('html, body').animate({ scrollTop: 0 }, 'slow');
                    Common.ocultarProcesando("btnActualizar", "Aceptar");
                }
            });
        }
        else {
            return false;
        }
    },
    configForm: function () {
        $("#txtimporteAlerta").maskMoney({ thousands: '', decimal: '.', allowZero: true });
        $("#txtimporteAlerta").val("");
        $("#msgAyudaErrorAlertas").html("");
        $("#divAyudaErrorAlertas").hide();
        Common.ocultarProcesando("btnActualizar", "Aceptar");

      

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
    },
    showModalAlerta: function () {
        $('#modalAlertas').modal('show');
        alertas.configForm();
    },
    /*** Busqueda***/
    resetearPagina: function () {
        $("#hdnPage").val("1");
    },
    nuevo: function () {
        $("#hdnID").val("0");
        alertas.showModalAlerta();
    },
    editar: function (id) {
        $("#divError").hide();
        $("#divOk").hide();

        var info = "{ id: " + parseInt(id) + "}";

        $.ajax({
            type: "POST",
            url: "/alertas.aspx/cargarEntidad",
            data: info,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data, text) {

                alertas.showModalAlerta();
                $("#hdnID").val(data.d.ID);
                $("#txtimporteAlerta").val(data.d.Importe);
                $("#ddlAvisoAlertasModal").val(data.d.AvisoAlerta);
                $("#ddlCondicionAlertas").val(data.d.Condicion);

            },
            error: function (response) {
                var r = jQuery.parseJSON(response.responseText);
                $("#msgErrorAlta").html(r.Message);
                $("#divErrorAlta").show();
                $("#divOk").hide();
                $('html, body').animate({ scrollTop: 0 }, 'slow');
            }
        });
    },
    eliminar: function (id, nombre) {
        bootbox.confirm("¿Está seguro que desea eliminar la alerta " + nombre + "?", function (result) {
            if (result) {
                $.ajax({
                    type: "POST",
                    url: "/alertas.aspx/delete",
                    data: "{ id: " + id + "}",
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    success: function (data, text) {
                        alertas.filtrar();
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
    },
    filtrar: function () {
        $("#divError").hide();

        $("#resultsContainer").html("");
        var currentPage = parseInt($("#hdnPage").val());

        var info = "{ avisoAlertas: '" + $("#ddlAvisoAlertas").val()
                   + "', page: " + currentPage + ", pageSize: " + PAGE_SIZE
                   + "}";

        $.ajax({
            type: "POST",
            url: "/alertas.aspx/getResults",
            data: info,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data, text) {

                var aux = (currentPage * PAGE_SIZE);
                if (aux > data.d.TotalItems)
                    aux = data.d.TotalItems;

                if (data.d.TotalPage > 0) {
                    $("#divPagination").show();
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
                $("#msgError").html(r.Message);
                $("#divError").show();
                $("#divOk").hide();
                $('html, body').animate({ scrollTop: 0 }, 'slow');
            }
        });
    },
    verTodos: function () {
        $("#ddlAvisoAlertas").val("").trigger("change");
        alertas.filtrar();
    },

    /*** HEADER***/
    esconderAlertaGenerada: function (idAlertaGenerada) {

        var info = "{ id: " + idAlertaGenerada + "}";
        $.ajax({
            type: "POST",
            url: "/alertas.aspx/esconderAlertaGenerada",
            data: info,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                //$("#alertaGenerada_" + idAlertaGenerada).hide();
                $("#alertaGenerada_" + idAlertaGenerada).toggle(400);

                var cant = parseInt($("#spanMsgcantidad").html());
                cant--;
              

                if (cant ==0) {
                    $("#spanMsgTitulo").html("No tienes aviso/s");
                    $("#spanMsgcantidad").html("");
                    $("#spamMensajes").html(" Mensajes");
                }
                else {
                    $("#spanMsgcantidad").html(cant);
                    $("#spanMsgTitulo").html("Tienes " + cant + " aviso/s");

                    $("#spamMensajes").html(" Mensajes (" + cant + ")");
                }
            }
        });
    },
    abrirComprobante: function (id, tipo) {

      
        if (tipo == 1) {
            window.location.href = "/pagose.aspx?ID=" + id;
        }
        else {
            window.location.href = "/cobranzase.aspx?ID=" + id;
        }
    }
}