using System;
using KirinAppCore;
using KirinAppCore.Model;
using KirinAppCore.Test;

class Program
{
    private static ManualResetEvent waitForInput = new ManualResetEvent(false);
    [STAThread]
    static void Main()
    {
        WinConfig winConfig = new WinConfig()
        {
            AppName = "Test",
            Height = 1200,
            Width = 1600,
            AppType = WebAppType.Blazor,
            BlazorComponent = typeof(App),
            Icon = "logo.ico"
        };
        var kirinApp = new KirinApp(winConfig);
        kirinApp.Loaded += async (_, _) =>
        {
            var js = "1+2";
            await kirinApp.ExecuteJavaScript(js);
        };

        Task.Run(() =>
        {
            Thread.Sleep(2000);
            kirinApp.OpenDevTool();
        });
        kirinApp.Run();
    }
}
