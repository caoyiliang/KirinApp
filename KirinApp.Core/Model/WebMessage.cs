using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KirinAppCore.Model;

public class WebMessageEvent : EventArgs
{
    public string Message { get; set; } = "";
}
