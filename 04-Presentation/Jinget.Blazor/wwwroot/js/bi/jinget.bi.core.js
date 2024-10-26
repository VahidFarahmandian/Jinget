window.toJingetGauge = (params = { id, width, hueLow, hueHigh, saturation, lightness, gaugeBackGroundColor, gaugeParentElementGroundColor } = {}) => {
    $('#' + params.id).simpleGauge({
        id: params.id,
        width: params.width,
        hueLow: params.hueLow,
        hueHigh: params.hueHigh,
        saturation: params.saturation,
        lightness: params.lightness,
        gaugeBG: params.gaugeBackGroundColor,
        parentBG: params.gaugeParentElementGroundColor
    });
}
window.setJingetGaugeValue = (params = { id, value, width, hueLow, hueHigh, saturation, lightness, gaugeBackGroundColor, gaugeParentElementGroundColor } = {}) => {
    $('#' + params.id).setGaugeValue(params.value, {
        width: params.width,
        hueLow: params.hueLow,
        hueHigh: params.hueHigh,
        saturation: params.saturation,
        lightness: params.lightness,
        gaugeBG: params.gaugeBackGroundColor,
        parentBG: params.gaugeParentElementGroundColor
    });
}