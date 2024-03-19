var elegirPlan = {
    ActualizarPlanBasico: function () {
        Common.mostrarProcesando("btnPlanBasico");
        $.ajax({
            type: "GET",
            url: "/modulos/seguridad/pagoDePlanes.aspx/GuardarPlanBasico",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data, text) {
                $('#divOk').show();
                $("#divError").hide();
                $('html, body').animate({ scrollTop: 0 }, 'slow');
                window.location.href = "/home.aspx";
            },
            error: function (response) {
                var r = jQuery.parseJSON(response.responseText);
                $("#msgError").html(r.Message);
                $("#divError").show();
                $("#divOk").hide();
                $('html, body').animate({ scrollTop: 0 }, 'slow');
                Common.ocultarProcesando("btnPlanBasico", "¡Comenzá a utilizarlo ya!");
            }
        });
    },
    subirPlan: function (plan) {
        var modo = $("input[name='ctl00$MainContent$r" + plan + "']:checked").attr('value');
        window.location.href = "/modulos/seguridad/pagoDePlanes.aspx?plan=" + plan + "&modo=" + modo;
    },
}

