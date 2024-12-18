using KirinAppCore.Interface;
using KirinAppCore.Model;
using KirinAppCore.Plateform.Webkit.Linux.Models;
using KirinAppCore.Plateform.WebView2.Windows.Models;
using KirinAppCore.Platform.Webkit.Linux;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebView;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Web.WebView2.Core;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace KirinAppCore.Plateform.Webkit.Linux;

internal class WebKit40 : IWebKit
{
    private const string Lib = "libwebkit2gtk-4.0.so.37";

    #region api
    [DllImport(Lib)]
    internal static extern IntPtr webkit_web_view_new();
    [DllImport(Lib)]
    internal static extern void webkit_web_view_load_uri(IntPtr webView, string uri);
    [DllImport(Lib)]
    internal static extern uint webkit_get_major_version();
    [DllImport(Lib)]
    internal static extern uint webkit_get_minor_version();
    [DllImport(Lib)]
    internal static extern uint webkit_get_micro_version();
    [DllImport(Lib)]
    internal static extern IntPtr webkit_user_script_new(string source, int injectionTime, int injectionFlags, IntPtr whitelist, IntPtr blacklist);
    [DllImport(Lib)]
    internal static extern IntPtr webkit_user_content_manager_new();
    [DllImport(Lib)]
    internal static extern IntPtr webkit_web_view_new_with_user_content_manager(IntPtr manager);
    [DllImport(Lib)]
    internal static extern void webkit_user_content_manager_add_script(IntPtr manager, IntPtr script);
    [DllImport(Lib)]
    internal static extern void webkit_user_script_unref(IntPtr script);
    [DllImport(Lib)]
    internal static extern void webkit_settings_set_enable_developer_extras(IntPtr settings, bool enable);
    [DllImport(Lib)]
    internal static extern IntPtr webkit_web_view_get_settings(IntPtr webView);
    [DllImport(Lib)]
    internal static extern IntPtr webkit_web_context_get_default();
    [DllImport(Lib)]
    internal static extern void webkit_web_context_register_uri_scheme(IntPtr context, string scheme, IntPtr callback, IntPtr user_data, IntPtr destroy_notify);
    [DllImport(Lib)]
    internal static extern IntPtr webkit_uri_scheme_request_get_uri(IntPtr request);
    [DllImport(Lib)]
    internal static extern void webkit_uri_scheme_request_finish(IntPtr request, IntPtr stream, int length, IntPtr mimeType);
    [DllImport(Lib)]
    internal static extern void webkit_user_content_manager_register_script_message_handler(IntPtr manager, string name);
    [DllImport(Lib)]
    internal static extern IntPtr webkit_javascript_result_get_js_value(IntPtr result);
    [DllImport(Lib)]
    internal static extern IntPtr jsc_value_to_string(IntPtr value);
    [DllImport(Lib)]
    internal static extern bool jsc_value_is_string(IntPtr value);
    [DllImport(Lib)]
    internal static extern IntPtr webkit_web_view_run_javascript(IntPtr webView, string script, IntPtr cancellable, IntPtr callback, IntPtr userData);
    [DllImport(Lib)]
    internal static extern void webkit_context_menu_remove_all(IntPtr menu);
    [DllImport(Lib)]
    private static extern void webkit_web_view_open_devtools(IntPtr webView);
    [DllImport(Lib)]
    private static extern void webkit_web_view_reload(IntPtr webView);
    #endregion


    public event EventHandler<WebMessageEvent>? WebMessageReceived;
    private IntPtr Handle { get; set; }
    private WebManager? WebManager { get; set; }
    private SchemeConfig? SchemeConfig { get; set; }
    private IWindow? Window { get; set; }
    private delegate void ContextMenuCallbackDelegate(IntPtr webView, IntPtr menu, IntPtr userData); private static void ContextMenuCallback(IntPtr webView, IntPtr menu, IntPtr userData)
    {
        webkit_context_menu_remove_all(menu);
    }
    private delegate IntPtr UriSchemeCallbackFunc(IntPtr request, IntPtr user_data);
    private delegate void ScriptMessageReceivedDelegate(IntPtr webView, IntPtr message, IntPtr userData);
    public async Task InitWebControl(IWindow window, WinConfig config)
    {
        Window = window;
        await Task.Delay(1);
        var contentManager = webkit_user_content_manager_new();
        Handle = webkit_web_view_new_with_user_content_manager(contentManager);
        GtkApi.gtk_container_add(window.Handle, Handle);

        if (config.Debug)
        {
            IntPtr settings = webkit_web_view_get_settings(Handle);
            webkit_settings_set_enable_developer_extras(settings, true);
        }
        else
        {
            IntPtr settings = webkit_web_view_get_settings(Handle);
            webkit_settings_set_enable_developer_extras(settings, false);
            GtkApi.g_signal_connect_data(Handle, "context-menu", Marshal.GetFunctionPointerForDelegate(new ContextMenuCallbackDelegate(ContextMenuCallback)), IntPtr.Zero, IntPtr.Zero, 0);
        }

        if (config.AppType != WebAppType.Http)
        {
            var url = $"http://localhost/";
            if (config.AppType == WebAppType.Static) url += config.Url;
            if (config.AppType == WebAppType.Blazor) url += "blazorindex.html";
            SchemeConfig = new Uri(url).ParseScheme();
            var dispatcher = new WebDispatcher(window);
            WebManager = new WebManager(window, dispatcher, window.ServiceProvide!.GetRequiredService<JSComponentConfigurationStore>(), SchemeConfig);
            if (config.AppType == WebAppType.Blazor)
            {
                if (config.BlazorComponent == null) throw new Exception("Blazor component not found!");
                _ = dispatcher.InvokeAsync(async () =>
                {
                    await WebManager.AddRootComponentAsync(config.BlazorComponent!, config.BlazorSelector,
                        ParameterView.Empty);
                });
            }

            UriSchemeCallbackFunc uriSchemeCallback = UriSchemeCallback;
            var context = webkit_web_context_get_default();
            webkit_web_context_register_uri_scheme(context, SchemeConfig.AppScheme, Marshal.GetFunctionPointerForDelegate(uriSchemeCallback), IntPtr.Zero, IntPtr.Zero);


            var assembly = Assembly.GetExecutingAssembly();
            var stream = assembly.GetManifestResourceStream("KirinAppCore.wwwroot.webkit.document.js")!;
            var content = new StreamReader(stream).ReadToEnd();
            var script = webkit_user_script_new(content, 2, 0, IntPtr.Zero, IntPtr.Zero);
            webkit_user_content_manager_add_script(contentManager, script);
            webkit_user_script_unref(script);

            GtkApi.g_signal_connect_data(contentManager,
                "script-message-received::KirinApp",
                Marshal.GetFunctionPointerForDelegate(new ScriptMessageReceivedDelegate(ScriptMessageReceived)),
                IntPtr.Zero, IntPtr.Zero, 0);
            webkit_user_content_manager_register_script_message_handler(contentManager, "KirinApp");
            WebManager.Navigate("/");
        }
        else
        {
            webkit_web_view_load_uri(Handle, config.Url!);
        }
    }

    public void ExecuteJavaScript(string js)
    {
        webkit_web_view_run_javascript(Handle, js, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
    }

    public string ExecuteJavaScriptWithResult(string js)
    {
        IntPtr jsHandle = webkit_web_view_run_javascript(Handle, js, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
        if (jsHandle == IntPtr.Zero)
        {
            throw new Exception("JavaScript execution failed.");
        }

        IntPtr res = webkit_javascript_result_get_js_value(jsHandle);
        if (jsc_value_is_string(res))
        {
            IntPtr jsString = jsc_value_to_string(res);
            return Marshal.PtrToStringAnsi(jsString) ?? throw new Exception("JavaScript execution failed.");
        }
        throw new Exception("JavaScript execution failed.");
    }

    public void OpenDevTool()
    {
        webkit_web_view_open_devtools(Handle);
    }

    public void Reload()
    {
        webkit_web_view_reload(Handle);
    }

    public void Navigate(string url)
    {
        webkit_web_view_load_uri(Handle, url);
    }

    public void SendWebMessage(string message)
    {
        var js = new StringBuilder();
        js.Append("__dispatchMessageCallback(\"");

        js.Append(message);
        js.Append("\")");
        webkit_web_view_run_javascript(Handle, js.ToString(), IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
    }


    private IntPtr UriSchemeCallback(IntPtr request, IntPtr user_data)
    {
        var uri = Marshal.PtrToStringAnsi(webkit_uri_scheme_request_get_uri(request));
        var response = WebManager!.OnResourceRequested(SchemeConfig!, uri!);

        using MemoryStream ms = new();
        response.Content.CopyTo(ms);
        var bytes = ms.ToArray();
        IntPtr bytesPtr = Marshal.AllocHGlobal(bytes.Length);
        Marshal.Copy(bytes, 0, bytesPtr, bytes.Length);
        var stream = GtkApi.g_memory_input_stream_new_from_data(bytesPtr, bytes.Length, IntPtr.Zero);
        webkit_uri_scheme_request_finish(request, stream, bytes.Length, Marshal.StringToCoTaskMemAnsi(response.Type));

        return IntPtr.Zero;
    }

    private void ScriptMessageReceived(IntPtr webView, IntPtr message, IntPtr userData)
    {
        IntPtr jsResult = webkit_javascript_result_get_js_value(message);
        if (jsc_value_is_string(jsResult))
        {
            IntPtr jsString = jsc_value_to_string(jsResult);
            string messageString = Marshal.PtrToStringAnsi(jsString)!;
            WebManager!.OnMessageReceived(SchemeConfig!.AppOrigin, messageString);
            try
            {
                var jobject = JObject.Parse(messageString);
                if (jobject.ContainsKey("cmd"))
                {
                    var cmd = jobject["cmd"]!.ToString();
                    switch (cmd)
                    {
                        case "max": Window!.Maximize(); break;
                        case "min": Window!.Minimize(); break;
                        case "hide": Window!.Hide(); break;
                        case "show": Window!.Show(); break;
                        case "focus": Window!.Focus(); break;
                        case "close": Window!.Close(); break;
                        default:
                            break;
                    }
                    Window!.Maximize();
                }
                return;
            }
            catch { }
            WebMessageEvent msg = new WebMessageEvent()
            {
                Message = messageString
            };
            WebMessageReceived?.Invoke(this, msg);
        }
    }
}
internal class WebKit41 : IWebKit
{
    private const string Libraries = "libwebkit2gtk-4.1.so.0";
    #region api

    #endregion

    public event EventHandler<WebMessageEvent>? WebMessageReceived;
    private IntPtr Handle { get; set; }
    private WebManager? WebManager { get; set; }
    private SchemeConfig? SchemeConfig { get; set; }
    private IWindow? Window { get; set; }

    public Task InitWebControl(IWindow window, WinConfig config)
    {
        throw new NotImplementedException();
    }

    public void ExecuteJavaScript(string js)
    {
        throw new NotImplementedException();
    }

    public string ExecuteJavaScriptWithResult(string js)
    {
        throw new NotImplementedException();
    }

    public void OpenDevTool()
    {
        throw new NotImplementedException();
    }

    public void Reload()
    {
        throw new NotImplementedException();
    }

    public void SendWebMessage(string message)
    {
        throw new NotImplementedException();
    }

    public void Navigate(string url)
    {
        throw new NotImplementedException();
    }
}
