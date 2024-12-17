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
        kirinApp.Loaded += async (_, _) =>
        {
            await Task.Delay(200);
            kirinApp.SendWebMessage("你好12312");
        };
        kirinApp.Created += (_, _) =>
        {
            Console.WriteLine(000);
        };
        kirinApp.OnLoad += (_, _) =>
        {
            Console.WriteLine(111);
        };
        kirinApp.OnCreate += (_, _) =>
        {
            Console.WriteLine(222);
        };
        kirinApp.OnClose += (_, _) =>
        {
            Console.WriteLine(222);
            return true;
        };
        kirinApp.PositionChange += (s, e) =>
        {
            Console.WriteLine(e.X + ":" + e.Y);
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
        };
        kirinApp.Run();
    }
}
