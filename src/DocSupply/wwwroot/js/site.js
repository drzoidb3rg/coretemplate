// Write your Javascript code.

$(document).ready(function () {

    $('.dropdown-menu li').click(function (e) {
        e.stopPropagation();
    });
    $('.dropdown-menu a').click(function (e) {
        var target = $(this).attr("data-target");
        $(target).modal().on('hidden.bs.modal', function (ev) { $(".dropdown-toggle").dropdown('toggle'); });
    });

    $('input:file').change(
           function () {
               if ($(this).val()) {
                   $('input:submit').attr('disabled', false);
               } else {
                   $('input:submit').attr('disabled', true);
               }
           });

});