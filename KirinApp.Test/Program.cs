using KirinAppCore;
using KirinAppCore.Model;
using KirinAppCore.Test;

class Program
{
    [STAThread]
    static void Main()
    {
        WinConfig winConfig = new WinConfig()
        {
            AppName = "Test",
            Height = 800,
            Width = 1000,
            AppType = WebAppType.Blazor,
            BlazorComponent = typeof(App),
            Url = "Index.html",
            RawString = "<span style='color:red'>这个是字符串</span>",
            Icon = "logo.ico",
            Debug = true,
        };
        var kirinApp = new KirinApp(winConfig);
        kirinApp.Loaded += async (_, _) =>
        {
            await Task.Delay(100);
            Console.WriteLine(333);
        };
        kirinApp.Created += (_, _) =>
        {
            Console.WriteLine(111);
        };
        kirinApp.OnLoad += (_, _) =>
        {
            Console.WriteLine(222);
        };
        kirinApp.OnCreate += (_, _) =>
        {
            Console.WriteLine(000);
        };
        kirinApp.OnClose += (_, _) =>
        {
            return true;
        };
        kirinApp.PositionChange += (s, e) =>
        {
            Console.WriteLine(e.X + ":" + e.Y);
        };
        kirinApp.WebMessageReceived += (_, e) =>
        {
            Console.WriteLine(e.Message);
            //kirinApp.SendWebMessage("你好Ubuntu");
        };
        kirinApp.Run();
    }
}
