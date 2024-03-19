/*** ACTIONS ***/
function downloadAllPathSelected() {
    var infoToDownloadTipos = "";
    var infoToDownloadPaths = "";

    $('.thmb').each(function () {
        if ($(this).hasClass('checked')) {
            var t = $(this);
            infoToDownloadTipos += (t.find("input[name='hdnTipoAux']").val()) + "X-X";
            infoToDownloadPaths += (t.find("input[name='hdnPathAux']").val()) + "X-X";
        }
    });

    if (infoToDownloadPaths != "" && infoToDownloadTipos) {
        var datos = "fileHandler.ashx?action=downloadAll&tipo=" + infoToDownloadTipos + "&path=" + infoToDownloadPaths;
        window.location.assign("fileHandler.ashx?action=downloadAll&tipo=" + infoToDownloadTipos + "&path=" + infoToDownloadPaths);
    }
}

function deleteAllPathSelected() {
    if ($("#lnkDeleteAll").attr('class') == "itemopt") {
        bootbox.confirm("¿Está seguro que desea eliminar todos los items seleccionados?", function (result) {
            if (result) {
                var infoToDeleteTipos = "";
                var infoToDeletePaths = "";

                $('.thmb').each(function () {
                    if ($(this).hasClass('checked')) {
                        var t = $(this);
                        infoToDeleteTipos += (t.find("input[name='hdnTipoAux']").val()) + "X-X";
                        infoToDeletePaths += (t.find("input[name='hdnPathAux']").val()) + "X-X";
                    }
                });

                if (infoToDeletePaths != "" && infoToDeleteTipos) {
                    $.ajax({
                        type: "GET",
                        url: "fileHandler.ashx?action=deleteAll&tipo=" + infoToDeleteTipos + "&path=" + infoToDeletePaths,
                        contentType: "application/json; charset=utf-8",
                        dataType: "json",
                        success: function (data, text) {
                            loadInfo($("#hdnPath").val());
                        },
                        error: function (response) {
                            alert(response.responseText);
                        }
                    });
                }
            }
        });
    }
}

function deletePath(tipo, path) {
    bootbox.confirm("¿Está seguro que desea eliminar el item seleccionado?", function (result) {
        if (result) {
            $.ajax({
                type: "GET",
                url: "fileHandler.ashx?action=deletePath&tipo=" + tipo + "&path=" + path,
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (data, text) {
                    loadInfo($("#hdnPath").val());
                },
                error: function (response) {
                    alert(response.responseText);
                }
            });
        }
    });
}

function downloadPath(tipo, path) {
    window.location.assign("fileHandler.ashx?action=download&tipo=" + tipo + "&path=" + path);
}

function doRename(urlCall) {
    $.ajax({
        type: "GET",
        url: "fileHandler.ashx?action=rename&" + urlCall,
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data, text) {
            bootbox.hideAll();
            loadInfo($("#hdnPath").val());
        },
        error: function (response) {
            alert(response.responseText);
        }
    });
}

function renamePath(tipo, path) {
    bootbox.prompt("Ingrese el nuevo nombre", function (result) {
        if (result) {
            doRename("tipo=" + tipo + "&path=" + path + "&newName=" + result);
        }
    }).find("div.modal-dialog").addClass("smallModalWidth");;

    $('form .bootbox-input').bind('keypress', function (event) {
        var keycode = (event.keyCode ? event.keyCode : event.which);
        if (keycode == '13') {
            doRename("tipo=" + tipo + "&path=" + path + "&newName=" + $('form .bootbox-input').val());
            return false;
        }
        else {
            var regex = new RegExp("^[A-za-z0-9\-_\\s\.\,]+$");
            var key = String.fromCharCode(!event.charCode ? event.which : event.charCode);
            if (!regex.test(key)) {
                event.preventDefault();
                return false;
            }
        }
    });
}

/*** END ACTIONS ***/

/*** NUEVA CARPETA ***/

function showNewFolder() {
    $("#divNuevaCarpeta").slideDown();
}

function cancelNewFolder() {
    $("#txtNuevaCarpeta").val("");
    $("#divNuevaCarpeta").slideUp();
}

function createNewFolder() {
    if ($("#txtNuevaCarpeta").val() != "") {

        $.ajax({
            type: "POST",
            url: "file-explorer.aspx/createFolder",
            data: "{path: '" + $("#hdnPath").val() + "', name:'" + $("#txtNuevaCarpeta").val() + "'}",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data, text) {
                loadInfo($("#hdnPath").val());
                cancelNewFolder();
            },
            error: function (response) {
                var r = jQuery.parseJSON(response.responseText);
                alert(r.Message);
            }
        });
    }
}

/*** END NUEVA CARPETA ***/

function enable_itemopt(enable) {
    if (enable) {
        $('.itemopt').removeClass('disabled');
    } else {

        // check all thumbs if no remaining checks
        // before we can disabled the options
        var ch = false;
        $('.thmb').each(function () {
            if ($(this).hasClass('checked'))
                ch = true;
        });

        if (!ch)
            $('.itemopt').addClass('disabled');
    }
}

function configResults() {
    $('.thmb').hover(function () {
        var t = $(this);
        t.find('.ckbox').show();
        t.find('.fm-group').show();
    }, function () {
        var t = $(this);
        if (!t.closest('.thmb').hasClass('checked')) {
            t.find('.ckbox').hide();
            t.find('.fm-group').hide();
        }
    });

    $('.ckbox').each(function () {
        var t = $(this);
        var parent = t.parent();
        if (t.find('input').is(':checked')) {
            t.show();
            parent.find('.fm-group').show();
            parent.addClass('checked');
        }
    });

    $('.ckbox').click(function () {
        var t = $(this);
        if (!t.find('input').is(':checked')) {
            t.closest('.thmb').removeClass('checked');
            enable_itemopt(false);
        } else {
            t.closest('.thmb').addClass('checked');
            enable_itemopt(true);
        }
    });

    //Replaces data-rel attribute to rel.
    //We use data-rel because of w3c validation issue
    $('a[data-rel]').each(function () {
        $(this).attr('rel', $(this).data('rel'));
    });

    $("a[rel^='prettyPhoto']").prettyPhoto({ social_tools: false });
    $("#selectall").attr('checked', false);
    $("#selectall").attr('disabled', false);

    enable_itemopt(false);

    $('#divUpload').slideUp();
}

function loadInfo(path) {
    $("#hdnPath").val(path);

    $("#selectall").attr('checked', false);
    $('.itemopt').addClass('disabled');

    $.ajax({
        type: "POST",
        url: "file-explorer.aspx/getInfo",
        data: "{path: '" + $("#hdnPath").val() + "'}",
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data, text) {

            $("#divPath").html(data.d.PathNavigation);

            // Render using the template
            if (data.d.TotalItems > 0) {
                $("#resultsContainer, #resultsFolders").empty();

                $("#resultTemplate").tmpl({ results: data.d.Folders }).appendTo("#resultsContainer");
                $("#resultTemplate").tmpl({ results: data.d.Files }).appendTo("#resultsContainer");
                $("#folderTemplate").tmpl({ results: data.d.Folders }).appendTo("#resultsFolders");

                if (data.d.Folders == "")
                    $("#resultsFolders").html("<li><a href='#'>No hay carpetas en esta ubicación</a></li>");

                configResults();
            }
            else {
                $("#resultsContainer").html("<h4 class='panel-title' style='margin-left: 15px;'>No hay nada subido. ¿Qué estas esperando?</h4>");

                $('#divUpload').slideDown();

                $("#resultsFolders").html("<li><a href='#'>No hay carpetas en esta ubicación</a></li>");
                //$("#divPath").empty();
                $("#selectall").attr('disabled', true);
            }
        },
        error: function (response) {
            var r = jQuery.parseJSON(response.responseText);
            alert(r.Message);
        }
    });
}

$(document).ready(function () {
    loadInfo("");
    $('#selectall').click(function () {
        if ($(this).is(':checked')) {
            $('.thmb').each(function () {
                $(this).find('input').attr('checked', true);
                $(this).addClass('checked');
                $(this).find('.ckbox, .fm-group').show();
            });
            enable_itemopt(true);
        } else {
            $('.thmb').each(function () {
                $(this).find('input').attr('checked', false);
                $(this).removeClass('checked');
                $(this).find('.ckbox, .fm-group').hide();
            });
            enable_itemopt(false);
        }
    });
    $('#txtNuevaCarpeta').bind('keypress', function (event) {
        var keycode = (event.keyCode ? event.keyCode : event.which);
        if (keycode == '13') {
            createNewFolder();
            return false;
        }
        else {
            var regex = new RegExp("^[A-za-z0-9\-_\\s\.\,]+$");
            var key = String.fromCharCode(!event.charCode ? event.which : event.charCode);
            if (!regex.test(key)) {
                event.preventDefault();
                return false;
            }
        }
    });
    enable_itemopt(false);
    Dropzone.options.myAwesomeDropzone = {
        paramName: "files", // The name that will be used to transfer the file
        maxFilesize: 5, // MB
        addRemoveLinks: false,
        //url: "/file-explorer.aspx/uploadFiles?path=" + $("#hdnPath").val()
        url: "/fileUpload.aspx?path=" + $("#hdnPath").val(),
        init: function () {
            // Set up any event handlers
            this.on("processing", function (file) {
                this.options.url = "/fileUpload.aspx?path=" + $("#hdnPath").val();
            });
            this.on('complete', function (data) {
                if (data.xhr.status != 200) {
                    var error = data.xhr.responseText.split("####")[0];
                    error = error.replace("\"", "")
                    $(".dz-error-message").html(error.replace("\"",""));
                }
                else {
                    if (this.getUploadingFiles().length === 0 && this.getQueuedFiles().length === 0) {
                        loadInfo($("#hdnPath").val());
                        $('#divUpload').slideUp();
                        this.removeAllFiles();
                    }
                }
            });
        }
    };
});