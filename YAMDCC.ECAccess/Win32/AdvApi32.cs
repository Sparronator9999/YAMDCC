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
using System.Runtime.InteropServices;

namespace YAMDCC.ECAccess.Win32
{
    /// <summary>
    /// Wraps native Win32 functions from <c>advapi32.dll</c>.
    /// </summary>
    internal static class AdvApi32
    {
        /// <summary>
        /// Opens an handle with full access to the local computer's
        /// active service control manager (SCM) database.
        /// </summary>
        /// <remarks>
        /// See the MSDN documentation for more info:
        /// <see href="https://learn.microsoft.com/en-us/windows/win32/api/winsvc/nf-winsvc-openscmanagerw"/>
        /// </remarks>
        /// <returns>
        /// <para>An handle (as an <see cref="IntPtr"/>) to the local computer's
        /// active SCM database, if the function succeeds.</para>
        /// <para><see cref="IntPtr.Zero"/> if the function fails. Call
        /// <see cref="Marshal.GetLastWin32Error"/> to get error information.</para>
        /// </returns>
        [DllImport("advapi32.dll",
            CharSet = CharSet.Unicode, ExactSpelling = true,
            EntryPoint = "OpenSCManagerW", SetLastError = true)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        internal static extern IntPtr OpenSCManager(
            string lpMachineName,
            string lpDatabaseName,
            [MarshalAs(UnmanagedType.U4)] SCMAccess dwDesiredAccess);

        /// <summary>
        /// Creates and adds a service to the local computer.
        /// </summary>
        /// <remarks>
        /// See the MSDN documentation for more info:
        /// <see href="https://learn.microsoft.com/en-us/windows/win32/api/winsvc/nf-winsvc-createservicew"/>
        /// </remarks>
        /// <returns>
        /// <para>An handle to the service if the function succeeds.</para>
        /// <para><see cref="IntPtr.Zero"/> if the function fails. Call
        /// <see cref="Marshal.GetLastWin32Error"/> for error information.</para>
        /// </returns>
        [DllImport("advapi32.dll",
        CharSet = CharSet.Unicode, ExactSpelling = true,
            EntryPoint = "CreateServiceW", SetLastError = true)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        internal static extern IntPtr CreateService(
            IntPtr hSCManager,
            string lpServiceName,
            [Optional] string lpDisplayName,
            [MarshalAs(UnmanagedType.U4)] ServiceAccess dwDesiredAccess,
            [MarshalAs(UnmanagedType.U4)] ServiceType dwServiceType,
            [MarshalAs(UnmanagedType.U4)] ServiceStartType dwStartType,
            [MarshalAs(UnmanagedType.U4)] ServiceError dwErrorControl,
            [Optional] string lpBinaryPathName,
            [Optional] string lpLoadOrderGroup,
            [Optional] string lpdwTagId,
            [Optional] string lpDependencies,
            [Optional] string lpServiceStartName,
            [Optional] string lpPassword);

        /// <summary>
        /// Opens an existing service.
        /// </summary>
        /// <remarks>
        /// See the MSDN documentation for more info:
        /// <see href="https://learn.microsoft.com/en-us/windows/win32/api/winsvc/nf-winsvc-openservicew"/>
        /// </remarks>
        /// <returns>
        /// <para>An handle to the service if the function succeeds.</para>
        /// <para><see cref="IntPtr.Zero"/> if the function fails. Call
        /// <see cref="Marshal.GetLastWin32Error"/> for error information.</para>
        /// </returns>
        [DllImport("advapi32.dll",
        CharSet = CharSet.Unicode, ExactSpelling = true,
            EntryPoint = "OpenServiceW", SetLastError = true)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        internal static extern IntPtr OpenService(
            IntPtr hSCManager,
            string lpServiceName,
            [MarshalAs(UnmanagedType.U4)] ServiceAccess dwDesiredAccess);

        /// <summary>
        /// Starts a service.
        /// </summary>
        /// <remarks>
        /// See the MSDN documentation for more info:
        /// <see href="https://learn.microsoft.com/en-us/windows/win32/api/winsvc/nf-winsvc-startservicew"/>
        /// </remarks>
        /// <returns>
        /// <para><c>true</c> if the function succeeds, otherwise <c>false.</c></para>
        /// <para>Call <see cref="Marshal.GetLastWin32Error"/> to get error information.</para>
        /// </returns>
        [DllImport("advapi32.dll",
            CharSet = CharSet.Unicode, ExactSpelling = true,
            EntryPoint = "StartServiceW", SetLastError = true)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool StartService(
            IntPtr hService,
            uint dwNumServiceArgs,
            [Optional] IntPtr lpServiceArgVectors);

        /// <summary>
        /// Sends a control code to a service.
        /// </summary>
        /// <remarks>
        /// See the MSDN documentation for more info:
        /// <see href="https://learn.microsoft.com/en-us/windows/win32/api/winsvc/nf-winsvc-controlservice"/>
        /// </remarks>
        /// <returns>
        /// <para><c>true</c> if the function succeeds, otherwise <c>false.</c></para>
        /// <para>Call <see cref="Marshal.GetLastWin32Error"/> to get error information.</para>
        /// </returns>
        [DllImport("advapi32.dll",
            ExactSpelling = true, SetLastError = true)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool ControlService(
            IntPtr hService,
            uint dwControl,
            out ServiceStatus lpServiceStatus);

        /// <inheritdoc cref="ControlService(IntPtr, uint, out ServiceStatus)"/>
        [DllImport("advapi32.dll",
            ExactSpelling = true, SetLastError = true)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool ControlService(
            IntPtr hService,
            [MarshalAs(UnmanagedType.U4)] ServiceControlCode dwControl,
            out ServiceStatus lpServiceStatus);

        /// <summary>
        /// Deletes a service from the system.
        /// </summary>
        /// <remarks>
        /// See the MSDN documentation for more info:
        /// <see href="https://learn.microsoft.com/en-us/windows/win32/api/winsvc/nf-winsvc-deleteservice"/>
        /// </remarks>
        /// <returns>
        /// <para><c>true</c> if the function succeeds, otherwise <c>false.</c></para>
        /// <para>Call <see cref="Marshal.GetLastWin32Error"/> to get error information.</para>
        /// </returns>
        [DllImport("advapi32.dll",
            ExactSpelling = true, SetLastError = true)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        internal static extern bool DeleteService(IntPtr hService);

        /// <summary>
        /// Closes an handle to a service control manager or service.
        /// </summary>
        /// <remarks>
        /// See the MSDN documentation for more info:
        /// <see href="https://learn.microsoft.com/en-us/windows/win32/api/winsvc/nf-winsvc-closeservicehandle"/>
        /// </remarks>
        /// <returns>
        /// <para><c>true</c> if the function succeeds, otherwise <c>false.</c></para>
        /// <para>To get error information, call <see cref="Marshal.GetLastWin32Error"/>.</para>
        /// </returns>
        [DllImport("advapi32.dll",
            ExactSpelling = true, SetLastError = true)]
        [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
        internal static extern bool CloseServiceHandle(IntPtr hSCObject);

        [StructLayout(LayoutKind.Sequential)]
        internal struct ServiceStatus
        {
            internal ServiceType dwServiceType;
            internal ServiceState dwCurrentState;
            internal uint dwControlsAccepted;
            internal uint dwWin32ExitCode;
            internal uint dwServiceSpecificExitCode;
            internal uint dwCheckPoint;
            internal uint dwWaitHint;
        }

        internal enum ServiceControlCode : uint
        {
            Stop = 0x01,
            Pause = 0x02,
            Continue = 0x03,
            Interrogate = 0x04,
            ParamChange = 0x06,
            NetBindAdd = 0x07,
            NetBindRemove = 0x08,
            NetBindEnable = 0x09,
            NetBindDisable = 0x0A,
        }

        [Flags]
        internal enum SCMAccess : uint
        {
            Connect = 0x0001,
            CreateService = 0x0002,
            EnumerateService = 0x0004,
            Lock = 0x0008,
            QueryLockStatus = 0x0010,
            ModifyBootConfig = 0x0020,
            All = 0xF003F,
        }

        [Flags]
        internal enum ServiceAccess : uint
        {
            QueryConfig = 0x0001,
            ChangeConfig = 0x0002,
            QueryStatus = 0x0004,
            EnumerateDependents = 0x0008,
            Start = 0x0010,
            Stop = 0x0020,
            PauseContinue = 0x0040,
            Interrogate = 0x0080,
            UserDefinedControl = 0x0100,
            Delete = 0x10000,
            ReadControl = 0x20000,
            WriteDac = 0x40000,
            WriteOwner = 0x80000,
            All = 0xF01FF,
        }

        [Flags]
        internal enum ServiceType : uint
        {
            /// <summary>
            /// A kernel-mode driver service.
            /// </summary>
            KernelDriver = 0x00000001,
            /// <summary>
            /// A file system driver service.
            /// </summary>
            FileSystemDriver = 0x00000002,
            /// <summary>
            /// A service that runs in its own process.
            /// </summary>
            Win32OwnProcess = 0x00000010,
            /// <summary>
            /// A service that shares a process with one or more other
            /// services.
            /// </summary>
            /// <remarks>
            /// An example of an executable that spawns multiple services
            /// is Windows' <c>svchost.exe</c>, which hosts many internal
            /// Windows services.
            /// </remarks>
            Win32ShareProcess = 0x00000020,
        }

        internal enum ServiceState : uint
        {
            Stopped = 1U,
            StartPending = 2U,
            StopPending = 3U,
            Running = 4U,
            ContinuePending = 5U,
            PausePending = 6U,
            Paused = 7U,
        }

        internal enum ServiceStartType : uint
        {
            /// <summary>
            /// A device driver started by the system loader.
            /// </summary>
            /// <remarks>
            /// Valid only for driver services.
            /// </remarks>
            BootStart = 0U,
            /// <summary>
            /// A device driver started by the <c>IoInitSystem</c> function.
            /// </summary>
            /// <remarks>
            /// Valid only for driver services.
            /// </remarks>
            SystemStart = 1U,
            /// <summary>
            /// A service started automatically by the Service
            /// Control Manager during system startup.
            /// </summary>
            AutoStart = 2U,
            /// <summary>
            /// A service started by the Service Control Manager when a
            /// process calls the <c>StartService</c> function.
            /// </summary>
            DemandStart = 3U,
            /// <summary>
            /// A service that is disabled.
            /// </summary>
            /// <remarks>
            /// Disabled services cannot be started.
            /// </remarks>
            Disabled = 4U,
        }

        /// <summary>
        /// The action to take if a service fails to start.
        /// </summary>
        internal enum ServiceError : uint
        {
            /// <summary>
            /// The error is ignored and the service continues to
            /// start up.
            /// </summary>
            Ignore = 0U,
            /// <summary>
            /// The error is logged to the event log, but the
            /// service continues to start up.
            /// </summary>
            Normal = 1U,
            /// <summary>
            /// The error is logged to the event log. If the last-known-good
            /// configuration is being started, the service continues to start
            /// up. Otherwise, the service is restarted with the last-known-good
            /// configuration.
            /// </summary>
            Severe = 2U,
            /// <summary>
            /// The error is logged to the event log. If the last-known-good
            /// configuration is being started, the service fails to start.
            /// Otherwise, the service is restarted with the last-known-good
            /// configuration.
            /// </summary>
            Critical = 3U,
        }
    }
}
