using System;
using System.Runtime.InteropServices;
using System.Text;

namespace nah_the_search.utils;

internal static class ShellLinkResolver
{
    [ComImport]
    [Guid("00021401-0000-0000-C000-000000000046")]
    private class CShellLink { }

    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("000214F9-0000-0000-C000-000000000046")]
    private interface IShellLinkW
    {
        void GetPath(StringBuilder pszFile, int cchMaxPath, IntPtr pfd, int fFlags);
        void GetIDList(out IntPtr ppidl);
        void SetIDList(IntPtr pidl);
        void GetDescription(StringBuilder pszName, int cchMaxName);
        void SetDescription(string pszName);
        void GetWorkingDirectory(StringBuilder pszDir, int cchMaxPath);
        void SetWorkingDirectory(string pszDir);
        void GetArguments(StringBuilder pszArgs, int cchMaxPath);
        void SetArguments(string pszArgs);
        void GetHotkey(out short pwHotkey);
        void SetHotkey(short wHotkey);
        void GetShowCmd(out int piShowCmd);
        void SetShowCmd(int iShowCmd);
        void GetIconLocation(StringBuilder pszIconPath, int cchIconPath, out int piIcon);
        void SetIconLocation(string pszIconPath, int iIcon);
        void SetRelativePath(string pszPathRel, int dwReserved);
        void Resolve(IntPtr hwnd, int fFlags);
        void SetPath(string pszFile);
    }

    [ComImport]
    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("0000010B-0000-0000-C000-000000000046")]
    private interface IPersistFile
    {
        void GetClassID(out Guid pClassID);
        void IsDirty();
        void Load([MarshalAs(UnmanagedType.LPWStr)] string pszFileName, uint dwMode);
        void Save(string pszFileName, bool fRemember);
        void SaveCompleted(string pszFileName);
        void GetCurFile(out string ppszFileName);
    }

    public static (string target, string args) Resolve(string shortcutPath)
    {
        var link = (IShellLinkW)new CShellLink();
        ((IPersistFile)link).Load(shortcutPath, 0);

        var target = new StringBuilder(260);
        var args = new StringBuilder(260);

        link.GetPath(target, target.Capacity, IntPtr.Zero, 0);
        link.GetArguments(args, args.Capacity);

        return (target.ToString(), args.ToString());
    }
}

