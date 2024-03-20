// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

/**INTENTO DEL TOAST

function borrarToast(controlador, metodo) {

    var accionToast = document.getElementById('borrarToast');
    var toast = new bootstrap.Toast(accionToast);
    toast.show();

    const data = {
        id: $('#Id').val(),
    };
    var url = '/' + controlador + '/' + metodo + ')'
    console.log(url);
    console.log(data);


    $.ajax({
        type: 'POST',
        url: url,
        data: JSON.stringify(data),
        contentType: 'application/json',
    })
        .done((data) => {
            console.log({ data });
        })
        .fail((err) => {
            console.error(err);
        })
        .always(() => {
            console.log('always called');
        });
};*/