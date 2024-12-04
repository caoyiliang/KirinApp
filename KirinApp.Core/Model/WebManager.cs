using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using KirinAppCore.Interface;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebView;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Web.WebView2.Core;
using static System.Net.Mime.MediaTypeNames;

namespace KirinAppCore.Model;

internal class WebManager : WebViewManager
{
    CoreWebView2 webView;
    WebDispatcher dispatcher;
    Task handleMessageTask;
    Channel<string> messageQueue;
    IWindow window;
    IFileProvider fileProvider;
    public WebManager(IWindow window, CoreWebView2 webView, IServiceProvider services, WebDispatcher dispatcher,
       JSComponentConfigurationStore jsComponents, SchemeConfig config)
       : base(services, dispatcher, config.AppOriginUri!, services.GetRequiredService<IFileProvider>(), jsComponents, config.HomePagePath)
    {

        this.window = window;
        this.webView = webView;
        this.dispatcher = dispatcher;
        fileProvider = services.GetRequiredService<IFileProvider>();
        messageQueue = Channel.CreateUnbounded<string>(new UnboundedChannelOptions() { SingleReader = true, SingleWriter = false, AllowSynchronousContinuations = false });
        handleMessageTask = Task.Factory.StartNew(MessageReadProgress, TaskCreationOptions.LongRunning);
    }

    protected override async void NavigateCore(Uri absoluteUri)
    {
        await dispatcher.InvokeAsync(() =>
        {
            webView.Navigate(absoluteUri.ToString());
        });
    }
    protected override void SendMessage(string message)
    {
        messageQueue.Writer.TryWrite(message);
    }
    async Task MessageReadProgress()
    {
        var reader = messageQueue.Reader;
        try
        {
            while (true)
            {
                var message = await reader.ReadAsync();
                await dispatcher.InvokeAsync(() =>
                {
                    var msg = message;
                    window.SendWebMessage(message);
                });
            }
        }
        catch (Exception)
        {
        }
    }

    protected override ValueTask DisposeAsyncCore()
    {
        try
        {
            messageQueue.Writer.Complete();
        }
        catch (Exception)
        {

        }

        handleMessageTask.Wait();
        handleMessageTask.Dispose();

        return base.DisposeAsyncCore();
    }

    #region 给各个平台浏览器 调用
    public void OnMessageReceived(string source, string message)
    {
        MessageReceived(new Uri(source), message);
    }

    public (Stream Content, string Type) OnResourceRequested(SchemeConfig config, string url)
    {
        var filePath = config.AppOriginUri!.RelativeUrl(url, config.HomePage);
        FileInfo fileInfo = new FileInfo(filePath);
        if (fileInfo.Exists)
        {
            return (fileInfo.OpenRead(), filePath.GetContentType().contentType);
        }
        else
        {
            var file = fileProvider.GetFileInfo(filePath);
            if (file.Exists) return (file.CreateReadStream(), filePath.GetContentType().contentType);
            var assembly = Assembly.GetExecutingAssembly();
            var stream = assembly.GetManifestResourceStream("KirinAppCore.wwwroot." + filePath);
            if (stream != null)
                return (stream, filePath.GetContentType().contentType);

            return (new MemoryStream(Encoding.UTF8.GetBytes("no content " + url)), "text/plain");
        }
    }
    #endregion
}
