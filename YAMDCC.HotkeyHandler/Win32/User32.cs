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
using System.Runtime.InteropServices;

namespace YAMDCC.HotkeyHandler.Win32;

internal static class User32
{
    [DllImport("User32")]
    internal static extern bool SetForegroundWindow(IntPtr hWnd);

    [DllImport("User32")]
    internal static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    [DllImport("User32", SetLastError = true, ExactSpelling = true)]
    internal static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("User32", SetLastError = true, ExactSpelling = true)]
    internal static extern bool UnregisterHotKey(IntPtr hWnd, int id);
}
