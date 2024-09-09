function loadScript(args) {

    var script = document.createElement('script');
    script.type = 'text/javascript';
    script.src = args.url;

    // Then bind the event to the callback function.
    // There are several events for cross browser compatibility.
    script.onreadystatechange = args.callback;
    script.onload = args.callback;

    var jinget = document.getElementById("jinget");

    //fire the loading
    jinget.after(script);
}

loadScript({
    url: '_content/Jinget.Blazor/js/jinget.select2.js',
    callback: loadScript({
        url: '_content/Jinget.Blazor/js/jinget.json.visualizer.js',
        callback: loadScript({
            //url: '_content/Jinget.Blazor/js/jinget.jalali.picker.date.js',
            //callback: loadScript({
            url: '_content/Jinget.Blazor/js/jinget.custom.js',
            callback: loadScript({
                url: '_content/Jinget.Blazor/js/jinget.select2ext.js',
                callback: loadScript({
                    url: '_content/Jinget.Blazor/js/jinget.select2tree.js'
                })
            })
            /*})*/
        })
    })
});

//load jinget bi resources
loadScript({
    url: '_content/Jinget.Blazor/js/jinget.bi.core.js',
    callback: loadScript({
        url: '_content/Jinget.Blazor/js/jinget.bi.gauge.js'
    })
});
