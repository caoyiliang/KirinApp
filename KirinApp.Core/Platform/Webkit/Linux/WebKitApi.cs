using KirinAppCore.Interface;
using KirinAppCore.Model;
using KirinAppCore.Platform.Webkit.Linux;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace KirinAppCore.Plateform.Webkit.Linux;

internal class WebKit40 : IWebKit
{
    private const string Lib = "libwebkit2gtk-4.0.so.37";

    #region api

    [DllImport(Lib)]
    internal static extern void webkit_web_view_load_uri(IntPtr webView, string uri);

    [DllImport(Lib)]
    internal static extern IntPtr webkit_user_script_new(string source, int injectionTime, int injectionFlags,
        IntPtr whitelist, IntPtr blacklist);

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
    internal static extern void webkit_web_context_register_uri_scheme(IntPtr context, string scheme, IntPtr callback,
        IntPtr user_data, IntPtr destroy_notify);

    [DllImport(Lib)]
    internal static extern IntPtr webkit_uri_scheme_request_get_uri(IntPtr request);

    [DllImport(Lib)]
    internal static extern void webkit_uri_scheme_request_finish(IntPtr request, IntPtr stream, int length,
        IntPtr mimeType);

    [DllImport(Lib)]
    internal static extern void
        webkit_user_content_manager_register_script_message_handler(IntPtr manager, string name);

    [DllImport(Lib)]
    internal static extern IntPtr webkit_javascript_result_get_js_value(IntPtr result);

    [DllImport(Lib)]
    internal static extern IntPtr jsc_value_to_string(IntPtr value);

    [DllImport(Lib)]
    internal static extern bool jsc_value_is_string(IntPtr value);

    [DllImport(Lib)]
    internal static extern IntPtr webkit_web_view_run_javascript(IntPtr webView, string script, IntPtr cancellable,
        IntPtr callback, IntPtr userData);

    [DllImport(Lib)]
    internal static extern void webkit_context_menu_remove_all(IntPtr menu);

    #endregion


    private IntPtr Handle { get; set; }
    private WebManager? WebManager { get; set; }
    private SchemeConfig? SchemeConfig { get; set; }
    private IWindow? Window { get; set; }
    private WinConfig Config => Window!.Config;
    public event EventHandler<WebMessageEvent>? WebMessageReceived;

    private delegate void ContextMenuCallbackDelegate(IntPtr webView, IntPtr menu, IntPtr userData);

    private delegate IntPtr UriSchemeCallbackFunc(IntPtr request, IntPtr user_data);

    private UriSchemeCallbackFunc uriSchemeCallback = new((_, _) => 0);

    private delegate void ScriptMessageReceivedDelegate(IntPtr webView, IntPtr message, IntPtr userData);

    public void InitWebControl(IWindow? window = null)
    {
        try
        {
            if (window != null)
                Window = window;

            var contentManager = webkit_user_content_manager_new();
            Handle = webkit_web_view_new_with_user_content_manager(contentManager);
            GtkApi.gtk_container_add(Window!.Handle, Handle);

            if (Config.Debug)
            {
                IntPtr settings = webkit_web_view_get_settings(Handle);
                webkit_settings_set_enable_developer_extras(settings, true);
            }
            else
            {
                IntPtr settings = webkit_web_view_get_settings(Handle);
                webkit_settings_set_enable_developer_extras(settings, false);
                GtkApi.g_signal_connect_data(Handle, "context-menu",
                    Marshal.GetFunctionPointerForDelegate(new ContextMenuCallbackDelegate(ContextMenuCallback)),
                    IntPtr.Zero, IntPtr.Zero, 0);
            }

            if (Config.AppType != WebAppType.Http)
            {
                var url = $"http://localhost/";
                if (Config.AppType == WebAppType.Static) url += Config.Url;
                if (Config.AppType == WebAppType.Blazor) url += "blazorindex.html";
                SchemeConfig = new Uri(url).ParseScheme();
                var dispatcher = new WebDispatcher(window!);
                WebManager = new WebManager(window!, dispatcher,
                    window!.ServiceProvide!.GetRequiredService<JSComponentConfigurationStore>(), SchemeConfig);
                if (Config.AppType == WebAppType.Blazor)
                {
                    if (Config.BlazorComponent == null) throw new Exception("Blazor component not found!");
                    _ = dispatcher.InvokeAsync(async () =>
                    {
                        await WebManager.AddRootComponentAsync(Config.BlazorComponent!, Config.BlazorSelector,
                            ParameterView.Empty);
                    });
                }

                uriSchemeCallback = UriSchemeCallback;
                var context = webkit_web_context_get_default();
                webkit_web_context_register_uri_scheme(context, SchemeConfig.AppScheme,
                    Marshal.GetFunctionPointerForDelegate(uriSchemeCallback), IntPtr.Zero, IntPtr.Zero);

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
                webkit_web_view_load_uri(Handle, Config.Url!);
            }
        }
        catch (Exception e)
        {
            System.Console.WriteLine(e.Message);
            throw;
        }
    }

    public string ExecuteJavaScript(string js)
    {
        var jsHandle = webkit_web_view_run_javascript(Handle, js, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
        if (jsHandle == IntPtr.Zero) return "";
        var res = webkit_javascript_result_get_js_value(jsHandle);
        if (!jsc_value_is_string(res)) return "";
        var jsString = jsc_value_to_string(res);
        var result = Marshal.PtrToStringAnsi(jsString);
        return result ?? "";
    }

    public void InjectJsObject(string name, object obj)
    {
        var js = $"window.external.{name}={JsonConvert.SerializeObject(obj)}";
        webkit_web_view_run_javascript(Handle, js, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
    }

    public void OpenDevTool()
    {
        return;
    }

    public void Reload()
    {
        InitWebControl();
    }

    public void Navigate(string url)
    {
        webkit_web_view_load_uri(Handle, url);
    }

    public void SendWebMessage(string message)
    {
        var js = new StringBuilder();
        js.Append("window.__dispatchMessageCallback(\"");
        js.Append(message.Replace(@"\", @"\\").Replace("'", @"\'").Replace("\"", "\\\""));
        js.Append("\")");
        webkit_web_view_run_javascript(Handle, js.ToString(), IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
    }

    private IntPtr UriSchemeCallback(IntPtr request, IntPtr user_data)
    {
        try
        {
            var contentType = "text/html; charset=UTF-8";
            byte[] byteArray;
            if (Window!.Config.AppType == WebAppType.RawString)
            {
                string pattern = @"<(\w+)([^>]*?)>(.*?)<\/\1>|<(\w+)([^>]*?)/>";
                if (!Regex.IsMatch(Window!.Config.RawString ?? "", pattern, RegexOptions.Singleline))
                    contentType = "text/plain; charset=UTF-8";
                byteArray = Encoding.UTF8.GetBytes(Window!.Config.RawString ?? "");
            }
            else
            {
                MemoryStream ms = new();
                var uri = Marshal.PtrToStringAnsi(webkit_uri_scheme_request_get_uri(request));
                var response = WebManager!.OnResourceRequested(SchemeConfig!, uri!);
                response.Content.CopyTo(ms);
                byteArray = ms.ToArray();
                contentType = response.Type;
            }

            IntPtr bytesPtr = Marshal.AllocHGlobal(byteArray.Length);
            Marshal.Copy(byteArray, 0, bytesPtr, byteArray.Length);
            var stream = GtkApi.g_memory_input_stream_new_from_data(bytesPtr, byteArray.Length, IntPtr.Zero);
            webkit_uri_scheme_request_finish(request, stream, byteArray.Length,
                Marshal.StringToCoTaskMemAnsi(contentType));
            return IntPtr.Zero;
        }
        catch (Exception e)
        {
            System.Console.WriteLine(e.Message);
            throw;
        }
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
                        case "change":
                            Window!.Change(jobject["data"]!["width"]!.ToString().ToInt(),
                                jobject["data"]!["height"]!.ToString().ToInt()); break;
                        case "normal": Window!.Normal(); break;
                        case "move":
                            Window!.Move(jobject["data"]!["x"]!.ToString().ToInt(),
                                jobject["data"]!["y"]!.ToString().ToInt()); break;
                        case "moveTo":
                            Window!.MoveTo(jobject["data"]!["x"]!.ToString().ToInt(),
                                jobject["data"]!["y"]!.ToString().ToInt()); break;
                        default:
                            break;
                    }
                }

                return;
            }
            catch
            {
            }

            WebMessageEvent msg = new WebMessageEvent()
            {
                Message = messageString
            };
            WebMessageReceived?.Invoke(this, msg);
        }
    }

    private static void ContextMenuCallback(IntPtr webView, IntPtr menu, IntPtr userData)
    {
        webkit_context_menu_remove_all(menu);
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

    public void InitWebControl(IWindow window)
    {
        throw new NotImplementedException();
    }

    public string ExecuteJavaScript(string js)
    {
        return "";
    }

    public void InjectJsObject(string name, object obj)
    {
        return;
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