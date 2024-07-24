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
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace MSIFanControl.ECAccess
{
    /// <summary>
    /// Methods to access the embedded controller in a computer.
    /// </summary>
    public class EC
    {
        // See ACPI specs ch 12.2
        [Flags]
        private enum ECStatus : byte
        {
            None = 0x00,
            OutputBufferFull = 0x01,    // OBF
            InputBufferFull = 0x02,     // IBF
            Command = 0x08,             // CMD
            BurstMode = 0x10,           // BURST
            SCIEventPending = 0x20,     // SCI_EVT
            SMIEventPending = 0x40      // SMI_EVT
        }

        // See ACPI specs ch 12.3
        private enum ECCommand : byte
        {
            Read = 0x80,            // RD_EC
            Write = 0x81,           // WR_EC
            BurstEnable = 0x82,     // BE_EC
            BurstDisable = 0x83,    // BD_EC
            Query = 0x84            // QR_EC
        }

        private const byte PORT_COMMAND = 0x66;   //EC_SC
        private const byte PORT_DATA = 0x62;      //EC_DATA

        /// <summary>
        /// The maximum number of read/write attempts before skipping the operation.
        /// </summary>
        private const int RW_MAX_RETRIES = 5;

        /// <summary>
        /// The maximum time (in <see cref="Stopwatch"/> ticks)
        /// to wait for an EC status.
        /// </summary>
        private static readonly long ECStatusTimeoutTicks = 1000 * Stopwatch.Frequency / 1000000L;

        /// <summary>
        /// Used to synchronise EC access.
        /// </summary>
        private static readonly Mutex EcMutex = new Mutex();

        /// <summary>
        /// Gets whether the WinRing0 driver is currently loaded.
        /// </summary>
        private readonly Driver _Driver;

        public EC()
        {
            _Driver = new Driver("WinRing0_1_2_0",
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                    Environment.Is64BitOperatingSystem
                        ? "WinRing0x64.sys"
                        : "WinRing0.sys"));
        }

        /// <summary>
        /// Loads the WinRing0 driver (if not already loaded).
        /// </summary>
        /// <remarks>
        /// If <c>false</c> was returned by this function,
        /// <seealso cref="GetDriverStatus"/> can be called to check for errors.
        /// </remarks>
        /// <returns>
        /// <c>true</c> if the WinRing0 driver was loaded successfully
        /// (or is already loaded), <c>false</c> otherwise.
        /// </returns>
        public bool LoadDriver()
        {
            // Attempt to open an already installed WinRing0 driver first
            if (_Driver.Open())
            {
                return true;
            }

            // If opening the driver fails, uninstall (if installed) and reinstall it
            _Driver.Uninstall();
            if (!_Driver.Install())
            {
                _Driver.Uninstall();
                return false;
            }

            return _Driver.Open();
        }

        /// <summary>
        /// Unloads the WinRing0 driver (if loaded).
        /// </summary>
        /// <remarks>
        /// This function should be called when the program exits
        /// or no longer requires EC access.
        /// </remarks>
        public void UnloadDriver()
        {
            if (GetRefCount() <= 1)
            {
                // only uninstall the driver if we're the last program using it
                // (Driver.Uninstall() calles Driver.Close() internally)
                _Driver.Uninstall();
            }
            else
            {
                // otherwise, just close the handle to the driver
                _Driver.Close();
            }
        }

        /// <summary>
        /// Reads a byte from the EC at the specified register.
        /// </summary>
        /// <param name="register">The register to read from.</param>
        /// <returns>The value at the specified register.</returns>
        public bool ReadByte(byte register, out byte value)
        {
            value = 0;

            // only attempt to read EC if driver connection has been opened
            if ((_Driver.Status & DriverStatus.Open) == DriverStatus.Open)
            {
                for (int i = 0; i < RW_MAX_RETRIES; i++)
                {
                    if (TryReadByte(register, out value))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// Writes a byte to the EC at the specified register.
        /// </summary>
        /// <param name="register">The register to write to.</param>
        /// <param name="value">The value to write at the register.</param>
        public bool WriteByte(byte register, byte value)
        {
            // only attempt to write EC if driver connection has been opened
            if ((_Driver.Status & DriverStatus.Open) == DriverStatus.Open)
            {
                for (int i = 0; i < RW_MAX_RETRIES; i++)
                {
                    if (TryWriteByte(register, value))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Reads a 16-bit integer (aka "word") from the EC at the specified register.
        /// </summary>
        /// <param name="register">The register to read from.</param>
        /// <param name="bigEndian">
        /// Indicates the endianness of the value to be read.
        /// Defaults to <c>false</c> (little-endian).
        /// </param>
        /// <returns>The value at the specified register.</returns>
        public bool ReadWord(byte register, out ushort value, bool bigEndian = false)
        {
            value = 0;

            // only attempt to read EC if driver connection has been opened
            if ((_Driver.Status & DriverStatus.Open) == DriverStatus.Open)
            {
                for (int i = 0; i < RW_MAX_RETRIES; i++)
                {
                    if (TryReadWord(register, bigEndian, out value))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Writes a 16-bit integer (aka "word") to the EC at the specified register.
        /// </summary>
        /// <param name="register">The register to write to.</param>
        /// <param name="value">The value to write at the register.</param>
        /// <param name="bigEndian">
        /// Indicates the endianness of the value to be written.
        /// Defaults to <c>false</c> (little-endian).
        /// </param>
        public bool WriteWord(byte register, ushort value, bool bigEndian = false)
        {
            // only attempt to write EC if driver connection has been opened
            if ((_Driver.Status & DriverStatus.Open) == DriverStatus.Open)
            {
                for (int i = 0; i < RW_MAX_RETRIES; i++)
                {
                    if (TryWriteWord(register, value, bigEndian))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Acquires a lock on the ISA bus. Call before any EC operation.
        /// </summary>
        /// <param name="timeout">The time to wait before releasing the lock.</param>
        /// <returns>
        /// <c>true</c> if the lock was acquired successfully, otherwise <c>false</c>.
        /// </returns>
        public static bool AcquireLock(int timeout) =>
            EcMutex.WaitOne(timeout);

        /// <summary>
        /// Releases a lock on the ISA bus.
        /// </summary>
        public static void ReleaseLock() =>
            EcMutex.ReleaseMutex();

        private bool TryReadByte(byte register, out byte value)
        {
            value = 0;
            return WaitWrite() && WriteIOPort(PORT_COMMAND, (byte)ECCommand.Read)
                && WaitWrite() && WriteIOPort(PORT_DATA, register)
                && WaitWrite() && WaitRead()
                && ReadIOPort(PORT_DATA, out value);
        }

        private bool TryWriteByte(byte register, byte value)
        {
            return WaitWrite() && WriteIOPort(PORT_COMMAND, (byte)ECCommand.Write)
                && WaitWrite() && WriteIOPort(PORT_DATA, register)
                && WaitWrite() && WriteIOPort(PORT_DATA, value);
        }

        private bool TryReadWord(byte register, bool bigEndian, out ushort value)
        {
            value = 0;

            // read least-significant byte
            if (!TryReadByte(bigEndian ? (byte)(register + 1) : register, out byte result))
            {
                return false;
            }
            value = result;

            // read most-significant byte
            if (!TryReadByte(bigEndian ? register : (byte)(register + 1), out result))
            {
                return false;
            }
            value |= (ushort)(result << 8);

            return true;
        }

        private bool TryWriteWord(byte register, ushort value, bool bigEndian)
        {
            byte lsb, msb;

            if (bigEndian)
            {
                msb = (byte)value;
                lsb = (byte)(value >> 8);
            }
            else
            {
                msb = (byte)(value >> 8);
                lsb = (byte)value;
            }

            return TryWriteByte(register, lsb) && TryWriteByte((byte)(register + 1), msb);
        }

        /// <summary>
        /// Waits for the EC output buffer to fill.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the EC is ready to have data read from it
        /// (with <see cref="ReadByte(byte, out byte)"/>,
        /// <c>false</c> if the operation timed out.
        /// </returns>
        private bool WaitRead() =>
            WaitForECStatus(ECStatus.OutputBufferFull, true);

        /// <summary>
        /// Waits for the EC input buffer to be free.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the EC is ready to accept data
        /// (with <see cref="WriteByte(byte, byte)"/>),
        /// <c>false</c> if the operation timed out.
        /// </returns>
        private bool WaitWrite() =>
            WaitForECStatus(ECStatus.InputBufferFull, false);

        /// <summary>
        /// Waits for the specified <see cref="ECStatus"/> to be set/unset.
        /// </summary>
        /// <param name="status">The <see cref="ECStatus"/> to wait for.</param>
        /// <param name="isSet">Whether to wait for the status to be set or unset.</param>
        /// <returns>
        /// <c>true</c> if the EC status was (un)set before timing out,
        /// otherwise <c>false</c>.
        /// </returns>
        private bool WaitForECStatus(ECStatus status, bool isSet)
        {
            bool success = false;

            Stopwatch sw = Stopwatch.StartNew();
            while (sw.ElapsedTicks < ECStatusTimeoutTicks)
            {
                // Read the EC status from the command port
                if (ReadIOPort(PORT_COMMAND, out byte value))
                {
                    ECStatus status2 = (ECStatus)value;

                    if (isSet && (status2 & status) == status)
                    {
                        success = true;
                        break;
                    }
                    else if (!isSet && (status2 & status) != status)
                    {
                        success = true;
                        break;
                    }
                }
            }
            sw.Stop();

            return success;
        }

        private bool ReadIOPort(uint port, out byte value)
        {
            bool success = _Driver.IOControl(
                (uint)Ring0Control.ReadIOPortByte,
                ref port, out uint val);

            // bitwise AND operation prevents integer overflow
            value = (byte)(val & 0xFF);
            return success;
        }

        private bool WriteIOPort(ushort port, byte value)
        {
            WriteIOPortInput input = new WriteIOPortInput(port, value);
            return _Driver.IOControl((uint)Ring0Control.WriteIOPortByte, input, null);
        }

        private uint GetRefCount()
        {
            uint refCount = 0;
            return _Driver.IOControl((uint)Ring0Control.GetRefCount, ref refCount, out refCount) ? refCount : 0;
        }

        /// <summary>
        /// Gets the status of the WinRing0 driver.
        /// </summary>
        /// <returns>
        /// A <see cref="DriverStatus"/> that represents
        /// the current driver status.
        /// </returns>
        public DriverStatus GetDriverStatus() =>
            _Driver.Status;

        public int GetDriverError() => _Driver.ErrorCode;

        private enum Ring0Control : uint
        {
            // DeviceType, Function, Access (1 = Read, 2 = Write, 0 = Any)
            GetDriverVersion = 40000u << 16 | 0x800 << 2,
            GetRefCount      = 40000u << 16 | 0x801 << 2,
            ReadIOPortByte   = 40000u << 16 | 0x833 << 2 | 1 << 14,
            WriteIOPortByte  = 40000u << 16 | 0x836 << 2 | 2 << 14,
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct WriteIOPortInput
        {
            public uint Port;
            public byte Value;

            public WriteIOPortInput(uint port, byte value)
            {
                Port = port;
                Value = value;
            }
        }
    }
}
