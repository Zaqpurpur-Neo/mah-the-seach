using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace nah_the_search.utils;

public class TrayIconManager : IDisposable
{
    const int WM_USER = 0x0400;
    const int WM_TRAY = WM_USER + 1;

    const int WM_COMMAND = 0x0111;

    const int NIM_ADD = 0x0;
    const int NIM_DELETE = 0x2;

    const int NIF_MESSAGE = 0x1;
    const int NIF_ICON = 0x2;
    const int NIF_TIP = 0x4;

    const int MF_STRING = 0x0;

    const int ID_SHOW = 1;
    const int ID_EXIT = 2;

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
    struct NOTIFYICONDATA
    {
        public int cbSize;
        public IntPtr hWnd;
        public int uID;
        public int uFlags;
        public int uCallbackMessage;
        public IntPtr hIcon;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
        public string szTip;
    }

    [DllImport("shell32.dll")]
    static extern bool Shell_NotifyIcon(int dwMessage, ref NOTIFYICONDATA lpData);

    [DllImport("user32.dll")]
    static extern IntPtr LoadIcon(IntPtr hInstance, IntPtr lpIconName);

    [DllImport("user32.dll")]
    static extern IntPtr CreatePopupMenu();

    [DllImport("user32.dll")]
    static extern bool AppendMenu(IntPtr hMenu, int flags, int id, string text);

    [DllImport("user32.dll")]
    static extern bool SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    static extern bool TrackPopupMenu(
        IntPtr hMenu,
        uint flags,
        int x,
        int y,
        int reserved,
        IntPtr hWnd,
        IntPtr rect);

    [DllImport("user32.dll")]
    static extern bool GetCursorPos(out POINT pt);

    struct POINT { public int X; public int Y; }

    private NOTIFYICONDATA data;
    private HwndSource source;
    private IntPtr menu;
    private Window window;

    public event Action ShowRequested;
    public event Action ExitRequested;

    public TrayIconManager(Window window, string tooltip)
    {
        this.window = window;

        var helper = new WindowInteropHelper(window);

        source = HwndSource.FromHwnd(helper.Handle);
        source.AddHook(WndProc);

        IntPtr iconHandle = LoadIcon(IntPtr.Zero, (IntPtr)0x7F00);

        data = new NOTIFYICONDATA();
        data.cbSize = Marshal.SizeOf(data);
        data.hWnd = helper.Handle;
        data.uID = 1;
        data.uFlags = NIF_MESSAGE | NIF_ICON | NIF_TIP;
        data.uCallbackMessage = WM_TRAY;
        data.hIcon = iconHandle;
        data.szTip = tooltip;

        Shell_NotifyIcon(NIM_ADD, ref data);

        BuildMenu();
    }

    private void BuildMenu()
    {
        menu = CreatePopupMenu();
        AppendMenu(menu, MF_STRING, ID_SHOW, "Show");
        AppendMenu(menu, MF_STRING, ID_EXIT, "Exit");
    }

    private void ShowMenu()
    {
        GetCursorPos(out POINT pt);
        SetForegroundWindow(data.hWnd);
        TrackPopupMenu(menu, 0, pt.X, pt.Y, 0, data.hWnd, IntPtr.Zero);
    }

    private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (msg == WM_TRAY)
        {
            int mouseMsg = lParam.ToInt32();

            if (mouseMsg == 0x0202) // left up
                ShowRequested?.Invoke();

            if (mouseMsg == 0x0205) // right up
                ShowMenu();
        }

        if (msg == WM_COMMAND)
        {
            int id = wParam.ToInt32();

            if (id == ID_SHOW)
                ShowRequested?.Invoke();

            if (id == ID_EXIT)
                ExitRequested?.Invoke();
        }

        return IntPtr.Zero;
    }

    public void Dispose()
    {
        Shell_NotifyIcon(NIM_DELETE, ref data);
        source.RemoveHook(WndProc);
    }
}

