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

using MSIFanControl.Config;
using MSIFanControl.ECAccess;
using MSIFanControl.IPC;
using MSIFanControl.Logs;
using System;
using System.ComponentModel;
using System.IO;
using System.IO.Pipes;
using System.Resources;
using System.Security.AccessControl;
using System.ServiceProcess;

namespace MSIFanControl.Service
{
    internal sealed partial class svcFanControl : ServiceBase
    {
        #region Fields

        private static readonly string DataPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
            "Sparronator9999", "MSI Fan Control");

        /// <summary>
        /// The currently loaded MSI Fan Control config.
        /// </summary>
        private FanControlConfig Config;

        /// <summary>
        /// Has a valid MSI Fan Control config been loaded?
        /// </summary>
        private bool ConfigLoaded;

        /// <summary>
        /// The named message pipe server that MSI Fan Control connects to.
        /// </summary>
        private readonly Server<ServiceCommand, ServiceResponse> IPCServer;

        /// <summary>
        /// The <see cref="Logger"/> instance to write logs to.
        /// </summary>
        private readonly Logger Log;

        /// <summary>
        /// The <see cref="ResourceManager"/> instance to obtain resources from.
        /// </summary>
        private readonly ResourceManager Res;

        private readonly EC _EC;
        #endregion

        /// <summary>
        /// Initialises a new instance of the <see cref="svcFanControl"/> class.
        /// </summary>
        /// <param name="logger">The <see cref="Logger"/> instance to write logs to.</param>
        internal svcFanControl(Logger logger, ResourceManager res)
        {
            InitializeComponent();
            Log = logger;
            Res = res;
            _EC = new EC();

            PipeSecurity security = new PipeSecurity();
            security.AddAccessRule(new PipeAccessRule("Administrators", PipeAccessRights.ReadWrite, AccessControlType.Allow));

            IPCServer = new Server<ServiceCommand, ServiceResponse>("MSIFC-Server", security);
        }

        #region Events
        protected override void OnStart(string[] args)
        {
            Log.Info(Res.GetString("svcStarting"));

            // Load the service config.
            LoadConf();

            // Install WinRing0 to get EC access
            try
            {
                Log.Info(Res.GetString("drvLoad"));
                if (!_EC.LoadDriver())
                {
                    throw new ApplicationException(string.Format(Res.GetString("drvLoadFailS"), new Win32Exception(_EC.GetDriverError()).Message));
                }
            }
            catch (ApplicationException)
            {
                Log.Fatal(Res.GetString("drvLoadFail"));
                _EC.UnloadDriver();
                ExitCode = 1;
                throw;
            }
            Log.Info(Res.GetString("drvLoadSuccess"));

            // Set up IPC server
            Log.Debug("Starting IPC server...");
            IPCServer.ClientConnected += IPCClientConnect;
            IPCServer.ClientDisconnected += IPCClientDisconnect;
            IPCServer.Start();

            Log.Info(Res.GetString("svcStarted"));

            // Apply the fan curve and charging threshold:
            ApplyCurve();
        }

        protected override void OnStop()
        {
            Log.Info(Res.GetString("svcStopping"));

            // Stop the IPC server:
            Log.Debug("Stopping IPC server...");
            IPCServer.Stop();
            IPCServer.ClientConnected -= IPCClientConnect;
            IPCServer.ClientDisconnected -= IPCClientDisconnect;

            // Uninstall WinRing0 to keep things clean
            Log.Debug(Res.GetString("drvUnload"));
            _EC.UnloadDriver();

            Log.Info(Res.GetString("svcStopped"));
        }

        protected override bool OnPowerEvent(PowerBroadcastStatus powerStatus)
        {
            Log.Debug($"Power event {powerStatus} received");
            switch (powerStatus)
            {
                case PowerBroadcastStatus.ResumeCritical:
                case PowerBroadcastStatus.ResumeSuspend:
                case PowerBroadcastStatus.ResumeAutomatic:
                    // Re-apply the fan curve after waking up from sleep:
                    Log.Debug($"Re-applying fan curve...");
                    ApplyCurve();
                    break;
            }
            return true;
        }

        private void IPCClientConnect(NamedPipeConnection<ServiceCommand, ServiceResponse> connection)
        {
            connection.ReceiveMessage += IPCClientMessage;
            Log.Info(Res.GetString("ipcConnect"), connection.ID);
        }

        private void IPCClientDisconnect(NamedPipeConnection<ServiceCommand, ServiceResponse> connection)
        {
            connection.ReceiveMessage -= IPCClientMessage;
            Log.Info(Res.GetString("ipcDC"), connection.ID);
        }

        private void IPCClientMessage(NamedPipeConnection<ServiceCommand, ServiceResponse> connection, ServiceCommand message)
        {
            int error;

            switch (message.Command)
            {
                case Command.ReadECByte:
                    error = ReadECByte(connection.Name, message.Arguments);
                    break;
                case Command.WriteECByte:
                    error = WriteECByte(connection.Name, message.Arguments);
                    break;
                case Command.GetFanSpeed:
                    error = GetFanSpeed(connection.Name, message.Arguments);
                    break;
                case Command.GetFanRPM:
                    error = GetFanRPM(connection.Name, message.Arguments);
                    break;
                case Command.GetTemp:
                    error = GetTemp(connection.Name, message.Arguments);
                    break;
                case Command.ApplyConfig:
                    LoadConf();
                    ApplyCurve();
                    error = 0;
                    break;
                case Command.FullBlast:
                    error = SetFullBlast(connection.Name, message.Arguments);
                    break;
                default:    // Unknown command
                    error = 1;
                    break;
            }

            switch (error)
            {
                case 1:
                    Log.Error(Res.GetString("errBadCmd"), message);
                    break;
                case 2:
                    Log.Error(Res.GetString("errOffendingCmd"), message.Command, message.Arguments);
                    break;
                case 3:
                    Log.Error(Res.GetString("errECLock"));
                    break;
                case 4:
                    Log.Error(Res.GetString("errECRead"));
                    break;
                case 5:
                    Log.Error(Res.GetString("errECWrite"));
                    break;
                default:
                    break;
            }
        }
        #endregion

        private void LoadConf()
        {
            Log.Info(Res.GetString("cfgLoading"));

            string confPath = Path.Combine(DataPath, "CurrentConfig.xml");

            if (File.Exists(confPath))
            {
                try
                {
                    Config = FanControlConfig.Load(confPath);
                }
                catch
                {
                    ConfigLoaded = false;
                    Log.Error(Res.GetString("cfgInvalid"));
                }

                ConfigLoaded = true;
                Log.Info(Res.GetString("cfgLoadSuccess"));
            }
            else
            {
                Log.Warn(Res.GetString("cfgNotFound"));
                ConfigLoaded = false;
            }
        }

        private void ApplyCurve()
        {
            Log.Debug("Applying MSI Fan Control config...");
            if (ConfigLoaded)
            {
                if (EC.AcquireLock(1000))
                {
                    // Write custom register values, if configured:
                    if (Config.RegConfigs.Length > 0)
                    {
                        Log.Debug("Writing custom EC register configs...");
                        foreach (RegConfig cfg in Config.RegConfigs)
                        {
                            _EC.ReadByte(cfg.Register, out byte oldVal);
                            Log.Debug($"Writing value 0x{cfg.Value:X} to register 0x{cfg.Register:X} (old value: 0x{oldVal:X})...");

                            _EC.WriteByte(cfg.Register, cfg.Value);
                        }
                    }

                    // Write the fan curve to the appropriate registers for each fan:
                    foreach (FanConfig cfg in Config.FanConfigs)
                    {
                        Log.Debug($"Writing fan curve configuration for {cfg.Name}...");
                        FanCurveConfig curveCfg = cfg.FanCurveConfigs[cfg.CurveSel];

                        for (int i = 0; i < curveCfg.TempThresholds.Length; i++)
                        {
                            _EC.ReadByte(cfg.FanCurveRegs[i], out byte oldVal);
                            Log.Debug($"Writing value 0x{curveCfg.TempThresholds[i].FanSpeed:X} to register 0x{cfg.FanCurveRegs[i]:X} (old value: 0x{oldVal:X}, FanCurve)...");
                            _EC.WriteByte(cfg.FanCurveRegs[i], curveCfg.TempThresholds[i].FanSpeed);

                            if (i > 0)
                            {
                                _EC.ReadByte(cfg.UpThresholdRegs[i - 1], out oldVal);
                                Log.Debug($"Writing value 0x{curveCfg.TempThresholds[i].UpThreshold:X} to register 0x{cfg.UpThresholdRegs[i - 1]:X} (old value: 0x{oldVal:X}, UpThreshold)...");
                                _EC.WriteByte(cfg.UpThresholdRegs[i - 1], curveCfg.TempThresholds[i].UpThreshold);

                                _EC.ReadByte(cfg.DownThresholdRegs[i - 1], out oldVal);
                                byte downT = (byte)(curveCfg.TempThresholds[i].UpThreshold - curveCfg.TempThresholds[i].DownThreshold);
                                Log.Debug($"Writing value 0x{downT:X} to register 0x{cfg.DownThresholdRegs[i - 1]:X} (old value: 0x{oldVal:X}, DownThreshold)...");
                                _EC.WriteByte(cfg.DownThresholdRegs[i - 1], downT);
                            }
                        }
                    }

                    // Write the charge threshold:
                    Log.Debug($"Writing charge limit configuration...");
                    byte value = (byte)(Config.ChargeLimitConfig.MinValue + Config.ChargeLimitConfig.Value);
                    Log.Debug($"Writing value 0x{value:X} to register 0x{Config.ChargeLimitConfig.Register:X}...");
                    _EC.WriteByte(Config.ChargeLimitConfig.Register, value);

                    EC.ReleaseLock();
                }
                else
                {
                    Log.Error(Res.GetString("errECLock"));
                }
            }
            else
            {
                Log.Warn(Res.GetString("cfgNotLoaded"));
            }
        }

        /// <summary>
        /// Parse arguments from a given string.
        /// </summary>
        /// <param name="args_in">The string containing the space-delimited arguments.</param>
        /// <param name="expected_args">The expected number of arguments. Must be zero or positive.</param>
        /// <param name="args_out">The parsed arguments. Will be empty if parsing fails.</param>
        /// <returns></returns>
        private bool ParseArgs(string args_in, int expected_args, out int[] args_out)
        {
            args_out = new int[expected_args];

            if (expected_args == 0)
            {
                if (!string.IsNullOrEmpty(args_in))
                {
                    Log.Warn(Res.GetString("warnArgsBadLength"));
                }
                return true;
            }

            if (!string.IsNullOrEmpty(args_in))
            {
                string[] args_str = args_in.Split(' ');
                if (args_str.Length == expected_args)
                {
                    for (int i = 0; i < expected_args; i++)
                    {
                        if (int.TryParse(args_str[i], out int value))
                        {
                            args_out[i] = value;
                        }
                        else
                        {
                            Log.Error(Res.GetString("errArgsBadType"));
                            return false;
                        }
                    }
                    return true;
                }
                else
                {
                    Log.Error(Res.GetString("errArgsBadLength"));
                }
            }
            else
            {
                Log.Error(Res.GetString("errArgsMissing"));
            }

            return false;
        }

        private int ReadECByte(string name, string args)
        {
            if (ParseArgs(args, 1, out int[] pArgs))
            {
                if (EC.AcquireLock(1000))
                {
                    Log.Debug($"Reading EC register {pArgs[0]:X}...");
                    bool success = _EC.ReadByte((byte)pArgs[0], out byte value);
                    if (success)
                    {
                        ServiceResponse response = new ServiceResponse(Response.ReadResult, $"{pArgs[0]} {value}");
                        IPCServer.PushMessage(response, name);
                        Log.Debug($"EC register {pArgs[1]:X} has value of {value:X}");
                    }
                    EC.ReleaseLock();
                    return success ? 0 : 4;
                }
                return 3;
            }
            return 2;
        }

        private int WriteECByte(string name, string args)
        {
            if (ParseArgs(args, 2, out int[] pArgs))
            {
                if (EC.AcquireLock(1000))
                {
                    Log.Debug($"Writing {pArgs[1]:X} to EC register {pArgs[0]:X}...");
                    bool success = _EC.WriteByte((byte)pArgs[0], (byte)pArgs[1]);
                    if (success)
                        Log.Debug($"Wrote {pArgs[1]:X} to {pArgs[0]:X} successfully");
                    EC.ReleaseLock();
                    return success ? 0 : 5;
                }
                return 3;
            }
            return 2;
        }

        private int GetFanSpeed(string name, string args)
        {
            if (ParseArgs(args, 1, out int[] pArgs))
            {
                if (EC.AcquireLock(1000))
                {
                    FanConfig cfg = Config.FanConfigs[pArgs[0]];
                    bool success = _EC.ReadByte(cfg.SpeedReadReg, out byte speed);
                    if (success)
                    {
                        ServiceResponse response = new ServiceResponse(Response.FanSpeed, speed.ToString());
                        IPCServer.PushMessage(response, name);
                    }
                    EC.ReleaseLock();
                    return success ? 0 : 4;
                }
                return 3;
            }
            return 2;
        }

        private int GetFanRPM(string name, string args)
        {
            if (ParseArgs(args, 1, out int[] pArgs))
            {
                FanConfig cfg = Config.FanConfigs[pArgs[0]];

                if (!(cfg.RPMConfig is null))
                {
                    if (EC.AcquireLock(1000))
                    {
                        bool success;
                        ushort rpmValue;
                        if (cfg.RPMConfig.Is16Bit)
                            success = _EC.ReadWord(cfg.RPMConfig.ReadReg, out rpmValue, cfg.RPMConfig.IsBigEndian);
                        else
                        {
                            success = _EC.ReadByte(cfg.RPMConfig.ReadReg, out byte rpmValByte);
                            rpmValue = rpmValByte;
                        }

                        if (success)
                        {
#pragma warning disable IDE0045 // Supress "if statement can be simplified" suggestion
                            int rpm;
                            if (cfg.RPMConfig.Invert)
                            {
                                if (rpmValue == 0)
                                    rpm = -1;
                                else if (cfg.RPMConfig.DivideByMult)
                                    rpm = cfg.RPMConfig.Multiplier / rpmValue;
                                else
                                    rpm = 1 / (rpmValue * cfg.RPMConfig.Multiplier);
                            }
                            else if (cfg.RPMConfig.DivideByMult)
                                rpm = rpmValue / cfg.RPMConfig.Multiplier;
                            else
                                rpm = rpmValue * cfg.RPMConfig.Multiplier;
#pragma warning restore IDE0045
                            ServiceResponse response = new ServiceResponse(Response.FanRPM, $"{rpm}");
                            IPCServer.PushMessage(response, name);
                        }
                        EC.ReleaseLock();
                        return success ? 0 : 4;
                    }
                    return 3;
                }
                return 0;
            }
            return 2;
        }

        private int GetTemp(string name, string args)
        {
            if (ParseArgs(args, 1, out int[] pArgs))
            {
                if (EC.AcquireLock(1000))
                {
                    FanConfig cfg = Config.FanConfigs[pArgs[0]];
                    bool success = _EC.ReadByte(cfg.TempReadReg, out byte temp);
                    if (success)
                    {
                        ServiceResponse response = new ServiceResponse(Response.Temp, temp.ToString());
                        IPCServer.PushMessage(response, name);
                    }
                    EC.ReleaseLock();
                    return success ? 0 : 4;
                }
                return 3;
            }
            return 2;
        }

        private int SetFullBlast(string name, string args)
        {
            if (ParseArgs(args, 1, out int[] pArgs))
            {
                if (EC.AcquireLock(500))
                {
                    bool success;
                    if (pArgs[0] == 1)
                    {
                        Log.Debug("Enabling Full Blast...");
                        success = _EC.WriteByte(Config.FullBlastConfig.Register,
                            Config.FullBlastConfig.OnValue);
                    }
                    else
                    {
                        Log.Debug("Disabling Full Blast...");
                        success = _EC.WriteByte(Config.FullBlastConfig.Register,
                            Config.FullBlastConfig.OffValue);
                    }
                    EC.ReleaseLock();
                    if (!success)
                    {
                        Log.Error($"Error writing to EC: {new Win32Exception(_EC.GetDriverError()).Message}");
                    }
                    return success ? 0 : 5;
                }
                return 3;
            }
            return 2;
        }
    }
}
