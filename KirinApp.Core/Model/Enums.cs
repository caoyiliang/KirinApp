using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KirinAppCore.Model;

/// <summary>
/// 窗体状态
/// </summary>
public enum WindowState
{
    Hide = -1,
    Normal,
    Minimize,
    Maximize
}

/// <summary>
/// 浏览器内容类别
/// </summary>
public enum WebAppType
{
    RawString,
    Blazor,
    Static,
    Http,
}

/// <summary>
/// 消息框按钮类型
/// </summary>
public enum MsgBtns
{
    OK,
    OKCancel,
    YesNo,
    YesNoCancel
}

/// <summary>
/// 消息框结果
/// </summary>
public enum MsgResult
{
    OK,
    Cancel,
    Yes,
    No
}

/// <summary>
/// 发送消息枚举
/// </summary>
public enum SystemCommands
{
    /// <summary>
    /// 关闭
    /// </summary>
    Close = 0,
    /// <summary>
    /// 最小化
    /// </summary>
    Minimize,
    /// <summary>
    /// 还原
    /// </summary>
    Restore,
    /// <summary>
    /// 最大化
    /// </summary>
    Maximized,
    /// <summary>
    /// 浏览器版本
    /// </summary>
    BrowserVersion,
    /// <summary>
    /// 错误对话框
    /// </summary>
    ShowError,
    /// <summary>
    /// 对话框
    /// </summary>
    ShowMessage,
    /// <summary>
    /// 打开系统浏览器
    /// </summary>
    SystemBroswer,
    /// <summary>
    /// 选择目录
    /// </summary>
    OpenDirectory,
    /// <summary>
    /// 选择文件
    /// </summary>
    OpenFile,
    /// <summary>
    /// 选择文件（多选）
    /// </summary>
    OpenFiles,
}