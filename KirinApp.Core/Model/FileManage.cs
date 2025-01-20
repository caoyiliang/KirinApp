using KirinAppCore.Interface;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KirinAppCore.Model;

public static class FileManage
{
    private static IWindow? window
    {
        get
        {
            if (Utils.Service == null) return null;
            return Utils.Service.GetRequiredService<IWindow>();
        }
    }
    public static (bool selected, DirectoryInfo? dir) OpenDirectory(string initialDir = "")
    {
        if (window == null) throw new Exception("KirinApp not initialized");
        return window.OpenDirectory(initialDir);
    }

    public static (bool selected, FileInfo? file) OpenFile(string filePath = "", Dictionary<string, string>? fileTypeFilter = null)
    {
        if (window == null) throw new Exception("KirinApp not initialized");
        return window.OpenFile(filePath, fileTypeFilter);
    }

    public static (bool selected, List<FileInfo>? files) OpenFiles(string filePath = "", Dictionary<string, string>? fileTypeFilter = null)
    {
        if (window == null) throw new Exception("KirinApp not initialized");
        return window.OpenFiles(filePath, fileTypeFilter);
    }
}
