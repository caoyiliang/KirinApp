using KirinAppCore.Model;
using KirinAppCore.Plateform.WebView2.Windows.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace KirinAppCore.Plateform.WebView2.Windows;

internal static class Win32Api
{
    internal const string U32 = "user32.dll";
    internal const string k32 = "kernel32.dll";
    internal const string G32 = "gdi32.dll";
    internal const string C32 = "comdlg32.dll";
    internal const string S32 = "shell32.dll";

    [DllImport(U32, CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern IntPtr CreateWindowExW(WindowExStyle dwExStyle, string lpClassName, string lpWindowName, WindowStyle dwStyle,
           int x, int y, int nWidth, int nHeight, IntPtr handleParent, IntPtr hMenu, IntPtr hInstance, object? lpParam);

    [DllImport(k32, CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern IntPtr GetConsoleWindow();

    [DllImport(U32, CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern int SetWindowTextW(IntPtr handle, string text);

    [DllImport(U32, CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern bool UpdateWindow(IntPtr handle);

    [DllImport(U32, CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern bool ShowWindow(IntPtr handle, SW nCmdShow);

    [DllImport(U32, CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern bool SetForegroundWindow(IntPtr hWnd);

    [DllImport(U32, CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern IntPtr DefWindowProcW(IntPtr handle, WindowMessage message, IntPtr wParam, IntPtr lParam);
    [DllImport(U32, CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern ushort RegisterClassW(ref WNDCLASS lpWndClass);
    [DllImport(U32, CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern IntPtr LoadCursorW(IntPtr hInstance, IntPtr lpCursorName);
    [DllImport(U32, CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern int GetSystemMetrics(int nIndex);
    [DllImport(U32, CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern nint GetDC(nint hwnd);
    [DllImport(U32, CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDC);
    [DllImport(G32, CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern int GetDeviceCaps(nint hdc, int nIndex);
    [DllImport(U32, CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern bool GetMessageW(out MSG lpMsg, IntPtr handle, uint wMsgFilterMin, uint wMsgFilterMax);
    [DllImport(U32, CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern bool TranslateMessage([In] ref MSG lpMsg);
    [DllImport(U32, CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern IntPtr DispatchMessageW([In] ref MSG lpmsg);
    [DllImport(U32, CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern void PostQuitMessage(int nExitCode);
    [DllImport(U32, CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern bool GetClientRect(IntPtr hWnd, out Rect lpRect);
    [DllImport(G32, CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern IntPtr CreateSolidBrush(uint crColor);
    [DllImport(U32, CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern bool FillRect(IntPtr hDC, ref Rect lprc, IntPtr hbr);
    [DllImport(U32, CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern bool SetProcessDPIAware();
    [DllImport(U32, CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern bool PostMessage(IntPtr handle, uint Msg, IntPtr wParam, IntPtr lParam);

    [DllImport(S32, CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern IntPtr SHBrowseForFolder(ref BrowseInfo lpbi);
    [DllImport(S32, SetLastError = true, CharSet = CharSet.Auto)]
    internal static extern bool SHGetPathFromIDList(IntPtr pidl, IntPtr pszPath);

    [DllImport(C32, CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern bool GetOpenFileName(ref OpenFileDialogParams param);
    [DllImport(U32, CharSet = CharSet.Unicode, SetLastError = true)]
    internal static extern int MessageBox(IntPtr handle, string msg, string title, int options);

    public delegate bool MonitorEnumProc(IntPtr hMonitor, IntPtr hdcMonitor, ref Rect lprcMonitor, IntPtr dwData);
    [DllImport(U32, CharSet = CharSet.Unicode, SetLastError = true)]
    public static extern bool EnumDisplayMonitors(IntPtr hdc, IntPtr lprcClip, MonitorEnumProc lpfnEnum, IntPtr dwData);
}
