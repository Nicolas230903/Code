var sistemas = {    
    grabar: async function () {

        if ($('#frmEdicion').valid()) {                

            Common.mostrarProcesando("btnActualizarInfo");
            $("#divError").hide();
            var info = "{ vigencia: '" + $("#txtVigencia").val()
                + "', clave: '" + $("#txtClave").val()
                + "', modulo1_Nombre: '" + $("#txtModulo1_Nombre").val()
                + "', modulo1_Version: '" + $("#txtModulo1_Version").val()
                + "', modulo2_Nombre: '" + $("#txtModulo2_Nombre").val()
                + "', modulo2_Version: '" + $("#txtModulo2_Version").val()
                + "', modulo3_Nombre: '" + $("#txtModulo3_Nombre").val()
                + "', modulo3_Version: '" + $("#txtModulo3_Version").val()
                + "', modulo4_Nombre: '" + $("#txtModulo4_Nombre").val()
                + "', modulo4_Version: '" + $("#txtModulo4_Version").val()
                + "', modulo5_Nombre: '" + $("#txtModulo5_Nombre").val()
                + "', modulo5_Version: '" + $("#txtModulo5_Version").val()                    
                + "' }";

            $.ajax({
                type: "POST",
                url: "/Sistemas/Guardar",
                data: info,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data) {
                    Common.ocultarProcesando("btnActualizarInfo", "Actualizar");

                    $('html, body').animate({ scrollTop: 0 }, 'slow');
                    if (data) {                        
                        $("#divOk").show();
                        setTimeout('document.location.reload()', 2000);
                    }
                    else {
                        $("#divError").show();
                        $("#msgError").html("no se guardaron los cambios");
                    }
                }
            });

        }
        else {
            return false;
        }
    }
}
