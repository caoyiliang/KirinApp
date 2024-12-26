using KirinAppCore.Interface;
using KirinAppCore.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KirinAppCore.Platform.Webkit.Linux;

internal interface IWebKit
{
    event EventHandler<WebMessageEvent>? WebMessageReceived;
    void InitWebControl(IWindow window);
    void ExecuteJavaScript(string js);
    string ExecuteJavaScriptWithResult(string js);
    void OpenDevTool();
    void SendWebMessage(string message);
    void Reload();
    void Navigate(string url);
    void Change();
}
