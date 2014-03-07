function sideBarToogle(element, notAuto) {
    $('#header').toggleClass('mini');
    $('#sidebar').toggleClass('mini');
    $('#content').toggleClass('mini');
    $(element).children('i.icon').toggleClass('icon-chevron-right');
    $('#sidebar [title]').data("placement", "right");

    if ($('#header').hasClass('mini')) {
        sessionStorage.mini = 'on';
        $('#sidebar [title]').tooltip();
    }
    else {
        sessionStorage.mini = 'off';
        $('#sidebar [title]').tooltip('destroy');
    }
}

var changeSideBar = function () {
    if ($(window).width() <= 1028) {
        $('#header').addClass('mini');
        $('#sidebar').addClass('mini');
        $('#content').addClass('mini');
        $("#sidebartoogle").children('i.icon').addClass('icon-chevron-right');
        $('#sidebar [title]').data("placement", "right");
        $('#sidebar [title]').tooltip();
    }
    else {
        if (!sessionStorage.mini || sessionStorage.mini != 'on') {
            $("#sidebartoogle").children('i.icon').removeClass('icon-chevron-right');
            $('#header').removeClass('mini');
            $('#sidebar').removeClass('mini');
            $('#content').removeClass('mini');
            $('#sidebar [title]').tooltip('destroy');
        }
    }
}

var elements = {};

$("[data-val-required]").each(function (index, el) {
    elements[($(el).attr('name'))] = { required: true }
});

$("form").validate({
    rules: elements,
    errorClass: "help-inline",
    errorElement: "span",
    highlight: function (element, errorClass, validClass) {
        $(element).addClass('error')
    },
    unhighlight: function (element, errorClass, validClass) {
        $(element).removeClass('error')
        $(element).addClass('success');
    }
});