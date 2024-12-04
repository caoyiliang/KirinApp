using KirinAppCore.Interface;
using KirinAppCore.Model;
using KirinAppCore.Plateform.Windows;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace KirinAppCore;

public class KirinApp
{
    public event EventHandler<EventArgs>? OnCreate;
    public event EventHandler<EventArgs>? Created;
    public event EventHandler<EventArgs>? OnLoad;
    public event EventHandler<EventArgs>? Loaded;
    internal IWindow Window { get; private set; }
    protected ServiceProvider ServiceProvide { get; private set; }
    private WinConfig Config = new();
    IServiceCollection serviceCollection = new ServiceCollection();
    public KirinApp(WinConfig winConfig)
    {
        Config = winConfig;
        ServiceProvide = InitPlateform();
        Window = ServiceProvide.GetRequiredService<IWindow>();
        Window.Init(ServiceProvide, Config);
        Window.OnCreate += (s, e) => OnCreate?.Invoke(s, e);
        Window.Created += (s, e) => Created?.Invoke(s, e);
        Window.OnLoad += (s, e) => OnLoad?.Invoke(s, e);
        Window.Loaded += (s, e) => Loaded?.Invoke(s, e);
    }

    public void Run()
    {
        Window.Show();
        Window.MessageLoop();
    }

    private ServiceProvider InitPlateform()
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
        RegistResource();
        return serviceCollection.BuildServiceProvider();
    }

    private void RegistResource()
    {
        List<IFileProvider> containers = new List<IFileProvider>();
        containers.Add(new ManifestEmbeddedFileProvider(typeof(Microsoft.AspNetCore.Components.WebView.WebViewManager).Assembly));
        IFileProvider provider = new CompositeFileProvider(containers);
        serviceCollection.AddSingleton(provider);
    }

    public async Task<string> ExecuteJavaScript(string js) => await Window.ExecuteJavaScript(js);

    public void OpenDevTool() => Window.OpenDevTool();
}
