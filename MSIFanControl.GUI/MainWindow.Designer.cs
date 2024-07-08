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
            System.Windows.Forms.MenuStrip menuStrip;
            System.Windows.Forms.ToolStripMenuItem tsiFile;
            System.Windows.Forms.ToolStripSeparator sep1;
            System.Windows.Forms.ToolStripSeparator sep2;
            System.Windows.Forms.ToolStripMenuItem tsiOptions;
            System.Windows.Forms.ToolStripMenuItem tsiHelp;
            System.Windows.Forms.TableLayoutPanel tblMain;
            System.Windows.Forms.FlowLayoutPanel flwFanSelect;
            System.Windows.Forms.Label lblFanSel;
            System.Windows.Forms.Label lblProfSel;
            System.Windows.Forms.FlowLayoutPanel flwStats;
            System.Windows.Forms.TableLayoutPanel tblExtras;
            System.Windows.Forms.FlowLayoutPanel flwExtras;
            System.Windows.Forms.Label lblExtra;
            System.Windows.Forms.Label lblChgLim;
            System.Windows.Forms.Label lblSpeed;
            System.Windows.Forms.Label lblUpT;
            System.Windows.Forms.Label lblDownT;
            this.tsiLoadConf = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiSaveConf = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiApply = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiRevert = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiExit = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiProfAdd = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiProfEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiProfRename = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiProfChangeDesc = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsiProfDel = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsiECMon = new System.Windows.Forms.ToolStripMenuItem();
            this.sep3 = new System.Windows.Forms.ToolStripSeparator();
            this.tsiUninstall = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiSource = new System.Windows.Forms.ToolStripMenuItem();
            this.cboFanSel = new System.Windows.Forms.ComboBox();
            this.cboProfSel = new System.Windows.Forms.ComboBox();
            this.btnProfAdd = new System.Windows.Forms.Button();
            this.btnProfDel = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblFanSpd = new System.Windows.Forms.Label();
            this.lblFanRPM = new System.Windows.Forms.Label();
            this.lblTemp = new System.Windows.Forms.Label();
            this.chkFullBlast = new System.Windows.Forms.CheckBox();
            this.numChgLim = new System.Windows.Forms.NumericUpDown();
            this.btnApply = new System.Windows.Forms.Button();
            this.btnRevert = new System.Windows.Forms.Button();
            this.tblCurve = new System.Windows.Forms.TableLayoutPanel();
            this.tmrPoll = new System.Windows.Forms.Timer(this.components);
            menuStrip = new System.Windows.Forms.MenuStrip();
            tsiFile = new System.Windows.Forms.ToolStripMenuItem();
            sep1 = new System.Windows.Forms.ToolStripSeparator();
            sep2 = new System.Windows.Forms.ToolStripSeparator();
            tsiOptions = new System.Windows.Forms.ToolStripMenuItem();
            tsiHelp = new System.Windows.Forms.ToolStripMenuItem();
            tblMain = new System.Windows.Forms.TableLayoutPanel();
            flwFanSelect = new System.Windows.Forms.FlowLayoutPanel();
            lblFanSel = new System.Windows.Forms.Label();
            lblProfSel = new System.Windows.Forms.Label();
            flwStats = new System.Windows.Forms.FlowLayoutPanel();
            tblExtras = new System.Windows.Forms.TableLayoutPanel();
            flwExtras = new System.Windows.Forms.FlowLayoutPanel();
            lblExtra = new System.Windows.Forms.Label();
            lblChgLim = new System.Windows.Forms.Label();
            lblSpeed = new System.Windows.Forms.Label();
            lblUpT = new System.Windows.Forms.Label();
            lblDownT = new System.Windows.Forms.Label();
            menuStrip.SuspendLayout();
            tblMain.SuspendLayout();
            flwFanSelect.SuspendLayout();
            flwStats.SuspendLayout();
            tblExtras.SuspendLayout();
            flwExtras.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numChgLim)).BeginInit();
            this.tblCurve.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            menuStrip.BackColor = System.Drawing.Color.WhiteSmoke;
            menuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            tsiFile,
            tsiOptions,
            tsiHelp});
            menuStrip.Location = new System.Drawing.Point(0, 0);
            menuStrip.Name = "menuStrip";
            menuStrip.Size = new System.Drawing.Size(540, 24);
            menuStrip.TabIndex = 0;
            // 
            // tsiFile
            // 
            tsiFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsiLoadConf,
            this.tsiSaveConf,
            sep1,
            this.tsiApply,
            this.tsiRevert,
            sep2,
            this.tsiExit});
            tsiFile.Name = "tsiFile";
            tsiFile.Size = new System.Drawing.Size(37, 20);
            tsiFile.Text = "File";
            // 
            // tsiLoadConf
            // 
            this.tsiLoadConf.Name = "tsiLoadConf";
            this.tsiLoadConf.Size = new System.Drawing.Size(154, 22);
            this.tsiLoadConf.Text = "Load config";
            this.tsiLoadConf.Click += new System.EventHandler(this.LoadConfClick);
            // 
            // tsiSaveConf
            // 
            this.tsiSaveConf.Name = "tsiSaveConf";
            this.tsiSaveConf.Size = new System.Drawing.Size(154, 22);
            this.tsiSaveConf.Text = "Save config";
            this.tsiSaveConf.Click += new System.EventHandler(this.SaveConfClick);
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
            // 
            // tsiRevert
            // 
            this.tsiRevert.Name = "tsiRevert";
            this.tsiRevert.Size = new System.Drawing.Size(154, 22);
            this.tsiRevert.Text = "Revert changes";
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
            this.tsiExit.Click += new System.EventHandler(this.tsiExitClick);
            // 
            // tsiOptions
            // 
            tsiOptions.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsiProfAdd,
            this.tsiProfEdit,
            this.toolStripMenuItem2,
            this.tsiECMon,
            this.sep3,
            this.tsiUninstall});
            tsiOptions.Name = "tsiOptions";
            tsiOptions.Size = new System.Drawing.Size(61, 20);
            tsiOptions.Text = "Options";
            // 
            // tsiProfAdd
            // 
            this.tsiProfAdd.Name = "tsiProfAdd";
            this.tsiProfAdd.Size = new System.Drawing.Size(192, 22);
            this.tsiProfAdd.Text = "New fan profile...";
            this.tsiProfAdd.Click += new System.EventHandler(this.tsiProfAdd_Click);
            // 
            // tsiProfEdit
            // 
            this.tsiProfEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsiProfRename,
            this.tsiProfChangeDesc,
            this.toolStripMenuItem1,
            this.tsiProfDel});
            this.tsiProfEdit.Name = "tsiProfEdit";
            this.tsiProfEdit.Size = new System.Drawing.Size(192, 22);
            this.tsiProfEdit.Text = "Edit current fan profile";
            // 
            // tsiProfRename
            // 
            this.tsiProfRename.Name = "tsiProfRename";
            this.tsiProfRename.Size = new System.Drawing.Size(180, 22);
            this.tsiProfRename.Text = "Change Name";
            this.tsiProfRename.Click += new System.EventHandler(this.tsiProfRename_Click);
            // 
            // tsiProfChangeDesc
            // 
            this.tsiProfChangeDesc.Name = "tsiProfChangeDesc";
            this.tsiProfChangeDesc.Size = new System.Drawing.Size(180, 22);
            this.tsiProfChangeDesc.Text = "Change Description";
            this.tsiProfChangeDesc.Click += new System.EventHandler(this.tsiProfChangeDesc_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(177, 6);
            // 
            // tsiProfDel
            // 
            this.tsiProfDel.Name = "tsiProfDel";
            this.tsiProfDel.Size = new System.Drawing.Size(180, 22);
            this.tsiProfDel.Text = "Delete";
            this.tsiProfDel.Click += new System.EventHandler(this.tsiProfDel_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(189, 6);
            // 
            // tsiECMon
            // 
            this.tsiECMon.CheckOnClick = true;
            this.tsiECMon.Name = "tsiECMon";
            this.tsiECMon.Size = new System.Drawing.Size(192, 22);
            this.tsiECMon.Text = "Enable EC monitoring";
            this.tsiECMon.Click += new System.EventHandler(this.tsiECMonClick);
            // 
            // sep3
            // 
            this.sep3.Name = "sep3";
            this.sep3.Size = new System.Drawing.Size(189, 6);
            // 
            // tsiUninstall
            // 
            this.tsiUninstall.Name = "tsiUninstall";
            this.tsiUninstall.Size = new System.Drawing.Size(192, 22);
            this.tsiUninstall.Text = "Uninstall service";
            this.tsiUninstall.Click += new System.EventHandler(this.tsiUninstallClick);
            // 
            // tsiHelp
            // 
            tsiHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsiAbout,
            this.tsiSource});
            tsiHelp.Name = "tsiHelp";
            tsiHelp.Size = new System.Drawing.Size(44, 20);
            tsiHelp.Text = "Help";
            // 
            // tsiAbout
            // 
            this.tsiAbout.Name = "tsiAbout";
            this.tsiAbout.Size = new System.Drawing.Size(141, 22);
            this.tsiAbout.Text = "About";
            this.tsiAbout.Click += new System.EventHandler(this.tsiAboutClick);
            // 
            // tsiSource
            // 
            this.tsiSource.Name = "tsiSource";
            this.tsiSource.Size = new System.Drawing.Size(141, 22);
            this.tsiSource.Text = "Source Code";
            this.tsiSource.Click += new System.EventHandler(this.tsiSrcClick);
            // 
            // tblMain
            // 
            tblMain.ColumnCount = 1;
            tblMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tblMain.Controls.Add(flwFanSelect, 0, 0);
            tblMain.Controls.Add(flwStats, 0, 3);
            tblMain.Controls.Add(tblExtras, 0, 2);
            tblMain.Controls.Add(this.tblCurve, 0, 1);
            tblMain.Dock = System.Windows.Forms.DockStyle.Fill;
            tblMain.Location = new System.Drawing.Point(0, 24);
            tblMain.Name = "tblMain";
            tblMain.RowCount = 4;
            tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 29F));
            tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
            tblMain.Size = new System.Drawing.Size(540, 396);
            tblMain.TabIndex = 1;
            // 
            // flwFanSelect
            // 
            flwFanSelect.AutoSize = true;
            flwFanSelect.Controls.Add(lblFanSel);
            flwFanSelect.Controls.Add(this.cboFanSel);
            flwFanSelect.Controls.Add(lblProfSel);
            flwFanSelect.Controls.Add(this.cboProfSel);
            flwFanSelect.Controls.Add(this.btnProfAdd);
            flwFanSelect.Controls.Add(this.btnProfDel);
            flwFanSelect.Dock = System.Windows.Forms.DockStyle.Fill;
            flwFanSelect.Location = new System.Drawing.Point(0, 0);
            flwFanSelect.Margin = new System.Windows.Forms.Padding(0);
            flwFanSelect.Name = "flwFanSelect";
            flwFanSelect.Size = new System.Drawing.Size(540, 29);
            flwFanSelect.TabIndex = 0;
            // 
            // lblFanSel
            // 
            lblFanSel.AutoSize = true;
            lblFanSel.Location = new System.Drawing.Point(3, 7);
            lblFanSel.Margin = new System.Windows.Forms.Padding(3, 7, 0, 3);
            lblFanSel.Name = "lblFanSel";
            lblFanSel.Size = new System.Drawing.Size(29, 15);
            lblFanSel.TabIndex = 0;
            lblFanSel.Text = "Fan:";
            // 
            // cboFanSel
            // 
            this.cboFanSel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboFanSel.FormattingEnabled = true;
            this.cboFanSel.Location = new System.Drawing.Point(35, 3);
            this.cboFanSel.Name = "cboFanSel";
            this.cboFanSel.Size = new System.Drawing.Size(121, 23);
            this.cboFanSel.TabIndex = 1;
            this.cboFanSel.SelectedIndexChanged += new System.EventHandler(this.FanSelIndexChanged);
            // 
            // lblProfSel
            // 
            lblProfSel.AutoSize = true;
            lblProfSel.Location = new System.Drawing.Point(162, 7);
            lblProfSel.Margin = new System.Windows.Forms.Padding(3, 7, 0, 3);
            lblProfSel.Name = "lblProfSel";
            lblProfSel.Size = new System.Drawing.Size(44, 15);
            lblProfSel.TabIndex = 2;
            lblProfSel.Text = "Profile:";
            // 
            // cboProfSel
            // 
            this.cboProfSel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboProfSel.FormattingEnabled = true;
            this.cboProfSel.Location = new System.Drawing.Point(209, 3);
            this.cboProfSel.Name = "cboProfSel";
            this.cboProfSel.Size = new System.Drawing.Size(121, 23);
            this.cboProfSel.TabIndex = 3;
            this.cboProfSel.SelectedIndexChanged += new System.EventHandler(this.ProfSelIndexChanged);
            // 
            // btnProfAdd
            // 
            this.btnProfAdd.Location = new System.Drawing.Point(334, 3);
            this.btnProfAdd.Margin = new System.Windows.Forms.Padding(1, 3, 1, 3);
            this.btnProfAdd.Name = "btnProfAdd";
            this.btnProfAdd.Size = new System.Drawing.Size(23, 23);
            this.btnProfAdd.TabIndex = 4;
            this.btnProfAdd.Text = "+";
            this.btnProfAdd.UseVisualStyleBackColor = true;
            this.btnProfAdd.Click += new System.EventHandler(this.btnProfAdd_Click);
            // 
            // btnProfDel
            // 
            this.btnProfDel.Location = new System.Drawing.Point(359, 3);
            this.btnProfDel.Margin = new System.Windows.Forms.Padding(1, 3, 1, 3);
            this.btnProfDel.Name = "btnProfDel";
            this.btnProfDel.Size = new System.Drawing.Size(23, 23);
            this.btnProfDel.TabIndex = 5;
            this.btnProfDel.Text = "-";
            this.btnProfDel.UseVisualStyleBackColor = true;
            this.btnProfDel.Click += new System.EventHandler(this.btnProfDel_Click);
            // 
            // flwStats
            // 
            flwStats.BackColor = System.Drawing.Color.WhiteSmoke;
            flwStats.Controls.Add(this.lblStatus);
            flwStats.Controls.Add(this.lblFanSpd);
            flwStats.Controls.Add(this.lblFanRPM);
            flwStats.Controls.Add(this.lblTemp);
            flwStats.Dock = System.Windows.Forms.DockStyle.Fill;
            flwStats.Location = new System.Drawing.Point(0, 373);
            flwStats.Margin = new System.Windows.Forms.Padding(0);
            flwStats.Name = "flwStats";
            flwStats.Size = new System.Drawing.Size(540, 23);
            flwStats.TabIndex = 2;
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(3, 3);
            this.lblStatus.Margin = new System.Windows.Forms.Padding(3);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(39, 15);
            this.lblStatus.TabIndex = 0;
            this.lblStatus.Text = "Ready";
            // 
            // lblFanSpd
            // 
            this.lblFanSpd.AutoSize = true;
            this.lblFanSpd.Location = new System.Drawing.Point(48, 3);
            this.lblFanSpd.Margin = new System.Windows.Forms.Padding(3);
            this.lblFanSpd.Name = "lblFanSpd";
            this.lblFanSpd.Size = new System.Drawing.Size(86, 15);
            this.lblFanSpd.TabIndex = 1;
            this.lblFanSpd.Text = "Fan speed: --%";
            this.lblFanSpd.Visible = false;
            // 
            // lblFanRPM
            // 
            this.lblFanRPM.AutoSize = true;
            this.lblFanRPM.Location = new System.Drawing.Point(140, 3);
            this.lblFanRPM.Margin = new System.Windows.Forms.Padding(3);
            this.lblFanRPM.Name = "lblFanRPM";
            this.lblFanRPM.Size = new System.Drawing.Size(58, 15);
            this.lblFanRPM.TabIndex = 2;
            this.lblFanRPM.Text = "RPM: ----";
            this.lblFanRPM.Visible = false;
            // 
            // lblTemp
            // 
            this.lblTemp.AutoSize = true;
            this.lblTemp.Location = new System.Drawing.Point(204, 3);
            this.lblTemp.Margin = new System.Windows.Forms.Padding(3);
            this.lblTemp.Name = "lblTemp";
            this.lblTemp.Size = new System.Drawing.Size(65, 15);
            this.lblTemp.TabIndex = 3;
            this.lblTemp.Text = "Temp: --°C";
            this.lblTemp.Visible = false;
            // 
            // tblExtras
            // 
            tblExtras.AutoSize = true;
            tblExtras.ColumnCount = 3;
            tblExtras.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tblExtras.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tblExtras.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tblExtras.Controls.Add(flwExtras, 0, 0);
            tblExtras.Controls.Add(this.btnApply, 2, 0);
            tblExtras.Controls.Add(this.btnRevert, 1, 0);
            tblExtras.Dock = System.Windows.Forms.DockStyle.Fill;
            tblExtras.Location = new System.Drawing.Point(0, 344);
            tblExtras.Margin = new System.Windows.Forms.Padding(0);
            tblExtras.Name = "tblExtras";
            tblExtras.RowCount = 1;
            tblExtras.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tblExtras.Size = new System.Drawing.Size(540, 29);
            tblExtras.TabIndex = 4;
            // 
            // flwExtras
            // 
            flwExtras.AutoSize = true;
            flwExtras.Controls.Add(lblExtra);
            flwExtras.Controls.Add(this.chkFullBlast);
            flwExtras.Controls.Add(lblChgLim);
            flwExtras.Controls.Add(this.numChgLim);
            flwExtras.Dock = System.Windows.Forms.DockStyle.Fill;
            flwExtras.Location = new System.Drawing.Point(0, 0);
            flwExtras.Margin = new System.Windows.Forms.Padding(0);
            flwExtras.Name = "flwExtras";
            flwExtras.Size = new System.Drawing.Size(378, 29);
            flwExtras.TabIndex = 1;
            // 
            // lblExtra
            // 
            lblExtra.AutoSize = true;
            lblExtra.Location = new System.Drawing.Point(3, 5);
            lblExtra.Margin = new System.Windows.Forms.Padding(3, 5, 0, 3);
            lblExtra.Name = "lblExtra";
            lblExtra.Size = new System.Drawing.Size(41, 15);
            lblExtra.TabIndex = 0;
            lblExtra.Text = "Extras:";
            // 
            // chkFullBlast
            // 
            this.chkFullBlast.AutoSize = true;
            this.chkFullBlast.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkFullBlast.Location = new System.Drawing.Point(47, 4);
            this.chkFullBlast.Margin = new System.Windows.Forms.Padding(3, 4, 3, 3);
            this.chkFullBlast.Name = "chkFullBlast";
            this.chkFullBlast.Size = new System.Drawing.Size(73, 19);
            this.chkFullBlast.TabIndex = 1;
            this.chkFullBlast.Text = "Full Blast";
            this.chkFullBlast.UseVisualStyleBackColor = true;
            this.chkFullBlast.CheckedChanged += new System.EventHandler(this.FullBlastToggled);
            // 
            // lblChgLim
            // 
            lblChgLim.AutoSize = true;
            lblChgLim.Location = new System.Drawing.Point(126, 5);
            lblChgLim.Margin = new System.Windows.Forms.Padding(3, 5, 0, 3);
            lblChgLim.Name = "lblChgLim";
            lblChgLim.Size = new System.Drawing.Size(75, 15);
            lblChgLim.TabIndex = 2;
            lblChgLim.Text = "Charge limit:";
            // 
            // numChgLim
            // 
            this.numChgLim.Location = new System.Drawing.Point(204, 3);
            this.numChgLim.Name = "numChgLim";
            this.numChgLim.Size = new System.Drawing.Size(60, 23);
            this.numChgLim.TabIndex = 3;
            this.numChgLim.ValueChanged += new System.EventHandler(this.ChargeLimChanged);
            // 
            // btnApply
            // 
            this.btnApply.Enabled = false;
            this.btnApply.Location = new System.Drawing.Point(462, 3);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 23);
            this.btnApply.TabIndex = 4;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.ApplyClick);
            // 
            // btnRevert
            // 
            this.btnRevert.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnRevert.Enabled = false;
            this.btnRevert.Location = new System.Drawing.Point(381, 3);
            this.btnRevert.Name = "btnRevert";
            this.btnRevert.Size = new System.Drawing.Size(75, 23);
            this.btnRevert.TabIndex = 5;
            this.btnRevert.Text = "Revert";
            this.btnRevert.UseVisualStyleBackColor = true;
            // 
            // tblCurve
            // 
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
            this.tblCurve.Controls.Add(lblSpeed, 0, 0);
            this.tblCurve.Controls.Add(lblUpT, 0, 2);
            this.tblCurve.Controls.Add(lblDownT, 0, 3);
            this.tblCurve.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblCurve.Location = new System.Drawing.Point(0, 29);
            this.tblCurve.Margin = new System.Windows.Forms.Padding(0);
            this.tblCurve.Name = "tblCurve";
            this.tblCurve.RowCount = 4;
            this.tblCurve.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblCurve.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblCurve.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblCurve.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblCurve.Size = new System.Drawing.Size(540, 315);
            this.tblCurve.TabIndex = 5;
            // 
            // lblSpeed
            // 
            lblSpeed.Anchor = System.Windows.Forms.AnchorStyles.Right;
            lblSpeed.AutoSize = true;
            lblSpeed.Location = new System.Drawing.Point(5, 0);
            lblSpeed.Name = "lblSpeed";
            lblSpeed.Size = new System.Drawing.Size(60, 15);
            lblSpeed.TabIndex = 0;
            lblSpeed.Text = "Speed (%)";
            // 
            // lblUpT
            // 
            lblUpT.Anchor = System.Windows.Forms.AnchorStyles.Right;
            lblUpT.AutoSize = true;
            lblUpT.Location = new System.Drawing.Point(19, 285);
            lblUpT.Name = "lblUpT";
            lblUpT.Size = new System.Drawing.Size(46, 15);
            lblUpT.TabIndex = 1;
            lblUpT.Text = "Up (°C)";
            // 
            // lblDownT
            // 
            lblDownT.Anchor = System.Windows.Forms.AnchorStyles.Right;
            lblDownT.AutoSize = true;
            lblDownT.Location = new System.Drawing.Point(3, 300);
            lblDownT.Name = "lblDownT";
            lblDownT.Size = new System.Drawing.Size(62, 15);
            lblDownT.TabIndex = 2;
            lblDownT.Text = "Down (°C)";
            // 
            // tmrPoll
            // 
            this.tmrPoll.Interval = 1000;
            this.tmrPoll.Tick += new System.EventHandler(this.tmrPollTick);
            // 
            // MainWindow
            // 
            this.AcceptButton = this.btnApply;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.btnRevert;
            this.ClientSize = new System.Drawing.Size(540, 420);
            this.Controls.Add(tblMain);
            this.Controls.Add(menuStrip);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MainMenuStrip = menuStrip;
            this.MaximizeBox = false;
            this.Name = "MainWindow";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "MSI Fan Control";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainWindowFormClosing);
            this.Load += new System.EventHandler(this.MainWindowLoad);
            menuStrip.ResumeLayout(false);
            menuStrip.PerformLayout();
            tblMain.ResumeLayout(false);
            tblMain.PerformLayout();
            flwFanSelect.ResumeLayout(false);
            flwFanSelect.PerformLayout();
            flwStats.ResumeLayout(false);
            flwStats.PerformLayout();
            tblExtras.ResumeLayout(false);
            tblExtras.PerformLayout();
            flwExtras.ResumeLayout(false);
            flwExtras.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numChgLim)).EndInit();
            this.tblCurve.ResumeLayout(false);
            this.tblCurve.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ToolStripMenuItem tsiECMon;
		private System.Windows.Forms.ComboBox cboFanSel;
		private System.Windows.Forms.ComboBox cboProfSel;
		private System.Windows.Forms.CheckBox chkFullBlast;
		private System.Windows.Forms.NumericUpDown numChgLim;
		private System.Windows.Forms.Button btnApply;
		private System.Windows.Forms.TableLayoutPanel tblCurve;
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
		private System.Windows.Forms.ToolStripSeparator sep3;
		private System.Windows.Forms.ToolStripMenuItem tsiUninstall;
		private System.Windows.Forms.Button btnProfAdd;
		private System.Windows.Forms.Button btnProfDel;
		private System.Windows.Forms.ToolStripMenuItem tsiProfAdd;
		private System.Windows.Forms.ToolStripMenuItem tsiProfEdit;
		private System.Windows.Forms.ToolStripMenuItem tsiProfRename;
		private System.Windows.Forms.ToolStripMenuItem tsiProfChangeDesc;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
		private System.Windows.Forms.ToolStripMenuItem tsiProfDel;
		private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
	}
}
