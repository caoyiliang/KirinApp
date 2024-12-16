using KirinAppCore.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace KirinAppCore.Plateform.WebView2.Windows.Models;
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
internal struct WNDCLASS
{
    public uint style;
    public IntPtr lpfnWndProc;
    public int cbClsExtra;
    public int cbWndExtra;
    public IntPtr hInstance;
    public IntPtr hIcon;
    public IntPtr hCursor;
    public IntPtr hbrBackground;

    [MarshalAs(UnmanagedType.LPWStr)] public string? lpszMenuName;

    [MarshalAs(UnmanagedType.LPWStr)] public string lpszClassName;
}
internal static class CursorResource
{
    public const int IDC_ARROW = 32512;
    public const int IDC_IBEAM = 32513;
    public const int IDC_WAIT = 32514;
    public const int IDC_CROSS = 32515;
    public const int IDC_SIZEALL = 32646;
    public const int IDC_SIZENWSE = 32642;
    public const int IDC_SIZENESW = 32643;
    public const int IDC_SIZEWE = 32644;
    public const int IDC_SIZENS = 32645;
    public const int IDC_UPARROW = 32516;
    public const int IDC_NO = 32648;
    public const int IDC_HAND = 32649;
    public const int IDC_APPSTARTING = 32650;
    public const int IDC_HELP = 32651;
}

[StructLayout(LayoutKind.Sequential)]
internal struct POINT
{
    public int X;
    public int Y;
}
[StructLayout(LayoutKind.Sequential)]
internal struct MSG
{
    public IntPtr hwnd;
    public WindowMessage message;
    public IntPtr wParam;
    public IntPtr lParam;
}
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
internal struct BrowseInfo
{
    public IntPtr hwndOwner;
    public IntPtr pidlRoot;
    public IntPtr pszDisplayName;
    public string lpszTitle;
    public uint ulFlags;
    public IntPtr lpfn;
    public IntPtr lParam;
    public int iImage;
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
internal struct OpenFileDialogParams
{
    public int structSize;
    public IntPtr ownerHandle;
    public IntPtr instanceHandle;
    public string filter;
    public string customFilter;
    public int filterIndex;
    public IntPtr file;
    public int maxFile;
    public string fileTitle;
    public int maxFileTitle;
    public string initialDir;
    public string title;
    public int flags;
    public short fileOffset;
    public short fileExtension;
    public string defExt;
    public IntPtr custData;
    public IntPtr hook;
    public string templateName;
    public IntPtr reservedPtr;
    public int reservedInt;
    public int flagsEx;
}

[StructLayout(LayoutKind.Sequential)]
internal struct NotifyIconData
{
    public uint cbSize;
    public IntPtr hWnd;
    public uint uID;
    public uint uFlags;
    public IntPtr hIcon;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
    public string szTip;
    public uint dwState;
    public uint dwStateMask;
    public uint uVersion;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
    public string szInfo;
    public uint uTimeoutOrVersion;
    public string szInfoTitle;
    public uint dwInfoFlags;
    public Guid guidItem;
    public IntPtr hBalloonIcon;
}

[StructLayout(LayoutKind.Sequential)]
internal struct MinMaxInfo
{
    public POINT ptReserved;
    public POINT ptMaxSize;
    public POINT ptMaxTrackSize;
    public POINT ptMinTrackSize;
    public POINT ptMinSize;
}