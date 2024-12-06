using KirinAppCore.Interface;
using KirinAppCore.Model;
using KirinAppCore.Plateform.Windows;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace KirinAppCore;

public class KirinApp
{
    private WinConfig Config = new();
    internal IWindow Window { get; private set; }

    protected ServiceProvider ServiceProvide { get; private set; }
    IServiceCollection serviceCollection = new ServiceCollection();

    public event EventHandler<EventArgs>? OnCreate;
    public event EventHandler<EventArgs>? Created;
    public event EventHandler<EventArgs>? OnLoad;
    public event EventHandler<EventArgs>? Loaded;
    public event NetClosingDelegate? OnClose;
    public delegate bool? NetClosingDelegate(object sender, EventArgs e);
    public event EventHandler<CoreWebView2WebMessageReceivedEventArgs>? WebMessageReceived;
    public event EventHandler<SizeChangeEventArgs>? SizeChange;

    /// <summary>
    /// 主显示器
    /// </summary>
    public Model.Monitor MainMonitor => Window.MainMonitor;

    public KirinApp()
    {
        InitPlateform();
        RegistResource();
        ServiceProvide = serviceCollection.BuildServiceProvider();
        Window = ServiceProvide.GetRequiredService<IWindow>();

        EventRegister();
    }
    public KirinApp(WinConfig winConfig)
    {
        Config = winConfig;
        InitPlateform();
        RegistResource();
        ServiceProvide = serviceCollection.BuildServiceProvider();
        Window = ServiceProvide.GetRequiredService<IWindow>();

        EventRegister();
    }

    private void EventRegister()
    {
        Window.OnCreate += (s, e) => OnCreate?.Invoke(s, e);
        Window.Created += (s, e) => Created?.Invoke(s, e);
        Window.OnLoad += (s, e) => OnLoad?.Invoke(s, e);
        Window.Loaded += (s, e) => Loaded?.Invoke(s, e);
        Window.OnClose += (s, e) => OnClose?.Invoke(s, e);
        Window.WebMessageReceived += (s, e) => WebMessageReceived?.Invoke(s, e);
        Window.SizeChangeEvent += (s, e) => SizeChange?.Invoke(s, e);
    }

    public void Run()
    {
        Window.Init(ServiceProvide, Config);
        Window.Show();
        Window.MessageLoop();
    }

    private void InitPlateform()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            serviceCollection.AddSingleton<IWindow, MainWIndow>();
        }
        else
        {
            serviceCollection.AddSingleton<IWindow, MainWIndow>();
        }
        if (Config.AppType != WebAppType.Http)
        {
            serviceCollection.AddSingleton<JSComponentConfigurationStore>();
            serviceCollection.AddBlazorWebView();
        }
    }

    private void RegistResource()
    {
        List<IFileProvider> containers = new List<IFileProvider>();
        containers.Add(new ManifestEmbeddedFileProvider(typeof(Microsoft.AspNetCore.Components.WebView.WebViewManager).Assembly));
        IFileProvider provider = new CompositeFileProvider(containers);
        serviceCollection.AddSingleton(provider);
    }

    #region 通过方法修改Config的属性
    public KirinApp SetAppName(string name)
    {
        Config.AppName = name;
        return this;
    }
    public KirinApp SetIco(string path)
    {
        Config.Icon = path;
        return this;
    }
    public KirinApp SetSize(int width, int height)
    {
        Config.Height = height;
        Config.Width = width;
        return this;
    }

    public KirinApp SetSize(Size size)
    {
        Config.Size = size;
        return this;
    }


    public KirinApp Chromless(bool b = true)
    {
        Config.Chromless = b;
        return this;
    }

    public KirinApp Debug(bool b = true)
    {
        Config.Debug = b;
        return this;
    }

    public KirinApp SetAppType(WebAppType appType)
    {
        Config.AppType = appType;
        return this;
    }

    public KirinApp SetPosition(int left, int top)
    {
        Config.Left = left;
        Config.Top = top;
        return this;
    }

    public KirinApp ResizeAble(bool b = true)
    {
        Config.ResizeAble = b;
        return this;
    }

    public KirinApp Center(bool b = true)
    {
        Config.Center = b;
        return this;
    }

    public KirinApp SetUrl(string url)
    {
        Config.Url = url;
        Reload();
        return this;
    }

    public KirinApp SetRawString(string rawString)
    {
        Config.RawString = rawString;
        Reload();
        return this;
    }

    public KirinApp SetMinSize(int width, int heigth)
    {
        Config.MinimumWidth = width;
        Config.MinimumHeigh = heigth;
        return this;
    }

    public KirinApp SetMinSize(Size size)
    {
        Config.MinimumSize = size;
        return this;
    }

    public KirinApp SetMaxSize(int width, int heigth)
    {
        Config.MaximumWidth = width;
        Config.MaximumHeigh = heigth;
        return this;
    }

    public KirinApp SetMaxSize(Size size)
    {
        Config.MaximumSize = size;
        return this;
    }

    public KirinApp UseRawString(string rawString)
    {
        Config.AppType = WebAppType.RawString;
        Config.RawString = rawString;
        return this;
    }

    public KirinApp UseStatic(string path)
    {
        Config.AppType = WebAppType.Static;
        Config.Url = path;

        //重新获取依赖注入
        InitPlateform();
        RegistResource();
        ServiceProvide = serviceCollection.BuildServiceProvider();
        Window = ServiceProvide.GetRequiredService<IWindow>();
        return this;
    }

    public KirinApp UseHttp(string url)
    {
        Config.AppType = WebAppType.Http;
        Config.Url = url;

        //重新获取依赖注入
        InitPlateform();
        RegistResource();
        ServiceProvide = serviceCollection.BuildServiceProvider();
        Window = ServiceProvide.GetRequiredService<IWindow>();
        return this;
    }

    public KirinApp UseBlazor<T>(string selector = "#app")
    {
        Config.AppType = WebAppType.Blazor;
        if (!selector.Contains("#")) selector = "#" + selector;
        Config.BlazorSelector = selector;
        Config.BlazorComponent = typeof(T);

        //重新获取依赖注入
        InitPlateform();
        RegistResource();
        ServiceProvide = serviceCollection.BuildServiceProvider();
        Window = ServiceProvide.GetRequiredService<IWindow>();
        return this;
    }

    #endregion

    public (bool selected, DirectoryInfo? dir) OpenDirectory(string initialDir = "") => Window.OpenDirectory(initialDir);
    public (bool selected, FileInfo? file) OpenFile(string filePath = "") => Window.OpenFile(filePath);
    public (bool selected, List<FileInfo>? files) OpenFiles(string filePath = "") => Window.OpenFiles(filePath);
    public MsgResult ShowDialog(string title, string message, MsgBtns btns = MsgBtns.OK) => Window.ShowDialog(title, message, btns);
    public void ExecuteJavaScript(string js) => Window.ExecuteJavaScript(js);
    public string ExecuteJavaScriptWithResult(string js) => Window.ExecuteJavaScriptWithResult(js);
    public void OpenDevTool() => Window.OpenDevTool();
    public void Reload() => Window.Reload();
    public void ReloadUrl(string url)
    {
        SetUrl(url);
        Reload();
    }
    public void ReloadRawString(string content)
    {
        SetRawString(content);
        Reload();
    }
}