using KirinAppCore.Model;
using KirinAppCore.Interface;
using KirinAppCore.Plateform.Windows.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;
using static System.Net.Mime.MediaTypeNames;
using System.IO;

namespace KirinAppCore.Plateform.Windows;

/// <summary>
/// Windows实现类
/// </summary>
internal class MainWIndow : IWindow
{
    #region 事件
    public override event EventHandler<EventArgs>? OnCreate;
    public override event EventHandler<EventArgs>? Created;
    public override event EventHandler<EventArgs>? OnLoad;
    public override event EventHandler<EventArgs>? Loaded;
    #endregion

    #region 窗体方法
    protected override void Create()
    {
        OnCreate?.Invoke(this, new());
        var hIns = Win32Api.GetConsoleWindow();
        WindowProc = WndProc;
        var className = Assembly.GetEntryAssembly()!.GetName().Name + "." + this.GetType().Name;
        var color = Win32Api.CreateSolidBrush((uint)ColorTranslator.ToWin32(ColorTranslator.FromHtml("#FFFFFF")));
        IntPtr ico = IntPtr.Zero;
        if (!string.IsNullOrWhiteSpace(Config.Icon))
        {
            var stream = new FileInfo(Config.Icon).OpenRead();
            if (stream != null)
                ico = new Bitmap(stream).GetHicon();
        }
        var windClass = new WNDCLASS
        {
            lpszClassName = className,
            lpfnWndProc = Marshal.GetFunctionPointerForDelegate(WindowProc),
            cbClsExtra = 0,
            cbWndExtra = 0,
            hbrBackground = color,
            style = 0x0003,
            hInstance = hIns,
            lpszMenuName = null,
            hCursor = Win32Api.LoadCursorW(IntPtr.Zero, (IntPtr)CursorResource.IDC_ARROW),
            hIcon = ico
        };
        if (Win32Api.RegisterClassW(ref windClass) == 0)
        {
            throw new Exception("初始化窗体失败!");
        }

        if (Config.Size != null)
        {
            Config.Width = Config.Size.Value.Width;
            Config.Height = Config.Size.Value.Height;
        }
        if (Config.Center)
        {
            Config.Left = (MainMonitor!.Width - Config.Width) / 2;
            Config.Top = (MainMonitor!.Height - Config.Height) / 2;
        }
        WindowStyle windowStyle;
        if (Config.Chromless)
            windowStyle = WindowStyle.POPUPWINDOW | WindowStyle.CLIPCHILDREN | WindowStyle.CLIPSIBLINGS | WindowStyle.THICKFRAME | WindowStyle.MINIMIZEBOX | WindowStyle.MAXIMIZEBOX;
        else
            windowStyle = WindowStyle.OVERLAPPEDWINDOW | WindowStyle.CLIPCHILDREN | WindowStyle.CLIPSIBLINGS;
        if (!Config.ResizeAble)
        {
            windowStyle &= ~WindowStyle.MAXIMIZEBOX;
            windowStyle &= ~WindowStyle.THICKFRAME;
        }

        var windowExStyle = WindowExStyle.APPWINDOW | WindowExStyle.WINDOWEDGE;
        Handle = Win32Api.CreateWindowExW(windowExStyle, className, Config.AppName, windowStyle, Config.Left,
            Config.Top, Config.Width, Config.Height, IntPtr.Zero, IntPtr.Zero, Win32Api.GetConsoleWindow(), null);
        if (Handle == IntPtr.Zero) throw new Exception("创建窗体失败！");
        Win32Api.SetWindowTextW(Handle, Config.AppName);
        Win32Api.UpdateWindow(Handle);
        Created?.Invoke(this, new());
    }

    protected override IntPtr WndProc(IntPtr hwnd, WindowMessage message, IntPtr wParam, IntPtr lParam)
    {
        switch (message)
        {
            case WindowMessage.PAINT:
                {
                    IntPtr hDC = Win32Api.GetDC(hwnd);
                    Win32Api.GetClientRect(hwnd, out Rect rect);
                    var color = (uint)ColorTranslator.ToWin32(ColorTranslator.FromHtml("#FFFFFF"));
                    IntPtr brush = Win32Api.CreateSolidBrush(color);
                    Win32Api.FillRect(hDC, ref rect, brush);
                    Win32Api.ReleaseDC(hwnd, hDC);
                    break;
                }
            case WindowMessage.DIY_FUN:
                {
                    if (wParam != IntPtr.Zero)
                    {
                        Action action = (Action)Marshal.GetDelegateForFunctionPointer(wParam, typeof(Action));
                        action.Invoke();
                    }
                    return IntPtr.Zero;
                }
        }
        return base.WndProc(hwnd, message, wParam, lParam);
    }

    public override void Show()
    {
        if (Win32Api.ShowWindow(Handle, SW.SHOW)) base.State = WindowState.Normal;
    }

    public override void Hide()
    {
        if (Win32Api.ShowWindow(Handle, SW.HIDE)) base.State = WindowState.Hide;
    }

    public override void Focus()
    {
        Win32Api.SetForegroundWindow(Handle);
    }

    public override void MessageLoop()
    {
        MSG message;
        while (Win32Api.GetMessageW(out message, IntPtr.Zero, 0, 0))
        {
            Win32Api.TranslateMessage(ref message);
            Win32Api.DispatchMessageW(ref message);
        }
    }

    public override Rect GetClientSize()
    {
        Rect rect;
        Win32Api.GetClientRect(Handle, out rect);
        return rect;
    }

    public override void Maximize()
    {
        if (Win32Api.ShowWindow(Handle, SW.MAXIMIZE)) base.State = WindowState.Maximize;
    }

    public override void Minimize()
    {
        if (Win32Api.ShowWindow(Handle, SW.MINIMIZE)) base.State = WindowState.Minimize;
    }

    public override void SizeChange(IntPtr handle, int width, int height)
    {
        Win32Api.UpdateWindow(handle);
        if (CoreWebCon != null) CoreWebCon.Bounds = new Rectangle(0, 0, width, height);
    }

    public override DirectoryInfo OpenDirectory(string initialDir = "")
    {
        throw new NotImplementedException();
    }

    public override FileInfo OpenFile(string initialDir = "", Dictionary<string, string>? fileTypeFilter = null)
    {
        throw new NotImplementedException();
    }

    public override List<FileInfo> OpenFiles(string initialDir = "", Dictionary<string, string>? fileTypeFilter = null)
    {
        throw new NotImplementedException();
    }

    public override MsgResult ShowDialog(string title, string msg, MsbBtns btn = MsbBtns.OK)
    {
        throw new NotImplementedException();
    }

    public override void ShowMsg(string title, string msg)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// 获取屏幕信息
    /// </summary>
    public override void SetScreenInfo()
    {
        int width = Win32Api.GetSystemMetrics(0);
        int height = Win32Api.GetSystemMetrics(1);

        nint hdc = Win32Api.GetDC(0);
        int screenWidth = Win32Api.GetDeviceCaps(hdc, 118);

        double dpi = Math.Round((double)screenWidth / width, 2);
        MainMonitor = new()
        {
            Width = width,
            Height = height,
            Zoom = dpi,
        };
    }
    #endregion

    #region WebView2方法
    public override bool CheckAccess()
    {
        return Environment.CurrentManagedThreadId == ManagedThreadId;
    }

    public override async Task InvokeAsync(Func<Task> workItem)
    {
        if (CheckAccess()) await workItem();
        else
        {
            IntPtr actionPtr = Marshal.GetFunctionPointerForDelegate(workItem);
            Action action = (Action)Marshal.GetDelegateForFunctionPointer(actionPtr, typeof(Action));
            action.Invoke();
        }
    }

    public override async Task InvokeAsync(Action workItem)
    {
        await Task.Delay(1);
        if (CheckAccess()) workItem();
        else
        {
            IntPtr actionPtr = Marshal.GetFunctionPointerForDelegate(workItem);
            Win32Api.PostMessage(Handle, (uint)WindowMessage.DIY_FUN, actionPtr, IntPtr.Zero);
        }
    }

    protected override async Task InitWebControl()
    {
        try
        {
            OnLoad?.Invoke(this, new());
            var userPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    Process.GetCurrentProcess().ProcessName);
            CoreWebEnv = await CoreWebView2Environment.CreateAsync(userDataFolder: userPath);
            CoreWebCon = await CoreWebEnv.CreateCoreWebView2ControllerAsync(Handle);
            CoreWebCon.CoreWebView2.Settings.AreDefaultScriptDialogsEnabled = false;
            CoreWebCon.CoreWebView2.Settings.IsStatusBarEnabled = false;
            CoreWebCon.CoreWebView2.Settings.IsZoomControlEnabled = false;
            CoreWebCon.CoreWebView2.Settings.IsBuiltInErrorPageEnabled = false;
            CoreWebCon.Bounds = new Rectangle(0, 0, Config.Width, Config.Height);

            //禁止新窗口打开
            CoreWebCon.CoreWebView2.NewWindowRequested += (s, e) => e.NewWindow ??= (CoreWebView2)s!;
            //屏蔽快捷键
            CoreWebCon.AcceleratorKeyPressed += (s, e) =>
            {
                if (!Config.Debug && e.VirtualKey >= 112 && e.VirtualKey <= 122) e.Handled = true;
            };

            CoreWebCon.CoreWebView2.Settings.AreDevToolsEnabled = Config.Debug;
            CoreWebCon.CoreWebView2.Settings.AreDefaultContextMenusEnabled = Config.Debug;

            if (Config.AppType != WebAppType.Http)
            {
                var url = "http://localhost/";
                if (Config.AppType == WebAppType.Static) url += Config.Url;
                if (Config.AppType == WebAppType.Blazor) url += "blazorindex.html";

                var schemeConfig = new Uri(url).ParseScheme();
                var dispatcher = new WebDispatcher(this);
                var webViewManager = new WebManager(this, CoreWebCon.CoreWebView2, ServiceProvide!, dispatcher,
                    ServiceProvide!.GetRequiredService<JSComponentConfigurationStore>(), schemeConfig);
                if (Config.AppType == WebAppType.Blazor)
                    _ = dispatcher.InvokeAsync(async () =>
                    {
                        await webViewManager.AddRootComponentAsync(Config.BlazorComponent!, Config.BlazorSelector,
                            ParameterView.Empty);
                    });
                CoreWebCon.CoreWebView2.WebMessageReceived += (s, e) =>
                {
                    webViewManager.OnMessageReceived(e.Source, e.TryGetWebMessageAsString());
                };
                CoreWebCon.CoreWebView2.AddWebResourceRequestedFilter($"{schemeConfig.AppOrigin}*",
                    CoreWebView2WebResourceContext.All);
                CoreWebCon.CoreWebView2.WebResourceRequested += (s, e) =>
                {
                    var response = webViewManager.OnResourceRequested(schemeConfig, e.Request.Uri.ToString());
                    if (response.Content != null)
                    {
                        e.Response = CoreWebEnv.CreateWebResourceResponse(response.Content, 200, "OK",
                            $"Content-Type:{response.Type}");
                    }
                };

                var assembly = Assembly.GetExecutingAssembly();
                var stream = assembly.GetManifestResourceStream("KirinAppCore.wwwroot.edge.document.js")!;
                var content = new StreamReader(stream).ReadToEnd();
                await CoreWebCon.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(content).ConfigureAwait(true);

                webViewManager.Navigate("/");
            }
            else
            {
                CoreWebCon.CoreWebView2.Navigate(Config.Url);
            }
            Loaded?.Invoke(this, new());
        }
        catch (Exception)
        {
            throw;
        }
    }

    public override async Task ExecuteJavaScript(string js)
    {
        if (CheckAccess())
        {
            if (CoreWebCon?.CoreWebView2 == null) throw new Exception("请在加载完成事件中或确保页面已经加载后调用");
            await CoreWebCon.CoreWebView2.ExecuteScriptAsync(js);
        }
        else
        {

            IntPtr actionPtr = Marshal.GetFunctionPointerForDelegate(() =>
            {
                CoreWebCon!.CoreWebView2.ExecuteScriptAsync(js);
            });
            Win32Api.PostMessage(Handle, (uint)WindowMessage.DIY_FUN, actionPtr, IntPtr.Zero);
        }
    }

    public override async Task<string> ExecuteJavaScriptWithResult(string js)
    {
        if (CheckAccess())
        {
            if (CoreWebCon?.CoreWebView2 == null) throw new Exception("请在加载完成事件中或确保页面已经加载后调用");
            return await CoreWebCon.CoreWebView2.ExecuteScriptAsync(js);
        }
        else
        {
            throw new Exception("请勿异步方法下调用或请确保在主线程调用");
        }
    }

    public override void OpenDevTool()
    {
        if (CheckAccess())
        {
            if (CoreWebCon?.CoreWebView2 == null) throw new Exception("请在加载完成事件中或确保页面已经加载后调用");
            CoreWebCon!.CoreWebView2.OpenDevToolsWindow();
        }
        else
        {
            IntPtr actionPtr = Marshal.GetFunctionPointerForDelegate(()=>CoreWebCon!.CoreWebView2.OpenDevToolsWindow());
            Win32Api.PostMessage(Handle, (uint)WindowMessage.DIY_FUN, actionPtr, IntPtr.Zero);
        }
    }

    public override void SendWebMessage(string message)
    {
        CoreWebCon!.CoreWebView2.PostWebMessageAsString(message);
    }
    #endregion
}