// This file is part of YAMDCC (Yet Another MSI Dragon Center Clone).
// Copyright © Sparronator9999 2023-2024.
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
using System.Security.AccessControl;

namespace YAMDCC.ECAccess
{
    /// <summary>
    /// Contains functions to install and manage kernel-level device drivers.
    /// </summary>
    internal sealed class Driver : IDisposable
    {
        private readonly string DeviceName;
        private readonly string DriverPath = string.Empty;
        private IntPtr hDevice;

        /// <summary>
        /// Gets whether the driver connection is open.
        /// </summary>
        public bool IsOpen { get; private set; }

        /// <summary>
        /// Gets whether the driver is installed to the computer.
        /// </summary>
        /// <remarks>
        /// This will be <c>false</c> if the driver has not been
        /// installed by this instance of the <see cref="Driver"/>,
        /// even if it is actaully installed to the system.
        /// </remarks>
        public bool IsInstalled { get; private set; }

        /// <summary>
        /// The underlying Win32 Error code generated
        /// by the last called method in this class instance.
        /// </summary>
        public int ErrorCode { get; private set; }

        /// <summary>
        /// Create an instance of the <see cref="Driver"/>
        /// class with the specified name and driver path,
        /// automatically installing the driver to the system.
        /// </summary>
        /// <param name="name">
        /// The driver name. This will be used as the device driver service name.
        /// </param>
        /// <param name="path">
        /// The path to the driver file (C:\path\to\driver.sys).
        /// </param>
        public Driver(string name, string path)
        {
            DeviceName = name;
            DriverPath = path;
        }

        /// <summary>
        /// Installs the driver on the local computer.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the driver was installed
        /// successfully, otherwise <c>false</c>.
        /// </returns>
        public bool Install()
        {
            ErrorCode = 0;

            if (string.IsNullOrEmpty(DriverPath))
            {
                throw new ArgumentException(
                    "The driver path is set to a null or empty string.", DriverPath);
            }

            // Make sure the file we're trying to install actually exists:
            string fullPath = Path.GetFullPath(DriverPath);

            if (!File.Exists(fullPath))
            {
                throw new FileNotFoundException($"{fullPath} was not found.", fullPath);
            }

            // Try to open the Service Control Manager:
            IntPtr hSCM = AdvApi32.OpenSCManager(null, null, AdvApi32.SCMAccess.All);
            if (hSCM == IntPtr.Zero)
            {
                // the SCM connection wasn't opened
                // successfully, return false:
                ErrorCode = Marshal.GetLastWin32Error();
                return false;
            }

            // Try to create the service:
            IntPtr hSvc = AdvApi32.CreateService(
                hSCM, DeviceName, DeviceName,
                AdvApi32.ServiceAccess.All,
                AdvApi32.ServiceType.KernelDriver,
                AdvApi32.ServiceStartType.DemandStart,
                AdvApi32.ServiceError.Normal,
                DriverPath, null, null, null, null, null);

            if (hSvc == IntPtr.Zero)
            {
                ErrorCode = Marshal.GetLastWin32Error();
                if (ErrorCode == 1073)  // ERROR_SERVICE_EXISTS
                {
                    hSvc = AdvApi32.OpenService(hSCM, DeviceName, AdvApi32.ServiceAccess.All);
                    if (hSvc == IntPtr.Zero)
                    {
                        ErrorCode = Marshal.GetLastWin32Error();
                        AdvApi32.CloseServiceHandle(hSCM);
                        return false;
                    }
                }
                else
                {
                    ErrorCode = Marshal.GetLastWin32Error();
                    AdvApi32.CloseServiceHandle(hSCM);
                    return false;
                }
            }
            IsInstalled = true;

            // Try to start the service:
            if (!AdvApi32.StartService(hSvc, 0, IntPtr.Zero))
            {
                int error = Marshal.GetLastWin32Error();
                if (error != 1056)  // ERROR_SERVICE_ALREADY_RUNNING
                {
                    ErrorCode = error;
                    AdvApi32.CloseServiceHandle(hSvc);
                    AdvApi32.CloseServiceHandle(hSCM);
                    return false;
                }
            }

            // Perform some cleanup:
            AdvApi32.CloseServiceHandle(hSvc);
            AdvApi32.CloseServiceHandle(hSCM);

            // Security fix for WinRing0 access from unprivileged processes.
            // This fix is present in the WinRing0 driver itself (WinRing0.sys)
            // in an updated fork (https://github.com/GermanAizek/WinRing0), but no
            // public production-signed build of the driver exists with the fixes.
            // This fix was "borrowed" from OpenHardwareMonitor:
            // https://github.com/openhardwaremonitor/openhardwaremonitor/
            FileInfo fi = new($"\\\\.\\{DeviceName}");
            FileSecurity security = fi.GetAccessControl();
            security.SetSecurityDescriptorSddlForm("O:BAG:SYD:(A;;FA;;;SY)(A;;FA;;;BA)");
            fi.SetAccessControl(security);

            return true;
        }

        /// <summary>
        /// Uninstalls the driver from the local computer.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the driver was uninstalled
        /// successfully, otherwise <c>false</c>.
        /// </returns>
        public bool Uninstall()
        {
            ErrorCode = 0;

            // Close the driver file handle (if it's open)
            Close();

            IntPtr hSCM = AdvApi32.OpenSCManager(null, null, AdvApi32.SCMAccess.All);
            if (hSCM == IntPtr.Zero)
            {
                // the SCM connection wasn't opened
                // successfully, return false:
                ErrorCode = Marshal.GetLastWin32Error();
                return false;
            }

            // Try to open the service:
            IntPtr hSvc = AdvApi32.OpenService(hSCM, DeviceName, AdvApi32.ServiceAccess.All);
            if (hSvc == IntPtr.Zero)
            {
                // Ignore ERROR_SERVICE_DOES_NOT_EXIST:
                int error = Marshal.GetLastWin32Error();
                bool success = error == 1060;
                if (success)
                {
                    IsInstalled = false;
                }
                else
                {
                    ErrorCode = error;
                }

                AdvApi32.CloseServiceHandle(hSCM);
                return success;
            }

            // Stop and delete the service:
            AdvApi32.ControlService(hSvc, AdvApi32.ServiceControlCode.Stop, out _);
            AdvApi32.DeleteService(hSvc);
            IsInstalled = false;

            // Close service handles
            AdvApi32.CloseServiceHandle(hSvc);
            AdvApi32.CloseServiceHandle(hSCM);
            return true;
        }

        /// <summary>
        /// Opens a connection to the driver.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the driver connection was
        /// opened successfully, otherwise <c>false</c>.
        /// </returns>
        public bool Open()
        {
            if (IsOpen)
            {
                return true;
            }

            ErrorCode = 0;

            if (hDevice == IntPtr.Zero)
            {
                hDevice = Kernel32.CreateFile(
                    $"\\\\.\\{DeviceName}",
                    Kernel32.GenericAccessRights.Read | Kernel32.GenericAccessRights.Write,
                    FileShare.None,
                    IntPtr.Zero,
                    FileMode.Open,
                    FileAttributes.Normal,
                    IntPtr.Zero);

                // Apparently CreateFileW() can return -1 instead of 0 for some reason
                if (hDevice == IntPtr.Zero || hDevice == new IntPtr(-1))
                {
                    ErrorCode = Marshal.GetLastWin32Error();
                    return false;
                }

                IsOpen = true;
                return true;
            }
            return true;
        }

        /// <summary>
        /// Closes the connection to the device driver, if open.
        /// </summary>
        public void Close()
        {
            if (hDevice != IntPtr.Zero)
            {
                Kernel32.CloseHandle(hDevice);
                hDevice = IntPtr.Zero;
                IsOpen = false;
            }
        }

        public unsafe bool IOControl(uint ctlCode, void* inBuffer, uint inBufSize, void* outBuffer, uint outBufSize, out uint bytesReturned)
        {
            if (hDevice == IntPtr.Zero)
            {
                bytesReturned = 0;
                return false;
            }

            bool success = Kernel32.DeviceIoControl(
                hDevice, ctlCode,
                inBuffer, inBufSize,
                outBuffer, outBufSize,
                out bytesReturned, null);

            ErrorCode = success
                ? 0
                : Marshal.GetLastWin32Error();

            return success;
        }

        public unsafe bool IOControl<T>(uint ctlCode, ref T inBuffer)
            where T : unmanaged
        {
            fixed (T* pBuffer = &inBuffer)
            {
                return IOControl(ctlCode,
                    pBuffer, (uint)sizeof(T),
                    null, 0,
                    out _);
            }
        }

        public unsafe bool IOControl<TIn, TOut>(uint ctlCode, ref TIn inBuffer, out TOut outBuffer)
            where TIn : unmanaged
            where TOut : unmanaged
        {
            int inSize = sizeof(TIn);

            fixed (TIn* pInBuffer = &inBuffer)
            fixed (TOut* pOutBuffer = &outBuffer)
            {
                return IOControl(ctlCode,
                    pInBuffer, (uint)inSize,
                    pOutBuffer, (uint)sizeof(TOut),
                    out _);
            }
        }

        #region Cleanup code
        ~Driver()
        {
            Cleanup();
        }

        /// <summary>
        /// Releases all resources associated with this <see cref="Driver"/>.
        /// </summary>
        public void Dispose()
        {
            Cleanup();
            GC.SuppressFinalize(this);
        }

        private void Cleanup()
        {
            // Don't do anything if we already called Dispose:
            if (!IsOpen)
            {
                return;
            }

            // Close all open file and service handles
            Close();

            IsOpen = false;
        }
        #endregion
    }
}
