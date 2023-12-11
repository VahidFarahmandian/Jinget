function refreshDatePicker() {

    var done = false;
    const targetNode = document.querySelector("body");
    const config = { attributes: true, childList: true, subtree: true };
    var observer = new MutationObserver(() => {
        var CONTROL_INTERVAL = setInterval(function () {
            if (document.querySelectorAll('.mud-picker-calendar-day:not(.mud-hidden)').length > 0) {
                if (document.querySelectorAll('.mud-picker-calendar-day:not(.mud-hidden)')[0].textContent != '1') {
                    if (done == false) {
                        document.getElementsByClassName('mud-picker-nav-button-next')[0].click();
                        done = true;
                    }
                    clearInterval(CONTROL_INTERVAL);
                    observer.disconnect();
                }
                else {
                    clearInterval(CONTROL_INTERVAL);
                    observer.disconnect();
                }
            }
        }, 100);
    });
    observer.observe(targetNode, config);
}

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

loadScript(
    {
        url: '../_content/MudBlazor/MudBlazor.min.js',
        callback: loadScript(
            {
                url: '_content/Jinget.Blazor/js/jinget.jalali.picker.date.js'
            })
    });