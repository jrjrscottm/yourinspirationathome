$('.button-checkbox').each(function () {

    // Settings
    var $widget = $(this),
        $button = $widget.find('button'),
        $checkbox = $widget.find('input:checkbox'),
        $cbValue = $("#TermsAccepted"),
        color = $button.data('color'),
        settings = {
            on: {
                icon: 'glyphicon glyphicon-check'
            },
            off: {
                icon: 'glyphicon glyphicon-unchecked'
            }
        };

    // Event Handlers
    $button.on('click', function () {
        $checkbox.prop('checked', !$checkbox.is(':checked'));
        $cbValue.prop("value", $checkbox.is(':checked'));
        $checkbox.triggerHandler('change');
        updateDisplay();
    });
    $checkbox.on('change', function () {
        updateDisplay();
    });

    // Actions
    function updateDisplay() {
        var isChecked = $checkbox.is(':checked');
        
        // Set the button's state
        $button.data('state', (isChecked) ? "on" : "off");

        // Set the button's icon
        $button.find('.state-icon')
            .removeClass()
            .addClass('state-icon ' + settings[$button.data('state')].icon);

        // Update the button's color
        if (isChecked) {
            $button
                .removeClass('btn-default')
                .addClass('btn-' + color + ' active');
        }
        else {
            $button
                .removeClass('btn-' + color + ' active')
                .addClass('btn-default');
        }
    }

    // Initialization
    function init() {

        updateDisplay();

        // Inject the icon if applicable
        if ($button.find('.state-icon').length === 0) {
            $button.prepend('<i class="state-icon ' + settings[$button.data('state')].icon + '"></i>');
        }
    }
    init();

    var clickEvent = false;
    $('#myCarousel').carousel({
        interval: 4000
    }).on('click', '.list-group li', function () {
        clickEvent = true;
        $('.list-group li').removeClass('active');
        $(this).addClass('active');
    }).on('slid.bs.carousel', function (e) {
        if (!clickEvent) {
            var count = $('.list-group').children().length - 1;
            var current = $('.list-group li.active');
            current.removeClass('active').next().addClass('active');
            var id = parseInt(current.data('slide-to'));
            if (count == id) {
                $('.list-group li').first().addClass('active');
            }
        }
        clickEvent = false;
    });

    $('#myCarousel').carousel({
        interval: 5000
    });

    $('#carousel-text').html($('#slide-content-0').html());

    //Handles the carousel thumbnails
    $('[id^=carousel-selector-]').click(function () {
        var id = this.id.substr(this.id.lastIndexOf("-") + 1);
        var id = parseInt(id);
        $('#myCarousel').carousel(id);
    });


    // When the carousel slides, auto update the text
    $('#myCarousel').on('slid.bs.carousel', function (e) {
        var id = $('.item.active').data('slide-number');
        $('#carousel-text').html($('#slide-content-' + id).html());
    });



});


$(window).load(function () {
    var boxheight = $('#myCarousel .carousel-inner').innerHeight();
    var itemlength = $('#myCarousel .item').length;
    var triggerheight = Math.round(boxheight / itemlength + 1);
    $('#myCarousel .list-group-item').outerHeight(triggerheight);
});