// This file is part of YAMDCC (Yet Another MSI Dragon Center Clone).
// Copyright © Sparronator9999 and Contributors 2025.
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

namespace YAMDCC.HotkeyHandler
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.MenuStrip menuStrip;
            System.Windows.Forms.ToolStripMenuItem tsiFile;
            System.Windows.Forms.ToolStripMenuItem tsiOptions;
            System.Windows.Forms.ToolStripSeparator sep3;
            System.Windows.Forms.ToolStripSeparator sep1;
            System.Windows.Forms.ToolStripMenuItem tsiHelp;
            System.Windows.Forms.ToolStripMenuItem tsiAbout;
            System.Windows.Forms.ToolStripMenuItem tsiSource;
            System.Windows.Forms.ContextMenuStrip TrayMenu;
            System.Windows.Forms.ToolStripSeparator sep2;
            System.Windows.Forms.GroupBox grpHotkeys;
            System.Windows.Forms.TableLayoutPanel tblMain;
            this.tsiExit = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiEnabled = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiTrayMin = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiTrayClose = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiSysStart = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiTrayAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiShowConfig = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiTrayExit = new System.Windows.Forms.ToolStripMenuItem();
            this.tblHotkeys = new System.Windows.Forms.TableLayoutPanel();
            this.btnRevert = new System.Windows.Forms.Button();
            this.btnApply = new System.Windows.Forms.Button();
            this.TrayIcon = new System.Windows.Forms.NotifyIcon(this.components);
            menuStrip = new System.Windows.Forms.MenuStrip();
            tsiFile = new System.Windows.Forms.ToolStripMenuItem();
            tsiOptions = new System.Windows.Forms.ToolStripMenuItem();
            sep3 = new System.Windows.Forms.ToolStripSeparator();
            sep1 = new System.Windows.Forms.ToolStripSeparator();
            tsiHelp = new System.Windows.Forms.ToolStripMenuItem();
            tsiAbout = new System.Windows.Forms.ToolStripMenuItem();
            tsiSource = new System.Windows.Forms.ToolStripMenuItem();
            TrayMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            sep2 = new System.Windows.Forms.ToolStripSeparator();
            grpHotkeys = new System.Windows.Forms.GroupBox();
            tblMain = new System.Windows.Forms.TableLayoutPanel();
            menuStrip.SuspendLayout();
            TrayMenu.SuspendLayout();
            grpHotkeys.SuspendLayout();
            tblMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            menuStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            tsiFile,
            tsiOptions,
            tsiHelp});
            menuStrip.Location = new System.Drawing.Point(0, 0);
            menuStrip.Name = "menuStrip";
            menuStrip.Padding = new System.Windows.Forms.Padding(5, 2, 0, 2);
            menuStrip.Size = new System.Drawing.Size(624, 24);
            menuStrip.TabIndex = 1;
            menuStrip.Text = "menuStrip1";
            // 
            // tsiFile
            // 
            tsiFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsiExit});
            tsiFile.Name = "tsiFile";
            tsiFile.Size = new System.Drawing.Size(37, 20);
            tsiFile.Text = "&File";
            // 
            // tsiExit
            // 
            this.tsiExit.Name = "tsiExit";
            this.tsiExit.Size = new System.Drawing.Size(180, 22);
            this.tsiExit.Text = "E&xit";
            this.tsiExit.Click += new System.EventHandler(this.Exit);
            // 
            // tsiOptions
            // 
            tsiOptions.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsiEnabled,
            sep3,
            this.tsiTrayMin,
            this.tsiTrayClose,
            sep1,
            this.tsiSysStart});
            tsiOptions.Name = "tsiOptions";
            tsiOptions.Size = new System.Drawing.Size(61, 20);
            tsiOptions.Text = "&Options";
            // 
            // tsiEnabled
            // 
            this.tsiEnabled.Checked = true;
            this.tsiEnabled.CheckOnClick = true;
            this.tsiEnabled.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsiEnabled.Name = "tsiEnabled";
            this.tsiEnabled.Size = new System.Drawing.Size(196, 22);
            this.tsiEnabled.Text = "Enable hotkeys";
            // 
            // sep3
            // 
            sep3.Name = "sep3";
            sep3.Size = new System.Drawing.Size(193, 6);
            // 
            // tsiTrayMin
            // 
            this.tsiTrayMin.Checked = true;
            this.tsiTrayMin.CheckOnClick = true;
            this.tsiTrayMin.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsiTrayMin.Name = "tsiTrayMin";
            this.tsiTrayMin.Size = new System.Drawing.Size(196, 22);
            this.tsiTrayMin.Text = "&Minimise to tray";
            // 
            // tsiTrayClose
            // 
            this.tsiTrayClose.CheckOnClick = true;
            this.tsiTrayClose.Name = "tsiTrayClose";
            this.tsiTrayClose.Size = new System.Drawing.Size(196, 22);
            this.tsiTrayClose.Text = "&Close to tray";
            // 
            // sep1
            // 
            sep1.Name = "sep1";
            sep1.Size = new System.Drawing.Size(193, 6);
            // 
            // tsiSysStart
            // 
            this.tsiSysStart.CheckOnClick = true;
            this.tsiSysStart.Name = "tsiSysStart";
            this.tsiSysStart.Size = new System.Drawing.Size(196, 22);
            this.tsiSysStart.Text = "&Start on boot (all users)";
            this.tsiSysStart.Click += new System.EventHandler(this.tsiSysStart_Click);
            // 
            // tsiHelp
            // 
            tsiHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            tsiAbout,
            tsiSource});
            tsiHelp.Name = "tsiHelp";
            tsiHelp.Size = new System.Drawing.Size(44, 20);
            tsiHelp.Text = "&Help";
            // 
            // tsiAbout
            // 
            tsiAbout.Name = "tsiAbout";
            tsiAbout.Size = new System.Drawing.Size(180, 22);
            tsiAbout.Text = "&About";
            tsiAbout.Click += new System.EventHandler(this.tsiAbout_Click);
            // 
            // tsiSource
            // 
            tsiSource.Name = "tsiSource";
            tsiSource.Size = new System.Drawing.Size(180, 22);
            tsiSource.Text = "Source &code";
            tsiSource.Click += new System.EventHandler(this.tsiSource_Click);
            // 
            // TrayMenu
            // 
            TrayMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            TrayMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsiTrayAbout,
            sep2,
            this.tsiShowConfig,
            this.tsiTrayExit});
            TrayMenu.Name = "TrayMenu";
            TrayMenu.Size = new System.Drawing.Size(221, 98);
            // 
            // tsiTrayAbout
            // 
            this.tsiTrayAbout.Name = "tsiTrayAbout";
            this.tsiTrayAbout.Size = new System.Drawing.Size(220, 22);
            this.tsiTrayAbout.Text = "About YAMDCC...";
            this.tsiTrayAbout.Click += new System.EventHandler(this.tsiAbout_Click);
            // 
            // sep2
            // 
            sep2.Name = "sep2";
            sep2.Size = new System.Drawing.Size(217, 6);
            // 
            // tsiShowConfig
            // 
            this.tsiShowConfig.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tsiShowConfig.Name = "tsiShowConfig";
            this.tsiShowConfig.Size = new System.Drawing.Size(220, 22);
            this.tsiShowConfig.Text = "Show hotkey configurator";
            this.tsiShowConfig.Click += new System.EventHandler(this.ShowHotkeyConfig);
            // 
            // tsiTrayExit
            // 
            this.tsiTrayExit.Name = "tsiTrayExit";
            this.tsiTrayExit.Size = new System.Drawing.Size(220, 22);
            this.tsiTrayExit.Text = "Exit";
            this.tsiTrayExit.Click += new System.EventHandler(this.Exit);
            // 
            // grpHotkeys
            // 
            tblMain.SetColumnSpan(grpHotkeys, 3);
            grpHotkeys.Controls.Add(this.tblHotkeys);
            grpHotkeys.Dock = System.Windows.Forms.DockStyle.Fill;
            grpHotkeys.Location = new System.Drawing.Point(3, 3);
            grpHotkeys.Name = "grpHotkeys";
            grpHotkeys.Size = new System.Drawing.Size(618, 322);
            grpHotkeys.TabIndex = 8;
            grpHotkeys.TabStop = false;
            grpHotkeys.Text = "Hotkeys";
            // 
            // tblHotkeys
            // 
            this.tblHotkeys.ColumnCount = 5;
            this.tblHotkeys.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 182F));
            this.tblHotkeys.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 134F));
            this.tblHotkeys.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblHotkeys.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblHotkeys.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblHotkeys.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblHotkeys.Location = new System.Drawing.Point(3, 19);
            this.tblHotkeys.Name = "tblHotkeys";
            this.tblHotkeys.RowCount = 1;
            this.tblHotkeys.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblHotkeys.Size = new System.Drawing.Size(612, 300);
            this.tblHotkeys.TabIndex = 7;
            // 
            // tblMain
            // 
            tblMain.ColumnCount = 3;
            tblMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tblMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tblMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            tblMain.Controls.Add(grpHotkeys, 0, 0);
            tblMain.Controls.Add(this.btnRevert, 1, 1);
            tblMain.Controls.Add(this.btnApply, 2, 1);
            tblMain.Dock = System.Windows.Forms.DockStyle.Fill;
            tblMain.Location = new System.Drawing.Point(0, 24);
            tblMain.Name = "tblMain";
            tblMain.RowCount = 2;
            tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            tblMain.RowStyles.Add(new System.Windows.Forms.RowStyle());
            tblMain.Size = new System.Drawing.Size(624, 357);
            tblMain.TabIndex = 10;
            // 
            // btnRevert
            // 
            this.btnRevert.Location = new System.Drawing.Point(465, 331);
            this.btnRevert.Name = "btnRevert";
            this.btnRevert.Size = new System.Drawing.Size(75, 23);
            this.btnRevert.TabIndex = 10;
            this.btnRevert.Text = "Revert";
            this.btnRevert.UseVisualStyleBackColor = true;
            this.btnRevert.Click += new System.EventHandler(this.btnRevert_Click);
            // 
            // btnApply
            // 
            this.btnApply.Location = new System.Drawing.Point(546, 331);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 23);
            this.btnApply.TabIndex = 11;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // TrayIcon
            // 
            this.TrayIcon.ContextMenuStrip = TrayMenu;
            this.TrayIcon.Visible = true;
            this.TrayIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.ShowHotkeyConfig);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(624, 381);
            this.Controls.Add(tblMain);
            this.Controls.Add(menuStrip);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MainMenuStrip = menuStrip;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            menuStrip.ResumeLayout(false);
            menuStrip.PerformLayout();
            TrayMenu.ResumeLayout(false);
            grpHotkeys.ResumeLayout(false);
            tblMain.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NotifyIcon TrayIcon;
        private System.Windows.Forms.TableLayoutPanel tblHotkeys;
        private System.Windows.Forms.ToolStripMenuItem tsiTrayMin;
        private System.Windows.Forms.ToolStripMenuItem tsiTrayClose;
        private System.Windows.Forms.ToolStripMenuItem tsiEnabled;
        private System.Windows.Forms.ToolStripMenuItem tsiTrayAbout;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.ToolStripMenuItem tsiShowConfig;
        private System.Windows.Forms.Button btnRevert;
        private System.Windows.Forms.ToolStripMenuItem tsiExit;
        private System.Windows.Forms.ToolStripMenuItem tsiSysStart;
        private System.Windows.Forms.ToolStripMenuItem tsiTrayExit;
    }
}
