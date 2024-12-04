window.external = {
    sendMessage: message => {
        window.chrome.webview.postMessage(message);
    },
    receiveMessage: callback => {
        window.chrome.webview.addEventListener('message', e => callback(e.data));
    },
};