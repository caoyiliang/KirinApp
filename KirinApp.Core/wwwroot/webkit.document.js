window.__receiveMessageCallbacks = [];
window.__dispatchMessageCallback = function (message) {
    window.__receiveMessageCallbacks.forEach(function (callback) { callback(message); });
};
window.external = {
    sendMessage: function (message) {
        window.webkit.messageHandlers.KirinApp.postMessage(message);
    },
    receiveMessage: function (callback) {
        window.__receiveMessageCallbacks.push(callback);
    },
    max: () => {
        var obj = { cmd: 'max' }
        window.webkit.messageHandlers.KirinApp.postMessage(JSON.stringify(obj));
    },
    min: () => {
        var obj = { cmd: 'min' }
        window.webkit.messageHandlers.KirinApp.postMessage(JSON.stringify(obj));
    },
    hide: () => {
        var obj = { cmd: 'hide' }
        window.webkit.messageHandlers.KirinApp.postMessage(JSON.stringify(obj));
    },
    show: () => {
        var obj = { cmd: 'show' }
        window.webkit.messageHandlers.KirinApp.postMessage(JSON.stringify(obj));
    },
    focus: () => {
        var obj = { cmd: 'focus' }
        window.webkit.messageHandlers.KirinApp.postMessage(JSON.stringify(obj));
    },
    close: () => {
        var obj = { cmd: 'close' }
        window.webkit.messageHandlers.KirinApp.postMessage(JSON.stringify(obj));
    },
    drag: (selector) => {
        selector.addEventListener('mousedown', function (win) {
            const initialX = win.clientX;
            const initialY = win.clientY;

            const mouseMoveHandler = function (e) {
                const obj = {
                    cmd: "moveTo",
                    data: { x: e.clientX - initialX, y: e.clientY - initialY }
                };
                window.webkit.messageHandlers.KirinApp.postMessage(JSON.stringify(obj));
            };

            const mouseUpHandler = function () {
                document.removeEventListener('mousemove', mouseMoveHandler);
                document.removeEventListener('mouseup', mouseUpHandler);
            };

            document.addEventListener('mousemove', mouseMoveHandler);
            document.addEventListener('mouseup', mouseUpHandler);
        });
    },
    move: (x, y) => {
        var obj = { cmd: 'move', data: { x, y } }
        window.webkit.messageHandlers.KirinApp.postMessage(JSON.stringify(obj));
    },
    moveTo: (x, y) => {
        var obj = { cmd: 'moveTo', data: { x, y } }
        window.webkit.messageHandlers.KirinApp.postMessage(JSON.stringify(obj));
    },
    change: (width, height) => {
        var obj = { cmd: 'change', data: { width, height } }
        window.webkit.messageHandlers.KirinApp.postMessage(JSON.stringify(obj));
    },
    normal: () => {
        var obj = { cmd: 'normal' }
        window.webkit.messageHandlers.KirinApp.postMessage(JSON.stringify(obj));
    }
};