using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace KirinAppCore.Model;


[StructLayout(LayoutKind.Sequential)]
internal struct Rect
{
    public int Left;
    public int Top;
    public int Right;
    public int Bottom;
    private int _width;
    private int _height;
    public int Width
    {
        get
        {
            if (_width != 0)
                return _width;
            else
                return this.Right - this.Left;
        }
        set
        {
            _width = value;
        }
    }
    public int Height
    {
        get
        {
            if (_height != 0)
                return _height;
            else
                return this.Bottom - this.Top;
        }
        set
        {
            _height = value;
        }
    }
}