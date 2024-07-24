// This file is part of MSI Fan Control.
// Copyright © Sparronator9999 2023-2024.
//
// MSI Fan Control is free software: you can redistribute it and/or modify it
// under the terms of the GNU General Public License as published by the Free
// Software Foundation, either version 3 of the License, or (at your option)
// any later version.
//
// MSI Fan Control is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for
// more details.
//
// You should have received a copy of the GNU General Public License along with
// MSI Fan Control. If not, see <https://www.gnu.org/licenses/>.

namespace MSIFanControl.GUI
{
    partial class MainWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.tsiFile = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiLoadConf = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiSaveConf = new System.Windows.Forms.ToolStripMenuItem();
            this.sep1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsiApply = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiRevert = new System.Windows.Forms.ToolStripMenuItem();
            this.sep2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsiExit = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiOptions = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiProfAdd = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiProfEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiProfRename = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiProfChangeDesc = new System.Windows.Forms.ToolStripMenuItem();
            this.sep3 = new System.Windows.Forms.ToolStripSeparator();
            this.tsiProfDel = new System.Windows.Forms.ToolStripMenuItem();
            this.sep4 = new System.Windows.Forms.ToolStripSeparator();
            this.tsiECMon = new System.Windows.Forms.ToolStripMenuItem();
            this.sep5 = new System.Windows.Forms.ToolStripSeparator();
            this.tsiStopSvc = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiUninstall = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiSource = new System.Windows.Forms.ToolStripMenuItem();
            this.tblMain = new System.Windows.Forms.TableLayoutPanel();
            this.flwStats = new System.Windows.Forms.FlowLayoutPanel();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblFanSpd = new System.Windows.Forms.Label();
            this.lblFanRPM = new System.Windows.Forms.Label();
            this.lblTemp = new System.Windows.Forms.Label();
            this.tblFCBottom = new System.Windows.Forms.TableLayoutPanel();
            this.chkFullBlast = new System.Windows.Forms.CheckBox();
            this.btnApply = new System.Windows.Forms.Button();
            this.btnRevert = new System.Windows.Forms.Button();
            this.tmrPoll = new System.Windows.Forms.Timer(this.components);
            this.tcMain = new System.Windows.Forms.TabControl();
            this.tabFanControl = new System.Windows.Forms.TabPage();
            this.tblFanControl = new System.Windows.Forms.TableLayoutPanel();
            this.flwFanSelect = new System.Windows.Forms.FlowLayoutPanel();
            this.lblFanSel = new System.Windows.Forms.Label();
            this.cboFanSel = new System.Windows.Forms.ComboBox();
            this.lblProfSel = new System.Windows.Forms.Label();
            this.cboProfSel = new System.Windows.Forms.ComboBox();
            this.btnProfAdd = new System.Windows.Forms.Button();
            this.btnProfDel = new System.Windows.Forms.Button();
            this.tabOptions = new System.Windows.Forms.TabPage();
            this.numChgLim = new System.Windows.Forms.NumericUpDown();
            this.lblChgLim = new System.Windows.Forms.Label();
            this.tblCurve = new System.Windows.Forms.TableLayoutPanel();
            this.lblSpeed = new System.Windows.Forms.Label();
            this.lblUpT = new System.Windows.Forms.Label();
            this.lblDownT = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lblGearMode = new System.Windows.Forms.Label();
            this.cboGearMode = new System.Windows.Forms.ComboBox();
            this.lblWinFnSwap = new System.Windows.Forms.Label();
            this.chkWinFnSwap = new System.Windows.Forms.CheckBox();
            this.menuStrip.SuspendLayout();
            this.tblMain.SuspendLayout();
            this.flwStats.SuspendLayout();
            this.tblFCBottom.SuspendLayout();
            this.tcMain.SuspendLayout();
            this.tabFanControl.SuspendLayout();
            this.tblFanControl.SuspendLayout();
            this.flwFanSelect.SuspendLayout();
            this.tabOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numChgLim)).BeginInit();
            this.tblCurve.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.BackColor = System.Drawing.Color.WhiteSmoke;
            this.menuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsiFile,
            this.tsiOptions,
            this.tsiHelp});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Padding = new System.Windows.Forms.Padding(5, 2, 0, 2);
            this.menuStrip.Size = new System.Drawing.Size(540, 24);
            this.menuStrip.TabIndex = 0;
            // 
            // tsiFile
            // 
            this.tsiFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsiLoadConf,
            this.tsiSaveConf,
            this.sep1,
            this.tsiApply,
            this.tsiRevert,
            this.sep2,
            this.tsiExit});
            this.tsiFile.Name = "tsiFile";
            this.tsiFile.Size = new System.Drawing.Size(37, 20);
            this.tsiFile.Text = "File";
            // 
            // tsiLoadConf
            // 
            this.tsiLoadConf.Name = "tsiLoadConf";
            this.tsiLoadConf.Size = new System.Drawing.Size(154, 22);
            this.tsiLoadConf.Text = "Load config";
            this.tsiLoadConf.Click += new System.EventHandler(this.tsiLoadConf_Click);
            // 
            // tsiSaveConf
            // 
            this.tsiSaveConf.Name = "tsiSaveConf";
            this.tsiSaveConf.Size = new System.Drawing.Size(154, 22);
            this.tsiSaveConf.Text = "Save config";
            this.tsiSaveConf.Click += new System.EventHandler(this.tsiSaveConf_Click);
            // 
            // sep1
            // 
            this.sep1.Name = "sep1";
            this.sep1.Size = new System.Drawing.Size(151, 6);
            // 
            // tsiApply
            // 
            this.tsiApply.Name = "tsiApply";
            this.tsiApply.Size = new System.Drawing.Size(154, 22);
            this.tsiApply.Text = "Apply changes";
            this.tsiApply.Click += new System.EventHandler(this.tsiApply_Click);
            // 
            // tsiRevert
            // 
            this.tsiRevert.Name = "tsiRevert";
            this.tsiRevert.Size = new System.Drawing.Size(154, 22);
            this.tsiRevert.Text = "Revert changes";
            this.tsiRevert.Click += new System.EventHandler(this.tsiRevert_Click);
            // 
            // sep2
            // 
            this.sep2.Name = "sep2";
            this.sep2.Size = new System.Drawing.Size(151, 6);
            // 
            // tsiExit
            // 
            this.tsiExit.Name = "tsiExit";
            this.tsiExit.Size = new System.Drawing.Size(154, 22);
            this.tsiExit.Text = "Exit";
            this.tsiExit.Click += new System.EventHandler(this.tsiExit_Click);
            // 
            // tsiOptions
            // 
            this.tsiOptions.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsiProfAdd,
            this.tsiProfEdit,
            this.sep4,
            this.tsiECMon,
            this.sep5,
            this.tsiStopSvc,
            this.tsiUninstall});
            this.tsiOptions.Name = "tsiOptions";
            this.tsiOptions.Size = new System.Drawing.Size(61, 20);
            this.tsiOptions.Text = "Options";
            // 
            // tsiProfAdd
            // 
            this.tsiProfAdd.Name = "tsiProfAdd";
            this.tsiProfAdd.Size = new System.Drawing.Size(204, 22);
            this.tsiProfAdd.Text = "New fan profile...";
            this.tsiProfAdd.Click += new System.EventHandler(this.tsiProfAdd_Click);
            // 
            // tsiProfEdit
            // 
            this.tsiProfEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsiProfRename,
            this.tsiProfChangeDesc,
            this.sep3,
            this.tsiProfDel});
            this.tsiProfEdit.Name = "tsiProfEdit";
            this.tsiProfEdit.Size = new System.Drawing.Size(204, 22);
            this.tsiProfEdit.Text = "Edit current fan profile";
            // 
            // tsiProfRename
            // 
            this.tsiProfRename.Name = "tsiProfRename";
            this.tsiProfRename.Size = new System.Drawing.Size(178, 22);
            this.tsiProfRename.Text = "Change Name";
            this.tsiProfRename.Click += new System.EventHandler(this.tsiProfRename_Click);
            // 
            // tsiProfChangeDesc
            // 
            this.tsiProfChangeDesc.Name = "tsiProfChangeDesc";
            this.tsiProfChangeDesc.Size = new System.Drawing.Size(178, 22);
            this.tsiProfChangeDesc.Text = "Change Description";
            this.tsiProfChangeDesc.Click += new System.EventHandler(this.tsiProfChangeDesc_Click);
            // 
            // sep3
            // 
            this.sep3.Name = "sep3";
            this.sep3.Size = new System.Drawing.Size(175, 6);
            // 
            // tsiProfDel
            // 
            this.tsiProfDel.Name = "tsiProfDel";
            this.tsiProfDel.Size = new System.Drawing.Size(178, 22);
            this.tsiProfDel.Text = "Delete";
            this.tsiProfDel.Click += new System.EventHandler(this.tsiProfDel_Click);
            // 
            // sep4
            // 
            this.sep4.Name = "sep4";
            this.sep4.Size = new System.Drawing.Size(201, 6);
            // 
            // tsiECMon
            // 
            this.tsiECMon.CheckOnClick = true;
            this.tsiECMon.Name = "tsiECMon";
            this.tsiECMon.Size = new System.Drawing.Size(204, 22);
            this.tsiECMon.Text = "Enable EC monitoring";
            this.tsiECMon.Click += new System.EventHandler(this.tsiECMon_Click);
            // 
            // sep5
            // 
            this.sep5.Name = "sep5";
            this.sep5.Size = new System.Drawing.Size(201, 6);
            // 
            // tsiStopSvc
            // 
            this.tsiStopSvc.Name = "tsiStopSvc";
            this.tsiStopSvc.Size = new System.Drawing.Size(204, 22);
            this.tsiStopSvc.Text = "Stop service and exit";
            this.tsiStopSvc.Click += new System.EventHandler(this.tsiStopSvc_Click);
            // 
            // tsiUninstall
            // 
            this.tsiUninstall.Name = "tsiUninstall";
            this.tsiUninstall.Size = new System.Drawing.Size(204, 22);
            this.tsiUninstall.Text = "Uninstall service and exit";
            this.tsiUninstall.Click += new System.EventHandler(this.tsiUninstall_Click);
            // 
            // tsiHelp
            // 
            this.tsiHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsiAbout,
            this.tsiSource});
            this.tsiHelp.Name = "tsiHelp";
            this.tsiHelp.Size = new System.Drawing.Size(44, 20);
            this.tsiHelp.Text = "Help";
            // 
            // tsiAbout
            // 
            this.tsiAbout.Name = "tsiAbout";
            this.tsiAbout.Size = new System.Drawing.Size(141, 22);
            this.tsiAbout.Text = "About";
            this.tsiAbout.Click += new System.EventHandler(this.tsiAbout_Click);
            // 
            // tsiSource
            // 
            this.tsiSource.Name = "tsiSource";
            this.tsiSource.Size = new System.Drawing.Size(141, 22);
            this.tsiSource.Text = "Source Code";
            this.tsiSource.Click += new System.EventHandler(this.tsiSrc_Click);
            // 
            // tblMain
            // 
            this.tblMain.ColumnCount = 1;
            this.tblMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblMain.Controls.Add(this.tcMain, 0, 0);
            this.tblMain.Controls.Add(this.flwStats, 0, 1);
            this.tblMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblMain.Location = new System.Drawing.Point(0, 24);
            this.tblMain.Margin = new System.Windows.Forms.Padding(0);
            this.tblMain.Name = "tblMain";
            this.tblMain.RowCount = 2;
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblMain.Size = new System.Drawing.Size(540, 416);
            this.tblMain.TabIndex = 1;
            // 
            // flwStats
            // 
            this.flwStats.AutoSize = true;
            this.flwStats.Controls.Add(this.lblStatus);
            this.flwStats.Controls.Add(this.lblFanSpd);
            this.flwStats.Controls.Add(this.lblFanRPM);
            this.flwStats.Controls.Add(this.lblTemp);
            this.flwStats.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flwStats.Location = new System.Drawing.Point(0, 398);
            this.flwStats.Margin = new System.Windows.Forms.Padding(0, 0, 0, 3);
            this.flwStats.Name = "flwStats";
            this.flwStats.Size = new System.Drawing.Size(540, 15);
            this.flwStats.TabIndex = 2;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(3, 0);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(39, 15);
            this.lblStatus.TabIndex = 0;
            this.lblStatus.Text = "Ready";
            // 
            // lblFanSpd
            // 
            this.lblFanSpd.AutoSize = true;
            this.lblFanSpd.Location = new System.Drawing.Point(48, 0);
            this.lblFanSpd.Name = "lblFanSpd";
            this.lblFanSpd.Size = new System.Drawing.Size(86, 15);
            this.lblFanSpd.TabIndex = 1;
            this.lblFanSpd.Text = "Fan speed: --%";
            this.lblFanSpd.Visible = false;
            // 
            // lblFanRPM
            // 
            this.lblFanRPM.AutoSize = true;
            this.lblFanRPM.Location = new System.Drawing.Point(140, 0);
            this.lblFanRPM.Name = "lblFanRPM";
            this.lblFanRPM.Size = new System.Drawing.Size(58, 15);
            this.lblFanRPM.TabIndex = 2;
            this.lblFanRPM.Text = "RPM: ----";
            this.lblFanRPM.Visible = false;
            // 
            // lblTemp
            // 
            this.lblTemp.AutoSize = true;
            this.lblTemp.Location = new System.Drawing.Point(204, 0);
            this.lblTemp.Name = "lblTemp";
            this.lblTemp.Size = new System.Drawing.Size(65, 15);
            this.lblTemp.TabIndex = 3;
            this.lblTemp.Text = "Temp: --°C";
            this.lblTemp.Visible = false;
            // 
            // tblFCBottom
            // 
            this.tblFCBottom.AutoSize = true;
            this.tblFCBottom.ColumnCount = 3;
            this.tblFCBottom.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblFCBottom.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblFCBottom.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblFCBottom.Controls.Add(this.chkFullBlast, 0, 0);
            this.tblFCBottom.Controls.Add(this.btnApply, 2, 0);
            this.tblFCBottom.Controls.Add(this.btnRevert, 1, 0);
            this.tblFCBottom.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblFCBottom.Location = new System.Drawing.Point(0, 334);
            this.tblFCBottom.Margin = new System.Windows.Forms.Padding(0);
            this.tblFCBottom.Name = "tblFCBottom";
            this.tblFCBottom.RowCount = 1;
            this.tblFCBottom.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblFCBottom.Size = new System.Drawing.Size(526, 30);
            this.tblFCBottom.TabIndex = 4;
            // 
            // chkFullBlast
            // 
            this.chkFullBlast.AutoSize = true;
            this.chkFullBlast.Enabled = false;
            this.chkFullBlast.Location = new System.Drawing.Point(6, 6);
            this.chkFullBlast.Margin = new System.Windows.Forms.Padding(6, 6, 6, 5);
            this.chkFullBlast.Name = "chkFullBlast";
            this.chkFullBlast.Size = new System.Drawing.Size(73, 19);
            this.chkFullBlast.TabIndex = 1;
            this.chkFullBlast.Text = "Full Blast";
            this.chkFullBlast.UseVisualStyleBackColor = true;
            this.chkFullBlast.CheckedChanged += new System.EventHandler(this.chkFullBlast_Toggled);
            // 
            // btnApply
            // 
            this.btnApply.Enabled = false;
            this.btnApply.Location = new System.Drawing.Point(448, 3);
            this.btnApply.Margin = new System.Windows.Forms.Padding(3, 3, 3, 2);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 25);
            this.btnApply.TabIndex = 4;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // btnRevert
            // 
            this.btnRevert.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnRevert.Enabled = false;
            this.btnRevert.Location = new System.Drawing.Point(367, 3);
            this.btnRevert.Margin = new System.Windows.Forms.Padding(3, 3, 3, 2);
            this.btnRevert.Name = "btnRevert";
            this.btnRevert.Size = new System.Drawing.Size(75, 25);
            this.btnRevert.TabIndex = 5;
            this.btnRevert.Text = "Revert";
            this.btnRevert.UseVisualStyleBackColor = true;
            this.btnRevert.Click += new System.EventHandler(this.btnRevert_Click);
            // 
            // tmrPoll
            // 
            this.tmrPoll.Interval = 1000;
            this.tmrPoll.Tick += new System.EventHandler(this.tmrPoll_Tick);
            // 
            // tcMain
            // 
            this.tcMain.Controls.Add(this.tabFanControl);
            this.tcMain.Controls.Add(this.tabOptions);
            this.tcMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tcMain.Location = new System.Drawing.Point(3, 3);
            this.tcMain.Name = "tcMain";
            this.tcMain.SelectedIndex = 0;
            this.tcMain.Size = new System.Drawing.Size(534, 392);
            this.tcMain.TabIndex = 6;
            // 
            // tabFanControl
            // 
            this.tabFanControl.BackColor = System.Drawing.Color.White;
            this.tabFanControl.Controls.Add(this.tblFanControl);
            this.tabFanControl.Location = new System.Drawing.Point(4, 24);
            this.tabFanControl.Name = "tabFanControl";
            this.tabFanControl.Size = new System.Drawing.Size(526, 364);
            this.tabFanControl.TabIndex = 0;
            this.tabFanControl.Text = "Fan Control";
            // 
            // tblFanControl
            // 
            this.tblFanControl.ColumnCount = 1;
            this.tblFanControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblFanControl.Controls.Add(this.tblCurve, 0, 1);
            this.tblFanControl.Controls.Add(this.flwFanSelect, 0, 0);
            this.tblFanControl.Controls.Add(this.tblFCBottom, 0, 2);
            this.tblFanControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblFanControl.Location = new System.Drawing.Point(0, 0);
            this.tblFanControl.Margin = new System.Windows.Forms.Padding(0);
            this.tblFanControl.Name = "tblFanControl";
            this.tblFanControl.RowCount = 3;
            this.tblFanControl.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblFanControl.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblFanControl.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblFanControl.Size = new System.Drawing.Size(526, 364);
            this.tblFanControl.TabIndex = 0;
            // 
            // flwFanSelect
            // 
            this.flwFanSelect.AutoSize = true;
            this.flwFanSelect.Controls.Add(this.lblFanSel);
            this.flwFanSelect.Controls.Add(this.cboFanSel);
            this.flwFanSelect.Controls.Add(this.lblProfSel);
            this.flwFanSelect.Controls.Add(this.cboProfSel);
            this.flwFanSelect.Controls.Add(this.btnProfAdd);
            this.flwFanSelect.Controls.Add(this.btnProfDel);
            this.flwFanSelect.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flwFanSelect.Location = new System.Drawing.Point(0, 0);
            this.flwFanSelect.Margin = new System.Windows.Forms.Padding(0);
            this.flwFanSelect.Name = "flwFanSelect";
            this.flwFanSelect.Size = new System.Drawing.Size(526, 29);
            this.flwFanSelect.TabIndex = 0;
            // 
            // lblFanSel
            // 
            this.lblFanSel.AutoSize = true;
            this.lblFanSel.Location = new System.Drawing.Point(3, 7);
            this.lblFanSel.Margin = new System.Windows.Forms.Padding(3, 7, 0, 3);
            this.lblFanSel.Name = "lblFanSel";
            this.lblFanSel.Size = new System.Drawing.Size(29, 15);
            this.lblFanSel.TabIndex = 0;
            this.lblFanSel.Text = "Fan:";
            // 
            // cboFanSel
            // 
            this.cboFanSel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFanSel.Enabled = false;
            this.cboFanSel.FormattingEnabled = true;
            this.cboFanSel.Location = new System.Drawing.Point(35, 3);
            this.cboFanSel.Name = "cboFanSel";
            this.cboFanSel.Size = new System.Drawing.Size(121, 23);
            this.cboFanSel.TabIndex = 1;
            this.cboFanSel.SelectedIndexChanged += new System.EventHandler(this.cboFanSel_IndexChanged);
            // 
            // lblProfSel
            // 
            this.lblProfSel.AutoSize = true;
            this.lblProfSel.Location = new System.Drawing.Point(162, 7);
            this.lblProfSel.Margin = new System.Windows.Forms.Padding(3, 7, 0, 3);
            this.lblProfSel.Name = "lblProfSel";
            this.lblProfSel.Size = new System.Drawing.Size(44, 15);
            this.lblProfSel.TabIndex = 2;
            this.lblProfSel.Text = "Profile:";
            // 
            // cboProfSel
            // 
            this.cboProfSel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboProfSel.Enabled = false;
            this.cboProfSel.FormattingEnabled = true;
            this.cboProfSel.Location = new System.Drawing.Point(209, 3);
            this.cboProfSel.Name = "cboProfSel";
            this.cboProfSel.Size = new System.Drawing.Size(121, 23);
            this.cboProfSel.TabIndex = 3;
            this.cboProfSel.SelectedIndexChanged += new System.EventHandler(this.cboProfSel_IndexChanged);
            // 
            // btnProfAdd
            // 
            this.btnProfAdd.Enabled = false;
            this.btnProfAdd.Location = new System.Drawing.Point(336, 3);
            this.btnProfAdd.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.btnProfAdd.Name = "btnProfAdd";
            this.btnProfAdd.Size = new System.Drawing.Size(23, 23);
            this.btnProfAdd.TabIndex = 4;
            this.btnProfAdd.Text = "+";
            this.btnProfAdd.UseVisualStyleBackColor = true;
            this.btnProfAdd.Click += new System.EventHandler(this.btnProfAdd_Click);
            // 
            // btnProfDel
            // 
            this.btnProfDel.Enabled = false;
            this.btnProfDel.Location = new System.Drawing.Point(359, 3);
            this.btnProfDel.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.btnProfDel.Name = "btnProfDel";
            this.btnProfDel.Size = new System.Drawing.Size(23, 23);
            this.btnProfDel.TabIndex = 5;
            this.btnProfDel.Text = "-";
            this.btnProfDel.UseVisualStyleBackColor = true;
            this.btnProfDel.Click += new System.EventHandler(this.btnProfDel_Click);
            // 
            // tabOptions
            // 
            this.tabOptions.BackColor = System.Drawing.Color.White;
            this.tabOptions.Controls.Add(this.tableLayoutPanel1);
            this.tabOptions.Location = new System.Drawing.Point(4, 24);
            this.tabOptions.Name = "tabOptions";
            this.tabOptions.Size = new System.Drawing.Size(526, 364);
            this.tabOptions.TabIndex = 1;
            this.tabOptions.Text = "Extras";
            // 
            // numChgLim
            // 
            this.numChgLim.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.numChgLim.Enabled = false;
            this.numChgLim.Location = new System.Drawing.Point(136, 3);
            this.numChgLim.Name = "numChgLim";
            this.numChgLim.Size = new System.Drawing.Size(60, 23);
            this.numChgLim.TabIndex = 3;
            // 
            // lblChgLim
            // 
            this.lblChgLim.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblChgLim.AutoSize = true;
            this.lblChgLim.Enabled = false;
            this.lblChgLim.Location = new System.Drawing.Point(55, 5);
            this.lblChgLim.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.lblChgLim.Name = "lblChgLim";
            this.lblChgLim.Size = new System.Drawing.Size(75, 15);
            this.lblChgLim.TabIndex = 2;
            this.lblChgLim.Text = "Charge limit:";
            // 
            // tblCurve
            // 
            this.tblCurve.AutoSize = true;
            this.tblCurve.ColumnCount = 2;
            this.tblCurve.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblCurve.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblCurve.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tblCurve.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tblCurve.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tblCurve.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tblCurve.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tblCurve.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tblCurve.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tblCurve.Controls.Add(this.lblSpeed, 0, 0);
            this.tblCurve.Controls.Add(this.lblUpT, 0, 2);
            this.tblCurve.Controls.Add(this.lblDownT, 0, 3);
            this.tblCurve.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblCurve.Location = new System.Drawing.Point(0, 29);
            this.tblCurve.Margin = new System.Windows.Forms.Padding(0);
            this.tblCurve.Name = "tblCurve";
            this.tblCurve.RowCount = 4;
            this.tblCurve.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblCurve.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblCurve.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblCurve.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblCurve.Size = new System.Drawing.Size(526, 305);
            this.tblCurve.TabIndex = 6;
            // 
            // lblSpeed
            // 
            this.lblSpeed.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblSpeed.AutoSize = true;
            this.lblSpeed.Location = new System.Drawing.Point(5, 0);
            this.lblSpeed.Name = "lblSpeed";
            this.lblSpeed.Size = new System.Drawing.Size(60, 15);
            this.lblSpeed.TabIndex = 0;
            this.lblSpeed.Text = "Speed (%)";
            // 
            // lblUpT
            // 
            this.lblUpT.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblUpT.AutoSize = true;
            this.lblUpT.Location = new System.Drawing.Point(19, 275);
            this.lblUpT.Name = "lblUpT";
            this.lblUpT.Size = new System.Drawing.Size(46, 15);
            this.lblUpT.TabIndex = 1;
            this.lblUpT.Text = "Up (°C)";
            // 
            // lblDownT
            // 
            this.lblDownT.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblDownT.AutoSize = true;
            this.lblDownT.Location = new System.Drawing.Point(3, 290);
            this.lblDownT.Name = "lblDownT";
            this.lblDownT.Size = new System.Drawing.Size(62, 15);
            this.lblDownT.TabIndex = 2;
            this.lblDownT.Text = "Down (°C)";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.lblChgLim, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.numChgLim, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.lblGearMode, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.cboGearMode, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.lblWinFnSwap, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.chkWinFnSwap, 1, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(526, 364);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // lblGearMode
            // 
            this.lblGearMode.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblGearMode.AutoSize = true;
            this.lblGearMode.Enabled = false;
            this.lblGearMode.Location = new System.Drawing.Point(18, 35);
            this.lblGearMode.Name = "lblGearMode";
            this.lblGearMode.Size = new System.Drawing.Size(112, 15);
            this.lblGearMode.TabIndex = 4;
            this.lblGearMode.Text = "Performance mode:";
            // 
            // cboGearMode
            // 
            this.cboGearMode.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.cboGearMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboGearMode.Enabled = false;
            this.cboGearMode.FormattingEnabled = true;
            this.cboGearMode.Items.AddRange(new object[] {
            "Maximum battery life",
            "Balanced",
            "High performance",
            "Turbo performance"});
            this.cboGearMode.Location = new System.Drawing.Point(136, 32);
            this.cboGearMode.Name = "cboGearMode";
            this.cboGearMode.Size = new System.Drawing.Size(121, 23);
            this.cboGearMode.TabIndex = 5;
            // 
            // lblWinFnSwap
            // 
            this.lblWinFnSwap.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblWinFnSwap.AutoSize = true;
            this.lblWinFnSwap.Enabled = false;
            this.lblWinFnSwap.Location = new System.Drawing.Point(3, 59);
            this.lblWinFnSwap.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.lblWinFnSwap.Name = "lblWinFnSwap";
            this.lblWinFnSwap.Size = new System.Drawing.Size(127, 15);
            this.lblWinFnSwap.TabIndex = 6;
            this.lblWinFnSwap.Text = "Swap Win and Fn keys:";
            // 
            // chkWinFnSwap
            // 
            this.chkWinFnSwap.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.chkWinFnSwap.AutoSize = true;
            this.chkWinFnSwap.Enabled = false;
            this.chkWinFnSwap.Location = new System.Drawing.Point(136, 59);
            this.chkWinFnSwap.Name = "chkWinFnSwap";
            this.chkWinFnSwap.Size = new System.Drawing.Size(68, 19);
            this.chkWinFnSwap.TabIndex = 7;
            this.chkWinFnSwap.Text = "Enabled";
            this.chkWinFnSwap.UseVisualStyleBackColor = true;
            // 
            // MainWindow
            // 
            this.AcceptButton = this.btnApply;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.btnRevert;
            this.ClientSize = new System.Drawing.Size(540, 440);
            this.Controls.Add(this.tblMain);
            this.Controls.Add(this.menuStrip);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = this.menuStrip;
            this.MaximizeBox = false;
            this.Name = "MainWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MSI Fan Control";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainWindow_Closing);
            this.Load += new System.EventHandler(this.MainWindow_Load);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.tblMain.ResumeLayout(false);
            this.tblMain.PerformLayout();
            this.flwStats.ResumeLayout(false);
            this.flwStats.PerformLayout();
            this.tblFCBottom.ResumeLayout(false);
            this.tblFCBottom.PerformLayout();
            this.tcMain.ResumeLayout(false);
            this.tabFanControl.ResumeLayout(false);
            this.tblFanControl.ResumeLayout(false);
            this.tblFanControl.PerformLayout();
            this.flwFanSelect.ResumeLayout(false);
            this.flwFanSelect.PerformLayout();
            this.tabOptions.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.numChgLim)).EndInit();
            this.tblCurve.ResumeLayout(false);
            this.tblCurve.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStripMenuItem tsiECMon;
        private System.Windows.Forms.CheckBox chkFullBlast;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Timer tmrPoll;
        private System.Windows.Forms.Label lblFanSpd;
        private System.Windows.Forms.Label lblFanRPM;
        private System.Windows.Forms.Label lblTemp;
        private System.Windows.Forms.ToolStripMenuItem tsiLoadConf;
        private System.Windows.Forms.ToolStripMenuItem tsiSaveConf;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.ToolStripMenuItem tsiExit;
        private System.Windows.Forms.ToolStripMenuItem tsiApply;
        private System.Windows.Forms.Button btnRevert;
        private System.Windows.Forms.ToolStripMenuItem tsiRevert;
        private System.Windows.Forms.ToolStripMenuItem tsiAbout;
        private System.Windows.Forms.ToolStripMenuItem tsiSource;
        private System.Windows.Forms.ToolStripSeparator sep5;
        private System.Windows.Forms.ToolStripMenuItem tsiUninstall;
        private System.Windows.Forms.ToolStripMenuItem tsiProfAdd;
        private System.Windows.Forms.ToolStripMenuItem tsiProfEdit;
        private System.Windows.Forms.ToolStripMenuItem tsiProfRename;
        private System.Windows.Forms.ToolStripMenuItem tsiProfChangeDesc;
        private System.Windows.Forms.ToolStripSeparator sep3;
        private System.Windows.Forms.ToolStripMenuItem tsiProfDel;
        private System.Windows.Forms.ToolStripSeparator sep4;
        private System.Windows.Forms.ToolStripMenuItem tsiStopSvc;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem tsiFile;
        private System.Windows.Forms.ToolStripSeparator sep1;
        private System.Windows.Forms.ToolStripSeparator sep2;
        private System.Windows.Forms.ToolStripMenuItem tsiOptions;
        private System.Windows.Forms.ToolStripMenuItem tsiHelp;
        private System.Windows.Forms.TableLayoutPanel tblMain;
        private System.Windows.Forms.FlowLayoutPanel flwStats;
        private System.Windows.Forms.TableLayoutPanel tblFCBottom;
        private System.Windows.Forms.TabControl tcMain;
        private System.Windows.Forms.TabPage tabFanControl;
        private System.Windows.Forms.TableLayoutPanel tblFanControl;
        private System.Windows.Forms.TableLayoutPanel tblCurve;
        private System.Windows.Forms.Label lblSpeed;
        private System.Windows.Forms.Label lblUpT;
        private System.Windows.Forms.Label lblDownT;
        private System.Windows.Forms.FlowLayoutPanel flwFanSelect;
        private System.Windows.Forms.Label lblFanSel;
        private System.Windows.Forms.ComboBox cboFanSel;
        private System.Windows.Forms.Label lblProfSel;
        private System.Windows.Forms.ComboBox cboProfSel;
        private System.Windows.Forms.Button btnProfAdd;
        private System.Windows.Forms.Button btnProfDel;
        private System.Windows.Forms.TabPage tabOptions;
        private System.Windows.Forms.NumericUpDown numChgLim;
        private System.Windows.Forms.Label lblChgLim;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label lblGearMode;
        private System.Windows.Forms.ComboBox cboGearMode;
        private System.Windows.Forms.Label lblWinFnSwap;
        private System.Windows.Forms.CheckBox chkWinFnSwap;
    }
}
