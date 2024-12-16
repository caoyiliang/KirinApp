using KirinAppCore.Plateform.Webkit.Linux.Models;
using KirinAppCore.Platform.Webkit.Linux;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace KirinAppCore.Plateform.Webkit.Linux;

internal class WebKit40 : IWebKit
{
    private const string Libraries = "libwebkit2gtk-4.0.so.37";
    public void ExecuteJavaScript(string js)
    {
        throw new NotImplementedException();
    }

    public string ExecuteJavaScriptWithResult(string js)
    {
        throw new NotImplementedException();
    }

    public Task InitWebControl()
    {
        throw new NotImplementedException();
    }

    public void OpenDevTool()
    {
        throw new NotImplementedException();
    }

    public void Reload()
    {
        throw new NotImplementedException();
    }

    public void SendWebMessage(string message)
    {
        throw new NotImplementedException();
    }
}
internal class WebKit41 : IWebKit
{
    private const string Libraries = "libwebkit2gtk-4.1.so.0";
    public void ExecuteJavaScript(string js)
    {
        throw new NotImplementedException();
    }

    public string ExecuteJavaScriptWithResult(string js)
    {
        throw new NotImplementedException();
    }

    public Task InitWebControl()
    {
        throw new NotImplementedException();
    }

    public void OpenDevTool()
    {
        throw new NotImplementedException();
    }

    public void Reload()
    {
        throw new NotImplementedException();
    }

    public void SendWebMessage(string message)
    {
        throw new NotImplementedException();
    }
}
