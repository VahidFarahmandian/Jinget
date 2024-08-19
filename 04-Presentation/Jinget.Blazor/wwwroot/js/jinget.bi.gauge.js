(function ($) {

    var gaugeDefaults = {
        width: '200',
        hueLow: '1', // Choose the starting hue for the active color (for value 0)
        hueHigh: '128', // Choose the ending hue for the active color (for value 100)
        saturation: '100%', // Saturation for active color
        lightness: '50%', // Lightness for active color
        gaugeBG: '#1b1b1f', // Background color of Gauge
        parentBG: '#323138' // This color should match the parent div of the gauge (or beyond)
    };
    $.fn.simpleGauge = function (options) {

        // Defaults
        var settings = $.extend({}, gaugeDefaults, options);

        $(this).each(function () {

            // Color & Data Settings
            var value = $(this).data('value');

            // Add DOM to allow for CSS3 Elements (would have been more elegant to use :before & :after pseudo-elements, but jQuery doesn't support them)
            $(this).prepend(
                '<div class="gauge-wrap-before"></div>' +
                '<div class="gauge-core">' +
                '<div class="gauge-bg"></div>' +
                '<div class="gauge-active-wrap">' +
                '<div class="gauge-active">' +
                '<div class="gauge-active-before"></div>' +
                '</div>' +
                '</div>' +
                '<div class="gauge-cap"></div>' +
                '</div>' +
                '<div class="gauge-wrap-after"></div>');
            $(this).setGaugeValue(value, settings);
        });
    };
    $.fn.setGaugeValue = function (value, options) {
        var settings = $.extend({}, gaugeDefaults, options);
        $(this).data('value', value);

        var activeColor = '';

        if (settings.hueHigh >= settings.hueLow) {
            activeColor = ((settings.hueHigh - settings.hueLow) * (value / 100)) + settings.hueLow;
        } else {
            activeColor = ((settings.hueLow - settings.hueHigh) * (value / 100)) + settings.hueHigh;
        }

        if (value) {
            $(this).find('.gauge-active, .gauge-wrap-before')
                .css('background-color', 'hsla(' + Math.round(activeColor) + ', ' + settings.saturation + ', ' + settings.lightness + ', 1)');
        }

        $(this).find('.gauge-bg, .gauge-wrap-after').css('background-color', settings.gaugeBG);
        $(this).find('.gauge-cap').css('background-color', settings.parentBG);

        $(this).find('.gauge-active-wrap').css({
            '-webkit-transform': 'rotate(' + (value * 1.8) + 'deg)',
            '-moz-transform': 'rotate(' + (value * 1.8) + 'deg)',
            '-ms-transform': 'rotate(' + (value * 1.8) + 'deg)',
            '-o-transform': 'rotate(' + (value * 1.8) + 'deg)',
            'transform': 'rotate(' + (value * 1.8) + 'deg)',
        });
    }

})(jQuery);
