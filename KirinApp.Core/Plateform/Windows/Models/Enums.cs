using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KirinAppCore.Plateform.Windows.Models;


[Flags]
public enum WindowExStyle : uint
{
    DLGMODALFRAME = 0x00000001,
    TOPMOST = 0x00000008,
    TOOLWINDOW = 0x00000080,
    NOACTIVATE = 0x00000040,
    APPWINDOW = 0x00040000,
    LAYERED = 0x00080000,
    WINDOWEDGE = 0x00000100,
    TRANSPARENT = 0x00000020,
    COMPOSITED = 0x02000000,
    NOINHERITLAYOUT = 0x00100000,
    LAYOUTRTL = 0x00400000,

}

[Flags]
public enum WindowStyle : uint
{
    OVERLAPPED = 0x00000000,
    CAPTION = 0xc00000,
    POPUP = 0x80000000,
    CHILD = 0x40000000,
    MINIMIZE = 0x20000000,
    VISIBLE = 0x10000000,
    DISABLED = 0x08000000,
    CLIPSIBLINGS = 0x04000000,
    CLIPCHILDREN = 0x02000000,
    MAXIMIZE = 0x01000000,
    BORDER = 0x00800000,
    DLGFRAME = 0x00400000,
    VSCROLL = 0x00200000,
    HSCROLL = 0x00100000,
    SYSMENU = 0x00080000,
    THICKFRAME = 0x00040000,
    GROUP = 0x00020000,
    TABSTOP = 0x00010000,
    MINIMIZEBOX = 0x20000,
    MAXIMIZEBOX = 0x10000,
    SIZEFRAME = 0x40000,
    POPUPWINDOW = POPUP | BORDER | SYSMENU,
    OVERLAPPEDWINDOW = OVERLAPPED | CAPTION | SYSMENU | SIZEFRAME | MINIMIZEBOX | MAXIMIZEBOX,
}

public enum SW : int
{
    HIDE,
    NORMAL,
    MINIMIZED,
    MAXIMIZE,
    SHOWNOACTIVATE,
    SHOW,
    MINIMIZE,
    SHOWMINNOACTIVE,
    SHOWNA,
    RESTORE,
    SHOWDEFAULT,
    FORCEMINIMIZE,
}

public enum WindowMessage : uint
{
    WM_NULL = 0x0000,                 // 空消息
    WM_CREATE = 0x0001,               // 创建窗口
    WM_DESTROY = 0x0002,              // 销毁窗口
    WM_MOVE = 0x0003,                 // 窗口移动
    WM_SIZE = 0x0005,                 // 窗口大小改变
    WM_ACTIVATE = 0x0006,             // 激活窗口
    WM_SETFOCUS = 0x0007,             // 窗口获得焦点
    WM_KILLFOCUS = 0x0008,            // 窗口失去焦点
    WM_ENABLE = 0x000A,                // 启用或禁用窗口
    WM_SETREDRAW = 0x000B,            // 设置重绘标志
    WM_SETTEXT = 0x000C,              // 设置窗口文本
    WM_GETTEXT = 0x000D,              // 获取窗口文本
    WM_GETTEXTLENGTH = 0x000E,        // 获取窗口文本长度
    WM_PAINT = 0x000F,                // 绘制窗口
    WM_CLOSE = 0x0010,                // 关闭窗口
    WM_QUERYENDSESSION = 0x0011,      // 查询结束会话
    WM_QUIT = 0x0012,                 // 退出应用程序
    WM_QUERYOPEN = 0x0013,            // 查询窗口是否打开
    WM_ERASEBKGND = 0x0014,           // 擦除背景
    WM_SYSCOLORCHANGE = 0x0015,       // 系统颜色变化
    WM_ENDSESSION = 0x0016,           // 结束会话
    WM_SYSTEMERROR = 0x0017,          // 系统错误
    WM_SHOWWINDOW = 0x0018,           // 显示窗口
    WM_ACTIVATEAPP = 0x001C,          // 激活应用程序
    WM_FONTCHANGE = 0x001D,           // 字体变化
    WM_TIMECHANGE = 0x001E,           // 时间变化
    WM_CANCELMODE = 0x001F,           // 取消模式
    WM_SETCURSOR = 0x0020,            // 设置光标
    WM_MOUSEACTIVATE = 0x0021,        // 鼠标激活窗口
    WM_CHILDACTIVATE = 0x0022,        // 激活子窗口
    WM_QUEUESYNC = 0x0023,            // 队列同步
    WM_GETMINMAXINFO = 0x0024,        // 获取最小最大信息
    WM_PAINTICON = 0x0026,            // 绘制图标
    WM_ICONERASEBKGND = 0x0027,       // 擦除图标背景
    WM_NEXTDLGCTL = 0x0028,           // 下一个对话框控件
    WM_DRAWITEM = 0x002B,             // 绘制项目
    WM_MEASUREITEM = 0x002C,          // 测量项目
    WM_DELETEITEM = 0x002D,           // 删除项目
    WM_VKEYTOITEM = 0x002E,           // 虚拟键到项目
    WM_CHARTOITEM = 0x002F,           // 字符到项目
    WM_SETFONT = 0x0030,              // 设置字体
    WM_GETFONT = 0x0031,              // 获取字体
    WM_SETHOTKEY = 0x0032,            // 设置热键
    WM_GETHOTKEY = 0x0033,            // 获取热键
    WM_QUERYDRAGICON = 0x0037,        // 查询拖动图标
    WM_COMPAREITEM = 0x0039,          // 比较项目
    WM_GETOBJECT = 0x003D,            // 获取对象
    WM_COMPACTING = 0x0041,           // 压缩
    WM_COMMNOTIFY = 0x0044,           // 通信通知
    WM_WINDOWPOSCHANGED = 0x0047,     // 窗口位置改变
    WM_WINDOWPOSCHANGING = 0x0046,    // 窗口位置变化
    WM_POWER = 0x0048,                 // 电源状态变化
    WM_COPYDATA = 0x004A,             // 复制数据
    WM_CANCELJOURNAL = 0x004B,        // 取消日志
    WM_NOTIFY = 0x004E,               // 通知消息
    WM_INPUTLANGCHANGEREQUEST = 0x0050, // 输入语言变化请求
    WM_INPUTLANGCHANGE = 0x0051,      // 输入语言变化
    WM_TCARD = 0x0052,                // T卡消息
    WM_HELP = 0x0053,                 // 帮助请求
    WM_USERCHANGED = 0x0054,          // 用户变化
    WM_NOTIFYFORMAT = 0x0055,         // 通知格式
    WM_CONTEXTMENU = 0x007B,          // 上下文菜单
    WM_STYLECHANGED = 0x007C,         // 样式改变
    WM_STYLECHANGING = 0x007D,        // 样式变化
    WM_SIZING = 0x0210,               // 窗口大小调整
    WM_CAPTURECHANGED = 0x0215,       // 捕获改变
    WM_DEVICECHANGE = 0x0219,         // 设备变化
    WM_MDICREATE = 0x0220,            // 创建MDI子窗口
    WM_MDIDESTROY = 0x0221,           // 销毁MDI子窗口
    WM_MDIACTIVATE = 0x0222,          // 激活MDI子窗口
    WM_MDIREFRESHMENU = 0x0223,       // 刷新MDI菜单
    WM_MDITILE = 0x0224,              // 瓶子排列MDI子窗口
    WM_MDICASCADE = 0x0225,           // 级联MDI子窗口
    WM_MDIICONARRANGE = 0x0226,       // 排列MDI子窗口图标
    WM_MDIGETACTIVE = 0x0227,         // 获取活动MDI子窗口
    WM_MDISETMENU = 0x0228,           // 设置MDI菜单
    WM_MDISETWINDOWPOS = 0x0229,      // 设置MDI窗口位置
    // 其他消息...
}

public enum DeviceCapability : int
{
    HORZRES = 8,
    VERTRES = 10,
    BITSPIXEL = 12,
    PLANES = 14,
    LOGPIXELSX = 88,
    LOGPIXELSY = 90
}