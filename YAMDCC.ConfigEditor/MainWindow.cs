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
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using YAMDCC.Common;
using YAMDCC.Common.Dialogs;
using YAMDCC.Config;
using YAMDCC.IPC;
using YAMDCC.Logs;

namespace YAMDCC.ConfigEditor
{
    internal sealed partial class MainWindow : Form
    {
        #region Fields
        private readonly Status AppStatus = new();

        private readonly CommonConfig GlobalConfig;

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

        public MainWindow()
        {
            InitializeComponent();

            // Set the window icon using the application icon.
            // Saves about 8-9 KB from not having to embed the same icon twice.
            Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);

            // set title text to include program version
            Text = $"YAMDCC config editor - v{Utils.GetVerString()}";

            // set literally every tooltip
            tsiLoadConf.ToolTipText = Strings.GetString("ttLoadConf");
            tsiSaveConf.ToolTipText = Strings.GetString("ttSaveConf");
            tsiApply.ToolTipText = Strings.GetString("ttApply");
            tsiRevert.ToolTipText = Strings.GetString("ttRevert");
            tsiExit.ToolTipText = Strings.GetString("ttExit");
            tsiProfAdd.ToolTipText = Strings.GetString("ttProfAdd");
            tsiProfRename.ToolTipText = Strings.GetString("ttProfRen");
            tsiProfChangeDesc.ToolTipText = Strings.GetString("ttProfChangeDesc");
            tsiSwitchAll.ToolTipText = Strings.GetString("ttSwitchAll");
            tsiECtoConf.ToolTipText = Strings.GetString("ttECtoConf");
            tsiProfDel.ToolTipText = Strings.GetString("ttProfDel");
            tsiECMon.ToolTipText = Strings.GetString("ttECMon");
            tsiStopSvc.ToolTipText = Strings.GetString("ttSvcStop");
            tsiUninstall.ToolTipText = Strings.GetString("ttSvcUninstall");
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
            tmrPoll.Tick += tmrPoll_Tick;

            tmrStatusReset = new()
            {
                Interval = 5000,
            };
            tmrStatusReset.Tick += tmrStatusReset_Tick;

            tmrSvcTimeout = new()
            {
                Interval = 10000,
            };
            tmrSvcTimeout.Tick += tmrSvcTimeout_Tick;

            GlobalConfig = CommonConfig.Load();
            if (GlobalConfig.App == "YAMDCC")
            {
                switch (GlobalConfig.LogLevel)
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
            }
            else
            {
                tsiLogDebug.Checked = true;
            }

            DisableAll();
        }

        #region Events
        private void MainWindow_Load(object sender, EventArgs e)
        {
            IPCClient.ServerMessage += IPC_MessageReceived;
            IPCClient.Error += IPCClient_Error;
            IPCClient.Start();

            ProgressDialog dlg = new("Connecting to YAMDCC service...",
                (e) => e.Result = !IPCClient.WaitForConnection(5000));
            dlg.ShowDialog();

            if (dlg.Result is bool b && b)
            {
                throw new TimeoutException(Strings.GetString("exSvcTimeout"));
            }
            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;

            LoadConf(Paths.CurrentConfig);

            if (Config is not null)
            {
                if (Config.KeyLightConf is null)
                {
                    ttMain.SetToolTip(tbKeyLight, Strings.GetString("ttNotSupported"));
                }
                else
                {
                    SendServiceMessage(new ServiceCommand(Command.GetKeyLightBright, string.Empty));
                }
            }

            if (File.Exists(Paths.ECToConfFail))
            {
                Utils.ShowError(Strings.GetString("dlgECtoConfErr", Paths.Logs));
            }
            else if (File.Exists(Paths.ECToConfSuccess))
            {
                MessageBox.Show(Strings.GetString("dlgECtoConfSuccess"),
                    "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            try
            {
                File.Delete(Paths.ECToConfSuccess);
                File.Delete(Paths.ECToConfFail);
            }
            catch (DirectoryNotFoundException) { }
        }

        private void MainWindow_Closing(object sender, FormClosingEventArgs e)
        {
            // Disable Full Blast if it was enabled while the program was running:
            if (chkFullBlast.Checked)
            {
                SendServiceMessage(new ServiceCommand(Command.FullBlast, "0"));
            }
            GlobalConfig.App = "YAMDCC";
            try
            {
                GlobalConfig.Save();
            }
            // ignore DirectoryNotFoundException, since we probably closed the
            // window due to uninstalling with data directory delete enabled
            catch (DirectoryNotFoundException) { }
        }

        private void OnProcessExit(object sender, EventArgs e)
        {
            // Close the connection to the YAMDCC
            // Service before exiting the program:
            tmrPoll.Stop();
            IPCClient.Stop();
        }

        private void IPC_MessageReceived(object sender, PipeMessageEventArgs<ServiceResponse, ServiceCommand> e)
        {
            tmrSvcTimeout.Stop();
            string[] args = e.Message.Value.Split(' ');
            if (args.Length == 1)
            {
                switch (e.Message.Response)
                {
                    case Response.Nothing:
                    {
                        UpdateStatus(StatusCode.ServiceResponseEmpty);
                        break;
                    }
                    case Response.Success:
                    {
                        HandleSuccessResponse(args);
                        break;
                    }
                    case Response.Error:
                    {
                        if (int.TryParse(args[0], out int value))
                        {
                            UpdateStatus(StatusCode.ServiceCommandFail, value);
                        }
                        break;
                    }
                    case Response.Temp:
                    {
                        if (int.TryParse(args[0], out int value))
                        {
                            UpdateFanMon(value, 0);
                        }
                        break;
                    }
                    case Response.FanSpeed:
                    {
                        if (int.TryParse(args[0], out int value))
                        {
                            UpdateFanMon(value, 1);
                        }
                        break;
                    }
                    case Response.FanRPM:
                    {
                        if (int.TryParse(args[0], out int value))
                        {
                            UpdateFanMon(value, 2);
                        }
                        break;
                    }
                    case Response.KeyLightBright:
                    {
                        if (int.TryParse(args[0], out int value))
                        {
                            // value received from service should be valid,
                            // but let's check anyway to avoid potential crashes
                            // from non-official YAMDCC services
                            if (value < 0 || value > Config.KeyLightConf.MaxVal - Config.KeyLightConf.MinVal)
                            {
                                break;
                            }

                            tbKeyLight.Invoke(new Action(delegate
                            {
                                tbKeyLight.Maximum = Config.KeyLightConf.MaxVal - Config.KeyLightConf.MinVal;
                                tbKeyLight.Value = value;
                                tbKeyLight.Enabled = lblKeyLightLow.Enabled = lblKeyLightHigh.Enabled = true;
                                ttMain.SetToolTip(tbKeyLight, Strings.GetString("ttKeyLight"));
                            }));
                        }
                        break;
                    }
                }
            }
        }

        private void IPCClient_Error(object sender, PipeErrorEventArgs<ServiceResponse, ServiceCommand> e)
        {
            CrashDialog dlg = new(e.Exception);
            dlg.ShowDialog();
        }

        private void HandleSuccessResponse(string[] args)
        {
            if (int.TryParse(args[0], out int value))
            {
                Command cmd = (Command)value;

                switch (cmd)
                {
                    case Command.ApplyConfig:
                        btnApply.Invoke(() => btnApply.Enabled = tsiApply.Enabled = true);
                        UpdateStatus(StatusCode.ConfApplySuccess);
                        if (Config.KeyLightConf is not null)
                        {
                            SendServiceMessage(new ServiceCommand(Command.GetKeyLightBright, ""));
                        }
                        break;
                    case Command.FullBlast:
                        UpdateStatus(StatusCode.FullBlastToggleSuccess);
                        break;
                }
            }
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
                SetLastConfPath(ofd.FileName);
                btnRevert.Enabled = tsiRevert.Enabled = false;
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
                SetLastConfPath(sfd.FileName);
                btnRevert.Enabled = tsiRevert.Enabled = false;
            }
        }

        private void tsiApply_Click(object sender, EventArgs e)
        {
            ApplyConf();
        }

        private void tsiRevert_Click(object sender, EventArgs e)
        {
            RevertConf();
        }

        private void tsiExit_Click(object sender, EventArgs e)
        {
            Close();
        }
        #endregion

        #region Options
        private void tsiProfAdd_Click(object sender, EventArgs e)
        {
            AddFanProfile();
        }

        private void tsiProfRename_Click(object sender, EventArgs e)
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
                btnRevert.Enabled = tsiRevert.Enabled = true;
            }
        }

        private void tsiProfChangeDesc_Click(object sender, EventArgs e)
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
                btnRevert.Enabled = tsiRevert.Enabled = true;
            }
        }

        private void tsiProfDel_Click(object sender, EventArgs e)
        {
            DelFanProfile();
        }

        private void tsiECtoConf_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(Strings.GetString("dlgECtoConfStart"),
                "Default fan profile from EC?", MessageBoxButtons.YesNo,
                MessageBoxIcon.Information) == DialogResult.Yes)
            {
                StreamWriter sw = new(Paths.ECToConfPending, false);
                try
                {
                    sw.Write(1);
                    sw.Flush();
                }
                finally
                {
                    sw.Close();
                }
                Application.Exit();
            }
        }

        private void tsiECMon_Click(object sender, EventArgs e)
        {
            if (tsiECMon.Checked)
            {
                tmrPoll.Start();
                PollEC();
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

        private void tsiStopSvc_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(
                Strings.GetString("dlgSvcStop"), "Stop Service",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                tmrPoll.Stop();
                IPCClient.Stop();
                Hide();

                ProgressDialog dlg = new(Strings.GetString("dlgSvcStopping"),
                    static (e) =>
                    {
                        if (!Utils.StopService("yamdccsvc"))
                        {
                            Utils.ShowError(Strings.GetString("dlgSvcStopErr"));
                        }
                    });
                dlg.ShowDialog();

                Close();
            }
        }

        private void tsiUninstall_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(Strings.GetString("dlgUninstall"), "Uninstall Service",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                bool delData = MessageBox.Show(
                    Strings.GetString("dlgSvcDelData", Paths.Data),
                    "Delete configuration data?",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2) == DialogResult.Yes;

                tmrPoll.Stop();
                IPCClient.Stop();
                Hide();

                // Apparently this fixes the YAMDCC service not uninstalling
                // when YAMDCC is launched by certain means
                ProgressDialog dlg = new(Strings.GetString("dlgSvcUninstalling"), (e) =>
                {
                    if (Utils.StopService("yamdccsvc"))
                    {
                        if (Utils.UninstallService("yamdccsvc"))
                        {
                            // Only delete service data if the
                            // service uninstalled successfully
                            if (delData)
                            {
                                Directory.Delete(Paths.Data, true);
                            }
                        }
                        else
                        {
                            Utils.ShowError(Strings.GetString("dlgUninstallErr"));
                        }
                    }
                    else
                    {
                        Utils.ShowError(Strings.GetString("dlgSvcStopErr"));
                    }
                });
                dlg.ShowDialog();

                Close();
            }
        }
        #endregion

        #region Help
        private void tsiAbout_Click(object sender, EventArgs e) =>
            new VersionDialog().ShowDialog();

        private void tsiSrc_Click(object sender, EventArgs e) =>
            Process.Start(Paths.SourcePrefix);
        #endregion

        #endregion

        private void cboFanSel_IndexChanged(object sender, EventArgs e)
        {
            if (Config is not null)
            {
                UpdateFanCurveDisplay();
            }
        }

        private void cboProfSel_IndexChanged(object sender, EventArgs e)
        {
            FanConf cfg = Config.FanConfs[cboFanSel.SelectedIndex];
            FanCurveConf curveCfg = cfg.FanCurveConfs[cboProfSel.SelectedIndex];

            if (tsiSwitchAll.Checked)
            {
                for (int i = 0; i < Config.FanConfs.Length; i++)
                {
                    Config.FanConfs[i].CurveSel = cboProfSel.SelectedIndex;
                }
            }
            else
            {
                cfg.CurveSel = cboProfSel.SelectedIndex;
            }

            ttMain.SetToolTip(cboProfSel, Strings.GetString(
                "ttProfSel", cfg.FanCurveConfs[cfg.CurveSel].Desc));

            int numTempThresholds = cfg.UpThresholdRegs.Length;

            // Fan curve
            for (int i = 0; i < numFanSpds.Length; i++)
            {
                if (i <= numTempThresholds)
                {
                    numFanSpds[i].Value = tbFanSpds[i].Value
                        = curveCfg.TempThresholds[i].FanSpeed;

                    numFanSpds[i].Enabled = tbFanSpds[i].Enabled = cfg.CurveSel != 0;
                }
            }

            // Temp thresholds
            for (int i = 0; i < numUpTs.Length; i++)
            {
                if (i <= numTempThresholds)
                {
                    TempThreshold t = curveCfg.TempThresholds[i + 1];
                    numUpTs[i].Value = t.UpThreshold;
                    numDownTs[i].Value = t.DownThreshold;

                    numUpTs[i].Enabled = numDownTs[i].Enabled = cfg.CurveSel != 0;
                }
                else
                {
                    numUpTs[i].Enabled = numDownTs[i].Enabled = false;
                }
            }
            btnApply.Enabled = tsiApply.Enabled = true;
            btnProfDel.Enabled = tsiProfDel.Enabled = cfg.CurveSel != 0;
        }

        private void btnProfAdd_Click(object sender, EventArgs e)
        {
            AddFanProfile();
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

        private void btnProfDel_Click(object sender, EventArgs e)
        {
            DelFanProfile();
        }

        private void numFanSpd_Changed(object sender, EventArgs e)
        {
            NumericUpDown nud = (NumericUpDown)sender;
            int i = (int)nud.Tag;
            tbFanSpds[i].Value = (int)numFanSpds[i].Value;

            Config.FanConfs[cboFanSel.SelectedIndex]
                .FanCurveConfs[cboProfSel.SelectedIndex]
                .TempThresholds[i].FanSpeed = (byte)numFanSpds[i].Value;

            btnRevert.Enabled = tsiRevert.Enabled = true;
        }

        private void tbFanSpd_Scroll(object sender, EventArgs e)
        {
            TrackBar tb = (TrackBar)sender;
            int i = (int)tb.Tag;
            numFanSpds[i].Value = tbFanSpds[i].Value;

            Config.FanConfs[cboFanSel.SelectedIndex]
                .FanCurveConfs[cboProfSel.SelectedIndex]
                .TempThresholds[i].FanSpeed = (byte)numFanSpds[i].Value;

            btnRevert.Enabled = tsiRevert.Enabled = true;
        }

        private void numUpT_Changed(object sender, EventArgs e)
        {
            NumericUpDown nud = (NumericUpDown)sender;
            int i = (int)nud.Tag;

            TempThreshold threshold = Config.FanConfs[cboFanSel.SelectedIndex]
                .FanCurveConfs[cboProfSel.SelectedIndex]
                .TempThresholds[i + 1];

            // Update associated down threshold slider
            numDownTs[i].Value += nud.Value - threshold.UpThreshold;

            threshold.UpThreshold = (byte)numUpTs[i].Value;

            btnRevert.Enabled = tsiRevert.Enabled = true;
        }

        private void numDownT_Changed(object sender, EventArgs e)
        {
            NumericUpDown nud = (NumericUpDown)sender;
            int i = (int)nud.Tag;

            Config.FanConfs[cboFanSel.SelectedIndex]
                .FanCurveConfs[cboProfSel.SelectedIndex]
                .TempThresholds[i + 1].DownThreshold = (byte)numDownTs[i].Value;

            btnRevert.Enabled = tsiRevert.Enabled = true;
        }

        private void chkFullBlast_Toggled(object sender, EventArgs e)
        {
            SendServiceMessage(new ServiceCommand(Command.FullBlast, chkFullBlast.Checked ? "1" : "0"));
        }

        private void chkChgLim_CheckedChanged(object sender, EventArgs e)
        {
            numChgLim.Enabled = chkChgLim.Checked;
        }

        private void numChgLim_Changed(object sender, EventArgs e)
        {
            if (Config is not null)
            {
                btnRevert.Enabled = tsiRevert.Enabled = true;
            }
        }

        private void cboPerfMode_IndexChanged(object sender, EventArgs e)
        {
            if (Config is not null)
            {
                int idx = cboPerfMode.SelectedIndex;
                Config.PerfModeConf.ModeSel = idx;
                ttMain.SetToolTip(cboPerfMode,
                    Strings.GetString("ttPerfMode", Config.PerfModeConf.PerfModes[idx].Desc));
                btnRevert.Enabled = tsiRevert.Enabled = true;
            }
        }

        private void chkWinFnSwap_Toggled(object sender, EventArgs e)
        {
            Config.KeySwapConf.Enabled = chkWinFnSwap.Checked;
            btnRevert.Enabled = tsiRevert.Enabled = true;
        }

        private void tbKeyLight_Scroll(object sender, EventArgs e)
        {
            SendServiceMessage(new ServiceCommand(Command.SetKeyLightBright, $"{tbKeyLight.Value}"));
        }

        private void btnRevert_Click(object sender, EventArgs e)
        {
            RevertConf();
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            btnApply.Enabled = tsiApply.Enabled = false;
            ApplyConf();
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
        private void UpdateFanMon(int value, int i)
        {
            switch (i)
            {
                case 0:
                    lblTemp.Invoke(new Action(delegate
                    {
                        lblTemp.Text = $"Temp: {value}°C";
                    }));
                    break;
                case 1:
                    lblFanSpd.Invoke(new Action(delegate
                    {
                        lblFanSpd.Text = $"Fan speed: {value}%";
                    }));
                    break;
                case 2:
                    lblFanRPM.Invoke(new Action(delegate
                    {
                        lblFanRPM.Text = value == -1
                            ? "RPM: 0"
                            : $"RPM: {value}";
                    }));
                    break;
            }
        }

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
            tsiSwitchAll.Checked = FansHaveSameProfileCount();

            tsiSaveConf.Enabled = true;

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
                chkChgLim.Enabled = numChgLim.Enabled = true;
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
            if (cfg.PerfModeConf is null)
            {
                ttMain.SetToolTip(cboPerfMode, Strings.GetString("ttNotSupported"));
            }
            else
            {
                PerfModeConf perfModeConf = cfg.PerfModeConf;
                for (int i = 0; i < perfModeConf.PerfModes.Length; i++)
                {
                    cboPerfMode.Items.Add(perfModeConf.PerfModes[i].Name);
                }

                cboPerfMode.SelectedIndex = perfModeConf.ModeSel;
                ttMain.SetToolTip(cboPerfMode, Strings.GetString(
                    "ttPerfMode", perfModeConf.PerfModes[perfModeConf.ModeSel].Desc));
                cboPerfMode.Enabled = true;
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
            for (int i = 0; i < cfg.FanConfs.Length; i++)
            {
                cboFanSel.Items.Add(cfg.FanConfs[i].Name);
            }

            btnProfAdd.Enabled = true;
            tsiProfAdd.Enabled = tsiProfEdit.Enabled = true;
            tsiECtoConf.Enabled = true;
            cboFanSel.Enabled = true;
            cboFanSel.SelectedIndex = 0;
            tsiECMon.Enabled = true;

            UpdateStatus(StatusCode.None);
        }

        private void ApplyConf()
        {
            // Save the updated config
            Config.ChargeLimitConf.CurVal = (byte)(chkChgLim.Checked
                ? numChgLim.Value : 0);
            Config.Save(Paths.CurrentConfig);

            // Tell the service to reload and apply the updated config
            SendServiceMessage(new ServiceCommand(Command.ApplyConfig, null));
        }

        private void RevertConf()
        {
            if (MessageBox.Show(
                Strings.GetString("dlgRevert"),
                "Revert?", MessageBoxButtons.YesNo, MessageBoxIcon.Warning)
                == DialogResult.Yes)
            {
                try
                {
                    YAMDCC_Config tempConf = YAMDCC_Config.Load(GetLastConfPath());
                    LoadConf(tempConf);
                    Config = tempConf;
                    UpdateFanCurveDisplay();
                    ApplyConf();
                    btnRevert.Enabled = tsiRevert.Enabled = false;
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

        private void PollEC()
        {
            SendServiceMessage(new ServiceCommand(Command.GetTemp, $"{cboFanSel.SelectedIndex}"));
            SendServiceMessage(new ServiceCommand(Command.GetFanSpeed, $"{cboFanSel.SelectedIndex}"));
            SendServiceMessage(new ServiceCommand(Command.GetFanRPM, $"{cboFanSel.SelectedIndex}"));
        }

        private void AddFanProfile()
        {
            if (tsiSwitchAll.Checked)
            {
                bool switchAll = tsiSwitchAll.Checked;
                tsiSwitchAll.Checked = false;
                AddFanProfImpl(0, cboFanSel.Items.Count);
                tsiSwitchAll.Checked = switchAll;
            }
            else
            {
                AddFanProfImpl(cboFanSel.SelectedIndex, cboFanSel.SelectedIndex + 1);
            }

            if (!FansHaveSameProfileCount())
            {
                tsiSwitchAll.Checked = false;
            }

            btnRevert.Enabled = tsiRevert.Enabled = true;
        }

        private void AddFanProfImpl(int start, int end)
        {
            FanConf cfg = Config.FanConfs[cboFanSel.SelectedIndex];
            string oldProfName = cfg.FanCurveConfs[cboProfSel.SelectedIndex].Name;

            TextInputDialog dlg = new(
                Strings.GetString("dlgProfAdd"),
                "New Profile", $"Copy of {oldProfName}");

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                for (int i = start; i < end; i++)
                {
                    cfg = Config.FanConfs[i];
                    FanCurveConf oldCurveCfg = cfg.FanCurveConfs[cfg.CurveSel];

                    // Create a copy of the currently selected fan profile:
                    FanCurveConf newCurveCfg = oldCurveCfg.Copy();

                    // Name it according to what the user specified
                    newCurveCfg.Name = dlg.Result;
                    newCurveCfg.Desc = $"(Copy of {oldCurveCfg.Name})\n{oldCurveCfg.Desc}";

                    // Add the new fan profile to the config's list
                    cfg.FanCurveConfs = [.. cfg.FanCurveConfs, newCurveCfg];
                    cfg.CurveSel = cfg.FanCurveConfs.Length - 1;

                    // Add the new fan profile to the UI's profile list and select it:
                    if (i == cboFanSel.SelectedIndex)
                    {
                        cboProfSel.Items.Add(dlg.Result);
                        cboProfSel.SelectedIndex = cfg.CurveSel;
                    }
                }
            }
        }

        private void DelFanProfile()
        {
            if (tsiSwitchAll.Checked)
            {
                DelFanProfImpl(0, cboFanSel.Items.Count);
            }
            else
            {
                DelFanProfImpl(cboFanSel.SelectedIndex, cboFanSel.SelectedIndex + 1);
            }

            if (!FansHaveSameProfileCount())
            {
                tsiSwitchAll.Checked = false;
            }

            btnRevert.Enabled = tsiRevert.Enabled = true;
        }

        private void DelFanProfImpl(int start, int end)
        {
            for (int i = start; i < end; i++)
            {
                FanConf cfg = Config.FanConfs[i];
                FanCurveConf curveCfg = cfg.FanCurveConfs[cfg.CurveSel];

                if (cfg.CurveSel != 0 && MessageBox.Show(
                    Strings.GetString("dlgProfDel", curveCfg.Name),
                    $"Delete fan profile? ({cfg.Name})", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    // Remove the fan profile from the config's list
                    List<FanCurveConf> curveCfgList = [.. cfg.FanCurveConfs];
                    curveCfgList.RemoveAt(cfg.CurveSel);
                    cfg.FanCurveConfs = [.. curveCfgList];
                    cfg.CurveSel -= 1;

                    // Remove from the list client-side, and select a different fan profile
                    if (i == cboFanSel.SelectedIndex)
                    {
                        cboProfSel.Items.RemoveAt(cboProfSel.SelectedIndex);
                        cboProfSel.SelectedIndex = cfg.CurveSel;
                    }
                }
            }
        }

        private static string GetLastConfPath()
        {
            StreamReader sr = new(Paths.LastConfig, Encoding.UTF8);
            try
            {
                string path = sr.ReadLine();
                return path;
            }
            finally
            {
                sr.Close();
            }
        }

        private static void SetLastConfPath(string path)
        {
            StreamWriter sw = new(Paths.LastConfig, false, Encoding.UTF8);
            try
            {
                sw.WriteLine(path);
            }
            finally
            {
                sw.Close();
            }
        }

        private void UpdateFanCurveDisplay()
        {
            FanConf cfg = Config.FanConfs[cboFanSel.SelectedIndex];

            cboProfSel.Items.Clear();
            foreach (FanCurveConf curve in cfg.FanCurveConfs)
            {
                cboProfSel.Items.Add(curve.Name);
            }

            if (numUpTs is null || numDownTs is null || numFanSpds is null || tbFanSpds is null ||
                numUpTs.Length != cfg.UpThresholdRegs.Length ||
                numDownTs.Length != cfg.DownThresholdRegs.Length ||
                numFanSpds.Length != cfg.FanCurveRegs.Length ||
                tbFanSpds.Length != cfg.FanCurveRegs.Length)
            {
                float scale = CurrentAutoScaleDimensions.Height / 72;

                tblCurve.Controls.Clear();
                numUpTs = new NumericUpDown[cfg.UpThresholdRegs.Length];
                numDownTs = new NumericUpDown[cfg.DownThresholdRegs.Length];
                numFanSpds = new NumericUpDown[cfg.FanCurveRegs.Length];
                tbFanSpds = new TrackBar[cfg.FanCurveRegs.Length];

                tblCurve.ColumnStyles.Clear();
                tblCurve.ColumnCount = numFanSpds.Length + 2;

                // labels on left side
                tblCurve.ColumnStyles.Add(new ColumnStyle());
                tblCurve.Controls.Add(FanCurveLabel("Speed (%)", scale, ContentAlignment.MiddleRight), 0, 0);
                tblCurve.Controls.Add(FanCurveLabel("Up (°C)", scale, ContentAlignment.MiddleRight), 0, 2);
                tblCurve.Controls.Add(FanCurveLabel("Down (°C)", scale, ContentAlignment.MiddleRight), 0, 3);

                for (int i = 0; i < numFanSpds.Length; i++)
                {
                    tblCurve.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F / numFanSpds.Length));
                    numFanSpds[i] = FanCurveNUD(i, scale);
                    ttMain.SetToolTip(numFanSpds[i], Strings.GetString("ttFanSpd"));
                    numFanSpds[i].ValueChanged += numFanSpd_Changed;
                    tblCurve.Controls.Add(numFanSpds[i], i + 1, 0);

                    tbFanSpds[i] = new TrackBar()
                    {
                        Dock = DockStyle.Fill,
                        LargeChange = 10,
                        Margin = new Padding((int)(10 * scale), 0, (int)(10 * scale), 0),
                        Orientation = Orientation.Vertical,
                        Tag = i,
                        TickFrequency = 5,
                        TickStyle = TickStyle.Both,
                    };
                    ttMain.SetToolTip(tbFanSpds[i], Strings.GetString("ttFanSpd"));
                    tbFanSpds[i].ValueChanged += tbFanSpd_Scroll;
                    tblCurve.Controls.Add(tbFanSpds[i], i + 1, 1);

                    if (i != 0)
                    {
                        numUpTs[i - 1] = FanCurveNUD(i - 1, scale);
                        ttMain.SetToolTip(numUpTs[i - 1], Strings.GetString("ttUpT"));
                        numUpTs[i - 1].ValueChanged += numUpT_Changed;
                        tblCurve.Controls.Add(numUpTs[i - 1], i + 1, 2);
                    }
                    else
                    {
                        tblCurve.Controls.Add(FanCurveLabel("Default", scale), i + 1, 2);
                    }

                    if (i != numFanSpds.Length - 1)
                    {
                        numDownTs[i] = FanCurveNUD(i, scale);
                        ttMain.SetToolTip(numDownTs[i], Strings.GetString("ttDownT"));
                        numDownTs[i].ValueChanged += numDownT_Changed;
                        tblCurve.Controls.Add(numDownTs[i], i + 1, 3);
                    }
                    else
                    {
                        tblCurve.Controls.Add(FanCurveLabel("Max", scale), i + 1, 3);
                    }
                }
            }

            for (int i = 0; i < numFanSpds.Length; i++)
            {
                if (cfg.FanCurveRegs.Length >= i)
                {
                    numFanSpds[i].Maximum = tbFanSpds[i].Maximum
                        = Math.Abs(cfg.MaxSpeed - cfg.MinSpeed);
                }
                else
                {
                    numFanSpds[i].Enabled = tbFanSpds[i].Enabled = false;
                }
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

        private static Label FanCurveLabel(string text, float scale, ContentAlignment textAlign = ContentAlignment.MiddleCenter)
        {
            return new Label
            {
                AutoSize = true,
                Dock = DockStyle.Fill,
                Margin = new Padding((int)(2 * scale)),
                Padding = new Padding(0, 0, 0, (int)(3 * scale)),
                Text = text,
                TextAlign = textAlign,
            };
        }

        private void tsiLogNone_Click(object sender, EventArgs e)
        {
            tsiLogNone.Checked = true;
            tsiLogDebug.Checked = false;
            tsiLogInfo.Checked = false;
            tsiLogWarn.Checked = false;
            tsiLogError.Checked = false;
            tsiLogFatal.Checked = false;
        }

        private void tsiLogDebug_Click(object sender, EventArgs e)
        {
            tsiLogNone.Checked = false;
            tsiLogDebug.Checked = true;
            tsiLogInfo.Checked = false;
            tsiLogWarn.Checked = false;
            tsiLogError.Checked = false;
            tsiLogFatal.Checked = false;
        }

        private void tsiLogInfo_Click(object sender, EventArgs e)
        {
            tsiLogNone.Checked = false;
            tsiLogDebug.Checked = false;
            tsiLogInfo.Checked = true;
            tsiLogWarn.Checked = false;
            tsiLogError.Checked = false;
            tsiLogFatal.Checked = false;
        }

        private void tsiLogWarn_Click(object sender, EventArgs e)
        {
            tsiLogNone.Checked = false;
            tsiLogDebug.Checked = false;
            tsiLogInfo.Checked = false;
            tsiLogWarn.Checked = true;
            tsiLogError.Checked = false;
            tsiLogFatal.Checked = false;
        }

        private void tsiLogError_Click(object sender, EventArgs e)
        {
            tsiLogNone.Checked = false;
            tsiLogDebug.Checked = false;
            tsiLogInfo.Checked = false;
            tsiLogWarn.Checked = false;
            tsiLogError.Checked = true;
            tsiLogFatal.Checked = false;
        }

        private void tsiLogFatal_Click(object sender, EventArgs e)
        {
            tsiLogNone.Checked = false;
            tsiLogDebug.Checked = false;
            tsiLogInfo.Checked = false;
            tsiLogWarn.Checked = false;
            tsiLogError.Checked = false;
            tsiLogFatal.Checked = true;
        }

        private void tsiSwitchAll_Click(object sender, EventArgs e)
        {
            if (!tsiSwitchAll.Checked)
            {
                if (FansHaveSameProfileCount())
                {
                    tsiSwitchAll.Checked = true;
                }
                else
                {
                    Utils.ShowError("All fans must have the same number of fan profiles.");
                }
            }
            else
            {
                tsiSwitchAll.Checked = false;
            }

        }

        private static NumericUpDown FanCurveNUD(int tag, float scale)
        {
            return new NumericUpDown()
            {
                Dock = DockStyle.Fill,
                Height = (int)(23 * scale),
                Margin = new Padding((int)(2 * scale)),
                Tag = tag,
            };
        }

        private void DisableAll()
        {
            btnProfAdd.Enabled = false;
            btnProfDel.Enabled = false;
            btnRevert.Enabled = false;
            btnApply.Enabled = false;
            cboFanSel.Enabled = false;
            cboProfSel.Enabled = false;
            cboPerfMode.Enabled = false;
            chkFullBlast.Enabled = false;
            chkWinFnSwap.Enabled = false;
            chkChgLim.Enabled = false;
            numChgLim.Enabled = false;
            lblKeyLightLow.Enabled = false;
            lblKeyLightHigh.Enabled = false;
            tbKeyLight.Enabled = false;

            tsiSaveConf.Enabled = false;
            tsiApply.Enabled = false;
            tsiRevert.Enabled = false;
            tsiProfAdd.Enabled = false;
            tsiProfEdit.Enabled = false;
            tsiProfDel.Enabled = false;
            tsiECtoConf.Enabled = false;
            tsiECMon.Enabled = false;

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
        }

        private bool FansHaveSameProfileCount()
        {
            for (int i = 0; i < Config.FanConfs.Length - 1; i++)
            {
                FanConf[] fanCfgs = Config.FanConfs;

                if (fanCfgs[i].FanCurveConfs.Length != fanCfgs[i + 1].FanCurveConfs.Length)
                {
                    return false;
                }
            }
            return true;
        }

        private void UpdateStatus(StatusCode status, int data = 0)
        {
            lblStatus.Invoke(() =>
            {
                if (AppStatus.Code == status)
                {
                    AppStatus.RepeatCount++;
                }
                else
                {
                    AppStatus.Code = status;
                    AppStatus.RepeatCount = 0;
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
                        lblStatus.Text = Strings.GetString("statSvcTimeout");
                        break;
                    case StatusCode.NoConfig:
                        persist = true;
                        lblStatus.Text = Strings.GetString("statNoConf");
                        break;
                    case StatusCode.ConfLoading:
                        lblStatus.Text = Strings.GetString("statConfLoading");
                        break;
                    case StatusCode.ConfApplySuccess:
                        lblStatus.Text = Strings.GetString("statConfApplied");
                        break;
                    case StatusCode.FullBlastToggleSuccess:
                        lblStatus.Text = Strings.GetString("statFBToggled");
                        break;
                    default:
                        persist = true;
                        AppStatus.RepeatCount = 0;
                        lblStatus.Text = "Ready";
                        break;
                }

                if (AppStatus.RepeatCount > 0)
                {
                    lblStatus.Text += $" (x{AppStatus.RepeatCount + 1})";
                }

                tmrStatusReset.Stop();
                if (!persist)
                {
                    tmrStatusReset.Start();
                }
            });
        }

        private void SendServiceMessage(ServiceCommand command)
        {
            IPCClient.PushMessage(command);
            tmrSvcTimeout.Start();
        }
        #endregion  // Private methods
    }
}
