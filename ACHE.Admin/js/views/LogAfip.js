var logAfip = {   
    obtenerLogAfip: function () {

        $("#divError").hide();
        $("#resultsContainer").html("");
        var cuit = parseInt($("#txtCuit").val());
        var take = parseInt($("#ddlTake").val());
        var info = "{ cuit: " + cuit + ", take: " + take + "}";

        $.ajax({
            type: "POST",
            url: "/LogAfip/ObtenerLog",
            contentType: "application/json; charset=utf-8",
            data: info,
            dataType: "json",
            success: function (data) {

                if (data.TotalPage > 0) {
                    $("#msjResultados").html("Mostrando los ultimos " + take + " resultados");
                }
                else {
                    $("#msjResultados").html("");
                }

                // Render using the template
                if (data.Items.length > 0)
                    $("#resultTemplate").tmpl({ results: data.Items }).appendTo("#resultsContainer");
                else
                    $("#noResultTemplate").tmpl({ results: data.Items }).appendTo("#resultsContainer");
            }
        });
    },
    detalle: function (id) {
        window.location.href = "LogAfip/View/" + id;
        //window.open("/LogAfip/View/" + id);
    }
}