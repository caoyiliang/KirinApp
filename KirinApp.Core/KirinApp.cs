using KirinAppCore.Interface;
using KirinAppCore.Model;
using KirinAppCore.Plateform.Linux;
using KirinAppCore.Plateform.Windows;
using KirinAppCore.Platform.Linux;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;
using System.Drawing;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Components.WebView;

namespace KirinAppCore;

public class KirinApp
{
    internal WinConfig Config = new();
    internal IWindow Window { get; private set; }

    protected ServiceProvider ServiceProvide { get; private set; }
    IServiceCollection serviceCollection = new ServiceCollection();

    public event EventHandler<EventArgs>? OnCreate;
    public event EventHandler<EventArgs>? Created;
    public event EventHandler<EventArgs>? Loaded;
    public event NetClosingDelegate? OnClose;

    public delegate bool? NetClosingDelegate(object sender, EventArgs e);

    public event EventHandler<WebMessageEvent>? WebMessageReceived;
    public event EventHandler<SizeChangeEventArgs>? SizeChange;
    public event EventHandler<PositionChangeEventArgs>? PositionChange;

    /// <summary>
    /// 主显示器
    /// </summary>
    public Model.Monitor MainMonitor => Window.MainMonitor;
    public OSPlatform OS { get; private set; } = OSPlatform.Windows;
    public bool IsMainThread => Environment.CurrentManagedThreadId == Utils.MainThreadId;
    public OperatingSystem OsVersion { get; private set; } = Environment.OSVersion;
    public WindowState WindowState => Window.State;
    public KirinApp(KirinApp? parent = null)
    {
        InitPlateform();
        RegistResource();
        ServiceProvide = serviceCollection.BuildServiceProvider();
        Utils.Service = ServiceProvide;
        Window = ServiceProvide.GetRequiredService<IWindow>();
        Window.ParentWindows = parent;

        Window.SetScreenInfo();
    }

    public KirinApp(WinConfig winConfig, KirinApp? parent = null)
    {
        Config = winConfig;
        InitPlateform();
        RegistResource();
        ServiceProvide = serviceCollection.BuildServiceProvider();
        Utils.Service = ServiceProvide;
        Window = ServiceProvide.GetRequiredService<IWindow>();
        Window.ParentWindows = parent;

        Window.SetScreenInfo();
    }

    private void EventRegister()
    {
        Window.OnCreate += (s, e) => OnCreate?.Invoke(s, e);
        Window.Created += (s, e) => Created?.Invoke(s, e);
        Window.OnClose += (s, e) => OnClose?.Invoke(s, e);
        Window.WebMessageReceived += (s, e) => WebMessageReceived?.Invoke(s, e);
        Window.SizeChangeEvent += (s, e) => SizeChange?.Invoke(s, e);
        Window.PositionChangeEvent += (s, e) => PositionChange?.Invoke(s, e);
    }

    public KirinApp Run()
    {
        if (Utils.MainThreadId == 0)
            Utils.MainThreadId = Environment.CurrentManagedThreadId;
        if (Window.CheckAccess() && Parent == null && Utils.Wnds.Count == 0)
        {
            EventRegister();
            Window.Init(ServiceProvide, Config);
            Window.Show();
            Loaded?.Invoke(this, new EventArgs());
            Utils.Wnds.Add(this);
            Window.MainLoop();
        }
        else
        {
            IntPtr actionPtr = Marshal.GetFunctionPointerForDelegate(() =>
            {
                EventRegister();
                Window.Init(ServiceProvide, Config);
                Window.Show();
                Loaded?.Invoke(this, new EventArgs());
                Utils.Wnds.Add(this);
                Window.MainLoop();
            });
            Win32Api.PostMessage(Utils.Wnds[0].Window.Handle, (uint)WindowMessage.DIY_FUN, actionPtr, IntPtr.Zero);
        };
        return this;
    }

    private void InitPlateform()
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            serviceCollection.AddSingleton<IWindow, Plateform.Windows.MainWIndow>();
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            serviceCollection.AddSingleton<IWindow, Plateform.Linux.MainWIndow>();
            //检测注入libwebkit库
            if (Utils.LinuxLibInstall("libwebkit2gtk-4.0"))
                serviceCollection.AddSingleton<IWebKit, WebKit40>();
            else if (Utils.LinuxLibInstall("libwebkit2gtk-4.1"))
                serviceCollection.AddSingleton<IWebKit, WebKit41>();
            else throw new Exception("检测到未安装libwebkit2gtk库");
        }
        //else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        //    serviceCollection.AddSingleton<IWindow, KirinAppCore.Plateform.Webkit.MacOS.MainWIndow>();
        else
        {
            throw new Exception("不支持的操作系统！");
        }

        serviceCollection.AddSingleton<JSComponentConfigurationStore>();
        serviceCollection.AddBlazorWebView();
    }

    private void RegistResource()
    {
        List<IFileProvider> containers = new List<IFileProvider>();
        containers.Add(
            new ManifestEmbeddedFileProvider(typeof(Microsoft.AspNetCore.Components.WebView.WebViewManager).Assembly));
        IFileProvider provider = new CompositeFileProvider(containers);
        serviceCollection.AddSingleton(provider);
    }

    #region 通过方法修改Config的属性

    public KirinApp? Parent
    {
        get => Window.ParentWindows;
        set => Window.ParentWindows = value;
    }

    public string AppName
    {
        get => Config.AppName;
        set => Config.AppName = value;
    }

    public KirinApp SetAppName(string name)
    {
        Config.AppName = name;
        return this;
    }

    public string Icon
    {
        get => Config.Icon;
        set => Config.Icon = value;
    }

    public KirinApp SetIcon(string path)
    {
        Config.Icon = path;
        return this;
    }

    public int Width
    {
        get => Config.Width;
        set => Config.Width = value;
    }

    public int Height
    {
        get => Config.Height;
        set => Config.Height = value;
    }

    public KirinApp SetSize(int width, int height)
    {
        Config.Height = height;
        Config.Width = width;
        return this;
    }

    public System.Drawing.Size? Size
    {
        get => Config.Size;
        set => Config.Size = value;
    }

    public KirinApp SetSize(Size size)
    {
        Config.Size = size;
        return this;
    }

    public bool Chromeless
    {
        get => Config.Chromeless;
        set => Config.Chromeless = value;
    }

    public KirinApp SetChromeless(bool b = true)
    {
        Config.Chromeless = b;
        return this;
    }

    public bool Debug
    {
        get => Config.Debug;
        set => Config.Debug = value;
    }

    public KirinApp SetDebug(bool b = true)
    {
        Config.Debug = b;
        return this;
    }

    public WebAppType AppType
    {
        get => Config.AppType;
        set => Config.AppType = value;
    }

    public KirinApp SetAppType(WebAppType appType)
    {
        Config.AppType = appType;
        return this;
    }

    public int Left
    {
        get => Config.Left;
        set => Config.Left = value;
    }

    public int Top
    {
        get => Config.Top;
        set => Config.Top = value;
    }

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

    public bool ResizeAble
    {
        get => Config.ResizeAble;
        set => Config.ResizeAble = value;
    }

    public KirinApp SetResizeAble(bool b = true)
    {
        Config.ResizeAble = b;
        return this;
    }

    public bool Center
    {
        get => Config.Center;
        set => Config.Center = value;
    }

    public KirinApp SetCenter(bool b = true)
    {
        Config.Center = b;
        return this;
    }

    public string? Url
    {
        get => Config.Url;
        set => Config.Url = value;
    }

    public KirinApp SetUrl(string url)
    {
        Config.Url = url;
        return this;
    }

    public Type? BlazorComponent
    {
        get => Config.BlazorComponent;
        set => Config.BlazorComponent = value;
    }

    public string? RawString
    {
        get => Config.RawString;
        set => Config.RawString = value;
    }

    public int MinimumWidth
    {
        get => Config.MinimumWidth;
        set => Config.MinimumWidth = value;
    }

    public int MinimumHeigh
    {
        get => Config.MinimumHeigh;
        set => Config.MinimumHeigh = value;
    }

    public KirinApp SetMinSize(int width, int heigth)
    {
        Config.MinimumWidth = width;
        Config.MinimumHeigh = heigth;
        return this;
    }

    public System.Drawing.Size? MinimumSize
    {
        get => Config.MinimumSize;
        set => Config.MinimumSize = value;
    }

    public KirinApp SetMinSize(Size size)
    {
        Config.MinimumSize = size;
        return this;
    }

    public int MaximumWidth
    {
        get => Config.MaximumWidth;
        set => Config.MaximumWidth = value;
    }

    public int MaximumHeigh
    {
        get => Config.MaximumHeigh;
        set => Config.MaximumHeigh = value;
    }

    public KirinApp SetMaxSize(int width, int heigth)
    {
        Config.MaximumWidth = width;
        Config.MaximumHeigh = heigth;
        return this;
    }

    public System.Drawing.Size? MaximumSize
    {
        get => Config.MaximumSize;
        set => Config.MaximumSize = value;
    }

    public KirinApp SetMaxSize(Size size)
    {
        Config.MaximumSize = size;
        return this;
    }

    public bool TopMost
    {
        get => Config.TopMost;
        set => Config.TopMost = value;
    }

    public KirinApp SetTopMost(bool b = true)
    {
        Config.TopMost = b;
        Window.TopMost(b);
        return this;
    }

    public KirinApp SetRawString(string rawString)
    {
        Config.AppType = WebAppType.RawString;
        Config.RawString = rawString;
        return this;
    }

    public KirinApp SetStatic(string path)
    {
        Config.AppType = WebAppType.Static;
        Config.Url = path;
        return this;
    }

    public KirinApp SetHttp(string url)
    {
        Config.AppType = WebAppType.Http;
        Config.Url = url;
        return this;
    }

    public KirinApp SetBlazor<T>() where T : class
    {
        Config.AppType = WebAppType.Blazor;
        Config.BlazorComponent = typeof(T);
        return this;
    }

    #endregion

    public void LoadStatic(string path)
    {
        SetStatic(path);
        Reload();
    }

    public void LoadUrl(string url)
    {
        SetHttp(url);
        Reload();
    }

    public void LoadRawString(string content)
    {
        SetRawString(content);
        Reload();
    }

    public void LoadBlazor<T>() where T : class
    {
        SetBlazor<T>();
        Reload();
    }

    public (bool selected, DirectoryInfo? dir) OpenDirectory(string initialDir = "") =>
        Window.OpenDirectory(initialDir);

    public (bool selected, FileInfo? file) OpenFile(string filePath = "",
        Dictionary<string, string>? fileTypeFilter = null) => Window.OpenFile(filePath, fileTypeFilter);

    public (bool selected, List<FileInfo>? files) OpenFiles(string filePath = "",
        Dictionary<string, string>? fileTypeFilter = null) => Window.OpenFiles(filePath, fileTypeFilter);

    public MsgResult ShowDialog(string title, string message, MsgBtns btns = MsgBtns.OK) =>
        Window.ShowDialog(title, message, btns);

    public void ExecuteJavaScript(string js, Action<string>? handlResult = null) =>
        Window.ExecuteJavaScript(js, handlResult);

    public async Task<T> ExecuteJavaScript<T>(string js)
    {
        var tcs = new TaskCompletionSource<T>();
        Window.ExecuteJavaScript(js, (data) =>
        {
            try
            {
                var res = JsonConvert.DeserializeObject<T>(data);
                if (res == null) throw new JsonSerializationException();
                tcs.SetResult(res);
            }
            catch (Exception ex)
            {
                tcs.SetException(ex);
            }
        });
        return await tcs.Task;
    }

    public void InjectJsObject(string name, object obj) => Window.InjectJsObject(name, obj);
    public void OpenDevTool() => Window.OpenDevTool();
    public void Reload() => Window.Reload();
    public void SendWebMessage(string msg) => Window.SendWebMessage(msg);
    public void Hide() => Window.Hide();
    public void Show() => Window.Show();
    public void Exit() => Window.Close();
    public void Change(int width, int height) => Window.Change(width, height);
    public void Change(Size size) => Window.Change(size);
    public void Focus() => Window.Focus();
    public void MoveTo(int x, int y) => Window.MoveTo(x, y);
    public void Minimize(bool minimize = true) => Window.Minimize(minimize);
    public void Maximize(bool maximize = true) => Window.Maximize(maximize);
    public void Normal() => Window.Normal();
    public void Invoke(Action action) => Window.Invoke(action);
    public async Task InvokeAsync(Func<Task> action) => await Window.InvokeAsync(action);
}