using KirinAppCore.Model;
using KirinAppCore.Plateform.MacOS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace KirinAppCore.Platform.MacOS;

internal class CoreApi
{
    const string AppKit = "/System/Library/Frameworks/AppKit.framework/AppKit";
    [DllImport(AppKit)]
    private static extern IntPtr NSApplicationMain(int argc, string[] argv);

    [DllImport(AppKit)]
    private static extern IntPtr NSWindow_alloc();

    [DllImport(AppKit)]
    private static extern IntPtr NSWindow_initWithContentRect_styleMask_backing_defer(
    IntPtr self, CGRect contentRect, uint styleMask, int backing, bool defer);

    [DllImport(AppKit)]
    private static extern void NSWindow_makeKeyAndOrderFront(IntPtr self, IntPtr sender);

}
