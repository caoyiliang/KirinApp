window.__receiveMessageCallbacks = [];
window.__dispatchMessageCallback = function (message) {
    window.__receiveMessageCallbacks.forEach(function (callback) { callback(message); });
};
window.external = {
    sendMessage: function (message) {
        window.webkit.messageHandlers.CodeDeskInterop.postMessage(message);
    },
    receiveMessage: function (callback) {
        window.__receiveMessageCallbacks.push(callback);
    },
    max: () => {
        var obj = { cmd: 'max' }
        window.webkit.messageHandlers.CodeDeskInterop.postMessage(JSON.stringify(obj));
    },
    min: () => {
        var obj = { cmd: 'min' }
        window.webkit.messageHandlers.CodeDeskInterop.postMessage(JSON.stringify(obj));
    },
    hide: () => {
        var obj = { cmd: 'hide' }
        window.webkit.messageHandlers.CodeDeskInterop.postMessage(JSON.stringify(obj));
    },
    show: () => {
        var obj = { cmd: 'show' }
        window.webkit.messageHandlers.CodeDeskInterop.postMessage(JSON.stringify(obj));
    },
    focus: () => {
        var obj = { cmd: 'focus' }
        window.webkit.messageHandlers.CodeDeskInterop.postMessage(JSON.stringify(obj));
    },
    close: () => {
        var obj = { cmd: 'close' }
        window.webkit.messageHandlers.CodeDeskInterop.postMessage(JSON.stringify(obj));
    }
};