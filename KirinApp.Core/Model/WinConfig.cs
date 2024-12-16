using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KirinAppCore.Model;

/// <summary>
/// 窗体配置类
/// </summary>
public class WinConfig
{
    /// <summary>
    /// 程序名
    /// </summary>
    public string AppName { get; set; } = "";

    /// <summary>
    /// 图标
    /// </summary>
    public string Icon { get; set; } = "";

    /// <summary>
    /// 高度
    /// </summary>
    public int Height { get; set; } = 600;

    /// <summary>
    /// 宽度
    /// </summary>
    public int Width { get; set; } = 800;

    /// <summary>
    /// 尺寸（优先）
    /// </summary>
    public Size? Size { get; set; }

    /// <summary>
    /// 无边框
    /// </summary>
    public bool Chromeless { get; set; } = false;

    /// <summary>
    /// 调试模式
    /// </summary>
    public bool Debug { get; set; } = false;

    /// <summary>
    /// 类型
    /// </summary>
    public WebAppType AppType { get; set; } = WebAppType.RawString;

    /// <summary>
    /// 距离屏幕左边宽度
    /// </summary>
    public int Left { get; set; }

    /// <summary>
    /// 距离屏幕顶部高度
    /// </summary>
    public int Top { get; set; }

    /// <summary>
    /// 是否能调整大小
    /// </summary>
    public bool ResizeAble { get; set; } = true;

    /// <summary>
    /// 是否居中显示
    /// </summary>
    public bool Center { get; set; } = true;

    /// <summary>
    /// url
    /// </summary>
    public string? Url { get; set; }

    /// <summary>
    /// 内容
    /// </summary>
    public string? RawString { get; set; }

    /// <summary>
    /// 最小尺寸（优先）
    /// </summary>
    public Size? MinimumSize { get; set; }

    /// <summary>
    /// 最大尺寸（优先）
    /// </summary>
    public Size? MaximumSize { get; set; }

    /// <summary>
    /// 最小高度
    /// </summary>
    public int MinimumHeigh { get; set; }

    /// <summary>
    /// 最小宽度
    /// </summary>
    public int MinimumWidth { get; set; }

    /// <summary>
    /// 最大高度
    /// </summary>
    public int MaximumHeigh { get; set; }

    /// <summary>
    /// 最小宽度
    /// </summary>
    public int MaximumWidth { get; set; }

    /// <summary>
    /// Blazor组件
    /// </summary>
    public Type? BlazorComponent { get; set; }

    /// <summary>
    /// blazor选择器
    /// </summary>
    public string BlazorSelector { get; set; } = "#app";
}

/// <summary>
/// 显示器
/// </summary>
public class Monitor
{
    /// <summary>
    /// 高度
    /// </summary>
    public int Height { get; set; }

    /// <summary>
    /// 宽度
    /// </summary>
    public int Width { get; set; }
}
