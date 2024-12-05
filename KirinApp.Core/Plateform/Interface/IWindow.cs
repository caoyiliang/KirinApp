using KirinAppCore.Model;
using KirinAppCore.Plateform.Windows;
using KirinAppCore.Plateform.Windows.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace KirinAppCore.Interface;

/// <summary>
/// 通用抽象类
/// </summary>
internal abstract class IWindow
{
    #region WebView2变量
    protected CoreWebView2Environment? CoreWebEnv;
    protected CoreWebView2Controller? CoreWebCon;
    protected ServiceProvider? ServiceProvide;
    protected int ManagedThreadId;
    #endregion

    #region 窗体变量
    /// <summary>
    /// 窗体句柄
    /// </summary>
    public IntPtr Handle { get; protected set; } = IntPtr.Zero;

    /// <summary>
    /// 窗体过程
    /// </summary>
    protected WndProcDelegate? WindowProc;

    /// <summary>
    /// 窗体过程委托
    /// </summary>
    /// <param name="hWnd"></param>
    /// <param name="msg"></param>
    /// <param name="wParam"></param>
    /// <param name="lParam"></param>
    /// <returns></returns>
    protected delegate IntPtr WndProcDelegate(IntPtr hWnd, WindowMessage msg, IntPtr wParam, IntPtr lParam);

    /// <summary>
    /// 窗体状态
    /// </summary>
    public WindowState State { get; protected set; } = WindowState.Normal;

    /// <summary>
    /// 窗体配置
    /// </summary>
    public WinConfig Config { get; protected set; } = new();

    /// <summary>
    /// 主显示器
    /// </summary>
    public Model.Monitor MainMonitor { get; protected set; } = new();

    /// <summary>
    /// 显示器
    /// </summary>
    public List<Model.Monitor> Monitors { get; protected set; } = new();
    #endregion

    #region 事件
    /// <summary>
    /// 接收消息事件
    /// </summary>
    public virtual event EventHandler<CoreWebView2WebMessageReceivedEventArgs>? WebMessageReceived;

    /// <summary>
    /// 窗口大小改变事件
    /// </summary>
    public virtual event EventHandler<SizeChangeEventArgs>? SizeChangeEvent;

    /// <summary>
    /// 窗体创建前
    /// </summary>
    public virtual event EventHandler<EventArgs>? OnCreate;

    /// <summary>
    /// 窗体创建完成
    /// </summary>
    public virtual event EventHandler<EventArgs>? Created;

    /// <summary>
    /// 页面加载前
    /// </summary>
    public virtual event EventHandler<EventArgs>? OnLoad;

    /// <summary>
    /// 页面加载完成
    /// </summary>
    public virtual event EventHandler<EventArgs>? Loaded;

    /// <summary>
    /// 关闭委托
    /// </summary>
    public virtual event NetClosingDelegate? OnClose;
    public delegate bool? NetClosingDelegate(object sender, EventArgs e);
    #endregion

    #region 窗体方法
    /// <summary>
    /// 窗口过程
    /// </summary>
    protected virtual IntPtr WndProc(IntPtr hwnd, WindowMessage message, IntPtr wParam, IntPtr lParam)
    {
        switch (message)
        {
            case WindowMessage.SIZE:
                {
                    var size = GetClientSize();
                    SizeChangeEvent?.Invoke(this, new SizeChangeEventArgs() { Width = size.Width(), Height = size.Height() });
                    break;
                }
            case WindowMessage.CLOSE:
                {
                    var res = OnClose?.Invoke(this, new());
                    if (res == null || res.Value)
                    {
                        Handle = IntPtr.Zero;
                        Environment.Exit(0);
                        return IntPtr.Zero;
                    }
                    else return IntPtr.Zero;
                }
        }
        return Win32Api.DefWindowProcW(hwnd, message, wParam, lParam);
    }

    /// <summary>
    /// 获取当前窗口的大小位置信息
    /// </summary>
    /// <returns></returns>
    public abstract Rect GetClientSize();
    /// <summary>
    /// 初始化
    /// </summary>
    public virtual void Init(ServiceProvider serviceProvider, WinConfig winConfig)
    {
        ServiceProvide = serviceProvider;
        ManagedThreadId = Environment.CurrentManagedThreadId;
        Config = winConfig;
        Win32Api.SetProcessDPIAware();
        SetScreenInfo();
        Create();
        InitWebControl();
        SystemTary();
        ShowSysMsg("123","456");
        SizeChangeEvent += (s, e) => SizeChange(Handle, e.Width, e.Height);
    }

    /// <summary>
    /// 创建窗体
    /// </summary>
    protected abstract void Create();

    /// <summary>
    /// 隐藏
    /// </summary>
    public abstract void Hide();

    /// <summary>
    /// 显示
    /// </summary>
    public abstract void Show();

    /// <summary>
    /// 聚焦
    /// </summary>
    public abstract void Focus();

    /// <summary>
    /// 关闭
    /// </summary>
    public virtual void Close() => Environment.Exit(0);

    /// <summary>
    /// 消息循环
    /// </summary>
    public abstract void MessageLoop();

    /// <summary>
    /// 最小化
    /// </summary>
    public abstract void Minimize();

    /// <summary>
    /// 最大化
    /// </summary>
    public abstract void Maximize();

    /// <summary>
    /// 显示对话框
    /// </summary>
    /// <param name="title">标题</param>
    /// <param name="msg">消息</param>
    /// <param name="btn">按钮</param>
    /// <returns></returns>
    public abstract MsgResult ShowDialog(string title, string msg, MsgBtns btn = MsgBtns.OK);

    /// <summary>
    /// 打开文件选择
    /// </summary>
    /// <param name="initialDir">初始目录</param>
    /// <param name="fileTypeFilter">文件类型过滤</param>
    /// <returns></returns>
    public abstract (bool selected, FileInfo? file) OpenFile(string initialDir = "", Dictionary<string, string>? fileTypeFilter = null);

    /// <summary>
    /// 打开文件选择（多选）
    /// </summary>
    /// <param name="initialDir">初始目录</param>
    /// <param name="fileTypeFilter">文件类型过滤</param>
    /// <returns></returns>
    public abstract (bool selected, List<FileInfo>? files) OpenFiles(string initialDir = "", Dictionary<string, string>? fileTypeFilter = null);

    /// <summary>
    /// 打开文件夹
    /// </summary>
    /// <param name="initialDir"></param>
    /// <returns></returns>
    public abstract (bool selected, DirectoryInfo? dir) OpenDirectory(string initialDir = "");

    /// <summary>
    /// 大小改变，需要修改web控件
    /// </summary>
    /// <param name="handle"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    public abstract void SizeChange(IntPtr handle, int width, int height);

    /// <summary>
    /// 获取屏幕信息
    /// </summary>
    public abstract void SetScreenInfo();

    /// <summary>
    /// 系统托盘任务
    /// </summary>
    public virtual void SystemTary() { return; }

    /// <summary>
    /// 发送气泡通知
    /// </summary>
    /// <param name="title">标题</param>
    /// <param name="msg">消息</param>
    public virtual void ShowTaryMsg(string title, string msg) { return; }

    /// <summary>
    /// 发送系统通知
    /// </summary>
    /// <param name="title">标题</param>
    /// <param name="msg">消息</param>
    public virtual void ShowSysMsg(string title, string msg) { return; }
    #endregion

    #region WebVew2方法
    /// <summary>
    /// 初始化浏览器控件(blazor支持)
    /// </summary>
    /// <returns></returns>
    protected abstract Task InitWebControl();

    /// <summary>
    /// 打开开发者工具
    /// </summary>
    public abstract void OpenDevTool();

    /// <summary>
    /// 线程检测
    /// </summary>
    /// <returns></returns>
    public abstract bool CheckAccess();

    /// <summary>
    /// 执行
    /// </summary>
    /// <param name="workItem"></param>
    /// <returns></returns>
    public abstract Task InvokeAsync(Func<Task> workItem);

    /// <summary>
    /// 执行
    /// </summary>
    /// <param name="workItem"></param>
    /// <returns></returns>
    public abstract Task InvokeAsync(Action workItem);

    /// <summary>
    /// 给前端发送消息
    /// </summary>
    /// <param name="message">消息</param>
    public abstract void SendWebMessage(string message);

    /// <summary>
    /// 执行js代码
    /// </summary>
    /// <param name="js"></param>
    /// <returns></returns>
    public abstract void ExecuteJavaScript(string js);
    public abstract string ExecuteJavaScriptWithResult(string js);
    #endregion
}