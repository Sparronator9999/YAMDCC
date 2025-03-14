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

namespace YAMDCC.ConfigEditor
{
    partial class MainForm
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
            System.Windows.Forms.MenuStrip menuStrip;
            System.Windows.Forms.ToolStripSeparator sep1;
            System.Windows.Forms.ToolStripSeparator sep2;
            System.Windows.Forms.ToolStripSeparator sep3;
            System.Windows.Forms.ToolStripSeparator sep4;
            System.Windows.Forms.ToolStripSeparator sep5;
            System.Windows.Forms.ToolStripMenuItem tsiLogLevel;
            System.Windows.Forms.ToolStripMenuItem tsiCheckUpdate;
            System.Windows.Forms.TabControl tcMain;
            System.Windows.Forms.TabPage tabFanControl;
            System.Windows.Forms.Label lblProfPerfMode;
            System.Windows.Forms.Label lblProfSel;
            System.Windows.Forms.Label lblFanSel;
            System.Windows.Forms.TabPage tabExtra;
            System.Windows.Forms.TableLayoutPanel tblExtra;
            System.Windows.Forms.Label lblFanMode;
            System.Windows.Forms.Label lblChgLim;
            System.Windows.Forms.Label lblPerfMode;
            System.Windows.Forms.Label lblWinFnSwap;
            System.Windows.Forms.Label lblKeyLight;
            System.Windows.Forms.FlowLayoutPanel flwKeyLight;
            System.Windows.Forms.FlowLayoutPanel flwChgLim;
            System.Windows.Forms.TableLayoutPanel tblInfo;
            System.Windows.Forms.Label lblModel;
            System.Windows.Forms.Label lblManufacturer;
            System.Windows.Forms.Label lblAuthor;
            System.Windows.Forms.Label lblFirmVer;
            System.Windows.Forms.Label lblFirmDate;
            System.Windows.Forms.TableLayoutPanel tblApply;
            System.Windows.Forms.FlowLayoutPanel flwStats;
            this.tsiFile = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiLoadConf = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiSaveConf = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiApply = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiRevert = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiExit = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiOptions = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiProfAdd = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiProfEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiProfRen = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiProfChangeDesc = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiProfDel = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiECtoConf = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiECMon = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiAdvanced = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiLogDebug = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiLogInfo = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiLogWarn = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiLogError = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiLogFatal = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiLogNone = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiStopSvc = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiUninstall = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiSource = new System.Windows.Forms.ToolStripMenuItem();
            this.tblFanControl = new System.Windows.Forms.TableLayoutPanel();
            this.cboProfPerfMode = new System.Windows.Forms.ComboBox();
            this.btnProfDel = new System.Windows.Forms.Button();
            this.btnProfAdd = new System.Windows.Forms.Button();
            this.cboProfSel = new System.Windows.Forms.ComboBox();
            this.cboFanSel = new System.Windows.Forms.ComboBox();
            this.tblCurve = new System.Windows.Forms.TableLayoutPanel();
            this.cboFanMode = new System.Windows.Forms.ComboBox();
            this.cboPerfMode = new System.Windows.Forms.ComboBox();
            this.chkWinFnSwap = new System.Windows.Forms.CheckBox();
            this.lblKeyLightLow = new System.Windows.Forms.Label();
            this.tbKeyLight = new System.Windows.Forms.TrackBar();
            this.lblKeyLightHigh = new System.Windows.Forms.Label();
            this.chkChgLim = new System.Windows.Forms.CheckBox();
            this.numChgLim = new System.Windows.Forms.NumericUpDown();
            this.tabInfo = new System.Windows.Forms.TabPage();
            this.txtFirmDate = new System.Windows.Forms.TextBox();
            this.txtModel = new System.Windows.Forms.TextBox();
            this.txtManufacturer = new System.Windows.Forms.TextBox();
            this.txtAuthor = new System.Windows.Forms.TextBox();
            this.btnGetModel = new System.Windows.Forms.Button();
            this.txtFirmVer = new System.Windows.Forms.TextBox();
            this.chkFullBlast = new System.Windows.Forms.CheckBox();
            this.btnRevert = new System.Windows.Forms.Button();
            this.btnApply = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblFanSpd = new System.Windows.Forms.Label();
            this.lblFanRPM = new System.Windows.Forms.Label();
            this.lblTemp = new System.Windows.Forms.Label();
            this.tblMain = new System.Windows.Forms.TableLayoutPanel();
            menuStrip = new System.Windows.Forms.MenuStrip();
            sep1 = new System.Windows.Forms.ToolStripSeparator();
            sep2 = new System.Windows.Forms.ToolStripSeparator();
            sep3 = new System.Windows.Forms.ToolStripSeparator();
            sep4 = new System.Windows.Forms.ToolStripSeparator();
            sep5 = new System.Windows.Forms.ToolStripSeparator();
            tsiLogLevel = new System.Windows.Forms.ToolStripMenuItem();
            tsiCheckUpdate = new System.Windows.Forms.ToolStripMenuItem();
            tcMain = new System.Windows.Forms.TabControl();
            tabFanControl = new System.Windows.Forms.TabPage();
            lblProfPerfMode = new System.Windows.Forms.Label();
            lblProfSel = new System.Windows.Forms.Label();
            lblFanSel = new System.Windows.Forms.Label();
            tabExtra = new System.Windows.Forms.TabPage();
            tblExtra = new System.Windows.Forms.TableLayoutPanel();
            lblFanMode = new System.Windows.Forms.Label();
            lblChgLim = new System.Windows.Forms.Label();
            lblPerfMode = new System.Windows.Forms.Label();
            lblWinFnSwap = new System.Windows.Forms.Label();
            lblKeyLight = new System.Windows.Forms.Label();
            flwKeyLight = new System.Windows.Forms.FlowLayoutPanel();
            flwChgLim = new System.Windows.Forms.FlowLayoutPanel();
            tblInfo = new System.Windows.Forms.TableLayoutPanel();
            lblModel = new System.Windows.Forms.Label();
            lblManufacturer = new System.Windows.Forms.Label();
            lblAuthor = new System.Windows.Forms.Label();
            lblFirmVer = new System.Windows.Forms.Label();
            lblFirmDate = new System.Windows.Forms.Label();
            tblApply = new System.Windows.Forms.TableLayoutPanel();
            flwStats = new System.Windows.Forms.FlowLayoutPanel();
            menuStrip.SuspendLayout();
            tcMain.SuspendLayout();
            tabFanControl.SuspendLayout();
            this.tblFanControl.SuspendLayout();
            tabExtra.SuspendLayout();
            tblExtra.SuspendLayout();
            flwKeyLight.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbKeyLight)).BeginInit();
            flwChgLim.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numChgLim)).BeginInit();
            this.tabInfo.SuspendLayout();
            tblInfo.SuspendLayout();
            tblApply.SuspendLayout();
            flwStats.SuspendLayout();
            this.tblMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            menuStrip.BackColor = System.Drawing.Color.WhiteSmoke;
            menuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsiFile,
            this.tsiOptions,
            this.tsiHelp});
            menuStrip.Location = new System.Drawing.Point(0, 0);
            menuStrip.Name = "menuStrip";
            menuStrip.Padding = new System.Windows.Forms.Padding(6, 2, 0, 2);
            menuStrip.Size = new System.Drawing.Size(675, 28);
            menuStrip.TabIndex = 0;
            // 
            // tsiFile
            // 
            this.tsiFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsiLoadConf,
            this.tsiSaveConf,
            sep1,
            this.tsiApply,
            this.tsiRevert,
            sep2,
            this.tsiExit});
            this.tsiFile.Name = "tsiFile";
            this.tsiFile.Size = new System.Drawing.Size(46, 24);
            this.tsiFile.Text = "&File";
            // 
            // tsiLoadConf
            // 
            this.tsiLoadConf.Name = "tsiLoadConf";
            this.tsiLoadConf.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.tsiLoadConf.Size = new System.Drawing.Size(259, 26);
            this.tsiLoadConf.Text = "L&oad config";
            this.tsiLoadConf.Click += new System.EventHandler(this.tsiLoadConf_Click);
            // 
            // tsiSaveConf
            // 
            this.tsiSaveConf.Name = "tsiSaveConf";
            this.tsiSaveConf.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.S)));
            this.tsiSaveConf.Size = new System.Drawing.Size(259, 26);
            this.tsiSaveConf.Text = "&Save config";
            this.tsiSaveConf.Click += new System.EventHandler(this.tsiSaveConf_Click);
            // 
            // sep1
            // 
            sep1.Name = "sep1";
            sep1.Size = new System.Drawing.Size(256, 6);
            // 
            // tsiApply
            // 
            this.tsiApply.Name = "tsiApply";
            this.tsiApply.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.tsiApply.Size = new System.Drawing.Size(259, 26);
            this.tsiApply.Text = "&Apply changes";
            this.tsiApply.Click += new System.EventHandler(this.ApplyConf);
            // 
            // tsiRevert
            // 
            this.tsiRevert.Name = "tsiRevert";
            this.tsiRevert.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.tsiRevert.Size = new System.Drawing.Size(259, 26);
            this.tsiRevert.Text = "&Revert changes";
            this.tsiRevert.Click += new System.EventHandler(this.RevertConf);
            // 
            // sep2
            // 
            sep2.Name = "sep2";
            sep2.Size = new System.Drawing.Size(256, 6);
            // 
            // tsiExit
            // 
            this.tsiExit.Name = "tsiExit";
            this.tsiExit.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Q)));
            this.tsiExit.Size = new System.Drawing.Size(259, 26);
            this.tsiExit.Text = "E&xit";
            this.tsiExit.Click += new System.EventHandler(this.tsiExit_Click);
            // 
            // tsiOptions
            // 
            this.tsiOptions.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsiProfAdd,
            this.tsiProfEdit,
            this.tsiECtoConf,
            sep4,
            this.tsiECMon,
            this.tsiAdvanced,
            sep5,
            tsiLogLevel,
            this.tsiStopSvc,
            this.tsiUninstall});
            this.tsiOptions.Name = "tsiOptions";
            this.tsiOptions.Size = new System.Drawing.Size(75, 24);
            this.tsiOptions.Text = "&Options";
            // 
            // tsiProfAdd
            // 
            this.tsiProfAdd.Name = "tsiProfAdd";
            this.tsiProfAdd.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.tsiProfAdd.Size = new System.Drawing.Size(322, 26);
            this.tsiProfAdd.Text = "&New fan profile...";
            this.tsiProfAdd.Click += new System.EventHandler(this.ProfAdd);
            // 
            // tsiProfEdit
            // 
            this.tsiProfEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsiProfRen,
            this.tsiProfChangeDesc,
            sep3,
            this.tsiProfDel});
            this.tsiProfEdit.Name = "tsiProfEdit";
            this.tsiProfEdit.Size = new System.Drawing.Size(322, 26);
            this.tsiProfEdit.Text = "&Edit current fan profile";
            // 
            // tsiProfRen
            // 
            this.tsiProfRen.Name = "tsiProfRen";
            this.tsiProfRen.Size = new System.Drawing.Size(222, 26);
            this.tsiProfRen.Text = "Change Name";
            this.tsiProfRen.Click += new System.EventHandler(this.ProfRename);
            // 
            // tsiProfChangeDesc
            // 
            this.tsiProfChangeDesc.Name = "tsiProfChangeDesc";
            this.tsiProfChangeDesc.Size = new System.Drawing.Size(222, 26);
            this.tsiProfChangeDesc.Text = "Change Description";
            this.tsiProfChangeDesc.Click += new System.EventHandler(this.ProfChangeDesc);
            // 
            // sep3
            // 
            sep3.Name = "sep3";
            sep3.Size = new System.Drawing.Size(219, 6);
            // 
            // tsiProfDel
            // 
            this.tsiProfDel.Name = "tsiProfDel";
            this.tsiProfDel.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Delete)));
            this.tsiProfDel.Size = new System.Drawing.Size(222, 26);
            this.tsiProfDel.Text = "Delete";
            this.tsiProfDel.Click += new System.EventHandler(this.ProfDel);
            // 
            // tsiECtoConf
            // 
            this.tsiECtoConf.Name = "tsiECtoConf";
            this.tsiECtoConf.Size = new System.Drawing.Size(322, 26);
            this.tsiECtoConf.Text = "Get &default fan profile from EC...";
            this.tsiECtoConf.Click += new System.EventHandler(this.ECtoConf);
            // 
            // sep4
            // 
            sep4.Name = "sep4";
            sep4.Size = new System.Drawing.Size(319, 6);
            // 
            // tsiECMon
            // 
            this.tsiECMon.CheckOnClick = true;
            this.tsiECMon.Name = "tsiECMon";
            this.tsiECMon.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.M)));
            this.tsiECMon.Size = new System.Drawing.Size(322, 26);
            this.tsiECMon.Text = "Enable EC &monitoring";
            this.tsiECMon.Click += new System.EventHandler(this.ECMonToggle);
            // 
            // tsiAdvanced
            // 
            this.tsiAdvanced.Name = "tsiAdvanced";
            this.tsiAdvanced.Size = new System.Drawing.Size(322, 26);
            this.tsiAdvanced.Text = "Show advanced settings";
            this.tsiAdvanced.Click += new System.EventHandler(this.AdvancedToggle);
            // 
            // sep5
            // 
            sep5.Name = "sep5";
            sep5.Size = new System.Drawing.Size(319, 6);
            // 
            // tsiLogLevel
            // 
            tsiLogLevel.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsiLogDebug,
            this.tsiLogInfo,
            this.tsiLogWarn,
            this.tsiLogError,
            this.tsiLogFatal,
            this.tsiLogNone});
            tsiLogLevel.Name = "tsiLogLevel";
            tsiLogLevel.Size = new System.Drawing.Size(322, 26);
            tsiLogLevel.Text = "Service log level";
            // 
            // tsiLogDebug
            // 
            this.tsiLogDebug.Name = "tsiLogDebug";
            this.tsiLogDebug.Size = new System.Drawing.Size(151, 26);
            this.tsiLogDebug.Text = "Debug";
            this.tsiLogDebug.Click += new System.EventHandler(this.tsiLogDebug_Click);
            // 
            // tsiLogInfo
            // 
            this.tsiLogInfo.Name = "tsiLogInfo";
            this.tsiLogInfo.Size = new System.Drawing.Size(151, 26);
            this.tsiLogInfo.Text = "Info";
            this.tsiLogInfo.Click += new System.EventHandler(this.tsiLogInfo_Click);
            // 
            // tsiLogWarn
            // 
            this.tsiLogWarn.Name = "tsiLogWarn";
            this.tsiLogWarn.Size = new System.Drawing.Size(151, 26);
            this.tsiLogWarn.Text = "Warning";
            this.tsiLogWarn.Click += new System.EventHandler(this.tsiLogWarn_Click);
            // 
            // tsiLogError
            // 
            this.tsiLogError.Name = "tsiLogError";
            this.tsiLogError.Size = new System.Drawing.Size(151, 26);
            this.tsiLogError.Text = "Error";
            this.tsiLogError.Click += new System.EventHandler(this.tsiLogError_Click);
            // 
            // tsiLogFatal
            // 
            this.tsiLogFatal.Name = "tsiLogFatal";
            this.tsiLogFatal.Size = new System.Drawing.Size(151, 26);
            this.tsiLogFatal.Text = "Fatal";
            this.tsiLogFatal.Click += new System.EventHandler(this.tsiLogFatal_Click);
            // 
            // tsiLogNone
            // 
            this.tsiLogNone.Name = "tsiLogNone";
            this.tsiLogNone.Size = new System.Drawing.Size(151, 26);
            this.tsiLogNone.Text = "Disabled";
            this.tsiLogNone.Click += new System.EventHandler(this.tsiLogNone_Click);
            // 
            // tsiStopSvc
            // 
            this.tsiStopSvc.Name = "tsiStopSvc";
            this.tsiStopSvc.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.Q)));
            this.tsiStopSvc.Size = new System.Drawing.Size(322, 26);
            this.tsiStopSvc.Text = "&Stop service and exit";
            this.tsiStopSvc.Click += new System.EventHandler(this.tsiStopSvc_Click);
            // 
            // tsiUninstall
            // 
            this.tsiUninstall.Name = "tsiUninstall";
            this.tsiUninstall.Size = new System.Drawing.Size(322, 26);
            this.tsiUninstall.Text = "&Uninstall service and exit";
            this.tsiUninstall.Click += new System.EventHandler(this.tsiUninstall_Click);
            // 
            // tsiHelp
            // 
            this.tsiHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsiAbout,
            this.tsiSource,
            tsiCheckUpdate});
            this.tsiHelp.Name = "tsiHelp";
            this.tsiHelp.Size = new System.Drawing.Size(55, 24);
            this.tsiHelp.Text = "&Help";
            // 
            // tsiAbout
            // 
            this.tsiAbout.Name = "tsiAbout";
            this.tsiAbout.Size = new System.Drawing.Size(220, 26);
            this.tsiAbout.Text = "&About";
            this.tsiAbout.Click += new System.EventHandler(this.tsiAbout_Click);
            // 
            // tsiSource
            // 
            this.tsiSource.Name = "tsiSource";
            this.tsiSource.Size = new System.Drawing.Size(220, 26);
            this.tsiSource.Text = "Source &code";
            this.tsiSource.Click += new System.EventHandler(this.tsiSrc_Click);
            // 
            // tsiCheckUpdate
            // 
            tsiCheckUpdate.Name = "tsiCheckUpdate";
            tsiCheckUpdate.Size = new System.Drawing.Size(220, 26);
            tsiCheckUpdate.Text = "Check for updates...";
            tsiCheckUpdate.Click += new System.EventHandler(this.tsiCheckUpdate_Click);
            // 
            // tcMain
            // 
            tcMain.Controls.Add(tabFanControl);
            tcMain.Controls.Add(tabExtra);
            tcMain.Controls.Add(this.tabInfo);
            tcMain.Dock = System.Windows.Forms.DockStyle.Fill;
            tcMain.Location = new System.Drawing.Point(4, 4);
            tcMain.Margin = new System.Windows.Forms.Padding(4, 4, 4, 2);
            tcMain.Name = "tcMain";
            tcMain.SelectedIndex = 0;
            tcMain.Size = new System.Drawing.Size(667, 456);
            tcMain.TabIndex = 2;
            // 
            // tabFanControl
            // 
            tabFanControl.BackColor = System.Drawing.Color.White;
            tabFanControl.Controls.Add(this.tblFanControl);
            tabFanControl.Location = new System.Drawing.Point(4, 29);
            tabFanControl.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            tabFanControl.Name = "tabFanControl";
            tabFanControl.Size = new System.Drawing.Size(659, 423);
            tabFanControl.TabIndex = 0;
            tabFanControl.Text = "Fan control";
            // 
            // tblFanControl
            // 
            this.tblFanControl.ColumnCount = 9;
            this.tblFanControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblFanControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 27.5F));
            this.tblFanControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblFanControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 27.5F));
            this.tblFanControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblFanControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblFanControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblFanControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 45F));
            this.tblFanControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblFanControl.Controls.Add(this.cboProfPerfMode, 7, 0);
            this.tblFanControl.Controls.Add(lblProfPerfMode, 6, 0);
            this.tblFanControl.Controls.Add(this.btnProfDel, 5, 0);
            this.tblFanControl.Controls.Add(this.btnProfAdd, 4, 0);
            this.tblFanControl.Controls.Add(this.cboProfSel, 3, 0);
            this.tblFanControl.Controls.Add(lblProfSel, 2, 0);
            this.tblFanControl.Controls.Add(this.cboFanSel, 1, 0);
            this.tblFanControl.Controls.Add(lblFanSel, 0, 0);
            this.tblFanControl.Controls.Add(this.tblCurve, 0, 1);
            this.tblFanControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblFanControl.Location = new System.Drawing.Point(0, 0);
            this.tblFanControl.Margin = new System.Windows.Forms.Padding(0);
            this.tblFanControl.Name = "tblFanControl";
            this.tblFanControl.RowCount = 2;
            this.tblFanControl.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblFanControl.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblFanControl.Size = new System.Drawing.Size(659, 423);
            this.tblFanControl.TabIndex = 0;
            // 
            // cboProfPerfMode
            // 
            this.cboProfPerfMode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cboProfPerfMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboProfPerfMode.DropDownWidth = 200;
            this.cboProfPerfMode.FormattingEnabled = true;
            this.cboProfPerfMode.Location = new System.Drawing.Point(474, 4);
            this.cboProfPerfMode.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cboProfPerfMode.Name = "cboProfPerfMode";
            this.cboProfPerfMode.Size = new System.Drawing.Size(179, 28);
            this.cboProfPerfMode.TabIndex = 7;
            this.cboProfPerfMode.SelectedIndexChanged += new System.EventHandler(this.ProfPerfModeChanged);
            // 
            // lblProfPerfMode
            // 
            lblProfPerfMode.Anchor = System.Windows.Forms.AnchorStyles.Left;
            lblProfPerfMode.AutoSize = true;
            lblProfPerfMode.Location = new System.Drawing.Point(387, 7);
            lblProfPerfMode.Margin = new System.Windows.Forms.Padding(4, 4, 0, 4);
            lblProfPerfMode.Name = "lblProfPerfMode";
            lblProfPerfMode.Padding = new System.Windows.Forms.Padding(0, 0, 0, 2);
            lblProfPerfMode.Size = new System.Drawing.Size(83, 22);
            lblProfPerfMode.TabIndex = 6;
            lblProfPerfMode.Text = "Perf. mode:";
            // 
            // btnProfDel
            // 
            this.btnProfDel.Location = new System.Drawing.Point(354, 4);
            this.btnProfDel.Margin = new System.Windows.Forms.Padding(0, 4, 0, 4);
            this.btnProfDel.Name = "btnProfDel";
            this.btnProfDel.Size = new System.Drawing.Size(29, 29);
            this.btnProfDel.TabIndex = 5;
            this.btnProfDel.Text = "-";
            this.btnProfDel.UseVisualStyleBackColor = true;
            this.btnProfDel.Click += new System.EventHandler(this.ProfDel);
            // 
            // btnProfAdd
            // 
            this.btnProfAdd.Location = new System.Drawing.Point(325, 4);
            this.btnProfAdd.Margin = new System.Windows.Forms.Padding(0, 4, 0, 4);
            this.btnProfAdd.Name = "btnProfAdd";
            this.btnProfAdd.Size = new System.Drawing.Size(29, 29);
            this.btnProfAdd.TabIndex = 4;
            this.btnProfAdd.Text = "+";
            this.btnProfAdd.UseVisualStyleBackColor = true;
            this.btnProfAdd.Click += new System.EventHandler(this.ProfAdd);
            this.btnProfAdd.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.btnProfAdd_KeyPress);
            // 
            // cboProfSel
            // 
            this.cboProfSel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cboProfSel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboProfSel.FormattingEnabled = true;
            this.cboProfSel.Location = new System.Drawing.Point(215, 4);
            this.cboProfSel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cboProfSel.Name = "cboProfSel";
            this.cboProfSel.Size = new System.Drawing.Size(106, 28);
            this.cboProfSel.TabIndex = 3;
            this.cboProfSel.SelectedIndexChanged += new System.EventHandler(this.ProfSelChanged);
            // 
            // lblProfSel
            // 
            lblProfSel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            lblProfSel.AutoSize = true;
            lblProfSel.Location = new System.Drawing.Point(156, 7);
            lblProfSel.Margin = new System.Windows.Forms.Padding(4, 4, 0, 4);
            lblProfSel.Name = "lblProfSel";
            lblProfSel.Padding = new System.Windows.Forms.Padding(0, 0, 0, 2);
            lblProfSel.Size = new System.Drawing.Size(55, 22);
            lblProfSel.TabIndex = 2;
            lblProfSel.Text = "Profile:";
            // 
            // cboFanSel
            // 
            this.cboFanSel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cboFanSel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFanSel.FormattingEnabled = true;
            this.cboFanSel.Location = new System.Drawing.Point(42, 4);
            this.cboFanSel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cboFanSel.Name = "cboFanSel";
            this.cboFanSel.Size = new System.Drawing.Size(106, 28);
            this.cboFanSel.TabIndex = 1;
            this.cboFanSel.SelectedIndexChanged += new System.EventHandler(this.FanSelChanged);
            // 
            // lblFanSel
            // 
            lblFanSel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            lblFanSel.AutoSize = true;
            lblFanSel.Location = new System.Drawing.Point(4, 7);
            lblFanSel.Margin = new System.Windows.Forms.Padding(4, 4, 0, 4);
            lblFanSel.Name = "lblFanSel";
            lblFanSel.Padding = new System.Windows.Forms.Padding(0, 0, 0, 2);
            lblFanSel.Size = new System.Drawing.Size(34, 22);
            lblFanSel.TabIndex = 0;
            lblFanSel.Text = "Fan:";
            // 
            // tblCurve
            // 
            this.tblCurve.AutoSize = true;
            this.tblCurve.ColumnCount = 1;
            this.tblFanControl.SetColumnSpan(this.tblCurve, 8);
            this.tblCurve.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblCurve.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tblCurve.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblCurve.Location = new System.Drawing.Point(0, 37);
            this.tblCurve.Margin = new System.Windows.Forms.Padding(0);
            this.tblCurve.Name = "tblCurve";
            this.tblCurve.RowCount = 4;
            this.tblCurve.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblCurve.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblCurve.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblCurve.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblCurve.Size = new System.Drawing.Size(657, 386);
            this.tblCurve.TabIndex = 2;
            // 
            // tabExtra
            // 
            tabExtra.BackColor = System.Drawing.Color.White;
            tabExtra.Controls.Add(tblExtra);
            tabExtra.Location = new System.Drawing.Point(4, 29);
            tabExtra.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            tabExtra.Name = "tabExtra";
            tabExtra.Size = new System.Drawing.Size(660, 409);
            tabExtra.TabIndex = 1;
            tabExtra.Text = "Extras";
            // 
            // tblExtra
            // 
            tblExtra.ColumnCount = 2;
            tblExtra.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tblExtra.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tblExtra.Controls.Add(this.cboFanMode, 1, 4);
            tblExtra.Controls.Add(lblFanMode, 0, 4);
            tblExtra.Controls.Add(lblChgLim, 0, 0);
            tblExtra.Controls.Add(lblPerfMode, 0, 1);
            tblExtra.Controls.Add(this.cboPerfMode, 1, 1);
            tblExtra.Controls.Add(lblWinFnSwap, 0, 2);
            tblExtra.Controls.Add(this.chkWinFnSwap, 1, 2);
            tblExtra.Controls.Add(lblKeyLight, 0, 3);
            tblExtra.Controls.Add(flwKeyLight, 1, 3);
            tblExtra.Controls.Add(flwChgLim, 1, 0);
            tblExtra.Dock = System.Windows.Forms.DockStyle.Fill;
            tblExtra.Location = new System.Drawing.Point(0, 0);
            tblExtra.Margin = new System.Windows.Forms.Padding(0);
            tblExtra.Name = "tblExtra";
            tblExtra.RowCount = 6;
            tblExtra.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tblExtra.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tblExtra.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tblExtra.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tblExtra.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tblExtra.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tblExtra.Size = new System.Drawing.Size(660, 409);
            tblExtra.TabIndex = 0;
            // 
            // cboFanMode
            // 
            this.cboFanMode.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.cboFanMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFanMode.FormattingEnabled = true;
            this.cboFanMode.Location = new System.Drawing.Point(202, 171);
            this.cboFanMode.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cboFanMode.Name = "cboFanMode";
            this.cboFanMode.Size = new System.Drawing.Size(186, 28);
            this.cboFanMode.TabIndex = 9;
            this.cboFanMode.SelectedIndexChanged += new System.EventHandler(this.FanModeChange);
            // 
            // lblFanMode
            // 
            lblFanMode.Anchor = System.Windows.Forms.AnchorStyles.Right;
            lblFanMode.AutoSize = true;
            lblFanMode.Location = new System.Drawing.Point(43, 175);
            lblFanMode.Margin = new System.Windows.Forms.Padding(4, 4, 0, 4);
            lblFanMode.Name = "lblFanMode";
            lblFanMode.Size = new System.Drawing.Size(155, 20);
            lblFanMode.TabIndex = 8;
            lblFanMode.Text = "Fan mode (advanced):";
            // 
            // lblChgLim
            // 
            lblChgLim.Anchor = System.Windows.Forms.AnchorStyles.Right;
            lblChgLim.AutoSize = true;
            lblChgLim.Location = new System.Drawing.Point(105, 5);
            lblChgLim.Margin = new System.Windows.Forms.Padding(4, 4, 0, 4);
            lblChgLim.Name = "lblChgLim";
            lblChgLim.Padding = new System.Windows.Forms.Padding(0, 0, 0, 4);
            lblChgLim.Size = new System.Drawing.Size(93, 24);
            lblChgLim.TabIndex = 0;
            lblChgLim.Text = "Charge limit:";
            // 
            // lblPerfMode
            // 
            lblPerfMode.Anchor = System.Windows.Forms.AnchorStyles.Right;
            lblPerfMode.AutoSize = true;
            lblPerfMode.Location = new System.Drawing.Point(5, 43);
            lblPerfMode.Margin = new System.Windows.Forms.Padding(5, 4, 0, 4);
            lblPerfMode.Name = "lblPerfMode";
            lblPerfMode.Size = new System.Drawing.Size(193, 20);
            lblPerfMode.TabIndex = 2;
            lblPerfMode.Text = "Default performance mode:";
            // 
            // cboPerfMode
            // 
            this.cboPerfMode.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.cboPerfMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPerfMode.FormattingEnabled = true;
            this.cboPerfMode.Location = new System.Drawing.Point(202, 39);
            this.cboPerfMode.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.cboPerfMode.Name = "cboPerfMode";
            this.cboPerfMode.Size = new System.Drawing.Size(186, 28);
            this.cboPerfMode.TabIndex = 3;
            this.cboPerfMode.SelectedIndexChanged += new System.EventHandler(this.PerfModeChange);
            // 
            // lblWinFnSwap
            // 
            lblWinFnSwap.Anchor = System.Windows.Forms.AnchorStyles.Right;
            lblWinFnSwap.AutoSize = true;
            lblWinFnSwap.Location = new System.Drawing.Point(40, 76);
            lblWinFnSwap.Margin = new System.Windows.Forms.Padding(4, 4, 0, 4);
            lblWinFnSwap.Name = "lblWinFnSwap";
            lblWinFnSwap.Padding = new System.Windows.Forms.Padding(0, 0, 0, 2);
            lblWinFnSwap.Size = new System.Drawing.Size(158, 22);
            lblWinFnSwap.TabIndex = 4;
            lblWinFnSwap.Text = "Swap Win and Fn keys:";
            // 
            // chkWinFnSwap
            // 
            this.chkWinFnSwap.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.chkWinFnSwap.AutoSize = true;
            this.chkWinFnSwap.Location = new System.Drawing.Point(202, 75);
            this.chkWinFnSwap.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkWinFnSwap.Name = "chkWinFnSwap";
            this.chkWinFnSwap.Size = new System.Drawing.Size(85, 24);
            this.chkWinFnSwap.TabIndex = 5;
            this.chkWinFnSwap.Text = "Enabled";
            this.chkWinFnSwap.UseVisualStyleBackColor = true;
            this.chkWinFnSwap.CheckedChanged += new System.EventHandler(this.WinFnSwapToggle);
            // 
            // lblKeyLight
            // 
            lblKeyLight.Anchor = System.Windows.Forms.AnchorStyles.Right;
            lblKeyLight.AutoSize = true;
            lblKeyLight.Location = new System.Drawing.Point(57, 125);
            lblKeyLight.Margin = new System.Windows.Forms.Padding(4, 4, 0, 4);
            lblKeyLight.Name = "lblKeyLight";
            lblKeyLight.Size = new System.Drawing.Size(141, 20);
            lblKeyLight.TabIndex = 6;
            lblKeyLight.Text = "Keyboard backlight:";
            // 
            // flwKeyLight
            // 
            flwKeyLight.AutoSize = true;
            flwKeyLight.Controls.Add(this.lblKeyLightLow);
            flwKeyLight.Controls.Add(this.tbKeyLight);
            flwKeyLight.Controls.Add(this.lblKeyLightHigh);
            flwKeyLight.Location = new System.Drawing.Point(198, 103);
            flwKeyLight.Margin = new System.Windows.Forms.Padding(0);
            flwKeyLight.Name = "flwKeyLight";
            flwKeyLight.Size = new System.Drawing.Size(283, 64);
            flwKeyLight.TabIndex = 7;
            // 
            // lblKeyLightLow
            // 
            this.lblKeyLightLow.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblKeyLightLow.AutoSize = true;
            this.lblKeyLightLow.Location = new System.Drawing.Point(4, 20);
            this.lblKeyLightLow.Margin = new System.Windows.Forms.Padding(4, 0, 0, 4);
            this.lblKeyLightLow.Name = "lblKeyLightLow";
            this.lblKeyLightLow.Size = new System.Drawing.Size(30, 20);
            this.lblKeyLightLow.TabIndex = 0;
            this.lblKeyLightLow.Text = "Off";
            // 
            // tbKeyLight
            // 
            this.tbKeyLight.LargeChange = 1;
            this.tbKeyLight.Location = new System.Drawing.Point(38, 4);
            this.tbKeyLight.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tbKeyLight.Name = "tbKeyLight";
            this.tbKeyLight.Size = new System.Drawing.Size(188, 56);
            this.tbKeyLight.TabIndex = 1;
            this.tbKeyLight.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.tbKeyLight.Scroll += new System.EventHandler(this.KeyLightChange);
            // 
            // lblKeyLightHigh
            // 
            this.lblKeyLightHigh.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblKeyLightHigh.AutoSize = true;
            this.lblKeyLightHigh.Location = new System.Drawing.Point(230, 20);
            this.lblKeyLightHigh.Margin = new System.Windows.Forms.Padding(0, 0, 4, 4);
            this.lblKeyLightHigh.Name = "lblKeyLightHigh";
            this.lblKeyLightHigh.Size = new System.Drawing.Size(49, 20);
            this.lblKeyLightHigh.TabIndex = 2;
            this.lblKeyLightHigh.Text = "Bright";
            // 
            // flwChgLim
            // 
            flwChgLim.AutoSize = true;
            flwChgLim.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            flwChgLim.Controls.Add(this.chkChgLim);
            flwChgLim.Controls.Add(this.numChgLim);
            flwChgLim.Location = new System.Drawing.Point(198, 0);
            flwChgLim.Margin = new System.Windows.Forms.Padding(0);
            flwChgLim.Name = "flwChgLim";
            flwChgLim.Size = new System.Drawing.Size(163, 35);
            flwChgLim.TabIndex = 1;
            // 
            // chkChgLim
            // 
            this.chkChgLim.AutoSize = true;
            this.chkChgLim.Location = new System.Drawing.Point(4, 4);
            this.chkChgLim.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.chkChgLim.Name = "chkChgLim";
            this.chkChgLim.Size = new System.Drawing.Size(85, 24);
            this.chkChgLim.TabIndex = 0;
            this.chkChgLim.Text = "Enabled";
            this.chkChgLim.UseVisualStyleBackColor = true;
            this.chkChgLim.CheckedChanged += new System.EventHandler(this.ChgLimToggle);
            // 
            // numChgLim
            // 
            this.numChgLim.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.numChgLim.Location = new System.Drawing.Point(97, 4);
            this.numChgLim.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.numChgLim.Name = "numChgLim";
            this.numChgLim.Size = new System.Drawing.Size(62, 27);
            this.numChgLim.TabIndex = 1;
            // 
            // tabInfo
            // 
            this.tabInfo.Controls.Add(tblInfo);
            this.tabInfo.Location = new System.Drawing.Point(4, 29);
            this.tabInfo.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabInfo.Name = "tabInfo";
            this.tabInfo.Padding = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.tabInfo.Size = new System.Drawing.Size(660, 409);
            this.tabInfo.TabIndex = 2;
            this.tabInfo.Text = "Info";
            this.tabInfo.UseVisualStyleBackColor = true;
            // 
            // tblInfo
            // 
            tblInfo.ColumnCount = 2;
            tblInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tblInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tblInfo.Controls.Add(this.txtFirmDate, 1, 4);
            tblInfo.Controls.Add(this.txtModel, 1, 2);
            tblInfo.Controls.Add(this.txtManufacturer, 1, 1);
            tblInfo.Controls.Add(lblModel, 0, 2);
            tblInfo.Controls.Add(lblManufacturer, 0, 1);
            tblInfo.Controls.Add(lblAuthor, 0, 0);
            tblInfo.Controls.Add(this.txtAuthor, 1, 0);
            tblInfo.Controls.Add(this.btnGetModel, 1, 5);
            tblInfo.Controls.Add(lblFirmVer, 0, 3);
            tblInfo.Controls.Add(lblFirmDate, 0, 4);
            tblInfo.Controls.Add(this.txtFirmVer, 1, 3);
            tblInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            tblInfo.Location = new System.Drawing.Point(4, 4);
            tblInfo.Margin = new System.Windows.Forms.Padding(0);
            tblInfo.Name = "tblInfo";
            tblInfo.RowCount = 6;
            tblInfo.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tblInfo.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tblInfo.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tblInfo.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tblInfo.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tblInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tblInfo.Size = new System.Drawing.Size(652, 401);
            tblInfo.TabIndex = 1;
            // 
            // txtFirmDate
            // 
            this.txtFirmDate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtFirmDate.Location = new System.Drawing.Point(163, 144);
            this.txtFirmDate.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtFirmDate.Name = "txtFirmDate";
            this.txtFirmDate.ReadOnly = true;
            this.txtFirmDate.Size = new System.Drawing.Size(485, 27);
            this.txtFirmDate.TabIndex = 10;
            // 
            // txtModel
            // 
            this.txtModel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtModel.Location = new System.Drawing.Point(163, 74);
            this.txtModel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtModel.Name = "txtModel";
            this.txtModel.ReadOnly = true;
            this.txtModel.Size = new System.Drawing.Size(485, 27);
            this.txtModel.TabIndex = 5;
            // 
            // txtManufacturer
            // 
            this.txtManufacturer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtManufacturer.Location = new System.Drawing.Point(163, 39);
            this.txtManufacturer.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtManufacturer.Name = "txtManufacturer";
            this.txtManufacturer.ReadOnly = true;
            this.txtManufacturer.Size = new System.Drawing.Size(485, 27);
            this.txtManufacturer.TabIndex = 4;
            // 
            // lblModel
            // 
            lblModel.Anchor = System.Windows.Forms.AnchorStyles.Right;
            lblModel.AutoSize = true;
            lblModel.Location = new System.Drawing.Point(49, 76);
            lblModel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblModel.Name = "lblModel";
            lblModel.Padding = new System.Windows.Forms.Padding(0, 0, 0, 2);
            lblModel.Size = new System.Drawing.Size(106, 22);
            lblModel.TabIndex = 0;
            lblModel.Text = "Laptop model:";
            // 
            // lblManufacturer
            // 
            lblManufacturer.Anchor = System.Windows.Forms.AnchorStyles.Right;
            lblManufacturer.AutoSize = true;
            lblManufacturer.Location = new System.Drawing.Point(4, 41);
            lblManufacturer.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblManufacturer.Name = "lblManufacturer";
            lblManufacturer.Padding = new System.Windows.Forms.Padding(0, 0, 0, 2);
            lblManufacturer.Size = new System.Drawing.Size(151, 22);
            lblManufacturer.TabIndex = 2;
            lblManufacturer.Text = "Laptop manufacturer:";
            // 
            // lblAuthor
            // 
            lblAuthor.Anchor = System.Windows.Forms.AnchorStyles.Right;
            lblAuthor.AutoSize = true;
            lblAuthor.Location = new System.Drawing.Point(52, 6);
            lblAuthor.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblAuthor.Name = "lblAuthor";
            lblAuthor.Padding = new System.Windows.Forms.Padding(0, 0, 0, 2);
            lblAuthor.Size = new System.Drawing.Size(103, 22);
            lblAuthor.TabIndex = 1;
            lblAuthor.Text = "Config author:";
            // 
            // txtAuthor
            // 
            this.txtAuthor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtAuthor.Location = new System.Drawing.Point(163, 4);
            this.txtAuthor.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtAuthor.Name = "txtAuthor";
            this.txtAuthor.Size = new System.Drawing.Size(485, 27);
            this.txtAuthor.TabIndex = 3;
            this.txtAuthor.Validating += new System.ComponentModel.CancelEventHandler(this.txtAuthor_Validating);
            // 
            // btnGetModel
            // 
            this.btnGetModel.Location = new System.Drawing.Point(163, 179);
            this.btnGetModel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.btnGetModel.Name = "btnGetModel";
            this.btnGetModel.Size = new System.Drawing.Size(219, 31);
            this.btnGetModel.TabIndex = 6;
            this.btnGetModel.Text = "Get model and manufacturer";
            this.btnGetModel.UseVisualStyleBackColor = true;
            this.btnGetModel.Click += new System.EventHandler(this.btnGetModel_Click);
            // 
            // lblFirmVer
            // 
            lblFirmVer.Anchor = System.Windows.Forms.AnchorStyles.Right;
            lblFirmVer.AutoSize = true;
            lblFirmVer.Location = new System.Drawing.Point(12, 112);
            lblFirmVer.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblFirmVer.Name = "lblFirmVer";
            lblFirmVer.Size = new System.Drawing.Size(143, 20);
            lblFirmVer.TabIndex = 7;
            lblFirmVer.Text = "EC firmware version:";
            // 
            // lblFirmDate
            // 
            lblFirmDate.Anchor = System.Windows.Forms.AnchorStyles.Right;
            lblFirmDate.AutoSize = true;
            lblFirmDate.Location = new System.Drawing.Point(29, 147);
            lblFirmDate.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            lblFirmDate.Name = "lblFirmDate";
            lblFirmDate.Size = new System.Drawing.Size(126, 20);
            lblFirmDate.TabIndex = 8;
            lblFirmDate.Text = "EC firmware date:";
            // 
            // txtFirmVer
            // 
            this.txtFirmVer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtFirmVer.Location = new System.Drawing.Point(163, 109);
            this.txtFirmVer.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            this.txtFirmVer.Name = "txtFirmVer";
            this.txtFirmVer.ReadOnly = true;
            this.txtFirmVer.Size = new System.Drawing.Size(485, 27);
            this.txtFirmVer.TabIndex = 9;
            // 
            // tblApply
            // 
            tblApply.AutoSize = true;
            tblApply.ColumnCount = 3;
            tblApply.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tblApply.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tblApply.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tblApply.Controls.Add(this.chkFullBlast, 0, 0);
            tblApply.Controls.Add(this.btnRevert, 1, 0);
            tblApply.Controls.Add(this.btnApply, 2, 0);
            tblApply.Dock = System.Windows.Forms.DockStyle.Fill;
            tblApply.Location = new System.Drawing.Point(2, 462);
            tblApply.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            tblApply.Name = "tblApply";
            tblApply.RowCount = 1;
            tblApply.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tblApply.Size = new System.Drawing.Size(671, 36);
            tblApply.TabIndex = 3;
            // 
            // chkFullBlast
            // 
            this.chkFullBlast.AutoSize = true;
            this.chkFullBlast.Location = new System.Drawing.Point(6, 6);
            this.chkFullBlast.Margin = new System.Windows.Forms.Padding(6, 6, 8, 6);
            this.chkFullBlast.Name = "chkFullBlast";
            this.chkFullBlast.Size = new System.Drawing.Size(90, 24);
            this.chkFullBlast.TabIndex = 0;
            this.chkFullBlast.Text = "Full Blast";
            this.chkFullBlast.UseVisualStyleBackColor = true;
            this.chkFullBlast.CheckedChanged += new System.EventHandler(this.FullBlastToggle);
            // 
            // btnRevert
            // 
            this.btnRevert.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnRevert.Location = new System.Drawing.Point(473, 2);
            this.btnRevert.Margin = new System.Windows.Forms.Padding(2, 2, 4, 2);
            this.btnRevert.Name = "btnRevert";
            this.btnRevert.Size = new System.Drawing.Size(94, 31);
            this.btnRevert.TabIndex = 1;
            this.btnRevert.Text = "Revert";
            this.btnRevert.UseVisualStyleBackColor = true;
            this.btnRevert.Click += new System.EventHandler(this.RevertConf);
            // 
            // btnApply
            // 
            this.btnApply.Location = new System.Drawing.Point(573, 2);
            this.btnApply.Margin = new System.Windows.Forms.Padding(2, 2, 4, 2);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(94, 31);
            this.btnApply.TabIndex = 2;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.ApplyConf);
            // 
            // flwStats
            // 
            flwStats.AutoSize = true;
            flwStats.Controls.Add(this.lblStatus);
            flwStats.Controls.Add(this.lblFanSpd);
            flwStats.Controls.Add(this.lblFanRPM);
            flwStats.Controls.Add(this.lblTemp);
            flwStats.Dock = System.Windows.Forms.DockStyle.Fill;
            flwStats.Location = new System.Drawing.Point(0, 498);
            flwStats.Margin = new System.Windows.Forms.Padding(0, 0, 0, 4);
            flwStats.Name = "flwStats";
            flwStats.Size = new System.Drawing.Size(675, 20);
            flwStats.TabIndex = 4;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(4, 0);
            this.lblStatus.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(50, 20);
            this.lblStatus.TabIndex = 0;
            this.lblStatus.Text = "Ready";
            // 
            // lblFanSpd
            // 
            this.lblFanSpd.AutoSize = true;
            this.lblFanSpd.Location = new System.Drawing.Point(62, 0);
            this.lblFanSpd.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblFanSpd.Name = "lblFanSpd";
            this.lblFanSpd.Size = new System.Drawing.Size(106, 20);
            this.lblFanSpd.TabIndex = 1;
            this.lblFanSpd.Text = "Fan speed: --%";
            this.lblFanSpd.Visible = false;
            // 
            // lblFanRPM
            // 
            this.lblFanRPM.AutoSize = true;
            this.lblFanRPM.Location = new System.Drawing.Point(176, 0);
            this.lblFanRPM.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblFanRPM.Name = "lblFanRPM";
            this.lblFanRPM.Size = new System.Drawing.Size(70, 20);
            this.lblFanRPM.TabIndex = 2;
            this.lblFanRPM.Text = "RPM: ----";
            this.lblFanRPM.Visible = false;
            // 
            // lblTemp
            // 
            this.lblTemp.AutoSize = true;
            this.lblTemp.Location = new System.Drawing.Point(254, 0);
            this.lblTemp.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblTemp.Name = "lblTemp";
            this.lblTemp.Size = new System.Drawing.Size(80, 20);
            this.lblTemp.TabIndex = 3;
            this.lblTemp.Text = "Temp: --°C";
            this.lblTemp.Visible = false;
            // 
            // tblMain
            // 
            this.tblMain.ColumnCount = 1;
            this.tblMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblMain.Controls.Add(tcMain, 0, 0);
            this.tblMain.Controls.Add(tblApply, 0, 1);
            this.tblMain.Controls.Add(flwStats, 0, 2);
            this.tblMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblMain.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tblMain.Location = new System.Drawing.Point(0, 28);
            this.tblMain.Margin = new System.Windows.Forms.Padding(0);
            this.tblMain.Name = "tblMain";
            this.tblMain.RowCount = 3;
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblMain.Size = new System.Drawing.Size(675, 522);
            this.tblMain.TabIndex = 1;
            // 
            // MainForm
            // 
            this.AcceptButton = this.btnApply;
            this.AutoScaleDimensions = new System.Drawing.SizeF(120F, 120F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.btnRevert;
            this.ClientSize = new System.Drawing.Size(675, 550);
            this.Controls.Add(this.tblMain);
            this.Controls.Add(menuStrip);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = menuStrip;
            this.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            menuStrip.ResumeLayout(false);
            menuStrip.PerformLayout();
            tcMain.ResumeLayout(false);
            tabFanControl.ResumeLayout(false);
            this.tblFanControl.ResumeLayout(false);
            this.tblFanControl.PerformLayout();
            tabExtra.ResumeLayout(false);
            tblExtra.ResumeLayout(false);
            tblExtra.PerformLayout();
            flwKeyLight.ResumeLayout(false);
            flwKeyLight.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbKeyLight)).EndInit();
            flwChgLim.ResumeLayout(false);
            flwChgLim.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numChgLim)).EndInit();
            this.tabInfo.ResumeLayout(false);
            tblInfo.ResumeLayout(false);
            tblInfo.PerformLayout();
            tblApply.ResumeLayout(false);
            tblApply.PerformLayout();
            flwStats.ResumeLayout(false);
            flwStats.PerformLayout();
            this.tblMain.ResumeLayout(false);
            this.tblMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStripMenuItem tsiFile;
        private System.Windows.Forms.ToolStripMenuItem tsiLoadConf;
        private System.Windows.Forms.ToolStripMenuItem tsiSaveConf;
        private System.Windows.Forms.ToolStripMenuItem tsiApply;
        private System.Windows.Forms.ToolStripMenuItem tsiRevert;
        private System.Windows.Forms.ToolStripMenuItem tsiExit;
        private System.Windows.Forms.ToolStripMenuItem tsiOptions;
        private System.Windows.Forms.ToolStripMenuItem tsiProfAdd;
        private System.Windows.Forms.ToolStripMenuItem tsiProfEdit;
        private System.Windows.Forms.ToolStripMenuItem tsiProfRen;
        private System.Windows.Forms.ToolStripMenuItem tsiProfChangeDesc;
        private System.Windows.Forms.ToolStripMenuItem tsiProfDel;
        private System.Windows.Forms.ToolStripMenuItem tsiECtoConf;
        private System.Windows.Forms.ToolStripMenuItem tsiECMon;
        private System.Windows.Forms.ToolStripMenuItem tsiStopSvc;
        private System.Windows.Forms.ToolStripMenuItem tsiUninstall;
        private System.Windows.Forms.ToolStripMenuItem tsiHelp;
        private System.Windows.Forms.ToolStripMenuItem tsiAbout;
        private System.Windows.Forms.ToolStripMenuItem tsiSource;
        private System.Windows.Forms.TableLayoutPanel tblFanControl;
        private System.Windows.Forms.TableLayoutPanel tblCurve;
        private System.Windows.Forms.ComboBox cboFanSel;
        private System.Windows.Forms.ComboBox cboProfSel;
        private System.Windows.Forms.Button btnProfAdd;
        private System.Windows.Forms.Button btnProfDel;
        private System.Windows.Forms.NumericUpDown numChgLim;
        private System.Windows.Forms.ComboBox cboPerfMode;
        private System.Windows.Forms.CheckBox chkWinFnSwap;
        private System.Windows.Forms.TrackBar tbKeyLight;
        private System.Windows.Forms.CheckBox chkFullBlast;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button btnRevert;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblFanSpd;
        private System.Windows.Forms.Label lblFanRPM;
        private System.Windows.Forms.Label lblTemp;
        private System.Windows.Forms.TableLayoutPanel tblMain;
        private System.Windows.Forms.Label lblKeyLightLow;
        private System.Windows.Forms.Label lblKeyLightHigh;
        private System.Windows.Forms.CheckBox chkChgLim;
        private System.Windows.Forms.ToolStripMenuItem tsiLogNone;
        private System.Windows.Forms.ToolStripMenuItem tsiLogDebug;
        private System.Windows.Forms.ToolStripMenuItem tsiLogInfo;
        private System.Windows.Forms.ToolStripMenuItem tsiLogWarn;
        private System.Windows.Forms.ToolStripMenuItem tsiLogError;
        private System.Windows.Forms.ToolStripMenuItem tsiLogFatal;
        private System.Windows.Forms.ToolStripMenuItem tsiAdvanced;
        private System.Windows.Forms.ComboBox cboFanMode;
        private System.Windows.Forms.TabPage tabInfo;
        private System.Windows.Forms.TextBox txtModel;
        private System.Windows.Forms.TextBox txtManufacturer;
        private System.Windows.Forms.TextBox txtAuthor;
        private System.Windows.Forms.Button btnGetModel;
        private System.Windows.Forms.TextBox txtFirmDate;
        private System.Windows.Forms.TextBox txtFirmVer;
        private System.Windows.Forms.ComboBox cboProfPerfMode;
    }
}
