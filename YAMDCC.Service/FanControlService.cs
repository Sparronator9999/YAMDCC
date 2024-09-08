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
        #endregion

        /// <summary>
        /// Initialises a new instance of the <see cref="FanControlService"/> class.
        /// </summary>
        /// <param name="logger">The <see cref="Logger"/> instance to write logs to.</param>
        public FanControlService(Logger logger)
        {
            CanHandlePowerEvent = true;

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

        protected override void OnStop()
        {
            Log.Info(Strings.GetString("svcStopping"));

            // Stop the IPC server:
            Log.Debug("Stopping IPC server...");
            IPCServer.Stop();
            IPCServer.ClientConnected -= IPCClientConnect;
            IPCServer.ClientDisconnected -= IPCClientDisconnect;
            IPCServer.Error -= IPCServerError;

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
                    if (Config.RegConfs is not null && Config.RegConfs.Length > 0)
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
                    if (Config.ChargeLimitConf is not null)
                    {
                        Log.Debug("Writing charge limit configuration...");
                        byte value = (byte)(Config.ChargeLimitConf.MinVal + Config.ChargeLimitConf.CurVal);
                        if (!_EC.WriteByte(Config.ChargeLimitConf.Reg, value))
                        {
                            LogECWriteError(Config.ChargeLimitConf.Reg);
                        }
                    }

                    // Write the performance mode
                    if (Config.PerfModeConf is not null)
                    {
                        Log.Debug("Writing performance mode setting...");
                        byte value = Config.PerfModeConf.PerfModes[Config.PerfModeConf.ModeSel].Value;
                        if (!_EC.WriteByte(Config.PerfModeConf.Reg, value))
                        {
                            LogECWriteError(Config.PerfModeConf.Reg);
                        }
                    }

                    // Write the Win/Fn key swap setting
                    if (Config.KeySwapConf is not null)
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
                    bool success = _EC.ReadByte((byte)pArgs[0], out byte value);
                    EC.ReleaseLock();

                    ServiceResponse response;
                    if (success)
                    {
                        Log.Debug(Strings.GetString("svcECReadSuccess"), $"{pArgs[1]:X}", $"{value:X}");
                        response = new(Response.ReadResult, $"{pArgs[0]} {value}");
                    }
                    else
                    {
                        LogECReadError(pArgs[0]);
                        response = new(Response.Error, $"{(int)Command.ReadECByte}");
                    }
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
                    bool success = _EC.WriteByte((byte)pArgs[0], (byte)pArgs[1]);
                    EC.ReleaseLock();

                    ServiceResponse response;
                    if (success)
                    {
                        Log.Debug(Strings.GetString("svcECWriteSuccess"), $"{pArgs[0]:X}");
                        response = new(Response.Success, $"{(int)Command.WriteECByte}");
                    }
                    else
                    {
                        LogECWriteError(pArgs[0]);
                        response = new(Response.Error, $"{(int)Command.WriteECByte}");
                    }
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
                    bool success = _EC.ReadByte(cfg.SpeedReadReg, out byte speed);
                    EC.ReleaseLock();

                    ServiceResponse response;
                    if (success)
                    {
                        response = new(Response.FanSpeed, $"{speed}");
                    }
                    else
                    {
                        LogECReadError(pArgs[0]);
                        response = new(Response.Error, $"{(int)Command.GetFanSpeed}");
                    }
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
                            success = _EC.ReadWord(cfg.RPMConf.ReadReg, out rpmValue, cfg.RPMConf.IsBigEndian);
                        }
                        else
                        {
                            success = _EC.ReadByte(cfg.RPMConf.ReadReg, out byte rpmValByte);
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
                            LogECReadError(pArgs[0]);
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
                    bool success = _EC.ReadByte(cfg.TempReadReg, out byte temp);
                    EC.ReleaseLock();

                    ServiceResponse response;
                    if (success)
                    {
                        response = new(Response.Temp, $"{temp}");
                    }
                    else
                    {
                        LogECReadError(pArgs[0]);
                        response = new(Response.Error, $"{(int)Command.GetTemp}");
                    }
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

                        ServiceResponse response;
                        if (success)
                        {
                            response = new(Response.Success, $"{(int)Command.FullBlast}");
                        }
                        else
                        {
                            LogECReadError(Config.FullBlastConf.Reg);
                            response = new(Response.Error, $"{(int)Command.FullBlast}");
                        }
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

            Log.Debug("Getting keyboard backlight brightness...");
            if (EC.AcquireLock(500))
            {
                bool success = _EC.ReadByte(Config.KeyLightConf.Reg, out byte value);
                EC.ReleaseLock();

                ServiceResponse response;
                if (success)
                {
                    int brightness = value - Config.KeyLightConf.MinVal;
                    Log.Debug($"Keyboard backlight brightness is {brightness}");
                    response = new(Response.KeyLightBright, $"{brightness}");
                }
                else
                {
                    LogECReadError(Config.KeyLightConf.Reg);
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
                Log.Debug($"Setting keyboard backlight brightness to {pArgs[0]}...");
                if (EC.AcquireLock(500))
                {
                    bool success = _EC.WriteByte(Config.KeyLightConf.Reg, (byte)(pArgs[0] + Config.KeyLightConf.MinVal));
                    EC.ReleaseLock();

                    ServiceResponse response;
                    if (success)
                    {
                        response = new(Response.Success, $"{Command.SetKeyLightBright}");
                    }
                    else
                    {
                        LogECWriteError(Config.KeyLightConf.Reg);
                        response = new(Response.Error, $"{Command.SetKeyLightBright}");
                    }

                    IPCServer.PushMessage(response, clientId);
                    return 0;
                }
                return 3;
            }
            return 2;
        }
    }
}
