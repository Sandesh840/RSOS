$(document)
    .ajaxStart(function () {
        $('#ajax-loader').show();
    })
    .ajaxStop(function () {
        $('#ajax-loader').hide();
    })
    .ajaxComplete(function () {
        $('#ajax-loader').hide();
    });

$(document).ajaxError(function (xhr, props) {
    if (props.status === 308) {
        location.reload();
    }

    $('#ajax-loader').modal('hide');
});