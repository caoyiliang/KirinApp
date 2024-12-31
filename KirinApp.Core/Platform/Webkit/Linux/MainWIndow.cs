using KirinAppCore.Model;
using KirinAppCore.Interface;
using Microsoft.Extensions.DependencyInjection;
using System.Drawing;
using System.Runtime.InteropServices;
using KirinAppCore.Plateform.Webkit.Linux.Models;
using KirinAppCore.Platform.Webkit.Linux;
using Newtonsoft.Json;
using KirinAppCore.Plateform.WebView2.Windows;

namespace KirinAppCore.Plateform.Webkit.Linux;

/// <summary>
/// Windows实现类
/// </summary>
internal class MainWIndow : IWindow
{
    private WebManager? WebManager { get; set; }
    private SchemeConfig? SchemeConfig { get; set; }

    #region 事件
    public override event EventHandler<WebMessageEvent>? WebMessageReceived;
    public override event EventHandler<SizeChangeEventArgs>? SizeChangeEvent;
    public override event EventHandler<PositionChangeEventArgs>? PositionChangeEvent;
    public override event CloseDelegate? OnClose;
    #endregion

    #region 窗体方法
    protected override void Create()
    {
        try
        {
            Handle = GtkApi.gtk_window_new(Config.Chromeless ? 1 : 0);        
            TopMost(Config.TopMost);
            GtkApi.gtk_window_set_title(Handle, Config.AppName);
            GtkApi.gtk_window_set_resizable(Handle, Config.ResizeAble);

            if (!string.IsNullOrWhiteSpace(Config.Icon))
            {
                using (var fs = new FileStream(Config.Icon, FileMode.Open))
                using (var ms = new MemoryStream())
                {
                    fs.CopyTo(ms);
                    byte[] byteArray = ms.ToArray();
                    var imagePtr = Marshal.UnsafeAddrOfPinnedArrayElement(byteArray, 0);
                    var stream = GtkApi.g_memory_input_stream_new_from_data(imagePtr, byteArray.Length, IntPtr.Zero);

                    var pixbuf = GtkApi.gdk_pixbuf_new_from_stream(stream, IntPtr.Zero, IntPtr.Zero);
                    GtkApi.g_input_stream_close(stream, IntPtr.Zero, IntPtr.Zero);

                    var sam = GtkApi.gdk_pixbuf_scale_simple(pixbuf, 64, 64, GdkInterpType.GDK_INTERP_BILINEAR);
                    GtkApi.gtk_window_set_icon(Handle, sam);

                    // 释放 pixbuf
                    GtkApi.g_object_unref(pixbuf);
                }
            }
            GtkApi.gtk_window_set_decorated(Handle, !Config.Chromeless);

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
            GtkApi.gtk_window_set_default_size(Handle, Config.Width, Config.Height);

            var color = ColorTranslator.FromHtml("#FFFFFF");
            var rgb = new GdkRGBA()
            {
                Red = color.R,
                Green = color.G,
                Blue = color.B,
                Alpha = color.A
            };
            IntPtr result = Marshal.AllocHGlobal(Marshal.SizeOf(rgb));
            Marshal.StructureToPtr(rgb, result, false);
            GtkApi.gtk_widget_override_background_color(Handle, 0, result);
            if (Config.Center) GtkApi.gtk_window_set_position(Handle, 1);
            else GtkApi.gtk_window_move(Handle, Config.Left, Config.Top); 
            if (Config.MinimumSize != null)
            {
                Config.MinimumHeigh = Config.MinimumSize.Value.Height;
                Config.MinimumWidth = Config.MinimumSize.Value.Width;
            }
            if (Config.MaximumSize != null)
            {
                Config.MaximumHeigh = Config.MaximumSize.Value.Height;
                Config.MaximumWidth = Config.MaximumSize.Value.Width;
            }
            var geometry = new GeometryInfo()
            {
                min_width = Config.MinimumWidth,
                min_height = Config.MinimumHeigh,
                max_width = Config.MaximumWidth,
                max_height = Config.MaximumHeigh
            };
            GtkApi.gtk_window_set_geometry_hints(Handle, IntPtr.Zero, ref geometry, GdkWindowHints.GDK_HINT_MIN_SIZE | GdkWindowHints.GDK_HINT_MAX_SIZE);

            GtkApi.gtk_widget_add_events(Handle, 0x2000);
            _onWindowConfigureDelegate = new(OnWindowConfigure);
            GtkApi.g_signal_connect_data(Handle, "configure-event", Marshal.GetFunctionPointerForDelegate(_onWindowConfigureDelegate), IntPtr.Zero, IntPtr.Zero, 0);
            _onWindowCloseDelegate = new(OnWindowClose);
            GtkApi.g_signal_connect_data(Handle, "delete-event", Marshal.GetFunctionPointerForDelegate(_onWindowCloseDelegate), IntPtr.Zero, IntPtr.Zero, 0);
        }
        catch (Exception e)
        {
            throw new Exception("初始化窗体失败！原因：" + e.Message);
        }
    }

    protected delegate IntPtr GtkDeleteEventDelegate(IntPtr widget, IntPtr ev, IntPtr data);
    private GtkDeleteEventDelegate _onWindowCloseDelegate = new((_, _, _) => IntPtr.Zero);
    private IntPtr OnWindowClose(IntPtr widget, IntPtr ev, IntPtr data)
    {
        var res = OnClose?.Invoke(this, EventArgs.Empty);
        if (res == null || res.Value)
        {
            GtkApi.gtk_widget_destroy(Handle);
            GtkApi.gtk_main_quit();
            Close();
            return (IntPtr)0;
        }
        else
        {
            return (IntPtr)1;
        }
    }

    protected delegate bool GtkWidgetEventDelegate(IntPtr widget, IntPtr ev, IntPtr data);
    private GtkWidgetEventDelegate _onWindowConfigureDelegate = new((_, _, _) => true);
    private int lastWidth;
    private int lastHeight;
    private int lastX;
    private int lastY;
    private bool OnWindowConfigure(IntPtr widget, IntPtr eventPtr, IntPtr data)
    {
        // 将事件指针转换为 GdkEventConfigure 结构
        GdkEventConfigure configureEvent = Marshal.PtrToStructure<GdkEventConfigure>(eventPtr);

        // 判断是否发生了大小变化
        if (configureEvent.width != lastWidth || configureEvent.height != lastHeight)
        {
            lastWidth = configureEvent.width;
            lastHeight = configureEvent.height;

            SizeChangeEvent?.Invoke(widget, new SizeChangeEventArgs() { Width = lastWidth, Height = lastHeight });
            SizeChange(Handle, lastWidth, lastHeight);
        }
        else if (configureEvent.x != lastX || configureEvent.y != lastY)
        {
            lastY = configureEvent.y;
            lastX = configureEvent.x;
            PositionChangeEvent?.Invoke(widget, new PositionChangeEventArgs() { X = lastX, Y = lastY });
        }
        else
        {
            Console.WriteLine("其他事件");
        }
        return false;
    }
    public override void Show()
    {
        GtkApi.gtk_widget_show_all(Handle);
        State = WindowState.Normal;
    }

    public override void Hide()
    {
        GtkApi.gtk_widget_hide_all(Handle);
        State = WindowState.Hide;
    }

    public override void Focus() => GtkApi.gtk_window_present(Handle);

    public override void MoveTo(int x, int y)
    {
        IntPtr window = GtkApi.gtk_widget_get_window(Handle);
        GtkApi.gdk_window_get_origin(window, out int windowX, out int windowY);
        GtkApi.gtk_window_move(Handle, windowX + x, windowY + y);
    }

    public override void Move(int x, int y)
    {
        GtkApi.gtk_window_move(Handle, x, y);
    }

    public override void Change(int width, int height)
    {
        GtkApi.gtk_window_resize(Handle, width, height);
    }

    public override void TopMost(bool top)
    {
        GtkApi.gtk_window_set_keep_above(Handle, true);
    }
    public override void Normal()
    {
        Move(Config.Left, Config.Top);
        Change(Config.Width, Config.Height);
        Show();
    }

    public override void MainLoop() => GtkApi.gtk_main();

    public override Rect GetClientSize()
    {
        Rect rect = new();
        GtkApi.gtk_window_get_position(Handle, out rect.Left, out rect.Top);
        int width, height;
        GtkApi.gtk_window_get_size(Handle, out width, out height);
        rect.Width = width;
        rect.Height = height;
        rect.Bottom = MainMonitor.Height - rect.Top - height;
        rect.Right = MainMonitor.Width - rect.Left - width;
        return rect;
    }

    public override void Maximize()
    {
        GtkApi.gtk_window_maximize(Handle);
        State = WindowState.Maximize;
    }

    public override void Minimize()
    {
        GtkApi.gtk_window_iconify(Handle);
        State = WindowState.Minimize;
    }

    private void CheckInitialDir(ref string initialDir)
    {
        if (string.IsNullOrWhiteSpace(initialDir))
            initialDir = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
    }

    private void CheckFileFilter(Dictionary<string, string>? dic)
    {
        if (dic == null || dic.Count == 0)
            dic = new Dictionary<string, string>() { { "所有文件(*.*)", "*.*" } };
    }

    public override (bool selected, DirectoryInfo? dir) OpenDirectory(string initialDir = "")
    {
        CheckInitialDir(ref initialDir);
        IntPtr fileChooser = GtkApi.gtk_file_chooser_dialog_new("选择目录", IntPtr.Zero, FileChooserAction.SelectFolder,
           "选择", ResponseType.Accept,
           "取消", ResponseType.Cancel,
           "", ResponseType.None, IntPtr.Zero);

        GtkApi.gtk_file_chooser_set_current_folder(fileChooser, initialDir);
        GtkApi.gtk_file_chooser_set_do_overwrite_confirmation(fileChooser, true);

        if (GtkApi.gtk_dialog_run(fileChooser) == (int)ResponseType.Accept)
        {
            var selectedPath = Marshal.PtrToStringAuto(GtkApi.gtk_file_chooser_get_current_folder(fileChooser));
            GtkApi.gtk_widget_destroy(fileChooser);
            return (true, new DirectoryInfo(selectedPath!));
        }
        GtkApi.gtk_widget_destroy(fileChooser);
        return (false, null);
    }

    public override (bool selected, FileInfo? file) OpenFile(string initialDir = "", Dictionary<string, string>? fileTypeFilter = null)
    {
        CheckInitialDir(ref initialDir);
        CheckFileFilter(fileTypeFilter);
        IntPtr fileChooser = GtkApi.gtk_file_chooser_dialog_new("选择文件", IntPtr.Zero, FileChooserAction.Open,
                "打开", ResponseType.Accept,
                "取消", ResponseType.Cancel,
                "", ResponseType.None, IntPtr.Zero);
        GtkApi.gtk_file_chooser_set_current_folder(fileChooser, initialDir);

        foreach (var filter in fileTypeFilter!)
        {
            var item = GtkApi.gtk_file_filter_new();
            GtkApi.gtk_file_filter_add_pattern(item, Marshal.StringToHGlobalAnsi(filter.Value));
            GtkApi.gtk_file_filter_set_name(item, Marshal.StringToHGlobalAnsi(filter.Key));
            GtkApi.gtk_file_chooser_add_filter(fileChooser, item);
        }
        if (GtkApi.gtk_dialog_run(fileChooser) == (int)ResponseType.Accept)
        {
            var selectedFilePath = Marshal.PtrToStringAuto(GtkApi.gtk_file_chooser_get_filename(fileChooser));
            GtkApi.gtk_widget_destroy(fileChooser);
            return (true, new FileInfo(selectedFilePath!));
        }
        GtkApi.gtk_widget_destroy(fileChooser);
        return (false, null);
    }

    public override (bool selected, List<FileInfo>? files) OpenFiles(string initialDir = "", Dictionary<string, string>? fileTypeFilter = null)
    {
        CheckInitialDir(ref initialDir);
        CheckFileFilter(fileTypeFilter);
        IntPtr fileChooser = GtkApi.gtk_file_chooser_dialog_new("选择文件", IntPtr.Zero, FileChooserAction.Open,
                "打开", ResponseType.Accept,
                "取消", ResponseType.Cancel,
                "", ResponseType.None, IntPtr.Zero);
        GtkApi.gtk_file_chooser_set_select_multiple(fileChooser, true);
        GtkApi.gtk_file_chooser_set_current_folder(fileChooser, initialDir);
        GtkApi.gtk_file_chooser_set_do_overwrite_confirmation(fileChooser, true);

        foreach (var filter in fileTypeFilter!)
        {
            var item = GtkApi.gtk_file_filter_new();
            GtkApi.gtk_file_filter_add_pattern(item, Marshal.StringToHGlobalAnsi(filter.Value));
            GtkApi.gtk_file_filter_set_name(item, Marshal.StringToHGlobalAnsi(filter.Key));
            GtkApi.gtk_file_chooser_add_filter(fileChooser, item);
        }
        if (GtkApi.gtk_dialog_run(fileChooser) == (int)ResponseType.Accept)
        {
            List<FileInfo> selectedFiles = new();
            var selectIntPrt = GtkApi.gtk_file_chooser_get_filenames(fileChooser);
            int i = 0;
            while (true)
            {
                IntPtr filePathPtr = Marshal.ReadIntPtr(selectIntPrt, i * IntPtr.Size);
                if (filePathPtr == IntPtr.Zero)
                    break;
                string? filePath = Marshal.PtrToStringAnsi(filePathPtr);
                if (string.IsNullOrWhiteSpace(filePath)) continue;
                selectedFiles.Add(new(filePath));
                i++;
            }
            GtkApi.gtk_widget_destroy(fileChooser);
            if (selectedFiles.Count == 0)
                return (false, null);
            return (true, selectedFiles);
        }
        GtkApi.gtk_widget_destroy(fileChooser);
        return (false, null);
    }

    public override MsgResult ShowDialog(string title, string msg, MsgBtns btn = MsgBtns.OK, MessageType messageType = MessageType.Info)
    {
        IntPtr dialog = GtkApi.gtk_message_dialog_new(Handle, 0, (int)messageType, (int)btn, msg);
        GtkApi.gtk_window_set_title(dialog, title);
        int response = GtkApi.gtk_dialog_run(dialog);
        GtkApi.gtk_widget_destroy(dialog);
        return Utils.ToMsgResult(response);
    }

    /// <summary>
    /// 获取屏幕信息
    /// </summary>
    public override void SetScreenInfo()
    {
        GtkApi.XInitThreads();
        IntPtr argv = IntPtr.Zero;
        int argc = 0;
        GtkApi.gtk_init(ref argc, ref argv);
        IntPtr display = GtkApi.gdk_display_get_default();
        if (display == IntPtr.Zero) return;
        IntPtr monitor = GtkApi.gdk_display_get_monitor(display, 0);
        GdkRectangle rect;
        GtkApi.gdk_monitor_get_geometry(monitor, out rect);
        MainMonitor = new()
        {
            Height = rect.Height,
            Width = rect.Width,
        };
    }
    #endregion

    #region Webkit方法
    public override bool CheckAccess()
    {
        return Environment.CurrentManagedThreadId == Utils.MainThreadId;
    }

    private delegate bool GdkIdleFunc(IntPtr data);
    private class InvokeWaitInfo
    {
        public required Action Callback { get; set; }
    }
    private class InvokeWaitInfoTask()
    {
        public required Func<Task> Callback { get; set; }
    }
    internal static bool InvokeCallback(IntPtr data)
    {
        GCHandle handle = GCHandle.FromIntPtr(data);
        var waitInfo = (InvokeWaitInfo?)handle.Target;
        waitInfo?.Callback.Invoke();
        return false;
    }
    public override async Task InvokeAsync(Func<Task> workItem)
    {
        if (CheckAccess()) await workItem();
        else
        {
            var ac = new InvokeWaitInfoTask() { Callback = workItem };
            var fun = Marshal.GetFunctionPointerForDelegate(new GdkIdleFunc(InvokeCallback));
            var data = GCHandle.ToIntPtr(GCHandle.Alloc(ac));
            GtkApi.gdk_threads_add_idle(fun, data);
        }
    }

    public override void Invoke(Action workItem)
    {
        if (CheckAccess()) workItem();
        else
        {
            var ac = new InvokeWaitInfo() { Callback = workItem };
            var fun = Marshal.GetFunctionPointerForDelegate(new GdkIdleFunc(InvokeCallback));
            var data = GCHandle.ToIntPtr(GCHandle.Alloc(ac));
            GtkApi.gdk_threads_add_idle(fun, data);
        }
    }

    private IWebKit? webKit;
    protected override Task InitWebControl()
    {
        webKit = ServiceProvide!.GetRequiredService<IWebKit>();
        try
        {
            webKit.InitWebControl(this);
            webKit.WebMessageReceived += (s, e) => WebMessageReceived?.Invoke(s, e);
            return Task.Delay(1);
        }
        catch (Exception e)
        {
            throw new Exception("界面初始化失败！原因：" + e.Message);
        }
    }

    public override async Task ExecuteJavaScript(string js, Action<string>? handlResult = null)
    {
        await Task.Run(async () =>
          {
              while (webKit == null)
                  await Task.Delay(10);
              await Task.Delay(100);
              await Task.Delay(100);
              Invoke(() =>
              {
                  var res = webKit!.ExecuteJavaScript(js);
                  handlResult?.Invoke(res);
              });
          });
    }

    public override async Task InjectJsObject(string name, object obj)
    {
        string js = $"window.external.{name} = {JsonConvert.SerializeObject(obj)}";
        await ExecuteJavaScript(js);
    }

    public override void OpenDevTool()
    {
        Task.Run(() =>
        {
            while (webKit == null)
                Thread.Sleep(10);
            Thread.Sleep(10);
            Invoke(() => webKit.OpenDevTool());
        });
    }

    public override void SendWebMessage(string message)
    {
        webKit!.SendWebMessage(message);
    }

    public override void Reload()
    {
        Task.Run(() =>
        {
            while (webKit == null)
                Thread.Sleep(10);
            Thread.Sleep(10);
            Invoke(() => webKit.Reload());
        });
    }

    public override void Navigate(string url)
    {
        Invoke(() => webKit!.Navigate(url));
    }
    #endregion
}