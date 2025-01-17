### 这是什么
这是一个基于.NET的桌面应用开发框架，你可以使用任何前端框架(vue，react，angular，blazor等)配合.NET来开发你的桌面应用。

### 优点
支持多平台，支持多前端框架，打包体积小，最小不到5m，启动速度快。

### 安装
1、nuget搜索kirinApp，下载即可。
2、克隆此仓库，引用项目KirinAppCore

### 创建应用
1、下载安装`.net 8.0`
2、创建一个基于.net8的控制台项目，右键属性，把应用程序-常规-输出类型修改为Windows应用程序。如果要用blazor支持，还需要右键项目，编辑项目文件，第一行的`<Project Sdk="Microsoft.NET.Sdk">`改为`<Project Sdk="Microsoft.NET.Sdk.Razor">`
3、创建窗体
    代码块
    ``` CSharp
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
            AppName = "KirinApp",
            Height = 600,
            Width = 800,
            AppType = WebAppType.Static,//Static和http加载url，RawString加载RawString
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
            if (e.Message.Contains("blazor"))
                kirinApp.LoadBlazor<App>();
        };
        kirinApp.WebMessageReceived += (_, e) =>
        {
            Console.WriteLine(e);
        };
        kirinApp.Run();
    }
}
    ```