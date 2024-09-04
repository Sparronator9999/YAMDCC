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
using System.ComponentModel;
using System.IO;
using System.IO.Pipes;
using System.ServiceProcess;
using YAMDCC.Config;
using YAMDCC.ECAccess;
using YAMDCC.IPC;
using YAMDCC.Logs;

namespace YAMDCC.Service
{
    internal sealed partial class svcFanControl : ServiceBase
    {
        #region Fields

        private static readonly string DataPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
            "Sparronator9999", "YAMDCC");

        /// <summary>
        /// The currently loaded MSI Fan Control config.
        /// </summary>
        private YAMDCC_Config Config;

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
        private static readonly Logger Log = new Logger
        {
            ConsoleLogLevel = LogLevel.None,
            FileLogLevel = LogLevel.Debug,
        };

        private readonly EC _EC;
        #endregion

        /// <summary>
        /// Initialises a new instance of the <see cref="svcFanControl"/> class.
        /// </summary>
        /// <param name="logger">The <see cref="Logger"/> instance to write logs to.</param>
        public svcFanControl()
        {
            InitializeComponent();
            AppDomain.CurrentDomain.UnhandledException += LogUnhandledException;

            _EC = new EC();

            PipeSecurity security = new PipeSecurity();
            //security.AddAccessRule(new PipeAccessRule("Administrators", PipeAccessRights.ReadWrite, AccessControlType.Allow));
            security.SetSecurityDescriptorSddlForm("O:BAG:SYD:(A;;GA;;;SY)(A;;GRGW;;;BA)");

            IPCServer = new Server<ServiceCommand, ServiceResponse>("YAMDCC-Server", 0, security);
        }

        #region Events
        protected override void OnStart(string[] args)
        {
            Log.Info(Strings.GetString("svcStarting"));

            // Load the service config.
            LoadConf();

            // Install WinRing0 to get EC access
            try
            {
                Log.Info(Strings.GetString("drvLoad"));
                if (!_EC.LoadDriver())
                {
                    throw new Win32Exception(_EC.GetDriverError());
                }
            }
            catch (Win32Exception)
            {
                Log.Fatal(Strings.GetString("drvLoadFail"));
                _EC.UnloadDriver();
                ExitCode = 1;
                throw;
            }
            Log.Info(Strings.GetString("drvLoadSuccess"));

            // Set up IPC server
            Log.Debug("Starting IPC server...");
            IPCServer.ClientConnected += IPCClientConnect;
            IPCServer.ClientDisconnected += IPCClientDisconnect;
            IPCServer.Start();

            Log.Info(Strings.GetString("svcStarted"));

            // Apply the fan curve and charging threshold:
            ApplySettings();
        }

        protected override void OnStop()
        {
            Log.Info(Strings.GetString("svcStopping"));

            // Stop the IPC server:
            Log.Debug("Stopping IPC server...");
            IPCServer.Stop();
            IPCServer.ClientConnected -= IPCClientConnect;
            IPCServer.ClientDisconnected -= IPCClientDisconnect;

            // Uninstall WinRing0 to keep things clean
            Log.Debug(Strings.GetString("drvUnload"));
            _EC.UnloadDriver();

            Log.Info(Strings.GetString("svcStopped"));
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
                    ApplySettings();
                    break;
            }
            return true;
        }

        private void IPCClientConnect(NamedPipeConnection<ServiceCommand, ServiceResponse> connection)
        {
            connection.ReceiveMessage += IPCClientMessage;
            Log.Info(Strings.GetString("ipcConnect"), connection.ID);
        }

        private void IPCClientDisconnect(NamedPipeConnection<ServiceCommand, ServiceResponse> connection)
        {
            connection.ReceiveMessage -= IPCClientMessage;
            Log.Info(Strings.GetString("ipcDC"), connection.ID);
        }

        private void IPCClientMessage(NamedPipeConnection<ServiceCommand, ServiceResponse> connection, ServiceCommand message)
        {
            int error = 0;

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
                    ApplySettings();
                    error = 0;
                    break;
                case Command.FullBlast:
                    error = SetFullBlast(connection.Name, message.Arguments);
                    break;
                default:    // Unknown command
                    Log.Error(Strings.GetString("errBadCmd"), message);
                    break;
            }

            switch (error)
            {
                case 2:
                    Log.Error(Strings.GetString("errOffendingCmd"), message.Command, message.Arguments);
                    break;
                case 3:
                    Log.Error(Strings.GetString("errECLock"));
                    break;
                default:
                    break;
            }
        }
        #endregion

        private void LogECReadError(int reg)
        {
            Log.Error(Strings.GetString("errECRead"), $"0x{reg:X}", new Win32Exception(_EC.GetDriverError()).Message);
        }

        private void LogECWriteError(int reg)
        {
            Log.Error(Strings.GetString("errECWrite"), $"0x{reg:X}", new Win32Exception(_EC.GetDriverError()).Message);
        }

        private void LoadConf()
        {
            Log.Info(Strings.GetString("cfgLoading"));

            string confPath = Path.Combine(DataPath, "CurrentConfig.xml");

            if (File.Exists(confPath))
            {
                try
                {
                    Config = YAMDCC_Config.Load(confPath);
                }
                catch (InvalidConfigException)
                {
                    ConfigLoaded = false;
                    Log.Error(Strings.GetString("cfgInvalid"));
                }

                ConfigLoaded = true;
                Log.Info(Strings.GetString("cfgLoadSuccess"));
            }
            else
            {
                Log.Warn(Strings.GetString("cfgNotFound"));
                ConfigLoaded = false;
            }
        }

        private void ApplySettings()
        {
            if (ConfigLoaded)
            {
                Log.Debug(Strings.GetString("cfgApplying"));
                if (EC.AcquireLock(1000))
                {
                    // Write custom register values, if configured:
                    if (!(Config.RegConfs is null) && Config.RegConfs.Length > 0)
                    {
                        for (int i = 0; i < Config.RegConfs.Length; i++)
                        {
                            RegConf cfg = Config.RegConfs[i];
                            Log.Debug($"Writing custom EC register configs ({i + 1}/{Config.RegConfs.Length})...");
                            if (!_EC.WriteByte(cfg.Reg, cfg.Value))
                            {
                                LogECWriteError(cfg.Reg);
                            }
                        }
                    }

                    // Write the fan curve to the appropriate registers for each fan:
                    for (int i = 0; i < Config.FanConfs.Length; i++)
                    {
                        FanConf cfg = Config.FanConfs[i];
                        Log.Debug($"Writing fan curve configuration for {cfg.Name} ({i + 1}/{Config.FanConfs.Length})...");
                        FanCurveConf curveCfg = cfg.FanCurveConfs[cfg.CurveSel];

                        for (int j = 0; j < curveCfg.TempThresholds.Length; j++)
                        {
                            if (!_EC.WriteByte(cfg.FanCurveRegs[j], curveCfg.TempThresholds[j].FanSpeed))
                            {
                                LogECWriteError(cfg.FanCurveRegs[j]);
                            }

                            if (j > 0)
                            {
                                if (!_EC.WriteByte(cfg.UpThresholdRegs[j - 1], curveCfg.TempThresholds[j].UpThreshold))
                                {
                                    LogECWriteError(cfg.UpThresholdRegs[j - 1]);
                                }

                                byte downT = (byte)(curveCfg.TempThresholds[j].UpThreshold - curveCfg.TempThresholds[j].DownThreshold);
                                if (!_EC.WriteByte(cfg.DownThresholdRegs[j - 1], downT))
                                {
                                    LogECWriteError(cfg.DownThresholdRegs[j - 1]);
                                }
                            }
                        }
                    }

                    // Write the charge threshold:
                    if (!(Config.ChargeLimitConf is null))
                    {
                        Log.Debug("Writing charge limit configuration...");
                        byte value = (byte)(Config.ChargeLimitConf.MinVal + Config.ChargeLimitConf.CurVal);
                        if (!_EC.WriteByte(Config.ChargeLimitConf.Reg, value))
                        {
                            LogECWriteError(Config.ChargeLimitConf.Reg);
                        }
                    }

                    // Write the performance mode
                    if (!(Config.PerfModeConf is null))
                    {
                        Log.Debug("Writing performance mode setting...");
                        byte value = Config.PerfModeConf.PerfModes[Config.PerfModeConf.ModeSel].Value;
                        if (!_EC.WriteByte(Config.PerfModeConf.Reg, value))
                        {
                            LogECWriteError(Config.PerfModeConf.Reg);
                        }
                    }

                    // Write the Win/Fn key swap setting
                    if (!(Config.KeySwapConf is null))
                    {
                        Log.Debug("Writing Win/Fn key swap setting...");
                        byte value = Config.KeySwapConf.Enabled
                            ? Config.KeySwapConf.OnVal
                            : Config.KeySwapConf.OffVal;

                        if (!_EC.WriteByte(Config.KeySwapConf.Reg, value))
                        {
                            LogECWriteError(Config.KeySwapConf.Reg);
                        }
                    }

                    EC.ReleaseLock();
                }
                else
                {
                    Log.Error(Strings.GetString("errECLock"));
                }
            }
        }

        /// <summary>
        /// Parse arguments from a given string.
        /// </summary>
        /// <param name="args_in">The string containing the space-delimited arguments.</param>
        /// <param name="expected_args">The expected number of arguments. Must be zero or positive.</param>
        /// <param name="args_out">The parsed arguments. Will be empty if parsing fails.</param>
        /// <returns></returns>
        private static bool ParseArgs(string args_in, int expected_args, out int[] args_out)
        {
            args_out = new int[expected_args];

            if (expected_args == 0)
            {
                if (!string.IsNullOrEmpty(args_in))
                {
                    Log.Warn(Strings.GetString("warnArgsBadLength"));
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
                            Log.Error(Strings.GetString("errArgsBadType"));
                            return false;
                        }
                    }
                    return true;
                }
                else
                {
                    Log.Error(Strings.GetString("errArgsBadLength"));
                }
            }
            else
            {
                Log.Error(Strings.GetString("errArgsMissing"));
            }

            return false;
        }

        private int ReadECByte(string name, string args)
        {
            if (ParseArgs(args, 1, out int[] pArgs))
            {
                if (EC.AcquireLock(1000))
                {
                    Log.Debug(Strings.GetString("svcECReading"), $"{pArgs[0]:X}");
                    bool success = _EC.ReadByte((byte)pArgs[0], out byte value);
                    EC.ReleaseLock();

                    if (success)
                    {
                        ServiceResponse response = new ServiceResponse(Response.ReadResult, $"{pArgs[0]} {value}");
                        IPCServer.PushMessage(response, name);
                        Log.Debug(Strings.GetString("svcECReadSuccess"), $"{pArgs[1]:X}", $"{value:X}");
                    }
                    else
                    {
                        LogECReadError(pArgs[0]);
                    }
                    return 0;
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
                    Log.Debug(Strings.GetString("svcECWriting"), $"{pArgs[1]:X}", $"{pArgs[0]:X}");
                    bool success = _EC.WriteByte((byte)pArgs[0], (byte)pArgs[1]);
                    EC.ReleaseLock();

                    if (success)
                    {
                        Log.Debug(Strings.GetString("svcECWriteSuccess"), $"{pArgs[0]:X}");
                    }
                    else
                    {
                        LogECWriteError(pArgs[0]);
                    }
                    return 0;
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
                    FanConf cfg = Config.FanConfs[pArgs[0]];
                    bool success = _EC.ReadByte(cfg.SpeedReadReg, out byte speed);
                    EC.ReleaseLock();

                    if (success)
                    {
                        ServiceResponse response = new ServiceResponse(Response.FanSpeed, $"{speed}");
                        IPCServer.PushMessage(response, name);
                    }
                    else
                    {
                        LogECReadError(pArgs[0]);
                    }
                    return 0;
                }
                return 3;
            }
            return 2;
        }

        private int GetFanRPM(string name, string args)
        {
            if (ParseArgs(args, 1, out int[] pArgs))
            {
                FanConf cfg = Config.FanConfs[pArgs[0]];

                if (!(cfg.RPMConf is null))
                {
                    if (EC.AcquireLock(1000))
                    {
                        bool success;
                        ushort rpmValue;
                        if (cfg.RPMConf.Is16Bit)
                        {
                            success = _EC.ReadWord(cfg.RPMConf.ReadReg, out rpmValue, cfg.RPMConf.IsBigEndian);
                        }
                        else
                        {
                            success = _EC.ReadByte(cfg.RPMConf.ReadReg, out byte rpmValByte);
                            rpmValue = rpmValByte;
                        }
                        EC.ReleaseLock();

                        if (success)
                        {
#pragma warning disable IDE0045 // Supress "if statement can be simplified" suggestion
                            int rpm;
                            if (cfg.RPMConf.Invert)
                            {
                                if (rpmValue == 0)
                                {
                                    rpm = -1;
                                }
                                else if (cfg.RPMConf.DivideByMult)
                                {
                                    rpm = cfg.RPMConf.RPMMult / rpmValue;
                                }
                                else
                                {
                                    rpm = 1 / (rpmValue * cfg.RPMConf.RPMMult);
                                }
                            }
                            else if (cfg.RPMConf.DivideByMult)
                            {
                                rpm = rpmValue / cfg.RPMConf.RPMMult;
                            }
                            else
                            {
                                rpm = rpmValue * cfg.RPMConf.RPMMult;
                            }
#pragma warning restore IDE0045
                            ServiceResponse response = new ServiceResponse(Response.FanRPM, $"{rpm}");
                            IPCServer.PushMessage(response, name);
                        }
                        else
                        {
                            LogECReadError(pArgs[0]);
                        }
                        return 0;
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
                    FanConf cfg = Config.FanConfs[pArgs[0]];
                    bool success = _EC.ReadByte(cfg.TempReadReg, out byte temp);
                    EC.ReleaseLock();
                    if (success)
                    {
                        ServiceResponse response = new ServiceResponse(Response.Temp, $"{temp}");
                        IPCServer.PushMessage(response, name);
                    }
                    else
                    {
                        LogECReadError(pArgs[0]);
                    }
                    return 0;
                }
                return 3;
            }
            return 2;
        }

        private int SetFullBlast(string name, string args)
        {
            if (!(Config.FullBlastConf is null))
            {
                if (ParseArgs(args, 1, out int[] pArgs))
                {
                    if (EC.AcquireLock(500))
                    {
                        bool success;
                        if (pArgs[0] == 1)
                        {
                            Log.Debug("Enabling Full Blast...");
                            success = _EC.WriteByte(Config.FullBlastConf.Reg,
                                Config.FullBlastConf.OnVal);
                        }
                        else
                        {
                            Log.Debug("Disabling Full Blast...");
                            success = _EC.WriteByte(Config.FullBlastConf.Reg,
                                Config.FullBlastConf.OffVal);
                        }
                        EC.ReleaseLock();

                        if (!success)
                        {
                            LogECReadError(Config.FullBlastConf.Reg);
                        }
                        return 0;
                    }
                    return 3;
                }
                return 2;
            }
            return 0;
        }

        private static void LogUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Log.Fatal(Strings.GetString("svcException"), e.ExceptionObject);
        }
    }
}
