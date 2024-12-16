using KirinAppCore.Model;
using KirinAppCore.Interface;
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
using System.Text.RegularExpressions;
using System.Reflection.Metadata;
using Microsoft.AspNetCore.Components.WebView;
using Newtonsoft.Json.Linq;
using KirinAppCore.Plateform.WebView2.Windows.Models;

namespace KirinAppCore.Plateform.WebView2.Windows;

/// <summary>
/// Windows实现类
/// </summary>
internal class MainWIndow : IWindow
{
    private CoreWebView2Environment? CoreWebEnv;
    private CoreWebView2Controller? CoreWebCon;
    private WebManager? WebManager { get; set; }
    private SchemeConfig? SchemeConfig { get; set; }
    private WndProcDelegate? WindowProc;
    #region 事件
    public override event EventHandler<CoreWebView2WebMessageReceivedEventArgs>? WebMessageReceived;
    public override event EventHandler<EventArgs>? OnCreate;
    public override event EventHandler<EventArgs>? Created;
    public override event EventHandler<EventArgs>? OnLoad;
    public override event EventHandler<EventArgs>? Loaded;
    public override event EventHandler<SizeChangeEventArgs>? SizeChangeEvent;
    public override event EventHandler<PositionChangeEventArgs>? PositionChangeEvent;
    public override event CloseDelegate? OnClose;
    #endregion

    #region 窗体方法
    protected override void Create()
    {
        OnCreate?.Invoke(this, new());
        var hIns = Win32Api.GetConsoleWindow();
        WindowProc = WndProc;
        var className = "KirinApp-" + Guid.NewGuid();
        var color = Win32Api.CreateSolidBrush((uint)ColorTranslator.ToWin32(ColorTranslator.FromHtml("#FFFFFF")));
        IntPtr ico = IntPtr.Zero;
        if (!string.IsNullOrWhiteSpace(Config.Icon))
        {
            var icon = new Icon(Config.Icon);
            if (icon != null)
                ico = icon.Handle;
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
            int errorCode = Marshal.GetLastWin32Error();
            throw new Exception("初始化窗体失败，错误代码：" + errorCode);
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
        if (Config.Chromeless)
            windowStyle = WindowStyle.POPUPWINDOW | WindowStyle.CLIPCHILDREN | WindowStyle.CLIPSIBLINGS | WindowStyle.THICKFRAME | WindowStyle.MINIMIZEBOX | WindowStyle.MAXIMIZEBOX;
        else
            windowStyle = WindowStyle.OVERLAPPEDWINDOW | WindowStyle.CLIPCHILDREN | WindowStyle.CLIPSIBLINGS;
        if (!Config.ResizeAble || Config.MaximumSize != null || Config.MaximumWidth > 0 || Config.MinimumHeigh > 0)
        {
            windowStyle &= ~WindowStyle.MAXIMIZEBOX;
            windowStyle &= ~WindowStyle.THICKFRAME;
        }

        var windowExStyle = WindowExStyle.APPWINDOW | WindowExStyle.WINDOWEDGE;
        Win32Api.SetProcessDPIAware();
        Handle = Win32Api.CreateWindowExW(windowExStyle, className, Config.AppName, windowStyle, Config.Left,
            Config.Top, Config.Width, Config.Height, IntPtr.Zero, IntPtr.Zero, Win32Api.GetConsoleWindow(), null);
        if (Handle == IntPtr.Zero) throw new Exception("创建窗体失败！");
        Win32Api.SetWindowTextW(Handle, Config.AppName);
        Win32Api.UpdateWindow(Handle);
        Created?.Invoke(this, new());
    }

    protected IntPtr WndProc(IntPtr hwnd, WindowMessage message, IntPtr wParam, IntPtr lParam)
    {
        switch (message)
        {
            case WindowMessage.GETMINMAXINFO:
                MinMaxInfo mmi = Marshal.PtrToStructure<MinMaxInfo>(lParam);
                if (Config.MinimumSize != null)
                {
                    Config.MinimumWidth = Config.MinimumSize.Value.Width;
                    Config.MinimumHeigh = Config.MinimumSize.Value.Height;

                }
                if (Config.MaximumSize != null)
                {
                    Config.MaximumWidth = Config.MaximumSize.Value.Width;
                    Config.MaximumHeigh = Config.MaximumSize.Value.Height;
                }
                mmi.ptMinTrackSize.X = Config.MinimumWidth; // 设置最小宽度
                mmi.ptMinTrackSize.Y = Config.MinimumHeigh; // 设置最小高度
                mmi.ptMaxTrackSize.X = Config.MaximumWidth; // 设置最大宽度
                mmi.ptMaxTrackSize.Y = Config.MaximumHeigh; // 设置最大高度
                Marshal.StructureToPtr(mmi, lParam, true);
                break;
            case WindowMessage.SIZE:
                {
                    var size = GetClientSize();
                    SizeChangeEvent?.Invoke(this, new SizeChangeEventArgs() { Width = size.Width, Height = size.Height });
                    SizeChange(Handle, size.Width, size.Height);
                    break;
                }
            case WindowMessage.MOVE:
                {
                    var move = GetClientSize();
                    PositionChangeEvent?.Invoke(this, new PositionChangeEventArgs() { X = move.Left, Y = move.Top });
                    break;
                }
            case WindowMessage.CLOSE:
                {
                    var res = OnClose?.Invoke(this, new());
                    if (res == null || res.Value)
                    {
                        Close();
                        return IntPtr.Zero;
                    }
                    else return IntPtr.Zero;
                }
            case WindowMessage.PAINT:
                {
                    Win32Api.SetProcessDPIAware();
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
        return Win32Api.DefWindowProcW(hwnd, message, wParam, lParam);
    }

    public override void Show()
    {
        if (Win32Api.ShowWindow(Handle, SW.SHOW)) State = WindowState.Normal;
    }

    public override void Hide()
    {
        if (Win32Api.ShowWindow(Handle, SW.HIDE)) State = WindowState.Hide;
    }

    public override void Focus()
    {
        Win32Api.SetForegroundWindow(Handle);
    }

    public override void Close()
    {
        if (ParentWindows == null && Utils.Wnds.Count <= 1) base.Close();
        var app = Utils.Wnds.FirstOrDefault(x => x.Window.Handle == Handle);
        if (app != null) Utils.Wnds.Remove(app);
        Win32Api.DestroyWindow(Handle);
    }

    public override void MainLoop()
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
        if (Win32Api.ShowWindow(Handle, SW.MAXIMIZE)) State = WindowState.Maximize;
    }

    public override void Minimize()
    {
        if (Win32Api.ShowWindow(Handle, SW.MINIMIZE)) State = WindowState.Minimize;
    }

    public override void SizeChange(IntPtr handle, int width, int height)
    {
        Win32Api.UpdateWindow(handle);
        if (CoreWebCon != null) CoreWebCon.Bounds = new Rectangle(0, 0, width, height);
    }

    private void CheckInitialDir(ref string initialDir)
    {
        if (string.IsNullOrWhiteSpace(initialDir))
            initialDir = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
    }

    private void CheckFileFilter(Dictionary<string, string>? dic)
    {
        if (dic == null || dic.Count == 0)
            dic = new Dictionary<string, string>() { { "所有文件（*.*)", "*.*" } };
    }

    public override (bool selected, DirectoryInfo? dir) OpenDirectory(string initialDir = "")
    {
        CheckInitialDir(ref initialDir);
        IntPtr pidl = IntPtr.Zero;
        var @params = new BrowseInfo()
        {
            hwndOwner = IntPtr.Zero,
            pidlRoot = IntPtr.Zero,
            pszDisplayName = IntPtr.Zero,
            lpszTitle = "选择目录",
            ulFlags = 0,
            lpfn = IntPtr.Zero,
            lParam = IntPtr.Zero,
            iImage = 0
        };
        try
        {
            pidl = Win32Api.SHBrowseForFolder(ref @params);
            if (pidl != IntPtr.Zero)
            {
                IntPtr pszPath = Marshal.AllocHGlobal(2048);
                if (Win32Api.SHGetPathFromIDList(pidl, pszPath))
                {
                    Marshal.FreeHGlobal(pszPath);
                    var path = Marshal.PtrToStringAuto(pszPath);
                    if (string.IsNullOrWhiteSpace(path)) throw new DirectoryNotFoundException();
                    if (!Directory.Exists(path)) throw new DirectoryNotFoundException();
                    return (true, new DirectoryInfo(path));
                }
                return (false, null);
            }
            return (false, null);
        }
        finally
        {
            if (pidl != IntPtr.Zero)
            {
                Marshal.FreeCoTaskMem(pidl);
            }
        }
    }

    public override (bool selected, FileInfo? file) OpenFile(string initialDir = "", Dictionary<string, string>? fileTypeFilter = null)
    {
        CheckInitialDir(ref initialDir);
        CheckFileFilter(fileTypeFilter);
        var bufferLength = 1024;
        OpenFileDialogParams @params = new OpenFileDialogParams()
        {
            ownerHandle = IntPtr.Zero,
            instanceHandle = IntPtr.Zero,
            filter = string.Join("", fileTypeFilter?.Select(s => $"{s.Key}\0{s.Value}\0") ?? new List<string>()),
            initialDir = initialDir,
            file = Marshal.StringToBSTR(new string(new char[bufferLength])),
            maxFile = bufferLength,
            fileTitle = new string(new char[bufferLength]),
            title = "打开文件",
            flags = 0x00000004 | 0x00080000 | 0x00001000 | 0x00000800 | 0x00000008 | 0x00000200
        };
        @params.structSize = Marshal.SizeOf(@params);
        if (Win32Api.GetOpenFileName(ref @params))
        {
            string file = Marshal.PtrToStringAuto(@params.file) ?? "";
            if (string.IsNullOrWhiteSpace(file)) throw new FileNotFoundException();
            if (!File.Exists(file)) throw new FileNotFoundException();
            return (true, new FileInfo(file));
        }
        return (false, null);
    }

    public override (bool selected, List<FileInfo>? files) OpenFiles(string initialDir = "", Dictionary<string, string>? fileTypeFilter = null)
    {
        CheckInitialDir(ref initialDir);
        CheckFileFilter(fileTypeFilter);
        var bufferLength = 1024;
        OpenFileDialogParams @params = new OpenFileDialogParams()
        {
            ownerHandle = IntPtr.Zero,
            instanceHandle = IntPtr.Zero,
            filter = string.Join("", fileTypeFilter?.Select(s => $"{s.Key}\0{s.Value}\0") ?? new List<string>()),
            initialDir = initialDir,
            file = Marshal.StringToBSTR(new string(new char[bufferLength])),
            maxFile = bufferLength,
            fileTitle = new string(new char[bufferLength]),
            title = "打开文件",
            flags = 0x00000004 | 0x00080000 | 0x00001000 | 0x00000800 | 0x00000008 | 0x00000200
        };
        @params.maxFileTitle = @params.fileTitle.Length;
        @params.structSize = Marshal.SizeOf(@params);

        if (Win32Api.GetOpenFileName(ref @params))
        {
            List<FileInfo> files = new List<FileInfo>();

            long pointer = (long)@params.file;
            string file = Marshal.PtrToStringAuto(@params.file) ?? "";

            var path = "";
            var index = 0;

            while (file?.Length > 0)
            {
                if (index == 0)
                {
                    path = file;
                }
                else
                {
                    files.Add(new FileInfo(Path.Combine(path, file)));
                }

                pointer += file.Length * 2 + 2;
                @params.file = (IntPtr)pointer;
                file = Marshal.PtrToStringAuto(@params.file) ?? "";
                index++;
            }
            return (true, files);
        }
        return (false, null);
    }

    public override MsgResult ShowDialog(string title, string msg, MsgBtns btn = MsgBtns.OK)
    {
        var handle = Win32Api.GetConsoleWindow();
        return Utils.ToMsgResult(Win32Api.MessageBox(handle, msg, title, (int)btn | 64));
    }

    /// <summary>
    /// 获取屏幕信息
    /// </summary>
    public override void SetScreenInfo()
    {
        //主显示器
        int width = Win32Api.GetSystemMetrics(0);
        int height = Win32Api.GetSystemMetrics(1);

        MainMonitor = new()
        {
            Width = width,
            Height = height,
        };
    }
    #endregion

    #region WebView2方法
    public override bool CheckAccess()
    {
        return Environment.CurrentManagedThreadId == Utils.MainThreadId;
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

    public override void Invoke(Action workItem)
    {
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
                if (!Config.Debug)
                    if (e.VirtualKey >= 112 && e.VirtualKey <= 122) // F1 - F12
                        e.Handled = true;
            };

            CoreWebCon.CoreWebView2.Settings.AreDevToolsEnabled = Config.Debug;
            CoreWebCon.CoreWebView2.Settings.AreDefaultContextMenusEnabled = Config.Debug;

            if (Config.AppType != WebAppType.Http)
            {
                var url = $"http://localhost/";
                if (Config.AppType == WebAppType.Static) url += Config.Url;
                if (Config.AppType == WebAppType.Blazor) url += "blazorindex.html";

                SchemeConfig = new Uri(url).ParseScheme();
                var dispatcher = new WebDispatcher(this);
                WebManager = new WebManager(this, CoreWebCon.CoreWebView2, ServiceProvide!, dispatcher,
                    ServiceProvide!.GetRequiredService<JSComponentConfigurationStore>(), SchemeConfig);
                if (Config.AppType == WebAppType.Blazor)
                {
                    if (Config.BlazorComponent == null) throw new Exception("Blazor component not found!");
                    _ = dispatcher.InvokeAsync(async () =>
                    {
                        await WebManager.AddRootComponentAsync(Config.BlazorComponent!, Config.BlazorSelector,
                            ParameterView.Empty);
                    });
                }
                CoreWebCon.CoreWebView2.WebMessageReceived += (s, e) =>
                {
                    WebManager.OnMessageReceived(e.Source, e.TryGetWebMessageAsString());
                    try
                    {
                        var jobject = JObject.Parse(e.TryGetWebMessageAsString());
                        if (jobject.ContainsKey("cmd"))
                        {
                            var cmd = jobject["cmd"]!.ToString();
                            switch (cmd)
                            {
                                case "max": Maximize(); break;
                                case "min": Minimize(); break;
                                case "hide": Hide(); break;
                                case "show": Show(); break;
                                case "focus": Focus(); break;
                                case "close": Close(); break;
                                default:
                                    break;
                            }
                            Maximize();
                        }
                        return;
                    }
                    catch { }
                    WebMessageReceived?.Invoke(s, e);
                };
                CoreWebCon.CoreWebView2.AddWebResourceRequestedFilter($"{SchemeConfig.AppOrigin}*",
                    CoreWebView2WebResourceContext.All);

                ResourceRequest();

                var assembly = Assembly.GetExecutingAssembly();
                var stream = assembly.GetManifestResourceStream("KirinAppCore.wwwroot.edge.document.js")!;
                var content = new StreamReader(stream).ReadToEnd();
                await CoreWebCon.CoreWebView2.AddScriptToExecuteOnDocumentCreatedAsync(content).ConfigureAwait(true);

                WebManager.Navigate("/");
            }
            if (Config.AppType == WebAppType.Http)
            {
                CoreWebCon.CoreWebView2.Navigate(Config.Url);
            }
            Loaded?.Invoke(this, new());
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }

    public override void ExecuteJavaScript(string js)
    {
        Task.Run(() =>
        {
            while (CoreWebCon == null)
                Thread.Sleep(10);
            Thread.Sleep(10);
            IntPtr actionPtr = Marshal.GetFunctionPointerForDelegate(() =>
            {
                _ = CoreWebCon.CoreWebView2.ExecuteScriptAsync(js).Result;
            });
            Win32Api.PostMessage(Handle, (uint)WindowMessage.DIY_FUN, actionPtr, IntPtr.Zero);
        });
    }

    public override string ExecuteJavaScriptWithResult(string js)
    {
        var tcs = new TaskCompletionSource<string>();
        Task.Run(() =>
        {
            while (CoreWebCon == null)
                Task.Delay(10);
            Task.Delay(10);
            // 创建指向结果的委托
            IntPtr actionPtr = Marshal.GetFunctionPointerForDelegate(new Action(async () =>
            {
                string res = await CoreWebCon.CoreWebView2.ExecuteScriptAsync(js);
                tcs.SetResult(res); // 设置结果
            }));

            // 发送消息
            Win32Api.PostMessage(Handle, (uint)WindowMessage.DIY_FUN, actionPtr, IntPtr.Zero);
        });
        // 等待结果
        return tcs.Task.Result; // 返回结果
    }

    public override void OpenDevTool()
    {
        Task.Run(() =>
        {
            while (CoreWebCon == null)
                Thread.Sleep(10);
            Thread.Sleep(10);
            IntPtr actionPtr = Marshal.GetFunctionPointerForDelegate(() => CoreWebCon!.CoreWebView2.OpenDevToolsWindow());
            Win32Api.PostMessage(Handle, (uint)WindowMessage.DIY_FUN, actionPtr, IntPtr.Zero);
        });
    }

    public override void SendWebMessage(string message)
    {
        CoreWebCon!.CoreWebView2.PostWebMessageAsString(message);
    }

    public override void Reload()
    {
        Task.Run(() =>
        {
            IntPtr actionPtr = Marshal.GetFunctionPointerForDelegate(() =>
            {
                ResourceRequest();
                CoreWebCon?.CoreWebView2.Reload();
            });
            if (Config.AppType == WebAppType.Http)
                actionPtr = Marshal.GetFunctionPointerForDelegate(() => CoreWebCon!.CoreWebView2.Navigate(Config.Url));
            Win32Api.PostMessage(Handle, (uint)WindowMessage.DIY_FUN, actionPtr, IntPtr.Zero);
        });
    }

    private void ResourceRequest()
    {
        CoreWebCon!.CoreWebView2.WebResourceRequested += (s, e) =>
        {
            if (Config.AppType == WebAppType.RawString)
            {
                var contentType = "text/html";
                string pattern = @"<(\w+)([^>]*?)>(.*?)<\/\1>|<(\w+)([^>]*?)/>";
                if (!Regex.IsMatch(Config.RawString ?? "", pattern, RegexOptions.Singleline))
                    contentType = "text/plain";
                byte[] byteArray = Encoding.UTF8.GetBytes(Config.RawString ?? "");
                MemoryStream ms = new(byteArray);
                e.Response = CoreWebEnv!.CreateWebResourceResponse(ms, 200, "OK", $"Content-Type:{contentType}; charset=utf-8");
            }
            else
            {
                var response = WebManager!.OnResourceRequested(SchemeConfig!, e.Request.Uri.ToString());
                if (response.Content != null)
                    e.Response = CoreWebEnv!.CreateWebResourceResponse(response.Content, 200, "OK",
                        $"Content-Type:{response.Type}; charset=utf-8");
            }
        };
    }
    #endregion
}