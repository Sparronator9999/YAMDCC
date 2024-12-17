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
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using YAMDCC.Config;
using YAMDCC.ConfigEditor.Dialogs;
using YAMDCC.IPC;

namespace YAMDCC.ConfigEditor
{
    internal sealed partial class MainWindow : Form
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

        private readonly NumericUpDown[] numUpTs = new NumericUpDown[6];
        private readonly NumericUpDown[] numDownTs = new NumericUpDown[6];
        private readonly NumericUpDown[] numFanSpds = new NumericUpDown[7];
        private readonly TrackBar[] tbFanSpds = new TrackBar[7];

        private readonly ToolTip ttMain = new();

        private readonly Timer tmrPoll, tmrStatusReset, tmrSvcTimeout;
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
            tsiProfRename.ToolTipText = Strings.GetString("ttProfRename");
            tsiProfChangeDesc.ToolTipText = Strings.GetString("ttProfChangeDesc");
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

            float scale = CurrentAutoScaleDimensions.Height / 72;

            tblCurve.ColumnStyles.Clear();
            tblCurve.ColumnCount = numFanSpds.Length + 2;
            tblCurve.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            for (int i = 0; i < numFanSpds.Length; i++)
            {
                tblCurve.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F / numFanSpds.Length));
                numFanSpds[i] = new NumericUpDown()
                {
                    Dock = DockStyle.Fill,
                    Margin = new Padding(2),
                    Tag = i,
                };
                ttMain.SetToolTip(numFanSpds[i], Strings.GetString("ttFanSpd"));
                numFanSpds[i].ValueChanged += numFanSpd_Changed;
                tblCurve.Controls.Add(numFanSpds[i], i + 1, 0);

                tbFanSpds[i] = new TrackBar()
                {
                    Dock = DockStyle.Fill,
                    Margin = new Padding((int)(9 * scale), 0, (int)(9 * scale), 0),
                    Orientation = Orientation.Vertical,
                    Tag = i,
                    TickFrequency = 5,
                    TickStyle = TickStyle.Both,
                };
                ttMain.SetToolTip(tbFanSpds[i], Strings.GetString("ttFanSpdTB"));
                tbFanSpds[i].ValueChanged += tbFanSpd_Scroll;
                tblCurve.Controls.Add(tbFanSpds[i], i + 1, 1);

                if (i != 0)
                {
                    numUpTs[i - 1] = new NumericUpDown()
                    {
                        Dock = DockStyle.Fill,
                        Height = (int)(23 * scale),
                        Margin = new Padding(2),
                        Tag = i - 1,
                    };
                    ttMain.SetToolTip(numUpTs[i - 1], Strings.GetString("ttUpT"));
                    numUpTs[i - 1].ValueChanged += numUpT_Changed;
                    tblCurve.Controls.Add(numUpTs[i - 1], i + 1, 2);
                }
                else
                {
                    tblCurve.Controls.Add(new Label
                    {
                        Dock = DockStyle.Fill,
                        Margin = new Padding(2),
                        Padding = new Padding(0, 0, 0, 3),
                        Text = "Default",
                        TextAlign = ContentAlignment.MiddleCenter,
                    },
                    i + 1, 2);
                }

                if (i != numFanSpds.Length - 1)
                {
                    numDownTs[i] = new NumericUpDown()
                    {
                        Dock = DockStyle.Fill,
                        Height = (int)(23 * scale),
                        Margin = new Padding(2),
                        Tag = i,
                    };
                    ttMain.SetToolTip(numDownTs[i], Strings.GetString("ttDownT"));
                    numDownTs[i].ValueChanged += numDownT_Changed;
                    tblCurve.Controls.Add(numDownTs[i], i + 1, 3);
                }
                else
                {
                    tblCurve.Controls.Add(new Label
                    {
                        Dock = DockStyle.Fill,
                        Margin = new Padding(2),
                        Padding = new Padding(0, 0, 0, 3),
                        Text = "Max",
                        TextAlign = ContentAlignment.MiddleCenter,
                    },
                    i + 1, 3);
                }
            }

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

            DisableAll();
        }

        #region Events
        private void MainWindow_Load(object sender, EventArgs e)
        {
            try
            {
                IPCClient.ServerMessage += IPC_MessageReceived;
                IPCClient.Error += IPCClient_Error;
                IPCClient.Start();
                IPCClient.WaitForConnection();
                AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
            }
            catch (Exception ex)
            {
                Utils.ShowError(Strings.GetString("svcErrorConnect", ex));
                Application.Exit();
                return;
            }

            LoadConf(Paths.CurrentConfig);

            if (Config is not null)
            {
                if (Config.KeyLightConf is null)
                {
                    ttMain.SetToolTip(tbKeyLight, Strings.GetString("ttNotSupported"));
                }
                else
                {
                    SendServiceMessage(new ServiceCommand(Command.GetKeyLightBright, ""));
                }
            }

            if (File.Exists(Paths.ECToConfFail))
            {
                Utils.ShowError(Strings.GetString("dlgECtoConfError", Paths.Logs));
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
        }

        private void OnProcessExit(object sender, EventArgs e)
        {
            // Close the connection to the YAMDCC
            // Service before exiting the program:
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
                                tbKeyLight.Enabled = lblKeyLight.Enabled = true;
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
                Strings.GetString("dlgProfRename"),
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

                IPCClient.Stop();
                Close();

                if (!Utils.StopService("yamdccsvc"))
                {
                    Utils.ShowError(Strings.GetString("dlgSvcStopError"));
                }
            }
        }

        private void tsiUninstall_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(Strings.GetString("dlgSvcUninstall"), "Uninstall Service",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                bool delData = MessageBox.Show(
                    Strings.GetString("dlgSvcDelData", Paths.Data),
                    "Delete configuration data?",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2) == DialogResult.Yes;

                IPCClient.Stop();
                Close();

                // Apparently this fixes the YAMDCC service not uninstalling
                // when YAMDCC is launched by certain means
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
                        Utils.ShowError(Strings.GetString("dlgSvcUninstallError"));
                    }
                }
                else
                {
                    Utils.ShowError(Strings.GetString("dlgSvcStopError"));
                }
            }
        }
        #endregion

        #region Help
        private void tsiAbout_Click(object sender, EventArgs e) =>
            new VersionDialog().ShowDialog();

        private void tsiSrc_Click(object sender, EventArgs e) =>
            // TODO: add GitHub project link
            Process.Start("https://youtu.be/dQw4w9WgXcQ");
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
            FanConf config = Config.FanConfs[cboFanSel.SelectedIndex];
            FanCurveConf curveConfig = config.FanCurveConfs[cboProfSel.SelectedIndex];

            config.CurveSel = cboProfSel.SelectedIndex;
            ttMain.SetToolTip(cboProfSel, Strings.GetString(
                "ttProfSel", config.FanCurveConfs[config.CurveSel].Desc));

            int numTempThresholds = config.UpThresholdRegs.Length;

            // Fan curve
            for (int i = 0; i < numFanSpds.Length; i++)
            {
                if (i <= numTempThresholds)
                {
                    numFanSpds[i].Value = tbFanSpds[i].Value
                        = curveConfig.TempThresholds[i].FanSpeed;

                    numFanSpds[i].Enabled = tbFanSpds[i].Enabled = curveConfig.Name != "Default";
                }
            }

            // Temp thresholds
            for (int i = 0; i < numUpTs.Length; i++)
            {
                if (i <= numTempThresholds)
                {
                    TempThreshold t = curveConfig.TempThresholds[i + 1];
                    numUpTs[i].Value = t.UpThreshold;
                    numDownTs[i].Value = t.DownThreshold;

                    numUpTs[i].Enabled = numDownTs[i].Enabled = curveConfig.Name != "Default";
                }
                else
                {
                    numUpTs[i].Enabled = numDownTs[i].Enabled = false;
                }
            }
            btnApply.Enabled = tsiApply.Enabled = true;
            btnProfDel.Enabled = tsiProfDel.Enabled = curveConfig.Name != "Default";
        }

        private void btnProfAdd_Click(object sender, EventArgs e)
        {
            AddFanProfile();
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

        private void numChargeLim_Changed(object sender, EventArgs e)
        {
            if (Config is not null)
            {
                Config.ChargeLimitConf.CurVal = (byte)numChgLim.Value;
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

        #endregion

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
            tsiSaveConf.Enabled = true;
        }

        private void LoadConf(YAMDCC_Config config)
        {
            DisableAll();

            tsiSaveConf.Enabled = true;

            if (config.FullBlastConf is null)
            {
                ttMain.SetToolTip(chkFullBlast, Strings.GetString("ttNotSupported"));
            }
            else
            {
                ttMain.SetToolTip(chkFullBlast, Strings.GetString("ttFullBlast"));
                chkFullBlast.Enabled = true;
            }

            if (config.ChargeLimitConf is null)
            {
                ttMain.SetToolTip(chkFullBlast, Strings.GetString("ttNotSupported"));
            }
            else
            {
                ttMain.SetToolTip(numChgLim, Strings.GetString("ttChgLim"));
                ChargeLimitConf cfg = config.ChargeLimitConf;
                numChgLim.Enabled = lblChgLim.Enabled = true;
                numChgLim.Value = cfg.CurVal;
                numChgLim.Maximum = Math.Abs(cfg.MaxVal - cfg.MinVal);
            }

            cboPerfMode.Items.Clear();
            if (config.PerfModeConf is null)
            {
                ttMain.SetToolTip(cboPerfMode, Strings.GetString("ttNotSupported"));
            }
            else
            {
                PerfModeConf cfg = config.PerfModeConf;
                for (int i = 0; i < cfg.PerfModes.Length; i++)
                {
                    cboPerfMode.Items.Add(cfg.PerfModes[i].Name);
                }

                cboPerfMode.SelectedIndex = cfg.ModeSel;
                ttMain.SetToolTip(cboPerfMode, Strings.GetString(
                    "ttPerfMode", cfg.PerfModes[cfg.ModeSel].Desc));
                cboPerfMode.Enabled = lblPerfMode.Enabled = true;
            }

            if (config.KeySwapConf is null)
            {
                ttMain.SetToolTip(chkWinFnSwap, Strings.GetString("ttNotSupported"));
            }
            else
            {
                chkWinFnSwap.Checked = config.KeySwapConf.Enabled;
                ttMain.SetToolTip(chkWinFnSwap, Strings.GetString("ttKeySwap"));
                chkWinFnSwap.Enabled = lblWinFnSwap.Enabled = true;
            }

            cboFanSel.Items.Clear();
            for (int i = 0; i < config.FanConfs.Length; i++)
            {
                cboFanSel.Items.Add(config.FanConfs[i].Name);
            }

            btnProfAdd.Enabled = tsiProfAdd.Enabled = true;
            tsiProfRename.Enabled = tsiProfChangeDesc.Enabled = true;
            tsiECtoConf.Enabled = true;
            cboFanSel.Enabled = true;
            cboFanSel.SelectedIndex = 0;
            tsiECMon.Enabled = true;

            UpdateStatus(StatusCode.None);
        }

        private void ApplyConf()
        {
            // Save the updated config
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
            FanConf fanConfig = Config.FanConfs[cboFanSel.SelectedIndex];
            FanCurveConf oldCurveCfg = fanConfig.FanCurveConfs[cboProfSel.SelectedIndex];

            TextInputDialog dlg = new(
                Strings.GetString("dlgProfAdd"),
                "New Profile", $"Copy of {oldCurveCfg.Name}");
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                // Create a copy of the currently selected fan profile:
                FanCurveConf newCurveCfg = oldCurveCfg.Copy();

                // Name it according to what the user specified
                newCurveCfg.Name = dlg.Result;
                newCurveCfg.Desc = $"Copy of {oldCurveCfg.Name}";

                // Add the new fan profile to the config's list
                List<FanCurveConf> curveCfgList = [.. fanConfig.FanCurveConfs, newCurveCfg];
                fanConfig.FanCurveConfs = [.. curveCfgList];

                // Add the new fan profile to the UI's profile list and select it:
                cboProfSel.Items.Add(dlg.Result);
                cboProfSel.SelectedIndex = cboProfSel.Items.Count - 1;

                btnRevert.Enabled = tsiRevert.Enabled = true;
            }
        }

        private void DelFanProfile()
        {
            if (cboProfSel.Text != "Default" && MessageBox.Show(
                Strings.GetString("dlgProfDel", cboProfSel.Text),
                "Delete fan profile?", MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                FanConf fanConfig = Config.FanConfs[cboFanSel.SelectedIndex];

                // Remove the fan profile from the config's list
                List<FanCurveConf> curveCfgList = [.. fanConfig.FanCurveConfs];
                curveCfgList.RemoveAt(cboProfSel.SelectedIndex);
                fanConfig.FanCurveConfs = [.. curveCfgList];

                // Remove from the list client-side, and select a different fan profile
                int oldIndex = cboProfSel.SelectedIndex;
                cboProfSel.Items.RemoveAt(cboProfSel.SelectedIndex);
                cboProfSel.SelectedIndex = oldIndex == 1 ? 1 : oldIndex - 1;

                btnRevert.Enabled = tsiRevert.Enabled = true;
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
            FanConf config = Config.FanConfs[cboFanSel.SelectedIndex];

            cboProfSel.Items.Clear();
            foreach (FanCurveConf curve in config.FanCurveConfs)
            {
                cboProfSel.Items.Add(curve.Name);
            }

            for (int i = 0; i < numFanSpds.Length; i++)
            {
                if (config.FanCurveRegs.Length >= i)
                {
                    numFanSpds[i].Maximum = tbFanSpds[i].Maximum
                        = Math.Abs(config.MaxSpeed - config.MinSpeed);
                }
                else
                {
                    numFanSpds[i].Enabled = tbFanSpds[i].Enabled = false;
                }
            }

            cboProfSel.Enabled = true;
            cboProfSel.SelectedIndex = config.CurveSel;

            if (tsiECMon.Checked)
            {
                tmrPoll.Stop();
                PollEC();
                tmrPoll.Start();
            }
        }
        #endregion

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
            lblChgLim.Enabled = false;
            lblPerfMode.Enabled = false;
            lblWinFnSwap.Enabled = false;
            lblKeyLight.Enabled = false;
            numChgLim.Enabled = false;
            tbKeyLight.Enabled = false;

            tsiApply.Enabled = false;
            tsiRevert.Enabled = false;
            tsiProfAdd.Enabled = false;
            tsiProfChangeDesc.Enabled = false;
            tsiProfRename.Enabled = false;
            tsiProfDel.Enabled = false;
            tsiECtoConf.Enabled = false;
            tsiECMon.Enabled = false;

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
                        lblStatus.Text = Strings.GetString("statSvcError", (Command)data);
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
    }
}
