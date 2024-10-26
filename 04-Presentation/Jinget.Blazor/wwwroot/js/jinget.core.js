// function loadScript() {
//     let script = document.createElement('script');
//     script.setAttribute('data-main', 'js/require.con.js');
//     script.src = '_content/Jinget.Blazor/js/infra/require.js';
//
//     let jinget = document.getElementById("jinget");
//     //fire the loading
//     jinget.after(script);
//     return true;
// }
//
// loadScript();


function loadScript(args) {
    let script = document.createElement('script');
    script.type = 'text/javascript';
    script.src = '_content/Jinget.Blazor/js/' + args.url;

    // Then bind the event to the callback function.
    // There are several events for cross browser compatibility.
    script.onreadystatechange = args.callback;
    script.onload = args.callback;
    let jinget = document.getElementById("jinget");
    //fire the loading
    jinget.after(script);
    return true;
}

loadScript({
    // url: 'infra/RequireJS2.3.5.js',
    // callback: loadScript({
    url: "datepicker/jinget.dp.js",
    callback: loadScript({
        url: "datepicker/jinget.dp.datepicker.js",
        callback: loadScript({
            url: "datepicker/jinget.dp.daterangepicker.js",
            callback: loadScript({
                url: 'jsonvisualizer/jinget.json.visualizer.js',
                callback: loadScript({
                    url: 'common/jinget.custom.js'
                })
            })
        })
    })
    //})
});

//load bi resources
loadScript({
    url: 'bi/jinget.bi.core.js',
    callback: loadScript({
        url: 'bi/jinget.bi.gauge.js'
    })
});

//load dropdown resources
loadScript({
    url: 'dropdown/jinget.select2.js',
    callback: loadScript({
        url: 'dropdown/jinget.select2ext.js',
        callback: loadScript({
            url: "dropdown/jinget.select2tree.js"
        })
    })
});