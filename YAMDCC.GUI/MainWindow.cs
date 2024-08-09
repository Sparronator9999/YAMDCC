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
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using YAMDCC.Config;
using YAMDCC.GUI.Dialogs;
using YAMDCC.IPC;

namespace YAMDCC.GUI
{
    internal partial class MainWindow : Form
    {
        #region Fields
        /// <summary>
        /// The path where program data is stored.
        /// </summary>
        private static readonly string DataPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData),
            "Sparronator9999", "YAMDCC");

        /// <summary>
        /// The YAMDCC config that is currently open for editing.
        /// </summary>
        private YAMDCC_Config Config;

        /// <summary>
        /// The client that connects to the YAMDCC Service
        /// </summary>
        private readonly NamedPipeClient<ServiceResponse, ServiceCommand> IPCClient =
            new NamedPipeClient<ServiceResponse, ServiceCommand>("YAMDCC-Server");

        private readonly NumericUpDown[] numUpTs = new NumericUpDown[6];
        private readonly NumericUpDown[] numDownTs = new NumericUpDown[6];
        private readonly NumericUpDown[] numFanSpds = new NumericUpDown[7];
        private readonly TrackBar[] tbFanSpds = new TrackBar[7];

        private readonly ToolTip ttMain = new ToolTip();
        #endregion

        public MainWindow()
        {
            InitializeComponent();

            // Set the window icon using the application icon.
            // Saves about 8-9 KB from not having to embed the same icon twice.
            Icon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);

            // set literally every tooltip
            tsiLoadConf.ToolTipText = Strings.GetString("ttLoadConf");
            tsiSaveConf.ToolTipText = Strings.GetString("ttSaveConf");
            tsiApply.ToolTipText = Strings.GetString("ttApply");
            tsiRevert.ToolTipText = Strings.GetString("ttRevert");
            tsiExit.ToolTipText = Strings.GetString("ttSelfExplan");
            tsiProfAdd.ToolTipText = Strings.GetString("ttProfAdd");
            tsiProfRename.ToolTipText = Strings.GetString("ttProfRename");
            tsiProfChangeDesc.ToolTipText = Strings.GetString("ttProfChangeDesc");
            tsiProfDel.ToolTipText = Strings.GetString("ttProfDel");
            tsiECMon.ToolTipText = Strings.GetString("ttECMon");
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
                    Enabled = false,
                    Margin = new Padding(2),
                    Tag = i,
                };
                numFanSpds[i].ValueChanged += numFanSpd_Changed;
                tblCurve.Controls.Add(numFanSpds[i], i + 1, 0);

                tbFanSpds[i] = new TrackBar()
                {
                    Dock = DockStyle.Fill,
                    Enabled = false,
                    Orientation = Orientation.Vertical,
                    Tag = i,
                    TickFrequency = 5,
                    TickStyle = TickStyle.Both,
                };
                tbFanSpds[i].ValueChanged += tbFanSpd_Scroll;
                tblCurve.Controls.Add(tbFanSpds[i], i + 1, 1);

                if (i != 0)
                {
                    numUpTs[i - 1] = new NumericUpDown()
                    {
                        Dock = DockStyle.Fill,
                        Enabled = false,
                        Height = (int)(23 * scale),
                        Margin = new Padding(2),
                        Tag = i - 1,
                    };
                    numUpTs[i - 1].ValueChanged += numUpT_Changed;
                    tblCurve.Controls.Add(numUpTs[i - 1], i + 1, 2);
                }
                else
                {
                    tblCurve.Controls.Add(new Label
                    {
                        Dock = DockStyle.Fill,
                        Margin = new Padding(4),
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
                        Enabled = false,
                        Height = (int)(23 * scale),
                        Margin = new Padding(2),
                        Tag = i,
                    };
                    numDownTs[i].ValueChanged += numDownT_Changed;
                    tblCurve.Controls.Add(numDownTs[i], i + 1, 3);
                }
                else
                {
                    tblCurve.Controls.Add(new Label
                    {
                        Dock = DockStyle.Fill,
                        Margin = new Padding(4),
                        Text = "Max",
                        TextAlign = ContentAlignment.MiddleCenter,
                    },
                    i + 1, 3);
                }
            }
        }

        #region Events
        private void MainWindow_Load(object sender, EventArgs e)
        {
            try
            {
                IPCClient.ServerMessage += IPC_MessageReceived;
                IPCClient.Start();
                AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format(Strings.GetString("svcErrorConnect"), ex),
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Application.Exit();
            }

            LoadConf(Path.Combine(DataPath, "CurrentConfig.xml"));
        }

        private void MainWindow_Closing(object sender, FormClosingEventArgs e)
        {
            // Disable Full Blast if it was enabled while the program was running:
            if (chkFullBlast.Checked)
            {
                ServiceCommand command = new ServiceCommand(Command.FullBlast, "0");
                IPCClient.PushMessage(command);
            }
        }

        private void OnProcessExit(object sender, EventArgs e)
        {
            // Close the connection to the YAMDCC
            // Service before exiting the program:
            IPCClient.Stop();
        }

        private void IPC_MessageReceived(NamedPipeConnection<ServiceResponse, ServiceCommand> connection, ServiceResponse message)
        {
            string[] args = message.Value.Split(' ');
            if (args.Length == 1)
            {
                switch (message.Response)
                {
                    case Response.Temp:
                        if (int.TryParse(args[0], out int value))
                        {
                            UpdateFanMon(value, 0);
                        }
                        break;
                    case Response.FanSpeed:
                        if (int.TryParse(args[0], out value))
                        {
                            UpdateFanMon(value, 1);
                        }
                        break;
                    case Response.FanRPM:
                        if (int.TryParse(args[0], out value))
                        {
                            UpdateFanMon(value, 2);
                        }
                        break;
                }
            }
        }

        #region Tool strip menu items

        #region File
        private void tsiLoadConf_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog()
            {
                AddExtension = true,
                CheckFileExists = true,
                Filter = "YAMDCC config files|*.xml",
                Title = "Load config",
            };

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                LoadConf(ofd.FileName);
            }
        }

        private void tsiSaveConf_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog()
            {
                AddExtension = true,
                Filter = "YAMDCC config files|*.xml",
                Title = "Save config",
            };

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                Config.Save(sfd.FileName);
            }
        }

        private void tsiApply_Click(object sender, EventArgs e)
        {
            ApplyConf();
        }

        private void tsiRevert_Click(object sender, EventArgs e)
        {

        }

        private void tsiExit_Click(object sender, EventArgs e) =>
            Close();
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

            TextInputDialog dlg = new TextInputDialog(
                Strings.GetString("dlgProfRename"),
                "Change Profile Name", curveCfg.Name);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                curveCfg.Name = dlg.Result;
                cboProfSel.Items[cboProfSel.SelectedIndex] = dlg.Result;
            }
        }

        private void tsiProfChangeDesc_Click(object sender, EventArgs e)
        {
            FanCurveConf curveCfg = Config.FanConfs[cboFanSel.SelectedIndex]
                .FanCurveConfs[cboProfSel.SelectedIndex];
            TextInputDialog dlg = new TextInputDialog(
                Strings.GetString("dlgProfChangeDesc"),
                "Change Profile Description", curveCfg.Desc, true);
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                curveCfg.Desc = dlg.Result;
                ttMain.SetToolTip(cboProfSel, dlg.Result);
            }
        }

        private void tsiProfDel_Click(object sender, EventArgs e)
        {
            DeleteFanProfile();
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
                Utils.StopService("yamdccsvc");
            }
        }

        private void tsiUninstall_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(Strings.GetString("dlgSvcUninstall"), "Uninstall Service",
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                bool delData = MessageBox.Show(
                    Strings.GetString("dlgSvcDelData", DataPath),
                    "Delete configuration data?",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning,
                    MessageBoxDefaultButton.Button2) == DialogResult.Yes;

                if (delData)
                {
                    Directory.Delete(DataPath, true);
                }

                IPCClient.Stop();
                Close();
                Utils.UninstallService("yamdccsvc");
            }
        }
        #endregion

        #region Help
        private void tsiAbout_Click(object sender, EventArgs e) =>
            MessageBox.Show(Strings.GetString("dlgAbout"), "About",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

        private void tsiSrc_Click(object sender, EventArgs e) =>
            // TODO: add GitHub project link
            Process.Start("https://youtu.be/dQw4w9WgXcQ");
        #endregion

        #endregion

        private void cboFanSel_IndexChanged(object sender, EventArgs e)
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

        private void cboProfSel_IndexChanged(object sender, EventArgs e)
        {
            FanConf config = Config.FanConfs[cboFanSel.SelectedIndex];
            FanCurveConf curveConfig = config.FanCurveConfs[cboProfSel.SelectedIndex];

            config.CurveSel = cboProfSel.SelectedIndex;
            ttMain.SetToolTip(cboProfSel, config.FanCurveConfs[config.CurveSel].Desc);

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
            btnApply.Enabled = true;
            btnProfDel.Enabled = curveConfig.Name != "Default";
        }

        private void btnProfAdd_Click(object sender, EventArgs e)
        {
            AddFanProfile();
        }

        private void btnProfDel_Click(object sender, EventArgs e)
        {
            DeleteFanProfile();
        }

        private void numFanSpd_Changed(object sender, EventArgs e)
        {
            NumericUpDown nud = (NumericUpDown)sender;
            int i = (int)nud.Tag;
            tbFanSpds[i].Value = (int)numFanSpds[i].Value;

            Config.FanConfs[cboFanSel.SelectedIndex]
                .FanCurveConfs[cboProfSel.SelectedIndex]
                .TempThresholds[i].FanSpeed = (byte)numFanSpds[i].Value;
        }

        private void tbFanSpd_Scroll(object sender, EventArgs e)
        {
            TrackBar tb = (TrackBar)sender;
            int i = (int)tb.Tag;
            numFanSpds[i].Value = tbFanSpds[i].Value;

            Config.FanConfs[cboFanSel.SelectedIndex]
                .FanCurveConfs[cboProfSel.SelectedIndex]
                .TempThresholds[i].FanSpeed = (byte)numFanSpds[i].Value;
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
        }

        private void numDownT_Changed(object sender, EventArgs e)
        {
            NumericUpDown nud = (NumericUpDown)sender;
            int i = (int)nud.Tag;

            Config.FanConfs[cboFanSel.SelectedIndex]
                .FanCurveConfs[cboProfSel.SelectedIndex]
                .TempThresholds[i + 1].DownThreshold = (byte)numDownTs[i].Value;
        }

        private void chkFullBlast_Toggled(object sender, EventArgs e)
        {
            ServiceCommand command = new ServiceCommand(Command.FullBlast, chkFullBlast.Checked ? "1" : "0");
            IPCClient.PushMessage(command);
        }

        private void numChargeLim_Changed(object sender, EventArgs e)
        {
            Config.ChargeLimitConf.CurVal = (byte)numChgLim.Value;
        }

        private void cboPerfMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            Config.PerfModeConf.ModeSel = cboPerfMode.SelectedIndex;
        }

        private void chkWinFnSwap_CheckedChanged(object sender, EventArgs e)
        {
            Config.KeySwapConf.Enabled = chkWinFnSwap.Checked;
        }

        private void btnRevert_Click(object sender, EventArgs e)
        {

        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            ApplyConf();
        }

        private void tmrPoll_Tick(object sender, EventArgs e)
        {
            PollEC();
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

        private void LoadConf(string configPath)
        {
            lblStatus.Text = "Loading config, please wait...";

            try
            {
                Config = YAMDCC_Config.Load(configPath);
            }
            catch
            {
                lblStatus.Text = "Please load a config to start";
                return;
            }
            tsiSaveConf.Enabled = true;

            if (Config.FullBlastConf is null)
            {
                ttMain.SetToolTip(chkFullBlast, Strings.GetString("ttNotSupported"));
                chkFullBlast.Enabled = false;
            }
            else
            {
                ttMain.SetToolTip(chkFullBlast, Strings.GetString("ttFullBlast"));
                chkFullBlast.Enabled = true;
            }

            if (Config.ChargeLimitConf is null)
            {
                ttMain.SetToolTip(chkFullBlast, Strings.GetString("ttNotSupported"));
                numChgLim.Enabled = lblChgLim.Enabled = false;
            }
            else
            {
                ttMain.SetToolTip(numChgLim, Strings.GetString("ttChgLim"));
                ChargeLimitConf cfg = Config.ChargeLimitConf;
                numChgLim.Enabled = lblChgLim.Enabled = true;
                numChgLim.Value = cfg.CurVal;
                numChgLim.Maximum = Math.Abs(cfg.MaxVal - cfg.MinVal);
            }

            if (Config.PerfModeConf is null)
            {
                ttMain.SetToolTip(cboPerfMode, Strings.GetString("ttNotSupported"));
                cboPerfMode.Enabled = lblPerfMode.Enabled = false;
            }
            else
            {
                PerfModeConf cfg = Config.PerfModeConf;
                for (int i = 0; i < cfg.PerfModes.Length; i++)
                {
                    cboPerfMode.Items.Add(cfg.PerfModes[i].Name);
                }

                cboPerfMode.SelectedIndex = cfg.ModeSel;
                ttMain.SetToolTip(cboPerfMode, cfg.PerfModes[cfg.ModeSel].Desc);
                cboPerfMode.Enabled = lblPerfMode.Enabled = true;
            }


            if (Config.KeySwapConf is null)
            {
                ttMain.SetToolTip(chkWinFnSwap, Strings.GetString("ttNotSupported"));
                chkWinFnSwap.Enabled = lblWinFnSwap.Enabled = false;
            }
            else
            {
                chkWinFnSwap.Checked = Config.KeySwapConf.Enabled;
                ttMain.SetToolTip(chkWinFnSwap, Strings.GetString("ttKeySwap"));
                chkWinFnSwap.Enabled = lblWinFnSwap.Enabled = true;
            }

            cboFanSel.Items.Clear();
            for (int i = 0; i < Config.FanConfs.Length; i++)
            {
                cboFanSel.Items.Add(Config.FanConfs[i].Name);
            }

            btnProfAdd.Enabled = true;
            cboFanSel.Enabled = true;
            cboFanSel.SelectedIndex = 0;
            tsiECMon.Enabled = true;

            lblStatus.Text = "Ready";
        }

        private void ApplyConf()
        {
            // Save the updated config
            Config.Save(Path.Combine(DataPath, "CurrentConfig.xml"));

            // Tell the service to reload and apply the updated config
            ServiceCommand command = new ServiceCommand(Command.ApplyConfig, null);
            IPCClient.PushMessage(command);
        }

        private void PollEC()
        {
            IPCClient.PushMessage(new ServiceCommand(Command.GetTemp, cboFanSel.SelectedIndex.ToString()));
            IPCClient.PushMessage(new ServiceCommand(Command.GetFanSpeed, cboFanSel.SelectedIndex.ToString()));
            IPCClient.PushMessage(new ServiceCommand(Command.GetFanRPM, cboFanSel.SelectedIndex.ToString()));
        }

        private void AddFanProfile()
        {
            FanConf fanConfig = Config.FanConfs[cboFanSel.SelectedIndex];
            FanCurveConf oldCurveCfg = fanConfig.FanCurveConfs[cboProfSel.SelectedIndex];

            TextInputDialog dlg = new TextInputDialog(
                Strings.GetString("dlgProfAdd"),
                "New Profile", $"Copy of {oldCurveCfg.Name}");
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                // Create a copy of the currently selected fan profile:
                FanCurveConf newCurveCfg = oldCurveCfg.Copy();

                // Name it according to what the user specified
                newCurveCfg.Name = dlg.Result;
                newCurveCfg.Desc = $"Copy of {oldCurveCfg.Name}";

                // Add the new fan profile to the list
                List<FanCurveConf> curveCfgList = fanConfig.FanCurveConfs.ToList();
                curveCfgList.Add(newCurveCfg);
                fanConfig.FanCurveConfs = curveCfgList.ToArray();

                // Add the new fan profile to the list and select it:
                cboProfSel.Items.Add(dlg.Result);
                cboProfSel.SelectedIndex = cboProfSel.Items.Count - 1;
            }
        }

        private void DeleteFanProfile()
        {
            if (cboProfSel.Text != "Default" && MessageBox.Show(
                Strings.GetString("dlgProfDel", cboProfSel.Text),
                "Delete fan profile?", MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                FanConf fanConfig = Config.FanConfs[cboFanSel.SelectedIndex];

                // Remove the fan profile
                List<FanCurveConf> curveCfgList = fanConfig.FanCurveConfs.ToList();
                curveCfgList.RemoveAt(cboProfSel.SelectedIndex);
                fanConfig.FanCurveConfs = curveCfgList.ToArray();

                // Remove from the list client-side, and select a different fan profile
                int oldIndex = cboProfSel.SelectedIndex;
                cboProfSel.Items.RemoveAt(cboProfSel.SelectedIndex);
                cboProfSel.SelectedIndex = oldIndex == 1 ? 1 : oldIndex - 1;
            }
        }
        #endregion
    }
}
