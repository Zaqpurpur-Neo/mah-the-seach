using System;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace nah_the_search.utils;

public class HotkeyManager {
	[DllImport("user32.dll")]
	static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

	[DllImport("user32.dll")]
	static extern bool UnregisterHotKey(IntPtr hWnd, int id);

	const int HOTKEY_ID = 9000;
	const uint MOD_CONTROL = 0x0002;

	private HwndSource source;
	private IntPtr handle;

	public event Action HotkeyPressed;

	public HotkeyManager(IntPtr windowHandle)
	{
		handle = windowHandle;
		source = HwndSource.FromHwnd(handle);
		source.AddHook(HwndHook);

		RegisterHotKey(handle, HOTKEY_ID, MOD_CONTROL, (uint)32);
	}

	private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
	{
		const int WM_HOTKEY = 0x0312;

		if (msg == WM_HOTKEY && wParam.ToInt32() == HOTKEY_ID)
		{
			HotkeyPressed?.Invoke();
			handled = true;
		}
		return IntPtr.Zero;
	}

	public void Dispose()
	{
		UnregisterHotKey(handle, HOTKEY_ID);
		source.RemoveHook(HwndHook);
	}
}
