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
using System.Globalization;
using System.IO;
using System.IO.Pipes;
using System.ServiceProcess;
using System.Timers;
using YAMDCC.Common;
using YAMDCC.Common.Configs;
using YAMDCC.Common.Logs;
using YAMDCC.ECAccess;
using YAMDCC.IPC;

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

    private readonly Timer CooldownTimer = new(1000);

    private EcInfo EcInfo;
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

        CooldownTimer.Elapsed += new ElapsedEventHandler(CooldownElapsed);

        IPCServer = new NamedPipeServer<ServiceCommand, ServiceResponse>("YAMDCC-Server", security);
        IPCServer.ClientConnected += new EventHandler<PipeConnectionEventArgs<ServiceCommand, ServiceResponse>>(IPCClientConnect);
        IPCServer.ClientDisconnected += new EventHandler<PipeConnectionEventArgs<ServiceCommand, ServiceResponse>>(IPCClientDisconnect);
        IPCServer.Error += new EventHandler<PipeErrorEventArgs<ServiceCommand, ServiceResponse>>(IPCServerError);
    }

    #region Events
    protected override void OnStart(string[] args)
    {
        try
        {
            Log.Info(Strings.GetString("svcStarting"));

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

            // Load the last applied YAMDCC config.
            bool confLoaded = LoadConf();

            // Set up IPC server
            Log.Info("Starting IPC server...");
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
        catch (Exception ex)
        {
            Log.Fatal(Strings.GetString("svcException", ex));
            throw;
        }
    }

    private void CooldownElapsed(object sender, ElapsedEventArgs e)
    {
        CooldownTimer.Stop();
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
        Log.Info(Strings.GetString("svcStopping"));

        // Stop the IPC server:
        Log.Info("Stopping IPC server...");
        IPCServer.Stop();

        // Uninstall WinRing0 to keep things clean
        Log.Info(Strings.GetString("drvUnload"));
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
                if (!CooldownTimer.Enabled)
                {
                    // Re-apply the fan curve after waking up from sleep:
                    Log.Info(Strings.GetString("svcWake"));
                    ApplyConf();
                    CooldownTimer.Start();
                }
                break;
        }
        return true;
    }

    private void IPCClientConnect(object sender, PipeConnectionEventArgs<ServiceCommand, ServiceResponse> e)
    {
        e.Connection.ReceiveMessage += new EventHandler<PipeMessageEventArgs<ServiceCommand, ServiceResponse>>(IPCClientMessage);
        Log.Info(Strings.GetString("ipcConnect", e.Connection.ID));
    }

    private void IPCClientDisconnect(object sender, PipeConnectionEventArgs<ServiceCommand, ServiceResponse> e)
    {
        e.Connection.ReceiveMessage -= new EventHandler<PipeMessageEventArgs<ServiceCommand, ServiceResponse>>(IPCClientMessage);
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

        Command cmd = e.Message.Command;
        object[] args = e.Message.Arguments;
        int id = e.Connection.ID;

        switch (cmd)
        {
            case Command.Nothing:
                Log.Warn("Empty command received!");
                return;
            case Command.GetServiceVer:
                IPCServer.PushMessage(new ServiceResponse(
                    Response.ServiceVer, Utils.GetRevision()), id);
                return;
            case Command.GetFirmVer:
            {
                parseSuccess = true;
                sendSuccessMsg = false;
                cmdSuccess = GetFirmVer(id);
                break;
            }
            case Command.ReadECByte:
            {
                if (args.Length == 1 && args[0] is byte reg)
                {
                    parseSuccess = true;
                    sendSuccessMsg = false;
                    cmdSuccess = LogECReadByte(reg, out byte value);
                    if (cmdSuccess)
                    {
                        IPCServer.PushMessage(new ServiceResponse(
                            Response.ReadResult, reg, value), id);
                    }
                }
                break;
            }
            case Command.WriteECByte:
            {
                if (args.Length == 2 && args[0] is byte reg && args[1] is byte value)
                {
                    parseSuccess = true;
                    cmdSuccess = LogECWriteByte(reg, value);
                }
                break;
            }
            case Command.GetFanSpeed:
            {
                if (args.Length == 1 && args[0] is int fan)
                {
                    parseSuccess = true;
                    sendSuccessMsg = false;
                    cmdSuccess = GetFanSpeed(id, fan);
                }
                break;
            }
            case Command.GetFanRPM:
            {
                if (args.Length == 1 && args[0] is int fan)
                {
                    parseSuccess = true;
                    sendSuccessMsg = false;
                    cmdSuccess = GetFanRPM(id, fan);
                }
                break;
            }
            case Command.GetTemp:
            {
                if (args.Length == 1 && args[0] is int fan)
                {
                    parseSuccess = true;
                    sendSuccessMsg = false;
                    cmdSuccess = GetTemp(id, fan);
                }
                break;
            }
            case Command.ApplyConf:
                parseSuccess = true;
                cmdSuccess = LoadConf() && ApplyConf();
                break;
            case Command.SetFullBlast:
            {
                if (args.Length == 1 && args[0] is bool enable)
                {
                    parseSuccess = true;
                    cmdSuccess = SetFullBlast(enable);
                }
                break;
            }
            case Command.GetKeyLightBright:
                parseSuccess = true;
                sendSuccessMsg = false;
                cmdSuccess = GetKeyLight(id);
                break;
            case Command.SetKeyLightBright:
            {
                if (args.Length == 1 && args[0] is byte brightness)
                {
                    parseSuccess = true;
                    cmdSuccess = SetKeyLight(brightness);
                }
                break;
            }
            case Command.ChangeFanProf:
            {
                if (args.Length == 1 && args[0] is int fanProf)
                {
                    parseSuccess = true;
                    foreach (FanConf cfg in Config.FanConfs)
                    {
                        cfg.CurveSel = fanProf;
                    }
                    cmdSuccess = ApplyConf();
                }
                break;
            }
            case Command.ChangePerfMode:
            {
                if (args.Length == 1 && args[0] is int perfMode)
                {
                    parseSuccess = true;
                    if (Config.PerfModeConf is not null)
                    {
                        Config.PerfModeConf.ModeSel = perfMode;
                        cmdSuccess = ApplyConf();
                    }
                }
                break;
            }
            default:    // Unknown command
                Log.Error(Strings.GetString("errBadCmd", cmd));
                break;
        }

        if (!cmdSuccess)
        {
            if (!parseSuccess)
            {
                Log.Error(Strings.GetString("errBadArgs", cmd, args));
            }
            IPCServer.PushMessage(new ServiceResponse(
                Response.Error, (int)cmd), id);
        }
        else if (sendSuccessMsg)
        {
            IPCServer.PushMessage(new ServiceResponse(
                Response.Success, (int)cmd), id);
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

    private bool LoadConf(int? clientID = null)
    {
        Log.Info(Strings.GetString("cfgLoading"));

        try
        {
            Config = YAMDCC_Config.Load(Paths.CurrentConf);
            Log.Info(Strings.GetString("cfgLoaded"));

            if (clientID is not null)
            {
                IPCServer?.PushMessage(new ServiceResponse(
                    Response.ConfLoaded, clientID.Value));
            }

            if (Config.FirmVerSupported)
            {
                EcInfo = new();
                if (_EC.ReadString(0xA0, 0xC, out string ecVer) && ecVer.Length == 0xC)
                {
                    EcInfo.Version = ecVer;
                    Log.Debug($"EC firmware version: {ecVer}");
                }
                if (_EC.ReadString(0xAC, 0x10, out string ecDate) && ecDate.Length == 0x10)
                {
                    try
                    {
                        string temp = $"{ecDate.Substring(4, 4)}-{ecDate.Substring(0, 2)}-{ecDate.Substring(2, 2)}" +
                    $"T{ecDate.Substring(8, 2).Replace(' ', '0')}:{ecDate.Substring(11, 2)}:{ecDate.Substring(14, 2)}";
                        EcInfo.Date = DateTime.ParseExact(temp, "s", CultureInfo.InvariantCulture);
                        Log.Debug($"EC firmware date: {EcInfo.Date:G}");
                    }
                    catch (FormatException ex)
                    {
                        Log.Error($"Failed to parse EC firmware date: {ex.Message}");
                        Log.Debug($"EC firmware date (raw): {ecDate}");
                    }
                }
            }

            return true;
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
            int numRegConfs = Config.RegConfs.Count;
            for (int i = 0; i < numRegConfs; i++)
            {
                RegConf cfg = Config.RegConfs[i];
                Log.Info(Strings.GetString("svcWriteRegConfs", i + 1, numRegConfs));
                if (!LogECWriteByte(cfg.Reg, cfg.Enabled ? cfg.OnVal : cfg.OffVal))
                {
                    success = false;
                }
            }
        }

        // Write the fan profile to the appropriate registers for each fan:
        int numFanConfs = Config.FanConfs.Count;
        for (int i = 0; i < numFanConfs; i++)
        {
            FanConf cfg = Config.FanConfs[i];
            Log.Info(Strings.GetString("svcWriteFanConfs", cfg.Name, i + 1, numFanConfs));

            FanCurveConf curveCfg = cfg.FanCurveConfs[cfg.CurveSel];
            for (int j = 0; j < curveCfg.TempThresholds.Count; j++)
            {
                TempThreshold t = curveCfg.TempThresholds[j];
                if (!LogECWriteByte(cfg.FanCurveRegs[j], t.FanSpeed))
                {
                    success = false;
                }
                if (j > 0)
                {
                    if (!LogECWriteByte(cfg.UpThresholdRegs[j - 1], t.UpThreshold))
                    {
                        success = false;
                    }
                    if (!LogECWriteByte(cfg.DownThresholdRegs[j - 1], (byte)(t.UpThreshold - t.DownThreshold)))
                    {
                        success = false;
                    }
                }
            }

            // Write the performance mode
            if (i == 0)
            {
                PerfModeConf pModeCfg = Config.PerfModeConf;
                if (pModeCfg is not null)
                {
                    Log.Info(Strings.GetString("svcWritePerfMode"));
                    int idx = curveCfg.PerfModeSel < 0
                        ? pModeCfg.ModeSel
                        : curveCfg.PerfModeSel;

                    if (!LogECWriteByte(pModeCfg.Reg, pModeCfg.PerfModes[idx].Value))
                    {
                        success = false;
                    }
                }
            }
        }

        // Write the charge threshold:
        ChargeLimitConf chgLimCfg = Config.ChargeLimitConf;
        if (chgLimCfg is not null)
        {
            Log.Info(Strings.GetString("svcWriteChgLim"));
            if (!LogECWriteByte(chgLimCfg.Reg, (byte)(chgLimCfg.MinVal + chgLimCfg.CurVal)))
            {
                success = false;
            }
        }

        // Write the fan mode
        FanModeConf fModeCfg = Config.FanModeConf;
        if (fModeCfg is not null)
        {
            Log.Info(Strings.GetString("svcWriteFanMode"));
            if (!LogECWriteByte(fModeCfg.Reg, fModeCfg.FanModes[fModeCfg.ModeSel].Value))
            {
                success = false;
            }
        }

        // Write the Win/Fn key swap setting
        KeySwapConf keySwapCfg = Config.KeySwapConf;
        if (keySwapCfg is not null)
        {
            Log.Info(Strings.GetString("svcWriteKeySwap"));
            if (!LogECWriteByte(keySwapCfg.Reg, keySwapCfg.Enabled
                ? keySwapCfg.OnVal
                : keySwapCfg.OffVal))
            {
                success = false;
            }
        }
        return success;
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
                Response.FanSpeed, fan, (int)speed), clientId);
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
        FanRPMConf rpmCfg = cfg.RPMConf;
        bool success;
        ushort rpmValue;

        if (rpmCfg.Is16Bit)
        {
            success = LogECReadWord(rpmCfg.ReadReg, out rpmValue, rpmCfg.IsBigEndian);
        }
        else
        {
            success = LogECReadByte(rpmCfg.ReadReg, out byte rpmValByte);
            rpmValue = rpmValByte;
        }

        if (success)
        {
            float rpm = 0;
            if (rpmValue > 0)
            {
                rpm = rpmCfg.DivideByMult
                    ? (float)rpmValue / rpmCfg.RPMMult
                    : (float)rpmValue * rpmCfg.RPMMult;

                if (rpmCfg.Invert)
                {
                    rpm = 1 / rpm;
                }
            }
            IPCServer.PushMessage(new ServiceResponse(
                Response.FanRPM, fan, (int)rpm), clientId);
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
                Response.Temp, fan, (int)temp), clientId);
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

        FullBlastConf fbCfg = Config.FullBlastConf;
        if (LogECReadByte(fbCfg.Reg, out byte value))
        {
            if (enable)
            {
                Log.Debug("Enabling Full Blast...");
                value |= fbCfg.Mask;
            }
            else
            {
                Log.Debug("Disabling Full Blast...");
                value &= (byte)~fbCfg.Mask;
            }

            if (LogECWriteByte(fbCfg.Reg, value))
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

        KeyLightConf klCfg = Config.KeyLightConf;
        if (LogECReadByte(klCfg.Reg, out byte value) &&
            value >= klCfg.MinVal && value <= klCfg.MaxVal)
        {
            int brightness = value - klCfg.MinVal;

            IPCServer.PushMessage(new ServiceResponse(
                Response.KeyLightBright, brightness), clientId);
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

        KeyLightConf klCfg = Config.KeyLightConf;
        byte value = (byte)(brightness + klCfg.MinVal);
        return value >= klCfg.MinVal && value <= klCfg.MaxVal &&
            LogECWriteByte(klCfg.Reg, value);
    }

    private bool GetFirmVer(int clientId)
    {
        if (Config is null || !Config.FirmVerSupported)
        {
            return false;
        }

        Log.Debug(Strings.GetString("svcGerFirmVer", clientId));
        IPCServer.PushMessage(new ServiceResponse(Response.FirmVer, EcInfo), clientId);
        return true;
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
                Config.Manufacturer = pcManufacturer;
            }

            if (string.IsNullOrEmpty(pcModel))
            {
                Log.Error(Strings.GetString("errReadModel"));
            }
            else
            {
                Config.Model = pcModel;
            }

            if (Config.FirmVerSupported)
            {
                Config.FirmVer = EcInfo.Version;
                Config.FirmDate = EcInfo.Date;
            }
            else
            {
                Config.FirmVer = null;
                Config.FirmDate = null;
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

                for (int j = 0; j < curveCfg.TempThresholds.Count; j++)
                {
                    curveCfg.TempThresholds[j] ??= new();
                    TempThreshold t = curveCfg.TempThresholds[j];

                    if (LogECReadByte(cfg.FanCurveRegs[j], out byte value))
                    {
                        t.FanSpeed = value;
                    }

                    if (j == 0)
                    {
                        t.UpThreshold = 0;
                        t.DownThreshold = 0;
                    }
                    else
                    {
                        if (LogECReadByte(cfg.UpThresholdRegs[j - 1], out value))
                        {
                            t.UpThreshold = value;
                        }
                        if (LogECReadByte(cfg.DownThresholdRegs[j - 1], out value))
                        {
                            t.DownThreshold = (byte)(t.UpThreshold - value);
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
