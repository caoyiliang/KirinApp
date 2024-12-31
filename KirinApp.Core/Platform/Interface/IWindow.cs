using KirinAppCore.Model;
using KirinAppCore.Plateform.Webkit.Linux.Models;
using KirinAppCore.Plateform.WebView2.Windows;
using KirinAppCore.Plateform.WebView2.Windows.Models;
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
    //计划方法: 系统托盘，发送系统通知，发送托盘气泡通知

    #region WebView2变量
    public ServiceProvider? ServiceProvide;
    #endregion

    #region 窗体变量
    /// <summary>
    /// 父窗体
    /// </summary>
    public KirinApp? ParentWindows { get; internal set; }

    /// <summary>
    /// 窗体句柄
    /// </summary>
    public IntPtr Handle { get; protected set; } = IntPtr.Zero;

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
    #endregion

    #region 事件
    /// <summary>
    /// 接收消息事件
    /// </summary>
    public virtual event EventHandler<WebMessageEvent>? WebMessageReceived;

    /// <summary>
    /// 窗口大小改变事件
    /// </summary>
    public virtual event EventHandler<SizeChangeEventArgs>? SizeChangeEvent;

    /// <summary>
    /// 窗口位置改变事件
    /// </summary>
    public virtual event EventHandler<PositionChangeEventArgs>? PositionChangeEvent;

    /// <summary>
    /// 窗体创建前
    /// </summary>
    public virtual event EventHandler<EventArgs>? OnCreate;

    /// <summary>
    /// 窗体创建完成
    /// </summary>
    public virtual event EventHandler<EventArgs>? Created;

    /// <summary>
    /// 关闭委托
    /// </summary>
    public virtual event CloseDelegate? OnClose;
    public delegate bool? CloseDelegate(object sender, EventArgs e);
    #endregion

    #region 窗体方法
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
        Config = winConfig;
        OnCreate?.Invoke(this, new EventArgs());
        Create();
        Created?.Invoke(this, new EventArgs());
        InitWebControl();
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
    /// 移动到（相对定位）
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public abstract void MoveTo(int x, int y);

    /// <summary>
    /// 移动到（绝对定位）
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    public abstract void Move(int x, int y);

    /// <summary>
    /// 改变大小
    /// </summary>
    public abstract void Change(int width, int height);

    /// <summary>
    /// 改变大小
    /// </summary>
    /// <param name="size"></param>
    public virtual void Change(Size size) => Change(size.Width, size.Height);

    /// <summary>
    /// 置顶
    /// </summary>
    public abstract void TopMost(bool top);

    /// <summary>
    /// 复原
    /// </summary>
    public abstract void Normal();

    /// <summary>
    /// 关闭
    /// </summary>
    public virtual void Close() => Environment.Exit(0);

    /// <summary>
    /// 消息循环
    /// </summary>
    public abstract void MainLoop();

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
    public abstract MsgResult ShowDialog(string title, string msg, MsgBtns btn = MsgBtns.OK, MessageType messageType = MessageType.Info);

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
    public virtual void SizeChange(IntPtr handle, int width, int height)
    {

    }

    /// <summary>
    /// 获取屏幕信息
    /// </summary>
    public abstract void SetScreenInfo();
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
    public abstract void Invoke(Action workItem);

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
    public abstract Task ExecuteJavaScript(string js, Action<string>? handlResult = null);

    /// <summary>
    /// 注入js对象
    /// </summary>
    /// <param name="name"></param>
    /// <param name="obj"></param>
    /// <returns></returns>
    public abstract Task InjectJsObject(string name, object obj);

    /// <summary>
    /// 重新渲染
    /// </summary>
    public abstract void Reload();

    /// <summary>
    /// 加载url
    /// </summary>
    /// <param name="url"></param>
    public abstract void Navigate(string url);
    #endregion
}