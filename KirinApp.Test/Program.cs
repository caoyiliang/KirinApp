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
            AppType = WebAppType.Http,
            BlazorComponent = typeof(App),
            Url = "https://ops.zink.asia:28238/",
            RawString = "<span style='color:red'>这个是字符串</span>",
            Icon = "logo.ico",
            Debug = true,
        };
        var kirinApp = new KirinApp(winConfig);
        kirinApp.Created += (_, _) =>
        {
            Task.Run(() =>
            {
                Thread.Sleep(1000);
                kirinApp.Reload();
            });
        };
        kirinApp.Run();
    }
}
