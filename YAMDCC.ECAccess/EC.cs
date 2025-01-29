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
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace YAMDCC.ECAccess;

/// <summary>
/// Methods to access the embedded controller in a computer.
/// </summary>
public sealed class EC
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
    /// The maximum number of read/write attempts before returning an error.
    /// </summary>
    private const int RW_MAX_RETRIES = 5;

    /// <summary>
    /// The maximum time (in <see cref="Stopwatch"/> ticks)
    /// to wait for an EC status.
    /// </summary>
    private static readonly long ECStatusTimeoutTicks =
        Stopwatch.Frequency / 100;  // 10 ms, should be plenty for EC status waits

    /// <summary>
    /// Used to synchronise EC access.
    /// </summary>
    private static readonly Mutex EcMutex = new(false,
        "EcMutex-{B39C3216-FB5D-431C-8F7B-C94EA01AB855}");

    /// <summary>
    /// The underlying driver interface object.
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
    /// If <see langword="false"/> was returned by this function,
    /// <see cref="GetDriverError"/> can be called to check for driver errors.
    /// </remarks>
    /// <returns>
    /// <see langword="true"/> if the WinRing0 driver was loaded successfully
    /// (or is already loaded), <see langword="false"/> otherwise.
    /// </returns>
    public bool LoadDriver()
    {
        // Attempt to open an already installed WinRing0 driver first
        if (_Driver.Open())
        {
            return true;
        }

        // If opening the driver fails, uninstall (if installed) and reinstall it
        if (!_Driver.Uninstall())
        {
            return false;
        }

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
    /// or otherwise no longer requires EC access.
    /// </remarks>
    public void UnloadDriver()
    {
        if (GetRefCount() <= 1)
        {
            // only uninstall the driver if we're the last program using it
            // (Driver.Uninstall() calls Driver.Close() internally)
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
    /// <param name="reg">
    /// The register to read from.
    /// </param>
    /// <param name="value">
    /// If successful, contains the value at the specified register (otherwise zero).
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the operation was successful, otherwise <see langword="false"/>.
    /// </returns>
    public bool ReadByte(byte reg, out byte value)
    {
        value = 0;

        // only attempt to read EC if driver connection has been opened
        if (_Driver.IsOpen)
        {
            for (int i = 0; i < RW_MAX_RETRIES; i++)
            {
                if (TryReadByte(reg, out value))
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
    /// <param name="reg">
    /// The register to write to.
    /// </param>
    /// <param name="value">
    /// The value to write to the register.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the operation was successful, otherwise <see langword="false"/>.
    /// </returns>
    public bool WriteByte(byte reg, byte value)
    {
        // only attempt to write EC if driver connection has been opened
        if (_Driver.IsOpen)
        {
            for (int i = 0; i < RW_MAX_RETRIES; i++)
            {
                if (TryWriteByte(reg, value))
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
    /// <param name="reg">
    /// The register to read from.
    /// </param>
    /// <param name="bigEndian">
    /// Indicates the endianness of the value to be read.
    /// Defaults to <see langword="false"/> (little-endian).
    /// </param>
    /// <param name="value">
    /// If successful, contains the value at the specified register (otherwise zero).
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the operation was successful, otherwise <see langword="false"/>.
    /// </returns>
    public bool ReadWord(byte reg, out ushort value, bool bigEndian = false)
    {
        value = 0;

        // only attempt to read EC if driver connection has been opened
        if (_Driver.IsOpen)
        {
            for (int i = 0; i < RW_MAX_RETRIES; i++)
            {
                if (TryReadWord(reg, bigEndian, out value))
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
    /// <param name="reg">
    /// The register to write to.
    /// </param>
    /// <param name="value">
    /// The value to write at the register.
    /// </param>
    /// <param name="bigEndian">
    /// Indicates the endianness of the value to be written.
    /// Defaults to <see langword="false"/> (little-endian).
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the operation was successful, otherwise <see langword="false"/>.
    /// </returns>
    public bool WriteWord(byte reg, ushort value, bool bigEndian = false)
    {
        // only attempt to write EC if driver connection has been opened
        if (_Driver.IsOpen)
        {
            for (int i = 0; i < RW_MAX_RETRIES; i++)
            {
                if (TryWriteWord(reg, value, bigEndian))
                {
                    return true;
                }
            }
        }
        return false;
    }

    private bool TryReadByte(byte reg, out byte value)
    {
        value = 0;
        if (EcMutex.WaitOne(2000))
        {
            try
            {
                return WaitWrite() && WriteIOPort(PORT_COMMAND, (byte)ECCommand.Read)
                    && WaitWrite() && WriteIOPort(PORT_DATA, reg)
                    && WaitWrite() && WaitRead()
                    && ReadIOPort(PORT_DATA, out value);
            }
            finally
            {
                EcMutex.ReleaseMutex();
            }
        }
        return false;
    }

    private bool TryWriteByte(byte reg, byte value)
    {
        if (EcMutex.WaitOne(2000))
        {
            try
            {
                return WaitWrite() && WriteIOPort(PORT_COMMAND, (byte)ECCommand.Write)
                    && WaitWrite() && WriteIOPort(PORT_DATA, reg)
                    && WaitWrite() && WriteIOPort(PORT_DATA, value);
            }
            finally
            {
                EcMutex.ReleaseMutex();
            }
        }
        return false;
    }

    private bool TryReadWord(byte reg, bool bigEndian, out ushort value)
    {
        value = 0;

        // read least-significant byte
        if (!TryReadByte(bigEndian ? (byte)(reg + 1) : reg, out byte result))
        {
            return false;
        }
        value = result;

        // read most-significant byte
        if (!TryReadByte(bigEndian ? reg : (byte)(reg + 1), out result))
        {
            return false;
        }
        value |= (ushort)(result << 8);

        return true;
    }

    private bool TryWriteWord(byte reg, ushort value, bool bigEndian)
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

        return TryWriteByte(reg, lsb) && TryWriteByte((byte)(reg + 1), msb);
    }

    /// <summary>
    /// Waits for the EC to process a read command.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the EC is ready to have data read from it,
    /// <see langword="false"/> if the operation timed out.
    /// </returns>
    private bool WaitRead()
    {
        return WaitForECStatus(ECStatus.OutputBufferFull, true);
    }

    /// <summary>
    /// Waits for the EC to process a write command.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the EC is ready to accept data,
    /// <see langword="false"/> if the operation timed out.
    /// </returns>
    private bool WaitWrite()
    {
        return WaitForECStatus(ECStatus.InputBufferFull, false);
    }

    /// <summary>
    /// Waits for the specified <see cref="ECStatus"/> to be set/unset.
    /// </summary>
    /// <param name="status">
    /// The <see cref="ECStatus"/> to wait for.
    /// </param>
    /// <param name="isSet">
    /// Whether to wait for the status to be set or unset.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the EC status was (un)set
    /// before timing out, otherwise <see langword="false"/>.
    /// </returns>
    private bool WaitForECStatus(ECStatus status, bool isSet)
    {
        Stopwatch sw = Stopwatch.StartNew();
        try
        {
            while (sw.ElapsedTicks < ECStatusTimeoutTicks)
            {
                // Read the EC status from the command port
                if (ReadIOPort(PORT_COMMAND, out byte value))
                {
                    ECStatus ecStatus = (ECStatus)value;
                    bool ecIsSet = (status & ecStatus) == status;

                    if (isSet == ecIsSet)
                    {
                        return true;
                    }
                }
            }
        }
        finally
        {
            sw.Stop();
        }

        return false;
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
        WriteIOPortInput input = new(port, value);
        return _Driver.IOControl((uint)Ring0Control.WriteIOPortByte, ref input);
    }

    private uint GetRefCount()
    {
        uint refCount = 0;
        return _Driver.IOControl((uint)Ring0Control.GetRefCount, ref refCount, out refCount) ? refCount : 0;
    }

    /// <summary>
    /// Gets the last error produced by the underlying driver library.
    /// </summary>
    /// <returns>
    /// The last error code produced by the driver library.
    /// </returns>
    public int GetDriverError()
    {
        return _Driver.ErrorCode;
    }

    private enum Ring0Control : uint
    {
        // DeviceType, Function, Access (1 = Read, 2 = Write, 0 = Any)
        GetDriverVersion = 40000u << 16 | 0x800 << 2,
        GetRefCount      = 40000u << 16 | 0x801 << 2,
        ReadIOPortByte   = 40000u << 16 | 0x833 << 2 | 1 << 14,
        WriteIOPortByte  = 40000u << 16 | 0x836 << 2 | 2 << 14,
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
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
