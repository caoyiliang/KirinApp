using KirinAppCore;
using KirinAppCore.Model;
using KirinAppCore.Test;
using Microsoft.Web.WebView2.Core;
using System.Timers;

namespace KirinAppCore.Test;
class Program
{
    public static KirinApp Kirin;
    [STAThread]
    static void Main()
    {
        WinConfig winConfig = new WinConfig()
        {
            AppName = "Test",
            Height = 800,
            Width = 1000,
            AppType = WebAppType.Static,
            BlazorComponent = typeof(App),
            Url = "Index.html",
            RawString = "<span style='color:red'>这个是字符串</span>",
            Icon = "logo.ico",
            Debug = true,
        };
        var kirinApp = Kirin = new KirinApp(winConfig);
        kirinApp.Loaded += (_, _) =>
        {
            Console.WriteLine(333);
            //await kirinApp.InjectJsObject("UserInfo", new
            // {
            //     userName = "admin",
            //     age = 18,
            //     sex = "男"
            // });
            //kirinApp.SetTopMost(true);
        };
        kirinApp.Created += async (_, _) =>
        {
            await Task.Delay(1000);
            Console.WriteLine(111);
        };
        kirinApp.OnCreate += (_, _) => { Console.WriteLine(000); };
        kirinApp.OnClose += (_, _) => { return true; };
        kirinApp.PositionChange += (s, e) => { Console.WriteLine(e.X + ":" + e.Y); };
        kirinApp.WebMessageReceived += (_, e) =>
        {
            if (e.Message.Contains("blazor"))
            {
                kirinApp.LoadBlazor<App>();

            }
            if (e.Message.Contains("reload")) kirinApp.Reload();
            if (e.Message.Contains("static")) kirinApp.LoadStatic("index.html");
            if (e.Message.Contains("string"))
            {
                kirinApp.LoadRawString("你好");
                Task.Run(async () =>
                {
                   await Task.Delay(3000); 
                    kirinApp.LoadStatic("index.html");
                    //kirinApp.LoadStatic("index.html")
                });

            }
        };
        kirinApp.WebMessageReceived += (_, e) =>
        {
            //kirinApp.Reload();
        };
        kirinApp.Run();
    }
}