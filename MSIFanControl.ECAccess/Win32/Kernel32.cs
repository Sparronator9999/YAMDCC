// This file is part of MSI Fan Control.
// Copyright © Sparronator9999 2023-2024.
//
// MSI Fan Control is free software: you can redistribute it and/or modify it
// under the terms of the GNU General Public License as published by the Free
// Software Foundation, either version 3 of the License, or (at your option)
// any later version.
//
// MSI Fan Control is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for
// more details.
//
// You should have received a copy of the GNU General Public License along with
// MSI Fan Control. If not, see <https://www.gnu.org/licenses/>.

using System;
using System.IO;
using System.Runtime.InteropServices;

namespace MSIFanControl.ECAccess
{
    /// <summary>
    /// Contains native Windows OS functions.
    /// </summary>
    internal static class Kernel32
    {
        /// <summary>
        /// Closes an open handle.
        /// </summary>
        /// <param name="hObject">An handle to close.</param>
        /// <returns>
        /// <para><c>true</c> if the function succeeds, otherwise <c>false</c>.</para>
        /// <para>To get error information, call <see cref="Marshal.GetLastWin32Error"/>.</para>
        /// </returns>
        [DllImport("kernel32.dll",
            ExactSpelling = true, SetLastError = true)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        public static extern bool CloseHandle(IntPtr hObject);

        /// <summary>
        /// Open an installed device driver for direct communication.
        /// </summary>
        /// <param name="serviceName">
        /// <para>The name of the device driver.</para>
        /// </param>
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
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        public static extern IntPtr CreateFile(
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
        /// <param name="hDevice">
        /// The handle to the device on which the operation is to be performed.
        /// </param>
        /// <param name="dwIoControlCode">
        /// The control code for the operation.
        /// </param>
        /// <param name="lpInBuffer">
        /// The input buffer to pass to the device driver.
        /// </param>
        /// <param name="lpOutBuffer">
        /// The output buffer to be returned by the driver.
        /// </param>
        /// <returns>
        /// <c>true</c> if the operation was successful, otherwise <c>false</c>.
        /// </returns>
        [DllImport("kernel32.dll",
        ExactSpelling = true, SetLastError = true)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeviceIoControl(
            IntPtr hDevice,
            uint dwIoControlCode,
            [Optional] ref byte lpInBuffer,
            uint nInBufferSize,
            [Optional] ref byte lpOutBuffer,
            uint nOutBufferSize,
            [Optional] out uint lpBytesReturned,
            [Optional] IntPtr lpOverlapped);

        /// <inheritdoc cref="DeviceIoControl(IntPtr, uint, ref byte, ref byte)"/>
        [DllImport("kernel32.dll",
            ExactSpelling = true, SetLastError = true)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeviceIoControl(
            IntPtr hDevice,
            uint dwIoControlCode,
            [Optional] ref ushort lpInBuffer,
            uint nInBufferSize,
            [Optional] ref ushort lpOutBuffer,
            uint nOutBufferSize,
            [Optional] out uint lpBytesReturned,
            [Optional] IntPtr lpOverlapped);

        /// <inheritdoc cref="DeviceIoControl(IntPtr, uint, ref byte, ref byte)"/>
        [DllImport("kernel32.dll",
            ExactSpelling = true, SetLastError = true)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeviceIoControl(
            IntPtr hDevice,
            uint dwIoControlCode,
            [Optional] ref uint lpInBuffer,
            uint nInBufferSize,
            [Optional] ref uint lpOutBuffer,
            uint nOutBufferSize,
            [Optional] out uint lpBytesReturned,
            [Optional] IntPtr lpOverlapped);


        /// <inheritdoc cref="DeviceIoControl(IntPtr, uint, ref byte, ref byte)"/>
        [DllImport("kernel32.dll",
            ExactSpelling = true, SetLastError = true)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeviceIoControl(
            IntPtr hDevice,
            uint dwIoControlCode,
            [MarshalAs(UnmanagedType.AsAny)][Optional] object lpInBuffer,
            uint nInBufferSize,
            [MarshalAs(UnmanagedType.AsAny)][Optional] object lpOutBuffer,
            uint nOutBufferSize,
            [Optional] out uint lpBytesReturned,
            [Optional] IntPtr lpOverlapped);

        public enum GenericAccessRights : uint
        {
            None = 0,
            All = 0x10000000,
            Execute = 0x20000000,
            Write = 0x40000000,
            Read = 0x80000000,
        }
    }
}
