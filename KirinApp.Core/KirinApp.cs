using KirinAppCore.Interface;
using KirinAppCore.Model;
using KirinAppCore.Plateform.Windows;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
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

    public KirinApp UseDebug()
    {
        Config.Debug = true;
        return this;
    }

    public KirinApp UseSystemTary()
    {
        Config.UseSystemTray = true;
        return this;
    }

    #endregion

    public void ExecuteJavaScript(string js) => Window.ExecuteJavaScript(js);
    public string ExecuteJavaScriptWithResult(string js) => Window.ExecuteJavaScriptWithResult(js);
    public void OpenDevTool() => Window.OpenDevTool();
}
