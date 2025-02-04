using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace YAMDCC.HotkeyHandler.Win32;

internal delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);

internal static class User32
{
    [DllImport("user32.dll", SetLastError = true, ExactSpelling = true,
        CharSet = CharSet.Unicode, EntryPoint = "SetWindowsHookExW")]
    internal static extern IntPtr SetWindowsHookEx(
        int idHook,
        HookProc lpfn,
        IntPtr hmod,
        uint threadId
    );

    [DllImport("user32.dll", SetLastError = true, ExactSpelling = true)]
    internal static extern bool UnhookWindowsHookEx(IntPtr hhk);

    [DllImport("user32.dll", SetLastError = true, ExactSpelling = true)]
    internal static extern IntPtr CallNextHookEx(
        IntPtr hhk,
        int nCode,
        IntPtr wParam,
        IntPtr lParam
    );
}

internal struct KbdLLHookStruct
{
#pragma warning disable CS0649  // CS0649: Fields are never assigned to (they are by Marshal.PtrToStructure)
    public Keys KeyCode;
    public uint ScanCode;
    public uint Flags;
    public uint Time;
    public UIntPtr ExtraInfo;
#pragma warning restore CS0649
}
