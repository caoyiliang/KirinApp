using KirinAppCore.Interface;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KirinAppCore.Model;

internal class WebDispatcher : Dispatcher
{
    IWindow window;
    public WebDispatcher(IWindow window)
    {
        this.window = window;
    }
    public override bool CheckAccess() => window.CheckAccess();

    public override async Task InvokeAsync(Action workItem)
    {
        try
        {
            window.Invoke(workItem);
        }
        catch (Exception)
        {
            throw;
        }
        await Task.CompletedTask;
    }

    public override async Task InvokeAsync(Func<Task> workItem)
    {
        try
        {
            await window.InvokeAsync(workItem);
        }
        catch (Exception)
        {
            throw;
        }
    }
    public override async Task<TResult> InvokeAsync<TResult>(Func<TResult> workItem)
    {
        return await InvokeAsync(workItem);
    }

    public override async Task<TResult> InvokeAsync<TResult>(Func<Task<TResult>> workItem)
    {
        return await InvokeAsync(workItem);
    }
}
