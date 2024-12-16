using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KirinAppCore.Model;

public class SizeChangeEventArgs : System.EventArgs
{
    public int Width { get; set; }
    public int Height { get; set; }
}

public class PositionChangeEventArgs : System.EventArgs
{
    public int X { get; set; }
    public int Y { get; set; }
}