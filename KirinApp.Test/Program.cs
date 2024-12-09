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
            AppType = WebAppType.Static,
            BlazorComponent = typeof(App),
            Url = "index.html",
            RawString = "<span style='color:red'>这个是字符串</span>",
            Icon = "logo.ico",
            Debug = true,
        };
        var kirinApp = new KirinApp(winConfig);
        kirinApp.Loaded += (_, _) =>
        {
            kirinApp.SendWebMessage("你好");
        };
        kirinApp.WebMessageReceived += (_, _) =>
        {
            Task.Run(() =>
            {
                var setApp = new KirinApp(kirinApp);
                setApp.AppName = "Setting";
                setApp.Icon = "logo.ico";
                setApp.Height = 400;
                setApp.Width = 600;
                setApp.UseBlazor<App>();
                setApp.Run();
            });
            //Blazor 两个一起创建，或者点击创建过快会报错
            Task.Run(() =>
            {
                var setApp2 = new KirinApp(kirinApp);
                setApp2.AppName = "Setting2";
                setApp2.Icon = "logo.ico";
                setApp2.Height = 400;
                setApp2.Width = 600;
                setApp2.UseBlazor<App>();
                setApp2.Run();
            });
        };
        kirinApp.Run();
    }
}
