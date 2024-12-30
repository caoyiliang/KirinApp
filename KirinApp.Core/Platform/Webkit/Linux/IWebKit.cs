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
    string ExecuteJavaScript(string js);
    void InjectJsObject(string name, object obj);
    void OpenDevTool();
    void SendWebMessage(string message);
    void Reload();
    void Navigate(string url);
}
