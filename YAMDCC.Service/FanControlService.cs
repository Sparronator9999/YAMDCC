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

using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Globalization;
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

            CooldownTimer.Elapsed += CooldownElapsed;

            // Set up IPC server
            Log.Info("Starting IPC server...");
            IPCServer.ClientConnected += IPCClientConnect;
            IPCServer.ClientDisconnected += IPCClientDisconnect;
            IPCServer.Error += IPCServerError;
            IPCServer.Start();

            Log.Info(Strings.GetString("svcStarted"));

            // Attempt to read default fan curve if it's pending:
            int rebootFlag = -1;
            try
            {
                StreamReader sr = new(Paths.ECtoConfPending);
                if (int.TryParse(sr.ReadToEnd(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int value))
                {
                    rebootFlag = value;
                }
                sr.Close();

                if (rebootFlag == 0)
                {
                    ECToConf();
                    File.Delete(Paths.ECtoConfPending);
                }
            }
            catch (FileNotFoundException) { }
            catch (DirectoryNotFoundException) { }

            // Apply the fan curve and charging threshold:
            ApplySettings();
        }

        private void CooldownElapsed(object sender, ElapsedEventArgs e)
        {
            CooldownTimer.Stop();
            Cooldown = false;
        }

        protected override void OnStop()
        {
            StopSvc();
        }

        protected override void OnShutdown()
        {
            int rebootFlag = -1;
            try
            {
                StreamReader sr = new(Paths.ECtoConfPending);
                try
                {
                    if (int.TryParse(sr.ReadToEnd(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int value))
                    {
                        rebootFlag = value;
                    }
                }
                finally
                {
                    sr.Close();
                }

                if (rebootFlag == 1)
                {
                    StreamWriter sw = new(Paths.ECtoConfPending);
                    try
                    {
                        sw.Write(0);
                    }
                    finally
                    {
                        sw.Close();
                    }
                }
            }
            catch (FileNotFoundException) { }
            catch (DirectoryNotFoundException) { }
            StopSvc();
        }

        private void StopSvc()
        {
            if (ExitCode == 0)
            {
                Log.Info(Strings.GetString("svcStopping"));

                // Stop the IPC server:
                Log.Info("Stopping IPC server...");
                IPCServer.Stop();
                IPCServer.ClientConnected -= IPCClientConnect;
                IPCServer.ClientDisconnected -= IPCClientDisconnect;
                IPCServer.Error -= IPCServerError;

                CooldownTimer.Elapsed -= CooldownElapsed;

                // Uninstall WinRing0 to keep things clean
                Log.Info(Strings.GetString("drvUnload"));
                _EC.UnloadDriver();

                Log.Info(Strings.GetString("svcStopped"));
            }
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
                        Log.Info(Strings.GetString("svcWake"));
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
            Log.Info(Strings.GetString("ipcConnect", e.Connection.ID));
        }

        private void IPCClientDisconnect(object sender, PipeConnectionEventArgs<ServiceCommand, ServiceResponse> e)
        {
            e.Connection.ReceiveMessage -= IPCClientMessage;
            Log.Info(Strings.GetString("ipcDC", e.Connection.ID));
        }

        private void IPCServerError(object sender, PipeErrorEventArgs<ServiceCommand, ServiceResponse> e)
        {
            Log.Error(Strings.GetString("ipcError", e.Connection.ID, e.Exception));
        }

        private void IPCClientMessage(object sender, PipeMessageEventArgs<ServiceCommand, ServiceResponse> e)
        {
            int error = 0;

            switch (e.Message.Command)
            {
                case Command.Nothing:
                    Log.Warn("Empty command received!");
                    break;
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
                    IPCServer.PushMessage(new ServiceResponse(
                        Response.Success, $"{(int)e.Message.Command}"), e.Connection.ID);
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
                    Log.Error(Strings.GetString("errBadCmd", e.Message));
                    break;
            }

            switch (error)
            {
                case 1:
                    IPCServer.PushMessage(new ServiceResponse(
                        Response.Error, $"{(int)e.Message.Command}"), e.Connection.ID);
                        break;
                case 2:
                    Log.Error(Strings.GetString("errOffendingCmd", e.Message.Command, e.Message.Arguments));
                    break;
                default:
                    break;
            }
        }
        #endregion

        private bool LogECReadByte(byte reg, out byte value)
        {
            Log.Debug(Strings.GetString("svcECReading", reg));
            bool success = _EC.ReadByte(reg, out value);
            if (success)
            {
                Log.Debug(Strings.GetString("svcECReadSuccess"), reg, value);
            }
            else
            {
                Log.Error(Strings.GetString("errECRead", reg, GetWin32Error(_EC.GetDriverError())));
            }
            return success;
        }

        private bool LogECReadWord(byte reg, out ushort value, bool bigEndian = false)
        {
            Log.Debug(Strings.GetString("svcECReading", reg));
            bool success = _EC.ReadWord(reg, out value, bigEndian);
            if (success)
            {
                Log.Debug(Strings.GetString("svcECReadSuccess"), reg, value);
            }
            else
            {
                Log.Error(Strings.GetString("errECRead", reg, GetWin32Error(_EC.GetDriverError())));
            }
            return success;
        }

        private bool LogECWriteByte(byte reg, byte value)
        {
            Log.Debug(Strings.GetString("svcECWriting", value, reg));
            bool success = _EC.WriteByte(reg, value);
            if (success)
            {
                Log.Debug(Strings.GetString("svcECWriteSuccess"), reg);
            }
            else
            {
                Log.Error(Strings.GetString("errECWrite", reg, GetWin32Error(_EC.GetDriverError())));
            }
            return success;
        }

        private void LoadConf()
        {
            Log.Info(Strings.GetString("cfgLoading"));

            string confPath = Paths.CurrentConfig;

            try
            {
                Config = YAMDCC_Config.Load(confPath);
            }
            catch (Exception ex)
            {
                if (ex is InvalidConfigException or InvalidOperationException)
                {
                    Log.Error(Strings.GetString("cfgInvalid"));
                }
                else if (ex is FileNotFoundException)
                {
                    Log.Warn(Strings.GetString("cfgNotFound"));
                }
                else
                {
                    throw;
                }
                ConfigLoaded = false;
                return;
            }

            ConfigLoaded = true;
            Log.Info(Strings.GetString("cfgLoaded"));
        }

        private void ApplySettings()
        {
            if (ConfigLoaded)
            {
                Log.Info(Strings.GetString("cfgApplying"));

                // Write custom register values, if configured:
                if (Config.RegConfs is not null && Config.RegConfs.Length > 0)
                {
                    for (int i = 0; i < Config.RegConfs.Length; i++)
                    {
                        RegConf cfg = Config.RegConfs[i];
                        Log.Info(Strings.GetString("svcWritingCustomRegs", i + 1, Config.RegConfs.Length));
                        LogECWriteByte(cfg.Reg, cfg.Value);
                    }
                }

                // Write the fan curve to the appropriate registers for each fan:
                for (int i = 0; i < Config.FanConfs.Length; i++)
                {
                    FanConf cfg = Config.FanConfs[i];
                    Log.Info(Strings.GetString("svcWritingFans", cfg.Name, i + 1, Config.FanConfs.Length));
                    FanCurveConf curveCfg = cfg.FanCurveConfs[cfg.CurveSel];

                    for (int j = 0; j < curveCfg.TempThresholds.Length; j++)
                    {
                        LogECWriteByte(cfg.FanCurveRegs[j], curveCfg.TempThresholds[j].FanSpeed);

                        if (j > 0)
                        {
                            LogECWriteByte(cfg.UpThresholdRegs[j - 1], curveCfg.TempThresholds[j].UpThreshold);

                            byte downT = (byte)(curveCfg.TempThresholds[j].UpThreshold - curveCfg.TempThresholds[j].DownThreshold);
                            LogECWriteByte(cfg.DownThresholdRegs[j - 1], downT);
                        }
                    }
                }

                // Write the charge threshold:
                if (Config.ChargeLimitConf is not null)
                {
                    Log.Info(Strings.GetString("svcWritingChgLim"));
                    byte value = (byte)(Config.ChargeLimitConf.MinVal + Config.ChargeLimitConf.CurVal);
                    LogECWriteByte(Config.ChargeLimitConf.Reg, value);
                }

                // Write the performance mode
                if (Config.PerfModeConf is not null)
                {
                    Log.Info(Strings.GetString("svcWritingPerfMode"));
                    byte value = Config.PerfModeConf.PerfModes[Config.PerfModeConf.ModeSel].Value;
                    LogECWriteByte(Config.PerfModeConf.Reg, value);
                }

                // Write the Win/Fn key swap setting
                if (Config.KeySwapConf is not null)
                {
                    Log.Info(Strings.GetString("svcWritingKeySwap"));
                    byte value = Config.KeySwapConf.Enabled
                        ? Config.KeySwapConf.OnVal
                        : Config.KeySwapConf.OffVal;

                    LogECWriteByte(Config.KeySwapConf.Reg, value);
                }
            }
        }

        /// <summary>
        /// Parse arguments from a given string.
        /// </summary>
        /// <param name="argsIn">
        /// The string containing the space-delimited arguments.
        /// </param>
        /// <param name="numExpectedArgs">
        /// The expected number of arguments. Must be zero or positive.
        /// </param>
        /// <param name="argsOut">
        /// The parsed arguments. Will be empty if parsing fails.
        /// </param>
        /// <returns>
        /// <c>true</c> if the arguments were parsed successfully,
        /// otherise <c>false</c>.
        /// </returns>
        private bool ParseArgs(string argsIn, int numExpectedArgs, out int[] argsOut)
        {
            argsOut = new int[numExpectedArgs];

            if (numExpectedArgs == 0)
            {
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
                if (LogECReadByte((byte)pArgs[0], out byte value))
                {
                    IPCServer.PushMessage(new ServiceResponse(
                        Response.ReadResult, $"{pArgs[0]} {value}"), clientId);
                    return 0;
                }
                return 1;
            }
            return 2;
        }

        private int WriteECByte(int clientId, string args)
        {
            if (ParseArgs(args, 2, out int[] pArgs))
            {
                if (LogECWriteByte((byte)pArgs[0], (byte)pArgs[1]))
                {
                    IPCServer.PushMessage(new ServiceResponse(
                        Response.Success, $"{(int)Command.WriteECByte}"), clientId);
                    return 0;
                }
                return 1;
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
                FanConf cfg = Config.FanConfs[pArgs[0]];

                if (LogECReadByte(cfg.SpeedReadReg, out byte speed))
                {
                    IPCServer.PushMessage(new ServiceResponse(
                        Response.FanSpeed, $"{speed}"), clientId);
                    return 0;
                }
                return 1;
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

                if (cfg.RPMConf is null)
                {
                    return 0;
                }

                bool success;
                ushort rpmValue;
                if (cfg.RPMConf.Is16Bit)
                {
                    success = LogECReadWord(cfg.RPMConf.ReadReg, out rpmValue, cfg.RPMConf.IsBigEndian);
                }
                else
                {
                    success = LogECReadByte(cfg.RPMConf.ReadReg, out byte rpmValByte);
                    rpmValue = rpmValByte;
                }

                if (success)
                {
                    int rpm;
#pragma warning disable IDE0045 // Supress "if statement can be simplified" suggestion
                    if (cfg.RPMConf.Invert)
                    {
                        rpm = cfg.RPMConf.DivideByMult
                            ? cfg.RPMConf.RPMMult / rpmValue
                            : 1 / (rpmValue * cfg.RPMConf.RPMMult);
                    }
                    else
                    {
                        rpm = cfg.RPMConf.DivideByMult
                            ? rpmValue / cfg.RPMConf.RPMMult
                            : rpmValue * cfg.RPMConf.RPMMult;
                    }
#pragma warning restore IDE0045
                    IPCServer.PushMessage(new ServiceResponse(
                        Response.FanRPM, $"{rpm}"), clientId);
                    return 0;
                }
                return 1;
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
                FanConf cfg = Config.FanConfs[pArgs[0]];

                if (LogECReadByte(cfg.TempReadReg, out byte temp))
                {
                    IPCServer.PushMessage(new ServiceResponse(
                        Response.Temp, $"{temp}"), clientId);
                    return 0;
                }
                return 1;
            }
            return 2;
        }

        private int SetFullBlast(int clientId, string args)
        {
            if (!ConfigLoaded || Config.FullBlastConf is null)
            {
                return 0;
            }

            if (ParseArgs(args, 1, out int[] pArgs))
            {
                if (LogECReadByte(Config.FullBlastConf.Reg, out byte value))
                {
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

                    if (LogECWriteByte(Config.FullBlastConf.Reg, value))
                    {
                        IPCServer.PushMessage(new ServiceResponse(
                            Response.Success, $"{(int)Command.FullBlast}"), clientId);
                        return 0;
                    }
                }
                return 1;
            }
            return 2;
        }

        private int GetKeyLightBright(int clientId)
        {
            if (!ConfigLoaded || Config.KeyLightConf is null)
            {
                return 0;
            }

            Log.Debug(Strings.GetString("svcGetKeyLightBright"));

            if (LogECReadByte(Config.KeyLightConf.Reg, out byte value) &&
                value >= Config.KeyLightConf.MinVal && value <= Config.KeyLightConf.MaxVal)
            {
                int brightness = value - Config.KeyLightConf.MinVal;

                IPCServer.PushMessage(new ServiceResponse(
                    Response.KeyLightBright, $"{brightness}"), clientId);
                return 0;
            }
            return 1;
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

                if (LogECWriteByte(Config.KeyLightConf.Reg, (byte)(pArgs[0] + Config.KeyLightConf.MinVal)))
                {
                    IPCServer.PushMessage(new ServiceResponse(
                        Response.Success, $"{Command.SetKeyLightBright}"), clientId);
                    return 0;
                }
                return 1;
            }
            return 2;
        }

        private void ECToConf()
        {
            if (!ConfigLoaded)
            {
                return;
            }

            try
            {
                Log.Info(Strings.GetString("svcReadingModel"));

                string pcManufacturer = GetPCManufacturer(),
                    pcModel = GetPCModel();

                if (string.IsNullOrEmpty(pcManufacturer))
                {
                    Log.Error(Strings.GetString("errReadManufacturer"));
                }
                else
                {
                    Config.Manufacturer = pcManufacturer.Trim();
                }

                if (string.IsNullOrEmpty(pcModel))
                {
                    Log.Error(Strings.GetString("errReadModel"));
                }
                else
                {
                    Config.Model = pcModel.Trim();
                }

                for (int i = 0; i < Config.FanConfs.Length; i++)
                {
                    Log.Info(Strings.GetString("svcReadingCurves", i + 1, Config.FanConfs.Length));

                    FanConf cfg = Config.FanConfs[i];
                    FanCurveConf curveCfg = cfg.FanCurveConfs[0];

                    // reset first fan curve config name and description
                    curveCfg.Name = "Default";
                    curveCfg.Desc = Strings.GetString("confDefaultDesc");

                    for (int j = 0; j < cfg.FanCurveRegs.Length; j++)
                    {
                        if (curveCfg.TempThresholds[j] is null)
                        {
                            curveCfg.TempThresholds[j] = new();
                        }

                        if (LogECReadByte(cfg.FanCurveRegs[j], out byte value))
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
                            if (LogECReadByte(cfg.UpThresholdRegs[j - 1], out value))
                            {
                                curveCfg.TempThresholds[j].UpThreshold = value;
                            }
                            if (LogECReadByte(cfg.DownThresholdRegs[j - 1], out value))
                            {
                                curveCfg.TempThresholds[j].DownThreshold = (byte)(curveCfg.TempThresholds[j].UpThreshold - value);
                            }
                        }
                    }
                }

                Log.Info("Saving config...");
                Config.Save(Paths.CurrentConfig);

                FileStream fs = File.Create(Paths.ECToConfSuccess);
                fs.Close();
            }
            catch
            {
                FileStream fs = File.Create(Paths.ECToConfFail);
                fs.Close();
            }
        }

        /// <summary>
        /// Gets the computer model name from registry.
        /// </summary>
        /// <returns>
        /// The computer model if the function succeeds,
        /// otherwise <c>null</c>.
        /// </returns>
        private static string GetPCModel()
        {
            return GetBIOSRegValue("SystemProductName");
        }

        /// <summary>
        /// Gets the computer manufacturer from registry.
        /// </summary>
        /// <returns>
        /// The computer manufacturer if the function succeeds,
        /// otherwise <c>null</c>.
        /// </returns>
        private static string GetPCManufacturer()
        {
            return GetBIOSRegValue("SystemManufacturer");
        }

        private static string GetBIOSRegValue(string name)
        {
            RegistryKey biosKey = Registry.LocalMachine.OpenSubKey(@"HARDWARE\DESCRIPTION\System\BIOS");
            try
            {
                return (string)biosKey?.GetValue(name, null);
            }
            finally
            {
                biosKey?.Close();
            }
        }

        private static string GetWin32Error(int error)
        {
            return new Win32Exception(error).Message;
        }
    }
}
