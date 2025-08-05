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
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using YAMDCC.Common;
using YAMDCC.Common.Configs;
using YAMDCC.Common.Dialogs;
using YAMDCC.Common.Logs;
using YAMDCC.IPC;

namespace YAMDCC.ConfigEditor;

internal sealed partial class MainForm : Form
{
    #region Fields
    private readonly Status AppStatus = new();

    /// <summary>
    /// The YAMDCC config that is currently open for editing.
    /// </summary>
    private YAMDCC_Config Config;

    /// <summary>
    /// The client that connects to the YAMDCC Service
    /// </summary>
    private readonly NamedPipeClient<ServiceResponse, ServiceCommand> IPCClient =
        new("YAMDCC-Server");

    private NumericUpDown[] numUpTs, numDownTs, numFanSpds;
    private TrackBar[] tbFanSpds;

    private readonly ToolTip ttMain = new();

    private readonly Timer tmrPoll, tmrStatusReset, tmrSvcTimeout;

    private int Debug;
    #endregion

    public MainForm()
    {
        InitializeComponent();

        // Set the window icon using the application icon.
        // Saves about 8-9 KB from not having to embed the same icon twice.
        Icon = Utils.GetEntryAssemblyIcon();

        // set title text to include program version
        Text = $"YAMDCC config editor - v{Utils.GetVerString()}";

        // set literally every tooltip
        tsiLoadConf.ToolTipText = Strings.GetString("ttLoadConf");
        tsiSaveConf.ToolTipText = Strings.GetString("ttSaveConf");
        tsiApply.ToolTipText = Strings.GetString("ttApply");
        tsiRevert.ToolTipText = Strings.GetString("ttRevert");
        tsiExit.ToolTipText = Strings.GetString("ttExit");
        tsiProfAdd.ToolTipText = Strings.GetString("ttProfAdd");
        tsiProfRen.ToolTipText = Strings.GetString("ttProfRen");
        tsiProfChangeDesc.ToolTipText = Strings.GetString("ttProfChangeDesc");
        tsiECtoConf.ToolTipText = Strings.GetString("ttECtoConf");
        tsiProfDel.ToolTipText = Strings.GetString("ttProfDel");
        tsiECMon.ToolTipText = Strings.GetString("ttECMon");
        tsiAdvanced.ToolTipText = Strings.GetString("ttAdvanced");
        tsiLogDebug.ToolTipText = Strings.GetString("ttLogLevel");
        tsiLogInfo.ToolTipText = Strings.GetString("ttLogLevel");
        tsiLogWarn.ToolTipText = Strings.GetString("ttLogLevel");
        tsiLogError.ToolTipText = Strings.GetString("ttLogLevel");
        tsiLogFatal.ToolTipText = Strings.GetString("ttLogLevel");
        tsiLogNone.ToolTipText = Strings.GetString("ttLogLevel");
        tsiStopSvc.ToolTipText = Strings.GetString("ttStopSvc");
        tsiUninstall.ToolTipText = Strings.GetString("ttUninstall");
        tsiAbout.ToolTipText = Strings.GetString("ttAbout");
        tsiSource.ToolTipText = Strings.GetString("ttSource");
        ttMain.SetToolTip(cboFanSel, Strings.GetString("ttFanSel"));
        ttMain.SetToolTip(btnProfAdd, Strings.GetString("ttProfAdd"));
        ttMain.SetToolTip(btnProfDel, Strings.GetString("ttProfDel"));
        ttMain.SetToolTip(btnApply, Strings.GetString("ttApply"));
        ttMain.SetToolTip(btnRevert, Strings.GetString("ttRevert"));

        tmrPoll = new()
        {
            Interval = 1000,
        };
        tmrPoll.Tick += new EventHandler(tmrPoll_Tick);

        tmrStatusReset = new()
        {
            Interval = 5000,
        };
        tmrStatusReset.Tick += new EventHandler(tmrStatusReset_Tick);

        tmrSvcTimeout = new()
        {
            Interval = 10000,
        };
        tmrSvcTimeout.Tick += new EventHandler(tmrSvcTimeout_Tick);

        tsiFBExit.Checked = CommonConfig.GetDisableFBOnExit();

        switch (CommonConfig.GetLogLevel())
        {
            case LogLevel.None:
                tsiLogNone.Checked = true;
                break;
            case LogLevel.Fatal:
                tsiLogFatal.Checked = true;
                break;
            case LogLevel.Error:
                tsiLogError.Checked = true;
                break;
            case LogLevel.Warn:
                tsiLogWarn.Checked = true;
                break;
            case LogLevel.Info:
                tsiLogInfo.Checked = true;
                break;
            case LogLevel.Debug:
                tsiLogDebug.Checked = true;
                break;
        }

        if (!CommonConfig.GetAutoUpdateAsked() && File.Exists("./Updater.exe"))
        {
            if (Utils.ShowInfo(Strings.GetString("dlgAutoUpdate"),
                "Check for updates?", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                string path = Path.GetFullPath(@".\Updater.exe");
                try
                {
                    Process.Start(path, "--setautoupdate true");
                }
                catch (Win32Exception ex)
                {
                    // catch the exception that occurs if the Updater is not found
                    if (ex.ErrorCode == -2147467259 && ex.NativeErrorCode == 2)
                    {
                        Utils.ShowError(
                            $"Failed to open the Updater to set auto-update state\n" +
                            $"(located at: {path}):\n" +
                            $"{ex.Message} ({ex.NativeErrorCode})");
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            CommonConfig.SetAutoUpdateAsked(true);
        }
        DisableAll();
    }

    #region Events
    protected override void OnLoad(EventArgs e)
    {
        base.OnLoad(e);
        IPCClient.ServerMessage += new EventHandler<PipeMessageEventArgs<ServiceResponse, ServiceCommand>>(IPCMessage);
        IPCClient.Error += new EventHandler<PipeErrorEventArgs<ServiceResponse, ServiceCommand>>(IPCError);
        IPCClient.Start();

        ProgressDialog<bool> dlg = new()
        {
            Caption = "Connecting to YAMDCC service...",
            DoWork = () => !IPCClient.WaitForConnection(5000)
        };
        dlg.ShowDialog();

        if (dlg.Result)
        {
            throw new TimeoutException(Strings.GetString("exSvcTimeout"));
        }
        AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);

        LoadConf(Paths.CurrentConf);

        if (Config?.KeyLightConf is null)
        {
            ttMain.SetToolTip(tbKeyLight, Strings.GetString("ttNotSupported"));
        }
        else
        {
            SendSvcMessage(new ServiceCommand(Command.GetKeyLightBright));
        }

        switch (CommonConfig.GetECtoConfState())
        {
            case ECtoConfState.Fail:
                Utils.ShowError(Strings.GetString("dlgECtoConfErr", Paths.Logs));
                CommonConfig.SetECtoConfState(ECtoConfState.None);
                break;
            case ECtoConfState.Success:
                Utils.ShowInfo(Strings.GetString("dlgECtoConfSuccess"), "Success");
                CommonConfig.SetECtoConfState(ECtoConfState.None);
                break;
        }
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        base.OnFormClosing(e);
        // Disable Full Blast if it was enabled while the program
        // was running and the user wants it disabled on exit:
        if (chkFullBlast.Checked && CommonConfig.GetDisableFBOnExit())
        {
            SendSvcMessage(new ServiceCommand(Command.SetFullBlast, 0));
        }
    }

    private void OnProcessExit(object sender, EventArgs e)
    {
        // Close the connection to the YAMDCC
        // Service before exiting the program:
        tmrPoll.Stop();
        IPCClient.Stop();
    }

    private void IPCMessage(object sender, PipeMessageEventArgs<ServiceResponse, ServiceCommand> e)
    {
        tmrSvcTimeout.Stop();
        Invoke(() =>
        {
            object[] args = e.Message.Value;
            switch (e.Message.Response)
            {
                case Response.Nothing:
                {
                    UpdateStatus(StatusCode.ServiceResponseEmpty);
                    break;
                }
                case Response.Success:
                {
                    if (args.Length != 1 || args[0] is not int cmd)
                    {
                        break;
                    }
                    switch ((Command)cmd)
                    {
                        case Command.ApplyConf:
                            ToggleSvcCmds(true);
                            UpdateStatus(StatusCode.ConfApplied);
                            if (Config.KeyLightConf is not null)
                            {
                                SendSvcMessage(new ServiceCommand(Command.GetKeyLightBright));
                            }
                            break;
                        case Command.SetFullBlast:
                            ToggleSvcCmds(true);
                            UpdateStatus(StatusCode.FullBlastToggled);
                            break;
                    }
                    break;
                }
                case Response.Error:
                {
                    if (args.Length == 1 && args[0] is int cmd)
                    {
                        UpdateStatus(StatusCode.ServiceCommandFail, cmd);
                    }
                    break;
                }
                case Response.Temp:
                {
                    if (args.Length == 2 && args[0] is int fan && args[1] is int temp)
                    {
                        if (fan == cboFanSel.SelectedIndex)
                        {
                            lblTemp.Text = $"Temp: {temp}°C";
                        }
                    }
                    break;
                }
                case Response.FanSpeed:
                {
                    if (args.Length == 2 && args[0] is int fan && args[1] is int speed && fan == cboFanSel.SelectedIndex)
                    {
                        lblFanSpd.Text = $"Fan speed: {speed}%";
                    }
                    break;
                }
                case Response.FanRPM:
                {
                    if (args.Length == 2 && args[0] is int fan && args[1] is int rpm && fan == cboFanSel.SelectedIndex)
                    {
                        if (rpm < 0)
                        {
                            rpm = 0;
                        }
                        lblFanRPM.Text = $"RPM: {rpm}";
                    }
                    break;
                }
                case Response.KeyLightBright:
                {
                    if (args.Length == 1 && args[0] is int brightness)
                    {
                        // value received from service should be valid,
                        // but let's check anyway to avoid potential crashes
                        // from non-official YAMDCC services
                        int max = Config.KeyLightConf.MaxVal - Config.KeyLightConf.MinVal;
                        if (brightness < 0 || brightness > max)
                        {
                            break;
                        }

                        tbKeyLight.Maximum = max;
                        tbKeyLight.Value = brightness;
                        ttMain.SetToolTip(tbKeyLight, Strings.GetString("ttKeyLight"));

                        tbKeyLight.Enabled = true;
                        lblKeyLightLow.Enabled = true;
                        lblKeyLightHigh.Enabled = true;
                    }
                    break;
                }
                case Response.FirmVer:
                {
                    // no idea how the EcInfo class became a nested
                    // object array, but this works so keeping it
                    if (args[0] is object[] obj && obj.Length == 2 &&
                        obj[0] is string ver && obj[1] is DateTime date)
                    {
                        Config.FirmVer = ver;
                        Config.FirmDate = date;
                        txtFirmVer.Text = Config.FirmVer;
                        txtFirmDate.Text = $"{Config.FirmDate:G}";
                    }
                    break;
                }
            }
        });
    }

    private void IPCError(object sender, PipeErrorEventArgs<ServiceResponse, ServiceCommand> e)
    {
        new CrashDialog(e.Exception).ShowDialog();
    }

    #region Tool strip menu items

    #region File
    private void tsiLoadConf_Click(object sender, EventArgs e)
    {
        OpenFileDialog ofd = new()
        {
            AddExtension = true,
            CheckFileExists = true,
            Filter = "YAMDCC config files|*.xml",
            Title = "Load config",
        };

        if (ofd.ShowDialog() == DialogResult.OK)
        {
            LoadConf(ofd.FileName);
            CommonConfig.SetLastConf(ofd.FileName);
        }
    }

    private void tsiSaveConf_Click(object sender, EventArgs e)
    {
        SaveFileDialog sfd = new()
        {
            AddExtension = true,
            Filter = "YAMDCC config files|*.xml",
            FileName = Config.Model.Replace(' ', '-'),
            Title = "Save config",
        };

        if (sfd.ShowDialog() == DialogResult.OK)
        {
            Config.ChargeLimitConf.CurVal = (byte)(chkChgLim.Checked
                ? numChgLim.Value : 0);
            Config.Save(sfd.FileName);
            CommonConfig.SetLastConf(sfd.FileName);
        }
    }

    private void tsiExit_Click(object sender, EventArgs e)
    {
        Close();
    }
    #endregion

    #region Options
    private void ProfRename(object sender, EventArgs e)
    {
        FanCurveConf curveCfg = Config.FanConfs[cboFanSel.SelectedIndex]
            .FanCurveConfs[cboProfSel.SelectedIndex];

        TextInputDialog dlg = new(
            Strings.GetString("dlgProfRen"),
            "Change Profile Name", curveCfg.Name);
        if (dlg.ShowDialog() == DialogResult.OK)
        {
            curveCfg.Name = dlg.Result;
            cboProfSel.Items[cboProfSel.SelectedIndex] = dlg.Result;
        }
    }

    private void ProfChangeDesc(object sender, EventArgs e)
    {
        FanCurveConf curveCfg = Config.FanConfs[cboFanSel.SelectedIndex]
            .FanCurveConfs[cboProfSel.SelectedIndex];

        TextInputDialog dlg = new(
            Strings.GetString("dlgProfChangeDesc"),
            "Change Profile Description", curveCfg.Desc, true);
        if (dlg.ShowDialog() == DialogResult.OK)
        {
            curveCfg.Desc = dlg.Result;
            ttMain.SetToolTip(cboProfSel, Strings.GetString(
                "ttProfSel", dlg.Result));
        }
    }

    private void ECtoConf(object sender, EventArgs e)
    {
        if (Utils.ShowInfo(Strings.GetString("dlgECtoConfStart"),
            "Default fan profile from EC?", MessageBoxButtons.YesNo) == DialogResult.Yes)
        {
            Application.Exit();
            CommonConfig.SetECtoConfState(ECtoConfState.PendingReboot);
        }
    }

    private void ECMonToggle(object sender, EventArgs e)
    {
        if (tsiECMon.Checked)
        {
            PollEC();
            tmrPoll.Start();
            lblFanSpd.Visible = true;
            lblFanRPM.Visible = true;
            lblTemp.Visible = true;
        }
        else
        {
            tmrPoll.Stop();
            lblFanSpd.Visible = false;
            lblFanRPM.Visible = false;
            lblTemp.Visible = false;
        }
    }

    private void AdvancedToggle(object sender, EventArgs e)
    {
        if (!tsiAdvanced.Checked && Utils.ShowWarning(Strings.GetString("dlgAdvanced"),
            "Show advanced settings?", MessageBoxDefaultButton.Button2) != DialogResult.Yes)
        {
            return;
        }
        tsiAdvanced.Checked = !tsiAdvanced.Checked;

        if (Config?.FanModeConf is not null)
        {
            cboFanMode.Enabled = tsiAdvanced.Checked;
        }
    }

    private void tsiLogNone_Click(object sender, EventArgs e)
    {
        CommonConfig.SetLogLevel(LogLevel.None);
        tsiLogNone.Checked = true;
        tsiLogDebug.Checked = false;
        tsiLogInfo.Checked = false;
        tsiLogWarn.Checked = false;
        tsiLogError.Checked = false;
        tsiLogFatal.Checked = false;
    }

    private void tsiLogDebug_Click(object sender, EventArgs e)
    {
        CommonConfig.SetLogLevel(LogLevel.Debug);
        tsiLogNone.Checked = false;
        tsiLogDebug.Checked = true;
        tsiLogInfo.Checked = false;
        tsiLogWarn.Checked = false;
        tsiLogError.Checked = false;
        tsiLogFatal.Checked = false;
    }

    private void tsiLogInfo_Click(object sender, EventArgs e)
    {
        CommonConfig.SetLogLevel(LogLevel.Info);
        tsiLogNone.Checked = false;
        tsiLogDebug.Checked = false;
        tsiLogInfo.Checked = true;
        tsiLogWarn.Checked = false;
        tsiLogError.Checked = false;
        tsiLogFatal.Checked = false;
    }

    private void tsiLogWarn_Click(object sender, EventArgs e)
    {
        CommonConfig.SetLogLevel(LogLevel.Warn);
        tsiLogNone.Checked = false;
        tsiLogDebug.Checked = false;
        tsiLogInfo.Checked = false;
        tsiLogWarn.Checked = true;
        tsiLogError.Checked = false;
        tsiLogFatal.Checked = false;
    }

    private void tsiLogError_Click(object sender, EventArgs e)
    {
        CommonConfig.SetLogLevel(LogLevel.Error);
        tsiLogNone.Checked = false;
        tsiLogDebug.Checked = false;
        tsiLogInfo.Checked = false;
        tsiLogWarn.Checked = false;
        tsiLogError.Checked = true;
        tsiLogFatal.Checked = false;
    }

    private void tsiLogFatal_Click(object sender, EventArgs e)
    {
        CommonConfig.SetLogLevel(LogLevel.Fatal);
        tsiLogNone.Checked = false;
        tsiLogDebug.Checked = false;
        tsiLogInfo.Checked = false;
        tsiLogWarn.Checked = false;
        tsiLogError.Checked = false;
        tsiLogFatal.Checked = true;
    }

    private void tsiFBExit_Click(object sender, EventArgs e)
    {
        CommonConfig.SetDisableFBOnExit(((ToolStripMenuItem)sender).Checked);
    }

    private void tsiStopSvc_Click(object sender, EventArgs e)
    {
        if (Utils.ShowWarning(Strings.GetString("dlgSvcStop"),
            "Stop Service") == DialogResult.Yes)
        {
            tmrPoll.Stop();
            IPCClient.Stop();
            Hide();

            ProgressDialog<bool> dlg = new()
            {
                Caption = Strings.GetString("dlgSvcStopping"),
                DoWork = static () =>
                {
                    if (!Utils.StopService("yamdccsvc"))
                    {
                        Utils.ShowError(Strings.GetString("dlgSvcStopErr"));
                        return false;
                    }
                    Utils.ShowInfo(Strings.GetString("dlgSvcStopped"), "Success");
                    return true;
                }
            };
            dlg.ShowDialog();

            Close();
        }
    }

    private void tsiUninstall_Click(object sender, EventArgs e)
    {
        if (Utils.ShowWarning(Strings.GetString("dlgUninstall"),
            "Uninstall?") == DialogResult.Yes)
        {
            bool delData = Utils.ShowWarning(
                Strings.GetString("dlgSvcDelData", Paths.Data),
                "Delete configuration data?",
                MessageBoxDefaultButton.Button2) == DialogResult.Yes;

            tmrPoll.Stop();
            IPCClient.Stop();
            Hide();

            ProgressDialog<bool> dlg = new()
            {
                Caption = Strings.GetString("dlgSvcUninstalling"),
                DoWork = () =>
                {
                    // Apparently this fixes the YAMDCC service not uninstalling
                    // when YAMDCC is launched by certain means
                    if (Utils.StopService("yamdccsvc"))
                    {
                        if (Utils.UninstallService("yamdccsvc"))
                        {
                            // Delete the auto-update scheduled task
                            try
                            {
                                Process.Start("./Updater.exe", "--setautoupdate false");
                            }
                            catch (Win32Exception ex)
                            {
                                // catch the exception that occurs if the Updater is not found
                                if (ex.ErrorCode == -2147467259 && ex.NativeErrorCode == 2)
                                {
                                    Utils.ShowError("Updater.exe not found!");
                                }
                                else
                                {
                                    throw;
                                }
                            }

                            // Only delete service data if the
                            // service uninstalled successfully
                            if (delData)
                            {
                                Directory.Delete(Paths.Data, true);
                            }
                            Utils.ShowInfo(Strings.GetString("dlgSvcUninstalled"), "Success");
                            return true;
                        }
                        Utils.ShowError(Strings.GetString("dlgUninstallErr"));
                        return false;
                    }
                    Utils.ShowError(Strings.GetString("dlgSvcStopErr"));
                    return false;
                }
            };
            dlg.ShowDialog();
            Close();
        }
    }
    #endregion

    #region Help
    private void tsiAbout_Click(object sender, EventArgs e)
    {
        new VersionDialog().ShowDialog();
    }

    private void tsiSrc_Click(object sender, EventArgs e)
    {
        Process.Start(Paths.GitHubPage);
    }

    private void tsiCheckUpdate_Click(object sender, EventArgs e)
    {
        try
        {
            Process.Start("./Updater.exe", "--checkupdate");
        }
        catch (Win32Exception ex)
        {
            // catch the exception that occurs if the Updater is not found
            if (ex.ErrorCode == -2147467259 && ex.NativeErrorCode == 2)
            {
                Utils.ShowError("Updater.exe not found!");
            }
            else
            {
                throw;
            }
        }
    }
    #endregion

    #endregion

    private void FanSelChanged(object sender, EventArgs e)
    {
        FanConf cfg = Config.FanConfs[cboFanSel.SelectedIndex];

        cboProfSel.Items.Clear();
        foreach (FanCurveConf curve in cfg.FanCurveConfs)
        {
            cboProfSel.Items.Add(curve.Name);
        }

        if (numFanSpds is null || numFanSpds.Length != cfg.FanCurveRegs.Length)
        {
            float scale = CurrentAutoScaleDimensions.Height / 96;

            tblCurve.SuspendLayout();
            tblCurve.Controls.Clear();
            numUpTs = new NumericUpDown[cfg.UpThresholdRegs.Length];
            numDownTs = new NumericUpDown[cfg.DownThresholdRegs.Length];
            numFanSpds = new NumericUpDown[cfg.FanCurveRegs.Length];
            tbFanSpds = new TrackBar[cfg.FanCurveRegs.Length];

            tblCurve.ColumnStyles.Clear();
            tblCurve.ColumnCount = numFanSpds.Length + 1;

            // labels on left side
            tblCurve.ColumnStyles.Add(new ColumnStyle());
            tblCurve.Controls.Add(FanCurveLabel("Speed (%)", scale), 0, 0);
            tblCurve.Controls.Add(FanCurveLabel("Up (°C)", scale), 0, 2);
            tblCurve.Controls.Add(FanCurveLabel("Down (°C)", scale), 0, 3);

            for (int i = 0; i < numFanSpds.Length; i++)
            {
                tblCurve.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F / numFanSpds.Length));

                numFanSpds[i] = FanCurveNUD(i, scale);
                ttMain.SetToolTip(numFanSpds[i], Strings.GetString("ttFanSpd"));
                numFanSpds[i].ValueChanged += new EventHandler(FanSpdChange);
                tblCurve.Controls.Add(numFanSpds[i], i + 1, 0);

                tbFanSpds[i] = new TrackBar()
                {
                    Dock = DockStyle.Fill,
                    LargeChange = 10,
                    Margin = new Padding((int)(12 * scale), 0, (int)(12 * scale), 0),
                    Orientation = Orientation.Vertical,
                    Tag = i,
                    TickFrequency = 5,
                    TickStyle = TickStyle.Both,
                };
                ttMain.SetToolTip(tbFanSpds[i], Strings.GetString("ttFanSpd"));
                tbFanSpds[i].ValueChanged += new EventHandler(FanSpdChange);
                tblCurve.Controls.Add(tbFanSpds[i], i + 1, 1);

                if (i != 0)
                {
                    numUpTs[i - 1] = FanCurveNUD(i - 1, scale);
                    ttMain.SetToolTip(numUpTs[i - 1], Strings.GetString("ttUpT"));
                    numUpTs[i - 1].ValueChanged += new EventHandler(UpTChange);
                    tblCurve.Controls.Add(numUpTs[i - 1], i + 1, 2);
                }
                else
                {
                    tblCurve.Controls.Add(FanCurveLabel("Default", scale, ContentAlignment.MiddleCenter), i + 1, 2);
                }

                if (i != numFanSpds.Length - 1)
                {
                    numDownTs[i] = FanCurveNUD(i, scale);
                    ttMain.SetToolTip(numDownTs[i], Strings.GetString("ttDownT"));
                    numDownTs[i].ValueChanged += new EventHandler(DownTChange);
                    tblCurve.Controls.Add(numDownTs[i], i + 1, 3);
                }
                else
                {
                    tblCurve.Controls.Add(FanCurveLabel("Max", scale, ContentAlignment.MiddleCenter), i + 1, 3);
                }
            }
            tblCurve.ResumeLayout(true);
        }

        for (int i = 0; i < numFanSpds.Length; i++)
        {
            numFanSpds[i].Maximum = tbFanSpds[i].Maximum =
                Math.Abs(cfg.MaxSpeed - cfg.MinSpeed);
        }

        cboProfSel.Enabled = true;
        cboProfSel.SelectedIndex = cfg.CurveSel;

        if (tsiECMon.Checked)
        {
            tmrPoll.Stop();
            PollEC();
            tmrPoll.Start();
        }
    }

    private void ProfSelChanged(object sender, EventArgs e)
    {
        FanConf cfg = Config.FanConfs[cboFanSel.SelectedIndex];
        FanCurveConf curveCfg = cfg.FanCurveConfs[cboProfSel.SelectedIndex];

        for (int i = 0; i < Config.FanConfs.Count; i++)
        {
            Config.FanConfs[i].CurveSel = cboProfSel.SelectedIndex;
        }

        ttMain.SetToolTip(cboProfSel, Strings.GetString(
            "ttProfSel", cfg.FanCurveConfs[cfg.CurveSel].Desc));

        if (Config.PerfModeConf is not null)
        {
            cboProfPerfMode.SelectedIndex = Config.FanConfs[0]
                .FanCurveConfs[cboProfSel.SelectedIndex].PerfModeSel + 1;
            cboProfPerfMode.Enabled = true;
        }

        bool enable = curveCfg.Name != "Default";
        for (int i = 0; i < numFanSpds.Length; i++)
        {
            // Fan profile
            TempThreshold t = curveCfg.TempThresholds[i];
            numFanSpds[i].Value = tbFanSpds[i].Value = t.FanSpeed;
            numFanSpds[i].Enabled = enable;
            tbFanSpds[i].Enabled = enable;

            // Temp thresholds
            if (i < cfg.UpThresholdRegs.Length)
            {
                t = curveCfg.TempThresholds[i + 1];
                numUpTs[i].Value = t.UpThreshold;
                numDownTs[i].Value = t.DownThreshold;
                numUpTs[i].Enabled = enable;
                numDownTs[i].Enabled = enable;
            }
        }
        btnProfDel.Enabled = enable;
        tsiProfDel.Enabled = enable;
    }

    private void ProfPerfModeChanged(object sender, EventArgs e)
    {
        int i = cboProfPerfMode.SelectedIndex;
        PerfModeConf pModeCfg = Config.PerfModeConf;
        Config.FanConfs[0].FanCurveConfs[cboProfSel.SelectedIndex].PerfModeSel = i - 1;

        if (i > 0)
        {
            ttMain.SetToolTip(cboProfPerfMode,
                Strings.GetString("ttProfPerfMode", pModeCfg.PerfModes[i - 1].Desc));
        }
        else    // use default performance mode description
        {
            ttMain.SetToolTip(cboProfPerfMode,
                Strings.GetString("ttProfPerfMode", pModeCfg.PerfModes[pModeCfg.ModeSel].Desc));
        }
    }

    private void ProfAdd(object sender, EventArgs e)
    {
        FanConf cfg = Config.FanConfs[cboFanSel.SelectedIndex];

        TextInputDialog dlg = new(
            Strings.GetString("dlgProfAdd"), "New Profile",
            $"Copy of {cfg.FanCurveConfs[cboProfSel.SelectedIndex].Name}");

        if (dlg.ShowDialog() == DialogResult.OK)
        {
            for (int i = 0; i < cboFanSel.Items.Count; i++)
            {
                cfg = Config.FanConfs[i];

                // Create a copy of the currently selected fan profile
                // and add it to the config's list:
                FanCurveConf oldCurveCfg = cfg.FanCurveConfs[cfg.CurveSel];
                cfg.FanCurveConfs.Add(oldCurveCfg.Copy());
                cfg.CurveSel = cfg.FanCurveConfs.Count - 1;

                // Name it according to what the user specified
                cfg.FanCurveConfs[cfg.CurveSel].Name = dlg.Result;
                cfg.FanCurveConfs[cfg.CurveSel].Desc = $"(Copy of {oldCurveCfg.Name})\n{oldCurveCfg.Desc}";
            }

            // Add the new fan profile to the UI's profile list and select it:
            cboProfSel.Items.Add(dlg.Result);
            cboProfSel.SelectedIndex = Config.FanConfs[cboFanSel.SelectedIndex].CurveSel;
        }
    }

    private void btnProfAdd_KeyPress(object sender, KeyPressEventArgs e)
    {
        // hidden crash test
        switch (e.KeyChar)
        {
            case 'd':
                Debug = Debug == 0 ? 1 : 0;
                break;
            case 'e':
                Debug = Debug == 1 ? 2 : 0;
                break;
            case 'b':
                Debug = Debug == 2 ? 3 : 0;
                break;
            case 'u':
                Debug = Debug == 3 ? 4 : 0;
                break;
            case 'g':
                if (Debug == 4)
                {
                    Debug = 0;

                    // should throw a NullReferenceException
                    // which should get caught by CrashDialog
                    YAMDCC_Config cfg = new();
                    lblFanSpd.Text = cfg.FanConfs[0].Name;
                }
                break;
            default:
                Debug = 0;
                break;
        }
    }

    private void ProfDel(object sender, EventArgs e)
    {
        FanConf cfg = Config.FanConfs[cboFanSel.SelectedIndex];
        FanCurveConf curveCfg = cfg.FanCurveConfs[cfg.CurveSel];

        if (curveCfg.Name != "Default" && Utils.ShowWarning(
            Strings.GetString("dlgProfDel", curveCfg.Name),
            $"Delete fan profile? ({cfg.Name})") == DialogResult.Yes)
        {
            // Remove each equivalent fan profile from the config's list
            for (int i = 0; i < cboFanSel.Items.Count; i++)
            {
                cfg = Config.FanConfs[i];
                cfg.FanCurveConfs.RemoveAt(cfg.CurveSel);
                cfg.CurveSel -= 1;
            }

            // Remove from the list client-side, and select a different fan profile
            cboProfSel.Items.RemoveAt(cboProfSel.SelectedIndex);
            cboProfSel.SelectedIndex = Config.FanConfs[cboFanSel.SelectedIndex].CurveSel;
        }
    }

    private void FanSpdChange(object sender, EventArgs e)
    {
        Control c = (Control)sender;
        int i = (int)c.Tag;

        if (c is NumericUpDown)
        {
            tbFanSpds[i].Value = (int)numFanSpds[i].Value;
        }
        else if (c is TrackBar)
        {
            numFanSpds[i].Value = tbFanSpds[i].Value;
        }

        if (cboProfSel.SelectedIndex != -1)
        {
            Config.FanConfs[cboFanSel.SelectedIndex]
                .FanCurveConfs[cboProfSel.SelectedIndex]
                .TempThresholds[i].FanSpeed = (byte)tbFanSpds[i].Value;
        }
    }

    private void UpTChange(object sender, EventArgs e)
    {
        NumericUpDown nud = (NumericUpDown)sender;
        int i = (int)nud.Tag;

        TempThreshold threshold = Config.FanConfs[cboFanSel.SelectedIndex]
            .FanCurveConfs[cboProfSel.SelectedIndex]
            .TempThresholds[i + 1];

        // Update associated down threshold slider
        numDownTs[i].Value += nud.Value - threshold.UpThreshold;

        threshold.UpThreshold = (byte)numUpTs[i].Value;
    }

    private void DownTChange(object sender, EventArgs e)
    {
        NumericUpDown nud = (NumericUpDown)sender;
        int i = (int)nud.Tag;

        Config.FanConfs[cboFanSel.SelectedIndex]
            .FanCurveConfs[cboProfSel.SelectedIndex]
            .TempThresholds[i + 1].DownThreshold = (byte)numDownTs[i].Value;
    }

    private void ChgLimToggle(object sender, EventArgs e)
    {
        numChgLim.Enabled = chkChgLim.Checked;
    }

    private void PerfModeChange(object sender, EventArgs e)
    {
        int i = cboPerfMode.SelectedIndex;
        Config.PerfModeConf.ModeSel = i;
        ttMain.SetToolTip(cboPerfMode,
            Strings.GetString("ttPerfMode", Config.PerfModeConf.PerfModes[i].Desc));

        PerfModeConf pCfg = Config.PerfModeConf;
        cboProfPerfMode.Items[0] = $"Default ({pCfg.PerfModes[pCfg.ModeSel].Name})";
    }

    private void WinFnSwapToggle(object sender, EventArgs e)
    {
        Config.KeySwapConf.Enabled = chkWinFnSwap.Checked;
    }

    private void KeyLightChange(object sender, EventArgs e)
    {
        SendSvcMessage(new ServiceCommand(Command.SetKeyLightBright, (byte)tbKeyLight.Value));
    }

    private void FanModeChange(object sender, EventArgs e)
    {
        int i = cboFanMode.SelectedIndex;
        Config.FanModeConf.ModeSel = i;
        ttMain.SetToolTip(cboFanMode,
            Strings.GetString("ttFanMode", Config.FanModeConf.FanModes[i].Desc));
    }

    private void txtAuthor_Validating(object sender, CancelEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtAuthor.Text))
        {
            MessageBox.Show("Author must not be empty", "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtAuthor.Text = Config.Author;
        }
        else
        {
            Config.Author = txtAuthor.Text.Trim();
        }
    }

    private void btnGetModel_Click(object sender, EventArgs e)
    {
        string pcManufacturer = Utils.GetPCManufacturer(),
            pcModel = Utils.GetPCModel();

        if (!string.IsNullOrEmpty(pcManufacturer))
        {
            txtManufacturer.Text = pcManufacturer;
            Config.Manufacturer = pcManufacturer;
        }
        if (!string.IsNullOrEmpty(pcModel))
        {
            txtModel.Text = pcModel;
            Config.Model = pcModel;
        }

        if (Config.FirmVerSupported)
        {
            SendSvcMessage(new ServiceCommand(Command.GetFirmVer));
        }
    }

    private void FullBlastToggle(object sender, EventArgs e)
    {
        ToggleSvcCmds(false);
        SendSvcMessage(new ServiceCommand(Command.SetFullBlast, chkFullBlast.Checked ? 1 : 0));
    }

    private void RevertConf(object sender, EventArgs e)
    {
        if (Utils.ShowWarning(Strings.GetString("dlgRevert"),
            "Revert?") == DialogResult.Yes)
        {
            try
            {
                Config = YAMDCC_Config.Load(CommonConfig.GetLastConf());
                LoadConf(Config);
                ApplyConf(sender, e);
            }
            catch (Exception ex)
            {
                if (ex is FileNotFoundException)
                {
                    Utils.ShowError(Strings.GetString("dlgOldConfMissing"));
                }
                else if (ex is InvalidConfigException or InvalidOperationException)
                {
                    Utils.ShowError(Strings.GetString("dlgOldConfInvalid"));
                }
                else
                {
                    throw;
                }
            }
        }
    }

    private void ApplyConf(object sender, EventArgs e)
    {
        ToggleSvcCmds(false);

        // Save the updated config
        Config.ChargeLimitConf.CurVal = (byte)(chkChgLim.Checked ? numChgLim.Value : 0);
        Config.Save(Paths.CurrentConf);

        // Tell the service to reload and apply the updated config
        SendSvcMessage(new ServiceCommand(Command.ApplyConf));
    }

    private void tmrPoll_Tick(object sender, EventArgs e)
    {
        PollEC();
    }

    private void tmrStatusReset_Tick(object sender, EventArgs e)
    {
        UpdateStatus(StatusCode.None);
        tmrStatusReset.Stop();
    }

    private void tmrSvcTimeout_Tick(object sender, EventArgs e)
    {
        UpdateStatus(StatusCode.ServiceTimeout);
        tmrSvcTimeout.Stop();
    }

    #endregion  // Events

    #region Private methods
    private void LoadConf(string confPath)
    {
        UpdateStatus(StatusCode.ConfLoading);

        try
        {
            Config = YAMDCC_Config.Load(confPath);
            LoadConf(Config);
        }
        catch (Exception ex)
        {
            if (ex is InvalidConfigException or InvalidOperationException or FileNotFoundException)
            {
                UpdateStatus(StatusCode.NoConfig);
                return;
            }
            else
            {
                throw;
            }
        }
    }

    private void LoadConf(YAMDCC_Config cfg)
    {
        DisableAll();

        tsiSaveConf.Enabled = true;

        txtAuthor.Text = cfg.Author;
        txtManufacturer.Text = cfg.Manufacturer;
        txtModel.Text = cfg.Model;
        if (cfg.FirmVerSupported)
        {
            txtFirmVer.Text = cfg.FirmVer;
            txtFirmDate.Text = $"{cfg.FirmDate:G}";
        }
        else
        {
            txtFirmVer.Text = "(unknown)";
            txtFirmDate.Text = "(unknown)";
        }
        txtAuthor.Enabled = true;
        btnGetModel.Enabled = true;

        if (cfg.FullBlastConf is null)
        {
            ttMain.SetToolTip(chkFullBlast, Strings.GetString("ttNotSupported"));
        }
        else
        {
            ttMain.SetToolTip(chkFullBlast, Strings.GetString("ttFullBlast"));
            chkFullBlast.Enabled = true;
        }

        if (cfg.ChargeLimitConf is null)
        {
            ttMain.SetToolTip(chkFullBlast, Strings.GetString("ttNotSupported"));
        }
        else
        {
            ttMain.SetToolTip(numChgLim, Strings.GetString("ttChgLim"));
            ChargeLimitConf chgLimConf = cfg.ChargeLimitConf;
            chkChgLim.Enabled = true;
            numChgLim.Enabled = true;
            numChgLim.Maximum = Math.Abs(chgLimConf.MaxVal - chgLimConf.MinVal);
            if (chgLimConf.CurVal == 0)
            {
                chkChgLim.Checked = false;
                numChgLim.Value = 80;
            }
            else
            {
                chkChgLim.Checked = true;
                numChgLim.Value = chgLimConf.CurVal;
            }
        }

        cboPerfMode.Items.Clear();
        cboProfPerfMode.Items.Clear();

        if (cfg.PerfModeConf is null)
        {
            ttMain.SetToolTip(cboPerfMode, Strings.GetString("ttNotSupported"));
        }
        else
        {
            PerfModeConf perfModeConf = cfg.PerfModeConf;
            cboProfPerfMode.Items.Add($"Default ({perfModeConf.PerfModes[perfModeConf.ModeSel].Name})");
            for (int i = 0; i < perfModeConf.PerfModes.Count; i++)
            {
                cboPerfMode.Items.Add(perfModeConf.PerfModes[i].Name);
                cboProfPerfMode.Items.Add(perfModeConf.PerfModes[i].Name);
            }

            cboPerfMode.SelectedIndex = perfModeConf.ModeSel;
            cboPerfMode.Enabled = true;
        }

        cboFanMode.Items.Clear();
        if (cfg.FanModeConf is null)
        {
            ttMain.SetToolTip(cboFanMode, Strings.GetString("ttNotSupported"));
        }
        else
        {
            FanModeConf fanModeConf = cfg.FanModeConf;
            for (int i = 0; i < fanModeConf.FanModes.Count; i++)
            {
                cboFanMode.Items.Add(fanModeConf.FanModes[i].Name);
            }

            cboFanMode.SelectedIndex = fanModeConf.ModeSel;
            cboFanMode.Enabled = tsiAdvanced.Checked;
        }

        if (cfg.KeySwapConf is null)
        {
            ttMain.SetToolTip(chkWinFnSwap, Strings.GetString("ttNotSupported"));
        }
        else
        {
            chkWinFnSwap.Checked = cfg.KeySwapConf.Enabled;
            ttMain.SetToolTip(chkWinFnSwap, Strings.GetString("ttKeySwap"));
            chkWinFnSwap.Enabled = true;
        }

        cboFanSel.Items.Clear();
        for (int i = 0; i < cfg.FanConfs.Count; i++)
        {
            cboFanSel.Items.Add(cfg.FanConfs[i].Name);
        }

        btnProfAdd.Enabled = true;
        tsiProfAdd.Enabled = true;
        tsiProfEdit.Enabled = true;
        tsiECtoConf.Enabled = true;
        cboFanSel.Enabled = true;
        cboFanSel.SelectedIndex = 0;
        tsiECMon.Enabled = true;

        if (cfg.RegConfs?.Count > 0)
        {
            // RegConfs will be removed in a future version of YAMDCC
            // due to no longer being needed on MSI laptops
            MessageBox.Show(Strings.GetString("dlgRegConfWarn"), "Warning",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        UpdateStatus(StatusCode.None);
        ToggleSvcCmds(true);
    }

    private void PollEC()
    {
        SendSvcMessage(new ServiceCommand(Command.GetTemp, cboFanSel.SelectedIndex));
        SendSvcMessage(new ServiceCommand(Command.GetFanSpeed, cboFanSel.SelectedIndex));
        SendSvcMessage(new ServiceCommand(Command.GetFanRPM, cboFanSel.SelectedIndex));
    }

    private static Label FanCurveLabel(string text, float scale, ContentAlignment align = ContentAlignment.MiddleRight)
    {
        return new Label
        {
            AutoSize = true,
            Dock = DockStyle.Fill,
            Margin = new Padding((int)(3 * scale)),
            Padding = new Padding(0, 0, 0, (int)(3 * scale)),
            Text = text,
            TextAlign = align,
        };
    }

    private static NumericUpDown FanCurveNUD(int tag, float scale)
    {
        return new NumericUpDown()
        {
            Dock = DockStyle.Fill,
            Height = (int)(23 * scale),
            Margin = new Padding((int)(3 * scale)),
            Tag = tag,
        };
    }

    private void DisableAll()
    {
        ToggleSvcCmds(false);
        tsiSaveConf.Enabled = false;
        tsiProfAdd.Enabled = false;
        tsiProfEdit.Enabled = false;
        tsiProfDel.Enabled = false;
        tsiECtoConf.Enabled = false;
        tsiECMon.Enabled = false;

        btnProfAdd.Enabled = false;
        btnProfDel.Enabled = false;
        cboFanSel.Enabled = false;
        cboProfSel.Enabled = false;
        cboProfPerfMode.Enabled = false;
        if (tbFanSpds is not null)
        {
            for (int i = 0; i < tbFanSpds.Length; i++)
            {
                tbFanSpds[i].Enabled = false;
                numFanSpds[i].Enabled = false;
                if (i != 0)
                {
                    numUpTs[i - 1].Enabled = false;
                    numDownTs[i - 1].Enabled = false;
                }
            }
        }

        cboPerfMode.Enabled = false;
        cboFanMode.Enabled = false;
        chkWinFnSwap.Enabled = false;
        chkChgLim.Enabled = false;
        numChgLim.Enabled = false;
        lblKeyLightLow.Enabled = false;
        lblKeyLightHigh.Enabled = false;
        tbKeyLight.Enabled = false;

        txtAuthor.Enabled = false;
        btnGetModel.Enabled = false;
    }

    private void ToggleSvcCmds(bool enable)
    {
        tsiApply.Enabled = enable;
        tsiRevert.Enabled = enable;
        chkFullBlast.Enabled = enable;
        btnApply.Enabled = enable;
        btnRevert.Enabled = enable;
    }

    private void UpdateStatus(StatusCode status, int data = 0)
    {
        if (AppStatus.Code == status)
        {
            AppStatus.Repeats++;
        }
        else
        {
            AppStatus.Code = status;
            AppStatus.Repeats = 0;
        }

        // set status text
        bool persist = false;
        switch (AppStatus.Code)
        {
            case StatusCode.ServiceCommandFail:
                persist = true;
                lblStatus.Text = Strings.GetString("statSvcErr", (Command)data);
                break;
            case StatusCode.ServiceResponseEmpty:
                lblStatus.Text = Strings.GetString("statResponseEmpty");
                break;
            case StatusCode.ServiceTimeout:
                persist = true;
                lblStatus.Text = Strings.GetString("statSvcTimeout");
                break;
            case StatusCode.NoConfig:
                persist = true;
                lblStatus.Text = Strings.GetString("statNoConf");
                break;
            case StatusCode.ConfLoading:
                lblStatus.Text = Strings.GetString("statConfLoading");
                break;
            case StatusCode.ConfApplied:
                lblStatus.Text = Strings.GetString("statConfApplied");
                break;
            case StatusCode.FullBlastToggled:
                lblStatus.Text = Strings.GetString("statFBToggled");
                break;
            default:
                persist = true;
                AppStatus.Repeats = 0;
                lblStatus.Text = "Ready";
                break;
        }

        if (AppStatus.Repeats > 0)
        {
            lblStatus.Text += $" (x{AppStatus.Repeats + 1})";
        }

        tmrStatusReset.Stop();
        if (!persist)
        {
            tmrStatusReset.Start();
        }
    }

    private void SendSvcMessage(ServiceCommand command)
    {
        IPCClient.PushMessage(command);
        tmrSvcTimeout.Start();
    }
    #endregion  // Private methods
}
