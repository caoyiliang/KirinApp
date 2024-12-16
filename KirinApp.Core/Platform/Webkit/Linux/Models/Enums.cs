using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KirinAppCore.Plateform.Webkit.Linux.Models;

internal enum GdkInterpType
{
    GDK_INTERP_NEAREST,
    GDK_INTERP_TILES,
    GDK_INTERP_BILINEAR,
    GDK_INTERP_HYPER,
    GDK_INTERP_LAST
}

public enum GdkWindowHints
{
    GDK_HINT_MIN_SIZE = 1 << 0,
    GDK_HINT_MAX_SIZE = 1 << 1
}

[Flags]
internal enum GdkEventMask
{
    ButtonPressMask = 1 << 2,
    ButtonReleaseMask = 1 << 3,
    PointerMotionMask = 1 << 6,
}
internal enum FileChooserAction
{
    Open,
    Save,
    SelectFolder,
    CreateFolder
}
internal enum ResponseType
{
    None = -1,
    Accept = -3,
    Cancel = -6
}