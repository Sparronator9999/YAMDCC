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

namespace YAMDCC.ConfigEditor
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
            System.Windows.Forms.MenuStrip menuStrip;
            System.Windows.Forms.ToolStripSeparator sep1;
            System.Windows.Forms.ToolStripSeparator sep2;
            System.Windows.Forms.ToolStripSeparator sep3;
            System.Windows.Forms.ToolStripSeparator sep4;
            System.Windows.Forms.ToolStripSeparator sep5;
            System.Windows.Forms.TabControl tcMain;
            System.Windows.Forms.TabPage tabFanControl;
            System.Windows.Forms.FlowLayoutPanel flwFanSelect;
            System.Windows.Forms.TabPage tabOptions;
            System.Windows.Forms.TableLayoutPanel tblExtra;
            System.Windows.Forms.Label lblChgLim;
            System.Windows.Forms.Label lblPerfMode;
            System.Windows.Forms.Label lblWinFnSwap;
            System.Windows.Forms.Label lblKeyLight;
            System.Windows.Forms.FlowLayoutPanel flwKeyLight;
            System.Windows.Forms.TableLayoutPanel tblFCBottom;
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
            this.tsiProfRename = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiProfChangeDesc = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiProfDel = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiECtoConf = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiECMon = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiStopSvc = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiUninstall = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiSource = new System.Windows.Forms.ToolStripMenuItem();
            this.tblFanControl = new System.Windows.Forms.TableLayoutPanel();
            this.tblCurve = new System.Windows.Forms.TableLayoutPanel();
            this.lblSpeed = new System.Windows.Forms.Label();
            this.lblUpT = new System.Windows.Forms.Label();
            this.lblDownT = new System.Windows.Forms.Label();
            this.lblFanSel = new System.Windows.Forms.Label();
            this.cboFanSel = new System.Windows.Forms.ComboBox();
            this.lblProfSel = new System.Windows.Forms.Label();
            this.cboProfSel = new System.Windows.Forms.ComboBox();
            this.btnProfAdd = new System.Windows.Forms.Button();
            this.btnProfDel = new System.Windows.Forms.Button();
            this.numChgLim = new System.Windows.Forms.NumericUpDown();
            this.cboPerfMode = new System.Windows.Forms.ComboBox();
            this.chkWinFnSwap = new System.Windows.Forms.CheckBox();
            this.lblKeyLightLow = new System.Windows.Forms.Label();
            this.tbKeyLight = new System.Windows.Forms.TrackBar();
            this.lblKeyLightHigh = new System.Windows.Forms.Label();
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
            tcMain = new System.Windows.Forms.TabControl();
            tabFanControl = new System.Windows.Forms.TabPage();
            flwFanSelect = new System.Windows.Forms.FlowLayoutPanel();
            tabOptions = new System.Windows.Forms.TabPage();
            tblExtra = new System.Windows.Forms.TableLayoutPanel();
            lblChgLim = new System.Windows.Forms.Label();
            lblPerfMode = new System.Windows.Forms.Label();
            lblWinFnSwap = new System.Windows.Forms.Label();
            lblKeyLight = new System.Windows.Forms.Label();
            flwKeyLight = new System.Windows.Forms.FlowLayoutPanel();
            tblFCBottom = new System.Windows.Forms.TableLayoutPanel();
            flwStats = new System.Windows.Forms.FlowLayoutPanel();
            menuStrip.SuspendLayout();
            tcMain.SuspendLayout();
            tabFanControl.SuspendLayout();
            this.tblFanControl.SuspendLayout();
            this.tblCurve.SuspendLayout();
            flwFanSelect.SuspendLayout();
            tabOptions.SuspendLayout();
            tblExtra.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numChgLim)).BeginInit();
            flwKeyLight.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbKeyLight)).BeginInit();
            tblFCBottom.SuspendLayout();
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
            menuStrip.Padding = new System.Windows.Forms.Padding(5, 2, 0, 2);
            menuStrip.Size = new System.Drawing.Size(540, 24);
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
            sep1.Name = "sep1";
            sep1.Size = new System.Drawing.Size(151, 6);
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
            sep2.Name = "sep2";
            sep2.Size = new System.Drawing.Size(151, 6);
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
            this.tsiECtoConf,
            sep4,
            this.tsiECMon,
            sep5,
            this.tsiStopSvc,
            this.tsiUninstall});
            this.tsiOptions.Name = "tsiOptions";
            this.tsiOptions.Size = new System.Drawing.Size(61, 20);
            this.tsiOptions.Text = "Options";
            // 
            // tsiProfAdd
            // 
            this.tsiProfAdd.Name = "tsiProfAdd";
            this.tsiProfAdd.Size = new System.Drawing.Size(244, 22);
            this.tsiProfAdd.Text = "New fan profile...";
            this.tsiProfAdd.Click += new System.EventHandler(this.tsiProfAdd_Click);
            // 
            // tsiProfEdit
            // 
            this.tsiProfEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsiProfRename,
            this.tsiProfChangeDesc,
            sep3,
            this.tsiProfDel});
            this.tsiProfEdit.Name = "tsiProfEdit";
            this.tsiProfEdit.Size = new System.Drawing.Size(244, 22);
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
            sep3.Name = "sep3";
            sep3.Size = new System.Drawing.Size(175, 6);
            // 
            // tsiProfDel
            // 
            this.tsiProfDel.Name = "tsiProfDel";
            this.tsiProfDel.Size = new System.Drawing.Size(178, 22);
            this.tsiProfDel.Text = "Delete";
            this.tsiProfDel.Click += new System.EventHandler(this.tsiProfDel_Click);
            // 
            // tsiECtoConf
            // 
            this.tsiECtoConf.Name = "tsiECtoConf";
            this.tsiECtoConf.Size = new System.Drawing.Size(244, 22);
            this.tsiECtoConf.Text = "Get default fan profile from EC...";
            this.tsiECtoConf.Click += new System.EventHandler(this.tsiECtoConf_Click);
            // 
            // sep4
            // 
            sep4.Name = "sep4";
            sep4.Size = new System.Drawing.Size(241, 6);
            // 
            // tsiECMon
            // 
            this.tsiECMon.CheckOnClick = true;
            this.tsiECMon.Name = "tsiECMon";
            this.tsiECMon.Size = new System.Drawing.Size(244, 22);
            this.tsiECMon.Text = "Enable EC monitoring";
            this.tsiECMon.Click += new System.EventHandler(this.tsiECMon_Click);
            // 
            // sep5
            // 
            sep5.Name = "sep5";
            sep5.Size = new System.Drawing.Size(241, 6);
            // 
            // tsiStopSvc
            // 
            this.tsiStopSvc.Name = "tsiStopSvc";
            this.tsiStopSvc.Size = new System.Drawing.Size(244, 22);
            this.tsiStopSvc.Text = "Stop service and exit";
            this.tsiStopSvc.Click += new System.EventHandler(this.tsiStopSvc_Click);
            // 
            // tsiUninstall
            // 
            this.tsiUninstall.Name = "tsiUninstall";
            this.tsiUninstall.Size = new System.Drawing.Size(244, 22);
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
            // tcMain
            // 
            tcMain.Controls.Add(tabFanControl);
            tcMain.Controls.Add(tabOptions);
            tcMain.Dock = System.Windows.Forms.DockStyle.Fill;
            tcMain.Location = new System.Drawing.Point(3, 3);
            tcMain.Margin = new System.Windows.Forms.Padding(3, 3, 3, 2);
            tcMain.Name = "tcMain";
            tcMain.SelectedIndex = 0;
            tcMain.Size = new System.Drawing.Size(534, 364);
            tcMain.TabIndex = 6;
            // 
            // tabFanControl
            // 
            tabFanControl.BackColor = System.Drawing.Color.White;
            tabFanControl.Controls.Add(this.tblFanControl);
            tabFanControl.Location = new System.Drawing.Point(4, 24);
            tabFanControl.Name = "tabFanControl";
            tabFanControl.Size = new System.Drawing.Size(526, 336);
            tabFanControl.TabIndex = 0;
            tabFanControl.Text = "Fan Control";
            // 
            // tblFanControl
            // 
            this.tblFanControl.ColumnCount = 1;
            this.tblFanControl.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblFanControl.Controls.Add(this.tblCurve, 0, 1);
            this.tblFanControl.Controls.Add(flwFanSelect, 0, 0);
            this.tblFanControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblFanControl.Location = new System.Drawing.Point(0, 0);
            this.tblFanControl.Margin = new System.Windows.Forms.Padding(0);
            this.tblFanControl.Name = "tblFanControl";
            this.tblFanControl.RowCount = 3;
            this.tblFanControl.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblFanControl.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblFanControl.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblFanControl.Size = new System.Drawing.Size(526, 336);
            this.tblFanControl.TabIndex = 0;
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
            this.tblCurve.Size = new System.Drawing.Size(526, 307);
            this.tblCurve.TabIndex = 6;
            // 
            // lblSpeed
            // 
            this.lblSpeed.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblSpeed.AutoSize = true;
            this.lblSpeed.Location = new System.Drawing.Point(4, 2);
            this.lblSpeed.Margin = new System.Windows.Forms.Padding(2, 2, 0, 2);
            this.lblSpeed.Name = "lblSpeed";
            this.lblSpeed.Padding = new System.Windows.Forms.Padding(0, 0, 0, 3);
            this.lblSpeed.Size = new System.Drawing.Size(60, 18);
            this.lblSpeed.TabIndex = 0;
            this.lblSpeed.Text = "Speed (%)";
            // 
            // lblUpT
            // 
            this.lblUpT.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblUpT.AutoSize = true;
            this.lblUpT.Location = new System.Drawing.Point(18, 265);
            this.lblUpT.Margin = new System.Windows.Forms.Padding(2, 2, 0, 2);
            this.lblUpT.Name = "lblUpT";
            this.lblUpT.Padding = new System.Windows.Forms.Padding(0, 0, 0, 3);
            this.lblUpT.Size = new System.Drawing.Size(46, 18);
            this.lblUpT.TabIndex = 1;
            this.lblUpT.Text = "Up (°C)";
            // 
            // lblDownT
            // 
            this.lblDownT.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblDownT.AutoSize = true;
            this.lblDownT.Location = new System.Drawing.Point(2, 287);
            this.lblDownT.Margin = new System.Windows.Forms.Padding(2, 2, 0, 2);
            this.lblDownT.Name = "lblDownT";
            this.lblDownT.Padding = new System.Windows.Forms.Padding(0, 0, 0, 3);
            this.lblDownT.Size = new System.Drawing.Size(62, 18);
            this.lblDownT.TabIndex = 2;
            this.lblDownT.Text = "Down (°C)";
            // 
            // flwFanSelect
            // 
            flwFanSelect.AutoSize = true;
            flwFanSelect.Controls.Add(this.lblFanSel);
            flwFanSelect.Controls.Add(this.cboFanSel);
            flwFanSelect.Controls.Add(this.lblProfSel);
            flwFanSelect.Controls.Add(this.cboProfSel);
            flwFanSelect.Controls.Add(this.btnProfAdd);
            flwFanSelect.Controls.Add(this.btnProfDel);
            flwFanSelect.Dock = System.Windows.Forms.DockStyle.Fill;
            flwFanSelect.Location = new System.Drawing.Point(0, 0);
            flwFanSelect.Margin = new System.Windows.Forms.Padding(0);
            flwFanSelect.Name = "flwFanSelect";
            flwFanSelect.Size = new System.Drawing.Size(526, 29);
            flwFanSelect.TabIndex = 0;
            // 
            // lblFanSel
            // 
            this.lblFanSel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblFanSel.AutoSize = true;
            this.lblFanSel.Location = new System.Drawing.Point(3, 6);
            this.lblFanSel.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.lblFanSel.Name = "lblFanSel";
            this.lblFanSel.Padding = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.lblFanSel.Size = new System.Drawing.Size(29, 17);
            this.lblFanSel.TabIndex = 0;
            this.lblFanSel.Text = "Fan:";
            // 
            // cboFanSel
            // 
            this.cboFanSel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFanSel.FormattingEnabled = true;
            this.cboFanSel.Location = new System.Drawing.Point(35, 3);
            this.cboFanSel.Name = "cboFanSel";
            this.cboFanSel.Size = new System.Drawing.Size(119, 23);
            this.cboFanSel.TabIndex = 1;
            this.cboFanSel.SelectedIndexChanged += new System.EventHandler(this.cboFanSel_IndexChanged);
            // 
            // lblProfSel
            // 
            this.lblProfSel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblProfSel.AutoSize = true;
            this.lblProfSel.Location = new System.Drawing.Point(160, 6);
            this.lblProfSel.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.lblProfSel.Name = "lblProfSel";
            this.lblProfSel.Padding = new System.Windows.Forms.Padding(0, 0, 0, 2);
            this.lblProfSel.Size = new System.Drawing.Size(44, 17);
            this.lblProfSel.TabIndex = 2;
            this.lblProfSel.Text = "Profile:";
            // 
            // cboProfSel
            // 
            this.cboProfSel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboProfSel.FormattingEnabled = true;
            this.cboProfSel.Location = new System.Drawing.Point(207, 3);
            this.cboProfSel.Name = "cboProfSel";
            this.cboProfSel.Size = new System.Drawing.Size(119, 23);
            this.cboProfSel.TabIndex = 3;
            this.cboProfSel.SelectedIndexChanged += new System.EventHandler(this.cboProfSel_IndexChanged);
            // 
            // btnProfAdd
            // 
            this.btnProfAdd.Location = new System.Drawing.Point(332, 3);
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
            this.btnProfDel.Location = new System.Drawing.Point(355, 3);
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
            tabOptions.BackColor = System.Drawing.Color.White;
            tabOptions.Controls.Add(tblExtra);
            tabOptions.Location = new System.Drawing.Point(4, 24);
            tabOptions.Name = "tabOptions";
            tabOptions.Size = new System.Drawing.Size(526, 336);
            tabOptions.TabIndex = 1;
            tabOptions.Text = "Extras";
            // 
            // tblExtra
            // 
            tblExtra.ColumnCount = 2;
            tblExtra.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tblExtra.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tblExtra.Controls.Add(lblChgLim, 0, 0);
            tblExtra.Controls.Add(this.numChgLim, 1, 0);
            tblExtra.Controls.Add(lblPerfMode, 0, 1);
            tblExtra.Controls.Add(this.cboPerfMode, 1, 1);
            tblExtra.Controls.Add(lblWinFnSwap, 0, 2);
            tblExtra.Controls.Add(this.chkWinFnSwap, 1, 2);
            tblExtra.Controls.Add(lblKeyLight, 0, 3);
            tblExtra.Controls.Add(flwKeyLight, 1, 3);
            tblExtra.Dock = System.Windows.Forms.DockStyle.Fill;
            tblExtra.Location = new System.Drawing.Point(0, 0);
            tblExtra.Margin = new System.Windows.Forms.Padding(0);
            tblExtra.Name = "tblExtra";
            tblExtra.RowCount = 5;
            tblExtra.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tblExtra.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tblExtra.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tblExtra.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tblExtra.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tblExtra.Size = new System.Drawing.Size(526, 336);
            tblExtra.TabIndex = 4;
            // 
            // lblChgLim
            // 
            lblChgLim.Anchor = System.Windows.Forms.AnchorStyles.Right;
            lblChgLim.AutoSize = true;
            lblChgLim.Location = new System.Drawing.Point(55, 4);
            lblChgLim.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            lblChgLim.Name = "lblChgLim";
            lblChgLim.Padding = new System.Windows.Forms.Padding(0, 0, 0, 3);
            lblChgLim.Size = new System.Drawing.Size(75, 18);
            lblChgLim.TabIndex = 2;
            lblChgLim.Text = "Charge limit:";
            // 
            // numChgLim
            // 
            this.numChgLim.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.numChgLim.Location = new System.Drawing.Point(132, 2);
            this.numChgLim.Margin = new System.Windows.Forms.Padding(2);
            this.numChgLim.Name = "numChgLim";
            this.numChgLim.Size = new System.Drawing.Size(50, 23);
            this.numChgLim.TabIndex = 3;
            this.numChgLim.ValueChanged += new System.EventHandler(this.numChargeLim_Changed);
            // 
            // lblPerfMode
            // 
            lblPerfMode.Anchor = System.Windows.Forms.AnchorStyles.Right;
            lblPerfMode.AutoSize = true;
            lblPerfMode.Location = new System.Drawing.Point(18, 34);
            lblPerfMode.Margin = new System.Windows.Forms.Padding(4, 3, 0, 3);
            lblPerfMode.Name = "lblPerfMode";
            lblPerfMode.Size = new System.Drawing.Size(112, 15);
            lblPerfMode.TabIndex = 4;
            lblPerfMode.Text = "Performance mode:";
            // 
            // cboPerfMode
            // 
            this.cboPerfMode.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.cboPerfMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboPerfMode.FormattingEnabled = true;
            this.cboPerfMode.Location = new System.Drawing.Point(133, 30);
            this.cboPerfMode.Name = "cboPerfMode";
            this.cboPerfMode.Size = new System.Drawing.Size(150, 23);
            this.cboPerfMode.TabIndex = 5;
            this.cboPerfMode.SelectedIndexChanged += new System.EventHandler(this.cboPerfMode_IndexChanged);
            // 
            // lblWinFnSwap
            // 
            lblWinFnSwap.Anchor = System.Windows.Forms.AnchorStyles.Right;
            lblWinFnSwap.AutoSize = true;
            lblWinFnSwap.Location = new System.Drawing.Point(3, 60);
            lblWinFnSwap.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            lblWinFnSwap.Name = "lblWinFnSwap";
            lblWinFnSwap.Padding = new System.Windows.Forms.Padding(0, 0, 0, 2);
            lblWinFnSwap.Size = new System.Drawing.Size(127, 17);
            lblWinFnSwap.TabIndex = 6;
            lblWinFnSwap.Text = "Swap Win and Fn keys:";
            // 
            // chkWinFnSwap
            // 
            this.chkWinFnSwap.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.chkWinFnSwap.AutoSize = true;
            this.chkWinFnSwap.Location = new System.Drawing.Point(133, 59);
            this.chkWinFnSwap.Name = "chkWinFnSwap";
            this.chkWinFnSwap.Size = new System.Drawing.Size(68, 19);
            this.chkWinFnSwap.TabIndex = 7;
            this.chkWinFnSwap.Text = "Enabled";
            this.chkWinFnSwap.UseVisualStyleBackColor = true;
            this.chkWinFnSwap.CheckedChanged += new System.EventHandler(this.chkWinFnSwap_Toggled);
            // 
            // lblKeyLight
            // 
            lblKeyLight.Anchor = System.Windows.Forms.AnchorStyles.Right;
            lblKeyLight.AutoSize = true;
            lblKeyLight.Location = new System.Drawing.Point(18, 99);
            lblKeyLight.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            lblKeyLight.Name = "lblKeyLight";
            lblKeyLight.Size = new System.Drawing.Size(112, 15);
            lblKeyLight.TabIndex = 8;
            lblKeyLight.Text = "Keyboard backlight:";
            // 
            // flwKeyLight
            // 
            flwKeyLight.AutoSize = true;
            flwKeyLight.Controls.Add(this.lblKeyLightLow);
            flwKeyLight.Controls.Add(this.tbKeyLight);
            flwKeyLight.Controls.Add(this.lblKeyLightHigh);
            flwKeyLight.Location = new System.Drawing.Point(130, 81);
            flwKeyLight.Margin = new System.Windows.Forms.Padding(0);
            flwKeyLight.Name = "flwKeyLight";
            flwKeyLight.Size = new System.Drawing.Size(225, 51);
            flwKeyLight.TabIndex = 10;
            // 
            // lblKeyLightLow
            // 
            this.lblKeyLightLow.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblKeyLightLow.AutoSize = true;
            this.lblKeyLightLow.Location = new System.Drawing.Point(3, 18);
            this.lblKeyLightLow.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.lblKeyLightLow.Name = "lblKeyLightLow";
            this.lblKeyLightLow.Size = new System.Drawing.Size(24, 15);
            this.lblKeyLightLow.TabIndex = 10;
            this.lblKeyLightLow.Text = "Off";
            // 
            // tbKeyLight
            // 
            this.tbKeyLight.Location = new System.Drawing.Point(30, 3);
            this.tbKeyLight.Maximum = 3;
            this.tbKeyLight.Name = "tbKeyLight";
            this.tbKeyLight.Size = new System.Drawing.Size(150, 45);
            this.tbKeyLight.TabIndex = 9;
            this.tbKeyLight.TickStyle = System.Windows.Forms.TickStyle.Both;
            this.tbKeyLight.Scroll += new System.EventHandler(this.tbKeyLight_Scroll);
            // 
            // lblKeyLightHigh
            // 
            this.lblKeyLightHigh.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblKeyLightHigh.AutoSize = true;
            this.lblKeyLightHigh.Location = new System.Drawing.Point(183, 18);
            this.lblKeyLightHigh.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.lblKeyLightHigh.Name = "lblKeyLightHigh";
            this.lblKeyLightHigh.Size = new System.Drawing.Size(39, 15);
            this.lblKeyLightHigh.TabIndex = 11;
            this.lblKeyLightHigh.Text = "Bright";
            // 
            // tblFCBottom
            // 
            tblFCBottom.AutoSize = true;
            tblFCBottom.ColumnCount = 3;
            tblFCBottom.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tblFCBottom.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tblFCBottom.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tblFCBottom.Controls.Add(this.chkFullBlast, 0, 0);
            tblFCBottom.Controls.Add(this.btnRevert, 1, 0);
            tblFCBottom.Controls.Add(this.btnApply, 2, 0);
            tblFCBottom.Dock = System.Windows.Forms.DockStyle.Fill;
            tblFCBottom.Location = new System.Drawing.Point(2, 369);
            tblFCBottom.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            tblFCBottom.Name = "tblFCBottom";
            tblFCBottom.RowCount = 1;
            tblFCBottom.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tblFCBottom.Size = new System.Drawing.Size(536, 29);
            tblFCBottom.TabIndex = 4;
            // 
            // chkFullBlast
            // 
            this.chkFullBlast.AutoSize = true;
            this.chkFullBlast.Location = new System.Drawing.Point(5, 5);
            this.chkFullBlast.Margin = new System.Windows.Forms.Padding(5, 5, 6, 5);
            this.chkFullBlast.Name = "chkFullBlast";
            this.chkFullBlast.Size = new System.Drawing.Size(73, 19);
            this.chkFullBlast.TabIndex = 1;
            this.chkFullBlast.Text = "Full Blast";
            this.chkFullBlast.UseVisualStyleBackColor = true;
            this.chkFullBlast.CheckedChanged += new System.EventHandler(this.chkFullBlast_Toggled);
            // 
            // btnRevert
            // 
            this.btnRevert.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnRevert.Location = new System.Drawing.Point(378, 2);
            this.btnRevert.Margin = new System.Windows.Forms.Padding(2, 2, 3, 2);
            this.btnRevert.Name = "btnRevert";
            this.btnRevert.Size = new System.Drawing.Size(75, 25);
            this.btnRevert.TabIndex = 5;
            this.btnRevert.Text = "Revert";
            this.btnRevert.UseVisualStyleBackColor = true;
            this.btnRevert.Click += new System.EventHandler(this.btnRevert_Click);
            // 
            // btnApply
            // 
            this.btnApply.Location = new System.Drawing.Point(458, 2);
            this.btnApply.Margin = new System.Windows.Forms.Padding(2, 2, 3, 2);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 25);
            this.btnApply.TabIndex = 4;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // flwStats
            // 
            flwStats.AutoSize = true;
            flwStats.Controls.Add(this.lblStatus);
            flwStats.Controls.Add(this.lblFanSpd);
            flwStats.Controls.Add(this.lblFanRPM);
            flwStats.Controls.Add(this.lblTemp);
            flwStats.Dock = System.Windows.Forms.DockStyle.Fill;
            flwStats.Location = new System.Drawing.Point(0, 398);
            flwStats.Margin = new System.Windows.Forms.Padding(0, 0, 0, 3);
            flwStats.Name = "flwStats";
            flwStats.Size = new System.Drawing.Size(540, 15);
            flwStats.TabIndex = 2;
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
            // tblMain
            // 
            this.tblMain.ColumnCount = 1;
            this.tblMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblMain.Controls.Add(tcMain, 0, 0);
            this.tblMain.Controls.Add(tblFCBottom, 0, 1);
            this.tblMain.Controls.Add(flwStats, 0, 2);
            this.tblMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblMain.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tblMain.Location = new System.Drawing.Point(0, 24);
            this.tblMain.Margin = new System.Windows.Forms.Padding(0);
            this.tblMain.Name = "tblMain";
            this.tblMain.RowCount = 3;
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblMain.Size = new System.Drawing.Size(540, 416);
            this.tblMain.TabIndex = 1;
            // 
            // MainWindow
            // 
            this.AcceptButton = this.btnApply;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.btnRevert;
            this.ClientSize = new System.Drawing.Size(540, 440);
            this.Controls.Add(this.tblMain);
            this.Controls.Add(menuStrip);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = menuStrip;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.Name = "MainWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainWindow_Closing);
            this.Load += new System.EventHandler(this.MainWindow_Load);
            menuStrip.ResumeLayout(false);
            menuStrip.PerformLayout();
            tcMain.ResumeLayout(false);
            tabFanControl.ResumeLayout(false);
            this.tblFanControl.ResumeLayout(false);
            this.tblFanControl.PerformLayout();
            this.tblCurve.ResumeLayout(false);
            this.tblCurve.PerformLayout();
            flwFanSelect.ResumeLayout(false);
            flwFanSelect.PerformLayout();
            tabOptions.ResumeLayout(false);
            tblExtra.ResumeLayout(false);
            tblExtra.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numChgLim)).EndInit();
            flwKeyLight.ResumeLayout(false);
            flwKeyLight.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tbKeyLight)).EndInit();
            tblFCBottom.ResumeLayout(false);
            tblFCBottom.PerformLayout();
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
        private System.Windows.Forms.ToolStripMenuItem tsiProfRename;
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
        private System.Windows.Forms.Label lblSpeed;
        private System.Windows.Forms.Label lblUpT;
        private System.Windows.Forms.Label lblDownT;
        private System.Windows.Forms.Label lblFanSel;
        private System.Windows.Forms.ComboBox cboFanSel;
        private System.Windows.Forms.Label lblProfSel;
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
    }
}
