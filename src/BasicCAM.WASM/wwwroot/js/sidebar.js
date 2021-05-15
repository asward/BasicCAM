var sidebarClicked = function (target) {

    var content = $('#' + target.attr('aria-controls'));

    var isActive = target.hasClass('active');

    //Hide everything
    $(".sidebar-item").removeClass('active');
    $(".slider").removeClass('slider-show');
    $(".slider").addClass('slider-hide');
    $(".slider-content").removeClass('content-show');
    $(".slider-content").addClass('content-hide');

    //hide entire slider
    console.log(isActive);

    //Show Selected thing
    if (!isActive) {
        target.addClass('active');
        $(".slider").removeClass('slider-hide');
        $(".slider").addClass('slider-show');
        content.removeClass('content-hide');
        content.addClass('content-show');
    }
};

var load = function () {
    $(document).ready(() => {
        sidebarClicked($('.sidebar-item.active'));
        $('.slider').css('width', "450px");
    });

    $('.sidebar-item').on('click', (e) => {
        sidebarClicked($(e.target))
    })

    var sliderMove = function (moveEvent, target) {
        var minWidth = 200;
        var maxWidth = 600;

        var newWidth = moveEvent.pageX;

        newWidth = newWidth < minWidth ? minWidth : newWidth;
        newWidth = newWidth > maxWidth ? maxWidth : newWidth;

        target.css('width', newWidth);
    }

    $('.slider-handle').mousedown(function (e, handler) {
        $($(e.target).data('target')).addClass('slider-active');
        window.Sidebar.resizeActive = true;
    });

    $(document).mousemove(function (e) {
        if (window.Sidebar.resizeActive)
            sliderMove(e,$('.slider-active'));
    });

    $(document).mouseup(function (e) {
        window.Sidebar.resizeActive = false;
        $('.slider-active').removeClass('slider-active');
    });
};
var initSearch = function () {
    var options = {
        listClass: 'property-list',
        item: 'property-row',
        valueNames: ['property-title']
    };

    var listObjcam = new List('cam', options);
    var listObjdocument = new List('tool', options);
    var listObjmachine = new List('machine', options);
    var listObjgcode = new List('gcode', options);

    $('#search').on('input', function () {
        var searchString = $(this).val();
        listObjgcode.search(searchString);
        listObjdocument.search(searchString);
        listObjcam.search(searchString);
        listObjmachine.search(searchString);
    });
}

window.Sidebar = {
    load: () => { load(); },
    initSearch: () => { initSearch();}
};