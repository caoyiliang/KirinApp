using KirinAppCore.Interface;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KirinAppCore.Model;

public static class DialogManage
{
    private static IWindow? window
    {
        get
        {
            if (Utils.Service == null) return null;
            return Utils.Service.GetRequiredService<IWindow>();
        }
    }

    public static MsgResult ShowDialog(string title, string message, MsgBtns btns = MsgBtns.OK)
    {
        if (window == null) throw new Exception("KirinApp not initialized");
        return window.ShowDialog(title, message, btns);
    }
}
