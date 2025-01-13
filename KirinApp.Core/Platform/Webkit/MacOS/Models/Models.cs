using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace KirinAppCore.Plateform.MacOS;

[StructLayout(LayoutKind.Sequential)]
public struct CGRect
{
    public double X;
    public double Y;
    public double Width;
    public double Height;

    public CGRect(double x, double y, double width, double height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }
}