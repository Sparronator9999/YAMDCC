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
            System.Windows.Forms.ToolStripMenuItem tsiExit;
            System.Windows.Forms.ToolStripMenuItem tsiOptions;
            System.Windows.Forms.ToolStripMenuItem tsiAppBtn;
            System.Windows.Forms.ToolStripSeparator sep3;
            System.Windows.Forms.ToolStripSeparator sep1;
            System.Windows.Forms.ToolStripMenuItem tsiSysStart;
            System.Windows.Forms.ToolStripMenuItem tsiHelp;
            System.Windows.Forms.ToolStripMenuItem tsiAbout;
            System.Windows.Forms.ToolStripMenuItem tsiSource;
            System.Windows.Forms.ContextMenuStrip TrayMenu;
            System.Windows.Forms.ToolStripSeparator sep2;
            System.Windows.Forms.ToolStripMenuItem tsiTrayExit;
            System.Windows.Forms.GroupBox grpHKeys;
            this.tsiEnabled = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiAppBtnConfEditor = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiAppBtnDefaultDisable = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiTrayMin = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiTrayClose = new System.Windows.Forms.ToolStripMenuItem();
            this.tsiTrayAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.tblHotKeys = new System.Windows.Forms.TableLayoutPanel();
            this.TrayIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.lblBindInProgress = new System.Windows.Forms.Label();
            menuStrip = new System.Windows.Forms.MenuStrip();
            tsiFile = new System.Windows.Forms.ToolStripMenuItem();
            tsiExit = new System.Windows.Forms.ToolStripMenuItem();
            tsiOptions = new System.Windows.Forms.ToolStripMenuItem();
            tsiAppBtn = new System.Windows.Forms.ToolStripMenuItem();
            sep3 = new System.Windows.Forms.ToolStripSeparator();
            sep1 = new System.Windows.Forms.ToolStripSeparator();
            tsiSysStart = new System.Windows.Forms.ToolStripMenuItem();
            tsiHelp = new System.Windows.Forms.ToolStripMenuItem();
            tsiAbout = new System.Windows.Forms.ToolStripMenuItem();
            tsiSource = new System.Windows.Forms.ToolStripMenuItem();
            TrayMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            sep2 = new System.Windows.Forms.ToolStripSeparator();
            tsiTrayExit = new System.Windows.Forms.ToolStripMenuItem();
            grpHKeys = new System.Windows.Forms.GroupBox();
            menuStrip.SuspendLayout();
            TrayMenu.SuspendLayout();
            grpHKeys.SuspendLayout();
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
            tsiExit});
            tsiFile.Name = "tsiFile";
            tsiFile.Size = new System.Drawing.Size(37, 20);
            tsiFile.Text = "&File";
            // 
            // tsiExit
            // 
            tsiExit.Name = "tsiExit";
            tsiExit.Size = new System.Drawing.Size(93, 22);
            tsiExit.Text = "E&xit";
            tsiExit.Click += new System.EventHandler(this.Exit);
            // 
            // tsiOptions
            // 
            tsiOptions.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsiEnabled,
            tsiAppBtn,
            sep3,
            this.tsiTrayMin,
            this.tsiTrayClose,
            sep1,
            tsiSysStart});
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
            this.tsiEnabled.Size = new System.Drawing.Size(244, 22);
            this.tsiEnabled.Text = "Enable hotkeys";
            // 
            // tsiAppBtn
            // 
            tsiAppBtn.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsiAppBtnConfEditor,
            this.tsiAppBtnDefaultDisable});
            tsiAppBtn.Name = "tsiAppBtn";
            tsiAppBtn.Size = new System.Drawing.Size(244, 22);
            tsiAppBtn.Text = "MSI Center shortcut key settings";
            // 
            // tsiAppBtnConfEditor
            // 
            this.tsiAppBtnConfEditor.Checked = true;
            this.tsiAppBtnConfEditor.CheckOnClick = true;
            this.tsiAppBtnConfEditor.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsiAppBtnConfEditor.Name = "tsiAppBtnConfEditor";
            this.tsiAppBtnConfEditor.Size = new System.Drawing.Size(249, 22);
            this.tsiAppBtnConfEditor.Text = "Open config editor when pressed";
            // 
            // tsiAppBtnDefaultDisable
            // 
            this.tsiAppBtnDefaultDisable.CheckOnClick = true;
            this.tsiAppBtnDefaultDisable.Name = "tsiAppBtnDefaultDisable";
            this.tsiAppBtnDefaultDisable.Size = new System.Drawing.Size(249, 22);
            this.tsiAppBtnDefaultDisable.Text = "Try to prevent default action";
            // 
            // sep3
            // 
            sep3.Name = "sep3";
            sep3.Size = new System.Drawing.Size(241, 6);
            // 
            // tsiTrayMin
            // 
            this.tsiTrayMin.Checked = true;
            this.tsiTrayMin.CheckOnClick = true;
            this.tsiTrayMin.CheckState = System.Windows.Forms.CheckState.Checked;
            this.tsiTrayMin.Name = "tsiTrayMin";
            this.tsiTrayMin.Size = new System.Drawing.Size(244, 22);
            this.tsiTrayMin.Text = "&Minimise to tray";
            // 
            // tsiTrayClose
            // 
            this.tsiTrayClose.CheckOnClick = true;
            this.tsiTrayClose.Name = "tsiTrayClose";
            this.tsiTrayClose.Size = new System.Drawing.Size(244, 22);
            this.tsiTrayClose.Text = "&Close to tray";
            // 
            // sep1
            // 
            sep1.Name = "sep1";
            sep1.Size = new System.Drawing.Size(241, 6);
            // 
            // tsiSysStart
            // 
            tsiSysStart.CheckOnClick = true;
            tsiSysStart.Name = "tsiSysStart";
            tsiSysStart.Size = new System.Drawing.Size(244, 22);
            tsiSysStart.Text = "&Start on boot (all users)";
            tsiSysStart.Click += new System.EventHandler(this.tsiSysStart_Click);
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
            tsiAbout.Size = new System.Drawing.Size(139, 22);
            tsiAbout.Text = "&About";
            tsiAbout.Click += new System.EventHandler(this.tsiAbout_Click);
            // 
            // tsiSource
            // 
            tsiSource.Name = "tsiSource";
            tsiSource.Size = new System.Drawing.Size(139, 22);
            tsiSource.Text = "Source &code";
            tsiSource.Click += new System.EventHandler(this.tsiSource_Click);
            // 
            // TrayMenu
            // 
            TrayMenu.ImageScalingSize = new System.Drawing.Size(20, 20);
            TrayMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsiTrayAbout,
            sep2,
            tsiTrayExit});
            TrayMenu.Name = "TrayMenu";
            TrayMenu.Size = new System.Drawing.Size(169, 54);
            // 
            // tsiTrayAbout
            // 
            this.tsiTrayAbout.Name = "tsiTrayAbout";
            this.tsiTrayAbout.Size = new System.Drawing.Size(168, 22);
            this.tsiTrayAbout.Text = "About YAMDCC...";
            this.tsiTrayAbout.Click += new System.EventHandler(this.tsiAbout_Click);
            // 
            // sep2
            // 
            sep2.Name = "sep2";
            sep2.Size = new System.Drawing.Size(165, 6);
            // 
            // tsiTrayExit
            // 
            tsiTrayExit.Name = "tsiTrayExit";
            tsiTrayExit.Size = new System.Drawing.Size(168, 22);
            tsiTrayExit.Text = "Exit";
            tsiTrayExit.Click += new System.EventHandler(this.Exit);
            // 
            // grpHKeys
            // 
            grpHKeys.Controls.Add(this.tblHotKeys);
            grpHKeys.Location = new System.Drawing.Point(0, 27);
            grpHKeys.Name = "grpHKeys";
            grpHKeys.Size = new System.Drawing.Size(624, 305);
            grpHKeys.TabIndex = 8;
            grpHKeys.TabStop = false;
            grpHKeys.Text = "Hotkeys";
            // 
            // tblHotKeys
            // 
            this.tblHotKeys.ColumnCount = 5;
            this.tblHotKeys.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 182F));
            this.tblHotKeys.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 134F));
            this.tblHotKeys.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblHotKeys.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblHotKeys.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblHotKeys.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tblHotKeys.Location = new System.Drawing.Point(3, 19);
            this.tblHotKeys.Name = "tblHotKeys";
            this.tblHotKeys.RowCount = 2;
            this.tblHotKeys.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tblHotKeys.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblHotKeys.Size = new System.Drawing.Size(618, 283);
            this.tblHotKeys.TabIndex = 7;
            // 
            // TrayIcon
            // 
            this.TrayIcon.ContextMenuStrip = TrayMenu;
            this.TrayIcon.Visible = true;
            this.TrayIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.TrayIcon_MouseDoubleClick);
            // 
            // lblBindInProgress
            // 
            this.lblBindInProgress.AutoSize = true;
            this.lblBindInProgress.Location = new System.Drawing.Point(12, 357);
            this.lblBindInProgress.Name = "lblBindInProgress";
            this.lblBindInProgress.Size = new System.Drawing.Size(33, 15);
            this.lblBindInProgress.TabIndex = 9;
            this.lblBindInProgress.Text = "False";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(624, 381);
            this.Controls.Add(this.lblBindInProgress);
            this.Controls.Add(grpHKeys);
            this.Controls.Add(menuStrip);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MainMenuStrip = menuStrip;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            menuStrip.ResumeLayout(false);
            menuStrip.PerformLayout();
            TrayMenu.ResumeLayout(false);
            grpHKeys.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.NotifyIcon TrayIcon;
        private System.Windows.Forms.TableLayoutPanel tblHotKeys;
        private System.Windows.Forms.ToolStripMenuItem tsiTrayMin;
        private System.Windows.Forms.ToolStripMenuItem tsiTrayClose;
        private System.Windows.Forms.ToolStripMenuItem tsiEnabled;
        private System.Windows.Forms.ToolStripMenuItem tsiAppBtnDefaultDisable;
        private System.Windows.Forms.ToolStripMenuItem tsiAppBtnConfEditor;
        private System.Windows.Forms.ToolStripMenuItem tsiTrayAbout;
        private System.Windows.Forms.Label lblBindInProgress;
    }
}
