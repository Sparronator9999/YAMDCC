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
using System.Timers;
using YAMDCC.Config;
using YAMDCC.ECAccess;
using YAMDCC.IPC;
using YAMDCC.Logs;

namespace YAMDCC.Service
{
    internal sealed class FanControlService : ServiceBase
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
        private readonly NamedPipeServer<ServiceCommand, ServiceResponse> IPCServer;

        private readonly Logger Log;

        private readonly EC _EC;

        private volatile bool Cooldown;
        private readonly Timer CooldownTimer = new()
        {
            AutoReset = false,
            Interval = 1000,
        };
        #endregion

        /// <summary>
        /// Initialises a new instance of the <see cref="FanControlService"/> class.
        /// </summary>
        /// <param name="logger">The <see cref="Logger"/> instance to write logs to.</param>
        public FanControlService(Logger logger)
        {
            CanHandlePowerEvent = true;
            CanShutdown = true;

            Log = logger;
            _EC = new EC();

            PipeSecurity security = new();
            //security.AddAccessRule(new PipeAccessRule("Administrators", PipeAccessRights.ReadWrite, AccessControlType.Allow));
            security.SetSecurityDescriptorSddlForm("O:BAG:SYD:(A;;GA;;;SY)(A;;GRGW;;;BA)");

            IPCServer = new NamedPipeServer<ServiceCommand, ServiceResponse>("YAMDCC-Server", security);
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

            CooldownTimer.Elapsed += CooldownTimer_Elapsed;

            // Set up IPC server
            Log.Debug("Starting IPC server...");
            IPCServer.ClientConnected += IPCClientConnect;
            IPCServer.ClientDisconnected += IPCClientDisconnect;
            IPCServer.Error += IPCServerError;
            IPCServer.Start();

            Log.Info(Strings.GetString("svcStarted"));

            // Apply the fan curve and charging threshold:
            ApplySettings();
        }

        private void CooldownTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            CooldownTimer.Stop();
            Cooldown = false;
        }

        protected override void OnStop()
        {
            StopService();
        }

        protected override void OnShutdown()
        {
            StopService();
        }

        private void StopService()
        {
            Log.Info(Strings.GetString("svcStopping"));

            // Stop the IPC server:
            Log.Debug("Stopping IPC server...");
            IPCServer.Stop();
            IPCServer.ClientConnected -= IPCClientConnect;
            IPCServer.ClientDisconnected -= IPCClientDisconnect;
            IPCServer.Error -= IPCServerError;

            CooldownTimer.Elapsed -= CooldownTimer_Elapsed;

            // Uninstall WinRing0 to keep things clean
            Log.Debug(Strings.GetString("drvUnload"));
            _EC.UnloadDriver();

            Log.Info(Strings.GetString("svcStopped"));
        }

        protected override bool OnPowerEvent(PowerBroadcastStatus powerStatus)
        {
            switch (powerStatus)
            {
                case PowerBroadcastStatus.ResumeCritical:
                case PowerBroadcastStatus.ResumeSuspend:
                case PowerBroadcastStatus.ResumeAutomatic:
                    if (!Cooldown)
                    {
                        // Re-apply the fan curve after waking up from sleep:
                        Log.Debug($"Re-applying fan curve...");
                        ApplySettings();
                        Cooldown = true;
                        CooldownTimer.Start();
                    }
                    break;
            }
            return true;
        }

        private void IPCClientConnect(object sender, PipeConnectionEventArgs<ServiceCommand, ServiceResponse> e)
        {
            e.Connection.ReceiveMessage += IPCClientMessage;
            Log.Info(Strings.GetString("ipcConnect"), e.Connection.ID);
        }

        private void IPCClientDisconnect(object sender, PipeConnectionEventArgs<ServiceCommand, ServiceResponse> e)
        {
            e.Connection.ReceiveMessage -= IPCClientMessage;
            Log.Info(Strings.GetString("ipcDC"), e.Connection.ID);
        }

        private void IPCServerError(object sender, PipeErrorEventArgs<ServiceCommand, ServiceResponse> e) =>
            throw e.Exception;

        private void IPCClientMessage(object sender, PipeMessageEventArgs<ServiceCommand, ServiceResponse> e)
        {
            int error = 0;

            switch (e.Message.Command)
            {
                case Command.ReadECByte:
                    error = ReadECByte(e.Connection.ID, e.Message.Arguments);
                    break;
                case Command.WriteECByte:
                    error = WriteECByte(e.Connection.ID, e.Message.Arguments);
                    break;
                case Command.GetFanSpeed:
                    error = GetFanSpeed(e.Connection.ID, e.Message.Arguments);
                    break;
                case Command.GetFanRPM:
                    error = GetFanRPM(e.Connection.ID, e.Message.Arguments);
                    break;
                case Command.GetTemp:
                    error = GetTemp(e.Connection.ID, e.Message.Arguments);
                    break;
                case Command.ApplyConfig:
                    LoadConf();
                    ApplySettings();
                    ServiceResponse response = new(Response.Success, $"{(int)e.Message.Command}");
                    IPCServer.PushMessage(response, e.Connection.ID);
                    error = 0;
                    break;
                case Command.FullBlast:
                    error = SetFullBlast(e.Connection.ID, e.Message.Arguments);
                    break;
                case Command.GetKeyLightBright:
                    error = GetKeyLightBright(e.Connection.ID);
                    break;
                case Command.SetKeyLightBright:
                    error = SetKeyLightBright(e.Connection.ID, e.Message.Arguments);
                    break;
                case Command.FanCurveECToConf:
                    error = FanCurveECToConf(e.Connection.ID);
                    break;
                default:    // Unknown command
                    Log.Error(Strings.GetString("errBadCmd"), e.Message);
                    break;
            }

            switch (error)
            {
                case 2:
                    Log.Error(Strings.GetString("errOffendingCmd"), e.Message.Command, e.Message.Arguments);
                    break;
                case 3:
                    Log.Error(Strings.GetString("errECLock"));
                    break;
                default:
                    break;
            }
        }
        #endregion

        private bool LogECReadByteError(byte reg, out byte value)
        {
            bool success = _EC.ReadByte(reg, out value);
            if (!success)
            {
                Log.Error(Strings.GetString("errECRead"), $"0x{reg:X}", new Win32Exception(_EC.GetDriverError()).Message);
            }
            return success;
        }

        private bool LogECReadWordError(byte reg, out ushort value, bool bigEndian = false)
        {
            bool success = _EC.ReadWord(reg, out value, bigEndian);
            if (!success)
            {
                Log.Error(Strings.GetString("errECRead"), $"0x{reg:X}", new Win32Exception(_EC.GetDriverError()).Message);
            }
            return success;
        }

        private bool LogECWriteError(byte reg, byte value)
        {
            bool success = _EC.WriteByte(reg, value);
            if (!success)
            {
                Log.Error(Strings.GetString("errECWrite"), $"0x{reg:X}", new Win32Exception(_EC.GetDriverError()).Message);
            }
            return success;
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
                catch (Exception ex)
                {
                    if (ex is InvalidConfigException or InvalidOperationException)
                    {
                        ConfigLoaded = false;
                        Log.Error(Strings.GetString("cfgInvalid"));
                    }
                    else
                    {
                        throw;
                    }
                }

                if (Config.Template)
                {
                    Log.Error(
                        "Template configs are still WIP and unsupported for now.\n" +
                        "Please load another config using the configurator app.");
                    ConfigLoaded = false;
                    return;
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
                    if (Config.RegConfs is not null && Config.RegConfs.Length > 0)
                    {
                        for (int i = 0; i < Config.RegConfs.Length; i++)
                        {
                            RegConf cfg = Config.RegConfs[i];
                            Log.Debug(Strings.GetString("svcWritingCustomRegs", i + 1, Config.RegConfs.Length));
                            LogECWriteError(cfg.Reg, cfg.Value);
                        }
                    }

                    // Write the fan curve to the appropriate registers for each fan:
                    for (int i = 0; i < Config.FanConfs.Length; i++)
                    {
                        FanConf cfg = Config.FanConfs[i];
                        Log.Debug(Strings.GetString("svcWritingCustomRegs", cfg.Name, i + 1, Config.FanConfs.Length));
                        FanCurveConf curveCfg = cfg.FanCurveConfs[cfg.CurveSel];

                        for (int j = 0; j < curveCfg.TempThresholds.Length; j++)
                        {
                            LogECWriteError(cfg.FanCurveRegs[j], curveCfg.TempThresholds[j].FanSpeed);

                            if (j > 0)
                            {
                                LogECWriteError(cfg.UpThresholdRegs[j - 1], curveCfg.TempThresholds[j].UpThreshold);

                                byte downT = (byte)(curveCfg.TempThresholds[j].UpThreshold - curveCfg.TempThresholds[j].DownThreshold);
                                LogECWriteError(cfg.DownThresholdRegs[j - 1], downT);
                            }
                        }
                    }

                    // Write the charge threshold:
                    if (Config.ChargeLimitConf is not null)
                    {
                        Log.Debug(Strings.GetString("svcWritingChgLim"));
                        byte value = (byte)(Config.ChargeLimitConf.MinVal + Config.ChargeLimitConf.CurVal);
                        LogECWriteError(Config.ChargeLimitConf.Reg, value);
                    }

                    // Write the performance mode
                    if (Config.PerfModeConf is not null)
                    {
                        Log.Debug(Strings.GetString("svcWritingPerfMode"));
                        byte value = Config.PerfModeConf.PerfModes[Config.PerfModeConf.ModeSel].Value;
                        LogECWriteError(Config.PerfModeConf.Reg, value);
                    }

                    // Write the Win/Fn key swap setting
                    if (Config.KeySwapConf is not null)
                    {
                        Log.Debug(Strings.GetString("svcWritingKeySwap"));
                        byte value = Config.KeySwapConf.Enabled
                            ? Config.KeySwapConf.OnVal
                            : Config.KeySwapConf.OffVal;

                        LogECWriteError(Config.KeySwapConf.Reg, value);
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
        /// <param name="argsIn">The string containing the space-delimited arguments.</param>
        /// <param name="numExpectedArgs">The expected number of arguments. Must be zero or positive.</param>
        /// <param name="argsOut">The parsed arguments. Will be empty if parsing fails.</param>
        /// <returns></returns>
        private bool ParseArgs(string argsIn, int numExpectedArgs, out int[] argsOut)
        {
            argsOut = new int[numExpectedArgs];

            if (numExpectedArgs == 0)
            {
                if (!string.IsNullOrEmpty(argsIn))
                {
                    Log.Warn(Strings.GetString("warnArgsBadLength"));
                }
                return true;
            }

            if (!string.IsNullOrEmpty(argsIn))
            {
                string[] args_str = argsIn.Split(' ');
                if (args_str.Length == numExpectedArgs)
                {
                    for (int i = 0; i < numExpectedArgs; i++)
                    {
                        if (int.TryParse(args_str[i], out int value))
                        {
                            argsOut[i] = value;
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

        private int ReadECByte(int clientId, string args)
        {
            if (ParseArgs(args, 1, out int[] pArgs))
            {
                if (EC.AcquireLock(1000))
                {
                    Log.Debug(Strings.GetString("svcECReading"), $"{pArgs[0]:X}");
                    bool success = LogECReadByteError((byte)pArgs[0], out byte value);
                    EC.ReleaseLock();

                    if (success)
                    {
                        Log.Debug(Strings.GetString("svcECReadSuccess"), $"{pArgs[1]:X}", $"{value:X}");
                    }

                    ServiceResponse response = success
                        ? new(Response.ReadResult, $"{pArgs[0]} {value}")
                        : new(Response.Error, $"{(int)Command.ReadECByte}");

                    IPCServer.PushMessage(response, clientId);
                    return 0;
                }
                return 3;
            }
            return 2;
        }

        private int WriteECByte(int clientId, string args)
        {
            if (ParseArgs(args, 2, out int[] pArgs))
            {
                if (EC.AcquireLock(1000))
                {
                    Log.Debug(Strings.GetString("svcECWriting"), $"{pArgs[1]:X}", $"{pArgs[0]:X}");
                    bool success = LogECWriteError((byte)pArgs[0], (byte)pArgs[1]);
                    EC.ReleaseLock();

                    if (success)
                    {
                        Log.Debug(Strings.GetString("svcECWriteSuccess"), $"{pArgs[0]:X}");
                    }

                    ServiceResponse response = success
                        ? new(Response.Success, $"{(int)Command.WriteECByte}")
                        : new(Response.Error, $"{(int)Command.WriteECByte}");

                    IPCServer.PushMessage(response, clientId);
                    return 0;
                }
                return 3;
            }
            return 2;
        }

        private int GetFanSpeed(int clientId, string args)
        {
            if (!ConfigLoaded)
            {
                return 0;
            }

            if (ParseArgs(args, 1, out int[] pArgs))
            {
                if (EC.AcquireLock(1000))
                {
                    FanConf cfg = Config.FanConfs[pArgs[0]];
                    bool success = LogECReadByteError(cfg.SpeedReadReg, out byte speed);
                    EC.ReleaseLock();

                    ServiceResponse response = success
                        ? new(Response.FanSpeed, $"{speed}")
                        : new(Response.Error, $"{(int)Command.GetFanSpeed}");

                    IPCServer.PushMessage(response, clientId);
                    return 0;
                }
                return 3;
            }
            return 2;
        }

        private int GetFanRPM(int clientId, string args)
        {
            if (!ConfigLoaded)
            {
                return 0;
            }

            if (ParseArgs(args, 1, out int[] pArgs))
            {
                FanConf cfg = Config.FanConfs[pArgs[0]];

                if (cfg.RPMConf is not null)
                {
                    if (EC.AcquireLock(1000))
                    {
                        bool success;
                        ushort rpmValue;
                        if (cfg.RPMConf.Is16Bit)
                        {
                            success = LogECReadWordError(cfg.RPMConf.ReadReg, out rpmValue, cfg.RPMConf.IsBigEndian);
                        }
                        else
                        {
                            success = LogECReadByteError(cfg.RPMConf.ReadReg, out byte rpmValByte);
                            rpmValue = rpmValByte;
                        }
                        EC.ReleaseLock();

                        ServiceResponse response;
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
                            response = new(Response.FanRPM, $"{rpm}");
                        }
                        else
                        {
                            response = new(Response.FanRPM, $"{(int)Command.GetFanRPM}");
                        }
                        IPCServer.PushMessage(response, clientId);
                        return 0;
                    }
                    return 3;
                }
                return 0;
            }
            return 2;
        }

        private int GetTemp(int clientId, string args)
        {
            if (!ConfigLoaded)
            {
                return 0;
            }

            if (ParseArgs(args, 1, out int[] pArgs))
            {
                if (EC.AcquireLock(1000))
                {
                    FanConf cfg = Config.FanConfs[pArgs[0]];
                    bool success = LogECReadByteError(cfg.TempReadReg, out byte temp);
                    EC.ReleaseLock();

                    ServiceResponse response = success
                        ? new(Response.Temp, $"{temp}")
                        : new(Response.Error, $"{(int)Command.GetTemp}");

                    IPCServer.PushMessage(response, clientId);
                    return 0;
                }
                return 3;
            }
            return 2;
        }

        private int SetFullBlast(int clientId, string args)
        {
            if (ConfigLoaded && Config.FullBlastConf is not null)
            {
                if (ParseArgs(args, 1, out int[] pArgs))
                {
                    if (EC.AcquireLock(500))
                    {
                        if (!LogECReadByteError(Config.FullBlastConf.Reg, out byte value))
                        {
                            return 0;
                        }

                        bool success;
                        if (pArgs[0] == 1)
                        {
                            Log.Debug("Enabling Full Blast...");
                            value |= Config.FullBlastConf.Mask;
                        }
                        else
                        {
                            Log.Debug("Disabling Full Blast...");
                            value &= (byte)~Config.FullBlastConf.Mask;
                        }
                        success = LogECWriteError(Config.FullBlastConf.Reg, value);
                        EC.ReleaseLock();

                        ServiceResponse response = success
                            ? new(Response.Success, $"{(int)Command.FullBlast}")
                            : new(Response.Error, $"{(int)Command.FullBlast}");

                        IPCServer.PushMessage(response, clientId);
                        return 0;
                    }
                    return 3;
                }
                return 2;
            }
            return 0;
        }

        private int GetKeyLightBright(int clientId)
        {
            if (!ConfigLoaded || Config.KeyLightConf is null)
            {
                return 0;
            }

            Log.Debug(Strings.GetString("svcGetKeyLightBright"));
            if (EC.AcquireLock(500))
            {
                bool success = LogECReadByteError(Config.KeyLightConf.Reg, out byte value);
                EC.ReleaseLock();

                ServiceResponse response;
                if (success)
                {
                    int brightness = value - Config.KeyLightConf.MinVal;
                    response = new(Response.KeyLightBright, $"{brightness}");
                }
                else
                {
                    response = new(Response.Error, $"{(int)Command.GetKeyLightBright}");
                }
                IPCServer.PushMessage(response, clientId);
                return 0;
            }
            return 3;
        }

        private int SetKeyLightBright(int clientId, string args)
        {
            if (!ConfigLoaded || Config.KeyLightConf is null)
            {
                return 0;
            }

            if (ParseArgs(args, 1, out int[] pArgs))
            {
                Log.Debug(Strings.GetString("svcSetKeyLightBright", pArgs[0]));
                if (EC.AcquireLock(500))
                {
                    bool success = LogECWriteError(Config.KeyLightConf.Reg, (byte)(pArgs[0] + Config.KeyLightConf.MinVal));
                    EC.ReleaseLock();

                    ServiceResponse response = success
                        ? new(Response.Success, $"{Command.SetKeyLightBright}")
                        : new(Response.Error, $"{Command.SetKeyLightBright}");

                    IPCServer.PushMessage(response, clientId);
                    return 0;
                }
                return 3;
            }
            return 2;
        }

        private int FanCurveECToConf(int clientId)
        {
            if (!ConfigLoaded)
            {
                return 0;
            }

            Log.Debug("Getting fan curve from EC...");
            if (EC.AcquireLock(500))
            {
                for (int i = 0; i < Config.FanConfs.Length; i++)
                {
                    FanConf cfg = Config.FanConfs[i];

                    if (cfg.FanCurveConfs is null || cfg.FanCurveConfs.Length == 0)
                    {
                        cfg.FanCurveConfs = new FanCurveConf[1];
                        cfg.FanCurveConfs[0] = new()
                        {
                            Name = "Default",
                            Desc = "Default fan curve (auto-generated by YAMDCC)",
                            TempThresholds = new TempThreshold[cfg.FanCurveRegs.Length],
                        };
                    }

                    for (int j = 0; j < cfg.FanCurveRegs.Length; j++)
                    {
                        FanCurveConf curveCfg = cfg.FanCurveConfs[0];
                        if (curveCfg.TempThresholds[j] is null)
                        {
                            curveCfg.TempThresholds[j] = new();
                        }

                        if (_EC.ReadByte(cfg.FanCurveRegs[j], out byte value))
                        {
                            curveCfg.TempThresholds[j].FanSpeed = value;
                        }

                        if (j == 0)
                        {
                            curveCfg.TempThresholds[j].UpThreshold = 0;
                            curveCfg.TempThresholds[j].DownThreshold = 0;
                        }
                        else
                        {
                            if (_EC.ReadByte(cfg.UpThresholdRegs[j], out value))
                            {
                                curveCfg.TempThresholds[j].UpThreshold = value;
                            }
                            if (_EC.ReadByte(cfg.DownThresholdRegs[j], out value))
                            {
                                curveCfg.TempThresholds[j].DownThreshold = value;
                            }
                        }
                    }
                }
            }

            Log.Debug("Saving config...");
            Config.Save(Path.Combine(DataPath, "CurrentConfig.xml"));

            return 0;
        }
    }
}
