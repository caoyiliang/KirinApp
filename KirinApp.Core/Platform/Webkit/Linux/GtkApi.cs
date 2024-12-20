using KirinAppCore.Plateform.Webkit.Linux.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace KirinAppCore.Plateform.Webkit.Linux;

internal class GtkApi
{
    internal const string GtkLib = "/lib/x86_64-linux-gnu/libgtk-3.so.0";
    internal const string GdkLib = "/lib/x86_64-linux-gnu/libgdk-3.so.0";
    internal const string Gioib = "/lib/x86_64-linux-gnu/libgio-2.0.so.0";
    internal const string GObjLb = "/lib/x86_64-linux-gnu/libgobject-2.0.so.0";


    [DllImport(GtkLib, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void XInitThreads();
    [DllImport(GtkLib, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void gtk_init(ref int argc, ref IntPtr argv);
    [DllImport(GtkLib, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr gtk_window_new(int type);
    [DllImport(GtkLib, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void gtk_window_set_title(IntPtr window, string title);
    [DllImport(GtkLib, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void gtk_window_set_resizable(IntPtr window, bool resizable);
    [DllImport(GtkLib, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void gtk_window_set_icon(IntPtr window, IntPtr icon);
    [DllImport(GtkLib, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr gdk_pixbuf_scale_simple(IntPtr pixbuf, int width, int height, GdkInterpType interp_type);
    [DllImport(GtkLib, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void gtk_window_set_decorated(IntPtr window, bool decorated);
    [DllImport(GtkLib, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void gtk_window_set_default_size(IntPtr window, int width, int height);
    [DllImport(GtkLib, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void gtk_widget_override_background_color(IntPtr widget, int state, IntPtr color);
    [DllImport(GtkLib, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void gtk_window_set_position(IntPtr window, int position);
    [DllImport(GtkLib, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void gtk_window_move(IntPtr raw, int x, int y);
    [DllImport(GtkLib, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void gtk_window_set_geometry_hints(IntPtr window, IntPtr geometry_widget, ref GeometryInfo geometry, GdkWindowHints geom_mask);
    [DllImport(GtkLib, CallingConvention = CallingConvention.Cdecl)]
    public static extern void gtk_widget_add_events(IntPtr widget, uint events);
    [DllImport(GtkLib, CallingConvention = CallingConvention.Cdecl)]
    public static extern void g_signal_connect(IntPtr instance, string detailed_signal, IntPtr handler, IntPtr data);
    [DllImport(GtkLib, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void gtk_main();
    [DllImport(GtkLib, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void gtk_widget_show_all(IntPtr widget);
    [DllImport(GtkLib, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void gtk_widget_hide_all(IntPtr widget);
    [DllImport(GtkLib, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void gtk_window_present(IntPtr widget);
    [DllImport(GtkLib, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr gdk_display_get_default();
    [DllImport(GtkLib, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr gdk_display_get_monitor(IntPtr display, int monitor_num);
    [DllImport(GtkLib, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void gdk_monitor_get_geometry(IntPtr monitor, out GdkRectangle rect);
    [DllImport(GtkLib, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void gtk_window_get_size(IntPtr widget, out int width, out int height);
    [DllImport(GtkLib, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void gtk_window_get_position(IntPtr widget, out int x, out int y);
    [DllImport(GtkLib, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void gtk_window_maximize(IntPtr window);
    [DllImport(GtkLib, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void gtk_window_iconify(IntPtr window);
    [DllImport(GtkLib, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint gdk_threads_add_idle(IntPtr function, IntPtr data);

    [DllImport(GtkLib, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr gtk_file_chooser_dialog_new(string title, IntPtr parent, FileChooserAction action, string button1, ResponseType response1, string button2, ResponseType response2, string button3, ResponseType response3, IntPtr nullTerminated);

    [DllImport(GtkLib, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void gtk_file_chooser_set_current_folder(IntPtr chooser, string folder);
    [DllImport(GtkLib, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr gtk_file_filter_new();
    [DllImport(GtkLib, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void gtk_file_filter_add_pattern(IntPtr filter, IntPtr pattern);
    [DllImport(GtkLib, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void gtk_file_filter_set_name(IntPtr filter, IntPtr name);
    [DllImport(GtkLib, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void gtk_file_chooser_add_filter(IntPtr chooser, IntPtr filter);
    [DllImport(GtkLib, CallingConvention = CallingConvention.Cdecl)]
    internal static extern int gtk_dialog_run(IntPtr dialog);
    [DllImport(GtkLib, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr gtk_file_chooser_get_filename(IntPtr chooser);
    [DllImport(GtkLib, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void gtk_widget_destroy(IntPtr widget);
    [DllImport(GtkLib, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void gtk_file_chooser_set_select_multiple(IntPtr chooser, bool select_multiple);
    [DllImport(GtkLib, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void gtk_file_chooser_set_do_overwrite_confirmation(IntPtr chooser, bool doOverwriteConfirmation);
    [DllImport(GtkLib, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr gtk_file_chooser_get_filenames(IntPtr chooser);
    [DllImport(GtkLib, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr gtk_file_chooser_get_current_folder(IntPtr chooser);
    [DllImport(GtkLib, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr gtk_message_dialog_new(IntPtr parent, int flags, int type, int buttons, string message);
    [DllImport(GtkLib, CallingConvention = CallingConvention.Cdecl)]
    internal static extern void gtk_container_add(IntPtr container, IntPtr widget);
    [DllImport(GObjLb, CallingConvention = CallingConvention.Cdecl)]
    internal static extern uint g_signal_connect_data(IntPtr instance, string detailedSignal, IntPtr handler, IntPtr data, IntPtr destroyData, uint connectFlags);
    [DllImport(GObjLb, CallingConvention = CallingConvention.Cdecl)]
    internal static extern IntPtr g_memory_input_stream_new_from_data(IntPtr data, long len, IntPtr destroy);

}
