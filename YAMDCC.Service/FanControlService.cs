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
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Pipes;
using System.ServiceProcess;
using System.Timers;
using YAMDCC.Common;
using YAMDCC.Config;
using YAMDCC.ECAccess;
using YAMDCC.IPC;
using YAMDCC.Logs;

namespace YAMDCC.Service;

internal sealed class FanControlService : ServiceBase
{
    #region Fields

    /// <summary>
    /// The currently loaded YAMDCC config.
    /// </summary>
    private YAMDCC_Config Config;

    /// <summary>
    /// The named message pipe server that YAMDCC connects to.
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
    /// <param name="logger">
    /// The <see cref="Logger"/> instance to write logs to.
    /// </param>
    public FanControlService(Logger logger)
    {
        CanHandlePowerEvent = true;
        CanShutdown = true;

        Log = logger;
        _EC = new EC();

        PipeSecurity security = new();
        // use SDDL descriptor since not everyone uses english Windows.
        // the SDDL descriptor should be roughly equivalent to the old
        // behaviour (commented out below):
        // security.AddAccessRule(new PipeAccessRule(
        //     "Administrators", PipeAccessRights.ReadWrite, AccessControlType.Allow));
        security.SetSecurityDescriptorSddlForm("O:BAG:SYD:(A;;GA;;;SY)(A;;GRGW;;;BA)");

        IPCServer = new NamedPipeServer<ServiceCommand, ServiceResponse>("YAMDCC-Server", security);
    }

    #region Events
    protected override void OnStart(string[] args)
    {
        Log.Info(Strings.GetString("svcStarting"));

        // Load the service config.
        bool confLoaded = LoadConf();

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
        if (CommonConfig.GetECtoConfState() == ECtoConfState.PostReboot)
        {
            ECtoConf();
        }

        // Apply the fan curve and charging threshold:
        if (confLoaded)
        {
            ApplyConf();
        }
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
        if (CommonConfig.GetECtoConfState() == ECtoConfState.PendingReboot)
        {
            CommonConfig.SetECtoConfState(ECtoConfState.PostReboot);
        }
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
                    ApplyConf();
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
        bool parseSuccess = false,
            cmdSuccess = false,
            sendSuccessMsg = true;

        if (ParseArgs(e.Message.Arguments, out int[] args))
        {
            switch (e.Message.Command)
            {
                case Command.Nothing:
                    Log.Warn("Empty command received!");
                    return;
                case Command.GetVersion:
                    IPCServer.PushMessage(new ServiceResponse(
                        Response.Version, Utils.GetRevision()), e.Connection.ID);
                    return;
                case Command.ReadECByte:
                    if (args.Length == 1)
                    {
                        parseSuccess = true;
                        sendSuccessMsg = false;
                        cmdSuccess = LogECReadByte((byte)args[0], out byte value);
                        if (cmdSuccess)
                        {
                            IPCServer.PushMessage(new ServiceResponse(
                                Response.ReadResult, $"{args[0]} {value}"), e.Connection.ID);
                        }
                    }
                    break;
                case Command.WriteECByte:
                    if (args.Length == 2)
                    {
                        parseSuccess = true;
                        cmdSuccess = LogECWriteByte((byte)args[0], (byte)args[1]);
                    }
                    break;
                case Command.GetFanSpeed:
                    if (args.Length == 1)
                    {
                        parseSuccess = true;
                        sendSuccessMsg = false;
                        cmdSuccess = GetFanSpeed(e.Connection.ID, args[0]);
                    }
                    break;
                case Command.GetFanRPM:
                    if (args.Length == 1)
                    {
                        parseSuccess = true;
                        sendSuccessMsg = false;
                        cmdSuccess = GetFanRPM(e.Connection.ID, args[0]);
                    }
                    break;
                case Command.GetTemp:
                    if (args.Length == 1)
                    {
                        parseSuccess = true;
                        sendSuccessMsg = false;
                        cmdSuccess = GetTemp(e.Connection.ID, args[0]);
                    }
                    break;
                case Command.ApplyConfig:
                    parseSuccess = true;
                    cmdSuccess = LoadConf() && ApplyConf();
                    break;
                case Command.FullBlast:
                    if (args.Length == 1)
                    {
                        parseSuccess = true;
                        cmdSuccess = SetFullBlast(args[0] == 1);
                    }
                    break;
                case Command.GetKeyLightBright:
                    parseSuccess = true;
                    sendSuccessMsg = false;
                    cmdSuccess = GetKeyLight(e.Connection.ID);
                    break;
                case Command.SetKeyLightBright:
                    if (args.Length == 1)
                    {
                        parseSuccess = true;
                        cmdSuccess = SetKeyLight((byte)args[0]);
                    }
                    break;
                default:    // Unknown command
                    Log.Error(Strings.GetString("errBadCmd", e.Message));
                    return;
            }
        }
        else
        {
            Log.Error(Strings.GetString("errArgsBadType"));
            Log.Error(Strings.GetString("errOffendingCmd", e.Message.Command, e.Message.Arguments));
        }

        if (!cmdSuccess)
        {
            if (!parseSuccess)
            {
                Log.Error(Strings.GetString("errArgsBadLen"));
                Log.Error(Strings.GetString("errOffendingCmd", e.Message.Command, e.Message.Arguments));
            }
            IPCServer.PushMessage(new ServiceResponse(
                Response.Error, $"{(int)e.Message.Command}"), e.Connection.ID);
        }
        else if (sendSuccessMsg)
        {
            IPCServer.PushMessage(new ServiceResponse(
                Response.Success, $"{(int)e.Message.Command}"), e.Connection.ID);
        }
    }
    #endregion

    private bool LogECReadByte(byte reg, out byte value)
    {
        bool success = _EC.ReadByte(reg, out value);
        if (success)
        {
            Log.Debug(Strings.GetString("svcECRead", reg, value));
        }
        else
        {
            Log.Error(Strings.GetString("errECRead", reg, GetWin32Error(_EC.GetDriverError())));
        }
        return success;
    }

    private bool LogECReadWord(byte reg, out ushort value, bool bigEndian)
    {
        bool success = _EC.ReadWord(reg, out value, bigEndian);
        if (success)
        {
            Log.Debug(Strings.GetString("svcECRead", reg, value));
        }
        else
        {
            Log.Error(Strings.GetString("errECRead", reg, GetWin32Error(_EC.GetDriverError())));
        }
        return success;
    }

    private bool LogECWriteByte(byte reg, byte value)
    {
        bool success = _EC.WriteByte(reg, value);
        if (success)
        {
            Log.Debug(Strings.GetString("svcECWrote", reg));
        }
        else
        {
            Log.Error(Strings.GetString("errECWrite", reg, GetWin32Error(_EC.GetDriverError())));
        }
        return success;
    }

    private bool LoadConf()
    {
        Log.Info(Strings.GetString("cfgLoading"));

        try
        {
            Config = YAMDCC_Config.Load(Paths.CurrentConf);
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
            Config = null;
            return false;
        }

        Log.Info(Strings.GetString("cfgLoaded"));
        return true;
    }

    private bool ApplyConf()
    {
        if (Config is null)
        {
            return false;
        }

        Log.Info(Strings.GetString("cfgApplying"));
        bool success = true;

        // Write custom register values, if configured:
        if (Config.RegConfs?.Count > 0)
        {
            for (int i = 0; i < Config.RegConfs.Count; i++)
            {
                RegConf cfg = Config.RegConfs[i];
                Log.Info(Strings.GetString("svcWriteRegConfs", i + 1, Config.RegConfs.Count));
                if (!LogECWriteByte(cfg.Reg, cfg.Enabled ? cfg.OnVal : cfg.OffVal))
                {
                    success = false;
                }
            }
        }

        // Write the fan curve to the appropriate registers for each fan:
        for (int i = 0; i < Config.FanConfs.Count; i++)
        {
            FanConf cfg = Config.FanConfs[i];
            Log.Info(Strings.GetString("svcWriteFanConfs", cfg.Name, i + 1, Config.FanConfs.Count));
            FanCurveConf curveCfg = cfg.FanCurveConfs[cfg.CurveSel];

            for (int j = 0; j < curveCfg.TempThresholds.Count; j++)
            {
                if (!LogECWriteByte(cfg.FanCurveRegs[j], curveCfg.TempThresholds[j].FanSpeed))
                {
                    success = false;
                }
                if (j > 0)
                {
                    if (!LogECWriteByte(cfg.UpThresholdRegs[j - 1], curveCfg.TempThresholds[j].UpThreshold))
                    {
                        success = false;
                    }
                    byte downT = (byte)(curveCfg.TempThresholds[j].UpThreshold - curveCfg.TempThresholds[j].DownThreshold);
                    if (!LogECWriteByte(cfg.DownThresholdRegs[j - 1], downT))
                    {
                        success = false;
                    }
                }
            }
        }

        // Write the charge threshold:
        if (Config.ChargeLimitConf is not null)
        {
            Log.Info(Strings.GetString("svcWriteChgLim"));
            byte value = (byte)(Config.ChargeLimitConf.MinVal + Config.ChargeLimitConf.CurVal);
            if (!LogECWriteByte(Config.ChargeLimitConf.Reg, value))
            {
                success = false;
            }
        }

        // Write the performance mode
        if (Config.PerfModeConf is not null)
        {
            Log.Info(Strings.GetString("svcWritePerfMode"));
            byte value = Config.PerfModeConf.PerfModes[Config.PerfModeConf.ModeSel].Value;
            if (!LogECWriteByte(Config.PerfModeConf.Reg, value))
            {
                success = false;
            }
        }

        // Write the fan mode
        if (Config.FanModeConf is not null)
        {
            Log.Info(Strings.GetString("svcWriteFanMode"));
            byte value = Config.FanModeConf.FanModes[Config.FanModeConf.ModeSel].Value;
            if (!LogECWriteByte(Config.FanModeConf.Reg, value))
            {
                success = false;
            }
        }

        // Write the Win/Fn key swap setting
        if (Config.KeySwapConf is not null)
        {
            Log.Info(Strings.GetString("svcWriteKeySwap"));
            byte value = Config.KeySwapConf.Enabled
                ? Config.KeySwapConf.OnVal
                : Config.KeySwapConf.OffVal;

            if (!LogECWriteByte(Config.KeySwapConf.Reg, value))
            {
                success = false;
            }
        }
        return success;
    }

    /// <summary>
    /// Parse arguments from a given string.
    /// </summary>
    /// <param name="argsIn">
    /// The string containing the space-delimited arguments.
    /// </param>
    /// <param name="argsOut">
    /// The parsed arguments. Will be empty if parsing fails.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the arguments were
    /// parsed successfully, otherise <see langword="false"/>.
    /// </returns>
    private static bool ParseArgs(string argsIn, out int[] argsOut)
    {
        if (string.IsNullOrEmpty(argsIn))
        {
            argsOut = [];
        }
        else
        {
            string[] args_str = argsIn.Split(' ');
            argsOut = new int[args_str.Length];

            for (int i = 0; i < args_str.Length; i++)
            {
                if (!int.TryParse(args_str[i], out argsOut[i]))
                {
                    return false;
                }
            }
        }
        return true;
    }

    private bool GetFanSpeed(int clientId, int fan)
    {
        if (Config is null)
        {
            return false;
        }

        FanConf cfg = Config.FanConfs[fan];

        if (LogECReadByte(cfg.SpeedReadReg, out byte speed))
        {
            IPCServer.PushMessage(new ServiceResponse(
                Response.FanSpeed, $"{speed}"), clientId);
            return true;
        }
        return false;
    }

    private bool GetFanRPM(int clientId, int fan)
    {
        if (Config?.FanConfs[fan].RPMConf is null)
        {
            return false;
        }

        FanConf cfg = Config.FanConfs[fan];

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
            float rpm = 0;
            if (rpmValue > 0)
            {
                rpm = cfg.RPMConf.DivideByMult
                    ? (float)rpmValue / cfg.RPMConf.RPMMult
                    : (float)rpmValue * cfg.RPMConf.RPMMult;

                if (cfg.RPMConf.Invert)
                {
                    rpm = 1 / rpm;
                }
            }
            IPCServer.PushMessage(new ServiceResponse(
                Response.FanRPM, $"{(int)rpm}"), clientId);
            return true;
        }
        return false;
    }

    private bool GetTemp(int clientId, int fan)
    {
        if (Config is null)
        {
            return false;
        }

        FanConf cfg = Config.FanConfs[fan];

        if (LogECReadByte(cfg.TempReadReg, out byte temp))
        {
            IPCServer.PushMessage(new ServiceResponse(
                Response.Temp, $"{temp}"), clientId);
            return true;
        }
        return false;
    }

    private bool SetFullBlast(bool enable)
    {
        if (Config?.FullBlastConf is null)
        {
            return false;
        }

        if (LogECReadByte(Config.FullBlastConf.Reg, out byte value))
        {
            if (enable)
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
                return true;
            }
        }
        return false;
    }

    private bool GetKeyLight(int clientId)
    {
        if (Config?.KeyLightConf is null)
        {
            return false;
        }

        Log.Debug(Strings.GetString("svcGetKeyLight"));

        if (LogECReadByte(Config.KeyLightConf.Reg, out byte value) &&
            value >= Config.KeyLightConf.MinVal && value <= Config.KeyLightConf.MaxVal)
        {
            int brightness = value - Config.KeyLightConf.MinVal;

            IPCServer.PushMessage(new ServiceResponse(
                Response.KeyLightBright, $"{brightness}"), clientId);
            return true;
        }
        return false;
    }

    private bool SetKeyLight(byte brightness)
    {
        if (Config?.KeyLightConf is null)
        {
            return false;
        }

        Log.Debug(Strings.GetString("svcSetKeyLight", brightness));

        return LogECWriteByte(Config.KeyLightConf.Reg, (byte)(brightness + Config.KeyLightConf.MinVal));
    }

    private bool ECtoConf()
    {
        if (Config is null)
        {
            return false;
        }

        try
        {
            Log.Info(Strings.GetString("svcReadModel"));

            string pcManufacturer = Utils.GetPCManufacturer(),
                pcModel = Utils.GetPCModel();

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

            for (int i = 0; i < Config.FanConfs.Count; i++)
            {
                Log.Info(Strings.GetString("svcReadProfs", i + 1, Config.FanConfs.Count));

                FanConf cfg = Config.FanConfs[i];

                // look for an already existing Default fan profile
                FanCurveConf curveCfg = null;
                for (int j = 0; j < cfg.FanCurveConfs.Count; j++)
                {
                    if (cfg.FanCurveConfs[j].Name == "Default")
                    {
                        curveCfg = cfg.FanCurveConfs[j];
                    }
                }

                // there isn't already a Default fan profile in this config,
                // make one and insert it at the start
                if (curveCfg is null)
                {
                    curveCfg = new()
                    {
                        Name = "Default",
                        TempThresholds = new List<TempThreshold>(cfg.FanCurveRegs.Length),
                    };
                    cfg.FanCurveConfs.Insert(0, curveCfg);
                    cfg.CurveSel++;
                }

                // reset first fan curve config description
                curveCfg.Desc = Strings.GetString("DefaultDesc");

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
            Config.Save(Paths.CurrentConf);

            CommonConfig.SetECtoConfState(ECtoConfState.Success);
            return true;
        }
        catch
        {
            CommonConfig.SetECtoConfState(ECtoConfState.Fail);
            return false;
        }
    }

    private static string GetWin32Error(int error)
    {
        return new Win32Exception(error).Message;
    }
}
