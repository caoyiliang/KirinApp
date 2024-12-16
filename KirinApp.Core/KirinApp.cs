using KirinAppCore.Interface;
using KirinAppCore.Model;
using KirinAppCore.Plateform.WebView2.Windows;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace KirinAppCore;

public class KirinApp
{
    internal WinConfig Config = new();
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
    public OSPlatform OS { get; private set; } = OSPlatform.Windows;
    public OperatingSystem OsVersion { get; private set; } = Environment.OSVersion;

    public KirinApp(KirinApp? parent = null)
    {
        InitPlateform();
        RegistResource();
        ServiceProvide = serviceCollection.BuildServiceProvider();
        Window = ServiceProvide.GetRequiredService<IWindow>();
        Window.ParentWindows = parent;

        //获取主显示器宽高
        Win32Api.SetProcessDPIAware();
        Window.SetScreenInfo();
    }
    public KirinApp(WinConfig winConfig, KirinApp? parent = null)
    {
        Config = winConfig;
        InitPlateform();
        RegistResource();
        ServiceProvide = serviceCollection.BuildServiceProvider();
        Window = ServiceProvide.GetRequiredService<IWindow>();
        Window.ParentWindows = parent;

        //获取主显示器宽高
        Win32Api.SetProcessDPIAware();
        Window.SetScreenInfo();
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
        if (Utils.MainThreadId == 0)
            Utils.MainThreadId = Environment.CurrentManagedThreadId;
        if (Window.CheckAccess() && Parent == null && Utils.Wnds.Count == 0)
        {
            Window.Init(ServiceProvide, Config);
            EventRegister();
            Window.Show();
            Utils.Wnds.Add(this);
            Window.MessageLoop();
        }
        else
        {
            IntPtr actionPtr = Marshal.GetFunctionPointerForDelegate(() =>
            {
                Window.Init(ServiceProvide, Config);
                EventRegister();
                Window.Show();
                Utils.Wnds.Add(this);
                Window.MessageLoop();
            });
            Win32Api.PostMessage(Utils.Wnds[0].Window.Handle, (uint)WindowMessage.DIY_FUN, actionPtr, IntPtr.Zero);
        };
    }

    private void InitPlateform()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            serviceCollection.AddSingleton<IWindow, KirinAppCore.Plateform.WebView2.Windows.MainWIndow>();
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            serviceCollection.AddSingleton<IWindow, KirinAppCore.Plateform.Webkit.Linux.MainWIndow>();
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            serviceCollection.AddSingleton<IWindow, KirinAppCore.Plateform.Webkit.MacOS.MainWIndow>();

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
    public KirinApp? Parent { get => Window.ParentWindows; set => Window.ParentWindows = value; }

    public string AppName { get => Config.AppName; set => Config.AppName = value; }
    public KirinApp SetAppName(string name)
    {
        Config.AppName = name;
        return this;
    }
    public string Icon { get => Config.Icon; set => Config.Icon = value; }
    public KirinApp SetIcon(string path)
    {
        Config.Icon = path;
        return this;
    }
    public int Width { get => Config.Width; set => Config.Width = value; }
    public int Height { get => Config.Height; set => Config.Height = value; }
    public KirinApp SetSize(int width, int height)
    {
        Config.Height = height;
        Config.Width = width;
        return this;
    }

    public System.Drawing.Size? Size { get => Config.Size; set => Config.Size = value; }
    public KirinApp SetSize(Size size)
    {
        Config.Size = size;
        return this;
    }

    public bool Chromeless { get => Config.Chromeless; set => Config.Chromeless = value; }
    public KirinApp IsChromeless(bool b = true)
    {
        Config.Chromeless = b;
        return this;
    }

    public bool Debug { get => Config.Debug; set => Config.Debug = value; }
    public KirinApp IsDebug(bool b = true)
    {
        Config.Debug = b;
        return this;
    }

    public WebAppType AppType { get => Config.AppType; set => Config.AppType = value; }
    public KirinApp SetAppType(WebAppType appType)
    {
        Config.AppType = appType;
        return this;
    }

    public int Left { get => Config.Left; set => Config.Left = value; }
    public int Top { get => Config.Top; set => Config.Top = value; }
    public KirinApp SetPosition(Point p)
    {
        Config.Left = p.X;
        Config.Top = p.Y;
        return this;
    }
    public KirinApp SetPosition(int left, int top)
    {
        Config.Left = left;
        Config.Top = top;
        return this;
    }

    public bool ResizeAble { get => Config.ResizeAble; set => Config.ResizeAble = value; }
    public KirinApp SetResizeAble(bool b = true)
    {
        Config.ResizeAble = b;
        return this;
    }

    public bool Center { get => Config.Center; set => Config.Center = value; }
    public KirinApp SetCenter(bool b = true)
    {
        Config.Center = b;
        return this;
    }

    public string? Url { get => Config.Url; set => Config.Url = value; }
    public KirinApp SetUrl(string url)
    {
        Config.Url = url;
        Reload();
        return this;
    }

    public Type? BlazorComponent { get => Config.BlazorComponent; set => Config.BlazorComponent = value; }
    public string BlazorSelector { get => Config.BlazorSelector; set => Config.BlazorSelector = value; }
    public KirinApp SetBlazor<T>(string blazorSelector = "#app") where T : class
    {
        Config.BlazorComponent = typeof(T);
        Config.BlazorSelector += blazorSelector;
        return this;
    }

    public string? RawString { get => Config.RawString; set => Config.RawString = value; }
    public KirinApp SetRawString(string rawString)
    {
        Config.RawString = rawString;
        Reload();
        return this;
    }

    public int MinimumWidth { get => Config.MinimumWidth; set => Config.MinimumWidth = value; }
    public int MinimumHeigh { get => Config.MinimumHeigh; set => Config.MinimumHeigh = value; }
    public KirinApp SetMinSize(int width, int heigth)
    {
        Config.MinimumWidth = width;
        Config.MinimumHeigh = heigth;
        return this;
    }

    public System.Drawing.Size? MinimumSize { get => Config.MinimumSize; set => Config.MinimumSize = value; }
    public KirinApp SetMinSize(Size size)
    {
        Config.MinimumSize = size;
        return this;
    }

    public int MaximumWidth { get => Config.MaximumWidth; set => Config.MaximumWidth = value; }
    public int MaximumHeigh { get => Config.MaximumHeigh; set => Config.MaximumHeigh = value; }
    public KirinApp SetMaxSize(int width, int heigth)
    {
        Config.MaximumWidth = width;
        Config.MaximumHeigh = heigth;
        return this;
    }

    public System.Drawing.Size? MaximumSize { get => Config.MaximumSize; set => Config.MaximumSize = value; }
    public KirinApp SetMaxSize(Size size)
    {
        Config.MaximumSize = size;
        return this;
    }

    public KirinApp UseRawString(string rawString)
    {
        Config.AppType = WebAppType.RawString;
        Config.RawString = rawString;

        //重新获取依赖注入
        InitPlateform();
        RegistResource();
        ServiceProvide = serviceCollection.BuildServiceProvider();
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
        return this;
    }

    public KirinApp UseBlazor<T>(string selector = "#app") where T : class
    {
        Config.AppType = WebAppType.Blazor;
        if (!selector.Contains("#")) selector = "#" + selector;
        Config.BlazorSelector = selector;
        Config.BlazorComponent = typeof(T);

        //重新获取依赖注入
        InitPlateform();
        RegistResource();
        ServiceProvide = serviceCollection.BuildServiceProvider();
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
    public void SendWebMessage(string msg) => Window.SendWebMessage(msg);
}