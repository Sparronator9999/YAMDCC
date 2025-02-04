using System;
using System.Windows.Forms;

namespace YAMDCC.HotkeyHandler;

internal class KeyHookEventArgs : EventArgs
{
    public Keys KeyCode { get; }
    public uint ScanCode { get; }
    public bool ExtendedKey { get; }
    public bool LowerILInjected { get; }
    public bool Injected { get; }

    /// <summary>
    /// Gets or sets a value indicating whether the
    /// key event should be sent to other applications.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the key event should not be sent
    /// to other applications, otherwise <see langword="false"/>.
    /// </returns>
    public bool SuppressKeyPress { get; set; }

    /// <summary>
    /// Initialises a new instance of the <see cref="KeyHookEventArgs"/> class.
    /// </summary>
    /// <param name="keyCode"></param>
    /// <param name="scanCode"></param>
    /// <param name="extendedKey"></param>
    /// <param name="lowerILInjected"></param>
    /// <param name="injected"></param>
    public KeyHookEventArgs(Keys keyCode, uint scanCode, bool extendedKey, bool lowerILInjected, bool injected)
    {
        KeyCode = keyCode;
        ScanCode = scanCode;
        ExtendedKey = extendedKey;
        LowerILInjected = lowerILInjected;
        Injected = injected;
    }
}
