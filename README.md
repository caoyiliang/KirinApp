### 这是什么
这是一个基于.NET的桌面应用开发框架，你可以使用任何前端框架配合.NET来开发你的桌面应用。

### 优点
支持多平台(Windows，linux，macos)，支持多前端框架(vue，react，angular，blazor等)，打包体积小，最小不到5m，启动速度快。

### 安装
1、nuget搜索kirinApp，下载即可。

2、克隆此仓库，引用项目KirinAppCore

### 创建应用
1、下载安装`.net 8.0`

2、创建一个基于.net8的控制台项目，右键属性，把应用程序-常规-输出类型修改为Windows应用程序。如果要用blazor支持，还需要右键项目，编辑项目文件，第一行的`<Project Sdk="Microsoft.NET.Sdk">`改为`<Project Sdk="Microsoft.NET.Sdk.Razor">`

3、创建窗体 ***如果涉及更新UI请勿在异步或者非主线程中更新***
``` C#
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
            BlazorComponent = typeof(App),//AppType类型是blazor，必须设置
            BlazorSelector = "#app",//可选，默认是#app，如果你修改了组件入口id，则必须设置
            Url = "Index.html",//AppType类型是Static或者http必须设置，如果是http则是一个完整的uri地址，如果是Static则是相对路径（相对于软件根目录）
            RawString = "<span style='color:red'>这个是字符串</span>",AppType类型RawString
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
### API
此框架不内嵌api接口，需要自己实现（配置和新建的webapi一样）,参考如下
```C#
public static WebApplication InitAPI()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddControllers().AddApplicationPart(Assembly.GetExecutingAssembly());//AddApplicationPart(Assembly.GetExecutingAssembly())是为了解决不在api项目调用启用api服务会获取不到接口

        //自行按照需求增加配置
        var app = builder.Build();

        //自定义异常捕获中间件
        app.UseMiddleware<ExceptionMiddleware>();

        app.UseStaticFiles(new StaticFileOptions
        {
            //设置不限制content-type
            ServeUnknownFileTypes = true
        });
        app.UseCors(builder => builder
            //允许任何来源
            .AllowAnyOrigin()
             //所有请求方法
             .AllowAnyMethod()
             //所有请求头
             .AllowAnyHeader());
        app.UseRouting();

        app.MapControllers();

        return app;
    }
InitApI().Run();//InitApI().RunAsync();
```
### 注
 - 暂时不支持macos系统，后续添加；
 - 框架还不是非常完善，功能有可能欠缺，会不断修复和开发新功能；
 - 使用过程中如果有bug，请提交issue；
 - 欢迎大家提交代码参与开发；
 - 如果可以，请点亮star；

### 开发计划
***由于是新框架，bug可能不少，所以以修复bug为主，开发新功能为辅，所以新功能会很慢***
 - MacOS支持
 - 系统托盘支持
 - 系统级消息气泡通知

### 图片展示
##### 静态资源
![image](https://github.com/user-attachments/assets/00a70b0c-a4e9-43f5-afc4-9aa9e2541f40)
##### 网页
![image](https://github.com/user-attachments/assets/7393136f-69d5-49e6-a451-a84ef7027fac)
##### blazor
![image](https://github.com/user-attachments/assets/f7ed8d1b-7125-412f-9ecb-2eb141f0b10a)
##### 字符串
![image](https://github.com/user-attachments/assets/0c9ab315-a68e-4a63-9f41-5030d600aac9)




