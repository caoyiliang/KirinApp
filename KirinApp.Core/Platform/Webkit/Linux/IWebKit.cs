using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KirinAppCore.Platform.Webkit.Linux;

internal interface IWebKit
{
    Task InitWebControl();
    void ExecuteJavaScript(string js);
    string ExecuteJavaScriptWithResult(string js);
    void OpenDevTool();
    void SendWebMessage(string message);
    void Reload();
}
