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
            Height = 600,
            Width = 800,
            AppType = WebAppType.Static,
            BlazorComponent = typeof(App),
            Url = "Index.html",
            RawString = "<span style='color:red'>这个是字符串</span>",
            Icon = "logo.ico",
            Debug = true,
        };
        var kirinApp = new KirinApp(winConfig);
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
            kirinApp.LoadRawString("234234");

        };
        kirinApp.WebMessageReceived += (_, e) =>
        {
            Console.WriteLine(e);
        };
        kirinApp.Run();
    }
}