using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using YAMDCC.HotkeyHandler.Win32;

namespace YAMDCC.HotkeyHandler;

internal class KeyHook : IDisposable
{
    private readonly HookProc KbdHookProc;
    private IntPtr KbdHookPtr;

    /// <summary>
    /// Used to determine if we should raise the <see cref="KeyDown"/>
    /// event when <see cref="IgnoreRepeats"/> is <see langword="true"/>.
    /// </summary>
    private readonly HashSet<uint> HeldKeys = [];

    /// <summary>
    /// Gets or sets whether to raise <see cref="KeyDown"/> events when
    /// a key press is automatically repeated due to the key being held down.
    /// </summary>
    public bool IgnoreRepeats { get; set; }

    /// <summary>
    /// Gets or sets whether to raise <see cref="KeyDown"/> or <see cref="KeyUp"/>
    /// events when a modifier key (Ctrl, Shift, or Alt) is pressed.
    /// </summary>
    public bool IgnoreModifiers { get; set; }

    /// <summary>
    /// Gets the currently held modifier keys.
    /// </summary>
    public ConsoleModifiers Modifiers { get; private set; }

    /// <summary>
    /// Occurs when a key is pressed.
    /// </summary>
    public event EventHandler<KeyHookEventArgs> KeyDown;

    /// <summary>
    /// Occurs when a key is released.
    /// </summary>
    public event EventHandler<KeyHookEventArgs> KeyUp;

    public KeyHook(bool ignoreRepeats = true, bool ignoreModifiers = false)
    {
        KbdHookProc = new HookProc(HookProcCallback);
        IgnoreRepeats = ignoreRepeats;
        IgnoreModifiers = ignoreModifiers;
    }

    /// <summary>
    /// Uninstalls the keyboard hook and releases all unmanaged
    /// resources associated with this <see cref="KeyHook"/> instance.
    /// </summary>
    public void Dispose()
    {
        Uninstall();
        GC.SuppressFinalize(this);
    }

    ~KeyHook()
    {
        Uninstall();
    }

    public bool Install()
    {
        if (KbdHookPtr == IntPtr.Zero)
        {
            KbdHookPtr = User32.SetWindowsHookEx(13, KbdHookProc,
                Marshal.GetHINSTANCE(typeof(KeyHook).Module), 0);
            return KbdHookPtr != IntPtr.Zero;
        }
        // keyboard hook already installed
        return true;
    }

    public bool Uninstall()
    {
        // uninstall keyboard hook if installed (KbdHookPtr != IntPtr.Zero),
        // otherwise just return true
        return KbdHookPtr == IntPtr.Zero || User32.UnhookWindowsHookEx(KbdHookPtr);
    }

    private IntPtr HookProcCallback(int nCode, IntPtr wParam, IntPtr lParam)
    {
        if (nCode >= 0 && wParam.ToInt32() is 0x100 or 0x101 or 0x104 or 0x105)
        {
            KbdLLHookStruct kbdStruct = Marshal.PtrToStructure<KbdLLHookStruct>(lParam);
            KeyHookEventArgs e = new(
                kbdStruct.KeyCode,
                kbdStruct.ScanCode,
                (kbdStruct.Flags & 0x01) == 1,
                (kbdStruct.Flags & 0x02) == 1,
                (kbdStruct.Flags & 0x10) == 1);

            switch (wParam.ToInt32())
            {
                case 0x100: // WM_KEYDOWN
                case 0x104: // WM_SYSKEYDOWN
                    if (HandleModifiers(e.KeyCode, true) &&
                        (!IgnoreRepeats || !HeldKeys.Contains(e.ScanCode)))
                    {
                        KeyDown?.Invoke(this, e);
                    }
                    HeldKeys.Add(e.ScanCode);
                    break;
                case 0x101: // WM_KEYUP
                case 0x105: // WM_SYSKEYUP
                    if (HandleModifiers(e.KeyCode, false))
                    {
                        KeyUp?.Invoke(this, e);
                    }
                    HeldKeys.Remove(e.ScanCode);
                    break;
            }

            if (e.SuppressKeyPress)
            {
                return new IntPtr(1);
            }
        }
        return User32.CallNextHookEx(KbdHookPtr, nCode, wParam, lParam);
    }

    private bool HandleModifiers(Keys key, bool pressed)
    {
        ConsoleModifiers modifier;
        switch (key)
        {
            case Keys.LMenu:
            case Keys.RMenu:
                modifier = ConsoleModifiers.Alt;
                break;
            case Keys.LShiftKey:
            case Keys.RShiftKey:
                modifier = ConsoleModifiers.Shift;
                break;
            case Keys.LControlKey:
            case Keys.RControlKey:
                modifier = ConsoleModifiers.Control;
                break;
            default:
                return true;
        }

        if (pressed)
        {
            Modifiers |= modifier;
        }
        else
        {
            Modifiers &= ~modifier;
        }
        return !IgnoreModifiers;
    }
}
