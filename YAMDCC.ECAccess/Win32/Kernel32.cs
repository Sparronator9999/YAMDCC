// This file is part of YAMDCC (Yet Another MSI Dragon Center Clone).
// Copyright © Sparronator9999 and Contributors 2023-2025.
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
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace YAMDCC.ECAccess.Win32;

/// <summary>
/// Wraps native Win32 functions from <c>kernel32.dll</c>.
/// </summary>
internal static class Kernel32
{
    /// <summary>
    /// Closes an open handle.
    /// </summary>
    /// <remarks>
    /// See the MSDN documentation for more info:
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/api/handleapi/nf-handleapi-closehandle"/>
    /// </remarks>
    /// <param name="hObject">
    /// An handle to close.
    /// </param>
    /// <returns>
    /// <para><see langword="true"/> if the function succeeds, otherwise <see langword="false"/>.</para>
    /// <para>To get error information, call <see cref="Marshal.GetLastWin32Error"/>.</para>
    /// </returns>
    [DllImport("kernel32.dll",
        ExactSpelling = true, SetLastError = true)]
    internal static extern bool CloseHandle(IntPtr hObject);

    /// <summary>
    /// Open an installed device driver for direct communication.
    /// </summary>
    /// <remarks>
    /// See the MSDN documentation for more info:
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/api/fileapi/nf-fileapi-createfilew"/>
    /// </remarks>
    /// <returns>
    /// <para>
    /// An open handle (as an <see cref="IntPtr"/>) to the specified
    /// device driver if the function succeeds.
    /// </para>
    /// <para>
    /// <see cref="IntPtr.Zero"/> if the function fails. Call
    /// <see cref="Marshal.GetLastWin32Error"/> for error information.
    /// </para>
    /// </returns>
    [DllImport("kernel32.dll",
        CharSet = CharSet.Unicode, ExactSpelling = true,
        EntryPoint = "CreateFileW", SetLastError = true)]
    internal static extern IntPtr CreateFile(
        string lpFileName,
        [MarshalAs(UnmanagedType.U4)] GenericAccessRights dwDesiredAccess,
        [MarshalAs(UnmanagedType.U4)] FileShare dwShareMode,
        [Optional] IntPtr lpSecurityAttributes,
        [MarshalAs(UnmanagedType.U4)] FileMode dwCreationDisposition,
        [MarshalAs(UnmanagedType.U4)] FileAttributes dwFlagsAndAttributes,
        [Optional] IntPtr hTemplateFile);

    /// <summary>
    /// Sends an IO control code directly to a specified device driver.
    /// </summary>
    /// <remarks>
    /// See the MSDN documentation for more info:
    /// <see href="https://learn.microsoft.com/en-us/windows/win32/api/ioapiset/nf-ioapiset-deviceiocontrol"/>
    /// </remarks>
    /// <returns>
    /// <see langword="true"/> if the operation was successful, otherwise <see langword="false"/>.
    /// </returns>
    [DllImport("kernel32.dll",
        ExactSpelling = true, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern unsafe bool DeviceIoControl(
        IntPtr hDevice,
        uint dwIoControlCode,
        [Optional] void* lpInBuffer,
        uint nInBufferSize,
        [Optional] void* lpOutBuffer,
        uint nOutBufferSize,
        [Optional] out uint lpBytesReturned,
        [Optional] NativeOverlapped* lpOverlapped);

    internal enum GenericAccessRights : uint
    {
        None = 0,
        All = 0x10000000,
        Execute = 0x20000000,
        Write = 0x40000000,
        Read = 0x80000000,
    }
}
