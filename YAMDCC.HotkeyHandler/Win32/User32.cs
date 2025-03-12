using System;
using System.Runtime.InteropServices;

namespace YAMDCC.HotkeyHandler.Win32;

internal static class User32
{
    [DllImport("User32", SetLastError = true, ExactSpelling = true)]
    internal static extern bool RegisterHotKey(
        IntPtr hWnd,
        int id,
        uint fsModifiers,
        uint vk
    );

    [DllImport("User32", SetLastError = true, ExactSpelling = true)]
    internal static extern bool UnregisterHotKey(
        IntPtr hWnd,
        int id
    );
}
