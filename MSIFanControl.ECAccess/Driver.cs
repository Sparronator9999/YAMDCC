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
using System.Security.AccessControl;

namespace MSIFanControl.ECAccess
{
    /// <summary>
    /// Contains functions to install and manage kernel-level device drivers.
    /// </summary>
    internal class Driver : IDisposable
    {
        private readonly string DriverName;
        private readonly string DriverPath;
        private IntPtr hDevice;

        private DriverStatus _status;
        private int _error;
        private bool Disposed = false;

        /// <summary>
        /// The current status of this <see cref="Driver"/> instance.
        /// </summary>
        public DriverStatus Status
        {
            get => _status;
            private set => _status = value;
        }

        /// <summary>
        /// The underlying Win32 Error code generated
        /// by the last called method in this class instance.
        /// </summary>
        public int ErrorCode
        {
            get => _error;
            private set => _error = value;
        }

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
            DriverName = name;
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

            // Make sure the file we're trying to install actually exists:
            try
            {
                string fullPath = Path.GetFullPath(DriverPath);
            }
            catch (ArgumentException)
            {
                Status |= DriverStatus.FileNotFound;
                return false;
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
                hSCM, DriverName, DriverName,
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
                    hSvc = AdvApi32.OpenService(hSCM, DriverName, AdvApi32.ServiceAccess.All);
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
            Status |= DriverStatus.Installed;

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
            // in the updated fork (https://github.com/GermanAizek/WinRing0), but
            // no public production-signed build of the driver exists with the fixes.
            FileInfo fi = new FileInfo($"\\\\.\\{DriverName}");
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
            IntPtr hSvc = AdvApi32.OpenService(hSCM, DriverName, AdvApi32.ServiceAccess.All);
            if (hSvc == IntPtr.Zero)
            {
                // Ignore ERROR_SERVICE_DOES_NOT_EXIST:
                int error = Marshal.GetLastWin32Error();
                bool success = error == 1060;
                if (success)
                {
                    Status &= ~DriverStatus.Installed;
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
            Status &= ~DriverStatus.Installed;

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
            ErrorCode = 0;

            if (hDevice == IntPtr.Zero)
            {
                hDevice = Kernel32.CreateFile(
                    $"\\\\.\\{DriverName}",
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


                Status |= DriverStatus.Open;
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
                Status &= ~DriverStatus.Open;
            }
        }

        public bool IOControl(uint ctlCode, object inBuffer, object outBuffer)
        {
            if (hDevice == IntPtr.Zero)
            {
                return false;
            }

            bool success = Kernel32.DeviceIoControl(
                hDevice, ctlCode,
                inBuffer, inBuffer is null ? 0 : (uint)Marshal.SizeOf(inBuffer),
                outBuffer, outBuffer is null ? 0 : (uint)Marshal.SizeOf(outBuffer),
                out _, IntPtr.Zero);
            if (!success)
            {
                ErrorCode = Marshal.GetLastWin32Error();
            }
            return success;
        }


        public bool IOControl(uint ctlCode, ref byte inBuffer, out byte outBuffer)
        {
            outBuffer = 0;
            if (hDevice == IntPtr.Zero)
            {
                return false;
            }

            bool success = Kernel32.DeviceIoControl(
                hDevice, ctlCode,
                ref inBuffer, sizeof(byte),
                ref outBuffer, sizeof(byte),
                out _, IntPtr.Zero);

            if (!success)
            {
                ErrorCode = Marshal.GetLastWin32Error();
            }
            return success;
        }

        public bool IOControl(uint ctlCode, ref ushort inBuffer, out ushort outBuffer)
        {
            outBuffer = 0;
            if (hDevice == IntPtr.Zero)
            {
                return false;
            }

            bool success = Kernel32.DeviceIoControl(
                hDevice, ctlCode,
                ref inBuffer, sizeof(ushort),
                ref outBuffer, sizeof(ushort),
                out _, IntPtr.Zero);

            if (!success)
            {
                ErrorCode = Marshal.GetLastWin32Error();
            }
            return success;
        }

        public bool IOControl(uint ctlCode, ref uint inBuffer, out uint outBuffer)
        {
            outBuffer = 0;
            if (hDevice == IntPtr.Zero)
            {
                return false;
            }

            bool success = Kernel32.DeviceIoControl(
                hDevice, ctlCode,
                ref inBuffer, sizeof(uint),
                ref outBuffer, sizeof(uint),
                out _, IntPtr.Zero);

            if (!success)
            {
                ErrorCode = Marshal.GetLastWin32Error();
            }
            return success;
        }

        #region Cleanup code
        ~Driver()
        {
            Dispose(false);
        }

        /// <summary>
        /// Releases all resources associated with this <see cref="Driver"/>.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            // Don't do anything if we already called Dispose:
            if (Disposed)
            {
                return;
            }

            if (disposing)
            {
                // Dispose managed objects
                // (classes that implement IDisposable, etc.)
            }

            // Close all open file and service handles
            Close();

            Disposed = true;
        }
        #endregion
    }

    [Flags]
    public enum DriverStatus
    {
        /// <summary>
        /// The driver is in an unknown state.
        /// </summary>
        Unknown = 0x00,
        /// <summary>
        /// The driver has been installed to the system.
        /// </summary>
        Installed = 0x01,
        /// <summary>
        /// The driver class is connected to the driver.
        /// </summary>
        Open = 0x02,
        /// <summary>
        /// The specified driver file was not found on the system.
        /// </summary>
        FileNotFound = 0x04,
        /// <summary>
        /// The specified driver file is invalid or corrupt.
        /// </summary>
        InvalidFile = 0x08,
    }
}
