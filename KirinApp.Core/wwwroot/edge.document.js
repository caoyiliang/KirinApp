window.external = {
    sendMessage: message => {
        window.chrome.webview.postMessage(message);
    },
    receiveMessage: callback => {
        window.chrome.webview.addEventListener('message', e => {
            console.log(e.data)
            callback(e.data)
        });
    },
    max: () => {
        var obj = { cmd: 'max' }
        window.chrome.webview.postMessage(JSON.stringify(obj));
    },
    min: () => {
        var obj = { cmd: 'min' }
        window.chrome.webview.postMessage(JSON.stringify(obj));
    },
    hide: () => {
        var obj = { cmd: 'hide' }
        window.chrome.webview.postMessage(JSON.stringify(obj));
    },
    show: () => {
        var obj = { cmd: 'show' }
        window.chrome.webview.postMessage(JSON.stringify(obj));
    },
    focus: () => {
        var obj = { cmd: 'focus' }
        window.chrome.webview.postMessage(JSON.stringify(obj));
    },
    close: () => {
        var obj = { cmd: 'close' }
        window.chrome.webview.postMessage(JSON.stringify(obj));
    }
};