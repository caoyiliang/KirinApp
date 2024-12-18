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

namespace KirinAppCore.Plateform.Webkit.IOS;

/// <summary>
/// Windows实现类
/// </summary>
internal class MainWIndow : IWindow
{
    private WebManager? WebManager { get; set; }
    private SchemeConfig? SchemeConfig { get; set; }

    #region 事件
    public override event EventHandler<WebMessageEvent>? WebMessageReceived;
    public override event EventHandler<EventArgs>? OnCreate;
    public override event EventHandler<EventArgs>? Created;
    public override event EventHandler<EventArgs>? OnLoad;
    public override event EventHandler<EventArgs>? Loaded;
    #endregion

    #region 窗体方法
    protected override void Create()
    {
        OnCreate?.Invoke(this, new());

        Created?.Invoke(this, new());
    }
    public override void Show()
    {

    }

    public override void Hide()
    {

    }

    public override void Focus()
    {

    }

    public override void MainLoop()
    {

    }

    public override Rect GetClientSize()
    {
        Rect rect = new();
        return rect;
    }

    public override void Maximize()
    {

    }

    public override void Minimize()
    {

    }

    public override void SizeChange(IntPtr handle, int width, int height)
    {

    }

    private void CheckInitialDir(ref string initialDir)
    {

    }

    private void CheckFileFilter(Dictionary<string, string>? dic)
    {

    }

    public override (bool selected, DirectoryInfo? dir) OpenDirectory(string initialDir = "")
    {
        CheckInitialDir(ref initialDir);
        return (false, null);
    }

    public override (bool selected, FileInfo? file) OpenFile(string initialDir = "", Dictionary<string, string>? fileTypeFilter = null)
    {
        CheckInitialDir(ref initialDir);
        CheckFileFilter(fileTypeFilter);
        return (false, null);
    }

    public override (bool selected, List<FileInfo>? files) OpenFiles(string initialDir = "", Dictionary<string, string>? fileTypeFilter = null)
    {
        CheckInitialDir(ref initialDir);
        CheckFileFilter(fileTypeFilter);
        return (false, null);
    }

    public override MsgResult ShowDialog(string title, string msg, MsgBtns btn = MsgBtns.OK, MessageType messageType = MessageType.Info)
    {
        return MsgResult.OK;
    }

    /// <summary>
    /// 获取屏幕信息
    /// </summary>
    public override void SetScreenInfo()
    {

    }
    #endregion

    #region Webkit方法
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
            //发送系统消息，在主线程执行
            //Win32Api.PostMessage(Handle, (uint)WindowMessage.DIY_FUN, actionPtr, IntPtr.Zero);
        }
    }

    protected override async Task InitWebControl()
    {
        try
        {
            OnLoad?.Invoke(this, new());
            await Task.Delay(1);
            Loaded?.Invoke(this, new());
        }
        catch (Exception)
        {
            throw;
        }
    }

    public override void ExecuteJavaScript(string js)
    {
        Task.Run(() =>
        {
            //while (CoreWebCon == null)
            //    Thread.Sleep(10);
            //Thread.Sleep(10);
            //IntPtr actionPtr = Marshal.GetFunctionPointerForDelegate(() =>
            //{
            //    _ = CoreWebCon.CoreWebView2.ExecuteScriptAsync(js).Result;
            //});
            //Win32Api.PostMessage(Handle, (uint)WindowMessage.DIY_FUN, actionPtr, IntPtr.Zero);
        });
    }

    public override string ExecuteJavaScriptWithResult(string js)
    {
        var tcs = new TaskCompletionSource<string>();
        Task.Run(() =>
        {
            //while (CoreWebCon == null)
            //    Task.Delay(10);
            //Task.Delay(10);
            //// 创建指向结果的委托
            //IntPtr actionPtr = Marshal.GetFunctionPointerForDelegate(new Action(async () =>
            //{
            //    string res = await CoreWebCon.CoreWebView2.ExecuteScriptAsync(js);
            //    tcs.SetResult(res); // 设置结果
            //}));

            //// 发送消息
            //Win32Api.PostMessage(Handle, (uint)WindowMessage.DIY_FUN, actionPtr, IntPtr.Zero);
        });
        // 等待结果
        return tcs.Task.Result; // 返回结果
    }

    public override void OpenDevTool()
    {
        Task.Run(() =>
        {
            //while (CoreWebCon == null)
            //    Thread.Sleep(10);
            //Thread.Sleep(10);
            //IntPtr actionPtr = Marshal.GetFunctionPointerForDelegate(() => CoreWebCon!.CoreWebView2.OpenDevToolsWindow());
            //Win32Api.PostMessage(Handle, (uint)WindowMessage.DIY_FUN, actionPtr, IntPtr.Zero);
        });
    }

    public override void SendWebMessage(string message)
    {

    }

    public override void Reload()
    {
        Task.Run(() =>
        {
            //IntPtr actionPtr = Marshal.GetFunctionPointerForDelegate(() =>
            //{
            //    ResourceRequest();
            //    CoreWebCon?.CoreWebView2.Reload();
            //});
            //if (Config.AppType == WebAppType.Http)
            //    actionPtr = Marshal.GetFunctionPointerForDelegate(() => CoreWebCon!.CoreWebView2.Navigate(Config.Url));
            //Win32Api.PostMessage(Handle, (uint)WindowMessage.DIY_FUN, actionPtr, IntPtr.Zero);
        });
    }

    public override void Navigate(string url)
    {
        throw new NotImplementedException();
    }
    #endregion
}