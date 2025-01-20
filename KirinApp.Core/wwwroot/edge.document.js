window.external = {
    sendMessage: message => {
        window.chrome.webview.postMessage(message);
    },
    receiveMessage: callback => {
        window.chrome.webview.addEventListener('message', e => {
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
                window.external.sendMessage(JSON.stringify(obj));
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
        window.chrome.webview.postMessage(JSON.stringify(obj));
    },
    moveTo: (x, y) => {
        var obj = { cmd: 'moveTo', data: { x, y } }
        window.chrome.webview.postMessage(JSON.stringify(obj));
    },
    change: (width, height) => {
        var obj = { cmd: 'change', data: { width, height } }
        window.chrome.webview.postMessage(JSON.stringify(obj));
    },
    normal: () => {
        var obj = { cmd: 'normal' }
        window.chrome.webview.postMessage(JSON.stringify(obj));
    }
};