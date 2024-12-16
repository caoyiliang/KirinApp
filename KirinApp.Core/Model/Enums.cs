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


public enum WindowMessage : uint
{
    NULL = 0x0000,                 // 空消息
    CREATE = 0x0001,               // 创建窗口
    DESTROY = 0x0002,              // 销毁窗口
    MOVE = 0x0003,                 // 窗口移动
    SIZE = 0x0005,                 // 窗口大小改变
    ACTIVATE = 0x0006,             // 激活窗口
    SETFOCUS = 0x0007,             // 窗口获得焦点
    KILLFOCUS = 0x0008,            // 窗口失去焦点
    ENABLE = 0x000A,                // 启用或禁用窗口
    SETREDRAW = 0x000B,            // 设置重绘标志
    SETTEXT = 0x000C,              // 设置窗口文本
    GETTEXT = 0x000D,              // 获取窗口文本
    GETTEXTLENGTH = 0x000E,        // 获取窗口文本长度
    PAINT = 0x000F,                // 绘制窗口
    CLOSE = 0x0010,                // 关闭窗口
    QUERYENDSESSION = 0x0011,      // 查询结束会话
    QUIT = 0x0012,                 // 退出应用程序
    QUERYOPEN = 0x0013,            // 查询窗口是否打开
    ERASEBKGND = 0x0014,           // 擦除背景
    SYSCOLORCHANGE = 0x0015,       // 系统颜色变化
    ENDSESSION = 0x0016,           // 结束会话
    SYSTEMERROR = 0x0017,          // 系统错误
    SHOWWINDOW = 0x0018,           // 显示窗口
    ACTIVATEAPP = 0x001C,          // 激活应用程序
    FONTCHANGE = 0x001D,           // 字体变化
    TIMECHANGE = 0x001E,           // 时间变化
    CANCELMODE = 0x001F,           // 取消模式
    SETCURSOR = 0x0020,            // 设置光标
    MOUSEACTIVATE = 0x0021,        // 鼠标激活窗口
    CHILDACTIVATE = 0x0022,        // 激活子窗口
    QUEUESYNC = 0x0023,            // 队列同步
    GETMINMAXINFO = 0x0024,        // 获取最小最大信息
    PAINTICON = 0x0026,            // 绘制图标
    ICONERASEBKGND = 0x0027,       // 擦除图标背景
    NEXTDLGCTL = 0x0028,           // 下一个对话框控件
    DRAWITEM = 0x002B,             // 绘制项目
    MEASUREITEM = 0x002C,          // 测量项目
    DELETEITEM = 0x002D,           // 删除项目
    VKEYTOITEM = 0x002E,           // 虚拟键到项目
    CHARTOITEM = 0x002F,           // 字符到项目
    SETFONT = 0x0030,              // 设置字体
    GETFONT = 0x0031,              // 获取字体
    SETHOTKEY = 0x0032,            // 设置热键
    GETHOTKEY = 0x0033,            // 获取热键
    QUERYDRAGICON = 0x0037,        // 查询拖动图标
    COMPAREITEM = 0x0039,          // 比较项目
    GETOBJECT = 0x003D,            // 获取对象
    COMPACTING = 0x0041,           // 压缩
    COMMNOTIFY = 0x0044,           // 通信通知
    WINDOWPOSCHANGED = 0x0047,     // 窗口位置改变
    WINDOWPOSCHANGING = 0x0046,    // 窗口位置变化
    POWER = 0x0048,                 // 电源状态变化
    COPYDATA = 0x004A,             // 复制数据
    CANCELJOURNAL = 0x004B,        // 取消日志
    NOTIFY = 0x004E,               // 通知消息
    INPUTLANGCHANGEREQUEST = 0x0050, // 输入语言变化请求
    INPUTLANGCHANGE = 0x0051,      // 输入语言变化
    TCARD = 0x0052,                // T卡消息
    HELP = 0x0053,                 // 帮助请求
    USERCHANGED = 0x0054,          // 用户变化
    NOTIFYFORMAT = 0x0055,         // 通知格式
    CONTEXTMENU = 0x007B,          // 上下文菜单
    STYLECHANGED = 0x007C,         // 样式改变
    STYLECHANGING = 0x007D,        // 样式变化
    SIZING = 0x0210,               // 窗口大小调整
    CAPTURECHANGED = 0x0215,       // 捕获改变
    DEVICECHANGE = 0x0219,         // 设备变化
    MDICREATE = 0x0220,            // 创建MDI子窗口
    MDIDESTROY = 0x0221,           // 销毁MDI子窗口
    MDIACTIVATE = 0x0222,          // 激活MDI子窗口
    MDIREFRESHMENU = 0x0223,       // 刷新MDI菜单
    MDITILE = 0x0224,              // 瓶子排列MDI子窗口
    MDICASCADE = 0x0225,           // 级联MDI子窗口
    MDIICONARRANGE = 0x0226,       // 排列MDI子窗口图标
    MDIGETACTIVE = 0x0227,         // 获取活动MDI子窗口
    MDISETMENU = 0x0228,           // 设置MDI菜单
    MDISETWINDOWPOS = 0x0229,      // 设置MDI窗口位置
                                   // 其他消息...
    DIY_FUN = 0x8000,
}
internal enum MessageType
{
    Info,
    Warning,
    Question,
    Error,
    Other
}