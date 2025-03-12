// This file is part of YAMDCC (Yet Another MSI Dragon Center Clone).
// Copyright © Sparronator9999 and Contributors 2025.
//
// YAMDCC is free software: you can redistribute it and/or modify it
// under the terms of the GNU General Public License as published by the Free
// Software Foundation, either version 3 of the License, or (at your option)
// any later version.
//
// YAMDCC is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for
// more details.
//
// You should have received a copy of the GNU General Public License along with
// YAMDCC. If not, see <https://www.gnu.org/licenses/>.

using System;
using System.Windows.Forms;

namespace YAMDCC.HotkeyHandler.Config;

public class Hotkey
{
    /// <summary>
    /// The key that must be pressed (along with its modifiers)
    /// to trigger this hotkey's action.
    /// </summary>
    public Keys KeyCode { get; set; }

    /// <summary>
    /// The modifiers that must be pressed to trigger this hotkey's action.
    /// </summary>
    public HotkeyModifiers Modifiers { get; set; }

    /// <summary>
    /// The action to take when the hotkey is pressed.
    /// </summary>
    public HotkeyAction Action { get; set; }

    /// <summary>
    /// Used with <see cref="HotkeyAction.SwitchFanProf"/>, ignored with other actions.
    /// </summary>
    /// <remarks>
    /// The zero-indexed fan profile to switch to when the associated
    /// hotkey is pressed. Set to -1 to cycle through all fan profiles.
    /// </remarks>
    public int ActionData { get; set; }
}

[Flags]
public enum HotkeyModifiers
{
    None = 0,
    Alt = 1,
    Ctrl = 2,
    Shift = 4,
    Windows = 8,
}

public enum HotkeyAction
{
    None,
    OpenConfEditor,
    ToggleFullBlast,
    ToggleWinFnSwap,
    KeyLightUp,
    KeyLightDown,
    SwitchFanProf,
    SwitchPerfMode,
}
