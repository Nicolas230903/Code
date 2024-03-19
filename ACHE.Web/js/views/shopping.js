var tipoPaquete;

function showModal(titulo)
{
    tipoPaquete = titulo;
    var obj = document.getElementById("titDetalleShopping");
    obj.textContent = titulo;
    tipoPaquete = titulo;
    $('#txtShoppingMensaje').val("");
    $("#divShoppingError").hide();
    $('#modalShopping').modal('show');


}

function enviarPaquetesShopping() {
    $("#divShoppingError").hide();

    if ($("#txtShoppingMensaje").val() == "") {
        $("#msgShoppingError").html("Por favor, describenos tu consulta y/o problema");
        $("#divShoppingError").show();
        $('html, body').animate({ scrollTop: 0 }, 'slow');
        return false;
    }
    else {

        var info = "{ mensaje: '" + $("#txtShoppingMensaje").val() + "' ,tipoPaquete: '" + tipoPaquete + "'}";

        $.ajax({
            type: "POST",
            url: "shopping.aspx/PaquetesShopping",
            data: info,
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data, text) {
                $("#txtShoppingMensaje").val("");
                $("#divShoppingError").hide();

                $('#modalShopping').modal('hide');
            },
            error: function (response) {
                var r = jQuery.parseJSON(response.responseText);
                $("#msgShoppingError").html(r.Message);
                $("#divShoppingError").show();
                $('html, body').animate({ scrollTop: 0 }, 'slow');
            }
        });
    }
}