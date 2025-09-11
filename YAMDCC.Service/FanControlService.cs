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
using YAMDCC.Service.RegLayouts;

namespace YAMDCC.Service;

internal sealed class FanControlService : ServiceBase
{
    #region Fields

    /// <summary>
    /// The currently loaded YAMDCC config.
    /// </summary>
    private YamdccCfg Config;

    /// <summary>
    /// The named message pipe server that YAMDCC connects to.
    /// </summary>
    private readonly NamedPipeServer<ServiceCommand, ServiceResponse> IPCServer;

    private readonly Logger Log;

    private readonly EC _EC;

    private readonly Timer CooldownTimer = new(1000);

    private EcInfo EcInfo;

    private bool FullBlastEnabled;
    #endregion

    #region EC register definitions
    private IRegLayout RegLayout;

    private const byte CPU_FAN_SPEED_REG = 0x71;
    private const byte GPU_FAN_SPEED_REG = 0x89;
    private const byte CPU_TEMP_REG = 0x68;
    private const byte GPU_TEMP_REG = 0x80;

    private readonly byte[] CpuTupRegs = [0x6A, 0x6B, 0x6C, 0x6D, 0x6E, 0x6F];
    private readonly byte[] CpuTdownRegs = [0x7A, 0x7B, 0x7C, 0x7D, 0x7E, 0x7F];
    private readonly byte[] CpuFanSpeedRegs = [0x72, 0x73, 0x74, 0x75, 0x76, 0x77, 0x78];

    private readonly byte[] GpuTupRegs = [0x82, 0x83, 0x84, 0x85, 0x86, 0x87];
    private readonly byte[] GpuTdownRegs = [0x92, 0x93, 0x94, 0x95, 0x96, 0x97];
    private readonly byte[] GpuFanSpeedRegs = [0x8A, 0x8B, 0x8C, 0x8D, 0x8E, 0x8F, 0x90];
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

            // Attempt to read default fan profile if it's pending:
            if (CommonConfig.GetECtoConfState() == ECtoConfState.PostReboot)
            {
                ECtoConf();
            }

            // Apply the fan profiles and charging threshold:
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
        // disable Full Blast if it was enabled while running
        SetFullBlast(0);

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
                    // fan settings get reset on sleep/restart
                    FullBlastEnabled = false;
                    // Re-apply the fan profiles after waking up from sleep:
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
            case Command.ApplyConf:
                parseSuccess = true;
                cmdSuccess = LoadConf() && ApplyConf();
                break;
            case Command.SetFullBlast:
            {
                if (args.Length == 1 && args[0] is int enable)
                {
                    parseSuccess = true;
                    cmdSuccess = SetFullBlast(enable);
                }
                break;
            }
            case Command.GetFanSpeed:
            {
                if (args.Length == 1 && args[0] is bool gpuFan)
                {
                    parseSuccess = true;
                    sendSuccessMsg = false;
                    cmdSuccess = GetFanSpeed(id, gpuFan);
                }
                break;
            }
            case Command.GetFanRPM:
            {
                if (args.Length == 1 && args[0] is int fan)
                {
                    parseSuccess = true;
                    sendSuccessMsg = false;
                    cmdSuccess = false; //GetFanRPM(id, fan);
                }
                break;
            }
            case Command.GetTemp:
            {
                if (args.Length == 1 && args[0] is bool gpuFan)
                {
                    parseSuccess = true;
                    sendSuccessMsg = false;
                    cmdSuccess = GetTemp(id, gpuFan);
                }
                break;
            }
            case Command.GetKeyLightSupported:
                parseSuccess = true;
                sendSuccessMsg = false;
                cmdSuccess = GetKeyLightSupported(id);
                break;
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
            case Command.GetKeySwapSupported:
                parseSuccess = true;
                sendSuccessMsg = false;
                cmdSuccess = RegLayout is not null;
                if (cmdSuccess)
                {
                    IPCServer.PushMessage(new ServiceResponse(Response.KeySwapSupported,
                        RegLayout.KeySwapReg.HasValue));
                }
                break;
            case Command.SetKeySwap:
            {
                if (args.Length == 1 && args[0] is int enable)
                {
                    parseSuccess = true;
                    if (enable == -1)
                    {
                        Config.KeySwapEnabled = !Config.KeySwapEnabled;
                    }
                    else if (enable == 0)
                    {
                        Config.KeySwapEnabled = false;
                    }
                    else if (enable == 1)
                    {
                        Config.KeySwapEnabled = true;
                    }
                    else
                    {
                        parseSuccess = false;
                    }
                    if (parseSuccess)
                    {
                        cmdSuccess = SetWinFnSwap();
                    }
                }
                break;
            }
            case Command.SetFanProf:
            {
                if (args.Length == 1 && args[0] is int fanProf)
                {
                    parseSuccess = true;
                    // TODO: make nicer
                    foreach (FanConf cfg in new FanConf[] { Config.CpuFan, Config.GpuFan })
                    {
                        if (fanProf < 0)
                        {
                            if (Config.CpuFan.ProfSel >= cfg.FanProfs.Count - 1)
                            {
                                cfg.ProfSel = 0;
                            }
                            else
                            {
                                cfg.ProfSel++;
                            }
                        }
                        else
                        {
                            cfg.ProfSel = fanProf;
                        }
                    }
                    cmdSuccess = ApplyConf();
                }
                break;
            }
            case Command.SetPerfMode:
            {
                if (args.Length == 1 && args[0] is int perfMode)
                {
                    parseSuccess = true;
                    if (perfMode < 0)
                    {
                        if (Config.PerfMode == PerfMode.Performance)
                        {
                            Config.PerfMode = PerfMode.MaxBattery;
                        }
                        else
                        {
                            Config.PerfMode++;
                        }
                    }
                    else
                    {
                        Config.PerfMode = (PerfMode)perfMode;
                    }
                    cmdSuccess = ApplyConf();
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
            Config = YamdccCfg.Load(Paths.CurrentConfV2);
            RegLayout = Config.IsNewEC ? new RegLayoutV2() : new RegLayoutV1();
            Log.Info(Strings.GetString("cfgLoaded"));

            if (clientID is not null)
            {
                IPCServer?.PushMessage(new ServiceResponse(
                    Response.ConfLoaded, clientID.Value));
            }

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

        // Write the fan profile to the appropriate registers for each fan:
        SetFanProf(Config.CpuFan.FanProfs[Config.CpuFan.ProfSel], false);
        SetFanProf(Config.CpuFan.FanProfs[Config.GpuFan.ProfSel], true);

        // Write the performance mode
        Log.Info(Strings.GetString("svcWritePerfMode"));
        PerfMode pMode = Config.CpuFan.FanProfs[Config.CpuFan.ProfSel].PerfMode == PerfMode.Default
            ? Config.PerfMode
            : Config.CpuFan.FanProfs[Config.CpuFan.ProfSel].PerfMode;

        if (!LogECWriteByte(RegLayout.PerfModeReg, pMode.ToECValue()))
        {
            success = false;
        }

        // Write the charge threshold:
        Log.Info(Strings.GetString("svcWriteChgLim"));
        if (!LogECWriteByte(RegLayout.ChargeLimReg, (byte)(Config.ChargeLim + 128)))
        {
            success = false;
        }

        // Write the fan mode
        Log.Info(Strings.GetString("svcWriteFanMode"));
        if (!LogECWriteByte(RegLayout.FanModeReg, Config.FanMode.ToECValue()))
        {
            success = false;
        }

        // Write the Win/Fn key swap setting
        if (!SetWinFnSwap())
        {
            success = false;
        }
        return success;
    }

    private bool SetFanProf(FanProf prof, bool gpu)
    {
        Log.Info(Strings.GetString("svcWriteFanConfs", gpu ? "GPU" : "CPU"));
        bool success = true;

        byte[] tUpRegs = gpu ? GpuTupRegs : CpuTupRegs;
        byte[] tDownRegs = gpu ? GpuTdownRegs : CpuTdownRegs;
        byte[] speedRegs = gpu ? GpuFanSpeedRegs : CpuFanSpeedRegs;
        for (int i = 0; i < prof.Thresholds.Count; i++)
        {
            Threshold t = prof.Thresholds[i];
            if (!LogECWriteByte(speedRegs[i], t.Speed))
            {
                success = false;
            }
            if (i > 0)
            {
                if (!LogECWriteByte(tUpRegs[i - 1], t.Tup))
                {
                    success = false;
                }
                byte downT = Config.OffsetDT
                    ? (byte)(t.Tup - t.Tdown)
                    : t.Tdown;

                if (!LogECWriteByte(tDownRegs[i - 1], downT))
                {
                    success = false;
                }
            }
        }
        return success;
    }

    private bool SetWinFnSwap()
    {
        Log.Info(Strings.GetString("svcWriteKeySwap"));
        if (RegLayout.KeySwapReg.HasValue)
        {
            return LogECWriteByte(RegLayout.KeySwapReg.Value,
                Config.KeySwapEnabled ? (byte)16 : (byte)0);
        }
        return false;
    }

    private bool GetFanSpeed(int clientId, bool gpuFan)
    {
        if (Config is null)
        {
            return false;
        }

        if (LogECReadByte(gpuFan ? GPU_FAN_SPEED_REG : CPU_FAN_SPEED_REG, out byte speed))
        {
            IPCServer.PushMessage(new ServiceResponse(
                Response.FanSpeed, gpuFan, speed), clientId);
            return true;
        }
        return false;
    }

    /*private bool GetFanRPM(int clientId, int fanIdx)
    {
        if (Config?.FanConfs[fan].RPMConf is null)
        {
            return false;
        }

        fan = GetValidFanIndex(fan);
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
    }*/

    private bool GetTemp(int clientId, bool gpuFan)
    {
        if (Config is null)
        {
            return false;
        }

        if (LogECReadByte(gpuFan ? GPU_TEMP_REG : CPU_TEMP_REG, out byte temp))
        {
            IPCServer.PushMessage(new ServiceResponse(
                Response.Temp, gpuFan, temp), clientId);
            return true;
        }
        return false;
    }

    private bool SetFullBlast(int enable)
    {
        if (RegLayout is null)
        {
            return false;
        }

        if (LogECReadByte(0x98, out byte value))
        {
            bool oldFbEnable = FullBlastEnabled;

            if (enable == -1)
            {
                FullBlastEnabled = !FullBlastEnabled;
            }
            else if (enable == 0)
            {
                FullBlastEnabled = false;
            }
            else if (enable == 1)
            {
                FullBlastEnabled = true;
            }
            else
            {
                // invalid Full Blast value
                return false;
            }

            if (FullBlastEnabled)
            {
                Log.Debug("Enabling Full Blast...");
                value |= 0x80;
            }
            else
            {
                Log.Debug("Disabling Full Blast...");
                value &= 0x7F;
            }

            if (LogECWriteByte(0x98, value))
            {
                return true;
            }
            // failed to change full blast state; revert to old full blast enabled
            FullBlastEnabled = oldFbEnable;
        }
        return false;
    }

    private bool GetKeyLightSupported(int clientId)
    {
        if (RegLayout is null)
        {
            return false;
        }

        if (LogECReadByte(RegLayout.KeyLightReg, out byte value))
        {
            IPCServer.PushMessage(new ServiceResponse(
                Response.KeyLightSupported, (value & 0x80) == 0x80), clientId);
            return true;
        }
        return false;
    }

    private bool GetKeyLight(int clientId)
    {
        if (RegLayout is null)
        {
            return false;
        }

        Log.Debug(Strings.GetString("svcGetKeyLight"));

        if (LogECReadByte(RegLayout.KeyLightReg, out byte value) &&
            (value & 0x80) == 0x80)
        {
            int brightness = value & 0x7F;

            IPCServer.PushMessage(new ServiceResponse(
                Response.KeyLightBright, brightness), clientId);
            return true;
        }
        return false;
    }

    private bool SetKeyLight(byte brightness)
    {
        if (RegLayout is null)
        {
            return false;
        }

        Log.Debug(Strings.GetString("svcSetKeyLight", brightness));

        if (LogECReadByte(RegLayout.KeyLightReg, out byte value) &&
            (value & 0x80) == 0x80 && brightness >= 0 && brightness <= 3)
        {
            value = (byte)(brightness | 0x80);
            return LogECWriteByte(RegLayout.KeyLightReg, value);
        }
        return false;
    }

    private bool GetFirmVer(int clientId)
    {
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

            Config.FirmVer = EcInfo.Version;
            Config.FirmDate = EcInfo.Date;

            Config.CpuFan.FanProfs[0] = GetDefaultFanProf(false);
            Config.GpuFan.FanProfs[0] = GetDefaultFanProf(true);

            Log.Info("Saving config...");
            Config.Save(Paths.CurrentConfV2);

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

    private FanProf GetDefaultFanProf(bool gpu)
    {
        Log.Info(Strings.GetString("svcReadProfs", gpu ? "GPU" : "CPU"));

        FanProf prof = new()
        {
            Name = "Default",
            Desc = Strings.GetString("DefaultDesc", gpu ? "GPU" : "CPU"),
            Thresholds = new List<Threshold>(GpuFanSpeedRegs.Length),
        };

        byte[] tUpRegs = gpu ? GpuTupRegs : CpuTupRegs;
        byte[] tDownRegs = gpu ? GpuTdownRegs : CpuTdownRegs;
        byte[] speedRegs = gpu ? GpuFanSpeedRegs : CpuFanSpeedRegs;

        for (int j = 0; j < prof.Thresholds.Count; j++)
        {
            prof.Thresholds[j] ??= new();
            Threshold t = prof.Thresholds[j];

            if (LogECReadByte(speedRegs[j], out byte value))
            {
                t.Speed = value;
            }

            if (j == 0)
            {
                t.Tup = 0;
                t.Tdown = 0;
            }
            else
            {
                if (LogECReadByte(tUpRegs[j - 1], out value))
                {
                    t.Tup = value;
                }
                if (LogECReadByte(tDownRegs[j - 1], out value))
                {
                    t.Tdown = Config.OffsetDT
                        ? (byte)(t.Tup - value)
                        : value;
                }
            }
        }
        return prof;
    }
}
