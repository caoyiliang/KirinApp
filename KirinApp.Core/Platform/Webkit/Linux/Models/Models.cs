using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace KirinAppCore.Plateform.Webkit.Linux.Models;

[StructLayout(LayoutKind.Sequential)]
public struct GdkRGBA
{
    public double Red;
    public double Green;
    public double Blue;
    public double Alpha;
}
[Flags]
internal enum GtkWindowPosition
{
    GtkWinPosNone,
    GtkWinPosCenter,
    GtkWinPosMouse,
    GtkWinPosCenterAlways,
    GtkWinPosCenterOnParent
}
public struct GeometryInfo
{
    public int min_width { get; set; }
    public int min_height { get; set; }
    public int max_width { get; set; }
    public int max_height { get; set; }
}
[StructLayout(LayoutKind.Sequential)]
public struct GdkRectangle
{
    public int X;
    public int Y;
    public int Width;
    public int Height;
}

[StructLayout(LayoutKind.Sequential)]
public struct GdkEventConfigure
{
    public IntPtr type;
    public IntPtr window;
    public int x;
    public int y;
    public int width;
    public int height;
    public int above;
    public int border_width;
    public int send_event;
}